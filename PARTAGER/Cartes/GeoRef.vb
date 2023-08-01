''' <summary> permet la gestion d'un coin de géoréférencement de la carte. </summary>
''' <remarks> toutes les cartes ont des coordonnées en DD quelque soit les coordonnées d'origines </remarks>
Friend Class GeoRef
    ''' <summary> Coordonnées en pixel par rapport au coin haut gauche de l'image de la carte </summary>
    Friend Pixels As Point
    ''' <summary> Coordonnée X en pixel par rapport au coin haut gauche de l'image de la carte </summary>
    Friend Property X_Pixel As Integer
        Get
            Return Pixels.X
        End Get
        Set
            Pixels.X = Value
        End Set
    End Property
    ''' <summary> Coordonnées Y en pixel par rapport au coin haut gauche de l'image de la carte </summary>
    Friend Property Y_Pixel As Integer
        Get
            Return Pixels.Y
        End Get
        Set(value As Integer)
            Pixels.Y = value
        End Set
    End Property
    ''' <summary> Coordonnées X, Y en mètres capturées lors de la capture d'écran ou calculer lors du téléchargement convertie en nombre </summary>
    Friend Grille As PointD
    ''' <summary> Coordonnée X en mètres </summary>
    Friend Property X_Grid As Double
        Get
            Return Grille.X
        End Get
        Set(value As Double)
            Grille.X = value
        End Set
    End Property
    ''' <summary> Coordonnée Y en mètres </summary>
    Friend Property Y_Grid As Double
        Get
            Return Grille.Y
        End Get
        Set(value As Double)
            Grille.Y = value
        End Set
    End Property
    ''' <summary> Longitude, Latitude du point en WGS84 exprimé en degrés décimaux </summary>
    Friend LatLon As PointD
    ''' <summary> Longitude du point en WGS84 exprimé en degrés décimaux </summary>
    Friend Property Longitude As Double
        Get
            Return LatLon.Lon
        End Get
        Set(value As Double)
            LatLon.Lon = value
        End Set
    End Property
    ''' <summary> Latitude du point en WGS84 exprimé en degrés décimaux </summary>
    Friend Property Latitude As Double
        Get
            Return LatLon.Lat
        End Get
        Set(value As Double)
            LatLon.Lat = value
        End Set
    End Property
    ''' <summary>Coordonnées X, Y ou LonLat capturées sous forme de texte lors de la capture d'écran ou calculer lors du téléchargement </summary>
    Friend CoordonneesCaptureEcran As String
    ''' <summary> Coord. X, Y UTM WGS84 issue de longitude et latitude. Il peut y avoir plus d'une Zone UTM (en mètres) pour les cartes à cheval sur 2 ou 3 zones </summary>
    Friend UTMs() As PointD
    ''' <summary> Numéro de Zone minimal des coordonnées UTM du coin(0) (entre 30 et 32 pour la france)</summary>
    Friend NumZone_UTM As Integer
    ''' <summary> Hemisphère des coordonnées UTM (Nord pour la france) </summary>
    Friend Hemisphere_UTM As Char
    ''' <summary> redimensionne le tableau des coordonnées UTM </summary>
    Friend Sub New(NbZonesUTM As Integer)
        ReDim UTMs(NbZonesUTM - 1)
    End Sub
End Class