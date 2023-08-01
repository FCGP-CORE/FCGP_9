using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Coordonnees.ProjectionCartesienne;
using static FCGP.Coordonnees.ProjectionLambert_2;
using static FCGP.Coordonnees.ProjectionLongitudeLatitude;
using static FCGP.Coordonnees.ProjectionMercatorUtm;
using static FCGP.Coordonnees.ProjectionSuisse;
using static FCGP.Coordonnees.ProjectionWebMercator;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary> contient toutes les fonctions et procédures nécessaires pour passer d'un système de projection de coordonnées à un autre parmi ceux listés dans les datums</summary>
    internal static class ConvertirCoordonnees
    {
        #region Constantes
        internal const double PI = Math.PI;
        internal const double Pi_2 = PI / 2.0d;
        internal const double Pi_4 = PI / 4.0d;
        internal const double TwoPI = 2.0d * PI;
        internal const double Rad_Deg = 180.0d / PI;
        internal const double Deg_Rad = PI / 180.0d;
        internal const double Deg_Rad_2 = Deg_Rad / 2.0d;
        internal const double RayonWGS84 = 6378137.0d; // a pour l'ellipsoide WGS84
        internal const double F_WGS84 = 1.0d / 298.257223563d; // 1/aplatissement pour l'ellipsoide WGS84
        internal const double E2_WGS84 = 2.0d * F_WGS84 - F_WGS84 * F_WGS84; // e2 pour l'ellipsoide WGS84
        internal const double DC = PI * RayonWGS84;       // constante de base pour la projection WebMercator
        #endregion
        #region fonctions généralistes de datum à datum
        /// <summary> convertit une coordonnée de départ exprimée en X , Y ou en LL en coordonnées d'arrivée en LL WGS84 exprimée en DD </summary>
        /// <param name="DatumDepart"> datum du point de coordonnées à convertir </param>
        /// <param name="PointDepart"> point à convertir </param>
        internal static PointD ConvertProjectionToWGS84(Datums DatumDepart, PointProjection PointDepart)
        {
            try
            {
                var LatLon = new PointD(0d, 0d); // afin d'avoir le flag empty à faux
                switch (PARAM_DATUMS.ParametresDatums[(int)DatumDepart].Projection)
                {
                    case Projections.Lambert:
                        {
                            LatLon = ConvertLambertToLL(PointDepart.Coordonnees, DatumDepart, false);
                            break;
                        }
                    case Projections.UTM:
                        {
                            if (PointDepart.Zone == 0 || PointDepart.Hemisphere == default(char))
                            {
                                throw new Exception("Paramètres projection UTM obligatoire");
                            }
                            LatLon = ConvertUtmToLL(DatumDepart, PointDepart, false);
                            break;
                        }
                    case Projections.Suisse:
                        {
                            if (DatumDepart == Datums.Grille_Suisse_LV95)
                            {
                                LV95ToLV03(ref PointDepart.Coordonnees);
                            }
                            LatLon = ConvertLV03ToWGS84(PointDepart.Coordonnees, false);
                            break;
                        }
                    case Projections.WMercator:
                        {
                            LatLon = ConvertWebMercatorToWGS84(PointDepart.Coordonnees, false);
                            break;
                        }
                    case Projections.LatLon:
                        {
                            LatLon = ConvertLatLonToWGS84(PointDepart.Coordonnees, false);
                            break;
                        }
                    case Projections.Datum:
                        {
                            LatLon = PointD.Scale(PointDepart.Coordonnees, Deg_Rad);
                            break;
                        }
                }
                if (!LatLon.IsEmpty) // pas d'erreur
                {
                    if (DatumDepart < Datums.Lambert_93 || DatumDepart > Datums.WGS84)
                    {
                        LatLon = ConvertDatumToDatum(DatumDepart, LatLon, Datums.WGS84, false);
                    }
                    LatLon.Scale(Rad_Deg);
                    return LatLon;
                }
                else
                {
                    return PointD.Empty;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "3B18");
                return PointD.Empty;
            }
        }
        /// <summary> convertit une coordonnée de départ en DD WGS84 en coordonnées X, Y exprimée en X , Y d'une projection Grille ou en DD </summary>
        /// <param name="PointDepart"> point LatLon WGS84 à convertir </param>
        /// <param name="DatumArrivee">Latitude de départ (input d.d)</param>
        internal static PointProjection ConvertWGS84ToProjection(PointD PointDepart, Datums DatumArrivee)
        {
            try
            {
                PointDepart.Scale(Deg_Rad);
                if (DatumArrivee < Datums.Lambert_93 || DatumArrivee > Datums.WGS84)
                {
                    // on doit basculer les coordonnées radians de wgs84 vers des coordonnées radians de l'éllipsoïde du datum d'arrivée
                    PointDepart = ConvertDatumToDatum(Datums.WGS84, PointDepart, DatumArrivee, false);
                }
                var PointArrivee = default(PointProjection);
                switch (PARAM_DATUMS.ParametresDatums[(int)DatumArrivee].Projection)
                {
                    case Projections.Lambert:
                        {
                            PointArrivee.Coordonnees = ConvertLLToLambert(PointDepart, DatumArrivee, false);
                            break;
                        }
                    case Projections.UTM:
                        {
                            PointArrivee = ConvertLLToUtm(PointDepart, DatumArrivee, Deg: false);
                            break;
                        }
                    case Projections.Suisse:
                        {
                            PointArrivee.Coordonnees = ConvertWGS84ToLV03(PointDepart, false);
                            if (DatumArrivee == Datums.Grille_Suisse_LV95)
                            {
                                LV03ToLV95(ref PointArrivee.Coordonnees);
                            }

                            break;
                        }
                    case Projections.WMercator:
                        {
                            PointArrivee.Coordonnees = ConvertWGS84ToWebMercator(PointDepart, false);
                            break;
                        }
                    case Projections.LatLon:
                        {
                            PointArrivee.Coordonnees = ConvertWGS84ToLatLon(PointDepart, false);
                            break;
                        }
                    case Projections.Datum:
                        {
                            PointArrivee.Coordonnees = PointD.Scale(PointDepart, Rad_Deg);
                            break;
                        }
                }
                return PointArrivee;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "3A2D");
                return new PointProjection();
            }
        }
        /// <summary> Converti une projection ou un datum dans une autre projection ou datum. Procédure généraliste </summary>
        /// <param name="DatumDepart"> projection ou datum au départ </param>
        /// <param name="PointDepart"> point à convertir </param>
        /// <param name="DatumArrivee"> projection ou datum à l'arrivée </param>
        internal static PointProjection ConvertProjectionToProjection(Datums DatumDepart, PointProjection PointDepart, Datums DatumArrivee)
        {
            var PointLatLon = default(PointD);
            var PointArrivee = default(PointProjection);
            do
            {
                try
                {
                    // tranforme le point de départ en coordoonées LatLon ramenées sur l'éllipsoïde associé au datum et exprimée en radians
                    switch (PARAM_DATUMS.ParametresDatums[(int)DatumDepart].Projection)
                    {
                        case Projections.Lambert:
                            {
                                PointLatLon = ConvertLambertToLL(PointDepart.Coordonnees, DatumDepart, false);
                                break;
                            }
                        case Projections.UTM:
                            {
                                if (PointDepart.Zone == 0 || PointDepart.Hemisphere == default(char))
                                {
                                    throw new Exception("Paramètres projection UTM obligatoire");
                                }
                                PointLatLon = ConvertUtmToLL(DatumDepart, PointDepart, false);
                                break;
                            }
                        case Projections.Suisse:
                            {
                                PointLatLon = ConvertSuisseToCH1903(PointDepart.Coordonnees, DatumDepart, false);
                                break;
                            }
                        case Projections.WMercator:
                            {
                                PointLatLon = ConvertWebMercatorToWGS84(PointDepart.Coordonnees, false);
                                break;
                            }
                        case Projections.LatLon:
                            {
                                PointLatLon = ConvertLatLonToWGS84(PointDepart.Coordonnees, false);
                                break;
                            }
                        case Projections.Datum:
                            {
                                PointLatLon = PointD.Scale(PointDepart.Coordonnees, Deg_Rad);
                                break;
                            }
                    }
                    // si il y a eu une erreur on sort
                    if (PointLatLon.IsEmpty)
                        break;
                    // transforme les coordonnées LatLon de l'ellispsoïde de départ en coordonnées LatLon sur l'Ellipsoïde d'arrivée exprimée en radians
                    if (PARAM_DATUMS.ParametresDatums[(int)DatumDepart].Ellipsoide != PARAM_DATUMS.ParametresDatums[(int)DatumArrivee].Ellipsoide)
                    {
                        PointLatLon = ConvertDatumToDatum(DatumDepart, PointLatLon, DatumArrivee, false);
                    }
                    // Tranforme les coordonnées latlon sur l'Ellipsoïde d'arrivée en coordonnées du point d'arrivée
                    switch (PARAM_DATUMS.ParametresDatums[(int)DatumArrivee].Projection)
                    {
                        case Projections.Lambert:
                            {
                                PointArrivee.Coordonnees = ConvertLLToLambert(PointLatLon, DatumArrivee, false);
                                break;
                            }
                        case Projections.UTM:
                            {
                                PointArrivee = ConvertLLToUtm(PointLatLon, DatumArrivee, Deg: false);
                                break;
                            }
                        case Projections.Suisse:
                            {
                                PointArrivee.Coordonnees = ConvertCH1903ToSuisse(PointLatLon, DatumArrivee, false);
                                break;
                            }
                        case Projections.WMercator:
                            {
                                PointArrivee.Coordonnees = ConvertWGS84ToWebMercator(PointLatLon, false);
                                break;
                            }
                        case Projections.LatLon:
                            {
                                PointArrivee.Coordonnees = ConvertWGS84ToLatLon(PointLatLon, false);
                                break;
                            }
                        case Projections.Datum:
                            {
                                PointArrivee.Coordonnees = PointD.Scale(PointLatLon, Rad_Deg);
                                break;
                            }
                    }
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "3D0E");
                    PointArrivee = new PointProjection();
                }
            }
            while (false);
            return PointArrivee;
        }
        /// <summary> convertir des degrés décimaux en radians </summary>
        /// <param name="Valeur">valeur à convertir (input d.d)</param>
        /// <returns>valeur convertie rad</returns>
        internal static double DegToRad(double Valeur)
        {
            return Valeur * Deg_Rad;
        }
        /// <summary> convertir des radians en degré décimaux </summary>
        /// <param name="Valeur">valeur à convertir (input rad)</param>
        /// <returns>valeur convertie d.d</returns>
        internal static double RadToDeg(double Valeur)
        {
            return Valeur * Rad_Deg;
        }
        #endregion
        #region Fonctions spécialisées
        #region Tuile <--> Pixels et Tuile <--> Grille
        #region Bas niveau
        /// <summary> transforme des pixels en indice de tuile </summary>
        /// <param name="Pixel"> Nombre de pixels (index base 0) </param>
        /// <param name="IsFinTuile"> les pixels sont considérés comme en début ou en fin de tuile. Uniquement quand le reste = 0</param>
        internal static int PixelToNumTile(int Pixel, bool IsFinTuile)
        {
            // on est obligé d'utiliser Math.Floor ou int car l'opérateur \ ne fonctionne pas pour les nombres négatifs. de -1 à -256 doit renvoyer -1
            int PixelToNumTileRet = (int)Math.Floor(Pixel / (double)NbPixelsTuile);
            if (IsFinTuile && Pixel % NbPixelsTuile == 0)
                PixelToNumTileRet -= 1;
            return PixelToNumTileRet;
        }
        /// <summary> transforme un indice de tuile en pixels </summary>
        /// <param name="Num"> indice tuile </param>
        /// <param name="IsFinTuile"> les pixels retournés sont ceux du début ou de fin de tuile. </param>
        private static int NumTileToPixel(int Num, bool IsFinTuile)
        {
            int NumTileToPixelRet;
            if (IsFinTuile)
            {
                NumTileToPixelRet = (Num + 1) * NbPixelsTuile;
            }
            else
            {
                NumTileToPixelRet = Num * NbPixelsTuile;
            }

            return NumTileToPixelRet;
        }
        #endregion
        /// <summary> transforme un point exprimé en indices tuile en un point exprimé en grille de la projection du site de capture </summary>
        /// <param name="Tuile"> point exprimé en coordonnées tuile à transformer </param>
        /// <param name="SiteCarto"> site de capture </param>
        /// <param name="Echelle"> indice de l'échelle de capture </param>
        /// <param name="IsFinTuile"> prendre en compte le point Pt2 de la tuile à la place du point Pt0 </param>
        internal static PointD TuileToPointGrille(Point Tuile, SitesCartographiques SiteCarto, Echelles Echelle, bool IsFinTuile)
        {
            var PointPixels = TuileToPointPixel(Tuile, IsFinTuile);
            return PointPixelToPointGrille(PointPixels, SiteCarto, Echelle);
        }
        /// <summary> transforme un point exprimé en grille de la projection du site de capture en un point exprimé en indices tuile </summary>
        /// <param name="PointGrille"></param>
        /// <param name="SiteCarto"> site de capture </param>
        /// <param name="Echelle"> indice de l'échelle de capture </param>
        /// <param name="IsFinTuile"> prendre en compte la fin de la tuile </param>
        internal static Point PointGrilleToTuile(PointD PointGrille, SitesCartographiques SiteCarto, Echelles Echelle, bool IsFinTuile)
        {
            var PointPixels = PointGrilleToPointPixel(PointGrille, SiteCarto, Echelle);
            return PointPixelToTuile(PointPixels, IsFinTuile);
        }
        /// <summary> transforme un point exprimé en indices tuile en un point exprimé en pixels de la projection du site de capture </summary>
        /// <param name="Tuile"> point exprimé en coordonnées tuile à transformer </param>
        /// <param name="IsFinTuile"> prendre en compte le point Pt2 de la tuile à la place du point Pt0 </param>
        internal static Point TuileToPointPixel(Point Tuile, bool IsFinTuile)
        {
            return new Point(NumTileToPixel(Tuile.X, IsFinTuile), NumTileToPixel(Tuile.Y, IsFinTuile));
        }
        /// <summary> transforme un point exprimé en pixels de la projection du site de capture en un point exprimé en indices tuile </summary>
        /// <param name="PointPixel"> point exprimé en pixels de la projection à transformer</param>
        /// <param name="IsFinTuile"> prendre en compte la fin de la tuile </param>
        internal static Point PointPixelToTuile(Point PointPixel, bool IsFinTuile)
        {
            return new Point(PixelToNumTile(PointPixel.X, IsFinTuile), PixelToNumTile(PointPixel.Y, IsFinTuile));
        }
        #endregion
        #region Region DD --> DMS et DD <--> WebMercator
        /// <summary> tranforme une surface exprimée en DD en une chaine décrivant Pt0 et PT2 en DMS </summary>
        /// <param name="RegionDD"> surface DD à transformer </param>
        /// <param name="InclureSens"> les caractères N,S,E,O sont inclus </param>
        /// <param name="Prec"> Nombre de décimale pour les secondes </param>
        /// <param name="Separateur"> caractère de séparation entre Pt0 et Pt2 </param>
        internal static string RegionDDToDMSText(RectangleD RegionDD, bool InclureSens = true, int Prec = 1, string Separateur = ",")
        {
            string Pt0DMS = ConvertPointDDtoDMS(RegionDD.Pt0, InclureSens, Prec, Separateur);
            string Pt2DMS = ConvertPointDDtoDMS(RegionDD.Pt2, InclureSens, Prec, Separateur);
            return $"PtNO : {Pt0DMS}, PtSE : {Pt2DMS}";
        }
        /// <summary> tranforme une surface exprimée en DD en une surface exprimée en grille PseudoMercator </summary>
        /// <param name="RegionDD"> surface DD à transformer </param>
        internal static RectangleD RegionDDToRegionWebMecator(RectangleD RegionDD)
        {
            return new RectangleD(PointDDToPointGrille(RegionDD.Pt0), PointDDToPointGrille(RegionDD.Pt2));
        }
        /// <summary> tranforme une surface exprimée en grille PseudoMercator en une surface exprimée en DD </summary>
        /// <param name="RegionGrille"> surface grille PseudoMercator à transformer </param>
        internal static RectangleD RegionWebMecatorToRegionDD(RectangleD RegionGrille)
        {
            return new RectangleD(PointGrilleToPointDD(RegionGrille.Pt0), PointGrilleToPointDD(RegionGrille.Pt2));
        }
        #endregion
        #region Grille <--> DD
        /// <summary> tranforme une surface exprimée en grille (mètres) en une surface exprimée en DD </summary>
        /// <param name="RegionGrille"> surface grille à transformer </param>
        /// <param name="SiteCarto"> Site carto de la projection grille </param>
        internal static RectangleD RegionGrilleToRegionDD(RectangleD RegionGrille, SitesCartographiques SiteCarto)
        {
            return new RectangleD(PointGrilleToPointDD(RegionGrille.Pt0, DatumSiteWeb((int)SiteCarto)),
                                  PointGrilleToPointDD(RegionGrille.Pt2, DatumSiteWeb((int)SiteCarto)));
        }
        /// <summary> tranforme une surface exprimée en DD en une surface exprimée en grille (mètres) </summary>
        /// <param name="RegionDD"> surface DD à transformer </param>
        /// <param name="SiteCarto"> Site carto de la projection grille </param>
        internal static RectangleD RegionDDToRegionGrille(RectangleD RegionDD, SitesCartographiques SiteCarto)
        {
            return new RectangleD(PointDDToPointGrille(RegionDD.Pt0, DatumSiteWeb((int)SiteCarto)),
                                  PointDDToPointGrille(RegionDD.Pt2, DatumSiteWeb((int)SiteCarto)));
        }
        /// <summary> tranforme une point exprimé en DD en un point exprimé en grille WebMercator ou en grille Suisse </summary>
        /// <param name="PointDD"> point DD à transformer </param>
        internal static PointD PointDDToPointGrille(PointD PointDD, Datums Datum = Datums.Web_Mercator)
        {
            PointD PointDDToPointGrilleRet;
            // il n'y a que 2 grilles possibles avec les Datums gérés par FCGP
            if (Datum == Datums.Web_Mercator)
            {
                PointDDToPointGrilleRet = ConvertWGS84ToWebMercator(PointDD, true);
            }
            else
            {
                PointDDToPointGrilleRet = ConvertWGS84ToLV03(PointDD, true);
            }

            return PointDDToPointGrilleRet;
        }
        /// <summary> tranforme un point exprimé en grille PseudoMercator ou en grille Suisse en un point exprimée en DD WGS84</summary>
        /// <param name="PointGrille"> point grille PseudoMercator à transformer </param>
        internal static PointD PointGrilleToPointDD(PointD PointGrille, Datums Datum = Datums.Web_Mercator)
        {
            PointD PointGrilleToPointDDRet;
            // il n'y a que 2 grilles possibles avec les Datums gérés par FCGP
            if (Datum == Datums.Web_Mercator)
            {
                PointGrilleToPointDDRet = ConvertWebMercatorToWGS84(PointGrille, true);
            }
            else
            {
                PointGrilleToPointDDRet = ConvertLV03ToWGS84(PointGrille, true);
            }

            return PointGrilleToPointDDRet;
        }
        #endregion
        #region Pixels <--> DD
        /// <summary> transforme une surface exprimée en coordonnées virtuelles (Pixels) en surface exprimée en coordonnées DD </summary>
        /// <param name="RegionPixels"> surface virtuelles </param>
        /// <param name="SiteCarto"> site carto des coordonnées </param>
        /// <param name="Echelle"> echelle des coordonnées virtuelles </param>
        internal static RectangleD RegionPixelsToRegionDD(Rectangle RegionPixels, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            return new RectangleD(PointPixelsToPointDD(RegionPixels.Location, SiteCarto, Echelle),
                                  PointPixelsToPointDD(Point.Add(RegionPixels.Location, RegionPixels.Size), SiteCarto, Echelle));
        }
        /// <summary> transforme une surface exprimée en coordonnées DD en surface exprimée en coordonnées virtuelles (Pixels) </summary>
        /// <param name="RegionDD"> Surface DD </param>
        /// <param name="SiteCarto"> site carto des coordonnées </param>
        /// <param name="Echelle"> echelle des coordonnées DD </param>
        internal static Rectangle RegionDDToRegionPixels(RectangleD RegionDD, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            var Pt0 = PointDDToPointPixels(RegionDD.Pt0, SiteCarto, Echelle);
            var Pt2 = PointDDToPointPixels(RegionDD.Pt2, SiteCarto, Echelle);
            return new Rectangle(Pt0, new Size(Pt2.X - Pt0.X, Pt2.Y - Pt0.Y));
        }
        /// <summary> transforme un point exprimé en coordonnées virtuelles (pixels d'image issue du web qui dépend de l'échelle de représentation) en coordonnées DD </summary>
        /// <param name="PointPixels"> coordonnées virtuelles </param>
        /// <param name="SiteCarto"> site carto des coordonnées </param>
        /// <param name="Echelle"> echelle des coordonnées virtuelles </param>
        internal static PointD PointPixelsToPointDD(Point PointPixels, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            // transformation pixel to projection grille web à l'aide de l'origine et de la résolution par mètres de l'échelle
            var Grille = PointPixelToPointGrille(PointPixels, SiteCarto, Echelle);
            // transformation projection grille web to dd à partir du module de transformation de coordonnées
            return PointGrilleToPointDD(Grille, DatumSiteWeb((int)SiteCarto));
        }
        /// <summary> transforme un point exprimé en DD en coordonnées virtuelles(pixels d'image issue du web qui dépend de l'échelle de représentation) </summary>
        /// <param name="PointCoordonnees"> coordonnées DD </param>
        /// <param name="SiteCarto"> site carto des coordonnées </param>
        /// <param name="Echelle"> echelle des coordonnées DD </param>
        internal static Point PointDDToPointPixels(PointD PointCoordonnees, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            // transformation dd to projection grille web à partir du module de transformation de coordonnées
            PointCoordonnees = PointDDToPointGrille(PointCoordonnees, DatumSiteWeb((int)SiteCarto));
            // transformation projection grille web to pixel à l'aide de l'origine et de la résolution par mètres de l'échelle
            return PointGrilleToPointPixel(PointCoordonnees, SiteCarto, Echelle);
        }
        /// <summary> transforme une latitude en pixel mais de type double de manière à avoir une partie fractionnaire indiquant le poids respectif de chaque ligne de pixel </summary>
        /// <param name="Lat"> Latitude à transformer </param>
        /// <param name="SiteCarto"> site carto des coordonnées </param>
        /// <param name="Echelle"> echelle des coordonnées </param>
        /// <remarks> Sert uniquement pour l'interpolation des cartes sans grille </remarks>
        internal static double LatitudeWebMercatorToPixel(double Lat, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            double GrilleY = Math.Log(Math.Tan(Lat * Deg_Rad_2 + Pi_4)) * RayonWGS84;
            var LatitudeWebMercatorToPixelRet = (OrigineSiteWeb((int)SiteCarto).Y - GrilleY) * PixelsMetre(Echelle);
            return LatitudeWebMercatorToPixelRet;
        }
        #endregion
        #region Pixels <--> Grille
        /// <summary> tranforme une surface exprimée en coordonnées virtuelles(pixels) en une surface exprimée en grille de la projection du site de capture </summary>
        /// <param name="RegionPixels"> surface exprimée en pixels </param>
        /// <param name="SiteCarto"> site de capture </param>
        /// <param name="Echelle"> échelle de capture </param>
        internal static RectangleD RegionPixelsToRegionGrille(Rectangle RegionPixels, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            return new RectangleD(PointPixelToPointGrille(RegionPixels.Location, SiteCarto, Echelle), PointPixelToPointGrille(new Point(RegionPixels.Right, RegionPixels.Bottom), SiteCarto, Echelle));
        }
        /// <summary> transforme un point de coordonnées virtuelles en un point de coordonnées exprimées en mètre de la grille du site carto </summary>
        /// <param name="PointPixel"> point à convertir exprimé en coordonnées virtuelles (pixels image) </param>
        /// <param name="SiteCarto"> site de capture </param>
        /// <param name="Echelle"> échelle de capture </param>
        internal static PointD PointPixelToPointGrille(Point PointPixel, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            var PointGrille = new PointD(PointPixel);
            PointGrille.Scale(MetrePixels(Echelle));
            PointGrille.X += OrigineSiteWeb((int)SiteCarto).X;
            PointGrille.Y = OrigineSiteWeb((int)SiteCarto).Y - PointGrille.Y;
            return PointGrille;
        }
        /// <summary> tranforme une surface exprimée en grille de la projection du site de capture en une surface exprimée en pixels </summary>
        /// <param name="RegionGrille"> surface exprimée en grille PseudoMercator ou Projection Suisse à transformer </param>
        /// <param name="SiteCarto"> site de capture </param>
        /// <param name="Echelle"> échelle de capture </param>
        internal static Rectangle RegionGrilleToRegionPixels(RectangleD RegionGrille, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            var Pt0 = PointGrilleToPointPixel(RegionGrille.Pt0, SiteCarto, Echelle);
            var Pt2 = PointGrilleToPointPixel(RegionGrille.Pt2, SiteCarto, Echelle);
            return new Rectangle(Pt0, new Size(Pt2.X - Pt0.X, Pt2.Y - Pt0.Y));
        }
        /// <summary> transforme un point de coordonnées exprimée en mètre de la grille du site carto en un point de coordonnées virtuelles </summary>
        /// <param name="PointGrille"> Point à convertir en grille PseudoMercator ou Projection Suisse </param>
        /// <param name="SiteCarto"> site de capture </param>
        /// <param name="Echelle"> échelle de capture </param>
        /// <returns> point exprimé en coordonnées virtuelles (pixels image) </returns>
        internal static Point PointGrilleToPointPixel(PointD PointGrille, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            PointGrille.X -= OrigineSiteWeb((int)SiteCarto).X;
            PointGrille.Y = OrigineSiteWeb((int)SiteCarto).Y - PointGrille.Y;
            PointGrille.Scale(PixelsMetre(Echelle));
            return PointGrille.ToPoint;
        }
        #endregion
        #endregion
    }
}