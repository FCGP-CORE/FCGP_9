namespace FCGP
{
    internal partial class AfficheGraphiqueTrace : Form
    {

        internal AfficheGraphiqueTrace()
        {
            InitializeComponent();
            InitialiserEvenements();
        }
        private void InitialiserEvenements()
        {
            // plot
            Plot1.DoubleClicked += Plot1_DoubleClicked;
            Plot1.AxesChanged += Plot1_AxesChanged;
            Plot1.MouseMove += Plot1_MouseMove;
            Plot1.MouseEnter += Plot1_MouseEnter;
            Plot1.MouseLeave += Plot1_MouseLeave;
            // formulaire
            Load += AfficheGraphiqueTrace_Load;
            FormClosed += AfficheGraphiqueTrace_FormClosed;
            KeyDown += AfficheGraphiqueTrace_KeyDown;
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
        private void InitializeComponent()
        {
            components = new Container();
            Plot1 = new ControlScottPlot();
            SuspendLayout();
            // 
            // Plot1
            // 
            Plot1.Location = new Point(0, 0);
            Plot1.Dock = DockStyle.Fill;
            Plot1.Margin = new Padding(0, 0, 0, 0);
            Plot1.Name = "Plot1";
            Plot1.Size = new Size(685, 373);
            Plot1.TabIndex = 0;
            // 
            // formulaire
            // 
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            AutoScaleMode = AutoScaleMode.None;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Controls.Add(Plot1);
            Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Name = "AfficheGraphique";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            ResumeLayout(false);
        }

        private ControlScottPlot Plot1;
    }
}