Imports FCGP.NiveauDetailCartographique
Imports FCGP.Regroupement
Imports FCGP.Regrouper

''' <summary> permet la création des fichiers tuiles à plusieurs regroupements (niveau de détails ou échelle de capture) </summary>
Friend Class FichiersTuilesJnxOrux
    Private TailleReserveMini As Integer
    Private SiteCartoEncours As SitesCartographiques, TypeCoordEncours As CoordonneesGeoreferencements
    Private NumSiteEncours As Integer, SystemeType As SystemeCartographique, _RegroupementEncours As Regroupement
    Private Const TailleMaxi As Double = 40.0
    Private SelectionEncours, SelectionModifie As Rectangle
    Private ReadOnly RegroupementsTuiles(4) As Regroupement, NiveauJNXModifie(4), JNXDefaut(4) As Double
    Private ReadOnly Inclus(4) As Boolean, NiveauOruxModifie(4), ORUXDefaut(4) As Integer
    Private IndexMoins, NbTuilesX, NbTuilesY, DimTuile As Integer, MessInfo, CheminTuile, Coins() As String, FlagAjoutNiveau, FlagEvenement As Boolean
    Private Datum As Datums, Echelle As Echelles, IndiceEchelle As Integer
    Private TitreMessageInformation As String
    Friend WriteOnly Property RegroupementEncours As Regroupement
        Set(value As Regroupement)
            _RegroupementEncours = value
        End Set
    End Property
    Private Sub FichiersTuilesJnxOrux_Load(sender As Object, e As EventArgs)
        TitreMessageInformation = TitreInformation
        'on indique au module carte le label où écrire les messages concernant le travail en cours
        LabelInformation = Information
        TitreInformation = "Création des fichiers Orux et Jnx"
        SystemeType = _RegroupementEncours.SystemeType
        NumSiteEncours = _RegroupementEncours.NumSite
        TypeCoordEncours = SystemeType.CoordonneesGeoreferencement
        SiteCartoEncours = SystemeType.SiteCarto
        FlagEvenement = True
        InitialiserValeurs()
        Dim Cpt As Integer = 0
        For Each R As KeyValuePair(Of String, Regroupement) In Regroupements
            If R.Value.NumSite = NumSiteEncours Then
                RegroupementsTuiles(Cpt) = R.Value
                Inclus(Cpt) = True
                Dim C As Panel = CType(Support.Controls("Echelle" & Cpt), Panel)
                C.Enabled = True
                InitialiserPanel(C, Cpt)
                Cpt += 1
                If Cpt = 5 Then Exit For
            End If
        Next
        Dim S = RegroupementsTuiles(0).SystemeType
        Datum = DatumSiteWeb(SiteCartoEncours)
        Echelle = S.Niveau.Echelle
        IndiceEchelle = EchelleToSiteIndiceEchelle(SiteCartoEncours, Echelle)
        MettreAJourDimensions()
        CalculerSelectionReel()
        CheminTuile = PartagerSettings.CHEMIN_TUILE
        CalculerPoidsTotal()
        FlagEvenement = False
        Quitter.Select()
    End Sub

    Private Sub FichierTuilesJnxOrux_FormClosed(sender As Object, e As FormClosedEventArgs)
        If SelectionEncours <> SelectionModifie Then
            RegroupementsTuiles(0).SelectionGeorefToSelectionReferencement(SelectionModifie)
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
            CalculerPoidsTotal()
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
                Case Keys.None 'arrondi le nb de pixels à la dimension de la tuile du fichier tuile. Permet de démarrer sur une valeur connue
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
    ''' <summary> inclu ou exclu un niveau du fichier tuile </summary>
    Private Sub Include_CheckedChanged(sender As Object, e As EventArgs)
        If Not FlagEvenement Then
            Dim C As CheckBox = CType(sender, CheckBox)
            Inclus(CInt(C.Name.Substring(7))) = C.Checked
            FlagEvenement = True
            MettreAJourDimensions()
            FlagEvenement = False
            CalculerSelectionReel()
            CalculerPoidsTotal()
        End If
    End Sub
    ''' <summary> lance la création du ou des fichiers tuiles </summary>
    Private Async Sub Lancer_Click(sender As Object, e As EventArgs)
        If Not VerifierValidation() Then Exit Sub
        Owner.Hide()
        MessageInformation = "Il y a eu une erreur lors de la création de la carte ou du fichier" & CrLf &
                             "La création du ou des fichiers est arrêtée"
        Lancer.Enabled = False
        Quitter.Enabled = False
        Cursor = Cursors.WaitCursor
        RegrouperSettings.FICHIERS_TUILE = If(FichierJNX.Checked, ChoixFichiersTuiles.JNX, ChoixFichiersTuiles.Aucun) Or
                                           If(FichierORUX.Checked, ChoixFichiersTuiles.ORUX, ChoixFichiersTuiles.Aucun)
        RegrouperSettings.DIM_TUILE_JNXORUX = CInt(N256.Value)
        RegrouperSettings.TAILLE_TAMPON = ChoixTailleTampon.SelectedIndex
        RegrouperSettings.Ecrire()
        'on met à jour la sélection pour prendre en compte une modification
        If SelectionEncours <> SelectionModifie Then
            RegroupementsTuiles(0).SelectionGeorefToSelectionReferencement(SelectionModifie)
            SelectionEncours = RegroupementsTuiles(0).SelectionGeoref
        End If
        'il faut calculer les 4 coins de la carte à construire
        Coins = CalculerCoins()
        Dim DebutTraitement As Date = Now
        Try
            For Cpt As Integer = 4 To 0 Step -1 'il peut y avoir 5 niveaux de détails
                If Inclus(Cpt) = True Then 'Creation fichier binaire correspondant à l'échelle si le niveau est inclus dans le fichier tuiles
                    MessInfo = NomCarte.Text & CrLf & "Création de la carte du niveau de détails : " & RegroupementsTuiles(Cpt).ClefEchelle
                    Information.Text = MessInfo
                    Information.Refresh()
                    Dim Compteur = Cpt
                    If Not Await Task.Run(Function() RealiserFichiersTuiles(Compteur)) Then Exit Try
                End If
            Next
            MessageInformation = $"Le ou les fichiers demandés ont été créés.{CrLf}Temps de traitement : {TimeSpanToStr(Now - DebutTraitement, False)}"
        Catch Ex As Exception
            AfficherErreur(Ex, "B4A5")
        End Try
        AfficherInformation()
        Owner.Show()
        Close()
    End Sub
    ''' <summary> Réalise le ou les fichiers tuile demandés </summary>
    ''' <param name="Cpt"> indice du niveau de détail </param>
    Private Function RealiserFichiersTuiles(Cpt As Integer) As Boolean
        RealiserFichiersTuiles = False
        Try
            Dim R As Regroupement = RegroupementsTuiles(Cpt)
            Dim FichierBinaire As String = CheminEnregistrementProvisoire & "\Fichierbinaire" & Cpt & ".raw"
            Dim Sys As SystemeCartographique = R.SystemeType
            Dim Selection As Rectangle = R.SelectionGeoref
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
                If (FichierORUX.Checked And NiveauOruxModifie(Cpt) <> ORUXDefaut(Cpt)) OrElse (FichierJNX.Checked And NiveauJNXModifie(Cpt) <> JNXDefaut(Cpt)) Then
                    FacteursSettings.Ecrire(Sys.IndiceSiteCarto, Sys.IndiceEchelle, NiveauJNXModifie(Cpt), NiveauOruxModifie(Cpt))
                End If
                'il faut assembler l'image de la carte sous forme d'un fichier binaire
                If Not RealiserFichierBinaire(CarteNiveau, Selection, R.Cartes) Then Exit Try
                Dim Compteur As Integer = Cpt
                'et réaliser le ou les ficheirs tuiles correspondnants
                If Not FinaliserFichiersTuiles(CarteNiveau, Compteur) Then Exit Try
                RealiserFichiersTuiles = True
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "X5R3")
        End Try
    End Function
    ''' <summary>  pour chaque niveau de détail il faut créer l'image de la carte sous forme de fichier binaire </summary>
    ''' <param name="CarteEncours"> carte contenant les information et le tampon pour la réalisation du fichier binaire </param>
    ''' <param name="Selection"> région couverte par le fichier binaire </param>
    ''' <param name="Cartes"> liste des cartes de capture permettant la réalisation de l'image globale </param>
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
            AfficherErreur(Ex, "I5D3")
        End Try
    End Function
    ''' <summary> pour chaque niveau de détail il faut soit créer le fichier tuile soit ajouter un niveau au fichier tuile </summary>
    ''' <param name="FichierBinaire"> chemin et nom du fichier binaire contenant l'image de la carte </param>
    ''' <param name="Cpt"> numéro du niveau de détail, de 0 à 4 </param>
    Private Function FinaliserFichiersTuiles(CarteEncours As Carte, Cpt As Integer) As Boolean
        CarteEncours.Separateur = CrLf
        CarteEncours.Nom = NomCarte.Text
        CarteEncours.Format = ImageFormat.Bmp
        CarteEncours.DimensionsTuile = DimTuile
        CarteEncours.CheminFichiersTuile = CheminTuile
        CarteEncours.DemandeAfficherGrille = False
        CarteEncours.DemandeAjouterCoordonneesGrille = ChoixCoordonneesGrille.Aucune
        CarteEncours.DemandeFichiersGeoref = ChoixFichiersGeoref.Aucun
        CarteEncours.FacteurJNX = NiveauJNXModifie(Cpt)
        CarteEncours.FacteurORUX = NiveauOruxModifie(Cpt)
        CarteEncours.DemandeFichiersTuiles = If(FichierJNX.Checked, ChoixFichiersTuiles.JNX, ChoixFichiersTuiles.Aucun) Or
                                             If(FichierORUX.Checked, ChoixFichiersTuiles.ORUX, ChoixFichiersTuiles.Aucun)
        CarteEncours.DemandeAjoutNiveau = FlagAjoutNiveau
        FlagAjoutNiveau = True
        Dim Info = NomCarte.Text & CrLf & "Finalisation de la carte du niveau de détails :  " & CarteEncours.SystemCartographique.Clef
        FinaliserFichiersTuiles = CarteEncours.FinaliserCarte(Coins, Info)
    End Function
    ''' <summary> change la dimension des tuiles de fichier. 256 ou 512 </summary>
    Private Sub N256_ValueChanged(sender As Object, e As EventArgs)
        DimTuile = CInt(N256.Value)
        If Not FlagEvenement Then CalculerPoidsTotal()
    End Sub
    ''' <summary> active ou non la création du fichier tuile JNX </summary>
    Private Sub FichierJNX_CheckedChanged(sender As Object, e As EventArgs)
        If Not FlagEvenement Then CalculerPoidsTotal()
    End Sub
    ''' <summary> change la taille de la mémoire tampon réservée pour la création des fichiers tuiles </summary>
    Private Sub ChoixTailleTampon_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ChoixTailleTampon.SelectedIndex = 0 Then
            TailleReserveMini = TailleMaxTampon \ 3 '0.167 Go
        ElseIf ChoixTailleTampon.SelectedIndex = 1 Then
            TailleReserveMini = TailleMaxTampon '0.500 Go
        Else
            TailleReserveMini = TailleMaxTampon * 2 '1.000 Go
        End If
        If Not FlagEvenement Then CalculerPoidsTotal()
    End Sub
    ''' <summary> change la valeur associée au zoom du niveau dans le fichier JNX </summary>
    Private Sub CorZoomJNX_ValueChanged(sender As Object, e As EventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown)
        Dim Cpt As Integer = CInt(C.Tag)
        NiveauJNXModifie(Cpt) = C.Value
        ChangerCouleurText(C, NiveauJNXModifie(Cpt), JNXDefaut(Cpt))
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
    ''' <summary> change la valeur associée au zoom du niveau dans le fichier ORUX </summary>
    Private Sub CorZoomORUX_ValueChanged(sender As Object, e As EventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown)
        Dim Cpt As Integer = CInt(C.Tag)
        NiveauOruxModifie(Cpt) = CInt(C.Value)
        ChangerCouleurText(C, NiveauOruxModifie(Cpt), ORUXDefaut(Cpt))
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
        For Cpt As Integer = 0 To 4
            If Inclus(Cpt) Then 'la première échelle est la bonne car c'est la plus grande
                IndexMoins = Cpt
                SelectionEncours = RegroupementsTuiles(IndexMoins).SelectionGeoref
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
                Exit Sub
            End If
        Next
    End Sub
    ''' <summary> initialise les valeurs par défaut des champs de choix utilisateur </summary>
    Private Sub InitialiserValeurs()
        DimTuile = RegrouperSettings.DIM_TUILE_JNXORUX
        N256.Value = DimTuile
        FichierORUX.Checked = RegrouperSettings.FICHIERS_TUILE.HasFlag(ChoixFichiersTuiles.ORUX)
        FichierJNX.Checked = RegrouperSettings.FICHIERS_TUILE.HasFlag(ChoixFichiersTuiles.JNX)
        ChoixTailleTampon.Items.AddRange(New String() {"Petit", "Moyen", "Grand"})
        ChoixTailleTampon.SelectedIndex = RegrouperSettings.TAILLE_TAMPON
    End Sub
    ''' <summary> remplit les champs de chaque ligne dédié aux niveaux de dtails : 4 champs </summary>
    ''' <param name="Pp"> Le panel à remplir </param>
    ''' <param name="Cpt"> le Numéro du panel </param>
    Private Sub InitialiserPanel(Pp As Panel, Cpt As Integer)
        Dim Sys As SystemeCartographique = RegroupementsTuiles(Cpt).SystemeType
        CType(Pp.Controls("Include" & Cpt), CheckBox).Checked = True
        'trouve le préfixe de la clef à afficher 
        Dim Clef As String = If(Sys.SiteCarto = SitesCartographiques.DomTom, SystemeCartographique.DomTomClef(Sys.DomTom),
                                                                             SystemeCartographique.SiteCartoClef(Sys.SiteCarto))
        'et rajoute la clef de l'échelle
        Pp.Controls("Clef" & Cpt).Text = Clef & "-" & Sys.Niveau.Clef
        'trouve la valeur actuelle de l'indice d'affichage JNX pour l'échelle
        Dim ValeurActuelleJNX = FacteursSettings.Lire(Sys.IndiceSiteCarto, Sys.IndiceEchelle).FacteurJNX
        Dim ValeurActuelleORUX = FacteursSettings.Lire(Sys.IndiceSiteCarto, Sys.IndiceEchelle).FacteurORUX
        'et trouve la valeur par defaut de l'indice d'affichage JNX pour l'échelle
        JNXDefaut(Cpt) = Sys.Niveau.NiveauAffichageJNX
        Pp.Controls("ZoomJNX" & Cpt).Text = DblToStr(ValeurActuelleJNX, PrecisionJNX)
        'la couleur du texte dépend si la valeur actuelle est la valeur par défaut
        If ValeurActuelleJNX = JNXDefaut(Cpt) Then
            Pp.Controls("ZoomJNX" & Cpt).ForeColor = Color.YellowGreen
        Else
            Pp.Controls("ZoomJNX" & Cpt).ForeColor = Color.Tomato
        End If
        Dim ValeurModifieJNX As NumericUpDown = CType(Pp.Controls("CorZoomJNX" & Cpt), NumericUpDown)
        'on donne la possibilité de modifier l'indice d'affichage dans une plage d +ou- 3 par rapport à la valeur par défaut
        ValeurModifieJNX.Minimum = CDec(JNXDefaut(Cpt) - 3)
        ValeurModifieJNX.Maximum = CDec(JNXDefaut(Cpt) + 3)
        If ValeurActuelleJNX > JNXDefaut(Cpt) + 3 Then ValeurActuelleJNX = JNXDefaut(Cpt) + 3
        If ValeurActuelleJNX < JNXDefaut(Cpt) - 3 Then ValeurActuelleJNX = JNXDefaut(Cpt) - 3
        ValeurModifieJNX.Value = CDec(ValeurActuelleJNX)
        'idem pour l'indice d'affichage ORUX

        ORUXDefaut(Cpt) = Sys.Niveau.NiveauAffichageORUX
        Pp.Controls("ZoomORUX" & Cpt).Text = ValeurActuelleORUX.ToString
        If ValeurActuelleORUX = ORUXDefaut(Cpt) Then
            Pp.Controls("ZoomORUX" & Cpt).ForeColor = Color.YellowGreen
        Else
            Pp.Controls("ZoomORUX" & Cpt).ForeColor = Color.Tomato
        End If
        Dim ValeurModifieORUX As NumericUpDown = CType(Pp.Controls("CorZoomORUX" & Cpt), NumericUpDown) '
        ValeurModifieORUX.Minimum = CDec(ORUXDefaut(Cpt) - 3)
        ValeurModifieORUX.Maximum = CDec(ORUXDefaut(Cpt) + 3)
        If ValeurActuelleORUX > ORUXDefaut(Cpt) + 3 Then ValeurActuelleORUX = ORUXDefaut(Cpt) + 3
        If ValeurActuelleORUX < ORUXDefaut(Cpt) - 3 Then ValeurActuelleORUX = ORUXDefaut(Cpt) - 3
        ValeurModifieORUX.Value = CDec(ValeurActuelleORUX)
    End Sub
    ''' <summary> Calcul le nb max de tuiles sur l'axe des X </summary>
    Private Function NbTuilesMaxX() As Integer
        Dim NbTuiles As Integer
        'si il y a une demande d'interpolation sur le site SM il faut beaucoup plus de mémoire pour la réaliser d'où une limitation
        'plus importante sur l'axe des X sinon la limitation est donnée par la hauteur de la tuile
        If SiteCartoEncours = SitesCartographiques.SuisseMobile AndAlso FichierJNX.Checked Then
            'constantes empiriques pour des tuiles de 256*256
            If ChoixTailleTampon.SelectedIndex = 1 Then '0.500 Go
                NbTuiles = 328
            ElseIf ChoixTailleTampon.SelectedIndex = 2 Then '1.000 Go
                NbTuiles = 740
            Else '0.167 Go
                NbTuiles = 160
            End If
            If DimTuile = 512 Then NbTuiles \= 2
        Else
            NbTuiles = TailleReserveMini \ (DimTuile * NbOctetsPixel * DimTuile)
        End If
        Return NbTuiles
    End Function
    ''' <summary> calcul du poids (nb d'octets) représenté par les images des différents niveaux de détails inclus dans les fichiers tuiles </summary>
    Private Sub CalculerPoidsTotal()
        Dim PoisTotalGeoref As Double
        Dim TuilesTotalGeoref As Integer
        For Cpt As Integer = 0 To 4
            If Inclus(Cpt) Then
                PoisTotalGeoref += PoidsSelection(Cpt)
            End If
        Next
        PoisTotalGeoref /= 2 ^ 30
        NbTuilesX = CInt(Math.Ceiling(SelectionModifie.Width / DimTuile))
        NbTuilesY = CInt(Math.Ceiling(SelectionModifie.Height / DimTuile))
        TuilesTotalGeoref = NbTuilesX * NbTuilesY
        Dim NBTuiles As Integer = NbTuilesMaxX()
        Lancer.Enabled = True
        Poids.Text = $"{PoisTotalGeoref:N3} Go / {TailleMaxi:N0} Go"

        If PoisTotalGeoref > TailleMaxi OrElse PoisTotalGeoref = 0 Then
            Poids.BackColor = Color.Tomato
            Lancer.Enabled = False
        Else
            Poids.BackColor = Color.YellowGreen
        End If

        NbTuilesToltal.Text = $"{TuilesTotalGeoref} / 50000"
        If TuilesTotalGeoref > 50000 Then
            NbTuilesToltal.BackColor = Color.Tomato
            Lancer.Enabled = False
        Else
            NbTuilesToltal.BackColor = Color.YellowGreen
        End If

        NbTuileMaxX.Text = NbTuilesX.ToString & " / " & NBTuiles.ToString
        If NbTuilesX > NBTuiles Then
            NbTuileMaxX.BackColor = Color.Tomato
            Lancer.Enabled = False
        Else
            NbTuileMaxX.BackColor = Color.YellowGreen
        End If
    End Sub
    ''' <summary> calcul du poids (nb d'octets) représenté par l'image du niveau de détails </summary>
    ''' <param name="Index"> Numéro du niveau de détails </param>
    Private Function PoidsSelection(Index As Integer) As Double
        Dim Selection As Rectangle = RegroupementsTuiles(Index).SelectionGeoref
        Return StrideImage(Selection.Width) * CDbl(Selection.Height)
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
    ''' <summary> calcule les coordonnées réelles correspondant à la sélection virtuelle et MAJ les controles associés </summary>
    Private Function CoordonneesReelles(PtPixels As Point) As String
        Dim LabelCoordonnees As String = ""
        Select Case RegrouperSettings.INDICE_TYPE_COORDONNEES
            Case 0
                LabelCoordonnees = CoordMouseGrilleText(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle))
            Case 1
                LabelCoordonnees = CoordMousseDDText(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle), Datum)
            Case 2
                LabelCoordonnees = CoordMouseDMSText(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle), Datum)
            Case 3
                LabelCoordonnees = CoordMouseUtmWGS84(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle), Datum)
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
        If (Not FichierJNX.Checked) And (Not FichierORUX.Checked) Then
            MessageInformation = "Vous devez indiquer au moins un fichier tuiles"
            AfficherInformation()
            FichierJNX.Focus()
            Return False
        End If
        Dim ValeurSupORUX As Integer = 50, ValeurInfJNX As Double = 0
        For Cpt As Integer = 0 To 4
            If Inclus(Cpt) Then
                Dim Sys As SystemeCartographique = RegroupementsTuiles(Cpt).SystemeType
                If NiveauJNXModifie(Cpt) > ValeurInfJNX Then
                    ValeurInfJNX = NiveauJNXModifie(Cpt)
                Else
                    MessageInformation = "Le zoom JNX du niveau de détail " & Sys.Clef & "n'est pas supérieur" & CrLf & "au zoom JNX du niveau de détail inclus précédent"
                    AfficherInformation()
                    Return False
                End If
                If NiveauOruxModifie(Cpt) < ValeurSupORUX Then
                    ValeurSupORUX = NiveauOruxModifie(Cpt)
                Else
                    MessageInformation = "Le zoom ORUX du niveau de détail " & Sys.Clef & "n'est pas inférieur" & CrLf & "au zoom ORUX du niveau de détail inclus précédent"
                    AfficherInformation()
                    Return False
                End If
            End If
        Next
        Return True
    End Function
End Class