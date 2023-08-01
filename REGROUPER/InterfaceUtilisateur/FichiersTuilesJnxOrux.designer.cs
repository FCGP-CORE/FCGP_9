using static FCGP.Commun;

namespace FCGP
{
    internal partial class FichiersTuilesJnxOrux : Form
    {

        internal FichiersTuilesJnxOrux()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôle simples
            CorZoomORUX0.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomJNX0.ValueChanged += CorZoomJNX_ValueChanged;
            Include0.CheckedChanged += Include_CheckedChanged;
            CorZoomORUX1.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomJNX1.ValueChanged += CorZoomJNX_ValueChanged;
            Include1.CheckedChanged += Include_CheckedChanged;
            CorZoomORUX2.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomJNX2.ValueChanged += CorZoomJNX_ValueChanged;
            Include2.CheckedChanged += Include_CheckedChanged;
            CorZoomJNX3.ValueChanged += CorZoomJNX_ValueChanged;
            CorZoomORUX3.ValueChanged += CorZoomORUX_ValueChanged;
            Include3.CheckedChanged += Include_CheckedChanged;
            CorZoomORUX4.ValueChanged += CorZoomORUX_ValueChanged;
            CorZoomJNX4.ValueChanged += CorZoomJNX_ValueChanged;
            Include4.CheckedChanged += Include_CheckedChanged;
            CorLargeur.ValueChanged += CorDimension_ValueChanged;
            CorLargeur.MouseDoubleClick += CorDimension_MouseDoubleClick;
            CorHauteur.ValueChanged += CorDimension_ValueChanged;
            CorHauteur.MouseDoubleClick += CorDimension_MouseDoubleClick;
            CorPT0X.ValueChanged += CorLocation_ValueChanged;
            CorPT0X.MouseDoubleClick += CorLocation_MouseDoubleClick;
            CorPT0Y.ValueChanged += CorLocation_ValueChanged;
            CorPT0Y.MouseDoubleClick += CorLocation_MouseDoubleClick;
            N256.ValueChanged += N256_ValueChanged;
            FichierJNX.CheckedChanged += FichierJNX_CheckedChanged;
            ChoixTailleTampon.SelectedIndexChanged += ChoixTailleTampon_SelectedIndexChanged;
            Lancer.Click += Lancer_Click;
            // formulaire
            Load += FichiersTuilesJnxOrux_Load;
            FormClosed += FichierTuilesJnxOrux_FormClosed;
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
            #region Constructeurs
            components = new Container();
            var resources = new ComponentResourceManager(typeof(FichiersTuilesJnxOrux));
            Echelle0 = new Panel();
            CorZoomORUX0 = new NumericUpDown();
            ZoomORUX0 = new Label();
            CorZoomJNX0 = new NumericUpDown();
            ZoomJNX0 = new Label();
            Clef0 = new Label();
            Include0 = new CheckBox();
            L005 = new Label();
            L006 = new Label();
            L007 = new Label();
            L008 = new Label();
            Echelle1 = new Panel();
            CorZoomORUX1 = new NumericUpDown();
            ZoomORUX1 = new Label();
            CorZoomJNX1 = new NumericUpDown();
            ZoomJNX1 = new Label();
            Clef1 = new Label();
            Include1 = new CheckBox();
            Echelle2 = new Panel();
            CorZoomORUX2 = new NumericUpDown();
            ZoomORUX2 = new Label();
            CorZoomJNX2 = new NumericUpDown();
            ZoomJNX2 = new Label();
            Clef2 = new Label();
            Include2 = new CheckBox();
            Echelle3 = new Panel();
            Include3 = new CheckBox();
            Clef3 = new Label();
            ZoomJNX3 = new Label();
            CorZoomJNX3 = new NumericUpDown();
            ZoomORUX3 = new Label();
            CorZoomORUX3 = new NumericUpDown();
            Echelle4 = new Panel();
            CorZoomORUX4 = new NumericUpDown();
            ZoomORUX4 = new Label();
            CorZoomJNX4 = new NumericUpDown();
            ZoomJNX4 = new Label();
            Clef4 = new Label();
            Include4 = new CheckBox();
            L003 = new Label();
            L004 = new Label();
            L010 = new Label();
            L009 = new Label();
            L002 = new Label();
            ToolTip1 = new ToolTip(components);
            CorLargeur = new NumericUpDown();
            CorHauteur = new NumericUpDown();
            CorPT0X = new NumericUpDown();
            CorPT0Y = new NumericUpDown();
            PT2Reel = new Label();
            PT0Reel = new Label();
            N256 = new NumericUpDown();
            Poids = new Label();
            FichierJNX = new CheckBox();
            FichierORUX = new CheckBox();
            Lancer = new Button();
            Quitter = new Button();
            NomCarte = new TextBox();
            NbTuileMaxX = new Label();
            ChoixTailleTampon = new ComboBox();
            NbTuilesToltal = new Label();
            LPT0X = new Label();
            LPT0Y = new Label();
            LLARGEUR = new Label();
            LHAUTEUR = new Label();
            L001 = new Label();
            L011 = new Label();
            L018 = new Label();
            L014 = new Label();
            LargeurMaxi = new Label();
            HauteurMaxi = new Label();
            L015 = new Label();
            L029 = new Label();
            L028 = new Label();
            L021 = new Label();
            L027 = new Label();
            L020 = new Label();
            L019 = new Label();
            L023 = new Label();
            L022 = new Label();
            L024 = new Label();
            PT0X = new Label();
            L012 = new Label();
            PT0Y = new Label();
            L013 = new Label();
            L017 = new Label();
            L016 = new Label();
            L025 = new Label();
            L026 = new Label();
            Information = new Label();
            Support = new Panel();
            Echelle0.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX0).BeginInit();
            ((ISupportInitialize)CorZoomJNX0).BeginInit();
            Echelle1.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX1).BeginInit();
            ((ISupportInitialize)CorZoomJNX1).BeginInit();
            Echelle2.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX2).BeginInit();
            ((ISupportInitialize)CorZoomJNX2).BeginInit();
            Echelle3.SuspendLayout();
            ((ISupportInitialize)CorZoomJNX3).BeginInit();
            ((ISupportInitialize)CorZoomORUX3).BeginInit();
            Echelle4.SuspendLayout();
            ((ISupportInitialize)CorZoomORUX4).BeginInit();
            ((ISupportInitialize)CorZoomJNX4).BeginInit();
            ((ISupportInitialize)CorLargeur).BeginInit();
            ((ISupportInitialize)CorHauteur).BeginInit();
            ((ISupportInitialize)CorPT0X).BeginInit();
            ((ISupportInitialize)CorPT0Y).BeginInit();
            ((ISupportInitialize)N256).BeginInit();
            Support.SuspendLayout();
            SuspendLayout();
            #endregion
            #region Echelles
            #region Entête Echelles
            // 
            // L001
            // 
            L001.BackColor = Color.Transparent;
            L001.BorderStyle = BorderStyle.FixedSingle;
            L001.Location = new Point(6, 6);
            L001.Name = "L001";
            L001.Size = new Size(480, 23);
            L001.TabIndex = 12;
            L001.Text = "Niveaux de détail des fichiers JNX et ORUX";
            L001.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L002
            // 
            L002.BackColor = Color.Transparent;
            L002.BorderStyle = BorderStyle.FixedSingle;
            L002.Location = new Point(6, 28);
            L002.Name = "L002";
            L002.Size = new Size(134, 21);
            L002.TabIndex = 11;
            L002.Text = "Echelles";
            L002.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L003
            // 
            L003.BackColor = Color.Transparent;
            L003.BorderStyle = BorderStyle.FixedSingle;
            L003.Location = new Point(139, 28);
            L003.Name = "L003";
            L003.Size = new Size(174, 21);
            L003.TabIndex = 7;
            L003.Text = "Indice affichage JNX";
            L003.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L004
            // 
            L004.BackColor = Color.Transparent;
            L004.BorderStyle = BorderStyle.FixedSingle;
            L004.Location = new Point(312, 28);
            L004.Name = "L004";
            L004.Size = new Size(174, 21);
            L004.TabIndex = 10;
            L004.Text = "Indice affichage ORUX";
            L004.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L005
            // 
            L005.BackColor = Color.Transparent;
            L005.BorderStyle = BorderStyle.FixedSingle;
            L005.Location = new Point(6, 48);
            L005.Name = "L005";
            L005.Size = new Size(50, 21);
            L005.TabIndex = 1;
            L005.Text = "Inclus";
            L005.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L005, "Si la case est cochée, l'échellesera intégrée au fichiers tuiles");
            // 
            // L006
            // 
            L006.BackColor = Color.Transparent;
            L006.BorderStyle = BorderStyle.FixedSingle;
            L006.Location = new Point(55, 48);
            L006.Name = "L006";
            L006.Size = new Size(85, 21);
            L006.TabIndex = 2;
            L006.Text = "Clef";
            L006.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L006, "Clef de l'échelle. Classé de la plus grande échelle à la plus petite");
            // 
            // L007
            // 
            L007.BackColor = Color.Transparent;
            L007.BorderStyle = BorderStyle.FixedSingle;
            L007.Location = new Point(139, 48);
            L007.Name = "L007";
            L007.Size = new Size(68, 21);
            L007.TabIndex = 3;
            L007.Text = "Base";
            L007.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L007, resources.GetString("Label3.ToolTip"));
            // 
            // L008
            // 
            L008.BackColor = Color.Transparent;
            L008.BorderStyle = BorderStyle.FixedSingle;
            L008.Location = new Point(206, 48);
            L008.Name = "L008";
            L008.Size = new Size(107, 21);
            L008.TabIndex = 4;
            L008.Text = "Modifié";
            L008.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L008, resources.GetString("Label4.ToolTip"));
            // 
            // L009
            // 
            L009.BackColor = Color.Transparent;
            L009.BorderStyle = BorderStyle.FixedSingle;
            L009.Location = new Point(312, 48);
            L009.Name = "L009";
            L009.Size = new Size(68, 21);
            L009.TabIndex = 8;
            L009.Text = "Base";
            L009.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L009, resources.GetString("Label31.ToolTip"));
            // 
            // L010
            // 
            L010.BackColor = Color.Transparent;
            L010.BorderStyle = BorderStyle.FixedSingle;
            L010.Location = new Point(379, 48);
            L010.Name = "L010";
            L010.Size = new Size(107, 21);
            L010.TabIndex = 9;
            L010.Text = "Modifié";
            L010.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(L010, "Valeur du zoom ORUX corrigé (pas de 1)" + CrLf + 
                                      "Valeur de transition" + CrLf + 
                                      "1 niveau de différence --> 75% -150%" + CrLf + 
                                      "2 niveaux de différence --> 50% - 200%" + CrLf + 
                                      "3 niveaux de différence --> 35% - 280%");
            #endregion
            #region Echelle0
            // 
            // Echelle0
            // 
            Echelle0.BackColor = Color.Transparent;
            Echelle0.BorderStyle = BorderStyle.FixedSingle;
            Echelle0.Controls.Add(CorZoomORUX0);
            Echelle0.Controls.Add(ZoomORUX0);
            Echelle0.Controls.Add(CorZoomJNX0);
            Echelle0.Controls.Add(ZoomJNX0);
            Echelle0.Controls.Add(Clef0);
            Echelle0.Controls.Add(Include0);
            Echelle0.Enabled = false;
            Echelle0.Location = new Point(6, 68);
            Echelle0.Margin = new Padding(0);
            Echelle0.Name = "Echelle0";
            Echelle0.Size = new Size(480, 24);
            Echelle0.TabIndex = 0;
            // 
            // Include0
            // 
            Include0.AutoSize = true;
            Include0.Location = new Point(17, 5);
            Include0.Name = "Include0";
            Include0.Size = new Size(16, 16);
            Include0.TabIndex = 0;
            ToolTip1.SetToolTip(Include0, "Voir l'aide sur Inclus");
            Include0.UseVisualStyleBackColor = true;
            // 
            // Clef0
            // 
            Clef0.BorderStyle = BorderStyle.FixedSingle;
            Clef0.Location = new Point(48, -1);
            Clef0.Name = "Clef0";
            Clef0.Size = new Size(85, 24);
            Clef0.TabIndex = 1;
            Clef0.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef0, "Clef du niveau de détail");
            // 
            // ZoomJNX0
            // 
            ZoomJNX0.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX0.Location = new Point(132, -1);
            ZoomJNX0.Name = "ZoomJNX0";
            ZoomJNX0.Size = new Size(68, 24);
            ZoomJNX0.TabIndex = 2;
            ZoomJNX0.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX0, "Voir l'aide sur Zoom JNX");
            // 
            // CorZoomJNX0
            // 
            CorZoomJNX0.BackColor = Color.White;
            CorZoomJNX0.BorderStyle = BorderStyle.None;
            CorZoomJNX0.DecimalPlaces = 2;
            CorZoomJNX0.Increment = 0.25m;
            CorZoomJNX0.Location = new Point(200, 0);
            CorZoomJNX0.Name = "CorZoomJNX0";
            CorZoomJNX0.ReadOnly = true;
            CorZoomJNX0.Size = new Size(105, 23);
            CorZoomJNX0.TabIndex = 3;
            CorZoomJNX0.Tag = "0";
            CorZoomJNX0.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX0, "Voir l'aide sur Correction Zoom JNX");
            // 
            // ZoomORUX0
            // 
            ZoomORUX0.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX0.Location = new Point(305, -1);
            ZoomORUX0.Name = "ZoomORUX0";
            ZoomORUX0.Size = new Size(68, 24);
            ZoomORUX0.TabIndex = 4;
            ZoomORUX0.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX0, "Voir l'aide sur lZoom ORUX");
            // 
            // CorZoomORUX0
            // 
            CorZoomORUX0.BackColor = Color.White;
            CorZoomORUX0.BorderStyle = BorderStyle.None;
            CorZoomORUX0.Location = new Point(374, 0);
            CorZoomORUX0.Name = "CorZoomORUX0";
            CorZoomORUX0.ReadOnly = true;
            CorZoomORUX0.Size = new Size(104, 23);
            CorZoomORUX0.TabIndex = 5;
            CorZoomORUX0.Tag = "0";
            CorZoomORUX0.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX0, "Voir l'aide sur Correction Zoom ORUX");
            #endregion
            #region Echelle1
            // 
            // Echelle1
            // 
            Echelle1.BackColor = Color.Transparent;
            Echelle1.BorderStyle = BorderStyle.FixedSingle;
            Echelle1.Controls.Add(CorZoomORUX1);
            Echelle1.Controls.Add(ZoomORUX1);
            Echelle1.Controls.Add(CorZoomJNX1);
            Echelle1.Controls.Add(ZoomJNX1);
            Echelle1.Controls.Add(Clef1);
            Echelle1.Controls.Add(Include1);
            Echelle1.Enabled = false;
            Echelle1.Location = new Point(6, 91);
            Echelle1.Margin = new Padding(0);
            Echelle1.Name = "Echelle1";
            Echelle1.Size = new Size(480, 24);
            Echelle1.TabIndex = 1;
            // 
            // Include1
            // 
            Include1.AutoSize = true;
            Include1.BackColor = Color.Transparent;
            Include1.Location = new Point(17, 5);
            Include1.Name = "Include1";
            Include1.Size = new Size(16, 16);
            Include1.TabIndex = 0;
            ToolTip1.SetToolTip(Include1, "Voir l'aide sur Inclus");
            Include1.UseVisualStyleBackColor = true;
            // 
            // Clef1
            // 
            Clef1.BorderStyle = BorderStyle.FixedSingle;
            Clef1.Location = new Point(48, -1);
            Clef1.Name = "Clef1";
            Clef1.Size = new Size(85, 24);
            Clef1.TabIndex = 1;
            Clef1.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef1, "Clef du niveau de détail");
            // 
            // ZoomJNX1
            // 
            ZoomJNX1.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX1.Location = new Point(132, -1);
            ZoomJNX1.Name = "ZoomJNX1";
            ZoomJNX1.Size = new Size(68, 24);
            ZoomJNX1.TabIndex = 2;
            ZoomJNX1.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX1, "Voir l'aide sur Zoom JNX");
            // 
            // CorZoomJNX1
            // 
            CorZoomJNX1.BackColor = Color.White;
            CorZoomJNX1.BorderStyle = BorderStyle.None;
            CorZoomJNX1.DecimalPlaces = 2;
            CorZoomJNX1.Increment = 0.25m;
            CorZoomJNX1.Location = new Point(200, 0);
            CorZoomJNX1.Name = "CorZoomJNX1";
            CorZoomJNX1.ReadOnly = true;
            CorZoomJNX1.Size = new Size(105, 23);
            CorZoomJNX1.TabIndex = 3;
            CorZoomJNX1.Tag = "1";
            CorZoomJNX1.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX1, "Voir l'aide sur Correction Zoom JNX");
            // 
            // ZoomORUX1
            // 
            ZoomORUX1.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX1.Location = new Point(305, -1);
            ZoomORUX1.Name = "ZoomORUX1";
            ZoomORUX1.Size = new Size(68, 24);
            ZoomORUX1.TabIndex = 4;
            ZoomORUX1.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX1, "Voir l'aide sur Zoom ORUX");
            // 
            // CorZoomORUX1
            // 
            CorZoomORUX1.BackColor = Color.White;
            CorZoomORUX1.BorderStyle = BorderStyle.None;
            CorZoomORUX1.Location = new Point(374, 0);
            CorZoomORUX1.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            CorZoomORUX1.Name = "CorZoomORUX1";
            CorZoomORUX1.ReadOnly = true;
            CorZoomORUX1.Size = new Size(104, 23);
            CorZoomORUX1.TabIndex = 5;
            CorZoomORUX1.Tag = "1";
            CorZoomORUX1.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX1, "Voir l'aide sur Correction Zoom ORUX");
            #endregion
            #region Echelle2
            // 
            // Echelle2
            // 
            Echelle2.BackColor = Color.Transparent;
            Echelle2.BorderStyle = BorderStyle.FixedSingle;
            Echelle2.Controls.Add(CorZoomORUX2);
            Echelle2.Controls.Add(ZoomORUX2);
            Echelle2.Controls.Add(CorZoomJNX2);
            Echelle2.Controls.Add(ZoomJNX2);
            Echelle2.Controls.Add(Clef2);
            Echelle2.Controls.Add(Include2);
            Echelle2.Enabled = false;
            Echelle2.Location = new Point(6, 114);
            Echelle2.Margin = new Padding(0);
            Echelle2.Name = "Echelle2";
            Echelle2.Size = new Size(480, 24);
            Echelle2.TabIndex = 2;
            // 
            // Include2
            // 
            Include2.AutoSize = true;
            Include2.Location = new Point(17, 5);
            Include2.Name = "Include2";
            Include2.Size = new Size(16, 16);
            Include2.TabIndex = 0;
            ToolTip1.SetToolTip(Include2, "Voir l'aide sur Inclus");
            Include2.UseVisualStyleBackColor = true;
            // 
            // Clef2
            // 
            Clef2.BorderStyle = BorderStyle.FixedSingle;
            Clef2.Location = new Point(48, -1);
            Clef2.Name = "Clef2";
            Clef2.Size = new Size(85, 24);
            Clef2.TabIndex = 1;
            Clef2.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef2, "Clef du niveau de détail");
            // 
            // ZoomJNX2
            // 
            ZoomJNX2.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX2.Location = new Point(132, -1);
            ZoomJNX2.Name = "ZoomJNX2";
            ZoomJNX2.Size = new Size(68, 24);
            ZoomJNX2.TabIndex = 2;
            ZoomJNX2.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(ZoomJNX2, "Voir l'aide sur Zoom JNX");
            // 
            // CorZoomJNX2
            // 
            CorZoomJNX2.BackColor = Color.White;
            CorZoomJNX2.BorderStyle = BorderStyle.None;
            CorZoomJNX2.DecimalPlaces = 2;
            CorZoomJNX2.Increment = 0.25m;
            CorZoomJNX2.Location = new Point(200, 0);
            CorZoomJNX2.Name = "CorZoomJNX2";
            CorZoomJNX2.ReadOnly = true;
            CorZoomJNX2.Size = new Size(105, 23);
            CorZoomJNX2.TabIndex = 3;
            CorZoomJNX2.Tag = "2";
            CorZoomJNX2.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX2, "Voir l'aide sur Correction Zoom JNX");
            // 
            // ZoomORUX2
            // 
            ZoomORUX2.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX2.Location = new Point(305, -1);
            ZoomORUX2.Name = "ZoomORUX2";
            ZoomORUX2.Size = new Size(68, 24);
            ZoomORUX2.TabIndex = 4;
            ZoomORUX2.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(ZoomORUX2, "Voir l'aide sur Zoom ORUX");
            // 
            // CorZoomORUX2
            // 
            CorZoomORUX2.BackColor = Color.White;
            CorZoomORUX2.BorderStyle = BorderStyle.None;
            CorZoomORUX2.Location = new Point(374, 0);
            CorZoomORUX2.Name = "CorZoomORUX2";
            CorZoomORUX2.ReadOnly = true;
            CorZoomORUX2.Size = new Size(104, 23);
            CorZoomORUX2.TabIndex = 5;
            CorZoomORUX2.Tag = "2";
            CorZoomORUX2.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX2, "Voir l'aide sur Correction Zoom ORUX");
            #endregion
            #region Echelle3
            // 
            // Echelle3
            // 
            Echelle3.BackColor = Color.Transparent;
            Echelle3.BorderStyle = BorderStyle.FixedSingle;
            Echelle3.Controls.Add(Include3);
            Echelle3.Controls.Add(Clef3);
            Echelle3.Controls.Add(ZoomJNX3);
            Echelle3.Controls.Add(CorZoomJNX3);
            Echelle3.Controls.Add(ZoomORUX3);
            Echelle3.Controls.Add(CorZoomORUX3);
            Echelle3.Enabled = false;
            Echelle3.Location = new Point(6, 137);
            Echelle3.Margin = new Padding(0);
            Echelle3.Name = "Echelle3";
            Echelle3.Size = new Size(480, 24);
            Echelle3.TabIndex = 3;
            // 
            // Include3
            // 
            Include3.AutoSize = true;
            Include3.Location = new Point(17, 5);
            Include3.Name = "Include3";
            Include3.Size = new Size(16, 16);
            Include3.TabIndex = 0;
            ToolTip1.SetToolTip(Include3, "Voir l'aide sur Inclus");
            Include3.UseVisualStyleBackColor = true;
            // 
            // Clef3
            // 
            Clef3.BorderStyle = BorderStyle.FixedSingle;
            Clef3.Location = new Point(48, -1);
            Clef3.Name = "Clef3";
            Clef3.Size = new Size(85, 24);
            Clef3.TabIndex = 1;
            Clef3.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef3, "Clef du niveau de détail");
            // 
            // ZoomJNX3
            // 
            ZoomJNX3.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX3.Location = new Point(132, -1);
            ZoomJNX3.Name = "ZoomJNX3";
            ZoomJNX3.Size = new Size(68, 24);
            ZoomJNX3.TabIndex = 2;
            ZoomJNX3.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX3, "Voir l'aide sur Zoom JNX");
            // 
            // CorZoomJNX3
            // 
            CorZoomJNX3.BackColor = Color.White;
            CorZoomJNX3.BorderStyle = BorderStyle.None;
            CorZoomJNX3.DecimalPlaces = 2;
            CorZoomJNX3.Increment = 0.25m;
            CorZoomJNX3.Location = new Point(200, 0);
            CorZoomJNX3.Name = "CorZoomJNX3";
            CorZoomJNX3.ReadOnly = true;
            CorZoomJNX3.Size = new Size(105, 23);
            CorZoomJNX3.TabIndex = 3;
            CorZoomJNX3.Tag = "3";
            CorZoomJNX3.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX3, "Voir l'aide sur Correction Zoom JNX");
            // 
            // ZoomORUX3
            // 
            ZoomORUX3.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX3.Location = new Point(305, -1);
            ZoomORUX3.Name = "ZoomORUX3";
            ZoomORUX3.Size = new Size(68, 24);
            ZoomORUX3.TabIndex = 4;
            ZoomORUX3.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX3, "Voir l'aide sur Zoom ORUX");
            // 
            // CorZoomORUX3
            // 
            CorZoomORUX3.BackColor = Color.White;
            CorZoomORUX3.BorderStyle = BorderStyle.None;
            CorZoomORUX3.Location = new Point(374, 0);
            CorZoomORUX3.Name = "CorZoomORUX3";
            CorZoomORUX3.ReadOnly = true;
            CorZoomORUX3.Size = new Size(104, 23);
            CorZoomORUX3.TabIndex = 5;
            CorZoomORUX3.Tag = "3";
            CorZoomORUX3.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX3, "Voir l'aide sur Correction Zoom ORUX");
            #endregion
            #region Echelle4
            // 
            // Echelle4
            // 
            Echelle4.BackColor = Color.Transparent;
            Echelle4.BorderStyle = BorderStyle.FixedSingle;
            Echelle4.Controls.Add(CorZoomORUX4);
            Echelle4.Controls.Add(ZoomORUX4);
            Echelle4.Controls.Add(CorZoomJNX4);
            Echelle4.Controls.Add(ZoomJNX4);
            Echelle4.Controls.Add(Clef4);
            Echelle4.Controls.Add(Include4);
            Echelle4.Enabled = false;
            Echelle4.Location = new Point(6, 160);
            Echelle4.Margin = new Padding(0);
            Echelle4.Name = "Echelle4";
            Echelle4.Size = new Size(480, 24);
            Echelle4.TabIndex = 4;
            // 
            // Include4
            // 
            Include4.AutoSize = true;
            Include4.Location = new Point(17, 5);
            Include4.Name = "Include4";
            Include4.Size = new Size(16, 16);
            Include4.TabIndex = 0;
            ToolTip1.SetToolTip(Include4, "Voir l'aide sur Inclus");
            Include4.UseVisualStyleBackColor = true;
            // 
            // Clef4
            // 
            Clef4.BorderStyle = BorderStyle.FixedSingle;
            Clef4.Location = new Point(48, -1);
            Clef4.Name = "Clef4";
            Clef4.Size = new Size(85, 24);
            Clef4.TabIndex = 1;
            Clef4.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Clef4, "Clef du niveau de détail");
            // 
            // ZoomJNX4
            // 
            ZoomJNX4.BorderStyle = BorderStyle.FixedSingle;
            ZoomJNX4.Location = new Point(132, -1);
            ZoomJNX4.Name = "ZoomJNX4";
            ZoomJNX4.Size = new Size(68, 24);
            ZoomJNX4.TabIndex = 2;
            ZoomJNX4.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomJNX4, "Voir l'aide sur Zoom JNX");
            // 
            // CorZoomJNX4
            // 
            CorZoomJNX4.BackColor = Color.White;
            CorZoomJNX4.BorderStyle = BorderStyle.None;
            CorZoomJNX4.DecimalPlaces = 2;
            CorZoomJNX4.Increment = 0.25m;
            CorZoomJNX4.Location = new Point(200, 0);
            CorZoomJNX4.Name = "CorZoomJNX4";
            CorZoomJNX4.ReadOnly = true;
            CorZoomJNX4.Size = new Size(105, 23);
            CorZoomJNX4.TabIndex = 3;
            CorZoomJNX4.Tag = "4";
            CorZoomJNX4.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomJNX4, "Voir l'aide sur Correction Zoom JNX");
            // 
            // ZoomORUX4
            // 
            ZoomORUX4.BorderStyle = BorderStyle.FixedSingle;
            ZoomORUX4.Location = new Point(305, -1);
            ZoomORUX4.Name = "ZoomORUX4";
            ZoomORUX4.Size = new Size(68, 24);
            ZoomORUX4.TabIndex = 4;
            ZoomORUX4.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(ZoomORUX4, "Voir l'aide sur Zoom ORUX");
            // 
            // CorZoomORUX4
            // 
            CorZoomORUX4.BackColor = Color.White;
            CorZoomORUX4.BorderStyle = BorderStyle.None;
            CorZoomORUX4.Location = new Point(374, 0);
            CorZoomORUX4.Name = "CorZoomORUX4";
            CorZoomORUX4.ReadOnly = true;
            CorZoomORUX4.Size = new Size(104, 23);
            CorZoomORUX4.TabIndex = 5;
            CorZoomORUX4.Tag = "4";
            CorZoomORUX4.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorZoomORUX4, "Voir l'aide sur Correction Zoom ORUX");
            #endregion
            #endregion
            #region Dimensions Fichier
            #region Entêtes
            // 
            // L011
            // 
            L011.BackColor = Color.Transparent;
            L011.BorderStyle = BorderStyle.FixedSingle;
            L011.Location = new Point(6, 189);
            L011.Name = "L011";
            L011.Size = new Size(480, 23);
            L011.TabIndex = 13;
            L011.Text = "Taille et position des fichiers JNX et ORUX";
            L011.TextAlign = ContentAlignment.MiddleCenter;
            #endregion
            #region Pt0
            // 
            // L012
            // 
            L012.BackColor = Color.Transparent;
            L012.BorderStyle = BorderStyle.FixedSingle;
            L012.Location = new Point(6, 211);
            L012.Name = "L012";
            L012.Size = new Size(285, 24);
            L012.TabIndex = 6;
            L012.Text = "PT0 X  en pixel de la plus grande échelle";
            L012.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LPT0X
            // 
            LPT0X.BackColor = Color.Transparent;
            LPT0X.BorderStyle = BorderStyle.FixedSingle;
            LPT0X.Location = new Point(290, 211);
            LPT0X.Name = "LPT0X";
            LPT0X.Size = new Size(90, 24);
            LPT0X.TabIndex = 6;
            LPT0X.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(LPT0X, "Voir l'aide sur Zoom ORUX");
            // 
            // PT0X
            // 
            PT0X.BackColor = Color.Transparent;
            PT0X.BorderStyle = BorderStyle.FixedSingle;
            PT0X.Location = new Point(379, 211);
            PT0X.Margin = new Padding(0);
            PT0X.Name = "PT0X";
            PT0X.Size = new Size(107, 24);
            PT0X.TabIndex = 17;
            // 
            // CorPT0X
            // 
            CorPT0X.BackColor = Color.White;
            CorPT0X.BorderStyle = BorderStyle.None;
            CorPT0X.ForeColor = Color.YellowGreen;
            CorPT0X.Location = new Point(380, 212);
            CorPT0X.Name = "CorPT0X";
            CorPT0X.Size = new Size(105, 23);
            CorPT0X.TabIndex = 7;
            CorPT0X.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorPT0X, resources.GetString("CorPT0X.ToolTip"));
            #endregion
            #region Pt2
            // 
            // L013
            // 
            L013.BackColor = Color.Transparent;
            L013.BorderStyle = BorderStyle.FixedSingle;
            L013.Location = new Point(6, 234);
            L013.Name = "L013";
            L013.Size = new Size(285, 24);
            L013.TabIndex = 6;
            L013.Text = "PT0 Y en pixel de la plus grande échelle";
            L013.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LPT0Y
            // 
            LPT0Y.BackColor = Color.Transparent;
            LPT0Y.BorderStyle = BorderStyle.FixedSingle;
            LPT0Y.Location = new Point(290, 234);
            LPT0Y.Name = "LPT0Y";
            LPT0Y.Size = new Size(90, 24);
            LPT0Y.TabIndex = 8;
            LPT0Y.TextAlign = ContentAlignment.TopCenter;
            ToolTip1.SetToolTip(LPT0Y, "Voir l'aide sur le label Zoom ORUX");
            // 
            // PT0Y
            // 
            PT0Y.BackColor = Color.Transparent;
            PT0Y.BorderStyle = BorderStyle.FixedSingle;
            PT0Y.Location = new Point(379, 234);
            PT0Y.Margin = new Padding(0);
            PT0Y.Name = "PT0Y";
            PT0Y.Size = new Size(107, 24);
            PT0Y.TabIndex = 18;
            // 
            // CorPT0Y
            // 
            CorPT0Y.BackColor = Color.White;
            CorPT0Y.BorderStyle = BorderStyle.None;
            CorPT0Y.ForeColor = Color.YellowGreen;
            CorPT0Y.Location = new Point(380, 235);
            CorPT0Y.Name = "CorPT0Y";
            CorPT0Y.Size = new Size(105, 23);
            CorPT0Y.TabIndex = 7;
            CorPT0Y.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorPT0Y, resources.GetString("CorPT0Y.ToolTip"));
            #endregion
            #region Largeur
            // 
            // L014
            // 
            L014.BackColor = Color.Transparent;
            L014.BorderStyle = BorderStyle.FixedSingle;
            L014.Location = new Point(6, 257);
            L014.Name = "L014";
            L014.Size = new Size(285, 24);
            L014.TabIndex = 6;
            L014.Text = "Largeur en pixel de la plus grande échelle";
            L014.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LLARGEUR
            // 
            LLARGEUR.BackColor = Color.Transparent;
            LLARGEUR.BorderStyle = BorderStyle.FixedSingle;
            LLARGEUR.Location = new Point(290, 257);
            LLARGEUR.Name = "LLARGEUR";
            LLARGEUR.Size = new Size(90, 24);
            LLARGEUR.TabIndex = 9;
            LLARGEUR.TextAlign = ContentAlignment.TopCenter;
            // 
            // LargeurMaxi
            // 
            LargeurMaxi.BackColor = Color.Transparent;
            LargeurMaxi.BorderStyle = BorderStyle.FixedSingle;
            LargeurMaxi.Location = new Point(379, 257);
            LargeurMaxi.Margin = new Padding(0);
            LargeurMaxi.Name = "LargeurMaxi";
            LargeurMaxi.Size = new Size(107, 24);
            LargeurMaxi.TabIndex = 15;

            // 
            // CorLargeur
            // 
            CorLargeur.BorderStyle = BorderStyle.None;
            CorLargeur.ForeColor = Color.YellowGreen;
            CorLargeur.Location = new Point(380, 258);
            CorLargeur.Name = "CorLargeur";
            CorLargeur.Size = new Size(105, 23);
            CorLargeur.TabIndex = 7;
            CorLargeur.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorLargeur, resources.GetString("CorLargeur.ToolTip"));
            #endregion
            #region Hauteur
            // 
            // L015
            // 
            L015.BackColor = Color.Transparent;
            L015.BorderStyle = BorderStyle.FixedSingle;
            L015.Location = new Point(6, 280);
            L015.Name = "L015";
            L015.Size = new Size(285, 24);
            L015.TabIndex = 6;
            L015.Text = "Hauteur en pixel de la plus grande échelle";
            L015.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LHAUTEUR
            // 
            LHAUTEUR.BorderStyle = BorderStyle.FixedSingle;
            LHAUTEUR.Location = new Point(290, 280);
            LHAUTEUR.Name = "LHAUTEUR";
            LHAUTEUR.Size = new Size(90, 24);
            LHAUTEUR.TabIndex = 10;
            LHAUTEUR.TextAlign = ContentAlignment.TopCenter;
            // 
            // HauteurMaxi
            // 
            HauteurMaxi.BackColor = Color.Transparent;
            HauteurMaxi.BorderStyle = BorderStyle.FixedSingle;
            HauteurMaxi.Location = new Point(379, 280);
            HauteurMaxi.Margin = new Padding(0);
            HauteurMaxi.Name = "HauteurMaxi";
            HauteurMaxi.Size = new Size(107, 24);
            HauteurMaxi.TabIndex = 16;
            // 
            // CorHauteur
            // 
            CorHauteur.BorderStyle = BorderStyle.None;
            CorHauteur.ForeColor = Color.YellowGreen;
            CorHauteur.Location = new Point(380, 281);
            CorHauteur.Name = "CorHauteur";
            CorHauteur.Size = new Size(105, 23);
            CorHauteur.TabIndex = 7;
            CorHauteur.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorHauteur, resources.GetString("CorHauteur.ToolTip"));
            #endregion
            #region Dimensions Réelles corrigées
            // 
            // L016
            // 
            L016.BackColor = Color.Transparent;
            L016.BorderStyle = BorderStyle.FixedSingle;
            L016.Location = new Point(6, 303);
            L016.Name = "L016";
            L016.Size = new Size(134, 23);
            L016.TabIndex = 34;
            L016.Text = "PT0 Réel corrigé";
            L016.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PT0Reel
            // 
            PT0Reel.BackColor = Color.Transparent;
            PT0Reel.BorderStyle = BorderStyle.FixedSingle;
            PT0Reel.Location = new Point(139, 303);
            PT0Reel.Name = "PT0Reel";
            PT0Reel.Size = new Size(347, 23);
            PT0Reel.TabIndex = 32;
            PT0Reel.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(PT0Reel, "Coin Nord Ouest de la sélection résultant des corrections" + CrLf + 
                                         "apportées en coordonnées réelles de géoréférencement.");
            // 
            // L017
            // 
            L017.BackColor = Color.Transparent;
            L017.BorderStyle = BorderStyle.FixedSingle;
            L017.Location = new Point(6, 325);
            L017.Name = "L017";
            L017.Size = new Size(134, 23);
            L017.TabIndex = 33;
            L017.Text = "PT2 Réel corrigé";
            L017.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PT2Reel
            // 
            PT2Reel.BackColor = Color.Transparent;
            PT2Reel.BorderStyle = BorderStyle.FixedSingle;
            PT2Reel.Location = new Point(139, 325);
            PT2Reel.Name = "PT2Reel";
            PT2Reel.Size = new Size(347, 23);
            PT2Reel.TabIndex = 8;
            PT2Reel.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(PT2Reel, "Coin Sud Est de la sélection résultant des corrections" + CrLf + 
                                         "apportées en coordonnées réelles de géoréférencement.");
            #endregion
            #endregion
            #region Création Fichier
            #region entête
            // 
            // L018
            // 
            L018.BackColor = Color.Transparent;
            L018.BorderStyle = BorderStyle.FixedSingle;
            L018.Location = new Point(6, 353);
            L018.Name = "L018";
            L018.Size = new Size(480, 23);
            L018.TabIndex = 14;
            L018.Text = "Création des fichiers JNX et ORUX";
            L018.TextAlign = ContentAlignment.MiddleCenter;
            #endregion
            #region Ligne Fichier
            // 
            // L019
            // 
            L019.BackColor = Color.Transparent;
            L019.BorderStyle = BorderStyle.FixedSingle;
            L019.Location = new Point(6, 375);
            L019.Name = "L019";
            L019.Size = new Size(160, 21);
            L019.TabIndex = 17;
            L019.Text = "Fichier JNX";
            L019.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L020
            // 
            L020.BackColor = Color.Transparent;
            L020.BorderStyle = BorderStyle.FixedSingle;
            L020.Location = new Point(165, 375);
            L020.Name = "L020";
            L020.Size = new Size(161, 21);
            L020.TabIndex = 18;
            L020.Text = "Fichier ORUX";
            L020.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L021
            // 
            L021.BackColor = Color.Transparent;
            L021.BorderStyle = BorderStyle.FixedSingle;
            L021.Location = new Point(325, 375);
            L021.Name = "L021";
            L021.Size = new Size(161, 21);
            L021.TabIndex = 7;
            L021.Text = "Tampon Support";
            L021.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L022
            // 
            L022.BackColor = Color.Transparent;
            L022.BorderStyle = BorderStyle.FixedSingle;
            L022.Location = new Point(6, 395);
            L022.Name = "L022";
            L022.Size = new Size(160, 23);
            L022.TabIndex = 20;
            L022.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FichierJNX
            // 
            FichierJNX.AutoSize = true;
            FichierJNX.Location = new Point(86, 400);
            FichierJNX.Name = "FichierJNX";
            FichierJNX.Size = new Size(15, 15);
            FichierJNX.TabIndex = 6;
            ToolTip1.SetToolTip(FichierJNX, "Génère le fichier JNX");
            FichierJNX.UseVisualStyleBackColor = true;
            // 
            // L023
            // 
            L023.BackColor = Color.Transparent;
            L023.BorderStyle = BorderStyle.FixedSingle;
            L023.Location = new Point(165, 395);
            L023.Name = "L023";
            L023.Size = new Size(161, 23);
            L023.TabIndex = 21;
            L023.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FichierORUX
            // 
            FichierORUX.AutoSize = true;
            FichierORUX.Location = new Point(239, 400);
            FichierORUX.Name = "FichierORUX";
            FichierORUX.Size = new Size(15, 15);
            FichierORUX.TabIndex = 23;
            ToolTip1.SetToolTip(FichierORUX, "Génère le fichier ORUX");
            FichierORUX.UseVisualStyleBackColor = true;
            // 
            // ChoixTailleTampon
            // 
            ChoixTailleTampon.FlatStyle = FlatStyle.System;
            ChoixTailleTampon.ItemHeight = 19;
            ChoixTailleTampon.Location = new Point(325, 391);
            ChoixTailleTampon.Margin = new Padding(0);
            ChoixTailleTampon.MaxDropDownItems = 3;
            ChoixTailleTampon.Name = "ChoixTailleTampon";
            ChoixTailleTampon.Size = new Size(161, 26);
            ChoixTailleTampon.TabIndex = 9;
            ToolTip1.SetToolTip(ChoixTailleTampon, "Influe sur le nb de tuiles maximum sur l'axe des X. " + CrLf + 
                                                   "Influe également sur la rapidité du traitement.");
            #endregion
            #region Ligne Nom
            // 
            // L024
            // 
            L024.BackColor = Color.Transparent;
            L024.BorderStyle = BorderStyle.FixedSingle;
            L024.Location = new Point(6, 417);
            L024.Name = "L024";
            L024.Size = new Size(320, 21);
            L024.TabIndex = 30;
            L024.Text = "Nom";
            L024.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L025
            // 
            L025.BackColor = Color.Transparent;
            L025.BorderStyle = BorderStyle.FixedSingle;
            L025.Location = new Point(325, 417);
            L025.Name = "L025";
            L025.Size = new Size(161, 21);
            L025.TabIndex = 35;
            L025.Text = "Dimensions Tuiles";
            L025.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NomCarte
            // 
            NomCarte.BackColor = SystemColors.Window;
            NomCarte.Location = new Point(6, 437);
            NomCarte.Name = "NomCarte";
            NomCarte.Size = new Size(320, 24);
            NomCarte.TabIndex = 39;
            ToolTip1.SetToolTip(NomCarte, "Nom des fichiers tuiles");
            // 
            // L026
            // 
            L026.BorderStyle = BorderStyle.FixedSingle;
            L026.Location = new Point(325, 437);
            L026.Name = "L026";
            L026.Size = new Size(161, 24);
            L026.TabIndex = 38;
            // 
            // N256
            // 
            N256.BorderStyle = BorderStyle.None;
            N256.BackColor = Color.White;
            N256.Location = new Point(326, 438);
            N256.Increment = 256m;
            N256.Maximum = 512m;
            N256.Minimum = 256m;
            N256.Value = 256m;
            N256.ReadOnly = true;
            N256.Name = "N256";
            N256.Size = new Size(159, 23);
            N256.TabIndex = 7;
            N256.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(N256, resources.GetString("N256.ToolTip"));
            #endregion
            #region Ligne Taille
            // 
            // L027
            // 
            L027.BackColor = Color.Transparent;
            L027.BorderStyle = BorderStyle.FixedSingle;
            L027.Location = new Point(6, 460);
            L027.Name = "L027";
            L027.Size = new Size(160, 21);
            L027.TabIndex = 6;
            L027.Text = "Poids Echelles";
            L027.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L028
            // 
            L028.BackColor = Color.Transparent;
            L028.BorderStyle = BorderStyle.FixedSingle;
            L028.Location = new Point(165, 460);
            L028.Name = "L028";
            L028.Size = new Size(161, 21);
            L028.TabIndex = 10;
            L028.Text = "NbTuiles Gde Echelle";
            L028.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L029
            // 
            L029.BackColor = Color.Transparent;
            L029.BorderStyle = BorderStyle.FixedSingle;
            L029.Location = new Point(325, 460);
            L029.Name = "L029";
            L029.Size = new Size(161, 21);
            L029.TabIndex = 12;
            L029.Text = "NbTuiles Axe X";
            L029.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Poids
            // 
            Poids.BackColor = Color.YellowGreen;
            Poids.BorderStyle = BorderStyle.FixedSingle;
            Poids.ForeColor = Color.Black;
            Poids.Location = new Point(6, 480);
            Poids.Name = "Poids";
            Poids.Size = new Size(160, 23);
            Poids.TabIndex = 6;
            Poids.Text = "0";
            Poids.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Poids, "Vert      --> OK pour le poids" + CrLf + 
                                       "Rouge  --> le poids total est trop important");
            // 
            // NbTuilesToltal
            // 
            NbTuilesToltal.BackColor = Color.YellowGreen;
            NbTuilesToltal.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesToltal.ForeColor = Color.Black;
            NbTuilesToltal.Location = new Point(165, 480);
            NbTuilesToltal.Name = "NbTuilesToltal";
            NbTuilesToltal.Size = new Size(161, 23);
            NbTuilesToltal.TabIndex = 11;
            NbTuilesToltal.Text = "0";
            NbTuilesToltal.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuilesToltal, "Vert     --> OK pour le poids et le nb de tuiles." + CrLf + 
                                                "Rouge --> le poids total est trop important");
            // 
            // NbTuileMaxX
            // 
            NbTuileMaxX.BackColor = Color.YellowGreen;
            NbTuileMaxX.BorderStyle = BorderStyle.FixedSingle;
            NbTuileMaxX.ForeColor = Color.Black;
            NbTuileMaxX.Location = new Point(325, 480);
            NbTuileMaxX.Name = "NbTuileMaxX";
            NbTuileMaxX.Size = new Size(161, 23);
            NbTuileMaxX.TabIndex = 8;
            NbTuileMaxX.Text = "0";
            NbTuileMaxX.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuileMaxX, "Vert     --> OK le nb de tuiles maxi sur l'axe des X n'est pas dépassé." + CrLf + 
                                             "Rouge --> le nb de tuiles maxi sur l'axe des X est dépassé.");
            #endregion
            #endregion
            #region Controles formulaire
            // 
            // Information
            // 
            Information.BackColor = Color.Transparent;
            Information.BorderStyle = BorderStyle.FixedSingle;
            Information.Location = new Point(6, 502);
            Information.Name = "Information";
            Information.Size = new Size(480, 63);
            Information.TabIndex = 40;
            // 
            // Support
            // 
            Support.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Controls.Add(NbTuileMaxX);
            Support.Controls.Add(NbTuilesToltal);
            Support.Controls.Add(L027);
            Support.Controls.Add(L029);
            Support.Controls.Add(Information);
            Support.Controls.Add(Poids);
            Support.Controls.Add(L028);
            Support.Controls.Add(NomCarte);
            Support.Controls.Add(N256);
            Support.Controls.Add(L026);
            Support.Controls.Add(L025);
            Support.Controls.Add(L021);
            Support.Controls.Add(L016);
            Support.Controls.Add(L017);
            Support.Controls.Add(PT0Reel);
            Support.Controls.Add(PT2Reel);
            Support.Controls.Add(ChoixTailleTampon);
            Support.Controls.Add(L012);
            Support.Controls.Add(LPT0X);
            Support.Controls.Add(CorPT0X);
            Support.Controls.Add(PT0X);
            Support.Controls.Add(L013);
            Support.Controls.Add(LPT0Y);
            Support.Controls.Add(CorPT0Y);
            Support.Controls.Add(PT0Y);
            Support.Controls.Add(L014);
            Support.Controls.Add(LLARGEUR);
            Support.Controls.Add(CorLargeur);
            Support.Controls.Add(LargeurMaxi);
            Support.Controls.Add(L015);
            Support.Controls.Add(LHAUTEUR);
            Support.Controls.Add(CorHauteur);
            Support.Controls.Add(HauteurMaxi);
            Support.Controls.Add(L024);
            Support.Controls.Add(FichierORUX);
            Support.Controls.Add(FichierJNX);
            Support.Controls.Add(L023);
            Support.Controls.Add(L022);
            Support.Controls.Add(L020);
            Support.Controls.Add(L019);
            Support.Controls.Add(L018);
            Support.Controls.Add(L011);
            Support.Controls.Add(L001);
            Support.Controls.Add(L002);
            Support.Controls.Add(L004);
            Support.Controls.Add(L010);
            Support.Controls.Add(L009);
            Support.Controls.Add(L003);
            Support.Controls.Add(L008);
            Support.Controls.Add(L007);
            Support.Controls.Add(L006);
            Support.Controls.Add(L005);
            Support.Controls.Add(Echelle0);
            Support.Controls.Add(Echelle1);
            Support.Controls.Add(Echelle2);
            Support.Controls.Add(Echelle3);
            Support.Controls.Add(Echelle4);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(492, 571);
            Support.TabIndex = 0;
            // 
            // Lancer
            // 
            Lancer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Lancer.Location = new Point(359, 577);
            Lancer.Name = "Lancer";
            Lancer.Size = new Size(124, 33);
            Lancer.TabIndex = 25;
            Lancer.Text = "Lancer la création";
            ToolTip1.SetToolTip(Lancer, "Lance la création des fichiers tuiles demandés avec la configuration choisie.");
            Lancer.UseVisualStyleBackColor = true;
            // 
            // Quitter
            // 
            Quitter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Quitter.DialogResult = DialogResult.Cancel;
            Quitter.Location = new Point(229, 577);
            Quitter.Name = "Quitter";
            Quitter.Size = new Size(124, 33);
            Quitter.TabIndex = 26;
            Quitter.Text = "Quitter";
            ToolTip1.SetToolTip(Quitter, "Ferme le formulaire sans la génération des fichiers tuiles");
            Quitter.UseVisualStyleBackColor = true;
            // 
            // FichierTuilesKMZ
            // 
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = Quitter;
            ClientSize = new Size(492, 616);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(Quitter);
            Controls.Add(Lancer);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "FichiersTuilesJnxOrux";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Création des Fichiers JNX et ORUX";
            Echelle0.ResumeLayout(false);
            Echelle0.PerformLayout();
            ((ISupportInitialize)CorZoomORUX0).EndInit();
            ((ISupportInitialize)CorZoomJNX0).EndInit();
            Echelle1.ResumeLayout(false);
            Echelle1.PerformLayout();
            ((ISupportInitialize)CorZoomORUX1).EndInit();
            ((ISupportInitialize)CorZoomJNX1).EndInit();
            Echelle2.ResumeLayout(false);
            Echelle2.PerformLayout();
            ((ISupportInitialize)CorZoomORUX2).EndInit();
            ((ISupportInitialize)CorZoomJNX2).EndInit();
            Echelle3.ResumeLayout(false);
            Echelle3.PerformLayout();
            ((ISupportInitialize)CorZoomJNX3).EndInit();
            ((ISupportInitialize)CorZoomORUX3).EndInit();
            Echelle4.ResumeLayout(false);
            Echelle4.PerformLayout();
            ((ISupportInitialize)CorZoomORUX4).EndInit();
            ((ISupportInitialize)CorZoomJNX4).EndInit();
            ((ISupportInitialize)CorLargeur).EndInit();
            ((ISupportInitialize)CorHauteur).EndInit();
            ((ISupportInitialize)CorPT0X).EndInit();
            ((ISupportInitialize)CorPT0Y).EndInit();
            ((ISupportInitialize)N256).EndInit();
            Support.ResumeLayout();
            ResumeLayout(false);
            PerformLayout();
            #endregion
        }
        #region Déclaration
        private Panel Echelle0;
        private Label Clef0;
        private CheckBox Include0;
        private NumericUpDown CorZoomJNX0;
        private Label ZoomJNX0;
        private Label L005;
        private Label L006;
        private Label L007;
        private Label L008;
        private Panel Echelle1;
        private NumericUpDown CorZoomJNX1;
        private Label ZoomJNX1;
        private Label Clef1;
        private CheckBox Include1;
        private Panel Echelle2;
        private NumericUpDown CorZoomJNX2;
        private Label ZoomJNX2;
        private Label Clef2;
        private CheckBox Include2;
        private Panel Echelle3;
        private NumericUpDown CorZoomJNX3;
        private Label ZoomJNX3;
        private Label Clef3;
        private CheckBox Include3;
        private Panel Echelle4;
        private NumericUpDown CorZoomJNX4;
        private Label ZoomJNX4;
        private Label Clef4;
        private CheckBox Include4;
        private NumericUpDown CorZoomORUX0;
        private Label ZoomORUX0;
        private Label L003;
        private NumericUpDown CorZoomORUX1;
        private Label ZoomORUX1;
        private NumericUpDown CorZoomORUX2;
        private Label ZoomORUX2;
        private NumericUpDown CorZoomORUX3;
        private Label ZoomORUX3;
        private NumericUpDown CorZoomORUX4;
        private Label ZoomORUX4;
        private Label L004;
        private Label L010;
        private Label L009;
        private ToolTip ToolTip1;
        private Label L002;
        private Label L001;
        private Label L011;
        private Label L018;
        private Label L014;
        private NumericUpDown CorLargeur;
        private Label LargeurMaxi;
        private Label HauteurMaxi;
        private Label L015;
        private NumericUpDown CorHauteur;
        private Label L027;
        private Label Poids;
        private Label L020;
        private Label L019;
        private Label L023;
        private Label L022;
        private CheckBox FichierJNX;
        private CheckBox FichierORUX;
        private Button Lancer;
        private Button Quitter;
        private Label L024;
        private Label PT0X;
        private Label L012;
        private NumericUpDown CorPT0X;
        private Label PT0Y;
        private Label L013;
        private NumericUpDown CorPT0Y;
        private Label PT2Reel;
        private Label PT0Reel;
        private Label L017;
        private Label L016;
        private Label L025;
        private NumericUpDown N256;
        private Label L026;
        private TextBox NomCarte;
        private Label Information;
        private ComboBox ChoixTailleTampon;
        private Label L021;
        private Label NbTuileMaxX;
        private Label L029;
        private Label L028;
        private Label NbTuilesToltal;
        private Label LLARGEUR;
        private Label LHAUTEUR;
        private Label LPT0X;
        private Label LPT0Y;
        private Panel Support;
        #endregion
    }
}