namespace FCGP
{
    /// <summary> permet la gestion d'un coin de géoréférencement de la carte. </summary>
    /// <remarks> toutes les cartes ont des coordonnées en DD quelque soit les coordonnées d'origines </remarks>
    internal class GeoRef
    {
        /// <summary> Coordonnées en pixel par rapport au coin haut gauche de l'image de la carte </summary>
        internal Point Pixels;
        /// <summary> Coordonnée X en pixel par rapport au coin haut gauche de l'image de la carte </summary>
        internal int X_Pixel
        {
            get
            {
                return Pixels.X;
            }
            set
            {
                Pixels.X = value;
            }
        }
        /// <summary> Coordonnées Y en pixel par rapport au coin haut gauche de l'image de la carte </summary>
        internal int Y_Pixel
        {
            get
            {
                return Pixels.Y;
            }
            set
            {
                Pixels.Y = value;
            }
        }
        /// <summary> Coordonnées X, Y en mètres capturées lors de la capture d'écran ou calculer lors du téléchargement convertie en nombre </summary>
        internal PointD Grille;
        /// <summary> Coordonnée X en mètres </summary>
        internal double X_Grid
        {
            get
            {
                return Grille.X;
            }
            set
            {
                Grille.X = value;
            }
        }
        /// <summary> Coordonnée Y en mètres </summary>
        internal double Y_Grid
        {
            get
            {
                return Grille.Y;
            }
            set
            {
                Grille.Y = value;
            }
        }
        /// <summary> Longitude, Latitude du point en WGS84 exprimé en degrés décimaux </summary>
        internal PointD LatLon;
        /// <summary> Longitude du point en WGS84 exprimé en degrés décimaux </summary>
        internal double Longitude
        {
            get
            {
                return LatLon.Lon;
            }
            set
            {
                LatLon.Lon = value;
            }
        }
        /// <summary> Latitude du point en WGS84 exprimé en degrés décimaux </summary>
        internal double Latitude
        {
            get
            {
                return LatLon.Lat;
            }
            set
            {
                LatLon.Lat = value;
            }
        }
        /// <summary>Coordonnées X, Y ou LonLat capturées sous forme de texte lors de la capture d'écran ou calculer lors du téléchargement </summary>
        internal string CoordonneesCaptureEcran;
        /// <summary> Coord. X, Y UTM WGS84 issue de longitude et latitude. Il peut y avoir plus d'une Zone UTM (en mètres) pour les cartes à cheval sur 2 ou 3 zones </summary>
        internal PointD[] UTMs;
        /// <summary> Numéro de Zone minimal des coordonnées UTM du coin(0) (entre 30 et 32 pour la france)</summary>
        internal int NumZone_UTM;
        /// <summary> Hemisphère des coordonnées UTM (Nord pour la france) </summary>
        internal char Hemisphere_UTM;
        /// <summary> redimensionne le tableau des coordonnées UTM </summary>
        internal GeoRef(int NbZonesUTM)
        {
            UTMs = new PointD[NbZonesUTM];
        }
    }
}