using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> variables et fonctions globales aux classes et modules de FCGP Capturer </summary>
    static class CapturerGlobal
    {
        #region Variables globales
        /// <summary> font servant pour l'écriture des étiquettes des points ou de l'échelle sur la visue </summary>
        internal static Font FontEtiquette;
        /// <summary> gestion de l'affichage des tuiles </summary>
        internal static AffichageCarte Affichage;
        /// <summary> gestion de l'obtention des tuiles </summary>
        internal static ServeurCarto Serveur;
        /// <summary> conteneur pour les variables qui permettent le dessin d'une échelle sur l'affichage </summary>
        internal static DessinEchelle EchelleDessin;
        /// <summary> flag indiquant si le site et niveau de détail encours permettent l'affichage du dessin de l'échelle </summary>
        internal static bool IsDessinEchelle;
        /// <summary> définition du système cartographique encours et qui servira pour le téléchargement des cartes </summary>
        internal static SystemeCartographique SysCartoEncours;
        /// <summary> liste des différents coefs alpha possibles de la couche pentes </summary>
        internal readonly static float[] CoefsAlphasPentes = new float[] { 0.15f, 0.33f, 0.5f, 0.66f, 0.8f, 1f };
        /// <summary> coef alpha de la couche des pentes sous forme de matrice GDI pour le dessin de la couche </summary>
        internal static ImageAttributes ImageAttributsPentes;
        /// <summary> flag indiquant qu'un remplissage de tampon a été demandé </summary>
        internal static bool FlagRemplirTampon;
        /// <summary> flag limitant les déplacements et la difinition des points aux limites du site de capture </summary>
        internal static bool FlagLimitesSite = true;
        /// <summary> Flag indiquant que la couche des pentes est à afficher sur la couche de fond de plan </summary>
        internal static bool IsAffichagePentes
        {
            get
            {
                return Affichage.CoucheFondIspentes && CapturerSettings.IS_AFFICHE_PENTES;
            }
        }
        #endregion
        /// <summary> ouvre un formulaire de saisie de coordonnées en fonction du type de coordonnées qui est sélectionné </summary>
        /// <param name="IndexUT"> type de coordonnées sélectionné </param>
        /// <param name="PointCoordonnees"> coordonnées saisies </param>
        /// <param name="Parent"> Formulaire à partir duquel l'action est lancée </param>
        /// <param name="Position"> position en pixel de la fenêtre parente. Si différent de Point.empty la position du formulaire sera manuel, sinon centrée sur le parent </param>
        /// <returns> Ok ou Abort </returns>
        internal static DialogResult OuvrirFormulaireSaisieCoordonnees(Form Parent, Point Position, int IndexUT, ref PointD PointCoordonnees)
        {
            Form R = null;
            var FlagDD = default(bool);
            switch (IndexUT)
            {
                case 0:
                    {
                        R = new SaisieGrille();
                        break;
                    }
                case 1:
                    {
                        FlagDD = true;
                        R = new SaisieDD();
                        break;
                    }
                case 2:
                    {
                        FlagDD = true;
                        R = new SaisieDMS();
                        break;
                    }
                case 3:
                    {
                        FlagDD = true;
                        R = new SaisieUTM();
                        break;
                    }
                case 4:
                    {
                        R = new SaisieTuile();
                        break;
                    }
            }
            // si l'appelant le désire, il peut positionner le formulaire de saisie des coordonnées là où il le souhaite
            if (Position != Point.Empty)
            {
                R.StartPosition = FormStartPosition.Manual;
                R.Location = Position;
            }
            if (FlagDD && !PointCoordonnees.IsEmpty)
            {
                PointCoordonnees = PointGrilleToPointDD(PointCoordonnees, SysCartoEncours.Projection.Datum);
            }
            R.Tag = PointCoordonnees;
            var Ret = R.ShowDialog(Parent);
            if (Ret == DialogResult.OK)
            {
                PointCoordonnees = (PointD)R.Tag;
                if (FlagDD)
                    PointCoordonnees = PointDDToPointGrille(PointCoordonnees, SysCartoEncours.Projection.Datum);
            }
            R.Dispose();
            return Ret;
        }
        /// <summary> conteneur pour le dessin des échelles qui est composé par 4 segments de droite(3 Verticaux et 1 horizontal)
        /// et une étiquette indiquant la distance représentée par le segment horizontal. dessinné en bas à droite de l'affichage 
        /// attention pour les sites WebMercator l'échelle n'est pas valable pour l'axe des Y </summary>
        internal struct DessinEchelle
        {
            internal string TexteEchelle;
            internal Point Pt1, Pt2, Pt3, Pt4;
            internal Rectangle RectangleEtiquette;
            internal DessinEchelle(ref bool IsDessinEchelle)
            {
                if (!Affichage.CentreAffichage.IsEmpty)
                {
                    var NiveauDetail = SysCartoEncours.Niveau;
                    // on vérifie que le niveau de détail permet l'affichage d'une échelle (équivalent à Grille)
                    if (NiveauDetail.PasGrilleNumeric == 0)
                    {
                        IsDessinEchelle = false;
                        return;
                    }
                    // initialisation des différentes variables
                    var Echelle = NiveauDetail.Echelle;
                    int LargeurVisue = VisueRectangle.Size.Width;
                    int HauteurVisue = VisueRectangle.Size.Height;
                    int LongueurEchelle;
                    if (Serveur.Datum == Datums.Web_Mercator)
                    {
                        // la grille WebMercator ne peut pas servir pour des calculs de distance, il faut passer par des conversions en grille UTM ou Lambert.
                        // UTM couvre le monde entier alors que Lambert est plus précis mais local à la France Métropolitaine
                        var Site = SysCartoEncours.SiteCarto;
                        var PDD = PointPixelsToPointDD(Affichage.CentreAffichage, Site, Echelle);
                        var PP = ConvertWGS84ToProjection(PDD, Datums.UTM_WGS84);
                        PP.X += NiveauDetail.PasGrilleNumeric;
                        PDD = ConvertProjectionToWGS84(Datums.UTM_WGS84, PP);
                        var CentreDecale = PointDDToPointPixels(PDD, Site, Echelle);
                        LongueurEchelle = CentreDecale.X - Affichage.CentreAffichage.X;
                    }
                    else
                    {
                        // SuisseMobile est le seul site géré par FCGP qui a une grille qui ne soit pas WebMercator
                        LongueurEchelle = (int)Math.Round(NiveauDetail.PasGrilleNumeric * PixelsMetre(Echelle));
                    }
                    TexteEchelle = NiveauDetail.PasGrilleTexte;
                    using (var G = FormApplication.CreateGraphics())
                    {
                        var S = G.MeasureString(TexteEchelle, FontEtiquette).ToSize();
                        Pt2 = new Point(LargeurVisue - 10, HauteurVisue - 10);
                        Pt1 = new Point(Pt2.X - LongueurEchelle, HauteurVisue - 10);
                        Pt3 = new Point(Pt2.X - LongueurEchelle / 2, HauteurVisue - 10);
                        Pt4 = new Point(Pt1.X + (LongueurEchelle - S.Width) / 2, Pt1.Y - S.Height - 15);
                        RectangleEtiquette = new Rectangle(Pt4, S);
                    }
                    IsDessinEchelle = true;
                }
            }
        }
        /// <summary> assume que certaines touche d'édition sont admises </summary>
        internal static bool SuppressionTouche(TypeSaisie TypeSaisie, Keys Touche)
        {
            bool Ret = true;
            switch (Touche)
            {
                // Touches d'édition. Commun à tous les types de saisie
                case Keys.Back:
                case Keys.Delete:
                case Keys.Left:
                case Keys.Right:
                case Keys.Tab:
                    {
                        Ret = false;
                        break;
                    }
                // Touches des digits. Commun à tous les types de saisie
                case var case1 when Keys.NumPad0 <= case1 && case1 <= Keys.NumPad9:
                case var case2 when Keys.D0 <= case2 && case2 <= Keys.D9:
                    {
                        Ret = false;
                        break;
                    }
                // Touches moins et décimal
                case Keys.Decimal:
                case Keys.OemPeriod:
                    {
                        if (TypeSaisie > TypeSaisie.EntierNegatif)
                        {
                            Ret = false;
                        }

                        break;
                    }
                case Keys.Subtract:
                    {
                        if (TypeSaisie == TypeSaisie.EntierNegatif || TypeSaisie == TypeSaisie.DecimalNegatif)
                        {
                            Ret = false;
                        }

                        break;
                    }
            }
            return Ret;
        }
        internal static bool SuppressionCaractere(TypeSaisie Saisie, TextBox C, char Caractere)
        {
            bool Ret = false;
            // on interdit la saisie du 1er caractère si celui existant est le signe -, il faut l'effacer en premier
            if (C.SelectionStart == 0 & C.Text.StartsWith("-"))
            {
                Ret = true;
            }
            else
            {
                switch (Caractere)
                {
                    case ';': // on interdit le point virgule mais c'est la même touche que le . décimal
                        {
                            Ret = true;
                            break;
                        }
                    case '.':
                        {
                            if (Saisie > TypeSaisie.EntierNegatif)
                            {
                                // on interdit d'avoir plus d'un caractère . dans le texte
                                if (C.Text.Contains('.'))
                                {
                                    Ret = true;
                                }
                            }
                            else
                            {
                                // on interdit d'avoir le caractère . dans le texte
                                Ret = true;
                            }

                            break;
                        }
                    case '-': // on interdit d'avoir plus d'un caractère - dans le texte
                        {
                            if (Saisie == TypeSaisie.EntierNegatif || Saisie == TypeSaisie.DecimalNegatif)
                            {
                                // on interdit plus d'un caractère - dans le texte
                                if (C.Text.Contains('-'))
                                {
                                    Ret = true;
                                }
                                // on interdit que le caractère - si il n'est pas le premier
                                else if (C.SelectionStart != 0)
                                {
                                    Ret = true;
                                }
                            }
                            else
                            {
                                // on interdit d'avoir le caractère - dans le texte
                                Ret = true;
                            }

                            break;
                        }
                    case var case1 when '0' <= case1 && case1 <= '9':
                        {
                            Ret = false;
                            break;
                        }
                    case var case1 when case1 == Back:
                        {
                            Ret = false;
                            break;
                        }

                    default:
                        {
                            Ret = true;
                            break;
                        }
                }
            }
            return Ret;
        }
        internal static string FormaterDecimal(string Texte)
        {
            string Ret = Texte;
            if (Texte.StartsWith(".") || Texte.StartsWith("-."))
            {
                Ret = Texte.Replace(".", "0.");
            }
            return Ret;
        }
        internal enum TypeSaisie
        {
            EntierPositif = 0,
            EntierNegatif,
            DecimalPositif,
            DecimalNegatif
        }
    }
}