Namespace TRKs
    Partial Friend Class TRK
#Region "Formulaire Propriétés trace en cours"
        Private Class FormProprietesTrace
            Private _Trace As TRK
            Private Hauteur, _IndexTrace As Integer
            Private Segments As List(Of (Longueur As Integer, Denivele As Integer, Duree As Integer, Cap As Double, Pente As Double, Vitesse As Double))
            Private LongueurTot, LongueurTotMontee, DureeTot, DenPositif, DenNegatif As Integer, AltMin, AltMax As Short
#Disable Warning IDE0052 ' Supprimer les membres privés non lus
            Private LongueurTotDescente, LongueurTotPlat As Integer
#Enable Warning IDE0052 ' Supprimer les membres privés non lus
            Private PenteMoyenne, PenteMaximum, PenteMinimum, VitesseGlobale, VitesseMaximum, VitesseMinimum As Double
            Private StockClipCurseur As Rectangle
            Private Titre As String
            ''' <summary> pour le passage du paramètre trace </summary>
            Friend WriteOnly Property Trace As TRK
                Set(value As TRK)
                    _Trace = value
                End Set
            End Property
            ''' <summary> pour le passage du paramètre index de la trace </summary>
            Friend WriteOnly Property IndexTrace As Integer
                Set(value As Integer)
                    _IndexTrace = value
                End Set
            End Property

            Friend ReadOnly Property IndexPoint As Integer
            Friend ReadOnly Property Repeindre As Boolean

            ''' <summary> initialise le formulaire avec les informations d'altitudes et d'horaires si elles existent dans la trace </summary>
            Private Sub ProprieteTrace_Load(sender As Object, e As EventArgs)
                Titre = TitreInformation
                TitreInformation = "Information Trace"
                StockClipCurseur = Cursor.Clip
                _IndexPoint = -1
                Hauteur = ProprietesCommunes.Bottom
                LabelCouleurTrace.BackColor = _Trace.CouleurSegment
                NomTrace.Text = _Trace.Nom
                NbPoints.Text = _Trace.NbPoints.ToString
                If CalculerStatsTrace() Then
                    'on affiche la seule stat qui ne dépend ni des altitudes ni des horaires
                    Distance.Text = (LongueurTot / 1000).ToString("0.00 km")
                    'on affiche les stats qui concerne l'information des altitudes
                    If _Trace.IsAltitude Then MettreAJourStatsAltitude()
                    'on affiche les stats qui concerne l'information des horaires
                    If _Trace.IsHoraire Then MettreAJourStatsHoraire()
                End If
                'on rajoute la hauteur des boutons + 2 fois l'écart avec une bordure
                Hauteur += (30 + 2 * 6)
                ClientSize = New Size(ClientSize.Width, Hauteur)
                'met le curseur sur le centre du bouton OK
                Cursor.Position = New Point(Location.X + Ok.Location.X + Ok.Width \ 2, Location.Y + Ok.Location.Y + Ok.Height \ 2)
                'et limite les déplacements de la souris au formulaire
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub
            Private Sub ProprieteTrace_FormClosed(sender As Object, e As FormClosedEventArgs)
                Cursor.Clip = StockClipCurseur
                TitreInformation = Titre
            End Sub
            ''' <summary> calcul les informations concernant la longeur et le nb de segments. Informations communes </summary>
            Private Function CalculerStatsTrace() As Boolean
                Segments = Nothing
                If _Trace.NbSegments > 0 Then Segments = _Trace.CalculerStatSegmentsTrace(1, _Trace.NbSegments)
                If Segments IsNot Nothing Then
                    Dim Cpt As Integer = 0
                    AltMin = 15000
                    AltMax = -15000
                    'calcul de toutes les stats qui seront affichées
                    For Each Segment In Segments
                        Cpt += 1
                        LongueurTot += Segment.Longueur
                        DureeTot += Segment.Duree
                        'si il y a des altitudes 
                        If Segment.Denivele < 0 Then
                            DenNegatif += Segment.Denivele
                            LongueurTotDescente += Segment.Longueur
                            If PenteMinimum > Segment.Pente Then PenteMinimum = Segment.Pente
                            If VitesseMinimum < Segment.Vitesse Then VitesseMinimum = Segment.Vitesse
                        ElseIf Segment.Denivele > 0 Then
                            DenPositif += Segment.Denivele
                            LongueurTotMontee += Segment.Longueur
                            If PenteMaximum < Segment.Pente Then PenteMaximum = Segment.Pente
                            If VitesseMaximum < Segment.Vitesse Then VitesseMaximum = Segment.Vitesse
                        Else
                            LongueurTotPlat += Segment.Longueur
                        End If
                        Dim Altitude As Short = _Trace.Altitude(Cpt)
                        If Altitude < AltMin Then AltMin = Altitude
                        If Altitude > AltMax Then AltMax = Altitude
                    Next
                    Return True
                Else
                    Return False
                End If
            End Function
            ''' <summary> calcul les informations liées à l'altitude des points. Information facultative </summary>
            Private Sub MettreAJourStatsAltitude()
                'rendre visible le panel altitude
                ProprietesAltitudes.Visible = True
                'et indiquer la hauteur correspondante
                Hauteur = ProprietesAltitudes.Bottom
                If LongueurTotMontee > 0 Then
                    PenteMoyenne = DenPositif / LongueurTotMontee
                End If
                'et des stats
                DenivelPositif.Text = DenPositif.ToString("0 m")
                DenivelNegatif.Text = DenNegatif.ToString("0 m")
                AltitudeMax.Text = AltMax.ToString("0 m")
                AltitudeMin.Text = AltMin.ToString("0 m")
                PenteMin.Text = PenteMinimum.ToString("p")
                PenteMax.Text = PenteMaximum.ToString("p")
                PenteMoy.Text = PenteMoyenne.ToString("p")
            End Sub
            ''' <summary> calcul les informations liées à l'horaire des points. Information facultative </summary>
            Private Sub MettreAJourStatsHoraire()
                If ProprietesAltitudes.Visible = False Then
                    'modifier la location car il n'y a pas de panel concernant les altitudes
                    ProprietesHoraires.Location = (New Point(ProprietesHoraires.Left, Hauteur))
                End If
                'rendre visible le panel vitesse
                ProprietesHoraires.Visible = True
                'et ajuster la hauteur en conséquence
                Hauteur = ProprietesHoraires.Bottom
                If DureeTot > 0 Then
                    VitesseGlobale = LongueurTot / DureeTot
                End If
                'et des stats
                Duree.Text = New TimeSpan(0, 0, DureeTot).ToString("t")
                VitesseMoy.Text = (VitesseGlobale * 3.6).ToString("0.00 km/h")
                VitesseMin.Text = (VitesseMinimum * 3.6).ToString("0.00 km/h")
                VitesseMax.Text = (VitesseMaximum * 3.6).ToString("0.00 km/h")
            End Sub
            ''' <summary> si le bouton ok est pressé, la touche Enter est appuyée ou un double click sur un des points de graphique Altitude ou vitesse
            ''' on prend en compte les modifications </summary>
            Private Sub ProprieteTrace_FormClosing(sender As Object, e As FormClosingEventArgs)
                _Repeindre = False
                If DialogResult = DialogResult.OK Then
                    If _Trace.Nom <> NomTrace.Text Then
                        'si le nom a changé on doit vérifier que le nouveau n'est pas dans la collection
                        If IsExisteNomTrace(NomTrace.Text) Then
                            e.Cancel = True
                            MessageInformation = "Le nom de la trace existe déjà dans la collection." & CrLf &
                                                 "Veuillez le changer"
                            AfficherInformation()
                            NomTrace.Text = _Trace.Nom
                            Exit Sub
                        Else
                            RenommerTrace(_IndexTrace, NomTrace.Text)
                        End If
                    End If
                    If _Trace.CouleurSegment <> LabelCouleurTrace.BackColor Then
                        _Trace.CouleurSegment = LabelCouleurTrace.BackColor
                        _Repeindre = True
                    End If
                End If
            End Sub
            ''' <summary> Supprime les caractères interdits pour les noms de fichiers </summary>
            Private Sub NomTrace_KeyPress(sender As Object, e As KeyPressEventArgs)
                If Array.IndexOf(CarsInterdits, e.KeyChar) <> -1 Then e.KeyChar = Nothing
            End Sub
            ''' <summary> Affichage du graphique Altitude ou Vitesse </summary>
            Private Sub Graphique_Click(sender As Object, e As EventArgs)
                Cursor.Clip = StockClipCurseur
                Dim FlagVitesse = CType(sender, Button).Name = "Vitesse"
                Dim ret = AfficheGraphique.Invoke(_Trace, FlagVitesse)
                If ret.DialogResult = DialogResult.OK Then
                    _IndexPoint = ret.IndexPoint
                    DialogResult = ret.DialogResult
                    Close()
                End If
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub
            ''' <summary> Affiche la boite de dialogue des couleurs </summary>
            Private Sub CouleurTrace_Click(sender As Object, e As EventArgs)
                Cursor.Clip = StockClipCurseur
                DialogCouleurTrace.Color = LabelCouleurTrace.BackColor
                DialogCouleurTrace.ShowDialog(Me)
                LabelCouleurTrace.BackColor = DialogCouleurTrace.Color
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub
        End Class
        Partial Private Class FormProprietesTrace
            Inherits Form

            Friend Sub New()
                InitializeComponent()
                InitialiserEvenements()
            End Sub
            Private Sub InitialiserEvenements()
                'contrôles simples
                AddHandler NomTrace.KeyPress, AddressOf NomTrace_KeyPress
                AddHandler LabelCouleurTrace.Click, AddressOf CouleurTrace_Click
                AddHandler BtAltitudeMax.Click, AddressOf Graphique_Click
                AddHandler BtAltitudeMin.Click, AddressOf Graphique_Click
                AddHandler BtVitesse.Click, AddressOf Graphique_Click
                'formulaire
                AddHandler Load, AddressOf ProprieteTrace_Load
                AddHandler FormClosing, AddressOf ProprieteTrace_FormClosing
                AddHandler FormClosed, AddressOf ProprieteTrace_FormClosed
            End Sub
            'Form remplace la méthode Dispose pour nettoyer la liste des composants.
            Protected Overrides Sub Dispose(disposing As Boolean)
                Try
                    If disposing AndAlso components IsNot Nothing Then
                        components.Dispose()
                    End If
                Finally
                    MyBase.Dispose(disposing)
                End Try
            End Sub

            'Requise par le Concepteur Windows Form
            Private components As IContainer

            'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
            'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
            'Ne la modifiez pas à l'aide de l'éditeur de code.
            Private Sub InitializeComponent()
                components = New Container()
                LabelNom = New Label()
                NomTrace = New TextBox()
                LabelCouleurTrace = New Label()
                DialogCouleurTrace = New ColorDialog()
                Ok = New Button()
                PasOk = New Button()
                LabelNbPoints = New Label()
                NbPoints = New Label()
                Distance = New Label()
                LabelDistance = New Label()
                DenivelPositif = New Label()
                CadreBtAltitudeMax = New Label()
                BtAltitudeMax = New Button()
                DenivelNegatif = New Label()
                CadreBtAltitudeMin = New Label()
                BtAltitudeMin = New Button()
                Duree = New Label()
                LabelDuree = New Label()
                VitesseMoy = New Label()
                CadreBtVitesse = New Label()
                BtVitesse = New Button()
                ToolTip1 = New ToolTip(components)
                LabelPente = New Label()
                LabelDenivelNegatif = New Label()
                LabelDenivelPositif = New Label()
                VitesseMax = New Label()
                VitesseMin = New Label()
                PenteMin = New Label()
                PenteMax = New Label()
                AltitudeMax = New Label()
                AltitudeMin = New Label()
                LabelVitesseMin = New Label()
                LabelVitesseMax = New Label()
                LabelVitesseMoyenne = New Label()
                PenteMoy = New Label()
                LabelPenteMoyenne = New Label()
                LabelPenteMin = New Label()
                LabelPenteMax = New Label()
                ProprietesAltitudes = New Panel()
                ProprietesCommunes = New Panel()
                ProprietesHoraires = New Panel()
                Total = New Panel()
                ProprietesAltitudes.SuspendLayout()
                ProprietesCommunes.SuspendLayout()
                ProprietesHoraires.SuspendLayout()
                Total.SuspendLayout()
                SuspendLayout()
                '
                'LabelNom
                '
                LabelNom.BackColor = Color.Transparent
                LabelNom.BorderStyle = BorderStyle.FixedSingle
                LabelNom.FlatStyle = FlatStyle.Flat
                LabelNom.Location = New Point(0, 0)
                LabelNom.Name = "LabelNom"
                LabelNom.Size = New Size(238, 23)
                LabelNom.TabIndex = 0
                LabelNom.Text = "Nom"
                LabelNom.TextAlign = ContentAlignment.MiddleCenter
                '
                'NomTrace
                '
                NomTrace.Location = New Point(0, 22)
                NomTrace.Name = "NomTrace"
                NomTrace.Size = New Size(238, 23)
                NomTrace.TabIndex = 1
                ToolTip1.SetToolTip(NomTrace, "Nom de la trace." & CrLf &
                                              "Si vous modifiez le nom, le fichier associé sera également renommé.")
                '
                'LabelCouleurTrace
                '
                LabelCouleurTrace.BackColor = Color.Transparent
                LabelCouleurTrace.BorderStyle = BorderStyle.FixedSingle
                LabelCouleurTrace.FlatStyle = FlatStyle.Flat
                LabelCouleurTrace.Location = New Point(0, 44)
                LabelCouleurTrace.Name = "LabelCouleurTrace"
                LabelCouleurTrace.Size = New Size(238, 23)
                LabelCouleurTrace.TabIndex = 2
                ToolTip1.SetToolTip(LabelCouleurTrace, "Couleur de la trace." & CrLf &
                                                       "Cliquez pour choisir une autre couleur.")
                '
                'LabelNbPoints
                '
                LabelNbPoints.BackColor = Color.Transparent
                LabelNbPoints.BorderStyle = BorderStyle.FixedSingle
                LabelNbPoints.FlatStyle = FlatStyle.Flat
                LabelNbPoints.Location = New Point(0, 66)
                LabelNbPoints.Name = "LabelNbPoints"
                LabelNbPoints.Size = New Size(119, 23)
                LabelNbPoints.TabIndex = 7
                LabelNbPoints.Text = "Nb Points :"
                LabelNbPoints.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelDistance
                '
                LabelDistance.BackColor = Color.Transparent
                LabelDistance.BorderStyle = BorderStyle.FixedSingle
                LabelDistance.FlatStyle = FlatStyle.Flat
                LabelDistance.Location = New Point(118, 66)
                LabelDistance.Name = "LabelDistance"
                LabelDistance.Size = New Size(120, 23)
                LabelDistance.TabIndex = 9
                LabelDistance.Text = "Distance"
                LabelDistance.TextAlign = ContentAlignment.MiddleCenter
                '
                'NbPoints
                '
                NbPoints.BackColor = Color.Transparent
                NbPoints.BorderStyle = BorderStyle.FixedSingle
                NbPoints.FlatStyle = FlatStyle.Flat
                NbPoints.Location = New Point(0, 88)
                NbPoints.Name = "NbPoints"
                NbPoints.Size = New Size(119, 23)
                NbPoints.TabIndex = 8
                NbPoints.Text = "0"
                NbPoints.TextAlign = ContentAlignment.MiddleCenter
                '
                'Distance
                '
                Distance.BackColor = Color.Transparent
                Distance.BorderStyle = BorderStyle.FixedSingle
                Distance.FlatStyle = FlatStyle.Flat
                Distance.Location = New Point(118, 88)
                Distance.Name = "Distance"
                Distance.Size = New Size(120, 23)
                Distance.TabIndex = 10
                Distance.Text = "0"
                Distance.TextAlign = ContentAlignment.MiddleCenter
                '
                'ProprietesCommunes
                '
                ProprietesCommunes.BackColor = Color.Transparent
                ProprietesCommunes.Controls.Add(LabelNom)
                ProprietesCommunes.Controls.Add(NomTrace)
                ProprietesCommunes.Controls.Add(LabelCouleurTrace)
                ProprietesCommunes.Controls.Add(LabelNbPoints)
                ProprietesCommunes.Controls.Add(NbPoints)
                ProprietesCommunes.Controls.Add(LabelDistance)
                ProprietesCommunes.Controls.Add(Distance)
                ProprietesCommunes.Location = New Point(6, 6)
                ProprietesCommunes.Name = "ProprietesCommunes"
                ProprietesCommunes.Size = New Size(238, 117) '111+6
                ProprietesCommunes.TabIndex = 41
                '
                'LabelPente
                '
                LabelPente.BackColor = Color.Transparent
                LabelPente.BorderStyle = BorderStyle.FixedSingle
                LabelPente.FlatStyle = FlatStyle.Flat
                LabelPente.Location = New Point(0, 0)
                LabelPente.Name = "LabelPente"
                LabelPente.Size = New Size(238, 23)
                LabelPente.TabIndex = 32
                LabelPente.Text = "Pente"
                LabelPente.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelPenteMax
                '
                LabelPenteMax.BackColor = Color.Transparent
                LabelPenteMax.BorderStyle = BorderStyle.FixedSingle
                LabelPenteMax.FlatStyle = FlatStyle.Flat
                LabelPenteMax.Location = New Point(0, 22)
                LabelPenteMax.Name = "LabelPenteMax"
                LabelPenteMax.Size = New Size(80, 23)
                LabelPenteMax.TabIndex = 37
                LabelPenteMax.Text = "Max +"
                LabelPenteMax.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelPenteMoyenne
                '
                LabelPenteMoyenne.BackColor = Color.Transparent
                LabelPenteMoyenne.BorderStyle = BorderStyle.FixedSingle
                LabelPenteMoyenne.FlatStyle = FlatStyle.Flat
                LabelPenteMoyenne.Location = New Point(79, 22)
                LabelPenteMoyenne.Name = "LabelPenteMoyenne"
                LabelPenteMoyenne.Size = New Size(80, 23)
                LabelPenteMoyenne.TabIndex = 39
                LabelPenteMoyenne.Text = "Moy +"
                LabelPenteMoyenne.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelPenteMin
                '
                LabelPenteMin.BackColor = Color.Transparent
                LabelPenteMin.BorderStyle = BorderStyle.FixedSingle
                LabelPenteMin.FlatStyle = FlatStyle.Flat
                LabelPenteMin.Location = New Point(158, 22)
                LabelPenteMin.Name = "LabelPenteMin"
                LabelPenteMin.Size = New Size(80, 23)
                LabelPenteMin.TabIndex = 38
                LabelPenteMin.Text = "Max -"
                LabelPenteMin.TextAlign = ContentAlignment.MiddleCenter
                '
                'PenteMin
                '
                PenteMin.BackColor = Color.Transparent
                PenteMin.BorderStyle = BorderStyle.FixedSingle
                PenteMin.FlatStyle = FlatStyle.Flat
                PenteMin.Location = New Point(158, 44)
                PenteMin.Name = "PenteMin"
                PenteMin.Size = New Size(80, 23)
                PenteMin.TabIndex = 28
                PenteMin.Text = "-----"
                PenteMin.TextAlign = ContentAlignment.MiddleCenter
                '
                'PenteMoy
                '
                PenteMoy.BackColor = Color.Transparent
                PenteMoy.BorderStyle = BorderStyle.FixedSingle
                PenteMoy.FlatStyle = FlatStyle.Flat
                PenteMoy.Location = New Point(79, 44)
                PenteMoy.Name = "PenteMoy"
                PenteMoy.Size = New Size(80, 23)
                PenteMoy.TabIndex = 36
                PenteMoy.Text = "-----"
                PenteMoy.TextAlign = ContentAlignment.MiddleCenter
                '
                'PenteMax
                '
                PenteMax.BackColor = Color.Transparent
                PenteMax.BorderStyle = BorderStyle.FixedSingle
                PenteMax.FlatStyle = FlatStyle.Flat
                PenteMax.Location = New Point(0, 44)
                PenteMax.Name = "PenteMax"
                PenteMax.Size = New Size(80, 23)
                PenteMax.TabIndex = 27
                PenteMax.Text = "-----"
                PenteMax.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelDenivelPositif
                '
                LabelDenivelPositif.BackColor = Color.Transparent
                LabelDenivelPositif.BorderStyle = BorderStyle.FixedSingle
                LabelDenivelPositif.FlatStyle = FlatStyle.Flat
                LabelDenivelPositif.Location = New Point(0, 66)
                LabelDenivelPositif.Name = "LabelDenivelPositif"
                LabelDenivelPositif.Size = New Size(119, 23)
                LabelDenivelPositif.TabIndex = 23
                LabelDenivelPositif.Text = "Dénivelé +"
                LabelDenivelPositif.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelDenivelNegatif
                '
                LabelDenivelNegatif.BackColor = Color.Transparent
                LabelDenivelNegatif.BorderStyle = BorderStyle.FixedSingle
                LabelDenivelNegatif.FlatStyle = FlatStyle.Flat
                LabelDenivelNegatif.Location = New Point(118, 66)
                LabelDenivelNegatif.Name = "LabelDenivelNegatif"
                LabelDenivelNegatif.Size = New Size(120, 23)
                LabelDenivelNegatif.TabIndex = 24
                LabelDenivelNegatif.Text = "Dénivelé - "
                LabelDenivelNegatif.TextAlign = ContentAlignment.MiddleCenter
                '
                'DenivelPositif
                '
                DenivelPositif.BackColor = Color.Transparent
                DenivelPositif.BorderStyle = BorderStyle.FixedSingle
                DenivelPositif.FlatStyle = FlatStyle.Flat
                DenivelPositif.Location = New Point(0, 88)
                DenivelPositif.Name = "DenivelPositif"
                DenivelPositif.Size = New Size(119, 23)
                DenivelPositif.TabIndex = 12
                DenivelPositif.Text = "-----"
                DenivelPositif.TextAlign = ContentAlignment.MiddleCenter
                '
                'DenivelNegatif
                '
                DenivelNegatif.BackColor = Color.Transparent
                DenivelNegatif.BorderStyle = BorderStyle.FixedSingle
                DenivelNegatif.FlatStyle = FlatStyle.Flat
                DenivelNegatif.Location = New Point(118, 88)
                DenivelNegatif.Name = "DenivelNegatif"
                DenivelNegatif.Size = New Size(120, 23)
                DenivelNegatif.TabIndex = 14
                DenivelNegatif.Text = "-----"
                DenivelNegatif.TextAlign = ContentAlignment.MiddleCenter
                '
                'CadreBtAltitudeMin
                '
                CadreBtAltitudeMin.BackColor = Color.Transparent
                CadreBtAltitudeMin.BorderStyle = BorderStyle.FixedSingle
                CadreBtAltitudeMin.FlatStyle = FlatStyle.Flat
                CadreBtAltitudeMin.Location = New Point(0, 110)
                CadreBtAltitudeMin.Name = "CadreBtAltitudeMin"
                CadreBtAltitudeMin.Size = New Size(119, 32)
                CadreBtAltitudeMin.TabIndex = 13
                '
                'CadreBtAltitudeMax
                '
                CadreBtAltitudeMax.BackColor = Color.Transparent
                CadreBtAltitudeMax.BorderStyle = BorderStyle.FixedSingle
                CadreBtAltitudeMax.FlatStyle = FlatStyle.Flat
                CadreBtAltitudeMax.Location = New Point(118, 110)
                CadreBtAltitudeMax.Name = "CadreBtAltitudeMax"
                CadreBtAltitudeMax.Size = New Size(120, 32)
                CadreBtAltitudeMax.TabIndex = 11
                '
                'BtAltitudeMin
                '
                BtAltitudeMin.BackColor = Color.Transparent
                BtAltitudeMin.Location = New Point(1, 111)
                BtAltitudeMin.Name = "BtAltitudeMin"
                BtAltitudeMin.Size = New Size(117, 30)
                BtAltitudeMin.TabIndex = 40
                BtAltitudeMin.Text = "Altitude Min"
                ToolTip1.SetToolTip(BtAltitudeMin, "Cliquez pour ouvrir le graphique" & CrLf &
                                                   "Altitude / Distance")
                BtAltitudeMin.UseVisualStyleBackColor = True
                '
                'BtAltitudeMax
                '
                BtAltitudeMax.BackColor = Color.Transparent
                BtAltitudeMax.Location = New Point(119, 111)
                BtAltitudeMax.Name = "BtAltitudeMax"
                BtAltitudeMax.Size = New Size(118, 30)
                BtAltitudeMax.TabIndex = 41
                BtAltitudeMax.Text = "Altitude Max"
                ToolTip1.SetToolTip(BtAltitudeMax, "Cliquez pour ouvrir le graphique" & CrLf &
                                                   "Altitude / Distance")
                BtAltitudeMax.UseVisualStyleBackColor = True
                '
                'AltitudeMin
                '
                AltitudeMin.BackColor = Color.Transparent
                AltitudeMin.BorderStyle = BorderStyle.FixedSingle
                AltitudeMin.FlatStyle = FlatStyle.Flat
                AltitudeMin.Location = New Point(0, 141)
                AltitudeMin.Name = "AltitudeMin"
                AltitudeMin.Size = New Size(119, 23)
                AltitudeMin.TabIndex = 29
                AltitudeMin.Text = "-----"
                AltitudeMin.TextAlign = ContentAlignment.MiddleCenter
                '
                'AltitudeMax
                '
                AltitudeMax.BackColor = Color.Transparent
                AltitudeMax.BorderStyle = BorderStyle.FixedSingle
                AltitudeMax.FlatStyle = FlatStyle.Flat
                AltitudeMax.Location = New Point(118, 141)
                AltitudeMax.Name = "AltitudeMax"
                AltitudeMax.Size = New Size(120, 23)
                AltitudeMax.TabIndex = 30
                AltitudeMax.Text = "-----"
                AltitudeMax.TextAlign = ContentAlignment.MiddleCenter
                '
                'ProprietesAltitudes
                '
                ProprietesAltitudes.BackColor = Color.Transparent
                ProprietesAltitudes.Controls.Add(LabelPente)
                ProprietesAltitudes.Controls.Add(LabelPenteMax)
                ProprietesAltitudes.Controls.Add(LabelPenteMoyenne)
                ProprietesAltitudes.Controls.Add(LabelPenteMin)
                ProprietesAltitudes.Controls.Add(PenteMax)
                ProprietesAltitudes.Controls.Add(PenteMoy)
                ProprietesAltitudes.Controls.Add(PenteMin)
                ProprietesAltitudes.Controls.Add(LabelDenivelPositif)
                ProprietesAltitudes.Controls.Add(LabelDenivelNegatif)
                ProprietesAltitudes.Controls.Add(DenivelNegatif)
                ProprietesAltitudes.Controls.Add(DenivelPositif)
                ProprietesAltitudes.Controls.Add(BtAltitudeMax)
                ProprietesAltitudes.Controls.Add(CadreBtAltitudeMax)
                ProprietesAltitudes.Controls.Add(BtAltitudeMin)
                ProprietesAltitudes.Controls.Add(CadreBtAltitudeMin)
                ProprietesAltitudes.Controls.Add(AltitudeMax)
                ProprietesAltitudes.Controls.Add(AltitudeMin)
                ProprietesAltitudes.Location = New Point(6, 123)
                ProprietesAltitudes.Name = "ProprietesAltitudes"
                ProprietesAltitudes.Size = New Size(238, 170) '164+6
                ProprietesAltitudes.TabIndex = 40
                ProprietesAltitudes.Visible = False
                '
                'LabelDuree
                '
                LabelDuree.BackColor = Color.Transparent
                LabelDuree.BorderStyle = BorderStyle.FixedSingle
                LabelDuree.FlatStyle = FlatStyle.Flat
                LabelDuree.Location = New Point(0, 0)
                LabelDuree.Name = "LabelDuree"
                LabelDuree.Size = New Size(238, 23)
                LabelDuree.TabIndex = 17
                LabelDuree.Text = "Durée"
                LabelDuree.TextAlign = ContentAlignment.MiddleCenter
                '
                'Duree
                '
                Duree.BackColor = Color.Transparent
                Duree.BorderStyle = BorderStyle.FixedSingle
                Duree.FlatStyle = FlatStyle.Flat
                Duree.Location = New Point(0, 22)
                Duree.Name = "Duree"
                Duree.Size = New Size(238, 23)
                Duree.TabIndex = 18
                Duree.Text = "-----"
                Duree.TextAlign = ContentAlignment.MiddleCenter
                '
                'Vitesse
                '
                CadreBtVitesse.BackColor = Color.Transparent
                CadreBtVitesse.BorderStyle = BorderStyle.FixedSingle
                CadreBtVitesse.FlatStyle = FlatStyle.Flat
                CadreBtVitesse.Location = New Point(0, 44)
                CadreBtVitesse.Name = "VitesseLabel"
                CadreBtVitesse.Size = New Size(238, 32)
                CadreBtVitesse.TabIndex = 19
                '
                'BtVitesse
                '
                BtVitesse.BackColor = Color.Transparent
                BtVitesse.Location = New Point(1, 45)
                BtVitesse.Name = "Vitesse"
                BtVitesse.Size = New Size(236, 30)
                BtVitesse.TabIndex = 42
                BtVitesse.Text = "Vitesse"
                ToolTip1.SetToolTip(BtVitesse, "Cliquez pour ouvrir le graphique" & CrLf &
                                               "Vitesse / Distance")
                BtVitesse.UseVisualStyleBackColor = True
                '
                'LabelVitesseMax
                '
                LabelVitesseMax.BackColor = Color.Transparent
                LabelVitesseMax.BorderStyle = BorderStyle.FixedSingle
                LabelVitesseMax.FlatStyle = FlatStyle.Flat
                LabelVitesseMax.Location = New Point(0, 75)
                LabelVitesseMax.Name = "LabelVitesseMontee"
                LabelVitesseMax.Size = New Size(80, 23)
                LabelVitesseMax.TabIndex = 33
                LabelVitesseMax.Text = "Max +"
                LabelVitesseMax.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelVitesseMoyenne
                '
                LabelVitesseMoyenne.BackColor = Color.Transparent
                LabelVitesseMoyenne.BorderStyle = BorderStyle.FixedSingle
                LabelVitesseMoyenne.FlatStyle = FlatStyle.Flat
                LabelVitesseMoyenne.Location = New Point(79, 75)
                LabelVitesseMoyenne.Name = "LabelVitesseMoyenne"
                LabelVitesseMoyenne.Size = New Size(80, 23)
                LabelVitesseMoyenne.TabIndex = 35
                LabelVitesseMoyenne.Text = "Moyenne"
                LabelVitesseMoyenne.TextAlign = ContentAlignment.MiddleCenter
                '
                'LabelVitesseMin
                '
                LabelVitesseMin.BackColor = Color.Transparent
                LabelVitesseMin.BorderStyle = BorderStyle.FixedSingle
                LabelVitesseMin.FlatStyle = FlatStyle.Flat
                LabelVitesseMin.Location = New Point(158, 75)
                LabelVitesseMin.Name = "LabelVitesseDescente"
                LabelVitesseMin.Size = New Size(80, 23)
                LabelVitesseMin.TabIndex = 34
                LabelVitesseMin.Text = "Max -"
                LabelVitesseMin.TextAlign = ContentAlignment.MiddleCenter
                '
                'VitesseMax
                '
                VitesseMax.BackColor = Color.Transparent
                VitesseMax.BorderStyle = BorderStyle.FixedSingle
                VitesseMax.FlatStyle = FlatStyle.Flat
                VitesseMax.Location = New Point(0, 97)
                VitesseMax.Name = "VitesseMax"
                VitesseMax.Size = New Size(80, 23)
                VitesseMax.TabIndex = 26
                VitesseMax.Text = "-----"
                VitesseMax.TextAlign = ContentAlignment.MiddleCenter
                '
                'VitesseMoy
                '
                VitesseMoy.BackColor = Color.Transparent
                VitesseMoy.BorderStyle = BorderStyle.FixedSingle
                VitesseMoy.FlatStyle = FlatStyle.Flat
                VitesseMoy.Location = New Point(79, 97)
                VitesseMoy.Name = "VitesseMoy"
                VitesseMoy.Size = New Size(80, 23)
                VitesseMoy.TabIndex = 20
                VitesseMoy.Text = "-----"
                VitesseMoy.TextAlign = ContentAlignment.MiddleCenter
                '
                'VitesseMin
                '
                VitesseMin.BackColor = Color.Transparent
                VitesseMin.BorderStyle = BorderStyle.FixedSingle
                VitesseMin.FlatStyle = FlatStyle.Flat
                VitesseMin.Location = New Point(158, 97)
                VitesseMin.Name = "VitesseMin"
                VitesseMin.Size = New Size(80, 23)
                VitesseMin.TabIndex = 25
                VitesseMin.Text = "-----"
                VitesseMin.TextAlign = ContentAlignment.MiddleCenter
                '
                'ProprietesHoraires
                '
                ProprietesHoraires.BackColor = Color.Transparent
                ProprietesHoraires.Controls.Add(LabelDuree)
                ProprietesHoraires.Controls.Add(Duree)
                ProprietesHoraires.Controls.Add(BtVitesse)
                ProprietesHoraires.Controls.Add(CadreBtVitesse)
                ProprietesHoraires.Controls.Add(LabelVitesseMax)
                ProprietesHoraires.Controls.Add(LabelVitesseMoyenne)
                ProprietesHoraires.Controls.Add(LabelVitesseMin)
                ProprietesHoraires.Controls.Add(VitesseMax)
                ProprietesHoraires.Controls.Add(VitesseMoy)
                ProprietesHoraires.Controls.Add(VitesseMin)
                ProprietesHoraires.Location = New Point(6, 293)
                ProprietesHoraires.Name = "ProprietesHoraires"
                ProprietesHoraires.Size = New Size(238, 126) '120 + 6
                ProprietesHoraires.TabIndex = 42
                ProprietesHoraires.Visible = False
                '
                'Total
                '
                Total.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
                Total.BackColor = Color.FromArgb(247, 247, 247)
                Total.Controls.Add(ProprietesCommunes)
                Total.Controls.Add(ProprietesAltitudes)
                Total.Controls.Add(ProprietesHoraires)
                Total.Location = New Point(0, 0)
                Total.Name = "Total"
                Total.Size = New Size(250, 419)
                Total.TabIndex = 43
                '
                'PasOk
                '
                PasOk.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Right)
                PasOk.DialogResult = DialogResult.Cancel
                PasOk.Location = New Point(68, 425)
                PasOk.Name = "PasOk"
                PasOk.Size = New Size(80, 30)
                PasOk.TabIndex = 6
                PasOk.Text = "Annuler"
                ToolTip1.SetToolTip(PasOk, "Annule les modifications éventuelles de nom et de couleur.")
                PasOk.UseVisualStyleBackColor = True
                '
                'Ok
                '
                Ok.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
                Ok.DialogResult = DialogResult.OK
                Ok.Location = New Point(154, 425)
                Ok.Name = "Ok"
                Ok.Size = New Size(90, 30)
                Ok.TabIndex = 5
                Ok.Text = "Valider"
                ToolTip1.SetToolTip(Ok, "Valide les modifications éventuelles de nom et de couleur.")
                Ok.UseVisualStyleBackColor = True

                '
                'ColorTrace
                '
                DialogCouleurTrace.AllowFullOpen = False
                DialogCouleurTrace.AnyColor = True
                DialogCouleurTrace.SolidColorOnly = True
                '
                'ToolTip1
                '
                ToolTip1.AutomaticDelay = 100
                ToolTip1.ShowAlways = True
                '
                'ProprietesTrace
                '
                AcceptButton = Ok
                AutoScaleMode = AutoScaleMode.None
                CancelButton = PasOk
                ClientSize = New Size(250, 461)
                ControlBox = False
                Controls.Add(Ok)
                Controls.Add(PasOk)
                Controls.Add(Total)
                Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
                FormBorderStyle = FormBorderStyle.FixedToolWindow
                StartPosition = FormStartPosition.CenterParent
                Name = "ProprietesTrace"
                ProprietesAltitudes.ResumeLayout(False)
                ProprietesCommunes.ResumeLayout(False)
                ProprietesCommunes.PerformLayout()
                ProprietesHoraires.ResumeLayout(False)
                Total.ResumeLayout(False)
                ResumeLayout(False)
            End Sub

            Private LabelNom As Label
            Private NomTrace As TextBox
            Private LabelCouleurTrace As Label
            Private DialogCouleurTrace As ColorDialog
            Private Ok As Button
            Private PasOk As Button
            Private LabelNbPoints As Label
            Private NbPoints As Label
            Private Distance As Label
            Private LabelDistance As Label
            Private DenivelPositif As Label
            Private CadreBtAltitudeMax As Label
            Private DenivelNegatif As Label
            Private CadreBtAltitudeMin As Label
            Private Duree As Label
            Private LabelDuree As Label
            Private VitesseMoy As Label
            Private CadreBtVitesse As Label
            Private ToolTip1 As ToolTip
            Private LabelDenivelNegatif As Label
            Private LabelDenivelPositif As Label
            Private VitesseMax As Label
            Private VitesseMin As Label
            Private PenteMin As Label
            Private PenteMax As Label
            Private AltitudeMax As Label
            Private AltitudeMin As Label
            Private LabelPente As Label
            Private LabelVitesseMin As Label
            Private LabelVitesseMax As Label
            Private LabelVitesseMoyenne As Label
            Private PenteMoy As Label
            Private LabelPenteMoyenne As Label
            Private LabelPenteMin As Label
            Private LabelPenteMax As Label
            Friend BtAltitudeMax As Button
            Friend BtAltitudeMin As Button
            Friend BtVitesse As Button
            Private ProprietesAltitudes As Panel
            Private ProprietesCommunes As Panel
            Private ProprietesHoraires As Panel
            Private Total As Panel
        End Class
#End Region
    End Class
End Namespace
