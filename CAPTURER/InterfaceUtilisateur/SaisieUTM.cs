using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary> Saisie d'un PointD en format UTM </summary>
    internal partial class SaisieUTM
    {
        private int[] ListeZones;
        private RectangleD LimiteSite;
        private readonly TypeSaisie SaisieXY = TypeSaisie.EntierPositif;
        private Rectangle StockClipCurseur;
        private string Titre;
        /// <summary> initialisation de la location et des différents champs de saisie </summary>
        private void SaisieUTM_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            // Attend un pointD en DD
            PointD P = (PointD)Tag;
            ListeZones = new int[] { 20, 21, 22, 30, 31, 32, 38, 40 };
            if (!P.IsEmpty)
            {
                // on tranforme en UTM
                var UTM = ConvertWGS84ToProjection(P, Datums.UTM_WGS84);
                if (UTM.Hemisphere == 'N')
                {
                    Hem.SelectedIndex = 0;
                }
                else
                {
                    Hem.SelectedIndex = 1;
                }
                Zone.SelectedIndex = Array.IndexOf(ListeZones, UTM.Zone);
                CoordX.Text = UTM.X.ToString("#0");
                CoordX.Select(CoordX.Text.Length, 0);
                CoordY.Text = UTM.Y.ToString("#0");
                CoordY.Select(CoordY.Text.Length, 0);
            }
            else
            {
                Hem.SelectedIndex = 0;
                Zone.SelectedIndex = 4;
            }
            LimiteSite = RegionGrilleToRegionDD(Serveur.Limites, Serveur.SiteCarto);
            Cursor.Clip = new Rectangle(Location, new Size(Size.Width, Size.Height + 105));
        }
        /// <summary> filtrage des touches admises </summary>
        private void CoordXY_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = SuppressionTouche(SaisieXY, e.KeyCode);
        }
        /// <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
        private void CoordXY_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox C = (TextBox)sender;
            e.Handled = SuppressionCaractere(SaisieXY, C, e.KeyChar);
        }
        /// <summary> affiche un message d'erreur </summary>
        private static void Erreur(string Champ, int Min, int Max)
        {
            MessageInformation = $"Le champ {Champ} doit être compris" + CrLf + $"entre {Min} et {Max}";
            TitreInformation = "Erreur de saisie";
            AfficherInformation();
        }
        /// <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
        private void SaisieUTM_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(CoordX.Text))
                {
                    e.Cancel = true;
                    Erreur("X", 0, 1000000);
                    CoordX.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(CoordY.Text))
                {
                    e.Cancel = true;
                    Erreur("Y", 0, 10000000);
                    CoordY.Focus();
                    return;
                }
                var UTM = new PointProjection(new PointD(int.Parse(CoordX.Text), int.Parse(CoordY.Text)), ListeZones[Zone.SelectedIndex], Convert.ToChar(Hem.SelectedItem));
                var Result = ConvertProjectionToWGS84(Datums.UTM_WGS84, UTM);
                // Logic point saisi en dehors des limites
                if (FlagLimitesSite && !LimiteSite.CoordonneesContains(Result))
                {
                    e.Cancel = true;
                    MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointDDtoUTM(LimiteSite.Pt0)}{CrLf}et {CrLf}{ConvertPointDDtoUTM(LimiteSite.Pt2)}";
                    TitreInformation = "Point Hors limites";
                    AfficherInformation();
                    return;
                }
                // renvoie un point double en DD
                Tag = Result;
            }
            Cursor.Clip = StockClipCurseur;
            TitreInformation = Titre;
        }

        private void HEM_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelHem.Text = Hem.SelectedItem.ToString();
        }

        private void LabelHem_Click(object sender, EventArgs e)
        {
            Hem.DroppedDown = true;
        }

        private void ZONE_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelZone.Text = Zone.SelectedItem.ToString();
        }

        private void LabelZONE_Click(object sender, EventArgs e)
        {
            Zone.DroppedDown = true;
        }
    }
}