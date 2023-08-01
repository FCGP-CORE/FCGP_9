Imports FCGP.NiveauDetailCartographique
Imports FCGP.SystemeCartographique
''' <summary> contient toutes les structures nécessaires pour enregistrer ou lire un fichier JNX</summary>
Friend Module StructuresJNX
#Region "Partie public"
    ''' <summary> Format associé au facteur d'affichage JNX</summary>
    Friend Const PrecisionJNX As String = "0.00"
    ''' <summary> Lit les tuiles d'un niveau du fichier ORUX et les écrit dans un répertoire </summary>
    ''' <param name="FichierCarteJnx"> chemin du fichier jnx de la carte </param>
    ''' <param name="Echelle"> Echelle dont on veut récupérer les tuiles </param>
    ''' <param name="CheminTuiles"> répertoire où l'on veut enregister les tuiles </param>
    ''' <remarks> cette fonction est donnée à titre d'exemple. Elle ne sert pas dans FCGP </remarks> 
    Friend Function LireNiveauCarteJnx(CheminCarteJnx As String, Echelle As Echelles, CheminTuiles As String) As Boolean
        'vérifier que le fichier est correct
        Dim InfosJnx As InfoJNX = VerifierCarteJNX(CheminCarteJnx)
        If InfosJnx.SiteCapture = SitesCartographiques.Aucun Then Return False
        Try
            For Cpt = 0 To InfosJnx.NbNiveauxDetail - 1
                Dim EchelleCapture = SiteIndiceEchelleToEchelle(InfosJnx.SiteCapture, InfosJnx.IndiceEchelleCapture(Cpt))
                If EchelleCapture = Echelle Then
                    Using B As New JNXReader(CheminCarteJnx) 'on ouvre le fichier JNX en lecture
                        Return B.LireNiveauJNX(Cpt, CheminTuiles)
                    End Using
                End If
            Next
        Catch ex As Exception
            AfficherErreur(ex, "Y4P3")
        End Try
        Return False
    End Function
    ''' <summary> ajoute un niveau de détail à la cart au format JNX </summary>
    ''' <param name="CarteActuelle">carte contenant le niveau de détail à ajouter</param>
    ''' <remarks> l'odre d'écriture des niveaux (indice d'échelle) n' a pas d'importance pour le GPS mais il y en a pour BaseCamp. Il faut commencer 
    ''' par la plus petite échelle donc l'indice le plus important et finir par la plus grande échelle donc indice le plus petit</remarks>
    Friend Function AjouterNiveauCarteJNX(CarteActuelle As Carte) As Boolean
        Dim CheminCarteJNX = CarteActuelle.CheminFichiersTuile & "\" & CarteActuelle.Nom & CarteActuelle.NomAjout & ".jnx"
        Dim InfJNX As InfoJNX = VerifierCarteJNX(CheminCarteJNX)
        If InfJNX.SiteCapture = SitesCartographiques.Aucun Then Return False 'il y a eu une erreur
        If InfJNX.SiteCapture <> CarteActuelle.SystemCartographique.SiteCarto Then Return False
        With CarteActuelle
            .NbIterations = 0
            .NbTotalIterations = .Tuiles.Length
            AfficherVisueInformation($"{ .MessInfo & .NbIterations} / { .NbTotalIterations}")
        End With
        For Cpt As Integer = 0 To InfJNX.NbNiveauxDetail - 1
            Dim IndiceEchelleSite As Integer = EchelleToSiteIndiceEchelle(CarteActuelle.SystemCartographique.SiteCarto, CarteActuelle.SystemCartographique.Niveau.Echelle)
            If InfJNX.IndiceEchelleCapture(Cpt) = IndiceEchelleSite Then Return False 'on ne rajoute pas le niveau de détail si il existe déjà
        Next
        AjouterNiveauCarteJNX = False
        Try
            Dim NiveauInfo As JNXLevelInfo(), NiveauMeta As JNXLevelMeta(), MapInfo As JNXMapInfo, MapMeta As JNXMapMeta
            Dim IndiceNiveau As Integer
            'lecture de l'ensemble des blocs du header de la carte
            Using B As New JNXReader(CheminCarteJNX)
                MapInfo = B.GetMapInfo() 'on lit les infos du niveau
                MapMeta = B.GetMapMeta()
                IndiceNiveau = MapInfo.Levels
                ReDim NiveauInfo(IndiceNiveau) 'plus 1 pour stocker le niveau à ajouter
                ReDim NiveauMeta(IndiceNiveau) 'plus 1 pour stocker le niveau à ajouter
                'lecture des niveaux de détail existants
                For Cpt As Integer = 0 To IndiceNiveau - 1
                    NiveauInfo(Cpt) = B.GetLevelInfo(Cpt)
                    NiveauMeta(Cpt) = B.GetLevelMeta(Cpt)
                Next
            End Using
            'on s'assure que le nom de la carte corresponde à celui du fichier jnx
            CarteActuelle.Nom = InfJNX.NomCarte
            'on ouvre le fichier jnx en écriture
            Using Writer As New JNXWriter(CheminCarteJNX, IndiceNiveau + 1)
                'on met à jour le nb de niveaux du fichier existant
                Writer.SetMapInfo(MapInfo) 'les données initiales et plus 1 au nb de niveau
                Writer.SetMapMeta(MapMeta) 'les données initiales et plus 1 au nb de niveau
                'renvoyer les différents blocks dejà existants
                For Cpt As Integer = 0 To IndiceNiveau - 1
                    Writer.SetLevelInfo(Cpt, NiveauInfo(Cpt))
                    Writer.SetLevelMeta(Cpt, NiveauMeta(Cpt))
                Next
                'écrire le nouveau détail
                Writer.EcrireNiveauJNX(CarteActuelle, IndiceNiveau)
            End Using
            AjouterNiveauCarteJNX = True
        Catch Ex As Exception
            AfficherErreur(Ex, "I2C3")
        End Try
    End Function
    ''' <summary> Quand on veut créer un fichier JNX. Doit être appelée directement à partir d'une carte à partir de la fonction RealiserDemandes</summary>
    Friend Function CreerCarteJNX(CarteActuelle As Carte) As Boolean
        CreerCarteJNX = False
        Try
            With CarteActuelle
                .NbIterations = 0
                .NbTotalIterations = .Tuiles.Length
                AfficherVisueInformation($"{ .MessInfo & .NbIterations} / { .NbTotalIterations}")
                Dim Clef As String = If(.SystemCartographique.SiteCarto = SitesCartographiques.DomTom,
                                            DomTomClef(.SystemCartographique.DomTom),
                                            SiteCartoClef(.SystemCartographique.SiteCarto))
                Dim NomMap As String = Clef & "-" & .Nom
                Dim Bounds As JNXRect = DoJNXRect(WGS84CoordToJNX(.Coins(0).Latitude), WGS84CoordToJNX(.Coins(2).Longitude),
                                                  WGS84CoordToJNX(.Coins(2).Latitude), WGS84CoordToJNX(.Coins(0).Longitude))
                Using Writer As New JNXWriter(.CheminFichiersTuile & "\" & .Nom & .NomAjout & ".jnx", NomMap, Bounds)
                    If Not Writer.IsOk Then Exit Try
                    Writer.EcrireNiveauJNX(CarteActuelle, 0)
                    'on écrit la signature et l'entête du fichier de manière indirecte.
                    'Dispose() écrit l'entête
                End Using
            End With
            CreerCarteJNX = True
        Catch Ex As Exception
            AfficherErreur(Ex, "C6S6")
        End Try
    End Function
    ''' <summary> Verifie si le fichier jnx correspond à la version créée à partir de la version 5 de FCGP et renvoie les infos concernant les blocs de l'entête </summary>
    ''' <param name="CheminCarteJNX"></param>
    Friend Function VerifierCarteJNX(CheminCarteJNX As String) As InfoJNX
        VerifierCarteJNX = New InfoJNX With {.SiteCapture = SitesCartographiques.Aucun, .NbNiveauxDetail = -1, .OrdreAffichage = -1}
        Try
            Using B As New JNXReader(CheminCarteJNX)
                If Not B.IsOk Then Exit Try 'Le fichier n'existe pas
                If Not B.VerifierFormatCarteJNX Then Exit Try 'le fichier n'est pas de version 4 ou la signature de fin est erronnée
                Dim MapInfo As JNXMapInfo = B.GetMapInfo
                If MapInfo.Levels < 1 Or MapInfo.Levels > 5 Then Exit Try 'le nb de niveau de détail est incorrect
                Dim InOr As New InfoJNX With {.NbNiveauxDetail = MapInfo.Levels, .OrdreAffichage = MapInfo.ZOrder}
                Dim MapMeta As JNXMapMeta = B.GetMapMeta
                Dim ClefSite As String = Nothing
                DecomposerMapName(MapMeta.MapName, InOr.NomCarte, InOr.SiteCapture, InOr.DomTom, ClefSite)

                If InOr.SiteCapture = SitesCartographiques.Aucun Then Exit Try
                ReDim InOr.NiveauDetail(MapInfo.Levels - 1)
                ReDim InOr.IndiceEchelleCapture(MapInfo.Levels - 1)
                ReDim InOr.IndiceAffichage(MapInfo.Levels - 1)
                ReDim InOr.NiveauZoomMP(MapInfo.Levels - 1)
                For Cpt As Integer = 0 To MapInfo.Levels - 1
                    Dim LevelMeta As JNXLevelMeta = B.GetLevelMeta(Cpt)
                    Dim ClefEchelle As String = Nothing
                    DecomposerLevelName(LevelMeta.Name, InOr.SiteCapture, InOr.IndiceEchelleCapture(Cpt), ClefEchelle)
                    If InOr.IndiceEchelleCapture(Cpt) = -1 Then Exit Try
                    InOr.NiveauZoomMP(Cpt) = LevelMeta.Zoom
                    Dim LevelInfo As JNXLevelInfo = B.GetLevelInfo(Cpt)
                    InOr.IndiceAffichage(Cpt) = TrouverIndiceAffichage(LevelInfo.Scale)
                    InOr.NiveauDetail(Cpt) = ClefSite & "-" & ClefEchelle
                Next
                VerifierCarteJNX = InOr
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "P2S1")
        End Try
    End Function
    ''' <summary> change l'ordre d'affichage de la carte au format JNX. " </summary>
    ''' <param name="CheminCarteJNX"> carte au format </param>
    ''' <param name="Valeur"></param>
    Friend Function ChangerOrdreAffichageCarteJNX(CheminCarteJNX As String, Valeur As Integer) As Boolean
        ChangerOrdreAffichageCarteJNX = False
        Dim InfosJnx = VerifierCarteJNX(CheminCarteJNX)
        If InfosJnx.SiteCapture = SitesCartographiques.Aucun Then Return False
        Try
            Using Writer = New JNXWriter(CheminCarteJNX, Valeur, -1D)
                ChangerOrdreAffichageCarteJNX = Writer.IsOk
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "W1T4")
        End Try
    End Function
    ''' <summary>  change l'indice d'affichage d'un niveau de détail (échelle à partir de laquelle le niveau de détail disparait</summary>
    ''' <param name="CheminCarteJNX">carte au format JNX</param>
    ''' <param name="IndexNiveau">le niveau de détail concerné par le changement</param>
    ''' <param name="IndiceAffichageModifie">valeur à ajouter à l'indice d'affichage. Cet indice sera traduit en valeur d'échelle dans le fichier JNX. Si positif le niveau disparait à une échelle plus grande</param>
    ''' <remarks>aucune vérification par rapport aux autres niveaux de détail</remarks>
    Friend Function ChangerNiveauAffichageCarteJNX(CheminCarteJNX As String, IndexNiveau As Integer, IndiceAffichageModifie As Double) As Boolean
        ChangerNiveauAffichageCarteJNX = False
        Dim InfosJnx As InfoJNX = VerifierCarteJNX(CheminCarteJNX)
        If InfosJnx.SiteCapture = SitesCartographiques.Aucun Then Return False
        Try
            Using Writer = New JNXWriter(CheminCarteJNX, IndexNiveau, IndiceAffichageModifie)
                ChangerNiveauAffichageCarteJNX = Writer.IsOk
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "D8M3")
        End Try
    End Function
    ''' <summary> pour renvoyer toutes les informations utiles concernant les fichiers JNX </summary>
    Friend Structure InfoJNX
        Friend SiteCapture As SitesCartographiques
        Friend DomTom As DomsToms
        Friend NomCarte As String
        Friend NbNiveauxDetail As Integer
        Friend OrdreAffichage As Integer 'ordre d'affichage de la carte par rapport aux autres : détermine la visibilité de la carte
        Friend NiveauDetail As String()
        Friend IndiceEchelleCapture As Integer()
        ''' <summary> valeur par pas de 0.25 indiquant, une fois transformée en valeur d'échelle dans le fichier JNX, 
        ''' à quelle échelle d'affichage le niveau de détail disparait </summary>
        Friend IndiceAffichage As Double()
        Friend NiveauZoomMP As Integer()
    End Structure
#End Region
#Region "constantes"
    Private Const NbOctetsUInt32 As Integer = 4
    Private Const NbOctetsInt32 As Integer = 4
    Private Const NbOctetsInt16 As Integer = 2
    Private Const NbOctetsJNXPoint As Integer = NbOctetsInt32 * 2
    Private Const NbOctetsJNXRect As Integer = NbOctetsJNXPoint * 2
    Private Const NbOctetsTileInfo As Integer = NbOctetsJNXRect + NbOctetsInt16 * 2 + NbOctetsInt32 + NbOctetsUInt32
    Private Const NbOctetsMapInfo As Integer = NbOctetsInt32 * 8 + NbOctetsJNXRect + NbOctetsUInt32
    Private Const JNX_EOF_SIGNATURE As String = "BirdsEye"
    Private Const JnxToDec As Double = 180 / &H_7F_FF_FF_FF
    Private Const DecToJnx As Double = &H_7F_FF_FF_FF / 180
    ''' <summary> différentes échelles en fonction de l'indice de zoom </summary>
    Private ReadOnly ZoomToScale() As Integer = New Integer() {19, 37, 75, 149, 298, 597, 1194, 2388, 4777, 9554, 19109, 38218, 76437, 152877,
                                                               305758, 611526, 1223072, 2446184, 2446184, 2446184, 2446184, 2446184, 2446184, 2446184}
#End Region
#Region "Fonctions non visibles "
    ''' <summary> retrouve dans le nom du niveau de détail la clef et l'indice de l'échelle de capture </summary>
    ''' <param name="LevelName"> Nom du niveau de détail </param>
    ''' <param name="SiteCapture"> site de capture </param>
    ''' <param name="IndiceEchelle"> indice de l'échelle </param>
    ''' <param name="ClefEchelle"> clef de l'échelle </param>
    Private Sub DecomposerLevelName(LevelName As String, SiteCapture As SitesCartographiques, ByRef IndiceEchelle As Integer, ByRef ClefEchelle As String)
        Dim Pos As Integer = LevelName.IndexOf("-"c)
        ClefEchelle = LevelName.Substring(0, Pos)
        IndiceEchelle = EchelleClefToSiteIndiceEchelle(SiteCapture, ClefEchelle)
    End Sub
    ''' <summary> retrouve dans le nom de la carteJNX, le nom de la carte, le site, la clef et le domtom de capture </summary>
    ''' <param name="MapName"> nom de la carteJNX </param>
    ''' <param name="NomCarte"> nom de la carte retrouvé </param>
    ''' <param name="SiteCapture"> site retrouvé</param>
    ''' <param name="DomTom"> DomTom retrouvé </param>
    ''' <param name="ClefSite"> clef du site retrouvé </param>
    Private Sub DecomposerMapName(MapName As String, ByRef NomCarte As String, ByRef SiteCapture As SitesCartographiques, ByRef DomTom As DomsToms, ByRef ClefSite As String)
        Dim Pos As Integer = MapName.IndexOf("-"c)
        ClefSite = MapName.Substring(0, Pos)
        NomCarte = MapName.Substring(Pos + 1)
        SiteCapture = TrouverSiteCartographiqueCle(ClefSite)
        DomTom = DomsToms.aucun
        If SiteCapture = SitesCartographiques.Aucun Then
            DomTom = TrouverDomTomCle(ClefSite)
            If DomTom > DomsToms.aucun Then
                SiteCapture = SitesCartographiques.DomTom
            Else
                Throw New Exception("Fichier JNX corrompu")
            End If
        End If
    End Sub
    ''' <summary> crée une structure JNXRect à partir de 4 coordonnées exprimées en coordonnées GPS </summary>
    Private Function DoJNXRect(northern_lat As Integer, eastern_lon As Integer, southern_lat As Integer, western_lon As Integer) As JNXRect
        With DoJNXRect
            .northEast.Latitude = Math.Max(northern_lat, southern_lat)
            .northEast.Longitude = Math.Max(eastern_lon, western_lon)
            .southWest.Latitude = Math.Min(northern_lat, southern_lat)
            .southWest.Longitude = Math.Min(eastern_lon, western_lon)
        End With
    End Function
    ''' <summary> transforme le nb de métre par pixel d'une carte capturée en échelle maximum à indiquer dans les infos du niveau </summary>
    ''' <param name="d"> echelleX exprimée en nombre de mètre par pixel de l'image </param>
    Private Function MetersPerPixelToScale(d As Double, FacteurJNX As Double) As Integer
        'on détermine l'indice d'échelle normal pour un facteur 0
        Dim IndiceEchelle As Integer = 12 + CInt(Math.Round(Math.Log(d * 1000 / 76437) / Math.Log(2)))
        'on le corrige en rajoutant la partie entière du facteur de correction
        IndiceEchelle += CInt(Math.Floor(FacteurJNX))
        Dim Coef As Double = FacteurJNX - Math.Floor(FacteurJNX) 'soit 0 soit 0.5 mais peut être amené à changer par pas de 0.1
        If Coef > 0 Then 'si la partie fractionnaire du facteur de correction est supérieure à 0 alors on apporte une correction supplémentaire
            Return ZoomToScale(IndiceEchelle) + CInt((ZoomToScale(IndiceEchelle + 1) - ZoomToScale(IndiceEchelle)) * Coef)
        Else
            Return ZoomToScale(IndiceEchelle)
        End If
    End Function
    ''' <summary> tranforme un coordonnée exprimée en degré décimal en unité de coordonnées GPS</summary>
    Private Function WGS84CoordToJNX(CoordonneesWGS84 As Double) As Integer
        Return CInt(CoordonneesWGS84 * DecToJnx)
    End Function
    ''' <summary> tranforme un coordonnée exprimée en unité de coordonnées GPS en degré décimal</summary>
    Private Function JNXCoordToWGS84(CoordonneesJNX As Integer) As Double
        Return CoordonneesJNX * JnxToDec
    End Function
    ''' <summary> renvoie un indice d'affichage en fonction d'une valeur passée en paramètre</summary>
    Private Function TrouverIndiceAffichage(Scale As Integer) As Double
        For Cpt = 0 To ZoomToScale.Length - 1
            If ZoomToScale(Cpt) = Scale Then
                Return Cpt
            ElseIf ZoomToScale(Cpt) > Scale Then
                Dim EcartGlobal As Integer = ZoomToScale(Cpt) - ZoomToScale(Cpt - 1)
                Dim EcartValeur = ZoomToScale(Cpt) - Scale
                Dim Correction As Integer = CInt(EcartValeur / EcartGlobal * 100)
                Return Cpt - Correction / 100
            End If
        Next
        Return -1
    End Function
    ''' <summary> renvoie une echelle d'affichage en fonction d'un indice passé en paramètre</summary>
    Private Function TrouverScaleAffichage(IndiceAffichage As Double) As Integer
        Dim Indice As Integer = CInt(Math.Floor(IndiceAffichage))
        Dim Fract As Double = IndiceAffichage - Indice
        If Fract = 0 Then Return ZoomToScale(Indice)
        Dim Ecart As Integer = CInt((ZoomToScale(Indice + 1) - ZoomToScale(Indice)) * Fract)
        Return ZoomToScale(Indice) + Ecart
    End Function
#End Region
#Region "Structures d'un fichier JNX "
    ''' <summary>point de coordonnées en unitées GPS. Voir fonction WGS84ToJNX</summary>
    Private Structure JNXPoint
        Friend Latitude As Integer 'stocker sous la forme : DD * (2^31-1) / 180
        Friend Longitude As Integer 'stocker sous la forme : DD * (2^31-1) / 180
    End Structure
    ''' <summary>coordonnées d'une tuile ou de la carte en unité GPS</summary>
    Private Structure JNXRect
        Friend northEast As JNXPoint
        Friend southWest As JNXPoint
    End Structure
    ''' <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 1ère</summary>
    Private Structure JNXMapInfo
        Friend Version As Integer 'N° de version : 3 ou 4. toujours 4 dans le cas de FCGP
        Friend DeviceSN As Integer ' Identifiant du périphérique de téléchargement. Toujours 0 avec les JNX de démo
        Friend MapBounds As JNXRect 'Coordonnées de la carte. 2 points voir strucutre JNXRect
        Friend Levels As Integer 'Nombre de niveau de détail de la carte. Maximum 5
        Friend Expiration As Integer 'date d'expiration en cas detéléchargement exprimée en nb de seconde depuis 30/12/1989 12H00. Toujours 0 avec les JNX de démo
        Friend ProductID As Integer 'identifiant du site de téléchargement. Toujours 0 avec les JNX de démo
        Friend CRC As Integer 'CRC32 es coordonnées des tuiles. Toujours 0 avec les JNX de démo
        Friend SigVersion As Integer 'Signature de la version du fichier téléchargé. Toujours 0 avec les JNX de démo
        Friend SigOffset As UInteger 'Offset de la signature. Toujours 0 avec les JNX de démo
        Friend ZOrder As Integer 'ordre d'affichage sur l'axe de Z sur le périphérique de éléchargement. de0 à 100. 100 étant le haut donc le plus visible
    End Structure
    ''' <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 3ème</summary>
    Private Structure JNXMapMeta
        Friend LevelMetaCount As Integer 'nombre de niveau de détail de la carte.Le même que celui du block JNXMapinfo
        Friend MapName As String 'nom de la carte
        Friend ProductID As Short 'Identifiant du site de téléchargement. le même que celui du block JNXMapinfo
        Friend Unknow As String ' chaine non utilisée. Toujours 0
        Friend ProductName As String 'nom du produit. toujours une chaine vide soit 0
        Friend GUID As String 'GUID de la carte. Identifiant unique de la forme 12345678-1234-1234-1234-123456789ABC. soir 36 caractères
        Friend Version As Integer 'version du block MapMeta. toujours 9
    End Structure
    ''' <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 2ème, 1 table avec nb niveaux enregistrements</summary>
    Private Structure JNXLevelInfo
        Friend Copyright As String 'Copyright du niveau
        Friend Unknow As Integer 'inconnu. Toujours 2 avec les JNX de démo
        Friend Scale As Integer 'Echelle d'affichage à partir duquel le niveau disparait ou apparait à l'affichage
        Friend TilesOffset As UInteger 'offset de la table de descritpion des tuiles
        Friend TileCount As Integer 'nombre de tuiles composant le niveau
    End Structure
    ''' <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 4ème, 1 table avec nb niveaux enregistrements</summary>
    Private Structure JNXLevelMeta
        Friend Name As String 'Nom du niveau. doit contenir la clef de l'échelle de capture
        Friend Description As String 'chaine non utilisée. Toujours 0
        Friend Copyright As String 'copyright du niveau. Le même que celui du block JNXLevelInfo
        Friend Zoom As Integer 'zoom de mapsource utilisé pour générer le niveau. doit être différent si plusieurs niveaux
    End Structure
    ''' <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 5ème, 1 table par niveau avec nbtuiles enregistrements</summary>
    Private Structure JNXTileInfo
        Friend PicOffset As UInteger 'offset de la tuile par rapport au début du fchier
        Friend PicSize As Integer ' aille de la tuile en octet sachant que la tuile est un fichier jpeg sans le 2 premiers octets qui sont la reconnaissance du format
        Friend PicHeight As Short 'hauteur de la tuile en pixel
        Friend PicWidth As Short 'largeur de la tuile en pixel
        Friend TileBounds As JNXRect 'coordonnées de la tuile. 2 points voir strucutre JNXRect
    End Structure
#End Region
    ''' <summary> cette classe gère l'écriture d'un fichier au format JNX. Seule la version 4 du format avec un seul fichier est supporté. 
    ''' On enregistre d'abord les niveaux de détail, pour un niveau c'est d'abord les toutes les images puis les infos tuiles afin d'éviter de modifier la position d'écriture. 
    ''' L'image des tuiles doit être au format Jpeg. Les coordonnées des tuiles sont exprimées en unitées GPS. Voir les fonctions WGS84ToJNX et DoJNXRect.
    ''' Les autres niveaux commencent après la dernière image du niveau précédent. Les blocs composants l'entête de fichier sont enrigistrés à la fermeture du fichier JNX.
    ''' Les fichiers JNX ont des limitations. Niveaux : de 1 à 5, Nb Tuiles : de 1 à 50000, Dimensions des Tuiles : inf ou égal à 1024*1024 et en multiple de 8 si possible, 
    ''' NbOctets Tuiles : inf à 3Mo, NBOctets fichier JNX : inf à 100Mo. La table d'info tuiles du 1er niveau commence toujours à l'offset 1024 ''' </summary>
    ''' <remarks> La taille du fichier JNX peut dépasser la capacité d'un integer d'où l'abandon de seek pour Basestream.Position au niveau de l'écriture des tuiles </remarks>
    Private Class JNXWriter
        Implements IDisposable
#Region "constantes et variables"
        ''' <summary>contient les données de la carte</summary>
        Private FMapInfo As JNXMapInfo
        ''' <summary>contient les données du niveau</summary>
        Private FLevelInfo As JNXLevelInfo()
        ''' <summary>contient les données descriptives de la carte</summary>
        Private FMapMeta As JNXMapMeta
        ''' <summary>contient les données descriptives des niveau</summary>
        Private FLevelMeta As JNXLevelMeta()
        ''' <summary>support du fichier à écrire</summary>
        Private ReadOnly FileBinaire As BinaryWriter
        ''' <summary>nb de niveaux de détail du fichier</summary>
        Private ReadOnly NbNiveaux As Integer
        ''' <summary>indique si il s'agit d'une création ou d'un ajout de niveau de détail</summary>
        Private ReadOnly FlagCreation As Boolean
        ''' <summary>addresse d'écriture de l'image de la tuile par rapport au début du fichier binaire</summary>
        Private OffsetImageTuile As UInteger
        ''' <summary>addresse d'écriture des données de la tuile par rapport au début du fichier binaire</summary>
        Private ReadOnly OffsetInfoTuile As UInteger
        Private Const MAX_TILES As Integer = 50000
        Private Const MAX_TILE_SIZE As Long = &H300000L
        Private Const MAX_FILE_SIZE As Long = &H100000000L
        Private Const JNX3_ZORDER As Integer = 75
        Private Const META_BLOCK_VERSION As Integer = 9
        Private Const META_BLOCK_MINSIZE As Integer = 1024

        Private Infostuiles As BinaryWriter
        Private BaseCheminTuiles As String
#End Region
#Region "procédures et fonctions"
#Region "visible"
        ''' <summary>ok si l'ouverture ou la création du fichier binaire c'est bien passée </summary>
        Friend ReadOnly Property IsOk As Boolean
        ''' <summary> constructeur pour la modification de l'ordre d'affichage ou du niveau d'affichage d'un niveau </summary>
        ''' <param name="Path">nom du fichier jnx</param>
        ''' <param name="Valeur"> ordre d'affichage de la carte JNX ou indice du niveau de détail à modifier </param>
        ''' <param name="NiveauAffichage"> indice d'affichage modifié du niveau de détail si >-1 </param>
        Friend Sub New(Path As String, Valeur As Integer, NiveauAffichage As Double)
            Try
                IsChangedValeur = True
                FileBinaire = New BinaryWriter(File.Open(Path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                Using B As New JNXReader(Path)
                    If Not B.IsOk Then Exit Try
                    If NiveauAffichage = -1 Then
                        'on modifie la valuer de l'ordre d'affichage de la carte JNX
                        FMapInfo = B.GetMapInfo
                        FMapInfo.ZOrder = Valeur
                        EcrireJNXMapInfo()
                    Else
                        'on modifie la valeur de l'indice d'affichage d'un niveau de détail
                        ReDim FLevelInfo(0)
                        FLevelInfo(0) = B.GetLevelInfo(Valeur) 'on lit les infos du niveau
                        Dim OffsetLevelInfo = B.Offset.LevelInfo(Valeur)
                        FLevelInfo(0).Scale = TrouverScaleAffichage(NiveauAffichage)
                        EcrireJNXLevelInfo(FLevelInfo(0), CInt(OffsetLevelInfo))
                    End If
                    IsOk = True
                End Using
                _IsOk = True
            Catch Ex As Exception
                AfficherErreur(Ex, "AF55")
                IsOk = False
            End Try
        End Sub
        ''' <summary> constructeur pour l'ajout d'un niveau de détail à un fichier jnx</summary>
        ''' <param name="Path">nom du fichier jnx</param>
        ''' <param name="NbNiveauxDetail">nb de niveau de détail avec celui encours d'écriture</param>
        Friend Sub New(Path As String, NbNiveauxDetail As Integer)
            Try
                FileBinaire = New BinaryWriter(File.Open(Path, FileMode.Open, FileAccess.Write))
                NbNiveaux = NbNiveauxDetail
                FlagCreation = False
                DimensionnerLevels()
                OffsetInfoTuile = CUInt(FileBinaire.BaseStream.Length - JNX_EOF_SIGNATURE.Length)
                _IsOk = True
            Catch Ex As Exception
                AfficherErreur(Ex, "A1Y6")
            End Try
        End Sub
        ''' <summary> constructeur pour la création d'un fichier jnx</summary>
        ''' <param name="Path">nom du fichier jnx</param>
        ''' <param name="NomMap">nom qui servira de nom de carte interne au fichier jnx et aux niveaux de détail</param>
        ''' <param name="Bounds">coordonnées de la carte exprimées en unités GPS</param>
        Friend Sub New(Path As String, NomMap As String, Bounds As JNXRect)
            Try
                FileBinaire = New BinaryWriter(File.Open(Path, FileMode.Create, FileAccess.Write))
                NbNiveaux = 1
                FlagCreation = True
                DimensionnerLevels()
                InitMapInfo()
                OffsetInfoTuile = META_BLOCK_MINSIZE
                FMapMeta.MapName = NomMap
                FMapInfo.MapBounds = Bounds
                _IsOk = True
            Catch Ex As Exception
                AfficherErreur(Ex, "E0J0")
            End Try
        End Sub
        ''' <summary> permet de libérer les ressources associées à l'EditableBitmap. N'a pas pour vocation a être appelé directement mais par le end using</summary>
        Friend Sub Dispose() Implements IDisposable.Dispose
            CloseFile()
            'nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(Me)
        End Sub
        ''' <summary> écrit un niveau de détail dans le fichier : infos et image des tuiles et met à jour les infos des blocs niveaux correpondant</summary>
        ''' <param name="CarteActuelle"></param>
        ''' <param name="IndiceNiveau"></param>
        Friend Sub EcrireNiveauJNX(CarteActuelle As Carte, IndiceNiveau As Integer)
            With CarteActuelle
                FLevelMeta(IndiceNiveau).Name = .SystemCartographique.Niveau.Clef & "-" & .Nom
                FLevelInfo(IndiceNiveau).Scale = TrouverScaleAffichage(.FacteurJNX)
                If .Tuiles.Length > MAX_TILES Then Throw New Exception("Le nombres de tuiles admissibles pour le niveau " & IndiceNiveau & " est dépassé")
                FLevelInfo(IndiceNiveau).TileCount = .Tuiles.Length
                FLevelInfo(IndiceNiveau).TilesOffset = OffsetInfoTuile
                'on garde dans le fichier jnx le nb de col et rang pour pour restituer le nom des tuiles
                FLevelInfo(IndiceNiveau).Copyright = $"{ .NbTuilesX:D3}*{ .NbTuilesY:D3}"
                FLevelMeta(IndiceNiveau).Zoom = IndiceNiveau + 1
                'initialiser l'offset de départ pour l'enregistrement de l'image des tuile
                OffsetImageTuile = OffsetInfoTuile + CUInt(.Tuiles.Length * NbOctetsTileInfo)
                'verifier que tout est ok
                If Not IsHeaderCompleted(IndiceNiveau) Then Throw New Exception("La description de l'entête du fichier JNX n'est pas complète : " &
                                                                            "A minima, les blocks de niveaux de détail existent et Nb de tuiles par niveau")
                'afin d'éviter des positionnement incessant au début et en fin de fichier 
                Dim MemoireInfosTuiles As New MemoryStream(FLevelInfo(IndiceNiveau).TileCount * NbOctetsTileInfo)
                Infostuiles = New BinaryWriter(MemoireInfosTuiles)
                'on positionne au début des données binaire de la 1ère tuile à écrire
                FileBinaire.BaseStream.Position = OffsetImageTuile
                BaseCheminTuiles = CheminEnregistrementProvisoire & Carte.ExtensionCheminImagesTuiles & "\"
                'on écrit les tuiles
                For Each T As DescriptionImageFichier In .Tuiles
                    'on lit l' image au format jpeg dans un buffer(tableau d'octets)
                    EcrireTuile(IndiceNiveau, T)
                    .NbIterations += 1
                    If .NbIterations Mod 100 = 0 Then AfficherVisueInformation($"{ .MessInfo & .NbIterations} / { .NbTotalIterations}")
                Next
                'on positionne sur l'emplacement des infos tuiles afin d'écrire les infos
                FileBinaire.BaseStream.Position = OffsetInfoTuile
                FileBinaire.Write(MemoireInfosTuiles.ToArray)
            End With
        End Sub
        ''' <summary> initialise la structure MapInfo du fichier JNX en une fois et ajoute 1 au nb de niveau de détail car forcément appelée par AjoutNiveau</summary>
        Friend Sub SetMapInfo(MapInfo As JNXMapInfo)
            If FlagCreation Then Throw New Exception("Vous ne pouvez pas transmettre le block MapInfo en mode création")
            'on rajoute un niveau
            FMapInfo = MapInfo
            FMapInfo.Levels += 1
        End Sub
        ''' <summary> initialise la structure MapMeta du fichier JNX en une fois et ajoute 1 au nb de niveau de détail car forcément appelée par AjoutNiveau</summary>
        Friend Sub SetMapMeta(MapMeta As JNXMapMeta)
            If FlagCreation Then Throw New Exception("Vous ne pouvez pas transmettre le block MapMeta en mode création")
            FMapMeta = MapMeta
            FMapMeta.LevelMetaCount += 1
        End Sub
        ''' <summary> initialise une des structures LevelInfo du fichier JNX en une fois. Forcément appeléé par AjoutNiveau </summary>
        Friend Sub SetLevelInfo(IndiceNiveau As Integer, LevelInfo As JNXLevelInfo)
            If FlagCreation Then Throw New Exception("Vous ne pouvez pas transmettre le block LevelInfo en mode création")
            FLevelInfo(IndiceNiveau) = LevelInfo
        End Sub
        ''' <summary> initialise une des structures LevelMeta du fichier JNX en une fois. Forcément appeléé par AjoutNiveau </summary>
        Friend Sub SetLevelMeta(IndiceNiveau As Integer, LevelMeta As JNXLevelMeta)
            If FlagCreation Then Throw New Exception("Vous ne pouvez pas transmettre le block LevelMeta en mode création")
            FLevelMeta(IndiceNiveau) = LevelMeta
        End Sub
        ''' <summary> champ non renseigné par défaut lors de l'initialisation des blocs de niveaux</summary>
        Friend Sub SetLevelCopyright(IndiceNiveau As Integer, Copyright As String)
            FLevelMeta(IndiceNiveau).Copyright = Copyright
        End Sub
        ''' <summary> champ non renseigné par défaut lors de l'initialisation des blocs de niveaux</summary>
        Friend Sub SetLevelDescription(IndiceNiveau As Integer, Description As String)
            FLevelMeta(IndiceNiveau).Description = Description
        End Sub
#End Region
#Region "invisible"
        ''' <summary> indique si le constructeur utilisé est prévue pour changer une valeur ou pour créer ou ajouter un niveau </summary>
        Private ReadOnly IsChangedValeur As Boolean
        ''' <summary>écrit la signature et l'entête du fichier JNX puis cloture celui ci</summary>
        Private Sub CloseFile()
            Try
                If FileBinaire IsNot Nothing Then
                    If Not IsChangedValeur Then
                        'on est dans la création ou l'ajout d'un niveau
                        EcrireEnteteEtSignature()
                    End If
                    FileBinaire.Flush()
                End If
            Catch Ex As Exception
                AfficherErreur(Ex, "X9S1")
            Finally
                FileBinaire?.Close()
            End Try
        End Sub
        ''' <summary> dimensionne les tableaux concernant les niveaux de détails </summary>
        Private Sub DimensionnerLevels()
            ReDim FLevelInfo(NbNiveaux - 1)
            ReDim FLevelMeta(NbNiveaux - 1)
        End Sub
        ''' <summary> Sert d'initialisateur de champs de la création du fichier JNX. Certains champs sont toujours indentiques. </summary>
        Private Sub InitMapInfo()
            If Not FlagCreation Then Throw New Exception("Vous ne pouvez plus initialiser ces champs directement en mode ajout")
            FMapInfo.Version = 4
            FMapInfo.ZOrder = JNX3_ZORDER
            FMapMeta.Version = META_BLOCK_VERSION
            FMapMeta.GUID = CreateGUID()
            FMapInfo.Levels = 1
            FMapMeta.LevelMetaCount = 1
        End Sub
        ''' <summary> enregistre la tuile, infos et image dans le fichier JNX </summary>
        ''' <param name="IndiceNiveau"> indice du niveau de détail de la tuile </param>
        ''' <param name="T"> Description de la tuile </param>
        ''' <remarks> on vérifie que le nb de tuiles du niveaux n'est pas dépassé et que la taille du fichier reste correcte</remarks>
        Private Sub EcrireTuile(IndiceNiveau As Integer, T As DescriptionImageFichier)
            Dim CheminTuile = BaseCheminTuiles & T.Nom
            Dim BitsJpeg = File.ReadAllBytes(CheminTuile)
            'on ne prend pas en compte les 2 octets marqueur du type de fichier (jpg)
            Dim TailleJpeg As Integer = BitsJpeg.Length - 2
            If TailleJpeg < 1 Then Throw New Exception("L'image de la tuile n'est pas correcte.")
            If TailleJpeg >= MAX_TILE_SIZE Then Throw New Exception("Une des tuiles du niveau " & IndiceNiveau & " dépasse le poids autorisé.")
            If (OffsetImageTuile + TailleJpeg + JNX_EOF_SIGNATURE.Length >= MAX_FILE_SIZE) Then Throw New Exception("Le poids total du fichier est dépassé au niveau " & IndiceNiveau)

            'calcul des coordonnées de la tuile exprimées en DD unité GPS (entier)
            Dim Bounds As JNXRect = DoJNXRect(WGS84CoordToJNX(T.Nord), WGS84CoordToJNX(T.Est),
                                              WGS84CoordToJNX(T.Sud), WGS84CoordToJNX(T.Ouest))
            Dim Tuile As New JNXTileInfo With {.TileBounds = Bounds, .PicWidth = CShort(T.LargeurPixels),
                                           .PicHeight = CShort(T.HauteurPixels), .PicSize = TailleJpeg,
                                           .PicOffset = OffsetImageTuile}
            'écriture de tileInfo dans le tampon infotuile
            EcrireJNXTileInfo(Tuile)
            'écriture de l'image de la tuile dans le fichier jnx
            FileBinaire.Write(BitsJpeg, 2, TailleJpeg)
            'calcul du nouvel index
            OffsetImageTuile = CUInt(FileBinaire.BaseStream.Position)
        End Sub
        ''' <summary> écrit l'entête et la fin du fichier. L'entête est composé par les structures d'information globale de la carte et par
        ''' les structures d'informations concernant les différents niveaux </summary>
        Private Sub EcrireEnteteEtSignature()
            'on positionne à la fin de la dernière tuile écrite
            FileBinaire.BaseStream.Position = OffsetImageTuile
            'il suffit de mettre la signature de fin au bon endroit c'est à dire après la dernière image de tuile
            EcrireJNXSignature()
            'on revient à zéro et on enregistre le header
            FileBinaire.BaseStream.Position = 0
            EcrireJNXMapInfo() 'bloc à longueur constante
            For Cpt = 0 To NbNiveaux - 1
                EcrireJNXLevelInfo(FLevelInfo(Cpt)) 'bloc à longueur constante
            Next
            EcrireJNXMapMeta(FMapMeta) 'bloc à longueur variable
            For Cpt = 0 To NbNiveaux - 1
                EcrireJNXLevelMeta(FLevelMeta(Cpt)) 'bloc à longueur variable
            Next
        End Sub
        ''' <summary>vérifie que les infos obligatoires ont été renseignées </summary>
        Private Function IsHeaderCompleted(IndiceNiveau As Integer) As Boolean
            If Not FMapInfo.Levels = NbNiveaux Then Return False
            If Not FLevelInfo(IndiceNiveau).TileCount > 0 Then Return False
            Return True
        End Function
        ''' <summary> écrit en fin du fichier la signature du fichier JNX </summary>
        Private Sub EcrireJNXSignature()
            FileBinaire.Write(Encoding.UTF8.GetBytes(JNX_EOF_SIGNATURE))
        End Sub
        ''' <summary> écrit une structue JNXPoint dans un flux binaire</summary>
        ''' <param name="R"> structure JNXPoint </param>
        ''' <param name="IsTuile"> Si il s'agit d'une tuile on écrit pas directement dans le fichier pour éviter les changements de position du pointeur de position </param>
        Private Sub EcrireJNXPoint(R As JNXPoint, IsTuile As Boolean)
            If IsTuile Then
                Infostuiles.Write(R.Latitude)
                Infostuiles.Write(R.Longitude)
            Else
                FileBinaire.Write(R.Latitude)
                FileBinaire.Write(R.Longitude)
            End If
        End Sub
        ''' <summary> écrit une structue JNXRect dans un flux binaire</summary>
        ''' <param name="R">structure JNXRect</param>
        ''' <param name="IsTuile"> Si il s'agit d'une tuile on écrit pas directement dans le fichier pour éviter les changements de position du pointeur de position </param>
        Private Sub EcrireJNXRect(R As JNXRect, Optional IsTuile As Boolean = False)
            EcrireJNXPoint(R.northEast, IsTuile)
            EcrireJNXPoint(R.southWest, IsTuile)
        End Sub
        ''' <summary> écrit une structue JNXMapInfo dans le fichier binaire</summary>
        ''' <param name="FMapInfo">structure JNXMapInfo</param>
        Private Sub EcrireJNXMapInfo()
            With FMapInfo
                FileBinaire.Write(.Version)
                FileBinaire.Write(.DeviceSN)
                EcrireJNXRect(.MapBounds)
                FileBinaire.Write(.Levels)
                FileBinaire.Write(.Expiration)
                FileBinaire.Write(.ProductID)
                FileBinaire.Write(.CRC)
                FileBinaire.Write(.SigVersion)
                FileBinaire.Write(.SigOffset)
                FileBinaire.Write(.ZOrder)
            End With
        End Sub
        ''' <summary> écrit une structue JNXMapMeta dans le fichier binaire</summary>
        ''' <param name="FMapMeta">structure JNXMapMeta</param>
        Private Sub EcrireJNXMapMeta(FMapMeta As JNXMapMeta)
            FileBinaire.Write(FMapMeta.Version)
            EcrireStringC(FMapMeta.GUID)
            EcrireStringC(FMapMeta.ProductName)
            EcrireStringC(FMapMeta.Unknow)
            FileBinaire.Write(FMapMeta.ProductID)
            EcrireStringC(FMapMeta.MapName)
            FileBinaire.Write(FMapMeta.LevelMetaCount)
        End Sub
        ''' <summary> écrit une structue JNXLevelInfo dans le fichier binaire </summary>
        ''' <param name="FLevelInfo">structure JNXLevelInfo</param>
        ''' <param name="Position"> indique la position dans le flux binaire si la position courante n'est pas la bonne. 
        ''' Concerne le changement de valeur et pas la création ou l'ajout d'un niveau de détail </param>
        Private Sub EcrireJNXLevelInfo(FLevelInfo As JNXLevelInfo, Optional Position As Integer = -1)
            If Position > -1 Then
                'on est dans la modification du niveau d'affichage
                FileBinaire.Seek(Position, SeekOrigin.Begin)
            End If
            With FLevelInfo
                .Unknow = 2
                FileBinaire.Write(.TileCount)
                FileBinaire.Write(.TilesOffset)
                FileBinaire.Write(.Scale)
                FileBinaire.Write(.Unknow)
                EcrireStringC(.Copyright)
            End With
        End Sub
        ''' <summary> écrit une structue JNXLevelMeta dans le fichier binaire</summary>
        ''' <param name="FLevelMeta">structure JNXLevelMeta</param>
        Private Sub EcrireJNXLevelMeta(FLevelMeta As JNXLevelMeta)
            EcrireStringC(FLevelMeta.Name)
            EcrireStringC(FLevelMeta.Description)
            EcrireStringC(FLevelMeta.Copyright)
            FileBinaire.Write(FLevelMeta.Zoom)
        End Sub
        ''' <summary> écrit une structue JNXTitleInfo dans le fichier binaire</summary>
        ''' <param name="TuileInfo">structure JNXTileInfo</param>
        Private Sub EcrireJNXTileInfo(TuileInfo As JNXTileInfo)
            EcrireJNXRect(TuileInfo.TileBounds, True)
            Infostuiles.Write(TuileInfo.PicWidth)
            Infostuiles.Write(TuileInfo.PicHeight)
            Infostuiles.Write(TuileInfo.PicSize)
            Infostuiles.Write(TuileInfo.PicOffset)
        End Sub
        ''' <summary> écrit une chaine de caractère en utf8 dans le flux binaire en rajoutant un null terminated comme pour les chaines du langage C </summary>
        ''' <param name="S"> Chaine à écrire dans le flux </param>
        Private Sub EcrireStringC(S As String)
            FileBinaire.Write(Encoding.UTF8.GetBytes(S & NullChar))
        End Sub
#End Region
#End Region
    End Class
    ''' <summary> cette classe gère la lecture d'un fichier au format JNX. Seule la version 4 du format est supportée.
    ''' Les coordonnées des tuiles sont exprimées en unitées GPS. Voir fonction JNXCoordToWGS84.
    ''' Les fichiers JNX ont certaines limitations : Niveaux : de 1 à 5, Nb Tuiles : de 1 à 50000, 
    ''' Dimensions des Tuiles : inf ou égal à 1024*1024 et en multiple de 8 si possible, 
    ''' NbOctets Tuiles : inf à 3Mo, NBOctets fichier JNX : inf à 100Mo </summary>
    Private Class JNXReader
        Implements IDisposable
#Region "constantes et variables"
        Private FMapInfo As JNXMapInfo
        Private FLevelsInfo As JNXLevelInfo()
        Private FMapMeta As JNXMapMeta
        Private FLevelsMeta As JNXLevelMeta()
        Private FileBinaire As BinaryReader
        Friend Offset As OffsetJNX
        Friend Structure OffsetJNX
            Friend LevelInfo As UInteger()
            Friend LevelMeta As UInteger()
            Friend MapMeta As Long
        End Structure
#End Region
#Region "procédures et fonctions"
#Region "visibles"
        ''' <summary> ok si l'ouverture du fichier binaire c'est bien passée </summary>
        Friend ReadOnly Property IsOk As Boolean
        ''' <summary> permet de libérer les ressources associées. N'a pas pour vocation a être appelé directement mais indirectement par END USING</summary>
        Friend Sub Dispose() Implements IDisposable.Dispose
            FileBinaire?.Close()
            'nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(Me)
        End Sub
        ''' <summary> unique constructeur de cette classe</summary>
        Friend Sub New(Path As String)
            If OpenFile(Path) Then
                LireMapInfo()
                LireLevelsInfo()
                LireMapMeta()
                LireLevelsMeta()
                IsOk = True
            Else
                IsOk = False
            End If
        End Sub
        ''' <summary> renvoie la structure de type JNXMapInfo du fichier JNX</summary>
        Friend Function GetMapInfo() As JNXMapInfo
            Return FMapInfo
        End Function
        ''' <summary> renvoie la structure de type JNXMapMeta du fichier JNX</summary>
        Friend Function GetMapMeta() As JNXMapMeta
            Return FMapMeta
        End Function
        ''' <summary> renvoie la ou les structures de type JNXLevelInfo du fichier JNX</summary>
        Friend Function GetLevelInfo(Niveau As Integer) As JNXLevelInfo
            If Niveau >= FMapInfo.Levels Then Throw New Exception("Le niveau n'existe pas")
            Return FLevelsInfo(Niveau)
        End Function
        ''' <summary> renvoie la ou les structures de type JNXLevelMeta du fichier JNX</summary>
        Friend Function GetLevelMeta(Niveau As Integer) As JNXLevelMeta
            If Niveau >= FMapInfo.Levels Then Throw New Exception("Le niveau n'existe pas")
            Return FLevelsMeta(Niveau)
        End Function
        ''' <summary> renvoie le nb de tuiles d'un niveau de détail </summary>
        Friend Function GetTileCount(Niveau As Integer) As Integer
            If Niveau >= FMapInfo.Levels Then Throw New Exception("Le niveau n'existe pas")
            Return FLevelsInfo(Niveau).TileCount
        End Function
        ''' <summary> renvoie les informations associées à une tuile d'index : index et du niveau de détail : Niveau</summary>
        Friend Function GetTileInfo(Niveau As Integer, Index As Integer) As JNXTileInfo
            If Niveau >= FMapInfo.Levels Then Throw New Exception("Le niveau n'existe pas")
            If Index >= FLevelsInfo(Niveau).TileCount Then Throw New Exception("La tuile n'existe pas")
            FileBinaire.BaseStream.Position = FLevelsInfo(Niveau).TilesOffset + Index * NbOctetsTileInfo
            Return LireJNXTileInfo(FileBinaire)
        End Function
        ''' <summary> renvoie l'image au format JPEG associée à une tuile d'index : index et du niveau de détail : Niveau</summary>
        Friend Function GetJpegStream(Niveau As Integer, index As Integer) As Byte()
            Try
                Dim TuileInfo As JNXTileInfo = GetTileInfo(Niveau, index)
                FileBinaire.BaseStream.Position = TuileInfo.PicOffset
                Dim Result(TuileInfo.PicSize + 1) As Byte
                Result(0) = &HFF
                Result(1) = &HD8
                FileBinaire.Read(Result, 2, TuileInfo.PicSize)
                Return Result
            Catch Ex As Exception
                AfficherErreur(Ex, "CE1F")
                Return Nothing
            End Try
        End Function
        ''' <summary> lit toutes les tuiles qui composent un niveau du fichier JNX et les enregistre</summary>
        ''' <param name="IndiceNiveau"> Niveau du fichier JNX à lire </param>
        ''' <param name="CheminTuile"> répertoire où enregistrer les tuiles </param>
        Friend Function LireNiveauJNX(IndiceNiveau As Integer, CheminTuile As String) As Boolean
            Dim NbCols = CInt(FLevelsInfo(IndiceNiveau).Copyright.Substring(0, 3))
            Dim X, Y As Integer
            For IndexTuile As Integer = 0 To FLevelsInfo(IndiceNiveau).TileCount - 1
                LireTuileToImage(IndiceNiveau, IndexTuile).Save($"{CheminTuile}C_{X:D3}{Y:D3}.jpg")
                X += 1
                If X = NbCols Then
                    X = 0
                    Y += 1
                End If
            Next
            Return True
        End Function
        ''' <summary> lit une tuile du fichier JNX sous forme d'image .net</summary>
        ''' <param name="Niveau"> niveau dans le fichier</param>
        ''' <param name="index"> index de la tuile dans le niveau </param>
        Friend Function LireTuileToImage(Niveau As Integer, index As Integer) As Image
            Return Image.FromStream(New MemoryStream(GetJpegStream(Niveau, index)))
        End Function
        ''' <summary> vérifie la strucutre de début et de fin du fichier jnx</summary>
        Friend Function VerifierFormatCarteJNX() As Boolean
            FileBinaire.BaseStream.Position = 0
            If FileBinaire.ReadByte <> 4 Then Return False 'le format du fichier JNX n'est pas celui attendu
            If Not LireJNXSignature() Then Return False 'la signature du fichier n'est pas celle attendue
            Return True
        End Function
#End Region
#Region "invisibles"
        ''' <summary> lit le bloc JNXMapInfo de la structure de l'entête du fichier jnx</summary>
        Private Sub LireMapInfo()
            FMapInfo = LireJNXMapInfo()
            If FMapInfo.Version <> 4 Then
                Throw New IndexOutOfRangeException("Unsupprted JNX version: " & FMapInfo.Version)
            End If
            Offset = New OffsetJNX
            ReDim FLevelsInfo(FMapInfo.Levels - 1)
            ReDim Offset.LevelInfo(FMapInfo.Levels - 1)
            ReDim FLevelsMeta(FMapInfo.Levels - 1)
            ReDim Offset.LevelMeta(FMapInfo.Levels - 1)
        End Sub
        ''' <summary> lit le bloc JNXMapMeta de la structure de l'entête du fichier jnx</summary>
        Private Sub LireMapMeta()
            Offset.MapMeta = FileBinaire.BaseStream.Position
            FMapMeta = LireJNXMapMeta()
        End Sub
        ''' <summary> lit le ou les blocs JNXLevelInfo de la structure de l'entête du fichier jnx</summary>
        Private Sub LireLevelsInfo()
            For NumNiveau = 0 To FMapInfo.Levels - 1
                Offset.LevelInfo(NumNiveau) = CUInt(FileBinaire.BaseStream.Position)
                FLevelsInfo(NumNiveau) = LireJNXLevelInfo()
            Next
        End Sub
        ''' <summary> lit le ou les blocs JNXLevelMeta de la structure de l'entête du fichier jnx</summary>
        Private Sub LireLevelsMeta()
            For NumNiveau = 0 To FMapInfo.Levels - 1
                Offset.LevelMeta(NumNiveau) = CUInt(FileBinaire.BaseStream.Position)
                FLevelsMeta(NumNiveau) = LireJNXLevelMeta()
            Next
        End Sub
        ''' <summary> lit et renvoie une structue JNXPoint dans le fichier binaire</summary>
        Private Function LireJNXPoint() As JNXPoint
            LireJNXPoint = New JNXPoint With {
                .Latitude = FileBinaire.ReadInt32,
                .Longitude = FileBinaire.ReadInt32
            }
        End Function
        ''' <summary> lire une structue JNXRect dans le fichier binaire</summary>
        ''' <param name="FileBinaire">fichier binaire</param>
        ''' <remarks>écrit à la position du pointeur de fichier</remarks>
        Private Function LireJNXRect() As JNXRect
            LireJNXRect = New JNXRect With {
                .northEast = LireJNXPoint(),
                .southWest = LireJNXPoint()
            }
        End Function
        ''' <summary> lit une structue JNXMapInfo dans le fichier binaire</summary>
        Private Function LireJNXMapInfo() As JNXMapInfo
            LireJNXMapInfo = New JNXMapInfo With {
                .Version = FileBinaire.ReadInt32,
                .DeviceSN = FileBinaire.ReadInt32,
                .MapBounds = LireJNXRect(),
                .Levels = FileBinaire.ReadInt32,
                .Expiration = FileBinaire.ReadInt32,
                .ProductID = FileBinaire.ReadInt32,
                .CRC = FileBinaire.ReadInt32,
                .SigVersion = FileBinaire.ReadInt32,
                .SigOffset = FileBinaire.ReadUInt32,
                .ZOrder = FileBinaire.ReadInt32
            }
        End Function
        ''' <summary> lit dans le fichier binaire et renvoie une structure JNXLevelInfo </summary>
        Private Function LireJNXLevelInfo() As JNXLevelInfo
            LireJNXLevelInfo = New JNXLevelInfo With {
                .TileCount = FileBinaire.ReadInt32,
                .TilesOffset = FileBinaire.ReadUInt32,
                .Scale = FileBinaire.ReadInt32,
                .Unknow = FileBinaire.ReadInt32,
                .Copyright = LireStringC()
            }
        End Function
        ''' <summary> lit dans le fichier binaire et renvoie une structure JNXTileInfo </summary>
        Private Function LireJNXTileInfo(FileBinaire As BinaryReader) As JNXTileInfo
            LireJNXTileInfo = New JNXTileInfo With {
                .TileBounds = LireJNXRect(),
                .PicWidth = FileBinaire.ReadInt16,
                .PicHeight = FileBinaire.ReadInt16,
                .PicSize = FileBinaire.ReadInt32,
                .PicOffset = FileBinaire.ReadUInt32
            }
        End Function
        ''' <summary> lit dans le fichier binaire et renvoie une structure JNXMapMeta </summary>
        Private Function LireJNXMapMeta() As JNXMapMeta
            LireJNXMapMeta = New JNXMapMeta With {
                .Version = FileBinaire.ReadInt32,
                .GUID = LireStringC(),
                .ProductName = LireStringC(),
                .Unknow = LireStringC(),
                .ProductID = FileBinaire.ReadInt16,
                .MapName = LireStringC(),
                .LevelMetaCount = FileBinaire.ReadInt32
            }
        End Function
        ''' <summary> lit dans le fichier binaire et renvoie une structure JNXLevelMeta </summary>
        Private Function LireJNXLevelMeta() As JNXLevelMeta
            LireJNXLevelMeta = New JNXLevelMeta With {
                .Name = LireStringC(),
                .Description = LireStringC(),
                .Copyright = LireStringC(),
                .Zoom = FileBinaire.ReadInt32
            }
        End Function
        ''' <summary>lit en fin du fichier JNX 8 octects et vérifie qu'ils correspondent à la signature d'un fichier JNX</summary>
        Private Function LireJNXSignature() As Boolean
            Dim Signature As Byte() = Encoding.UTF8.GetBytes(JNX_EOF_SIGNATURE)
            FileBinaire.BaseStream.Position = FileBinaire.BaseStream.Length - Signature.Length
            For Cpt = 0 To Signature.Length - 1
                If FileBinaire.ReadByte <> Signature(Cpt) Then Return False
            Next
            Return True
        End Function
        ''' <summary> lit une chaine de caractère en utf8 dans le flux binaire </summary>
        Private Function LireStringC() As String
            Dim S As New StringBuilder("", 80)
            Dim C As Char = FileBinaire.ReadChar()
            While C <> NullChar
                S.Append(C)
                C = FileBinaire.ReadChar()
            End While
            Return S.ToString
        End Function
        ''' <summary> ouvre le fichier jnx</summary>
        Private Function OpenFile(Path As String) As Boolean
            Try
                FileBinaire = New BinaryReader(File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            Catch Ex As Exception
                AfficherErreur(Ex, "E051")
            End Try
            Return True
        End Function
#End Region
#End Region
    End Class
End Module