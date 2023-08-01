''' <summary> contient toutes les fonctions et procédures nécessaires pour passer d'un système de projection de coordonnées à un autre parmi ceux listés dans les datums</summary>
Friend Module ConvertirCoordonnees
#Region "Constantes"
    Friend Const PI As Double = Math.PI
    Friend Const Pi_2 As Double = PI / 2.0R
    Friend Const Pi_4 As Double = PI / 4.0R
    Friend Const TwoPI As Double = 2.0R * PI
    Friend Const Rad_Deg As Double = 180.0R / PI
    Friend Const Deg_Rad As Double = PI / 180.0R
    Friend Const Deg_Rad_2 = Deg_Rad / 2.0R
    Friend Const RayonWGS84 As Double = 6378137.0R 'a pour l'ellipsoide WGS84
    Friend Const F_WGS84 As Double = 1.0R / 298.257223563R '1/aplatissement pour l'ellipsoide WGS84
    Friend Const E2_WGS84 As Double = 2.0R * F_WGS84 - F_WGS84 * F_WGS84 'e2 pour l'ellipsoide WGS84
    Friend Const DC As Double = PI * RayonWGS84       'constante de base pour la projection WebMercator
#End Region
#Region "fonctions généralistes de datum à datum"
    '''<summary> convertit une coordonnée de départ exprimée en X , Y ou en LL en coordonnées d'arrivée en LL WGS84 exprimée en DD </summary>
    '''<param name="DatumDepart"> datum du point de coordonnées à convertir </param>
    '''<param name="PointDepart"> point à convertir </param>
    Friend Function ConvertProjectionToWGS84(DatumDepart As Datums, PointDepart As PointProjection) As PointD
        Try
            Dim LatLon As New PointD(0, 0) 'afin d'avoir le flag empty à faux
            Select Case ParametresDatums(DatumDepart).Projection
                Case Projections.Lambert
                    LatLon = ConvertLambertToLL(PointDepart.Coordonnees, DatumDepart, False)
                Case Projections.UTM
                    If PointDepart.Zone = 0 OrElse PointDepart.Hemisphere = Nothing Then
                        Throw New Exception("Paramètres projection UTM obligatoire")
                    End If
                    LatLon = ConvertUtmToLL(DatumDepart, PointDepart, False)
                Case Projections.Suisse
                    If DatumDepart = Datums.Grille_Suisse_LV95 Then
                        LV95ToLV03(PointDepart.Coordonnees)
                    End If
                    LatLon = ConvertLV03ToWGS84(PointDepart.Coordonnees, False)
                Case Projections.WMercator
                    LatLon = ConvertWebMercatorToWGS84(PointDepart.Coordonnees, False)
                Case Projections.LatLon
                    LatLon = ConvertLatLonToWGS84(PointDepart.Coordonnees, False)
                Case Projections.Datum
                    LatLon = PointD.Scale(PointDepart.Coordonnees, Deg_Rad)
            End Select
            If Not LatLon.IsEmpty Then 'pas d'erreur
                If DatumDepart < Datums.Lambert_93 OrElse DatumDepart > Datums.WGS84 Then
                    LatLon = ConvertDatumToDatum(DatumDepart, LatLon, Datums.WGS84, False)
                End If
                LatLon.Scale(Rad_Deg)
                Return LatLon
            Else
                Return PointD.Empty
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "3B18")
            Return PointD.Empty
        End Try
    End Function
    ''' <summary> convertit une coordonnée de départ en DD WGS84 en coordonnées X, Y exprimée en X , Y d'une projection Grille ou en DD </summary>
    ''' <param name="PointDepart"> point LatLon WGS84 à convertir </param>
    ''' <param name="DatumArrivee">Latitude de départ (input d.d)</param>
    Friend Function ConvertWGS84ToProjection(PointDepart As PointD, DatumArrivee As Datums) As PointProjection
        Try
            PointDepart.Scale(Deg_Rad)
            If DatumArrivee < Datums.Lambert_93 OrElse DatumArrivee > Datums.WGS84 Then
                'on doit basculer les coordonnées radians de wgs84 vers des coordonnées radians de l'éllipsoïde du datum d'arrivée
                PointDepart = ConvertDatumToDatum(Datums.WGS84, PointDepart, DatumArrivee, False)
            End If
            Dim PointArrivee As PointProjection
            Select Case ParametresDatums(DatumArrivee).Projection
                Case Projections.Lambert
                    PointArrivee.Coordonnees = ConvertLLToLambert(PointDepart, DatumArrivee, False)
                Case Projections.UTM
                    PointArrivee = ConvertLLToUtm(PointDepart, DatumArrivee,, False)
                Case Projections.Suisse
                    PointArrivee.Coordonnees = ConvertWGS84ToLV03(PointDepart, False)
                    If DatumArrivee = Datums.Grille_Suisse_LV95 Then
                        LV03ToLV95(PointArrivee.Coordonnees)
                    End If
                Case Projections.WMercator
                    PointArrivee.Coordonnees = ConvertWGS84ToWebMercator(PointDepart, False)
                Case Projections.LatLon
                    PointArrivee.Coordonnees = ConvertWGS84ToLatLon(PointDepart, False)
                Case Projections.Datum
                    PointArrivee.Coordonnees = PointD.Scale(PointDepart, Rad_Deg)
            End Select
            Return PointArrivee
        Catch Ex As Exception
            AfficherErreur(Ex, "3A2D")
            Return New PointProjection
        End Try
    End Function
    ''' <summary> Converti une projection ou un datum dans une autre projection ou datum. Procédure généraliste </summary>
    ''' <param name="DatumDepart"> projection ou datum au départ </param>
    ''' <param name="PointDepart"> point à convertir </param>
    ''' <param name="DatumArrivee"> projection ou datum à l'arrivée </param>
    Friend Function ConvertProjectionToProjection(DatumDepart As Datums, PointDepart As PointProjection, DatumArrivee As Datums) As PointProjection
        Dim PointLatLon As PointD, PointArrivee As PointProjection
        Try
            'tranforme le point de départ en coordoonées LatLon ramenées sur l'éllipsoïde associé au datum et exprimée en radians
            Select Case ParametresDatums(DatumDepart).Projection
                Case Projections.Lambert
                    PointLatLon = ConvertLambertToLL(PointDepart.Coordonnees, DatumDepart, False)
                Case Projections.UTM
                    If PointDepart.Zone = 0 OrElse PointDepart.Hemisphere = Nothing Then
                        Throw New Exception("Paramètres projection UTM obligatoire")
                    End If
                    PointLatLon = ConvertUtmToLL(DatumDepart, PointDepart, False)
                Case Projections.Suisse
                    PointLatLon = ConvertSuisseToCH1903(PointDepart.Coordonnees, DatumDepart, False)
                Case Projections.WMercator
                    PointLatLon = ConvertWebMercatorToWGS84(PointDepart.Coordonnees, False)
                Case Projections.LatLon
                    PointLatLon = ConvertLatLonToWGS84(PointDepart.Coordonnees, False)
                Case Projections.Datum
                    PointLatLon = PointD.Scale(PointDepart.Coordonnees, Deg_Rad)
            End Select
            'si il y a eu une erreur on sort
            If PointLatLon.IsEmpty Then Exit Try
            'transforme les coordonnées LatLon de l'ellispsoïde de départ en coordonnées LatLon sur l'Ellipsoïde d'arrivée exprimée en radians
            If ParametresDatums(DatumDepart).Ellipsoide <> ParametresDatums(DatumArrivee).Ellipsoide Then
                PointLatLon = ConvertDatumToDatum(DatumDepart, PointLatLon, DatumArrivee, False)
            End If
            'Tranforme les coordonnées latlon sur l'Ellipsoïde d'arrivée en coordonnées du point d'arrivée
            Select Case ParametresDatums(DatumArrivee).Projection
                Case Projections.Lambert
                    PointArrivee.Coordonnees = ConvertLLToLambert(PointLatLon, DatumArrivee, False)
                Case Projections.UTM
                    PointArrivee = ConvertLLToUtm(PointLatLon, DatumArrivee, , False)
                Case Projections.Suisse
                    PointArrivee.Coordonnees = ConvertCH1903ToSuisse(PointLatLon, DatumArrivee, False)
                Case Projections.WMercator
                    PointArrivee.Coordonnees = ConvertWGS84ToWebMercator(PointLatLon, False)
                Case Projections.LatLon
                    PointArrivee.Coordonnees = ConvertWGS84ToLatLon(PointLatLon, False)
                Case Projections.Datum
                    PointArrivee.Coordonnees = PointD.Scale(PointLatLon, Rad_Deg)
            End Select
        Catch Ex As Exception
            AfficherErreur(Ex, "3D0E")
            PointArrivee = New PointProjection
        End Try
        Return PointArrivee
    End Function
    ''' <summary> convertir des degrés décimaux en radians </summary>
    ''' <param name="Valeur">valeur à convertir (input d.d)</param>
    ''' <returns>valeur convertie rad</returns>
    Friend Function DegToRad(Valeur As Double) As Double
        Return Valeur * Deg_Rad
    End Function
    ''' <summary> convertir des radians en degré décimaux </summary>
    ''' <param name="Valeur">valeur à convertir (input rad)</param>
    ''' <returns>valeur convertie d.d</returns>
    Friend Function RadToDeg(Valeur As Double) As Double
        Return Valeur * Rad_Deg
    End Function
#End Region
#Region "Fonctions spécialisées"
#Region "Tuile <--> Pixels et Tuile <--> Grille"
#Region "Bas niveau"
    ''' <summary> transforme des pixels en indice de tuile </summary>
    ''' <param name="Pixel"> Nombre de pixels (index base 0) </param>
    ''' <param name="IsFinTuile"> les pixels sont considérés comme en début ou en fin de tuile. Uniquement quand le reste = 0</param>
    Friend Function PixelToNumTile(Pixel As Integer, IsFinTuile As Boolean) As Integer
        'on est obligé d'utiliser Math.Floor ou int car l'opérateur \ ne fonctionne pas pour les nombres négatifs. de -1 à -256 doit renvoyer -1
        PixelToNumTile = CInt(Math.Floor(Pixel / NbPixelsTuile))
        If IsFinTuile AndAlso Pixel Mod NbPixelsTuile = 0 Then PixelToNumTile -= 1
    End Function
    ''' <summary> transforme un indice de tuile en pixels </summary>
    ''' <param name="Num"> indice tuile </param>
    ''' <param name="IsFinTuile"> les pixels retournés sont ceux du début ou de fin de tuile. </param>
    Private Function NumTileToPixel(Num As Integer, IsFinTuile As Boolean) As Integer
        If IsFinTuile Then
            NumTileToPixel = (Num + 1) * NbPixelsTuile
        Else
            NumTileToPixel = Num * NbPixelsTuile
        End If
    End Function
#End Region
    ''' <summary> transforme un point exprimé en indices tuile en un point exprimé en grille de la projection du site de capture </summary>
    ''' <param name="Tuile"> point exprimé en coordonnées tuile à transformer </param>
    ''' <param name="SiteCarto"> site de capture </param>
    ''' <param name="Echelle"> indice de l'échelle de capture </param>
    ''' <param name="IsFinTuile"> prendre en compte le point Pt2 de la tuile à la place du point Pt0 </param>
    Friend Function TuileToPointGrille(Tuile As Point, SiteCarto As SitesCartographiques, Echelle As Echelles, IsFinTuile As Boolean) As PointD
        Dim PointPixels As Point = TuileToPointPixel(Tuile, IsFinTuile)
        Return PointPixelToPointGrille(PointPixels, SiteCarto, Echelle)
    End Function
    ''' <summary> transforme un point exprimé en grille de la projection du site de capture en un point exprimé en indices tuile </summary>
    ''' <param name="PointGrille"></param>
    ''' <param name="SiteCarto"> site de capture </param>
    ''' <param name="Echelle"> indice de l'échelle de capture </param>
    ''' <param name="IsFinTuile"> prendre en compte la fin de la tuile </param>
    Friend Function PointGrilleToTuile(PointGrille As PointD, SiteCarto As SitesCartographiques, Echelle As Echelles, IsFinTuile As Boolean) As Point
        Dim PointPixels As Point = PointGrilleToPointPixel(PointGrille, SiteCarto, Echelle)
        Return PointPixelToTuile(PointPixels, IsFinTuile)
    End Function
    ''' <summary> transforme un point exprimé en indices tuile en un point exprimé en pixels de la projection du site de capture </summary>
    ''' <param name="Tuile"> point exprimé en coordonnées tuile à transformer </param>
    ''' <param name="IsFinTuile"> prendre en compte le point Pt2 de la tuile à la place du point Pt0 </param>
    Friend Function TuileToPointPixel(Tuile As Point, IsFinTuile As Boolean) As Point
        Return New Point(NumTileToPixel(Tuile.X, IsFinTuile), NumTileToPixel(Tuile.Y, IsFinTuile))
    End Function
    ''' <summary> transforme un point exprimé en pixels de la projection du site de capture en un point exprimé en indices tuile </summary>
    ''' <param name="PointPixel"> point exprimé en pixels de la projection à transformer</param>
    ''' <param name="IsFinTuile"> prendre en compte la fin de la tuile </param>
    Friend Function PointPixelToTuile(PointPixel As Point, IsFinTuile As Boolean) As Point
        Return New Point(PixelToNumTile(PointPixel.X, IsFinTuile), PixelToNumTile(PointPixel.Y, IsFinTuile))
    End Function
#End Region
#Region "Region DD --> DMS et DD <--> WebMercator"
    ''' <summary> tranforme une surface exprimée en DD en une chaine décrivant Pt0 et PT2 en DMS </summary>
    ''' <param name="RegionDD"> surface DD à transformer </param>
    ''' <param name="InclureSens"> les caractères N,S,E,O sont inclus </param>
    ''' <param name="Prec"> Nombre de décimale pour les secondes </param>
    ''' <param name="Separateur"> caractère de séparation entre Pt0 et Pt2 </param>
    Friend Function RegionDDToDMSText(RegionDD As RectangleD, Optional InclureSens As Boolean = True, Optional Prec As Integer = 1, Optional Separateur As String = ",") As String
        Dim Pt0DMS As String = ConvertPointDDtoDMS(RegionDD.Pt0, InclureSens, Prec, Separateur)
        Dim Pt2DMS As String = ConvertPointDDtoDMS(RegionDD.Pt2, InclureSens, Prec, Separateur)
        Return $"PtNO : {Pt0DMS}, PtSE : {Pt2DMS}"
    End Function
    ''' <summary> tranforme une surface exprimée en DD en une surface exprimée en grille PseudoMercator </summary>
    ''' <param name="RegionDD"> surface DD à transformer </param>
    Friend Function RegionDDToRegionWebMecator(RegionDD As RectangleD) As RectangleD
        Return New RectangleD(PointDDToPointGrille(RegionDD.Pt0), PointDDToPointGrille(RegionDD.Pt2))
    End Function
    ''' <summary> tranforme une surface exprimée en grille PseudoMercator en une surface exprimée en DD </summary>
    ''' <param name="RegionGrille"> surface grille PseudoMercator à transformer </param>
    Friend Function RegionWebMecatorToRegionDD(RegionGrille As RectangleD) As RectangleD
        Return New RectangleD(PointGrilleToPointDD(RegionGrille.Pt0), PointGrilleToPointDD(RegionGrille.Pt2))
    End Function
#End Region
#Region "Grille <--> DD"
    ''' <summary> tranforme une surface exprimée en grille (mètres) en une surface exprimée en DD </summary>
    ''' <param name="RegionGrille"> surface grille à transformer </param>
    ''' <param name="SiteCarto"> Site carto de la projection grille </param>
    Friend Function RegionGrilleToRegionDD(RegionGrille As RectangleD, SiteCarto As SitesCartographiques) As RectangleD
        Return New RectangleD(PointGrilleToPointDD(RegionGrille.Pt0, DatumSiteWeb(SiteCarto)),
                              PointGrilleToPointDD(RegionGrille.Pt2, DatumSiteWeb(SiteCarto)))
    End Function
    ''' <summary> tranforme une surface exprimée en DD en une surface exprimée en grille (mètres) </summary>
    ''' <param name="RegionDD"> surface DD à transformer </param>
    ''' <param name="SiteCarto"> Site carto de la projection grille </param> 
    Friend Function RegionDDToRegionGrille(RegionDD As RectangleD, SiteCarto As SitesCartographiques) As RectangleD
        Return New RectangleD(PointDDToPointGrille(RegionDD.Pt0, DatumSiteWeb(SiteCarto)),
                              PointDDToPointGrille(RegionDD.Pt2, DatumSiteWeb(SiteCarto)))
    End Function
    ''' <summary> tranforme une point exprimé en DD en un point exprimé en grille WebMercator ou en grille Suisse </summary>
    ''' <param name="PointDD"> point DD à transformer </param>
    Friend Function PointDDToPointGrille(PointDD As PointD, Optional Datum As Datums = Datums.Web_Mercator) As PointD
        'il n'y a que 2 grilles possibles avec les Datums gérés par FCGP
        If Datum = Datums.Web_Mercator Then
            PointDDToPointGrille = ConvertWGS84ToWebMercator(PointDD, True)
        Else
            PointDDToPointGrille = ConvertWGS84ToLV03(PointDD, True)
        End If
    End Function
    ''' <summary> tranforme un point exprimé en grille PseudoMercator ou en grille Suisse en un point exprimée en DD WGS84</summary>
    ''' <param name="PointGrille"> point grille PseudoMercator à transformer </param>
    Friend Function PointGrilleToPointDD(PointGrille As PointD, Optional Datum As Datums = Datums.Web_Mercator) As PointD
        'il n'y a que 2 grilles possibles avec les Datums gérés par FCGP
        If Datum = Datums.Web_Mercator Then
            PointGrilleToPointDD = ConvertWebMercatorToWGS84(PointGrille, True)
        Else
            PointGrilleToPointDD = ConvertLV03ToWGS84(PointGrille, True)
        End If
    End Function
#End Region
#Region "Pixels <--> DD"
    ''' <summary> transforme une surface exprimée en coordonnées virtuelles (Pixels) en surface exprimée en coordonnées DD </summary>
    ''' <param name="RegionPixels"> surface virtuelles </param>
    ''' <param name="SiteCarto"> site carto des coordonnées </param>
    ''' <param name="Echelle"> echelle des coordonnées virtuelles </param>
    Friend Function RegionPixelsToRegionDD(RegionPixels As Rectangle, SiteCarto As SitesCartographiques, Echelle As Echelles) As RectangleD
        Return New RectangleD(PointPixelsToPointDD(RegionPixels.Location, SiteCarto, Echelle),
                              PointPixelsToPointDD(Point.Add(RegionPixels.Location, RegionPixels.Size), SiteCarto, Echelle))
    End Function
    ''' <summary> transforme une surface exprimée en coordonnées DD en surface exprimée en coordonnées virtuelles (Pixels) </summary>
    ''' <param name="RegionDD"> Surface DD </param>
    ''' <param name="SiteCarto"> site carto des coordonnées </param>
    ''' <param name="Echelle"> echelle des coordonnées DD </param>
    Friend Function RegionDDToRegionPixels(RegionDD As RectangleD, SiteCarto As SitesCartographiques, Echelle As Echelles) As Rectangle
        Dim Pt0 As Point = PointDDToPointPixels(RegionDD.Pt0, SiteCarto, Echelle)
        Dim Pt2 As Point = PointDDToPointPixels(RegionDD.Pt2, SiteCarto, Echelle)
        Return New Rectangle(Pt0, New Size(Pt2.X - Pt0.X, Pt2.Y - Pt0.Y))
    End Function
    ''' <summary> transforme un point exprimé en coordonnées virtuelles (pixels d'image issue du web qui dépend de l'échelle de représentation) en coordonnées DD </summary>
    ''' <param name="PointPixels"> coordonnées virtuelles </param>
    ''' <param name="SiteCarto"> site carto des coordonnées </param>
    ''' <param name="Echelle"> echelle des coordonnées virtuelles </param>
    Friend Function PointPixelsToPointDD(PointPixels As Point, SiteCarto As SitesCartographiques, Echelle As Echelles) As PointD
        'transformation pixel to projection grille web à l'aide de l'origine et de la résolution par mètres de l'échelle
        Dim Grille As PointD = PointPixelToPointGrille(PointPixels, SiteCarto, Echelle)
        'transformation projection grille web to dd à partir du module de transformation de coordonnées
        Return PointGrilleToPointDD(Grille, DatumSiteWeb(SiteCarto))
    End Function
    ''' <summary> transforme un point exprimé en DD en coordonnées virtuelles(pixels d'image issue du web qui dépend de l'échelle de représentation) </summary>
    ''' <param name="PointCoordonnees"> coordonnées DD </param>
    ''' <param name="SiteCarto"> site carto des coordonnées </param>
    ''' <param name="Echelle"> echelle des coordonnées DD </param>
    Friend Function PointDDToPointPixels(PointCoordonnees As PointD, SiteCarto As SitesCartographiques, Echelle As Echelles) As Point
        'transformation dd to projection grille web à partir du module de transformation de coordonnées
        PointCoordonnees = PointDDToPointGrille(PointCoordonnees, DatumSiteWeb(SiteCarto))
        'transformation projection grille web to pixel à l'aide de l'origine et de la résolution par mètres de l'échelle
        Return PointGrilleToPointPixel(PointCoordonnees, SiteCarto, Echelle)
    End Function
    ''' <summary> transforme une latitude en pixel mais de type double de manière à avoir une partie fractionnaire indiquant le poids respectif de chaque ligne de pixel </summary>
    ''' <param name="Lat"> Latitude à transformer </param>
    ''' <param name="SiteCarto"> site carto des coordonnées </param>
    ''' <param name="Echelle"> echelle des coordonnées </param>
    ''' <remarks> Sert uniquement pour l'interpolation des cartes sans grille </remarks>
    Friend Function LatitudeWebMercatorToPixel(Lat As Double, SiteCarto As SitesCartographiques, Echelle As Echelles) As Double
        Dim GrilleY As Double = Math.Log(Math.Tan(Lat * Deg_Rad_2 + Pi_4)) * RayonWGS84
        LatitudeWebMercatorToPixel = (OrigineSiteWeb(SiteCarto).Y - GrilleY) * PixelsMetre(Echelle)
    End Function
#End Region
#Region "Pixels <--> Grille"
    ''' <summary> tranforme une surface exprimée en coordonnées virtuelles(pixels) en une surface exprimée en grille de la projection du site de capture </summary>
    ''' <param name="RegionPixels"> surface exprimée en pixels </param>
    ''' <param name="SiteCarto"> site de capture </param>
    ''' <param name="Echelle"> échelle de capture </param>
    Friend Function RegionPixelsToRegionGrille(RegionPixels As Rectangle, SiteCarto As SitesCartographiques, Echelle As Echelles) As RectangleD
        Return New RectangleD(PointPixelToPointGrille(RegionPixels.Location, SiteCarto, Echelle),
                              PointPixelToPointGrille(New Point(RegionPixels.Right, RegionPixels.Bottom), SiteCarto, Echelle))
    End Function
    ''' <summary> transforme un point de coordonnées virtuelles en un point de coordonnées exprimées en mètre de la grille du site carto </summary>
    ''' <param name="PointPixel"> point à convertir exprimé en coordonnées virtuelles (pixels image) </param>
    ''' <param name="SiteCarto"> site de capture </param>
    ''' <param name="Echelle"> échelle de capture </param>
    Friend Function PointPixelToPointGrille(PointPixel As Point, SiteCarto As SitesCartographiques, Echelle As Echelles) As PointD
        Dim PointGrille As New PointD(PointPixel)
        PointGrille.Scale(MetrePixels(Echelle))
        PointGrille.X += OrigineSiteWeb(SiteCarto).X
        PointGrille.Y = OrigineSiteWeb(SiteCarto).Y - PointGrille.Y
        Return PointGrille
    End Function
    ''' <summary> tranforme une surface exprimée en grille de la projection du site de capture en une surface exprimée en pixels </summary>
    ''' <param name="RegionGrille"> surface exprimée en grille PseudoMercator ou Projection Suisse à transformer </param>
    ''' <param name="SiteCarto"> site de capture </param>
    ''' <param name="Echelle"> échelle de capture </param>
    Friend Function RegionGrilleToRegionPixels(RegionGrille As RectangleD, SiteCarto As SitesCartographiques, Echelle As Echelles) As Rectangle
        Dim Pt0 As Point = PointGrilleToPointPixel(RegionGrille.Pt0, SiteCarto, Echelle)
        Dim Pt2 As Point = PointGrilleToPointPixel(RegionGrille.Pt2, SiteCarto, Echelle)
        Return New Rectangle(Pt0, New Size(Pt2.X - Pt0.X, Pt2.Y - Pt0.Y))
    End Function
    ''' <summary> transforme un point de coordonnées exprimée en mètre de la grille du site carto en un point de coordonnées virtuelles </summary>
    ''' <param name="PointGrille"> Point à convertir en grille PseudoMercator ou Projection Suisse </param>
    ''' <param name="SiteCarto"> site de capture </param>
    ''' <param name="Echelle"> échelle de capture </param>
    ''' <returns> point exprimé en coordonnées virtuelles (pixels image) </returns>
    Friend Function PointGrilleToPointPixel(PointGrille As PointD, SiteCarto As SitesCartographiques, Echelle As Echelles) As Point
        PointGrille.X -= OrigineSiteWeb(SiteCarto).X
        PointGrille.Y = OrigineSiteWeb(SiteCarto).Y - PointGrille.Y
        PointGrille.Scale(PixelsMetre(Echelle))
        Return PointGrille.ToPoint
    End Function
#End Region
#End Region
End Module