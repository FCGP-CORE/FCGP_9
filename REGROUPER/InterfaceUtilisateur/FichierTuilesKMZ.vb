Imports FCGP.NiveauDetailCartographique
Imports FCGP.Regroupement
Imports FCGP.Regrouper

''' <summary> permet la création de fichier tuiles KMZ à 1niveau de détails ou échelle de capture de taille plus importante que capture ou convertir </summary>
Friend Class FichierTuilesKMZ
    Private TailleReserveMini As Integer
    Private TypeCoordEncours As CoordonneesGeoreferencements, _RegroupementEncours As Regroupement
    Private SystemeCartoEncours As SystemeCartographique, NumSiteEncours As Integer
    Private Const NbTuilesMaxi As Integer = 3000
    Private SelectionEncours, SelectionModifie As Rectangle
    Private NbTuilesX, NbTuilesY, NbTuilesTotal, DimTuile As Integer, MessInfo, CheminTuile, Coins() As String, FlagEvenement As Boolean
    Private Datum As Datums, SiteCarto As SitesCartographiques, Echelle As Echelles, IndiceEchelle As Integer
    Private TitreMessageInformation As String
    Friend WriteOnly Property RegroupementEncours As Regroupement
        Set(value As Regroupement)
            _RegroupementEncours = value
        End Set
    End Property
    Private Sub FichierTuilesKMZ_Load(sender As Object, e As EventArgs)
        TitreMessageInformation = TitreInformation
        'on indique au module carte le label où écrire les messages concernant le travail en cours
        LabelInformation = Information
        TitreInformation = "Création des fichiers Kmz"
        SystemeCartoEncours = _RegroupementEncours.SystemeType
        TypeCoordEncours = _RegroupementEncours.SystemeType.CoordonneesGeoreferencement
        NumSiteEncours = _RegroupementEncours.NumSite
        SiteCarto = SystemeCartoEncours.SiteCarto
        Datum = DatumSiteWeb(SiteCarto)
        IndiceEchelle = SystemeCartoEncours.IndiceEchelle
        Echelle = SiteIndiceEchelleToEchelle(SiteCarto, IndiceEchelle)
        FlagEvenement = True
        InitialiserValeurs()
        MettreAJourDimensions()
        CalculerSelectionReel()
        CheminTuile = PartagerSettings.CHEMIN_TUILE
        CalculerNbTuilesKMZ()
        FlagEvenement = False
        Quitter.Select()
    End Sub
    Private Sub FichierTuilesKMZ_FormClosed(sender As Object, e As FormClosedEventArgs)
        If SelectionEncours <> SelectionModifie Then
            _RegroupementEncours.SelectionGeorefToSelectionReferencement(SelectionModifie)
        End If
        TitreInformation = TitreMessageInformation
    End Sub
    ''' <summary> ajuste la couleur du texte et calcul la sélection réelle et le poids de la sélection </summary>
    Private Sub CorDimension_ValueChanged(sender As Object, e As EventArgs)
        If Not FlagEvenement Then
            Dim C As NumericUpDown = CType(sender, NumericUpDown)
            If C.Name = "CorLargeur" Then
                ChangerCouleurText(C, C.Value, SelectionEncours.Width)
            Else
                ChangerCouleurText(C, C.Value, SelectionEncours.Height)
            End If
            CalculerSelectionReel()
            CalculerNbTuilesKMZ()
        End If
    End Sub
    ''' <summary> ajuste la largeur ou la hauteur de la sélection à un nb entier de tuiles </summary>
    Private Sub CorDimension_MouseDoubleClick(sender As Object, e As MouseEventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown)
        If e.X < C.Size.Width - 18 Then
            Dim Value As Integer = CInt(C.Value)
            Dim NewValue As Decimal
            Select Case ModifierKeys
                Case Keys.Shift ' arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                    NewValue = (Value \ DimTuile + 1) * DimTuile
                Case Keys.Control ' arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                    NewValue = (Value \ DimTuile - 1) * DimTuile
                Case Else 'arrondi le nb de pixels à la dimension de la tuile du fichier tuile. Permet de démarrer sur une valeur connue
                    NewValue = CDec(Math.Round(Value / DimTuile) * DimTuile)
            End Select
            If NewValue >= C.Minimum AndAlso NewValue <= C.Maximum Then
                C.Value = NewValue
            End If
        End If
    End Sub
    ''' <summary> déplace le X ou Y de la sélection d'une tuile </summary>
    Private Sub CorLocation_ValueChanged(sender As Object, e As EventArgs)
        If Not FlagEvenement Then
            Dim C As NumericUpDown = CType(sender, NumericUpDown)
            If C.Name = "CorPT0X" Then
                ChangerCouleurText(C, C.Value, SelectionEncours.Location.X)
            Else
                ChangerCouleurText(C, C.Value, SelectionEncours.Location.Y)
            End If
            CalculerSelectionReel()
        End If
    End Sub
    ''' <summary> déplace le X ou Y de la sélection d'une tuile fichier ou arrondi aux tuiles serveur</summary>
    Private Sub CorLocation_MouseDoubleClick(sender As Object, e As MouseEventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown)
        If e.X < C.Size.Width - 18 Then
            Dim Value As Integer = CInt(C.Value)
            Dim NewValue As Decimal
            Select Case ModifierKeys
                Case Keys.Shift 'décale en ajoutant le nb de pixels d'une tuile du fichier tuile
                    NewValue = Value + NbPixelsTuile
                Case Keys.Control 'décale en enlèvant le nb de pixels d'une tuile du fichier tuile
                    NewValue = Value - NbPixelsTuile
                Case Keys.None 'arrondi au pixel de début de la tuile serveur la plus proche. Permet de démarrer sur une valeur connue
                    NewValue = CDec(Math.Round(Value / NbPixelsTuile) * NbPixelsTuile)
            End Select
            If NewValue >= C.Minimum AndAlso NewValue <= C.Maximum Then
                C.Value = NewValue
            End If
        End If
    End Sub
    ''' <summary> change la taille de la mémoire tampon réservée pour la création des fichiers tuiles </summary>
    Private Sub ChoixTailleTampon_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ChoixTailleTampon.SelectedIndex = 0 Then
            TailleReserveMini = TailleMaxTampon \ 3
        ElseIf ChoixTailleTampon.SelectedIndex = 1 Then
            TailleReserveMini = TailleMaxTampon
        Else
            TailleReserveMini = TailleMaxTampon * 2
        End If

        If Not FlagEvenement Then CalculerNbTuilesKMZ()
    End Sub
    ''' <summary> lance la création du ou des fichiers tuiles </summary>
    Private Async Sub Lancer_Click(sender As Object, e As EventArgs)
        If Not VerifierValidation() Then Exit Sub
        Owner.Hide()
        MessageInformation = "Il y a eu une erreur lors de la création de la carte ou du fichier." & CrLf &
                             "La création du fichier Kmz est arrêtée"
        Lancer.Enabled = False
        Quitter.Enabled = False
        Cursor = Cursors.WaitCursor

        RegrouperSettings.DIM_TUILE_KMZ = CInt(N512.Value)
        RegrouperSettings.TAILLE_TAMPON = ChoixTailleTampon.SelectedIndex
        RegrouperSettings.Ecrire()
        'on met à jour la sélection pour prendre en compte une modification
        If SelectionEncours <> SelectionModifie Then
            _RegroupementEncours.SelectionGeorefToSelectionReferencement(SelectionModifie)
            SelectionEncours = _RegroupementEncours.SelectionGeoref
        End If
        'il faut calculer les 4 coins de la carte à construire
        Coins = CalculerCoins()
        Try
            Dim DebutTraitement As Date = Now
            MessInfo = NomCarte.Text & CrLf & "Création de la carte du niveau de détails : " & _RegroupementEncours.SystemeType.ClefEchelle
            Information.Text = MessInfo
            Information.Refresh()
            If Not Await Task.Run(Function() RealiserFichiersTuiles()) Then Exit Try
            MessageInformation = $"Le fichier {NomCarte.Text}.kmz a été créé.{CrLf}Temps de traitement : {TimeSpanToStr(Now - DebutTraitement, False)}"
        Catch Ex As Exception
            AfficherErreur(Ex, "A8T2")
        End Try
        AfficherInformation()
        Owner.Show()
        Close()
    End Sub
    ''' <summary> Réalisation du fichier KMZ demandé en arrière plan </summary>
    Private Function RealiserFichiersTuiles() As Boolean
        RealiserFichiersTuiles = False
        Try
            Dim FichierBinaire As String = CheminEnregistrementProvisoire & "\Fichierbinaire.raw"
            Dim Sys As SystemeCartographique = _RegroupementEncours.SystemeType
            Dim Selection As Rectangle = _RegroupementEncours.SelectionGeoref
            'on calcule le nb d'octets par ligne
            Dim LargeurOctets = StrideImage(Selection.Width)
            'on vérifie que la hauteur de sélection fasse au moins une tuile
            Dim HauteurSelection = If(Selection.Height > DimTuile, Selection.Height, DimTuile) + 1
            'on calcule combien de ligne peut tenir dans le tampon
            Dim HauteurTampon = TailleReserveMini \ LargeurOctets
            'et on prend la plus petite des 2
            If HauteurTampon > HauteurSelection Then HauteurTampon = HauteurSelection
            Dim TailleSelection = LargeurOctets * HauteurTampon
            'Création de la carte du niveau et réserve de la place en mémoire pour le tampon de travail
            Using CarteNiveau As New Carte(Selection.Width, Selection.Height, Sys, FichierBinaire, TailleSelection)
                'si il y a eu une erreur lors de la création de la carte on arrête la création du fichier tuiles
                If Not CarteNiveau.IsOk Then Exit Try
                'il faut assembler l'image de la carte sous forme d'un fichier binaire
                If Not RealiserFichierBinaire(CarteNiveau, Selection, _RegroupementEncours.Cartes) Then Exit Try
                'et réaliser le ou les ficheirs tuiles correspondnants
                If Not FinaliserFichiersTuiles(CarteNiveau) Then Exit Try
                RealiserFichiersTuiles = True
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "6DCB")
        End Try
    End Function
    ''' <summary>  pour chaque niveau de détail il faut créer l'image de la carte sous forme de fichier binaire </summary>
    ''' <param name="FichierBinaire"> chemin complet du fichier binaire qui sera créé </param>
    ''' <param name="R"> regroupement (niveau de détail) en cours </param>
    Private Function RealiserFichierBinaire(CarteEncours As Carte, Selection As Rectangle, Cartes As List(Of Carte)) As Boolean
        RealiserFichierBinaire = False
        Try
            Dim Stride As Integer = CarteEncours.LargeurOctets 'nb d'octets pour une ligne du tampon
            Dim RaR As Rectangle, Pt0 As Point = Selection.Location
            Dim HauteurLue As Integer, HauteurTampon As Integer = CarteEncours.TailleReserve \ Stride
            If HauteurTampon > Selection.Height + 1 Then HauteurTampon = Selection.Height + 1 'hauteur de l'image au maximum 
            Using T As New TamponVisue(New Size(Selection.Width, HauteurTampon), CarteEncours.Reserve, Cartes, "ImageNiveauDetail")
                T.Pt0 = Pt0 'on cale le georef du tampon
                RaR = T.Georef 'sur le début du georef de la carte
                Using FichierBin As New FileStream(CarteEncours.CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None)
                    Do
                        Dim Info = MessInfo & CrLf & HauteurLue & " / " & Selection.Height
                        AfficherVisueInformation(Info)
                        T.RemplirTampon(RaR, PartagerSettings.COULEUR_CARTE)
                        FichierBin.Write(T.Bits, T.IndexBits, HauteurTampon * Stride)
                        HauteurLue += HauteurTampon
                        Pt0.Offset(0, HauteurTampon) 'on décale le Georef du tampon
                        T.Pt0 = Pt0 'pour le prochain tour
                        If HauteurLue + HauteurTampon > Selection.Height Then
                            HauteurTampon = Selection.Height - HauteurLue
                            RaR = New Rectangle(T.Pt0, New Size(T.Georef.Width, HauteurTampon))
                        Else
                            RaR = T.Georef
                        End If
                    Loop While HauteurLue < Selection.Height
                    RealiserFichierBinaire = True
                End Using
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "0693")
        End Try
    End Function ''' <summary> pour chaque niveau de détail il faut soit créer le fichier tuile soit ajouter un niveau au fichier tuile </summary>
    ''' <param name="FichierBinaire"> chemin et nom du fichier binaire contenant l'image de la carte </param>
    ''' <param name="Cpt"> numéro du niveau de détail, de 0 à 4 </param>
    ''' <param name="R"> regroupement (niveau de détail) en cours </param>
    Private Function FinaliserFichiersTuiles(CarteEncours As Carte) As Boolean
        CarteEncours.Separateur = CrLf
        CarteEncours.Nom = NomCarte.Text
        CarteEncours.Format = ImageFormat.Bmp
        CarteEncours.DimensionsTuile = DimTuile
        CarteEncours.CheminFichiersTuile = CheminTuile
        CarteEncours.DemandeAfficherGrille = False
        CarteEncours.DemandeAjouterCoordonneesGrille = ChoixCoordonneesGrille.Aucune
        CarteEncours.DemandeFichiersGeoref = ChoixFichiersGeoref.Aucun
        CarteEncours.DemandeFichiersTuiles = ChoixFichiersTuiles.KMZ
        Dim Info = NomCarte.Text & CrLf & "Finalisation de la carte du niveau de détails :  " & CarteEncours.SystemCartographique.Clef
        FinaliserFichiersTuiles = CarteEncours.FinaliserCarte(Coins, Info)
    End Function
    ''' <summary> change la dimension des tuiles de fichier. 256 ou 512 </summary>
    Private Sub N512_ValueChanged(sender As Object, e As EventArgs)
        DimTuile = CInt(N512.Value)
        If Not FlagEvenement Then CalculerNbTuilesKMZ()
    End Sub
    ''' <summary> change la couleur du texte du control si la valeur de celui-ci est modifiée par rapport à la valeur initiale</summary>
    ''' <param name="C"> Control concerné </param>
    ''' <param name="Val1"> valeur initiale </param>
    ''' <param name="Val2"> valeur finale </param>
    Private Shared Sub ChangerCouleurText(C As NumericUpDown, Val1 As Double, Val2 As Double)
        If Val1 = Val2 Then
            C.ForeColor = Color.YellowGreen
        Else
            C.ForeColor = Color.Tomato
        End If
    End Sub
    ''' <summary> transforme des coordonnées réelles en texte comme si elles venaient d'une capture </summary>
    ''' <param name="Sys"> système cartographique associé aux coordonnées </param>
    Private Function CalculerCoins() As String()
        Dim Coins(3) As String
        Dim Selection As RectangleD = SelectionReferencementSiteCarto(NumSiteEncours)
        If TypeCoordEncours = CoordonneesGeoreferencements.DD Then
            For Cpt As Integer = 0 To 3
                Coins(Cpt) = ConvertPointDDtoDMS(Selection.Pt(Cpt))
            Next
        Else
            For Cpt As Integer = 0 To 3
                Coins(Cpt) = ConvertPointXYtoChaine(New PointProjection(Selection.Pt(Cpt)), "N2")
            Next
        End If
        Return Coins
    End Function
    ''' <summary> MAJ des valeurs des différents controles associés aux dimensions virtuelles de l'emprise des fichiers tuiles </summary>
    Private Sub MettreAJourDimensions()
        SelectionEncours = _RegroupementEncours.SelectionGeoref
        LLARGEUR.Text = SelectionEncours.Width.ToString
        CorLargeur.Minimum = SelectionEncours.Width - 10000
        CorLargeur.Maximum = SelectionEncours.Width + 10000
        CorLargeur.Value = SelectionEncours.Width
        LHAUTEUR.Text = SelectionEncours.Height.ToString
        CorHauteur.Minimum = SelectionEncours.Height - 10000
        CorHauteur.Maximum = SelectionEncours.Height + 10000
        CorHauteur.Value = SelectionEncours.Height
        LPT0X.Text = SelectionEncours.Location.X.ToString
        CorPT0X.Minimum = SelectionEncours.Location.X - 10000
        CorPT0X.Maximum = SelectionEncours.Location.X + 10000
        CorPT0X.Value = SelectionEncours.Location.X
        LPT0Y.Text = SelectionEncours.Location.Y.ToString
        CorPT0Y.Minimum = SelectionEncours.Location.Y - 10000
        CorPT0Y.Maximum = SelectionEncours.Location.Y + 10000
        CorPT0Y.Value = SelectionEncours.Location.Y
    End Sub
    ''' <summary> initialise les valeurs par défaut des champs de choix utilisateur </summary>
    Private Sub InitialiserValeurs()
        DimTuile = RegrouperSettings.DIM_TUILE_KMZ
        N512.Value = DimTuile
        ChoixTailleTampon.Items.AddRange(New String() {"Petit", "Moyen", "Grand"})
        ChoixTailleTampon.SelectedIndex = RegrouperSettings.TAILLE_TAMPON
    End Sub
    ''' <summary> calcul du poids (nb d'octets) représenté par les images des différents niveaux de détails inclus dans les fichiers tuiles </summary>
    Private Sub CalculerNbTuilesKMZ()
        NbTuilesX = CInt(Math.Ceiling(SelectionModifie.Width / DimTuile))
        NbTuilesY = CInt(Math.Ceiling(SelectionModifie.Height / DimTuile))
        NbTuilesTotal = NbTuilesX * NbTuilesY
        NbCols.Text = NbTuilesX.ToString("N0")
        NbRangs.Text = NbTuilesY.ToString("N0")
        NbTuilesKMZ.Text = $"{NbTuilesTotal:N0} / {NbTuilesMaxi:N0}"
        Lancer.Enabled = True
        If NbTuilesTotal > NbTuilesMaxi OrElse NbTuilesTotal = 0 Then
            NbTuilesKMZ.BackColor = Color.Tomato
            Lancer.Enabled = False
        Else
            NbTuilesKMZ.BackColor = Color.YellowGreen
        End If
        Dim NBTuiles As Integer = NbTuilesMaxX()
        NbCols.Text = NbTuilesX.ToString & " / " & NBTuiles.ToString
        If NbTuilesX > NBTuiles Then
            NbCols.BackColor = Color.Tomato
            Lancer.Enabled = False
        Else
            NbCols.BackColor = Color.YellowGreen
        End If
    End Sub
    ''' <summary> Calcul le nb max de tuile sur l'axe des X </summary>
    Private Function NbTuilesMaxX() As Integer
        Dim NbTuiles As Integer
        'si il y a une demande d'interpolation sur le site SM il faut beaucoup plus de mémoire pour la réaliser d'où une limitation
        'plus importante sur l'axe des X sinon la limitation est donnée par la hauteur de la tuile
        If SystemeCartoEncours.SiteCarto = SitesCartographiques.SuisseMobile Then
            'constantes empiriques pour des tuiles de 256*256
            If ChoixTailleTampon.SelectedIndex = 1 Then '0.500 Go
                NbTuiles = 328 \ 2
            ElseIf ChoixTailleTampon.SelectedIndex = 2 Then '1.000 Go
                NbTuiles = 740 \ 2
            Else '0.167 Go
                NbTuiles = 160 \ 2
            End If
            If DimTuile = 1024 Then NbTuiles \= 2
        Else
            NbTuiles = TailleReserveMini \ (DimTuile * NbOctetsPixel * DimTuile)
        End If
        Return NbTuiles
    End Function
    ''' <summary> calcule les coordonnées réelles correspondant à la sélection virtuelle et MAJ les controles associés </summary>
    Private Sub CalculerSelectionReel()
        SelectionModifie.X = CInt(CorPT0X.Value)
        SelectionModifie.Y = CInt(CorPT0Y.Value)
        SelectionModifie.Width = CInt(CorLargeur.Value)
        SelectionModifie.Height = CInt(CorHauteur.Value)
        PT0Reel.Text = CoordonneesReelles(SelectionModifie.Location)
        PT2Reel.Text = CoordonneesReelles(New Point(SelectionModifie.Right, SelectionModifie.Bottom))
    End Sub
    ''' <summary> Calcul des coordonnées réelles en fonction du type de coordonnées d'un point exprimées en coordonnées virtuelles </summary>
    ''' <param name="PtPixels"> point en coordonnées virtuelles à convertir </param>
    Private Function CoordonneesReelles(PtPixels As Point) As String
        Dim LabelCoordonnees As String = ""
        Select Case RegrouperSettings.INDICE_TYPE_COORDONNEES
            Case 0
                LabelCoordonnees = CoordMouseGrilleText(CoordMouseGrille(PtPixels, SiteCarto, Echelle))
            Case 1
                LabelCoordonnees = CoordMousseDDText(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum)
            Case 2
                LabelCoordonnees = CoordMouseDMSText(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum)
            Case 3
                LabelCoordonnees = CoordMouseUtmWGS84(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum)
            Case 4
                LabelCoordonnees = CoordMousePointG(PtPixels, IndiceEchelle).ToString
        End Select
        Return LabelCoordonnees
    End Function
    ''' <summary> vérifications avant de lancer la création des fichiers tuiles </summary>
    ''' <returns> ok si tout est correct </returns>
    Private Function VerifierValidation() As Boolean
        If NomCarte.Text & "" = "" Then
            MessageInformation = "Vous devez indiquer un nom pour la carte"
            AfficherInformation()
            NomCarte.Focus()
            Return False
        End If
        Return True
    End Function
End Class