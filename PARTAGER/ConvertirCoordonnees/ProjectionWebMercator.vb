Namespace Coordonnees
    ''' <summary> Projection Pseudo Mercator ou Web Mercator valable sur de -85.0 à +85.0 de Latitude. Utilisé principalement sur les site Web de cartographie</summary>
    Module ProjectionWebMercator
#Region "Haut Niveau"
        ''' <summary> WGS84 en DD ou en Rad vers WebMercator en mètres. </summary>
        ''' <param name="PointLatLon"> le point LatLon à convertir </param>
        ''' <param name="Deg"> Flag indiquant si les coordonnées du point à convertir sont exprimées en DD </param>
        Friend Function ConvertWGS84ToWebMercator(PointLatLon As PointD, Optional Deg As Boolean = True) As PointD
            Try
                If Deg Then PointLatLon.Scale(Deg_Rad)
                Return New PointD(PointLatLon.Lon * RayonWGS84, Math.Log(Math.Tan(PointLatLon.Lat / 2 + Pi_4)) * RayonWGS84)
            Catch Ex As Exception
                AfficherErreur(Ex, "CD1E")
                Return PointD.Empty
            End Try
        End Function
        ''' <summary> WebMercator en mètres vers WGS84 en DD ou en Rad. </summary>
        '''<param name="PointGrille"> le point grille à convertir </param>
        '''<param name="Deg"> Flag indiquant si les coordonnées du point converti sont exprimées en DD ou en Rad </param>
        Friend Function ConvertWebMercatorToWGS84(PointGrille As PointD, Optional Deg As Boolean = True) As PointD
            Try
                Dim Ret As New PointD(PointGrille.X / RayonWGS84, (2.0R * Math.Atan(Math.Exp(PointGrille.Y / RayonWGS84)) - Pi_2))
                If Deg Then Ret.Scale(Rad_Deg)
                Return Ret
            Catch Ex As Exception
                AfficherErreur(Ex, "DB1C")
                Return PointD.Empty
            End Try
        End Function
#End Region
    End Module
End Namespace