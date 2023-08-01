Namespace Coordonnees
    '''<summary> This component provides conversions between geodetic coordinates  (latitude and longitudes) and Universal Transverse Mercator (UTM)
    '''projection (zone, hemisphere, easting, and northing) coordinates.  (latitude and longitudes) and Universal Transverse Mercator (UTM) projection (zone, hemisphere, easting, and northing) coordinates.
    '''UTM is intended for reuse by any application that performs a Universal Transverse Mercator (UTM) projection or its inverse traduit du projet Geotrans écrit en C </summary>
    Friend Module ProjectionMercatorUtm
#Region "Haut Niveau"
        ''' <summary> UTM en mètres vers ellispsoïde du datum en DD ou en Rad. </summary>
        ''' <param name="PointGrille"> point à convertir </param>
        ''' <param name="Datum"> datum de la projection UTM </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point convertit sont exprimées en DD ou en Rad </param>
        Friend Function ConvertUtmToLL(Datum As Datums, PointGrille As PointProjection, Optional Deg As Boolean = True) As PointD
            If Ellipsoide <> ParametresDatums(Datum).Ellipsoide Then
                SetParametresDatum(Datum) 'datum de départ
            End If
            Dim Ret As New PointD(0, 0)
            If Convert_UTM_To_Geodetic(PointGrille, Ret) = UTM.NO_ERROR Then
                If Deg Then Ret.Scale(Rad_Deg)
                Return Ret
            Else
                Return PointD.Empty
            End If
        End Function
        ''' <summary> Coordonnées lat lon sur ellispsoïde du datum en DD ou en Rad vers Utm </summary>
        ''' <param name="PointLatLon"> point Lat Lon à convertir </param>
        ''' <param name="Datum"> Datum de la projection UTM </param>
        ''' <param name="Zone"> Numéro de la zone à laquelle on veut rattacher le point à convertir. 0 = Zone par défaut </param>
        ''' <param name="Deg"> indique si la coordonnées Lat Lon sont en DD </param>
        ''' <returns></returns>
        Friend Function ConvertLLToUtm(PointLatLon As PointD, Datum As Datums, Optional Zone As Integer = 0, Optional Deg As Boolean = True) As PointProjection
            If Ellipsoide <> ParametresDatums(Datum).Ellipsoide Then
                SetParametresDatum(Datum) 'datum de départ ou Nom
            End If
            If Deg Then PointLatLon.Scale(Deg_Rad)
            Dim Ret As New PointProjection(New PointD(0, 0), Zone)
            If Convert_Geodetic_To_UTM(PointLatLon, Ret) = UTM.NO_ERROR Then
                Return Ret
            Else
                Return New PointProjection
            End If
        End Function
#End Region
#Region "Constantes"
        '''<summary>Liste des différentes erreurs pouvant survenir lors de la transformation des coordonnées UTM </summary>
        <Flags>
        Private Enum UTM As Integer
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
            '''<summary>Flag erreur concernant la zone UTM</summary>
            ZONE_ERROR = 16
            '''<summary>Flag erreur concernant l'hémisphère</summary>
            HEMISPHERE_ERROR = 32
            '''<summary>Flag erreur concernant la zone override</summary>
            ZONE_OVERRIDE_ERROR = 64
            '''<summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 128
            '''<summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 256
        End Enum
#End Region
#Region "Functions and Procedures"
        '''<summary>  La fonction Convert_Geodetic_To_UTM convertit des coordonnées géodétic (Longitude et Latitude)
        ''' en coordonnées UTM (Zone, Hémisphère, X et Y) en fonction de l'éllipsoïde en cours et de la zone éventuellement demandée</summary>
        '''<param name="PointLatLon">Latitude en radians(entrée)</param>
        '''<param name="PointUtm">PointUtm</param>
        '''<returns> If any errors occur, the error code(s) are returned by the function, otherwise UTM.NO_ERROR is returned. </returns>
        Private Function Convert_Geodetic_To_UTM(PointLatLon As PointD, ByRef PointUtm As PointProjection) As UTM
            Const MIN_LAT As Double = Deg_Rad * -80.5R
            Const MAX_LAT As Double = Deg_Rad * 84.5R
            Const MIN_EASTING As Integer = -1350000
            Const MAX_EASTING As Integer = 2200000
            Const MIN_NORTHING As Integer = 0
            Const MAX_NORTHING As Integer = 10000000
            Dim Lat_Degrees, Long_Degrees, temp_zone As Integer
            Dim Error_Code As UTM
            Dim tm_error_code As TRANMERC
            Dim Origin_Latitude As Double, Central_Meridian As Double, False_Easting As Double = 500000.0R, False_Northing As Double, Scale As Double = 0.9996R
            Dim UTM_a As Double = A
            Dim UTM_f As Double = F
            Dim UTM_Override As Integer
            If (PointLatLon.Lat < MIN_LAT) OrElse (PointLatLon.Lat > MAX_LAT) Then
                Error_Code = Error_Code Or UTM.LAT_ERROR
            End If
            If (PointLatLon.Lon < -PI) OrElse (PointLatLon.Lon > TwoPI) Then
                Error_Code = Error_Code Or UTM.LON_ERROR
            End If
            If Error_Code = UTM.NO_ERROR Then
                If PointLatLon.Lon < 0 Then
                    PointLatLon.Lon += TwoPI + 0.0000000001
                End If
                Lat_Degrees = CInt(PointLatLon.Lat * Rad_Deg)
                Long_Degrees = CInt(PointLatLon.Lon * Rad_Deg)
                If PointLatLon.Lon < PI Then
                    temp_zone = 31 + CInt(Math.Floor(PointLatLon.Lon * Rad_Deg)) \ 6
                Else
                    temp_zone = CInt(Math.Floor(PointLatLon.Lon * Rad_Deg)) \ 6 - 29
                End If
                If temp_zone > 60 Then
                    temp_zone = 1
                End If
                If (Lat_Degrees > 55) AndAlso (Lat_Degrees < 64) AndAlso (Long_Degrees > -1) AndAlso (Long_Degrees < 3) Then
                    temp_zone = 31
                End If
                If (Lat_Degrees > 55) AndAlso (Lat_Degrees < 64) AndAlso (Long_Degrees > 2) AndAlso (Long_Degrees < 12) Then
                    temp_zone = 32
                End If
                If (Lat_Degrees > 71) AndAlso (Long_Degrees > -1) AndAlso (Long_Degrees < 9) Then
                    temp_zone = 31
                End If
                If (Lat_Degrees > 71) AndAlso (Long_Degrees > 8) AndAlso (Long_Degrees < 21) Then
                    temp_zone = 33
                End If
                If (Lat_Degrees > 71) AndAlso (Long_Degrees > 20) AndAlso (Long_Degrees < 33) Then
                    temp_zone = 35
                End If
                If (Lat_Degrees > 71) AndAlso (Long_Degrees > 32) AndAlso (Long_Degrees < 42) Then
                    temp_zone = 37
                End If
                If UTM_Override <> 0 Then
                    If (temp_zone = 1) AndAlso (UTM_Override = 60) Then
                        temp_zone = UTM_Override
                    ElseIf (temp_zone = 60) AndAlso (UTM_Override = 1) Then
                        temp_zone = UTM_Override
                    ElseIf ((temp_zone - 1) <= UTM_Override) AndAlso (UTM_Override <= (temp_zone + 1)) Then
                        temp_zone = UTM_Override
                    Else
                        Error_Code = UTM.ZONE_OVERRIDE_ERROR
                    End If
                End If
                If PointUtm.Zone <> 0 Then temp_zone = PointUtm.Zone
                If Error_Code = UTM.NO_ERROR Then
                    If temp_zone >= 31 Then
                        Central_Meridian = (6 * temp_zone - 183) * Deg_Rad
                    Else
                        Central_Meridian = (6 * temp_zone + 177) * Deg_Rad
                    End If
                    PointUtm.Zone = temp_zone
                    If PointLatLon.Lat < 0 Then
                        False_Northing = 10000000
                        PointUtm.Hemisphere = "S"c
                    Else
                        PointUtm.Hemisphere = "N"c
                    End If
                    tm_error_code = Set_Transverse_Mercator_Parameters(UTM_a, UTM_f, Origin_Latitude, Central_Meridian,
                                                                       False_Easting, False_Northing, Scale)
                    tm_error_code = Convert_Geodetic_To_Transverse_Mercator(PointLatLon, PointUtm.Coordonnees)
                    If (PointUtm.Coordonnees.X < MIN_EASTING) OrElse (PointUtm.Coordonnees.X > MAX_EASTING) Then
                        Error_Code = UTM.EASTING_ERROR
                    End If
                    If (PointUtm.Coordonnees.Y < MIN_NORTHING) OrElse (PointUtm.Coordonnees.Y > MAX_NORTHING) Then
                        Error_Code = Error_Code Or UTM.NORTHING_ERROR
                    End If
                End If
            End If
            Return Error_Code
        End Function
        ''' <summary>The function Convert_UTM_To_Geodetic converts UTM projection (zone, hemisphere, easting and northing) coordinates to geodetic(latitude
        '''and  longitude) coordinates, according to the current ellipsoid parameters.</summary>
        ''' <param name="PointUtm"> point à convertir </param>
        ''' <param name="PointLatLon"> résultat de la conversion </param>
        ''' <returns>If any errors occur, the error code(s) are returned by the function, otherwise UTM.NO_ERROR is returned.</returns>
        Private Function Convert_UTM_To_Geodetic(PointUtm As PointProjection, ByRef PointLatLon As PointD) As UTM
            Const MIN_LAT As Double = Deg_Rad * -80.5R
            Const MAX_LAT As Double = Deg_Rad * 84.5R
            Const MIN_EASTING As Integer = -1_350_000
            Const MAX_EASTING As Integer = 2_200_000I
            Const MIN_NORTHING As Integer = 0
            Const MAX_NORTHING As Integer = 10_000_000I
            Dim UTM_a As Double = A
            Dim UTM_f As Double = F
            Dim Error_Code As UTM
            Dim tm_error_code As TRANMERC
            Dim Origin_Latitude As Double, Central_Meridian As Double, False_Easting As Double = 500000.0R, False_Northing As Double, Scale As Double = 0.9996R
            If (PointUtm.Zone < 1) OrElse (PointUtm.Zone > 60) Then
                Error_Code = Error_Code Or UTM.ZONE_ERROR
            End If
            If (PointUtm.Hemisphere <> "S"c) AndAlso (PointUtm.Hemisphere <> "N"c) Then
                Error_Code = Error_Code Or UTM.HEMISPHERE_ERROR
            End If
            If (PointUtm.Coordonnees.X < MIN_EASTING) OrElse (PointUtm.Coordonnees.X > MAX_EASTING) Then
                Error_Code = Error_Code Or UTM.EASTING_ERROR
            End If
            If (PointUtm.Coordonnees.Y < MIN_NORTHING) OrElse (PointUtm.Coordonnees.Y > MAX_NORTHING) Then
                Error_Code = Error_Code Or UTM.NORTHING_ERROR
            End If
            If Error_Code = UTM.NO_ERROR Then
                If PointUtm.Zone >= 31 Then
                    Central_Meridian = ((6 * PointUtm.Zone - 183) * Deg_Rad)
                Else
                    Central_Meridian = ((6 * PointUtm.Zone + 177) * Deg_Rad)
                End If
                If PointUtm.Hemisphere = "S"c Then
                    False_Northing = 10000000
                End If
                Set_Transverse_Mercator_Parameters(UTM_a, UTM_f, Origin_Latitude, Central_Meridian, False_Easting, False_Northing, Scale)

                tm_error_code = Convert_Transverse_Mercator_To_Geodetic(PointUtm.Coordonnees, PointLatLon)

                If tm_error_code <> TRANMERC.NO_ERROR Then
                    If (tm_error_code And TRANMERC.EASTING_ERROR) <> 0 OrElse (tm_error_code And TRANMERC.LON_WARNING) <> 0 Then
                        Error_Code = Error_Code Or UTM.EASTING_ERROR
                    End If
                    If (tm_error_code And TRANMERC.NORTHING_ERROR) <> 0 Then
                        Error_Code = Error_Code Or UTM.NORTHING_ERROR
                    End If
                End If
                If (PointLatLon.Lat < MIN_LAT) OrElse (PointLatLon.Lat > MAX_LAT) Then
                    Error_Code = Error_Code Or UTM.NORTHING_ERROR
                End If
            End If
            Return Error_Code
        End Function
#End Region
    End Module
End Namespace