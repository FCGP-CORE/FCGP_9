Partial Class Configuration
    Inherits Form

    Const MsgUri As String = "<https://urs.earthdata.nasa.gov/home>."
    Const MsgChemin As String = "Cliquez pour changer de chemin."
    Const MsgCouleur As String = "Cliquez pour choisir une autre couleur."
    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    ''' <summary> Initialise l'ensemble des évenements liés au formulaire car pas variable avec le modificateur WithEvents 
    ''' rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
    Private Sub InitialiserEvenements()
        'controles simples
        AddHandler CouleurCartes.Click, AddressOf Couleurs_Click
        AddHandler CouleurAffichage.Click, AddressOf Couleurs_Click
        AddHandler IsToutFormat.CheckedChanged, AddressOf IsToutFormat_CheckedChanged
        AddHandler RepertoireCachesTuiles.Click, AddressOf Repertoires_Click
        AddHandler AvecAltitudes.CheckedChanged, AddressOf AvecAltitude_CheckedChanged
        AddHandler ID.TextChanged, AddressOf IDs_TextChanged
        AddHandler MP.TextChanged, AddressOf IDs_TextChanged
        AddHandler VerifierID.Click, AddressOf VerifierID_Click
        AddHandler RepertoireFichiersAltitudes.Click, AddressOf Repertoires_Click
        AddHandler CouleurTraces.Click, AddressOf Couleurs_Click
        AddHandler CouleurDessinTraces.Click, AddressOf Couleurs_Click
        AddHandler CoefAlphaTraces.ValueChanged, AddressOf TransparenceTrace_ValueChanged
        AddHandler RepertoireTraces.Click, AddressOf Repertoires_Click
        'EcranTravail
        AddHandler LabelChoixEcranTravail.Click, AddressOf Labels_Click
        AddHandler ListeChoixEcranTravail.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
        'SupportCarte
        AddHandler LabelChoixSupportCarte.Click, AddressOf Labels_Click
        AddHandler ListeChoixSupportCarte.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
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
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Configuration))
        PanelSupport = New Panel()
        LabelCouleurAffichage = New Label()
        CouleurAffichage = New Label()
        LabelCouleurCartes = New Label()
        CouleurCartes = New Label()
        LabelEcranTravail = New Label()
        LabelChoixEcranTravail = New Label()
        ListeChoixEcranTravail = New ComboBox()
        LabelJpeg = New Label()
        LabelQualiteJpeg = New Label()
        QualiteJpeg = New NumericUpDown
        LabelSupportCarte = New Label()
        LabelChoixSupportCarte = New Label()
        ListeChoixSupportCarte = New ComboBox()
        LabelRepertoireChachesTuiles = New Label()
        RepertoireCachesTuiles = New Label()
        IsToutFormat = New CheckBox()
        LabelIsToutFormat = New Label()
        AvecAltitudes = New CheckBox()
        LabelAvecAltitudes = New Label()
        PanelAltitudes = New Panel()
        LabelIdentifiantsNASA = New Label()
        LabelID = New Label()
        ID = New TextBox()
        LabelMP = New Label()
        MP = New MaskedTextBox()
        VerifierID = New Label()
        DEM1 = New CheckBox()
        MSGAltitudes = New CheckBox()
        LabelDonneesAltitudes = New Label()
        LabelRepertoireFichiersAltitudes = New Label()
        RepertoireFichiersAltitudes = New Label()
        LabelCouleurTraces = New Label()
        CouleurTraces = New Label()
        LabelAspectTraces = New Label()
        LabelCouleurDessinTraces = New Label()
        CouleurDessinTraces = New Label()
        LabelCoefAlphaTraces = New Label()
        CoefAlphaTraces = New NumericUpDown()
        LabelEpaisseurTraces = New Label()
        EpaisseurTraces = New NumericUpDown()
        LabelRepertoireTraces = New Label()
        RepertoireTraces = New Label()
        BoutonEnter = New Button()
        BoutonEscape = New Button()
        ToolTip1 = New ToolTip(components)
        CType(QualiteJpeg, ISupportInitialize).BeginInit()
        CType(CoefAlphaTraces, ISupportInitialize).BeginInit()
        CType(EpaisseurTraces, ISupportInitialize).BeginInit()
        PanelSupport.SuspendLayout()
        PanelAltitudes.SuspendLayout()
        SuspendLayout()
        '
        'LabelCouleurAffichage
        '
        LabelCouleurAffichage.BackColor = Color.Transparent
        LabelCouleurAffichage.BorderStyle = BorderStyle.FixedSingle
        LabelCouleurAffichage.Location = New Point(6, 6)
        LabelCouleurAffichage.Margin = New Padding(0)
        LabelCouleurAffichage.Name = "LabelCouleurAffichage"
        LabelCouleurAffichage.Size = New Size(171, 21)
        LabelCouleurAffichage.TabIndex = 79
        LabelCouleurAffichage.Text = "Couleur fond Affichage"
        LabelCouleurAffichage.TextAlign = ContentAlignment.MiddleCenter
        '
        'CouleurAffichage
        '
        CouleurAffichage.BackColor = Color.White
        CouleurAffichage.BorderStyle = BorderStyle.FixedSingle
        CouleurAffichage.Location = New Point(6, 26)
        CouleurAffichage.Name = "CouleurAffichage"
        CouleurAffichage.Size = New Size(171, 21)
        CouleurAffichage.TabIndex = 80
        CouleurAffichage.Tag = False
        ToolTip1.SetToolTip(CouleurAffichage, "Couleur actuelle de la fenêtre d'affichage." & CrLf & MsgCouleur)
        '
        'LabeCouleurCartes
        '
        LabelCouleurCartes.BackColor = Color.Transparent
        LabelCouleurCartes.BorderStyle = BorderStyle.FixedSingle
        LabelCouleurCartes.Location = New Point(176, 6)
        LabelCouleurCartes.Margin = New Padding(0)
        LabelCouleurCartes.Name = "LabeCouleurCartes"
        LabelCouleurCartes.Size = New Size(172, 21)
        LabelCouleurCartes.TabIndex = 78
        LabelCouleurCartes.Text = "Couleur fond des Cartes"
        LabelCouleurCartes.TextAlign = ContentAlignment.MiddleCenter
        '
        'CouleurCartes
        '
        CouleurCartes.BackColor = Color.White
        CouleurCartes.BorderStyle = BorderStyle.FixedSingle
        CouleurCartes.Location = New Point(176, 26)
        CouleurCartes.Name = "CouleurCartes"
        CouleurCartes.Size = New Size(172, 21)
        CouleurCartes.TabIndex = 83
        CouleurCartes.Tag = False
        ToolTip1.SetToolTip(CouleurCartes, "Couleur actuelle des fonds de carte." & CrLf & MsgCouleur)
        '
        'LabelEcranTravail
        '
        LabelEcranTravail.BackColor = Color.Transparent
        LabelEcranTravail.BorderStyle = BorderStyle.FixedSingle
        LabelEcranTravail.Location = New Point(6, 52)
        LabelEcranTravail.Margin = New Padding(0)
        LabelEcranTravail.Name = "LabelEcranTravail"
        LabelEcranTravail.Size = New Size(342, 21)
        LabelEcranTravail.TabIndex = 76
        LabelEcranTravail.Text = "Ecran de travail"
        LabelEcranTravail.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelChoixEcranTravail
        '
        LabelChoixEcranTravail.BackColor = Color.White
        LabelChoixEcranTravail.BorderStyle = BorderStyle.FixedSingle
        LabelChoixEcranTravail.Location = New Point(6, 72)
        LabelChoixEcranTravail.Margin = New Padding(0)
        LabelChoixEcranTravail.Name = "EcranTravail"
        LabelChoixEcranTravail.Size = New Size(342, 21)
        LabelChoixEcranTravail.TabIndex = 118
        LabelChoixEcranTravail.Text = "LabelChoixEcranTravail"
        LabelChoixEcranTravail.TextAlign = ContentAlignment.MiddleCenter
        LabelChoixEcranTravail.Tag = ListeChoixEcranTravail
        ToolTip1.SetToolTip(LabelChoixEcranTravail, "Ecran de travail actuel. " & CrLf &
                                                    "Cliquez pour voir la liste des écrans disponibles.")
        '
        'ListeChoixEcranTravail
        '
        ListeChoixEcranTravail.BackColor = Color.White
        ListeChoixEcranTravail.FormattingEnabled = True
        ListeChoixEcranTravail.Location = New Point(6, 65)
        ListeChoixEcranTravail.Name = "ListeChoixEcranTravail"
        ListeChoixEcranTravail.Size = New Size(342, 0)
        ListeChoixEcranTravail.Visible = False
        ListeChoixEcranTravail.TabIndex = 72
        ListeChoixEcranTravail.Tag = LabelChoixEcranTravail
        '
        'LabelJpeg
        '
        LabelJpeg.BackColor = Color.Transparent
        LabelJpeg.BorderStyle = BorderStyle.FixedSingle
        LabelJpeg.Location = New Point(6, 99)
        LabelJpeg.Margin = New Padding(0)
        LabelJpeg.Name = "LabelJpeg"
        LabelJpeg.Size = New Size(90, 21)
        LabelJpeg.TabIndex = 77
        LabelJpeg.Text = "Qualité Jpeg"
        LabelJpeg.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelQualiteJpeg
        '
        LabelQualiteJpeg.BackColor = Color.White
        LabelQualiteJpeg.BorderStyle = BorderStyle.FixedSingle
        LabelQualiteJpeg.Location = New Point(6, 119)
        LabelQualiteJpeg.Margin = New Padding(0)
        LabelQualiteJpeg.Name = "LabelQualiteJpeg"
        LabelQualiteJpeg.Size = New Size(90, 24)
        LabelQualiteJpeg.TabIndex = 117
        LabelQualiteJpeg.TextAlign = ContentAlignment.MiddleCenter
        '
        'QualiteJpeg
        '
        QualiteJpeg.BackColor = Color.White
        QualiteJpeg.Location = New Point(7, 120)
        QualiteJpeg.BorderStyle = BorderStyle.None
        QualiteJpeg.Maximum = 100D
        QualiteJpeg.Minimum = 50D
        QualiteJpeg.Increment = 1D
        QualiteJpeg.Name = "QualiteJpeg"
        QualiteJpeg.Size = New Size(88, 23)
        QualiteJpeg.TabIndex = 111
        QualiteJpeg.TextAlign = HorizontalAlignment.Center
        QualiteJpeg.Value = 75D
        ToolTip1.SetToolTip(QualiteJpeg, "Qualité désirée pour l'enregistrement des cartes au format Jpeg" & CrLf &
                                         "100 : Très bonne qualité mais taille du fichier importante" & CrLf &
                                         "50 : Mauvaise qualité mais petite taille de fichier")
        '
        'LabelSupportCarte
        '
        LabelSupportCarte.BackColor = Color.Transparent
        LabelSupportCarte.BorderStyle = BorderStyle.FixedSingle
        LabelSupportCarte.Location = New Point(95, 99)
        LabelSupportCarte.Margin = New Padding(0)
        LabelSupportCarte.Name = "LabelSupportCarte"
        LabelSupportCarte.Size = New Size(253, 21)
        LabelSupportCarte.TabIndex = 77
        LabelSupportCarte.Text = "Support de la Carte"
        LabelSupportCarte.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelChoixSupportCarte
        '
        LabelChoixSupportCarte.BackColor = Color.White
        LabelChoixSupportCarte.BorderStyle = BorderStyle.FixedSingle
        LabelChoixSupportCarte.Location = New Point(95, 119)
        LabelChoixSupportCarte.Margin = New Padding(0)
        LabelChoixSupportCarte.Name = "LabelChoixSupportCarte"
        LabelChoixSupportCarte.Size = New Size(137, 24)
        LabelChoixSupportCarte.TabIndex = 117
        LabelChoixSupportCarte.Text = "Support de la Carte"
        LabelChoixSupportCarte.TextAlign = ContentAlignment.MiddleCenter
        LabelChoixSupportCarte.Tag = ListeChoixSupportCarte
        ToolTip1.SetToolTip(LabelChoixSupportCarte, resources.GetString("SupportCarte.ToolTip"))
        '
        'ListeChoixSupportCarte
        '
        ListeChoixSupportCarte.BackColor = Color.White
        ListeChoixSupportCarte.FormattingEnabled = True
        ListeChoixSupportCarte.Items.AddRange(New Object() {"Mémoire", "Fichier"})
        ListeChoixSupportCarte.Location = New Point(95, 115)
        ListeChoixSupportCarte.Name = "ListeChoixSupportCarte"
        ListeChoixSupportCarte.Size = New Size(137, 0)
        ListeChoixSupportCarte.Visible = False
        ListeChoixSupportCarte.TabIndex = 73
        ListeChoixSupportCarte.Visible = False
        ListeChoixSupportCarte.Tag = LabelChoixSupportCarte
        '
        'IsToutFormat
        '
        IsToutFormat.BackColor = Color.Transparent
        IsToutFormat.CheckAlign = ContentAlignment.MiddleRight
        IsToutFormat.Location = New Point(234, 122)
        IsToutFormat.Name = "IsToutFormat"
        IsToutFormat.Size = New Size(105, 20)
        IsToutFormat.TabIndex = 74
        IsToutFormat.Text = "Tout Format"
        IsToutFormat.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(IsToutFormat, resources.GetString("IsToutFormat.ToolTip"))
        '
        'LabelToutFormat
        '
        LabelIsToutFormat.BackColor = Color.Transparent
        LabelIsToutFormat.BorderStyle = BorderStyle.FixedSingle
        LabelIsToutFormat.Location = New Point(231, 119)
        LabelIsToutFormat.Name = "LabelToutFormat"
        LabelIsToutFormat.Size = New Size(117, 24)
        LabelIsToutFormat.TabIndex = 75
        '
        'LabelRepertoireChacheTuiles
        '
        LabelRepertoireChachesTuiles.BackColor = Color.Transparent
        LabelRepertoireChachesTuiles.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireChachesTuiles.Location = New Point(6, 149)
        LabelRepertoireChachesTuiles.Margin = New Padding(0)
        LabelRepertoireChachesTuiles.Name = "LabelRepertoireChacheTuiles"
        LabelRepertoireChachesTuiles.Size = New Size(342, 21)
        LabelRepertoireChachesTuiles.TabIndex = 122
        LabelRepertoireChachesTuiles.Text = "Répertoire des Caches Tuiles"
        LabelRepertoireChachesTuiles.TextAlign = ContentAlignment.MiddleCenter
        '
        'RepertoireCacheTuiles
        '
        RepertoireCachesTuiles.BackColor = Color.White
        RepertoireCachesTuiles.BorderStyle = BorderStyle.FixedSingle
        RepertoireCachesTuiles.Location = New Point(6, 169)
        RepertoireCachesTuiles.Name = "RepertoireCacheTuiles"
        RepertoireCachesTuiles.Size = New Size(342, 21)
        RepertoireCachesTuiles.TabIndex = 121
        RepertoireCachesTuiles.Tag = "Caches Tuiles"
        RepertoireCachesTuiles.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(RepertoireCachesTuiles, "Chemin d'enregistrement actuel des fichiers de trace (.trk)" & CrLf & MsgChemin)
        '
        'AvecAltitudes
        '
        AvecAltitudes.BackColor = Color.Transparent
        AvecAltitudes.CheckAlign = ContentAlignment.MiddleRight
        AvecAltitudes.Location = New Point(8, 198)
        AvecAltitudes.Margin = New Padding(0)
        AvecAltitudes.Name = "AvecAltitudes"
        AvecAltitudes.Size = New Size(331, 20)
        AvecAltitudes.TabIndex = 86
        AvecAltitudes.Text = "Altitude avec les coordonnées"
        AvecAltitudes.TextAlign = ContentAlignment.MiddleCenter
        AvecAltitudes.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(AvecAltitudes, "Indique si FCGP_Capturer doit afficher l'altitude avec les coordonnées" & CrLf &
                                           "lors du déplacement du pointeur de souris.")
        '
        'LabelAltitudes
        '
        LabelAvecAltitudes.BackColor = Color.Transparent
        LabelAvecAltitudes.BorderStyle = BorderStyle.FixedSingle
        LabelAvecAltitudes.Location = New Point(6, 196)
        LabelAvecAltitudes.Margin = New Padding(0)
        LabelAvecAltitudes.Name = "LabelAltitudes"
        LabelAvecAltitudes.Size = New Size(342, 23)
        LabelAvecAltitudes.TabIndex = 120
        LabelAvecAltitudes.TextAlign = ContentAlignment.MiddleCenter
        '
        'PanelAltitudes
        '
        PanelAltitudes.BackColor = Color.Transparent
        PanelAltitudes.Controls.Add(LabelIdentifiantsNASA)
        PanelAltitudes.Controls.Add(LabelID)
        PanelAltitudes.Controls.Add(ID)
        PanelAltitudes.Controls.Add(LabelMP)
        PanelAltitudes.Controls.Add(MP)
        PanelAltitudes.Controls.Add(VerifierID)
        PanelAltitudes.Controls.Add(DEM1)
        PanelAltitudes.Controls.Add(MSGAltitudes)
        PanelAltitudes.Controls.Add(LabelDonneesAltitudes)
        PanelAltitudes.Controls.Add(LabelRepertoireFichiersAltitudes)
        PanelAltitudes.Controls.Add(RepertoireFichiersAltitudes)
        PanelAltitudes.Location = New Point(6, 218)
        PanelAltitudes.Name = "PanelAltitudes"
        PanelAltitudes.Size = New Size(342, 111)
        PanelAltitudes.TabIndex = 119
        '
        'LabelIdentifiantsNASA
        '
        LabelIdentifiantsNASA.BackColor = Color.Transparent
        LabelIdentifiantsNASA.BorderStyle = BorderStyle.FixedSingle
        LabelIdentifiantsNASA.Location = New Point(0, 0)
        LabelIdentifiantsNASA.Margin = New Padding(0)
        LabelIdentifiantsNASA.Name = "LabelIdentifiantsNASA"
        LabelIdentifiantsNASA.Size = New Size(231, 21)
        LabelIdentifiantsNASA.TabIndex = 92
        LabelIdentifiantsNASA.Text = "Identifiants du site de la NASA"
        LabelIdentifiantsNASA.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelID
        '
        LabelID.BackColor = Color.Transparent
        LabelID.BorderStyle = BorderStyle.FixedSingle
        LabelID.Location = New Point(0, 20)
        LabelID.Name = "LabelID"
        LabelID.Size = New Size(33, 26)
        LabelID.TabIndex = 90
        LabelID.Text = "ID "
        LabelID.TextAlign = ContentAlignment.MiddleLeft
        '
        'ID
        '
        ID.BackColor = Color.White
        ID.Location = New Point(32, 20)
        ID.Name = "ID"
        ID.Size = New Size(155, 26)
        ID.TabIndex = 87
        ToolTip1.SetToolTip(ID, "Indiquez le nom d'utilisateur de votre compte sur le site de données de la NASA" & CrLf & MsgUri)
        '
        'LabelMP
        '
        LabelMP.BackColor = Color.Transparent
        LabelMP.BorderStyle = BorderStyle.FixedSingle
        LabelMP.Location = New Point(0, 45)
        LabelMP.Name = "LabelMP"
        LabelMP.Size = New Size(33, 26)
        LabelMP.TabIndex = 91
        LabelMP.Text = "MP "
        LabelMP.TextAlign = ContentAlignment.MiddleLeft
        '
        'MP
        '
        MP.BackColor = Color.White
        MP.Location = New Point(32, 45)
        MP.Name = "MP"
        MP.PasswordChar = "*"c 'Convert.ToChar(42)
        MP.Size = New Size(155, 19)
        MP.TabIndex = 88
        ToolTip1.SetToolTip(MP, "Indiquez le mot de passe de votre compte sur le site de données de la NASA" & CrLf & MsgUri)
        '
        'VerifierID
        '
        VerifierID.BorderStyle = BorderStyle.FixedSingle
        VerifierID.Font = New Font("Microsoft Sans Serif", 9.75!, FontStyle.Bold, GraphicsUnit.Point, 0)
        VerifierID.Location = New Point(186, 20)
        VerifierID.Name = "VerifierID"
        VerifierID.Size = New Size(45, 51)
        VerifierID.TabIndex = 113
        VerifierID.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(VerifierID, "Vérifie que les identifiants indiqués sont reconnus par" & CrLf &
                                        "le serveur de données de la NASA" & CrLf & MsgUri)

        '
        'DEM1
        '
        DEM1.BackColor = Color.Transparent
        DEM1.Location = New Point(233, 12)
        DEM1.Margin = New Padding(0)
        DEM1.Name = "DEM1"
        DEM1.RightToLeft = RightToLeft.Yes
        DEM1.Size = New Size(100, 23)
        DEM1.TabIndex = 96
        DEM1.Text = "Type Arc 1"
        DEM1.TextAlign = ContentAlignment.MiddleRight
        DEM1.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(DEM1, resources.GetString("DEM1.ToolTip"))
        '
        'MSGAltitudes
        '
        MSGAltitudes.BackColor = Color.Transparent
        MSGAltitudes.Location = New Point(233, 39)
        MSGAltitudes.Margin = New Padding(0)
        MSGAltitudes.Name = "MSGAltitudes"
        MSGAltitudes.RightToLeft = RightToLeft.Yes
        MSGAltitudes.Size = New Size(100, 23)
        MSGAltitudes.TabIndex = 116
        MSGAltitudes.Text = "Messages" & CrLf
        MSGAltitudes.TextAlign = ContentAlignment.MiddleRight
        MSGAltitudes.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(MSGAltitudes, "Indique si les messages liés au téléchargement" & CrLf &
                                          "des fichiers d'altitudes sont affichés.")
        '
        'LabelDonneesAltitudes
        '
        LabelDonneesAltitudes.BackColor = Color.Transparent
        LabelDonneesAltitudes.BorderStyle = BorderStyle.FixedSingle
        LabelDonneesAltitudes.Location = New Point(230, 0)
        LabelDonneesAltitudes.Name = "LabelDonneesAltitudes"
        LabelDonneesAltitudes.Size = New Size(112, 71)
        LabelDonneesAltitudes.TabIndex = 115
        '
        'LabelRepertoirefichiersAltitude
        '
        LabelRepertoireFichiersAltitudes.BackColor = Color.Transparent
        LabelRepertoireFichiersAltitudes.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireFichiersAltitudes.Location = New Point(0, 70)
        LabelRepertoireFichiersAltitudes.Margin = New Padding(0)
        LabelRepertoireFichiersAltitudes.Name = "LabelRepertoirefichiersAltitude"
        LabelRepertoireFichiersAltitudes.Size = New Size(342, 21)
        LabelRepertoireFichiersAltitudes.TabIndex = 95
        LabelRepertoireFichiersAltitudes.Text = "Répertoire d'enregistrement des fichiers d'altitudes" '& CrLf 
        LabelRepertoireFichiersAltitudes.TextAlign = ContentAlignment.MiddleCenter
        '
        'RepertoireAltitudes
        '
        RepertoireFichiersAltitudes.BackColor = Color.White
        RepertoireFichiersAltitudes.BorderStyle = BorderStyle.FixedSingle
        RepertoireFichiersAltitudes.Location = New Point(0, 90)
        RepertoireFichiersAltitudes.Name = "RepertoireAltitudes"
        RepertoireFichiersAltitudes.Size = New Size(342, 21)
        RepertoireFichiersAltitudes.TabIndex = 94
        RepertoireFichiersAltitudes.Tag = "fichiers d'altitudes"
        RepertoireFichiersAltitudes.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(RepertoireFichiersAltitudes, "Chemin d'enregistrement actuel des fichiers d'altitude (.hgt)" & CrLf & MsgChemin)
        '
        'LabelCouleurTraces
        '
        LabelCouleurTraces.BackColor = Color.Transparent
        LabelCouleurTraces.BorderStyle = BorderStyle.FixedSingle
        LabelCouleurTraces.Location = New Point(6, 335)
        LabelCouleurTraces.Margin = New Padding(0)
        LabelCouleurTraces.Name = "LabelCouleurTraces"
        LabelCouleurTraces.Size = New Size(342, 21)
        LabelCouleurTraces.TabIndex = 102
        LabelCouleurTraces.Text = "Couleur par défaut des Traces à l'écran"
        LabelCouleurTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'CouleurTraces
        '
        CouleurTraces.BackColor = Color.White
        CouleurTraces.BorderStyle = BorderStyle.FixedSingle
        CouleurTraces.Location = New Point(6, 355)
        CouleurTraces.Name = "CouleurTraces"
        CouleurTraces.Size = New Size(342, 21)
        CouleurTraces.TabIndex = 103
        CouleurTraces.Tag = False
        ToolTip1.SetToolTip(CouleurTraces, "Couleur par défaut des traces sur l'écran." & CrLf & MsgCouleur)
        '
        'LabelAspectTraces
        '
        LabelAspectTraces.BackColor = Color.Transparent
        LabelAspectTraces.BorderStyle = BorderStyle.FixedSingle
        LabelAspectTraces.Location = New Point(6, 375)
        LabelAspectTraces.Margin = New Padding(0)
        LabelAspectTraces.Name = "LabelAspectTraces"
        LabelAspectTraces.Size = New Size(342, 21)
        LabelAspectTraces.TabIndex = 112
        LabelAspectTraces.Text = "Aspect des Traces lors du dessin sur la carte"
        LabelAspectTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelCouleurDessinTraces
        '
        LabelCouleurDessinTraces.BackColor = Color.Transparent
        LabelCouleurDessinTraces.BorderStyle = BorderStyle.FixedSingle
        LabelCouleurDessinTraces.Location = New Point(6, 395)
        LabelCouleurDessinTraces.Name = "LabelCouleurDessinTraces"
        LabelCouleurDessinTraces.Size = New Size(171, 40)
        LabelCouleurDessinTraces.TabIndex = 108
        LabelCouleurDessinTraces.Text = "Couleur"
        LabelCouleurDessinTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'CouleurDessinTraces
        '
        CouleurDessinTraces.BackColor = Color.White
        CouleurDessinTraces.BorderStyle = BorderStyle.FixedSingle
        CouleurDessinTraces.Location = New Point(6, 434)
        CouleurDessinTraces.Name = "CouleurDessinTraces"
        CouleurDessinTraces.Size = New Size(171, 26)
        CouleurDessinTraces.TabIndex = 109
        CouleurDessinTraces.Tag = True
        ToolTip1.SetToolTip(CouleurDessinTraces, "Couleur des traces lors du dessin sur la carte." & CrLf & MsgCouleur)
        '
        'LabelCoefAlphaTraces
        '
        LabelCoefAlphaTraces.BackColor = Color.Transparent
        LabelCoefAlphaTraces.BorderStyle = BorderStyle.FixedSingle
        LabelCoefAlphaTraces.Location = New Point(176, 395)
        LabelCoefAlphaTraces.Name = "LabelCoefAlphaTraces"
        LabelCoefAlphaTraces.Size = New Size(95, 40)
        LabelCoefAlphaTraces.TabIndex = 107
        LabelCoefAlphaTraces.Text = "Transparence" & CrLf & "%"
        LabelCoefAlphaTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'CoefAlphaTraces
        '
        CoefAlphaTraces.AutoSize = True
        CoefAlphaTraces.BackColor = Color.White
        CoefAlphaTraces.Location = New Point(176, 434)
        CoefAlphaTraces.Name = "CoefAlphaTraces"
        CoefAlphaTraces.Size = New Size(95, 26)
        CoefAlphaTraces.TabIndex = 110
        CoefAlphaTraces.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CoefAlphaTraces, "Transparence du dessin de la trace." & CrLf & "0% --> Opaque" & CrLf & "100% --> Invisible")
        '
        'LabelEpaisseurTraces
        '
        LabelEpaisseurTraces.BackColor = Color.Transparent
        LabelEpaisseurTraces.BorderStyle = BorderStyle.FixedSingle
        LabelEpaisseurTraces.Location = New Point(270, 395)
        LabelEpaisseurTraces.Name = "LabelEpaisseurTraces"
        LabelEpaisseurTraces.Size = New Size(78, 40)
        LabelEpaisseurTraces.TabIndex = 106
        LabelEpaisseurTraces.Text = "Epaisseur" & CrLf & "en pixels"
        LabelEpaisseurTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'EpaisseurTraces
        '
        EpaisseurTraces.AutoSize = True
        EpaisseurTraces.BackColor = Color.White
        EpaisseurTraces.Location = New Point(270, 434)
        EpaisseurTraces.Maximum = 20D
        EpaisseurTraces.Minimum = 2D
        EpaisseurTraces.Name = "EpaisseurTraces"
        EpaisseurTraces.Size = New Size(78, 26)
        EpaisseurTraces.TabIndex = 111
        EpaisseurTraces.TextAlign = HorizontalAlignment.Center
        EpaisseurTraces.Value = 2D
        ToolTip1.SetToolTip(EpaisseurTraces, "Epaisseur de la trace en pixels." & CrLf & "Le nb de pixels dépend de l'échelle de la carte" & CrLf &
                                             "pour que la trace soit correctement visible.")
        '
        'LabelRepertoireTraces
        '
        LabelRepertoireTraces.BackColor = Color.Transparent
        LabelRepertoireTraces.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireTraces.Location = New Point(6, 459)
        LabelRepertoireTraces.Margin = New Padding(0)
        LabelRepertoireTraces.Name = "LabelRepertoireTraces"
        LabelRepertoireTraces.Size = New Size(342, 21)
        LabelRepertoireTraces.TabIndex = 100
        LabelRepertoireTraces.Text = "Répertoire d'enregistrement des Traces"
        LabelRepertoireTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'RepertoireTraces
        '
        RepertoireTraces.BackColor = Color.White
        RepertoireTraces.BorderStyle = BorderStyle.FixedSingle
        RepertoireTraces.Location = New Point(6, 479)
        RepertoireTraces.Name = "RepertoireTraces"
        RepertoireTraces.Size = New Size(342, 22)
        RepertoireTraces.TabIndex = 99
        RepertoireTraces.Tag = "fichiers de traces"
        RepertoireTraces.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(RepertoireTraces, "Chemin d'enregistrement actuel des fichiers de trace (.trk)" & CrLf & MsgChemin)
        '
        'PanelSupport
        '
        PanelSupport.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        PanelSupport.BackColor = Color.FromArgb(247, 247, 247)
        PanelSupport.Controls.Add(LabelCouleurAffichage)
        PanelSupport.Controls.Add(CouleurAffichage)
        PanelSupport.Controls.Add(LabelCouleurCartes)
        PanelSupport.Controls.Add(CouleurCartes)
        PanelSupport.Controls.Add(LabelEcranTravail)
        PanelSupport.Controls.Add(LabelChoixEcranTravail)
        PanelSupport.Controls.Add(ListeChoixEcranTravail)
        PanelSupport.Controls.Add(LabelJpeg)
        PanelSupport.Controls.Add(QualiteJpeg)
        PanelSupport.Controls.Add(LabelQualiteJpeg)
        PanelSupport.Controls.Add(LabelSupportCarte)
        PanelSupport.Controls.Add(LabelChoixSupportCarte)
        PanelSupport.Controls.Add(ListeChoixSupportCarte)
        PanelSupport.Controls.Add(IsToutFormat)
        PanelSupport.Controls.Add(LabelIsToutFormat)
        PanelSupport.Controls.Add(LabelRepertoireChachesTuiles)
        PanelSupport.Controls.Add(RepertoireCachesTuiles)
        PanelSupport.Controls.Add(AvecAltitudes)
        PanelSupport.Controls.Add(LabelAvecAltitudes)
        PanelSupport.Controls.Add(PanelAltitudes)
        PanelSupport.Controls.Add(LabelCouleurTraces)
        PanelSupport.Controls.Add(CouleurTraces)
        PanelSupport.Controls.Add(LabelAspectTraces)
        PanelSupport.Controls.Add(LabelCouleurDessinTraces)
        PanelSupport.Controls.Add(CouleurDessinTraces)
        PanelSupport.Controls.Add(LabelCoefAlphaTraces)
        PanelSupport.Controls.Add(LabelEpaisseurTraces)
        PanelSupport.Controls.Add(LabelRepertoireTraces)
        PanelSupport.Controls.Add(CoefAlphaTraces)
        PanelSupport.Controls.Add(EpaisseurTraces)
        PanelSupport.Controls.Add(RepertoireTraces)
        PanelSupport.Location = New Point(0, 0)
        PanelSupport.Name = "PanelSupport"
        PanelSupport.Size = New Size(354, 507)
        PanelSupport.TabIndex = 72
        '
        'BoutonEnter
        '
        BoutonEnter.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BoutonEnter.DialogResult = DialogResult.OK
        BoutonEnter.Location = New Point(273, 513)
        BoutonEnter.Name = "BoutonEnter"
        BoutonEnter.Size = New Size(75, 30)
        BoutonEnter.TabIndex = 1
        BoutonEnter.Text = "Valider"
        BoutonEnter.UseVisualStyleBackColor = True
        ToolTip1.SetToolTip(BoutonEnter, "Ferme le formulaire en prenant en compte les modifications.")
        '
        'BoutonEscape
        '
        BoutonEscape.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BoutonEscape.DialogResult = DialogResult.Cancel
        BoutonEscape.Location = New Point(192, 513)
        BoutonEscape.Name = "BoutonEscape"
        BoutonEscape.Size = New Size(75, 30)
        BoutonEscape.TabIndex = 2
        BoutonEscape.Text = "Annuler"
        BoutonEscape.UseVisualStyleBackColor = True
        ToolTip1.SetToolTip(BoutonEscape, "Ferme le formulaire sans prendre en compte les modifications.")
        '
        'ToolTip1
        '
        ToolTip1.AutoPopDelay = 5000
        ToolTip1.InitialDelay = 100
        ToolTip1.ReshowDelay = 100
        '
        'Configuration
        '
        AcceptButton = BoutonEnter
        AutoScaleMode = AutoScaleMode.None
        CancelButton = BoutonEscape
        ClientSize = New Size(354, 549)
        ControlBox = False
        Controls.Add(PanelSupport)
        Controls.Add(BoutonEscape)
        Controls.Add(BoutonEnter)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        MaximizeBox = False
        MinimizeBox = False
        Name = "Configuration"
        ShowIcon = False
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.CenterParent
        CType(QualiteJpeg, ISupportInitialize).EndInit()
        CType(EpaisseurTraces, ISupportInitialize).EndInit()
        CType(CoefAlphaTraces, ISupportInitialize).EndInit()
        PanelSupport.ResumeLayout(False)
        PanelSupport.PerformLayout()
        PanelAltitudes.ResumeLayout(False)
        PanelAltitudes.PerformLayout()
        ResumeLayout(False)
    End Sub

    Private PanelSupport As Panel
    Private LabelCouleurAffichage As Label
    Private CouleurAffichage As Label
    Private LabelCouleurCartes As Label
    Private CouleurCartes As Label
    Private LabelEcranTravail As Label
    Private LabelChoixEcranTravail As Label
    Private ListeChoixEcranTravail As ComboBox
    Private LabelJpeg As Label
    Private LabelQualiteJpeg As Label
    Private QualiteJpeg As NumericUpDown
    Private LabelSupportCarte As Label
    Private LabelChoixSupportCarte As Label
    Private ListeChoixSupportCarte As ComboBox
    Private IsToutFormat As CheckBox
    Private LabelIsToutFormat As Label
    Private LabelRepertoireChachesTuiles As Label
    Private RepertoireCachesTuiles As Label
    Private AvecAltitudes As CheckBox
    Private LabelAvecAltitudes As Label
    Private PanelAltitudes As Panel
    Private LabelIdentifiantsNASA As Label
    Private LabelID As Label
    Private ID As TextBox
    Private LabelMP As Label
    Private MP As MaskedTextBox
    Private VerifierID As Label
    Private DEM1 As CheckBox
    Private MSGAltitudes As CheckBox
    Private LabelDonneesAltitudes As Label
    Private LabelRepertoireFichiersAltitudes As Label
    Private RepertoireFichiersAltitudes As Label
    Private LabelCouleurTraces As Label
    Private CouleurTraces As Label
    Private LabelAspectTraces As Label
    Private LabelCouleurDessinTraces As Label
    Private CouleurDessinTraces As Label
    Private LabelCoefAlphaTraces As Label
    Private CoefAlphaTraces As NumericUpDown
    Private LabelEpaisseurTraces As Label
    Private EpaisseurTraces As NumericUpDown
    Private LabelRepertoireTraces As Label
    Private RepertoireTraces As Label
    Private BoutonEnter As Button
    Private BoutonEscape As Button
    Private ToolTip1 As ToolTip
End Class