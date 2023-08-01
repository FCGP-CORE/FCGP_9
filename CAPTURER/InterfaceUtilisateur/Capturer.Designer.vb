Partial Class Capturer
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    ''' <summary> Initialise l'ensemble des évenement liés au formulaire car aucune variable avec le modificateur WithEvents 
    ''' rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
    Private Sub InitialiserEvenements()
#Region "SurfaceAffichageTuile"
        'SurfaceAffichageTuile
        AddHandler SurfaceAffichageTuile.Paint, AddressOf SurfaceAffichageTuile_Paint
        AddHandler SurfaceAffichageTuile.MouseClick, AddressOf SurfaceAffichageTuile_MouseClick
        AddHandler SurfaceAffichageTuile.MouseDown, AddressOf SurfaceAffichageTuile_MouseDown
        AddHandler SurfaceAffichageTuile.MouseMove, AddressOf SurfaceAffichageTuile_MouseMove
        AddHandler SurfaceAffichageTuile.MouseUp, AddressOf SurfaceAffichageTuile_MouseUp
        AddHandler SurfaceAffichageTuile.MouseWheel, AddressOf SurfaceAffichageTuile_MouseWheel
        AddHandler SurfaceAffichageTuile.Resize, AddressOf SurfaceAffichageTuile_Resize
        'RafraichisseurVisue : permet la MAJ de la suface d'affichage toute les 0.5s quand des tuiles sont téléchargées
        AddHandler RafraichisseurVisue.Tick, AddressOf RafraichisseurVisue_Tick
        'ContextMenu SurfaceAffichageTuile
        'DefinitPt1
        AddHandler DefinitPt1.Click, AddressOf DefinitPts_Click
        'DefinitPt2
        AddHandler DefinitPt2.Click, AddressOf DefinitPts_Click
        'AfficheDessinEchelle
        AddHandler AfficheDessinEchelle.Click, AddressOf AfficheDessinEchelle_Click
        'AfficheCouchePentes
        AddHandler AfficheCouchePentes.Click, AddressOf AfficheCouchePentes_Click
        'ChoisitAlphaPentes
        AddHandler ChoisitCoefAlphaPentes.SelectedIndexChanged, AddressOf ChoisitCoefAlphaPentes_SelectedIndexChanged
        'choisitFondPlan
        AddHandler ChoisitFondsPlan.SelectedIndexChanged, AddressOf ChoisitFondPlan_SelectedIndexChanged
        'ChoisitSitesOSM
        AddHandler ChoisitSitesOSM.SelectedIndexChanged, AddressOf ChoisitSitesOSM_SelectedIndexChanged
#End Region
#Region "Liste Sites"
        'LabelSites
        AddHandler LabelSites.Click, AddressOf LabelListes_Click
        AddHandler LabelSites.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelSites.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelSites.MouseMove, AddressOf LabelListesMenus_MouseMove
        'ListeChoixSites 
        AddHandler ListeChoixSites.SelectedIndexChanged, AddressOf ListeChoixSites_SelectedIndexChanged
        AddHandler ListeChoixSites.DropDownClosed, AddressOf ListesChoix_DropDownClosed
#End Region
#Region "Liste Echelles"
        'LabelChoixEchelles
        AddHandler LabelEchelles.Click, AddressOf LabelListes_Click
        AddHandler LabelEchelles.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelEchelles.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelEchelles.MouseMove, AddressOf LabelListesMenus_MouseMove
        'ListeEchelles
        AddHandler ListeChoixEchelles.SelectedIndexChanged, AddressOf ListeChoixEchelles_SelectedIndexChanged
        AddHandler ListeChoixEchelles.DropDownClosed, AddressOf ListesChoix_DropDownClosed
#End Region
#Region "Liste Coordonnées"
        'LabelCoordonnees
        AddHandler LabelCoordonnees.Click, AddressOf LabelListes_Click
        AddHandler LabelCoordonnees.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelCoordonnees.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelCoordonnees.MouseMove, AddressOf LabelListesMenus_MouseMove
        'ListeChoixCoordonnees
        AddHandler ListeChoixTypeCoordonnees.SelectedIndexChanged, AddressOf ListeChoixTypeCoordonnees_SelectedIndexChanged
        AddHandler ListeChoixTypeCoordonnees.DropDownClosed, AddressOf ListesChoix_DropDownClosed
#End Region
#Region "MenuTraces"
        'LabelTrace
        AddHandler LabelTrace.Click, AddressOf LabelMenus_Click
        AddHandler LabelTrace.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelTrace.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelTrace.MouseMove, AddressOf LabelListesMenus_MouseMove
        'ContextMenuTrace
        AddHandler ContextMenuTrace.Closed, AddressOf ContextMenus_Closed
        AddHandler ContextMenuTrace.Opening, AddressOf ContextMenus_Opening
        'EditeTrace
        AddHandler EditeTrace.Click, AddressOf EditeTrace_Click
        'FermeTrace
        AddHandler FermeTrace.Click, AddressOf FermeTrace_Click
        'ProprietesTrace
        AddHandler ProprietesTrace.Click, AddressOf ProprietesTrace_Click
        'Va_A_DebutTrace
        AddHandler Va_A_DebutTrace.Click, AddressOf Va_A_DebutTrace_Click
        'Va_A_FinTrace
        AddHandler Va_A_FinTrace.Click, AddressOf Va_A_FinTrace_Click
        'RevientDe
        AddHandler RevientDe_MenuTrace.Click, AddressOf RevientDe_Click
        'OuvreTrace
        AddHandler OuvreTrace.Click, AddressOf OuvreTrace_Click
#End Region
#Region "MenuPoints"
        'LabelMenuPt1
        AddHandler LabelMenuPt1.Click, AddressOf LabelMenus_Click
        AddHandler LabelMenuPt1.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelMenuPt1.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelMenuPt1.MouseMove, AddressOf LabelListesMenus_MouseMove
        'LabelMenuPt2
        AddHandler LabelMenuPt2.Click, AddressOf LabelMenus_Click
        AddHandler LabelMenuPt2.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelMenuPt2.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelMenuPt2.MouseMove, AddressOf LabelListesMenus_MouseMove
        'ContextMenuPts
        AddHandler ContextMenuPts.Closed, AddressOf ContextMenus_Closed
        AddHandler ContextMenuPts.Opening, AddressOf ContextMenus_Opening
        'SaisieSurfaceCapturePts
        AddHandler SaisieSurfaceTraitementPts.Click, AddressOf SaisieSurfacesTraitements_Click
        'DefinitCentrePts
        AddHandler DefinitCentrePts.Click, AddressOf DefinitCentrePts_Click
        'EffacePts
        AddHandler EffacePts.Click, AddressOf EffacePts_Click
        'Va_A_Pts
        AddHandler Va_A_Pts.Click, AddressOf Va_A_Pts_Click
        'RevientDe_Pts
        AddHandler RevientDe_Pts.Click, AddressOf RevientDe_Click
#End Region
#Region "MenuApplication"
        'LabelMenuApp
        AddHandler LabelMenuApp.Click, AddressOf LabelMenus_Click
        AddHandler LabelMenuApp.MouseEnter, AddressOf LabelListesMenus_MouseEnter
        AddHandler LabelMenuApp.MouseLeave, AddressOf LabelListesMenus_MouseLeave
        AddHandler LabelMenuApp.MouseMove, AddressOf LabelListesMenus_MouseMove
        'ContextMenuApp
        AddHandler ContextMenuApp.Closed, AddressOf ContextMenus_Closed
        AddHandler ContextMenuApp.Opening, AddressOf ContextMenus_Opening
        'ConfigureApp
        AddHandler ConfigureApp.Click, AddressOf ConfigureApp_Click
        'TesteServeur
        AddHandler TesteServeur.Click, AddressOf TesteServeur_Click
        'CompacteCacheTuiles
        AddHandler CompacteCacheTuiles.Click, AddressOf CompacteCacheTuiles_Click
        'SupprimeTuiles
        AddHandler SupprimeTuiles.Click, AddressOf SupprimeTuiles_Click
        'LanceCreationCarte
        AddHandler LanceCreationCarte.Click, AddressOf LanceCreationCarte_Click
        'Va_A_App
        AddHandler Va_A_App.Click, AddressOf Va_A_App_Click
        'RevientDe_App
        AddHandler RevientDe_App.Click, AddressOf RevientDe_Click
        'LimitesSite
        AddHandler LimitesSite.Click, AddressOf LimitesSite_Click
        'Va_A_App
        AddHandler Va_A_Site.Click, AddressOf Va_A_site_Click
        'SaisieSurfaceTraitementApp
        AddHandler SaisieSurfaceTraitementApp.Click, AddressOf SaisieSurfacesTraitements_Click
        'APropos
        AddHandler APropos.Click, AddressOf APropos_Click
        'Aide
        AddHandler Aide.Click, AddressOf Aide_Click
        'ImprimerCarte
        AddHandler ImprimeCarte.Click, AddressOf ImprimerCarte_Click
        'FermeApp
        AddHandler FermeApp.Click, AddressOf FermeApp_Click
#End Region
#Region "Zones Informations"
        'Internet
        'ConnectedInternet
        AddHandler ConnexionInternet.Tick, AddressOf ConnectedInternet_Tick
        'LabelInternet
        AddHandler LabelConnexionInternet.MouseMove, AddressOf Information_MouseMove
        'NbDemandesTuiles
        AddHandler LabelNbDemandesTuiles.MouseMove, AddressOf Information_MouseMove
        'LabelAltitude
        AddHandler LabelAltitude.MouseMove, AddressOf Information_MouseMove
        'TuileCurseur
        AddHandler LabelInformation.MouseMove, AddressOf Information_MouseMove
#End Region
#Region "Formulaire"
        AddHandler FormClosing, AddressOf Capturer_FormClosing
        AddHandler FormClosed, AddressOf Capturer_FormClosed
        AddHandler Load, AddressOf Capturer_Load
        AddHandler LocationChanged, AddressOf Capturer_LocationChanged
        AddHandler KeyDown, AddressOf Capturer_KeyDown
        AddHandler KeyUp, AddressOf Capturer_KeyUp
        AddHandler PreviewKeyDown, AddressOf Capturer_PreviewKeyDown
#End Region
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
#Region "création des contrôles Hors IU"
        components = New Container()
        Dim resources As New ComponentResourceManager(GetType(Capturer))
        'attention plusieurs timer sont accessibles à ce niveau
        RafraichisseurVisue = New System.Windows.Forms.Timer(components)
        ConnexionInternet = New System.Windows.Forms.Timer(components)
        AideContextuelle = New ToolTip(components)
        SupportControlesIU = New Panel()
#End Region
#Region "création des contrôles IU"
        SurfaceAffichageTuile = New PictureBox()
        ContextMenuVisue = New ContextMenuStrip(components)
        DefinitPt1 = New ToolStripMenuItem()
        DefinitPt2 = New ToolStripMenuItem()
        MenuVisueSeparateur_0 = New ToolStripSeparator()
        AfficheDessinEchelle = New ToolStripMenuItem
        AfficheCouchePentes = New ToolStripMenuItem
        ChoisitCoefAlphaPentes = New ToolStripComboBox()
        MenuVisueSeparateur_1 = New ToolStripSeparator()
        ChoisitFondsPlan = New ToolStripComboBox()
        MenuVisueSeparateur_2 = New ToolStripSeparator()
        ChoisitSitesOSM = New ToolStripComboBox()

        LabelConnexionInternet = New Label()
        LabelNbDemandesTuiles = New Label()

        LabelSites = New Label()
        ListeChoixSites = New ComboBox()

        LabelCoordonnees = New Label()
        ListeChoixTypeCoordonnees = New ComboBox()

        LabelEchelles = New Label()
        ListeChoixEchelles = New ComboBox()

        LabelAltitude = New Label()

        LabelTrace = New Label()
        ContextMenuTrace = New ContextMenuStrip(components)
        EditeTrace = New ToolStripMenuItem()
        FermeTrace = New ToolStripMenuItem()
        ProprietesTrace = New ToolStripMenuItem()
        MenuTraceSeparator_0 = New ToolStripSeparator()
        Va_A_DebutTrace = New ToolStripMenuItem()
        Va_A_FinTrace = New ToolStripMenuItem()
        RevientDe_MenuTrace = New ToolStripMenuItem()
        MenuTraceSeparator_1 = New ToolStripSeparator()
        OuvreTrace = New ToolStripMenuItem()

        LabelMenuPt1 = New Label()
        LabelMenuPt2 = New Label()
        ContextMenuPts = New ContextMenuStrip(components)
        SaisieSurfaceTraitementPts = New ToolStripMenuItem()
        MenuPtsSeparator_0 = New ToolStripSeparator()
        DefinitCentrePts = New ToolStripMenuItem()
        EffacePts = New ToolStripMenuItem()
        MenuPtsSeparator_1 = New ToolStripSeparator()
        Va_A_Pts = New ToolStripMenuItem()
        RevientDe_Pts = New ToolStripMenuItem()

        LabelInformation = New Label()

        LabelMenuApp = New Label()
        ContextMenuApp = New ContextMenuStrip(components)
        ConfigureApp = New ToolStripMenuItem()
        TesteServeur = New ToolStripMenuItem()
        CompacteCacheTuiles = New ToolStripMenuItem()
        MenuAppSeparator_0 = New ToolStripSeparator()
        SaisieSurfaceTraitementApp = New ToolStripMenuItem()
        LanceCreationCarte = New ToolStripMenuItem()
        SupprimeTuiles = New ToolStripMenuItem()
        MenuAppSeparator_1 = New ToolStripSeparator()
        Va_A_App = New ToolStripMenuItem()
        RevientDe_App = New ToolStripMenuItem()
        Va_A_Site = New ToolStripMenuItem()
        LimitesSite = New ToolStripMenuItem()
        MenuAppSeparator_2 = New ToolStripSeparator()
        APropos = New ToolStripMenuItem()
        Aide = New ToolStripMenuItem()
        ImprimeCarte = New ToolStripMenuItem()
        MenuAppSeparator_3 = New ToolStripSeparator()
        FermeApp = New ToolStripMenuItem()

        'Contrôles conteneurs
        CType(SurfaceAffichageTuile, ISupportInitialize).BeginInit()
        ContextMenuVisue.SuspendLayout()
        ContextMenuApp.SuspendLayout()
        ContextMenuPts.SuspendLayout()
        ContextMenuTrace.SuspendLayout()
        SupportControlesIU.SuspendLayout()
        SuspendLayout()
#End Region
#Region "Configuration de éléments IU"
#Region "Visue"
        '
        'SurfaceAffichageTuile
        '
        SurfaceAffichageTuile.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        SurfaceAffichageTuile.ContextMenuStrip = ContextMenuVisue
        SurfaceAffichageTuile.Location = New Point(0, 0)
        SurfaceAffichageTuile.Margin = New Padding(0)
        SurfaceAffichageTuile.Name = "SurfaceAffichageTuile"
        SurfaceAffichageTuile.Size = New Size(1184, 438)
        SurfaceAffichageTuile.TabIndex = 0
        SurfaceAffichageTuile.TabStop = False
        '
        'MenuVisue
        '
        ContextMenuVisue.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ContextMenuVisue.Items.AddRange(New ToolStripItem() {DefinitPt1, DefinitPt2,
                                                             MenuVisueSeparateur_0, AfficheDessinEchelle, AfficheCouchePentes, ChoisitCoefAlphaPentes,
                                                             MenuVisueSeparateur_1, ChoisitFondsPlan,
                                                             MenuVisueSeparateur_2, ChoisitSitesOSM})
        ContextMenuVisue.Name = "ContextMenuVisue"
        ContextMenuVisue.ShowImageMargin = False
        '
        'DefinitPt1
        '
        DefinitPt1.Name = "DefinirPt1"
        DefinitPt1.ShowShortcutKeys = False
        DefinitPt1.Tag = "Pt1"
        DefinitPt1.Text = "Définir Pt1"
        DefinitPt1.ToolTipText = "Définit le point avec les coordonnées du pointeur de souris."
        '
        'DefinitPt2
        '
        DefinitPt2.Name = "DefinirPt2"
        DefinitPt2.ShowShortcutKeys = False
        DefinitPt2.Tag = "Pt2"
        DefinitPt2.Text = "Définir Pt2"
        DefinitPt2.ToolTipText = "Définit le point avec les coordonnées du pointeur de souris."
        '
        'MenuVisueSeparateur_0
        '
        MenuVisueSeparateur_0.Name = "MenuVisueSeparateur_0"
        '
        'AfficheDessinEchelle
        '
        AfficheDessinEchelle.Name = "AfficheDessinEchelle"
        AfficheDessinEchelle.ShowShortcutKeys = False
        AfficheDessinEchelle.ShortcutKeys = Keys.F12
        AfficheDessinEchelle.Text = "AfficherEchelle"
        AfficheDessinEchelle.ToolTipText = resources.GetString("AfficheDessinEchelle.ToolTip")
        '
        'AfficheCouchePentes
        '
        AfficheCouchePentes.Name = "AfficheCouchePentes"
        AfficheCouchePentes.ShowShortcutKeys = False
        AfficheCouchePentes.ShortcutKeys = Keys.Alt Or Keys.F12
        AfficheCouchePentes.Text = "AfficherEchelle"
        AfficheCouchePentes.ToolTipText = resources.GetString("AffichePentes.ToolTip")
        '
        'ChoisitCoefAlphaPentes
        '
        ChoisitCoefAlphaPentes.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ChoisitCoefAlphaPentes.Items.AddRange(New String() {"  15 %", "  33 %", "  50 %", "  66 %", "  80 %", " 100 %"})
        ChoisitCoefAlphaPentes.Name = "ChoisitCoefAlphaPentes"
        ChoisitCoefAlphaPentes.Size = New Size(120, 27)
        ChoisitCoefAlphaPentes.ToolTipText = "Permet de choisir le coefficient de transparence" & CrLf & "de la couche des pentes"
        '
        'MenuVisueSeparateur_1
        '
        MenuVisueSeparateur_1.Name = "MenuVisueSeparateur_1"
        '
        'ChoisitFondPlan
        '
        ChoisitFondsPlan.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ChoisitFondsPlan.Name = "ChoisitFondPlan"
        ChoisitFondsPlan.Size = New Size(120, 27)
        ChoisitFondsPlan.ToolTipText = "Permet de choisir le fond de plan à afficher"
        '
        'MenuVisueSeparateur_2
        '
        MenuVisueSeparateur_2.Name = "MenuVisueSeparateur_2"
        '
        'ChoisitSitesOSM
        '
        ChoisitSitesOSM.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ChoisitSitesOSM.Name = "ChoisitSitesOSM"
        ChoisitSitesOSM.Size = New Size(120, 27)
        ChoisitSitesOSM.ToolTipText = "Permet de choisir un des 2 sites OSM"
#End Region
#Region "Information Internet"
        '
        'LabelConnexionInternet
        '
        LabelConnexionInternet.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelConnexionInternet.Font = New Font("HoloLens MDL2 Assets", 18.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        LabelConnexionInternet.Location = New Point(0, 438)
        LabelConnexionInternet.Margin = New Padding(0)
        LabelConnexionInternet.Name = "LabelConnexionInternet"
        LabelConnexionInternet.Size = New Size(25, 24)
        LabelConnexionInternet.TabIndex = 9
        LabelConnexionInternet.Text = ""
        LabelConnexionInternet.TextAlign = ContentAlignment.MiddleLeft
        AideContextuelle.SetToolTip(LabelConnexionInternet, "Indique l'état de la connection internet." & CrLf &
                                                            "Couleur Verte     : Connecté" & CrLf &
                                                            "Couleur Rouge  : Non Connecté")
#End Region
#Region "Information NbDemandesTuiles"
        '
        'LabelNbDemandesTuiles
        '
        LabelNbDemandesTuiles.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelNbDemandesTuiles.Location = New Point(25, 438)
        LabelNbDemandesTuiles.Margin = New Padding(0)
        LabelNbDemandesTuiles.Name = "LabelNbDemandesTuiles"
        LabelNbDemandesTuiles.Size = New Size(25, 24)
        LabelNbDemandesTuiles.TabIndex = 10
        LabelNbDemandesTuiles.Text = "0"
        LabelNbDemandesTuiles.TextAlign = ContentAlignment.MiddleCenter
        AideContextuelle.SetToolTip(LabelNbDemandesTuiles, resources.GetString("LabelNbDemandesTuiles.ToolTip"))
#End Region
#Region "Choix Sites"
        '
        'LabelSites
        '
        LabelSites.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelSites.BackColor = Color.Transparent
        LabelSites.Location = New Point(50, 438)
        LabelSites.Margin = New Padding(0)
        LabelSites.Name = "LabelSites"
        LabelSites.Size = New Size(110, 24)
        LabelSites.TabIndex = 14
        LabelSites.TextAlign = ContentAlignment.MiddleLeft
        LabelSites.Tag = ListeChoixSites
        AideContextuelle.SetToolTip(LabelSites, "Site ou Domtom sélectionné pour la capture." & CrLf &
                                                "Cliquez dessus pour ouvrir une liste de choix.")
        '
        'ListeChoixSites
        '
        ListeChoixSites.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        ListeChoixSites.DropDownStyle = ComboBoxStyle.DropDownList
        ListeChoixSites.FlatStyle = FlatStyle.System
        ListeChoixSites.FormattingEnabled = True
        ListeChoixSites.Location = New Point(50, 438)
        ListeChoixSites.Margin = New Padding(0)
        ListeChoixSites.Name = "ListeChoixSites"
        ListeChoixSites.Size = New Size(105, 0)
        ListeChoixSites.TabIndex = 4
        ListeChoixSites.TabStop = False
        ListeChoixSites.Visible = False
#End Region
#Region "Choix Echelles"
        '
        'LabelEchelles
        '
        LabelEchelles.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelEchelles.Location = New Point(160, 438)
        LabelEchelles.Margin = New Padding(0)
        LabelEchelles.Name = "LabelEchelles"
        LabelEchelles.Size = New Size(90, 24)
        LabelEchelles.TabIndex = 13
        LabelEchelles.TextAlign = ContentAlignment.MiddleLeft
        LabelEchelles.Tag = ListeChoixEchelles
        AideContextuelle.SetToolTip(LabelEchelles, "Echelle sélectionnée pour la capture." & CrLf &
                                                   "Cliquez dessus pour ouvrir une liste de choix.")
        '
        'ListeChoixEchelles
        '
        ListeChoixEchelles.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        ListeChoixEchelles.DropDownStyle = ComboBoxStyle.DropDownList
        ListeChoixEchelles.FlatStyle = FlatStyle.System
        ListeChoixEchelles.FormattingEnabled = True
        ListeChoixEchelles.Location = New Point(160, 438)
        ListeChoixEchelles.Margin = New Padding(0)
        ListeChoixEchelles.Name = "ListeChoixEchelles"
        ListeChoixEchelles.Size = New Size(85, 0)
        ListeChoixEchelles.TabIndex = 6
        ListeChoixEchelles.TabStop = False
        ListeChoixEchelles.Visible = False
#End Region
#Region "Choix Type Coordonnées"
        '
        'LabelCoordonnees
        '
        LabelCoordonnees.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelCoordonnees.Location = New Point(250, 438)
        LabelCoordonnees.Margin = New Padding(0)
        LabelCoordonnees.Name = "LabelCoordonnees"
        LabelCoordonnees.Size = New Size(255, 24)
        LabelCoordonnees.TabIndex = 1
        LabelCoordonnees.TextAlign = ContentAlignment.MiddleLeft
        LabelCoordonnees.Tag = ListeChoixTypeCoordonnees
        AideContextuelle.SetToolTip(LabelCoordonnees, resources.GetString("LabelCoordonnees.ToolTip"))
        '
        'ListeChoixTypeCoordonnees
        '
        ListeChoixTypeCoordonnees.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        ListeChoixTypeCoordonnees.DropDownStyle = ComboBoxStyle.DropDownList
        ListeChoixTypeCoordonnees.FlatStyle = FlatStyle.System
        ListeChoixTypeCoordonnees.FormattingEnabled = True
        ListeChoixTypeCoordonnees.Location = New Point(250, 438)
        ListeChoixTypeCoordonnees.Margin = New Padding(0)
        ListeChoixTypeCoordonnees.Name = "ListeChoixTypeCoordonnees"
        ListeChoixTypeCoordonnees.Size = New Size(140, 0)
        ListeChoixTypeCoordonnees.TabIndex = 0
        ListeChoixTypeCoordonnees.TabStop = False
        ListeChoixTypeCoordonnees.Visible = False
#End Region
#Region "Information Altitude"
        '
        'LabelAltitude
        '
        LabelAltitude.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelAltitude.Location = New Point(505, 438)
        LabelAltitude.Margin = New Padding(0)
        LabelAltitude.Name = "LabelAltitude"
        LabelAltitude.Size = New Size(60, 24)
        LabelAltitude.TabIndex = 11
        LabelAltitude.Text = "0 m"
        LabelAltitude.TextAlign = ContentAlignment.MiddleRight
        AideContextuelle.SetToolTip(LabelAltitude, "Altitude du pointeur de souris")
#End Region
#Region "MenuTrace"
        '
        'LabelTrace
        '
        LabelTrace.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelTrace.ContextMenuStrip = ContextMenuTrace
        LabelTrace.Location = New Point(565, 438)
        LabelTrace.Margin = New Padding(0)
        LabelTrace.Name = "LabelTrace"
        LabelTrace.Size = New Size(160, 24)
        LabelTrace.TabIndex = 12
        LabelTrace.Text = "Pas de trace"
        LabelTrace.TextAlign = ContentAlignment.MiddleLeft
        AideContextuelle.SetToolTip(LabelTrace, resources.GetString("LabelTrace.ToolTip"))
        '
        'ContextMenuTrace
        '
        ContextMenuTrace.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ContextMenuTrace.Items.AddRange(New ToolStripItem() {EditeTrace, FermeTrace, ProprietesTrace,
                                                             MenuTraceSeparator_0, Va_A_DebutTrace, Va_A_FinTrace, RevientDe_MenuTrace,
                                                             MenuTraceSeparator_1, OuvreTrace})
        ContextMenuTrace.Name = "ContextMenuTrace"
        ContextMenuTrace.ShowImageMargin = False
        ContextMenuTrace.Tag = LabelTrace
        '
        'EditeTrace
        '
        EditeTrace.Name = "EditionTrace"
        EditeTrace.ShortcutKeys = Keys.Alt Or Keys.E
        EditeTrace.Text = "&Edition Trace On"
        EditeTrace.ToolTipText = "Bascule du mode édition de la trace." & CrLf &
                                 "Off : Curseur Grande croix et couleur du nom de la trace vert." & CrLf &
                                 "On : Curseur Réticule et couleur du nom de la trace Orange."
        '
        'FermeTrace
        '
        FermeTrace.Name = "FermerTrace"
        FermeTrace.ShortcutKeys = Keys.Alt Or Keys.F
        FermeTrace.Text = "&Femer Trace ..."
        FermeTrace.ToolTipText = "Ouvre le formulaire de fermeture d'une trace."
        '
        'ProprietesTrace
        '
        ProprietesTrace.Name = "ProprietesTrace"
        ProprietesTrace.ShortcutKeys = Keys.Alt Or Keys.P
        ProprietesTrace.Text = "&Propriété Trace ..."
        ProprietesTrace.ToolTipText = "Ouvre le formulaire des propriétés de la trace."
        '
        'MenuTraceSeparator_0
        '
        MenuTraceSeparator_0.Name = "MenuTraceSeparator_0"
        '
        'Va_A_DebutTrace
        '
        Va_A_DebutTrace.Name = "DebutTrace"
        Va_A_DebutTrace.ShortcutKeys = Keys.Alt Or Keys.D
        Va_A_DebutTrace.Text = "Aller à &Début Trace"
        Va_A_DebutTrace.ToolTipText = "Positionne le centre de l'affichage" & CrLf &
                                      "sur le 1er point de la trace."
        '
        'Va_A_FinTrace
        '
        Va_A_FinTrace.Name = "Va_A_FinTrace"
        Va_A_FinTrace.ShortcutKeys = Keys.Alt Or Keys.A
        Va_A_FinTrace.Text = "&Aller à Fin Trace"
        Va_A_FinTrace.ToolTipText = "Positionne le centre de l'affichage" & CrLf &
                                    "sur le dernier point de la trace."
        '
        'RevientDe_MenuTrace
        '
        RevientDe_MenuTrace.Name = "RevientDe_MenuTrace"
        RevientDe_MenuTrace.ShortcutKeys = Keys.F5
        RevientDe_MenuTrace.Text = "Revenir"
        '
        'MenuTraceSeparator_1
        '
        MenuTraceSeparator_1.Name = "MenuTraceSeparator_1"
        '
        'OuvreTrace
        '
        OuvreTrace.Name = "OuvreTrace"
        OuvreTrace.ShortcutKeys = Keys.F10
        OuvreTrace.Text = "Ouvrir une Trace ..."
        OuvreTrace.ToolTipText = "Ouvre le formulaire d'ouverture d'une trace."
#End Region
#Region "MenuPoints"
        '
        'LabelMenuPt1
        '
        LabelMenuPt1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelMenuPt1.ContextMenuStrip = ContextMenuPts
        LabelMenuPt1.FlatStyle = FlatStyle.Flat
        LabelMenuPt1.Font = New Font("Wingdings", 23.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        LabelMenuPt1.Location = New Point(725, 438)
        LabelMenuPt1.Margin = New Padding(0)
        LabelMenuPt1.Name = "LabelMenuPt1"
        LabelMenuPt1.Padding = New Padding(2, 0, 0, 0)
        LabelMenuPt1.Size = New Size(25, 24)
        LabelMenuPt1.TabIndex = 2
        LabelMenuPt1.Tag = "Pt1"
        LabelMenuPt1.Text = ""
        LabelMenuPt1.TextAlign = ContentAlignment.MiddleCenter
        AideContextuelle.SetToolTip(LabelMenuPt1, "1er point définissant la surface de traitement." & CrLf &
                                    resources.GetString("LabelMenuPts.ToolTip"))
        '
        'LabelMenuPt2
        '
        LabelMenuPt2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        LabelMenuPt2.ContextMenuStrip = ContextMenuPts
        LabelMenuPt2.FlatStyle = FlatStyle.Flat
        LabelMenuPt2.Font = New Font("Wingdings", 23.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        LabelMenuPt2.Location = New Point(750, 438)
        LabelMenuPt2.Margin = New Padding(0)
        LabelMenuPt2.Name = "LabelMenuPt2"
        LabelMenuPt2.Padding = New Padding(2, 0, 0, 0)
        LabelMenuPt2.Size = New Size(25, 24)
        LabelMenuPt2.TabIndex = 3
        LabelMenuPt2.Tag = "Pt2"
        LabelMenuPt2.Text = ""
        LabelMenuPt2.TextAlign = ContentAlignment.MiddleCenter
        AideContextuelle.SetToolTip(LabelMenuPt2, "2ème point définissant la surface de traitement." & CrLf &
                                    resources.GetString("LabelMenuPts.ToolTip"))
        '
        'ContextMenuPts
        '
        ContextMenuPts.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ContextMenuPts.Items.AddRange(New ToolStripItem() {SaisieSurfaceTraitementPts,
                                                           MenuPtsSeparator_0, DefinitCentrePts, EffacePts,
                                                           MenuPtsSeparator_1, Va_A_Pts, RevientDe_Pts})
        ContextMenuPts.Name = "ContextMenuPts"
        ContextMenuPts.ShowImageMargin = False
        '
        'SaisieSurfaceTraitementPts
        '
        SaisieSurfaceTraitementPts.Name = "SaisieSurfaceTraitementPts"
        SaisieSurfaceTraitementPts.Tag = "Pts"
        SaisieSurfaceTraitementPts.Text = "Surface de Traitement ..."
        SaisieSurfaceTraitementPts.ToolTipText = resources.GetString("SaisieSurfaceTraitementPts.ToolTip")
        '
        'MenuPtsSeparator_0
        '
        MenuPtsSeparator_0.Name = "MenuPtsSeparator_0"
        '
        'DefinitCentrePts
        '
        DefinitCentrePts.Name = "DefinitCentrePts"
        DefinitCentrePts.ToolTipText = "Définit le point avec les coordonnées du centre de l'affichage"
        '
        'EffacePts
        '
        EffacePts.Name = "EffacePts"
        EffacePts.ToolTipText = "Efface le point."
        '
        'MenuPtsSeparator_1
        '
        MenuPtsSeparator_1.Name = "MenuPtsSeparator_1"
        '
        'Va_A_Pts
        '
        Va_A_Pts.Name = "Va_A_Pts"
        Va_A_Pts.ToolTipText = "L'affichage sera centré sur le point."
        '
        'RevientDe_Pts
        '
        RevientDe_Pts.Name = "RevientDe_Pts"
        RevientDe_Pts.ShortcutKeys = Keys.F5
        RevientDe_Pts.Text = "Revenir"
        RevientDe_Pts.ToolTipText = "Recentre l'affichage sur " & CrLf &
                                    "le point de départ d'une action Aller à"
#End Region
#Region "LabelInformation"
        '
        'LabelInformation
        '
        LabelInformation.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        LabelInformation.AutoEllipsis = True
        LabelInformation.Location = New Point(775, 438)
        LabelInformation.Margin = New Padding(0)
        LabelInformation.Name = "LabelInformation" 'a vérifier par rapport à "TuileCurseur"
        LabelInformation.Size = New Size(369, 24)
        LabelInformation.TabIndex = 8
        LabelInformation.TextAlign = ContentAlignment.MiddleLeft
        AideContextuelle.SetToolTip(LabelInformation, resources.GetString("LabelInformation.ToolTip"))
#End Region
#Region "MenuApp"
        '
        'LabelMenuApp
        '
        LabelMenuApp.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        LabelMenuApp.ContextMenuStrip = ContextMenuApp
        LabelMenuApp.FlatStyle = FlatStyle.Flat
        LabelMenuApp.Font = New Font("Wingdings", 20.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        LabelMenuApp.Location = New Point(1144, 438)
        LabelMenuApp.Margin = New Padding(0)
        LabelMenuApp.Name = "LabelMenuApp"
        LabelMenuApp.Padding = New Padding(3, 2, 0, 0)
        LabelMenuApp.Size = New Size(40, 24)
        LabelMenuApp.TabIndex = 7
        LabelMenuApp.Text = "žžžž"
        LabelMenuApp.TextAlign = ContentAlignment.MiddleCenter
        AideContextuelle.SetToolTip(LabelMenuApp, "Cliquez pour ouvrir le menu de l'application")
        '
        'ContextMenuApp
        '
        ContextMenuApp.Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        ContextMenuApp.Items.AddRange(New ToolStripItem() {ConfigureApp, TesteServeur, CompacteCacheTuiles,
                                                           MenuAppSeparator_0, SaisieSurfaceTraitementApp, LanceCreationCarte, SupprimeTuiles,
                                                           MenuAppSeparator_1, Va_A_App, Va_A_Site, RevientDe_App, LimitesSite,
                                                           MenuAppSeparator_2, APropos, Aide, ImprimeCarte,
                                                           MenuAppSeparator_3, FermeApp})
        ContextMenuApp.Name = "ContextMenuApp"
        ContextMenuApp.ShowImageMargin = False
        ContextMenuApp.Width = 50
        ContextMenuApp.Tag = LabelMenuApp
        '
        'ConfigureApp
        '
        ConfigureApp.Name = "ConfigureApp"
        ConfigureApp.ShortcutKeys = Keys.F3
        ConfigureApp.Text = "Configuration ..."
        ConfigureApp.ToolTipText = "Ouvre le formulaire de configuration"
        '
        'TesteServeur
        '
        TesteServeur.Name = "TesteServeur"
        TesteServeur.ShortcutKeys = Keys.Alt Or Keys.F3
        TesteServeur.Text = "Tester Serveur Carto"
        TesteServeur.ToolTipText = "Teste si le serveur carto en cours répond." & CrLf &
                                   "Si le résultat est Ok un fichier des capacités du serveur est généré."
        '
        'CompacteCacheTuiles
        '
        CompacteCacheTuiles.Name = "CompacteCacheTuiles"
        CompacteCacheTuiles.Text = "Compacter Cache"
        CompacteCacheTuiles.ToolTipText = resources.GetString("CompacteCacheTuiles.ToolTip")
        '
        'MenuAppSeparator_0
        '
        MenuAppSeparator_0.Name = "MenuAppSeparator_0"
        '
        'SaisieSurfaceTraitementApp
        '
        SaisieSurfaceTraitementApp.Name = "SaisieSurfaceTraitementApp"
        SaisieSurfaceTraitementApp.ShortcutKeys = Keys.F7
        SaisieSurfaceTraitementApp.Tag = "App"
        SaisieSurfaceTraitementApp.Text = "Surface Traitement ..."
        SaisieSurfaceTraitementApp.ToolTipText = resources.GetString("SaisieSurfaceTraitementApp.ToolTip")
        '
        'LanceCreationCarte
        '
        LanceCreationCarte.Name = "LanceCreationCarte"
        LanceCreationCarte.ShortcutKeys = Keys.F2
        LanceCreationCarte.Text = "Lancer Création ..."
        LanceCreationCarte.ToolTipText = "Ouvre le formulaire Préférences." & CrLf &
                                         "A minima il faut indiquer le nom de la carte à créer."
        '
        'SupprimerTuiles
        '
        SupprimeTuiles.Name = "SupprimeTuiles"
        SupprimeTuiles.ShortcutKeys = Keys.Alt Or Keys.F2
        SupprimeTuiles.Text = "Supprimer Tuiles"
        SupprimeTuiles.ToolTipText = "Supprime du cache les tuiles sélectionnées." & CrLf &
                                     "Cela permet de les re-télécharger suite à une mises à jour sur le serveur."
        '
        'MenuAppSeparator_1
        '
        MenuAppSeparator_1.Name = "MenuAppSeparator_1"
        '
        'Va_A_App
        '
        Va_A_App.Name = "Va_A_App"
        Va_A_App.ShortcutKeys = Keys.F6
        Va_A_App.Text = "Aller à ..."
        Va_A_App.ToolTipText = "Ouvre le formulaire de saisie de coordonnées." & CrLf &
                               "L'affichage sera centrer sur les coordonnées indiquées."
        'Va_A_Site
        '
        Va_A_Site.Name = "Va_A_Site"
        Va_A_Site.ShortcutKeys = Keys.Alt Or Keys.F6
        Va_A_Site.Text = "Aller à Centre du site"
        Va_A_Site.ToolTipText = "Recentre l'affichage sur" & CrLf &
                                "le centre du site de capture."
        '
        'RevientDe_App
        '
        RevientDe_App.Name = "RevientDe_App"
        RevientDe_App.ShortcutKeys = Keys.F5
        RevientDe_App.Text = "Revenir"
        RevientDe_App.ToolTipText = "Recentre l'affichage sur " & CrLf &
                                    "le point de départ d'une action Aller à"
        '
        'LimitesSite
        '
        LimitesSite.Name = "LimitesSite"
        LimitesSite.ShortcutKeys = Keys.Alt Or Keys.F11
        LimitesSite.Text = "Limites site : Off"
        LimitesSite.ToolTipText = "Autorise ou non le dépassement" & CrLf &
                                  "des limites du site de capture"
        '
        'MenuAppSeparator_2
        '
        MenuAppSeparator_2.Name = "MenuAppSeparator_2"
        '
        'APropos
        '
        APropos.Name = "APropos"
        APropos.ShortcutKeys = Keys.F4
        APropos.Text = "A Propos ..."
        APropos.ToolTipText = "Ouvre un formulaire qui affiche des informations sur l'application."
        '
        'Aide
        '
        Aide.Name = "Aide"
        Aide.ShortcutKeys = Keys.F1
        Aide.Text = "Aide ..."
        Aide.ToolTipText = "Ouvre un formulaire qui donne une aide générale sur l'application" & CrLf &
                           "et en particulier sur la zone d'affichage."
        '
        'ImprimeCarte
        '
        ImprimeCarte.Name = "ImprimeCarte"
        ImprimeCarte.ShortcutKeys = Keys.Alt Or Keys.F1
        ImprimeCarte.Text = "Imprimer Carte ..."
        ImprimeCarte.ToolTipText = "Ouvre un formulaire qui permet d'imprimer" & CrLf &
                                   "la carte associée à un fichier Georef"
        '
        'MenuAppSeparator_3
        '
        MenuAppSeparator_3.Name = "MenuAppSeparator_3"
        '
        'FermeApp
        '
        FermeApp.Name = "FermeApp"
        FermeApp.ShortcutKeyDisplayString = ""
        FermeApp.ShortcutKeys = Keys.Alt Or Keys.F4
        FermeApp.Text = "Quitter"
        FermeApp.ToolTipText = "Sort de l'application en enregistrant les paramètres :" & CrLf &
                               "Site, DomTom, Echelle, Unité de coordonnées, Points de téléchargement" & CrLf &
                               "et coordonnées du centre de l'affichage."
#End Region
#End Region
#Region "Configuration des contrôles hors IU"
        '
        'Afficheur
        '
        RafraichisseurVisue.Interval = 500
        '
        'ConnectedInternet
        '
        ConnexionInternet.Interval = 10000
        '
        'ToolTip1
        '
        AideContextuelle.AutoPopDelay = 5000
        AideContextuelle.InitialDelay = 1000
        AideContextuelle.ReshowDelay = 100
        AideContextuelle.UseAnimation = False
        AideContextuelle.UseFading = False
        AideContextuelle.IsBalloon = False
        '
        'SupportControlesIU
        '
        SupportControlesIU.Controls.Add(LabelConnexionInternet)
        SupportControlesIU.Controls.Add(LabelNbDemandesTuiles)
        SupportControlesIU.Controls.Add(LabelAltitude)
        SupportControlesIU.Controls.Add(LabelInformation)
        'les labels menus ont des context menus associés
        SupportControlesIU.Controls.Add(LabelTrace)
        SupportControlesIU.Controls.Add(LabelMenuPt1)
        SupportControlesIU.Controls.Add(LabelMenuPt2)
        SupportControlesIU.Controls.Add(LabelMenuApp)
        'les labels liste recouvrent leur liste de choix associées (combobox)
        SupportControlesIU.Controls.Add(LabelSites)
        SupportControlesIU.Controls.Add(ListeChoixSites)
        SupportControlesIU.Controls.Add(LabelEchelles)
        SupportControlesIU.Controls.Add(ListeChoixEchelles)
        SupportControlesIU.Controls.Add(LabelCoordonnees)
        SupportControlesIU.Controls.Add(ListeChoixTypeCoordonnees)
        'obligatoirement ajouter en dernier dans la collection pour que les évenements associés soient traités en premier
        SupportControlesIU.Controls.Add(SurfaceAffichageTuile)
        SupportControlesIU.Dock = DockStyle.Fill
        SupportControlesIU.Location = New Point(0, 0)
        SupportControlesIU.Name = "SupportControl"
        SupportControlesIU.Size = New Size(1184, 462)
        SupportControlesIU.TabIndex = 11
#End Region
#Region "Configuration du formulaire"
        '
        'Capturer
        '
        AutoScaleMode = AutoScaleMode.None
        BackColor = Color.FromArgb(227, 227, 227)
        ClientSize = New Size(1184, 462)
        Controls.Add(SupportControlesIU)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        KeyPreview = True
        MinimumSize = New Size(1200, 501)
        Name = "Capturer"
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.CenterScreen
        'Contrôles conteneurs
        CType(SurfaceAffichageTuile, ISupportInitialize).EndInit()
        ContextMenuVisue.ResumeLayout(False)
        ContextMenuApp.ResumeLayout(False)
        ContextMenuPts.ResumeLayout(False)
        ContextMenuTrace.ResumeLayout(False)
        SupportControlesIU.ResumeLayout(False)
        ResumeLayout(False)
#End Region
    End Sub
#Region "Declaration des Contrôles privées"
    Private AideContextuelle As ToolTip
    Private RafraichisseurVisue As Windows.Forms.Timer
    Private ConnexionInternet As Windows.Forms.Timer
    Private SupportControlesIU As Panel

    Private SurfaceAffichageTuile As PictureBox
    Private ContextMenuVisue As ContextMenuStrip
    Private DefinitPt1 As ToolStripMenuItem
    Private DefinitPt2 As ToolStripMenuItem
    Private MenuVisueSeparateur_0 As ToolStripSeparator
    Private AfficheDessinEchelle As ToolStripMenuItem
    Private AfficheCouchePentes As ToolStripMenuItem
    Private ChoisitCoefAlphaPentes As ToolStripComboBox
    Private MenuVisueSeparateur_1 As ToolStripSeparator
    Private ChoisitFondsPlan As ToolStripComboBox
    Private MenuVisueSeparateur_2 As ToolStripSeparator
    Private ChoisitSitesOSM As ToolStripComboBox

    Private LabelConnexionInternet As Label
    Private LabelNbDemandesTuiles As Label

    Private LabelSites As Label
    Private ListeChoixSites As ComboBox

    Private LabelEchelles As Label
    Private ListeChoixEchelles As ComboBox

    Private LabelCoordonnees As Label
    Private ListeChoixTypeCoordonnees As ComboBox

    Private LabelAltitude As Label

    Private LabelTrace As Label
    Private ContextMenuTrace As ContextMenuStrip
    Private EditeTrace As ToolStripMenuItem
    Private FermeTrace As ToolStripMenuItem
    Private ProprietesTrace As ToolStripMenuItem
    Private MenuTraceSeparator_0 As ToolStripSeparator
    Private Va_A_DebutTrace As ToolStripMenuItem
    Private Va_A_FinTrace As ToolStripMenuItem
    Private RevientDe_MenuTrace As ToolStripMenuItem
    Private MenuTraceSeparator_1 As ToolStripSeparator
    Private OuvreTrace As ToolStripMenuItem

    Private LabelMenuPt1 As Label
    Private LabelMenuPt2 As Label
    Private ContextMenuPts As ContextMenuStrip
    Private SaisieSurfaceTraitementPts As ToolStripMenuItem
    Private MenuPtsSeparator_0 As ToolStripSeparator
    Private DefinitCentrePts As ToolStripMenuItem
    Private EffacePts As ToolStripMenuItem
    Private MenuPtsSeparator_1 As ToolStripSeparator
    Private Va_A_Pts As ToolStripMenuItem
    Private RevientDe_Pts As ToolStripMenuItem

    Private LabelInformation As Label

    Private LabelMenuApp As Label
    Private ContextMenuApp As ContextMenuStrip
    Private ConfigureApp As ToolStripMenuItem
    Private TesteServeur As ToolStripMenuItem
    Private CompacteCacheTuiles As ToolStripMenuItem
    Private MenuAppSeparator_0 As ToolStripSeparator
    Private SaisieSurfaceTraitementApp As ToolStripMenuItem
    Private LanceCreationCarte As ToolStripMenuItem
    Private SupprimeTuiles As ToolStripMenuItem
    Private MenuAppSeparator_1 As ToolStripSeparator
    Private Va_A_App As ToolStripMenuItem
    Private RevientDe_App As ToolStripMenuItem
    Private Va_A_Site As ToolStripMenuItem
    Private LimitesSite As ToolStripMenuItem
    Private MenuAppSeparator_2 As ToolStripSeparator
    Private APropos As ToolStripMenuItem
    Private Aide As ToolStripMenuItem
    Private ImprimeCarte As ToolStripMenuItem
    Private MenuAppSeparator_3 As ToolStripSeparator
    Private FermeApp As ToolStripMenuItem
#End Region
End Class