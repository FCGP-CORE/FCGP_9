using static FCGP.ConvertirCoordonnees;

namespace FCGP.Coordonnees
{
    /// <summary> Projection Pseudo Mercator ou Web Mercator valable sur de -85.0 à +85.0 de Latitude. Utilisé principalement sur les site Web de cartographie</summary>
    static class ProjectionWebMercator
    {
        #region Haut Niveau
        /// <summary> WGS84 en DD ou en Rad vers WebMercator en mètres. </summary>
        /// <param name="PointLatLon"> le point LatLon à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        internal static PointD ConvertWGS84ToWebMercator(PointD PointLatLon, bool Deg = true)
        {
            try
            {
                if (Deg)
                    PointLatLon.Scale(Deg_Rad);

                return new PointD(PointLatLon.Lon * RayonWGS84, Math.Log(Math.Tan(PointLatLon.Lat / 2d + Pi_4)) * RayonWGS84);
            }
            catch
            {
                return PointD.Empty;
            }
        }
        /// <summary> WebMercator en mètres vers WGS84 en DD ou en Rad. </summary>
        /// <param name="PointGrille"> le point grille à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertWebMercatorToWGS84(PointD PointGrille, bool Deg = true)
        {
            try
            {
                var Ret = new PointD(PointGrille.X / RayonWGS84, 2.0d * Math.Atan(Math.Exp(PointGrille.Y / RayonWGS84)) - Pi_2);
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
    }
}