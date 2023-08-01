''' <summary> structure qui sert à véhiculer un point de coordonnées Grille (X,Y ou PointD, Zone et Hémisphère pour un point de coordonnées UTM </summary>
Friend Structure PointProjection
    Friend Property X As Double
        Get
            Return Coordonnees.X
        End Get
        Set(value As Double)
            Coordonnees.X = value
        End Set
    End Property
    Friend Property Y As Double
        Get
            Return Coordonnees.Y
        End Get
        Set(value As Double)
            Coordonnees.Y = value
        End Set
    End Property
    Friend Coordonnees As PointD
    Friend Zone As Integer
    Friend Hemisphere As Char
    Friend Sub New(Point As PointD, Optional Z As Integer = 0, Optional H As Char = Nothing)
        Coordonnees = Point
        Zone = Z
        Hemisphere = H
    End Sub
End Structure