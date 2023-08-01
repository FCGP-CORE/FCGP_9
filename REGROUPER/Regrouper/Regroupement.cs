using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.ObjetRectangle;
using static FCGP.Regrouper;
using static FCGP.Settings;
using static FCGP.SystemeCartographique;

namespace FCGP
{
    /// <summary>permet d'indiquer à l'appelant comment c'est passer l'action demandée sur l'échelle</summary>
    internal enum VerifierRegroupement : int
    {
        /// <summary>La création de l'échelle est ok</summary>
        OK = 0,
        /// <summary>Le fichier géoref n'est pas valide</summary>
        FichierGeoref_Non_Valide = 1,
        /// <summary>L'échelle est déja existante</summary>
        Regroupement_Déja_Existant = 2,
        /// <summary>L'échelle est inexistante</summary>
        Regroupement_Inexistant = 3,
        /// <summary>Les cartes du répertoire coorespondantes à l'échelle sont déja importées</summary>
        Repertoire_Déja_Importé = 4,
        /// <summary> aucune carte valide n'a été trouvée pour ce regroupement dans ce répertoire </summary>
        Aucune_Carte_Valide = 5
    }
    /// <summary> regroupe toutes les cartes situées dans un même répertoire et ayant le même système cartographique ou compatible 
    /// Site+Echelle et datum peut être différent. Ex : GF 015 avec WebMercator ou WGS84 ou Lambert93 </summary>
    internal sealed class Regroupement
    {
        #region Elements Regroupement
        /// <summary> numéro unique représantant le site carto et le domtom. Sert pour la sélection relle associée à un SiteCarto+DomTom </summary>
        internal int NumSite { get; private set; }
        /// <summary>Cle du système cartographique </summary>
        /// <remarks> donné par la carte qui initialise le regroupement </remarks>
        internal string ClefEchelle
        {
            get
            {
                return SystemeType.ClefEchelle;
            }
        }
        /// <summary> datum associé au regroupement. Si coordonnées de géoréférencement = DD alors WGS84 sinon C'est un datum Grille </summary>
        /// <remarks> donné par la carte qui initialise le regroupement </remarks>
        internal Datums Datum
        {
            get
            {
                return SystemeType.Projection.Datum;
            }
        }
        /// <summary>données définissant le système de coordonnées du regroupement </summary>
        /// <remarks> donné par la carte qui initialise le regroupement </remarks>
        internal SystemeCartographique SystemeType { get; private set; }
        /// <summary> Cle du regroupement. Chaine unique identifiant Site+DomTom+Echelle, Doit permettre le tri croissant ou décroissant.
        /// Fourni à la demande de création par l'appelant </summary>
        internal string Clef { get; private set; }
        /// <summary> Cartes associées au regroupement </summary>
        /// <remarks> par défaut les cartes sont issues du même répertoire que la carte qui sert à la création du regroupement </remarks>
        internal List<Carte> Cartes { get; private set; }
        /// <summary> nb de cartes du regroupement </summary>
        internal int NbCartes
        {
            get
            {
                return Cartes.Count;
            }
        }
        /// <summary> Région virtuelle couverte par l'ensemble des cartes du regroupement. C'est la plus petite zone rectangulaire qui contient l'ensemble des cartes </summary>
        internal Rectangle RegionVirtuelle { get; private set; }
        /// <summary> Coef du zoom de l'affichage du regroupement </summary>
        internal double CoefZoomAffichageRetour { get; set; }
        /// <summary> coordonnées en monde virtuel qui sera le centre de l'affichage lors de l'affichage du regroupement </summary>
        internal Point Affiche_PositionRetour { get; set; }
        /// <summary> taille en unité virtuelle de la zone de sélection du regroupement en cours </summary>
        internal Rectangle SelectionGeoref
        {
            get
            {
                // On appele la transformation pour obtenir la sélection de référencement associé au site carto </remarks>
                SelectionReferencementToSelectionGeoref();
                return _SelectionGeoref;
            }
        }

        private Point positionGeoref;
        /// <summary> position du pointeur de souris en unité virtuelles lors de la sélection de la carte: Retour AffichePlanEchelle ou AjouterCartes</summary>
        internal Point PositionGeoref
        {
            get
            { return positionGeoref; }
            private set
            { positionGeoref = value; }
        }
        /// <summary>Numéro de carte sélectionnée -1 ou 1 à N cartes : Retour AffichePlanEchelle ou AjouterCartes</summary>
        internal int NumCarteSelection { get; private set; }
        /// <summary> Permet l'ajout de cartes dans la collection de cartes de l'échelle transmise en pramètre </summary>
        /// <param name="FichierGeoref">fichier georef permettant l'initalisation de l'échelle</param>
        /// <param name="NbCartesAjout"> Nb de carte ajoutées à l'échelle </param>
        internal VerifierRegroupement AjouterCartes(string FichierGeoref, ref int NbCartesAjout)
        {
            // trouve le répertoire du fichier
            string Chemin = Path.GetDirectoryName(FichierGeoref);
            // on regarde si le répertoire a déja été importé 
            int IndexRepertoireImporte = RepertoiresImportes.BinarySearch(Chemin);
            // si oui on ressort car le répertoire a déja été importé
            if (IndexRepertoireImporte >= 0)
            {
                NbCartesAjout = 0;
                return VerifierRegroupement.Repertoire_Déja_Importé;
            }
            else
            {
                // si non on insert le répertoire dans la collection en respectant le tri
                IndexRepertoireImporte = IndexRepertoireImporte ^ -1;
                RepertoiresImportes.Insert(IndexRepertoireImporte, Chemin);
            }
            // commence la boucle d'ajout des cartes, on trouve l'ensemble des fichiers georef du répertoire
            var RetCarte = default(VerificationRenvoiCarte);
            NbCartesAjout = Cartes.Count;
            string[] ListeFichierGeoref = Directory.GetFiles(Path.GetDirectoryName(FichierGeoref), "*.GEOREF", SearchOption.TopDirectoryOnly);
            foreach (string Fichier in ListeFichierGeoref)
            {
                // si la carte est valide et correspond à l'échelle actuelle
                var CarteActuelle = Carte.RenvoyerCarte(Fichier, ref RetCarte, TypeSupportCarte.Aucun, false, SystemeType, ChoixFichiersTuiles.Aucun, Cartes.Count);
                // s'agit'il du bon système et de la bonne échelle  
                if (RetCarte == VerificationRenvoiCarte.OK && CarteActuelle.IsOk && CarteActuelle.Format == ImageFormat.Bmp)
                {
                    // si c'est la carte qui sert de point de départ on détermine le numéro et son centre pour l'affichage au retour
                    if ((FichierGeoref ?? "") == (Fichier ?? ""))
                    {
                        // on indique le numéro dans la collection
                        NumCarteSelection = Cartes.Count;
                        // et le point central de la carte
                        positionGeoref.X = CarteActuelle.RegionVirtuelle.X + CarteActuelle.RegionVirtuelle.Width / 2;
                        positionGeoref.Y = CarteActuelle.RegionVirtuelle.Y + CarteActuelle.RegionVirtuelle.Height / 2;
                    }
                    Cartes.Add(CarteActuelle);
                    // on met à jour l'emprise virtuelle représentée par l'ensemble des cartes
                    if (RegionVirtuelle == Rectangle.Empty)
                    {
                        RegionVirtuelle = CarteActuelle.RegionVirtuelle;
                    }
                    else
                    {
                        RegionVirtuelle = Rectangle.Union(RegionVirtuelle, CarteActuelle.RegionVirtuelle);
                    }
                }
            }
            NbCartesAjout = Cartes.Count - NbCartesAjout;
            if (NbCartesAjout == 0)
            {
                return VerifierRegroupement.Aucune_Carte_Valide;
            }
            else
            {
                return VerifierRegroupement.OK;
            }
        }
        /// <summary> affiche le plan de calepinage du regroupement sur une fenêtre qui aura les dimensions indiquées et à bords des bords de la fenêtres </summary>
        /// <param name="Dimension"> dimensions de la fenêtre d'affichage du plan </param>
        /// <param name="Bords"> le plan sera à minima à bords des bors de la fenêtre (padding) </param>
        /// <param name="Affichage">rectangle qui représente les dimensions et la position de l'affichage en monde virtuel
        /// Si rectangle.empty, la représentation de l'affichage de la carte ne sera pas affichée sur le plan</param>
        internal void AfficherCalepinage(Rectangle Affichage, int Dimension, int Bords)
        {
            using (var F = new CalepinageRegroupement() { RegroupementActuel = this })
            {
                SelectionReferencementToSelectionGeoref();
                // mise en place des variables à passer au formulaire MappingAffiche
                RegionAffichage = Affichage;
                // dimensionnement de la fenêtre plan Echelle   
                if (Dimension < 700)
                    Dimension = 700;
                TaillePlanRegroupement = new Size(Dimension, Dimension);
                if (Bords < 10 || Bords > 150)
                    Bords = 35;
                Affiche_Bords = Bords;
                F.ShowDialog(FormApplication);
                SelectionGeorefToSelectionReferencement();
            }
        }
        /// <summary> chaque site peut avoir une sélection qui est commune à tous les regroupement associé au site </summary>
        internal void SelectionReferencementToSelectionGeoref()
        {
            // Si la sélection grille n'a pas changée, la sélection virtuelle non plus
            if (OldSelectionReferencement == SelectionsReferencement[NumSite])
                return;
            OldSelectionReferencement = SelectionsReferencement[NumSite];
            if (OldSelectionReferencement.IsEmpty)
            {
                _SelectionGeoref = Rectangle.Empty;
            }
            else
            {
                var PT0_Virtuel = SystemeType.ConvertirCoordonneesReellesToVirtuelles(OldSelectionReferencement.Pt0);
                var PT2_Virtuel = SystemeType.ConvertirCoordonneesReellesToVirtuelles(OldSelectionReferencement.Pt2);
                _SelectionGeoref = new Rectangle(PT0_Virtuel.X, PT0_Virtuel.Y, PT2_Virtuel.X - PT0_Virtuel.X, PT2_Virtuel.Y - PT0_Virtuel.Y);
            }
        }
        /// <summary> transforme rectangle exprimée en coordonnées virtuelle du systeme cartographique en un rectangle exprimée 
        /// en coordonnées de référencement du systeme cartographique</summary>
        /// <param name="SystemeCarto">sytème cartographique de l'échelle à charger ou à afficher</param>
        /// <param name="Sel">si non null permet de modifier la selection réelles</param>
        internal void SelectionGeorefToSelectionReferencement(Rectangle Sel = default)
        {
            var Selection = Sel == default ? _SelectionGeoref : Sel;
            if (Selection == Rectangle.Empty)
            {
                SelectionsReferencement[NumSite] = RectangleD.Empty;
            }
            else
            {
                var PT0_Virtuel = Selection.Location;
                var PT2_Virtuel = Point.Add(PT0_Virtuel, Selection.Size);
                var PT0_Reel = SystemeType.ConvertirCoordonneesVirtuellesToReelles(PT0_Virtuel);
                var PT2_Reel = SystemeType.ConvertirCoordonneesVirtuellesToReelles(PT2_Virtuel);
                SelectionsReferencement[NumSite] = new RectangleD(PT0_Reel, PT2_Reel);
            }
        }
        /// <summary> initialisation du regroupement </summary>
        /// <param name="Sys"> Système cartographique associé au regroupement (Site, domtom, Echelle) </param>
        private Regroupement(SystemeCartographique Sys, string Cle)
        {
            Clef = Cle;
            SystemeType = Sys;
            RepertoiresImportes = new List<string>(15);
            Cartes = new List<Carte>(50);
            NumSite = SiteDomTomToIndex(SystemeType.SiteCarto, SystemeType.DomTom);
        }
        /// <summary> Contient tous les répertoires dans lequel sont stockés les cartes de la collection pour éviter de les ré-importer </summary>
        private List<string> RepertoiresImportes;
        /// <summary> taille en unité virtuelle de la zone de sélection du regroupement en cours: Retour AffichePlanEchelle</summary>
        private Rectangle _SelectionGeoref;
        private RectangleD OldSelectionReferencement;
        #endregion
        #region Elements Regroupements
        /// <summary> initialisation des regroupements </summary>
        static Regroupement()
        {
            Regroupements = new SortedList<string, Regroupement>();
            // il y a 12 sites carto gérer par fcgp. 5 sites uniques + 7 domstoms associés au site DomTom et chaucun peut avoir ses regroupements et sa sélection 
            SelectionsReferencement = new RectangleD[12];
        }
        /// <summary> taille en unité de reférencement de la zone de sélection du site carto </summary>
        internal static RectangleD SelectionReferencementSiteCarto(int NumSite)
        {
            return SelectionsReferencement[NumSite];
        }
        /// <summary> efface la sélection associée au site carto </summary>
        internal static void EffacerSelectionReferencementSiteCarto(int NumSite)
        {
            SelectionsReferencement[NumSite] = RectangleD.Empty;
        }
        /// <summary> liste des regroupements existants </summary>
        internal static SortedList<string, Regroupement> Regroupements { get; private set; }
        /// <summary> renvoie le nb de regroupements appartenants au même site dans la collection </summary>
        internal static int NbRegroupementsSiteCarto(int NumSite)
        {
            int NbEchelles = 0;
            foreach (KeyValuePair<string, Regroupement> R in Regroupements)
            {
                if (R.Value.NumSite == NumSite)
                    NbEchelles += 1;
            }
            return NbEchelles;
        }
        /// <summary> Verifie si le fichier Georef est valide. Si Ok verifie que le regroupement correspondant n'existe pas. Si il n'existe pas 
        /// on le crée et on charge les cartes du répertoire sinon on envoie un message </summary>
        /// <param name="FichierGeorefInit"> Chemin de la carte d'initialisation. Le système doit être valide et non interpolée</param>
        /// <param name="ClefRegroupement"> Clef du regroupement </param>
        /// <param name="NbCartesAjout"> Nb de carte ajoutées à l'échelle </param>
        internal static VerifierRegroupement CreerRegroupement(string FichierGeorefInit, string ClefRegroupement, ref int NbCartesAjout)
        {
            VerifierRegroupement CreerRegroupementRet = default;
            // si l'échelle existe on renvoie un message d'erreur. L'utilisateur pourra toujour utiliser la fonction ajout d'un repertoire pour ajouter les cartes
            if (Regroupements.ContainsKey(ClefRegroupement))
            {
                return VerifierRegroupement.Regroupement_Déja_Existant;
            }
            var SystemeType = Carte.RenvoyerSystemeCartographique(FichierGeorefInit);
            // dans le cas du regroupement, le système type est ramené à la projection principale du site carto associé, ce qui rend compatible les différentes projections de cartes compatibles
            var DatumPrincipal = ProjectionCartographique.DatumPrincipal(SystemeType.SiteCarto);
            if (SystemeType.Projection.Datum != DatumPrincipal)
            {
                SystemeType = new SystemeCartographique(SystemeType.IndiceSiteCarto, SystemeType.Niveau.Echelle, DatumPrincipal);
            }
            // Création de la nouvelle échelle 
            var RegroupementActuel = new Regroupement(SystemeType, ClefRegroupement);
            // ajoute les cartes du répertoire
            CreerRegroupementRet = RegroupementActuel.AjouterCartes(FichierGeorefInit, ref NbCartesAjout);
            // si l'ajout des cartes c'est bien passé on ajoute le regroupement dans la liste
            if (CreerRegroupementRet == VerifierRegroupement.OK)
            {
                Regroupements.Add(RegroupementActuel.Clef, RegroupementActuel);
            }

            return CreerRegroupementRet;
        }
        /// <summary> Supprime l'échelle passée en paramètre et libère les ressources associées </summary>
        internal static void SupprimerRegroupement(Regroupement RegroupementActuel)
        {
            RegroupementActuel.Cartes.Clear();
            RegroupementActuel.Cartes = null;
            RegroupementActuel.RepertoiresImportes.Clear();
            RegroupementActuel.RepertoiresImportes = null;
            Regroupements.Remove(RegroupementActuel.Clef);
        }
        /// <summary> associé à la propriété SelectionReferencement. Sert de tampon pour l'ensemble SiteCarto-DomTom possible </summary>
        private static readonly RectangleD[] SelectionsReferencement;
        /// <summary> taille en pixel du formulaire AffichePlanRegroupement </summary>
        private static Size TaillePlanRegroupement;
        /// <summary>associé à la propriété AfficheBord</summary>
        private static int Affiche_Bords;
        /// <summary>associé à la propriété Affiche_PositionVise</summary>
        private static Rectangle RegionAffichage;
        #endregion
        #region Formulaire d'affichage du plan de calpinage d'un regroupement
        private partial class CalepinageRegroupement
        {
            private int NumForme;
            private double CoefR;
            private int Xorigine;
            private int Yorigine;
            private Regroupement _RegroupementActuel;
            private Rectangle SelectionVirtuelle, SelectionPixel;
            private SitesCartographiques SiteCarto;
            private Datums Datum;
            private Echelles Echelle;
            private int IndiceEchelle;
            internal Regroupement RegroupementActuel
            {
                set
                {
                    _RegroupementActuel = value;
                }
            }
            /// <summary> initialise le formulaire d'affichage du plan de l'échelle </summary>
            private void CalepinageRegroupement_Load(object sender, EventArgs e)
            {
                Size = TaillePlanRegroupement;
                // centre le formulaire sur le formulaire parent
                {
                    var withBlock = Owner;
                    Location = new Point(withBlock.Location.X + (withBlock.Width - Width) / 2, withBlock.Location.Y + (withBlock.Height - Height) / 2);
                }
                // récupération de l'echelle actuelle
                SelectionVirtuelle = _RegroupementActuel._SelectionGeoref;
                // on ajoute les cartes
                FaireRectanglesCartes();
                var S = _RegroupementActuel.SystemeType;
                SiteCarto = S.SiteCarto;
                Datum = DatumSiteWeb((int)SiteCarto);
                Echelle = S.Niveau.Echelle;
                IndiceEchelle = EchelleToSiteIndiceEchelle(SiteCarto, Echelle);
            }
            private void CalepinageRegroupement_FormClosed(object sender, FormClosedEventArgs e)
            {
                Rectangle S = SurfaceTravail;
                if (S != SelectionPixel)
                {
                    if (S == Rectangle.Empty)
                    {
                        _RegroupementActuel._SelectionGeoref = Rectangle.Empty;
                    }
                    else
                    {
                        // il faut recalculer les coordonnées virtuelles car elles ont été modifiées à partir du plan de calepinage du regroupement
                        _RegroupementActuel._SelectionGeoref = new Rectangle((int)Math.Round((S.X - Xorigine) / CoefR),
                                                                        (int)Math.Round((S.Y - Yorigine) / CoefR),
                                                                        (int)Math.Round(S.Width / CoefR),
                                                                        (int)Math.Round(S.Height / CoefR));
                    }
                }
            }
            /// <summary>  Affichage sur le formulaire de l'ensemble des rectangles qui compose l'échelle et éventuellement de la visue  </summary>
            private void CalepinageRegroupement_Paint(object sender, PaintEventArgs e)
            {
                AfficherRectangles(e.Graphics);
                // si le dessin de la position de la visue a été demandé, on l'affiche
                if (!RegionAffichage.IsEmpty)
                {
                    // ajout d'un rectangle qui représente l'affichage de l'écran
                    e.Graphics.DrawRectangle(new Pen(Color.Blue, 2f), new Rectangle(Xorigine + (int)Math.Round(RegionAffichage.X * CoefR),
                                                                                    Yorigine + (int)Math.Round(RegionAffichage.Y * CoefR),
                                                                                    (int)Math.Round(RegionAffichage.Width * CoefR),
                                                                                    (int)Math.Round(RegionAffichage.Height * CoefR)));
                }
            }
            /// <summary> détermine si une carte est survolée et prépare la création ou la modification du rectangle de travail </summary>
            private void CalepinageRegroupement_MouseDown(object sender, MouseEventArgs e)
            {
                AppuyerBoutonSouris(e.Location, ModifierKeys);
            }
            /// <summary>  affiche les différentes informations cartographiques concernant la carte survolée et redessine le plan du regroupement si besoin </summary>
            private void CalepinageRegroupement_MouseMove(object sender, MouseEventArgs e)
            {
                // trouver le rectangle qui est survolé
                DeplacerSouris(e.Location);
                switch (NumRectangleSurvol)
                {
                    case -1:
                        {
                            // pas de rectangle survolé
                            RemplirCoordonneesCarte(-1);
                            // on efface de l'écran l'ancien rectangle survolé (il ne l'est plus)
                            if (NumForme != -1)
                                Invalidate();
                            break;
                        }
                    case 0:
                        {
                            // rectangle de travail survolé
                            RemplirCoordonneesCarte(0);
                            Invalidate();// on redessine toujours le rectangle de travail sur un évènement MouseMove
                            break;
                        }
                    case var @case when @case > 0:
                        {
                            // carte survolée
                            if (NumForme != NumRectangleSurvol)
                            {
                                RemplirCoordonneesCarte(NumRectangleSurvol);
                                // on affiche à l'écran le nouveau rectangle survolé
                                Invalidate();
                            }

                            break;
                        }
                }
                // on garde en mémoire le rectangle survolé
                NumForme = NumRectangleSurvol;
                RemplirCoordonneesCurseur(e.Location);
            }
            /// <summary> En mode Surface un click sur le plan de calepinage ferme le formulaire et détermine
            /// les nouvelles coordonnées virtuelles de l'affichage si demandé par l'utilisateur </summary>
            private void CalepinageRegroupement_MouseUp(object sender, MouseEventArgs e)
            {
                RelacherBoutonSouris();
                // il y a eu un click sur le plan de calepinage.
                if (ClickRectangle)
                {
                    // La fermeture est demandée sur une carte existante cela indique que l'on veut changer la position de l'affichage
                    _RegroupementActuel.PositionGeoref = new Point((int)Math.Round((e.X - Xorigine) / CoefR), (int)Math.Round((e.Y - Yorigine) / CoefR));
                    // on indique la carte survolée.
                    _RegroupementActuel.NumCarteSelection = NumRectangleSurvol;
                    // on ferme le formulaire de calepinage
                    Close();
                }
                else if (NumRectangleSurvol == -1)
                {
                    _RegroupementActuel.NumCarteSelection = NumRectangleSurvol;
                    // click en dehors des cartes
                    Close();
                }
                else
                {
                    // c'est la fin de création ou de modification du rectangle de travail
                    Invalidate();
                }
            }
            /// <summary> gère les touches clavier envoyées au formulaire. La touche escape permet de sortir sans carte sélectionnée </summary>
            private void CalepinageRegroupement_KeyDown(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        {
                            DialogResult = DialogResult.Cancel;
                            _RegroupementActuel.NumCarteSelection = -1;
                            Close();
                            break;
                        }
                    case Keys.Enter:
                        {
                            DialogResult = DialogResult.OK;
                            _RegroupementActuel.NumCarteSelection = -1;
                            Close();
                            break;
                        }
                    case Keys.Delete:
                    case Keys.Back:
                        {
                            CreerRectangleTravail(Rectangle.Empty);
                            Invalidate();
                            break;
                        }
                }
            }
            /// <summary> détermine l'échelle et les rectangles qui composent la représentation de l'échelle sur le formulaire</summary>
            private void FaireRectanglesCartes()
            {
                var DimensionsPlanRegroupement = new Size(ClientSize.Width - Affiche_Bords * 2, ClientSize.Height - Affiche_Bords * 2 - CoordonneesCurseur.Height);
                // récupération de la surface de l'ensemble des cartes du regroupement et calcul du coefficient de mise à l'échelle dessin/regroupement
                var DimensionsRegroupement = _RegroupementActuel.RegionVirtuelle;
                // si le positionnement de l'affichage sur le plan est demandé, il peut être ne dehors de l'emprise du regroupement
                if (!RegionAffichage.IsEmpty)
                {
                    DimensionsRegroupement = Rectangle.Union(DimensionsRegroupement, RegionAffichage);
                }
                // si il y a une sélection à afficher sur le plan, elle peut être ne dehors de l'emprise du regroupement
                if (SelectionVirtuelle != Rectangle.Empty)
                {
                    DimensionsRegroupement = Rectangle.Union(DimensionsRegroupement, SelectionVirtuelle);
                }
                double R_X = DimensionsPlanRegroupement.Width / (double)DimensionsRegroupement.Width;
                double R_Y = DimensionsPlanRegroupement.Height / (double)DimensionsRegroupement.Height;
                if (R_X > R_Y)
                {
                    CoefR = R_Y;
                }
                else
                {
                    CoefR = R_X;
                }
                // calcul des X et Y d'origine en pixels de la zone de dessin
                Xorigine = (DimensionsPlanRegroupement.Width - (int)Math.Round(DimensionsRegroupement.Width * CoefR, 0)) / 2 +
                                                               (int)Math.Round(-DimensionsRegroupement.X * CoefR, 0) + Affiche_Bords;
                Yorigine = (DimensionsPlanRegroupement.Height - (int)Math.Round(DimensionsRegroupement.Height * CoefR, 0)) / 2 +
                                                                (int)Math.Round(-DimensionsRegroupement.Y * CoefR, 0) + Affiche_Bords;

                // calcul de la selection
                if (SelectionVirtuelle != Rectangle.Empty)
                {
                    SelectionPixel = new Rectangle(Xorigine + (int)Math.Round(SelectionVirtuelle.X * CoefR, 0),
                                                   Yorigine + (int)Math.Round(SelectionVirtuelle.Y * CoefR, 0),
                                                   (int)Math.Round(SelectionVirtuelle.Width * CoefR, 0),
                                                   (int)Math.Round(SelectionVirtuelle.Height * CoefR, 0));
                }
                // initialisation du module Objetrectangle avec le rectangle de travail en mode surface et la possibilité d'en avoir un
                // Le -3 correspond à la largeur minimum du rectangle de séléction, -24 correspond à la hauteur de la barre d'info plus 2 pixels de bordure
                var SurfaceDessin = new Rectangle(new Point(ClientRectangle.X + 3, ClientRectangle.Y + 3), new Size(ClientRectangle.Width - 6, ClientRectangle.Height - 24 - 6));
                InitialiserRectangles(SurfaceDessin, Mode.Surface, SelectionPixel, true);
                foreach (var CarteActuelle in _RegroupementActuel.Cartes)
                {
                    // calcul des dimensions de la carte en pixels de la zone de dessin
                    Rectangle DimensionsCarte = CarteActuelle.RegionVirtuelle;
                    int X = Xorigine + (int)Math.Round(DimensionsCarte.X * CoefR, 0);
                    int Y = Yorigine + (int)Math.Round(DimensionsCarte.Y * CoefR, 0);
                    int L = (int)Math.Round(DimensionsCarte.Width * CoefR, 0);
                    int H = (int)Math.Round(DimensionsCarte.Height * CoefR, 0);
                    string Name = CarteActuelle.Nom;
                    // ajout dans la collection du rectangle représentant la carte
                    AjouterRectangle(new Point(X, Y), new Point(X + L, Y + H), Name);
                }
            }
            /// <summary> remplit la barre d'information concernant les coordonnées du pointeur de souris </summary>
            private void RemplirCoordonneesCurseur(Point PtCurseur)
            {
                // affiche les coordonnées du curseur de souris en unité du monde réel
                var Pt = new Point((int)Math.Round((PtCurseur.X - Xorigine) / CoefR), (int)Math.Round((PtCurseur.Y - Yorigine) / CoefR));
                CoordonneesCurseur.Text = CoordonneesReelles(Pt);
            }
            /// <summary> remplit la barre d'information concernant la carte survolée </summary>
            private void RemplirCoordonneesCarte(int NumCarte)
            {
                if (NumCarte > 0)
                {
                    string Nom = _RegroupementActuel.Cartes[NumCarte - 1].Nom;
                    Rectangle R = _RegroupementActuel.Cartes[NumCarte - 1].RegionVirtuelle;

                    var Pt0 = R.Location;
                    var Pt2 = Point.Add(Pt0, R.Size);
                    CoordonneesCartes.Text = $"{Nom} : Pt0 {CoordonneesReelles(Pt0)}, Pt2 {CoordonneesReelles(Pt2)}";
                }
                else
                {
                    CoordonneesCartes.Text = "Pas de carte sélectionnée";
                }
            }

            private string CoordonneesReelles(Point PtPixels)
            {
                string LabelCoordonnees = "";
                switch (RegrouperSettings.INDICE_TYPE_COORDONNEES)
                {
                    case 0:
                        {
                            LabelCoordonnees = CoordMouseGrilleText(CoordMouseGrille(PtPixels, SiteCarto, Echelle));
                            break;
                        }
                    case 1:
                        {
                            LabelCoordonnees = CoordMousseDDText(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum);
                            break;
                        }
                    case 2:
                        {
                            LabelCoordonnees = CoordMouseDMSText(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum);
                            break;
                        }
                    case 3:
                        {
                            LabelCoordonnees = CoordMouseUtmWGS84(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum);
                            break;
                        }
                    case 4:
                        {
                            LabelCoordonnees = CoordMousePointG(PtPixels, IndiceEchelle).ToString();
                            break;
                        }
                }
                return LabelCoordonnees;
            }
        }
        private partial class CalepinageRegroupement : Form
        {
            internal CalepinageRegroupement()
            {
                InitialiseComposants();
                InitialiserEvenements();
            }

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
            private void InitialiserEvenements()
            {
                Load += CalepinageRegroupement_Load;
                FormClosed += CalepinageRegroupement_FormClosed;
                Paint += CalepinageRegroupement_Paint;
                MouseDown += CalepinageRegroupement_MouseDown;
                MouseMove += CalepinageRegroupement_MouseMove;
                MouseUp += CalepinageRegroupement_MouseUp;
                KeyDown += CalepinageRegroupement_KeyDown;
            }
            private void InitialiseComposants()
            {
                components = new Container();
                CoordonneesCartes = new Label();
                CoordonneesCurseur = new Label();
                SuspendLayout();
                // 
                // CoordonneesCurseur
                // 
                CoordonneesCurseur.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                CoordonneesCurseur.BorderStyle = BorderStyle.FixedSingle;
                CoordonneesCurseur.FlatStyle = FlatStyle.Flat;
                CoordonneesCurseur.Location = new Point(-1, 701);
                CoordonneesCurseur.Name = "Info1";
                CoordonneesCurseur.Size = new Size(257, 23);
                CoordonneesCurseur.TabIndex = 0;
                // CoordonneesCurseur.Text = "Nb Tuiles : 1000"
                CoordonneesCurseur.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // CoordonneesCartes
                // 
                CoordonneesCartes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                CoordonneesCartes.BorderStyle = BorderStyle.FixedSingle;
                CoordonneesCartes.FlatStyle = FlatStyle.Flat;
                CoordonneesCartes.Location = new Point(255, 701);
                CoordonneesCartes.Name = "Info2";
                CoordonneesCartes.Size = new Size(446, 23);
                CoordonneesCartes.TabIndex = 1;
                // CoordonneesCartes.Text = "Nb Tuiles à supprimer : 1000"
                CoordonneesCartes.TextAlign = ContentAlignment.MiddleCenter;


                // Form1
                // 
                AutoScaleMode = AutoScaleMode.None;
                AutoSize = false;
                AutoSizeMode = AutoSizeMode.GrowOnly;
                ClientSize = new Size(700, 723);
                ControlBox = false;
                Controls.Add(CoordonneesCurseur);
                Controls.Add(CoordonneesCartes);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "Form1";
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.Manual;
                DoubleBuffered = true;
                ResumeLayout(false);
            }

            private Label CoordonneesCartes;
            private Label CoordonneesCurseur;
        }
        #endregion
    }
}