Namespace Coordonnees
    ''' <summary> Ce module contient les fonctions permettant la conversion entre les coordonnées Géodétique (Latitude et Longitude) et une projection 
    ''' Lambert Conforme Conique à 1 parallèle en X et Y ou l'inverse traduit du projet Geotrans écrit en C </summary>
    Friend Module ProjectionLambert_1
#Region "Variables et Constantes"
        '''<summary> Donne par defaut les différentes constantes liées à la projection Lambert 1 et au système CLARKE 1880 ("NTF") </summary>
        Private Const MAX_LAT As Double = Deg_Rad * 89.9997222222222R ' ((PI * 89.9997222222222) / 180)
        Private Const MIN_SCALE_FACTOR As Double = 0.000000001R
        Private Const ONE_SECOND As Double = 0.00000489R
        Private Lambert_1_a As Double = RayonWGS84
        Private Lambert_1_f As Double = F_WGS84 ' 1 / 298.257223563
        Private es As Double = 0.0818191908426219R
        Private es_OVER_2 As Double = 0.040909595421311R
        Private Lambert_1_n As Double = 0.70710678118655R
        Private Lambert_1_rho0 As Double = 6388838.2901212R
        Private Lambert_1_rho_olat As Double = 6388838.2901211R
        Private Lambert_1_t0 As Double = 0.41618115138974R
        Private Lambert_1_Origin_Lat As Double = Deg_Rad * 45.0R ' (45 * PI / 180)
        Private Lambert_1_Origin_Long As Double = 0.0R
        Private Lambert_1_False_Northing As Double = 0.0R
        Private Lambert_1_False_Easting As Double = 0.0R
        Private Lambert_1_Scale_Factor As Double = 1.0R
        Private Const Lambert_Delta_Easting As Double = 400_000_000.0R
        Private Const Lambert_Delta_Northing As Double = 400_000_000.0R

        '''<summary> Donne différentes erreurs pouvant survenir lors de la transformation des coordonnées </summary>
        <Flags>
        Friend Enum LAMBERT_1 As Integer
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
            '''<summary>Flag erreur concernant l'origine de la Latitude</summary>
            ORIGIN_LAT_ERROR = 16
            '''<summary>Flag erreur concernant le méridien central</summary>
            CENT_MER_ERROR = 32
            '''<summary>Flag erreur concernant le facteur d'échelle</summary>
            SCALE_FACTOR_ERROR = 64
            '''<summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 128
            '''<summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 256
        End Enum
#End Region
#Region "Functions and Procedures"
        Private Function LAMBERT_m(clat As Double, essin As Double) As Double
            Return (clat / Math.Sqrt(1.0R - essin * essin))
        End Function
        Private Function LAMBERT_t(lat As Double, essin As Double) As Double
            Return Math.Tan(Pi_4 - lat / 2.0R) / Math.Pow((1.0R - essin) / (1.0R + essin), es_OVER_2)
        End Function
        Private Function ES_SIN_Func(sinlat As Double) As Double
            Return (es * sinlat)
        End Function
        '''<summary>The function Set_Lambert_1_Parameters receives the ellipsoid parameters and Lambert Conformal Conic (1 parallel)
        ''' projection parameters as inputs, and sets the corresponding state variables. </summary>
        '''<param name="a">Semi-major axis of ellipsoid, in meters(input)</param>
        '''<param name="f">Flattening of ellipsoid(input)</param>
        '''<param name="Origin_Latitude">Latitude of origin, in radians(input)</param>
        '''<param name="Central_Meridian">Longitude of origin, in radians(input)</param>
        '''<param name="False_Easting">False easting, in meters(input)</param>
        '''<param name="False_Northing">False northing, in meters(input)</param>
        '''<param name="Scale_Factor">Projection scale factor(input)</param>
        '''<returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_1.NO_ERROR is returned.</returns>
        Friend Function Set_LAMBCC1_Parameters(a As Double, f As Double, Origin_Latitude As Double, Central_Meridian As Double,
                                                         False_Easting As Double, False_Northing As Double, Scale_Factor As Double) As LAMBERT_1
            Dim es2 As Double, es_sin As Double, m0 As Double, inv_f As Double = 1.0R / f
            Dim Error_Code As LAMBERT_1
            If a <= 0 Then
                Error_Code = Error_Code Or LAMBERT_1.A_ERROR
            End If
            If (inv_f < 250.0R) OrElse (inv_f > 350.0R) Then
                Error_Code = Error_Code Or LAMBERT_1.INV_F_ERROR
            End If
            If ((Origin_Latitude < -MAX_LAT) OrElse (Origin_Latitude > MAX_LAT)) OrElse (Origin_Latitude > -ONE_SECOND) AndAlso (Origin_Latitude < ONE_SECOND) Then
                Error_Code = Error_Code Or LAMBERT_1.ORIGIN_LAT_ERROR
            End If
            If (Central_Meridian < -PI) OrElse (Central_Meridian > TwoPI) Then
                Error_Code = Error_Code Or LAMBERT_1.CENT_MER_ERROR
            End If
            If Scale_Factor < MIN_SCALE_FACTOR Then
                Error_Code = Error_Code Or LAMBERT_1.SCALE_FACTOR_ERROR
            End If
            If Error_Code = LAMBERT_1.NO_ERROR Then
                Lambert_1_a = a
                Lambert_1_f = f
                Lambert_1_Origin_Lat = Origin_Latitude
                If Central_Meridian > PI Then
                    Central_Meridian -= TwoPI
                End If
                Lambert_1_Origin_Long = Central_Meridian
                Lambert_1_False_Easting = False_Easting
                Lambert_1_False_Northing = False_Northing
                Lambert_1_Scale_Factor = Scale_Factor
                es2 = 2.0R * Lambert_1_f - Lambert_1_f * Lambert_1_f
                es = Math.Sqrt(es2)
                es_OVER_2 = es / 2.0R
                Lambert_1_n = Math.Sin(Lambert_1_Origin_Lat)
                es_sin = ES_SIN_Func(Math.Sin(Lambert_1_Origin_Lat))
                m0 = LAMBERT_m(Math.Cos(Lambert_1_Origin_Lat), es_sin)
                Lambert_1_t0 = LAMBERT_t(Lambert_1_Origin_Lat, es_sin)
                Lambert_1_rho0 = Lambert_1_a * Lambert_1_Scale_Factor * m0 / Lambert_1_n
                Lambert_1_rho_olat = Lambert_1_rho0
            End If
            Return Error_Code
        End Function
        '''<summary>The function Convert_Lambert_1_To_Geodetic converts Lambert ConformalConic (1 parallel) projection (easting and northing) coordinates to Geodetic
        '''(latitude and longitude) coordinates, according to the current ellipsoid and Lambert Conformal Conic (1 parallel) projection parameters.</summary>
        '''<param name="PointGrille"> point à convertir </param>
        '''<param name="PointLatLon"> resultat de la convertion </param>
        '''<returns>If any errors occur, the error code(s) are returned by the function PointLatLon is empty, otherwise LAMBERT_NO_ERROR is returned.</returns>
        Friend Function Convert_LAMBCC1_To_Geodetic(PointGrille As PointD, ByRef PointLatLon As PointD) As LAMBERT_1
            Dim dx, dy, rho, rho_olat_MINUS_dy, t, PHI, es_sin, tempPHI, theta As Double
            Dim tolerance As Double = 0.000000000485R
            Dim count As Integer = 30
            Dim Error_Code As LAMBERT_1 = LAMBERT_1.NO_ERROR
            If (PointGrille.X < (Lambert_1_False_Easting - Lambert_Delta_Easting)) OrElse (PointGrille.X > (Lambert_1_False_Easting + Lambert_Delta_Easting)) Then
                Error_Code = Error_Code Or LAMBERT_1.EASTING_ERROR
            End If
            If (PointGrille.Y < (Lambert_1_False_Northing - Lambert_Delta_Northing)) OrElse (PointGrille.Y > (Lambert_1_False_Northing + Lambert_Delta_Northing)) Then
                Error_Code = Error_Code Or LAMBERT_1.NORTHING_ERROR
            End If
            If Error_Code = LAMBERT_1.NO_ERROR Then
                dy = PointGrille.Y - Lambert_1_False_Northing
                dx = PointGrille.X - Lambert_1_False_Easting
                rho_olat_MINUS_dy = Lambert_1_rho_olat - dy
                rho = Math.Sqrt(dx * dx + rho_olat_MINUS_dy * rho_olat_MINUS_dy)
                If Lambert_1_n < 0 Then
                    rho *= -1
                    dx *= -1
                    rho_olat_MINUS_dy *= -1
                End If
                If rho <> 0 Then
                    theta = Math.Atan2(dx, rho_olat_MINUS_dy) / Lambert_1_n
                    t = Lambert_1_t0 * Math.Pow(rho / Lambert_1_rho0, 1 / Lambert_1_n)
                    PHI = Pi_2 - 2 * Math.Atan(t)
                    While (Math.Abs(PHI - tempPHI) > tolerance) AndAlso (count > 0)
                        tempPHI = PHI
                        es_sin = ES_SIN_Func(Math.Sin(PHI))
                        PHI = Pi_2 - 2 * Math.Atan(t * Math.Pow((1 - es_sin) / (1 + es_sin), es_OVER_2))
                        count -= 1
                    End While
                    If count = 0 Then
                        Error_Code = Error_Code Or LAMBERT_1.NORTHING_ERROR
                        Return Error_Code
                    End If
                    PointLatLon.Lat = PHI
                    PointLatLon.Lon = theta + Lambert_1_Origin_Long
                    If Math.Abs(PointLatLon.Lat) < 0.0000002 Then
                        PointLatLon.Lat = 0
                    End If
                    If PointLatLon.Lat > Pi_2 Then
                        PointLatLon.Lat = Pi_2
                    ElseIf PointLatLon.Lat < -Pi_2 Then
                        PointLatLon.Lat = -Pi_2
                    End If
                    If PointLatLon.Lon > PI Then
                        If PointLatLon.Lon - PI < 0.0000035 Then
                            PointLatLon.Lon = PI
                        Else
                            PointLatLon.Lon -= TwoPI
                        End If
                    End If
                    If PointLatLon.Lon < -PI Then
                        If Math.Abs(PointLatLon.Lon + PI) < 0.0000035 Then
                            PointLatLon.Lon = -PI
                        Else
                            PointLatLon.Lon += TwoPI
                        End If
                    End If

                    If Math.Abs(PointLatLon.Lon) < 0.0000002 Then
                        PointLatLon.Lon = 0
                    End If
                    If PointLatLon.Lon > PI Then
                        PointLatLon.Lon = PI
                    ElseIf PointLatLon.Lon < -PI Then
                        PointLatLon.Lon = -PI
                    End If
                Else
                    If Lambert_1_n > 0 Then
                        PointLatLon.Lat = Pi_2
                    Else
                        PointLatLon.Lat = -Pi_2
                    End If
                    PointLatLon.Lon = Lambert_1_Origin_Long
                End If
            End If
            Return Error_Code
        End Function
        ''' <summary>The function Convert_Geodetic_To_Lambert_1 converts Geodetic (latitude and longitude) coordinates to Lambert Conformal Conic (1 parallel) 
        ''' projection (easting and northing) coordinates, according to the current ellipsoid and
        ''' Lambert Conformal Conic (1 parallel) projection parameters. </summary>
        ''' <param name="PointLatLon"> point à convertir en rad </param>
        ''' <param name="PointGrille"> résultat de la conversion en mètres </param>
        ''' <returns>If any errors occur, the error code(s) are returned by the function, otherwise LAMBERT_NO_ERROR is returned.</returns>
        Friend Function Convert_Geodetic_To_LAMBCC1(PointLatLon As PointD, ByRef PointGrille As PointD) As LAMBERT_1
            Dim t As Double, rho As Double, dlam As Double, theta As Double
            Dim Error_Code As LAMBERT_1
            If (PointLatLon.Lat < -Pi_2) OrElse (PointLatLon.Lat > Pi_2) Then
                Error_Code = Error_Code Or LAMBERT_1.LAT_ERROR
            End If
            If (PointLatLon.Lon < -PI) OrElse (PointLatLon.Lon > TwoPI) Then
                Error_Code = Error_Code Or LAMBERT_1.LON_ERROR
            End If
            If Error_Code = LAMBERT_1.NO_ERROR Then
                If Math.Abs(Math.Abs(PointLatLon.Lat) - Pi_2) > 0.0000000001 Then
                    t = LAMBERT_t(PointLatLon.Lat, ES_SIN_Func(Math.Sin(PointLatLon.Lat)))
                    rho = Lambert_1_rho0 * Math.Pow(t / Lambert_1_t0, Lambert_1_n)
                Else
                    If (PointLatLon.Lat * Lambert_1_n) <= 0 Then
                        Error_Code = Error_Code Or LAMBERT_1.LAT_ERROR
                        Return (Error_Code)
                    End If
                    rho = 0
                End If
                dlam = PointLatLon.Lon - Lambert_1_Origin_Long
                If dlam > PI Then
                    dlam -= TwoPI
                End If
                If dlam < -PI Then
                    dlam += TwoPI
                End If
                theta = Lambert_1_n * dlam
                PointGrille.X = rho * Math.Sin(theta) + Lambert_1_False_Easting
                PointGrille.Y = Lambert_1_rho_olat - rho * Math.Cos(theta) + Lambert_1_False_Northing
            End If
            Return Error_Code
        End Function
#End Region
    End Module
End Namespace