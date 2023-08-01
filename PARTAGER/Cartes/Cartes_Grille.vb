Partial Friend Class Carte
#Region "Procédures associées aux grilles d'une carte"
    ''' <summary>Permet de dessiner la grille sur l'image de la carte pour 1 des 3 zones max.</summary>
    Private Function AfficherGrille() As Boolean
        'si l'échelle est trop petite pour afficher une grille kilométrique on sort sans générer d'erreur 
        If Not SystemCartographique.AfficherGrilleIsOK Then Return True
        'si la grille est déja dessinnée on passe son chemin sans renvoyer d'erreur
        If SystemCartographique.GrilleExiste = True Then Return True
        AfficherVisueInformation($"{NomComplet} :{Separateur}Affichage de la grille sur la carte")
        If Not CalculerGrille() Then Return False 'c'est une erreur
        AfficherGrille = False
        Try
            Dim HauteurTravail = If(HauteurReserve > HauteurPixel, HauteurPixel, HauteurReserve)
            If TamponType = TypeSupportCarte.Fichier Then
                Using FichierBIN As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                    Dim TailleTravail As Integer = LargeurOctets * HauteurTravail, HauteurALire As Integer
                    CheminFluxLecture = CheminEnregistrementProvisoire & "\CarteAvecGrille.raw"
                    Using FichierAvecGrille As New FileStream(CheminFluxLecture, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, TailleTravail)
                        Do
                            'on charge une partie de la carte sans grille dans le tampon
                            FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTravail)
                            'on dessinne la grille
                            For Cpt As Integer = 0 To NbZonesUTM - 1
                                If Not AfficherGrilleZones(Cpt, HauteurTravail, HauteurALire) Then Exit Try 'il peut y avoir une erreur
                            Next
                            'on enregistre la carte avec la grille
                            FichierAvecGrille.Write(Tampon.Bits, Tampon.IndexBits, TailleTravail)
                            HauteurALire += HauteurReserve
                            If HauteurALire < HauteurPixel And HauteurALire + HauteurReserve > HauteurPixel Then
                                TailleTravail = LargeurOctets * (HauteurPixel - HauteurALire)
                            End If
                        Loop While HauteurALire < HauteurPixel
                    End Using
                End Using
            Else ' cas du suppport de carte en mémoire. Il n'y a qu'une seule tranche donc pas de décalage pour le dessin et on travaille sur la carte entière
                For Cpt As Integer = 0 To NbZonesUTM - 1
                    If Not AfficherGrilleZones(Cpt, HauteurTravail, 0) Then Exit Try 'il peut y avoir une erreur
                Next
            End If
            GrilleIsAffiche = True
            AfficherGrille = True
        Catch Ex As Exception
            AfficherErreur(Ex, "O9A5")
        End Try
    End Function
    ''' <summary>Permet de dessiner la grille sur l'image de la carte pour 1 des 3 zones max.</summary>
    ''' <param name="Zone_UTM">1 à 3 si la carte est à cheval sur 3 zones UTM</param>
    ''' <remarks>normalement cette procédure n'est pas appelé directement</remarks>
    Private Function AfficherGrilleZones(Zone_UTM As Integer, HauteurTravail As Integer, Optional DecalY As Integer = 0) As Boolean
        AfficherGrilleZones = False
        Try
            'Pour dessiner avec 2 largeurs différentes
            Dim Pb_1 As Pen, Pb_2 As Pen, Decalage As New Size(0, -DecalY)
            'pour l'échelle d'impression "025" et l'échelle de capture "007" on élargi la taille
            If SystemCartographique.SystemDoubleGrille() Then
                Pb_1 = New Pen(Color.Blue, 2)
                Pb_2 = New Pen(Color.Blue, 4)
            Else    'environ 190 dpi pour toutes les autres échelles
                Pb_1 = New Pen(Color.Blue, 1)
                Pb_2 = New Pen(Color.Blue, 2)
            End If
            'On indique que l'on travaille avec la carte en cours, si l'image provient d'une carte existante on est obligé de recréer un bitmap car le bitmap existant est en lecture seul
            Using Carte_GrilleGraphic As Graphics = Graphics.FromImage(If(Tampon.IsBitmap, New Bitmap(LargeurPixel, HauteurTravail, LargeurOctets, FormatPixel, ImageBitPtr), Image))
                With Grilles(Zone_UTM)
                    Carte_GrilleGraphic.PageUnit = GraphicsUnit.Pixel
                    Carte_GrilleGraphic.SmoothingMode = SmoothingMode.AntiAlias
                    Carte_GrilleGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality
                    'Trace les méridiens multiples de 6° qui délimitent les zones UTM pour les zones N°2 et N°3
                    If Zone_UTM > 0 Then
                        Carte_GrilleGraphic.DrawLine(Pb_2, .ClipZoneDeb, 0, .ClipZoneDeb, HauteurTravail)
                    End If
                    'définit la surface sur laquelle on peut dessiner (pixel par rapport à  ceux de l'image
                    Dim ClipRect As New Rectangle(.ClipZoneDeb, 0, .ClipZoneFin - .ClipZoneDeb, HauteurTravail)
                    Carte_GrilleGraphic.SetClip(ClipRect)
                    'Pour chaque ligne sur l'axe des X on la trace sur la carte les segments de droite représentants les lignes verticales de la grilles
                    For BoucleX As Integer = 0 To UBound(.Points, 1)
                        For BoucleY As Integer = 0 To UBound(.Points, 2) - 1
                            'on desinne les droites entre 2 points de la grille
                            Carte_GrilleGraphic.DrawLine(Pb_1, Point.Add(.Points(BoucleX, BoucleY), Decalage), Point.Add(.Points(BoucleX, BoucleY + 1), Decalage))
                        Next
                    Next
                    'Pour chaque ligne sur l'axe des Y on la trace sur la carte les segments de droite représentants les lignes horizontales de la grilles
                    For BoucleY As Integer = 0 To UBound(.Points, 2)
                        For BoucleX As Integer = 0 To UBound(.Points, 1) - 1
                            'on desinne les droites entre 2 points de la grille
                            Carte_GrilleGraphic.DrawLine(Pb_1, Point.Add(.Points(BoucleX, BoucleY), Decalage), Point.Add(.Points(BoucleX + 1, BoucleY), Decalage))
                        Next
                    Next
                End With
            End Using
            AfficherGrilleZones = True
        Catch Ex As Exception
            AfficherErreur(Ex, "N2X2")
        End Try
    End Function
    ''' <summary>Permet d'ajouter les étiquettes de la grille sur l'image de la carte. Cette procédure gère les grilles à cheval sur plusieurs zones </summary>
    Private Function AjouterReferences() As Boolean
        If Not SystemCartographique.AfficherGrilleIsOK Then
            GrilleIsAffiche = False
            Return True 'on sort sans erreur mais le fichier des références grille ne sera pas généré car l'échelle est trop petite.
        End If
        AjouterReferences = False
        'mettre à jour la fenêtre d'information
        AfficherVisueInformation($"{NomComplet} :{Separateur}Ajout des références de la grille")
        If Grilles Is Nothing Then 'on ne recalcule pas si cela a déjà été fait pour le dessin
            If Not CalculerGrille() Then Return False 'erreur dans le calcul de la grille
        End If
        Try
            Dim HauteurTravail = If(HauteurReserve > HauteurPixel, HauteurPixel, HauteurReserve)
            Dim NomFichierCarte As String = $"{CheminCarte}\{NomComplet & "_R"}"
            If TamponType = TypeSupportCarte.Fichier Then
                Using FichierBIN As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                    Dim TailleTravail As Integer = LargeurOctets * HauteurTravail, HauteurALire As Integer
                    'On enregistre la carte avec les references directement au bon endroit mais en format raw
                    CheminFluxLecture = CheminEnregistrementProvisoire & "\CarteAvecGrilleReference.raw"
                    Using FichierAvecReference As New FileStream(CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None, TailleTravail)
                        Do
                            'on charge une partie de la carte sans grille dans le tampon
                            FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTravail)
                            'on dessinne la grille
                            For Cpt As Integer = 0 To NbZonesUTM - 1
                                If Not AjouterReferencesZones(Cpt, HauteurALire, HauteurTravail) Then Exit Try 'il peut y avoir une erreur
                            Next
                            'on enregistrer la carte avec la grille
                            FichierAvecReference.Write(Tampon.Bits, Tampon.IndexBits, TailleTravail)
                            HauteurALire += HauteurTravail
                            If HauteurALire < HauteurPixel And HauteurALire + HauteurTravail > HauteurPixel Then
                                TailleTravail = LargeurOctets * (HauteurPixel - HauteurALire)
                            End If
                        Loop While HauteurALire < HauteurPixel
                    End Using
                End Using
                If ToutFormatCarte AndAlso Image.Width <= MaxPixelsJpeg AndAlso Image.Height <= MaxPixelsJpeg Then
                    'on sauvegarde toujours en JPEG sauf dépassement des capacités de l'encodeur Jpeg de .net
                    If Not FichierRawToFormats(NomFichierCarte & ".Jpeg", True) Then Exit Try
                Else
                    'on sauvegarde toujours en BMP
                    If Not FichierSrcToDst(NomFichierCarte & ".Bmp", True) Then Exit Try
                End If
            Else ' cas du suppport de carte en mémoire. Il n'y a qu'une seule tranche donc pas de décalage pour le dessin et on travaille sur la carte entière
                For Cpt As Integer = 0 To NbZonesUTM - 1
                    If Not AjouterReferencesZones(Cpt, 0, HauteurTravail) Then Exit Try
                Next
                'on sauvegarde toujours en JPEG sauf dépassement des capacités de l'encodeur Jpeg de .net
                If Image.Width > MaxPixelsJpeg OrElse Image.Height > MaxPixelsJpeg Then
                    Image.Save(NomFichierCarte & ".bmp", ImageFormat.Bmp)
                Else
                    Image.Save(NomFichierCarte & ".Jpeg", EncodeurJpeg, QualiteJPEG(PartagerSettings.QUALITE_JPEG))
                End If
            End If
            AjouterReferences = True
        Catch Ex As Exception
            AfficherErreur(Ex, "B2Y3")
        End Try
    End Function
    ''' <summary> calcul et dessine les références des grilles (Etiquettes des lignes et nom de la grille) </summary>
    ''' <param name="Zone">Numéro de la zone UTM de 0 à 2</param>
    Private Function AjouterReferencesZones(Zone As Integer, DecalTrancheY As Integer, HauteurTravail As Integer) As Boolean
        AjouterReferencesZones = False
        Try
            Dim Pb_1 As New Pen(Color.Blue, 2), Pb_2 As New Pen(Color.Blue, 4)
            'Pour recevoir la police des étiquettes. permet d'adapter la taille du texte en fonction de l'échelle d'impression et de l'échelle de capture
            Dim F As New Font("Microsoft Sans Serif", 30 * (DPIImpressionX / SystemCartographique.MaxDPIImpression), FontStyle.Bold, GraphicsUnit.Pixel)
            'si l'image provient d'une carte existante on est obligé de recréer un bitmap car le bitmap existant est en lecture seule
            Using SourceGraphic As Graphics = Graphics.FromImage(If(Tampon.IsBitmap, New Bitmap(LargeurPixel, HauteurTravail, LargeurOctets, FormatPixel, ImageBitPtr), Image))
                With Grilles(Zone)
#Region "Ecriture des étiquettes sur l'axe des X"
                    'définit la surface sur laquelle on peut dessiner
                    Dim ClipRect As New Rectangle(.ClipZoneDeb, 0, .ClipZoneFin - .ClipZoneDeb, HauteurTravail)
                    SourceGraphic.SetClip(ClipRect)
                    'pour chaque ligne de la grille sur l'axe des X (vertical)
                    For Boucle As Integer = .MinX To .MaxX
                        'On trouve le texte de l'étiquette en km et une décimale pour l'échelle 1/4000
                        Dim Etiquette = (Boucle * SystemCartographique.Niveau.PasGrilleNumeric / 1000).ToString
                        'on regarde la dimension de l'étiquette avec la police choisie (taille constante quelque soit l'échelle d'impression)
                        Dim S = SourceGraphic.MeasureString(Etiquette, F)
                        'le Y correspond à la hauteur de la carte (attention les Y du monde virtuel sont inversés par rapport aux y d'une image - la hauteur de l'étiquette
                        Dim YEtiquette As Single = HauteurPixel
                        'Le X en pixel correspondant pour le haut de la carte (sud)
                        Dim X1 = .Points(Boucle - .MinX, 1).X
                        Dim Y1 = .Points(Boucle - .MinX, 1).Y
                        Dim X2 = .Points(Boucle - .MinX, 0).X
                        Dim Y2 = .Points(Boucle - .MinX, 0).Y
                        Dim XEtiquette = X1 + (X2 - X1) * (Y1 - YEtiquette) / (Y1 - Y2)
                        YEtiquette = YEtiquette - DecalTrancheY - S.Height
                        'on dessine un rectangle contenant la taille de l'étiquette
                        SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette - S.Width / 2, YEtiquette, S.Width, S.Height)
                        'on ecrit l'étiquette par dessus
                        SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette - S.Width / 2, YEtiquette)
                        'Y correspondant du nord de la carte toujours  0
                        YEtiquette = 0
                        'Le X en pixel correspondant pour le haut de la carte (nord)
                        X1 = .Points(Boucle - .MinX, .MaxY - .MinY - 1).X
                        Y1 = .Points(Boucle - .MinX, .MaxY - .MinY - 1).Y
                        X2 = .Points(Boucle - .MinX, .MaxY - .MinY).X
                        Y2 = .Points(Boucle - .MinX, .MaxY - .MinY).Y
                        XEtiquette = X1 + (X2 - X1) * (Y1 - YEtiquette) / (Y1 - Y2)
                        YEtiquette = -DecalTrancheY
                        'on dessine un rectangle contenant la taille de l'étiquette
                        SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette - S.Width / 2, YEtiquette, S.Width, S.Height)
                        'on ecrit l'étiquette par dessus
                        SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette - S.Width / 2, YEtiquette)
                    Next
#End Region
#Region "Ecriture des étiquettes sur l'axe des Y"
                    'pour chaque ligne de la grille sur l'axe des Y (horizontal) si la zone est concernée
                    If Zone = 0 Or Zone = NbZonesUTM - 1 Then
                        For Boucle As Integer = .MinY To .MaxY
                            'On trouve l'étiquette en km
                            Dim Etiquette = (Boucle * SystemCartographique.Niveau.PasGrilleNumeric / 1000).ToString
                            'on regarde la dimension de l'étiquette avec la police choisie (taille constante quelque soit l'échelle d'impression)
                            Dim S = SourceGraphic.MeasureString(Etiquette, F)
                            'si il s'agit de la première zone (0) on met les étiquette coté ouest
                            If Zone = 0 Then
                                'le X correspondant toujours 0 pour l'ouest
                                Dim XEtiquette = 0F
                                'Le Y en pixel correspondant pour le coté gauche de la carte (ouest)
                                'pour l'interpolation, les lignes verticales n'étant pas forcément verticale on est obligé de tester si le X du 2ème point est bien 
                                'sur la carte
                                Dim DecaleY = If(.Points(1, Boucle - .MinY).X < 0, 1, 0)
                                Dim X1 = .Points(0 + DecaleY, Boucle - .MinY).X
                                Dim Y1 = .Points(0 + DecaleY, Boucle - .MinY).Y
                                Dim X2 = .Points(1 + DecaleY, Boucle - .MinY).X
                                Dim Y2 = .Points(1 + DecaleY, Boucle - .MinY).Y
                                Dim YEtiquette = Y1 + (Y2 - Y1) * (X1 - XEtiquette) / (X1 - X2) - DecalTrancheY
                                'on dessine un rectangle contenant la taille de l'étiquette
                                SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette, YEtiquette - S.Height / 2, S.Width, S.Height)
                                'on ecrit l'étiquette par dessus
                                SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette, YEtiquette - S.Height / 2)
                            End If
                            'si il s'agit de la dernière zone on met les étiquettes coté Est
                            If Zone = NbZonesUTM - 1 Or SystemCartographique.GrilleExiste = True Then
                                'le X correspond toujours la largeur de la carte
                                Dim XEtiquette As Single = _LargeurPixel
                                'Le Y en pixel correspondant pour le coté droit de la carte (est)
                                'pour l'interpolation, les lignes verticales n'étant pas forcément verticale on est obligé de tester si le X du 1er point est bien
                                'sur la carte
                                Dim DecaleY = If(.Points(.MaxX - .MinX - 1, Boucle - .MinY).X > _LargeurPixel, 1, 0)
                                Dim X1 = .Points(.MaxX - .MinX - 1 - DecaleY, Boucle - .MinY).X
                                Dim Y1 = .Points(.MaxX - .MinX - 1 - DecaleY, Boucle - .MinY).Y
                                Dim X2 = .Points(.MaxX - .MinX - DecaleY, Boucle - .MinY).X
                                Dim Y2 = .Points(.MaxX - .MinX - DecaleY, Boucle - .MinY).Y
                                Dim YEtiquette = Y1 + (Y2 - Y1) * (X1 - XEtiquette) / (X1 - X2) - DecalTrancheY
                                'on dessine un rectangle contenant la taille de l'étiquette
                                SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette - S.Width, YEtiquette - S.Height / 2, S.Width, S.Height)
                                'on ecrit l'étiquette par dessus
                                SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette - S.Width, YEtiquette - S.Height / 2)
                            End If
                        Next
                    End If
#End Region
#Region "Ecriture du nom de grille"
                    'on écrit la référence de la grille dans le coin haut gauche de la carte
                    Dim EtiquetteGrille = SystemCartographique.GrilleNom
                    Dim YEtiquetteGrille = 0F + DecalTrancheY
                    'si il s'agit d'un grille UTM on rajoute la zone
                    If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD Then
                        EtiquetteGrille &= $" {Coins(0).Hemisphere_UTM}{(Coins(0).NumZone_UTM + Zone):N0}"
                    End If
                    'on mesure la taille du texte
                    Dim S_Grille = SourceGraphic.MeasureString(EtiquetteGrille & ".", F)
                    'on dessine un rectangle
                    SourceGraphic.FillRectangle(Brushes.Coral, .ClipZoneDeb, YEtiquetteGrille, S_Grille.Width, S_Grille.Height)
                    'on ecrit par dessus
                    SourceGraphic.DrawString(EtiquetteGrille, F, Brushes.Black, .ClipZoneDeb, YEtiquetteGrille)
#End Region
                End With
            End Using
            AjouterReferencesZones = True
        Catch Ex As Exception
            AfficherErreur(Ex, "H9Z5")
        End Try
    End Function
    ''' <summary>Procédure qui définit la surface sur laquelle peut être dessinée la grille en fonction du nb de zones et du numéro de zone
    ''' elle appelle le calcul de la grille et des références</summary>
    ''' <remarks>normalement cette procédure n'est pas appelé directement</remarks>
    Private Function CalculerGrille() As Boolean
        CalculerGrille = False
        Try
            'on dimensionne le receptacle des calculs de grille (1 par zone utm)
            ReDim Grilles(NbZonesUTM - 1)
            'Calcul du méridien qui sépare la zone N°1 de la zone N°2
            Dim MeridienFinZoneUTM As Integer = (Coins(0).NumZone_UTM - 30) * 6
            Dim ZoneFin As Integer = 0
            Dim LongitudeDebZone = Coins(0).Longitude
            Dim LongitudeFinZone As Double
            'Il peut y avoir jusquà 3 zones en France et 2 en Guyanne et 1 en suisse
            For Cpt As Integer = 0 To NbZonesUTM - 1
                'Le début de la zone est égal à la fin de la zone suivante
                Grilles(Cpt) = New Grille() With {.ClipZoneDeb = ZoneFin}
                'Le début de la zone est égal à la fin de la zone suivante
                'si le méridien de la première zone est supèrieur à la longitude de fin de la carte il s'agit de la dernière zone et le clip de fin est
                'égal à la fin de la carte. C'est aussi le cas pour les cartes Suisse qui ne comporte qu'une seule zone
                If MeridienFinZoneUTM > Coins(1).Longitude Then
                    Grilles(Cpt).ClipZoneFin = LargeurPixel
                    LongitudeFinZone = Coins(1).Longitude
                Else
                    'sinon on calcule la fin de la zone exprimée en pixel. Les longitudes étant de la forme ax+b on peut se contenter de faire une règle de 3
                    Grilles(Cpt).ClipZoneFin = CInt((MeridienFinZoneUTM - Coins(0).Longitude) * LargeurPixel / (Coins(1).Longitude - Coins(0).Longitude))
                    LongitudeFinZone = MeridienFinZoneUTM
                End If
                Dim MeridienMilieuZone = MeridienFinZoneUTM - 3
                Dim IsOverMiddleZoneUtm = LongitudeDebZone < MeridienMilieuZone AndAlso LongitudeFinZone > MeridienMilieuZone
                'Calcul des lignes X et Y de la grille de la zone UTM N°1 et plus. il peut y avoir des erreurs
                If Not CalculerGrilleZones(Cpt, MeridienMilieuZone, IsOverMiddleZoneUtm) Then Exit Try 'il peut y avoir une erreur
                'on oublie pas de passer à la zone suivante
                MeridienFinZoneUTM += 6
                ZoneFin = Grilles(Cpt).ClipZoneFin
                LongitudeDebZone = LongitudeFinZone
            Next
            CalculerGrille = True
        Catch Ex As Exception
            AfficherErreur(Ex, "T5Y0")
        End Try
    End Function
    ''' <summary>Calcule les lignes qui composent la grille UTM qui sera dessinée sur la carte géo-référencée
    ''' réservé pour les carte sans grille donc issues des données IGN</summary>
    ''' <param name="Zone_UTM">Numéro de la zone à calculer (de 1 à 3)</param>
    ''' <param name="MeridienMilieuZone"> Méridien du milieu de la zone utm (-3, 3, 9 pour les 3 zones utm de la france) </param>
    ''' <param name="IsOverMiddleZoneUtm"> La zone de capture est à cheval sur le méridien de milieu de zone </param>
    Private Function CalculerGrilleZones(Zone_UTM As Integer, MeridienMilieuZone As Integer, IsOverMiddleZoneUtm As Boolean) As Boolean
        Dim PasGrille As Integer = SystemCartographique.Niveau.PasGrilleNumeric
        CalculerGrilleZones = False
        Try
            With Grilles(Zone_UTM)
                If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille Then
#Region "SystemCartographique.GrilleExiste = True"
                    'Il s'agit de la grille Suisse
                    'On trouve le minimum et le maximum sur chaque axe en multiple du pas de la grille
                    .MinX = CInt(Math.Truncate(Math.Min(Coins(0).X_Grid, Coins(3).X_Grid) / PasGrille))
                    .MaxX = CInt(Math.Truncate(Math.Max(Coins(1).X_Grid, Coins(2).X_Grid) / PasGrille + 1))
                    .MinY = CInt(Math.Truncate(Math.Min(Coins(3).Y_Grid, Coins(2).Y_Grid) / PasGrille))
                    .MaxY = CInt(Math.Truncate(Math.Max(Coins(0).Y_Grid, Coins(1).Y_Grid) / PasGrille + 1))
                    ReDim .Points(.MaxX - .MinX, .MaxY - .MinY)
                    'On calcule les coordonnées Géodésique WGS84 de chaque point de la grille
                    For BoucleX As Integer = .MinX To .MaxX
                        For BoucleY As Integer = .MinY To .MaxY
                            Dim Grille As New PointProjection(New PointD(BoucleX * PasGrille, BoucleY * PasGrille))
                            If SystemCartographique.IsInterpol Then
                                'si la carte est retaillée il faut passer par les DD à partir des coordonnées de la grille
                                Dim LatLon As PointD = ConvertProjectionToWGS84(SystemCartographique.Projection.Datum, Grille)
                                If LatLon.IsEmpty Then Exit Try
                                'calculer les pixels avec les fonctions du systemecartographique propre à chaque carte
                                .Points(BoucleX - .MinX, BoucleY - .MinY) = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(LatLon)
                            Else
                                'calculer les pixels correspondants aux LL à partir des fonctions coordonnées to pixel de la carte
                                .Points(BoucleX - .MinX, BoucleY - .MinY) = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Grille.Coordonnees)
                                'pour les cartes non interpolées il faut passer du système virtuel à l'image
                                .Points(BoucleX - .MinX, BoucleY - .MinY) -= PointOrigne
                            End If
                        Next
                    Next
#End Region
                Else
#Region "SystemCartographique.GrilleExiste = False"
                    'On trouve le minimum et le maximum sur chaque axe en multiple du pas de la grille UTM
                    .MinX = CInt(Math.Truncate(Math.Min(Coins(0).UTMs(Zone_UTM).X, Coins(3).UTMs(Zone_UTM).X) / PasGrille))
                    .MaxX = CInt(Math.Truncate(Math.Max(Coins(1).UTMs(Zone_UTM).X, Coins(2).UTMs(Zone_UTM).X) / PasGrille + 1))
                    .MaxY = CInt(Math.Truncate(Math.Max(Coins(0).UTMs(Zone_UTM).Y, Coins(1).UTMs(Zone_UTM).Y) / PasGrille + 1))
                    If IsOverMiddleZoneUtm Then
                        'surface de capture à cheval sur le méridien de milieu de zone UTM. Le milieu de la zone donne le Y le plus bas 
                        Dim PointUtmZone2 = ConvertLLToUtm(New PointD(MeridienMilieuZone, Coins(2).Latitude),
                                                           Datums.WGS84, Coins(2).NumZone_UTM + Zone_UTM)
                        .MinY = CInt(Math.Truncate(PointUtmZone2.Y / PasGrille))
                    Else
                        'surface de capture sur une moitié de zone UTM
                        .MinY = CInt(Math.Truncate(Math.Min(Coins(3).UTMs(Zone_UTM).Y, Coins(2).UTMs(Zone_UTM).Y) / PasGrille))
                    End If
                    ReDim .Points(.MaxX - .MinX, .MaxY - .MinY)
                    'On calcule les coordonnées Géodésique WGS84 de chaque point de la grille car le géoréférencement est en LL sur GF et DT
                    For BoucleX As Integer = .MinX To .MaxX
                        For BoucleY As Integer = .MinY To .MaxY
                            'calcul de la longitude, latitude à partir des coordonnées UTM
                            Dim Grille As New PointProjection(New PointD(BoucleX * PasGrille, BoucleY * PasGrille), Coins(0).NumZone_UTM + Zone_UTM, Coins(0).Hemisphere_UTM)
                            Dim LatLon = ConvertUtmToLL(Datums.UTM_WGS84, Grille)
                            If LatLon.IsEmpty Then Exit Try
                            'conversion des DD en Pixel.
                            .Points(BoucleX - .MinX, BoucleY - .MinY) = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(LatLon)
                            'pour les cartes non interpolées il faut passer du système virtuel à l'image, pour les cartes interpolée on est déja ramené à l'image
                            .Points(BoucleX - .MinX, BoucleY - .MinY) -= PointOrigne
                        Next
                    Next
#End Region
                End If
            End With
            CalculerGrilleZones = True
        Catch Ex As Exception
            AfficherErreur(Ex, "E0W8")
        End Try
    End Function
#End Region
End Class