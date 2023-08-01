using static FCGP.AfficheInformation;
using static FCGP.Commun;

namespace FCGP.TRKs
{
    internal partial class TRK
    {
        #region Formulaire Propriétés trace en cours
        private partial class FormProprietesTrace
        {
            private TRK _Trace;
            private int Hauteur, _IndexTrace;
            private List<(int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse)> Segments;
            private int LongueurTot, LongueurTotMontee, DureeTot, DenPositif, DenNegatif;
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
            private int LongueurTotDescente, LongueurTotPlat;
#pragma warning restore IDE0052 // Supprimer les membres privés non lus
            private short AltMin, AltMax;
            private double PenteMoyenne, PenteMaximum, PenteMinimum, VitesseGlobale, VitesseMaximum, VitesseMinimum;
            private Rectangle StockClipCurseur;
            private string Titre;
            /// <summary> pour le passage du paramètre trace </summary>
            internal TRK Trace
            {
                set
                {
                    _Trace = value;
                }
            }
            /// <summary> pour le passage du paramètre index de la trace </summary>
            internal int IndexTrace
            {
                set
                {
                    _IndexTrace = value;
                }
            }

            internal int IndexPoint { get; private set; }
            internal bool Repeindre { get; private set; }

            /// <summary> initialise le formulaire avec les informations d'altitudes et d'horaires si elles existent dans la trace </summary>
            private void ProprieteTrace_Load(object sender, EventArgs e)
            {
                Titre = TitreInformation;
                TitreInformation = "Information Trace";
                StockClipCurseur = Cursor.Clip;
                IndexPoint = -1;
                Hauteur = ProprietesCommunes.Bottom;
                LabelCouleurTrace.BackColor = _Trace.CouleurSegment;
                NomTrace.Text = _Trace.Nom;
                NbPoints.Text = _Trace.NbPoints.ToString();
                if (CalculerStatsTrace())
                {
                    // on affiche la seule stat qui ne dépend ni des altitudes ni des horaires
                    Distance.Text = (LongueurTot / 1000d).ToString("0.00 km");
                    // on affiche les stats qui concerne l'information des altitudes
                    if (_Trace.IsAltitude)
                        MettreAJourStatsAltitude();
                    // on affiche les stats qui concerne l'information des horaires
                    if (_Trace.IsHoraire)
                        MettreAJourStatsHoraire();
                }
                // on rajoute la hauteur des boutons + 2 fois l'écart avec une bordure
                Hauteur += 30 + 2 * 6;
                ClientSize = new Size(ClientSize.Width, Hauteur);
                // met le curseur sur le centre du bouton OK
                Cursor.Position = new Point(Location.X + Ok.Location.X + Ok.Width / 2, Location.Y + Ok.Location.Y + Ok.Height / 2);
                // et limite les déplacements de la souris au formulaire
                Cursor.Clip = new Rectangle(Location, Size);
            }
            private void ProprieteTrace_FormClosed(object sender, FormClosedEventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                TitreInformation = Titre;
            }
            /// <summary> calcul les informations concernant la longeur et le nb de segments. Informations communes </summary>
            private bool CalculerStatsTrace()
            {
                Segments = null;
                if (_Trace.NbSegments > 0)
                    Segments = _Trace.CalculerStatSegmentsTrace(1, _Trace.NbSegments);
                if (Segments is not null)
                {
                    int Cpt = 0;
                    AltMin = 15000;
                    AltMax = -15000;
                    // calcul de toutes les stats qui seront affichées
                    foreach (var Segment in Segments)
                    {
                        Cpt += 1;
                        LongueurTot += Segment.Longueur;
                        DureeTot += Segment.Duree;
                        // si il y a des altitudes 
                        if (Segment.Denivele < 0)
                        {
                            DenNegatif += Segment.Denivele;
                            LongueurTotDescente += Segment.Longueur;
                            if (PenteMinimum > Segment.Pente)
                                PenteMinimum = Segment.Pente;
                            if (VitesseMinimum < Segment.Vitesse)
                                VitesseMinimum = Segment.Vitesse;
                        }
                        else if (Segment.Denivele > 0)
                        {
                            DenPositif += Segment.Denivele;
                            LongueurTotMontee += Segment.Longueur;
                            if (PenteMaximum < Segment.Pente)
                                PenteMaximum = Segment.Pente;
                            if (VitesseMaximum < Segment.Vitesse)
                                VitesseMaximum = Segment.Vitesse;
                        }
                        else
                        {
                            LongueurTotPlat += Segment.Longueur;
                        }
                        short Altitude = _Trace.Altitude(Cpt);
                        if (Altitude < AltMin)
                            AltMin = Altitude;
                        if (Altitude > AltMax)
                            AltMax = Altitude;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary> calcul les informations liées à l'altitude des points. Information facultative </summary>
            private void MettreAJourStatsAltitude()
            {
                // rendre visible le panel altitude
                ProprietesAltitudes.Visible = true;
                // et indiquer la hauteur correspondante
                Hauteur = ProprietesAltitudes.Bottom;
                if (LongueurTotMontee > 0)
                {
                    PenteMoyenne = DenPositif / (double)LongueurTotMontee;
                }
                // et des stats
                DenivelPositif.Text = DenPositif.ToString("0 m");
                DenivelNegatif.Text = DenNegatif.ToString("0 m");
                AltitudeMax.Text = AltMax.ToString("0 m");
                AltitudeMin.Text = AltMin.ToString("0 m");
                PenteMin.Text = PenteMinimum.ToString("p");
                PenteMax.Text = PenteMaximum.ToString("p");
                PenteMoy.Text = PenteMoyenne.ToString("p");
            }
            /// <summary> calcul les informations liées à l'horaire des points. Information facultative </summary>
            private void MettreAJourStatsHoraire()
            {
                if (ProprietesAltitudes.Visible == false)
                {
                    // modifier la location car il n'y a pas de panel concernant les altitudes
                    ProprietesHoraires.Location = new Point(ProprietesHoraires.Left, Hauteur);
                }
                // rendre visible le panel vitesse
                ProprietesHoraires.Visible = true;
                // et ajuster la hauteur en conséquence
                Hauteur = ProprietesHoraires.Bottom;
                if (DureeTot > 0)
                {
                    VitesseGlobale = LongueurTot / (double)DureeTot;
                }
                // et des stats
                Duree.Text = new TimeSpan(0, 0, DureeTot).ToString("t");
                VitesseMoy.Text = (VitesseGlobale * 3.6d).ToString("0.00 km/h");
                VitesseMin.Text = (VitesseMinimum * 3.6d).ToString("0.00 km/h");
                VitesseMax.Text = (VitesseMaximum * 3.6d).ToString("0.00 km/h");
            }
            /// <summary> si le bouton ok est pressé, la touche Enter est appuyée ou un double click sur un des points de graphique Altitude ou vitesse
            /// on prend en compte les modifications </summary>
            private void ProprieteTrace_FormClosing(object sender, FormClosingEventArgs e)
            {
                Repeindre = false;
                if (DialogResult == DialogResult.OK)
                {
                    if ((_Trace.Nom ?? "") != (NomTrace.Text ?? ""))
                    {
                        // si le nom a changé on doit vérifier que le nouveau n'est pas dans la collection
                        if (IsExisteNomTrace(NomTrace.Text))
                        {
                            e.Cancel = true;
                            MessageInformation = "Le nom de la trace existe déjà dans la collection." + CrLf +
                                                 "Veuillez le changer";
                            AfficherInformation();
                            NomTrace.Text = _Trace.Nom;
                            return;
                        }
                        else
                        {
                            RenommerTrace(_IndexTrace, NomTrace.Text);
                        }
                    }
                    if (_Trace.CouleurSegment != LabelCouleurTrace.BackColor)
                    {
                        _Trace.CouleurSegment = LabelCouleurTrace.BackColor;
                        Repeindre = true;
                    }
                }
            }
            /// <summary> Supprime les caractères interdits pour les noms de fichiers </summary>
            private void NomTrace_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (Array.IndexOf(CarsInterdits, e.KeyChar) != -1)
                    e.KeyChar = default;
            }
            /// <summary> Affichage du graphique Altitude ou Vitesse </summary>
            private void Graphique_Click(object sender, EventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                bool FlagVitesse = ((Button)sender).Name == "Vitesse";
                var ret = AfficheGraphique.Invoke(_Trace, FlagVitesse);
                if (ret.DialogResult == DialogResult.OK)
                {
                    IndexPoint = ret.IndexPoint;
                    DialogResult = ret.DialogResult;
                    Close();
                }
                Cursor.Clip = new Rectangle(Location, Size);
            }
            /// <summary> Affiche la boite de dialogue des couleurs </summary>
            private void CouleurTrace_Click(object sender, EventArgs e)
            {
                Cursor.Clip = StockClipCurseur;
                DialogCouleurTrace.Color = LabelCouleurTrace.BackColor;
                DialogCouleurTrace.ShowDialog(this);
                LabelCouleurTrace.BackColor = DialogCouleurTrace.Color;
                Cursor.Clip = new Rectangle(Location, Size);
            }
        }
        private partial class FormProprietesTrace : Form
        {

            internal FormProprietesTrace()
            {
                InitializeComponent();
                InitialiserEvenements();
            }
            private void InitialiserEvenements()
            {
                // contrôles simples
                NomTrace.KeyPress += NomTrace_KeyPress;
                LabelCouleurTrace.Click += CouleurTrace_Click;
                BtAltitudeMax.Click += Graphique_Click;
                BtAltitudeMin.Click += Graphique_Click;
                BtVitesse.Click += Graphique_Click;
                // formulaire
                Load += ProprieteTrace_Load;
                FormClosing += ProprieteTrace_FormClosing;
                FormClosed += ProprieteTrace_FormClosed;
            }
            // Form remplace la méthode Dispose pour nettoyer la liste des composants.
            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && components is not null)
                    {
                        components.Dispose();
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }

            // Requise par le Concepteur Windows Form
            private IContainer components;

            // REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
            // Elle peut être modifiée à l'aide du Concepteur Windows Form.  
            // Ne la modifiez pas à l'aide de l'éditeur de code.
            private void InitializeComponent()
            {
                components = new Container();
                LabelNom = new Label();
                NomTrace = new TextBox();
                LabelCouleurTrace = new Label();
                DialogCouleurTrace = new ColorDialog();
                Ok = new Button();
                PasOk = new Button();
                LabelNbPoints = new Label();
                NbPoints = new Label();
                Distance = new Label();
                LabelDistance = new Label();
                DenivelPositif = new Label();
                CadreBtAltitudeMax = new Label();
                BtAltitudeMax = new Button();
                DenivelNegatif = new Label();
                CadreBtAltitudeMin = new Label();
                BtAltitudeMin = new Button();
                Duree = new Label();
                LabelDuree = new Label();
                VitesseMoy = new Label();
                CadreBtVitesse = new Label();
                BtVitesse = new Button();
                ToolTip1 = new ToolTip(components);
                LabelPente = new Label();
                LabelDenivelNegatif = new Label();
                LabelDenivelPositif = new Label();
                VitesseMax = new Label();
                VitesseMin = new Label();
                PenteMin = new Label();
                PenteMax = new Label();
                AltitudeMax = new Label();
                AltitudeMin = new Label();
                LabelVitesseMin = new Label();
                LabelVitesseMax = new Label();
                LabelVitesseMoyenne = new Label();
                PenteMoy = new Label();
                LabelPenteMoyenne = new Label();
                LabelPenteMin = new Label();
                LabelPenteMax = new Label();
                ProprietesAltitudes = new Panel();
                ProprietesCommunes = new Panel();
                ProprietesHoraires = new Panel();
                Total = new Panel();
                ProprietesAltitudes.SuspendLayout();
                ProprietesCommunes.SuspendLayout();
                ProprietesHoraires.SuspendLayout();
                Total.SuspendLayout();
                SuspendLayout();
                // 
                // LabelNom
                // 
                LabelNom.BackColor = Color.Transparent;
                LabelNom.BorderStyle = BorderStyle.FixedSingle;
                LabelNom.FlatStyle = FlatStyle.Flat;
                LabelNom.Location = new Point(0, 0);
                LabelNom.Name = "LabelNom";
                LabelNom.Size = new Size(238, 23);
                LabelNom.TabIndex = 0;
                LabelNom.Text = "Nom";
                LabelNom.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // NomTrace
                // 
                NomTrace.Location = new Point(0, 22);
                NomTrace.Name = "NomTrace";
                NomTrace.Size = new Size(238, 23);
                NomTrace.TabIndex = 1;
                ToolTip1.SetToolTip(NomTrace, "Nom de la trace." + CrLf +
                                              "Si vous modifiez le nom, le fichier associé sera également renommé.");
                // 
                // LabelCouleurTrace
                // 
                LabelCouleurTrace.BackColor = Color.Transparent;
                LabelCouleurTrace.BorderStyle = BorderStyle.FixedSingle;
                LabelCouleurTrace.FlatStyle = FlatStyle.Flat;
                LabelCouleurTrace.Location = new Point(0, 44);
                LabelCouleurTrace.Name = "LabelCouleurTrace";
                LabelCouleurTrace.Size = new Size(238, 23);
                LabelCouleurTrace.TabIndex = 2;
                ToolTip1.SetToolTip(LabelCouleurTrace, "Couleur de la trace." + CrLf +
                                                       "Cliquez pour choisir une autre couleur.");
                // 
                // LabelNbPoints
                // 
                LabelNbPoints.BackColor = Color.Transparent;
                LabelNbPoints.BorderStyle = BorderStyle.FixedSingle;
                LabelNbPoints.FlatStyle = FlatStyle.Flat;
                LabelNbPoints.Location = new Point(0, 66);
                LabelNbPoints.Name = "LabelNbPoints";
                LabelNbPoints.Size = new Size(119, 23);
                LabelNbPoints.TabIndex = 7;
                LabelNbPoints.Text = "Nb Points :";
                LabelNbPoints.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelDistance
                // 
                LabelDistance.BackColor = Color.Transparent;
                LabelDistance.BorderStyle = BorderStyle.FixedSingle;
                LabelDistance.FlatStyle = FlatStyle.Flat;
                LabelDistance.Location = new Point(118, 66);
                LabelDistance.Name = "LabelDistance";
                LabelDistance.Size = new Size(120, 23);
                LabelDistance.TabIndex = 9;
                LabelDistance.Text = "Distance";
                LabelDistance.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // NbPoints
                // 
                NbPoints.BackColor = Color.Transparent;
                NbPoints.BorderStyle = BorderStyle.FixedSingle;
                NbPoints.FlatStyle = FlatStyle.Flat;
                NbPoints.Location = new Point(0, 88);
                NbPoints.Name = "NbPoints";
                NbPoints.Size = new Size(119, 23);
                NbPoints.TabIndex = 8;
                NbPoints.Text = "0";
                NbPoints.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // Distance
                // 
                Distance.BackColor = Color.Transparent;
                Distance.BorderStyle = BorderStyle.FixedSingle;
                Distance.FlatStyle = FlatStyle.Flat;
                Distance.Location = new Point(118, 88);
                Distance.Name = "Distance";
                Distance.Size = new Size(120, 23);
                Distance.TabIndex = 10;
                Distance.Text = "0";
                Distance.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // ProprietesCommunes
                // 
                ProprietesCommunes.BackColor = Color.Transparent;
                ProprietesCommunes.Controls.Add(LabelNom);
                ProprietesCommunes.Controls.Add(NomTrace);
                ProprietesCommunes.Controls.Add(LabelCouleurTrace);
                ProprietesCommunes.Controls.Add(LabelNbPoints);
                ProprietesCommunes.Controls.Add(NbPoints);
                ProprietesCommunes.Controls.Add(LabelDistance);
                ProprietesCommunes.Controls.Add(Distance);
                ProprietesCommunes.Location = new Point(6, 6);
                ProprietesCommunes.Name = "ProprietesCommunes";
                ProprietesCommunes.Size = new Size(238, 117); // 111+6
                ProprietesCommunes.TabIndex = 41;
                // 
                // LabelPente
                // 
                LabelPente.BackColor = Color.Transparent;
                LabelPente.BorderStyle = BorderStyle.FixedSingle;
                LabelPente.FlatStyle = FlatStyle.Flat;
                LabelPente.Location = new Point(0, 0);
                LabelPente.Name = "LabelPente";
                LabelPente.Size = new Size(238, 23);
                LabelPente.TabIndex = 32;
                LabelPente.Text = "Pente";
                LabelPente.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelPenteMax
                // 
                LabelPenteMax.BackColor = Color.Transparent;
                LabelPenteMax.BorderStyle = BorderStyle.FixedSingle;
                LabelPenteMax.FlatStyle = FlatStyle.Flat;
                LabelPenteMax.Location = new Point(0, 22);
                LabelPenteMax.Name = "LabelPenteMax";
                LabelPenteMax.Size = new Size(80, 23);
                LabelPenteMax.TabIndex = 37;
                LabelPenteMax.Text = "Max +";
                LabelPenteMax.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelPenteMoyenne
                // 
                LabelPenteMoyenne.BackColor = Color.Transparent;
                LabelPenteMoyenne.BorderStyle = BorderStyle.FixedSingle;
                LabelPenteMoyenne.FlatStyle = FlatStyle.Flat;
                LabelPenteMoyenne.Location = new Point(79, 22);
                LabelPenteMoyenne.Name = "LabelPenteMoyenne";
                LabelPenteMoyenne.Size = new Size(80, 23);
                LabelPenteMoyenne.TabIndex = 39;
                LabelPenteMoyenne.Text = "Moy +";
                LabelPenteMoyenne.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelPenteMin
                // 
                LabelPenteMin.BackColor = Color.Transparent;
                LabelPenteMin.BorderStyle = BorderStyle.FixedSingle;
                LabelPenteMin.FlatStyle = FlatStyle.Flat;
                LabelPenteMin.Location = new Point(158, 22);
                LabelPenteMin.Name = "LabelPenteMin";
                LabelPenteMin.Size = new Size(80, 23);
                LabelPenteMin.TabIndex = 38;
                LabelPenteMin.Text = "Max -";
                LabelPenteMin.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // PenteMin
                // 
                PenteMin.BackColor = Color.Transparent;
                PenteMin.BorderStyle = BorderStyle.FixedSingle;
                PenteMin.FlatStyle = FlatStyle.Flat;
                PenteMin.Location = new Point(158, 44);
                PenteMin.Name = "PenteMin";
                PenteMin.Size = new Size(80, 23);
                PenteMin.TabIndex = 28;
                PenteMin.Text = "-----";
                PenteMin.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // PenteMoy
                // 
                PenteMoy.BackColor = Color.Transparent;
                PenteMoy.BorderStyle = BorderStyle.FixedSingle;
                PenteMoy.FlatStyle = FlatStyle.Flat;
                PenteMoy.Location = new Point(79, 44);
                PenteMoy.Name = "PenteMoy";
                PenteMoy.Size = new Size(80, 23);
                PenteMoy.TabIndex = 36;
                PenteMoy.Text = "-----";
                PenteMoy.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // PenteMax
                // 
                PenteMax.BackColor = Color.Transparent;
                PenteMax.BorderStyle = BorderStyle.FixedSingle;
                PenteMax.FlatStyle = FlatStyle.Flat;
                PenteMax.Location = new Point(0, 44);
                PenteMax.Name = "PenteMax";
                PenteMax.Size = new Size(80, 23);
                PenteMax.TabIndex = 27;
                PenteMax.Text = "-----";
                PenteMax.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelDenivelPositif
                // 
                LabelDenivelPositif.BackColor = Color.Transparent;
                LabelDenivelPositif.BorderStyle = BorderStyle.FixedSingle;
                LabelDenivelPositif.FlatStyle = FlatStyle.Flat;
                LabelDenivelPositif.Location = new Point(0, 66);
                LabelDenivelPositif.Name = "LabelDenivelPositif";
                LabelDenivelPositif.Size = new Size(119, 23);
                LabelDenivelPositif.TabIndex = 23;
                LabelDenivelPositif.Text = "Dénivelé +";
                LabelDenivelPositif.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelDenivelNegatif
                // 
                LabelDenivelNegatif.BackColor = Color.Transparent;
                LabelDenivelNegatif.BorderStyle = BorderStyle.FixedSingle;
                LabelDenivelNegatif.FlatStyle = FlatStyle.Flat;
                LabelDenivelNegatif.Location = new Point(118, 66);
                LabelDenivelNegatif.Name = "LabelDenivelNegatif";
                LabelDenivelNegatif.Size = new Size(120, 23);
                LabelDenivelNegatif.TabIndex = 24;
                LabelDenivelNegatif.Text = "Dénivelé - ";
                LabelDenivelNegatif.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // DenivelPositif
                // 
                DenivelPositif.BackColor = Color.Transparent;
                DenivelPositif.BorderStyle = BorderStyle.FixedSingle;
                DenivelPositif.FlatStyle = FlatStyle.Flat;
                DenivelPositif.Location = new Point(0, 88);
                DenivelPositif.Name = "DenivelPositif";
                DenivelPositif.Size = new Size(119, 23);
                DenivelPositif.TabIndex = 12;
                DenivelPositif.Text = "-----";
                DenivelPositif.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // DenivelNegatif
                // 
                DenivelNegatif.BackColor = Color.Transparent;
                DenivelNegatif.BorderStyle = BorderStyle.FixedSingle;
                DenivelNegatif.FlatStyle = FlatStyle.Flat;
                DenivelNegatif.Location = new Point(118, 88);
                DenivelNegatif.Name = "DenivelNegatif";
                DenivelNegatif.Size = new Size(120, 23);
                DenivelNegatif.TabIndex = 14;
                DenivelNegatif.Text = "-----";
                DenivelNegatif.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // CadreBtAltitudeMin
                // 
                CadreBtAltitudeMin.BackColor = Color.Transparent;
                CadreBtAltitudeMin.BorderStyle = BorderStyle.FixedSingle;
                CadreBtAltitudeMin.FlatStyle = FlatStyle.Flat;
                CadreBtAltitudeMin.Location = new Point(0, 110);
                CadreBtAltitudeMin.Name = "CadreBtAltitudeMin";
                CadreBtAltitudeMin.Size = new Size(119, 32);
                CadreBtAltitudeMin.TabIndex = 13;
                // 
                // CadreBtAltitudeMax
                // 
                CadreBtAltitudeMax.BackColor = Color.Transparent;
                CadreBtAltitudeMax.BorderStyle = BorderStyle.FixedSingle;
                CadreBtAltitudeMax.FlatStyle = FlatStyle.Flat;
                CadreBtAltitudeMax.Location = new Point(118, 110);
                CadreBtAltitudeMax.Name = "CadreBtAltitudeMax";
                CadreBtAltitudeMax.Size = new Size(120, 32);
                CadreBtAltitudeMax.TabIndex = 11;
                // 
                // BtAltitudeMin
                // 
                BtAltitudeMin.BackColor = Color.Transparent;
                BtAltitudeMin.Location = new Point(1, 111);
                BtAltitudeMin.Name = "BtAltitudeMin";
                BtAltitudeMin.Size = new Size(117, 30);
                BtAltitudeMin.TabIndex = 40;
                BtAltitudeMin.Text = "Altitude Min";
                ToolTip1.SetToolTip(BtAltitudeMin, "Cliquez pour ouvrir le graphique" + CrLf +
                                                   "Altitude / Distance");
                BtAltitudeMin.UseVisualStyleBackColor = true;
                // 
                // BtAltitudeMax
                // 
                BtAltitudeMax.BackColor = Color.Transparent;
                BtAltitudeMax.Location = new Point(119, 111);
                BtAltitudeMax.Name = "BtAltitudeMax";
                BtAltitudeMax.Size = new Size(118, 30);
                BtAltitudeMax.TabIndex = 41;
                BtAltitudeMax.Text = "Altitude Max";
                ToolTip1.SetToolTip(BtAltitudeMax, "Cliquez pour ouvrir le graphique" + CrLf +
                                                   "Altitude / Distance");
                BtAltitudeMax.UseVisualStyleBackColor = true;
                // 
                // AltitudeMin
                // 
                AltitudeMin.BackColor = Color.Transparent;
                AltitudeMin.BorderStyle = BorderStyle.FixedSingle;
                AltitudeMin.FlatStyle = FlatStyle.Flat;
                AltitudeMin.Location = new Point(0, 141);
                AltitudeMin.Name = "AltitudeMin";
                AltitudeMin.Size = new Size(119, 23);
                AltitudeMin.TabIndex = 29;
                AltitudeMin.Text = "-----";
                AltitudeMin.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // AltitudeMax
                // 
                AltitudeMax.BackColor = Color.Transparent;
                AltitudeMax.BorderStyle = BorderStyle.FixedSingle;
                AltitudeMax.FlatStyle = FlatStyle.Flat;
                AltitudeMax.Location = new Point(118, 141);
                AltitudeMax.Name = "AltitudeMax";
                AltitudeMax.Size = new Size(120, 23);
                AltitudeMax.TabIndex = 30;
                AltitudeMax.Text = "-----";
                AltitudeMax.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // ProprietesAltitudes
                // 
                ProprietesAltitudes.BackColor = Color.Transparent;
                ProprietesAltitudes.Controls.Add(LabelPente);
                ProprietesAltitudes.Controls.Add(LabelPenteMax);
                ProprietesAltitudes.Controls.Add(LabelPenteMoyenne);
                ProprietesAltitudes.Controls.Add(LabelPenteMin);
                ProprietesAltitudes.Controls.Add(PenteMax);
                ProprietesAltitudes.Controls.Add(PenteMoy);
                ProprietesAltitudes.Controls.Add(PenteMin);
                ProprietesAltitudes.Controls.Add(LabelDenivelPositif);
                ProprietesAltitudes.Controls.Add(LabelDenivelNegatif);
                ProprietesAltitudes.Controls.Add(DenivelNegatif);
                ProprietesAltitudes.Controls.Add(DenivelPositif);
                ProprietesAltitudes.Controls.Add(BtAltitudeMax);
                ProprietesAltitudes.Controls.Add(CadreBtAltitudeMax);
                ProprietesAltitudes.Controls.Add(BtAltitudeMin);
                ProprietesAltitudes.Controls.Add(CadreBtAltitudeMin);
                ProprietesAltitudes.Controls.Add(AltitudeMax);
                ProprietesAltitudes.Controls.Add(AltitudeMin);
                ProprietesAltitudes.Location = new Point(6, 123);
                ProprietesAltitudes.Name = "ProprietesAltitudes";
                ProprietesAltitudes.Size = new Size(238, 170); // 164+6
                ProprietesAltitudes.TabIndex = 40;
                ProprietesAltitudes.Visible = false;
                // 
                // LabelDuree
                // 
                LabelDuree.BackColor = Color.Transparent;
                LabelDuree.BorderStyle = BorderStyle.FixedSingle;
                LabelDuree.FlatStyle = FlatStyle.Flat;
                LabelDuree.Location = new Point(0, 0);
                LabelDuree.Name = "LabelDuree";
                LabelDuree.Size = new Size(238, 23);
                LabelDuree.TabIndex = 17;
                LabelDuree.Text = "Durée";
                LabelDuree.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // Duree
                // 
                Duree.BackColor = Color.Transparent;
                Duree.BorderStyle = BorderStyle.FixedSingle;
                Duree.FlatStyle = FlatStyle.Flat;
                Duree.Location = new Point(0, 22);
                Duree.Name = "Duree";
                Duree.Size = new Size(238, 23);
                Duree.TabIndex = 18;
                Duree.Text = "-----";
                Duree.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // Vitesse
                // 
                CadreBtVitesse.BackColor = Color.Transparent;
                CadreBtVitesse.BorderStyle = BorderStyle.FixedSingle;
                CadreBtVitesse.FlatStyle = FlatStyle.Flat;
                CadreBtVitesse.Location = new Point(0, 44);
                CadreBtVitesse.Name = "VitesseLabel";
                CadreBtVitesse.Size = new Size(238, 32);
                CadreBtVitesse.TabIndex = 19;
                // 
                // BtVitesse
                // 
                BtVitesse.BackColor = Color.Transparent;
                BtVitesse.Location = new Point(1, 45);
                BtVitesse.Name = "Vitesse";
                BtVitesse.Size = new Size(236, 30);
                BtVitesse.TabIndex = 42;
                BtVitesse.Text = "Vitesse";
                ToolTip1.SetToolTip(BtVitesse, "Cliquez pour ouvrir le graphique" + CrLf +
                                               "Vitesse / Distance");
                BtVitesse.UseVisualStyleBackColor = true;
                // 
                // LabelVitesseMax
                // 
                LabelVitesseMax.BackColor = Color.Transparent;
                LabelVitesseMax.BorderStyle = BorderStyle.FixedSingle;
                LabelVitesseMax.FlatStyle = FlatStyle.Flat;
                LabelVitesseMax.Location = new Point(0, 75);
                LabelVitesseMax.Name = "LabelVitesseMontee";
                LabelVitesseMax.Size = new Size(80, 23);
                LabelVitesseMax.TabIndex = 33;
                LabelVitesseMax.Text = "Max +";
                LabelVitesseMax.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelVitesseMoyenne
                // 
                LabelVitesseMoyenne.BackColor = Color.Transparent;
                LabelVitesseMoyenne.BorderStyle = BorderStyle.FixedSingle;
                LabelVitesseMoyenne.FlatStyle = FlatStyle.Flat;
                LabelVitesseMoyenne.Location = new Point(79, 75);
                LabelVitesseMoyenne.Name = "LabelVitesseMoyenne";
                LabelVitesseMoyenne.Size = new Size(80, 23);
                LabelVitesseMoyenne.TabIndex = 35;
                LabelVitesseMoyenne.Text = "Moyenne";
                LabelVitesseMoyenne.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // LabelVitesseMin
                // 
                LabelVitesseMin.BackColor = Color.Transparent;
                LabelVitesseMin.BorderStyle = BorderStyle.FixedSingle;
                LabelVitesseMin.FlatStyle = FlatStyle.Flat;
                LabelVitesseMin.Location = new Point(158, 75);
                LabelVitesseMin.Name = "LabelVitesseDescente";
                LabelVitesseMin.Size = new Size(80, 23);
                LabelVitesseMin.TabIndex = 34;
                LabelVitesseMin.Text = "Max -";
                LabelVitesseMin.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // VitesseMax
                // 
                VitesseMax.BackColor = Color.Transparent;
                VitesseMax.BorderStyle = BorderStyle.FixedSingle;
                VitesseMax.FlatStyle = FlatStyle.Flat;
                VitesseMax.Location = new Point(0, 97);
                VitesseMax.Name = "VitesseMax";
                VitesseMax.Size = new Size(80, 23);
                VitesseMax.TabIndex = 26;
                VitesseMax.Text = "-----";
                VitesseMax.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // VitesseMoy
                // 
                VitesseMoy.BackColor = Color.Transparent;
                VitesseMoy.BorderStyle = BorderStyle.FixedSingle;
                VitesseMoy.FlatStyle = FlatStyle.Flat;
                VitesseMoy.Location = new Point(79, 97);
                VitesseMoy.Name = "VitesseMoy";
                VitesseMoy.Size = new Size(80, 23);
                VitesseMoy.TabIndex = 20;
                VitesseMoy.Text = "-----";
                VitesseMoy.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // VitesseMin
                // 
                VitesseMin.BackColor = Color.Transparent;
                VitesseMin.BorderStyle = BorderStyle.FixedSingle;
                VitesseMin.FlatStyle = FlatStyle.Flat;
                VitesseMin.Location = new Point(158, 97);
                VitesseMin.Name = "VitesseMin";
                VitesseMin.Size = new Size(80, 23);
                VitesseMin.TabIndex = 25;
                VitesseMin.Text = "-----";
                VitesseMin.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // ProprietesHoraires
                // 
                ProprietesHoraires.BackColor = Color.Transparent;
                ProprietesHoraires.Controls.Add(LabelDuree);
                ProprietesHoraires.Controls.Add(Duree);
                ProprietesHoraires.Controls.Add(BtVitesse);
                ProprietesHoraires.Controls.Add(CadreBtVitesse);
                ProprietesHoraires.Controls.Add(LabelVitesseMax);
                ProprietesHoraires.Controls.Add(LabelVitesseMoyenne);
                ProprietesHoraires.Controls.Add(LabelVitesseMin);
                ProprietesHoraires.Controls.Add(VitesseMax);
                ProprietesHoraires.Controls.Add(VitesseMoy);
                ProprietesHoraires.Controls.Add(VitesseMin);
                ProprietesHoraires.Location = new Point(6, 293);
                ProprietesHoraires.Name = "ProprietesHoraires";
                ProprietesHoraires.Size = new Size(238, 126); // 120 + 6
                ProprietesHoraires.TabIndex = 42;
                ProprietesHoraires.Visible = false;
                // 
                // Total
                // 
                Total.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Total.BackColor = Color.FromArgb(247, 247, 247);
                Total.Controls.Add(ProprietesCommunes);
                Total.Controls.Add(ProprietesAltitudes);
                Total.Controls.Add(ProprietesHoraires);
                Total.Location = new Point(0, 0);
                Total.Name = "Total";
                Total.Size = new Size(250, 419);
                Total.TabIndex = 43;
                // 
                // PasOk
                // 
                PasOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                PasOk.DialogResult = DialogResult.Cancel;
                PasOk.Location = new Point(68, 425);
                PasOk.Name = "PasOk";
                PasOk.Size = new Size(80, 30);
                PasOk.TabIndex = 6;
                PasOk.Text = "Annuler";
                ToolTip1.SetToolTip(PasOk, "Annule les modifications éventuelles de nom et de couleur.");
                PasOk.UseVisualStyleBackColor = true;
                // 
                // Ok
                // 
                Ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                Ok.DialogResult = DialogResult.OK;
                Ok.Location = new Point(154, 425);
                Ok.Name = "Ok";
                Ok.Size = new Size(90, 30);
                Ok.TabIndex = 5;
                Ok.Text = "Valider";
                ToolTip1.SetToolTip(Ok, "Valide les modifications éventuelles de nom et de couleur.");
                Ok.UseVisualStyleBackColor = true;

                // 
                // ColorTrace
                // 
                DialogCouleurTrace.AllowFullOpen = false;
                DialogCouleurTrace.AnyColor = true;
                DialogCouleurTrace.SolidColorOnly = true;
                // 
                // ToolTip1
                // 
                ToolTip1.AutomaticDelay = 100;
                ToolTip1.ShowAlways = true;
                // 
                // ProprietesTrace
                // 
                AcceptButton = Ok;
                AutoScaleMode = AutoScaleMode.None;
                CancelButton = PasOk;
                ClientSize = new Size(250, 461);
                ControlBox = false;
                Controls.Add(Ok);
                Controls.Add(PasOk);
                Controls.Add(Total);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                StartPosition = FormStartPosition.CenterParent;
                Name = "ProprietesTrace";
                ProprietesAltitudes.ResumeLayout(false);
                ProprietesCommunes.ResumeLayout(false);
                ProprietesCommunes.PerformLayout();
                ProprietesHoraires.ResumeLayout(false);
                Total.ResumeLayout(false);
                ResumeLayout(false);
            }

            private Label LabelNom;
            private TextBox NomTrace;
            private Label LabelCouleurTrace;
            private ColorDialog DialogCouleurTrace;
            private Button Ok;
            private Button PasOk;
            private Label LabelNbPoints;
            private Label NbPoints;
            private Label Distance;
            private Label LabelDistance;
            private Label DenivelPositif;
            private Label CadreBtAltitudeMax;
            private Label DenivelNegatif;
            private Label CadreBtAltitudeMin;
            private Label Duree;
            private Label LabelDuree;
            private Label VitesseMoy;
            private Label CadreBtVitesse;
            private ToolTip ToolTip1;
            private Label LabelDenivelNegatif;
            private Label LabelDenivelPositif;
            private Label VitesseMax;
            private Label VitesseMin;
            private Label PenteMin;
            private Label PenteMax;
            private Label AltitudeMax;
            private Label AltitudeMin;
            private Label LabelPente;
            private Label LabelVitesseMin;
            private Label LabelVitesseMax;
            private Label LabelVitesseMoyenne;
            private Label PenteMoy;
            private Label LabelPenteMoyenne;
            private Label LabelPenteMin;
            private Label LabelPenteMax;
            internal Button BtAltitudeMax;
            internal Button BtAltitudeMin;
            internal Button BtVitesse;
            private Panel ProprietesAltitudes;
            private Panel ProprietesCommunes;
            private Panel ProprietesHoraires;
            private Panel Total;
        }
        #endregion
    }
}