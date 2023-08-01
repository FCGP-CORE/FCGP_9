using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;

namespace FCGP.Coordonnees
{
    /// <summary> fait la conversion géocentric d'un datum vers wgs84 ou inversement (3 paramètres et pas 7).  
    /// Pour le changement d'un système vers un autre traduit du projet Geotrans écrit en C </summary>
    internal static class ProjectionCartesienne
    {
        #region Haut niveau
        /// <summary> convertit une coordonnées exprimée en DD d'un datum une coordonnées exprimée en DD d'un autre datum</summary>
        /// <param name="DatumDepart"> datum de départ. Ellipsoide obligatoire </param>
        /// <param name="PointLatLon"> latitude-longitude de départ en dd</param>
        /// <param name="DatumArrivee"> datum d'arrivée. Ellipsoide obligatoire </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertDatumToDatum(Datums DatumDepart, PointD PointLatLon, Datums DatumArrivee, bool Deg = true)
        {
            double X_Depart = default, Y_Depart = default, Z_Depart = default, X_Arrivee = default, Y_Arrivee = default, Z_Arrivee = default;
            try
            {
                if (PARAM_DATUMS.Ellipsoide != PARAM_DATUMS.ParametresDatums[(int)DatumDepart].Ellipsoide)
                {
                    PARAM_DATUMS.SetParametresDatum(DatumDepart);
                }
                // passe de degrés en radians si besoin
                if (Deg)
                    PointLatLon.Scale(Deg_Rad);
                // transforme les DD du datum de départe en X,Y,Z du datum de départ
                Convert_Geographiques_To_Cartesiennes(PointLatLon, ref X_Depart, ref Y_Depart, ref Z_Depart);
                // tranforme les X, Y, Z du datum de depart en X, Y, Z du datum d'arrivée
                Cartesiennes_DatumDepart_To_DatumArrivee(DatumArrivee, X_Depart, Y_Depart, Z_Depart, ref X_Arrivee, ref Y_Arrivee, ref Z_Arrivee);
                // tranforme les X, Y, Z du datum d'arrivée en DD du datum d'arrivée
                var Ret = Convert_Cartesiennes_To_Geographiques(X_Arrivee, Y_Arrivee, Z_Arrivee);
                // passe de radians en degrés si besoin
                if (Deg)
                    Ret.Scale(Rad_Deg);
                return Ret;
            }
            catch
            {
                return PointD.Empty;
            }
        }
        #endregion
        #region constantes
        private const double AD_C = 1.0026d;
        /// <summary>Donne différentes erreur pouvant survenir lors de la transformation des coordonnées</summary>
        [Flags]
        private enum GEOCENT : int
        {
            /// <summary>Flag OK</summary>"
            NO_ERROR = 0,
            /// <summary>Flag erreur concernant la Latitude</summary>
            LAT_ERROR = 1,
            /// <summary>Flag erreur concernant la Longitude</summary>
            LON_ERROR = 2,
            /// <summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 4,
            /// <summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 8
        }
        #endregion
        #region Fonctions et procédures
        /// <summary> Convertit coordonnées latitude, longitude en coordonnées cartésiennes X,Y,Z </summary>
        /// <param name="PointLatLon">Geodetic latitude in radians(input)</param>
        /// <param name="X">Calculated Geocentric X coordinate, in meters(output)</param>
        /// <param name="Y">Calculated Geocentric Y coordinate, in meters(output)</param>
        /// <param name="Z">Calculated Geocentric Z coordinate, in meters(output)</param>
        /// <returns>If any errors occur, the error code(s) are returned by the function, otherwise GEOCENT.NO_ERROR is returned.</returns>
        private static GEOCENT Convert_Geographiques_To_Cartesiennes(PointD PointLatLon, ref double X, ref double Y, ref double Z)
        {
            double Geocent_a = PARAM_DATUMS.A;
            double Geocent_e2 = PARAM_DATUMS.E2;
            var Error_Code = default(GEOCENT);
            double Rn;
            double Sin_Lat;
            double Sin2_Lat;
            double Cos_Lat;
            if (PointLatLon.Lat < -Pi_2 || PointLatLon.Lat > Pi_2)
            {
                Error_Code |= GEOCENT.LAT_ERROR;
            }
            if (PointLatLon.Lon < -PI || PointLatLon.Lon > TwoPI)
            {
                Error_Code |= GEOCENT.LON_ERROR;
            }
            if (Error_Code == GEOCENT.NO_ERROR)
            {
                if (PointLatLon.Lon > PI)
                {
                    PointLatLon.Lon -= TwoPI;
                }
                Sin_Lat = Math.Sin(PointLatLon.Lat);
                Cos_Lat = Math.Cos(PointLatLon.Lat);
                Sin2_Lat = Sin_Lat * Sin_Lat;
                Rn = Geocent_a / Math.Sqrt(1d - Geocent_e2 * Sin2_Lat);
                X = Rn * Cos_Lat * Math.Cos(PointLatLon.Lon);
                Y = Rn * Cos_Lat * Math.Sin(PointLatLon.Lon);
                Z = Rn * (1d - Geocent_e2) * Sin_Lat;
            }
            return Error_Code;
        }
        /// <summary> Convertit des coordonnées cartésiennes X,Y,Z en coordonnées Latitude, longitude </summary>
        /// <param name="X">Geocentric X coordinate, in meters.(input)</param>
        /// <param name="Y">Geocentric Y coordinate, in meters.(input)</param>
        /// <param name="Z">Geocentric Z coordinate, in meters.(input)</param>
        /// <returns> le point LatLon </returns>
        private static PointD Convert_Cartesiennes_To_Geographiques(double X, double Y, double Z)
        {
            double Geocent_a = PARAM_DATUMS.A;
            double Geocent_e2 = PARAM_DATUMS.E2;
            double Geocent_ep2 = PARAM_DATUMS.EP2;
            double Geocent_b = PARAM_DATUMS.B;
            double W, W2, T0, T1, S0, S1, Sin_B0, Sin3_B0, Cos_B0, Cos3_B0, Sin_p1, Cos_p1, Sum;
            var At_Pole = default(bool);

            var PointLatLon = new PointD(0d, 0d);
            if (X != 0d)
            {
                PointLatLon.Lon = Math.Atan2(Y, X);
            }
            else if (Y > 0d)
            {
                PointLatLon.Lon = Pi_2;
            }
            else if (Y < 0d)
            {
                PointLatLon.Lon = -Pi_2;
            }
            else // Y=0
            {
                At_Pole = true;
                PointLatLon.Lon = 0d;
                if (Z > 0d)
                {
                    PointLatLon.Lat = Pi_2;
                }
                else if (Z < 0d)
                {
                    PointLatLon.Lat = -Pi_2;
                }
                else
                {
                    PointLatLon.Lat = Pi_2;
                    return PointLatLon;
                }
            }
            W2 = X * X + Y * Y;
            W = Math.Sqrt(W2);
            T0 = Z * AD_C;
            S0 = Math.Sqrt(T0 * T0 + W2);
            Sin_B0 = T0 / S0;
            Cos_B0 = W / S0;
            Sin3_B0 = Sin_B0 * Sin_B0 * Sin_B0;
            Cos3_B0 = Cos_B0 * Cos_B0 * Cos_B0;
            T1 = Z + Geocent_b * Geocent_ep2 * Sin3_B0;
            Sum = W - Geocent_a * Geocent_e2 * Cos3_B0; // Cos_B0 * Cos_B0 * Cos_B0
            S1 = Math.Sqrt(T1 * T1 + Sum * Sum);
            Sin_p1 = T1 / S1;    // pas nécessaire
            Cos_p1 = Sum / S1;   // pas nécessaire

            if (At_Pole == false)
            {
                PointLatLon.Lat = Math.Atan(Sin_p1 / Cos_p1);    // equivalent à Math.Atan(T1/Sum)
            }
            return PointLatLon;
        }
        /// <summary> Passe d'un ellipsoïde à l'autre en fonction des paramètres de transformation </summary>"
        /// <param name="X_Depart">X coordinate relative to the source datum (ouput)</param>
        /// <param name="Y_Depart">Y coordinate relative to the source datum (ouput)</param>
        /// <param name="Z_Depart">Z coordinate relative to the source datum (ouput)</param>
        /// <param name="X_Arrivee">X coordinate relative to WGS84 (intput)</param>
        /// <param name="Y_Arrivee">Y coordinate relative to WGS84 (intput)</param>
        /// <param name="Z_Arrivee">Z coordinate relative to WGS84 (intput)</param>
        /// <remarks>en sortie les paramètre associés au datum ont changés et correspondent à ceux du datum d'arrivée</remarks>
        private static void Cartesiennes_DatumDepart_To_DatumArrivee(Datums DatumArrivee, double X_Depart, double Y_Depart, double Z_Depart,
                                                                     ref double X_Arrivee, ref double Y_Arrivee, ref double Z_Arrivee)
        {
            if (PARAM_DATUMS.Ellipsoide == PARAM_DATUMS.ParametresDatums[(int)DatumArrivee].Ellipsoide)
            {
                // si l'ellipsoïde de départ = le l'ellipsoïde d'arrivée il n'y a pas de transformation à faire
                // Ex : UTM_ED50 to DD ED50
                X_Arrivee = X_Depart;
                Y_Arrivee = Y_Depart;
                Z_Arrivee = Z_Depart;
            }
            else if (PARAM_DATUMS.ParametresDatums[(int)DatumArrivee].Ellipsoide == Ellipsoides.WGS84)
            {
                // si l'ellipsoïde d'arrivée est le WGS84 il faut ajouter les paramètres pour la transformation
                // Ex DD ED50 vers DD WGS84
                X_Arrivee = X_Depart + PARAM_DATUMS.DX;
                Y_Arrivee = Y_Depart + PARAM_DATUMS.DY;
                Z_Arrivee = Z_Depart + PARAM_DATUMS.DZ;
                // on met les paramètres datum arrivée à disposition
                PARAM_DATUMS.SetParametresDatum(DatumArrivee);
            }
            else if (PARAM_DATUMS.Ellipsoide == Ellipsoides.WGS84)
            {
                // si l'ellipsoïde de départ est le WGS84 il faut enlever les paramètres pour la transformation
                // Ex : DD WGS84 to DD ED50
                PARAM_DATUMS.SetParametresDatum(DatumArrivee);
                X_Arrivee = X_Depart - PARAM_DATUMS.DX;
                Y_Arrivee = Y_Depart - PARAM_DATUMS.DY;
                Z_Arrivee = Z_Depart - PARAM_DATUMS.DZ;
            }
            else
            {
                // sinon si il faut passer par l'ellipsoïde WGS84 qui est le système pivot
                // Ex : DD NTF to DD ED50
                // d'abord DD NTF to DD WGS84
                X_Arrivee = X_Depart + PARAM_DATUMS.DX;
                Y_Arrivee = Y_Depart + PARAM_DATUMS.DY;
                Z_Arrivee = Z_Depart + PARAM_DATUMS.DZ;
                // puis DD WGS84 to DD ED50
                PARAM_DATUMS.SetParametresDatum(DatumArrivee);
                X_Arrivee -= PARAM_DATUMS.DX;
                Y_Arrivee -= PARAM_DATUMS.DY;
                Z_Arrivee -= PARAM_DATUMS.DZ;
            }
        }
        #endregion
    }
}