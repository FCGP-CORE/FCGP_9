using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;


namespace FCGP.Coordonnees
{
    /// <summary> Summary description for ApproxSwissProj.  trouvée sur le site Swiss Topo </summary>
    internal static class ProjectionSuisse
    {
        private const int DELTA_X_LV95 = 2_000_000;
        private const int DELTA_Y_LV95 = 1_000_000;
        private const int DELTA_X_LV03 = 600_000;
        private const int DELTA_Y_LV03 = 200_000;
        private const double SecToDeg = 100.0d / 36.0d;
        private const double DegToSec = 36.0d / 100.0d;
        #region Haut niveau
        /// <summary> Suisse en mètres vers CH1903 en DD ou en Rad. Formules approchées  </summary>
        /// <param name="PointGrille"> le point grille à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        /// <param name="Datum"> Datum de la projection suisse </param>
        /// <returns> le point converti </returns>
        internal static PointD ConvertSuisseToCH1903(PointD PointGrille, Datums Datum, bool Deg = true)
        {
            if (PARAM_DATUMS.Ellipsoide != PARAM_DATUMS.ParametresDatums[(int)Datum].Ellipsoide)
            {
                PARAM_DATUMS.SetParametresDatum(Datum); // datum de départ ou Nom
            }
            if (Datum != Datums.Grille_Suisse_LV03)
            {
                // on ramène déjà les coordonnées métrique en LV03
                LV95ToLV03(ref PointGrille);
            }
            return ConvertLV03ToCH1903(PointGrille, Deg);
        }
        /// <summary> CH1903 en DD ou en Rad vers LV03/LV95 en mètres. Formules approchées </summary>
        /// <param name="PointLatLon"> le point LatLon à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        /// <param name="Datum"> projection suisse du point à convertir </param>
        internal static PointD ConvertCH1903ToSuisse(PointD PointLatLon, Datums Datum, bool Deg = true)
        {
            var Ret = ConvertCH1903ToLV03(PointLatLon, Deg);
            if (!Ret.IsEmpty & Datum == Datums.Grille_Suisse_LV95)
            {
                Ret.Add(new SizeD(DELTA_X_LV95, DELTA_Y_LV95));
            }
            return Ret;
        }
        /// <summary> Suisse en mètres vers CH1903 en DD ou en Rad. Formules approchées  </summary>
        /// <param name="PointGrille"> le point grille à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        /// <param name="Datum"> Datum de la projection suisse  </param>
        /// <returns> le point converti </returns>
        internal static PointD ConvertSuisseToWGS84(PointD PointGrille, Datums Datum, bool Deg = true)
        {
            if (PARAM_DATUMS.Ellipsoide != PARAM_DATUMS.ParametresDatums[(int)Datum].Ellipsoide)
            {
                PARAM_DATUMS.SetParametresDatum(Datum); // datum de départ ou Nom
            }
            if (Datum != Datums.Grille_Suisse_LV03)
            {
                // on ramène déjà les coordonnées métrique en LV03 pour LV95 Suisse seul
                LV95ToLV03(ref PointGrille);
            }
            return ConvertLV03ToWGS84(PointGrille, Deg);
        }
        /// <summary> CH1903 en DD ou en Rad vers LV03/LV95 en mètres. Formules approchées </summary>
        /// <param name="PointLatLon"> le point LatLon à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        /// <param name="Datum"> projection suisse du point à convertir </param>
        internal static PointD ConvertWGS84ToSuisse(PointD PointLatLon, Datums Datum, bool Deg = true)
        {
            var Ret = ConvertWGS84ToLV03(PointLatLon, Deg);
            if (!Ret.IsEmpty & Datum == Datums.Grille_Suisse_LV95)
            {
                Ret.Add(new SizeD(DELTA_X_LV95, DELTA_Y_LV95));
            }
            return Ret;
        }
        /// <summary> Convertit une coordonnées de départ exprimée en LV03 vers une coordonnées exprimée en LV95 </summary>
        /// <param name="PointGrille"> PointGrille à convertir </param>
        /// <remarks> Spécifique pour la grille Suisse </remarks>
        internal static void LV03ToLV95(ref PointD PointGrille)
        {
            PointGrille.Add(new SizeD(DELTA_X_LV95, DELTA_Y_LV95));
        }
        /// <summary> Convertit une coordonnées de départ exprimée en LV95 vers une coordonnées exprimée en LV03 </summary>
        /// <param name="PointGrille"> PointGrille à convertir </param>
        /// <remarks> Spécifique pour la grille Suisse </remarks>
        internal static void LV95ToLV03(ref PointD PointGrille)
        {
            if (PointGrille.X > DELTA_X_LV95)
                PointGrille.X -= DELTA_X_LV95;
            if (PointGrille.Y > DELTA_Y_LV95)
                PointGrille.Y -= DELTA_Y_LV95;
        }
        #endregion
        #region Moyen niveau
        /// <summary> LV03 en mètres vers CH1903 en DD ou en Rad. Formules approchées </summary>
        /// <param name="PointGrille"> le point grille à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertLV03ToCH1903(PointD PointGrille, bool Deg = true)
        {
            try
            {
                // initialise les données communes x et y
                PointGrille.Add(new SizeD(-DELTA_X_LV03, -DELTA_Y_LV03));
                PointGrille.Scale(1d / 1_000_000.0d);
                double X_aux = PointGrille.X;
                double Y_aux = PointGrille.Y;
                double X_aux2 = X_aux * X_aux;
                double X_aux3 = X_aux2 * X_aux;
                double X_aux4 = X_aux3 * X_aux;
                double X_aux5 = X_aux4 * X_aux;
                double Y_aux2 = Y_aux * Y_aux;
                double Y_aux3 = Y_aux2 * Y_aux;
                double Y_aux4 = Y_aux3 * Y_aux;
                // coefs longitude
                double Aa_1 = 4.72973056d + 0.7925714d * Y_aux + 0.132812d * Y_aux2 + 0.0255d * Y_aux3 + 0.0048d * Y_aux4;
                double Aa_3 = -0.04427d - 0.0255d * Y_aux - 0.0096d * Y_aux2;
                double Aa_5 = 0.00096d;
                // coefs latitude
                double Pp_0 = +3.23864877d * Y_aux - 0.0025486d * Y_aux2 - 0.013245d * Y_aux3 + 0.000048d * Y_aux4;
                double Pp_2 = -0.27135379d - 0.0450442d * Y_aux - 0.007553d * Y_aux2 - 0.00146d * Y_aux3;
                double pp_4 = 0.002442d + 0.00132d * Y_aux;
                // Process lon lat
                var Ret = new PointD(2.67825d + Aa_1 * X_aux + Aa_3 * X_aux3 + Aa_5 * X_aux5, 16.902866d + Pp_0 + Pp_2 * X_aux2 + pp_4 * X_aux4);
                // Unit 10000" to 1 " and converts seconds to degrees (dec)
                Ret.Scale(SecToDeg);
                if (!Deg)
                    Ret.Scale(Deg_Rad);
                return Ret;
            }
            catch
            {
                return PointD.Empty;
            }
        }
        /// <summary> LV03 en mètres vers WGS84 en DD ou en Rad. Formules approchées </summary>
        /// <param name="PointGrille"> le point grille à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertLV03ToWGS84(PointD PointGrille, bool Deg = true)
        {
            try
            {
                // Converts militar to civil and to unit = 1000km
                PointGrille.Add(new SizeD(-DELTA_X_LV03, -DELTA_Y_LV03));
                PointGrille.Scale(1d / 1_000_000.0d);
                double X_aux = PointGrille.X;
                double Y_aux = PointGrille.Y;
                double X_aux2 = X_aux * X_aux;
                double X_aux3 = X_aux2 * X_aux;
                double Y_aux2 = Y_aux * Y_aux;
                double Y_aux3 = Y_aux2 * Y_aux;
                // Process lon Lat
                var Ret = new PointD(2.6779094d + 4.728982d * X_aux + 0.791484d * X_aux * Y_aux + 0.1306d * X_aux * Y_aux2 - 0.0436d * X_aux3,
                                     16.902389199999998d + 3.238272d * Y_aux - 0.270978d * X_aux2 - 0.002528d * Y_aux2 - 0.0447d * X_aux2 * Y_aux - 0.014d * Y_aux3);
                // Unit 10000" to 1 " and converts seconds to degrees (dec)
                Ret.Scale(SecToDeg);
                if (!Deg)
                    Ret.Scale(Deg_Rad);
                return Ret;
            }
            catch
            {
                return PointD.Empty;
            }
        }
        /// <summary> CH1903 en DD ou en Rad vers LV03 en mètres. Formules approchées </summary>
        /// <param name="PointLatLon"> le point LatLon à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        internal static PointD ConvertCH1903ToLV03(PointD PointLatLon, bool Deg = true)
        {
            try
            {
                // initialise les données communes lat et lon
                if (!Deg)
                    PointLatLon.Scale(Rad_Deg);

                PointLatLon.Scale(DegToSec);
                double Lon = PointLatLon.Lon - 2.67825d;
                double Lat = PointLatLon.Lat - 16.902866d;
                double Lon2 = Lon * Lon;
                double Lon3 = Lon2 * Lon;
                double Lon4 = Lon3 * Lon;
                double Lon5 = Lon4 * Lon;
                double Lat2 = Lat * Lat;
                double Lat3 = Lat2 * Lat;
                double Lat4 = Lat3 * Lat;
                double Lat5 = Lat4 * Lat;
                // calcul coef x
                double X1 = 0.2114285339d - 0.010939608d * Lat - 0.000002658d * Lat2 - 0.00000853d * Lat3;
                double X3 = -0.0000442327d + 0.000004291d * Lat - 0.000000309d * Lat2;
                double X5 = 0.0000000197d;
                // calcul coef y
                double Y0 = 0.3087707463d * Lat + 0.000075028d * Lat2 + 0.000120435d * Lat3 + 0.00000007d * Lat5;
                double Y2 = 0.0037454089d - 0.0001937927d * Lat + 0.00000434d * Lat2 - 0.000000376d * Lat3;
                double Y4 = -0.0000007346d + 0.0000001444d * Lat;
                // retour du point transformé
                var Ret = new PointD(X1 * Lon + X3 * Lon3 + X5 * Lon5, Y0 + Y2 * Lon2 + Y4 * Lon4);
                Ret.Scale(1_000_000.0d);
                Ret.Add(new SizeD(DELTA_X_LV03, DELTA_Y_LV03));
                return Ret;
            }
            catch
            {
                return PointD.Empty;
            }
        }
        /// <summary> WGS84 en DD ou en Rad vers LV03 en mètres. Formules approchées </summary>
        /// <param name="PointLatLon"> le point LatLon à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        internal static PointD ConvertWGS84ToLV03(PointD PointLatLon, bool Deg = true)
        {
            try
            {
                // initialise les données communes lat et lon
                if (!Deg)
                    PointLatLon.Scale(Rad_Deg);

                PointLatLon.Scale(DegToSec);
                double Lon = PointLatLon.Lon - 2.67825d;
                double Lat = PointLatLon.Lat - 16.902866d;
                double Lon2 = Lon * Lon;
                double Lon3 = Lon2 * Lon;
                double Lat2 = Lat * Lat;
                double Lat3 = Lat2 * Lat;
                // calcul coef X
                return new PointD(600072.37d + 211455.93d * Lon - 10938.51d * Lon * Lat - 0.36d * Lon * Lat2 - 44.54d * Lon3,
                                  200147.07d + 308807.95d * Lat + 3745.25d * Lon2 + 76.63d * Lat2 - 194.56d * Lon2 * Lat + 119.79d * Lat3);
            }
            catch
            {
                return PointD.Empty;
            }
        }
        #endregion
    }
}