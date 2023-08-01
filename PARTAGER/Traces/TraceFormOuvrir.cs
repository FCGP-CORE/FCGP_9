using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.DonneesSiteWeb;

namespace FCGP.TRKs
{
    internal partial class TRK
    {
        #region Formulaire Ouvrir Trace en cours
        private partial class FormOuvrirTrace
        {
            private int IndexCollection
            {
                get
                {
                    if (ListeTraces.SelectedIndex > -1)
                    {
                        return ListeIndex[ListeTraces.SelectedIndex];
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            private List<TRK> _CollectionTraces;
            internal List<TRK> CollectionTraces
            {
                set
                {
                    _CollectionTraces = value;
                }
            }

            private int _IndexTrace;
            internal int IndexTrace
            {
                set
                {
                    _IndexTrace = value;
                }
            }
            private List<int> ListeIndex;
            private const string NomDefaut = "NouvelleTrace";
            private string Titre;
            private Rectangle StockClipCurseur;
            /// <summary> permet d'initialiser la liste des traces avec la collection des traces </summary>
            private void OuvrirTrace_Load(object sender, EventArgs e)
            {
                Titre = TitreInformation;
                TitreInformation = "Information Trace";
                StockClipCurseur = Cursor.Clip;
                ListeIndex = new List<int>(_CollectionTraces.Count);
                // on remplit la liste des traces à partir des traces présentes dans la collection et qui sont compatibles avec le site encours
                for (int Cpt = 0, loopTo = _CollectionTraces.Count - 1; Cpt <= loopTo; Cpt++)
                {
                    var T = _CollectionTraces[Cpt];
                    if (LimiteSiteWeb(IndiceSite).CoordonneesIntersectsWith(RegionDDToRegionGrille(T.BoundingBoxDD, SiteCarto)))
                    {
                        ListeTraces.Items.Add(T.Nom);
                        ListeIndex.Add(Cpt);
                        // on recherche si la trace que l'on vient d'ajouter correspond à la trace encours
                        if (_IndexTrace > -1 && (T.Nom ?? "") == (_CollectionTraces[_IndexTrace].Nom ?? ""))
                            ListeTraces.SelectedIndex = ListeTraces.Items.Count - 1;
                    }
                }
                // si pas de trace encours on en crée une nouvelle
                if (_IndexTrace == -1)
                    BoutonNouveau_Click(null, null);
                // met le curseur sur le centre du bouton OK
                Cursor.Position = new Point(Location.X + Ok.Location.X + Ok.Width / 2, Location.Y + Ok.Location.Y + Ok.Height / 2);
                // et limite les déplacements de la souris au formulaire
                Cursor.Clip = new Rectangle(Location, Size);
            }

            private void OuvrirTrace_FormClosed(object sender, FormClosedEventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                TitreInformation = Titre;
            }
            /// <summary> crée une nouvelle trace avec un nom par default et la couleur par default </summary>
            private void BoutonNouveau_Click(object sender, EventArgs e)
            {
                NomTrace.Text = NomDefaut;
                LabelCouleurTrace.BackColor = Settings.TracesSettings.SEG_TRK;
                ListeTraces.SelectedIndex = -1;
                BoutonNouveau.Enabled = false;
                NomTrace.SelectionStart = NomTrace.Text.Length;
            }
            /// <summary> sélectionne une trace à partir de la liste </summary>
            private void ListeTraces_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (ListeTraces.SelectedIndex > -1)
                {
                    NomTrace.Text = ListeTraces.SelectedItem.ToString();
                    LabelCouleurTrace.BackColor = _CollectionTraces[IndexCollection].CouleurSegment;
                    BoutonNouveau.Enabled = true;
                }
            }
            /// <summary> importe une trace à partir d'un fichier GPX et l'ajoute dans la liste des traces </summary>
            private void BoutonImport_Click(object sender, EventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                if (OpenFileGPX.ShowDialog() == DialogResult.OK)
                {
                    // si l'import de la trace c'est bien passé
                    if (OuvrirTrace(OpenFileGPX.FileName, TypesFichier.GPX))
                    {
                        // indextrace 
                        _IndexTrace = _CollectionTraces.Count - 1;
                        // si la surface de la trace est compatible avec les limites du site encours
                        if (LimiteSiteWeb(IndiceSite).CoordonneesIntersectsWith(RegionDDToRegionGrille(_CollectionTraces[_IndexTrace].BoundingBoxDD, SiteCarto)))
                        {
                            // on prend la nouvelle trace en compte
                            NomTrace.Text = _CollectionTraces[_IndexTrace].Nom;
                            LabelCouleurTrace.BackColor = _CollectionTraces[_IndexTrace].CouleurSegment;
                            ListeTraces.Items.Add(NomTrace.Text); // on ajoute le nom de la trace à la liste de sélection
                            ListeIndex.Add(_IndexTrace); // on ajoute l'index de la trace dans la collection à la liste des index
                            ListeTraces.SelectedIndex = ListeTraces.Items.Count - 1; // on sélectionne la trace importée donc la dernière
                            BoutonNouveau.Enabled = true;
                        }
                        else
                        {
                            // sinon on informe l'utilisateur que la trace a été importée mais n'est pas compatible avec le site en cours
                            MessageInformation = "La trace a été importée" + CrLf +
                                                 "mais elle n'est pas compatible" + CrLf +
                                                 "avec les limites du siteCarto actuel";
                            AfficherInformation();
                        }
                    }
                    else
                    {
                        BoutonNouveau_Click(null, null);
                    }
                }
                Cursor.Clip = new Rectangle(Location, Size);
            }
            /// <summary> permet de changer la couleur de la trace </summary>
            private void CouleurTrace_Click(object sender, EventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                DialogCouleurTrace.Color = LabelCouleurTrace.BackColor;
                DialogCouleurTrace.ShowDialog(this);
                LabelCouleurTrace.BackColor = DialogCouleurTrace.Color;
                Cursor.Clip = new Rectangle(Location, Size);
            }
            /// <summary> suite au renommage du nom de trace celui existe donc on avertit qu'il existe déjà</summary>
            private void ReInitialiseNomTrace()
            {
                MessageInformation = "Le nom de la trace existe déjà dans la collection." + CrLf +
                                     "Veuillez le changer";
                AfficherInformation();
                if (ListeTraces.SelectedIndex == -1)
                {
                    NomTrace.Text = NomDefaut;
                }
                else
                {
                    NomTrace.Text = _CollectionTraces[IndexCollection].Nom;
                }
            }
            /// <summary> transmet à l'appelant </summary>
            private void OuvrirTrace_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (DialogResult == DialogResult.OK)
                {
                    if (ListeTraces.SelectedIndex == -1)
                    {
                        if (IsExisteNomTrace(NomTrace.Text))
                        {
                            e.Cancel = true;
                            ReInitialiseNomTrace();
                            return;
                        }
                    }
                    else if ((NomTrace.Text ?? "") != (ListeTraces.SelectedItem.ToString() ?? ""))
                    {
                        if (IsExisteNomTrace(NomTrace.Text))
                        {
                            e.Cancel = true;
                            ReInitialiseNomTrace();
                            return;
                        }
                        else
                        {
                            RenommerTrace(IndexCollection, NomTrace.Text);
                        }
                    }
                    Tag = new ResumeTrace(IndexCollection, NomTrace.Text, true, LabelCouleurTrace.BackColor);
                }
            }
            /// <summary> enlève les caractères interdits dans les noms de fichiers sous windows </summary>
            private void NomTrace_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (Array.IndexOf(CarsInterdits, e.KeyChar) != -1)
                    e.KeyChar = default;
            }
        }
        private partial class FormOuvrirTrace : Form
        {

            internal FormOuvrirTrace()
            {
                InitializeComponent();
                InitialiserEvenements();
            }
            private void InitialiserEvenements()
            {
                // contrôles simples
                ListeTraces.SelectedIndexChanged += ListeTraces_SelectedIndexChanged;
                LabelCouleurTrace.Click += CouleurTrace_Click;
                NomTrace.KeyPress += NomTrace_KeyPress;
                BoutonNouveau.Click += BoutonNouveau_Click;
                BoutonImport.Click += BoutonImport_Click;
                // formulaire
                Load += OuvrirTrace_Load;
                FormClosing += OuvrirTrace_FormClosing;
                FormClosed += OuvrirTrace_FormClosed;
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
                ListeTraces = new ListBox();
                LabelListeTraces = new Label();
                PasOk = new Button();
                Ok = new Button();
                LabelCouleurTrace = new Label();
                NomTrace = new TextBox();
                LabelNom = new Label();
                BoutonNouveau = new Button();
                CadreBoutonNouveau = new Label();
                DialogCouleurTrace = new ColorDialog();
                BoutonImport = new Button();
                CadreBoutonImport = new Label();
                OpenFileGPX = new OpenFileDialog();
                ToolTip1 = new ToolTip(components);
                PanelAction = new Panel();
                PanelAction.SuspendLayout();
                SuspendLayout();
                // 
                // CadreBoutonNouveau
                // 
                CadreBoutonNouveau.BackColor = Color.Transparent;
                CadreBoutonNouveau.BorderStyle = BorderStyle.FixedSingle;
                CadreBoutonNouveau.FlatStyle = FlatStyle.Flat;
                CadreBoutonNouveau.Location = new Point(6, 6);
                CadreBoutonNouveau.Name = "CadreBoutonNouveau";
                CadreBoutonNouveau.Size = new Size(204, 32);
                CadreBoutonNouveau.TabIndex = 15;
                CadreBoutonNouveau.Text = "Label1";
                // 
                // BoutonNouveau
                // 
                BoutonNouveau.Location = new Point(7, 7);
                BoutonNouveau.Name = "BoutonNouveau";
                BoutonNouveau.Size = new Size(202, 30);
                BoutonNouveau.TabIndex = 14;
                BoutonNouveau.TabStop = false;
                BoutonNouveau.Text = "Créer une nouvelle trace";
                ToolTip1.SetToolTip(BoutonNouveau, "Appuyez sur ce bouton pour créer une nouvelle trace." + CrLf +
                                                   "Vous pouvez changer le nom et la couleur  par défaut de la nouvelle trace.");
                BoutonNouveau.UseVisualStyleBackColor = true;
                // 
                // CadreBoutonImport
                // 
                CadreBoutonImport.BackColor = Color.Transparent;
                CadreBoutonImport.BorderStyle = BorderStyle.FixedSingle;
                CadreBoutonImport.FlatStyle = FlatStyle.Flat;
                CadreBoutonImport.Location = new Point(6, 37);
                CadreBoutonImport.Name = "CadreBoutonImport";
                CadreBoutonImport.Size = new Size(204, 32);
                CadreBoutonImport.TabIndex = 17;
                CadreBoutonImport.Text = "Label1";
                // 
                // BoutonImport
                // 
                BoutonImport.Location = new Point(7, 38);
                BoutonImport.Name = "BoutonImport";
                BoutonImport.Size = new Size(202, 30);
                BoutonImport.TabIndex = 16;
                BoutonImport.TabStop = false;
                BoutonImport.Text = "Importer un fichier GPX";
                ToolTip1.SetToolTip(BoutonImport, "Importer une trace d'un autre logiciel." + CrLf +
                                                  "La trace est importée dans la collection. Si son emprise n'est pas" + CrLf +
                                                  "compatible  avec le site encours elle ne sera pas affichée." + CrLf + "Vous pouvez changer le nom et la couleur de la trace importée.");
                BoutonImport.UseVisualStyleBackColor = true;
                // 
                // LabelListeTraces
                // 
                LabelListeTraces.BackColor = Color.Transparent;
                LabelListeTraces.BorderStyle = BorderStyle.FixedSingle;
                LabelListeTraces.FlatStyle = FlatStyle.Flat;
                LabelListeTraces.Location = new Point(6, 68);
                LabelListeTraces.Name = "LabelListeTraces";
                LabelListeTraces.Size = new Size(204, 21);
                LabelListeTraces.TabIndex = 3;
                LabelListeTraces.Text = "Sélectionner une trace existante";
                LabelListeTraces.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // ListeTraces
                // 
                ListeTraces.BackColor = Color.White;
                ListeTraces.Location = new Point(6, 88);
                ListeTraces.Name = "ListeTraces";
                ListeTraces.Size = new Size(204, 118);
                ListeTraces.TabIndex = 2;
                ListeTraces.TabStop = false;
                ToolTip1.SetToolTip(ListeTraces, "Cliquez sur une des traces présentes dans la collection" + CrLf +
                                                 "pour l'ouvrir." + CrLf +
                                                 "Vous pouvez modifier le nom et la couleur de la trace existante.");
                // 
                // LabelNom
                // 
                LabelNom.BackColor = Color.Transparent;
                LabelNom.BorderStyle = BorderStyle.FixedSingle;
                LabelNom.FlatStyle = FlatStyle.Flat;
                LabelNom.Location = new Point(6, 212);
                LabelNom.Name = "LabelNom";
                LabelNom.Size = new Size(204, 21);
                LabelNom.TabIndex = 7;
                LabelNom.Text = "Nom de la trace";
                LabelNom.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // NomTrace
                // 
                NomTrace.BackColor = Color.White;
                NomTrace.Location = new Point(6, 232);
                NomTrace.Name = "NomTrace";
                NomTrace.Size = new Size(204, 23);
                NomTrace.TabIndex = 0;
                ToolTip1.SetToolTip(NomTrace, "Nom de la trace." + CrLf +
                                              "Si vous modifiez le nom, le fichier associé sera également renommé.");
                // 
                // LabelCouleurTrace
                // 
                LabelCouleurTrace.BackColor = Color.Transparent;
                LabelCouleurTrace.BorderStyle = BorderStyle.FixedSingle;
                LabelCouleurTrace.FlatStyle = FlatStyle.Flat;
                LabelCouleurTrace.Location = new Point(6, 253);
                LabelCouleurTrace.Name = "LabelCouleurTrace";
                LabelCouleurTrace.Size = new Size(204, 23);
                LabelCouleurTrace.TabIndex = 1;
                ToolTip1.SetToolTip(LabelCouleurTrace, "Couleur de la trace");
                // 
                // PanelAction
                // 
                PanelAction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                PanelAction.BackColor = Color.FromArgb(247, 247, 247);
                PanelAction.Controls.Add(BoutonNouveau);
                PanelAction.Controls.Add(CadreBoutonNouveau);
                PanelAction.Controls.Add(BoutonImport);
                PanelAction.Controls.Add(CadreBoutonImport);
                PanelAction.Controls.Add(LabelListeTraces);
                PanelAction.Controls.Add(ListeTraces);
                PanelAction.Controls.Add(LabelNom);
                PanelAction.Controls.Add(NomTrace);
                PanelAction.Controls.Add(LabelCouleurTrace);
                PanelAction.Location = new Point(0, 0);
                PanelAction.Name = "PanelAction";
                PanelAction.Size = new Size(215, 283);
                PanelAction.TabIndex = 18;
                // 
                // PasOk
                // 
                PasOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                PasOk.DialogResult = DialogResult.Cancel;
                PasOk.Location = new Point(29, 289);
                PasOk.Name = "PasOk";
                PasOk.Size = new Size(80, 30);
                PasOk.TabIndex = 3;
                PasOk.Text = "Annuler";
                ToolTip1.SetToolTip(PasOk, "Indique que vous n'avez pas choisi de trace et annule" + CrLf +
                                           "les modifications éventuelles de nom et de couleur.");
                PasOk.UseVisualStyleBackColor = true;
                // 
                // Ok
                // 
                Ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                Ok.DialogResult = DialogResult.OK;
                Ok.Location = new Point(115, 289);
                Ok.Name = "Ok";
                Ok.Size = new Size(95, 30);
                Ok.TabIndex = 2;
                Ok.Text = "Ouvrir Trace";
                ToolTip1.SetToolTip(Ok, "Affiche la trace sélectionnée." + CrLf +
                                        "Si une trace est présente sur l'affichage elle sera fermée.");
                Ok.UseVisualStyleBackColor = true;
                // 
                // OpenFileGPX
                // 
                OpenFileGPX.Filter = "\"GPX files (*.gpx)|*.gpx";
                OpenFileGPX.RestoreDirectory = true;
                OpenFileGPX.Title = "Ouvrir un fichier GPX";
                // 
                // ColorDialog
                // 
                DialogCouleurTrace.AllowFullOpen = false;
                DialogCouleurTrace.AnyColor = true;
                DialogCouleurTrace.SolidColorOnly = true;
                // 
                // OuvrirTrace
                // 
                AcceptButton = Ok;
                AutoScaleMode = AutoScaleMode.None;
                CancelButton = PasOk;
                ClientSize = new Size(216, 325);
                ControlBox = false;
                Controls.Add(PanelAction);
                Controls.Add(PasOk);
                Controls.Add(Ok);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "OuvrirTrace";
                ShowIcon = false;
                ShowInTaskbar = false;
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.CenterParent;
                PanelAction.ResumeLayout(false);
                PanelAction.PerformLayout();
                ResumeLayout(false);
            }

            private ListBox ListeTraces;
            private Label LabelListeTraces;
            private Button PasOk;
            private Button Ok;
            private Label LabelCouleurTrace;
            private TextBox NomTrace;
            private Label LabelNom;
            private Button BoutonNouveau;
            private Label CadreBoutonNouveau;
            private ColorDialog DialogCouleurTrace;
            private Button BoutonImport;
            private Label CadreBoutonImport;
            private OpenFileDialog OpenFileGPX;
            private ToolTip ToolTip1;
            private Panel PanelAction;
        }
        #endregion
    }
}