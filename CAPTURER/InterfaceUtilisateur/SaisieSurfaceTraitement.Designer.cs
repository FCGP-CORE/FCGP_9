using static FCGP.Commun;

namespace FCGP
{
    internal partial class SaisieSurfaceTraitement : Form
    {

        internal SaisieSurfaceTraitement()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            DebCol.Click += IndexNombreTuile_Click;
            NbCol.Click += IndexNombreTuile_Click;
            DebRow.Click += IndexNombreTuile_Click;
            NbRow.Click += IndexNombreTuile_Click;
            Valider.Click += Fermer_Click;
            Point1.Click += Point_Click;
            Point2.Click += Point_Click;
            Annuler.Click += Fermer_Click;
            // SaisieIndex
            SaisieIndex.DoubleClick += SaisieIndex_DoubleClick;
            SaisieIndex.PreviewKeyDown += SaisieIndex_PreviewKeyDown;
            SaisieIndex.KeyUp += SaisieIndex_KeyUp;
            SaisieIndex.KeyDown += SaisieIndex_KeyDown;
            // formulaire
            Load += SurfaceCaptureTuile_Load;
            FormClosed += SurfaceCaptureTuile_FormClosed;
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
            var resources = new ComponentResourceManager(typeof(SaisieSurfaceTraitement));
            DebCol = new Label();
            NbCol = new Label();
            DebRow = new Label();
            NbRow = new Label();
            Valider = new Button();
            L_Delimitation = new Label();
            L_PtNO = new Label();
            L_PtSE = new Label();
            L_Largeur = new Label();
            L_Hauteur = new Label();
            Point1 = new Label();
            Hauteur = new Label();
            Largeur = new Label();
            Point2 = new Label();
            Surface = new Label();
            L_NbTuilesMax = new Label();
            L_TuilesTelechargees = new Label();
            NbTuilesMax = new Label();
            L_NbTuilesMaxX = new Label();
            NbTuilesMaxX = new Label();
            L_Rangees = new Label();
            L_Colonnes = new Label();
            L_IndexCol = new Label();
            L_NbCol = new Label();
            L_NbRow = new Label();
            L_IndexRow = new Label();
            SaisieIndex = new TextBox();
            Annuler = new Button();
            ToolTip1 = new ToolTip(components);
            L_Surface = new Label();
            NbTuilesTelecharger = new Label();
            L_NbTuilesTelecharger = new Label();
            Support = new Panel();
            Support.SuspendLayout();
            SuspendLayout();
            // 
            // L_Delimitation
            // 
            L_Delimitation.BackColor = Color.Transparent;
            L_Delimitation.BorderStyle = BorderStyle.FixedSingle;
            L_Delimitation.Location = new Point(6, 6);
            L_Delimitation.Margin = new Padding(0);
            L_Delimitation.Name = "L_Delimitation";
            L_Delimitation.Size = new Size(341, 21);
            L_Delimitation.TabIndex = 22;
            L_Delimitation.Text = "Points de délimitation";
            L_Delimitation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_PtNO
            // 
            L_PtNO.BackColor = Color.Transparent;
            L_PtNO.BorderStyle = BorderStyle.FixedSingle;
            L_PtNO.Location = new Point(6, 26);
            L_PtNO.Margin = new Padding(0);
            L_PtNO.Name = "L_PtNO";
            L_PtNO.Size = new Size(86, 21);
            L_PtNO.TabIndex = 24;
            L_PtNO.Tag = "";
            L_PtNO.Text = "Point NO";
            L_PtNO.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Point1
            // 
            Point1.BackColor = Color.White;
            Point1.BorderStyle = BorderStyle.FixedSingle;
            Point1.Location = new Point(91, 26);
            Point1.Margin = new Padding(0);
            Point1.Name = "Point1";
            Point1.Size = new Size(256, 21);
            Point1.TabIndex = 31;
            Point1.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Point1, resources.GetString("Point1.ToolTip"));
            // 
            // L_PtSE
            // 
            L_PtSE.BackColor = Color.Transparent;
            L_PtSE.BorderStyle = BorderStyle.FixedSingle;
            L_PtSE.Location = new Point(6, 46);
            L_PtSE.Margin = new Padding(0);
            L_PtSE.Name = "L_PtSE";
            L_PtSE.Size = new Size(86, 21);
            L_PtSE.TabIndex = 25;
            L_PtSE.Text = "Point SE";
            L_PtSE.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Point2
            // 
            Point2.BackColor = Color.White;
            Point2.BorderStyle = BorderStyle.FixedSingle;
            Point2.Location = new Point(91, 46);
            Point2.Margin = new Padding(0);
            Point2.Name = "Point2";
            Point2.Size = new Size(256, 21);
            Point2.TabIndex = 35;
            Point2.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Point2, resources.GetString("Point2.ToolTip"));
            // 
            // L_Largeur
            // 
            L_Largeur.BackColor = Color.Transparent;
            L_Largeur.BorderStyle = BorderStyle.FixedSingle;
            L_Largeur.Location = new Point(6, 66);
            L_Largeur.Margin = new Padding(0);
            L_Largeur.Name = "L_Largeur";
            L_Largeur.Size = new Size(86, 21);
            L_Largeur.TabIndex = 29;
            L_Largeur.Text = "Largeur (m)";
            L_Largeur.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Largeur
            // 
            Largeur.BackColor = Color.Transparent;
            Largeur.BorderStyle = BorderStyle.FixedSingle;
            Largeur.Location = new Point(91, 66);
            Largeur.Margin = new Padding(0);
            Largeur.Name = "Largeur";
            Largeur.Size = new Size(86, 21);
            Largeur.TabIndex = 33;
            Largeur.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Largeur, "Largeur approximative de la zone de téléchargement exprimée en mètres.");
            // 
            // L_Surface
            // 
            L_Surface.BackColor = Color.Transparent;
            L_Surface.BorderStyle = BorderStyle.FixedSingle;
            L_Surface.Location = new Point(176, 66);
            L_Surface.Margin = new Padding(0);
            L_Surface.Name = "L_Surface";
            L_Surface.Size = new Size(171, 21);
            L_Surface.TabIndex = 61;
            L_Surface.Text = "Surface (km²) : ";
            L_Surface.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Hauteur
            // 
            L_Hauteur.BackColor = Color.Transparent;
            L_Hauteur.BorderStyle = BorderStyle.FixedSingle;
            L_Hauteur.Location = new Point(6, 86);
            L_Hauteur.Margin = new Padding(0);
            L_Hauteur.Name = "L_Hauteur";
            L_Hauteur.Size = new Size(86, 21);
            L_Hauteur.TabIndex = 30;
            L_Hauteur.Text = "Hauteur (m)";
            L_Hauteur.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Hauteur
            // 
            Hauteur.BackColor = Color.Transparent;
            Hauteur.BorderStyle = BorderStyle.FixedSingle;
            Hauteur.Location = new Point(91, 86);
            Hauteur.Margin = new Padding(0);
            Hauteur.Name = "Hauteur";
            Hauteur.Size = new Size(86, 21);
            Hauteur.TabIndex = 34;
            Hauteur.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Hauteur, "Hauteur approximative de la zone de téléchargement exprimée en mètres.");
            // 
            // Surface
            // 
            Surface.BackColor = Color.Transparent;
            Surface.BorderStyle = BorderStyle.FixedSingle;
            Surface.Location = new Point(176, 86);
            Surface.Margin = new Padding(0);
            Surface.Name = "Surface";
            Surface.Size = new Size(171, 21);
            Surface.TabIndex = 37;
            Surface.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(Surface, "Surface approximative de la zone de téléchargement exprimée en kilomètres carrés.");
            // 
            // L_Colonnes
            // 
            L_Colonnes.BackColor = Color.Transparent;
            L_Colonnes.BorderStyle = BorderStyle.FixedSingle;
            L_Colonnes.Location = new Point(6, 112);
            L_Colonnes.Margin = new Padding(0);
            L_Colonnes.Name = "L_Colonnes";
            L_Colonnes.Size = new Size(171, 21);
            L_Colonnes.TabIndex = 48;
            L_Colonnes.Text = "Colonnes : Ouest-Est";
            L_Colonnes.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Rangees
            // 
            L_Rangees.BackColor = Color.Transparent;
            L_Rangees.BorderStyle = BorderStyle.FixedSingle;
            L_Rangees.Location = new Point(176, 112);
            L_Rangees.Margin = new Padding(0);
            L_Rangees.Name = "L_Rangees";
            L_Rangees.Size = new Size(171, 21);
            L_Rangees.TabIndex = 49;
            L_Rangees.Text = "Rangées : Nord-Sud";
            L_Rangees.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_IndexCol
            // 
            L_IndexCol.BackColor = Color.Transparent;
            L_IndexCol.BorderStyle = BorderStyle.FixedSingle;
            L_IndexCol.Location = new Point(6, 132);
            L_IndexCol.Margin = new Padding(0);
            L_IndexCol.Name = "L_IndexCol";
            L_IndexCol.Size = new Size(86, 21);
            L_IndexCol.TabIndex = 50;
            L_IndexCol.Text = "Index";
            L_IndexCol.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_NbCol
            // 
            L_NbCol.BackColor = Color.Transparent;
            L_NbCol.BorderStyle = BorderStyle.FixedSingle;
            L_NbCol.Location = new Point(91, 132);
            L_NbCol.Margin = new Padding(0);
            L_NbCol.Name = "L_NbCol";
            L_NbCol.Size = new Size(86, 21);
            L_NbCol.TabIndex = 51;
            L_NbCol.Text = "Nombre";
            L_NbCol.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_IndexRow
            // 
            L_IndexRow.BackColor = Color.Transparent;
            L_IndexRow.BorderStyle = BorderStyle.FixedSingle;
            L_IndexRow.Location = new Point(176, 132);
            L_IndexRow.Margin = new Padding(0);
            L_IndexRow.Name = "L_IndexRow";
            L_IndexRow.Size = new Size(86, 21);
            L_IndexRow.TabIndex = 52;
            L_IndexRow.Text = "Index";
            L_IndexRow.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_NbRow
            // 
            L_NbRow.BackColor = Color.Transparent;
            L_NbRow.BorderStyle = BorderStyle.FixedSingle;
            L_NbRow.Location = new Point(261, 132);
            L_NbRow.Margin = new Padding(0);
            L_NbRow.Name = "L_NbRow";
            L_NbRow.Size = new Size(86, 21);
            L_NbRow.TabIndex = 53;
            L_NbRow.Text = "Nombre";
            L_NbRow.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DebCol
            // 
            DebCol.BackColor = Color.White;
            DebCol.BorderStyle = BorderStyle.FixedSingle;
            DebCol.Location = new Point(6, 152);
            DebCol.Margin = new Padding(0);
            DebCol.Name = "DebCol";
            DebCol.Size = new Size(86, 21);
            DebCol.TabIndex = 54;
            DebCol.Tag = 0;
            DebCol.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(DebCol, "Index de colonne de la tuile en haut et à gauche (NO)" + CrLf + 
                                        "de la surface de téléchargement.");
            // 
            // NbCol
            // 
            NbCol.BackColor = Color.White;
            NbCol.BorderStyle = BorderStyle.FixedSingle;
            NbCol.Location = new Point(91, 152);
            NbCol.Margin = new Padding(0);
            NbCol.Name = "NbCol";
            NbCol.Size = new Size(86, 21);
            NbCol.TabIndex = 55;
            NbCol.Tag = 1;
            NbCol.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbCol, "Nb de colonnes de la surface de téléchargement");
            // 
            // DebRow
            // 
            DebRow.BackColor = Color.White;
            DebRow.BorderStyle = BorderStyle.FixedSingle;
            DebRow.Location = new Point(176, 152);
            DebRow.Margin = new Padding(0);
            DebRow.Name = "DebRow";
            DebRow.Size = new Size(86, 21);
            DebRow.TabIndex = 56;
            DebRow.Tag = 2;
            DebRow.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(DebRow, "Index de rangée de la tuile en haut et à gauche (NO)" + CrLf + 
                                        "de la surface de téléchargement.");
            // 
            // NbRow
            // 
            NbRow.BackColor = Color.White;
            NbRow.BorderStyle = BorderStyle.FixedSingle;
            NbRow.Location = new Point(261, 152);
            NbRow.Margin = new Padding(0);
            NbRow.Name = "NbRow";
            NbRow.Size = new Size(86, 21);
            NbRow.TabIndex = 57;
            NbRow.Tag = 3;
            NbRow.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbRow, "Nb de rangées de la surface de téléchargement.");
            // 
            // L_TuilesTelechargees
            // 
            L_TuilesTelechargees.BackColor = Color.Transparent;
            L_TuilesTelechargees.BorderStyle = BorderStyle.FixedSingle;
            L_TuilesTelechargees.Location = new Point(6, 178);
            L_TuilesTelechargees.Margin = new Padding(0);
            L_TuilesTelechargees.Name = "L_TuilesTelechargees";
            L_TuilesTelechargees.Size = new Size(341, 21);
            L_TuilesTelechargees.TabIndex = 40;
            L_TuilesTelechargees.Text = "Tuiles à télécharger";
            L_TuilesTelechargees.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_NbTuilesMaxX
            // 
            L_NbTuilesMaxX.BackColor = Color.Transparent;
            L_NbTuilesMaxX.BorderStyle = BorderStyle.FixedSingle;
            L_NbTuilesMaxX.Location = new Point(6, 198);
            L_NbTuilesMaxX.Margin = new Padding(0);
            L_NbTuilesMaxX.Name = "L_NbTuilesMaxX";
            L_NbTuilesMaxX.Size = new Size(114, 40);
            L_NbTuilesMaxX.TabIndex = 44;
            L_NbTuilesMaxX.Text = "Nb maxi tuiles" + CrLf + "Axe Colonnes";
            L_NbTuilesMaxX.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_NbTuilesMax
            // 
            L_NbTuilesMax.BackColor = Color.Transparent;
            L_NbTuilesMax.BorderStyle = BorderStyle.FixedSingle;
            L_NbTuilesMax.Location = new Point(119, 198);
            L_NbTuilesMax.Margin = new Padding(0);
            L_NbTuilesMax.Name = "L_NbTuilesMax";
            L_NbTuilesMax.Size = new Size(115, 40);
            L_NbTuilesMax.TabIndex = 39;
            L_NbTuilesMax.Text = "Nb Maxi Tuiles";
            L_NbTuilesMax.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // L_NbTuilesTelecharger
            // 
            L_NbTuilesTelecharger.BackColor = Color.Transparent;
            L_NbTuilesTelecharger.BorderStyle = BorderStyle.FixedSingle;
            L_NbTuilesTelecharger.Location = new Point(233, 198);
            L_NbTuilesTelecharger.Margin = new Padding(0);
            L_NbTuilesTelecharger.Name = "L_NbTuilesTelecharger";
            L_NbTuilesTelecharger.Size = new Size(114, 40);
            L_NbTuilesTelecharger.TabIndex = 59;
            L_NbTuilesTelecharger.Text = "Nb Tuiles" + CrLf + "à télécharger";
            L_NbTuilesTelecharger.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NbTuilesMaxX
            // 
            NbTuilesMaxX.BackColor = Color.Transparent;
            NbTuilesMaxX.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesMaxX.Location = new Point(6, 237);
            NbTuilesMaxX.Margin = new Padding(0);
            NbTuilesMaxX.Name = "NbTuilesMaxX";
            NbTuilesMaxX.Size = new Size(114, 21);
            NbTuilesMaxX.TabIndex = 46;
            NbTuilesMaxX.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuilesMaxX, resources.GetString("NbTuilesMaxX.ToolTip"));
            // 
            // NbTuilesMax
            // 
            NbTuilesMax.BackColor = Color.Transparent;
            NbTuilesMax.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesMax.Location = new Point(119, 237);
            NbTuilesMax.Margin = new Padding(0);
            NbTuilesMax.Name = "NbTuilesMax";
            NbTuilesMax.Size = new Size(115, 21);
            NbTuilesMax.TabIndex = 42;
            NbTuilesMax.TextAlign = ContentAlignment.MiddleCenter;
            ToolTip1.SetToolTip(NbTuilesMax, "Nombre de tuiles maximum qui peuvent être" + CrLf + 
                                             "téléchargées pour le support de carte en cours." + CrLf + 
                                             "Ok si vert, Not Ok si rouge");
            // 
            // NbTuilesTelecharger
            // 
            NbTuilesTelecharger.BackColor = Color.Transparent;
            NbTuilesTelecharger.BorderStyle = BorderStyle.FixedSingle;
            NbTuilesTelecharger.Location = new Point(233, 237);
            NbTuilesTelecharger.Margin = new Padding(0);
            NbTuilesTelecharger.Name = "NbTuilesTelecharger";
            NbTuilesTelecharger.Size = new Size(114, 21);
            NbTuilesTelecharger.TabIndex = 60;
            NbTuilesTelecharger.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Panel1
            // 
            Support.BackColor = Color.FromArgb(247, 247, 247);
            // obligation pour que la touche Tab fonctionne correctement :
            // Tab --> déplacement dans les saisies d'index et de nombres de colonnes et de rangées
            // L'index du contrôle de saisie dans la collection doit correspondre à l'énumération NI
            Support.Controls.Add(DebCol); // NI.DebCol
            Support.Controls.Add(NbCol);  // NI.NbCol
            Support.Controls.Add(DebRow); // NI.DebRow
            Support.Controls.Add(NbRow);  // NI.NbRow
            Support.Controls.Add(L_Delimitation); // 0
            Support.Controls.Add(L_PtNO);
            Support.Controls.Add(Point1);
            Support.Controls.Add(L_PtSE);
            Support.Controls.Add(Point2);
            Support.Controls.Add(L_Largeur);
            Support.Controls.Add(Largeur);
            Support.Controls.Add(L_Surface);
            Support.Controls.Add(Surface);
            Support.Controls.Add(L_Hauteur);
            Support.Controls.Add(Hauteur);
            Support.Controls.Add(L_Colonnes);
            Support.Controls.Add(L_Rangees);
            Support.Controls.Add(L_IndexCol);
            Support.Controls.Add(L_NbCol);
            Support.Controls.Add(L_IndexRow);
            Support.Controls.Add(L_NbRow);
            Support.Controls.Add(L_TuilesTelechargees);
            Support.Controls.Add(L_NbTuilesMaxX);
            Support.Controls.Add(L_NbTuilesMax);
            Support.Controls.Add(L_NbTuilesTelecharger);
            Support.Controls.Add(NbTuilesMax);
            Support.Controls.Add(NbTuilesMaxX);
            Support.Controls.Add(NbTuilesTelecharger);
            Support.Location = new Point(0, 0);
            Support.Name = "Panel1";
            Support.Size = new Size(353, 264);
            Support.TabIndex = 62;

            // 
            // Valider
            // 
            Valider.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Valider.DialogResult = DialogResult.OK;
            Valider.Location = new Point(272, 270);
            Valider.Name = "Valider";
            Valider.Size = new Size(75, 30);
            Valider.TabIndex = 14;
            Valider.Text = "Valider";
            ToolTip1.SetToolTip(Valider, "Ferme le formulaire en prenant en compte les modifications." + CrLf + 
                                         "Le Point NO devient le point Pt1 de l'affichage et " + CrLf + 
                                         "le Point SE devient le point Pt2 de l'affichage.");
            Valider.UseVisualStyleBackColor = true;
            // 
            // Annuler
            // 
            Annuler.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Annuler.DialogResult = DialogResult.Cancel;
            Annuler.Location = new Point(191, 270);
            Annuler.Name = "Annuler";
            Annuler.Size = new Size(75, 30);
            Annuler.TabIndex = 58;
            Annuler.Text = "Annuler";
            ToolTip1.SetToolTip(Annuler, "Ferme le formulaire sans prendre en compte les modifications.");
            Annuler.UseVisualStyleBackColor = true;
            // 
            // ToolTip1
            // 
            ToolTip1.AutoPopDelay = 5000;
            ToolTip1.InitialDelay = 100;
            ToolTip1.ReshowDelay = 100;
            // 
            // SaisieIndex
            // 
            SaisieIndex.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SaisieIndex.BackColor = Color.White;
            SaisieIndex.BorderStyle = BorderStyle.None;
            SaisieIndex.Location = new Point(0, 0);
            SaisieIndex.Margin = new Padding(0);
            SaisieIndex.Name = "SaisieIndex";
            SaisieIndex.Size = new Size(82, 19);
            SaisieIndex.TabIndex = 0;
            SaisieIndex.TabStop = false;
            ToolTip1.SetToolTip(SaisieIndex, "");
            SaisieIndex.Visible = false;
            // 
            // SaisieSurfaceCaptureTuile
            // 
            AcceptButton = Valider;
            AccessibleRole = AccessibleRole.Cursor;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = Annuler;
            ClientSize = new Size(353, 306);
            ControlBox = false;
            Controls.Add(SaisieIndex);
            Controls.Add(Support);
            Controls.Add(Annuler);
            Controls.Add(Valider);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaisieSurfaceCaptureTuile";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            // TopMost = True
            Support.ResumeLayout(false);
            Support.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button Valider;
        private Label L_Delimitation;
        private Label L_PtNO;
        private Label L_PtSE;
        private Label L_Largeur;
        private Label L_Hauteur;
        private Label Point1;
        private Label Hauteur;
        private Label Largeur;
        private Label Point2;
        private Label Surface;
        private Label L_NbTuilesMax;
        private Label L_TuilesTelechargees;
        private Label NbTuilesMax;
        private Label L_NbTuilesMaxX;
        private Label NbTuilesMaxX;
        private Label L_Rangees;
        private Label L_Colonnes;
        private Label L_IndexCol;
        private Label L_NbCol;
        private Label L_NbRow;
        private Label L_IndexRow;
        private Label NbRow;
        private Label DebRow;
        private Label NbCol;
        private Label DebCol;
        private TextBox SaisieIndex;
        private Button Annuler;
        private ToolTip ToolTip1;
        private Label L_NbTuilesTelecharger;
        private Label NbTuilesTelecharger;
        private Label L_Surface;
        private Panel Support;
    }
}