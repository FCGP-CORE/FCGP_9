using static FCGP.ConvertirCoordonnees;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary> Conteneur servant à décrire une surface (rectangle) qui est composée par des tuiles de serveur. 
    /// aussi appelé surface de traitement car il y a 3 possibilité d'utilisation :
    /// téléchargement d'une carte, suppression de tuiles dans le cache et affichage d'une carte en 3D </summary>
    internal class SurfaceTuiles
    {
        private SitesCartographiques _Site;
        private Echelles _Echelle;
        private TuileServeurCarto Tuile0;
        private TuileServeurCarto Tuile2;
        private bool FlagNotEmpty;
        /// <summary> constructeur pour une instance vide </summary>
        internal SurfaceTuiles()
        {
            Tuile0 = new TuileServeurCarto();
            Tuile2 = new TuileServeurCarto();
        }
        /// <summary> gère une surface composée de tuiles </summary>
        /// <param name="SurfaceGrille"> rectangleD dont les dimensions sont exprimées en mètres de la projection du serveur carto </param>
        /// <param name="SiteCarto"> site du serveur carto </param>
        /// <param name="Echelle"> indice de l'échelle du serveur carto </param>
        internal SurfaceTuiles(RectangleD SurfaceGrille, SitesCartographiques SiteCarto, Echelles Echelle) : this()
        {
            if (SurfaceGrille.IsEmpty)
                return;
            CalculerTuiles(SurfaceGrille, SiteCarto, Echelle);
        }
        /// <summary> gère une surface composée de tuiles </summary>
        /// <param name="Point1"> PointD dont les dimensions sont exprimées en mètres de la projection du serveur carto </param>
        /// <param name="Point2"> PointD dont les dimensions sont exprimées en mètres de la projection du serveur carto </param>
        /// <param name="SiteCarto"> site du serveur carto </param>
        /// <param name="Echelle"> indice de l'échelle du serveur carto </param>
        internal SurfaceTuiles(PointD Point1, PointD Point2, SitesCartographiques SiteCarto, Echelles Echelle) : this()
        {
            if (Point1.IsEmpty || Point2.IsEmpty)
                return;
            var SurfaceGrille = new RectangleD(Math.Min(Point1.X, Point2.X), Math.Max(Point1.Y, Point2.Y), Math.Max(Point1.X, Point2.X), Math.Min(Point1.Y, Point2.Y));
            CalculerTuiles(SurfaceGrille, SiteCarto, Echelle);
        }
        /// <summary> gère une surface composée de tuiles </summary>
        /// <param name="SurfacePixels"> rectangle dont les dimensions sont exprimées en pixels de la projection et de l'échelle du serveur carto </param>
        /// <param name="SiteCarto"> site du serveur carto </param>
        /// <param name="Echelle"> indice de l'échelle du serveur carto </param>
        internal SurfaceTuiles(Rectangle SurfacePixels, SitesCartographiques SiteCarto, Echelles Echelle) : this()
        {
            if (SurfacePixels.IsEmpty)
                return;
            CalculerTuiles(SurfacePixels, SiteCarto, Echelle);
        }
        /// <summary> gère une surface composée de tuiles </summary>
        /// <param name="Point1"> Point dont les dimensions sont exprimées en pixels de la projection et de l'échelle du serveur carto </param>
        /// <param name="Point2"> Point dont les dimensions sont exprimées en pixels de la projection et de l'échelle du serveur carto </param>
        /// <param name="SiteCarto"> site du serveur carto </param>
        /// <param name="Echelle"> indice de l'échelle du serveur carto </param>
        internal SurfaceTuiles(Point Point1, Point Point2, SitesCartographiques SiteCarto, Echelles Echelle) : this()
        {
            if (Point1.IsEmpty || Point2.IsEmpty)
                return;
            var SurfacePixels = new Rectangle(Math.Min(Point1.X, Point2.X), Math.Min(Point1.Y, Point2.Y), Math.Max(Point1.X, Point2.X) - Math.Min(Point1.X, Point2.X), Math.Max(Point1.Y, Point2.Y) - Math.Min(Point1.Y, Point2.Y));
            CalculerTuiles(SurfacePixels, SiteCarto, Echelle);
        }
        /// <summary> calcule les tuiles qui délimitent la surface à partir d'un rectangle dont les unités sont exprimées en mètres </summary>
        /// <param name="SurfaceGrille"> rectangle exprimé en mètres </param>
        /// <param name="SiteCarto"> site catographiques </param>
        /// <param name="Echelle"> indice de l'échelle </param>
        private void CalculerTuiles(RectangleD SurfaceGrille, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            _Site = SiteCarto;
            _Echelle = Echelle;
            Tuile0 = new TuileServeurCarto(SurfaceGrille.Pt0, SiteCarto, Echelle, false);
            Tuile2 = new TuileServeurCarto(SurfaceGrille.Pt2, SiteCarto, Echelle, true);
            FlagNotEmpty = true;
            CalculerSurface();
        }
        /// <summary> calcule les tuiles qui délimitent la surface à partir d'un rectangle dont les unités sont exprimées en pixels </summary>
        /// <param name="SurfacePixels"> rectangle exprimé en pixels </param>
        /// <param name="SiteCarto"> site catographiques </param>
        /// <param name="Echelle"> indice de l'échelle </param>
        private void CalculerTuiles(Rectangle SurfacePixels, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            _Site = SiteCarto;
            _Echelle = Echelle;
            Tuile0 = new TuileServeurCarto(SurfacePixels.Location, SiteCarto, Echelle, false);
            Tuile2 = new TuileServeurCarto(new Point(SurfacePixels.Right, SurfacePixels.Bottom), SiteCarto, Echelle, true);
            FlagNotEmpty = true;
            CalculerSurface();
        }
        /// <summary> calcul les surfaces en pixels et en mètre de la surface représentées par l'ensemble des tuiles </summary>
        private void CalculerSurface()
        {
            RectangleGrille = new RectangleD(Tuile0.ToGrille(false), Tuile2.ToGrille(true));
            var Pt0 = Tuile0.ToPixels(false);
            var Pt2 = Tuile2.ToPixels(true);
            RectanglePixels = new Rectangle(Tuile0.ToPixels(false), new Size(Pt2.X - Pt0.X, Pt2.Y - Pt0.Y));
        }
        /// <summary> Site associé à la surface </summary>
        internal SitesCartographiques Site
        {
            get
            {
                if (!FlagNotEmpty)
                {
                    return SitesCartographiques.Aucun;
                }
                else
                {
                    return _Site;
                }
            }
        }
        /// <summary> Indice de l'échelle associée à la surface </summary>
        internal Echelles Echelle
        {
            get
            {
                if (!FlagNotEmpty)
                {
                    return Echelles._000;
                }
                else
                {
                    return _Echelle;
                }
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
        /// <summary> Nb de tuiles de la surface </summary>
        internal int NbTuiles
        {
            get
            {
                if (!FlagNotEmpty)
                {
                    return 0;
                }
                else
                {
                    return (Tuile2.NumCol - Tuile0.NumCol + 1) * (Tuile2.NumRang - Tuile0.NumRang + 1);
                }
            }
        }
        /// <summary> Nb de colonnes de la surface (Largeur) </summary>
        internal int NbColonnes
        {
            get
            {
                if (!FlagNotEmpty)
                {
                    return 0;
                }
                else
                {
                    return Tuile2.NumCol - Tuile0.NumCol + 1;
                }
            }
        }
        /// <summary> Nb de rangées de la surface (Hauteur) </summary>
        internal int NbRangees
        {
            get
            {
                if (!FlagNotEmpty)
                {
                    return 0;
                }
                else
                {
                    return Tuile2.NumRang - Tuile0.NumRang + 1;
                }
            }
        }
        /// <summary> numéro de colonne de départ (Axe des X) </summary>
        internal int NumColDeb
        {
            get
            {
                return Tuile0.NumCol;
            }
        }
        /// <summary> numéro de colonne de fin (Axe des X) </summary>
        internal int NumColFin
        {
            get
            {
                return Tuile2.NumCol;
            }
        }
        /// <summary> numéro de rangée de départ (Axe des Y) </summary>
        internal int NumRangDeb
        {
            get
            {
                return Tuile0.NumRang;
            }
        }
        /// <summary> numéro de rangée de fin (Axe des Y) </summary>
        internal int NumRangFin
        {
            get
            {
                return Tuile2.NumRang;
            }
        }
        /// <summary> numéro de Zoom des tuiles </summary>
        internal byte Couche
        {
            get
            {
                return FlagNotEmpty ? EchelleToCouche(_Echelle) : (byte)0;
            }
        }
        /// <summary> largeur et hauteur d'une tuile exprimée en mètres de la grille. Ce n'est pas forcément des vrais mètres </summary>
        internal double LargeurHauteurTuile
        {
            get
            {
                return FlagNotEmpty ? MetresTuile(_Echelle) : 0.0d;
            }
        }
        /// <summary> surface des tuiles exprimée en mètres de la projection du site </summary>
        internal RectangleD RectangleGrille { get; private set; }
        internal PointD PtGrille(int NumCoin)
        {
            return RectangleGrille.Pt(NumCoin);
        }
        /// <summary> Coordonnées du point Nord Ouest exprimé en mètres </summary>
        internal PointD PtGrille_NO
        {
            get
            {
                return RectangleGrille.Pt0;
            }
        }
        /// <summary> Coordonnées du point Sud Est exprimé en mètres </summary>
        internal PointD PtGrille_SE
        {
            get
            {
                return RectangleGrille.Pt2;
            }
        }
        /// <summary> Hauteur de la surface exprimée en mètres </summary>
        internal double HauteurGrille
        {
            get
            {
                return -RectangleGrille.Hauteur;
            }
        }
        /// <summary> Largeur de la surface exprimée en mètres </summary>
        internal double LargeurGrille
        {
            get
            {
                return RectangleGrille.Largeur;
            }
        }
        /// <summary> surface de la surface des tuiles exprimée en m2 </summary>
        internal double SurfaceGrille
        {
            get
            {
                return -RectangleGrille.Largeur * RectangleGrille.Hauteur;
            }
        }
        /// <summary> surface des tuiles exprimée en pixels de la projection du site </summary>
        internal Rectangle RectanglePixels { get; private set; }
        /// <summary> Coordonnées du point Nord Ouest exprimé en pixels </summary>
        internal Point PtPixels_NO
        {
            get
            {
                return RectanglePixels.Location;
            }
        }
        /// <summary> Coordonnées du point Sud Est exprimé en pixels </summary>
        internal Point PtPixels_SE
        {
            get
            {
                return Point.Add(RectanglePixels.Location, RectanglePixels.Size);
            }
        }
        internal Point PtPixels(int NumCoin)
        {
            switch (NumCoin)
            {
                case 0:
                    {
                        return new Point(RectanglePixels.Left, RectanglePixels.Top);
                    }
                case 1:
                    {
                        return new Point(RectanglePixels.Right, RectanglePixels.Top);
                    }
                case 2:
                    {
                        return new Point(RectanglePixels.Right, RectanglePixels.Bottom);
                    }
                case 3:
                    {
                        return new Point(RectanglePixels.Left, RectanglePixels.Bottom);
                    }
            }
            return Point.Add(RectanglePixels.Location, RectanglePixels.Size);
        }
        /// <summary> Largeur de la surface exprimée en pixels </summary>
        internal int LargeurPixels
        {
            get
            {
                return RectanglePixels.Width;
            }
        }
        /// <summary> Hauteur de la surface exprimée en pixels </summary>
        internal int HauteurPixels
        {
            get
            {
                return RectanglePixels.Height;
            }
        }
        /// <summary> chaine représentant la surfaceTuiles </summary>
        public override string ToString()
        {
            return $"Col : {NumColDeb}, Row : {NumRangDeb}, NbCol : {NbColonnes}, NbRow : {NbRangees}, Nb Tuiles : {NbTuiles}";
        }

        /// <summary> décrit les données associées à une tuile qui contient un des 2 points définissant l'emprise d'une carte </summary>
        private class TuileServeurCarto
        {
            private readonly Point Indices;
            private readonly bool FlagNotEmpty;
            private readonly Echelles _Echelle;
            private readonly SitesCartographiques _Site;
            /// <summary> site web en relation avec la tuile </summary>
            internal SitesCartographiques Site
            {
                get
                {
                    if (!FlagNotEmpty)
                    {
                        return SitesCartographiques.Aucun;
                    }
                    else
                    {
                        return _Site;
                    }
                }
            }
            /// <summary> indice echelle (echelle ou zoom) du site web en relation avec la tuile </summary>
            internal Echelles Echelle
            {
                get
                {
                    if (!FlagNotEmpty)
                    {
                        return Echelles._000;
                    }
                    else
                    {
                        return _Echelle;
                    }
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
            /// <summary> Constructeur pour avoir une tuile vide </summary>
            internal TuileServeurCarto()
            {
            }
            /// <summary> creation de la tuile qui contient le pointGrille </summary>
            /// <param name="CoordonneesGrille"> point exprimé en mètre de la projection du serveur </param>
            /// <param name="Site"> site du serveur (indice dans le tableau des serveur connus) </param>
            /// <param name="Echelle"> indice de l'échelle demandée (donne le zoom du serveur) </param>
            /// <param name="IsFinTuile"> on se place à la fin de tuile pour le calcul des coordonnées </param>
            internal TuileServeurCarto(PointD CoordonneesGrille, SitesCartographiques Site, Echelles Echelle, bool IsFinTuile)
            {
                _Site = Site;
                _Echelle = Echelle;
                Indices = PointGrilleToTuile(CoordonneesGrille, Site, Echelle, IsFinTuile);
                FlagNotEmpty = true;
            }
            /// <summary> creation de la tuile qui contient le pointPixel </summary>
            /// <param name="CoordonneesPixel"> point exprimé en pixels du serveur </param>
            /// <param name="Site"> site du serveur (indice dans le tableau des serveur connus) </param>
            /// <param name="Echelle"> échelle demandée (donne le zoom du serveur) </param>
            /// <param name="IsFinTuile"> on se place à la fin de tuile pour le calcul des coordonnées </param>
            internal TuileServeurCarto(Point CoordonneesPixel, SitesCartographiques Site, Echelles Echelle, bool IsFinTuile)
            {
                _Site = Site;
                _Echelle = Echelle;
                Indices = PointPixelToTuile(CoordonneesPixel, IsFinTuile);
                FlagNotEmpty = true;
            }
            /// <summary> transforme les indices d'une tuile en point exprimé en mètre de la projection du serveur </summary>
            /// <param name="IsFinTuile"> si False on prend le haut et gauche sinon le bas et droit de la tuile </param>
            internal PointD ToGrille(bool IsFinTuile)
            {
                return TuileToPointGrille(Indices, _Site, _Echelle, IsFinTuile);
            }
            /// <summary> transforme les indices d'une tuile en point exprimé en pixels de la projection du serveur</summary>
            /// <param name="IsFinTuile"> si False on prend le haut et gauche sinon le bas et droit de la tuile </param>
            internal Point ToPixels(bool IsFinTuile)
            {
                return TuileToPointPixel(Indices, IsFinTuile);
            }
            /// <summary> numéro de colonne de la tuile </summary>
            internal int NumCol
            {
                get
                {
                    return Indices.X;
                }
            }
            /// <summary> numéro de rangée de la tuile </summary>
            internal int NumRang
            {
                get
                {
                    return Indices.Y;
                }
            }
        }
    }
}