using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> formulaire principal de l'application FCGP_Convertir </summary>
    internal sealed partial class Convertir
    {
        #region Variables globales
        /// <summary> remplace le controle FolderBrowser classique du windowsForm </summary>
        private FolderBrowserDialog ChercheChemin;
        /// <summary> nom de la carte en cours de traitement </summary>
        private string NomCarte;
        /// <summary> format d'enregistrement de la carte en cours de traitement </summary>
        private ImageFormat FormatCarte;
        /// <summary> support de la carte en cours de traitement </summary>
        private TypeSupportCarte SupportCarte;
        /// <summary> autorise le format d'enregistrment de la carte en cours de traitement </summary>
        private bool ToutFormat;
        /// <summary> indique si l'utilisateur à demandé l'annulation lors d'un traitement à plusieurs carte (mode batch) </summary>
        private bool FlagAnnuler;
        /// <summary> indique que l'application est en cours d'initialisation </summary>
        private bool FlagInit;
        /// <summary> indique si les 3 listes de choix sont libres ou gérées par le nb de path (choix imposés)</summary>
        private bool FlagNbPahsChanged;
        /// <summary> indique qu'il faudra enregistrer les choix de l'utilisateur en sortant de l'application </summary>
        private bool FlagEnregisterSettings;
        /// <summary> variables de sauvegarde associées au 3 listes de choix du formulaire </summary>
        private int SauvegardeIndexFichierTuile, SauvegardeIndexGeoref, SauvegardeIndexChoixReferences;
        /// <summary> sauvegarde temporaire </summary>
        private decimal ValeurDimTuilesLarg;
        /// <summary> sauvegarde temporaire de la liste de choix ouverte </summary>
        private ComboBox ComboxDropDown;
        /// <summary> liste pour stocker les traces sélectionnées </summary>
        private List<string> FichiersTRK;
        /// <summary> Nb de tuiles serveur sur l'axe des X </summary>
        private bool FlagNbTuilesX;
        /// <summary> traduction index text en valeur et vice versa pour les listes de choix </summary>
        private readonly string[] FichiersTuilesNoms = new[]
        {
                "Aucun", "ORUX", "JNX", "KMZ", "JNX KMZ", "JNX ORUX",
                "KMZ ORUX", "Tous"
        };
        private readonly ChoixFichiersTuiles[] FichiersTuilesValeurs = new[]
        {
                ChoixFichiersTuiles.Aucun, ChoixFichiersTuiles.ORUX,
                ChoixFichiersTuiles.JNX, ChoixFichiersTuiles.KMZ,
                ChoixFichiersTuiles.JNX_KMZ, ChoixFichiersTuiles.JNX_ORUX,
                ChoixFichiersTuiles.KMZ_ORUX, ChoixFichiersTuiles.Tous
        };
        private readonly string[] FichiersGeorefNoms = new[]
        {
                "Aucun", "Cartes", "Compeland Carte", "Compeland Interpol",
                "Compelands", "Georef Carte", "Georef Interpol", "Georefs",
                "Interpols", "MapInfo Carte", "MapInfo Interpol", "MapInfos",
                "OziExploreur Carte", "OziExploreur Interpol","OziExploreurs",
                "QGIS Carte", "QGIS Interpol", "QGISs","Tous"
        };
        private readonly ChoixFichiersGeoref[] FichiersGeorefValeurs = new[]
        {
                ChoixFichiersGeoref.Aucun, ChoixFichiersGeoref.Cartes, ChoixFichiersGeoref.CompeLand_Carte,
                ChoixFichiersGeoref.CompeLand_Interpol, ChoixFichiersGeoref.CompeLands,
                ChoixFichiersGeoref.Georef_Carte, ChoixFichiersGeoref.Georef_Interpol,
                ChoixFichiersGeoref.Georefs, ChoixFichiersGeoref.Interpols,
                ChoixFichiersGeoref.MapInfo_Carte, ChoixFichiersGeoref.MapInfo_Interpol,
                ChoixFichiersGeoref.MapInfos, ChoixFichiersGeoref.OziExploreur_Carte,
                ChoixFichiersGeoref.OziExploreur_Interpol, ChoixFichiersGeoref.OziExploreurs,
                ChoixFichiersGeoref.QGIS_Carte, ChoixFichiersGeoref.QGIS_Interpol,
                ChoixFichiersGeoref.QGISs, ChoixFichiersGeoref.Tous
        };
        private readonly ChoixCoordonneesGrille[] GrillesReferenceValeurs = new[] { ChoixCoordonneesGrille.Aucune, ChoixCoordonneesGrille.Ref_Carte, ChoixCoordonneesGrille.Ref_Interpol, ChoixCoordonneesGrille.Toutes };
        private readonly int[] NbTuilesMaxCarteSuisseInterpol = new int[] { 410, 410, 204 };
        #endregion
        #region Evenements du formulaire et des controles
        /// <summary> initialisation complète de tous les controles à partir du settings </summary>
        private void Convertir_Load(object sender, EventArgs e)
        {
            if (!InitialiserBaseApplication(this, "Core 4.000 CS", "01/03/2023"))
            {
                Close();
                return;
            }
            TitreInformation = "Information FCGP_Convertir";
            // indique aux threads hors UI la procédure à appeler pour afficher un message d'information ou une erreur
            FormHorsUI = new Action(AfficherInfomationsErreurs);
            // si la base des settings n'existe pas elle est crée avec des valeurs par défaut
            if (!InitialiserSettings("9.000"))
            {
                Close();
                return;
            }
            if (InitialiserCommun())
            {
                FlagInit = true;
                MessageInformation = "";
                LabelInformation = Information;
                // met à jour les listes déroulantes des choix qui permettent de produire une sortie de fichier. Celles-ci sont associées à une variable de sauvegarde
                GeoRefs.Items.AddRange(FichiersGeorefNoms);
                References.Items.AddRange(CoordonneesGrille);
                FichiersTuiles.Items.AddRange(FichiersTuilesNoms);
                // sélectionne les derniers choix effectués
                GeoRefs.SelectedIndex = ConvertirSettings.GEOREF;
                References.SelectedIndex = ConvertirSettings.REFERENCE;
                FichiersTuiles.SelectedIndex = ConvertirSettings.FICHIER_TUILE;
                FormatCartes.SelectedIndex = ConvertirSettings.FORMAT;
                IsGrille.Checked = ConvertirSettings.IS_GRILLE;
                IsTrace.Checked = ConvertirSettings.IS_TRACE;
                DimTuilesLarg.Value = (decimal)ConvertirSettings.DIMENSIONS_TUILE;
                Couleur.BackColor = ConvertirSettings.COULEUR_TRACE;
                Transparence.Value = ValeurToPourcent(Couleur.BackColor.A);
                Epaisseur.Value = (decimal)ConvertirSettings.EPAISSEUR_TRACE;
                NbPaths.Value = (decimal)ConvertirSettings.NB_PATHS;
                LabelRepertoireCartes.Text = ConvertirSettings.CHEMIN_CARTE;
                LabelRepertoireTuiles.Text = ConvertirSettings.CHEMIN_TUILE;
                // initialisation des autres contrôlesdépendant d'un autre settings
                OuvrirFichiersTrace.Title = "Choisir une ou plusieurs traces";
                OuvrirFichiersTrace.InitialDirectory = TracesSettings.CHEMIN_TRACE;
                OuvrirFichiersCarte.Title = "Choisir une ou plusieurs cartes pour le traitement";
                OuvrirFichiersCarte.InitialDirectory = CapturerSettings.CHEMIN_CARTE;
                FichiersTRK = new List<string>(5);
                Text = NumFCGP;
                FlagInit = false;
                InitialiserConvertir();
                {
                    Location = new Point(VisueRectangle.X + VisueRectangle.Width / 2 - Size.Width / 2,
                                         VisueRectangle.Y + VisueRectangle.Height / 2 - Size.Height / 2);
                }
                ChercheChemin = new FolderBrowserDialog();
#if !BETA
                VerifierVersionApplication();
#endif
            }
            else
            {
                Close();
            }
        }
        /// <summary> vérifie que le formulaire de l'application peut être fermé suite à une demande de l'utilisateur </summary>
        private void Convertir_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Traitement.Text.StartsWith("Annuler"))
            {
                MessageInformation = "Vous devez attendre la fin du traitement.";
                AfficherInformation();
                e.Cancel = true;
            }
        }
        /// <summary> ferme le formulaire de l'application suite à une demande de l'utilisateur </summary>
        private void Convertir_FormClosed(object sender, FormClosedEventArgs e)
        {
            // on libère les ressources et efface le répertoire provisoire créé au lancement de FCGP
            CloturerCommun();
            if (FlagEnregisterSettings)
            {
                ConvertirSettings.GEOREF = SauvegardeIndexGeoref;
                ConvertirSettings.REFERENCE = SauvegardeIndexChoixReferences;
                ConvertirSettings.FICHIER_TUILE = SauvegardeIndexFichierTuile;
                ConvertirSettings.Ecrire();
            }
        }
        /// <summary> on récupère toutes le frappe de clavier à cet endroit pour traiter l'ouverture du formulaire d'aide </summary>
        private void Convertir_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        if (ComboxDropDown is not null)
                        {
                            ComboxDropDown.DroppedDown = false;
                            ComboxDropDown = null;
                        }

                        break;
                    }
                case Keys.F1:
                    {
                        AfficherAide(this);
                        break;
                    }
            }
        }
        /// <summary> connaitre l'écran sur lequel le formulaire est affiché</summary>
        private void Convertir_LocationChanged(object sender, EventArgs e)
        {
            VisueRectangle = RectangleToScreen(ClientRectangle);
        }
        /// <summary> met ajour la couleur de fond du label en fonction d'un choix de couleur et du % de transparence </summary>
        private void Couleur_Click(object sender, EventArgs e)
        {
            ChoixCouleurTrace.Color = Couleur.BackColor;
            if (ChoixCouleurTrace.ShowDialog() == DialogResult.OK)
            {
                Couleur.BackColor = Color.FromArgb(PourcentToValeur((int)Math.Round(Transparence.Value)), ChoixCouleurTrace.Color);
            }
        }
        /// <summary> action à réaliser lors la modification du check du controle </summary>
        private void IsTrace_CheckedChanged(object sender, EventArgs e)
        {
            DefinirEtatControles();
        }
        /// <summary> met à jour la couleur du label couleur en fonction du pourcentage de transparence désiré par l'utilisateur </summary>
        private void Transparence_ValueChanged(object sender, EventArgs e)
        {
            Couleur.BackColor = Color.FromArgb(PourcentToValeur((int)Math.Round(Transparence.Value)), Couleur.BackColor);
        }
        /// <summary> permet de changer le répertoire d'enregistrement des cartes issues du traitement </summary>
        private void LabelRepertoireCartes_Click(object sender, EventArgs e)
        {
            try
            {
                ChercheChemin.Description = "Chemin d'enregistrement des fichiers de géo-référencement";
                ChercheChemin.InitialDirectory = LabelRepertoireCartes.Text;
                // Affiche la boite de dialogue de sélection de répertoires
                if (ChercheChemin.ShowDialog(this) == DialogResult.OK)
                {
                    // et met à jour le champ
                    LabelRepertoireCartes.Text = ChercheChemin.SelectedPath;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "G4M6");
            }
        }
        /// <summary> permet de changer le répertoire d'enregistrement des fichiers tuiles issus du traitement </summary>
        private void LabelRepertoireFichiersTuiles_Click(object sender, EventArgs e)
        {
            try
            {
                ChercheChemin.Description = "Chemin d'enregistrement des fichiers tuiles";
                ChercheChemin.InitialDirectory = LabelRepertoireTuiles.Text;
                // Affiche la boite de dialogue de sélection de répertoires
                if (ChercheChemin.ShowDialog(this) == DialogResult.OK)
                {
                    // et met à jour le champ
                    LabelRepertoireTuiles.Text = ChercheChemin.SelectedPath;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "R7C3");
            }
        }
        /// <summary> permet de sélectionner une ou plusieurs traces de type FCGP (.trk) </summary>
        private void LabelTraces_Click(object sender, EventArgs e)
        {
            try
            {
                // Affiche la boite de dialogue de sélection des traces
                if (OuvrirFichiersTrace.ShowDialog() == DialogResult.OK)
                {
                    OuvrirFichiersTrace.InitialDirectory = Path.GetDirectoryName(OuvrirFichiersTrace.FileName);
                    // et met à jour la liste déroulante des traces choisies
                    Traces.Items.Clear();
                    FichiersTRK.Clear();
                    foreach (string T in OuvrirFichiersTrace.FileNames)
                    {
                        string Ext = Path.GetExtension(T);
                        if (Ext == ".trk")
                        {
                            Traces.Items.Add(Path.GetFileNameWithoutExtension(T));
                            FichiersTRK.Add(T);
                        }
                    }
                    if (Traces.Items.Count > 0)
                    {
                        LabelTraces.Text = (string)Traces.Items[0];
                        OuvrirFichiersTrace.InitialDirectory = Path.GetDirectoryName(FichiersTRK[0]); // 
                    }
                    DefinirEtatControles();
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T4S5");
            }
        }
        /// <summary> le nb de paths détermine l'état des groupe Cartes et FichiersTuiles </summary>
        private void NbPaths_ValueChanged(object sender, EventArgs e)
        {
            DefinirEtatControles();
        }
        /// <summary> ouvre la liste des choix associée au label </summary>
        private void LabelListesChoix_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            ComboxDropDown = (ComboBox)L.Tag;
            ComboxDropDown.DroppedDown = true;
        }
        /// <summary> ferme la liste déroulante des choix de type de coordonnées </summary>
        private void ListesChoix_DropDownClosed(object sender, EventArgs e)
        {
            ComboxDropDown = null;
        }
        /// <summary> met à jour le label associé à la liste des choix </summary>
        private void FormatCartes_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelFormatCartes.Text = FormatCartes.SelectedItem.ToString();
        }
        /// <summary> met à jour le label associé à la liste des choix </summary>
        private void References_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FlagNbPahsChanged)
            {
                // si ce n'est pas la phase d'initialisation du formulaire
                SauvegardeIndexChoixReferences = References.SelectedIndex;
            }
            LabelReferences.Text = CoordonneesGrille[References.SelectedIndex];
        }
        /// <summary> met à jour le label associé à la liste des choix </summary>
        private void GeoRefs_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelGeoRefs.Text = GeoRefs.SelectedItem.ToString();
            if (!FlagNbPahsChanged)
            {
                SauvegardeIndexGeoref = GeoRefs.SelectedIndex;
                DefinirEtatControles();
            }
        }
        /// <summary> action à réaliser lors de la modification de la sélection d'un index du controle et met à jour le label associé à la liste des choix </summary>
        private void FichiersTuiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelFichiersTuiles.Text = FichiersTuiles.SelectedItem.ToString();
            if (!FlagNbPahsChanged)
            {
                SauvegardeIndexFichierTuile = FichiersTuiles.SelectedIndex;
                DefinirEtatControles();
            }
        }
        /// <summary> enlève la possibilité du choix 768</summary>
        private void DimTuilesLarg_ValueChanged(object sender, EventArgs e)
        {
            if (DimTuilesLarg.Value == 768m)
            {
                if (ValeurDimTuilesLarg > 768m)
                {
                    DimTuilesLarg.Value = 512m;
                }
                else
                {
                    DimTuilesLarg.Value = 1024m;
                }
            }
            ValeurDimTuilesLarg = DimTuilesLarg.Value;
        }
        /// <summary> annule la changement d'index </summary>
        private void Traces_SelectedIndexChanged(object sender, EventArgs e)
        {
            Traces.SelectedIndex = -1;
        }
        /// <summary> action lié au bouton Quitter : Demande de fermeture du formulaire général </summary>
        private void Quitter_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary> Lance le traitement des cartes si la sélection d'une ou plusieurs cartes est OK </summary>
        private void Traitement_Click(object sender, EventArgs e)
        {
            if (!Traitement.Text.StartsWith("Annuler"))
            {
                if (OuvrirFichiersCarte.ShowDialog() == DialogResult.OK)
                {
                    FlagEnregisterSettings = true;
                    OuvrirFichiersCarte.InitialDirectory = Path.GetDirectoryName(OuvrirFichiersCarte.FileName);
                    // met le bouton de traitement soit en mode Annuler si plusieurs cartes soit 
                    // le rend inactif si une seule carte car on ne peut pas arrêter un traitment encours
                    if (OuvrirFichiersCarte.FileNames.Length > 1)
                    {
                        Traitement.Text = "Annuler le traitement des autres cartes";
                        Informations.SetToolTip(Traitement, "Annulez les traitements restants" + CrLf +
                                                            "après le traitement en cours.");
                    }
                    else
                    {
                        Traitement.Enabled = false;
                    }
                    GroupeConfigurations.Enabled = false;
                    Quitter.Enabled = false;
                    // renseigne le setting avec les différents paramètres choisis par l'utilisateur
                    ConvertirSettings.GEOREF = GeoRefs.SelectedIndex;
                    ConvertirSettings.FORMAT = FormatCartes.SelectedIndex;
                    ConvertirSettings.IS_GRILLE = IsGrille.Checked;
                    ConvertirSettings.REFERENCE = References.SelectedIndex;
                    ConvertirSettings.FICHIER_TUILE = FichiersTuiles.SelectedIndex;
                    ConvertirSettings.DIMENSIONS_TUILE = (int)Math.Round(DimTuilesLarg.Value);
                    ConvertirSettings.CHEMIN_CARTE = LabelRepertoireCartes.Text;
                    ConvertirSettings.CHEMIN_TUILE = LabelRepertoireTuiles.Text;
                    ConvertirSettings.IS_TRACE = IsTrace.Checked;
                    ConvertirSettings.COULEUR_TRACE = Couleur.BackColor;
                    ConvertirSettings.EPAISSEUR_TRACE = (int)Math.Round(Epaisseur.Value);
                    ConvertirSettings.NB_PATHS = (int)Math.Round(NbPaths.Value);
                    TraitementAsync(OuvrirFichiersCarte.FileNames);
                }
            }
            else if (!FlagAnnuler)
                FlagAnnuler = true;
        }
        #endregion
        #region Traitement des cartes
        /// <summary> Traitement des cartes séléctionées. On lance le travail et rend la main immédiatement
        /// de manière à pouvoir annuler le traitement si il y a plusieurs cartes à traiter </summary>
        /// <param name="FichiersGeorefInit"></param>
        private async void TraitementAsync(string[] FichiersGeorefInit)
        {
            SystemeCartographique SystemeType = null;
            // pour chaque fichier georef sélectionné
            for (int Cpt = 0, loopTo = FichiersGeorefInit.Length - 1; Cpt <= loopTo; Cpt++)
            {
                if (!FlagAnnuler)
                {
                    TrouverParametresCarte(FichiersGeorefInit[Cpt]);
                    if (FlagNbTuilesX)
                    {
                        MessageInformation = $"La carte {NomCarte} ne supporte pas d'être interpolée.{CrLf}Elle ne sera pas prise en compte.";
                        AfficherInformation();
                    }
                    else
                    {
                        SystemeType = Carte.RenvoyerSystemeCartographique(FichiersGeorefInit[Cpt]);
                        if (SystemeType?.IsOk == true)
                        {
                            // on cherche les différents paramètre permettant le dimensionnement du tampon
                            Information.Text = "Chargement de la carte : " + CrLf + NomCarte;
                            await Task.Run(() => TraitementCarte(FichiersGeorefInit[Cpt]));
                        }
                        else
                        {
                            if (SystemeType is null)
                            {
                                MessageInformation = $"La carte {NomCarte} est une carte interpolée.{CrLf}Elle ne sera pas prise en compte.";
                            }
                            else
                            {
                                MessageInformation = $"Le système cartographique de la carte {NomCarte} n'est pas reconnu.{CrLf}Elle ne sera pas prise en compte.";
                            }
                            AfficherInformation();
                        }
                    }
                }
            }
            InitialiserConvertir();
        }
        /// <summary> traitement pour une carte </summary>
        /// <param name="FichierGeoref"></param>
        /// <returns></returns>
        private VerificationRenvoiCarte TraitementCarte(string FichierGeoref)
        {
            var Ret = default(VerificationRenvoiCarte);
            using (var CarteActuelle = Carte.RenvoyerCarte(FichierGeoref, ref Ret, SupportCarte, ToutFormat, default, FichiersTuilesValeurs[ConvertirSettings.FICHIER_TUILE]))
            {
                if (Ret == VerificationRenvoiCarte.OK)
                {
                    CarteActuelle.Separateur = CrLf; // " "
                    CarteActuelle.Format = FormatCarte;
                    CarteActuelle.CheminCarte = ConvertirSettings.CHEMIN_CARTE;
                    CarteActuelle.CheminFichiersTuile = ConvertirSettings.CHEMIN_TUILE;
                    CarteActuelle.FacteurJNX = FacteursSettings.Lire(CarteActuelle.SystemCartographique.IndiceSiteCarto, CarteActuelle.SystemCartographique.IndiceEchelle).FacteurJNX;
                    CarteActuelle.FacteurORUX = FacteursSettings.Lire(CarteActuelle.SystemCartographique.IndiceSiteCarto, CarteActuelle.SystemCartographique.IndiceEchelle).FacteurORUX;
                    CarteActuelle.DemandeAfficherGrille = ConvertirSettings.IS_GRILLE;
                    CarteActuelle.DemandeAjouterCoordonneesGrille = GrillesReferenceValeurs[ConvertirSettings.REFERENCE];
                    CarteActuelle.DemandeFichiersGeoref = FichiersGeorefValeurs[ConvertirSettings.GEOREF];
                    CarteActuelle.PinceauTrace = new Pen(ConvertirSettings.COULEUR_TRACE, ConvertirSettings.EPAISSEUR_TRACE) { LineJoin = LineJoin.Round };
                    CarteActuelle.FichiersTRK = FichiersTRK;
                    CarteActuelle.DemandeAfficherTrace = ConvertirSettings.IS_TRACE && ConvertirSettings.NB_PATHS == 0;
                    CarteActuelle.NbTracesKML = ConvertirSettings.IS_TRACE ? ConvertirSettings.NB_PATHS : 0;
                    CarteActuelle.RealiserDemandes();
                }
                else
                {
                    MessageInformation = $"Le fichier de la carte{NomCarte} n'est pas conforme";
                    AfficherInformation();
                }
            }
            return Ret;
        }
        /// <summary> calcule le nom de base, le format et la taille du tampon support pour la carte au début du traitement </summary>
        /// <param name="FichierGeoref"></param>
        private void TrouverParametresCarte(string FichierGeoref)
        {
            string[] LignesGeoref = File.ReadAllLines(FichierGeoref, Encoding_FCGP);
            int Pos = LignesGeoref[0].IndexOf('.') + 1;
            // Trouve le système de capture de la carte
            string Site = TrouverChaineCompriseEntre(LignesGeoref[0], ref Pos, ':');
            Pos = 0;
            string CheminCarte = TrouverChaineCompriseEntre(LignesGeoref[0], ref Pos, ':', ',').Trim(' ');
            string[] S = CheminCarte.Split('.');
            NomCarte = S[0];
            FormatCarte = ConvertIntToImageFormat(ConvertirSettings.FORMAT);

            Pos = 0;
            int LargPix = int.Parse(TrouverChaineCompriseEntre(LignesGeoref[11], ref Pos, ':', ','));
            int HautPix = int.Parse(TrouverChaineCompriseEntre(LignesGeoref[11], ref Pos, ':'));
            int PoidsCarte = StrideImage(LargPix) * HautPix;
            if (PoidsCarte > TailleMaxTampon)
            {
                SupportCarte = TypeSupportCarte.Fichier;
            }
            else if (FlagSmallMemory)
            {
                SupportCarte = TypeSupportCarte.MemoryStatic;
            }
            else
            {
                SupportCarte = TypeSupportCarte.MemoryDynamic;
            }
            ToutFormat = true;
            if (SupportCarte == TypeSupportCarte.Fichier && FormatCarte == ImageFormat.Bmp)
                ToutFormat = false;
            int NbTuilesX = (int)Math.Ceiling(LargPix / (double)NbPixelsTuile);
            FlagNbTuilesX = Site == "SuisseMobile" && NbTuilesX > NbTuilesMaxCarteSuisseInterpol[(int)CartesSettings.SUPPORT_CARTE];
        }
        #endregion
        #region Autres procédures ou fonctions
        /// <summary> initialisation à chaque fin de traitement de l'ensemble des cartes </summary>
        private void InitialiserConvertir()
        {
            Traitement.Text = "Selection Fichier(s) Georef et Traitement";
            Informations.SetToolTip(Traitement, "Sélectionnez un ou plusieurs fichiers Georef à convertir." + CrLf +
                                                "La validation de la sélection des fichiers lancera le traitement.");
            Quitter.Enabled = true;
            GroupeConfigurations.Enabled = true;
            DefinirEtatControles();
            Information.Text = "";
            FlagAnnuler = false;
        }
        /// <summary> définit l'etat et éventuellement la valeur des controles du formulaire
        /// en fonction des choix faits par l'utilisateur </summary>
        private void DefinirEtatControles()
        {
            if (!FlagInit)
            {
                // on interdit l'évenement ChoixFichiersTuile.SelectedIndexChanged
                FlagNbPahsChanged = true;
                if (NbPaths.Value > 0m && IsTrace.Checked)
                {
                    // kmz uniquement
                    FichiersTuiles.SelectedIndex = 3;
                    GeoRefs.SelectedIndex = 0;
                    References.SelectedIndex = 0;
                }
                else
                {
                    // choix utilisateur
                    FichiersTuiles.SelectedIndex = SauvegardeIndexFichierTuile;
                    GeoRefs.SelectedIndex = SauvegardeIndexGeoref;
                    References.SelectedIndex = SauvegardeIndexChoixReferences;
                }
                FlagNbPahsChanged = false;
                // d'abord les controles liés à l'inclusion des trace
                OuvrirFichiersCarte.Multiselect = NbPaths.Value == 0m;
                Traces.Enabled = IsTrace.Checked && Traces.Items.Count > 1;
                EtiquetteTraces.Enabled = IsTrace.Checked && Traces.Items.Count > 0;
                LabelTraces.Enabled = IsTrace.Checked;
                NbPaths.Enabled = IsTrace.Checked;
                EtiquetteNbPaths.Enabled = NbPaths.Enabled;
                LabelGeoRefs.Enabled = !IsTrace.Checked || NbPaths.Value == 0m;
                EtiquetteGeoRefs.Enabled = LabelGeoRefs.Enabled;
                // si il s'agit juste de rajoutée la trace dans un fichier kmz on interdit tous les controles
                // sauf le choix de traces, la dimension des tuiles, le nombre de path et si on prend en compte les trace        
                GroupeCartes.Enabled = GeoRefs.SelectedIndex > 0;
                GroupeFichiersTuiles.Enabled = FichiersTuiles.SelectedIndex > 0;
                GroupeConfigurationTraces.Enabled = IsTrace.Checked && NbPaths.Value == 0m;
                LabelFichiersTuiles.Enabled = NbPaths.Value == 0m || !IsTrace.Checked;
                EtiquetteFichiersTuiles.Enabled = LabelFichiersTuiles.Enabled;

                Traitement.Enabled = false;
                if (!IsTrace.Checked)
                {
                    Traitement.Enabled = FichiersTuiles.SelectedIndex > 0 || GeoRefs.SelectedIndex > 0;
                }
                else
                {
                    Traitement.Enabled = Traces.Items.Count > 0 && (FichiersTuiles.SelectedIndex > 0 || GeoRefs.SelectedIndex > 0);
                }
            }
        }
        /// <summary> tranforme un % transparence de 0 à 100 à une valeur du canal alpha de couleur de 255 à 0 </summary>
        /// <param name="Trans"> % transparence </param>
        private static int PourcentToValeur(int Trans)
        {
            return 255 - 255 * Trans / 100;
        }
        /// <summary> transforme une valeur du canal alpha de couleur de 255 à 0 en un % transparence de 0 à 100 </summary>
        /// <param name="Valeur"> valeur du canal alpha </param>
        /// <returns></returns>
        private static int ValeurToPourcent(byte Valeur)
        {
            return 100 - Valeur * 100 / 255;
        }
        /// <summary> procédure appelée par AffichageInformation du module Information lors d'appel inter-threads </summary>
        internal void AfficherInfomationsErreurs()
        {
            AfficherInformation();
        }
        #endregion
    }
}