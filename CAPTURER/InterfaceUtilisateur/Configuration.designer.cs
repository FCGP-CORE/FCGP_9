using static FCGP.Commun;

namespace FCGP
{
    internal partial class Configuration : Form
    {

        private const string MsgUri = "<https://urs.earthdata.nasa.gov/home>.";
        private const string MsgChemin = "Cliquez pour changer de chemin.";
        private const string MsgCouleur = "Cliquez pour choisir une autre couleur.";
        internal Configuration()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        /// <summary> Initialise l'ensemble des évenements liés au formulaire car pas variable avec le modificateur WithEvents 
        /// rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
        private void InitialiserEvenements()
        {
            // controles simples
            CouleurCartes.Click += Couleurs_Click;
            CouleurAffichage.Click += Couleurs_Click;
            IsToutFormat.CheckedChanged += IsToutFormat_CheckedChanged;
            RepertoireCachesTuiles.Click += Repertoires_Click;
            AvecAltitudes.CheckedChanged += AvecAltitude_CheckedChanged;
            ID.TextChanged += IDs_TextChanged;
            MP.TextChanged += IDs_TextChanged;
            VerifierID.Click += VerifierID_Click;
            RepertoireFichiersAltitudes.Click += Repertoires_Click;
            CouleurTraces.Click += Couleurs_Click;
            CouleurDessinTraces.Click += Couleurs_Click;
            CoefAlphaTraces.ValueChanged += TransparenceTrace_ValueChanged;
            RepertoireTraces.Click += Repertoires_Click;
            // EcranTravail
            LabelChoixEcranTravail.Click += Labels_Click;
            ListeChoixEcranTravail.SelectedIndexChanged += Listes_SelectedIndexChanged;
            // SupportCarte
            LabelChoixSupportCarte.Click += Labels_Click;
            ListeChoixSupportCarte.SelectedIndexChanged += Listes_SelectedIndexChanged;
            // formulaire
            Load += Configuration_Load;
            FormClosed += Configuration_FormClosed;
            FormClosing += Configuration_FormClosing;
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
            components = new Container();
            var resources = new ComponentResourceManager(typeof(Configuration));
            PanelSupport = new Panel();
            LabelCouleurAffichage = new Label();
            CouleurAffichage = new Label();
            LabelCouleurCartes = new Label();
            CouleurCartes = new Label();
            LabelEcranTravail = new Label();
            LabelChoixEcranTravail = new Label();
            ListeChoixEcranTravail = new ComboBox();
            LabelJpeg = new Label();
            LabelQualiteJpeg = new Label();
            QualiteJpeg = new NumericUpDown();
            LabelSupportCarte = new Label();
            LabelChoixSupportCarte = new Label();
            ListeChoixSupportCarte = new ComboBox();
            LabelRepertoireChachesTuiles = new Label();
            RepertoireCachesTuiles = new Label();
            IsToutFormat = new CheckBox();
            LabelIsToutFormat = new Label();
            AvecAltitudes = new CheckBox();
            LabelAvecAltitudes = new Label();
            PanelAltitudes = new Panel();
            LabelIdentifiantsNASA = new Label();
            LabelID = new Label();
            ID = new TextBox();
            LabelMP = new Label();
            MP = new MaskedTextBox();
            VerifierID = new Label();
            DEM1 = new CheckBox();
            MSGAltitudes = new CheckBox();
            LabelDonneesAltitudes = new Label();
            LabelRepertoireFichiersAltitudes = new Label();
            RepertoireFichiersAltitudes = new Label();
            LabelCouleurTraces = new Label();
            CouleurTraces = new Label();
            LabelAspectTraces = new Label();
            LabelCouleurDessinTraces = new Label();
            CouleurDessinTraces = new Label();
            LabelCoefAlphaTraces = new Label();
            CoefAlphaTraces = new NumericUpDown();
            LabelEpaisseurTraces = new Label();
            EpaisseurTraces = new NumericUpDown();
            LabelRepertoireTraces = new Label();
            RepertoireTraces = new Label();
            BoutonEnter = new Button();
            BoutonEscape = new Button();
            ToolTip1 = new ToolTip(components);
            ((ISupportInitialize)QualiteJpeg).BeginInit();
            ((ISupportInitialize)CoefAlphaTraces).BeginInit();
            ((ISupportInitialize)EpaisseurTraces).BeginInit();
            PanelSupport.SuspendLayout();
            PanelAltitudes.SuspendLayout();
            SuspendLayout();
            // 
            // LabelCouleurAffichage
            // 
            LabelCouleurAffichage.BackColor = Color.Transparent;
            LabelCouleurAffichage.BorderStyle = BorderStyle.FixedSingle;
            LabelCouleurAffichage.Location = new Point(6, 6);
            LabelCouleurAffichage.Margin = new Padding(0);
            LabelCouleurAffichage.Name = "LabelCouleurAffichage";
            LabelCouleurAffichage.Size = new Size(171, 21);
            LabelCouleurAffichage.TabIndex = 79;
            LabelCouleurAffichage.Text = "Couleur fond Affichage";
            LabelCouleurAffichage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CouleurAffichage
            // 
            CouleurAffichage.BackColor = Color.White;
            CouleurAffichage.BorderStyle = BorderStyle.FixedSingle;
            CouleurAffichage.Location = new Point(6, 26);
            CouleurAffichage.Name = "CouleurAffichage";
            CouleurAffichage.Size = new Size(171, 21);
            CouleurAffichage.TabIndex = 80;
            CouleurAffichage.Tag = false;
            ToolTip1.SetToolTip(CouleurAffichage, "Couleur actuelle de la fenêtre d'affichage." + CrLf + MsgCouleur);
            // 
            // LabeCouleurCartes
            // 
            LabelCouleurCartes.BackColor = Color.Transparent;
            LabelCouleurCartes.BorderStyle = BorderStyle.FixedSingle;
            LabelCouleurCartes.Location = new Point(176, 6);
            LabelCouleurCartes.Margin = new Padding(0);
            LabelCouleurCartes.Name = "LabeCouleurCartes";
            LabelCouleurCartes.Size = new Size(172, 21);
            LabelCouleurCartes.TabIndex = 78;
            LabelCouleurCartes.Text = "Couleur fond des Cartes";
            LabelCouleurCartes.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CouleurCartes
            // 
            CouleurCartes.BackColor = Color.White;
            CouleurCartes.BorderStyle = BorderStyle.FixedSingle;
            CouleurCartes.Location = new Point(176, 26);
            CouleurCartes.Name = "CouleurCartes";
            CouleurCartes.Size = new Size(172, 21);
            CouleurCartes.TabIndex = 83;
            CouleurCartes.Tag = false;
            ToolTip1.SetToolTip(CouleurCartes, "Couleur actuelle des fonds de carte." + CrLf + MsgCouleur);
            // 
            // LabelEcranTravail
            // 
            LabelEcranTravail.BackColor = Color.Transparent;
            LabelEcranTravail.BorderStyle = BorderStyle.FixedSingle;
            LabelEcranTravail.Location = new Point(6, 52);
            LabelEcranTravail.Margin = new Padding(0);
            LabelEcranTravail.Name = "LabelEcranTravail";
            LabelEcranTravail.Size = new Size(342, 21);
            LabelEcranTravail.TabIndex = 76;
            LabelEcranTravail.Text = "Ecran de travail";
            LabelEcranTravail.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelChoixEcranTravail
            // 
            LabelChoixEcranTravail.BackColor = Color.White;
            LabelChoixEcranTravail.BorderStyle = BorderStyle.FixedSingle;
            LabelChoixEcranTravail.Location = new Point(6, 72);
            LabelChoixEcranTravail.Margin = new Padding(0);
            LabelChoixEcranTravail.Name = "EcranTravail";
            LabelChoixEcranTravail.Size = new Size(342, 21);
            LabelChoixEcranTravail.TabIndex = 118;
            LabelChoixEcranTravail.Text = "LabelChoixEcranTravail";
            LabelChoixEcranTravail.TextAlign = ContentAlignment.MiddleCenter;
            LabelChoixEcranTravail.Tag = ListeChoixEcranTravail;
            ToolTip1.SetToolTip(LabelChoixEcranTravail, "Ecran de travail actuel. " + CrLf +
                                                        "Cliquez pour voir la liste des écrans disponibles.");
            // 
            // ListeChoixEcranTravail
            // 
            ListeChoixEcranTravail.BackColor = Color.White;
            ListeChoixEcranTravail.FormattingEnabled = true;
            ListeChoixEcranTravail.Location = new Point(6, 65);
            ListeChoixEcranTravail.Name = "ListeChoixEcranTravail";
            ListeChoixEcranTravail.Size = new Size(342, 0);
            ListeChoixEcranTravail.Visible = false;
            ListeChoixEcranTravail.TabIndex = 72;
            ListeChoixEcranTravail.Tag = LabelChoixEcranTravail;
            // 
            // LabelJpeg
            // 
            LabelJpeg.BackColor = Color.Transparent;
            LabelJpeg.BorderStyle = BorderStyle.FixedSingle;
            LabelJpeg.Location = new Point(6, 99);
            LabelJpeg.Margin = new Padding(0);
            LabelJpeg.Name = "LabelJpeg";
            LabelJpeg.Size = new Size(90, 21);
            LabelJpeg.TabIndex = 77;
            LabelJpeg.Text = "Qualité Jpeg";
            LabelJpeg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelQualiteJpeg
            // 
            LabelQualiteJpeg.BackColor = Color.White;
            LabelQualiteJpeg.BorderStyle = BorderStyle.FixedSingle;
            LabelQualiteJpeg.Location = new Point(6, 119);
            LabelQualiteJpeg.Margin = new Padding(0);
            LabelQualiteJpeg.Name = "LabelQualiteJpeg";
            LabelQualiteJpeg.Size = new Size(90, 24);
            LabelQualiteJpeg.TabIndex = 117;
            LabelQualiteJpeg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // QualiteJpeg
            // 
            QualiteJpeg.BackColor = Color.White;
            QualiteJpeg.Location = new Point(7, 120);
            QualiteJpeg.BorderStyle = BorderStyle.None;
            QualiteJpeg.Maximum = 100m;
            QualiteJpeg.Minimum = 50m;
            QualiteJpeg.Increment = 1m;
            QualiteJpeg.Name = "QualiteJpeg";
            QualiteJpeg.Size = new Size(88, 23);
            QualiteJpeg.TabIndex = 111;
            QualiteJpeg.TextAlign = HorizontalAlignment.Center;
            QualiteJpeg.Value = 75m;
            ToolTip1.SetToolTip(QualiteJpeg, "Qualité désirée pour l'enregistrement des cartes au format Jpeg" + CrLf + 
                                             "100 : Très bonne qualité mais taille du fichier importante" + CrLf + 
                                             "50 : Mauvaise qualité mais petite taille de fichier");
            // 
            // LabelSupportCarte
            // 
            LabelSupportCarte.BackColor = Color.Transparent;
            LabelSupportCarte.BorderStyle = BorderStyle.FixedSingle;
            LabelSupportCarte.Location = new Point(95, 99);
            LabelSupportCarte.Margin = new Padding(0);
            LabelSupportCarte.Name = "LabelSupportCarte";
            LabelSupportCarte.Size = new Size(253, 21);
            LabelSupportCarte.TabIndex = 77;
            LabelSupportCarte.Text = "Support de la Carte";
            LabelSupportCarte.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelChoixSupportCarte
            // 
            LabelChoixSupportCarte.BackColor = Color.White;
            LabelChoixSupportCarte.BorderStyle = BorderStyle.FixedSingle;
            LabelChoixSupportCarte.Location = new Point(95, 119);
            LabelChoixSupportCarte.Margin = new Padding(0);
            LabelChoixSupportCarte.Name = "LabelChoixSupportCarte";
            LabelChoixSupportCarte.Size = new Size(137, 24);
            LabelChoixSupportCarte.TabIndex = 117;
            LabelChoixSupportCarte.Text = "Support de la Carte";
            LabelChoixSupportCarte.TextAlign = ContentAlignment.MiddleCenter;
            LabelChoixSupportCarte.Tag = ListeChoixSupportCarte;
            ToolTip1.SetToolTip(LabelChoixSupportCarte, resources.GetString("SupportCarte.ToolTip"));
            // 
            // ListeChoixSupportCarte
            // 
            ListeChoixSupportCarte.BackColor = Color.White;
            ListeChoixSupportCarte.FormattingEnabled = true;
            ListeChoixSupportCarte.Items.AddRange(new object[] { "Mémoire", "Fichier" });
            ListeChoixSupportCarte.Location = new Point(95, 115);
            ListeChoixSupportCarte.Name = "ListeChoixSupportCarte";
            ListeChoixSupportCarte.Size = new Size(137, 0);
            ListeChoixSupportCarte.Visible = false;
            ListeChoixSupportCarte.TabIndex = 73;
            ListeChoixSupportCarte.Visible = false;
            ListeChoixSupportCarte.Tag = LabelChoixSupportCarte;
            // 
            // IsToutFormat
            // 
            IsToutFormat.BackColor = Color.Transparent;
            IsToutFormat.CheckAlign = ContentAlignment.MiddleRight;
            IsToutFormat.Location = new Point(234, 122);
            IsToutFormat.Name = "IsToutFormat";
            IsToutFormat.Size = new Size(105, 20);
            IsToutFormat.TabIndex = 74;
            IsToutFormat.Text = "Tout Format";
            IsToutFormat.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(IsToutFormat, resources.GetString("IsToutFormat.ToolTip"));
            // 
            // LabelToutFormat
            // 
            LabelIsToutFormat.BackColor = Color.Transparent;
            LabelIsToutFormat.BorderStyle = BorderStyle.FixedSingle;
            LabelIsToutFormat.Location = new Point(231, 119);
            LabelIsToutFormat.Name = "LabelToutFormat";
            LabelIsToutFormat.Size = new Size(117, 24);
            LabelIsToutFormat.TabIndex = 75;
            // 
            // LabelRepertoireChacheTuiles
            // 
            LabelRepertoireChachesTuiles.BackColor = Color.Transparent;
            LabelRepertoireChachesTuiles.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireChachesTuiles.Location = new Point(6, 149);
            LabelRepertoireChachesTuiles.Margin = new Padding(0);
            LabelRepertoireChachesTuiles.Name = "LabelRepertoireChacheTuiles";
            LabelRepertoireChachesTuiles.Size = new Size(342, 21);
            LabelRepertoireChachesTuiles.TabIndex = 122;
            LabelRepertoireChachesTuiles.Text = "Répertoire des Caches Tuiles";
            LabelRepertoireChachesTuiles.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RepertoireCacheTuiles
            // 
            RepertoireCachesTuiles.BackColor = Color.White;
            RepertoireCachesTuiles.BorderStyle = BorderStyle.FixedSingle;
            RepertoireCachesTuiles.Location = new Point(6, 169);
            RepertoireCachesTuiles.Name = "RepertoireCacheTuiles";
            RepertoireCachesTuiles.Size = new Size(342, 21);
            RepertoireCachesTuiles.TabIndex = 121;
            RepertoireCachesTuiles.Tag = "Caches Tuiles";
            RepertoireCachesTuiles.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(RepertoireCachesTuiles, "Chemin d'enregistrement actuel des fichiers de trace (.trk)" + CrLf + MsgChemin);
            // 
            // AvecAltitudes
            // 
            AvecAltitudes.BackColor = Color.Transparent;
            AvecAltitudes.CheckAlign = ContentAlignment.MiddleRight;
            AvecAltitudes.Location = new Point(8, 198);
            AvecAltitudes.Margin = new Padding(0);
            AvecAltitudes.Name = "AvecAltitudes";
            AvecAltitudes.Size = new Size(331, 20);
            AvecAltitudes.TabIndex = 86;
            AvecAltitudes.Text = "Altitude avec les coordonnées";
            AvecAltitudes.TextAlign = ContentAlignment.MiddleCenter;
            AvecAltitudes.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(AvecAltitudes, "Indique si FCGP_Capturer doit afficher l'altitude avec les coordonnées" + CrLf + 
                                               "lors du déplacement du pointeur de souris.");
            // 
            // LabelAltitudes
            // 
            LabelAvecAltitudes.BackColor = Color.Transparent;
            LabelAvecAltitudes.BorderStyle = BorderStyle.FixedSingle;
            LabelAvecAltitudes.Location = new Point(6, 196);
            LabelAvecAltitudes.Margin = new Padding(0);
            LabelAvecAltitudes.Name = "LabelAltitudes";
            LabelAvecAltitudes.Size = new Size(342, 23);
            LabelAvecAltitudes.TabIndex = 120;
            LabelAvecAltitudes.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PanelAltitudes
            // 
            PanelAltitudes.BackColor = Color.Transparent;
            PanelAltitudes.Controls.Add(LabelIdentifiantsNASA);
            PanelAltitudes.Controls.Add(LabelID);
            PanelAltitudes.Controls.Add(ID);
            PanelAltitudes.Controls.Add(LabelMP);
            PanelAltitudes.Controls.Add(MP);
            PanelAltitudes.Controls.Add(VerifierID);
            PanelAltitudes.Controls.Add(DEM1);
            PanelAltitudes.Controls.Add(MSGAltitudes);
            PanelAltitudes.Controls.Add(LabelDonneesAltitudes);
            PanelAltitudes.Controls.Add(LabelRepertoireFichiersAltitudes);
            PanelAltitudes.Controls.Add(RepertoireFichiersAltitudes);
            PanelAltitudes.Location = new Point(6, 218);
            PanelAltitudes.Name = "PanelAltitudes";
            PanelAltitudes.Size = new Size(342, 111);
            PanelAltitudes.TabIndex = 119;
            // 
            // LabelIdentifiantsNASA
            // 
            LabelIdentifiantsNASA.BackColor = Color.Transparent;
            LabelIdentifiantsNASA.BorderStyle = BorderStyle.FixedSingle;
            LabelIdentifiantsNASA.Location = new Point(0, 0);
            LabelIdentifiantsNASA.Margin = new Padding(0);
            LabelIdentifiantsNASA.Name = "LabelIdentifiantsNASA";
            LabelIdentifiantsNASA.Size = new Size(231, 21);
            LabelIdentifiantsNASA.TabIndex = 92;
            LabelIdentifiantsNASA.Text = "Identifiants du site de la NASA";
            LabelIdentifiantsNASA.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelID
            // 
            LabelID.BackColor = Color.Transparent;
            LabelID.BorderStyle = BorderStyle.FixedSingle;
            LabelID.Location = new Point(0, 20);
            LabelID.Name = "LabelID";
            LabelID.Size = new Size(33, 26);
            LabelID.TabIndex = 90;
            LabelID.Text = "ID ";
            LabelID.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ID
            // 
            ID.BackColor = Color.White;
            ID.Location = new Point(32, 20);
            ID.Name = "ID";
            ID.Size = new Size(155, 26);
            ID.TabIndex = 87;
            ToolTip1.SetToolTip(ID, "Indiquez le nom d'utilisateur de votre compte sur le site de données de la NASA" + CrLf + MsgUri);
            // 
            // LabelMP
            // 
            LabelMP.BackColor = Color.Transparent;
            LabelMP.BorderStyle = BorderStyle.FixedSingle;
            LabelMP.Location = new Point(0, 45);
            LabelMP.Name = "LabelMP";
            LabelMP.Size = new Size(33, 26);
            LabelMP.TabIndex = 91;
            LabelMP.Text = "MP ";
            LabelMP.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MP
            // 
            MP.BackColor = Color.White;
            MP.Location = new Point(32, 45);
            MP.Name = "MP";
            MP.PasswordChar = '*'; // Convert.ToChar(42)
            MP.Size = new Size(155, 19);
            MP.TabIndex = 88;
            ToolTip1.SetToolTip(MP, "Indiquez le mot de passe de votre compte sur le site de données de la NASA" + CrLf + MsgUri);
            // 
            // VerifierID
            // 
            VerifierID.BorderStyle = BorderStyle.FixedSingle;
            VerifierID.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            VerifierID.Location = new Point(186, 20);
            VerifierID.Name = "VerifierID";
            VerifierID.Size = new Size(45, 51);
            VerifierID.TabIndex = 113;
            VerifierID.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(VerifierID, "Vérifie que les identifiants indiqués sont reconnus par" + CrLf + 
                                            "le serveur de données de la NASA" + CrLf + MsgUri);

            // 
            // DEM1
            // 
            DEM1.BackColor = Color.Transparent;
            DEM1.Location = new Point(233, 12);
            DEM1.Margin = new Padding(0);
            DEM1.Name = "DEM1";
            DEM1.RightToLeft = RightToLeft.Yes;
            DEM1.Size = new Size(100, 23);
            DEM1.TabIndex = 96;
            DEM1.Text = "Type Arc 1";
            DEM1.TextAlign = ContentAlignment.MiddleRight;
            DEM1.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(DEM1, resources.GetString("DEM1.ToolTip"));
            // 
            // MSGAltitudes
            // 
            MSGAltitudes.BackColor = Color.Transparent;
            MSGAltitudes.Location = new Point(233, 39);
            MSGAltitudes.Margin = new Padding(0);
            MSGAltitudes.Name = "MSGAltitudes";
            MSGAltitudes.RightToLeft = RightToLeft.Yes;
            MSGAltitudes.Size = new Size(100, 23);
            MSGAltitudes.TabIndex = 116;
            MSGAltitudes.Text = "Messages" + CrLf;
            MSGAltitudes.TextAlign = ContentAlignment.MiddleRight;
            MSGAltitudes.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(MSGAltitudes, "Indique si les messages liés au téléchargement" + CrLf + 
                                              "des fichiers d'altitudes sont affichés.");
            // 
            // LabelDonneesAltitudes
            // 
            LabelDonneesAltitudes.BackColor = Color.Transparent;
            LabelDonneesAltitudes.BorderStyle = BorderStyle.FixedSingle;
            LabelDonneesAltitudes.Location = new Point(230, 0);
            LabelDonneesAltitudes.Name = "LabelDonneesAltitudes";
            LabelDonneesAltitudes.Size = new Size(112, 71);
            LabelDonneesAltitudes.TabIndex = 115;
            // 
            // LabelRepertoirefichiersAltitude
            // 
            LabelRepertoireFichiersAltitudes.BackColor = Color.Transparent;
            LabelRepertoireFichiersAltitudes.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireFichiersAltitudes.Location = new Point(0, 70);
            LabelRepertoireFichiersAltitudes.Margin = new Padding(0);
            LabelRepertoireFichiersAltitudes.Name = "LabelRepertoirefichiersAltitude";
            LabelRepertoireFichiersAltitudes.Size = new Size(342, 21);
            LabelRepertoireFichiersAltitudes.TabIndex = 95;
            LabelRepertoireFichiersAltitudes.Text = "Répertoire d'enregistrement des fichiers d'altitudes"; // & CrLf 
            LabelRepertoireFichiersAltitudes.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RepertoireAltitudes
            // 
            RepertoireFichiersAltitudes.BackColor = Color.White;
            RepertoireFichiersAltitudes.BorderStyle = BorderStyle.FixedSingle;
            RepertoireFichiersAltitudes.Location = new Point(0, 90);
            RepertoireFichiersAltitudes.Name = "RepertoireAltitudes";
            RepertoireFichiersAltitudes.Size = new Size(342, 21);
            RepertoireFichiersAltitudes.TabIndex = 94;
            RepertoireFichiersAltitudes.Tag = "fichiers d'altitudes";
            RepertoireFichiersAltitudes.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(RepertoireFichiersAltitudes, "Chemin d'enregistrement actuel des fichiers d'altitude (.hgt)" + CrLf + MsgChemin);
            // 
            // LabelCouleurTraces
            // 
            LabelCouleurTraces.BackColor = Color.Transparent;
            LabelCouleurTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelCouleurTraces.Location = new Point(6, 335);
            LabelCouleurTraces.Margin = new Padding(0);
            LabelCouleurTraces.Name = "LabelCouleurTraces";
            LabelCouleurTraces.Size = new Size(342, 21);
            LabelCouleurTraces.TabIndex = 102;
            LabelCouleurTraces.Text = "Couleur par défaut des Traces à l'écran";
            LabelCouleurTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CouleurTraces
            // 
            CouleurTraces.BackColor = Color.White;
            CouleurTraces.BorderStyle = BorderStyle.FixedSingle;
            CouleurTraces.Location = new Point(6, 355);
            CouleurTraces.Name = "CouleurTraces";
            CouleurTraces.Size = new Size(342, 21);
            CouleurTraces.TabIndex = 103;
            CouleurTraces.Tag = false;
            ToolTip1.SetToolTip(CouleurTraces, "Couleur par défaut des traces sur l'écran." + CrLf + MsgCouleur);
            // 
            // LabelAspectTraces
            // 
            LabelAspectTraces.BackColor = Color.Transparent;
            LabelAspectTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelAspectTraces.Location = new Point(6, 375);
            LabelAspectTraces.Margin = new Padding(0);
            LabelAspectTraces.Name = "LabelAspectTraces";
            LabelAspectTraces.Size = new Size(342, 21);
            LabelAspectTraces.TabIndex = 112;
            LabelAspectTraces.Text = "Aspect des Traces lors du dessin sur la carte";
            LabelAspectTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelCouleurDessinTraces
            // 
            LabelCouleurDessinTraces.BackColor = Color.Transparent;
            LabelCouleurDessinTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelCouleurDessinTraces.Location = new Point(6, 395);
            LabelCouleurDessinTraces.Name = "LabelCouleurDessinTraces";
            LabelCouleurDessinTraces.Size = new Size(171, 40);
            LabelCouleurDessinTraces.TabIndex = 108;
            LabelCouleurDessinTraces.Text = "Couleur";
            LabelCouleurDessinTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CouleurDessinTraces
            // 
            CouleurDessinTraces.BackColor = Color.White;
            CouleurDessinTraces.BorderStyle = BorderStyle.FixedSingle;
            CouleurDessinTraces.Location = new Point(6, 434);
            CouleurDessinTraces.Name = "CouleurDessinTraces";
            CouleurDessinTraces.Size = new Size(171, 26);
            CouleurDessinTraces.TabIndex = 109;
            CouleurDessinTraces.Tag = true;
            ToolTip1.SetToolTip(CouleurDessinTraces, "Couleur des traces lors du dessin sur la carte." + CrLf + MsgCouleur);
            // 
            // LabelCoefAlphaTraces
            // 
            LabelCoefAlphaTraces.BackColor = Color.Transparent;
            LabelCoefAlphaTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelCoefAlphaTraces.Location = new Point(176, 395);
            LabelCoefAlphaTraces.Name = "LabelCoefAlphaTraces";
            LabelCoefAlphaTraces.Size = new Size(95, 40);
            LabelCoefAlphaTraces.TabIndex = 107;
            LabelCoefAlphaTraces.Text = "Transparence" + CrLf + "%";
            LabelCoefAlphaTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CoefAlphaTraces
            // 
            CoefAlphaTraces.AutoSize = true;
            CoefAlphaTraces.BackColor = Color.White;
            CoefAlphaTraces.Location = new Point(176, 434);
            CoefAlphaTraces.Name = "CoefAlphaTraces";
            CoefAlphaTraces.Size = new Size(95, 26);
            CoefAlphaTraces.TabIndex = 110;
            CoefAlphaTraces.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CoefAlphaTraces, "Transparence du dessin de la trace." + CrLf + "0% --> Opaque" + CrLf + "100% --> Invisible");
            // 
            // LabelEpaisseurTraces
            // 
            LabelEpaisseurTraces.BackColor = Color.Transparent;
            LabelEpaisseurTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelEpaisseurTraces.Location = new Point(270, 395);
            LabelEpaisseurTraces.Name = "LabelEpaisseurTraces";
            LabelEpaisseurTraces.Size = new Size(78, 40);
            LabelEpaisseurTraces.TabIndex = 106;
            LabelEpaisseurTraces.Text = "Epaisseur" + CrLf + "en pixels";
            LabelEpaisseurTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EpaisseurTraces
            // 
            EpaisseurTraces.AutoSize = true;
            EpaisseurTraces.BackColor = Color.White;
            EpaisseurTraces.Location = new Point(270, 434);
            EpaisseurTraces.Maximum = 20m;
            EpaisseurTraces.Minimum = 2m;
            EpaisseurTraces.Name = "EpaisseurTraces";
            EpaisseurTraces.Size = new Size(78, 26);
            EpaisseurTraces.TabIndex = 111;
            EpaisseurTraces.TextAlign = HorizontalAlignment.Center;
            EpaisseurTraces.Value = 2m;
            ToolTip1.SetToolTip(EpaisseurTraces, "Epaisseur de la trace en pixels." + CrLf + "Le nb de pixels dépend de l'échelle de la carte" + CrLf + 
                                                 "pour que la trace soit correctement visible.");
            // 
            // LabelRepertoireTraces
            // 
            LabelRepertoireTraces.BackColor = Color.Transparent;
            LabelRepertoireTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireTraces.Location = new Point(6, 459);
            LabelRepertoireTraces.Margin = new Padding(0);
            LabelRepertoireTraces.Name = "LabelRepertoireTraces";
            LabelRepertoireTraces.Size = new Size(342, 21);
            LabelRepertoireTraces.TabIndex = 100;
            LabelRepertoireTraces.Text = "Répertoire d'enregistrement des Traces";
            LabelRepertoireTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RepertoireTraces
            // 
            RepertoireTraces.BackColor = Color.White;
            RepertoireTraces.BorderStyle = BorderStyle.FixedSingle;
            RepertoireTraces.Location = new Point(6, 479);
            RepertoireTraces.Name = "RepertoireTraces";
            RepertoireTraces.Size = new Size(342, 22);
            RepertoireTraces.TabIndex = 99;
            RepertoireTraces.Tag = "fichiers de traces";
            RepertoireTraces.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(RepertoireTraces, "Chemin d'enregistrement actuel des fichiers de trace (.trk)" + CrLf + MsgChemin);
            // 
            // PanelSupport
            // 
            PanelSupport.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            PanelSupport.BackColor = Color.FromArgb(247, 247, 247);
            PanelSupport.Controls.Add(LabelCouleurAffichage);
            PanelSupport.Controls.Add(CouleurAffichage);
            PanelSupport.Controls.Add(LabelCouleurCartes);
            PanelSupport.Controls.Add(CouleurCartes);
            PanelSupport.Controls.Add(LabelEcranTravail);
            PanelSupport.Controls.Add(LabelChoixEcranTravail);
            PanelSupport.Controls.Add(ListeChoixEcranTravail);
            PanelSupport.Controls.Add(LabelJpeg);
            PanelSupport.Controls.Add(QualiteJpeg);
            PanelSupport.Controls.Add(LabelQualiteJpeg);
            PanelSupport.Controls.Add(LabelSupportCarte);
            PanelSupport.Controls.Add(LabelChoixSupportCarte);
            PanelSupport.Controls.Add(ListeChoixSupportCarte);
            PanelSupport.Controls.Add(IsToutFormat);
            PanelSupport.Controls.Add(LabelIsToutFormat);
            PanelSupport.Controls.Add(LabelRepertoireChachesTuiles);
            PanelSupport.Controls.Add(RepertoireCachesTuiles);
            PanelSupport.Controls.Add(AvecAltitudes);
            PanelSupport.Controls.Add(LabelAvecAltitudes);
            PanelSupport.Controls.Add(PanelAltitudes);
            PanelSupport.Controls.Add(LabelCouleurTraces);
            PanelSupport.Controls.Add(CouleurTraces);
            PanelSupport.Controls.Add(LabelAspectTraces);
            PanelSupport.Controls.Add(LabelCouleurDessinTraces);
            PanelSupport.Controls.Add(CouleurDessinTraces);
            PanelSupport.Controls.Add(LabelCoefAlphaTraces);
            PanelSupport.Controls.Add(LabelEpaisseurTraces);
            PanelSupport.Controls.Add(LabelRepertoireTraces);
            PanelSupport.Controls.Add(CoefAlphaTraces);
            PanelSupport.Controls.Add(EpaisseurTraces);
            PanelSupport.Controls.Add(RepertoireTraces);
            PanelSupport.Location = new Point(0, 0);
            PanelSupport.Name = "PanelSupport";
            PanelSupport.Size = new Size(354, 507);
            PanelSupport.TabIndex = 72;
            // 
            // BoutonEnter
            // 
            BoutonEnter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BoutonEnter.DialogResult = DialogResult.OK;
            BoutonEnter.Location = new Point(273, 513);
            BoutonEnter.Name = "BoutonEnter";
            BoutonEnter.Size = new Size(75, 30);
            BoutonEnter.TabIndex = 1;
            BoutonEnter.Text = "Valider";
            BoutonEnter.UseVisualStyleBackColor = true;
            ToolTip1.SetToolTip(BoutonEnter, "Ferme le formulaire en prenant en compte les modifications.");
            // 
            // BoutonEscape
            // 
            BoutonEscape.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BoutonEscape.DialogResult = DialogResult.Cancel;
            BoutonEscape.Location = new Point(192, 513);
            BoutonEscape.Name = "BoutonEscape";
            BoutonEscape.Size = new Size(75, 30);
            BoutonEscape.TabIndex = 2;
            BoutonEscape.Text = "Annuler";
            BoutonEscape.UseVisualStyleBackColor = true;
            ToolTip1.SetToolTip(BoutonEscape, "Ferme le formulaire sans prendre en compte les modifications.");
            // 
            // ToolTip1
            // 
            ToolTip1.AutoPopDelay = 5000;
            ToolTip1.InitialDelay = 100;
            ToolTip1.ReshowDelay = 100;
            // 
            // Configuration
            // 
            AcceptButton = BoutonEnter;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = BoutonEscape;
            ClientSize = new Size(354, 549);
            ControlBox = false;
            Controls.Add(PanelSupport);
            Controls.Add(BoutonEscape);
            Controls.Add(BoutonEnter);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Configuration";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            ((ISupportInitialize)QualiteJpeg).EndInit();
            ((ISupportInitialize)EpaisseurTraces).EndInit();
            ((ISupportInitialize)CoefAlphaTraces).EndInit();
            PanelSupport.ResumeLayout(false);
            PanelSupport.PerformLayout();
            PanelAltitudes.ResumeLayout(false);
            PanelAltitudes.PerformLayout();
            ResumeLayout(false);
        }

        private Panel PanelSupport;
        private Label LabelCouleurAffichage;
        private Label CouleurAffichage;
        private Label LabelCouleurCartes;
        private Label CouleurCartes;
        private Label LabelEcranTravail;
        private Label LabelChoixEcranTravail;
        private ComboBox ListeChoixEcranTravail;
        private Label LabelJpeg;
        private Label LabelQualiteJpeg;
        private NumericUpDown QualiteJpeg;
        private Label LabelSupportCarte;
        private Label LabelChoixSupportCarte;
        private ComboBox ListeChoixSupportCarte;
        private CheckBox IsToutFormat;
        private Label LabelIsToutFormat;
        private Label LabelRepertoireChachesTuiles;
        private Label RepertoireCachesTuiles;
        private CheckBox AvecAltitudes;
        private Label LabelAvecAltitudes;
        private Panel PanelAltitudes;
        private Label LabelIdentifiantsNASA;
        private Label LabelID;
        private TextBox ID;
        private Label LabelMP;
        private MaskedTextBox MP;
        private Label VerifierID;
        private CheckBox DEM1;
        private CheckBox MSGAltitudes;
        private Label LabelDonneesAltitudes;
        private Label LabelRepertoireFichiersAltitudes;
        private Label RepertoireFichiersAltitudes;
        private Label LabelCouleurTraces;
        private Label CouleurTraces;
        private Label LabelAspectTraces;
        private Label LabelCouleurDessinTraces;
        private Label CouleurDessinTraces;
        private Label LabelCoefAlphaTraces;
        private NumericUpDown CoefAlphaTraces;
        private Label LabelEpaisseurTraces;
        private NumericUpDown EpaisseurTraces;
        private Label LabelRepertoireTraces;
        private Label RepertoireTraces;
        private Button BoutonEnter;
        private Button BoutonEscape;
        private ToolTip ToolTip1;
    }
}