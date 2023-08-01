namespace FCGP
{
    internal partial class SaisieGrille : Form
    {

        internal SaisieGrille()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            CoordX.KeyDown += CoordXY_KeyDown;
            CoordX.KeyPress += CoordXY_KeyPress;
            CoordY.KeyDown += CoordXY_KeyDown;
            CoordY.KeyPress += CoordXY_KeyPress;
            // formulaire
            Load += SaisieGrille_Load;
            FormClosing += SaisieGrille_FormClosing;
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
            components = new Container();
            CoordX = new TextBox();
            L_CoordX = new Label();
            CoordY = new TextBox();
            L_CoordY = new Label();
            Support = new Panel();
            ESC = new Button();
            OK = new Button();
            SuspendLayout();
            // 
            // L_CoordX
            // 
            L_CoordX.BorderStyle = BorderStyle.FixedSingle;
            L_CoordX.Location = new Point(1, 1);
            L_CoordX.Margin = new Padding(0);
            L_CoordX.Name = "L_CoordX";
            L_CoordX.Size = new Size(30, 26);
            L_CoordX.TabIndex = 5;
            L_CoordX.Text = "X : ";
            L_CoordX.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CoordX
            // 
            CoordX.BackColor = Color.White;
            CoordX.BorderStyle = BorderStyle.FixedSingle;
            CoordX.Location = new Point(30, 1);
            CoordX.Margin = new Padding(0);
            CoordX.MaxLength = 9;
            CoordX.Name = "CoordX";
            CoordX.Size = new Size(127, 26);
            CoordX.TabIndex = 0;
            // 
            // L_CoordY
            // 
            L_CoordY.BorderStyle = BorderStyle.FixedSingle;
            L_CoordY.Location = new Point(1, 26);
            L_CoordY.Margin = new Padding(0);
            L_CoordY.Name = "L_CoordY";
            L_CoordY.Size = new Size(30, 26);
            L_CoordY.TabIndex = 5;
            L_CoordY.Text = "Y : ";
            L_CoordY.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CoordY
            // 
            CoordY.BackColor = Color.White;
            CoordY.BorderStyle = BorderStyle.FixedSingle;
            CoordY.Location = new Point(30, 26);
            CoordY.Margin = new Padding(0);
            CoordY.MaxLength = 10;
            CoordY.Name = "CoordY";
            CoordY.Size = new Size(127, 26);
            CoordY.TabIndex = 1;
            // 
            // Support
            // 
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(158, 58);
            Support.TabIndex = 62;
            Support.Controls.Add(L_CoordX);
            Support.Controls.Add(CoordX);
            Support.Controls.Add(L_CoordY);
            Support.Controls.Add(CoordY);
            // 
            // OK
            // 
            OK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OK.DialogResult = DialogResult.OK;
            OK.Location = new Point(82, 64);
            OK.Name = "OK";
            OK.Size = new Size(70, 30);
            OK.TabIndex = 3;
            OK.TabStop = false;
            OK.Text = "Valider";
            OK.UseVisualStyleBackColor = true;
            // 
            // ESC
            // 
            ESC.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ESC.DialogResult = DialogResult.Cancel;
            ESC.Location = new Point(6, 64);
            ESC.Name = "ESC";
            ESC.Size = new Size(70, 30);
            ESC.TabIndex = 2;
            ESC.TabStop = false;
            ESC.Text = "Annuler";
            ESC.UseVisualStyleBackColor = true;
            // 
            // SaisieGrille
            // 
            AcceptButton = OK;
            AccessibleRole = AccessibleRole.Cursor;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = ESC;
            ClientSize = new Size(158, 100);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(OK);
            Controls.Add(ESC);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaisieGrille";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox CoordX;
        private Label L_CoordX;
        private TextBox CoordY;
        private Label L_CoordY;
        private Panel Support;
        private Button ESC;
        private Button OK;
    }
}