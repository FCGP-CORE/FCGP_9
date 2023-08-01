using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> permet la saisie d'un pointD au format tuile </summary>
    internal partial class SaisieTuile
    {
        private Rectangle LimiteSite, StockClipCurseur;
        private string Titre;
        private readonly TypeSaisie SaisieXY = TypeSaisie.EntierPositif;
        /// <summary> initialisation de la location et des différents champs de saisie </summary>
        private void SaisieTuile_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            // Attend un point dd en PointGrille
            PointD PointCoordonnees = (PointD)Tag;
            if (!PointCoordonnees.IsEmpty)
            {
                var PointPixels = PointGrilleToPointPixel(PointCoordonnees, Serveur.SiteCarto, Affichage.Echelle);
                var PG = new PointG(CapturerSettings.INDICE_ECHELLE, PointPixels);
                Col.Text = PG.IndexTuile.X.ToString("#0");
                Col.Select(Col.Text.Length, 0);
                DecalX.Text = PG.Offset.Width.ToString("#0");
                DecalX.Select(DecalX.Text.Length, 0);
                Row.Text = PG.IndexTuile.Y.ToString("#0");
                Row.Select(Row.Text.Length, 0);
                DecalY.Text = PG.Offset.Height.ToString("#0");
                DecalY.Select(DecalY.Text.Length, 0);
            }
            LimiteSite = RegionGrilleToRegionPixels(Serveur.Limites, Serveur.SiteCarto, Affichage.Echelle);
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> filtrage des touches admises </summary>
        private void Coord_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = SuppressionTouche(SaisieXY, e.KeyCode);
        }
        /// <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
        private void Coord_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox C = (TextBox)sender;
            e.Handled = SuppressionCaractere(SaisieXY, C, e.KeyChar);
        }
        /// <summary> affiche un message d'erreur </summary>
        private static void Erreur(string Champ)
        {
            MessageInformation = $"Le champ {Champ} doit être égal ou supérieur à 0";
            TitreInformation = "Erreur de saisie";
            AfficherInformation();
        }
        /// <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
        private void SaisieTuile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                // renvoie un point double en DD
                if (string.IsNullOrEmpty(Col.Text))
                {
                    e.Cancel = true;
                    Erreur("Index Tuile X");
                    Col.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(Row.Text))
                {
                    e.Cancel = true;
                    Erreur("Index Tuile Y");
                    Row.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(DecalX.Text))
                {
                    e.Cancel = true;
                    Erreur("Decalage Pixels X");
                    DecalX.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(DecalY.Text))
                {
                    e.Cancel = true;
                    Erreur("Decalage Pixels Y");
                    DecalY.Focus();
                    return;
                }
                var Result = new PointG(0, int.Parse(Col.Text), int.Parse(Row.Text), int.Parse(DecalX.Text), int.Parse(DecalY.Text));
                // Logic point saisi en dehors des limites
                if (FlagLimitesSite && !LimiteSite.Contains(Result.Location))
                {
                    e.Cancel = true;
                    MessageInformation = $"Le point doit être compris entre :{CrLf}{new PointG(CapturerSettings.INDICE_ECHELLE, LimiteSite.Location).ToString()}{CrLf}et {CrLf}" + new PointG(CapturerSettings.INDICE_ECHELLE, Point.Add(LimiteSite.Location, new Size(LimiteSite.Width, LimiteSite.Height))).ToString();
                    TitreInformation = "Point Hors limites";
                    AfficherInformation();
                    return;
                }
                // renvoie un point double en mètres
                Tag = PointPixelToPointGrille(Result.Location, Serveur.SiteCarto, Affichage.Echelle);
            }
            Cursor.Clip = StockClipCurseur;
            TitreInformation = Titre;
        }
    }
}