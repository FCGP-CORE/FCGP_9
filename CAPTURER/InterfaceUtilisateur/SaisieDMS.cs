using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;

namespace FCGP
{
    /// <summary> permet la saisie d'un PointD au format DMS </summary>
    internal partial class SaisieDMS
    {
        private RectangleD LimiteSite;
        private readonly TypeSaisie SaisieDM = TypeSaisie.EntierPositif;
        private readonly TypeSaisie SaisieS = TypeSaisie.DecimalPositif;
        private Rectangle StockClipCurseur;
        private string Titre;
        /// <summary> initialisation de la location et des différents champs de saisie </summary>
        private void SaisieDMS_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            // Attend un pointD en DD
            PointD P = (PointD)Tag;
            if (P.IsEmpty)
            {
                Longitude.SelectedIndex = 0; // "E"
                Latitude.SelectedIndex = 0; // "N"
            }
            else
            {
                (char Sens, int Deg, int Min, double Sec, string DMSTxt) DMS;
                DMS = ConvertDDtoDMS(P.X, Enumerations.TypesCoordsDD.Longitude, ".0", true);
                DegX.Text = DMS.Deg.ToString();
                DegX.Select(DegX.Text.Length, 0);
                MinX.Text = DMS.Min.ToString();
                MinX.Select(MinX.Text.Length, 0);
                SecX.Text = DblToStr(DMS.Sec, "N1");
                SecX.Select(SecX.Text.Length, 0);
                Longitude.SelectedIndex = DMS.Sens == 'E' ? 0 : 1;
                DMS = ConvertDDtoDMS(P.Y, Enumerations.TypesCoordsDD.Latitude, ".0", true);
                DegY.Text = DMS.Deg.ToString();
                DegY.Select(DegY.Text.Length, 0);
                MinY.Text = DMS.Min.ToString();
                MinY.Select(MinY.Text.Length, 0);
                SecY.Text = DblToStr(DMS.Sec, "N1");
                SecY.Select(SecY.Text.Length, 0);
                Latitude.SelectedIndex = DMS.Sens == 'N' ? 0 : 1;
            }
            LimiteSite = RegionGrilleToRegionDD(Serveur.Limites, Serveur.SiteCarto);
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> filtrage des touches admises </summary>
        private void Coord_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox C = (TextBox)sender;
            if (C.Name[0] == 'S')
            {
                e.SuppressKeyPress = SuppressionTouche(SaisieS, e.KeyCode);
            }
            else
            {
                e.SuppressKeyPress = SuppressionTouche(SaisieDM, e.KeyCode);
            }
        }
        /// <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
        private void Coord_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox C = (TextBox)sender;
            if (C.Name[0] == 'S')
            {
                e.Handled = SuppressionCaractere(SaisieS, C, e.KeyChar);
            }
            else
            {
                e.Handled = SuppressionCaractere(SaisieDM, C, e.KeyChar);
            }
        }
        private void Coord_Leave(object sender, EventArgs e)
        {
            TextBox C = (TextBox)sender;
            C.Text = FormaterDecimal(C.Text);
        }
        /// <summary> convertit une chaine de caractères en integer. Si null en integer incompatible Lon/Lat </summary>
        private static short TextToShort(string Text)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return 500;
            }
            else
            {
                return short.Parse(Text);
            }
        }
        /// <summary> convertit une chaine de caractères en double. Si null en double incompatible seconde </summary>
        private static double TextToDbl(string Text)
        {
            if (string.IsNullOrEmpty(Text) || Text == ".")
            {
                return 500.0d;
            }
            else
            {
                return StrToDbl(Text);
            }
        }
        /// <summary> affiche un message d'erreur </summary>
        private static void Erreur(string Champ, double Min, double Max)
        {
            MessageInformation = $"Le champ {Champ} doit être compris" + CrLf + $"entre {Min} et {Max}";
            TitreInformation = "Erreur de saisie";
            AfficherInformation();
        }
        /// <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
        private void SaisieDMS_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                short VDX = TextToShort(DegX.Text);
                if (VDX > 180)
                {
                    Erreur("Longitude Degré", 0d, 180d);
                    DegX.Focus();
                    e.Cancel = true;
                    return;
                }
                short VMX = TextToShort(MinX.Text);
                if (VMX > 60)
                {
                    Erreur("Minute", 0d, 60d);
                    MinX.Focus();
                    e.Cancel = true;
                    return;
                }
                double VSX = TextToDbl(SecX.Text);
                if (VSX > 60.0d)
                {
                    Erreur("Seconde", 0d, 60d);
                    SecX.Focus();
                    e.Cancel = true;
                    return;
                }
                short VDY = TextToShort(DegY.Text);
                if (VDY > 90)
                {
                    Erreur("Latitude Degré", 0d, 90d);
                    DegY.Focus();
                    e.Cancel = true;
                    return;
                }
                short VMY = TextToShort(MinY.Text);
                if (VMY > 60)
                {
                    Erreur("Minute", 0d, 60d);
                    MinY.Focus();
                    e.Cancel = true;
                    return;
                }
                double VSY = TextToDbl(SecY.Text);
                if (VSY > 60.0d)
                {
                    Erreur("Seconde", 0d, 60d);
                    SecY.Focus();
                    e.Cancel = true;
                    return;
                }
                // renvoie un point double en DD
                var Result = new PointD(ConvertDMStoDD(Longitude.Text[0], VDX, VMX, VSX), ConvertDMStoDD(Latitude.Text[0], VDY, VMY, VSY));
                // Logic point saisi en dehors des limites
                if (FlagLimitesSite && !LimiteSite.CoordonneesContains(Result))
                {
                    e.Cancel = true;
                    MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointDDtoDMS(LimiteSite.Pt0)}{CrLf}et {CrLf}{ConvertPointDDtoDMS(LimiteSite.Pt2)}";
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

        private void Latitude_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelLatitude.Text = Latitude.SelectedItem.ToString();
        }

        private void LabelLatitude_Click(object sender, EventArgs e)
        {
            Latitude.DroppedDown = true;
        }

        private void Longitude_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabelLongitude.Text = Longitude.SelectedItem.ToString();
        }

        private void LabelLongitude_Click(object sender, EventArgs e)
        {
            Longitude.DroppedDown = true;
        }
    }
}