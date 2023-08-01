using System.Drawing.Printing;
using System.Drawing.Text;

using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;
using static FCGP.ObjetRectangle;

namespace FCGP
{
    /// <summary> pour imprimer 1 carte géoreférencée. pas d'impression simultanée </summary>
    static class ImprimerCarte
    {
        #region Document Page Impression
        /// <summary> 8 valeurs en Mm converties en pixels pour éviter des calculs redondants lors du du dessin des pages </summary>
        private static int PixelsRecouvreX, PixelsRecouvreY, PixelsLargeurImprimable, PixelsHauteurImprimable;
        private static int PixelsCentrageXDeb, PixelsCentrageYDeb, PixelsCentrageXFin, PixelsCentrageYFin;
        /// <summary> image de la carte à imprimer </summary>
        private static Image ImageCarte;
        /// <summary> valeurs réeeles du recouvrement entre 2 pages sur les 2 axes (en Mm) </summary>
        private static float RecouvreX, RecouvreY;
        /// <summary> echelle d'impression réelle de la carte  </summary>
        private static Font FPoint;
        /// <summary> différence entre la hauteur de ligne de la font et la hauteur de la font </summary>
        private static float DeltaYFontPage;
        /// <summary> texte invariant de l'impression des coordonnées </summary>
        private static string TexteInvariant;
        /// <summary> valeur de la marge pour l'impression des coordonnées de la page si demandé (en mm) </summary>
        private const int MargeCoordonnees = 3;
        #endregion
        #region ValeurImpression : Commune module et IU
        #region Variables issues du Fichier Georef
        /// <summary> Chemin du fichier georef et du ou des fichiers images associés (carte et ou références coordonnées) </summary>
        private static string CheminFichierGeoref;
        /// <summary> Nom du fichier image à imprimer </summary>
        private static string NomCarte;
        /// <summary> dimension en pixels de la carte </summary>
        private static Size PixelGeoref;
        /// <summary> dimensions réelles de la carte exprimées en millimètres. </summary>
        private static SizeF MmGeoref;
        /// <summary> coordonnées virtuelles du Pt0 de la carte (pixels). </summary>
        private static Point Pt0Pixels;
        /// <summary> site carto de la carte </summary>
        private static SitesCartographiques SiteCartoCarte;
        /// <summary> site carto de la carte </summary>
        private static Echelles EchelleCarte;
        #endregion
        #region Document Impression
        /// <summary> imprimante pour le document </summary>
        private static string NomImprimante;
        /// <summary> taille du papier sélectionné pour le document sans tenir compte de l'orientation (portrait ou paysage) </summary>
        private static PaperSize Papier;
        /// <summary> résolution sélectionnée pour le document </summary>
        private static PrinterResolution Resolution;
        /// <summary> valeur de la marge des bords Top, Bottom, Left et Right qui ne sera pas imprimée (en mm) hors valeur minimale due au matériel </summary>
        private static int Bords;
        /// <summary> valeur du recouvrement entre 2 feuilles (en mm) </summary>
        private static int ReCouvre;
        /// <summary> indique si les coordonnées du PT0 des pages sont immprimées </summary>
        private static bool IsCoordonnees;
        /// <summary> orientation des pages : Paysage si true sinon portrait </summary>
        private static bool IsPaysage;
        /// <summary> indique si la carte est imprimée en couleur ou en niveau de gris </summary>
        private static bool IsColor;
        /// <summary> indique si l'imprimante peut imprimer en recto verso </summary>
        private static bool IsRectoVerso;
        /// <summary> largeur et hauteur imprimable d'une page prenant en compte l'orientation (portrait ou paysage) </summary>
        private static float LargeurPageImprimable, HauteurPageImprimable;
        /// <summary> dimensions de la surface de la carte 1 fois imprimée (en mm) </summary>
        private static SizeF DimensionsCarteImprimable;
        /// <summary> concerne les pages à imprimer  </summary>
        private static int NbPagesX, NbPagesY;
        /// <summary> echelle d'impression réelle de la carte  </summary>
        private static string EchelleImpression;
        /// <summary> valeur des décalages pour centrer ou pas l'image de la carte sur l'ensemble des pages à imprimer (en mm) </summary>
        private static float CentrageImprXDeb, CentrageImprXFin, CentrageImprYDeb, CentrageImprYFin;
        /// <summary> Nb de pages à imprimer et Nb de pages déjà imprimées </summary>
        private static int NbPagesTotal, NbPagesImprimees;
        /// <summary> liste des pages à imprimer </summary>
        private static int[] ListePages;
        #endregion
        #endregion
        /// <summary> indique si une impression est en cours </summary>
        internal static bool IsPrinting { get; private set; }
        /// <summary> méthode à appeler pour imprimer une carte. 1 seule impression encours </summary>
        internal async static void LancerImpressionCarteAsync()
        {
            using (var R = new ChoixImpressionCarte())
            {
                if (R.ShowDialog(FormApplication) == DialogResult.OK)
                {
                    IsPrinting = true;
                    await Task.Run(() => Imprimer());
                    MessageInformation = $"La carte {NomCarte} a été imprimée";
                    TitreInformation = "Information";
                    AfficherInformation();
                    IsPrinting = false;
                }
            }
        }
        /// <summary> arrondi une valeur théorique pour que celle ci soit un multiple d'une valeur entière (physique) </summary>
        /// <param name="Valeur"> valeur théoriquee </param>
        /// <param name="Multiple"> multiple </param>
        /// <returns> valeur entière (physique) </returns>
        private static int ArrondirValeur(ref float Valeur, float Multiple)
        {
            int Resultats = (int)Math.Round((double)(Valeur * Multiple), 0);
            Valeur = Resultats / Multiple;
            return Resultats;
        }
        /// <summary> Imprime la carte avec les choix de l'utilisateur </summary>
        private static void Imprimer()
        {
            NbPagesImprimees = 0;
            // on transforme les Mm théoriques de l'impression en pixels d'éviter des calculs redondants lors de l'impression
            // mais les pixels sont des entiers. On réajuste les valeurs théoriques des dimensions en Mm pour correspondre à un nombre entier de pixels
            // Coef X d'échelle Mm imprimé en Pixel image carte
            float MmToPixelX = PixelGeoref.Width / DimensionsCarteImprimable.Width;
            RecouvreX = ReCouvre;
            PixelsRecouvreX = ArrondirValeur(ref RecouvreX, MmToPixelX);
            PixelsLargeurImprimable = ArrondirValeur(ref LargeurPageImprimable, MmToPixelX);
            PixelsCentrageXDeb = ArrondirValeur(ref CentrageImprXDeb, MmToPixelX);
            // on calcule le nb de pixels nécessaire au centrage du fin de page
            PixelsCentrageXFin = NbPagesX * PixelsLargeurImprimable + PixelsRecouvreX - PixelGeoref.Width - PixelsCentrageXDeb;
            // on affecte les erreurs dues aux arrondis au décalage de fin
            CentrageImprXFin = PixelsCentrageXFin / MmToPixelX;
            // Normalement ce sont des pixels carrés (x=Y) sauf pour les cartes interpolées suisse
            // Coef Y d'échelle Mm imprimé en Pixel image carte
            float MmToPixelY = PixelGeoref.Height / DimensionsCarteImprimable.Height;
            RecouvreY = ReCouvre;
            PixelsRecouvreY = ArrondirValeur(ref RecouvreY, MmToPixelY);
            PixelsHauteurImprimable = ArrondirValeur(ref HauteurPageImprimable, MmToPixelY);
            PixelsCentrageYDeb = ArrondirValeur(ref CentrageImprYDeb, MmToPixelY);
            // on calcule le nb de pixels nécessaire au centrage de fin de page
            PixelsCentrageYFin = NbPagesY * PixelsHauteurImprimable + PixelsRecouvreY - PixelGeoref.Height - PixelsCentrageYDeb;
            // on affecte les erreurs dues aux arrondis au décalage de fin
            CentrageImprYFin = PixelsCentrageYFin / MmToPixelY;
            // configuration du document. Toutes les pages ont la même configuration
            using (var Carte = new PrintDocument())
            {
                Carte.PrintPage += Carte_PrintPage;
                Carte.DocumentName = NomCarte;
                Carte.PrinterSettings.PrinterName = NomImprimante;
                Carte.PrinterSettings.Copies = 1;
                // on imprime pas en rectoverso
                if (IsRectoVerso)
                    Carte.PrinterSettings.Duplex = Duplex.Simplex;
                Carte.DefaultPageSettings.Landscape = IsPaysage;
                Carte.DefaultPageSettings.PaperSize = Papier;
                Carte.DefaultPageSettings.Color = IsColor;
                Carte.DefaultPageSettings.PrinterResolution = Resolution;
                // on indique une sélection automatique de la source de papier par l'imprimante
                Carte.DefaultPageSettings.PaperSource = new PaperSource() { SourceName = "Sélection automatique du magasin", RawKind = 7 };
                // on ouvre l'image ici pour éviter de le faire lors de chaque impression de page
                ImageCarte = Image.FromFile(CheminFichierGeoref + @"\" + NomCarte);
                Carte.Print();
                ImageCarte.Dispose();
            }
        }
        /// <summary> imprime une page sur l'imprimante sélectionnée à la demande du document. Chaque page ne dépend que du nb de pages déjà imprimées
        /// on pourrait donc facilement n'imprimer qu'une plage de pages au lieu de la totalité </summary>
        private static void Carte_PrintPage(object sender, PrintPageEventArgs e)
        {
            // retrouve la colonne et le rang de la page à imprimer
            int NumPage = ListePages[NbPagesImprimees];
            int YImpr = Math.DivRem(NumPage, NbPagesX, out int XImpr);
            using (var g = e.Graphics)
            {
                g.PageUnit = GraphicsUnit.Millimeter; // on travaille en millimètre
                g.PageScale = 1f; // sans mise à l'échelle particulière
                // valeur en MM du décalage de l'image de la carte par rapport au coin haut gauche de la page en incluant les bords non imprimables.
                // Les pages sur les bords ont un delta qui sert à centrer l'impression de la carte
                float MmX = (XImpr == 0 ? CentrageImprXDeb : 0f) + Bords;
                float MmY = (YImpr == 0 ? CentrageImprYDeb : 0f) + Bords + (IsCoordonnees ? MargeCoordonnees : 0);
                // valeur en MM de la largeur et hauteur de la partie de l'image de la carte correspondant à la page actuelle.
                // Les pages sur le bord ont un delta qui sert à centrer l'impression de la carte
                float MmL = LargeurPageImprimable + RecouvreX - (XImpr == 0 ? CentrageImprXDeb : 0f) - (XImpr == NbPagesX - 1 ? CentrageImprXFin : 0f);
                float MmH = HauteurPageImprimable + RecouvreY - (YImpr == 0 ? CentrageImprYDeb : 0f) - (YImpr == NbPagesY - 1 ? CentrageImprYFin : 0f);
                // valeur en pixels du décalage de la partie de l'image de la carte
                int PixelsX = XImpr == 0 ? 0 : PixelsLargeurImprimable * XImpr - PixelsCentrageXDeb;
                int PixelsY = YImpr == 0 ? 0 : PixelsHauteurImprimable * YImpr - PixelsCentrageYDeb;
                // valeur en pixels de la largeur et de la hauteur de la partie de l'image de la carte correspondant à la page actuelle.
                int PixelsL = PixelsLargeurImprimable + PixelsRecouvreX - (XImpr == 0 ? PixelsCentrageXDeb : 0) - (XImpr == NbPagesX - 1 ? PixelsCentrageXFin : 0);
                int PixelsH = PixelsHauteurImprimable + PixelsRecouvreY - (YImpr == 0 ? PixelsCentrageYDeb : 0) - (YImpr == NbPagesY - 1 ? PixelsCentrageYFin : 0);
                // envoi de la partie de la carte correspondant à la page actuelle à l'imprimante                                   
                g.DrawImage(ImageCarte,
                            new RectangleF(new PointF(MmX, MmY), new SizeF(MmL, MmH)),              // le rectangle source est exprimé en pixels. Il s'agit de tout ou partie 
                            new Rectangle(new Point(PixelsX, PixelsY), new Size(PixelsL, PixelsH)), // le rectangle destination est exprimé en Mm sur la page
                            GraphicsUnit.Pixel);                                                    // de l'image de la carte afin de respecter l'échelle ou le nb de pages            

                if (IsCoordonnees)
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    if (NbPagesImprimees == 0)
                    {
                        FPoint = new Font("Segoe UI", 72.0f / 25.4f * MargeCoordonnees);
                        DeltaYFontPage = FPoint.GetHeight(g) - MargeCoordonnees;
                        TexteInvariant = $", Echelle {EchelleImpression}, {NomCarte}";
                    }
                    string T = $"Page N°{NumPage + 1}, {CoordonneesPage(PixelsX, PixelsY)}{TexteInvariant}";
                    g.DrawString(T, FPoint, Brushes.Black, Bords, Bords - DeltaYFontPage);
                }
            }
            // on indique que la page a été imprimée
            NbPagesImprimees += 1;
            // et on continue tant que toutes les pages n'ont pas été imprimées
            e.HasMorePages = NbPagesImprimees < NbPagesTotal;
        }
        /// <summary> transforme les coordonnées virtuelles du point Pt0 de la page en coordonnées Grille Suisse ou UTM </summary>
        /// <param name="PixelsX"> Coordonnée X du point PT0 </param>
        /// <param name="PixelsY"> Coordonnée Y du point PT0 </param>
        private static string CoordonneesPage(int PixelsX, int PixelsY)
        {
            var Pt0Page = new Point(Pt0Pixels.X + PixelsX, Pt0Pixels.Y + PixelsY);
            PointD Pt0;
            string Ret;
            if (SiteCartoCarte == SitesCartographiques.SuisseMobile)
            {
                Pt0 = PointPixelToPointGrille(Pt0Page, SiteCartoCarte, EchelleCarte);
                Ret = ConvertPointXYtoChaine(new PointProjection(Pt0));
            }
            else
            {
                Pt0 = PointPixelsToPointDD(Pt0Page, SiteCartoCarte, EchelleCarte);
                Ret = ConvertPointDDtoUTM(Pt0);
            }
            return Ret;
        }

        /// <summary> Formulaire concernant les choix de la carte à imprimer </summary>
        private partial class ChoixImpressionCarte : Form
        {
            /// <summary> Portrait indique que l'on prend pour les dimensions de la page imprimables largeur * Hauteur des dimensions de la page
            /// Paysage indique que l'on prend pour les dimensions de la page imprimables Hauteur * largeur des dimensions de la page </summary>
            private enum OrientationPapier : int
            {
                Portrait = 0,
                Paysage
            }
            /// <summary> Vertical indique que l'on prend pour les dimensions imprimables nbCols * nbRangs
            /// Horizontal indique que l'on prend pour les dimensions imprimables nbRangs * nbCols </summary>
            private enum OrientionCarte : int
            {
                Vertical = 0,
                Horizontal
            }
            #region Plan Impression
            /// <summary> pinceau  pour le dessin de la carte </summary>
            private readonly Pen Pen_Carte = new(Color.Blue, 1f);
            /// <summary> brosse semi transparente pour le dessin de la carte </summary>
            private readonly SolidBrush Brosse_Carte = new(Color.FromArgb(64, 0, 0, 64));
            /// <summary> pour la sélection des pages à imprimer </summary>
            private Keys Modifier;
            /// <summary> Retour de la visualisation d'une page à imprimer </summary>
            private Point PosMouse;
            /// <summary> numéro de la page à visualiser </summary>
            private int NumPage;
            /// <summary> dimensions des pages à imprimer sur la surface d'affichage du plan </summary>
            private Rectangle RegionPagesPlan;
            /// <summary> rapport Mm et pixel de la region d'impression du plan </summary>
            private float CoefR;
            /// <summary> valeur en pixel de la marge des bords </summary>
            private int Marge;
            /// <summary> valeur en pixel de la marge des coordonnées </summary>
            private int Coord;
            /// <summary> dimensions de la surface d'affichage du plan </summary>
            private Size TailleImpressionPlan;
            /// <summary> dimensions maximales pour le rectangle de travail (sélection) </summary>
            private Rectangle SurfaceRectangleTravail;
            /// <summary> taille de la carte en pixels du plan </summary>
            private Size TailleCartePlan;
            /// <summary> btmap de la carte pour pouvoir afficher une page en particulier </summary>
            private Bitmap BitMapCarte;
            /// <summary> valeur associé à l'impression des coordonnées sur la page. 0 si pas demandé ou MargeCoordonnées (en mm) </summary>
            private int BordCoordonnees;
            /// <summary> indique si la carte est centrée sur la surface imprimable </summary>
            private bool IsCentrage;
            /// <summary> dimensions réelles de la page de papier choisit (en mm) </summary>
            private SizeF DimensionsPapier;
            /// <summary> dimensions théoriques d'une page de papier une fois enlever les bords et le recouvrement 
            /// sans présager de l'orientation et de la marge des coordonnées (en mm) </summary>
            private SizeF DimensionsPageImprimable;
            /// <summary> dimensions théoriques de la surface imprimable représentée par l'ensemble des pages à imprimer et de leur orientation (en mm) </summary>
            private SizeF DimensionsPagesImprimable;
            /// <summary> valeur de toutes les marges du sens vertical soit 2 bords et un recouvrement (en mm) </summary>
            private int BordsXPageImprimable;
            /// <summary> valeur de toutes les marges du sens horizontal soit 2 bords, un recouvrement et une coordonnées (en mm) </summary>
            private int BordsYPageImprimable;
            /// <summary> rapport larg / haut de la carte sur les dimensions en Mm </summary>
            private float RapCarteMm;
            /// <summary> 1/echelle d'impression du fichier georef de la carte par exemple 25000 pour 1/25000 </summary>
            private float EchelleImpressionGeoref;
            /// <summary> valeur de non dessin entre la surface d'affichage du plan et celle du controle support (en pixels) </summary>
            private const int AffichageBords = 15;
            #endregion
            #region Imprimante
            /// <summary> liste des imprimantes acceptées par l'application: objets </summary>
            private List<string> NomsImprimantes;
            /// <summary> marge minimum du aux contraintes matérielles d'une imprimante donnée (en mm) </summary>
            private int BordsMini;
            /// <summary> liste des papiers acceptés pour une imprimante donnée : noms pour liste de choix </summary>
            private List<string> PapiersNoms;
            /// <summary> liste des papiers acceptés pour une imprimante donnée : objets </summary>
            private List<PaperSize> Papiers;
            /// <summary> liste des resolutions d'impression acceptées par une imprimante donnée : noms pour liste de choix </summary>
            private List<string> ResolutionsNoms;
            /// <summary> Liste des résolutions d'impression acceptées par une imprimante donnée : objets </summary>
            private List<PrinterResolution> Resolutions;
            /// <summary> flag indiquant si une imprimante donné supporte le mode paysage </summary>
            private bool IsSupportPaysage;
            /// <summary> flag indiquant si une imprimante donné supporte le mode couleur </summary>
            private bool IsSupportColor;
            #endregion
            private void ChoixImpressionCarte_Load(object sender, EventArgs e)
            {
                // donne l'imprimante par défaut du système
                var Imprimante = new PrinterSettings();
                // et son nom
                string NomImprimante = Imprimante.PrinterName;
                int IndexImprimante = -1;
                TitreInformation = "Information FCGP_Partager";
                OuvrirFichierGeoref.Filter = "Georef Files|*.georef";
                OuvrirFichierGeoref.Title = "Choix du fichier Georef";
                OuvrirFichierGeoref.InitialDirectory = Settings.CapturerSettings.CHEMIN_CARTE;
                // remplit la liste de choix des imprimantes acceptées du système 
                NomsImprimantes = new List<string>();
                foreach (string Nom in PrinterSettings.InstalledPrinters)
                {
                    // on ajoute les imprimantes physiques et virtuelles XPS ou PDF mais pas les traceurs
                    if (!(Nom.StartsWith("One") || Nom.StartsWith("Fax") || Imprimante.IsPlotter) && Imprimante.IsValid)
                    {
                        if ((NomImprimante ?? "") == (Nom ?? ""))
                            IndexImprimante = NomsImprimantes.Count;
                        NomsImprimantes.Add(Nom);
                    }
                }
                ListeChoixImprimante.Items.AddRange(NomsImprimantes.ToArray());
                ListeChoixImprimante.SelectedIndex = IndexImprimante;

                // valeurs constantes liées au contrôle d'affichage du plan de mapping des pages à imprimer
                TailleImpressionPlan = new Size(PlanImpression.ClientSize.Width - AffichageBords * 2, PlanImpression.ClientSize.Height - AffichageBords * 2);   // taille en pixels
                SurfaceRectangleTravail = new Rectangle(new Point(3, 3), new Size(PlanImpression.ClientSize.Width - 6, PlanImpression.ClientRectangle.Height - 6));
                // détermine la procédure à appeler lors d'un click sur une page du plan avec le modificateur Alt
                ActionRectangleSimple = AfficherImagePage;
                IsCentrage = ChoixCentrage.Checked;
                IsCoordonnees = ChoixCoordonnees.Checked;
            }
            private void ChoixImpressionCarte_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (DialogResult == DialogResult.OK)
                {
                    TitreInformation = "Information Impression Carte";
                    if (NbRectanglesSelected == 0)
                    {
                        MessageInformation = $"Il n'y a plus de page à imprimer";
                        e.Cancel = true;
                        return;
                    }
                    MessageInformation = $"Imprimer la carte avec ces choix ?";
                    if (AfficherConfirmation() == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
            private void ChoixImpressionCarte_FormClosed(object sender, FormClosedEventArgs e)
            {
                if (BitMapCarte is not null)
                {
                    BitMapCarte.Dispose();
                    BitMapCarte = null;
                }
                if (DialogResult == DialogResult.OK)
                {
                    // on indique les bonnes valeurs de centrage en fonction du choix final de l'utilisateur
                    if (!IsCentrage)
                    {
                        CentrageImprXFin += CentrageImprXDeb;
                        CentrageImprXDeb = 0f;
                        CentrageImprYFin += CentrageImprYDeb;
                        CentrageImprYDeb = 0f;
                    }
                    // on enlève la marge inhérente du matériel qui sera ajoutée automatiquement par celui-ci
                    Bords -= BordsMini;
                    // on indique les pages à imprimer
                    NbPagesTotal = NbRectanglesSelected;
                    ListePages = RectanglesSelected;
                    // on indique les coordonnées de la carte
                    TrouverPt0Pixels();
                    // et tous les renseignements de l'imprimante non liés directement au calcul de l'impression
                    NomImprimante = ListeChoixImprimante.Text;
                    Resolution = Resolutions[ListeChoixResolution.SelectedIndex];
                    IsColor = ListeChoixCouleur.SelectedIndex == 0;
                }
            }
            /// <summary> gestion du choix du fichier Georef </summary>
            private void LabelChoixFichierGeoref_Click(object sender, EventArgs e)
            {
                // Affiche la boite de dialogue de sélection des fichiers georef
                if (OuvrirFichierGeoref.ShowDialog() == DialogResult.OK)
                {
                    var Ret = LireFichierGeoref();
                    if (Ret != VerificationRenvoiCarte.OK)
                    {
                        MessageInformation = "Problème avec le fichier Georef : " + OuvrirFichierGeoref.FileName + CrLf + Ret.ToString();
                        AfficherInformation();
                    }
                    else
                    {
                        if (BitMapCarte is not null)
                        {
                            BitMapCarte.Dispose();
                            BitMapCarte = null;
                        }
                        OuvrirFichierGeoref.InitialDirectory = CheminFichierGeoref;
                        BitMapCarte = (Bitmap)Image.FromFile(CheminFichierGeoref + @"\" + NomCarte);
                        LabelChoixFichierGeoref.Text = NomCarte;
                        btnYES.Enabled = true;
                        btnYES.Focus();
                        CalculerImpression();
                    }
                }
            }
            /// <summary> Met à jour le label de l'échelle en fonction de la valeur choisie 
            /// avec éventuellement le lancement des calculs liés à l'impression </summary>
            private void ValeurEchelle_ValueChanged(object sender, EventArgs e)
            {
                if (ChoixEchelle.Checked && btnYES.Enabled)
                    CalculerImpression();
            }
            private void ValeurNbPages_ValueChanged(object sender, EventArgs e)
            {
                if (ChoixNbPages.Checked && btnYES.Enabled)
                    CalculerImpression();
            }
            /// <summary> Lancement des calculs liés à l'impression </summary>
            private void ValeurMarges_ValueChanged(object sender, EventArgs e)
            {
                if (btnYES.Enabled)
                    CalculerImpression();
            }
            /// <summary> Demande un nouvel affichage du plan de mapping </summary>
            private void ChoixCentrage_CheckedChanged(object sender, EventArgs e)
            {
                IsCentrage = ChoixCentrage.Checked;
                if (btnYES.Enabled)
                    PlanImpression.Invalidate();
            }
            /// <summary> Lancement des calculs liés à l'impression </summary>
            private void ChoixCoordonnees_CheckedChanged(object sender, EventArgs e)
            {
                IsCoordonnees = ChoixCoordonnees.Checked;
                if (btnYES.Enabled)
                    CalculerImpression();
            }
            /// <summary> ouvre les listes déroulantes </summary>
            private void Labels_Click(object sender, EventArgs e)
            {
                Label L = (Label)sender;
                ComboBox C = (ComboBox)L.Tag;
                if (L.Name == "LabelChoixCouleur" && !IsSupportColor)
                    return;
                C.DroppedDown = true;
            }
            /// <summary> Met à jour le texte du label associé à la liste </summary>
            private void Listes_SelectedIndexChanged(object sender, EventArgs e)
            {
                ComboBox C = (ComboBox)sender;
                Label L = (Label)C.Tag;
                L.Text = C.SelectedItem.ToString();
                if (C.Name == "ListeChoixImprimante")
                    ChoisirImprimante();
                if (C.Name == "ListeChoixPapier" && btnYES.Enabled)
                    CalculerImpression();
            }
            /// <summary> lance le dessin du plan impression. Normalement on pourrait appelr directement la procédure sous-jacente
            /// mais ne donne pas de bon résultats sur la 1ère miseà jour d'une numericupdown </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void PlanImpression_Paint(object sender, PaintEventArgs e)
            {
                if (!btnYES.Enabled)
                    return;
                var G = e.Graphics;
                // afficher les rectangles représeantant les pages à imprimer
                AfficherRectangles(G);
                // afficher la carte à imprimer
                var R = RegionCartePlan();
                G.FillRectangle(Brosse_Carte, R);
                G.DrawRectangle(Pen_Carte, R);
            }
            /// <summary> prend en compte le fait que l'utilisateur appui sur un des boutons de la souris pour débuter une action 
            /// qui dépend du type de rectangle et de leur utilisation </summary>
            private void PlanImpression_MouseDown(object sender, MouseEventArgs e)
            {
                if (!btnYES.Enabled)
                    return;
                // on enregistre la position de départ pour replacer le curseur en revenant de la boite de dialogue Image
                PosMouse = Cursor.Position;
                Modifier = ModifierKeys;
                // cherche si on est au début création de rectangle de travail ou au début click sur un rectangle simple 
                AppuyerBoutonSouris(e.Location, Modifier);
            }
            /// <summary> gestion du déplacement de la souris sur les rectangles </summary>
            private void PlanImpression_MouseMove(object sender, MouseEventArgs e)
            {
                if (!btnYES.Enabled)
                    return;
                // trouver le rectangle qui est survolé
                DeplacerSouris(e.Location);
                switch (NumRectangleSurvol)
                {
                    case -1: // pas de rectangle survolé
                        {
                            // on efface l'ancien rectangle survolé puisqu'il ne l'est plus
                            if (NumPage != -1)
                                PlanImpression.Invalidate();
                            break;
                        }
                    case 0: // rectangle de travail survolé
                        {
                            // on redessine toujours le rectangle de travail sur un évènement MouseMove (changement de dimensions)
                            PlanImpression.Invalidate();
                            break;
                        }
                    case var @case when @case > 0: // tuile survolée
                        {
                            if (NumPage != NumRectangleSurvol)
                            {
                                // on affiche à l'écran le nouveau rectangle simple survolé
                                PlanImpression.Invalidate();
                            }

                            break;
                        }
                }
                // on garde en mémoire le rectangle survolé
                NumPage = NumRectangleSurvol;
            }
            /// <summary> prend en compte le fait que l'utilisateur relache le bouton de la souris pour finir l'action en cours </summary>
            private void PlanImpression_MouseUp(object sender, MouseEventArgs e)
            {
                if (!btnYES.Enabled)
                    return;
                RelacherBoutonSouris();
                if (ClickRectangle)
                {
                    PlanImpression.Invalidate();
                    // replace le curseur à l'endroit de départ si la boite de dialogue Image a été appelée
                    if (Modifier == Keys.Alt && NumPage > 0)
                    {
                        Cursor.Position = PosMouse;
                    }
                }
            }
            /// <summary> calcule l'image de la page à imprimer et l'affiche </summary>
            private DialogResult AfficherImagePage(int NumPage)
            {
                // retrouve la colonne et le rang de la page à imprimer
                int YImpr = Math.DivRem(NumPage - 1, NbPagesX, out int XImpr);
                // facteur d'échelle pixels virtuel en Mm à imprimer sur l'axe des X
                float MmToPixelX = PixelGeoref.Width / DimensionsCarteImprimable.Width;
                RecouvreX = ReCouvre;
                PixelsRecouvreX = (int)Math.Round(RecouvreX * MmToPixelX, 0);
                PixelsLargeurImprimable = (int)Math.Round(LargeurPageImprimable * MmToPixelX, 0);
                PixelsCentrageXDeb = ChoixCentrage.Checked ? (int)Math.Round(CentrageImprXDeb * MmToPixelX, 0) : 0;
                // on calcule le nb de pixels nécessaire au centrage du fin de page
                PixelsCentrageXFin = NbPagesX * PixelsLargeurImprimable + PixelsRecouvreX - PixelGeoref.Width - PixelsCentrageXDeb;

                // puis les valeurs pixels sur l'axe des y en mettant à jour les valeurs Mm théorique.  
                // Normalement ce sont des pixels carrés (x=Y) sauf pour les cartes interpolées suisse
                // facteur d'échelle pixels virtuel en Mm à imprimer sur l'axe des Y
                float MmToPixelY = PixelGeoref.Height / DimensionsCarteImprimable.Height;
                RecouvreY = ReCouvre;
                PixelsRecouvreY = (int)Math.Round(RecouvreY * MmToPixelY, 0);
                PixelsHauteurImprimable = (int)Math.Round(HauteurPageImprimable * MmToPixelY, 0);
                PixelsCentrageYDeb = ChoixCentrage.Checked ? (int)Math.Round(CentrageImprYDeb * MmToPixelY, 0) : 0;
                // on calcule le nb de pixels nécessaire au centrage de fin de page
                PixelsCentrageYFin = NbPagesY * PixelsHauteurImprimable + PixelsRecouvreY - PixelGeoref.Height - PixelsCentrageYDeb;

                // valeur en pixels du décalage de la partie de l'image de la carte de la page demandée
                int PixelsX = XImpr == 0 ? 0 : PixelsLargeurImprimable * XImpr - PixelsCentrageXDeb;
                int PixelsY = YImpr == 0 ? 0 : PixelsHauteurImprimable * YImpr - PixelsCentrageYDeb;
                // valeur en pixels de la largeur et de la hauteur de la partie de l'image de la carte correspondant à la page demandée.
                int PixelsL = PixelsLargeurImprimable + PixelsRecouvreX - (XImpr == 0 ? PixelsCentrageXDeb : 0) - (XImpr == NbPagesX - 1 ? PixelsCentrageXFin : 0);
                int PixelsH = PixelsHauteurImprimable + PixelsRecouvreY - (YImpr == 0 ? PixelsCentrageYDeb : 0) - (YImpr == NbPagesY - 1 ? PixelsCentrageYFin : 0);
                // Découpage de l'image de la carte en fonction des dimensions calculées et passage à la boite de dialogue
                using (var S = BitMapCarte.Clone(new Rectangle(new Point(PixelsX, PixelsY), new Size(PixelsL, PixelsH)), BitMapCarte.PixelFormat))
                {
                    return AfficherImage(this, S, new Size(800, 800));
                }
            }
            /// <summary> trouve les variables associées à une imprssion en fonction des choix de l'utilisateur </summary>
            private void CalculerImpression()
            {
                InitialiserImpression();
                // calcul des différentes variables
                if (ChoixEchelle.Checked)
                {
                    EchelleToNbPages((float)ValeurEchelle.Value);
                }
                else
                {
                    NbPagesToEchelle((int)ValeurNbCol.Value, (int)ValeurNbRang.Value);
                }
                FinaliserImpression();
                // affichage des résultats sous forme de texte
                LabelResultats.Text = $"Nb Pages : {NbPagesX * NbPagesY} ({NbPagesX}*{NbPagesY}) en {(IsPaysage ? "Paysage" : "Portrait")}, Echelle : {EchelleImpression}";
                // et de plan
                CalculerPlanImpresion();
                PlanImpression.Invalidate();
            }
            /// <summary> Variables associées à la page de papier </summary>
            private void InitialiserImpression()
            {
                // trouver les dimensions du papier
                int PosCar = 2;
                string NomPapier = ListeChoixPapier.Text;
                string Largeur = TrouverChaineCompriseEntre(NomPapier, ref PosCar, '(', '*');
                string Hauteur = TrouverChaineCompriseEntre(NomPapier, ref PosCar, '*', ' ');
                DimensionsPapier = new SizeF(StrToSng(Largeur), StrToSng(Hauteur));
                Bords = (int)Math.Round(ValeurBordures.Value);
                ReCouvre = (int)Math.Round(ValeurRecouvrement.Value);
                IsPaysage = false;
                BordCoordonnees = IsCoordonnees ? MargeCoordonnees : 0;
                BordsXPageImprimable = Bords * 2 + ReCouvre;
                BordsYPageImprimable = Bords * 2 + ReCouvre + BordCoordonnees;
                // dimensions d'une page imprimable sans tenir compte de la marge pour les coordonnées car on ne connait pas encore l'orentiation du papier.
                DimensionsPageImprimable = new SizeF(DimensionsPapier.Width - BordsXPageImprimable, DimensionsPapier.Height - BordsXPageImprimable);
            }
            /// <summary> variables associées à la surface d'impression. Ne traite que des dimensions théoriques en Mm </summary>
            private void FinaliserImpression()
            {
                // échelle d'impression de la carte
                EchelleImpression = $"1/{MmGeoref.Height / DimensionsCarteImprimable.Height:N0}";
                // trouver les dimensions de la surface imprimable. La dernière page en X ou Y a un recouvrement qui ne compte pas
                DimensionsPagesImprimable = new SizeF(LargeurPageImprimable * NbPagesX + ReCouvre, HauteurPageImprimable * NbPagesY + ReCouvre);
                // les valeurs de centrage de la carte si demandé
                CentrageImprXDeb = (DimensionsPagesImprimable.Width - DimensionsCarteImprimable.Width) / 2f;
                CentrageImprXFin = CentrageImprXDeb;
                CentrageImprYDeb = (DimensionsPagesImprimable.Height - DimensionsCarteImprimable.Height) / 2f;
                CentrageImprYFin = CentrageImprYDeb;
                // Valeurs de l'imprimante liées à l'impression
                Papier = Papiers[ListeChoixPapier.SelectedIndex];
            }
            /// <summary> Affiche les résultats des calculs liés à l'impression sous forme de plan </summary>
            private void CalculerPlanImpresion()
            {
                float LargeurPages = DimensionsPagesImprimable.Width + BordsXPageImprimable;
                float HauteurPages = DimensionsPagesImprimable.Height + BordsYPageImprimable;
                // calcul du coef d'échelle Mm Imprimé en Pixel écran
                CoefR = TailleImpressionPlan.Width / LargeurPages;
                float R_Y = TailleImpressionPlan.Height / HauteurPages;
                // on prend le plus petit pour que cela tienne dans la surface de dessin allouée sans déformation
                if (CoefR > R_Y)
                    CoefR = R_Y;
                // transformation des dimensions Mm imprimé en pixel écran
                Marge = (int)Math.Round(Bords * CoefR);
                Coord = (int)Math.Round(BordCoordonnees * CoefR);
                int Recouv = (int)Math.Round(ReCouvre * CoefR);
                int LargP = (int)Math.Round((LargeurPageImprimable + BordsXPageImprimable) * CoefR);
                int DeltaX = LargP - (Recouv + Marge);
                int HautP = (int)Math.Round((HauteurPageImprimable + BordsYPageImprimable) * CoefR);
                int DeltaY = HautP - (Recouv + Marge + Coord);
                // calcule les dimensions de l'ensemble des pages à imprimer en pixels
                var TaillePagesPlan = new Size(DeltaX * NbPagesX + Recouv + Marge, DeltaY * NbPagesY + Coord + Recouv + Marge);
                // calcul de l'origine des pages à imprimer en pixels en centrant dans la zone de dessin
                int XPagesPlan = (TailleImpressionPlan.Width - TaillePagesPlan.Width) / 2 + AffichageBords;
                int YPagesPlan = (TailleImpressionPlan.Height - TaillePagesPlan.Height) / 2 + AffichageBords;
                RegionPagesPlan = new Rectangle(new Point(XPagesPlan, YPagesPlan), TaillePagesPlan);
                // dimensions de la carte à imprimer en rapport avec la taille des pages en pixels pour éviter les erreurs d'arrondi sur la représentation de la carte
                float PX = DimensionsCarteImprimable.Width / LargeurPages;
                float PY = DimensionsCarteImprimable.Height / HauteurPages;
                TailleCartePlan = new Size((int)Math.Round(TaillePagesPlan.Width * PX), (int)Math.Round(TaillePagesPlan.Height * PY));
                // initialisation et remplissage de la collection des pages à imprimer
                InitialiserRectangles(SurfaceRectangleTravail, Mode.Selection, Rectangle.Empty, true);
                int Cpt = 1;
                int DY = YPagesPlan;
                for (int PageY = 0, loopTo = NbPagesY - 1; PageY <= loopTo; PageY++)
                {
                    int DX = XPagesPlan;
                    for (int PageX = 0, loopTo1 = NbPagesX - 1; PageX <= loopTo1; PageX++)
                    {
                        string Name = $"Page {Cpt}";
                        AjouterRectangle(new Point(DX, DY), new Point(DX + LargP, DY + HautP), Name, true);
                        DX += DeltaX;
                        Cpt += 1;
                    }
                    DY += DeltaY;
                }
            }
            /// <summary> renvoi le rectangle occupé par la carte sur le plan de mapping en fonction du choix de centrage </summary>
            private Rectangle RegionCartePlan()
            {
                Rectangle R;
                if (IsCentrage)
                {
                    int DeltaX = (RegionPagesPlan.Width - TailleCartePlan.Width) / 2;
                    if (DeltaX < Marge)
                        DeltaX = Marge;
                    int DeltaY = Coord + (RegionPagesPlan.Height - Coord - TailleCartePlan.Height) / 2;
                    if (DeltaY < Marge)
                        DeltaY = Marge;
                    R = new Rectangle(new Point(RegionPagesPlan.X + DeltaX, RegionPagesPlan.Y + DeltaY), TailleCartePlan);
                }
                else
                {
                    var Pt = Point.Add(RegionPagesPlan.Location, new Size(Marge, Marge + Coord));
                    R = new Rectangle(Pt, TailleCartePlan);
                }
                return R;
            }
            /// <summary> calcule le nb de pages minimum en X, Y pour imprimer la carte avec l'échelle demandée </summary>
            /// <param name="EchelleImpr"> échelle demandée </param>
            private void EchelleToNbPages(float EchelleImpr)
            {
                // dimensions de la carte en Mm une fois imprimée pour respecter l'échelle demandée
                DimensionsCarteImprimable = new SizeF(MmGeoref.Width / EchelleImpr, MmGeoref.Height / EchelleImpr);
                // La surface imprimable doit être => à la surface imprimée de la carte. On teste l'orientation portrait et paysage et on prend celle qui a le moins de pages
                NbPagesX = (int)Math.Ceiling((DimensionsCarteImprimable.Width - ReCouvre) / DimensionsPageImprimable.Width);
                NbPagesY = (int)Math.Ceiling((DimensionsCarteImprimable.Height - ReCouvre) / (DimensionsPageImprimable.Height - BordCoordonnees));
                NbPagesTotal = NbPagesX * NbPagesY;
                // si l'imprimante accepte le mode paysage il y a une 2ème possibilité
                if (IsSupportPaysage)
                {
                    int NbPagesXH = (int)Math.Ceiling((DimensionsCarteImprimable.Width - ReCouvre) / DimensionsPageImprimable.Height);
                    int NbPagesYH = (int)Math.Ceiling((DimensionsCarteImprimable.Height - ReCouvre) / (DimensionsPageImprimable.Width - BordCoordonnees));
                    if (NbPagesTotal > NbPagesXH * NbPagesYH)
                    {
                        NbPagesX = NbPagesXH;
                        NbPagesY = NbPagesYH;
                        IsPaysage = true;
                    }
                }
                // on prend en compte l'orientation du papier pour trouver les dimensions réelles des pages imprimables
                LargeurPageImprimable = IsPaysage ? DimensionsPageImprimable.Height : DimensionsPageImprimable.Width;
                HauteurPageImprimable = (IsPaysage ? DimensionsPageImprimable.Width : DimensionsPageImprimable.Height) - BordCoordonnees;
            }
            /// <summary> calcule la plus grande échelle d'impression de la carte avec le nb de pages demandé. </summary>
            /// <param name="NbCol"> nb de colonnes ou axe des X </param>
            /// <param name="NbRang"> nb de rangées ou axe des Y </param>
            private void NbPagesToEchelle(int NbCol, int NbRang)
            {
                double SurfaceCalcul, SurfaceCarte = default;
                (OrientationPapier SensPapier, OrientionCarte SensCarte) OrientationsCalcul, OrientationsCarte = default;
                // il y a 2 possibilités par orientation de la carte. il n'y a qu'une seule orientation de carte si nbCol=nbRang
                for (int Possible = 0, loopTo = NbCol == NbRang ? 1 : 3; Possible <= loopTo; Possible++)
                {
                    OrientationsCalcul = ((OrientationPapier)(Possible % 2), (OrientionCarte)(Possible / 2));
                    // si l'imprimante accepte le mode paysage il y a une 2ème possibilité pour une orientation de carte
                    if (OrientationsCalcul.SensPapier == OrientationPapier.Portrait || IsSupportPaysage)
                    {
                        SurfaceCalcul = (double)CalculerDimensionsCarteImpression(OrientationsCalcul, NbCol, NbRang);
                        if (SurfaceCalcul > SurfaceCarte)
                        {
                            OrientationsCarte = OrientationsCalcul;
                            SurfaceCarte = SurfaceCalcul;
                        }
                    }
                }
                // on prend en compte l'orientation du papier et de la carte qui rend l'échelle la plus grande
                CalculerDimensionsCarteImpression(OrientationsCarte, NbCol, NbRang);
                // On Vérifie que le nb de pages demandé pour l'impression correspond au besoin de l'impression, cela supprime les pages vides 
                int DeltaX = (int)Math.Floor((LargeurPageImprimable * NbPagesX + ReCouvre - DimensionsCarteImprimable.Width) / LargeurPageImprimable);
                NbPagesX -= DeltaX;
                int DeltaY = (int)Math.Floor((HauteurPageImprimable * NbPagesY + ReCouvre - DimensionsCarteImprimable.Height) / HauteurPageImprimable);
                NbPagesY -= DeltaY;
            }
            /// <summary> détermine les variables et calcule les dimensions de la carte imprimée </summary>
            /// <param name="OrientationCarte">SensPapier = Portrait ou Paysage et SensCarte = Horizontal ou Vertical </param>
            /// <param name="NbCol"> Nb de Colonnes </param>
            /// <param name="NbRang"> Nb de Rangées</param>
            private float CalculerDimensionsCarteImpression((OrientationPapier SensPapier, OrientionCarte SensCarte) OrientationCarte, int NbCol, int NbRang)
            {
                IsPaysage = OrientationCarte.SensPapier == OrientationPapier.Paysage;
                NbPagesX = OrientationCarte.SensCarte == OrientionCarte.Horizontal ? NbRang : NbCol;
                NbPagesY = OrientationCarte.SensCarte == OrientionCarte.Horizontal ? NbCol : NbRang;
                LargeurPageImprimable = IsPaysage ? DimensionsPageImprimable.Height : DimensionsPageImprimable.Width;
                HauteurPageImprimable = (IsPaysage ? DimensionsPageImprimable.Width : DimensionsPageImprimable.Height) - BordCoordonnees;
                DimensionsPagesImprimable = new SizeF(LargeurPageImprimable * NbPagesX + ReCouvre, HauteurPageImprimable * NbPagesY + ReCouvre);
                float RapportI = DimensionsPagesImprimable.Width / RapCarteMm;
                if (RapportI > DimensionsPagesImprimable.Height)
                {
                    // on s'assure ainsi que DimensionsCarteImpression.Hauteur=DimensionsImprimable.Hauteur sinon il peut avoir des erreurs d'arrondi
                    DimensionsCarteImprimable = new SizeF(DimensionsPagesImprimable.Height * RapCarteMm, DimensionsPagesImprimable.Height);
                }
                else
                {
                    // on s'assure ainsi que DimensionsCarteImpression.Largeur=DimensionsImprimable.Largeur  sinon il peut avoir des erreurs d'arrondi
                    DimensionsCarteImprimable = new SizeF(DimensionsPagesImprimable.Width, RapportI);
                }
                return DimensionsCarteImprimable.Width * DimensionsCarteImprimable.Height;
            }
            /// <summary> Trouve les valeurs par défaut associées à l'imprimante chosie par l'utilisateur </summary>
            private void ChoisirImprimante()
            {
                var Imprimante = new PrinterSettings();
                string Nom = ListeChoixImprimante.Text;
                Imprimante.PrinterName = Nom;
                // on cherche les bords matériels minimum de l'imprimante
                BordsMini = (int)Math.Round(Imprimante.DefaultPageSettings.HardMarginX * CinchToMm, 0);
                BordsMini = Math.Max(BordsMini, (int)Math.Round(Imprimante.DefaultPageSettings.HardMarginY * CinchToMm, 0));
                // et on l'indique dans le formulaire
                ValeurBordures.Minimum = BordsMini;
                var PapierImprimante = Imprimante.DefaultPageSettings.PaperSize;
                bool FlagAdd;
                string KindNom = "";
                int IndexPapier = -1;
                Papiers = new List<PaperSize>();
                PapiersNoms = new List<string>();
                ListeChoixPapier.Items.Clear();
                foreach (PaperSize Pap in Imprimante.PaperSizes)
                {
                    FlagAdd = true;
                    switch (Pap.Kind)
                    {
                        case PaperKind.A4:
                            {
                                KindNom = "A4 (210*297 mm)";
                                break;
                            }
                        case PaperKind.A3:
                            {
                                KindNom = "A3 (297*420 mm)";
                                break;
                            }
                        case PaperKind.Letter:
                            {
                                KindNom = "Lettre US (215.9*279.4 mm)";
                                break;
                            }
                        case PaperKind.Ledger:
                            {
                                KindNom = "Ledger US (431.8*279.4 mm)";
                                break;
                            }
                        case PaperKind.Legal:
                            {
                                KindNom = "Legal US (215.9*355.6 mm)";
                                break;
                            }
                        case PaperKind.Tabloid:
                            {
                                KindNom = "Tabloïd (279.4*431.8 mm)";
                                break;
                            }

                        default:
                            {
                                FlagAdd = false;
                                break;
                            }
                    }
                    if (FlagAdd)
                    {
                        if (Pap.Kind == PapierImprimante.Kind)
                            IndexPapier = Papiers.Count;
                        Papiers.Add(Pap);
                        PapiersNoms.Add(KindNom);
                    }
                }
                ListeChoixPapier.Items.AddRange(PapiersNoms.ToArray());
                ListeChoixPapier.SelectedIndex = IndexPapier;

                var ResolutionImprimante = Imprimante.DefaultPageSettings.PrinterResolution;
                int IndexResolution = -1;
                Resolutions = new List<PrinterResolution>();
                ResolutionsNoms = new List<string>();
                ListeChoixResolution.Items.Clear();
                foreach (PrinterResolution Reso in Imprimante.PrinterResolutions)
                {
                    FlagAdd = true;
                    switch (Reso.Kind)
                    {
                        case PrinterResolutionKind.Draft:
                            {
                                KindNom = "Qualité Brouillon";
                                break;
                            }
                        case PrinterResolutionKind.Low:
                            {
                                KindNom = "Qualité Basse (Texte)";
                                break;
                            }
                        case PrinterResolutionKind.Medium:
                            {
                                KindNom = "Qualité Moyenne (Texte & Photos)";
                                break;
                            }
                        case PrinterResolutionKind.High:
                            {
                                KindNom = "Qualité Haute (Photos)";
                                break;
                            }
                        case PrinterResolutionKind.Custom:
                            {
                                KindNom = $"Qualité Personnalisée ({Reso.X}*{Reso.Y})";
                                break;
                            }

                        default:
                            {
                                FlagAdd = false;
                                break;
                            }
                    }
                    if (FlagAdd)
                    {
                        if (Reso.Kind == ResolutionImprimante.Kind)
                            IndexResolution = Resolutions.Count;
                        Resolutions.Add(Reso);
                        ResolutionsNoms.Add(KindNom);
                    }
                }
                if (IndexResolution == -1)
                {
                    IndexResolution = Resolutions.Count;
                    Resolutions.Add(ResolutionImprimante);
                    ResolutionsNoms.Add($"Qualité Personnalisée ({ResolutionImprimante.X}*{ResolutionImprimante.Y})");
                }
                ListeChoixResolution.Items.AddRange(ResolutionsNoms.ToArray());
                ListeChoixResolution.SelectedIndex = IndexResolution;

                if (Imprimante.SupportsColor)
                {
                    IsSupportColor = true;
                    ListeChoixCouleur.SelectedIndex = Imprimante.DefaultPageSettings.Color ? 0 : 1;
                    ListeChoixCouleur.Enabled = true;
                }
                else
                {
                    IsSupportColor = false;
                    ListeChoixCouleur.SelectedIndex = 1;
                    ListeChoixCouleur.Enabled = false;
                }
                IsSupportPaysage = Imprimante.LandscapeAngle != 0;
                IsRectoVerso = Imprimante.CanDuplex;
            }
            /// <summary> calcul le Pt0 de la carte à imprimer en pixels </summary>
            private void TrouverPt0Pixels()
            {
                string[] LignesFichierGeoref = File.ReadAllLines(OuvrirFichierGeoref.FileName, Encoding_FCGP);
                int PosCar = LignesFichierGeoref[0].IndexOf('.') + 1;
                // Trouve le système de capture de la carte
                string SiteLibelle = TrouverChaineCompriseEntre(LignesFichierGeoref[0], ref PosCar, ':');
                // Trouve l'échelle de capture de la carte sur la deuxième ligne du fichier georef
                PosCar = 0;
                string ClefEchelle = TrouverChaineCompriseEntre(LignesFichierGeoref[1], ref PosCar, ':', ',');
                // Trouve le type de coordonnées capturées de la carte sur la deuxième ligne du fichier georef
                PosCar = LignesFichierGeoref[1].IndexOf(", C");
                string LibelleProjection = TrouverChaineCompriseEntre(LignesFichierGeoref[1], ref PosCar, ':');

                PosCar = LignesFichierGeoref[3].IndexOf(':') + 1;
                string X = TrouverChaineCompriseEntre(LignesFichierGeoref[3], ref PosCar, ':', ',');
                int Bid = LignesFichierGeoref[3].IndexOf(':', PosCar);
                string Y;
                if (Bid == -1)
                {
                    Y = TrouverChaineCompriseEntre(LignesFichierGeoref[3], ref PosCar, ',');
                }
                else
                {
                    int argPosCar1 = PosCar + 1;
                    Y = TrouverChaineCompriseEntre(LignesFichierGeoref[3], ref argPosCar1, ':');
                }
                var Pt0Grille = new PointD(StrToDbl(X), StrToDbl(Y));

                int Z = 0;
                var H = default(char);
                var SystemeCartographiqueCarte = new SystemeCartographique(SiteLibelle, ClefEchelle, LibelleProjection, Z, H);
                if (SystemeCartographiqueCarte.SiteCarto == SitesCartographiques.SuisseMobile)
                {
                    ProjectionSuisse.LV95ToLV03(ref Pt0Grille);
                }

                SiteCartoCarte = SystemeCartographiqueCarte.SiteCarto;
                EchelleCarte = SystemeCartographiqueCarte.Niveau.Echelle;
                Pt0Pixels = PointGrilleToPointPixel(Pt0Grille, SiteCartoCarte, EchelleCarte);
            }
            /// <summary> Trouve toutes les variables de la carte associées au fichier Georef </summary>
            /// <param name="NomFichierGeoref"> Chemin complet du fichier Georef </param>
            private VerificationRenvoiCarte LireFichierGeoref()
            {
                CheminFichierGeoref = Path.GetDirectoryName(OuvrirFichierGeoref.FileName);
                OuvrirFichierGeoref.InitialDirectory = CheminFichierGeoref;
                string[] LignesFichierGeoref = File.ReadAllLines(OuvrirFichierGeoref.FileName, Encoding_FCGP);
                var PosCar = default(int);
                // Trouve le nom du fichier de l'image associée à la carte sur la 1ère ligne du fichier georef
                string NomFichierCapture = TrouverChaineCompriseEntre(LignesFichierGeoref[0], ref PosCar, ':', ',');
                var Di = new DirectoryInfo(CheminFichierGeoref);
                // on cherche si la carte a un fichier associé avec les références des coordonnées
                NomCarte = $"{Path.GetFileNameWithoutExtension(NomFichierCapture)}_R";
                FileInfo[] Fichiers = Di.GetFiles(NomCarte + ".*");
                if (Fichiers.Length == 0)
                {
                    // puis le fichier image associé au fichier georef
                    Fichiers = Di.GetFiles(NomFichierCapture);
                    if (Fichiers.Length == 0)
                        return VerificationRenvoiCarte.Carte_Inexistante;
                    NomCarte = NomFichierCapture;
                }
                else
                {
                    NomCarte = Fichiers[0].Name;
                }
                // recherche de l'échelle d'impression
                PosCar = LignesFichierGeoref[1].IndexOf(',');
                string EchImpr = TrouverChaineCompriseEntre(LignesFichierGeoref[1], ref PosCar, ':', ',');
                // rendu nécessaire pour les fichiers georef antérieur à 8.600
                if (string.IsNullOrEmpty(EchImpr))
                    EchImpr = "1000";
                EchelleImpressionGeoref = StrToSng(EchImpr) * 1000f;
                ValeurEchelle.Value = (decimal)EchelleImpressionGeoref;
                PosCar = LignesFichierGeoref[2].IndexOf("DPI");
                float InchtoMm = (float)InchToMm;
                float DPI_X = StrToSng(TrouverChaineCompriseEntre(LignesFichierGeoref[2], ref PosCar, ':', ',')) / InchtoMm;
                float DPI_Y = StrToSng(TrouverChaineCompriseEntre(LignesFichierGeoref[2], ref PosCar, ':')) / InchtoMm;
                PosCar = 10;
                PixelGeoref.Width = int.Parse(TrouverChaineCompriseEntre(LignesFichierGeoref[11], ref PosCar, ':', ','));
                PixelGeoref.Height = int.Parse(TrouverChaineCompriseEntre(LignesFichierGeoref[11], ref PosCar, ':'));
                MmGeoref = new SizeF(PixelGeoref.Width * EchelleImpressionGeoref / DPI_X, PixelGeoref.Height * EchelleImpressionGeoref / DPI_Y);
                RapCarteMm = MmGeoref.Width / MmGeoref.Height;
                return VerificationRenvoiCarte.OK;
            }
        }
        private partial class ChoixImpressionCarte : Form
        {
            internal ChoixImpressionCarte()
            {
                // Cet appel est requis par le concepteur.
                InitializeComponent();
                InitialiserEvenements();
            }
            /// <summary> Initialise l'ensemble des évenement liés au formulaire car aucune variable avec le modificateur WithEvents 
            /// rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
            private void InitialiserEvenements()
            {
                ValeurBordures.ValueChanged += ValeurMarges_ValueChanged;
                ValeurRecouvrement.ValueChanged += ValeurMarges_ValueChanged;

                ChoixCentrage.CheckedChanged += ChoixCentrage_CheckedChanged;

                ChoixCoordonnees.CheckedChanged += ChoixCoordonnees_CheckedChanged;

                ChoixNbPages.CheckedChanged += ValeurNbPages_ValueChanged;
                ValeurNbCol.ValueChanged += ValeurNbPages_ValueChanged;
                ValeurNbRang.ValueChanged += ValeurNbPages_ValueChanged;

                ChoixEchelle.CheckedChanged += ValeurEchelle_ValueChanged;
                ValeurEchelle.ValueChanged += ValeurEchelle_ValueChanged;

                LabelChoixFichierGeoref.Click += LabelChoixFichierGeoref_Click;
                PlanImpression.Paint += PlanImpression_Paint;
                PlanImpression.MouseDown += PlanImpression_MouseDown;
                PlanImpression.MouseMove += PlanImpression_MouseMove;
                PlanImpression.MouseUp += PlanImpression_MouseUp;
                #region Listes et label associés
                LabelChoixImprimante.Click += Labels_Click;
                LabelChoixPapier.Click += Labels_Click;
                LabelChoixResolution.Click += Labels_Click;
                LabelChoixCouleur.Click += Labels_Click;

                ListeChoixImprimante.SelectedIndexChanged += Listes_SelectedIndexChanged;
                ListeChoixPapier.SelectedIndexChanged += Listes_SelectedIndexChanged;
                ListeChoixResolution.SelectedIndexChanged += Listes_SelectedIndexChanged;
                ListeChoixCouleur.SelectedIndexChanged += Listes_SelectedIndexChanged;
                #endregion
                #region Formulaire
                Load += ChoixImpressionCarte_Load;
                FormClosing += ChoixImpressionCarte_FormClosing;
                FormClosed += ChoixImpressionCarte_FormClosed;
                #endregion
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
            // <System.Diagnostics.DebuggerStepThrough()> _
            private void InitializeComponent()
            {
                components = new Container();
                PanelChoix = new Panel();
                ChoixCentrage = new CheckBox();
                LabelCentrage = new Label();
                LabelImprimante = new Label();
                LabelChoixImprimante = new Label();
                ListeChoixImprimante = new ComboBox();
                LabelChoixPapier = new Label();
                ListeChoixPapier = new ComboBox();
                LabelChoixResolution = new Label();
                ListeChoixResolution = new ComboBox();
                LabelChoixCouleur = new Label();
                ListeChoixCouleur = new ComboBox();
                LabelFichierGeoref = new Label();
                LabelChoixFichierGeoref = new Label();
                LabelImpression = new Label();
                LabelBordures = new Label();
                ChoixNbPages = new RadioButton();
                LabelNbPages = new Label();
                LabelRecouvrement = new Label();
                LabelNbCol = new Label();
                LabelNbRang = new Label();
                ChoixEchelle = new RadioButton();
                LabelEchelle = new Label();
                LabelCoordonnees = new Label();
                ChoixCoordonnees = new CheckBox();
                ValeurBordures = new NumericUpDown();
                ValeurRecouvrement = new NumericUpDown();
                ValeurNbCol = new NumericUpDown();
                ValeurNbRang = new NumericUpDown();
                ValeurEchelle = new NumericUpDown();
                LabelPlanImpression = new Label();
                PlanImpression = new Label();
                LabelResultats = new Label();
                OuvrirFichierGeoref = new OpenFileDialog();
                ToolTip1 = new ToolTip(components);
                btnNO = new Button();
                btnYES = new Button();
                PanelChoix.SuspendLayout();
                ((ISupportInitialize)ValeurBordures).BeginInit();
                ((ISupportInitialize)ValeurRecouvrement).BeginInit();
                ((ISupportInitialize)ValeurNbCol).BeginInit();
                ((ISupportInitialize)ValeurNbRang).BeginInit();
                ((ISupportInitialize)ValeurEchelle).BeginInit();
                SuspendLayout();
                // 
                // LabelImprimante
                // 
                LabelImprimante.BorderStyle = BorderStyle.FixedSingle;
                LabelImprimante.Location = new Point(6, 6);
                LabelImprimante.Name = "LabelImprimante";
                LabelImprimante.Size = new Size(353, 27);
                LabelImprimante.TabIndex = 14;
                LabelImprimante.Text = "Choix concernant l'imprimante";
                LabelImprimante.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelChoixImprimante
                // 
                LabelChoixImprimante.BackColor = Color.White;
                LabelChoixImprimante.BorderStyle = BorderStyle.FixedSingle;
                LabelChoixImprimante.Location = new Point(6, 32);
                LabelChoixImprimante.Name = "LabelChoixImprimante";
                LabelChoixImprimante.Size = new Size(353, 23);
                LabelChoixImprimante.TabIndex = 3;
                LabelChoixImprimante.Tag = ListeChoixImprimante;
                ToolTip1.SetToolTip(LabelChoixImprimante, "Cliquez pour choisir une imprimante dans la liste");
                // 
                // ListeChoixImprimante
                // 
                ListeChoixImprimante.BackColor = Color.White;
                ListeChoixImprimante.FormattingEnabled = true;
                ListeChoixImprimante.Location = new Point(6, 27);
                ListeChoixImprimante.Name = "ListeChoixImprimante";
                ListeChoixImprimante.Size = new Size(353, 27);
                ListeChoixImprimante.TabIndex = 2;
                ListeChoixImprimante.Tag = LabelChoixImprimante;
                ListeChoixImprimante.Visible = false;
                // 
                // LabelChoixPapier
                // 
                LabelChoixPapier.BackColor = Color.White;
                LabelChoixPapier.BorderStyle = BorderStyle.FixedSingle;
                LabelChoixPapier.Location = new Point(6, 54);
                LabelChoixPapier.Name = "LabelChoixPapier";
                LabelChoixPapier.Size = new Size(353, 23);
                LabelChoixPapier.TabIndex = 4;
                LabelChoixPapier.Tag = ListeChoixPapier;
                ToolTip1.SetToolTip(LabelChoixPapier, "Cliquez choisir le format de papier" + CrLf +
                                                      "parmi ceux acceptés par l'imprimante.");
                // 
                // ListeChoixPapier
                // 
                ListeChoixPapier.FormattingEnabled = true;
                ListeChoixPapier.Location = new Point(6, 49);
                ListeChoixPapier.Name = "ListeChoixPapier";
                ListeChoixPapier.Size = new Size(353, 27);
                ListeChoixPapier.TabIndex = 3;
                ListeChoixPapier.Tag = LabelChoixPapier;
                ListeChoixPapier.Visible = false;
                // 
                // LabelChoixResolution
                // 
                LabelChoixResolution.BackColor = Color.White;
                LabelChoixResolution.BorderStyle = BorderStyle.FixedSingle;
                LabelChoixResolution.Location = new Point(6, 76);
                LabelChoixResolution.Name = "LabelChoixResolution";
                LabelChoixResolution.Size = new Size(243, 23);
                LabelChoixResolution.TabIndex = 5;
                LabelChoixResolution.Tag = ListeChoixResolution;
                ToolTip1.SetToolTip(LabelChoixResolution, "Cliquez pour choisir la qualité d'impression");
                // 
                // ListeChoixResolution
                // 
                ListeChoixResolution.FormattingEnabled = true;
                ListeChoixResolution.Location = new Point(6, 71);
                ListeChoixResolution.Name = "ListeChoixResolution";
                ListeChoixResolution.Size = new Size(243, 27);
                ListeChoixResolution.TabIndex = 4;
                ListeChoixResolution.Tag = LabelChoixResolution;
                ListeChoixResolution.Visible = false;
                // 
                // LabelChoixCouleur
                // 
                LabelChoixCouleur.BackColor = Color.White;
                LabelChoixCouleur.BorderStyle = BorderStyle.FixedSingle;
                LabelChoixCouleur.Location = new Point(248, 76);
                LabelChoixCouleur.Name = "LabelChoixCouleur";
                LabelChoixCouleur.Size = new Size(111, 23);
                LabelChoixCouleur.TabIndex = 6;
                LabelChoixCouleur.Tag = ListeChoixCouleur;
                ToolTip1.SetToolTip(LabelChoixCouleur, "Cliquez pour choisir entre " + CrLf +
                                                       "couleur et niveau de gris");
                // 
                // ListeChoixCouleur
                // 
                ListeChoixCouleur.FormattingEnabled = true;
                ListeChoixCouleur.Items.AddRange(new object[] { "Couleur", "Niveau de gris" });
                ListeChoixCouleur.Location = new Point(248, 71);
                ListeChoixCouleur.Name = "ListeChoixCouleur";
                ListeChoixCouleur.Size = new Size(111, 27);
                ListeChoixCouleur.TabIndex = 5;
                ListeChoixCouleur.Tag = LabelChoixCouleur;
                ListeChoixCouleur.Visible = false;
                // 
                // LabelFichierGeoref
                // 
                LabelFichierGeoref.BorderStyle = BorderStyle.FixedSingle;
                LabelFichierGeoref.Location = new Point(6, 105);
                LabelFichierGeoref.Name = "LabelFichierGeoref";
                LabelFichierGeoref.Size = new Size(353, 27);
                LabelFichierGeoref.TabIndex = 19;
                LabelFichierGeoref.Text = "Fichier Georef de la carte";
                LabelFichierGeoref.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelChoixFichierGeoref
                // 
                LabelChoixFichierGeoref.BackColor = Color.White;
                LabelChoixFichierGeoref.BorderStyle = BorderStyle.FixedSingle;
                LabelChoixFichierGeoref.Location = new Point(6, 131);
                LabelChoixFichierGeoref.Name = "LabelChoixFichierGeoref";
                LabelChoixFichierGeoref.Size = new Size(353, 23);
                LabelChoixFichierGeoref.TabIndex = 7;
                LabelChoixFichierGeoref.TextAlign = ContentAlignment.MiddleLeft;
                ToolTip1.SetToolTip(LabelChoixFichierGeoref, "Cliquez pour sélectionner une carte (fichier Georef associé)");
                // 
                // LabelImpression
                // 
                LabelImpression.BorderStyle = BorderStyle.FixedSingle;
                LabelImpression.Location = new Point(6, 160);
                LabelImpression.Name = "LabelImpression";
                LabelImpression.Size = new Size(353, 27);
                LabelImpression.TabIndex = 20;
                LabelImpression.Text = "Choix concernant l'impression";
                LabelImpression.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelCentrage
                // 
                LabelCentrage.BorderStyle = BorderStyle.FixedSingle;
                LabelCentrage.Location = new Point(6, 186);
                LabelCentrage.Name = "LabelCentrage";
                LabelCentrage.Size = new Size(117, 26);
                LabelCentrage.TabIndex = 34;
                // 
                // ChoixCentrage
                // 
                ChoixCentrage.CheckAlign = ContentAlignment.MiddleRight;
                ChoixCentrage.Checked = true;
                ChoixCentrage.CheckState = CheckState.Checked;
                ChoixCentrage.Location = new Point(9, 187);
                ChoixCentrage.Name = "ChoixCentrage";
                ChoixCentrage.Size = new Size(105, 24);
                ChoixCentrage.TabIndex = 35;
                ChoixCentrage.Text = "Centrage";
                ChoixCentrage.TextAlign = ContentAlignment.TopLeft;
                ToolTip1.SetToolTip(ChoixCentrage, "Centre la carte sur la surface imprimable");
                ChoixCentrage.UseVisualStyleBackColor = true;
                // 
                // LabelBordures
                // 
                LabelBordures.BorderStyle = BorderStyle.FixedSingle;
                LabelBordures.Location = new Point(122, 186);
                LabelBordures.Name = "LabelBordures";
                LabelBordures.Size = new Size(80, 26);
                LabelBordures.TabIndex = 7;
                LabelBordures.Text = "Bordures";
                // 
                // ValeurBordures
                // 
                ValeurBordures.BorderStyle = BorderStyle.FixedSingle;
                ValeurBordures.Location = new Point(201, 186);
                ValeurBordures.Maximum = 10m;
                ValeurBordures.Name = "ValeurBordures";
                ValeurBordures.Size = new Size(40, 26);
                ValeurBordures.TabIndex = 8;
                ToolTip1.SetToolTip(ValeurBordures, "Définit en Mm la largeur des bords" + CrLf +
                                                    "de la page qui n'est pas imprimée");
                ValeurBordures.Value = 5m;
                // 
                // LabelRecouvrement
                // 
                LabelRecouvrement.BorderStyle = BorderStyle.FixedSingle;
                LabelRecouvrement.Location = new Point(240, 186);
                LabelRecouvrement.Name = "LabelRecouvrement";
                LabelRecouvrement.Size = new Size(80, 26);
                LabelRecouvrement.TabIndex = 9;
                LabelRecouvrement.Text = "Recouvre";
                // 
                // ValeurRecouvrement
                // 
                ValeurRecouvrement.BorderStyle = BorderStyle.FixedSingle;
                ValeurRecouvrement.Location = new Point(319, 186);
                ValeurRecouvrement.Maximum = 10m;
                ValeurRecouvrement.Name = "ValeurRecouvrement";
                ValeurRecouvrement.Size = new Size(40, 26);
                ValeurRecouvrement.TabIndex = 9;
                ToolTip1.SetToolTip(ValeurRecouvrement, "Définit en Mm le recouvrement ou " + CrLf +
                                                        "chevauchement entre 2 pages");
                // 
                // LabelNbPages
                // 
                LabelNbPages.BorderStyle = BorderStyle.FixedSingle;
                LabelNbPages.Location = new Point(6, 211);
                LabelNbPages.Name = "LabelNbPages";
                LabelNbPages.Size = new Size(117, 26);
                LabelNbPages.TabIndex = 21;
                // 
                // ChoixNbPages
                // 
                ChoixNbPages.CheckAlign = ContentAlignment.MiddleRight;
                ChoixNbPages.Checked = true;
                ChoixNbPages.Location = new Point(9, 212);
                ChoixNbPages.Name = "ChoixNbPages";
                ChoixNbPages.Size = new Size(104, 24);
                ChoixNbPages.TabIndex = 10;
                ChoixNbPages.TabStop = true;
                ChoixNbPages.Text = "Nb de Pages";
                ToolTip1.SetToolTip(ChoixNbPages, "Le nombre de pages demandé définit" + CrLf +
                                                  "l'échelle maximale d'impression");
                ChoixNbPages.UseVisualStyleBackColor = true;
                // 
                // LabelNbCol
                // 
                LabelNbCol.BorderStyle = BorderStyle.FixedSingle;
                LabelNbCol.Location = new Point(122, 211);
                LabelNbCol.Name = "LabelNbCol";
                LabelNbCol.Size = new Size(80, 26);
                LabelNbCol.TabIndex = 28;
                LabelNbCol.Text = "Nb Cols";
                // 
                // ValeurNbCol
                // 
                ValeurNbCol.Location = new Point(201, 211);
                ValeurNbCol.Maximum = 25m;
                ValeurNbCol.Minimum = 1m;
                ValeurNbCol.Name = "ValeurNbCol";
                ValeurNbCol.Size = new Size(40, 26);
                ValeurNbCol.TabIndex = 11;
                ToolTip1.SetToolTip(ValeurNbCol, "Nombre de colonnes." + CrLf +
                                                 "Peut être inversée avec le nombre de rangées" + CrLf +
                                                 "ou diminué si il y a des pages qui ne sont imprimées");
                ValeurNbCol.Value = 1m;
                // 
                // LabelNbRang
                // 
                LabelNbRang.BorderStyle = BorderStyle.FixedSingle;
                LabelNbRang.Location = new Point(240, 211);
                LabelNbRang.Name = "LabelNbRang";
                LabelNbRang.Size = new Size(80, 26);
                LabelNbRang.TabIndex = 30;
                LabelNbRang.Text = "Nb Rangs";
                // 
                // ValeurNbRang
                // 
                ValeurNbRang.Location = new Point(319, 211);
                ValeurNbRang.Maximum = 25m;
                ValeurNbRang.Minimum = 1m;
                ValeurNbRang.Name = "ValeurNbRang";
                ValeurNbRang.Size = new Size(40, 26);
                ValeurNbRang.TabIndex = 12;
                ToolTip1.SetToolTip(ValeurNbRang, "Nombre de rangées." + CrLf +
                                                  "Peut être inversé avec le nombre de colonnes " + CrLf +
                                                  "ou diminué si il y a des pages qui ne sont imprimées");
                ValeurNbRang.Value = 1m;
                // 
                // LabelEchelle
                // 
                LabelEchelle.BorderStyle = BorderStyle.FixedSingle;
                LabelEchelle.Location = new Point(6, 236);
                LabelEchelle.Name = "LabelEchelle";
                LabelEchelle.Size = new Size(117, 26);
                LabelEchelle.TabIndex = 31;
                // 
                // ChoixEchelle
                // 
                ChoixEchelle.CheckAlign = ContentAlignment.MiddleRight;
                ChoixEchelle.Location = new Point(9, 237);
                ChoixEchelle.Name = "ChoixEchelle";
                ChoixEchelle.Size = new Size(104, 24);
                ChoixEchelle.TabIndex = 13;
                ChoixEchelle.TabStop = true;
                ChoixEchelle.Text = "Echelle";
                ToolTip1.SetToolTip(ChoixEchelle, "L'échelle demandée définit" + CrLf +
                                                  "le nombre minimal de pages");
                ChoixEchelle.UseVisualStyleBackColor = true;
                // 
                // ValeurEchelle
                // 
                ValeurEchelle.Increment = 500m;
                ValeurEchelle.Location = new Point(122, 236);
                ValeurEchelle.Maximum = 1_000_000m;
                ValeurEchelle.Minimum = 5_000m;
                ValeurEchelle.Name = "ValeurEchelle";
                ValeurEchelle.Size = new Size(80, 26);
                ValeurEchelle.TabIndex = 14;
                ValeurEchelle.TextAlign = HorizontalAlignment.Right;
                ValeurEchelle.ThousandsSeparator = true;
                ToolTip1.SetToolTip(ValeurEchelle, "Valeur demandée pour l'échelle" + CrLf +
                                                   "Toujours respectée");
                ValeurEchelle.Value = 5_000m;
                // 
                // TexteEchelle
                // 
                LabelCoordonnees.BorderStyle = BorderStyle.FixedSingle;
                LabelCoordonnees.Location = new Point(201, 236);
                LabelCoordonnees.Margin = new Padding(0, 0, 3, 0);
                LabelCoordonnees.Name = "TexteEchelle";
                LabelCoordonnees.Size = new Size(158, 26);
                LabelCoordonnees.TabIndex = 26;
                LabelCoordonnees.Text = "1/5 000";
                LabelCoordonnees.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // ChoixCoordonnees
                // 
                ChoixCoordonnees.CheckAlign = ContentAlignment.MiddleRight;
                // ChoixCoordonnees.Checked = False
                // ChoixCoordonnees.CheckState = CheckState.Checked
                ChoixCoordonnees.Location = new Point(205, 237);
                ChoixCoordonnees.Name = "ChoixCoordonnees";
                ChoixCoordonnees.Size = new Size(147, 24);
                ChoixCoordonnees.TabIndex = 35;
                ChoixCoordonnees.Text = "Coordonnées Page";
                ChoixCoordonnees.TextAlign = ContentAlignment.TopLeft;
                ToolTip1.SetToolTip(ChoixCoordonnees, "Imprime les coordonnées du Pt0 de la page" + CrLf +
                                                      "en UTM pour les sites WebMercator" + CrLf +
                                                      "ou Grille Suisse pour le site Suisse Mobile");
                ChoixCoordonnees.UseVisualStyleBackColor = true;
                // 
                // LabelPlanImpression
                // 
                LabelPlanImpression.BorderStyle = BorderStyle.FixedSingle;
                LabelPlanImpression.Location = new Point(6, 268);
                LabelPlanImpression.Name = "LabelPlanImpression";
                LabelPlanImpression.Size = new Size(353, 27);
                LabelPlanImpression.TabIndex = 32;
                LabelPlanImpression.Text = "Plan des pages à imprimer en fonction des choix";
                LabelPlanImpression.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // PlanImpression
                // 
                PlanImpression.BorderStyle = BorderStyle.FixedSingle;
                PlanImpression.Location = new Point(6, 294);
                PlanImpression.Name = "PlanImpression";
                PlanImpression.Size = new Size(353, 353);
                PlanImpression.TabIndex = 33;
                // 
                // LabelResultats
                // 
                LabelResultats.BorderStyle = BorderStyle.FixedSingle;
                LabelResultats.Location = new Point(6, 646);
                LabelResultats.Name = "LabelResultats";
                LabelResultats.Size = new Size(353, 23);
                LabelResultats.TabIndex = 32;
                LabelResultats.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // PanelChoix
                // 
                PanelChoix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                PanelChoix.BackColor = Color.FromArgb(247, 247, 247);
                PanelChoix.Controls.Add(ChoixCentrage);
                PanelChoix.Controls.Add(LabelCentrage);
                PanelChoix.Controls.Add(LabelImprimante);
                PanelChoix.Controls.Add(LabelChoixImprimante);
                PanelChoix.Controls.Add(ListeChoixImprimante);
                PanelChoix.Controls.Add(LabelChoixPapier);
                PanelChoix.Controls.Add(ListeChoixPapier);
                PanelChoix.Controls.Add(LabelChoixResolution);
                PanelChoix.Controls.Add(ListeChoixResolution);
                PanelChoix.Controls.Add(LabelChoixCouleur);
                PanelChoix.Controls.Add(ListeChoixCouleur);
                PanelChoix.Controls.Add(LabelFichierGeoref);
                PanelChoix.Controls.Add(LabelChoixFichierGeoref);
                PanelChoix.Controls.Add(LabelImpression);
                PanelChoix.Controls.Add(LabelBordures);
                PanelChoix.Controls.Add(ChoixNbPages);
                PanelChoix.Controls.Add(LabelNbPages);
                PanelChoix.Controls.Add(LabelRecouvrement);
                PanelChoix.Controls.Add(LabelNbCol);
                PanelChoix.Controls.Add(LabelNbRang);
                PanelChoix.Controls.Add(ChoixEchelle);
                PanelChoix.Controls.Add(LabelEchelle);
                PanelChoix.Controls.Add(ChoixCoordonnees);
                PanelChoix.Controls.Add(LabelCoordonnees);
                PanelChoix.Controls.Add(ValeurBordures);
                PanelChoix.Controls.Add(ValeurRecouvrement);
                PanelChoix.Controls.Add(ValeurNbCol);
                PanelChoix.Controls.Add(ValeurNbRang);
                PanelChoix.Controls.Add(ValeurEchelle);
                PanelChoix.Controls.Add(LabelPlanImpression);
                PanelChoix.Controls.Add(PlanImpression);
                PanelChoix.Controls.Add(LabelResultats);
                PanelChoix.Location = new Point(0, 0);
                PanelChoix.Name = "PanelChoix";
                PanelChoix.Size = new Size(365, 676);
                PanelChoix.TabIndex = 26;
                // 
                // btnNO
                // 
                btnNO.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnNO.DialogResult = DialogResult.Cancel;
                btnNO.Location = new Point(203, 682);
                btnNO.Name = "btnNO";
                btnNO.Size = new Size(75, 30);
                btnNO.TabIndex = 2;
                btnNO.Text = "Quitter";
                ToolTip1.SetToolTip(btnNO, "Ferme le formulaire sans " + CrLf +
                                           "imprimer la carte");
                btnNO.UseVisualStyleBackColor = true;
                // 
                // btnYES
                // 
                btnYES.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnYES.DialogResult = DialogResult.OK;
                btnYES.Enabled = false;
                btnYES.Location = new Point(284, 682);
                btnYES.Name = "btnYES";
                btnYES.Size = new Size(75, 30);
                btnYES.TabIndex = 1;
                btnYES.Text = "Imprimer";
                ToolTip1.SetToolTip(btnYES, "Ferme le formulaire " + CrLf +
                                            "et lance l'impression");
                btnYES.UseVisualStyleBackColor = true;
                // 
                // ImprimeCarte
                // 
                AcceptButton = btnYES;
                AutoScaleMode = AutoScaleMode.None;
                CancelButton = btnNO;
                ClientSize = new Size(365, 718);
                ControlBox = false;
                Controls.Add(PanelChoix);
                Controls.Add(btnNO);
                Controls.Add(btnYES);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "ImprimeCarte";
                ShowIcon = false;
                ShowInTaskbar = false;
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.CenterScreen;
                // TopMost = True
                PanelChoix.ResumeLayout(false);
                ((ISupportInitialize)ValeurBordures).EndInit();
                ((ISupportInitialize)ValeurRecouvrement).EndInit();
                ((ISupportInitialize)ValeurNbCol).EndInit();
                ((ISupportInitialize)ValeurNbRang).EndInit();
                ((ISupportInitialize)ValeurEchelle).EndInit();
                PanelChoix.PerformLayout();
                ResumeLayout(false);
            }
            #region Declration variables
            private Label LabelChoixFichierGeoref;
            private OpenFileDialog OuvrirFichierGeoref;
            private ComboBox ListeChoixImprimante;
            private ComboBox ListeChoixPapier;
            private ComboBox ListeChoixResolution;
            private ComboBox ListeChoixCouleur;
            private NumericUpDown ValeurBordures;
            private Label LabelBordures;
            private Label LabelRecouvrement;
            private NumericUpDown ValeurRecouvrement;
            private ToolTip ToolTip1;
            private Button btnNO;
            private Button btnYES;
            private Label LabelImprimante;
            private Label LabelChoixImprimante;
            private Label LabelChoixPapier;
            private Label LabelChoixResolution;
            private Label LabelChoixCouleur;
            private Label LabelFichierGeoref;
            private Label LabelImpression;
            private Label LabelNbPages;
            private RadioButton ChoixNbPages;
            private RadioButton ChoixEchelle;
            private NumericUpDown ValeurEchelle;
            private Label LabelCoordonnees;
            private CheckBox ChoixCoordonnees;
            private Label LabelNbCol;
            private NumericUpDown ValeurNbCol;
            private Label LabelNbRang;
            private NumericUpDown ValeurNbRang;
            private Label LabelEchelle;
            private Label LabelPlanImpression;
            private Label LabelResultats;
            private Label PlanImpression;
            private Panel PanelChoix;
            private CheckBox ChoixCentrage;
            private Label LabelCentrage;
            #endregion
        }
    }
}