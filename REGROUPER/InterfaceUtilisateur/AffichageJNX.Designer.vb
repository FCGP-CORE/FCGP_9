Partial Class AffichageJNX
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiseEvenements()
    End Sub
    Private Sub InitialiseEvenements()
        'contrôles simples
        AddHandler CorZoomJNX0.ValueChanged, AddressOf CorZoomJNX_ValueChanged
        AddHandler CorZoomJNX1.ValueChanged, AddressOf CorZoomJNX_ValueChanged
        AddHandler CorZoomJNX2.ValueChanged, AddressOf CorZoomJNX_ValueChanged
        AddHandler CorZoomJNX3.ValueChanged, AddressOf CorZoomJNX_ValueChanged
        AddHandler CorZoomJNX4.ValueChanged, AddressOf CorZoomJNX_ValueChanged
        AddHandler ChoixFichierJNX.Click, AddressOf ChoixFichierJNX_Click
        AddHandler CorNiveauAffichage.ValueChanged, AddressOf CorNiveauAffichage_ValueChanged
        AddHandler Appliquer.Click, AddressOf Appliquer_Click
        AddHandler Load, AddressOf AffichageJNX_Load
        AddHandler FormClosed, AddressOf AffichageJNX_FormClosed
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(AffichageJNX))
        L001 = New Label()
        L004 = New Label()
        L003 = New Label()
        L002 = New Label()
        Echelle0 = New Panel()
        CorZoomJNX0 = New NumericUpDown()
        ZoomJNX0 = New Label()
        Clef0 = New Label()
        Echelle1 = New Panel()
        CorZoomJNX1 = New NumericUpDown()
        ZoomJNX1 = New Label()
        Clef1 = New Label()
        Echelle2 = New Panel()
        CorZoomJNX2 = New NumericUpDown()
        ZoomJNX2 = New Label()
        Clef2 = New Label()
        Echelle3 = New Panel()
        Clef3 = New Label()
        ZoomJNX3 = New Label()
        CorZoomJNX3 = New NumericUpDown()
        Echelle4 = New Panel()
        CorZoomJNX4 = New NumericUpDown()
        ZoomJNX4 = New Label()
        Clef4 = New Label()
        ChoixFichierJNX = New Button()
        Appliquer = New Button()
        Quitter = New Button()
        L005 = New Label()
        OrdreAffichage = New Panel()
        CorNiveauAffichage = New NumericUpDown()
        NiveauAffichage = New Label()
        ToolTip1 = New ToolTip(components)
        ChercheFichierJNX = New OpenFileDialog()
        Support = New Panel
        Echelle0.SuspendLayout()
        CType(CorZoomJNX0, ISupportInitialize).BeginInit()
        Echelle1.SuspendLayout()
        CType(CorZoomJNX1, ISupportInitialize).BeginInit()
        Echelle2.SuspendLayout()
        CType(CorZoomJNX2, ISupportInitialize).BeginInit()
        Echelle3.SuspendLayout()
        CType(CorZoomJNX3, ISupportInitialize).BeginInit()
        Echelle4.SuspendLayout()
        CType(CorZoomJNX4, ISupportInitialize).BeginInit()
        OrdreAffichage.SuspendLayout()
        CType(CorNiveauAffichage, ISupportInitialize).BeginInit()
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
        ToolTip1.SetToolTip(L004, resources.GetString("Label4.ToolTip"))
#End Region
#Region "echelles"
        '
        'Echelle0
        '
        Echelle0.BackColor = Color.Transparent
        Echelle0.BorderStyle = BorderStyle.FixedSingle
        Echelle0.Controls.Add(CorZoomJNX0)
        Echelle0.Controls.Add(ZoomJNX0)
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
        'ZoomJNX0
        '
        ZoomJNX0.BorderStyle = BorderStyle.FixedSingle
        ZoomJNX0.Location = New Point(83, -1)
        ZoomJNX0.Name = "ZoomJNX0"
        ZoomJNX0.Size = New Size(105, 24)
        ZoomJNX0.TabIndex = 2
        ZoomJNX0.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomJNX0, "Voir l'aide sur le label Zoom")
        '
        'CorZoomJNX0
        '
        CorZoomJNX0.BackColor = Color.White
        CorZoomJNX0.DecimalPlaces = 2
        CorZoomJNX0.Increment = 0.25D
        CorZoomJNX0.Location = New Point(188, 0)
        CorZoomJNX0.Maximum = 18D
        CorZoomJNX0.Name = "CorZoomJNX0"
        CorZoomJNX0.ReadOnly = True
        CorZoomJNX0.Size = New Size(135, 23)
        CorZoomJNX0.TabIndex = 3
        CorZoomJNX0.BorderStyle = BorderStyle.None
        CorZoomJNX0.Tag = "0"
        CorZoomJNX0.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomJNX0, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle1
        '
        Echelle1.BackColor = Color.Transparent
        Echelle1.BorderStyle = BorderStyle.FixedSingle
        Echelle1.Controls.Add(CorZoomJNX1)
        Echelle1.Controls.Add(ZoomJNX1)
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
        'ZoomJNX1
        '
        ZoomJNX1.BorderStyle = BorderStyle.FixedSingle
        ZoomJNX1.Location = New Point(83, -1)
        ZoomJNX1.Name = "ZoomJNX1"
        ZoomJNX1.Size = New Size(105, 24)
        ZoomJNX1.TabIndex = 2
        ZoomJNX1.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomJNX1, "Voir l'aide sur le label Zoom")
        '
        'CorZoomJNX1
        '
        CorZoomJNX1.BackColor = Color.White
        CorZoomJNX1.DecimalPlaces = 2
        CorZoomJNX1.Increment = 0.25D
        CorZoomJNX1.Location = New Point(188, 0)
        CorZoomJNX1.Maximum = 18D
        CorZoomJNX1.Name = "CorZoomJNX1"
        CorZoomJNX1.ReadOnly = True
        CorZoomJNX1.Size = New Size(135, 23)
        CorZoomJNX1.TabIndex = 3
        CorZoomJNX1.BorderStyle = BorderStyle.None
        CorZoomJNX1.Tag = "1"
        CorZoomJNX1.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomJNX1, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle2
        '
        Echelle2.BackColor = Color.Transparent
        Echelle2.BorderStyle = BorderStyle.FixedSingle
        Echelle2.Controls.Add(CorZoomJNX2)
        Echelle2.Controls.Add(ZoomJNX2)
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
        'ZoomJNX2
        '
        ZoomJNX2.BorderStyle = BorderStyle.FixedSingle
        ZoomJNX2.Location = New Point(83, -1)
        ZoomJNX2.Name = "ZoomJNX2"
        ZoomJNX2.Size = New Size(105, 24)
        ZoomJNX2.TabIndex = 2
        ZoomJNX2.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomJNX2, "Voir l'aide sur le label Zoom")
        '
        'CorZoomJNX2
        '
        CorZoomJNX2.BackColor = Color.White
        CorZoomJNX2.DecimalPlaces = 2
        CorZoomJNX2.Increment = 0.25D
        CorZoomJNX2.Location = New Point(188, 0)
        CorZoomJNX2.Maximum = 18D
        CorZoomJNX2.Name = "CorZoomJNX2"
        CorZoomJNX2.ReadOnly = True
        CorZoomJNX2.Size = New Size(135, 23)
        CorZoomJNX2.TabIndex = 3
        CorZoomJNX2.BorderStyle = BorderStyle.None
        CorZoomJNX2.Tag = "2"
        CorZoomJNX2.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomJNX2, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle3
        '
        Echelle3.BackColor = Color.Transparent
        Echelle3.BorderStyle = BorderStyle.FixedSingle
        Echelle3.Controls.Add(Clef3)
        Echelle3.Controls.Add(ZoomJNX3)
        Echelle3.Controls.Add(CorZoomJNX3)
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
        'ZoomJNX3
        '
        ZoomJNX3.BorderStyle = BorderStyle.FixedSingle
        ZoomJNX3.Location = New Point(83, -1)
        ZoomJNX3.Name = "ZoomJNX3"
        ZoomJNX3.Size = New Size(105, 24)
        ZoomJNX3.TabIndex = 2
        ZoomJNX3.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomJNX3, "Voir l'aide sur le label Zoom")
        '
        'CorZoomJNX3
        '
        CorZoomJNX3.BackColor = Color.White
        CorZoomJNX3.DecimalPlaces = 2
        CorZoomJNX3.Increment = 0.25D
        CorZoomJNX3.Location = New Point(188, 0)
        CorZoomJNX3.Maximum = 18D
        CorZoomJNX3.Name = "CorZoomJNX3"
        CorZoomJNX3.ReadOnly = True
        CorZoomJNX3.Size = New Size(135, 23)
        CorZoomJNX3.TabIndex = 3
        CorZoomJNX3.BorderStyle = BorderStyle.None
        CorZoomJNX3.Tag = "3"
        CorZoomJNX3.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomJNX3, "Voir l'aide sur le label Correction Zoom")
        '
        'Echelle4
        '
        Echelle4.BackColor = Color.Transparent
        Echelle4.BorderStyle = BorderStyle.FixedSingle
        Echelle4.Controls.Add(CorZoomJNX4)
        Echelle4.Controls.Add(ZoomJNX4)
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
        'ZoomJNX4
        '
        ZoomJNX4.BorderStyle = BorderStyle.FixedSingle
        ZoomJNX4.Location = New Point(83, -1)
        ZoomJNX4.Name = "ZoomJNX4"
        ZoomJNX4.Size = New Size(105, 24)
        ZoomJNX4.TabIndex = 2
        ZoomJNX4.TextAlign = ContentAlignment.TopCenter
        ToolTip1.SetToolTip(ZoomJNX4, "Voir l'aide sur le label Zoom")
        '
        'CorZoomJNX4
        '
        CorZoomJNX4.BackColor = Color.White
        CorZoomJNX4.DecimalPlaces = 2
        CorZoomJNX4.Increment = 0.25D
        CorZoomJNX4.Location = New Point(188, 0)
        CorZoomJNX4.Maximum = 18D
        CorZoomJNX4.Name = "CorZoomJNX4"
        CorZoomJNX4.ReadOnly = True
        CorZoomJNX4.Size = New Size(135, 23)
        CorZoomJNX4.TabIndex = 3
        CorZoomJNX4.BorderStyle = BorderStyle.None
        CorZoomJNX4.Tag = "4"
        CorZoomJNX4.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorZoomJNX4, "Voir l'aide sur le label Correction Zoom")
#End Region
#Region "Ordre affichage"
        '
        'Label1
        '
        L005.BackColor = Color.Transparent
        L005.BorderStyle = BorderStyle.FixedSingle
        L005.Location = New Point(6, 170)
        L005.Name = "Label1"
        L005.Size = New Size(325, 23)
        L005.TabIndex = 26
        L005.Text = "Ordre d'affichage du fichier"
        L005.TextAlign = ContentAlignment.MiddleCenter
        '
        'OrdreAffichage
        '
        OrdreAffichage.BackColor = Color.Transparent
        OrdreAffichage.BorderStyle = BorderStyle.FixedSingle
        OrdreAffichage.Controls.Add(CorNiveauAffichage)
        OrdreAffichage.Controls.Add(NiveauAffichage)
        OrdreAffichage.Enabled = False
        OrdreAffichage.Location = New Point(6, 192)
        OrdreAffichage.Margin = New Padding(0)
        OrdreAffichage.Name = "OrdreAffichage"
        OrdreAffichage.Size = New Size(325, 24)
        OrdreAffichage.TabIndex = 13
        '
        'NiveauAffichage
        '
        NiveauAffichage.BorderStyle = BorderStyle.FixedSingle
        NiveauAffichage.Location = New Point(-1, -1)
        NiveauAffichage.Name = "NiveauAffichage"
        NiveauAffichage.Size = New Size(165, 24)
        NiveauAffichage.TabIndex = 2
        NiveauAffichage.TextAlign = ContentAlignment.MiddleCenter
        '
        'CorNiveauAffichage
        '
        CorNiveauAffichage.BackColor = Color.White
        CorNiveauAffichage.Location = New Point(164, 0)
        CorNiveauAffichage.Name = "CorNiveauAffichage"
        CorNiveauAffichage.Size = New Size(159, 23)
        CorNiveauAffichage.TabIndex = 3
        CorNiveauAffichage.BorderStyle = BorderStyle.None
        CorNiveauAffichage.Tag = "0"
        CorNiveauAffichage.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorNiveauAffichage, "Ordre d'affichage de la carte par rapport aux autres cartes. " & CrLf &
                                                "Permet de masquer plus de détails d'une carte vectorielle superposée.")
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
        Support.Controls.Add(L005)
        Support.Controls.Add(OrdreAffichage)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(337, 222)
        Support.TabIndex = 0

        '
        'ChoixFichierJNX
        '
        ChoixFichierJNX.Location = New Point(5, 228)
        ChoixFichierJNX.Name = "ChoixFichierJNX"
        ChoixFichierJNX.Size = New Size(93, 46)
        ChoixFichierJNX.TabIndex = 23
        ChoixFichierJNX.Text = "Choisir un fichier"
        ToolTip1.SetToolTip(ChoixFichierJNX, "Ouvre un formulaire qui permet de choisir un fichier JNX. Celui-ci peut être sur le GPS")
        ChoixFichierJNX.UseVisualStyleBackColor = True
        '
        'Appliquer
        '
        Appliquer.Enabled = False
        Appliquer.Location = New Point(109, 228)
        Appliquer.Name = "Appliquer"
        Appliquer.Size = New Size(125, 46)
        Appliquer.TabIndex = 24
        Appliquer.Text = "Appliquer les modifications"
        ToolTip1.SetToolTip(Appliquer, "Enregistre les modifications apportées aux indices d'affichage dans le fichier JNX")
        Appliquer.UseVisualStyleBackColor = True
        '
        'Quitter
        '
        Quitter.DialogResult = DialogResult.Cancel
        Quitter.Location = New Point(245, 228)
        Quitter.Name = "Quitter"
        Quitter.Size = New Size(85, 46)
        Quitter.TabIndex = 25
        Quitter.Text = "Quitter"
        ToolTip1.SetToolTip(Quitter, "Ferme le formulaire")
        Quitter.UseVisualStyleBackColor = True
        '
        'AffichageJNX
        '
        AcceptButton = ChoixFichierJNX
        AutoScaleMode = AutoScaleMode.None
        AutoSize = True
        CancelButton = Quitter
        ClientSize = New Size(337, 280)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(Quitter)
        Controls.Add(Appliquer)
        Controls.Add(ChoixFichierJNX)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "AffichageJNX"
        Text = "Correction Affichage des Fichiers JNX"
        StartPosition = FormStartPosition.CenterParent
        Echelle0.ResumeLayout(False)
        CType(CorZoomJNX0, ISupportInitialize).EndInit()
        Echelle1.ResumeLayout(False)
        CType(CorZoomJNX1, ISupportInitialize).EndInit()
        Echelle2.ResumeLayout(False)
        CType(CorZoomJNX2, ISupportInitialize).EndInit()
        Echelle3.ResumeLayout(False)
        CType(CorZoomJNX3, ISupportInitialize).EndInit()
        Echelle4.ResumeLayout(False)
        CType(CorZoomJNX4, ISupportInitialize).EndInit()
        OrdreAffichage.ResumeLayout(False)
        CType(CorNiveauAffichage, ISupportInitialize).EndInit()
        ResumeLayout(False)
#End Region
    End Sub

    Private L001 As Label
    Private L004 As Label
    Private L003 As Label
    Private L002 As Label
    Private Echelle0 As Panel
    Private CorZoomJNX0 As NumericUpDown
    Private ZoomJNX0 As Label
    Private Clef0 As Label
    Private Echelle1 As Panel
    Private CorZoomJNX1 As NumericUpDown
    Private ZoomJNX1 As Label
    Private Clef1 As Label
    Private Echelle2 As Panel
    Private CorZoomJNX2 As NumericUpDown
    Private ZoomJNX2 As Label
    Private Clef2 As Label
    Private Echelle3 As Panel
    Private Clef3 As Label
    Private ZoomJNX3 As Label
    Private CorZoomJNX3 As NumericUpDown
    Private Echelle4 As Panel
    Private CorZoomJNX4 As NumericUpDown
    Private ZoomJNX4 As Label
    Private Clef4 As Label
    Private ChoixFichierJNX As Button
    Private Appliquer As Button
    Private Quitter As Button
    Private L005 As Label
    Private OrdreAffichage As Panel
    Private CorNiveauAffichage As NumericUpDown
    Private NiveauAffichage As Label
    Private ToolTip1 As ToolTip
    Private ChercheFichierJNX As OpenFileDialog
    Private Support As Panel
End Class
