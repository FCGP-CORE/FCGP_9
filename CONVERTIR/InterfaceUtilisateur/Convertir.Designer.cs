using static FCGP.Commun;

namespace FCGP
{
    internal partial class Convertir : Form
    {
        internal Convertir()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            Couleur.Click += Couleur_Click;
            Transparence.ValueChanged += Transparence_ValueChanged;
            IsTrace.CheckedChanged += IsTrace_CheckedChanged;
            NbPaths.ValueChanged += NbPaths_ValueChanged;
            DimTuilesLarg.ValueChanged += DimTuilesLarg_ValueChanged;
            Traitement.Click += Traitement_Click;
            LabelRepertoireCartes.Click += LabelRepertoireCartes_Click;
            LabelRepertoireTuiles.Click += LabelRepertoireFichiersTuiles_Click;
            Quitter.Click += Quitter_Click;
            // GeoRefs
            LabelGeoRefs.Click += LabelListesChoix_Click;
            GeoRefs.DropDownClosed += ListesChoix_DropDownClosed;
            GeoRefs.SelectedIndexChanged += GeoRefs_SelectedIndexChanged;
            // FormatCartes
            LabelFormatCartes.Click += LabelListesChoix_Click;
            FormatCartes.DropDownClosed += ListesChoix_DropDownClosed;
            FormatCartes.SelectedIndexChanged += FormatCartes_SelectedIndexChanged;
            // References
            LabelReferences.Click += LabelListesChoix_Click;
            References.DropDownClosed += ListesChoix_DropDownClosed;
            References.SelectedIndexChanged += References_SelectedIndexChanged;
            // Traces
            LabelTraces.Click += LabelTraces_Click;
            Traces.DropDownClosed += ListesChoix_DropDownClosed;
            Traces.SelectedIndexChanged += Traces_SelectedIndexChanged;
            // FichiersTuiles
            LabelFichiersTuiles.Click += LabelListesChoix_Click;
            FichiersTuiles.SelectedIndexChanged += FichiersTuiles_SelectedIndexChanged;
            // formulaire
            Load += Convertir_Load;
            FormClosing += Convertir_FormClosing;
            FormClosed += Convertir_FormClosed;
            LocationChanged += Convertir_LocationChanged;
            KeyDown += Convertir_KeyDown;
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
            var resources = new ComponentResourceManager(typeof(Convertir));
            GroupeConfigurations = new Panel();
            EtiquetteGroupeConfigurationCarte = new Label();
            LabelGeoRefs = new Label();
            EtiquetteGeoRefs = new Label();
            GeoRefs = new ComboBox();
            IsGrille = new CheckBox();
            EtiquetteIsGrille = new Label();
            GroupeCartes = new Panel();
            LabelFormatCartes = new Label();
            EtiquetteFormatCarte = new Label();
            FormatCartes = new ComboBox();
            LabelReferences = new Label();
            EtiquetteReferences = new Label();
            References = new ComboBox();
            EtiquetteRepertoireCartes = new Label();
            LabelRepertoireCartes = new Label();
            EtiquetteGroupeConfigurationTraces = new Label();
            LabelTraces = new Label();
            EtiquetteTraces = new Label();
            Traces = new ComboBox();
            EtiquetteBoutonTrace = new Label();
            GroupeConfigurationTraces = new Panel();
            EtiquetteEpaisseur = new Label();
            Epaisseur = new NumericUpDown();
            EtiquetteCouleur = new Label();
            Couleur = new Label();
            EtiquetteTransparence = new Label();
            Transparence = new NumericUpDown();
            IsTrace = new CheckBox();
            EtiquetteIsTrace = new Label();
            EtiquetteNbPaths = new Label();
            NbPaths = new NumericUpDown();
            LabelFichiersTuiles = new Label();
            EtiquetteFichiersTuiles = new Label();
            FichiersTuiles = new ComboBox();
            EtiquetteGroupeConfigurationFichiersTuiles = new Label();
            GroupeFichiersTuiles = new Panel();
            EtiquetteRepertoireTuiles = new Label();
            LabelRepertoireTuiles = new Label();
            EtiquetteDimTuilesLarg = new Label();
            DimTuilesLarg = new NumericUpDown();
            Information = new Label();
            Quitter = new Button();
            Traitement = new Button();
            OuvrirFichiersCarte = new OpenFileDialog();
            Informations = new ToolTip(components);
            ChoixCouleurTrace = new ColorDialog();
            OuvrirFichiersTrace = new OpenFileDialog();
            GroupeConfigurations.SuspendLayout();
            GroupeCartes.SuspendLayout();
            GroupeConfigurationTraces.SuspendLayout();
            ((ISupportInitialize)Epaisseur).BeginInit();
            ((ISupportInitialize)Transparence).BeginInit();
            ((ISupportInitialize)NbPaths).BeginInit();
            GroupeFichiersTuiles.SuspendLayout();
            ((ISupportInitialize)DimTuilesLarg).BeginInit();
            SuspendLayout();
            #region groupe configuration des cartes
            // 
            // EtiquetteGroupeConfigurationCarte
            // 
            EtiquetteGroupeConfigurationCarte.BackColor = Color.Transparent;
            EtiquetteGroupeConfigurationCarte.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteGroupeConfigurationCarte.Cursor = Cursors.Default;
            EtiquetteGroupeConfigurationCarte.FlatStyle = FlatStyle.Flat;
            EtiquetteGroupeConfigurationCarte.Location = new Point(6, 6);
            EtiquetteGroupeConfigurationCarte.Margin = new Padding(0);
            EtiquetteGroupeConfigurationCarte.Name = "EtiquetteGroupeConfigurationCarte";
            EtiquetteGroupeConfigurationCarte.Size = new Size(424, 27);
            EtiquetteGroupeConfigurationCarte.TabIndex = 66;
            EtiquetteGroupeConfigurationCarte.Text = "Configuration des Cartes";
            EtiquetteGroupeConfigurationCarte.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelGeoRefs
            // 
            LabelGeoRefs.BackColor = Color.White;
            LabelGeoRefs.BorderStyle = BorderStyle.FixedSingle;
            LabelGeoRefs.Cursor = Cursors.Default;
            LabelGeoRefs.FlatStyle = FlatStyle.Flat;
            LabelGeoRefs.Location = new Point(6, 71);
            LabelGeoRefs.Margin = new Padding(0);
            LabelGeoRefs.Name = "LabelGeoRefs";
            LabelGeoRefs.Size = new Size(145, 23);
            LabelGeoRefs.TabIndex = 51;
            LabelGeoRefs.Tag = GeoRefs;
            LabelGeoRefs.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(LabelGeoRefs, "Choix des géoréférencement des cartes. Voir l'aide");
            // 
            // EtiquetteGeoRefs
            // 
            EtiquetteGeoRefs.BackColor = Color.Transparent;
            EtiquetteGeoRefs.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteGeoRefs.Cursor = Cursors.Default;
            EtiquetteGeoRefs.FlatStyle = FlatStyle.Flat;
            EtiquetteGeoRefs.Location = new Point(6, 32);
            EtiquetteGeoRefs.Margin = new Padding(0);
            EtiquetteGeoRefs.Name = "EtiquetteGeoRefs";
            EtiquetteGeoRefs.Size = new Size(145, 40);
            EtiquetteGeoRefs.TabIndex = 0;
            EtiquetteGeoRefs.Text = "Géo-référencement";
            EtiquetteGeoRefs.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GeoRefs
            // 
            GeoRefs.BackColor = Color.White;
            GeoRefs.Cursor = Cursors.Default;
            GeoRefs.DropDownStyle = ComboBoxStyle.DropDownList;
            GeoRefs.FlatStyle = FlatStyle.System;
            GeoRefs.FormattingEnabled = true;
            GeoRefs.Location = new Point(6, 66);
            GeoRefs.Name = "GeoRefs";
            GeoRefs.Size = new Size(145, 27);
            GeoRefs.TabIndex = 2;
            GeoRefs.Visible = false;
            // 
            // GroupeCartes
            // 
            GroupeCartes.BackColor = Color.Transparent;
            GroupeCartes.Controls.Add(LabelFormatCartes);
            GroupeCartes.Controls.Add(EtiquetteFormatCarte);
            GroupeCartes.Controls.Add(FormatCartes);
            GroupeCartes.Controls.Add(LabelReferences);
            GroupeCartes.Controls.Add(EtiquetteReferences);
            GroupeCartes.Controls.Add(References);
            GroupeCartes.Controls.Add(IsGrille);
            GroupeCartes.Controls.Add(EtiquetteIsGrille);
            GroupeCartes.Controls.Add(EtiquetteRepertoireCartes);
            GroupeCartes.Controls.Add(LabelRepertoireCartes);
            GroupeCartes.Location = new Point(6, 32);
            GroupeCartes.Margin = new Padding(0);
            GroupeCartes.Name = "GroupeCartes";
            GroupeCartes.Size = new Size(424, 104);
            GroupeCartes.TabIndex = 65;
            // 
            // LabelFormatCartes
            // 
            LabelFormatCartes.BackColor = Color.White;
            LabelFormatCartes.BorderStyle = BorderStyle.FixedSingle;
            LabelFormatCartes.Cursor = Cursors.Default;
            LabelFormatCartes.FlatStyle = FlatStyle.Flat;
            LabelFormatCartes.Location = new Point(144, 39);
            LabelFormatCartes.Margin = new Padding(0);
            LabelFormatCartes.Name = "LabelFormatCartes";
            LabelFormatCartes.Size = new Size(107, 23);
            LabelFormatCartes.TabIndex = 53;
            LabelFormatCartes.Tag = FormatCartes;
            LabelFormatCartes.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(LabelFormatCartes, "Choix du format d'enregistrement des cartes.");
            // 
            // EtiquetteFormatCarte
            // 
            EtiquetteFormatCarte.BackColor = Color.Transparent;
            EtiquetteFormatCarte.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteFormatCarte.Cursor = Cursors.Default;
            EtiquetteFormatCarte.FlatStyle = FlatStyle.Flat;
            EtiquetteFormatCarte.Location = new Point(144, 0);
            EtiquetteFormatCarte.Margin = new Padding(0);
            EtiquetteFormatCarte.Name = "EtiquetteFormatCarte";
            EtiquetteFormatCarte.Size = new Size(107, 40);
            EtiquetteFormatCarte.TabIndex = 0;
            EtiquetteFormatCarte.Text = "Format" + CrLf + "Enregistrement";
            EtiquetteFormatCarte.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormatCartes
            // 
            FormatCartes.BackColor = Color.White;
            FormatCartes.Cursor = Cursors.Default;
            FormatCartes.DropDownStyle = ComboBoxStyle.DropDownList;
            FormatCartes.FlatStyle = FlatStyle.System;
            FormatCartes.FormattingEnabled = true;
            FormatCartes.Items.AddRange(new object[] { "Bmp", "Png", "Jpeg" });
            FormatCartes.Location = new Point(144, 34);
            FormatCartes.Name = "FormatCartes";
            FormatCartes.Size = new Size(107, 24);
            FormatCartes.TabIndex = 3;
            FormatCartes.Visible = false;
            // 
            // LabelReferences
            // 
            LabelReferences.BackColor = Color.White;
            LabelReferences.BorderStyle = BorderStyle.FixedSingle;
            LabelReferences.Cursor = Cursors.Default;
            LabelReferences.FlatStyle = FlatStyle.Flat;
            LabelReferences.Location = new Point(250, 39);
            LabelReferences.Margin = new Padding(0);
            LabelReferences.Name = "LabelReferences";
            LabelReferences.Size = new Size(95, 23);
            LabelReferences.TabIndex = 52;
            LabelReferences.Tag = References;
            LabelReferences.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(LabelReferences, "Choix des références des grilles issues des cartes.");
            // 
            // EtiquetteReferences
            // 
            EtiquetteReferences.BackColor = Color.Transparent;
            EtiquetteReferences.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteReferences.Cursor = Cursors.Default;
            EtiquetteReferences.FlatStyle = FlatStyle.Flat;
            EtiquetteReferences.Location = new Point(250, 0);
            EtiquetteReferences.Margin = new Padding(0);
            EtiquetteReferences.Name = "EtiquetteReferences";
            EtiquetteReferences.Size = new Size(95, 40);
            EtiquetteReferences.TabIndex = 0;
            EtiquetteReferences.Text = "Coordonnées" + CrLf + "Grille";
            EtiquetteReferences.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // References
            // 
            References.BackColor = Color.White;
            References.Cursor = Cursors.Default;
            References.DropDownStyle = ComboBoxStyle.DropDownList;
            References.FlatStyle = FlatStyle.System;
            References.FormattingEnabled = true;
            References.Location = new Point(250, 34);
            References.Name = "References";
            References.Size = new Size(95, 24);
            References.TabIndex = 5;
            References.Visible = false;
            // 
            // IsGrille
            // 
            IsGrille.BackColor = Color.Transparent;
            IsGrille.CheckAlign = ContentAlignment.BottomCenter;
            IsGrille.Location = new Point(360, 4);
            IsGrille.Margin = new Padding(0);
            IsGrille.Name = "IsGrille";
            IsGrille.Size = new Size(51, 47);
            IsGrille.TabIndex = 51;
            IsGrille.TabStop = false;
            IsGrille.Text = "Grille";
            IsGrille.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(IsGrille, resources.GetString("IsGrille.ToolTip"));
            IsGrille.UseVisualStyleBackColor = false;
            // 
            // EtiquetteIsGrille
            // 
            EtiquetteIsGrille.BackColor = Color.Transparent;
            EtiquetteIsGrille.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteIsGrille.Cursor = Cursors.Default;
            EtiquetteIsGrille.FlatStyle = FlatStyle.Flat;
            EtiquetteIsGrille.Location = new Point(344, 0);
            EtiquetteIsGrille.Margin = new Padding(0);
            EtiquetteIsGrille.Name = "EtiquetteIsGrille";
            EtiquetteIsGrille.Size = new Size(80, 62);
            EtiquetteIsGrille.TabIndex = 54;
            EtiquetteIsGrille.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EtiquetteRepertoireCartes
            // 
            EtiquetteRepertoireCartes.BackColor = Color.Transparent;
            EtiquetteRepertoireCartes.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteRepertoireCartes.Location = new Point(0, 61);
            EtiquetteRepertoireCartes.Margin = new Padding(0);
            EtiquetteRepertoireCartes.Name = "EtiquetteCheminCartes";
            EtiquetteRepertoireCartes.Size = new Size(424, 21);
            EtiquetteRepertoireCartes.TabIndex = 39;
            EtiquetteRepertoireCartes.Text = "Répertoire d'enregistrement des Cartes";
            EtiquetteRepertoireCartes.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelRepertoireCartes
            // 
            LabelRepertoireCartes.BackColor = Color.White;
            LabelRepertoireCartes.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireCartes.Location = new Point(0, 81);
            LabelRepertoireCartes.Margin = new Padding(0);
            LabelRepertoireCartes.Name = "LabelCheminCartes";
            LabelRepertoireCartes.Size = new Size(424, 23);
            LabelRepertoireCartes.TabIndex = 0;
            LabelRepertoireCartes.TextAlign = ContentAlignment.MiddleLeft;
            Informations.SetToolTip(LabelRepertoireCartes, "Indique le répertoire où seront enregistrés la carte" + CrLf + 
                                                           " et le fichier de géo-référencement associé.");
            #endregion
            #region groupe configuration des traces
            // 
            // EtiquetteGroupeConfigurationTraces
            // 
            EtiquetteGroupeConfigurationTraces.BackColor = Color.Transparent;
            EtiquetteGroupeConfigurationTraces.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteGroupeConfigurationTraces.Cursor = Cursors.Default;
            EtiquetteGroupeConfigurationTraces.FlatStyle = FlatStyle.Flat;
            EtiquetteGroupeConfigurationTraces.Location = new Point(6, 142);
            EtiquetteGroupeConfigurationTraces.Margin = new Padding(0);
            EtiquetteGroupeConfigurationTraces.Name = "EtiquetteGroupeConfigurationTraces";
            EtiquetteGroupeConfigurationTraces.Size = new Size(424, 27);
            EtiquetteGroupeConfigurationTraces.TabIndex = 1;
            EtiquetteGroupeConfigurationTraces.Text = "Configuration des Traces";
            EtiquetteGroupeConfigurationTraces.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EtiquetteCouleur
            // 
            EtiquetteCouleur.BackColor = Color.Transparent;
            EtiquetteCouleur.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteCouleur.Cursor = Cursors.Default;
            EtiquetteCouleur.FlatStyle = FlatStyle.Flat;
            EtiquetteCouleur.Location = new Point(0, 0);
            EtiquetteCouleur.Margin = new Padding(0);
            EtiquetteCouleur.Name = "EtiquetteCouleur";
            EtiquetteCouleur.Size = new Size(80, 40);
            EtiquetteCouleur.TabIndex = 0;
            EtiquetteCouleur.Text = "Couleur";
            EtiquetteCouleur.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Couleur
            // 
            Couleur.BackColor = Color.White;
            Couleur.BorderStyle = BorderStyle.FixedSingle;
            Couleur.Location = new Point(0, 39);
            Couleur.Margin = new Padding(0);
            Couleur.Name = "Couleur";
            Couleur.Size = new Size(80, 26);
            Couleur.TabIndex = 1;
            Informations.SetToolTip(Couleur, "Cliquez pour choisir " + CrLf + "une autre couleur.");
            // 
            // EtiquetteTransparence
            // 
            EtiquetteTransparence.BackColor = Color.Transparent;
            EtiquetteTransparence.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteTransparence.Cursor = Cursors.Default;
            EtiquetteTransparence.FlatStyle = FlatStyle.Flat;
            EtiquetteTransparence.Location = new Point(79, 0);
            EtiquetteTransparence.Margin = new Padding(0);
            EtiquetteTransparence.Name = "EtiquetteTransparence";
            EtiquetteTransparence.Size = new Size(108, 40);
            EtiquetteTransparence.TabIndex = 0;
            EtiquetteTransparence.Text = "Transparence" + CrLf + "%";
            EtiquetteTransparence.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Transparence
            // 
            Transparence.AutoSize = true;
            Transparence.BackColor = Color.White;
            Transparence.BorderStyle = BorderStyle.FixedSingle;
            Transparence.Location = new Point(79, 39);
            Transparence.Margin = new Padding(0);
            Transparence.Name = "Transparence";
            Transparence.Size = new Size(108, 26);
            Transparence.TabIndex = 2;
            Transparence.TabStop = false;
            Transparence.TextAlign = HorizontalAlignment.Center;
            Informations.SetToolTip(Transparence, "Transparence du dessin de la trace." + CrLf + 
                                                  "0% --> Opaque" + CrLf + "100% --> Invisible");
            // 
            // EtiquetteEpaisseur
            // 
            EtiquetteEpaisseur.BackColor = Color.Transparent;
            EtiquetteEpaisseur.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteEpaisseur.Cursor = Cursors.Default;
            EtiquetteEpaisseur.FlatStyle = FlatStyle.Flat;
            EtiquetteEpaisseur.Location = new Point(186, 0);
            EtiquetteEpaisseur.Margin = new Padding(0);
            EtiquetteEpaisseur.Name = "EtiquetteEpaisseur";
            EtiquetteEpaisseur.Size = new Size(77, 40);
            EtiquetteEpaisseur.TabIndex = 0;
            EtiquetteEpaisseur.Text = "Epaisseur" + CrLf + "en pixels";
            EtiquetteEpaisseur.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Epaisseur
            // 
            Epaisseur.AutoSize = true;
            Epaisseur.BackColor = Color.White;
            Epaisseur.BorderStyle = BorderStyle.FixedSingle;
            Epaisseur.Location = new Point(186, 39);
            Epaisseur.Margin = new Padding(0);
            Epaisseur.Maximum = 20m;
            Epaisseur.Minimum = 2m;
            Epaisseur.Name = "Epaisseur";
            Epaisseur.Size = new Size(77, 26);
            Epaisseur.TabIndex = 43;
            Epaisseur.TabStop = false;
            Epaisseur.TextAlign = HorizontalAlignment.Center;
            Informations.SetToolTip(Epaisseur, "Epaisseur de la trace en pixels." + CrLf + 
                                               "Le nb de pixels doit dépendre de l'échelle de la carte" + CrLf + 
                                               "afin que la trace soit visible correctement.");
            Epaisseur.Value = 2m;
            // 
            // GroupeConfigurationTraces
            // 
            GroupeConfigurationTraces.BackColor = Color.Transparent;
            GroupeConfigurationTraces.Controls.Add(EtiquetteCouleur);
            GroupeConfigurationTraces.Controls.Add(Couleur);
            GroupeConfigurationTraces.Controls.Add(EtiquetteTransparence);
            GroupeConfigurationTraces.Controls.Add(Transparence);
            GroupeConfigurationTraces.Controls.Add(EtiquetteEpaisseur);
            GroupeConfigurationTraces.Controls.Add(Epaisseur);
            GroupeConfigurationTraces.Enabled = false;
            GroupeConfigurationTraces.Location = new Point(6, 168);
            GroupeConfigurationTraces.Margin = new Padding(0);
            GroupeConfigurationTraces.Name = "GroupeConfigurationTraces";
            GroupeConfigurationTraces.Size = new Size(263, 65);
            GroupeConfigurationTraces.TabIndex = 63;
            // 
            // EtiquetteNbPaths
            // 
            EtiquetteNbPaths.BackColor = Color.Transparent;
            EtiquetteNbPaths.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteNbPaths.Cursor = Cursors.Default;
            EtiquetteNbPaths.FlatStyle = FlatStyle.Flat;
            EtiquetteNbPaths.Location = new Point(268, 168);
            EtiquetteNbPaths.Margin = new Padding(0);
            EtiquetteNbPaths.Name = "EtiquetteNbPaths";
            EtiquetteNbPaths.Size = new Size(83, 40);
            EtiquetteNbPaths.TabIndex = 62;
            EtiquetteNbPaths.Text = "Nb de Paths" + CrLf + "dans KMZ";
            EtiquetteNbPaths.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NbPaths
            // 
            NbPaths.BackColor = Color.White;
            NbPaths.BorderStyle = BorderStyle.FixedSingle;
            NbPaths.Location = new Point(268, 207);
            NbPaths.Margin = new Padding(0);
            NbPaths.Maximum = 2m;
            NbPaths.Name = "NbPaths";
            NbPaths.Size = new Size(83, 26);
            NbPaths.TabIndex = 64;
            NbPaths.TabStop = false;
            NbPaths.TextAlign = HorizontalAlignment.Center;
            Informations.SetToolTip(NbPaths, resources.GetString("NbPaths.ToolTip"));
            // 
            // IsTrace
            // 
            IsTrace.BackColor = Color.Transparent;
            IsTrace.CheckAlign = ContentAlignment.BottomCenter;
            IsTrace.Location = new Point(364, 175);
            IsTrace.Margin = new Padding(0);
            IsTrace.Name = "IsTrace";
            IsTrace.Size = new Size(55, 47);
            IsTrace.TabIndex = 59;
            IsTrace.TabStop = false;
            IsTrace.Text = "Inclure";
            IsTrace.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(IsTrace, "Prise en compte d'une ou plusieurs traces" + CrLf + 
                                             "dans les fichiers en sortie de traitement.");
            IsTrace.UseVisualStyleBackColor = false;
            // 
            // EtiquetteIsTrace
            // 
            EtiquetteIsTrace.BackColor = Color.Transparent;
            EtiquetteIsTrace.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteIsTrace.Location = new Point(350, 168);
            EtiquetteIsTrace.Margin = new Padding(0);
            EtiquetteIsTrace.Name = "EtiquetteIsTrace";
            EtiquetteIsTrace.Size = new Size(80, 65);
            EtiquetteIsTrace.TabIndex = 60;

            // 
            // LabelTraces
            // 
            LabelTraces.BackColor = Color.White;
            LabelTraces.BorderStyle = BorderStyle.FixedSingle;
            LabelTraces.Location = new Point(6, 252);
            LabelTraces.Margin = new Padding(0);
            LabelTraces.Name = "LabelTraces";
            LabelTraces.Size = new Size(406, 24);
            LabelTraces.TabIndex = 15;
            LabelTraces.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(LabelTraces, "Première des traces sélectionnées." + CrLf + 
                                                 "Cliquez dessus pour changer la sélection.");
            // 
            // EtiquetteTraces
            // 
            EtiquetteTraces.BackColor = Color.Transparent;
            EtiquetteTraces.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteTraces.Location = new Point(6, 232);
            EtiquetteTraces.Margin = new Padding(0);
            EtiquetteTraces.Name = "EtiquetteTraces";
            EtiquetteTraces.Size = new Size(424, 21);
            EtiquetteTraces.TabIndex = 61;
            EtiquetteTraces.Text = "Traces sélectionnées pour être dessinées sur la carte";
            EtiquetteTraces.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Traces
            // 
            Traces.BackColor = Color.White;
            Traces.DropDownStyle = ComboBoxStyle.DropDownList;
            Traces.Enabled = false;
            Traces.FlatStyle = FlatStyle.System;
            Traces.FormattingEnabled = true;
            Traces.Location = new Point(6, 248);
            Traces.Margin = new Padding(0);
            Traces.Name = "Traces";
            Traces.Size = new Size(423, 24);
            Traces.TabIndex = 58;
            Traces.TabStop = false;
            Informations.SetToolTip(Traces, "Liste des traces sélectionnées.");
            // 
            // EtiquetteBoutonTrace
            // 
            EtiquetteBoutonTrace.BackColor = Color.Transparent;
            EtiquetteBoutonTrace.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteBoutonTrace.Location = new Point(411, 252);
            EtiquetteBoutonTrace.Name = "EtiquetteBoutonTrace";
            EtiquetteBoutonTrace.Size = new Size(19, 24);
            EtiquetteBoutonTrace.TabIndex = 0;
            EtiquetteBoutonTrace.Text = "Label16";
            #endregion
            #region groupe configuration des fichiers tuiles
            // 
            // EtiquetteGroupeConfigurationFichiersTuiles
            // 
            EtiquetteGroupeConfigurationFichiersTuiles.BackColor = Color.Transparent;
            EtiquetteGroupeConfigurationFichiersTuiles.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteGroupeConfigurationFichiersTuiles.Cursor = Cursors.Default;
            EtiquetteGroupeConfigurationFichiersTuiles.FlatStyle = FlatStyle.Flat;
            EtiquetteGroupeConfigurationFichiersTuiles.Location = new Point(6, 282);
            EtiquetteGroupeConfigurationFichiersTuiles.Margin = new Padding(0);
            EtiquetteGroupeConfigurationFichiersTuiles.Name = "EtiquetteGroupeConfigurationFichiersTuiles";
            EtiquetteGroupeConfigurationFichiersTuiles.Size = new Size(424, 27);
            EtiquetteGroupeConfigurationFichiersTuiles.TabIndex = 67;
            EtiquetteGroupeConfigurationFichiersTuiles.Text = "Configuration des Fichiers Tuiles";
            EtiquetteGroupeConfigurationFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelFichiersTuiles
            // 
            LabelFichiersTuiles.BackColor = Color.White;
            LabelFichiersTuiles.BorderStyle = BorderStyle.FixedSingle;
            LabelFichiersTuiles.Cursor = Cursors.Default;
            LabelFichiersTuiles.FlatStyle = FlatStyle.Flat;
            LabelFichiersTuiles.Location = new Point(6, 328);
            LabelFichiersTuiles.Margin = new Padding(0);
            LabelFichiersTuiles.Name = "LabelFichiersTuiles";
            LabelFichiersTuiles.Size = new Size(212, 26);
            LabelFichiersTuiles.TabIndex = 55;
            LabelFichiersTuiles.Tag = FichiersTuiles;
            LabelFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter;
            Informations.SetToolTip(LabelFichiersTuiles, "Choix des fichiers tuiles issus des cartes.");
            // 
            // EtiquetteFichiersTuiles
            // 
            EtiquetteFichiersTuiles.BackColor = Color.Transparent;
            EtiquetteFichiersTuiles.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteFichiersTuiles.Cursor = Cursors.Default;
            EtiquetteFichiersTuiles.FlatStyle = FlatStyle.Flat;
            EtiquetteFichiersTuiles.Location = new Point(6, 308);
            EtiquetteFichiersTuiles.Margin = new Padding(0);
            EtiquetteFichiersTuiles.Name = "EtiquetteFichiersTuiles";
            EtiquetteFichiersTuiles.Size = new Size(212, 21);
            EtiquetteFichiersTuiles.TabIndex = 0;
            EtiquetteFichiersTuiles.Text = "Fichiers Tuiles";
            EtiquetteFichiersTuiles.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FichiersTuiles
            // 
            FichiersTuiles.BackColor = Color.White;
            FichiersTuiles.Cursor = Cursors.Default;
            FichiersTuiles.DropDownStyle = ComboBoxStyle.DropDownList;
            FichiersTuiles.FlatStyle = FlatStyle.System;
            FichiersTuiles.FormattingEnabled = true;
            FichiersTuiles.Location = new Point(6, 326);
            FichiersTuiles.Margin = new Padding(0);
            FichiersTuiles.Name = "FichiersTuiles";
            FichiersTuiles.Size = new Size(212, 24);
            FichiersTuiles.TabIndex = 8;
            FichiersTuiles.Visible = false;
            // 
            // GroupeFichiersTuiles
            // 
            GroupeFichiersTuiles.BackColor = Color.Transparent;
            GroupeFichiersTuiles.BorderStyle = BorderStyle.FixedSingle;
            GroupeFichiersTuiles.Controls.Add(EtiquetteRepertoireTuiles);
            GroupeFichiersTuiles.Controls.Add(LabelRepertoireTuiles);
            GroupeFichiersTuiles.Controls.Add(EtiquetteDimTuilesLarg);
            GroupeFichiersTuiles.Controls.Add(DimTuilesLarg);
            GroupeFichiersTuiles.Location = new Point(6, 308);
            GroupeFichiersTuiles.Name = "GroupeFichiersTuiles";
            GroupeFichiersTuiles.Size = new Size(424, 88);
            GroupeFichiersTuiles.TabIndex = 68;
            // 
            // EtiquetteDimTuilesLarg
            // 
            EtiquetteDimTuilesLarg.BackColor = Color.Transparent;
            EtiquetteDimTuilesLarg.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteDimTuilesLarg.Cursor = Cursors.Default;
            EtiquetteDimTuilesLarg.FlatStyle = FlatStyle.Flat;
            EtiquetteDimTuilesLarg.Location = new Point(210, -1);
            EtiquetteDimTuilesLarg.Margin = new Padding(0);
            EtiquetteDimTuilesLarg.Name = "EtiquetteDimTuilesLarg";
            EtiquetteDimTuilesLarg.Size = new Size(213, 21);
            EtiquetteDimTuilesLarg.TabIndex = 51;
            EtiquetteDimTuilesLarg.Text = "Dimensions Tuiles";
            EtiquetteDimTuilesLarg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DimTuilesLarg
            // 
            DimTuilesLarg.BackColor = Color.White;
            DimTuilesLarg.BorderStyle = BorderStyle.FixedSingle;
            DimTuilesLarg.Increment = 256m;
            DimTuilesLarg.Location = new Point(210, 19);
            DimTuilesLarg.Margin = new Padding(0);
            DimTuilesLarg.Maximum = 1024m;
            DimTuilesLarg.Minimum = 256m;
            DimTuilesLarg.Name = "DimTuilesLarg";
            DimTuilesLarg.ReadOnly = true;
            DimTuilesLarg.Size = new Size(213, 26);
            DimTuilesLarg.TabIndex = 52;
            DimTuilesLarg.TabStop = false;
            DimTuilesLarg.TextAlign = HorizontalAlignment.Center;
            Informations.SetToolTip(DimTuilesLarg, resources.GetString("DimTuilesLarg.ToolTip"));
            DimTuilesLarg.Value = 256m;
            // 
            // EtiquetteRepertoireTuiles
            // 
            EtiquetteRepertoireTuiles.BackColor = Color.Transparent;
            EtiquetteRepertoireTuiles.BorderStyle = BorderStyle.FixedSingle;
            EtiquetteRepertoireTuiles.Location = new Point(-1, 44);
            EtiquetteRepertoireTuiles.Margin = new Padding(0);
            EtiquetteRepertoireTuiles.Name = "EtiquetteCheminTuiles";
            EtiquetteRepertoireTuiles.Size = new Size(425, 21);
            EtiquetteRepertoireTuiles.TabIndex = 0;
            EtiquetteRepertoireTuiles.Text = "Répertoire d'enregistrement des fichiers Tuiles";
            EtiquetteRepertoireTuiles.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelRepertoireTuiles
            // 
            LabelRepertoireTuiles.BackColor = Color.White;
            LabelRepertoireTuiles.BorderStyle = BorderStyle.FixedSingle;
            LabelRepertoireTuiles.Location = new Point(-1, 64);
            LabelRepertoireTuiles.Margin = new Padding(0);
            LabelRepertoireTuiles.Name = "LabelCheminTuiles";
            LabelRepertoireTuiles.Size = new Size(424, 23);
            LabelRepertoireTuiles.TabIndex = 0;
            LabelRepertoireTuiles.TextAlign = ContentAlignment.MiddleLeft;
            Informations.SetToolTip(LabelRepertoireTuiles, "Indique le répertoire où seront enregistrés les fichiers tuiles.");
            #endregion
            // 
            // GroupeConfigurations
            // 
            GroupeConfigurations.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GroupeConfigurations.BackColor = Color.FromArgb(247, 247, 247);
            GroupeConfigurations.Controls.Add(EtiquetteGroupeConfigurationCarte);
            GroupeConfigurations.Controls.Add(LabelGeoRefs);
            GroupeConfigurations.Controls.Add(EtiquetteGeoRefs);
            GroupeConfigurations.Controls.Add(GeoRefs);
            GroupeConfigurations.Controls.Add(GroupeCartes);
            GroupeConfigurations.Controls.Add(EtiquetteGroupeConfigurationTraces);
            GroupeConfigurations.Controls.Add(LabelTraces);
            GroupeConfigurations.Controls.Add(EtiquetteTraces);
            GroupeConfigurations.Controls.Add(Traces);
            GroupeConfigurations.Controls.Add(EtiquetteBoutonTrace);
            GroupeConfigurations.Controls.Add(GroupeConfigurationTraces);
            GroupeConfigurations.Controls.Add(IsTrace);
            GroupeConfigurations.Controls.Add(EtiquetteIsTrace);
            GroupeConfigurations.Controls.Add(EtiquetteNbPaths);
            GroupeConfigurations.Controls.Add(NbPaths);
            GroupeConfigurations.Controls.Add(LabelFichiersTuiles);
            GroupeConfigurations.Controls.Add(EtiquetteFichiersTuiles);
            GroupeConfigurations.Controls.Add(FichiersTuiles);
            GroupeConfigurations.Controls.Add(EtiquetteGroupeConfigurationFichiersTuiles);
            GroupeConfigurations.Controls.Add(GroupeFichiersTuiles);
            GroupeConfigurations.Location = new Point(0, 0);
            GroupeConfigurations.Name = "GroupeConfigurations";
            GroupeConfigurations.Size = new Size(434, 471);
            GroupeConfigurations.TabIndex = 0;
            // 
            // Information
            // 
            Information.BackColor = Color.FromArgb(247, 247, 247);
            Information.BorderStyle = BorderStyle.FixedSingle;
            Information.Location = new Point(6, 402);
            Information.TextAlign = ContentAlignment.TopLeft;
            Information.Name = "Information";
            Information.Size = new Size(424, 63);
            Information.TabIndex = 13;
            Informations.SetToolTip(Information, "Zone d'information sur les différentes phases de traitement des cartes.");
            // 
            // Quitter
            // 
            Quitter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Quitter.Location = new Point(303, 477);
            Quitter.Name = "Quitter";
            Quitter.Size = new Size(128, 33);
            Quitter.TabIndex = 15;
            Quitter.Text = "Quitter";
            Informations.SetToolTip(Quitter, "Quitter FCGP_Convert.");
            Quitter.UseVisualStyleBackColor = true;
            // 
            // Traitement
            // 
            Traitement.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Traitement.Location = new Point(6, 477);
            Traitement.Name = "Traitement";
            Traitement.Size = new Size(293, 33);
            Traitement.TabIndex = 14;
            Traitement.TabStop = false;
            Informations.SetToolTip(Traitement, "Choisir une carte ou des cartes pour lancer le traitement.");
            Traitement.UseVisualStyleBackColor = true;
            // 
            // OuvrirFichiersCarte
            // 
            OuvrirFichiersCarte.Filter = "GEOREF Files|*.GEOREF";
            OuvrirFichiersCarte.Multiselect = true;
            OuvrirFichiersCarte.ReadOnlyChecked = true;
            OuvrirFichiersCarte.Title = "Choix du ou des fichiers Georef";
            // 
            // OuvrirFichiersTrace
            // 
            OuvrirFichiersTrace.Filter = "Traces FCGP|*.trk";
            OuvrirFichiersTrace.Multiselect = true;
            OuvrirFichiersTrace.ReadOnlyChecked = true;
            OuvrirFichiersTrace.Title = "Choisir une ou plusieurs traces";
            // 
            // Convertir
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(436, 523);
            Controls.Add(Quitter);
            Controls.Add(Traitement);
            Controls.Add(Information);
            Controls.Add(GroupeConfigurations);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Convertir";
            Text = "ConvertirCarte";
            StartPosition = FormStartPosition.CenterScreen;
            GroupeConfigurations.ResumeLayout(false);
            GroupeCartes.ResumeLayout(false);
            GroupeConfigurationTraces.ResumeLayout(false);
            GroupeConfigurationTraces.PerformLayout();
            ((ISupportInitialize)Epaisseur).EndInit();
            ((ISupportInitialize)Transparence).EndInit();
            ((ISupportInitialize)NbPaths).EndInit();
            GroupeFichiersTuiles.ResumeLayout(false);
            ((ISupportInitialize)DimTuilesLarg).EndInit();
            ResumeLayout(false);
        }

        private Panel GroupeConfigurations;
        private CheckBox IsTrace;
        private Label EtiquetteIsTrace;
        private Label EtiquetteTraces;
        private ComboBox Traces;
        private Label EtiquetteNbPaths;
        private Label EtiquetteDimTuilesLarg;
        private Label EtiquetteEpaisseur;
        private NumericUpDown Epaisseur;
        private Label Couleur;
        private Label EtiquetteTransparence;
        private NumericUpDown Transparence;
        private Label LabelTraces;
        private Label EtiquetteCouleur;
        private Label EtiquetteBoutonTrace;
        private Label EtiquetteFichiersTuiles;
        private Label LabelRepertoireTuiles;
        private Label EtiquetteRepertoireTuiles;
        private ComboBox FichiersTuiles;
        private CheckBox IsGrille;
        private Label EtiquetteIsGrille;
        private Label EtiquetteGeoRefs;
        private ComboBox GeoRefs;
        private Label LabelGeoRefs;
        private Label EtiquetteFormatCarte;
        private ComboBox FormatCartes;
        private Label LabelFormatCartes;
        private Label EtiquetteReferences;
        private ComboBox References;
        private Label LabelRepertoireCartes;
        private Label EtiquetteRepertoireCartes;
        private Label LabelReferences;
        private Button Quitter;
        private Label Information;
        private Button Traitement;
        private OpenFileDialog OuvrirFichiersCarte;
        private ToolTip Informations;
        private ColorDialog ChoixCouleurTrace;
        private OpenFileDialog OuvrirFichiersTrace;
        private Panel GroupeConfigurationTraces;
        private Label EtiquetteGroupeConfigurationTraces;
        private NumericUpDown NbPaths;
        private Panel GroupeCartes;
        private Label EtiquetteGroupeConfigurationCarte;
        private Label EtiquetteGroupeConfigurationFichiersTuiles;
        private Label LabelFichiersTuiles;
        private NumericUpDown DimTuilesLarg;
        private Panel GroupeFichiersTuiles;
    }
}