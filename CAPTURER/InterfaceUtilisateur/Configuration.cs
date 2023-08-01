using static FCGP.AfficheInformation;
using static FCGP.Altitudes;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> permet la configuration de FCGP_Capturer </summary>
    internal partial class Configuration : Form
    {

        private bool FlagIsToutFormat;
        private FolderBrowserDialog ChercheChemin;
        private Rectangle StockClipCurseur;
        private string Titre;
        /// <summary> initialisation des différentes variables internes en fonction des settings </summary>
        private void Configuration_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            // la mise à jour de l'index génère le calcul des dimensions min-max de capture
            foreach (Screen S in Screen.AllScreens)
                ListeChoixEcranTravail.Items.Add(S.DeviceName + ", " + S.Bounds.Width.ToString() + "*" + S.Bounds.Height);
            // MAJ champs dépendants de CarteSettings
            FlagIsToutFormat = CartesSettings.IS_TOUT_FORMAT;
            ListeChoixSupportCarte.SelectedIndex = CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier ? 1 : 0;
            // MAJ champs dépendants de CapturerSettings
            RepertoireCachesTuiles.Text = CapturerSettings.CHEMIN_CACHE;
            // met à jour les champs qui dépendent du settings commun
            ListeChoixEcranTravail.SelectedIndex = PartagerSettings.NUM_ECRAN;
            CouleurAffichage.BackColor = PartagerSettings.COULEUR_VISUE;
            CouleurCartes.BackColor = PartagerSettings.COULEUR_CARTE;
            QualiteJpeg.Value = PartagerSettings.QUALITE_JPEG;
            // MAJ champs dépendants du AltitudeSettings
            ID.Text = AltitudesSettings.ID;
            MP.Text = AltitudesSettings.MP;
            RepertoireFichiersAltitudes.Text = AltitudesSettings.CHEMIN_DEM;
            AvecAltitudes.Checked = AltitudesSettings.IS_ALTITUDE;
            DEM1.Checked = AltitudesSettings.TYPE_DEM == 1;
            MSGAltitudes.Checked = AltitudesSettings.IS_MSG;
            VerifierID.Text = AltitudesSettings.IS_VALIDE ? "OK" : "?";
            VerifierID.BackColor = AltitudesSettings.IS_VALIDE ? Color.YellowGreen : Color.Transparent;
            VerifierID.Enabled = VerifierID.BackColor != Color.YellowGreen;
            PanelAltitudes.Enabled = AvecAltitudes.Checked;
            // MAJ champs dépendants de TraceSettings
            RepertoireTraces.Text = TracesSettings.CHEMIN_TRACE;
            CouleurTraces.BackColor = TracesSettings.SEG_TRK;
            CouleurDessinTraces.BackColor = TracesSettings.COULEUR_TRACE;
            CoefAlphaTraces.Value = ValeurToPourcent(CouleurDessinTraces.BackColor.A);
            EpaisseurTraces.Value = TracesSettings.EPAISSEUR_TRACE;

            // positionnement du curseur de souris au milieu du bouton ok
            {
                ref var withBlock = ref BoutonEnter;
                Cursor.Position = new Point(Location.X + withBlock.Bounds.X + withBlock.Bounds.Width / 2, Location.Y + withBlock.Location.Y + withBlock.Size.Height / 2);
            }
            ChercheChemin = new FolderBrowserDialog();
            // et limite les déplacements de la souris au formulaire
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> ferme la boite de dialogue ave un validation éventuelle du choix </summary>
        private void Configuration_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                // MAJ de TraceSettings
                TracesSettings.CHEMIN_TRACE = RepertoireTraces.Text;
                TracesSettings.SEG_TRK = CouleurTraces.BackColor;
                TracesSettings.COULEUR_TRACE = CouleurDessinTraces.BackColor;
                TracesSettings.EPAISSEUR_TRACE = (int)Math.Round(EpaisseurTraces.Value);
                TracesSettings.Ecrire();
                // MAJ de AltitudeSettings
                AltitudesSettings.IS_ALTITUDE = AvecAltitudes.Checked;
                AltitudesSettings.IS_MSG = MSGAltitudes.Checked;
                if (AvecAltitudes.Checked)
                {
                    // on garde l'existant pour voir si il y a un changement
                    string RepAncien = AltitudesSettings.CHEMIN_DEM;
                    AltitudesSettings.TYPE_DEM = DEM1.Checked ? 1 : 3;
                    AltitudesSettings.CHEMIN_DEM = RepertoireFichiersAltitudes.Text;
                    // le répertoire a changé ou la liste n'a pas été initialisée il faut réinitialiser la liste des fichiers d'altitude
                    if ((AltitudesSettings.CHEMIN_DEM ?? "") != (RepAncien ?? "") || !IsOkFichiersAltitudes)
                    {
                        InitialiserListeFichiersAltitudes();
                    }
                    AltitudesSettings.ID = ID.Text;
                    AltitudesSettings.MP = MP.Text;
                    AltitudesSettings.IS_VALIDE = true;
                    // La connexion avec le serveur n'est pas établie, il faut la créer
                    if (!IsConnexion)
                    {
                        InitialiserConnexionAltitudes();
                    }
                }
                else if (VerifierID.Text != "OK")
                {
                    // on met ou remet les identifiants à zéro et on ferme la connexion avec le serveur 
                    AltitudesSettings.ID = "";
                    AltitudesSettings.MP = "";
                    AltitudesSettings.IS_VALIDE = false;
                    InitialiserConnexionAltitudes();
                }
                AltitudesSettings.Ecrire();
                // MAJ de CartesSettings
                CartesSettings.SUPPORT_CARTE = (TypeSupportCarte)ListeChoixSupportCarte.SelectedIndex;
                if (CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier)
                    CartesSettings.IS_TOUT_FORMAT = FlagIsToutFormat;
                CartesSettings.Ecrire();
                // MAJ de PartagerSettings
                PartagerSettings.NUM_ECRAN = ListeChoixEcranTravail.SelectedIndex;
                PartagerSettings.COULEUR_VISUE = CouleurAffichage.BackColor;
                PartagerSettings.COULEUR_CARTE = CouleurCartes.BackColor;
                PartagerSettings.QUALITE_JPEG = (int)Math.Round(QualiteJpeg.Value);
                PartagerSettings.Ecrire();
                // MAJ de CaptureSettings
                string RepNouveau = RepertoireCachesTuiles.Text;
                if ((RepNouveau ?? "") != (CapturerSettings.CHEMIN_CACHE ?? "")) // le répertoire a changé il faut réinitialise
                {
                    Cursor = Cursors.WaitCursor;
                    if (Serveur.Cache.ChangerRepertoireCaches(RepNouveau))
                    {
                        CapturerSettings.CHEMIN_CACHE = RepNouveau;
                        CapturerSettings.Ecrire();
                    }
                }
            }
            Cursor.Clip = StockClipCurseur;
            TitreInformation = Titre;
        }
        private void Configuration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (AvecAltitudes.Checked & VerifierID.Text != "OK")
                {
                    MessageInformation = "Vous devez renseigner les identifiants" + CrLf + "et les vérifier.";
                    AfficherInformation();
                    e.Cancel = true;
                }
            }
        }
        // ''' <summary> met à jour la variable globale en fonction de la case "ToutFormat" </summary>
        private void IsToutFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (ListeChoixSupportCarte.SelectedIndex == 1)
            {
                FlagIsToutFormat = IsToutFormat.Checked;
            }
        }
        /// <summary> met ajour la couleur de fond du label en fonction d'un choix de couleur et du % de transparence </summary>
        private void Couleurs_Click(object sender, EventArgs e)
        {
            Cursor.Clip = StockClipCurseur;
            Label L = (Label)sender;
            bool FlagTransparent = Convert.ToBoolean(L.Tag);
            using (var C = new ColorDialog())
            {
                C.Color = L.BackColor;
                if (C.ShowDialog(this) == DialogResult.OK)
                {
                    if (FlagTransparent)
                    {
                        L.BackColor = Color.FromArgb(PourcentToValeur((int)Math.Round(CoefAlphaTraces.Value)), C.Color);
                    }
                    else
                    {
                        L.BackColor = C.Color;
                    }
                }
            }
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> permet de choisir si on veut voir afficher les altitudes des coordonnées du pointeur de souris </summary>
        private void AvecAltitude_CheckedChanged(object sender, EventArgs e)
        {
            PanelAltitudes.Enabled = AvecAltitudes.Checked;
        }
        /// <summary> permet de modifier le répertoire d'enregistrement des caches tuiles, des fichiers d'altitudes et des traces </summary>
        private void Repertoires_Click(object sender, EventArgs e)
        {
            Cursor.Clip = StockClipCurseur;
            Label L = (Label)sender;
            if (L.Name == "RepertoireCacheTuiles")
            {
                MessageInformation = "Si le cache tuiles du site n'existe pas dans le nouveau répertoire" + CrLf + "un cache tuile vide sera créeé. Voulez vous continuer ?";
                if (AfficherConfirmation() == DialogResult.Cancel)
                    return;
            }
            try
            {
                ChercheChemin.Description = "Répertoire d'enregistrement des " + Convert.ToString(L.Tag);
                ChercheChemin.InitialDirectory = L.Text;
                ChercheChemin.UseDescriptionForTitle = true;
                if (ChercheChemin.ShowDialog(this) == DialogResult.OK)
                {
                    if (Directory.Exists(ChercheChemin.SelectedPath))
                        L.Text = ChercheChemin.SelectedPath;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "Y8X2");
            }
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> tranforme un % transparence de 0 à 100 à une valeur du canal alpha de couleur de 255 à 0 </summary>
        /// <param name="Trans"> % transparence </param>
        private static int PourcentToValeur(int Trans)
        {
            return 255 - 255 * Trans / 100;
        }
        /// <summary> transforme une valeur du canal alpha de couleur de 255 à 0 en un % transparence de 0 à 100 </summary>
        /// <param name="Valeur"> valeur du canal alpha </param>
        private static int ValeurToPourcent(byte Valeur)
        {
            return 100 - Valeur * 100 / 255;
        }
        /// <summary> met à jour la couleur du label couleur en fonction du pourcentage de transparence désiré par l'utilisateur </summary>
        private void TransparenceTrace_ValueChanged(object sender, EventArgs e)
        {
            CouleurDessinTraces.BackColor = Color.FromArgb(PourcentToValeur((int)Math.Round(CoefAlphaTraces.Value)), CouleurDessinTraces.BackColor);
        }
        /// <summary> demande la vérification des identifiants du serveur de la NASA </summary>
        private void VerifierID_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ID.Text) || string.IsNullOrEmpty(MP.Text))
            {
                MessageInformation = "Vous devez renseigner les 2 identifiants" + CrLf + "avant de pouvoir demander la vérification.";
            }
            else if (IsConnected)
            {
                if (VerifierIDAltitudes(ID.Text, MP.Text) == DialogResult.OK)
                {
                    MessageInformation = "Identifiants valides";
                    VerifierID.Text = "OK";
                    VerifierID.BackColor = Color.YellowGreen;
                    VerifierID.Enabled = false;
                }
                else
                {
                    MessageInformation = "Identifiants non valides";
                    VerifierID.Text = "Not" + CrLf + "OK";
                    VerifierID.BackColor = Color.Tomato;
                    VerifierID.Enabled = true;
                }
            }
            else
            {
                MessageInformation = "La connexion internet n'est pas établie." + CrLf + "La vérification des identifiants est impossible";
            }
            AfficherInformation();
        }
        /// <summary> invalide Verifier ID pour que l'on oblige l'utiliseteur à vérifier les identifiants </summary>
        private void IDs_TextChanged(object sender, EventArgs e)
        {
            VerifierID.Text = "?";
            VerifierID.BackColor = Color.Transparent;
            VerifierID.Enabled = true;
        }
        /// <summary>Affiche le support de carte et autorise la case "ToutFormat" en fonction du support de carte ou affiche l'écran sélectioné </summary>
        private void Listes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox C = (ComboBox)sender;
            Label L = (Label)C.Tag;
            if (C.Name == "ListeChoixSupportCarte")
            {
                if (ListeChoixSupportCarte.SelectedIndex == 0)
                {
                    IsToutFormat.Checked = true;
                    IsToutFormat.Enabled = false;
                }
                else
                {
                    IsToutFormat.Checked = FlagIsToutFormat;
                    IsToutFormat.Enabled = true;
                }
            }
            L.Text = C.SelectedItem.ToString();
        }
        /// <summary> ouvre les listes déroulantes </summary>
        private void Labels_Click(object sender, EventArgs e)
        {
            Label L = (Label)sender;
            ComboBox C = (ComboBox)L.Tag;
            C.DroppedDown = true;
            C.Show();
        }
    }
}