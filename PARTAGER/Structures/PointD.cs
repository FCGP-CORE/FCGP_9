namespace FCGP
{
    /// <summary> décrit un point avec des coordonnées qui sont des doubles </summary>
    internal struct PointD
    {
        #region Partagé
        /// <summary> renvoie un nouveau pointD décalé de la valeur de l'offset </summary>
        /// <param name="Point"> PointD de départ </param>
        /// <param name="X"> décalage X du point </param>
        /// <param name="Y"> décalage Y du point </param>
        internal static PointD Offset(PointD Point, double X, double Y)
        {
            Point.Offset(X, Y);
            return Point;
        }
        /// <summary> renvoie un nouveau pointD décalé de la valeur de sizeD </summary>
        /// <param name="Point"> PointD de départ </param>
        /// <param name="Taille"> décalage X et Y du point </param>
        internal static PointD Add(PointD Point, SizeD Taille)
        {
            Point.Add(Taille);
            return Point;
        }
        /// <summary> renvoie un nouveau pointD étendu par le facteur </summary>
        /// <param name="Point"> PointD de départ </param>
        /// <param name="Facteur"></param>
        /// <returns></returns>
        internal static PointD Scale(PointD Point, double Facteur)
        {
            Point.Scale(Facteur);
            return Point;
        }
        /// <summary> opérateur qui détermine si un pointD est egal à un autre pointd </summary>
        public static bool operator ==(PointD Pt1, PointD Pt2)
        {
            return Pt1.Egal(Pt2);
        }

        /// <summary> opérateur qui détermine si un pointD est différent d'un autre pointd </summary>
        public static bool operator !=(PointD Pt1, PointD Pt2)
        {
            return Pt1.Different(Pt2);
        }
        /// <summary> renvoie un  pointD qui est non initialisé avec IsEmpty = true </summary>
        internal static readonly PointD Empty;
        #endregion
        #region Instance
        public override bool Equals(object obj)
        {
            if (obj is PointD Pt)
            {
                return Egal(Pt);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _X.GetHashCode() | _Y.GetHashCode();
        }

        /// <summary> détermine si le pointD est egal à un autre pointd </summary>
        internal bool Egal(PointD Pt)
        {
            return FlagNotEmpty == Pt.FlagNotEmpty && _X == Pt.X && _Y == Pt.Y;
        }
        /// <summary> détermine si le pointD est différent d'un autre pointd </summary>
        internal bool Different(PointD Pt)
        {
            return FlagNotEmpty != Pt.FlagNotEmpty || _X != Pt.X || _Y != Pt.Y;
        }
        /// <summary> décale le point de la valeur de l'offset </summary>
        /// <param name="X"> décalage X du point </param>
        /// <param name="Y"> décalage Y du point </param>
        internal void Offset(double X, double Y)
        {
            _X += X;
            _Y += Y;
        }
        /// <summary> décale le point de la valeur de sizeD </summary>
        /// <param name="Taille"> décalage X et Y du point </param>
        internal void Add(SizeD Taille)
        {
            _X += Taille.Largeur;
            _Y += Taille.Hauteur;
        }
        /// <summary> etend les coordonnées du point à partir d'un facteur </summary>
        /// <param name="Facteur"></param>
        internal void Scale(double Facteur)
        {
            _X *= Facteur;
            _Y *= Facteur;
        }

        private readonly bool FlagNotEmpty;
        private double _X;
        private double _Y;
        /// <summary> initialise la structure </summary>
        /// <param name="X"> coordonnée X du point </param>
        /// <param name="Y"> coordonnée Y du point </param>
        internal PointD(double X, double Y)
        {
            _X = X;
            _Y = Y;
            FlagNotEmpty = true;
        }
        /// <summary> initialise la structure </summary>
        /// <param name="Location"> point </param>
        internal PointD(Point Location)
        {
            _X = Location.X;
            _Y = Location.Y;
            FlagNotEmpty = true;
        }
        /// <summary> chaine représentant le PointD </summary>
        public override string ToString()
        {
            return $"X = {_X} Y = {_Y}";
        }

        /// <summary> renvoie la coordonnée X du point </summary>
        internal double X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }
        /// <summary> renvoie la coordonnée Longitude du point si celui-ci est considré comme un point LatLon</summary>
        internal double Lon
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }
        /// <summary> renvoie la coordonnée y du point </summary>
        internal double Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }
        /// <summary> renvoie la coordonnée Latitude du point si celui-ci est considré comme un point LatLon</summary>
        internal double Lat
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }
        /// <summary> renvoi un Point à partir du pointD </summary>
        internal Point ToPoint
        {
            get
            {
                return new Point((int)Math.Round(_X), (int)Math.Round(_Y));
            }
        }
        /// <summary> renvoi un PointF à partir du pointD </summary>
        internal PointF ToPointF
        {
            get
            {
                return new PointF((float)_X, (float)_Y);
            }
        }
        /// <summary> True si la structure a été initialisée avec le constructeur par defaut(sans paramètres) </summary>
        internal bool IsEmpty
        {
            get
            {
                return !FlagNotEmpty;
            }
        }
        #endregion
    }
}