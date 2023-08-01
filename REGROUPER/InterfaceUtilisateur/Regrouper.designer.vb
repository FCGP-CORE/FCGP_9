Partial Class Regrouper
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'LabelCoordonnees
        AddHandler LabelCoordonnees.Click, AddressOf LabelListes_Click
        AddHandler LabelCoordonnees.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelCoordonnees.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        'LabelDimensions
        AddHandler LabelDimensions.Click, AddressOf LabelListes_Click
        AddHandler LabelDimensions.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelDimensions.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        'ListeChoixCoordonnees
        AddHandler ListeChoixTypeCoordonnees.SelectedIndexChanged, AddressOf ListeChoixTypeCoordonnees_SelectedIndexChanged
        'ListeChoixDimensions
        AddHandler ListeChoixDimensions.SelectedIndexChanged, AddressOf ListeChoixDimensions_SelectedIndexChanged
        'BarreIconeMode : IconeDeplacerCarte
        AddHandler IconeDeplacerCarte.Click, AddressOf IconeDeplacerCarte_Click
        'BarreIconeGestionRegroupement : IconeOuvrirRegroupement, IconeAfficherPlanRegroupement, IconeAjouterCartesRegroupement 
        AddHandler IconeOuvrirRegroupement.Click, AddressOf OuvirRegroupement_Click
        AddHandler IconeAfficherPlanRegroupement.Click, AddressOf AfficherPlanRegroupement_Click
        AddHandler IconeAjouterCartesRegroupement.Click, AddressOf AjouterCartesRegroupement_Click
        'BarreIconeListeRegroupement : IconeListeRegroupement
        AddHandler IconeListeRegroupement.SelectedIndexChanged, AddressOf IconeListeEchelle_SelectedIndexChanged
        'BarreIconeZoom : IconeZoomMoins, IconeZoom, IconeZoomPlus
        AddHandler IconeZoomMoins.Click, AddressOf ZoomMoins_Click
        AddHandler IconeZoom.Click, AddressOf IconeZoom_Click
        AddHandler IconeZoomPlus.Click, AddressOf ZoomPlus_Click
        'menu Programme : MenuGestionRegroupements, MenuFichiersTuiles, MenuConfiguration, MenuAide
        'MenuGestionRegroupements : MenuOuvrirRegroupement, MenuAjouterRegroupement, MenuSupprimerRegroupement, MenuQuitter
        AddHandler MenuOuvrirRegroupement.Click, AddressOf OuvirRegroupement_Click
        AddHandler MenuAjouterRegroupement.Click, AddressOf AjouterCartesRegroupement_Click
        'MenuSupprimerRegroupement : SuppressionListeRegroupement
        AddHandler SuppressionListeRegroupement.SelectedIndexChanged, AddressOf SuppressionListeRegroupement_SelectedIndexChanged
        AddHandler MenuQuitter.Click, AddressOf MenuQuitter_Click
        'MenuFichiersTuiles : MenuCreerFichiers, MenuModifierOrdreAffichage
        AddHandler MenuCreerFichiersJNX_ORUX.Click, AddressOf MenuCreerFichiersJNX_ORUX_Click
        AddHandler MenuCreerFichiersKMZ.Click, AddressOf MenuCreerFichiersKMZ_Click
        'MenuModifierOrdreAffichage : FichiersJNXToolStripMenuItem, FichiersORUXToolStripMenuItem
        AddHandler MenuModifierFichierKMZ.Click, AddressOf MenuModifierFichierKMZ_Click
        AddHandler FichiersJNXToolStripMenuItem.Click, AddressOf MenuModifierFichiersJNX_Click
        AddHandler FichiersORUXToolStripMenuItem.Click, AddressOf MenuModifierFichiersORUX_Click
        'MenuConfiguration : MenuConfigurationRegrouper
        AddHandler MenuConfigurationRegrouper.Click, AddressOf MenuConfigurationRegrouper_Click
        'MenuAide : MenuAideRegrouper, MenuAPropos
        AddHandler MenuAideRegrouper.Click, AddressOf MenuAideRegrouper_Click
        AddHandler MenuAPropos.Click, AddressOf MenuAPropos_Click
        'cmsZoomMenu : Zoom_0500, Zoom_0660, Zoom_0750, Zoom_0875, Zoom_1000, Zoom_1125, Zoom_1250, Zoom_1500, Zoom_1750, Zoom_2000
        AddHandler Zoom_0500.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_0660.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_0750.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_0875.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_1000.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_1125.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_1250.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_1500.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_1750.Click, AddressOf ZoomValeur_Click
        AddHandler Zoom_2000.Click, AddressOf ZoomValeur_Click
        'AffichageCarte
        AddHandler AffichageCarte.Paint, AddressOf AffichageCarte_Paint
        AddHandler AffichageCarte.MouseClick, AddressOf AffichageCarte_MouseClick
        AddHandler AffichageCarte.MouseDown, AddressOf AffichageCarte_MouseDown
        AddHandler AffichageCarte.MouseMove, AddressOf AffichageCarte_MouseMove
        AddHandler AffichageCarte.MouseUp, AddressOf AffichageCarte_MouseUp
        AddHandler AffichageCarte.MouseWheel, AddressOf AffichageCarte_MouseWheel
        AddHandler AffichageCarte.PreviewKeyDown, AddressOf AffichageCarte_PreviewKeyDown
        AddHandler AffichageCarte.Resize, AddressOf AffichageCarte_Resize
        'MenuCxtVisualisationCarte : ContextMenuListeRegroupement, ToolStripSeparator2, ContextMenuAfficherPlanRegroupement, SupprimerSelectionToolStripMenuItem
        AddHandler ContextMenuListeRegroupement.SelectedIndexChanged, AddressOf ContextMenuListeRegroupement_SelectedIndexChanged
        AddHandler ContextMenuAfficherPlanRegroupement.Click, AddressOf AfficherPlanRegroupement_Click
        AddHandler SupprimerSelectionToolStripMenuItem.Click, AddressOf SupprimerSelectionToolStripMenuItem_Click
        'formulaire
        AddHandler FormClosed, AddressOf Regrouper_FormClosed
        AddHandler Load, AddressOf Regrouper_Load
        AddHandler Shown, AddressOf Regrouper_Shown
        AddHandler LocationChanged, AddressOf Regrouper_LocationChanged
        AddHandler KeyDown, AddressOf Regrouper_KeyDown
        AddHandler KeyUp, AddressOf Regrouper_KeyUp
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Private Sub InitializeComponent()
#Region "instanciation"
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Regrouper))
        IconeOuvrirRegroupement = New ToolStripButton()
        IconeAfficherPlanRegroupement = New ToolStripButton()
        IconeAjouterCartesRegroupement = New ToolStripButton()
        MenuProgramme = New MenuStrip()
        MenuGestionRegroupements = New ToolStripMenuItem()
        MenuOuvrirRegroupement = New ToolStripMenuItem()
        MenuAjouterRegroupement = New ToolStripMenuItem()
        MenuSupprimerRegroupement = New ToolStripMenuItem()
        SuppressionListeRegroupement = New ToolStripComboBox()
        ToolStripSeparator5 = New ToolStripSeparator()
        MenuQuitter = New ToolStripMenuItem()
        MenuFichiersTuiles = New ToolStripMenuItem()
        MenuCreerFichiersJNX_ORUX = New ToolStripMenuItem()
        MenuCreerFichiersKMZ = New ToolStripMenuItem()
        ToolStripSeparator4 = New ToolStripSeparator()
        MenuModifierIndicesAffichage = New ToolStripMenuItem()
        MenuModifierFichierKMZ = New ToolStripMenuItem()
        FichiersJNXToolStripMenuItem = New ToolStripMenuItem()
        FichiersORUXToolStripMenuItem = New ToolStripMenuItem()
        MenuConfiguration = New ToolStripMenuItem()
        MenuConfigurationRegrouper = New ToolStripMenuItem()
        MenuAide = New ToolStripMenuItem()
        MenuAideRegrouper = New ToolStripMenuItem()
        MenuAPropos = New ToolStripMenuItem()
        cmsZoomMenu = New ContextMenuStrip(components)
        Zoom_0500 = New ToolStripMenuItem()
        Zoom_0660 = New ToolStripMenuItem()
        Zoom_0750 = New ToolStripMenuItem()
        Zoom_0875 = New ToolStripMenuItem()
        Zoom_1000 = New ToolStripMenuItem()
        Zoom_1125 = New ToolStripMenuItem()
        Zoom_1250 = New ToolStripMenuItem()
        Zoom_1500 = New ToolStripMenuItem()
        Zoom_1750 = New ToolStripMenuItem()
        Zoom_2000 = New ToolStripMenuItem()
        IconeZoom = New ToolStripSplitButton()
        IconeZoomMoins = New ToolStripButton()
        IconeZoomPlus = New ToolStripButton()
        BarreIconeZoom = New ToolStrip()
        OuvrirFichier = New OpenFileDialog()
        AffichageCarte = New PictureBox()
        MenuCxtVisualisationCarte = New ContextMenuStrip(components)
        ContextMenuListeRegroupement = New ToolStripComboBox()
        ToolStripSeparator2 = New ToolStripSeparator()
        ContextMenuAfficherPlanRegroupement = New ToolStripMenuItem()
        SupprimerSelectionToolStripMenuItem = New ToolStripMenuItem()
        BarreIconeGestionRegroupement = New ToolStrip()
        ToolStripSeparator1 = New ToolStripSeparator()
        BarreIconeMode = New ToolStrip()
        IconeDeplacerCarte = New ToolStripButton()
        BarreIconeListeRegroupement = New ToolStrip()
        ToolStripSeparator6 = New ToolStripSeparator()
        IconeListeRegroupement = New ToolStripComboBox()
        ToolStripSeparator7 = New ToolStripSeparator()
        Regroupement = New Label()
        LabelCoordonnees = New Label()
        ListeChoixTypeCoordonnees = New ComboBox()
        Altitude = New Label()
        LZoom = New Label()
        LModeEncours = New Label()
        LOutilEnCours = New Label()
        ListeChoixDimensions = New ComboBox()
        LabelDimensions = New Label()
        ZoneSelection = New Label()
        MenuProgramme.SuspendLayout()
        cmsZoomMenu.SuspendLayout()
        CType(AffichageCarte, ISupportInitialize).BeginInit()
        MenuCxtVisualisationCarte.SuspendLayout()
        BarreIconeZoom.SuspendLayout()
        BarreIconeGestionRegroupement.SuspendLayout()
        BarreIconeMode.SuspendLayout()
        BarreIconeListeRegroupement.SuspendLayout()
        SuspendLayout()
#End Region
#Region "Menus programme"
        '
        'MenuProgramme
        '
        MenuProgramme.AutoSize = False
        MenuProgramme.BackColor = SystemColors.Control
        MenuProgramme.Dock = DockStyle.None
        MenuProgramme.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        MenuProgramme.Items.AddRange(New ToolStripItem() {MenuGestionRegroupements, MenuFichiersTuiles, MenuConfiguration, MenuAide})
        MenuProgramme.Location = New Point(1, 0)
        MenuProgramme.Name = "MenuProgramme"
        MenuProgramme.Padding = New Padding(0)
        MenuProgramme.RenderMode = ToolStripRenderMode.ManagerRenderMode
        MenuProgramme.Size = New Size(920, 26)
        MenuProgramme.TabIndex = 0
        MenuProgramme.Text = "Fichier"
        '
        'MenuGestionEchelles
        '
        MenuGestionRegroupements.DropDownItems.AddRange(New ToolStripItem() {MenuOuvrirRegroupement, MenuAjouterRegroupement, MenuSupprimerRegroupement,
                                                                             ToolStripSeparator5, MenuQuitter})
        MenuGestionRegroupements.Name = "MenuGestionRegroupements"
        MenuGestionRegroupements.ShortcutKeyDisplayString = ""
        MenuGestionRegroupements.ShowShortcutKeys = False
        MenuGestionRegroupements.Size = New Size(134, 26)
        MenuGestionRegroupements.Text = "Gestion Regroupements"
        MenuGestionRegroupements.ToolTipText = "Gestion des regroupements (niveaux de détails)"
        '
        'MenuOuvrirEchelle
        '
        MenuOuvrirRegroupement.Image = CreerRegroupement
        MenuOuvrirRegroupement.Name = "MenuOuvrirRegroupement"
        MenuOuvrirRegroupement.RightToLeftAutoMirrorImage = True
        MenuOuvrirRegroupement.ShortcutKeyDisplayString = ""
        MenuOuvrirRegroupement.Size = New Size(249, 26)
        MenuOuvrirRegroupement.Text = "Ouvrir un regroupement ..."
        MenuOuvrirRegroupement.ToolTipText = "Ouvrir un regroupement à partir d'une carte existante (georef) et importer " & CrLf &
                                             "l'ensemble des cartes non interpolées situées dans le même répertoire" & CrLf &
                                             "ayant le même site et la même échelle de capture."
        '
        'MenuAjouterEchelle
        '
        MenuAjouterRegroupement.Image = AjouterCartesRegroupement
        MenuAjouterRegroupement.Name = "MenuAjouterRegroupement"
        MenuAjouterRegroupement.Size = New Size(249, 26)
        MenuAjouterRegroupement.Text = "Ajouter des cartes ..."
        MenuAjouterRegroupement.ToolTipText = "Ajouter les cartes non interpolées d'un autre répertoire" & CrLf &
                                              "ayant le même site et la même échelle de capture que le regroupement affiché."
        '
        'MenuSupprimerEchelle
        '
        MenuSupprimerRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Text
        MenuSupprimerRegroupement.DropDownItems.AddRange(New ToolStripItem() {SuppressionListeRegroupement})
        MenuSupprimerRegroupement.Name = "MenuSupprimerRegroupement"
        MenuSupprimerRegroupement.Size = New Size(249, 26)
        MenuSupprimerRegroupement.Text = "Supprimer un regroupement ..."
        MenuSupprimerRegroupement.ToolTipText = "Choisir le regroupement à supprimer à partir d'une liste."
        '
        'MenuListeEchelle
        '
        SuppressionListeRegroupement.DropDownStyle = ComboBoxStyle.DropDownList
        SuppressionListeRegroupement.FlatStyle = FlatStyle.System
        SuppressionListeRegroupement.Name = "MenuListeRegroupement"
        SuppressionListeRegroupement.Size = New Size(121, 23)
        '
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        ToolStripSeparator5.Size = New Size(246, 6)
        '
        'MenuQuitter
        '
        MenuQuitter.Name = "MenuQuitter"
        MenuQuitter.ShortcutKeys = Keys.Alt Or Keys.F4
        MenuQuitter.Size = New Size(249, 26)
        MenuQuitter.Text = "Quitter"
        MenuQuitter.ToolTipText = "Quitter FCGP_Visue"
        '
        'MenuFichiersTuiles
        '
        MenuFichiersTuiles.DropDownItems.AddRange(New ToolStripItem() {MenuCreerFichiersJNX_ORUX, MenuCreerFichiersKMZ, ToolStripSeparator4,
                                                                       MenuModifierFichierKMZ, MenuModifierIndicesAffichage})
        MenuFichiersTuiles.Name = "MenuFichiersTuiles"
        MenuFichiersTuiles.ShowShortcutKeys = False
        MenuFichiersTuiles.Size = New Size(116, 26)
        MenuFichiersTuiles.Text = "Fichiers tuiles"
        MenuFichiersTuiles.ToolTipText = "Gestion des fichiers tuiles"
        '
        'MenuCreerFichiersJNX_ORUX
        '
        MenuCreerFichiersJNX_ORUX.Name = "MenuCreerFichiersJNX_ORUX"
        MenuCreerFichiersJNX_ORUX.Size = New Size(271, 26)
        MenuCreerFichiersJNX_ORUX.Text = "Créer fichier ORUX / JNX ..."
        MenuCreerFichiersJNX_ORUX.ToolTipText = "Ouvre le formulaire de création" & CrLf & "des fichiers ORUX ou JNX"
        '
        'MenuCreerFichiersKMZ
        '
        MenuCreerFichiersKMZ.Name = "MenuCreerFichiersJNX_ORUX"
        MenuCreerFichiersKMZ.Size = New Size(271, 26)
        MenuCreerFichiersKMZ.Text = "Créer fichier KMZ ..."
        MenuCreerFichiersKMZ.ToolTipText = "Ouvre le formulaire de création" & CrLf & "des fichiers KMZ"
        '
        'ToolStripSeparator4
        '
        ToolStripSeparator4.Name = "ToolStripSeparator4"
        ToolStripSeparator4.Size = New Size(268, 6)
        '
        'MenuModifierIndicesAffichage
        '
        MenuModifierIndicesAffichage.DropDownItems.AddRange(New ToolStripItem() {FichiersJNXToolStripMenuItem, FichiersORUXToolStripMenuItem})
        MenuModifierIndicesAffichage.Name = "MenuModifierIndicesAffichage"
        MenuModifierIndicesAffichage.Size = New Size(271, 26)
        MenuModifierIndicesAffichage.Text = "Modifier indices d'affichage"
        MenuModifierIndicesAffichage.ToolTipText = "Permet la modification des indices d'affichages" & CrLf &
                                                   "des niveaux de détails des fichiers tuiles"
        '
        'MenuModifierFichierKMZ
        '
        MenuModifierFichierKMZ.Name = "MenuModifierFichierKMZ"
        MenuModifierFichierKMZ.Size = New Size(271, 26)
        MenuModifierFichierKMZ.Text = "Modifier fichier KMZ ..."
        MenuModifierFichierKMZ.ToolTipText = "Permet la modification d'un fichier KMZ"
        '
        'FichiersJNXToolStripMenuItem
        '
        FichiersJNXToolStripMenuItem.Name = "FichiersJNXToolStripMenuItem"
        FichiersJNXToolStripMenuItem.Size = New Size(192, 26)
        FichiersJNXToolStripMenuItem.Text = "Fichiers JNX ..."
        FichiersJNXToolStripMenuItem.ToolTipText = "Concerne les niveaux d'affichage des fichiers JNX"
        '
        'FichiersORUXToolStripMenuItem
        '
        FichiersORUXToolStripMenuItem.Name = "FichiersORUXToolStripMenuItem"
        FichiersORUXToolStripMenuItem.Size = New Size(192, 26)
        FichiersORUXToolStripMenuItem.Text = "Fichiers ORUX ..."
        FichiersORUXToolStripMenuItem.ToolTipText = "Concerne les niveaux d'affichage des fichiers ORUX"
        '
        'MenuConfiguration
        '
        MenuConfiguration.DropDownItems.AddRange(New ToolStripItem() {MenuConfigurationRegrouper})
        MenuConfiguration.Name = "MenuConfiguration"
        MenuConfiguration.Size = New Size(118, 26)
        MenuConfiguration.Text = "Configuration"
        '
        'MenuConfigurationVisue
        '
        MenuConfigurationRegrouper.Name = "MenuConfigurationRegrouper"
        MenuConfigurationRegrouper.ShortcutKeys = Keys.F3
        MenuConfigurationRegrouper.Size = New Size(158, 26)
        MenuConfigurationRegrouper.Text = "Regrouper ..."
        MenuConfigurationRegrouper.ToolTipText = "Ouvre le formulaire de configuration de FCGP_REGROUPER"
        '
        'MenuAide
        '
        MenuAide.DropDownItems.AddRange(New ToolStripItem() {MenuAideRegrouper, MenuAPropos})
        MenuAide.Name = "MenuAide"
        MenuAide.Size = New Size(53, 26)
        MenuAide.Text = "Aide"
        '
        'MenuAideVisue
        '
        MenuAideRegrouper.Name = "MenuAideRegrouper"
        MenuAideRegrouper.ShortcutKeys = Keys.F1
        MenuAideRegrouper.Size = New Size(187, 26)
        MenuAideRegrouper.Text = "Regrouper ..."
        MenuAideRegrouper.ToolTipText = "Ouvre le formulaire d'aide de FCGP_REGROUPER"
        '
        'MenuAPropos
        '
        MenuAPropos.Name = "MenuAPropos"
        MenuAPropos.ShortcutKeys = Keys.F4
        MenuAPropos.Size = New Size(187, 26)
        MenuAPropos.Text = "A Propos  ..."
        MenuAPropos.ToolTipText = "Ouvre le formulaire A Propos"
#End Region
#Region "ContexteMenu Carte"
        '
        'MenuCxtVisualisationCarte
        '
        MenuCxtVisualisationCarte.Items.AddRange(New ToolStripItem() {ContextMenuListeRegroupement, ToolStripSeparator2, ContextMenuAfficherPlanRegroupement,
                                                                      SupprimerSelectionToolStripMenuItem})
        MenuCxtVisualisationCarte.Name = "MenuContextuelVisue"
        MenuCxtVisualisationCarte.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        MenuCxtVisualisationCarte.RenderMode = ToolStripRenderMode.Professional
        MenuCxtVisualisationCarte.Size = New Size(241, 81)
        '
        'ContextMenuListeEchelle
        '
        ContextMenuListeRegroupement.AutoSize = False
        ContextMenuListeRegroupement.DropDownStyle = ComboBoxStyle.DropDownList
        ContextMenuListeRegroupement.Enabled = False
        ContextMenuListeRegroupement.FlatStyle = FlatStyle.System
        ContextMenuListeRegroupement.Name = "ContextMenuListeRegroupement"
        ContextMenuListeRegroupement.Size = New Size(180, 23)
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(237, 6)
        '
        'ContextMenuAfficherPlanEchelle
        '
        ContextMenuAfficherPlanRegroupement.Image = RegarderPlanRegroupement
        ContextMenuAfficherPlanRegroupement.Name = "ContextMenuAfficherPlanRegroupement"
        ContextMenuAfficherPlanRegroupement.ShowShortcutKeys = False
        ContextMenuAfficherPlanRegroupement.Size = New Size(240, 22)
        ContextMenuAfficherPlanRegroupement.Text = "Plan Regroupement"
        '
        'SupprimerSelectionToolStripMenuItem
        '
        SupprimerSelectionToolStripMenuItem.Name = "SupprimerSelectionToolStripMenuItem"
        SupprimerSelectionToolStripMenuItem.Size = New Size(240, 22)
        SupprimerSelectionToolStripMenuItem.Text = "Supprimer Selection"
#End Region
#Region "barreIconeMode"
        '
        'BarreIconeMode
        '
        BarreIconeMode.AllowMerge = False
        BarreIconeMode.CanOverflow = False
        BarreIconeMode.Dock = DockStyle.None
        BarreIconeMode.GripStyle = ToolStripGripStyle.Hidden
        'BarreIconeMode.ImeMode = ImeMode.Off
        BarreIconeMode.Items.AddRange(New ToolStripItem() {IconeDeplacerCarte})
        BarreIconeMode.LayoutStyle = ToolStripLayoutStyle.Flow
        BarreIconeMode.Location = New Point(1, 27)
        BarreIconeMode.Name = "BarreIconeMode"
        BarreIconeMode.RenderMode = ToolStripRenderMode.Professional
        BarreIconeGestionRegroupement.Size = New Size(36, 36)
        '
        'IconeDeplacerCarte
        '
        IconeDeplacerCarte.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeDeplacerCarte.Image = Mode_Deplacement
        IconeDeplacerCarte.ImageScaling = ToolStripItemImageScaling.None
        IconeDeplacerCarte.ImageTransparentColor = Color.Magenta
        IconeDeplacerCarte.Name = "IconeDeplacerCarte"
        IconeDeplacerCarte.Size = New Size(36, 36)
        IconeDeplacerCarte.Tag = "1"
        IconeDeplacerCarte.ToolTipText = "Mode Déplacement carte"
#End Region
#Region "BarreIconeGestionRegroupement"
        '
        'BarreIconeGestionEchelle
        '
        BarreIconeGestionRegroupement.AllowMerge = False
        BarreIconeGestionRegroupement.BackColor = SystemColors.Control
        BarreIconeGestionRegroupement.CanOverflow = False
        BarreIconeGestionRegroupement.Dock = DockStyle.None
        BarreIconeGestionRegroupement.GripStyle = ToolStripGripStyle.Hidden
        BarreIconeGestionRegroupement.ImageScalingSize = New Size(32, 32)
        BarreIconeGestionRegroupement.Items.AddRange(New ToolStripItem() {ToolStripSeparator1, IconeOuvrirRegroupement, IconeAfficherPlanRegroupement,
                                                                          IconeAjouterCartesRegroupement})
        BarreIconeGestionRegroupement.LayoutStyle = ToolStripLayoutStyle.Flow
        BarreIconeGestionRegroupement.Location = New Point(37, 27)
        BarreIconeGestionRegroupement.Name = "BarreIconeGestionRegroupement"
        BarreIconeGestionRegroupement.RenderMode = ToolStripRenderMode.Professional
        BarreIconeGestionRegroupement.Size = New Size(120, 36)
        BarreIconeGestionRegroupement.TabIndex = 2
        BarreIconeGestionRegroupement.Text = "BarreIconeEchelle"
        '
        'ToolStripSeparator1
        '
        ToolStripSeparator1.AutoSize = False
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(12, 39)
        '
        'IconeOuvrirEchelle
        '
        IconeOuvrirRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeOuvrirRegroupement.Image = CreerRegroupement
        IconeOuvrirRegroupement.ImageTransparentColor = SystemColors.Control
        IconeOuvrirRegroupement.Name = "IconeOuvrirRegroupement"
        IconeOuvrirRegroupement.Size = New Size(36, 36)
        IconeOuvrirRegroupement.Text = "&Ouvrir Regroupement"
        IconeOuvrirRegroupement.TextImageRelation = TextImageRelation.ImageAboveText
        IconeOuvrirRegroupement.ToolTipText = "Ouvrir un regroupement à partir d'une carte existante (georef) et importer " & CrLf &
                                              "l'ensemble des cartes non interpolées situées dans le même répertoire" & CrLf &
                                              "ayant le même site et la même échelle de capture."
        '
        'IconeAfficherPlanEchelle
        '
        IconeAfficherPlanRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeAfficherPlanRegroupement.Enabled = False
        IconeAfficherPlanRegroupement.Image = RegarderPlanRegroupement
        IconeAfficherPlanRegroupement.ImageTransparentColor = Color.Magenta
        IconeAfficherPlanRegroupement.Name = "IconeAfficherPlanRegroupement"
        IconeAfficherPlanRegroupement.Size = New Size(36, 36)
        IconeAfficherPlanRegroupement.Text = "&Afficher Plan"
        IconeAfficherPlanRegroupement.ToolTipText = "Affiche le plan de calepinage du regroupement affiché"
        'IconeAjouterCartesEchelle
        '
        IconeAjouterCartesRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeAjouterCartesRegroupement.Enabled = False
        IconeAjouterCartesRegroupement.Image = AjouterCartesRegroupement
        IconeAjouterCartesRegroupement.ImageTransparentColor = Color.Magenta
        IconeAjouterCartesRegroupement.Name = "IconeAjouterCartesRegroupement"
        IconeAjouterCartesRegroupement.Size = New Size(36, 36)
        IconeAjouterCartesRegroupement.Text = "A&jouter Cartes"
        IconeAjouterCartesRegroupement.ToolTipText = "Ajouter les cartes non interpolées d'un autre répertoire" & CrLf &
                                                     "ayant le même site et la même échelle de capture que le regroupement affiché."
#End Region
#Region "Liste regroupement"
        '
        'BarreIconeListeRegroupement
        '
        BarreIconeListeRegroupement.AllowMerge = False
        BarreIconeListeRegroupement.CanOverflow = False
        BarreIconeListeRegroupement.BackColor = SystemColors.Control
        BarreIconeListeRegroupement.Dock = DockStyle.None
        'BarreIconeListeRegroupement.Anchor = AnchorStyles.Left Or AnchorStyles.Top
        BarreIconeListeRegroupement.GripStyle = ToolStripGripStyle.Hidden
        BarreIconeListeRegroupement.Items.AddRange(New ToolStripItem() {ToolStripSeparator6, IconeListeRegroupement})
        BarreIconeListeRegroupement.Location = New Point(157, 27)
        BarreIconeListeRegroupement.Name = "BarreIconeListeRegroupement"
        BarreIconeListeRegroupement.RenderMode = ToolStripRenderMode.Professional
        BarreIconeListeRegroupement.Size = New Size(132, 36)
        BarreIconeListeRegroupement.TabIndex = 4
        '
        'ToolStripSeparator6
        '
        ToolStripSeparator6.AutoSize = False
        ToolStripSeparator6.Name = "ToolStripSeparator6"
        ToolStripSeparator6.Size = New Size(12, 39)
        '
        'IconeListeEchelle
        '
        IconeListeRegroupement.AutoSize = False
        IconeListeRegroupement.BackColor = SystemColors.Control
        IconeListeRegroupement.DropDownStyle = ComboBoxStyle.DropDownList
        IconeListeRegroupement.FlatStyle = FlatStyle.Flat
        IconeListeRegroupement.Font = New Font("Segoe UI", 18.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        IconeListeRegroupement.Name = "IconeListeRegroupement"
        IconeListeRegroupement.Size = New Size(120, 36)
        IconeListeRegroupement.DropDownWidth = 120
        IconeListeRegroupement.ToolTipText = "Liste des regroupements ouverts."

#End Region
#Region "Barre Zoom"
        '
        'BarreIconeZoom
        '
        BarreIconeZoom.AllowMerge = False
        BarreIconeZoom.BackColor = SystemColors.Control
        BarreIconeZoom.CanOverflow = False
        BarreIconeZoom.Dock = DockStyle.None
        BarreIconeZoom.Enabled = False
        BarreIconeZoom.GripStyle = ToolStripGripStyle.Hidden
        BarreIconeZoom.ImageScalingSize = New Size(32, 32)
        BarreIconeZoom.ImeMode = ImeMode.Off
        BarreIconeZoom.Items.AddRange(New ToolStripItem() {ToolStripSeparator7, IconeZoomMoins, IconeZoom, IconeZoomPlus})
        'BarreIconeZoom.LayoutStyle = ToolStripLayoutStyle.Flow
        BarreIconeZoom.Location = New Point(289, 27)
        BarreIconeZoom.Name = "BarreIconeZoom"
        BarreIconeZoom.RenderMode = ToolStripRenderMode.Professional
        BarreIconeZoom.Size = New Size(134, 36)
        BarreIconeZoom.TabIndex = 3
        BarreIconeZoom.Text = "BarreIconeZoom"
        '
        'ToolStripSeparator7
        '
        ToolStripSeparator7.AutoSize = False
        ToolStripSeparator7.Name = "ToolStripSeparator7"
        ToolStripSeparator7.Size = New Size(12, 39)
        '
        'IconeZoomMoins
        '
        IconeZoomMoins.BackColor = SystemColors.Control
        IconeZoomMoins.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeZoomMoins.Image = ZOOM_MOINS_COULEUR
        IconeZoomMoins.ImageTransparentColor = SystemColors.Control
        IconeZoomMoins.Name = "IconeZoomMoins"
        IconeZoomMoins.Size = New Size(36, 36)
        IconeZoomMoins.Text = "Zoom Moins"
        '
        'IconeZoom
        '
        IconeZoom.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeZoom.DropDown = cmsZoomMenu
        IconeZoom.DropDownButtonWidth = 12
        IconeZoom.Image = ZOOM_COULEUR
        IconeZoom.ImageTransparentColor = Color.Magenta
        IconeZoom.MergeAction = MergeAction.Replace
        IconeZoom.Name = "IconeZoom"
        IconeZoom.Size = New Size(50, 36)
        IconeZoom.Text = "Valeur"
        IconeZoom.ToolTipText = "Zoom par Valeur"
#Region "menuZoom"
        '
        'cmsZoomMenu
        '
        cmsZoomMenu.Items.AddRange(New ToolStripItem() {Zoom_0500, Zoom_0660, Zoom_0750, Zoom_0875, Zoom_1000, Zoom_1125,
                                                        Zoom_1250, Zoom_1500, Zoom_1750, Zoom_2000})
        cmsZoomMenu.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
        cmsZoomMenu.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        cmsZoomMenu.Name = "cmsZoom"
        cmsZoomMenu.Text = "Essai"
        cmsZoomMenu.OwnerItem = IconeZoom
        cmsZoomMenu.RenderMode = ToolStripRenderMode.Professional
        cmsZoomMenu.ShowCheckMargin = True
        cmsZoomMenu.ShowImageMargin = False
        cmsZoomMenu.Size = New Size(115, 224)
        '
        'Zoom_0500
        '
        Zoom_0500.Name = "Zoom_0500"
        Zoom_0500.Size = New Size(114, 22)
        Zoom_0500.Text = "  50 %"
        Zoom_0500.ToolTipText = "Zoom à la valeur de" & CrLf & "50% de la taille normale"
        '
        'Zoom_0660
        '
        Zoom_0660.Name = "Zoom_0660"
        Zoom_0660.Size = New Size(114, 22)
        Zoom_0660.Text = "  66 %"
        Zoom_0660.ToolTipText = "Zoom à la valeur de" & CrLf & "66% de la taille normale"
        '
        'Zoom_0750
        '
        Zoom_0750.Name = "Zoom_0750"
        Zoom_0750.Size = New Size(114, 22)
        Zoom_0750.Tag = "0.75"
        Zoom_0750.Text = "  75 %"
        Zoom_0750.ToolTipText = "Zoom à la valeur de" & CrLf & "75% de la taille normale"
        '
        'Zoom_0875
        '
        Zoom_0875.Name = "Zoom_0875"
        Zoom_0875.Size = New Size(114, 22)
        Zoom_0875.Tag = "0.875"
        Zoom_0875.Text = "  87.5 %"
        Zoom_0875.ToolTipText = "Zoom à la valeur de" & CrLf & "88% de la taille normale"
        '
        'Zoom_1000
        '
        Zoom_1000.Checked = True
        Zoom_1000.CheckState = CheckState.Checked
        Zoom_1000.Name = "Zoom_1000"
        Zoom_1000.Size = New Size(114, 22)
        Zoom_1000.Tag = "1.0"
        Zoom_1000.Text = "100 %"
        Zoom_1000.ToolTipText = "Zoom à la valeur de" & CrLf & "100% de la taille normale"
        '
        'Zoom_1125
        '
        Zoom_1125.Name = "Zoom_1125"
        Zoom_1125.Size = New Size(114, 22)
        Zoom_1125.Tag = "1.125"
        Zoom_1125.Text = "112.5 %"
        Zoom_1125.ToolTipText = "Zoom à la valeur de" & CrLf & "113% de la taille normale"
        '
        'Zoom_1250
        '
        Zoom_1250.Name = "Zoom_1250"
        Zoom_1250.Size = New Size(114, 22)
        Zoom_1250.Tag = "1.25"
        Zoom_1250.Text = "125 %"
        Zoom_1250.ToolTipText = "Zoom à la valeur de" & CrLf & "125% de la taille normale"
        '
        'Zoom_1500
        '
        Zoom_1500.Name = "Zoom_1500"
        Zoom_1500.Size = New Size(114, 22)
        Zoom_1500.Tag = "1.5"
        Zoom_1500.Text = "150 %"
        Zoom_1500.ToolTipText = "Zoom à la valeur de" & CrLf & "150% de la taille normale"
        '
        'Zoom_1750
        '
        Zoom_1750.Name = "Zoom_1750"
        Zoom_1750.Size = New Size(114, 22)
        Zoom_1750.Tag = "1.75"
        Zoom_1750.Text = "175 %"
        Zoom_1750.ToolTipText = "Zoom à la valeur de" & CrLf & "175% de la taille normale"
        '
        'Zoom_2000
        '
        Zoom_2000.Name = "Zoom_2000"
        Zoom_2000.Size = New Size(114, 22)
        Zoom_2000.Tag = "2.0"
        Zoom_2000.Text = "200 %"
        Zoom_2000.ToolTipText = "Zoom à la valeur de" & CrLf & "200% de la taille normale"
#End Region
        '
        'IconeZoomPlus
        '
        IconeZoomPlus.DisplayStyle = ToolStripItemDisplayStyle.Image
        IconeZoomPlus.Image = ZOOM_PLUS_COULEUR
        IconeZoomPlus.ImageTransparentColor = Color.Magenta
        IconeZoomPlus.Name = "IconeZoomPlus"
        IconeZoomPlus.Size = New Size(36, 36)
        IconeZoomPlus.Text = "Zoom Plus"
#End Region
#Region "Statut"
        '
        'AffichageCarte
        '
        AffichageCarte.BackColor = SystemColors.ActiveCaption
        AffichageCarte.BackgroundImageLayout = ImageLayout.None
        AffichageCarte.BorderStyle = BorderStyle.FixedSingle
        AffichageCarte.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top Or AnchorStyles.Bottom
        AffichageCarte.Dock = DockStyle.None
        AffichageCarte.Size = New Size(922, 560)
        AffichageCarte.Location = New Point(-1, 65)
        AffichageCarte.Margin = New Padding(0)
        AffichageCarte.Name = "ImageCarte"
        AffichageCarte.TabIndex = 8
        AffichageCarte.TabStop = False
        '
        'Regroupement
        '
        Regroupement.BorderStyle = BorderStyle.FixedSingle
        Regroupement.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        Regroupement.TextAlign = ContentAlignment.MiddleCenter
        Regroupement.AutoSize = False
        Regroupement.Name = "Regroupement"
        Regroupement.Location = New Point(-1, 624)
        Regroupement.Size = New Size(200, 27)
        '
        'LZoom
        '
        LZoom.BorderStyle = BorderStyle.FixedSingle
        LZoom.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        LZoom.TextAlign = ContentAlignment.MiddleCenter
        LZoom.Name = "LZoom"
        LZoom.AutoSize = False
        LZoom.Location = New Point(198, 624)
        LZoom.Size = New Size(100, 27)
        '
        'LabelCoordonnees
        '
        LabelCoordonnees.BorderStyle = BorderStyle.FixedSingle
        LabelCoordonnees.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        LabelCoordonnees.TextAlign = ContentAlignment.MiddleCenter
        LabelCoordonnees.AutoSize = False
        LabelCoordonnees.Tag = ListeChoixTypeCoordonnees
        LabelCoordonnees.Name = "LabelCoordonnees"
        LabelCoordonnees.Location = New Point(297, 624)
        LabelCoordonnees.Size = New Size(255, 27)
        '
        'ListeChoixTypeCoordonnees
        '
        ListeChoixTypeCoordonnees.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        ListeChoixTypeCoordonnees.FormattingEnabled = True
        ListeChoixTypeCoordonnees.Location = New Point(297, 624)
        ListeChoixTypeCoordonnees.Margin = New Padding(0)
        ListeChoixTypeCoordonnees.Name = "ListeChoixTypeCoordonnees"
        ListeChoixTypeCoordonnees.Size = New Size(140, 0)
        ListeChoixTypeCoordonnees.Tag = LabelCoordonnees
        ListeChoixTypeCoordonnees.TabIndex = 0
        ListeChoixTypeCoordonnees.TabStop = False
        ListeChoixTypeCoordonnees.Visible = False
        '
        'Altitude
        '
        Altitude.BorderStyle = BorderStyle.FixedSingle
        Altitude.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        Altitude.TextAlign = ContentAlignment.MiddleCenter
        Altitude.Name = "LZoom"
        Altitude.AutoSize = False
        Altitude.Location = New Point(551, 624)
        Altitude.Size = New Size(75, 27)
        '
        'LabelDimensions
        '
        LabelDimensions.BorderStyle = BorderStyle.FixedSingle
        LabelDimensions.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        LabelDimensions.TextAlign = ContentAlignment.MiddleCenter
        LabelDimensions.AutoSize = False
        LabelDimensions.Name = "LabelDimensions"
        LabelDimensions.Tag = ListeChoixDimensions
        LabelDimensions.Location = New Point(625, 624)
        LabelDimensions.Size = New Size(75, 27)
        '
        'ListeChoixDimensions
        '
        ListeChoixDimensions.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        ListeChoixDimensions.FormattingEnabled = True
        ListeChoixDimensions.Items.AddRange(New String() {"OruxJnx", "Kmz"})
        ListeChoixDimensions.Location = New Point(625, 624)
        ListeChoixDimensions.Margin = New Padding(0)
        ListeChoixDimensions.Name = "ListeChoixDimensions"
        ListeChoixDimensions.Size = New Size(75, 0)
        ListeChoixDimensions.Tag = LabelDimensions
        ListeChoixDimensions.TabIndex = 0
        ListeChoixDimensions.TabStop = False
        ListeChoixDimensions.Visible = False
        '
        'ZoneSelection
        '
        ZoneSelection.BorderStyle = BorderStyle.FixedSingle
        ZoneSelection.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        ZoneSelection.TextAlign = ContentAlignment.MiddleCenter
        ZoneSelection.AutoSize = False
        ZoneSelection.Name = "ZoneSelection"
        ZoneSelection.Location = New Point(699, 624)
        ZoneSelection.Size = New Size(300, 27)
        '
        'LModeEncours
        '
        LModeEncours.BorderStyle = BorderStyle.FixedSingle
        LModeEncours.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom
        LModeEncours.TextAlign = ContentAlignment.MiddleCenter
        LModeEncours.Name = "LZoom"
        LModeEncours.AutoSize = False
        LModeEncours.Location = New Point(622, 624)
        LModeEncours.Size = New Size(150, 27)
        '
        'LOutilEnCours
        '
        LOutilEnCours.BorderStyle = BorderStyle.FixedSingle
        LOutilEnCours.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom
        LOutilEnCours.TextAlign = ContentAlignment.MiddleCenter
        LOutilEnCours.Name = "LZoom"
        LOutilEnCours.AutoSize = False
        LOutilEnCours.Location = New Point(771, 624)
        LOutilEnCours.Size = New Size(150, 27)
#End Region
#Region "Formulaire"
        '
        'OuvrirFichier
        '
        OuvrirFichier.Filter = "GEOREF Files|*.GEOREF"
        '
        'Regrouper
        '
        AutoScaleMode = AutoScaleMode.None
        BackColor = SystemColors.Control
        BackgroundImageLayout = ImageLayout.None
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ClientSize = New Size(920, 650)
        Controls.Add(Regroupement)
        Controls.Add(ListeChoixTypeCoordonnees)
        Controls.Add(LabelCoordonnees)
        Controls.Add(ListeChoixDimensions)
        Controls.Add(LabelDimensions)
        Controls.Add(Altitude)
        Controls.Add(LZoom)
        Controls.Add(ZoneSelection)
        Controls.Add(LModeEncours)
        Controls.Add(LOutilEnCours)
        Controls.Add(AffichageCarte)
        Controls.Add(MenuProgramme)
        Controls.Add(BarreIconeMode)
        Controls.Add(BarreIconeGestionRegroupement)
        Controls.Add(BarreIconeListeRegroupement)
        Controls.Add(BarreIconeZoom)
        Cursor = Cursors.Default
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        KeyPreview = True
        Location = New Point(1980, 0)
        Margin = New Padding(2)
        MaximumSize = New Size(1920, 1200)
        MinimumSize = New Size(800, 600)
        Name = "Regrouper"
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.CenterScreen
        MenuProgramme.ResumeLayout(False)
        MenuProgramme.PerformLayout()
        cmsZoomMenu.ResumeLayout(False)
        CType(AffichageCarte, ISupportInitialize).EndInit()
        MenuCxtVisualisationCarte.ResumeLayout(False)
        BarreIconeZoom.ResumeLayout(False)
        BarreIconeZoom.PerformLayout()
        BarreIconeGestionRegroupement.ResumeLayout(False)
        BarreIconeGestionRegroupement.PerformLayout()
        BarreIconeMode.ResumeLayout(False)
        BarreIconeMode.PerformLayout()
        BarreIconeListeRegroupement.ResumeLayout(False)
        BarreIconeListeRegroupement.PerformLayout()
        ResumeLayout(False)
#End Region
    End Sub
#Region "Déclaration"
    Private AffichageCarte As PictureBox
    Private MenuProgramme As MenuStrip
    Private MenuGestionRegroupements As ToolStripMenuItem
    Private MenuOuvrirRegroupement As ToolStripMenuItem
    Private OuvrirFichier As OpenFileDialog
    Private cmsZoomMenu As ContextMenuStrip
    Private Zoom_0750 As ToolStripMenuItem
    Private Zoom_0875 As ToolStripMenuItem
    Private Zoom_1000 As ToolStripMenuItem
    Private Zoom_1500 As ToolStripMenuItem
    Private Zoom_2000 As ToolStripMenuItem
    Private MenuQuitter As ToolStripMenuItem
    Private ToolStripSeparator2 As ToolStripSeparator
    Private ContextMenuAfficherPlanRegroupement As ToolStripMenuItem
    Private MenuCxtVisualisationCarte As ContextMenuStrip
    Private IconeZoom As ToolStripSplitButton
    Private BarreIconeZoom As ToolStrip
    Private IconeZoomMoins As ToolStripButton
    Private IconeZoomPlus As ToolStripButton
    Private BarreIconeGestionRegroupement As ToolStrip
    Private IconeOuvrirRegroupement As ToolStripButton
    Private IconeAfficherPlanRegroupement As ToolStripButton
    Private IconeAjouterCartesRegroupement As ToolStripButton
    Private MenuFichiersTuiles As ToolStripMenuItem
    Private Zoom_1125 As ToolStripMenuItem
    Private Zoom_1250 As ToolStripMenuItem
    Private Zoom_1750 As ToolStripMenuItem
    Private ContextMenuListeRegroupement As ToolStripComboBox
    Private MenuSupprimerRegroupement As ToolStripMenuItem
    Private ToolStripSeparator5 As ToolStripSeparator
    Private BarreIconeListeRegroupement As ToolStrip
    Private IconeListeRegroupement As ToolStripComboBox
    Private BarreIconeMode As ToolStrip
    Private IconeDeplacerCarte As ToolStripButton
    Private MenuConfiguration As ToolStripMenuItem
    Private MenuConfigurationRegrouper As ToolStripMenuItem
    Private MenuAide As ToolStripMenuItem
    Private MenuAideRegrouper As ToolStripMenuItem
    Private MenuAPropos As ToolStripMenuItem
    Private MenuAjouterRegroupement As ToolStripMenuItem
    Private ToolStripSeparator1 As ToolStripSeparator
    Private MenuCreerFichiersJNX_ORUX As ToolStripMenuItem
    Private MenuCreerFichiersKMZ As ToolStripMenuItem
    Private ToolStripSeparator4 As ToolStripSeparator
    Private MenuModifierIndicesAffichage As ToolStripMenuItem
    Private MenuModifierFichierKMZ As ToolStripMenuItem
    Private ToolStripSeparator6 As ToolStripSeparator
    Private ToolStripSeparator7 As ToolStripSeparator
    Private FichiersJNXToolStripMenuItem As ToolStripMenuItem
    Private FichiersORUXToolStripMenuItem As ToolStripMenuItem
    Private Zoom_0500 As ToolStripMenuItem
    Private Zoom_0660 As ToolStripMenuItem
    Private SupprimerSelectionToolStripMenuItem As ToolStripMenuItem
    Private SuppressionListeRegroupement As ToolStripComboBox
    Private Regroupement As Label
    Private LabelCoordonnees As Label
    Private Altitude As Label
    Private LZoom As Label
    Private LModeEncours As Label
    Private LOutilEnCours As Label
    Private ListeChoixTypeCoordonnees As ComboBox
    Private ListeChoixDimensions As ComboBox
    Private LabelDimensions As Label
    Private ZoneSelection As Label
#End Region
End Class
