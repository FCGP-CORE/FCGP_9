using static FCGP.AfficheInformation;
using static FCGP.Commun;

namespace FCGP.TRKs
{
    internal partial class TRK
    {
        #region Formulaire Fermer Trace en cours
        private partial class FormFermerTrace : Form
        {
            private List<TRK> _CollectionTraces;
            private Rectangle StockClipCurseur;
            internal List<TRK> CollectionTraces
            {
                set
                {
                    _CollectionTraces = value;
                }
            }
            private int _IndexTrace;
            private string Titre;
            internal int IndexTrace
            {
                set
                {
                    _IndexTrace = value;
                }
            }
            private void FermerTrace_Load(object sender, EventArgs e)
            {
                Titre = TitreInformation;
                TitreInformation = "Information Trace";
                StockClipCurseur = Cursor.Clip;
                // met le curseur sur le centre du bouton OK
                Cursor.Position = new Point(Location.X + Ok.Location.X + Ok.Width / 2, Location.Y + Ok.Location.Y + Ok.Height / 2);
                // et limite les déplacements de la souris au formulaire
                Cursor.Clip = new Rectangle(Location, Size);
            }
            private void FermerTrace_FormClosed(object sender, FormClosedEventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                TitreInformation = Titre;
            }

            /// <summary> permet l'enregistrement de la trace sous un autre fichier .trk </summary>
            private void ButtonEnregistreSous_Click(object sender, EventArgs e)
            {
                if (_CollectionTraces[_IndexTrace].NbSegments > 0)
                {
                    Cursor.Clip = StockClipCurseur;
                    EnregistrerTrace.Filter = "TRK files (*.trk)|*.trk";
                    EnregistrerTrace.DefaultExt = ".trk";
                    EnregistrerTrace.Title = "Sauvegarder la trace sous";
                    EnregistrerTrace.FileName = _CollectionTraces[_IndexTrace].Nom;
                    // c'est la boite de dialogue qui gère si le nom existe déjà dans la collection
                    if (EnregistrerTrace.ShowDialog() == DialogResult.OK)
                    {
                        // on enregistre la trace sous son nouveau nom
                        TRK.EnregistrerTrace(_IndexTrace, EnregistrerTrace.FileName, TypesFichier.TRK);
                    }
                    Cursor.Clip = new Rectangle(Location, Size);
                }
                else
                {
                    MessageInformation = "La trace doit avoir au moins un segment" + CrLf +
                                         "pour pouvoir être sauvegardée";
                    AfficherInformation();
                }
            }
            /// <summary> permet l'enregistrement de la trace sous un fichier .gpx </summary>
            private void BoutonExport_Click(object sender, EventArgs e)
            {
                if (_CollectionTraces[_IndexTrace].NbSegments > 0)
                {
                    Cursor.Clip = StockClipCurseur;
                    EnregistrerTrace.Filter = "GPX files (*.gpx)|*.gpx";
                    EnregistrerTrace.DefaultExt = ".gpx";
                    EnregistrerTrace.Title = "Exporter la trace";
                    EnregistrerTrace.FilterIndex = 1; // GPX
                    EnregistrerTrace.FileName = _CollectionTraces[_IndexTrace].Nom;
                    // c'est la boite de dialogue qui gère si le nom existe déjà dans la collection
                    if (EnregistrerTrace.ShowDialog() == DialogResult.OK)
                    {
                        TRK.EnregistrerTrace(_IndexTrace, EnregistrerTrace.FileName, TypesFichier.GPX);
                    }
                    Cursor.Clip = new Rectangle(Location, Size);
                }
                else
                {
                    MessageInformation = "La trace doit avoir au moins un segment" + CrLf +
                                         "pour pouvoir être exportée";
                    AfficherInformation();
                }
            }
            /// <summary> permet de supprimer la trace de la collection et le fichier .trk correspondant </summary>
            private void BoutonSupprimer_Click(object sender, EventArgs e)
            {
                MessageInformation = "Voulez vous effectivement supprimer la trace encours ?";
                if (AfficherConfirmation() == DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
                Close();
            }
        }
        private partial class FormFermerTrace : Form
        {
            internal FormFermerTrace()
            {
                InitializeComponent();
                InitialiserEvenements();
            }
            private void InitialiserEvenements()
            {
                BoutonExporter.Click += BoutonExport_Click;
                ButtonEnregistreSous.Click += ButtonEnregistreSous_Click;
                BoutonSupprimer.Click += BoutonSupprimer_Click;
                Load += FermerTrace_Load;
                FormClosed += FermerTrace_FormClosed;
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
                ButtonEnregistreSous = new Button();
                CadreBoutonEnregistrerSous = new Label();
                BoutonExporter = new Button();
                CadreBoutonExporter = new Label();
                BoutonSupprimer = new Button();
                CadreBoutonSupprimer = new Label();
                PasOk = new Button();
                Ok = new Button();
                components = new Container();
                EnregistrerTrace = new SaveFileDialog();
                ToolTip1 = new ToolTip(components);
                PanelActions = new Panel();
                PanelActions.SuspendLayout();
                SuspendLayout();
                // 
                // CadreBoutonEnregistrerSous
                // 
                CadreBoutonEnregistrerSous.BorderStyle = BorderStyle.FixedSingle;
                CadreBoutonEnregistrerSous.FlatStyle = FlatStyle.Flat;
                CadreBoutonEnregistrerSous.Location = new Point(6, 6);
                CadreBoutonEnregistrerSous.Name = "CadreBoutonEnregistrerSous";
                CadreBoutonEnregistrerSous.Size = new Size(201, 32);
                CadreBoutonEnregistrerSous.TabIndex = 19;
                CadreBoutonEnregistrerSous.Text = "Label1";
                // 
                // ButtonEnregistreSous
                // 
                ButtonEnregistreSous.BackColor = Color.Transparent;
                ButtonEnregistreSous.Location = new Point(7, 7);
                ButtonEnregistreSous.Name = "ButtonEnregistreSous";
                ButtonEnregistreSous.Size = new Size(199, 30);
                ButtonEnregistreSous.TabIndex = 1;
                ButtonEnregistreSous.TabStop = false;
                ButtonEnregistreSous.Tag = "";
                ButtonEnregistreSous.Text = "Sauvegarder sous";
                ButtonEnregistreSous.UseVisualStyleBackColor = false;
                ToolTip1.SetToolTip(ButtonEnregistreSous, "Sauvegarde la trace avec un nom différent." + CrLf +
                                                          "La trace encours reste la trace de l'affichage.");
                // 
                // CadreBoutonExporter
                // 
                CadreBoutonExporter.BackColor = Color.Transparent;
                CadreBoutonExporter.BorderStyle = BorderStyle.FixedSingle;
                CadreBoutonExporter.FlatStyle = FlatStyle.Flat;
                CadreBoutonExporter.Location = new Point(6, 37);
                CadreBoutonExporter.Name = "CadreBoutonExporter";
                CadreBoutonExporter.Size = new Size(201, 32);
                CadreBoutonExporter.TabIndex = 21;
                CadreBoutonExporter.Text = "Label1";
                // 
                // BoutonExporter
                // 
                BoutonExporter.BackColor = Color.Transparent;
                BoutonExporter.Location = new Point(7, 38);
                BoutonExporter.Name = "BoutonExporter";
                BoutonExporter.Size = new Size(199, 30);
                BoutonExporter.TabIndex = 1;
                BoutonExporter.TabStop = false;
                BoutonExporter.Tag = "";
                BoutonExporter.Text = "Exporter un fichier GPX";
                BoutonExporter.UseVisualStyleBackColor = false;
                ToolTip1.SetToolTip(BoutonExporter, "Exporte la trace encours sous forme de fichier GPX." + CrLf +
                                                    "Cela permet l'échange avec d'autres applications ou le GPS.");
                // 
                // CadreBoutonSupprimer
                // 
                CadreBoutonSupprimer.BackColor = Color.Transparent;
                CadreBoutonSupprimer.BorderStyle = BorderStyle.FixedSingle;
                CadreBoutonSupprimer.FlatStyle = FlatStyle.Flat;
                CadreBoutonSupprimer.Location = new Point(6, 68);
                CadreBoutonSupprimer.Name = "CadreBoutonSupprimer";
                CadreBoutonSupprimer.Size = new Size(201, 32);
                CadreBoutonSupprimer.TabIndex = 25;
                CadreBoutonSupprimer.Text = "Label1";
                // 
                // BoutonSupprimer
                // 
                BoutonSupprimer.BackColor = Color.Transparent;
                BoutonSupprimer.Location = new Point(7, 69);
                BoutonSupprimer.Name = "BoutonSupprimer";
                BoutonSupprimer.Size = new Size(199, 30);
                BoutonSupprimer.TabIndex = 1;
                BoutonSupprimer.TabStop = false;
                BoutonSupprimer.Text = "Supprimer";
                BoutonSupprimer.UseVisualStyleBackColor = false;
                ToolTip1.SetToolTip(BoutonSupprimer, "Supprime la trace de l'affichage," + CrLf +
                                                     "de la collection et du fichier associé.");
                // 
                // PanelActions
                // 
                PanelActions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                PanelActions.BackColor = Color.FromArgb(247, 247, 247);
                PanelActions.Controls.Add(ButtonEnregistreSous);
                PanelActions.Controls.Add(CadreBoutonEnregistrerSous);
                PanelActions.Controls.Add(BoutonExporter);
                PanelActions.Controls.Add(CadreBoutonExporter);
                PanelActions.Controls.Add(BoutonSupprimer);
                PanelActions.Controls.Add(CadreBoutonSupprimer);
                PanelActions.Location = new Point(0, 0);
                PanelActions.Name = "Panel1";
                PanelActions.Size = new Size(214, 106);
                PanelActions.TabIndex = 26;
                // 
                // PasOk
                // 
                PasOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                PasOk.DialogResult = DialogResult.Cancel;
                PasOk.Location = new Point(26, 112);
                PasOk.Name = "PasOk";
                PasOk.Size = new Size(80, 30);
                PasOk.TabIndex = 1;
                PasOk.Text = "Annuler";
                PasOk.UseVisualStyleBackColor = true;
                ToolTip1.SetToolTip(PasOk, "Annule la fermetrue de la trace.");
                // 
                // Ok
                // 
                Ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                Ok.DialogResult = DialogResult.OK;
                Ok.Location = new Point(112, 112);
                Ok.Name = "Ok";
                Ok.Size = new Size(95, 30);
                Ok.TabIndex = 0;
                Ok.Text = "Fermer Trace";
                Ok.UseVisualStyleBackColor = true;
                ToolTip1.SetToolTip(Ok, "Supprime la trace de l'affichage." + CrLf +
                                        "Les dernières modifications sont enregistrées.");
                // 
                // EnregistrerTrace
                // 
                EnregistrerTrace.RestoreDirectory = true;
                // 
                // FermerTrace
                // 
                AcceptButton = Ok;
                AutoScaleMode = AutoScaleMode.None;
                CancelButton = PasOk;
                ClientSize = new Size(213, 148);
                ControlBox = false;
                Controls.Add(PanelActions);
                Controls.Add(PasOk);
                Controls.Add(Ok);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "FermerTrace";
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.CenterParent;
                PanelActions.ResumeLayout(false);
                ResumeLayout(false);
            }

            private Button BoutonExporter;
            private Label CadreBoutonExporter;
            private Button ButtonEnregistreSous;
            private Label CadreBoutonEnregistrerSous;
            private Button PasOk;
            private Button Ok;
            private SaveFileDialog EnregistrerTrace;
            private Button BoutonSupprimer;
            private ToolTip ToolTip1;
            private Label CadreBoutonSupprimer;
            private Panel PanelActions;
        }
        #endregion
    }
}