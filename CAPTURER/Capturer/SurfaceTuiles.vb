''' <summary> Conteneur servant à décrire une surface (rectangle) qui est composée par des tuiles de serveur. 
''' aussi appelé surface de traitement car il y a 3 possibilité d'utilisation :
''' téléchargement d'une carte, suppression de tuiles dans le cache et affichage d'une carte en 3D </summary>
Friend Class SurfaceTuiles
    Private _Site As SitesCartographiques, _Echelle As Echelles, Tuile0 As TuileServeurCarto, Tuile2 As TuileServeurCarto, FlagNotEmpty As Boolean
    ''' <summary> constructeur pour une instance vide </summary>
    Friend Sub New()
        Tuile0 = New TuileServeurCarto
        Tuile2 = New TuileServeurCarto
    End Sub
    ''' <summary> gère une surface composée de tuiles </summary>
    ''' <param name="SurfaceGrille"> rectangleD dont les dimensions sont exprimées en mètres de la projection du serveur carto </param>
    ''' <param name="SiteCarto"> site du serveur carto </param>
    ''' <param name="Echelle"> indice de l'échelle du serveur carto </param>
    Friend Sub New(SurfaceGrille As RectangleD, SiteCarto As SitesCartographiques, Echelle As Echelles)
        Me.New
        If SurfaceGrille.IsEmpty Then Exit Sub
        CalculerTuiles(SurfaceGrille, SiteCarto, Echelle)
    End Sub
    ''' <summary> gère une surface composée de tuiles </summary>
    ''' <param name="Point1"> PointD dont les dimensions sont exprimées en mètres de la projection du serveur carto </param>
    ''' <param name="Point2"> PointD dont les dimensions sont exprimées en mètres de la projection du serveur carto </param>
    ''' <param name="SiteCarto"> site du serveur carto </param>
    ''' <param name="Echelle"> indice de l'échelle du serveur carto </param>
    Friend Sub New(Point1 As PointD, Point2 As PointD, SiteCarto As SitesCartographiques, Echelle As Echelles)
        Me.New
        If Point1.IsEmpty OrElse Point2.IsEmpty Then Exit Sub
        Dim SurfaceGrille As New RectangleD(Math.Min(Point1.X, Point2.X), Math.Max(Point1.Y, Point2.Y),
                                                         Math.Max(Point1.X, Point2.X), Math.Min(Point1.Y, Point2.Y))
        CalculerTuiles(SurfaceGrille, SiteCarto, Echelle)
    End Sub
    ''' <summary> gère une surface composée de tuiles </summary>
    ''' <param name="SurfacePixels"> rectangle dont les dimensions sont exprimées en pixels de la projection et de l'échelle du serveur carto </param>
    ''' <param name="SiteCarto"> site du serveur carto </param>
    ''' <param name="Echelle"> indice de l'échelle du serveur carto </param>
    Friend Sub New(SurfacePixels As Rectangle, SiteCarto As SitesCartographiques, Echelle As Echelles)
        Me.New
        If SurfacePixels.IsEmpty Then Exit Sub
        CalculerTuiles(SurfacePixels, SiteCarto, Echelle)
    End Sub
    ''' <summary> gère une surface composée de tuiles </summary>
    ''' <param name="Point1"> Point dont les dimensions sont exprimées en pixels de la projection et de l'échelle du serveur carto </param>
    ''' <param name="Point2"> Point dont les dimensions sont exprimées en pixels de la projection et de l'échelle du serveur carto </param>
    ''' <param name="SiteCarto"> site du serveur carto </param>
    ''' <param name="Echelle"> indice de l'échelle du serveur carto </param>
    Friend Sub New(Point1 As Point, Point2 As Point, SiteCarto As SitesCartographiques, Echelle As Echelles)
        Me.New
        If Point1.IsEmpty OrElse Point2.IsEmpty Then Exit Sub
        Dim SurfacePixels As New Rectangle(Math.Min(Point1.X, Point2.X), Math.Min(Point1.Y, Point2.Y),
                                                         Math.Max(Point1.X, Point2.X) - Math.Min(Point1.X, Point2.X),
                                                         Math.Max(Point1.Y, Point2.Y) - Math.Min(Point1.Y, Point2.Y))
        CalculerTuiles(SurfacePixels, SiteCarto, Echelle)
    End Sub
    ''' <summary> calcule les tuiles qui délimitent la surface à partir d'un rectangle dont les unités sont exprimées en mètres </summary>
    ''' <param name="SurfaceGrille"> rectangle exprimé en mètres </param>
    ''' <param name="SiteCarto"> site catographiques </param>
    ''' <param name="Echelle"> indice de l'échelle </param>
    Private Sub CalculerTuiles(SurfaceGrille As RectangleD, SiteCarto As SitesCartographiques, Echelle As Echelles)
        _Site = SiteCarto
        _Echelle = Echelle
        Tuile0 = New TuileServeurCarto(SurfaceGrille.Pt0, SiteCarto, Echelle, False)
        Tuile2 = New TuileServeurCarto(SurfaceGrille.Pt2, SiteCarto, Echelle, True)
        FlagNotEmpty = True
        CalculerSurface()
    End Sub
    ''' <summary> calcule les tuiles qui délimitent la surface à partir d'un rectangle dont les unités sont exprimées en pixels </summary>
    ''' <param name="SurfacePixels"> rectangle exprimé en pixels </param>
    ''' <param name="SiteCarto"> site catographiques </param>
    ''' <param name="Echelle"> indice de l'échelle </param>
    Private Sub CalculerTuiles(SurfacePixels As Rectangle, SiteCarto As SitesCartographiques, Echelle As Echelles)
        _Site = SiteCarto
        _Echelle = Echelle
        Tuile0 = New TuileServeurCarto(SurfacePixels.Location, SiteCarto, Echelle, False)
        Tuile2 = New TuileServeurCarto(New Point(SurfacePixels.Right, SurfacePixels.Bottom), SiteCarto, Echelle, True)
        FlagNotEmpty = True
        CalculerSurface()
    End Sub
    ''' <summary> calcul les surfaces en pixels et en mètre de la surface représentées par l'ensemble des tuiles </summary>
    Private Sub CalculerSurface()
        _RectangleGrille = New RectangleD(Tuile0.ToGrille(False), Tuile2.ToGrille(True))
        Dim Pt0 As Point = Tuile0.ToPixels(False)
        Dim Pt2 As Point = Tuile2.ToPixels(True)
        _RectanglePixels = New Rectangle(Tuile0.ToPixels(False), New Size(Pt2.X - Pt0.X, Pt2.Y - Pt0.Y))
    End Sub
    ''' <summary> Site associé à la surface </summary>
    Friend ReadOnly Property Site As SitesCartographiques
        Get
            If Not FlagNotEmpty Then
                Return SitesCartographiques.Aucun
            Else
                Return _Site
            End If
        End Get
    End Property
    ''' <summary> Indice de l'échelle associée à la surface </summary>
    Friend ReadOnly Property Echelle As Echelles
        Get
            If Not FlagNotEmpty Then
                Return Echelles._000
            Else
                Return _Echelle
            End If
        End Get
    End Property
    ''' <summary> True si la structure a été initialisée avec le constructeur </summary>
    Friend ReadOnly Property IsEmpty() As Boolean
        Get
            Return Not FlagNotEmpty
        End Get
    End Property
    ''' <summary> Nb de tuiles de la surface </summary>
    Friend ReadOnly Property NbTuiles As Integer
        Get
            If Not FlagNotEmpty Then
                Return 0
            Else
                Return (Tuile2.NumCol - Tuile0.NumCol + 1) * (Tuile2.NumRang - Tuile0.NumRang + 1)
            End If
        End Get
    End Property
    ''' <summary> Nb de colonnes de la surface (Largeur) </summary>
    Friend ReadOnly Property NbColonnes As Integer
        Get
            If Not FlagNotEmpty Then
                Return 0
            Else
                Return Tuile2.NumCol - Tuile0.NumCol + 1
            End If
        End Get
    End Property
    ''' <summary> Nb de rangées de la surface (Hauteur) </summary>
    Friend ReadOnly Property NbRangees As Integer
        Get
            If Not FlagNotEmpty Then
                Return 0
            Else
                Return Tuile2.NumRang - Tuile0.NumRang + 1
            End If
        End Get
    End Property
    ''' <summary> numéro de colonne de départ (Axe des X) </summary>
    Friend ReadOnly Property NumColDeb As Integer
        Get
            Return Tuile0.NumCol
        End Get
    End Property
    ''' <summary> numéro de colonne de fin (Axe des X) </summary>
    Friend ReadOnly Property NumColFin As Integer
        Get
            Return Tuile2.NumCol
        End Get
    End Property
    ''' <summary> numéro de rangée de départ (Axe des Y) </summary>
    Friend ReadOnly Property NumRangDeb As Integer
        Get
            Return Tuile0.NumRang
        End Get
    End Property
    ''' <summary> numéro de rangée de fin (Axe des Y) </summary>
    Friend ReadOnly Property NumRangFin As Integer
        Get
            Return Tuile2.NumRang
        End Get
    End Property
    ''' <summary> numéro de Zoom des tuiles </summary>
    Friend ReadOnly Property Couche As Byte
        Get
            Return If(FlagNotEmpty, EchelleToCouche(_Echelle), CByte(0))
        End Get
    End Property
    ''' <summary> largeur et hauteur d'une tuile exprimée en mètres de la grille. Ce n'est pas forcément des vrais mètres </summary>
    Friend ReadOnly Property LargeurHauteurTuile As Double
        Get
            Return If(FlagNotEmpty, MetresTuile(_Echelle), 0.0R)
        End Get
    End Property
    ''' <summary> surface des tuiles exprimée en mètres de la projection du site </summary>
    Friend ReadOnly Property RectangleGrille() As RectangleD
    Friend ReadOnly Property PtGrille(NumCoin As Integer) As PointD
        Get
            Return _RectangleGrille.Pt(NumCoin)
        End Get
    End Property
    ''' <summary> Coordonnées du point Nord Ouest exprimé en mètres </summary>
    Friend ReadOnly Property PtGrille_NO() As PointD
        Get
            Return _RectangleGrille.Pt0
        End Get
    End Property
    ''' <summary> Coordonnées du point Sud Est exprimé en mètres </summary>
    Friend ReadOnly Property PtGrille_SE() As PointD
        Get
            Return _RectangleGrille.Pt2
        End Get
    End Property
    ''' <summary> Hauteur de la surface exprimée en mètres </summary>
    Friend ReadOnly Property HauteurGrille As Double
        Get
            Return -_RectangleGrille.Hauteur
        End Get
    End Property
    ''' <summary> Largeur de la surface exprimée en mètres </summary>
    Friend ReadOnly Property LargeurGrille As Double
        Get
            Return _RectangleGrille.Largeur
        End Get
    End Property
    ''' <summary> surface de la surface des tuiles exprimée en m2 </summary>
    Friend ReadOnly Property SurfaceGrille As Double
        Get
            Return -_RectangleGrille.Largeur * _RectangleGrille.Hauteur
        End Get
    End Property
    ''' <summary> surface des tuiles exprimée en pixels de la projection du site </summary>
    Friend ReadOnly Property RectanglePixels() As Rectangle
    ''' <summary> Coordonnées du point Nord Ouest exprimé en pixels </summary>
    Friend ReadOnly Property PtPixels_NO() As Point
        Get
            Return _RectanglePixels.Location
        End Get
    End Property
    ''' <summary> Coordonnées du point Sud Est exprimé en pixels </summary>
    Friend ReadOnly Property PtPixels_SE() As Point
        Get
            Return Point.Add(_RectanglePixels.Location, _RectanglePixels.Size)
        End Get
    End Property
    Friend ReadOnly Property PtPixels(NumCoin As Integer) As Point
        Get
            Select Case NumCoin
                Case 0
                    Return New Point(_RectanglePixels.Left, _RectanglePixels.Top)
                Case 1
                    Return New Point(_RectanglePixels.Right, _RectanglePixels.Top)
                Case 2
                    Return New Point(_RectanglePixels.Right, _RectanglePixels.Bottom)
                Case 3
                    Return New Point(_RectanglePixels.Left, _RectanglePixels.Bottom)
            End Select
            Return Point.Add(_RectanglePixels.Location, _RectanglePixels.Size)
        End Get
    End Property
    ''' <summary> Largeur de la surface exprimée en pixels </summary>
    Friend ReadOnly Property LargeurPixels As Integer
        Get
            Return _RectanglePixels.Width
        End Get
    End Property
    ''' <summary> Hauteur de la surface exprimée en pixels </summary>
    Friend ReadOnly Property HauteurPixels As Integer
        Get
            Return _RectanglePixels.Height
        End Get
    End Property
    ''' <summary> chaine représentant la surfaceTuiles </summary>
    Public Overrides Function ToString() As String
        Return $"Col : {NumColDeb}, Row : {NumRangDeb}, NbCol : {NbColonnes}, NbRow : {NbRangees}, Nb Tuiles : {NbTuiles}"
    End Function

    ''' <summary> décrit les données associées à une tuile qui contient un des 2 points définissant l'emprise d'une carte </summary>
    Private Class TuileServeurCarto
        Private ReadOnly Indices As Point, FlagNotEmpty As Boolean, _Echelle As Echelles, _Site As SitesCartographiques
        ''' <summary> site web en relation avec la tuile </summary>
        Friend ReadOnly Property Site As SitesCartographiques
            Get
                If Not FlagNotEmpty Then
                    Return SitesCartographiques.Aucun
                Else
                    Return _Site
                End If
            End Get
        End Property
        ''' <summary> indice echelle (echelle ou zoom) du site web en relation avec la tuile </summary>
        Friend ReadOnly Property Echelle As Echelles
            Get
                If Not FlagNotEmpty Then
                    Return Echelles._000
                Else
                    Return _Echelle
                End If
            End Get
        End Property
        ''' <summary> True si la structure a été initialisée avec le constructeur </summary>
        Friend ReadOnly Property IsEmpty() As Boolean
            Get
                Return Not FlagNotEmpty
            End Get
        End Property
        ''' <summary> Constructeur pour avoir une tuile vide </summary>
        Friend Sub New()
        End Sub
        ''' <summary> creation de la tuile qui contient le pointGrille </summary>
        ''' <param name="CoordonneesGrille"> point exprimé en mètre de la projection du serveur </param>
        ''' <param name="Site"> site du serveur (indice dans le tableau des serveur connus) </param>
        ''' <param name="Echelle"> indice de l'échelle demandée (donne le zoom du serveur) </param>
        ''' <param name="IsFinTuile"> on se place à la fin de tuile pour le calcul des coordonnées </param>
        Friend Sub New(CoordonneesGrille As PointD, Site As SitesCartographiques, Echelle As Echelles, IsFinTuile As Boolean)
            _Site = Site
            _Echelle = Echelle
            Indices = PointGrilleToTuile(CoordonneesGrille, Site, Echelle, IsFinTuile)
            FlagNotEmpty = True
        End Sub
        ''' <summary> creation de la tuile qui contient le pointPixel </summary>
        ''' <param name="CoordonneesPixel"> point exprimé en pixels du serveur </param>
        ''' <param name="Site"> site du serveur (indice dans le tableau des serveur connus) </param>
        ''' <param name="Echelle"> échelle demandée (donne le zoom du serveur) </param>
        ''' <param name="IsFinTuile"> on se place à la fin de tuile pour le calcul des coordonnées </param>
        Friend Sub New(CoordonneesPixel As Point, Site As SitesCartographiques, Echelle As Echelles, IsFinTuile As Boolean)
            _Site = Site
            _Echelle = Echelle
            Indices = PointPixelToTuile(CoordonneesPixel, IsFinTuile)
            FlagNotEmpty = True
        End Sub
        ''' <summary> transforme les indices d'une tuile en point exprimé en mètre de la projection du serveur </summary>
        ''' <param name="IsFinTuile"> si False on prend le haut et gauche sinon le bas et droit de la tuile </param>
        Friend ReadOnly Property ToGrille(IsFinTuile As Boolean) As PointD
            Get
                Return TuileToPointGrille(Indices, _Site, _Echelle, IsFinTuile)
            End Get
        End Property
        ''' <summary> transforme les indices d'une tuile en point exprimé en pixels de la projection du serveur</summary>
        ''' <param name="IsFinTuile"> si False on prend le haut et gauche sinon le bas et droit de la tuile </param>
        Friend ReadOnly Property ToPixels(IsFinTuile As Boolean) As Point
            Get
                Return TuileToPointPixel(Indices, IsFinTuile)
            End Get
        End Property
        ''' <summary> numéro de colonne de la tuile </summary>
        Friend ReadOnly Property NumCol As Integer
            Get
                Return Indices.X
            End Get
        End Property
        ''' <summary> numéro de rangée de la tuile </summary>
        Friend ReadOnly Property NumRang As Integer
            Get
                Return Indices.Y
            End Get
        End Property
    End Class
End Class