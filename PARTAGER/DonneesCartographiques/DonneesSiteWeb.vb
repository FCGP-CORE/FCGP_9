Imports FCGP.Enumerations
''' <summary> données cartographiques associées aux sites Web</summary>
Module DonneesSiteWeb
#Region "Données du TileMatrixSet des sites web (WebMercator et SuisseMobile)"
    ''' <summary> largeur et hauteur en pixel des tuiles des serveurs carto. </summary>
    Friend Const NbPixelsTuile As Integer = 256
    ''' <summary> Renvoie la constante Metres pour l'échelle de capture </summary>
    ''' <param name="Echelle"> Indice de l'échelle de capture </param>
    Friend Function MetresTuile(Echelle As Echelles) As Double
        Return DonneesCouches(Echelle).PixelsMetre * NbPixelsTuile
    End Function
    ''' <summary> Renvoie la constante Pixels par Metres pour l'échelle de capture </summary>
    ''' <param name="Echelle"> Indice de l'échelle de capture </param>
    Friend Function PixelsMetre(Echelle As Echelles) As Double
        Return DonneesCouches(Echelle).PixelsMetre
    End Function
    ''' <summary> Renvoie la constante Metres par Pixels pour l'échelle de capture </summary>
    ''' <param name="Echelle"> Indice de l'échelle de capture </param>
    Friend Function MetrePixels(Echelle As Echelles) As Double
        Return DonneesCouches(Echelle).MetrePixels
    End Function
    ''' <summary> Renvoie la constante N° couche du serveur web en fonction de l'indice de l'échelle de capture </summary>
    ''' <param name="Echelle"> Indice de l'échelle de capture </param>
    Friend Function EchelleToCouche(Echelle As Echelles) As Byte
        Return DonneesCouches(Echelle).Couche
    End Function
    ''' <summary> données relatives à un niveau du tile matrixset d'un serveur carto (WebMercator et SuisseMobile) 
    ''' sous forme de constantes pour éviter les calculs </summary>
    Private ReadOnly DonneesCouches As (MetrePixels As Double, PixelsMetre As Double, Couche As Byte)() =
        New(Double, Double, Byte)() {
            (DC / 2.0R ^ 24.0R, 2.0R ^ 24.0R / DC, 17), ' 1/4 000 WebMercator
            (DC / 2.0R ^ 23.0R, 2.0R ^ 23.0R / DC, 16), ' 1/7 500 WebMercator
            (1.0R, 1.0R, 25),                           ' 1/5 000 Grille Suisse
            (2.5R, 1.0R / 2.5R, 22),                    ' 1/10 000 Grille Suisse
            (DC / 2.0R ^ 22.0R, 2.0R ^ 22.0R / DC, 15), ' 1/15 000 WebMercator
            (5.0R, 1.0R / 5.0R, 21),                    ' 1/25 000 Grille Suisse
            (DC / 2.0R ^ 21.0R, 2.0R ^ 21.0R / DC, 14), ' 1/30 000 WebMercator
            (10.0R, 1.0R / 10.0R, 20),                  ' 1/50 000 Grille Suisse
            (DC / 2.0R ^ 20.0R, 2.0R ^ 20.0R / DC, 13), ' 1/60 000 WebMercator
            (20.0R, 1.0R / 20.0R, 19),                  ' 1/100 000 Grille Suisse
            (DC / 2.0R ^ 19.0R, 2.0R ^ 19.0R / DC, 12), ' 1/120 000 WebMercator
            (50.0R, 1.0R / 50.0R, 18),                  ' 1/200 000 Grille Suisse
            (DC / 2.0R ^ 18.0R, 2.0R ^ 18.0R / DC, 11), ' 1/250 000 WebMercator
            (100.0R, 1.0R / 100.0R, 17),                ' 1/400 000 Grille Suisse
            (DC / 2.0R ^ 17.0R, 2.0R ^ 17.0R / DC, 10), ' 1/500 000 WebMercator
            (250.0R, 1.0R / 250.0R, 16),                ' 1/600 000 Grille Suisse
            (500.0R, 1.0R / 500.0R, 15),                ' 1/800 000 Grille Suisse
            (DC / 2.0R ^ 16.0R, 2.0R ^ 16.0R / DC, 9),  ' 1/1 000 000 WebMercator
            (DC / 2.0R ^ 15.0R, 2.0R ^ 15.0R / DC, 8),  ' 1/2 000 000 WebMercator
            (DC / 2.0R ^ 14.0R, 2.0R ^ 14.0R / DC, 7),  ' 1/4 000 000 WebMercator
            (DC / 2.0R ^ 13.0R, 2.0R ^ 13.0R / DC, 6)}  ' 1/8 000 000 WebMercator
#End Region
#Region "Données cartographiques Sites Web"
    ''' <summary> constantes des points d'affichage par défaut des sites de capture. </summary>
    Friend ReadOnly Property CentreSiteWeb(Indice_Site As Integer) As PointG
        Get
            Return DonneesSitesDomsToms(Indice_Site).Centre
        End Get
    End Property
    ''' <summary> constantes des origines X et Y des sites de capture. Origine à rajouter au coordonnées pour avoir les coordonnées de la Grille </summary>
    Friend ReadOnly Property OrigineSiteWeb(IndiceSite As Integer) As PointD
        Get
            Return DonneesSitesDomsToms(IndiceSite).Origine
        End Get
    End Property
    ''' <summary> constantes des Datums des sites de capture. </summary>
    Friend ReadOnly Property DatumSiteWeb(IndiceSite As Integer) As Datums
        Get
            Return DonneesSitesDomsToms(IndiceSite).Datum
        End Get
    End Property
    ''' <summary> constantes des points d'affichage par défaut des sites de capture. </summary>
    Friend ReadOnly Property LimiteSiteWeb(IndiceSite As Integer) As RectangleD
        Get
            Return DonneesSitesDomsToms(IndiceSite).Limites
        End Get
    End Property
    ''' <summary> indique si la portée du site carto peut être transposée à un site de portée Européenne </summary>
    ''' <param name="SiteCarto"></param>
    Friend ReadOnly Property IsNationalToEurope(SiteCarto As SitesCartographiques) As Boolean
        Get
            Return Array.IndexOf(_SitesNationalEuropeen, SiteCarto) > -1
        End Get
    End Property
    ''' <summary> retourne les sites carto nationaux à portée européenne </summary>
    Friend ReadOnly Property SitesNationalEuropeen() As SitesCartographiques()
        Get
            Return _SitesNationalEuropeen
        End Get
    End Property
    ''' <summary> retourne les sites carto à portée européenne </summary>
    Friend ReadOnly Property SitesOSMEurope As SitesCartographiques()
        Get
            Return _SitesOsmEurope
        End Get
    End Property
    ''' <summary> tableau des sites qui permette d'appeler un site OSM d'emprise Européene </summary>
    Private ReadOnly _SitesNationalEuropeen() As SitesCartographiques = New SitesCartographiques() {SitesCartographiques.Géofoncier,
                                                                                                    SitesCartographiques.SuisseMobile,
                                                                                                    SitesCartographiques.IgnEspagnol}
    ''' <summary> tableau des sites OSM d'emprise Européene </summary>
    Private ReadOnly _SitesOsmEurope() As SitesCartographiques = New SitesCartographiques() {SitesCartographiques.CyclOSM,
                                                                                             SitesCartographiques.OpenTopoMap}

    ''' <summary> données relatives aux sites et DomToms </summary>
    Private ReadOnly DonneesSitesDomsToms As (Limites As RectangleD, Centre As PointG, Origine As PointD, Datum As Datums)() =
        New(Limites As RectangleD, Centre As PointG, Origine As PointD, Datum As Datums)() {
                (New RectangleD(-590000.0R, 6674500.0R, 1076000.0R, 5054000.0R), New PointG(9I, 33176I, 23037I), New PointD(-DC, DC), Datums.Web_Mercator),             'GF
                (New RectangleD(475000.0R, 305000.0R, 855000.0R, 70000.0R), New PointG(6I, 1800I, 1500I), New PointD(420000.0R, 350000.0R), Datums.Grille_Suisse_LV03), 'SM
                (New RectangleD(6144800.0R, -2374000.0R, 6217200.0R, -2439700.0R), New PointG(5I, 685855I, 587209I), New PointD(-DC, DC), Datums.Web_Mercator),         'DT-REU-0
                (New RectangleD(-6818300.0R, 1676500.0R, -6768200.0R, 1617800.0R), New PointG(6I, 173258I, 240588I), New PointD(-DC, DC), Datums.Web_Mercator),         'DT-MAR-1
                (New RectangleD(-6883300.0R, 1865000.0R, -6790500.0R, 1783900.0R), New PointG(6I, 172683I, 238266I), New PointD(-DC, DC), Datums.Web_Mercator),         'DT-GUA-2
                (New RectangleD(-7031000.0R, 2053700.0R, -7009000.0R, 2037000.0R), New PointG(4I, 681071I, 941503I), New PointD(-DC, DC), Datums.Web_Mercator),         'DT-STM-3
                (New RectangleD(-6079200.0R, 647900.0R, -5745200.0R, 234900.0R), New PointG(7I, 92290I, 128021I), New PointD(-DC, DC), Datums.Web_Mercator),            'DT-GUY-4
                (New RectangleD(5002700.0R, -1411800.0R, 5043000.0R, -1468900.0R), New PointG(4I, 1311529I, 1123820I), New PointD(-DC, DC), Datums.Web_Mercator),       'DT-MAY-5
                (New RectangleD(-7008500.0R, 2036000.0R, -6988500.0R, 2022000.0R), New PointG(3I, 1364990I, 1885027I), New PointD(-DC, DC), Datums.Web_Mercator),       'DT-STB-6
                (New RectangleD(-1200000.0R, 11500000.0R, 3550000.0R, 4120000.0R), New PointG(10I, 17133I, 11258I), New PointD(-DC, DC), Datums.Web_Mercator),          'CY
                (New RectangleD(-1200000.0R, 11500000.0R, 3550000.0R, 4120000.0R), New PointG(10I, 17133I, 11258I), New PointD(-DC, DC), Datums.Web_Mercator),          'OT
                (New RectangleD(-2041000.0R, 5440000.0R, 501500.0R, 3195000.0R), New PointG(9I, 32110I, 24708I), New PointD(-DC, DC), Datums.Web_Mercator),             'ES
                (New RectangleD(2475000.0R, 1305000.0R, 2855000.0R, 1070000.0R), New PointG(6I, 1800I, 1500I), New PointD(2420000.0R, 1350000.0R), Datums.Grille_Suisse_LV95)} 'S5
#End Region
End Module