using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.NiveauDetailCartographique;
using static FCGP.Settings;
using static FCGP.StructuresJNX;
using static FCGP.SystemeCartographique;

namespace FCGP
{

    /// <summary> permet la modification des niveaux d'affichage dans 1 fichier ORUX </summary>
    internal partial class AffichageJNX
    {
        // variables container des champs de données du formulaires
        private readonly double[] NiveauJNXModifie = new double[5], NiveauJNX = new double[5], ValeursDefaut = new double[5];
        private readonly string[] ClefNiveauJNX = new string[5];
        private readonly int[] NiveauORUX = new int[5], IndicesEchelle = new int[5];
        private int NivAffichage;
        private int NivAffichageModifie;
        private string FichierJNX;
        private int NbNiveauxDetail;
        private InfoJNX F;
        private int IndiceSiteJNX;
        private string Titre;
        /// <summary> initialisation de la boite de dialogue d'ouverture des ficheirs JNX </summary>
        private void AffichageJNX_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            TitreInformation = "Information modification fichier Jnx";
            ChercheFichierJNX.Filter = "JNX Files|*.jnx";
            ChercheFichierJNX.Title = "Choix du fichier JNX";
            // valeur par défault du répertoire lors de la 1ère recherche de fichier
            ChercheFichierJNX.InitialDirectory = PartagerSettings.CHEMIN_TUILE;
        }
        private void AffichageJNX_FormClosed(object sender, FormClosedEventArgs e)
        {
            TitreInformation = Titre;
        }
        /// <summary> Choix du fichier à modifié </summary>
        private void ChoixFichierJNX_Click(object sender, EventArgs e)
        {
            if (ChercheFichierJNX.ShowDialog(this) == DialogResult.OK)
            {
                FichierJNX = ChercheFichierJNX.FileName;
                F = VerifierCarteJNX(FichierJNX);
                IndiceSiteJNX = SiteDomTomToIndex(F.SiteCapture, F.DomTom);

                if (F.NbNiveauxDetail == -1)
                {
                    MessageInformation = "Le fichier JNX est corrompu ou n'a pas été créé" + CrLf +
                                         "avec une version FCGP Capture N°5 ou supérieure ou FCGP Visue";
                    AfficherInformation();
                }
                else
                {
                    // on garde le répertoire choisi pour la prochaine recherche
                    ChercheFichierJNX.InitialDirectory = Path.GetDirectoryName(FichierJNX);
                    // on efface le fichier choisi pour obliger une nouvelle sélection
                    ChercheFichierJNX.FileName = null;
                    InitialiserPanel();
                }
            }
        }
        /// <summary> applique les modifications demandées aux indices d'affichage du fichier JNX </summary>
        private void Appliquer_Click(object sender, EventArgs e)
        {
            if (!VerifierValidation())
                return;
            var DebutTraitement = DateTime.Now;
            Cursor = Cursors.WaitCursor;
            Appliquer.Enabled = false;
            ChoixFichierJNX.Enabled = false;
            Quitter.Enabled = false;
            for (int Cpt = 0, loopTo = NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
            {
                if (NiveauJNXModifie[Cpt] != NiveauJNX[Cpt]) // si il y a une modification on change dans le fichier et dans la base
                {
                    ChangerNiveauAffichageCarteJNX(FichierJNX, NbNiveauxDetail - Cpt - 1, NiveauJNXModifie[Cpt]);
                    FacteursSettings.Ecrire(IndiceSiteJNX, IndicesEchelle[Cpt], NiveauJNXModifie[Cpt], NiveauORUX[Cpt]);
                }
            }
            if (NivAffichageModifie != NivAffichage)
            {
                ChangerOrdreAffichageCarteJNX(FichierJNX, NivAffichageModifie);
            }
            Cursor = Cursors.Default;
            MessageInformation = $"Le fichier {Path.GetFileNameWithoutExtension(FichierJNX)} a été modifié.{CrLf}Temps de traitement : {TimeSpanToStr(DateTime.Now - DebutTraitement, true)}";
            AfficherInformation();

            RAZPanel();
        }
        /// <summary> remise à zéro des informations concernant les différents niveaux de détails </summary>
        private void RAZPanel()
        {
            // initialise les champs à zéro
            for (int Cpt = 0; Cpt <= 4; Cpt++)
            {
                Panel Pp = (Panel)Support.Controls["Echelle" + Cpt];
                Pp.Controls["Clef" + Cpt].Text = "";
                Pp.Controls["ZoomJNX" + Cpt].Text = "";
                NumericUpDown SaisieValeurModif = (NumericUpDown)Pp.Controls["CorZoomJNX" + Cpt];
                SaisieValeurModif.Minimum = 0m;
                SaisieValeurModif.Value = 0m;
                SaisieValeurModif.ForeColor = SystemColors.ControlText;
            }
            Panel P = (Panel)Support.Controls["OrdreAffichage"];
            P.Controls["NiveauAffichage"].Text = "";
            ((NumericUpDown)P.Controls["CorNiveauAffichage"]).Value = 0m;
            Appliquer.Enabled = false;
            ChoixFichierJNX.Enabled = true;
            Quitter.Enabled = true;
        }
        /// <summary> initialise les information concernant les différents niveaux de détails d'un fichier JNX </summary>
        private void InitialiserPanel()
        {
            NbNiveauxDetail = F.NbNiveauxDetail;
            double[] FacteursJNXDefauts = FacteursJNXSite(F.SiteCapture, F.DomTom);
            // remplissage des champs avec les valeurs liées au fichier
            for (int Cpt = NbNiveauxDetail - 1; Cpt >= 0; Cpt -= 1)
            {
                int IndiceNiveau = NbNiveauxDetail - Cpt - 1;
                Panel Pp = (Panel)Support.Controls["Echelle" + IndiceNiveau];
                Pp.Enabled = true;
                ClefNiveauJNX[IndiceNiveau] = F.NiveauDetail[Cpt];
                IndicesEchelle[IndiceNiveau] = F.IndiceEchelleCapture[Cpt];
                Pp.Controls["Clef" + IndiceNiveau].Text = ClefNiveauJNX[IndiceNiveau];
                NiveauJNX[IndiceNiveau] = F.IndiceAffichage[Cpt];
                var AfficheValeurActuelle = Pp.Controls["ZoomJNX" + IndiceNiveau];
                AfficheValeurActuelle.Text = DblToStr(NiveauJNX[IndiceNiveau], PrecisionJNX);
                ValeursDefaut[IndiceNiveau] = FacteursJNXDefauts[F.IndiceEchelleCapture[Cpt]];
                if (NiveauJNX[IndiceNiveau] == ValeursDefaut[IndiceNiveau])
                {
                    AfficheValeurActuelle.ForeColor = Color.YellowGreen;
                }
                else
                {
                    AfficheValeurActuelle.ForeColor = Color.Tomato;
                }
                NumericUpDown SaisieValeurModif = (NumericUpDown)Pp.Controls["CorZoomJNX" + IndiceNiveau];
                SaisieValeurModif.Minimum = (decimal)(ValeursDefaut[IndiceNiveau] - 3d);
                SaisieValeurModif.Maximum = (decimal)(ValeursDefaut[IndiceNiveau] + 3d);
                double ValeurActuelle = FacteursSettings.Lire(IndiceSiteJNX, F.IndiceEchelleCapture[Cpt]).FacteurJNX;
                //pour pouvoir écrire le settings
                NiveauORUX[IndiceNiveau] = FacteursSettings.Lire(IndiceSiteJNX, F.IndiceEchelleCapture[Cpt]).FacteurORUX;
                if (ValeurActuelle > ValeursDefaut[IndiceNiveau] + 3d)
                    ValeurActuelle = ValeursDefaut[IndiceNiveau] + 3d;
                if (ValeurActuelle < ValeursDefaut[IndiceNiveau] - 3d)
                    ValeurActuelle = ValeursDefaut[IndiceNiveau] - 3d;
                SaisieValeurModif.Value = SaisieValeurModif.Minimum; // pour forcer l'évenement changed du control
                SaisieValeurModif.Value = (decimal)ValeurActuelle;
            }
            Panel P = (Panel)Support.Controls["OrdreAffichage"];
            P.Enabled = true;
            NivAffichage = F.OrdreAffichage;
            NivAffichageModifie = NivAffichage;
            P.Controls["NiveauAffichage"].Text = NivAffichage.ToString();
            ((NumericUpDown)P.Controls["CorNiveauAffichage"]).Value = (decimal)F.OrdreAffichage;
            Appliquer.Enabled = Is_Appliquer();
        }
        private void CorZoomJNX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            int IndiceNiveau = Convert.ToInt32(C.Tag);

            NiveauJNXModifie[IndiceNiveau] = (double)C.Value;

            if (NiveauJNXModifie[IndiceNiveau] == ValeursDefaut[IndiceNiveau])
            {
                C.ForeColor = Color.YellowGreen;
            }
            else
            {
                C.ForeColor = Color.Tomato;
            }
            Appliquer.Enabled = Is_Appliquer();
        }
        /// <summary> indique si il y a au moins une différence entre les valeurs actuelles et les valeurs modifiées</summary>
        private bool Is_Appliquer()
        {
            for (int Cpt = 0; Cpt <= 4; Cpt++)
            {
                if (Support.Controls["Echelle" + Cpt].Enabled)
                {
                    if (NiveauJNXModifie[Cpt] != NiveauJNX[Cpt])
                        return true;
                }
            }
            if (NivAffichageModifie != NivAffichage)
                return true;
            return false;
        }
        /// <summary> prend en compte la modification d'un niveau d'affichage </summary>
        private void CorNiveauAffichage_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            NivAffichageModifie = (int)Math.Round(C.Value);
            Appliquer.Enabled = Is_Appliquer();
        }
        /// <summary> Verifie que les niveaux d'affichages des différents niveau de détail soient cohérents entre eux </summary>
        private bool VerifierValidation()
        {
            double ValeurInfJNX = 0d;
            for (int Cpt = 0, loopTo = NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
            {
                if (NiveauJNXModifie[Cpt] > ValeurInfJNX)
                {
                    ValeurInfJNX = NiveauJNXModifie[Cpt];
                }
                else
                {
                    MessageInformation = $"Le zoom JNX du niveau de détail {ClefNiveauJNX[Cpt]} n'est pas supérieur {CrLf} au zoom JNX du niveau de détail précédent";
                    AfficherInformation();
                    return false;
                }
            }
            return true;
        }
    }
}