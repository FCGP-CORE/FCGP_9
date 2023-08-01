Namespace TRKs
    Partial Friend Class TRK
#Region "Formulaire Fermer Trace en cours"
        Private Class FormFermerTrace
            Inherits Form
            Private _CollectionTraces As List(Of TRK)
            Private StockClipCurseur As Rectangle
            Friend WriteOnly Property CollectionTraces As List(Of TRK)
                Set(value As List(Of TRK))
                    _CollectionTraces = value
                End Set
            End Property
            Private _IndexTrace As Integer
            Private Titre As String
            Friend WriteOnly Property IndexTrace As Integer
                Set(value As Integer)
                    _IndexTrace = value
                End Set
            End Property
            Private Sub FermerTrace_Load(sender As Object, e As EventArgs)
                Titre = TitreInformation
                TitreInformation = "Information Trace"
                StockClipCurseur = Cursor.Clip
                'met le curseur sur le centre du bouton OK
                Cursor.Position = New Point(Location.X + Ok.Location.X + Ok.Width \ 2, Location.Y + Ok.Location.Y + Ok.Height \ 2)
                'et limite les déplacements de la souris au formulaire
                Cursor.Clip = New Rectangle(Location, Size)
            End Sub
            Private Sub FermerTrace_FormClosed(sender As Object, e As FormClosedEventArgs)
                Cursor.Clip = StockClipCurseur
                TitreInformation = Titre
            End Sub

            ''' <summary> permet l'enregistrement de la trace sous un autre fichier .trk </summary>
            Private Sub ButtonEnregistreSous_Click(sender As Object, e As EventArgs)
                If _CollectionTraces(_IndexTrace).NbSegments > 0 Then
                    Cursor.Clip = StockClipCurseur
                    EnregistrerTrace.Filter = "TRK files (*.trk)|*.trk"
                    EnregistrerTrace.DefaultExt = ".trk"
                    EnregistrerTrace.Title = "Sauvegarder la trace sous"
                    EnregistrerTrace.FileName = _CollectionTraces(_IndexTrace).Nom
                    'c'est la boite de dialogue qui gère si le nom existe déjà dans la collection
                    If EnregistrerTrace.ShowDialog() = DialogResult.OK Then
                        'on enregistre la trace sous son nouveau nom
                        TRK.EnregistrerTrace(_IndexTrace, EnregistrerTrace.FileName, TypesFichier.TRK)
                    End If
                    Cursor.Clip = New Rectangle(Location, Size)
                Else
                    MessageInformation = "La trace doit avoir au moins un segment" & CrLf &
                                         "pour pouvoir être sauvegardée"
                    AfficherInformation()
                End If
            End Sub
            ''' <summary> permet l'enregistrement de la trace sous un fichier .gpx </summary>
            Private Sub BoutonExport_Click(sender As Object, e As EventArgs)
                If _CollectionTraces(_IndexTrace).NbSegments > 0 Then
                    Cursor.Clip = StockClipCurseur
                    EnregistrerTrace.Filter = "GPX files (*.gpx)|*.gpx"
                    EnregistrerTrace.DefaultExt = ".gpx"
                    EnregistrerTrace.Title = "Exporter la trace"
                    EnregistrerTrace.FilterIndex = 1 'GPX
                    EnregistrerTrace.FileName = _CollectionTraces(_IndexTrace).Nom
                    'c'est la boite de dialogue qui gère si le nom existe déjà dans la collection
                    If EnregistrerTrace.ShowDialog() = DialogResult.OK Then
                        TRK.EnregistrerTrace(_IndexTrace, EnregistrerTrace.FileName, TypesFichier.GPX)
                    End If
                    Cursor.Clip = New Rectangle(Location, Size)
                Else
                    MessageInformation = "La trace doit avoir au moins un segment" & CrLf &
                                         "pour pouvoir être exportée"
                    AfficherInformation()
                End If
            End Sub
            ''' <summary> permet de supprimer la trace de la collection et le fichier .trk correspondant </summary>
            Private Sub BoutonSupprimer_Click(sender As Object, e As EventArgs)
                MessageInformation = "Voulez vous effectivement supprimer la trace encours ?"
                If AfficherConfirmation() = DialogResult.OK Then
                    DialogResult = DialogResult.Abort
                Else
                    DialogResult = DialogResult.Cancel
                End If
                Close()
            End Sub
        End Class
        Partial Private Class FormFermerTrace
            Inherits Form
            Friend Sub New()
                InitializeComponent()
                InitialiserEvenements()
            End Sub
            Private Sub InitialiserEvenements()
                AddHandler BoutonExporter.Click, AddressOf BoutonExport_Click
                AddHandler ButtonEnregistreSous.Click, AddressOf ButtonEnregistreSous_Click
                AddHandler BoutonSupprimer.Click, AddressOf BoutonSupprimer_Click
                AddHandler Load, AddressOf FermerTrace_Load
                AddHandler FormClosed, AddressOf FermerTrace_FormClosed
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
                ButtonEnregistreSous = New Button
                CadreBoutonEnregistrerSous = New Label
                BoutonExporter = New Button
                CadreBoutonExporter = New Label
                BoutonSupprimer = New Button
                CadreBoutonSupprimer = New Label
                PasOk = New Button
                Ok = New Button
                components = New Container()
                EnregistrerTrace = New SaveFileDialog()
                ToolTip1 = New ToolTip(components)
                PanelActions = New Panel()
                PanelActions.SuspendLayout()
                SuspendLayout()
                '
                'CadreBoutonEnregistrerSous
                '
                CadreBoutonEnregistrerSous.BorderStyle = BorderStyle.FixedSingle
                CadreBoutonEnregistrerSous.FlatStyle = FlatStyle.Flat
                CadreBoutonEnregistrerSous.Location = New Point(6, 6)
                CadreBoutonEnregistrerSous.Name = "CadreBoutonEnregistrerSous"
                CadreBoutonEnregistrerSous.Size = New Size(201, 32)
                CadreBoutonEnregistrerSous.TabIndex = 19
                CadreBoutonEnregistrerSous.Text = "Label1"
                '
                'ButtonEnregistreSous
                '
                ButtonEnregistreSous.BackColor = Color.Transparent
                ButtonEnregistreSous.Location = New Point(7, 7)
                ButtonEnregistreSous.Name = "ButtonEnregistreSous"
                ButtonEnregistreSous.Size = New Size(199, 30)
                ButtonEnregistreSous.TabIndex = 1
                ButtonEnregistreSous.TabStop = False
                ButtonEnregistreSous.Tag = ""
                ButtonEnregistreSous.Text = "Sauvegarder sous"
                ButtonEnregistreSous.UseVisualStyleBackColor = False
                ToolTip1.SetToolTip(ButtonEnregistreSous, "Sauvegarde la trace avec un nom différent." & CrLf &
                                                          "La trace encours reste la trace de l'affichage.")
                '
                'CadreBoutonExporter
                '
                CadreBoutonExporter.BackColor = Color.Transparent
                CadreBoutonExporter.BorderStyle = BorderStyle.FixedSingle
                CadreBoutonExporter.FlatStyle = FlatStyle.Flat
                CadreBoutonExporter.Location = New Point(6, 37)
                CadreBoutonExporter.Name = "CadreBoutonExporter"
                CadreBoutonExporter.Size = New Size(201, 32)
                CadreBoutonExporter.TabIndex = 21
                CadreBoutonExporter.Text = "Label1"
                '
                'BoutonExporter
                '
                BoutonExporter.BackColor = Color.Transparent
                BoutonExporter.Location = New Point(7, 38)
                BoutonExporter.Name = "BoutonExporter"
                BoutonExporter.Size = New Size(199, 30)
                BoutonExporter.TabIndex = 1
                BoutonExporter.TabStop = False
                BoutonExporter.Tag = ""
                BoutonExporter.Text = "Exporter un fichier GPX"
                BoutonExporter.UseVisualStyleBackColor = False
                ToolTip1.SetToolTip(BoutonExporter, "Exporte la trace encours sous forme de fichier GPX." & CrLf &
                                                    "Cela permet l'échange avec d'autres applications ou le GPS.")
                '
                'CadreBoutonSupprimer
                '
                CadreBoutonSupprimer.BackColor = Color.Transparent
                CadreBoutonSupprimer.BorderStyle = BorderStyle.FixedSingle
                CadreBoutonSupprimer.FlatStyle = FlatStyle.Flat
                CadreBoutonSupprimer.Location = New Point(6, 68)
                CadreBoutonSupprimer.Name = "CadreBoutonSupprimer"
                CadreBoutonSupprimer.Size = New Size(201, 32)
                CadreBoutonSupprimer.TabIndex = 25
                CadreBoutonSupprimer.Text = "Label1"
                '
                'BoutonSupprimer
                '
                BoutonSupprimer.BackColor = Color.Transparent
                BoutonSupprimer.Location = New Point(7, 69)
                BoutonSupprimer.Name = "BoutonSupprimer"
                BoutonSupprimer.Size = New Size(199, 30)
                BoutonSupprimer.TabIndex = 1
                BoutonSupprimer.TabStop = False
                BoutonSupprimer.Text = "Supprimer"
                BoutonSupprimer.UseVisualStyleBackColor = False
                ToolTip1.SetToolTip(BoutonSupprimer, "Supprime la trace de l'affichage," & CrLf &
                                                     "de la collection et du fichier associé.")
                '
                'PanelActions
                '
                PanelActions.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
                PanelActions.BackColor = Color.FromArgb(247, 247, 247)
                PanelActions.Controls.Add(ButtonEnregistreSous)
                PanelActions.Controls.Add(CadreBoutonEnregistrerSous)
                PanelActions.Controls.Add(BoutonExporter)
                PanelActions.Controls.Add(CadreBoutonExporter)
                PanelActions.Controls.Add(BoutonSupprimer)
                PanelActions.Controls.Add(CadreBoutonSupprimer)
                PanelActions.Location = New Point(0, 0)
                PanelActions.Name = "Panel1"
                PanelActions.Size = New Size(214, 106)
                PanelActions.TabIndex = 26
                '
                'PasOk
                '
                PasOk.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Right)
                PasOk.DialogResult = DialogResult.Cancel
                PasOk.Location = New Point(26, 112)
                PasOk.Name = "PasOk"
                PasOk.Size = New Size(80, 30)
                PasOk.TabIndex = 1
                PasOk.Text = "Annuler"
                PasOk.UseVisualStyleBackColor = True
                ToolTip1.SetToolTip(PasOk, "Annule la fermetrue de la trace.")
                '
                'Ok
                '
                Ok.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Right)
                Ok.DialogResult = DialogResult.OK
                Ok.Location = New Point(112, 112)
                Ok.Name = "Ok"
                Ok.Size = New Size(95, 30)
                Ok.TabIndex = 0
                Ok.Text = "Fermer Trace"
                Ok.UseVisualStyleBackColor = True
                ToolTip1.SetToolTip(Ok, "Supprime la trace de l'affichage." & CrLf &
                                        "Les dernières modifications sont enregistrées.")
                '
                'EnregistrerTrace
                '
                EnregistrerTrace.RestoreDirectory = True
                '
                'FermerTrace
                '
                AcceptButton = Ok
                AutoScaleMode = AutoScaleMode.None
                CancelButton = PasOk
                ClientSize = New Size(213, 148)
                ControlBox = False
                Controls.Add(PanelActions)
                Controls.Add(PasOk)
                Controls.Add(Ok)
                Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
                FormBorderStyle = FormBorderStyle.FixedToolWindow
                MaximizeBox = False
                MinimizeBox = False
                Name = "FermerTrace"
                SizeGripStyle = SizeGripStyle.Hide
                StartPosition = FormStartPosition.CenterParent
                PanelActions.ResumeLayout(False)
                ResumeLayout(False)
            End Sub

            Private BoutonExporter As Button
            Private CadreBoutonExporter As Label
            Private ButtonEnregistreSous As Button
            Private CadreBoutonEnregistrerSous As Label
            Private PasOk As Button
            Private Ok As Button
            Private EnregistrerTrace As SaveFileDialog
            Private BoutonSupprimer As Button
            Private ToolTip1 As ToolTip
            Private CadreBoutonSupprimer As Label
            Private PanelActions As Panel
        End Class
#End Region
    End Class
End Namespace
