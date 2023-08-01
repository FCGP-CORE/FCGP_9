''' <summary> décrit un rectangle avec des coordonnées qui sont des doubles </summary>
Friend Structure RectangleD
    Private ReadOnly FlagNotEmpty As Boolean
    Friend Shared ReadOnly Empty As RectangleD
    ''' <summary> opérateur qui détermine si un pointD est egal à un autre pointd </summary>
    Public Shared Operator =(Rd1 As RectangleD, Rd2 As RectangleD) As Boolean
        Return Rd1.Egal(Rd2)
    End Operator

    ''' <summary> opérateur qui détermine si un pointD est différent d'un autre pointd </summary>
    Public Shared Operator <>(Rd1 As RectangleD, Rd2 As RectangleD) As Boolean
        Return Rd1.Different(Rd2)
    End Operator

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is RectangleD Then
            Dim Rd As RectangleD = DirectCast(obj, RectangleD)
            Return Egal(Rd)
        End If
        Return False
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return _Pt0.GetHashCode() Or _Pt2.GetHashCode()
    End Function
    ''' <summary> détermine si le rectangleD est egal à un autre rectangleD </summary>
    Friend Function Egal(Rd As RectangleD) As Boolean
        Return FlagNotEmpty = Rd.FlagNotEmpty AndAlso _Pt0 = Rd.Pt0 AndAlso _Pt2 = Rd.Pt2
    End Function
    ''' <summary> détermine si le rectangleD est différent d'un autre rectangleD </summary>
    Friend Function Different(Rd As RectangleD) As Boolean
        Return FlagNotEmpty <> Rd.FlagNotEmpty OrElse _Pt0 <> Rd.Pt0 OrElse _Pt2 <> Rd.Pt2
    End Function
    ''' <summary> initialise la structure à partir de 2 pointsD </summary>
    ''' <param name="Pt0"> point haut et gauche du rectangle </param>
    ''' <param name="Pt2"> point bas et droit du rectangle </param>
    Friend Sub New(Pt0 As PointD, Pt2 As PointD)
        _Pt0 = Pt0
        _Pt2 = Pt2
        FlagNotEmpty = True
    End Sub
    ''' <summary> initialise la structure à partir de 4 doubles </summary>
    ''' <param name="X0"> gauche du rectangle </param>
    ''' <param name="Y0"> haut du rectangle </param>
    ''' <param name="X2"> droit du rectangle </param>
    ''' <param name="Y2"> bas du rectangle </param>
    Friend Sub New(X0 As Double, Y0 As Double, X2 As Double, Y2 As Double)
        _Pt0 = New PointD(X0, Y0)
        _Pt2 = New PointD(X2, Y2)
        FlagNotEmpty = True
    End Sub
    ''' <summary> initialise la structure à partir de 4 doubles </summary>
    ''' <param name="Pt0"> location du rectangle </param>
    ''' <param name="Taille"> taille du rectangle </param>
    Friend Sub New(Pt0 As PointD, Taille As SizeD)
        _Pt0 = Pt0
        _Pt2 = PointD.Add(Pt0, Taille)
        FlagNotEmpty = True
    End Sub
    ''' <summary> renvoie le point haut et gauche ou NordOuest du rectangle </summary>
    Friend Property Pt0 As PointD
    ''' <summary> point bas et droit du rectangle ou SudEst </summary>
    Friend Property Pt2 As PointD
    ''' <summary> renvoie le point demandé </summary>
    ''' <param name="Item"> numéro du point demandé </param>
    Friend ReadOnly Property Pt(Item As Integer) As PointD
        Get
            Select Case Item
                Case 0
                    Return _Pt0
                Case 2
                    Return _Pt2
                Case 1
                    Return New PointD(_Pt2.X, _Pt0.Y)
                Case 3
                    Return New PointD(_Pt0.X, _Pt2.Y)
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    ''' <summary> True si la structure a été initialisée avec un constructeur public</summary>
    Friend ReadOnly Property IsEmpty() As Boolean
        Get
            Return Not FlagNotEmpty
        End Get
    End Property
    ''' <summary> renvoie la largeur du rectangle </summary>
    Friend ReadOnly Property Largeur As Double
        Get
            Return _Pt2.X - _Pt0.X
        End Get
    End Property
    ''' <summary> renvoie la hauteur du rectangle </summary>
    Friend ReadOnly Property Hauteur As Double
        Get
            Return _Pt2.Y - _Pt0.Y
        End Get
    End Property
    ''' <summary> renvoie la largeur du rectangle mais dans un contexte de coordonnées. =largeur </summary>
    Friend ReadOnly Property LargeurCoordonnees As Double
        Get
            Return _Pt2.X - _Pt0.X
        End Get
    End Property
    ''' <summary> renvoie la hauteur du rectangle mais dans un contexte de coordonnées. =-hauteur </summary>
    Friend ReadOnly Property HauteurCoordonnees As Double
        Get
            Return _Pt0.Y - _Pt2.Y
        End Get
    End Property
    Friend ReadOnly Property Taille As SizeD
        Get
            Return New SizeD(_Pt2.X - _Pt0.X, _Pt2.Y - _Pt0.Y)
        End Get
    End Property
    ''' <summary> Pt0.X </summary>
    Friend Property X0 As Double
        Get
            Return _Pt0.X
        End Get
        Set(value As Double)
            _Pt0.X = value
        End Set
    End Property
    ''' <summary> Pt2.X </summary>
    Friend Property X2 As Double
        Get
            Return _Pt2.X
        End Get
        Set(value As Double)
            _Pt2.X = value
        End Set
    End Property
    ''' <summary> PT0.Y </summary>
    Friend Property Y0 As Double
        Get
            Return _Pt0.Y
        End Get
        Set(value As Double)
            _Pt0.Y = value
        End Set
    End Property
    ''' <summary> PT2.Y </summary>
    Friend Property Y2 As Double
        Get
            Return _Pt2.Y
        End Get
        Set(value As Double)
            _Pt2.Y = value
        End Set
    End Property
    ''' <summary> Pt0.X mais dans un contexte de coordonnées </summary>
    Friend Property Ouest() As Double
        Get
            Return _Pt0.X
        End Get
        Set(value As Double)
            _Pt0.X = value
        End Set
    End Property
    ''' <summary> Pt2.X mais dans un contexte de coordonnées </summary>
    Friend Property Est() As Double
        Get
            Return _Pt2.X
        End Get
        Set(value As Double)
            _Pt2.X = value
        End Set
    End Property
    ''' <summary> Pt0.Y mais dans un contexte de coordonnées </summary>
    Friend Property Nord() As Double
        Get
            Return _Pt0.Y
        End Get
        Set(value As Double)
            _Pt0.Y = value
        End Set
    End Property
    ''' <summary> Pt2.Y mais dans un contexte de coordonnées </summary>
    Friend Property Sud() As Double
        Get
            Return _Pt2.Y
        End Get
        Set(value As Double)
            _Pt2.Y = value
        End Set
    End Property

    ''' <summary> renvoie rectangle à partir du rectangleD </summary>
    Friend ReadOnly Property ToRectangle() As Rectangle
        Get
            Return New Rectangle(_Pt0.ToPoint(), Taille.ToSize)
        End Get
    End Property
    ''' <summary> renvoie rectangleF à partir du rectangleD </summary>
    Friend ReadOnly Property ToRectangleF() As RectangleF
        Get
            Return New RectangleF(_Pt0.ToPointF(), New SizeF(CSng(Largeur), CSng(Hauteur)))
        End Get
    End Property
    ''' <summary> indique si le rectangle contient le point </summary>
    ''' <param name="Point"> point à tester </param>
    Friend Function Contains(Point As PointD) As Boolean
        Return _Pt0.X <= Point.X AndAlso Point.X < _Pt2.X AndAlso _Pt0.Y <= Point.Y AndAlso Point.Y < _Pt2.Y
    End Function
    ''' <summary> indique si le rectangle contient le point mais dans un contexte de coordonnées </summary>
    ''' <param name="Point"> point à tester </param>
    Friend Function CoordonneesContains(Point As PointD) As Boolean
        Return _Pt0.X <= Point.X AndAlso Point.X < _Pt2.X AndAlso _Pt0.Y >= Point.Y AndAlso Point.Y > _Pt2.Y
    End Function
    ''' <summary> indique si le rectangle contient entièrement le rectangle </summary>
    ''' <param name="Rect"> rectangle à tester </param>
    Friend Function Contains(Rect As RectangleD) As Boolean
        Return _Pt0.X <= Rect._Pt0.X AndAlso Rect._Pt2.X <= _Pt2.X AndAlso _Pt0.Y <= Rect._Pt0.Y AndAlso Rect._Pt2.Y <= _Pt2.Y
    End Function
    ''' <summary> indique si le rectangle contient entièrement le rectangle mais dans un contexte de coordonnées </summary>
    ''' <param name="Rect"> rectangle à tester </param>
    Friend Function CoordonneesContains(Rect As RectangleD) As Boolean
        Return _Pt0.X <= Rect._Pt0.X AndAlso Rect._Pt2.X <= _Pt2.X AndAlso _Pt0.Y >= Rect._Pt0.Y AndAlso Rect._Pt2.Y >= _Pt2.Y
    End Function
    ''' <summary> indique si le rectangle a une intersection le rectangle </summary>
    ''' <param name="Rect"> rectangle à tester </param>
    Friend Function IntersectsWith(rect As RectangleD) As Boolean
        Return _Pt0.X < rect._Pt2.X AndAlso rect._Pt0.X < _Pt2.X AndAlso rect._Pt0.Y < _Pt2.Y AndAlso _Pt0.Y < rect._Pt2.Y
    End Function
    ''' <summary> indique si le rectangle a une intersection le rectangle mais dans un contexte de coordonnées </summary>
    ''' <param name="Rect"> rectangle à tester </param>
    Friend Function CoordonneesIntersectsWith(Rect As RectangleD) As Boolean
        Return _Pt0.X < Rect._Pt2.X AndAlso Rect._Pt0.X < _Pt2.X AndAlso Rect._Pt0.Y > _Pt2.Y AndAlso _Pt0.Y > Rect._Pt2.Y
    End Function
    ''' <summary> chaine représentant le RectangleD </summary>
    Public Overrides Function ToString() As String
        Return $"X = {_Pt0.X} Y = {_Pt0.Y} Width = {Largeur} Height = {Hauteur}"
    End Function
End Structure