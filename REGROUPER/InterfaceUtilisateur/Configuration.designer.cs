using static FCGP.Commun;

namespace FCGP
{
    internal partial class Configuration : Form
    {

        internal Configuration()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            AvecAltitude.CheckedChanged += AvecAltitude_CheckedChanged;
            ID.TextChanged += IDs_TextChanged;
            MP.TextChanged += IDs_TextChanged;
            VerifierID.Click += VerifierID_Click;
            CouleurFondCarte.Click += Couleur_Click;
            CouleurFondAffichage.Click += Couleur_Click;
            RepertoireAltitudes.Click += RepertoireTuiles_Click;
            RepertoireTuiles.Click += RepertoireTuiles_Click;
            // EcranTravail
            EcranTravail.Click += EcranTravail_Click;
            ListeEcrans.DropDownClosed += ListeEcrans_DropDownClosed;
            ListeEcrans.SelectedIndexChanged += ListeEcrans_SelectedIndexChanged;
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
            const string URS = "\"https://urs.earthdata.nasa.gov/home\"";
            components = new Container();
            var resources = new ComponentResourceManager(typeof(Configuration));
            ToolTip1 = new ToolTip(components);
            Valider = new Button();
            Ignorer = new Button();
            PasDeplacement = new NumericUpDown();
            DEM1 = new CheckBox();
            AvecAltitude = new CheckBox();
            ID = new TextBox();
            MSGAltitude = new CheckBox();
            MP = new MaskedTextBox();
            CouleurFondCarte = new Label();
            CouleurFondAffichage = new Label();
            EcranTravail = new Label();
            VerifierID = new Label();
            RepertoireAltitudes = new Label();
            RepertoireTuiles = new Label();
            ListeEcrans = new ComboBox();
            ChoixCouleurTrace = new ColorDialog();
            Support = new Panel();
            L006 = new Label();
            L007 = new Label();
            PanelAltitude = new Panel();
            LabelID = new Label();
            LabelIdentifiantsNASA = new Label();
            LabelFichiersAltitude = new Label();
            LabelMP = new Label();
            LabelRepertoirefichiersAltitude = new Label();
            L004 = new Label();
            L001 = new Label();
            L002 = new Label();
            L003 = new Label();
            L005 = new Label();
            ((ISupportInitialize)PasDeplacement).BeginInit();
            Support.SuspendLayout();
            PanelAltitude.SuspendLayout();
            SuspendLayout();

            // 
            // L001
            // 
            L001.BorderStyle = BorderStyle.FixedSingle;
            L001.Location = new Point(6, 6);
            L001.Margin = new Padding(0);
            L001.Name = "L001";
            L001.Size = new Size(171, 23);
            L001.TabIndex = 79;
            L001.Text = "Couleur fond Affichage";
            L001.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L002
            // 
            L002.BorderStyle = BorderStyle.FixedSingle;
            L002.Location = new Point(176, 6);
            L002.Margin = new Padding(0);
            L002.Name = "L002";
            L002.Size = new Size(172, 23);
            L002.TabIndex = 78;
            L002.Text = "Couleur fond des Cartes";
            L002.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CouleurFondAffichage
            // 
            CouleurFondAffichage.BorderStyle = BorderStyle.FixedSingle;
            CouleurFondAffichage.Location = new Point(6, 28);
            CouleurFondAffichage.Name = "CouleurFondAffichage";
            CouleurFondAffichage.Size = new Size(171, 23);
            CouleurFondAffichage.TabIndex = 80;
            ToolTip1.SetToolTip(CouleurFondAffichage, "Couleur actuelle de la fenêtre d'affichage.Cliquez pour choisir une autre couleur.");
            // 
            // CouleurFondCarte
            // 
            CouleurFondCarte.BorderStyle = BorderStyle.FixedSingle;
            CouleurFondCarte.Location = new Point(176, 28);
            CouleurFondCarte.Name = "CouleurFondCarte";
            CouleurFondCarte.Size = new Size(172, 23);
            CouleurFondCarte.TabIndex = 83;
            ToolTip1.SetToolTip(CouleurFondCarte, "Couleur actuelle des fonds de carte.Cliquez pour choisir une autre couleur.");
            // 
            // L003
            // 
            L003.BorderStyle = BorderStyle.FixedSingle;
            L003.Location = new Point(6, 57);
            L003.Margin = new Padding(0);
            L003.Name = "L003";
            L003.Size = new Size(342, 23);
            L003.TabIndex = 76;
            L003.Text = "Ecran de travail";
            L003.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EcranTravail
            // 
            EcranTravail.BackColor = Color.White;
            EcranTravail.BorderStyle = BorderStyle.FixedSingle;
            EcranTravail.Location = new Point(6, 79);
            EcranTravail.Margin = new Padding(0);
            EcranTravail.Name = "EcranTravail";
            EcranTravail.Size = new Size(342, 23);
            EcranTravail.TabIndex = 118;
            EcranTravail.Text = "EcranTravail";
            EcranTravail.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(EcranTravail, "Ecran de travail actuel. Cliquez pour voir la liste des écrans disponibles.");
            // 
            // ListeEcrans
            // 
            ListeEcrans.FormattingEnabled = true;
            ListeEcrans.Visible = false;
            ListeEcrans.Location = new Point(6, 74);
            ListeEcrans.Name = "ListeEcrans";
            ListeEcrans.Size = new Size(342, 24);
            ListeEcrans.TabIndex = 72;
            // 
            // L004
            // 
            L004.BorderStyle = BorderStyle.FixedSingle;
            L004.Location = new Point(6, 108);
            L004.Margin = new Padding(0);
            L004.Name = "L004";
            L004.Size = new Size(231, 24);
            L004.TabIndex = 100;
            L004.Text = "Pas de déplacement";
            L004.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L005
            // 
            L005.BorderStyle = BorderStyle.FixedSingle;
            L005.Name = "L005";
            L005.Location = new Point(236, 108);
            L005.Size = new Size(112, 24);
            // 
            // PasDeplacement
            // 
            PasDeplacement.BackColor = Color.White;
            PasDeplacement.BorderStyle = BorderStyle.None;
            PasDeplacement.Location = new Point(237, 109);
            PasDeplacement.Name = "PasDeplacement";
            PasDeplacement.Size = new Size(110, 22);
            PasDeplacement.TabIndex = 112;
            PasDeplacement.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(PasDeplacement, "Transparence du dessin de la trace.0% --> Opaque100% --> Invisible");
            PasDeplacement.Value = 25m;
            // 
            // L006
            // 
            L006.BorderStyle = BorderStyle.FixedSingle;
            L006.Location = new Point(6, 138);
            L006.Margin = new Padding(0);
            L006.Name = "L006";
            L006.Size = new Size(342, 23);
            L006.TabIndex = 122;
            L006.Text = "Répertoire d'enregistrement des Fichiers Tuiles";
            L006.TextAlign = ContentAlignment.BottomCenter;
            // 
            // RepertoireTuiles
            // 
            RepertoireTuiles.BackColor = Color.White;
            RepertoireTuiles.BorderStyle = BorderStyle.FixedSingle;
            RepertoireTuiles.Location = new Point(6, 160);
            RepertoireTuiles.Name = "RepertoireTuiles";
            RepertoireTuiles.Size = new Size(342, 23);
            RepertoireTuiles.TabIndex = 121;
            RepertoireTuiles.TextAlign = ContentAlignment.MiddleCenter;
            RepertoireTuiles.Tag = "Répertoire d'enregistrement des Fichiers Tuiles";
            ToolTip1.SetToolTip(RepertoireTuiles, "Répertoire d'enregistrement actuel des fichiers Tuiles" + CrLf +
                                                  "Cliquez pour changer de répertoire.");
            // 
            // L007
            // 
            L007.BackColor = Color.Transparent;
            L007.BorderStyle = BorderStyle.FixedSingle;
            L007.Location = new Point(6, 189);
            L007.Margin = new Padding(0);
            L007.Name = "Label3";
            L007.Size = new Size(342, 25);
            L007.TabIndex = 120;
            L007.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AvecAltitude
            // 
            AvecAltitude.BackColor = Color.Transparent;
            AvecAltitude.CheckAlign = ContentAlignment.MiddleRight;
            AvecAltitude.Location = new Point(9, 192);
            AvecAltitude.Margin = new Padding(0);
            AvecAltitude.Name = "AvecAltitude";
            AvecAltitude.Size = new Size(330, 21);
            AvecAltitude.TabIndex = 86;
            AvecAltitude.Text = "Afficher l'altitude avec les coordonnées";
            AvecAltitude.TextAlign = ContentAlignment.TopLeft;
            ToolTip1.SetToolTip(AvecAltitude, "Indique si FCGP_Visue doit afficher l'altitude" + CrLf +
                                              "avec les coordonnéeslors du déplacement du pointeur de souris.");
            AvecAltitude.UseVisualStyleBackColor = false;
            // 
            // LabelIdentifiantsNASA
            // 
            LabelIdentifiantsNASA.BorderStyle = BorderStyle.FixedSingle;
            LabelIdentifiantsNASA.Location = new Point(0, 0);
            LabelIdentifiantsNASA.Margin = new Padding(0);
            LabelIdentifiantsNASA.Name = "LabelIdentifiantsNASA";
            LabelIdentifiantsNASA.Size = new Size(231, 21);
            LabelIdentifiantsNASA.TabIndex = 92;
            LabelIdentifiantsNASA.Text = "Identifiants du site de la NASA";
            LabelIdentifiantsNASA.TextAlign = ContentAlignment.BottomCenter;
            // 
            // LabelFichiersAltitude
            // 
            LabelFichiersAltitude.BorderStyle = BorderStyle.FixedSingle;
            LabelFichiersAltitude.Location = new Point(230, 0);
            LabelFichiersAltitude.Name = "LabelFichiersAltitude";
            LabelFichiersAltitude.Size = new Size(112, 67);
            LabelFichiersAltitude.TabIndex = 115;
            LabelFichiersAltitude.TextAlign = ContentAlignment.TopCenter;
            // 
            // LabelID
            // 
            // LabelID.BackColor = Color.Transparent
            LabelID.BorderStyle = BorderStyle.FixedSingle;
            LabelID.Location = new Point(0, 20);
            LabelID.Name = "LabelID";
            LabelID.Size = new Size(33, 24);
            LabelID.TabIndex = 90;
            LabelID.Text = "ID ";
            LabelID.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ID
            // 
            ID.BackColor = Color.White;
            ID.Location = new Point(32, 18);
            ID.Name = "ID";
            ID.Size = new Size(165, 24);
            ID.TabIndex = 87;
            ToolTip1.SetToolTip(ID, $"Indiquez le nom d'utilisateur de votre compte sur le site de données{CrLf}de la NASA<{URS}> .");
            // 
            // LabelMP
            // 
            LabelMP.BackColor = Color.Transparent;
            LabelMP.BorderStyle = BorderStyle.FixedSingle;
            LabelMP.Location = new Point(0, 43);
            LabelMP.Name = "LabelMP";
            LabelMP.Size = new Size(33, 24);
            LabelMP.TabIndex = 91;
            LabelMP.Text = "MP ";
            LabelMP.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MP
            // 
            MP.BackColor = Color.White;
            MP.Location = new Point(32, 41);
            MP.Name = "MP";
            MP.PasswordChar = '*';
            MP.Size = new Size(165, 24);
            MP.TabIndex = 88;
            ToolTip1.SetToolTip(MP, $"Indiquez le mot de passe de votre compte sur le site de données{CrLf}de la NASA<{URS}>.");
            // 
            // VerifierID
            // 
            VerifierID.BorderStyle = BorderStyle.FixedSingle;
            VerifierID.Location = new Point(196, 20);
            VerifierID.Name = "VerifierID";
            VerifierID.Size = new Size(35, 47);
            VerifierID.TabIndex = 113;
            VerifierID.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(VerifierID, $"Vérifie que les identifiants indiqués sont reconnus par{CrLf}le serveur de données de la NASA<{URS}>.");
            // 
            // DEM1
            // 
            DEM1.Location = new Point(233, 10);
            DEM1.Margin = new Padding(0);
            DEM1.Name = "DEM1";
            DEM1.RightToLeft = RightToLeft.Yes;
            DEM1.Size = new Size(100, 22);
            DEM1.TabIndex = 96;
            DEM1.Text = "Type Arc 1";
            DEM1.TextAlign = ContentAlignment.MiddleRight;
            ToolTip1.SetToolTip(DEM1, resources.GetString("DEM1.ToolTip"));
            DEM1.UseVisualStyleBackColor = false;
            // 
            // MSGAltitude
            // 
            MSGAltitude.Location = new Point(233, 37);
            MSGAltitude.Margin = new Padding(0);
            MSGAltitude.Name = "MSGAltitude";
            MSGAltitude.RightToLeft = RightToLeft.Yes;
            MSGAltitude.Size = new Size(100, 22);
            MSGAltitude.TabIndex = 116;
            MSGAltitude.Text = "Messages";
            MSGAltitude.TextAlign = ContentAlignment.MiddleRight;
            ToolTip1.SetToolTip(MSGAltitude, "Indique si les messages liés au téléchargement" + CrLf +
                                             "des fichiers d'altitudes sont affichés.");
            MSGAltitude.UseVisualStyleBackColor = false;
            // 
            // LabelRepertoirefichiersAltitude
            // 
            LabelRepertoirefichiersAltitude.BackColor = Color.Transparent;
            LabelRepertoirefichiersAltitude.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoirefichiersAltitude.Location = new Point(0, 66);
            LabelRepertoirefichiersAltitude.Margin = new Padding(0);
            LabelRepertoirefichiersAltitude.Name = "LabelRepertoirefichiersAltitude";
            LabelRepertoirefichiersAltitude.Size = new Size(342, 23);
            LabelRepertoirefichiersAltitude.TabIndex = 95;
            LabelRepertoirefichiersAltitude.Text = "Répertoire d'enregistrement des fichiers d'altitudes";
            LabelRepertoirefichiersAltitude.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RepertoireAltitudes
            // 
            RepertoireAltitudes.BackColor = Color.White;
            RepertoireAltitudes.BorderStyle = BorderStyle.FixedSingle;
            RepertoireAltitudes.Location = new Point(0, 88);
            RepertoireAltitudes.Name = "RepertoireAltitudes";
            RepertoireAltitudes.Size = new Size(342, 22);
            RepertoireAltitudes.TabIndex = 94;
            RepertoireAltitudes.Tag = "Répertoire d'enregistrement des Fichiers d'Altitude";
            ToolTip1.SetToolTip(RepertoireAltitudes, "Répertoire d'enregistrement actuel des fichiers d'altitude (.hgt)" + CrLf +
                                                     "Cliquez pour changer de répertoire.");
            // 
            // PanelAltitude
            // 
            PanelAltitude.Controls.Add(LabelID);
            PanelAltitude.Controls.Add(LabelIdentifiantsNASA);
            PanelAltitude.Controls.Add(DEM1);
            PanelAltitude.Controls.Add(MSGAltitude);
            PanelAltitude.Controls.Add(LabelFichiersAltitude);
            PanelAltitude.Controls.Add(LabelMP);
            PanelAltitude.Controls.Add(RepertoireAltitudes);
            PanelAltitude.Controls.Add(VerifierID);
            PanelAltitude.Controls.Add(LabelRepertoirefichiersAltitude);
            PanelAltitude.Controls.Add(ID);
            PanelAltitude.Controls.Add(MP);
            PanelAltitude.BorderStyle = BorderStyle.None;
            PanelAltitude.Location = new Point(6, 213);
            PanelAltitude.Name = "PanelAltitude";
            PanelAltitude.Size = new Size(342, 110);
            PanelAltitude.TabIndex = 119;
            // 
            // Support
            // 
            Support.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Controls.Add(L006);
            Support.Controls.Add(RepertoireTuiles);
            Support.Controls.Add(AvecAltitude);
            Support.Controls.Add(L007);
            Support.Controls.Add(PanelAltitude);
            Support.Controls.Add(EcranTravail);
            Support.Controls.Add(L004);
            Support.Controls.Add(PasDeplacement);
            Support.Controls.Add(L005);
            Support.Controls.Add(CouleurFondCarte);
            Support.Controls.Add(CouleurFondAffichage);
            Support.Controls.Add(L001);
            Support.Controls.Add(L002);
            Support.Controls.Add(L003);
            Support.Controls.Add(ListeEcrans);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(354, 329);
            Support.TabIndex = 72;
            // 
            // Ok
            // 
            Valider.Anchor = AnchorStyles.Bottom;
            Valider.DialogResult = DialogResult.OK;
            Valider.Location = new Point(273, 335);
            Valider.Name = "Ok";
            Valider.Size = new Size(75, 33);
            Valider.TabIndex = 13;
            Valider.Text = "Valider";
            ToolTip1.SetToolTip(Valider, "Ferme le formualaire en prenant en compte les modifications.");
            Valider.UseVisualStyleBackColor = true;
            // 
            // Ignore
            // 
            Ignorer.Anchor = AnchorStyles.Bottom;
            Ignorer.DialogResult = DialogResult.Abort;
            Ignorer.Location = new Point(192, 335);
            Ignorer.Name = "Ignore";
            Ignorer.Size = new Size(75, 33);
            Ignorer.TabIndex = 12;
            Ignorer.Text = "Annuler";
            ToolTip1.SetToolTip(Ignorer, "Ferme le formulaire sans prendre en compte les modifications.");
            Ignorer.UseVisualStyleBackColor = true;
            // 
            // ToolTip1
            // 
            ToolTip1.AutoPopDelay = 5000;
            ToolTip1.InitialDelay = 100;
            ToolTip1.ReshowDelay = 100;
            // 
            // Configuration
            // 
            AcceptButton = Valider;
            AutoScaleMode = AutoScaleMode.None;
            AutoSizeMode = AutoSizeMode.GrowOnly;
            CancelButton = Ignorer;
            ClientSize = new Size(354, 374);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(Ignorer);
            Controls.Add(Valider);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Configuration";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            // TopMost = True
            ((ISupportInitialize)PasDeplacement).EndInit();
            Support.ResumeLayout(false);
            PanelAltitude.ResumeLayout(false);
            PanelAltitude.PerformLayout();
            ResumeLayout(false);
        }

        private ToolTip ToolTip1;
        private Button Valider;
        private Button Ignorer;
        private ColorDialog ChoixCouleurTrace;
        private Panel Support;
        private Label VerifierID;
        private NumericUpDown PasDeplacement;
        private Label L004;
        private CheckBox DEM1;
        private Label LabelRepertoirefichiersAltitude;
        private Label RepertoireAltitudes;
        private CheckBox AvecAltitude;
        private TextBox ID;
        private Label LabelIdentifiantsNASA;
        private CheckBox MSGAltitude;
        private Label LabelFichiersAltitude;
        private MaskedTextBox MP;
        private Label LabelMP;
        private Label LabelID;
        private Label CouleurFondCarte;
        private Label CouleurFondAffichage;
        private Label L001;
        private Label L002;
        private Label L003;
        private ComboBox ListeEcrans;
        private Label EcranTravail;
        private Panel PanelAltitude;
        private Label L007;
        private Label L006;
        private Label RepertoireTuiles;
        private Label L005;
    }
}