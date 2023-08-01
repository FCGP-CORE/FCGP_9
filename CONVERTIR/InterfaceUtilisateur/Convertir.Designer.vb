Partial Class Convertir
    Inherits Form
    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler Couleur.Click, AddressOf Couleur_Click
        AddHandler Transparence.ValueChanged, AddressOf Transparence_ValueChanged
        AddHandler IsTrace.CheckedChanged, AddressOf IsTrace_CheckedChanged
        AddHandler NbPaths.ValueChanged, AddressOf NbPaths_ValueChanged
        AddHandler DimTuilesLarg.ValueChanged, AddressOf DimTuilesLarg_ValueChanged
        AddHandler Traitement.Click, AddressOf Traitement_Click
        AddHandler LabelRepertoireCartes.Click, AddressOf LabelRepertoireCartes_Click
        AddHandler LabelRepertoireTuiles.Click, AddressOf LabelRepertoireFichiersTuiles_Click
        AddHandler Quitter.Click, AddressOf Quitter_Click
        'GeoRefs
        AddHandler LabelGeoRefs.Click, AddressOf LabelListesChoix_Click
        AddHandler GeoRefs.DropDownClosed, AddressOf ListesChoix_DropDownClosed
        AddHandler GeoRefs.SelectedIndexChanged, AddressOf GeoRefs_SelectedIndexChanged
        'FormatCartes
        AddHandler LabelFormatCartes.Click, AddressOf LabelListesChoix_Click
        AddHandler FormatCartes.DropDownClosed, AddressOf ListesChoix_DropDownClosed
        AddHandler FormatCartes.SelectedIndexChanged, AddressOf FormatCartes_SelectedIndexChanged
        'References
        AddHandler LabelReferences.Click, AddressOf LabelListesChoix_Click
        AddHandler References.DropDownClosed, AddressOf ListesChoix_DropDownClosed
        AddHandler References.SelectedIndexChanged, AddressOf References_SelectedIndexChanged
        'Traces
        AddHandler LabelTraces.Click, AddressOf LabelTraces_Click
        AddHandler Traces.DropDownClosed, AddressOf ListesChoix_DropDownClosed
        AddHandler Traces.SelectedIndexChanged, AddressOf Traces_SelectedIndexChanged
        'FichiersTuiles
        AddHandler LabelFichiersTuiles.Click, AddressOf LabelListesChoix_Click
        AddHandler FichiersTuiles.SelectedIndexChanged, AddressOf FichiersTuiles_SelectedIndexChanged
        'formulaire
        AddHandler Me.Load, AddressOf Convertir_Load
        AddHandler Me.FormClosing, AddressOf Convertir_FormClosing
        AddHandler Me.FormClosed, AddressOf Convertir_FormClosed
        AddHandler Me.LocationChanged, AddressOf Convertir_LocationChanged
        AddHandler Me.KeyDown, AddressOf Convertir_KeyDown
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
        Dim resources As New ComponentResourceManager(GetType(Convertir))
        GroupeConfigurations = New Panel()
        EtiquetteGroupeConfigurationCarte = New Label()
        LabelGeoRefs = New Label()
        EtiquetteGeoRefs = New Label()
        GeoRefs = New ComboBox()
        IsGrille = New CheckBox()
        EtiquetteIsGrille = New Label()
        GroupeCartes = New Panel()
        LabelFormatCartes = New Label()
        EtiquetteFormatCarte = New Label()
        FormatCartes = New ComboBox()
        LabelReferences = New Label()
        EtiquetteReferences = New Label()
        References = New ComboBox()
        EtiquetteRepertoireCartes = New Label()
        LabelRepertoireCartes = New Label()
        EtiquetteGroupeConfigurationTraces = New Label()
        LabelTraces = New Label()
        EtiquetteTraces = New Label()
        Traces = New ComboBox()
        EtiquetteBoutonTrace = New Label()
        GroupeConfigurationTraces = New Panel()
        EtiquetteEpaisseur = New Label()
        Epaisseur = New NumericUpDown()
        EtiquetteCouleur = New Label()
        Couleur = New Label()
        EtiquetteTransparence = New Label()
        Transparence = New NumericUpDown()
        IsTrace = New CheckBox()
        EtiquetteIsTrace = New Label()
        EtiquetteNbPaths = New Label()
        NbPaths = New NumericUpDown()
        LabelFichiersTuiles = New Label()
        EtiquetteFichiersTuiles = New Label()
        FichiersTuiles = New ComboBox()
        EtiquetteGroupeConfigurationFichiersTuiles = New Label()
        GroupeFichiersTuiles = New Panel()
        EtiquetteRepertoireTuiles = New Label()
        LabelRepertoireTuiles = New Label()
        EtiquetteDimTuilesLarg = New Label()
        DimTuilesLarg = New NumericUpDown()
        Information = New Label()
        Quitter = New Button()
        Traitement = New Button()
        OuvrirFichiersCarte = New OpenFileDialog()
        Informations = New ToolTip(components)
        ChoixCouleurTrace = New ColorDialog()
        OuvrirFichiersTrace = New OpenFileDialog()
        GroupeConfigurations.SuspendLayout()
        GroupeCartes.SuspendLayout()
        GroupeConfigurationTraces.SuspendLayout()
        CType(Epaisseur, ISupportInitialize).BeginInit()
        CType(Transparence, ISupportInitialize).BeginInit()
        CType(NbPaths, ISupportInitialize).BeginInit()
        GroupeFichiersTuiles.SuspendLayout()
        CType(DimTuilesLarg, ISupportInitialize).BeginInit()
        SuspendLayout()
#Region "groupe configuration des cartes"
        '
        'EtiquetteGroupeConfigurationCarte
        '
        EtiquetteGroupeConfigurationCarte.BackColor = Color.Transparent
        EtiquetteGroupeConfigurationCarte.BorderStyle = BorderStyle.FixedSingle
        EtiquetteGroupeConfigurationCarte.Cursor = Cursors.Default
        EtiquetteGroupeConfigurationCarte.FlatStyle = FlatStyle.Flat
        EtiquetteGroupeConfigurationCarte.Location = New Point(6, 6)
        EtiquetteGroupeConfigurationCarte.Margin = New Padding(0)
        EtiquetteGroupeConfigurationCarte.Name = "EtiquetteGroupeConfigurationCarte"
        EtiquetteGroupeConfigurationCarte.Size = New Size(424, 27)
        EtiquetteGroupeConfigurationCarte.TabIndex = 66
        EtiquetteGroupeConfigurationCarte.Text = "Configuration des Cartes"
        EtiquetteGroupeConfigurationCarte.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelGeoRefs
        '
        LabelGeoRefs.BackColor = Color.White
        LabelGeoRefs.BorderStyle = BorderStyle.FixedSingle
        LabelGeoRefs.Cursor = Cursors.Default
        LabelGeoRefs.FlatStyle = FlatStyle.Flat
        LabelGeoRefs.Location = New Point(6, 71)
        LabelGeoRefs.Margin = New Padding(0)
        LabelGeoRefs.Name = "LabelGeoRefs"
        LabelGeoRefs.Size = New Size(145, 23)
        LabelGeoRefs.TabIndex = 51
        LabelGeoRefs.Tag = GeoRefs
        LabelGeoRefs.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(LabelGeoRefs, "Choix des géoréférencement des cartes. Voir l'aide")
        '
        'EtiquetteGeoRefs
        '
        EtiquetteGeoRefs.BackColor = Color.Transparent
        EtiquetteGeoRefs.BorderStyle = BorderStyle.FixedSingle
        EtiquetteGeoRefs.Cursor = Cursors.Default
        EtiquetteGeoRefs.FlatStyle = FlatStyle.Flat
        EtiquetteGeoRefs.Location = New Point(6, 32)
        EtiquetteGeoRefs.Margin = New Padding(0)
        EtiquetteGeoRefs.Name = "EtiquetteGeoRefs"
        EtiquetteGeoRefs.Size = New Size(145, 40)
        EtiquetteGeoRefs.TabIndex = 0
        EtiquetteGeoRefs.Text = "Géo-référencement"
        EtiquetteGeoRefs.TextAlign = ContentAlignment.MiddleCenter
        '
        'GeoRefs
        '
        GeoRefs.BackColor = Color.White
        GeoRefs.Cursor = Cursors.Default
        GeoRefs.DropDownStyle = ComboBoxStyle.DropDownList
        GeoRefs.FlatStyle = FlatStyle.System
        GeoRefs.FormattingEnabled = True
        GeoRefs.Location = New Point(6, 66)
        GeoRefs.Name = "GeoRefs"
        GeoRefs.Size = New Size(145, 27)
        GeoRefs.TabIndex = 2
        GeoRefs.Visible = False
        '
        'GroupeCartes
        '
        GroupeCartes.BackColor = Color.Transparent
        GroupeCartes.Controls.Add(LabelFormatCartes)
        GroupeCartes.Controls.Add(EtiquetteFormatCarte)
        GroupeCartes.Controls.Add(FormatCartes)
        GroupeCartes.Controls.Add(LabelReferences)
        GroupeCartes.Controls.Add(EtiquetteReferences)
        GroupeCartes.Controls.Add(References)
        GroupeCartes.Controls.Add(IsGrille)
        GroupeCartes.Controls.Add(EtiquetteIsGrille)
        GroupeCartes.Controls.Add(EtiquetteRepertoireCartes)
        GroupeCartes.Controls.Add(LabelRepertoireCartes)
        GroupeCartes.Location = New Point(6, 32)
        GroupeCartes.Margin = New Padding(0)
        GroupeCartes.Name = "GroupeCartes"
        GroupeCartes.Size = New Size(424, 104)
        GroupeCartes.TabIndex = 65
        '
        'LabelFormatCartes
        '
        LabelFormatCartes.BackColor = Color.White
        LabelFormatCartes.BorderStyle = BorderStyle.FixedSingle
        LabelFormatCartes.Cursor = Cursors.Default
        LabelFormatCartes.FlatStyle = FlatStyle.Flat
        LabelFormatCartes.Location = New Point(144, 39)
        LabelFormatCartes.Margin = New Padding(0)
        LabelFormatCartes.Name = "LabelFormatCartes"
        LabelFormatCartes.Size = New Size(107, 23)
        LabelFormatCartes.TabIndex = 53
        LabelFormatCartes.Tag = FormatCartes
        LabelFormatCartes.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(LabelFormatCartes, "Choix du format d'enregistrement des cartes.")
        '
        'EtiquetteFormatCarte
        '
        EtiquetteFormatCarte.BackColor = Color.Transparent
        EtiquetteFormatCarte.BorderStyle = BorderStyle.FixedSingle
        EtiquetteFormatCarte.Cursor = Cursors.Default
        EtiquetteFormatCarte.FlatStyle = FlatStyle.Flat
        EtiquetteFormatCarte.Location = New Point(144, 0)
        EtiquetteFormatCarte.Margin = New Padding(0)
        EtiquetteFormatCarte.Name = "EtiquetteFormatCarte"
        EtiquetteFormatCarte.Size = New Size(107, 40)
        EtiquetteFormatCarte.TabIndex = 0
        EtiquetteFormatCarte.Text = "Format" & CrLf & "Enregistrement"
        EtiquetteFormatCarte.TextAlign = ContentAlignment.MiddleCenter
        '
        'FormatCartes
        '
        FormatCartes.BackColor = Color.White
        FormatCartes.Cursor = Cursors.Default
        FormatCartes.DropDownStyle = ComboBoxStyle.DropDownList
        FormatCartes.FlatStyle = FlatStyle.System
        FormatCartes.FormattingEnabled = True
        FormatCartes.Items.AddRange(New Object() {"Bmp", "Png", "Jpeg"})
        FormatCartes.Location = New Point(144, 34)
        FormatCartes.Name = "FormatCartes"
        FormatCartes.Size = New Size(107, 24)
        FormatCartes.TabIndex = 3
        FormatCartes.Visible = False
        '
        'LabelReferences
        '
        LabelReferences.BackColor = Color.White
        LabelReferences.BorderStyle = BorderStyle.FixedSingle
        LabelReferences.Cursor = Cursors.Default
        LabelReferences.FlatStyle = FlatStyle.Flat
        LabelReferences.Location = New Point(250, 39)
        LabelReferences.Margin = New Padding(0)
        LabelReferences.Name = "LabelReferences"
        LabelReferences.Size = New Size(95, 23)
        LabelReferences.TabIndex = 52
        LabelReferences.Tag = References
        LabelReferences.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(LabelReferences, "Choix des références des grilles issues des cartes.")
        '
        'EtiquetteReferences
        '
        EtiquetteReferences.BackColor = Color.Transparent
        EtiquetteReferences.BorderStyle = BorderStyle.FixedSingle
        EtiquetteReferences.Cursor = Cursors.Default
        EtiquetteReferences.FlatStyle = FlatStyle.Flat
        EtiquetteReferences.Location = New Point(250, 0)
        EtiquetteReferences.Margin = New Padding(0)
        EtiquetteReferences.Name = "EtiquetteReferences"
        EtiquetteReferences.Size = New Size(95, 40)
        EtiquetteReferences.TabIndex = 0
        EtiquetteReferences.Text = "Coordonnées" & CrLf & "Grille"
        EtiquetteReferences.TextAlign = ContentAlignment.MiddleCenter
        '
        'References
        '
        References.BackColor = Color.White
        References.Cursor = Cursors.Default
        References.DropDownStyle = ComboBoxStyle.DropDownList
        References.FlatStyle = FlatStyle.System
        References.FormattingEnabled = True
        References.Location = New Point(250, 34)
        References.Name = "References"
        References.Size = New Size(95, 24)
        References.TabIndex = 5
        References.Visible = False
        '
        'IsGrille
        '
        IsGrille.BackColor = Color.Transparent
        IsGrille.CheckAlign = ContentAlignment.BottomCenter
        IsGrille.Location = New Point(360, 4)
        IsGrille.Margin = New Padding(0)
        IsGrille.Name = "IsGrille"
        IsGrille.Size = New Size(51, 47)
        IsGrille.TabIndex = 51
        IsGrille.TabStop = False
        IsGrille.Text = "Grille"
        IsGrille.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(IsGrille, resources.GetString("IsGrille.ToolTip"))
        IsGrille.UseVisualStyleBackColor = False
        '
        'EtiquetteIsGrille
        '
        EtiquetteIsGrille.BackColor = Color.Transparent
        EtiquetteIsGrille.BorderStyle = BorderStyle.FixedSingle
        EtiquetteIsGrille.Cursor = Cursors.Default
        EtiquetteIsGrille.FlatStyle = FlatStyle.Flat
        EtiquetteIsGrille.Location = New Point(344, 0)
        EtiquetteIsGrille.Margin = New Padding(0)
        EtiquetteIsGrille.Name = "EtiquetteIsGrille"
        EtiquetteIsGrille.Size = New Size(80, 62)
        EtiquetteIsGrille.TabIndex = 54
        EtiquetteIsGrille.TextAlign = ContentAlignment.MiddleCenter
        '
        'EtiquetteRepertoireCartes
        '
        EtiquetteRepertoireCartes.BackColor = Color.Transparent
        EtiquetteRepertoireCartes.BorderStyle = BorderStyle.FixedSingle
        EtiquetteRepertoireCartes.Location = New Point(0, 61)
        EtiquetteRepertoireCartes.Margin = New Padding(0)
        EtiquetteRepertoireCartes.Name = "EtiquetteCheminCartes"
        EtiquetteRepertoireCartes.Size = New Size(424, 21)
        EtiquetteRepertoireCartes.TabIndex = 39
        EtiquetteRepertoireCartes.Text = "Répertoire d'enregistrement des Cartes"
        EtiquetteRepertoireCartes.TextAlign = ContentAlignment.MiddleLeft
        '
        'LabelRepertoireCartes
        '
        LabelRepertoireCartes.BackColor = Color.White
        LabelRepertoireCartes.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireCartes.Location = New Point(0, 81)
        LabelRepertoireCartes.Margin = New Padding(0)
        LabelRepertoireCartes.Name = "LabelCheminCartes"
        LabelRepertoireCartes.Size = New Size(424, 23)
        LabelRepertoireCartes.TabIndex = 0
        LabelRepertoireCartes.TextAlign = ContentAlignment.MiddleLeft
        Informations.SetToolTip(LabelRepertoireCartes, "Indique le répertoire où seront enregistrés la carte" & CrLf &
                                                       " et le fichier de géo-référencement associé.")
#End Region
#Region "groupe configuration des traces"
        '
        'EtiquetteGroupeConfigurationTraces
        '
        EtiquetteGroupeConfigurationTraces.BackColor = Color.Transparent
        EtiquetteGroupeConfigurationTraces.BorderStyle = BorderStyle.FixedSingle
        EtiquetteGroupeConfigurationTraces.Cursor = Cursors.Default
        EtiquetteGroupeConfigurationTraces.FlatStyle = FlatStyle.Flat
        EtiquetteGroupeConfigurationTraces.Location = New Point(6, 142)
        EtiquetteGroupeConfigurationTraces.Margin = New Padding(0)
        EtiquetteGroupeConfigurationTraces.Name = "EtiquetteGroupeConfigurationTraces"
        EtiquetteGroupeConfigurationTraces.Size = New Size(424, 27)
        EtiquetteGroupeConfigurationTraces.TabIndex = 1
        EtiquetteGroupeConfigurationTraces.Text = "Configuration des Traces"
        EtiquetteGroupeConfigurationTraces.TextAlign = ContentAlignment.MiddleCenter
        '
        'EtiquetteCouleur
        '
        EtiquetteCouleur.BackColor = Color.Transparent
        EtiquetteCouleur.BorderStyle = BorderStyle.FixedSingle
        EtiquetteCouleur.Cursor = Cursors.Default
        EtiquetteCouleur.FlatStyle = FlatStyle.Flat
        EtiquetteCouleur.Location = New Point(0, 0)
        EtiquetteCouleur.Margin = New Padding(0)
        EtiquetteCouleur.Name = "EtiquetteCouleur"
        EtiquetteCouleur.Size = New Size(80, 40)
        EtiquetteCouleur.TabIndex = 0
        EtiquetteCouleur.Text = "Couleur"
        EtiquetteCouleur.TextAlign = ContentAlignment.MiddleCenter
        '
        'Couleur
        '
        Couleur.BackColor = Color.White
        Couleur.BorderStyle = BorderStyle.FixedSingle
        Couleur.Location = New Point(0, 39)
        Couleur.Margin = New Padding(0)
        Couleur.Name = "Couleur"
        Couleur.Size = New Size(80, 26)
        Couleur.TabIndex = 1
        Informations.SetToolTip(Couleur, "Cliquez pour choisir " & CrLf & "une autre couleur.")
        '
        'EtiquetteTransparence
        '
        EtiquetteTransparence.BackColor = Color.Transparent
        EtiquetteTransparence.BorderStyle = BorderStyle.FixedSingle
        EtiquetteTransparence.Cursor = Cursors.Default
        EtiquetteTransparence.FlatStyle = FlatStyle.Flat
        EtiquetteTransparence.Location = New Point(79, 0)
        EtiquetteTransparence.Margin = New Padding(0)
        EtiquetteTransparence.Name = "EtiquetteTransparence"
        EtiquetteTransparence.Size = New Size(108, 40)
        EtiquetteTransparence.TabIndex = 0
        EtiquetteTransparence.Text = "Transparence" & CrLf & "%"
        EtiquetteTransparence.TextAlign = ContentAlignment.MiddleCenter
        '
        'Transparence
        '
        Transparence.AutoSize = True
        Transparence.BackColor = Color.White
        Transparence.BorderStyle = BorderStyle.FixedSingle
        Transparence.Location = New Point(79, 39)
        Transparence.Margin = New Padding(0)
        Transparence.Name = "Transparence"
        Transparence.Size = New Size(108, 26)
        Transparence.TabIndex = 2
        Transparence.TabStop = False
        Transparence.TextAlign = HorizontalAlignment.Center
        Informations.SetToolTip(Transparence, "Transparence du dessin de la trace." & CrLf &
                                              "0% --> Opaque" & CrLf & "100% --> Invisible")
        '
        'EtiquetteEpaisseur
        '
        EtiquetteEpaisseur.BackColor = Color.Transparent
        EtiquetteEpaisseur.BorderStyle = BorderStyle.FixedSingle
        EtiquetteEpaisseur.Cursor = Cursors.Default
        EtiquetteEpaisseur.FlatStyle = FlatStyle.Flat
        EtiquetteEpaisseur.Location = New Point(186, 0)
        EtiquetteEpaisseur.Margin = New Padding(0)
        EtiquetteEpaisseur.Name = "EtiquetteEpaisseur"
        EtiquetteEpaisseur.Size = New Size(77, 40)
        EtiquetteEpaisseur.TabIndex = 0
        EtiquetteEpaisseur.Text = "Epaisseur" & CrLf & "en pixels"
        EtiquetteEpaisseur.TextAlign = ContentAlignment.MiddleCenter
        '
        'Epaisseur
        '
        Epaisseur.AutoSize = True
        Epaisseur.BackColor = Color.White
        Epaisseur.BorderStyle = BorderStyle.FixedSingle
        Epaisseur.Location = New Point(186, 39)
        Epaisseur.Margin = New Padding(0)
        Epaisseur.Maximum = 20D
        Epaisseur.Minimum = 2D
        Epaisseur.Name = "Epaisseur"
        Epaisseur.Size = New Size(77, 26)
        Epaisseur.TabIndex = 43
        Epaisseur.TabStop = False
        Epaisseur.TextAlign = HorizontalAlignment.Center
        Informations.SetToolTip(Epaisseur, "Epaisseur de la trace en pixels." & CrLf &
                                           "Le nb de pixels doit dépendre de l'échelle de la carte" & CrLf &
                                           "afin que la trace soit visible correctement.")
        Epaisseur.Value = 2D
        '
        'GroupeConfigurationTraces
        '
        GroupeConfigurationTraces.BackColor = Color.Transparent
        GroupeConfigurationTraces.Controls.Add(EtiquetteCouleur)
        GroupeConfigurationTraces.Controls.Add(Couleur)
        GroupeConfigurationTraces.Controls.Add(EtiquetteTransparence)
        GroupeConfigurationTraces.Controls.Add(Transparence)
        GroupeConfigurationTraces.Controls.Add(EtiquetteEpaisseur)
        GroupeConfigurationTraces.Controls.Add(Epaisseur)
        GroupeConfigurationTraces.Enabled = False
        GroupeConfigurationTraces.Location = New Point(6, 168)
        GroupeConfigurationTraces.Margin = New Padding(0)
        GroupeConfigurationTraces.Name = "GroupeConfigurationTraces"
        GroupeConfigurationTraces.Size = New Size(263, 65)
        GroupeConfigurationTraces.TabIndex = 63
        '
        'EtiquetteNbPaths
        '
        EtiquetteNbPaths.BackColor = Color.Transparent
        EtiquetteNbPaths.BorderStyle = BorderStyle.FixedSingle
        EtiquetteNbPaths.Cursor = Cursors.Default
        EtiquetteNbPaths.FlatStyle = FlatStyle.Flat
        EtiquetteNbPaths.Location = New Point(268, 168)
        EtiquetteNbPaths.Margin = New Padding(0)
        EtiquetteNbPaths.Name = "EtiquetteNbPaths"
        EtiquetteNbPaths.Size = New Size(83, 40)
        EtiquetteNbPaths.TabIndex = 62
        EtiquetteNbPaths.Text = "Nb de Paths" & CrLf & "dans KMZ"
        EtiquetteNbPaths.TextAlign = ContentAlignment.MiddleCenter
        '
        'NbPaths
        '
        NbPaths.BackColor = Color.White
        NbPaths.BorderStyle = BorderStyle.FixedSingle
        NbPaths.Location = New Point(268, 207)
        NbPaths.Margin = New Padding(0)
        NbPaths.Maximum = 2D
        NbPaths.Name = "NbPaths"
        NbPaths.Size = New Size(83, 26)
        NbPaths.TabIndex = 64
        NbPaths.TabStop = False
        NbPaths.TextAlign = HorizontalAlignment.Center
        Informations.SetToolTip(NbPaths, resources.GetString("NbPaths.ToolTip"))
        '
        'IsTrace
        '
        IsTrace.BackColor = Color.Transparent
        IsTrace.CheckAlign = ContentAlignment.BottomCenter
        IsTrace.Location = New Point(364, 175)
        IsTrace.Margin = New Padding(0)
        IsTrace.Name = "IsTrace"
        IsTrace.Size = New Size(55, 47)
        IsTrace.TabIndex = 59
        IsTrace.TabStop = False
        IsTrace.Text = "Inclure"
        IsTrace.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(IsTrace, "Prise en compte d'une ou plusieurs traces" & CrLf &
                                         "dans les fichiers en sortie de traitement.")
        IsTrace.UseVisualStyleBackColor = False
        '
        'EtiquetteIsTrace
        '
        EtiquetteIsTrace.BackColor = Color.Transparent
        EtiquetteIsTrace.BorderStyle = BorderStyle.FixedSingle
        EtiquetteIsTrace.Location = New Point(350, 168)
        EtiquetteIsTrace.Margin = New Padding(0)
        EtiquetteIsTrace.Name = "EtiquetteIsTrace"
        EtiquetteIsTrace.Size = New Size(80, 65)
        EtiquetteIsTrace.TabIndex = 60

        '
        'LabelTraces
        '
        LabelTraces.BackColor = Color.White
        LabelTraces.BorderStyle = BorderStyle.FixedSingle
        LabelTraces.Location = New Point(6, 252)
        LabelTraces.Margin = New Padding(0)
        LabelTraces.Name = "LabelTraces"
        LabelTraces.Size = New Size(406, 24)
        LabelTraces.TabIndex = 15
        LabelTraces.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(LabelTraces, "Première des traces sélectionnées." & CrLf &
                                             "Cliquez dessus pour changer la sélection.")
        '
        'EtiquetteTraces
        '
        EtiquetteTraces.BackColor = Color.Transparent
        EtiquetteTraces.BorderStyle = BorderStyle.FixedSingle
        EtiquetteTraces.Location = New Point(6, 232)
        EtiquetteTraces.Margin = New Padding(0)
        EtiquetteTraces.Name = "EtiquetteTraces"
        EtiquetteTraces.Size = New Size(424, 21)
        EtiquetteTraces.TabIndex = 61
        EtiquetteTraces.Text = "Traces sélectionnées pour être dessinées sur la carte"
        EtiquetteTraces.TextAlign = ContentAlignment.MiddleLeft
        '
        'Traces
        '
        Traces.BackColor = Color.White
        Traces.DropDownStyle = ComboBoxStyle.DropDownList
        Traces.Enabled = False
        Traces.FlatStyle = FlatStyle.System
        Traces.FormattingEnabled = True
        Traces.Location = New Point(6, 248)
        Traces.Margin = New Padding(0)
        Traces.Name = "Traces"
        Traces.Size = New Size(423, 24)
        Traces.TabIndex = 58
        Traces.TabStop = False
        Informations.SetToolTip(Traces, "Liste des traces sélectionnées.")
        '
        'EtiquetteBoutonTrace
        '
        EtiquetteBoutonTrace.BackColor = Color.Transparent
        EtiquetteBoutonTrace.BorderStyle = BorderStyle.FixedSingle
        EtiquetteBoutonTrace.Location = New Point(411, 252)
        EtiquetteBoutonTrace.Name = "EtiquetteBoutonTrace"
        EtiquetteBoutonTrace.Size = New Size(19, 24)
        EtiquetteBoutonTrace.TabIndex = 0
        EtiquetteBoutonTrace.Text = "Label16"
#End Region
#Region "groupe configuration des fichiers tuiles"
        '
        'EtiquetteGroupeConfigurationFichiersTuiles
        '
        EtiquetteGroupeConfigurationFichiersTuiles.BackColor = Color.Transparent
        EtiquetteGroupeConfigurationFichiersTuiles.BorderStyle = BorderStyle.FixedSingle
        EtiquetteGroupeConfigurationFichiersTuiles.Cursor = Cursors.Default
        EtiquetteGroupeConfigurationFichiersTuiles.FlatStyle = FlatStyle.Flat
        EtiquetteGroupeConfigurationFichiersTuiles.Location = New Point(6, 282)
        EtiquetteGroupeConfigurationFichiersTuiles.Margin = New Padding(0)
        EtiquetteGroupeConfigurationFichiersTuiles.Name = "EtiquetteGroupeConfigurationFichiersTuiles"
        EtiquetteGroupeConfigurationFichiersTuiles.Size = New Size(424, 27)
        EtiquetteGroupeConfigurationFichiersTuiles.TabIndex = 67
        EtiquetteGroupeConfigurationFichiersTuiles.Text = "Configuration des Fichiers Tuiles"
        EtiquetteGroupeConfigurationFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelFichiersTuiles
        '
        LabelFichiersTuiles.BackColor = Color.White
        LabelFichiersTuiles.BorderStyle = BorderStyle.FixedSingle
        LabelFichiersTuiles.Cursor = Cursors.Default
        LabelFichiersTuiles.FlatStyle = FlatStyle.Flat
        LabelFichiersTuiles.Location = New Point(6, 328)
        LabelFichiersTuiles.Margin = New Padding(0)
        LabelFichiersTuiles.Name = "LabelFichiersTuiles"
        LabelFichiersTuiles.Size = New Size(212, 26)
        LabelFichiersTuiles.TabIndex = 55
        LabelFichiersTuiles.Tag = FichiersTuiles
        LabelFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter
        Informations.SetToolTip(LabelFichiersTuiles, "Choix des fichiers tuiles issus des cartes.")
        '
        'EtiquetteFichiersTuiles
        '
        EtiquetteFichiersTuiles.BackColor = Color.Transparent
        EtiquetteFichiersTuiles.BorderStyle = BorderStyle.FixedSingle
        EtiquetteFichiersTuiles.Cursor = Cursors.Default
        EtiquetteFichiersTuiles.FlatStyle = FlatStyle.Flat
        EtiquetteFichiersTuiles.Location = New Point(6, 308)
        EtiquetteFichiersTuiles.Margin = New Padding(0)
        EtiquetteFichiersTuiles.Name = "EtiquetteFichiersTuiles"
        EtiquetteFichiersTuiles.Size = New Size(212, 21)
        EtiquetteFichiersTuiles.TabIndex = 0
        EtiquetteFichiersTuiles.Text = "Fichiers Tuiles"
        EtiquetteFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter
        '
        'FichiersTuiles
        '
        FichiersTuiles.BackColor = Color.White
        FichiersTuiles.Cursor = Cursors.Default
        FichiersTuiles.DropDownStyle = ComboBoxStyle.DropDownList
        FichiersTuiles.FlatStyle = FlatStyle.System
        FichiersTuiles.FormattingEnabled = True
        FichiersTuiles.Location = New Point(6, 326)
        FichiersTuiles.Margin = New Padding(0)
        FichiersTuiles.Name = "FichiersTuiles"
        FichiersTuiles.Size = New Size(212, 24)
        FichiersTuiles.TabIndex = 8
        FichiersTuiles.Visible = False
        '
        'GroupeFichiersTuiles
        '
        GroupeFichiersTuiles.BackColor = Color.Transparent
        GroupeFichiersTuiles.BorderStyle = BorderStyle.FixedSingle
        GroupeFichiersTuiles.Controls.Add(EtiquetteRepertoireTuiles)
        GroupeFichiersTuiles.Controls.Add(LabelRepertoireTuiles)
        GroupeFichiersTuiles.Controls.Add(EtiquetteDimTuilesLarg)
        GroupeFichiersTuiles.Controls.Add(DimTuilesLarg)
        GroupeFichiersTuiles.Location = New Point(6, 308)
        GroupeFichiersTuiles.Name = "GroupeFichiersTuiles"
        GroupeFichiersTuiles.Size = New Size(424, 88)
        GroupeFichiersTuiles.TabIndex = 68
        '
        'EtiquetteDimTuilesLarg
        '
        EtiquetteDimTuilesLarg.BackColor = Color.Transparent
        EtiquetteDimTuilesLarg.BorderStyle = BorderStyle.FixedSingle
        EtiquetteDimTuilesLarg.Cursor = Cursors.Default
        EtiquetteDimTuilesLarg.FlatStyle = FlatStyle.Flat
        EtiquetteDimTuilesLarg.Location = New Point(210, -1)
        EtiquetteDimTuilesLarg.Margin = New Padding(0)
        EtiquetteDimTuilesLarg.Name = "EtiquetteDimTuilesLarg"
        EtiquetteDimTuilesLarg.Size = New Size(213, 21)
        EtiquetteDimTuilesLarg.TabIndex = 51
        EtiquetteDimTuilesLarg.Text = "Dimensions Tuiles"
        EtiquetteDimTuilesLarg.TextAlign = ContentAlignment.MiddleCenter
        '
        'DimTuilesLarg
        '
        DimTuilesLarg.BackColor = Color.White
        DimTuilesLarg.BorderStyle = BorderStyle.FixedSingle
        DimTuilesLarg.Increment = 256D
        DimTuilesLarg.Location = New Point(210, 19)
        DimTuilesLarg.Margin = New Padding(0)
        DimTuilesLarg.Maximum = 1024D
        DimTuilesLarg.Minimum = 256D
        DimTuilesLarg.Name = "DimTuilesLarg"
        DimTuilesLarg.ReadOnly = True
        DimTuilesLarg.Size = New Size(213, 26)
        DimTuilesLarg.TabIndex = 52
        DimTuilesLarg.TabStop = False
        DimTuilesLarg.TextAlign = HorizontalAlignment.Center
        Informations.SetToolTip(DimTuilesLarg, resources.GetString("DimTuilesLarg.ToolTip"))
        DimTuilesLarg.Value = 256D
        '
        'EtiquetteRepertoireTuiles
        '
        EtiquetteRepertoireTuiles.BackColor = Color.Transparent
        EtiquetteRepertoireTuiles.BorderStyle = BorderStyle.FixedSingle
        EtiquetteRepertoireTuiles.Location = New Point(-1, 44)
        EtiquetteRepertoireTuiles.Margin = New Padding(0)
        EtiquetteRepertoireTuiles.Name = "EtiquetteCheminTuiles"
        EtiquetteRepertoireTuiles.Size = New Size(425, 21)
        EtiquetteRepertoireTuiles.TabIndex = 0
        EtiquetteRepertoireTuiles.Text = "Répertoire d'enregistrement des fichiers Tuiles"
        EtiquetteRepertoireTuiles.TextAlign = ContentAlignment.MiddleLeft
        '
        'LabelRepertoireTuiles
        '
        LabelRepertoireTuiles.BackColor = Color.White
        LabelRepertoireTuiles.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireTuiles.Location = New Point(-1, 64)
        LabelRepertoireTuiles.Margin = New Padding(0)
        LabelRepertoireTuiles.Name = "LabelCheminTuiles"
        LabelRepertoireTuiles.Size = New Size(424, 23)
        LabelRepertoireTuiles.TabIndex = 0
        LabelRepertoireTuiles.TextAlign = ContentAlignment.MiddleLeft
        Informations.SetToolTip(LabelRepertoireTuiles, "Indique le répertoire où seront enregistrés les fichiers tuiles.")
#End Region
        '
        'GroupeConfigurations
        '
        GroupeConfigurations.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        GroupeConfigurations.BackColor = Color.FromArgb(247, 247, 247)
        GroupeConfigurations.Controls.Add(EtiquetteGroupeConfigurationCarte)
        GroupeConfigurations.Controls.Add(LabelGeoRefs)
        GroupeConfigurations.Controls.Add(EtiquetteGeoRefs)
        GroupeConfigurations.Controls.Add(GeoRefs)
        GroupeConfigurations.Controls.Add(GroupeCartes)
        GroupeConfigurations.Controls.Add(EtiquetteGroupeConfigurationTraces)
        GroupeConfigurations.Controls.Add(LabelTraces)
        GroupeConfigurations.Controls.Add(EtiquetteTraces)
        GroupeConfigurations.Controls.Add(Traces)
        GroupeConfigurations.Controls.Add(EtiquetteBoutonTrace)
        GroupeConfigurations.Controls.Add(GroupeConfigurationTraces)
        GroupeConfigurations.Controls.Add(IsTrace)
        GroupeConfigurations.Controls.Add(EtiquetteIsTrace)
        GroupeConfigurations.Controls.Add(EtiquetteNbPaths)
        GroupeConfigurations.Controls.Add(NbPaths)
        GroupeConfigurations.Controls.Add(LabelFichiersTuiles)
        GroupeConfigurations.Controls.Add(EtiquetteFichiersTuiles)
        GroupeConfigurations.Controls.Add(FichiersTuiles)
        GroupeConfigurations.Controls.Add(EtiquetteGroupeConfigurationFichiersTuiles)
        GroupeConfigurations.Controls.Add(GroupeFichiersTuiles)
        GroupeConfigurations.Location = New Point(0, 0)
        GroupeConfigurations.Name = "GroupeConfigurations"
        GroupeConfigurations.Size = New Size(434, 471)
        GroupeConfigurations.TabIndex = 0
        '
        'Information
        '
        Information.BackColor = Color.FromArgb(247, 247, 247)
        Information.BorderStyle = BorderStyle.FixedSingle
        Information.Location = New Point(6, 402)
        Information.TextAlign = ContentAlignment.TopLeft
        Information.Name = "Information"
        Information.Size = New Size(424, 63)
        Information.TabIndex = 13
        Informations.SetToolTip(Information, "Zone d'information sur les différentes phases de traitement des cartes.")
        '
        'Quitter
        '
        Quitter.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Quitter.Location = New Point(303, 477)
        Quitter.Name = "Quitter"
        Quitter.Size = New Size(128, 33)
        Quitter.TabIndex = 15
        Quitter.Text = "Quitter"
        Informations.SetToolTip(Quitter, "Quitter FCGP_Convert.")
        Quitter.UseVisualStyleBackColor = True
        '
        'Traitement
        '
        Traitement.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Traitement.Location = New Point(6, 477)
        Traitement.Name = "Traitement"
        Traitement.Size = New Size(293, 33)
        Traitement.TabIndex = 14
        Traitement.TabStop = False
        Informations.SetToolTip(Traitement, "Choisir une carte ou des cartes pour lancer le traitement.")
        Traitement.UseVisualStyleBackColor = True
        '
        'OuvrirFichiersCarte
        '
        OuvrirFichiersCarte.Filter = "GEOREF Files|*.GEOREF"
        OuvrirFichiersCarte.Multiselect = True
        OuvrirFichiersCarte.ReadOnlyChecked = True
        OuvrirFichiersCarte.Title = "Choix du ou des fichiers Georef"
        '
        'OuvrirFichiersTrace
        '
        OuvrirFichiersTrace.Filter = "Traces FCGP|*.trk"
        OuvrirFichiersTrace.Multiselect = True
        OuvrirFichiersTrace.ReadOnlyChecked = True
        OuvrirFichiersTrace.Title = "Choisir une ou plusieurs traces"
        '
        'Convertir
        '
        AutoScaleMode = AutoScaleMode.None
        ClientSize = New Size(436, 523)
        Controls.Add(Quitter)
        Controls.Add(Traitement)
        Controls.Add(Information)
        Controls.Add(GroupeConfigurations)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "Convertir"
        Text = "ConvertirCarte"
        StartPosition = FormStartPosition.CenterScreen
        GroupeConfigurations.ResumeLayout(False)
        GroupeCartes.ResumeLayout(False)
        GroupeConfigurationTraces.ResumeLayout(False)
        GroupeConfigurationTraces.PerformLayout()
        CType(Epaisseur, ISupportInitialize).EndInit()
        CType(Transparence, ISupportInitialize).EndInit()
        CType(NbPaths, ISupportInitialize).EndInit()
        GroupeFichiersTuiles.ResumeLayout(False)
        CType(DimTuilesLarg, ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Private GroupeConfigurations As Panel
    Private IsTrace As CheckBox
    Private EtiquetteIsTrace As Label
    Private EtiquetteTraces As Label
    Private Traces As ComboBox
    Private EtiquetteNbPaths As Label
    Private EtiquetteDimTuilesLarg As Label
    Private EtiquetteEpaisseur As Label
    Private Epaisseur As NumericUpDown
    Private Couleur As Label
    Private EtiquetteTransparence As Label
    Private Transparence As NumericUpDown
    Private LabelTraces As Label
    Private EtiquetteCouleur As Label
    Private EtiquetteBoutonTrace As Label
    Private EtiquetteFichiersTuiles As Label
    Private LabelRepertoireTuiles As Label
    Private EtiquetteRepertoireTuiles As Label
    Private FichiersTuiles As ComboBox
    Private IsGrille As CheckBox
    Private EtiquetteIsGrille As Label
    Private EtiquetteGeoRefs As Label
    Private GeoRefs As ComboBox
    Private LabelGeoRefs As Label
    Private EtiquetteFormatCarte As Label
    Private FormatCartes As ComboBox
    Private LabelFormatCartes As Label
    Private EtiquetteReferences As Label
    Private References As ComboBox
    Private LabelRepertoireCartes As Label
    Private EtiquetteRepertoireCartes As Label
    Private LabelReferences As Label
    Private Quitter As Button
    Private Information As Label
    Private Traitement As Button
    Private OuvrirFichiersCarte As OpenFileDialog
    Private Informations As ToolTip
    Private ChoixCouleurTrace As ColorDialog
    Private OuvrirFichiersTrace As OpenFileDialog
    Private GroupeConfigurationTraces As Panel
    Private EtiquetteGroupeConfigurationTraces As Label
    Private NbPaths As NumericUpDown
    Private GroupeCartes As Panel
    Private EtiquetteGroupeConfigurationCarte As Label
    Private EtiquetteGroupeConfigurationFichiersTuiles As Label
    Private LabelFichiersTuiles As Label
    Private DimTuilesLarg As NumericUpDown
    Private GroupeFichiersTuiles As Panel
End Class
