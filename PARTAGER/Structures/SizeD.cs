namespace FCGP
{
    internal struct SizeD
    {
        private readonly bool FlagNotEmpty;
        internal static SizeD Add(SizeD Taille1, SizeD Taille2)
        {
            return new SizeD(Taille1.Largeur + Taille2.Largeur, Taille1.Hauteur + Taille2.Hauteur);
        }
        internal static SizeD Scale(SizeD Taille, double Facteur)
        {
            return new SizeD(Taille.Largeur * Facteur, Taille.Hauteur * Facteur);
        }
        internal static readonly SizeD Empty;
        internal SizeD(PointD Point)
        {
            Largeur = Point.X;
            Hauteur = Point.Y;
            FlagNotEmpty = true;
        }
        internal SizeD(Size Size)
        {
            Largeur = Size.Width;
            Hauteur = Size.Height;
            FlagNotEmpty = true;
        }
        internal SizeD(double Largeur, double Hauteur)
        {
            this.Largeur = Largeur;
            this.Hauteur = Hauteur;
            FlagNotEmpty = true;
        }
        internal bool IsEmpty
        {
            get
            {
                return !FlagNotEmpty;
            }
        }
        /// <summary> renvoie la largeur du rectangle </summary>
        internal double Largeur { get; private set; }
        /// <summary> renvoie la hauteur du rectangle </summary>
        internal double Hauteur { get; private set; }
        /// <summary> renvoie une size à partir du sizeD avec un arrondi Sup</summary>
        internal Size ToSize
        {
            get
            {
                return new Size((int)Math.Round(Largeur), (int)Math.Round(Hauteur));
            }
        }
        internal Point ToPoint
        {
            get
            {
                return new Point(ToSize);
            }
        }
        /// <summary> renvoie sizeF à partir du sizeD </summary>
        internal SizeF ToSizeF
        {
            get
            {
                return new SizeF((float)Largeur, (float)Hauteur);
            }
        }
        internal PointF ToPoinF
        {
            get
            {
                return new PointF((float)Largeur, (float)Hauteur);
            }
        }
        /// <summary> chaine représentant le PointD </summary>
        public override string ToString()
        {
            return $"Width = {Largeur} Height = {Hauteur}";
        }
    }
}