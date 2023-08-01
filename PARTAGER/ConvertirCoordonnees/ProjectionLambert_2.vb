Namespace Coordonnees
    ''' <summary> Ce module contient les fonctions permettant la conversion entre les coordonnées Géodétique (Latitude et Longitude) et une projection 
    ''' Lambert Conforme Conique à 2 parallèles en X et Y ou l'inverse  traduit du projet Geotrans écrit en C </summary>
    Friend Module ProjectionLambert_2
#Region "Haut Niveau"
        ''' <summary> Lambert en mètres vers ellispsoïde du datum en DD ou en Rad. </summary>
        ''' <param name="PointGrille"> point X-Y à convertir </param>
        ''' <param name="Datum"> datum de la projection Lambert </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point convertit sont exprimées en DD ou en Rad </param>
        Friend Function ConvertLambertToLL(PointGrille As PointD, Datum As Datums, Optional Deg As Boolean = True) As PointD
            'on initialise les paramètres de la projection si besoin
            If ProjectionLambert_2.Datum <> Datum Then
                If Set_LAMBCC2_Parameters(Datum) <> LAMBERT_2.NO_ERROR Then
                    Throw New Exception("Erreur dans la mise à jour des paramètres de la projection")
                End If
            End If
            Dim Ret As New PointD(0, 0)
            'transforme les x, y en longitude et latitude exprimée en radians
            If Convert_LAMBCC2_To_Geodetic(PointGrille, Ret) = LAMBERT_2.NO_ERROR Then
                If Deg Then Ret.Scale(Rad_Deg)
                Return Ret
            Else
                Return PointD.Empty
            End If
        End Function
        ''' <summary> Coordonnées lat lon sur ellispsoïde du datum en DD ou en Rad vers Lambert </summary>
        ''' <param name="PointLatLon"> point Lat Lon à convertir </param>
        ''' <param name="Datum"> Datum de la projection Lambert </param>
        ''' <param name="Deg"> indique si la coordonnées Lat Lon sont en DD </param>
        ''' <returns> Le point Lat lon en DD ou rad </returns>
        Friend Function ConvertLLToLambert(PointLatLon As PointD, Datum As Datums, Optional Deg As Boolean = True) As PointD
            'transforme les DD du datum de depart en X, Y, Z du datum de depart
            If ProjectionLambert_2.Datum <> Datum Then
                If Set_LAMBCC2_Parameters(Datum) <> LAMBERT_2.NO_ERROR Then
                    Throw New Exception("Erreur dans la mise à jour des paramètres de la projection")
                End If
            End If
            If Deg Then PointLatLon.Scale(Deg_Rad)
            'transforme les DD du datum UTM en X, Y UTM du datum UTM
            Dim Ret As New PointD(0, 0)
            If Convert_Geodetic_To_LAMBCC2(PointLatLon, Ret) = LAMBERT_2.NO_ERROR Then
                Return Ret
            Else
                Return PointD.Empty
            End If
        End Function
#End Region
#Region "Constantes des différentes projections lambert"
        ''' <summary> Libellés des différents projections lambert. Il y a un décalage de 2 pour prendre en compte l'enum Datums </summary>
        Private ReadOnly Centrals_Meridians() As Double = {0.0R, 0.0R, 2.33722917R, 2.33722917R, 2.33722917R, 2.33722917R, 2.33722917R, 3.0R}
        Private ReadOnly Origins_Latitudes() As Double = {0.0R, 0.0R, 49.5R, 46.8R, 46.8R, 44.1R, 42.165R, 46.5R}
        Private ReadOnly Std_Parallels_1() As Double = {0.0R, 0.0R, 48.5985228R, 45.89891889R, 45.89891889R, 41.5603878R, 41.5603878, 44.0R}
        Private ReadOnly Std_Parallels_2() As Double = {0.0R, 0.0R, 50.3959117R, 47.69601444R, 47.69601444R, 42.7676633R, 42.7676633, 49.0R}
        Private ReadOnly False_Eastings() As Double = {0.0R, 0.0R, 600000.0R, 600000.0R, 600000.0R, 600000.0R, 234.358R, 700000.0R}
        Private ReadOnly False_Northings() As Double = {0.0R, 0.0R, 200000.0R, 200000.0R, 2200000.0R, 200000.0R, 185861.369R, 6600000.0R}

        'Private ReadOnly Libelles() As String = {"", "", "Projection Lambert conique conforme I (Nord)", "Projection Lambert conique conforme II (Centre)",
        '                                         "Projection Lambert conique conforme II étendue (France)", "Projection Lambert conique conforme III (Sud)",
        '                                         "Projection Lambert conique conforme IV (Corse)", "Projection Lambert conique conforme 93 (France)"}
#End Region
#Region "Constantes et Variables"
        Private Const MAX_LAT As Double = Deg_Rad * 89.9997222222222R, Lambert_Delta_Easting As Double = 400_000_000R, Lambert_Delta_Northing As Double = 400_000_000R
        Private Datum As Datums = Datums.Aucun, Ellipsoide As Ellipsoides = Ellipsoides.Aucun
        Private Lambert_a, Lambert_f, es, es_OVER_2, Lambert_lat0, Lambert_k0, Lambert_false_northing, Lambert_Std_Parallel_1 As Double
        Private Lambert_Std_Parallel_2, Lambert_Origin_Lat, Lambert_Origin_Long, Lambert_False_Northing_M, Lambert_False_Easting As Double
        ''' <summary> Liste des différentes erreurs pouvant survenir lors de la transformation des coordonnées </summary>
        <Flags>
        Private Enum LAMBERT_2 As Integer
            '''<summary>Flag OK</summary>"
            NO_ERROR = 0
            '''<summary>Flag erreur concernant la Latitude</summary>
            LAT_ERROR = 1
            '''<summary>Flag erreur concernant la Longitude</summary>
            LON_ERROR = 2
            '''<summary>Flag erreur concernant les abscisses(Easting ou X)</summary>
            EASTING_ERROR = 4
            '''<summary>Flag erreur concernant les ordonnées(Northing ou Y)</summary>
            NORTHING_ERROR = 8
            '''<summary>Flag erreur concernant le premier parallèle standard</summary>
            FIRST_STDP_ERROR = 16
            '''<summary>Flag erreur concernant le second parallèle standard</summary>
            SECOND_STDP_ERROR = 32
            '''<summary>Flag erreur concernant l'origine de la Longitude</summary>
            ORIGIN_LAT_ERROR = 64
            '''<summary>Flag erreur concernant le méridien central</summary>
            CENT_MER_ERROR = 128
            '''<summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 256
            '''<summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 512
            '''<summary>Flag erreur concernant l'hémisphère</summary>
            HEMISPHERE_ERROR = 1024
            '<summary>Flag erreur concernant le premier et second parallèle standards qui sont à zéro</summary>
            FIRST_SECOND_ERROR = 2048
            '''<summary>Flag erreur concernant le facteur d'échelle</summary>
            SCALE_FACTOR_ERROR = 4096
        End Enum
#End Region
#Region "Functions and Procedures"
        Private Function LAMBERT_m(clat As Double, essin As Double) As Double
            Return (clat / Math.Sqrt(1.0R - essin * essin))
        End Function
        Private Function LAMBERT_t(lat As Double, essin As Double) As Double
            Return Math.Tan(Pi_4 - lat / 2.0R) * Math.Pow((1 + essin) / (1.0R - essin), es_OVER_2)
        End Function
        Private Function ES_SIN_Func(sinlat As Double) As Double
            Return (es * sinlat)
        End Function
        '''<summary>The function Set_Lambert_2_Parameters receives the ellipsoid parameters and Lambert Conformal Conic (2 parallel)
        ''' projection parameters as inputs, and sets the corresponding state variables.</summary>
        Private Function Set_LAMBCC2_Parameters(DatumLambert As Datums) As LAMBERT_2
            Datum = DatumLambert
            Ellipsoide = ParametresDatums(Datum).Ellipsoide
            Dim a As Double = ParametresEllipsoides(Ellipsoide).A
            Dim F As Double = ParametresEllipsoides(Ellipsoide).F
            Dim Origin_Latitude As Double = Origins_Latitudes(Datum) * Deg_Rad
            Dim Central_Meridian As Double = Centrals_Meridians(Datum) * Deg_Rad
            Dim Std_Parallel_1 As Double = Std_Parallels_1(Datum) * Deg_Rad
            Dim Std_Parallel_2 As Double = Std_Parallels_2(Datum) * Deg_Rad
            Dim False_Easting As Double = False_Eastings(Datum)
            Dim False_Northing As Double = False_Northings(Datum)

            Dim inv_f As Double = 1 / F
            Dim es2, es_sin, t0, t1, t2, t_olat, m0, m1, m2, n, const_value As Double

            Dim lambert1_error_Code As LAMBERT_1, Error_Code As LAMBERT_2
            If a <= 0 Then
                Error_Code = Error_Code Or LAMBERT_2.A_ERROR
            End If
            If (inv_f < 250) OrElse (inv_f > 350) Then
                Error_Code = Error_Code Or LAMBERT_2.INV_F_ERROR
            End If
            If (Origin_Latitude < -MAX_LAT) OrElse (Origin_Latitude > MAX_LAT) Then
                Error_Code = Error_Code Or LAMBERT_2.ORIGIN_LAT_ERROR
            End If
            If (Std_Parallel_1 < -MAX_LAT) OrElse (Std_Parallel_1 > MAX_LAT) Then
                Error_Code = Error_Code Or LAMBERT_2.FIRST_STDP_ERROR
            End If
            If (Std_Parallel_2 < -MAX_LAT) OrElse (Std_Parallel_2 > MAX_LAT) Then
                Error_Code = Error_Code Or LAMBERT_2.SECOND_STDP_ERROR
            End If
            If (Std_Parallel_1 = 0) AndAlso (Std_Parallel_2 = 0) Then
                Error_Code = Error_Code Or LAMBERT_2.FIRST_SECOND_ERROR
            End If
            If Std_Parallel_1 = -Std_Parallel_2 Then
                Error_Code = Error_Code Or LAMBERT_2.HEMISPHERE_ERROR
            End If
            If (Central_Meridian < -PI) OrElse (Central_Meridian > TwoPI) Then
                Error_Code = Error_Code Or LAMBERT_2.CENT_MER_ERROR
            End If
            If Error_Code = LAMBERT_2.NO_ERROR Then
                Lambert_a = a
                Lambert_f = F
                Lambert_Origin_Lat = Origin_Latitude
                Lambert_Std_Parallel_1 = Std_Parallel_1
                Lambert_Std_Parallel_2 = Std_Parallel_2
                If Central_Meridian > PI Then
                    Central_Meridian -= TwoPI
                End If
                Lambert_Origin_Long = Central_Meridian
                Lambert_False_Easting = False_Easting
                Lambert_False_Northing_M = False_Northing
                If Math.Abs(Lambert_Std_Parallel_1 - Lambert_Std_Parallel_2) > 0.0000000001 Then
                    es2 = 2.0R * Lambert_f - Lambert_f * Lambert_f
                    es = Math.Sqrt(es2)
                    es_OVER_2 = es / 2.0R
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_Origin_Lat))
                    t_olat = LAMBERT_t(Lambert_Origin_Lat, es_sin)
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_Std_Parallel_1))
                    m1 = LAMBERT_m(Math.Cos(Lambert_Std_Parallel_1), es_sin)
                    t1 = LAMBERT_t(Lambert_Std_Parallel_1, es_sin)
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_Std_Parallel_2))
                    m2 = LAMBERT_m(Math.Cos(Lambert_Std_Parallel_2), es_sin)
                    t2 = LAMBERT_t(Lambert_Std_Parallel_2, es_sin)
                    n = Math.Log(m1 / m2) / Math.Log(t1 / t2)
                    Lambert_lat0 = Math.Asin(n)
                    es_sin = ES_SIN_Func(Math.Sin(Lambert_lat0))
                    m0 = LAMBERT_m(Math.Cos(Lambert_lat0), es_sin)
                    t0 = LAMBERT_t(Lambert_lat0, es_sin)
                    Lambert_k0 = (m1 / m0) * (Math.Pow(t0 / t1, n))
                    const_value = ((Lambert_a * m2) / (n * Math.Pow(t2, n)))
                    Lambert_false_northing = (const_value * Math.Pow(t_olat, n)) - (const_value * Math.Pow(t0, n)) + Lambert_False_Northing_M
                Else
                    Lambert_lat0 = Lambert_Std_Parallel_1
                    Lambert_k0 = 1
                    Lambert_false_northing = Lambert_False_Northing_M
                End If
                lambert1_error_Code = Set_LAMBCC1_Parameters(Lambert_a, Lambert_f, Lambert_lat0, Lambert_Origin_Long, Lambert_False_Easting,
                    Lambert_false_northing, Lambert_k0)
                If lambert1_error_Code <> LAMBERT_1.NO_ERROR Then
                    If (lambert1_error_Code And LAMBERT_1.A_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.A_ERROR
                    End If
                    If (lambert1_error_Code And LAMBERT_1.INV_F_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.INV_F_ERROR
                    End If
                    If (lambert1_error_Code And LAMBERT_1.ORIGIN_LAT_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.ORIGIN_LAT_ERROR
                    End If
                    If (lambert1_error_Code And LAMBERT_1.CENT_MER_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.CENT_MER_ERROR
                    End If
                    If (lambert1_error_Code And LAMBERT_1.SCALE_FACTOR_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.SCALE_FACTOR_ERROR
                    End If
                End If
            End If
            Return Error_Code
        End Function
        '''<summary> The function Convert_Lambert_2_To_Geodetic converts Lambert Conformal Conic (2 parallel) projection (easting and northing) coordinates to 
        '''Geodetic (latitude and longitude) coordinates, according to the current ellipsoid and Lambert Conformal Conic (2 parallel) projection parameters. </summary>
        '''<param name="PointGrille"> PointGrille à convertir </param>
        '''<param name="PointLatLon"> résultat de la conversion </param>
        '''<returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_2.NO_ERROR is returned.</returns>
        Private Function Convert_LAMBCC2_To_Geodetic(PointGrille As PointD, ByRef PointLatLon As PointD) As LAMBERT_2
            Dim lambert1_error_Code As LAMBERT_1, Error_Code As LAMBERT_2
            If (PointGrille.X < (Lambert_False_Easting - Lambert_Delta_Easting)) OrElse (PointGrille.X > (Lambert_False_Easting + Lambert_Delta_Easting)) Then
                Error_Code = Error_Code Or LAMBERT_2.EASTING_ERROR
            End If
            If (PointGrille.Y < (Lambert_false_northing - Lambert_Delta_Northing)) OrElse (PointGrille.Y > (Lambert_false_northing + Lambert_Delta_Northing)) Then
                Error_Code = Error_Code Or LAMBERT_2.NORTHING_ERROR
            End If
            If Error_Code = LAMBERT_2.NO_ERROR Then
                lambert1_error_Code = Convert_LAMBCC1_To_Geodetic(PointGrille, PointLatLon)
                If lambert1_error_Code <> LAMBERT_1.NO_ERROR Then
                    If (lambert1_error_Code And LAMBERT_1.EASTING_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.EASTING_ERROR
                    End If
                    If (lambert1_error_Code And LAMBERT_1.NORTHING_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.NORTHING_ERROR
                    End If
                End If
            End If
            Return Error_Code
        End Function
        ''' <summary>The function Convert_Geodetic_To_Lambert_2 converts Geodetic (latitude and longitude) coordinates to Lambert Conformal Conic (2 parallel)
        ''' projection (eastingand northing) coordinates, according to the current ellipsoid and Lambert Conformal Conic (2 parallel) projection parameters.</summary>
        ''' <param name="PointLatLon"> PointLatLon à convertir </param>
        ''' <param name="PointGrille"> résultat de la convertion</param>
        ''' <returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_2.NO_ERROR is returned.</returns>
        Private Function Convert_Geodetic_To_LAMBCC2(PointLatLon As PointD, ByRef PointGrille As PointD) As LAMBERT_2
            Dim lambert1_error_Code As LAMBERT_1, Error_Code As LAMBERT_2
            If (PointLatLon.Lat < -Pi_2) OrElse (PointLatLon.Lat > Pi_2) Then
                Error_Code = Error_Code Or LAMBERT_2.LAT_ERROR
            End If
            If (PointLatLon.Lon < -PI) OrElse (PointLatLon.Lon > TwoPI) Then
                Error_Code = Error_Code Or LAMBERT_2.LON_ERROR
            End If
            If Error_Code = LAMBERT_2.NO_ERROR Then
                lambert1_error_Code = Convert_Geodetic_To_LAMBCC1(PointLatLon, PointGrille)
                If lambert1_error_Code <> LAMBERT_1.NO_ERROR Then
                    If (lambert1_error_Code And LAMBERT_1.LAT_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.LAT_ERROR
                    End If
                    If (lambert1_error_Code And LAMBERT_1.LON_ERROR) <> 0 Then
                        Error_Code = Error_Code Or LAMBERT_2.LON_ERROR
                    End If
                End If
            End If
            Return Error_Code
        End Function
#End Region
    End Module
End Namespace