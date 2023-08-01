using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.ProjectionCartographique;

namespace FCGP
{
    /// <summary> Définit un système cartographique associé à une carte.</summary>
    internal class SystemeCartographique
    {
        #region Fonctions partagées
        /// <summary> renvoie un index unique qui prend en compte à la fois le site et le domtom </summary>
        /// <param name="Site"> site cartographique </param>
        /// <param name="Domtom"> domtom associé au site cartographique </param>
        internal static int SiteDomTomToIndex(SitesCartographiques Site, DomsToms Domtom)
        {
            if (Site < SitesCartographiques.DomTom)          // GF, SM  : 0, 1
            {
                return (int)Site;
            }
            else if (Site > SitesCartographiques.DomTom)      // CY, OT, ES : 9, 10, 11
            {
                return (int)Site + DomsTomsLibelles.Length - 1;
            }
            else
            {
                return (int)Site + (int)Domtom;
            }                            // les doms-toms de 2 à 8
        }
        /// <summary> transforme un index de liste déroulante des sites en un site carto et domtom correspondant au tableau LibellesSitesDomTom </summary>
        /// <param name="IndexListe"> Index de la liste </param>
        internal static (SitesCartographiques Site, DomsToms DomTom) IndexToSiteDomTom(int IndexListe)
        {
            SitesCartographiques Site;
            DomsToms DomTom;
            if (IndexListe < (int)SitesCartographiques.DomTom) // -1, 0, 1 --> -1, 0, 1 
            {
                Site = (SitesCartographiques)IndexListe;
                DomTom = DomsToms.aucun;
            }
            else if (IndexListe >= (int)SitesCartographiques.DomTom + DomsTomsLibelles.Length) // 9, 10, 11 --> 3, 4, 5
            {
                Site = (SitesCartographiques)(IndexListe - DomsTomsLibelles.Length + 1);
                DomTom = DomsToms.aucun;
            }
            else
            {
                Site = SitesCartographiques.DomTom;
                DomTom = (DomsToms)(IndexListe - 2);
            }
            return (Site, DomTom);
        }
        /// <summary> Retourne l'indice du domtom inclus dans la cle du systèmecarto passé en paramètre </summary>
        /// <param name="ClefSystemecarto">Site cartographique lu au niveau du fichier Géoref Ex: DomTom-reunion</param>
        internal static DomsToms TrouverDomTomCle(string ClefSystemecarto)
        {
            int Pos = ClefSystemecarto.IndexOf('-');
            // si il y a un '-' on peut supposer qu'il s'agit de la forme CleSite-ClefDomTom
            // sinon il s'agit de la forme ClefDomTom uniquement
            if (Pos > -1)
                ClefSystemecarto = ClefSystemecarto[(Pos + 1)..];
            return (DomsToms)Array.IndexOf(DomsTomsClefs, ClefSystemecarto);
        }
        /// <summary> Retourne l'indice du site carto inclus dans la clé du systèmecarto passé en paramètre </summary>
        /// <param name="CleSystemeCarto">site cartographique lu au niveau du fichier Géoref</param>
        internal static SitesCartographiques TrouverSiteCartographiqueCle(string CleSystemeCarto)
        {
            // pour le site Domtom on élimine le nom du Domtom
            if (CleSystemeCarto.Length > 2)
                CleSystemeCarto = CleSystemeCarto[..2];
            return (SitesCartographiques)Array.IndexOf(SitesCartoClefs, CleSystemeCarto);
        }
        /// <summary> indique si le site carto du système carto possède une grille incorporée </summary>
        /// <param name="SiteCarto"></param>
        internal static bool GrilleSiteExiste(SitesCartographiques SiteCarto)
        {
            return GrillesExistes[(int)SiteCarto];
        }
        /// <summary> renvoie la cle associé au sitecarto passé en paramètre </summary>
        /// <param name="SiteCarto"></param>
        internal static string SiteCartoClef(SitesCartographiques SiteCarto)
        {
            return SitesCartoClefs[(int)SiteCarto];
        }
        /// <summary> renvoie la cle associé au DomTom passé en paramètre </summary>
        /// <param name="DomTom"></param>
        internal static string DomTomClef(DomsToms DomTom)
        {
            return DomsTomsClefs[(int)DomTom];
        }
        /// <summary> renvoie le numéro de zone et l'hémisphère de la projection utm pour le domtom passé en paramètre </summary>
        /// <param name="DomTom"></param>
        internal static string ZoneUtmDomTom(DomsToms DomTom)
        {
            return ZonesUtmDomTom[(int)DomTom];
        }
        #endregion
        #region Constantes privées
        /// <summary>indique pour les différents Domstoms du site domTom la zone UTM principale</summary>
        private static readonly string[] ZonesUtmDomTom = new string[] { "S40", "N20", "N20", "N20", "N22", "S38", "N20" };
        /// <summary>indique le nom des grilles à faire apparaitre sur les références</summary>
        private static readonly string[] GrillesNoms = new string[] { "UTM WGS84", "SUISSE LV03", "UTM WGS84", "UTM WGS84", "UTM WGS84", "UTM WGS84" };
        /// <summary>indique si les cartes du site ont déja la grille affichée</summary>
        private static readonly bool[] GrillesExistes = new bool[] { false, true, false, false, false, false }; // GF,SM,DT,CY,OP,ES
        /// <summary>Pour adapter la taille du texte dans les références de grille</summary>
        private static readonly float[] MaxsDPIsImpression = new float[] { 340.0f, 254.0f, 340.0f, 340.0f, 340.0f, 340.0f };
        #endregion
        #region Fonctions et propriétés visibles
        /// <summary> constructeur pour une carte réalisée à partir de captures d'écran ou de téléchargement de tuiles
        /// sur un serveur, sans interpolation (Fichier Georef avec Reatail à NON) </summary>
        /// <param name="LibelleSystemeCarto"> libellé du système cartographique de la carte </param>
        /// <param name="ClefEchelle"> cle de l'échelle de la carte </param>
        /// <param name="LibelleProjection"> libellé de la projection des coordonnées de capture de la carte </param>
        /// <param name="H_Utm"> si la projection est une projection UTM, hémisphère de la projection </param>
        /// <param name="Z_Utm"> si la projection est une projection UTM, numéro de zone de la projection </param>
        internal SystemeCartographique(string LibelleSystemeCarto, string ClefEchelle, string LibelleProjection, int Z_Utm, char H_Utm)
        {
            IsOk = false;
            SiteCarto = TrouverSiteCartographiqueLibelle(LibelleSystemeCarto);
            // il ne s'agit pas d'un site référencé
            if (SiteCarto == SitesCartographiques.Aucun)
                return;
            DomTom = TrouverDomTomLibelle(LibelleSystemeCarto);
            IndiceSiteCarto = SiteDomTomToIndex(SiteCarto, DomTom);
            // le site et est le site de capture domtom mais il n'y a pas le domtom d'indiqué
            if (SiteCarto == SitesCartographiques.DomTom & DomTom == DomsToms.aucun)
                return;
            IsInterpol = false;
            Niveau = new NiveauDetailCartographique(SiteCarto, ClefEchelle);
            if (!Niveau.IsOk)
                return;
            Projection = new ProjectionCartographique(SiteCarto, LibelleProjection);
            if (!Projection.IsOk)
                return;
            if (DatumPrincipal(SiteCarto) == Datums.WGS84)
            {
                CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD;
            }
            else
            {
                CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille;
            }
            IsCapture = true;
            ZoneUtmReferencement = Z_Utm;
            HemisphereUtmReferencement = H_Utm;
            // dans le cas du site de capture DOMTOM on met les valeurs par defaut (ancien fichier georef)
            if (DomTom > DomsToms.aucun & (Z_Utm == 0 | HemisphereUtmReferencement == default(char)))
            {
                HemisphereUtmReferencement = ZonesUtmDomTom[(int)DomTom][0];
                ZoneUtmReferencement = int.Parse(ZonesUtmDomTom[(int)DomTom][1..]);
            }
            IsOk = true;
        }
        /// <summary> constructeur pour les cartes issues d'un téléchargement de tuiles sur serveur carto. </summary>
        /// <param name="SiteCarto"> site de téléchargement </param>
        /// <param name="Domtom"> domtom de téléchargement </param>
        /// <param name="Echelle"> echelle de téléchargement </param>
        /// <param name="DatumProjection"> libellé de la projection du serveur </param>
        internal SystemeCartographique(int IndiceSite, Echelles Echelle, Datums DatumProjection)
        {
            IsOk = false;
            IndiceSiteCarto = IndiceSite;
            var SiteDomTom = IndexToSiteDomTom(IndiceSiteCarto);
            SiteCarto = SiteDomTom.Site;
            IsInterpol = false;
            DomTom = SiteDomTom.DomTom;
            IndiceSiteCarto = SiteDomTomToIndex(SiteCarto, DomTom);
            Niveau = new NiveauDetailCartographique(SiteCarto, Echelle);
            if (!Niveau.IsOk)
                return;
            Projection = new ProjectionCartographique(SiteCarto, DatumProjection);
            if (!Projection.IsOk)
                return;
            if (DatumPrincipal(SiteCarto) == Datums.WGS84)
            {
                CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD;
            }
            else
            {
                CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille;
            }
            IsCapture = true;
            // dans le cas du site de capture DOMTOM on met les valeurs par defaut (ancien fichier georef)
            if (SiteCarto == SitesCartographiques.DomTom & (ZoneUtmReferencement == 0 || HemisphereUtmReferencement == default(char)))
            {
                HemisphereUtmReferencement = ZonesUtmDomTom[(int)DomTom][0];
                ZoneUtmReferencement = int.Parse(ZonesUtmDomTom[(int)DomTom][1..]);
            }
            IsOk = true;
        }
        /// <summary> constructeur pour un fichier georef issu d'une carte réalisée à partir de captures d'écran ou téléchargement de tuiles 
        /// sur 1 serveur avec interpolation (Fichier Georef avec Reatail à OUI) ou à partir d'un scan avec ou sans interpolation </summary>
        /// <param name="RegionUniteCarte"> exprimée en mètres ou en DD. permet le calcul du rapport unité/pixel </param>
        /// <param name="SurfacePixel"> exprimée en pixel. permet le calcul du rapport unité/pixel </param>
        /// <param name="ClefSiteCarto"> cle du site carto </param>
        /// <param name="ClefEchelle"> cle de l'échelle de capture ou de scan </param>
        /// <param name="LibelleProjection"> libelle de la projection des coordonnées </param>
        /// <param name="H_Utm"> si la projection est une projection UTM, hémisphère de la projection </param>
        /// <param name="Z_Utm"> si la projection est une projection UTM, numéro de zone de la projection </param>
        internal SystemeCartographique(RectangleD RegionUniteCarte, Size SurfacePixel, string ClefSiteCarto, string ClefEchelle, string LibelleProjection,
                                            int Z_Utm, char H_Utm)
        {
            do
            {
                try
                {
                    SiteCarto = TrouverSiteCartographiqueCle(ClefSiteCarto);
                    DomTom = TrouverDomTomCle(ClefSiteCarto);
                    IndiceSiteCarto = SiteDomTomToIndex(SiteCarto, DomTom);
                    Projection = new ProjectionCartographique(SiteCarto, LibelleProjection);
                    if (!Projection.IsOk)
                        break;
                    if (DatumPrincipal(SiteCarto) == Datums.WGS84)
                    {
                        CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD;
                    }
                    else
                    {
                        CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille;
                    }
                    Niveau = new NiveauDetailCartographique(SiteCarto, ClefEchelle);
                    if (!Niveau.IsOk)
                        break;
                    IsCapture = false;
                    IsInterpol = true;
                    RegionPixel = SurfacePixel;
                    RegionUnite = RegionUniteCarte.Taille;
                    EchelleLongitude = RegionPixel.Width / RegionUnite.Largeur;
                    EchelleLatitude = RegionPixel.Height / RegionUnite.Hauteur;
                    EchelleX = RegionUnite.Largeur / RegionPixel.Width;
                    EchelleY = RegionUnite.Hauteur / RegionPixel.Height;
                    PT0_Unité = RegionUniteCarte.Pt0;
                    ZoneUtmReferencement = Z_Utm;
                    HemisphereUtmReferencement = H_Utm;
                    IsOk = true;
                }
                catch (Exception Ex)
                {
                    AfficheInformation.AfficherErreur(Ex, "7BCD");
                    IsOk = false;
                }
            }
            while (false);
        }
        /// <summary> transforme un point exprimé en pixels du monde virtuel en coordonnée de géororeferencement du système </summary>
        internal PointD ConvertirCoordonneesVirtuellesToReelles(Point PointCoordonnees)
        {
            if (IsCapture)
            {
                if (CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
                {
                    return PointPixelToPointGrille(PointCoordonnees, SiteCarto, Niveau.Echelle);
                }
                else
                {
                    return PointPixelsToPointDD(PointCoordonnees, SiteCarto, Niveau.Echelle);
                }
            }
            else
            {
                return PixelsToCoordonnees_Autre(PointCoordonnees);
            }
        }
        /// <summary> tranforme un point de coordonnées exprimé en coordonnées de géoréférencement du système cartographique en pixels du monde virtuel </summary>
        /// <param name="PointCoordonnees"></param>
        internal Point ConvertirCoordonneesReellesToVirtuelles(PointD PointCoordonnees)
        {
            if (IsCapture) // ce n'est pas une carte interpolée
            {
                if (CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
                {
                    return PointGrilleToPointPixel(PointCoordonnees, SiteCarto, Niveau.Echelle);
                }
                else
                {
                    return PointDDToPointPixels(PointCoordonnees, SiteCarto, Niveau.Echelle);
                }
            }
            else
            {
                return CoordonneesToPixels_Autre(PointCoordonnees);
            }
        }
        /// Pour adapter la taille du texte dans les références de grille et l'épaisseur des traits de la grille
        internal bool SystemDoubleGrille
        {
            get
            {
                bool Ret = false;
                int Indice = EchelleToSiteIndiceEchelle(SiteCarto, Niveau.Echelle);
                switch (SiteCarto)
                {
                    case SitesCartographiques.SuisseMobile:
                        {
                            if (Indice == 0)
                                Ret = true;
                            break;
                        }

                    default:
                        {
                            if (Indice == 1)
                                Ret = true;
                            break;
                        }
                }
                return Ret;
            }
        }
        /// <summary> Numéro de zone UTM qui a servi au référencement (coordonnéescapturées) </summary>
        internal int ZoneUtmReferencement { get; set; }
        /// <summary> Hemisphère de zone UTM qui a servi au référencement (coordonnéescapturées) </summary>
        internal char HemisphereUtmReferencement { get; set; }
        /// <summary> flag indiquant si le système fait référence à une carte qui a été interpolée </summary>
        internal bool IsInterpol { get; private set; }
        /// <summary> flag indiquant si le système fait référence à une carte qui a été interpolée </summary>
        internal bool IsOk { get; private set; }
        /// <summary> le Domtom des coordonnées de la carte si systemeCoordonnées est DomTom par exemple réunion </summary>
        internal string LibelleDomtom
        {
            get
            {
                if (DomTom == DomsToms.aucun)
                {
                    return "metropole";
                }
                else
                {
                    return DomsTomsLibelles[(int)DomTom];
                }
            }
        }
        /// <summary>le Domtom des coordonnées de la carte si systemeCoordonnées est DomTom par exemple réunion </summary>
        internal string LibelleSiteCarto
        {
            get
            {
                return SitesCartoLibelles[(int)SiteCarto];
            }
        }
        /// <summary> Quelles sont les coordonnées du fichier géoref qui doivent servir pour le géoréférencement
        /// "Capture" indique que l'on doit se servir des coordonnées capturées
        /// "DD" indique que l'on doit prendre la longitude/latitude en DD présente dans le fichier Georef </summary>
        internal CoordonneesGeoreferencements CoordonneesGeoreferencement { get; private set; }
        /// <summary>en sortie FCGP peut tracer une grille sur la carte et indiquer les coordonnées de celle-ci. il s'agit du nom de la grille </summary>
        internal string GrilleNom
        {
            get
            {
                return GrillesNoms[(int)SiteCarto];
            }
        }
        /// <summary> indique si la grille de sortie est déjà présente sur la carte capturée, si oui il n'y a pas besoin de la retracer </summary>
        internal bool GrilleExiste
        {
            get
            {
                return GrillesExistes[(int)SiteCarto];
            }
        }
        /// <summary> indique si la grille de sortie est présente sur la carte capturée à l'échelle du système cartographique </summary>
        internal bool AfficherGrilleIsOK
        {
            get
            {
                return Niveau.PasGrilleNumeric > 0;
            }
        }
        /// <summary>Pour adapter la taille du texte dans les références de grille</summary>
        internal float MaxDPIImpression
        {
            get
            {
                return MaxsDPIsImpression[(int)SiteCarto];
            }
        }
        /// <summary>Clef du système cartographique sous la forme SS-EEE si GF ou SM ou SS-DDD-EEE si DT </summary>
        internal string ClefEchelle
        {
            get
            {
                return Clef + "-" + Niveau.Clef;
            }
        }
        /// <summary>Clef du système géographique sous la forme SS si GF ou SM ou SS-DDD si DT </summary>
        internal string Clef
        {
            get
            {
                string _Clef;
                if (IsCapture && !IsInterpol)
                {
                    _Clef = SiteCartoClef(SiteCarto);
                    if (SiteCarto == SitesCartographiques.DomTom)
                        _Clef += "-" + DomTomClef(DomTom);
                }
                else
                {
                    _Clef = "Non Géré";
                }
                return _Clef;
            }
        }
        internal int IndiceEchelle
        {
            get
            {
                return EchelleToSiteIndiceEchelle(SiteCarto, Niveau.Echelle);
            }
        }
        /// <summary>indice du système (GéoFoncier, GéoPortail, Suisse Mobile)</summary>
        internal SitesCartographiques SiteCarto { get; private set; }
        /// <summary>indice du DomTom de la carte ou du site DomTom</summary>
        internal DomsToms DomTom { get; private set; }
        /// <summary>indice unique site carto et du DomTom</summary>
        internal int IndiceSiteCarto { get; private set; }
        /// <summary>indice de l'échelle de capture de la carte</summary>
        internal NiveauDetailCartographique Niveau { get; private set; }
        /// <summary>indice du type de coordonnées de la carte qui est à l'origine de la création de l'échelle</summary>
        internal ProjectionCartographique Projection { get; private set; }
        /// <summary>flag indiquant si le système provient d'une carte capturée; si false il s'agit d'une carte interpolée ou d'une carte issue de fond de plan papier</summary>
        internal bool IsCapture { get; private set; }
        #endregion
        #region Fonctions et propriétés cachées
        /// <summary> Retourne l'indice du domtom inclus dans le libellé du systèmecarto passé en paramètre </summary>
        /// <param name="LibelleSystemecarto">Site cartographique lu au niveau du fichier Géoref Ex: DomTom-reunion</param>
        private static DomsToms TrouverDomTomLibelle(string LibelleSystemecarto)
        {
            int Pos = LibelleSystemecarto.IndexOf('-');
            if (Pos == -1) // cela élimine Géofoncier et suisse mobile
            {
                return DomsToms.aucun;
            }
            else
            {
                string SiteDomTom = LibelleSystemecarto[(Pos + 1)..];
                return (DomsToms)Array.IndexOf(DomsTomsLibelles, SiteDomTom);
            }
        }
        /// <summary> Retourne l'indice du site carto inclus dans le libellé du systèmecarto passé en paramètre </summary>
        /// <param name="LibelleSystemeCarto">site cartographique lu au niveau du fichier Géoref</param>
        private static SitesCartographiques TrouverSiteCartographiqueLibelle(string LibelleSystemeCarto)
        {
            // pour suisse mobile, 2 écritures cohabitent 'Suisse Mobile et SuisseMobile' et aussi pour Géofoncier, 2 écritures cohabitent 'GéoFoncier et géofoncier'
            string SystemeCarto = LibelleSystemeCarto.Replace(" ", null).Replace('F', 'f');
            // pour le site Domtom on élimine le nom du Domtom
            int Pos = SystemeCarto.IndexOf('-');
            if (Pos > -1)
                SystemeCarto = SystemeCarto[..Pos];
            return (SitesCartographiques)Array.IndexOf(SitesCartoLibelles, SystemeCarto);
        }
        /// <summary> fonction de conversion des unités en pixel de la carte associée à un système différent d'une capture </summary>
        private Point CoordonneesToPixels_Autre(PointD PointCoordonnees)
        {
            Point CoordonneesToPixels_AutreRet = default;
            CoordonneesToPixels_AutreRet.X = (int)Math.Round(EchelleLongitude * (PointCoordonnees.X - PT0_Unité.X));
            CoordonneesToPixels_AutreRet.Y = (int)Math.Round(EchelleLatitude * (PointCoordonnees.Y - PT0_Unité.Y));
            return CoordonneesToPixels_AutreRet;
        }
        /// <summary> fonction de conversion des pixel en unités de la carte associée à un système différent d'une capture </summary>
        private PointD PixelsToCoordonnees_Autre(Point PointPixel)
        {
            PointD PixelsToCoordonnees_AutreRet = default;
            PixelsToCoordonnees_AutreRet.X = EchelleX * PointPixel.X + PT0_Unité.X;
            PixelsToCoordonnees_AutreRet.Y = EchelleY * PointPixel.Y + PT0_Unité.Y;
            return PixelsToCoordonnees_AutreRet;
        }
        /// <summary> dimensions en pixels de la carte associée à un système différent d'une capture </summary>
        private readonly Size RegionPixel;
        /// <summary>point d'origine en unité (DD ou M) de la carte associée à un système différent d'une capture </summary>
        private readonly PointD PT0_Unité;
        /// <summary>dimensions en unité (DD ou M) de la carte associée à un système différent d'une capture </summary>
        private readonly SizeD RegionUnite;
        /// <summary> afin d'éviter des calculs trop longs lors des conversions de coordonnées de systèmes interpolés </summary>
        private readonly double EchelleLongitude, EchelleLatitude, EchelleX, EchelleY;
        #endregion
    }
}