using static FCGP.Commun;

namespace FCGP
{
    /// <summary> tampon support de l'affichage de la visue. Les données afichées sont stockées ici </summary>
    internal class TamponVisue : IDisposable
    {
        /// <summary> le tableau d'octets traité en tant que bitMap (image)</summary>
        internal Bitmap Image { get; private set; }
        /// <summary> Nb d'octets du tampon</summary>
        internal int NbBits
        {
            get
            {
                return Tampon.NbBytes;
            }
        }
        /// <summary> tableau d'octets du tampon</summary>
        internal byte[] Bits
        {
            get
            {
                return Tampon.Bits;
            }
        }
        /// <summary> index du 1er octet du tampon dans le tableau</summary>
        internal int IndexBits
        {
            get
            {
                return Tampon.IndexByte;
            }
        }
        /// <summary> pointeur sur le 1er octet du tampon dans le tableau</summary>
        internal IntPtr BitPtr
        {
            get
            {
                return Tampon.BitPtr;
            }
        }
        /// <summary> Region représentée par le tampon exprimée en unité du monde virtuel</summary>
        internal Rectangle Georef { get; private set; }
        internal Point Pt0
        {
            get
            {
                return _Pt0;
            }
            set
            {
                _Pt0 = value;
                Georef = new Rectangle(_Pt0, Surface);
            }
        }
        internal int Stride { get; private set; }
        internal int NbLignes { get; private set; }
        internal Size Surface { get; private set; }
        /// <summary> Pt0 de la region représentée par le tampon exprimée en unité du monde virtuel</summary>
        private Point _Pt0;
        /// <summary> tampon servant pour l'initialisation du tampon avec une couleur </summary>
        private readonly SharedPinnedByteArray Tampon;
        /// <summary> représente le décalge en octets par rapport au début du tampon de RaR </summary>
        private int IndexDepart(Rectangle Rar)
        {
            return (Rar.Y - _Pt0.Y) * Stride + (Rar.X - _Pt0.X) * NbOctetsPixel;
        }
        private List<Carte> Cartes;
        private CarteIntersectTampon CartePrincipale;
        /// <summary> liste des cartes servant pour le remplissage des tampons </summary>
        internal List<Carte> ListeCartes
        {
            set
            {
                Cartes = value;
            }
        }
        /// <summary> réserve en mémoire le nb d'octects nécéssaires pour le stockage d'une image ayant la surface indiquée </summary>
        /// <param name="SurfGeoref"> surface de l'image exprimée en pixels</param>
        /// <param name="ReserveBase"> faut'il utiliser un tampon d'octets déja existant en memoire </param>
        /// <param name="LC">Liste des cartes servant au remplissage du tampon</param>
        internal TamponVisue(Size SurfGeoref, SharedPinnedByteArray Reserve, List<Carte> LC, string NomTampon)
        {
            Surface = SurfGeoref;
            Stride = StrideImage(Surface.Width);
            NbLignes = Surface.Height;
            Cartes = LC;
            CartePrincipale = new CarteIntersectTampon();
            Tampon = new SharedPinnedByteArray(Stride, NbLignes, Reserve, NomTampon);
            Image = new Bitmap(Surface.Width, Surface.Height, Stride, FormatPixel, Tampon.BitPtr);
        }
        internal TamponVisue(Size SurfGeoref, string NomTampon)
        {
            Surface = SurfGeoref;
            Stride = StrideImage(Surface.Width);
            NbLignes = Surface.Height;
            CartePrincipale = new CarteIntersectTampon();
            Tampon = new SharedPinnedByteArray(Stride, NbLignes, default, NomTampon);
            Image = new Bitmap(Surface.Width, Surface.Height, Stride, FormatPixel, Tampon.BitPtr);
        }
        /// <summary> remplit la region du tampon avec des cartes en fonction de leur Georef commun </summary>
        /// <param name="RaR">Region du tampon à remplir</param>
        /// <param name="Couleur"> couleur de fond du tampon </param>
        internal void RemplirTampon(Rectangle RaR, Color Couleur)
        {
            // avant de remplir il faut mettre la couleur par défaut au cas où il n'y aurait pas la totalité du remplissage par les cartes
            if (RaR == Georef)
            {
                Tampon.ClearColor(Couleur);
            }
            else
            {
                Tampon.ClearColor(Couleur, IndexDepart(RaR), StrideImage(RaR.Width), RaR.Height);
            }
            if (Cartes is not null) // on peut remplir le tampon avec des cartes
            {
                var Cs = RenvoyerCartesIntersectWithTampon(RaR);
                var ListeTache = new Task[Cs.Count];
                for (int Cpt = 0, loopTo = Cs.Count - 1; Cpt <= loopTo; Cpt++)
                {
                    var C = Cs[Cpt];
                    ListeTache[Cpt] = Task.Run(() => LireCarteDansTampon(RaR, C, 4));
                }
                Task.WaitAll(ListeTache);
            }
        }
        #region Procedures pour remplir un tampon
        /// <summary> Transfert de tout ou partie d'une image au format BMP dans un tampon en fonction du georef commun</summary>
        /// <param name="CIT">carte à transférer d'un fichier vers le tampon avec son Georef</param>
        /// <param name="RaR">Georef du tampon</param>
        /// <param name="NbSousCartes"> si > 1, lance la lecture de la carte avec des boucles en parallele </param>
        private void LireCarteDansTampon(Rectangle RaR, CarteIntersectTampon CIT, int NbSousCartes = 4)
        {
            int StrideCarte = CIT.LargeurCarte;
            // où démarre la lecture dans la carte. Dans la carte les lignes sont stockées dans le sens inverse. On commence par la dernière ligne
            long IndexOctetsCarteTotal = (long)((CIT.PositionGeoref.Bottom - CIT.Intersect.Bottom) * StrideCarte + (CIT.Intersect.Left - CIT.PositionGeoref.Left) * NbOctetsPixel + OffsetDataBMP);
            // nb d'octets à lire dans l'image et à écrire dans le tampon = largeur de l'intersection carte tampon * nb octets du format graphique (3)
            int NbOctectsAlire = CIT.Intersect.Width * NbOctetsPixel;
            // où démarre l'écriture dans le tampon
            int IndexOctetsTamponTotal = IndexDepart(RaR) + (RaR.Height + CIT.Intersect.Bottom - RaR.Bottom - 1) * Stride + (CIT.Intersect.Left - RaR.Left) * NbOctetsPixel + IndexBits;
            int HauteurTotale = CIT.Intersect.Height;
            if (NbSousCartes == 1)
            {
                int HauteurPartielle = CIT.Intersect.Height;
                LireSousCarteDansSousTampon(CIT.Chemin, StrideCarte, IndexOctetsCarteTotal, NbOctectsAlire, IndexOctetsTamponTotal, HauteurTotale, HauteurPartielle, 0);
            }
            else
            {
                int HauteurPartielle = (CIT.Intersect.Height - 1) / NbSousCartes + 1;
                Parallel.For(0, NbSousCartes, (IndexSousCarte) =>
                                               LireSousCarteDansSousTampon(CIT.Chemin, StrideCarte, IndexOctetsCarteTotal, NbOctectsAlire,
                                                                           IndexOctetsTamponTotal, HauteurTotale, HauteurPartielle, IndexSousCarte
                                                                          ));
            }
        }
        /// <summary> transfert d'une sous partie de la carte à partir d'un fichier de type .bmp  vers le tampon</summary>
        /// <param name="CheminCarte">chemin de la carte</param>
        /// <param name="StrideCarte">nb d'octets à lire pour passer à la ligne suivante de la carte</param>
        /// <param name="IndexOctetsCarteTotal">ou commence dans la carte la lecture de la première ligne à transférer</param>
        /// <param name="NbOctectsAlire">combien d'octets doit'on lire sur la ligne en cours</param>
        /// <param name="IndexOctetsTamponTotal">ou commence dans le tampon l'écriture de la première ligne à transférer</param>
        /// <param name="HauteurTotale">Hauteur totale à lire et écrire</param>
        /// <param name="HauteurPartielle">Hauteur partielle à lire et écrire</param>
        /// <param name="IndexSousCarte">index de la sous-carte en cours</param>
        private void LireSousCarteDansSousTampon(string CheminCarte, int StrideCarte, long IndexOctetsCarteTotal, int NbOctectsAlire, int IndexOctetsTamponTotal, int HauteurTotale, int HauteurPartielle, int IndexSousCarte)
        {
            // nb de lignes à lire et à écrire
            int HauteurLue = IndexSousCarte == 0 ? 0 : HauteurPartielle * IndexSousCarte;
            int HauteurSousCarte = HauteurLue + HauteurPartielle > HauteurTotale ? HauteurTotale - HauteurLue : HauteurPartielle;
            // où démarre la lecture dans la sous-carte
            long IndexOctetsSousCarte = IndexOctetsCarteTotal + (IndexSousCarte == 0 ? 0 : IndexSousCarte * HauteurPartielle * StrideCarte);
            // où démarre l'écriture dans le sous-tampon
            int IndexOctetsSousTampon = IndexOctetsTamponTotal - (IndexSousCarte == 0 ? 0 : IndexSousCarte * HauteurPartielle * Stride);
            using (var FluxBMP = new FileStream(CheminCarte, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (int BoucleLigne = 0, loopTo = HauteurSousCarte - 1; BoucleLigne <= loopTo; BoucleLigne++)
                {
                    FluxBMP.Position = IndexOctetsSousCarte;
                    FluxBMP.Read(Bits, IndexOctetsSousTampon, NbOctectsAlire);
                    IndexOctetsSousCarte += StrideCarte;
                    IndexOctetsSousTampon -= Stride;
                }
            }
        }
        /// <summary> sert à l'initialisation de la recherche récursive et aux tests initiaux </summary>
        /// <returns>la liste des parties de carte associées au tampon qui recouvrent le tampon sans chevauchement</returns>
        private List<CarteIntersectTampon> RenvoyerCartesIntersectWithTampon(Rectangle Rar)
        {
            // pour contenir le résultat des cartes qui ont une intersection avec le tampon ainsi que la surface correspondante
            var ResultatListeSurfaceCarte = new List<CarteIntersectTampon>();
            // est ce que la carte de l'historique, celle qui a été la première au dernier remplissage, contient la totalité de la surface du tampon
            if (Rar == Rectangle.Intersect(CartePrincipale.PositionGeoref, Rar))
            {
                // si oui il n'y a plus rien à faire, si ce n'est l'indiquer dans le résultat
                ResultatListeSurfaceCarte.Add(new CarteIntersectTampon()
                {
                    Intersect = Rar,
                    IntersectTot = true,
                    Chemin = CartePrincipale.Chemin,
                    PositionGeoref = CartePrincipale.PositionGeoref,
                    LargeurCarte = CartePrincipale.LargeurCarte
                });
            }
            else
            {
                // sinon il faut décomposer le résultat. on initialise la liste des cartes servant de paramètre
                var ListeCarteIntersectTampon = RequeterCartesIntersectWithTampon(Rar);
                // si il y a au moins une carte qui intersect avec le tampon, la plus grande surface est toujours la première de la liste
                if (ResultatListeSurfaceCarte.Count > 0)
                {
                    CartePrincipale = ListeCarteIntersectTampon[0];
                }
                else
                {
                    CartePrincipale = new CarteIntersectTampon();
                }
                // on lance la recherche récursive, on récupère le résultat et on le trie par ordre croissant du nom de la carte
                ResultatListeSurfaceCarte = TrouverCartesIntersectWithRectangles(Rar, ListeCarteIntersectTampon).OrderBy(C => C.Chemin).ToList();
                // on fusionne les enregistrements des cartes qui apparaissent plusieurs fois afin de n'avoir qu'un seul acces au fichier .bmp de la carte
                string NomCarte = "";
                for (int Cpt = ResultatListeSurfaceCarte.Count - 1; Cpt >= 0; Cpt -= 1)
                {
                    // si le nom de la carte est égal à celui de l'enregistrement précédent c'est que la carte est présente plusieurs fois
                    if ((ResultatListeSurfaceCarte[Cpt].Chemin ?? "") == (NomCarte ?? ""))
                    {
                        // on les fusionne en faisant une union de leur surface
                        ResultatListeSurfaceCarte[Cpt].Intersect = Rectangle.Union(ResultatListeSurfaceCarte[Cpt].Intersect, ResultatListeSurfaceCarte[Cpt + 1].Intersect);
                        // et en supprimant l'enregistrement précédent
                        ResultatListeSurfaceCarte.RemoveAt(Cpt + 1);
                    }
                    else
                    {
                        // on stocke le nom de la carte pour le prochain tour
                        NomCarte = ResultatListeSurfaceCarte[Cpt].Chemin;
                    }
                }
            }
            return ResultatListeSurfaceCarte;
        }
        /// <summary> Trouve l'ensemble des cartes qui ont une surface en intersection avec la surface virtuelle représentée par le tampon et les renvoie sous forme de liste 
        /// il s'agit d'une simple requête</summary>
       	private List<CarteIntersectTampon> RequeterCartesIntersectWithTampon(Rectangle Region)
        {
            IEnumerable<CarteIntersectTampon> F = from Carte C in Cartes
                                                  where !(C.RegionVirtuelle.Right < Region.Left || C.RegionVirtuelle.Left > Region.Right ||
                                                          C.RegionVirtuelle.Top > Region.Bottom || C.RegionVirtuelle.Bottom < Region.Top)
                                                  let Intersect = C.CroiserAvecRegionVirtuelle(Region)
                                                  let Surface = Intersect.Width * Intersect.Height
                                                  let IntersectTot = Surface == (Region.Width * Region.Height)
                                                  orderby IntersectTot descending, Surface descending
                                                  select new CarteIntersectTampon
                                                  {
                                                      Intersect = Intersect,
                                                      Chemin = C.CheminFluxLecture,
                                                      IntersectTot = IntersectTot,
                                                      PositionGeoref = C.RegionVirtuelle,
                                                      LargeurCarte = C.LargeurOctets
                                                  };
            return F.ToList();
        }

        /// <summary> Trouve l'ensemble des cartes qui ont une surface en intersection avec la surface virtuelle représentée par le rectangle et les renvoie sous forme de liste 
        /// il s'agit d'une simple requête</summary>
        /// <param name="LCIT"> Liste de cartes ayant été selectionnées auparavant </param>
        /// <param name="RaR"> Rectangle de sélection </param>
        private static List<CarteIntersectTampon> RequeterCartesIntersectWithRectangle(Rectangle RaR, List<CarteIntersectTampon> LCIT)
        {
            //trouve les cartes qui intersectent la surface du rectangle
            IEnumerable<CarteIntersectTampon> F = from CarteIntersectTampon L in LCIT
                                                  where !(L.Intersect.Right < RaR.Left || L.Intersect.Left > RaR.Right || L.Intersect.Top > RaR.Bottom || L.Intersect.Bottom < RaR.Top)
                                                  let Intersect = L.IntersectWithTampon(RaR)
                                                  let Surface = Intersect.Width * Intersect.Height
                                                  let IntersectTot = Surface == (RaR.Width * RaR.Height)
                                                  orderby IntersectTot descending, Surface descending
                                                  select new CarteIntersectTampon
                                                  {
                                                      Intersect = Intersect,
                                                      IntersectTot = IntersectTot,
                                                      Chemin = L.Chemin,
                                                      PositionGeoref = L.PositionGeoref,
                                                      LargeurCarte = L.LargeurCarte
                                                  };
            return F.ToList();
        }

        /// <summary>procédure récursive. renvoie les cartes qui ont une intersection avec le tampon mais par rapport à une liste initale
        /// la surface d'intersection est ajustée pour qu'elles ne se chevauche pas en éliminant les doublons et en priorisant les cartes ayant les plus grandes surfaces d'intersection </summary>
        /// <param name="RaR">surface à remplir</param>
        /// <param name="LCIT">liste des cartes qui ont une intersection avec le tampon</param>
        private List<CarteIntersectTampon> TrouverCartesIntersectWithRectangles(Rectangle RaR, List<CarteIntersectTampon> LCIT)
        {
            // pour le retour de la fonction
            var ResultatListesurfaceCarte = new List<CarteIntersectTampon>();
            // si il y a au moins une carte qui intersecte avec le tampon on met à jour le résulat
            if (LCIT.Count > 0)
            {
                // il s'agit de la première carte
                ResultatListesurfaceCarte.Add(LCIT[0]);
                // Si la carte ne remplit pas toute la surface du tampon il faut continuer le processus de décompositon 
                // de la surface restante en rectangle
                if (!LCIT[0].IntersectTot)
                {
                    // on prend la surface initiale du tampon
                    var DejaRemplit = LCIT[0].Intersect;
                    // on a plus besoin de la carte que l'on a pris en résultat
                    LCIT.RemoveAt(0);
                    // et on trouve de 1 à 4 rectangles qui composent la surface restant à remplir
                    var ListeRectangle = new List<Rectangle>();
                    // Si les rectangles sont vides c'est qu'il n'y a pas d'intersection
                    if (RaR.Bottom != DejaRemplit.Bottom)
                        ListeRectangle.Add(new Rectangle(RaR.Left, DejaRemplit.Bottom, RaR.Width, RaR.Bottom - DejaRemplit.Bottom));
                    if (RaR.Right != DejaRemplit.Right)
                        ListeRectangle.Add(new Rectangle(DejaRemplit.Right, DejaRemplit.Top, RaR.Right - DejaRemplit.Right, DejaRemplit.Height));
                    if (RaR.Left != DejaRemplit.Left)
                        ListeRectangle.Add(new Rectangle(RaR.Left, DejaRemplit.Top, DejaRemplit.Left - RaR.Left, DejaRemplit.Height));
                    if (RaR.Top != DejaRemplit.Top)
                        ListeRectangle.Add(new Rectangle(RaR.Left, RaR.Top, RaR.Width, DejaRemplit.Top - RaR.Top));
                    // pour chaque rectangle trouvé on repète le processus jusqu'à ce qu'il n'y ai plus de carte dans la liste ou que l'on
                    // trouve une carte qui fasse toute la surface à remplir
                    foreach (Rectangle R in ListeRectangle)
                        // on continue la recherche récursive et on récupère le résultat
                        ResultatListesurfaceCarte.AddRange(TrouverCartesIntersectWithRectangles(R, RequeterCartesIntersectWithRectangle(R, LCIT)));
                }
            }
            return ResultatListesurfaceCarte;
        }
        #endregion
        #region IDisposable Support
        private bool disposedValue; // Pour détecter les appels redondants
                                    // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    /// <summary> libère si besoin la mémoire réservé par le tampon</summary>
                    Image.Dispose();
                    Tampon.Dispose();
                }
            }
            disposedValue = true;
        }
        // Ce code est ajouté par Visual Basic pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(disposing As Boolean) ci-dessus.
            Dispose(true);
        }
        #endregion
        /// <summary>  classe qui permet de faire la relation entre une carte et un tampon  : intersection des 2 Georeférencement en monde virtuel </summary>
        private class CarteIntersectTampon
        {
            /// <summary>pour stocker la surface qui représente l'intersection d'une carte et du tampon virtuel</summary>
            internal Rectangle Intersect;
            /// <summary>true si la surface d'intersection est égale à la surface du tampon</summary>
            internal bool IntersectTot;
            /// <summary>Géoréférencement du tampon dans le système virtuel associé à l'échelle en cours</summary>
            internal Rectangle PositionGeoref;
            /// <summary>Nom de la carte yc le chemin complet, permet l'acces au fichier</summary>
            internal string Chemin;
            /// <summary>nb d'octets à lire pour passer à la ligne suivante : nb bits * nb bits du format graphique --> multiple de 4</summary>
            internal int LargeurCarte;
            /// <summary>  trouve la surface qui intersecte la carte et le tampon passé en paramètre </summary>
            /// <param name="Region">Rectangle en monde virtuel désignant la région à tester</param>
            /// <returns>la surface qui intersect la surface de la carte avec la surface du tampon passé en paramètre. Peut être vide</returns>
            internal Rectangle IntersectWithTampon(Rectangle Region)
            {
                return Rectangle.Intersect(Intersect, Region);
            }
        }
    }
}