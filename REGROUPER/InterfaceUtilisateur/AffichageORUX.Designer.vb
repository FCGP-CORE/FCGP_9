Partial Class AffichageORUX
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler CorZoomORUX0.ValueChanged, AddressOf CorZoomORUX_ValueChanged
        AddHandler CorZoomORUX1.ValueChanged, AddressOf CorZoomORUX_ValueChanged
        AddHandler CorZoomORUX2.ValueChanged, AddressOf CorZoomORUX_ValueChanged
        AddHandler CorZoomORUX3.ValueChanged, AddressOf CorZoomORUX_ValueChanged
        AddHandler CorZoomORUX4.ValueChanged, AddressOf CorZoomORUX_ValueChanged
        AddHandler ChoixFichierORUX.Click, AddressOf ChoixFichierORUX_Click
        AddHandler Appliquer.Click, AddressOf Appliquer_Click
        AddHandler Load, AddressOf AffichageORUX_Load
        AddHandler FormClosed, AddressOf AffichageORUX_FormClosed
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(AffichageORUX))
        L001 = New Label()
        L004 = New Label()
        L003 = New Label()
        L002 = New Label()
        Echelle0 = New Panel()
        CorZoomORUX0 = New NumericUpDown()
        ZoomORUX0 = New Label()
        Clef0 = New Label()
        Echelle1 = New Panel()
        CorZoomORUX1 = New NumericUpDown()
        ZoomORUX1 = New Label()
        Clef1 = New Label()
        Echelle2 = New Panel()
        CorZoomORUX2 = New NumericUpDown()
        ZoomORUX2 = New Label()
        Clef2 = New Label()
        Echelle3 = New Panel()
        Clef3 = New Label()
        ZoomORUX3 = New Label()
        CorZoomORUX3 = New NumericUpDown()
        Echelle4 = New Panel()
        CorZoomORUX4 = New NumericUpDown()
        ZoomORUX4 = New Label()
        Clef4 = New Label()
        ChoixFichierORUX = New Button()
        Appliquer = New Button()
        Quitter = New Button()
        ToolTip1 = New ToolTip(components)
        ChercheFichierORUX = New OpenFileDialog()
        Support = New Panel
        Echelle0.SuspendLayout()
        CType(CorZoomORUX0, ISupportInitialize).BeginInit()
        Echelle1.SuspendLayout()
        CType(CorZoomORUX1, ISupportInitialize).BeginInit()
        Echelle2.SuspendLayout()
        CType(CorZoomORUX2, ISupportInitialize).BeginInit()
        Echelle3.SuspendLayout()
        CType(CorZoomORUX3, ISupportInitialize).BeginInit()
        Echelle4.SuspendLayout()
        CType(CorZoomORUX4, ISupportInitialize).BeginInit()
        SuspendLayout()
#Region "Entête"
        '
        'Label25
        '
        L001.BackColor = Color.Transparent
        L001.BorderStyle = BorderStyle.FixedSingle
        L001.Location = New Point(6, 6)
        L001.Name = "Label25"
        L001.Size = New Size(325, 23)
        L001.TabIndex = 21
        L001.Text = "Niveaux de détails du fichier"
        L001.TextAlign = ContentAlignment.MiddleCenter
        '
        'Label2
        '
        L002.BackColor = Color.Transparent
        L002.BorderStyle = BorderStyle.FixedSingle
        L002.Location = New Point(6, 28)
        L002.Name = "Label2"
        L002.Size = New Size(85, 21)
        L002.TabIndex = 15
        L002.Text = "Clef"
        L002.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(L002, "Clef du niveau de détail")
        '
        'Label3
        '
        L003.BackColor = Color.Transparent
        L003.BorderStyle = BorderStyle.FixedSingle
        L003.Location = New Point(90, 28)
        L003.Name = "Label3"
        L003.Size = New Size(105, 21)
        L003.TabIndex = 17
        L003.Text = "Indice Actuel"
        L003.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(L003, resources.GetString("Label3.ToolTip"))
        '
        'Label4
        '
        L004.BackColor = Color.Transparent
        L004.BorderStyle = BorderStyle.FixedSingle
        L004.Location = New Point(194, 28)
        L004.Name = "Label4"
        L004.Size = New Size(137, 21)
        L004.TabIndex = 19
        L004.Text = "Indice Modifié"
        L004.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(L004, "Valeur du zoom ORUX corrigé (pas de 1)" & CrLf & "Valeur de transition" & CrLf &
                                    "1 niveau de différence --> 75% -150%" & CrLf & "2 niveaux de différence --> 50% - 200%" & CrLf &
                                    "3 niveaux de différence --> 35% - 280%")
#End Region
#Region "Echelles"
        '
        'Echelle0
        '
        Echelle0.BackColor = Color.Transparent
        Echelle0.BorderStyle = BorderStyle.FixedSingle
        Echelle0.Controls.Add(CorZoomORUX0)
        Echelle0.Controls.Add(ZoomORUX0)
        Echelle0.Controls.Add(Clef0)
        Echelle0.Enabled = False
        Echelle0.Location = New Point(6, 48)
        Echelle0.Margin = New Padding(0)
        Echelle0.Name = "Echelle0"
        Echelle0.Size = New Size(325, 24)
        Echelle0.TabIndex = 12
        '
        'Clef0
        '
        Clef0.BorderStyle = BorderStyle.FixedSingle
        Clef0.Location = New Point(-1, -1)
        Clef0.Name = "Clef0"
        Clef0.Size = New Size(85, 24)
        Clef0.TabIndex = 1
        Clef0.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Clef0, "Clef du niveau de détail")
        '
        'ZoomORUX0
        '
        ZoomORUX0.BorderStyle = BorderStyle.FixedSingle
        ZoomORUX0.Location = New Point(83, -1)
        ZoomORUX0.Name = "ZoomORUX0"
        ZoomORUX0.Size = New Size(105, 24)
        ZoomORUX0.TabIndex = 2
        ZoomORUX0.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomORUX0, "Voir l'aide sur le label Zoom")
        '
        'CorZoomORUX0
        '
        CorZoomORUX0.BackColor = Color.White
        CorZoomORUX0.Location = New Point(188, 0)
        CorZoomORUX0.Maximum = 20D
        CorZoomORUX0.Name = "CorZoomORUX0"
        CorZoomORUX0.ReadOnly = True
        CorZoomORUX0.Size = New Size(135, 23)
        CorZoomORUX0.TabIndex = 3
        CorZoomORUX0.BorderStyle = BorderStyle.None
        CorZoomORUX0.Tag = "0"
        CorZoomORUX0.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomORUX0, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle1
        '
        Echelle1.BackColor = Color.Transparent
        Echelle1.BorderStyle = BorderStyle.FixedSingle
        Echelle1.Controls.Add(CorZoomORUX1)
        Echelle1.Controls.Add(ZoomORUX1)
        Echelle1.Controls.Add(Clef1)
        Echelle1.Enabled = False
        Echelle1.Location = New Point(6, 71)
        Echelle1.Margin = New Padding(0)
        Echelle1.Name = "Echelle1"
        Echelle1.Size = New Size(325, 24)
        Echelle1.TabIndex = 14
        '
        'Clef1
        '
        Clef1.BorderStyle = BorderStyle.FixedSingle
        Clef1.Location = New Point(-1, -1)
        Clef1.Name = "Clef1"
        Clef1.Size = New Size(85, 24)
        Clef1.TabIndex = 1
        Clef1.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Clef1, "Clef du niveau de détail")
        '
        'ZoomORUX1
        '
        ZoomORUX1.BorderStyle = BorderStyle.FixedSingle
        ZoomORUX1.Location = New Point(83, -1)
        ZoomORUX1.Name = "ZoomORUX1"
        ZoomORUX1.Size = New Size(105, 24)
        ZoomORUX1.TabIndex = 2
        ZoomORUX1.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomORUX1, "Voir l'aide sur le label Zoom")
        '
        'CorZoomORUX1
        '
        CorZoomORUX1.BackColor = Color.White
        CorZoomORUX1.Location = New Point(188, 0)
        CorZoomORUX1.Maximum = 20D
        CorZoomORUX1.Name = "CorZoomORUX1"
        CorZoomORUX1.ReadOnly = True
        CorZoomORUX1.Size = New Size(135, 23)
        CorZoomORUX1.TabIndex = 3
        CorZoomORUX1.BorderStyle = BorderStyle.None
        CorZoomORUX1.Tag = "1"
        CorZoomORUX1.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomORUX1, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle2
        '
        Echelle2.BackColor = Color.Transparent
        Echelle2.BorderStyle = BorderStyle.FixedSingle
        Echelle2.Controls.Add(CorZoomORUX2)
        Echelle2.Controls.Add(ZoomORUX2)
        Echelle2.Controls.Add(Clef2)
        Echelle2.Enabled = False
        Echelle2.Location = New Point(6, 94)
        Echelle2.Margin = New Padding(0)
        Echelle2.Name = "Echelle2"
        Echelle2.Size = New Size(325, 24)
        Echelle2.TabIndex = 16
        '
        'Clef2
        '
        Clef2.BorderStyle = BorderStyle.FixedSingle
        Clef2.Location = New Point(-1, -1)
        Clef2.Name = "Clef2"
        Clef2.Size = New Size(85, 24)
        Clef2.TabIndex = 1
        Clef2.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Clef2, "Clef du niveau de détail")
        '
        'ZoomORUX2
        '
        ZoomORUX2.BorderStyle = BorderStyle.FixedSingle
        ZoomORUX2.Location = New Point(83, -1)
        ZoomORUX2.Name = "ZoomORUX2"
        ZoomORUX2.Size = New Size(105, 24)
        ZoomORUX2.TabIndex = 2
        ZoomORUX2.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomORUX2, "Voir l'aide sur le label Zoom")
        '
        'CorZoomORUX2
        '
        CorZoomORUX2.BackColor = Color.White
        CorZoomORUX2.Location = New Point(188, 0)
        CorZoomORUX2.Maximum = 20D
        CorZoomORUX2.Name = "CorZoomORUX2"
        CorZoomORUX2.ReadOnly = True
        CorZoomORUX2.Size = New Size(135, 23)
        CorZoomORUX2.TabIndex = 3
        CorZoomORUX2.BorderStyle = BorderStyle.None
        CorZoomORUX2.Tag = "2"
        CorZoomORUX2.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomORUX2, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle3
        '
        Echelle3.BackColor = Color.Transparent
        Echelle3.BorderStyle = BorderStyle.FixedSingle
        Echelle3.Controls.Add(Clef3)
        Echelle3.Controls.Add(ZoomORUX3)
        Echelle3.Controls.Add(CorZoomORUX3)
        Echelle3.Enabled = False
        Echelle3.Location = New Point(6, 117)
        Echelle3.Margin = New Padding(0)
        Echelle3.Name = "Echelle3"
        Echelle3.Size = New Size(325, 24)
        Echelle3.TabIndex = 18
        '
        'Clef3
        '
        Clef3.BorderStyle = BorderStyle.FixedSingle
        Clef3.Location = New Point(-1, -1)
        Clef3.Name = "Clef3"
        Clef3.Size = New Size(85, 24)
        Clef3.TabIndex = 1
        Clef3.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Clef3, "Clef du niveau de détail")
        '
        'ZoomORUX3
        '
        ZoomORUX3.BorderStyle = BorderStyle.FixedSingle
        ZoomORUX3.Location = New Point(83, -1)
        ZoomORUX3.Name = "ZoomORUX3"
        ZoomORUX3.Size = New Size(105, 24)
        ZoomORUX3.TabIndex = 2
        ZoomORUX3.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomORUX3, "Voir l'aide sur le label Zoom")
        '
        'CorZoomORUX3
        '
        CorZoomORUX3.BackColor = Color.White
        CorZoomORUX3.Location = New Point(188, 0)
        CorZoomORUX3.Maximum = 20D
        CorZoomORUX3.Name = "CorZoomORUX3"
        CorZoomORUX3.ReadOnly = True
        CorZoomORUX3.Size = New Size(135, 23)
        CorZoomORUX3.TabIndex = 3
        CorZoomORUX3.BorderStyle = BorderStyle.None
        CorZoomORUX3.Tag = "3"
        CorZoomORUX3.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomORUX3, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle4
        '
        Echelle4.BackColor = Color.Transparent
        Echelle4.BorderStyle = BorderStyle.FixedSingle
        Echelle4.Controls.Add(CorZoomORUX4)
        Echelle4.Controls.Add(ZoomORUX4)
        Echelle4.Controls.Add(Clef4)
        Echelle4.Enabled = False
        Echelle4.Location = New Point(6, 140)
        Echelle4.Margin = New Padding(0)
        Echelle4.Name = "Echelle4"
        Echelle4.Size = New Size(325, 24)
        Echelle4.TabIndex = 20
        '
        'Clef4
        '
        Clef4.BorderStyle = BorderStyle.FixedSingle
        Clef4.Location = New Point(-1, -1)
        Clef4.Name = "Clef4"
        Clef4.Size = New Size(85, 24)
        Clef4.TabIndex = 1
        Clef4.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Clef4, "Clef du niveau de détail")
        '
        'ZoomORUX4
        '
        ZoomORUX4.BorderStyle = BorderStyle.FixedSingle
        ZoomORUX4.Location = New Point(83, -1)
        ZoomORUX4.Name = "ZoomORUX4"
        ZoomORUX4.Size = New Size(105, 24)
        ZoomORUX4.TabIndex = 2
        ZoomORUX4.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomORUX4, "Voir l'aide sur le label Zoom")
        '
        'CorZoomORUX4
        '
        CorZoomORUX4.BackColor = Color.White
        CorZoomORUX4.Location = New Point(188, 0)
        CorZoomORUX4.Maximum = 20D
        CorZoomORUX4.Name = "CorZoomORUX4"
        CorZoomORUX4.ReadOnly = True
        CorZoomORUX4.Size = New Size(135, 23)
        CorZoomORUX4.TabIndex = 3
        CorZoomORUX4.BorderStyle = BorderStyle.None
        CorZoomORUX4.Tag = "4"
        CorZoomORUX4.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomORUX4, "Voir l'aide sur le label Correction Zoom")
#End Region
#Region "Form"
        '
        'Support
        '
        Support.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Controls.Add(L001)
        Support.Controls.Add(L002)
        Support.Controls.Add(L003)
        Support.Controls.Add(L004)
        Support.Controls.Add(Echelle0)
        Support.Controls.Add(Echelle1)
        Support.Controls.Add(Echelle2)
        Support.Controls.Add(Echelle3)
        Support.Controls.Add(Echelle4)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(337, 170)
        Support.TabIndex = 0
        '
        'ChoixFichierORUX
        '
        ChoixFichierORUX.Location = New Point(6, 176)
        ChoixFichierORUX.Name = "ChoixFichierORUX"
        ChoixFichierORUX.Size = New Size(93, 46)
        ChoixFichierORUX.TabIndex = 23
        ChoixFichierORUX.Text = "Choisir un fichier"
        ToolTip1.SetToolTip(ChoixFichierORUX, "Ouvre un formulaire qui permet de choisir un fichier ORUX." & CrLf &
                                              "Celui-ci ne peut pas être sur le Smartphone (Lecture seule).")
        ChoixFichierORUX.UseVisualStyleBackColor = True
        '
        'Appliquer
        '
        Appliquer.Enabled = False
        Appliquer.Location = New Point(109, 176)
        Appliquer.Name = "Appliquer"
        Appliquer.Size = New Size(125, 46)
        Appliquer.TabIndex = 24
        Appliquer.Text = "Appliquer les modifications"
        ToolTip1.SetToolTip(Appliquer, "Enregistre les modifications apportées aux indices d'affichage dans le fichier ORUX")
        Appliquer.UseVisualStyleBackColor = True
        '
        'Quitter
        '
        Quitter.DialogResult = DialogResult.Cancel
        Quitter.Location = New Point(245, 176)
        Quitter.Name = "Quitter"
        Quitter.Size = New Size(85, 46)
        Quitter.TabIndex = 25
        Quitter.Text = "Quitter"
        ToolTip1.SetToolTip(Quitter, "Ferme le formulaire")
        Quitter.UseVisualStyleBackColor = True
        '
        'AffichageORUX
        '
        AcceptButton = ChoixFichierORUX
        AutoScaleMode = AutoScaleMode.None
        AutoSize = True
        CancelButton = Quitter
        ClientSize = New Size(337, 228)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(Quitter)
        Controls.Add(Appliquer)
        Controls.Add(ChoixFichierORUX)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "AffichageORUX"
        Text = "Correction Affichage des Fichiers ORUX"
        StartPosition = FormStartPosition.CenterParent
        Echelle0.ResumeLayout(False)
        CType(CorZoomORUX0, ISupportInitialize).EndInit()
        Echelle1.ResumeLayout(False)
        CType(CorZoomORUX1, ISupportInitialize).EndInit()
        Echelle2.ResumeLayout(False)
        CType(CorZoomORUX2, ISupportInitialize).EndInit()
        Echelle3.ResumeLayout(False)
        CType(CorZoomORUX3, ISupportInitialize).EndInit()
        Echelle4.ResumeLayout(False)
        CType(CorZoomORUX4, ISupportInitialize).EndInit()
        ResumeLayout(False)
#End Region
    End Sub

    Private L001 As Label
    Private L004 As Label
    Private L003 As Label
    Private L002 As Label
    Private Echelle0 As Panel
    Private CorZoomORUX0 As NumericUpDown
    Private ZoomORUX0 As Label
    Private Clef0 As Label
    Private Echelle1 As Panel
    Private CorZoomORUX1 As NumericUpDown
    Private ZoomORUX1 As Label
    Private Clef1 As Label
    Private Echelle2 As Panel
    Private CorZoomORUX2 As NumericUpDown
    Private ZoomORUX2 As Label
    Private Clef2 As Label
    Private Echelle3 As Panel
    Private Clef3 As Label
    Private ZoomORUX3 As Label
    Private CorZoomORUX3 As NumericUpDown
    Private Echelle4 As Panel
    Private CorZoomORUX4 As NumericUpDown
    Private ZoomORUX4 As Label
    Private Clef4 As Label
    Private ChoixFichierORUX As Button
    Private Appliquer As Button
    Private Quitter As Button
    Private ToolTip1 As ToolTip
    Private ChercheFichierORUX As OpenFileDialog
    Private Support As Panel
End Class
