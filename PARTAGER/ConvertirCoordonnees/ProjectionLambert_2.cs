using static FCGP.ConvertirCoordonnees;
using static FCGP.Coordonnees.ProjectionLambert_1;
using static FCGP.Enumerations;

namespace FCGP.Coordonnees
{
    /// <summary> Ce module contient les fonctions permettant la conversion entre les coordonnées Géodétique (Latitude et Longitude) et une projection 
    /// Lambert Conforme Conique à 2 parallèles en X et Y ou l'inverse  traduit du projet Geotrans écrit en C </summary>
    internal static class ProjectionLambert_2
    {
        #region Haut Niveau
        /// <summary> Lambert en mètres vers ellispsoïde du datum en DD ou en Rad. </summary>
        /// <param name="PointGrille"> point X-Y à convertir </param>
        /// <param name="Datum"> datum de la projection Lambert </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point convertit sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertLambertToLL(PointD PointGrille, Datums Datum, bool Deg = true)
        {
            // on initialise les paramètres de la projection si besoin
            if (ProjectionLambert_2.Datum != Datum)
            {
                if (Set_LAMBCC2_Parameters(Datum) != LAMBERT_2.NO_ERROR)
                {
                    throw new Exception("Erreur dans la mise à jour des paramètres de la projection");
                }
            }
            var Ret = new PointD(0d, 0d);
            // transforme les x, y en longitude et latitude exprimée en radians
            if (Convert_LAMBCC2_To_Geodetic(PointGrille, ref Ret) == LAMBERT_2.NO_ERROR)
            {
                if (Deg)
                    Ret.Scale(Rad_Deg);
                return Ret;
            }
            else
            {
                return PointD.Empty;
            }
        }
        /// <summary> Coordonnées lat lon sur ellispsoïde du datum en DD ou en Rad vers Lambert </summary>
        /// <param name="PointLatLon"> point Lat Lon à convertir </param>
        /// <param name="Datum"> Datum de la projection Lambert </param>
        /// <param name="Deg"> indique si la coordonnées Lat Lon sont en DD </param>
        /// <returns> Le point Lat lon en DD ou rad </returns>
        internal static PointD ConvertLLToLambert(PointD PointLatLon, Datums Datum, bool Deg = true)
        {
            // transforme les DD du datum de depart en X, Y, Z du datum de depart
            if (ProjectionLambert_2.Datum != Datum)
            {
                if (Set_LAMBCC2_Parameters(Datum) != LAMBERT_2.NO_ERROR)
                {
                    throw new Exception("Erreur dans la mise à jour des paramètres de la projection");
                }
            }
            if (Deg)
                PointLatLon.Scale(Deg_Rad);
            // transforme les DD du datum UTM en X, Y UTM du datum UTM
            var Ret = new PointD(0d, 0d);
            if (Convert_Geodetic_To_LAMBCC2(PointLatLon, ref Ret) == LAMBERT_2.NO_ERROR)
            {
                return Ret;
            }
            else
            {
                return PointD.Empty;
            }
        }
        #endregion
        #region Constantes des différentes projections lambert
        /// <summary> Libellés des différents projections lambert. Il y a un décalage de 2 pour prendre en compte l'enum Datums </summary>
        private readonly static double[] Centrals_Meridians = new double[] { 0.0d, 0.0d, 2.33722917d, 2.33722917d, 2.33722917d, 2.33722917d, 2.33722917d, 3.0d };
        private readonly static double[] Origins_Latitudes = new double[] { 0.0d, 0.0d, 49.5d, 46.8d, 46.8d, 44.1d, 42.165d, 46.5d };
        private readonly static double[] Std_Parallels_1 = new double[] { 0.0d, 0.0d, 48.5985228d, 45.89891889d, 45.89891889d, 41.5603878d, 41.5603878d, 44.0d };
        private readonly static double[] Std_Parallels_2 = new double[] { 0.0d, 0.0d, 50.3959117d, 47.69601444d, 47.69601444d, 42.7676633d, 42.7676633d, 49.0d };
        private readonly static double[] False_Eastings = new double[] { 0.0d, 0.0d, 600000.0d, 600000.0d, 600000.0d, 600000.0d, 234.358d, 700000.0d };
        private readonly static double[] False_Northings = new double[] { 0.0d, 0.0d, 200000.0d, 200000.0d, 2200000.0d, 200000.0d, 185861.369d, 6600000.0d };

        // Private ReadOnly Libelles() As String = {"", "", "Projection Lambert conique conforme I (Nord)", "Projection Lambert conique conforme II (Centre)",
        // "Projection Lambert conique conforme II étendue (France)", "Projection Lambert conique conforme III (Sud)",
        // "Projection Lambert conique conforme IV (Corse)", "Projection Lambert conique conforme 93 (France)"}
        #endregion
        #region Constantes et Variables
        private const double MAX_LAT = Deg_Rad * 89.9997222222222d;
        private const double Lambert_Delta_Easting = 400_000_000d;
        private const double Lambert_Delta_Northing = 400_000_000d;
        private static Datums Datum = Datums.Aucun;
        private static Ellipsoides Ellipsoide = Ellipsoides.Aucun;
        private static double Lambert_a, Lambert_f, es, es_OVER_2, Lambert_lat0, Lambert_k0, Lambert_false_northing, Lambert_Std_Parallel_1;
        private static double Lambert_Std_Parallel_2, Lambert_Origin_Lat, Lambert_Origin_Long, Lambert_False_Northing_M, Lambert_False_Easting;
        /// <summary> Liste des différentes erreurs pouvant survenir lors de la transformation des coordonnées </summary>
        [Flags]
        private enum LAMBERT_2 : int
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
            /// <summary>Flag erreur concernant le premier parallèle standard</summary>
            FIRST_STDP_ERROR = 16,
            /// <summary>Flag erreur concernant le second parallèle standard</summary>
            SECOND_STDP_ERROR = 32,
            /// <summary>Flag erreur concernant l'origine de la Longitude</summary>
            ORIGIN_LAT_ERROR = 64,
            /// <summary>Flag erreur concernant le méridien central</summary>
            CENT_MER_ERROR = 128,
            /// <summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 256,
            /// <summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 512,
            /// <summary>Flag erreur concernant l'hémisphère</summary>
            HEMISPHERE_ERROR = 1024,
            // <summary>Flag erreur concernant le premier et second parallèle standards qui sont à zéro</summary>
            FIRST_SECOND_ERROR = 2048,
            /// <summary>Flag erreur concernant le facteur d'échelle</summary>
            SCALE_FACTOR_ERROR = 4096
        }
        #endregion
        #region Functions and Procedures
        private static double LAMBERT_m(double clat, double essin)
        {
            return clat / Math.Sqrt(1.0d - essin * essin);
        }
        private static double LAMBERT_t(double lat, double essin)
        {
            return Math.Tan(Pi_4 - lat / 2.0d) * Math.Pow((1d + essin) / (1.0d - essin), es_OVER_2);
        }
        private static double ES_SIN_Func(double sinlat)
        {
            return es * sinlat;
        }
        /// <summary>The function Set_Lambert_2_Parameters receives the ellipsoid parameters and Lambert Conformal Conic (2 parallel)
        /// projection parameters as inputs, and sets the corresponding state variables.</summary>
        private static LAMBERT_2 Set_LAMBCC2_Parameters(Datums DatumLambert)
        {
            Datum = DatumLambert;
            Ellipsoide = PARAM_DATUMS.ParametresDatums[(int)Datum].Ellipsoide;
            double a = PARAM_DATUMS.ParametresEllipsoides[(int)Ellipsoide].A;
            double F = PARAM_DATUMS.ParametresEllipsoides[(int)Ellipsoide].F;
            double Origin_Latitude = Origins_Latitudes[(int)Datum] * Deg_Rad;
            double Central_Meridian = Centrals_Meridians[(int)Datum] * Deg_Rad;
            double Std_Parallel_1 = Std_Parallels_1[(int)Datum] * Deg_Rad;
            double Std_Parallel_2 = Std_Parallels_2[(int)Datum] * Deg_Rad;
            double False_Easting = False_Eastings[(int)Datum];
            double False_Northing = False_Northings[(int)Datum];

            double inv_f = 1d / F;
            double es2, es_sin, t0, t1, t2, t_olat, m0, m1, m2, n, const_value;

            LAMBERT_1 lambert1_error_Code;
            var Error_Code = default(LAMBERT_2);
            if (a <= 0d)
            {
                Error_Code |= LAMBERT_2.A_ERROR;
            }
            if (inv_f < 250d || inv_f > 350d)
            {
                Error_Code |= LAMBERT_2.INV_F_ERROR;
            }
            if (Origin_Latitude < -MAX_LAT || Origin_Latitude > MAX_LAT)
            {
                Error_Code |= LAMBERT_2.ORIGIN_LAT_ERROR;
            }
            if (Std_Parallel_1 < -MAX_LAT || Std_Parallel_1 > MAX_LAT)
            {
                Error_Code |= LAMBERT_2.FIRST_STDP_ERROR;
            }
            if (Std_Parallel_2 < -MAX_LAT || Std_Parallel_2 > MAX_LAT)
            {
                Error_Code |= LAMBERT_2.SECOND_STDP_ERROR;
            }
            if (Std_Parallel_1 == 0d && Std_Parallel_2 == 0d)
            {
                Error_Code |= LAMBERT_2.FIRST_SECOND_ERROR;
            }
            if (Std_Parallel_1 == -Std_Parallel_2)
            {
                Error_Code |= LAMBERT_2.HEMISPHERE_ERROR;
            }
            if (Central_Meridian < -PI || Central_Meridian > TwoPI)
            {
                Error_Code |= LAMBERT_2.CENT_MER_ERROR;
            }
            if (Error_Code == LAMBERT_2.NO_ERROR)
            {
                Lambert_a = a;
                Lambert_f = F;
                Lambert_Origin_Lat = Origin_Latitude;
                Lambert_Std_Parallel_1 = Std_Parallel_1;
                Lambert_Std_Parallel_2 = Std_Parallel_2;
                if (Central_Meridian > PI)
                {
                    Central_Meridian -= TwoPI;
                }
                Lambert_Origin_Long = Central_Meridian;
                Lambert_False_Easting = False_Easting;
                Lambert_False_Northing_M = False_Northing;
                if (Math.Abs(Lambert_Std_Parallel_1 - Lambert_Std_Parallel_2) > 0.0000000001d)
                {
                    es2 = 2.0d * Lambert_f - Lambert_f * Lambert_f;
                    es = Math.Sqrt(es2);
                    es_OVER_2 = es / 2.0d;
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_Origin_Lat));
                    t_olat = LAMBERT_t(Lambert_Origin_Lat, es_sin);
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_Std_Parallel_1));
                    m1 = LAMBERT_m(Math.Cos(Lambert_Std_Parallel_1), es_sin);
                    t1 = LAMBERT_t(Lambert_Std_Parallel_1, es_sin);
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_Std_Parallel_2));
                    m2 = LAMBERT_m(Math.Cos(Lambert_Std_Parallel_2), es_sin);
                    t2 = LAMBERT_t(Lambert_Std_Parallel_2, es_sin);
                    n = Math.Log(m1 / m2) / Math.Log(t1 / t2);
                    Lambert_lat0 = Math.Asin(n);
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_lat0));
                    m0 = LAMBERT_m(Math.Cos(Lambert_lat0), es_sin);
                    t0 = LAMBERT_t(Lambert_lat0, es_sin);
                    Lambert_k0 = m1 / m0 * Math.Pow(t0 / t1, n);
                    const_value = Lambert_a * m2 / (n * Math.Pow(t2, n));
                    Lambert_false_northing = const_value * Math.Pow(t_olat, n) - const_value * Math.Pow(t0, n) + Lambert_False_Northing_M;
                }
                else
                {
                    Lambert_lat0 = Lambert_Std_Parallel_1;
                    Lambert_k0 = 1d;
                    Lambert_false_northing = Lambert_False_Northing_M;
                }
                lambert1_error_Code = Set_LAMBCC1_Parameters(Lambert_a, Lambert_f, Lambert_lat0, Lambert_Origin_Long, Lambert_False_Easting, Lambert_false_northing, Lambert_k0);
                if (lambert1_error_Code != LAMBERT_1.NO_ERROR)
                {
                    if ((lambert1_error_Code & LAMBERT_1.A_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.A_ERROR;
                    }
                    if ((lambert1_error_Code & LAMBERT_1.INV_F_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.INV_F_ERROR;
                    }
                    if ((lambert1_error_Code & LAMBERT_1.ORIGIN_LAT_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.ORIGIN_LAT_ERROR;
                    }
                    if ((lambert1_error_Code & LAMBERT_1.CENT_MER_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.CENT_MER_ERROR;
                    }
                    if ((lambert1_error_Code & LAMBERT_1.SCALE_FACTOR_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.SCALE_FACTOR_ERROR;
                    }
                }
            }
            return Error_Code;
        }
        /// <summary> The function Convert_Lambert_2_To_Geodetic converts Lambert Conformal Conic (2 parallel) projection (easting and northing) coordinates to 
        /// Geodetic (latitude and longitude) coordinates, according to the current ellipsoid and Lambert Conformal Conic (2 parallel) projection parameters. </summary>
        /// <param name="PointGrille"> PointGrille à convertir </param>
        /// <param name="PointLatLon"> résultat de la conversion </param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_2.NO_ERROR is returned.</returns>
        private static LAMBERT_2 Convert_LAMBCC2_To_Geodetic(PointD PointGrille, ref PointD PointLatLon)
        {
            LAMBERT_1 lambert1_error_Code;
            var Error_Code = default(LAMBERT_2);
            if (PointGrille.X < Lambert_False_Easting - Lambert_Delta_Easting || PointGrille.X > Lambert_False_Easting + Lambert_Delta_Easting)
            {
                Error_Code |= LAMBERT_2.EASTING_ERROR;
            }
            if (PointGrille.Y < Lambert_false_northing - Lambert_Delta_Northing || PointGrille.Y > Lambert_false_northing + Lambert_Delta_Northing)
            {
                Error_Code |= LAMBERT_2.NORTHING_ERROR;
            }
            if (Error_Code == LAMBERT_2.NO_ERROR)
            {
                lambert1_error_Code = Convert_LAMBCC1_To_Geodetic(PointGrille, ref PointLatLon);
                if (lambert1_error_Code != LAMBERT_1.NO_ERROR)
                {
                    if ((lambert1_error_Code & LAMBERT_1.EASTING_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.EASTING_ERROR;
                    }
                    if ((lambert1_error_Code & LAMBERT_1.NORTHING_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.NORTHING_ERROR;
                    }
                }
            }
            return Error_Code;
        }
        /// <summary>The function Convert_Geodetic_To_Lambert_2 converts Geodetic (latitude and longitude) coordinates to Lambert Conformal Conic (2 parallel)
        /// projection (eastingand northing) coordinates, according to the current ellipsoid and Lambert Conformal Conic (2 parallel) projection parameters.</summary>
        /// <param name="PointLatLon"> PointLatLon à convertir </param>
        /// <param name="PointGrille"> résultat de la convertion</param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_2.NO_ERROR is returned.</returns>
        private static LAMBERT_2 Convert_Geodetic_To_LAMBCC2(PointD PointLatLon, ref PointD PointGrille)
        {
            LAMBERT_1 lambert1_error_Code;
            var Error_Code = default(LAMBERT_2);
            if (PointLatLon.Lat < -Pi_2 || PointLatLon.Lat > Pi_2)
            {
                Error_Code |= LAMBERT_2.LAT_ERROR;
            }
            if (PointLatLon.Lon < -PI || PointLatLon.Lon > TwoPI)
            {
                Error_Code |= LAMBERT_2.LON_ERROR;
            }
            if (Error_Code == LAMBERT_2.NO_ERROR)
            {
                lambert1_error_Code = Convert_Geodetic_To_LAMBCC1(PointLatLon, ref PointGrille);
                if (lambert1_error_Code != LAMBERT_1.NO_ERROR)
                {
                    if ((lambert1_error_Code & LAMBERT_1.LAT_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.LAT_ERROR;
                    }
                    if ((lambert1_error_Code & LAMBERT_1.LON_ERROR) != 0)
                    {
                        Error_Code |= LAMBERT_2.LON_ERROR;
                    }
                }
            }
            return Error_Code;
        }
        #endregion
    }
}