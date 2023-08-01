Namespace Coordonnees
    ''' <summary> Summary description for ApproxSwissProj.  trouvée sur le site Swiss Topo </summary>
    Friend Module ProjectionSuisse
        Private Const DELTA_X_LV95 As Integer = 2_000_000
        Private Const DELTA_Y_LV95 As Integer = 1_000_000
        Private Const DELTA_X_LV03 As Integer = 600_000
        Private Const DELTA_Y_LV03 As Integer = 200_000
        Private Const SecToDeg As Double = 100.0R / 36.0R
        Private Const DegToSec As Double = 36.0R / 100.0R
#Region "Haut niveau"
        ''' <summary> Suisse en mètres vers CH1903 en DD ou en Rad. Formules approchées  </summary>
        '''<param name="PointGrille"> le point grille à convertir </param>
        '''<param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        ''' <param name="Datum"> Datum de la projection suisse </param>
        ''' <returns> le point converti </returns>
        Friend Function ConvertSuisseToCH1903(PointGrille As PointD, Datum As Datums, Optional Deg As Boolean = True) As PointD
            If PARAM_DATUMS.Ellipsoide <> ParametresDatums(Datum).Ellipsoide Then
                SetParametresDatum(Datum) 'datum de départ ou Nom
            End If
            If Datum <> Datums.Grille_Suisse_LV03 Then
                'on ramène déjà les coordonnées métrique en LV03
                LV95ToLV03(PointGrille)
            End If
            Return ConvertLV03ToCH1903(PointGrille, Deg)
        End Function
        ''' <summary> CH1903 en DD ou en Rad vers LV03/LV95 en mètres. Formules approchées </summary>
        ''' <param name="PointLatLon"> le point LatLon à convertir </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        ''' <param name="Datum"> projection suisse du point à convertir </param>
        Friend Function ConvertCH1903ToSuisse(PointLatLon As PointD, Datum As Datums, Optional Deg As Boolean = True) As PointD
            Dim Ret As PointD = ConvertCH1903ToLV03(PointLatLon, Deg)
            If Not Ret.IsEmpty And Datum = Datums.Grille_Suisse_LV95 Then
                Ret.Add(New SizeD(DELTA_X_LV95, DELTA_Y_LV95))
            End If
            Return Ret
        End Function
        ''' <summary> Suisse en mètres vers CH1903 en DD ou en Rad. Formules approchées  </summary>
        '''<param name="PointGrille"> le point grille à convertir </param>
        '''<param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        ''' <param name="Datum"> Datum de la projection suisse  </param>
        ''' <returns> le point converti </returns>
        Friend Function ConvertSuisseToWGS84(PointGrille As PointD, Datum As Datums, Optional Deg As Boolean = True) As PointD
            If PARAM_DATUMS.Ellipsoide <> ParametresDatums(Datum).Ellipsoide Then
                SetParametresDatum(Datum) 'datum de départ ou Nom
            End If
            If Datum <> Datums.Grille_Suisse_LV03 Then
                'on ramène déjà les coordonnées métrique en LV03 pour LV95 Suisse seul
                LV95ToLV03(PointGrille)
            End If
            Return ConvertLV03ToWGS84(PointGrille, Deg)
        End Function
        ''' <summary> CH1903 en DD ou en Rad vers LV03/LV95 en mètres. Formules approchées </summary>
        ''' <param name="PointLatLon"> le point LatLon à convertir </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        ''' <param name="Datum"> projection suisse du point à convertir </param>
        Friend Function ConvertWGS84ToSuisse(PointLatLon As PointD, Datum As Datums, Optional Deg As Boolean = True) As PointD
            Dim Ret As PointD = ConvertWGS84ToLV03(PointLatLon, Deg)
            If Not Ret.IsEmpty And Datum = Datums.Grille_Suisse_LV95 Then
                Ret.Add(New SizeD(DELTA_X_LV95, DELTA_Y_LV95))
            End If
            Return Ret
        End Function
        ''' <summary> Convertit une coordonnées de départ exprimée en LV03 vers une coordonnées exprimée en LV95 </summary>
        ''' <param name="PointGrille"> PointGrille à convertir </param>
        ''' <remarks> Spécifique pour la grille Suisse </remarks>
        Friend Sub LV03ToLV95(ByRef PointGrille As PointD)
            PointGrille.Add(New SizeD(DELTA_X_LV95, DELTA_Y_LV95))
        End Sub
        ''' <summary> Convertit une coordonnées de départ exprimée en LV95 vers une coordonnées exprimée en LV03 </summary>
        ''' <param name="PointGrille"> PointGrille à convertir </param>
        ''' <remarks> Spécifique pour la grille Suisse </remarks>
        Friend Sub LV95ToLV03(ByRef PointGrille As PointD)
            If PointGrille.X > DELTA_X_LV95 Then PointGrille.X -= DELTA_X_LV95
            If PointGrille.Y > DELTA_Y_LV95 Then PointGrille.Y -= DELTA_Y_LV95
        End Sub
#End Region
#Region "Moyen niveau"
        ''' <summary> LV03 en mètres vers CH1903 en DD ou en Rad. Formules approchées </summary>
        '''<param name="PointGrille"> le point grille à convertir </param>
        '''<param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        Friend Function ConvertLV03ToCH1903(PointGrille As PointD, Optional Deg As Boolean = True) As PointD
            Try
                'initialise les données communes x et y
                PointGrille.Add(New SizeD(-DELTA_X_LV03, -DELTA_Y_LV03))
                PointGrille.Scale(1 / 1_000_000.0R)
                Dim X_aux As Double = PointGrille.X
                Dim Y_aux As Double = PointGrille.Y
                Dim X_aux2 As Double = X_aux * X_aux, X_aux3 As Double = X_aux2 * X_aux, X_aux4 As Double = X_aux3 * X_aux, X_aux5 As Double = X_aux4 * X_aux
                Dim Y_aux2 As Double = Y_aux * Y_aux, Y_aux3 As Double = Y_aux2 * Y_aux, Y_aux4 As Double = Y_aux3 * Y_aux
                'coefs longitude
                Dim Aa_1 As Double = 4.72973056 + 0.7925714 * Y_aux + 0.132812 * Y_aux2 + 0.0255 * Y_aux3 + 0.0048 * Y_aux4
                Dim Aa_3 As Double = -0.04427 - 0.0255 * Y_aux - 0.0096 * Y_aux2
                Dim Aa_5 As Double = 0.00096
                'coefs latitude
                Dim Pp_0 As Double = +3.23864877 * Y_aux - 0.0025486 * Y_aux2 - 0.013245 * Y_aux3 + 0.000048 * Y_aux4
                Dim Pp_2 As Double = -0.27135379 - 0.0450442 * Y_aux - 0.007553 * Y_aux2 - 0.00146 * Y_aux3
                Dim pp_4 As Double = 0.002442 + 0.00132 * Y_aux
                ' Process lon lat
                Dim Ret As New PointD(2.67825 + Aa_1 * X_aux + Aa_3 * X_aux3 + Aa_5 * X_aux5,
                                      16.902866 + Pp_0 + Pp_2 * X_aux2 + pp_4 * X_aux4)
                ' Unit 10000" to 1 " and converts seconds to degrees (dec)
                Ret.Scale(SecToDeg)
                If Not Deg Then Ret.Scale(Deg_Rad)
                Return Ret
            Catch Ex As Exception
                AfficherErreur(Ex, "8D5C")
                Return PointD.Empty
            End Try
        End Function
        ''' <summary> LV03 en mètres vers WGS84 en DD ou en Rad. Formules approchées </summary>
        '''<param name="PointGrille"> le point grille à convertir </param>
        '''<param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        Friend Function ConvertLV03ToWGS84(PointGrille As PointD, Optional Deg As Boolean = True) As PointD
            Try
                ' Converts militar to civil and to unit = 1000km
                PointGrille.Add(New SizeD(-DELTA_X_LV03, -DELTA_Y_LV03))
                PointGrille.Scale(1 / 1_000_000.0R)
                Dim X_aux As Double = PointGrille.X
                Dim Y_aux As Double = PointGrille.Y
                Dim X_aux2 As Double = X_aux * X_aux, X_aux3 As Double = X_aux2 * X_aux
                Dim Y_aux2 As Double = Y_aux * Y_aux, Y_aux3 As Double = Y_aux2 * Y_aux
                ' Process lon Lat
                Dim Ret As New PointD(2.6779094 + 4.728982 * X_aux + 0.791484 * X_aux * Y_aux + 0.1306 * X_aux * Y_aux2 - 0.0436 * X_aux3,
                                      16.902389199999998 + 3.238272 * Y_aux - 0.270978 * X_aux2 - 0.002528 * Y_aux2 - 0.0447 * X_aux2 * Y_aux - 0.014 * Y_aux3)
                ' Unit 10000" to 1 " and converts seconds to degrees (dec)
                Ret.Scale(SecToDeg)
                If Not Deg Then Ret.Scale(Deg_Rad)
                Return Ret
            Catch Ex As Exception
                AfficherErreur(Ex, "19F5")
                Return PointD.Empty
            End Try
        End Function
        ''' <summary> CH1903 en DD ou en Rad vers LV03 en mètres. Formules approchées </summary>
        ''' <param name="PointLatLon"> le point LatLon à convertir </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        Friend Function ConvertCH1903ToLV03(PointLatLon As PointD, Optional Deg As Boolean = True) As PointD
            Try
                'initialise les données communes lat et lon
                If Not Deg Then PointLatLon.Scale(Rad_Deg)
                PointLatLon.Scale(DegToSec)
                Dim Lon As Double = PointLatLon.Lon - 2.67825
                Dim Lat As Double = PointLatLon.Lat - 16.902866
                Dim Lon2 As Double = Lon * Lon, Lon3 As Double = Lon2 * Lon, Lon4 As Double = Lon3 * Lon, Lon5 As Double = Lon4 * Lon
                Dim Lat2 As Double = Lat * Lat, Lat3 As Double = Lat2 * Lat, Lat4 As Double = Lat3 * Lat, Lat5 As Double = Lat4 * Lat
                'calcul coef x
                Dim X1 As Double = 0.2114285339 - 0.010939608 * Lat - 0.000002658 * Lat2 - 0.00000853 * Lat3
                Dim X3 As Double = -0.0000442327 + 0.000004291 * Lat - 0.000000309 * Lat2
                Dim X5 As Double = 0.0000000197
                'calcul coef y
                Dim Y0 As Double = 0.3087707463 * Lat + 0.000075028 * Lat2 + 0.000120435 * Lat3 + 0.00000007 * Lat5
                Dim Y2 As Double = 0.0037454089 - 0.0001937927 * Lat + 0.00000434 * Lat2 - 0.000000376 * Lat3
                Dim Y4 As Double = -0.0000007346 + 0.0000001444 * Lat
                'retour du point transformé
                Dim Ret As New PointD(X1 * Lon + X3 * Lon3 + X5 * Lon5,
                                               Y0 + Y2 * Lon2 + Y4 * Lon4)
                Ret.Scale(1_000_000.0R)
                Ret.Add(New SizeD(DELTA_X_LV03, DELTA_Y_LV03))
                Return Ret
            Catch Ex As Exception
                AfficherErreur(Ex, "466F")
                Return PointD.Empty
            End Try
        End Function
        ''' <summary> WGS84 en DD ou en Rad vers LV03 en mètres. Formules approchées </summary>
        ''' <param name="PointLatLon"> le point LatLon à convertir </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        Friend Function ConvertWGS84ToLV03(PointLatLon As PointD, Optional Deg As Boolean = True) As PointD
            Try
                'initialise les données communes lat et lon
                If Not Deg Then PointLatLon.Scale(Rad_Deg)
                PointLatLon.Scale(DegToSec)
                Dim Lon As Double = PointLatLon.Lon - 2.67825
                Dim Lat As Double = PointLatLon.Lat - 16.902866
                Dim Lon2 As Double = Lon * Lon, Lon3 As Double = Lon2 * Lon
                Dim Lat2 = Lat * Lat, Lat3 As Double = Lat2 * Lat
                'calcul coef X
                Return New PointD(600072.37 + 211455.93 * Lon - 10938.51 * Lon * Lat - 0.36 * Lon * Lat2 - 44.54 * Lon3,
                                  200147.07 + 308807.95 * Lat + 3745.25 * Lon2 + 76.63 * Lat2 - 194.56 * Lon2 * Lat + 119.79 * Lat3)
            Catch Ex As Exception
                AfficherErreur(Ex, "7782")
                Return PointD.Empty
            End Try
        End Function
#End Region
    End Module
End Namespace