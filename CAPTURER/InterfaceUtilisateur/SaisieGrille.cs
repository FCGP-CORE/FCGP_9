using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;

namespace FCGP
{
    /// <summary> permet la saisie d'un PointD au format Grille </summary>
    internal partial class SaisieGrille
    {
        private RectangleD LimiteSite;
        private readonly TypeSaisie SaisieXY = TypeSaisie.EntierNegatif;
        private Rectangle StockClipCurseur;
        private string Titre;
        /// <summary> initialisation de la location et des différents champs de saisie </summary>
        private void SaisieGrille_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            // Attend un point dd en DD
            PointD P = (PointD)Tag;
            if (!P.IsEmpty)
            {
                CoordX.Text = P.X.ToString("#0");
                CoordX.Select(CoordX.Text.Length, 0);
                CoordY.Text = P.Y.ToString("#0");
                CoordY.Select(CoordY.Text.Length, 0);
            }
            LimiteSite = Serveur.Limites;
            Cursor.Clip = new Rectangle(Location, Size);
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
        private void SaisieGrille_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(CoordX.Text))
                {
                    e.Cancel = true;
                    Erreur("X", -20000000, 20000000);
                    CoordX.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(CoordY.Text))
                {
                    e.Cancel = true;
                    Erreur("Y", -10000000, 10000000);
                    CoordY.Focus();
                    return;
                }
                var Result = new PointD(int.Parse(CoordX.Text), int.Parse(CoordY.Text));
                // Logic point saisi en dehors des limites
                if (FlagLimitesSite && !LimiteSite.CoordonneesContains(Result))
                {
                    e.Cancel = true;
                    MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointXYtoChaine(new PointProjection(LimiteSite.Pt0))}{CrLf}et {CrLf}" + ConvertPointXYtoChaine(new PointProjection(LimiteSite.Pt2));
                    TitreInformation = "Point Hors limites";
                    AfficherInformation();
                    return;
                }
                // renvoie un point double en mètres
                Tag = Result;
            }
            Cursor.Clip = StockClipCurseur;
            TitreInformation = Titre;
        }
    }
}