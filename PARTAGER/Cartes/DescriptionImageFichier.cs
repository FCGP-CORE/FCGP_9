namespace FCGP
{
    /// <summary> regroupement des données concernant une image représentant une région cartographique
    /// Tuile de fichier tuile, carte ou page à imprimer </summary>
    internal class DescriptionImageFichier
    {
        /// <summary> nom de l'image qui peut être un chemin pour un fichier ou juste un nom pour un élement en mémoire </summary>
        internal string Nom;
        /// <summary> coordonnées réelles sous différentes formes </summary>
        internal double Nord
        {
            get
            {
                return Region.Nord;
            }
            set
            {
                Region.Nord = value;
            }
        }
        internal double Ouest
        {
            get
            {
                return Region.Ouest;
            }
            set
            {
                Region.Ouest = value;
            }
        }
        internal double Est
        {
            get
            {
                return Region.Est;
            }
            set
            {
                Region.Est = value;
            }
        }
        internal double Sud
        {
            get
            {
                return Region.Sud;
            }
            set
            {
                Region.Sud = value;
            }
        }
        internal RectangleD Region;
        internal int LargeurPixels
        {
            get
            {
                return TaillePixels.Width;
            }
            set
            {
                TaillePixels.Width = value;
            }
        }

        internal int HauteurPixels
        {
            get
            {
                return TaillePixels.Height;
            }
            set
            {
                TaillePixels.Height = value;
            }
        }
        /// <summary> taille de la tuile exprimée en pixels </summary>
        internal Size TaillePixels;
    }
}