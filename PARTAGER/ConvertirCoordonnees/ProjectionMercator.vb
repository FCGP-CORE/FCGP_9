Namespace Coordonnees
    '''<summary> This component provides conversions between geodetic coordinates  (latitude and longitudes) and Universal Transverse Mercator (UTM)
    '''projection (zone, hemisphere, easting, and northing) coordinates.  (latitude and longitudes) and Universal Transverse Mercator (UTM) projection (zone, hemisphere, easting, and northing) coordinates.
    '''UTM is intended for reuse by any application that performs a Universal Transverse Mercator (UTM) projection or its inverse traduit du projet Geotrans écrit en C </summary>
    Friend Module ProjectionMercator
#Region "Constantes et Variables"
        Private Const MAX_LAT As Double = Deg_Rad * 89.99R '((PI * 89.99) / 180)
        Private Const MAX_DELTA_LONG As Double = Deg_Rad * 90.0R '((PI * 90) / 180)
        Private Const MIN_SCALE_FACTOR As Double = 0.3R
        Private Const MAX_SCALE_FACTOR As Double = 3.0R
        Private TranMerc_a As Double = RayonWGS84
        Private TranMerc_f As Double = F_WGS84 '1 / 298.257223563
        Private TranMerc_es As Double = 66_943_799_901_413_800.0R
        Private TranMerc_ebs As Double = 67_394_967_565_869.0R
        Private TranMerc_Origin_Lat As Double
        Private TranMerc_Origin_Long As Double
        Private TranMerc_False_Northing As Double
        Private TranMerc_False_Easting As Double
        Private TranMerc_Scale_Factor As Double = 1.0R
        Private TranMerc_ap As Double = 6_367_449.1458008R
        Private TranMerc_bp As Double = 16_038.508696861R
        Private TranMerc_cp As Double = 16.832613334334R
        Private TranMerc_dp As Double = 21_984_404_273_757.0R
        Private TranMerc_ep As Double = 0.000031148371319283R
        Private TranMerc_Delta_Easting As Double = 40_000_000.0R
        Private TranMerc_Delta_Northing As Double = 40_000_000.0R

        '''<summary>Liste des différentes erreurs pouvant survenir lors de la transformation des coordonnées Mercator </summary>
        <Flags>
        Friend Enum TRANMERC As Integer
            '''<summary>Flag OK</summary>"
            NO_ERROR = 0
            '''<summary>Flag erreur concernant la Latitude</summary>"
            LAT_ERROR = 1
            '''<summary>Flag erreur concernant la Longitude</summary>"
            LON_ERROR = 2
            '''<summary>Flag erreur concernant les abscisses(Easting ou X)</summary>"
            EASTING_ERROR = 4
            '''<summary>Flag erreur concernant les ordonnées(Northing ou Y)</summary>"
            NORTHING_ERROR = 8
            '''<summary>Flag erreur concernant l'origine de la Latitude</summary>"
            ORIGIN_LAT_ERROR = 16
            '''<summary>Flag erreur concernant l'origine de la Longitude</summary>"
            CENT_MER_ERROR = 32
            '''<summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>"
            A_ERROR = 64
            '''<summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>"
            INV_F_ERROR = 128
            '''<summary>Flag erreur concernant le facteur d'échelle</summary>"
            SCALE_FACTOR_ERROR = 256
            '''<summary>Flag erreur sur la distrosion de la Longitude</summary>"
            LON_WARNING = 512
        End Enum
#End Region
#Region "Functions and Procedures"
        Private Function SPHTMD(Latitude As Double) As Double
            Return TranMerc_ap * Latitude - TranMerc_bp * Math.Sin(2.0R * Latitude) + TranMerc_cp * Math.Sin(4.0R * Latitude) -
                   TranMerc_dp * Math.Sin(6.0R * Latitude) + TranMerc_ep * Math.Sin(8.0R * Latitude)
        End Function
        Private Function SPHSN(Latitude As Double) As Double
            Return TranMerc_a / Math.Sqrt(1.0R - TranMerc_es * Math.Pow(Math.Sin(Latitude), 2.0R))
        End Function
        Private Function SPHSR(Latitude As Double) As Double
            Return TranMerc_a * (1.0R - TranMerc_es) / Math.Pow(DENOM(Latitude), 3.0R)
        End Function
        Private Function DENOM(Latitude As Double) As Double
            Return Math.Sqrt(1.0R - TranMerc_es * Math.Pow(Math.Sin(Latitude), 2.0R))
        End Function
        '''<summary>"The function Convert_Geodetic_To_Transverse_Mercator converts geodetic
        '''(latitude and longitude) coordinates to Transverse Mercator projection
        '''(easting and northing) coordinates, according to the current ellipsoid
        '''and Transverse Mercator projection coordinates.</summary>
        '''<param name="PointLatLon"> PointLatLon en rad à convertir </param>
        '''<param name="PointUtm"> résultat de la conversion </param>
        '''<returns>If any errors occur, the error code(s) are returned by the function, otherwise TRANMERC.NO_ERROR is returned.</returns>
        Friend Function Convert_Geodetic_To_Transverse_Mercator(PointLatLon As PointD, ByRef PointUtm As PointD) As TRANMERC
            Dim c As Double, c2 As Double, c3 As Double, c5 As Double, c7 As Double
            Dim dlam As Double
            Dim eta As Double
            Dim eta2 As Double, eta3 As Double, eta4 As Double
            Dim s As Double
            Dim sn As Double
            Dim t As Double, tan2 As Double, tan3 As Double, tan4 As Double, tan5 As Double, tan6 As Double
            Dim t1 As Double
            Dim t2 As Double
            Dim t3 As Double
            Dim t4 As Double
            Dim t5 As Double
            Dim t6 As Double
            Dim t7 As Double
            Dim t8 As Double
            Dim t9 As Double
            Dim tmd As Double
            Dim tmdo As Double
            Dim Error_Code As TRANMERC
            Dim temp_Origin As Double, temp_Long As Double
            If (PointLatLon.Lat < -MAX_LAT) OrElse (PointLatLon.Lat > MAX_LAT) Then
                Error_Code = Error_Code Or TRANMERC.LAT_ERROR
            End If
            If PointLatLon.Lon > PI Then
                PointLatLon.Lon -= TwoPI
            End If
            If (PointLatLon.Lon < (TranMerc_Origin_Long - MAX_DELTA_LONG)) OrElse (PointLatLon.Lon > (TranMerc_Origin_Long + MAX_DELTA_LONG)) Then
                If PointLatLon.Lon < 0 Then
                    temp_Long = PointLatLon.Lon + TwoPI
                Else
                    temp_Long = PointLatLon.Lon
                End If
                If TranMerc_Origin_Long < 0 Then
                    temp_Origin = TranMerc_Origin_Long + TwoPI
                Else
                    temp_Origin = TranMerc_Origin_Long
                End If
                If (temp_Long < (temp_Origin - MAX_DELTA_LONG)) OrElse (temp_Long > (temp_Origin + MAX_DELTA_LONG)) Then
                    Error_Code = Error_Code Or TRANMERC.LON_ERROR
                End If
            End If
            If Error_Code = TRANMERC.NO_ERROR Then
                dlam = PointLatLon.Lon - TranMerc_Origin_Long
                If Math.Abs(dlam) > (9 * Deg_Rad) Then
                    Error_Code = Error_Code Or TRANMERC.LON_WARNING
                End If
                If dlam > PI Then
                    dlam -= TwoPI
                End If
                If dlam < -PI Then
                    dlam += TwoPI
                End If
                If Math.Abs(dlam) < 0.0000000002 Then
                    dlam = 0
                End If
                s = Math.Sin(PointLatLon.Lat)
                c = Math.Cos(PointLatLon.Lat)
                c2 = c * c
                c3 = c2 * c
                c5 = c3 * c2
                c7 = c5 * c2
                t = Math.Tan(PointLatLon.Lat)
                tan2 = t * t
                tan3 = tan2 * t
                tan4 = tan3 * t
                tan5 = tan4 * t
                tan6 = tan5 * t
                eta = TranMerc_ebs * c2
                eta2 = eta * eta
                eta3 = eta2 * eta
                eta4 = eta3 * eta
                sn = SPHSN(PointLatLon.Lat)
                tmd = SPHTMD(PointLatLon.Lat)
                tmdo = SPHTMD(TranMerc_Origin_Lat)
                t1 = (tmd - tmdo) * TranMerc_Scale_Factor
                t2 = sn * s * c * TranMerc_Scale_Factor / 2
                t3 = sn * s * c3 * TranMerc_Scale_Factor * (5 - tan2 + 9 * eta + 4 * eta2) / 24
                t4 = sn * s * c5 * TranMerc_Scale_Factor * (61 - 58 * tan2 + tan4 + 270 * eta - 330 * tan2 * eta + 445 * eta2 + 324 * eta3 - 680 * tan2 * eta2 + 88 * eta4 -
                                                            600 * tan2 * eta3 - 192 * tan2 * eta4) / 720
                t5 = sn * s * c7 * TranMerc_Scale_Factor * (1385 - 3111 * tan2 + 543 * tan4 - tan6) / 40320
                PointUtm.Y = TranMerc_False_Northing + t1 + Math.Pow(dlam, 2) * t2 + Math.Pow(dlam, 4) * t3 + Math.Pow(dlam, 6) * t4 + Math.Pow(dlam, 8) * t5
                t6 = sn * c * TranMerc_Scale_Factor
                t7 = sn * c3 * TranMerc_Scale_Factor * (1 - tan2 + eta) / 6
                t8 = sn * c5 * TranMerc_Scale_Factor * (5 - 18 * tan2 + tan4 + 14 * eta - 58 * tan2 * eta + 13 * eta2 + 4 * eta3 - 64 * tan2 * eta2 - 24 * tan2 * eta3) / 120
                t9 = sn * c7 * TranMerc_Scale_Factor * (61 - 479 * tan2 + 179 * tan4 - tan6) / 5040
                PointUtm.X = TranMerc_False_Easting + dlam * t6 + Math.Pow(dlam, 3) * t7 + Math.Pow(dlam, 5) * t8 + Math.Pow(dlam, 7) * t9
            End If
            Return Error_Code
        End Function
        ''' <summary>The function Convert_Transverse_Mercator_To_Geodetic converts Transverse
        '''Mercator projection (easting and northing) coordinates to geodetic
        '''(latitude and longitude) coordinates, according to the current ellipsoid
        '''and Transverse Mercator projection parameters.</summary>
        ''' <param name="PointUtm"> Point à convertir </param>
        ''' <param name="PointLatLon"> résultat de la conversion</param>
        ''' <returns>If any errors occur, the error code(s) are returned by the function, otherwise TRANMERC.NO_ERROR is returned.</returns>
        Friend Function Convert_Transverse_Mercator_To_Geodetic(PointUtm As PointD, ByRef PointLatLon As PointD) As TRANMERC
            Dim c As Double
            Dim de As Double
            Dim dlam As Double
            Dim eta As Double, eta2 As Double, eta3 As Double, eta4 As Double
            Dim ftphi As Double
            Dim i As Integer
            Dim sn As Double
            Dim sr As Double
            Dim t As Double, tan2 As Double, tan4 As Double
            Dim t10 As Double
            Dim t11 As Double
            Dim t12 As Double
            Dim t13 As Double
            Dim t14 As Double
            Dim t15 As Double
            Dim t16 As Double
            Dim t17 As Double
            Dim tmd As Double
            Dim tmdo As Double
            Dim Error_Code As TRANMERC
            If (PointUtm.X < (TranMerc_False_Easting - TranMerc_Delta_Easting)) OrElse (PointUtm.X > (TranMerc_False_Easting + TranMerc_Delta_Easting)) Then
                Error_Code = Error_Code Or TRANMERC.EASTING_ERROR
            End If
            If (PointUtm.Y < (TranMerc_False_Northing - TranMerc_Delta_Northing)) OrElse (PointUtm.Y > (TranMerc_False_Northing + TranMerc_Delta_Northing)) Then
                Error_Code = Error_Code Or TRANMERC.NORTHING_ERROR
            End If
            If Error_Code = TRANMERC.NO_ERROR Then
                tmdo = SPHTMD(TranMerc_Origin_Lat)
                tmd = tmdo + (PointUtm.Y - TranMerc_False_Northing) / TranMerc_Scale_Factor
                sr = SPHSR(0)
                ftphi = tmd / sr
                For i = 0 To 4
                    t10 = SPHTMD(ftphi)
                    sr = SPHSR(ftphi)
                    ftphi += (tmd - t10) / sr
                Next
                sr = SPHSR(ftphi)
                sn = SPHSN(ftphi)
                c = Math.Cos(ftphi)
                t = Math.Tan(ftphi)
                tan2 = t * t
                tan4 = tan2 * tan2
                eta = TranMerc_ebs * Math.Pow(c, 2)
                eta2 = eta * eta
                eta3 = eta2 * eta
                eta4 = eta3 * eta
                de = PointUtm.X - TranMerc_False_Easting
                If Math.Abs(de) < 1 Then
                    de = 0
                End If
                t10 = t / (2 * sr * sn * Math.Pow(TranMerc_Scale_Factor, 2))
                t11 = t * (5 + 3 * tan2 + eta - 4 * Math.Pow(eta, 2) - 9 * tan2 * eta) / (24 * sr * Math.Pow(sn, 3) * Math.Pow(TranMerc_Scale_Factor, 4))
                t12 = t * (61 + 90 * tan2 + 46 * eta + 45 * tan4 - 252 * tan2 * eta - 3 * eta2 + 100 * eta3 - 66 * tan2 * eta2 -
                           90 * tan4 * eta + 88 * eta4 + 225 * tan4 * eta2 + 84 * tan2 * eta3 - 192 * tan2 * eta4) / (720 * sr * Math.Pow(sn, 5) * Math.Pow(TranMerc_Scale_Factor, 6))
                t13 = t * (1385 + 3633 * tan2 + 4095 * tan4 + 1575 * Math.Pow(t, 6)) / (40320 * sr * Math.Pow(sn, 7) * Math.Pow(TranMerc_Scale_Factor, 8))
                PointLatLon.Lat = ftphi - Math.Pow(de, 2) * t10 + Math.Pow(de, 4) * t11 - Math.Pow(de, 6) * t12 + Math.Pow(de, 8) * t13
                t14 = 1 / (sn * c * TranMerc_Scale_Factor)
                t15 = (1 + 2 * tan2 + eta) / (6 * Math.Pow(sn, 3) * c * Math.Pow(TranMerc_Scale_Factor, 3))
                t16 = (5 + 6 * eta + 28 * tan2 - 3 * eta2 + 8 * tan2 * eta + 24 * tan4 - 4 * eta3 + 4 * tan2 * eta2 +
                       24 * tan2 * eta3) / (120 * Math.Pow(sn, 5) * c * Math.Pow(TranMerc_Scale_Factor, 5))
                t17 = (61 + 662 * tan2 + 1320 * tan4 + 720 * Math.Pow(t, 6)) / (5040 * Math.Pow(sn, 7) * c * Math.Pow(TranMerc_Scale_Factor, 7))
                dlam = de * t14 - Math.Pow(de, 3) * t15 + Math.Pow(de, 5) * t16 - Math.Pow(de, 7) * t17
                PointLatLon.Lon = TranMerc_Origin_Long + dlam
                If Math.Abs(PointLatLon.Lat) > (90 * Deg_Rad) Then 'If Math.Abs(Latitude) > (90 * PI / 180) Then
                    Error_Code = Error_Code Or TRANMERC.NORTHING_ERROR
                End If
                If PointLatLon.Lon > PI Then
                    PointLatLon.Lon -= TwoPI
                    If Math.Abs(PointLatLon.Lon) > PI Then
                        Error_Code = Error_Code Or TRANMERC.EASTING_ERROR
                    End If
                End If
                If PointLatLon.Lat > 10000000000 Then
                    Error_Code = Error_Code Or TRANMERC.LON_WARNING
                End If
            End If
            Return Error_Code
        End Function
        '''<summary>The function Set_Tranverse_Mercator_Parameters receives the ellipsoid parameters and
        '''Tranverse Mercator projection parameters as inputs, andsets the corresponding state variables. </summary>
        '''<param name="a">Semi-major axis of ellipsoid, in meters(input)</param>
        '''<param name="f">Flattening of ellipsoid(input)</param>
        '''<param name="Origin_Latitude">Latitude in radians at the origin of the projection(input)</param>
        '''<param name="Central_Meridian">Longitude in radians at the center of the projection(input)</param>
        '''<param name="False_Easting">Easting/X at the center of the projection(input)</param>
        '''<param name="False_Northing">Northing/Y at the center of the projection(input)</param>
        '''<param name="Scale_Factor">Projection scale factor(input)</param>
        '''<returns>If any errors occur, the error code(s) are returned by the function, otherwise TRANMERC.NO_ERROR is returned.</returns>
        Friend Function Set_Transverse_Mercator_Parameters(a As Double, f As Double, Origin_Latitude As Double, Central_Meridian As Double,
                                                           False_Easting As Double, False_Northing As Double, Scale_Factor As Double) As TRANMERC
            Dim tn As Double, tn2 As Double, tn3 As Double, tn4 As Double, tn5 As Double
            Dim dummy_northing As Double = 0.0R
            Dim TranMerc_b As Double
            Dim inv_f As Double = 1.0R / f
            Dim Error_Code As TRANMERC
            If a <= 0 Then
                Error_Code = Error_Code Or TRANMERC.A_ERROR
            End If
            If (inv_f < 250) OrElse (inv_f > 350) Then
                Error_Code = Error_Code Or TRANMERC.INV_F_ERROR
            End If
            If (Origin_Latitude < -Pi_2) OrElse (Origin_Latitude > Pi_2) Then
                Error_Code = Error_Code Or TRANMERC.ORIGIN_LAT_ERROR
            End If
            If (Central_Meridian < -PI) OrElse (Central_Meridian > (2 * PI)) Then
                Error_Code = Error_Code Or TRANMERC.CENT_MER_ERROR
            End If
            If (Scale_Factor < MIN_SCALE_FACTOR) OrElse (Scale_Factor > MAX_SCALE_FACTOR) Then
                Error_Code = Error_Code Or TRANMERC.SCALE_FACTOR_ERROR
            End If
            If Error_Code = TRANMERC.NO_ERROR Then
                TranMerc_a = a
                TranMerc_f = f
                TranMerc_Origin_Lat = 0.0R
                TranMerc_Origin_Long = 0.0R
                TranMerc_False_Northing = 0.0R
                TranMerc_False_Easting = 0.0R
                TranMerc_Scale_Factor = 1.0R
                TranMerc_es = 2.0R * TranMerc_f - TranMerc_f * TranMerc_f
                TranMerc_ebs = (1.0R / (1.0R - TranMerc_es)) - 1.0R
                TranMerc_b = TranMerc_a * (1.0R - TranMerc_f)
                tn = (TranMerc_a - TranMerc_b) / (TranMerc_a + TranMerc_b)
                tn2 = tn * tn
                tn3 = tn2 * tn
                tn4 = tn3 * tn
                tn5 = tn4 * tn
                TranMerc_ap = TranMerc_a * (1 - tn + 5 * (tn2 - tn3) / 4 + 81 * (tn4 - tn5) / 64)
                TranMerc_bp = 3 * TranMerc_a * (tn - tn2 + 7 * (tn3 - tn4) / 8 + 55 * tn5 / 64) / 2
                TranMerc_cp = 15 * TranMerc_a * (tn2 - tn3 + 3 * (tn4 - tn5) / 4) / 16
                TranMerc_dp = 35 * TranMerc_a * (tn3 - tn4 + 11 * tn5 / 16) / 48
                TranMerc_ep = 315 * TranMerc_a * (tn4 - tn5) / 512
                Dim Ret As New PointD(TranMerc_Delta_Easting, TranMerc_Delta_Northing)
                Convert_Geodetic_To_Transverse_Mercator(New PointD(MAX_LAT, MAX_DELTA_LONG), Ret)
                TranMerc_Delta_Northing = Ret.Y
                Ret.Y = dummy_northing
                Convert_Geodetic_To_Transverse_Mercator(New PointD(0, MAX_DELTA_LONG), Ret)
                TranMerc_Delta_Easting = Ret.X
                TranMerc_Origin_Lat = Origin_Latitude
                If Central_Meridian > PI Then
                    Central_Meridian -= TwoPI
                End If
                TranMerc_Origin_Long = Central_Meridian
                TranMerc_False_Northing = False_Northing
                TranMerc_False_Easting = False_Easting
                TranMerc_Scale_Factor = Scale_Factor
            End If
            Return Error_Code
        End Function
#End Region
    End Module
End Namespace