using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.Regroupement;
using static FCGP.Regrouper;
using static FCGP.Settings;

namespace FCGP
{

    /// <summary> permet la création de fichier tuiles KMZ à 1niveau de détails ou échelle de capture de taille plus importante que capture ou convertir </summary>
    internal partial class FichierTuilesKMZ
    {
        private int TailleReserveMini;
        private CoordonneesGeoreferencements TypeCoordEncours;
        private Regroupement _RegroupementEncours;
        private SystemeCartographique SystemeCartoEncours;
        private int NumSiteEncours;
        private const int NbTuilesMaxi = 3000;
        private Rectangle SelectionEncours, SelectionModifie;
        private int NbTuilesX, NbTuilesY, NbTuilesTotal, DimTuile;
        private string MessInfo, CheminTuile;
        private string[] Coins;
        private bool FlagEvenement;
        private Datums Datum;
        private SitesCartographiques SiteCarto;
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
        private void FichierTuilesKMZ_Load(object sender, EventArgs e)
        {
            TitreMessageInformation = TitreInformation;
            // on indique au module carte le label où écrire les messages concernant le travail en cours
            LabelInformation = Information;
            TitreInformation = "Création des fichiers Kmz";
            SystemeCartoEncours = _RegroupementEncours.SystemeType;
            TypeCoordEncours = _RegroupementEncours.SystemeType.CoordonneesGeoreferencement;
            NumSiteEncours = _RegroupementEncours.NumSite;
            SiteCarto = SystemeCartoEncours.SiteCarto;
            Datum = DatumSiteWeb((int)SiteCarto);
            IndiceEchelle = SystemeCartoEncours.IndiceEchelle;
            Echelle = SiteIndiceEchelleToEchelle(SiteCarto, IndiceEchelle);
            FlagEvenement = true;
            InitialiserValeurs();
            MettreAJourDimensions();
            CalculerSelectionReel();
            CheminTuile = PartagerSettings.CHEMIN_TUILE;
            CalculerNbTuilesKMZ();
            FlagEvenement = false;
            Quitter.Select();
        }
        private void FichierTuilesKMZ_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SelectionEncours != SelectionModifie)
            {
                _RegroupementEncours.SelectionGeorefToSelectionReferencement(SelectionModifie);
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
                CalculerNbTuilesKMZ();
            }
        }
        /// <summary> ajuste la largeur ou la hauteur de la sélection à un nb entier de tuiles </summary>
        private void CorDimension_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            if (e.X < C.Size.Width - 18)
            {
                int Value = (int)Math.Round(C.Value);
                decimal NewValue;
                switch (ModifierKeys)
                {
                    case Keys.Shift: // arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                        {
                            NewValue = (Value / DimTuile + 1) * DimTuile;
                            break;
                        }
                    case Keys.Control: // arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                        {
                            NewValue = (Value / DimTuile - 1) * DimTuile; // arrondi le nb de pixels à la dimension de la tuile du fichier tuile. Permet de démarrer sur une valeur connue
                            break;
                        }

                    default:
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
        /// <summary> change la taille de la mémoire tampon réservée pour la création des fichiers tuiles </summary>
        private void ChoixTailleTampon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChoixTailleTampon.SelectedIndex == 0)
            {
                TailleReserveMini = TailleMaxTampon / 3;
            }
            else if (ChoixTailleTampon.SelectedIndex == 1)
            {
                TailleReserveMini = TailleMaxTampon;
            }
            else
            {
                TailleReserveMini = TailleMaxTampon * 2;
            }

            if (!FlagEvenement)
                CalculerNbTuilesKMZ();
        }
        /// <summary> lance la création du ou des fichiers tuiles </summary>
        private async void Lancer_Click(object sender, EventArgs e)
        {
            if (!VerifierValidation())
                return;
            Owner.Hide();
            MessageInformation = "Il y a eu une erreur lors de la création de la carte ou du fichier." + CrLf + "La création du fichier Kmz est arrêtée";
            Lancer.Enabled = false;
            Quitter.Enabled = false;
            Cursor = Cursors.WaitCursor;

            RegrouperSettings.DIM_TUILE_KMZ = (int)Math.Round(N512.Value);
            RegrouperSettings.TAILLE_TAMPON = ChoixTailleTampon.SelectedIndex;
            RegrouperSettings.Ecrire();
            // on met à jour la sélection pour prendre en compte une modification
            if (SelectionEncours != SelectionModifie)
            {
                _RegroupementEncours.SelectionGeorefToSelectionReferencement(SelectionModifie);
                SelectionEncours = _RegroupementEncours.SelectionGeoref;
            }
            // il faut calculer les 4 coins de la carte à construire
            Coins = CalculerCoins();
            try
            {
                var DebutTraitement = DateTime.Now;
                MessInfo = NomCarte.Text + CrLf + "Création de la carte du niveau de détails : " + _RegroupementEncours.SystemeType.ClefEchelle;
                Information.Text = MessInfo;
                Information.Refresh();
                if (!await Task.Run(() => RealiserFichiersTuiles()))
                    return;
                MessageInformation = $"Le fichier {NomCarte.Text}.kmz a été créé.{CrLf}Temps de traitement : {TimeSpanToStr(DateTime.Now - DebutTraitement, false)}";
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A8T2");
            };
            AfficherInformation();
            Owner.Show();
            Close();
        }
        /// <summary> Réalisation du fichier KMZ demandé en arrière plan </summary>
        private bool RealiserFichiersTuiles()
        {
            bool RealiserFichiersTuilesRet = false;
            try
            {
                string FichierBinaire = CheminEnregistrementProvisoire + @"\Fichierbinaire.raw";
                var Sys = _RegroupementEncours.SystemeType;
                var Selection = _RegroupementEncours.SelectionGeoref;
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
                    // il faut assembler l'image de la carte sous forme d'un fichier binaire
                    if (!this.RealiserFichierBinaire(CarteNiveau, Selection, _RegroupementEncours.Cartes))
                        return RealiserFichiersTuilesRet;
                    // et réaliser le ou les ficheirs tuiles correspondnants
                    if (!FinaliserFichiersTuiles(CarteNiveau))
                        return RealiserFichiersTuilesRet;
                    RealiserFichiersTuilesRet = true;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "6DCB");
            }
            return RealiserFichiersTuilesRet;
        }
        /// <summary>  pour chaque niveau de détail il faut créer l'image de la carte sous forme de fichier binaire </summary>
        /// <param name="FichierBinaire"> chemin complet du fichier binaire qui sera créé </param>
        /// <param name="R"> regroupement (niveau de détail) en cours </param>
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
                AfficherErreur(Ex, "0693");
            }

            return RealiserFichierBinaireRet;
        } // '' <summary> pour chaque niveau de détail il faut soit créer le fichier tuile soit ajouter un niveau au fichier tuile </summary>
        /// <param name="FichierBinaire"> chemin et nom du fichier binaire contenant l'image de la carte </param>
        /// <param name="Cpt"> numéro du niveau de détail, de 0 à 4 </param>
        /// <param name="R"> regroupement (niveau de détail) en cours </param>
        private bool FinaliserFichiersTuiles(Carte CarteEncours)
        {
            CarteEncours.Separateur = CrLf;
            CarteEncours.Nom = NomCarte.Text;
            CarteEncours.Format = ImageFormat.Bmp;
            CarteEncours.DimensionsTuile = DimTuile;
            CarteEncours.CheminFichiersTuile = CheminTuile;
            CarteEncours.DemandeAfficherGrille = false;
            CarteEncours.DemandeAjouterCoordonneesGrille = ChoixCoordonneesGrille.Aucune;
            CarteEncours.DemandeFichiersGeoref = ChoixFichiersGeoref.Aucun;
            CarteEncours.DemandeFichiersTuiles = ChoixFichiersTuiles.KMZ;
            var Info = NomCarte.Text + CrLf + "Finalisation de la carte du niveau de détails :  " + CarteEncours.SystemCartographique.Clef;
            bool FinaliserFichiersTuilesRet = CarteEncours.FinaliserCarte(Coins, Info);
            return FinaliserFichiersTuilesRet;
        }
        /// <summary> change la dimension des tuiles de fichier. 256 ou 512 </summary>
        private void N512_ValueChanged(object sender, EventArgs e)
        {
            DimTuile = (int)N512.Value;
            if (!FlagEvenement)
                CalculerNbTuilesKMZ();
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
            SelectionEncours = _RegroupementEncours.SelectionGeoref;
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
        }
        /// <summary> initialise les valeurs par défaut des champs de choix utilisateur </summary>
        private void InitialiserValeurs()
        {
            DimTuile = RegrouperSettings.DIM_TUILE_KMZ;
            N512.Value = DimTuile;
            ChoixTailleTampon.Items.AddRange(new string[] { "Petit", "Moyen", "Grand" });
            ChoixTailleTampon.SelectedIndex = RegrouperSettings.TAILLE_TAMPON;
        }
        /// <summary> calcul du poids (nb d'octets) représenté par les images des différents niveaux de détails inclus dans les fichiers tuiles </summary>
        private void CalculerNbTuilesKMZ()
        {
            NbTuilesX = (int)Math.Ceiling(SelectionModifie.Width / (double)DimTuile);
            NbTuilesY = (int)Math.Ceiling(SelectionModifie.Height / (double)DimTuile);
            NbTuilesTotal = NbTuilesX * NbTuilesY;
            NbCols.Text = NbTuilesX.ToString("N0");
            NbRangs.Text = NbTuilesY.ToString("N0");
            NbTuilesKMZ.Text = $"{NbTuilesTotal:N0} / {NbTuilesMaxi:N0}";
            Lancer.Enabled = true;
            if (NbTuilesTotal > NbTuilesMaxi || NbTuilesTotal == 0)
            {
                NbTuilesKMZ.BackColor = Color.Tomato;
                Lancer.Enabled = false;
            }
            else
            {
                NbTuilesKMZ.BackColor = Color.YellowGreen;
            }
            int NBTuiles = NbTuilesMaxX();
            NbCols.Text = NbTuilesX.ToString() + " / " + NBTuiles.ToString();
            if (NbTuilesX > NBTuiles)
            {
                NbCols.BackColor = Color.Tomato;
                Lancer.Enabled = false;
            }
            else
            {
                NbCols.BackColor = Color.YellowGreen;
            }
        }
        /// <summary> Calcul le nb max de tuile sur l'axe des X </summary>
        private int NbTuilesMaxX()
        {
            int NbTuiles;
            // si il y a une demande d'interpolation sur le site SM il faut beaucoup plus de mémoire pour la réaliser d'où une limitation
            // plus importante sur l'axe des X sinon la limitation est donnée par la hauteur de la tuile
            if (SystemeCartoEncours.SiteCarto == SitesCartographiques.SuisseMobile)
            {
                // constantes empiriques pour des tuiles de 256*256
                if (ChoixTailleTampon.SelectedIndex == 1) // 0.500 Go
                {
                    NbTuiles = 328 / 2;
                }
                else if (ChoixTailleTampon.SelectedIndex == 2) // 1.000 Go
                {
                    NbTuiles = 740 / 2;
                }
                else // 0.167 Go
                {
                    NbTuiles = 160 / 2;
                }
                if (DimTuile == 1024)
                    NbTuiles /= 2;
            }
            else
            {
                NbTuiles = TailleReserveMini / (DimTuile * NbOctetsPixel * DimTuile);
            }
            return NbTuiles;
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
        /// <summary> Calcul des coordonnées réelles en fonction du type de coordonnées d'un point exprimées en coordonnées virtuelles </summary>
        /// <param name="PtPixels"> point en coordonnées virtuelles à convertir </param>
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
            return true;
        }
    }
}