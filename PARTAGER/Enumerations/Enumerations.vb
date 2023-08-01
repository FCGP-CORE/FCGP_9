''' <summary> Enumérations et variables associées concernant les sites, DomToms, Echelles et autres énumérations partagées </summary>
Module Enumerations
#Region "Enum Système cartographique"
    ''' <summary>chaque type de projection est exprimé avec une certaine unité.</summary>
    Friend Enum UnitesCoordonnees As Integer
        Aucun = -1 : DMS : Mètre
    End Enum
    ''' <summary>chaque type de cartes doit être référencée avec les coordonnées qui lui correspondent.</summary>
    Friend Enum CoordonneesGeoreferencements As Integer
        Inconnu = -1 : DD : Grille 'pour les cartes Grilles   et DD pour les web mercator
    End Enum
    ''' <summary>permet d'avoir un code plus clair auto-commenté</summary>
    Friend Enum SitesCartographiques As Integer
        Aucun = -1
        Géofoncier = 0 'France métropolitaine & Corse, site à Grille WebMercator avec une portée Nationale
        SuisseMobile = 1 'Suisse, site à Grille Suisse LV03 avec une portée Nationale
        DomTom = 2 'DomsToms Français, site à Grille WebMercator avec une portée Locale
        CyclOSM = 3 'carte à partir des données OSM pour les itinéraires à vélo, site à Grille WebMercator avec une portée Européenne
        OpenTopoMap = 4 'carte à partir des données OSM pour les itinéraires à pied et les détails du relief, site à Grille WebMercator avec une portée Européenne
        IgnEspagnol = 5 'Espagne & Canaries, site à Grille WebMercator avec une portée Nationale
    End Enum
    ''' <summary>clef des sites gérés</summary>
    Friend ReadOnly SitesCartoClefs As String() = {"GF", "SM", "DT", "CY", "OT", "ES"}
    Friend ReadOnly SitesCartoLibelles As String() = {"Géofoncier", "SuisseMobile", "DomTom", "CycleOSM", "OpenTopoMap", "IgnEspagnol"}
    ''' <summary> liste des différentes echelles gérées par FCGP </summary>
    ''' <remarks> 'G G S S G S G S G S G S G S G S S G G G G ; passage tableaux escaliers en tableau linéaire </remarks>
    Friend Enum Echelles As Integer
        _000 = -1 : _004 : _007 : _005 : _010 : _015 : _025 : _030 : _050 : _060 : _100 : _120 : _200 : _250 : _400 : _500 : _600 : _800 : _1000 : _2000 : _4000 : _8000
    End Enum
    ''' <summary> Clés des différentes échelles </summary>
    Friend ReadOnly EchellesClefs As String() = New String() {"004", "007", "005", "010", "015", "025", "030", "050", "060", "100", "120",
                                                              "200", "250", "400", "500", "600", "800", "1000", "2000", "4000", "8000"}
    ''' <summary> Libellés des différentes Eéchelles </summary>
    Friend ReadOnly EchellesLibelles As String() = New String() {"1/4 000", "1/7 500", "1/5 000", "1/10 000", "1/15 000", "1/25 000", "1/30 000", "1/50 000",
                                                                 "1/60 000", "1/100 000", "1/120 000", "1/200 000", "1/250 000", "1/400 000", "1/500 000",
                                                                 "1/600 000", "1/800 000", "1/1 000 000", "1/2 000 000", "1/4 000 000", "1/8 000 000"}
    ''' <summary>permet d'avoir un code plus clair auto-commenté</summary>
    Friend Enum DomsToms As Integer
        aucun = -1 'pour les sites autre que Domtom
        reunion = 0
        martinique = 1
        guadeloupe = 2
        stmartin = 3
        guyane = 4
        mayotte = 5
        stbarthelemy = 6
    End Enum
    ''' <summary> clef des DomsToms </summary>
    Friend ReadOnly DomsTomsClefs As String() = New String() {"reu", "mar", "gua", "stm", "guy", "may", "stb"}
    Friend ReadOnly DomsTomsLibelles As String() = New String() {"reunion", "martinique", "guadeloupe", "stmartin", "guyane", "mayotte", "stbarthelemy"}
#End Region
#Region "Enum  tampon mémoire"
    ''' <summary>Pour le renvoit du type d'erreur dans les fonctions et procdure d'editablebitmap</summary>
    Friend Enum ErreurEditableBitmap As Integer
        Erreur_Fonction = -1
        Ok = 0
        DecalTamponX_incompatible_tampon = 1
        DecalTamponX_LargeurALire_incompatible_tampon = 2
        DecalTamponY_incompatible_tampon = 3
        DecalTamponY_HauteurALire_incompatible_tampon = 4
        DecalCarteX_LargeurALire_incompatible_Image = 5
        DecalCarteY_HauteurALire_incompatible_Image = 6
        CheminCarte_Non_Renseigne = 7
        TamponSource_Non_Renseigne = 8
        ChargeCarte_Non_Renseigne = 9
        Tampon_Donnees_Incompatible = 10
        Donnees_Image_Non_Renseignees = 11
    End Enum
    ''' <summary>indique le type de transfert pour initaliser un tampon avec une image : soit un fichier soit un tampon déjà initialisé</summary>
    Friend Enum TypeImage
        Fichier = 0
        Memoire = 1
        Aucun = -1
    End Enum
#End Region
#Region "Datums, projections et Ellipsoïdes dans FCGP"
    ''' <summary> liste des datums type grille et Ellipsoïdes utilisés par les fonctions de conversion de coordonnées du module </summary>
    Friend Enum Datums As Integer
        Aucun = 0 '
        UTM_ED50 = 1                ' projection avec changement d'ellispsoïde
        Lambert_I = 2               ' projection avec changement d'ellispsoïde
        Lambert_II = 3              ' projection avec changement d'ellispsoïde
        Lambert_IIE = 4             ' projection avec changement d'ellispsoïde
        Lambert_III = 5             ' projection avec changement d'ellispsoïde
        Lambert_IV = 6              ' projection avec changement d'ellispsoïde
        Lambert_93 = 7              ' projection. GRS80 assimilé à WGS84. Pas de changement d'ellipsoïde
        Grille_Suisse = 8           ' projection. Pas de changement d'ellipsoïde, fonctions de transformation directe. Assimilé à LV03
        Grille_Suisse_LV03 = 9      ' projection. Pas de changement d'ellipsoïde, fonctions de transformation directe
        Grille_Suisse_LV95 = 10     ' projection. Pas de changement d'ellipsoïde, fonctions de transformation directe
        UTM_WGS84 = 11              ' projection. Pas de changement d'ellipsoïde
        Latitude_Longitude = 12     ' projection. Pas de changement d'ellipsoïde, fonctions de transformation directe
        Web_Mercator = 13           ' projection. Pas de changement d'ellipsoïde, fonctions de transformation directe
        RGF93 = 14                  'datum. GRS80 assimilé à WGS84. Pas de changement d'ellipsoïde
        WGS84 = 15                  'datum. Système pivot
        ED50 = 16                   'datum avec changement d'ellispsoïde
        NTF = 17                    'datum avec changement d'ellispsoïde
        CH1903 = 18                 'datum avec changement d'ellispsoïde
    End Enum
    Friend ReadOnly DatumsLibelles As String() = {"Aucune", "UTM ED50",
                "Lambert I", "Lambert II", "Lambert IIE", "Lambert III", "Lambert IV",
                "Lambert 93", "Grille Suisse", "Grille Suisse LV03", "Grille Suisse LV95",
                "UTM WGS84", "Latitude Longitude", "Web Mercator",
                "RGF93", "WGS84",
                "ED50", "NTF", "CH1903"}
    ''' <summary> listes des projections acceptées </summary>
    '''<remarks> Datum n'est pas une projection mais bien un Datum </remarks>
    Friend Enum Projections As Integer
        Aucun = 0 : Lambert : UTM : WMercator : LatLon : Suisse : Datum
    End Enum
    Friend ReadOnly LibellesProjections() As String = {"", "Lambert", "Transvers Mercator", "WebMercator",
                                                       "Plate Carree", "Oblique Mercator", "Datum"}
    Friend Enum Ellipsoides As Integer
        Aucun = 0 : C_1880_M : GRS80 : HAY09 : WGS84 : BESSEL_1841
    End Enum
    Friend ReadOnly LibellesEllipsoides() As String = {"", "Ellipsoïde de Clarke modifié IGN", "Ellipsoïde IAG GRS 80",
               "Ellipsoïde International de Hayford 1909", "Ellipsoïde International WGS84", "Ellipsoïde de Bessel"}
#End Region
#Region "Enum Cartes"
    ''' <summary>Enumération liés aux différentes vérifications du fichier georef</summary>
    <Flags>
    Friend Enum VerificationRenvoiCarte As Integer
        ''' <summary>La verification complète du fichier Georef est ok</summary>
        OK = &H0
        ''' <summary>Le nom du fichier BMP est différent de celui du fichier Georef</summary>
        Nom_Carte_Different = &H1
        ''' <summary>La carte n'est pas enregistrée au format BMP</summary>
        Format_Carte_Non_Valide = &H2
        ''' <summary>Le fichier BMP de la carte n'est pas dans le même répertoire que celui du ficheir Georef</summary>
        Carte_Inexistante = &H4
        ''' <summary>La carte a été retaillée</summary>
        Carte_Retaillee = &H8
        ''' <summary>La carte ne possède pas de grille</summary>
        Carte_Sans_Grille = &H10
        ''' <summary>Le fichier Georef est une ancienne version non supportée</summary>
        Georef_Non_Supporte = &H20
        ''' <summary>Le système du fichier Georef est inexistant</summary>
        Systeme_Inexistant = &H40
        ''' <summary>Le système du fichier Georef est différent de celui passé en paramètre</summary>
        Systeme_Different = &H80
        ''' <summary>L'échelle de capture du fichier Georef est inexistante</summary>
        Echelle_Capture_Inexistante = &H100
        ''' <summary>L'échelle de capture du fichier Georef est différente de celle passée en paramètre</summary>
        Echelle_Capture_Differente = &H200
        ''' <summary>Les fonctions de géoréférencement du type de coordonnées ne sont pas connues </summary>
        Type_Coordonnees_Inexistant = &H400
        ''' <summary>Les fonctions de géoréférencement du type de coordonnées ne sont pas connues </summary>
        Fonctions_Georeferencement_Inexistant = &H800
        ''' <summary>Le DomTom du fichier Georef est inexistant</summary>
        DomTom_Inexistant = &H1000
        ''' <summary>Le DomTom du fichier Georef est différent de celui passé en paramètre</summary>
        DomTom_Different = &H2000
        ''' <summary>Le systèmecarto n'a pas pu etre créé</summary>
        Erreur_Creation_Systeme = &H4000
        ''' <summary>Le systèmecarto n'a pas pu etre créé</summary>
        Erreur_Creation_Tampon = &H8000
        ''' ''' <summary>Le systèmecarto n'a pas pu etre créé</summary>
        Erreur_Chargement_Image = &H16000
    End Enum
    ''' <summary> indique les fichiers de géoréférencement associés à la carte. </summary>
    <FlagsAttribute>
    Friend Enum ChoixFichiersTuiles
        Aucun = 0
        ORUX = 1
        JNX = 2
        KMZ = 4
        JNX_KMZ = JNX Or KMZ ' 2 + 4 = 6
        JNX_ORUX = ORUX Or JNX ' 2 + 1 = 3
        KMZ_ORUX = ORUX Or KMZ ' 4 + 1 = 5
        Tous = ORUX Or JNX Or KMZ ' 1 + 2 + 4 = 7
    End Enum
    ''' <summary> indique les fichiers de géoréférencement associés à la carte. </summary>
    <FlagsAttribute>
    Friend Enum ChoixFichiersGeoref
        ''' <summary>aucun fichier Georef et pas d'image de la carte</summary>
        Aucun = 0
        ''' <summary>uniquement le fichier Georef et l'image de la carte</summary>
        Georef_Carte = 1
        ''' <summary>Georef_Carte plus un fichier pour MapInfo</summary>
        MapInfo_Carte = 2
        ''' <summary>Georef_Carte plus un fichier pour OziExploreur</summary>
        OziExploreur_Carte = 4
        ''' <summary>Georef_Carte plus un fichier pour CompeLandGps</summary>
        CompeLand_Carte = 8
        ''' <summary>Georef_Carte plus un fichier pour QGIS</summary>
        QGIS_Carte = 16
        ''' <summary>Georef_Interpol</summary>
        Georef_Interpol = 256
        ''' <summary>Georef_Interpol plus un fichier pour MapInfo pour l'interpolation</summary>
        MapInfo_Interpol = 512
        ''' <summary>Georef_Interpol plus un fichier pour OziExploreur pour l'interpolation</summary>
        OziExploreur_Interpol = 1024
        ''' <summary>Georef_interpol plus un fichier pour CompeLandGps pour l'interpolation</summary>
        CompeLand_Interpol = 2048
        ''' <summary>Georef_Carte plus un fichier pour QGIS pour l'interpolation</summary>
        QGIS_Interpol = 4096
        ''' <summary>tous les fichiers de géoréférencement Georef</summary>
        Georefs = Georef_Carte Or Georef_Interpol ' 1 + 256 = 257
        ''' <summary>tous les fichiers de géoréférencement Mapinfo</summary>
        MapInfos = MapInfo_Carte Or MapInfo_Interpol ' 2 + 512 = 514
        ''' <summary>tous les fichiers de géoréférencement OziExploreur</summary>
        OziExploreurs = OziExploreur_Carte Or OziExploreur_Interpol ' 4 + 1024 = 1028
        ''' <summary>tous les fichiers de géoréférencement CompeLand</summary>
        CompeLands = CompeLand_Carte Or CompeLand_Interpol ' 8 + 2048 = 2056
        ''' <summary>tous les fichiers de géoréférencement QGIS</summary>
        QGISs = QGIS_Carte Or QGIS_Interpol ' 16 + 4096 = 4112
        ''' <summary>tous les fichiers de géoréférencement Carte</summary>
        Cartes = Georef_Carte Or MapInfo_Carte Or OziExploreur_Carte Or CompeLand_Carte Or QGIS_Carte ' 1 + 2 + 4 + 8 + 16 = 31
        ''' <summary>tous les fichiers de géoréférencement Interpol</summary>
        Interpols = Georef_Interpol Or MapInfo_Interpol Or OziExploreur_Interpol Or CompeLand_Interpol Or QGIS_Interpol ' 256 + 512 + 1024 + 2048 + 4096 = 7936
        ''' <summary>tous les fichiers de géoréférencement</summary>
        Tous = Cartes Or Interpols ' 31 + 7936 = 7967
    End Enum
    ''' <summary> indique les fichiers de géoréférencement associés à la carte. </summary>
    <FlagsAttribute>
    Friend Enum ChoixCoordonneesGrille
        ''' <summary>Pas d'image avec les références de la grille</summary>
        Aucune = 0
        ''' <summary>Image de la carte avec reférences de la grille</summary>
        Ref_Carte = 1
        ''' <summary>Image de l'interpolation avec reférences de la grille</summary>
        Ref_Interpol = 2
        ''' <summary>Les 2 ensemble</summary>
        Toutes = Ref_Carte Or Ref_Interpol
    End Enum
    ''' <summary> Permet d'éviter la fonction qui récupère le noms des éléments d'une Enum et de pouvoir la dotfuscer </summary>
    Friend ReadOnly CoordonneesGrille As String() = {"Aucune", "Ref Carte", "Ref Interpol", "Toutes"}
    ''' <summary> définit le type du tampon associé à une carte </summary>
    Friend Enum TypeSupportCarte 'Cartes
        Aucun = -1
        MemoryDynamic = 0 'le tampon est dimensionné dynamiquement en fonction des dimensions de la carte
        Fichier = 1 'le tampon est dimensionné statiquement avec une taille définit à l'avance, le résultat d'un travail est enregistré dans un fichier
        MemoryStatic = 2 'le tampon est dimensionné statiquement avec une taille définit à l'avance, les dimensions de la carte doivent être compatibles avec la taille du tampon
    End Enum
    ''' <summary> Permet d'éviter la fonction qui récupère le noms des éléments d'une Enum et de pouvoir la dotfuscer </summary>
    Friend SupportsCarteNom As String() = {"MemoryDynamic", "Fichier", "MemoryStatic"}
#End Region
#Region "Enum Chronomètres"
    Friend Enum StatutChrono
        Indefini = 0
        Run = 1
        Stoped = 2
        Read = 3
    End Enum
#End Region
#Region "Enum fonctions de conversions DD --> DMS"
    Friend Enum TypesCoordsDD
        Latitude = 0
        Longitude = 1
    End Enum
#End Region
#Region "Enum Curseurs"
    Friend Enum Curseurs As Integer
        Defaut = 0
        CarteDefaut = 1
        CarteDeplacement = 2
        CarteDeplacementHorizontal = 3
        CarteDeplacementVertical = 4
        CarteDeplacementVHS = 5
        CarteDeplacementVSH = 6
        TraceDefaut = 7
        TraceDeplacement = 8
        TraceEdite = 9
        TraceInsertPt = 10
        TraceDeplacePt = 11
        TraceSupprime = 12
        TraceSupprimePtSeg = 13
        TraceCoupe = 14
        TraceCoupePtSeg = 15
        FRefAgrandirHorizontal = 16
        FRefAgrandirVertical = 17
        FRefAgrandir_HD_BG = 18
        FRefAgrandir_HG_BD = 19
        FRefDeplacerVertical = 20
        FRefDeplacerHorizontal = 21
        FRefDeplacer = 22
        Attendre = 23
        Arrow = 24
        Zoom = 25
    End Enum
#End Region
End Module