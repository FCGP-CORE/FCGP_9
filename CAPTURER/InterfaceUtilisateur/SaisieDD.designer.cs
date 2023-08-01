namespace FCGP
{
    internal partial class SaisieDD : Form
    {
        internal SaisieDD()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            CoordLon.KeyDown += CoordLatLon_KeyDown;
            CoordLon.KeyPress += CoordLatLon_KeyPress;
            CoordLon.Leave += CoordLatLon_Leave;
            CoordLat.KeyDown += CoordLatLon_KeyDown;
            CoordLat.KeyPress += CoordLatLon_KeyPress;
            CoordLat.Leave += CoordLatLon_Leave;
            // formulaire
            Load += SaisieDD_Load;
            FormClosing += SaisieDD_FormClosing;
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
            CoordLon = new TextBox();
            L_Longitude = new Label();
            CoordLat = new TextBox();
            L_Latitude = new Label();
            ESC = new Button();
            OK = new Button();
            Support = new Panel();
            SuspendLayout();
            // 
            // L_Longitude
            // 
            L_Longitude.BorderStyle = BorderStyle.FixedSingle;
            L_Longitude.Location = new Point(1, 1);
            L_Longitude.Margin = new Padding(0);
            L_Longitude.Name = "L_Longitude";
            L_Longitude.Size = new Size(85, 26);
            L_Longitude.TabIndex = 5;
            L_Longitude.Text = "Longitude : ";
            L_Longitude.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // CoordX
            // 
            CoordLon.BackColor = Color.White;
            CoordLon.BorderStyle = BorderStyle.FixedSingle;
            CoordLon.Location = new Point(85, 1);
            CoordLon.Margin = new Padding(0);
            CoordLon.MaxLength = 10;
            CoordLon.Name = "CoordX";
            CoordLon.Size = new Size(108, 26);
            CoordLon.TabIndex = 0;
            // 
            // L_Latitude
            // 
            L_Latitude.BorderStyle = BorderStyle.FixedSingle;
            L_Latitude.Location = new Point(1, 26);
            L_Latitude.Margin = new Padding(0);
            L_Latitude.Name = "L_Latitude";
            L_Latitude.Size = new Size(85, 26);
            L_Latitude.TabIndex = 5;
            L_Latitude.Text = "Latitude : ";
            L_Latitude.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // CoordY
            // 
            CoordLat.BackColor = Color.White;
            CoordLat.BorderStyle = BorderStyle.FixedSingle;
            CoordLat.Location = new Point(85, 26);
            CoordLat.Margin = new Padding(0);
            CoordLat.MaxLength = 9;
            CoordLat.Name = "CoordY";
            CoordLat.Size = new Size(108, 26);
            CoordLat.TabIndex = 1;
            // 
            // Support
            // 
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(194, 58);
            Support.TabIndex = 62;
            Support.Controls.Add(CoordLat);
            Support.Controls.Add(L_Latitude);
            Support.Controls.Add(CoordLon);
            Support.Controls.Add(L_Longitude);
            // 
            // OK
            // 
            OK.DialogResult = DialogResult.OK;
            OK.Location = new Point(118, 64);
            OK.Name = "OK";
            OK.Size = new Size(70, 30);
            OK.TabIndex = 2;
            OK.TabStop = false;
            OK.Text = "Valider";
            OK.UseVisualStyleBackColor = true;
            // 
            // ESC
            // 
            ESC.DialogResult = DialogResult.Cancel;
            ESC.Location = new Point(42, 64);
            ESC.Name = "ESC";
            ESC.Size = new Size(70, 30);
            ESC.TabIndex = 3;
            ESC.TabStop = false;
            ESC.Text = "Annuler";
            ESC.UseVisualStyleBackColor = true;
            // 
            // SaisieDD
            // 
            AcceptButton = OK;
            AccessibleRole = AccessibleRole.Cursor;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = ESC;
            ClientSize = new Size(194, 100);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(OK);
            Controls.Add(ESC);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaisieDD";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox CoordLon;
        private Label L_Longitude;
        private TextBox CoordLat;
        private Label L_Latitude;
        private Button ESC;
        private Button OK;
        private Panel Support;
    }
}