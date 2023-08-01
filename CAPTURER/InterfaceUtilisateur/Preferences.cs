using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> permet la configuration de la sortie de la capture de la carte </summary>
    internal partial class Preferences
    {
        private int IndexFichiersTuiles, DimensionsTuile;
        private FolderBrowserDialog ChercheChemin;
        private Rectangle StockClipCurseur;
        private SurfaceTuiles _SurfaceTraitement;
        /// <summary> nb de tuiles max en colonnes pour l'interpolation des cartes suisses sous peine de plantage. Valeurs empiriques </summary>
        private readonly int[] NbTuilesMaxCarteSuisseInterpol = new int[] { 328, 328, 204 };
        private string Titre;

        internal SurfaceTuiles SurfaceTraitement
        {
            set
            {
                _SurfaceTraitement = value;
            }
        }

        /// <summary>associe les informations du settings avec les champs de données du formulaire et remet à zéro le nom de la carte</summary>
        private void Preferences_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            try
            {

                IsTrace.Enabled = Convert.ToBoolean(Tag);
                if (IsTrace.Enabled)
                    IsTrace.Checked = CartesSettings.IS_TRACE;

                if ((CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier && CartesSettings.IS_TOUT_FORMAT == false) ||
                    _SurfaceTraitement.LargeurPixels > MaxPixelsJpeg || _SurfaceTraitement.HauteurPixels > MaxPixelsJpeg)
                {
                    //dans ce cas il n'y a pas de choix mais uniquement le format Bmp est possible
                    ListeChoixFormatCartes.SelectedIndex = -1;
                    ListeChoixFormatCartes.Enabled = false;
                    LabelChoixFormatCartes.Text = "Bmp";
                }
                else
                {
                    ListeChoixFormatCartes.Items.AddRange(new string[] { "Bmp", "Png", "Jpeg" });
                    //initialisation du format de la carte
                    ListeChoixFormatCartes.SelectedIndex = CartesSettings.FORMAT;
                }

                // initialisation de la demande de grille
                if (SysCartoEncours.AfficherGrilleIsOK)
                {
                    if (!SysCartoEncours.GrilleExiste)
                    {
                        IsGrille.Checked = CartesSettings.IS_GRILLE;
                    }
                    else
                    {
                        IsGrille.Checked = true;
                    }
                    IsGrille.Enabled = !SysCartoEncours.GrilleExiste;
                    IsReferences.Enabled = true;
                    IsReferences.Checked = CartesSettings.IS_REFERENCES; // initialisation de l'index à sélectionner
                }
                else
                {
                    IsGrille.Checked = false;
                    IsGrille.Enabled = false;
                    IsReferences.Checked = false;
                    IsReferences.Enabled = false;
                }

                // initialisation des facteurs de niveau d'affichage
                CartesSettings.FACTEUR_JNX = FacteursSettings.Lire().FacteurJNX;
                CartesSettings.FACTEUR_ORUX = FacteursSettings.Lire().FacteurORUX;
                RemplirFacteur();

                // initialisation des tuiles
                DimTuilesLarg.Value = CartesSettings.DIMENSIONS_TUILE;

                // initialisation des chemins d'enregistrement, pour être sur de ne pas avoir d'erreur à cause du chemin on en rajoute un par defaut
                RepertoireCartes.Text = CapturerSettings.CHEMIN_CARTE;
                RepertoireTuiles.Text = PartagerSettings.CHEMIN_TUILE;
                // pour obliger à saisir un nouveau nom
                NomCarte.Text = "";
                // mettre à jour les listes choix georef et choix ref
                ListeChoixGeoRefs.Items.AddRange(new string[] { "Aucun", "Georef Carte", "MapInfo Carte", "OziExploreur Carte", "Compeland Carte", "QGIS Carte" });
                ListeChoixGeoRefs.SelectedIndex = CartesSettings.GEOREF; // initialisation de l'index à sélectionner
                ChercheChemin = new FolderBrowserDialog();

                NomCarte.Select();
                {
                    ref var withBlock = ref NomCarte;
                    Cursor.Position = new Point(Location.X + withBlock.Bounds.X + withBlock.Bounds.Width / 2, Location.Y + withBlock.Location.Y + withBlock.Size.Height + 16);
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "I0S9");
                Close();
            }
            // et limite les déplacements de la souris au formulaire
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary>vérifie que le nom de la carte est saisi et d'initialiser la structure captures et l'enregistrement des settings</summary>
        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            // si on continue la carte en appuyant sur le 'bouton lance capture', on initialise les différentes variables après s'être assuré que le nom de la carte a été saisi
            if (DialogResult == DialogResult.OK)
            {
                if (!VerifierSaisies())
                {
                    // on annule la fermeture du formulaire
                    e.Cancel = true;
                }
            }
        }
        /// <summary> ferme la boite de dialogue ave un validation éventuelle du choix </summary>
        private void Preferences_FormClosed(object sender, FormClosedEventArgs e)
        {
            // si on continue la carte en appuyant sur le 'bouton lance capture', on initialise les différentes variables après s'être assuré que le nom de la carte a été saisi
            if (DialogResult == DialogResult.OK)
            {
                try
                {
                    // on enregistre les variables du settings pour la prochaine carte hormis le nom que l'on passe en paramètre
                    if (SysCartoEncours.AfficherGrilleIsOK)
                    {
                        CartesSettings.IS_GRILLE = IsGrille.Checked;
                        CartesSettings.IS_REFERENCES = IsReferences.Checked;
                    }
                    if (IsTrace.Enabled)
                        CartesSettings.IS_TRACE = IsTrace.Checked;
                    CartesSettings.FACTEUR_JNX = (double)FactJNX.Value;
                    CartesSettings.FACTEUR_ORUX = (int)Math.Round(FactORUX.Value);
                    CartesSettings.FICHIER_TUILE = IndexFichiersTuiles;
                    CartesSettings.DIMENSIONS_TUILE = (int)Math.Round(DimTuilesLarg.Value);
                    CartesSettings.GEOREF = ListeChoixGeoRefs.SelectedIndex;
                    //on ne prend en compte que le choix effectif de l'utilisateur
                    if (ListeChoixFormatCartes.SelectedIndex > -1)
                    {
                        CartesSettings.FORMAT = ListeChoixFormatCartes.SelectedIndex;
                    }
                    CartesSettings.FORMAT = ListeChoixFormatCartes.SelectedIndex;
                    CartesSettings.Ecrire();
                    CapturerSettings.CHEMIN_CARTE = RepertoireCartes.Text;
                    CapturerSettings.Ecrire();
                    PartagerSettings.CHEMIN_TUILE = RepertoireTuiles.Text;
                    PartagerSettings.Ecrire();
                    FacteursSettings.Ecrire();
                    Tag = NomCarte.Text; // & If(IsTrace.Enabled AndAlso IsTrace.Checked, "_Trace", "")
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "V9P9");
                }
            }
            Cursor.Clip = StockClipCurseur;
            TitreInformation = Titre;
        }
        /// <summary>sélectionne le chemin pour l'enrgistrement des 2 fichiers représentant une carte ou des tuiles </summary>
        private void Repertoires_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            Cursor.Clip = StockClipCurseur;
            try
            {
                ChercheChemin.Description = "Chemin d'enregistrement des " + Convert.ToString(L.Tag);
                ChercheChemin.InitialDirectory = L.Text;
                ChercheChemin.UseDescriptionForTitle = true;
                ChercheChemin.ShowNewFolderButton = false;
                if (ChercheChemin.ShowDialog(this) == DialogResult.OK)
                {
                    if (Directory.Exists(ChercheChemin.SelectedPath))
                        L.Text = ChercheChemin.SelectedPath;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "V7P8");
            }
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> verifie les différentes saisies avant de valider la fermeture du fomulaire avec acceptation </summary>
        private bool VerifierSaisies()
        {
            if (string.IsNullOrEmpty(NomCarte.Text + ""))
            {
                MessageInformation = "Vous devez indiquer un nom pour la carte";
                AfficherInformation();
                NomCarte.Focus();
                return false;
            }
            if (!Directory.Exists(RepertoireCartes.Text))
            {
                MessageInformation = "Vous devez indiquer répertoire existant pour" + CrLf + "l'enregistrement de la carte";
                AfficherInformation();
                RepertoireCartes.ForeColor = Color.Red;
                // ChoixCheminCartes.Focus()
                return false;
            }
            if (!Directory.Exists(RepertoireTuiles.Text))
            {
                MessageInformation = "Vous devez indiquer répertoire existant pour" + CrLf + "l'enregistrement des fichiers Tuiles";
                AfficherInformation();
                RepertoireTuiles.ForeColor = Color.Red;
                // ChoixCheminTuile.Focus()
                return false;
            }
            if (ListeChoixGeoRefs.SelectedIndex == 0 & IsReferences.Checked == false & IndexFichiersTuiles == 0)
            {
                MessageInformation = "Vous n'avez pas demandé de création de fichier après la capture";
                AfficherInformation();
                return false;
            }
            return true;
        }
        /// <summary> met à jour la variable du choix des fichiers tuiles en fonction du radio button coché </summary>
        private void Tuiles_CheckedChanged(object sender, EventArgs e)
        {
            char Ind = ((RadioButton)sender).Name[7];
            IndexFichiersTuiles = Convert.ToInt32(Ind) - 48;
        }
        /// <summary> limite les valeurs possibles de corrections à + ou - 3 par rapport à la valeur par défaut </summary>
        private void RemplirFacteur()
        {
            FactORUX.Minimum = SysCartoEncours.Niveau.NiveauAffichageORUX - 3;
            FactORUX.Maximum = SysCartoEncours.Niveau.NiveauAffichageORUX + 3;
            FactJNX.Minimum = (decimal)(SysCartoEncours.Niveau.NiveauAffichageJNX - 3d);
            FactJNX.Maximum = (decimal)(SysCartoEncours.Niveau.NiveauAffichageJNX + 3d);

            FactJNX.Value = (decimal)CartesSettings.FACTEUR_JNX;
            if (FactJNX.Value < FactJNX.Minimum)
                FactJNX.Value = FactJNX.Minimum;
            if (FactJNX.Value > FactJNX.Maximum)
                FactJNX.Value = FactJNX.Maximum;

            FactORUX.Value = CartesSettings.FACTEUR_ORUX;
            if (FactORUX.Value < FactORUX.Minimum)
                FactORUX.Value = FactORUX.Minimum;
            if (FactORUX.Value > FactORUX.Maximum)
                FactORUX.Value = FactORUX.Maximum;
        }
        /// <summary> indique d'une couleur différente la valeur par défaut des autres   </summary>
        private void FactORUX_ValueChanged(object sender, EventArgs e)
        {
            if (FactORUX.Value == SysCartoEncours.Niveau.NiveauAffichageORUX)
            {
                FactORUX.ForeColor = Color.YellowGreen;
            }
            else
            {
                FactORUX.ForeColor = Color.Tomato;
            }
        }
        /// <summary> indique d'une couleur différente la valeur par défaut des autres </summary>
        private void FactJNX_ValueChanged(object sender, EventArgs e)
        {
            if ((double)FactJNX.Value == SysCartoEncours.Niveau.NiveauAffichageJNX)
            {
                FactJNX.ForeColor = Color.YellowGreen;
            }
            else
            {
                FactJNX.ForeColor = Color.Tomato;
            }
        }
        /// <summary> Gère IsReference en fonction du choix IsGrille </summary>
        private void IsGrille_CheckedChanged(object sender, EventArgs e)
        {
            if (IsGrille.Checked == false)
            {
                IsReferences.Checked = false;
                IsReferences.Enabled = false;
            }
            else
            {
                IsReferences.Enabled = true;
            }
        }
        /// <summary> enlève la possibilité du choix 768</summary>
        private void DimTuilesLarg_ValueChanged(object sender, EventArgs e)
        {
            if (DimTuilesLarg.Value == 768m)
            {
                if (DimensionsTuile > 768)
                {
                    DimTuilesLarg.Value = 512m;
                }
                else
                {
                    DimTuilesLarg.Value = 1024m;
                }
            }
            else
            {
                DimensionsTuile = (int)Math.Round(DimTuilesLarg.Value);
                AutoriserFichiersTuiles();
            }
        }
        /// <summary> ferme les listes déroulantes </summary>
        private void Listes_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox C = (ComboBox)sender;
            C.DroppedDown = false;
            C.Visible = false;
        }
        /// <summary> ouvre les listes déroulantes </summary>
        private void Labels_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            ComboBox C = (ComboBox)L.Tag;
            if (C.Items.Count > 1)
            {
                C.DroppedDown = true;
                C.Show();
            }
        }
        /// <summary> Met à jour le texte du label associé à la liste </summary>
        private void Listes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox C = (ComboBox)sender;
            Label L = (Label)C.Tag;
            L.Text = C.SelectedItem.ToString();
        }

        private void AutoriserFichiersTuiles()
        {
            // initialisation des fichiers tuiles
            bool NotIsJnxKmz = IsMaxFichierTuiles_X(true); // interpolation SM
            bool NotIsOrux = IsMaxFichierTuiles_X(false);    // interpolation GF ou pas d'interpolation
            if (NotIsOrux)
            {
                // on interdit tous les fichiers tuiles car le nb de tuiles sur l'axe des x est trop grand
                Tuiles_0.Enabled = true;
                Tuiles_1.Enabled = false; // Orux
                Tuiles_2.Enabled = false; // Jnx
                Tuiles_4.Enabled = false; // Kmz
                Tuiles_7.Enabled = false; // Tous
                Tuiles_0.Checked = true;
            }
            else if (NotIsJnxKmz && CapturerSettings.SITE == SitesCartographiques.SuisseMobile)
            {
                // on choisit par défaut pas de fichiers tuiles
                if (CartesSettings.FICHIER_TUILE == (int)ChoixFichiersTuiles.Aucun || CartesSettings.FICHIER_TUILE > (int)ChoixFichiersTuiles.ORUX)
                {
                    Tuiles_1.Checked = true;
                }
                else
                {
                    CartesSettings.FICHIER_TUILE = (int)ChoixFichiersTuiles.Aucun;
                    Tuiles_0.Checked = true;
                }
                // on interdit les fichiers JNX ou KMZ car le nb de tuiles sur l'axe des x est trop grand. Ne concerne que SM
                Tuiles_0.Enabled = true;
                Tuiles_1.Enabled = true; // Orux
                Tuiles_2.Enabled = false; // Jnx
                Tuiles_4.Enabled = false; // Kmz
                Tuiles_7.Enabled = false; // Tous
            }
            else // Pas de limites
            {
                Tuiles_0.Enabled = true;
                Tuiles_1.Enabled = true; // Orux
                Tuiles_2.Enabled = true; // Jnx
                Tuiles_4.Enabled = true; // Kmz
                Tuiles_7.Enabled = true; // Tous
                ((RadioButton)GroupeReferences.Controls["Tuiles_" + CartesSettings.FICHIER_TUILE]).Checked = true;
            }
        }
        /// <summary> le nb de colonnes des fichiers tuiles a une limites qui est fonction de la taille du tampon, de la hauteur des tuiles et si
        /// il s'agit d'une interpolation liée au site SuisseMobile.
        /// Soit par la taille du tampon et par la hauteur des tuiles pour les sites web mercator et suisse Mobiles non interpolées.
        /// Soit par la taille du tampon et un nombre définit empiriquement pour suisse Mobile et une interpolation. </summary>
        private bool IsMaxFichierTuiles_X(bool FlagSM_Interpol)
        {
            int Rapport = DimensionsTuile / 256;
            int NbTuilesFichierX = _SurfaceTraitement.NbColonnes / Rapport;
            if (FlagSM_Interpol)
            {
                // constantes empiriques pour des tuiles de 256*256
                int NbTuiles = NbTuilesMaxCarteSuisseInterpol[(int)TypeSupportCarte.Fichier] / Rapport;
                return NbTuilesFichierX > NbTuiles;
            }
            else if (CartesSettings.SUPPORT_CARTE != TypeSupportCarte.Fichier)
            {
                return NbTuilesFichierX > TelechargementCarte.NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, Rapport * Rapport);
            }
            else
            {
                // dans le cas d'un support de fichier on est toujours bon car la vérification sur les X est faite
                // au niveau de la surface de capture
                return false;
            }
        }
    }
}