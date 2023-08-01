''' <summary> décrit un point avec des coordonnées qui sont exprimées en pixels d'un zoom ou echelle d'un site carto en indices tuile et offset </summary>
Friend Structure PointG
#Region "Partagé"
    ''' <summary> renvoie un pointG qui est egal à point.add(point.location,taille) </summary>
    Friend Shared Function Add(Point As PointG, Taille As Size) As PointG
        Point.Add(Taille)
        Return Point
    End Function
    ''' <summary> renvoie un pointG qui est egal à point.offset(location) </summary>
    Friend Shared Function Add(Point As PointG, Location As Point) As PointG
        Point.Add(Location)
        Return Point
    End Function
    ''' <summary> renvoie un pointG qui est egal à point.substract(point.location,taille)</summary>
    Friend Shared Function Substract(Point As PointG, Taille As Size) As PointG
        Point.Substract(Taille)
        Return Point
    End Function
    ''' <summary> renvoie un pointG qui est egal à point.X-location.X et point.y-location.y </summary>
    Friend Shared Function Substract(Point As PointG, Location As Point) As PointG
        Point.Substract(Location)
        Return Point
    End Function
    ''' <summary> opérateur qui détermine si un pointG est egal à un autre pointG </summary>
    Public Shared Operator =(Pt1 As PointG, Pt2 As PointG) As Boolean
        Return Pt1.Egal(Pt2)
    End Operator
    ''' <summary> opérateur qui détermine si un pointG est différent d'un autre pointG </summary>
    Public Shared Operator <>(Pt1 As PointG, Pt2 As PointG) As Boolean
        Return Pt1.Different(Pt2)
    End Operator
    Friend Shared ReadOnly Empty As PointG
#End Region
#Region "Instance"
    Public Overrides Function GetHashCode() As Integer
        Return _IndiceEchelle.GetHashCode() Or _Location.GetHashCode
    End Function
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is PointG Then
            Dim Pt As PointG = DirectCast(obj, PointG)
            Return Egal(Pt)
        End If
        Return False
    End Function
    Friend Function Egal(Pt As PointG) As Boolean
        Return FlagNotEmpty = Pt.FlagNotEmpty AndAlso _IndiceEchelle = Pt.IndiceEchelle AndAlso _Location = Pt._Location
    End Function
    ''' <summary> détermine si le pointD est différent d'un autre pointd </summary>
    Friend Function Different(Pt As PointG) As Boolean
        Return FlagNotEmpty <> Pt.FlagNotEmpty OrElse _IndiceEchelle <> Pt.IndiceEchelle OrElse _Location <> Pt.Location
    End Function
    ''' <summary> additionne la taille à la location du point </summary>
    Friend Sub Add(Taille As Size)
        _Location = Point.Add(_Location, Taille)
        CalculerIndexTuile()
    End Sub
    ''' <summary> decale la location du point de location</summary>
    Friend Sub Add(Location As Point)
        _Location.Offset(Location)
        CalculerIndexTuile()
    End Sub
    ''' <summary> soustrait la taille à la location du point </summary>
    Friend Sub Substract(taille As Size)
        _Location = Point.Subtract(_Location, taille)
        CalculerIndexTuile()
    End Sub
    ''' <summary> decale la location du point de location</summary>
    Friend Sub Substract(Location As Point)
        _Location.X -= Location.X
        _Location.Y -= Location.Y
        CalculerIndexTuile()
    End Sub
    ''' <summary> stockage de l'index des tuiles </summary>
    Private IndexTuileX, IndexTuileY As Integer, _Offset As Size
    ''' <summary> calcul l'index des tuiles de la location du point </summary>
    Private Sub CalculerIndexTuile()
        IndexTuileX = PixelToNumTile(_Location.X, False)
        IndexTuileY = PixelToNumTile(_Location.Y, False)
        _Offset = New Size(_Location.X - IndexTuileX * NbPixelsTuile, _Location.Y - IndexTuileY * NbPixelsTuile)
    End Sub
    ''' <summary> calcul la location du point exprimé en pixels </summary>
    Private Sub CalculerLocation()
        _Location.X = IndexTuileX * NbPixelsTuile + _Offset.Width
        _Location.Y = IndexTuileY * NbPixelsTuile + _Offset.Height
    End Sub
    ''' <summary> flag indiquant que si la structure a été initialisée </summary>
    Private ReadOnly FlagNotEmpty As Boolean
    '''' <summary> indice des valeurs Echelle ou Zoom correspondant à coordonnées </summary>
    Friend ReadOnly Property IndiceEchelle As Integer
    ''' <summary> coordonnées X en pixels du système WebMercator </summary>
    Friend ReadOnly Property X As Integer
        Get
            Return _Location.X
        End Get
    End Property
    ''' <summary> coordonnées Y en pixels du système WebMercator </summary>
    Friend ReadOnly Property Y As Integer
        Get
            Return _Location.Y
        End Get
    End Property
    ''' <summary> coordonnées en pixels du système WebMercator </summary>
    Friend ReadOnly Property Location As Point
    ''' <summary> initialisation de la structure à partir d'un niveau de zoom et de 2 entiers pour les coordonnées </summary>
    Friend Sub New(IndiceEchelle As Integer, X As Integer, Y As Integer)
        _Location.X = X
        _Location.Y = Y
        _IndiceEchelle = IndiceEchelle
        CalculerIndexTuile()
        FlagNotEmpty = True
    End Sub
    ''' <summary> initialisation de la structure à partir d'un niveau de zoom et d'un point pour les coordonnées </summary>
    Friend Sub New(IndiceEchelle As Integer, Location As Point)
        _Location = Location
        _IndiceEchelle = IndiceEchelle
        CalculerIndexTuile()
        FlagNotEmpty = True
    End Sub
    ''' <summary> initialisation de la structure à partir d'un niveau de zoom et d'une tuile (point) et d'un décalagePixels (size) </summary>
    Friend Sub New(IndiceEchelle As Integer, IndexTuile As Point, Offset As Size)
        _IndiceEchelle = IndiceEchelle
        IndexTuileX = IndexTuile.X
        IndexTuileY = IndexTuile.Y
        _Offset = Offset
        CalculerLocation()
        FlagNotEmpty = True
    End Sub
    ''' <summary> initialisation de la structure à partir d'un niveau de zoom et de 4 entier </summary>
    Friend Sub New(IndiceEchelle As Integer, IndexTuileX As Integer, IndexTuileY As Integer, OffsetX As Integer, OffsetY As Integer)
        _IndiceEchelle = IndiceEchelle
        Me.IndexTuileX = IndexTuileX
        Me.IndexTuileY = IndexTuileY
        _Offset = New Size(OffsetX, OffsetY)
        CalculerLocation()
        FlagNotEmpty = True
    End Sub
    ''' <summary> tuile contenant les coordonnées </summary>
    Friend ReadOnly Property IndexTuile As Point
        Get
            Return New Point(IndexTuileX, IndexTuileY)
        End Get
    End Property
    ''' <summary> valeur exprimée en pixels du décalage par rapport au début de tuile </summary>
    Friend ReadOnly Property Offset As Size
        Get
            Return _Offset
        End Get
    End Property
    ''' <summary> coordonnées en pixels du système WebMercator du Point Haut Gauche de la tuile contenant le point </summary>
    Friend ReadOnly Property Pt0Tuile As Point
        Get
            Return New Point(IndexTuileX * NbPixelsTuile, IndexTuileY * NbPixelsTuile)
        End Get
    End Property
    ''' <summary> True si la structure a été initialisée avec le constructeur </summary>
    Friend ReadOnly Property IsEmpty() As Boolean
        Get
            Return Not FlagNotEmpty
        End Get
    End Property
    ''' <summary> chaine représentant le PointG </summary>
    Public Overloads Function ToString() As String
        Return $"Col={IndexTuileX} Row={IndexTuileY} X={_Offset.Width} Y={_Offset.Height}"
    End Function
#End Region
End Structure
