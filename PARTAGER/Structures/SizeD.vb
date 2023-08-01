Friend Structure SizeD
    Private ReadOnly FlagNotEmpty As Boolean
    Friend Shared Function Add(Taille1 As SizeD, Taille2 As SizeD) As SizeD
        Return New SizeD(Taille1.Largeur + Taille2.Largeur, Taille1.Hauteur + Taille2.Hauteur)
    End Function
    Friend Shared Function Scale(Taille As SizeD, Facteur As Double) As SizeD
        Return New SizeD(Taille.Largeur * Facteur, Taille.Hauteur * Facteur)
    End Function
    Friend Shared ReadOnly Empty As SizeD
    Friend Sub New(Point As PointD)
        _Largeur = Point.X
        _Hauteur = Point.Y
        FlagNotEmpty = True
    End Sub
    Friend Sub New(Size As Size)
        _Largeur = Size.Width
        _Hauteur = Size.Height
        FlagNotEmpty = True
    End Sub
    Friend Sub New(Largeur As Double, Hauteur As Double)
        _Largeur = Largeur
        _Hauteur = Hauteur
        FlagNotEmpty = True
    End Sub
    Friend ReadOnly Property IsEmpty() As Boolean
        Get
            Return Not FlagNotEmpty
        End Get
    End Property
    ''' <summary> renvoie la largeur du rectangle </summary>
    Friend ReadOnly Property Largeur As Double
    ''' <summary> renvoie la hauteur du rectangle </summary>
    Friend ReadOnly Property Hauteur As Double
    ''' <summary> renvoie une size à partir du sizeD avec un arrondi Sup</summary>
    Friend ReadOnly Property ToSize() As Size
        Get
            Return New Size(CInt(Math.Round(_Largeur)), CInt(Math.Round(_Hauteur)))
        End Get
    End Property
    Friend ReadOnly Property ToPoint() As Point
        Get
            Return New Point(ToSize)
        End Get
    End Property
    ''' <summary> renvoie sizeF à partir du sizeD </summary>
    Friend ReadOnly Property ToSizeF() As SizeF
        Get
            Return New SizeF(CSng(_Largeur), CSng(_Hauteur))
        End Get
    End Property
    Friend ReadOnly Property ToPoinF() As PointF
        Get
            Return New PointF(CSng(_Largeur), CSng(_Hauteur))
        End Get
    End Property
    ''' <summary> chaine représentant le PointD </summary> 
    Public Overrides Function ToString() As String
        Return $"Width = {_Largeur} Height = {_Hauteur}"
    End Function
End Structure
