using static FCGP.ConvertirCoordonnees;
using static FCGP.DonneesSiteWeb;

namespace FCGP
{
    /// <summary> décrit un point avec des coordonnées qui sont exprimées en pixels d'un zoom ou echelle d'un site carto en indices tuile et offset </summary>
    internal struct PointG
    {
        #region Partagé
        /// <summary> renvoie un pointG qui est egal à point.add(point.location,taille) </summary>
        internal static PointG Add(PointG Point, Size Taille)
        {
            Point.Add(Taille);
            return Point;
        }
        /// <summary> renvoie un pointG qui est egal à point.offset(location) </summary>
        internal static PointG Add(PointG Point, Point Location)
        {
            Point.Add(Location);
            return Point;
        }
        /// <summary> renvoie un pointG qui est egal à point.substract(point.location,taille)</summary>
        internal static PointG Substract(PointG Point, Size Taille)
        {
            Point.Substract(Taille);
            return Point;
        }
        /// <summary> renvoie un pointG qui est egal à point.X-location.X et point.y-location.y </summary>
        internal static PointG Substract(PointG Point, Point Location)
        {
            Point.Substract(Location);
            return Point;
        }
        /// <summary> opérateur qui détermine si un pointG est egal à un autre pointG </summary>
        public static bool operator ==(PointG Pt1, PointG Pt2)
        {
            return Pt1.Egal(Pt2);
        }
        /// <summary> opérateur qui détermine si un pointG est différent d'un autre pointG </summary>
        public static bool operator !=(PointG Pt1, PointG Pt2)
        {
            return Pt1.Different(Pt2);
        }
#pragma warning disable CS0649
        internal static readonly PointG Empty;
#pragma warning restore CS0649
        #endregion
        #region Instance
        public override int GetHashCode()
        {
            return IndiceEchelle.GetHashCode() | location.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is PointG Pt)
            {
                return Egal(Pt);
            }
            return false;
        }
        internal bool Egal(PointG Pt)
        {
            return FlagNotEmpty == Pt.FlagNotEmpty && IndiceEchelle == Pt.IndiceEchelle && location == Pt.Location;
        }
        /// <summary> détermine si le pointD est différent d'un autre pointd </summary>
        internal bool Different(PointG Pt)
        {
            return FlagNotEmpty != Pt.FlagNotEmpty || IndiceEchelle != Pt.IndiceEchelle || location != Pt.Location;
        }
        /// <summary> additionne la taille à la location du point </summary>
        internal void Add(Size Taille)
        {
            location = Point.Add(location, Taille);
            CalculerIndexTuile();
        }
        /// <summary> decale la location du point de location</summary>
        internal void Add(Point Location)
        {
            location.Offset(Location);
            CalculerIndexTuile();
        }
        /// <summary> soustrait la taille à la location du point </summary>
        internal void Substract(Size taille)
        {
            location = Point.Subtract(location, taille);
            CalculerIndexTuile();
        }
        /// <summary> decale la location du point de location</summary>
        internal void Substract(Point Location)
        {
            location.X -= Location.X;
            location.Y -= Location.Y;
            CalculerIndexTuile();
        }
        /// <summary> stockage de l'index des tuiles </summary>
        private int IndexTuileX, IndexTuileY;
        private Size _Offset;
        /// <summary> calcul l'index des tuiles de la location du point </summary>
        private void CalculerIndexTuile()
        {
            IndexTuileX = PixelToNumTile(location.X, false);
            IndexTuileY = PixelToNumTile(location.Y, false);
            _Offset = new Size(location.X - IndexTuileX * NbPixelsTuile, location.Y - IndexTuileY * NbPixelsTuile);
        }
        /// <summary> calcul la location du point exprimé en pixels </summary>
        private void CalculerLocation()
        {
            location.X = IndexTuileX * NbPixelsTuile + _Offset.Width;
            location.Y = IndexTuileY * NbPixelsTuile + _Offset.Height;
        }
        /// <summary> flag indiquant que si la structure a été initialisée </summary>
        private readonly bool FlagNotEmpty;
        // ''' <summary> indice des valeurs Echelle ou Zoom correspondant à coordonnées </summary>
        internal int IndiceEchelle { get; private set; }
        /// <summary> coordonnées X en pixels du système WebMercator </summary>
        internal int X
        {
            get
            {
                return location.X;
            }
        }
        /// <summary> coordonnées Y en pixels du système WebMercator </summary>
        internal int Y
        {
            get
            {
                return location.Y;
            }
        }
        /// <summary> coordonnées en pixels du système WebMercator </summary>
        internal Point Location { get { return location; } }
        private Point location;
        /// <summary> initialisation de la structure à partir d'un niveau de zoom et de 2 entiers pour les coordonnées </summary>
        internal PointG(int IndiceEchelle, int X, int Y)
        {
            location.X = X;
            location.Y = Y;
            this.IndiceEchelle = IndiceEchelle;
            CalculerIndexTuile();
            FlagNotEmpty = true;
        }
        /// <summary> initialisation de la structure à partir d'un niveau de zoom et d'un point pour les coordonnées </summary>
        internal PointG(int IndiceEchelle, Point Location)
        {
            location = Location;
            this.IndiceEchelle = IndiceEchelle;
            CalculerIndexTuile();
            FlagNotEmpty = true;
        }
        /// <summary> initialisation de la structure à partir d'un niveau de zoom et d'une tuile (point) et d'un décalagePixels (size) </summary>
        internal PointG(int IndiceEchelle, Point IndexTuile, Size Offset)
        {
            this.IndiceEchelle = IndiceEchelle;
            IndexTuileX = IndexTuile.X;
            IndexTuileY = IndexTuile.Y;
            _Offset = Offset;
            CalculerLocation();
            FlagNotEmpty = true;
        }
        /// <summary> initialisation de la structure à partir d'un niveau de zoom et de 4 entier </summary>
        internal PointG(int IndiceEchelle, int IndexTuileX, int IndexTuileY, int OffsetX, int OffsetY)
        {
            this.IndiceEchelle = IndiceEchelle;
            this.IndexTuileX = IndexTuileX;
            this.IndexTuileY = IndexTuileY;
            _Offset = new Size(OffsetX, OffsetY);
            CalculerLocation();
            FlagNotEmpty = true;
        }
        /// <summary> tuile contenant les coordonnées </summary>
        internal Point IndexTuile
        {
            get
            {
                return new Point(IndexTuileX, IndexTuileY);
            }
        }
        /// <summary> valeur exprimée en pixels du décalage par rapport au début de tuile </summary>
        internal Size Offset
        {
            get
            {
                return _Offset;
            }
        }
        /// <summary> coordonnées en pixels du système WebMercator du Point Haut Gauche de la tuile contenant le point </summary>
        internal Point Pt0Tuile
        {
            get
            {
                return new Point(IndexTuileX * NbPixelsTuile, IndexTuileY * NbPixelsTuile);
            }
        }
        /// <summary> True si la structure a été initialisée avec le constructeur </summary>
        internal bool IsEmpty
        {
            get
            {
                return !FlagNotEmpty;
            }
        }
        /// <summary> chaine représentant le PointG </summary>
        public new string ToString()
        {
            return $"Col={IndexTuileX} Row={IndexTuileY} X={_Offset.Width} Y={_Offset.Height}";
        }
        #endregion
    }
}