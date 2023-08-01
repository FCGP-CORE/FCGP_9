Partial Class Preferences
    Inherits Form
    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    ''' <summary> Initialise l'ensemble des évenements liés au formulaire car pas variable avec le modificateur WithEvents 
    ''' rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
    Private Sub InitialiserEvenements()
        'controles simples
        AddHandler RepertoireCartes.Click, AddressOf Repertoires_Click
        AddHandler RepertoireTuiles.Click, AddressOf Repertoires_Click
        AddHandler FactJNX.ValueChanged, AddressOf FactJNX_ValueChanged
        AddHandler FactORUX.ValueChanged, AddressOf FactORUX_ValueChanged
        AddHandler IsGrille.CheckedChanged, AddressOf IsGrille_CheckedChanged
        AddHandler Tuiles_0.CheckedChanged, AddressOf Tuiles_CheckedChanged
        AddHandler Tuiles_1.CheckedChanged, AddressOf Tuiles_CheckedChanged
        AddHandler Tuiles_7.CheckedChanged, AddressOf Tuiles_CheckedChanged
        AddHandler Tuiles_4.CheckedChanged, AddressOf Tuiles_CheckedChanged
        AddHandler Tuiles_2.CheckedChanged, AddressOf Tuiles_CheckedChanged
        AddHandler DimTuilesLarg.ValueChanged, AddressOf DimTuilesLarg_ValueChanged
        'FormatCartes
        AddHandler LabelChoixFormatCartes.Click, AddressOf Labels_Click
        AddHandler ListeChoixFormatCartes.DropDownClosed, AddressOf Listes_DropDownClosed
        AddHandler ListeChoixFormatCartes.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
        'GeoRefs
        AddHandler LabelChoixGeoRefs.Click, AddressOf Labels_Click
        AddHandler ListeChoixGeoRefs.DropDownClosed, AddressOf Listes_DropDownClosed
        AddHandler ListeChoixGeoRefs.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
        'formulaire
        AddHandler Load, AddressOf Preferences_Load
        AddHandler FormClosing, AddressOf Preferences_FormClosing
        AddHandler FormClosed, AddressOf Preferences_FormClosed
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Preferences))
        GroupeReferences = New Panel()
        LabelNomCarte = New Label()
        NomCarte = New TextBox()
        LabelChoixFormatCartes = New Label()
        ListeChoixFormatCartes = New ComboBox()
        LabelGeoRefs = New Label()
        LabelChoixGeoRefs = New Label()
        ListeChoixGeoRefs = New ComboBox()
        LabelFormatCartes = New Label()
        LabelRepertoireCartes = New Label()
        RepertoireCartes = New Label()
        LabelRepertoireTuiles = New Label()
        RepertoireTuiles = New Label()
        LabelGroupeFichiersTuiles = New Label()
        LabelIndices = New Label()
        LabelPlusMoins = New Label()
        FactJNX = New NumericUpDown()
        FactORUX = New NumericUpDown()
        Tuiles_0 = New RadioButton()
        Tuiles_1 = New RadioButton()
        Tuiles_2 = New RadioButton()
        Tuiles_4 = New RadioButton()
        Tuiles_7 = New RadioButton()
        LabelDimTuilesLarg = New Label()
        DimTuilesLarg = New NumericUpDown()
        GroupeFichiersTuiles = New Label()
        LabelIsGrille = New Label()
        IsGrille = New CheckBox()
        LabelIsReferences = New Label()
        IsReferences = New CheckBox()
        LabelIsTrace = New Label()
        IsTrace = New CheckBox()
        btnYES = New Button()
        btnNO = New Button()
        ToolTip1 = New ToolTip(components)
        CType(FactJNX, ISupportInitialize).BeginInit()
        CType(FactORUX, ISupportInitialize).BeginInit()
        CType(DimTuilesLarg, ISupportInitialize).BeginInit()
        GroupeReferences.SuspendLayout()
        SuspendLayout()
        '
        'LabelNomCarte
        '
        LabelNomCarte.BackColor = Color.Transparent
        LabelNomCarte.BorderStyle = BorderStyle.FixedSingle
        LabelNomCarte.FlatStyle = FlatStyle.Flat
        LabelNomCarte.Location = New Point(6, 6)
        LabelNomCarte.Name = "LabelNomCarte"
        LabelNomCarte.Size = New Size(380, 21)
        LabelNomCarte.TabIndex = 1000
        LabelNomCarte.TextAlign = ContentAlignment.MiddleCenter
        LabelNomCarte.Text = "Nom de la carte"
        '
        'NomCarte
        '
        NomCarte.BackColor = Color.White
        NomCarte.BorderStyle = BorderStyle.FixedSingle
        NomCarte.Location = New Point(6, 26)
        NomCarte.Margin = New Padding(0)
        NomCarte.Name = "NomCarte"
        NomCarte.Size = New Size(380, 26)
        NomCarte.TabIndex = 1
        ToolTip1.SetToolTip(NomCarte, "Nom de la carte. Sert de base à l'ensemble des fichiers créés.")
        '
        'LabelFormatCartes
        '
        LabelFormatCartes.BackColor = Color.Transparent
        LabelFormatCartes.BorderStyle = BorderStyle.FixedSingle
        LabelFormatCartes.Location = New Point(6, 51)
        LabelFormatCartes.Name = "LabelFormatCartes"
        LabelFormatCartes.Size = New Size(191, 21)
        LabelFormatCartes.TabIndex = 4
        LabelFormatCartes.Text = "Format enregistrement"
        LabelFormatCartes.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelChoixFormatCartes
        '
        LabelChoixFormatCartes.BackColor = Color.White
        LabelChoixFormatCartes.BorderStyle = BorderStyle.FixedSingle
        LabelChoixFormatCartes.Location = New Point(6, 71)
        LabelChoixFormatCartes.Name = "LabelChoixFormatCartes"
        LabelChoixFormatCartes.Size = New Size(191, 21)
        LabelChoixFormatCartes.TabIndex = 1007
        LabelChoixFormatCartes.TextAlign = ContentAlignment.MiddleCenter
        LabelChoixFormatCartes.Tag = ListeChoixFormatCartes
        ToolTip1.SetToolTip(LabelChoixFormatCartes, resources.GetString("LabelChoixFormatCartes.ToolTip"))
        '
        'ListeChoixFormatCartes
        '
        ListeChoixFormatCartes.BackColor = Color.White
        ListeChoixFormatCartes.Location = New Point(6, 64) '71-1
        ListeChoixFormatCartes.Margin = New Padding(0)
        ListeChoixFormatCartes.Name = "ListeChoixFormatCartes"
        ListeChoixFormatCartes.Size = New Size(191, 0)
        ListeChoixFormatCartes.Visible = False
        ListeChoixFormatCartes.TabIndex = 1
        ListeChoixFormatCartes.Tag = LabelChoixFormatCartes
        '
        'LabelGeoRefs
        '
        LabelGeoRefs.BackColor = Color.Transparent
        LabelGeoRefs.BorderStyle = BorderStyle.FixedSingle
        LabelGeoRefs.Location = New Point(196, 51)
        LabelGeoRefs.Name = "LabelGeoRefs"
        LabelGeoRefs.Size = New Size(190, 21)
        LabelGeoRefs.TabIndex = 23
        LabelGeoRefs.Text = "Géo-référencement"
        LabelGeoRefs.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelChoixGeoRefs
        '
        LabelChoixGeoRefs.BackColor = Color.White
        LabelChoixGeoRefs.BorderStyle = BorderStyle.FixedSingle
        LabelChoixGeoRefs.Location = New Point(196, 71)
        LabelChoixGeoRefs.Name = "LabelChoixGeoRefs"
        LabelChoixGeoRefs.Size = New Size(190, 21)
        LabelChoixGeoRefs.TabIndex = 45
        LabelChoixGeoRefs.TextAlign = ContentAlignment.MiddleCenter
        LabelChoixGeoRefs.Tag = ListeChoixGeoRefs
        ToolTip1.SetToolTip(LabelChoixGeoRefs, resources.GetString("LabelChoixGeoRefs.ToolTip"))
        '
        'ListeChoixGeoRefs
        '
        ListeChoixGeoRefs.BackColor = Color.White
        ListeChoixGeoRefs.Location = New Point(196, 64)
        ListeChoixGeoRefs.Name = "ListeChoixGeoRefs"
        ListeChoixGeoRefs.Size = New Size(190, 0)
        ListeChoixGeoRefs.Visible = False
        ListeChoixGeoRefs.TabIndex = 2
        ListeChoixGeoRefs.Tag = LabelChoixGeoRefs
        '
        'LabelRepertoireCartes
        '
        LabelRepertoireCartes.BackColor = Color.Transparent
        LabelRepertoireCartes.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireCartes.Location = New Point(6, 91)
        LabelRepertoireCartes.Name = "LabelRepertoireCartes"
        LabelRepertoireCartes.Size = New Size(380, 21)
        LabelRepertoireCartes.TabIndex = 19
        LabelRepertoireCartes.TextAlign = ContentAlignment.MiddleCenter
        LabelRepertoireCartes.Text = "Répertoire des Cartes"
        '
        'RepertoireCartes
        '
        RepertoireCartes.BackColor = Color.White
        RepertoireCartes.BorderStyle = BorderStyle.FixedSingle
        RepertoireCartes.Location = New Point(6, 111)
        RepertoireCartes.Name = "RepertoireCartes"
        RepertoireCartes.Size = New Size(380, 21)
        RepertoireCartes.TabIndex = 7
        RepertoireCartes.TextAlign = ContentAlignment.MiddleLeft
        RepertoireCartes.Tag = "cartes"
        ToolTip1.SetToolTip(RepertoireCartes, "Indique le répertoire actuel des cartes.")
        '
        'LabelRepertoireTuiles
        '
        LabelRepertoireTuiles.BackColor = Color.Transparent
        LabelRepertoireTuiles.BorderStyle = BorderStyle.FixedSingle
        LabelRepertoireTuiles.Location = New Point(6, 137)
        LabelRepertoireTuiles.Name = "LabelRepertoireTuiles"
        LabelRepertoireTuiles.Size = New Size(380, 21)
        LabelRepertoireTuiles.TabIndex = 16
        LabelRepertoireTuiles.TextAlign = ContentAlignment.MiddleCenter
        LabelRepertoireTuiles.Text = "Répertoire des fichiers Tuiles"
        '
        'RepertoireTuiles
        '
        RepertoireTuiles.BackColor = Color.White
        RepertoireTuiles.BorderStyle = BorderStyle.FixedSingle
        RepertoireTuiles.Location = New Point(6, 157)
        RepertoireTuiles.Name = "RepertoireTuiles"
        RepertoireTuiles.Size = New Size(380, 21)
        RepertoireTuiles.TabIndex = 17
        RepertoireTuiles.TextAlign = ContentAlignment.MiddleLeft
        RepertoireTuiles.Tag = "fichiers Tuiles"
        ToolTip1.SetToolTip(RepertoireTuiles, "Indique le répertoire actuel des fichiers tuiles.")
        '
        'LabelGroupeFichiersTuiles
        '
        LabelGroupeFichiersTuiles.BackColor = Color.Transparent
        LabelGroupeFichiersTuiles.BorderStyle = BorderStyle.FixedSingle
        LabelGroupeFichiersTuiles.Location = New Point(6, 183)
        LabelGroupeFichiersTuiles.Name = "LabelGroupeFichiersTuiles"
        LabelGroupeFichiersTuiles.Size = New Size(232, 21)
        LabelGroupeFichiersTuiles.TabIndex = 1009
        LabelGroupeFichiersTuiles.Text = "Génération des Fichiers Tuiles"
        LabelGroupeFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter
        '
        'Tuiles_0
        '
        Tuiles_0.Font = New Font("Segoe UI", 13.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        Tuiles_0.BackColor = Color.Transparent
        Tuiles_0.CheckAlign = ContentAlignment.BottomCenter
        Tuiles_0.Location = New Point(12, 203)
        Tuiles_0.Margin = New Padding(0)
        Tuiles_0.Name = "Tuiles_0"
        Tuiles_0.Size = New Size(20, 90)
        Tuiles_0.TabIndex = 5
        Tuiles_0.TabStop = True
        Tuiles_0.Text = "A" & CrLf & "c" & CrLf & "u" & CrLf & "n"
        Tuiles_0.TextAlign = ContentAlignment.TopCenter
        Tuiles_0.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(Tuiles_0, "Aucun fichier tuiles ne sera créé.")
        '
        'Tuiles_2
        '
        Tuiles_2.BackColor = Color.Transparent
        Tuiles_2.CheckAlign = ContentAlignment.BottomCenter
        Tuiles_2.Location = New Point(41, 253)
        Tuiles_2.Name = "Tuiles_2"
        Tuiles_2.Size = New Size(50, 40)
        Tuiles_2.TabIndex = 6
        Tuiles_2.TabStop = True
        Tuiles_2.Text = "JNX"
        Tuiles_2.TextAlign = ContentAlignment.TopCenter
        Tuiles_2.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(Tuiles_2, "Le fichier tuiles JNX (GPS Garmin) sera créé.")
        '
        'Tuiles_4
        '
        Tuiles_4.BackColor = Color.Transparent
        Tuiles_4.CheckAlign = ContentAlignment.BottomCenter
        Tuiles_4.Location = New Point(99, 253)
        Tuiles_4.Name = "Tuiles_4"
        Tuiles_4.Size = New Size(50, 40)
        Tuiles_4.TabIndex = 7
        Tuiles_4.TabStop = True
        Tuiles_4.Text = "KMZ"
        Tuiles_4.TextAlign = ContentAlignment.TopCenter
        Tuiles_4.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(Tuiles_4, "Le fichier tuiles KMZ (GPS Garmin et autre SIGs) sera créé.")
        '
        'Tuiles_1
        '
        Tuiles_1.BackColor = Color.Transparent
        Tuiles_1.CheckAlign = ContentAlignment.BottomCenter
        Tuiles_1.Location = New Point(157, 253)
        Tuiles_1.Margin = New Padding(0)
        Tuiles_1.Name = "Tuiles_1"
        Tuiles_1.Size = New Size(50, 40)
        Tuiles_1.TabIndex = 8
        Tuiles_1.TabStop = True
        Tuiles_1.Text = "ORUX"
        Tuiles_1.TextAlign = ContentAlignment.TopCenter
        Tuiles_1.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(Tuiles_1, "Le fichier tuiles ORUX (OruxMap et smartphone) sera créé.")
        '
        'Tuiles_7
        '
        Tuiles_7.Font = New Font("Segoe UI", 13.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        Tuiles_7.BackColor = Color.Transparent
        Tuiles_7.CheckAlign = ContentAlignment.BottomCenter
        Tuiles_7.Location = New Point(214, 203)
        Tuiles_7.Margin = New Padding(0)
        Tuiles_7.Name = "Tuiles_7"
        Tuiles_7.Size = New Size(20, 90)
        Tuiles_7.TabIndex = 9
        Tuiles_7.TabStop = True
        Tuiles_7.Text = "T" & CrLf & "o" & CrLf & "u" & CrLf & "s"
        Tuiles_7.TextAlign = ContentAlignment.TopCenter
        Tuiles_7.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(Tuiles_7, "Les fichiers tuiles JNX, KMZ et ORUX seront créés.")
        '
        'LabelIndices
        '
        LabelIndices.BackColor = Color.Transparent
        LabelIndices.BorderStyle = BorderStyle.FixedSingle
        LabelIndices.Location = New Point(38, 209)
        LabelIndices.Name = "LabelIndices"
        LabelIndices.Size = New Size(170, 21)
        LabelIndices.TabIndex = 43
        LabelIndices.Text = "Indices d'affichage"
        LabelIndices.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(LabelIndices, "Permet de modifier l'indice d'affichage." & CrLf & "Change l'échelle de transition entre 2 " &
                                          "de niveaux de détails" & CrLf & "si plusieurs niveaux de détails existent pour la même zone." & CrLf &
                                          "La valeur par défaut est en vert.")
        '
        'FactJNX
        '
        FactJNX.BackColor = Color.White
        FactJNX.DecimalPlaces = 2
        FactJNX.Increment = 0.25D
        FactJNX.Location = New Point(38, 229)
        FactJNX.Maximum = 0D
        FactJNX.Name = "FactJNX"
        FactJNX.ReadOnly = True
        FactJNX.Size = New Size(57, 26)
        FactJNX.TabIndex = 10
        FactJNX.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(FactJNX, "Concerne les fichiers JNX.")
        '
        'LabelPlusMoins
        '
        LabelPlusMoins.BackColor = Color.Transparent
        LabelPlusMoins.BorderStyle = BorderStyle.FixedSingle
        LabelPlusMoins.Location = New Point(94, 229)
        LabelPlusMoins.Name = "LabelPlusMoins"
        LabelPlusMoins.Size = New Size(58, 26)
        LabelPlusMoins.TabIndex = 42
        LabelPlusMoins.Text = "+ ou -"
        LabelPlusMoins.TextAlign = ContentAlignment.MiddleCenter
        '
        'FactORUX
        '
        FactORUX.BackColor = Color.White
        FactORUX.ForeColor = SystemColors.WindowText
        FactORUX.Location = New Point(151, 229)
        FactORUX.Maximum = 0D
        FactORUX.Name = "FactORUX"
        FactORUX.ReadOnly = True
        FactORUX.Size = New Size(57, 26)
        FactORUX.TabIndex = 11
        FactORUX.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(FactORUX, "Concerne les fichiers ORUX.")
        '
        'LabelDimTuilesLarg
        '
        LabelDimTuilesLarg.BackColor = Color.Transparent
        LabelDimTuilesLarg.BorderStyle = BorderStyle.FixedSingle
        LabelDimTuilesLarg.Location = New Point(12, 299)
        LabelDimTuilesLarg.Name = "LabelDimTuilesLarg"
        LabelDimTuilesLarg.Size = New Size(220, 21)
        LabelDimTuilesLarg.TabIndex = 44
        LabelDimTuilesLarg.Text = "Dimensions H*L des Tuiles"
        LabelDimTuilesLarg.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(LabelDimTuilesLarg, "Largeur et hauteur en pixel des tuiles" & CrLf & "incorporées dans les fichiers tuiles.")
        '
        'DimTuilesLarg
        '
        DimTuilesLarg.BackColor = Color.White
        DimTuilesLarg.BorderStyle = BorderStyle.FixedSingle
        DimTuilesLarg.Increment = 256D
        DimTuilesLarg.Location = New Point(12, 319)
        DimTuilesLarg.Maximum = 1024D
        DimTuilesLarg.Minimum = 256D
        DimTuilesLarg.Name = "DimTuilesLarg"
        DimTuilesLarg.ReadOnly = True
        DimTuilesLarg.Size = New Size(220, 26)
        DimTuilesLarg.TabIndex = 12
        DimTuilesLarg.TextAlign = HorizontalAlignment.Center
        DimTuilesLarg.Value = 256D
        ToolTip1.SetToolTip(DimTuilesLarg, resources.GetString("DimTuilesLarg.ToolTip"))
        '
        'GroupeFichiersTuiles
        '
        GroupeFichiersTuiles.BackColor = Color.Transparent
        GroupeFichiersTuiles.BorderStyle = BorderStyle.FixedSingle
        GroupeFichiersTuiles.Location = New Point(6, 203)
        GroupeFichiersTuiles.Name = "GroupeFichiersTuiles"
        GroupeFichiersTuiles.Size = New Size(232, 149)
        GroupeFichiersTuiles.TabIndex = 45
        GroupeFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter
        '
        'LabelIsGrille
        '
        LabelIsGrille.BackColor = Color.Transparent
        LabelIsGrille.BorderStyle = BorderStyle.FixedSingle
        LabelIsGrille.Location = New Point(243, 183)
        LabelIsGrille.Name = "LabelIsGrille"
        LabelIsGrille.Size = New Size(143, 57)
        LabelIsGrille.TabIndex = 39
        LabelIsGrille.TextAlign = ContentAlignment.MiddleLeft
        '
        'IsGrille
        '
        IsGrille.BackColor = Color.Transparent
        IsGrille.BackgroundImageLayout = ImageLayout.Center
        IsGrille.CheckAlign = ContentAlignment.BottomCenter
        IsGrille.FlatAppearance.BorderColor = SystemColors.ActiveCaptionText
        IsGrille.ImageAlign = ContentAlignment.MiddleRight
        IsGrille.Location = New Point(244, 193)
        IsGrille.Name = "IsGrille"
        IsGrille.Size = New Size(140, 36)
        IsGrille.TabIndex = 14
        IsGrille.Text = "Afficher Grille"
        IsGrille.TextAlign = ContentAlignment.TopCenter
        IsGrille.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(IsGrille, resources.GetString("IsGrille.ToolTip"))
        '
        'LabelIsReferences
        '
        LabelIsReferences.BackColor = Color.Transparent
        LabelIsReferences.BorderStyle = BorderStyle.FixedSingle
        LabelIsReferences.Location = New Point(243, 239)
        LabelIsReferences.Name = "LabelIsReferences"
        LabelIsReferences.Size = New Size(143, 57)
        LabelIsReferences.TabIndex = 1003
        LabelIsReferences.TextAlign = ContentAlignment.TopCenter
        '
        'IsReferences
        '
        IsReferences.BackColor = Color.Transparent
        IsReferences.BackgroundImageLayout = ImageLayout.Center
        IsReferences.CheckAlign = ContentAlignment.BottomCenter
        IsReferences.FlatAppearance.BorderColor = SystemColors.ActiveCaptionText
        IsReferences.Location = New Point(244, 248)
        IsReferences.Margin = New Padding(0)
        IsReferences.Name = "IsReferences"
        IsReferences.Size = New Size(140, 36)
        IsReferences.TabIndex = 15
        IsReferences.Text = "Reférences"
        IsReferences.TextAlign = ContentAlignment.TopCenter
        IsReferences.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(IsReferences, resources.GetString("IsReferences.ToolTip"))
        '
        'LabelIsTrace
        '
        LabelIsTrace.BackColor = Color.Transparent
        LabelIsTrace.BorderStyle = BorderStyle.FixedSingle
        LabelIsTrace.Location = New Point(243, 295)
        LabelIsTrace.Name = "LabelIsTrace"
        LabelIsTrace.Size = New Size(143, 57)
        LabelIsTrace.TabIndex = 1005
        LabelIsTrace.TextAlign = ContentAlignment.TopCenter
        '
        'IsTrace
        '
        IsTrace.BackColor = Color.Transparent
        IsTrace.CheckAlign = ContentAlignment.BottomCenter
        IsTrace.Location = New Point(244, 303)
        IsTrace.Name = "IsTrace"
        IsTrace.Size = New Size(140, 36)
        IsTrace.TabIndex = 1006
        IsTrace.Text = "Inclure Trace"
        IsTrace.TextAlign = ContentAlignment.TopCenter
        IsTrace.UseVisualStyleBackColor = False
        ToolTip1.SetToolTip(IsTrace, "Permet de dessiner la trace sur la carte capturée." & CrLf &
                                     "Cette option n'est pas disponible si il n'y a pas de trace ou" & CrLf &
                                     "si la trace n'est pas sur l'emprise de la carte à capturer.")
        '
        'GroupeReferences
        '
        GroupeReferences.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        GroupeReferences.BackColor = Color.FromArgb(247, 247, 247)
        GroupeReferences.Location = New Point(0, 0)
        GroupeReferences.Name = "GroupeReferences"
        GroupeReferences.Size = New Size(392, 358)
        GroupeReferences.TabIndex = 1008
        GroupeReferences.Controls.Add(LabelNomCarte)
        GroupeReferences.Controls.Add(NomCarte)
        GroupeReferences.Controls.Add(LabelFormatCartes)
        GroupeReferences.Controls.Add(LabelChoixFormatCartes)
        GroupeReferences.Controls.Add(ListeChoixFormatCartes)
        GroupeReferences.Controls.Add(LabelGeoRefs)
        GroupeReferences.Controls.Add(LabelChoixGeoRefs)
        GroupeReferences.Controls.Add(ListeChoixGeoRefs)
        GroupeReferences.Controls.Add(LabelRepertoireCartes)
        GroupeReferences.Controls.Add(RepertoireCartes)
        GroupeReferences.Controls.Add(LabelRepertoireTuiles)
        GroupeReferences.Controls.Add(RepertoireTuiles)
        GroupeReferences.Controls.Add(LabelGroupeFichiersTuiles)
        GroupeReferences.Controls.Add(LabelIndices)
        GroupeReferences.Controls.Add(LabelPlusMoins)
        GroupeReferences.Controls.Add(FactJNX)
        GroupeReferences.Controls.Add(FactORUX)
        GroupeReferences.Controls.Add(Tuiles_0)
        GroupeReferences.Controls.Add(Tuiles_2)
        GroupeReferences.Controls.Add(Tuiles_4)
        GroupeReferences.Controls.Add(Tuiles_1)
        GroupeReferences.Controls.Add(Tuiles_7)
        GroupeReferences.Controls.Add(LabelDimTuilesLarg)
        GroupeReferences.Controls.Add(DimTuilesLarg)
        GroupeReferences.Controls.Add(GroupeFichiersTuiles)
        GroupeReferences.Controls.Add(IsGrille)
        GroupeReferences.Controls.Add(LabelIsGrille)
        GroupeReferences.Controls.Add(IsReferences)
        GroupeReferences.Controls.Add(LabelIsReferences)
        GroupeReferences.Controls.Add(IsTrace)
        GroupeReferences.Controls.Add(LabelIsTrace)
        '
        'btnYES
        '
        btnYES.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnYES.DialogResult = DialogResult.OK
        btnYES.Location = New Point(311, 364)
        btnYES.Name = "btnYES"
        btnYES.Size = New Size(75, 30)
        btnYES.TabIndex = 16
        btnYES.Text = "Lancer"
        btnYES.UseVisualStyleBackColor = True
        ToolTip1.SetToolTip(btnYES, "Lance la création de la carte.")
        '
        'btnNO
        '
        btnNO.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnNO.BackColor = SystemColors.Control
        btnNO.DialogResult = DialogResult.Cancel
        btnNO.Location = New Point(230, 364)
        btnNO.Margin = New Padding(3, 3, 10, 10)
        btnNO.Name = "btnNO"
        btnNO.Size = New Size(75, 30)
        btnNO.TabIndex = 17
        btnNO.Text = "Annuler"
        btnNO.UseVisualStyleBackColor = True
        ToolTip1.SetToolTip(btnNO, "Annule la création de la carte.")
        '
        'ToolTip1
        '
        ToolTip1.AutoPopDelay = 5000
        ToolTip1.InitialDelay = 100
        ToolTip1.ReshowDelay = 100
        '
        'Preferences
        '
        AcceptButton = btnYES
        AutoScaleMode = AutoScaleMode.None
        CancelButton = btnNO
        ClientSize = New Size(392, 400)
        ControlBox = False
        Controls.Add(GroupeReferences)
        Controls.Add(btnNO)
        Controls.Add(btnYES)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        MaximizeBox = False
        MinimizeBox = False
        Name = "Preferences"
        ShowIcon = False
        ShowInTaskbar = False
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.CenterParent
        TransparencyKey = Color.FromArgb(255, 192, 192)
        CType(DimTuilesLarg, ISupportInitialize).EndInit()
        CType(FactJNX, ISupportInitialize).EndInit()
        CType(FactORUX, ISupportInitialize).EndInit()
        GroupeReferences.ResumeLayout(False)
        GroupeReferences.PerformLayout()
        ResumeLayout(False)
    End Sub

    Private GroupeReferences As Panel
    Private LabelNomCarte As Label
    Private NomCarte As TextBox
    Private LabelFormatCartes As Label
    Private LabelChoixFormatCartes As Label
    Private ListeChoixFormatCartes As ComboBox
    Private LabelGeoRefs As Label
    Private LabelChoixGeoRefs As Label
    Private ListeChoixGeoRefs As ComboBox
    Private LabelRepertoireCartes As Label
    Private RepertoireCartes As Label
    Private LabelRepertoireTuiles As Label
    Private RepertoireTuiles As Label
    Private LabelGroupeFichiersTuiles As Label
    Private LabelIndices As Label
    Private LabelPlusMoins As Label
    Private FactJNX As NumericUpDown
    Private FactORUX As NumericUpDown
    Private Tuiles_0 As RadioButton
    Private Tuiles_1 As RadioButton
    Private Tuiles_2 As RadioButton
    Private Tuiles_4 As RadioButton
    Private Tuiles_7 As RadioButton
    Private LabelDimTuilesLarg As Label
    Private DimTuilesLarg As NumericUpDown
    Private GroupeFichiersTuiles As Label
    Private LabelIsGrille As Label
    Private IsGrille As CheckBox
    Private LabelIsReferences As Label
    Private IsReferences As CheckBox
    Private LabelIsTrace As Label
    Private IsTrace As CheckBox
    Private btnYES As Button
    Private btnNO As Button
    Private ToolTip1 As ToolTip
End Class
