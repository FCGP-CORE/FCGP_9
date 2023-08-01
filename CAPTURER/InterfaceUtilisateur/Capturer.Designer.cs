using static FCGP.Commun;

namespace FCGP
{
    internal partial class Capturer : Form
    {

        internal Capturer()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        /// <summary> Initialise l'ensemble des évenement liés au formulaire car aucune variable avec le modificateur WithEvents 
    /// rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
        private void InitialiserEvenements()
        {
            #region SurfaceAffichageTuile
            // SurfaceAffichageTuile
            SurfaceAffichageTuile.Paint += SurfaceAffichageTuile_Paint;
            SurfaceAffichageTuile.MouseClick += SurfaceAffichageTuile_MouseClick;
            SurfaceAffichageTuile.MouseDown += SurfaceAffichageTuile_MouseDown;
            SurfaceAffichageTuile.MouseMove += SurfaceAffichageTuile_MouseMove;
            SurfaceAffichageTuile.MouseUp += SurfaceAffichageTuile_MouseUp;
            SurfaceAffichageTuile.MouseWheel += SurfaceAffichageTuile_MouseWheel;
            SurfaceAffichageTuile.Resize += SurfaceAffichageTuile_Resize;
            // RafraichisseurVisue : permet la MAJ de la suface d'affichage toute les 0.5s quand des tuiles sont téléchargées
            RafraichisseurVisue.Tick += RafraichisseurVisue_Tick;
            // ContextMenu SurfaceAffichageTuile
            // DefinitPt1
            DefinitPt1.Click += DefinitPts_Click;
            // DefinitPt2
            DefinitPt2.Click += DefinitPts_Click;
            // AfficheDessinEchelle
            AfficheDessinEchelle.Click += AfficheDessinEchelle_Click;
            // AfficheCouchePentes
            AfficheCouchePentes.Click += AfficheCouchePentes_Click;
            // ChoisitAlphaPentes
            ChoisitCoefAlphaPentes.SelectedIndexChanged += ChoisitCoefAlphaPentes_SelectedIndexChanged;
            // choisitFondPlan
            ChoisitFondsPlan.SelectedIndexChanged += ChoisitFondPlan_SelectedIndexChanged;
            // ChoisitSitesOSM
            ChoisitSitesOSM.SelectedIndexChanged += ChoisitSitesOSM_SelectedIndexChanged;
            #endregion
            #region Liste Sites
            // LabelSites
            LabelSites.Click += LabelListes_Click;
            LabelSites.MouseEnter += LabelListesMenus_MouseEnter;
            LabelSites.MouseLeave += LabelListesMenus_MouseLeave;
            LabelSites.MouseMove += LabelListesMenus_MouseMove;
            // ListeChoixSites 
            ListeChoixSites.SelectedIndexChanged += ListeChoixSites_SelectedIndexChanged;
            ListeChoixSites.DropDownClosed += ListesChoix_DropDownClosed;
            #endregion
            #region Liste Echelles
            // LabelChoixEchelles
            LabelEchelles.Click += LabelListes_Click;
            LabelEchelles.MouseEnter += LabelListesMenus_MouseEnter;
            LabelEchelles.MouseLeave += LabelListesMenus_MouseLeave;
            LabelEchelles.MouseMove += LabelListesMenus_MouseMove;
            // ListeEchelles
            ListeChoixEchelles.SelectedIndexChanged += ListeChoixEchelles_SelectedIndexChanged;
            ListeChoixEchelles.DropDownClosed += ListesChoix_DropDownClosed;
            #endregion
            #region Liste Coordonnées
            // LabelCoordonnees
            LabelCoordonnees.Click += LabelListes_Click;
            LabelCoordonnees.MouseEnter += LabelListesMenus_MouseEnter;
            LabelCoordonnees.MouseLeave += LabelListesMenus_MouseLeave;
            LabelCoordonnees.MouseMove += LabelListesMenus_MouseMove;
            // ListeChoixCoordonnees
            ListeChoixTypeCoordonnees.SelectedIndexChanged += ListeChoixTypeCoordonnees_SelectedIndexChanged;
            ListeChoixTypeCoordonnees.DropDownClosed += ListesChoix_DropDownClosed;
            #endregion
            #region MenuTraces
            // LabelTrace
            LabelTrace.Click += LabelMenus_Click;
            LabelTrace.MouseEnter += LabelListesMenus_MouseEnter;
            LabelTrace.MouseLeave += LabelListesMenus_MouseLeave;
            LabelTrace.MouseMove += LabelListesMenus_MouseMove;
            // ContextMenuTrace
            ContextMenuTrace.Closed += ContextMenus_Closed;
            ContextMenuTrace.Opening += ContextMenus_Opening;
            // EditeTrace
            EditeTrace.Click += EditeTrace_Click;
            // FermeTrace
            FermeTrace.Click += FermeTrace_Click;
            // ProprietesTrace
            ProprietesTrace.Click += ProprietesTrace_Click;
            // Va_A_DebutTrace
            Va_A_DebutTrace.Click += Va_A_DebutTrace_Click;
            // Va_A_FinTrace
            Va_A_FinTrace.Click += Va_A_FinTrace_Click;
            // RevientDe
            RevientDe_MenuTrace.Click += RevientDe_Click;
            // OuvreTrace
            OuvreTrace.Click += OuvreTrace_Click;
            #endregion
            #region MenuPoints
            // LabelMenuPt1
            LabelMenuPt1.Click += LabelMenus_Click;
            LabelMenuPt1.MouseEnter += LabelListesMenus_MouseEnter;
            LabelMenuPt1.MouseLeave += LabelListesMenus_MouseLeave;
            LabelMenuPt1.MouseMove += LabelListesMenus_MouseMove;
            // LabelMenuPt2
            LabelMenuPt2.Click += LabelMenus_Click;
            LabelMenuPt2.MouseEnter += LabelListesMenus_MouseEnter;
            LabelMenuPt2.MouseLeave += LabelListesMenus_MouseLeave;
            LabelMenuPt2.MouseMove += LabelListesMenus_MouseMove;
            // ContextMenuPts
            ContextMenuPts.Closed += ContextMenus_Closed;
            ContextMenuPts.Opening += ContextMenus_Opening;
            // SaisieSurfaceCapturePts
            SaisieSurfaceTraitementPts.Click += SaisieSurfacesTraitements_Click;
            // DefinitCentrePts
            DefinitCentrePts.Click += DefinitCentrePts_Click;
            // EffacePts
            EffacePts.Click += EffacePts_Click;
            // Va_A_Pts
            Va_A_Pts.Click += Va_A_Pts_Click;
            // RevientDe_Pts
            RevientDe_Pts.Click += RevientDe_Click;
            #endregion
            #region MenuApplication
            // LabelMenuApp
            LabelMenuApp.Click += LabelMenus_Click;
            LabelMenuApp.MouseEnter += LabelListesMenus_MouseEnter;
            LabelMenuApp.MouseLeave += LabelListesMenus_MouseLeave;
            LabelMenuApp.MouseMove += LabelListesMenus_MouseMove;
            // ContextMenuApp
            ContextMenuApp.Closed += ContextMenus_Closed;
            ContextMenuApp.Opening += ContextMenus_Opening;
            // ConfigureApp
            ConfigureApp.Click += ConfigureApp_Click;
            // TesteServeur
            TesteServeur.Click += TesteServeur_Click;
            // CompacteCacheTuiles
            CompacteCacheTuiles.Click += CompacteCacheTuiles_Click;
            // SupprimeTuiles
            SupprimeTuiles.Click += SupprimeTuiles_Click;
            // LanceCreationCarte
            LanceCreationCarte.Click += LanceCreationCarte_Click;
            // Va_A_App
            Va_A_App.Click += Va_A_App_Click;
            // RevientDe_App
            RevientDe_App.Click += RevientDe_Click;
            // LimitesSite
            LimitesSite.Click += LimitesSite_Click;
            // Va_A_App
            Va_A_Site.Click += Va_A_Site_Click;
            // SaisieSurfaceTraitementApp
            SaisieSurfaceTraitementApp.Click += SaisieSurfacesTraitements_Click;
            // APropos
            APropos.Click += APropos_Click;
            // Aide
            Aide.Click += Aide_Click;
            // ImprimerCarte
            ImprimeCarte.Click += ImprimerCarte_Click;
            // FermeApp
            FermeApp.Click += FermeApp_Click;
            #endregion
            #region Zones Informations
            // Internet
            // ConnectedInternet
            ConnexionInternet.Tick += ConnectedInternet_Tick;
            // LabelInternet
            LabelConnexionInternet.MouseMove += Information_MouseMove;
            // NbDemandesTuiles
            LabelNbDemandesTuiles.MouseMove += Information_MouseMove;
            // LabelAltitude
            LabelAltitude.MouseMove += Information_MouseMove;
            // TuileCurseur
            LabelInformation.MouseMove += Information_MouseMove;
            #endregion
            #region Formulaire
            FormClosing += Capturer_FormClosing;
            FormClosed += Capturer_FormClosed;
            Load += Capturer_Load;
            LocationChanged += Capturer_LocationChanged;
            KeyDown += Capturer_KeyDown;
            KeyUp += Capturer_KeyUp;
            PreviewKeyDown += Capturer_PreviewKeyDown;
            #endregion
        }
        // Form remplace la méthode Dispose pour nettoyer la liste des composants.
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Requise par le Concepteur Windows Form
        private IContainer components;

        // REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        // Elle peut être modifiée à l'aide du Concepteur Windows Form.  
        // Ne la modifiez pas à l'aide de l'éditeur de code.
        private void InitializeComponent()
        {
            #region création des contrôles Hors IU
            components = new Container();
            var resources = new ComponentResourceManager(typeof(Capturer));
            // attention plusieurs timer sont accessibles à ce niveau
            RafraichisseurVisue = new System.Windows.Forms.Timer(components);
            ConnexionInternet = new System.Windows.Forms.Timer(components);
            AideContextuelle = new ToolTip(components);
            SupportControlesIU = new Panel();
            #endregion
            #region création des contrôles IU
            SurfaceAffichageTuile = new PictureBox();
            ContextMenuVisue = new ContextMenuStrip(components);
            DefinitPt1 = new ToolStripMenuItem();
            DefinitPt2 = new ToolStripMenuItem();
            MenuVisueSeparateur_0 = new ToolStripSeparator();
            AfficheDessinEchelle = new ToolStripMenuItem();
            AfficheCouchePentes = new ToolStripMenuItem();
            ChoisitCoefAlphaPentes = new ToolStripComboBox();
            MenuVisueSeparateur_1 = new ToolStripSeparator();
            ChoisitFondsPlan = new ToolStripComboBox();
            MenuVisueSeparateur_2 = new ToolStripSeparator();
            ChoisitSitesOSM = new ToolStripComboBox();

            LabelConnexionInternet = new Label();
            LabelNbDemandesTuiles = new Label();

            LabelSites = new Label();
            ListeChoixSites = new ComboBox();

            LabelCoordonnees = new Label();
            ListeChoixTypeCoordonnees = new ComboBox();

            LabelEchelles = new Label();
            ListeChoixEchelles = new ComboBox();

            LabelAltitude = new Label();

            LabelTrace = new Label();
            ContextMenuTrace = new ContextMenuStrip(components);
            EditeTrace = new ToolStripMenuItem();
            FermeTrace = new ToolStripMenuItem();
            ProprietesTrace = new ToolStripMenuItem();
            MenuTraceSeparator_0 = new ToolStripSeparator();
            Va_A_DebutTrace = new ToolStripMenuItem();
            Va_A_FinTrace = new ToolStripMenuItem();
            RevientDe_MenuTrace = new ToolStripMenuItem();
            MenuTraceSeparator_1 = new ToolStripSeparator();
            OuvreTrace = new ToolStripMenuItem();

            LabelMenuPt1 = new Label();
            LabelMenuPt2 = new Label();
            ContextMenuPts = new ContextMenuStrip(components);
            SaisieSurfaceTraitementPts = new ToolStripMenuItem();
            MenuPtsSeparator_0 = new ToolStripSeparator();
            DefinitCentrePts = new ToolStripMenuItem();
            EffacePts = new ToolStripMenuItem();
            MenuPtsSeparator_1 = new ToolStripSeparator();
            Va_A_Pts = new ToolStripMenuItem();
            RevientDe_Pts = new ToolStripMenuItem();

            LabelInformation = new Label();

            LabelMenuApp = new Label();
            ContextMenuApp = new ContextMenuStrip(components);
            ConfigureApp = new ToolStripMenuItem();
            TesteServeur = new ToolStripMenuItem();
            CompacteCacheTuiles = new ToolStripMenuItem();
            MenuAppSeparator_0 = new ToolStripSeparator();
            SaisieSurfaceTraitementApp = new ToolStripMenuItem();
            LanceCreationCarte = new ToolStripMenuItem();
            SupprimeTuiles = new ToolStripMenuItem();
            MenuAppSeparator_1 = new ToolStripSeparator();
            Va_A_App = new ToolStripMenuItem();
            RevientDe_App = new ToolStripMenuItem();
            Va_A_Site = new ToolStripMenuItem();
            LimitesSite = new ToolStripMenuItem();
            MenuAppSeparator_2 = new ToolStripSeparator();
            APropos = new ToolStripMenuItem();
            Aide = new ToolStripMenuItem();
            ImprimeCarte = new ToolStripMenuItem();
            MenuAppSeparator_3 = new ToolStripSeparator();
            FermeApp = new ToolStripMenuItem();

            // Contrôles conteneurs
            ((ISupportInitialize)SurfaceAffichageTuile).BeginInit();
            ContextMenuVisue.SuspendLayout();
            ContextMenuApp.SuspendLayout();
            ContextMenuPts.SuspendLayout();
            ContextMenuTrace.SuspendLayout();
            SupportControlesIU.SuspendLayout();
            SuspendLayout();
            #endregion
            #region Configuration de éléments IU
            #region Visue
            // 
            // SurfaceAffichageTuile
            // 
            SurfaceAffichageTuile.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SurfaceAffichageTuile.ContextMenuStrip = ContextMenuVisue;
            SurfaceAffichageTuile.Location = new Point(0, 0);
            SurfaceAffichageTuile.Margin = new Padding(0);
            SurfaceAffichageTuile.Name = "SurfaceAffichageTuile";
            SurfaceAffichageTuile.Size = new Size(1184, 438);
            SurfaceAffichageTuile.TabIndex = 0;
            SurfaceAffichageTuile.TabStop = false;
            // 
            // MenuVisue
            // 
            ContextMenuVisue.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ContextMenuVisue.Items.AddRange(new ToolStripItem[] { DefinitPt1, DefinitPt2, 
                                                                  MenuVisueSeparateur_0, AfficheDessinEchelle, AfficheCouchePentes, ChoisitCoefAlphaPentes, 
                                                                  MenuVisueSeparateur_1, ChoisitFondsPlan, 
                                                                  MenuVisueSeparateur_2, ChoisitSitesOSM });
            ContextMenuVisue.Name = "ContextMenuVisue";
            ContextMenuVisue.ShowImageMargin = false;
            // 
            // DefinitPt1
            // 
            DefinitPt1.Name = "DefinirPt1";
            DefinitPt1.ShowShortcutKeys = false;
            DefinitPt1.Tag = "Pt1";
            DefinitPt1.Text = "Définir Pt1";
            DefinitPt1.ToolTipText = "Définit le point avec les coordonnées du pointeur de souris.";
            // 
            // DefinitPt2
            // 
            DefinitPt2.Name = "DefinirPt2";
            DefinitPt2.ShowShortcutKeys = false;
            DefinitPt2.Tag = "Pt2";
            DefinitPt2.Text = "Définir Pt2";
            DefinitPt2.ToolTipText = "Définit le point avec les coordonnées du pointeur de souris.";
            // 
            // MenuVisueSeparateur_0
            // 
            MenuVisueSeparateur_0.Name = "MenuVisueSeparateur_0";
            // 
            // AfficheDessinEchelle
            // 
            AfficheDessinEchelle.Name = "AfficheDessinEchelle";
            AfficheDessinEchelle.ShowShortcutKeys = false;
            AfficheDessinEchelle.ShortcutKeys = Keys.F12;
            AfficheDessinEchelle.Text = "AfficherEchelle";
            AfficheDessinEchelle.ToolTipText = resources.GetString("AfficheDessinEchelle.ToolTip");
            // 
            // AfficheCouchePentes
            // 
            AfficheCouchePentes.Name = "AfficheCouchePentes";
            AfficheCouchePentes.ShowShortcutKeys = false;
            AfficheCouchePentes.ShortcutKeys = Keys.Alt | Keys.F12;
            AfficheCouchePentes.Text = "AfficherEchelle";
            AfficheCouchePentes.ToolTipText = resources.GetString("AffichePentes.ToolTip");
            // 
            // ChoisitCoefAlphaPentes
            // 
            ChoisitCoefAlphaPentes.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ChoisitCoefAlphaPentes.Items.AddRange(new string[] { "  15 %", "  33 %", "  50 %", "  66 %", "  80 %", " 100 %" });
            ChoisitCoefAlphaPentes.Name = "ChoisitCoefAlphaPentes";
            ChoisitCoefAlphaPentes.Size = new Size(120, 27);
            ChoisitCoefAlphaPentes.ToolTipText = "Permet de choisir le coefficient de transparence" + CrLf + "de la couche des pentes";
            // 
            // MenuVisueSeparateur_1
            // 
            MenuVisueSeparateur_1.Name = "MenuVisueSeparateur_1";
            // 
            // ChoisitFondPlan
            // 
            ChoisitFondsPlan.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ChoisitFondsPlan.Name = "ChoisitFondPlan";
            ChoisitFondsPlan.Size = new Size(120, 27);
            ChoisitFondsPlan.ToolTipText = "Permet de choisir le fond de plan à afficher";
            // 
            // MenuVisueSeparateur_2
            // 
            MenuVisueSeparateur_2.Name = "MenuVisueSeparateur_2";
            // 
            // ChoisitSitesOSM
            // 
            ChoisitSitesOSM.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ChoisitSitesOSM.Name = "ChoisitSitesOSM";
            ChoisitSitesOSM.Size = new Size(120, 27);
            ChoisitSitesOSM.ToolTipText = "Permet de choisir un des 2 sites OSM";
            #endregion
            #region Information Internet
            // 
            // LabelConnexionInternet
            // 
            LabelConnexionInternet.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelConnexionInternet.Font = new Font("HoloLens MDL2 Assets", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            LabelConnexionInternet.Location = new Point(0, 438);
            LabelConnexionInternet.Margin = new Padding(0);
            LabelConnexionInternet.Name = "LabelConnexionInternet";
            LabelConnexionInternet.Size = new Size(25, 24);
            LabelConnexionInternet.TabIndex = 9;
            LabelConnexionInternet.Text = "";
            LabelConnexionInternet.TextAlign = ContentAlignment.MiddleLeft;
            AideContextuelle.SetToolTip(LabelConnexionInternet, "Indique l'état de la connection internet." + CrLf + 
                                                                "Couleur Verte     : Connecté" + CrLf + 
                                                                "Couleur Rouge  : Non Connecté");
            #endregion
            #region Information NbDemandesTuiles
            // 
            // LabelNbDemandesTuiles
            // 
            LabelNbDemandesTuiles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelNbDemandesTuiles.Location = new Point(25, 438);
            LabelNbDemandesTuiles.Margin = new Padding(0);
            LabelNbDemandesTuiles.Name = "LabelNbDemandesTuiles";
            LabelNbDemandesTuiles.Size = new Size(25, 24);
            LabelNbDemandesTuiles.TabIndex = 10;
            LabelNbDemandesTuiles.Text = "0";
            LabelNbDemandesTuiles.TextAlign = ContentAlignment.MiddleCenter;
            AideContextuelle.SetToolTip(LabelNbDemandesTuiles, resources.GetString("LabelNbDemandesTuiles.ToolTip"));
            #endregion
            #region Choix Sites
            // 
            // LabelSites
            // 
            LabelSites.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;            LabelSites.BackColor = Color.Transparent;
            LabelSites.Location = new Point(50, 438);
            LabelSites.Margin = new Padding(0);
            LabelSites.Name = "LabelSites";
            LabelSites.Size = new Size(110, 24);
            LabelSites.TabIndex = 14;
            LabelSites.TextAlign = ContentAlignment.MiddleLeft;
            LabelSites.Tag = ListeChoixSites;
            AideContextuelle.SetToolTip(LabelSites, "Site ou Domtom sélectionné pour la capture." + CrLf + 
                                                    "Cliquez dessus pour ouvrir une liste de choix.");
            // 
            // ListeChoixSites
            // 
            ListeChoixSites.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ListeChoixSites.DropDownStyle = ComboBoxStyle.DropDownList;
            ListeChoixSites.FlatStyle = FlatStyle.System;
            ListeChoixSites.FormattingEnabled = true;
            ListeChoixSites.Location = new Point(50, 438);
            ListeChoixSites.Margin = new Padding(0);
            ListeChoixSites.Name = "ListeChoixSites";
            ListeChoixSites.Size = new Size(105, 0);
            ListeChoixSites.TabIndex = 4;
            ListeChoixSites.TabStop = false;
            ListeChoixSites.Visible = false;
            #endregion
            #region Choix Echelles
            // 
            // LabelEchelles
            // 
            LabelEchelles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelEchelles.Location = new Point(160, 438);
            LabelEchelles.Margin = new Padding(0);
            LabelEchelles.Name = "LabelEchelles";
            LabelEchelles.Size = new Size(90, 24);
            LabelEchelles.TabIndex = 13;
            LabelEchelles.TextAlign = ContentAlignment.MiddleLeft;
            LabelEchelles.Tag = ListeChoixEchelles;
            AideContextuelle.SetToolTip(LabelEchelles, "Echelle sélectionnée pour la capture." + CrLf + 
                                                       "Cliquez dessus pour ouvrir une liste de choix.");
            // 
            // ListeChoixEchelles
            // 
            ListeChoixEchelles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ListeChoixEchelles.DropDownStyle = ComboBoxStyle.DropDownList;
            ListeChoixEchelles.FlatStyle = FlatStyle.System;
            ListeChoixEchelles.FormattingEnabled = true;
            ListeChoixEchelles.Location = new Point(160, 438);
            ListeChoixEchelles.Margin = new Padding(0);
            ListeChoixEchelles.Name = "ListeChoixEchelles";
            ListeChoixEchelles.Size = new Size(85, 0);
            ListeChoixEchelles.TabIndex = 6;
            ListeChoixEchelles.TabStop = false;
            ListeChoixEchelles.Visible = false;
            #endregion
            #region Choix Type Coordonnées
            // 
            // LabelCoordonnees
            // 
            LabelCoordonnees.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelCoordonnees.Location = new Point(250, 438);
            LabelCoordonnees.Margin = new Padding(0);
            LabelCoordonnees.Name = "LabelCoordonnees";
            LabelCoordonnees.Size = new Size(255, 24);
            LabelCoordonnees.TabIndex = 1;
            LabelCoordonnees.TextAlign = ContentAlignment.MiddleLeft;
            LabelCoordonnees.Tag = ListeChoixTypeCoordonnees;
            AideContextuelle.SetToolTip(LabelCoordonnees, resources.GetString("LabelCoordonnees.ToolTip"));
            // 
            // ListeChoixTypeCoordonnees
            // 
            ListeChoixTypeCoordonnees.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ListeChoixTypeCoordonnees.DropDownStyle = ComboBoxStyle.DropDownList;
            ListeChoixTypeCoordonnees.FlatStyle = FlatStyle.System;
            ListeChoixTypeCoordonnees.FormattingEnabled = true;
            ListeChoixTypeCoordonnees.Location = new Point(250, 438);
            ListeChoixTypeCoordonnees.Margin = new Padding(0);
            ListeChoixTypeCoordonnees.Name = "ListeChoixTypeCoordonnees";
            ListeChoixTypeCoordonnees.Size = new Size(140, 0);
            ListeChoixTypeCoordonnees.TabIndex = 0;
            ListeChoixTypeCoordonnees.TabStop = false;
            ListeChoixTypeCoordonnees.Visible = false;
            #endregion
            #region Information Altitude
            // 
            // LabelAltitude
            // 
            LabelAltitude.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelAltitude.Location = new Point(505, 438);
            LabelAltitude.Margin = new Padding(0);
            LabelAltitude.Name = "LabelAltitude";
            LabelAltitude.Size = new Size(60, 24);
            LabelAltitude.TabIndex = 11;
            LabelAltitude.Text = "0 m";
            LabelAltitude.TextAlign = ContentAlignment.MiddleRight;
            AideContextuelle.SetToolTip(LabelAltitude, "Altitude du pointeur de souris");
            #endregion
            #region MenuTrace
            // 
            // LabelTrace
            // 
            LabelTrace.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelTrace.ContextMenuStrip = ContextMenuTrace;
            LabelTrace.Location = new Point(565, 438);
            LabelTrace.Margin = new Padding(0);
            LabelTrace.Name = "LabelTrace";
            LabelTrace.Size = new Size(160, 24);
            LabelTrace.TabIndex = 12;
            LabelTrace.Text = "Pas de trace";
            LabelTrace.TextAlign = ContentAlignment.MiddleLeft;
            AideContextuelle.SetToolTip(LabelTrace, resources.GetString("LabelTrace.ToolTip"));
            // 
            // ContextMenuTrace
            // 
            ContextMenuTrace.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ContextMenuTrace.Items.AddRange(new ToolStripItem[] { EditeTrace, FermeTrace, ProprietesTrace, 
                                                                  MenuTraceSeparator_0, Va_A_DebutTrace, Va_A_FinTrace, RevientDe_MenuTrace, 
                                                                  MenuTraceSeparator_1, OuvreTrace });
            ContextMenuTrace.Name = "ContextMenuTrace";
            ContextMenuTrace.ShowImageMargin = false;
            ContextMenuTrace.Tag = LabelTrace;
            // 
            // EditeTrace
            // 
            EditeTrace.Name = "EditionTrace";
            EditeTrace.ShortcutKeys = Keys.Alt | Keys.E;
            EditeTrace.Text = "&Edition Trace On";
            EditeTrace.ToolTipText = "Bascule du mode édition de la trace." + CrLf + 
                                     "Off : Curseur Grande croix et couleur du nom de la trace vert." + CrLf + 
                                     "On : Curseur Réticule et couleur du nom de la trace Orange.";
            // 
            // FermeTrace
            // 
            FermeTrace.Name = "FermerTrace";
            FermeTrace.ShortcutKeys = Keys.Alt | Keys.F;
            FermeTrace.Text = "&Femer Trace ...";
            FermeTrace.ToolTipText = "Ouvre le formulaire de fermeture d'une trace.";
            // 
            // ProprietesTrace
            // 
            ProprietesTrace.Name = "ProprietesTrace";
            ProprietesTrace.ShortcutKeys = Keys.Alt | Keys.P;
            ProprietesTrace.Text = "&Propriété Trace ...";
            ProprietesTrace.ToolTipText = "Ouvre le formulaire des propriétés de la trace.";
            // 
            // MenuTraceSeparator_0
            // 
            MenuTraceSeparator_0.Name = "MenuTraceSeparator_0";
            // 
            // Va_A_DebutTrace
            // 
            Va_A_DebutTrace.Name = "DebutTrace";
            Va_A_DebutTrace.ShortcutKeys = Keys.Alt | Keys.D;
            Va_A_DebutTrace.Text = "Aller à &Début Trace";
            Va_A_DebutTrace.ToolTipText = "Positionne le centre de l'affichage" + CrLf + 
                                          "sur le 1er point de la trace.";
            // 
            // Va_A_FinTrace
            // 
            Va_A_FinTrace.Name = "Va_A_FinTrace";
            Va_A_FinTrace.ShortcutKeys = Keys.Alt | Keys.A;
            Va_A_FinTrace.Text = "&Aller à Fin Trace";
            Va_A_FinTrace.ToolTipText = "Positionne le centre de l'affichage" + CrLf + 
                                        "sur le dernier point de la trace.";
            // 
            // RevientDe_MenuTrace
            // 
            RevientDe_MenuTrace.Name = "RevientDe_MenuTrace";
            RevientDe_MenuTrace.ShortcutKeys = Keys.F5;
            RevientDe_MenuTrace.Text = "Revenir";
            // 
            // MenuTraceSeparator_1
            // 
            MenuTraceSeparator_1.Name = "MenuTraceSeparator_1";
            // 
            // OuvreTrace
            // 
            OuvreTrace.Name = "OuvreTrace";
            OuvreTrace.ShortcutKeys = Keys.F10;
            OuvreTrace.Text = "Ouvrir une Trace ...";
            OuvreTrace.ToolTipText = "Ouvre le formulaire d'ouverture d'une trace.";
            #endregion
            #region MenuPoints
            // 
            // LabelMenuPt1
            // 
            LabelMenuPt1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelMenuPt1.ContextMenuStrip = ContextMenuPts;
            LabelMenuPt1.FlatStyle = FlatStyle.Flat;
            LabelMenuPt1.Font = new Font("Wingdings", 23.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            LabelMenuPt1.Location = new Point(725, 438);
            LabelMenuPt1.Margin = new Padding(0);
            LabelMenuPt1.Name = "LabelMenuPt1";
            LabelMenuPt1.Padding = new Padding(2, 0, 0, 0);
            LabelMenuPt1.Size = new Size(25, 24);
            LabelMenuPt1.TabIndex = 2;
            LabelMenuPt1.Tag = "Pt1";
            LabelMenuPt1.Text = "";
            LabelMenuPt1.TextAlign = ContentAlignment.MiddleCenter;
            AideContextuelle.SetToolTip(LabelMenuPt1, "1er point définissant la surface de traitement." + CrLf + 
                                        resources.GetString("LabelMenuPts.ToolTip"));
            // 
            // LabelMenuPt2
            // 
            LabelMenuPt2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelMenuPt2.ContextMenuStrip = ContextMenuPts;
            LabelMenuPt2.FlatStyle = FlatStyle.Flat;
            LabelMenuPt2.Font = new Font("Wingdings", 23.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            LabelMenuPt2.Location = new Point(750, 438);
            LabelMenuPt2.Margin = new Padding(0);
            LabelMenuPt2.Name = "LabelMenuPt2";
            LabelMenuPt2.Padding = new Padding(2, 0, 0, 0);
            LabelMenuPt2.Size = new Size(25, 24);
            LabelMenuPt2.TabIndex = 3;
            LabelMenuPt2.Tag = "Pt2";
            LabelMenuPt2.Text = "";
            LabelMenuPt2.TextAlign = ContentAlignment.MiddleCenter;
            AideContextuelle.SetToolTip(LabelMenuPt2, "2ème point définissant la surface de traitement." + CrLf + 
                                        resources.GetString("LabelMenuPts.ToolTip"));
            // 
            // ContextMenuPts
            // 
            ContextMenuPts.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ContextMenuPts.Items.AddRange(new ToolStripItem[] { SaisieSurfaceTraitementPts, 
                                                                MenuPtsSeparator_0, DefinitCentrePts, EffacePts, 
                                                                MenuPtsSeparator_1, Va_A_Pts, RevientDe_Pts });
            ContextMenuPts.Name = "ContextMenuPts";
            ContextMenuPts.ShowImageMargin = false;
            // 
            // SaisieSurfaceTraitementPts
            // 
            SaisieSurfaceTraitementPts.Name = "SaisieSurfaceTraitementPts";
            SaisieSurfaceTraitementPts.Tag = "Pts";
            SaisieSurfaceTraitementPts.Text = "Surface de Traitement ...";
            SaisieSurfaceTraitementPts.ToolTipText = resources.GetString("SaisieSurfaceTraitementPts.ToolTip");
            // 
            // MenuPtsSeparator_0
            // 
            MenuPtsSeparator_0.Name = "MenuPtsSeparator_0";
            // 
            // DefinitCentrePts
            // 
            DefinitCentrePts.Name = "DefinitCentrePts";
            DefinitCentrePts.ToolTipText = "Définit le point avec les coordonnées du centre de l'affichage";
            // 
            // EffacePts
            // 
            EffacePts.Name = "EffacePts";
            EffacePts.ToolTipText = "Efface le point.";
            // 
            // MenuPtsSeparator_1
            // 
            MenuPtsSeparator_1.Name = "MenuPtsSeparator_1";
            // 
            // Va_A_Pts
            // 
            Va_A_Pts.Name = "Va_A_Pts";
            Va_A_Pts.ToolTipText = "L'affichage sera centré sur le point.";
            // 
            // RevientDe_Pts
            // 
            RevientDe_Pts.Name = "RevientDe_Pts";
            RevientDe_Pts.ShortcutKeys = Keys.F5;
            RevientDe_Pts.Text = "Revenir";
            RevientDe_Pts.ToolTipText = "Recentre l'affichage sur " + CrLf + 
                                        "le point de départ d'une action Aller à";
            #endregion
            #region LabelInformation
            // 
            // LabelInformation
            // 
            LabelInformation.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LabelInformation.AutoEllipsis = true;
            LabelInformation.Location = new Point(775, 438);
            LabelInformation.Margin = new Padding(0);
            LabelInformation.Name = "LabelInformation"; // a vérifier par rapport à "TuileCurseur"
            LabelInformation.Size = new Size(369, 24);
            LabelInformation.TabIndex = 8;
            LabelInformation.TextAlign = ContentAlignment.MiddleLeft;
            AideContextuelle.SetToolTip(LabelInformation, resources.GetString("LabelInformation.ToolTip"));
            #endregion
            #region MenuApp
            // 
            // LabelMenuApp
            // 
            LabelMenuApp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LabelMenuApp.ContextMenuStrip = ContextMenuApp;
            LabelMenuApp.FlatStyle = FlatStyle.Flat;
            LabelMenuApp.Font = new Font("Wingdings", 20.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            LabelMenuApp.Location = new Point(1144, 438);
            LabelMenuApp.Margin = new Padding(0);
            LabelMenuApp.Name = "LabelMenuApp";
            LabelMenuApp.Padding = new Padding(3, 2, 0, 0);
            LabelMenuApp.Size = new Size(40, 24);
            LabelMenuApp.TabIndex = 7;
            LabelMenuApp.Text = "žžžž";
            LabelMenuApp.TextAlign = ContentAlignment.MiddleCenter;
            AideContextuelle.SetToolTip(LabelMenuApp, "Cliquez pour ouvrir le menu de l'application");
            // 
            // ContextMenuApp
            // 
            ContextMenuApp.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ContextMenuApp.Items.AddRange(new ToolStripItem[] { ConfigureApp, TesteServeur, CompacteCacheTuiles, 
                                                                MenuAppSeparator_0, SaisieSurfaceTraitementApp, LanceCreationCarte, SupprimeTuiles, 
                                                                MenuAppSeparator_1, Va_A_App, Va_A_Site, RevientDe_App, LimitesSite, 
                                                                MenuAppSeparator_2, APropos, Aide, ImprimeCarte, 
                                                                MenuAppSeparator_3, FermeApp });
            ContextMenuApp.Name = "ContextMenuApp";
            ContextMenuApp.ShowImageMargin = false;
            ContextMenuApp.Width = 50;
            ContextMenuApp.Tag = LabelMenuApp;
            // 
            // ConfigureApp
            // 
            ConfigureApp.Name = "ConfigureApp";
            ConfigureApp.ShortcutKeys = Keys.F3;
            ConfigureApp.Text = "Configuration ...";
            ConfigureApp.ToolTipText = "Ouvre le formulaire de configuration";
            // 
            // TesteServeur
            // 
            TesteServeur.Name = "TesteServeur";
            TesteServeur.ShortcutKeys = Keys.Alt | Keys.F3;
            TesteServeur.Text = "Tester Serveur Carto";
            TesteServeur.ToolTipText = "Teste si le serveur carto en cours répond." + CrLf + 
                                       "Si le résultat est Ok un fichier des capacités du serveur est généré.";
            // 
            // CompacteCacheTuiles
            // 
            CompacteCacheTuiles.Name = "CompacteCacheTuiles";
            CompacteCacheTuiles.Text = "Compacter Cache";
            CompacteCacheTuiles.ToolTipText = resources.GetString("CompacteCacheTuiles.ToolTip");
            // 
            // MenuAppSeparator_0
            // 
            MenuAppSeparator_0.Name = "MenuAppSeparator_0";
            // 
            // SaisieSurfaceTraitementApp
            // 
            SaisieSurfaceTraitementApp.Name = "SaisieSurfaceTraitementApp";
            SaisieSurfaceTraitementApp.ShortcutKeys = Keys.F7;
            SaisieSurfaceTraitementApp.Tag = "App";
            SaisieSurfaceTraitementApp.Text = "Surface Traitement ...";
            SaisieSurfaceTraitementApp.ToolTipText = resources.GetString("SaisieSurfaceTraitementApp.ToolTip");
            // 
            // LanceCreationCarte
            // 
            LanceCreationCarte.Name = "LanceCreationCarte";
            LanceCreationCarte.ShortcutKeys = Keys.F2;
            LanceCreationCarte.Text = "Lancer Création ...";
            LanceCreationCarte.ToolTipText = "Ouvre le formulaire Préférences." + CrLf + 
                                             "A minima il faut indiquer le nom de la carte à créer.";
            // 
            // SupprimerTuiles
            // 
            SupprimeTuiles.Name = "SupprimeTuiles";
            SupprimeTuiles.ShortcutKeys = Keys.Alt | Keys.F2;
            SupprimeTuiles.Text = "Supprimer Tuiles";
            SupprimeTuiles.ToolTipText = "Supprime du cache les tuiles sélectionnées." + CrLf + 
                                         "Cela permet de les re-télécharger suite à une mises à jour sur le serveur.";
            // 
            // MenuAppSeparator_1
            // 
            MenuAppSeparator_1.Name = "MenuAppSeparator_1";
            // 
            // Va_A_App
            // 
            Va_A_App.Name = "Va_A_App";
            Va_A_App.ShortcutKeys = Keys.F6;
            Va_A_App.Text = "Aller à ...";
            Va_A_App.ToolTipText = "Ouvre le formulaire de saisie de coordonnées." + CrLf + 
                                   "L'affichage sera centrer sur les coordonnées indiquées.";
            // 
            // Va_A_Site
            // 
            Va_A_Site.Name = "Va_A_Site";
            Va_A_Site.ShortcutKeys = Keys.Alt | Keys.F5;
            Va_A_Site.Text = "Aller au centre du site";
            Va_A_Site.ToolTipText = "Recentre l'affichage sur" + CrLf +
                                    "le centre du site de capture.";
            // 
            // RevientDe_App
            // 
            RevientDe_App.Name = "RevientDe_App";
            RevientDe_App.ShortcutKeys = Keys.F5;
            RevientDe_App.Text = "Revenir";
            RevientDe_App.ToolTipText = "Recentre l'affichage sur " + CrLf + 
                                        "le point de départ d'une action Aller à";
            // 
            // LimitesSite
            // 
            LimitesSite.Name = "LimitesSite";
            LimitesSite.ShortcutKeys = Keys.Alt | Keys.F11;
            LimitesSite.Text = "Limites site : Off";
            LimitesSite.ToolTipText = "Autorise ou non le dépassement" + CrLf + 
                                      "des limites du site de capture";
            // 
            // MenuAppSeparator_2
            // 
            MenuAppSeparator_2.Name = "MenuAppSeparator_2";
            // 
            // APropos
            // 
            APropos.Name = "APropos";
            APropos.ShortcutKeys = Keys.F4;
            APropos.Text = "A Propos ...";
            APropos.ToolTipText = "Ouvre un formulaire qui affiche des informations sur l'application.";
            // 
            // Aide
            // 
            Aide.Name = "Aide";
            Aide.ShortcutKeys = Keys.F1;
            Aide.Text = "Aide ...";
            Aide.ToolTipText = "Ouvre un formulaire qui donne une aide générale sur l'application" + CrLf + 
                               "et en particulier sur la zone d'affichage.";
            // 
            // ImprimeCarte
            // 
            ImprimeCarte.Name = "ImprimeCarte";
            ImprimeCarte.ShortcutKeys = Keys.Alt | Keys.F1;
            ImprimeCarte.Text = "Imprimer Carte ...";
            ImprimeCarte.ToolTipText = "Ouvre un formulaire qui permet d'imprimer" + CrLf + 
                                       "la carte associée à un fichier Georef";
            // 
            // MenuAppSeparator_3
            // 
            MenuAppSeparator_3.Name = "MenuAppSeparator_3";
            // 
            // FermeApp
            // 
            FermeApp.Name = "FermeApp";
            FermeApp.ShortcutKeyDisplayString = "";
            FermeApp.ShortcutKeys = Keys.Alt | Keys.F4;
            FermeApp.Text = "Quitter";
            FermeApp.ToolTipText = "Sort de l'application en enregistrant les paramètres :" + CrLf + 
                                   "Site, DomTom, Echelle, Unité de coordonnées, Points de téléchargement" + CrLf + 
                                   "et coordonnées du centre de l'affichage.";
            #endregion
            #endregion
            #region Configuration des contrôles hors IU
            // 
            // Afficheur
            // 
            RafraichisseurVisue.Interval = 500;
            // 
            // ConnectedInternet
            // 
            ConnexionInternet.Interval = 10000;
            // 
            // ToolTip1
            // 
            AideContextuelle.AutoPopDelay = 5000;
            AideContextuelle.InitialDelay = 1000;
            AideContextuelle.ReshowDelay = 100;
            AideContextuelle.UseAnimation = false;
            AideContextuelle.UseFading = false;
            AideContextuelle.IsBalloon = false;
            // 
            // SupportControlesIU
            // 
            SupportControlesIU.Controls.Add(LabelConnexionInternet);
            SupportControlesIU.Controls.Add(LabelNbDemandesTuiles);
            SupportControlesIU.Controls.Add(LabelAltitude);
            SupportControlesIU.Controls.Add(LabelInformation);
            // les labels menus ont des context menus associés
            SupportControlesIU.Controls.Add(LabelTrace);
            SupportControlesIU.Controls.Add(LabelMenuPt1);
            SupportControlesIU.Controls.Add(LabelMenuPt2);
            SupportControlesIU.Controls.Add(LabelMenuApp);
            //les labels liste recouvrent leur liste de choix associées (combobox)
            SupportControlesIU.Controls.Add(LabelSites);
            SupportControlesIU.Controls.Add(ListeChoixSites);
            SupportControlesIU.Controls.Add(LabelEchelles);
            SupportControlesIU.Controls.Add(ListeChoixEchelles);
            SupportControlesIU.Controls.Add(LabelCoordonnees);
            SupportControlesIU.Controls.Add(ListeChoixTypeCoordonnees);
            // obligatoirement ajouter en dernier dans la collection pour que les évenements associés soient traités en priorité
            SupportControlesIU.Controls.Add(SurfaceAffichageTuile);
            SupportControlesIU.Dock = DockStyle.Fill;
            SupportControlesIU.Location = new Point(0, 0);
            SupportControlesIU.Name = "SupportControl";
            SupportControlesIU.Size = new Size(1184, 462);
            SupportControlesIU.TabIndex = 11;
            #endregion
            #region Configuration du formulaire
            // 
            // Capturer
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(227, 227, 227);
            ClientSize = new Size(1184, 462);
            Controls.Add(SupportControlesIU);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MinimumSize = new Size(1200, 501);
            Name = "Capturer";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            // Contrôles conteneurs
            ((ISupportInitialize)SurfaceAffichageTuile).EndInit();
            ContextMenuVisue.ResumeLayout(false);
            ContextMenuApp.ResumeLayout(false);
            ContextMenuPts.ResumeLayout(false);
            ContextMenuTrace.ResumeLayout(false);
            SupportControlesIU.ResumeLayout(false);
            ResumeLayout(false);
            #endregion
        }
        #region Declaration des Contrôles privées
        private ToolTip AideContextuelle;
        private System.Windows.Forms.Timer RafraichisseurVisue;
        private System.Windows.Forms.Timer ConnexionInternet;
        private Panel SupportControlesIU;

        private PictureBox SurfaceAffichageTuile;
        private ContextMenuStrip ContextMenuVisue;
        private ToolStripMenuItem DefinitPt1;
        private ToolStripMenuItem DefinitPt2;
        private ToolStripSeparator MenuVisueSeparateur_0;
        private ToolStripMenuItem AfficheDessinEchelle;
        private ToolStripMenuItem AfficheCouchePentes;
        private ToolStripComboBox ChoisitCoefAlphaPentes;
        private ToolStripSeparator MenuVisueSeparateur_1;
        private ToolStripComboBox ChoisitFondsPlan;
        private ToolStripSeparator MenuVisueSeparateur_2;
        private ToolStripComboBox ChoisitSitesOSM;

        private Label LabelConnexionInternet;
        private Label LabelNbDemandesTuiles;

        private Label LabelSites;
        private ComboBox ListeChoixSites;

        private Label LabelEchelles;
        private ComboBox ListeChoixEchelles;

        private Label LabelCoordonnees;
        private ComboBox ListeChoixTypeCoordonnees;

        private Label LabelAltitude;

        private Label LabelTrace;
        private ContextMenuStrip ContextMenuTrace;
        private ToolStripMenuItem EditeTrace;
        private ToolStripMenuItem FermeTrace;
        private ToolStripMenuItem ProprietesTrace;
        private ToolStripSeparator MenuTraceSeparator_0;
        private ToolStripMenuItem Va_A_DebutTrace;
        private ToolStripMenuItem Va_A_FinTrace;
        private ToolStripMenuItem RevientDe_MenuTrace;
        private ToolStripSeparator MenuTraceSeparator_1;
        private ToolStripMenuItem OuvreTrace;

        private Label LabelMenuPt1;
        private Label LabelMenuPt2;
        private ContextMenuStrip ContextMenuPts;
        private ToolStripMenuItem SaisieSurfaceTraitementPts;
        private ToolStripSeparator MenuPtsSeparator_0;
        private ToolStripMenuItem DefinitCentrePts;
        private ToolStripMenuItem EffacePts;
        private ToolStripSeparator MenuPtsSeparator_1;
        private ToolStripMenuItem Va_A_Pts;
        private ToolStripMenuItem RevientDe_Pts;

        private Label LabelInformation;

        private Label LabelMenuApp;
        private ContextMenuStrip ContextMenuApp;
        private ToolStripMenuItem ConfigureApp;
        private ToolStripMenuItem TesteServeur;
        private ToolStripMenuItem CompacteCacheTuiles;
        private ToolStripSeparator MenuAppSeparator_0;
        private ToolStripMenuItem SaisieSurfaceTraitementApp;
        private ToolStripMenuItem LanceCreationCarte;
        private ToolStripMenuItem SupprimeTuiles;
        private ToolStripSeparator MenuAppSeparator_1;
        private ToolStripMenuItem Va_A_App;
        private ToolStripMenuItem RevientDe_App;
        private ToolStripMenuItem Va_A_Site;
        private ToolStripMenuItem LimitesSite;
        private ToolStripSeparator MenuAppSeparator_2;
        private ToolStripMenuItem APropos;
        private ToolStripMenuItem Aide;
        private ToolStripMenuItem ImprimeCarte;
        private ToolStripSeparator MenuAppSeparator_3;
        private ToolStripMenuItem FermeApp;
        #endregion
    }
}