using System.Security.Cryptography;

using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.SystemeCartographique;

namespace FCGP
{
    /// <summary> Ecriture et lecture des configurations associées aux différents modules du programme FCGP </summary>
    static class Settings
    {
        private static SettingsVersion Version;
        internal static SettingsAltitudes AltitudesSettings;
        internal static SettingsFacteurs FacteursSettings;
        internal static SettingsCartes CartesSettings;
        internal static SettingsPartager PartagerSettings;
        internal static SettingsTraces TracesSettings;
        internal static SettingsConvertir ConvertirSettings;
        internal static SettingsRegrouper RegrouperSettings;
        internal static SettingsCapturer CapturerSettings;

        internal class SettingsFacteurs
        {
            /// <summary> rend la classe non instanciable par erreur </summary>
            private SettingsFacteurs()
            {
            }
            internal SettingsFacteurs(bool FlagLire)
            {
                // il n'y a rien à lire alors on sort
                if (FlagLire)
                    return;
                Initialiser();
            }
            private static void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [FACTEURS] ([INDICE_SITE] Integer NOT NULL, [INDECH] Integer NOT NULL,
                                                                                   [FACTEUR_JNX] Real Not NULL, [FACTEUR_ORUX] Integer Not NULL);
                                                     CREATE INDEX [CLEFACTEURS] ON [Facteurs] ([INDICE_SITE] ASC, [INDECH] ASC);";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            /// <summary> pour FGCP_Convertir et FCGP_REGROUPER </summary>
            internal (double FacteurJNX, int FacteurORUX) Lire(int IndiceSite, int IndiceEchelle)
            {
                INDICE_SITE = IndiceSite;
                INDICE_ECHELLE = IndiceEchelle;
                LireFacteurs();
                return (FACTEUR_JNX, FACTEUR_ORUX);
            }
            /// <summary> pour FGCP_CAPTURER </summary>
            internal (double FacteurJNX, int FacteurORUX) Lire()
            {
                INDICE_SITE = CapturerSettings.INDICE_SITE;
                INDICE_ECHELLE = CapturerSettings.INDICE_ECHELLE;
                LireFacteurs();
                return (FACTEUR_JNX, FACTEUR_ORUX);
            }
            private void LireFacteurs()
            {
                FACTEUR_JNX = 0.0d;
                FACTEUR_ORUX = 0;
                CommandConnectionSettings.CommandText = $@"SELECT [FACTEUR_JNX], [FACTEUR_ORUX] FROM [FACTEURS] 
                                                            WHERE [INDICE_SITE] = {INDICE_SITE} And [INDECH] = {INDICE_ECHELLE};";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    while (Reponse.Read())
                    {
                        FACTEUR_JNX = Convert.ToDouble(Reponse["FACTEUR_JNX"]);
                        FACTEUR_ORUX = Convert.ToInt32(Reponse["FACTEUR_ORUX"]);
                    }
                }
                // si l'enregistrement correspondant n'existe pas on le crée
                if (FACTEUR_JNX == 0.0d)
                {
                    FACTEUR_JNX = FacteurJNX_Defaut(CapturerSettings.SITE, INDICE_ECHELLE);
                    FACTEUR_ORUX = FacteurORUX_Defaut(CapturerSettings.SITE, INDICE_ECHELLE);
                    Inserer();
                }
            }
            private void Inserer()
            {
                CommandConnectionSettings.CommandText = $@"INSERT INTO [FACTEURS] ([INDICE_SITE], [INDECH], [FACTEUR_JNX], [FACTEUR_ORUX])
                                                                           VALUES ({INDICE_SITE}, {INDICE_ECHELLE},
                                                                                   {DblToStr(FACTEUR_JNX, StructuresJNX.PrecisionJNX)}, {FACTEUR_ORUX});";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            /// <summary> pour FGCP_Convertir et FCGP_REGROUPER </summary>
            internal void Ecrire(int IndiceSite, int IndiceEchelle, double FacteurJNX, int FacteurOrux)
            {
                INDICE_SITE = IndiceSite;
                INDICE_ECHELLE = IndiceEchelle;
                FACTEUR_JNX = FacteurJNX;
                FACTEUR_ORUX = FacteurOrux;
                EcrireFacteurs();
            }
            /// <summary> pour FGCP_CAPTURER </summary>
            internal void Ecrire()
            {
                INDICE_SITE = CapturerSettings.INDICE_SITE;
                INDICE_ECHELLE = CapturerSettings.INDICE_ECHELLE;
                FACTEUR_JNX = CartesSettings.FACTEUR_JNX;
                FACTEUR_ORUX = CartesSettings.FACTEUR_ORUX;
                EcrireFacteurs();
            }
            private void EcrireFacteurs()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [FACTEURS] 
                                                              SET [FACTEUR_JNX]= {DblToStr(FACTEUR_JNX, StructuresJNX.PrecisionJNX)}, [FACTEUR_ORUX]= {FACTEUR_ORUX} 
                                                            WHERE [INDICE_SITE] = {INDICE_SITE} And [INDECH]= {INDICE_ECHELLE};";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private int INDICE_SITE;
            private int INDICE_ECHELLE;
            private double FACTEUR_JNX;
            private int FACTEUR_ORUX;
        }
        /// <summary> configuration concernant les fichiers DEM sous FCGP_Capturer et FCGP_Regrouper </summary>
        internal class SettingsAltitudes
        {
            #region Champs
            /// <summary> Répertoire d'enrgistrement des fichiers DEM téléchargés. Valeur défaut '' ou Document du système </summary>
            private string _CHEMIN_DEM;
            internal string CHEMIN_DEM
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_DEM) || !Directory.Exists(_CHEMIN_DEM))
                    {
                        _CHEMIN_DEM = CheminParDefaut + @"\DEM";
                        if (!Directory.Exists(_CHEMIN_DEM))
                            Directory.CreateDirectory(_CHEMIN_DEM);
                    }
                    return _CHEMIN_DEM;
                }
                set
                {
                    _CHEMIN_DEM = value;
                }
            }
            /// <summary> Type des fichiers DEM téléchargés. Valeur défaut 3 ou ou 1 point tous les 90 mètres environs </summary>
            internal int TYPE_DEM;
            /// <summary> Identifiant du compte pour le téléchargement des fichiers DEM. Valeur défaut '' </summary>
            internal string ID;
            /// <summary> Mot de passe du compte pour le téléchargement des fichiers DEM. Valeur défaut '' </summary>
            internal string MP;
            /// <summary> L'altitude sous le curseur sera affichée avec les coordoonées. Valeur défaut 0 ou false </summary>
            internal bool IS_ALTITUDE;
            /// <summary> les identifiants ont été validés. Valeur défaut 0 ou false </summary>
            internal bool IS_VALIDE;
            /// <summary> indique si les messages de téléchargement sont affichés. Valeur défaut -1 ou true </summary>
            internal bool IS_MSG;
            #endregion
            /// <summary> rend la class non instanciable par erreur </summary>
            private SettingsAltitudes()
            {

            }
            internal SettingsAltitudes(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [ALTITUDES] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    _CHEMIN_DEM = Convert.ToString(Reponse["CHEMIN_DEM"]);
                    TYPE_DEM = Convert.ToInt32(Reponse["TYPE_DEM"]);
                    ID = Convert.ToString(Reponse["ID"]);
                    MP = CrypterString(Convert.ToString(Reponse["MP"]), NomBaseSettings.Substring(5, 8), false);
                    IS_ALTITUDE = Convert.ToBoolean(Reponse["IS_ALTITUDE"]);
                    IS_VALIDE = Convert.ToBoolean(Reponse["IS_VALIDE"]);
                    IS_MSG = Convert.ToBoolean(Reponse["IS_MSG"]);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [ALTITUDES] 
                                                              SET [CHEMIN_DEM]= '{_CHEMIN_DEM}', [TYPE_DEM]= {TYPE_DEM}, [ID]= '{ID}', 
                                                                  [MP]= '{CrypterString(MP, NomBaseSettings.Substring(5, 8), true)}', 
                                                                  [IS_ALTITUDE]= {Convert.ToInt32(IS_ALTITUDE)}, [IS_VALIDE]= {Convert.ToInt32(IS_VALIDE)},
                                                                  [IS_MSG]= {Convert.ToInt32(IS_MSG)} 
                                                            WHERE [_rowid_]= 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [ALTITUDES] ([CHEMIN_DEM] Text Not NULL, [TYPE_DEM] Integer Not NULL, [ID] Text Not NULL,
                                                                                    [MP] Text Not NULL, [IS_ALTITUDE] Integer NOT NULL, [IS_VALIDE] Integer NOT NULL,
                                                                                    [IS_MSG] Integer NOT NULL);
                                                           INSERT INTO [ALTITUDES] ([CHEMIN_DEM], [TYPE_DEM], [ID], [MP], [IS_ALTITUDE], [IS_VALIDE], [IS_MSG]) 
                                                                            VALUES ('', 3, '', '', 0, 0, 0);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> configuration commune concernant les applications FCGP_Capturer, FCGP_Convertir et FCGP_Regrouper </summary>
        internal class SettingsPartager
        {
            #region Champs
            /// <summary> N° de l'écran support des applications FCGP. Valeur défaut -1 </summary>
            internal int NUM_ECRAN;
            /// <summary> Chemin d'enregistrement des fichiers tuiles pour Capture et Visue. Valeur '' </summary>
            private string _CHEMIN_TUILE;
            internal string CHEMIN_TUILE
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_TUILE) || !Directory.Exists(_CHEMIN_TUILE))
                    {
                        _CHEMIN_TUILE = CheminParDefaut + @"\TUILES";
                        if (!Directory.Exists(_CHEMIN_TUILE))
                            Directory.CreateDirectory(_CHEMIN_TUILE);
                    }
                    return _CHEMIN_TUILE;
                }
                set
                {
                    _CHEMIN_TUILE = value;
                }
            }
            /// <summary> Couleur de fond des affichages pour Capture et Visue. Valeur défaut 105, 105, 105 ou darkgrey</summary>
            internal Color COULEUR_VISUE;
            /// <summary> Couleur de fond des cartes quand il n'y a pas de données carto. Valeur défaut 255, 255, 255 ou blanc </summary>
            internal Color COULEUR_CARTE;

            internal int QUALITE_JPEG;
            #endregion
            /// <summary> rend la class non instanciable par erreur </summary>
            private SettingsPartager()
            {

            }
            internal SettingsPartager(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [PARTAGER] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    NUM_ECRAN = Convert.ToInt32(Reponse["ECRAN"]);
                    _CHEMIN_TUILE = Convert.ToString(Reponse["CHEMIN_TUILE"]);
                    COULEUR_VISUE = Color.FromArgb(Convert.ToInt32(Reponse["COUL_VISUE"]));
                    COULEUR_CARTE = Color.FromArgb(Convert.ToInt32(Reponse["COUL_CARTE"]));
                    QUALITE_JPEG = Convert.ToInt32(Reponse["QUALITE_JPEG"]);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [PARTAGER] 
                                                              SET [ECRAN]= {NUM_ECRAN}, [CHEMIN_TUILE]= '{_CHEMIN_TUILE}', 
                                                                  [COUL_VISUE]= {COULEUR_VISUE.ToArgb()}, [COUL_CARTE]= {COULEUR_CARTE.ToArgb()}, 
                                                                  [QUALITE_JPEG]={QUALITE_JPEG}
                                                            WHERE [_rowid_]= 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [PARTAGER] ([ECRAN] Integer NOT NULL, [CHEMIN_TUILE] Text Not NULL, 
                                                                                   [COUL_VISUE] Integer Not NULL, [COUL_CARTE] Integer NOT NULL,
                                                                                   [QUALITE_JPEG] Integer NOT NULL);
                                                           INSERT INTO [PARTAGER] ([ECRAN], [CHEMIN_TUILE], [COUL_VISUE], [COUL_CARTE], [QUALITE_JPEG]) 
                                                                           VALUES (-1, '', -9868951, -1, 75);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> configuration concernant l'application FCGP_Convertir </summary>
        internal class SettingsConvertir
        {
            #region Champs
            /// <summary> L'affichage de la grile est demandé. Valeur défaut 0 ou Aucun </summary>
            internal int GEOREF;
            /// <summary> Format d'enregistrement de la carte. Valeur défaut 0 ou Bmp </summary>en
            internal int FORMAT;
            /// <summary> L'affichage de la grille est demandé. Valeur défaut 1 ou true </summary>
            internal bool IS_GRILLE;
            /// <summary> Références de grilles demandées. Valeur défaut 0 ou Aucune </summary>
            internal int REFERENCE;
            /// <summary> Répertoire d'enrgistrement des cartes générées. Valeur défaut '' ou Document du système </summary>
            private string _CHEMIN_CARTE;
            internal string CHEMIN_CARTE
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_CARTE) || !Directory.Exists(_CHEMIN_CARTE))
                    {
                        _CHEMIN_CARTE = CheminParDefaut + @"\CONVERT_CARTES";
                        if (!Directory.Exists(_CHEMIN_CARTE))
                            Directory.CreateDirectory(_CHEMIN_CARTE);
                    }
                    return _CHEMIN_CARTE;
                }
                set
                {
                    _CHEMIN_CARTE = value;
                }
            }
            /// <summary> Fichiers tuile demandés. Valeur défaut 0 ou Aucun </summary>
            internal int FICHIER_TUILE;
            /// <summary> Dimensions des tuiles des fichiers tuile. Valeur défaut 512 </summary>
            internal int DIMENSIONS_TUILE;
            /// <summary> Répertoire d'enrgistrement des fichiers tuile générés. Valeur défaut '' ou Document du système </summary>
            private string _CHEMIN_TUILE;
            internal string CHEMIN_TUILE
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_TUILE) || !Directory.Exists(_CHEMIN_TUILE))
                    {
                        _CHEMIN_TUILE = CheminParDefaut + @"\CONVERT_TUILES";
                        if (!Directory.Exists(_CHEMIN_TUILE))
                            Directory.CreateDirectory(_CHEMIN_TUILE);
                    }
                    return _CHEMIN_TUILE;
                }
                set
                {
                    _CHEMIN_TUILE = value;
                }
            }
            internal Color COULEUR_TRACE;
            internal int EPAISSEUR_TRACE;
            internal int NB_PATHS;
            internal bool IS_TRACE;
            #endregion
            /// <summary> rend la class non instanciable par erreur </summary>
            private SettingsConvertir()
            {

            }
            internal SettingsConvertir(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [CONVERTIR] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    GEOREF = Convert.ToInt32(Reponse["GEOREF"]);
                    FORMAT = Convert.ToInt32(Reponse["FORMAT"]);
                    REFERENCE = Convert.ToInt32(Reponse["REFERENCE"]);
                    IS_GRILLE = Convert.ToBoolean(Reponse["IS_GRILLE"]);
                    _CHEMIN_CARTE = Convert.ToString(Reponse["CHEMIN_CARTE"]);
                    FICHIER_TUILE = Convert.ToInt32(Reponse["FICHIER_TUILE"]);
                    DIMENSIONS_TUILE = Convert.ToInt32(Reponse["DIMENSIONS_TUILE"]);
                    _CHEMIN_TUILE = Convert.ToString(Reponse["CHEMIN_TUILE"]);
                    COULEUR_TRACE = Color.FromArgb(Convert.ToInt32(Reponse["COULEUR_TRACE"]));
                    EPAISSEUR_TRACE = Convert.ToInt32(Reponse["EPAISSEUR_TRACE"]);
                    NB_PATHS = Convert.ToInt32(Reponse["NB_PATHS"]);
                    IS_TRACE = Convert.ToBoolean(Reponse["IS_TRACE"]);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [CONVERTIR] 
                                                              SET [GEOREF]= {GEOREF}, [FORMAT]= {FORMAT}, [IS_GRILLE]= {Convert.ToInt32(IS_GRILLE)}, 
                                                                  [REFERENCE]= {REFERENCE}, [CHEMIN_CARTE]= '{_CHEMIN_CARTE}', 
                                                                  [FICHIER_TUILE]= {FICHIER_TUILE}, [DIMENSIONS_TUILE]= {DIMENSIONS_TUILE}, 
                                                                  [CHEMIN_TUILE]= '{_CHEMIN_TUILE}', [COULEUR_TRACE]= {COULEUR_TRACE.ToArgb()}, 
                                                                  [EPAISSEUR_TRACE]= {EPAISSEUR_TRACE}, [NB_PATHS]= {NB_PATHS}, [IS_TRACE]= {Convert.ToInt32(IS_TRACE)} 
                                                            WHERE [_rowid_]= 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [CONVERTIR] ([GEOREF] Integer Not NULL, [FORMAT] Integer Not NULL,
                                                                                    [IS_GRILLE] Integer Not NULL, [REFERENCE] Integer Not NULL, 
                                                                                    [CHEMIN_CARTE] Text Not NULL, [FICHIER_TUILE] Integer Not NULL,  
                                                                                    [DIMENSIONS_TUILE] Integer Not NULL,[CHEMIN_TUILE] Text Not NULL,
                                                                                    [COULEUR_TRACE] Integer Not NULL, [EPAISSEUR_TRACE] Integer Not NULL, 
                                                                                    [NB_PATHS] Integer Not NULL, [IS_TRACE] Integer Not NULL);
                                                           INSERT INTO [CONVERTIR] ([GEOREF], [FORMAT], [IS_GRILLE], [REFERENCE], [CHEMIN_CARTE],
                                                                                    [FICHIER_TUILE], [DIMENSIONS_TUILE], [CHEMIN_TUILE],
                                                                                    [COULEUR_TRACE], [EPAISSEUR_TRACE], [NB_PATHS], [IS_TRACE]) 
                                                                            VALUES (0, 0, 1, 0, '', 0, 512, '', -1722176257, 5, 0, 0);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> configuration concernant l'application FCGP_Capturer </summary>
        internal class SettingsCapturer
        {
            #region Champs
            /// <summary> Coordonnées du centre de l'affichage du dernier site séléctionné en tuile </summary>
            internal PointG CENTRE_AFFICHAGE_SITE
            {
                get
                {
                    return new PointG(INDICE_ECHELLE, new Point(X, Y));
                }
            }

            private int _INDICE_SITE;
            /// <summary>  Dernier site affiché avant de sortir de l'application. Indice unique SiteDomTom</summary>
            internal int INDICE_SITE
            {
                get
                {
                    return _INDICE_SITE;
                }
                set
                {
                    _INDICE_SITE = value;
                    (SITE, DOMTOM) = IndexToSiteDomTom(INDICE_SITE);
                }
            }
            /// <summary>  Dernier site affiché avant de sortir de l'application.</summary>
            internal SitesCartographiques SITE { get; private set; }
            /// <summary>  Dernier domtom affiché avant de sortir de l'application. Valeur défaut 0 ou réunion </summary>
            internal DomsToms DOMTOM { get; private set; }
            /// <summary>  Chemin d'enregistrement des cartes faites à partir de FCGP_Capture. Valeur défaut '' ou Document du système </summary>
            private string _CHEMIN_CARTE;
            internal string CHEMIN_CARTE
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_CARTE) || !Directory.Exists(_CHEMIN_CARTE))
                    {
                        _CHEMIN_CARTE = CheminParDefaut + @"\CARTES";
                        if (!Directory.Exists(_CHEMIN_CARTE))
                            Directory.CreateDirectory(_CHEMIN_CARTE);
                    }
                    return _CHEMIN_CARTE;
                }
                set
                {
                    _CHEMIN_CARTE = value;
                }
            }
            /// <summary>  Chemin d'enregistrement des caches utilisés par FCGP_Capture. Valeur défaut '' ou Document du système </summary>
            private string _CHEMIN_CACHE;
            internal string CHEMIN_CACHE
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_CACHE) || !Directory.Exists(_CHEMIN_CACHE))
                    {
                        _CHEMIN_CACHE = CheminParDefaut;
                        if (!Directory.Exists(_CHEMIN_CACHE))
                            Directory.CreateDirectory(_CHEMIN_CACHE);
                    }
                    return _CHEMIN_CACHE;
                }
                set
                {
                    _CHEMIN_CACHE = value;
                }
            }
            /// <summary>  Type de coordonnées du site sélectionné. Valeur défaut 0 ou coordonnées grille du site web </summary>
            internal int INDICE_TYPE_COORDONNEES;
            /// <summary>  Indice Echelle du site selectionné. Valeur défaut 0 correspond à celle du centre du site web </summary>
            internal int INDICE_ECHELLE;
            /// <summary>  Indice du CoefAlpha de la couche des Pentes du site selectionné. Valeur défaut 1 ou 33% </summary>
            internal int INDICE_COEF_ALPHA_PENTES;
            /// <summary>  Indice du fond de plan sur l'affichage . Valeur défaut 0 ou Cartes IGN </summary>
            internal int INDICE_FOND_PLAN;
            /// <summary> Indice du dernier site OSM visité par le site de capture . Valeur défaut 0 ou ClycleOSM </summary>
            internal int INDICE_OSM;
            /// <summary>  Coordonnées virtuelles du centre de l'affichage du site séléctionné. Valeur défaut point empty </summary>
            internal Point LOCATION_CENTRE
            {
                get
                {
                    return new Point(X, Y);
                }
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }
            /// <summary> Coordonnées X du centre de l'affichage du site séléctionné. Valeur défaut 0 </summary>
            private int X;
            /// <summary> Coordonnées Y du centre de l'affichage du site séléctionné. Valeur défaut 0 </summary>
            private int Y;
            /// <summary> Coordonnées du point1 de séléction du site séléctionné. Valeur défaut 0,0 ou point empty </summary>
            internal PointD POINT1
            {
                get
                {
                    if (IS_NOTOK0)
                    {
                        return new PointD();
                    }
                    else
                    {
                        return new PointD(X0, Y0);
                    }
                }
                set
                {
                    X0 = value.X;
                    Y0 = value.Y;
                    IS_NOTOK0 = value.IsEmpty;
                }
            }
            /// <summary>  le point1 de sélection est OK. Valeur défaut -1  ou not ok </summary>
            private bool IS_NOTOK0;
            /// <summary> Coordonnées X du point 1 de séléction du site séléctionné. Valeur défaut 0 </summary>
            private double X0;
            /// <summary> Coordonnées Y du point 1 de séléction du site séléctionné. Valeur défaut 0 </summary>
            private double Y0;
            /// <summary> Coordonnées du point2 de séléction du site séléctionné. Valeur défaut 0,0 ou point empty </summary>
            internal PointD POINT2
            {
                get
                {
                    if (IS_NOTOK2)
                    {
                        return new PointD();
                    }
                    else
                    {
                        return new PointD(X2, Y2);
                    }
                }
                set
                {
                    X2 = value.X;
                    Y2 = value.Y;
                    IS_NOTOK2 = value.IsEmpty;
                }
            }
            /// <summary> le point2 de sélection est OK. Valeur défaut -1  ou not ok </summary>
            private bool IS_NOTOK2;
            /// <summary> Coordonnées X du point 2 de séléction du site séléctionné. Valeur défaut 0 </summary>
            private double X2;
            /// <summary> Coordonnées Y du point 2 de séléction du site séléctionné. Valeur défaut 0 </summary>
            private double Y2;
            /// <summary> Nom de la trace encours pour le site ou domtom </summary>
            internal string NOM_TRACE;
            /// <summary> flag indiquant si une échelle est affichée dans le coin bas droit de la visue </summary>
            internal bool IS_AFFICHE_ECHELLE;
            /// <summary> flag indiquant si la couche des pentes est affichée sur le fond de plan </summary>
            internal bool IS_AFFICHE_PENTES;
            #endregion
            /// <summary> rend la classe non instanciable par erreur </summary>
            private SettingsCapturer()
            {

            }
            internal SettingsCapturer(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [CAPTURER] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    _INDICE_SITE = Convert.ToInt32(Reponse["INDICE_SITE"]);
                    _CHEMIN_CARTE = Convert.ToString(Reponse["CHEMIN_CARTE"]);
                    _CHEMIN_CACHE = Convert.ToString(Reponse["CHEMIN_CACHE"]);
                    IS_AFFICHE_ECHELLE = Convert.ToBoolean(Reponse["IS_AFFICHE_ECHELLE"]);
                    IS_AFFICHE_PENTES = Convert.ToBoolean(Reponse["IS_AFFICHE_PENTES"]);
                    INDICE_COEF_ALPHA_PENTES = Convert.ToInt32(Reponse["IND_COEF_ALPHA_PENTES"]);
                    (SITE, DOMTOM) = IndexToSiteDomTom(_INDICE_SITE);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [CAPTURER] 
                                                              SET [INDICE_SITE]= {_INDICE_SITE}, [CHEMIN_CARTE]= '{_CHEMIN_CARTE}',  
                                                                  [CHEMIN_CACHE]= '{_CHEMIN_CACHE}', [IS_AFFICHE_ECHELLE]= {Convert.ToInt32(IS_AFFICHE_ECHELLE)}, 
                                                                  [IS_AFFICHE_PENTES]= {Convert.ToInt32(IS_AFFICHE_PENTES)}, 
                                                                  [IND_COEF_ALPHA_PENTES]= {INDICE_COEF_ALPHA_PENTES}
                                                            WHERE [_rowid_]= 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            internal void LirePosition()
            {
                bool PositionsExiste;
                CommandConnectionSettings.CommandText = $"SELECT * FROM [POSITIONS] WHERE [INDICE_SITE] = {_INDICE_SITE};";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    PositionsExiste = Reponse.HasRows;
                    while (Reponse.Read())
                    {
                        INDICE_ECHELLE = Convert.ToInt32(Reponse["INDECH"]);
                        INDICE_TYPE_COORDONNEES = Convert.ToInt32(Reponse["COORD_TYPE"]);
                        X = Convert.ToInt32(Reponse["X"]);
                        Y = Convert.ToInt32(Reponse["Y"]);
                        IS_NOTOK0 = Convert.ToBoolean(Reponse["IS_NOTOK0"]);
                        X0 = Convert.ToDouble(Reponse["X0"]);
                        Y0 = Convert.ToDouble(Reponse["Y0"]);
                        IS_NOTOK2 = Convert.ToBoolean(Reponse["IS_NOTOK2"]);
                        X2 = Convert.ToDouble(Reponse["X2"]);
                        Y2 = Convert.ToDouble(Reponse["Y2"]);
                        NOM_TRACE = Convert.ToString(Reponse["NOM_TRACE"]);
                        INDICE_FOND_PLAN = Convert.ToInt32(Reponse["IND_FOND_PLAN"]);
                        INDICE_OSM = Convert.ToInt32(Reponse["IND_OSM"]);
                    }
                }
                if (!PositionsExiste)
                {
                    // on insert l'enregistrement correspondant avec des valeurs par défaut car il n'existe pas encore
                    var Coord = CentreSiteWeb(_INDICE_SITE);
                    INDICE_ECHELLE = Coord.IndiceEchelle;
                    INDICE_TYPE_COORDONNEES = SITE == SitesCartographiques.SuisseMobile ? 0 : 2;
                    X = Coord.X;
                    Y = Coord.Y;
                    IS_NOTOK0 = true;
                    X0 = 0d;
                    Y0 = 0d;
                    IS_NOTOK2 = true;
                    X2 = 0d;
                    Y2 = 0d;
                    NOM_TRACE = "";
                    INDICE_FOND_PLAN = 0;
                    INDICE_OSM = 0;
                    InsererPosition();
                }
            }
            private void InsererPosition()
            {
                CommandConnectionSettings.CommandText = $@"INSERT INTO [POSITIONS] ([INDICE_SITE], [INDECH], [COORD_TYPE], [X], [Y], [IS_NOTOK0],  
                                                                                    [X0], [Y0], [IS_NOTOK2], [X2], [Y2], [NOM_TRACE], [IND_FOND_PLAN], [IND_OSM])
                                                                            VALUES ({_INDICE_SITE}, {INDICE_ECHELLE}, {INDICE_TYPE_COORDONNEES}, {X}, {Y},
                                                                                    {Convert.ToInt32(IS_NOTOK0)}, {DblToStr(X0, "N4")}, {DblToStr(Y0, "N4")}, {Convert.ToInt32(IS_NOTOK2)}, 
                                                                                    {DblToStr(X2, "N4")}, {DblToStr(Y2, "N4")}, '{NOM_TRACE}', {INDICE_FOND_PLAN}, {INDICE_OSM});";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            internal void EcrirePosition()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [POSITIONS] 
                                                              SET [INDECH]= {INDICE_ECHELLE}, [COORD_TYPE]= {INDICE_TYPE_COORDONNEES}, [X]= {X}, [Y]= {Y}, 
                                                                  [IS_NOTOK0] = {Convert.ToInt32(IS_NOTOK0)}, [X0] = {DblToStr(X0, "0.0000")}, [Y0] = {DblToStr(Y0, "0.0000")},
                                                                  [IS_NOTOK2] = {Convert.ToInt32(IS_NOTOK2)}, [X2] = {DblToStr(X2, "0.0000")}, [Y2] = {DblToStr(Y2, "0.0000")}, 
                                                                  [NOM_TRACE] = '{NOM_TRACE}', [IND_FOND_PLAN] = {INDICE_FOND_PLAN}, [IND_OSM] = {INDICE_OSM} 
                                                            WHERE [INDICE_SITE]= {_INDICE_SITE};";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [CAPTURER] ([INDICE_SITE] Integer Not NULL, [CHEMIN_CARTE] Text Not NULL, 
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
                                       CREATE INDEX [CLEPOSITIONS] ON [POSITIONS] ([INDICE_SITE] ASC);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> configuration concernant la génération des cartes sous FCGP_Capturer, FCGP_Convertir et FCGP_Regrouper </summary>
        internal class SettingsCartes
        {
            #region Champs
            /// <summary> Les références de la grille sont demandées. Valeur défaut 0 ou false </summary>
            internal bool IS_REFERENCES;
            /// <summary> fichiers tuile demandé. Valeur défaut 0 ou Aucun </summary>
            internal int FICHIER_TUILE;
            /// <summary> Fichier de réféencement demandé. Valeur défaut 0 ou .Georef </summary>
            internal int GEOREF;
            /// <summary> Le dessin de la trace est demandé. Valeur défaut 0 ou false </summary>
            internal bool IS_TRACE;
            /// <summary> L'affichage de la grille est demandé. Valeur défaut -1 ou true </summary>
            private bool _IS_GRILLE;
            internal bool IS_GRILLE
            {
                get
                {
                    if (GrilleSiteExiste(CapturerSettings.SITE))
                    {
                        return true;
                    }
                    else
                    {
                        return _IS_GRILLE;
                    }
                }
                set
                {
                    if (!GrilleSiteExiste(CapturerSettings.SITE))
                    {
                        _IS_GRILLE = value;
                    }
                }
            }
            /// <summary> Dimensions des tuiles des fichiers tuile. Valeur défaut 512 </summary>
            internal int DIMENSIONS_TUILE;
            /// <summary> Format d'enregistrement de la carte. Valeur défaut 0 ou Bmp </summary>
            internal int FORMAT;
            /// <summary> Support physique du tampon de la carte. Valeur défaut 0 ou Mémoire </summary>
            private int SUPPORT_TAMPON;
            internal TypeSupportCarte SUPPORT_CARTE
            {
                get
                {
                    return SUPPORT_TAMPON == 0 ? (FlagSmallMemory ? TypeSupportCarte.MemoryStatic : TypeSupportCarte.MemoryDynamic) : TypeSupportCarte.Fichier;
                }
                set
                {
                    SUPPORT_TAMPON = value == TypeSupportCarte.Fichier ? 1 : 0;
                }
            }
            /// <summary> On accepte ous les formats d'eregistrement de la carte en cas de suport tampon Fichier. Valeur défaut 0 ou false </summary>
            internal bool IS_TOUT_FORMAT;
#pragma warning disable CS0649
            /// <summary> Facteur JNX. Valeur obtenue lors de l'ouverture du form Preference. n'est pas enregistrée </summary>
            internal double FACTEUR_JNX;
            /// <summary> Facteur ORUX. Valeur défaut du site web qui dépend du site et de l'échelle de capture </summary>
            internal int FACTEUR_ORUX;
#pragma warning restore CS0649
            #endregion
            /// <summary> rend la class non instanciable par erreur </summary>
            private SettingsCartes()
            {

            }
            internal SettingsCartes(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [CARTES] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    IS_REFERENCES = Convert.ToBoolean(Reponse["IS_REFERENCES"]);
                    FICHIER_TUILE = Convert.ToInt32(Reponse["FICHIER_TUILE"]);
                    GEOREF = Convert.ToInt32(Reponse["GEOREF"]);
                    _IS_GRILLE = Convert.ToBoolean(Reponse["IS_GRILLE"]);
                    DIMENSIONS_TUILE = Convert.ToInt32(Reponse["DIMENSIONS_TUILE"]);
                    FORMAT = Convert.ToInt32(Reponse["FORMAT"]);
                    SUPPORT_TAMPON = Convert.ToInt32(Reponse["SUPPORT_TAMPON"]);
                    IS_TOUT_FORMAT = Convert.ToBoolean(Reponse["IS_TOUT_FORMAT"]);
                    IS_TRACE = Convert.ToBoolean(Reponse["IS_TRACE"]);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [CARTES] 
                                                              SET [IS_REFERENCES]= {Convert.ToInt32(IS_REFERENCES)}, [FICHIER_TUILE]= {FICHIER_TUILE}, [GEOREF]= {GEOREF}, 
                                                                  [IS_GRILLE]= {Convert.ToInt32(_IS_GRILLE)}, [DIMENSIONS_TUILE]= {DIMENSIONS_TUILE}, 
                                                                  [FORMAT]= {FORMAT}, [SUPPORT_TAMPON]= {SUPPORT_TAMPON}, 
                                                                  [IS_TOUT_FORMAT]= {Convert.ToInt32(IS_TOUT_FORMAT)}, [IS_TRACE]= {Convert.ToInt32(IS_TRACE)}
                                                            WHERE [_rowid_]= 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [CARTES] ([IS_REFERENCES] Integer Not NULL, [FICHIER_TUILE] Integer Not NULL,
                                                                                 [GEOREF] Integer Not NULL, [IS_GRILLE] Integer Not NULL, [DIMENSIONS_TUILE] Integer Not NULL,
                                                                                 [FORMAT] Integer Not NULL, [SUPPORT_TAMPON] Integer Not NULL, 
                                                                                 [IS_TOUT_FORMAT] Integer Not NULL, [IS_TRACE] Integer Not NULL);
                                                           INSERT INTO [CARTES] ([IS_REFERENCES], [FICHIER_TUILE], [GEOREF], [IS_GRILLE], [DIMENSIONS_TUILE], 
                                                                                 [FORMAT], [SUPPORT_TAMPON], [IS_TOUT_FORMAT], [IS_TRACE]) 
                                                                         VALUES (0, 0, 1, -1, 512, 0, 0, 0, 0);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> configuration concernant les traces sous FCGP_Capturer </summary>
        internal class SettingsTraces
        {
            #region Champs
            /// <summary> Répertoire d'enrgistrement des fichiers DEM téléchargés. Valeur défaut '' ou Document du système </summary>
            private string _CHEMIN_TRACE;
            internal string CHEMIN_TRACE
            {
                get
                {
                    if (string.IsNullOrEmpty(_CHEMIN_TRACE) || !Directory.Exists(_CHEMIN_TRACE))
                    {
                        _CHEMIN_TRACE = CheminParDefaut + @"\TRK";
                        if (!Directory.Exists(_CHEMIN_TRACE))
                            Directory.CreateDirectory(_CHEMIN_TRACE);
                    }
                    return _CHEMIN_TRACE;
                }
                set
                {
                    _CHEMIN_TRACE = value;
                }
            }
            internal int SENSIB;
            internal Color PT_SV;
            internal Color SEG_SV;
            internal Color SEG_PT_SV;
            internal Color PT_SP;
            internal Color SEG_SP;
            internal Color PT_AJ;
            internal Color SEG_AJ;
            internal Color SEG_TRK;
            internal Color COULEUR_TRACE;
            internal int EPAISSEUR_TRACE;
            #endregion
            /// <summary> rend la class non instanciable par erreur </summary>
            private SettingsTraces()
            {

            }
            internal SettingsTraces(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [TRACES] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    SENSIB = Convert.ToInt32(Reponse["SENSIB"]);
                    PT_SV = Color.FromArgb(Convert.ToInt32(Reponse["PT_SV"]));
                    SEG_SV = Color.FromArgb(Convert.ToInt32(Reponse["SEG_SV"]));
                    SEG_PT_SV = Color.FromArgb(Convert.ToInt32(Reponse["SEG_PT_SV"]));
                    PT_SP = Color.FromArgb(Convert.ToInt32(Reponse["PT_SP"]));
                    SEG_SP = Color.FromArgb(Convert.ToInt32(Reponse["SEG_SP"]));
                    PT_AJ = Color.FromArgb(Convert.ToInt32(Reponse["PT_AJ"]));
                    SEG_AJ = Color.FromArgb(Convert.ToInt32(Reponse["SEG_AJ"]));
                    SEG_TRK = Color.FromArgb(Convert.ToInt32(Reponse["SEG_TRK"]));
                    _CHEMIN_TRACE = Convert.ToString(Reponse["CHEMIN_TRACE"]);
                    COULEUR_TRACE = Color.FromArgb(Convert.ToInt32(Reponse["COULEUR_TRACE"]));
                    EPAISSEUR_TRACE = Convert.ToInt32(Reponse["EPAISSEUR_TRACE"]);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [TRACES] 
                                                              SET [SENSIB]= {SENSIB}, [PT_SV]= {PT_SV.ToArgb()}, 
                                                                  [SEG_SV]= {SEG_SV.ToArgb()}, [SEG_PT_SV]= {SEG_PT_SV.ToArgb()},  
                                                                  [PT_SP]= {PT_SP.ToArgb()}, [SEG_SP]= {SEG_SP.ToArgb()},
                                                                  [PT_AJ]= {PT_AJ.ToArgb()}, [SEG_AJ]= {SEG_AJ.ToArgb()}, 
                                                                  [SEG_TRK]= {SEG_TRK.ToArgb()}, [CHEMIN_TRACE]= '{_CHEMIN_TRACE}',
                                                                  [COULEUR_TRACE]= {COULEUR_TRACE.ToArgb()}, [EPAISSEUR_TRACE]= {EPAISSEUR_TRACE}
                                                            WHERE [_rowid_]= 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = $@"CREATE TABLE [TRACES] ([SENSIB] Integer Not NULL, [PT_SV] Integer Not NULL,
                                                                                  [SEG_SV] Integer Not NULL, [SEG_PT_SV] Integer Not NULL, 
                                                                                  [PT_SP] Integer Not NULL, [SEG_SP] Integer Not NULL,  
                                                                                  [PT_AJ] Integer Not NULL, [SEG_AJ] Integer Not NULL,
                                                                                  [SEG_TRK] Integer Not NULL, [CHEMIN_TRACE] Text Not NULL,
                                                                                  [COULEUR_TRACE] Integer Not NULL, [EPAISSEUR_TRACE] Integer Not NULL);
                                                            INSERT INTO [TRACES] ([SENSIB], [PT_SV], [SEG_SV], [SEG_PT_SV], [PT_SP],[SEG_SP],
                                                                                  [PT_AJ], [SEG_AJ], [SEG_TRK], [CHEMIN_TRACE], [COULEUR_TRACE], [EPAISSEUR_TRACE]) 
                                                                          VALUES (3, {Color.Lime.ToArgb()}, {Color.Lime.ToArgb()}, {Color.Black.ToArgb()}, {Color.Cyan.ToArgb()}, 
                                                                                  {Color.FromArgb(96, Color.Cyan).ToArgb()}, {Color.Green.ToArgb()}, {Color.Green.ToArgb()}, 
                                                                                  {Color.FromArgb(255, 255, 0, 128).ToArgb()}, '{_CHEMIN_TRACE}', -1722176257, 5);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> configuration concernant l'application FCGP_Regrouper </summary>
        internal class SettingsRegrouper
        {
            #region Champs
            /// <summary> Pas de déplacement de l'affichage lorsqu'on appuie sur uen touche écran. Valeur défaut 30 </summary>
            internal int PAS_DEPLACEMENT;
            /// <summary> Fichiers tuile générés. Valeur défaut 0 ou aucun </summary>
            internal ChoixFichiersTuiles FICHIERS_TUILE;
            /// <summary> Les tuiles des fichiers tuiles JNX ou ORUX font 512*512. Valeur défaut -1 ou True </summary>
            internal int DIM_TUILE_JNXORUX;
            /// <summary> Les tuiles des fichiers tuiles KMZ font 1024*1024. Valeur défaut -1 ou True </summary>
            internal int DIM_TUILE_KMZ;
            /// <summary> Choix pour la mise à jour de la largeur et hauteur de la sélection sur l'affichage </summary>
            internal int TYPE_DIMENSIONS;
            /// <summary>  Type de coordonnées du regroupement en cours. Valeur défaut 0 ou coordonnées grille du site web de capture </summary>
            internal int INDICE_TYPE_COORDONNEES;
            /// <summary> de 0 à 2 . Indique la taille de tampon pour la création des fichiers tuiles </summary>
            internal int TAILLE_TAMPON;
            #endregion
            /// <summary> rend la class non instanciable par erreur </summary>
            private SettingsRegrouper()
            {

            }
            internal SettingsRegrouper(bool FlagLire)
            {
                if (FlagLire)
                {
                    Lire();
                }
                else
                {
                    Initialiser();
                }
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = "SELECT * FROM [REGROUPER] WHERE [_rowid_] = 1;";
                using (var Reponse = CommandConnectionSettings.ExecuteReader())
                {
                    Reponse.Read();
                    PAS_DEPLACEMENT = Convert.ToInt32(Reponse["PAS_DEPLACE"]);
                    FICHIERS_TUILE = (ChoixFichiersTuiles)Convert.ToInt32(Reponse["FICHIERS_TUILE"]);
                    DIM_TUILE_JNXORUX = Convert.ToInt32(Reponse["DIM_TUILE_JNXORUX"]);
                    DIM_TUILE_KMZ = Convert.ToInt32(Reponse["DIM_TUILE_KMZ"]);
                    TYPE_DIMENSIONS = Convert.ToInt32(Reponse["TYPE_DIMENSIONS"]);
                    INDICE_TYPE_COORDONNEES = Convert.ToInt32(Reponse["INDICE_TYPE_COORDONNEES"]);
                    TAILLE_TAMPON = Convert.ToInt32(Reponse["TAILLE_TAMPON"]);
                }
            }
            internal void Ecrire()
            {
                CommandConnectionSettings.CommandText = $@"UPDATE [REGROUPER] 
                                                              SET [PAS_DEPLACE]= {PAS_DEPLACEMENT}, [FICHIERS_TUILE]= {(int)FICHIERS_TUILE}, 
                                                                  [DIM_TUILE_JNXORUX]= {DIM_TUILE_JNXORUX}, [DIM_TUILE_KMZ]= {DIM_TUILE_KMZ},
                                                                  [TYPE_DIMENSIONS]= {TYPE_DIMENSIONS}, [INDICE_TYPE_COORDONNEES]= {INDICE_TYPE_COORDONNEES},
                                                                  [TAILLE_TAMPON]= {TAILLE_TAMPON}
                                                            WHERE [_rowid_] = 1;";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = @"CREATE TABLE [REGROUPER] ([PAS_DEPLACE] Integer Not NULL, [FICHIERS_TUILE] Integer Not NULL, 
                                                                                    [DIM_TUILE_JNXORUX] Integer Not NULL, [DIM_TUILE_KMZ] Integer Not NULL,
                                                                                    [TYPE_DIMENSIONS] Integer Not NULL, [INDICE_TYPE_COORDONNEES] Integer Not NULL,
                                                                                    [TAILLE_TAMPON] Integer Not NULL);
                                                           INSERT INTO [REGROUPER] ([PAS_DEPLACE], [FICHIERS_TUILE], [DIM_TUILE_JNXORUX], [DIM_TUILE_KMZ], 
                                                                                    [TYPE_DIMENSIONS], [INDICE_TYPE_COORDONNEES], [TAILLE_TAMPON]) 
                                                                            VALUES (30, 0, 512, 1024, 0, 2, 0);";
                CommandConnectionSettings.ExecuteNonQuery();
                Lire();
            }
        }
        /// <summary> Numéro de version de la base settings </summary>
        private class SettingsVersion
        {
            private string _NUM_VERSION;
            internal string NUM_VERSION
            {
                get
                {
                    return _NUM_VERSION;
                }
            }
            internal bool IsOkVersion { get; private set; }
            /// <summary> rend la classe non instanciable par erreur </summary>
            private SettingsVersion()
            {
            }
            internal SettingsVersion(bool FlagLire, string NumVersion)
            {
                if (FlagLire)
                {
                    Lire();
                    IsOkVersion = _NUM_VERSION == NumVersion;
                }
                else
                {
                    _NUM_VERSION = NumVersion;
                    IsOkVersion = true;
                    Initialiser();
                }
            }
            private void Initialiser()
            {
                CommandConnectionSettings.CommandText = $@"CREATE TABLE [VERSION] ([NUM_VERSION] Text Not NULL);
                                                            INSERT INTO [VERSION] ([NUM_VERSION]) 
                                                                           VALUES ('{_NUM_VERSION}');";
                CommandConnectionSettings.ExecuteNonQuery();
            }
            private void Lire()
            {
                CommandConnectionSettings.CommandText = $"SELECT [NUM_VERSION] FROM [VERSION] WHERE [_rowid_] = 1;";
                _NUM_VERSION = Convert.ToString(CommandConnectionSettings.ExecuteScalar());
            }
        }
        /// <summary> nom de la base de données SQLite. C'est toujours le même </summary>
        private const string NomBaseSettings = "FCGP_Settings.db";
        /// <summary> connexion avec la base SQLite</summary>
        private static SQLiteConnection _ConnectionSettings;
        private static SQLiteConnection ConnectionSettings
        {
            get
            {
                if (_ConnectionSettings is null)
                {
                    _ConnectionSettings = new SQLiteConnection($"Data Source={NomBaseSettings};");
                    CommandConnectionSettings = _ConnectionSettings.CreateCommand();
                }
                return _ConnectionSettings;
            }
        }
        /// <summary> command a envoyer à la base de données SQLite </summary>
        private static SQLiteCommand CommandConnectionSettings;
        internal static bool IsSettingsOk { get; private set; }
        /// <summary> création de la base des settings avec les valeurs par défault </summary>
        /// <returns> True si cela c'est bien passé</returns>
        internal static bool InitialiserSettings(string NumVersion)
        {
            var Titre = TitreInformation;
            TitreInformation = "Information Settings";
            // si les settings sont déjà initialisés on sort
            if (IsSettingsOk)
                return true;
            if (File.Exists(NomBaseSettings))
            {
                LireBaseSettings(NumVersion);
            }
            else
            {
                InitialiserBaseSettings(NumVersion);
            }
            TitreInformation = Titre;
            return IsSettingsOk;
        }
        /// <summary> Lecture des différents settings </summary>
        /// <param name="NumVersion"> numéro de version de la base des settings demandé par l'application </param>
        private static void LireBaseSettings(string NumVersion)
        {
            try
            {
                ConnectionSettings.Open();
                Version = new SettingsVersion(true, NumVersion);
                if (Version.IsOkVersion)
                {
                    FacteursSettings = new SettingsFacteurs(true);
                    AltitudesSettings = new SettingsAltitudes(true);
                    CapturerSettings = new SettingsCapturer(true);
                    CartesSettings = new SettingsCartes(true);
                    PartagerSettings = new SettingsPartager(true);
                    ConvertirSettings = new SettingsConvertir(true);
                    TracesSettings = new SettingsTraces(true);
                    RegrouperSettings = new SettingsRegrouper(true);
                    IsSettingsOk = true;
                }
                else
                {
                    MessageInformation = "La base des settings en cours ne correspond pas" + CrLf +
                                         $"à celle attendue par FCGP_{TypeFCGP}." + CrLf +
                                         "La base va être supprimée et recréée. " + CrLf +
                                         "Les paramètres de configuration actuels seront perdus";
                    TitreInformation = "Information Settings";
                    if (AfficherConfirmation() == DialogResult.OK)
                    {
                        CloturerSettings(true);
                        InitialiserBaseSettings(NumVersion);
                    }
                    else
                    {
                        _ConnectionSettings.Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T1E3");
                ErreurSettings();
            }
        }
        /// <summary> Création de la base des settings et mise des valeurs par defaut pour les différents settings </summary>
        /// <param name="NumVersion">  numéro de version de la base des settings demandé par l'application  </param>
        private static void InitialiserBaseSettings(string NumVersion)
        {
            try
            {
                ConnectionSettings.Open();
                FacteursSettings = new SettingsFacteurs(false);
                AltitudesSettings = new SettingsAltitudes(false);
                CapturerSettings = new SettingsCapturer(false);
                CartesSettings = new SettingsCartes(false);
                PartagerSettings = new SettingsPartager(false);
                ConvertirSettings = new SettingsConvertir(false);
                TracesSettings = new SettingsTraces(false);
                RegrouperSettings = new SettingsRegrouper(false);
                Version = new SettingsVersion(false, NumVersion);
                IsSettingsOk = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "M3P1");
                ErreurSettings();
            }
        }
        /// <summary> Libère les ressources liées à la connection et peut supprimer le fichier associé à la base des settings </summary>
        /// <param name="FlagSupprimeBase"> flag indiquant si il faut supprimer la base des settings </param>
        internal static void CloturerSettings(bool FlagSupprimeBase = false)
        {
            CommandConnectionSettings?.Dispose();
            if (_ConnectionSettings is not null)
            {
                _ConnectionSettings.Close();
                _ConnectionSettings.Dispose();
            }
            if (FlagSupprimeBase)
            {
                CommandConnectionSettings = null;
                _ConnectionSettings = null;
                File.Delete(NomBaseSettings);
            }
        }
        private static void ErreurSettings()
        {
            CommandConnectionSettings.Dispose();
            CommandConnectionSettings = null;
            _ConnectionSettings.Dispose();
            _ConnectionSettings = null;
            MessageInformation = "La base des settings est corrompue." + CrLf +
                                 "Veuillez supprimer le fichier FCGP_Settings.db";
            AfficherInformation();
        }
        /// <summary> crypte une chaine de caractères non cryptée ou décrypte une chaine de caractères cryptée</summary>
        /// <param name="MotDePasse">Chaine à crypter ou décrypter</param>
        /// <param name="ClefCryptage">clef de cryptage</param>
        /// <param name="FlagCrypter">Cryptage de la chaine si true, décryptage de la chaine si false</param>
        internal static string CrypterString(string MotDePasse, string ClefCryptage, bool FlagCrypter)
        {
            if (string.IsNullOrEmpty(MotDePasse))
                return "";
            var AES_256 = Aes.Create();
            var SHA_256 = SHA256.Create();
            AES_256.Key = SHA_256.ComputeHash(Encoding_FCGP.GetBytes(ClefCryptage));
            AES_256.Mode = CipherMode.ECB;

            var Crypter = FlagCrypter ? AES_256.CreateEncryptor() : AES_256.CreateDecryptor();
            var Buffer = FlagCrypter ? Encoding_FCGP.GetBytes(MotDePasse) : Convert.FromBase64String(MotDePasse);
            var B = Crypter.TransformFinalBlock(Buffer, 0, Buffer.Length);
            return FlagCrypter ? Convert.ToBase64String(B) : Encoding_FCGP.GetString(B);
        }
    }
}