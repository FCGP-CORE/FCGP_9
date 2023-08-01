Partial Class Configuration
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler AvecAltitude.CheckedChanged, AddressOf AvecAltitude_CheckedChanged
        AddHandler ID.TextChanged, AddressOf IDs_TextChanged
        AddHandler MP.TextChanged, AddressOf IDs_TextChanged
        AddHandler VerifierID.Click, AddressOf VerifierID_Click
        AddHandler CouleurFondCarte.Click, AddressOf Couleur_Click
        AddHandler CouleurFondAffichage.Click, AddressOf Couleur_Click
        AddHandler RepertoireAltitudes.Click, AddressOf RepertoireTuiles_Click
        AddHandler RepertoireTuiles.Click, AddressOf RepertoireTuiles_Click
        'EcranTravail
        AddHandler EcranTravail.Click, AddressOf EcranTravail_Click
        AddHandler ListeEcrans.DropDownClosed, AddressOf ListeEcrans_DropDownClosed
        AddHandler ListeEcrans.SelectedIndexChanged, AddressOf ListeEcrans_SelectedIndexChanged
        'formulaire
        AddHandler Load, AddressOf Configuration_Load
        AddHandler FormClosed, AddressOf Configuration_FormClosed
        AddHandler FormClosing, AddressOf Configuration_FormClosing
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
        Const URS As String = """https://urs.earthdata.nasa.gov/home"""
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Configuration))
        ToolTip1 = New ToolTip(components)
        Valider = New Button()
        Ignorer = New Button()
        PasDeplacement = New NumericUpDown()
        DEM1 = New CheckBox()
        AvecAltitude = New CheckBox()
        ID = New TextBox()
        MSGAltitude = New CheckBox()
        MP = New MaskedTextBox()
        CouleurFondCarte = New Label()
        CouleurFondAffichage = New Label()
        EcranTravail = New Label()
        VerifierID = New Label()
        RepertoireAltitudes = New Label()
        RepertoireTuiles = New Label()
        ListeEcrans = New ComboBox()
        ChoixCouleurTrace = New ColorDialog()
        Support = New Panel()
        L006 = New Label()
        L007 = New Label()
        PanelAltitude = New Panel()
        LabelID = New Label()
        LabelIdentifiantsNASA = New Label()
        LabelFichiersAltitude = New Label()
        LabelMP = New Label()
        LabelRepertoirefichiersAltitude = New Label()
        L004 = New Label()
        L001 = New Label()
        L002 = New Label()
        L003 = New Label()
        L005 = New Label()
        CType(PasDeplacement, ISupportInitialize).BeginInit()
        Support.SuspendLayout()
        PanelAltitude.SuspendLayout()
        SuspendLayout()

        '
        'L001
        '
        L001.BorderStyle = BorderStyle.FixedSingle
        L001.Location = New Point(6, 6)
        L001.Margin = New Padding(0)
        L001.Name = "L001"
        L001.Size = New Size(171, 23)
        L001.TabIndex = 79
        L001.Text = "Couleur fond Affichage"
        L001.TextAlign = ContentAlignment.MiddleCenter
        '
        'L002
        '
        L002.BorderStyle = BorderStyle.FixedSingle
        L002.Location = New Point(176, 6)
        L002.Margin = New Padding(0)
        L002.Name = "L002"
        L002.Size = New Size(172, 23)
        L002.TabIndex = 78
        L002.Text = "Couleur fond des Cartes"
        L002.TextAlign = ContentAlignment.MiddleCenter
        '
        'CouleurFondAffichage
        '
        CouleurFondAffichage.BorderStyle = BorderStyle.FixedSingle
        CouleurFondAffichage.Location = New Point(6, 28)
        CouleurFondAffichage.Name = "CouleurFondAffichage"
        CouleurFondAffichage.Size = New Size(171, 23)
        CouleurFondAffichage.TabIndex = 80
        ToolTip1.SetToolTip(CouleurFondAffichage, "Couleur actuelle de la fenêtre d'affichage.Cliquez pour choisir une autre couleur.")
        '
        'CouleurFondCarte
        '
        CouleurFondCarte.BorderStyle = BorderStyle.FixedSingle
        CouleurFondCarte.Location = New Point(176, 28)
        CouleurFondCarte.Name = "CouleurFondCarte"
        CouleurFondCarte.Size = New Size(172, 23)
        CouleurFondCarte.TabIndex = 83
        ToolTip1.SetToolTip(CouleurFondCarte, "Couleur actuelle des fonds de carte.Cliquez pour choisir une autre couleur.")
        '
        'L003
        '
        L003.BorderStyle = BorderStyle.FixedSingle
        L003.Location = New Point(6, 57)
        L003.Margin = New Padding(0)
        L003.Name = "L003"
        L003.Size = New Size(342, 23)
        L003.TabIndex = 76
        L003.Text = "Ecran de travail"
        L003.TextAlign = ContentAlignment.MiddleCenter
        '
        'EcranTravail
        '
        EcranTravail.BackColor = Color.White
        EcranTravail.BorderStyle = BorderStyle.FixedSingle
        EcranTravail.Location = New Point(6, 79)
        EcranTravail.Margin = New Padding(0)
        EcranTravail.Name = "EcranTravail"
        EcranTravail.Size = New Size(342, 23)
        EcranTravail.TabIndex = 118
        EcranTravail.Text = "EcranTravail"
        EcranTravail.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(EcranTravail, "Ecran de travail actuel. Cliquez pour voir la liste des écrans disponibles.")
        '
        'ListeEcrans
        '
        ListeEcrans.FormattingEnabled = True
        ListeEcrans.Visible = False
        ListeEcrans.Location = New Point(6, 74)
        ListeEcrans.Name = "ListeEcrans"
        ListeEcrans.Size = New Size(342, 24)
        ListeEcrans.TabIndex = 72
        '
        'L004
        '
        L004.BorderStyle = BorderStyle.FixedSingle
        L004.Location = New Point(6, 108)
        L004.Margin = New Padding(0)
        L004.Name = "L004"
        L004.Size = New Size(231, 24)
        L004.TabIndex = 100
        L004.Text = "Pas de déplacement"
        L004.TextAlign = ContentAlignment.MiddleCenter
        '
        'L005
        '
        L005.BorderStyle = BorderStyle.FixedSingle
        L005.Name = "L005"
        L005.Location = New Point(236, 108)
        L005.Size = New Size(112, 24)
        '
        'PasDeplacement
        '
        PasDeplacement.BackColor = Color.White
        PasDeplacement.BorderStyle = BorderStyle.None
        PasDeplacement.Location = New Point(237, 109)
        PasDeplacement.Name = "PasDeplacement"
        PasDeplacement.Size = New Size(110, 22)
        PasDeplacement.TabIndex = 112
        PasDeplacement.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(PasDeplacement, "Transparence du dessin de la trace.0% --> Opaque100% --> Invisible")
        PasDeplacement.Value = 25D
        '
        'L006
        '
        L006.BorderStyle = BorderStyle.FixedSingle
        L006.Location = New Point(6, 138)
        L006.Margin = New Padding(0)
        L006.Name = "L006"
        L006.Size = New Size(342, 23)
        L006.TabIndex = 122
        L006.Text = "Répertoire d'enregistrement des Fichiers Tuiles"
        L006.TextAlign = ContentAlignment.BottomCenter
        '
        'RepertoireTuiles
        '
        RepertoireTuiles.BackColor = Color.White
        RepertoireTuiles.BorderStyle = BorderStyle.FixedSingle
        RepertoireTuiles.Location = New Point(6, 160)
        RepertoireTuiles.Name = "RepertoireTuiles"
        RepertoireTuiles.Size = New Size(342, 23)
        RepertoireTuiles.TabIndex = 121
        RepertoireTuiles.TextAlign = ContentAlignment.MiddleCenter
        RepertoireTuiles.Tag = "Répertoire d'enregistrement des Fichiers Tuiles"
        ToolTip1.SetToolTip(RepertoireTuiles, "Répertoire d'enregistrement actuel des fichiers Tuiles" & CrLf &
                                              "Cliquez pour changer de répertoire.")
        '
        'L007
        '
        L007.BackColor = Color.Transparent
        L007.BorderStyle = BorderStyle.FixedSingle
        L007.Location = New Point(6, 189)
        L007.Margin = New Padding(0)
        L007.Name = "Label3"
        L007.Size = New Size(342, 25)
        L007.TabIndex = 120
        L007.TextAlign = ContentAlignment.MiddleCenter
        '
        'AvecAltitude
        '
        AvecAltitude.BackColor = Color.Transparent
        AvecAltitude.CheckAlign = ContentAlignment.MiddleRight
        AvecAltitude.Location = New Point(9, 192)
        AvecAltitude.Margin = New Padding(0)
        AvecAltitude.Name = "AvecAltitude"
        AvecAltitude.Size = New Size(330, 21)
        AvecAltitude.TabIndex = 86
        AvecAltitude.Text = "Afficher l'altitude avec les coordonnées"
        AvecAltitude.TextAlign = ContentAlignment.TopLeft
        ToolTip1.SetToolTip(AvecAltitude, "Indique si FCGP_Visue doit afficher l'altitude" & CrLf &
                                          "avec les coordonnéeslors du déplacement du pointeur de souris.")
        AvecAltitude.UseVisualStyleBackColor = False
        '
        'LabelIdentifiantsNASA
        '
        LabelIdentifiantsNASA.BorderStyle = BorderStyle.FixedSingle
        LabelIdentifiantsNASA.Location = New Point(0, 0)
        LabelIdentifiantsNASA.Margin = New Padding(0)
        LabelIdentifiantsNASA.Name = "LabelIdentifiantsNASA"
        LabelIdentifiantsNASA.Size = New Size(231, 21)
        LabelIdentifiantsNASA.TabIndex = 92
        LabelIdentifiantsNASA.Text = "Identifiants du site de la NASA"
        LabelIdentifiantsNASA.TextAlign = ContentAlignment.BottomCenter
        '
        'LabelFichiersAltitude
        '
        LabelFichiersAltitude.BorderStyle = BorderStyle.FixedSingle
        LabelFichiersAltitude.Location = New Point(230, 0)
        LabelFichiersAltitude.Name = "LabelFichiersAltitude"
        LabelFichiersAltitude.Size = New Size(112, 67)
        LabelFichiersAltitude.TabIndex = 115
        LabelFichiersAltitude.TextAlign = ContentAlignment.TopCenter
        '
        'LabelID
        '
        'LabelID.BackColor = Color.Transparent
        LabelID.BorderStyle = BorderStyle.FixedSingle
        LabelID.Location = New Point(0, 20)
        LabelID.Name = "LabelID"
        LabelID.Size = New Size(33, 24)
        LabelID.TabIndex = 90
        LabelID.Text = "ID "
        LabelID.TextAlign = ContentAlignment.MiddleLeft
        '
        'ID
        '
        ID.BackColor = Color.White
        ID.Location = New Point(32, 18)
        ID.Name = "ID"
        ID.Size = New Size(165, 24)
        ID.TabIndex = 87
        ToolTip1.SetToolTip(ID, $"Indiquez le nom d'utilisateur de votre compte sur le site de données{CrLf}de la NASA<{URS}> .")
        '
        'LabelMP
        '
        LabelMP.BackColor = Color.Transparent
        LabelMP.BorderStyle = BorderStyle.FixedSingle
        LabelMP.Location = New Point(0, 43)
        LabelMP.Name = "LabelMP"
        LabelMP.Size = New Size(33, 24)
        LabelMP.TabIndex = 91
        LabelMP.Text = "MP "
        LabelMP.TextAlign = ContentAlignment.MiddleLeft
        '
        'MP
        '
        MP.BackColor = Color.White
        MP.Location = New Point(32, 41)
        MP.Name = "MP"
        MP.PasswordChar = "*"c
        MP.Size = New Size(165, 24)
        MP.TabIndex = 88
        ToolTip1.SetToolTip(MP, $"Indiquez le mot de passe de votre compte sur le site de données{CrLf}de la NASA<{URS}>.")
        '
        'VerifierID
        '
        VerifierID.BorderStyle = BorderStyle.FixedSingle
        VerifierID.Location = New Point(196, 20)
        VerifierID.Name = "VerifierID"
        VerifierID.Size = New Size(35, 47)
        VerifierID.TabIndex = 113
        VerifierID.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(VerifierID, $"Vérifie que les identifiants indiqués sont reconnus par{CrLf}le serveur de données de la NASA<{URS}>.")
        '
        'DEM1
        '
        DEM1.Location = New Point(233, 10)
        DEM1.Margin = New Padding(0)
        DEM1.Name = "DEM1"
        DEM1.RightToLeft = RightToLeft.Yes
        DEM1.Size = New Size(100, 22)
        DEM1.TabIndex = 96
        DEM1.Text = "Type Arc 1"
        DEM1.TextAlign = ContentAlignment.MiddleRight
        ToolTip1.SetToolTip(DEM1, resources.GetString("DEM1.ToolTip"))
        DEM1.UseVisualStyleBackColor = False
        '
        'MSGAltitude
        '
        MSGAltitude.Location = New Point(233, 37)
        MSGAltitude.Margin = New Padding(0)
        MSGAltitude.Name = "MSGAltitude"
        MSGAltitude.RightToLeft = RightToLeft.Yes
        MSGAltitude.Size = New Size(100, 22)
        MSGAltitude.TabIndex = 116
        MSGAltitude.Text = "Messages"
        MSGAltitude.TextAlign = ContentAlignment.MiddleRight
        ToolTip1.SetToolTip(MSGAltitude, "Indique si les messages liés au téléchargement" & CrLf &
                                         "des fichiers d'altitudes sont affichés.")
        MSGAltitude.UseVisualStyleBackColor = False
        '
        'LabelRepertoirefichiersAltitude
        '
        LabelRepertoirefichiersAltitude.BackColor = Color.Transparent
        LabelRepertoirefichiersAltitude.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoirefichiersAltitude.Location = New Point(0, 66)
        LabelRepertoirefichiersAltitude.Margin = New Padding(0)
        LabelRepertoirefichiersAltitude.Name = "LabelRepertoirefichiersAltitude"
        LabelRepertoirefichiersAltitude.Size = New Size(342, 23)
        LabelRepertoirefichiersAltitude.TabIndex = 95
        LabelRepertoirefichiersAltitude.Text = "Répertoire d'enregistrement des fichiers d'altitudes"
        LabelRepertoirefichiersAltitude.TextAlign = ContentAlignment.MiddleCenter
        '
        'RepertoireAltitudes
        '
        RepertoireAltitudes.BackColor = Color.White
        RepertoireAltitudes.BorderStyle = BorderStyle.FixedSingle
        RepertoireAltitudes.Location = New Point(0, 88)
        RepertoireAltitudes.Name = "RepertoireAltitudes"
        RepertoireAltitudes.Size = New Size(342, 22)
        RepertoireAltitudes.TabIndex = 94
        RepertoireAltitudes.Tag = "Répertoire d'enregistrement des Fichiers d'Altitude"
        ToolTip1.SetToolTip(RepertoireAltitudes, "Répertoire d'enregistrement actuel des fichiers d'altitude (.hgt)" & CrLf &
                                                 "Cliquez pour changer de répertoire.")
        '
        'PanelAltitude
        '
        PanelAltitude.Controls.Add(LabelID)
        PanelAltitude.Controls.Add(LabelIdentifiantsNASA)
        PanelAltitude.Controls.Add(DEM1)
        PanelAltitude.Controls.Add(MSGAltitude)
        PanelAltitude.Controls.Add(LabelFichiersAltitude)
        PanelAltitude.Controls.Add(LabelMP)
        PanelAltitude.Controls.Add(RepertoireAltitudes)
        PanelAltitude.Controls.Add(VerifierID)
        PanelAltitude.Controls.Add(LabelRepertoirefichiersAltitude)
        PanelAltitude.Controls.Add(ID)
        PanelAltitude.Controls.Add(MP)
        PanelAltitude.BorderStyle = BorderStyle.None
        PanelAltitude.Location = New Point(6, 213)
        PanelAltitude.Name = "PanelAltitude"
        PanelAltitude.Size = New Size(342, 110)
        PanelAltitude.TabIndex = 119
        '
        'Support
        '
        Support.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Controls.Add(L006)
        Support.Controls.Add(RepertoireTuiles)
        Support.Controls.Add(AvecAltitude)
        Support.Controls.Add(L007)
        Support.Controls.Add(PanelAltitude)
        Support.Controls.Add(EcranTravail)
        Support.Controls.Add(L004)
        Support.Controls.Add(PasDeplacement)
        Support.Controls.Add(L005)
        Support.Controls.Add(CouleurFondCarte)
        Support.Controls.Add(CouleurFondAffichage)
        Support.Controls.Add(L001)
        Support.Controls.Add(L002)
        Support.Controls.Add(L003)
        Support.Controls.Add(ListeEcrans)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(354, 329)
        Support.TabIndex = 72
        '
        'Ok
        '
        Valider.Anchor = AnchorStyles.Bottom
        Valider.DialogResult = DialogResult.OK
        Valider.Location = New Point(273, 335)
        Valider.Name = "Ok"
        Valider.Size = New Size(75, 33)
        Valider.TabIndex = 13
        Valider.Text = "Valider"
        ToolTip1.SetToolTip(Valider, "Ferme le formualaire en prenant en compte les modifications.")
        Valider.UseVisualStyleBackColor = True
        '
        'Ignore
        '
        Ignorer.Anchor = AnchorStyles.Bottom
        Ignorer.DialogResult = DialogResult.Abort
        Ignorer.Location = New Point(192, 335)
        Ignorer.Name = "Ignore"
        Ignorer.Size = New Size(75, 33)
        Ignorer.TabIndex = 12
        Ignorer.Text = "Annuler"
        ToolTip1.SetToolTip(Ignorer, "Ferme le formulaire sans prendre en compte les modifications.")
        Ignorer.UseVisualStyleBackColor = True
        '
        'ToolTip1
        '
        ToolTip1.AutoPopDelay = 5000
        ToolTip1.InitialDelay = 100
        ToolTip1.ReshowDelay = 100
        '
        'Configuration
        '
        AcceptButton = Valider
        AutoScaleMode = AutoScaleMode.None
        AutoSizeMode = AutoSizeMode.GrowOnly
        CancelButton = Ignorer
        ClientSize = New Size(354, 374)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(Ignorer)
        Controls.Add(Valider)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        MaximizeBox = False
        MinimizeBox = False
        Name = "Configuration"
        ShowIcon = False
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.CenterParent
        'TopMost = True
        CType(PasDeplacement, ISupportInitialize).EndInit()
        Support.ResumeLayout(False)
        PanelAltitude.ResumeLayout(False)
        PanelAltitude.PerformLayout()
        ResumeLayout(False)
    End Sub

    Private ToolTip1 As ToolTip
    Private Valider As Button
    Private Ignorer As Button
    Private ChoixCouleurTrace As ColorDialog
    Private Support As Panel
    Private VerifierID As Label
    Private PasDeplacement As NumericUpDown
    Private L004 As Label
    Private DEM1 As CheckBox
    Private LabelRepertoirefichiersAltitude As Label
    Private RepertoireAltitudes As Label
    Private AvecAltitude As CheckBox
    Private ID As TextBox
    Private LabelIdentifiantsNASA As Label
    Private MSGAltitude As CheckBox
    Private LabelFichiersAltitude As Label
    Private MP As MaskedTextBox
    Private LabelMP As Label
    Private LabelID As Label
    Private CouleurFondCarte As Label
    Private CouleurFondAffichage As Label
    Private L001 As Label
    Private L002 As Label
    Private L003 As Label
    Private ListeEcrans As ComboBox
    Private EcranTravail As Label
    Private PanelAltitude As Panel
    Private L007 As Label
    Private L006 As Label
    Private RepertoireTuiles As Label
    Private L005 As Label
End Class