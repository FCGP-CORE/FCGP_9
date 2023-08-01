using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;

namespace FCGP
{
    /// <summary> permet la saisie d'un PointD au format DD </summary>
    internal partial class SaisieDD
    {
        private RectangleD LimiteSite;
        private readonly TypeSaisie SaisieLatLon = TypeSaisie.DecimalNegatif;
        private Rectangle StockClipCurseur;
        private string Titre;
        /// <summary> initialisation de la location et des différents champs de saisie </summary>
        private void SaisieDD_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            // Attend un pointD en DD
            PointD Pt = (PointD)Tag;
            if (!Pt.IsEmpty)
            {
                CoordLon.Text = DblToStr(Pt.X, "N8");
                CoordLon.Select(CoordLon.Text.Length, 0);
                CoordLat.Text = DblToStr(Pt.Y, "N8");
                CoordLat.Select(CoordLat.Text.Length, 0);
            }
            LimiteSite = RegionGrilleToRegionDD(Serveur.Limites, Serveur.SiteCarto);
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> filtrage des touches admises </summary>
        private void CoordLatLon_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = SuppressionTouche(SaisieLatLon, e.KeyCode);
        }
        /// <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
        private void CoordLatLon_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox C = (TextBox)sender;
            e.Handled = SuppressionCaractere(SaisieLatLon, C, e.KeyChar);
        }
        private void CoordLatLon_Leave(object sender, EventArgs e)
        {
            TextBox C = (TextBox)sender;
            C.Text = FormaterDecimal(C.Text);
        }
        /// <summary> affiche un message d'erreur </summary>
        private static void Erreur(string Champ, double Min, double Max)
        {
            MessageInformation = $"Le champ {Champ} doit être compris" + CrLf + $"entre {Min:0.00} et {Max:0.00}";
            TitreInformation = "Erreur de saisie";
            AfficherInformation();
        }
        /// <summary> convertit une chaine de caractères en double. Si null en double incompatible seconde </summary>
        private static double TextToDbl(string Text)
        {
            if (string.IsNullOrEmpty(Text) || Text == "." || Text == "-." || Text == "-")
            {
                return 500.0d;
            }
            else
            {
                return StrToDbl(Text);
            }
        }
        /// <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
        private void SaisieDD_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                double VDX = TextToDbl(CoordLon.Text);
                if (VDX > 180d || VDX < -180)
                {
                    Erreur("Longitude", -180, 180d);
                    CoordLon.Focus();
                    e.Cancel = true;
                    return;
                }
                double VDY = TextToDbl(CoordLat.Text);
                if (VDY > 90d || VDY < -90)
                {
                    Erreur("Latitude", -90, 90d);
                    CoordLat.Focus();
                    e.Cancel = true;
                    return;
                }
                var Result = new PointD(VDX, VDY);
                // Logic point saisi en dehors des limites
                if (FlagLimitesSite && !LimiteSite.CoordonneesContains(Result))
                {
                    e.Cancel = true;
                    MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointDDtoChaine(LimiteSite.Pt0, "N5")}{CrLf}et {CrLf}{ConvertPointDDtoChaine(LimiteSite.Pt2, "N5")}";
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
    }
}