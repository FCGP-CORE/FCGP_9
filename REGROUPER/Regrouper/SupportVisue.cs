using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.NativeMethods;

namespace FCGP
{
    internal static class SupportVisue
    {
        /// <summary> brosse semi transparente pour le dessin de la zone de sélection </summary>
        private readonly static SolidBrush Brosse_Selection = new(Color.FromArgb(48, 64, 64, 64));
        /// <summary>indique si l'etat du déplacement du tampon d'affichage est en cours</summary>
        private static EtatsTampon EtatDecalage;
        /// <summary>tache effectuant le déplacement du tampon d'affichage </summary>
        private static Task TacheDecalage;
        /// <summary> tampon servant pour l'initialisation du tampon avec une couleur </summary>
        private static Color CouleurVisue;
        /// <summary> surface de l'écran sur lequel s'éxécute FCGP exprimée en pixels </summary>
        private static Size SurfaceEcran;
        /// <summary> surface de la partie horizontale à remplir sur à la demande de décalage exprimée en monde virtuel </summary>
        private static Rectangle GV_Horizontal;
        /// <summary> surface de la partie verticale à remplir sur à la demande de décalage exprimée en monde virtuel </summary>
        private static Rectangle GV_Vertical;
        /// <summary>definit la zone au delà de laquelle il faut demander le remplissage du tampon futur, depend du coefzoom</summary>
        private static Rectangle GeorefRemplissage
        {
            get
            {
                return Rectangle.Inflate(GeorefEncours, (int)Math.Round(-SurfaceEcran.Width * CoefGeorefRemplissage), (int)Math.Round(-SurfaceEcran.Height * CoefGeorefRemplissage));
            }
        }
        /// <summary>definit la région du tampon en cours exprimée en unité du monde virtuel</summary>
        private static Rectangle GeorefEncours
        {
            get
            {
                return Tampons[Encours].Georef;
            }
        }
        /// <summary>definit le pt0 de la region du tampon encours exprimée en unité du monde virtuel</summary>
        private static Point Pt0Encours
        {
            get
            {
                return Tampons[Encours].Pt0;
            }
        }
        /// <summary> surface de la region représentée par les tampons, encours et futur, exprimée en unité du monde virtuel </summary>
        private static Size SurfaceGeorefEncours
        {
            get
            {
                return Tampons[Encours].Surface;
            }
        }
        /// <summary> nb d'octects d'une ligne du tampon </summary>
        private static int StrideEncours
        {
            get
            {
                return Tampons[Encours].Stride;
            }
        }
        /// <summary> nb de lignes de pixels  des tampons encours et futur </summary>
        private static int NbLignesEncours
        {
            get
            {
                return Tampons[Encours].NbLignes;
            }
        }
        /// <summary>definit le pt0 de la region du tampon encours exprimée en unité du monde virtuel</summary>
        private static Bitmap ImageEncours
        {
            get
            {
                return Tampons[Encours].Image;
            }
        }
        /// <summary> tableau qui contient les 2 tampons d'affichage </summary>
        private static FCGP.TamponVisue[] Tampons;
        /// <summary> indice du tamponsupportvisue qui est à l'affichage</summary>
        private static int Encours;
        /// <summary> indice du TamponSupportVisue qui sera à l'affichage après permutation avec le tampon encours</summary>
        private static int Futur;
        /// <summary>Etat du tampon d'affichage</summary>
        private enum EtatsTampon : int
        {
            /// <summary>Après MAJ de l'origine du tampon, la visue est à l'interieur de la zone de demande de remplissage et de décalage donc pas d'action</summary>
            Aucun = 0,
            /// <summary>demande de calcul du GeorefDécalage et de remplissage des tampons auxillaires</summary>
            DemandeDecalage = 1,
            /// <summary>la demande de décalage (copie des 3 tampon) a été traitée</summary>
            DemandeDecalageFinie = 2
        }
        /// <summary>coefZoom de la visue qui sert de coef multiplicateur au niveau de l'affichage</summary>
        private static double CoefZoomAffichage;
        /// <summary>coef multiplicateur au niveau de la surfarce de demande de remplissage</summary>
        private static double CoefGeorefRemplissage;
        /// <summary>coef multiplicateur au niveau des tampons horizontal et vertical</summary>
        private static double CoefDimensions;
        /// <summary> surface de la visue exprimée en pixel </summary>
        private static Size SurfaceAffichage;
        /// <summary> surface de la visue exprimée en pixel zoom, le zoom etant le rapport  entre affichage et visueZoom</summary>
        private static Size VisueZoom;
        /// <summary>Largeur du tampon vertical, depend du coefzoom</summary>
        private static int NbPixelsX;
        /// <summary>Hauteur du tampon horizontal du coefzoom</summary>
        private static int NbPixelsY;
        /// <summary> cette procédure calcule pour les 2 tampons auxiliaires la surface, en coordonnées du monde virtuel de l'échelle encours,
        /// qu'il faut remplir en fonction du ou des  déplacements demandés</summary>
        /// <param name="GeorefVisue">surface de l'affichage en coordonnées du monde virtuel</param>
        /// <remarks> en sortie soit GeoRef futur est égal à Geroef encours et cela indique qu'il n'y a pas de déplacement demandé
        /// soit le GeorRef futur n'est pas égal à GeroefVirtuel et cela indique la surface ou les surfaces qu'il faudra remplir avant de réaliser 
        /// la permutation des tampons</remarks>
        private static int CalculerDeplacement(Rectangle GeorefVisue)
        {
            var NbDeplacements = default(int);
            // cherche le ou les décalages
            GV_Vertical = Rectangle.Empty;
            GV_Horizontal = Rectangle.Empty;
            Tampons[Futur].Pt0 = Pt0Encours;
            // 1 si deplacement Est, -1 si déplacement  Ouest et 0 si pas de déplacement
            int DecalX = GeorefVisue.Right > GeorefRemplissage.Right ? 1 : GeorefVisue.Left < GeorefRemplissage.Left ? -1 : 0;
            // 1 si deplacement Sud, -1 si déplacement  Nord et 0 si pas de déplacement
            int DecalY = GeorefVisue.Bottom > GeorefRemplissage.Bottom ? 1 : GeorefVisue.Top < GeorefRemplissage.Top ? -1 : 0;
            if (DecalX != 0 || DecalY != 0)  // il y a au moins un déplacement de décalage
            {
                // mise à jour du Pt0 du tampon futur
                Tampons[Futur].Pt0 = new Point(Pt0Encours.X + NbPixelsX * DecalX, Pt0Encours.Y + NbPixelsY * DecalY);
                // mise à jour du tampon horizontal si il y a un déplacement dans ce sens
                if (DecalX != 0)
                {
                    NbDeplacements += 1;
                    GV_Horizontal.X = DecalX > 0 ? GeorefEncours.Right : GeorefEncours.Left - NbPixelsX;
                    GV_Horizontal.Y = GeorefEncours.Top + (DecalY > 0 ? NbPixelsY : 0);
                    GV_Horizontal.Size = new Size(NbPixelsX, GeorefEncours.Height - (DecalY != 0 ? NbPixelsY : 0));
                }
                // mise à jour du tampon vertical si il y a un déplacement dans ce sens
                if (DecalY != 0)
                {
                    NbDeplacements += 1;
                    GV_Vertical.X = GeorefEncours.Left + DecalX * NbPixelsX;
                    GV_Vertical.Y = DecalY > 0 ? GeorefEncours.Bottom : GeorefEncours.Top - NbPixelsY;
                    GV_Vertical.Size = new Size(GeorefEncours.Width, NbPixelsY);
                }
            }
            return NbDeplacements;
        }
        /// <summary> remplit les 3 parties du tampon futur</summary>
        private static void DecalerTampon(int NbDeplacements)
        {
            // dimensionne le tableau des taches à lancer de manière asynchrone
            var T = new Task[NbDeplacements + 1];
            var NbTaches = default(int);
            if (GV_Horizontal != Rectangle.Empty)
            {
                T[NbTaches] = Task.Factory.StartNew(() => Tampons[Futur].RemplirTampon(GV_Horizontal, CouleurVisue));
                NbTaches += 1;
            }
            if (GV_Vertical != Rectangle.Empty)
            {
                T[NbTaches] = Task.Factory.StartNew(() => Tampons[Futur].RemplirTampon(GV_Vertical, CouleurVisue));
                NbTaches += 1;
            }
            T[NbTaches] = Task.Factory.StartNew(() => CopierTampon());
            Task.WaitAll(T);
        }
        /// <summary> demande le decalage si besoin du tampon futur </summary>
        /// <param name="Visue"></param>
        private static void DemanderDecalage(Rectangle Visue)
        {
            EtatDecalage = EtatsTampon.DemandeDecalage;
            int NbDeplacements = CalculerDeplacement(Visue);
            if (NbDeplacements > 0)
            {
                DecalerTampon(NbDeplacements);
                PermutterTampon();
            }
            EtatDecalage = EtatsTampon.Aucun;
        }
        /// <summary> permute entre les 2 tampons si besoin. cette action finie la demande de décalage</summary>
        private static void PermutterTampon()
        {
            //int Tempo = Futur;
            //Futur = Encours;
            //Encours = Tempo;
            (Encours, Futur) = (Futur, Encours);
            EtatDecalage = EtatsTampon.Aucun;
        }
        /// <summary> calcule certaines variables de la visue qui dépendent du CoefZoom de la visue ou de la surface de la visue</summary>
        /// <param name="CoefZoom">coeficient zoom de la visue de 2.0 à 0.5</param>
        /// <param name="VisueAffichage"> surface de la visue exprimée en pixels</param>
        private static void CalculerVariablesSupportVisue(double CoefZoom, Size VisueAffichage)
        {
            if (CoefZoomAffichage != CoefZoom)
            {
                CoefZoomAffichage = CoefZoom;
                CoefGeorefRemplissage = 2d - (CoefZoom > 1d ? 0d : 1d - CoefZoom);
                CoefDimensions = 1d + (CoefZoom >= 1d ? 0d : (1d - CoefZoom) * 2d);
                NbPixelsX = (int)Math.Round(SurfaceEcran.Width * CoefDimensions);
                NbPixelsY = (int)Math.Round(SurfaceEcran.Height * CoefDimensions);
                VisueZoom = new Size((int)Math.Round(VisueAffichage.Width / CoefZoom), (int)Math.Round(VisueAffichage.Height / CoefZoom));
            }
            if (SurfaceAffichage != VisueAffichage)
            {
                SurfaceAffichage = VisueAffichage;
                VisueZoom = new Size((int)Math.Round(VisueAffichage.Width / CoefZoom), (int)Math.Round(VisueAffichage.Height / CoefZoom));
            }
        }
        /// <summary> copie du tampon encours vers le tampon futur de la partie non modifiée suite au décalage</summary>
        private static void CopierTampon()
        {
            int DecalX = (Tampons[Futur].Pt0.X - Pt0Encours.X) * NbOctetsPixel;
            int DecalY = (Tampons[Futur].Pt0.Y - Pt0Encours.Y) * StrideEncours;
            int NbOctectsTransfert = StrideEncours - Math.Abs(DecalX);
            var IndexSource = Tampons[Encours].BitPtr + (DecalY > 0 ? DecalY : 0) + (DecalX > 0 ? DecalX : 0);
            var IndexDestination = Tampons[Futur].BitPtr + (DecalY < 0 ? -DecalY : 0) + (DecalX < 0 ? -DecalX : 0);
            Parallel.For(0, NbLignesEncours - Math.Abs(Tampons[Futur].Pt0.Y - Pt0Encours.Y),
                             (Cpt) =>
                             {
                                 IntPtr IndexEncours = IndexSource + Cpt * StrideEncours;
                                 IntPtr IndexFutur = IndexDestination + Cpt * StrideEncours;
                                 CopierMemoire(IndexFutur, IndexEncours, NbOctectsTransfert);
                             });
        }
        /// <summary> dimensionne les tampons en fonction de la surface d'ecran sur lequel FCGP tourne. Remplace le constructeur d'une classe </summary>
        /// <param name="SurfEcran"> surface de l'écran</param>
        /// <param name="CouleurFond">couleur de la visue quand il n'y a pas de carte</param>
        internal static void InitialiserSupportVisue(Size SurfEcran, Color CouleurVisue)
        {
            // il n'y a pas de vérification sur la taille de réservebase ce qui peut poser des problèmes sur les écrans de dimensions > 1920*1200
            Tampons = new TamponVisue[2];
            SurfaceEcran = SurfEcran;
            var Georef = new Size(SurfaceEcran.Width * 6, SurfaceEcran.Height * 6);
            Tampons[0] = new TamponVisue(Georef, "Visue_1") { Pt0 = Point.Empty };
            Tampons[1] = new TamponVisue(Georef, "Visue_2") { Pt0 = Point.Empty };
            ChangerCouleurVisue(CouleurVisue);
            CalculerVariablesSupportVisue(1d, SurfEcran);
        }
        /// <summary>libère l'espace memoire des tampons si besoin</summary>
        internal static void CloturerVisue()
        {
            Tampons[0].Dispose();
            Tampons[1].Dispose();
        }
        /// <summary> affichage la carte sur la visue en fonction des 3 paramètres. appel par la méthode paint de la visue si il y a un déplacement demandé</summary>
        /// <param name="CoefZoom">coeficient zoom de la visue de 2.0 à 0.5</param>
        /// <param name="VisueAffichage"> surface de la visue exprimée en pixels</param>
        /// <param name="VisuePtOrigine">Pt0 de la visue exprimée en corrdonnéées virtuel</param>
        /// <param name="e"></param>
        internal static void AfficherTamponVisue(double CoefZoom, Size VisueAffichage, Point VisuePtOrigine, Rectangle Selection, Graphics e)
        {
            CalculerVariablesSupportVisue(CoefZoom, VisueAffichage);
            // calcul du georef de la visue
            var GeorefVisue = new Rectangle(VisuePtOrigine, VisueZoom);
            if (!GeorefEncours.Contains(GeorefVisue))
            {
                try
                {
                    if (EtatDecalage == EtatsTampon.Aucun)
                    {
                        DemanderDecalage(GeorefVisue);
                    }
                    else // If EtatDecalage = EtatsTampon.DemandeDecalage Then
                    {
                        TacheDecalage.Wait();
                    }
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "L1P4");
                }
            }
            AfficherTamponvisue(VisueAffichage, VisuePtOrigine, Selection, e);
            // on ne peut lancer qu'une tache
            if (!GeorefRemplissage.Contains(GeorefVisue) & EtatDecalage == EtatsTampon.Aucun)
            {
                // on calcule le ou les déplacements et on demande le ou les remplissage de tampon
                TacheDecalage = Task.Factory.StartNew(() => DemanderDecalage(GeorefVisue), TaskCreationOptions.PreferFairness);
            }
        }
        /// <summary> affichage la carte sur la visue en fonction des 2 paramètres.  appel par la méthode paint de la visue si il n'y a pas de déplacement demandé </summary>
        /// <param name="VisueAffichage"> surface de la visue exprimée en pixels</param>
        /// <param name="VisuePtOrigine">Pt0 de la visue exprimée en corrdonnéées virtuel</param>
        /// <param name="e"></param>
        internal static void AfficherTamponvisue(Size VisueAffichage, Point VisuePtOrigine, Rectangle Selection, Graphics e)
        {
            // calcul du décalage en pixel de l'image du tampon par rapport à la surface d'affichage
            int DecalageTamponX = VisuePtOrigine.X - Pt0Encours.X;
            int DecalageTamponY = VisuePtOrigine.Y - Pt0Encours.Y;
            // La dimension de la visue reste toujours identique, on joue sur la largeur et la hauteur de l'image à afficher pour générer l'effet zoom
            e.DrawImage(ImageEncours, new Rectangle(0, 0, VisueAffichage.Width, VisueAffichage.Height), new Rectangle(DecalageTamponX, DecalageTamponY, VisueZoom.Width, VisueZoom.Height), GraphicsUnit.Pixel);
            if (!Selection.IsEmpty)
                AfficherSelection(VisuePtOrigine, Selection, e);
        }
        /// <summary> affiche un rectangle représentant la séléction en cours </summary>
        /// <param name="VisuePtOrigine"> Pt0 de l'affichage </param>
        /// <param name="Selection"> sélection en coordonnées virtuelles </param>
        /// <param name="e"> Graphics sur lequel dessiner </param>
        internal static void AfficherSelection(Point VisuePtOrigine, Rectangle Selection, Graphics e)
        {
            var Pt0 = VisuePtOrigine;
            int DecalageTamponX = Selection.X - Pt0.X;
            int DecalageTamponY = Selection.Y - Pt0.Y;
            Selection.Location = new Point((int)Math.Round(DecalageTamponX * CoefZoomAffichage), (int)Math.Round(DecalageTamponY * CoefZoomAffichage));
            var S = Selection.Size;
            Selection.Size = new Size((int)Math.Round(S.Width * CoefZoomAffichage), (int)Math.Round(S.Height * CoefZoomAffichage));
            e.FillRectangle(Brosse_Selection, Selection);
        }
        /// <summary> Charge une échelle et remplit le tampon encours </summary>
        /// <param name="VisuePtOrigine">point exprimé en coordonnées virtuelles qui sera au centre de la visue</param>
        /// <param name="ListeCartes">liste des cartes composants l'échelle</param>
        /// <param name="CoefZoom">CoefZoom demandé pour l'affichage</param>
        /// <param name="VisueAffichage">dimensions en pixel de la visue</param>
        internal static void ChargerRegroupement(Point VisuePtOrigine, List<Carte> ListeCartes, double CoefZoom, Size VisueAffichage, Color CoulVisue)
        {
            // initialisation des différentes variables de supportvisue
            CalculerVariablesSupportVisue(CoefZoom, VisueAffichage);
            Encours = 0;
            Futur = 1;
            Tampons[Encours].ListeCartes = ListeCartes;
            Tampons[Futur].ListeCartes = ListeCartes;
            // détermination du pt(0) du tampon et donc du Georef en sachant que PTInit représente le centre du tampon
            Tampons[Encours].Pt0 = new Point(VisuePtOrigine.X - SurfaceGeorefEncours.Width / 2, VisuePtOrigine.Y - SurfaceGeorefEncours.Height / 2);
            ChangerCouleurVisue(CoulVisue);
            EtatDecalage = EtatsTampon.Aucun;
        }
        /// <summary> initialise la ligne de couleur avec la couleur de fond demandée et met à jour le tampon </summary>
        /// <param name="CouleurFond">couleur pour l'initialisation </param>
        internal static void ChangerCouleurVisue(Color CouleurFond)
        {
            CouleurVisue = CouleurFond;
            Tampons[Encours].RemplirTampon(GeorefEncours, CouleurVisue);
        }
    }
}