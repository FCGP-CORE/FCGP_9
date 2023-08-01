''' <summary> permet la configuration de l'application FCGP_Regrouper </summary>
Friend Class Configuration
    Inherits Form

    Private ChercheChemin As FolderBrowserDialog, Titre As String
    ''' <summary> correspondance site --> radiobouton </summary>
    Private Sub Configuration_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        TitreInformation = "Information Configuration"
        'la mise à jour de l'index génère le calcul des dimensions min-max de capture
        For Each S As Screen In Screen.AllScreens
            ListeEcrans.Items.Add(S.DeviceName & ", " & S.Bounds.Width.ToString & "*" & S.Bounds.Height)
        Next

        'met à jour les champs qui dépendent du settings commun
        ListeEcrans.SelectedIndex = PartagerSettings.NUM_ECRAN
        CouleurFondAffichage.BackColor = PartagerSettings.COULEUR_VISUE
        CouleurFondCarte.BackColor = PartagerSettings.COULEUR_CARTE
        RepertoireTuiles.Text = PartagerSettings.CHEMIN_TUILE
        'MAJ champs dépendants du AltitudeSettings
        ID.Text = AltitudesSettings.ID
        MP.Text = AltitudesSettings.MP
        RepertoireAltitudes.Text = AltitudesSettings.CHEMIN_DEM
        AvecAltitude.Checked = AltitudesSettings.IS_ALTITUDE
        DEM1.Checked = AltitudesSettings.TYPE_DEM = 1
        MSGAltitude.Checked = AltitudesSettings.IS_MSG
        VerifierID.Text = If(AltitudesSettings.IS_VALIDE, "OK", "?")
        VerifierID.BackColor = If(AltitudesSettings.IS_VALIDE, Color.YellowGreen, Color.Transparent)
        VerifierID.Enabled = VerifierID.BackColor <> Color.YellowGreen
        PanelAltitude.Enabled = AvecAltitude.Checked

        PasDeplacement.Value = RegrouperSettings.PAS_DEPLACEMENT

        'positionnement du curseur de souris au milieu du bouton ok
        With Valider
            Cursor.Position = New Point(Location.X + .Bounds.X + .Bounds.Width \ 2, Location.Y + .Location.Y + .Size.Height \ 2)
        End With
        ChercheChemin = New FolderBrowserDialog()
    End Sub
    ''' <summary> ferme la boite de dialogue ave un validation éventuelle du choix </summary>
    Private Sub Configuration_FormClosed(sender As Object, e As FormClosedEventArgs)
        If DialogResult = DialogResult.OK Then
            'on met à jour le settings altitude
            AltitudesSettings.IS_ALTITUDE = AvecAltitude.Checked
            AltitudesSettings.IS_MSG = MSGAltitude.Checked
            If AvecAltitude.Checked Then
                'on garde l'existant pour voir si il y a un changement
                Dim RepAncien As String = AltitudesSettings.CHEMIN_DEM
                AltitudesSettings.TYPE_DEM = If(DEM1.Checked, 1, 3)
                AltitudesSettings.CHEMIN_DEM = RepertoireAltitudes.Text
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
            'on met à jour le settings commun
            PartagerSettings.NUM_ECRAN = ListeEcrans.SelectedIndex
            PartagerSettings.COULEUR_VISUE = CouleurFondAffichage.BackColor
            PartagerSettings.COULEUR_CARTE = CouleurFondCarte.BackColor
            PartagerSettings.CHEMIN_TUILE = RepertoireTuiles.Text
            PartagerSettings.Ecrire()
            'on met à jour le settings visue
            RegrouperSettings.PAS_DEPLACEMENT = CInt(PasDeplacement.Value)
            RegrouperSettings.Ecrire()
        End If
        TitreInformation = Titre
    End Sub
    ''' <summary> vérifie que les identifiants pour le téléchargement des fichiers d'altitude ont été correctement saisis </summary>
    Private Sub Configuration_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            If AvecAltitude.Checked And VerifierID.Text <> "OK" Then
                MessageInformation = "Vous devez renseigner les identifiants et" & CrLf & "et les vérifier."
                AfficherInformation()
                e.Cancel = True
            End If
        End If
    End Sub
    '''' <summary> met à jour la couleur de fond de la visue </summary>
    Private Sub Couleur_Click(sender As Object, e As EventArgs)
        Using C As New ColorDialog()
            Dim Couleur As Label = CType(sender, Label)
            C.Color = Couleur.BackColor
            If C.ShowDialog = DialogResult.OK Then
                Couleur.BackColor = C.Color
            End If
        End Using
    End Sub
    ''' <summary> permet de changer le repertoire d'enregistrement des fichiers tuiles ou des fichiers d'altitude </summary>
    Private Sub RepertoireTuiles_Click(sender As Object, e As EventArgs)
        Try
            Dim CheminFichiersInitial As Label = CType(sender, Label)
            ChercheChemin.Description = CStr(CheminFichiersInitial.Tag)
            ChercheChemin.InitialDirectory = CheminFichiersInitial.Text
            If ChercheChemin.ShowDialog(Me) = DialogResult.OK Then
                If Directory.Exists(ChercheChemin.SelectedPath) Then CheminFichiersInitial.Text = ChercheChemin.SelectedPath
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "I8G0")
        End Try
    End Sub
    ''' <summary> permet de choisir si on veut voir afficher les altitudes des coordonnées du pointeur de souris </summary>
    Private Sub AvecAltitude_CheckedChanged(sender As Object, e As EventArgs)
        PanelAltitude.Enabled = AvecAltitude.Checked
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
    Private Sub IDs_TextChanged(sender As Object, e As EventArgs)
        VerifierID.Text = "?"
        VerifierID.BackColor = Color.Transparent
        VerifierID.Enabled = True
    End Sub
    Private Sub ListeEcrans_DropDownClosed(sender As Object, e As EventArgs)
        ListeEcrans.Visible = False
    End Sub
    Private Sub ListeEcrans_SelectedIndexChanged(sender As Object, e As EventArgs)
        EcranTravail.Text = ListeEcrans.SelectedItem.ToString
    End Sub
    Private Sub EcranTravail_Click(sender As Object, e As EventArgs)
        ListeEcrans.DroppedDown = True
        ListeEcrans.Show()
    End Sub
End Class