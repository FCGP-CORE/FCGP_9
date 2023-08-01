namespace FCGP
{
    /// <summary> ObjetRectangle
    /// Cette classe gère un ensemble de rectangles qui représente des objets de forme similaire comme des cartes ou des tuiles en ayant pour but
    /// l'affichage d'un plan de mapping et l'interaction avec celui-ci.
    /// Il s'agit principalement d'une liste de rectangles simples et d'un rectangle de travail dont l'index réservé dans la liste est 0
    /// 
    /// Il y a 2 modes de fonctionnement. 
    /// Le premier est utilisé pour la représentation d'un regroupement de cartes ou de tuiles et le rectangle de travail est utilisé
    /// pour déterminé une surface en relation avec les dimensions du regroupement des rectangles simples. Il s'agit du mode_Surface
    /// Le deuxième est utilsé pour la représentation d'un regroupement de cartes ou de tuiles et le rectangle de travail est utilisé
    /// pour sélectionner un ou des rectangles simples. Il s'agit du mode_Selection
    /// 
    /// 1er mode ou mode_Surface  
    /// Le rectangle simple est obligatoirement créé par programmation. Il n'a qu'un seul état et 2 représentations : Survolé ou non survolé.
    /// Il n 'a qu'une seule zone d'interaction qui est sa surface.
    /// Le click(modificateur de clavier non pris en compte) sur la zone d'interaction produit une action qui renvoie simplement le numéro d'index du rectangle simple dans la liste.
    /// Le rectangle de travail peut être créé par programmation ou avec la souris sur le surface d'affichage. Dans les 2 cas il est modifiable avec la souris.
    /// Il n 'à qu'un seul état et 2 représentations : Créé et non survolé ou survolé, encours de création ou encours de modification.
    /// Il a 8 zones d'interaction. Ces zones permettent de détecter le survol du rectangle de travail et la modification de celui-ci lorsque la zone est accrochée
    /// par la souris. La création se fait obligatoirement par un click qui marque une des 4 zones d'interaction en coins et un déplacement vers le coin opposé du
    /// rectangle de travail à créer. 
    /// Les zones d'interaction sont les 4 coins et les 4 cotés du rectangle.
    /// 
    /// 2ème mode ou mode_Selection :
    /// Le rectangle simple est obligatoirment créé par programmation. Il a 2 états : Sélectionné ou pas sélectionné et 2 représentations par états : Survolé ou
    /// non survolé. Le click renvoie une action fonction du modificateur de clavier. 
    /// Pas de modificateur : inverse l 'état sélectionné, 
    /// Shift: passe à l'état sélectionné,
    /// Ctrl: passe à l'état non sélectionné,
    /// Alt: lance une action qui est à la charge de l'appelant (délégate).
    /// Le rectangle de travail est obligatoirement créé avec la souris sur la surface d'affichage. Il est  modifiable uniquement pendant la création et il n'est pas sauvegardé une fois créé.
    /// La fin de la création du rectangle de travail sélectionne l'ensemble des rectangles simples dans son emprise en fonction du modificateur de clavier.
    /// Pas de modificateur : inverse l 'état sélectionné, 
    /// Shift: passe à l'état sélectionné,
    /// Ctrl: passe à l'état non sélectionné. 
    /// Alt : au choix </summary>

    internal static class ObjetRectangle
    {
        #region Variables et fonctions partagées
        /// <summary> indique que la forme est sélectionnée pour faire une action (modification dimensions ou rien) </summary>
        private static TypeActionLibelle ActionTravail;
        /// <summary> sert pour le calcul de DX et DY du mode action (différence entre 2 positions du pointeur de souris) </summary>
        private static Point Pt_Depart;
        /// <summary> constante valeur par défaut du pinceau de base( hors survol et action): vert et 1 pixels de large </summary>
        private readonly static Pen s_p_Base = new(Color.LimeGreen, 1f);
        /// <summary> constante valeur par défaut du pinceau de base spécial(survol ou action): rouge et 1 pixels de large</summary>
        private readonly static Pen s_p_Special = new(Color.Red, 2f);
        /// <summary>valeur par defaut de la demi largeur d'une zone de sensibilité : 1 pixels par défault</summary>
        private const int s_d_Sensibilite = 1;
        /// <summary> index réservé pour le rectangle de travail </summary>
        private const int Travail = 0;
        /// <summary> constante valeur par défaut de la brosse semi transparente pour le remplissage du rectangle simple si il est sélectionné </summary>
        private readonly static SolidBrush s_b_SemiTransBrush = new(Color.FromArgb(64, 128, 128, 128));
        /// <summary>constante valeur par défaut de la police d'affichage des infos</summary>
        private readonly static Font s_d_Police = new("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        /// <summary> collection des rectangles pour cette session. L'indice 0 est réservé pour le rectangle de travail </summary>
        private readonly static List<RectangleSimple> Rectangles = new();
        /// <summary> Concerne le rectangle de travail. 1 quand le 1er coin est connu
        /// 2 quand le coin opposé est connu. Le rectangle de travail est affichable. </summary>
        private static int FlagCreationTravail;
        /// <summary> modifieur de clavier lors de l'évenement Down de la souris (debut click) </summary>
        private static Keys ModifierAction;
        /// <summary> flag indiquant si le rectangle de travail est autorisé </summary>
        private static bool FlagRectangleTravail;
        /// <summary> Action à lancer lors d'un click sur un rectangle simple avec le modificateur Alt et le mode Sélection </summary>
        private static Func<int, DialogResult> _ActionRectangleSimple;
        /// <summary> Surface du rectangle de travail exprimée en pixel. </summary>
        internal static Rectangle SurfaceTravail
        {
            get
            {
                return Rectangles[Travail].Surface;
            }
        }
        /// <summary> Numéro du rectangle qui a une zone action en cours de survol par la souris </summary>
        internal static int NumRectangleSurvol { get; private set; }
        /// <summary> surface dans laquelle le dessin des Rectangles est autorisée. Généralement correspond au rectangle du Graphics
        /// d'affichage mais pour certaines Rectangle peut être ramenée à l'univers virtuel et pas la surface d'affichage </summary>
        internal static Rectangle SurfaceAutorise { get; private set; }
        /// <summary> indique le mode d'utilisation du rectangle de travail </summary>
        internal static Mode ModeTravail { get; private set; }
        /// <summary> renvoi le nb de rectangles simples </summary>
        internal static int NbRectangles
        {
            get
            {
                return Rectangles.Count - 1;
            }
        }
        /// <summary> renvoi le nb de rectangles simples dont la propriété IsSelected est false </summary>
        internal static int NbRectanglesNotSelected
        {
            get
            {
                return Rectangles.Count - 1 - NbRectanglesSelected;
            }
        }
        /// <summary> renvoi le nb de rectangles simples dont la propriété IsSelected est true </summary>
        internal static int NbRectanglesSelected { get; private set; }
        /// <summary> renvoi un tableau de boolean correspondant à la propriété IsSelected des rectangles simples. IndexBase 0 </summary>
        internal static bool[] RectanglesIsSelected
        {
            get
            {
                // on dimensionne le tableau en fonction de nb de rectangles simples
                var Ret = new bool[Rectangles.Count - 2 + 1];
                for (int IndexRectangle = 1, loopTo = Rectangles.Count - 1; IndexRectangle <= loopTo; IndexRectangle++)
                    // on livre un tableau à index base 0
                    Ret[IndexRectangle - 1] = Rectangles[IndexRectangle].IsSelected;
                return Ret;
            }
        }
        /// <summary> renvoi un tableau qui contient l'index des rectangles qui ont la propriété IsSelected à true. IndexBase 0 </summary>
        internal static int[] RectanglesSelected
        {
            get
            {
                var T = new int[NbRectanglesSelected];
                int Cpt = default, CptRectangle;
                var loopTo = Rectangles.Count - 1;
                for (CptRectangle = 1; CptRectangle <= loopTo; CptRectangle++)
                {
                    if (Rectangles[CptRectangle].IsSelected)
                    {
                        T[Cpt] = CptRectangle - 1;
                        Cpt += 1;
                    }
                }
                return T;
            }
        }

        /// <summary> permet d'indiquer à l'appelant qu'il y a eu un click sur un rectangle simple </summary>
        internal static bool ClickRectangle { get; private set; }
        /// <summary> définit l'action qui doit être réalisée lors d'un click sur un rectangle simple (addresse d'une fonction à 1 paramètre integer) </summary>
        internal static Func<int, DialogResult> ActionRectangleSimple
        {
            set
            {
                _ActionRectangleSimple = value;
            }
        }

        /// <summary> permet de connaitre à partir d'une seule instruction (manip collective) le rectangle qui est détecté </summary>
        /// <param name="Pt"> Position du curseur de souris</param>
        /// <param name="ModifiersKey"> Etat des touches shift, ctrl et alt lors de l'appui sur le bouton de souris </param>
        /// <remarks> Cette action est appelée dans l'évenement Mouse_Down du controle support du plan de calepinage </remarks>
        internal static void AppuyerBoutonSouris(Point Pt, Keys ModifiersKey)
        {
            Pt_Depart = Pt;
            ModifierAction = ModifiersKey;
            ClickRectangle = false;
            // les rectangles simples ne sont pas concernés par l'évènement down si le rectangle de travail est créé
            if (NumRectangleSurvol > Travail && FlagCreationTravail == 2)
            {
                return;
            }
            // si le rectangle de travail n'est pas créé on vérifie avant de le créer qu'il ne s'agit pas d'un click sur un rectangle simple 
            // mais d'un appui avec déplacement d'où un armement qui sera enlevé si l'évènement Click suit directement au lieu d'un évenement Move
            if (NumRectangleSurvol != Travail && FlagCreationTravail == 0 && FlagRectangleTravail)
            {
                FlagCreationTravail = 1;
                ActionTravail = TypeActionLibelle.Modification;
                return;
            }
            // si le rectangle travail est créé on indique que l'on rentre dans une action de changement de taille du rectangle
            if (NumRectangleSurvol == Travail && FlagCreationTravail == 2)
            {
                Rectangles[Travail].RealiserActionSouris(Pt);
                ActionTravail = TypeActionLibelle.Modification;
            }
        }
        /// <summary> renvoie le numéro de l'instance qui a terminée l'action(manip collective)</summary>
        /// <remarks> Cette action est appelée dans l'évenement Mouse_Up du controle support du plan de calepinage </remarks>
        internal static void RelacherBoutonSouris()
        {
            // si le flag concernant la création du rectangle de travail est mis on l'enlève car il indique qu'il n'y a pas eu de déplacement de la souris
            if (FlagCreationTravail == 1)
            {
                FlagCreationTravail = 0;
                ActionTravail = TypeActionLibelle.Aucune;
            }
            if (NumRectangleSurvol >= Travail)
            {
                Rectangles[NumRectangleSurvol].FinirActionSouris();
            }
        }
        /// <summary> permet à partir d'une seule instruction(manip collective) de definir l'action à réaliser l'ensemble des formes de la collection. </summary>
        /// <param name="Pt"> position du curseur de souris </param>
        /// <remarks> Cette action est appelée dans l'évenement Mouse_Move du controle support du plan de calepinage </remarks>
        internal static void DeplacerSouris(Point Pt)
        {
            // si il n'y a pas d'action en cours, il faut savoir si un des rectangles est ou est suceptible d'être survolé.
            if (ActionTravail == TypeActionLibelle.Aucune)
            {
                // Cas du rectangle de travail créé
                if (NumRectangleSurvol == Travail && Rectangles[Travail].ZonesAction[(int)Rectangles[Travail].ZoneActionEnCours].Contains(Pt))
                {
                    // le rectangle de travail est déjà survolé avec la même zone d'action
                    // et comme il est prioritaire il n'y a plus rien à faire
                    return;
                }
                // on cherche en premier si le rectangle de travail peut être survolé car il est prioritaire mais il faut qu'il soit autorisé
                else if (FlagRectangleTravail && Rectangles[Travail].TrouverZoneActionSurvol(Pt))
                {
                    // si oui on rénitialise le rectangle survolé sauf si c'est déjà le rectangle de travail
                    if (NumRectangleSurvol > Travail)
                    {
                        Rectangles[NumRectangleSurvol].ZoneActionEnCours = ZonesInterAction.Aucune;
                        Rectangles[NumRectangleSurvol].IsOver = false;
                    }
                    // on indique que le rectangle de travail est survolé
                    NumRectangleSurvol = Travail;
                    return;
                }
                // sinon est ce qu'un rectangle simple est déja survolé avec la même zone d'action. Il n'y a qu'une seule zone d'action
                else if (NumRectangleSurvol > Travail && Rectangles[NumRectangleSurvol].ZonesAction[(int)Rectangles[NumRectangleSurvol].ZoneActionEnCours].Contains(Pt))
                {
                    // dans ce cas il 'y a plus rien à faire
                    return;
                }
                else
                {
                    // sinon il faut trouver parmi l'ensemble des rectangles simples si il y en a un qui est survolé par le curseur de souris
                    for (int Cpt = 1, loopTo = Rectangles.Count - 1; Cpt <= loopTo; Cpt++)
                    {
                        // il ne peut y avoir qu'un seul rectangle qui prend la main donc on sort au premier trouvé.
                        // Cela peut bloquer un rectangle plus petit ayant un index supérieur. Il ne sera jamais survolé
                        if (Rectangles[Cpt].TrouverZoneActionSurvol(Pt))
                        {
                            // si oui on rénitialise le rectangle survolé
                            if (NumRectangleSurvol >= Travail)
                            {
                                Rectangles[NumRectangleSurvol].ZoneActionEnCours = ZonesInterAction.Aucune;
                                Rectangles[NumRectangleSurvol].IsOver = false;
                            }
                            // on indique que l'un des rectangle simple est survolé
                            NumRectangleSurvol = Cpt;
                            return;
                        }
                    }
                    // il n'y a pas de rectangle survolé donc on l'indique et on RAZ éventuellement l'ancien
                    if (NumRectangleSurvol >= Travail)
                    {
                        Rectangles[NumRectangleSurvol].ZoneActionEnCours = ZonesInterAction.Aucune;
                        Rectangles[NumRectangleSurvol].IsOver = false;
                    }
                    NumRectangleSurvol = -1;
                }
            }
            // TypeAction = TypeActionPossible.Modification car c'est la seule possible avec le rectangle de travail
            // Le click est suivi d'un mouvement de la souris donc on considère que l'on crée le rectangle de travail
            else if (FlagCreationTravail == 1)
            {
                int DX = Pt.X - Pt_Depart.X;
                int DY = Pt.Y - Pt_Depart.Y;
                // on attend que le déplacement soit bien marqué pour choisir la zone d'action de départ
                if (DX == 0 || DY == 0)
                    return;
                ZonesInterAction Zone;
                // determine le sens de déplacement de la souris afin de connaitre le coin de départ du rectangle de travail
                if (DX >= 0)
                {
                    if (DY >= 0)
                    {
                        Zone = ZonesInterAction.Coin_BasDroit;
                    }
                    else
                    {
                        Zone = ZonesInterAction.Coin_HautDroit;
                    }
                }
                else if (DY <= 0)
                {
                    Zone = ZonesInterAction.Coin_HautGauche;
                }
                else
                {
                    Zone = ZonesInterAction.Coin_BasGauche;
                }
                // le rectangle de travail va être créé, on marque le 1er coin
                Rectangles[Travail].DebuterActionSouris(Pt_Depart, Zone);
                // éventuellement on reinitialise le rectangle simple qui était survolé
                if (NumRectangleSurvol > Travail)
                {
                    Rectangles[NumRectangleSurvol].ZoneActionEnCours = ZonesInterAction.Aucune;
                    Rectangles[NumRectangleSurvol].IsOver = false;
                }
                // on indique que le rectangle de travail est le rectangle survolé et qu'il est créé au sens affichable
                NumRectangleSurvol = Travail;
                FlagCreationTravail = 2;
            }
            // le rectangle de travail est créé, il faut réaliser l'action demandée, c'est à dire le redimensionnement de celui-ci
            else if (FlagRectangleTravail)
                Rectangles[Travail].RealiserActionSouris(Pt);
        }
        /// <summary> permet à partir d'une seule instruction(manip collective) de dessiner l'ensemble des rectangles de la collection. </summary>
        /// <param name="Graph">zone de dessin</param>
        /// <remarks> Cette action est appelée dans l'évenement Paint du controle support du plan de calepinage </remarks>
        internal static void AfficherRectangles(Graphics Graph)
        {
            // affiche tous les rectangles simples qui ne sont pas survolés
            for (int Cpt = 1, loopTo = Rectangles.Count - 1; Cpt <= loopTo; Cpt++)
            {
                if (!Rectangles[Cpt].IsOver)
                {
                    Rectangles[Cpt].Dessiner(Graph);
                }
            }
            // affiche tous les rectangles simples qui sont survolés après afin qu'ils soient visibles
            for (int Cpt = 1, loopTo1 = Rectangles.Count - 1; Cpt <= loopTo1; Cpt++)
            {
                if (Rectangles[Cpt].IsOver)
                {
                    Rectangles[Cpt].Dessiner(Graph);
                }
            }
            // si le rectangle de travail existe on l'affiche par dessus tous les autres
            if (FlagCreationTravail > 1)
            {
                Rectangles[Travail].Dessiner(Graph);
            }
            if (NumRectangleSurvol != -1)
            {
                CurseurSouris.CurseurEncours = Rectangles[NumRectangleSurvol].TypeCurseurEncours;
            }
            else
            {
                CurseurSouris.CurseurEncours = Enumerations.Curseurs.Defaut;
            }
        }
        /// <summary> Permet d'instancier un rectangle simple avec les 2 principaux paramètres obligatoires (Point de début ou hautgauche  
        /// et point de fin ou point basdroit. Ces deux points permettent de définir le rectangle </summary>
        /// <param name="PtHG">Obligatoire : Point Haut Gauche du rectangle</param>
        /// <param name="PtBD">Obligatoire : Point Bas Droit du rectangle</param>
        /// <param name="Info"> Optionnel : si vide rien ne sera affiché lors du survol du rectangle simple </param>
        /// <param name="IsSelect"> Optionnel : indique si le rectangle est considéré comme selectionné </param>
        internal static void AjouterRectangle(Point PtHG, Point PtBD, string Info = "", bool IsSelect = false)
        {
            Rectangles.Add(new RectangleSimple(PtHG, PtBD, Info, Rectangles.Count, IsSelect));
            if (IsSelect)
                NbRectanglesSelected += 1;
        }
        /// <summary> vide la collection et remet à zéro les variables partagées par l'ensembles des rectangles </summary>
        /// <param name="SurfaceDessin"> indique la surface sur laquelle les rectangles doivent être contenus. Cela permet de limiter
        /// l'extension du rectangle de sélection, c'est 3 pixels des bords </param>
        /// <param name="SurfaceTravail" > indique la surface du rectangle de travail </param>
        /// <param name="Utilite"> indique le mode d'utilisation souhaité pour le rectangle de travail </param>
        /// <param name="FlagTravail"> indique que l'on veut ou non un rectangle de travail </param>
        internal static void InitialiserRectangles(Rectangle SurfaceDessin, Mode Utilite, Rectangle SurfaceTravail, bool FlagTravail = true)
        {
            Rectangles.Clear();
            NumRectangleSurvol = -1;
            SurfaceAutorise = SurfaceDessin;
            ModeTravail = Utilite;
            FlagRectangleTravail = FlagTravail;
            NbRectanglesSelected = 0;
            ClickRectangle = false;
            ActionTravail = TypeActionLibelle.Aucune;
            // on crée le rectangle de travail à l'indice 0
            CreerRectangleTravail(SurfaceTravail);
        }
        /// <summary> initialisation du rectangle de travail. Indice 0 dans la collection sans passer par l'initialisation des rectangles simples </summary> 
        /// <param name="Selection" > indique la surface du rectangle de sélection </param>
        internal static void CreerRectangleTravail(Rectangle Selection)
        {
            var RectangleSelection = new ObjetRectangle_Travail(Selection.Location, new Point(Selection.Right, Selection.Bottom), "", 0);
            // on ajoute ou remplace le rectangle selection
            if (Rectangles.Count > 0)
            {
                Rectangles[Travail] = RectangleSelection;
            }
            else
            {
                Rectangles.Add(RectangleSelection);
            }
            FlagCreationTravail = Selection == Rectangle.Empty ? 0 : 2;
        }
        /// <summary> calcule si le rectangle doit être sélectionné ou pas et le nb de rectangles simples sélectionnés </summary>
        /// <param name="R"> Rectangle simple qui doit être modifié </param>
        /// <param name="Alt"> ce que l'on veut comme sélection si le modificateur est Alt </param>
        private static void ModifierSelection(RectangleSimple R, bool Alt = true)
        {
            bool AncienSelected = R.IsSelected;
            bool Selected;
            switch (ModifierAction)
            {
                case Keys.Shift:
                    {
                        Selected = true;
                        break;
                    }
                case Keys.Control:
                    {
                        Selected = false;
                        break;
                    }
                case Keys.Alt:
                    {
                        Selected = Alt; // keys.none 
                        break;
                    }

                default:
                    {
                        Selected = !AncienSelected;
                        break;
                    }
            }
            if (AncienSelected != Selected)
            {
                NbRectanglesSelected += AncienSelected ? -1 : +1;
            }
            R.IsSelected = Selected;
        }
        #endregion
        #region Classe Rectangle Simple
        private class RectangleSimple
        {
            #region Variables de l'instance
            /// <summary>Flag indiquant si le rectangle est survolé par le curseur de souris. Sert pour le rendu de l'affichage</summary>
            internal bool IsOver;
            /// <summary>renvoie true si le controle rectangle est selectionné </summary>
            internal bool IsSelected;
            /// <summary> tableau des zones de sensibilité : surface. Doit être redimensionné dans la phase d'initialisation du rectangle et mis à jour par la procédure DetermineZones</summary>
            internal Rectangle[] ZonesAction;
            /// <summary>indique la zone action du rectangle qui est survolée par le curseur. index des 2 tableaux précédents</summary>
            internal ZonesInterAction ZoneActionEnCours;
            /// <summary>sommet Haut- Gauche du rectangle</summary>
            protected Point PointHautGauche;
            /// <summary>sommet Bas-Droit du rectangle</summary>
            protected Point PointBasDroit;
            /// <summary>variable associée à la propriété PinceauBase du rectangle</summary>
            protected Pen PinceauBase;
            /// <summary>variable associée à la propriété PinceauSpecial du rectangle action et survol</summary>
            protected Pen PinceauSpecial;
            /// <summary> tableau des zones de sensibilité : curseurs. Doit être redimensionné dans la phase d'initialisation du rectangle et mis à jour par la procédure DetermineZones</summary>
            protected Enumerations.Curseurs[] ZonesActionCurseurs;
            /// <summary> nb de zones de sensibilité sur une longueur ou largeur du contrôle rectangle </summary>
            protected int NbZonesSensibleLH;
            /// <summary> renvoie la 1/2 largeur de la zone de sensibilité (1 pixels par défault). 1 de chaque coté de la ligne </summary>
            protected int Sensibilite;
            /// <summary>variable de la police d'affichage des infos</summary>
            private Font Police;
            /// <summary> Flag indiquant si les infos associées au rectangle doivent être affichées quand le rectangle est survolé</summary>
            private bool AfficherInfo;
            /// <summary>Informations à afficher lors du survol du rectangle</summary>
            private string Information;
            #endregion
            #region Procédures et propriétés Instance
            /// <summary> renvoie la surface du rectangle  </summary>
            internal virtual Rectangle Surface
            {
                get
                {
                    return new Rectangle(PointHautGauche, new Size(Largeur, Hauteur));
                }
            }
            /// <summary>renvoie le curseur de la zone d'action en cours</summary>
            internal virtual Enumerations.Curseurs TypeCurseurEncours
            {
                get
                {
                    return ZonesActionCurseurs[(int)ZoneActionEnCours];
                }
            }
            /// <summary>réalise l'action de l'instance.  Si on arrive dans cette procédure, il y a forcément une zone de sensibilité de l'instance qui est selectionnée</summary>
            /// <param name="Pt">position du pointeur de souris</param>
            /// <remarks> est appelé suite à l'évenement Mousemove </remarks>
            internal virtual void RealiserActionSouris(Point Pt)
            {
                // il n'y a pas d'action pour le rectangle simple
            }
            /// <summary> Permet le passage au mode 'Actif' de l'instance. L'action est déterminée par le type du rectangle et éventuellement par la zone d'action survolée </summary>
            /// <param name="Pt">point correspondant au pointeur de souris au départ de l'action</param>
            /// <remarks> est appelé suite à l'évenement MouseDOWN </remarks>
            internal virtual void DebuterActionSouris(Point Pt, ZonesInterAction Zone)
            {
                // il n'y a pas d'action pour le rectangle de base
            }
            /// <summary> finalise l'action de l'instance </summary>
            /// <remarks> est appelé suite à l'évenement MouseUP </remarks>
            internal virtual void FinirActionSouris()
            {
                // on renvoie toujours vrai pour un click sur rectangle simple
                ClickRectangle = true;
                if (ModeTravail == Mode.Selection)
                {
                    // en mode sélection le modificateur alt indique qu'il faut lancer l'action
                    if (ModifierAction == Keys.Alt)
                    {
                        if (_ActionRectangleSimple(NumRectangleSurvol) == DialogResult.OK)
                        {
                            // si enter on déselectionne le rectangle simple
                            ModifierSelection(this, false);
                        }
                        else
                        {
                            // sinon on reconduit la sélection du rectangle simple
                            ModifierSelection(this, Rectangles[NumRectangleSurvol].IsSelected);
                        }
                    }
                    else
                    {
                        // c'est le modifieur de clavier qui force ou non la sélection du rectangle simple
                        ModifierSelection(this);
                    }
                }
            }
            /// <summary> constructeur de rectangle simple </summary>
            internal RectangleSimple(Point PtHG, Point PtBD, string Info, int Num, bool IsSelect)
            {
                // il n'y a qu'une seule zone de survol possible pour le rectangle de base
                NbZonesSensibleLH = 1;
                ZonesAction = new Rectangle[2];
                ZonesActionCurseurs = new Enumerations.Curseurs[2];
                Initialiser(PtHG, PtBD, Info, Num);
                // couleur de la forme
                PinceauBase = s_p_Base;
                PinceauSpecial = s_p_Special;
                IsSelected = IsSelect;
            }
            /// <summary> dessine le rectangle sur la surface d'affichage passée en paramètre </summary>
            /// <param name="Graph">surface d'affichage</param>
            /// <remarks>est appelé indirectement par l'évenement paint du controle support de l'affichage </remarks>
            internal void Dessiner(Graphics Graph)
            {
                var Pinceau = PinceauBase;
                var R = Surface;
                if (IsOver)
                    Pinceau = PinceauSpecial;
                // on dessine la forme avec le pinceau qui va bien
                Graph.DrawRectangle(Pinceau, R);
                // si il est sélectionné on le prend en compte
                if (IsSelected)
                    Graph.FillRectangle(s_b_SemiTransBrush, R);
                // si l'affichage de l'info associée est demandé et si le rectangle est survolé
                if (AfficherInfo == true && IsOver && NumRectangleSurvol > Travail)
                {
                    // trouve la surface de la chaine d'information
                    var S = Graph.MeasureString(Information, Police);
                    float X;
                    float Y;
                    // on affiche l'info centré par raport au rectangle
                    X = Surface.X + (R.Width - S.Width) / 2f;
                    Y = R.Y + (R.Height - S.Height) / 2f;
                    Graph.DrawString(Information, Police, Brushes.Black, X, Y);
                }
            }
            /// <summary>permet la détection de la zone d'action du rectangle</summary>
            /// <param name="Pt">position du curseur de souris</param>
            internal bool TrouverZoneActionSurvol(Point Pt)
            {
                ZoneActionEnCours = ZonesInterAction.Aucune;
                IsOver = false;
                // on cherche la zone d'action qui contient le curseur de souris
                for (int Cpt = 1, loopTo = ZonesAction.Length - 1; Cpt <= loopTo; Cpt++)
                {
                    if (ZonesAction[Cpt].Contains(Pt))
                    {
                        ZoneActionEnCours = (ZonesInterAction)Cpt;
                        IsOver = true;
                        break;
                    }
                }
                return IsOver;
            }
            /// <summary> initalise une partie des variables associées au rectangle simple </summary>
            /// <param name="PtHG">Point haut gauche exprimé en pixel de la zone client d'affichage</param>
            /// <param name="PtBD">Point bas droit exprimé en pixel de la zone client d'affichage</param>
            /// <param name="Info"> l'information à affiché lors du survol du rectangle par le pointeur de souris</param>
            /// <param name="Num"> numéro du rectangle </param>
            protected void Initialiser(Point PtHG, Point PtBD, string Info, int Num)
            {
                ZoneActionEnCours = ZonesInterAction.Aucune;
                Police = s_d_Police;
                Sensibilite = s_d_Sensibilite;
                if (PtHG == Point.Empty && PtBD == Point.Empty)
                {
                    // on considère que le rectangle n'est pas créé et qu'il faudra l'initialiser après coup par programation.
                    // Pas implémenté pour le rectangle de base
                    if (Type == "Rectangle_Travail")
                        return;
                    throw new Exception("Les points du rectangle simple doivent être définis avant de demander sa création");
                }
                else
                {
                    // le point de début ne peut pas être en dehors du graphics. 
                    if (PtHG.X < 0)
                        PtHG.X = 0;
                    if (PtHG.Y < 0)
                        PtHG.Y = 0;
                    // la longueur et la hauteur ne peuvent pas être inférieures à distance mini
                    if (PtBD.X - PtHG.X < DistanceMini)
                    {
                        PtBD.X = PtHG.X + DistanceMini;
                    }
                    if (PtBD.Y - PtHG.Y < DistanceMini)
                    {
                        PtBD.Y = PtHG.Y + DistanceMini;
                    }
                }
                PointHautGauche = PtHG;
                PointBasDroit = PtBD;
                // ajout des infos de la forme dans la collection Infos
                if (string.IsNullOrEmpty(Info))
                {
                    // par défaut le type et le numID
                    Information = Type + "-" + Num.ToString("00");
                    AfficherInfo = false;
                }
                else
                {
                    Information = Info;
                    AfficherInfo = true;
                }
                // aucune zone action initiale
                ZonesAction[(int)ZonesInterAction.Aucune] = Rectangle.Empty;
                ZonesActionCurseurs[(int)ZonesInterAction.Aucune] = Enumerations.Curseurs.Defaut;
                // on determine les différentes zones d'action et les curseurs de souris qui vont avec
                CalculerZonesAction();
            }
            /// <summary>met à jour différentes zones d'action du rectangle </summary>
            /// <remarks>doit être appelé à la création du rectangle si il est créé et à la fin d'une action qui change la taille ou la postion du rectangle</remarks>
            protected virtual void CalculerZonesAction()
            {
                // il n'y a qu'une zone d'action pour le rectangle de base
                ZonesAction[(int)ZonesInterAction.Zone_Unique] = Surface;
                ZonesActionCurseurs[(int)ZonesInterAction.Zone_Unique] = Enumerations.Curseurs.Defaut;
            }
            /// <summary> représente la longueur ou la hauteur minimale du rectangle afin qu'il puisse contenir le nombre de zones de sensibiité prévues </summary>
            protected int DistanceMini
            {
                get
                {
                    return NbZonesSensibleLH * Sensibilite;
                }
            }
            /// <summary> renvoie la largeur du rectangle </summary>
            protected int Largeur
            {
                get
                {
                    return PointBasDroit.X - PointHautGauche.X;
                }
            }
            /// <summary> renvoie la hauteur du rectangle </summary>
            protected int Hauteur
            {
                get
                {
                    return PointBasDroit.Y - PointHautGauche.Y;
                }
            }
            /// <summary> renvoie le type controle_Reference </summary>
            protected virtual string Type
            {
                get
                {
                    return "Rectangle_Simple";
                }
            }
            /// <summary> uniquement pour l'héritage de la classe </summary>
            protected RectangleSimple()
            {
            }
            #endregion
        }
        #endregion
        #region Types Imbriqués
        /// <summary>definit les différents types possibles d'action pour les formes </summary>
        private enum TypeActionLibelle : int
        {
            /// <summary> quand il n'y a pas d'action en cours </summary>
            Aucune = 0,
            /// <summary> pour indiquer qu'il s'agit d'une modification du rectangle de travail </summary>
            Modification = 1
        }
        /// <summary> definit les zones de sensibilité possibles </summary>
        private enum ZonesInterAction : int
        {
            /// <summary> quand il n'y a pas de controle rectangle survolé ou en cours d'action move </summary>
            Aucune = 0,
            /// <summary> Zone unique pour les rectangles simples </summary>
            Zone_Unique = 1,
            /// <summary> modification du rectangle de travail avec le coin haut gauche </summary>
#pragma warning disable CA1069 // Les valeurs enum ne doivent pas être dupliquées
            Coin_HautGauche = 1, // pour les 8 zones qui recouvre l'ensemble du rectangle de sélection lors de la création ou de l'agrandissement
#pragma warning restore CA1069 // Les valeurs enum ne doivent pas être dupliquées
            /// <summary> modification du rectangle de travail avec le coin bas droit </summary>
            Coin_BasDroit = 2,
            /// <summary> modification du rectangle de travail avec le coin haut droit </summary>
            Coin_HautDroit = 3,
            /// <summary> modification du rectangle de travail avec le coin bas gauche </summary>
            Coin_BasGauche = 4,
            /// <summary> modification du rectangle de travail avec le bord du haut </summary>
            Bord_Haut = 5,
            /// <summary> modification du rectangle de travail avec le bord du bas </summary>
            Bord_Bas = 6,
            /// <summary> modification du rectangle de travail avec le bord gaughe </summary>
            Bord_Gauche = 7,
            /// <summary> modification du rectangle de travail avec le bord droit </summary>
            Bord_Droit = 8
        }
        /// <summary> definit les differents modes possibles pour utiliser le rectangle de travail </summary>
        internal enum Mode
        {
            Aucun = 0,
            /// <summary> Le rectangle de travail est utilisé pour derterminer une surface. Il n'interagit pas avec les rectangles simples </summary>
            Surface = 1,
            /// <summary> Le rectangle de travail est utilisé pour sélectionner les rectangles simples </summary>
            Selection = 2
        }
        #endregion
        #region Class Rectangle_Travail
        private class ObjetRectangle_Travail : RectangleSimple
        {
            /// <summary>Permet de calculer 8 zones action du rectangle : 4 coins et 4 lignes </summary>
            protected override void CalculerZonesAction()
            {
                int Sensib_2_Plus = Sensibilite * 2 + 1;
                int Sensib_Plus = Sensibilite + 1;
                int LargeurLigne = Largeur - Sensib_2_Plus;
                int HauteurLigne = Hauteur - Sensib_2_Plus;

                // calcul des 2 autres coins du rectanglereference pour faciliter les différents calculs des zones de sensibilité
                var _PointHautDroit = new Point(PointHautGauche.X + Largeur, PointHautGauche.Y);
                var _PointBasGauche = new Point(PointHautGauche.X, PointHautGauche.Y + Hauteur);
                // Coin Haut Gauche
                ZonesAction[(int)ZonesInterAction.Coin_HautGauche] = new Rectangle(PointHautGauche.X - Sensibilite, PointHautGauche.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus);
                ZonesActionCurseurs[(int)ZonesInterAction.Coin_HautGauche] = Enumerations.Curseurs.FRefAgrandir_HG_BD;
                // Coin Bas Gauche
                ZonesAction[(int)ZonesInterAction.Coin_BasGauche] = new Rectangle(_PointBasGauche.X - Sensibilite, _PointBasGauche.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus);
                ZonesActionCurseurs[(int)ZonesInterAction.Coin_BasGauche] = Enumerations.Curseurs.FRefAgrandir_HD_BG;
                // Coin Haut Droit
                ZonesAction[(int)ZonesInterAction.Coin_HautDroit] = new Rectangle(_PointHautDroit.X - Sensibilite, _PointHautDroit.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus);
                ZonesActionCurseurs[(int)ZonesInterAction.Coin_HautDroit] = Enumerations.Curseurs.FRefAgrandir_HD_BG;
                // Coin Bas Droit
                ZonesAction[(int)ZonesInterAction.Coin_BasDroit] = new Rectangle(PointBasDroit.X - Sensibilite, PointBasDroit.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus);
                ZonesActionCurseurs[(int)ZonesInterAction.Coin_BasDroit] = Enumerations.Curseurs.FRefAgrandir_HG_BD;
                // Lignes Horizontales
                ZonesAction[(int)ZonesInterAction.Bord_Haut] = new Rectangle(PointHautGauche.X + Sensib_Plus, PointHautGauche.Y - Sensibilite, LargeurLigne, Sensib_2_Plus);
                ZonesActionCurseurs[(int)ZonesInterAction.Bord_Haut] = Enumerations.Curseurs.FRefDeplacerHorizontal;

                ZonesAction[(int)ZonesInterAction.Bord_Bas] = new Rectangle(_PointBasGauche.X + Sensib_Plus, _PointBasGauche.Y - Sensibilite, LargeurLigne, Sensib_2_Plus);
                ZonesActionCurseurs[(int)ZonesInterAction.Bord_Bas] = Enumerations.Curseurs.FRefDeplacerHorizontal;
                // Lignes Verticales
                ZonesAction[(int)ZonesInterAction.Bord_Gauche] = new Rectangle(PointHautGauche.X - Sensibilite, PointHautGauche.Y + Sensib_Plus, Sensib_2_Plus, HauteurLigne);
                ZonesActionCurseurs[(int)ZonesInterAction.Bord_Gauche] = Enumerations.Curseurs.FRefDeplacerVertical;

                ZonesAction[(int)ZonesInterAction.Bord_Droit] = new Rectangle(_PointHautDroit.X - Sensibilite, _PointHautDroit.Y + Sensib_Plus, Sensib_2_Plus, HauteurLigne);
                ZonesActionCurseurs[(int)ZonesInterAction.Bord_Droit] = Enumerations.Curseurs.FRefDeplacerVertical;
            }
            /// <summary> initialise les coins du rectangle de sélection lors de sa création sur le plan d'affichage </summary>
            /// <param name="Pt"> position de la souris </param>
            internal override void DebuterActionSouris(Point Pt, ZonesInterAction Zone)
            {
                PointHautGauche = Pt;
                PointBasDroit = Pt;
                ZoneActionEnCours = Zone;
                CurseurSouris.CurseurEncours = TypeCurseurEncours;
                IsOver = true;
            }
            /// <summary> réalise l'action de l'instance modification de taille. </summary>
            /// <param name="Pt">position du pointeur de souris</param>
            internal override void RealiserActionSouris(Point Pt)
            {
                CurseurSouris.CurseurEncours = TypeCurseurEncours;
                // si le pointeur de souris n'est plus dans la surface autorisée on arrête l'action en cours
                if (!SurfaceAutorise.Contains(Pt))
                    return;
                // calcul de Pt0 ou Pt2 en fonction du déplacement de la souris
                switch (ZoneActionEnCours)
                {
                    case ZonesInterAction.Coin_HautGauche:
                        {
                            PointHautGauche = Pt;
                            break;
                        }
                    case ZonesInterAction.Coin_BasDroit:
                        {
                            PointBasDroit = Pt;
                            break;
                        }
                    case ZonesInterAction.Coin_HautDroit:
                        {
                            PointHautGauche.Y = Pt.Y;
                            PointBasDroit.X = Pt.X;
                            break;
                        }
                    case ZonesInterAction.Coin_BasGauche:
                        {
                            PointHautGauche.X = Pt.X;
                            PointBasDroit.Y = Pt.Y;
                            break;
                        }
                    case ZonesInterAction.Bord_Haut:
                        {
                            PointHautGauche.Y = Pt.Y;
                            break;
                        }
                    case ZonesInterAction.Bord_Bas:
                        {
                            PointBasDroit.Y = Pt.Y;
                            break;
                        }
                    case ZonesInterAction.Bord_Gauche:
                        {
                            PointHautGauche.X = Pt.X;
                            break;
                        }
                    case ZonesInterAction.Bord_Droit:
                        {
                            PointBasDroit.X = Pt.X;
                            break;
                        }
                }
                CalculerZonesAction();
                if (ModeTravail == Mode.Selection)
                {
                    // on trouve les rectangles simples survolés par la surface du rectangle de travail afin de 
                    // pouvoir les visualiser comme des rectangles simples survolés
                    var St = Rectangles[Travail].Surface;
                    for (int IndexRectangle = 1, loopTo = Rectangles.Count - 1; IndexRectangle <= loopTo; IndexRectangle++)
                    {
                        var R = Rectangles[IndexRectangle];
                        R.IsOver = R.Surface.IntersectsWith(St);
                    }
                }
            }
            /// <summary> Permet de revenir au mode 'Inactif' de l'instance. C'est l'équivalent du click </summary>
            internal override void FinirActionSouris()
            {
                IsOver = false; // on dessine le rectangle en mode inactif
                ZoneActionEnCours = ZonesInterAction.Aucune;
                ActionTravail = TypeActionLibelle.Aucune;

                CurseurSouris.CurseurEncours = TypeCurseurEncours;
                if (ModeTravail == Mode.Surface)
                {
                    // cas où le rectangle de travail est créé
                    if (Largeur < DistanceMini)
                    {
                        PointBasDroit.X = PointHautGauche.X + DistanceMini;
                    }
                    if (Hauteur < DistanceMini)
                    {
                        PointBasDroit.Y = PointHautGauche.Y + DistanceMini;
                    }
                }
                else
                {
                    var R = Surface;
                    // la surface de travail n'a pas pour vocation à rester affiché sur le plan
                    CreerRectangleTravail(Rectangle.Empty);
                    // il n'y a pas de modification spécifique associée au modifieur alt dans le mode Sélection
                    if (ModifierAction == Keys.Alt)
                        ModifierAction = Keys.None;
                    // sélectionner les rectangles simples en fonction du modifier
                    for (int IndexRectangle = 1, loopTo = Rectangles.Count - 1; IndexRectangle <= loopTo; IndexRectangle++)
                    {
                        var Rsimple = Rectangles[IndexRectangle];
                        if (R.IntersectsWith(Rsimple.Surface))
                            ModifierSelection(Rsimple);
                        // le marquage du survol doit être effacé sauf pour le dernier rectangle simple qui a été marqué
                        Rsimple.IsOver = IndexRectangle == NumRectangleSurvol;
                    }
                    // le rectangle de travail agit comme un click sur les rectangles simples survolés 
                    ClickRectangle = true;
                }
            }
            internal ObjetRectangle_Travail(Point PtHG, Point PtBD, string Info, int Num)
            {
                NbZonesSensibleLH = 3;
                ZonesAction = new Rectangle[9];
                ZonesActionCurseurs = new Enumerations.Curseurs[9];
                Initialiser(PtHG, PtBD, Info, Num);
                // couleur du rectangle de sélection en affichage normal
                PinceauBase = new Pen(Color.Black, 2f); // s_p_Base
                                                        // couleur du rectangle de sélection en mode création ou redimensionnent
                if (ModeTravail == Mode.Surface)
                {
                    PinceauSpecial = new Pen(Color.Magenta, 1f);
                }
                else
                {
                    PinceauSpecial = new Pen(Color.Orange, 2f);
                }
            }
            /// <summary>variable associée à la propriété type de la forme</summary>
            protected override string Type
            {
                get
                {
                    return "Rectangle_Travail";
                }
            }
        }
        #endregion
    }
}