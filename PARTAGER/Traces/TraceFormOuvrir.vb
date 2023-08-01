Namespace TRKs
    Partial Friend Class TRK
#Region "Formulaire Ouvrir Trace en cours"
        Private Class FormOuvrirTrace
            Private ReadOnly Property IndexCollection As Integer
                Get
                    If ListeTraces.SelectedIndex > -1 Then
                        Return ListeIndex(ListeTraces.SelectedIndex)
                    Else
                        Return -1
                    End If
                End Get
            End Property
            Private _CollectionTraces As List(Of TRK)
            Friend WriteOnly Property CollectionTraces As List(Of TRK)
                Set(value As List(Of TRK))
                    _CollectionTraces = value
                End Set
            End Property

            Private _IndexTrace As Integer
            Friend WriteOnly Property IndexTrace As Integer
                Set(value As Integer)
                    _IndexTrace = value
                End Set
            End Property
            Private ListeIndex As List(Of Integer)
            Private Const NomDefaut As String = "NouvelleTrace"
            Private Titre As String
            Private StockClipCurseur As Rectangle
            ''' <summary> permet d'initialiser la liste des traces avec la collection des traces </summary>
            Private Sub OuvrirTrace_Load(sender As Object, e As EventArgs)
                Titre = TitreInformation
                TitreInformation = "Information Trace"
                StockClipCurseur = Cursor.Clip
                ListeIndex = New List(Of Integer)(_CollectionTraces.Count)
                'on remplit la liste des traces à partir des traces présentes dans la collection et qui sont compatibles avec le site encours
                For Cpt As Integer = 0 To _CollectionTraces.Count - 1
                    Dim T As TRK = _CollectionTraces(Cpt)
                    If LimiteSiteWeb(_IndiceSite).CoordonneesIntersectsWith(RegionDDToRegionGrille(T.BoundingBoxDD, _SiteCarto)) Then
                        ListeTraces.Items.Add(T.Nom)
                        ListeIndex.Add(Cpt)
                        'on recherche si la trace que l'on vient d'ajouter correspond à la trace encours
                        If _IndexTrace > -1 AndAlso T.Nom = _CollectionTraces(_IndexTrace).Nom Then ListeTraces.SelectedIndex = ListeTraces.Items.Count - 1
                    End If
                Next
                ' si pas de trace encours on en crée une nouvelle
                If _IndexTrace = -1 Then BoutonNouveau_Click(Nothing, Nothing)
                'met le curseur sur le centre du bouton OK
                Cursor.Position = New Point(Location.X + Ok.Location.X + Ok.Width \ 2, Location.Y + Ok.Location.Y + Ok.Height \ 2)
                'et limite les déplacements de la souris au formulaire
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub

            Private Sub OuvrirTrace_FormClosed(sender As Object, e As FormClosedEventArgs)
                Cursor.Clip = StockClipCurseur
                TitreInformation = Titre
            End Sub
            ''' <summary> crée une nouvelle trace avec un nom par default et la couleur par default </summary>
            Private Sub BoutonNouveau_Click(sender As Object, e As EventArgs)
                NomTrace.Text = NomDefaut
                LabelCouleurTrace.BackColor = TracesSettings.SEG_TRK
                ListeTraces.SelectedIndex = -1
                BoutonNouveau.Enabled = False
                NomTrace.SelectionStart = NomTrace.Text.Length
            End Sub
            ''' <summary> sélectionne une trace à partir de la liste </summary>
            Private Sub ListeTraces_SelectedIndexChanged(sender As Object, e As EventArgs)
                If ListeTraces.SelectedIndex > -1 Then
                    NomTrace.Text = ListeTraces.SelectedItem.ToString
                    LabelCouleurTrace.BackColor = _CollectionTraces(IndexCollection).CouleurSegment
                    BoutonNouveau.Enabled = True
                End If
            End Sub
            ''' <summary> importe une trace à partir d'un fichier GPX et l'ajoute dans la liste des traces </summary>
            Private Sub BoutonImport_Click(sender As Object, e As EventArgs)
                Cursor.Clip = StockClipCurseur
                If OpenFileGPX.ShowDialog() = DialogResult.OK Then
                    'si l'import de la trace c'est bien passé
                    If TRK.OuvrirTrace(OpenFileGPX.FileName, TRK.TypesFichier.GPX) Then
                        'indextrace 
                        _IndexTrace = _CollectionTraces.Count - 1
                        'si la surface de la trace est compatible avec les limites du site encours
                        If LimiteSiteWeb(_IndiceSite).CoordonneesIntersectsWith(RegionDDToRegionGrille(_CollectionTraces(_IndexTrace).BoundingBoxDD, _SiteCarto)) Then
                            'on prend la nouvelle trace en compte
                            NomTrace.Text = _CollectionTraces(_IndexTrace).Nom
                            LabelCouleurTrace.BackColor = _CollectionTraces(_IndexTrace).CouleurSegment
                            ListeTraces.Items.Add(NomTrace.Text) 'on ajoute le nom de la trace à la liste de sélection
                            ListeIndex.Add(_IndexTrace) 'on ajoute l'index de la trace dans la collection à la liste des index
                            ListeTraces.SelectedIndex = ListeTraces.Items.Count - 1 'on sélectionne la trace importée donc la dernière
                            BoutonNouveau.Enabled = True
                        Else
                            'sinon on informe l'utilisateur que la trace a été importée mais n'est pas compatible avec le site en cours
                            MessageInformation = "La trace a été importée" & CrLf &
                                                 "mais elle n'est pas compatible" & CrLf &
                                                 "avec les limites du siteCarto actuel"
                            AfficherInformation()
                        End If
                    Else
                        BoutonNouveau_Click(Nothing, Nothing)
                    End If
                End If
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub
            ''' <summary> permet de changer la couleur de la trace </summary>
            Private Sub CouleurTrace_Click(sender As Object, e As EventArgs)
                Cursor.Clip = StockClipCurseur
                DialogCouleurTrace.Color = LabelCouleurTrace.BackColor
                DialogCouleurTrace.ShowDialog(Me)
                LabelCouleurTrace.BackColor = DialogCouleurTrace.Color
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub
            ''' <summary> suite au renommage du nom de trace celui existe donc on avertit qu'il existe déjà</summary>
            Private Sub ReInitialiseNomTrace()
                MessageInformation = "Le nom de la trace existe déjà dans la collection." & CrLf &
                                     "Veuillez le changer"
                AfficherInformation()
                If ListeTraces.SelectedIndex = -1 Then
                    NomTrace.Text = NomDefaut
                Else
                    NomTrace.Text = _CollectionTraces(IndexCollection).Nom
                End If
            End Sub
            ''' <summary> transmet à l'appelant </summary>
            Private Sub OuvrirTrace_FormClosing(sender As Object, e As FormClosingEventArgs)
                If DialogResult = DialogResult.OK Then
                    If ListeTraces.SelectedIndex = -1 Then
                        If TRK.IsExisteNomTrace(NomTrace.Text) Then
                            e.Cancel = True
                            ReInitialiseNomTrace()
                            Exit Sub
                        End If
                    Else
                        If NomTrace.Text <> ListeTraces.SelectedItem.ToString Then
                            If TRK.IsExisteNomTrace(NomTrace.Text) Then
                                e.Cancel = True
                                ReInitialiseNomTrace()
                                Exit Sub
                            Else
                                TRK.RenommerTrace(IndexCollection, NomTrace.Text)
                            End If
                        End If
                    End If
                    Tag = New ResumeTrace(IndexCollection, NomTrace.Text, True, LabelCouleurTrace.BackColor)
                End If
            End Sub
            ''' <summary> enlève les caractères interdits dans les noms de fichiers sous windows </summary>
            Private Sub NomTrace_KeyPress(sender As Object, e As KeyPressEventArgs)
                If Array.IndexOf(CarsInterdits, e.KeyChar) <> -1 Then e.KeyChar = Nothing
            End Sub
        End Class
        Partial Private Class FormOuvrirTrace
            Inherits Form

            Friend Sub New()
                InitializeComponent()
                InitialiserEvenements()
            End Sub
            Private Sub InitialiserEvenements()
                'contrôles simples
                AddHandler ListeTraces.SelectedIndexChanged, AddressOf ListeTraces_SelectedIndexChanged
                AddHandler LabelCouleurTrace.Click, AddressOf CouleurTrace_Click
                AddHandler NomTrace.KeyPress, AddressOf NomTrace_KeyPress
                AddHandler BoutonNouveau.Click, AddressOf BoutonNouveau_Click
                AddHandler BoutonImport.Click, AddressOf BoutonImport_Click
                'formulaire
                AddHandler Load, AddressOf OuvrirTrace_Load
                AddHandler FormClosing, AddressOf OuvrirTrace_FormClosing
                AddHandler FormClosed, AddressOf OuvrirTrace_FormClosed
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
                ListeTraces = New ListBox()
                LabelListeTraces = New Label()
                PasOk = New Button()
                Ok = New Button()
                LabelCouleurTrace = New Label()
                NomTrace = New TextBox()
                LabelNom = New Label()
                BoutonNouveau = New Button()
                CadreBoutonNouveau = New Label()
                DialogCouleurTrace = New ColorDialog()
                BoutonImport = New Button()
                CadreBoutonImport = New Label()
                OpenFileGPX = New OpenFileDialog()
                ToolTip1 = New ToolTip(components)
                PanelAction = New Panel()
                PanelAction.SuspendLayout()
                SuspendLayout()
                '
                'CadreBoutonNouveau
                '
                CadreBoutonNouveau.BackColor = Color.Transparent
                CadreBoutonNouveau.BorderStyle = BorderStyle.FixedSingle
                CadreBoutonNouveau.FlatStyle = FlatStyle.Flat
                CadreBoutonNouveau.Location = New Point(6, 6)
                CadreBoutonNouveau.Name = "CadreBoutonNouveau"
                CadreBoutonNouveau.Size = New Size(204, 32)
                CadreBoutonNouveau.TabIndex = 15
                CadreBoutonNouveau.Text = "Label1"
                '
                'BoutonNouveau
                '
                BoutonNouveau.Location = New Point(7, 7)
                BoutonNouveau.Name = "BoutonNouveau"
                BoutonNouveau.Size = New Size(202, 30)
                BoutonNouveau.TabIndex = 14
                BoutonNouveau.TabStop = False
                BoutonNouveau.Text = "Créer une nouvelle trace"
                ToolTip1.SetToolTip(BoutonNouveau, "Appuyez sur ce bouton pour créer une nouvelle trace." & CrLf &
                                                   "Vous pouvez changer le nom et la couleur  par défaut de la nouvelle trace.")
                BoutonNouveau.UseVisualStyleBackColor = True
                '
                'CadreBoutonImport
                '
                CadreBoutonImport.BackColor = Color.Transparent
                CadreBoutonImport.BorderStyle = BorderStyle.FixedSingle
                CadreBoutonImport.FlatStyle = FlatStyle.Flat
                CadreBoutonImport.Location = New Point(6, 37)
                CadreBoutonImport.Name = "CadreBoutonImport"
                CadreBoutonImport.Size = New Size(204, 32)
                CadreBoutonImport.TabIndex = 17
                CadreBoutonImport.Text = "Label1"
                '
                'BoutonImport
                '
                BoutonImport.Location = New Point(7, 38)
                BoutonImport.Name = "BoutonImport"
                BoutonImport.Size = New Size(202, 30)
                BoutonImport.TabIndex = 16
                BoutonImport.TabStop = False
                BoutonImport.Text = "Importer un fichier GPX"
                ToolTip1.SetToolTip(BoutonImport, "Importer une trace d'un autre logiciel." & CrLf &
                                                  "La trace est importée dans la collection. Si son emprise n'est pas" & CrLf &
                                                  "compatible  avec le site encours elle ne sera pas affichée." & CrLf &
                                                  "Vous pouvez changer le nom et la couleur de la trace importée.")
                BoutonImport.UseVisualStyleBackColor = True
                '
                'LabelListeTraces
                '
                LabelListeTraces.BackColor = Color.Transparent
                LabelListeTraces.BorderStyle = BorderStyle.FixedSingle
                LabelListeTraces.FlatStyle = FlatStyle.Flat
                LabelListeTraces.Location = New Point(6, 68)
                LabelListeTraces.Name = "LabelListeTraces"
                LabelListeTraces.Size = New Size(204, 21)
                LabelListeTraces.TabIndex = 3
                LabelListeTraces.Text = "Sélectionner une trace existante"
                LabelListeTraces.TextAlign = ContentAlignment.MiddleCenter
                '
                'ListeTraces
                '
                ListeTraces.BackColor = Color.White
                ListeTraces.Location = New Point(6, 88)
                ListeTraces.Name = "ListeTraces"
                ListeTraces.Size = New Size(204, 118)
                ListeTraces.TabIndex = 2
                ListeTraces.TabStop = False
                ToolTip1.SetToolTip(ListeTraces, "Cliquez sur une des traces présentes dans la collection" & CrLf &
                                                 "pour l'ouvrir." & CrLf &
                                                 "Vous pouvez modifier le nom et la couleur de la trace existante.")
                '
                'LabelNom
                '
                LabelNom.BackColor = Color.Transparent
                LabelNom.BorderStyle = BorderStyle.FixedSingle
                LabelNom.FlatStyle = FlatStyle.Flat
                LabelNom.Location = New Point(6, 212)
                LabelNom.Name = "LabelNom"
                LabelNom.Size = New Size(204, 21)
                LabelNom.TabIndex = 7
                LabelNom.Text = "Nom de la trace"
                LabelNom.TextAlign = ContentAlignment.MiddleCenter
                '
                'NomTrace
                '
                NomTrace.BackColor = Color.White
                NomTrace.Location = New Point(6, 232)
                NomTrace.Name = "NomTrace"
                NomTrace.Size = New Size(204, 23)
                NomTrace.TabIndex = 0
                ToolTip1.SetToolTip(NomTrace, "Nom de la trace." & CrLf &
                                              "Si vous modifiez le nom, le fichier associé sera également renommé.")
                '
                'LabelCouleurTrace
                '
                LabelCouleurTrace.BackColor = Color.Transparent
                LabelCouleurTrace.BorderStyle = BorderStyle.FixedSingle
                LabelCouleurTrace.FlatStyle = FlatStyle.Flat
                LabelCouleurTrace.Location = New Point(6, 253)
                LabelCouleurTrace.Name = "LabelCouleurTrace"
                LabelCouleurTrace.Size = New Size(204, 23)
                LabelCouleurTrace.TabIndex = 1
                ToolTip1.SetToolTip(LabelCouleurTrace, "Couleur de la trace")
                '
                'PanelAction
                '
                PanelAction.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
                PanelAction.BackColor = Color.FromArgb(247, 247, 247)
                PanelAction.Controls.Add(BoutonNouveau)
                PanelAction.Controls.Add(CadreBoutonNouveau)
                PanelAction.Controls.Add(BoutonImport)
                PanelAction.Controls.Add(CadreBoutonImport)
                PanelAction.Controls.Add(LabelListeTraces)
                PanelAction.Controls.Add(ListeTraces)
                PanelAction.Controls.Add(LabelNom)
                PanelAction.Controls.Add(NomTrace)
                PanelAction.Controls.Add(LabelCouleurTrace)
                PanelAction.Location = New Point(0, 0)
                PanelAction.Name = "PanelAction"
                PanelAction.Size = New Size(215, 283)
                PanelAction.TabIndex = 18
                '
                'PasOk
                '
                PasOk.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Right)
                PasOk.DialogResult = DialogResult.Cancel
                PasOk.Location = New Point(29, 289)
                PasOk.Name = "PasOk"
                PasOk.Size = New Size(80, 30)
                PasOk.TabIndex = 3
                PasOk.Text = "Annuler"
                ToolTip1.SetToolTip(PasOk, "Indique que vous n'avez pas choisi de trace et annule" & CrLf &
                                           "les modifications éventuelles de nom et de couleur.")
                PasOk.UseVisualStyleBackColor = True
                '
                'Ok
                '
                Ok.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Right)
                Ok.DialogResult = DialogResult.OK
                Ok.Location = New Point(115, 289)
                Ok.Name = "Ok"
                Ok.Size = New Size(95, 30)
                Ok.TabIndex = 2
                Ok.Text = "Ouvrir Trace"
                ToolTip1.SetToolTip(Ok, "Affiche la trace sélectionnée." & CrLf &
                                        "Si une trace est présente sur l'affichage elle sera fermée.")
                Ok.UseVisualStyleBackColor = True
                '
                'OpenFileGPX
                '
                OpenFileGPX.Filter = """GPX files (*.gpx)|*.gpx"
                OpenFileGPX.RestoreDirectory = True
                OpenFileGPX.Title = "Ouvrir un fichier GPX"
                '
                'ColorDialog
                '
                DialogCouleurTrace.AllowFullOpen = False
                DialogCouleurTrace.AnyColor = True
                DialogCouleurTrace.SolidColorOnly = True
                '
                'OuvrirTrace
                '
                AcceptButton = Ok
                AutoScaleMode = AutoScaleMode.None
                CancelButton = PasOk
                ClientSize = New Size(216, 325)
                ControlBox = False
                Controls.Add(PanelAction)
                Controls.Add(PasOk)
                Controls.Add(Ok)
                Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
                FormBorderStyle = FormBorderStyle.FixedToolWindow
                MaximizeBox = False
                MinimizeBox = False
                Name = "OuvrirTrace"
                ShowIcon = False
                ShowInTaskbar = False
                SizeGripStyle = SizeGripStyle.Hide
                StartPosition = FormStartPosition.CenterParent
                PanelAction.ResumeLayout(False)
                PanelAction.PerformLayout()
                ResumeLayout(False)
            End Sub

            Private ListeTraces As ListBox
            Private LabelListeTraces As Label
            Private PasOk As Button
            Private Ok As Button
            Private LabelCouleurTrace As Label
            Private NomTrace As TextBox
            Private LabelNom As Label
            Private BoutonNouveau As Button
            Private CadreBoutonNouveau As Label
            Private DialogCouleurTrace As ColorDialog
            Private BoutonImport As Button
            Private CadreBoutonImport As Label
            Private OpenFileGPX As OpenFileDialog
            Private ToolTip1 As ToolTip
            Private PanelAction As Panel
        End Class
#End Region
    End Class
End Namespace