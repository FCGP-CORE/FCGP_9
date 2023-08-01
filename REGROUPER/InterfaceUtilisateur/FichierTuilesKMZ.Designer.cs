using static FCGP.Commun;

namespace FCGP
{
    internal partial class FichierTuilesKMZ : Form
    {

        internal FichierTuilesKMZ()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôle simples
            CorLargeur.ValueChanged += CorDimension_ValueChanged;
            CorLargeur.MouseDoubleClick += CorDimension_MouseDoubleClick;
            CorHauteur.ValueChanged += CorDimension_ValueChanged;
            CorHauteur.MouseDoubleClick += CorDimension_MouseDoubleClick;
            CorPT0X.ValueChanged += CorLocation_ValueChanged;
            CorPT0X.MouseDoubleClick += CorLocation_MouseDoubleClick;
            CorPT0Y.ValueChanged += CorLocation_ValueChanged;
            CorPT0Y.MouseDoubleClick += CorLocation_MouseDoubleClick;
            N512.ValueChanged += N512_ValueChanged;
            Lancer.Click += Lancer_Click;
            ChoixTailleTampon.SelectedIndexChanged += ChoixTailleTampon_SelectedIndexChanged;
            // formulaire
            Load += FichierTuilesKMZ_Load;
            FormClosed += FichierTuilesKMZ_FormClosed;
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
            var resources = new ComponentResourceManager(typeof(FichierTuilesKMZ));
            ToolTip1 = new ToolTip(components);
            CorLargeur = new NumericUpDown();
            CorHauteur = new NumericUpDown();
            CorPT0X = new NumericUpDown();
            CorPT0Y = new NumericUpDown();
            PT2Reel = new Label();
            PT0Reel = new Label();
            N512 = new NumericUpDown();
            NbTuilesKMZ = new Label();
            Lancer = new Button();
            Quitter = new Button();
            NomCarte = new TextBox();
            NbRangs = new Label();
            NbCols = new Label();
            LPT0X = new Label();
            LPT0Y = new Label();
            LLARGEUR = new Label();
            LHAUTEUR = new Label();
            L011 = new Label();
            L018 = new Label();
            L014 = new Label();
            LargeurMaxi = new Label();
            HauteurMaxi = new Label();
            L015 = new Label();
            L029 = new Label();
            L028 = new Label();
            L027 = new Label();
            L024 = new Label();
            PT0X = new Label();
            L012 = new Label();
            PT0Y = new Label();
            L013 = new Label();
            L017 = new Label();
            L016 = new Label();
            L030 = new Label();
            L025 = new Label();
            L026 = new Label();
            ChoixTailleTampon = new ComboBox();
            Information = new Label();
            Support = new Panel();
            ((ISupportInitialize)CorLargeur).BeginInit();
            ((ISupportInitialize)CorHauteur).BeginInit();
            ((ISupportInitialize)CorPT0X).BeginInit();
            ((ISupportInitialize)CorPT0Y).BeginInit();
            Support.SuspendLayout();
            SuspendLayout();
            #endregion
            #region Dimensions Fichier
            #region Entêtes
            // 
            // L011
            // 
            L011.BackColor = Color.Transparent;
            L011.BorderStyle = BorderStyle.FixedSingle;
            L011.Location = new Point(6, 6); // -183
            L011.Name = "L011";
            L011.Size = new Size(480, 23);
            L011.TabIndex = 13;
            L011.Text = "Taille et position du fichier KMZ";
            L011.TextAlign = ContentAlignment.MiddleCenter;
            #endregion
            #region Pt0X
            // 
            // L012
            // 
            L012.BackColor = Color.Transparent;
            L012.BorderStyle = BorderStyle.FixedSingle;
            L012.Location = new Point(6, 28);
            L012.Name = "L012";
            L012.Size = new Size(241, 24);
            L012.TabIndex = 6;
            L012.Text = "PT0 X  en pixel du fichier";
            L012.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LPT0X
            // 
            LPT0X.BackColor = Color.Transparent;
            LPT0X.BorderStyle = BorderStyle.FixedSingle;
            LPT0X.Location = new Point(246, 28);
            LPT0X.Name = "LPT0X";
            LPT0X.Size = new Size(121, 24);
            LPT0X.TabIndex = 6;
            LPT0X.TextAlign = ContentAlignment.TopCenter;
            // PT0X
            // 
            PT0X.BackColor = Color.Transparent;
            PT0X.BorderStyle = BorderStyle.FixedSingle;
            PT0X.Location = new Point(365, 28);
            PT0X.Margin = new Padding(0);
            PT0X.Name = "PT0X";
            PT0X.Size = new Size(121, 24);
            PT0X.TabIndex = 17;
            // 
            // CorPT0X
            // 
            CorPT0X.BackColor = Color.White;
            CorPT0X.BorderStyle = BorderStyle.None;
            CorPT0X.ForeColor = Color.YellowGreen;
            CorPT0X.Location = new Point(366, 29);
            CorPT0X.Name = "CorPT0X";
            CorPT0X.Size = new Size(119, 23);
            CorPT0X.TabIndex = 7;
            CorPT0X.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(CorPT0X, resources.GetString("CorPT0X.ToolTip"));
            #endregion
            #region Pt0Y
            // 
            // L013
            // 
            L013.BackColor = Color.Transparent;
            L013.BorderStyle = BorderStyle.FixedSingle;
            L013.Location = new Point(6, 51);
            L013.Name = "L013";
            L013.Size = new Size(241, 24);
            L013.TabIndex = 6;
            L013.Text = "PT0 Y en pixel du fichier";
            L013.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LPT0Y
            // 
            LPT0Y.BackColor = Color.Transparent;
            LPT0Y.BorderStyle = BorderStyle.FixedSingle;
            LPT0Y.Location = new Point(246, 51);
            LPT0Y.Name = "LPT0Y";
            LPT0Y.Size = new Size(121, 24);
            LPT0Y.TabIndex = 8;
            LPT0Y.TextAlign = ContentAlignment.TopCenter;
            // 
            // PT0Y
            // 
            PT0Y.BackColor = Color.Transparent;
            PT0Y.BorderStyle = BorderStyle.FixedSingle;
            PT0Y.Location = new Point(365, 51);
            PT0Y.Margin = new Padding(0);
            PT0Y.Name = "PT0Y";
            PT0Y.Size = new Size(121, 24);
            PT0Y.TabIndex = 18;
            // 
            // CorPT0Y
            // 
            CorPT0Y.BackColor = Color.White;
            CorPT0Y.BorderStyle = BorderStyle.None;
            CorPT0Y.ForeColor = Color.YellowGreen;
            CorPT0Y.Location = new Point(366, 52);
            CorPT0Y.Name = "CorPT0Y";
            CorPT0Y.Size = new Size(119, 23);
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
            L014.Location = new Point(6, 74);
            L014.Name = "L014";
            L014.Size = new Size(241, 24);
            L014.TabIndex = 6;
            L014.Text = "Largeur en pixel du fichier";
            L014.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LLARGEUR
            // 
            LLARGEUR.BackColor = Color.Transparent;
            LLARGEUR.BorderStyle = BorderStyle.FixedSingle;
            LLARGEUR.Location = new Point(246, 74);
            LLARGEUR.Name = "LLARGEUR";
            LLARGEUR.Size = new Size(121, 24);
            LLARGEUR.TabIndex = 9;
            LLARGEUR.TextAlign = ContentAlignment.TopCenter;
            // 
            // LargeurMaxi
            // 
            LargeurMaxi.BackColor = Color.Transparent;
            LargeurMaxi.BorderStyle = BorderStyle.FixedSingle;
            LargeurMaxi.Location = new Point(365, 74);
            LargeurMaxi.Margin = new Padding(0);
            LargeurMaxi.Name = "LargeurMaxi";
            LargeurMaxi.Size = new Size(121, 24);
            LargeurMaxi.TabIndex = 15;

            // 
            // CorLargeur
            // 
            CorLargeur.BorderStyle = BorderStyle.None;
            CorLargeur.ForeColor = Color.YellowGreen;
            CorLargeur.Location = new Point(366, 75);
            CorLargeur.Name = "CorLargeur";
            CorLargeur.Size = new Size(119, 23);
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
            L015.Location = new Point(6, 97);
            L015.Name = "L015";
            L015.Size = new Size(241, 24);
            L015.TabIndex = 6;
            L015.Text = "Hauteur en pixel du fichier";
            L015.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LHAUTEUR
            // 
            LHAUTEUR.BorderStyle = BorderStyle.FixedSingle;
            LHAUTEUR.Location = new Point(246, 97);
            LHAUTEUR.Name = "LHAUTEUR";
            LHAUTEUR.Size = new Size(121, 24);
            LHAUTEUR.TabIndex = 10;
            LHAUTEUR.TextAlign = ContentAlignment.TopCenter;
            // 
            // HauteurMaxi
            // 
            HauteurMaxi.BackColor = Color.Transparent;
            HauteurMaxi.BorderStyle = BorderStyle.FixedSingle;
            HauteurMaxi.Location = new Point(365, 97);
            HauteurMaxi.Margin = new Padding(0);
            HauteurMaxi.Name = "HauteurMaxi";
            HauteurMaxi.Size = new Size(121, 24);
            HauteurMaxi.TabIndex = 16;
            // 
            // CorHauteur
            // 
            CorHauteur.BorderStyle = BorderStyle.None;
            CorHauteur.ForeColor = Color.YellowGreen;
            CorHauteur.Location = new Point(366, 98);
            CorHauteur.Name = "CorHauteur";
            CorHauteur.Size = new Size(119, 23);
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
            L016.Location = new Point(6, 120);
            L016.Name = "L016";
            L016.Size = new Size(120, 23);
            L016.TabIndex = 34;
            L016.Text = "PT0 Réel corrigé";
            L016.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PT0Reel
            // 
            PT0Reel.BackColor = Color.Transparent;
            PT0Reel.BorderStyle = BorderStyle.FixedSingle;
            PT0Reel.Location = new Point(125, 120);
            PT0Reel.Name = "PT0Reel";
            PT0Reel.Size = new Size(361, 23);
            PT0Reel.TabIndex = 32;
            PT0Reel.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(PT0Reel, "Coin Nord Ouest de la sélection résultant des corrections" + CrLf + 
                                         "apportées en coordonnées réelles de géoréférencement.");
            // 
            // L017
            // 
            L017.BackColor = Color.Transparent;
            L017.BorderStyle = BorderStyle.FixedSingle;
            L017.Location = new Point(6, 142);
            L017.Name = "L017";
            L017.Size = new Size(120, 23);
            L017.TabIndex = 33;
            L017.Text = "PT2 Réel corrigé";
            L017.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PT2Reel
            // 
            PT2Reel.BackColor = Color.Transparent;
            PT2Reel.BorderStyle = BorderStyle.FixedSingle;
            PT2Reel.Location = new Point(125, 142);
            PT2Reel.Name = "PT2Reel";
            PT2Reel.Size = new Size(361, 23);
            PT2Reel.TabIndex = 8;
            PT2Reel.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(PT2Reel, "Coin Sud Est de la sélection résultant des corrections" + CrLf + "" +
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
            L018.Location = new Point(6, 170);
            L018.Name = "L018";
            L018.Size = new Size(480, 23);
            L018.TabIndex = 14;
            L018.Text = "Création du fichier KMZ";
            L018.TextAlign = ContentAlignment.MiddleCenter;
            #endregion
            #region Ligne Nom
            // 
            // L024
            // 
            L024.BackColor = Color.Transparent;
            L024.BorderStyle = BorderStyle.FixedSingle;
            L024.Location = new Point(6, 192);
            L024.Name = "L024";
            L024.Size = new Size(360, 21);
            L024.TabIndex = 30;
            L024.Text = "Nom";
            L024.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L025
            // 
            L025.BackColor = Color.Transparent;
            L025.BorderStyle = BorderStyle.FixedSingle;
            L025.Location = new Point(365, 192);
            L025.Name = "L025";
            L025.Size = new Size(121, 21);
            L025.TabIndex = 35;
            L025.Text = "Dimensions Tuiles";
            L025.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NomCarte
            // 
            NomCarte.BackColor = SystemColors.Window;
            NomCarte.Location = new Point(6, 212);
            NomCarte.Name = "NomCarte";
            NomCarte.Size = new Size(360, 24);
            NomCarte.TabIndex = 39;
            ToolTip1.SetToolTip(NomCarte, "Nom des fichiers tuiles");
            // 
            // L026
            // 
            L026.BorderStyle = BorderStyle.FixedSingle;
            L026.Location = new Point(365, 212);
            L026.Name = "L026";
            L026.Size = new Size(121, 24);
            L026.TabIndex = 38;
            // 
            // N512
            // 
            N512.BorderStyle = BorderStyle.None;
            N512.BackColor = Color.White;
            N512.Location = new Point(366, 213);
            N512.Increment = 512m;
            N512.Maximum = 1024m;
            N512.Minimum = 512m;
            N512.Value = 512m;
            N512.ReadOnly = true;
            N512.Name = "N256";
            N512.Size = new Size(119, 23);
            N512.TabIndex = 7;
            N512.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(N512, resources.GetString("N512.ToolTip"));
            #endregion
            #region Ligne Taille
            // 
            // L027
            // 
            L027.BackColor = Color.Transparent;
            L027.BorderStyle = BorderStyle.FixedSingle;
            L027.Location = new Point(6, 235);
            L027.Name = "L027";
            L027.Size = new Size(120, 21);
            L027.TabIndex = 6;
            L027.Text = "Nb Tuiles";
            L027.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L028
            // 
            L028.BackColor = Color.Transparent;
            L028.BorderStyle = BorderStyle.FixedSingle;
            L028.Location = new Point(125, 235);
            L028.Name = "L028";
            L028.Size = new Size(121, 21);
            L028.TabIndex = 10;
            L028.Text = "Nb Colonnes";
            L028.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L029
            // 
            L029.BackColor = Color.Transparent;
            L029.BorderStyle = BorderStyle.FixedSingle;
            L029.Location = new Point(245, 235);
            L029.Name = "L029";
            L029.Size = new Size(121, 21);
            L029.TabIndex = 12;
            L029.Text = "Nb Rangées";
            L029.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // L030
            // 
            L030.BackColor = Color.Transparent;
            L030.BorderStyle = BorderStyle.FixedSingle;
            L030.Location = new Point(365, 235);
            L030.Name = "L030";
            L030.Size = new Size(121, 21);
            L030.TabIndex = 7;
            L030.Text = "Tampon Support";
            L030.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // Poids
            // 
            NbTuilesKMZ.BackColor = Color.YellowGreen;
            NbTuilesKMZ.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesKMZ.ForeColor = Color.Black;
            NbTuilesKMZ.Location = new Point(6, 255);
            NbTuilesKMZ.Name = "Poids";
            NbTuilesKMZ.Size = new Size(120, 23);
            NbTuilesKMZ.TabIndex = 6;
            NbTuilesKMZ.Text = "0";
            NbTuilesKMZ.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuilesKMZ, "Vert      --> OK pour le poids" + CrLf + 
                                             "Rouge  --> le poids total est trop important");
            // 
            // NbColonnes
            // 
            NbCols.BackColor = Color.Transparent;
            NbCols.BorderStyle = BorderStyle.FixedSingle;
            NbCols.Location = new Point(125, 255);
            NbCols.Name = "NbColonnes";
            NbCols.Size = new Size(121, 23);
            NbCols.TabIndex = 11;
            NbCols.Text = "0";
            NbCols.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbCols, "Vert     --> OK pour le poids et le nb de tuiles." + CrLf + 
                                        "Rouge --> le poids total est trop important");
            // 
            // NbRangees
            // 
            NbRangs.BackColor = Color.Transparent;
            NbRangs.BorderStyle = BorderStyle.FixedSingle;
            NbRangs.Location = new Point(245, 255);
            NbRangs.Name = "NbRangees";
            NbRangs.Size = new Size(121, 23);
            NbRangs.TabIndex = 8;
            NbRangs.Text = "0";
            NbRangs.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbRangs, "Vert     --> OK le nb de tuiles maxi sur l'axe des X n'est pas dépassé." + CrLf + 
                                         "Rouge --> le nb de tuiles maxi sur l'axe des X est dépassé.");

            // 
            // ChoixTailleTampon
            // 
            ChoixTailleTampon.FlatStyle = FlatStyle.System;
            ChoixTailleTampon.ItemHeight = 19;
            ChoixTailleTampon.Location = new Point(365, 251);
            ChoixTailleTampon.Margin = new Padding(0);
            ChoixTailleTampon.MaxDropDownItems = 3;
            ChoixTailleTampon.Name = "ChoixTailleTampon";
            ChoixTailleTampon.Size = new Size(121, 26);
            ChoixTailleTampon.TabIndex = 9;
            ToolTip1.SetToolTip(ChoixTailleTampon, "Influe sur le nb de tuiles maximum sur l'axe des X. " + CrLf + 
                                                   "Influe également sur la rapidité du traitement.");

            #endregion
            #endregion
            #region Controles formulaire
            // 
            // Information
            // 
            Information.BackColor = Color.Transparent;
            Information.BorderStyle = BorderStyle.FixedSingle;
            Information.Location = new Point(6, 277);
            Information.Name = "Information";
            Information.Size = new Size(480, 63);
            Information.TabIndex = 40;
            // 
            // Support
            // 
            Support.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Controls.Add(L030);
            Support.Controls.Add(ChoixTailleTampon);
            Support.Controls.Add(NbRangs);
            Support.Controls.Add(NbCols);
            Support.Controls.Add(L027);
            Support.Controls.Add(L029);
            Support.Controls.Add(Information);
            Support.Controls.Add(NbTuilesKMZ);
            Support.Controls.Add(L028);
            Support.Controls.Add(NomCarte);
            Support.Controls.Add(N512);
            Support.Controls.Add(L026);
            Support.Controls.Add(L025);
            Support.Controls.Add(L016);
            Support.Controls.Add(L017);
            Support.Controls.Add(PT0Reel);
            Support.Controls.Add(PT2Reel);
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
            Support.Controls.Add(L018);
            Support.Controls.Add(L011);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(492, 346);
            Support.TabIndex = 0;
            // 
            // Lancer
            // 
            Lancer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Lancer.Location = new Point(359, 352);
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
            Quitter.Location = new Point(229, 352);
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
            ClientSize = new Size(492, 391);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(Quitter);
            Controls.Add(Lancer);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "FichierTuilesKMZ";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Création des Fichiers KMZ";
            ((ISupportInitialize)CorLargeur).EndInit();
            ((ISupportInitialize)CorHauteur).EndInit();
            ((ISupportInitialize)CorPT0X).EndInit();
            ((ISupportInitialize)CorPT0Y).EndInit();
            Support.ResumeLayout();
            ResumeLayout(false);
            PerformLayout();
            #endregion
        }
        #region Déclaration
        private ToolTip ToolTip1;
        private Label L011;
        private Label L018;
        private Label L014;
        private NumericUpDown CorLargeur;
        private Label LargeurMaxi;
        private Label HauteurMaxi;
        private Label L015;
        private NumericUpDown CorHauteur;
        private Label L027;
        private Label NbTuilesKMZ;
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
        private NumericUpDown N512;
        private Label L026;
        private TextBox NomCarte;
        private Label Information;
        private Label NbRangs;
        private Label L029;
        private Label L028;
        private Label NbCols;
        private Label LLARGEUR;
        private Label LHAUTEUR;
        private Label LPT0X;
        private Label LPT0Y;
        private Label L030;
        private ComboBox ChoixTailleTampon;
        private Panel Support;
        #endregion
    }
}