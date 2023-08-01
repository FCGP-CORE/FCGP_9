''' <summary> décrit un point avec des coordonnées qui sont des doubles </summary>
Friend Structure PointD
#Region "Partagé"
    ''' <summary> renvoie un nouveau pointD décalé de la valeur de l'offset </summary>
    ''' <param name="Point"> PointD de départ </param>
    ''' <param name="X"> décalage X du point </param>
    ''' <param name="Y"> décalage Y du point </param>
    Friend Shared Function Offset(Point As PointD, X As Double, Y As Double) As PointD
        Point.Offset(X, Y)
        Return Point
    End Function
    ''' <summary> renvoie un nouveau pointD décalé de la valeur de sizeD </summary>
    ''' <param name="Point"> PointD de départ </param>
    ''' <param name="Taille"> décalage X et Y du point </param>
    Friend Shared Function Add(Point As PointD, Taille As SizeD) As PointD
        Point.Add(Taille)
        Return Point
    End Function
    ''' <summary> renvoie un nouveau pointD étendu par le facteur </summary>
    ''' <param name="Point"> PointD de départ </param>
    ''' <param name="Facteur"></param>
    ''' <returns></returns>
    Friend Shared Function Scale(Point As PointD, Facteur As Double) As PointD
        Point.Scale(Facteur)
        Return Point
    End Function
    ''' <summary> opérateur qui détermine si un pointD est egal à un autre pointd </summary>
    Public Shared Operator =(Pt1 As PointD, Pt2 As PointD) As Boolean
        Return Pt1.Egal(Pt2)
    End Operator

    ''' <summary> opérateur qui détermine si un pointD est différent d'un autre pointd </summary>
    Public Shared Operator <>(Pt1 As PointD, Pt2 As PointD) As Boolean
        Return Pt1.Different(Pt2)
    End Operator
    ''' <summary> renvoie un  pointD qui est non initialisé avec IsEmpty = true </summary>
    Friend Shared ReadOnly Empty As PointD
#End Region
#Region "Instance"
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is PointD Then
            Dim Pt As PointD = DirectCast(obj, PointD)
            Return Egal(Pt)
        End If
        Return False
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return _X.GetHashCode() Or _Y.GetHashCode
    End Function

    ''' <summary> détermine si le pointD est egal à un autre pointd </summary>
    Friend Function Egal(Pt As PointD) As Boolean
        Return FlagNotEmpty = Pt.FlagNotEmpty AndAlso _X = Pt.X AndAlso _Y = Pt.Y
    End Function
    ''' <summary> détermine si le pointD est différent d'un autre pointd </summary>
    Friend Function Different(Pt As PointD) As Boolean
        Return FlagNotEmpty <> Pt.FlagNotEmpty OrElse _X <> Pt.X OrElse _Y <> Pt.Y
    End Function
    ''' <summary> décale le point de la valeur de l'offset </summary>
    ''' <param name="X"> décalage X du point </param>
    ''' <param name="Y"> décalage Y du point </param>
    Friend Sub Offset(X As Double, Y As Double)
        _X += X
        _Y += Y
    End Sub
    ''' <summary> décale le point de la valeur de sizeD </summary>
    ''' <param name="Taille"> décalage X et Y du point </param>
    Friend Sub Add(Taille As SizeD)
        _X += Taille.Largeur
        _Y += Taille.Hauteur
    End Sub
    ''' <summary> etend les coordonnées du point à partir d'un facteur </summary>
    ''' <param name="Facteur"></param>
    Friend Sub Scale(Facteur As Double)
        _X *= Facteur
        _Y *= Facteur
    End Sub

    Private ReadOnly FlagNotEmpty As Boolean
    Private _X As Double, _Y As Double
    ''' <summary> initialise la structure </summary>
    ''' <param name="X"> coordonnée X du point </param>
    ''' <param name="Y"> coordonnée Y du point </param>
    Friend Sub New(X As Double, Y As Double)
        _X = X
        _Y = Y
        FlagNotEmpty = True
    End Sub
    ''' <summary> initialise la structure </summary>
    ''' <param name="Location"> point </param>
    Friend Sub New(Location As Point)
        _X = Location.X
        _Y = Location.Y
        FlagNotEmpty = True
    End Sub
    ''' <summary> chaine représentant le PointD </summary>
    Public Overrides Function ToString() As String
        Return $"X = {_X} Y = {_Y}"
    End Function

    ''' <summary> renvoie la coordonnée X du point </summary>
    Friend Property X As Double
        Get
            Return _X
        End Get
        Set(value As Double)
            _X = value
        End Set
    End Property
    ''' <summary> renvoie la coordonnée Longitude du point si celui-ci est considré comme un point LatLon</summary>
    Friend Property Lon As Double
        Get
            Return _X
        End Get
        Set(value As Double)
            _X = value
        End Set
    End Property
    ''' <summary> renvoie la coordonnée y du point </summary>
    Friend Property Y As Double
        Get
            Return _Y
        End Get
        Set(value As Double)
            _Y = value
        End Set
    End Property
    ''' <summary> renvoie la coordonnée Latitude du point si celui-ci est considré comme un point LatLon</summary>
    Friend Property Lat As Double
        Get
            Return _Y
        End Get
        Set(value As Double)
            _Y = value
        End Set
    End Property
    ''' <summary> renvoi un Point à partir du pointD </summary>
    Friend ReadOnly Property ToPoint() As Point
        Get
            Return New Point(CInt(Math.Round(_X)), CInt(Math.Round(_Y)))
        End Get
    End Property
    ''' <summary> renvoi un PointF à partir du pointD </summary>
    Friend ReadOnly Property ToPointF() As PointF
        Get
            Return New PointF(CSng(_X), CSng(_Y))
        End Get
    End Property
    ''' <summary> True si la structure a été initialisée avec le constructeur par defaut(sans paramètres) </summary>
    Friend ReadOnly Property IsEmpty() As Boolean
        Get
            Return Not FlagNotEmpty
        End Get
    End Property
#End Region
End Structure