Partial Class SaisieSurfaceTraitement
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler DebCol.Click, AddressOf IndexNombreTuile_Click
        AddHandler NbCol.Click, AddressOf IndexNombreTuile_Click
        AddHandler DebRow.Click, AddressOf IndexNombreTuile_Click
        AddHandler NbRow.Click, AddressOf IndexNombreTuile_Click
        AddHandler Valider.Click, AddressOf Fermer_Click
        AddHandler Point1.Click, AddressOf Point_Click
        AddHandler Point2.Click, AddressOf Point_Click
        AddHandler Annuler.Click, AddressOf Fermer_Click
        'SaisieIndex
        AddHandler SaisieIndex.DoubleClick, AddressOf SaisieIndex_DoubleClick
        AddHandler SaisieIndex.PreviewKeyDown, AddressOf SaisieIndex_PreviewKeyDown
        AddHandler SaisieIndex.KeyUp, AddressOf SaisieIndex_KeyUp
        AddHandler SaisieIndex.KeyDown, AddressOf SaisieIndex_KeyDown
        'formulaire
        AddHandler Load, AddressOf SurfaceCaptureTuile_Load
        AddHandler FormClosed, AddressOf SurfaceCaptureTuile_FormClosed
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
        Dim resources As New ComponentResourceManager(GetType(SaisieSurfaceTraitement))
        DebCol = New Label()
        NbCol = New Label()
        DebRow = New Label()
        NbRow = New Label()
        Valider = New Button()
        L_Delimitation = New Label()
        L_PtNO = New Label()
        L_PtSE = New Label()
        L_Largeur = New Label()
        L_Hauteur = New Label()
        Point1 = New Label()
        Hauteur = New Label()
        Largeur = New Label()
        Point2 = New Label()
        Surface = New Label()
        L_NbTuilesMax = New Label()
        L_TuilesTelechargees = New Label()
        NbTuilesMax = New Label()
        L_NbTuilesMaxX = New Label()
        NbTuilesMaxX = New Label()
        L_Rangees = New Label()
        L_Colonnes = New Label()
        L_IndexCol = New Label()
        L_NbCol = New Label()
        L_NbRow = New Label()
        L_IndexRow = New Label()
        SaisieIndex = New TextBox()
        Annuler = New Button()
        ToolTip1 = New ToolTip(components)
        L_Surface = New Label()
        NbTuilesTelecharger = New Label()
        L_NbTuilesTelecharger = New Label()
        Support = New Panel()
        Support.SuspendLayout()
        SuspendLayout()
        '
        'L_Delimitation
        '
        L_Delimitation.BackColor = Color.Transparent
        L_Delimitation.BorderStyle = BorderStyle.FixedSingle
        L_Delimitation.Location = New Point(6, 6)
        L_Delimitation.Margin = New Padding(0)
        L_Delimitation.Name = "L_Delimitation"
        L_Delimitation.Size = New Size(341, 21)
        L_Delimitation.TabIndex = 22
        L_Delimitation.Text = "Points de délimitation"
        L_Delimitation.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_PtNO
        '
        L_PtNO.BackColor = Color.Transparent
        L_PtNO.BorderStyle = BorderStyle.FixedSingle
        L_PtNO.Location = New Point(6, 26)
        L_PtNO.Margin = New Padding(0)
        L_PtNO.Name = "L_PtNO"
        L_PtNO.Size = New Size(86, 21)
        L_PtNO.TabIndex = 24
        L_PtNO.Tag = ""
        L_PtNO.Text = "Point NO"
        L_PtNO.TextAlign = ContentAlignment.MiddleCenter
        '
        'Point1
        '
        Point1.BackColor = Color.White
        Point1.BorderStyle = BorderStyle.FixedSingle
        Point1.Location = New Point(91, 26)
        Point1.Margin = New Padding(0)
        Point1.Name = "Point1"
        Point1.Size = New Size(256, 21)
        Point1.TabIndex = 31
        Point1.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Point1, resources.GetString("Point1.ToolTip"))
        '
        'L_PtSE
        '
        L_PtSE.BackColor = Color.Transparent
        L_PtSE.BorderStyle = BorderStyle.FixedSingle
        L_PtSE.Location = New Point(6, 46)
        L_PtSE.Margin = New Padding(0)
        L_PtSE.Name = "L_PtSE"
        L_PtSE.Size = New Size(86, 21)
        L_PtSE.TabIndex = 25
        L_PtSE.Text = "Point SE"
        L_PtSE.TextAlign = ContentAlignment.MiddleCenter
        '
        'Point2
        '
        Point2.BackColor = Color.White
        Point2.BorderStyle = BorderStyle.FixedSingle
        Point2.Location = New Point(91, 46)
        Point2.Margin = New Padding(0)
        Point2.Name = "Point2"
        Point2.Size = New Size(256, 21)
        Point2.TabIndex = 35
        Point2.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Point2, resources.GetString("Point2.ToolTip"))
        '
        'L_Largeur
        '
        L_Largeur.BackColor = Color.Transparent
        L_Largeur.BorderStyle = BorderStyle.FixedSingle
        L_Largeur.Location = New Point(6, 66)
        L_Largeur.Margin = New Padding(0)
        L_Largeur.Name = "L_Largeur"
        L_Largeur.Size = New Size(86, 21)
        L_Largeur.TabIndex = 29
        L_Largeur.Text = "Largeur (m)"
        L_Largeur.TextAlign = ContentAlignment.MiddleCenter
        '
        'Largeur
        '
        Largeur.BackColor = Color.Transparent
        Largeur.BorderStyle = BorderStyle.FixedSingle
        Largeur.Location = New Point(91, 66)
        Largeur.Margin = New Padding(0)
        Largeur.Name = "Largeur"
        Largeur.Size = New Size(86, 21)
        Largeur.TabIndex = 33
        Largeur.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Largeur, "Largeur approximative de la zone de téléchargement exprimée en mètres.")
        '
        'L_Surface
        '
        L_Surface.BackColor = Color.Transparent
        L_Surface.BorderStyle = BorderStyle.FixedSingle
        L_Surface.Location = New Point(176, 66)
        L_Surface.Margin = New Padding(0)
        L_Surface.Name = "L_Surface"
        L_Surface.Size = New Size(171, 21)
        L_Surface.TabIndex = 61
        L_Surface.Text = "Surface (km²) : "
        L_Surface.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Hauteur
        '
        L_Hauteur.BackColor = Color.Transparent
        L_Hauteur.BorderStyle = BorderStyle.FixedSingle
        L_Hauteur.Location = New Point(6, 86)
        L_Hauteur.Margin = New Padding(0)
        L_Hauteur.Name = "L_Hauteur"
        L_Hauteur.Size = New Size(86, 21)
        L_Hauteur.TabIndex = 30
        L_Hauteur.Text = "Hauteur (m)"
        L_Hauteur.TextAlign = ContentAlignment.MiddleCenter
        '
        'Hauteur
        '
        Hauteur.BackColor = Color.Transparent
        Hauteur.BorderStyle = BorderStyle.FixedSingle
        Hauteur.Location = New Point(91, 86)
        Hauteur.Margin = New Padding(0)
        Hauteur.Name = "Hauteur"
        Hauteur.Size = New Size(86, 21)
        Hauteur.TabIndex = 34
        Hauteur.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Hauteur, "Hauteur approximative de la zone de téléchargement exprimée en mètres.")
        '
        'Surface
        '
        Surface.BackColor = Color.Transparent
        Surface.BorderStyle = BorderStyle.FixedSingle
        Surface.Location = New Point(176, 86)
        Surface.Margin = New Padding(0)
        Surface.Name = "Surface"
        Surface.Size = New Size(171, 21)
        Surface.TabIndex = 37
        Surface.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(Surface, "Surface approximative de la zone de téléchargement exprimée en kilomètres carrés.")
        '
        'L_Colonnes
        '
        L_Colonnes.BackColor = Color.Transparent
        L_Colonnes.BorderStyle = BorderStyle.FixedSingle
        L_Colonnes.Location = New Point(6, 112)
        L_Colonnes.Margin = New Padding(0)
        L_Colonnes.Name = "L_Colonnes"
        L_Colonnes.Size = New Size(171, 21)
        L_Colonnes.TabIndex = 48
        L_Colonnes.Text = "Colonnes : Ouest-Est"
        L_Colonnes.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Rangees
        '
        L_Rangees.BackColor = Color.Transparent
        L_Rangees.BorderStyle = BorderStyle.FixedSingle
        L_Rangees.Location = New Point(176, 112)
        L_Rangees.Margin = New Padding(0)
        L_Rangees.Name = "L_Rangees"
        L_Rangees.Size = New Size(171, 21)
        L_Rangees.TabIndex = 49
        L_Rangees.Text = "Rangées : Nord-Sud"
        L_Rangees.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_IndexCol
        '
        L_IndexCol.BackColor = Color.Transparent
        L_IndexCol.BorderStyle = BorderStyle.FixedSingle
        L_IndexCol.Location = New Point(6, 132)
        L_IndexCol.Margin = New Padding(0)
        L_IndexCol.Name = "L_IndexCol"
        L_IndexCol.Size = New Size(86, 21)
        L_IndexCol.TabIndex = 50
        L_IndexCol.Text = "Index"
        L_IndexCol.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_NbCol
        '
        L_NbCol.BackColor = Color.Transparent
        L_NbCol.BorderStyle = BorderStyle.FixedSingle
        L_NbCol.Location = New Point(91, 132)
        L_NbCol.Margin = New Padding(0)
        L_NbCol.Name = "L_NbCol"
        L_NbCol.Size = New Size(86, 21)
        L_NbCol.TabIndex = 51
        L_NbCol.Text = "Nombre"
        L_NbCol.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_IndexRow
        '
        L_IndexRow.BackColor = Color.Transparent
        L_IndexRow.BorderStyle = BorderStyle.FixedSingle
        L_IndexRow.Location = New Point(176, 132)
        L_IndexRow.Margin = New Padding(0)
        L_IndexRow.Name = "L_IndexRow"
        L_IndexRow.Size = New Size(86, 21)
        L_IndexRow.TabIndex = 52
        L_IndexRow.Text = "Index"
        L_IndexRow.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_NbRow
        '
        L_NbRow.BackColor = Color.Transparent
        L_NbRow.BorderStyle = BorderStyle.FixedSingle
        L_NbRow.Location = New Point(261, 132)
        L_NbRow.Margin = New Padding(0)
        L_NbRow.Name = "L_NbRow"
        L_NbRow.Size = New Size(86, 21)
        L_NbRow.TabIndex = 53
        L_NbRow.Text = "Nombre"
        L_NbRow.TextAlign = ContentAlignment.MiddleCenter
        '
        'DebCol
        '
        DebCol.BackColor = Color.White
        DebCol.BorderStyle = BorderStyle.FixedSingle
        DebCol.Location = New Point(6, 152)
        DebCol.Margin = New Padding(0)
        DebCol.Name = "DebCol"
        DebCol.Size = New Size(86, 21)
        DebCol.TabIndex = 54
        DebCol.Tag = 0
        DebCol.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(DebCol, "Index de colonne de la tuile en haut et à gauche (NO)" & CrLf &
                                    "de la surface de téléchargement.")
        '
        'NbCol
        '
        NbCol.BackColor = Color.White
        NbCol.BorderStyle = BorderStyle.FixedSingle
        NbCol.Location = New Point(91, 152)
        NbCol.Margin = New Padding(0)
        NbCol.Name = "NbCol"
        NbCol.Size = New Size(86, 21)
        NbCol.TabIndex = 55
        NbCol.Tag = 1
        NbCol.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbCol, "Nb de colonnes de la surface de téléchargement")
        '
        'DebRow
        '
        DebRow.BackColor = Color.White
        DebRow.BorderStyle = BorderStyle.FixedSingle
        DebRow.Location = New Point(176, 152)
        DebRow.Margin = New Padding(0)
        DebRow.Name = "DebRow"
        DebRow.Size = New Size(86, 21)
        DebRow.TabIndex = 56
        DebRow.Tag = 2
        DebRow.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(DebRow, "Index de rangée de la tuile en haut et à gauche (NO)" & CrLf &
                                    "de la surface de téléchargement.")
        '
        'NbRow
        '
        NbRow.BackColor = Color.White
        NbRow.BorderStyle = BorderStyle.FixedSingle
        NbRow.Location = New Point(261, 152)
        NbRow.Margin = New Padding(0)
        NbRow.Name = "NbRow"
        NbRow.Size = New Size(86, 21)
        NbRow.TabIndex = 57
        NbRow.Tag = 3
        NbRow.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbRow, "Nb de rangées de la surface de téléchargement.")
        '
        'L_TuilesTelechargees
        '
        L_TuilesTelechargees.BackColor = Color.Transparent
        L_TuilesTelechargees.BorderStyle = BorderStyle.FixedSingle
        L_TuilesTelechargees.Location = New Point(6, 178)
        L_TuilesTelechargees.Margin = New Padding(0)
        L_TuilesTelechargees.Name = "L_TuilesTelechargees"
        L_TuilesTelechargees.Size = New Size(341, 21)
        L_TuilesTelechargees.TabIndex = 40
        L_TuilesTelechargees.Text = "Tuiles à télécharger"
        L_TuilesTelechargees.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_NbTuilesMaxX
        '
        L_NbTuilesMaxX.BackColor = Color.Transparent
        L_NbTuilesMaxX.BorderStyle = BorderStyle.FixedSingle
        L_NbTuilesMaxX.Location = New Point(6, 198)
        L_NbTuilesMaxX.Margin = New Padding(0)
        L_NbTuilesMaxX.Name = "L_NbTuilesMaxX"
        L_NbTuilesMaxX.Size = New Size(114, 40)
        L_NbTuilesMaxX.TabIndex = 44
        L_NbTuilesMaxX.Text = "Nb maxi tuiles" & CrLf & "Axe Colonnes"
        L_NbTuilesMaxX.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_NbTuilesMax
        '
        L_NbTuilesMax.BackColor = Color.Transparent
        L_NbTuilesMax.BorderStyle = BorderStyle.FixedSingle
        L_NbTuilesMax.Location = New Point(119, 198)
        L_NbTuilesMax.Margin = New Padding(0)
        L_NbTuilesMax.Name = "L_NbTuilesMax"
        L_NbTuilesMax.Size = New Size(115, 40)
        L_NbTuilesMax.TabIndex = 39
        L_NbTuilesMax.Text = "Nb Maxi Tuiles"
        L_NbTuilesMax.TextAlign = ContentAlignment.MiddleCenter

        '
        'L_NbTuilesTelecharger
        '
        L_NbTuilesTelecharger.BackColor = Color.Transparent
        L_NbTuilesTelecharger.BorderStyle = BorderStyle.FixedSingle
        L_NbTuilesTelecharger.Location = New Point(233, 198)
        L_NbTuilesTelecharger.Margin = New Padding(0)
        L_NbTuilesTelecharger.Name = "L_NbTuilesTelecharger"
        L_NbTuilesTelecharger.Size = New Size(114, 40)
        L_NbTuilesTelecharger.TabIndex = 59
        L_NbTuilesTelecharger.Text = "Nb Tuiles" & CrLf & "à télécharger"
        L_NbTuilesTelecharger.TextAlign = ContentAlignment.MiddleCenter
        '
        'NbTuilesMaxX
        '
        NbTuilesMaxX.BackColor = Color.Transparent
        NbTuilesMaxX.BorderStyle = BorderStyle.FixedSingle
        NbTuilesMaxX.Location = New Point(6, 237)
        NbTuilesMaxX.Margin = New Padding(0)
        NbTuilesMaxX.Name = "NbTuilesMaxX"
        NbTuilesMaxX.Size = New Size(114, 21)
        NbTuilesMaxX.TabIndex = 46
        NbTuilesMaxX.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbTuilesMaxX, resources.GetString("NbTuilesMaxX.ToolTip"))
        '
        'NbTuilesMax
        '
        NbTuilesMax.BackColor = Color.Transparent
        NbTuilesMax.BorderStyle = BorderStyle.FixedSingle
        NbTuilesMax.Location = New Point(119, 237)
        NbTuilesMax.Margin = New Padding(0)
        NbTuilesMax.Name = "NbTuilesMax"
        NbTuilesMax.Size = New Size(115, 21)
        NbTuilesMax.TabIndex = 42
        NbTuilesMax.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbTuilesMax, "Nombre de tuiles maximum qui peuvent être" & CrLf &
                                         "téléchargées pour le support de carte en cours." & CrLf &
                                         "Ok si vert, Not Ok si rouge")
        '
        'NbTuilesTelecharger
        '
        NbTuilesTelecharger.BackColor = Color.Transparent
        NbTuilesTelecharger.BorderStyle = BorderStyle.FixedSingle
        NbTuilesTelecharger.Location = New Point(233, 237)
        NbTuilesTelecharger.Margin = New Padding(0)
        NbTuilesTelecharger.Name = "NbTuilesTelecharger"
        NbTuilesTelecharger.Size = New Size(114, 21)
        NbTuilesTelecharger.TabIndex = 60
        NbTuilesTelecharger.TextAlign = ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Support.BackColor = Color.FromArgb(247, 247, 247)
        'obligation pour que la touche Tab fonctionne correctement :
        'Tab --> déplacement dans les saisies d'index et de nombres de colonnes et de rangées
        'L'index du contrôle de saisie dans la collection doit correspondre à l'énumération NI
        Support.Controls.Add(DebCol) 'NI.DebCol
        Support.Controls.Add(NbCol)  'NI.NbCol
        Support.Controls.Add(DebRow) 'NI.DebRow
        Support.Controls.Add(NbRow)  'NI.NbRow
        Support.Controls.Add(L_Delimitation) '0
        Support.Controls.Add(L_PtNO)
        Support.Controls.Add(Point1)
        Support.Controls.Add(L_PtSE)
        Support.Controls.Add(Point2)
        Support.Controls.Add(L_Largeur)
        Support.Controls.Add(Largeur)
        Support.Controls.Add(L_Surface)
        Support.Controls.Add(Surface)
        Support.Controls.Add(L_Hauteur)
        Support.Controls.Add(Hauteur)
        Support.Controls.Add(L_Colonnes)
        Support.Controls.Add(L_Rangees)
        Support.Controls.Add(L_IndexCol)
        Support.Controls.Add(L_NbCol)
        Support.Controls.Add(L_IndexRow)
        Support.Controls.Add(L_NbRow)
        Support.Controls.Add(L_TuilesTelechargees)
        Support.Controls.Add(L_NbTuilesMaxX)
        Support.Controls.Add(L_NbTuilesMax)
        Support.Controls.Add(L_NbTuilesTelecharger)
        Support.Controls.Add(NbTuilesMax)
        Support.Controls.Add(NbTuilesMaxX)
        Support.Controls.Add(NbTuilesTelecharger)
        Support.Location = New Point(0, 0)
        Support.Name = "Panel1"
        Support.Size = New Size(353, 264)
        Support.TabIndex = 62

        '
        'Valider
        '
        Valider.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Valider.DialogResult = DialogResult.OK
        Valider.Location = New Point(272, 270)
        Valider.Name = "Valider"
        Valider.Size = New Size(75, 30)
        Valider.TabIndex = 14
        Valider.Text = "Valider"
        ToolTip1.SetToolTip(Valider, "Ferme le formulaire en prenant en compte les modifications." & CrLf &
                                     "Le Point NO devient le point Pt1 de l'affichage et " & CrLf &
                                     "le Point SE devient le point Pt2 de l'affichage.")
        Valider.UseVisualStyleBackColor = True
        '
        'Annuler
        '
        Annuler.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Annuler.DialogResult = DialogResult.Cancel
        Annuler.Location = New Point(191, 270)
        Annuler.Name = "Annuler"
        Annuler.Size = New Size(75, 30)
        Annuler.TabIndex = 58
        Annuler.Text = "Annuler"
        ToolTip1.SetToolTip(Annuler, "Ferme le formulaire sans prendre en compte les modifications.")
        Annuler.UseVisualStyleBackColor = True
        '
        'ToolTip1
        '
        ToolTip1.AutoPopDelay = 5000
        ToolTip1.InitialDelay = 100
        ToolTip1.ReshowDelay = 100
        '
        'SaisieIndex
        '
        SaisieIndex.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        SaisieIndex.BackColor = Color.White
        SaisieIndex.BorderStyle = BorderStyle.None
        SaisieIndex.Location = New Point(0, 0)
        SaisieIndex.Margin = New Padding(0)
        SaisieIndex.Name = "SaisieIndex"
        SaisieIndex.Size = New Size(82, 19)
        SaisieIndex.TabIndex = 0
        SaisieIndex.TabStop = False
        ToolTip1.SetToolTip(SaisieIndex, "")
        SaisieIndex.Visible = False
        '
        'SaisieSurfaceCaptureTuile
        '
        AcceptButton = Valider
        AccessibleRole = AccessibleRole.Cursor
        AutoScaleMode = AutoScaleMode.None
        CancelButton = Annuler
        ClientSize = New Size(353, 306)
        ControlBox = False
        Controls.Add(SaisieIndex)
        Controls.Add(Support)
        Controls.Add(Annuler)
        Controls.Add(Valider)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        MaximizeBox = False
        MinimizeBox = False
        Name = "SaisieSurfaceCaptureTuile"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterParent
        'TopMost = True
        Support.ResumeLayout(False)
        Support.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private Valider As Button
    Private L_Delimitation As Label
    Private L_PtNO As Label
    Private L_PtSE As Label
    Private L_Largeur As Label
    Private L_Hauteur As Label
    Private Point1 As Label
    Private Hauteur As Label
    Private Largeur As Label
    Private Point2 As Label
    Private Surface As Label
    Private L_NbTuilesMax As Label
    Private L_TuilesTelechargees As Label
    Private NbTuilesMax As Label
    Private L_NbTuilesMaxX As Label
    Private NbTuilesMaxX As Label
    Private L_Rangees As Label
    Private L_Colonnes As Label
    Private L_IndexCol As Label
    Private L_NbCol As Label
    Private L_NbRow As Label
    Private L_IndexRow As Label
    Private NbRow As Label
    Private DebRow As Label
    Private NbCol As Label
    Private DebCol As Label
    Private SaisieIndex As TextBox
    Private Annuler As Button
    Private ToolTip1 As ToolTip
    Private L_NbTuilesTelecharger As Label
    Private NbTuilesTelecharger As Label
    Private L_Surface As Label
    Private Support As Panel
End Class
