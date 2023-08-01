using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.FichierCarteOrux;
using static FCGP.NiveauDetailCartographique;
using static FCGP.Settings;

namespace FCGP
{

    /// <summary> permet la modification des niveaux d'affichage dans 1 fichier ORUX </summary>
    internal partial class AffichageORUX
    {
        private readonly int[] NiveauORUXModifie = new int[5], NiveauORUX = new int[5], ValeursDefaut = new int[5], IndicesEchelle = new int[5];
        private readonly string[] ClefNiveauORUX = new string[5];
        private readonly double[] NiveauJNX = new double[5];
        private string FichierORUX;
        private int NbNiveauxDetail;
        private InfoORUX F;
        private int IndiceSiteORUX;
        private string Titre;

        private void AffichageORUX_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            TitreInformation = "Information modification fichier Orux";
            ChercheFichierORUX.Filter = "ORUX Files|*.otrk2.xml";
            ChercheFichierORUX.Title = "Choix du fichier ORUX";
            ChercheFichierORUX.InitialDirectory = PartagerSettings.CHEMIN_TUILE;
        }
        private void AffichageORUX_FormClosed(object sender, FormClosedEventArgs e)
        {
            TitreInformation = Titre;
        }
        private void ChoixFichierORUX_Click(object sender, EventArgs e)
        {
            if (ChercheFichierORUX.ShowDialog(this) == DialogResult.OK)
            {
                FichierORUX = ChercheFichierORUX.FileName;
                F = VerifierCarteORUX(FichierORUX);
                if (F.NbNiveauxDetail == -1)
                {
                    MessageInformation = "Le fichier ORUX est corrompu ou n'a pas été créé" + CrLf +
                                         "avec une version FCGP Capture N°5 ou supérieure ou FCGP Visue";
                    AfficherInformation();
                }
                else
                {
                    ChercheFichierORUX.InitialDirectory = Path.GetDirectoryName(FichierORUX);
                    ChercheFichierORUX.FileName = null;
                    IndiceSiteORUX = FCGP.SystemeCartographique.SiteDomTomToIndex(F.SiteCapture, F.DomTom);
                    InitialiserPanel();
                }
            }
        }
        private void Appliquer_Click(object sender, EventArgs e)
        {
            if (!VerifierValidation())
                return;
            var DebutTraitement = DateTime.Now;
            Appliquer.Enabled = false;
            ChoixFichierORUX.Enabled = false;
            Quitter.Enabled = false;
            Cursor = Cursors.WaitCursor;
            if (SensMaxMin())
            {
                for (int Cpt = NbNiveauxDetail - 1; Cpt >= 0; Cpt -= 1)
                {
                    if (NiveauORUXModifie[Cpt] != NiveauORUX[Cpt])
                    {
                        ChangerNiveauAffichageCarteOrux(FichierORUX, NbNiveauxDetail - Cpt - 1, NiveauORUXModifie[Cpt]);
                        FacteursSettings.Ecrire(IndiceSiteORUX, IndicesEchelle[Cpt], NiveauJNX[Cpt], NiveauORUXModifie[Cpt]);
                    }
                }
            }
            else
            {
                for (int Cpt = 0, loopTo = NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
                {
                    if (NiveauORUXModifie[Cpt] != NiveauORUX[Cpt])
                    {
                        ChangerNiveauAffichageCarteOrux(FichierORUX, NbNiveauxDetail - Cpt - 1, NiveauORUXModifie[Cpt]);
                        FacteursSettings.Ecrire(IndiceSiteORUX, IndicesEchelle[Cpt], NiveauJNX[Cpt], NiveauORUXModifie[Cpt]);
                    }
                }
            }
            TitreInformation = "Information Regrouper";
            MessageInformation = $"Le fichier {Path.GetFileNameWithoutExtension(FichierORUX)} a été modifié.{CrLf}Temps de traitement : {TimeSpanToStr(DateTime.Now - DebutTraitement, true)}";
            AfficherInformation();
            Cursor = Cursors.Default;
            RAZPanel();
        }
        private bool SensMaxMin()
        {
            for (int Cpt = NbNiveauxDetail - 1; Cpt >= 0; Cpt -= 1)
            {
                if (NiveauORUX.Contains(NiveauORUXModifie[Cpt]))
                {
                    for (int CptInf = Cpt - 1; CptInf >= 0; CptInf -= 1)
                    {
                        if (NiveauORUX[CptInf] == NiveauORUXModifie[Cpt])
                            return false;
                    }
                }
            }
            return true;
        }
        private void RAZPanel()
        {
            // initialise les champs à zéro
            for (int Cpt = 0; Cpt <= 4; Cpt++)
            {
                Panel Pp = (Panel)Support.Controls["Echelle" + Cpt];
                Pp.Controls["Clef" + Cpt].Text = "";
                Pp.Controls["ZoomORUX" + Cpt].Text = "";
                NumericUpDown SaisieValeurModif = (NumericUpDown)Pp.Controls["CorZoomORUX" + Cpt];
                SaisieValeurModif.Minimum = 0m;
                SaisieValeurModif.Value = 0m;
                SaisieValeurModif.ForeColor = SystemColors.ControlText;
            }
            Appliquer.Enabled = false;
            ChoixFichierORUX.Enabled = true;
            Quitter.Enabled = true;
        }
        private void InitialiserPanel()
        {
            NbNiveauxDetail = F.NbNiveauxDetail;
            int[] FacteursORUX = FacteursORUXSite(F.SiteCapture, F.DomTom);
            // remplissage des champs avec les valers liées au fichier
            for (int Cpt = NbNiveauxDetail - 1; Cpt >= 0; Cpt -= 1)
            {
                int IndiceNiveau = NbNiveauxDetail - Cpt - 1;
                Panel Pp = (Panel)Support.Controls["Echelle" + IndiceNiveau];
                Pp.Enabled = true;
                ClefNiveauORUX[IndiceNiveau] = F.NiveauDetail[Cpt];
                Pp.Controls["Clef" + IndiceNiveau].Text = ClefNiveauORUX[IndiceNiveau];

                ValeursDefaut[IndiceNiveau] = FacteursORUX[F.IndiceEchelleCapture[Cpt]];
                IndicesEchelle[IndiceNiveau] = F.IndiceEchelleCapture[Cpt];
                NiveauORUX[IndiceNiveau] = F.IndiceAffichage[Cpt];
                var AfficheValeurActuelle = Pp.Controls["ZoomORUX" + IndiceNiveau];
                AfficheValeurActuelle.Text = NiveauORUX[IndiceNiveau].ToString("0");
                if (NiveauORUX[IndiceNiveau] == ValeursDefaut[IndiceNiveau])
                {
                    AfficheValeurActuelle.ForeColor = Color.YellowGreen;
                }
                else
                {
                    AfficheValeurActuelle.ForeColor = Color.Tomato;
                }
                NumericUpDown SaisieValeurModif = (NumericUpDown)Pp.Controls["CorZoomORUX" + IndiceNiveau];
                SaisieValeurModif.Minimum = ValeursDefaut[IndiceNiveau] - 3;
                SaisieValeurModif.Maximum = ValeursDefaut[IndiceNiveau] + 3;
                NiveauJNX[IndiceNiveau] = FacteursSettings.Lire(IndiceSiteORUX, F.IndiceEchelleCapture[Cpt]).FacteurJNX;
                int ValeurActuelle = FacteursSettings.Lire(IndiceSiteORUX, F.IndiceEchelleCapture[Cpt]).FacteurORUX;
                if (ValeurActuelle > ValeursDefaut[IndiceNiveau] + 3)
                    ValeurActuelle = ValeursDefaut[IndiceNiveau] + 3;
                if (ValeurActuelle < ValeursDefaut[IndiceNiveau] - 3)
                    ValeurActuelle = ValeursDefaut[IndiceNiveau] - 3;
                SaisieValeurModif.Value = SaisieValeurModif.Minimum; // pour forcer l'évenement changed du control
                SaisieValeurModif.Value = ValeurActuelle;
            }
            Appliquer.Enabled = Is_Appliquer();
        }
        private void CorZoomORUX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown C = (NumericUpDown)sender;
            int IndiceNiveau = Convert.ToInt32(C.Tag);
            NiveauORUXModifie[IndiceNiveau] = (int)Math.Round(C.Value);
            if (NiveauORUXModifie[IndiceNiveau] == ValeursDefaut[IndiceNiveau])
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
                    if (NiveauORUXModifie[Cpt] != NiveauORUX[Cpt])
                        return true;
                }
            }
            return false;
        }

        private bool VerifierValidation()
        {
            double ValeurSupORUX = 50d;
            for (int Cpt = 0, loopTo = NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
            {
                if (NiveauORUXModifie[Cpt] < ValeurSupORUX)
                {
                    ValeurSupORUX = NiveauORUXModifie[Cpt];
                }
                else
                {
                    MessageInformation = "Le zoom corrigé du niveau de détail " + ClefNiveauORUX[Cpt] + " n'est pas inférieur" + CrLf + "à celui du niveau de détail précédent";
                    TitreInformation = "Information Fichier ORUX";
                    AfficherInformation();
                    return false;
                }
            }
            return true;
        }
    }
}