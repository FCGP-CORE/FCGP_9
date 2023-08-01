namespace FCGP
{
    internal partial class SaisieDMS : Form
    {

        internal SaisieDMS()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // contrôles simples
            DegX.KeyDown += Coord_KeyDown;
            DegX.KeyPress += Coord_KeyPress;
            MinX.KeyDown += Coord_KeyDown;
            MinX.KeyPress += Coord_KeyPress;
            SecX.KeyDown += Coord_KeyDown;
            SecX.KeyPress += Coord_KeyPress;
            SecX.Leave += Coord_Leave;
            DegY.KeyDown += Coord_KeyDown;
            DegY.KeyPress += Coord_KeyPress;
            MinY.KeyDown += Coord_KeyDown;
            MinY.KeyPress += Coord_KeyPress;
            SecY.KeyDown += Coord_KeyDown;
            SecY.KeyPress += Coord_KeyPress;
            SecY.Leave += Coord_Leave;
            // Longitude
            LabelLongitude.Click += LabelLongitude_Click;
            Longitude.SelectedIndexChanged += Longitude_SelectedIndexChanged;
            // Latitude
            LabelLatitude.Click += LabelLatitude_Click;
            Latitude.SelectedIndexChanged += Latitude_SelectedIndexChanged;
            // formulaire
            Load += SaisieDMS_Load;
            FormClosing += SaisieDMS_FormClosing;
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
            LabelLongitude = new Label();
            DegY = new TextBox();
            L_Latitude = new Label();
            ESC = new Button();
            OK = new Button();
            L_Deg = new Label();
            L_Sec = new Label();
            L_Min = new Label();
            Longitude = new ComboBox();
            Latitude = new ComboBox();
            L_Sens = new Label();
            SecY = new TextBox();
            MinY = new TextBox();
            SecX = new TextBox();
            MinX = new TextBox();
            DegX = new TextBox();
            LabelLatitude = new Label();
            L_Longitude = new Label();
            Support = new Panel();
            SuspendLayout();
            // 
            // L_Sens
            // 
            L_Sens.BackColor = Color.Transparent;
            L_Sens.BorderStyle = BorderStyle.FixedSingle;
            L_Sens.Location = new Point(84, 1);
            L_Sens.Margin = new Padding(0);
            L_Sens.Name = "L_Sens";
            L_Sens.Size = new Size(40, 21);
            L_Sens.TabIndex = 23;
            L_Sens.Text = "Sens";
            L_Sens.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Deg
            // 
            L_Deg.BackColor = Color.Transparent;
            L_Deg.BorderStyle = BorderStyle.FixedSingle;
            L_Deg.Location = new Point(123, 1);
            L_Deg.Margin = new Padding(0);
            L_Deg.Name = "L_Deg";
            L_Deg.Size = new Size(36, 21);
            L_Deg.TabIndex = 20;
            L_Deg.Text = "Deg";
            L_Deg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Min
            // 
            L_Min.BackColor = Color.Transparent;
            L_Min.BorderStyle = BorderStyle.FixedSingle;
            L_Min.Location = new Point(158, 1);
            L_Min.Margin = new Padding(0);
            L_Min.Name = "L_Min";
            L_Min.Size = new Size(36, 21);
            L_Min.TabIndex = 20;
            L_Min.Text = "Min";
            L_Min.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Sec
            // 
            L_Sec.BackColor = Color.Transparent;
            L_Sec.BorderStyle = BorderStyle.FixedSingle;
            L_Sec.Location = new Point(193, 1);
            L_Sec.Margin = new Padding(0);
            L_Sec.Name = "L_Sec";
            L_Sec.Size = new Size(51, 21);
            L_Sec.TabIndex = 20;
            L_Sec.Text = "Sec";
            L_Sec.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // L_Longitude
            // 
            L_Longitude.BackColor = Color.Transparent;
            L_Longitude.BorderStyle = BorderStyle.FixedSingle;
            L_Longitude.Location = new Point(1, 21);
            L_Longitude.Margin = new Padding(0);
            L_Longitude.Name = "L_Longitude";
            L_Longitude.Size = new Size(84, 26);
            L_Longitude.TabIndex = 25;
            L_Longitude.Text = "Longitude : ";
            L_Longitude.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelLongitude
            // 
            LabelLongitude.BackColor = Color.White;
            LabelLongitude.BorderStyle = BorderStyle.FixedSingle;
            LabelLongitude.Location = new Point(84, 21);
            LabelLongitude.Margin = new Padding(0);
            LabelLongitude.Name = "LabelLongitude";
            LabelLongitude.Size = new Size(40, 26);
            LabelLongitude.TabIndex = 20;
            LabelLongitude.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Longitude
            // 
            Longitude.BackColor = Color.White;
            Longitude.FormattingEnabled = true;
            Longitude.Items.AddRange(new object[] { "E", "O" });
            Longitude.Location = new Point(84, 19);
            Longitude.Name = "Longitude";
            Longitude.Size = new Size(40, 20);
            Longitude.TabIndex = 0;
            Longitude.Visible = false;
            // 
            // DegX
            // 
            DegX.BackColor = Color.White;
            DegX.BorderStyle = BorderStyle.FixedSingle;
            DegX.Location = new Point(123, 21);
            DegX.Margin = new Padding(0);
            DegX.MaxLength = 3;
            DegX.Name = "DegX";
            DegX.Size = new Size(36, 26);
            DegX.TabIndex = 1;
            // 
            // MinX
            // 
            MinX.BackColor = Color.White;
            MinX.BorderStyle = BorderStyle.FixedSingle;
            MinX.Location = new Point(158, 21);
            MinX.Margin = new Padding(0);
            MinX.MaxLength = 2;
            MinX.Name = "MinX";
            MinX.Size = new Size(36, 26);
            MinX.TabIndex = 2;
            // 
            // SecX
            // 
            SecX.BackColor = Color.White;
            SecX.BorderStyle = BorderStyle.FixedSingle;
            SecX.Location = new Point(193, 21);
            SecX.Margin = new Padding(0);
            SecX.MaxLength = 5;
            SecX.Name = "SecX";
            SecX.Size = new Size(51, 26);
            SecX.TabIndex = 3;
            // 
            // L_Latitude
            // 
            L_Latitude.BackColor = Color.Transparent;
            L_Latitude.BorderStyle = BorderStyle.FixedSingle;
            L_Latitude.Location = new Point(1, 46);
            L_Latitude.Margin = new Padding(0);
            L_Latitude.Name = "L_Latitude";
            L_Latitude.Size = new Size(84, 26);
            L_Latitude.TabIndex = 20;
            L_Latitude.Text = "Latitude : ";
            L_Latitude.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelLatitude
            // 
            LabelLatitude.BackColor = Color.White;
            LabelLatitude.BorderStyle = BorderStyle.FixedSingle;
            LabelLatitude.Location = new Point(84, 46);
            LabelLatitude.Margin = new Padding(0);
            LabelLatitude.Name = "LabelLatitude";
            LabelLatitude.Size = new Size(40, 26);
            LabelLatitude.TabIndex = 24;
            LabelLatitude.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Latitude
            // 
            Latitude.BackColor = Color.White;
            Latitude.FormattingEnabled = true;
            Latitude.Items.AddRange(new object[] { "N", "S" });
            Latitude.Location = new Point(84, 44);
            Latitude.Margin = new Padding(0);
            Latitude.Name = "Latitude";
            Latitude.Size = new Size(40, 20);
            Latitude.TabIndex = 4;
            Latitude.Visible = false;
            // 
            // DegY
            // 
            DegY.BackColor = Color.White;
            DegY.BorderStyle = BorderStyle.FixedSingle;
            DegY.Location = new Point(123, 46);
            DegY.Margin = new Padding(0);
            DegY.MaxLength = 2;
            DegY.Name = "DegY";
            DegY.Size = new Size(36, 26);
            DegY.TabIndex = 5;
            // 
            // MinY
            // 
            MinY.BackColor = Color.White;
            MinY.BorderStyle = BorderStyle.FixedSingle;
            MinY.Location = new Point(158, 46);
            MinY.Margin = new Padding(0);
            MinY.MaxLength = 2;
            MinY.Name = "MinY";
            MinY.Size = new Size(36, 26);
            MinY.TabIndex = 6;
            // 
            // SecY
            // 
            SecY.BackColor = Color.White;
            SecY.BorderStyle = BorderStyle.FixedSingle;
            SecY.Location = new Point(193, 46);
            SecY.Margin = new Padding(0);
            SecY.MaxLength = 5;
            SecY.Name = "SecY";
            SecY.Size = new Size(51, 26);
            SecY.TabIndex = 7;
            // 
            // Support
            // 
            Support.BackColor = Color.FromArgb(247, 247, 247);
            Support.Location = new Point(0, 0);
            Support.Name = "Support";
            Support.Size = new Size(245, 78);
            Support.TabIndex = 62;
            Support.Controls.Add(L_Longitude);
            Support.Controls.Add(LabelLatitude);
            Support.Controls.Add(DegY);
            Support.Controls.Add(DegX);
            Support.Controls.Add(L_Latitude);
            Support.Controls.Add(LabelLongitude);
            Support.Controls.Add(L_Sens);
            Support.Controls.Add(SecY);
            Support.Controls.Add(MinY);
            Support.Controls.Add(SecX);
            Support.Controls.Add(MinX);
            Support.Controls.Add(L_Min);
            Support.Controls.Add(L_Sec);
            Support.Controls.Add(L_Deg);
            Support.Controls.Add(Latitude);
            Support.Controls.Add(Longitude);
            // 
            // ESC
            // 
            ESC.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ESC.DialogResult = DialogResult.Cancel;
            ESC.Location = new Point(93, 84);
            ESC.Name = "ESC";
            ESC.Size = new Size(70, 30);
            ESC.TabIndex = 8;
            ESC.TabStop = false;
            ESC.Text = "Annuler";
            ESC.UseVisualStyleBackColor = true;
            // 
            // OK
            // 
            OK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OK.DialogResult = DialogResult.OK;
            OK.Location = new Point(169, 84);
            OK.Name = "OK";
            OK.Size = new Size(70, 30);
            OK.TabIndex = 9;
            OK.TabStop = false;
            OK.Text = "Valider";
            OK.UseVisualStyleBackColor = true;
            // 
            // SaisieDMS
            // 
            AcceptButton = OK;
            AccessibleRole = AccessibleRole.Cursor;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = ESC;
            ClientSize = new Size(245, 120);
            ControlBox = false;
            Controls.Add(Support);
            Controls.Add(OK);
            Controls.Add(ESC);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaisieDMS";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private Label LabelLongitude;
        private TextBox DegY;
        private Label L_Latitude;
        private Button ESC;
        private Button OK;
        private Label L_Deg;
        private Label L_Sec;
        private Label L_Min;
        private ComboBox Longitude;
        private ComboBox Latitude;
        private Label L_Sens;
        private TextBox SecY;
        private TextBox MinY;
        private TextBox SecX;
        private TextBox MinX;
        private TextBox DegX;
        private Label LabelLatitude;
        private Label L_Longitude;
        private Panel Support;
    }
}