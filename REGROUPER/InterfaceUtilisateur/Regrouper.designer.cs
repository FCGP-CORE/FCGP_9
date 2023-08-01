using static FCGP.Commun;
using static FCGP.Proprietes.Resources;

namespace FCGP
{
    internal partial class Regrouper : Form
    {

        internal Regrouper()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // LabelCoordonnees
            LabelCoordonnees.Click += LabelListes_Click;
            LabelCoordonnees.MouseEnter += LabelListesMenus_MouseEnter;
            LabelCoordonnees.MouseLeave += LabelListesMenus_MouseLeave;
            // LabelDimensions
            LabelDimensions.Click += LabelListes_Click;
            LabelDimensions.MouseEnter += LabelListesMenus_MouseEnter;
            LabelDimensions.MouseLeave += LabelListesMenus_MouseLeave;
            // ListeChoixCoordonnees
            ListeChoixTypeCoordonnees.SelectedIndexChanged += ListeChoixTypeCoordonnees_SelectedIndexChanged;
            // ListeChoixDimensions
            ListeChoixDimensions.SelectedIndexChanged += ListeChoixDimensions_SelectedIndexChanged;
            // BarreIconeMode : IconeDeplacerCarte
            IconeDeplacerCarte.Click += IconeDeplacerCarte_Click;
            // BarreIconeGestionRegroupement : IconeOuvrirRegroupement, IconeAfficherPlanRegroupement, IconeAjouterCartesRegroupement 
            IconeOuvrirRegroupement.Click += OuvirRegroupement_Click;
            IconeAfficherPlanRegroupement.Click += AfficherPlanRegroupement_Click;
            IconeAjouterCartesRegroupement.Click += AjouterCartesRegroupement_Click;
            // BarreIconeListeRegroupement : IconeListeRegroupement
            IconeListeRegroupement.SelectedIndexChanged += IconeListeEchelle_SelectedIndexChanged;
            // BarreIconeZoom : IconeZoomMoins, IconeZoom, IconeZoomPlus
            IconeZoomMoins.Click += ZoomMoins_Click;
            IconeZoom.Click += IconeZoom_Click;
            IconeZoomPlus.Click += ZoomPlus_Click;
            // menu Programme : MenuGestionRegroupements, MenuFichiersTuiles, MenuConfiguration, MenuAide
            // MenuGestionRegroupements : MenuOuvrirRegroupement, MenuAjouterRegroupement, MenuSupprimerRegroupement, MenuQuitter
            MenuOuvrirRegroupement.Click += OuvirRegroupement_Click;
            MenuAjouterRegroupement.Click += AjouterCartesRegroupement_Click;
            // MenuSupprimerRegroupement : SuppressionListeRegroupement
            SuppressionListeRegroupement.SelectedIndexChanged += SuppressionListeRegroupement_SelectedIndexChanged;
            MenuQuitter.Click += MenuQuitter_Click;
            // MenuFichiersTuiles : MenuCreerFichiers, MenuModifierOrdreAffichage
            MenuCreerFichiersJNX_ORUX.Click += MenuCreerFichiersJNX_ORUX_Click;
            MenuCreerFichiersKMZ.Click += MenuCreerFichiersKMZ_Click;
            // MenuModifierOrdreAffichage : FichiersJNXToolStripMenuItem, FichiersORUXToolStripMenuItem
            MenuModifierFichierKMZ.Click += MenuModifierFichierKMZ_Click;
            FichiersJNXToolStripMenuItem.Click += MenuModifierFichiersJNX_Click;
            FichiersORUXToolStripMenuItem.Click += MenuModifierFichiersORUX_Click;
            // MenuConfiguration : MenuConfigurationRegrouper
            MenuConfigurationRegrouper.Click += MenuConfigurationRegrouper_Click;
            // MenuAide : MenuAideRegrouper, MenuAPropos
            MenuAideRegrouper.Click += MenuAideRegrouper_Click;
            MenuAPropos.Click += MenuAPropos_Click;
            // cmsZoomMenu : Zoom_0500, Zoom_0660, Zoom_0750, Zoom_0875, Zoom_1000, Zoom_1125, Zoom_1250, Zoom_1500, Zoom_1750, Zoom_2000
            Zoom_0500.Click += ZoomValeur_Click;
            Zoom_0660.Click += ZoomValeur_Click;
            Zoom_0750.Click += ZoomValeur_Click;
            Zoom_0875.Click += ZoomValeur_Click;
            Zoom_1000.Click += ZoomValeur_Click;
            Zoom_1125.Click += ZoomValeur_Click;
            Zoom_1250.Click += ZoomValeur_Click;
            Zoom_1500.Click += ZoomValeur_Click;
            Zoom_1750.Click += ZoomValeur_Click;
            Zoom_2000.Click += ZoomValeur_Click;
            // AffichageCarte
            AffichageCarte.Paint += AffichageCarte_Paint;
            AffichageCarte.MouseClick += AffichageCarte_MouseClick;
            AffichageCarte.MouseDown += AffichageCarte_MouseDown;
            AffichageCarte.MouseMove += AffichageCarte_MouseMove;
            AffichageCarte.MouseUp += AffichageCarte_MouseUp;
            AffichageCarte.MouseWheel += AffichageCarte_MouseWheel;
            AffichageCarte.PreviewKeyDown += AffichageCarte_PreviewKeyDown;
            AffichageCarte.Resize += AffichageCarte_Resize;
            // MenuCxtVisualisationCarte : ContextMenuListeRegroupement, ToolStripSeparator2, ContextMenuAfficherPlanRegroupement, SupprimerSelectionToolStripMenuItem
            ContextMenuListeRegroupement.SelectedIndexChanged += ContextMenuListeRegroupement_SelectedIndexChanged;
            ContextMenuAfficherPlanRegroupement.Click += AfficherPlanRegroupement_Click;
            SupprimerSelectionToolStripMenuItem.Click += SupprimerSelectionToolStripMenuItem_Click;
            // formulaire
            FormClosed += Regrouper_FormClosed;
            Load += Regrouper_Load;
            Shown += Regrouper_Shown;
            LocationChanged += Regrouper_LocationChanged;
            KeyDown += Regrouper_KeyDown;
            KeyUp += Regrouper_KeyUp;
        }

        // Form overrides dispose to clean up the component list.
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

        // Required by the Windows Form Designer
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        private void InitializeComponent()
        {
            #region instanciation
            components = new Container();
            var resources = new ComponentResourceManager(typeof(Regrouper));
            IconeOuvrirRegroupement = new ToolStripButton();
            IconeAfficherPlanRegroupement = new ToolStripButton();
            IconeAjouterCartesRegroupement = new ToolStripButton();
            MenuProgramme = new MenuStrip();
            MenuGestionRegroupements = new ToolStripMenuItem();
            MenuOuvrirRegroupement = new ToolStripMenuItem();
            MenuAjouterRegroupement = new ToolStripMenuItem();
            MenuSupprimerRegroupement = new ToolStripMenuItem();
            SuppressionListeRegroupement = new ToolStripComboBox();
            ToolStripSeparator5 = new ToolStripSeparator();
            MenuQuitter = new ToolStripMenuItem();
            MenuFichiersTuiles = new ToolStripMenuItem();
            MenuCreerFichiersJNX_ORUX = new ToolStripMenuItem();
            MenuCreerFichiersKMZ = new ToolStripMenuItem();
            ToolStripSeparator4 = new ToolStripSeparator();
            MenuModifierIndicesAffichage = new ToolStripMenuItem();
            MenuModifierFichierKMZ = new ToolStripMenuItem();
            FichiersJNXToolStripMenuItem = new ToolStripMenuItem();
            FichiersORUXToolStripMenuItem = new ToolStripMenuItem();
            MenuConfiguration = new ToolStripMenuItem();
            MenuConfigurationRegrouper = new ToolStripMenuItem();
            MenuAide = new ToolStripMenuItem();
            MenuAideRegrouper = new ToolStripMenuItem();
            MenuAPropos = new ToolStripMenuItem();
            cmsZoomMenu = new ContextMenuStrip(components);
            Zoom_0500 = new ToolStripMenuItem();
            Zoom_0660 = new ToolStripMenuItem();
            Zoom_0750 = new ToolStripMenuItem();
            Zoom_0875 = new ToolStripMenuItem();
            Zoom_1000 = new ToolStripMenuItem();
            Zoom_1125 = new ToolStripMenuItem();
            Zoom_1250 = new ToolStripMenuItem();
            Zoom_1500 = new ToolStripMenuItem();
            Zoom_1750 = new ToolStripMenuItem();
            Zoom_2000 = new ToolStripMenuItem();
            IconeZoom = new ToolStripSplitButton();
            IconeZoomMoins = new ToolStripButton();
            IconeZoomPlus = new ToolStripButton();
            BarreIconeZoom = new ToolStrip();
            OuvrirFichier = new OpenFileDialog();
            AffichageCarte = new PictureBox();
            MenuCxtVisualisationCarte = new ContextMenuStrip(components);
            ContextMenuListeRegroupement = new ToolStripComboBox();
            ToolStripSeparator2 = new ToolStripSeparator();
            ContextMenuAfficherPlanRegroupement = new ToolStripMenuItem();
            SupprimerSelectionToolStripMenuItem = new ToolStripMenuItem();
            BarreIconeGestionRegroupement = new ToolStrip();
            ToolStripSeparator1 = new ToolStripSeparator();
            BarreIconeMode = new ToolStrip();
            IconeDeplacerCarte = new ToolStripButton();
            BarreIconeListeRegroupement = new ToolStrip();
            ToolStripSeparator6 = new ToolStripSeparator();
            IconeListeRegroupement = new ToolStripComboBox();
            ToolStripSeparator7 = new ToolStripSeparator();
            Regroupement = new Label();
            LabelCoordonnees = new Label();
            ListeChoixTypeCoordonnees = new ComboBox();
            Altitude = new Label();
            LZoom = new Label();
            LModeEncours = new Label();
            LOutilEnCours = new Label();
            ListeChoixDimensions = new ComboBox();
            LabelDimensions = new Label();
            ZoneSelection = new Label();
            MenuProgramme.SuspendLayout();
            cmsZoomMenu.SuspendLayout();
            ((ISupportInitialize)AffichageCarte).BeginInit();
            MenuCxtVisualisationCarte.SuspendLayout();
            BarreIconeZoom.SuspendLayout();
            BarreIconeGestionRegroupement.SuspendLayout();
            BarreIconeMode.SuspendLayout();
            BarreIconeListeRegroupement.SuspendLayout();
            SuspendLayout();
            #endregion
            #region Menus programme
            // 
            // MenuProgramme
            // 
            MenuProgramme.AutoSize = false;
            MenuProgramme.BackColor = SystemColors.Control;
            MenuProgramme.Dock = DockStyle.None;
            MenuProgramme.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            MenuProgramme.Items.AddRange(new ToolStripItem[] { MenuGestionRegroupements, MenuFichiersTuiles, MenuConfiguration, MenuAide });
            MenuProgramme.Location = new Point(1, 0);
            MenuProgramme.Name = "MenuProgramme";
            MenuProgramme.Padding = new Padding(0);
            MenuProgramme.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            MenuProgramme.Size = new Size(920, 26);
            MenuProgramme.TabIndex = 0;
            MenuProgramme.Text = "Fichier";
            // 
            // MenuGestionEchelles
            // 
            MenuGestionRegroupements.DropDownItems.AddRange(new ToolStripItem[] { MenuOuvrirRegroupement, MenuAjouterRegroupement, MenuSupprimerRegroupement, ToolStripSeparator5, MenuQuitter });
            MenuGestionRegroupements.Name = "MenuGestionRegroupements";
            MenuGestionRegroupements.ShortcutKeyDisplayString = "";
            MenuGestionRegroupements.ShowShortcutKeys = false;
            MenuGestionRegroupements.Size = new Size(134, 26);
            MenuGestionRegroupements.Text = "Gestion Regroupements";
            MenuGestionRegroupements.ToolTipText = "Gestion des regroupements (niveaux de détails)";
            // 
            // MenuOuvrirEchelle
            // 
            MenuOuvrirRegroupement.Image = CreerRegroupement;
            MenuOuvrirRegroupement.Name = "MenuOuvrirRegroupement";
            MenuOuvrirRegroupement.RightToLeftAutoMirrorImage = true;
            MenuOuvrirRegroupement.ShortcutKeyDisplayString = "";
            MenuOuvrirRegroupement.Size = new Size(249, 26);
            MenuOuvrirRegroupement.Text = "Ouvrir un regroupement ...";
            MenuOuvrirRegroupement.ToolTipText = "Ouvrir un regroupement à partir d'une carte existante (georef) et importer " + CrLf + 
                                                 "l'ensemble des cartes non interpolées situées dans le même répertoire" + CrLf + 
                                                 "ayant le même site et la même échelle de capture.";
            // 
            // MenuAjouterEchelle
            // 
            MenuAjouterRegroupement.Image = AjouterCartesRegroupement;
            MenuAjouterRegroupement.Name = "MenuAjouterRegroupement";
            MenuAjouterRegroupement.Size = new Size(249, 26);
            MenuAjouterRegroupement.Text = "Ajouter des cartes ...";
            MenuAjouterRegroupement.ToolTipText = "Ajouter les cartes non interpolées d'un autre répertoire" + CrLf + 
                                                  "ayant le même site et la même échelle de capture que le regroupement affiché.";
            // 
            // MenuSupprimerEchelle
            // 
            MenuSupprimerRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Text;
            MenuSupprimerRegroupement.DropDownItems.AddRange(new ToolStripItem[] { SuppressionListeRegroupement });
            MenuSupprimerRegroupement.Name = "MenuSupprimerRegroupement";
            MenuSupprimerRegroupement.Size = new Size(249, 26);
            MenuSupprimerRegroupement.Text = "Supprimer un regroupement ...";
            MenuSupprimerRegroupement.ToolTipText = "Choisir le regroupement à supprimer à partir d'une liste.";
            // 
            // MenuListeEchelle
            // 
            SuppressionListeRegroupement.DropDownStyle = ComboBoxStyle.DropDownList;
            SuppressionListeRegroupement.FlatStyle = FlatStyle.System;
            SuppressionListeRegroupement.Name = "MenuListeRegroupement";
            SuppressionListeRegroupement.Size = new Size(121, 23);
            // 
            // ToolStripSeparator5
            // 
            ToolStripSeparator5.Name = "ToolStripSeparator5";
            ToolStripSeparator5.Size = new Size(246, 6);
            // 
            // MenuQuitter
            // 
            MenuQuitter.Name = "MenuQuitter";
            MenuQuitter.ShortcutKeys = Keys.Alt | Keys.F4;
            MenuQuitter.Size = new Size(249, 26);
            MenuQuitter.Text = "Quitter";
            MenuQuitter.ToolTipText = "Quitter FCGP_Visue";
            // 
            // MenuFichiersTuiles
            // 
            MenuFichiersTuiles.DropDownItems.AddRange(new ToolStripItem[] { MenuCreerFichiersJNX_ORUX, MenuCreerFichiersKMZ, ToolStripSeparator4, 
                                                                            MenuModifierFichierKMZ, MenuModifierIndicesAffichage });
            MenuFichiersTuiles.Name = "MenuFichiersTuiles";
            MenuFichiersTuiles.ShowShortcutKeys = false;
            MenuFichiersTuiles.Size = new Size(116, 26);
            MenuFichiersTuiles.Text = "Fichiers tuiles";
            MenuFichiersTuiles.ToolTipText = "Gestion des fichiers tuiles";
            // 
            // MenuCreerFichiersJNX_ORUX
            // 
            MenuCreerFichiersJNX_ORUX.Name = "MenuCreerFichiersJNX_ORUX";
            MenuCreerFichiersJNX_ORUX.Size = new Size(271, 26);
            MenuCreerFichiersJNX_ORUX.Text = "Créer fichier ORUX / JNX ...";
            MenuCreerFichiersJNX_ORUX.ToolTipText = "Ouvre le formulaire de création" + CrLf + "des fichiers ORUX ou JNX";
            // 
            // MenuCreerFichiersKMZ
            // 
            MenuCreerFichiersKMZ.Name = "MenuCreerFichiersJNX_ORUX";
            MenuCreerFichiersKMZ.Size = new Size(271, 26);
            MenuCreerFichiersKMZ.Text = "Créer fichier KMZ ...";
            MenuCreerFichiersKMZ.ToolTipText = "Ouvre le formulaire de création" + CrLf + "des fichiers KMZ";
            // 
            // ToolStripSeparator4
            // 
            ToolStripSeparator4.Name = "ToolStripSeparator4";
            ToolStripSeparator4.Size = new Size(268, 6);
            // 
            // MenuModifierIndicesAffichage
            // 
            MenuModifierIndicesAffichage.DropDownItems.AddRange(new ToolStripItem[] { FichiersJNXToolStripMenuItem, FichiersORUXToolStripMenuItem });
            MenuModifierIndicesAffichage.Name = "MenuModifierIndicesAffichage";
            MenuModifierIndicesAffichage.Size = new Size(271, 26);
            MenuModifierIndicesAffichage.Text = "Modifier indices d'affichage";
            MenuModifierIndicesAffichage.ToolTipText = "Permet la modification des indices d'affichages" + CrLf + 
                                                       "des niveaux de détails des fichiers tuiles";
            // 
            // MenuModifierFichierKMZ
            // 
            MenuModifierFichierKMZ.Name = "MenuModifierFichierKMZ";
            MenuModifierFichierKMZ.Size = new Size(271, 26);
            MenuModifierFichierKMZ.Text = "Modifier fichier KMZ ...";
            MenuModifierFichierKMZ.ToolTipText = "Permet la modification d'un fichier KMZ";
            // 
            // FichiersJNXToolStripMenuItem
            // 
            FichiersJNXToolStripMenuItem.Name = "FichiersJNXToolStripMenuItem";
            FichiersJNXToolStripMenuItem.Size = new Size(192, 26);
            FichiersJNXToolStripMenuItem.Text = "Fichiers JNX ...";
            FichiersJNXToolStripMenuItem.ToolTipText = "Concerne les niveaux d'affichage des fichiers JNX";
            // 
            // FichiersORUXToolStripMenuItem
            // 
            FichiersORUXToolStripMenuItem.Name = "FichiersORUXToolStripMenuItem";
            FichiersORUXToolStripMenuItem.Size = new Size(192, 26);
            FichiersORUXToolStripMenuItem.Text = "Fichiers ORUX ...";
            FichiersORUXToolStripMenuItem.ToolTipText = "Concerne les niveaux d'affichage des fichiers ORUX";
            // 
            // MenuConfiguration
            // 
            MenuConfiguration.DropDownItems.AddRange(new ToolStripItem[] { MenuConfigurationRegrouper });
            MenuConfiguration.Name = "MenuConfiguration";
            MenuConfiguration.Size = new Size(118, 26);
            MenuConfiguration.Text = "Configuration";
            // 
            // MenuConfigurationVisue
            // 
            MenuConfigurationRegrouper.Name = "MenuConfigurationRegrouper";
            MenuConfigurationRegrouper.ShortcutKeys = Keys.F3;
            MenuConfigurationRegrouper.Size = new Size(158, 26);
            MenuConfigurationRegrouper.Text = "Regrouper ...";
            MenuConfigurationRegrouper.ToolTipText = "Ouvre le formulaire de configuration de FCGP_REGROUPER";
            // 
            // MenuAide
            // 
            MenuAide.DropDownItems.AddRange(new ToolStripItem[] { MenuAideRegrouper, MenuAPropos });
            MenuAide.Name = "MenuAide";
            MenuAide.Size = new Size(53, 26);
            MenuAide.Text = "Aide";
            // 
            // MenuAideVisue
            // 
            MenuAideRegrouper.Name = "MenuAideRegrouper";
            MenuAideRegrouper.ShortcutKeys = Keys.F1;
            MenuAideRegrouper.Size = new Size(187, 26);
            MenuAideRegrouper.Text = "Regrouper ...";
            MenuAideRegrouper.ToolTipText = "Ouvre le formulaire d'aide de FCGP_REGROUPER";
            // 
            // MenuAPropos
            // 
            MenuAPropos.Name = "MenuAPropos";
            MenuAPropos.ShortcutKeys = Keys.F4;
            MenuAPropos.Size = new Size(187, 26);
            MenuAPropos.Text = "A Propos  ...";
            MenuAPropos.ToolTipText = "Ouvre le formulaire A Propos";
            #endregion
            #region ContexteMenu Carte
            // 
            // MenuCxtVisualisationCarte
            // 
            MenuCxtVisualisationCarte.Items.AddRange(new ToolStripItem[] { ContextMenuListeRegroupement, ToolStripSeparator2, ContextMenuAfficherPlanRegroupement, SupprimerSelectionToolStripMenuItem });
            MenuCxtVisualisationCarte.Name = "MenuContextuelVisue";
            MenuCxtVisualisationCarte.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            MenuCxtVisualisationCarte.RenderMode = ToolStripRenderMode.Professional;
            MenuCxtVisualisationCarte.Size = new Size(241, 81);
            // 
            // ContextMenuListeEchelle
            // 
            ContextMenuListeRegroupement.AutoSize = false;
            ContextMenuListeRegroupement.DropDownStyle = ComboBoxStyle.DropDownList;
            ContextMenuListeRegroupement.Enabled = false;
            ContextMenuListeRegroupement.FlatStyle = FlatStyle.System;
            ContextMenuListeRegroupement.Name = "ContextMenuListeRegroupement";
            ContextMenuListeRegroupement.Size = new Size(180, 23);
            // 
            // ToolStripSeparator2
            // 
            ToolStripSeparator2.Name = "ToolStripSeparator2";
            ToolStripSeparator2.Size = new Size(237, 6);
            // 
            // ContextMenuAfficherPlanEchelle
            // 
            ContextMenuAfficherPlanRegroupement.Image = RegarderPlanRegroupement;
            ContextMenuAfficherPlanRegroupement.Name = "ContextMenuAfficherPlanRegroupement";
            ContextMenuAfficherPlanRegroupement.ShowShortcutKeys = false;
            ContextMenuAfficherPlanRegroupement.Size = new Size(240, 22);
            ContextMenuAfficherPlanRegroupement.Text = "Plan Regroupement";
            // 
            // SupprimerSelectionToolStripMenuItem
            // 
            SupprimerSelectionToolStripMenuItem.Name = "SupprimerSelectionToolStripMenuItem";
            SupprimerSelectionToolStripMenuItem.Size = new Size(240, 22);
            SupprimerSelectionToolStripMenuItem.Text = "Supprimer Selection";
            #endregion
            #region barreIconeMode
            // 
            // BarreIconeMode
            // 
            BarreIconeMode.AllowMerge = false;
            BarreIconeMode.CanOverflow = false;
            BarreIconeMode.Dock = DockStyle.None;
            BarreIconeMode.GripStyle = ToolStripGripStyle.Hidden;
            // BarreIconeMode.ImeMode = ImeMode.Off
            BarreIconeMode.Items.AddRange(new ToolStripItem[] { IconeDeplacerCarte });
            BarreIconeMode.LayoutStyle = ToolStripLayoutStyle.Flow;
            BarreIconeMode.Location = new Point(1, 27);
            BarreIconeMode.Name = "BarreIconeMode";
            BarreIconeMode.RenderMode = ToolStripRenderMode.Professional;
            BarreIconeGestionRegroupement.Size = new Size(36, 36);
            // 
            // IconeDeplacerCarte
            // 
            IconeDeplacerCarte.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeDeplacerCarte.Image = Mode_Deplacement;
            IconeDeplacerCarte.ImageScaling = ToolStripItemImageScaling.None;
            IconeDeplacerCarte.ImageTransparentColor = Color.Magenta;
            IconeDeplacerCarte.Name = "IconeDeplacerCarte";
            IconeDeplacerCarte.Size = new Size(36, 36);
            IconeDeplacerCarte.Tag = "1";
            IconeDeplacerCarte.ToolTipText = "Mode Déplacement carte";
            #endregion
            #region BarreIconeGestionRegroupement
            // 
            // BarreIconeGestionEchelle
            // 
            BarreIconeGestionRegroupement.AllowMerge = false;
            BarreIconeGestionRegroupement.BackColor = SystemColors.Control;
            BarreIconeGestionRegroupement.CanOverflow = false;
            BarreIconeGestionRegroupement.Dock = DockStyle.None;
            BarreIconeGestionRegroupement.GripStyle = ToolStripGripStyle.Hidden;
            BarreIconeGestionRegroupement.ImageScalingSize = new Size(32, 32);
            BarreIconeGestionRegroupement.Items.AddRange(new ToolStripItem[] { ToolStripSeparator1, IconeOuvrirRegroupement, IconeAfficherPlanRegroupement, 
                                                                               IconeAjouterCartesRegroupement });
            BarreIconeGestionRegroupement.LayoutStyle = ToolStripLayoutStyle.Flow;
            BarreIconeGestionRegroupement.Location = new Point(37, 27);
            BarreIconeGestionRegroupement.Name = "BarreIconeGestionRegroupement";
            BarreIconeGestionRegroupement.RenderMode = ToolStripRenderMode.Professional;
            BarreIconeGestionRegroupement.Size = new Size(120, 36);
            BarreIconeGestionRegroupement.TabIndex = 2;
            BarreIconeGestionRegroupement.Text = "BarreIconeEchelle";
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.AutoSize = false;
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new Size(12, 39);
            // 
            // IconeOuvrirEchelle
            // 
            IconeOuvrirRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeOuvrirRegroupement.Image = CreerRegroupement;
            IconeOuvrirRegroupement.ImageTransparentColor = SystemColors.Control;
            IconeOuvrirRegroupement.Name = "IconeOuvrirRegroupement";
            IconeOuvrirRegroupement.Size = new Size(36, 36);
            IconeOuvrirRegroupement.Text = "&Ouvrir Regroupement";
            IconeOuvrirRegroupement.TextImageRelation = TextImageRelation.ImageAboveText;
            IconeOuvrirRegroupement.ToolTipText = "Ouvrir un regroupement à partir d'une carte existante (georef) et importer " + CrLf + 
                                                  "l'ensemble des cartes non interpolées situées dans le même répertoire" + CrLf + 
                                                  "ayant le même site et la même échelle de capture.";
            // 
            // IconeAfficherPlanEchelle
            // 
            IconeAfficherPlanRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeAfficherPlanRegroupement.Enabled = false;
            IconeAfficherPlanRegroupement.Image = RegarderPlanRegroupement;
            IconeAfficherPlanRegroupement.ImageTransparentColor = Color.Magenta;
            IconeAfficherPlanRegroupement.Name = "IconeAfficherPlanRegroupement";
            IconeAfficherPlanRegroupement.Size = new Size(36, 36);
            IconeAfficherPlanRegroupement.Text = "&Afficher Plan";
            IconeAfficherPlanRegroupement.ToolTipText = "Affiche le plan de calepinage du regroupement affiché";
            // IconeAjouterCartesEchelle
            // 
            IconeAjouterCartesRegroupement.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeAjouterCartesRegroupement.Enabled = false;
            IconeAjouterCartesRegroupement.Image = AjouterCartesRegroupement;
            IconeAjouterCartesRegroupement.ImageTransparentColor = Color.Magenta;
            IconeAjouterCartesRegroupement.Name = "IconeAjouterCartesRegroupement";
            IconeAjouterCartesRegroupement.Size = new Size(36, 36);
            IconeAjouterCartesRegroupement.Text = "A&jouter Cartes";
            IconeAjouterCartesRegroupement.ToolTipText = "Ajouter les cartes non interpolées d'un autre répertoire" + CrLf + 
                                                         "ayant le même site et la même échelle de capture que le regroupement affiché.";
            #endregion
            #region Liste regroupement
            // 
            // BarreIconeListeRegroupement
            // 
            BarreIconeListeRegroupement.AllowMerge = false;
            BarreIconeListeRegroupement.CanOverflow = false;
            BarreIconeListeRegroupement.BackColor = SystemColors.Control;
            BarreIconeListeRegroupement.Dock = DockStyle.None;
            // BarreIconeListeRegroupement.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            BarreIconeListeRegroupement.GripStyle = ToolStripGripStyle.Hidden;
            BarreIconeListeRegroupement.Items.AddRange(new ToolStripItem[] { ToolStripSeparator6, IconeListeRegroupement });
            BarreIconeListeRegroupement.Location = new Point(157, 27);
            BarreIconeListeRegroupement.Name = "BarreIconeListeRegroupement";
            BarreIconeListeRegroupement.RenderMode = ToolStripRenderMode.Professional;
            BarreIconeListeRegroupement.Size = new Size(132, 36);
            BarreIconeListeRegroupement.TabIndex = 4;
            // 
            // ToolStripSeparator6
            // 
            ToolStripSeparator6.AutoSize = false;
            ToolStripSeparator6.Name = "ToolStripSeparator6";
            ToolStripSeparator6.Size = new Size(12, 39);
            // 
            // IconeListeEchelle
            // 
            IconeListeRegroupement.AutoSize = false;
            IconeListeRegroupement.BackColor = SystemColors.Control;
            IconeListeRegroupement.DropDownStyle = ComboBoxStyle.DropDownList;
            IconeListeRegroupement.FlatStyle = FlatStyle.Flat;
            IconeListeRegroupement.Font = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            IconeListeRegroupement.Name = "IconeListeRegroupement";
            IconeListeRegroupement.Size = new Size(120, 36);
            IconeListeRegroupement.DropDownWidth = 120;
            IconeListeRegroupement.ToolTipText = "Liste des regroupements ouverts.";

            #endregion
            #region Barre Zoom
            // 
            // BarreIconeZoom
            // 
            BarreIconeZoom.AllowMerge = false;
            BarreIconeZoom.BackColor = SystemColors.Control;
            BarreIconeZoom.CanOverflow = false;
            BarreIconeZoom.Dock = DockStyle.None;
            BarreIconeZoom.Enabled = false;
            BarreIconeZoom.GripStyle = ToolStripGripStyle.Hidden;
            BarreIconeZoom.ImageScalingSize = new Size(32, 32);
            BarreIconeZoom.ImeMode = ImeMode.Off;
            BarreIconeZoom.Items.AddRange(new ToolStripItem[] { ToolStripSeparator7, IconeZoomMoins, IconeZoom, IconeZoomPlus });
            // BarreIconeZoom.LayoutStyle = ToolStripLayoutStyle.Flow
            BarreIconeZoom.Location = new Point(289, 27);
            BarreIconeZoom.Name = "BarreIconeZoom";
            BarreIconeZoom.RenderMode = ToolStripRenderMode.Professional;
            BarreIconeZoom.Size = new Size(134, 36);
            BarreIconeZoom.TabIndex = 3;
            BarreIconeZoom.Text = "BarreIconeZoom";
            // 
            // ToolStripSeparator7
            // 
            ToolStripSeparator7.AutoSize = false;
            ToolStripSeparator7.Name = "ToolStripSeparator7";
            ToolStripSeparator7.Size = new Size(12, 39);
            // 
            // IconeZoomMoins
            // 
            IconeZoomMoins.BackColor = SystemColors.Control;
            IconeZoomMoins.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeZoomMoins.Image = ZOOM_MOINS_COULEUR;
            IconeZoomMoins.ImageTransparentColor = SystemColors.Control;
            IconeZoomMoins.Name = "IconeZoomMoins";
            IconeZoomMoins.Size = new Size(36, 36);
            IconeZoomMoins.Text = "Zoom Moins";
            // 
            // IconeZoom
            // 
            IconeZoom.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeZoom.DropDown = cmsZoomMenu;
            IconeZoom.DropDownButtonWidth = 12;
            IconeZoom.Image = ZOOM_COULEUR;
            IconeZoom.ImageTransparentColor = Color.Magenta;
            IconeZoom.MergeAction = MergeAction.Replace;
            IconeZoom.Name = "IconeZoom";
            IconeZoom.Size = new Size(50, 36);
            IconeZoom.Text = "Valeur";
            IconeZoom.ToolTipText = "Zoom par Valeur";
            #region menuZoom
            // 
            // cmsZoomMenu
            // 
            cmsZoomMenu.Items.AddRange(new ToolStripItem[] { Zoom_0500, Zoom_0660, Zoom_0750, Zoom_0875, Zoom_1000, Zoom_1125, 
                                                             Zoom_1250, Zoom_1500, Zoom_1750, Zoom_2000 });
            cmsZoomMenu.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            cmsZoomMenu.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            cmsZoomMenu.Name = "cmsZoom";
            cmsZoomMenu.Text = "Essai";
            cmsZoomMenu.OwnerItem = IconeZoom;
            cmsZoomMenu.RenderMode = ToolStripRenderMode.Professional;
            cmsZoomMenu.ShowCheckMargin = true;
            cmsZoomMenu.ShowImageMargin = false;
            cmsZoomMenu.Size = new Size(115, 224);
            // 
            // Zoom_0500
            // 
            Zoom_0500.Name = "Zoom_0500";
            Zoom_0500.Size = new Size(114, 22);
            Zoom_0500.Text = "  50 %";
            Zoom_0500.ToolTipText = "Zoom à la valeur de" + CrLf + "50% de la taille normale";
            // 
            // Zoom_0660
            // 
            Zoom_0660.Name = "Zoom_0660";
            Zoom_0660.Size = new Size(114, 22);
            Zoom_0660.Text = "  66 %";
            Zoom_0660.ToolTipText = "Zoom à la valeur de" + CrLf + "66% de la taille normale";
            // 
            // Zoom_0750
            // 
            Zoom_0750.Name = "Zoom_0750";
            Zoom_0750.Size = new Size(114, 22);
            Zoom_0750.Tag = "0.75";
            Zoom_0750.Text = "  75 %";
            Zoom_0750.ToolTipText = "Zoom à la valeur de" + CrLf + "75% de la taille normale";
            // 
            // Zoom_0875
            // 
            Zoom_0875.Name = "Zoom_0875";
            Zoom_0875.Size = new Size(114, 22);
            Zoom_0875.Tag = "0.875";
            Zoom_0875.Text = "  87.5 %";
            Zoom_0875.ToolTipText = "Zoom à la valeur de" + CrLf + "88% de la taille normale";
            // 
            // Zoom_1000
            // 
            Zoom_1000.Checked = true;
            Zoom_1000.CheckState = CheckState.Checked;
            Zoom_1000.Name = "Zoom_1000";
            Zoom_1000.Size = new Size(114, 22);
            Zoom_1000.Tag = "1.0";
            Zoom_1000.Text = "100 %";
            Zoom_1000.ToolTipText = "Zoom à la valeur de" + CrLf + "100% de la taille normale";
            // 
            // Zoom_1125
            // 
            Zoom_1125.Name = "Zoom_1125";
            Zoom_1125.Size = new Size(114, 22);
            Zoom_1125.Tag = "1.125";
            Zoom_1125.Text = "112.5 %";
            Zoom_1125.ToolTipText = "Zoom à la valeur de" + CrLf + "113% de la taille normale";
            // 
            // Zoom_1250
            // 
            Zoom_1250.Name = "Zoom_1250";
            Zoom_1250.Size = new Size(114, 22);
            Zoom_1250.Tag = "1.25";
            Zoom_1250.Text = "125 %";
            Zoom_1250.ToolTipText = "Zoom à la valeur de" + CrLf + "125% de la taille normale";
            // 
            // Zoom_1500
            // 
            Zoom_1500.Name = "Zoom_1500";
            Zoom_1500.Size = new Size(114, 22);
            Zoom_1500.Tag = "1.5";
            Zoom_1500.Text = "150 %";
            Zoom_1500.ToolTipText = "Zoom à la valeur de" + CrLf + "150% de la taille normale";
            // 
            // Zoom_1750
            // 
            Zoom_1750.Name = "Zoom_1750";
            Zoom_1750.Size = new Size(114, 22);
            Zoom_1750.Tag = "1.75";
            Zoom_1750.Text = "175 %";
            Zoom_1750.ToolTipText = "Zoom à la valeur de" + CrLf + "175% de la taille normale";
            // 
            // Zoom_2000
            // 
            Zoom_2000.Name = "Zoom_2000";
            Zoom_2000.Size = new Size(114, 22);
            Zoom_2000.Tag = "2.0";
            Zoom_2000.Text = "200 %";
            Zoom_2000.ToolTipText = "Zoom à la valeur de" + CrLf + "200% de la taille normale";
            #endregion
            // 
            // IconeZoomPlus
            // 
            IconeZoomPlus.DisplayStyle = ToolStripItemDisplayStyle.Image;
            IconeZoomPlus.Image = ZOOM_PLUS_COULEUR;
            IconeZoomPlus.ImageTransparentColor = Color.Magenta;
            IconeZoomPlus.Name = "IconeZoomPlus";
            IconeZoomPlus.Size = new Size(36, 36);
            IconeZoomPlus.Text = "Zoom Plus";
            #endregion
            #region Statut
            // 
            // AffichageCarte
            // 
            AffichageCarte.BackColor = SystemColors.ActiveCaption;
            AffichageCarte.BackgroundImageLayout = ImageLayout.None;
            AffichageCarte.BorderStyle = BorderStyle.FixedSingle;
            AffichageCarte.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            AffichageCarte.Dock = DockStyle.None;
            AffichageCarte.Size = new Size(922, 560);
            AffichageCarte.Location = new Point(-1, 65);
            AffichageCarte.Margin = new Padding(0);
            AffichageCarte.Name = "ImageCarte";
            AffichageCarte.TabIndex = 8;
            AffichageCarte.TabStop = false;
            // 
            // Regroupement
            // 
            Regroupement.BorderStyle = BorderStyle.FixedSingle;
            Regroupement.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            Regroupement.TextAlign = ContentAlignment.MiddleCenter;
            Regroupement.AutoSize = false;
            Regroupement.Name = "Regroupement";
            Regroupement.Location = new Point(-1, 624);
            Regroupement.Size = new Size(200, 27);
            // 
            // LZoom
            // 
            LZoom.BorderStyle = BorderStyle.FixedSingle;
            LZoom.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            LZoom.TextAlign = ContentAlignment.MiddleCenter;
            LZoom.Name = "LZoom";
            LZoom.AutoSize = false;
            LZoom.Location = new Point(198, 624);
            LZoom.Size = new Size(100, 27);
            // 
            // LabelCoordonnees
            // 
            LabelCoordonnees.BorderStyle = BorderStyle.FixedSingle;
            LabelCoordonnees.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            LabelCoordonnees.TextAlign = ContentAlignment.MiddleCenter;
            LabelCoordonnees.AutoSize = false;
            LabelCoordonnees.Tag = ListeChoixTypeCoordonnees;
            LabelCoordonnees.Name = "LabelCoordonnees";
            LabelCoordonnees.Location = new Point(297, 624);
            LabelCoordonnees.Size = new Size(255, 27);
            // 
            // ListeChoixTypeCoordonnees
            // 
            ListeChoixTypeCoordonnees.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ListeChoixTypeCoordonnees.FormattingEnabled = true;
            ListeChoixTypeCoordonnees.Location = new Point(297, 624);
            ListeChoixTypeCoordonnees.Margin = new Padding(0);
            ListeChoixTypeCoordonnees.Name = "ListeChoixTypeCoordonnees";
            ListeChoixTypeCoordonnees.Size = new Size(140, 0);
            ListeChoixTypeCoordonnees.Tag = LabelCoordonnees;
            ListeChoixTypeCoordonnees.TabIndex = 0;
            ListeChoixTypeCoordonnees.TabStop = false;
            ListeChoixTypeCoordonnees.Visible = false;
            // 
            // Altitude
            // 
            Altitude.BorderStyle = BorderStyle.FixedSingle;
            Altitude.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            Altitude.TextAlign = ContentAlignment.MiddleCenter;
            Altitude.Name = "LZoom";
            Altitude.AutoSize = false;
            Altitude.Location = new Point(551, 624);
            Altitude.Size = new Size(75, 27);
            // 
            // LabelDimensions
            // 
            LabelDimensions.BorderStyle = BorderStyle.FixedSingle;
            LabelDimensions.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            LabelDimensions.TextAlign = ContentAlignment.MiddleCenter;
            LabelDimensions.AutoSize = false;
            LabelDimensions.Name = "LabelDimensions";
            LabelDimensions.Tag = ListeChoixDimensions;
            LabelDimensions.Location = new Point(625, 624);
            LabelDimensions.Size = new Size(75, 27);
            // 
            // ListeChoixDimensions
            // 
            ListeChoixDimensions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ListeChoixDimensions.FormattingEnabled = true;
            ListeChoixDimensions.Items.AddRange(new string[] { "OruxJnx", "Kmz" });
            ListeChoixDimensions.Location = new Point(625, 624);
            ListeChoixDimensions.Margin = new Padding(0);
            ListeChoixDimensions.Name = "ListeChoixDimensions";
            ListeChoixDimensions.Size = new Size(75, 0);
            ListeChoixDimensions.Tag = LabelDimensions;
            ListeChoixDimensions.TabIndex = 0;
            ListeChoixDimensions.TabStop = false;
            ListeChoixDimensions.Visible = false;
            // 
            // ZoneSelection
            // 
            ZoneSelection.BorderStyle = BorderStyle.FixedSingle;
            ZoneSelection.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            ZoneSelection.TextAlign = ContentAlignment.MiddleCenter;
            ZoneSelection.AutoSize = false;
            ZoneSelection.Name = "ZoneSelection";
            ZoneSelection.Location = new Point(699, 624);
            ZoneSelection.Size = new Size(300, 27);
            // 
            // LModeEncours
            // 
            LModeEncours.BorderStyle = BorderStyle.FixedSingle;
            LModeEncours.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            LModeEncours.TextAlign = ContentAlignment.MiddleCenter;
            LModeEncours.Name = "LZoom";
            LModeEncours.AutoSize = false;
            LModeEncours.Location = new Point(622, 624);
            LModeEncours.Size = new Size(150, 27);
            // 
            // LOutilEnCours
            // 
            LOutilEnCours.BorderStyle = BorderStyle.FixedSingle;
            LOutilEnCours.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            LOutilEnCours.TextAlign = ContentAlignment.MiddleCenter;
            LOutilEnCours.Name = "LZoom";
            LOutilEnCours.AutoSize = false;
            LOutilEnCours.Location = new Point(771, 624);
            LOutilEnCours.Size = new Size(150, 27);
            #endregion
            #region Formulaire
            // 
            // OuvrirFichier
            // 
            OuvrirFichier.Filter = "GEOREF Files|*.GEOREF";
            // 
            // Regrouper
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ClientSize = new Size(920, 650);
            Controls.Add(Regroupement);
            Controls.Add(ListeChoixTypeCoordonnees);
            Controls.Add(LabelCoordonnees);
            Controls.Add(ListeChoixDimensions);
            Controls.Add(LabelDimensions);
            Controls.Add(Altitude);
            Controls.Add(LZoom);
            Controls.Add(ZoneSelection);
            Controls.Add(LModeEncours);
            Controls.Add(LOutilEnCours);
            Controls.Add(AffichageCarte);
            Controls.Add(MenuProgramme);
            Controls.Add(BarreIconeMode);
            Controls.Add(BarreIconeGestionRegroupement);
            Controls.Add(BarreIconeListeRegroupement);
            Controls.Add(BarreIconeZoom);
            Cursor = Cursors.Default;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Location = new Point(1980, 0);
            Margin = new Padding(2);
            MaximumSize = new Size(1920, 1200);
            MinimumSize = new Size(800, 600);
            Name = "Regrouper";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            MenuProgramme.ResumeLayout(false);
            MenuProgramme.PerformLayout();
            cmsZoomMenu.ResumeLayout(false);
            ((ISupportInitialize)AffichageCarte).EndInit();
            MenuCxtVisualisationCarte.ResumeLayout(false);
            BarreIconeZoom.ResumeLayout(false);
            BarreIconeZoom.PerformLayout();
            BarreIconeGestionRegroupement.ResumeLayout(false);
            BarreIconeGestionRegroupement.PerformLayout();
            BarreIconeMode.ResumeLayout(false);
            BarreIconeMode.PerformLayout();
            BarreIconeListeRegroupement.ResumeLayout(false);
            BarreIconeListeRegroupement.PerformLayout();
            ResumeLayout(false);
            #endregion
        }
        #region Déclaration
        private PictureBox AffichageCarte;
        private MenuStrip MenuProgramme;
        private ToolStripMenuItem MenuGestionRegroupements;
        private ToolStripMenuItem MenuOuvrirRegroupement;
        private OpenFileDialog OuvrirFichier;
        private ContextMenuStrip cmsZoomMenu;
        private ToolStripMenuItem Zoom_0750;
        private ToolStripMenuItem Zoom_0875;
        private ToolStripMenuItem Zoom_1000;
        private ToolStripMenuItem Zoom_1500;
        private ToolStripMenuItem Zoom_2000;
        private ToolStripMenuItem MenuQuitter;
        private ToolStripSeparator ToolStripSeparator2;
        private ToolStripMenuItem ContextMenuAfficherPlanRegroupement;
        private ContextMenuStrip MenuCxtVisualisationCarte;
        private ToolStripSplitButton IconeZoom;
        private ToolStrip BarreIconeZoom;
        private ToolStripButton IconeZoomMoins;
        private ToolStripButton IconeZoomPlus;
        private ToolStrip BarreIconeGestionRegroupement;
        private ToolStripButton IconeOuvrirRegroupement;
        private ToolStripButton IconeAfficherPlanRegroupement;
        private ToolStripButton IconeAjouterCartesRegroupement;
        private ToolStripMenuItem MenuFichiersTuiles;
        private ToolStripMenuItem Zoom_1125;
        private ToolStripMenuItem Zoom_1250;
        private ToolStripMenuItem Zoom_1750;
        private ToolStripComboBox ContextMenuListeRegroupement;
        private ToolStripMenuItem MenuSupprimerRegroupement;
        private ToolStripSeparator ToolStripSeparator5;
        private ToolStrip BarreIconeListeRegroupement;
        private ToolStripComboBox IconeListeRegroupement;
        private ToolStrip BarreIconeMode;
        private ToolStripButton IconeDeplacerCarte;
        private ToolStripMenuItem MenuConfiguration;
        private ToolStripMenuItem MenuConfigurationRegrouper;
        private ToolStripMenuItem MenuAide;
        private ToolStripMenuItem MenuAideRegrouper;
        private ToolStripMenuItem MenuAPropos;
        private ToolStripMenuItem MenuAjouterRegroupement;
        private ToolStripSeparator ToolStripSeparator1;
        private ToolStripMenuItem MenuCreerFichiersJNX_ORUX;
        private ToolStripMenuItem MenuCreerFichiersKMZ;
        private ToolStripSeparator ToolStripSeparator4;
        private ToolStripMenuItem MenuModifierIndicesAffichage;
        private ToolStripMenuItem MenuModifierFichierKMZ;
        private ToolStripSeparator ToolStripSeparator6;
        private ToolStripSeparator ToolStripSeparator7;
        private ToolStripMenuItem FichiersJNXToolStripMenuItem;
        private ToolStripMenuItem FichiersORUXToolStripMenuItem;
        private ToolStripMenuItem Zoom_0500;
        private ToolStripMenuItem Zoom_0660;
        private ToolStripMenuItem SupprimerSelectionToolStripMenuItem;
        private ToolStripComboBox SuppressionListeRegroupement;
        private Label Regroupement;
        private Label LabelCoordonnees;
        private Label Altitude;
        private Label LZoom;
        private Label LModeEncours;
        private Label LOutilEnCours;
        private ComboBox ListeChoixTypeCoordonnees;
        private ComboBox ListeChoixDimensions;
        private Label LabelDimensions;
        private Label ZoneSelection;
        #endregion
    }
}