Namespace Coordonnees
    ''' <summary> Projection LatitudeLongitude valable sur la terre entière. Utilisé principalement sur les ordinateurs
    ''' aussi appelée Pseudo Plate carrée. A la base ce n'est pas vraiement une projection </summary>
    Friend Module ProjectionLongitudeLatitude
#Region "Haut Niveau"
        ''' <summary> WGS84 en DD ou en Rad vers LatLon en mètres. </summary>
        ''' <param name="PointLatLon"> le point LatLon à convertir </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        Friend Function ConvertWGS84ToLatLon(PointLatLon As PointD, Optional Deg As Boolean = True) As PointD
            Try
                If Deg Then PointLatLon.Scale(Deg_Rad)
                Return New PointD(PointLatLon.Lon * RayonWGS84, PointLatLon.Lat * RayonWGS84)
            Catch Ex As Exception
                AfficherErreur(Ex, "A654")
                Return PointD.Empty
            End Try
        End Function
        ''' <summary> LatLon en mètres vers WGS84 en DD ou en Rad. </summary>
        '''<param name="PointGrille"> le point grille à convertir </param>
        '''<param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        Friend Function ConvertLatLonToWGS84(PointGrille As PointD, Optional Deg As Boolean = True) As PointD
            Try
                Dim Ret As New PointD(PointGrille.X / RayonWGS84, PointGrille.Y / RayonWGS84)
                If Deg Then Ret.Scale(Rad_Deg)
                Return Ret
            Catch Ex As Exception
                AfficherErreur(Ex, "EE86")
                Return PointD.Empty
            End Try
        End Function
#End Region
    End Module
End Namespace