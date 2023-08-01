using static FCGP.ConvertirCoordonnees;

namespace FCGP.Coordonnees
{
    /// <summary> This component provides conversions between geodetic coordinates  (latitude and longitudes) and Universal Transverse Mercator (UTM)
    /// projection (zone, hemisphere, easting, and northing) coordinates.  (latitude and longitudes) and Universal Transverse Mercator (UTM) projection (zone, hemisphere, easting, and northing) coordinates.
    /// UTM is intended for reuse by any application that performs a Universal Transverse Mercator (UTM) projection or its inverse traduit du projet Geotrans écrit en C </summary>
    internal static class ProjectionMercatorUtm
    {
        #region Haut Niveau
        /// <summary> UTM en mètres vers ellispsoïde du datum en DD ou en Rad. </summary>
        /// <param name="PointGrille"> point à convertir </param>
        /// <param name="Datum"> datum de la projection UTM </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point convertit sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertUtmToLL(Enumerations.Datums Datum, PointProjection PointGrille, bool Deg = true)
        {
            if (PARAM_DATUMS.Ellipsoide != PARAM_DATUMS.ParametresDatums[(int)Datum].Ellipsoide)
            {
                PARAM_DATUMS.SetParametresDatum(Datum); // datum de départ
            }
            var Ret = new PointD(0d, 0d);
            if (Convert_UTM_To_Geodetic(PointGrille, ref Ret) == UTM.NO_ERROR)
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
        /// <summary> Coordonnées lat lon sur ellispsoïde du datum en DD ou en Rad vers Utm </summary>
        /// <param name="PointLatLon"> point Lat Lon à convertir </param>
        /// <param name="Datum"> Datum de la projection UTM </param>
        /// <param name="Zone"> Numéro de la zone à laquelle on veut rattacher le point à convertir. 0 = Zone par défaut </param>
        /// <param name="Deg"> indique si la coordonnées Lat Lon sont en DD </param>
        /// <returns></returns>
        internal static PointProjection ConvertLLToUtm(PointD PointLatLon, Enumerations.Datums Datum, int Zone = 0, bool Deg = true)
        {
            if (PARAM_DATUMS.Ellipsoide != PARAM_DATUMS.ParametresDatums[(int)Datum].Ellipsoide)
            {
                PARAM_DATUMS.SetParametresDatum(Datum); // datum de départ ou Nom
            }
            if (Deg)
                PointLatLon.Scale(Deg_Rad);
            var Ret = new PointProjection(new PointD(0d, 0d), Zone);
            if (Convert_Geodetic_To_UTM(PointLatLon, ref Ret) == UTM.NO_ERROR)
            {
                return Ret;
            }
            else
            {
                return new PointProjection();
            }
        }
        #endregion
        #region Constantes
        /// <summary>Liste des différentes erreurs pouvant survenir lors de la transformation des coordonnées UTM </summary>
        [Flags]
        private enum UTM : int
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
            /// <summary>Flag erreur concernant la zone UTM</summary>
            ZONE_ERROR = 16,
            /// <summary>Flag erreur concernant l'hémisphère</summary>
            HEMISPHERE_ERROR = 32,
            /// <summary>Flag erreur concernant la zone override</summary>
            ZONE_OVERRIDE_ERROR = 64,
            /// <summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 128,
            /// <summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 256
        }
        #endregion
        #region Functions and Procedures
        /// <summary>  La fonction Convert_Geodetic_To_UTM convertit des coordonnées géodétic (Longitude et Latitude)
        /// en coordonnées UTM (Zone, Hémisphère, X et Y) en fonction de l'éllipsoïde en cours et de la zone éventuellement demandée</summary>
        /// <param name="PointLatLon">Latitude en radians(entrée)</param>
        /// <param name="PointUtm">PointUtm</param>
        /// <returns> If any errors occur, the error code(s) are returned by the function, otherwise UTM.NO_ERROR is returned. </returns>
        private static UTM Convert_Geodetic_To_UTM(PointD PointLatLon, ref PointProjection PointUtm)
        {
            const double MIN_LAT = Deg_Rad * -80.5d;
            const double MAX_LAT = Deg_Rad * 84.5d;
            const int MIN_EASTING = -1350000;
            const int MAX_EASTING = 2200000;
            const int MIN_NORTHING = 0;
            const int MAX_NORTHING = 10000000;
            int Lat_Degrees, Long_Degrees, temp_zone;
            var Error_Code = default(UTM);
            var Origin_Latitude = default(double);
            double Central_Meridian;
            double False_Easting = 500000.0d;
            var False_Northing = default(double);
            double Scale = 0.9996d;
            double UTM_a = PARAM_DATUMS.A;
            double UTM_f = PARAM_DATUMS.F;
            var UTM_Override = default(int);
            if (PointLatLon.Lat < MIN_LAT || PointLatLon.Lat > MAX_LAT)
            {
                Error_Code |= UTM.LAT_ERROR;
            }
            if (PointLatLon.Lon < -PI || PointLatLon.Lon > TwoPI)
            {
                Error_Code |= UTM.LON_ERROR;
            }
            if (Error_Code == UTM.NO_ERROR)
            {
                if (PointLatLon.Lon < 0d)
                {
                    PointLatLon.Lon += TwoPI + 0.0000000001d;
                }
                Lat_Degrees = (int)Math.Round(PointLatLon.Lat * Rad_Deg);
                Long_Degrees = (int)Math.Round(PointLatLon.Lon * Rad_Deg);
                if (PointLatLon.Lon < PI)
                {
                    temp_zone = 31 + (int)Math.Floor(PointLatLon.Lon * Rad_Deg) / 6;
                }
                else
                {
                    temp_zone = (int)Math.Floor(PointLatLon.Lon * Rad_Deg) / 6 - 29;
                }
                if (temp_zone > 60)
                {
                    temp_zone = 1;
                }
                if (Lat_Degrees > 55 && Lat_Degrees < 64 && Long_Degrees > -1 && Long_Degrees < 3)
                {
                    temp_zone = 31;
                }
                if (Lat_Degrees > 55 && Lat_Degrees < 64 && Long_Degrees > 2 && Long_Degrees < 12)
                {
                    temp_zone = 32;
                }
                if (Lat_Degrees > 71 && Long_Degrees > -1 && Long_Degrees < 9)
                {
                    temp_zone = 31;
                }
                if (Lat_Degrees > 71 && Long_Degrees > 8 && Long_Degrees < 21)
                {
                    temp_zone = 33;
                }
                if (Lat_Degrees > 71 && Long_Degrees > 20 && Long_Degrees < 33)
                {
                    temp_zone = 35;
                }
                if (Lat_Degrees > 71 && Long_Degrees > 32 && Long_Degrees < 42)
                {
                    temp_zone = 37;
                }
                if (UTM_Override != 0)
                {
                    if (temp_zone == 1 && UTM_Override == 60)
                    {
                        temp_zone = UTM_Override;
                    }
                    else if (temp_zone == 60 && UTM_Override == 1)
                    {
                        temp_zone = UTM_Override;
                    }
                    else if (temp_zone - 1 <= UTM_Override && UTM_Override <= temp_zone + 1)
                    {
                        temp_zone = UTM_Override;
                    }
                    else
                    {
                        Error_Code = UTM.ZONE_OVERRIDE_ERROR;
                    }
                }
                if (PointUtm.Zone != 0)
                    temp_zone = PointUtm.Zone;
                if (Error_Code == UTM.NO_ERROR)
                {
                    if (temp_zone >= 31)
                    {
                        Central_Meridian = (6 * temp_zone - 183) * Deg_Rad;
                    }
                    else
                    {
                        Central_Meridian = (6 * temp_zone + 177) * Deg_Rad;
                    }
                    PointUtm.Zone = temp_zone;
                    if (PointLatLon.Lat < 0d)
                    {
                        False_Northing = 10000000d;
                        PointUtm.Hemisphere = 'S';
                    }
                    else
                    {
                        PointUtm.Hemisphere = 'N';
                    }
                    _ = ProjectionMercator.Set_Transverse_Mercator_Parameters(UTM_a, UTM_f, Origin_Latitude, Central_Meridian, False_Easting, False_Northing, Scale);
                    _ = ProjectionMercator.Convert_Geodetic_To_Transverse_Mercator(PointLatLon, ref PointUtm.Coordonnees);
                    if (PointUtm.Coordonnees.X < (double)MIN_EASTING || PointUtm.Coordonnees.X > (double)MAX_EASTING)
                    {
                        Error_Code = UTM.EASTING_ERROR;
                    }
                    if (PointUtm.Coordonnees.Y < (double)MIN_NORTHING || PointUtm.Coordonnees.Y > (double)MAX_NORTHING)
                    {
                        Error_Code |= UTM.NORTHING_ERROR;
                    }
                }
            }
            return Error_Code;
        }
        /// <summary>The function Convert_UTM_To_Geodetic converts UTM projection (zone, hemisphere, easting and northing) coordinates to geodetic(latitude
        /// and  longitude) coordinates, according to the current ellipsoid parameters.</summary>
        /// <param name="PointUtm"> point à convertir </param>
        /// <param name="PointLatLon"> résultat de la conversion </param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise UTM.NO_ERROR is returned.</returns>
        private static UTM Convert_UTM_To_Geodetic(PointProjection PointUtm, ref PointD PointLatLon)
        {
            const double MIN_LAT = Deg_Rad * -80.5d;
            const double MAX_LAT = Deg_Rad * 84.5d;
            const int MIN_EASTING = -1_350_000;
            const int MAX_EASTING = 2_200_000;
            const int MIN_NORTHING = 0;
            const int MAX_NORTHING = 10_000_000;
            double UTM_a = PARAM_DATUMS.A;
            double UTM_f = PARAM_DATUMS.F;
            var Error_Code = default(UTM);
            ProjectionMercator.TRANMERC tm_error_code;
            var Origin_Latitude = default(double);
            double Central_Meridian;
            double False_Easting = 500000.0d;
            var False_Northing = default(double);
            double Scale = 0.9996d;
            if (PointUtm.Zone < 1 || PointUtm.Zone > 60)
            {
                Error_Code |= UTM.ZONE_ERROR;
            }
            if (PointUtm.Hemisphere != 'S' && PointUtm.Hemisphere != 'N')
            {
                Error_Code |= UTM.HEMISPHERE_ERROR;
            }
            if (PointUtm.Coordonnees.X < (double)MIN_EASTING || PointUtm.Coordonnees.X > (double)MAX_EASTING)
            {
                Error_Code |= UTM.EASTING_ERROR;
            }
            if (PointUtm.Coordonnees.Y < (double)MIN_NORTHING || PointUtm.Coordonnees.Y > (double)MAX_NORTHING)
            {
                Error_Code |= UTM.NORTHING_ERROR;
            }
            if (Error_Code == UTM.NO_ERROR)
            {
                if (PointUtm.Zone >= 31)
                {
                    Central_Meridian = (double)(6 * PointUtm.Zone - 183) * Deg_Rad;
                }
                else
                {
                    Central_Meridian = (double)(6 * PointUtm.Zone + 177) * Deg_Rad;
                }
                if (PointUtm.Hemisphere == 'S')
                {
                    False_Northing = 10000000d;
                }
                ProjectionMercator.Set_Transverse_Mercator_Parameters(UTM_a, UTM_f, Origin_Latitude, Central_Meridian, False_Easting, False_Northing, Scale);

                tm_error_code = ProjectionMercator.Convert_Transverse_Mercator_To_Geodetic(PointUtm.Coordonnees, ref PointLatLon);

                if (tm_error_code != ProjectionMercator.TRANMERC.NO_ERROR)
                {
                    if ((tm_error_code & ProjectionMercator.TRANMERC.EASTING_ERROR) != 0 || (tm_error_code & ProjectionMercator.TRANMERC.LON_WARNING) != 0)
                    {
                        Error_Code |= UTM.EASTING_ERROR;
                    }
                    if ((tm_error_code & ProjectionMercator.TRANMERC.NORTHING_ERROR) != 0)
                    {
                        Error_Code |= UTM.NORTHING_ERROR;
                    }
                }
                if (PointLatLon.Lat < MIN_LAT || PointLatLon.Lat > MAX_LAT)
                {
                    Error_Code |= UTM.NORTHING_ERROR;
                }
            }
            return Error_Code;
        }
        #endregion
    }
}