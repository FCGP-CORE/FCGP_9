using static FCGP.ConvertirCoordonnees;

namespace FCGP.Coordonnees
{
    /// <summary> Ce module contient les fonctions permettant la conversion entre les coordonnées Géodétique (Latitude et Longitude) et une projection 
    /// Lambert Conforme Conique à 1 parallèle en X et Y ou l'inverse traduit du projet Geotrans écrit en C </summary>
    internal static class ProjectionLambert_1
    {
        #region Variables et Constantes
        /// <summary> Donne par defaut les différentes constantes liées à la projection Lambert 1 et au système CLARKE 1880 ("NTF") </summary>
        private const double MAX_LAT = Deg_Rad * 89.9997222222222d; // ((PI * 89.9997222222222) / 180)
        private const double MIN_SCALE_FACTOR = 0.000000001d;
        private const double ONE_SECOND = 0.00000489d;
        private static double Lambert_1_a = RayonWGS84;
        private static double Lambert_1_f = F_WGS84; // 1 / 298.257223563
        private static double es = 0.0818191908426219d;
        private static double es_OVER_2 = 0.040909595421311d;
        private static double Lambert_1_n = 0.70710678118655d;
        private static double Lambert_1_rho0 = 6388838.2901212d;
        private static double Lambert_1_rho_olat = 6388838.2901211d;
        private static double Lambert_1_t0 = 0.41618115138974d;
        private static double Lambert_1_Origin_Lat = Deg_Rad * 45.0d; // (45 * PI / 180)
        private static double Lambert_1_Origin_Long = 0.0d;
        private static double Lambert_1_False_Northing = 0.0d;
        private static double Lambert_1_False_Easting = 0.0d;
        private static double Lambert_1_Scale_Factor = 1.0d;
        private const double Lambert_Delta_Easting = 400_000_000.0d;
        private const double Lambert_Delta_Northing = 400_000_000.0d;

        /// <summary> Donne différentes erreurs pouvant survenir lors de la transformation des coordonnées </summary>
        [Flags]
        internal enum LAMBERT_1 : int
        {
            /// <summary>Flag OK</summary>"
            NO_ERROR = 0,
            /// <summary>Flag erreur concernant la Latitude</summary>
            LAT_ERROR = 1,
            /// <summary>Flag erreur concernant la Longitude</summary>
            LON_ERROR = 2,
            /// <summary>Flag erreur concernant les abscisses(Easting ou X)</summary>
            EASTING_ERROR = 4,
            /// <summary>Flag erreur concernant les ordonnées(Northing ou Y)</summary>
            NORTHING_ERROR = 8,
            /// <summary>Flag erreur concernant l'origine de la Latitude</summary>
            ORIGIN_LAT_ERROR = 16,
            /// <summary>Flag erreur concernant le méridien central</summary>
            CENT_MER_ERROR = 32,
            /// <summary>Flag erreur concernant le facteur d'échelle</summary>
            SCALE_FACTOR_ERROR = 64,
            /// <summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 128,
            /// <summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 256
        }
        #endregion
        #region Functions and Procedures
        private static double LAMBERT_m(double clat, double essin)
        {
            return clat / Math.Sqrt(1.0d - essin * essin);
        }
        private static double LAMBERT_t(double lat, double essin)
        {
            return Math.Tan(Pi_4 - lat / 2.0d) / Math.Pow((1.0d - essin) / (1.0d + essin), es_OVER_2);
        }
        private static double ES_SIN_Func(double sinlat)
        {
            return es * sinlat;
        }
        /// <summary>The function Set_Lambert_1_Parameters receives the ellipsoid parameters and Lambert Conformal Conic (1 parallel)
        /// projection parameters as inputs, and sets the corresponding state variables. </summary>
        /// <param name="a">Semi-major axis of ellipsoid, in meters(input)</param>
        /// <param name="f">Flattening of ellipsoid(input)</param>
        /// <param name="Origin_Latitude">Latitude of origin, in radians(input)</param>
        /// <param name="Central_Meridian">Longitude of origin, in radians(input)</param>
        /// <param name="False_Easting">False easting, in meters(input)</param>
        /// <param name="False_Northing">False northing, in meters(input)</param>
        /// <param name="Scale_Factor">Projection scale factor(input)</param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_1.NO_ERROR is returned.</returns>
        internal static LAMBERT_1 Set_LAMBCC1_Parameters(double a, double f, double Origin_Latitude, double Central_Meridian, double False_Easting, double False_Northing, double Scale_Factor)
        {
            double es2;
            double es_sin;
            double m0;
            double inv_f = 1.0d / f;
            var Error_Code = default(LAMBERT_1);
            if (a <= 0d)
            {
                Error_Code |= LAMBERT_1.A_ERROR;
            }
            if (inv_f < 250.0d || inv_f > 350.0d)
            {
                Error_Code |= LAMBERT_1.INV_F_ERROR;
            }
            if (Origin_Latitude < -MAX_LAT || Origin_Latitude > MAX_LAT || Origin_Latitude > -ONE_SECOND && Origin_Latitude < ONE_SECOND)
            {
                Error_Code |= LAMBERT_1.ORIGIN_LAT_ERROR;
            }
            if (Central_Meridian < -PI || Central_Meridian > TwoPI)
            {
                Error_Code |= LAMBERT_1.CENT_MER_ERROR;
            }
            if (Scale_Factor < MIN_SCALE_FACTOR)
            {
                Error_Code |= LAMBERT_1.SCALE_FACTOR_ERROR;
            }
            if (Error_Code == LAMBERT_1.NO_ERROR)
            {
                Lambert_1_a = a;
                Lambert_1_f = f;
                Lambert_1_Origin_Lat = Origin_Latitude;
                if (Central_Meridian > PI)
                {
                    Central_Meridian -= TwoPI;
                }
                Lambert_1_Origin_Long = Central_Meridian;
                Lambert_1_False_Easting = False_Easting;
                Lambert_1_False_Northing = False_Northing;
                Lambert_1_Scale_Factor = Scale_Factor;
                es2 = 2.0d * Lambert_1_f - Lambert_1_f * Lambert_1_f;
                es = Math.Sqrt(es2);
                es_OVER_2 = es / 2.0d;
                Lambert_1_n = Math.Sin(Lambert_1_Origin_Lat);
                es_sin = ES_SIN_Func(Math.Sin(Lambert_1_Origin_Lat));
                m0 = LAMBERT_m(Math.Cos(Lambert_1_Origin_Lat), es_sin);
                Lambert_1_t0 = LAMBERT_t(Lambert_1_Origin_Lat, es_sin);
                Lambert_1_rho0 = Lambert_1_a * Lambert_1_Scale_Factor * m0 / Lambert_1_n;
                Lambert_1_rho_olat = Lambert_1_rho0;
            }
            return Error_Code;
        }
        /// <summary>The function Convert_Lambert_1_To_Geodetic converts Lambert ConformalConic (1 parallel) projection (easting and northing) coordinates to Geodetic
        /// (latitude and longitude) coordinates, according to the current ellipsoid and Lambert Conformal Conic (1 parallel) projection parameters.</summary>
        /// <param name="PointGrille"> point à convertir </param>
        /// <param name="PointLatLon"> resultat de la convertion </param>
        /// <returns>If any errors occur, the error code(s) are returned by the function PointLatLon is empty, otherwise LAMBERT_NO_ERROR is returned.</returns>
        internal static LAMBERT_1 Convert_LAMBCC1_To_Geodetic(PointD PointGrille, ref PointD PointLatLon)
        {
            double dx, dy, rho, rho_olat_MINUS_dy, t, PHI, es_sin, tempPHI = default, theta;
            double tolerance = 0.000000000485d;
            int count = 30;
            var Error_Code = LAMBERT_1.NO_ERROR;
            if (PointGrille.X < Lambert_1_False_Easting - Lambert_Delta_Easting || PointGrille.X > Lambert_1_False_Easting + Lambert_Delta_Easting)
            {
                Error_Code |= LAMBERT_1.EASTING_ERROR;
            }
            if (PointGrille.Y < Lambert_1_False_Northing - Lambert_Delta_Northing || PointGrille.Y > Lambert_1_False_Northing + Lambert_Delta_Northing)
            {
                Error_Code |= LAMBERT_1.NORTHING_ERROR;
            }
            if (Error_Code == LAMBERT_1.NO_ERROR)
            {
                dy = PointGrille.Y - Lambert_1_False_Northing;
                dx = PointGrille.X - Lambert_1_False_Easting;
                rho_olat_MINUS_dy = Lambert_1_rho_olat - dy;
                rho = Math.Sqrt(dx * dx + rho_olat_MINUS_dy * rho_olat_MINUS_dy);
                if (Lambert_1_n < 0d)
                {
                    rho *= -1;
                    dx *= -1;
                    rho_olat_MINUS_dy *= -1;
                }
                if (rho != 0d)
                {
                    theta = Math.Atan2(dx, rho_olat_MINUS_dy) / Lambert_1_n;
                    t = Lambert_1_t0 * Math.Pow(rho / Lambert_1_rho0, 1d / Lambert_1_n);
                    PHI = Pi_2 - 2d * Math.Atan(t);
                    while (Math.Abs(PHI - tempPHI) > tolerance && count > 0)
                    {
                        tempPHI = PHI;
                        es_sin = ES_SIN_Func(Math.Sin(PHI));
                        PHI = Pi_2 - 2d * Math.Atan(t * Math.Pow((1d - es_sin) / (1d + es_sin), es_OVER_2));
                        count -= 1;
                    }
                    if (count == 0)
                    {
                        Error_Code |= LAMBERT_1.NORTHING_ERROR;
                        return Error_Code;
                    }
                    PointLatLon.Lat = PHI;
                    PointLatLon.Lon = theta + Lambert_1_Origin_Long;
                    if (Math.Abs(PointLatLon.Lat) < 0.0000002d)
                    {
                        PointLatLon.Lat = 0d;
                    }
                    if (PointLatLon.Lat > Pi_2)
                    {
                        PointLatLon.Lat = Pi_2;
                    }
                    else if (PointLatLon.Lat < -Pi_2)
                    {
                        PointLatLon.Lat = -Pi_2;
                    }
                    if (PointLatLon.Lon > PI)
                    {
                        if (PointLatLon.Lon - PI < 0.0000035d)
                        {
                            PointLatLon.Lon = PI;
                        }
                        else
                        {
                            PointLatLon.Lon -= TwoPI;
                        }
                    }
                    if (PointLatLon.Lon < -PI)
                    {
                        if (Math.Abs(PointLatLon.Lon + PI) < 0.0000035d)
                        {
                            PointLatLon.Lon = -PI;
                        }
                        else
                        {
                            PointLatLon.Lon += TwoPI;
                        }
                    }

                    if (Math.Abs(PointLatLon.Lon) < 0.0000002d)
                    {
                        PointLatLon.Lon = 0d;
                    }
                    if (PointLatLon.Lon > PI)
                    {
                        PointLatLon.Lon = PI;
                    }
                    else if (PointLatLon.Lon < -PI)
                    {
                        PointLatLon.Lon = -PI;
                    }
                }
                else
                {
                    if (Lambert_1_n > 0d)
                    {
                        PointLatLon.Lat = Pi_2;
                    }
                    else
                    {
                        PointLatLon.Lat = -Pi_2;
                    }
                    PointLatLon.Lon = Lambert_1_Origin_Long;
                }
            }
            return Error_Code;
        }
        /// <summary>The function Convert_Geodetic_To_Lambert_1 converts Geodetic (latitude and longitude) coordinates to Lambert Conformal Conic (1 parallel) 
        /// projection (easting and northing) coordinates, according to the current ellipsoid and
        /// Lambert Conformal Conic (1 parallel) projection parameters. </summary>
        /// <param name="PointLatLon"> point à convertir en rad </param>
        /// <param name="PointGrille"> résultat de la conversion en mètres </param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_NO_ERROR is returned.</returns>
        internal static LAMBERT_1 Convert_Geodetic_To_LAMBCC1(PointD PointLatLon, ref PointD PointGrille)
        {
            double t;
            double rho;
            double dlam;
            double theta;
            var Error_Code = default(LAMBERT_1);
            if (PointLatLon.Lat < -Pi_2 || PointLatLon.Lat > Pi_2)
            {
                Error_Code |= LAMBERT_1.LAT_ERROR;
            }
            if (PointLatLon.Lon < -PI || PointLatLon.Lon > TwoPI)
            {
                Error_Code |= LAMBERT_1.LON_ERROR;
            }
            if (Error_Code == LAMBERT_1.NO_ERROR)
            {
                if (Math.Abs(Math.Abs(PointLatLon.Lat) - Pi_2) > 0.0000000001d)
                {
                    t = LAMBERT_t(PointLatLon.Lat, ES_SIN_Func(Math.Sin(PointLatLon.Lat)));
                    rho = Lambert_1_rho0 * Math.Pow(t / Lambert_1_t0, Lambert_1_n);
                }
                else
                {
                    if (PointLatLon.Lat * Lambert_1_n <= 0d)
                    {
                        Error_Code |= LAMBERT_1.LAT_ERROR;
                        return Error_Code;
                    }
                    rho = 0d;
                }
                dlam = PointLatLon.Lon - Lambert_1_Origin_Long;
                if (dlam > PI)
                {
                    dlam -= TwoPI;
                }
                if (dlam < -PI)
                {
                    dlam += TwoPI;
                }
                theta = Lambert_1_n * dlam;
                PointGrille.X = rho * Math.Sin(theta) + Lambert_1_False_Easting;
                PointGrille.Y = Lambert_1_rho_olat - rho * Math.Cos(theta) + Lambert_1_False_Northing;
            }
            return Error_Code;
        }
        #endregion
    }
}