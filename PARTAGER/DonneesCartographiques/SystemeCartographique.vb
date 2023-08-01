''' <summary> Définit un système cartographique associé à une carte.</summary>
Friend Class SystemeCartographique
#Region "Fonctions partagées"
    ''' <summary> renvoie un index unique qui prend en compte à la fois le site et le domtom </summary>
    ''' <param name="Site"> site cartographique </param>
    ''' <param name="Domtom"> domtom associé au site cartographique </param>
    Friend Shared Function SiteDomTomToIndex(Site As SitesCartographiques, Domtom As DomsToms) As Integer
        If Site < SitesCartographiques.DomTom Then          'GF, SM  : 0, 1
            Return Site
        ElseIf Site > SitesCartographiques.DomTom Then      'CY, OT, ES : 9, 10, 11
            Return Site + DomsTomsLibelles.Length - 1
        Else
            Return Site + Domtom                            'les doms-toms de 2 à 8
        End If
    End Function
    ''' <summary> transforme un index de liste déroulante des sites en un site carto et domtom correspondant au tableau LibellesSitesDomTom </summary>
    ''' <param name="IndexListe"> Index de la liste </param>
    Friend Shared Function IndexToSiteDomTom(IndexListe As Integer) As (Site As SitesCartographiques, DomTom As DomsToms)
        Dim Site As SitesCartographiques, DomTom As DomsToms
        If IndexListe < SitesCartographiques.DomTom Then '-1, 0, 1 --> -1, 0, 1 
            Site = CType(IndexListe, SitesCartographiques)
            DomTom = DomsToms.aucun
        ElseIf IndexListe >= SitesCartographiques.DomTom + DomsTomsLibelles.Length Then '9, 10, 11 --> 3, 4, 5
            Site = CType(IndexListe - DomsTomsLibelles.Length + 1, SitesCartographiques)
            DomTom = DomsToms.aucun
        Else
            Site = SitesCartographiques.DomTom
            DomTom = CType(IndexListe - 2, DomsToms)
        End If
        Return (Site, DomTom)
    End Function
    ''' <summary> Retourne l'indice du domtom inclus dans la cle du systèmecarto passé en paramètre </summary>
    ''' <param name="ClefSystemecarto">Site cartographique lu au niveau du fichier Géoref Ex: DomTom-reunion</param>
    Friend Shared Function TrouverDomTomCle(ClefSystemecarto As String) As DomsToms
        Dim Pos As Integer = ClefSystemecarto.IndexOf("-"c)
        'si il y a un '-' on peut supposer qu'il s'agit de la forme CleSite-ClefDomTom
        'sinon il s'agit de la forme ClefDomTom uniquement
        If Pos > -1 Then ClefSystemecarto = ClefSystemecarto.Substring(Pos + 1)
        Return CType(Array.IndexOf(DomsTomsClefs, ClefSystemecarto), DomsToms)
    End Function
    ''' <summary> Retourne l'indice du site carto inclus dans la clé du systèmecarto passé en paramètre </summary>
    ''' <param name="CleSystemeCarto">site cartographique lu au niveau du fichier Géoref</param>
    Friend Shared Function TrouverSiteCartographiqueCle(CleSystemeCarto As String) As SitesCartographiques
        'pour le site Domtom on élimine le nom du Domtom
        If CleSystemeCarto.Length > 2 Then CleSystemeCarto = CleSystemeCarto.Substring(0, 2)
        Return CType(Array.IndexOf(SitesCartoClefs, CleSystemeCarto), SitesCartographiques)
    End Function
    ''' <summary> indique si le site carto du système carto possède une grille incorporée </summary>
    ''' <param name="SiteCarto"></param>
    Friend Shared ReadOnly Property GrilleSiteExiste(SiteCarto As SitesCartographiques) As Boolean
        Get
            Return GrillesExistes(SiteCarto)
        End Get
    End Property
    ''' <summary> renvoie la cle associé au sitecarto passé en paramètre </summary>
    ''' <param name="SiteCarto"></param>
    Friend Shared ReadOnly Property SiteCartoClef(SiteCarto As SitesCartographiques) As String
        Get
            Return SitesCartoClefs(SiteCarto)
        End Get
    End Property
    ''' <summary> renvoie la cle associé au DomTom passé en paramètre </summary>
    ''' <param name="DomTom"></param>
    Friend Shared ReadOnly Property DomTomClef(DomTom As DomsToms) As String
        Get
            Return DomsTomsClefs(DomTom)
        End Get
    End Property
    ''' <summary> renvoie le numéro de zone et l'hémisphère de la projection utm pour le domtom passé en paramètre </summary>
    ''' <param name="DomTom"></param>
    Friend Shared Function ZoneUtmDomTom(DomTom As DomsToms) As String
        Return ZonesUtmDomTom(DomTom)
    End Function
#End Region
#Region "Constantes privées"
    ''' <summary>indique pour les différents Domstoms du site domTom la zone UTM principale</summary>
    Private Shared ReadOnly ZonesUtmDomTom() As String = New String() {"S40", "N20", "N20", "N20", "N22", "S38", "N20"}
    ''' <summary>indique le nom des grilles à faire apparaitre sur les références</summary>
    Private Shared ReadOnly GrillesNoms() As String = {"UTM WGS84", "SUISSE LV03", "UTM WGS84", "UTM WGS84", "UTM WGS84", "UTM WGS84"}
    ''' <summary>indique si les cartes du site ont déja la grille affichée</summary>
    Private Shared ReadOnly GrillesExistes() As Boolean = {False, True, False, False, False, False} 'GF,SM,DT,CY,OP,ES
    '''<summary>Pour adapter la taille du texte dans les références de grille</summary>
    Private Shared ReadOnly MaxsDPIsImpression() As Single = {340.0, 254.0, 340.0, 340.0, 340.0, 340.0}
#End Region
#Region "Fonctions et propriétés visibles"
    ''' <summary> constructeur pour une carte réalisée à partir de captures d'écran ou de téléchargement de tuiles
    ''' sur un serveur, sans interpolation (Fichier Georef avec Reatail à NON) </summary>
    ''' <param name="LibelleSystemeCarto"> libellé du système cartographique de la carte </param>
    ''' <param name="ClefEchelle"> cle de l'échelle de la carte </param>
    ''' <param name="LibelleProjection"> libellé de la projection des coordonnées de capture de la carte </param>
    ''' <param name="H_Utm"> si la projection est une projection UTM, hémisphère de la projection </param>
    ''' <param name="Z_Utm"> si la projection est une projection UTM, numéro de zone de la projection </param>
    Friend Sub New(LibelleSystemeCarto As String, ClefEchelle As String, LibelleProjection As String, Z_Utm As Integer, H_Utm As Char)
        _IsOk = False
        _SiteCarto = TrouverSiteCartographiqueLibelle(LibelleSystemeCarto)
        'il ne s'agit pas d'un site référencé
        If _SiteCarto = SitesCartographiques.Aucun Then Exit Sub
        _DomTom = TrouverDomTomLibelle(LibelleSystemeCarto)
        _IndiceSiteCarto = SiteDomTomToIndex(_SiteCarto, _DomTom)
        'le site et est le site de capture domtom mais il n'y a pas le domtom d'indiqué
        If _SiteCarto = SitesCartographiques.DomTom And _DomTom = DomsToms.aucun Then Exit Sub
        _IsInterpol = False
        _Niveau = New NiveauDetailCartographique(_SiteCarto, ClefEchelle)
        If Not _Niveau.IsOk Then Exit Sub
        _Projection = New ProjectionCartographique(_SiteCarto, LibelleProjection)
        If Not _Projection.IsOk Then Exit Sub
        If ProjectionCartographique.DatumPrincipal(_SiteCarto) = Datums.WGS84 Then
            _CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD
        Else
            _CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille
        End If
        _IsCapture = True
        _ZoneUtmReferencement = Z_Utm
        _HemisphereUtmReferencement = H_Utm
        'dans le cas du site de capture DOMTOM on met les valeurs par defaut (ancien fichier georef)
        If _DomTom > DomsToms.aucun And (Z_Utm = 0 Or _HemisphereUtmReferencement = Nothing) Then
            _HemisphereUtmReferencement = ZonesUtmDomTom(_DomTom).Chars(0)
            _ZoneUtmReferencement = CInt(ZonesUtmDomTom(_DomTom).Substring(1))
        End If
        _IsOk = True
    End Sub
    ''' <summary> constructeur pour les cartes issues d'un téléchargement de tuiles sur serveur carto. </summary>
    ''' <param name="SiteCarto"> site de téléchargement </param>
    ''' <param name="Domtom"> domtom de téléchargement </param>
    ''' <param name="Echelle"> echelle de téléchargement </param>
    ''' <param name="DatumProjection"> libellé de la projection du serveur </param>
    Friend Sub New(IndiceSite As Integer, Echelle As Echelles, DatumProjection As Datums)
        _IsOk = False
        _IndiceSiteCarto = IndiceSite
        Dim SiteDomTom = IndexToSiteDomTom(_IndiceSiteCarto)
        _SiteCarto = SiteDomTom.Site
        _IsInterpol = False
        _DomTom = SiteDomTom.DomTom
        _IndiceSiteCarto = SiteDomTomToIndex(_SiteCarto, _DomTom)
        _Niveau = New NiveauDetailCartographique(_SiteCarto, Echelle)
        If Not _Niveau.IsOk Then Exit Sub
        _Projection = New ProjectionCartographique(_SiteCarto, DatumProjection)
        If Not _Projection.IsOk Then Exit Sub
        If ProjectionCartographique.DatumPrincipal(_SiteCarto) = Datums.WGS84 Then
            _CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD
        Else
            _CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille
        End If
        _IsCapture = True
        'dans le cas du site de capture DOMTOM on met les valeurs par defaut (ancien fichier georef)
        If _SiteCarto = SitesCartographiques.DomTom And (_ZoneUtmReferencement = 0 OrElse _HemisphereUtmReferencement = Nothing) Then
            _HemisphereUtmReferencement = ZonesUtmDomTom(_DomTom).Chars(0)
            _ZoneUtmReferencement = CInt(ZonesUtmDomTom(_DomTom).Substring(1))
        End If
        _IsOk = True
    End Sub
    ''' <summary> constructeur pour un fichier georef issu d'une carte réalisée à partir de captures d'écran ou téléchargement de tuiles 
    ''' sur 1 serveur avec interpolation (Fichier Georef avec Reatail à OUI) ou à partir d'un scan avec ou sans interpolation </summary>
    ''' <param name="RegionUniteCarte"> exprimée en mètres ou en DD. permet le calcul du rapport unité/pixel </param>
    ''' <param name="SurfacePixel"> exprimée en pixel. permet le calcul du rapport unité/pixel </param>
    ''' <param name="ClefSiteCarto"> cle du site carto </param>
    ''' <param name="ClefEchelle"> cle de l'échelle de capture ou de scan </param>
    ''' <param name="LibelleProjection"> libelle de la projection des coordonnées </param>
    ''' <param name="H_Utm"> si la projection est une projection UTM, hémisphère de la projection </param>
    ''' <param name="Z_Utm"> si la projection est une projection UTM, numéro de zone de la projection </param>
    Friend Sub New(RegionUniteCarte As RectangleD, SurfacePixel As Size, ClefSiteCarto As String, ClefEchelle As String, LibelleProjection As String,
                                            Z_Utm As Integer, H_Utm As Char)
        Try
            _SiteCarto = TrouverSiteCartographiqueCle(ClefSiteCarto)
            _DomTom = TrouverDomTomCle(ClefSiteCarto)
            _IndiceSiteCarto = SiteDomTomToIndex(_SiteCarto, _DomTom)
            _Projection = New ProjectionCartographique(_SiteCarto, LibelleProjection)
            If Not _Projection.IsOk Then Exit Try
            If ProjectionCartographique.DatumPrincipal(_SiteCarto) = Datums.WGS84 Then
                _CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD
            Else
                _CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille
            End If
            _Niveau = New NiveauDetailCartographique(_SiteCarto, ClefEchelle)
            If Not _Niveau.IsOk Then Exit Try
            _IsCapture = False
            _IsInterpol = True
            RegionPixel = SurfacePixel
            RegionUnite = RegionUniteCarte.Taille
            EchelleLongitude = RegionPixel.Width / RegionUnite.Largeur
            EchelleLatitude = RegionPixel.Height / RegionUnite.Hauteur
            EchelleX = RegionUnite.Largeur / RegionPixel.Width
            EchelleY = RegionUnite.Hauteur / RegionPixel.Height
            PT0_Unité = RegionUniteCarte.Pt0
            ZoneUtmReferencement = Z_Utm
            HemisphereUtmReferencement = H_Utm
            _IsOk = True
        Catch Ex As Exception
            AfficherErreur(Ex, "7BCD")
            _IsOk = False
        End Try
    End Sub
    '''<summary> transforme un point exprimé en pixels du monde virtuel en coordonnée de géororeferencement du système </summary>
    Friend Function ConvertirCoordonneesVirtuellesToReelles(PointCoordonnees As Point) As PointD
        If _IsCapture Then
            If _CoordonneesGeoreferencement = Enumerations.CoordonneesGeoreferencements.Grille Then
                Return PointPixelToPointGrille(PointCoordonnees, _SiteCarto, _Niveau.Echelle)
            Else
                Return PointPixelsToPointDD(PointCoordonnees, _SiteCarto, _Niveau.Echelle)
            End If
        Else
            Return PixelsToCoordonnees_Autre(PointCoordonnees)
        End If
    End Function
    ''' <summary> tranforme un point de coordonnées exprimé en coordonnées de géoréférencement du système cartographique en pixels du monde virtuel </summary>
    ''' <param name="PointCoordonnees"></param>
    Friend Function ConvertirCoordonneesReellesToVirtuelles(PointCoordonnees As PointD) As Point
        If _IsCapture Then 'ce n'est pas une carte interpolée
            If _CoordonneesGeoreferencement = Enumerations.CoordonneesGeoreferencements.Grille Then
                Return PointGrilleToPointPixel(PointCoordonnees, _SiteCarto, _Niveau.Echelle)
            Else
                Return PointDDToPointPixels(PointCoordonnees, _SiteCarto, _Niveau.Echelle)
            End If
        Else
            Return CoordonneesToPixels_Autre(PointCoordonnees)
        End If
    End Function
    '''Pour adapter la taille du texte dans les références de grille et l'épaisseur des traits de la grille
    Friend ReadOnly Property SystemDoubleGrille() As Boolean
        Get
            Dim Ret As Boolean = False
            Dim Indice As Integer = NiveauDetailCartographique.EchelleToSiteIndiceEchelle(_SiteCarto, _Niveau.Echelle)
            Select Case _SiteCarto
                Case SitesCartographiques.SuisseMobile
                    If Indice = 0 Then Ret = True
                Case Else
                    If Indice = 1 Then Ret = True
            End Select
            Return Ret
        End Get
    End Property
    ''' <summary> Numéro de zone UTM qui a servi au référencement (coordonnéescapturées) </summary>
    Friend Property ZoneUtmReferencement() As Integer
    ''' <summary> Hemisphère de zone UTM qui a servi au référencement (coordonnéescapturées) </summary>
    Friend Property HemisphereUtmReferencement() As Char
    ''' <summary> flag indiquant si le système fait référence à une carte qui a été interpolée </summary>
    Friend ReadOnly Property IsInterpol() As Boolean
    ''' <summary> flag indiquant si le système fait référence à une carte qui a été interpolée </summary>
    Friend ReadOnly Property IsOk() As Boolean
    ''' <summary> le Domtom des coordonnées de la carte si systemeCoordonnées est DomTom par exemple réunion </summary>
    Friend ReadOnly Property LibelleDomtom As String
        Get
            If _DomTom = DomsToms.aucun Then
                Return "metropole"
            Else
                Return DomsTomsLibelles(_DomTom)
            End If
        End Get
    End Property
    ''' <summary>le Domtom des coordonnées de la carte si systemeCoordonnées est DomTom par exemple réunion </summary>
    Friend ReadOnly Property LibelleSiteCarto As String
        Get
            Return SitesCartoLibelles(_SiteCarto)
        End Get
    End Property
    ''' <summary> Quelles sont les coordonnées du fichier géoref qui doivent servir pour le géoréférencement
    '''"Capture" indique que l'on doit se servir des coordonnées capturées
    '''"DD" indique que l'on doit prendre la longitude/latitude en DD présente dans le fichier Georef </summary>
    Friend ReadOnly Property CoordonneesGeoreferencement As CoordonneesGeoreferencements
    ''' <summary>en sortie FCGP peut tracer une grille sur la carte et indiquer les coordonnées de celle-ci. il s'agit du nom de la grille </summary>
    Friend ReadOnly Property GrilleNom() As String
        Get
            Return GrillesNoms(_SiteCarto)
        End Get
    End Property
    ''' <summary> indique si la grille de sortie est déjà présente sur la carte capturée, si oui il n'y a pas besoin de la retracer </summary>
    Friend ReadOnly Property GrilleExiste() As Boolean
        Get
            Return GrillesExistes(_SiteCarto)
        End Get
    End Property
    ''' <summary> indique si la grille de sortie est présente sur la carte capturée à l'échelle du système cartographique </summary>
    Friend ReadOnly Property AfficherGrilleIsOK() As Boolean
        Get
            Return _Niveau.PasGrilleNumeric() > 0
        End Get
    End Property
    '''<summary>Pour adapter la taille du texte dans les références de grille</summary>
    Friend ReadOnly Property MaxDPIImpression() As Single
        Get
            Return MaxsDPIsImpression(_SiteCarto)
        End Get
    End Property
    '''<summary>Clef du système cartographique sous la forme SS-EEE si GF ou SM ou SS-DDD-EEE si DT </summary>
    Friend ReadOnly Property ClefEchelle As String
        Get
            Return Clef & "-" & _Niveau.Clef
        End Get
    End Property
    '''<summary>Clef du système géographique sous la forme SS si GF ou SM ou SS-DDD si DT </summary>
    Friend ReadOnly Property Clef As String
        Get
            Dim _Clef As String
            If IsCapture AndAlso Not IsInterpol Then
                _Clef = SiteCartoClef(_SiteCarto)
                If _SiteCarto = SitesCartographiques.DomTom Then _Clef &= "-" & DomTomClef(_DomTom)
            Else
                _Clef = "Non Géré"
            End If
            Return _Clef
        End Get
    End Property
    Friend ReadOnly Property IndiceEchelle As Integer
        Get
            Return NiveauDetailCartographique.EchelleToSiteIndiceEchelle(_SiteCarto, Niveau.Echelle)
        End Get
    End Property
    ''' <summary>indice du système (GéoFoncier, GéoPortail, Suisse Mobile)</summary>
    Friend ReadOnly Property SiteCarto As SitesCartographiques
    ''' <summary>indice du DomTom de la carte ou du site DomTom</summary>
    Friend ReadOnly Property DomTom As DomsToms
    ''' <summary>indice unique site carto et du DomTom</summary>
    Friend ReadOnly Property IndiceSiteCarto As Integer
    ''' <summary>indice de l'échelle de capture de la carte</summary>
    Friend ReadOnly Property Niveau As NiveauDetailCartographique
    ''' <summary>indice du type de coordonnées de la carte qui est à l'origine de la création de l'échelle</summary>
    Friend ReadOnly Property Projection As ProjectionCartographique
    ''' <summary>flag indiquant si le système provient d'une carte capturée; si false il s'agit d'une carte interpolée ou d'une carte issue de fond de plan papier</summary>
    Friend ReadOnly Property IsCapture As Boolean
#End Region
#Region "Fonctions et propriétés cachées"
    ''' <summary> Retourne l'indice du domtom inclus dans le libellé du systèmecarto passé en paramètre </summary>
    ''' <param name="LibelleSystemecarto">Site cartographique lu au niveau du fichier Géoref Ex: DomTom-reunion</param>
    Private Shared Function TrouverDomTomLibelle(LibelleSystemecarto As String) As DomsToms
        Dim Pos As Integer = LibelleSystemecarto.IndexOf("-"c)
        If Pos = -1 Then 'cela élimine Géofoncier et suisse mobile
            Return DomsToms.aucun
        Else
            Dim SiteDomTom As String = LibelleSystemecarto.Substring(Pos + 1)
            Return CType(Array.IndexOf(DomsTomsLibelles, SiteDomTom), DomsToms)
        End If
    End Function
    ''' <summary> Retourne l'indice du site carto inclus dans le libellé du systèmecarto passé en paramètre </summary>
    ''' <param name="LibelleSystemeCarto">site cartographique lu au niveau du fichier Géoref</param>
    Private Shared Function TrouverSiteCartographiqueLibelle(LibelleSystemeCarto As String) As SitesCartographiques
        'pour suisse mobile, 2 écritures cohabitent 'Suisse Mobile et SuisseMobile' et aussi pour Géofoncier, 2 écritures cohabitent 'GéoFoncier et géofoncier'
        Dim SystemeCarto = LibelleSystemeCarto.Replace(" ", Nothing).Replace("F"c, "f"c)
        'pour le site Domtom on élimine le nom du Domtom
        Dim Pos As Integer = SystemeCarto.IndexOf("-"c)
        If Pos > -1 Then SystemeCarto = SystemeCarto.Substring(0, Pos)
        Return CType(Array.IndexOf(SitesCartoLibelles, SystemeCarto), SitesCartographiques)
    End Function
    ''' <summary> fonction de conversion des unités en pixel de la carte associée à un système différent d'une capture </summary>
    Private Function CoordonneesToPixels_Autre(PointCoordonnees As PointD) As Point
        CoordonneesToPixels_Autre.X = CInt(Math.Round(EchelleLongitude * (PointCoordonnees.X - PT0_Unité.X)))
        CoordonneesToPixels_Autre.Y = CInt(Math.Round(EchelleLatitude * (PointCoordonnees.Y - PT0_Unité.Y)))
    End Function
    ''' <summary> fonction de conversion des pixel en unités de la carte associée à un système différent d'une capture </summary>
    Private Function PixelsToCoordonnees_Autre(PointPixel As Point) As PointD
        PixelsToCoordonnees_Autre.X = EchelleX * PointPixel.X + PT0_Unité.X
        PixelsToCoordonnees_Autre.Y = EchelleY * PointPixel.Y + PT0_Unité.Y
    End Function
    ''' <summary> dimensions en pixels de la carte associée à un système différent d'une capture </summary>
    Private ReadOnly RegionPixel As Size
    ''' <summary>point d'origine en unité (DD ou M) de la carte associée à un système différent d'une capture </summary>
    Private ReadOnly PT0_Unité As PointD
    ''' <summary>dimensions en unité (DD ou M) de la carte associée à un système différent d'une capture </summary>
    Private ReadOnly RegionUnite As SizeD
    ''' <summary> afin d'éviter des calculs trop longs lors des conversions de coordonnées de systèmes interpolés </summary>
    Private ReadOnly EchelleLongitude, EchelleLatitude, EchelleX, EchelleY As Double
#End Region
End Class
