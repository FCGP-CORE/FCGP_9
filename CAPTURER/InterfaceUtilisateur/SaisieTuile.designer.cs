namespace FCGP
{
    internal partial class SaisieTuile : Form
    {

        internal SaisieTuile()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            Row.KeyDown += Coord_KeyDown;
            Row.KeyPress += Coord_KeyPress;
            DecalY.KeyDown += Coord_KeyDown;
            DecalY.KeyPress += Coord_KeyPress;
            DecalX.KeyDown += Coord_KeyDown;
            DecalX.KeyPress += Coord_KeyPress;
            Col.KeyDown += Coord_KeyDown;
            Col.KeyPress += Coord_KeyPress;
            // formulaire
            Load += SaisieTuile_Load;
            FormClosing += SaisieTuile_FormClosing;
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
#pragma warning disable CS0649
        private IContainer components;
#pragma warning restore CS0649
        // REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        // Elle peut être modifiée à l'aide du Concepteur Windows Form.  
        // Ne la modifiez pas à l'aide de l'éditeur de code.
        private void InitializeComponent()
        {
            L_Col = new Label();
            Row = new TextBox();
            L_Row = new Label();
            ESC = new Button();
            OK = new Button();
            L_IndexTuile = new Label();
            L_DecalagePixels = new Label();
            DecalY = new TextBox();
            DecalX = new TextBox();
            Col = new TextBox();
            Support = new Panel();
            SuspendLayout();
            // 
            // L_IndexTuile
            // 
            L_IndexTuile.BorderStyle = BorderStyle.FixedSingle;
            L_IndexTuile.Location = new Point(45, 1);
            L_IndexTuile.Margin = new Padding(0);
            L_IndexTuile.Name = "L_IndexTuile";
            L_IndexTuile.Size = new Size(85, 21);
            L_IndexTuile.TabIndex = 20;
            L_IndexTuile.Text = "Index Tuile";
            L_IndexTuile.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_DecalagePixels
            // 
            L_DecalagePixels.BorderStyle = BorderStyle.FixedSingle;
            L_DecalagePixels.Location = new Point(129, 1);
            L_DecalagePixels.Margin = new Padding(0);
            L_DecalagePixels.Name = "L_DecalagePixels";
            L_DecalagePixels.Size = new Size(85, 21);
            L_DecalagePixels.TabIndex = 20;
            L_DecalagePixels.Text = "Decalage Pixels";
            L_DecalagePixels.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Col
            // 
            L_Col.BorderStyle = BorderStyle.FixedSingle;
            L_Col.Location = new Point(1, 21);
            L_Col.Margin = new Padding(0);
            L_Col.Name = "L_Col";
            L_Col.Size = new Size(45, 26);
            L_Col.TabIndex = 20;
            L_Col.Text = "Col : ";
            L_Col.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Col
            // 
            Col.BackColor = Color.White;
            Col.BorderStyle = BorderStyle.FixedSingle;
            Col.Location = new Point(45, 21);
            Col.Margin = new Padding(0);
            Col.MaxLength = 5;
            Col.Name = "Col";
            Col.Size = new Size(85, 26);
            Col.TabIndex = 0;
            // 
            // DecalX
            // 
            DecalX.BackColor = Color.White;
            DecalX.BorderStyle = BorderStyle.FixedSingle;
            DecalX.Location = new Point(129, 21);
            DecalX.Margin = new Padding(0);
            DecalX.MaxLength = 3;
            DecalX.Name = "DecalX";
            DecalX.Size = new Size(85, 26);
            DecalX.TabIndex = 1;
            // 
            // L_Row
            // 
            L_Row.BorderStyle = BorderStyle.FixedSingle;
            L_Row.Location = new Point(1, 46);
            L_Row.Margin = new Padding(0);
            L_Row.Name = "L_Row";
            L_Row.Size = new Size(45, 26);
            L_Row.TabIndex = 20;
            L_Row.Text = "Row : ";
            L_Row.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Row
            // 
            Row.BackColor = Color.White;
            Row.BorderStyle = BorderStyle.FixedSingle;
            Row.Location = new Point(45, 46);
            Row.Margin = new Padding(0);
            Row.MaxLength = 5;
            Row.Name = "Row";
            Row.Size = new Size(85, 26);
            Row.TabIndex = 2;
            // 
            // DecalY
            // 
            DecalY.BackColor = Color.White;
            DecalY.BorderStyle = BorderStyle.FixedSingle;
            DecalY.Location = new Point(129, 46);
            DecalY.Margin = new Padding(0);
            DecalY.MaxLength = 3;
            DecalY.Name = "DecalY";
            DecalY.Size = new Size(85, 26);
            DecalY.TabIndex = 3;
            // 
            // Support
            // 
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(215, 78);
            Support.TabIndex = 62;
            Support.Controls.Add(L_IndexTuile);
            Support.Controls.Add(L_DecalagePixels);
            Support.Controls.Add(L_Col);
            Support.Controls.Add(Col);
            Support.Controls.Add(DecalX);
            Support.Controls.Add(L_Row);
            Support.Controls.Add(Row);
            Support.Controls.Add(DecalY);
            // 
            // ESC
            // 
            ESC.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ESC.DialogResult = DialogResult.Cancel;
            ESC.Location = new Point(63, 84);
            ESC.Name = "ESC";
            ESC.Size = new Size(70, 30);
            ESC.TabIndex = 4;
            ESC.TabStop = false;
            ESC.Text = "Annuler";
            ESC.UseVisualStyleBackColor = true;
            // 
            // OK
            // 
            OK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OK.DialogResult = DialogResult.OK;
            OK.Location = new Point(139, 84);
            OK.Name = "OK";
            OK.Size = new Size(70, 30);
            OK.TabIndex = 5;
            OK.TabStop = false;
            OK.Text = "Valider";
            OK.UseVisualStyleBackColor = true;
            // 
            // SaisieTuile
            // 
            AcceptButton = OK;
            AccessibleRole = AccessibleRole.Cursor;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = ESC;
            ClientSize = new Size(215, 120);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(OK);
            Controls.Add(ESC);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaisieTuile";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private Label L_Col;
        private TextBox Row;
        private Label L_Row;
        private Button ESC;
        private Button OK;
        private Label L_IndexTuile;
        private Label L_DecalagePixels;
        private TextBox DecalY;
        private TextBox DecalX;
        private TextBox Col;
        private Panel Support;
    }
}