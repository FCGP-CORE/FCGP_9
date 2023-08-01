using static FCGP.Commun;

using static FCGP.Proprietes.Resources;

namespace FCGP
{
    /// <summary> affiche une information sous différentes formes </summary>
    static class AfficheInformation
    {
        #region Procédures et autres partagées
        /// <summary> message à afficher dans la fenêtre information. Commun à toutes les boites </summary>
        internal static string MessageInformation { get; set; }
        /// <summary>titre de la fenêtre information. Commun à toutes les boites </summary>
        internal static string TitreInformation { get; set; }
        /// <summary> action qui permet le rappel d'AfficherInformation à partir du thread UI </summary>
        internal static Action FormHorsUI { get; set; }
        internal static void AfficherInformation()
        {
            if (Environment.CurrentManagedThreadId != ID_Thread_IU)
            {
                FormApplication.Invoke(FormHorsUI);
            }
            else
            {
                using (var F = new DialogueInformation() { Boutons = TypeBoutons.Info_OK })
                {
                    F.ShowDialog();
                }
            }
        }
        /// <summary> sert a indiquer qu'il y a eu une erreur gérée dans l'application appelante </summary>
        internal static void AfficherErreur(Exception Ex, string CleProcedureErreur)
        {
            // on sauvegarde les messages d'information
            string Message = MessageInformation;
            string Titre = TitreInformation;
            string NomProcedureErreur = RenvoyerNomMethodeErreur(CleProcedureErreur);
            MessageInformation = NomProcedureErreur + CrLf + Ex.Message + CrLf + "Source : " + Ex.Source;
            // on indique l'erreur
            TitreInformation = TitreErreur + TypeFCGP;
            AfficherInformation();
            // on restore les messages d'information
            MessageInformation = Message;
            TitreInformation = Titre;
        }
        /// <summary> Affiche dans une boite de dialogue le message passé en paramètre et permet de cliquer sur un hyperlien </summary>
        /// <param name="NomLien"> texte à afficher pour le lien </param>
        /// <param name="UriLien"> addresse du lien </param>
        internal static void AfficherInformationEtLien(Form Parent, string NomLien, string UriLien)
        {

            using (var F = new DialogueInformation()
            {
                Boutons = TypeBoutons.Info_OK_Mail,
                NomLien = NomLien,
                UriLien = UriLien
            })
            {
                F.ShowDialog(Parent);
            }
        }
        /// <summary> Affiche dans une boite de dialogue le message passé en paramètre et demande une confirmation </summary>
        internal static DialogResult AfficherConfirmation()
        {
            using (var F = new DialogueInformation() { Boutons = TypeBoutons.Info_Oui_Non })
            {
                return F.ShowDialog();
            }
        }
        /// <summary> Affiche dans une boite de dialogue un message passé en paramètre et attend qu'une tache soit finie pour se fermer tout seul </summary>
        /// <param name="Parent"> formulaire appeleant </param>
        /// <returns> le dialogresult renvoyé par le tâche </returns>
        internal static DialogResult LancerTache(Func<DialogResult> Tache)
        {
            using (var F = new DialogueInformation() { Boutons = TypeBoutons.Tache, Tache = Tache })
            {
                return F.ShowDialog();
            }
        }
        /// <summary> affiche le fichier d'aide de l'application appelante </summary>
        /// <param name="Parent"> Le formulaire appelant </param>
        internal static void AfficherAide(Form Parent)
        {
            using (var F = new DialogueAide())
            {
                F.ShowDialog(Parent);
            }
        }
        /// <summary> affiche une image avec le centre de l'image au centre de la zone d'affichage.
        /// L'image peut être déplacée sur l'affichage avec un appui d'un bouton de la souris et déplacement de celle-ci </summary>
        /// <param name="Parent"> Le formulaire appelant </param>
        /// <param name="Image"> Image à afficher </param>
        /// <param name="DimensionsAffichage"> Largeur et hauteur de la zone d'affichage </param>
        internal static DialogResult AfficherImage(Form Parent, Image Image, Size DimensionsAffichage)
        {
            using (var F = new DialogueImage() { ImageCarte = Image, DimensionsAffichage = DimensionsAffichage })
            {
                return F.ShowDialog(Parent);
            }
        }
        private enum TypeBoutons
        {
            /// <summary> Texte d'information + Bouton OK </summary>
            Info_OK = 0,
            /// <summary> Texte d'information + Bouton OK + Bouton servant de lien sur une addresse web </summary>
            Info_OK_Mail = 1,
            /// <summary> Texte d'information + Bouton Oui + Bouton Non </summary>
            Info_Oui_Non = 2,
            /// <summary> Texte d'information sans bouton pour les tâches </summary>
            Tache = 3
        }
        #endregion
        #region DialogueInformation
        /// <summary> classe pouvant être remplacer par le taskdialog de windowsforms quand elle gera les hyperliens. Normalement Net 7.0 </summary>
        private partial class DialogueInformation : Form
        {
            /// <summary>largeur minimale de la fenêtre en fonction du ou des boutons demandés </summary>
            private readonly int[] LargeurMinBoutons = new int[] { 90, 195, 170, 0 };
            private Rectangle StockClipCurseur;
            private Func<DialogResult> _Tache;
            internal TypeBoutons Boutons { get; set; }
            internal string UriLien { get; set; }
            internal Func<DialogResult> Tache
            {
                set
                {
                    _Tache = value;
                }
            }
            internal string NomLien { get; set; }
            private const int LargeurTexteInit = 262; // Panel.Width -6 -6
            private const int HauteurTexteInit = 38; // Panel.Height -6 -6
            private void BoiteInformation_Load(object sender, EventArgs e)
            {
                SuspendLayout();
                StockClipCurseur = Cursor.Clip;
                Text = TitreInformation;
                if (Boutons < TypeBoutons.Tache)
                {
                    // il y a toujours un texte d'information
                    AfficherInformationBoutons();
                    // si il y a des boutons on les affiche
                    AfficherBoutons();
                }
                else
                {
                    AfficherInformationTache();
                }
                // on met à jour les dimensions des différents contrôles
                ResumeLayout(true);
                // on centre par rapport au formulaire de l'application sans passer le formulaire en tant que parent. 
                // Cela permet d'éviter les appels inter threads.  
                var V = VisueRectangle;
                var Pc = new Point(V.X + V.Width / 2, V.Y + V.Height / 2);
                Location = new Point(Pc.X - ClientSize.Width / 2, Pc.Y - ClientSize.Height / 2);
                // met le curseur sur le centre du bouton OK
                Cursor.Position = new Point(Location.X + B_OK.Location.X + B_OK.Width / 2 + 7, Location.Y + B_OK.Location.Y + B_OK.Height / 2 + 31);
                // et limite les déplacements de la souris au formulaire
                Cursor.Clip = new Rectangle(Location, Size);
            }
            /// <summary> utilise cet évenement pour tester les identifants et fermer automatiquement le formulaire </summary>
            private void DialogueTache_Shown(object sender, EventArgs e)
            {
                // on force le dessin du formulaire car il arrive qu'il n'est pas le temps de le faire avant de fermer
                Refresh();
                // si c'est une tâche on ferme la boite de dialogue au retour de la tache
                if (Boutons == TypeBoutons.Tache)
                {
                    DialogResult = _Tache();
                    Close();
                }
                Focus();
            }
            private void BoiteInformation_FormClosed(object sender, FormClosedEventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
            }
            private void AfficherInformationBoutons()
            {
                var TailleClient = ClientSize;
                int LargeurTexte = Information.ClientSize.Width;
                int Hauteurtexte = Information.ClientSize.Height;
                int LargeurTitre;
                using (var G = Information.CreateGraphics())
                {
                    LargeurTitre = G.MeasureString(Text, Font).ToSize().Width;
                }
                int LargeurMinFormulaire = Math.Max(LargeurMinBoutons[(int)Boutons], LargeurTitre);
                int LargeurFormulaire = Math.Max(LargeurTexte, LargeurMinFormulaire);
                int DeltaLargeur = LargeurFormulaire - LargeurTexteInit;
                int DeltaHauteur = Hauteurtexte - HauteurTexteInit;
                TailleClient = Size.Add(TailleClient, new Size(DeltaLargeur, DeltaHauteur));
                ClientSize = TailleClient;
            }
            private void AfficherInformationTache()
            {
                int LargeurTexte = Information.ClientSize.Width;
                int Hauteurtexte = Information.ClientSize.Height;
                // on supprime la hauteur réservée pour les boutons
                Controls.Remove(LienSite);
                Controls.Remove(B_ESC);
                Controls.Remove(B_OK);
                Information.Location = new Point(10, 10);
                Information.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Panel1.Dock = DockStyle.Fill;
                ClientSize = new Size(LargeurTexte + 20, Hauteurtexte + 20);
            }
            /// <summary> Affiche le ou les boutons concernés et vérifie que la largeur du formulaire permet de les voir </summary>
            private void AfficherBoutons()
            {
                switch (Boutons)
                {
                    case TypeBoutons.Info_OK:
                        {
                            CancelButton = B_OK;
                            B_ESC.Visible = false;
                            LienSite.Visible = false;
                            break;
                        }
                    case TypeBoutons.Info_Oui_Non:
                        {
                            B_OK.Text = "Oui";
                            LienSite.Visible = false;
                            break;
                        }
                    case TypeBoutons.Info_OK_Mail:
                        {
                            CancelButton = B_OK;
                            B_ESC.Visible = false;
                            LienSite.Text = NomLien;
                            break;
                        }
                }
            }
            /// <summary> ouvre le navigateur par défaut avec le lien du site de FCGP</summary>
            private void Info_2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                LienSite.LinkVisited = true;
                var psi = new ProcessStartInfo()
                {
                    FileName = UriLien,
                    UseShellExecute = true
                };
                Process.Start(psi);
                Close();
            }
        }
        private partial class DialogueInformation : Form
        {
            internal DialogueInformation()
            {
                InitializeComponent();
                InitialiserEvenements();
            }
            private void InitialiserEvenements()
            {
                LienSite.LinkClicked += Info_2_LinkClicked;
                Load += BoiteInformation_Load;
                Shown += DialogueTache_Shown;
                FormClosed += BoiteInformation_FormClosed;
            }

            // Requise par le Concepteur Windows Form
#pragma warning disable CS0169
            private IContainer components;
#pragma warning restore CS0169
            private void InitializeComponent()
            {
                B_OK = new Button();
                B_ESC = new Button();
                LienSite = new LinkLabel();
                Information = new Label();
                Panel1 = new Panel();
                Panel1.SuspendLayout();
                SuspendLayout();
                // 
                // B_OK
                // 
                B_OK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                B_OK.DialogResult = DialogResult.OK;
                B_OK.Location = new Point(193, 56);
                B_OK.Name = "B_OK";
                B_OK.Size = new Size(75, 30);
                B_OK.TabIndex = 1;
                B_OK.Text = "Fermer";
                B_OK.UseVisualStyleBackColor = true;
                // 
                // B_ESC
                // 
                B_ESC.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                B_ESC.DialogResult = DialogResult.Cancel;
                B_ESC.Location = new Point(112, 56);
                B_ESC.Name = "B_ESC";
                B_ESC.Size = new Size(75, 30);
                B_ESC.TabIndex = 2;
                B_ESC.Text = "Non";
                B_ESC.UseVisualStyleBackColor = true;
                // 
                // LienSite
                // 
                LienSite.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                LienSite.Location = new Point(6, 56);
                LienSite.Name = "LienSite";
                LienSite.Size = new Size(100, 30);
                LienSite.TabIndex = 3;
                LienSite.TabStop = true;
                LienSite.Text = "";
                LienSite.TextAlign = ContentAlignment.MiddleLeft;
                // 
                // Information
                // 
                Information.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                Information.Location = new Point(6, 6);
                Information.Padding = new Padding(0);
                Information.Margin = new Padding(0);
                Information.Name = "Information";
                Information.AutoSize = true;
                Information.Text = MessageInformation;
                Information.TabIndex = 4;
                // 
                // Panel1
                // 
                Panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Panel1.BackColor = Color.FromArgb(247, 247, 247);
                Panel1.Controls.Add(Information);
                Panel1.Location = new Point(0, 0);
                Panel1.Name = "Panel1";
                Panel1.Size = new Size(274, 50);
                Panel1.TabIndex = 5;
                // 
                // BoiteInformation
                // 
                AutoScaleMode = AutoScaleMode.None;
                BackColor = SystemColors.Control;
                ClientSize = new Size(274, 92);
                ControlBox = false;
                Controls.Add(Panel1);
                Controls.Add(LienSite);
                Controls.Add(B_ESC);
                Controls.Add(B_OK);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "BoiteInformation";
                ShowIcon = false;
                ShowInTaskbar = false;
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.Manual;
                TopMost = true;
                AcceptButton = B_OK;
                CancelButton = B_ESC;
                TopMost = true;
                Panel1.ResumeLayout(false);
                ResumeLayout(false);
            }

            private Button B_OK;
            private Button B_ESC;
            private LinkLabel LienSite;
            private Label Information;
            private Panel Panel1;
        }
        #endregion
        #region DialogueAide
        /// <summary> affiche un formulaire d'aide concernant l'utilisation de l'application appelante </summary>
        private partial class DialogueAide : Form
        {
            private Rectangle StockClipCurseur;
            /// <summary> charge le fichier d'aide </summary>
            private void FormAide_Load(object sender, EventArgs e)
            {
                StockClipCurseur = Cursor.Clip;
                byte[] DonneesAide = (byte[])ResourceManager.GetObject("Aide_FCGP_" + TypeFCGP, Culture);
                using (var MS = new MemoryStream(DonneesAide))
                {
                    Aide.LoadFile(MS, RichTextBoxStreamType.RichText);
                }
                Aide.RightMargin = ClientSize.Width - 37;
                Cursor.Clip = new Rectangle(Location, Size);
            }
            private void FormAide_FormClosed(object sender, FormClosedEventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
            }
            /// <summary> lance un navigateur avec l'adresse du site sélectionnée </summary>
            private void Aide_LinkClicked(object sender, LinkClickedEventArgs e)
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }
        private partial class DialogueAide : Form
        {
            internal DialogueAide()
            {
                InitializeComponent();
                InitialiserEvenements();
            }
            private void InitialiserEvenements()
            {
                Aide.LinkClicked += Aide_LinkClicked;
                Load += FormAide_Load;
                FormClosed += FormAide_FormClosed;
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
                Fond = new Panel();
                Aide = new RichTextBox();
                Fermer = new Button();
                Infos = new ToolTip(components);
                SuspendLayout();
                // 
                // Aide
                // 
                Aide.Dock = DockStyle.Fill;
                Aide.BackColor = Color.Linen;
                Aide.BorderStyle = BorderStyle.None;
                Aide.Name = "Aide";
                Aide.ReadOnly = true;
                Aide.ScrollBars = RichTextBoxScrollBars.Vertical;
                Aide.TabIndex = 0;
                Aide.TabStop = false;
                Aide.Text = "";
                // 
                // Fond
                // 
                Fond.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Fond.BackColor = Color.Linen;
                Fond.Location = new Point(0, 0);
                Fond.Name = "Fond";
                Fond.Size = new Size(798, 557);
                Fond.Padding = new Padding(10, 10, 0, 0);
                Fond.TabIndex = 4;
                Fond.Controls.Add(Aide);
                // 
                // Fermer
                // 
                Fermer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                Fermer.BackColor = SystemColors.Control;
                Fermer.DialogResult = DialogResult.Cancel;
                Fermer.Location = new Point(717, 563);
                Fermer.Name = "Fermer";
                Fermer.TextAlign = ContentAlignment.MiddleCenter;
                Fermer.Size = new Size(75, 30);
                Fermer.TabIndex = 3;
                Fermer.Text = "Fermer";
                Infos.SetToolTip(Fermer, "Ferme le formulaire d'aide");
                Fermer.UseVisualStyleBackColor = true;
                // 
                // FormAide
                // 
                AcceptButton = Fermer;
                AutoScaleMode = AutoScaleMode.None;
                BackColor = SystemColors.Control;
                CancelButton = Fermer;
                ClientSize = new Size(798, 599);
                ControlBox = false;
                Controls.Add(Fond);
                Controls.Add(Fermer);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                Name = "FormAide";
                ShowIcon = false;
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.CenterParent;
                TopMost = true;
                Text = null;
                ResumeLayout(false);
            }

            private Button Fermer;
            private ToolTip Infos;
            private Panel Fond;
            private RichTextBox Aide;
        }
        #endregion
        #region DialogueImage
        /// <summary> affiche un formulaire qui affiche une image et permet de la déplacer sur la surface de celui-ci </summary>
        private partial class DialogueImage : Form
        {
            /// <summary> coordonnées en pixels du coin haut gauche de l'image par rapport à la zone d'affichage </summary>
            private Point Pt0;
            /// <summary> Largeur et hauteur (carré) de la zone d'affichage de l'image </summary>
            internal Size DimensionsAffichage { get; set; }
            /// <summary> image à afficher </summary>
            internal Image ImageCarte { get; set; }
            /// <summary> positionnement de l'image sur le formulaire d'affichage. Permet le scrolling de l'image
            /// juste en faisant varier Pt0 </summary>
            private Rectangle RegionImage
            {
                get
                {
                    return new Rectangle(Pt0, ImageCarte.Size);
                }
            }
            /// <summary> bornes à ne pas dépasser pour Pt0 </summary>
            private int BorneMiniX, BorneMaxiX, BorneMiniY, BorneMaxiY;
            /// <summary> indique qu'un bouton de souris est appuié </summary>
            private bool FlagMouseDown;
            /// <summary> position du curseur de souris lors du dernier événement de souris </summary>
            private Point PtMouse;
            /// <summary> limitation sur le déplacement du curseur de souris du formulaire appelant </summary>
            private Rectangle StockClipCurseur;
            private bool FlagDeplacement;
            /// <summary> initialise les différentes varaibles et la psotion du formulaire </summary>
            private void DialogueImage_Load(object sender, EventArgs e)
            {
                StockClipCurseur = Cursor.Clip;
                FlagMouseDown = false;
                BorneMiniX = -ImageCarte.Width + 10;
                BorneMaxiX = DimensionsAffichage.Width - 10;
                BorneMiniY = -ImageCarte.Height + 10;
                BorneMaxiY = DimensionsAffichage.Height - 10;
                Pt0 = new Point((DimensionsAffichage.Width - ImageCarte.Width) / 2, (DimensionsAffichage.Height - ImageCarte.Height) / 2);
                ClientSize = DimensionsAffichage.Width == 0 ? new Size(700, 700 + Info.Height) : new Size(DimensionsAffichage.Width, DimensionsAffichage.Height + Info.Height);
                // centre le formulaire sur le formulaire parent
                if (Owner is null)
                {
                    var V = VisueRectangle;
                    var Pc = new Point(V.X + V.Width / 2, V.Y + V.Height / 2);
                    Location = new Point(Pc.X - ClientSize.Width / 2, Pc.Y - ClientSize.Height / 2);
                }
                else
                {
                    {
                        var withBlock = Owner;
                        Location = new Point(withBlock.Location.X + (withBlock.Width - Width) / 2, withBlock.Location.Y + (withBlock.Height - Height) / 2);
                    }
                }
                Cursor.Position = new Point(Location.X + DimensionsAffichage.Width / 2, Location.Y + DimensionsAffichage.Height / 2);
                var RegionAffichage = new Rectangle(Point.Empty, Affichage.ClientSize);
                FlagDeplacement = !RegionAffichage.Contains(RegionImage);
                // et limite les déplacements de la souris au formulaire
                Cursor.Clip = new Rectangle(Location, Size);
            }
            /// <summary> replace les limitations de déplacement du curseur de souris du formulaire appeleant </summary>
            private void DialogueImage_FormClosed(object sender, FormClosedEventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
            }
            /// <summary> Ferme le formulaire sur Escape ou Enter en différencier le dialogresult du formulaire </summary>
            private void DialogueImage_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                if (e.KeyCode == Keys.Enter)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            /// <summary> enregistre le fait d'appuyer sur un bouton et l'emplacement de la souris </summary>
            private void Affichage_MouseDown(object sender, MouseEventArgs e)
            {
                FlagMouseDown = true;
                PtMouse = e.Location;
            }
            /// <summary> calcule le delta de déplacement de la souris et modifie le PT0 de l'image en conséquence </summary>
            private void Affichage_MouseMove(object sender, MouseEventArgs e)
            {
                if (FlagMouseDown && FlagDeplacement)
                {
                    int X = Pt0.X - PtMouse.X + e.X;
                    int Y = Pt0.Y - PtMouse.Y + e.Y;
                    if (X < BorneMiniX)
                    {
                        Pt0.X = BorneMiniX;
                    }
                    else if (X > BorneMaxiX)
                    {
                        Pt0.X = BorneMaxiX;
                    }
                    else
                    {
                        Pt0.X = X;
                    }
                    if (Y < BorneMiniY)
                    {
                        Pt0.Y = BorneMiniY;
                    }
                    else if (Y > BorneMaxiY)
                    {
                        Pt0.Y = BorneMaxiY;
                    }
                    else
                    {
                        Pt0.Y = Y;
                    }
                    Affichage.Invalidate();
                    PtMouse = e.Location;
                }
            }
            /// <summary> enregistre le fait de relacher le bouton de la souris </summary>
            private void Affichage_MouseUp(object sender, MouseEventArgs e)
            {
                FlagMouseDown = false;
            }
            /// <summary> dessinne l'image à l'endroit spécifié sur la zone d'affichage </summary>
            private void Affichage_Paint(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawImage(ImageCarte, RegionImage);
            }
        }
        private partial class DialogueImage : Form
        {

            public DialogueImage()
            {
                InitializeComponent();
                InitialiserEvenements();
            }

            private void InitialiserEvenements()
            {
                Load += DialogueImage_Load;
                FormClosed += DialogueImage_FormClosed;
                KeyDown += DialogueImage_KeyDown;
                Affichage.Paint += Affichage_Paint;
                Affichage.MouseDown += Affichage_MouseDown;
                Affichage.MouseMove += Affichage_MouseMove;
                Affichage.MouseUp += Affichage_MouseUp;
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
                Affichage = new PictureBox();
                Info = new Label();
                ((ISupportInitialize)Affichage).BeginInit();
                SuspendLayout();
                // 
                // Affichage
                // 
                Affichage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Affichage.BorderStyle = BorderStyle.None;
                Affichage.Location = new Point(0, 0);
                Affichage.Margin = new Padding(0);
                Affichage.Name = "Affichage";
                Affichage.Size = new Size(700, 700);
                Affichage.TabIndex = 0;
                Affichage.TabStop = false;
                // 
                // Info
                // 
                Info.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Info.BorderStyle = BorderStyle.FixedSingle;
                Info.FlatStyle = FlatStyle.Flat;
                Info.Location = new Point(-1, 701);
                Info.Name = "Info";
                Info.Size = new Size(702, 23);
                Info.TabIndex = 1;
                Info.Font = new Font("Segoe UI", 14.0f, FontStyle.Bold, GraphicsUnit.Pixel);
                Info.Text = "Click pour déplacer Image, Esc ou Enter pour fermer";
                Info.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // AfficheImage
                // 
                AutoScaleMode = AutoScaleMode.None;
                ClientSize = new Size(700, 723);
                ControlBox = false;
                Controls.Add(Info);
                Controls.Add(Affichage);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "AfficheImage";
                ShowIcon = false;
                ShowInTaskbar = false;
                ((ISupportInitialize)Affichage).EndInit();
                ResumeLayout(false);
            }

            private PictureBox Affichage;
            private Label Info;
        }
        #endregion
    }
}