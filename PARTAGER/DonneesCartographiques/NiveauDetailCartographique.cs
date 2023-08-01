using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary> définit une echelle d'un système cartographique </summary>
    internal class NiveauDetailCartographique
    {
        #region Functions partagees. renvoie des informations d'échelles associées à un site carto
        /// <summary> retourne un integer qui représente l'indice de la clef de l'échelle de capture dans le tableau des échelles pour le site considéré </summary>
        /// <param name="SiteCarto">indice représentant le système de coordonnées</param>
        /// <param name="ClefEchelle">Clef de l'Echelle de capture telle qu'indiquée dans le fichier Georef</param>
        internal static int EchelleClefToSiteIndiceEchelle(SitesCartographiques SiteCarto, string ClefEchelle)
        {
            // on cherche si le nom de la clé existe dans la collection des clés
            int IndiceCle = Array.IndexOf(EchellesClefs, ClefEchelle);
            Echelles Echelle;
            var IndiceEchelle = default(int);
            if (IndiceCle > -1)
            {
                // on transforme l'indice en echelle
                Echelle = (Echelles)IndiceCle;
                // et on cherche l'indice de l'échelle pour le site considéré
                IndiceEchelle = Array.IndexOf(EchellesSiteClefs[(int)SiteCarto], Echelle);
            }
            return IndiceEchelle;
        }
        internal static int EchelleToSiteIndiceEchelle(SitesCartographiques SiteCarto, Echelles Echelle)
        {
            return Array.IndexOf(EchellesSiteClefs[(int)SiteCarto], Echelle);
        }
        internal static Echelles SiteIndiceEchelleToEchelle(SitesCartographiques SiteCarto, int IndiceEchelle)
        {
            return EchellesSiteClefs[(int)SiteCarto][IndiceEchelle];
        }
        /// <summary> echelles possibles pour une site-domtom </summary>
        internal static string[] EchellesSite(SitesCartographiques Site, DomsToms DomTom)
        {
            int NbEchelles = NbEchellesSite(Site, DomTom);
            var Echs = new string[NbEchelles];
            for (int Cpt = 0, loopTo = NbEchelles - 1; Cpt <= loopTo; Cpt++)
                Echs[Cpt] = EchellesLibelles[(int)EchellesSiteClefs[(int)Site][Cpt]];
            return Echs;
        }
        internal static int NbEchellesSite(SitesCartographiques Site, DomsToms DomTom)
        {
            int NbEchelles = Site == SitesCartographiques.DomTom ? NbEchellesDomsToms[(int)DomTom] : EchellesSiteClefs[(int)Site].Length;
            return NbEchelles;
        }
        /// <summary> Facteurs ORUX possibles pour un site-domtom dans l'ordre croissant des échelles </summary>
        internal static int[] FacteursORUXSite(SitesCartographiques Site, DomsToms DomTom)
        {
            int NbEchelles = NbEchellesSite(Site, DomTom);
            var ORUXs = new int[NbEchelles];
            for (int Cpt = 0, loopTo = NbEchelles - 1; Cpt <= loopTo; Cpt++)
                ORUXs[Cpt] = NiveauxAffichagesORUX[(int)EchellesSiteClefs[(int)Site][Cpt]];
            return ORUXs;
        }
        /// <summary> Facteur ORUX pour un site-domtom-echelle</summary>
        internal static int FacteurORUX_Defaut(SitesCartographiques Site, int IndiceEchelle)
        {
            return NiveauxAffichagesORUX[(int)EchellesSiteClefs[(int)Site][IndiceEchelle]];
        }
        /// <summary> Facteurs JNX possibles pour un site-domtom dans l'ordre croissant des échelles </summary>
        internal static double[] FacteursJNXSite(SitesCartographiques Site, DomsToms DomTom)
        {
            int NbEchelles = NbEchellesSite(Site, DomTom);
            var JNXs = new double[NbEchelles];
            for (int Cpt = 0, loopTo = NbEchelles - 1; Cpt <= loopTo; Cpt++)
                JNXs[Cpt] = NiveauxAffichagesJNX[(int)EchellesSiteClefs[(int)Site][Cpt]];
            return JNXs;
        }
        /// <summary> Facteur JNX pour un site-domtom-echelle</summary>
        internal static double FacteurJNX_Defaut(SitesCartographiques Site, int IndiceEchelle)
        {
            return NiveauxAffichagesJNX[(int)EchellesSiteClefs[(int)Site][IndiceEchelle]];
        }
        /// <summary> Couches possibles pour un site-domtom</summary>
        internal static byte[] CouchesSite(SitesCartographiques Site, DomsToms DomTom)
        {
            int NbEchelles = NbEchellesSite(Site, DomTom);
            var Couches = new byte[NbEchelles];
            for (int Cpt = 0, loopTo = NbEchelles - 1; Cpt <= loopTo; Cpt++)
                Couches[Cpt] = EchelleToCouche(EchellesSiteClefs[(int)Site][Cpt]);
            return Couches;
        }
        /// <summary> couche pour un site-echelle </summary>
        internal static byte Couche(SitesCartographiques Site, int IndiceEchelle)
        {
            return EchelleToCouche(EchellesSiteClefs[(int)Site][IndiceEchelle]);
        }
        #endregion
        #region Constantes privées concernant les Echelles
        /// <summary> nb d'échelles par DomTom. Le site DT gère l'ensemble des échelles des DomsToms, cela permet de renvoyer
        /// uniquement celles concernées par un DomTom particulier </summary>
        private static readonly int[] NbEchellesDomsToms = new int[] { 7, 7, 7, 5, 9, 6, 5 };
        /// <summary> liste des échelles par site. Permet de faire la relation entre les indices d'une liste déroulante des échelles d'un site
        /// et le N° de couches des serveurs </summary>
        private static readonly Echelles[][] EchellesSiteClefs = new[] {
                new Echelles[] { Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                                 Echelles._2000, Echelles._4000, Echelles._8000 },
                new Echelles[] { Echelles._005, Echelles._010, Echelles._025, Echelles._050, Echelles._100, Echelles._200, Echelles._400, Echelles._600, Echelles._800 },
                new Echelles[] { Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000 },
                new Echelles[] { Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                                 Echelles._2000, Echelles._4000, Echelles._8000 },
                new Echelles[] { Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                                 Echelles._2000, Echelles._4000, Echelles._8000 },
                new Echelles[] { Echelles._004, Echelles._007, Echelles._015, Echelles._030, Echelles._060, Echelles._120, Echelles._250, Echelles._500, Echelles._1000,
                                 Echelles._2000, Echelles._4000, Echelles._8000 } };
        /// <summary> niveau d'affichage avec les GPS Garmin en fonction de l'échelle du site </summary>
        private static readonly double[] NiveauxAffichagesJNX = new double[] { 4.0d, 5.0d, 5.5d, 6.0d, 6.0d, 7.0d, 7.0d, 8.0d, 8.0d, 9.0d, 9.0d, 10.0d, 10.0d,
                                                                               11.0d, 11.0d, 12.0d, 13.0d, 12.0d, 13.0d, 14.0d, 15.0d };
        /// <summary> niveau d'affichage avec l'application Orux en fonction de l'échelle du site </summary>
        private static readonly int[] NiveauxAffichagesORUX = new int[] { 18, 17, 17, 16, 16, 15, 15, 14, 14, 13, 13, 12, 12, 11, 11, 10, 9, 10, 9, 8, 7 };
        /// <summary>stock des pas de grille en fonction des échelles sous forme de texte</summary>
        private static readonly string[] PasGrillesTextes = new string[] { "500 m", "1 Km", "1 Km", "1 Km", "1 Km", "1 Km", "2 Km", "5 Km", "5 Km", "10 Km", "10 Km", "50 Km",
                                                                           "25 Km", "", "50 Km", "", "", "", "", "", "" };
        /// <summary>stock des pas de grille en fonction des échelles sous forme de nombre qui exprime des mètres</summary>
        private static readonly int[] PasGrillesNumerics = new int[] { 500, 1000, 1000, 1000, 1000, 1000, 2000, 5000, 5000, 10000, 10000, 50000, 25000, 0, 50000, 0, 0, 0, 0, 0, 0 };
        /// <summary>stock des échelles d'impression en fonction des échelles gérées par site sous forme de texte</summary>
        private static readonly string[] EchellesImpressions = new string[] { "1/5000", "1/10000", "1/5000", "1/10000", "1/25000", "1/25000", "1/50000", "1/50000", "1/100000",
                                                                              "1/100000", "1/200000", "1/200000", "1/400000", "", "1/800000", "", "", "", "", "", "" };
        /// <summary>stock échelles d'impression en fonction des échelles gérées par site sous forme de clef texte</summary>
        private static readonly string[] EchellesImpressionsClefs = new string[] { "005", "010", "005", "010", "025", "025", "050", "050", "100", "100", "200", "200", "400", "",
                                                                                   "800", "", "", "", "", "", "" };
        /// <summary>stock pour les sites avec grilles incorporées la qualité souhaitée pour l'interpolation en fonction des échelles gérées par le site</summary>
        private static readonly int[] QualitesInterpolsGrilles = new[] { 0, 0, 1500, 2500, 0, 5000, 0, 10000, 0, 25000, 0, 50000, 0, 75000, 0, 100000, 150000, 0, 0, 0, 0 };
        #endregion
        #region Information propre à une echelle
        /// <summary> constructeur pour la capture d'une carte ou la création d'une carte existante(fichier georef)
        /// permet d'initialiser une echelle soit à partir de la clef (fichier géoref), soit du libellé (capture) </summary>
        /// <param name="SiteCarto">site associé à l'échelle</param>
        /// <param name="ClefEchelle">libellé de l'échelle</param>
        internal NiveauDetailCartographique(SitesCartographiques SiteCarto, string ClefEchelle)
        {
            int IndiceCle = Array.IndexOf(EchellesClefs, ClefEchelle);
            Echelle = (Echelles)IndiceCle;
            if (Echelle > Echelles._000)
            {
                if (Array.IndexOf(EchellesSiteClefs[(int)SiteCarto], Echelle) > -1)
                {
                    IsOk = true;
                }
                else
                {
                    Echelle = Echelles._000;
                }
            }
        }
        /// <summary> constructeur spécifique au téléchargement des cartes</summary>
        /// <param name="SiteCarto">site associé à l'échelle</param>
        /// <param name="Echelle">libellé de l'échelle</param>
        internal NiveauDetailCartographique(SitesCartographiques SiteCarto, Echelles Echelle)
        {
            int IndiceEchelle = Array.IndexOf(EchellesSiteClefs[(int)SiteCarto], Echelle);
            if (IndiceEchelle > -1)
            {
                this.Echelle = Echelle;
                IsOk = true;
            }
            else
            {
                this.Echelle = Echelles._000;
            }
        }
        /// <summary>indice en base zéro de l'échelle pour le site carto</summary>
        internal Echelles Echelle { get; private set; }
        /// <summary>indique si l'objet a été crée correctement</summary>
        internal bool IsOk { get; private set; }
        /// <summary> renvoi le niveau d'affichage pour un fichier tuile ORUX </summary>
        internal int NiveauAffichageORUX
        {
            get
            {
                return IsOk ? NiveauxAffichagesORUX[(int)Echelle] : 0;
            }
        }
        /// <summary> renvoi le niveau d'affichage pour un fichier tuile JNX </summary>
        internal double NiveauAffichageJNX
        {
            get
            {
                return IsOk ? NiveauxAffichagesJNX[(int)Echelle] : 0.0d;
            }
        }
        /// <summary> Echelle de capture des cartes </summary>
        internal string Libelle
        {
            get
            {
                return IsOk ? EchellesLibelles[(int)Echelle] : "Aucun";
            }
        }
        /// <summary>clef de l'échelle de capture des cartes. info présente dans le fichier GeoRef. </summary>
        internal string Clef
        {
            get
            {
                return IsOk ? EchellesClefs[(int)Echelle] : "";
            }
        }
        /// <summary> Pour les calculs de liés à la grille </summary>
        internal int QualiteInterpolGrille
        {
            get
            {
                return IsOk ? QualitesInterpolsGrilles[(int)Echelle] : 0;
            }
        }
        /// <summary> Echelle d'impression conseillée. Tableau escaliers IndiceSytème et IndiceEchelleCapture</summary>
        internal string Impression
        {
            get
            {
                return IsOk ? EchellesImpressions[(int)Echelle] : "";
            }
        }
        /// <summary> Clef de l'échelle d'impression conseillée. info présente dans le fichier GeoRef </summary>
        internal string ImpressionClef
        {
            get
            {
                string Ech = "";
                if (IsOk)
                {
                    Ech = EchellesImpressionsClefs[(int)Echelle];
                    // nécessaire pour pouvoir retrouver les dimensions en mètre de la carte à travers les DPI
                    if (string.IsNullOrEmpty(Ech))
                        Ech = "1000";
                }
                return Ech;
            }
        }
        /// <summary> pour la génération des fichiers OziExploreur </summary>
        internal string PasGrilleTexte
        {
            get
            {
                return IsOk ? PasGrillesTextes[(int)Echelle] : "";
            }
        }
        /// <summary> Pour les calculs de liés à la grille </summary>
        internal int PasGrilleNumeric
        {
            get
            {
                return IsOk ? PasGrillesNumerics[(int)Echelle] : 0;
            }
        }
        #endregion
    }
}