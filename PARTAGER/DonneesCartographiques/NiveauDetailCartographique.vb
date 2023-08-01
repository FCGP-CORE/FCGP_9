''' <summary> définit une echelle d'un système cartographique </summary>
Friend Class NiveauDetailCartographique
#Region "Functions partagees. renvoie des informations d'échelles associées à un site carto"
    ''' <summary> retourne un integer qui représente l'indice de la clef de l'échelle de capture dans le tableau des échelles pour le site considéré </summary>
    ''' <param name="SiteCarto">indice représentant le système de coordonnées</param>
    ''' <param name="ClefEchelle">Clef de l'Echelle de capture telle qu'indiquée dans le fichier Georef</param>
    Friend Shared Function EchelleClefToSiteIndiceEchelle(SiteCarto As SitesCartographiques, ClefEchelle As String) As Integer
        'on cherche si le nom de la clé existe dans la collection des clés
        Dim IndiceCle = Array.IndexOf(EchellesClefs, ClefEchelle)
        Dim Echelle As Echelles, IndiceEchelle As Integer
        If IndiceCle > -1 Then
            'on transforme l'indice en echelle
            Echelle = CType(IndiceCle, Echelles)
            'et on cherche l'indice de l'échelle pour le site considéré
            IndiceEchelle = Array.IndexOf(EchellesSiteClefs(SiteCarto), Echelle)
        End If
        Return IndiceEchelle
    End Function
    Friend Shared Function EchelleToSiteIndiceEchelle(SiteCarto As SitesCartographiques, Echelle As Echelles) As Integer
        Return Array.IndexOf(EchellesSiteClefs(SiteCarto), Echelle)
    End Function
    Friend Shared Function SiteIndiceEchelleToEchelle(SiteCarto As SitesCartographiques, IndiceEchelle As Integer) As Echelles
        Return EchellesSiteClefs(SiteCarto)(IndiceEchelle)
    End Function
    ''' <summary> echelles possibles pour une site-domtom </summary>
    Friend Shared Function EchellesSite(Site As SitesCartographiques, DomTom As DomsToms) As String()
        Dim NbEchelles = NbEchellesSite(Site, DomTom)
        Dim Echs(NbEchelles - 1) As String
        For Cpt As Integer = 0 To NbEchelles - 1
            Echs(Cpt) = EchellesLibelles(EchellesSiteClefs(Site)(Cpt))
        Next
        Return Echs
    End Function
    Friend Shared Function NbEchellesSite(Site As SitesCartographiques, DomTom As DomsToms) As Integer
        Dim NbEchelles As Integer = If(Site = SitesCartographiques.DomTom, NbEchellesDomsToms(DomTom), EchellesSiteClefs(Site).Length)
        Return NbEchelles
    End Function
    ''' <summary> Facteurs ORUX possibles pour un site-domtom dans l'ordre croissant des échelles </summary>
    Friend Shared Function FacteursORUXSite(Site As SitesCartographiques, DomTom As DomsToms) As Integer()
        Dim NbEchelles = NbEchellesSite(Site, DomTom)
        Dim ORUXs(NbEchelles - 1) As Integer
        For Cpt As Integer = 0 To NbEchelles - 1
            ORUXs(Cpt) = NiveauxAffichagesORUX(EchellesSiteClefs(Site)(Cpt))
        Next
        Return ORUXs
    End Function
    ''' <summary> Facteur ORUX pour un site-domtom-echelle</summary>
    Friend Shared Function FacteurORUX_Defaut(Site As SitesCartographiques, IndiceEchelle As Integer) As Integer
        Return NiveauxAffichagesORUX(EchellesSiteClefs(Site)(IndiceEchelle))
    End Function
    ''' <summary> Facteurs JNX possibles pour un site-domtom dans l'ordre croissant des échelles </summary>
    Friend Shared Function FacteursJNXSite(Site As SitesCartographiques, DomTom As DomsToms) As Double()
        Dim NbEchelles = NbEchellesSite(Site, DomTom)
        Dim JNXs(NbEchelles - 1) As Double
        For Cpt As Integer = 0 To NbEchelles - 1
            JNXs(Cpt) = NiveauxAffichagesJNX(EchellesSiteClefs(Site)(Cpt))
        Next
        Return JNXs
    End Function
    ''' <summary> Facteur JNX pour un site-domtom-echelle</summary>
    Friend Shared Function FacteurJNX_Defaut(Site As SitesCartographiques, IndiceEchelle As Integer) As Double
        Return NiveauxAffichagesJNX(EchellesSiteClefs(Site)(IndiceEchelle))
    End Function
    ''' <summary> Couches possibles pour un site-domtom</summary>
    Friend Shared Function CouchesSite(Site As SitesCartographiques, DomTom As DomsToms) As Byte()
        Dim NbEchelles = NbEchellesSite(Site, DomTom)
        Dim Couches(NbEchelles - 1) As Byte
        For Cpt As Integer = 0 To NbEchelles - 1
            Couches(Cpt) = EchelleToCouche(EchellesSiteClefs(Site)(Cpt))
        Next
        Return Couches
    End Function
    ''' <summary> couche pour un site-echelle </summary>
    Friend Shared Function Couche(Site As SitesCartographiques, IndiceEchelle As Integer) As Byte
        Return EchelleToCouche(EchellesSiteClefs(Site)(IndiceEchelle))
    End Function
#End Region
#Region "Constantes privées concernant les Echelles"
    ''' <summary> nb d'échelles par DomTom. Le site DT gère l'ensemble des échelles des DomsToms, cela permet de renvoyer
    ''' uniquement celles concernées par un DomTom particulier </summary>
    Private Shared ReadOnly NbEchellesDomsToms() As Integer = New Integer() {7, 7, 7, 5, 9, 6, 5}
    ''' <summary> liste des échelles par site. Permet de faire la relation entre les indices d'une liste déroulante des échelles d'un site
    ''' et le N° de couches des serveurs </summary>
    Private Shared ReadOnly EchellesSiteClefs As Echelles()() = {
       New Echelles() {Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                       Echelles._2000, Echelles._4000, Echelles._8000},
       New Echelles() {Echelles._005, Echelles._010, Echelles._025, Echelles._050, Echelles._100, Echelles._200, Echelles._400, Echelles._600, Echelles._800},'Echelles._008 -->_006
       New Echelles() {Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000},
       New Echelles() {Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                       Echelles._2000, Echelles._4000, Echelles._8000},
       New Echelles() {Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                       Echelles._2000, Echelles._4000, Echelles._8000},
       New Echelles() {Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                       Echelles._2000, Echelles._4000, Echelles._8000}}
    ''' <summary> niveau d'affichage avec les GPS Garmin en fonction de l'échelle du site </summary>
    Private Shared ReadOnly NiveauxAffichagesJNX As Double() = New Double() {4.0, 5.0, 5.5, 6.0, 6.0, 7.0, 7.0, 8.0, 8.0, 9.0, 9.0, 10.0, 10.0, 11.0, 11.0, 12.0,
                                                                             13.0, 12.0, 13.0, 14.0, 15.0}
    ''' <summary> niveau d'affichage avec l'application Orux en fonction de l'échelle du site </summary>
    Private Shared ReadOnly NiveauxAffichagesORUX As Integer() = New Integer() {18, 17, 17, 16, 16, 15, 15, 14, 14, 13, 13, 12, 12, 11, 11, 10, 9, 10, 9, 8, 7}
    ''' <summary>stock des pas de grille en fonction des échelles sous forme de texte</summary>
    Private Shared ReadOnly PasGrillesTextes As String() = New String() {"500 m", "1 Km", "1 Km", "1 Km", "1 Km", "1 Km", "2 Km", "5 Km", "5 Km", "10 Km", "10 Km",
                                                                         "50 Km", "25 Km", "", "50 Km", "", "", "", "", "", ""}
    ''' <summary>stock des pas de grille en fonction des échelles sous forme de nombre qui exprime des mètres</summary>
    Private Shared ReadOnly PasGrillesNumerics As Integer() = New Integer() {500, 1000, 1000, 1000, 1000, 1000, 2000, 5000, 5000, 10000, 10000, 50000, 25000, 0, 50000, 0, 0, 0, 0, 0, 0}
    ''' <summary>stock des échelles d'impression en fonction des échelles gérées par site sous forme de texte</summary>
    Private Shared ReadOnly EchellesImpressions As String() = New String() {"1/5000", "1/10000", "1/5000", "1/10000", "1/25000", "1/25000", "1/50000", "1/50000", "1/100000",
                                                                            "1/100000", "1/200000", "1/200000", "1/400000", "", "1/800000", "", "", "", "", "", ""}
    ''' <summary>stock échelles d'impression en fonction des échelles gérées par site sous forme de clef texte</summary>
    Private Shared ReadOnly EchellesImpressionsClefs As String() = New String() {"005", "010", "005", "010", "025", "025", "050", "050", "100", "100",
                                                                                 "200", "200", "400", "", "800", "", "", "", "", "", ""}
    ''' <summary>stock pour les sites avec grilles incorporées la qualité souhaitée pour l'interpolation en fonction des échelles gérées par le site</summary>
    Private Shared ReadOnly QualitesInterpolsGrilles As Integer() = {0, 0, 1500, 2500, 0, 5000, 0, 10000, 0, 25000, 0, 50000, 0, 75000, 0, 100000, 150000, 0, 0, 0, 0}
#End Region
#Region "Information propre à une echelle"
    ''' <summary> constructeur pour la capture d'une carte ou la création d'une carte existante(fichier georef)
    ''' permet d'initialiser une echelle soit à partir de la clef (fichier géoref), soit du libellé (capture) </summary>
    ''' <param name="SiteCarto">site associé à l'échelle</param>
    ''' <param name="ClefEchelle">libellé de l'échelle</param>
    Friend Sub New(SiteCarto As SitesCartographiques, ClefEchelle As String)
        Dim IndiceCle = Array.IndexOf(EchellesClefs, ClefEchelle)
        _Echelle = CType(IndiceCle, Echelles)
        If _Echelle > Echelles._000 Then
            If Array.IndexOf(EchellesSiteClefs(SiteCarto), _Echelle) > -1 Then
                _IsOk = True
            Else
                _Echelle = Echelles._000
            End If
        End If
    End Sub
    ''' <summary> constructeur spécifique au téléchargement des cartes</summary>
    ''' <param name="SiteCarto">site associé à l'échelle</param>
    ''' <param name="Echelle">libellé de l'échelle</param>
    Friend Sub New(SiteCarto As SitesCartographiques, Echelle As Echelles)
        Dim IndiceEchelle As Integer = Array.IndexOf(EchellesSiteClefs(SiteCarto), Echelle)
        If IndiceEchelle > -1 Then
            _Echelle = Echelle
            _IsOk = True
        Else
            _Echelle = Echelles._000
        End If
    End Sub
    ''' <summary>indice en base zéro de l'échelle pour le site carto</summary>
    Friend ReadOnly Property Echelle() As Echelles
    ''' <summary>indique si l'objet a été crée correctement</summary>
    Friend ReadOnly Property IsOk() As Boolean
    '''<summary> renvoi le niveau d'affichage pour un fichier tuile ORUX </summary>
    Friend ReadOnly Property NiveauAffichageORUX() As Integer
        Get
            Return If(_IsOk, NiveauxAffichagesORUX(_Echelle), 0)
        End Get
    End Property
    '''<summary> renvoi le niveau d'affichage pour un fichier tuile JNX </summary>
    Friend ReadOnly Property NiveauAffichageJNX() As Double
        Get
            Return If(_IsOk, NiveauxAffichagesJNX(_Echelle), 0.0)
        End Get
    End Property
    ''' <summary> Echelle de capture des cartes </summary>
    Friend ReadOnly Property Libelle() As String
        Get
            Return If(_IsOk, EchellesLibelles(_Echelle), "Aucun")
        End Get
    End Property
    ''' <summary>clef de l'échelle de capture des cartes. info présente dans le fichier GeoRef. </summary>
    Friend ReadOnly Property Clef() As String
        Get
            Return If(_IsOk, EchellesClefs(_Echelle), "")
        End Get
    End Property
    ''' <summary> Pour les calculs de liés à la grille </summary>
    Friend ReadOnly Property QualiteInterpolGrille() As Integer
        Get
            Return If(_IsOk, QualitesInterpolsGrilles(_Echelle), 0)
        End Get
    End Property
    ''' <summary> Echelle d'impression conseillée. Tableau escaliers IndiceSytème et IndiceEchelleCapture</summary>
    Friend ReadOnly Property Impression() As String
        Get
            Return If(_IsOk, EchellesImpressions(_Echelle), "")
        End Get
    End Property
    ''' <summary> Clef de l'échelle d'impression conseillée. info présente dans le fichier GeoRef </summary>
    Friend ReadOnly Property ImpressionClef() As String
        Get
            Dim Ech As String = ""
            If _IsOk Then
                Ech = EchellesImpressionsClefs(_Echelle)
                'nécessaire pour pouvoir retrouver les dimensions en mètre de la carte à travers les DPI
                If Ech = "" Then Ech = "1000"
            End If
            Return Ech
        End Get
    End Property
    ''' <summary> pour la génération des fichiers OziExploreur </summary>
    Friend ReadOnly Property PasGrilleTexte() As String
        Get
            Return If(_IsOk, PasGrillesTextes(_Echelle), "")
        End Get
    End Property
    ''' <summary> Pour les calculs de liés à la grille </summary>
    Friend ReadOnly Property PasGrilleNumeric() As Integer
        Get
            Return If(_IsOk, PasGrillesNumerics(_Echelle), 0)
        End Get
    End Property
#End Region
End Class