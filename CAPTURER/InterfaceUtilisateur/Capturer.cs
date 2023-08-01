using static FCGP.AfficheInformation;
using static FCGP.Altitudes;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.CurseurSouris;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.Settings;
using static FCGP.SystemeCartographique;

namespace FCGP
{
    /// <summary> formulaire principal de l'application FCGP_Capturer </summary>
    internal sealed partial class Capturer
    {
        #region Champs et propriétés
        /// <summary> libellés des sites et Domstoms pour le remplissage de la liste déroulante de sélection des sites </summary>
        private readonly string[] LibellesSitesDomTom = new string[] { "Géofoncier", "Suisse Mobile", "Réunion", "Martinique", "Guadeloupe", "St Martin",
                                                                       "Guyane", "Mayotte", "St Barthélémy", "CycleOSM", "OpenTopo Map", "Ign Espagnol" };
        /// <summary> Liste des différentes unités pour les coordonnées du pointeur de souris. Evite une variable static interne méthode </summary>
        private readonly string[] CoordType = new string[] { "DD WGS84", "DMS WGS84", "UTM WGS84", "TUILE Couche N°00" };
        /// <summary> conserve le nb de crans de la molette de la souris. Evite une variable static interne méthode </summary>
        private int MemoireDeltaSouris;
        /// <summary> Liste des différentes actions possibles sur l'affichage </summary>
        private enum Actions : int
        {
            Aucune = 0,          // rien ou c'est la trace qui a la main
            DefinirPoints,       // Début de la définition des 2 points de capture avec la touche Alt + le click et déplacement de la souris
            FinirDefinirPoints,  // fin de la définition des 2 points de capture avec la touche Alt + le click et déplacement de la souris
            DeplacerCarte        // la carte est en cours de déplacement
        }
        private string NomBaseCacheTuile;
        /// <summary> action encours sur l'affichage </summary>
        private Actions Action;
        /// <summary> Pas en pixels utilisé par les touches de déplacement pixels avec les modifiers </summary>
        private readonly int[] PasTouchesPixels = new int[] { 1, 5, 25, 0, 125 };
        /// <summary> Pas en % de la hauteur ou de la largeur de la surface d'affichage, utilisé par les touches de déplacement écran avec les modifiers </summary>
        private readonly double[] PasTouchesAffichage = new double[] { 1.0d, 0.25d, 0.5d, 0d, 0.75d };
        /// <summary> flag indiquant que le programme est en mode initialisation. Certains évenement ne seront pas traité ou modifié </summary>
        private bool FlagInit = true;
        /// <summary> flag indiquant qu'il s'agit d'un évenement déclencher par programmation et pas par IU </summary>
        private bool FlagEchelle;
        /// <summary> Taille maximale des tampons d'affichage. sert à dimensionner la reserve d'octets nécessaires en mémoire </summary>
        private Size TailleMaxTamponAffichage;
        /// <summary> tampon réservé à l'affichage </summary>
        private SharedPinnedByteArray ReserveOctetsAffichage;
        /// <summary> permet de connaitre la position du pointeur de souris afin de calculer les coordonnées pixels du pointeur et de calculer un delta de position du pointeur </summary>
        private Point PosMouse;
        /// <summary>altitude à la position du pointeur de souris </summary>
        private short AltitudePosMouse;
        /// <summary> flag indiquant si les coordonnées à traiter sont associées au pointeur de souris ou au centre de l'affichage </summary>
        private bool FlagPosMouse;
        /// <summary> flag indiquant si une carte est encours de création </summary>
        private bool FlagCapture;
        /// <summary> flag indiquant qu'il y a eu une action AllerA de réalisée </summary>
        private bool FlagRevenir;
        /// <summary> Dans le cas d'une action AllerA le site de destination concerné. </summary>
        private SitesCartographiques SiteAllerA;
        /// <summary> Dans le cas d'un changement de site avec le menu contextuel de la visue la position du centre du site sélectionné. </summary>
        private PointG PositionSiteAllerA;
        /// <summary> Coordonnees de sauvegarde lors d'une action AllerA </summary>
        private PointD CoordonneesRevenir;
        /// <summary> label sur lequel est positionné le curseur de la souris </summary>
        private Label LabelEncours;
        /// <summary> nom du Pointlabel sur lequel est le curseur de la souris </summary>
        private string NomPointEncours;
        /// <summary> différentes variables pour le dessin des points et de la surface de capture </summary>
        private Pen PinceauPoint, PinceauEchelle_1, PinceauEchelle_2, PinceauLimites;
        private SolidBrush BrosseOk, BrosseNotOk, BrosseEtiquette;
        private Size EtiquettePoint, DemiEpaisseur;
        /// <summary> Indique la surface maximale de décalage dans la visue </summary>
        private Rectangle SurfaceDecalage;
        /// <summary> surface de capture définit à partir des 2 points de capture </summary>
        private SurfaceTuiles SurfaceTraitement;
        /// <summary> flag qui indique si la surface de traitement est ok pour une création de carte, une suppression de tuiles ou une vue 3D (à venir) </summary>
        private bool SurfaceTraitementIsOk;
        /// <summary> indique qu'un menu associé à un label est ouvert </summary>
        private bool FlagContextMenuShow;
        /// <summary> combox qui est ouverte lors d'un click sur un label de choix possibles (Sites, Echelle ou TypeCoordonnées) </summary>
        private ComboBox ComboxDropDown;
        /// <summary> Limites en coordonnées virtuelles pour lesquelles le site est définit </summary>
        internal Rectangle LimitesVirtuellesSite;
        /// <summary> location ou Pt0 de l'affichage en Coordonnées bureau. définit comme size pour éviter les transtypages </summary>
        private Size AffichageOffsetEcran;
        /// <summary> Centre de l'affichage de l'affichage en Coordonnées bureau </summary>
        private Point CentreAffichage;
        #endregion
        #region Evènements de Capturer, des 3 listes déroulantes et des procédures associées
        /// <summary> initialisation de l'application </summary>
        private void Capturer_Load(object sender, EventArgs e)
        {
            if (!InitialiserBaseApplication(this, "Core 9.200 CS", "01/05/2023"))
            {
                Close();
                return;
            }
            TitreInformation = "Information FCGP_Capturer";
            // indique aux threads hors UI la procédure à appeler pour afficher un message d'information ou une erreur
            FormHorsUI = new Action(AfficherInfomationsErreurs);

            if (!InitialiserSettings("9.000"))
            {
                Close();
                return;
            }
            if (!InitialiserCommun())
            {
                Close();
                return;
            }
            if (!InitialiserCapturer())
            {
                Close();
            }

            // sélectionne l'index dans la liste déroulante des Sites et DomsToms avec le dernier site visité
            ListeChoixSites.SelectedIndex = CapturerSettings.INDICE_SITE;
            if (FlagAborted)
            {
                // l'initialisation de l'affichage s'est mal passé on ferme l'application
                var Ex = new Exception("Erreur inconnue lors de l'initialisation du site de capture" + CrLf +
                                       "Fermeture de l'application");
                AfficherErreur(Ex, "Y1H3");
                Close();
            }
            else
            {
                FlagInit = false;
            }
        }
        /// <summary> verifie si on peut sortir correctement du programme </summary>
        private void Capturer_FormClosing(object sender, FormClosingEventArgs e)
        {
            int NbDemandes = Serveur is not null ? Serveur.NbDemandesAffichage : 0;
            if (NbDemandes > 0) // cas de l'affichage en cours de mise à jour
            {
                MessageInformation = "Le formulaire sera fermé quand toutes " + CrLf + "les demandes de tuiles seront terminées." + CrLf +
                                     $"{NbDemandes} demande{(NbDemandes > 1 ? "s" : "")} de tuiles {(NbDemandes > 1 ? "sont" : "est")} en attente.";
                AfficherInformation();
                e.Cancel = true;
                return;
            }
            if (FlagCapture) // cas de la carte en cours de création
            {
                MessageInformation = "Vous ne pouvez pas quitter l'application" + CrLf + "tant qu'une carte est en cours de création.";
                AfficherInformation();
                e.Cancel = true;
            }
        }
        /// <summary> sauvegarde de l'environement de la visue et libération des ressources </summary>
        private void Capturer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (FlagAborted)
            {
                MessageInformation = "Problème dans l'initialisation de l'application FCGP_Capturer" + CrLf +
                                     "Veuillez le signaler à " + Contact + CrLf +
                                     "L'application va être fermée";
            }
            else if (Affichage is not null)
            {
                // on sauvegarde toutes les paramètres concernant le site actuel
                if (IsSettingsOk)
                {
                    TRK.CloturerTraces();
                    CapturerSettings.LOCATION_CENTRE = Affichage.CentreAffichage;
                    CapturerSettings.Ecrire();
                    CapturerSettings.EcrirePosition();
                }
            }
            CompacterCache();
            CloturerAltitudes();
            CloturerSettings();
            CloturerCommun();
            CloturerCapturer();
        }
        /// <summary> marque les touches de direction pour pouvoir gèrer le déplacement de la carte </summary>
        private void Capturer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case var case1 when Keys.Left <= case1 && case1 <= Keys.Down:
                case var case2 when Keys.PageUp <= case2 && case2 <= Keys.Home:
                    {
                        e.IsInputKey = true;
                        break;
                    }
            }
        }
        /// <summary> gestion des actions déclenchées par les touches du clavier </summary>
        private void Capturer_KeyDown(object sender, KeyEventArgs e)
        {
            // on ne prend pas en compte l'événement si une action est déjà lancée
            if (Action == Actions.Aucune)
            {
                if (TouchesMenuTrace(e))
                    return;
                if (TouchesMenuVisue(e))
                    return;
                if (TouchesMenuApp(e))
                    return;
                if (TouchesMenuPoints(e))
                    return;
                if (TouchesDeplacementCarte(e))
                    return;
                if (TouchesDeplacementCurseur(e))
                    return;
                // touches du formulaire
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        {
                            // ferme le menu qui est ouvert ou la liste de choix ouverte. La fermeture effective du menu est faite par
                            // l'événement ContextMenu_Closed provoqué par DroppedDown
                            if (ComboxDropDown is not null)
                                ComboxDropDown.DroppedDown = false;
                            break;
                        }
                    case Keys.F11:
                        {
                            if (e.Modifiers == Keys.Alt)
                            {
                                AppliquerLimitesSite();
                            }
                            else if (e.Modifiers == Keys.None)
                            {
                                if (WindowState == FormWindowState.Maximized)
                                {
                                    WindowState = FormWindowState.Normal;
                                }
                                else
                                {
                                    WindowState = FormWindowState.Maximized;
                                }
                            }

                            break;
                        }
                }
                // on ne fait pas suivre les touches inutilsées
                e.SuppressKeyPress = true;
            }
        }
        /// <summary> l'action DeplacerCarte n'est pas concernée par l'événement KeyUp </summary>
        private void Capturer_KeyUp(object sender, KeyEventArgs e)
        {
            if (Action == Actions.DefinirPoints)
            {
                // le bouton gauche de la souris a été relevé en 1er, on ne peut pas finir l'action
                FinirActionDefinirPoints();
            }
            else if (Action == Actions.FinirDefinirPoints)
            {
                // la touche menu a été relevé en premier on peut fini l'action
                Action = Actions.Aucune;
                TrouverCurseurCarte();
            }
            else if (Action != Actions.DeplacerCarte && TRK.IsTraceEncoursModeEdition)
            {
                // on transmet à la trace l'évenement, il faut réaliser l'ActionEncours
                if (TRK.TraceEncours.ToucheUp(e.KeyCode))
                {
                    // la trace a pris l'événement à son compte
                    SurfaceAffichageTuile.Invalidate();
                }
            }
        }
        /// <summary> gestion des déplacements du formulaire et redimensionnent du formulaire </summary>
        private void Capturer_LocationChanged(object sender, EventArgs e)
        {
            if (!FlagInit)
            {
                VisueRectangle = SurfaceAffichageTuile.RectangleToScreen(SurfaceAffichageTuile.ClientRectangle);
                AffichageOffsetEcran = (Size)VisueRectangle.Location;
            }
        }
        /// <summary> met à jour la visue quand le site change à partir de la liste déroulante ou à l'ouverture du programme </summary>
        private void ListeChoixSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelSites.Text = ListeChoixSites.SelectedItem.ToString();
            // si ce n'est pas l'ouverture du programme
            if (!FlagInit)
            {
                // il faut sauver la position centre visue et des autres données du site actuel avant d'effectuer le changement
                CapturerSettings.LOCATION_CENTRE = Affichage.CentreAffichage;
                // on change de site donc on ferme la trace encours
                if (TRK.IsTraceEncours)
                {
                    // d'abord on sauvegarde son nom
                    CapturerSettings.NOM_TRACE = TRK.TraceEncours.Nom;
                    TRK.FermerTraceEncours(false);
                }
                else
                {
                    CapturerSettings.NOM_TRACE = "";
                }
                CapturerSettings.EcrirePosition();
                CapturerSettings.INDICE_SITE = ListeChoixSites.SelectedIndex;
                // et supprime le serveur web associé à l'ancien site de capture
                Serveur.Dispose();
            }
            else
            {
                // on crée le support d'affichage
                Affichage = new AffichageCarte(ReserveOctetsAffichage, TailleMaxTamponAffichage, PartagerSettings.COULEUR_VISUE);
                // si l'initialisation de l'affichage ne s'est pas bien passée on sort
                if (!Affichage.IsOk)
                    return;
                // pour le site on garde les valeurs liées à l'initialistion du settings c'est à dire le dernier visité
            }
            // on lit les paramètres enregistrés du nouveau site 
            CapturerSettings.LirePosition();
            // Si le changement de site est du à la liste déroulante du menu contextuel de la visue il faut modifier la position et l'échelle
            if (SiteAllerA > SitesCartographiques.Aucun)
            {
                CapturerSettings.INDICE_ECHELLE = PositionSiteAllerA.IndiceEchelle;
                CapturerSettings.LOCATION_CENTRE = PositionSiteAllerA.Location;
                SiteAllerA = SitesCartographiques.Aucun;
            }
            GererMenuRevenir(false);
            // on créé le serveur associé au nouveau site de capture
            Serveur = new ServeurCarto(Affichage);
            if (!Serveur.IsOk)
                return;
            // on initialise l'affichage (dimensions et IU)
            InitialiserAffichage();
            // fin de l'initialisation de FCGP_Capturer, tout c'est bien passé
            FlagAborted = false;
        }
        /// <summary> met à jour la visue quand l'echelle change à partir de la liste déroulante ou de la souris </summary>
        private void ListeChoixEchelles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Delta = default(int);
            var PositionCoordonnees = default(Point);
            if (!FlagEchelle)
            {
                // changement par la liste déroulante ou souris wheel. On récupère le point à afficher au centre de l'affichage
                PositionCoordonnees = FlagPosMouse ? PosMouse : CentreAffichage;
                // et l'écart entre l'indice actuel et demandé. Toujours 1 ou -1 pour la souris wheel
                Delta = ListeChoixEchelles.SelectedIndex - CapturerSettings.INDICE_ECHELLE;
                // on indique la nouvelle échelle
                CapturerSettings.INDICE_ECHELLE = ListeChoixEchelles.SelectedIndex;
            }
            // on maj les variables qui dépendent de l'échelle
            Affichage.CoucheFond = Serveur.CouchesLayer(0)[CapturerSettings.INDICE_ECHELLE];
            Affichage.Echelle = NiveauDetailCartographique.SiteIndiceEchelleToEchelle(CapturerSettings.SITE, CapturerSettings.INDICE_ECHELLE);
            Affichage.CoucheFondIspentes = Serveur.IsIndiceEchellesPentes(CapturerSettings.INDICE_ECHELLE);
            LimitesVirtuellesSite = RegionGrilleToRegionPixels(Serveur.Limites, Serveur.SiteCarto, Affichage.Echelle);
            SysCartoEncours = new SystemeCartographique(CapturerSettings.INDICE_SITE, Affichage.Echelle, Serveur.Datum);
            ListeChoixTypeCoordonnees.Items[4] = "TUILE Couche N°" + Affichage.CoucheFond.ToString("00");
            if (!FlagEchelle)
            {
                // on recalcule le monde virtuel de l'éventuelle trace en cours
                TRK.ChangerSiteEchelleTraces(CapturerSettings.SITE, CapturerSettings.DOMTOM, Affichage.Echelle, true);
                // copie l'ancien tampon en position et facteur pour faire patienter pendant la demande de remplissage du tampon 
                GererChangementCouche(Delta, PositionCoordonnees);
            }
            // on recalcule la surface de traitement pour savoir si elle encore ok
            GererIUMenuPts();
            LabelEchelles.Text = ListeChoixEchelles.SelectedItem.ToString();
            FlagEchelle = false;
        }
        /// <summary> change l'affichage des coordonnées du pointeur de souris </summary>
        private void ListeChoixTypeCoordonnees_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapturerSettings.INDICE_TYPE_COORDONNEES = ListeChoixTypeCoordonnees.SelectedIndex;
            AfficherCoordonnees();
        }
        /// <summary> Initialise l'affichahe lors d'un changement de site de capture </summary>
        private void InitialiserAffichage()
        {
            // on met à jour la liste des types de coordonnées qui dépend du datum du site
            MettreAjourTypeCoordonnees();
            ListeChoixTypeCoordonnees.SelectedIndex = CapturerSettings.INDICE_TYPE_COORDONNEES;
            // on met à jour l'affichage sur l'IU'on met à jour la liste déroulante des échelles qui dépend du site
            MettreAJourListeEchelles();
            // MAJ des variables qui dépendent de l'échelle du site
            FlagEchelle = true;
            ListeChoixEchelles.SelectedIndex = CapturerSettings.INDICE_ECHELLE;
            FlagEchelle = false;
            // configuration du menu contextuel de la visue
            GererMenuVisue();
            // eventuellement chargement de la trace associée au site de capture
            TRK.ChargerTraceCaptureSettings(Affichage.Echelle);
            GererMenuTrace();
            // MAJ des variables de l'affichage qui dépendent de la taille et du point central à afficher
            if (FlagInit)
            {
                // si c'est l'ouverture du programme, on initialise l'affichage à partir de l'évenement resize à 80% de la taille de l'écran
                DimensionnerAffichage();
            }
            else
            {
                // cas du changement de site carto où il faut demander explicitement la réinitialisation de l'affichage
                FlagInit = true;
                GererChangementTaille();
                FlagInit = false;
            }
        }
        /// <summary> gère le menus contextuel de l'affichage (click droit sur la visue) en fonction du site </summary>
        private void GererMenuVisue()
        {
            // flag indiquant que le site peut renvoyer directement sur un site OSM
            bool FlagChangeSite = IsNationalToEurope(CapturerSettings.SITE);
            MenuVisueSeparateur_2.Visible = FlagChangeSite;
            // on interdit l'évenement de se produire
            ChoisitSitesOSM.Visible = false;
            ChoisitSitesOSM.SelectedIndex = CapturerSettings.INDICE_OSM;
            ChoisitSitesOSM.Visible = FlagChangeSite;

            AfficheDessinEchelle.Text = ChangerTextBascule(false);

            // flag indiquant que le site peut afficher une couche des pentes
            bool FlagPentes = Serveur.IsPentes;
            AfficheCouchePentes.Text = ChangerTextBascule(true);
            ChoisitCoefAlphaPentes.SelectedIndex = CapturerSettings.INDICE_COEF_ALPHA_PENTES;
            AfficheCouchePentes.Visible = FlagPentes;
            ChoisitCoefAlphaPentes.Visible = FlagPentes;
            float CoefAlphaPentes = CoefsAlphasPentes[ChoisitCoefAlphaPentes.SelectedIndex];
            ImageAttributsPentes.SetColorMatrix(new ColorMatrix() { Matrix33 = CoefAlphaPentes }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            // flag indiquant que le site à plusieurs couches possible d'affichage (2 ou 3)
            bool FlagFondPlan = Serveur.IsFondsPlan;
            if (FlagFondPlan)
            {
                ChoisitFondsPlan.Items.Clear();
                ChoisitFondsPlan.Items.AddRange(Serveur.TypesFondsPlan);
                ChoisitFondsPlan.SelectedIndex = CapturerSettings.INDICE_FOND_PLAN;
            }
            MenuVisueSeparateur_1.Visible = FlagFondPlan;
            ChoisitFondsPlan.Visible = FlagFondPlan;
        }
        /// <summary> mise à jour des items de la liste déroulante des échelles </summary>
        private void MettreAJourListeEchelles()
        {
            ListeChoixEchelles.Items.Clear();
            ListeChoixEchelles.Items.AddRange(Serveur.EchellesLayer(0));
        }
        /// <summary> mise à jour de la liste déroulante des types de coordonnées du pointeur de souris </summary>
        private void MettreAjourTypeCoordonnees()
        {
            ListeChoixTypeCoordonnees.Items.Clear();
            ListeChoixTypeCoordonnees.Items.Add(DatumsLibelles[(int)Serveur.Datum]);
            ListeChoixTypeCoordonnees.Items.AddRange(CoordType);
        }
        /// <summary> Verifie toutes les 15 secondes que la connection internet n'est pas tombée </summary>
        private async void ConnectedInternet_Tick(object sender, EventArgs e) // Handles ConnectedInternet.Tick
        {
            await TesterConnectionInternetAsync();
            LabelConnexionInternet.BackColor = IsConnected ? Color.YellowGreen : Color.Tomato;
            LabelConnexionInternet.Refresh();
        }
        /// <summary> initialise les différentes variables privées </summary>
        private bool InitialiserCapturer()
        {
            Text = NumFCGP; // mise à jour du titre du formulaire
                            // initialisation des différents curseurs utilisés par FCGP
            InitialiserCurseurs(SurfaceAffichageTuile, Cursor);
            // initialisation des fichiers altitudes et de la connexion si besoin
            if (AltitudesSettings.IS_ALTITUDE)
            {
                InitialiserListeFichiersAltitudes();
                InitialiserConnexionAltitudes();
            }
            PinceauEchelle_1 = new Pen(Color.Black, 4f);
            PinceauEchelle_2 = new Pen(Color.Black, 2f);
            PinceauPoint = new Pen(Color.Red, 2f);
            PinceauLimites = new Pen(Color.FromArgb(96, Color.DarkCyan), 10f);
            BrosseOk = new SolidBrush(Color.FromArgb(96, Color.YellowGreen));
            BrosseNotOk = new SolidBrush(Color.FromArgb(64, Color.Tomato));
            BrosseEtiquette = new SolidBrush(Color.FromArgb(128, Color.White));
            FontEtiquette = new Font("Segoe UI", 14.0f, FontStyle.Bold, GraphicsUnit.Pixel);
            EtiquettePoint = SurfaceAffichageTuile.CreateGraphics().MeasureString("Pt0", FontEtiquette).ToSize();
            DemiEpaisseur = new Size(1, 0);
            SurfaceDecalage = new Rectangle(Point.Empty, new Size(NbPixelsTuile, NbPixelsTuile));
            // dimensionnement du tampon d'affichage en fonction de la résolution maximale des écrans et du nb de tuiles serveur pour le recouvrir
            var SizeTuile = new Size(PixelToNumTile(MaxBoundsEcran.Width + NbPixelsTuile, false) + 1, PixelToNumTile(MaxBoundsEcran.Height + NbPixelsTuile, false) + 1);
            TailleMaxTamponAffichage = new Size(StrideImage(SizeTuile.Width * NbPixelsTuile), SizeTuile.Height * NbPixelsTuile);
            // la mémoire liée à l'affichage est en dehors de la réserve commune. 2 tampons Pour la couche par défaut
            ReserveOctetsAffichage = new SharedPinnedByteArray(TailleMaxTamponAffichage.Width, TailleMaxTamponAffichage.Height * 2, null, "AffichageVisue");
            if (!ReserveOctetsAffichage.IsOk)
            {
                return false;
            }
            TRK.InitialiserTraces(new TRK.AfficherGraphiqueTrace(AfficherGraphiqueTrace));
            // remplisage de la liste des sites
            ListeChoixSites.Items.AddRange(LibellesSitesDomTom);
            // remplisage de la liste des sites OSM Européan du menu contextuel de la visue
            for (int Cpt = 0, loopTo = SitesOSMEurope.Length - 1; Cpt <= loopTo; Cpt++)
                ChoisitSitesOSM.Items.Add(LibellesSitesDomTom[SiteDomTomToIndex(SitesOSMEurope[Cpt], DomsToms.aucun)]);
            SiteAllerA = SitesCartographiques.Aucun;
            ImageAttributsPentes = new ImageAttributes();
            // test de la connection internet et lance sa vérification toutes le 10 secondes
            ConnectedInternet_Tick(null, null);
            ConnexionInternet.Enabled = true;
            ConnexionInternet.Start();
            CurseurEncours = Curseurs.CarteDefaut;
#if !BETA
            VerifierVersionApplication();
#endif
            return true;
        }
        /// <summary> libère les ressources du formulaire </summary>
        private void CloturerCapturer()
        {
            if (BrosseEtiquette is not null)
            {
                BrosseEtiquette.Dispose();
                BrosseNotOk.Dispose();
                BrosseOk.Dispose();
                PinceauPoint.Dispose();
                PinceauLimites.Dispose();
                PinceauEchelle_1.Dispose();
                PinceauEchelle_2.Dispose();
                FontEtiquette.Dispose();
            }
            Serveur?.Dispose();
            Affichage?.Dispose();
            ReserveOctetsAffichage?.Dispose();
        }
        /// <summary> Gère le compactage du cache en fonction des réponses de l'utilisateur </summary>
        private void CompacterCache()
        {
            if (!string.IsNullOrEmpty(NomBaseCacheTuile))
            {
                MessageInformation = $"Le compactage du cache tuiles {NomBaseCacheTuile} a été demandé.{CrLf}" + $"Cette opération peut prendre beaucoup de temps{CrLf}Voulez continuer?";
                if (AfficherConfirmation() == DialogResult.Cancel)
                {
                    NomBaseCacheTuile = null;
                }
                else
                {
                    Cursor = Cursors.WaitCursor;
                    LabelInformation.Text = $"Compactage du cache tuiles {NomBaseCacheTuile}";
                    LabelInformation.BackColor = Color.SeaShell;
                    LabelInformation.Refresh();
                    string Titre = TitreInformation;
                    MessageInformation = $"Le compactage du cache tuiles{CrLf}{NomBaseCacheTuile}{CrLf} est encours. Veuillez patienter";
                    TitreInformation = "Compactage d'un cache tuiles";
                    LancerTache(CompacterCacheTuiles);
                    TitreInformation = Titre;
                }
            }
        }
        /// <summary> Compacte le cache tuiles sélectionné </summary>
        private DialogResult CompacterCacheTuiles()
        {
            var Connection = new SQLiteConnection($"Data Source={NomBaseCacheTuile};");
            SQLiteCommand CommandConnection;
            Connection.Open();
            CommandConnection = Connection.CreateCommand();
            CommandConnection.CommandText = "VACUUM 'main';";
            CommandConnection.ExecuteNonQuery();
            Connection.Close();
            CommandConnection.Dispose();
            Connection.Dispose();
            return DialogResult.Yes;
        }
        /// <summary> événement KeyDown du formulaire principal mais spécifique au deplacement du curseur de souris sur la visue </summary>
        /// <param name="Touche"> touche de déplacement </param>
        /// <summary> gére le point Decalage lors d'un déplacement de la souris sur la surface d'affichage </summary>
        private bool TouchesDeplacementCurseur(KeyEventArgs e)
        {
            bool TouchesDeplacementCurseurRet = true;
            switch (e.KeyCode)
            {
                case Keys.NumPad0:
                case Keys.D0:
                    {
                        if (e.Control)
                        {
                            AllerAuPointTraitementSurface(0);
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            Cursor.Position = new Point(VisueRectangle.X, VisueRectangle.Y);
                        }
                        else
                        {
                            TouchesDeplacementCurseurRet = false;
                        }

                        break;
                    }
                case Keys.NumPad1:
                case Keys.D1:
                    {
                        if (e.Control)
                        {
                            AllerAuPointTraitementSurface(1);
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            Cursor.Position = new Point(VisueRectangle.X + VisueRectangle.Width - 1, VisueRectangle.Y);
                        }
                        else
                        {
                            TouchesDeplacementCurseurRet = false;
                        }

                        break;
                    }
                case Keys.NumPad2:
                case Keys.D2:
                    {
                        if (e.Control)
                        {
                            AllerAuPointTraitementSurface(2);
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            Cursor.Position = new Point(VisueRectangle.X + VisueRectangle.Width - 1, VisueRectangle.Y + VisueRectangle.Height - 1);
                        }
                        else
                        {
                            TouchesDeplacementCurseurRet = false;
                        }

                        break;
                    }
                case Keys.NumPad3:
                case Keys.D3:
                    {
                        if (e.Control)
                        {
                            AllerAuPointTraitementSurface(3);
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            Cursor.Position = new Point(VisueRectangle.X, VisueRectangle.Y - 1 + VisueRectangle.Height);
                        }
                        else
                        {
                            TouchesDeplacementCurseurRet = false;
                        }

                        break;
                    }
                case Keys.NumPad4:
                case Keys.D4:
                    {
                        if (e.Modifiers == Keys.None)
                        {
                            DeplacerCurseurCentreVisue();
                        }
                        else
                        {
                            TouchesDeplacementCurseurRet = false;
                        }

                        break;
                    }

                default:
                    {
                        TouchesDeplacementCurseurRet = false;
                        break;
                    }
            }
            e.SuppressKeyPress = TouchesDeplacementCurseurRet;
            return TouchesDeplacementCurseurRet;
        }
        /// <summary> procédure appeler par le formulaire Proprité des traces pour afficher un Graphique </summary>
        /// <param name="Trace"> trace concernée par le graphique </param>
        /// <param name="Is_Vitesse"> s'agit'il du graphique Vitesse / par longueur de trace </param>
        private (int IndexPoint, DialogResult DialogResult) AfficherGraphiqueTrace(TRK Trace, bool Is_Vitesse)
        {
            using (var F = new AfficheGraphiqueTrace() { Trace = Trace, Is_Vitesse = Is_Vitesse })
            {
                F.ShowDialog(this);
                return (F.IndexPoint, F.DialogResult);
            }
        }
        /// <summary> procédure appeler par le module Information pour éviter les appels inter-threads </summary>
        internal void AfficherInfomationsErreurs()
        {
            AfficherInformation();
        }
        #endregion
        #region Demande de tuiles et remplissage du tampon
        /// <summary> demande la creation d'une demande de tuile </summary>
        /// <param name="TotalTuiles"> nb de tuiles concernées par la demande </param>
        private async void CreerDemandeTuilesAsync(Rectangle Surface, byte Couche)
        {
            if (Serveur.NbDemandesAffichage == 0)
            {
                // pour l'affichage automatique des tuiles à leur arrivée sur la surface d'affichage
                RafraichisseurVisue.Start();
                ListeChoixSites.Enabled = false;
                GererMenuLancerCreation();
                LabelNbDemandesTuiles.BackColor = Color.Tomato;
                ConfigureApp.Enabled = false;
            }
            var Demande = Serveur.CreerDemandeAffichage(Surface.Width * Surface.Height, Couche);
            // on s'abonne à l'évenement de fin de réalisation de la demande
            Demande.Finished += SupprimerDemandeTuiles;
            await Demande.CreerTuilesAffichageAsync(Surface);
            LabelNbDemandesTuiles.Text = Serveur.NbDemandesAffichage.ToString();
            LabelNbDemandesTuiles.Refresh();
        }
        /// <summary> demande la suppression d'une demande de tuile </summary>
        /// <param name="ID_Demande"> identifiant de la demande </param>
        private void SupprimerDemandeTuiles(int ID_Demande, bool IsErreur)
        {
            if (IsErreur)
            {
                MessageInformation = $"Impossible de télécharger de nouvelles tuiles{CrLf + Serveur.DemandesAffichage(ID_Demande).ToString() + CrLf}" + 
                                     "Le serveur ne répond pas ou la connexion internet est tombée";
                AfficherInformation();
            }
            // on supprime la demande
            Serveur.SupprimerDemandeAffichage(ID_Demande);
            LabelNbDemandesTuiles.Text = Serveur.NbDemandesAffichage.ToString();
            LabelNbDemandesTuiles.Refresh();
            if (Serveur.NbDemandesAffichage == 0)
            {
                // on arrête la mise à jour automatique de l'affichage
                RafraichisseurVisue.Stop();
                ListeChoixSites.Enabled = true;
                GererMenuLancerCreation();
                LabelNbDemandesTuiles.BackColor = Color.YellowGreen;
                ConfigureApp.Enabled = true;
                FlagRemplirTampon = true;
                // on lance la maj de l'affichage pour être sur que les dernières tuiles téléchargées sont prises en compte
                SurfaceAffichageTuile.Invalidate();
            }
        }
        /// <summary> lance la copie sur le tampon de toutes les tuiles qui composent la surface d'affichage </summary>
        private void RemplirTampon()
        {
            FlagRemplirTampon = true;
            // demander le téléchargement des tuiles de fond de plan
            CreerDemandeTuilesAsync(Affichage.SurfaceFond, Affichage.CoucheFond);
            if (IsAffichagePentes)
            {
                // demander le téléchargement des tuiles des pentes
                CreerDemandeTuilesAsync(Affichage.SurfacePentes, 255);
            }
        }
        #endregion
        #region Afficher les coordonnees et mettre à jour le dessin de la carte
        /// <summary> sert quand on a besoin d'un automatisme pour repeindre la surface d'affichage lors d'une DemandeTuile </summary>
        private void RafraichisseurVisue_Tick(object sender, EventArgs e)
        {
            SurfaceAffichageTuile.Invalidate();
        }
        /// <summary> affiche les coordonnées du curseur avec le type spécifié par l'unité sélectionnée </summary>
        private void AfficherCoordonnees()
        {
            switch (ListeChoixTypeCoordonnees.SelectedIndex)
            {
                case 3:
                    {
                        LabelCoordonnees.Text = CoordMouseUtmWGS84;
                        break;
                    }
                case 2:
                    {
                        LabelCoordonnees.Text = CoordMouseDMSText;
                        break;
                    }
                case 1:
                    {
                        LabelCoordonnees.Text = CoordMouseDDText;
                        break;
                    }
                case 0:
                    {
                        LabelCoordonnees.Text = CoordMouseGrilleText;
                        break;
                    }
                case 4:
                    {
                        LabelCoordonnees.Text = CoordMousePointG.ToString();
                        break;
                    }
            }
            if (AltitudesSettings.IS_ALTITUDE)
            {
                LabelAltitude.Text = AltitudePosMouse == -9999 ? "0 m" : AltitudePosMouse.ToString() + " m";
            }
            else
            {
                LabelAltitude.Text = "---- m";
            }
        }
        /// <summary> demande la mise à jour de la surface d'affichage </summary>
        private void RepeindreSurfaceAffichage()
        {
            AfficherCoordonnees();
            // si il y a une capture on affiche les informations la concernant
            if (!FlagCapture)
            {
                LabelInformation.Text = SurfaceTraitement.ToString();
                LabelInformation.BackColor = Color.Transparent;
            }
            else
            {
                LabelInformation.BackColor = Color.SeaShell;
            }
            SurfaceAffichageTuile.Invalidate();
        }
        /// <summary> redessine la surface d'affichage </summary>
        private async void SurfaceAffichageTuile_Paint(object sender, PaintEventArgs e)
        {
            if (!FlagAborted)
            {
                using (var Image = await Affichage.ImageAffichage(FlagRemplirTampon))
                {
                    // on ne translate pas avec e.Graphics.TranslateTransform(-Affichage.LocationAffichage.X, -Affichage.LocationAffichage.Y)
                    // car il y a des transformations qui induisent des artefacts à l'affichage, dommage ce serait beaucoup plus simple
                    e.Graphics.DrawImage(Image, new Rectangle(new Point(-Affichage.DecalageTamponX, -Affichage.DecalageTamponY), Image.Size));
                    FlagRemplirTampon = false;
                    if (!SurfaceTraitement.IsEmpty)
                        DessinerSurfaceTraitement(e.Graphics);
                    if (!CapturerSettings.POINT1.IsEmpty)
                        DessinerPoint(e.Graphics, CapturerSettings.POINT1, "Pt1");
                    if (!CapturerSettings.POINT2.IsEmpty)
                        DessinerPoint(e.Graphics, CapturerSettings.POINT2, "Pt2");
                    var SauveEtats = e.Graphics.BeginContainer();
                    e.Graphics.TranslateTransform(-Affichage.LocationAffichage.X, -Affichage.LocationAffichage.Y);
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    TRK.DessinerTraces(e.Graphics, Rectangle.Empty, true);
                    if (FlagLimitesSite)
                        e.Graphics.DrawRectangle(PinceauLimites, LimitesVirtuellesSite);
                    e.Graphics.EndContainer(SauveEtats);
                    // on est revenu par rapport à l'affichage
                    if (CapturerSettings.IS_AFFICHE_ECHELLE)
                        DessinerEchelle(e.Graphics);
                }
            }
        }
        /// <summary> dessinne une échelle sur la visue en bas à droite </summary>
        /// <param name="G"> surface de dessin où l'on dessinne </param>
        private void DessinerEchelle(Graphics G)
        {
            if (IsDessinEchelle)
            {
                {
                    ref var wB = ref EchelleDessin;
                    // Dessin du texte de l'échelle
                    G.FillRectangle(BrosseEtiquette, wB.RectangleEtiquette);
                    G.DrawString(wB.TexteEchelle, FontEtiquette, Brushes.Black, wB.Pt4);
                    G.DrawLine(PinceauEchelle_1, wB.Pt1, wB.Pt2);
                    // dessin des traits de l'échelle
                    // pour prendre en compte l'épaisseur du trait
                    var Pt1 = Point.Add(wB.Pt1, DemiEpaisseur);
                    var Pt2 = Point.Subtract(wB.Pt2, DemiEpaisseur);
                    G.DrawLine(PinceauEchelle_2, Pt1, new Point(Pt1.X, Pt1.Y - 20));
                    G.DrawLine(PinceauEchelle_2, wB.Pt3, new Point(wB.Pt3.X, wB.Pt3.Y - 10));
                    G.DrawLine(PinceauEchelle_2, Pt2, new Point(Pt2.X, Pt2.Y - 20));
                }
            }
        }
        /// <summary> dessine une croix et ajoute une étiquette avec le nom du point </summary>
        /// <param name="G"> surface de dessin où l'on dessinne </param>
        /// <param name="PointGrille"> point à dessinner en coordonnées grille </param>
        /// <param name="NomPoint"> Nom du point à mettre sur l'étiquette</param>
        private void DessinerPoint(Graphics G, PointD PointGrille, string NomPoint)
        {
            var PointPixelSite = PointGrilleToPointPixel(PointGrille, CapturerSettings.SITE, Affichage.Echelle);
            var Centre = PixelsSiteToPixelsEcran(PointPixelSite);
            G.DrawLine(PinceauPoint, Centre.X - 15, Centre.Y, Centre.X + 15, Centre.Y);
            G.DrawLine(PinceauPoint, Centre.X, Centre.Y - 15, Centre.X, Centre.Y + 15);
            // on dessine un rectangle contenant la taille de l'étiquette
            var Pt4 = new Point(Centre.X + 4, Centre.Y - EtiquettePoint.Height - 4);
            G.FillRectangle(BrosseEtiquette, new Rectangle(Pt4, EtiquettePoint));
            // on ecrit l'étiquette par dessus
            G.DrawString(NomPoint, FontEtiquette, Brushes.Black, Pt4);
        }
        /// <summary> dessine un rectangle translucide pour indiquer la surface de capture à l'écran </summary>
        /// <param name="G"> surface de dessin où l'on dessinne </param>
        private void DessinerSurfaceTraitement(Graphics G)
        {
            var PointPixelSite = SurfaceTraitement.PtPixels_NO;
            var LocEcran = PixelsSiteToPixelsEcran(PointPixelSite);
            var SizeEcran = SurfaceTraitement.RectanglePixels.Size;
            G.FillRectangle(SurfaceTraitementIsOk ? BrosseOk : BrosseNotOk, new Rectangle(LocEcran, SizeEcran));
        }
        #endregion
        #region Gestion de la souris, du clavier sur la Visue et des évenements de la visue
        /// <summary> transforme un point donné en pixels de la surface d'affichage en pixels de la projection grille et pour l'echelle encours </summary>
        private static Point PixelsEcranToPixelsSite(Point PixelsEcran)
        {
            return new Point(Affichage.LocationAffichage.X + PixelsEcran.X, Affichage.LocationAffichage.Y + PixelsEcran.Y);
        }
        /// <summary> transforme un point donné en pixels de la projection grille et pour l'echelle encours, en pixels de la surface d'affichage </summary>
        private static Point PixelsSiteToPixelsEcran(Point PixelsSite)
        {
            return new Point(PixelsSite.X - Affichage.LocationAffichage.X, PixelsSite.Y - Affichage.LocationAffichage.Y);
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en coordonnées du monde virtuel (pixels) </summary>
        private Point CoordMousePixels
        {
            get
            {
                return PixelsEcranToPixelsSite(PosMouse);
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en index tuile et offset. </summary>
        private PointG CoordMousePointG
        {
            get
            {
                return new PointG(CapturerSettings.INDICE_ECHELLE, CoordMousePixels);
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection grille </summary>
        private PointD CoordMouseGrille
        {
            get
            {
                return PointPixelToPointGrille(CoordMousePixels, CapturerSettings.SITE, Affichage.Echelle);
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection Grille sous forme de chaine caractères formatée </summary>
        private string CoordMouseGrilleText
        {
            get
            {
                return ConvertPointXYtoChaine(new PointProjection(CoordMouseGrille));
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux </summary>
        private PointD CoordMouseDD
        {
            get
            {
                return PointGrilleToPointDD(CoordMouseGrille, Serveur.Datum);
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux sous forme de chaine de caractères formatée </summary>
        private string CoordMouseDDText
        {
            get
            {
                return ConvertPointDDtoChaine(CoordMouseDD);
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimée en degrés Miniute et secondes sous forme de chaine de caractères formatée </summary>
        private string CoordMouseDMSText
        {
            get
            {
                return ConvertPointDDtoDMS(CoordMouseDD);
            }
        }
        /// <summary> Coordonnées du pointeur de souris exprimée en mètre de la projection UTM WGS84 sous forme de chaine de caractères formatée </summary>
        private string CoordMouseUtmWGS84
        {
            get
            {
                return ConvertPointDDtoUTM(CoordMouseDD);
            }
        }
        /// <summary> gère le décalage du tampon d'affichage en fonction des décalages X et Y exprimés en pixels </summary>
        private void GererDecalageAffichage()
        {
            // il y a besoin d'un décalage si la valeur du décalage tampon n'est plus dans le nombre des pixels d'une tuile
            if (!SurfaceDecalage.Contains(Affichage.DecalageTampon))
            {
                Affichage.DecalerTampon();
                RemplirTampon();
            }
        }
        /// <summary> gère le changement du zoom de l'affichage </summary>
        /// <param name="DeltaIndiceCouche"> Ecart entre les indices de l'échelle actuelle et de l'échelle future </param>
        /// <param name="PositionCoordonnees">Position du curseur de souris exprimé en pixel écran qui restera au même endroit sur le nouvel affichage </param>
        private void GererChangementCouche(int DeltaIndiceCouche, Point PositionCoordonnees)
        {
            Affichage.ChangerCouche(PositionCoordonnees, DeltaIndiceCouche);
            // on affiche la couche précédente avec le zoom de l'actuelle. sert quand le ser
            SurfaceAffichageTuile.Invalidate();
            RemplirTampon();
        }
        /// <summary> gère le changement de la taille de l'affichage </summary>
        private void GererChangementTaille()
        {
            if (!FlagInit)
            {
                // changement de taille du à la touche F11 ou équivalent, on prend comme coordonnées celles du centre de l'affichage
                CapturerSettings.LOCATION_CENTRE = Affichage.CentreAffichage;
            }
            // sinon on affiche les dernières coordonnées du site
            Affichage.ChangerTaille(VisueRectangle.Size, CapturerSettings.CENTRE_AFFICHAGE_SITE, FlagInit);
            RemplirTampon();
        }
        private static Rectangle RegionAffichageDecalage(Size Decalage)
        {
            var Pt0Decalage = Point.Add(Affichage.LocationAffichage, Decalage);
            return new Rectangle(Pt0Decalage, VisueRectangle.Size);
        }
        /// <summary> événement KeyDown du formulaire principal mais spécifique aux touches de déplacement de la Visue </summary>
        /// <param name="e"> information sur la touche de déplacement et de son modifieur éventuel </param>
        private bool TouchesDeplacementCarte(KeyEventArgs e)
        {
            bool TouchesDeplacementCarteRet = false;
            int PasX = 0, PasY = 0;
            // si le nb maximum de tuiles en cours de téléchargement est atteint on ne fait pas le décalage
            if (Serveur.NbTuilesDemandeAffichage < ServeurCarto.MaxRequetesTuiles)
            {
                switch (e.KeyCode)
                {
                    case var case1 when Keys.Left <= case1 && case1 <= Keys.Down: // on déplace l'affichage par pixels
                        {
                            int Pas = PasTouchesPixels[(int)e.Modifiers / (int)Keys.Shift];
                            switch (e.KeyCode)
                            {
                                case Keys.Left:
                                    {
                                        PasX = Pas;
                                        break;
                                    }
                                case Keys.Right:
                                    {
                                        PasX = -Pas;
                                        break;
                                    }
                                case Keys.Up:
                                    {
                                        PasY = Pas; // Keys.Down
                                        break;
                                    }

                                default:
                                    {
                                        PasY = -Pas;
                                        break;
                                    }
                            }
                            TouchesDeplacementCarteRet = true;
                            break;
                        }
                    case var case2 when Keys.PageUp <= case2 && case2 <= Keys.Home: // on déplace l'affichage en fonction des dimensions de la visue
                        {
                            double Pas = PasTouchesAffichage[(int)e.Modifiers / (int)Keys.Shift];
                            switch (e.KeyCode)
                            {
                                case Keys.Home:
                                    {
                                        PasX = (int)Math.Round(Pas * SurfaceAffichageTuile.ClientSize.Width);
                                        break;
                                    }
                                case Keys.End:
                                    {
                                        PasX = -(int)Math.Round(Pas * SurfaceAffichageTuile.ClientSize.Width);
                                        break;
                                    }
                                case Keys.PageUp:
                                    {
                                        PasY = (int)Math.Round(Pas * SurfaceAffichageTuile.ClientSize.Height); // Keys.PageDown
                                        break;
                                    }

                                default:
                                    {
                                        PasY = -(int)Math.Round(Pas * SurfaceAffichageTuile.ClientSize.Height);
                                        break;
                                    }
                            }
                            TouchesDeplacementCarteRet = true;
                            break;
                        }
                }
                if (TouchesDeplacementCarteRet)
                {
                    // limitation du déplacement aux limites du site web
                    if (FlagLimitesSite && !LimitesVirtuellesSite.IntersectsWith(RegionAffichageDecalage(new Size(PasX, PasY))))
                    {
                        MessageInformation = "La surface de l'affichage sera en dehors des limites du site de capture.";
                        AfficherInformation();
                    }
                    else
                    {
                        AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD);
                        Affichage.DecalageTamponX += PasX;
                        Affichage.DecalageTamponY += PasY;
                        GererDecalageAffichage();
                        RepeindreSurfaceAffichage();
                    }
                }
            }
            e.SuppressKeyPress = TouchesDeplacementCarteRet;
            return TouchesDeplacementCarteRet;
        }
        private void SurfaceAffichageTuile_MouseMove(object sender, MouseEventArgs e)
        {
            //si l'évenement mouse leave d'un label liste ou menu n'a pas été généré on le force
            if (LabelEncours is not null)
                LabelListesMenus_MouseLeave(LabelEncours, null);
            // on verifie d'abord que le curseur de souris est toujours sur l'affichage
            if (VisueRectangle.Contains(Point.Add(e.Location, AffichageOffsetEcran)))
            {
                if (Action == Actions.DefinirPoints)
                {
                    PosMouse = e.Location;
                    if (FlagLimitesSite && !LimitesVirtuellesSite.Contains(CoordMousePixels))
                    {
                        MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture.";
                        AfficherInformation();
                        Action = Actions.Aucune;
                    }
                    else
                    {
                        NomPointEncours = "Pt2";
                        DefinirPoint();
                    }
                }
                else if (Action == Actions.DeplacerCarte)
                {
                    int PasX = PosMouse.X - e.Location.X;
                    int PasY = PosMouse.Y - e.Location.Y;
                    // limitation du déplacement aux limites du site web
                    if (FlagLimitesSite && !LimitesVirtuellesSite.IntersectsWith(RegionAffichageDecalage(new Size(PasX, PasY))))
                    {
                        MessageInformation = "La surface de l'affichage sera en dehors des limites du site de capture.";
                        AfficherInformation();
                        Action = Actions.Aucune;
                        TrouverCurseurCarte();
                    }
                    else
                    {
                        Affichage.DecalageTamponX += PasX;
                        Affichage.DecalageTamponY += PasY;
                        PosMouse = e.Location;
                        GererDecalageAffichage();
                    }
                }
                else if (TRK.IsTraceEncoursModeEdition) // Si la trace peut être éditée ou est éditée
                {
                    PosMouse = e.Location;
                    if (TRK.TraceEncours.SourisBouge(CoordMousePixels))
                        SurfaceAffichageTuile.Invalidate();
                }
                else
                {
                    PosMouse = e.Location;
                }
                AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD);
                RepeindreSurfaceAffichage();
            }
        }
        /// <summary> gère le flag qui indique que le bouton gauche de la souris est enfoncé (outil déplacement de la carte avec la souris) </summary>
        private void SurfaceAffichageTuile_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // on ne prend en compte que le bouton gauche de la souris
            {
                PosMouse = e.Location;
                AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD);
                if (ModifierKeys == Keys.Alt) // il s'agit de la définition des PT1 et PT2 avec la souris
                {
                    if (FlagLimitesSite && !LimitesVirtuellesSite.Contains(CoordMousePixels))
                    {
                        MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture.";
                        AfficherInformation();
                    }
                    else
                    {
                        CurseurEncours = Curseurs.FRefDeplacer;
                        Action = Actions.DefinirPoints;
                        NomPointEncours = "Pt1";
                        DefinirPoint();
                    }
                }
                else if (TRK.IsTraceEncoursEditing)
                {
                    // la trace encours est entrain d'être éditée Action DéplacerPoint, CouperPoint ou Segment, SupprimerPoint ou Segment
                    // on avertit la trace qu'il faut commencer l'ActionEncours
                    var Attr = new Attribut() { Altitude = AltitudePosMouse };
                    if (TRK.TraceEncours.SourisDown(CoordMousePixels, Attr))
                        SurfaceAffichageTuile.Invalidate();
                }
                else if (Serveur.NbTuilesDemandeAffichage < ServeurCarto.MaxRequetesTuiles) // action Deplacercarte
                {
                    Action = Actions.DeplacerCarte;
                    TrouverCurseurCarte();
                }
            }
        }
        /// <summary> gère le flag qui indique que le bouton gauche de la souris est relevé (outil déplacement avec la souris) </summary>
        private void SurfaceAffichageTuile_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // on ne prend en compte que le bouton gauche de la souris
            {
                PosMouse = e.Location;
                if (Action == Actions.DefinirPoints)
                {
                    // le bouton gauche a été relevée en 1er on ne peut pas finir l'action directement
                    FinirActionDefinirPoints();
                }
                else if (Action == Actions.FinirDefinirPoints)
                {
                    // la touche menu a été relevé en 1er on peut finir l'action
                    Action = Actions.Aucune;
                    TrouverCurseurCarte();
                }
                else if (TRK.IsTraceEncoursEditing)
                {
                    // la trace encours est entrain d'être éditée Action DéplacerPoint, CouperPoint ou Segment, SupprimerPoint ou Segment
                    // on avertit la trace qu'il faut finir l'ActionEncours
                    var Attr = new Attribut() { Altitude = AltitudePosMouse };
                    if (TRK.TraceEncours.SourisUp(CoordMousePixels, Attr))
                        SurfaceAffichageTuile.Invalidate();
                    CurseurEncours = Curseurs.TraceDefaut;
                }
                else // action Deplacercarte
                {
                    Action = Actions.Aucune;
                    TrouverCurseurCarte();
                }
            }
        }
        /// <summary> logique de gestion de la molette de la souris : changement de couche </summary>
        private void SurfaceAffichageTuile_MouseWheel(object sender, MouseEventArgs e)
        {
            var DeltaIndiceEchelle = default(int);
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
                if (MemoireDeltaSouris == 3)
                {
                    if (CapturerSettings.INDICE_ECHELLE > 0)
                    {
                        PosMouse = e.Location;
                        MemoireDeltaSouris = 0;
                        DeltaIndiceEchelle = -1;
                    }
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
                if (MemoireDeltaSouris == -3)
                {
                    if (CapturerSettings.INDICE_ECHELLE < Serveur.NbEchellesLayer(0))
                    {
                        PosMouse = e.Location;
                        MemoireDeltaSouris = 0;
                        DeltaIndiceEchelle = +1;
                    }
                }
            }
            if (FlagLimitesSite && !LimitesVirtuellesSite.Contains(CoordMousePixels))
            {
                MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture.";
                AfficherInformation();
            }
            else
            {
                FlagPosMouse = true;
                ListeChoixEchelles.SelectedIndex += DeltaIndiceEchelle;
                FlagPosMouse = false;
            }
        }
        // ''' <summary> met à jour les coordonnées du pointeur de souris en cas de click </summary>
        private void SurfaceAffichageTuile_MouseClick(object sender, MouseEventArgs e) // Handles SurfaceAffichageTuile.MouseClick
        {
            PosMouse = e.Location;
            AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD);
        }
        /// <summary> gère le redimensionnement de la surface d'affichage </summary>
        private void SurfaceAffichageTuile_Resize(object sender, EventArgs e)
        {
            // Size.Empty est le cas de la réduction de FCGP_Capturer à une icone sur la barre des tâches, on sort car il n'y a rien à afficher
            if (SurfaceAffichageTuile.ClientRectangle.Size != Size.Empty)
            {
                // sert à déterminer si le curseur de la souris est sur la visue
                VisueRectangle = SurfaceAffichageTuile.RectangleToScreen(SurfaceAffichageTuile.ClientRectangle);
                CentreAffichage = new Point(VisueRectangle.Width / 2, VisueRectangle.Height / 2);
                AffichageOffsetEcran = (Size)VisueRectangle.Location;
                GererChangementTaille();
            }
        }
        #endregion
        #region Actions spécifiques Menu_Application et touches associées
        /// <summary> ouvre le formulaire de configuration à partir du menu du programme </summary>
        private void ConfigureApp_Click(object sender, EventArgs e)
        {
            OuvrirConfiguration();
        }
        /// <summary> lance le test du serveur avec une requête GetCapabilities </summary>
        private void TesteServeur_Click(object sender, EventArgs e)
        {
            Serveur.TesterServeurAsync();
        }
        /// <summary> compacte le cache tuile selectionné </summary>
        private void CompacteCacheTuiles_Click(object sender, EventArgs e)
        {
            MessageInformation = $"Vous avez demandé à compacter le cache du site {ListeChoixSites.SelectedItem}{CrLf}" +
                                 $"Cette action peut prendre beaucoup de temps en sortie de FCGP Capturer{CrLf}Voulez continuer?";
            if (AfficherConfirmation() == DialogResult.OK)
            {
                NomBaseCacheTuile = CapturerSettings.CHEMIN_CACHE + @"\CacheTuiles" + SitesCartoClefs[(int)CapturerSettings.SITE] + ".db";
            }
        }
        /// <summary> ouvre le formulaire de configuration des captures de cartes à partir du menu du programme </summary>
        private void LanceCreationCarte_Click(object sender, EventArgs e)
        {
            OuvrirPreferencesAsync();
        }
        /// <summary> Supprime les tuiles correspondantes à la surface de téléchargement (1 seul niveau de détails) </summary>
        private void SupprimeTuiles_Click(object sender, EventArgs e)
        {
            SupprimerTuiles();
        }
        /// <summary> ouvre le formulaire de saisie de coordonnées en fonction du type de celle-ci </summary>
        private void Va_A_App_Click(object sender, EventArgs e)
        {
            CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(PointEncours, CapturerSettings.SITE, Affichage.Echelle);
            OuvrirSaisieCoordonnees();
        }
        private void Va_A_Site_Click(object sender, EventArgs e)
        {
            AllerAuCentreSite();
        }
        /// <summary> ouvre la boite d'information A Propos à partir du menu du programme </summary>
        private void APropos_Click(object sender, EventArgs e)
        {
            OuvrirAPropos();
        }
        /// <summary> ouvre le formulaire d'aide à partir du menu du programme </summary>
        private void Aide_Click(object sender, EventArgs e)
        {
            AfficherAide(this);
        }
        /// <summary> ouvre le formulaire d'aide à partir du menu du programme </summary>
        private void ImprimerCarte_Click(object sender, EventArgs e)
        {
            OuvrirImprimerCarte();
        }
        private void LimitesSite_Click(object sender, EventArgs e)
        {
            AppliquerLimitesSite();
        }
        /// <summary> quite le programme à partir du menu du programme </summary>
        private void FermeApp_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary> événement KeyDown du formulaire principal mais spécifique au Menu Application </summary>
        private bool TouchesMenuApp(KeyEventArgs e)
        {
            bool TouchesMenuAppRet = true;
            switch (e.KeyCode)
            {
                case Keys.F1:
                    {
                        if (e.Alt)
                        {
                            OuvrirImprimerCarte();
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            AfficherAide(this);
                        }
                        else
                        {
                            TouchesMenuAppRet = false;
                        }

                        break;
                    }
                case Keys.F2: // on ouvre  Préférences si il y a une capture encours ou si les conditions sont réunies si il n'y a pas de capture encours
                    {
                        if (e.Alt)
                        {
                            if (SurfaceTraitementIsOk)
                                SupprimerTuiles();
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            if (SurfaceTraitementIsOk && Serveur.NbDemandesAffichage == 0 && IsConnected || FlagCapture)
                                OuvrirPreferencesAsync();
                        }
                        else
                        {
                            TouchesMenuAppRet = false;
                        }

                        break;
                    }
                case Keys.F3:
                    {
                        if (e.Alt)
                        {
                            Serveur.TesterServeurAsync();
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            if (Serveur.NbDemandesAffichage == 0 && !FlagCapture)
                                OuvrirConfiguration();
                        }
                        else
                        {
                            TouchesMenuAppRet = false;
                        }

                        break;
                    }
                case Keys.F4:
                    {
                        if (e.Alt)
                        {
                            Close();
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            OuvrirAPropos();
                        }
                        else
                        {
                            TouchesMenuAppRet = false;
                        }

                        break;
                    }
                case Keys.F5:
                    {
                        if (e.Modifiers == Keys.None)
                        {
                            if (FlagRevenir)
                            {
                                RevenirDe();
                            }
                        }
                        else
                        {
                            TouchesMenuAppRet = false;
                        }

                        break;
                    }
                case Keys.F6:
                    {
                        if (e.Alt)
                        {
                            AllerAuCentreSite();
                        }
                        else
                        {
                            TouchesMenuAppRet = false;
                        }

                        break;
                    }

                default:
                    {
                        TouchesMenuAppRet = false;
                        break;
                    }
            }
            e.SuppressKeyPress = TouchesMenuAppRet;
            return TouchesMenuAppRet;
        }
        #endregion
        #region Actions spécifiques MenuPoints et touches associées
        /// <summary> définit le PointEncours avec les coordonnées du centre de la visue </summary>
        private void DefinitCentrePts_Click(object sender, EventArgs e)
        {
            DefinirPointCentre();
        }
        /// <summary> efface le pointencours </summary>
        private void EffacePts_Click(object sender, EventArgs e)
        {
            EffacerPoint();
        }
        /// <summary> Place l </summary>
        private void Va_A_Pts_Click(object sender, EventArgs e)
        {
            AllerAuPointCapture();
        }
        /// <summary> événement KeyDown du formulaire principal mais spécifique au Menu Points </summary>
        private bool TouchesMenuPoints(KeyEventArgs e)
        {
            bool TouchesMenuPointsRet = true; // par defaut on dit que la touche est traitée
            switch (e.KeyCode)
            {
                case Keys.F6:
                    {
                        if (!CapturerSettings.POINT1.IsEmpty && e.Shift)
                        {
                            NomPointEncours = "Pt1";
                            AllerAuPointCapture();
                        }
                        else if (!CapturerSettings.POINT2.IsEmpty && e.Control)
                        {
                            NomPointEncours = "Pt2";
                            AllerAuPointCapture();
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            OuvrirSaisieCoordonnees();
                        }
                        else
                        {
                            TouchesMenuPointsRet = false;
                        }

                        break;
                    }
                case Keys.F7:
                    {
                        if (e.Shift || e.Control)
                        {
                            if (!CapturerSettings.POINT1.IsEmpty & !CapturerSettings.POINT2.IsEmpty)
                            {
                                OuvrirSurfaceTraitement(SurfaceTraitement);
                            }
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            OuvrirSurfaceTraitement(new SurfaceTuiles());
                        }
                        else
                        {
                            TouchesMenuPointsRet = false;
                        }

                        break;
                    }
                case Keys.F8:
                    {
                        if (e.Shift)
                        {
                            NomPointEncours = "Pt1";
                            DefinirPointCentre();
                        }
                        else if (e.Control)
                        {
                            NomPointEncours = "Pt2";
                            DefinirPointCentre();
                        }
                        else
                        {
                            TouchesMenuPointsRet = false;
                        }

                        break;
                    }
                case Keys.F9:
                    {
                        if (e.Shift)
                        {
                            NomPointEncours = "Pt1";
                            EffacerPoint();
                        }
                        else if (e.Control)
                        {
                            NomPointEncours = "Pt2";
                            EffacerPoint();
                        }
                        else
                        {
                            TouchesMenuPointsRet = false;
                        }

                        break;
                    }

                default:
                    {
                        TouchesMenuPointsRet = false;
                        break;
                    }
            }
            e.SuppressKeyPress = TouchesMenuPointsRet;
            return TouchesMenuPointsRet;
        }
        #endregion
        #region Actions spécifiques Menu_Trace et touches associées
        /// <summary> invers le mode d'édition de la trace encours </summary>
        private void EditeTrace_Click(object sender, EventArgs e)
        {
            InverserEditionTrace();
        }
        /// <summary> ouvre le formulaire de fermeture de la trace encours </summary>
        private void FermeTrace_Click(object sender, EventArgs e)
        {
            FermerTraceAffichage();
        }
        /// <summary> ouvre le formulaire des propriétés de la trace encours </summary>
        private void ProprietesTrace_Click(object sender, EventArgs e)
        {
            ProprietesTraceAffichage();
        }
        /// <summary> positionne début de la trace encours au centre de l'affichage </summary>
        private void Va_A_DebutTrace_Click(object sender, EventArgs e)
        {
            AllerAuPointTrace(true);
        }
        /// <summary> positionne la fin de la trace encours au centre de l'affichage </summary>
        private void Va_A_FinTrace_Click(object sender, EventArgs e)
        {
            AllerAuPointTrace(false);
        }
        /// <summary> ouvre le formulaire de d'ouverture de la trace encours </summary>
        private void OuvreTrace_Click(object sender, EventArgs e)
        {
            OuvrirTraceAffichage();
        }
        /// <summary> événement KeyDown du formulaire principale mais spécifique au Menu Trace </summary>
        /// <param name="ToucheMenuTrace"> Code de la touche à transmettre à la trace </param>
        /// <returns> True si l'évenement est pris en compte </returns>
        private bool TouchesMenuTrace(KeyEventArgs e)
        {
            bool TouchesMenuTraceRet = true;
            if (TRK.IsTraceEncours && e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.E: // edition trace on/off
                        {
                            InverserEditionTrace();
                            break;
                        }
                    case Keys.P:
                        {
                            ProprietesTraceAffichage();
                            break;
                        }
                    case Keys.F:
                        {
                            FermerTraceAffichage();
                            break;
                        }
                    case Keys.D:
                        {
                            AllerAuPointTrace(true);
                            break;
                        }
                    case Keys.A:
                        {
                            AllerAuPointTrace(false);
                            break;
                        }

                    default:
                        {
                            TouchesMenuTraceRet = false;
                            break;
                        }
                }
            }
            else if (e.KeyCode == Keys.F10)
            {
                OuvrirTraceAffichage();
            }
            else if (TRK.IsTraceEncoursModeEdition && TRK.TraceEncours.ToucheDown(e.KeyCode))
            {
                // si la trace peut être éditée, on transmet l'événement pour qu'elle puisse traiter les touches A,S,C
                SurfaceAffichageTuile.Invalidate();
            }
            else
            {
                TouchesMenuTraceRet = false;
            }
            e.SuppressKeyPress = TouchesMenuTraceRet;
            return TouchesMenuTraceRet;
        }
        #endregion
        #region Actions spécifiques Context Menu_Visue et touches associées
        /// <summary> Définit le point qui est cliqué à la position de la souris </summary>
        private void DefinitPts_Click(object sender, EventArgs e)
        {
            NomPointEncours = ((ToolStripMenuItem)sender).Tag.ToString();
            DefinirPoint();
        }
        /// <summary> permet ou non d'afficher une échelle dans le coin bas droit de l'écran pour certains niveaux </summary>
        private void AfficheDessinEchelle_Click(object sender, EventArgs e)
        {
            AfficherDessinEchelle();
        }
        /// <summary> permet ou non d'afficher la couche des pentes pour certains sites et certains niveaux </summary>
        private void AfficheCouchePentes_Click(object sender, EventArgs e)
        {
            AfficherPentes();
        }
        /// <summary> permet de choisir le coefficient de transparence de la couche des Pentes </summary>
        private void ChoisitCoefAlphaPentes_SelectedIndexChanged(object sender, EventArgs e) // Async
        {
            if (CapturerSettings.INDICE_COEF_ALPHA_PENTES != ChoisitCoefAlphaPentes.SelectedIndex)
            {
                // on force la fermeture du tooltip
                ChoisitCoefAlphaPentes.Visible = false;
                ContextMenuVisue.Hide();
                ChoisitCoefAlphaPentes.Visible = true;
                CapturerSettings.INDICE_COEF_ALPHA_PENTES = ChoisitCoefAlphaPentes.SelectedIndex;
                float CoefAlphaPentes = CoefsAlphasPentes[ChoisitCoefAlphaPentes.SelectedIndex];
                ImageAttributsPentes.SetColorMatrix(new ColorMatrix() { Matrix33 = CoefAlphaPentes }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                RemplirTampon();
            }
        }
        /// <summary> Change le site actuel pour le site à portée européenne indiqué mais avec la couche la plus approchante et la même position </summary>
        private void ChoisitSitesOSM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChoisitSitesOSM.Visible == true)
            {
                // on force la fermeture du tooltip
                ChoisitSitesOSM.Visible = false;
                ContextMenuVisue.Hide();
                ChoisitSitesOSM.Visible = true;
                SiteAllerA = SitesOSMEurope[ChoisitSitesOSM.SelectedIndex];
                if (DatumSiteWeb((int)CapturerSettings.SITE) != Datums.Web_Mercator)
                {
                    // les sites Europeen sont forcément des sites WebMercator donc si le site de départ est SM il faut passer par une transformation de coordonnées
                    int IndiceEchelle = CapturerSettings.INDICE_ECHELLE;
                    var Echelle = NiveauDetailCartographique.SiteIndiceEchelleToEchelle(SiteAllerA, IndiceEchelle);
                    var Point_WebMercator = PointDDToPointPixels(CoordMouseDD, SiteAllerA, Echelle);
                    PositionSiteAllerA = new PointG(IndiceEchelle, Point_WebMercator);
                }
                else
                {
                    PositionSiteAllerA = CoordMousePointG;
                }
                CapturerSettings.INDICE_OSM = ChoisitSitesOSM.SelectedIndex;
                // on demande le changement du site actuel par le site OSM
                ListeChoixSites.SelectedIndex = SiteDomTomToIndex(SiteAllerA, DomsToms.aucun);
            }
        }
        /// <summary> Change le fond de plan d'affichage du site actuel </summary>
        private void ChoisitFondPlan_SelectedIndexChanged(object sender, EventArgs e) // Async
        {
            if (CapturerSettings.INDICE_FOND_PLAN != ChoisitFondsPlan.SelectedIndex)
            {
                ContextMenuVisue.Hide();
                CapturerSettings.INDICE_FOND_PLAN = ChoisitFondsPlan.SelectedIndex;
                Serveur.IndiceFondPlan = ChoisitFondsPlan.SelectedIndex;
                RemplirTampon();
            }
        }
        /// <summary> événement KeyDown du formulaire principal mais spécifique au context Menu Visue </summary>
        private bool TouchesMenuVisue(KeyEventArgs e)
        {
            bool TouchesMenuVisueRet = true;
            switch (e.KeyCode)
            {
                case Keys.F12:
                    {
                        if (ModifierKeys == Keys.Alt)
                        {
                            AfficherPentes();
                        }
                        else if (e.Modifiers == Keys.None)
                        {
                            AfficherDessinEchelle();
                        }
                        else
                        {
                            TouchesMenuVisueRet = false;
                        }

                        break;
                    }

                default:
                    {
                        TouchesMenuVisueRet = false;
                        break;
                    }
            }
            e.SuppressKeyPress = TouchesMenuVisueRet;
            return TouchesMenuVisueRet;
        }
        #endregion
        #region Actions Communes à plusieurs menus ou liste de choix ou touches communes à plusieurs Menus
        // la gestion des évènements dans windowsforms est sujette à caution. Le système ne réagit pas très vite.
        // Cela concerne surtout les 1er contrôles ajoutés dans la collection de contrôles du formulaire.
        // De ce fait on est obligé de vérifier que les évenements prévus ont bien été émis et réalisés ce qui complexifie un peu le code
        /// <summary> ouvre le formulaire de saisie d'une surface de capture </summary>
        private void SaisieSurfacesTraitements_Click(object sender, EventArgs e)
        {
            string NomMenu = ((ToolStripMenuItem)sender).Tag.ToString();
            var Surface = NomMenu == "App" ? new SurfaceTuiles() : SurfaceTraitement;
            OuvrirSurfaceTraitement(Surface);
        }
        /// <summary> lance l'action de revenir au dernier point de sauvegarde </summary>
        private void RevientDe_Click(object sender, EventArgs e)
        {
            RevenirDe();
        }
        /// <summary> vérifie que l'apparence des Labels avec liste ou menu soit non survolé</summary>
        /// <remarks> A l'entrée Labelencours doit être null </remarks>
        private void Information_MouseMove(object sender, EventArgs e)
        {
            // si l'évenement mouse leave d'un label liste ou menu n'a pas été généré on le force
            if (LabelEncours is not null)
                LabelListesMenus_MouseLeave(LabelEncours, null);
        }
        /// <summary> vérifie que l'apparence de sender soit survolé et non survolée pour les autres</summary>
        /// <remarks> à l'entrée labelencours doit être sender</remarks>
        private void LabelListesMenus_MouseMove(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            if (L != LabelEncours)
            {
                LabelListesMenus_MouseEnter(L, null);
            }
        }
        ///<summary> passe l'apparence de sender à survolée et non survolée pour les autres </summary>
        /// <remarks> A l'entrée Labelencours doit être null </remarks>
        private void LabelListesMenus_MouseEnter(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            // l'évenement mouse leave de l'ancien labelencours n'a pas été généré on le force
            if (LabelEncours is not null)
                LabelListesMenus_MouseLeave(LabelEncours, null);
            //mettre à jour l'apparence du label comme étant survolé
            L.BackColor = Color.FromArgb(255, 229, 241, 251);
            L.BorderStyle = BorderStyle.FixedSingle;
            L.Refresh();
            //on indique le labelencours
            LabelEncours = L;
        }
        /// <summary> passe l'apparence de sender à non survolée </summary>
        private void LabelListesMenus_MouseLeave(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            //mettre à jour l'apparence du label comme étant non survolé
            L.BackColor = Color.Transparent;
            L.BorderStyle = BorderStyle.None;
            L.Refresh();
            //on indique qu'il n'y a plus de labelencours
            LabelEncours = null;
            if (L == LabelMenuPt1 || L == LabelMenuPt2)
                GererIUMenuPts();
            if (L == LabelTrace)
                GererIUMenuTrace();
        }
        /// <summary> ouvre le menu associé à un LabelMenu </summary>
        private void LabelMenus_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            if (!FlagContextMenuShow)
            {
                if (L == LabelMenuPt1 || L == LabelMenuPt2)
                {
                    NomPointEncours = L.Tag.ToString();
                    GererMenuPts();
                }
                var R2 = L.RectangleToScreen(L.ClientRectangle);
                FlagContextMenuShow = true;
                L.ContextMenuStrip.Show();
                var s = L.ContextMenuStrip.Size;
                if (L == LabelMenuApp || L == LabelMenuPt1) // à gauche du label
                {
                    L.ContextMenuStrip.SetBounds(R2.Right - s.Width, R2.Top - s.Height, s.Width, s.Height);
                }
                else // à droite du label
                {
                    L.ContextMenuStrip.SetBounds(R2.Left, R2.Top - s.Height, s.Width, s.Height);
                }
            }
            else
            {
                L.ContextMenuStrip.Close();
                FlagContextMenuShow = false;
            }
        }
        /// <summary> ouvre la liste de choix associée à un LabelListe </summary>
        private void LabelListes_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            if (L == LabelSites && Serveur.NbDemandesAffichage > 0)
                return;
            if (L == LabelEchelles)
            {
                // si le centre de l'affichage est en dehors des limites du site on sort
                if (FlagLimitesSite && !LimitesVirtuellesSite.Contains(Affichage.CentreAffichage))
                {
                    MessageInformation = "Le centre de l'affichage est en dehors des limites du site de capture.";
                    AfficherInformation();
                    return;
                }
            }
            //trouve la combobox associée au label
            ComboxDropDown = (ComboBox)L.Tag;
            //ouvre la liste de choix de la combobox associée au label
            ComboxDropDown.DroppedDown = true;
            // rend visible la combobox associée au label
            ComboxDropDown.Show();
        }
        /// <summary> ferme la liste déroulante des choix de type sites, echelles ou coordonnées </summary>
        private void ListesChoix_DropDownClosed(object sender, EventArgs e)
        {
            //rendre la combobox support invisible
            ComboxDropDown.Hide();
            //supprimer la relation avec la combobox support
            ComboxDropDown = null;
        }
        /// <summary> autorise ou non l'ouverture du menu trace</summary>
        private void ContextMenus_Opening(object sender, CancelEventArgs e)
        {
            if (!FlagContextMenuShow)
                e.Cancel = true;
        }
        /// <summary> gère la fermeture des menus Points, trace et application </summary>
        private void ContextMenus_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ContextMenuStrip C = (ContextMenuStrip)sender;
            Label T = (Label)C.Tag;
            // si un click en dehors du menu ou la touche escape ou une action du menu on indique que le menu est fermé
            if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard || e.CloseReason == ToolStripDropDownCloseReason.ItemClicked || T.BackColor != Color.FromArgb(255, 229, 241, 251))
            {
                FlagContextMenuShow = false;
            }
        }
        #endregion
        #region Actions communes à des clicks sur menu et des touches de raccourci
        /// <summary> dimensionne la taille de la visue à 80% environ de l'écran </summary>
        private void DimensionnerAffichage()
        {
            // si c'est le premier passage on dimensionne la visue et l'affichage à partir du déclemenchement de l'évenement resize
            var R = Rectangle.Inflate(DimensionsEcranSupport, (int)Math.Round(DimensionsEcranSupport.Width * -0.1d), (int)Math.Round(DimensionsEcranSupport.Height * -0.1d));
            Location = R.Location;
            ClientSize = R.Size;
        }
        /// <summary> inverse le flag qui permet l'affichage ou non d'une echelle dans le coin bas droit de la visue </summary>
        private void AfficherDessinEchelle()
        {
            CapturerSettings.IS_AFFICHE_ECHELLE = !CapturerSettings.IS_AFFICHE_ECHELLE;
            AfficheDessinEchelle.Text = ChangerTextBascule(false);
            SurfaceAffichageTuile.Invalidate();
        }
        /// <summary> inverse le flag qui permet l'affichage ou non de la couche des pentes par dessus la couche de fond de plan </summary>
        private void AfficherPentes()
        {
            CapturerSettings.IS_AFFICHE_PENTES = !CapturerSettings.IS_AFFICHE_PENTES;
            AfficheCouchePentes.Text = ChangerTextBascule(true);
            Affichage.CalculerTamponPentes();
            RemplirTampon();
        }
        /// <summary> Calcul le texte du context menueVisue. il s'agit d'une bascule On / OFf </summary>
        /// <param name="FlagPentes"> Oui si le menu concerne les pentes, Non si le menu concerne le dessin de l'échelle </param>
        private static string ChangerTextBascule(bool FlagPentes)
        {
            string Ret;
            if (FlagPentes)
            {
                Ret = $"Afficher Pentes  : {(CapturerSettings.IS_AFFICHE_PENTES ? "Off" : "On")}";
            }
            else
            {
                Ret = $"Afficher Echelle  : {(CapturerSettings.IS_AFFICHE_ECHELLE ? "Off" : "On")}";
            }
            return Ret;
        }
        /// <summary> supprime toutes les tuiles qui font partie de la surface de téléchargement. Les tuiles Pentes ne sont pas concernées </summary>
        private void SupprimerTuiles()
        {
            MessageInformation = $"Effacer les tuiles sélectionnées ?" + CrLf;
            if (AfficherConfirmation() == DialogResult.OK)
            {
                // suppression des tuiles définies par la surface de capture
                Serveur.Cache.EffacerSurfaceTuile(SurfaceTraitement);
                CapturerSettings.POINT1 = PointD.Empty;
                CapturerSettings.POINT2 = PointD.Empty;
                GererIUMenuPts();
                // on télécharge les tuiles éffacées et qui doivent être affichées
                RemplirTampon();
            }
        }
        /// <summary> ouvre le formulaire de saisie de coordonnées adequat en fonction du type d'unité </summary>
        private void OuvrirSaisieCoordonnees()
        {
            var Coordonnees = new PointD();
            if (OuvrirFormulaireSaisieCoordonnees(this, Point.Empty, ListeChoixTypeCoordonnees.SelectedIndex, ref Coordonnees) == DialogResult.OK)
            {
                GererMenuRevenir(true);
                CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(Coordonnees, CapturerSettings.SITE, Affichage.Echelle);
                AllerA();
            }
        }
        /// <summary> ouvre la boite d'information A Propos </summary>
        private void OuvrirAPropos()
        {
            var S = new StringBuilder();
            S.AppendLine(NumFCGP);
            S.AppendLine("en date du " + DateVersionFCGP);
            S.AppendLine("Faire des Cartes à partir de GéoPortail (Capture)");
            S.AppendLine("Contact : " + Contact);
            S.AppendLine("Dimensions Ecran : " + DimensionsEcranSupport.Width + "*" + DimensionsEcranSupport.Height);
            S.AppendLine("Nom Ecran : " + Screen.AllScreens[PartagerSettings.NUM_ECRAN].DeviceName);
            S.AppendLine("Site de capture : " + SitesCartoLibelles[(int)CapturerSettings.SITE]);
            S.AppendLine("Memoire dispo : " + (NativeMethods.GetTotalMemory / Math.Pow(1024d, 3d)).ToString("0.00") + " Go, Nb Processeurs" + Environment.ProcessorCount);
            S.AppendLine("Support capture : " + SupportsCarteNom[(int)CartesSettings.SUPPORT_CARTE]);
            MessageInformation = S.ToString();
            AfficherInformationEtLien(this, "Visitez le site FCGP", "https://fcgp.e-monsite.com");
        }
        /// <summary> ouvre le formulaire de configuration </summary>
        private void OuvrirConfiguration()
        {
            int NumEcran = PartagerSettings.NUM_ECRAN;
            var CouleurFond = PartagerSettings.COULEUR_VISUE;
            var SupportCarte = CartesSettings.SUPPORT_CARTE;
            string RepAncien = TracesSettings.CHEMIN_TRACE;
            // on demande à l'utilisateur le site sur lequel il va travailler et sur quel écran
            using (Form R = new Configuration())
            {
                R.ShowDialog(this);
            }
            if (NumEcran != PartagerSettings.NUM_ECRAN)
            {
                bool FlagMaximize = WindowState == FormWindowState.Maximized;
                if (FlagMaximize)
                    WindowState = FormWindowState.Normal;
                DimensionsEcranSupport = Screen.AllScreens[PartagerSettings.NUM_ECRAN].Bounds;
                DimensionnerAffichage();
                if (FlagMaximize)
                    WindowState = FormWindowState.Maximized;
            }
            if ((TracesSettings.CHEMIN_TRACE ?? "") != (RepAncien ?? ""))
            {
                TRK.InitialiserTraces();
                GererMenuTrace();
            }
            if (SupportCarte != CartesSettings.SUPPORT_CARTE)
            {
                // InitialiserTelechargement()
                GererIUMenuPts();
            }
            if (CouleurFond != PartagerSettings.COULEUR_VISUE)
                Affichage.CouleurFond = PartagerSettings.COULEUR_VISUE;
        }
        /// <summary> ouvre le formulaire de configuration des captures de cartes </summary>
        private async void OuvrirPreferencesAsync()
        {
            if (!FlagCapture)
            {
                using (var R = new Preferences() { SurfaceTraitement = SurfaceTraitement })
                {
                    R.Tag = false;
                    if (TRK.IsTraceEncours)
                    {
                        // pour mettre à jour l'emprise de la trace et le fichier qui peut servir pour le dessin de la carte
                        TRK.EnregistrerTraceEncours();
                        R.Tag = SurfaceTraitement.RectanglePixels.IntersectsWith(TRK.TraceEncours.BoundingBoxVirtuel);
                    }
                    if (R.ShowDialog(this) == DialogResult.OK)
                    {
                        FlagCapture = true;
                        LanceCreationCarte.Text = "Annuler Création ...";
                        ConfigureApp.Enabled = false;
                        LabelInformation.BackColor = Color.SeaShell;
                        string NomCarte = R.Tag.ToString();
                        await TelechargementCarte.RealiserCarteAsync(NomCarte, SurfaceTraitement, LabelInformation);
                        // on remet tout en ordre pour la prochaine capture
                        FlagCapture = false;
                        LanceCreationCarte.Text = "Lancer Création ...";
                        ConfigureApp.Enabled = true;
                    }
                }
            }
            else
            {
                MessageInformation = "Voulez vous annuler le téléchargement des tuiles?";
                if (AfficherConfirmation() == DialogResult.OK)
                {
                    TelechargementCarte.FlagAnnulerTelechargement = true;
                }
            }
        }
        /// <summary> ouvre le formulaire de configuration des captures de cartes </summary>
        /// <param name="SurfaceTraitement"> surfacetuiles représentée par 2 points exprimés en mètre de la grille du site d</param>
        private void OuvrirSurfaceTraitement(SurfaceTuiles SurfaceTraitement)
        {
            using (Form R = new SaisieSurfaceTraitement())
            {
                R.Tag = (SurfaceTraitement, ListeChoixTypeCoordonnees.SelectedIndex);
                if (R.ShowDialog(this) == DialogResult.OK)
                {
                    SurfaceTuiles Retour = (SurfaceTuiles)R.Tag;
                    CapturerSettings.POINT1 = Retour.PtGrille_NO;
                    CapturerSettings.POINT2 = Retour.PtGrille_SE;
                    GererIUMenuPts();
                }
            }
        }
        /// <summary> Lance l'ouverture du formulaire qui permet l'impression d'une carte </summary>
        private static void OuvrirImprimerCarte()
        {
            if (ImprimerCarte.IsPrinting)
            {
                MessageInformation = "Veuillez attendre la fin" + CrLf +
                                     "de l'impression en cours";
                AfficherInformation();
            }
            else
            {
                ImprimerCarte.LancerImpressionCarteAsync();
            }
        }
        /// <summary> ouvre la trace qui sera éditée sur l'affichage donc la trace encours </summary>
        private void OuvrirTraceAffichage()
        {
            if (TRK.OuvrirTraceEncours(AltitudesSettings.IS_ALTITUDE, false))
            {
                AllerAuPointTrace(true);
                // on indique le nom de la trace en cours
                TRK.TraceEncours.ModeEdition = true;
                GererMenuTrace();
            }
        }
        /// <summary> ferme la trace qui est éditée sur l'affichage donc la trace encours </summary>
        private void FermerTraceAffichage()
        {
            if (TRK.IsTraceEncours)
            {
                if (TRK.FermerTraceEncours(true))
                {
                    GererMenuTrace();
                }
            }
        }
        /// <summary> ouvre le formulaire de propriété des traces </summary>
        private void ProprietesTraceAffichage()
        {
            int Index = -1;
            bool Repeindre = TRK.ChangerProprietesTraceEncours(ref Index);
            if (Index > -1)
            {
                // un point précis de la trace a été selectionné
                CapturerSettings.LOCATION_CENTRE = TRK.TraceEncours.CoordonneesVirtuelles(Index);
                AllerA();
            }
            else if (Repeindre)
            {
                SurfaceAffichageTuile.Invalidate(); // changement de couleur
            }
            LabelTrace.Text = TRK.TraceEncours.Nom; // changement de nom
        }
        /// <summary> positionne le centre de l'affichage au coordonnées définit par CapturerSettings.LOCATION_CENTRE </summary>
        private void AllerA()
        {
            FlagInit = true;
            GererChangementTaille();
            // déplace le cuseur au centre de l'affichage
            DeplacerCurseurCentreVisue();
            FlagInit = false;
        }
        /// <summary> positionne le centre de l'affichage en fonction d'un des points de capture </summary>
        private void AllerAuPointCapture()
        {
            GererMenuRevenir(true);
            CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(PointEncours, CapturerSettings.SITE, Affichage.Echelle);
            AllerA();
        }
        /// <summary> positionne le centre de l'affichage en fonction du centre du site de capture </summary>
        private void AllerAuCentreSite()
        {
            GererMenuRevenir(true);
            var EchelleCentre = NiveauDetailCartographique.SiteIndiceEchelleToEchelle(Serveur.SiteCarto, Serveur.Centre.IndiceEchelle);
            var SiteCentreGrille = PointPixelToPointGrille(Serveur.Centre.Location, Serveur.SiteCarto, EchelleCentre);
            CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(SiteCentreGrille, Serveur.SiteCarto, Affichage.Echelle);
            AllerA();
        }
        /// <summary> positionne le centre de l'affichage sur le point de début ou de fin de la trace </summary>
        /// <param name="Debut"> Point de début si true, point de fin si false </param>
        private void AllerAuPointTrace(bool Debut)
        {
            // si la trace a au moins un point on peut le positionner au centre de l'affichage
            if (TRK.TraceEncours.NbPoints > 0)
            {
                int Cpt = Debut ? 0 : TRK.TraceEncours.NbPoints - 1;
                GererMenuRevenir(true);
                CapturerSettings.LOCATION_CENTRE = TRK.TraceEncours.CoordonneesVirtuelles(Cpt);
                AllerA();
            }
        }
        /// <summary> positionne le centre de l'affichage en fonction d'un des points de la surface de traitement </summary>
        /// <param name="Pt">N° du point de 0 à 3</param>
        private void AllerAuPointTraitementSurface(int Pt)
        {
            if (!SurfaceTraitement.IsEmpty)
            {
                GererMenuRevenir(true);
                CapturerSettings.LOCATION_CENTRE = SurfaceTraitement.PtPixels(Pt);
                AllerA();
            }
        }
        /// <summary> positionne le centre de l'affichage tel qu'il était avant une action AllerA </summary>
        private void RevenirDe()
        {
            GererMenuRevenir(false);
            CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(CoordonneesRevenir, CapturerSettings.SITE, Affichage.Echelle);
            AllerA();
        }
        /// <summary> bascule permettant de mettre le mode Edition de la trace On/Off </summary>
        private void InverserEditionTrace()
        {
            if (TRK.IsTraceEncoursModeEdition)
            {
                TRK.TraceEncours.ModeEdition = false;
                CurseurEncours = Curseurs.CarteDefaut;
            }
            else
            {
                TRK.TraceEncours.ModeEdition = true;
                CurseurEncours = Curseurs.TraceDefaut;
            }
            // on indique qu'on active ou désactive le mode trace sur l'interface
            GererIUMenuTrace();
        }
        /// <summary> autorise ou interdit les menus Revenir </summary>
        private void GererMenuRevenir(bool FlagAutoriser)
        {
            if (FlagAutoriser)
            {
                CoordonneesRevenir = PointPixelToPointGrille(Affichage.CentreAffichage, CapturerSettings.SITE, Affichage.Echelle);
            }
            RevientDe_MenuTrace.Enabled = FlagAutoriser;
            RevientDe_Pts.Enabled = FlagAutoriser;
            RevientDe_App.Enabled = FlagAutoriser;
            FlagRevenir = FlagAutoriser;
        }
        /// <summary> gère l'apparence des MenuPts </summary>
        private void GererIUMenuPts()
        {
            LabelMenuPt1.BorderStyle = BorderStyle.None;
            LabelMenuPt2.BorderStyle = BorderStyle.None;
            if (CapturerSettings.POINT1.IsEmpty)
            {
                LabelMenuPt1.BackColor = Color.Transparent;
            }
            else
            {
                LabelMenuPt1.BackColor = Color.Orange;
            }
            if (CapturerSettings.POINT2.IsEmpty)
            {
                LabelMenuPt2.BackColor = Color.Transparent;
            }
            else
            {
                LabelMenuPt2.BackColor = Color.Orange;
            }
            CalculerSurfaceTraitement();
            if (!CapturerSettings.POINT1.IsEmpty && !CapturerSettings.POINT2.IsEmpty)
            {
                if (!SurfaceTraitementIsOk)
                {
                    LabelMenuPt1.BackColor = Color.Tomato;
                    LabelMenuPt2.BackColor = Color.Tomato;
                }
                else
                {
                    LabelMenuPt1.BackColor = Color.YellowGreen;
                    LabelMenuPt2.BackColor = Color.YellowGreen;
                }
            }
            GererMenuLancerCreation();
            LabelMenuPt1.Refresh();
            LabelMenuPt2.Refresh();
            SurfaceAffichageTuile.Invalidate();
        }
        /// <summary> gére le menu associé à un point </summary>
        private void GererMenuPts()
        {
            SaisieSurfaceTraitementPts.Enabled = !SurfaceTraitement.IsEmpty;
            var Modifier = NomPointEncours == "Pt1" ? Keys.Shift : Keys.Control;
            bool IsEmpty = NomPointEncours == "Pt1" ? CapturerSettings.POINT1.IsEmpty : CapturerSettings.POINT2.IsEmpty;
            SaisieSurfaceTraitementPts.ShortcutKeys = Modifier | Keys.F7;
            DefinitCentrePts.Text = $"Définir {NomPointEncours} au centre";
            DefinitCentrePts.ShortcutKeys = Modifier | Keys.F8;
            EffacePts.Enabled = !IsEmpty;
            EffacePts.Text = $"Effacer {NomPointEncours}";
            EffacePts.ShortcutKeys = Modifier | Keys.F9;
            Va_A_Pts.Enabled = !IsEmpty;
            Va_A_Pts.Text = $"Aller à {NomPointEncours}";
            Va_A_Pts.ShortcutKeys = Modifier | Keys.F6;
            ContextMenuPts.Tag = NomPointEncours == "Pt1" ? LabelMenuPt1 : LabelMenuPt2;
        }
        /// <summary> Gère l'apparence du Menu Trace </summary>
        private void GererIUMenuTrace()
        {
            if (TRK.IsTraceEncoursModeEdition)
            {
                CurseurEncours = Curseurs.TraceDefaut;
                LabelTrace.BackColor = Color.Orange;
                EditeTrace.Text = "&Edition Trace Off";
            }
            else if (TRK.IsTraceEncours)
            {
                CurseurEncours = Curseurs.CarteDefaut;
                LabelTrace.BackColor = Color.YellowGreen;
                EditeTrace.Text = "&Edition Trace On";
            }
            else
            {
                CurseurEncours = Curseurs.CarteDefaut;
                LabelTrace.BackColor = Color.Transparent;
                EditeTrace.Text = "&Edition Trace On";
            }
        }
        /// <summary> gère le menu trace </summary>
        private void GererMenuTrace()
        {
            bool FlagTrace = TRK.IsTraceEncours;
            LabelTrace.Text = FlagTrace ? TRK.TraceEncours.Nom : "Pas de trace";
            FermeTrace.Enabled = FlagTrace;
            Va_A_DebutTrace.Enabled = FlagTrace;
            Va_A_FinTrace.Enabled = FlagTrace;
            ProprietesTrace.Enabled = FlagTrace;
            EditeTrace.Enabled = FlagTrace;
            GererIUMenuTrace();
            SurfaceAffichageTuile.Invalidate();
        }
        /// <summary> gère l'accessibilité au menu lancer </summary>
        private void GererMenuLancerCreation()
        {
            if (FlagCapture)
            {
                // on autorise toujours l'annulation d'une carte
                LanceCreationCarte.Enabled = true;
            }
            else
            {
                LanceCreationCarte.Enabled = SurfaceTraitementIsOk && Serveur.NbDemandesAffichage == 0 && IsConnected;
            }
            SupprimeTuiles.Enabled = SurfaceTraitementIsOk;
        }
        /// <summary> Finit réellement l'action définirPoint au relevé soit de la touche Menu soit du bouton gauche de la souris mais indique que 
        /// l'action n'est pas totalement terminer tant que l'autre n'est pas relevé afin d'interdir les événements Down (clavier et souris) </summary>
        private void FinirActionDefinirPoints()
        {
            // action qui ne sert qu'à attendre le relèvement de la touche Alt ou du bouton gauche de la souris
            Action = Actions.FinirDefinirPoints;
            TrouverCurseurCarte();
        }
        /// <summary> trouve le curseur par défaut de la carte soit celui de la carte soit celui de la trace si il y en a une en mode édition </summary>
        private void TrouverCurseurCarte()
        {
            // determine le curseur en fonction de la trace
            if (TRK.IsTraceEncoursModeEdition)
            {
                if (Action == Actions.DeplacerCarte)
                {
                    CurseurEncours = Curseurs.TraceDeplacement;
                }
                else
                {
                    CurseurEncours = Curseurs.TraceDefaut;
                }
            }
            else if (Action == Actions.DeplacerCarte)
            {
                CurseurEncours = Curseurs.CarteDeplacement;
            }
            else
            {
                CurseurEncours = Curseurs.CarteDefaut;
            }
        }
        /// <summary> renvoie le pointencours </summary>
        private PointD PointEncours
        {
            get
            {
                if (NomPointEncours == "Pt1")
                    return CapturerSettings.POINT1;
                if (NomPointEncours == "Pt2")
                    return CapturerSettings.POINT2;
                return default;
            }
            set
            {
                if (NomPointEncours == "Pt1")
                {
                    CapturerSettings.POINT1 = value;
                }
                else if (NomPointEncours == "Pt2")
                {
                    CapturerSettings.POINT2 = value;
                }
            }
        }
        /// <summary> Définit le pointencours avec les coordonnées de la souris </summary>
        private void DefinirPoint()
        {
            if (FlagLimitesSite && !LimitesVirtuellesSite.Contains(CoordMousePixels))
            {
                TitreInformation = "Action Hors Limites";
                MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture.";
                AfficherInformation();
            }
            else
            {
                PointEncours = CoordMouseGrille;
                GererIUMenuPts();
            }
        }
        /// <summary> Définit le pointencours avec les coordonnées du centre de l'affichage </summary>
        private void DefinirPointCentre()
        {
            if (FlagLimitesSite && !LimitesVirtuellesSite.Contains(Affichage.CentreAffichage))
            {
                MessageInformation = "Le centre de l'affichage est en dehors des limites du site de capture.";
                AfficherInformation();
            }
            else
            {
                DeplacerCurseurCentreVisue();
                // on attend que le curseur de la souris soit bien positioné par le systeme
                Attendre(150);
                DefinirPoint();
            }
        }
        /// <summary> Efface le point </summary>
        private void EffacerPoint()
        {
            PointEncours = new PointD();
            GererIUMenuPts();
        }
        /// <summary> demande au système de placer le curseur de souris au centre de l'affichage </summary>
        private void DeplacerCurseurCentreVisue()
        {
            Cursor.Position = new Point(VisueRectangle.X + CentreAffichage.X, VisueRectangle.Y + CentreAffichage.Y);
        }
        /// <summary> indique si la surfacecapture est bien définie pour la capture </summary>
        private void CalculerSurfaceTraitement()
        {
            SurfaceTraitement = new SurfaceTuiles(CapturerSettings.POINT1, CapturerSettings.POINT2, CapturerSettings.SITE, Affichage.Echelle);
            SurfaceTraitementIsOk = !SurfaceTraitement.IsEmpty && SurfaceTraitement.NbTuiles <= TelechargementCarte.NB_Max_TuilesServeurCarto && SurfaceTraitement.NbColonnes <= TelechargementCarte.NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, 1);
            if (FlagLimitesSite && !SurfaceTraitement.IsEmpty)
            {
                // on doit vérifier que les 2 points de la surface de traitement sont dans la limite du site
                var Pt1 = PointGrilleToPointPixel(CapturerSettings.POINT1, CapturerSettings.SITE, Affichage.Echelle);
                var Pt2 = PointGrilleToPointPixel(CapturerSettings.POINT2, CapturerSettings.SITE, Affichage.Echelle);
                if (!(LimitesVirtuellesSite.Contains(Pt1) && LimitesVirtuellesSite.Contains(Pt2)))
                {
                    MessageInformation = "Au moins un des points de la surface de traitement" + CrLf +
                                         "est en dehors des limites du site de capture.";
                    AfficherInformation();
                    SurfaceTraitementIsOk = false;
                }
            }
        }
        /// <summary> autorise ou non le dépassement des limites du site de caapture </summary>
        private void AppliquerLimitesSite()
        {
            FlagLimitesSite = !FlagLimitesSite;
            LimitesSite.Text = $"Limites site : {(FlagLimitesSite ? "Off" : "On")}";
            CalculerSurfaceTraitement();
            SurfaceAffichageTuile.Invalidate();
        }
        #endregion
    }
}