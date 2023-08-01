using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.SystemeCartographique;

namespace FCGP
{
    /// <summary> contient toutes les structures nécessaires pour enregistrer ou lire un fichier JNX</summary>
    internal static class StructuresJNX
    {
        #region Partie public
        /// <summary> Format associé au facteur d'affichage JNX</summary>
        internal const string PrecisionJNX = "0.00";
        /// <summary> Lit les tuiles d'un niveau du fichier ORUX et les écrit dans un répertoire </summary>
        /// <param name="FichierCarteJnx"> chemin du fichier jnx de la carte </param>
        /// <param name="Echelle"> Echelle dont on veut récupérer les tuiles </param>
        /// <param name="CheminTuiles"> répertoire où l'on veut enregister les tuiles </param>
        /// <remarks> cette fonction est donnée à titre d'exemple. Elle ne sert pas dans FCGP </remarks>
        internal static bool LireNiveauCarteJnx(string CheminCarteJnx, Echelles Echelle, string CheminTuiles)
        {
            // vérifier que le fichier est correct
            var InfosJnx = VerifierCarteJNX(CheminCarteJnx);
            if (InfosJnx.SiteCapture == SitesCartographiques.Aucun)
                return false;
            try
            {
                for (int Cpt = 0, loopTo = InfosJnx.NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
                {
                    var EchelleCapture = SiteIndiceEchelleToEchelle(InfosJnx.SiteCapture, InfosJnx.IndiceEchelleCapture[Cpt]);
                    if (EchelleCapture == Echelle)
                    {
                        using (var B = new JNXReader(CheminCarteJnx)) // on ouvre le fichier JNX en lecture
                        {
                            return B.LireNiveauJNX(Cpt, CheminTuiles);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AfficherErreur(ex, "Y4P3");
            }
            return false;
        }
        /// <summary> ajoute un niveau de détail à la cart au format JNX </summary>
        /// <param name="CarteActuelle">carte contenant le niveau de détail à ajouter</param>
        /// <remarks> l'odre d'écriture des niveaux (indice d'échelle) n' a pas d'importance pour le GPS mais il y en a pour BaseCamp. Il faut commencer 
        /// par la plus petite échelle donc l'indice le plus important et finir par la plus grande échelle donc indice le plus petit</remarks>
        internal static bool AjouterNiveauCarteJNX(Carte CarteActuelle)
        {
            bool AjouterNiveauCarteJNXRet = false;
            string CheminCarteJNX = CarteActuelle.CheminFichiersTuile + @"\" + CarteActuelle.Nom + CarteActuelle.NomAjout + ".jnx";
            var InfJNX = VerifierCarteJNX(CheminCarteJNX);
            if (InfJNX.SiteCapture == SitesCartographiques.Aucun)
                return AjouterNiveauCarteJNXRet; // il y a eu une erreur
            if (InfJNX.SiteCapture != CarteActuelle.SystemCartographique.SiteCarto)
                return AjouterNiveauCarteJNXRet;
            CarteActuelle.NbIterations = 0;
            CarteActuelle.NbTotalIterations = CarteActuelle.Tuiles.Length;
            AfficherVisueInformation($"{CarteActuelle.MessInfo + CarteActuelle.NbIterations} / {CarteActuelle.NbTotalIterations}");
            for (int Cpt = 0, loopTo = InfJNX.NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
            {
                int IndiceEchelleSite = EchelleToSiteIndiceEchelle(CarteActuelle.SystemCartographique.SiteCarto, CarteActuelle.SystemCartographique.Niveau.Echelle);
                if (InfJNX.IndiceEchelleCapture[Cpt] == IndiceEchelleSite)
                    return AjouterNiveauCarteJNXRet; // on ne rajoute pas le niveau de détail si il existe déjà
            }
            try
            {
                JNXLevelInfo[] NiveauInfo;
                JNXLevelMeta[] NiveauMeta;
                JNXMapInfo MapInfo;
                JNXMapMeta MapMeta;
                int IndiceNiveau;
                // lecture de l'ensemble des blocs du header de la carte
                using (var B = new JNXReader(CheminCarteJNX))
                {
                    MapInfo = B.GetMapInfo(); // on lit les infos du niveau
                    MapMeta = B.GetMapMeta();
                    IndiceNiveau = MapInfo.Levels;
                    NiveauInfo = new JNXLevelInfo[IndiceNiveau + 1]; // plus 1 pour stocker le niveau à ajouter
                    NiveauMeta = new JNXLevelMeta[IndiceNiveau + 1]; // plus 1 pour stocker le niveau à ajouter
                                                                     // lecture des niveaux de détail existants
                    for (int Cpt = 0, loopTo1 = IndiceNiveau - 1; Cpt <= loopTo1; Cpt++)
                    {
                        NiveauInfo[Cpt] = B.GetLevelInfo(Cpt);
                        NiveauMeta[Cpt] = B.GetLevelMeta(Cpt);
                    }
                }
                // on s'assure que le nom de la carte corresponde à celui du fichier jnx
                CarteActuelle.Nom = InfJNX.NomCarte;
                // on ouvre le fichier jnx en écriture
                using (var Writer = new JNXWriter(CheminCarteJNX, IndiceNiveau + 1))
                {
                    // on met à jour le nb de niveaux du fichier existant
                    Writer.SetMapInfo(MapInfo); // les données initiales et plus 1 au nb de niveau
                    Writer.SetMapMeta(MapMeta); // les données initiales et plus 1 au nb de niveau
                                                // renvoyer les différents blocks dejà existants
                    for (int Cpt = 0, loopTo2 = IndiceNiveau - 1; Cpt <= loopTo2; Cpt++)
                    {
                        Writer.SetLevelInfo(Cpt, NiveauInfo[Cpt]);
                        Writer.SetLevelMeta(Cpt, NiveauMeta[Cpt]);
                    }
                    // écrire le nouveau détail
                    Writer.EcrireNiveauJNX(CarteActuelle, IndiceNiveau);
                }
                AjouterNiveauCarteJNXRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "I2C3");
            }

            return AjouterNiveauCarteJNXRet;
        }
        /// <summary> Quand on veut créer un fichier JNX. Doit être appelée directement à partir d'une carte à partir de la fonction RealiserDemandes</summary>
        internal static bool CreerCarteJNX(Carte CarteActuelle)
        {
            bool CreerCarteJNXRet = false;
            try
            {
                CarteActuelle.NbIterations = 0;
                CarteActuelle.NbTotalIterations = CarteActuelle.Tuiles.Length;
                AfficherVisueInformation($"{CarteActuelle.MessInfo + CarteActuelle.NbIterations} / {CarteActuelle.NbTotalIterations}");
                string Clef = CarteActuelle.SystemCartographique.SiteCarto == SitesCartographiques.DomTom ? DomTomClef(CarteActuelle.SystemCartographique.DomTom) : SiteCartoClef(CarteActuelle.SystemCartographique.SiteCarto);
                string NomMap = Clef + "-" + CarteActuelle.Nom;
                var Bounds = DoJNXRect(WGS84CoordToJNX(CarteActuelle.Coins[0].Latitude), WGS84CoordToJNX(CarteActuelle.Coins[2].Longitude), WGS84CoordToJNX(CarteActuelle.Coins[2].Latitude), WGS84CoordToJNX(CarteActuelle.Coins[0].Longitude));
                using (var Writer = new JNXWriter(CarteActuelle.CheminFichiersTuile + @"\" + CarteActuelle.Nom + CarteActuelle.NomAjout + ".jnx", NomMap, Bounds))
                {
                    if (!Writer.IsOk)
                        return CreerCarteJNXRet;
                    Writer.EcrireNiveauJNX(CarteActuelle, 0);
                    // on écrit la signature et l'entête du fichier de manière indirecte.
                    // Dispose() écrit l'entête
                }
                CreerCarteJNXRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "C6S6");
            }
            return CreerCarteJNXRet;
        }
        /// <summary> Verifie si le fichier jnx correspond à la version créée à partir de la version 5 de FCGP et renvoie les infos concernant les blocs de l'entête </summary>
        /// <param name="CheminCarteJNX"></param>
        internal static InfoJNX VerifierCarteJNX(string CheminCarteJNX)
        {
            InfoJNX VerifierCarteJNXRet = new()
            {
                SiteCapture = SitesCartographiques.Aucun,
                NbNiveauxDetail = -1,
                OrdreAffichage = -1
            };
            try
            {
                using (var B = new JNXReader(CheminCarteJNX))
                {
                    if (!B.IsOk)
                        return VerifierCarteJNXRet; // Le fichier n'existe pas
                    if (!B.VerifierFormatCarteJNX())
                        return VerifierCarteJNXRet; // le fichier n'est pas de version 4 ou la signature de fin est erronnée
                    var MapInfo = B.GetMapInfo();
                    if (MapInfo.Levels < 1 | MapInfo.Levels > 5)
                        return VerifierCarteJNXRet; // le nb de niveau de détail est incorrect
                    var InOr = new InfoJNX() { NbNiveauxDetail = MapInfo.Levels, OrdreAffichage = MapInfo.ZOrder };
                    var MapMeta = B.GetMapMeta();
                    string ClefSite = null;
                    DecomposerMapName(MapMeta.MapName, ref InOr.NomCarte, ref InOr.SiteCapture, ref InOr.DomTom, ref ClefSite);

                    if (InOr.SiteCapture == SitesCartographiques.Aucun)
                        return VerifierCarteJNXRet;
                    InOr.NiveauDetail = new string[MapInfo.Levels];
                    InOr.IndiceEchelleCapture = new int[MapInfo.Levels];
                    InOr.IndiceAffichage = new double[MapInfo.Levels];
                    InOr.NiveauZoomMP = new int[MapInfo.Levels];
                    for (int Cpt = 0, loopTo = MapInfo.Levels - 1; Cpt <= loopTo; Cpt++)
                    {
                        var LevelMeta = B.GetLevelMeta(Cpt);
                        string ClefEchelle = null;
                        DecomposerLevelName(LevelMeta.Name, InOr.SiteCapture, ref InOr.IndiceEchelleCapture[Cpt], ref ClefEchelle);
                        if (InOr.IndiceEchelleCapture[Cpt] == -1)
                            return VerifierCarteJNXRet;
                        InOr.NiveauZoomMP[Cpt] = LevelMeta.Zoom;
                        var LevelInfo = B.GetLevelInfo(Cpt);
                        InOr.IndiceAffichage[Cpt] = TrouverIndiceAffichage(LevelInfo.Scale);
                        InOr.NiveauDetail[Cpt] = ClefSite + "-" + ClefEchelle;
                    }
                    VerifierCarteJNXRet = InOr;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "P2S1");
            }
            return VerifierCarteJNXRet;
        }
        /// <summary> change l'ordre d'affichage de la carte au format JNX. " </summary>
        /// <param name="CheminCarteJNX"> carte au format </param>
        /// <param name="Valeur"></param>
        internal static bool ChangerOrdreAffichageCarteJNX(string CheminCarteJNX, int Valeur)
        {
            bool ChangerOrdreAffichageCarteJNXRet = false;
            var InfosJnx = VerifierCarteJNX(CheminCarteJNX);
            if (InfosJnx.SiteCapture == SitesCartographiques.Aucun)
                return ChangerOrdreAffichageCarteJNXRet;
            try
            {
                using (var Writer = new JNXWriter(CheminCarteJNX, Valeur, (double)-1m))
                {
                    ChangerOrdreAffichageCarteJNXRet = Writer.IsOk;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "W1T4");
            }

            return ChangerOrdreAffichageCarteJNXRet;
        }
        /// <summary>  change l'indice d'affichage d'un niveau de détail (échelle à partir de laquelle le niveau de détail disparait</summary>
        /// <param name="CheminCarteJNX">carte au format JNX</param>
        /// <param name="IndexNiveau">le niveau de détail concerné par le changement</param>
        /// <param name="IndiceAffichageModifie">valeur à ajouter à l'indice d'affichage. Cet indice sera traduit en valeur d'échelle dans le fichier JNX. Si positif le niveau disparait à une échelle plus grande</param>
        /// <remarks>aucune vérification par rapport aux autres niveaux de détail</remarks>
        internal static bool ChangerNiveauAffichageCarteJNX(string CheminCarteJNX, int IndexNiveau, double IndiceAffichageModifie)
        {
            bool ChangerNiveauAffichageCarteJNXRet = false;
            var InfosJnx = VerifierCarteJNX(CheminCarteJNX);
            if (InfosJnx.SiteCapture == SitesCartographiques.Aucun)
                return false;
            try
            {
                using (var Writer = new JNXWriter(CheminCarteJNX, IndexNiveau, IndiceAffichageModifie))
                {
                    ChangerNiveauAffichageCarteJNXRet = Writer.IsOk;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "D8M3");
            }

            return ChangerNiveauAffichageCarteJNXRet;
        }
        /// <summary> pour renvoyer toutes les informations utiles concernant les fichiers JNX </summary>
        internal struct InfoJNX
        {
            internal SitesCartographiques SiteCapture;
            internal DomsToms DomTom;
            internal string NomCarte;
            internal int NbNiveauxDetail;
            internal int OrdreAffichage; // ordre d'affichage de la carte par rapport aux autres : détermine la visibilité de la carte
            internal string[] NiveauDetail;
            internal int[] IndiceEchelleCapture;
            /// <summary> valeur par pas de 0.25 indiquant, une fois transformée en valeur d'échelle dans le fichier JNX, 
            /// à quelle échelle d'affichage le niveau de détail disparait </summary>
            internal double[] IndiceAffichage;
            internal int[] NiveauZoomMP;
        }
        #endregion
        #region constantes
        private const int NbOctetsUInt32 = 4;
        private const int NbOctetsInt32 = 4;
        private const int NbOctetsInt16 = 2;
        private const int NbOctetsJNXPoint = NbOctetsInt32 * 2;
        private const int NbOctetsJNXRect = NbOctetsJNXPoint * 2;
        private const int NbOctetsTileInfo = NbOctetsJNXRect + NbOctetsInt16 * 2 + NbOctetsInt32 + NbOctetsUInt32;
        private const int NbOctetsMapInfo = NbOctetsInt32 * 8 + NbOctetsJNXRect + NbOctetsUInt32;
        private const string JNX_EOF_SIGNATURE = "BirdsEye";
        private const double JnxToDec = 180d / 0X_7F_FF_FF_FF;
        private const double DecToJnx = 0X_7F_FF_FF_FF / 180d;
        /// <summary> différentes échelles en fonction de l'indice de zoom </summary>
        private readonly static int[] ZoomToScale = new int[] { 19, 37, 75, 149, 298, 597, 1194, 2388, 4777, 9554, 19109, 38218, 76437, 152877,
                                                                305758, 611526, 1223072, 2446184, 2446184, 2446184, 2446184, 2446184, 2446184, 2446184 };
        #endregion
        #region Fonctions non visibles 
        /// <summary> retrouve dans le nom du niveau de détail la clef et l'indice de l'échelle de capture </summary>
        /// <param name="LevelName"> Nom du niveau de détail </param>
        /// <param name="SiteCapture"> site de capture </param>
        /// <param name="IndiceEchelle"> indice de l'échelle </param>
        /// <param name="ClefEchelle"> clef de l'échelle </param>
        private static void DecomposerLevelName(string LevelName, SitesCartographiques SiteCapture, ref int IndiceEchelle, ref string ClefEchelle)
        {
            int Pos = LevelName.IndexOf('-');
            ClefEchelle = LevelName[..Pos];
            IndiceEchelle = EchelleClefToSiteIndiceEchelle(SiteCapture, ClefEchelle);
        }
        /// <summary> retrouve dans le nom de la carteJNX, le nom de la carte, le site, la clef et le domtom de capture </summary>
        /// <param name="MapName"> nom de la carteJNX </param>
        /// <param name="NomCarte"> nom de la carte retrouvé </param>
        /// <param name="SiteCapture"> site retrouvé</param>
        /// <param name="DomTom"> DomTom retrouvé </param>
        /// <param name="ClefSite"> clef du site retrouvé </param>
        private static void DecomposerMapName(string MapName, ref string NomCarte, ref SitesCartographiques SiteCapture, ref DomsToms DomTom, ref string ClefSite)
        {
            int Pos = MapName.IndexOf('-');
            ClefSite = MapName[..Pos];
            NomCarte = MapName[(Pos + 1)..];
            SiteCapture = TrouverSiteCartographiqueCle(ClefSite);
            DomTom = DomsToms.aucun;
            if (SiteCapture == SitesCartographiques.Aucun)
            {
                DomTom = TrouverDomTomCle(ClefSite);
                if (DomTom > DomsToms.aucun)
                {
                    SiteCapture = SitesCartographiques.DomTom;
                }
                else
                {
                    throw new Exception("Fichier JNX corrompu");
                }
            }
        }
        /// <summary> crée une structure JNXRect à partir de 4 coordonnées exprimées en coordonnées GPS </summary>
        private static JNXRect DoJNXRect(int northern_lat, int eastern_lon, int southern_lat, int western_lon)
        {
            JNXRect DoJNXRectRet = default;
            DoJNXRectRet.northEast.Latitude = Math.Max(northern_lat, southern_lat);
            DoJNXRectRet.northEast.Longitude = Math.Max(eastern_lon, western_lon);
            DoJNXRectRet.southWest.Latitude = Math.Min(northern_lat, southern_lat);
            DoJNXRectRet.southWest.Longitude = Math.Min(eastern_lon, western_lon);
            return DoJNXRectRet;
        }
        /// <summary> transforme le nb de métre par pixel d'une carte capturée en échelle maximum à indiquer dans les infos du niveau </summary>
        /// <param name="d"> echelleX exprimée en nombre de mètre par pixel de l'image </param>
        private static int MetersPerPixelToScale(double d, double FacteurJNX)
        {
            // on détermine l'indice d'échelle normal pour un facteur 0
            int IndiceEchelle = 12 + (int)Math.Round(Math.Log(d * 1000d / 76437d) / Math.Log(2d));
            // on le corrige en rajoutant la partie entière du facteur de correction
            IndiceEchelle += (int)Math.Floor(FacteurJNX);
            double Coef = FacteurJNX - Math.Floor(FacteurJNX); // soit 0 soit 0.5 mais peut être amené à changer par pas de 0.1
            if (Coef > 0d) // si la partie fractionnaire du facteur de correction est supérieure à 0 alors on apporte une correction supplémentaire
            {
                return ZoomToScale[IndiceEchelle] + (int)Math.Round((ZoomToScale[IndiceEchelle + 1] - ZoomToScale[IndiceEchelle]) * Coef);
            }
            else
            {
                return ZoomToScale[IndiceEchelle];
            }
        }
        /// <summary> tranforme un coordonnée exprimée en degré décimal en unité de coordonnées GPS</summary>
        private static int WGS84CoordToJNX(double CoordonneesWGS84)
        {
            return (int)Math.Round(CoordonneesWGS84 * DecToJnx);
        }
        /// <summary> tranforme un coordonnée exprimée en unité de coordonnées GPS en degré décimal</summary>
        private static double JNXCoordToWGS84(int CoordonneesJNX)
        {
            return CoordonneesJNX * JnxToDec;
        }
        /// <summary> renvoie un indice d'affichage en fonction d'une valeur passée en paramètre</summary>
        private static double TrouverIndiceAffichage(int Scale)
        {
            for (int Cpt = 0, loopTo = ZoomToScale.Length - 1; Cpt <= loopTo; Cpt++)
            {
                if (ZoomToScale[Cpt] == Scale)
                {
                    return Cpt;
                }
                else if (ZoomToScale[Cpt] > Scale)
                {
                    int EcartGlobal = ZoomToScale[Cpt] - ZoomToScale[Cpt - 1];
                    int EcartValeur = ZoomToScale[Cpt] - Scale;
                    int Correction = (int)Math.Round(EcartValeur / (double)EcartGlobal * 100d);
                    return Cpt - Correction / 100d;
                }
            }
            return -1;
        }
        /// <summary> renvoie une echelle d'affichage en fonction d'un indice passé en paramètre</summary>
        private static int TrouverScaleAffichage(double IndiceAffichage)
        {
            int Indice = (int)Math.Floor(IndiceAffichage);
            double Fract = IndiceAffichage - Indice;
            if (Fract == 0d)
                return ZoomToScale[Indice];
            int Ecart = (int)Math.Round((ZoomToScale[Indice + 1] - ZoomToScale[Indice]) * Fract);
            return ZoomToScale[Indice] + Ecart;
        }
        #endregion
        #region Structures d'un fichier JNX 
        /// <summary>point de coordonnées en unitées GPS. Voir fonction WGS84ToJNX</summary>
        private struct JNXPoint
        {
            internal int Latitude; // stocker sous la forme : DD * (2^31-1) / 180
            internal int Longitude; // stocker sous la forme : DD * (2^31-1) / 180
        }
        /// <summary>coordonnées d'une tuile ou de la carte en unité GPS</summary>
        private struct JNXRect
        {
            internal JNXPoint northEast;
            internal JNXPoint southWest;
        }
        /// <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 1ère</summary>
        private struct JNXMapInfo
        {
            internal int Version; // N° de version : 3 ou 4. toujours 4 dans le cas de FCGP
            internal int DeviceSN; // Identifiant du périphérique de téléchargement. Toujours 0 avec les JNX de démo
            internal JNXRect MapBounds; // Coordonnées de la carte. 2 points voir strucutre JNXRect
            internal int Levels; // Nombre de niveau de détail de la carte. Maximum 5
            internal int Expiration; // date d'expiration en cas detéléchargement exprimée en nb de seconde depuis 30/12/1989 12H00. Toujours 0 avec les JNX de démo
            internal int ProductID; // identifiant du site de téléchargement. Toujours 0 avec les JNX de démo
            internal int CRC; // CRC32 es coordonnées des tuiles. Toujours 0 avec les JNX de démo
            internal int SigVersion; // Signature de la version du fichier téléchargé. Toujours 0 avec les JNX de démo
            internal uint SigOffset; // Offset de la signature. Toujours 0 avec les JNX de démo
            internal int ZOrder; // ordre d'affichage sur l'axe de Z sur le périphérique de éléchargement. de0 à 100. 100 étant le haut donc le plus visible
        }
        /// <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 3ème</summary>
        private struct JNXMapMeta
        {
            internal int LevelMetaCount; // nombre de niveau de détail de la carte.Le même que celui du block JNXMapinfo
            internal string MapName; // nom de la carte
            internal short ProductID; // Identifiant du site de téléchargement. le même que celui du block JNXMapinfo
            internal string Unknow; // chaine non utilisée. Toujours 0
            internal string ProductName; // nom du produit. toujours une chaine vide soit 0
            internal string GUID; // GUID de la carte. Identifiant unique de la forme 12345678-1234-1234-1234-123456789ABC. soir 36 caractères
            internal int Version; // version du block MapMeta. toujours 9
        }
        /// <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 2ème, 1 table avec nb niveaux enregistrements</summary>
        private struct JNXLevelInfo
        {
            internal string Copyright; // Copyright du niveau
            internal int Unknow; // inconnu. Toujours 2 avec les JNX de démo
            internal int Scale; // Echelle d'affichage à partir duquel le niveau disparait ou apparait à l'affichage
            internal uint TilesOffset; // offset de la table de descritpion des tuiles
            internal int TileCount; // nombre de tuiles composant le niveau
        }
        /// <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 4ème, 1 table avec nb niveaux enregistrements</summary>
        private struct JNXLevelMeta
        {
            internal string Name; // Nom du niveau. doit contenir la clef de l'échelle de capture
            internal string Description; // chaine non utilisée. Toujours 0
            internal string Copyright; // copyright du niveau. Le même que celui du block JNXLevelInfo
            internal int Zoom; // zoom de mapsource utilisé pour générer le niveau. doit être différent si plusieurs niveaux
        }
        /// <summary>l'entête du fichier JNX est découpé en plusieurs parties. Il s'agit de la 5ème, 1 table par niveau avec nbtuiles enregistrements</summary>
        private struct JNXTileInfo
        {
            internal uint PicOffset; // offset de la tuile par rapport au début du fchier
            internal int PicSize; // aille de la tuile en octet sachant que la tuile est un fichier jpeg sans le 2 premiers octets qui sont la reconnaissance du format
            internal short PicHeight; // hauteur de la tuile en pixel
            internal short PicWidth; // largeur de la tuile en pixel
            internal JNXRect TileBounds; // coordonnées de la tuile. 2 points voir strucutre JNXRect
        }
        #endregion
        /// <summary> cette classe gère l'écriture d'un fichier au format JNX. Seule la version 4 du format avec un seul fichier est supporté. 
        /// On enregistre d'abord les niveaux de détail, pour un niveau c'est d'abord les toutes les images puis les infos tuiles afin d'éviter de modifier la position d'écriture. 
        /// L'image des tuiles doit être au format Jpeg. Les coordonnées des tuiles sont exprimées en unitées GPS. Voir les fonctions WGS84ToJNX et DoJNXRect.
        /// Les autres niveaux commencent après la dernière image du niveau précédent. Les blocs composants l'entête de fichier sont enrigistrés à la fermeture du fichier JNX.
        /// Les fichiers JNX ont des limitations. Niveaux : de 1 à 5, Nb Tuiles : de 1 à 50000, Dimensions des Tuiles : inf ou égal à 1024*1024 et en multiple de 8 si possible, 
        /// NbOctets Tuiles : inf à 3Mo, NBOctets fichier JNX : inf à 100Mo. La table d'info tuiles du 1er niveau commence toujours à l'offset 1024 ''' </summary>
        /// <remarks> La taille du fichier JNX peut dépasser la capacité d'un integer d'où l'abandon de seek pour Basestream.Position au niveau de l'écriture des tuiles </remarks>
        private class JNXWriter : IDisposable
        {
            #region constantes et variables
            /// <summary>contient les données de la carte</summary>
            private JNXMapInfo FMapInfo;
            /// <summary>contient les données du niveau</summary>
            private JNXLevelInfo[] FLevelInfo;
            /// <summary>contient les données descriptives de la carte</summary>
            private JNXMapMeta FMapMeta;
            /// <summary>contient les données descriptives des niveau</summary>
            private JNXLevelMeta[] FLevelMeta;
            /// <summary>support du fichier à écrire</summary>
            private readonly BinaryWriter FileBinaire;
            /// <summary>nb de niveaux de détail du fichier</summary>
            private readonly int NbNiveaux;
            /// <summary>indique si il s'agit d'une création ou d'un ajout de niveau de détail</summary>
            private readonly bool FlagCreation;
            /// <summary>addresse d'écriture de l'image de la tuile par rapport au début du fichier binaire</summary>
            private uint OffsetImageTuile;
            /// <summary>addresse d'écriture des données de la tuile par rapport au début du fichier binaire</summary>
            private readonly uint OffsetInfoTuile;
            private const int MAX_TILES = 50000;
            private const long MAX_TILE_SIZE = 0x_30_00_00L;
            private const long MAX_FILE_SIZE = 0x_1_00_00_00_00L;
            private const int JNX3_ZORDER = 75;
            private const int META_BLOCK_VERSION = 9;
            private const int META_BLOCK_MINSIZE = 1024;

            private BinaryWriter Infostuiles;
            private string BaseCheminTuiles;
            #endregion
            #region procédures et fonctions
            #region visible
            /// <summary>ok si l'ouverture ou la création du fichier binaire c'est bien passée </summary>
            internal bool IsOk { get; private set; }
            /// <summary> constructeur pour la modification de l'ordre d'affichage ou du niveau d'affichage d'un niveau </summary>
            /// <param name="Path">nom du fichier jnx</param>
            /// <param name="Valeur"> ordre d'affichage de la carte JNX ou indice du niveau de détail à modifier </param>
            /// <param name="NiveauAffichage"> indice d'affichage modifié du niveau de détail si >-1 </param>
            internal JNXWriter(string Path, int Valeur, double NiveauAffichage)
            {
                try
                {
                    IsChangedValeur = true;
                    FileBinaire = new BinaryWriter(File.Open(Path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
                    using (var B = new JNXReader(Path))
                    {
                        if (!B.IsOk)
                            return;
                        if (NiveauAffichage == -1)
                        {
                            // on modifie la valuer de l'ordre d'affichage de la carte JNX
                            FMapInfo = B.GetMapInfo();
                            FMapInfo.ZOrder = Valeur;
                            EcrireJNXMapInfo();
                        }
                        else
                        {
                            // on modifie la valeur de l'indice d'affichage d'un niveau de détail
                            FLevelInfo = new JNXLevelInfo[1];
                            FLevelInfo[0] = B.GetLevelInfo(Valeur); // on lit les infos du niveau
                            var OffsetLevelInfo = B.Offset.LevelInfo[Valeur];
                            FLevelInfo[0].Scale = TrouverScaleAffichage(NiveauAffichage);
                            EcrireJNXLevelInfo(FLevelInfo[0], (int)OffsetLevelInfo);
                        }
                        IsOk = true;
                    }
                    IsOk = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "AF55");
                    IsOk = false;
                }
            }
            /// <summary> constructeur pour l'ajout d'un niveau de détail à un fichier jnx</summary>
            /// <param name="Path">nom du fichier jnx</param>
            /// <param name="NbNiveauxDetail">nb de niveau de détail avec celui encours d'écriture</param>
            internal JNXWriter(string Path, int NbNiveauxDetail)
            {
                try
                {
                    FileBinaire = new BinaryWriter(File.Open(Path, FileMode.Open, FileAccess.Write));
                    NbNiveaux = NbNiveauxDetail;
                    FlagCreation = false;
                    DimensionnerLevels();
                    OffsetInfoTuile = (uint)(FileBinaire.BaseStream.Length - JNX_EOF_SIGNATURE.Length);
                    IsOk = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "A1Y6");
                }
            }
            /// <summary> constructeur pour la création d'un fichier jnx</summary>
            /// <param name="Path">nom du fichier jnx</param>
            /// <param name="NomMap">nom qui servira de nom de carte interne au fichier jnx et aux niveaux de détail</param>
            /// <param name="Bounds">coordonnées de la carte exprimées en unités GPS</param>
            internal JNXWriter(string Path, string NomMap, JNXRect Bounds)
            {
                try
                {
                    FileBinaire = new BinaryWriter(File.Open(Path, FileMode.Create, FileAccess.Write));
                    NbNiveaux = 1;
                    FlagCreation = true;
                    DimensionnerLevels();
                    InitMapInfo();
                    OffsetInfoTuile = META_BLOCK_MINSIZE;
                    FMapMeta.MapName = NomMap;
                    FMapInfo.MapBounds = Bounds;
                    IsOk = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "E0J0");
                }
            }
            /// <summary> permet de libérer les ressources associées à l'EditableBitmap. N'a pas pour vocation a être appelé directement mais par le end using</summary>
            public void Dispose()
            {
                CloseFile();
                // nécessaire pour éviter que le GC ne redemande Finalize()
                GC.SuppressFinalize(this);
            }
            /// <summary> écrit un niveau de détail dans le fichier : infos et image des tuiles et met à jour les infos des blocs niveaux correpondant</summary>
            /// <param name="CarteActuelle"></param>
            /// <param name="IndiceNiveau"></param>
            internal void EcrireNiveauJNX(Carte CarteActuelle, int IndiceNiveau)
            {
                FLevelMeta[IndiceNiveau].Name = CarteActuelle.SystemCartographique.Niveau.Clef + "-" + CarteActuelle.Nom;
                FLevelInfo[IndiceNiveau].Scale = TrouverScaleAffichage(CarteActuelle.FacteurJNX);
                if (CarteActuelle.Tuiles.Length > MAX_TILES)
                    throw new Exception("Le nombres de tuiles admissibles pour le niveau " + IndiceNiveau + " est dépassé");
                FLevelInfo[IndiceNiveau].TileCount = CarteActuelle.Tuiles.Length;
                FLevelInfo[IndiceNiveau].TilesOffset = OffsetInfoTuile;
                // on garde dans le fichier jnx le nb de col et rang pour pour restituer le nom des tuiles
                FLevelInfo[IndiceNiveau].Copyright = $"{CarteActuelle.NbTuilesX:D3}*{CarteActuelle.NbTuilesY:D3}";
                FLevelMeta[IndiceNiveau].Zoom = IndiceNiveau + 1;
                // initialiser l'offset de départ pour l'enregistrement de l'image des tuile
                OffsetImageTuile = OffsetInfoTuile + (uint)(CarteActuelle.Tuiles.Length * NbOctetsTileInfo);
                // verifier que tout est ok
                if (!IsHeaderCompleted(IndiceNiveau))
                    throw new Exception("La description de l'entête du fichier JNX n'est pas complète : " + "A minima, les blocks de niveaux de détail existent et Nb de tuiles par niveau");
                // afin d'éviter des positionnement incessant au début et en fin de fichier 
                var MemoireInfosTuiles = new MemoryStream(FLevelInfo[IndiceNiveau].TileCount * NbOctetsTileInfo);
                Infostuiles = new BinaryWriter(MemoireInfosTuiles);
                // on positionne au début des données binaire de la 1ère tuile à écrire
                FileBinaire.BaseStream.Position = OffsetImageTuile;
                BaseCheminTuiles = CheminEnregistrementProvisoire + Carte.ExtensionCheminImagesTuiles + @"\";
                // on écrit les tuiles
                foreach (DescriptionImageFichier T in CarteActuelle.Tuiles)
                {
                    // on lit l' image au format jpeg dans un buffer(tableau d'octets)
                    EcrireTuile(IndiceNiveau, T);
                    CarteActuelle.NbIterations += 1;
                    if (CarteActuelle.NbIterations % 100 == 0)
                        AfficherVisueInformation($"{CarteActuelle.MessInfo + CarteActuelle.NbIterations} / {CarteActuelle.NbTotalIterations}");
                }
                // on positionne sur l'emplacement des infos tuiles afin d'écrire les infos
                FileBinaire.BaseStream.Position = OffsetInfoTuile;
                FileBinaire.Write(MemoireInfosTuiles.ToArray());
            }
            /// <summary> initialise la structure MapInfo du fichier JNX en une fois et ajoute 1 au nb de niveau de détail car forcément appelée par AjoutNiveau</summary>
            internal void SetMapInfo(JNXMapInfo MapInfo)
            {
                if (FlagCreation)
                    throw new Exception("Vous ne pouvez pas transmettre le block MapInfo en mode création");
                // on rajoute un niveau
                FMapInfo = MapInfo;
                FMapInfo.Levels += 1;
            }
            /// <summary> initialise la structure MapMeta du fichier JNX en une fois et ajoute 1 au nb de niveau de détail car forcément appelée par AjoutNiveau</summary>
            internal void SetMapMeta(JNXMapMeta MapMeta)
            {
                if (FlagCreation)
                    throw new Exception("Vous ne pouvez pas transmettre le block MapMeta en mode création");
                FMapMeta = MapMeta;
                FMapMeta.LevelMetaCount += 1;
            }
            /// <summary> initialise une des structures LevelInfo du fichier JNX en une fois. Forcément appeléé par AjoutNiveau </summary>
            internal void SetLevelInfo(int IndiceNiveau, JNXLevelInfo LevelInfo)
            {
                if (FlagCreation)
                    throw new Exception("Vous ne pouvez pas transmettre le block LevelInfo en mode création");
                FLevelInfo[IndiceNiveau] = LevelInfo;
            }
            /// <summary> initialise une des structures LevelMeta du fichier JNX en une fois. Forcément appeléé par AjoutNiveau </summary>
            internal void SetLevelMeta(int IndiceNiveau, JNXLevelMeta LevelMeta)
            {
                if (FlagCreation)
                    throw new Exception("Vous ne pouvez pas transmettre le block LevelMeta en mode création");
                FLevelMeta[IndiceNiveau] = LevelMeta;
            }
            /// <summary> champ non renseigné par défaut lors de l'initialisation des blocs de niveaux</summary>
            internal void SetLevelCopyright(int IndiceNiveau, string Copyright)
            {
                FLevelMeta[IndiceNiveau].Copyright = Copyright;
            }
            /// <summary> champ non renseigné par défaut lors de l'initialisation des blocs de niveaux</summary>
            internal void SetLevelDescription(int IndiceNiveau, string Description)
            {
                FLevelMeta[IndiceNiveau].Description = Description;
            }
            #endregion
            #region invisible
            /// <summary> indique si le constructeur utilisé est prévue pour changer une valeur ou pour créer ou ajouter un niveau </summary>
            private readonly bool IsChangedValeur;
            /// <summary>écrit la signature et l'entête du fichier JNX puis cloture celui ci</summary>
            private void CloseFile()
            {
                try
                {
                    if (FileBinaire is not null)
                    {
                        if (!IsChangedValeur)
                        {
                            // on est dans la création ou l'ajout d'un niveau
                            EcrireEnteteEtSignature();
                        }
                        FileBinaire.Flush();
                    }
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "X9S1");
                }
                finally
                {
                    FileBinaire?.Close();
                }
            }
            /// <summary> dimensionne les tableaux concernant les niveaux de détails </summary>
            private void DimensionnerLevels()
            {
                FLevelInfo = new JNXLevelInfo[NbNiveaux];
                FLevelMeta = new JNXLevelMeta[NbNiveaux];
            }
            /// <summary> Sert d'initialisateur de champs de la création du fichier JNX. Certains champs sont toujours indentiques. </summary>
            private void InitMapInfo()
            {
                if (!FlagCreation)
                    throw new Exception("Vous ne pouvez plus initialiser ces champs directement en mode ajout");
                FMapInfo.Version = 4;
                FMapInfo.ZOrder = JNX3_ZORDER;
                FMapMeta.Version = META_BLOCK_VERSION;
                FMapMeta.GUID = CreateGUID();
                FMapInfo.Levels = 1;
                FMapMeta.LevelMetaCount = 1;
            }
            /// <summary> enregistre la tuile, infos et image dans le fichier JNX </summary>
            /// <param name="IndiceNiveau"> indice du niveau de détail de la tuile </param>
            /// <param name=T"> Description de la tuile </param>
            /// <remarks> on vérifie que le nb de tuiles du niveaux n'est pas dépassé et que la taille du fichier reste correcte</remarks>
            private void EcrireTuile(int IndiceNiveau, DescriptionImageFichier T)
            {
                string CheminTuile = BaseCheminTuiles + T.Nom;
                byte[] BitsJpeg = File.ReadAllBytes(CheminTuile);
                // on ne prend pas en compte les 2 octets marqueur du type de fichier (jpg)
                int TailleJpeg = BitsJpeg.Length - 2;
                if (TailleJpeg < 1)
                    throw new Exception("L'image de la tuile n'est pas correcte.");
                if (TailleJpeg >= MAX_TILE_SIZE)
                    throw new Exception("Une des tuiles du niveau " + IndiceNiveau + " dépasse le poids autorisé.");
                if (OffsetImageTuile + TailleJpeg + JNX_EOF_SIGNATURE.Length >= MAX_FILE_SIZE)
                    throw new Exception("Le poids total du fichier est dépassé au niveau " + IndiceNiveau);

                // calcul des coordonnées de la tuile exprimées en DD unité GPS (entier)
                var Bounds = DoJNXRect(WGS84CoordToJNX(T.Nord), WGS84CoordToJNX(T.Est),
                                       WGS84CoordToJNX(T.Sud), WGS84CoordToJNX(T.Ouest));
                var Tuile = new JNXTileInfo()
                {
                    TileBounds = Bounds,
                    PicWidth = (short)T.LargeurPixels,
                    PicHeight = (short)T.HauteurPixels,
                    PicSize = TailleJpeg,
                    PicOffset = OffsetImageTuile
                };
                // écriture de tileInfo dans le tampon infotuile
                EcrireJNXTileInfo(Tuile);
                // écriture de l'image de la tuile dans le fichier jnx
                FileBinaire.Write(BitsJpeg, 2, TailleJpeg);
                // calcul du nouvel index
                OffsetImageTuile = (uint)FileBinaire.BaseStream.Position;
            }
            /// <summary> écrit l'entête et la fin du fichier. L'entête est composé par les structures d'information globale de la carte et par
            /// les structures d'informations concernant les différents niveaux </summary>
            private void EcrireEnteteEtSignature()
            {
                // on positionne à la fin de la dernière tuile écrite
                FileBinaire.BaseStream.Position = OffsetImageTuile;
                // il suffit de mettre la signature de fin au bon endroit c'est à dire après la dernière image de tuile
                EcrireJNXSignature();
                // on revient à zéro et on enregistre le header
                FileBinaire.BaseStream.Position = 0L;
                EcrireJNXMapInfo(); // bloc à longueur constante
                for (int Cpt = 0, loopTo = NbNiveaux - 1; Cpt <= loopTo; Cpt++)
                    EcrireJNXLevelInfo(FLevelInfo[Cpt]); // bloc à longueur constante
                EcrireJNXMapMeta(FMapMeta); // bloc à longueur variable
                for (int Cpt = 0, loopTo1 = NbNiveaux - 1; Cpt <= loopTo1; Cpt++)
                    EcrireJNXLevelMeta(FLevelMeta[Cpt]); // bloc à longueur variable
            }
            /// <summary>vérifie que les infos obligatoires ont été renseignées </summary>
            private bool IsHeaderCompleted(int IndiceNiveau)
            {
                if (!(FMapInfo.Levels == NbNiveaux))
                    return false;
                if (!(FLevelInfo[IndiceNiveau].TileCount > 0))
                    return false;
                return true;
            }
            /// <summary> écrit en fin du fichier la signature du fichier JNX </summary>
            private void EcrireJNXSignature()
            {
                FileBinaire.Write(Encoding.UTF8.GetBytes(JNX_EOF_SIGNATURE));
            }
            /// <summary> écrit une structue JNXPoint dans un flux binaire</summary>
            /// <param name="R"> structure JNXPoint </param>
            /// <param name="IsTuile"> Si il s'agit d'une tuile on écrit pas directement dans le fichier pour éviter les changements de position du pointeur de position </param>
            private void EcrireJNXPoint(JNXPoint R, bool IsTuile)
            {
                if (IsTuile)
                {
                    Infostuiles.Write(R.Latitude);
                    Infostuiles.Write(R.Longitude);
                }
                else
                {
                    FileBinaire.Write(R.Latitude);
                    FileBinaire.Write(R.Longitude);
                }
            }
            /// <summary> écrit une structue JNXRect dans un flux binaire</summary>
            /// <param name="R">structure JNXRect</param>
            /// <param name="IsTuile"> Si il s'agit d'une tuile on écrit pas directement dans le fichier pour éviter les changements de position du pointeur de position </param>
            private void EcrireJNXRect(JNXRect R, bool IsTuile = false)
            {
                EcrireJNXPoint(R.northEast, IsTuile);
                EcrireJNXPoint(R.southWest, IsTuile);
            }
            /// <summary> écrit une structue JNXMapInfo dans le fichier binaire</summary>
            /// <param name="FMapInfo">structure JNXMapInfo</param>
            private void EcrireJNXMapInfo()
            {
                {
                    ref var withBlock = ref FMapInfo;
                    FileBinaire.Write(withBlock.Version);
                    FileBinaire.Write(withBlock.DeviceSN);
                    EcrireJNXRect(withBlock.MapBounds);
                    FileBinaire.Write(withBlock.Levels);
                    FileBinaire.Write(withBlock.Expiration);
                    FileBinaire.Write(withBlock.ProductID);
                    FileBinaire.Write(withBlock.CRC);
                    FileBinaire.Write(withBlock.SigVersion);
                    FileBinaire.Write(withBlock.SigOffset);
                    FileBinaire.Write(withBlock.ZOrder);
                }
            }
            /// <summary> écrit une structue JNXMapMeta dans le fichier binaire</summary>
            /// <param name="FMapMeta">structure JNXMapMeta</param>
            private void EcrireJNXMapMeta(JNXMapMeta FMapMeta)
            {
                FileBinaire.Write(FMapMeta.Version);
                EcrireStringC(FMapMeta.GUID);
                EcrireStringC(FMapMeta.ProductName);
                EcrireStringC(FMapMeta.Unknow);
                FileBinaire.Write(FMapMeta.ProductID);
                EcrireStringC(FMapMeta.MapName);
                FileBinaire.Write(FMapMeta.LevelMetaCount);
            }
            /// <summary> écrit une structue JNXLevelInfo dans le fichier binaire </summary>
            /// <param name="FLevelInfo">structure JNXLevelInfo</param>
            /// <param name="Position"> indique la position dans le flux binaire si la position courante n'est pas la bonne. 
            /// Concerne le changement de valeur et pas la création ou l'ajout d'un niveau de détail </param>
            private void EcrireJNXLevelInfo(JNXLevelInfo FLevelInfo, int Position = -1)
            {
                if (Position > -1)
                {
                    // on est dans la modification du niveau d'affichage
                    FileBinaire.Seek(Position, SeekOrigin.Begin);
                }
                FLevelInfo.Unknow = 2;
                FileBinaire.Write(FLevelInfo.TileCount);
                FileBinaire.Write(FLevelInfo.TilesOffset);
                FileBinaire.Write(FLevelInfo.Scale);
                FileBinaire.Write(FLevelInfo.Unknow);
                EcrireStringC(FLevelInfo.Copyright);
            }
            /// <summary> écrit une structue JNXLevelMeta dans le fichier binaire</summary>
            /// <param name="FLevelMeta">structure JNXLevelMeta</param>
            private void EcrireJNXLevelMeta(JNXLevelMeta FLevelMeta)
            {
                EcrireStringC(FLevelMeta.Name);
                EcrireStringC(FLevelMeta.Description);
                EcrireStringC(FLevelMeta.Copyright);
                FileBinaire.Write(FLevelMeta.Zoom);
            }
            /// <summary> écrit une structue JNXTitleInfo dans le fichier binaire</summary>
            /// <param name="TuileInfo">structure JNXTileInfo</param>
            private void EcrireJNXTileInfo(JNXTileInfo TuileInfo)
            {
                EcrireJNXRect(TuileInfo.TileBounds, true);
                Infostuiles.Write(TuileInfo.PicWidth);
                Infostuiles.Write(TuileInfo.PicHeight);
                Infostuiles.Write(TuileInfo.PicSize);
                Infostuiles.Write(TuileInfo.PicOffset);
            }
            /// <summary> écrit une chaine de caractère en utf8 dans le flux binaire en rajoutant un null terminated comme pour les chaines du langage C </summary>
            /// <param name="S"> Chaine à écrire dans le flux </param>
            private void EcrireStringC(string S)
            {
                FileBinaire.Write(Encoding.UTF8.GetBytes(S + NullChar));
            }
            #endregion
            #endregion
        }
        /// <summary> cette classe gère la lecture d'un fichier au format JNX. Seule la version 4 du format est supportée.
        /// Les coordonnées des tuiles sont exprimées en unitées GPS. Voir fonction JNXCoordToWGS84.
        /// Les fichiers JNX ont certaines limitations : Niveaux : de 1 à 5, Nb Tuiles : de 1 à 50000, 
        /// Dimensions des Tuiles : inf ou égal à 1024*1024 et en multiple de 8 si possible, 
        /// NbOctets Tuiles : inf à 3Mo, NBOctets fichier JNX : inf à 100Mo </summary>
        private class JNXReader : IDisposable
        {
            #region constantes et variables
            private JNXMapInfo FMapInfo;
            private JNXLevelInfo[] FLevelsInfo;
            private JNXMapMeta FMapMeta;
            private JNXLevelMeta[] FLevelsMeta;
            private BinaryReader FileBinaire;
            internal OffsetJNX Offset;
            internal struct OffsetJNX
            {
                internal uint[] LevelInfo;
                internal uint[] LevelMeta;
                internal long MapMeta;
            }
            #endregion
            #region procédures et fonctions
            #region visibles
            /// <summary> ok si l'ouverture du fichier binaire c'est bien passée </summary>
            internal bool IsOk { get; private set; }
            /// <summary> permet de libérer les ressources associées. N'a pas pour vocation a être appelé directement mais indirectement par END USING</summary>
            public void Dispose()
            {
                FileBinaire?.Close();
                // nécessaire pour éviter que le GC ne redemande Finalize()
                GC.SuppressFinalize(this);
            }
            /// <summary> unique constructeur de cette classe</summary>
            internal JNXReader(string Path)
            {
                if (OpenFile(Path))
                {
                    LireMapInfo();
                    LireLevelsInfo();
                    LireMapMeta();
                    LireLevelsMeta();
                    IsOk = true;
                }
                else
                {
                    IsOk = false;
                }
            }
            /// <summary> renvoie la structure de type JNXMapInfo du fichier JNX</summary>
            internal JNXMapInfo GetMapInfo()
            {
                return FMapInfo;
            }
            /// <summary> renvoie la structure de type JNXMapMeta du fichier JNX</summary>
            internal JNXMapMeta GetMapMeta()
            {
                return FMapMeta;
            }
            /// <summary> renvoie la ou les structures de type JNXLevelInfo du fichier JNX</summary>
            internal JNXLevelInfo GetLevelInfo(int Niveau)
            {
                if (Niveau >= FMapInfo.Levels)
                    throw new Exception("Le niveau n'existe pas");
                return FLevelsInfo[Niveau];
            }
            /// <summary> renvoie la ou les structures de type JNXLevelMeta du fichier JNX</summary>
            internal JNXLevelMeta GetLevelMeta(int Niveau)
            {
                if (Niveau >= FMapInfo.Levels)
                    throw new Exception("Le niveau n'existe pas");
                return FLevelsMeta[Niveau];
            }
            /// <summary> renvoie le nb de tuiles d'un niveau de détail </summary>
            internal int GetTileCount(int Niveau)
            {
                if (Niveau >= FMapInfo.Levels)
                    throw new Exception("Le niveau n'existe pas");
                return FLevelsInfo[Niveau].TileCount;
            }
            /// <summary> renvoie les informations associées à une tuile d'index : index et du niveau de détail : Niveau</summary>
            internal JNXTileInfo GetTileInfo(int Niveau, int Index)
            {
                if (Niveau >= FMapInfo.Levels)
                    throw new Exception("Le niveau n'existe pas");
                if (Index >= FLevelsInfo[Niveau].TileCount)
                    throw new Exception("La tuile n'existe pas");
                FileBinaire.BaseStream.Position = FLevelsInfo[Niveau].TilesOffset + Index * NbOctetsTileInfo;
                return LireJNXTileInfo(FileBinaire);
            }
            /// <summary> renvoie l'image au format JPEG associée à une tuile d'index : index et du niveau de détail : Niveau</summary>
            internal byte[] GetJpegStream(int Niveau, int index)
            {
                try
                {
                    var TuileInfo = GetTileInfo(Niveau, index);
                    FileBinaire.BaseStream.Position = TuileInfo.PicOffset;
                    var Result = new byte[TuileInfo.PicSize + 2];
                    Result[0] = 0xFF;
                    Result[1] = 0xD8;
                    FileBinaire.Read(Result, 2, TuileInfo.PicSize);
                    return Result;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "CE1F");
                    return null;
                }
            }
            /// <summary> lit toutes les tuiles qui composent un niveau du fichier JNX et les enregistre</summary>
            /// <param name="IndiceNiveau"> Niveau du fichier JNX à lire </param>
            /// <param name="CheminTuile"> répertoire où enregistrer les tuiles </param>
            internal bool LireNiveauJNX(int IndiceNiveau, string CheminTuile)
            {
                int NbCols = int.Parse(FLevelsInfo[IndiceNiveau].Copyright[..3]);
                int X = 0, Y = 0;
                for (int IndexTuile = 0, loopTo = FLevelsInfo[IndiceNiveau].TileCount - 1; IndexTuile <= loopTo; IndexTuile++)
                {
                    LireTuileToImage(IndiceNiveau, IndexTuile).Save($"{CheminTuile}C_{X:D3}{Y:D3}.jpg");
                    X += 1;
                    if (X == NbCols)
                    {
                        X = 0;
                        Y += 1;
                    }
                }
                return true;
            }
            /// <summary> lit une tuile du fichier JNX sous forme d'image .net</summary>
            /// <param name="Niveau"> niveau dans le fichier</param>
            /// <param name="index"> index de la tuile dans le niveau </param>
            internal Image LireTuileToImage(int Niveau, int index)
            {
                return Image.FromStream(new MemoryStream(GetJpegStream(Niveau, index)));
            }
            /// <summary> vérifie la strucutre de début et de fin du fichier jnx</summary>
            internal bool VerifierFormatCarteJNX()
            {
                FileBinaire.BaseStream.Position = 0L;
                if (FileBinaire.ReadByte() != 4)
                    return false; // le format du fichier JNX n'est pas celui attendu
                if (!LireJNXSignature())
                    return false; // la signature du fichier n'est pas celle attendue
                return true;
            }
            #endregion
            #region invisibles
            /// <summary> lit le bloc JNXMapInfo de la structure de l'entête du fichier jnx</summary>
            private void LireMapInfo()
            {
                FMapInfo = LireJNXMapInfo();
                if (FMapInfo.Version != 4)
                {
                    throw new IndexOutOfRangeException("Unsupprted JNX version: " + FMapInfo.Version);
                }
                Offset = new OffsetJNX();
                FLevelsInfo = new JNXLevelInfo[FMapInfo.Levels];
                Offset.LevelInfo = new uint[FMapInfo.Levels];
                FLevelsMeta = new JNXLevelMeta[FMapInfo.Levels];
                Offset.LevelMeta = new uint[FMapInfo.Levels];
            }
            /// <summary> lit le bloc JNXMapMeta de la structure de l'entête du fichier jnx</summary>
            private void LireMapMeta()
            {
                Offset.MapMeta = FileBinaire.BaseStream.Position;
                FMapMeta = LireJNXMapMeta();
            }
            /// <summary> lit le ou les blocs JNXLevelInfo de la structure de l'entête du fichier jnx</summary>
            private void LireLevelsInfo()
            {
                for (int NumNiveau = 0, loopTo = FMapInfo.Levels - 1; NumNiveau <= loopTo; NumNiveau++)
                {
                    Offset.LevelInfo[NumNiveau] = (uint)FileBinaire.BaseStream.Position;
                    FLevelsInfo[NumNiveau] = LireJNXLevelInfo();
                }
            }
            /// <summary> lit le ou les blocs JNXLevelMeta de la structure de l'entête du fichier jnx</summary>
            private void LireLevelsMeta()
            {
                for (int NumNiveau = 0, loopTo = FMapInfo.Levels - 1; NumNiveau <= loopTo; NumNiveau++)
                {
                    Offset.LevelMeta[NumNiveau] = (uint)FileBinaire.BaseStream.Position;
                    FLevelsMeta[NumNiveau] = LireJNXLevelMeta();
                }
            }
            /// <summary> lit et renvoie une structue JNXPoint dans le fichier binaire</summary>
            private JNXPoint LireJNXPoint()
            {
                JNXPoint LireJNXPointRet = new()
                {
                    Latitude = FileBinaire.ReadInt32(),
                    Longitude = FileBinaire.ReadInt32()
                };
                return LireJNXPointRet;
            }
            /// <summary> lire une structue JNXRect dans le fichier binaire</summary>
            /// <param name="FileBinaire">fichier binaire</param>
            /// <remarks>écrit à la position du pointeur de fichier</remarks>
            private JNXRect LireJNXRect()
            {
                JNXRect LireJNXRectRet = new()
                {
                    northEast = LireJNXPoint(),
                    southWest = LireJNXPoint()
                };
                return LireJNXRectRet;
            }
            /// <summary> lit une structue JNXMapInfo dans le fichier binaire</summary>
            private JNXMapInfo LireJNXMapInfo()
            {
                JNXMapInfo LireJNXMapInfoRet = new()
                {
                    Version = FileBinaire.ReadInt32(),
                    DeviceSN = FileBinaire.ReadInt32(),
                    MapBounds = LireJNXRect(),
                    Levels = FileBinaire.ReadInt32(),
                    Expiration = FileBinaire.ReadInt32(),
                    ProductID = FileBinaire.ReadInt32(),
                    CRC = FileBinaire.ReadInt32(),
                    SigVersion = FileBinaire.ReadInt32(),
                    SigOffset = FileBinaire.ReadUInt32(),
                    ZOrder = FileBinaire.ReadInt32()
                };
                return LireJNXMapInfoRet;
            }
            /// <summary> lit dans le fichier binaire et renvoie une structure JNXLevelInfo </summary>
            private JNXLevelInfo LireJNXLevelInfo()
            {
                JNXLevelInfo LireJNXLevelInfoRet = new()
                {
                    TileCount = FileBinaire.ReadInt32(),
                    TilesOffset = FileBinaire.ReadUInt32(),
                    Scale = FileBinaire.ReadInt32(),
                    Unknow = FileBinaire.ReadInt32(),
                    Copyright = LireStringC()
                };
                return LireJNXLevelInfoRet;
            }
            /// <summary> lit dans le fichier binaire et renvoie une structure JNXTileInfo </summary>
            private JNXTileInfo LireJNXTileInfo(BinaryReader FileBinaire)
            {
                JNXTileInfo LireJNXTileInfoRet = new()
                {
                    TileBounds = LireJNXRect(),
                    PicWidth = FileBinaire.ReadInt16(),
                    PicHeight = FileBinaire.ReadInt16(),
                    PicSize = FileBinaire.ReadInt32(),
                    PicOffset = FileBinaire.ReadUInt32()
                };
                return LireJNXTileInfoRet;
            }
            /// <summary> lit dans le fichier binaire et renvoie une structure JNXMapMeta </summary>
            private JNXMapMeta LireJNXMapMeta()
            {
                JNXMapMeta LireJNXMapMetaRet = new()
                {
                    Version = FileBinaire.ReadInt32(),
                    GUID = LireStringC(),
                    ProductName = LireStringC(),
                    Unknow = LireStringC(),
                    ProductID = FileBinaire.ReadInt16(),
                    MapName = LireStringC(),
                    LevelMetaCount = FileBinaire.ReadInt32()
                };
                return LireJNXMapMetaRet;
            }
            /// <summary> lit dans le fichier binaire et renvoie une structure JNXLevelMeta </summary>
            private JNXLevelMeta LireJNXLevelMeta()
            {
                JNXLevelMeta LireJNXLevelMetaRet = new()
                {
                    Name = LireStringC(),
                    Description = LireStringC(),
                    Copyright = LireStringC(),
                    Zoom = FileBinaire.ReadInt32()
                };
                return LireJNXLevelMetaRet;
            }
            /// <summary>lit en fin du fichier JNX 8 octects et vérifie qu'ils correspondent à la signature d'un fichier JNX</summary>
            private bool LireJNXSignature()
            {
                byte[] Signature = Encoding.UTF8.GetBytes(JNX_EOF_SIGNATURE);
                FileBinaire.BaseStream.Position = FileBinaire.BaseStream.Length - Signature.Length;
                for (int Cpt = 0, loopTo = Signature.Length - 1; Cpt <= loopTo; Cpt++)
                {
                    if (FileBinaire.ReadByte() != Signature[Cpt])
                        return false;
                }
                return true;
            }
            /// <summary> lit une chaine de caractère en utf8 dans le flux binaire </summary>
            private string LireStringC()
            {
                var S = new StringBuilder("", 80);
                char C = FileBinaire.ReadChar();
                while (C != NullChar)
                {
                    S.Append(C);
                    C = FileBinaire.ReadChar();
                }
                return S.ToString();
            }
            /// <summary> ouvre le fichier jnx</summary>
            private bool OpenFile(string Path)
            {
                try
                {
                    FileBinaire = new BinaryReader(File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "E051");
                }
                return true;
            }
            #endregion
            #endregion
        }
    }
}