using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.NativeMethods;
using static FCGP.ServeurCarto;

namespace FCGP
{
    /// <summary> classe servant de support à l'affichage de la carte </summary>
    internal class AffichageCarte : IDisposable
    {
        /// <summary> couleur de base de l'affichage avant le dessin des tuiles </summary>
        internal Color CouleurFond { get; set; }
        /// <summary> surface des fonds de plan exprimée en tuiles dela couche des fonds de plan </summary>
        internal Rectangle SurfaceFond
        {
            get
            {
                return new Rectangle(Pt0.IndexTuile, Taille);
            }
        }
        /// <summary> surface des pentes exprimée en tuiles de la couche des pentes </summary>
        internal Rectangle SurfacePentes
        {
            get
            {
                return new Rectangle(Pt0Pentes.IndexTuile, TaillePentes);
            }
        }
        /// <summary> Décalage en pixels entre Pt0 de l'affichage et le Pt0 du tampon de fond. Calculer à chaque décalage effectif du tampon puis mis à jour
        /// avec le déplacement de la carte par la souris ou les touches. Le décalage doit toujours être compris dans la surface d'une tuile de fond </summary>
        internal Point DecalageTampon
        {
            get
            {
                return (Point)_DecalageTampon;
            }
        }
        /// <summary> Décalage en pixels entre Pt0.X de l'affichage et le Pt0.X du tampon de fond. Calculer à chaque décalage effectif du tamon puis mis
        /// à jour avec le déplacement de la carte par la souris ou les touches. Le décalage X doit toujours être entre 0 et la largeur d'une tuile -1 </summary>
        internal int DecalageTamponX
        {
            get
            {
                return _DecalageTampon.Width;
            }
            set
            {
                _DecalageTampon.Width = value;
            }
        }
        /// <summary> Décalage en pixels entre PT0.Y de l'affichage et le Pt0.Y du tampon de fond. Calculer à chaque décalage effectif du tamon puis mis 
        /// à jour avec le déplacement de la carte par la souris ou les touches. Le décalage Y doit toujours être entre 0 et la hauteur d'une tuile -1 </summary>
        internal int DecalageTamponY
        {
            get
            {
                return _DecalageTampon.Height;
            }
            set
            {
                _DecalageTampon.Height = value;
            }
        }
        /// <summary> coordonnées en monde virtuel du centre de l'affichage </summary>
        internal Point CentreAffichage
        {
            get
            {
                return Point.Add(LocationAffichage, QuartVisue);
            }
        }
        /// <summary> coordonnées en monde virtuel (pixels) du coin haut gauche de la visue. </summary>
        internal Point LocationAffichage
        {
            get
            {
                return Point.Add(Pt0.Pt0Tuile, _DecalageTampon);
            }
        }
        /// <summary> indique si l'initialisation de l'affichage s'est bien déroulée </summary>
        internal bool IsOk { get; private set; }
        /// <summary> initialise l'affichage </summary>
        /// <param name="ReserveTampon"> Espace mémoire pour stocker des données sous forme binaire </param>
        /// <param name="TailleTampon"> dimensions maximum accordé au stockage de l'image de fond </param>
        /// <param name="CouleurFond"> couleur initiale de l'image de fond </param>
        internal AffichageCarte(SharedPinnedByteArray ReserveTampon, Size TailleTampon, Color CouleurFond)
        {
            Encours = 0;
            Futur = 1;
            this.CouleurFond = CouleurFond;
            // dimensionne 2 tampons d'affichage pour le fond
            SurfaceAffichage = new SharedPinnedByteArray[2];
            // réserve l'espace mémoire pour les 2 tampons associé à la visue
            SurfaceAffichage[Encours] = new SharedPinnedByteArray(TailleTampon.Width, TailleTampon.Height, ReserveTampon, "Affichage_1");
            if (!SurfaceAffichage[Encours].IsOk)
                return;
            SurfaceAffichage[Futur] = new SharedPinnedByteArray(TailleTampon.Width, TailleTampon.Height, ReserveTampon, "Affichage_2");
            if (!SurfaceAffichage[Futur].IsOk)
                return;
            IsOk = true;
        }
        /// <summary> initialise les dimensions réelles de l'affichage en fonction de la surface de la visue </summary>
        /// <param name="TailleVisue"> taille en pixels de la surface d'affichage </param>
        /// <param name="Coordonnees"> coordonnées exprimées en pixels du monde virtuel du centre de l'affichage </param>
        /// <param name="FlagInit"> indique que l'application n'a pas encore finie d'être initialisée </param>
        internal void ChangerTaille(Size TailleVisue, PointG Coordonnees, bool FlagInit)
        {
            var IndexTuile_Encours = Pt0.IndexTuile;
            int Stride_Encours = StrideImageFond;
            // Recherche de la taille des tampons de l'affichage
            Taille = new Size((int)Math.Ceiling((TailleVisue.Width + NbPixelsTuile) / (double)NbPixelsTuile), (int)Math.Ceiling((TailleVisue.Height + NbPixelsTuile) / (double)NbPixelsTuile));
            // initialisation de variables utiles durant l'utilisation de l'affichage
            NbLignesFond = Taille.Height * NbPixelsTuile;
            NbPixelsLigneFond = Taille.Width * NbPixelsTuile;
            QuartVisue = new Size(TailleVisue.Width / 2, TailleVisue.Height / 2);
            StrideImageFond = StrideImage(NbPixelsLigneFond);
            // calcule les coordonnées du Pt0 de la visue
            Pt0 = PointG.Substract(Coordonnees, QuartVisue);
            // au depart l'offset du tampon et le décalage sont égaux. offset est immuable alors que le décalage varie avec le déplacement
            _DecalageTampon = Pt0.Offset;
            if (IsAffichagePentes)
                CalculerTamponPentes();
            EchelleDessin = new DessinEchelle(ref IsDessinEchelle);
            if (!FlagInit)
            {
                // on RAZ le tampon futur afin de voir les nouvelles tuiles arrivées
                SurfaceAffichage[Futur].ClearColor(CouleurFond);
                // on copie la partie du tampon en cours qui se trouve encore sur le tampon futur afin d'éviter les scintillements
                CopierTampon(IndexTuile_Encours.X - Pt0.IndexTuile.X, IndexTuile_Encours.Y - Pt0.IndexTuile.Y, Stride_Encours, StrideImageFond);
                (Encours, Futur) = (Futur, Encours);
            }
            else
            {
                // on RAZ les tampons
                SurfaceAffichage[Encours].ClearColor(CouleurFond);
                SurfaceAffichage[Futur].ClearColor(CouleurFond);
            }
        }
        /// <summary> calcul des coordoonnées pixels avec la nouvelle échelle, positionne l'image actuelle en fonction de la nouvelle échelle </summary>
        /// <param name="LocationCurseur"> Position du curseur de souris sur la surface de la Visue en pixels écran qui devient le centre du nouvel affichage </param>
        /// <param name="DeltaIndiceEchelles"> Ecart entre les indices de l'échelle actuelle et de l'échelle future </param>
        internal void ChangerCouche(Point LocationCurseur, int DeltaIndiceEchelles)
        {
            // calcul du facteur de modification de l'image actuelle. Si deltaEchelle < 0 alors agrandissement l'image actuelle par fact sinon réduction par 1/fact
            double FactZoom = Serveur.FacteurEchelles(Pt0.IndiceEchelle, DeltaIndiceEchelles);
            // calcul des coordonnées de la souris ou du centre de la visue avec le nouveau zoom fonction du deltazoom
            var CoordDebut = Point.Add(LocationAffichage, new Size(LocationCurseur));
            var CoordFin = new PointG(Pt0.IndiceEchelle + DeltaIndiceEchelles, new Point((int)Math.Round(CoordDebut.X * FactZoom), (int)Math.Round(CoordDebut.Y * FactZoom)));
            var Pt0Fin = PointG.Substract(CoordFin, LocationCurseur);
            // rectangle servant pour l'affichage de l'image actuelle agrandie ou réduite à l'endroit de la nouvelle image pour donner l'impression de continuté
            Rectangle RectSource;
            Rectangle RectDest;
            // calcul des rectangles permettant l'agrandissement ou la réduction lors du dessin de l'image dans le tampon futur
            if (DeltaIndiceEchelles < 0)
            {
                // dans ce cas il faut inverser le debut et la fin ainsi que le facteur de zoom pour le calcul du rectangle zoomé
                FactZoom = 1d / FactZoom;
                // le rectangle Source est une partie de l'image source
                RectSource = TrouverRectangleZoom(Pt0Fin.IndexTuile, Pt0.IndexTuile, FactZoom);
                // qui sera dessiné agrandi à la taille du tampon de fond
                RectDest = new Rectangle(Point.Empty, new Size(NbPixelsLigneFond, NbLignesFond));
            }
            else
            {
                // le rectangle source est la totalité de l'image source
                RectSource = new Rectangle(Point.Empty, new Size(NbPixelsLigneFond, NbLignesFond));
                // qui sera dessiné réduit à une partie de la taille du tampon de fond
                RectDest = TrouverRectangleZoom(Pt0.IndexTuile, Pt0Fin.IndexTuile, FactZoom);
            }
            // on RAZ le tampon futur afin de voir les nouvelles tuiles arrivées
            SurfaceAffichage[Futur].ClearColor(CouleurFond);
            // on dessine sur le tampon futur la partie de l'image agrandie ou réduite qui se trouve à l'endroit de la nouvelle image
            using (var Bmp = new Bitmap(NbPixelsLigneFond, NbLignesFond, StrideImageFond, FormatPixel, SurfaceAffichage[Futur].BitPtr))
            {
                using (var G = Graphics.FromImage(Bmp))
                {
                    G.DrawImage(ImageFond, RectDest, RectSource, GraphicsUnit.Pixel);
                }
            }
            // échange des surfaces d'affichage
            (Encours, Futur) = (Futur, Encours);
            // change les coordonnées du Pt0 avec les coordonnées du nouveau zoom 
            Pt0 = Pt0Fin;
            _DecalageTampon = Pt0.Offset;
            if (IsAffichagePentes)
                CalculerTamponPentes();
            EchelleDessin = new DessinEchelle(ref IsDessinEchelle);
        }
        /// <summary> calcule la surface en tuile Pentes qu'il faut télécharger pour correspondre à la surface visue actuelle </summary>
        internal void CalculerTamponPentes()
        {
            int IndiceEchellePentes = Serveur.IndiceEchellePentes;
            int IndiceEchelleAffichage = Pt0.IndiceEchelle;
            int DeltaIndiceEchelle = IndiceEchellePentes - IndiceEchelleAffichage;
            double FactZoom = Serveur.FacteurEchelles(IndiceEchelleAffichage, DeltaIndiceEchelle);
            double InvFactZoom = 1d / FactZoom;
            // l'emploi du Round est obligatoire pour le site SM qui n'a pas que des coefs de zoom de 2 pour chaque niveaux
            DimensionsTuilePentes = (int)Math.Round(NbPixelsTuile * InvFactZoom, 0);
            var LocationPentes = new Point((int)Math.Round(Pt0.Location.X * FactZoom), (int)Math.Round(Pt0.Location.Y * FactZoom));
            Pt0Pentes = new PointG(IndiceEchellePentes, LocationPentes);
            TaillePentes = new Size((int)Math.Ceiling((QuartVisue.Width * 2 + Pt0Pentes.Offset.Width * InvFactZoom) / DimensionsTuilePentes),
                                    (int)Math.Ceiling((QuartVisue.Height * 2 + Pt0Pentes.Offset.Height * InvFactZoom) / DimensionsTuilePentes));
            DecalageTamponPentes = new Point(DeltaIndiceEchelle == 0 ? 0 : Pt0.Offset.Width - (int)Math.Round(Pt0Pentes.Offset.Width * InvFactZoom, 0),
                                             DeltaIndiceEchelle == 0 ? 0 : Pt0.Offset.Height - (int)Math.Round(Pt0Pentes.Offset.Height * InvFactZoom, 0));
        }
        /// <summary> décale le tampon en mettant à jour indexDeb et Decale X ou Y et en copiant une partie du tampon encours sur le tampon futur </summary>
        internal void DecalerTampon()
        {
            // on calcule le nb entier de colonne et de rangée à décaler, pour la copie du tampon encours sur le tampon futur
            int DecalageCol = (int)Math.Floor(_DecalageTampon.Width / (double)NbPixelsTuile);
            int DecalageRow = (int)Math.Floor(_DecalageTampon.Height / (double)NbPixelsTuile);
            // on RAZ le tampon futur afin de voir les nouvelles tuiles arrivées
            SurfaceAffichage[Futur].ClearColor(CouleurFond);
            // on copie la partie du tampon en cours qui se trouve encore sur le tampon futur afin d'éviter les scintillements
            CopierTampon(DecalageCol, DecalageRow);
            // on procède au décalage du point Pt0 du tampon et de l'affichage
            var DecalageReelVisue = Size.Subtract(_DecalageTampon, Pt0.Offset);
            Pt0.Add(DecalageReelVisue);
            // on ajuste le décalage de la visue en fonction
            _DecalageTampon = Pt0.Offset;
            // on échange les surfaces d'affichage
            (Encours, Futur) = (Futur, Encours);
            if (IsAffichagePentes)
                CalculerTamponPentes();
        }
        /// <summary> Renvoie une image qui représente le fond de plan avec éventuellement la couche des pentes </summary>
        /// <param name="FlagRemplirTampon"> indique qu'une demande de remplissage du tampon a été demandée </param>
        internal async Task<Bitmap> ImageAffichage(bool FlagRemplirTampon)
        {
            if (Serveur.NbDemandesAffichage > 0 || FlagRemplirTampon)
            {
                return await CreerImageAffichageAsync();
            }
            else
            {
                return ImageFond;
            }
        }
        /// <summary> permet la libération des ressources http et memoire </summary>
        public void Dispose()
        {
            Dispose(true);
            // nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(this);
        }
        #region private
        private async Task<Bitmap> CreerImageAffichageAsync()
        {
            int DecalPixelsX = default, DecalPixelsY = default;
            Rectangle Surface;
            IntPtr ReserveBitPtr;
            Bitmap Image;
            IntPtr IndexTamponPtr;
            string CoucheTxt;
            Surface = SurfaceFond;
            ReserveBitPtr = SurfaceAffichage[Encours].BitPtr;
            CoucheTxt = CoucheFond.ToString("00");
            // decalage d'une tuile vers le bas
            int DecalY = NbPixelsTuile * StrideImageFond;
            int DecalTuile = 0;
            int IndexTuile = 0;
            SurfaceAffichage[Futur].ClearColor(CouleurFond);
            // remplir le tampon selectionné avec les tuiles correspondantes à partir du cache
            for (int Row = Surface.Top, loopTo = Surface.Bottom - 1; Row <= loopTo; Row++)
            {
                for (int Col = Surface.Left, loopTo1 = Surface.Right - 1; Col <= loopTo1; Col++)
                {
                    byte[] BytesTuile = await Serveur.Cache.LireOctetsTuileAsync(Col, Row, CoucheTxt);
                    if (BytesTuile is not null)
                    {
                        IndexTamponPtr = ReserveBitPtr + DecalPixelsX + DecalPixelsY;
                        using (var TuileImage = new Bitmap(new MemoryStream(BytesTuile)))
                        {
                            var BD = TuileImage.LockBits(new Rectangle(0, 0, NbPixelsTuile, NbPixelsTuile), ImageLockMode.ReadOnly, FormatPixel);
                            var IndexTuilePtr = BD.Scan0;
                            for (int Cpt = 0; Cpt <= NbPixelsTuile - 1; Cpt++)
                            {
                                CopierMemoire(IndexTamponPtr, IndexTuilePtr, NbOctetsDecalXTuile);
                                IndexTamponPtr += StrideImageFond;
                                IndexTuilePtr += NbOctetsDecalXTuile;
                            }
                            TuileImage.UnlockBits(BD);
                        }
                    }
                    DecalPixelsX += NbOctetsDecalXTuile;
                    IndexTuile += 1;
                }
                DecalPixelsX = 0;
                DecalPixelsY += DecalY;
                DecalTuile += Surface.Width;
                IndexTuile = DecalTuile;
            }
            Image = new Bitmap(NbPixelsLigneFond, NbLignesFond, StrideImageFond, FormatPixel, ReserveBitPtr);
            if (IsAffichagePentes)
                await AjouterPentes(Image);
            return Image;
        }
        /// <summary> Ajoute la couche des Pentes à l'image de l'affichage</summary>
        private async Task AjouterPentes(Bitmap Image)
        {
            var DecalageTuileDestination = DecalageTamponPentes;
            var TailleTuileDestination = new Size(DimensionsTuilePentes, DimensionsTuilePentes);
            int IndexTuile = 0;
            Rectangle TuileImageDestination;
            var SurfacePentes = new Rectangle(Pt0Pentes.IndexTuile, TaillePentes);
            using (var G = Graphics.FromImage(Image))
            {
                for (int Row = SurfacePentes.Top, loopTo = SurfacePentes.Bottom - 1; Row <= loopTo; Row++)
                {
                    for (int Col = SurfacePentes.Left, loopTo1 = SurfacePentes.Right - 1; Col <= loopTo1; Col++)
                    {
                        byte[] BytesTuile = await Serveur.Cache.LireOctetsTuileAsync(Col, Row);
                        if (BytesTuile is not null)
                        {
                            using (var TuileImage = new Bitmap(new MemoryStream(BytesTuile)))
                            {
                                TuileImageDestination = new Rectangle(DecalageTuileDestination, TailleTuileDestination);
                                G.DrawImage(TuileImage, TuileImageDestination, 0, 0, NbPixelsTuile, NbPixelsTuile, GraphicsUnit.Pixel, ImageAttributsPentes);
                            }
                        }
                        IndexTuile += 1;
                        DecalageTuileDestination.X += DimensionsTuilePentes;
                    }
                    DecalageTuileDestination.X = DecalageTamponPentes.X;
                    DecalageTuileDestination.Y += DimensionsTuilePentes;
                }
            }
        }
        /// <summary> rectangle qui permet d'afficher une couche serveur avec une échelle différente de celle de la couche
        /// tout en conservant la même coordonnées indiquer pour la couche (centre affichage ou click souris </summary>
        /// <param name="IndexTuileDeb"> Indextuile du Pt0 de la couche de début </param>
        /// <param name="IndexTuileFin"> Indextuile du Pt0 de la couche de fin </param>
        /// <param name="FactZoom"> facteur de Zoom à appliquer à la couche début </param>
        private Rectangle TrouverRectangleZoom(Point IndexTuileDeb, Point IndexTuileFin, double FactZoom)
        {
            var Rect = new Rectangle(new Point((int)Math.Round((IndexTuileDeb.X * FactZoom - IndexTuileFin.X) * NbPixelsTuile),
                                               (int)Math.Round((IndexTuileDeb.Y * FactZoom - IndexTuileFin.Y) * NbPixelsTuile)),
                       new Size((int)Math.Round(NbPixelsLigneFond * FactZoom),
                                (int)Math.Round(NbLignesFond * FactZoom)));
            return Rect;
        }
        /// <summary> copie en partie le tampon encours sur le tampon futur. Cas du changement de taille de la fenêtre </summary>
        /// <param name="DecalageCol"> nb de colonnes de tuiles à décaler </param>
        /// <param name="DecalageRow"> nb de rangées de tuiles à décaler </param>
        /// <param name="StrideEncours">Nb de pixels par ligne du tampon encours </param>
        /// <param name="StrideFutur"> Nb de pixels par ligne du tampon futur </param>
        private void CopierTampon(int DecalageCol, int DecalageRow, int StrideEncours, int StrideFutur)
        {
            int NbOctectsTransfert;
            IntPtr IndexEncours, IndexFutur;
            if (DecalageCol < 0 || DecalageRow < 0) // La taille de la fenêtre a diminuée, le décalage se fait par rapport au tampon encours
            {
                NbOctectsTransfert = StrideFutur;
                IndexEncours = SurfaceAffichage[Encours].BitPtr - DecalageCol * NbOctetsDecalXTuile - DecalageRow * NbPixelsTuile * StrideEncours;
                IndexFutur = SurfaceAffichage[Futur].BitPtr;
            }
            else // la taille de la fenêtre a augmentée, le décalage se fait par rapport au tampon futur
            {
                NbOctectsTransfert = StrideEncours;
                IndexEncours = SurfaceAffichage[Encours].BitPtr;
                IndexFutur = SurfaceAffichage[Futur].BitPtr + DecalageCol * NbOctetsDecalXTuile + DecalageRow * NbPixelsTuile * StrideFutur;
            }
            Parallel.For(0, NbLignesFond - Math.Abs(DecalageRow * NbPixelsTuile),
                         (Cpt) =>
                         {
                             var IndexSource = IndexEncours + Cpt * StrideEncours;
                             var IndexDestination = IndexFutur + Cpt * StrideFutur;
                             CopierMemoire(IndexDestination, IndexSource, NbOctectsTransfert);
                         });
        }
        /// <summary> copie en partie le tampon encours sur le tampon futur. Cas du décalage de la carte dans la même fenêtre </summary>
        /// <param name="DecalageCol"> nb de colonnes de tuiles à décaler </param>
        /// <param name="DecalageRow"> nb de rangées de tuiles à décaler </param>
        private void CopierTampon(int DecalageCol, int DecalageRow)
        {
            int DecalOctetsX = DecalageCol * NbOctetsDecalXTuile;
            int DecalOctetsY = DecalageRow * NbPixelsTuile * StrideImageFond;
            int NbOctectsTransfert = StrideImageFond - Math.Abs(DecalOctetsX);
            var IndexEncours = SurfaceAffichage[Encours].BitPtr + (DecalOctetsX > 0 ? DecalOctetsX : 0) + (DecalOctetsY > 0 ? DecalOctetsY : 0);
            var IndexFutur = SurfaceAffichage[Futur].BitPtr + (DecalOctetsX > 0 ? 0 : -DecalOctetsX) + (DecalOctetsY > 0 ? 0 : -DecalOctetsY);
            Parallel.For(0, NbLignesFond - Math.Abs(DecalageRow * NbPixelsTuile),
                         (Cpt) =>
                         {
                             var IndexSource = IndexEncours + Cpt * StrideImageFond;
                             var IndexDestination = IndexFutur + Cpt * StrideImageFond;
                             CopierMemoire(IndexDestination, IndexSource, NbOctectsTransfert);
                         });
        }
        /// <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                SurfaceAffichage[Encours]?.Dispose();
                SurfaceAffichage[Futur]?.Dispose();
            }
            disposed = true;
        }
        /// <summary> c'est l'image des fonds de plan de l'affichage (tampon encours) </summary>
        private Bitmap ImageFond
        {
            get
            {
                return new Bitmap(NbPixelsLigneFond, NbLignesFond, StrideImageFond, FormatPixel, SurfaceAffichage[Encours].BitPtr);
            }
        }
        /// <summary> dimensions d'une tuile pentes vue de la couche Fond. il y a un coef de zoom </summary>
        private int DimensionsTuilePentes;
        /// <summary> décalage du tampon des pentes par rapport au tampon du fond exprimé en pixels. C'est toujours 0 ou une dimension de tuilePentes </summary>
        private Point DecalageTamponPentes;
        /// <summary> nb d'octets par ligne du tampon de fond </summary>
        private int StrideImageFond;
        /// <summary> tampons pour l'affichage </summary>
        private readonly SharedPinnedByteArray[] SurfaceAffichage;
        /// <summary> indique si la carte a été détruite</summary>
        private bool disposed;
        /// <summary> nb de lignes composant l'affichage du fond </summary>
        private int NbLignesFond;
        /// <summary> nb de pixels par ligne composant l'affichage du fond </summary>
        private int NbPixelsLigneFond;
        /// <summary> information initiale (changerTaille ou decalerTampon) du point haut gauche de l'affichage du fond </summary>
        private PointG Pt0;
        /// <summary> information initale (changerTaille ou decalerTampon) du point haut gauche de l'affichage des pentes </summary>
        private PointG Pt0Pentes;
        /// <summary> décalage de l'affichage par rapport au tampon </summary>
        private Size _DecalageTampon;
        /// <summary> Nb de colonnes et de rangées de l'affichage exprimée en nb de tuiles </summary>
        private Size Taille;
        /// <summary> Nb de colonnes et de rangées des pentes exprimée en nb de tuiles </summary>
        private Size TaillePentes;
        /// <summary> index du tampon d'affichage encours d'utilisation </summary>
        private int Encours;
        /// <summary> index du tampon d'affichage qui sera utiliser après une opération de décalage ou d'agrandissement ou de réduction </summary>
        private int Futur;
        /// <summary> quart de la surface de la visue </summary>
        internal Size QuartVisue;
        /// <summary> Couche sur le serveur correspondant à l'échelle en cours du tampon de fond </summary>
        internal byte CoucheFond;
        /// <summary> Echelle des fonds de plan. </summary>
        internal Enumerations.Echelles Echelle;
        /// <summary> indique si la couche encours du tampon de fond peut afficher les pentes </summary>
        internal bool CoucheFondIspentes;
        #endregion
    }
}