namespace FCGP
{
    /// <summary> structure qui sert à véhiculer un point de coordonnées Grille (X,Y ou PointD, Zone et Hémisphère pour un point de coordonnées UTM </summary>
    internal struct PointProjection
    {
        internal double X
        {
            get
            {
                return Coordonnees.X;
            }
            set
            {
                Coordonnees.X = value;
            }
        }
        internal double Y
        {
            get
            {
                return Coordonnees.Y;
            }
            set
            {
                Coordonnees.Y = value;
            }
        }
        internal PointD Coordonnees;
        internal int Zone;
        internal char Hemisphere;
        internal PointProjection(PointD Point, int Z = 0, char H = default)
        {
            Coordonnees = Point;
            Zone = Z;
            Hemisphere = H;
        }
    }
}