namespace FCGP
{
    /// <summary> décrit un rectangle avec des coordonnées qui sont des doubles </summary>
    internal struct RectangleD
    {
        private readonly bool FlagNotEmpty;
#pragma warning disable CS0649
        internal static readonly RectangleD Empty;
#pragma warning restore CS0649
        /// <summary> opérateur qui détermine si un pointD est egal à un autre pointd </summary>
        public static bool operator ==(RectangleD Rd1, RectangleD Rd2)
        {
            return Rd1.Egal(Rd2);
        }

        /// <summary> opérateur qui détermine si un pointD est différent d'un autre pointd </summary>
        public static bool operator !=(RectangleD Rd1, RectangleD Rd2)
        {
            return Rd1.Different(Rd2);
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleD Rd)
            {
                return Egal(Rd);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return pt0.GetHashCode() | pt2.GetHashCode();
        }
        /// <summary> détermine si le rectangleD est egal à un autre rectangleD </summary>
        internal bool Egal(RectangleD Rd)
        {
            return FlagNotEmpty == Rd.FlagNotEmpty && pt0 == Rd.pt0 && pt2 == Rd.pt2;
        }
        /// <summary> détermine si le rectangleD est différent d'un autre rectangleD </summary>
        internal bool Different(RectangleD Rd)
        {
            return FlagNotEmpty != Rd.FlagNotEmpty || pt0 != Rd.pt0 || pt2 != Rd.pt2;
        }
        /// <summary> initialise la structure à partir de 2 pointsD </summary>
        /// <param name="Pt0"> point haut et gauche du rectangle </param>
        /// <param name="Pt2"> point bas et droit du rectangle </param>
        internal RectangleD(PointD Pt0, PointD Pt2)
        {
            pt0 = Pt0;
            pt2 = Pt2;
            FlagNotEmpty = true;
        }
        /// <summary> initialise la structure à partir de 4 doubles </summary>
        /// <param name="X0"> gauche du rectangle </param>
        /// <param name="Y0"> haut du rectangle </param>
        /// <param name="X2"> droit du rectangle </param>
        /// <param name="Y2"> bas du rectangle </param>
        internal RectangleD(double X0, double Y0, double X2, double Y2)
        {
            pt0 = new PointD(X0, Y0);
            pt2 = new PointD(X2, Y2);
            FlagNotEmpty = true;
        }
        /// <summary> initialise la structure à partir de 4 doubles </summary>
        /// <param name="Pt0"> location du rectangle </param>
        /// <param name="Taille"> taille du rectangle </param>
        internal RectangleD(PointD Pt0, SizeD Taille)
        {
            this.pt0 = Pt0;
            pt2 = PointD.Add(Pt0, Taille);
            FlagNotEmpty = true;
        }
        private PointD pt0, pt2;
        /// <summary> renvoie le point haut et gauche ou NordOuest du rectangle </summary>
        internal PointD Pt0 { get { return pt0; } set { pt0 = value; } }
        /// <summary> point bas et droit du rectangle ou SudEst </summary>
        internal PointD Pt2 { get { return pt2; } set { pt2 = value; } }
        /// <summary> renvoie le point demandé </summary>
        /// <param name="Item"> numéro du point demandé </param>
        internal PointD Pt(int Item)
        {
            switch (Item)
            {
                case 0:
                    {
                        return pt0;
                    }
                case 2:
                    {
                        return pt2;
                    }
                case 1:
                    {
                        return new PointD(pt2.X, pt0.Y);
                    }
                case 3:
                    {
                        return new PointD(pt0.X, pt2.Y);
                    }

                default:
                    {
                        return default;
                    }
            }
        }
        /// <summary> True si la structure a été initialisée avec un constructeur public</summary>
        internal bool IsEmpty
        {
            get
            {
                return !FlagNotEmpty;
            }
        }
        /// <summary> renvoie la largeur du rectangle </summary>
        internal double Largeur
        {
            get
            {
                return pt2.X - pt0.X;
            }
        }
        /// <summary> renvoie la hauteur du rectangle </summary>
        internal double Hauteur
        {
            get
            {
                return pt2.Y - pt0.Y;
            }
        }
        /// <summary> renvoie la largeur du rectangle mais dans un contexte de coordonnées. =largeur </summary>
        internal double LargeurCoordonnees
        {
            get
            {
                return pt2.X - pt0.X;
            }
        }
        /// <summary> renvoie la hauteur du rectangle mais dans un contexte de coordonnées. =-hauteur </summary>
        internal double HauteurCoordonnees
        {
            get
            {
                return pt0.Y - pt2.Y;
            }
        }
        internal SizeD Taille
        {
            get
            {
                return new SizeD(pt2.X - pt0.X, pt2.Y - pt0.Y);
            }
        }
        /// <summary> Pt0.X </summary>
        internal double X0
        {
            get
            {
                return pt0.X;
            }
            set
            {
                pt0.X = value;
            }
        }
        /// <summary> Pt2.X </summary>
        internal double X2
        {
            get
            {
                return pt2.X;
            }
            set
            {
                pt2.X = value;
            }
        }
        /// <summary> PT0.Y </summary>
        internal double Y0
        {
            get
            {
                return pt0.Y;
            }
            set
            {
                pt0.Y = value;
            }
        }
        /// <summary> PT2.Y </summary>
        internal double Y2
        {
            get
            {
                return pt2.Y;
            }
            set
            {
                pt2.Y = value;
            }
        }
        /// <summary> Pt0.X mais dans un contexte de coordonnées </summary>
        internal double Ouest
        {
            get
            {
                return pt0.X;
            }
            set
            {
                pt0.X = value;
            }
        }
        /// <summary> Pt2.X mais dans un contexte de coordonnées </summary>
        internal double Est
        {
            get
            {
                return pt2.X;
            }
            set
            {
                pt2.X = value;
            }
        }
        /// <summary> Pt0.Y mais dans un contexte de coordonnées </summary>
        internal double Nord
        {
            get
            {
                return pt0.Y;
            }
            set
            {
                pt0.Y = value;
            }
        }
        /// <summary> Pt2.Y mais dans un contexte de coordonnées </summary>
        internal double Sud
        {
            get
            {
                return pt2.Y;
            }
            set
            {
                pt2.Y = value;
            }
        }

        /// <summary> renvoie rectangle à partir du rectangleD </summary>
        internal Rectangle ToRectangle
        {
            get
            {
                return new Rectangle(pt0.ToPoint, Taille.ToSize);
            }
        }
        /// <summary> renvoie rectangleF à partir du rectangleD </summary>
        internal RectangleF ToRectangleF
        {
            get
            {
                return new RectangleF(pt0.ToPointF, new SizeF((float)Largeur, (float)Hauteur));
            }
        }

        /// <summary> indique si le rectangle contient le point </summary>
        /// <param name="Point"> point à tester </param>
        internal bool Contains(PointD Point)
        {
            return pt0.X <= Point.X && Point.X < pt2.X && pt0.Y <= Point.Y && Point.Y < pt2.Y;
        }
        /// <summary> indique si le rectangle contient le point mais dans un contexte de coordonnées summary>
        /// <param name="Point"> point à tester </param>
        internal bool CoordonneesContains(PointD Point)
        {
            return pt0.X <= Point.X && Point.X < pt2.X && pt0.Y >= Point.Y && Point.Y > pt2.Y;
        }
        /// <summary> indique si le rectangle contient entièrement le rectangle </summary>
        /// <param name="Rect"> rectangle à tester </param>
        internal bool Contains(RectangleD Rect)
        {
            return Pt0.X <= Rect.pt0.X && Rect.pt2.X <= pt2.X && pt0.Y <= Rect.pt0.Y && Rect.pt2.Y <= pt2.Y;
        }
        /// <summary> indique si le rectangle contient entièrement le rectangle mais dans un contexte de coordonnées </summary>
        /// <param name="Rect"> rectangle à tester </param>
        internal bool CoordonneesContains(RectangleD Rect)
        {
            return Pt0.X <= Rect.pt0.X && Rect.pt2.X <= pt2.X && pt0.Y >= Rect.pt0.Y && Rect.pt2.Y >= pt2.Y;
        }
        /// <summary> indique si le rectangle a une intersection le rectangle </summary>
        /// <param name="Rect"> rectangle à tester </param>
        internal bool IntersectsWith(RectangleD rect)
        {
            return pt0.X < rect.pt2.X && rect.pt0.X < pt2.X && rect.pt0.Y < pt2.Y && pt0.Y < rect.pt2.Y;
        }
        /// <summary> indique si le rectangle a une intersection le rectangle mais dans un contexte de coordonnées </summary>
        /// <param name="Rect"> rectangle à tester </param>
        internal bool CoordonneesIntersectsWith(RectangleD Rect)
        {
            return pt0.X < Rect.pt2.X && Rect.pt0.X < pt2.X && Rect.pt0.Y > pt2.Y && pt0.Y > Rect.pt2.Y;
        }
        /// <summary> chaine représentant le RectangleD </summary>
        public override string ToString()
        {
            return $"X = {pt0.X} Y = {pt0.Y} Width = {Largeur} Height = {Hauteur}";
        }
    }
}