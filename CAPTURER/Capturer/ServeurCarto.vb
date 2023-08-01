Imports FCGP.NiveauDetailCartographique
Imports FCGP.SystemeCartographique

''' <summary> Décrit les caractéristiques d'un serveur Cartographique </summary>
Friend Class ServeurCarto
    Implements IDisposable
#Region "Visible"
    ''' <summary> nombre d'octects à lire pour une ligne de la tuile de fond </summary>
    Friend Const NbOctetsDecalXTuile As Integer = NbPixelsTuile * NbOctetsPixel
    ''' <summary> nombre d'octects d'une tuile de fond </summary>
    Friend Const PoidsTuile As Integer = NbOctetsDecalXTuile * NbPixelsTuile
    ''' <summary> Nb maximum de tuiles demandées en même temps par le http du serveur carto </summary>
    Friend Const MaxRequetesTuiles As Integer = 80
    ''' <summary> cache associé au serveur et au layer de fond de plan </summary>
    Friend ReadOnly Property Cache As CacheTuiles
    ''' <summary> nb de demande d'affichage encours </summary>
    Friend ReadOnly Property NbDemandesAffichage As Integer
        Get
            Return If(ListeDemandesAffichage Is Nothing, 0, ListeDemandesAffichage.Count)
        End Get
    End Property
    ''' <summary> nb de tuile concernées par les demande d'affichage encours </summary>
    Friend ReadOnly Property NbTuilesDemandeAffichage As Integer
        Get
            Return _NbTuilesDemandeAffichage
        End Get
    End Property
    ''' <summary> retourne la demande d'affichage ayant l'Id demandé </summary>
    Friend ReadOnly Property DemandesAffichage(Id As Integer) As DemandeTuiles
        Get
            Return ListeDemandesAffichage(Id)
        End Get
    End Property
    ''' <summary> nb de layers associé au serveur carto pour le téléchargement </summary>
    Friend ReadOnly Property NbLayers As Integer
    ''' <summary> nb requêtes encours associées à la ressource http </summary>
    Friend ReadOnly Property NbRequetes As Integer()
    ''' <summary> ressources http </summary>
    Friend ReadOnly Property RequeteHttp As HttpClient
    ''' <summary> Sitecarto associé au serveur Carto. permet d'obtenir les constantes associées </summary>
    Friend ReadOnly Property SiteCarto As SitesCartographiques
    ''' <summary> pointG décrivant le centre de la projection du site de capture </summary>
    Friend ReadOnly Property Centre() As PointG
    ''' <summary> RectangleD décrivant les limites de la projection du site de capture en DD ou GrilleSuisse</summary>
    Friend ReadOnly Property Limites() As RectangleD
    ''' <summary> Nb d'échelles ou de couches pour le layer </summary>
    ''' <param name="IndexLayer"> Layer demandé </param>
    Friend ReadOnly Property NbEchellesLayer(IndexLayer As Integer) As Integer
        Get
            Return Layers(IndexLayer).NbEchelles
        End Get
    End Property
    ''' <summary> Echelles du layer </summary>
    ''' <param name="IndexLayer"> Layer demandé </param>
    Friend ReadOnly Property EchellesLayer(IndexLayer As Integer) As String()
        Get
            Return Layers(IndexLayer).Echelles
        End Get
    End Property
    ''' <summary> Couches du tileMatrixSet du layer </summary>
    ''' <param name="IndexLayer"> Layer demandé </param>
    Friend ReadOnly Property CouchesLayer(IndexLayer As Integer) As Byte()
        Get
            Return Layers(IndexLayer).Couches
        End Get
    End Property
    ''' <summary> Coefficient Alpha (transparence) du layer </summary>
    ''' <param name="IndexLayer"> Layer demandé </param>
    Friend ReadOnly Property CoefAlphaLayer(IndexLayer As Integer) As Single
        Get
            Return Layers(IndexLayer).CoefAlpha
        End Get
    End Property
    ''' <summary> Nom du serveur </summary>
    Friend ReadOnly Property Nom As String
    ''' <summary> clef du site carto associé au serveur carto </summary>
    Friend ReadOnly Property Cle As String
    ''' <summary> datum du site carto associé au serveur carto </summary>
    Friend ReadOnly Property Datum As Datums
    ''' <summary> indique si l'initialisation du serveur carto s'est bien déroulée </summary>
    Friend ReadOnly Property IsOk As Boolean
    ''' <summary> indique si le site associé au serveur carto peut afficher les pentes </summary>
    Friend ReadOnly Property IsPentes As Boolean
    ''' <summary> indique si l'echelle associée au site du serveur carto peut afficher les pentes </summary>
    Friend ReadOnly Property IsIndiceEchellesPentes(IndiceEchelle As Integer) As Boolean
        Get
            Return Array.IndexOf(DonneesLayersPentes(_SiteCarto).IndicesEchelle, IndiceEchelle) > -1
        End Get
    End Property
    ''' <summary> N° de la couche sur le serveur pour le téléchargement des pentes. 1 couche unique pour 4 niveaux d'affichage </summary>
    Friend ReadOnly Property IndiceEchellePentes As Integer
    ''' <summary> Change le Fond de Plan affiché sur la Visue pour les sites ayant plusieurs fond de Plan d'affichage </summary>
    Friend Property IndiceFondPlan As Integer
        Get
            Return _IndiceFondPlan
        End Get
        Set(value As Integer)
            If value <> _IndiceFondPlan AndAlso value >= 0 AndAlso value <= NbLayersAffichages Then
                _IndiceFondPlan = value
                ChangerFondPlanAffichage()
            End If
        End Set
    End Property
    ''' <summary> renvoie un tableau décrivant les types de fond de plan d'affichage (Carte, Plan ou Photo) du serveur carto</summary>
    Friend ReadOnly Property TypesFondsPlan() As String()
        Get
            Dim Types(NbLayersAffichages) As String
            For Cpt As Integer = 0 To NbLayersAffichages
                Types(Cpt) = DonneesLayersAffichages(_SiteCarto)(Cpt).Type
            Next

            Return Types
        End Get
    End Property
    ''' <summary> indique si le site associé au serveurWMTS peut afficher plusieurs fond de plan </summary>
    Friend ReadOnly Property IsFondsPlan As Boolean
        Get
            Return NbLayersAffichages > 0
        End Get
    End Property
    ''' <summary> constructeur et vérifie que le serveur existe </summary>
    ''' <param name="Affichage"> Si le serveur est pour un affichage sinon null </param>
    ''' <param name="Site"> Site carto associé au serveur si différent de celui de capturerSettings </param>
    ''' <param name="Domtom"> DomTom associé au serveur si site différent de celui de CapturerSettings </param>
    ''' <param name="CheminCache">chemin du cache associé au serveur si différent de celui de capturerSetings </param>
    Friend Sub New(Optional Affichage As AffichageCarte = Nothing,
                   Optional Site As SitesCartographiques = SitesCartographiques.Aucun,
                   Optional DomTom As DomsToms = DomsToms.aucun, Optional Chemin As String = Nothing)
        Dim Titre = TitreInformation
        TitreInformation = "Information Serveur Carto"
#Region "définition des infos liées au serveur carto"
        _SiteCarto = If(Site = SitesCartographiques.Aucun, CapturerSettings.SITE, Site)
        _DomTom = If(Site = SitesCartographiques.Aucun, CapturerSettings.DOMTOM, DomTom)
        Url = DonneesSitesWebs(_SiteCarto).Url
        _Datum = DatumSiteWeb(_SiteCarto)
        Dim IndiceSite = If(Site = SitesCartographiques.Aucun, CapturerSettings.INDICE_SITE, SiteDomTomToIndex(Site, DomTom))
        _Centre = CentreSiteWeb(IndiceSite)
        _Limites = LimiteSiteWeb(IndiceSite)
        _Nom = SitesCartoLibelles(_SiteCarto)
        _Cle = SiteCartoClef(_SiteCarto)
        _IsPentes = DonneesLayersPentes(_SiteCarto).Identifer <> ""
        _IndiceEchellePentes = -1
        TypeTelechargement = Affichage Is Nothing
        InitialiserHttp()
#End Region
#Region "définition des infos liées aux layers"
        CheminCache = If(Chemin Is Nothing, CapturerSettings.CHEMIN_CACHE & "\", If(Chemin = "", Chemin, Chemin & "\"))
        _IndiceFondPlan = If(Site = SitesCartographiques.Aucun, CapturerSettings.INDICE_FOND_PLAN, 0)
        NbLayersAffichages = DonneesLayersAffichages(_SiteCarto).Length - 1
        TypeServeur = DonneesSitesWebs(_SiteCarto).TypeWMTS
        EPSG_Rendu = DonneesSitesWebs(_SiteCarto).EPSG
        Layer.InitialiserLayers(TypeServeur, DonneesSitesWebs(_SiteCarto).TileMatrixSet, EPSG_Rendu, DonneesSitesWebs(_SiteCarto).XY)
        Dim NbLayersMax As Integer = If(TypeTelechargement, 3, 1)
        ListeLayers = New SortedList(Of Integer, Layer)(NbLayersMax)
        If TypeTelechargement Then
            FichierLayersServeurs = CheminCache & "\LayersServeurs.xml"
            'on vérifie que le fichier CouchesServeur existe
            If Not File.Exists(FichierLayersServeurs) Then
                Using f As StreamWriter = File.CreateText(FichierLayersServeurs)
                    f.Write(LayersServeurs)
                End Using
            End If
            'on ajoute les Layers du fichier LayersServeur associés au serveur du site carto en testant qu'ils existent
            _IsOk = AjouterLayers(True)
        Else 'TypeAffichage
            'on ajoute le layer par défaut pour la visualisation
            _IsOk = AjouterLayersAffichage()
        End If
        If _IsOk Then
            _NbLayers = ListeLayers.Count
            Layers = ListeLayers.Values.ToArray
            ListeLayers.Clear()
            ListeLayers = Nothing
            ReDim _NbRequetes(_NbLayers)
            If Not TypeTelechargement Then
                _IsOk = InitialiserAffichage(Affichage)
                If _IsOk Then
#Region "définition des infos liées au layer Pentes"
                    'pour les pentes on prend la plus petite échelle afin de réduire au maximum le nb de tuiles à télécharger
                    If _IsPentes Then
                        _IndiceEchellePentes = DonneesLayersPentes(_SiteCarto).IndicesEchelle.Length - 1
                        CouchePentes = Layers(0).Couches(_IndiceEchellePentes)
                        LayerPentes = New Layer With {.Indice = -1, .Nom = "Pentes",
                                      .Identifier = DonneesLayersPentes(_SiteCarto).Identifer,
                                      .Style = DonneesLayersPentes(_SiteCarto).Style,
                                      .Format = DonneesLayersPentes(_SiteCarto).Format,
                                      .Time = DonneesLayersPentes(_SiteCarto).Time,
                                      .CoefAlpha = CoefsAlphasPentes(If(Site = SitesCartographiques.Aucun, CapturerSettings.INDICE_COEF_ALPHA_PENTES, 1))}
                        LayerPentes.Initialiser(Url)
                    End If
#End Region
                End If
            End If
        End If
#End Region
        TitreInformation = Titre
    End Sub
    ''' <summary> uri de la tuile que l'on veut demander au serveur. </summary>
    ''' <param name="IndexLayer"> Index de la couche dans le tableau des couches </param>
    ''' <param name="Numcol"> Numéro de colonne de la tuile </param>
    ''' <param name="Numrow"> Numéro de rangée de la tuile </param>
    ''' <param name="Couche"> Couche de la tuile </param>
    Friend Function UriTuileAffichage(IndexLayer As Integer, Couche As Byte, Numcol As Integer, Numrow As Integer) As String
        Dim Layer As Layer = Layers(IndexLayer)
        Dim UriTuile As String
        If TypeServeur = TypesServeur.OSM Then
            CptOSM += 1
            UriTuile = PorteSiteOSM(CptOSM Mod 3) & Layer.UriDeb
        Else
            UriTuile = Layer.UriDeb
        End If
        UriTuile &= Layer.CalculerUriTilePosition(Couche, Numcol, Numrow)
        Return UriTuile
    End Function
    ''' <summary> uri de la tuile des pentes que l'on veut demander au serveur. </summary>
    ''' <param name="Numcol"> Numéro de colonne de la tuile </param>
    ''' <param name="Numrow"> Numéro de rangée de la tuile </param>
    Friend Function UriTuilePentes(Numcol As Integer, Numrow As Integer) As String
        Dim UriTuile As String = LayerPentes.UriDeb & LayerPentes.CalculerUriTilePosition(CouchePentes, Numcol, Numrow)
        Return UriTuile
    End Function
    ''' <summary> renvoie une demande de tuile </summary>
    ''' <param name="NbTuiles"> Nb de tuiles de la demande </param>
    ''' <param name="Couche"> Couche du layer de fond de plan </param>
    Friend Function CreerDemandeAffichage(NbTuiles As Integer, Couche As Byte) As DemandeTuiles
        Dim Demande As New DemandeTuiles(NbTuiles, Couche, Me)
        'on ajoute la demande dans une liste cela permet de savoir si les taches sont terminées juste avec un test sur le nb d'éléments de la liste
        ListeDemandesAffichage.Add(Demande.ID, Demande)
        Return Demande
    End Function
    ''' <summary> Suppression d'une demande de tuiles. On stocke son id. On suppose qu'elle est finie </summary>
    Friend Sub SupprimerDemandeAffichage(ID As Integer)
        ListeDemandesAffichage.Remove(ID)
    End Sub
    ''' <summary> Teste l'url du serveur avec une demande des capacités </summary>
    Friend Async Sub TesterServeurAsync()
        Dim Titre = TitreInformation
        TitreInformation = "Information Serveur Carto"
        If _SiteCarto > SitesCartographiques.DomTom Then
            MessageInformation = $"Il n'y a qu'une seule couche de données pour ce site."
            AfficherInformation()
        Else
            MessageInformation = $"Votre demande a été prise en compte.{CrLf}Une information vous avertira de la fin du test."
            AfficherInformation()
            Dim NomCapacities As String = $"{CapturerSettings.CHEMIN_CACHE}\Capacites_{_Cle}.xml"
            If Await RequeteWebCapactiesAsync(0, NomCapacities) Then
                MessageInformation = $"Le fichier {NomCapacities} a été créé en réponse au test du serveur{CrLf}du site cartographique {_Nom}"
                AfficherInformation()
            Else
                MessageInformation = $"Le serveur du site cartographique {_Nom} n'est pas accessible"
                AfficherInformation()
            End If
        End If
        TitreInformation = Titre
    End Sub
    ''' <summary> donne le facteur d'agrandissement ou de réduction entre 2 valeurs d'échelle associées aux couches du TileMatrixSet du serveur </summary>
    ''' <param name="IndiceEchelle"> valeur actuelle de l'indiceEchelle </param>
    ''' <param name="DeltaIndicesEchelle"> différence d'indice d'échelle </param>
    Friend Function FacteurEchelles(IndiceEchelle As Integer, DeltaIndicesEchelle As Integer) As Double
        If DeltaIndicesEchelle = 0 Then
            FacteurEchelles = 1
        Else
            If _Datum = Datums.Web_Mercator Then
                FacteurEchelles = 2 ^ -DeltaIndicesEchelle
            Else ' Datums.Grille_Suisse_LV03. On est obligé de passer par la données Mètre par pixel
                Dim EchelleDepart As Echelles = SiteIndiceEchelleToEchelle(_SiteCarto, IndiceEchelle)
                Dim EchelleArrivee As Echelles = SiteIndiceEchelleToEchelle(_SiteCarto, IndiceEchelle + DeltaIndicesEchelle)
                FacteurEchelles = MetrePixels(EchelleDepart) / MetrePixels(EchelleArrivee)
            End If
        End If
    End Function
    ''' <summary> permet la libération des ressources http </summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        'nécessaire pour éviter que le GC ne redemande Finalize()
        GC.SuppressFinalize(Me)
    End Sub
#End Region
#Region "Privée"
#Region "Champs"
    ''' <summary> Liste des demandes de tuiles qui ne sont pas finies </summary>
    Private ListeDemandesAffichage As SortedList(Of Integer, DemandeTuiles)
    ''' <summary> Nb de tuiles correspondant à l'ensemble des demandes de tuiles pas encore finies </summary>
    Private _NbTuilesDemandeAffichage As Integer
    ''' <summary> Appelant de la création du serveur si ce n'est pas un serveur pour le téléchargement </summary>
    Private Affichage As AffichageCarte
    ''' <summary> chemin ou se situe le ou les caches tuiles </summary>
    Private CheminCache As String
    ''' <summary> indique si le serveur a été détruit </summary>
    Private disposed As Boolean
    ''' <summary> permet de change la porte d'accès des serveurs OSM</summary>
    Private CptOSM As Integer
    ''' <summary> indice de la couche d'affichage encours </summary>
    Private _IndiceFondPlan As Integer
    ''' <summary> Layers d'affichage acceptés pour le serveur. Stockage provisioire </summary>
    Private ReadOnly ListeLayers As SortedList(Of Integer, Layer)
    ''' <summary> Layers d'affichage acceptés pour le serveur </summary>
    Private ReadOnly Layers() As Layer
    ''' <summary> Layer des pentes accepté pour le serveur </summary>
    Private ReadOnly LayerPentes As Layer
    ''' <summary> Domtom associé au serveur de capture </summary>
    Private ReadOnly _DomTom As DomsToms
    ''' <summary> Addresse du serveur </summary>
    Private ReadOnly Url As String
    ''' <summary> indique qu'il s'agit d'un serveur dédié au téléchargement de carte </summary>
    Private ReadOnly TypeTelechargement As Boolean
    ''' <summary> couches de données possibles pour les serveurs carto </summary>
    Private ReadOnly FichierLayersServeurs As String
    ''' <summary> Numuméro de la couche du layer Pentes </summary>
    Private ReadOnly CouchePentes As Byte
    ''' <summary> Nb possible de layers pour l'affichage de 1 à 3 </summary>
    Private ReadOnly NbLayersAffichages As Integer
    ''' <summary> Type du serveur </summary>
    Private ReadOnly TypeServeur As TypesServeur
    ''' <summary> Type du serveur </summary>
    Private ReadOnly EPSG_Rendu As String
#End Region
#Region "Données des sites web"
    ''' <summary> Cle d'accès aux données IGN (GéoPortail) mais il est possible de la remplacer par sa propre clé
    ''' que l'on demande à IGN Professionnel pour avoir des données particulières et des services </summary>
    Private Const ClefIGN As String = "an7nvfzojv5wa96dsga5nk8w"
    ''' <summary> valeur defaut pour les UsersAgents </summary>
    Private Const UserAgent As String = "Mozilla/5.0 (Windows NT 5.1; rv: 42.0) Gecko/20100101 Firefox/42.0"
    ''' <summary> debut d'une définition de porte (accès) pour un site OSM </summary>
    Private Const DebutPorte As String = "https://"
    ''' <summary> 3 portes d'accès diffèrentes pour les serveur de type OSM </summary>
    Private Shared ReadOnly PorteSiteOSM As String() = New String() {DebutPorte & "a"c, DebutPorte & "b"c, DebutPorte & "c"c}
    ''' <summary> datas qui définit l'accés aux données correspondant aux sites web  </summary>
    Private Shared ReadOnly DonneesSitesWebs As (TileMatrixSet As String, TypeWMTS As TypesServeur, EPSG As String, XY As Boolean, Url As String, Referer As String, UserAgent As String)() =
            New(String, TypesServeur, String, Boolean, String, String, String)() {
                ("PM", TypesServeur.KVP, "", True, $"https://wxs.ign.fr/{ClefIGN}/geoportail/wmts", "https://www.geoportail.gouv.fr/carte", UserAgent),
                ("", TypesServeur.REST, "21781", False, "https://wmts100.geo.admin.ch", "https://map.geo.admin.ch", UserAgent),
                ("PM", TypesServeur.KVP, "", True, $"https://wxs.ign.fr/{ClefIGN}/geoportail/wmts", "https://www.geoportail.gouv.fr/carte", UserAgent),
                ("", TypesServeur.OSM, "", True, ".tile-cyclosm.openstreetmap.fr/cyclosm", "https://www.cyclosm.org", UserAgent),
                ("", TypesServeur.OSM, "", True, ".tile.opentopomap.org", "https://opentopomap.org", UserAgent),
                ("GoogleMapsCompatible", TypesServeur.KVP, "", True, "http://www.ign.es/wmts/mapa-raster", "https://www.ign.es", UserAgent),
                ("", TypesServeur.REST, "2056", True, "https://wmts100.geo.admin.ch", "https://map.geo.admin.ch", UserAgent)}
    ''' <summary> Data pour le ou les layers de l'affichage d'un site </summary>
    Private Shared ReadOnly DonneesLayersAffichages As (Identifer As String, Format As String, Style As String, Time As String, Type As String)()() =
            New(String, String, String, String, String)()() {
                New(String, String, String, String, String)() {("GEOGRAPHICALGRIDSYSTEMS.MAPS", "jpeg", "normal", "", "Cartes"),            ' ou "GEOGRAPHICALGRIDSYSTEMS.MAPS.SCAN-EXPRESS.STANDARD"
                                                               ("GEOGRAPHICALGRIDSYSTEMS.PLANIGNV2", "png", "normal", "", "Plans"),         ' avec format jpeg pour le plan
                                                               ("ORTHOIMAGERY.ORTHOPHOTOS", "jpeg", "normal", "", "Photos")},               'GF
                New(String, String, String, String, String)() {("ch.swisstopo.pixelkarte-farbe", "jpeg", "default", "current", "Cartes"),
                                                               ("ch.swisstopo.swissimage", "jpeg", "default", "current", "Photos")},        'SM
                New(String, String, String, String, String)() {("GEOGRAPHICALGRIDSYSTEMS.MAPS", "jpeg", "normal", "", "Cartes"),
                                                               ("GEOGRAPHICALGRIDSYSTEMS.PLANIGNV2", "png", "normal", "", "Plans"),
                                                               ("ORTHOIMAGERY.ORTHOPHOTOS", "jpeg", "normal", "", "Photos")},               'DT
                New(String, String, String, String, String)() {("", "png", "", "", "Cartes")},                                              'CY
                New(String, String, String, String, String)() {("", "png", "", "", "Cartes")},                                              'OT
                New(String, String, String, String, String)() {("MTN", "jpeg", "default", "", "Cartes")},                                   'ES
                New(String, String, String, String, String)() {("ch.swisstopo.pixelkarte-farbe", "jpeg", "default", "current", "Cartes")}}  'S5
    ''' <summary> Data pour la couche des pentes </summary>
    Private Shared ReadOnly DonneesLayersPentes As (Identifer As String, Format As String, Style As String, Time As String, IndicesEchelle As Integer())() =
            New(String, String, String, String, Integer())() {("GEOGRAPHICALGRIDSYSTEMS.SLOPES.MOUNTAIN", "png", "normal", "", New Integer() {0, 1, 2, 3}),     'GF
                                                              ("ch.swisstopo.hangneigung-ueber_30", "png", "default", "current", New Integer() {0, 1, 2, 3}),   'SM
                                                              ("", "", "", "", Array.Empty(Of Integer)()),                                                      'DT
                                                              ("", "", "", "", Array.Empty(Of Integer)()),                                                      'CY
                                                              ("", "", "", "", Array.Empty(Of Integer)()),                                                      'OT
                                                              ("", "", "", "", Array.Empty(Of Integer)()),                                                      'ES
                                                              ("ch.swisstopo.hangneigung-ueber_30", "png", "default", "current", New Integer() {0, 1, 2, 3})}   'S5
#End Region
    ''' <summary> permet de changer le layer des fonds de plan associé à l'affichage 
    ''' Attention après cette action il faut recréer un cache sinon les tuiles déjà enregistrées
    ''' dans le cache ne seront pas affectées </summary>
    Private Sub ChangerFondPlanAffichage()
        'd'abord on change le layer d'affichage
        Layers(0).Identifier = DonneesLayersAffichages(_SiteCarto)(_IndiceFondPlan).Identifer
        Layers(0).Format = DonneesLayersAffichages(_SiteCarto)(_IndiceFondPlan).Format
        Layers(0).Initialiser(Url)

        _Cache.Dispose()
        _Cache = Nothing
        _Cache = New CacheTuiles(Me)
    End Sub
    ''' <summary> quand il n'y a pas de layer à ajouter on ajoute le layer carte </summary>
    Private Function AjouterLayersAffichage() As Boolean
        Dim L As New Layer() With {.Indice = 0, .Nom = "Defaut",
                                   .Identifier = DonneesLayersAffichages(_SiteCarto)(_IndiceFondPlan).Identifer,
                                   .Style = DonneesLayersAffichages(_SiteCarto)(_IndiceFondPlan).Style,
                                   .Format = DonneesLayersAffichages(_SiteCarto)(_IndiceFondPlan).Format,
                                   .Time = DonneesLayersAffichages(_SiteCarto)(_IndiceFondPlan).Time, .CoefAlpha = 1}
        InitialiserLayer(L, False)
        Return ListeLayers.Count > 0
    End Function
    ''' <summary> remplit le tableau des serveurs connus à partir de ceux contenus dans un fichier XML </summary>
    ''' <param name="TestLayer"> Doit'on tester les couches. Si oui on teste l'existence de la couche au centre de la projection du site et à l'echelle définit </param>
    Private Function AjouterLayers(TestLayer As Boolean) As Boolean
        Dim Ret As Boolean = False
        Try
            Dim Cpt As Integer
            Dim ServeursXML As XElement = XElement.Load(FichierLayersServeurs)
            For Each ServeurXML As XElement In ServeursXML.Elements(XName.Get("Serveur"))
                Dim Fcgp As String = ServeurXML.Attribute(XName.Get("fcgp")).Value
                If Fcgp = _Cle Then
                    'on verifie les différents couches
                    Dim LayersXML As XElement = ServeurXML.Element(XName.Get("Layers"))
                    For Each LayerXML As XElement In LayersXML.Elements(XName.Get("Layer"))
                        Dim IndiceLayer As Integer = CInt(LayerXML.Attribute(XName.Get("indice")).Value)
                        If IndiceLayer > -1 And IndiceLayer < 3 Then
                            Dim Layer = New Layer With {.Indice = IndiceLayer,
                                                        .CoefAlpha = CSng(StrToDbl(LayerXML.Attribute(XName.Get("visibilite")).Value)),
                                                        .Nom = LayerXML.Element(XName.Get("Nom")).Value,
                                                        .Identifier = LayerXML.Attribute(XName.Get("identifiant")).Value,
                                                        .Format = LayerXML.Attribute(XName.Get("format")).Value,
                                                        .Style = LayerXML.Attribute(XName.Get("style")).Value,
                                                        .Time = LayerXML.Attribute(XName.Get("time")).Value}
                            InitialiserLayer(Layer, TestLayer)
                            Cpt += 1
                        End If
                    Next
                End If
            Next
            If ListeLayers.Count = 0 Then
                AjouterLayersAffichage()
                If _SiteCarto <= SitesCartographiques.DomTom AndAlso Cpt > 0 Then
                    MessageInformation = "Aucun des Layers du fichier LayersServeurs.xml" & CrLf & $"n'a été validé par le serveur{_Nom}" & CrLf &
                                         "Le Layer d'affichage sera utilisé pour le téléchargement"
                    AfficherInformation()
                End If
            End If
            Ret = ListeLayers.Count > 0
        Catch Ex As Exception
            AfficherErreur(Ex, "L3N5")
        End Try
        Return Ret
    End Function
    ''' <summary> initialise la couche et trouve les zooms associés </summary>
    ''' <param name="Layer"> Layer à tester </param>
    ''' <param name="FlagTestLayer"> indique si l'on doit tester si le layer existe sur le serveur </param>
    Private Sub InitialiserLayer(Layer As Layer, FlagTestLayer As Boolean)
        Layer.Initialiser(Url)
        Dim UriTuile As String = Layer.UriDeb & Layer.CalculerUriTilePosition(CouchesSite(_SiteCarto, _DomTom)(_Centre.IndiceEchelle), _Centre.IndexTuile.X, _Centre.IndexTuile.Y)
        'on provoque une erreur si le serveur ne réagit pas pas notre requête avant le délai indiqué
        If Not FlagTestLayer OrElse TesterLayer(UriTuile, Layer.Identifier) Then
            Layer.NbEchelles = NbEchellesSite(_SiteCarto, _DomTom) - 1
            Layer.Couches = CouchesSite(_SiteCarto, _DomTom)
            Layer.Echelles = EchellesSite(_SiteCarto, _DomTom)
            ListeLayers.Add(Layer.Indice, Layer)
        End If
    End Sub
    ''' <summary> initialise le cache et les demandes de tuiles pour l'affichage </summary>
    ''' <param name="Affichage"> Affichage qui demandera les demande de tuiles </param>
    Private Function InitialiserAffichage(Affichage As AffichageCarte) As Boolean
        _Cache = New CacheTuiles(Me)
        If Not _Cache.IsOk Then Return False
        Me.Affichage = Affichage
        ListeDemandesAffichage = New SortedList(Of Integer, DemandeTuiles)(50)
        Return True
    End Function
    ''' <summary> initialise le support des requêtes </summary>
    Private Sub InitialiserHttp()
        _RequeteHttp = New HttpClient() With {.Timeout = New TimeSpan(0, 0, 10)}
        _RequeteHttp.DefaultRequestHeaders.Clear()
        _RequeteHttp.DefaultRequestHeaders.Add("User-Agent", DonneesSitesWebs(_SiteCarto).UserAgent)
        _RequeteHttp.DefaultRequestHeaders.Add("Referer", DonneesSitesWebs(_SiteCarto).Referer)
    End Sub
    ''' <summary> teste si la couche existe sur le serveur et trouve les zooms associés </summary>
    Private Function TesterLayer(UriTuile As String, Identifier As String) As Boolean
        MessageInformation = Nothing
        Try
            Dim Cpt As Integer = 0
            Do
                Using Reponse As HttpResponseMessage = _RequeteHttp.Send(New HttpRequestMessage(HttpMethod.Get, UriTuile))
                    If Reponse.StatusCode = HttpStatusCode.OK Then
                        'pas de message tout est normal
                        Return True
                    Else
                        If Reponse.StatusCode = HttpStatusCode.Forbidden OrElse Reponse.StatusCode = HttpStatusCode.BadRequest Then
                            MessageInformation = $"Le Layer {Identifier} tel que défini dans{CrLf}le fichier LayersServeurs.xml n'existe pas sur le serveur {_Nom}"
                            Cpt = 3
                        Else
                            Cpt += 1
                        End If
                    End If
                End Using
            Loop While Cpt < 2
            If String.IsNullOrEmpty(MessageInformation) Then
                MessageInformation = $"Le Layer {Identifier} n'existe pas sur le serveur {_Nom}" & CrLf & "ou problème avec le serveur"
            End If
        Catch ex As Exception
            MessageInformation = $"Problème avec le serveur : {ex.Message}"
        End Try
        AfficherInformation()
        Return False
    End Function
    ''' <summary> demande les capacités du serveur </summary>
    ''' <param name="VersionWMTS"> dernier chiffre de la version WMTS. peut être à ou 1 </param>
    ''' <returns> une chaine ayant le format attendu en fonction des caractéristiques du serveur </returns>
    Private Function ServiceCapabilities(VersionWMTS As Integer) As String
        If TypeServeur = TypesServeur.KVP Then
            Return "?Service=WMTS&Request=GetCapabilities"
        Else
            Return $"/EPSG/{EPSG_Rendu}/1.0.{VersionWMTS}/WMTSCapabilities.xml"
        End If
    End Function
    ''' <summary> demande les capacités du serveur </summary>
    ''' <param name="VersionWMTS"> peut être 0 ou 1 mais 0 fonctionne très bien </param>
    ''' <returns> un Xml qui peut être utilisé pour déterminer les capacités du serveur ou nothing si la demande retourne une erreur </returns>
    Private Async Function RequeteWebCapactiesAsync(VersionWMTS As Integer, NomCapacities As String) As Task(Of Boolean)
        Try
            Using Reponse As HttpResponseMessage = Await _RequeteHttp.GetAsync(Url & ServiceCapabilities(VersionWMTS))
                If Reponse.StatusCode = HttpStatusCode.OK Then
                    Using S As Stream = Await Reponse.Content.ReadAsStreamAsync
                        Await Task.Run(Sub()
                                           Using X As XmlReader = XmlReader.Create(S)
                                               Dim CapacitiesXML As XElement = XElement.Load(X)
                                               CapacitiesXML.Save(NomCapacities)
                                           End Using
                                       End Sub)
                    End Using
                    Return True
                Else
                    MessageInformation = "Erreur lors de la requête WebCapacties" & CrLf & "Réessayez plus tard"
                    Dim Titre = TitreInformation
                    TitreInformation = "Information Serveur Carto"
                    AfficherInformation()
                    TitreInformation = Titre
                    Return False
                End If
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "V5B8")
            Return False
        End Try
    End Function
    ''' <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If disposed Then Exit Sub
        If disposing Then
            _RequeteHttp?.Dispose()
            _Cache?.Dispose()
        End If
        disposed = True
    End Sub
#End Region
End Class