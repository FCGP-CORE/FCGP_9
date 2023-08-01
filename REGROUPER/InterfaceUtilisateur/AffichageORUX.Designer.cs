using static FCGP.Commun;

namespace FCGP
{
    internal partial class AffichageORUX : Form
    {

        internal AffichageORUX()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            CorZoomORUX0.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomORUX1.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomORUX2.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomORUX3.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomORUX4.ValueChanged += CorZoomORUX_ValueChanged;
            ChoixFichierORUX.Click += ChoixFichierORUX_Click;
            Appliquer.Click += Appliquer_Click;
            Load += AffichageORUX_Load;
            FormClosed += AffichageORUX_FormClosed;
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
            var resources = new ComponentResourceManager(typeof(AffichageORUX));
            L001 = new Label();
            L004 = new Label();
            L003 = new Label();
            L002 = new Label();
            Echelle0 = new Panel();
            CorZoomORUX0 = new NumericUpDown();
            ZoomORUX0 = new Label();
            Clef0 = new Label();
            Echelle1 = new Panel();
            CorZoomORUX1 = new NumericUpDown();
            ZoomORUX1 = new Label();
            Clef1 = new Label();
            Echelle2 = new Panel();
            CorZoomORUX2 = new NumericUpDown();
            ZoomORUX2 = new Label();
            Clef2 = new Label();
            Echelle3 = new Panel();
            Clef3 = new Label();
            ZoomORUX3 = new Label();
            CorZoomORUX3 = new NumericUpDown();
            Echelle4 = new Panel();
            CorZoomORUX4 = new NumericUpDown();
            ZoomORUX4 = new Label();
            Clef4 = new Label();
            ChoixFichierORUX = new Button();
            Appliquer = new Button();
            Quitter = new Button();
            ToolTip1 = new ToolTip(components);
            ChercheFichierORUX = new OpenFileDialog();
            Support = new Panel();
            Echelle0.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX0).BeginInit();
            Echelle1.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX1).BeginInit();
            Echelle2.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX2).BeginInit();
            Echelle3.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX3).BeginInit();
            Echelle4.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX4).BeginInit();
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
            ToolTip1.SetToolTip(L004, "Valeur du zoom ORUX corrigé (pas de 1)" + CrLf + "Valeur de transition" + CrLf +
                                      "1 niveau de différence --> 75% -150%" + CrLf +
                                      "2 niveaux de différence --> 50% - 200%" + CrLf + "3 niveaux de différence --> 35% - 280%");
            #endregion
            #region Echelles
            // 
            // Echelle0
            // 
            Echelle0.BackColor = Color.Transparent;
            Echelle0.BorderStyle = BorderStyle.FixedSingle;
            Echelle0.Controls.Add(CorZoomORUX0);
            Echelle0.Controls.Add(ZoomORUX0);
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
            // ZoomORUX0
            // 
            ZoomORUX0.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX0.Location = new Point(83, -1);
            ZoomORUX0.Name = "ZoomORUX0";
            ZoomORUX0.Size = new Size(105, 24);
            ZoomORUX0.TabIndex = 2;
            ZoomORUX0.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX0, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomORUX0
            // 
            CorZoomORUX0.BackColor = Color.White;
            CorZoomORUX0.Location = new Point(188, 0);
            CorZoomORUX0.Maximum = 20m;
            CorZoomORUX0.Name = "CorZoomORUX0";
            CorZoomORUX0.ReadOnly = true;
            CorZoomORUX0.Size = new Size(135, 23);
            CorZoomORUX0.TabIndex = 3;
            CorZoomORUX0.BorderStyle = BorderStyle.None;
            CorZoomORUX0.Tag = "0";
            CorZoomORUX0.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX0, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle1
            // 
            Echelle1.BackColor = Color.Transparent;
            Echelle1.BorderStyle = BorderStyle.FixedSingle;
            Echelle1.Controls.Add(CorZoomORUX1);
            Echelle1.Controls.Add(ZoomORUX1);
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
            // ZoomORUX1
            // 
            ZoomORUX1.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX1.Location = new Point(83, -1);
            ZoomORUX1.Name = "ZoomORUX1";
            ZoomORUX1.Size = new Size(105, 24);
            ZoomORUX1.TabIndex = 2;
            ZoomORUX1.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX1, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomORUX1
            // 
            CorZoomORUX1.BackColor = Color.White;
            CorZoomORUX1.Location = new Point(188, 0);
            CorZoomORUX1.Maximum = 20m;
            CorZoomORUX1.Name = "CorZoomORUX1";
            CorZoomORUX1.ReadOnly = true;
            CorZoomORUX1.Size = new Size(135, 23);
            CorZoomORUX1.TabIndex = 3;
            CorZoomORUX1.BorderStyle = BorderStyle.None;
            CorZoomORUX1.Tag = "1";
            CorZoomORUX1.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX1, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle2
            // 
            Echelle2.BackColor = Color.Transparent;
            Echelle2.BorderStyle = BorderStyle.FixedSingle;
            Echelle2.Controls.Add(CorZoomORUX2);
            Echelle2.Controls.Add(ZoomORUX2);
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
            // ZoomORUX2
            // 
            ZoomORUX2.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX2.Location = new Point(83, -1);
            ZoomORUX2.Name = "ZoomORUX2";
            ZoomORUX2.Size = new Size(105, 24);
            ZoomORUX2.TabIndex = 2;
            ZoomORUX2.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX2, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomORUX2
            // 
            CorZoomORUX2.BackColor = Color.White;
            CorZoomORUX2.Location = new Point(188, 0);
            CorZoomORUX2.Maximum = 20m;
            CorZoomORUX2.Name = "CorZoomORUX2";
            CorZoomORUX2.ReadOnly = true;
            CorZoomORUX2.Size = new Size(135, 23);
            CorZoomORUX2.TabIndex = 3;
            CorZoomORUX2.BorderStyle = BorderStyle.None;
            CorZoomORUX2.Tag = "2";
            CorZoomORUX2.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX2, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle3
            // 
            Echelle3.BackColor = Color.Transparent;
            Echelle3.BorderStyle = BorderStyle.FixedSingle;
            Echelle3.Controls.Add(Clef3);
            Echelle3.Controls.Add(ZoomORUX3);
            Echelle3.Controls.Add(CorZoomORUX3);
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
            // ZoomORUX3
            // 
            ZoomORUX3.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX3.Location = new Point(83, -1);
            ZoomORUX3.Name = "ZoomORUX3";
            ZoomORUX3.Size = new Size(105, 24);
            ZoomORUX3.TabIndex = 2;
            ZoomORUX3.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX3, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomORUX3
            // 
            CorZoomORUX3.BackColor = Color.White;
            CorZoomORUX3.Location = new Point(188, 0);
            CorZoomORUX3.Maximum = 20m;
            CorZoomORUX3.Name = "CorZoomORUX3";
            CorZoomORUX3.ReadOnly = true;
            CorZoomORUX3.Size = new Size(135, 23);
            CorZoomORUX3.TabIndex = 3;
            CorZoomORUX3.BorderStyle = BorderStyle.None;
            CorZoomORUX3.Tag = "3";
            CorZoomORUX3.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX3, "Voir l'aide sur le label Correction Zoom");
            // 
            // Echelle4
            // 
            Echelle4.BackColor = Color.Transparent;
            Echelle4.BorderStyle = BorderStyle.FixedSingle;
            Echelle4.Controls.Add(CorZoomORUX4);
            Echelle4.Controls.Add(ZoomORUX4);
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
            // ZoomORUX4
            // 
            ZoomORUX4.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX4.Location = new Point(83, -1);
            ZoomORUX4.Name = "ZoomORUX4";
            ZoomORUX4.Size = new Size(105, 24);
            ZoomORUX4.TabIndex = 2;
            ZoomORUX4.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX4, "Voir l'aide sur le label Zoom");
            // 
            // CorZoomORUX4
            // 
            CorZoomORUX4.BackColor = Color.White;
            CorZoomORUX4.Location = new Point(188, 0);
            CorZoomORUX4.Maximum = 20m;
            CorZoomORUX4.Name = "CorZoomORUX4";
            CorZoomORUX4.ReadOnly = true;
            CorZoomORUX4.Size = new Size(135, 23);
            CorZoomORUX4.TabIndex = 3;
            CorZoomORUX4.BorderStyle = BorderStyle.None;
            CorZoomORUX4.Tag = "4";
            CorZoomORUX4.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX4, "Voir l'aide sur le label Correction Zoom");
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
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(337, 170);
            Support.TabIndex = 0;
            // 
            // ChoixFichierORUX
            // 
            ChoixFichierORUX.Location = new Point(6, 176);
            ChoixFichierORUX.Name = "ChoixFichierORUX";
            ChoixFichierORUX.Size = new Size(93, 46);
            ChoixFichierORUX.TabIndex = 23;
            ChoixFichierORUX.Text = "Choisir un fichier";
            ToolTip1.SetToolTip(ChoixFichierORUX, "Ouvre un formulaire qui permet de choisir un fichier ORUX." + CrLf + 
                                                  "Celui-ci ne peut pas être sur le Smartphone (Lecture seule).");
            ChoixFichierORUX.UseVisualStyleBackColor = true;
            // 
            // Appliquer
            // 
            Appliquer.Enabled = false;
            Appliquer.Location = new Point(109, 176);
            Appliquer.Name = "Appliquer";
            Appliquer.Size = new Size(125, 46);
            Appliquer.TabIndex = 24;
            Appliquer.Text = "Appliquer les modifications";
            ToolTip1.SetToolTip(Appliquer, "Enregistre les modifications apportées aux indices d'affichage dans le fichier ORUX");
            Appliquer.UseVisualStyleBackColor = true;
            // 
            // Quitter
            // 
            Quitter.DialogResult = DialogResult.Cancel;
            Quitter.Location = new Point(245, 176);
            Quitter.Name = "Quitter";
            Quitter.Size = new Size(85, 46);
            Quitter.TabIndex = 25;
            Quitter.Text = "Quitter";
            ToolTip1.SetToolTip(Quitter, "Ferme le formulaire");
            Quitter.UseVisualStyleBackColor = true;
            // 
            // AffichageORUX
            // 
            AcceptButton = ChoixFichierORUX;
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            CancelButton = Quitter;
            ClientSize = new Size(337, 228);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(Quitter);
            Controls.Add(Appliquer);
            Controls.Add(ChoixFichierORUX);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AffichageORUX";
            Text = "Correction Affichage des Fichiers ORUX";
            StartPosition = FormStartPosition.CenterParent;
            Echelle0.ResumeLayout(false);
            ((ISupportInitialize)CorZoomORUX0).EndInit();
            Echelle1.ResumeLayout(false);
            ((ISupportInitialize)CorZoomORUX1).EndInit();
            Echelle2.ResumeLayout(false);
            ((ISupportInitialize)CorZoomORUX2).EndInit();
            Echelle3.ResumeLayout(false);
            ((ISupportInitialize)CorZoomORUX3).EndInit();
            Echelle4.ResumeLayout(false);
            ((ISupportInitialize)CorZoomORUX4).EndInit();
            ResumeLayout(false);
            #endregion
        }

        private Label L001;
        private Label L004;
        private Label L003;
        private Label L002;
        private Panel Echelle0;
        private NumericUpDown CorZoomORUX0;
        private Label ZoomORUX0;
        private Label Clef0;
        private Panel Echelle1;
        private NumericUpDown CorZoomORUX1;
        private Label ZoomORUX1;
        private Label Clef1;
        private Panel Echelle2;
        private NumericUpDown CorZoomORUX2;
        private Label ZoomORUX2;
        private Label Clef2;
        private Panel Echelle3;
        private Label Clef3;
        private Label ZoomORUX3;
        private NumericUpDown CorZoomORUX3;
        private Panel Echelle4;
        private NumericUpDown CorZoomORUX4;
        private Label ZoomORUX4;
        private Label Clef4;
        private Button ChoixFichierORUX;
        private Button Appliquer;
        private Button Quitter;
        private ToolTip ToolTip1;
        private OpenFileDialog ChercheFichierORUX;
        private Panel Support;
    }
}