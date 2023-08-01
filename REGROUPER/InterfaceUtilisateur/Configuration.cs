using static FCGP.AfficheInformation;
using static FCGP.Altitudes;
using static FCGP.Commun;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> permet la configuration de l'application FCGP_Regrouper </summary>
    internal partial class Configuration : Form
    {
        private FolderBrowserDialog ChercheChemin;
        private string Titre;
        /// <summary> correspondance site --> radiobouton </summary>
        private void Configuration_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            TitreInformation = "Information Configuration";
            // la mise à jour de l'index génère le calcul des dimensions min-max de capture
            foreach (Screen S in Screen.AllScreens)
                ListeEcrans.Items.Add(S.DeviceName + ", " + S.Bounds.Width.ToString() + "*" + S.Bounds.Height);

            // met à jour les champs qui dépendent du settings commun
            ListeEcrans.SelectedIndex = PartagerSettings.NUM_ECRAN;
            CouleurFondAffichage.BackColor = PartagerSettings.COULEUR_VISUE;
            CouleurFondCarte.BackColor = PartagerSettings.COULEUR_CARTE;
            RepertoireTuiles.Text = PartagerSettings.CHEMIN_TUILE;
            // MAJ champs dépendants du AltitudeSettings
            ID.Text = AltitudesSettings.ID;
            MP.Text = AltitudesSettings.MP;
            RepertoireAltitudes.Text = AltitudesSettings.CHEMIN_DEM;
            AvecAltitude.Checked = AltitudesSettings.IS_ALTITUDE;
            DEM1.Checked = AltitudesSettings.TYPE_DEM == 1;
            MSGAltitude.Checked = AltitudesSettings.IS_MSG;
            VerifierID.Text = AltitudesSettings.IS_VALIDE ? "OK" : "?";
            VerifierID.BackColor = AltitudesSettings.IS_VALIDE ? Color.YellowGreen : Color.Transparent;
            VerifierID.Enabled = VerifierID.BackColor != Color.YellowGreen;
            PanelAltitude.Enabled = AvecAltitude.Checked;

            PasDeplacement.Value = (decimal)RegrouperSettings.PAS_DEPLACEMENT;

            // positionnement du curseur de souris au milieu du bouton ok
            {
                Cursor.Position = new Point(Location.X + Valider.Bounds.X + Valider.Bounds.Width / 2, Location.Y + Valider.Location.Y + Valider.Size.Height / 2);
            }
            ChercheChemin = new FolderBrowserDialog();
        }
        /// <summary> ferme la boite de dialogue ave un validation éventuelle du choix </summary>
        private void Configuration_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                // on met à jour le settings altitude
                AltitudesSettings.IS_ALTITUDE = AvecAltitude.Checked;
                AltitudesSettings.IS_MSG = MSGAltitude.Checked;
                if (AvecAltitude.Checked)
                {
                    // on garde l'existant pour voir si il y a un changement
                    string RepAncien = AltitudesSettings.CHEMIN_DEM;
                    AltitudesSettings.TYPE_DEM = DEM1.Checked ? 1 : 3;
                    AltitudesSettings.CHEMIN_DEM = RepertoireAltitudes.Text;
                    //le répertoire a changé ou la liste n'a pas été initialisée il faut réinitialiser la liste des fichiers d'altitude
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
                // on met à jour le settings commun
                PartagerSettings.NUM_ECRAN = ListeEcrans.SelectedIndex;
                PartagerSettings.COULEUR_VISUE = CouleurFondAffichage.BackColor;
                PartagerSettings.COULEUR_CARTE = CouleurFondCarte.BackColor;
                PartagerSettings.CHEMIN_TUILE = RepertoireTuiles.Text;
                PartagerSettings.Ecrire();
                // on met à jour le settings visue
                RegrouperSettings.PAS_DEPLACEMENT = (int)Math.Round(PasDeplacement.Value);
                RegrouperSettings.Ecrire();
            }
            TitreInformation = Titre;
        }
        /// <summary> vérifie que les identifiants pour le téléchargement des fichiers d'altitude ont été correctement saisis </summary>
        private void Configuration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (AvecAltitude.Checked & VerifierID.Text != "OK")
                {
                    MessageInformation = "Vous devez renseigner les identifiants et" + CrLf + "et les vérifier.";
                    AfficherInformation();
                    e.Cancel = true;
                }
            }
        }
        // ''' <summary> met à jour la couleur de fond de la visue </summary>
        private void Couleur_Click(object sender, EventArgs e)
        {
            using (var C = new ColorDialog())
            {
                Label Couleur = (Label)sender;
                C.Color = Couleur.BackColor;
                if (C.ShowDialog() == DialogResult.OK)
                {
                    Couleur.BackColor = C.Color;
                }
            }
        }
        /// <summary> permet de changer le repertoire d'enregistrement des fichiers tuiles ou des fichiers d'altitude </summary>
        private void RepertoireTuiles_Click(object sender, EventArgs e)
        {
            try
            {
                Label CheminFichiersInitial = (Label)sender;
                ChercheChemin.Description = Convert.ToString(CheminFichiersInitial.Tag);
                ChercheChemin.InitialDirectory = CheminFichiersInitial.Text;
                if (ChercheChemin.ShowDialog(this) == DialogResult.OK)
                {
                    if (Directory.Exists(ChercheChemin.SelectedPath))
                        CheminFichiersInitial.Text = ChercheChemin.SelectedPath;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "I8G0");
            }
        }
        /// <summary> permet de choisir si on veut voir afficher les altitudes des coordonnées du pointeur de souris </summary>
        private void AvecAltitude_CheckedChanged(object sender, EventArgs e)
        {
            PanelAltitude.Enabled = AvecAltitude.Checked;
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
        private void IDs_TextChanged(object sender, EventArgs e)
        {
            VerifierID.Text = "?";
            VerifierID.BackColor = Color.Transparent;
            VerifierID.Enabled = true;
        }
        private void ListeEcrans_DropDownClosed(object sender, EventArgs e)
        {
            ListeEcrans.Visible = false;
        }
        private void ListeEcrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            EcranTravail.Text = ListeEcrans.SelectedItem.ToString();
        }
        private void EcranTravail_Click(object sender, EventArgs e)
        {
            ListeEcrans.DroppedDown = true;
            ListeEcrans.Show();
        }
    }
}