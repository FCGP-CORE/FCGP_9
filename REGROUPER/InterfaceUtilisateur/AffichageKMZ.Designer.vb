Partial Class AffichageKMZ
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiseEvenements()
    End Sub
    Private Sub InitialiseEvenements()
        'contrôles simples
        AddHandler Choisir.Click, AddressOf ChoixFichierKMZ_Click
        AddHandler Modifier.Click, AddressOf ModifierTuiles_Click
        AddHandler Selectionner.Click, AddressOf Selectionner_Click
        AddHandler Load, AddressOf AffichageKMZ_Load
        AddHandler FormClosed, AddressOf AffichageKMZ_FormClosed
    End Sub
    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    Private Sub InitializeComponent()
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(AffichageKMZ))
        Groupe = New Panel()
        LabelInformation = New Label()
        Label4 = New Label()
        Label3 = New Label()
        Label2 = New Label()
        Choisir = New Button()
        Modifier = New Button()
        Quitter = New Button()
        ToolTip1 = New ToolTip(components)
        Selectionner = New Button()
        Label5 = New Label()
        TailleTuile = New Label()
        TailleFichier = New Label()
        NbTuilesFichier = New Label()
        NbTuilesSelect = New Label()
        ChercheFichierKMZ = New OpenFileDialog()
        Label1 = New Label()
        NomFichier = New Label()
        Label7 = New Label()
        Groupe.SuspendLayout()
        SuspendLayout()
        '
        'LabelInformation
        '
        LabelInformation.BackColor = Color.Transparent
        LabelInformation.BorderStyle = BorderStyle.FixedSingle
        LabelInformation.Location = New Point(6, 6)
        LabelInformation.Name = "LabelInformation"
        LabelInformation.Size = New Size(412, 29)
        LabelInformation.TabIndex = 0
        LabelInformation.Text = "Informations fichier sélectionné"
        LabelInformation.TextAlign = ContentAlignment.MiddleCenter
        '
        'Label7
        '
        Label7.BackColor = Color.Transparent
        Label7.BorderStyle = BorderStyle.FixedSingle
        Label7.Location = New Point(6, 34)
        Label7.Name = "Label7"
        Label7.Size = New Size(207, 23)
        Label7.TabIndex = 0
        Label7.Text = "Nom du Fichier"
        Label7.TextAlign = ContentAlignment.MiddleCenter
        '
        'NomFichier
        '
        NomFichier.BackColor = Color.Transparent
        NomFichier.BorderStyle = BorderStyle.FixedSingle
        NomFichier.Location = New Point(212, 34)
        NomFichier.Name = "NomFichier"
        NomFichier.Size = New Size(206, 23)
        NomFichier.TabIndex = 0
        NomFichier.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NomFichier, "Nom du fichier KMZ à traiter")
        '
        'Label2
        '
        Label2.BackColor = Color.Transparent
        Label2.BorderStyle = BorderStyle.FixedSingle
        Label2.Location = New Point(6, 56)
        Label2.Name = "Label2"
        Label2.Size = New Size(207, 23)
        Label2.TabIndex = 0
        Label2.Text = "Nb Tuiles du Fichier"
        Label2.TextAlign = ContentAlignment.MiddleCenter
        '
        'NbTuilesFichier
        '
        NbTuilesFichier.BackColor = Color.Transparent
        NbTuilesFichier.BorderStyle = BorderStyle.FixedSingle
        NbTuilesFichier.Location = New Point(212, 56)
        NbTuilesFichier.Name = "NbTuilesFichier"
        NbTuilesFichier.Size = New Size(206, 23)
        NbTuilesFichier.TabIndex = 0
        NbTuilesFichier.Text = "0"
        NbTuilesFichier.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbTuilesFichier, "Nb de tuiles du fichier KMZ (Colonnes * Rangées)")
        '
        'Label3
        '
        Label3.BackColor = Color.Transparent
        Label3.BorderStyle = BorderStyle.FixedSingle
        Label3.Location = New Point(6, 78)
        Label3.Name = "Label3"
        Label3.Size = New Size(207, 23)
        Label3.TabIndex = 0
        Label3.Text = "Taille du Fichier en pixels"
        Label3.TextAlign = ContentAlignment.MiddleCenter
        '
        'TailleFichier
        '
        TailleFichier.BackColor = Color.Transparent
        TailleFichier.BorderStyle = BorderStyle.FixedSingle
        TailleFichier.Location = New Point(212, 78)
        TailleFichier.Name = "TailleFichier"
        TailleFichier.Size = New Size(206, 23)
        TailleFichier.TabIndex = 0
        TailleFichier.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(TailleFichier, "Taille de la carte en pixels")
        '
        'Label5
        '
        Label5.BackColor = Color.Transparent
        Label5.BorderStyle = BorderStyle.FixedSingle
        Label5.Location = New Point(6, 100)
        Label5.Name = "Label5"
        Label5.Size = New Size(207, 23)
        Label5.TabIndex = 0
        Label5.Text = "Taille des Tuiles en pixels"
        Label5.TextAlign = ContentAlignment.MiddleCenter
        '
        'TailleTuile
        '
        TailleTuile.BackColor = Color.Transparent
        TailleTuile.BorderStyle = BorderStyle.FixedSingle
        TailleTuile.Location = New Point(212, 100)
        TailleTuile.Name = "TailleTuile"
        TailleTuile.Size = New Size(206, 23)
        TailleTuile.TabIndex = 0
        TailleTuile.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(TailleTuile, "Taille des tuiles en pixels")
        '
        'Label1
        '
        Label1.BackColor = Color.Transparent
        Label1.BorderStyle = BorderStyle.FixedSingle
        Label1.Location = New Point(6, 122)
        Label1.Name = "Label1"
        Label1.Size = New Size(412, 29)
        Label1.TabIndex = 0
        Label1.Text = "Tuiles à supprimer"
        Label1.TextAlign = ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Label4.BackColor = Color.Transparent
        Label4.BorderStyle = BorderStyle.FixedSingle
        Label4.Location = New Point(6, 150)
        Label4.Name = "Label4"
        Label4.Size = New Size(207, 23)
        Label4.TabIndex = 0
        Label4.Text = "Nb Tuiles sélectionnées"
        Label4.TextAlign = ContentAlignment.MiddleCenter
        '
        'NbTuilesSelect
        '
        NbTuilesSelect.BackColor = Color.Transparent
        NbTuilesSelect.BorderStyle = BorderStyle.FixedSingle
        NbTuilesSelect.Location = New Point(212, 150)
        NbTuilesSelect.Name = "NbTuilesSelect"
        NbTuilesSelect.Size = New Size(206, 23)
        NbTuilesSelect.TabIndex = 0
        NbTuilesSelect.Tag = ""
        NbTuilesSelect.Text = "0"
        NbTuilesSelect.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbTuilesSelect, "Nb de tuiles sélectionnées pour être supprimées")
        '
        'Groupe
        '
        Groupe.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        Groupe.BackColor = Color.FromArgb(247, 247, 247)
        Controls.Add(NomFichier)
        Controls.Add(Label7)
        Controls.Add(NbTuilesSelect)
        Controls.Add(TailleTuile)
        Controls.Add(TailleFichier)
        Controls.Add(NbTuilesFichier)
        Controls.Add(Label5)
        Controls.Add(Label1)
        Controls.Add(LabelInformation)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Groupe.Location = New Point(0, 0)
        Groupe.Name = "Groupe"
        Groupe.Size = New Size(424, 179)
        Groupe.TabIndex = 0
        '
        'Choisir
        '
        Choisir.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Choisir.Location = New Point(6, 185)
        Choisir.Name = "Choisir"
        Choisir.Size = New Size(100, 46)
        Choisir.TabIndex = 1
        Choisir.Text = "Choisir" & CrLf & "Fichier KMZ"
        ToolTip1.SetToolTip(Choisir, "Ouvre un formulaire qui permet de choisir un fichier KMZ.")
        Choisir.UseVisualStyleBackColor = True
        '
        'Selectionner
        '
        Selectionner.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Selectionner.Location = New Point(110, 185)
        Selectionner.Name = "Selectionner"
        Selectionner.Size = New Size(100, 46)
        Selectionner.TabIndex = 2
        Selectionner.Text = "Selectionner" & CrLf & "Tuiles"
        ToolTip1.SetToolTip(Selectionner, "Ouvre le plan de calepinage des tuiles du fichier KMZ" & CrLf &
                                          "pour sélectionner les tuiles à supprimer")
        Selectionner.UseVisualStyleBackColor = True
        '
        'Modifier
        '
        Modifier.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Modifier.Location = New Point(214, 185)
        Modifier.Name = "Modifier"
        Modifier.Size = New Size(100, 46)
        Modifier.TabIndex = 3
        Modifier.Text = "Modifier" & CrLf & "Fichier KMZ"
        ToolTip1.SetToolTip(Modifier, "Supprime les tuiles sélectionnées du ficheir KMZ")
        Modifier.UseVisualStyleBackColor = True
        '
        'Quitter
        '
        Quitter.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Quitter.DialogResult = DialogResult.Cancel
        Quitter.Location = New Point(318, 185)
        Quitter.Name = "Quitter"
        Quitter.Size = New Size(100, 46)
        Quitter.TabIndex = 4
        Quitter.Text = "Quitter"
        ToolTip1.SetToolTip(Quitter, "Ferme le formulaire")
        Quitter.UseVisualStyleBackColor = True
        '
        'AffichageKMZ
        '
        AutoScaleMode = AutoScaleMode.None
        AutoSize = True
        CancelButton = Quitter
        AcceptButton = Choisir
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ClientSize = New Size(424, 236)
        ControlBox = False
        Controls.Add(Groupe)
        Controls.Add(Selectionner)
        Controls.Add(Quitter)
        Controls.Add(Modifier)
        Controls.Add(Choisir)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "AffichageKMZ"
        StartPosition = FormStartPosition.CenterParent
        Text = "Suppression Tuiles des Fichiers KMZ"
        Groupe.ResumeLayout(False)
        Groupe.PerformLayout()
        ResumeLayout(False)
    End Sub

    Private LabelInformation As Label
    Private Label4 As Label
    Private Label3 As Label
    Private Label2 As Label
    Private Choisir As Button
    Private Modifier As Button
    Private Quitter As Button
    Private ToolTip1 As ToolTip
    Private ChercheFichierKMZ As OpenFileDialog
    Private Selectionner As Button
    Private Label1 As Label
    Private Label5 As Label
    Private TailleTuile As Label
    Private TailleFichier As Label
    Private NbTuilesFichier As Label
    Private NbTuilesSelect As Label
    Private NomFichier As Label
    Private Label7 As Label
    Private Groupe As Panel
End Class
