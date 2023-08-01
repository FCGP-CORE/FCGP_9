using static FCGP.ConvertirCoordonnees;

namespace FCGP.Coordonnees
{
    /// <summary> Projection LatitudeLongitude valable sur la terre entière. Utilisé principalement sur les ordinateurs
    /// aussi appelée Pseudo Plate carrée. A la base ce n'est pas vraiement une projection </summary>
    internal static class ProjectionLongitudeLatitude
    {
        #region Haut Niveau
        /// <summary> WGS84 en DD ou en Rad vers LatLon en mètres. </summary>
        /// <param name="PointLatLon"> le point LatLon à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        internal static PointD ConvertWGS84ToLatLon(PointD PointLatLon, bool Deg = true)
        {
            try
            {
                if (Deg)
                    PointLatLon.Scale(Deg_Rad);
                return new PointD(PointLatLon.Lon * RayonWGS84, PointLatLon.Lat * RayonWGS84);
            }
            catch
            {
                return PointD.Empty;
            }
        }
        /// <summary> LatLon en mètres vers WGS84 en DD ou en Rad. </summary>
        /// <param name="PointGrille"> le point grille à convertir </param>
        /// <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        internal static PointD ConvertLatLonToWGS84(PointD PointGrille, bool Deg = true)
        {
            try
            {
                var Ret = new PointD(PointGrille.X / RayonWGS84, PointGrille.Y / RayonWGS84);
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