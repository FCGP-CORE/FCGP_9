using static FCGP.AfficheInformation;
using static FCGP.Altitudes;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.CurseurSouris;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.NativeMethods;
using static FCGP.Regroupement;
using static FCGP.Settings;
using static FCGP.SupportVisue;
namespace FCGP
{

    /// <summary> formulaire principal de l'application FCGP_Regrouper </summary>
    internal sealed partial class Regrouper
    {
        #region variables Privées
        private enum ModesFCGP : int
        {
            /// <summary>il n'y a aucune carte ou échelle affichée</summary>
            AucunMode = 0,
            /// <summary>Mode Dépalcer carte</summary>
            DeplacerCarte = 1,
            /// <summary>Mode Créer Trace</summary>
            CreerTrace = 2,
            /// <summary>Mode Modifier Trace</summary>
            ModifierTrace = 3,
            /// <summary>mode qui permet l'export d'une séléction de carte sous forme de fichier KMZ</summary>
            ExporterSelectionKMZ = 4,
            /// <summary>mode qui permet l'impression d'une sélection de carte</summary>
            ImprimerSelection = 5
        }
        /// <summary>les différents outils ou mode disponible dans FCGP_Visue</summary>
        /// <remarks> pour l'instant seul l'outil Aucun et DeplacerCarte sont pris en compte </remarks>
        private enum Outils : int
        {
            /// <summary>il n'y a aucune carte ou échelle affichée</summary>
            AucunOutil = 0,
            /// <summary>Mode Dépalcer carte</summary>
            DeplacerCarte = 1,
            /// <summary>Mode Créer Trace</summary>
            CreerTrace = 2,
            /// <summary>Mode Modifier Trace</summary>
            ModifierTrace = 3,
            /// <summary>mode qui permet l'export d'une séléction de carte sous forme de fichier KMZ</summary>
            ExporterSelectionKMZ = 4,
            /// <summary>mode qui permet l'impression d'une sélection de carte</summary>
            ImprimerSelection = 5,
            /// <summary>en mode déplacer carte, l'outil de déplacement (clic de souris maintenu)</summary>
            DeplacementCarteHV = 6,
            /// <summary>mode qui permet l'impression d'une sélection de carte</summary>
            DeplacementPointTrace = 7,
            /// <summary>mode qui permet l'impression d'une sélection de carte</summary>
            SuppressionPointTrace = 8,
            /// <summary>mode qui permet l'impression d'une sélection de carte</summary>
            InsertionPointTrace = 9,
            /// <summary>en mode déplacer carte, l'outil de zoom (mollette de la souris sans modificateur)</summary>
            Zoom = 10,
            /// <summary>mode KMZ</summary>
            KMZ = 11,
            /// <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
            Attente = 12,
            /// <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
            DeplacementCarteH = 13,
            /// <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
            DeplacementCarteV = 14,
            /// <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
            DeplacementCarteVHS = 15,
            /// <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
            DeplacementCarteVSH = 16

        }
        private Outils OutilEncours
        {
            set
            {
                _OutilEncours = value;
                AffichageCarte.Cursor = CurseurOutilEncours;
                AffichageCarte.Refresh();
                LOutilEnCours.Text = _OutilEncours.ToString();
                LOutilEnCours.Refresh();
            }
        }
        private int DimTuile
        {
            get
            {
                if (ListeChoixDimensions.SelectedIndex == 0)
                {
                    // Dimensions des tuiles pour les fichiers JNX ou ORUX
                    return RegrouperSettings.DIM_TUILE_JNXORUX;
                }
                else
                {
                    // Dimensions des tuiles pour les fichiers KMZ
                    return RegrouperSettings.DIM_TUILE_KMZ;
                }
            }
        }
        /// <summary>Nb d'unité en monde virtuel pour un déplacement Horizontal ou vertical soit à la molette de souris soit aux touches de déplacement</summary>
        private int DeltaDeplacement
        {
            get
            {
                return (int)Math.Round(DeltaDeplacementPixel / ZoomAffichage);
            }
        }
        private int DeltaDeplacementVertical
        {
            get
            {
                return (int)Math.Round(TailleAffichage.Height / ZoomAffichage / 2d);
            }
        }
        private int DeltaDeplacementHorizontal
        {
            get
            {
                return (int)Math.Round(TailleAffichage.Width / ZoomAffichage / 2d);
            }
        }
        /// <summary>les évenements concernant l'affichage des cartes sont autorisés, c'est à dire qu'il y a une échelle affichée</summary>
        private bool EvenementCarteAutorise
        {
            get
            {
                return ClefRegroupementEncours is not null;
            }
        }
        /// <summary>le changement d'affichage et la suppression des échelles est possible</summary>
        private static bool GestionRegroupementAutorise
        {
            get
            {
                return Regroupements.Count > 0;
            }
        }
        /// <summary> renvoie le curseur associé à l'outil encours </summary>
        private Cursor CurseurOutilEncours
        {
            get
            {
                return CurseursOutils[(int)_OutilEncours];
            }
        }
        /// <summary>region en unité du monde virtuel représentée par la surface d'affichage de la carte. 
        /// Cette région est dépendante du coef Zoom et de la surface en pixel de la surface d'affichage</summary>
        /// <remarks>est modifié par une action de zoom appliquée à la surface d'affichage ou par un changement de la surface d'affichage</remarks>
        private Rectangle RegionAffichage
        {
            get
            {
                return new Rectangle(Pt0Affichage, new Size((int)Math.Round(TailleAffichage.Width / ZoomAffichage), (int)Math.Round(TailleAffichage.Height / ZoomAffichage)));
            }
        }
        /// <summary>point en unité du monde virtuel représentée par le centre de la surface d'affichage de la carte. 
        /// Ce point est directement dépendant du coef Zoom et de la surface en pixel de la surface d'affichage</summary>
        /// <remarks>est modifié par une action de zoom appliquée à la surface d'affichage</remarks>
        private Point CentreAffichage
        {
            get
            {
                return PixelsEcranToPixelsSite(new Point(TailleAffichage.Width / 2, TailleAffichage.Height / 2));
            }
        }
        private Point PixelsEcranToPixelsSite(Point PixelsEcran)
        {
            return new Point(Pt0Affichage.X + (int)Math.Round(PixelsEcran.X / ZoomAffichage), Pt0Affichage.Y + (int)Math.Round(PixelsEcran.Y / ZoomAffichage));
        }
        /// <summary> transforme un point donné en pixels de la projection grille et pour l'echelle encours, en pixels de la surface d'affichage </summary>
        private Point PixelsSiteToPixelsEcran(Point PixelsSite)
        {
            return new Point((int)Math.Round(PixelsSite.X * ZoomAffichage) - Pt0Affichage.X, (int)Math.Round(PixelsSite.Y * ZoomAffichage) - Pt0Affichage.Y);
        }

        /// <summary>dernière position enregistrée lors d'un évenement, du pointeur de souris sur la surface d'affichage de la carte exprimée en monde virtuel</summary>
        /// <remarks>Ce point est directement dépendant du coef Zoom et de la position en pixel du pointeur de souris </remarks>
        private Point CoordMousePixels
        {
            get
            {
                return PixelsEcranToPixelsSite(PosMouse);
            }
        }

        /// <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection grille </summary>
        internal static PointD CoordMouseGrille(Point PtPixels, SitesCartographiques SiteCarto, Echelles Echelle)
        {
            return PointPixelToPointGrille(PtPixels, SiteCarto, Echelle);
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection Grille sous forme de chaine caractères formatée </summary>
        internal static string CoordMouseGrilleText(PointD PtGrille)
        {
            return ConvertPointXYtoChaine(new PointProjection(PtGrille));
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux </summary>
        internal static PointD CoordMousseDD(PointD PtGrille, Datums Datum)
        {
            return PointGrilleToPointDD(PtGrille, Datum);
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux sous forme de chaine de caractères formatée </summary>
        internal static string CoordMousseDDText(PointD PtGrille, Datums Datum)
        {
            return ConvertPointDDtoChaine(CoordMousseDD(PtGrille, Datum));
        }
        /// <summary> Coordonnées du pointeur de souris exprimée en degrés Miniute et secondes sous forme de chaine de caractères formatée </summary>
        internal static string CoordMouseDMSText(PointD PtGrille, Datums Datum)
        {
            return ConvertPointDDtoDMS(CoordMousseDD(PtGrille, Datum));
        }
        /// <summary> Coordonnées du pointeur de souris exprimée en mètre de la projection UTM WGS84 sous forme de chaine de caractères formatée </summary>
        internal static string CoordMouseUtmWGS84(PointD PtGrille, Datums Datum)
        {
            return ConvertPointDDtoUTM(CoordMousseDD(PtGrille, Datum));
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en index tuile et offset. </summary>
        internal static PointG CoordMousePointG(Point PtPixels, int IndiceEchelle)
        {
            return new PointG(IndiceEchelle, PtPixels);
        }
        /// <summary> 4 variables utiles pour le calcul des différents types de coordonnées concernant le regroupement encours </summary>
        internal static Datums Datum { get; private set; }
        internal static SitesCartographiques SiteCarto { get; private set; }
        internal static Echelles Echelle { get; private set; }
        internal static int IndiceEchelle { get; private set; }
        /// <summary> altitude au coordonnées du pointeur de souris </summary>
        private string CoordMouseAltitude
        {
            get
            {
                if (AltitudesSettings.IS_ALTITUDE)
                {
                    short AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMousseDD(CoordMouseGrille(CoordMousePixels, SiteCarto, Echelle), Datum));
                    return AltitudePosMouse == -9999 ? "0 m" : AltitudePosMouse.ToString() + " m";
                }
                else
                {
                    return "--- m";
                }
            }
        }
        /// <summary> regroupement actuellement encours d'affichage ou regroupement de travail</summary>
        private Regroupement RegroupementEncours
        {
            get
            {
                return ClefRegroupementEncours is null ? null : Regroupements[ClefRegroupementEncours];
            }
        }
        /// <summary> système Cartographique associé au regroupement en cours</summary>
        private SystemeCartographique SystemeEncours
        {
            get
            {
                return RegroupementEncours?.SystemeType;
            }
        }
        /// <summary> indique si il y a une sélection pour le regroupement en cours</summary>
        private bool FlagSelectionEncours
        {
            get
            {
                return SystemeEncours is not null && !SelectionReferencementSiteCarto(RegroupementEncours.NumSite).IsEmpty;
            }
        }

        /// <summary> Liste des différentes unités pour les coordonnées du pointeur de souris. Evite une variable static interne méthode </summary>
        private readonly string[] CoordType = new string[] { "DD WGS84", "DMS WGS84", "UTM WGS84" };
        /// <summary>Quel est le mode en cours</summary>
        /// <remarks> Pour l'instant seul le mode déplacer carte est pris en charge </remarks>
        private ModesFCGP ModeEncours = ModesFCGP.AucunMode;
        /// <summary>Quel est l'outil ou le mode en cours</summary>
        private Outils _OutilEncours;
        /// <summary>collection de curseurs correspondant aux différents outils disponible dans FCGP_Visue</summary>
        private readonly Cursor[] CurseursOutils = new Cursor[(Enum.GetNames(typeof(Outils)).GetLength(0))];
        /// <summary>Nb de pixel pour un déplacement Horizontal ou vertical soit à la molette de souris soit aux touches de déplacement</summary>
        private int DeltaDeplacementPixel;
        /// <summary>couleur d'affichage par defaut du fond de l'affichage carte</summary>
        private Color CouleurAffichage;
        /// <summary>flag indiquant que le bouton gauche de la souris est appuyé. Cela sert dans l'outil de déplacement de la carte au niveau de l'évènement move de la souris</summary>
        private bool FlagSourisDown;
        /// <summary>flag indiquant qu'il ne faut pas prendre en compte l'évenement encours. Principalement pour la gestion des menus : par exemple combo ChoixEchelle</summary>
        private bool FlagIgnoreEvenement;
        /// <summary>clef de l'échelle en cours</summary>
        private string ClefRegroupementEncours;
        /// <summary>Coef du Zoom. inférieur à 1 on voit plus de carte, supérieur à 1 on voit moins de carte</summary>
        private double ZoomAffichage = 1d;
        /// <summary>taille en pixel de la zone client de la picturebox qui supporte l'affichage de la carte. Mis à jour à la fin de l'évenement Resize</summary>
        private Size TailleAffichage;
        /// <summary>Point de reférence de la surface d'affichage de la carte par rapport au monde virtuel de l'échelle</summary>
        /// <remarks>peut être modifié par une action de déplacement ou par une action de zoom ou par une action de resize appliquée à la surface d'affichage de la carte</remarks>
        private Point Pt0Affichage;
        /// <summary>dernière position enregistrée lors d'un évenement, du pointeur de souris sur la surface d'affichage de la carte exprimée en pixel</summary>
        /// <remarks>permet de garder en mémoire la position du pointeur de souris afin de calculer un delta en pixel avec la nouvelle position du pointeur</remarks>
        private Point PosMouse;
        #endregion
        #region Evènement du formulaire
        /// <summary> Actions lors de l'ouverture du formulaire </summary>
        private void Regrouper_Load(object sender, EventArgs e)
        {
            // initialisation du type de programme
            InitialiserBaseApplication(this, "Core 4.000 CS", "01/03/2023");
            TitreInformation = "Information FCGP_Regrouper";
            // indique aux threads hors UI la procédure à appeler pour afficher un message d'information ou une erreur
            FormHorsUI = new Action(AfficherInfomationsErreurs);
            if (!InitialiserSettings("9.000"))
            {
                Close();
                return;
            }
            try
            {
                InitialiserCommun();
                InitialiserCurseursVisue();
                InitialiserAffichageCarte(true);
                InitialiserRegionAffichageCarte(true);
                if (AltitudesSettings.IS_ALTITUDE)
                {
                    InitialiserListeFichiersAltitudes();
                    InitialiserConnexionAltitudes();
                }
                Text = NumFCGP;
                ListeChoixDimensions.SelectedIndex = RegrouperSettings.TYPE_DIMENSIONS;
                GererMenus();
                MAJ_ZoneInfoRegroupement();
                MAJ_ZoneInfoCoordonnees();
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "O8K0");
            }
        }
        /// <summary> permet de traiter la touche alt comme modifiers et non comme sélecteur du menu </summary>
        private void Regrouper_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Alt)
            {
                AffichageCarte.Select();
                e.SuppressKeyPress = true;
            }
        }
        /// <summary>Actions lors de la fermeture du formulaire</summary>
        private void Regrouper_FormClosed(object sender, FormClosedEventArgs e)
        {
            // on sauvegarde les informations de travail
            RegrouperSettings.Ecrire();
            // arrête éventuellement le téléchargement  de fichiers d'altitude en cours et libère les resource
            CloturerCommun();
            CloturerAltitudes();
            // on attend que le thread des téléchargements libère effectivement les ressources
            Thread.Sleep(500);
        }
        /// <summary> met à jour la surface d'affichage de l'application </summary>
        private void Regrouper_Shown(object sender, EventArgs e)
        {
#if !BETA
            VerifierVersionApplication();
#endif
        }
        /// <summary> gère les touches de fonction de l'application </summary>
        private void Regrouper_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F11:
                    {
                        if (WindowState == FormWindowState.Maximized)
                        {
                            WindowState = FormWindowState.Normal;
                        }
                        else
                        {
                            WindowState = FormWindowState.Maximized;
                        }

                        break;
                    }
                case Keys.F1:
                    {
                        AfficherAide(this);
                        break;
                    }
                case Keys.F3:
                    {
                        OuvrirConfiguration();
                        break;
                    }
                case Keys.F4:
                    {
                        if (e.Modifiers == Keys.Alt)
                        {
                            Close();
                        }
                        else
                        {
                            OuvrirAPropos();
                        }

                        break;
                    }
                case var case1 when Keys.NumPad0 <= case1 && case1 <= Keys.NumPad3:
                case var case2 when Keys.D0 <= case2 && case2 <= Keys.D3:
                    {
                        if (RegroupementEncours.SelectionGeoref.IsEmpty)
                            return;
                        if (e.Control)
                        {
                            AfficherCoinSelection(e.KeyCode);
                        }

                        break;
                    }
                case Keys.NumPad4:
                case Keys.D4:
                    {
                        // on positionne le curseur au centre de l'affichage soit le coin choisi
                        Cursor.Position = AffichageCarte.PointToScreen(new Point(TailleAffichage.Width / 2, TailleAffichage.Height / 2));
                        break;
                    }
                case Keys.X:
                case Keys.Y:
                case Keys.L:
                case Keys.H:
                    {
                        if (RegroupementEncours.SelectionGeoref.IsEmpty)
                            return;
                        if (e.Modifiers == Keys.Control || e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt)
                        {
                            ModifierSelection(e.KeyCode);
                        }

                        break;
                    }
            }
        }
        /// <summary> Stocke la surface d'affichage de l'application. Ne sert à rien pour l'instant. Préparation pour les traces </summary>
        private void Regrouper_LocationChanged(object sender, EventArgs e)
        {
            VisueRectangle = AffichageCarte.RectangleToScreen(AffichageCarte.ClientRectangle);
        }
        private Point _AffichageCarte_Paint_Pt0 = Point.Empty;
        #endregion
        #region évenement VisualisationCarte
        /// <summary> redessine la carte avec la position indiquée indiquée en coordonnées virtuelles, le coef zoom et la surface d'affichage exprimée en pixel </summary>
        private void AffichageCarte_Paint(object sender, PaintEventArgs e)
        {
            var Selection = RegroupementEncours is null ? Rectangle.Empty : RegroupementEncours.SelectionGeoref;
            if (EvenementCarteAutorise == true & _AffichageCarte_Paint_Pt0 != Pt0Affichage)
            {
                // affichage avec un déplacement de la carte
                _AffichageCarte_Paint_Pt0 = Pt0Affichage;
                AfficherTamponVisue(ZoomAffichage, TailleAffichage, Pt0Affichage, Selection, e.Graphics);
            }
            else
            {
                // affichage sans déplacement de la carte
                AfficherTamponvisue(TailleAffichage, Pt0Affichage, Selection, e.Graphics);
            }
        }
        /// <summary> logique en cas de redimensionnement de la surface d'affichage. Le centre de la carte reste au centre </summary>
        private void AffichageCarte_Resize(object sender, EventArgs e)
        {
            if (EvenementCarteAutorise == true)
            {
                // calcul de PT0Visualisation pour que le centre de la carte avant le changement de carte reste le centre de la carte après le changement de taille
                Pt0Affichage.X = CentreAffichage.X - (int)Math.Round(AffichageCarte.ClientSize.Width / 2d / ZoomAffichage);
                Pt0Affichage.Y = CentreAffichage.Y - (int)Math.Round(AffichageCarte.ClientSize.Height / 2d / ZoomAffichage);
                AffichageCarte.Invalidate();
            }
            TailleAffichage = AffichageCarte.ClientSize;
            VisueRectangle = AffichageCarte.RectangleToScreen(AffichageCarte.ClientRectangle);
        }
        /// <summary> pas d'utilisation pour l'instant si ce n'est de rendre actif les évenements sur la surface d'affichage. Servira dans l'outil trace </summary>
        private void AffichageCarte_MouseClick(object sender, MouseEventArgs e)
        {
            AffichageCarte.Select();
        }
        /// <summary> évenement qui permet d'activer l'outil à utiliser en fonction du modeEncours </summary>
        private void AffichageCarte_MouseDown(object sender, MouseEventArgs e)
        {
            if (EvenementCarteAutorise == true)
            {
                // on garde en mémoire le fait que la souris est appuyée. Cela permet de déplacer la carte
                FlagSourisDown = true;
                switch (ModeEncours)
                {
                    case ModesFCGP.DeplacerCarte:
                        {
                            OutilEncours = Outils.DeplacementCarteHV;
                            break;
                        }
                    case ModesFCGP.CreerTrace:
                        {
                            OutilEncours = Outils.CreerTrace;
                            break;
                        }
                    case ModesFCGP.ModifierTrace:
                        {
                            OutilEncours = Outils.ModifierTrace;
                            break;
                        }
                    case ModesFCGP.ExporterSelectionKMZ:
                        {
                            OutilEncours = Outils.ExporterSelectionKMZ;
                            break;
                        }
                    case ModesFCGP.ImprimerSelection:
                        {
                            OutilEncours = Outils.ImprimerSelection;
                            break;
                        }
                }
            }
        }
        /// <summary> gere le deplacement de la carte avec le bouton gauche de la souris appuyé. Gére l'affichage des coordonnées de la carte </summary>
        private void AffichageCarte_MouseMove(object sender, MouseEventArgs e)
        {
            // si il y a une carte ou une échelle à l'affichage on peut la dépalcer
            if (EvenementCarteAutorise == true)
            {
                // si le flag souris appuyé est mis c'est que l'on bouge la carte
                if (FlagSourisDown == true)
                {
                    // il faut que le curseur soit positionné sur la surface de visualisation de la carte
                    if (e.X >= 0 & e.X <= TailleAffichage.Width & e.Y >= 0 & e.Y <= TailleAffichage.Height)
                    {
                        switch (ModeEncours)
                        {
                            case ModesFCGP.DeplacerCarte:
                                {
                                    // on calcul les nouvelles coordonnées du Pt(0) ou point d'origine de la visue en prenant en compte le zoom 
                                    Pt0Affichage.X += (int)Math.Round((PosMouse.X - e.X) / ZoomAffichage);
                                    Pt0Affichage.Y += (int)Math.Round((PosMouse.Y - e.Y) / ZoomAffichage);
                                    AffichageCarte.Invalidate();
                                    break;
                                }
                        }
                    }
                    else
                    {
                        // sinon on considère que la souris n'est plus appuyée et on ne bouge plus la carte
                        FlagSourisDown = false;
                        OutilEncours = (Outils)ModeEncours;
                    }
                }
                else
                {
                    OutilEncours = (Outils)ModeEncours;
                }
                PosMouse = e.Location;
                // affichage à l'écran des coordonnées du pointeur de souris  
                MAJ_ZoneInfoCoordonnees();
            }
        }
        /// <summary>  gere la fin du déplacement de la carte avec bouton gauche de la souris appuyé </summary>
        private void AffichageCarte_MouseUp(object sender, MouseEventArgs e)
        {
            OutilEncours = (Outils)ModeEncours;
            FlagSourisDown = false;
        }
        /// <summary> rendu nécessaire par la traduction VB ==> C#. C# n'acepte pas les variables static 
        /// dans les procédures ou les fonctions </summary>
        private int MemoireDeltaSouris = 0;
        /// <summary> logique de gestion de la molette de la souris : Deplacement vertical, horizontal ou diagonal de la carte et zoom plus ou moins </summary>
        private void AffichageCarte_MouseWheel(object sender, MouseEventArgs e)
        {
            // si il y a une carte ou une échelle à l'affichage on peut la déplacer ou la zoomer
            if (EvenementCarteAutorise == true)
            {
                // calcul de la différence entre la largeur de la picturebox et de la surface d'affichage
                int DeltaX = (AffichageCarte.Width - TailleAffichage.Width) / 2;
                int DeltaY = (AffichageCarte.Height - TailleAffichage.Height) / 2;
                // si la position de la souris est à l'intérieur de la surface d'affichage, l'évenement concerne la visue, sinon il s'agit d'un autre controle
                if (e.X >= AffichageCarte.Left + DeltaX & e.X <= AffichageCarte.Right - DeltaX & e.Y <= AffichageCarte.Bottom - DeltaY & e.Y >= AffichageCarte.Top + DeltaY)
                {
                    switch (ModifierKeys)
                    {
                        case Keys.Shift:
                        case Keys.Shift | Keys.Alt:
                            {
                                MemoireDeltaSouris = 0;
                                if (ModifierKeys == (Keys.Shift | Keys.Alt))
                                {
                                    OutilEncours = Outils.DeplacementCarteVHS;
                                    Pt0Affichage.Y += e.Delta > 0 ? DeltaDeplacement : -DeltaDeplacement;
                                    Pt0Affichage.X += e.Delta > 0 ? DeltaDeplacement : -DeltaDeplacement;
                                }
                                else
                                {
                                    OutilEncours = Outils.DeplacementCarteV;
                                    Pt0Affichage.Y += e.Delta > 0 ? DeltaDeplacement : -DeltaDeplacement;
                                }
                                AffichageCarte.Invalidate();
                                break;
                            }
                        case Keys.Control:
                        case Keys.Control | Keys.Alt:
                            {
                                MemoireDeltaSouris = 0;
                                if (ModifierKeys == (Keys.Control | Keys.Alt))
                                {
                                    OutilEncours = Outils.DeplacementCarteVSH;
                                    Pt0Affichage.Y += e.Delta > 0 ? -DeltaDeplacement : +DeltaDeplacement;
                                    Pt0Affichage.X += e.Delta > 0 ? DeltaDeplacement : -DeltaDeplacement;
                                }
                                else
                                {
                                    OutilEncours = Outils.DeplacementCarteH;
                                    Pt0Affichage.X += e.Delta > 0 ? DeltaDeplacement : -DeltaDeplacement;
                                }
                                AffichageCarte.Invalidate();
                                break;
                            }
                        case Keys.Alt:
                            {
                                OutilEncours = Outils.Zoom;
                                // cette section ne déplace pas la carte mais joue sur le coef zoom
                                if (e.Delta > 0)
                                {
                                    if (MemoireDeltaSouris >= 0)
                                    {
                                        MemoireDeltaSouris += 1;
                                    }
                                    else
                                    {
                                        MemoireDeltaSouris = 1;
                                    }
                                    if (MemoireDeltaSouris == 5)
                                    {
                                        PosMouse = e.Location;
                                        Zoom(DonneProchaineValeurZoom(false), true, true);
                                        MemoireDeltaSouris = 0;
                                    }
                                }
                                else
                                {
                                    if (MemoireDeltaSouris <= 0)
                                    {
                                        MemoireDeltaSouris -= 1;
                                    }
                                    else
                                    {
                                        MemoireDeltaSouris = -1;
                                    }
                                    if (MemoireDeltaSouris == -5)
                                    {
                                        PosMouse = e.Location;
                                        Zoom(DonneProchaineValeurZoom(true), true, true);
                                        MemoireDeltaSouris = 0;
                                    }
                                }

                                break;
                            }

                        default:
                            {
                                MemoireDeltaSouris = 0;
                                OutilEncours = (Outils)ModeEncours;
                                break;
                            }
                    }
                    MAJ_ZoneInfoCoordonnees();
                }
            }
        }
        /// <summary> gère le déplacement de la carte avec les touches clavier de direction </summary>
        private void AffichageCarte_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // si il y a une carte ou une échelle à l'affichage on peut la dépalcer
            if (EvenementCarteAutorise)
            {
                var DX = default(int);
                var DY = default(int);
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DY = -DeltaDeplacement;
                            }
                            DX = -DeltaDeplacement;
                            break;
                        }
                    case Keys.Right:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DY = DeltaDeplacement;
                            }
                            DX = DeltaDeplacement;
                            break;
                        }
                    case Keys.Up:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DX = DeltaDeplacement;
                            }
                            DY = -DeltaDeplacement;
                            break;
                        }
                    case Keys.Down:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DX = -DeltaDeplacement;
                            }
                            DY = DeltaDeplacement;
                            break;
                        }
                    case Keys.Home:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DY = -DeltaDeplacementVertical;
                            }
                            DX = -DeltaDeplacementHorizontal;
                            break;
                        }
                    case Keys.End:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DY = DeltaDeplacementVertical;
                            }
                            DX = DeltaDeplacementHorizontal;
                            break;
                        }
                    case Keys.PageUp:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DX = DeltaDeplacementHorizontal;
                            }
                            DY = -DeltaDeplacementVertical;
                            break;
                        }
                    case Keys.PageDown:
                        {
                            if (ModifierKeys == Keys.Alt)
                            {
                                DX = -DeltaDeplacementHorizontal;
                            }
                            DY = DeltaDeplacementVertical;
                            break;
                        }
                }
                Pt0Affichage.X -= DX;
                Pt0Affichage.Y -= DY;
                AffichageCarte.Invalidate();
                MAJ_ZoneInfoCoordonnees();
            }
        }
        #endregion
        #region Gestion des menus et barre de statut
        /// <summary>  gère la disponibilité des différents menus et boutons </summary>
        private void GererMenus()
        {
            AffichageCarte.Select();
            EtatMenuGestionRegroupements();
            EtatMenuFichiersTuiles();
            EtatBarreIconeMode();
            EtatBarreIconeZoom();
            EtatBarreIconeListeRegroupements();
            EtatBarreIconeRegroupementEtMenus();
            EtatMenucontextuel();
            MAJ_ZoneInfoRegroupement();
        }
        private void EtatMenucontextuel()
        {
            // si il y a une carte d'affichée on peut mettre le menu contextuel correspondant
            if (EvenementCarteAutorise)
            {
                AffichageCarte.ContextMenuStrip = MenuCxtVisualisationCarte;
                // pour la liste des regroupements il faut au moins 2 echelles pour la débloquer
                ContextMenuListeRegroupement.Enabled = Regroupements.Count > 1;
            }
            else
            {
                AffichageCarte.ContextMenuStrip = null;
            }
        }
        private void EtatBarreIconeZoom()
        {
            // pour que la barre soit active il faut une échelle à l'affichage
            BarreIconeZoom.Enabled = EvenementCarteAutorise;
        }
        private void EtatBarreIconeMode()
        {
            BarreIconeMode.Enabled = EvenementCarteAutorise || GestionRegroupementAutorise;
        }
        private void EtatBarreIconeRegroupementEtMenus()
        {
            IconeAfficherPlanRegroupement.Enabled = EvenementCarteAutorise;
            IconeAjouterCartesRegroupement.Enabled = EvenementCarteAutorise;
        }
        private void EtatMenuGestionRegroupements()
        {
            MenuSupprimerRegroupement.Enabled = GestionRegroupementAutorise;
            MenuAjouterRegroupement.Enabled = EvenementCarteAutorise;
        }
        private void EtatMenuFichiersTuiles()
        {
            MenuCreerFichiersJNX_ORUX.Enabled = EvenementCarteAutorise && FlagSelectionEncours;
            MenuCreerFichiersKMZ.Enabled = EvenementCarteAutorise && FlagSelectionEncours;
        }
        private void EtatBarreIconeListeRegroupements()
        {
            BarreIconeListeRegroupement.Enabled = GestionRegroupementAutorise && ModeEncours == ModesFCGP.DeplacerCarte;
            IconeListeRegroupement.Enabled = GestionRegroupementAutorise && ModeEncours == ModesFCGP.DeplacerCarte;
        }
        /// <summary> Mettre à jour la zone de texte réservée à l'échelle </summary>
        private void MAJ_ZoneInfoRegroupement()
        {
            if (EvenementCarteAutorise)
            {
                Regroupement.Text = $"{RegroupementEncours.ClefEchelle}, Nb cartes: {RegroupementEncours.Cartes.Count}";
                ZoneSelection.Text = DimensionsSelection();
            }
            else
            {
                Regroupement.Text = "Aucun Regroupement";
                ZoneSelection.Text = $"Aucune Sélection [{DimTuile}]";
            }
            Regroupement.Refresh();
            ZoneSelection.Refresh();
        }
        private string DimensionsSelection()
        {
            if (FlagSelectionEncours)
            {
                var R = RegroupementEncours.SelectionGeoref;
                double Txd = R.Width / (double)DimTuile;
                int Tx = (int)Math.Ceiling(Txd);
                double Tyd = R.Height / (double)DimTuile;
                int Ty = (int)Math.Ceiling(Tyd);
                return $"{Tx} * {Ty} ({Txd:N2} * {Tyd:N2}) [{DimTuile}]";
            }
            return $"Aucune Sélection [{DimTuile}]";
        }
        /// <summary> mettre à jour les zones de texte réservée au coordonnées </summary>
        private void MAJ_ZoneInfoCoordonnees()
        {
            if (EvenementCarteAutorise)
            {
                switch (RegrouperSettings.INDICE_TYPE_COORDONNEES)
                {
                    case 3:
                        {
                            LabelCoordonnees.Text = CoordMouseUtmWGS84(CoordMouseGrille(CoordMousePixels, SiteCarto, Echelle), Datum);
                            break;
                        }
                    case 2:
                        {
                            LabelCoordonnees.Text = CoordMouseDMSText(CoordMouseGrille(CoordMousePixels, SiteCarto, Echelle), Datum);
                            break;
                        }
                    case 1:
                        {
                            LabelCoordonnees.Text = CoordMousseDDText(CoordMouseGrille(CoordMousePixels, SiteCarto, Echelle), Datum);
                            break;
                        }
                    case 0:
                        {
                            LabelCoordonnees.Text = CoordMouseGrilleText(CoordMouseGrille(CoordMousePixels, SiteCarto, Echelle));
                            break;
                        }
                    case 4:
                        {
                            LabelCoordonnees.Text = CoordMousePointG(CoordMousePixels, IndiceEchelle).ToString();
                            break;
                        }
                }
                if (AltitudesSettings.IS_ALTITUDE)
                {
                    Altitude.Text = CoordMouseAltitude;
                }
                else
                {
                    Altitude.Text = "---- m";
                }
            }
            else
            {
                LabelCoordonnees.Text = "";
                Altitude.Text = "";
            }
            LabelCoordonnees.Refresh();
            Altitude.Refresh();
        }
        /// <summary> supprime la sélection associé au site encours </summary>
        private void SupprimerSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EffacerSelectionReferencementSiteCarto(RegroupementEncours.NumSite);
            EtatMenuFichiersTuiles();
        }
        /// <summary> procedure appelée par programme </summary>
        /// <param name="Mode"></param>
        private void SelectionnerMode(ModesFCGP Mode)
        {
            ToolStripButton ts;
            // on déselectionne l'ensemble des boutons
            foreach (ToolStripButton currentTs in BarreIconeMode.Items)
            {
                ts = currentTs;
                ts.Checked = false;
            } ((ToolStripButton)BarreIconeMode.Items[0]).Checked = false;
            if (Mode != ModesFCGP.AucunMode)
            {
                // on selectionne le bouton qui vient d'être cliqué
                ts = (ToolStripButton)BarreIconeMode.Items[(int)Mode - 1];
                ts.Checked = true;
            }
            // on indique le mode en cours et l'outil correspondant
            ModeEncours = Mode;
            OutilEncours = (Outils)Mode;
            // on indique à l'utilisateur
            LModeEncours.Text = Mode.ToString();
            LModeEncours.Refresh();
            GererMenus();
        }
        private void IconeDeplacerCarte_Click(object sender, EventArgs e)
        {
            ToolStripButton ts = (ToolStripButton)sender;
            if (ts.Checked == false)
            {
                // on indique le mode en cours et l'outil correspondant
                ModeEncours = ModesFCGP.DeplacerCarte;
                OutilEncours = (Outils)ModeEncours;
                ts.Checked = true;
                GererMenus();
            }
        }
        /// <summary>Fermeture du formulaire par le menu</summary>
        private void MenuQuitter_Click(object sender, EventArgs e) // Handles MenuQuitter.Click
        {
            Close();
        }
        #endregion
        #region Gestion du zoom
        /// <summary> quand on click sur le menu Zoom- ou le bouton Zoom-, permet de diminuer la valeur du Zoom d'un cran prédéfini à l'avance
        /// n'efface pas la valeur du zoom sélectionnée par valeur </summary>
        private void ZoomMoins_Click(object sender, EventArgs e)
        {
            Zoom(DonneProchaineValeurZoom(false));
        }
        /// <summary> quand on click sur le menu Zoom+ ou le bouton Zoom+  permet d'augmenter la valeur du Zoom d'un cran prédéfini à l'avance
        /// n'efface pas la valeur du zoom sélectionnée par valeur  </summary>
        private void ZoomPlus_Click(object sender, EventArgs e)
        {
            Zoom(DonneProchaineValeurZoom(true));
        }
        /// <summary> permet d'afficher la liste deroulante des valeurs du zoom si on click sur l'image du bouton </summary>
        private void IconeZoom_Click(object sender, EventArgs e)
        {
            IconeZoom.ShowDropDown();
        }
        /// <summary>  quand on click sur un des menus représentant une valeur définie permet de sélectionner une valeur pour le zoom </summary>
        private void ZoomValeur_Click(object sender, EventArgs e)
        {
            Zoom(double.Parse(((ToolStripMenuItem)sender).Name[5..]) / 1000d);
        }
        /// <summary>  contient la logique de gestion des menus associés au Zoom et le calcul des coordonnées de la fenêtre d'affichage de la carte
        /// soit par rapport au centre de la carte (liste déroulant icone)  soit par rapport au pointeur (menu ctxt) </summary>
        /// <param name="ValeurZoom"> valeur du zoom de l'affichage de 0.5 à 2 </param>
        /// <param name="FlagCoordonneesPointeur"> Est ce que l'on prend en compte la position du curseur lors du dessin de l'affichage </param>
        /// <param name="FlagRedessineImage"> Est ce que l'on redessine l'affichage </param>
        private void Zoom(double ValeurZoom, bool FlagRedessineImage = true, bool FlagCoordonneesPointeur = false)
        {
            if (ValeurZoom != ZoomAffichage)
            {
                ToolStripMenuItem tsmi;
                // on dé-cheke l'ancienne valeur 
                tsmi = (ToolStripMenuItem)cmsZoomMenu.Items[$"Zoom_{ZoomAffichage * 1000d:0000}"];
                tsmi.Checked = false;
                // on checke la nouvelle valeur
                tsmi = (ToolStripMenuItem)cmsZoomMenu.Items[$"Zoom_{ValeurZoom * 1000d:0000}"];
                tsmi.Checked = true;
                // calcul du point d'origine 
                if (FlagCoordonneesPointeur)
                {
                    // par rapport à la position de la souris
                    Pt0Affichage.X = CoordMousePixels.X - (int)Math.Round(PosMouse.X / ValeurZoom);
                    Pt0Affichage.Y = CoordMousePixels.Y - (int)Math.Round(PosMouse.Y / ValeurZoom);
                }
                else
                {
                    // par rapport au centre de la carte
                    Pt0Affichage.X = CentreAffichage.X - (int)Math.Round(TailleAffichage.Width / 2d / ValeurZoom);
                    Pt0Affichage.Y = CentreAffichage.Y - (int)Math.Round(TailleAffichage.Height / 2d / ValeurZoom);
                }
                // on mémorise le nouveau coef du zoom
                ZoomAffichage = ValeurZoom;
                // on redessine la carte
                if (FlagRedessineImage)
                    AffichageCarte.Invalidate();
            }
            // on met à jour l'info utilisateur
            if (EvenementCarteAutorise)
            {
                LZoom.Text = $"{ZoomAffichage * 100d:000}%";
            }
            else
            {
                LZoom.Text = "";
            }
            LZoom.Refresh();
        }
        /// <summary> renvoie la prochaine valeur du coef zoom en fonction du sens demandé  </summary>
        /// <param name="FlagPlus"></param>
        private double DonneProchaineValeurZoom(bool FlagPlus = false)
        {
            double valeur;
            if (FlagPlus)
            {
                switch (ZoomAffichage)
                {
                    case 0.5d:
                        {
                            valeur = 0.66d;
                            break;
                        }
                    case 0.66d:
                        {
                            valeur = 0.75d;
                            break;
                        }
                    case 0.75d:
                        {
                            valeur = 0.875d;
                            break;
                        }
                    case 0.875d:
                        {
                            valeur = 1.0d;
                            break;
                        }
                    case 1.0d:
                        {
                            valeur = 1.125d;
                            break;
                        }
                    case 1.125d:
                        {
                            valeur = 1.25d;
                            break;
                        }
                    case 1.25d:
                        {
                            valeur = 1.5d;
                            break;
                        }
                    case 1.5d:
                        {
                            valeur = 1.75d;
                            break;
                        }
                    case 1.75d:
                        {
                            valeur = 2d;
                            break;
                        }

                    default:
                        {
                            valeur = 2.0d;
                            break;
                        }
                }
            }
            else
            {
                switch (ZoomAffichage)
                {
                    case 0.66d:
                        {
                            valeur = 0.5d;
                            break;
                        }
                    case 0.75d:
                        {
                            valeur = 0.66d;
                            break;
                        }
                    case 0.875d:
                        {
                            valeur = 0.75d;
                            break;
                        }
                    case 1.0d:
                        {
                            valeur = 0.875d;
                            break;
                        }
                    case 1.125d:
                        {
                            valeur = 1.0d;
                            break;
                        }
                    case 1.25d:
                        {
                            valeur = 1.125d;
                            break;
                        }
                    case 1.5d:
                        {
                            valeur = 1.25d;
                            break;
                        }
                    case 1.75d:
                        {
                            valeur = 1.5d;
                            break;
                        }
                    case 2.0d:
                        {
                            valeur = 1.75d;
                            break;
                        }

                    default:
                        {
                            valeur = 0.5d;
                            break;
                        }
                }
            }
            return valeur;
        }
        #endregion
        #region Gestion des Echelles
        /// <summary> Création d'un regroupement à partir d'une carte et affichage du regroupement avec le centre de la carte </summary>
        private void OuvirRegroupement_Click(object sender, EventArgs e)
        {
            if (OuvrirFichier.ShowDialog(this) == DialogResult.OK)
            {
                var SystemeType = Carte.RenvoyerSystemeCartographique(OuvrirFichier.FileName);
                // pour que le systeme soit pris en compte dans une échelle il faut qu'il soit issue d'une capture de site sans interpolation
                if (SystemeType.IsOk & SystemeType.IsCapture)
                {
                    string ClefSysNew = CalculerClefRegroupement(SystemeType.ClefEchelle);
                    var NbCartes = default(int);
                    var Ret = CreerRegroupement(OuvrirFichier.FileName, ClefSysNew, ref NbCartes); // flagcalpinage = true
                    if (Ret == VerifierRegroupement.OK)
                    {
                        var R = RegroupementEncours;
                        // on met l'outil Attente car l'opération peut être longue
                        OutilEncours = Outils.Attente;
                        // on  sauvegarde la position d'affichage et le coefzoom du regroupement en cours si il y en a un d'affiché
                        if (ClefRegroupementEncours is not null)
                        {
                            // sauvegarde le coefZoom d'affichage de l'échelle en cours
                            R.CoefZoomAffichageRetour = ZoomAffichage;
                            // sauvegarde la position d'affichage de l'échelle en cours
                            R.Affiche_PositionRetour = CentreAffichage;
                        }
                        ClefRegroupementEncours = ClefSysNew;
                        R = Regroupements[ClefSysNew];
                        MessageInformation = "Le regroupement " + R.ClefEchelle + " a été créé : " + NbCartes + " cartes ajoutées";
                        // réinitialisation du coeficient du zoom
                        Zoom(1.0d, false);
                        // trouver le centre de la carte qui sera aussi le centre de l'affichage et du tampon, c'est le point du centre de la carte qui
                        // a servie à la création de l'échelle + la moitié de la surface d'affichage. Quand le coefzoom =1, monde virtuel = monde physique en pixel
                        Pt0Affichage.X = R.PositionGeoref.X - TailleAffichage.Width / 2;
                        Pt0Affichage.Y = R.PositionGeoref.Y - TailleAffichage.Height / 2;
                        // mettre à jour le tampon avec l'image de la carte
                        ChargerRegroupement(R.PositionGeoref, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage);
                        // demander le mode correspondant
                        SelectionnerMode(ModesFCGP.DeplacerCarte);
                        // il y a au moins une carte à l'affichage donc il faut remplir les différentes combox associées aux échelles
                        RemplirChoixRegroupements();
                        // demander la mise à jour de l'affichage
                        AffichageCarte.Invalidate();
                    }
                    else
                    {
                        MessageInformation = "Le regroupement n'a pas été créé : " + Ret.ToString();
                    }
                }
                else
                {
                    MessageInformation = "Le regroupement n'a pas été créé : " + CrLf +
                                         "Le système Cartographique de la carte" + CrLf +
                                         "n'est pas valide ou il s'agit d'une carte interpolée.";
                }
                AfficherInformation();
                OutilEncours = (Outils)ModeEncours;
            }
        }
        /// <summary> permet d'ajouter les cartes d'un autre répertoire dans l'échelle courante </summary>
        private void AjouterCartesRegroupement_Click(object sender, EventArgs e)
        {
            if (OuvrirFichier.ShowDialog() == DialogResult.OK)
            {
                var NbCartes = default(int);
                var R = RegroupementEncours;
                string FichierGeorefInit = OuvrirFichier.FileName;
                // on autorise le traitement
                var Ret = R.AjouterCartes(FichierGeorefInit, ref NbCartes);
                if (Ret == VerifierRegroupement.OK)
                {
                    OutilEncours = Outils.Attente;
                    MessageInformation = NbCartes + " cartes ont ajoutées au regroupement : " + R.ClefEchelle;
                    // on demande l'initialisation du tampon à partir de la carte qui a été selectionnée
                    // le centre de la carte sera aussi le centre de la visue et du tampon. 
                    // coefzoom =1, monde virtuel = monde physique en pixel. On le prend en compte pour les cas ou il soit différent
                    Pt0Affichage.X = R.PositionGeoref.X - (int)Math.Round(TailleAffichage.Width / 2d / ZoomAffichage);
                    Pt0Affichage.Y = R.PositionGeoref.Y - (int)Math.Round(TailleAffichage.Height / 2d / ZoomAffichage);
                    ChargerRegroupement(R.PositionGeoref, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage);
                    OutilEncours = (Outils)ModeEncours;
                    MAJ_ZoneInfoRegroupement();
                    AffichageCarte.Invalidate();
                }
                else
                {
                    MessageInformation = "Aucune carte n'a été ajoutée au regroupement : " + Ret.ToString();
                }
                AfficherInformation();
                OutilEncours = (Outils)ModeEncours;
            }
        }
        /// <summary> permet d'afficher le plan de l'échelle courante </summary>
        private void AfficherPlanRegroupement_Click(object sender, EventArgs e)
        {
            var R = RegroupementEncours;
            // on limite la hauteur du plan de clapinage à 80% de la hauteur du formulaire
            R.AfficherCalepinage(RegionAffichage, (int)Math.Round(Height * 0.8d), 35);
            if (R.NumCarteSelection > 0) // si il y a une carte de sélectionnée
            {
                OutilEncours = Outils.Attente;
                // on calcule les nouvelles coordonnées du pt(0) de l'écran
                Pt0Affichage.X = R.PositionGeoref.X - (int)Math.Round(TailleAffichage.Width / 2d / ZoomAffichage);
                Pt0Affichage.Y = R.PositionGeoref.Y - (int)Math.Round(TailleAffichage.Height / 2d / ZoomAffichage);
                // on demande l'initialisation du tampon à partir du point sélectionné qui sera au centre du tampon
                ChargerRegroupement(R.PositionGeoref, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage);
                OutilEncours = (Outils)ModeEncours;
                AffichageCarte.Invalidate();
            }
            MAJ_ZoneInfoRegroupement();
            GererMenus();
        }
        /// <summary> gère la logique du changement de regroupement à l'affichage en remettant le coef zoom et la position de la visue 
        /// quand le regroupement a été enlevée de l'affichage </summary>
        private void IconeListeEchelle_SelectedIndexChanged(object sender, EventArgs e)
        {
            // si le flag est mis, cela indique qu'il ne s'agit pas d'une MAJ par un click de l'utilisateur, donc on ne prend pas en compte l'évènement
            if (!FlagIgnoreEvenement)
            {
                var R = RegroupementEncours;
                // si le choix est différent de l'échelle encours on affiche le choix
                string ClefSysNew = CalculerClefRegroupement(IconeListeRegroupement.SelectedItem.ToString());
                if ((ClefSysNew ?? "") != (ClefRegroupementEncours ?? ""))
                {
                    OutilEncours = Outils.Attente;
                    // si il y a une échelle d'affichée on sauvegarde les données d'affichage associée pour pouvoir la réafficher aux mêmes conditions
                    if (EvenementCarteAutorise)
                    {
                        // sauvegarde le coefZoom d'affichage de l'échelle en cours
                        R.CoefZoomAffichageRetour = ZoomAffichage;
                        // sauvegarde la position d'affichage de l'échelle en cours
                        R.Affiche_PositionRetour = CentreAffichage;
                    }
                    ClefRegroupementEncours = ClefSysNew;
                    R = Regroupements[ClefSysNew];
                    // et on met à jour la 2ème combox
                    FlagIgnoreEvenement = true;
                    ContextMenuListeRegroupement.SelectedItem = R.ClefEchelle;
                    FlagIgnoreEvenement = false;
                    // on affiche aux mêmes conditions
                    Zoom(R.CoefZoomAffichageRetour, false);
                    // mettre  jour les coordonnées de la fenêtre d'affichage
                    Pt0Affichage.X = R.Affiche_PositionRetour.X - (int)Math.Round(TailleAffichage.Width / 2d / ZoomAffichage);
                    Pt0Affichage.Y = R.Affiche_PositionRetour.Y - (int)Math.Round(TailleAffichage.Height / 2d / ZoomAffichage);
                    // on met à jour le tampon avec l'image de la carte
                    ChargerRegroupement(R.Affiche_PositionRetour, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage);
                    // on met à jour les infos de l'échelle qui sont affichées
                    GererMenus();
                    OutilEncours = (Outils)ModeEncours;
                    AffichageCarte.Invalidate();
                }
            }
            MAJ_TypeCoordonnees();
        }
        /// <summary> logique associée à la liste déroulante des regroupements du menu contextuel </summary>
        private void ContextMenuListeRegroupement_SelectedIndexChanged(object sender, EventArgs e)
        {
            // si le flag est mis, cela indique qu'il ne s'agit pas d'une MAJ par un click de l'utilisateur, donc on ne prend pas en compte l'évènement
            if (!FlagIgnoreEvenement)
            {
                // Si on arrive ici c'est qu'il y a au moins 2 echelles et qu'il y en a une qui est en cours d'affichage
                // on ferme le menu
                MenuCxtVisualisationCarte.Hide();
                string ClefSysNew = CalculerClefRegroupement(ContextMenuListeRegroupement.SelectedItem.ToString());
                if ((ClefSysNew ?? "") != (ClefRegroupementEncours ?? ""))
                {
                    var R = RegroupementEncours;
                    // on met le curseur d'attente
                    AffichageCarte.Cursor = CurseursOutils[(int)Outils.Attente];
                    // sauvegarde le coefZoom d'affichage de l'échelle en cours
                    R.CoefZoomAffichageRetour = ZoomAffichage;
                    // sauvegarde la position d'affichage de l'échelle en cours
                    R.Affiche_PositionRetour = CentreAffichage;
                    // on récupère les coordonnées virtuelles du point cliqué 
                    var CoordonneesClick = CoordMousePixels;
                    // on les transforme en coordonnées réelles
                    var PtR = R.SystemeType.ConvertirCoordonneesVirtuellesToReelles(CoordonneesClick);
                    var Datum_Old = R.Datum;
                    // on met la valeur de zoom à 1
                    Zoom(1d, false);
                    // puis en coordonnées virtuelles de la nouvelle echelle
                    ClefRegroupementEncours = ClefSysNew;
                    R = Regroupements[ClefSysNew];
                    if (Datum_Old != R.Datum)
                    {
                        ConvertirCoordonnees(Datum_Old, R.Datum, ref PtR);
                    }
                    // on récupère les coordonnées virtuelles du nouveau site
                    CoordonneesClick = R.SystemeType.ConvertirCoordonneesReellesToVirtuelles(PtR);
                    // on le recentre par rapport à l'affichage
                    Pt0Affichage.X = CoordonneesClick.X - (int)Math.Round(TailleAffichage.Width / 2d / ZoomAffichage);
                    Pt0Affichage.Y = CoordonneesClick.Y - (int)Math.Round(TailleAffichage.Height / 2d / ZoomAffichage);
                    // et on met à jour la 2ème combox
                    FlagIgnoreEvenement = true;
                    IconeListeRegroupement.SelectedItem = R.ClefEchelle;
                    FlagIgnoreEvenement = false;
                    // on met à jour le tampon avec l'image de la carte
                    ChargerRegroupement(CoordonneesClick, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage);
                    GererMenus();
                    AffichageCarte.Invalidate();
                    AffichageCarte.Cursor = CurseurOutilEncours;
                }
            }
        }
        /// <summary> logique associée à la liste déroulante des regroupements du menu principal/ supprimer un regroupement </summary>
        private void SuppressionListeRegroupement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FlagIgnoreEvenement)
            {
                // pour fermer la liste et le menu
                MenuSupprimerRegroupement.HideDropDown();
                MenuGestionRegroupements.HideDropDown();
                ToolStripComboBox T = (ToolStripComboBox)sender;
                string CleRegroupementSupprime = (string)T.SelectedItem;
                string ClefSysSupprimee = CalculerClefRegroupement(CleRegroupementSupprime);

                MessageInformation = "Etes vous sur de vouloir supprimer" + CrLf + "le regroupement : " + CleRegroupementSupprime;
                if (AfficherConfirmation() == DialogResult.OK)
                {
                    // on met le curseur d'attente
                    AffichageCarte.Cursor = CurseursOutils[(int)Outils.Attente];
                    int NumSiteRegroupementSupprime = Regroupements[ClefSysSupprimee].NumSite;
                    // on supprime l'échelle de la collection
                    SupprimerRegroupement(Regroupements[ClefSysSupprimee]);
                    // si il n'y a plus d'échelle pour le site on supprime la séléction associée au site
                    if (NbRegroupementsSiteCarto(NumSiteRegroupementSupprime) == 0)
                        EffacerSelectionReferencementSiteCarto(NumSiteRegroupementSupprime);
                    // si il n'y a pas d'échelle à afficher, il faut reinitialiser les variables et l'affichage
                    if ((ClefSysSupprimee ?? "") == (ClefRegroupementEncours ?? "") | Regroupements.Count == 0)
                    {
                        ClefRegroupementEncours = null;
                        Zoom(1.0d, false);
                        Pt0Affichage = Point.Empty;
                        ChargerRegroupement(CentreAffichage, null, ZoomAffichage, TailleAffichage, CouleurAffichage);
                        SelectionnerMode(ModesFCGP.AucunMode);
                        AffichageCarte.Invalidate();
                        MAJ_ZoneInfoCoordonnees();
                    }
                    // on met à jour les listes déroulantes associées aux échelles
                    RemplirChoixRegroupements();
                }
            }
        }
        /// <summary> Remplit les 3 listes déroulantes (Menu, menu contextuel et icone) avec la liste des regroupements en cours et évite le déclenchement</summary>
        private void RemplirChoixRegroupements()
        {
            IconeListeRegroupement.Items.Clear();
            ContextMenuListeRegroupement.Items.Clear();
            SuppressionListeRegroupement.Items.Clear();
            // pour chaque echelle dans la collection
            foreach (KeyValuePair<string, Regroupement> T in Regroupements)
            {
                IconeListeRegroupement.Items.Add(T.Value.ClefEchelle);
                ContextMenuListeRegroupement.Items.Add(T.Value.ClefEchelle);
                SuppressionListeRegroupement.Items.Add(T.Value.ClefEchelle);
            }
            // on met à jour les 3 combobox associées mais on fait en sorte de ne pas prendre en compte l'évenement
            if (RegroupementEncours is not null)
            {
                // on interdit le traitements des évenements sur les listes de regroupements
                FlagIgnoreEvenement = true;
                SuppressionListeRegroupement.SelectedItem = RegroupementEncours.ClefEchelle;
                IconeListeRegroupement.SelectedItem = RegroupementEncours.ClefEchelle;
                ContextMenuListeRegroupement.SelectedItem = RegroupementEncours.ClefEchelle;
                // on autorise le traitements des évenements sur les listes de regroupements
                FlagIgnoreEvenement = false;
            }
            GererMenus();
        }
        #endregion
        #region Gestion des autres actions
        /// <summary> lance l'ouverture du formulaire liés aux informations générales de l'application </summary>
        private void MenuAPropos_Click(object sender, EventArgs e)
        {
            OuvrirAPropos();
        }
        /// <summary> lance l'ouverture du formulaire associé à l'aide de l'application </summary>
        private void MenuAideRegrouper_Click(object sender, EventArgs e)
        {
            AfficherAide(this);
        }
        /// <summary> lance l'ouverture du formulaire associé à la configuration de l'application </summary>
        private void MenuConfigurationRegrouper_Click(object sender, EventArgs e)
        {
            OuvrirConfiguration();
        }
        /// <summary> lance l'ouverture du formulaire associé à la création des fichiers ORUX et ou JNX </summary>
        private void MenuCreerFichiersJNX_ORUX_Click(object sender, EventArgs e)
        {
            // sauvegarde le coefZoom d'affichage de l'échelle en cours
            RegroupementEncours.CoefZoomAffichageRetour = ZoomAffichage;
            // sauvegarde la position d'affichage de l'échelle en cours
            RegroupementEncours.Affiche_PositionRetour = CentreAffichage;
            using (Form R = new FichiersTuilesJnxOrux() { RegroupementEncours = RegroupementEncours })
            {
                R.ShowDialog(this);
            }
            AffichageCarte.Invalidate();
            ListeChoixDimensions.SelectedIndex = 0;
            MAJ_ZoneInfoRegroupement();
        }
        /// <summary> lance l'ouverture du formulaire associé à la création des fichiers KMZ </summary>
        private void MenuCreerFichiersKMZ_Click(object sender, EventArgs e)
        {
            // sauvegarde le coefZoom d'affichage de l'échelle en cours
            RegroupementEncours.CoefZoomAffichageRetour = ZoomAffichage;
            // sauvegarde la position d'affichage de l'échelle en cours
            RegroupementEncours.Affiche_PositionRetour = CentreAffichage;
            using (Form R = new FichierTuilesKMZ() { RegroupementEncours = RegroupementEncours })
            {
                R.ShowDialog(this);
            }
            AffichageCarte.Invalidate();
            ListeChoixDimensions.SelectedIndex = 1;
            MAJ_ZoneInfoRegroupement();
        }
        /// <summary> lance l'ouverture du formulaire associé à la modification des fichiers KMZ </summary>
        private void MenuModifierFichierKMZ_Click(object sender, EventArgs e)
        {
            using (Form R = new AffichageKMZ())
            {
                R.ShowDialog(this);
            }
        }
        /// <summary> ouvre le formulaire liés aux indices d'affichage des fichiers JNX </summary>
        private void MenuModifierFichiersJNX_Click(object sender, EventArgs e) // Handles FichiersJNXToolStripMenuItem.Click
        {
            using (Form R = new AffichageJNX())
            {
                R.ShowDialog(this);
            }
        }
        /// <summary> ouvre le formulaire liés aux indices d'affichage des fichiers ORUX </summary>
        private void MenuModifierFichiersORUX_Click(object sender, EventArgs e) // Handles FichiersORUXToolStripMenuItem.Click
        {
            using (Form R = new AffichageORUX())
            {
                R.ShowDialog(this);
            }
        }
        #endregion
        #region Procédures et fonction générales
        internal void AfficherInfomationsErreurs()
        {
            AfficherInformation();
        }
        /// <summary> initialise les différents curseurs dont on a besoin que ceux ci soit déja présents dans le formulaire ou pas </summary>
        private void InitialiserCurseursVisue()
        {
            InitialiserCurseurs(AffichageCarte, Cursor);
            CurseursOutils[(int)Outils.AucunOutil] = Curseur(Curseurs.Arrow);
            CurseursOutils[(int)Outils.DeplacerCarte] = Curseur(Curseurs.CarteDefaut);
            CurseursOutils[(int)Outils.CreerTrace] = Curseur(Curseurs.TraceDefaut);
            CurseursOutils[(int)Outils.ModifierTrace] = Curseur(Curseurs.TraceDeplacePt);
            CurseursOutils[(int)Outils.ExporterSelectionKMZ] = Curseur(Curseurs.Arrow);
            CurseursOutils[(int)Outils.ImprimerSelection] = Curseur(Curseurs.Arrow);
            CurseursOutils[(int)Outils.DeplacementCarteHV] = Curseur(Curseurs.CarteDeplacement);
            CurseursOutils[(int)Outils.DeplacementPointTrace] = Curseur(Curseurs.TraceDeplacePt);
            CurseursOutils[(int)Outils.InsertionPointTrace] = Curseur(Curseurs.TraceInsertPt);
            CurseursOutils[(int)Outils.SuppressionPointTrace] = Curseur(Curseurs.TraceSupprimePtSeg);
            CurseursOutils[(int)Outils.Zoom] = Curseur(Curseurs.Zoom);
            CurseursOutils[(int)Outils.KMZ] = Curseur(Curseurs.Arrow);
            CurseursOutils[(int)Outils.Attente] = Curseur(Curseurs.Attendre);
            CurseursOutils[(int)Outils.DeplacementCarteH] = Curseur(Curseurs.CarteDeplacementVertical);
            CurseursOutils[(int)Outils.DeplacementCarteV] = Curseur(Curseurs.CarteDeplacementHorizontal);
            CurseursOutils[(int)Outils.DeplacementCarteVHS] = Curseur(Curseurs.CarteDeplacementVHS);
            CurseursOutils[(int)Outils.DeplacementCarteVSH] = Curseur(Curseurs.CarteDeplacementVSH);
        }
        /// <summary> met à jour les principales variables globales du formulaire</summary>
        /// <param name="FlagInitSupportVisue"> flag indiquant si il faut initialiser les variables du support visue </param>
        private void InitialiserAffichageCarte(bool FlagInitSupportVisue)
        {
            DeltaDeplacementPixel = RegrouperSettings.PAS_DEPLACEMENT;
            CouleurAffichage = PartagerSettings.COULEUR_VISUE;
            if (FlagInitSupportVisue)
            {
                InitialiserSupportVisue(MaxBoundsEcran, CouleurAffichage);
            }
            else
            {
                ChangerCouleurVisue(CouleurAffichage);
            }
            AffichageCarte.Invalidate();
        }
        /// <summary>  gère le redimensionnement de l'écran et la taille de l'affichage </summary>
        /// <param name="FlagMaximize"> indique si le formulaire doit être maximisé </param>
        private void InitialiserRegionAffichageCarte(bool FlagMaximize)
        {
            WindowState = FormWindowState.Normal;
            // dimensions du formulaire environ 80% des dimensions de l'écran
            Bounds = Rectangle.Inflate(DimensionsEcranSupport, (int)(DimensionsEcranSupport.Width * -0.1d), (int)(DimensionsEcranSupport.Height * -0.1d));
            // a l'ouverture, l'application est maximisée sur l'écran de travail définit par le settings
            if (FlagMaximize)
                WindowState = FormWindowState.Maximized;
        }
        /// <summary> renvoie une chaine unique basée sur la clefEchelle d'un système cartographique. Permet un tri par ordre croissant ou décroissant  </summary>
        /// <param name="ClefEchelle"></param>
        private static string CalculerClefRegroupement(string ClefEchelle)
        {
            string[] s = ClefEchelle.Split('-');
            if (s.Length == 3)
            {
                s[0] += "-" + s[1] + (s[2].Length > 3 ? "-" : "-0") + s[2];
            }
            else
            {
                s[0] += (s[1].Length > 3 ? "-" : "-0") + s[1];
            }
            return s[0];
        }
        /// <summary> sert pour passer des coordonnées réelles d'une échelle en projection aux coordonnées réelles d'une échelle en LL et vice versa </summary>
        /// <param name="ProjOld">projection en cours du point Ptr</param>
        /// <param name="ProjNew">projection future du point Ptr</param>
        /// <param name="Ptr">point en coordonnées réelles à transformer d'une projection à une autre</param>
        private static void ConvertirCoordonnees(Datums ProjOld, Datums ProjNew, ref PointD Ptr)
        {
            if (ProjOld == Datums.WGS84)
            {
                Ptr = ConvertWGS84ToProjection(Ptr, ProjNew).Coordonnees;
            }
            else
            {
                Ptr = ConvertProjectionToWGS84(ProjOld, new PointProjection(Ptr));
            }
        }
        /// <summary> ouvre le formulaire associé à la configuration de l'application </summary>
        private void OuvrirConfiguration()
        {
            int NumEcran = PartagerSettings.NUM_ECRAN;
            // on demande à l'utilisateur le site sur lequel il va travailler et sur quel écran
            using (Form R = new Configuration())
            {
                R.ShowDialog(this);
                if (R.DialogResult == DialogResult.OK)
                {
                    InitialiserAffichageCarte(false);
                    // si on change d'écran 
                    if (NumEcran != PartagerSettings.NUM_ECRAN)
                    {
                        DimensionsEcranSupport = Screen.AllScreens[PartagerSettings.NUM_ECRAN].Bounds;
                        InitialiserRegionAffichageCarte(WindowState == FormWindowState.Maximized);
                    }
                }
            }
        }
        /// <summary> ouvre le formulaire lié aux informations générales de l'application </summary>
        private void OuvrirAPropos()
        {
            MessageInformation = NumFCGP + CrLf + "en date du " + DateVersionFCGP + CrLf +
                "Faire des Cartes à partir de GéoPortail (Regrouper)" + CrLf +
                "Contact : " + Contact + CrLf +
                "Dimensions Ecran : " + DimensionsEcranSupport.Width + "*" + DimensionsEcranSupport.Height + CrLf +
                "Memoire dispo : " + (GetTotalMemory / Math.Pow(1024d, 3d)).ToString("0.00") + " Go, Nb Processeurs : " + Environment.ProcessorCount;
            AfficherInformationEtLien(this, "Visitez le site FCGP", "https://fcgp.e-monsite.com");
        }
        /// <summary> affiche un des coins de la sélection au centre de l'affichage </summary>
        /// <param name="Coin"> numéro du coin de la sélection que l'on veut voir afficher au centre de l'affichage </param>
        private void AfficherCoinSelection(Keys Coin)
        {
            var Curseur = Cursor;
            Cursor = Cursors.WaitCursor;
            var R = RegroupementEncours;
            var Selection = R.SelectionGeoref;

            // on récupère les coordonnées virtuelles du nouveau site
            var CoordonneesCoin = default(Point);
            switch (Coin)
            {
                case Keys.NumPad0:
                case Keys.D0:
                    {
                        CoordonneesCoin = new Point(Selection.Left, Selection.Top);
                        break;
                    }
                case Keys.NumPad1:
                case Keys.D1:
                    {
                        CoordonneesCoin = new Point(Selection.Right, Selection.Top);
                        break;
                    }
                case Keys.NumPad2:
                case Keys.D2:
                    {
                        CoordonneesCoin = new Point(Selection.Right, Selection.Bottom);
                        break;
                    }
                case Keys.NumPad3:
                case Keys.D3:
                    {
                        CoordonneesCoin = new Point(Selection.Left, Selection.Bottom);
                        break;
                    }
            }
            // on le recentre par rapport à l'affichage
            Pt0Affichage.X = CoordonneesCoin.X - (int)Math.Round(TailleAffichage.Width / 2d / ZoomAffichage);
            Pt0Affichage.Y = CoordonneesCoin.Y - (int)Math.Round(TailleAffichage.Height / 2d / ZoomAffichage);
            // on met à jour le tampon avec l'image de la carte
            ChargerRegroupement(CoordonneesCoin, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage);
            AffichageCarte.Invalidate();
            // on positionne le curseur au centre de l'affichage soit le coin choisi
            Cursor.Position = AffichageCarte.PointToScreen(new Point(TailleAffichage.Width / 2, TailleAffichage.Height / 2));
            MAJ_ZoneInfoCoordonnees();
            Cursor = Curseur;
        }
        private void ModifierSelection(Keys Touche)
        {
            var Selection = RegroupementEncours.SelectionGeoref;
            int Value;
            switch (Touche)
            {
                case Keys.X:
                    {
                        Value = Selection.X;
                        switch (ModifierKeys)
                        {
                            case Keys.Shift: // décale en ajoutant le nb de pixels d'une tuile du serveur
                                {
                                    Selection.X = Value + NbPixelsTuile;
                                    break;
                                }
                            case Keys.Control: // décale en enlèvant le nb de pixels d'une tuile du serveur
                                {
                                    Selection.X = Value - NbPixelsTuile;
                                    break;
                                }
                            case Keys.Alt: // arrondi au pixel de début de la tuile serveur la plus proche. Permet de démarrer sur une valeur connue
                                {
                                    Selection.X = (int)Math.Round(Value / (double)NbPixelsTuile) * NbPixelsTuile;
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.Y:
                    {
                        Value = Selection.Y;
                        switch (ModifierKeys)
                        {
                            case Keys.Shift: // décale en ajoutant le nb de pixels d'une tuile du serveur
                                {
                                    Selection.Y = Value + NbPixelsTuile;
                                    break;
                                }
                            case Keys.Control: // décale en enlèvant le nb de pixels d'une tuile du serveur
                                {
                                    Selection.Y = Value - NbPixelsTuile;
                                    break;
                                }
                            case Keys.Alt: // arrondi au pixel de début de la tuile serveur la plus proche. Permet de démarrer sur une valeur connue
                                {
                                    Selection.Y = (int)Math.Round(Value / (double)NbPixelsTuile) * NbPixelsTuile;
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.L:
                    {
                        Value = Selection.Width;
                        switch (ModifierKeys)
                        {
                            case Keys.Shift: // arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                                {
                                    Selection.Width = (Value / DimTuile + 1) * DimTuile;
                                    break;
                                }
                            case Keys.Control: // arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                                {
                                    Selection.Width = (Value / DimTuile - 1) * DimTuile;
                                    break;
                                }
                            case Keys.Alt: // arrondi le nb de pixels aux dimensions du nb de tuiles du fichier tuile le plus proche. Permet de démarrer sur une valeur connue
                                {
                                    Selection.Width = (int)Math.Round(Value / (double)DimTuile) * DimTuile;
                                    break;
                                }
                        }

                        break;
                    }
                case Keys.H:
                    {
                        Value = Selection.Height;
                        switch (ModifierKeys)
                        {
                            case Keys.Shift: // arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                                {
                                    Selection.Height = (Value / DimTuile + 1) * DimTuile;
                                    break;
                                }
                            case Keys.Control: // arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                                {
                                    Selection.Height = (Value / DimTuile - 1) * DimTuile;
                                    break;
                                }
                            case Keys.Alt: // arrondi le nb de pixels aux dimensions du nb de tuiles du fichier tuile le plus proche. Permet de démarrer sur une valeur connue
                                {
                                    Selection.Height = (int)Math.Round(Value / (double)DimTuile) * DimTuile;
                                    break;
                                }
                        }

                        break;
                    }
            }
            RegroupementEncours.SelectionGeorefToSelectionReferencement(Selection);
            AffichageCarte.Invalidate();
            MAJ_ZoneInfoRegroupement();
        }
        #endregion
        #region Listes de Choix
        /// <summary> ouvre la liste de choix associée à un LabelListe </summary>
        private void LabelListes_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            ComboBox C = (ComboBox)L.Tag;
            if (RegroupementEncours is not null)
                C.DroppedDown = true;
        }
        /// <summary> change les couleurs d'un Label avec menu à l'entrée de la souris sur la surface d'un label avec menu </summary>
        private void LabelListesMenus_MouseEnter(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            L.BackColor = Color.FromArgb(255, 229, 241, 251);
            L.Refresh();
        }
        /// <summary> change les couleurs d'un Label avec menu à la sortie de la souris sur sa surface </summary>
        private void LabelListesMenus_MouseLeave(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            L.BackColor = Color.Transparent;
            LabelCoordonnees.Refresh();
        }
        /// <summary> change l'affichage des coordonnées du pointeur de souris </summary>
        private void ListeChoixTypeCoordonnees_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegrouperSettings.INDICE_TYPE_COORDONNEES = ListeChoixTypeCoordonnees.SelectedIndex;
            MAJ_ZoneInfoCoordonnees();
            AffichageCarte.Select();
        }
        /// <summary> change l'affichage des coordonnées du pointeur de souris </summary>
        private void ListeChoixDimensions_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegrouperSettings.TYPE_DIMENSIONS = ListeChoixDimensions.SelectedIndex;
            LabelDimensions.Text = ListeChoixDimensions.SelectedItem.ToString();
            MAJ_ZoneInfoRegroupement();
            AffichageCarte.Select();
        }
        /// <summary> mise à jour de la liste déroulante des types de coordonnées du pointeur de souris </summary>
        private void MAJ_TypeCoordonnees()
        {
            var SystemeType = RegroupementEncours.SystemeType;
            SiteCarto = SystemeType.SiteCarto;
            Datum = DatumSiteWeb((int)SiteCarto);
            Echelle = SystemeType.Niveau.Echelle;
            IndiceEchelle = NiveauDetailCartographique.EchelleToSiteIndiceEchelle(SiteCarto, Echelle);
            ListeChoixTypeCoordonnees.Items.Clear();
            ListeChoixTypeCoordonnees.Items.Add(DatumsLibelles[(int)Datum]);
            ListeChoixTypeCoordonnees.Items.AddRange(CoordType);
            int Couche = (int)EchelleToCouche(Echelle);
            ListeChoixTypeCoordonnees.Items.Add("TUILE Couche N°" + Couche);
            ListeChoixTypeCoordonnees.SelectedIndex = RegrouperSettings.INDICE_TYPE_COORDONNEES;
            PosMouse = PixelsSiteToPixelsEcran(CentreAffichage);
        }
        #endregion
    }
}