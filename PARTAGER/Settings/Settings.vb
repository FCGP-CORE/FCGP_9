Imports System.Security.Cryptography

Imports FCGP.NiveauDetailCartographique
Imports FCGP.SystemeCartographique

''' <summary> Ecriture et lecture des configurations associées aux différents modules du programme FCGP </summary>
Module Settings
    Private Version As SettingsVersion
    Friend AltitudesSettings As SettingsAltitudes
    Friend FacteursSettings As SettingsFacteurs
    Friend CartesSettings As SettingsCartes
    Friend PartagerSettings As SettingsPartager
    Friend TracesSettings As SettingsTraces
    Friend ConvertirSettings As SettingsConvertir
    Friend RegrouperSettings As SettingsRegrouper
    Friend CapturerSettings As SettingsCapturer

    Friend Class SettingsFacteurs
        '''<summary> rend la classe non instanciable par erreur </summary>
        Private Sub New()
        End Sub
        Friend Sub New(FlagLire As Boolean)
            'il n'y a rien à lire alors on sort
            If FlagLire Then Exit Sub
            Initialiser()
        End Sub
        Private Shared Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [FACTEURS] ([INDICE_SITE] Integer NOT NULL, [INDECH] Integer NOT NULL,
                                                                              [FACTEUR_JNX] Real Not NULL, [FACTEUR_ORUX] Integer Not NULL);
                                                CREATE INDEX [CLEFACTEURS] ON [Facteurs] ([INDICE_SITE] ASC, [INDECH] ASC);"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        ''' <summary> pour FGCP_Convertir et FCGP_REGROUPER </summary>
        Friend Function Lire(IndiceSite As Integer, IndiceEchelle As Integer) As (FacteurJNX As Double, FacteurORUX As Integer)
            INDICE_SITE = IndiceSite
            INDICE_ECHELLE = IndiceEchelle
            LireFacteurs()
            Return (FACTEUR_JNX, FACTEUR_ORUX)
        End Function
        ''' <summary> pour FGCP_CAPTURER </summary>
        Friend Function Lire() As (FacteurJNX As Double, FacteurORUX As Integer)
            INDICE_SITE = CapturerSettings.INDICE_SITE
            INDICE_ECHELLE = CapturerSettings.INDICE_ECHELLE
            LireFacteurs()
            Return (FACTEUR_JNX, FACTEUR_ORUX)
        End Function
        Private Sub LireFacteurs()
            FACTEUR_JNX = 0.0#
            FACTEUR_ORUX = 0
            CommandConnectionSettings.CommandText = $"SELECT [FACTEUR_JNX], [FACTEUR_ORUX] FROM [FACTEURS] 
                                                      WHERE [INDICE_SITE] = {INDICE_SITE} And [INDECH] = {INDICE_ECHELLE};"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Do While Reponse.Read
                    FACTEUR_JNX = CDbl(Reponse.Item("FACTEUR_JNX"))
                    FACTEUR_ORUX = CInt(Reponse.Item("FACTEUR_ORUX"))
                Loop
            End Using
            'si l'enregistrement correspondant n'existe pas on le crée
            If FACTEUR_JNX = 0.0# Then
                FACTEUR_JNX = FacteurJNX_Defaut(CapturerSettings.SITE, INDICE_ECHELLE)
                FACTEUR_ORUX = FacteurORUX_Defaut(CapturerSettings.SITE, INDICE_ECHELLE)
                Inserer()
            End If
        End Sub
        Private Sub Inserer()
            CommandConnectionSettings.CommandText = $"INSERT INTO [FACTEURS] ([INDICE_SITE], [INDECH], [FACTEUR_JNX], [FACTEUR_ORUX])
                                                                      VALUES ({INDICE_SITE}, {INDICE_ECHELLE},
                                                                              {DblToStr(FACTEUR_JNX, PrecisionJNX)}, {FACTEUR_ORUX});"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        ''' <summary> pour FGCP_Convertir et FCGP_REGROUPER </summary>
        Friend Sub Ecrire(IndiceSite As Integer, IndiceEchelle As Integer, FacteurJNX As Double, FacteurOrux As Integer)
            INDICE_SITE = IndiceSite
            INDICE_ECHELLE = IndiceEchelle
            FACTEUR_JNX = FacteurJNX
            FACTEUR_ORUX = FacteurOrux
            EcrireFacteurs()
        End Sub
        ''' <summary> pour FGCP_CAPTURER </summary>
        Friend Sub Ecrire()
            INDICE_SITE = CapturerSettings.INDICE_SITE
            INDICE_ECHELLE = CapturerSettings.INDICE_ECHELLE
            FACTEUR_JNX = CartesSettings.FACTEUR_JNX
            FACTEUR_ORUX = CartesSettings.FACTEUR_ORUX
            EcrireFacteurs()
        End Sub
        Private Sub EcrireFacteurs()
            CommandConnectionSettings.CommandText = $"UPDATE [FACTEURS] 
                                                         SET [FACTEUR_JNX]= {DblToStr(FACTEUR_JNX, PrecisionJNX)}, [FACTEUR_ORUX]= {FACTEUR_ORUX} 
                                                       WHERE [INDICE_SITE] = {INDICE_SITE} And [INDECH]= {INDICE_ECHELLE};"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private INDICE_SITE As Integer
        Private INDICE_ECHELLE As Integer
        Private FACTEUR_JNX As Double
        Private FACTEUR_ORUX As Integer
    End Class
    ''' <summary> configuration concernant les fichiers DEM sous FCGP_Capturer et FCGP_Regrouper </summary>
    Friend Class SettingsAltitudes
#Region "Champs"
        ''' <summary> Répertoire d'enrgistrement des fichiers DEM téléchargés. Valeur défaut '' ou Document du système </summary>
        Private _CHEMIN_DEM As String
        Friend Property CHEMIN_DEM As String
            Get
                If _CHEMIN_DEM = "" OrElse Not Directory.Exists(_CHEMIN_DEM) Then
                    _CHEMIN_DEM = CheminParDefaut & "\DEM"
                    If Not Directory.Exists(_CHEMIN_DEM) Then Directory.CreateDirectory(_CHEMIN_DEM)
                End If
                Return _CHEMIN_DEM
            End Get
            Set(value As String)
                _CHEMIN_DEM = value
            End Set
        End Property
        ''' <summary> Type des fichiers DEM téléchargés. Valeur défaut 3 ou ou 1 point tous les 90 mètres environs </summary>
        Friend TYPE_DEM As Integer
        ''' <summary> Identifiant du compte pour le téléchargement des fichiers DEM. Valeur défaut '' </summary>
        Friend ID As String
        ''' <summary> Mot de passe du compte pour le téléchargement des fichiers DEM. Valeur défaut '' </summary>
        Friend MP As String
        ''' <summary> L'altitude sous le curseur sera affichée avec les coordoonées. Valeur défaut 0 ou false </summary>
        Friend IS_ALTITUDE As Boolean
        ''' <summary> les identifiants ont été validés. Valeur défaut 0 ou false </summary>
        Friend IS_VALIDE As Boolean
        ''' <summary> indique si les messages de téléchargement sont affichés. Valeur défaut -1 ou true </summary>
        Friend IS_MSG As Boolean
#End Region
        '''<summary> rend la class non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [ALTITUDES] WHERE [_rowid_] = 1;"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                _CHEMIN_DEM = CStr(Reponse.Item("CHEMIN_DEM"))
                TYPE_DEM = CInt(Reponse.Item("TYPE_DEM"))
                ID = CStr(Reponse.Item("ID"))
                MP = CrypterString(CStr(Reponse.Item("MP")), NomBaseSettings.Substring(5, 8), False)
                IS_ALTITUDE = CBool(Reponse.Item("IS_ALTITUDE"))
                IS_VALIDE = CBool(Reponse.Item("IS_VALIDE"))
                IS_MSG = CBool(Reponse.Item("IS_MSG"))
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [ALTITUDES] 
                                                         SET [CHEMIN_DEM]= '{_CHEMIN_DEM}', [TYPE_DEM]= {TYPE_DEM}, [ID]= '{ID}', 
                                                             [MP]= '{CrypterString(MP, NomBaseSettings.Substring(5, 8), True)}', 
                                                             [IS_ALTITUDE]= {CInt(IS_ALTITUDE)}, [IS_VALIDE]= {CInt(IS_VALIDE)},
                                                             [IS_MSG]= {CInt(IS_MSG)} 
                                                       WHERE [_rowid_]= 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [ALTITUDES] ([CHEMIN_DEM] Text Not NULL, [TYPE_DEM] Integer Not NULL, [ID] Text Not NULL,
                                                                               [MP] Text Not NULL, [IS_ALTITUDE] Integer NOT NULL, [IS_VALIDE] Integer NOT NULL,
                                                                               [IS_MSG] Integer NOT NULL);
                                                      INSERT INTO [ALTITUDES] ([CHEMIN_DEM], [TYPE_DEM], [ID], [MP], [IS_ALTITUDE], [IS_VALIDE], [IS_MSG]) 
                                                                       VALUES ('', 3, '', '', 0, 0, 0);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> configuration commune concernant les applications FCGP_Capturer, FCGP_Convertir et FCGP_Regrouper </summary>
    Friend Class SettingsPartager
#Region "Champs"
        ''' <summary> N° de l'écran support des applications FCGP. Valeur défaut -1 </summary>
        Friend NUM_ECRAN As Integer
        ''' <summary> Chemin d'enregistrement des fichiers tuiles pour Capture et Visue. Valeur '' </summary>
        Private _CHEMIN_TUILE As String
        Friend Property CHEMIN_TUILE As String
            Get
                If _CHEMIN_TUILE = "" OrElse Not Directory.Exists(_CHEMIN_TUILE) Then
                    _CHEMIN_TUILE = CheminParDefaut & "\TUILES"
                    If Not Directory.Exists(_CHEMIN_TUILE) Then Directory.CreateDirectory(_CHEMIN_TUILE)
                End If
                Return _CHEMIN_TUILE
            End Get
            Set(value As String)
                _CHEMIN_TUILE = value
            End Set
        End Property
        ''' <summary> Couleur de fond des affichages pour Capture et Visue. Valeur défaut 105, 105, 105 ou darkgrey</summary>
        Friend COULEUR_VISUE As Color
        ''' <summary> Couleur de fond des cartes quand il n'y a pas de données carto. Valeur défaut 255, 255, 255 ou blanc </summary>
        Friend COULEUR_CARTE As Color

        Friend QUALITE_JPEG As Integer
#End Region
        '''<summary> rend la class non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [PARTAGER] WHERE [_rowid_] = 1;"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                NUM_ECRAN = CInt(Reponse.Item("ECRAN"))
                _CHEMIN_TUILE = CStr(Reponse.Item("CHEMIN_TUILE"))
                COULEUR_VISUE = Color.FromArgb(CInt(Reponse.Item("COUL_VISUE")))
                COULEUR_CARTE = Color.FromArgb(CInt(Reponse.Item("COUL_CARTE")))
                QUALITE_JPEG = CInt(Reponse.Item("QUALITE_JPEG"))
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [PARTAGER] 
                                                         SET [ECRAN]= {NUM_ECRAN}, [CHEMIN_TUILE]= '{_CHEMIN_TUILE}', 
                                                             [COUL_VISUE]= {COULEUR_VISUE.ToArgb}, [COUL_CARTE]= {COULEUR_CARTE.ToArgb}, 
                                                             [QUALITE_JPEG]={QUALITE_JPEG}
                                                       WHERE [_rowid_]= 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [PARTAGER] ([ECRAN] Integer NOT NULL, [CHEMIN_TUILE] Text Not NULL, 
                                                                              [COUL_VISUE] Integer Not NULL, [COUL_CARTE] Integer NOT NULL,
                                                                              [QUALITE_JPEG] Integer NOT NULL);
                                                      INSERT INTO [PARTAGER] ([ECRAN], [CHEMIN_TUILE], [COUL_VISUE], [COUL_CARTE], [QUALITE_JPEG]) 
                                                                      VALUES (-1, '', -9868951, -1, 75);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> configuration concernant l'application FCGP_Convertir </summary>
    Friend Class SettingsConvertir
#Region "Champs"
        ''' <summary> L'affichage de la grile est demandé. Valeur défaut 0 ou Aucun </summary>
        Friend GEOREF As Integer
        ''' <summary> Format d'enregistrement de la carte. Valeur défaut 0 ou Bmp </summary>en
        Friend FORMAT As Integer
        ''' <summary> L'affichage de la grille est demandé. Valeur défaut 1 ou true </summary>
        Friend IS_GRILLE As Boolean
        ''' <summary> Références de grilles demandées. Valeur défaut 0 ou Aucune </summary>
        Friend REFERENCE As Integer
        ''' <summary> Répertoire d'enrgistrement des cartes générées. Valeur défaut '' ou Document du système </summary>
        Private _CHEMIN_CARTE As String
        Friend Property CHEMIN_CARTE As String
            Get
                If _CHEMIN_CARTE = "" OrElse Not Directory.Exists(_CHEMIN_CARTE) Then
                    _CHEMIN_CARTE = CheminParDefaut & "\CONVERT_CARTES"
                    If Not Directory.Exists(_CHEMIN_CARTE) Then Directory.CreateDirectory(_CHEMIN_CARTE)
                End If
                Return _CHEMIN_CARTE
            End Get
            Set(value As String)
                _CHEMIN_CARTE = value
            End Set
        End Property
        ''' <summary> Fichiers tuile demandés. Valeur défaut 0 ou Aucun </summary>
        Friend FICHIER_TUILE As Integer
        ''' <summary> Dimensions des tuiles des fichiers tuile. Valeur défaut 512 </summary>
        Friend DIMENSIONS_TUILE As Integer
        ''' <summary> Répertoire d'enrgistrement des fichiers tuile générés. Valeur défaut '' ou Document du système </summary>
        Private _CHEMIN_TUILE As String
        Friend Property CHEMIN_TUILE As String
            Get
                If _CHEMIN_TUILE = "" OrElse Not Directory.Exists(_CHEMIN_TUILE) Then
                    _CHEMIN_TUILE = CheminParDefaut & "\CONVERT_TUILES"
                    If Not Directory.Exists(_CHEMIN_TUILE) Then Directory.CreateDirectory(_CHEMIN_TUILE)
                End If
                Return _CHEMIN_TUILE
            End Get
            Set(value As String)
                _CHEMIN_TUILE = value
            End Set
        End Property
        Friend COULEUR_TRACE As Color
        Friend EPAISSEUR_TRACE As Integer
        Friend NB_PATHS As Integer
        Friend IS_TRACE As Boolean
#End Region
        '''<summary> rend la class non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [CONVERTIR] WHERE [_rowid_] = 1;"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                GEOREF = CInt(Reponse.Item("GEOREF"))
                FORMAT = CInt(Reponse.Item("FORMAT"))
                REFERENCE = CInt(Reponse.Item("REFERENCE"))
                IS_GRILLE = CBool(Reponse.Item("IS_GRILLE"))
                _CHEMIN_CARTE = CStr(Reponse.Item("CHEMIN_CARTE"))
                FICHIER_TUILE = CInt(Reponse.Item("FICHIER_TUILE"))
                DIMENSIONS_TUILE = CInt(Reponse.Item("DIMENSIONS_TUILE"))
                _CHEMIN_TUILE = CStr(Reponse.Item("CHEMIN_TUILE"))
                COULEUR_TRACE = Color.FromArgb(CInt(Reponse.Item("COULEUR_TRACE")))
                EPAISSEUR_TRACE = CInt(Reponse.Item("EPAISSEUR_TRACE"))
                NB_PATHS = CInt(Reponse.Item("NB_PATHS"))
                IS_TRACE = CBool(Reponse.Item("IS_TRACE"))
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [CONVERTIR] 
                                                         SET [GEOREF]= {GEOREF}, [FORMAT]= {FORMAT}, [IS_GRILLE]= {CInt(IS_GRILLE)}, 
                                                             [REFERENCE]= {REFERENCE}, [CHEMIN_CARTE]= '{_CHEMIN_CARTE}', 
                                                             [FICHIER_TUILE]= {FICHIER_TUILE}, [DIMENSIONS_TUILE]= {DIMENSIONS_TUILE}, 
                                                             [CHEMIN_TUILE]= '{_CHEMIN_TUILE}', [COULEUR_TRACE]= {COULEUR_TRACE.ToArgb()}, 
                                                             [EPAISSEUR_TRACE]= {EPAISSEUR_TRACE}, [NB_PATHS]= {NB_PATHS}, [IS_TRACE]= {CInt(IS_TRACE)} 
                                                       WHERE [_rowid_]= 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [CONVERTIR] ([GEOREF] Integer Not NULL, [FORMAT] Integer Not NULL,
                                                                               [IS_GRILLE] Integer Not NULL, [REFERENCE] Integer Not NULL, 
                                                                               [CHEMIN_CARTE] Text Not NULL, [FICHIER_TUILE] Integer Not NULL,  
                                                                               [DIMENSIONS_TUILE] Integer Not NULL,[CHEMIN_TUILE] Text Not NULL,
                                                                               [COULEUR_TRACE] Integer Not NULL, [EPAISSEUR_TRACE] Integer Not NULL, 
                                                                               [NB_PATHS] Integer Not NULL, [IS_TRACE] Integer Not NULL);
                                                      INSERT INTO [CONVERTIR] ([GEOREF], [FORMAT], [IS_GRILLE], [REFERENCE], [CHEMIN_CARTE],
                                                                               [FICHIER_TUILE], [DIMENSIONS_TUILE], [CHEMIN_TUILE],
                                                                               [COULEUR_TRACE], [EPAISSEUR_TRACE], [NB_PATHS], [IS_TRACE]) 
                                                                       VALUES (0, 0, 1, 0, '', 0, 512, '', -1722176257, 5, 0, 0);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> configuration concernant l'application FCGP_Capturer </summary>
    Friend Class SettingsCapturer
#Region "Champs"
        ''' <summary> Coordonnées du centre de l'affichage du dernier site séléctionné en tuile </summary>
        Friend ReadOnly Property CENTRE_AFFICHAGE_SITE As PointG
            Get
                Return New PointG(INDICE_ECHELLE, New Point(X, Y))
            End Get
        End Property

        Private _INDICE_SITE As Integer
        ''' <summary>  Dernier site affiché avant de sortir de l'application. Indice unique SiteDomTom</summary>
        Friend Property INDICE_SITE As Integer
            Get
                Return _INDICE_SITE
            End Get
            Set(value As Integer)
                _INDICE_SITE = value
                Dim SiteDomTom = SystemeCartographique.IndexToSiteDomTom(_INDICE_SITE)
                _SITE = SiteDomTom.Site
                _DOMTOM = SiteDomTom.DomTom
            End Set
        End Property
        ''' <summary>  Dernier site affiché avant de sortir de l'application.</summary>
        Friend ReadOnly Property SITE As SitesCartographiques
        ''' <summary>  Dernier domtom affiché avant de sortir de l'application. Valeur défaut 0 ou réunion </summary>
        Friend ReadOnly Property DOMTOM As DomsToms
        ''' <summary>  Chemin d'enregistrement des cartes faites à partir de FCGP_Capture. Valeur défaut '' ou Document du système </summary>
        Private _CHEMIN_CARTE As String
        Friend Property CHEMIN_CARTE As String
            Get
                If _CHEMIN_CARTE = "" OrElse Not Directory.Exists(_CHEMIN_CARTE) Then
                    _CHEMIN_CARTE = CheminParDefaut & "\CARTES"
                    If Not Directory.Exists(_CHEMIN_CARTE) Then Directory.CreateDirectory(_CHEMIN_CARTE)
                End If
                Return _CHEMIN_CARTE
            End Get
            Set(value As String)
                _CHEMIN_CARTE = value
            End Set
        End Property
        ''' <summary>  Chemin d'enregistrement des caches utilisés par FCGP_Capture. Valeur défaut '' ou Document du système </summary>
        Private _CHEMIN_CACHE As String
        Friend Property CHEMIN_CACHE As String
            Get
                If _CHEMIN_CACHE = "" OrElse Not Directory.Exists(_CHEMIN_CACHE) Then
                    _CHEMIN_CACHE = CheminParDefaut
                    If Not Directory.Exists(_CHEMIN_CACHE) Then Directory.CreateDirectory(_CHEMIN_CACHE)
                End If
                Return _CHEMIN_CACHE
            End Get
            Set(value As String)
                _CHEMIN_CACHE = value
            End Set
        End Property
        ''' <summary>  Type de coordonnées du site sélectionné. Valeur défaut 0 ou coordonnées grille du site web </summary>
        Friend INDICE_TYPE_COORDONNEES As Integer
        ''' <summary>  Indice Echelle du site selectionné. Valeur défaut 0 correspond à celle du centre du site web </summary>
        Friend INDICE_ECHELLE As Integer
        ''' <summary>  Indice du CoefAlpha de la couche des Pentes du site selectionné. Valeur défaut 1 ou 33% </summary>
        Friend INDICE_COEF_ALPHA_PENTES As Integer
        ''' <summary>  Indice du fond de plan sur l'affichage . Valeur défaut 0 ou Cartes IGN </summary>
        Friend INDICE_FOND_PLAN As Integer
        ''' <summary> Indice du dernier site OSM visité par le site de capture . Valeur défaut 0 ou ClycleOSM </summary>
        Friend INDICE_OSM As Integer
        ''' <summary>  Coordonnées virtuelles du centre de l'affichage du site séléctionné. Valeur défaut point empty </summary>
        Friend Property LOCATION_CENTRE As Point
            Get
                Return New Point(X, Y)
            End Get
            Set(value As Point)
                X = value.X
                Y = value.Y
            End Set
        End Property
        ''' <summary> Coordonnées X du centre de l'affichage du site séléctionné. Valeur défaut 0 </summary>
        Private X As Integer
        ''' <summary> Coordonnées Y du centre de l'affichage du site séléctionné. Valeur défaut 0 </summary>
        Private Y As Integer
        ''' <summary> Coordonnées du point1 de séléction du site séléctionné. Valeur défaut 0,0 ou point empty </summary>
        Friend Property POINT1 As PointD
            Get
                If IS_NOTOK0 Then
                    Return New PointD
                Else
                    Return New PointD(X0, Y0)
                End If
            End Get
            Set(value As PointD)
                X0 = value.X
                Y0 = value.Y
                IS_NOTOK0 = value.IsEmpty
            End Set
        End Property
        ''' <summary>  le point1 de sélection est OK. Valeur défaut -1  ou not ok </summary>
        Private IS_NOTOK0 As Boolean
        ''' <summary> Coordonnées X du point 1 de séléction du site séléctionné. Valeur défaut 0 </summary>
        Private X0 As Double
        ''' <summary> Coordonnées Y du point 1 de séléction du site séléctionné. Valeur défaut 0 </summary>
        Private Y0 As Double
        ''' <summary> Coordonnées du point2 de séléction du site séléctionné. Valeur défaut 0,0 ou point empty </summary>
        Friend Property POINT2 As PointD
            Get
                If IS_NOTOK2 Then
                    Return New PointD
                Else
                    Return New PointD(X2, Y2)
                End If
            End Get
            Set(value As PointD)
                X2 = value.X
                Y2 = value.Y
                IS_NOTOK2 = value.IsEmpty
            End Set
        End Property
        ''' <summary> le point2 de sélection est OK. Valeur défaut -1  ou not ok </summary>
        Private IS_NOTOK2 As Boolean
        ''' <summary> Coordonnées X du point 2 de séléction du site séléctionné. Valeur défaut 0 </summary>
        Private X2 As Double
        ''' <summary> Coordonnées Y du point 2 de séléction du site séléctionné. Valeur défaut 0 </summary>
        Private Y2 As Double
        ''' <summary> Nom de la trace encours pour le site ou domtom </summary>
        Friend NOM_TRACE As String
        ''' <summary> flag indiquant si une échelle est affichée dans le coin bas droit de la visue </summary>
        Friend IS_AFFICHE_ECHELLE As Boolean
        ''' <summary> flag indiquant si la couche des pentes est affichée sur le fond de plan </summary>
        Friend IS_AFFICHE_PENTES As Boolean
#End Region
        '''<summary> rend la classe non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [CAPTURER] WHERE [_rowid_] = 1;"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                _INDICE_SITE = CInt(Reponse.Item("INDICE_SITE"))
                _CHEMIN_CARTE = CStr(Reponse.Item("CHEMIN_CARTE"))
                _CHEMIN_CACHE = CStr(Reponse.Item("CHEMIN_CACHE"))
                IS_AFFICHE_ECHELLE = CBool(Reponse.Item("IS_AFFICHE_ECHELLE"))
                IS_AFFICHE_PENTES = CBool(Reponse.Item("IS_AFFICHE_PENTES"))
                INDICE_COEF_ALPHA_PENTES = CInt(Reponse.Item("IND_COEF_ALPHA_PENTES"))
                Dim SiteDomTom = IndexToSiteDomTom(_INDICE_SITE)
                _SITE = SiteDomTom.Site
                _DOMTOM = SiteDomTom.DomTom
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [CAPTURER] 
                                                         SET [INDICE_SITE]= {_INDICE_SITE}, [CHEMIN_CARTE]= '{_CHEMIN_CARTE}',  
                                                             [CHEMIN_CACHE]= '{_CHEMIN_CACHE}', [IS_AFFICHE_ECHELLE]= {CInt(IS_AFFICHE_ECHELLE)}, 
                                                             [IS_AFFICHE_PENTES]= {CInt(IS_AFFICHE_PENTES)}, 
                                                             [IND_COEF_ALPHA_PENTES]= {INDICE_COEF_ALPHA_PENTES}
                                                       WHERE [_rowid_]= 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Friend Sub LirePosition()
            Dim PositionsExiste As Boolean
            CommandConnectionSettings.CommandText = $"SELECT * FROM [POSITIONS] WHERE [INDICE_SITE] = {_INDICE_SITE};"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                PositionsExiste = Reponse.HasRows
                Do While Reponse.Read()
                    INDICE_ECHELLE = CInt(Reponse.Item("INDECH"))
                    INDICE_TYPE_COORDONNEES = CInt(Reponse.Item("COORD_TYPE"))
                    X = CInt(Reponse.Item("X"))
                    Y = CInt(Reponse.Item("Y"))
                    IS_NOTOK0 = CBool(Reponse.Item("IS_NOTOK0"))
                    X0 = CDbl(Reponse.Item("X0"))
                    Y0 = CDbl(Reponse.Item("Y0"))
                    IS_NOTOK2 = CBool(Reponse.Item("IS_NOTOK2"))
                    X2 = CDbl(Reponse.Item("X2"))
                    Y2 = CDbl(Reponse.Item("Y2"))
                    NOM_TRACE = CStr(Reponse.Item("NOM_TRACE"))
                    INDICE_FOND_PLAN = CInt(Reponse.Item("IND_FOND_PLAN"))
                    INDICE_OSM = CInt(Reponse.Item("IND_OSM"))
                Loop
            End Using
            If Not PositionsExiste Then
                'on insert l'enregistrement correspondant avec des valeurs par défaut car il n'existe pas encore
                Dim Coord As PointG = CentreSiteWeb(_INDICE_SITE)
                INDICE_ECHELLE = Coord.IndiceEchelle
                INDICE_TYPE_COORDONNEES = If(_SITE = SitesCartographiques.SuisseMobile, 0, 2)
                X = Coord.X
                Y = Coord.Y
                IS_NOTOK0 = True
                X0 = 0
                Y0 = 0
                IS_NOTOK2 = True
                X2 = 0
                Y2 = 0
                NOM_TRACE = ""
                INDICE_FOND_PLAN = 0
                INDICE_OSM = 0
                InsererPosition()
            End If
        End Sub
        Private Sub InsererPosition()
            CommandConnectionSettings.CommandText = $"INSERT INTO [POSITIONS] ([INDICE_SITE], [INDECH], [COORD_TYPE], [X], [Y], [IS_NOTOK0],  
                                                                               [X0], [Y0], [IS_NOTOK2], [X2], [Y2], [NOM_TRACE], [IND_FOND_PLAN], [IND_OSM])
                                                                       VALUES ({_INDICE_SITE}, {INDICE_ECHELLE}, {INDICE_TYPE_COORDONNEES}, {X}, {Y},
                                                                               {CInt(IS_NOTOK0)}, {DblToStr(X0, "N4")}, {DblToStr(Y0, "N4")}, {CInt(IS_NOTOK2)}, 
                                                                               {DblToStr(X2, "N4")}, {DblToStr(Y2, "N4")}, '{NOM_TRACE}', {INDICE_FOND_PLAN}, {INDICE_OSM});"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Friend Sub EcrirePosition()
            CommandConnectionSettings.CommandText = $"UPDATE [POSITIONS] 
                                                         SET [INDECH]= {INDICE_ECHELLE}, [COORD_TYPE]= {INDICE_TYPE_COORDONNEES}, [X]= {X}, [Y]= {Y}, 
                                                             [IS_NOTOK0] = {CInt(IS_NOTOK0)}, [X0] = {DblToStr(X0, "0.0000")}, [Y0] = {DblToStr(Y0, "0.0000")},
                                                             [IS_NOTOK2] = {CInt(IS_NOTOK2)}, [X2] = {DblToStr(X2, "0.0000")}, [Y2] = {DblToStr(Y2, "0.0000")}, 
                                                             [NOM_TRACE] = '{NOM_TRACE}', [IND_FOND_PLAN] = {INDICE_FOND_PLAN}, [IND_OSM] = {INDICE_OSM} 
                                                       WHERE [INDICE_SITE]= {_INDICE_SITE};"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [CAPTURER] ([INDICE_SITE] Integer Not NULL, [CHEMIN_CARTE] Text Not NULL, 
                                                                              [CHEMIN_CACHE] Text Not NULL, [IS_AFFICHE_ECHELLE] Integer Not NULL, 
                                                                              [IS_AFFICHE_PENTES] Integer Not NULL, [IND_COEF_ALPHA_PENTES] Integer NOT NULL);
                                                      INSERT INTO [CAPTURER] ([INDICE_SITE], [CHEMIN_CARTE], [CHEMIN_CACHE], [IS_AFFICHE_ECHELLE], 
                                                                              [IS_AFFICHE_PENTES], [IND_COEF_ALPHA_PENTES]) 
                                                                      VALUES (0, '', '', 0, 0, 1);
                                                    CREATE TABLE [POSITIONS] ([INDICE_SITE] Integer NOT NULL, [INDECH] Integer NOT NULL,
                                                                              [COORD_TYPE] Integer NOT NULL, [X] Integer NOT NULL, [Y] Integer NOT NULL,
                                                                              [IS_NOTOK0] Integer NOT NULL, [X0] Real NOT NULL, [Y0] Real NOT NULL, 
                                                                              [IS_NOTOK2] Integer NOT NULL, [X2] Real NOT NULL, [Y2] Real NOT NULL,
                                                                              [NOM_TRACE] Text Not NULL, [IND_FOND_PLAN] Integer NOT NULL, [IND_OSM] Integer NOT NULL);
                                  CREATE INDEX [CLEPOSITIONS] ON [POSITIONS] ([INDICE_SITE] ASC);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> configuration concernant la génération des cartes sous FCGP_Capturer, FCGP_Convertir et FCGP_Regrouper </summary>
    Friend Class SettingsCartes
#Region "Champs"
        ''' <summary> Les références de la grille sont demandées. Valeur défaut 0 ou false </summary>
        Friend IS_REFERENCES As Boolean
        ''' <summary> fichiers tuile demandé. Valeur défaut 0 ou Aucun </summary>
        Friend FICHIER_TUILE As Integer
        ''' <summary> Fichier de réféencement demandé. Valeur défaut 0 ou .Georef </summary>
        Friend GEOREF As Integer
        ''' <summary> Le dessin de la trace est demandé. Valeur défaut 0 ou false </summary>
        Friend IS_TRACE As Boolean
        ''' <summary> L'affichage de la grille est demandé. Valeur défaut -1 ou true </summary>
        Private _IS_GRILLE As Boolean
        Friend Property IS_GRILLE As Boolean
            Get
                If SystemeCartographique.GrilleSiteExiste(CapturerSettings.SITE) Then
                    Return True
                Else
                    Return _IS_GRILLE
                End If
            End Get
            Set(value As Boolean)
                If Not SystemeCartographique.GrilleSiteExiste(CapturerSettings.SITE) Then
                    _IS_GRILLE = value
                End If
            End Set
        End Property
        ''' <summary> Dimensions des tuiles des fichiers tuile. Valeur défaut 512 </summary>
        Friend DIMENSIONS_TUILE As Integer
        ''' <summary> Format d'enregistrement de la carte. Valeur défaut 0 ou Bmp </summary>
        Friend FORMAT As Integer
        ''' <summary> Support physique du tampon de la carte. Valeur défaut 0 ou Mémoire </summary>
        Private SUPPORT_TAMPON As Integer
        Friend Property SUPPORT_CARTE As TypeSupportCarte
            Get
                Return If(SUPPORT_TAMPON = 0, If(FlagSmallMemory, TypeSupportCarte.MemoryStatic, TypeSupportCarte.MemoryDynamic), TypeSupportCarte.Fichier)
            End Get
            Set(value As TypeSupportCarte)
                SUPPORT_TAMPON = If(value = TypeSupportCarte.Fichier, 1, 0)
            End Set
        End Property
        ''' <summary> On accepte ous les formats d'eregistrement de la carte en cas de suport tampon Fichier. Valeur défaut 0 ou false </summary>
        Friend IS_TOUT_FORMAT As Boolean
        ''' <summary> Facteur JNX. Valeur obtenue lors de l'ouverture du form Preference. n'est pas enregistrée </summary>
        Friend FACTEUR_JNX As Double
        ''' <summary> Facteur ORUX. Valeur défaut du site web qui dépend du site et de l'échelle de capture </summary>
        Friend FACTEUR_ORUX As Integer
#End Region
        '''<summary> rend la class non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [CARTES] WHERE [_rowid_] = 1;"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                IS_REFERENCES = CBool(Reponse.Item("IS_REFERENCES"))
                FICHIER_TUILE = CInt(Reponse.Item("FICHIER_TUILE"))
                GEOREF = CInt(Reponse.Item("GEOREF"))
                _IS_GRILLE = CBool(Reponse.Item("IS_GRILLE"))
                DIMENSIONS_TUILE = CInt(Reponse.Item("DIMENSIONS_TUILE"))
                FORMAT = CInt(Reponse.Item("FORMAT"))
                SUPPORT_TAMPON = CInt(Reponse.Item("SUPPORT_TAMPON"))
                IS_TOUT_FORMAT = CBool(Reponse.Item("IS_TOUT_FORMAT"))
                IS_TRACE = CBool(Reponse.Item("IS_TRACE"))
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [CARTES] 
                                                         SET [IS_REFERENCES]= {CInt(IS_REFERENCES)}, [FICHIER_TUILE]= {FICHIER_TUILE}, [GEOREF]= {GEOREF}, 
                                                             [IS_GRILLE]= {CInt(_IS_GRILLE)}, [DIMENSIONS_TUILE]= {DIMENSIONS_TUILE}, 
                                                             [FORMAT]= {FORMAT}, [SUPPORT_TAMPON]= {SUPPORT_TAMPON}, 
                                                             [IS_TOUT_FORMAT]= {CInt(IS_TOUT_FORMAT)}, [IS_TRACE]= {CInt(IS_TRACE)}
                                                       WHERE [_rowid_]= 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [CARTES] ([IS_REFERENCES] Integer Not NULL, [FICHIER_TUILE] Integer Not NULL,
                                                                            [GEOREF] Integer Not NULL, [IS_GRILLE] Integer Not NULL, [DIMENSIONS_TUILE] Integer Not NULL,
                                                                            [FORMAT] Integer Not NULL, [SUPPORT_TAMPON] Integer Not NULL, 
                                                                            [IS_TOUT_FORMAT] Integer Not NULL, [IS_TRACE] Integer Not NULL);
                                                      INSERT INTO [CARTES] ([IS_REFERENCES], [FICHIER_TUILE], [GEOREF], [IS_GRILLE], [DIMENSIONS_TUILE], 
                                                                            [FORMAT], [SUPPORT_TAMPON], [IS_TOUT_FORMAT], [IS_TRACE]) 
                                                                    VALUES (0, 0, 1, -1, 512, 0, 0, 0, 0);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> configuration concernant les traces sous FCGP_Capturer </summary>
    Friend Class SettingsTraces
#Region "Champs"
        ''' <summary> Répertoire d'enrgistrement des fichiers DEM téléchargés. Valeur défaut '' ou Document du système </summary>
        Private _CHEMIN_TRACE As String
        Friend Property CHEMIN_TRACE As String
            Get
                If _CHEMIN_TRACE = "" OrElse Not Directory.Exists(_CHEMIN_TRACE) Then
                    _CHEMIN_TRACE = CheminParDefaut & "\TRK"
                    If Not Directory.Exists(_CHEMIN_TRACE) Then Directory.CreateDirectory(_CHEMIN_TRACE)
                End If
                Return _CHEMIN_TRACE
            End Get
            Set(value As String)
                _CHEMIN_TRACE = value
            End Set
        End Property
        Friend SENSIB As Integer
        Friend PT_SV As Color
        Friend SEG_SV As Color
        Friend SEG_PT_SV As Color
        Friend PT_SP As Color
        Friend SEG_SP As Color
        Friend PT_AJ As Color
        Friend SEG_AJ As Color
        Friend SEG_TRK As Color
        Friend COULEUR_TRACE As Color
        Friend EPAISSEUR_TRACE As Integer
#End Region
        '''<summary> rend la class non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [TRACES] WHERE [_rowid_] = 1;"
            Using Reponse = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                SENSIB = CInt(Reponse.Item("SENSIB"))
                PT_SV = Color.FromArgb(CInt(Reponse.Item("PT_SV")))
                SEG_SV = Color.FromArgb(CInt(Reponse.Item("SEG_SV")))
                SEG_PT_SV = Color.FromArgb(CInt(Reponse.Item("SEG_PT_SV")))
                PT_SP = Color.FromArgb(CInt(Reponse.Item("PT_SP")))
                SEG_SP = Color.FromArgb(CInt(Reponse.Item("SEG_SP")))
                PT_AJ = Color.FromArgb(CInt(Reponse.Item("PT_AJ")))
                SEG_AJ = Color.FromArgb(CInt(Reponse.Item("SEG_AJ")))
                SEG_TRK = Color.FromArgb(CInt(Reponse.Item("SEG_TRK")))
                _CHEMIN_TRACE = CStr(Reponse.Item("CHEMIN_TRACE"))
                COULEUR_TRACE = Color.FromArgb(CInt(Reponse.Item("COULEUR_TRACE")))
                EPAISSEUR_TRACE = CInt(Reponse.Item("EPAISSEUR_TRACE"))
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [TRACES] 
                                                         SET [SENSIB]= {SENSIB}, [PT_SV]= {PT_SV.ToArgb}, 
                                                             [SEG_SV]= {SEG_SV.ToArgb}, [SEG_PT_SV]= {SEG_PT_SV.ToArgb},  
                                                             [PT_SP]= {PT_SP.ToArgb}, [SEG_SP]= {SEG_SP.ToArgb},
                                                             [PT_AJ]= {PT_AJ.ToArgb}, [SEG_AJ]= {SEG_AJ.ToArgb}, 
                                                             [SEG_TRK]= {SEG_TRK.ToArgb}, [CHEMIN_TRACE]= '{_CHEMIN_TRACE}',
                                                             [COULEUR_TRACE]= {COULEUR_TRACE.ToArgb()}, [EPAISSEUR_TRACE]= {EPAISSEUR_TRACE}
                                                       WHERE [_rowid_]= 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = $"CREATE TABLE [TRACES] ([SENSIB] Integer Not NULL, [PT_SV] Integer Not NULL,
                                                                             [SEG_SV] Integer Not NULL, [SEG_PT_SV] Integer Not NULL, 
                                                                             [PT_SP] Integer Not NULL, [SEG_SP] Integer Not NULL,  
                                                                             [PT_AJ] Integer Not NULL, [SEG_AJ] Integer Not NULL,
                                                                             [SEG_TRK] Integer Not NULL, [CHEMIN_TRACE] Text Not NULL,
                                                                             [COULEUR_TRACE] Integer Not NULL, [EPAISSEUR_TRACE] Integer Not NULL);
                                                       INSERT INTO [TRACES] ([SENSIB], [PT_SV], [SEG_SV], [SEG_PT_SV], [PT_SP],[SEG_SP],
                                                                             [PT_AJ], [SEG_AJ], [SEG_TRK], [CHEMIN_TRACE], [COULEUR_TRACE], [EPAISSEUR_TRACE]) 
                                                                     VALUES (3, {Color.Lime.ToArgb}, {Color.Lime.ToArgb}, {Color.Black.ToArgb}, {Color.Cyan.ToArgb}, 
                                                                             {Color.FromArgb(96, Color.Cyan).ToArgb}, {Color.Green.ToArgb}, {Color.Green.ToArgb}, 
                                                                             {Color.FromArgb(255, 255, 0, 128).ToArgb}, '{_CHEMIN_TRACE}', -1722176257, 5);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> configuration concernant l'application FCGP_Regrouper </summary>
    Friend Class SettingsRegrouper
#Region "Champs"
        ''' <summary> Pas de déplacement de l'affichage lorsqu'on appuie sur uen touche écran. Valeur défaut 30 </summary>
        Friend PAS_DEPLACEMENT As Integer
        ''' <summary> Fichiers tuile générés. Valeur défaut 0 ou aucun </summary>
        Friend FICHIERS_TUILE As ChoixFichiersTuiles
        ''' <summary> Les tuiles des fichiers tuiles JNX ou ORUX font 512*512. Valeur défaut -1 ou True </summary>
        Friend DIM_TUILE_JNXORUX As Integer
        ''' <summary> Les tuiles des fichiers tuiles KMZ font 1024*1024. Valeur défaut -1 ou True </summary>
        Friend DIM_TUILE_KMZ As Integer
        ''' <summary> Choix pour la mise à jour de la largeur et hauteur de la sélection sur l'affichage </summary>
        Friend TYPE_DIMENSIONS As Integer
        ''' <summary>  Type de coordonnées du regroupement en cours. Valeur défaut 0 ou coordonnées grille du site web de capture </summary>
        Friend INDICE_TYPE_COORDONNEES As Integer
        ''' <summary> de 0 à 2 . Indique la taille de tampon pour la création des fichiers tuiles </summary>
        Friend TAILLE_TAMPON As Integer
#End Region
        '''<summary> rend la class non instanciable par erreur </summary>
        Private Sub New()

        End Sub
        Friend Sub New(FlagLire As Boolean)
            If FlagLire Then
                Lire()
            Else
                Initialiser()
            End If
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = "SELECT * FROM [REGROUPER] WHERE [_rowid_] = 1;"
            Using Reponse As SQLiteDataReader = CommandConnectionSettings.ExecuteReader
                Reponse.Read()
                PAS_DEPLACEMENT = CInt(Reponse.Item("PAS_DEPLACE"))
                FICHIERS_TUILE = CType(Reponse.Item("FICHIERS_TUILE"), ChoixFichiersTuiles)
                DIM_TUILE_JNXORUX = CInt(Reponse.Item("DIM_TUILE_JNXORUX"))
                DIM_TUILE_KMZ = CInt(Reponse.Item("DIM_TUILE_KMZ"))
                TYPE_DIMENSIONS = CInt(Reponse.Item("TYPE_DIMENSIONS"))
                INDICE_TYPE_COORDONNEES = CInt(Reponse.Item("INDICE_TYPE_COORDONNEES"))
                TAILLE_TAMPON = CInt(Reponse.Item("TAILLE_TAMPON"))
            End Using
        End Sub
        Friend Sub Ecrire()
            CommandConnectionSettings.CommandText = $"UPDATE [REGROUPER] 
                                                         SET [PAS_DEPLACE]= {PAS_DEPLACEMENT}, [FICHIERS_TUILE]= {CInt(FICHIERS_TUILE)}, 
                                                             [DIM_TUILE_JNXORUX]= {DIM_TUILE_JNXORUX}, [DIM_TUILE_KMZ]= {DIM_TUILE_KMZ},
                                                             [TYPE_DIMENSIONS]= {TYPE_DIMENSIONS}, [INDICE_TYPE_COORDONNEES]= {INDICE_TYPE_COORDONNEES},
                                                             [TAILLE_TAMPON]= {TAILLE_TAMPON}
                                                       WHERE [_rowid_] = 1;"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = "CREATE TABLE [REGROUPER] ([PAS_DEPLACE] Integer Not NULL, [FICHIERS_TUILE] Integer Not NULL, 
                                                                               [DIM_TUILE_JNXORUX] Integer Not NULL, [DIM_TUILE_KMZ] Integer Not NULL,
                                                                               [TYPE_DIMENSIONS] Integer Not NULL, [INDICE_TYPE_COORDONNEES] Integer Not NULL,
                                                                               [TAILLE_TAMPON] Integer Not NULL);
                                                      INSERT INTO [REGROUPER] ([PAS_DEPLACE], [FICHIERS_TUILE], [DIM_TUILE_JNXORUX], [DIM_TUILE_KMZ], 
                                                                               [TYPE_DIMENSIONS], [INDICE_TYPE_COORDONNEES], [TAILLE_TAMPON]) 
                                                                       VALUES (30, 0, 512, 1024, 0, 2, 0);"
            CommandConnectionSettings.ExecuteNonQuery()
            Lire()
        End Sub
    End Class
    ''' <summary> Numéro de version de la base settings </summary>
    Private Class SettingsVersion
        Private _NUM_VERSION As String
        Friend ReadOnly Property NUM_VERSION As String
            Get
                Return _NUM_VERSION
            End Get
        End Property
        Friend ReadOnly Property IsOkVersion As Boolean
        '''<summary> rend la classe non instanciable par erreur </summary>
        Private Sub New()
        End Sub
        Friend Sub New(FlagLire As Boolean, NumVersion As String)
            If FlagLire Then
                Lire()
                _IsOkVersion = _NUM_VERSION = NumVersion
            Else
                _NUM_VERSION = NumVersion
                IsOkVersion = True
                Initialiser()
            End If
        End Sub
        Private Sub Initialiser()
            CommandConnectionSettings.CommandText = $"CREATE TABLE [VERSION] ([NUM_VERSION] Text Not NULL);
                                                       INSERT INTO [VERSION] ([NUM_VERSION]) 
                                                                      VALUES ('{_NUM_VERSION}');"
            CommandConnectionSettings.ExecuteNonQuery()
        End Sub
        Private Sub Lire()
            CommandConnectionSettings.CommandText = $"SELECT [NUM_VERSION] FROM [VERSION] WHERE [_rowid_] = 1;"
            _NUM_VERSION = CStr(CommandConnectionSettings.ExecuteScalar())
        End Sub
    End Class
    ''' <summary> nom de la base de données SQLite. C'est toujours le même </summary>
    Private Const NomBaseSettings As String = "FCGP_Settings.db"
    ''' <summary> connexion avec la base SQLite</summary>
    Private _ConnectionSettings As SQLiteConnection
    Private ReadOnly Property ConnectionSettings() As SQLiteConnection
        Get
            If _ConnectionSettings Is Nothing Then
                _ConnectionSettings = New SQLiteConnection($"Data Source={NomBaseSettings};")
                CommandConnectionSettings = _ConnectionSettings.CreateCommand
            End If
            Return _ConnectionSettings
        End Get
    End Property
    ''' <summary> command a envoyer à la base de données SQLite </summary>
    Private CommandConnectionSettings As SQLiteCommand
    Friend ReadOnly Property IsSettingsOk As Boolean
    ''' <summary> création de la base des settings avec les valeurs par défault </summary>
    ''' <returns> True si cela c'est bien passé</returns>
    Friend Function InitialiserSettings(NumVersion As String) As Boolean
        Dim Titre = TitreInformation
        TitreInformation = "Information Settings"
        'si les settings sont déjà initialisés on sort
        If _IsSettingsOk Then Return True
        If File.Exists(NomBaseSettings) Then
            LireBaseSettings(NumVersion)
        Else
            InitialiserBaseSettings(NumVersion)
        End If
        TitreInformation = Titre
        Return _IsSettingsOk
    End Function
    ''' <summary> Lecture des différents settings </summary>
    ''' <param name="NumVersion"> numéro de version de la base des settings demandé par l'application </param>
    Private Sub LireBaseSettings(NumVersion As String)
        Try
            ConnectionSettings.Open()
            Version = New SettingsVersion(True, NumVersion)
            If Version.IsOkVersion Then
                FacteursSettings = New SettingsFacteurs(True)
                AltitudesSettings = New SettingsAltitudes(True)
                CapturerSettings = New SettingsCapturer(True)
                CartesSettings = New SettingsCartes(True)
                PartagerSettings = New SettingsPartager(True)
                ConvertirSettings = New SettingsConvertir(True)
                TracesSettings = New SettingsTraces(True)
                RegrouperSettings = New SettingsRegrouper(True)
                _IsSettingsOk = True
            Else
                MessageInformation = "La base des settings en cours ne correspond pas" & CrLf &
                                     $"à celle attendue par FCGP_{TypeFCGP}." & CrLf &
                                     "La base va être supprimée et recréée. " & CrLf &
                                     "Les paramètres de configuration actuels seront perdus"
                If AfficherConfirmation() = DialogResult.OK Then
                    CloturerSettings(True)
                    InitialiserBaseSettings(NumVersion)
                Else
                    _ConnectionSettings.Close()
                End If
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "T1E3")
            ErreurSettings()
        End Try
    End Sub
    ''' <summary> Création de la base des settings et mise des valeurs par defaut pour les différents settings </summary>
    ''' <param name="NumVersion">  numéro de version de la base des settings demandé par l'application  </param>
    Private Sub InitialiserBaseSettings(NumVersion As String)
        Try
            ConnectionSettings.Open()
            FacteursSettings = New SettingsFacteurs(False)
            AltitudesSettings = New SettingsAltitudes(False)
            CapturerSettings = New SettingsCapturer(False)
            CartesSettings = New SettingsCartes(False)
            PartagerSettings = New SettingsPartager(False)
            ConvertirSettings = New SettingsConvertir(False)
            TracesSettings = New SettingsTraces(False)
            RegrouperSettings = New SettingsRegrouper(False)
            Version = New SettingsVersion(False, NumVersion)
            _IsSettingsOk = True
        Catch Ex As Exception
            AfficherErreur(Ex, "M3P1")
            ErreurSettings()
        End Try
    End Sub
    ''' <summary> Libère les ressources liées à la connection et peut supprimer le fichier associé à la base des settings </summary>
    ''' <param name="FlagSupprimeBase"> flag indiquant si il faut supprimer la base des settings </param>
    Friend Sub CloturerSettings(Optional FlagSupprimeBase As Boolean = False)
        CommandConnectionSettings?.Dispose()
        If _ConnectionSettings IsNot Nothing Then
            _ConnectionSettings.Close()
            _ConnectionSettings.Dispose()
        End If
        If FlagSupprimeBase Then
            CommandConnectionSettings = Nothing
            _ConnectionSettings = Nothing
            File.Delete(NomBaseSettings)
        End If
    End Sub
    Private Sub ErreurSettings()
        CommandConnectionSettings.Dispose()
        CommandConnectionSettings = Nothing
        _ConnectionSettings.Dispose()
        _ConnectionSettings = Nothing
        MessageInformation = "La base des settings est corrompue." & CrLf &
                             "Veuillez supprimer le fichier FCGP_Settings.db"
        AfficherInformation()
    End Sub
    ''' <summary> crypte une chaine de caractères non cryptée ou décrypte une chaine de caractères cryptée</summary>
    ''' <param name="MotDePasse">Chaine à crypter ou décrypter</param>
    ''' <param name="ClefCryptage">clef de cryptage</param>
    ''' <param name="FlagCrypter">Cryptage de la chaine si true, décryptage de la chaine si false</param>
    Friend Function CrypterString(MotDePasse As String, ClefCryptage As String, FlagCrypter As Boolean) As String
        If String.IsNullOrEmpty(MotDePasse) Then Return ""
        Dim AES_256 = Aes.Create()
        Dim SHA_256 = SHA256.Create
        AES_256.Key = SHA_256.ComputeHash(Encoding_FCGP.GetBytes(ClefCryptage))
        AES_256.Mode = CipherMode.ECB

        Dim Crypter = If(FlagCrypter, AES_256.CreateEncryptor, AES_256.CreateDecryptor)
        Dim Buffer = If(FlagCrypter, Encoding_FCGP.GetBytes(MotDePasse), Convert.FromBase64String(MotDePasse))
        Dim B = Crypter.TransformFinalBlock(Buffer, 0, Buffer.Length)
        Return If(FlagCrypter, Convert.ToBase64String(B), Encoding_FCGP.GetString(B))
    End Function
End Module