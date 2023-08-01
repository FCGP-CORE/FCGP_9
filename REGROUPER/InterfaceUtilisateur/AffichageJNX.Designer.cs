using static FCGP.Commun;

namespace FCGP
{
    internal partial class AffichageJNX : Form
    {

        internal AffichageJNX()
        {
            InitializeComponent();
            InitialiseEvenements();
        }
        private void InitialiseEvenements()
        {
            // contrôles simples
            CorZoomJNX0.ValueChanged += CorZoomJNX_ValueChanged;
            CorZoomJNX1.ValueChanged += CorZoomJNX_ValueChanged;
            CorZoomJNX2.ValueChanged += CorZoomJNX_ValueChanged;
            CorZoomJNX3.ValueChanged += CorZoomJNX_ValueChanged;
            CorZoomJNX4.ValueChanged += CorZoomJNX_ValueChanged;
            ChoixFichierJNX.Click += ChoixFichierJNX_Click;
            CorNiveauAffichage.ValueChanged += CorNiveauAffichage_ValueChanged;
            Appliquer.Click += Appliquer_Click;
            Load += AffichageJNX_Load;
            FormClosed += AffichageJNX_FormClosed;
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
            var resources = new ComponentResourceManager(typeof(AffichageJNX));
            L001 = new Label();
            L004 = new Label();
            L003 = new Label();
            L002 = new Label();
            Echelle0 = new Panel();
            CorZoomJNX0 = new NumericUpDown();
            ZoomJNX0 = new Label();
            Clef0 = new Label();
            Echelle1 = new Panel();
            CorZoomJNX1 = new NumericUpDown();
            ZoomJNX1 = new Label();
            Clef1 = new Label();
            Echelle2 = new Panel();
            CorZoomJNX2 = new NumericUpDown();
            ZoomJNX2 = new Label();
            Clef2 = new Label();
            Echelle3 = new Panel();
            Clef3 = new Label();
            ZoomJNX3 = new Label();
            CorZoomJNX3 = new NumericUpDown();
            Echelle4 = new Panel();
            CorZoomJNX4 = new NumericUpDown();
            ZoomJNX4 = new Label();
            Clef4 = new Label();
            ChoixFichierJNX = new Button();
            Appliquer = new Button();
            Quitter = new Button();
            L005 = new Label();
            OrdreAffichage = new Panel();
            CorNiveauAffichage = new NumericUpDown();
            NiveauAffichage = new Label();
            ToolTip1 = new ToolTip(components);
            ChercheFichierJNX = new OpenFileDialog();
            Support = new Panel();
            Echelle0.SuspendLayout();
            ((ISupportInitialize)CorZoomJNX0).BeginInit();
            Echelle1.SuspendLayout();
            ((ISupportInitialize)CorZoomJNX1).BeginInit();
            Echelle2.SuspendLayout();
            ((ISupportInitialize)CorZoomJNX2).BeginInit();
            Echelle3.SuspendLayout();
            ((ISupportInitialize)CorZoomJNX3).BeginInit();
            Echelle4.SuspendLayout();
            ((ISupportInitialize)CorZoomJNX4).BeginInit();
            OrdreAffichage.SuspendLayout();
            ((ISupportInitialize)CorNiveauAffichage).BeginInit();
            SuspendLayout();
            #region Entête
            // 
            // Label25
            // 
            L001.BackColor = Color.Transparent;
            L001.BorderStyle = BorderStyle.FixedSingle;
            L001.Location = new Point(6, 6);
            L001.Name = "Label25";
            L001.Size = new Size(325, 23);
            L001.TabIndex = 21;
            L001.Text = "Niveaux de détails du fichier";
            L001.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            L002.BackColor = Color.Transparent;
            L002.BorderStyle = BorderStyle.FixedSingle;
            L002.Location = new Point(6, 28);
            L002.Name = "Label2";
            L002.Size = new Size(85, 21);
            L002.TabIndex = 15;
            L002.Text = "Clef";
            L002.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L002, "Clef du niveau de détail");
            // 
            // Label3
            // 
            L003.BackColor = Color.Transparent;
            L003.BorderStyle = BorderStyle.FixedSingle;
            L003.Location = new Point(90, 28);
            L003.Name = "Label3";
            L003.Size = new Size(105, 21);
            L003.TabIndex = 17;
            L003.Text = "Indice Actuel";
            L003.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L003, resources.GetString("Label3.ToolTip"));
            // 
            // Label4
            // 
            L004.BackColor = Color.Transparent;
            L004.BorderStyle = BorderStyle.FixedSingle;
            L004.Location = new Point(194, 28);
            L004.Name = "Label4";
            L004.Size = new Size(137, 21);
            L004.TabIndex = 19;
            L004.Text = "Indice Modifié";
            L004.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L004, resources.GetString("Label4.ToolTip"));
            #endregion
            #region echelles
            // 
            // Echelle0
            // 
            Echelle0.BackColor = Color.Transparent;
            Echelle0.BorderStyle = BorderStyle.FixedSingle;
            Echelle0.Controls.Add(CorZoomJNX0);
            Echelle0.Controls.Add(ZoomJNX0);
            Echelle0.Controls.Add(Clef0);
            Echelle0.Enabled = false;
            Echelle0.Location = new Point(6, 48);
            Echelle0.Margin = new Padding(0);
            Echelle0.Name = "Echelle0";
            Echelle0.Size = new Size(325, 24);
            Echelle0.TabIndex = 12;
            // 
            // Clef0
            // 
            Clef0.BorderStyle = BorderStyle.FixedSingle;
            Clef0.Location = new Point(-1, -1);
            Clef0.Name = "Clef0";
            Clef0.Size = new Size(85, 24);
            Clef0.TabIndex = 1;
            Clef0.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef0, "Clef du niveau de détail");
            // 
            // ZoomJNX0
            // 
            ZoomJNX0.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX0.Location = new Point(83, -1);
            ZoomJNX0.Name = "ZoomJNX0";
            ZoomJNX0.Size = new Size(105, 24);
            ZoomJNX0.TabIndex = 2;
            ZoomJNX0.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX0, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomJNX0
            // 
            CorZoomJNX0.BackColor = Color.White;
            CorZoomJNX0.DecimalPlaces = 2;
            CorZoomJNX0.Increment = 0.25m;
            CorZoomJNX0.Location = new Point(188, 0);
            CorZoomJNX0.Maximum = 18m;
            CorZoomJNX0.Name = "CorZoomJNX0";
            CorZoomJNX0.ReadOnly = true;
            CorZoomJNX0.Size = new Size(135, 23);
            CorZoomJNX0.TabIndex = 3;
            CorZoomJNX0.BorderStyle = BorderStyle.None;
            CorZoomJNX0.Tag = "0";
            CorZoomJNX0.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX0, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle1
            // 
            Echelle1.BackColor = Color.Transparent;
            Echelle1.BorderStyle = BorderStyle.FixedSingle;
            Echelle1.Controls.Add(CorZoomJNX1);
            Echelle1.Controls.Add(ZoomJNX1);
            Echelle1.Controls.Add(Clef1);
            Echelle1.Enabled = false;
            Echelle1.Location = new Point(6, 71);
            Echelle1.Margin = new Padding(0);
            Echelle1.Name = "Echelle1";
            Echelle1.Size = new Size(325, 24);
            Echelle1.TabIndex = 14;
            // 
            // Clef1
            // 
            Clef1.BorderStyle = BorderStyle.FixedSingle;
            Clef1.Location = new Point(-1, -1);
            Clef1.Name = "Clef1";
            Clef1.Size = new Size(85, 24);
            Clef1.TabIndex = 1;
            Clef1.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef1, "Clef du niveau de détail");
            // 
            // ZoomJNX1
            // 
            ZoomJNX1.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX1.Location = new Point(83, -1);
            ZoomJNX1.Name = "ZoomJNX1";
            ZoomJNX1.Size = new Size(105, 24);
            ZoomJNX1.TabIndex = 2;
            ZoomJNX1.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX1, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomJNX1
            // 
            CorZoomJNX1.BackColor = Color.White;
            CorZoomJNX1.DecimalPlaces = 2;
            CorZoomJNX1.Increment = 0.25m;
            CorZoomJNX1.Location = new Point(188, 0);
            CorZoomJNX1.Maximum = 18m;
            CorZoomJNX1.Name = "CorZoomJNX1";
            CorZoomJNX1.ReadOnly = true;
            CorZoomJNX1.Size = new Size(135, 23);
            CorZoomJNX1.TabIndex = 3;
            CorZoomJNX1.BorderStyle = BorderStyle.None;
            CorZoomJNX1.Tag = "1";
            CorZoomJNX1.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX1, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle2
            // 
            Echelle2.BackColor = Color.Transparent;
            Echelle2.BorderStyle = BorderStyle.FixedSingle;
            Echelle2.Controls.Add(CorZoomJNX2);
            Echelle2.Controls.Add(ZoomJNX2);
            Echelle2.Controls.Add(Clef2);
            Echelle2.Enabled = false;
            Echelle2.Location = new Point(6, 94);
            Echelle2.Margin = new Padding(0);
            Echelle2.Name = "Echelle2";
            Echelle2.Size = new Size(325, 24);
            Echelle2.TabIndex = 16;
            // 
            // Clef2
            // 
            Clef2.BorderStyle = BorderStyle.FixedSingle;
            Clef2.Location = new Point(-1, -1);
            Clef2.Name = "Clef2";
            Clef2.Size = new Size(85, 24);
            Clef2.TabIndex = 1;
            Clef2.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef2, "Clef du niveau de détail");
            // 
            // ZoomJNX2
            // 
            ZoomJNX2.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX2.Location = new Point(83, -1);
            ZoomJNX2.Name = "ZoomJNX2";
            ZoomJNX2.Size = new Size(105, 24);
            ZoomJNX2.TabIndex = 2;
            ZoomJNX2.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX2, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomJNX2
            // 
            CorZoomJNX2.BackColor = Color.White;
            CorZoomJNX2.DecimalPlaces = 2;
            CorZoomJNX2.Increment = 0.25m;
            CorZoomJNX2.Location = new Point(188, 0);
            CorZoomJNX2.Maximum = 18m;
            CorZoomJNX2.Name = "CorZoomJNX2";
            CorZoomJNX2.ReadOnly = true;
            CorZoomJNX2.Size = new Size(135, 23);
            CorZoomJNX2.TabIndex = 3;
            CorZoomJNX2.BorderStyle = BorderStyle.None;
            CorZoomJNX2.Tag = "2";
            CorZoomJNX2.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX2, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle3
            // 
            Echelle3.BackColor = Color.Transparent;
            Echelle3.BorderStyle = BorderStyle.FixedSingle;
            Echelle3.Controls.Add(Clef3);
            Echelle3.Controls.Add(ZoomJNX3);
            Echelle3.Controls.Add(CorZoomJNX3);
            Echelle3.Enabled = false;
            Echelle3.Location = new Point(6, 117);
            Echelle3.Margin = new Padding(0);
            Echelle3.Name = "Echelle3";
            Echelle3.Size = new Size(325, 24);
            Echelle3.TabIndex = 18;
            // 
            // Clef3
            // 
            Clef3.BorderStyle = BorderStyle.FixedSingle;
            Clef3.Location = new Point(-1, -1);
            Clef3.Name = "Clef3";
            Clef3.Size = new Size(85, 24);
            Clef3.TabIndex = 1;
            Clef3.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef3, "Clef du niveau de détail");
            // 
            // ZoomJNX3
            // 
            ZoomJNX3.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX3.Location = new Point(83, -1);
            ZoomJNX3.Name = "ZoomJNX3";
            ZoomJNX3.Size = new Size(105, 24);
            ZoomJNX3.TabIndex = 2;
            ZoomJNX3.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX3, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomJNX3
            // 
            CorZoomJNX3.BackColor = Color.White;
            CorZoomJNX3.DecimalPlaces = 2;
            CorZoomJNX3.Increment = 0.25m;
            CorZoomJNX3.Location = new Point(188, 0);
            CorZoomJNX3.Maximum = 18m;
            CorZoomJNX3.Name = "CorZoomJNX3";
            CorZoomJNX3.ReadOnly = true;
            CorZoomJNX3.Size = new Size(135, 23);
            CorZoomJNX3.TabIndex = 3;
            CorZoomJNX3.BorderStyle = BorderStyle.None;
            CorZoomJNX3.Tag = "3";
            CorZoomJNX3.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX3, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle4
            // 
            Echelle4.BackColor = Color.Transparent;
            Echelle4.BorderStyle = BorderStyle.FixedSingle;
            Echelle4.Controls.Add(CorZoomJNX4);
            Echelle4.Controls.Add(ZoomJNX4);
            Echelle4.Controls.Add(Clef4);
            Echelle4.Enabled = false;
            Echelle4.Location = new Point(6, 140);
            Echelle4.Margin = new Padding(0);
            Echelle4.Name = "Echelle4";
            Echelle4.Size = new Size(325, 24);
            Echelle4.TabIndex = 20;
            // 
            // Clef4
            // 
            Clef4.BorderStyle = BorderStyle.FixedSingle;
            Clef4.Location = new Point(-1, -1);
            Clef4.Name = "Clef4";
            Clef4.Size = new Size(85, 24);
            Clef4.TabIndex = 1;
            Clef4.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef4, "Clef du niveau de détail");
            // 
            // ZoomJNX4
            // 
            ZoomJNX4.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX4.Location = new Point(83, -1);
            ZoomJNX4.Name = "ZoomJNX4";
            ZoomJNX4.Size = new Size(105, 24);
            ZoomJNX4.TabIndex = 2;
            ZoomJNX4.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX4, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomJNX4
            // 
            CorZoomJNX4.BackColor = Color.White;
            CorZoomJNX4.DecimalPlaces = 2;
            CorZoomJNX4.Increment = 0.25m;
            CorZoomJNX4.Location = new Point(188, 0);
            CorZoomJNX4.Maximum = 18m;
            CorZoomJNX4.Name = "CorZoomJNX4";
            CorZoomJNX4.ReadOnly = true;
            CorZoomJNX4.Size = new Size(135, 23);
            CorZoomJNX4.TabIndex = 3;
            CorZoomJNX4.BorderStyle = BorderStyle.None;
            CorZoomJNX4.Tag = "4";
            CorZoomJNX4.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX4, "Voir l'aide sur le label Correction Zoom");
            #endregion
            #region Ordre affichage
            // 
            // Label1
            // 
            L005.BackColor = Color.Transparent;
            L005.BorderStyle = BorderStyle.FixedSingle;
            L005.Location = new Point(6, 170);
            L005.Name = "Label1";
            L005.Size = new Size(325, 23);
            L005.TabIndex = 26;
            L005.Text = "Ordre d'affichage du fichier";
            L005.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // OrdreAffichage
            // 
            OrdreAffichage.BackColor = Color.Transparent;
            OrdreAffichage.BorderStyle = BorderStyle.FixedSingle;
            OrdreAffichage.Controls.Add(CorNiveauAffichage);
            OrdreAffichage.Controls.Add(NiveauAffichage);
            OrdreAffichage.Enabled = false;
            OrdreAffichage.Location = new Point(6, 192);
            OrdreAffichage.Margin = new Padding(0);
            OrdreAffichage.Name = "OrdreAffichage";
            OrdreAffichage.Size = new Size(325, 24);
            OrdreAffichage.TabIndex = 13;
            // 
            // NiveauAffichage
            // 
            NiveauAffichage.BorderStyle = BorderStyle.FixedSingle;
            NiveauAffichage.Location = new Point(-1, -1);
            NiveauAffichage.Name = "NiveauAffichage";
            NiveauAffichage.Size = new Size(165, 24);
            NiveauAffichage.TabIndex = 2;
            NiveauAffichage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CorNiveauAffichage
            // 
            CorNiveauAffichage.BackColor = Color.White;
            CorNiveauAffichage.Location = new Point(164, 0);
            CorNiveauAffichage.Name = "CorNiveauAffichage";
            CorNiveauAffichage.Size = new Size(159, 23);
            CorNiveauAffichage.TabIndex = 3;
            CorNiveauAffichage.BorderStyle = BorderStyle.None;
            CorNiveauAffichage.Tag = "0";
            CorNiveauAffichage.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorNiveauAffichage, "Ordre d'affichage de la carte par rapport aux autres cartes. " + CrLf + 
                                                    "Permet de masquer plus de détails d'une carte vectorielle superposée.");
            #endregion
            #region Form
            // 
            // Support
            // 
            Support.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Controls.Add(L001);
            Support.Controls.Add(L002);
            Support.Controls.Add(L003);
            Support.Controls.Add(L004);
            Support.Controls.Add(Echelle0);
            Support.Controls.Add(Echelle1);
            Support.Controls.Add(Echelle2);
            Support.Controls.Add(Echelle3);
            Support.Controls.Add(Echelle4);
            Support.Controls.Add(L005);
            Support.Controls.Add(OrdreAffichage);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(337, 222);
            Support.TabIndex = 0;

            // 
            // ChoixFichierJNX
            // 
            ChoixFichierJNX.Location = new Point(5, 228);
            ChoixFichierJNX.Name = "ChoixFichierJNX";
            ChoixFichierJNX.Size = new Size(93, 46);
            ChoixFichierJNX.TabIndex = 23;
            ChoixFichierJNX.Text = "Choisir un fichier";
            ToolTip1.SetToolTip(ChoixFichierJNX, "Ouvre un formulaire qui permet de choisir un fichier JNX. Celui-ci peut être sur le GPS");
            ChoixFichierJNX.UseVisualStyleBackColor = true;
            // 
            // Appliquer
            // 
            Appliquer.Enabled = false;
            Appliquer.Location = new Point(109, 228);
            Appliquer.Name = "Appliquer";
            Appliquer.Size = new Size(125, 46);
            Appliquer.TabIndex = 24;
            Appliquer.Text = "Appliquer les modifications";
            ToolTip1.SetToolTip(Appliquer, "Enregistre les modifications apportées aux indices d'affichage dans le fichier JNX");
            Appliquer.UseVisualStyleBackColor = true;
            // 
            // Quitter
            // 
            Quitter.DialogResult = DialogResult.Cancel;
            Quitter.Location = new Point(245, 228);
            Quitter.Name = "Quitter";
            Quitter.Size = new Size(85, 46);
            Quitter.TabIndex = 25;
            Quitter.Text = "Quitter";
            ToolTip1.SetToolTip(Quitter, "Ferme le formulaire");
            Quitter.UseVisualStyleBackColor = true;
            // 
            // AffichageJNX
            // 
            AcceptButton = ChoixFichierJNX;
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            CancelButton = Quitter;
            ClientSize = new Size(337, 280);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(Quitter);
            Controls.Add(Appliquer);
            Controls.Add(ChoixFichierJNX);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AffichageJNX";
            Text = "Correction Affichage des Fichiers JNX";
            StartPosition = FormStartPosition.CenterParent;
            Echelle0.ResumeLayout(false);
            ((ISupportInitialize)CorZoomJNX0).EndInit();
            Echelle1.ResumeLayout(false);
            ((ISupportInitialize)CorZoomJNX1).EndInit();
            Echelle2.ResumeLayout(false);
            ((ISupportInitialize)CorZoomJNX2).EndInit();
            Echelle3.ResumeLayout(false);
            ((ISupportInitialize)CorZoomJNX3).EndInit();
            Echelle4.ResumeLayout(false);
            ((ISupportInitialize)CorZoomJNX4).EndInit();
            OrdreAffichage.ResumeLayout(false);
            ((ISupportInitialize)CorNiveauAffichage).EndInit();
            ResumeLayout(false);
            #endregion
        }

        private Label L001;
        private Label L004;
        private Label L003;
        private Label L002;
        private Panel Echelle0;
        private NumericUpDown CorZoomJNX0;
        private Label ZoomJNX0;
        private Label Clef0;
        private Panel Echelle1;
        private NumericUpDown CorZoomJNX1;
        private Label ZoomJNX1;
        private Label Clef1;
        private Panel Echelle2;
        private NumericUpDown CorZoomJNX2;
        private Label ZoomJNX2;
        private Label Clef2;
        private Panel Echelle3;
        private Label Clef3;
        private Label ZoomJNX3;
        private NumericUpDown CorZoomJNX3;
        private Panel Echelle4;
        private NumericUpDown CorZoomJNX4;
        private Label ZoomJNX4;
        private Label Clef4;
        private Button ChoixFichierJNX;
        private Button Appliquer;
        private Button Quitter;
        private Label L005;
        private Panel OrdreAffichage;
        private NumericUpDown CorNiveauAffichage;
        private Label NiveauAffichage;
        private ToolTip ToolTip1;
        private OpenFileDialog ChercheFichierJNX;
        private Panel Support;
    }
}