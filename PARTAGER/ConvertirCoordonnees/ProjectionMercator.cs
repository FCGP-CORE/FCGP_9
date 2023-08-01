using static FCGP.ConvertirCoordonnees;


namespace FCGP.Coordonnees
{
    /// <summary> This component provides conversions between geodetic coordinates  (latitude and longitudes) and Universal Transverse Mercator (UTM)
    /// projection (zone, hemisphere, easting, and northing) coordinates.  (latitude and longitudes) and Universal Transverse Mercator (UTM) projection (zone, hemisphere, easting, and northing) coordinates.
    /// UTM is intended for reuse by any application that performs a Universal Transverse Mercator (UTM) projection or its inverse traduit du projet Geotrans écrit en C </summary>
    internal static class ProjectionMercator
    {
        #region Constantes et Variables
        private const double MAX_LAT = Deg_Rad * 89.99d; // ((PI * 89.99) / 180)
        private const double MAX_DELTA_LONG = Deg_Rad * 90.0d; // ((PI * 90) / 180)
        private const double MIN_SCALE_FACTOR = 0.3d;
        private const double MAX_SCALE_FACTOR = 3.0d;
        private static double TranMerc_a = RayonWGS84;
        private static double TranMerc_f = F_WGS84; // 1 / 298.257223563
        private static double TranMerc_es = 66_943_799_901_413_800.0d;
        private static double TranMerc_ebs = 67_394_967_565_869.0d;
        private static double TranMerc_Origin_Lat;
        private static double TranMerc_Origin_Long;
        private static double TranMerc_False_Northing;
        private static double TranMerc_False_Easting;
        private static double TranMerc_Scale_Factor = 1.0d;
        private static double TranMerc_ap = 6_367_449.1458008d;
        private static double TranMerc_bp = 16_038.508696861d;
        private static double TranMerc_cp = 16.832613334334d;
        private static double TranMerc_dp = 21_984_404_273_757.0d;
        private static double TranMerc_ep = 0.000031148371319283d;
        private static double TranMerc_Delta_Easting = 40_000_000.0d;
        private static double TranMerc_Delta_Northing = 40_000_000.0d;

        /// <summary>Liste des différentes erreurs pouvant survenir lors de la transformation des coordonnées Mercator </summary>
        [Flags]
        internal enum TRANMERC : int
        {
            /// <summary>Flag OK</summary>"
            NO_ERROR = 0,
            /// <summary>Flag erreur concernant la Latitude</summary>"
            LAT_ERROR = 1,
            /// <summary>Flag erreur concernant la Longitude</summary>"
            LON_ERROR = 2,
            /// <summary>Flag erreur concernant les abscisses(Easting ou X)</summary>"
            EASTING_ERROR = 4,
            /// <summary>Flag erreur concernant les ordonnées(Northing ou Y)</summary>"
            NORTHING_ERROR = 8,
            /// <summary>Flag erreur concernant l'origine de la Latitude</summary>"
            ORIGIN_LAT_ERROR = 16,
            /// <summary>Flag erreur concernant l'origine de la Longitude</summary>"
            CENT_MER_ERROR = 32,
            /// <summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>"
            A_ERROR = 64,
            /// <summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>"
            INV_F_ERROR = 128,
            /// <summary>Flag erreur concernant le facteur d'échelle</summary>"
            SCALE_FACTOR_ERROR = 256,
            /// <summary>Flag erreur sur la distrosion de la Longitude</summary>"
            LON_WARNING = 512
        }
        #endregion
        #region Functions and Procedures
        private static double SPHTMD(double Latitude)
        {
            return TranMerc_ap * Latitude - TranMerc_bp * Math.Sin(2.0d * Latitude) + TranMerc_cp * Math.Sin(4.0d * Latitude) -
                   TranMerc_dp * Math.Sin(6.0d * Latitude) + TranMerc_ep * Math.Sin(8.0d * Latitude);
        }
        private static double SPHSN(double Latitude)
        {
            return TranMerc_a / Math.Sqrt(1.0d - TranMerc_es * Math.Pow(Math.Sin(Latitude), 2.0d));
        }
        private static double SPHSR(double Latitude)
        {
            return TranMerc_a * (1.0d - TranMerc_es) / Math.Pow(DENOM(Latitude), 3.0d);
        }
        private static double DENOM(double Latitude)
        {
            return Math.Sqrt(1.0d - TranMerc_es * Math.Pow(Math.Sin(Latitude), 2.0d));
        }
        /// <summary>"The function Convert_Geodetic_To_Transverse_Mercator converts geodetic
        /// (latitude and longitude) coordinates to Transverse Mercator projection
        /// (easting and northing) coordinates, according to the current ellipsoid
        /// and Transverse Mercator projection coordinates.</summary>
        /// <param name="PointLatLon"> PointLatLon en rad à convertir </param>
        /// <param name="PointUtm"> résultat de la conversion </param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise TRANMERC.NO_ERROR is returned.</returns>
        internal static TRANMERC Convert_Geodetic_To_Transverse_Mercator(PointD PointLatLon, ref PointD PointUtm)
        {
            double c;
            double c2;
            double c3;
            double c5;
            double c7;
            double dlam;
            double eta;
            double eta2;
            double eta3;
            double eta4;
            double s;
            double sn;
            double t;
            double tan2;
            double tan3;
            double tan4;
            double tan5;
            double tan6;
            double t1;
            double t2;
            double t3;
            double t4;
            double t5;
            double t6;
            double t7;
            double t8;
            double t9;
            double tmd;
            double tmdo;
            var Error_Code = default(TRANMERC);
            double temp_Origin;
            double temp_Long;
            if (PointLatLon.Lat < -MAX_LAT || PointLatLon.Lat > MAX_LAT)
            {
                Error_Code |= TRANMERC.LAT_ERROR;
            }
            if (PointLatLon.Lon > PI)
            {
                PointLatLon.Lon -= TwoPI;
            }
            if (PointLatLon.Lon < TranMerc_Origin_Long - MAX_DELTA_LONG || PointLatLon.Lon > TranMerc_Origin_Long + MAX_DELTA_LONG)
            {
                if (PointLatLon.Lon < 0d)
                {
                    temp_Long = PointLatLon.Lon + TwoPI;
                }
                else
                {
                    temp_Long = PointLatLon.Lon;
                }
                if (TranMerc_Origin_Long < 0d)
                {
                    temp_Origin = TranMerc_Origin_Long + TwoPI;
                }
                else
                {
                    temp_Origin = TranMerc_Origin_Long;
                }
                if (temp_Long < temp_Origin - MAX_DELTA_LONG || temp_Long > temp_Origin + MAX_DELTA_LONG)
                {
                    Error_Code |= TRANMERC.LON_ERROR;
                }
            }
            if (Error_Code == TRANMERC.NO_ERROR)
            {
                dlam = PointLatLon.Lon - TranMerc_Origin_Long;
                if (Math.Abs(dlam) > 9d * Deg_Rad)
                {
                    Error_Code |= TRANMERC.LON_WARNING;
                }
                if (dlam > PI)
                {
                    dlam -= TwoPI;
                }
                if (dlam < -PI)
                {
                    dlam += TwoPI;
                }
                if (Math.Abs(dlam) < 0.0000000002d)
                {
                    dlam = 0d;
                }
                s = Math.Sin(PointLatLon.Lat);
                c = Math.Cos(PointLatLon.Lat);
                c2 = c * c;
                c3 = c2 * c;
                c5 = c3 * c2;
                c7 = c5 * c2;
                t = Math.Tan(PointLatLon.Lat);
                tan2 = t * t;
                tan3 = tan2 * t;
                tan4 = tan3 * t;
                tan5 = tan4 * t;
                tan6 = tan5 * t;
                eta = TranMerc_ebs * c2;
                eta2 = eta * eta;
                eta3 = eta2 * eta;
                eta4 = eta3 * eta;
                sn = ProjectionMercator.SPHSN(PointLatLon.Lat);
                tmd = ProjectionMercator.SPHTMD(PointLatLon.Lat);
                tmdo = SPHTMD(TranMerc_Origin_Lat);
                t1 = (tmd - tmdo) * TranMerc_Scale_Factor;
                t2 = sn * s * c * TranMerc_Scale_Factor / 2d;
                t3 = sn * s * c3 * TranMerc_Scale_Factor * (5d - tan2 + 9d * eta + 4d * eta2) / 24d;
                t4 = sn * s * c5 * TranMerc_Scale_Factor * (61d - 58d * tan2 + tan4 + 270d * eta - 330d * tan2 * eta + 445d * eta2 + 324d * eta3 -
                                                            680d * tan2 * eta2 + 88d * eta4 - 600d * tan2 * eta3 - 192d * tan2 * eta4) / 720d;
                t5 = sn * s * c7 * TranMerc_Scale_Factor * (1385d - 3111d * tan2 + 543d * tan4 - tan6) / 40320d;
                PointUtm.Y = TranMerc_False_Northing + t1 + Math.Pow(dlam, 2d) * t2 + Math.Pow(dlam, 4d) * t3 + Math.Pow(dlam, 6d) * t4 + Math.Pow(dlam, 8d) * t5;
                t6 = sn * c * TranMerc_Scale_Factor;
                t7 = sn * c3 * TranMerc_Scale_Factor * (1d - tan2 + eta) / 6d;
                t8 = sn * c5 * TranMerc_Scale_Factor * (5d - 18d * tan2 + tan4 + 14d * eta - 58d * tan2 * eta + 13d * eta2 + 4d * eta3 - 64d * tan2 * eta2 - 24d * tan2 * eta3) / 120d;
                t9 = sn * c7 * TranMerc_Scale_Factor * (61d - 479d * tan2 + 179d * tan4 - tan6) / 5040d;
                PointUtm.X = TranMerc_False_Easting + dlam * t6 + Math.Pow(dlam, 3d) * t7 + Math.Pow(dlam, 5d) * t8 + Math.Pow(dlam, 7d) * t9;
            }
            return Error_Code;
        }
        /// <summary>The function Convert_Transverse_Mercator_To_Geodetic converts Transverse
        /// Mercator projection (easting and northing) coordinates to geodetic
        /// (latitude and longitude) coordinates, according to the current ellipsoid
        /// and Transverse Mercator projection parameters.</summary>
        /// <param name="PointUtm"> Point à convertir </param>
        /// <param name="PointLatLon"> résultat de la conversion</param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise TRANMERC.NO_ERROR is returned.</returns>
        internal static TRANMERC Convert_Transverse_Mercator_To_Geodetic(PointD PointUtm, ref PointD PointLatLon)
        {
            double c;
            double de;
            double dlam;
            double eta;
            double eta2;
            double eta3;
            double eta4;
            double ftphi;
            int i;
            //double s;
            double sn;
            double sr;
            double t;
            double tan2;
            double tan4;
            double t10;
            double t11;
            double t12;
            double t13;
            double t14;
            double t15;
            double t16;
            double t17;
            double tmd;
            double tmdo;
            var Error_Code = default(TRANMERC);
            if (PointUtm.X < TranMerc_False_Easting - TranMerc_Delta_Easting || PointUtm.X > TranMerc_False_Easting + TranMerc_Delta_Easting)
            {
                Error_Code |= TRANMERC.EASTING_ERROR;
            }
            if (PointUtm.Y < TranMerc_False_Northing - TranMerc_Delta_Northing || PointUtm.Y > TranMerc_False_Northing + TranMerc_Delta_Northing)
            {
                Error_Code |= TRANMERC.NORTHING_ERROR;
            }
            if (Error_Code == TRANMERC.NO_ERROR)
            {
                tmdo = SPHTMD(TranMerc_Origin_Lat);
                tmd = tmdo + (PointUtm.Y - TranMerc_False_Northing) / TranMerc_Scale_Factor;
                sr = SPHSR(0d);
                ftphi = tmd / sr;
                for (i = 0; i <= 4; i++)
                {
                    t10 = SPHTMD(ftphi);
                    sr = SPHSR(ftphi);
                    ftphi += (tmd - t10) / sr;
                }
                sr = SPHSR(ftphi);
                sn = SPHSN(ftphi);
                //s = Math.Sin(ftphi);
                c = Math.Cos(ftphi);
                t = Math.Tan(ftphi);
                tan2 = t * t;
                tan4 = tan2 * tan2;
                eta = TranMerc_ebs * Math.Pow(c, 2d);
                eta2 = eta * eta;
                eta3 = eta2 * eta;
                eta4 = eta3 * eta;
                de = PointUtm.X - TranMerc_False_Easting;
                if (Math.Abs(de) < 1d)
                {
                    de = 0d;
                }
                t10 = t / (2d * sr * sn * Math.Pow(TranMerc_Scale_Factor, 2d));
                t11 = t * (5d + 3d * tan2 + eta - 4d * Math.Pow(eta, 2d) - 9d * tan2 * eta) / (24d * sr * Math.Pow(sn, 3d) * Math.Pow(TranMerc_Scale_Factor, 4d));
                t12 = t * (61d + 90d * tan2 + 46d * eta + 45d * tan4 - 252d * tan2 * eta - 3d * eta2 + 100d * eta3 - 66d * tan2 * eta2 - 90d * tan4 * eta +
                           88d * eta4 + 225d * tan4 * eta2 + 84d * tan2 * eta3 - 192d * tan2 * eta4) / (720d * sr * Math.Pow(sn, 5d) * Math.Pow(TranMerc_Scale_Factor, 6d));
                t13 = t * (1385d + 3633d * tan2 + 4095d * tan4 + 1575d * Math.Pow(t, 6d)) / (40320d * sr * Math.Pow(sn, 7d) * Math.Pow(TranMerc_Scale_Factor, 8d));
                PointLatLon.Lat = ftphi - Math.Pow(de, 2d) * t10 + Math.Pow(de, 4d) * t11 - Math.Pow(de, 6d) * t12 + Math.Pow(de, 8d) * t13;
                t14 = 1d / (sn * c * TranMerc_Scale_Factor);
                t15 = (1d + 2d * tan2 + eta) / (6d * Math.Pow(sn, 3d) * c * Math.Pow(TranMerc_Scale_Factor, 3d));
                t16 = (5d + 6d * eta + 28d * tan2 - 3d * eta2 + 8d * tan2 * eta + 24d * tan4 - 4d * eta3 + 4d * tan2 * eta2 + 24d * tan2 * eta3) /
                      (120d * Math.Pow(sn, 5d) * c * Math.Pow(TranMerc_Scale_Factor, 5d));
                t17 = (61d + 662d * tan2 + 1320d * tan4 + 720d * Math.Pow(t, 6d)) / (5040d * Math.Pow(sn, 7d) * c * Math.Pow(TranMerc_Scale_Factor, 7d));
                dlam = de * t14 - Math.Pow(de, 3d) * t15 + Math.Pow(de, 5d) * t16 - Math.Pow(de, 7d) * t17;
                PointLatLon.Lon = TranMerc_Origin_Long + dlam;
                if (Math.Abs(PointLatLon.Lat) > 90d * Deg_Rad) // If Math.Abs(Latitude) > (90 * PI / 180) Then
                {
                    Error_Code |= TRANMERC.NORTHING_ERROR;
                }
                if (PointLatLon.Lon > PI)
                {
                    PointLatLon.Lon -= TwoPI;
                    if (Math.Abs(PointLatLon.Lon) > PI)
                    {
                        Error_Code |= TRANMERC.EASTING_ERROR;
                    }
                }
                if (PointLatLon.Lat > 10000000000d)
                {
                    Error_Code |= TRANMERC.LON_WARNING;
                }
            }
            return Error_Code;
        }
        /// <summary>The function Set_Tranverse_Mercator_Parameters receives the ellipsoid parameters and
        /// Tranverse Mercator projection parameters as inputs, andsets the corresponding state variables. </summary>
        /// <param name="a">Semi-major axis of ellipsoid, in meters(input)</param>
        /// <param name="f">Flattening of ellipsoid(input)</param>
        /// <param name="Origin_Latitude">Latitude in radians at the origin of the projection(input)</param>
        /// <param name="Central_Meridian">Longitude in radians at the center of the projection(input)</param>
        /// <param name="False_Easting">Easting/X at the center of the projection(input)</param>
        /// <param name="False_Northing">Northing/Y at the center of the projection(input)</param>
        /// <param name="Scale_Factor">Projection scale factor(input)</param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise TRANMERC.NO_ERROR is returned.</returns>
        internal static TRANMERC Set_Transverse_Mercator_Parameters(double a, double f, double Origin_Latitude, double Central_Meridian,
                                                                    double False_Easting, double False_Northing, double Scale_Factor)
        {
            double tn;
            double tn2;
            double tn3;
            double tn4;
            double tn5;
            double dummy_northing = 0.0d;
            double TranMerc_b;
            double inv_f = 1.0d / f;
            var Error_Code = default(TRANMERC);
            if (a <= 0d)
            {
                Error_Code |= TRANMERC.A_ERROR;
            }
            if (inv_f < 250d || inv_f > 350d)
            {
                Error_Code |= TRANMERC.INV_F_ERROR;
            }
            if (Origin_Latitude < -Pi_2 || Origin_Latitude > Pi_2)
            {
                Error_Code |= TRANMERC.ORIGIN_LAT_ERROR;
            }
            if (Central_Meridian < -PI || Central_Meridian > 2d * PI)
            {
                Error_Code |= TRANMERC.CENT_MER_ERROR;
            }
            if (Scale_Factor < MIN_SCALE_FACTOR || Scale_Factor > MAX_SCALE_FACTOR)
            {
                Error_Code |= TRANMERC.SCALE_FACTOR_ERROR;
            }
            if (Error_Code == TRANMERC.NO_ERROR)
            {
                TranMerc_a = a;
                TranMerc_f = f;
                TranMerc_Origin_Lat = 0.0d;
                TranMerc_Origin_Long = 0.0d;
                TranMerc_False_Northing = 0.0d;
                TranMerc_False_Easting = 0.0d;
                TranMerc_Scale_Factor = 1.0d;
                TranMerc_es = 2.0d * TranMerc_f - TranMerc_f * TranMerc_f;
                TranMerc_ebs = 1.0d / (1.0d - TranMerc_es) - 1.0d;
                TranMerc_b = TranMerc_a * (1.0d - TranMerc_f);
                tn = (TranMerc_a - TranMerc_b) / (TranMerc_a + TranMerc_b);
                tn2 = tn * tn;
                tn3 = tn2 * tn;
                tn4 = tn3 * tn;
                tn5 = tn4 * tn;
                TranMerc_ap = TranMerc_a * (1d - tn + 5d * (tn2 - tn3) / 4d + 81d * (tn4 - tn5) / 64d);
                TranMerc_bp = 3d * TranMerc_a * (tn - tn2 + 7d * (tn3 - tn4) / 8d + 55d * tn5 / 64d) / 2d;
                TranMerc_cp = 15d * TranMerc_a * (tn2 - tn3 + 3d * (tn4 - tn5) / 4d) / 16d;
                TranMerc_dp = 35d * TranMerc_a * (tn3 - tn4 + 11d * tn5 / 16d) / 48d;
                TranMerc_ep = 315d * TranMerc_a * (tn4 - tn5) / 512d;
                var Ret = new PointD(TranMerc_Delta_Easting, TranMerc_Delta_Northing);
                Convert_Geodetic_To_Transverse_Mercator(new PointD(MAX_LAT, MAX_DELTA_LONG), ref Ret);
                TranMerc_Delta_Northing = Ret.Y;
                Ret.Y = dummy_northing;
                Convert_Geodetic_To_Transverse_Mercator(new PointD(0d, MAX_DELTA_LONG), ref Ret);
                TranMerc_Delta_Easting = Ret.X;
                TranMerc_Origin_Lat = Origin_Latitude;
                if (Central_Meridian > PI)
                {
                    Central_Meridian -= TwoPI;
                }
                TranMerc_Origin_Long = Central_Meridian;
                TranMerc_False_Northing = False_Northing;
                TranMerc_False_Easting = False_Easting;
                TranMerc_Scale_Factor = Scale_Factor;
            }
            return Error_Code;
        }
        #endregion
    }
}