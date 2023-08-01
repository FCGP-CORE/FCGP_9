using static FCGP.Commun;

namespace FCGP
{
    internal partial class Preferences : Form
    {
        internal Preferences()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        /// <summary> Initialise l'ensemble des évenements liés au formulaire car pas variable avec le modificateur WithEvents 
        /// rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
        private void InitialiserEvenements()
        {
            // controles simples
            RepertoireCartes.Click += Repertoires_Click;
            RepertoireTuiles.Click += Repertoires_Click;
            FactJNX.ValueChanged += FactJNX_ValueChanged;
            FactORUX.ValueChanged += FactORUX_ValueChanged;
            IsGrille.CheckedChanged += IsGrille_CheckedChanged;
            Tuiles_0.CheckedChanged += Tuiles_CheckedChanged;
            Tuiles_1.CheckedChanged += Tuiles_CheckedChanged;
            Tuiles_7.CheckedChanged += Tuiles_CheckedChanged;
            Tuiles_4.CheckedChanged += Tuiles_CheckedChanged;
            Tuiles_2.CheckedChanged += Tuiles_CheckedChanged;
            DimTuilesLarg.ValueChanged += DimTuilesLarg_ValueChanged;
            // FormatCartes
            LabelChoixFormatCartes.Click += Labels_Click;
            ListeChoixFormatCartes.DropDownClosed += Listes_DropDownClosed;
            ListeChoixFormatCartes.SelectedIndexChanged += Listes_SelectedIndexChanged;
            // GeoRefs
            LabelChoixGeoRefs.Click += Labels_Click;
            ListeChoixGeoRefs.DropDownClosed += Listes_DropDownClosed;
            ListeChoixGeoRefs.SelectedIndexChanged += Listes_SelectedIndexChanged;
            // formulaire
            Load += Preferences_Load;
            FormClosing += Preferences_FormClosing;
            FormClosed += Preferences_FormClosed;
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
            var resources = new ComponentResourceManager(typeof(Preferences));
            GroupeReferences = new Panel();
            LabelNomCarte = new Label();
            NomCarte = new TextBox();
            LabelChoixFormatCartes = new Label();
            ListeChoixFormatCartes = new ComboBox();
            LabelGeoRefs = new Label();
            LabelChoixGeoRefs = new Label();
            ListeChoixGeoRefs = new ComboBox();
            LabelFormatCartes = new Label();
            LabelRepertoireCartes = new Label();
            RepertoireCartes = new Label();
            LabelRepertoireTuiles = new Label();
            RepertoireTuiles = new Label();
            LabelGroupeFichiersTuiles = new Label();
            LabelIndices = new Label();
            LabelPlusMoins = new Label();
            FactJNX = new NumericUpDown();
            FactORUX = new NumericUpDown();
            Tuiles_0 = new RadioButton();
            Tuiles_1 = new RadioButton();
            Tuiles_2 = new RadioButton();
            Tuiles_4 = new RadioButton();
            Tuiles_7 = new RadioButton();
            LabelDimTuilesLarg = new Label();
            DimTuilesLarg = new NumericUpDown();
            GroupeFichiersTuiles = new Label();
            LabelIsGrille = new Label();
            IsGrille = new CheckBox();
            LabelIsReferences = new Label();
            IsReferences = new CheckBox();
            LabelIsTrace = new Label();
            IsTrace = new CheckBox();
            btnYES = new Button();
            btnNO = new Button();
            ToolTip1 = new ToolTip(components);
            ((ISupportInitialize)FactJNX).BeginInit();
            ((ISupportInitialize)FactORUX).BeginInit();
            ((ISupportInitialize)DimTuilesLarg).BeginInit();
            GroupeReferences.SuspendLayout();
            SuspendLayout();
            // 
            // LabelNomCarte
            // 
            LabelNomCarte.BackColor = Color.Transparent;
            LabelNomCarte.BorderStyle = BorderStyle.FixedSingle;
            LabelNomCarte.FlatStyle = FlatStyle.Flat;
            LabelNomCarte.Location = new Point(6, 6);
            LabelNomCarte.Name = "LabelNomCarte";
            LabelNomCarte.Size = new Size(380, 21);
            LabelNomCarte.TabIndex = 1000;
            LabelNomCarte.TextAlign = ContentAlignment.MiddleCenter;
            LabelNomCarte.Text = "Nom de la carte";
            // 
            // NomCarte
            // 
            NomCarte.BackColor = Color.White;
            NomCarte.BorderStyle = BorderStyle.FixedSingle;
            NomCarte.Location = new Point(6, 26);
            NomCarte.Margin = new Padding(0);
            NomCarte.Name = "NomCarte";
            NomCarte.Size = new Size(380, 26);
            NomCarte.TabIndex = 1;
            ToolTip1.SetToolTip(NomCarte, "Nom de la carte. Sert de base à l'ensemble des fichiers créés.");
            // 
            // LabelFormatCartes
            // 
            LabelFormatCartes.BackColor = Color.Transparent;
            LabelFormatCartes.BorderStyle = BorderStyle.FixedSingle;
            LabelFormatCartes.Location = new Point(6, 51);
            LabelFormatCartes.Name = "LabelFormatCartes";
            LabelFormatCartes.Size = new Size(191, 21);
            LabelFormatCartes.TabIndex = 4;
            LabelFormatCartes.Text = "Format enregistrement";
            LabelFormatCartes.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelChoixFormatCartes
            // 
            LabelChoixFormatCartes.BackColor = Color.White;
            LabelChoixFormatCartes.BorderStyle = BorderStyle.FixedSingle;
            LabelChoixFormatCartes.Location = new Point(6, 71);
            LabelChoixFormatCartes.Name = "LabelChoixFormatCartes";
            LabelChoixFormatCartes.Size = new Size(191, 21);
            LabelChoixFormatCartes.TabIndex = 1007;
            LabelChoixFormatCartes.TextAlign = ContentAlignment.MiddleCenter;
            LabelChoixFormatCartes.Tag = ListeChoixFormatCartes;
            ToolTip1.SetToolTip(LabelChoixFormatCartes, resources.GetString("LabelChoixFormatCartes.ToolTip"));
            // 
            // ListeChoixFormatCartes
            // 
            ListeChoixFormatCartes.BackColor = Color.White;
            ListeChoixFormatCartes.Location = new Point(6, 64); // 71-1
            ListeChoixFormatCartes.Margin = new Padding(0);
            ListeChoixFormatCartes.Name = "ListeChoixFormatCartes";
            ListeChoixFormatCartes.Size = new Size(191, 0);
            ListeChoixFormatCartes.Visible = false;
            ListeChoixFormatCartes.TabIndex = 1;
            ListeChoixFormatCartes.Tag = LabelChoixFormatCartes;
            // 
            // LabelGeoRefs
            // 
            LabelGeoRefs.BackColor = Color.Transparent;
            LabelGeoRefs.BorderStyle = BorderStyle.FixedSingle;
            LabelGeoRefs.Location = new Point(196, 51);
            LabelGeoRefs.Name = "LabelGeoRefs";
            LabelGeoRefs.Size = new Size(190, 21);
            LabelGeoRefs.TabIndex = 23;
            LabelGeoRefs.Text = "Géo-référencement";
            LabelGeoRefs.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelChoixGeoRefs
            // 
            LabelChoixGeoRefs.BackColor = Color.White;
            LabelChoixGeoRefs.BorderStyle = BorderStyle.FixedSingle;
            LabelChoixGeoRefs.Location = new Point(196, 71);
            LabelChoixGeoRefs.Name = "LabelChoixGeoRefs";
            LabelChoixGeoRefs.Size = new Size(190, 21);
            LabelChoixGeoRefs.TabIndex = 45;
            LabelChoixGeoRefs.TextAlign = ContentAlignment.MiddleCenter;
            LabelChoixGeoRefs.Tag = ListeChoixGeoRefs;
            ToolTip1.SetToolTip(LabelChoixGeoRefs, resources.GetString("LabelChoixGeoRefs.ToolTip"));
            // 
            // ListeChoixGeoRefs
            // 
            ListeChoixGeoRefs.BackColor = Color.White;
            ListeChoixGeoRefs.Location = new Point(196, 64);
            ListeChoixGeoRefs.Name = "ListeChoixGeoRefs";
            ListeChoixGeoRefs.Size = new Size(190, 0);
            ListeChoixGeoRefs.Visible = false;
            ListeChoixGeoRefs.TabIndex = 2;
            ListeChoixGeoRefs.Tag = LabelChoixGeoRefs;
            // 
            // LabelRepertoireCartes
            // 
            LabelRepertoireCartes.BackColor = Color.Transparent;
            LabelRepertoireCartes.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireCartes.Location = new Point(6, 91);
            LabelRepertoireCartes.Name = "LabelRepertoireCartes";
            LabelRepertoireCartes.Size = new Size(380, 21);
            LabelRepertoireCartes.TabIndex = 19;
            LabelRepertoireCartes.TextAlign = ContentAlignment.MiddleCenter;
            LabelRepertoireCartes.Text = "Répertoire des Cartes";
            // 
            // RepertoireCartes
            // 
            RepertoireCartes.BackColor = Color.White;
            RepertoireCartes.BorderStyle = BorderStyle.FixedSingle;
            RepertoireCartes.Location = new Point(6, 111);
            RepertoireCartes.Name = "RepertoireCartes";
            RepertoireCartes.Size = new Size(380, 21);
            RepertoireCartes.TabIndex = 7;
            RepertoireCartes.TextAlign = ContentAlignment.MiddleLeft;
            RepertoireCartes.Tag = "cartes";
            ToolTip1.SetToolTip(RepertoireCartes, "Indique le répertoire actuel des cartes.");
            // 
            // LabelRepertoireTuiles
            // 
            LabelRepertoireTuiles.BackColor = Color.Transparent;
            LabelRepertoireTuiles.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireTuiles.Location = new Point(6, 137);
            LabelRepertoireTuiles.Name = "LabelRepertoireTuiles";
            LabelRepertoireTuiles.Size = new Size(380, 21);
            LabelRepertoireTuiles.TabIndex = 16;
            LabelRepertoireTuiles.TextAlign = ContentAlignment.MiddleCenter;
            LabelRepertoireTuiles.Text = "Répertoire des fichiers Tuiles";
            // 
            // RepertoireTuiles
            // 
            RepertoireTuiles.BackColor = Color.White;
            RepertoireTuiles.BorderStyle = BorderStyle.FixedSingle;
            RepertoireTuiles.Location = new Point(6, 157);
            RepertoireTuiles.Name = "RepertoireTuiles";
            RepertoireTuiles.Size = new Size(380, 21);
            RepertoireTuiles.TabIndex = 17;
            RepertoireTuiles.TextAlign = ContentAlignment.MiddleLeft;
            RepertoireTuiles.Tag = "fichiers Tuiles";
            ToolTip1.SetToolTip(RepertoireTuiles, "Indique le répertoire actuel des fichiers tuiles.");
            // 
            // LabelGroupeFichiersTuiles
            // 
            LabelGroupeFichiersTuiles.BackColor = Color.Transparent;
            LabelGroupeFichiersTuiles.BorderStyle = BorderStyle.FixedSingle;
            LabelGroupeFichiersTuiles.Location = new Point(6, 183);
            LabelGroupeFichiersTuiles.Name = "LabelGroupeFichiersTuiles";
            LabelGroupeFichiersTuiles.Size = new Size(232, 21);
            LabelGroupeFichiersTuiles.TabIndex = 1009;
            LabelGroupeFichiersTuiles.Text = "Génération des Fichiers Tuiles";
            LabelGroupeFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Tuiles_0
            // 
            Tuiles_0.Font = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Tuiles_0.BackColor = Color.Transparent;
            Tuiles_0.CheckAlign = ContentAlignment.BottomCenter;
            Tuiles_0.Location = new Point(12, 203);
            Tuiles_0.Margin = new Padding(0);
            Tuiles_0.Name = "Tuiles_0";
            Tuiles_0.Size = new Size(20, 90);
            Tuiles_0.TabIndex = 5;
            Tuiles_0.TabStop = true;
            Tuiles_0.Text = "A" + CrLf + "c" + CrLf + "u" + CrLf + "n";
            Tuiles_0.TextAlign = ContentAlignment.TopCenter;
            Tuiles_0.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(Tuiles_0, "Aucun fichier tuiles ne sera créé.");
            // 
            // Tuiles_2
            // 
            Tuiles_2.BackColor = Color.Transparent;
            Tuiles_2.CheckAlign = ContentAlignment.BottomCenter;
            Tuiles_2.Location = new Point(41, 253);
            Tuiles_2.Name = "Tuiles_2";
            Tuiles_2.Size = new Size(50, 40);
            Tuiles_2.TabIndex = 6;
            Tuiles_2.TabStop = true;
            Tuiles_2.Text = "JNX";
            Tuiles_2.TextAlign = ContentAlignment.TopCenter;
            Tuiles_2.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(Tuiles_2, "Le fichier tuiles JNX (GPS Garmin) sera créé.");
            // 
            // Tuiles_4
            // 
            Tuiles_4.BackColor = Color.Transparent;
            Tuiles_4.CheckAlign = ContentAlignment.BottomCenter;
            Tuiles_4.Location = new Point(99, 253);
            Tuiles_4.Name = "Tuiles_4";
            Tuiles_4.Size = new Size(50, 40);
            Tuiles_4.TabIndex = 7;
            Tuiles_4.TabStop = true;
            Tuiles_4.Text = "KMZ";
            Tuiles_4.TextAlign = ContentAlignment.TopCenter;
            Tuiles_4.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(Tuiles_4, "Le fichier tuiles KMZ (GPS Garmin et autre SIGs) sera créé.");
            // 
            // Tuiles_1
            // 
            Tuiles_1.BackColor = Color.Transparent;
            Tuiles_1.CheckAlign = ContentAlignment.BottomCenter;
            Tuiles_1.Location = new Point(157, 253);
            Tuiles_1.Margin = new Padding(0);
            Tuiles_1.Name = "Tuiles_1";
            Tuiles_1.Size = new Size(50, 40);
            Tuiles_1.TabIndex = 8;
            Tuiles_1.TabStop = true;
            Tuiles_1.Text = "ORUX";
            Tuiles_1.TextAlign = ContentAlignment.TopCenter;
            Tuiles_1.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(Tuiles_1, "Le fichier tuiles ORUX (OruxMap et smartphone) sera créé.");
            // 
            // Tuiles_7
            // 
            Tuiles_7.Font = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Tuiles_7.BackColor = Color.Transparent;
            Tuiles_7.CheckAlign = ContentAlignment.BottomCenter;
            Tuiles_7.Location = new Point(214, 203);
            Tuiles_7.Margin = new Padding(0);
            Tuiles_7.Name = "Tuiles_7";
            Tuiles_7.Size = new Size(20, 90);
            Tuiles_7.TabIndex = 9;
            Tuiles_7.TabStop = true;
            Tuiles_7.Text = "T" + CrLf + "o" + CrLf + "u" + CrLf + "s";
            Tuiles_7.TextAlign = ContentAlignment.TopCenter;
            Tuiles_7.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(Tuiles_7, "Les fichiers tuiles JNX, KMZ et ORUX seront créés.");
            // 
            // LabelIndices
            // 
            LabelIndices.BackColor = Color.Transparent;
            LabelIndices.BorderStyle = BorderStyle.FixedSingle;
            LabelIndices.Location = new Point(38, 209);
            LabelIndices.Name = "LabelIndices";
            LabelIndices.Size = new Size(170, 21);
            LabelIndices.TabIndex = 43;
            LabelIndices.Text = "Indices d'affichage";
            LabelIndices.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(LabelIndices, "Permet de modifier l'indice d'affichage." + CrLf + 
                                              "Change l'échelle de transition entre 2 " + "de niveaux de détails" + CrLf + 
                                              "si plusieurs niveaux de détails existent pour la même zone." + CrLf + "La valeur par défaut est en vert.");
            // 
            // FactJNX
            // 
            FactJNX.BackColor = Color.White;
            FactJNX.DecimalPlaces = 2;
            FactJNX.Increment = 0.25m;
            FactJNX.Location = new Point(38, 229);
            FactJNX.Maximum = 0m;
            FactJNX.Name = "FactJNX";
            FactJNX.ReadOnly = true;
            FactJNX.Size = new Size(57, 26);
            FactJNX.TabIndex = 10;
            FactJNX.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(FactJNX, "Concerne les fichiers JNX.");
            // 
            // LabelPlusMoins
            // 
            LabelPlusMoins.BackColor = Color.Transparent;
            LabelPlusMoins.BorderStyle = BorderStyle.FixedSingle;
            LabelPlusMoins.Location = new Point(94, 229);
            LabelPlusMoins.Name = "LabelPlusMoins";
            LabelPlusMoins.Size = new Size(58, 26);
            LabelPlusMoins.TabIndex = 42;
            LabelPlusMoins.Text = "+ ou -";
            LabelPlusMoins.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FactORUX
            // 
            FactORUX.BackColor = Color.White;
            FactORUX.ForeColor = SystemColors.WindowText;
            FactORUX.Location = new Point(151, 229);
            FactORUX.Maximum = 0m;
            FactORUX.Name = "FactORUX";
            FactORUX.ReadOnly = true;
            FactORUX.Size = new Size(57, 26);
            FactORUX.TabIndex = 11;
            FactORUX.TextAlign = HorizontalAlignment.Center;
            ToolTip1.SetToolTip(FactORUX, "Concerne les fichiers ORUX.");
            // 
            // LabelDimTuilesLarg
            // 
            LabelDimTuilesLarg.BackColor = Color.Transparent;
            LabelDimTuilesLarg.BorderStyle = BorderStyle.FixedSingle;
            LabelDimTuilesLarg.Location = new Point(12, 299);
            LabelDimTuilesLarg.Name = "LabelDimTuilesLarg";
            LabelDimTuilesLarg.Size = new Size(220, 21);
            LabelDimTuilesLarg.TabIndex = 44;
            LabelDimTuilesLarg.Text = "Dimensions H*L des Tuiles";
            LabelDimTuilesLarg.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(LabelDimTuilesLarg, "Largeur et hauteur en pixel des tuiles" + CrLf + "incorporées dans les fichiers tuiles.");
            // 
            // DimTuilesLarg
            // 
            DimTuilesLarg.BackColor = Color.White;
            DimTuilesLarg.BorderStyle = BorderStyle.FixedSingle;
            DimTuilesLarg.Increment = 256m;
            DimTuilesLarg.Location = new Point(12, 319);
            DimTuilesLarg.Maximum = 1024m;
            DimTuilesLarg.Minimum = 256m;
            DimTuilesLarg.Name = "DimTuilesLarg";
            DimTuilesLarg.ReadOnly = true;
            DimTuilesLarg.Size = new Size(220, 26);
            DimTuilesLarg.TabIndex = 12;
            DimTuilesLarg.TextAlign = HorizontalAlignment.Center;
            DimTuilesLarg.Value = 256m;
            ToolTip1.SetToolTip(DimTuilesLarg, resources.GetString("DimTuilesLarg.ToolTip"));
            // 
            // GroupeFichiersTuiles
            // 
            GroupeFichiersTuiles.BackColor = Color.Transparent;
            GroupeFichiersTuiles.BorderStyle = BorderStyle.FixedSingle;
            GroupeFichiersTuiles.Location = new Point(6, 203);
            GroupeFichiersTuiles.Name = "GroupeFichiersTuiles";
            GroupeFichiersTuiles.Size = new Size(232, 149);
            GroupeFichiersTuiles.TabIndex = 45;
            GroupeFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelIsGrille
            // 
            LabelIsGrille.BackColor = Color.Transparent;
            LabelIsGrille.BorderStyle = BorderStyle.FixedSingle;
            LabelIsGrille.Location = new Point(243, 183);
            LabelIsGrille.Name = "LabelIsGrille";
            LabelIsGrille.Size = new Size(143, 57);
            LabelIsGrille.TabIndex = 39;
            LabelIsGrille.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // IsGrille
            // 
            IsGrille.BackColor = Color.Transparent;
            IsGrille.BackgroundImageLayout = ImageLayout.Center;
            IsGrille.CheckAlign = ContentAlignment.BottomCenter;
            IsGrille.FlatAppearance.BorderColor = SystemColors.ActiveCaptionText;
            IsGrille.ImageAlign = ContentAlignment.MiddleRight;
            IsGrille.Location = new Point(244, 193);
            IsGrille.Name = "IsGrille";
            IsGrille.Size = new Size(140, 36);
            IsGrille.TabIndex = 14;
            IsGrille.Text = "Afficher Grille";
            IsGrille.TextAlign = ContentAlignment.TopCenter;
            IsGrille.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(IsGrille, resources.GetString("IsGrille.ToolTip"));
            // 
            // LabelIsReferences
            // 
            LabelIsReferences.BackColor = Color.Transparent;
            LabelIsReferences.BorderStyle = BorderStyle.FixedSingle;
            LabelIsReferences.Location = new Point(243, 239);
            LabelIsReferences.Name = "LabelIsReferences";
            LabelIsReferences.Size = new Size(143, 57);
            LabelIsReferences.TabIndex = 1003;
            LabelIsReferences.TextAlign = ContentAlignment.TopCenter;
            // 
            // IsReferences
            // 
            IsReferences.BackColor = Color.Transparent;
            IsReferences.BackgroundImageLayout = ImageLayout.Center;
            IsReferences.CheckAlign = ContentAlignment.BottomCenter;
            IsReferences.FlatAppearance.BorderColor = SystemColors.ActiveCaptionText;
            IsReferences.Location = new Point(244, 248);
            IsReferences.Margin = new Padding(0);
            IsReferences.Name = "IsReferences";
            IsReferences.Size = new Size(140, 36);
            IsReferences.TabIndex = 15;
            IsReferences.Text = "Reférences";
            IsReferences.TextAlign = ContentAlignment.TopCenter;
            IsReferences.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(IsReferences, resources.GetString("IsReferences.ToolTip"));
            // 
            // LabelIsTrace
            // 
            LabelIsTrace.BackColor = Color.Transparent;
            LabelIsTrace.BorderStyle = BorderStyle.FixedSingle;
            LabelIsTrace.Location = new Point(243, 295);
            LabelIsTrace.Name = "LabelIsTrace";
            LabelIsTrace.Size = new Size(143, 57);
            LabelIsTrace.TabIndex = 1005;
            LabelIsTrace.TextAlign = ContentAlignment.TopCenter;
            // 
            // IsTrace
            // 
            IsTrace.BackColor = Color.Transparent;
            IsTrace.CheckAlign = ContentAlignment.BottomCenter;
            IsTrace.Location = new Point(244, 303);
            IsTrace.Name = "IsTrace";
            IsTrace.Size = new Size(140, 36);
            IsTrace.TabIndex = 1006;
            IsTrace.Text = "Inclure Trace";
            IsTrace.TextAlign = ContentAlignment.TopCenter;
            IsTrace.UseVisualStyleBackColor = false;
            ToolTip1.SetToolTip(IsTrace, "Permet de dessiner la trace sur la carte capturée." + CrLf + 
                                         "Cette option n'est pas disponible si il n'y a pas de trace ou" + CrLf + 
                                         "si la trace n'est pas sur l'emprise de la carte à capturer.");
            // 
            // GroupeReferences
            // 
            GroupeReferences.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            GroupeReferences.BackColor = Color.FromArgb(247, 247, 247);
            GroupeReferences.Location = new Point(0, 0);
            GroupeReferences.Name = "GroupeReferences";
            GroupeReferences.Size = new Size(392, 358);
            GroupeReferences.TabIndex = 1008;
            GroupeReferences.Controls.Add(LabelNomCarte);
            GroupeReferences.Controls.Add(NomCarte);
            GroupeReferences.Controls.Add(LabelFormatCartes);
            GroupeReferences.Controls.Add(LabelChoixFormatCartes);
            GroupeReferences.Controls.Add(ListeChoixFormatCartes);
            GroupeReferences.Controls.Add(LabelGeoRefs);
            GroupeReferences.Controls.Add(LabelChoixGeoRefs);
            GroupeReferences.Controls.Add(ListeChoixGeoRefs);
            GroupeReferences.Controls.Add(LabelRepertoireCartes);
            GroupeReferences.Controls.Add(RepertoireCartes);
            GroupeReferences.Controls.Add(LabelRepertoireTuiles);
            GroupeReferences.Controls.Add(RepertoireTuiles);
            GroupeReferences.Controls.Add(LabelGroupeFichiersTuiles);
            GroupeReferences.Controls.Add(LabelIndices);
            GroupeReferences.Controls.Add(LabelPlusMoins);
            GroupeReferences.Controls.Add(FactJNX);
            GroupeReferences.Controls.Add(FactORUX);
            GroupeReferences.Controls.Add(Tuiles_0);
            GroupeReferences.Controls.Add(Tuiles_2);
            GroupeReferences.Controls.Add(Tuiles_4);
            GroupeReferences.Controls.Add(Tuiles_1);
            GroupeReferences.Controls.Add(Tuiles_7);
            GroupeReferences.Controls.Add(LabelDimTuilesLarg);
            GroupeReferences.Controls.Add(DimTuilesLarg);
            GroupeReferences.Controls.Add(GroupeFichiersTuiles);
            GroupeReferences.Controls.Add(IsGrille);
            GroupeReferences.Controls.Add(LabelIsGrille);
            GroupeReferences.Controls.Add(IsReferences);
            GroupeReferences.Controls.Add(LabelIsReferences);
            GroupeReferences.Controls.Add(IsTrace);
            GroupeReferences.Controls.Add(LabelIsTrace);
            // 
            // btnYES
            // 
            btnYES.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnYES.DialogResult = DialogResult.OK;
            btnYES.Location = new Point(311, 364);
            btnYES.Name = "btnYES";
            btnYES.Size = new Size(75, 30);
            btnYES.TabIndex = 16;
            btnYES.Text = "Lancer";
            btnYES.UseVisualStyleBackColor = true;
            ToolTip1.SetToolTip(btnYES, "Lance la création de la carte.");
            // 
            // btnNO
            // 
            btnNO.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNO.BackColor = SystemColors.Control;
            btnNO.DialogResult = DialogResult.Cancel;
            btnNO.Location = new Point(230, 364);
            btnNO.Margin = new Padding(3, 3, 10, 10);
            btnNO.Name = "btnNO";
            btnNO.Size = new Size(75, 30);
            btnNO.TabIndex = 17;
            btnNO.Text = "Annuler";
            btnNO.UseVisualStyleBackColor = true;
            ToolTip1.SetToolTip(btnNO, "Annule la création de la carte.");
            // 
            // ToolTip1
            // 
            ToolTip1.AutoPopDelay = 5000;
            ToolTip1.InitialDelay = 100;
            ToolTip1.ReshowDelay = 100;
            // 
            // Preferences
            // 
            AcceptButton = btnYES;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = btnNO;
            ClientSize = new Size(392, 400);
            ControlBox = false;
            Controls.Add(GroupeReferences);
            Controls.Add(btnNO);
            Controls.Add(btnYES);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Preferences";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            TransparencyKey = Color.FromArgb(255, 192, 192);
            ((ISupportInitialize)DimTuilesLarg).EndInit();
            ((ISupportInitialize)FactJNX).EndInit();
            ((ISupportInitialize)FactORUX).EndInit();
            GroupeReferences.ResumeLayout(false);
            GroupeReferences.PerformLayout();
            ResumeLayout(false);
        }

        private Panel GroupeReferences;
        private Label LabelNomCarte;
        private TextBox NomCarte;
        private Label LabelFormatCartes;
        private Label LabelChoixFormatCartes;
        private ComboBox ListeChoixFormatCartes;
        private Label LabelGeoRefs;
        private Label LabelChoixGeoRefs;
        private ComboBox ListeChoixGeoRefs;
        private Label LabelRepertoireCartes;
        private Label RepertoireCartes;
        private Label LabelRepertoireTuiles;
        private Label RepertoireTuiles;
        private Label LabelGroupeFichiersTuiles;
        private Label LabelIndices;
        private Label LabelPlusMoins;
        private NumericUpDown FactJNX;
        private NumericUpDown FactORUX;
        private RadioButton Tuiles_0;
        private RadioButton Tuiles_1;
        private RadioButton Tuiles_2;
        private RadioButton Tuiles_4;
        private RadioButton Tuiles_7;
        private Label LabelDimTuilesLarg;
        private NumericUpDown DimTuilesLarg;
        private Label GroupeFichiersTuiles;
        private Label LabelIsGrille;
        private CheckBox IsGrille;
        private Label LabelIsReferences;
        private CheckBox IsReferences;
        private Label LabelIsTrace;
        private CheckBox IsTrace;
        private Button btnYES;
        private Button btnNO;
        private ToolTip ToolTip1;
    }
}