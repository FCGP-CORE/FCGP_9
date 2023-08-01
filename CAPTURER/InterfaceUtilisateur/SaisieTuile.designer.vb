Partial Class SaisieTuile
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler Row.KeyDown, AddressOf Coord_KeyDown
        AddHandler Row.KeyPress, AddressOf Coord_KeyPress
        AddHandler DecalY.KeyDown, AddressOf Coord_KeyDown
        AddHandler DecalY.KeyPress, AddressOf Coord_KeyPress
        AddHandler DecalX.KeyDown, AddressOf Coord_KeyDown
        AddHandler DecalX.KeyPress, AddressOf Coord_KeyPress
        AddHandler Col.KeyDown, AddressOf Coord_KeyDown
        AddHandler Col.KeyPress, AddressOf Coord_KeyPress
        'formulaire
        AddHandler Load, AddressOf SaisieTuile_Load
        AddHandler FormClosing, AddressOf SaisieTuile_FormClosing
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
        L_Col = New Label()
        Row = New TextBox()
        L_Row = New Label()
        ESC = New Button()
        OK = New Button()
        L_IndexTuile = New Label()
        L_DecalagePixels = New Label()
        DecalY = New TextBox()
        DecalX = New TextBox()
        Col = New TextBox()
        Support = New Panel()
        SuspendLayout()
        '
        'L_IndexTuile
        '
        L_IndexTuile.BorderStyle = BorderStyle.FixedSingle
        L_IndexTuile.Location = New Point(45, 1)
        L_IndexTuile.Margin = New Padding(0)
        L_IndexTuile.Name = "L_IndexTuile"
        L_IndexTuile.Size = New Size(85, 21)
        L_IndexTuile.TabIndex = 20
        L_IndexTuile.Text = "Index Tuile"
        L_IndexTuile.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_DecalagePixels
        '
        L_DecalagePixels.BorderStyle = BorderStyle.FixedSingle
        L_DecalagePixels.Location = New Point(129, 1)
        L_DecalagePixels.Margin = New Padding(0)
        L_DecalagePixels.Name = "L_DecalagePixels"
        L_DecalagePixels.Size = New Size(85, 21)
        L_DecalagePixels.TabIndex = 20
        L_DecalagePixels.Text = "Decalage Pixels"
        L_DecalagePixels.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Col
        '
        L_Col.BorderStyle = BorderStyle.FixedSingle
        L_Col.Location = New Point(1, 21)
        L_Col.Margin = New Padding(0)
        L_Col.Name = "L_Col"
        L_Col.Size = New Size(45, 26)
        L_Col.TabIndex = 20
        L_Col.Text = "Col : "
        L_Col.TextAlign = ContentAlignment.MiddleLeft
        '
        'Col
        '
        Col.BackColor = Color.White
        Col.BorderStyle = BorderStyle.FixedSingle
        Col.Location = New Point(45, 21)
        Col.Margin = New Padding(0)
        Col.MaxLength = 5
        Col.Name = "Col"
        Col.Size = New Size(85, 26)
        Col.TabIndex = 0
        '
        'DecalX
        '
        DecalX.BackColor = Color.White
        DecalX.BorderStyle = BorderStyle.FixedSingle
        DecalX.Location = New Point(129, 21)
        DecalX.Margin = New Padding(0)
        DecalX.MaxLength = 3
        DecalX.Name = "DecalX"
        DecalX.Size = New Size(85, 26)
        DecalX.TabIndex = 1
        '
        'L_Row
        '
        L_Row.BorderStyle = BorderStyle.FixedSingle
        L_Row.Location = New Point(1, 46)
        L_Row.Margin = New Padding(0)
        L_Row.Name = "L_Row"
        L_Row.Size = New Size(45, 26)
        L_Row.TabIndex = 20
        L_Row.Text = "Row : "
        L_Row.TextAlign = ContentAlignment.MiddleLeft
        '
        'Row
        '
        Row.BackColor = Color.White
        Row.BorderStyle = BorderStyle.FixedSingle
        Row.Location = New Point(45, 46)
        Row.Margin = New Padding(0)
        Row.MaxLength = 5
        Row.Name = "Row"
        Row.Size = New Size(85, 26)
        Row.TabIndex = 2
        '
        'DecalY
        '
        DecalY.BackColor = Color.White
        DecalY.BorderStyle = BorderStyle.FixedSingle
        DecalY.Location = New Point(129, 46)
        DecalY.Margin = New Padding(0)
        DecalY.MaxLength = 3
        DecalY.Name = "DecalY"
        DecalY.Size = New Size(85, 26)
        DecalY.TabIndex = 3
        '
        'Support
        '
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(215, 78)
        Support.TabIndex = 62
        Support.Controls.Add(L_IndexTuile)
        Support.Controls.Add(L_DecalagePixels)
        Support.Controls.Add(L_Col)
        Support.Controls.Add(Col)
        Support.Controls.Add(DecalX)
        Support.Controls.Add(L_Row)
        Support.Controls.Add(Row)
        Support.Controls.Add(DecalY)
        '
        'ESC
        '
        ESC.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ESC.DialogResult = DialogResult.Cancel
        ESC.Location = New Point(63, 84)
        ESC.Name = "ESC"
        ESC.Size = New Size(70, 30)
        ESC.TabIndex = 4
        ESC.TabStop = False
        ESC.Text = "Annuler"
        ESC.UseVisualStyleBackColor = True
        '
        'OK
        '
        OK.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        OK.DialogResult = DialogResult.OK
        OK.Location = New Point(139, 84)
        OK.Name = "OK"
        OK.Size = New Size(70, 30)
        OK.TabIndex = 5
        OK.TabStop = False
        OK.Text = "Valider"
        OK.UseVisualStyleBackColor = True
        '
        'SaisieTuile
        '
        AcceptButton = OK
        AccessibleRole = AccessibleRole.Cursor
        AutoScaleMode = AutoScaleMode.None
        CancelButton = ESC
        ClientSize = New Size(215, 120)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(OK)
        Controls.Add(ESC)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "SaisieTuile"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterParent
        TopMost = True
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private L_Col As Label
    Private Row As TextBox
    Private L_Row As Label
    Private ESC As Button
    Private OK As Button
    Private L_IndexTuile As Label
    Private L_DecalagePixels As Label
    Private DecalY As TextBox
    Private DecalX As TextBox
    Private Col As TextBox
    Private Support As Panel
End Class
