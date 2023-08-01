Partial Friend Class Carte
#Region "Fichiers de géoréférencement"
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à FCGP </summary>
    Private Function EcrireFichierGeoref() As Boolean
        EcrireFichierGeoref = False
        Try
            With SystemCartographique
                Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
                S.AppendLine($"""Carte"" Nom : { NomComplet & "." & Format.ToString }, Site : { .LibelleSiteCarto}" &
                             If(.SiteCarto = SitesCartographiques.DomTom, "-" & .LibelleDomtom, ""))
                S.AppendLine($"""Carte"" Echelle : { .Niveau.Clef }, Echelle Impression : { .Niveau.ImpressionClef }, Coordonnées : { .Projection.Libelle}" &
                             If(.Projection.Libelle.Chars(0) = "U"c, $" { .HemisphereUtmReferencement } { .ZoneUtmReferencement:00}", ""))
                S.AppendLine($"""Carte"" Retail : { If(.IsInterpol = True, "Oui", "Non") }, Grille : { If(GrilleIsAffiche = True, "Oui", "Non") }" &
                             $", DPI_X : { DblToStr(DPIImpressionX, "000.00") }, DPI_Y : { DblToStr(DPIImpressionY, "000.00")}")
                'Pour chaque coin de la carte
                For Cpt = 0 To 3
                    'écrit les coordonnées capturées sur le site internet
                    S.AppendLine($"""Pt{ Cpt:0}"" Coordonnées capturées : { _Coins(Cpt).CoordonneesCaptureEcran}")
                    'écrit la longitude et la latitude en degrés décimaux avec le signe plus ou moins
                    S.AppendLine($"""Pt{ Cpt:0}"" " &
                                 If(_Coins(Cpt).Longitude >= 0, "Longitude : +", "Longitude : ") & DblToStr(_Coins(Cpt).Longitude, "000.00000000") & ", " &
                                 If(_Coins(Cpt).Latitude >= 0, "Latitude : +", "Latitude : ") & DblToStr(_Coins(Cpt).Latitude, "00.00000000"))
                    'écrit les X et Y en pixel
                    S.AppendLine($"""Pt{ Cpt:0}"" X image : { _Coins(Cpt).X_Pixel:000000}, Y image : { _Coins(Cpt).Y_Pixel:000000}")
                Next
                'écrit le nb de zones UTM de la carte
                S.AppendLine("""UTM WGS84"" NB Zones : " & NbZonesUTM)
                'Pour chaque coin de la carte et pour chaque zone UTM
                For Boucle = 0 To NbZonesUTM - 1
                    For Cpt = 0 To 3
                        'écrit les coordonnées UTM, l'hémisphère et la zone
                        S.AppendLine($"""Pt{ Cpt:0}"" " &
                                     If(_Coins(Cpt).UTMs(Boucle).X < 0, "X : ", "X : +") & DblToStr(_Coins(Cpt).UTMs(Boucle).X, "00000000.0") & ", " &
                                     If(_Coins(Cpt).UTMs(Boucle).Y < 0, "Y : ", "Y : +") & DblToStr(_Coins(Cpt).UTMs(Boucle).Y, "00000000.0") & ", " &
                                     $"Hemisphère : { _Coins(0).Hemisphere_UTM }, Zone : { (_Coins(0).NumZone_UTM + Boucle):00}")
                    Next
                Next

                File.WriteAllText(CheminCarte & "\" & NomComplet & ".georef", S.ToString(), Encoding_FCGP)
            End With
            EcrireFichierGeoref = True
        Catch Ex As Exception
            AfficherErreur(Ex, "E6A4")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à CompeGPSLand en système WGS84 (latitude et longitude)
    ''' pour 2 coins  de la carte</summary>
    Private Function EcrireCompeLandMercator() As Boolean
        EcrireCompeLandMercator = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("CompeGPS MAP File" & CrLf & "<Header>" & CrLf & "Version=2" & CrLf & "VerCompeGPS=7.7.2" & CrLf &
                         "Projection=2,Mercator," & CrLf & "Coordinates=1" & CrLf & "Datum=WGS 84" & CrLf & "</Header>")

            S.AppendLine("<Map>" & CrLf & "Bitmap=" & NomComplet & "." & Format.ToString & CrLf & "BitsPerPixel=0" & CrLf &
                     "BitmapWidth=" & _Coins(2).X_Pixel.ToString() & CrLf & "BitmapHeight=" & _Coins(2).Y_Pixel.ToString() & CrLf &
                     "Type=10" & CrLf & "</Map>" & CrLf & "<Calibration>")
            For Cpt = 0 To 2 Step 2
                S.AppendLine("P" & Cpt \ 2 & "=" & _Coins(Cpt).X_Pixel.ToString() & "," & _Coins(Cpt).Y_Pixel.ToString() & ",A," &
                             DblToStr(_Coins(Cpt).Longitude, "0.00000000") & "," & DblToStr(_Coins(Cpt).Latitude, "0.00000000"))
            Next
            S.AppendLine("</Calibration>" & CrLf & "<MainPolygonBitmap>")
            For Cpt = 0 To 3
                S.AppendLine("M" & Cpt & "=" & DblToStr(_Coins(Cpt).X_Pixel, "0.0") & "," & DblToStr(_Coins(Cpt).Y_Pixel, "0.0"))
            Next
            S.Append("</MainPolygonBitmap>")

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".Imp", S.ToString, Encoding_FCGP)
            EcrireCompeLandMercator = True
        Catch Ex As Exception
            AfficherErreur(Ex, "W2T9")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à CompeGPSLand en MN03 (projection des cartes suisses) associé à CH1903 (système Suisse)
    ''' pour 2 coins  de la carte</summary>
    Private Function EcrireCompeLandLV03() As Boolean
        EcrireCompeLandLV03 = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("CompeGPS MAP File" & CrLf & "<Header>" & CrLf & "Version=2" & CrLf & "VerCompeGPS=7.7.2" & CrLf &
                         "Projection=14,Swiss Grid," & CrLf & "Coordinates=1" & CrLf & "Datum=CH-1903" & CrLf & "</Header>")

            S.AppendLine("<Map>" & CrLf & "Bitmap=" & NomComplet & "." & Format.ToString & CrLf & "BitsPerPixel=0" & CrLf &
                         "BitmapWidth=" & _Coins(2).X_Pixel.ToString() & CrLf & "BitmapHeight=" & _Coins(2).Y_Pixel.ToString() & CrLf &
                         "Type=10" & CrLf & "</Map>" & CrLf & "<Calibration>")
            For Cpt = 0 To 2 Step 2
                Dim PGR = _Coins(Cpt).Grille
                LV95ToLV03(PGR)
                Dim PDD As PointD = ConvertLV03ToCH1903(PGR, True)
                S.AppendLine("P" & Cpt \ 2 & "=" & _Coins(Cpt).X_Pixel.ToString() & "," & _Coins(Cpt).Y_Pixel.ToString() & ",A," &
                             DblToStr(PDD.Lon, "0.00000000") & "," & DblToStr(PDD.Lat, "0.00000000"))
            Next
            S.AppendLine("</Calibration>" & CrLf & "<MainPolygonBitmap>")
            For Cpt = 0 To 3
                S.AppendLine("M" & Cpt & "=" & DblToStr(_Coins(Cpt).X_Pixel, "0.0") & "," & DblToStr(_Coins(Cpt).Y_Pixel, "0.0"))
            Next
            S.Append("</MainPolygonBitmap>")

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".Imp", S.ToString, Encoding_FCGP)
            EcrireCompeLandLV03 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "W2K7")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à CompeGPSLand en système WGS84 (latitude et longitude)
    ''' pour 2 coins  de la carte</summary>
    Private Function EcrireCompeLandWGS84() As Boolean
        EcrireCompeLandWGS84 = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("CompeGPS MAP File" & CrLf & "<Header>" & CrLf & "Version=2" & CrLf & "VerCompeGPS=7.7.2" & CrLf &
                         "Projection=1,Lat/Long," & CrLf & "Coordinates=1" & CrLf & "Datum=WGS 84" & CrLf & "</Header>")
            S.AppendLine("<Map>" & CrLf & "Bitmap=" & NomComplet & "." & Format.ToString & CrLf & "BitsPerPixel=0" & CrLf &
                         "BitmapWidth=" & _Coins(2).X_Pixel.ToString() & CrLf & "BitmapHeight=" & _Coins(2).Y_Pixel.ToString() & CrLf &
                         "Type=10" & CrLf & "</Map>" & CrLf & "<Calibration>")
            For Cpt = 0 To 2 Step 2
                S.AppendLine("P" & Cpt \ 2 & "=" & _Coins(Cpt).X_Pixel.ToString() & "," & _Coins(Cpt).Y_Pixel.ToString() & ",A," &
                             DblToStr(_Coins(Cpt).Longitude, "0.00000000") & "," & DblToStr(_Coins(Cpt).Latitude, "0.00000000"))
            Next
            S.AppendLine("</Calibration>" & CrLf & "<MainPolygonBitmap>")
            For Cpt = 0 To 3
                S.AppendLine("M" & Cpt & "=" & DblToStr(_Coins(Cpt).X_Pixel, "0.0") & "," & DblToStr(_Coins(Cpt).Y_Pixel, "0.0"))
            Next
            S.Append("</MainPolygonBitmap>")

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".Imp", S.ToString, Encoding_FCGP)
            EcrireCompeLandWGS84 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "B4C0")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en MN03 (projection des cartes suisses) associé à CH1903 (système Suisse)
    ''' pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
    '''(Longitude, Latidude) (Ximg, Yimg) Label "PTn",. Il n'y a pas de virgule pour le dernier coin </summary>
    ''' <remarks>Si le numéro d'écran est vide, il s'agit d'une carte et non une capture écran</remarks>
    Private Function EcrireMapInfoLV03() As Boolean
        EcrireMapInfoLV03 = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("!table" & CrLf & "!version 300" & CrLf & "!charset WindowsLatin1" & CrLf & CrLf & "Definition Table" & CrLf &
                         "  File """ & NomComplet & "." & Format.ToString & """" & CrLf & "  Type ""RASTER""")
            'Il n'y a pas de virgule pour le dernier coin 
            For Cpt = 0 To 3
                Dim PGR As PointD = Coins(Cpt).Grille
                LV95ToLV03(PGR)
                S.AppendLine(If(_Coins(Cpt).X_Grid >= 0, "  (+", "  (") & DblToStr(PGR.X, "00000000.0") &
                             If(_Coins(Cpt).Y_Grid >= 0, ",+", ",") & DblToStr(PGR.Y, "00000000.0") & ") (" &
                             _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ") Label ""Pt" & Cpt.ToString("0") & """" & If(Cpt <> 3, ",", ""))
            Next
            'projection utm associée à l'ellipsoïde wgs84. elle change avec la zone UTM
            S.Append("  CoordSys Earth Projection 25, 1003, ""m"", 7.4395833333, 46.9524055555, 600000, 200000" & CrLf &
                     "  Units ""m""" & CrLf + "  RasterStyle 7 0")

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".Tab", S.ToString, Encoding_FCGP)
            EcrireMapInfoLV03 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "Q3I6")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en UTM Mercator associé à wgs84
    ''' pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
    '''(Longitude, Latidude) (Ximg, Yimg) Label "PTn",. Il n'y a pas de virgule pour le dernier coin </summary>
    ''' <remarks>Si le numéro d'écran est vide, il s'agit d'une carte et non une capture écran</remarks>
    Private Function EcrireMapInfoUTM() As Boolean
        EcrireMapInfoUTM = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("!table" & CrLf & "!version 300" & CrLf & "!charset WindowsLatin1" & CrLf & CrLf & "Definition Table" & CrLf &
                         "  File """ & NomComplet & "." & Format.ToString & """" & CrLf & "  Type ""RASTER""")
            'Il n'y a pas de virgule pour le dernier coin 
            For Cpt = 0 To 3
                S.AppendLine(If(_Coins(Cpt).UTMs(0).X >= 0, "  (+", "  (") & DblToStr(_Coins(Cpt).UTMs(0).X, "00000000.0") &
                             If(_Coins(Cpt).UTMs(0).Y >= 0, ",+", ",") & DblToStr(_Coins(Cpt).UTMs(0).Y, "00000000.0") & ") (" &
                             _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ") Label ""Pt" & Cpt.ToString("0") & """" & If(Cpt <> 3, ",", ""))
            Next
            'projection utm associée à l'ellipsoïde wgs84. elle change avec la zone UTM
            S.Append("  CoordSys Earth Projection 8, 104, ""m"", " & (Coins(0).NumZone_UTM * 6 - 183).ToString("0") &
                     ", 0, 0.9996, 500000, 0" & CrLf & "  Units ""m""" & CrLf + "  RasterStyle 7 0")

            File.WriteAllText(CheminCarte & "\" & NomComplet & "_UTM.Tab", S.ToString, Encoding_FCGP)
            EcrireMapInfoUTM = True
        Catch Ex As Exception
            AfficherErreur(Ex, "T5J2")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en longitude, latitude WGS84 pour Carte DT ou carte interpolée SM ou GF
    ''' pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
    '''(Longitude, Latidude) (Ximg, Yimg) Label "PTn",. 6 caractères pour les X, Y à compléter avec des 0 si besoin est </summary>
    Private Function EcrireMapInfoWGS84() As Boolean
        EcrireMapInfoWGS84 = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("!table" & CrLf & "!version 300" & CrLf & "!charset WindowsLatin1" & CrLf & CrLf & "Definition Table" & CrLf &
                         "  File """ & NomComplet & "." & Format.ToString & """" & CrLf & "  Type ""RASTER""")
            'Il n'y a pas de virgule pour le dernier coin 
            For Cpt = 0 To 3
                S.AppendLine(If(_Coins(Cpt).Longitude >= 0, "  (+", "  (") & DblToStr(_Coins(Cpt).Longitude, "000.0000000") &
                             If(_Coins(Cpt).Latitude >= 0, ",+", ",") & DblToStr(_Coins(Cpt).Latitude, "00.0000000") &
                             ") (" & _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ") Label ""Pt" & Cpt.ToString("0") & """" & If(Cpt <> 3, ",", ""))
            Next
            'la fin est toujours la même : projection lat/lon et ellipsoïde wgs84
            S.Append("  CoordSys Earth Projection 1, 104" & CrLf & "  Units ""Degree""" & CrLf & "  RasterStyle 7 0")

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".Tab", S.ToString, Encoding_FCGP)
            EcrireMapInfoWGS84 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "F8W1")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en longitude, latitude WGS84 pour Carte DT ou carte interpolée SM ou GF
    ''' pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
    '''(Longitude, Latidude) (Ximg, Yimg) Label "PTn",. 6 caractères pour les X, Y à compléter avec des 0 si besoin est </summary>
    Private Function EcrireMapInfoMercator() As Boolean
        EcrireMapInfoMercator = False
        Try
            Dim Cpt As Integer, S As New StringBuilder   'pour écrire le fichier des structures georef
            'l'entête est  toujours le même
            S.AppendLine("!table" & CrLf & "!version 300" & CrLf & "!charset WindowsLatin1" & CrLf & CrLf & "Definition Table" & CrLf &
                         "  File """ & NomComplet & "." & Format.ToString & """" & CrLf & "  Type ""RASTER""")
            'Il n'y a pas de virgule pour le dernier coin 
            For Cpt = 0 To 3
                S.AppendLine(If(_Coins(Cpt).Longitude >= 0, "  (+", "  (") & DblToStr(_Coins(Cpt).Longitude, "000.0000000") &
                             If(_Coins(Cpt).Latitude >= 0, ",+", ",") & DblToStr(_Coins(Cpt).Latitude, "00.0000000") &
                             ") (" & _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ") Label ""Pt" & Cpt.ToString("0") & """" & If(Cpt <> 3, ",", ""))
            Next
            'la fin est toujours la même : projection mercator et ellipsoïde wgs84
            S.Append("  CoordSys Earth Projection 10, 104, ""m"", 0" & CrLf & "  Units ""Degree""" & CrLf & "  RasterStyle 7 0")

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".Tab", S.ToString, Encoding_FCGP)
            EcrireMapInfoMercator = True
        Catch Ex As Exception
            AfficherErreur(Ex, "A9P8")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à OziExploreur en système Mercator (grille ) 
    ''' valable pour le site SM ou GF interpolé ou DT. Voir la description du format sur le net</summary>
    Private Function EcrireOziExploreurMercator() As Boolean
        EcrireOziExploreurMercator = False
        Try
            Dim S As New StringBuilder   'pour écrire le fichier
            'l'entête est  toujours le même
            S.AppendLine("OziExplorer Map Data File Version 2.2" & CrLf & NomComplet & "." & Format.ToString & CrLf &
                         CheminCarte & "\" & NomComplet & "." & Format.ToString & CrLf & "1 ,Map Code," & CrLf &
                         "WGS 84,WGS 84,   0.0000,   0.0000,WGS 84" & CrLf & "Reserved 1" & CrLf & "Reserved 2" & CrLf &
                         "Magnetic Variation,,,E" & CrLf & "Map Projection,Mercator,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No")

            Dim NumPoint As Integer = 0
            For Cpt = 0 To 2 Step 2
                NumPoint += 1
                Dim Longitude = Math.Abs(_Coins(Cpt).Longitude)
                Dim Latitude = Math.Abs(_Coins(Cpt).Latitude)
                Dim IsPositifLatitude = _Coins(Cpt).Latitude >= 0
                Dim IsPositifLongitude = _Coins(Cpt).Longitude >= 0
                S.AppendLine("Point" & NumPoint.ToString("00") & ",xy," & _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ",in, deg," & Math.Floor(Latitude).ToString("00") & "," &
                             DblToStr((Latitude - Math.Floor(Latitude)) * 60, "00.000000") & "," &
                             If(IsPositifLatitude, "N", "S") & "," & Math.Floor(Longitude).ToString("000") & "," &
                             DblToStr((Longitude - Math.Floor(Longitude)) * 60, "00.000000") & "," &
                             If(IsPositifLongitude, "E", "W") & ",grid,,,,N")
            Next

            For Cpt = 3 To 9
                S.AppendLine("Point" & Cpt.ToString("00") & ",xy,,,in,deg,,,N,,,,grid,,,,N")
            Next
            For Cpt = 10 To 30
                S.AppendLine("Point" & Cpt.ToString("00") & ",xy,,,in,deg,,,,,,,grid,,,,")
            Next

            S.AppendLine("Projection Setup,,,,,,,,,," & CrLf &
                         "Map Feature = MF ; Map Comment = MC     These follow if they exist" & CrLf &
                         "Track File = TF      These follow if they exist" & CrLf &
                         "MM0,Yes" & CrLf & "MMPNUM,4")

            For Cpt = 0 To 3
                S.AppendLine("MMPXY," & (Cpt + 1).ToString("0") & "," & _Coins(Cpt).X_Pixel.ToString("000000") & "," &
                             _Coins(Cpt).Y_Pixel.ToString("000000"))
            Next
            For Cpt = 0 To 3
                S.AppendLine("MMPLL," & (Cpt + 1).ToString("0") & "," & If(_Coins(Cpt).Longitude >= 0, "+", "") &
                             DblToStr(_Coins(Cpt).Longitude, "000.0000000") & If(_Coins(Cpt).Latitude >= 0, ",+", ",") &
                             DblToStr(_Coins(Cpt).Latitude, "00.0000000"))
            Next

            S.AppendLine("MM1B," & DblToStr(Echelle_M_PixelX, "0.000000") & CrLf &
                         "LL Grid Setup" & CrLf & "LLGRID,No,No Grid,Yes,255,16711680,0,No Labels,0,16777215,7,1,Yes,x" & CrLf &
                         "Other Grid Setup" & CrLf & "GRGRID,Yes," & SystemCartographique.Niveau.PasGrilleTexte &
                         ",Yes,16711935,255," & SystemCartographique.Niveau.PasGrilleTexte & ",0,16777215,8,1,Yes,No,No,x" & CrLf &
                         "MOP,Map Open Position,0,0")

            S.Append("IWH,Map Image Width/Height," & _Coins(2).X_Pixel.ToString("000000") & "," & _Coins(2).Y_Pixel.ToString("000000"))

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".map", S.ToString, Encoding_FCGP)
            EcrireOziExploreurMercator = True
        Catch Ex As Exception
            AfficherErreur(Ex, "A4T8")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à OziExploreur pour SM non interpolé en grille Suisse associée au système CH1903 
    ''' Voir la description du format sur le net</summary>
    Private Function EcrireOziExploreurLV03() As Boolean
        EcrireOziExploreurLV03 = False
        Try
            Dim S As New StringBuilder   'pour écrire le fichier
            'l'entête est  toujours le même
            S.AppendLine("OziExplorer Map Data File Version 2.2" & CrLf & NomComplet & "." & Format.ToString & CrLf &
                         CheminCarte & "\" & NomComplet & "." & Format.ToString & CrLf & "1 ,Map Code," & CrLf &
                         "CH-1903,WGS 84,   0.0000,   0.0000,WGS 84" & CrLf & "Reserved 1" & CrLf & "Reserved 2" & CrLf &
                         "Magnetic Variation,,,E" & CrLf & "Map Projection,(SUI) Swiss Grid,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No")

            Dim NumPoint As Integer = 0
            For Cpt = 0 To 2 Step 2
                NumPoint += 1
                Dim PGR = _Coins(Cpt).Grille
                LV95ToLV03(PGR)
                S.AppendLine("Point" & NumPoint.ToString("00") & ",xy," & _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ",in, deg,,,N,,,E,grid,," & If(PGR.X >= 0, "+", "") & DblToStr(PGR.X, "00000000.0") &
                             If(PGR.Y >= 0, ",+", ",") & DblToStr(PGR.Y, "00000000.0") & ",N")
            Next

            For Cpt = 3 To 30
                S.AppendLine("Point" & Cpt.ToString("00") & ",xy,,,in,deg,,,,,,,grid,,,,")
            Next

            S.AppendLine("Projection Setup,,,,,,,,,," & CrLf &
                         "Map Feature = MF ; Map Comment = MC     These follow if they exist" & CrLf &
                         "Track File = TF      These follow if they exist" & CrLf &
                         "MM0,Yes" & CrLf & "MMPNUM,4")

            For Cpt = 0 To 3
                S.AppendLine("MMPXY," & (Cpt + 1).ToString("0") & "," & Coins(Cpt).X_Pixel.ToString("000000") & "," &
                      Coins(Cpt).Y_Pixel.ToString("000000"))
            Next

            For Cpt = 0 To 3
                Dim PGR = Coins(Cpt).Grille
                LV95ToLV03(PGR)
                Dim PDD = ConvertLV03ToCH1903(PGR, True)
                S.AppendLine("MMPLL," & (Cpt + 1).ToString("0") & "," & If(PDD.Lon >= 0, "+", "") &
                 DblToStr(PDD.Lon, "000.0000000") & If(PDD.Lat >= 0, ",+", ",") &
                 DblToStr(PDD.Lat, "00.0000000"))
            Next

            S.Append("MM1B," & DblToStr(Echelle_M_PixelX, "0.000000") & CrLf &
                     "LL Grid Setup" & CrLf & "LLGRID,No,No Grid,Yes,255,16711680,0,No Labels,0,16777215,7,1,Yes,x" & CrLf &
                     "Other Grid Setup" & CrLf & "GRGRID,Yes," & SystemCartographique.Niveau.PasGrilleTexte & ",Yes,16711935,255," &
                     SystemCartographique.Niveau.PasGrilleTexte & ",0,16777215,8,1,Yes,No,No,x" & CrLf &
                     "MOP,Map Open Position," & _Coins(0).X_Pixel.ToString("000000") & "," & _Coins(0).Y_Pixel.ToString("000000") & CrLf &
                     "IWH,Map Image Width/Height," & _Coins(2).X_Pixel.ToString("000000") & "," & _Coins(2).Y_Pixel.ToString("000000"))

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".map", S.ToString, Encoding_FCGP)
            EcrireOziExploreurLV03 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "U3D5")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à OziExploreur en système WGS84 (latitude et longitude) 
    ''' valable pour le site SM ou GF interpolé ou DT. Voir la description du format sur le net</summary>
    Private Function EcrireOziExploreurWGS84() As Boolean
        EcrireOziExploreurWGS84 = False
        Try
            Dim S As New StringBuilder   'pour écrire le fichier
            'l'entête est  toujours le même
            S.AppendLine("OziExplorer Map Data File Version 2.2" & CrLf & NomComplet & "." & Format.ToString & CrLf &
                         CheminCarte & "\" & NomComplet & "." & Format.ToString & CrLf & "1 ,Map Code," & CrLf &
                         "WGS 84,WGS 84,   0.0000,   0.0000,WGS 84" & CrLf & "Reserved 1" & CrLf & "Reserved 2" & CrLf &
                         "Magnetic Variation,,,E" & CrLf & "Map Projection,Latitude/Longitude,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No")

            Dim NumPoint As Integer = 0
            For Cpt = 0 To 2 Step 2
                NumPoint += 1
                Dim Longitude = Math.Abs(_Coins(Cpt).Longitude)
                Dim Latitude = Math.Abs(_Coins(Cpt).Latitude)
                Dim IsPositifLatitude = _Coins(Cpt).Latitude >= 0
                Dim IsPositifLongitude = _Coins(Cpt).Longitude >= 0
                S.AppendLine("Point" & NumPoint.ToString("00") & ",xy," & _Coins(Cpt).X_Pixel.ToString("000000") & "," & _Coins(Cpt).Y_Pixel.ToString("000000") &
                             ",in, deg," & Math.Floor(Latitude).ToString("00") & "," &
                             DblToStr((Latitude - Math.Floor(Latitude)) * 60, "00.000000") & "," &
                             If(IsPositifLatitude, "N", "S") & "," & Math.Floor(Longitude).ToString("000") & "," &
                             DblToStr((Longitude - Math.Floor(Longitude)) * 60, "00.000000") & "," &
                             If(IsPositifLongitude, "E", "W") & ",grid,,,,N")
            Next

            For Cpt = 3 To 9
                S.AppendLine("Point" & Cpt.ToString("00") & ",xy,,,in,deg,,,N,,,,grid,,,,N")
            Next
            For Cpt = 10 To 30
                S.AppendLine("Point" & Cpt.ToString("00") & ",xy,,,in,deg,,,,,,,grid,,,,")
            Next

            S.AppendLine("Projection Setup,,,,,,,,,," & CrLf &
                         "Map Feature = MF ; Map Comment = MC     These follow if they exist" & CrLf &
                         "Track File = TF      These follow if they exist" & CrLf &
                         "MM0,Yes" & CrLf & "MMPNUM,4")

            For Cpt = 0 To 3
                S.AppendLine("MMPXY," & (Cpt + 1).ToString("0") & "," & _Coins(Cpt).X_Pixel.ToString("000000") & "," &
                             _Coins(Cpt).Y_Pixel.ToString("000000"))
            Next
            For Cpt = 0 To 3
                S.AppendLine("MMPLL," & (Cpt + 1).ToString("0") & "," & If(Coins(Cpt).Longitude >= 0, "+", "") &
                             DblToStr(Coins(Cpt).Longitude, "000.0000000") & If(Coins(Cpt).Latitude >= 0, ",+", ",") &
                             DblToStr(Coins(Cpt).Latitude, "00.0000000"))
            Next

            S.AppendLine("MM1B," & DblToStr(Echelle_M_PixelX, "0.000000") & CrLf &
                         "LL Grid Setup" & CrLf & "LLGRID,No,No Grid,Yes,255,16711680,0,No Labels,0,16777215,7,1,Yes,x" & CrLf &
                         "Other Grid Setup" & CrLf & "GRGRID,Yes," & SystemCartographique.Niveau.PasGrilleTexte &
                         ",Yes,16711935,255," & SystemCartographique.Niveau.PasGrilleTexte & ",0,16777215,8,1,Yes,No,No,x" & CrLf &
                         "MOP,Map Open Position,0,0")

            S.Append("IWH,Map Image Width/Height," & _Coins(2).X_Pixel.ToString("000000") & "," & _Coins(2).Y_Pixel.ToString("000000"))

            File.WriteAllText(CheminCarte & "\" & NomComplet & ".map", S.ToString, Encoding_FCGP)
            EcrireOziExploreurWGS84 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "E3Z5")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à QGIS en système WebMercator (EPSG:3857) 
    ''' valable pour le site GF non interpolé . Voir la description du format sur le net</summary>
    Private Function EcrireQgisMercator() As Boolean
        EcrireQgisMercator = False
        Try
            Dim S As New StringBuilder   'pour écrire le fichier
            'Dim X0 As Double, Y0 As Double, X2 As Double, Y2 As Double
            Dim Pt0 = ConvertWGS84ToProjection(_Coins(0).LatLon, Datums.Web_Mercator)
            Dim Pt2 = ConvertWGS84ToProjection(_Coins(2).LatLon, Datums.Web_Mercator)
            Dim DeltaX As Double = (Pt2.X - Pt0.X) / _Coins(2).X_Pixel
            Dim DeltaY As Double = (Pt2.Y - Pt0.Y) / _Coins(2).Y_Pixel
            Dim Extension As String = "." & Format.ToString.Chars(0) & Format.ToString.Chars(Format.ToString.Length - 1) & "w"
            S.AppendLine(DblToStr(DeltaX, "0.0000000000000000"))
            S.AppendLine(_Coins(0).X_Pixel.ToString("0"))
            S.AppendLine(_Coins(0).Y_Pixel.ToString("0"))
            S.AppendLine(DblToStr(DeltaY, "0.00000000000000000"))
            'géo-référencement au centre du pixel alors que pour FCGP le géo-référencement est au coin haut-gauche du pixel
            S.AppendLine(DblToStr(Pt0.X + DeltaX / 2, "0.0000000000000000"))
            S.AppendLine(DblToStr(Pt0.Y + DeltaY / 2, "0.0000000000000000"))
            S.Append("EPSG:3857")
            File.WriteAllText(CheminCarte & "\" & NomComplet & Extension, S.ToString, Encoding_FCGP)
            EcrireQgisMercator = True
        Catch Ex As Exception
            AfficherErreur(Ex, "P8W7")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à QGIS en système CH1903 / LV03 (EPSG:21781) 
    ''' valable pour le site SM non interpolé . Voir la description du format sur le net</summary>
    Private Function EcrireQgisLV03() As Boolean
        EcrireQgisLV03 = False
        Try
            Dim S As New StringBuilder   'pour écrire le fichier
            Dim DeltaX As Double = (_Coins(2).X_Grid - _Coins(0).X_Grid) / _Coins(2).X_Pixel
            Dim DeltaY As Double = (_Coins(2).Y_Grid - _Coins(0).Y_Grid) / _Coins(2).Y_Pixel
            Dim Extension As String = "." & Format.ToString.Chars(0) & Format.ToString.Chars(Format.ToString.Length - 1) & "w"
            S.AppendLine(DblToStr(DeltaX, "0.0000000000000000"))
            S.AppendLine(_Coins(0).X_Pixel.ToString("0"))
            S.AppendLine(_Coins(0).Y_Pixel.ToString("0"))
            S.AppendLine(DblToStr(DeltaY, "0.00000000000000000"))
            'géo-référencement au centre du pixel alors que pour FCGP le géo-référencement est au coin haut-gauche du pixel
            Dim PGR As New PointD(_Coins(0).X_Grid, _Coins(0).Y_Grid)
            LV95ToLV03(PGR)
            S.AppendLine(DblToStr(PGR.X + DeltaX / 2, "0.0000000000000000"))
            S.AppendLine(DblToStr(PGR.Y + DeltaY / 2, "0.0000000000000000"))
            S.Append("EPSG:21781")

            File.WriteAllText(CheminCarte & "\" & NomComplet & Extension, S.ToString, Encoding_FCGP)
            EcrireQgisLV03 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "O7T5")
        End Try
    End Function
    ''' <summary> Ecrit le fichier de géoréférencement spécifique à QGIS en système CH1903 / LV03 (EPSG:21781) 
    ''' valable pour le site SM non interpolé . Voir la description du format sur le net</summary>
    Private Function EcrireQgisWGS84() As Boolean
        EcrireQgisWGS84 = False
        Try
            Dim S As New StringBuilder   'pour écrire le fichier
            Dim DeltaLon As Double = (_Coins(2).Longitude - _Coins(0).Longitude) / _Coins(2).X_Pixel
            Dim DeltaLat As Double = (_Coins(2).Latitude - _Coins(0).Latitude) / _Coins(2).Y_Pixel
            Dim Extension As String = "." & Format.ToString.Chars(0) & Format.ToString.Chars(Format.ToString.Length - 1) & "w"
            S.AppendLine(DblToStr(DeltaLon, "0.0000000000000000"))
            S.AppendLine(_Coins(0).X_Pixel.ToString("0"))
            S.AppendLine(_Coins(0).Y_Pixel.ToString("0"))
            S.AppendLine(DblToStr(DeltaLat, "0.00000000000000000"))
            'géo-référencement au centre du pixel alors que pour FCGP le géo-référencement est au coin haut-gauche du pixel
            S.AppendLine(DblToStr(_Coins(0).Longitude + DeltaLon / 2, "0.0000000000000000"))
            S.AppendLine(DblToStr(_Coins(0).Latitude + DeltaLat / 2, "0.0000000000000000"))
            S.Append("EPSG:4326")
            File.WriteAllText(CheminCarte & "\" & NomComplet & Extension, S.ToString, Encoding_FCGP)
            EcrireQgisWGS84 = True
        Catch Ex As Exception
            AfficherErreur(Ex, "O4J4")
        End Try
    End Function
#End Region
End Class