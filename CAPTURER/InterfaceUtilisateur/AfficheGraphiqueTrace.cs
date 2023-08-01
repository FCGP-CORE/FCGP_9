using ScottPlot;
using ScottPlot.Plottable;

using static FCGP.Commun;

namespace FCGP
{
    internal partial class AfficheGraphiqueTrace
    {
        private Crosshair CrossHair;
        private Plot Plt;
        private ScatterPlot PltPoint;
        private MarkerPlot PltPointLight;
        private Tooltip PltTooltip;
        private Polygon PltPoly;
        private double EpaisseurLinePltPoly;
        private bool FlagRatio, FlagIndex;
        private (double x, double y) Coordinates;
        private (double x, double y, int Index) Point;
        private double[] Dists, Vits, Alts, Durees;
        private double DeltaRecherche, UnitParSegments;
        private TRK _Trace;
        private bool _Is_Vitesse;
        private List<(int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse)> Segments;

        private Func<double, string> CustomFormatter()
        {
            return Position => _Is_Vitesse ? Position.ToString("0.00") : Position.ToString("0");
        }

        private Rectangle StockClipCurseur;
        internal TRK Trace
        {
            set
            {
                _Trace = value;
            }
        }
        /// <summary> true : graphique Vitesse / Distance 
        /// false : graphique Altitude / Distance </summary>
        internal bool Is_Vitesse
        {
            set
            {
                _Is_Vitesse = value;
            }
        }
        internal int IndexPoint
        {
            get
            {
                return Point.Index;
            }
        }

        private void AfficheGraphiqueTrace_Load(object sender, EventArgs e)
        {
            StockClipCurseur = System.Windows.Forms.Cursor.Clip;
            // centre le formulaire sur le formulaire parent
            ClientSize = new Size((int)Math.Round(VisueRectangle.Width * 0.8d), (int)Math.Round(VisueRectangle.Height * 0.8d));
            {
                var WB = Owner;
                Location = new Point(WB.Location.X + (WB.Width - Width) / 2, WB.Location.Y + (WB.Height - Height) / 2);
            }
            Text = $"Trace {_Trace.Nom}";

            InitialaliserSeries();

            Plt = Plot1.Plot;
            Plt.Style(figureBackground: Color.FromArgb(247, 247, 247), dataBackground: Color.FromArgb(240, 240, 240));
            Plt.Grid(enable: false);
            Plt.Title(_Is_Vitesse ? "Vitesse en fonction de la distance" : "Altitude en fonction de la distance");
            Plt.XLabel("Distance en Km");
            Plt.YLabel(_Is_Vitesse ? "Vitesse en Km/h" : "Altitude en Mètres");

            PltPoint = Plt.AddScatterPoints(Dists, _Is_Vitesse ? Vits : Alts);
            PltPoint.Color = Color.FromArgb(247, 247, 247);
            PltPoint.MarkerColor = Plt.GetNextColor(1d);
            PltPoint.MarkerLineWidth = 0f;
            PltPoint.MarkerSize = 0f;
            // Add a red circle we can move around later as a highlighted point indicator
            PltPointLight = Plt.AddPoint(0d, 0d);
            PltPointLight.Color = Plt.GetNextColor(1d);
            PltPointLight.MarkerSize = 10f;
            PltPointLight.MarkerShape = MarkerShape.openCircle;
            PltPointLight.IsVisible = false;

            CrossHair = Plt.AddCrosshair(0d, 0d);
            CrossHair.HorizontalLine.PositionFormatter = CustomFormatter();

            var Xsr = Tools.Pad(Dists, cloneEdges: true);
            var Ysr = Tools.Pad(_Is_Vitesse ? Vits : Alts);

            EpaisseurLinePltPoly = 1d;
            PltPoly = Plt.AddPolygon(Xsr, Ysr, Color.FromArgb(40, 225, 175, 125), lineWidth: EpaisseurLinePltPoly);

            PltTooltip = Plt.AddTooltip("", 0d, 0d);
            PltTooltip.IsVisible = false;
            PltTooltip.ArrowSize = 35;

            Plot1.Refresh();

            Point.Index = -1;
            // et limite les déplacements de la souris au formulaire
            System.Windows.Forms.Cursor.Clip = new Rectangle(Location, Size);
        }

        private void AfficheGraphiqueTrace_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Cursor.Clip = StockClipCurseur;
        }
        private void AfficheGraphiqueTrace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                Point.Index = -1;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        /// <summary> fait ressortir les données associées aux points et aux segments d'une trace </summary>
        private void InitialaliserSeries()
        {
            int DureeTot = default, LongueurTot, Cpt = default;
            LongueurTot = 0;
            int NbSegments = _Trace.NbSegments;
            Segments = _Trace.CalculerStatSegmentsTrace(1, NbSegments);
            Dists = new double[NbSegments + 1];
            Vits = new double[NbSegments + 1];
            Alts = new double[NbSegments + 1];
            Durees = new double[NbSegments + 1];
            Dists[0] = 0d;
            Vits[0] = 0d;
            Durees[0] = 0d;
            Alts[0] = (double)_Trace.Altitude(0);
            foreach (var (Longueur, Denivele, Duree, Cap, Pente, Vitesse) in Segments)
            {
                Cpt += 1;
                LongueurTot += Longueur;
                DureeTot += Duree;
                Dists[Cpt] = LongueurTot / 1000d;
                Vits[Cpt] = Vitesse * 3.6d;
                Alts[Cpt] = (double)_Trace.Altitude(Cpt);
                Durees[Cpt] = DureeTot;
            }
            UnitParSegments = Dists[Cpt] / NbSegments;
        }
        private void DessinnerToolTip()
        {
            TrouverFlagIndex(DeltaRecherche);
            if (FlagIndex)
            {
                PltPointLight.X = Point.x;
                PltPointLight.Y = Point.y;
                PltPointLight.IsVisible = true;
                PltTooltip.X = Point.x;
                PltTooltip.Y = Point.y;
                int Index = Point.Index;
                string PointStr = $"Distance: {Dists[Index]:0.00 km}, Altitude: {Alts[Index]:0} m";
                string PointStrHoraire = _Trace.IsHoraire ? $", Temps: {new TimeSpan(0, 0, (int)Math.Round(Durees[Index])):c}" : "";
                PltTooltip.Label = PointStr + PointStrHoraire;
                if (Index > 0)
                {
                    int NumSeg = Index - 1;
                    string SegmentStr = $"Longueur: {Segments[NumSeg].Longueur} m, Denivelé: {Segments[NumSeg].Denivele} m, " + CrLf +
                                        $"Pente: {Segments[NumSeg].Pente:P}, Cap: {Segments[NumSeg].Cap:N2}°";
                    string SegmentStrHoraire = _Trace.IsHoraire ? CrLf + $"Durée: {Segments[NumSeg].Duree} s, Vitesse : {Vits[Index]:0.00 km/h}" : "";
                    PltTooltip.Label += CrLf + SegmentStr + SegmentStrHoraire;
                }
                PltTooltip.IsVisible = true;
            }
            else
            {
                CrossHair.X = Coordinates.x;
                CrossHair.Y = Coordinates.y;
                PltPointLight.IsVisible = false;
                PltTooltip.IsVisible = false;
            }
        }

        private void TrouverFlagIndex(double Espsilon = 0.25d)
        {
            Coordinates = Plot1.GetMouseCoordinates();
            FlagIndex = false;
            if (FlagRatio)
            {
                Point = PltPoint.GetPointNearest(Coordinates.x, Coordinates.y, XyRatio);
                FlagIndex = Math.Abs(Point.x - Coordinates.x) < Espsilon && Math.Abs(Point.y - Coordinates.y) < Espsilon;
            }
            if (!FlagIndex)
                Point.Index = -1;
        }

        private double XyRatio
        {
            get
            {
                return Plt.XAxis.Dims.PxPerUnit / Plt.YAxis.Dims.PxPerUnit;
            }
        }
        private void Plot1_DoubleClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PltPointLight.IsVisible = false;
                PltTooltip.IsVisible = false;
                if (Point.Index > -1)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
        private void Plot1_AxesChanged(object sender, EventArgs e)
        {
            double RatioDistance = UnitParSegments * Plt.XAxis.Dims.PxPerUnit;
            DeltaRecherche = 0.1d * RatioDistance;
            FlagRatio = RatioDistance > 3.2d;
            if (FlagRatio)
            {
                CrossHair.IsVisible = false;
                PltPoint.LineWidth = 1d;
                PltPoint.MarkerSize = 8f;
                PltPoly.LineWidth = 0d;
            }
            else
            {
                CrossHair.IsVisible = true;
                PltPoint.MarkerLineWidth = 0f;
                PltPoint.MarkerSize = 0f;
                PltPoly.LineWidth = EpaisseurLinePltPoly;
            }
            DessinnerToolTip();
            Plot1.Refresh(lowQuality: false, skipIfCurrentlyRendering: true);
        }
        private void Plot1_MouseEnter(object sender, EventArgs e)
        {
            CrossHair.IsVisible = !FlagRatio;
        }
        private void Plot1_MouseMove(object sender, MouseEventArgs e)
        {
            DessinnerToolTip();
            Plot1.Refresh(lowQuality: false, skipIfCurrentlyRendering: true);
        }
        private void Plot1_MouseLeave(object sender, EventArgs e)
        {
            CrossHair.IsVisible = false;
            Plot1.Refresh();
        }

        #region ScottPlot WinForms
        /// <summary> ScottPlot Interactive Plotting Library for .NET : https://scottplot.net/
        /// Code traduit de C# à VB et modifié du controle WindowsForms de la librairie ScottPlot 
        /// https://github.com/ScottPlot/ScottPlot/tree/main/src/ScottPlot4/ScottPlot.WinForms  
        /// pour correspondre aux besoins spécifiques de FCGP Capturer. 
        /// Les modifications portent principalement sur le LeftdoubleClick et sur le RightClick menu par défaut</summary>
        private partial class ControlScottPlot : UserControl
        {
            /// <summary> This is the plot displayed by the user control.
            /// After modifying it you may need to call Refresh() to request the plot be redrawn on the screen. </summary>
            internal Plot Plot
            {
                get
                {
                    return Backend.Plot;
                }
            }

            /// <summary> This object can be used to modify advanced behaior and customization of this user control. </summary>
            internal readonly ScottPlot.Control.Configuration Configuration;

            /// <summary> This event is invoked any time the axis limits are modified. </summary>
            internal event EventHandler AxesChanged;
            /// <summary> This event is invoked any time the users doubleclick with the mouse on the plot. </summary>
            internal event MouseEventHandler DoubleClicked;
            /// <summary> This event is invoked any time the plot is right-clicked.
            /// By default it contains DefaultRightClickEvent(), but you can remove this and add your own method. </summary>
            internal event EventHandler RightClicked;

            /// <summary> This event is invoked after the mouse moves while dragging a draggable plottable.
            /// The object passed is the plottable being dragged. </summary>
            internal event EventHandler PlottableDragged;

            /// <summary> This event is invoked right after a draggable plottable was dropped.
            /// The object passed is the plottable that was just dropped. </summary>
            internal event EventHandler PlottableDropped;
            private readonly ScottPlot.Control.ControlBackEnd Backend;
            private readonly Dictionary<ScottPlot.Cursor, System.Windows.Forms.Cursor> Cursors;
            private readonly bool IsDesignerMode = Equals(Process.GetCurrentProcess().ProcessName, "devenv");

            internal ControlScottPlot()
            {
                Backend = new ScottPlot.Control.ControlBackEnd(1f, 1f, "FormsPlot");
                Backend.Resize(Width, Height, useDelayedRendering: false);
                Backend.BitmapChanged += new EventHandler(OnBitmapChanged);
                Backend.BitmapUpdated += new EventHandler(OnBitmapUpdated);
                Backend.CursorChanged += new EventHandler(OnCursorChanged);
                Backend.RightClicked += new EventHandler(OnRightClicked);
                Backend.AxesChanged += new EventHandler(OnAxesChanged);
                Backend.PlottableDragged += new EventHandler(OnPlottableDragged);
                Backend.PlottableDropped += new EventHandler(OnPlottableDropped);
                Configuration = Backend.Configuration;

                if (IsDesignerMode)
                {
                    try
                    {
                        Configuration.WarnIfRenderNotCalledManually = false;
                        Plot.Title($"ScottPlot {Plot.Version}");
                        Plot.Render();
                    }
                    catch (Exception e)
                    {
                        InitializeComponent();
                        pictureBox1.Visible = false;
                        rtbErrorMessage.Visible = true;
                        rtbErrorMessage.Dock = DockStyle.Fill;
                        rtbErrorMessage.Text = "ERROR: ScottPlot failed to render in design mode." + CrLf +
                                               "This may be due to incompatible Common versions or a 32-bit/64-bit mismatch." + CrLf +
                                               "Although rendering failed at design time, it may still function normally at runtime." + CrLf +
                                               $"Exception details:{e}";
                        return;
                    }
                }

                Cursors = new Dictionary<ScottPlot.Cursor, System.Windows.Forms.Cursor>(){  { ScottPlot.Cursor.Arrow, System.Windows.Forms.Cursors.Arrow },
                                                                                            { ScottPlot.Cursor.WE, System.Windows.Forms.Cursors.SizeWE },
                                                                                            { ScottPlot.Cursor.NS, System.Windows.Forms.Cursors.SizeNS },
                                                                                            { ScottPlot.Cursor.All, System.Windows.Forms.Cursors.SizeAll },
                                                                                            { ScottPlot.Cursor.Crosshair, System.Windows.Forms.Cursors.Cross },
                                                                                            { ScottPlot.Cursor.Hand, System.Windows.Forms.Cursors.Hand },
                                                                                            { ScottPlot.Cursor.Question, System.Windows.Forms.Cursors.Help } };
                InitializeComponent();
                rtbErrorMessage.Visible = false;
                pictureBox1.BackColor = Color.Transparent;
                Plot.Style(figureBackground: Color.Transparent);
                pictureBox1.MouseWheel += PictureBox1_MouseWheel;
                RightClicked += DefaultRightClickEvent;
                Backend.StartProcessingEvents();
            }

            /// <summary> Return the mouse position on the plot (in coordinate space) for the latest X and Y coordinates </summary>
            internal (double x, double y) GetMouseCoordinates(int xAxisIndex = 0, int yAxisIndex = 0)
            {
                return Backend.GetMouseCoordinates(xAxisIndex, yAxisIndex);
            }

            /// <summary> Reset this control by replacing the current plot with an existing plot </summary>
            internal void Reset(Plot newPlot)
            {
                Backend.Reset(Width, Height, newPlot);
            }

            /// <summary> Re-render the plot and update the image displayed by this control. </summary>
            /// <param name="lowQuality">disable anti-aliasing to produce faster (but lower quality) plots </param>
            /// <param name="skipIfCurrentlyRendering"></param>
            internal void Refresh(bool lowQuality = false, bool skipIfCurrentlyRendering = false)
            {
                Application.DoEvents();
                Backend.WasManuallyRendered = true;
                Backend.Render(lowQuality, skipIfCurrentlyRendering);
            }

            private void FormsPlot_Load(object sender, EventArgs e)
            {
                OnSizeChanged(null, null);
            }

            private void OnBitmapUpdated(object sender, EventArgs e)
            {
                Application.DoEvents();
                pictureBox1.Invalidate();
            }

            private void OnBitmapChanged(object sender, EventArgs e)
            {
                pictureBox1.Image = Backend.GetLatestBitmap();
            }

            private void OnCursorChanged(object sender, EventArgs e)
            {
                Cursor = Cursors[Backend.Cursor];
            }

            private void OnSizeChanged(object sender, EventArgs e)
            {
                Backend.Resize(Width, Height, useDelayedRendering: true);
            }

            private void OnAxesChanged(object sender, EventArgs e)
            {
                AxesChanged?.Invoke(this, e);
            }

            private void OnRightClicked(object sender, EventArgs e)
            {
                RightClicked?.Invoke(this, e);
            }

            private void OnPlottableDragged(object sender, EventArgs e)
            {
                PlottableDragged?.Invoke(sender, e);
            }

            private void OnPlottableDropped(object sender, EventArgs e)
            {
                PlottableDropped?.Invoke(sender, e);
            }

            private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
            {
                Backend.MouseDown(GetInputState(e));
                OnMouseDown(e);
            }

            private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
            {
                Backend.MouseUp(GetInputState(e));
                base.OnMouseUp(e);
            }

            private void OnDoubleClicked(object sender, EventArgs e)
            {
                DoubleClicked?.Invoke(this, (MouseEventArgs)e);
            }

            private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
            {
                Backend.MouseWheel(GetInputState(e));
                OnMouseWheel(e);
            }

            private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
            {
                Backend.MouseMove(GetInputState(e));
                base.OnMouseMove(e);
            }

            private void PictureBox1_MouseEnter(object sender, EventArgs e)
            {
                base.OnMouseEnter(e);
            }

            private void PictureBox1_MouseLeave(object sender, EventArgs e)
            {
                base.OnMouseLeave(e);
            }

            private static ScottPlot.Control.InputState GetInputState(MouseEventArgs e)
            {
                return new ScottPlot.Control.InputState()
                {
                    X = e.X,
                    Y = e.Y,
                    LeftWasJustPressed = e.Button == MouseButtons.Left,
                    RightWasJustPressed = e.Button == MouseButtons.Right,
                    MiddleWasJustPressed = e.Button == MouseButtons.Middle,
                    ShiftDown = ModifierKeys.HasFlag(Keys.Shift),
                    CtrlDown = ModifierKeys.HasFlag(Keys.Control),
                    AltDown = ModifierKeys.HasFlag(Keys.Alt),
                    WheelScrolledUp = e.Delta > 0,
                    WheelScrolledDown = e.Delta < 0
                };
            }

            public void DefaultRightClickEvent(object sender, EventArgs e)
            {
                DefaultRightClickMenu.Show(System.Windows.Forms.Cursor.Position);
            }

            private void RightClickMenu_Copy_Click(object sender, EventArgs e)
            {
                Clipboard.SetImage(Plot.Render());
            }

            private void RightClickMenu_Help_Click(object sender, EventArgs e)
            {
                new ControlScottPlotHelp().Show(this);
            }

            private void RightClickMenu_AutoAxis_Click(object sender, EventArgs e)
            {
                Plot.AxisAuto();
                Refresh();
            }

            private void RightClickMenu_SaveImage_Click(object sender, EventArgs e)
            {
                var sfd = new SaveFileDialog()
                {
                    FileName = "ScottPlot.png",
                    Filter = "PNG Files (*.png)|*.png;*.png" + "|JPG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg" + "|BMP Files (*.bmp)|*.bmp;*.bmp" + "|All files (*.*)|*.*"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                    Plot.SaveFig(sfd.FileName);
            }
            #region FormHelp
            #region Hors concepteur
            private partial class ControlScottPlotHelp : Form
            {

                internal ControlScottPlotHelp()
                {
                    InitializeComponent();
                    var resources = new ComponentResourceManager(typeof(AfficheGraphiqueTrace));
                    lblVersion.Text = $"ScottPlot {Plot.Version}";
                    lblMessage.Text = resources.GetString("lblMessage.Text");
                }
            }
            #endregion
            #region concepteur
            private partial class ControlScottPlotHelp
            {
                /// <summary> Required designer variable. </summary>
                private readonly IContainer components = null;

                /// <summary> Clean up any resources being used. </summary>
                /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
                protected override void Dispose(bool disposing)
                {
                    if (disposing && components is not null)
                    {
                        components.Dispose();
                    }

                    base.Dispose(disposing);
                }
                /// <summary> Required method for Designer support - do not modify
                /// the contents of this method with the code editor. </summary>
                private void InitializeComponent()
                {
                    lblTitle = new Label();
                    lblVersion = new Label();
                    lblMessage = new Label();
                    SuspendLayout();
                    // 
                    // lblTitle
                    // 
                    lblTitle.AutoSize = true;
                    lblTitle.Font = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Point);
                    lblTitle.Location = new Point(14, 10);
                    lblTitle.Margin = new Padding(4, 0, 4, 0);
                    lblTitle.Name = "lblTitle";
                    lblTitle.Size = new Size(265, 32);
                    lblTitle.TabIndex = 0;
                    lblTitle.Text = "Contrôles avec la souris";
                    // 
                    // lblVersion
                    // 
                    lblVersion.AutoSize = true;
                    lblVersion.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point);
                    lblVersion.ForeColor = SystemColors.ControlDark;
                    lblVersion.Location = new Point(16, 47);
                    lblVersion.Margin = new Padding(4, 0, 4, 0);
                    lblVersion.Name = "lblVersion";
                    lblVersion.Size = new Size(0, 17);
                    lblVersion.TabIndex = 1;
                    // 
                    // lblMessage
                    // 
                    lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    lblMessage.AutoSize = true;
                    lblMessage.Font = new Font("Segoe UI", 11.25f, FontStyle.Regular, GraphicsUnit.Point);
                    lblMessage.Location = new Point(14, 83);
                    lblMessage.Margin = new Padding(4, 0, 4, 0);
                    lblMessage.Name = "lblMessage";
                    lblMessage.Size = new Size(293, 340);
                    lblMessage.TabIndex = 2;
                    // 
                    // FormHelp
                    // 
                    AutoScaleDimensions = new SizeF(7.0f, 15.0f);
                    AutoScaleMode = AutoScaleMode.Font;
                    ClientSize = new Size(364, 500);
                    Controls.Add(lblMessage);
                    Controls.Add(lblVersion);
                    Controls.Add(lblTitle);
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                    Margin = new Padding(4, 3, 4, 3);
                    Name = "FormHelp";
                    Text = "Help";
                    ResumeLayout(false);
                    PerformLayout();
                }
                #endregion
                private Label lblTitle;
                private Label lblVersion;
                private Label lblMessage;
            }
            #endregion
        }
        private partial class ControlScottPlot
        {
            /// <summary>  Required designer variable. </summary>
            private IContainer components = null;

            /// <summary> Clean up any resources being used. </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }

                base.Dispose(disposing);
            }

            #region Component Designer generated code
            /// <summary> Required method for Designer support - do not modify 
            /// the contents of this method with the code editor. </summary>
            private void InitializeComponent()
            {
                components = new Container();
                pictureBox1 = new PictureBox();
                DefaultRightClickMenu = new ContextMenuStrip(components);
                copyMenuItem = new ToolStripMenuItem();
                saveImageMenuItem = new ToolStripMenuItem();
                toolStripSeparator1 = new ToolStripSeparator();
                autoAxisMenuItem = new ToolStripMenuItem();
                toolStripSeparator2 = new ToolStripSeparator();
                helpMenuItem = new ToolStripMenuItem();
                rtbErrorMessage = new RichTextBox();
                ((ISupportInitialize)pictureBox1).BeginInit();
                DefaultRightClickMenu.SuspendLayout();
                SuspendLayout();
                // 
                // pictureBox1
                // 
                pictureBox1.BackColor = Color.Navy;
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.Location = new Point(0, 0);
                pictureBox1.Name = "pictureBox1";
                pictureBox1.Size = new Size(400, 300);
                pictureBox1.TabIndex = 0;
                pictureBox1.TabStop = false;
                pictureBox1.DoubleClick += new EventHandler(OnDoubleClicked);
                pictureBox1.MouseDown += new MouseEventHandler(PictureBox1_MouseDown);
                pictureBox1.MouseEnter += new EventHandler(PictureBox1_MouseEnter);
                pictureBox1.MouseLeave += new EventHandler(PictureBox1_MouseLeave);
                pictureBox1.MouseMove += new MouseEventHandler(PictureBox1_MouseMove);
                pictureBox1.MouseUp += new MouseEventHandler(PictureBox1_MouseUp);
                // 
                // DefaultRightClickMenu
                // 
                DefaultRightClickMenu.ImageScalingSize = new Size(20, 20);
                DefaultRightClickMenu.Items.AddRange(new ToolStripItem[] { copyMenuItem, saveImageMenuItem, toolStripSeparator1,
                                                                           autoAxisMenuItem, toolStripSeparator2,
                                                                           helpMenuItem });
                DefaultRightClickMenu.Name = "contextMenuStrip1";
                DefaultRightClickMenu.Size = new Size(191, 132);
                DefaultRightClickMenu.ShowImageMargin = false;
                // 
                // copyMenuItem
                // 
                copyMenuItem.Name = "copyMenuItem";
                copyMenuItem.Size = new Size(190, 22);
                copyMenuItem.Text = "Copie Image";
                copyMenuItem.Click += new EventHandler(RightClickMenu_Copy_Click);
                // 
                // saveImageMenuItem
                // 
                saveImageMenuItem.Name = "saveImageMenuItem";
                saveImageMenuItem.Size = new Size(190, 22);
                saveImageMenuItem.Text = "Sauve Image ...";
                saveImageMenuItem.Click += new EventHandler(RightClickMenu_SaveImage_Click);
                // 
                // toolStripSeparator1
                // 
                toolStripSeparator1.Name = "toolStripSeparator1";
                toolStripSeparator1.Size = new Size(187, 6);
                // 
                // autoAxisMenuItem
                // 
                autoAxisMenuItem.Name = "autoAxisMenuItem";
                autoAxisMenuItem.Size = new Size(190, 22);
                autoAxisMenuItem.Text = "Recentre le tracé";
                autoAxisMenuItem.Click += new EventHandler(RightClickMenu_AutoAxis_Click);
                // 
                // toolStripSeparator2
                // 
                toolStripSeparator2.Name = "toolStripSeparator2";
                toolStripSeparator2.Size = new Size(187, 6);
                // 
                // helpMenuItem
                // 
                helpMenuItem.Name = "helpMenuItem";
                helpMenuItem.Size = new Size(190, 22);
                helpMenuItem.Text = "Aide ...";
                helpMenuItem.Click += new EventHandler(RightClickMenu_Help_Click);
                // 
                // rtbErrorMessage
                // 
                rtbErrorMessage.BackColor = Color.Maroon;
                rtbErrorMessage.Font = new Font("Consolas", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
                rtbErrorMessage.ForeColor = Color.White;
                rtbErrorMessage.Location = new Point(21, 24);
                rtbErrorMessage.Name = "rtbErrorMessage";
                rtbErrorMessage.Size = new Size(186, 84);
                rtbErrorMessage.TabIndex = 1;
                rtbErrorMessage.Text = "error message";
                rtbErrorMessage.Visible = false;
                // 
                // FormsPlot
                // 
                AutoScaleDimensions = new SizeF(6.0f, 13.0f);
                AutoScaleMode = AutoScaleMode.Font;
                Controls.Add(rtbErrorMessage);
                Controls.Add(pictureBox1);
                Name = "FormsPlot";
                Size = new Size(400, 300);
                Load += new EventHandler(FormsPlot_Load);
                SizeChanged += new EventHandler(OnSizeChanged);
                ((ISupportInitialize)pictureBox1).EndInit();
                DefaultRightClickMenu.ResumeLayout(false);
                ResumeLayout(false);
            }
            #endregion

            private PictureBox pictureBox1;
            private ContextMenuStrip DefaultRightClickMenu;
            private ToolStripMenuItem autoAxisMenuItem;
            private ToolStripMenuItem saveImageMenuItem;
            private ToolStripMenuItem helpMenuItem;
            private ToolStripMenuItem copyMenuItem;
            private ToolStripSeparator toolStripSeparator1;
            private ToolStripSeparator toolStripSeparator2;
            private RichTextBox rtbErrorMessage;
        }
        #endregion
    }
}