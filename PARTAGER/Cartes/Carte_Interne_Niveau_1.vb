''' <summary>contient toutes les procédures et fonctions nécessaires pour écrire les fichiers de géoréferencement
''' d'une carte ou lire un fichier georef mais uniquement private</summary>
Partial Friend Class Carte
#Region "procédures générales"
    ''' <summary> Procède à la création de la carte interpolée et réalise les demandes associées. </summary>
    Private Function GenererInterpolation() As Boolean
        GenererInterpolation = False
        Try
            'création de la carte interpolée
            Using CarteInterpole As New Carte()
                'il faut faire l'interpolation de la carte avant de générer les fichiers tuiles
                If Not InterpolerCarte(CarteInterpole) = ErreurInterpolation.Ok Then Exit Try
                If (DemandeFichiersGeoref And ChoixFichiersGeoref.Interpols) > ChoixFichiersGeoref.Aucun Then
                    If Not CarteInterpole.GenererFichiersGeoreferencement() Then Exit Try
                End If
                If DemandeFichiersTuiles > ChoixFichiersTuiles.Aucun Then
                    If Not CarteInterpole.GenererFichiersTuiles() Then Exit Try
                End If
                If GrilleIsAffiche And DemandeAjouterCoordonneesGrille.HasFlag(ChoixCoordonneesGrille.Ref_Interpol) Then
                    If Not CarteInterpole.AjouterReferences() Then Exit Try
                End If
            End Using
            GenererInterpolation = True
        Catch Ex As Exception
            AfficherErreur(Ex, "L8L7")
        End Try
    End Function
    ''' <summary> Ecrit les fichiers de géoréférencement associés à la carte. Cette procédure est appelée soit par la carte originale soit par la carte interpolée</summary>
    Private Function GenererFichiersGeoreferencement() As Boolean
        GenererFichiersGeoreferencement = False
        AfficherVisueInformation($"{NomComplet} :{Separateur}Création des fichiers de géoréférencement")
        Try
            'on en enregistre systématiquement la carte par type de carte car il y a forcément un fichier de géo-référencment si on est là
            If ((DemandeFichiersGeoref And ChoixFichiersGeoref.Cartes) > ChoixFichiersGeoref.Aucun And Not SystemCartographique.IsInterpol) OrElse
               ((DemandeFichiersGeoref And ChoixFichiersGeoref.Interpols) > ChoixFichiersGeoref.Aucun And SystemCartographique.IsInterpol) Then
                Dim NomFichierCarte As String = CheminCarte & "\" & NomComplet
                If TamponType = TypeSupportCarte.Fichier Then
                    If File.Exists(NomFichierCarte) Then File.Delete(NomFichierCarte)
                    If Image.Width > MaxPixelsJpeg OrElse Image.Height > MaxPixelsJpeg OrElse Format Is ImageFormat.Bmp Then
                        'l'enregistrement se fera en BMP et on change éventuellement le format de la carte si il n'est pas supporté
                        If Format IsNot ImageFormat.Bmp Then Format = ImageFormat.Bmp
                        If Not FichierSrcToDst(NomFichierCarte & "." & Format.ToString, True) Then Exit Try
                    Else
                        'sinon en JPEG ou PNG suivant le choix effectif de l'utilisateur
                        If Not FichierRawToFormats(NomFichierCarte & "." & Format.ToString, False) Then Exit Try
                    End If
                Else
                    If Image.Width > MaxPixelsJpeg OrElse Image.Height > MaxPixelsJpeg OrElse Format Is ImageFormat.Bmp Then
                        'l'enregistrement se fera en BMP et on change éventuellement le format de la carte si il n'est pas supporté
                        If Format IsNot ImageFormat.Bmp Then Format = ImageFormat.Bmp
                        Image.Save(NomFichierCarte & "." & Format.ToString, Format)
                    ElseIf Format Is ImageFormat.Jpeg Then
                        'ou en JPEG avec la qualité désirée
                        Image.Save(NomFichierCarte & "." & Format.ToString, EncodeurJpeg, QualiteJPEG(PartagerSettings.QUALITE_JPEG))
                    Else
                        'ou en PNG sans perte
                        Image.Save(NomFichierCarte & "." & Format.ToString, Format)
                    End If
                End If
            End If
            If (DemandeFichiersGeoref And ChoixFichiersGeoref.Georefs) > ChoixFichiersGeoref.Aucun Then
                If ((DemandeFichiersGeoref And ChoixFichiersGeoref.Georef_Carte) > ChoixFichiersGeoref.Aucun AndAlso Not SystemCartographique.IsInterpol) OrElse
                   ((DemandeFichiersGeoref And ChoixFichiersGeoref.Georef_Interpol) > ChoixFichiersGeoref.Aucun AndAlso SystemCartographique.IsInterpol) Then
                    If Not EcrireFichierGeoref() Then Exit Try
                End If
            End If
            'on sauve les fichiers de géoréferencement
            If (DemandeFichiersGeoref And ChoixFichiersGeoref.MapInfos) > ChoixFichiersGeoref.Aucun Then
                If (SystemCartographique.SiteCarto = SitesCartographiques.Géofoncier OrElse
                    SystemCartographique.SiteCarto = SitesCartographiques.DomTom) And Not SystemCartographique.IsInterpol Then
                    If Not EcrireMapInfoMercator() Then Exit Try
                ElseIf SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile And Not SystemCartographique.IsInterpol Then
                    If Not EcrireMapInfoLV03() Then Exit Try
                Else
                    If Not EcrireMapInfoWGS84() Then Exit Try
                End If
            End If
            If (DemandeFichiersGeoref And ChoixFichiersGeoref.OziExploreurs) > ChoixFichiersGeoref.Aucun Then
                'si retail cela indique que c'est une Demarage.Captures Suisse interpolée en wgs84
                If (SystemCartographique.SiteCarto = SitesCartographiques.Géofoncier OrElse
                    SystemCartographique.SiteCarto = SitesCartographiques.DomTom) And Not SystemCartographique.IsInterpol Then
                    If Not EcrireOziExploreurMercator() Then Exit Try
                ElseIf SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile And Not SystemCartographique.IsInterpol Then
                    If Not EcrireOziExploreurLV03() Then Exit Try
                Else
                    If Not EcrireOziExploreurWGS84() Then Exit Try
                End If
            End If
            If (DemandeFichiersGeoref And ChoixFichiersGeoref.CompeLands) > ChoixFichiersGeoref.Aucun Then
                If (SystemCartographique.SiteCarto = SitesCartographiques.Géofoncier OrElse
                    SystemCartographique.SiteCarto = SitesCartographiques.DomTom) And Not SystemCartographique.IsInterpol Then
                    If Not EcrireCompeLandMercator() Then Exit Try
                ElseIf SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile And Not SystemCartographique.IsInterpol Then
                    If Not EcrireCompeLandLV03() Then Exit Try
                Else
                    If Not EcrireCompeLandWGS84() Then Exit Try
                End If
            End If
            If (DemandeFichiersGeoref And ChoixFichiersGeoref.QGISs) > ChoixFichiersGeoref.Aucun Then
                If (SystemCartographique.SiteCarto = SitesCartographiques.Géofoncier OrElse
                    SystemCartographique.SiteCarto = SitesCartographiques.DomTom) And Not SystemCartographique.IsInterpol Then
                    If Not EcrireQgisMercator() Then Exit Try
                ElseIf SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile And Not SystemCartographique.IsInterpol Then
                    If Not EcrireQgisLV03() Then Exit Try
                Else
                    If Not EcrireQgisWGS84() Then Exit Try
                End If
            End If
            GenererFichiersGeoreferencement = True
        Catch Ex As Exception
            AfficherErreur(Ex, "D9Z0")
        End Try
    End Function
    ''' <summary> fait le ou les fichiers tuiles à partir de la carte transmise et du nb de tuiles demandées </summary>
    Private Function GenererFichiersTuiles() As Boolean
        GenererFichiersTuiles = False
        'créer un repertoire tampon fichier KML et un sous repertoire tampon des tuiles
        Try
            Dim DirInfo As DirectoryInfo = Directory.CreateDirectory(CheminEnregistrementProvisoire & ExtensionCheminImagesTuiles)
            MessInfo = $"{NomComplet} :{Separateur}Génération des tuiles :{Separateur}"
            If Not EcrireTuiles() Then Exit Try
            If Tuiles IsNot Nothing Then 'si il y a au moins une tuile on peut générer les fichiers tuiles si ils sont demandés
                If DemandeFichiersTuiles.HasFlag(ChoixFichiersTuiles.KMZ) Then
                    MessInfo = $"{NomComplet} :{Separateur}Génération du fichier KMZ :{Separateur}"
                    If Not EcrireFichierKMZ() Then Exit Try
                End If
                If DemandeFichiersTuiles.HasFlag(ChoixFichiersTuiles.JNX) Then
                    If DemandeAjoutNiveau Then
                        MessInfo = $"{NomComplet} :{Separateur}Ajout d'un niveau au fichier JNX :{Separateur}"
                        If Not AjouterNiveauCarteJNX(Me) Then Exit Try
                    Else
                        MessInfo = $"{NomComplet} :{Separateur}Génération du fichier JNX :{Separateur}"
                        If Not CreerCarteJNX(Me) Then Exit Try
                    End If
                End If
                If DemandeFichiersTuiles.HasFlag(ChoixFichiersTuiles.ORUX) Then
                    If DemandeAjoutNiveau Then
                        MessInfo = $"{NomComplet} :{Separateur}Ajout d'un niveau au fichier ORUX :{Separateur}"
                        If Not AjouterNiveauCarteOrux(Me) Then Exit Try
                    Else
                        MessInfo = $"{NomComplet} :{Separateur}Génération du fichier ORUX :{Separateur}"
                        If Not CreerCarteOrux(Me) Then Exit Try
                    End If
                End If
            End If
            GenererFichiersTuiles = True
        Catch Ex As Exception
            AfficherErreur(Ex, "G3U1")
        Finally
            'efface le repertoire tampon fichier KML et le sous repertoire tampon des tuiles avec tous les fichiers
            If Directory.Exists(CheminEnregistrementProvisoire & ExtensionCheminTuiles) Then Directory.Delete(CheminEnregistrementProvisoire & ExtensionCheminTuiles, True)
        End Try
    End Function
    ''' <summary> fait un fichier kmz à partir des tuiles transmises </summary>
    Private Function EcrireFichierKMZ() As Boolean
        Const NomKML As String = "doc.kml"
        Dim STuiles As New StringBuilder, SCarte As New StringBuilder
        Dim N As Double = -100.0R, E As Double = -200.0R, S As Double = 100.0R, O As Double = 200.0R
        EcrireFichierKMZ = False
        AfficherVisueInformation($"{MessInfo}Ecriture fichier KMZ")
        Try
            'ecrire la partie des tuiles dans le tampon texte
            For Each T As DescriptionImageFichier In Tuiles
                STuiles.AppendLine("    <GroundOverlay>")
                STuiles.AppendLine($"      <name>{T.Nom}</name>")
#If DEBUG Then 'Permet de vérifier que l'interpolation des cartes est correcte avec basecamp ou google earth
                STuiles.AppendLine("      <color>80ffffff</color>")
#End If
                STuiles.AppendLine("      <drawOrder>50</drawOrder>")
                STuiles.AppendLine("      <Icon>")
                STuiles.AppendLine($"        <href>files/{T.Nom}</href>")
                STuiles.AppendLine("      </Icon>")
                STuiles.AppendLine("      <LatLonBox>")
                STuiles.AppendLine($"        <north>{DblToStr(T.Nord, "0.000000000")}</north>")
                STuiles.AppendLine($"        <south>{DblToStr(T.Sud, "0.000000000")}</south>")
                STuiles.AppendLine($"        <east>{DblToStr(T.Est, "0.000000000")}</east>")
                STuiles.AppendLine($"        <west>{DblToStr(T.Ouest, "0.000000000")}</west>")
                STuiles.AppendLine("      </LatLonBox>")
                STuiles.AppendLine("    </GroundOverlay>")
                'calcule la plus grande surface contenant l'ensemble des tuiles. normalement les coordonnées LL en DD du fichier georef
                N = Math.Max(T.Nord, N)
                E = Math.Max(T.Est, E)
                S = Math.Min(T.Sud, S)
                O = Math.Min(T.Ouest, O)
            Next
            'Ecrire entête du fichier
            SCarte.AppendLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
            SCarte.AppendLine("<kml xmlns=""http://earth.google.com/kml/2.2/FCGP"">")
            SCarte.AppendLine("  <Document>")
            SCarte.AppendLine($"    <name>{Nom}.{Format}</name>")
            SCarte.AppendLine("    <open>1</open>")
            SCarte.AppendLine("    <Region> ")
            SCarte.AppendLine("      <LatLonAltBox>")
            SCarte.AppendLine($"        <north>{DblToStr(N, "0.000000000")}</north>")
            SCarte.AppendLine($"        <south>{DblToStr(S, "0.000000000")}</south>")
            SCarte.AppendLine($"        <east>{DblToStr(E, "0.000000000")}</east>")
            SCarte.AppendLine($"        <west>{DblToStr(O, "0.000000000")}</west>")
            SCarte.AppendLine("      </LatLonAltBox>")
            SCarte.AppendLine("    </Region>")
            'ajouter la description des tuiles
            SCarte.Append(STuiles)
            'si il y a eu une demande pour incorporer une ou des traces dans le fichier kmz en tant qu'objet kml
            If NbTracesKML > 0 Then
                Dim Strace As New StringBuilder(25000 * NbTracesKML)
                If Not DefinirTracesKML(Strace) Then Exit Try
                'si il y a eu une ou des traces ok on les ajoute au fichier KMZ
                If Strace.Length > 0 Then
                    SCarte.Append(Strace)
                    'on rajoute un suffixe au nom pour indiquer que le fichier KMZ contient une ou des traces
                    Nom &= "_KML"
                End If
            End If
            'ecrire la fin du fichier kmz
            SCarte.AppendLine("  </Document>")
            SCarte.AppendLine("</kml>")
            AfficherVisueInformation($"{MessInfo}Compression des fichiers")
            'enregistrer le fichier doc.kml sur le disque dur
            File.WriteAllText(CheminEnregistrementProvisoire + ExtensionCheminTuiles + "\" + NomKML, SCarte.ToString(), Encoding_FCGP)
            'zip les differents fichiers pour en faire un fichier kmz
            Dim NomArchive = CheminFichiersTuile & "\" & NomComplet & ".kmz"
            If File.Exists(NomArchive) Then File.Delete(NomArchive)
            ZipFile.CreateFromDirectory(CheminEnregistrementProvisoire & ExtensionCheminTuiles, NomArchive)
            EcrireFichierKMZ = True
        Catch Ex As Exception
            AfficherErreur(Ex, "D6W2")
        End Try
    End Function
#End Region
#Region "Traces"
    ''' <summary> procédure à appeler pour ajouter les points de la trace en cours dans le fichier kmz en tant que trace KML(objet de dessin) </summary>
    ''' <param name="S"> Stringbuilder vide mais dimmensionée </param>
    Private Function DefinirTracesKML(ByRef S As StringBuilder) As Boolean
        Dim FlagUnPath As Boolean = False, FlagTraceOk As Boolean = False
        Try
            FlagUnPath = False
            'on ajoute le style des traces KMZ au sens Google
            S.Append(TRK.AjouterStyleKML(NbTracesKML, Color.Red, 5, Color.FromArgb(252, Color.White), 8))
            For Each NomTrace As String In FichiersTRK
                Dim Trace As New TRK(NomTrace, SystemCartographique.SiteCarto, SystemCartographique.DomTom, SystemCartographique.Niveau.Echelle)
                'si la trace est tout ou en partie sur la carte on peut la dessiner
                If _RegionVirtuelle.IntersectsWith(Trace.BoundingBoxVirtuel) Then
                    S.Append(TRK.AjouterTraceKML(NbTracesKML, Trace))
                    FlagTraceOk = True
                Else
                    MessageInformation = $"La trace {Trace.Nom} n'est pas dans l'emprise de la carte {Nom}"
                    Dim Titre = TitreInformation
                    TitreInformation = "Information Trace"
                    AfficherInformation()
                    TitreInformation = Titre
                End If
            Next
            If Not FlagTraceOk Then S.Clear()
            FlagUnPath = True
        Catch Ex As Exception
            AfficherErreur(Ex, "A0T4")
        End Try
        Return FlagUnPath
    End Function
    ''' <summary> procédure à appeler pour dessiner les traces sur la carte ou les ajouter au kmz. Uniquement dans carte interpole </summary>
    Private Function AfficherTraces() As Boolean
        AfficherVisueInformation($"{NomComplet} :{Separateur}Dessin des traces sur la carte")
        AfficherTraces = False
        Try
            Dim CptTrace As Integer
            If FichiersTRK IsNot Nothing Then
                'on s'assure qu'il y a bien un pinceau pour peindre la trace
                If PinceauTrace Is Nothing Then PinceauTrace = New Pen(ConvertirSettings.COULEUR_TRACE, ConvertirSettings.EPAISSEUR_TRACE) With {.LineJoin = LineJoin.Round}
                For Each NomTrace As String In FichiersTRK
                    Dim Trace As New TRK(NomTrace, SystemCartographique.SiteCarto, SystemCartographique.DomTom, SystemCartographique.Niveau.Echelle)
                    If _RegionVirtuelle.IntersectsWith(Trace.BoundingBoxVirtuel) Then 'si la trace est tout ou en partie sur la carte on peut la dessiner
                        If TamponType = TypeSupportCarte.Fichier Then
                            Using FichierBIN As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                                Dim TailleTampon As Integer = LargeurOctets * HauteurReserve, HauteurALire As Integer
                                CheminFluxLecture = CheminEnregistrementProvisoire & "\CarteAvecTrace.raw"
                                Using FichierAvecGrille As New IO.FileStream(CheminFluxLecture, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.Read, TailleTampon)
                                    Do
                                        'on charge une partie de la carte sans grille dans le tampon
                                        FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTampon)
                                        'on dessinne la grille
                                        For Cpt As Integer = 0 To NbZonesUTM - 1
                                            If Not DessinerTrace(HauteurALire, Trace.ListePointsPixel) Then Exit Try 'il peut y avoir une erreur
                                        Next
                                        'on enregistrer la carte avec la grille
                                        FichierAvecGrille.Write(Tampon.Bits, Tampon.IndexBits, TailleTampon)
                                        HauteurALire += HauteurReserve
                                        If HauteurALire < HauteurPixel And HauteurALire + HauteurReserve > HauteurPixel Then
                                            TailleTampon = LargeurOctets * (HauteurPixel - HauteurALire)
                                        End If
                                    Loop While HauteurALire < HauteurPixel
                                End Using
                            End Using
                        Else ' cas du suppport de carte en mémoire. Il n'y a qu'une seule tranche donc pas de décalage pour le dessin et on travaille sur la carte entière
                            If Not DessinerTrace(0, Trace.ListePointsPixel) Then Exit Try 'il peut y avoir une erreur
                        End If
                        CptTrace += 1
                    Else
                        MessageInformation = $"La trace {Trace.Nom} n'est pas dans l'emprise de la carte {Nom}"
                        Dim Titre = TitreInformation
                        TitreInformation = "Information Trace"
                        AfficherInformation()
                        TitreInformation = Titre
                    End If
                Next
            End If
            'on indique que la carte contient 1 ou plusieurs traces
            If CptTrace > 0 Then Nom &= "_Trace"
            AfficherTraces = True
        Catch Ex As Exception
            AfficherErreur(Ex, "F0I1")
        End Try
    End Function
    ''' <summary> dessine une trace sur un graphics qui représente tout ou partie de la carte </summary>
    ''' <param name="DecalY"> si il y a plusieurs tranche, représente le décalage à prendre en compte  sur l'axe des y en pixels </param>
    ''' <param name="PointsTrace"> points de la trace à dessiner sur la carte </param>
    Private Function DessinerTrace(DecalY As Integer, PointsTrace As Point()) As Boolean
        DessinerTrace = False
        Try
            'On indique que l'on travaille avec la carte en cours, si l'image provient d'une carte existante on est obligé de recréer un bitmap car le bitmap existant est en lecture seul
            Using Carte_TraceGraphic As Graphics = Graphics.FromImage(If(Tampon.IsBitmap, New Bitmap(LargeurPixel, HauteurReserve, LargeurOctets, FormatPixel, ImageBitPtr), Image))
                Carte_TraceGraphic.PageUnit = GraphicsUnit.Pixel
                Carte_TraceGraphic.SmoothingMode = SmoothingMode.AntiAlias
                Carte_TraceGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality
                Carte_TraceGraphic.TranslateTransform(-PointOrigne.Width, -PointOrigne.Height - DecalY)
                Carte_TraceGraphic.DrawLines(PinceauTrace, PointsTrace)
            End Using
            DessinerTrace = True
        Catch Ex As Exception
            AfficherErreur(Ex, "F8J4")
        End Try
    End Function
#End Region
#Region "Création tuiles"
    ''' <summary> génère un tableau décrivant l'ensemble des tuiles et ecrit les images correspondantes dans le repertoire idoine </summary> 
    Private Function EcrireTuiles() As Boolean
        EcrireTuiles = False
        Try
            'pour le calcul du geo-referencement des tuiles des fichiers tuiles JNX et KMZ. Orux n'en a pas besoin seulement la carte
            Dim EchelleX As Double = (Coins(2).Longitude - Coins(0).Longitude) / LargeurPixel
            Dim EchelleY As Double = (Coins(0).Latitude - Coins(2).Latitude) / HauteurPixel
            'Cherche le nb de tuiles en largeur et hauteur
            _NbTuilesX = CInt(Math.Ceiling(LargeurPixel / DimensionsTuile))
            _NbTuilesY = CInt(Math.Ceiling(HauteurPixel / DimensionsTuile))
            'il faut au moins une tuile pour faire un fichier tuile et dans le cas de IsExact = True et  IsRompu = False, il peut ne pas y en avoir
            NbIterations = 0
            NbTotalIterations = NbTuilesX * NbTuilesY
            AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
            ReDim Tuiles(NbTotalIterations - 1)
            If TamponType = TypeSupportCarte.Fichier Then
                Using FichierRaw As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                    If HauteurReserve < DimensionsTuile Then
                        Throw New Exception("La hauteur du tampon de la carte est " & CrLf &
                                            "inférieure à la hauteur de la tuile du fichier tuile")
                    End If
                    Dim NbHauteurTuiles, TailleTamponTuiles As Integer
                    NbHauteurTuiles = HauteurReserve \ DimensionsTuile  'Nb tuiles maximum en vertical par tranche
                    TailleTamponTuiles = _LargeurOctets * NbHauteurTuiles * DimensionsTuile
                    Dim DecalTuileY As Integer
                    Do
                        'on charge une partie de la carte sans grille dans le tampon
                        FichierRaw.Read(Tampon.Bits, Tampon.IndexBits, TailleTamponTuiles)
                        'on génère les tuiles de la tranche
                        If Not EcrireTuilesTranche(DecalTuileY, NbHauteurTuiles, EchelleX, EchelleY) Then Exit Try
                        DecalTuileY += NbHauteurTuiles
                        'la tranche  à venir est elle entière : NbHauteurTuiles, partielle : 1 à NbHauteurTuiles-1 ou vide : 0
                        If DecalTuileY + NbHauteurTuiles >= NbTuilesY Then 'il faut recalculer la hauteur en nombre de tuile
                            NbHauteurTuiles = NbTuilesY - DecalTuileY
                        End If
                    Loop While DecalTuileY < NbTuilesY
                End Using
            Else 'il n'y a qu'une seule tranche quand le support de carte est la mémoire
                If Not EcrireTuilesTranche(0, NbTuilesY, EchelleX, EchelleY) Then Exit Try
            End If
            EcrireTuiles = True
        Catch Ex As Exception
            AfficherErreur(Ex, "D5X8")
        End Try
    End Function
    ''' <summary> génère les tuiles de la carte transmise en fonction des dimensions fournies de l'image</summary>
    ''' <remarks>si les dimensions de la carte sont plus grandes que le nb de tuiles, il y aura des petites tuiles en plus pour compléter</remarks>
    Private Function EcrireTuilesTranche(DebutTuileY As Integer, NBY As Integer, EchelleX As Double, EchelleY As Double) As Boolean
        EcrireTuilesTranche = False
        Try
            Dim CptTuiles As Integer = DebutTuileY * NbTuilesX
            Dim FinY As Integer = -1
            Dim HTuile = DimensionsTuile
            For CptTuilesY As Integer = DebutTuileY To DebutTuileY + NBY - 1
                Dim DebutY As Integer = FinY + 1 'debut des tuiles en pixel
                If CptTuilesY = 0 Then
                    'si c'est la première tuile sur les Y C00X00Y ou ligne 0, le nord égal à la latitude du coins 0
                    Tuiles(CptTuiles) = New DescriptionImageFichier With {.Nord = Coins(0).Latitude}
                Else
                    'sinon les données sont les mêmes hormis nord  = sud de la tuile précedente le sud et la hauteur qui dépendent si c'est la dernière tuile de la colonne
                    Tuiles(CptTuiles) = New DescriptionImageFichier With {.Nord = Tuiles(CptTuiles - NbTuilesX).Sud}
                End If
                If CptTuilesY = NbTuilesY - 1 Then
                    HTuile = HauteurPixel - CptTuilesY * HTuile 'la dernière ligne est différente si les rompus sont acceptés
                    Tuiles(CptTuiles).Sud = Coins(2).Latitude
                Else
                    FinY += HTuile
                    Tuiles(CptTuiles).Sud = Tuiles(CptTuiles).Nord - HTuile * EchelleY
                End If
                Tuiles(CptTuiles).HauteurPixels = HTuile
                'boucle sur la largeur de la carte
                Dim FinX As Integer = -1
                Dim LTuile As Integer = DimensionsTuile
                For CptTuilesX As Integer = 0 To NbTuilesX - 1
                    Dim DebutX As Integer = FinX + 1
                    If CptTuilesX = 0 Then
                        'si c'est la première tuile sur les X  C00000Y ou colonne 0, l'ouest égal à la longitude du coins 0
                        Tuiles(CptTuiles).Ouest = Coins(0).Longitude    'la tuile est déjà créée dans la boucle Y
                    Else
                        'sinon l'ouest égal l'est de la colonne précédente et les latitudes et la hauteur aussi
                        'il faut créer la tuile
                        Tuiles(CptTuiles) = New DescriptionImageFichier With {.Ouest = Tuiles(CptTuiles - 1).Est, .Nord = Tuiles(CptTuiles - 1).Nord,
                                                                   .Sud = Tuiles(CptTuiles - 1).Sud, .HauteurPixels = Tuiles(CptTuiles - 1).HauteurPixels}
                    End If
                    If CptTuilesX = NbTuilesX - 1 Then
                        LTuile = LargeurPixel - CptTuilesX * LTuile 'la dernière colonne est différente si les rompus sont acceptés
                        Tuiles(CptTuiles).Est = Coins(2).Longitude
                    Else
                        FinX += LTuile
                        Tuiles(CptTuiles).Est = Tuiles(CptTuiles).Ouest + LTuile * EchelleX
                    End If
                    Tuiles(CptTuiles).LargeurPixels = LTuile
                    'calcule le nom de la tuile
                    Tuiles(CptTuiles).Nom = "C" & (CptTuilesX).ToString("000") & (CptTuilesY).ToString("000") & ".jpg"
                    'déclaration de la tuile en tant qu'image jpeg
                    Using b As New Bitmap(LTuile, HTuile, Tampon.Stride, FormatPixel, Tampon.BitPtr + DebutX * NbOctetsPixel + DebutY * Tampon.Stride)
                        b.Save(CheminEnregistrementProvisoire & ExtensionCheminImagesTuiles & "\" & Tuiles(CptTuiles).Nom, ImageFormat.Jpeg)
                    End Using
                    CptTuiles += 1
                    Interlocked.Increment(NbIterations)
                    If NbIterations Mod 100 = 0 Then AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
                Next
            Next
            EcrireTuilesTranche = True
        Catch Ex As Exception
            AfficherErreur(Ex, "P9G3")
        End Try
    End Function
#End Region
#Region "Création Carte existante à partir fichier Georef"
    ''' <summary>Lit un fichier georef et remplit toutes les variables de la carte hormis le système cartographique
    ''' ne prend pas en compte la première version du format (geoportail)</summary>
    ''' <param name="LignesFichierGeoref">lignes du fichier Georef à lire</param>
    Private Function LireFichierGeoref(LignesFichierGeoref() As String) As Boolean
        Dim Ret As Boolean = False
        Try
            Dim Cpt As Integer
            Cpt = LignesFichierGeoref(2).IndexOf(","c) + 1
            GrilleIsAffiche = True
            If TrouverChaineCompriseEntre(LignesFichierGeoref(2), Cpt, ":"c, ","c) = "Non" Then GrilleIsAffiche = False
            For Boucle As Integer = 0 To 3
                Cpt = 0
                Coins(Boucle).CoordonneesCaptureEcran = TrouverChaineCompriseEntre(LignesFichierGeoref(3 * Boucle + 3), Cpt, ":"c)
                'remplir toutes les variables associées aux coordonnées Grille et LL de la structure Georef de 4 coins
                If Not DecomposerCoordonneesCapturees(SystemCartographique, Coins(Boucle)) Then Return False
                'lire les coordonnées en pixel de chaque coins
                Cpt = 0
                Coins(Boucle).X_Pixel = CInt(TrouverChaineCompriseEntre(LignesFichierGeoref(3 * Boucle + 5), Cpt, ":"c, ","c))
                Coins(Boucle).Y_Pixel = CInt(TrouverChaineCompriseEntre(LignesFichierGeoref(3 * Boucle + 5), Cpt, ":"c))
            Next
            'remplir toutes les variables associées aux coordonnées UTM de la structure Georef de 4 coins
            TrouverNbZonesUtm()
            _LargeurPixel = Coins(2).X_Pixel
            _HauteurPixel = Coins(2).Y_Pixel
            _LargeurOctets = StrideImage(_LargeurPixel)
            HauteurReserve = HauteurPixel
            'pour les cartes issues d'une capture d'écran on peut faire un monde virtuel (échelle)
            'pour les autres l'origine du monde virtuel est le pt(0) car on ne peut pas faire d'échelle (positionnement relatif des cartes les unes par rapport au autres)
            If SystemCartographique.IsCapture Then
                If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille Then
                    PointOrigne = New Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins(0).Grille))
                Else
                    PointOrigne = New Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins(0).LatLon))
                End If
            End If
            _RegionVirtuelle = New Rectangle(New Point(PointOrigne), New Size(_LargeurPixel, _HauteurPixel))
            'on calcule divers paramètres afin de connaître l'échelle de la carte et les DPI correspondants
            TrouverDPIImpression()
            Ret = True
        Catch Ex As Exception
            AfficherErreur(Ex, "A3A1")
        End Try
        Return Ret
    End Function
    ''' <summary> Ouvre un fichier de type GEOREF et vérifie un certain nombre de paramètres avant de le déclarer valide. 
    ''' notatement en vérifiant que le système carto du fichier est compatible avec le système passé en paramètre </summary>
    ''' <param name="LignesFichierGeoref">Les lignes de texte du fichier de type GEOREF de la carte</param>
    ''' <param name="SystemeCarto">Données concernant le système géographique à verifier dan le fichier georef, normalement celui d'une échelle</param>
    ''' <returns>VerifSystemeGeographique.Ok si le fichier GEOREF est valide ou VerifSystemeGeographique.xx .</returns>
    Private Function VerifierGeoref(LignesFichierGeoref() As String, ByRef SystemeCarto As SystemeCartographique) As VerificationRenvoiCarte
        'Si ce n'est pas le bon fichier (2 formats GEOREF cohabite)
        If LignesFichierGeoref(0).Chars(1) <> "C"c Then Return VerificationRenvoiCarte.Georef_Non_Supporte
        Dim Ret As VerificationRenvoiCarte = VerificationRenvoiCarte.OK
        'verification communes aux 2 manières (création echelle et ajout carte)
        Dim Cpt As Integer
        'Trouve le nom du fichier de l'image associée à la carte sur la 1ère ligne du fichier georef
        CheminFluxLecture = TrouverChaineCompriseEntre(LignesFichierGeoref(0), Cpt, ":"c, ","c)
        'Est ce que fichier graphique existe dans le même répertoire que le fichier GEOREF
        If Not File.Exists(If(CheminCarte = "", "", CheminCarte & "\") & CheminFluxLecture) Then Return VerificationRenvoiCarte.Carte_Inexistante
        Dim SiteCarto As String = Nothing, Projection As String = Nothing, Echelle As String = Nothing, Z As Integer = 0, H As Char = Nothing, Interpol As Boolean = False
        TrouverElementsSystemeCartographique(LignesFichierGeoref, SiteCarto, Echelle, Projection, Z, H, Interpol)
        If Interpol Then
            Ret = VerificationRenvoiCarte.Carte_Retaillee
        Else
            SystemeCarto = New SystemeCartographique(SiteCarto, Echelle, Projection, Z, H)
            If Not SystemeCarto.IsOk Then Ret = VerificationRenvoiCarte.Erreur_Creation_Systeme
        End If
        Return Ret
    End Function
    ''' <summary> renvoie les 6 élements qui permettent de créer un système cartograhique à partir d'un fichier georef </summary>
    ''' <param name="LignesFichierGeoref">textez du fichier georef</param>
    ''' <param name="SiteCarto">chaine qui contiendra le libellé du site carto</param>
    ''' <param name="ClefEchelle">chaine qui contiendra la cle de l'echelle de la carte</param>
    ''' <param name="LibelleProjection">chaine qui contiendra le libellé de la projection</param>
    ''' <param name="Hemis">char qui contiendra l'hemisphère de la projection si celle ci est UTM</param>
    ''' <param name="Zone">long qui contiendra le numéro de zone de la projection si celle ci est UTM</param>
    ''' <param name="Interpol">boolean qui indiquera si la carte est interpolée</param>
    Private Shared Sub TrouverElementsSystemeCartographique(LignesFichierGeoref() As String, ByRef SiteCarto As String,
                        ByRef ClefEchelle As String, ByRef LibelleProjection As String, ByRef Zone As Integer, ByRef Hemis As Char, ByRef Interpol As Boolean)
        Dim Cpt As Integer = LignesFichierGeoref(0).IndexOf("."c) + 1
        'Trouve le système de capture de la carte
        SiteCarto = TrouverChaineCompriseEntre(LignesFichierGeoref(0), Cpt, ":"c)
        'Trouve l'échelle de capture de la carte sur la deuxième ligne du fichier georef
        Cpt = 0
        ClefEchelle = TrouverChaineCompriseEntre(LignesFichierGeoref(1), Cpt, ":"c, ","c)
        'Trouve le type de coordonnées capturées de la carte sur la deuxième ligne du fichier georef
        Cpt = LignesFichierGeoref(1).IndexOf(", C")
        LibelleProjection = TrouverChaineCompriseEntre(LignesFichierGeoref(1), Cpt, ":"c)
        Zone = 0
        Hemis = Nothing
        Interpol = True
        If TrouverChaineCompriseEntre(LignesFichierGeoref(2), 0, ":"c, ","c) = "Non" Then Interpol = False
        If LibelleProjection.Chars(0) = "U"c Then 'si il s'agit d'une projection UTM il faut trouver la zone et l'hemisphère
            Dim CH() As String = LibelleProjection.Split(" "c)
            If CH.Length > 2 Then
                LibelleProjection = CH(0) & " " & CH(1)
                Hemis = CH(2)(0)
                Zone = CInt(CH(3))
            End If
        End If
    End Sub
    ''' <summary> vérifie la faisabilité d'associer un tampon à une carte et charge l'image si besoin</summary>
    ''' <remarks>ReserveSource ou interpole doit être initialisé avant d'appeler cette procédure lorqu'il s'agit d'un support fichier</remarks>
    Private Function AssocierTampon() As VerificationRenvoiCarte
        ReserverMemoireTampon("Reserve Carte Base")
        If TamponType <> TypeSupportCarte.Fichier Then
            Tampon = New EditableBitmap(CheminFluxLecture)
            If Not Tampon.IsOk Then Return VerificationRenvoiCarte.Erreur_Creation_Tampon
        Else
            'la hauteur max du tampon de travail dépend de la taille effective de la réserve
            HauteurReserve = TailleReserve \ _LargeurOctets
            'avec un support de carte fichier on a besoin d'un tampon spécifique 
            Tampon = New EditableBitmap(_LargeurPixel, HauteurReserve, _Reserve, "Tampon Base") 'on dimensionne le nouveau tampon
            If Not Tampon.IsOk Then Return VerificationRenvoiCarte.Erreur_Creation_Tampon
            'si le support est Fichier, l'image de la carte doit être transformée en .bin et le fichier indiquer dans CheminFluxLecture
            'transformation de l'image de la carte en fichier bin
            Dim Dst As String = CheminEnregistrementProvisoire & "\" & "Carte.raw"
            If Format Is ImageFormat.Bmp Then 'pas d'utilisation superflue de mémoire
                If Not FichierSrcToDst(Dst, False) Then Return VerificationRenvoiCarte.Erreur_Chargement_Image
            Else
                If Not FichierFormatsToRaw(Dst) Then Return VerificationRenvoiCarte.Erreur_Chargement_Image
            End If
            CheminFluxLecture = Dst
        End If
        Return VerificationRenvoiCarte.OK
    End Function
    ''' <summary> reserve des octets en mémoire qui serviront pour le tampon (image de la carte) ou pour l'écriture dans un fichier </summary>
    Private Function ReserverMemoireTampon(NomReserve As String) As Boolean
        'Si egal à 0 cela indique une demande de Capturer ou de Convertir
        If _TailleReserve = 0 Then
            'pour le support de tampon type fichier le plus grand besoin se situe au niveau du découpage des tuiles du fichier tuile soit une hauteur mini de 256,512 ou 1024
            DimensionsTuile = If(DemandeFichiersTuiles > ChoixFichiersTuiles.Aucun, CartesSettings.DIMENSIONS_TUILE, NbPixelsTuile)
            'ce qui limite pour le support fichier à 682 tuiles sur l'axe des tuiles sinon on dépasse TailleMaxTampon
            HauteurReserve = If(TamponType = TypeSupportCarte.Fichier, DimensionsTuile, _HauteurPixel)
            'on détermine d'abord si il y a un interpolation avec une carte suisse car il faut réserver plus de mémoire pour le support fichier
            Dim InterpolerCarteSuisse As Boolean = SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile AndAlso DemandeFichiersTuiles > ChoixFichiersTuiles.ORUX
            'pour une interpolation de carte suisse il faut que la réserve puisse contenir le maximum de la hauteur de l'image
            _TailleReserve = _LargeurOctets * If(InterpolerCarteSuisse, Math.Max(HauteurReserve, _HauteurPixel), HauteurReserve)
            'mais on limite à tailleMaxTampon d'où la limite sur le nb de colonnes à 410 tuiles serveur car la hauteur n'à pas d'influence
            'en effet plus il y a de largeur et plus le nb de ligne source est important pour pouvoir réaliser l'interpolation
            If _TailleReserve > TailleMaxTampon Then _TailleReserve = TailleMaxTampon
        End If
        'on reserve la mémoire.
        _Reserve = New SharedPinnedByteArray(_TailleReserve, 1, Nothing, NomReserve)
        If _Reserve.IsOk Then Reserve.ClearColor(PartagerSettings.COULEUR_CARTE)
        Return _Reserve.IsOk
    End Function
#End Region
End Class