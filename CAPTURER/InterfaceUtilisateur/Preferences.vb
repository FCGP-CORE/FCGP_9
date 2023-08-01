''' <summary> permet la configuration de la sortie de la capture de la carte </summary>
Friend Class Preferences
    Private IndexFichiersTuiles, DimensionsTuile As Integer
    Private ChercheChemin As FolderBrowserDialog
    Private StockClipCurseur As Rectangle
    Private _SurfaceTraitement As SurfaceTuiles
    ''' <summary> nb de tuiles max en colonnes pour l'interpolation des cartes suisses sous peine de plantage. Valeurs empiriques </summary>
    Private ReadOnly NbTuilesMaxCarteSuisseInterpol() As Integer = New Integer() {328, 328, 204}
    Private Titre As String

    Friend WriteOnly Property SurfaceTraitement As SurfaceTuiles
        Set(value As SurfaceTuiles)
            _SurfaceTraitement = value
        End Set
    End Property

    ''' <summary>associe les informations du settings avec les champs de données du formulaire et remet à zéro le nom de la carte</summary>
    Private Sub Preferences_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        Try

            IsTrace.Enabled = CBool(Tag)
            If IsTrace.Enabled Then IsTrace.Checked = CartesSettings.IS_TRACE
            'on teste pour déterminer le format d'enregistrement de la carte si la carte n'est pas trop grande pour les encodeur JPEG et PNG de GDI+
            If (CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier AndAlso CartesSettings.IS_TOUT_FORMAT = False) OrElse
                _SurfaceTraitement.LargeurPixels > MaxPixelsJpeg OrElse _SurfaceTraitement.HauteurPixels > MaxPixelsJpeg Then
                'dans ce cas il n'y a pas de choix mais uniquement le format Bmp est possible
                ListeChoixFormatCartes.SelectedIndex = -1
                ListeChoixFormatCartes.Enabled = False
                LabelChoixFormatCartes.Text = "Bmp"
            Else
                ListeChoixFormatCartes.Items.AddRange(New String() {"Bmp", "Png", "Jpeg"})
                'initialisation du format de la carte
                ListeChoixFormatCartes.SelectedIndex = CartesSettings.FORMAT
            End If

            'initialisation de la demande de grille
            If SysCartoEncours.AfficherGrilleIsOK Then
                If Not SysCartoEncours.GrilleExiste Then
                    IsGrille.Checked = CartesSettings.IS_GRILLE
                Else
                    IsGrille.Checked = True
                End If
                IsGrille.Enabled = Not SysCartoEncours.GrilleExiste
                IsReferences.Enabled = True
                IsReferences.Checked = CartesSettings.IS_REFERENCES 'initialisation de l'index à sélectionner
            Else
                IsGrille.Checked = False
                IsGrille.Enabled = False
                IsReferences.Checked = False
                IsReferences.Enabled = False
            End If

            'initialisation des facteurs de niveau d'affichage
            CartesSettings.FACTEUR_JNX = FacteursSettings.Lire().FacteurJNX
            CartesSettings.FACTEUR_ORUX = FacteursSettings.Lire().FacteurORUX
            RemplirFacteur()

            'initialisation des tuiles
            DimTuilesLarg.Value = CartesSettings.DIMENSIONS_TUILE

            'initialisation des chemins d'enregistrement, pour être sur de ne pas avoir d'erreur à cause du chemin on en rajoute un par defaut
            RepertoireCartes.Text = CapturerSettings.CHEMIN_CARTE
            RepertoireTuiles.Text = PartagerSettings.CHEMIN_TUILE
            'pour obliger à saisir un nouveau nom
            NomCarte.Text = ""
            'mettre à jour les listes choix georef et choix ref
            ListeChoixGeoRefs.Items.AddRange(New String() {"Aucun", "Georef Carte", "MapInfo Carte", "OziExploreur Carte", "Compeland Carte", "QGIS Carte"})
            ListeChoixGeoRefs.SelectedIndex = CartesSettings.GEOREF 'initialisation de l'index à sélectionner
            ChercheChemin = New FolderBrowserDialog()

            NomCarte.Select()
            With NomCarte
                Cursor.Position = New Point(Location.X + .Bounds.X + .Bounds.Width \ 2, Location.Y + .Location.Y + .Size.Height + 16)
            End With
        Catch Ex As Exception
            AfficherErreur(Ex, "I0S9")
            Close()
        End Try
        'et limite les déplacements de la souris au formulaire
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary>vérifie que le nom de la carte est saisi et d'initialiser la structure captures et l'enregistrement des settings</summary>
    Private Sub Preferences_FormClosing(sender As Object, e As FormClosingEventArgs)
        'si on continue la carte en appuyant sur le 'bouton lance capture', on initialise les différentes variables après s'être assuré que le nom de la carte a été saisi
        If DialogResult = DialogResult.OK Then
            If Not VerifierSaisies() Then
                'on annule la fermeture du formulaire
                e.Cancel = True
            End If
        End If
    End Sub
    ''' <summary> ferme la boite de dialogue ave un validation éventuelle du choix </summary>
    Private Sub Preferences_FormClosed(sender As Object, e As FormClosedEventArgs)
        'si on continue la carte en appuyant sur le 'bouton lance capture', on initialise les différentes variables après s'être assuré que le nom de la carte a été saisi
        If DialogResult = DialogResult.OK Then
            Try
                'on enregistre les variables du settings pour la prochaine carte hormis le nom que l'on passe en paramètre
                If SysCartoEncours.AfficherGrilleIsOK Then
                    CartesSettings.IS_GRILLE = IsGrille.Checked
                    CartesSettings.IS_REFERENCES = IsReferences.Checked
                End If
                If IsTrace.Enabled Then CartesSettings.IS_TRACE = IsTrace.Checked
                CartesSettings.FACTEUR_JNX = FactJNX.Value
                CartesSettings.FACTEUR_ORUX = CInt(FactORUX.Value)
                CartesSettings.FICHIER_TUILE = IndexFichiersTuiles
                CartesSettings.DIMENSIONS_TUILE = CInt(DimTuilesLarg.Value)
                CartesSettings.GEOREF = ListeChoixGeoRefs.SelectedIndex
                'on ne prend en compte que le choix effectif de l'utilisateur
                If ListeChoixFormatCartes.SelectedIndex > -1 Then CartesSettings.FORMAT = ListeChoixFormatCartes.SelectedIndex
                CartesSettings.Ecrire()
                CapturerSettings.CHEMIN_CARTE = RepertoireCartes.Text
                CapturerSettings.Ecrire()
                PartagerSettings.CHEMIN_TUILE = RepertoireTuiles.Text
                PartagerSettings.Ecrire()
                FacteursSettings.Ecrire()
                Tag = NomCarte.Text '& If(IsTrace.Enabled AndAlso IsTrace.Checked, "_Trace", "")
            Catch Ex As Exception
                AfficherErreur(Ex, "V9P9")
            End Try
        End If
        Cursor.Clip = StockClipCurseur
        TitreInformation = Titre
    End Sub
    ''' <summary>sélectionne le chemin pour l'enrgistrement des 2 fichiers représentant une carte ou des tuiles </summary>
    Private Sub Repertoires_Click(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        Cursor.Clip = StockClipCurseur
        Try
            ChercheChemin.Description = "Chemin d'enregistrement des " & CStr(L.Tag)
            ChercheChemin.InitialDirectory = L.Text
            ChercheChemin.UseDescriptionForTitle = True
            ChercheChemin.ShowNewFolderButton = False
            If ChercheChemin.ShowDialog(Me) = DialogResult.OK Then
                If Directory.Exists(ChercheChemin.SelectedPath) Then L.Text = ChercheChemin.SelectedPath
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "V7P8")
        End Try
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> verifie les différentes saisies avant de valider la fermeture du fomulaire avec acceptation </summary>
    Private Function VerifierSaisies() As Boolean
        If NomCarte.Text & "" = "" Then
            MessageInformation = "Vous devez indiquer un nom pour la carte"
            AfficherInformation()
            NomCarte.Focus()
            Return False
        End If
        If Not Directory.Exists(RepertoireCartes.Text) Then
            MessageInformation = "Vous devez indiquer répertoire existant pour" & CrLf & "l'enregistrement de la carte"
            AfficherInformation()
            RepertoireCartes.ForeColor = Color.Red
            'ChoixCheminCartes.Focus()
            Return False
        End If
        If Not Directory.Exists(RepertoireTuiles.Text) Then
            MessageInformation = "Vous devez indiquer répertoire existant pour" & CrLf & "l'enregistrement des fichiers Tuiles"
            AfficherInformation()
            RepertoireTuiles.ForeColor = Color.Red
            'ChoixCheminTuile.Focus()
            Return False
        End If
        If ListeChoixGeoRefs.SelectedIndex = 0 And IsReferences.Checked = False And IndexFichiersTuiles = 0 Then
            MessageInformation = "Vous n'avez pas demandé de création de fichier après la capture"
            AfficherInformation()
            Return False
        End If
        Return True
    End Function
    ''' <summary> met à jour la variable du choix des fichiers tuiles en fonction du radio button coché </summary>
    Private Sub Tuiles_CheckedChanged(sender As Object, e As EventArgs)
        Dim Ind As Char = CType(sender, RadioButton).Name(7)
        IndexFichiersTuiles = Convert.ToInt32(Ind) - 48
    End Sub
    ''' <summary> limite les valeurs possibles de corrections à + ou - 3 par rapport à la valeur par défaut </summary>
    Private Sub RemplirFacteur()
        FactORUX.Minimum = SysCartoEncours.Niveau.NiveauAffichageORUX - 3
        FactORUX.Maximum = SysCartoEncours.Niveau.NiveauAffichageORUX + 3
        FactJNX.Minimum = CDec(SysCartoEncours.Niveau.NiveauAffichageJNX - 3)
        FactJNX.Maximum = CDec(SysCartoEncours.Niveau.NiveauAffichageJNX + 3)

        FactJNX.Value = CDec(CartesSettings.FACTEUR_JNX)
        If FactJNX.Value < FactJNX.Minimum Then FactJNX.Value = FactJNX.Minimum
        If FactJNX.Value > FactJNX.Maximum Then FactJNX.Value = FactJNX.Maximum

        FactORUX.Value = CartesSettings.FACTEUR_ORUX
        If FactORUX.Value < FactORUX.Minimum Then FactORUX.Value = FactORUX.Minimum
        If FactORUX.Value > FactORUX.Maximum Then FactORUX.Value = FactORUX.Maximum
    End Sub
    ''' <summary> indique d'une couleur différente la valeur par défaut des autres   </summary>
    Private Sub FactORUX_ValueChanged(sender As Object, e As EventArgs)
        If FactORUX.Value = SysCartoEncours.Niveau.NiveauAffichageORUX Then
            FactORUX.ForeColor = Color.YellowGreen
        Else
            FactORUX.ForeColor = Color.Tomato
        End If
    End Sub
    ''' <summary> indique d'une couleur différente la valeur par défaut des autres </summary>
    Private Sub FactJNX_ValueChanged(sender As Object, e As EventArgs)
        If FactJNX.Value = SysCartoEncours.Niveau.NiveauAffichageJNX Then
            FactJNX.ForeColor = Color.YellowGreen
        Else
            FactJNX.ForeColor = Color.Tomato
        End If
    End Sub
    ''' <summary> Gère IsReference en fonction du choix IsGrille </summary>
    Private Sub IsGrille_CheckedChanged(sender As Object, e As EventArgs)
        If IsGrille.Checked = False Then
            IsReferences.Checked = False
            IsReferences.Enabled = False
        Else
            IsReferences.Enabled = True
        End If
    End Sub
    ''' <summary> enlève la possibilité du choix 768</summary>
    Private Sub DimTuilesLarg_ValueChanged(sender As Object, e As EventArgs)
        If DimTuilesLarg.Value = 768 Then
            If DimensionsTuile > 768 Then
                DimTuilesLarg.Value = 512
            Else
                DimTuilesLarg.Value = 1024
            End If
        Else
            DimensionsTuile = CInt(DimTuilesLarg.Value)
            AutoriserFichiersTuiles()
        End If
    End Sub
    ''' <summary> ferme les listes déroulantes </summary>
    Private Sub Listes_DropDownClosed(sender As Object, e As EventArgs)
        Dim C As ComboBox = CType(sender, ComboBox)
        C.DroppedDown = False
        C.Visible = False
    End Sub
    ''' <summary> ouvre les listes déroulantes </summary>
    Private Sub Labels_Click(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label), C As ComboBox = CType(L.Tag, ComboBox)
        If C.Items.Count > 1 Then
            C.DroppedDown = True
            C.Show()
        End If
    End Sub
    ''' <summary> Met à jour le texte du label associé à la liste </summary>
    Private Sub Listes_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim C As ComboBox = CType(sender, ComboBox), L As Label = CType(C.Tag, Label)
        L.Text = C.SelectedItem.ToString()
    End Sub

    Private Sub AutoriserFichiersTuiles()
        'initialisation des fichiers tuiles
        Dim NotIsJnxKmz = IsMaxFichierTuiles_X(True) 'interpolation SM
        Dim NotIsOrux = IsMaxFichierTuiles_X(False)    'interpolation GF ou pas d'interpolation
        If NotIsOrux Then
            'on interdit tous les fichiers tuiles car le nb de tuiles sur l'axe des x est trop grand
            Tuiles_0.Enabled = True
            Tuiles_1.Enabled = False 'Orux
            Tuiles_2.Enabled = False 'Jnx
            Tuiles_4.Enabled = False 'Kmz
            Tuiles_7.Enabled = False 'Tous
            Tuiles_0.Checked = True
        ElseIf NotIsJnxKmz AndAlso CapturerSettings.SITE = SitesCartographiques.SuisseMobile Then
            'on choisit par défaut pas de fichiers tuiles
            If CartesSettings.FICHIER_TUILE = ChoixFichiersTuiles.Aucun OrElse CartesSettings.FICHIER_TUILE > ChoixFichiersTuiles.ORUX Then
                Tuiles_1.Checked = True
            Else CartesSettings.FICHIER_TUILE = ChoixFichiersTuiles.Aucun
                Tuiles_0.Checked = True
            End If
            'on interdit les fichiers JNX ou KMZ car le nb de tuiles sur l'axe des x est trop grand. Ne concerne que SM
            Tuiles_0.Enabled = True
            Tuiles_1.Enabled = True 'Orux
            Tuiles_2.Enabled = False 'Jnx
            Tuiles_4.Enabled = False 'Kmz
            Tuiles_7.Enabled = False 'Tous
        Else ' Pas de limites
            Tuiles_0.Enabled = True
            Tuiles_1.Enabled = True 'Orux
            Tuiles_2.Enabled = True 'Jnx
            Tuiles_4.Enabled = True 'Kmz
            Tuiles_7.Enabled = True 'Tous
            CType(GroupeReferences.Controls.Item("Tuiles_" & CartesSettings.FICHIER_TUILE), RadioButton).Checked = True
        End If
    End Sub
    ''' <summary> le nb de colonnes des fichiers tuiles a une limites qui est fonction de la taille du tampon, de la hauteur des tuiles et si
    ''' il s'agit d'une interpolation liée au site SuisseMobile.
    ''' Soit par la taille du tampon et par la hauteur des tuiles pour les sites web mercator et suisse Mobiles non interpolées.
    ''' Soit par la taille du tampon et un nombre définit empiriquement pour suisse Mobile et une interpolation. </summary>
    Private ReadOnly Property IsMaxFichierTuiles_X(FlagSM_Interpol As Boolean) As Boolean
        Get
            Dim Rapport = DimensionsTuile \ 256
            Dim NbTuilesFichierX = _SurfaceTraitement.NbColonnes \ Rapport
            If FlagSM_Interpol Then
                Dim NbTuiles As Integer = NbTuilesMaxCarteSuisseInterpol(TypeSupportCarte.Fichier) \ Rapport
                Return NbTuilesFichierX > NbTuiles
            Else
                If CartesSettings.SUPPORT_CARTE <> TypeSupportCarte.Fichier Then
                    Return NbTuilesFichierX > NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, Rapport * Rapport)
                Else
                    'dans le cas d'un support de fichier on est toujours bon car la vérification sur les X est faite
                    'au niveau de la surface de capture
                    Return False
                End If
            End If
        End Get
    End Property
End Class