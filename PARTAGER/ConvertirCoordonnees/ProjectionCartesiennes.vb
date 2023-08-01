Namespace Coordonnees
    '''<summary> fait la conversion géocentric d'un datum vers wgs84 ou inversement (3 paramètres et pas 7).  
    ''' Pour le changement d'un système vers un autre traduit du projet Geotrans écrit en C </summary>
    Friend Module ProjectionCartesienne
#Region "Haut niveau"
        ''' <summary> convertit une coordonnées exprimée en DD d'un datum une coordonnées exprimée en DD d'un autre datum</summary>
        ''' <param name="DatumDepart"> datum de départ. Ellipsoide obligatoire </param>
        ''' <param name="PointLatLon"> latitude-longitude de départ en dd</param>
        ''' <param name="DatumArrivee"> datum d'arrivée. Ellipsoide obligatoire </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        Friend Function ConvertDatumToDatum(DatumDepart As Datums, PointLatLon As PointD, DatumArrivee As Datums, Optional Deg As Boolean = True) As PointD
            Dim X_Depart, Y_Depart, Z_Depart, X_Arrivee, Y_Arrivee, Z_Arrivee As Double
            Try
                If Ellipsoide <> ParametresDatums(DatumDepart).Ellipsoide Then
                    SetParametresDatum(DatumDepart)
                End If
                'passe de degrés en radians si besoin
                If Deg Then PointLatLon.Scale(Deg_Rad)
                'transforme les DD du datum de départe en X,Y,Z du datum de départ
                Convert_Geographiques_To_Cartesiennes(PointLatLon, X_Depart, Y_Depart, Z_Depart)
                'tranforme les X, Y, Z du datum de depart en X, Y, Z du datum d'arrivée
                Cartesiennes_DatumDepart_To_DatumArrivee(DatumArrivee, X_Depart, Y_Depart, Z_Depart, X_Arrivee, Y_Arrivee, Z_Arrivee)
                'tranforme les X, Y, Z du datum d'arrivée en DD du datum d'arrivée
                Dim Ret As PointD = Convert_Cartesiennes_To_Geographiques(X_Arrivee, Y_Arrivee, Z_Arrivee)
                'passe de radians en degrés si besoin
                If Deg Then Ret.Scale(Rad_Deg)
                Return Ret
            Catch Ex As Exception
                AfficherErreur(Ex, "E4F2")
                Return PointD.Empty
            End Try
        End Function
#End Region
#Region "constantes"
        Private Const AD_C As Double = 1.0026R
        ''' <summary>Donne différentes erreur pouvant survenir lors de la transformation des coordonnées</summary>
        <Flags>
        Private Enum GEOCENT As Integer
            '''<summary>Flag OK</summary>"
            NO_ERROR = 0
            '''<summary>Flag erreur concernant la Latitude</summary>
            LAT_ERROR = 1
            '''<summary>Flag erreur concernant la Longitude</summary>
            LON_ERROR = 2
            '''<summary>Flag erreur concernant le demi axe de l'éllipsoïde</summary>
            A_ERROR = 4
            '''<summary>Flag erreur concernant l'applatissement de l'éllipsoïde</summary>
            INV_F_ERROR = 8
        End Enum
#End Region
#Region "Fonctions et procédures"
        '''<summary> Convertit coordonnées latitude, longitude en coordonnées cartésiennes X,Y,Z </summary>
        '''<param name="PointLatLon">Geodetic latitude in radians(input)</param>
        '''<param name="X">Calculated Geocentric X coordinate, in meters(output)</param>
        '''<param name="Y">Calculated Geocentric Y coordinate, in meters(output)</param>
        '''<param name="Z">Calculated Geocentric Z coordinate, in meters(output)</param>
        '''<returns>If any errors occur, the error code(s) are returned by the function, otherwise GEOCENT.NO_ERROR is returned.</returns>
        Private Function Convert_Geographiques_To_Cartesiennes(PointLatLon As PointD, ByRef X As Double, ByRef Y As Double, ByRef Z As Double) As GEOCENT
            Dim Geocent_a As Double = A
            Dim Geocent_e2 As Double = E2
            Dim Error_Code As GEOCENT
            Dim Rn As Double
            Dim Sin_Lat As Double
            Dim Sin2_Lat As Double
            Dim Cos_Lat As Double
            If (PointLatLon.Lat < -Pi_2) OrElse (PointLatLon.Lat > Pi_2) Then
                Error_Code = Error_Code Or GEOCENT.LAT_ERROR
            End If
            If (PointLatLon.Lon < -PI) OrElse (PointLatLon.Lon > (TwoPI)) Then
                Error_Code = Error_Code Or GEOCENT.LON_ERROR
            End If
            If Error_Code = GEOCENT.NO_ERROR Then
                If PointLatLon.Lon > PI Then
                    PointLatLon.Lon -= (TwoPI)
                End If
                Sin_Lat = Math.Sin(PointLatLon.Lat)
                Cos_Lat = Math.Cos(PointLatLon.Lat)
                Sin2_Lat = Sin_Lat * Sin_Lat
                Rn = Geocent_a / Math.Sqrt(1 - Geocent_e2 * Sin2_Lat)
                X = Rn * Cos_Lat * Math.Cos(PointLatLon.Lon)
                Y = Rn * Cos_Lat * Math.Sin(PointLatLon.Lon)
                Z = Rn * (1 - Geocent_e2) * Sin_Lat
            End If
            Return Error_Code
        End Function
        '''<summary> Convertit des coordonnées cartésiennes X,Y,Z en coordonnées Latitude, longitude </summary>
        '''<param name="X">Geocentric X coordinate, in meters.(input)</param>
        '''<param name="Y">Geocentric Y coordinate, in meters.(input)</param>
        '''<param name="Z">Geocentric Z coordinate, in meters.(input)</param>
        '''<returns> le point LatLon </returns>
        Private Function Convert_Cartesiennes_To_Geographiques(X As Double, Y As Double, Z As Double) As PointD
            Dim Geocent_a As Double = A
            Dim Geocent_e2 As Double = E2
            Dim Geocent_ep2 As Double = EP2
            Dim Geocent_b As Double = B
            Dim W, W2, T0, T1, S0, S1, Sin_B0, Sin3_B0, Cos_B0, Cos3_B0, Sin_p1, Cos_p1, Sum As Double
            Dim At_Pole As Boolean

            Dim PointLatLon As New PointD(0, 0)
            If X <> 0 Then
                PointLatLon.Lon = Math.Atan2(Y, X)
            Else
                If Y > 0 Then
                    PointLatLon.Lon = Pi_2
                ElseIf Y < 0 Then
                    PointLatLon.Lon = -Pi_2
                Else 'Y=0
                    At_Pole = True
                    PointLatLon.Lon = 0
                    If Z > 0 Then
                        PointLatLon.Lat = Pi_2
                    ElseIf Z < 0 Then
                        PointLatLon.Lat = -Pi_2
                    Else
                        PointLatLon.Lat = Pi_2
                        Return PointLatLon
                    End If
                End If
            End If
            W2 = X * X + Y * Y
            W = Math.Sqrt(W2)
            T0 = Z * AD_C
            S0 = Math.Sqrt(T0 * T0 + W2)
            Sin_B0 = T0 / S0
            Cos_B0 = W / S0
            Sin3_B0 = Sin_B0 * Sin_B0 * Sin_B0
            Cos3_B0 = Cos_B0 * Cos_B0 * Cos_B0
            T1 = Z + Geocent_b * Geocent_ep2 * Sin3_B0
            Sum = W - Geocent_a * Geocent_e2 * Cos3_B0 'Cos_B0 * Cos_B0 * Cos_B0
            S1 = Math.Sqrt(T1 * T1 + Sum * Sum)
            Sin_p1 = T1 / S1    'pas nécessaire
            Cos_p1 = Sum / S1   'pas nécessaire

            If At_Pole = False Then
                PointLatLon.Lat = Math.Atan(Sin_p1 / Cos_p1)    'equivalent à Math.Atan(T1/Sum)
            End If
            Return PointLatLon
        End Function
        '''<summary> Passe d'un ellipsoïde à l'autre en fonction des paramètres de transformation </summary>"
        '''<param name="X_Depart">X coordinate relative to the source datum (ouput)</param>
        '''<param name="Y_Depart">Y coordinate relative to the source datum (ouput)</param>
        '''<param name="Z_Depart">Z coordinate relative to the source datum (ouput)</param>
        '''<param name="X_Arrivee">X coordinate relative to WGS84 (intput)</param>
        '''<param name="Y_Arrivee">Y coordinate relative to WGS84 (intput)</param>
        '''<param name="Z_Arrivee">Z coordinate relative to WGS84 (intput)</param>
        ''' <remarks>en sortie les paramètre associés au datum ont changés et correspondent à ceux du datum d'arrivée</remarks>
        Private Sub Cartesiennes_DatumDepart_To_DatumArrivee(DatumArrivee As Datums, X_Depart As Double, Y_Depart As Double, Z_Depart As Double,
                                                             ByRef X_Arrivee As Double, ByRef Y_Arrivee As Double, ByRef Z_Arrivee As Double)
            If Ellipsoide = ParametresDatums(DatumArrivee).Ellipsoide Then
                'si l'ellipsoïde de départ = le l'ellipsoïde d'arrivée il n'y a pas de transformation à faire
                'Ex : UTM_ED50 to DD ED50
                X_Arrivee = X_Depart
                Y_Arrivee = Y_Depart
                Z_Arrivee = Z_Depart
            ElseIf ParametresDatums(DatumArrivee).Ellipsoide = Ellipsoides.WGS84 Then
                'si l'ellipsoïde d'arrivée est le WGS84 il faut ajouter les paramètres pour la transformation
                'Ex DD ED50 vers DD WGS84
                X_Arrivee = X_Depart + DX
                Y_Arrivee = Y_Depart + DY
                Z_Arrivee = Z_Depart + DZ
                'on met les paramètres datum arrivée à disposition
                SetParametresDatum(DatumArrivee)
            ElseIf Ellipsoide = Ellipsoides.WGS84 Then
                'si l'ellipsoïde de départ est le WGS84 il faut enlever les paramètres pour la transformation
                'Ex : DD WGS84 to DD ED50
                SetParametresDatum(DatumArrivee)
                X_Arrivee = X_Depart - DX
                Y_Arrivee = Y_Depart - DY
                Z_Arrivee = Z_Depart - DZ
            Else
                'sinon si il faut passer par l'ellipsoïde WGS84 qui est le système pivot
                'Ex : DD NTF to DD ED50
                'd'abord DD NTF to DD WGS84
                X_Arrivee = X_Depart + DX
                Y_Arrivee = Y_Depart + DY
                Z_Arrivee = Z_Depart + DZ
                'puis DD WGS84 to DD ED50
                SetParametresDatum(DatumArrivee)
                X_Arrivee -= DX
                Y_Arrivee -= DY
                Z_Arrivee -= DZ
            End If
        End Sub
#End Region
    End Module
End Namespace