using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;
using static FCGP.Settings;
using static FCGP.TelechargementCarte;

namespace FCGP
{
    /// <summary> Permet la saisie d'une surface de capture à partir de 2 points de délimitation  ou 
    /// directement à partir des index et du nombre de rangées et de colonnes </summary>
    internal partial class SaisieSurfaceTraitement
    {
        private const string DC = "Double Click";
        private const string N = "nb de ";
        private Label Index;
        private int IndexUT;
        private SitesCartographiques SiteCarto;
        private Echelles Echelle;
        private int IndiceEchelle;
        private Datums Datum;
        private PointD PG1, PG2;
        private Point PP1, PP2;
        private double HauteurMetres, LargeurMetres, SurfaceKm;
        private int[] ValeurIndex;
        private NI NumIndex;
        private Dir Direction;
        private SurfaceTuiles SurfaceTuileLimite;
        private int Diviseur;
        private Keys ToucheModif;
        private Rectangle StockClipCurseur;
        private string Titre;
        /// <summary> pour indiquer en clair le type des index et des nombres de colonne ou de rangée</summary>
        private enum NI : int
        {
            DebCol = 0,
            NbCol,
            DebRow,
            NbRow
        }
        /// <summary> pour indiquer en clair les directions </summary>
        private enum Dir : int
        {
            Aucun = 0,
            Est,
            Ouest,
            Nord,
            Sud
        }
        /// <summary> Positionne le formulaire au centre de l'écran et remplit les informations </summary>
        private void SurfaceCaptureTuile_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            StockClipCurseur = Cursor.Clip;
            (SurfaceTuiles SurfaceCapture, int SelectedIndex) = ((SurfaceTuiles, int))Tag;
            Datum = Serveur.Datum;
            IndexUT = SelectedIndex;
            NbTuilesMax.Text = NB_Max_TuilesServeurCarto.ToString("N0");
            NbTuilesMaxX.Text = NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, 1).ToString("N0");
            SiteCarto = CapturerSettings.SITE;
            Echelle = Affichage.Echelle;
            IndiceEchelle = CapturerSettings.INDICE_ECHELLE;
            ValeurIndex = new int[4];
            RemplirInformations(SurfaceCapture);
            SurfaceTuileLimite = new SurfaceTuiles(RegionGrilleToRegionPixels(Serveur.Limites, SiteCarto, Echelle), SiteCarto, Echelle);
            // positionnement du curseur de souris au milieu du bouton ok
            {
                Cursor.Position = new Point(Location.X + Valider.Bounds.X + Valider.Bounds.Width / 2, Location.Y + Valider.Location.Y + Valider.Size.Height / 2);
            }
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> ferme le formulaire et met à jour les points de capture de la visue si demandé </summary>
        private void SurfaceCaptureTuile_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                Tag = new SurfaceTuiles(PG1, PG2, SiteCarto, Echelle);
            }
            Cursor.Clip = StockClipCurseur;
            TitreInformation = Titre;
        }
        /// <summary> Remplit les informations des champs tuile et des points </summary>
        private void RemplirInformations(SurfaceTuiles Surface)
        {
            // mettre à jour les champs dépendants de la surface tuile si elle n'est pas vide
            RemplirChampsTuile(Surface);
            // mettre à jour les champs concernants les 2 points de capture si ils ne sont pas vide et en fonction du type de coordonnées
            switch (IndexUT)
            {
                case 0: // GRILLE
                    {
                        MettreAJourPointGrille(PG1, Point1);
                        MettreAJourPointGrille(PG2, Point2);
                        break;
                    }
                case 1: // DD
                    {
                        MettreAJourPointDD(PG1, Point1);
                        MettreAJourPointDD(PG2, Point2);
                        break;
                    }
                case 2: // DMS
                    {
                        MettreAJourPointDMS(PG1, Point1);
                        MettreAJourPointDMS(PG2, Point2);
                        break;
                    }
                case 3: // UTM
                    {
                        MettreAjourPointUTM(PG1, Point1);
                        MettreAjourPointUTM(PG2, Point2);
                        break;
                    }
                case 4: // Tuile
                    {
                        MettreAjourPointTuile(PG1, Point1);
                        MettreAjourPointTuile(PG2, Point2);
                        break;
                    }
            }
        }
        /// <summary> Remplit les informations des points avec des coordoonées en Grille   </summary>
        private static void MettreAJourPointGrille(PointD PG, Label Lab)
        {
            if (PG.IsEmpty)
            {
                Lab.Text = "";
            }
            else
            {
                Lab.Text = ConvertPointXYtoChaine(new PointProjection(PG));
            }
        }
        /// <summary> Remplit les informations des points avec des coordoonées en DD   </summary>
        private void MettreAJourPointDD(PointD PG, Label Lab)
        {
            if (PG.IsEmpty)
            {
                Lab.Text = "";
            }
            else
            {
                PG = PointGrilleToPointDD(PG, Datum);
                Lab.Text = ConvertPointDDtoChaine(PG);
            }
        }
        /// <summary> Remplit les informations des points avec des coordoonées en DMS   </summary>
        private void MettreAJourPointDMS(PointD PG, Label Lab)
        {
            if (PG.IsEmpty)
            {
                Lab.Text = "";
            }
            else
            {
                PG = PointGrilleToPointDD(PG, Datum);
                Lab.Text = ConvertPointDDtoDMS(PG);
            }
        }
        /// <summary> Remplit les informations des points avec des coordoonées en UTM </summary>
        private void MettreAjourPointUTM(PointD PG, Label Lab)
        {
            if (PG.IsEmpty)
            {
                Lab.Text = "";
            }
            else
            {
                PG = PointGrilleToPointDD(PG, Datum);
                Lab.Text = ConvertPointDDtoUTM(PG);
            }
        }
        /// <summary> Remplit les informations des points avec des coordoonées en tuile_Niveau </summary>
        private void MettreAjourPointTuile(PointD PG, Label Lab)
        {
            if (PG.IsEmpty)
            {
                Lab.Text = "";
            }
            else
            {
                var PointPixel = PointGrilleToPointPixel(PG, SiteCarto, Echelle);
                Lab.Text = new PointG(IndiceEchelle, PointPixel).ToString();
            }
        }
        /// <summary> calcule de manière aprroximative les dimensions de la surface de téléchargement à partir des coordonnées </summary>
        private void CalculerDimensionsCapture(SurfaceTuiles S)
        {
            (double Largeur, double Hauteur) DimensionsSurface;
            PointD Pt0, Pt2;
            if (Datum == Datums.Web_Mercator) // il s'agit de mètre qui n'en sont plus sorti de l'équateur
            {
                Pt0 = PointGrilleToPointDD(S.RectangleGrille.Pt(0));
                Pt2 = PointGrilleToPointDD(S.RectangleGrille.Pt(2));
                DimensionsSurface = CalculerDimensionsCarte(Pt0, Pt2, true);
            }
            else
            {
                Pt0 = S.RectangleGrille.Pt(0);
                Pt2 = S.RectangleGrille.Pt(2);
                DimensionsSurface = CalculerDimensionsCarte(Pt0, Pt2, false);
            }
            LargeurMetres = DimensionsSurface.Largeur;
            HauteurMetres = DimensionsSurface.Hauteur;
            SurfaceKm = LargeurMetres * HauteurMetres / 1000000d;
        }
        /// <summary> Remplit les informations des champs tuile </summary>
        private void RemplirChampsTuile(SurfaceTuiles S)
        {
            ValeurIndex[(int)NI.DebCol] = S.NumColDeb;
            DebCol.Text = S.NumColDeb.ToString("N0");
            ValeurIndex[(int)NI.NbCol] = S.NbColonnes;
            NbCol.Text = S.NbColonnes.ToString("N0");
            ValeurIndex[(int)NI.DebRow] = S.NumRangDeb;
            DebRow.Text = S.NumRangDeb.ToString("N0");
            ValeurIndex[(int)NI.NbRow] = S.NbRangees;
            NbRow.Text = S.NbRangees.ToString("N0");
            // on remplit largeur, hauteur, surface et tous les champs associé aux tuiles
            if (!S.IsEmpty)
            {
                PG1 = S.PtGrille_NO;
                PG2 = S.PtGrille_SE;
                PP1 = S.PtPixels_NO;
                PP2 = S.PtPixels_SE;
                CalculerDimensionsCapture(S);
                Largeur.Text = LargeurMetres.ToString("N0");
                Hauteur.Text = HauteurMetres.ToString("N0");
                Surface.Text = SurfaceKm.ToString("N0");
                NbTuilesTelecharger.Text = S.NbTuiles.ToString("N0");
                if (S.NbColonnes > NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, 1))
                {
                    NbTuilesMaxX.BackColor = Color.Tomato;
                }
                else
                {
                    NbTuilesMaxX.BackColor = Color.YellowGreen;
                }
                var TextTooltip = new StringBuilder("Nombre de tuiles pour la surface de téléchargement encours.", 250);
                if (S.NbTuiles > NB_Max_TuilesServeurCarto)
                {
                    NbTuilesMax.BackColor = Color.Tomato;
                    ToolTexteNbTuile(S, TextTooltip);
                }
                else
                {
                    NbTuilesMax.BackColor = Color.YellowGreen;
                }
                ToolTip1.SetToolTip(NbTuilesTelecharger, TextTooltip.ToString());
            }
            else
            {
                Largeur.Text = "";
                Hauteur.Text = "";
                Surface.Text = "";
                NbTuilesTelecharger.Text = "";
            }
        }
        /// <summary> Action du bouton Fermer </summary>
        private void Fermer_Click(object sender, EventArgs e)
        {
            // si l'on est entrain de saisir un index on valide les donnees avant de fermer
            FermerSaisieIndex();
            Close();
        }
        /// <summary> ferme la zone de saisie des index et des nombres si elle est ouverte avec validation </summary>
        private void FermerSaisieIndex()
        {
            if (SaisieIndex.Visible)
            {
                SaisieIndex_KeyDown(null, new KeyEventArgs(Keys.Enter));
            }
        }
        /// <summary> Configure le formulaire pour qu'il ne puisse pas se fermer sur Enter ou ESC et remplit le champ de saisie </summary>
        private void IndexNombreTuile_Click(object sender, EventArgs e)
        {
            // si l'on est deja entrain de saisir un index on valide les donnees
            FermerSaisieIndex();
            AcceptButton = null;
            CancelButton = null;
            Index = (Label)sender;
            NumIndex = (NI)Convert.ToInt32(Index.Tag);
            SaisieIndex.Text = ValeurIndexToStr(ValeurIndex[(int)NumIndex]);
            SaisieIndex.Select(SaisieIndex.Text.Length, 0);
            SaisieIndex.Location = new Point(Index.Location.X + 3, Index.Location.Y + 1);
            SaisieIndex.Visible = true;
            SaisieIndex.Focus();
            ToolTip1.SetToolTip(SaisieIndex, TextToolTip());
        }
        /// <summary> Permet d'ajuster les index ou les nombres en fonction du modificateur de touche sur le doubleClick </summary>
        private void SaisieIndex_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SaisieIndex.Text))
            {
                int Valeur = int.Parse(SaisieIndex.Text);
                if (NumIndex == NI.NbCol || NumIndex == NI.NbRow) // sur les nombres de col ou row  on peut diviser
                {
                    if (ToucheModif != Keys.None)
                        CalculerModificationNombre();
                    if (Diviseur > 0)
                    {
                        Valeur = (int)Math.Ceiling(Valeur / (double)Diviseur);
                        Diviseur = 0;
                    }
                    else
                    {
                        // on réinitialise à la valeur de départ
                        Valeur = ValeurIndex[(int)NumIndex];
                    }
                }
                else
                {
                    if (ToucheModif != Keys.None)
                        CalculerModificationIndex();
                    // sur les index de col ou de row on peut influer sur la direction
                    switch (Direction)
                    {
                        case Dir.Est:
                            {
                                Valeur += ValeurIndex[(int)NI.NbCol];
                                break;
                            }
                        case Dir.Ouest:
                            {
                                Valeur -= ValeurIndex[(int)NI.NbCol];
                                break;
                            }
                        case Dir.Sud:
                            {
                                Valeur += ValeurIndex[(int)NI.NbRow];
                                break;
                            }
                        case Dir.Nord:
                            {
                                Valeur -= ValeurIndex[(int)NI.NbRow];
                                break;
                            }
                        case Dir.Aucun:
                            {
                                // on réinitialise à la valeur de départ
                                Valeur = ValeurIndex[(int)NumIndex];
                                break;
                            }
                    }
                    Direction = Dir.Aucun;
                }
                SaisieIndex.Text = Valeur.ToString("0");
                SaisieIndex.Select(SaisieIndex.Text.Length, 0);
                AcceptButton = null;
                CancelButton = null;
            }
        }
        /// <summary> autorise la touche Tab à être traitée par saisieindex </summary>
        private void SaisieIndex_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                e.IsInputKey = true;
        }
        /// <summary> n'autorise que les touches clavier représentant un chiffre ou les touches d'édition </summary>
        private void SaisieIndex_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.N:
                case Keys.O:
                case Keys.Q:
                case Keys.S:
                case Keys.T:
                    {
                        ToucheModif = Keys.None;
                        break;
                    }
            }
        }
        /// <summary> n'autorise que les touches clavier représentant un chiffre ou les touches d'édition </summary>
        private void SaisieIndex_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case var case1 when Keys.NumPad0 <= case1 && case1 <= Keys.NumPad9:
                case var case2 when Keys.D0 <= case2 && case2 <= Keys.D9:
                case Keys.Back:
                case Keys.Delete:
                case Keys.Left:
                case Keys.Right:
                    {
                        break;
                    }
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.N:
                case Keys.O:
                case Keys.Q:
                case Keys.S:
                case Keys.T:
                    {
                        ToucheModif = e.KeyCode;
                        e.SuppressKeyPress = true;
                        return;
                    }
                case Keys.Tab:
                    {
                        // DebCol doit être le N°0 dans la liste des controles du panel et les autres doivent suivre l'ordre de NI
                        if (e.Modifiers == Keys.Shift)
                        {
                            IndexNombreTuile_Click(Support.Controls[NumIndex > NI.DebCol ? (int)NumIndex - 1 : (int)NI.NbRow], new EventArgs());
                        }
                        else
                        {
                            IndexNombreTuile_Click(Support.Controls[NumIndex < NI.NbRow ? (int)NumIndex + 1 : (int)NI.DebCol], new EventArgs());
                        }
                        e.SuppressKeyPress = true;
                        break;
                    }
                case Keys.Enter:
                    {
                        if (string.IsNullOrEmpty(SaisieIndex.Text))
                        {
                            ValeurIndex[(int)NumIndex] = 0;
                            Index.Text = "";
                        }
                        else
                        {
                            int Valeur = int.Parse(SaisieIndex.Text);
                            switch (NumIndex)
                            {
                                case NI.DebCol:
                                    {
                                        if ((Valeur < SurfaceTuileLimite.NumColDeb || Valeur > SurfaceTuileLimite.NumColFin) && FlagLimitesSite)
                                        {
                                            MessageInformation = $"L'index de colonnes doit être compris entre :{CrLf}{SurfaceTuileLimite.NumColDeb} et {SurfaceTuileLimite.NumColFin}";
                                            TitreInformation = "Point Hors limites";
                                            AfficherInformation();
                                            return;
                                        }

                                        break;
                                    }
                                case NI.NbCol:
                                    {
                                        int ValeurX = ValeurIndex[(int)NI.DebCol];
                                        if (ValeurX + Valeur > SurfaceTuileLimite.NumColFin && FlagLimitesSite)
                                        {
                                            MessageInformation = $"Le nb de colonnes doit être compris entre :{CrLf} 0 et {SurfaceTuileLimite.NumColFin - ValeurX}{CrLf}" +
                                                                 "par rapport à l'index des colonnes";
                                            TitreInformation = "Erreur de saisie";
                                            AfficherInformation();
                                            return;
                                        }

                                        break;
                                    }
                                case NI.DebRow:
                                    {
                                        if ((Valeur < SurfaceTuileLimite.NumRangDeb || Valeur > SurfaceTuileLimite.NumRangFin) && FlagLimitesSite)
                                        {
                                            MessageInformation = $"L'index de rangées doit être compris entre : {CrLf}{SurfaceTuileLimite.NumRangDeb} et {SurfaceTuileLimite.NumRangFin}";
                                            TitreInformation = "Point Hors limites";
                                            AfficherInformation();
                                            return;
                                        }

                                        break;
                                    }
                                case NI.NbRow:
                                    {
                                        int ValeurY = ValeurIndex[(int)NI.DebRow];
                                        if (ValeurY + Valeur > SurfaceTuileLimite.NumRangFin && FlagLimitesSite)
                                        {
                                            MessageInformation = $"Le nb de rangées doit être compris entre :{CrLf} 0 et {SurfaceTuileLimite.NumRangFin - ValeurY}{CrLf}" + "par rapport à l'index des rangées";
                                            TitreInformation = "Erreur de saisie";
                                            AfficherInformation();
                                            return;
                                        }

                                        break;
                                    }
                            }
                            ValeurIndex[(int)NumIndex] = Valeur;
                            Index.Text = Valeur.ToString("N0");
                        }
                        SortirSaisieIndex();
                        CalculerPointsPixels();
                        e.SuppressKeyPress = true;
                        break;
                    }
                case Keys.Escape:
                    {
                        SortirSaisieIndex();
                        e.SuppressKeyPress = true;
                        break;
                    }

                default:
                    {
                        e.SuppressKeyPress = true;
                        break;
                    }
            }
        }
        /// <summary> Configure le formulaire pour une fermeture par ESC ou ENTER </summary>
        private void SortirSaisieIndex()
        {
            // on marque saisieIndex comme fermer
            SaisieIndex.Visible = false;
            // on permet au formumaire principal de réagir avec les touches Enter et Escape
            AcceptButton = Valider;
            CancelButton = Annuler;
        }
        /// <summary> calcul les modification à apporter aux index ou aux nombres </summary>
        private void CalculerModificationIndex()
        {
            Direction = Dir.Aucun;
            switch (ToucheModif)
            {
                case Keys.E:
                    {
                        if (NumIndex == NI.DebCol)
                            Direction = Dir.Est;
                        break;
                    }
                case Keys.N:
                    {
                        if (NumIndex == NI.DebRow)
                            Direction = Dir.Nord;
                        break;
                    }
                case Keys.O:
                    {
                        if (NumIndex == NI.DebCol)
                            Direction = Dir.Ouest;
                        break;
                    }
                case Keys.S:
                    {
                        if (NumIndex == NI.DebRow)
                            Direction = Dir.Sud;
                        break;
                    }
            }
        }
        /// <summary> trouve le divisiseur des nombres en fonction de la touche appuyée </summary>
        private void CalculerModificationNombre()
        {
            Diviseur = 0;
            switch (ToucheModif)
            {
                case Keys.C:
                    {
                        Diviseur = 5;
                        break;
                    }
                case Keys.D:
                    {
                        Diviseur = 2;
                        break;
                    }
                case Keys.N:
                    {
                        Diviseur = 9;
                        break;
                    }
                case Keys.Q:
                    {
                        Diviseur = 4;
                        break;
                    }
                case Keys.S:
                    {
                        Diviseur = 7;
                        break;
                    }
                case Keys.T:
                    {
                        Diviseur = 3;
                        break;
                    }
            }
        }
        /// <summary> ouvre le formulaire adequat pour la saisie d'un point de coordonnée </summary>
        private void Point_Click(object sender, EventArgs e)
        {
            Cursor.Clip = StockClipCurseur;
            // si l'on est deja entrain de saisir un index on valide les donnees
            if (SaisieIndex.Visible == true)
                SaisieIndex_KeyDown(null, new KeyEventArgs(Keys.Enter));
            string NomPoint = ((Label)sender).Name;
            Point PositionSaisiePoints;
            PointD Coordonnees;
            if (NomPoint == "Point1")
            {
                PositionSaisiePoints = PointToScreen(Point1.Location);
                Coordonnees = PG1;
            }
            else
            {
                PositionSaisiePoints = PointToScreen(Point2.Location);
                Coordonnees = PG2;
            }
            PositionSaisiePoints.Y += 2;
            PositionSaisiePoints.X += 2;
            if (OuvrirFormulaireSaisieCoordonnees(this, PositionSaisiePoints, IndexUT, ref Coordonnees) == DialogResult.OK)
            {
                if (NomPoint == "Point1")
                {
                    PG1 = Coordonnees;
                }
                else
                {
                    PG2 = Coordonnees;
                }
                RemplirInformations(new SurfaceTuiles(PG1, PG2, SiteCarto, Echelle));
            }
            Cursor.Clip = new Rectangle(Location, Size);
        }
        /// <summary> Recalcul les points de capture en fonction des nouveaux index de tuiles </summary>
        private void CalculerPointsPixels()
        {
            if (ValeurIndex[(int)NI.DebCol] > 0 && ValeurIndex[(int)NI.DebRow] > 0 && ValeurIndex[(int)NI.NbCol] > 0 && ValeurIndex[(int)NI.NbRow] > 0)
            {
                var Pt0Tuile = new Point(ValeurIndex[(int)NI.DebCol], ValeurIndex[(int)NI.DebRow]);
                var Pt2Tuile = new Point(ValeurIndex[(int)NI.DebCol] + ValeurIndex[(int)NI.NbCol] - 1, ValeurIndex[(int)NI.DebRow] + ValeurIndex[(int)NI.NbRow] - 1);
                PP1 = TuileToPointPixel(Pt0Tuile, false);
                PP2 = TuileToPointPixel(Pt2Tuile, true);
                RemplirInformations(new SurfaceTuiles(PP1, PP2, SiteCarto, Echelle));
            }
        }
        /// <summary> renvoie un texte d'aide pour les index en fonction du type d'index</summary>
        /// <param name="TypeIndex"> type d'index </param>
        private static string ToolTexteIndex(string TypeIndex)
        {
            if (TypeIndex == "colonnes")
            {
                return $"{DC} + O --> Déplace l'index vers l'Ouest du {N}{TypeIndex}." + CrLf + $"{DC} + E --> Déplace l'index vers l'Est du {N}{TypeIndex}." + CrLf + $"{DC} --> Initialise l'index de {TypeIndex} à sa valeur de départ.";
            }
            else
            {
                return $"{DC} + N --> Déplace l'index vers le Nord du {N}{TypeIndex}." + CrLf + $"{DC} + S --> Déplace l'index vers le Sud du {N}{TypeIndex}." + CrLf + $"{DC} --> Initialise l'index de {TypeIndex} à sa valeur de départ.";
            }
        }
        /// <summary> renvoie un texte d'aide pour les nombres en fonction du type d'index</summary>
        /// <param name="TypeIndex"> type d'index </param>
        private static string ToolTexteNombre(string TypeIndex)
        {
            return $"{DC} + D --> Divise le {N}{TypeIndex} par 2." + CrLf +
                   $"{DC} + T --> Divise le {N}{TypeIndex} par 3." + CrLf +
                   $"{DC} + Q --> Divise le {N}{TypeIndex} par 4." + CrLf +
                   $"{DC} + C --> Divise le {N}{TypeIndex} par 5." + CrLf +
                   $"{DC} + S --> Divise le {N}{TypeIndex} par 7." + CrLf +
                   $"{DC} + N --> Divise le {N}{TypeIndex} par 9." + CrLf +
                   $"{DC} --> Initialise le {N}{TypeIndex} à sa valeur de départ.";
        }
        /// <summary> renvoie un texte d'aide en fonction de l'index ou du nombre passé en paramètre </summary>
        private string TextToolTip()
        {
            switch (NumIndex)
            {
                case NI.DebCol:
                    {
                        return ToolTexteIndex("colonnes");
                    }
                case NI.NbCol:
                    {
                        return ToolTexteNombre("colonnes");
                    }
                case NI.DebRow:
                    {
                        return ToolTexteIndex("rangées");
                    }
                case NI.NbRow:
                    {
                        return ToolTexteNombre("rangées");
                    }

                default:
                    {
                        return "";
                    }
            }
        }
        /// <summary> met à jour la description de la décomposition du nb de tuiles si celui-ci dépasse le nb de tuiles Max </summary>
        private static void ToolTexteNbTuile(SurfaceTuiles S, StringBuilder TextTooltip)
        {
            // mettre à jour le nb de rectangles pour découper la surface principale afin que le nb de tuiles maxi ne soit dépassé
#pragma warning disable IDE0042 // Déconstruire la déclaration de variable
            var Exa = Facteurs.NbMiniRectangle(S.NbColonnes, S.NbRangees, NB_Max_TuilesServeurCarto, true, true);
            var NExa = Facteurs.NbMiniRectangle(S.NbColonnes, S.NbRangees, NB_Max_TuilesServeurCarto, false, false);
#pragma warning restore IDE0042 // Déconstruire la déclaration de variable
            TextTooltip.AppendLine($"{CrLf}Décomposition possible de la surface");
            TextTooltip.Append($"Exact : Nb = {Exa.NbMini + CrLf}X : {MulText(S.NbColonnes, Exa.MultipleX, Exa.Xn)}");
            TextTooltip.Append($", Y : {MulText(S.NbRangees, Exa.MultipleY, Exa.Yn)}, Nb Tuiles : {Exa.Xn * Exa.Yn}");
            if (Exa.NbMini != NExa.NbMini)
            {
                TextTooltip.Append($"{CrLf}{(NExa.IsExact ? "Autre possibilité" : "Pas Exact")} : Nb = {NExa.NbMini + CrLf}X : {MulText(S.NbColonnes, NExa.MultipleX, NExa.Xn)}");
                TextTooltip.Append($", Y : {MulText(S.NbRangees, NExa.MultipleY, NExa.Yn)}, Nb Tuiles : {NExa.Xn * NExa.Yn}");
            }
        }
        /// <summary> renvoie un texte décrivant une division en texte en fonction du divisieur </summary>
        private static string MulText(int Nb, int Mul, int Result)
        {
            string MulTextRet = Mul == 1 ? $"{Nb}" : $"{Nb}/{Mul}={Result}";
            return MulTextRet;
        }
        /// <summary> transforme un nombre entier en texte </summary>
        private static string ValeurIndexToStr(int ValeurIndex)
        {
            string Ret = "";
            if (ValeurIndex > 0)
            {
                Ret = ValeurIndex.ToString();
            }
            return Ret;
        }

        /// <summary> contient la décomposition d'un nombre en facteurs </summary>
        private class Facteurs
        {
            /// <summary> calcul le nb minimal de rectangles nécessaires pour couvrir nbtotal sans dépasser nbmax </summary>
            /// <param name="X"> Largeur de la surface </param>
            /// <param name="Y"> Hauteur de la surface </param>
            /// <param name="NbMax"> Nombre maximum autorisé pour la surface représenté par X*Y </param>
            /// <param name="FlagExact"> autorise t'on une surface un peu plus grande que la surface initiale. True : Non. Dans ce cas on ne prend pas en compte FlagPremier </param>
            /// <param name="FlagPremier"> Autorise t'on les nb premiers comme retour. Cela suppose qu'une seule dimension soit coupée </param>
            internal static (int NbMini, bool IsExact, int Xn, int MultipleX, int Yn, int MultipleY)
                            NbMiniRectangle(int X, int Y, int NbMax, bool FlagExact = true, bool FlagPremier = false)
            {
                Facteurs FactDimension;
                int Reste, NbMini;
                int Surface = X * Y;
                // si le nbtotal est < nbmax on peut télécharger en une seule fois
                if (Surface <= NbMax)
                    return (1, true, X, 1, Y, 1);
                NbMini = (int)Math.Ceiling(Surface / (double)NbMax); // nb minimum de rectangles pour découper la surface initiale et respecter la contrainte de NbMax
                // on veut absolument que le nb de rectangle mini renvoie le même nb de tuile que la surface initiale au détriment du nb de rectangle qui peut devenir très grand.
                // il y a forcément un diviseur qui fonctionne c'est 1
                if (FlagExact)
                {
                    do
                    {
                        _ = Math.DivRem(Surface, NbMini, out Reste);
                        if (Reste == 0) // on a trouver le nb mini qui coupe la surface en rectangle entier
                        {
                            FactDimension = new Facteurs(NbMini);
                            int MultipleX = 1;
                            int MultipleY = 1;
                            for (int Cpt = FactDimension.Multiples.Length - 1; Cpt >= 0; Cpt -= 1) // il y a forcément 1 ou 2 dimensions qui sont divisibles par le multiple encours
                            {
                                int Multiple = FactDimension.Multiples[Cpt];
                                int ResultX = Math.DivRem(X, Multiple, out int ResteX);
                                int ResultY = Math.DivRem(Y, Multiple, out int ResteY);
                                if (ResteX == 0 & ResteY > 0) // x est divisible par le multiple de nbmini
                                {
                                    X = ResultX;
                                    MultipleX *= Multiple;
                                }
                                else if (ResteX > 0 & ResteY == 0) // Y est divisible par le multiple de nbmini
                                {
                                    Y = ResultY;
                                    MultipleY *= Multiple;
                                }
                                else if (X > Y) // X et Y sont divisibles par le multiple de nbmini et on prend la dimension la plus grande
                                {
                                    X = ResultX;
                                    MultipleX *= Multiple;
                                }
                                else
                                {
                                    Y = ResultY;
                                    MultipleY *= Multiple;
                                }
                            }
                            return (NbMini, true, X, MultipleX, Y, MultipleY);
                        }
                        else
                        {
                            NbMini += 1;
                        }
                    }
                    while (true);
                }
                else
                {
                    // on privilégie le nb de rectangle minimum au détriment du nb de tuiles à télécharger qui pourra être plus grand
                    // si on veut pouvoir diviser les 2 cotés il ne faut pas que nbmini soit premier
                    if (!FlagPremier)
                    {
                        if (NbMini == 2)
                        {
                            NbMini = 4; // 1er nombre non premier
                        }
                        else if (IsNbPremier(NbMini))
                        {
                            NbMini += 1; // les autres sont impair il suffit de rajouter 1 pour les rendre pair donc non premier
                        }
                    }
                    FactDimension = new Facteurs(NbMini);
                    // on regarde si l'opération renvoie le même nb de tuiles que celui de la surface initiale
                    _ = Math.DivRem(Surface, NbMini, out Reste); // normalement c'es
                                                                 // si NbMini est premier (1 seul multiple total) on ne peut diviser la surface que d'un seul coté
                    if (FactDimension.IsPremier)
                    {
                        // calcul des dimensions divisées par le multiple
                        int X0 = (int)Math.Ceiling(X / (double)NbMini);
                        int Y0 = (int)Math.Ceiling(Y / (double)NbMini);
                        // on prend celui qui rajoute le moins de tuile par rapport à la surface initiale
                        if (X0 * Y < Y0 * X)
                        {
                            return (NbMini, Reste == 0, X0, NbMini, Y, 1);
                        }
                        else
                        {
                            return (NbMini, Reste == 0, X, 1, Y0, NbMini);
                        }
                    }
                    else
                    {
                        // si il y a 2 multiples ou plus il faut trouver une composition qui donne 1 diviseur pour chaque dimension
                        // et faire en sorte de prendre ceux qui ajoutent le moins de tuiles
                        int Multiple0 = 1;
                        int Multiple1 = 1;
                        for (int Cpt = FactDimension.Multiples.Length - 1; Cpt >= 0; Cpt -= 1)
                        {
                            if (Multiple0 > Multiple1)
                            {
                                Multiple1 *= FactDimension.Multiples[Cpt];
                            }
                            else
                            {
                                Multiple0 *= FactDimension.Multiples[Cpt];
                            }
                        }
                        int X0 = (int)Math.Ceiling(X / (double)Multiple0);
                        int X1 = (int)Math.Ceiling(X / (double)Multiple1);
                        int Y0 = (int)Math.Ceiling(Y / (double)Multiple0);
                        int Y1 = (int)Math.Ceiling(Y / (double)Multiple1);
                        if (X0 * Y1 < X1 * Y0)
                        {
                            return (NbMini, X0 * Multiple0 * Y1 * Multiple1 == Surface, X0, Multiple0, Y1, Multiple1);
                        }
                        else
                        {
                            return (NbMini, X1 * Multiple1 * Y0 * Multiple0 == Surface, X1, Multiple1, Y0, Multiple0);
                        }
                    }
                }
            }
            /// <summary> les 33 premiers nombres Premier. sert à accélérer les calculs et recherches associés aux nbs premiers </summary>
            private static readonly int[] NombresPremiersConnus = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,
                                                                          67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137 };
            private readonly List<int> ListeMultiples;
            /// <summary> tableau des mulitples triés par ordre croissant issus de la décomposition du nb </summary>
            private int[] Multiples
            {
                get
                {
                    return ListeMultiples.ToArray();
                }
            }
            /// <summary> indique si le nb est un nombre premier </summary>
            private bool IsPremier
            {
                get
                {
                    return ListeMultiples.Count == 1;
                }
            }
            /// <summary> nombre initial </summary>
            private readonly int Nombre;
            /// <summary> interdit l'initialisation de la structure en externe </summary>
            private Facteurs()
            {
            }
            /// <summary> initialisation de la structure en interne</summary>
            private Facteurs(int NbInitial)
            {
                if (NbInitial > 1 & NbInitial < 18770)
                {
                    Nombre = NbInitial;
                    ListeMultiples = new List<int>(15);
                    DecomposerNombre();
                }
                else
                {
                    throw new Exception("Le nombre à tester doit être compris entre 2 et 18769 inclus");
                }
            }
            /// <summary> décompose un nombre en multiple de nb premiers </summary>
            private void DecomposerNombre()
            {
                int NbInit = Nombre;
                var IndNbPrem = default(int);
                int Max = (int)Math.Floor(Math.Sqrt(Nombre)); // borne à laquelle s'arrête les nb premiers connus
                do
                {
                    int Result = Math.DivRem(NbInit, NombresPremiersConnus[IndNbPrem], out int Reste);
                    if (Reste == 0) // NbInit est divisible par le nombre premier connu encours
                    {
                        ListeMultiples.Add(NombresPremiersConnus[IndNbPrem]); // on l'ajoute à la liste des multiples
                        if (Result == 1)
                        {
                            return; // on a finit de décomposer NbInit en nombre premier connu on sort
                        }
                        else
                        {
                            NbInit = Result;
                        } // on permute pour avoir la nouvelle valeur sur NbInit
                    }
                    else // NbInit n'est pas divisible par le nombre premier connu encours
                    {
                        IndNbPrem += 1;
                    } // donc on passe au nb premier connu suivant
                }
                while (NombresPremiersConnus[IndNbPrem] <= Max);
                // si ListeMultiples.count = 1 alors NbInit est un nombre premier non connu 
                // sinon c'est un multiple d'un ou plusieurs nombre premier connu et NbInit est Nb Premier connu ou pas
                ListeMultiples.Add(NbInit);
            }
            /// <summary> indique si un nombre est un nombre premier </summary>
            /// <param name="Nombre"> nombre à tester </param>
            private static bool IsNbPremier(int Nombre)
            {
                bool IsNbPremierRet = false;
                if (Nombre > 1 & Nombre < 18770)
                {
                    if (NombresPremiersConnus.Contains(Nombre) || IsNbPremierNonConnu(Nombre))
                        IsNbPremierRet = true;
                }
                else
                {
                    throw new Exception("Le nombre à tester doit être compris entre 2 et 18769 inclus");
                }

                return IsNbPremierRet;
            }
            /// <summary> renvoie true si NbInit est un nombre premier non connu </summary>
            /// <param name="NbInit"> nombre à tester</param>
            private static bool IsNbPremierNonConnu(int NbInit)
            {
                bool IsNbPremierNonConnuRet = false;
                var IndicePremier = default(int);
                int Max = (int)Math.Floor(Math.Sqrt(NbInit));  // borne à laquelle s'arrête les nbs premiers connus pour le nb à tester
                do
                {
                    if (NombresPremiersConnus[IndicePremier] <= Max) // on commence par diviser NBInit par et au max les 33 Nombres Premier connus et stockés
                    {
                        _ = Math.DivRem(NbInit, NombresPremiersConnus[IndicePremier], out int Reste);
                        if (Reste == 0) // Nb à tester est divisible par le nombre premier connu encours
                        {
                            break; // on a finit car le nb à tester est un multiple d'un nb premier connu
                        }
                        else // Nb à tester n'est pas divisible par le nombre premier connu encours
                        {
                            IndicePremier += 1;
                        } // on passe au nb premier connu suivant
                    }
                    else // on a finit car on a passé tous les nb premier inférieur à la racine du nb initial
                    {
                        IsNbPremierNonConnuRet = true; // NbInit est multiple d'un nombre premier connu
                        break;
                    }
                }
                while (IndicePremier != -1); // on fait en sorte que la boucle soit infinie
                return IsNbPremierNonConnuRet;
            }
        }
    }
}