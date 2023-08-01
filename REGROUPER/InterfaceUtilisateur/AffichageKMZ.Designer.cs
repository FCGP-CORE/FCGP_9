using static FCGP.Commun;

namespace FCGP
{
    internal partial class AffichageKMZ : Form
    {

        internal AffichageKMZ()
        {
            InitializeComponent();
            InitialiseEvenements();
        }
        private void InitialiseEvenements()
        {
            // contrôles simples
            Choisir.Click += ChoixFichierKMZ_Click;
            Modifier.Click += ModifierTuiles_Click;
            Selectionner.Click += Selectionner_Click;
            Load += AffichageKMZ_Load;
            FormClosed += AffichageKMZ_FormClosed;
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
            var resources = new ComponentResourceManager(typeof(AffichageKMZ));
            Groupe = new Panel();
            LabelInformation = new Label();
            Label4 = new Label();
            Label3 = new Label();
            Label2 = new Label();
            Choisir = new Button();
            Modifier = new Button();
            Quitter = new Button();
            ToolTip1 = new ToolTip(components);
            Selectionner = new Button();
            Label5 = new Label();
            TailleTuile = new Label();
            TailleFichier = new Label();
            NbTuilesFichier = new Label();
            NbTuilesSelect = new Label();
            ChercheFichierKMZ = new OpenFileDialog();
            Label1 = new Label();
            NomFichier = new Label();
            Label7 = new Label();
            Groupe.SuspendLayout();
            SuspendLayout();
            // 
            // LabelInformation
            // 
            LabelInformation.BackColor = Color.Transparent;
            LabelInformation.BorderStyle = BorderStyle.FixedSingle;
            LabelInformation.Location = new Point(6, 6);
            LabelInformation.Name = "LabelInformation";
            LabelInformation.Size = new Size(412, 29);
            LabelInformation.TabIndex = 0;
            LabelInformation.Text = "Informations fichier sélectionné";
            LabelInformation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label7
            // 
            Label7.BackColor = Color.Transparent;
            Label7.BorderStyle = BorderStyle.FixedSingle;
            Label7.Location = new Point(6, 34);
            Label7.Name = "Label7";
            Label7.Size = new Size(207, 23);
            Label7.TabIndex = 0;
            Label7.Text = "Nom du Fichier";
            Label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NomFichier
            // 
            NomFichier.BackColor = Color.Transparent;
            NomFichier.BorderStyle = BorderStyle.FixedSingle;
            NomFichier.Location = new Point(212, 34);
            NomFichier.Name = "NomFichier";
            NomFichier.Size = new Size(206, 23);
            NomFichier.TabIndex = 0;
            NomFichier.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NomFichier, "Nom du fichier KMZ à traiter");
            // 
            // Label2
            // 
            Label2.BackColor = Color.Transparent;
            Label2.BorderStyle = BorderStyle.FixedSingle;
            Label2.Location = new Point(6, 56);
            Label2.Name = "Label2";
            Label2.Size = new Size(207, 23);
            Label2.TabIndex = 0;
            Label2.Text = "Nb Tuiles du Fichier";
            Label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NbTuilesFichier
            // 
            NbTuilesFichier.BackColor = Color.Transparent;
            NbTuilesFichier.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesFichier.Location = new Point(212, 56);
            NbTuilesFichier.Name = "NbTuilesFichier";
            NbTuilesFichier.Size = new Size(206, 23);
            NbTuilesFichier.TabIndex = 0;
            NbTuilesFichier.Text = "0";
            NbTuilesFichier.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuilesFichier, "Nb de tuiles du fichier KMZ (Colonnes * Rangées)");
            // 
            // Label3
            // 
            Label3.BackColor = Color.Transparent;
            Label3.BorderStyle = BorderStyle.FixedSingle;
            Label3.Location = new Point(6, 78);
            Label3.Name = "Label3";
            Label3.Size = new Size(207, 23);
            Label3.TabIndex = 0;
            Label3.Text = "Taille du Fichier en pixels";
            Label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TailleFichier
            // 
            TailleFichier.BackColor = Color.Transparent;
            TailleFichier.BorderStyle = BorderStyle.FixedSingle;
            TailleFichier.Location = new Point(212, 78);
            TailleFichier.Name = "TailleFichier";
            TailleFichier.Size = new Size(206, 23);
            TailleFichier.TabIndex = 0;
            TailleFichier.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(TailleFichier, "Taille de la carte en pixels");
            // 
            // Label5
            // 
            Label5.BackColor = Color.Transparent;
            Label5.BorderStyle = BorderStyle.FixedSingle;
            Label5.Location = new Point(6, 100);
            Label5.Name = "Label5";
            Label5.Size = new Size(207, 23);
            Label5.TabIndex = 0;
            Label5.Text = "Taille des Tuiles en pixels";
            Label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TailleTuile
            // 
            TailleTuile.BackColor = Color.Transparent;
            TailleTuile.BorderStyle = BorderStyle.FixedSingle;
            TailleTuile.Location = new Point(212, 100);
            TailleTuile.Name = "TailleTuile";
            TailleTuile.Size = new Size(206, 23);
            TailleTuile.TabIndex = 0;
            TailleTuile.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(TailleTuile, "Taille des tuiles en pixels");
            // 
            // Label1
            // 
            Label1.BackColor = Color.Transparent;
            Label1.BorderStyle = BorderStyle.FixedSingle;
            Label1.Location = new Point(6, 122);
            Label1.Name = "Label1";
            Label1.Size = new Size(412, 29);
            Label1.TabIndex = 0;
            Label1.Text = "Tuiles à supprimer";
            Label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label4
            // 
            Label4.BackColor = Color.Transparent;
            Label4.BorderStyle = BorderStyle.FixedSingle;
            Label4.Location = new Point(6, 150);
            Label4.Name = "Label4";
            Label4.Size = new Size(207, 23);
            Label4.TabIndex = 0;
            Label4.Text = "Nb Tuiles sélectionnées";
            Label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NbTuilesSelect
            // 
            NbTuilesSelect.BackColor = Color.Transparent;
            NbTuilesSelect.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesSelect.Location = new Point(212, 150);
            NbTuilesSelect.Name = "NbTuilesSelect";
            NbTuilesSelect.Size = new Size(206, 23);
            NbTuilesSelect.TabIndex = 0;
            NbTuilesSelect.Tag = "";
            NbTuilesSelect.Text = "0";
            NbTuilesSelect.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuilesSelect, "Nb de tuiles sélectionnées pour être supprimées");
            // 
            // Groupe
            // 
            Groupe.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Groupe.BackColor = Color.FromArgb(247, 247, 247);
            Controls.Add(NomFichier);
            Controls.Add(Label7);
            Controls.Add(NbTuilesSelect);
            Controls.Add(TailleTuile);
            Controls.Add(TailleFichier);
            Controls.Add(NbTuilesFichier);
            Controls.Add(Label5);
            Controls.Add(Label1);
            Controls.Add(LabelInformation);
            Controls.Add(Label4);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Groupe.Location = new Point(0, 0);
            Groupe.Name = "Groupe";
            Groupe.Size = new Size(424, 179);
            Groupe.TabIndex = 0;
            // 
            // Choisir
            // 
            Choisir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Choisir.Location = new Point(6, 185);
            Choisir.Name = "Choisir";
            Choisir.Size = new Size(100, 46);
            Choisir.TabIndex = 1;
            Choisir.Text = "Choisir" + CrLf + "Fichier KMZ";
            ToolTip1.SetToolTip(Choisir, "Ouvre un formulaire qui permet de choisir un fichier KMZ.");
            Choisir.UseVisualStyleBackColor = true;
            // 
            // Selectionner
            // 
            Selectionner.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Selectionner.Location = new Point(110, 185);
            Selectionner.Name = "Selectionner";
            Selectionner.Size = new Size(100, 46);
            Selectionner.TabIndex = 2;
            Selectionner.Text = "Selectionner" + CrLf + "Tuiles";
            ToolTip1.SetToolTip(Selectionner, "Ouvre le plan de calepinage des tuiles du fichier KMZ" + CrLf + 
                                              "pour sélectionner les tuiles à supprimer");
            Selectionner.UseVisualStyleBackColor = true;
            // 
            // Modifier
            // 
            Modifier.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Modifier.Location = new Point(214, 185);
            Modifier.Name = "Modifier";
            Modifier.Size = new Size(100, 46);
            Modifier.TabIndex = 3;
            Modifier.Text = "Modifier" + CrLf + "Fichier KMZ";
            ToolTip1.SetToolTip(Modifier, "Supprime les tuiles sélectionnées du ficheir KMZ");
            Modifier.UseVisualStyleBackColor = true;
            // 
            // Quitter
            // 
            Quitter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Quitter.DialogResult = DialogResult.Cancel;
            Quitter.Location = new Point(318, 185);
            Quitter.Name = "Quitter";
            Quitter.Size = new Size(100, 46);
            Quitter.TabIndex = 4;
            Quitter.Text = "Quitter";
            ToolTip1.SetToolTip(Quitter, "Ferme le formulaire");
            Quitter.UseVisualStyleBackColor = true;
            // 
            // AffichageKMZ
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            CancelButton = Quitter;
            AcceptButton = Choisir;
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            ClientSize = new Size(424, 236);
            ControlBox = false;
            Controls.Add(Groupe);
            Controls.Add(Selectionner);
            Controls.Add(Quitter);
            Controls.Add(Modifier);
            Controls.Add(Choisir);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AffichageKMZ";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Suppression Tuiles des Fichiers KMZ";
            Groupe.ResumeLayout(false);
            Groupe.PerformLayout();
            ResumeLayout(false);
        }

        private Label LabelInformation;
        private Label Label4;
        private Label Label3;
        private Label Label2;
        private Button Choisir;
        private Button Modifier;
        private Button Quitter;
        private ToolTip ToolTip1;
        private OpenFileDialog ChercheFichierKMZ;
        private Button Selectionner;
        private Label Label1;
        private Label Label5;
        private Label TailleTuile;
        private Label TailleFichier;
        private Label NbTuilesFichier;
        private Label NbTuilesSelect;
        private Label NomFichier;
        private Label Label7;
        private Panel Groupe;
    }
}