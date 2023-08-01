using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.Regroupement;
using static FCGP.Regrouper;
using static FCGP.Settings;
using static FCGP.StructuresJNX;

namespace FCGP
{

    /// <summary> permet la création des fichiers tuiles à plusieurs regroupements (niveau de détails ou échelle de capture) </summary>
    internal partial class FichiersTuilesJnxOrux
    {
        private int TailleReserveMini;
        private SitesCartographiques SiteCartoEncours;
        private CoordonneesGeoreferencements TypeCoordEncours;
        private int NumSiteEncours;
        private SystemeCartographique SystemeType;
        private Regroupement _RegroupementEncours;
        private const double TailleMaxi = 40.0d;
        private Rectangle SelectionEncours, SelectionModifie;
        private readonly Regroupement[] RegroupementsTuiles = new Regroupement[5];
        private readonly double[] NiveauJNXModifie = new double[5], JNXDefaut = new double[5];
        private readonly bool[] Inclus = new bool[5];
        private readonly int[] NiveauOruxModifie = new int[5], ORUXDefaut = new int[5];
        private int IndexMoins, NbTuilesX, NbTuilesY, DimTuile;
        private string MessInfo, CheminTuile;
        private string[] Coins;
        private bool FlagAjoutNiveau, FlagEvenement;
        private Datums Datum;
        private Echelles Echelle;
        private int IndiceEchelle;
        private string TitreMessageInformation;
        internal Regroupement RegroupementEncours
        {
            set
            {
                _RegroupementEncours = value;
            }
        }
        private void FichiersTuilesJnxOrux_Load(object sender, EventArgs e)
        {
            TitreMessageInformation = TitreInformation;
            // on indique au module carte le label où écrire les messages concernant le travail en cours
            LabelInformation = Information;
            TitreInformation = "Création des fichiers Orux et Jnx";
            SystemeType = _RegroupementEncours.SystemeType;
            NumSiteEncours = _RegroupementEncours.NumSite;
            TypeCoordEncours = SystemeType.CoordonneesGeoreferencement;
            SiteCartoEncours = SystemeType.SiteCarto;
            FlagEvenement = true;
            InitialiserValeurs();
            int Cpt = 0;
            foreach (KeyValuePair<string, Regroupement> R in Regroupements)
            {
                if (R.Value.NumSite == NumSiteEncours)
                {
                    RegroupementsTuiles[Cpt] = R.Value;
                    Inclus[Cpt] = true;
                    Panel C = (Panel)Support.Controls["Echelle" + Cpt];
                    C.Enabled = true;
                    InitialiserPanel(C, Cpt);
                    Cpt += 1;
                    if (Cpt == 5)
                        break;
                }
            }
            var S = RegroupementsTuiles[0].SystemeType;
            Datum = DatumSiteWeb((int)SiteCartoEncours);
            Echelle = S.Niveau.Echelle;
            IndiceEchelle = EchelleToSiteIndiceEchelle(SiteCartoEncours, Echelle);
            MettreAJourDimensions();
            CalculerSelectionReel();
            CheminTuile = PartagerSettings.CHEMIN_TUILE;
            CalculerPoidsTotal();
            FlagEvenement = false;
            Quitter.Select();
        }

        private void FichierTuilesJnxOrux_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SelectionEncours != SelectionModifie)
            {
                RegroupementsTuiles[0].SelectionGeorefToSelectionReferencement(SelectionModifie);
            }
            TitreInformation = TitreMessageInformation;
        }
        /// <summary> ajuste la couleur du texte et calcul la sélection réelle et le poids de la sélection </summary>
        private void CorDimension_ValueChanged(object sender, EventArgs e)
        {
            if (!FlagEvenement)
            {
                NumericUpDown C = (NumericUpDown)sender;
                if (C.Name == "CorLargeur")
                {
                    ChangerCouleurText(C, (double)C.Value, SelectionEncours.Width);
                }
                else
                {
                    ChangerCouleurText(C, (double)C.Value, SelectionEncours.Height);
                }
                CalculerSelectionReel();
                CalculerPoidsTotal();
            }
        }
        /// <summary> ajuste la largeur ou la hauteur de la sélection à un nb entier de tuiles </summary>
        private void CorDimension_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            if (e.X < C.Size.Width - 18)
            {
                int Value = (int)Math.Round(C.Value);
                var NewValue = default(decimal);
                switch (ModifierKeys)
                {
                    case Keys.Shift: // arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                        {
                            NewValue = (Value / DimTuile + 1) * DimTuile;
                            break;
                        }
                    case Keys.Control: // arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                        {
                            NewValue = (Value / DimTuile - 1) * DimTuile;
                            break;
                        }
                    case Keys.None: // arrondi le nb de pixels à la dimension de la tuile du fichier tuile. Permet de démarrer sur une valeur connue
                        {
                            NewValue = (decimal)(Math.Round(Value / (double)DimTuile) * DimTuile);
                            break;
                        }
                }
                if (NewValue >= C.Minimum && NewValue <= C.Maximum)
                {
                    C.Value = NewValue;
                }
            }
        }
        /// <summary> déplace le X ou Y de la sélection d'une tuile </summary>
        private void CorLocation_ValueChanged(object sender, EventArgs e)
        {
            if (!FlagEvenement)
            {
                NumericUpDown C = (NumericUpDown)sender;
                if (C.Name == "CorPT0X")
                {
                    ChangerCouleurText(C, (double)C.Value, SelectionEncours.Location.X);
                }
                else
                {
                    ChangerCouleurText(C, (double)C.Value, SelectionEncours.Location.Y);
                }
                CalculerSelectionReel();
            }
        }
        /// <summary> déplace le X ou Y de la sélection d'une tuile fichier ou arrondi aux tuiles serveur</summary>
        private void CorLocation_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            if (e.X < C.Size.Width - 18)
            {
                int Value = (int)Math.Round(C.Value);
                var NewValue = default(decimal);
                switch (ModifierKeys)
                {
                    case Keys.Shift: // décale en ajoutant le nb de pixels d'une tuile du fichier tuile
                        {
                            NewValue = (Value + NbPixelsTuile);
                            break;
                        }
                    case Keys.Control: // décale en enlèvant le nb de pixels d'une tuile du fichier tuile
                        {
                            NewValue = (Value - NbPixelsTuile);
                            break;
                        }
                    case Keys.None: // arrondi au pixel de début de la tuile serveur la plus proche. Permet de démarrer sur une valeur connue
                        {
                            NewValue = (decimal)Math.Round(Value / (double)NbPixelsTuile) * NbPixelsTuile;
                            break;
                        }
                }
                if (NewValue >= C.Minimum && NewValue <= C.Maximum)
                {
                    C.Value = NewValue;
                }
            }
        }
        /// <summary> inclu ou exclu un niveau du fichier tuile </summary>
        private void Include_CheckedChanged(object sender, EventArgs e)
        {
            if (!FlagEvenement)
            {
                CheckBox C = (CheckBox)sender;
                Inclus[int.Parse(C.Name[7..])] = C.Checked;
                FlagEvenement = true;
                MettreAJourDimensions();
                FlagEvenement = false;
                CalculerSelectionReel();
                CalculerPoidsTotal();
            }
        }
        /// <summary> lance la création du ou des fichiers tuiles </summary>
        private async void Lancer_Click(object sender, EventArgs e)
        {
            if (!VerifierValidation())
                return;
            Owner.Hide();
            MessageInformation = "Il y a eu une erreur lors de la création de la carte ou du fichier" + CrLf + "La création du ou des fichiers est arrêtée";
            Lancer.Enabled = false;
            Quitter.Enabled = false;
            Cursor = Cursors.WaitCursor;
            RegrouperSettings.FICHIERS_TUILE = (FichierJNX.Checked ? ChoixFichiersTuiles.JNX : ChoixFichiersTuiles.Aucun) |
                                               (FichierORUX.Checked ? ChoixFichiersTuiles.ORUX : ChoixFichiersTuiles.Aucun);
            RegrouperSettings.DIM_TUILE_JNXORUX = (int)Math.Round(N256.Value);
            RegrouperSettings.TAILLE_TAMPON = ChoixTailleTampon.SelectedIndex;
            RegrouperSettings.Ecrire();
            // on met à jour la sélection pour prendre en compte une modification
            if (SelectionEncours != SelectionModifie)
            {
                RegroupementsTuiles[0].SelectionGeorefToSelectionReferencement(SelectionModifie);
                SelectionEncours = RegroupementsTuiles[0].SelectionGeoref;
            }
            // il faut calculer les 4 coins de la carte à construire
            Coins = CalculerCoins();
            var DebutTraitement = DateTime.Now;
            try
            {
                for (int Cpt = 4; Cpt >= 0; Cpt -= 1) // il peut y avoir 5 niveaux de détails
                {
                    if (Inclus[Cpt] == true) // Creation fichier binaire correspondant à l'échelle si le niveau est inclus dans le fichier tuiles
                    {
                        MessInfo = NomCarte.Text + CrLf + "Création de la carte du niveau de détails : " + RegroupementsTuiles[Cpt].ClefEchelle;
                        Information.Text = MessInfo;
                        Information.Refresh();
                        int Compteur = Cpt;
                        if (!await Task.Run(() => RealiserFichiersTuiles(Compteur)))
                            return;
                    }
                }
                MessageInformation = $"Le ou les fichiers demandés ont été créés.{CrLf}Temps de traitement : {TimeSpanToStr(DateTime.Now - DebutTraitement, false)}";
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "B4A5");
            }
            AfficherInformation();
            Owner.Show();
            Close();
        }
        /// <summary> Réalise le ou les fichiers tuile demandés </summary>
        /// <param name="Cpt"> indice du niveau de détail </param>
        private bool RealiserFichiersTuiles(int Cpt)
        {
            bool RealiserFichiersTuilesRet = false;
            try
            {
                var R = RegroupementsTuiles[Cpt];
                string FichierBinaire = CheminEnregistrementProvisoire + @"\Fichierbinaire" + Cpt + ".raw";
                var Sys = R.SystemeType;
                var Selection = R.SelectionGeoref;
                // on calcule le nb d'octets par ligne
                int LargeurOctets = StrideImage(Selection.Width);
                // on vérifie que la hauteur de sélection fasse au moins une tuile
                int HauteurSelection = (Selection.Height > DimTuile ? Selection.Height : DimTuile) + 1;
                // on calcule combien de ligne peut tenir dans le tampon
                int HauteurTampon = TailleReserveMini / LargeurOctets;
                // et on prend la plus petite des 2
                if (HauteurTampon > HauteurSelection)
                    HauteurTampon = HauteurSelection;
                int TailleSelection = LargeurOctets * HauteurTampon;
                // Création de la carte du niveau et réserve de la place en mémoire pour le tampon de travail
                using (var CarteNiveau = new Carte(Selection.Width, Selection.Height, Sys, FichierBinaire, TailleSelection))
                {
                    // si il y a eu une erreur lors de la création de la carte on arrête la création du fichier tuiles
                    if (!CarteNiveau.IsOk)
                        return RealiserFichiersTuilesRet;
                    if (FichierORUX.Checked & NiveauOruxModifie[Cpt] != ORUXDefaut[Cpt] || FichierJNX.Checked & NiveauJNXModifie[Cpt] != JNXDefaut[Cpt])
                    {
                        FacteursSettings.Ecrire(Sys.IndiceSiteCarto, Sys.IndiceEchelle, NiveauJNXModifie[Cpt], NiveauOruxModifie[Cpt]);
                    }
                    // il faut assembler l'image de la carte sous forme d'un fichier binaire
                    if (!this.RealiserFichierBinaire(CarteNiveau, Selection, R.Cartes))
                        return RealiserFichiersTuilesRet;
                    int Compteur = Cpt;
                    // et réaliser le ou les ficheirs tuiles correspondnants
                    if (!FinaliserFichiersTuiles(CarteNiveau, Compteur))
                        return RealiserFichiersTuilesRet;
                    RealiserFichiersTuilesRet = true;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "X5R3");
            }
            return RealiserFichiersTuilesRet;
        }
        /// <summary>  pour chaque niveau de détail il faut créer l'image de la carte sous forme de fichier binaire </summary>
        /// <param name="CarteEncours"> carte contenant les information et le tampon pour la réalisation du fichier binaire </param>
        /// <param name="Selection"> région couverte par le fichier binaire </param>
        /// <param name="Cartes"> liste des cartes de capture permettant la réalisation de l'image globale </param>
        private bool RealiserFichierBinaire(Carte CarteEncours, Rectangle Selection, List<Carte> Cartes)
        {
            bool RealiserFichierBinaireRet = false;
            try
            {
                int Stride = CarteEncours.LargeurOctets; // nb d'octets pour une ligne du tampon
                Rectangle RaR;
                var Pt0 = Selection.Location;
                var HauteurLue = default(int);
                int HauteurTampon = CarteEncours.TailleReserve / Stride;
                if (HauteurTampon > Selection.Height + 1)
                    HauteurTampon = Selection.Height + 1; // hauteur de l'image au maximum 
                using (var T = new TamponVisue(new Size(Selection.Width, HauteurTampon), CarteEncours.Reserve, Cartes, "ImageNiveauDetail"))
                {
                    T.Pt0 = Pt0; // on cale le georef du tampon
                    RaR = T.Georef; // sur le début du georef de la carte
                    using (var FichierBin = new FileStream(CarteEncours.CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        do
                        {
                            string Info = MessInfo + CrLf + HauteurLue + " / " + Selection.Height;
                            AfficherVisueInformation(Info);
                            T.RemplirTampon(RaR, PartagerSettings.COULEUR_CARTE);
                            FichierBin.Write(T.Bits, T.IndexBits, HauteurTampon * Stride);
                            HauteurLue += HauteurTampon;
                            Pt0.Offset(0, HauteurTampon); // on décale le Georef du tampon
                            T.Pt0 = Pt0; // pour le prochain tour
                            if (HauteurLue + HauteurTampon > Selection.Height)
                            {
                                HauteurTampon = Selection.Height - HauteurLue;
                                RaR = new Rectangle(T.Pt0, new Size(T.Georef.Width, HauteurTampon));
                            }
                            else
                            {
                                RaR = T.Georef;
                            }
                        }
                        while (HauteurLue < Selection.Height);
                        RealiserFichierBinaireRet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "I5D3");
            }

            return RealiserFichierBinaireRet;
        }
        /// <summary> pour chaque niveau de détail il faut soit créer le fichier tuile soit ajouter un niveau au fichier tuile </summary>
        /// <param name="FichierBinaire"> chemin et nom du fichier binaire contenant l'image de la carte </param>
        /// <param name="Cpt"> numéro du niveau de détail, de 0 à 4 </param>
        private bool FinaliserFichiersTuiles(Carte CarteEncours, int Cpt)
        {
            CarteEncours.Separateur = CrLf;
            CarteEncours.Nom = NomCarte.Text;
            CarteEncours.Format = ImageFormat.Bmp;
            CarteEncours.DimensionsTuile = DimTuile;
            CarteEncours.CheminFichiersTuile = CheminTuile;
            CarteEncours.DemandeAfficherGrille = false;
            CarteEncours.DemandeAjouterCoordonneesGrille = ChoixCoordonneesGrille.Aucune;
            CarteEncours.DemandeFichiersGeoref = ChoixFichiersGeoref.Aucun;
            CarteEncours.FacteurJNX = NiveauJNXModifie[Cpt];
            CarteEncours.FacteurORUX = NiveauOruxModifie[Cpt];
            CarteEncours.DemandeFichiersTuiles = (FichierJNX.Checked ? ChoixFichiersTuiles.JNX : ChoixFichiersTuiles.Aucun) |
                                                 (FichierORUX.Checked ? ChoixFichiersTuiles.ORUX : ChoixFichiersTuiles.Aucun);
            CarteEncours.DemandeAjoutNiveau = FlagAjoutNiveau;
            FlagAjoutNiveau = true;
            var Info = NomCarte.Text + CrLf + "Finalisation de la carte du niveau de détails :  " + CarteEncours.SystemCartographique.Clef;
            bool FinaliserFichiersTuilesRet = CarteEncours.FinaliserCarte(Coins, Info);
            return FinaliserFichiersTuilesRet;
        }
        /// <summary> change la dimension des tuiles de fichier. 256 ou 512 </summary>
        private void N256_ValueChanged(object sender, EventArgs e)
        {
            DimTuile = (int)N256.Value;
            if (!FlagEvenement)
                CalculerPoidsTotal();
        }
        /// <summary> active ou non la création du fichier tuile JNX </summary>
        private void FichierJNX_CheckedChanged(object sender, EventArgs e)
        {
            if (!FlagEvenement)
                CalculerPoidsTotal();
        }
        /// <summary> change la taille de la mémoire tampon réservée pour la création des fichiers tuiles </summary>
        private void ChoixTailleTampon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChoixTailleTampon.SelectedIndex == 0)
            {
                TailleReserveMini = TailleMaxTampon / 3; // 0.167 Go
            }
            else if (ChoixTailleTampon.SelectedIndex == 1)
            {
                TailleReserveMini = TailleMaxTampon; // 0.500 Go
            }
            else
            {
                TailleReserveMini = TailleMaxTampon * 2;
            } // 1.000 Go
            if (!FlagEvenement)
                CalculerPoidsTotal();
        }
        /// <summary> change la valeur associée au zoom du niveau dans le fichier JNX </summary>
        private void CorZoomJNX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            int Cpt = Convert.ToInt32(C.Tag);
            NiveauJNXModifie[Cpt] = (double)C.Value;
            ChangerCouleurText(C, NiveauJNXModifie[Cpt], JNXDefaut[Cpt]);
        }
        /// <summary> change la couleur du texte du control si la valeur de celui-ci est modifiée par rapport à la valeur initiale</summary>
        /// <param name="C"> Control concerné </param>
        /// <param name="Val1"> valeur initiale </param>
        /// <param name="Val2"> valeur finale </param>
        private static void ChangerCouleurText(NumericUpDown C, double Val1, double Val2)
        {
            if (Val1 == Val2)
            {
                C.ForeColor = Color.YellowGreen;
            }
            else
            {
                C.ForeColor = Color.Tomato;
            }
        }
        /// <summary> change la valeur associée au zoom du niveau dans le fichier ORUX </summary>
        private void CorZoomORUX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            int Cpt = Convert.ToInt32(C.Tag);
            NiveauOruxModifie[Cpt] = (int)Math.Round(C.Value);
            ChangerCouleurText(C, NiveauOruxModifie[Cpt], ORUXDefaut[Cpt]);
        }
        /// <summary> transforme des coordonnées réelles en texte comme si elles venaient d'une capture </summary>
        /// <param name="Sys"> système cartographique associé aux coordonnées </param>
        private string[] CalculerCoins()
        {
            var Coins = new string[4];
            var Selection = SelectionReferencementSiteCarto(NumSiteEncours);
            if (TypeCoordEncours == CoordonneesGeoreferencements.DD)
            {
                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    Coins[Cpt] = ConvertPointDDtoDMS(Selection.Pt(Cpt));
            }
            else
            {
                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    Coins[Cpt] = ConvertPointXYtoChaine(new PointProjection(Selection.Pt(Cpt)), "N2");
            }
            return Coins;
        }
        /// <summary> MAJ des valeurs des différents controles associés aux dimensions virtuelles de l'emprise des fichiers tuiles </summary>
        private void MettreAJourDimensions()
        {
            for (int Cpt = 0; Cpt <= 4; Cpt++)
            {
                if (Inclus[Cpt]) // la première échelle est la bonne car c'est la plus grande
                {
                    IndexMoins = Cpt;
                    SelectionEncours = RegroupementsTuiles[IndexMoins].SelectionGeoref;
                    LLARGEUR.Text = SelectionEncours.Width.ToString();
                    CorLargeur.Minimum = SelectionEncours.Width - 10000;
                    CorLargeur.Maximum = SelectionEncours.Width + 10000;
                    CorLargeur.Value = SelectionEncours.Width;
                    LHAUTEUR.Text = SelectionEncours.Height.ToString();
                    CorHauteur.Minimum = SelectionEncours.Height - 10000;
                    CorHauteur.Maximum = SelectionEncours.Height + 10000;
                    CorHauteur.Value = SelectionEncours.Height;
                    LPT0X.Text = SelectionEncours.Location.X.ToString();
                    CorPT0X.Minimum = SelectionEncours.Location.X - 10000;
                    CorPT0X.Maximum = SelectionEncours.Location.X + 10000;
                    CorPT0X.Value = SelectionEncours.Location.X;
                    LPT0Y.Text = SelectionEncours.Location.Y.ToString();
                    CorPT0Y.Minimum = SelectionEncours.Location.Y - 10000;
                    CorPT0Y.Maximum = SelectionEncours.Location.Y + 10000;
                    CorPT0Y.Value = SelectionEncours.Location.Y;
                    return;
                }
            }
        }
        /// <summary> initialise les valeurs par défaut des champs de choix utilisateur </summary>
        private void InitialiserValeurs()
        {
            DimTuile = RegrouperSettings.DIM_TUILE_JNXORUX;
            N256.Value = DimTuile;
            FichierORUX.Checked = RegrouperSettings.FICHIERS_TUILE.HasFlag(ChoixFichiersTuiles.ORUX);
            FichierJNX.Checked = RegrouperSettings.FICHIERS_TUILE.HasFlag(ChoixFichiersTuiles.JNX);
            ChoixTailleTampon.Items.AddRange(new string[] { "Petit", "Moyen", "Grand" });
            ChoixTailleTampon.SelectedIndex = RegrouperSettings.TAILLE_TAMPON;
        }
        /// <summary> remplit les champs de chaque ligne dédié aux niveaux de dtails : 4 champs </summary>
        /// <param name="Pp"> Le panel à remplir </param>
        /// <param name="Cpt"> le Numéro du panel </param>
        private void InitialiserPanel(Panel Pp, int Cpt)
        {
            var Sys = RegroupementsTuiles[Cpt].SystemeType;
            ((CheckBox)Pp.Controls["Include" + Cpt]).Checked = true;
            // trouve le préfixe de la clef à afficher 
            string Clef = Sys.SiteCarto == SitesCartographiques.DomTom ? SystemeCartographique.DomTomClef(Sys.DomTom) :
                                                                         SystemeCartographique.SiteCartoClef(Sys.SiteCarto);
            // et rajoute la clef de l'échelle
            Pp.Controls["Clef" + Cpt].Text = Clef + "-" + Sys.Niveau.Clef;
            // trouve la valeur actuelle de l'indice d'affichage JNX pour l'échelle
            double ValeurActuelleJNX = FacteursSettings.Lire(Sys.IndiceSiteCarto, Sys.IndiceEchelle).FacteurJNX;
            int ValeurActuelleORUX = FacteursSettings.Lire(Sys.IndiceSiteCarto, Sys.IndiceEchelle).FacteurORUX;
            // et trouve la valeur par defaut de l'indice d'affichage JNX pour l'échelle
            JNXDefaut[Cpt] = Sys.Niveau.NiveauAffichageJNX;
            Pp.Controls["ZoomJNX" + Cpt].Text = DblToStr(ValeurActuelleJNX, PrecisionJNX);
            // la couleur du texte dépend si la valeur actuelle est la valeur par défaut
            if (ValeurActuelleJNX == JNXDefaut[Cpt])
            {
                Pp.Controls["ZoomJNX" + Cpt].ForeColor = Color.YellowGreen;
            }
            else
            {
                Pp.Controls["ZoomJNX" + Cpt].ForeColor = Color.Tomato;
            }
            NumericUpDown ValeurModifieJNX = (NumericUpDown)Pp.Controls["CorZoomJNX" + Cpt];
            // on donne la possibilité de modifier l'indice d'affichage dans une plage d +ou- 3 par rapport à la valeur par défaut
            ValeurModifieJNX.Minimum = (decimal)(JNXDefaut[Cpt] - 3d);
            ValeurModifieJNX.Maximum = (decimal)(JNXDefaut[Cpt] + 3d);
            if (ValeurActuelleJNX > JNXDefaut[Cpt] + 3d)
                ValeurActuelleJNX = JNXDefaut[Cpt] + 3d;
            if (ValeurActuelleJNX < JNXDefaut[Cpt] - 3d)
                ValeurActuelleJNX = JNXDefaut[Cpt] - 3d;
            ValeurModifieJNX.Value = (decimal)ValeurActuelleJNX;
            // idem pour l'indice d'affichage ORUX

            ORUXDefaut[Cpt] = Sys.Niveau.NiveauAffichageORUX;
            Pp.Controls["ZoomORUX" + Cpt].Text = ValeurActuelleORUX.ToString();
            if (ValeurActuelleORUX == ORUXDefaut[Cpt])
            {
                Pp.Controls["ZoomORUX" + Cpt].ForeColor = Color.YellowGreen;
            }
            else
            {
                Pp.Controls["ZoomORUX" + Cpt].ForeColor = Color.Tomato;
            }
            NumericUpDown ValeurModifieORUX = (NumericUpDown)Pp.Controls["CorZoomORUX" + Cpt]; // 
            ValeurModifieORUX.Minimum = ORUXDefaut[Cpt] - 3;
            ValeurModifieORUX.Maximum = ORUXDefaut[Cpt] + 3;
            if (ValeurActuelleORUX > ORUXDefaut[Cpt] + 3)
                ValeurActuelleORUX = ORUXDefaut[Cpt] + 3;
            if (ValeurActuelleORUX < ORUXDefaut[Cpt] - 3)
                ValeurActuelleORUX = ORUXDefaut[Cpt] - 3;
            ValeurModifieORUX.Value = ValeurActuelleORUX;
        }
        /// <summary> Calcul le nb max de tuiles sur l'axe des X </summary>
        private int NbTuilesMaxX()
        {
            int NbTuiles;
            // si il y a une demande d'interpolation sur le site SM il faut beaucoup plus de mémoire pour la réaliser d'où une limitation
            // plus importante sur l'axe des X sinon la limitation est donnée par la hauteur de la tuile
            if (SiteCartoEncours == SitesCartographiques.SuisseMobile && FichierJNX.Checked)
            {
                // constantes empiriques pour des tuiles de 256*256
                if (ChoixTailleTampon.SelectedIndex == 1) // 0.500 Go
                {
                    NbTuiles = 328;
                }
                else if (ChoixTailleTampon.SelectedIndex == 2) // 1.000 Go
                {
                    NbTuiles = 740;
                }
                else // 0.167 Go
                {
                    NbTuiles = 160;
                }
                if (DimTuile == 512)
                    NbTuiles /= 2;
            }
            else
            {
                NbTuiles = TailleReserveMini / (DimTuile * NbOctetsPixel * DimTuile);
            }
            return NbTuiles;
        }
        /// <summary> calcul du poids (nb d'octets) représenté par les images des différents niveaux de détails inclus dans les fichiers tuiles </summary>
        private void CalculerPoidsTotal()
        {
            var PoisTotalGeoref = default(double);
            int TuilesTotalGeoref;
            for (int Cpt = 0; Cpt <= 4; Cpt++)
            {
                if (Inclus[Cpt])
                {
                    PoisTotalGeoref += PoidsSelection(Cpt);
                }
            }
            PoisTotalGeoref /= Math.Pow(2d, 30d);
            NbTuilesX = (int)Math.Ceiling(SelectionModifie.Width / (double)DimTuile);
            NbTuilesY = (int)Math.Ceiling(SelectionModifie.Height / (double)DimTuile);
            TuilesTotalGeoref = NbTuilesX * NbTuilesY;
            int NBTuiles = NbTuilesMaxX();
            Lancer.Enabled = true;
            Poids.Text = $"{PoisTotalGeoref:N3} Go / {TailleMaxi:N0} Go";

            if (PoisTotalGeoref > TailleMaxi || PoisTotalGeoref == 0d)
            {
                Poids.BackColor = Color.Tomato;
                Lancer.Enabled = false;
            }
            else
            {
                Poids.BackColor = Color.YellowGreen;
            }

            NbTuilesToltal.Text = $"{TuilesTotalGeoref} / 50000";
            if (TuilesTotalGeoref > 50000)
            {
                NbTuilesToltal.BackColor = Color.Tomato;
                Lancer.Enabled = false;
            }
            else
            {
                NbTuilesToltal.BackColor = Color.YellowGreen;
            }

            NbTuileMaxX.Text = NbTuilesX.ToString() + " / " + NBTuiles.ToString();
            if (NbTuilesX > NBTuiles)
            {
                NbTuileMaxX.BackColor = Color.Tomato;
                Lancer.Enabled = false;
            }
            else
            {
                NbTuileMaxX.BackColor = Color.YellowGreen;
            }
        }
        /// <summary> calcul du poids (nb d'octets) représenté par l'image du niveau de détails </summary>
        /// <param name="Index"> Numéro du niveau de détails </param>
        private double PoidsSelection(int Index)
        {
            var Selection = RegroupementsTuiles[Index].SelectionGeoref;
            return StrideImage(Selection.Width) * (double)Selection.Height;
        }
        /// <summary> calcule les coordonnées réelles correspondant à la sélection virtuelle et MAJ les controles associés </summary>
        private void CalculerSelectionReel()
        {
            SelectionModifie.X = (int)Math.Round(CorPT0X.Value);
            SelectionModifie.Y = (int)Math.Round(CorPT0Y.Value);
            SelectionModifie.Width = (int)Math.Round(CorLargeur.Value);
            SelectionModifie.Height = (int)Math.Round(CorHauteur.Value);
            PT0Reel.Text = CoordonneesReelles(SelectionModifie.Location);
            PT2Reel.Text = CoordonneesReelles(new Point(SelectionModifie.Right, SelectionModifie.Bottom));
        }
        /// <summary> calcule les coordonnées réelles correspondant à la sélection virtuelle et MAJ les controles associés </summary>
        private string CoordonneesReelles(Point PtPixels)
        {
            string LabelCoordonnees = "";
            switch (RegrouperSettings.INDICE_TYPE_COORDONNEES)
            {
                case 0:
                    {
                        LabelCoordonnees = CoordMouseGrilleText(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle));
                        break;
                    }
                case 1:
                    {
                        LabelCoordonnees = CoordMousseDDText(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle), Datum);
                        break;
                    }
                case 2:
                    {
                        LabelCoordonnees = CoordMouseDMSText(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle), Datum);
                        break;
                    }
                case 3:
                    {
                        LabelCoordonnees = CoordMouseUtmWGS84(CoordMouseGrille(PtPixels, SiteCartoEncours, Echelle), Datum);
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
        /// <summary> vérifications avant de lancer la création des fichiers tuiles </summary>
        /// <returns> ok si tout est correct </returns>
        private bool VerifierValidation()
        {
            if (string.IsNullOrEmpty(NomCarte.Text + ""))
            {
                MessageInformation = "Vous devez indiquer un nom pour la carte";
                AfficherInformation();
                NomCarte.Focus();
                return false;
            }
            if (!FichierJNX.Checked & !FichierORUX.Checked)
            {
                MessageInformation = "Vous devez indiquer au moins un fichier tuiles";
                AfficherInformation();
                FichierJNX.Focus();
                return false;
            }
            int ValeurSupORUX = 50;
            double ValeurInfJNX = 0d;
            for (int Cpt = 0; Cpt <= 4; Cpt++)
            {
                if (Inclus[Cpt])
                {
                    var Sys = RegroupementsTuiles[Cpt].SystemeType;
                    if (NiveauJNXModifie[Cpt] > ValeurInfJNX)
                    {
                        ValeurInfJNX = NiveauJNXModifie[Cpt];
                    }
                    else
                    {
                        MessageInformation = "Le zoom JNX du niveau de détail " + Sys.Clef + "n'est pas supérieur" + CrLf + "au zoom JNX du niveau de détail inclus précédent";
                        AfficherInformation();
                        return false;
                    }
                    if (NiveauOruxModifie[Cpt] < ValeurSupORUX)
                    {
                        ValeurSupORUX = NiveauOruxModifie[Cpt];
                    }
                    else
                    {
                        MessageInformation = "Le zoom ORUX du niveau de détail " + Sys.Clef + "n'est pas inférieur" + CrLf + "au zoom ORUX du niveau de détail inclus précédent";
                        AfficherInformation();
                        return false;
                    }
                }
            }
            return true;
        }
    }
}