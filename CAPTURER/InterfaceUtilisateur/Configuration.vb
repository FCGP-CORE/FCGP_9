''' <summary> permet la configuration de FCGP_Capturer </summary>
Friend Class Configuration
    Inherits Form

    Private FlagIsToutFormat As Boolean
    Private ChercheChemin As FolderBrowserDialog
    Private StockClipCurseur As Rectangle
    Private Titre As String
    ''' <summary> initialisation des différentes variables internes en fonction des settings </summary>
    Private Sub Configuration_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        'la mise à jour de l'index génère le calcul des dimensions min-max de capture
        For Each S As Screen In Screen.AllScreens
            ListeChoixEcranTravail.Items.Add(S.DeviceName & ", " & S.Bounds.Width.ToString & "*" & S.Bounds.Height)
        Next
        'MAJ champs dépendants de CarteSettings
        FlagIsToutFormat = CartesSettings.IS_TOUT_FORMAT
        ListeChoixSupportCarte.SelectedIndex = If(CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier, 1, 0)
        'MAJ champs dépendants de CapturerSettings
        RepertoireCachesTuiles.Text = CapturerSettings.CHEMIN_CACHE
        'met à jour les champs qui dépendent du settings commun
        ListeChoixEcranTravail.SelectedIndex = PartagerSettings.NUM_ECRAN
        CouleurAffichage.BackColor = PartagerSettings.COULEUR_VISUE
        CouleurCartes.BackColor = PartagerSettings.COULEUR_CARTE
        QualiteJpeg.Value = PartagerSettings.QUALITE_JPEG
        'MAJ champs dépendants du AltitudeSettings
        ID.Text = AltitudesSettings.ID
        MP.Text = AltitudesSettings.MP
        RepertoireFichiersAltitudes.Text = AltitudesSettings.CHEMIN_DEM
        AvecAltitudes.Checked = AltitudesSettings.IS_ALTITUDE
        DEM1.Checked = AltitudesSettings.TYPE_DEM = 1
        MSGAltitudes.Checked = AltitudesSettings.IS_MSG
        VerifierID.Text = If(AltitudesSettings.IS_VALIDE, "OK", "?")
        VerifierID.BackColor = If(AltitudesSettings.IS_VALIDE, Color.YellowGreen, Color.Transparent)
        VerifierID.Enabled = VerifierID.BackColor <> Color.YellowGreen
        PanelAltitudes.Enabled = AvecAltitudes.Checked
        'MAJ champs dépendants de TraceSettings
        RepertoireTraces.Text = TracesSettings.CHEMIN_TRACE
        CouleurTraces.BackColor = TracesSettings.SEG_TRK
        CouleurDessinTraces.BackColor = TracesSettings.COULEUR_TRACE
        CoefAlphaTraces.Value = ValeurToPourcent(CouleurDessinTraces.BackColor.A)
        EpaisseurTraces.Value = TracesSettings.EPAISSEUR_TRACE

        'positionnement du curseur de souris au milieu du bouton ok
        With BoutonEnter
            Cursor.Position = New Point(Location.X + .Bounds.X + .Bounds.Width \ 2, Location.Y + .Location.Y + .Size.Height \ 2)
        End With
        ChercheChemin = New FolderBrowserDialog()
        'et limite les déplacements de la souris au formulaire
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> ferme la boite de dialogue ave un validation éventuelle du choix </summary>
    Private Sub Configuration_FormClosed(sender As Object, e As FormClosedEventArgs)
        If Me.DialogResult = DialogResult.OK Then
            'MAJ de TraceSettings
            TracesSettings.CHEMIN_TRACE = RepertoireTraces.Text
            TracesSettings.SEG_TRK = CouleurTraces.BackColor
            TracesSettings.COULEUR_TRACE = CouleurDessinTraces.BackColor
            TracesSettings.EPAISSEUR_TRACE = CInt(EpaisseurTraces.Value)
            TracesSettings.Ecrire()
            'MAJ de AltitudeSettings
            AltitudesSettings.IS_ALTITUDE = AvecAltitudes.Checked
            AltitudesSettings.IS_MSG = MSGAltitudes.Checked
            If AvecAltitudes.Checked Then
                'on garde l'existant pour voir si il y a un changement
                Dim RepAncien As String = AltitudesSettings.CHEMIN_DEM
                AltitudesSettings.TYPE_DEM = If(DEM1.Checked, 1, 3)
                AltitudesSettings.CHEMIN_DEM = RepertoireFichiersAltitudes.Text
                'le répertoire a changé ou la liste n'a pas été initialisée il faut réinitialiser la liste des fichiers d'altitude
                If AltitudesSettings.CHEMIN_DEM <> RepAncien OrElse Not IsOkFichiersAltitudes Then
                    InitialiserListeFichiersAltitudes()
                End If
                AltitudesSettings.ID = ID.Text
                AltitudesSettings.MP = MP.Text
                AltitudesSettings.IS_VALIDE = True
                'La connexion avec le serveur n'est pas établie, il faut la créer
                If Not IsConnexion Then
                    InitialiserConnexionAltitudes()
                End If
            Else
                If VerifierID.Text <> "OK" Then
                    'on met ou remet les identifiants à zéro et on ferme la connexion avec le serveur 
                    AltitudesSettings.ID = ""
                    AltitudesSettings.MP = ""
                    AltitudesSettings.IS_VALIDE = False
                    InitialiserConnexionAltitudes()
                End If
            End If
            AltitudesSettings.Ecrire()
            'MAJ de CartesSettings
            CartesSettings.SUPPORT_CARTE = CType(ListeChoixSupportCarte.SelectedIndex, TypeSupportCarte)
            If CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier Then CartesSettings.IS_TOUT_FORMAT = FlagIsToutFormat
            CartesSettings.Ecrire()
            'MAJ de PartagerSettings
            PartagerSettings.NUM_ECRAN = ListeChoixEcranTravail.SelectedIndex
            PartagerSettings.COULEUR_VISUE = CouleurAffichage.BackColor
            PartagerSettings.COULEUR_CARTE = CouleurCartes.BackColor
            PartagerSettings.QUALITE_JPEG = CInt(QualiteJpeg.Value)
            PartagerSettings.Ecrire()
            'MAJ de CaptureSettings
            Dim RepNouveau = RepertoireCachesTuiles.Text
            If RepNouveau <> CapturerSettings.CHEMIN_CACHE Then 'le répertoire a changé il faut réinitialise
                Cursor = Cursors.WaitCursor
                If Serveur.Cache.ChangerRepertoireCaches(RepNouveau) Then
                    CapturerSettings.CHEMIN_CACHE = RepNouveau
                    CapturerSettings.Ecrire()
                End If
            End If
        End If
        Cursor.Clip = StockClipCurseur
        TitreInformation = Titre
    End Sub
    Private Sub Configuration_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            If AvecAltitudes.Checked And VerifierID.Text <> "OK" Then
                MessageInformation = "Vous devez renseigner les identifiants" & CrLf & "et les vérifier."
                AfficherInformation()
                e.Cancel = True
            End If
        End If
    End Sub
    '''' <summary> met à jour la variable globale en fonction de la case "ToutFormat" </summary>
    Private Sub IsToutFormat_CheckedChanged(sender As Object, e As EventArgs)
        If ListeChoixSupportCarte.SelectedIndex = 1 Then
            FlagIsToutFormat = IsToutFormat.Checked
        End If
    End Sub
    ''' <summary> met ajour la couleur de fond du label en fonction d'un choix de couleur et du % de transparence </summary>
    Private Sub Couleurs_Click(sender As Object, e As EventArgs)
        Cursor.Clip = StockClipCurseur
        Dim L As Label = CType(sender, Label), FlagTransparent As Boolean = CBool(L.Tag)
        Using C As New ColorDialog()
            C.Color = L.BackColor
            If C.ShowDialog(Me) = DialogResult.OK Then
                If FlagTransparent Then
                    L.BackColor = Color.FromArgb(PourcentToValeur(CInt(CoefAlphaTraces.Value)), C.Color)
                Else
                    L.BackColor = C.Color
                End If
            End If
        End Using
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> permet de choisir si on veut voir afficher les altitudes des coordonnées du pointeur de souris </summary>
    Private Sub AvecAltitude_CheckedChanged(sender As Object, e As EventArgs)
        PanelAltitudes.Enabled = AvecAltitudes.Checked
    End Sub
    ''' <summary> permet de modifier le répertoire d'enregistrement des caches tuiles, des fichiers d'altitudes et des traces </summary>
    Private Sub Repertoires_Click(sender As Object, e As EventArgs)
        Cursor.Clip = StockClipCurseur
        Dim L As Label = CType(sender, Label)
        If L.Name = "RepertoireCacheTuiles" Then
            MessageInformation = "Si le cache tuiles du site n'existe pas dans le nouveau répertoire" & CrLf &
                                 "un cache tuile vide sera créeé. Voulez vous continuer ?"
            If AfficherConfirmation() = DialogResult.Cancel Then Exit Sub
        End If
        Try
            ChercheChemin.Description = "Répertoire d'enregistrement des " & CStr(L.Tag)
            ChercheChemin.InitialDirectory = L.Text
            ChercheChemin.UseDescriptionForTitle = True
            If Me.ChercheChemin.ShowDialog(Me) = DialogResult.OK Then
                If Directory.Exists(ChercheChemin.SelectedPath) Then L.Text = ChercheChemin.SelectedPath
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "Y8X2")
        End Try
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> tranforme un % transparence de 0 à 100 à une valeur du canal alpha de couleur de 255 à 0 </summary>
    ''' <param name="Trans"> % transparence </param>
    Private Shared Function PourcentToValeur(Trans As Integer) As Integer
        Return 255 - (255 * Trans) \ 100
    End Function
    ''' <summary> transforme une valeur du canal alpha de couleur de 255 à 0 en un % transparence de 0 à 100 </summary>
    ''' <param name="Valeur"> valeur du canal alpha </param>
    Private Shared Function ValeurToPourcent(Valeur As Byte) As Integer
        Return 100 - ((Valeur * 100) \ 255)
    End Function
    ''' <summary> met à jour la couleur du label couleur en fonction du pourcentage de transparence désiré par l'utilisateur </summary>
    Private Sub TransparenceTrace_ValueChanged(sender As Object, e As EventArgs)
        CouleurDessinTraces.BackColor = Color.FromArgb(PourcentToValeur(CInt(CoefAlphaTraces.Value)), CouleurDessinTraces.BackColor)
    End Sub
    ''' <summary> demande la vérification des identifiants du serveur de la NASA </summary>
    Private Sub VerifierID_Click(sender As Object, e As EventArgs)
        If ID.Text = "" OrElse MP.Text = "" Then
            MessageInformation = "Vous devez renseigner les 2 identifiants" & CrLf & "avant de pouvoir demander la vérification."
        Else
            If IsConnected Then
                If VerifierIDAltitudes(ID.Text, MP.Text) = DialogResult.OK Then
                    MessageInformation = "Identifiants valides"
                    VerifierID.Text = "OK"
                    VerifierID.BackColor = Color.YellowGreen
                    VerifierID.Enabled = False
                Else
                    MessageInformation = "Identifiants non valides"
                    VerifierID.Text = "Not" & CrLf & "OK"
                    VerifierID.BackColor = Color.Tomato
                    VerifierID.Enabled = True
                End If
            Else
                MessageInformation = "La connexion internet n'est pas établie." & CrLf & "La vérification des identifiants est impossible"
            End If
        End If
        AfficherInformation()
    End Sub
    ''' <summary> invalide Verifier ID pour que l'on oblige l'utiliseteur à vérifier les identifiants </summary>
    Private Sub IDs_TextChanged(sender As Object, e As EventArgs)
        VerifierID.Text = "?"
        VerifierID.BackColor = Color.Transparent
        VerifierID.Enabled = True
    End Sub
    ''' <summary>Affiche le support de carte et autorise la case "ToutFormat" en fonction du support de carte ou affiche l'écran sélectioné </summary>
    Private Sub Listes_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim C As ComboBox = CType(sender, ComboBox), L As Label = CType(C.Tag, Label)
        If C.Name = "ListeChoixSupportCarte" Then
            If ListeChoixSupportCarte.SelectedIndex = 0 Then
                IsToutFormat.Checked = True
                IsToutFormat.Enabled = False
            Else
                IsToutFormat.Checked = FlagIsToutFormat
                IsToutFormat.Enabled = True
            End If
        End If
        L.Text = C.SelectedItem.ToString()
    End Sub
    ''' <summary> ouvre les listes déroulantes </summary>
    Private Sub Labels_Click(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label), C As ComboBox = CType(L.Tag, ComboBox)
        C.DroppedDown = True
        C.Show()
    End Sub
End Class