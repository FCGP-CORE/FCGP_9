namespace FCGP
{
    internal partial class SaisieUTM : Form
    {

        internal SaisieUTM()
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
            // ZONE
            LabelZone.Click += LabelZONE_Click;
            Zone.SelectedIndexChanged += ZONE_SelectedIndexChanged;
            // Hem
            LabelHem.Click += LabelHem_Click;
            Hem.SelectedIndexChanged += HEM_SelectedIndexChanged;
            // formulaire
            Load += SaisieUTM_Load;
            FormClosing += SaisieUTM_FormClosing;
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
            SuspendLayout();
            // 
            // L_Hem
            // 
            L_Hem = new Label()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(1, 1),
                Margin = new Padding(0),
                Name = "L_Hem",
                Size = new Size(50, 26),
                TabIndex = 10,
                Text = "Hem. : ",
                TextAlign = ContentAlignment.MiddleLeft
            };
            // 
            // LabelHem
            // 
            LabelHem = new Label()
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(50, 1),
                Margin = new Padding(0),
                Name = "LabelHem",
                Size = new Size(30, 26),
                TabIndex = 12,
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 
            // Hem
            // 
            Hem = new ComboBox()
            {
                BackColor = Color.White,
                DropDownWidth = 30,
                FormattingEnabled = true,
                Location = new Point(50, -1),
                Name = "Hem",
                Size = new Size(30, 0),
                TabIndex = 0,
                Visible = false
            };
            Hem.Items.AddRange(new object[] { "N", "S" });
            // 
            // L_X
            // 
            L_X = new Label()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(79, 1),
                Margin = new Padding(0),
                Name = "L_X",
                Size = new Size(26, 26),
                TabIndex = 5,
                Text = "X : ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 
            // CoordX
            // 
            CoordX = new TextBox()
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(104, 1),
                Margin = new Padding(0),
                MaxLength = 9,
                Name = "CoordX",
                Size = new Size(105, 26),
                TabIndex = 2
            };
            // 
            // L_Zone
            // 
            L_Zone = new Label()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(1, 26),
                Margin = new Padding(0),
                Name = "L_Zone",
                Size = new Size(50, 26),
                TabIndex = 9,
                Text = "Zone : ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 
            // LabelZone
            // 
            LabelZone = new Label()
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(50, 26),
                Margin = new Padding(0),
                Name = "LabelZone",
                Size = new Size(30, 26),
                TabIndex = 11,
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 
            // Zone
            // 
            Zone = new ComboBox()
            {
                BackColor = Color.White,
                DropDownWidth = 30,
                FlatStyle = FlatStyle.System,
                FormattingEnabled = true,
                Location = new Point(50, 24),
                Name = "Zone",
                Size = new Size(30, 20),
                TabIndex = 1,
                Visible = false
            };
            Zone.Items.AddRange(new object[] { "20", "21", "22", "30", "31", "32", "38", "40" });
            // 
            // L_Y
            // 
            L_Y = new Label()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(79, 26),
                Margin = new Padding(0),
                Name = "L_Y",
                Size = new Size(26, 26),
                TabIndex = 5,
                Text = "Y : ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 
            // CoordY
            // 
            CoordY = new TextBox()
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(104, 26),
                Margin = new Padding(0),
                MaxLength = 10,
                Name = "CoordY",
                Size = new Size(105, 26),
                TabIndex = 3
            };
            // 
            // Support
            // 
            Support = new Panel()
            {
                BackColor = Color.FromArgb(247, 247, 247),
                Location = new Point(0, 0),
                Name = "Support",
                Size = new Size(210, 58),
                TabIndex = 62
            };
            Support.Controls.Add(L_Hem);
            Support.Controls.Add(LabelHem);
            Support.Controls.Add(Hem);
            Support.Controls.Add(L_X);
            Support.Controls.Add(CoordX);
            Support.Controls.Add(L_Zone);
            Support.Controls.Add(LabelZone);
            Support.Controls.Add(Zone);
            Support.Controls.Add(L_Y);
            Support.Controls.Add(CoordY);
            // 
            // OK
            // 
            OK = new Button()
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK,
                Location = new Point(134, 64),
                Name = "OK",
                Size = new Size(70, 30),
                TabIndex = 5,
                TabStop = false,
                Text = "Valider",
                UseVisualStyleBackColor = true
            };
            // 
            // ESC
            // 
            ESC = new Button()
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel,
                Location = new Point(58, 64),
                Name = "ESC",
                Size = new Size(70, 30),
                TabIndex = 4,
                TabStop = false,
                Text = "Annuler",
                UseVisualStyleBackColor = true
            };
            // 
            // SaisieUTM
            // 
            AcceptButton = OK;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = ESC;
            ClientSize = new Size(210, 100);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(OK);
            Controls.Add(ESC);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaisieUTM";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private Label L_Hem;
        private Label LabelHem;
        private ComboBox Hem;
        private TextBox CoordX;
        private Label L_X;
        private Label L_Zone;
        private Label LabelZone;
        private ComboBox Zone;
        private TextBox CoordY;
        private Label L_Y;
        private Panel Support;
        private Button ESC;
        private Button OK;
    }
}