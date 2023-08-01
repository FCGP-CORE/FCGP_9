using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary> données cartographiques associées aux sites Web</summary>
    static class DonneesSiteWeb
    {
        #region Données du TileMatrixSet des sites web (WebMercator et SuisseMobile)
        /// <summary> largeur et hauteur en pixel des tuiles des serveurs carto. </summary>
        internal const int NbPixelsTuile = 256;
        /// <summary> Renvoie la constante Metres pour l'échelle de capture </summary>
        /// <param name="Echelle"> Indice de l'échelle de capture </param>
        internal static double MetresTuile(Echelles Echelle)
        {
            return DonneesCouches[(int)Echelle].PixelsMetre * NbPixelsTuile;
        }
        /// <summary> Renvoie la constante Pixels par Metres pour l'échelle de capture </summary>
        /// <param name="Echelle"> Indice de l'échelle de capture </param>
        internal static double PixelsMetre(Echelles Echelle)
        {
            return DonneesCouches[(int)Echelle].PixelsMetre;
        }
        /// <summary> Renvoie la constante Metres par Pixels pour l'échelle de capture </summary>
        /// <param name="Echelle"> Indice de l'échelle de capture </param>
        internal static double MetrePixels(Echelles Echelle)
        {
            return DonneesCouches[(int)Echelle].MetrePixels;
        }
        /// <summary> Renvoie la constante N° couche du serveur web en fonction de l'indice de l'échelle de capture </summary>
        /// <param name="Echelle"> Indice de l'échelle de capture </param>
        internal static byte EchelleToCouche(Echelles Echelle)
        {
            return DonneesCouches[(int)Echelle].Couche;
        }
        /// <summary> données relatives à un niveau du tile matrixset d'un serveur carto (WebMercator et SuisseMobile) 
        /// sous forme de constantes pour éviter les calculs </summary>
        private readonly static (double MetrePixels, double PixelsMetre, byte Couche)[] DonneesCouches =
            new (double, double, byte)[] {
                (DC / Math.Pow(2.0d, 24.0d), Math.Pow(2.0d, 24.0d) / DC, 17),
                (DC / Math.Pow(2.0d, 23.0d), Math.Pow(2.0d, 23.0d) / DC, 16),
                (1.0d, 1.0d, 25), (2.5d, 1.0d / 2.5d, 22),
                (DC / Math.Pow(2.0d, 22.0d), Math.Pow(2.0d, 22.0d) / DC, 15),
                (5.0d, 1.0d / 5.0d, 21),
                (DC / Math.Pow(2.0d, 21.0d), Math.Pow(2.0d, 21.0d) / DC, 14),
                (10.0d, 1.0d / 10.0d, 20),
                (DC / Math.Pow(2.0d, 20.0d), Math.Pow(2.0d, 20.0d) / DC, 13),
                (20.0d, 1.0d / 20.0d, 19),
                (DC / Math.Pow(2.0d, 19.0d), Math.Pow(2.0d, 19.0d) / DC, 12),
                (50.0d, 1.0d / 50.0d, 18),
                (DC / Math.Pow(2.0d, 18.0d), Math.Pow(2.0d, 18.0d) / DC, 11),
                (100.0d, 1.0d / 100.0d, 17),
                (DC / Math.Pow(2.0d, 17.0d), Math.Pow(2.0d, 17.0d) / DC, 10),
                (250.0d, 1.0d / 250.0d, 16), (500.0d, 1.0d / 500.0d, 15),
                (DC / Math.Pow(2.0d, 16.0d), Math.Pow(2.0d, 16.0d) / DC, 9),
                (DC / Math.Pow(2.0d, 15.0d), Math.Pow(2.0d, 15.0d) / DC, 8),
                (DC / Math.Pow(2.0d, 14.0d), Math.Pow(2.0d, 14.0d) / DC, 7),
                (DC / Math.Pow(2.0d, 13.0d), Math.Pow(2.0d, 13.0d) / DC, 6) };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            // 1/8 000 000 WebMercator
        #endregion
        #region Données cartographiques Sites Web
        /// <summary> constantes des points d'affichage par défaut des sites de capture. </summary>
        internal static PointG CentreSiteWeb(int Indice_Site)
        {
            return DonneesSitesDomsToms[Indice_Site].Centre;
        }
        /// <summary> constantes des origines X et Y des sites de capture. Origine à rajouter au coordonnées pour avoir les coordonnées de la Grille </summary>
        internal static PointD OrigineSiteWeb(int IndiceSite)
        {
            return DonneesSitesDomsToms[IndiceSite].Origine;
        }
        /// <summary> constantes des Datums des sites de capture. </summary>
        internal static Datums DatumSiteWeb(int IndiceSite)
        {
            return DonneesSitesDomsToms[IndiceSite].Datum;
        }
        /// <summary> constantes des points d'affichage par défaut des sites de capture. </summary>
        internal static RectangleD LimiteSiteWeb(int IndiceSite)
        {
            return DonneesSitesDomsToms[IndiceSite].Limites;
        }
        /// <summary> indique si la portée du site carto peut être transposée à un site de portée Européenne </summary>
        /// <param name="SiteCarto"></param>
        internal static bool IsNationalToEurope(SitesCartographiques SiteCarto)
        {
            return Array.IndexOf(_SitesNationalEuropeen, SiteCarto) > -1;
        }
        /// <summary> retourne les sites carto nationaux à portée européenne </summary>
        internal static SitesCartographiques[] SitesNationalEuropeen
        {
            get
            {
                return _SitesNationalEuropeen;
            }
        }
        /// <summary> retourne les sites carto à portée européenne </summary>
        internal static SitesCartographiques[] SitesOSMEurope
        {
            get
            {
                return _SitesOsmEurope;
            }
        }
        /// <summary> tableau des sites qui permette d'appeler un site OSM d'emprise Européene </summary>
        private readonly static SitesCartographiques[] _SitesNationalEuropeen =
            new SitesCartographiques[] { SitesCartographiques.Géofoncier, SitesCartographiques.SuisseMobile, SitesCartographiques.IgnEspagnol };
        /// <summary> tableau des sites OSM d'emprise Européene </summary>
        private readonly static SitesCartographiques[] _SitesOsmEurope =
            new SitesCartographiques[] { SitesCartographiques.CyclOSM, SitesCartographiques.OpenTopoMap };

        /// <summary> données relatives aux sites et DomToms </summary>
        private readonly static (RectangleD Limites, PointG Centre, PointD Origine, Datums Datum)[] DonneesSitesDomsToms =
            new (RectangleD Limites, PointG Centre, PointD Origine, Datums Datum)[] {
                (new RectangleD(-590000.0d, 6674500.0d, 1076000.0d, 5054000.0d), new PointG(9, 33176, 23037), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(475000.0d, 305000.0d, 855000.0d, 70000.0d), new PointG(6, 1800, 1500), new PointD(420000.0d, 350000.0d), Datums.Grille_Suisse_LV03),
                (new RectangleD(6144800.0d, -2374000.0d, 6217200.0d, -2439700.0d), new PointG(5, 685855, 587209), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-6818300.0d, 1676500.0d, -6768200.0d, 1617800.0d), new PointG(6, 173258, 240588), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-6883300.0d, 1865000.0d, -6790500.0d, 1783900.0d), new PointG(6, 172683, 238266), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-7031000.0d, 2053700.0d, -7009000.0d, 2037000.0d), new PointG(4, 681071, 941503), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-6079200.0d, 647900.0d, -5745200.0d, 234900.0d), new PointG(7, 92290, 128021), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(5002700.0d, -1411800.0d, 5043000.0d, -1468900.0d), new PointG(4, 1311529, 1123820), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-7008500.0d, 2036000.0d, -6988500.0d, 2022000.0d), new PointG(3, 1364990, 1885027), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-1200000.0d, 11500000.0d, 3550000.0d, 4120000.0d), new PointG(10, 17133, 11258), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-1200000.0d, 11500000.0d, 3550000.0d, 4120000.0d), new PointG(10, 17133, 11258), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(-2041000.0d, 5440000.0d, 501500.0d, 3195000.0d), new PointG(9, 32110, 24708), new PointD(-DC, DC), Datums.Web_Mercator),
                (new RectangleD(2475000.0d, 1305000.0d, 2855000.0d, 1070000.0d), new PointG(6, 1800, 1500), new PointD(2420000.0d, 1350000.0d), Datums.Grille_Suisse_LV95) };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          // S5
        #endregion
    }
}