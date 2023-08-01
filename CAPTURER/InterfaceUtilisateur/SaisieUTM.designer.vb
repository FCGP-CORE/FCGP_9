Partial Class SaisieUTM
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler CoordX.KeyDown, AddressOf CoordXY_KeyDown
        AddHandler CoordX.KeyPress, AddressOf CoordXY_KeyPress
        AddHandler CoordY.KeyDown, AddressOf CoordXY_KeyDown
        AddHandler CoordY.KeyPress, AddressOf CoordXY_KeyPress
        'ZONE
        AddHandler LabelZone.Click, AddressOf LabelZONE_Click
        AddHandler Zone.SelectedIndexChanged, AddressOf ZONE_SelectedIndexChanged
        'Hem
        AddHandler LabelHem.Click, AddressOf LabelHem_Click
        AddHandler Hem.SelectedIndexChanged, AddressOf HEM_SelectedIndexChanged
        'formulaire
        AddHandler Load, AddressOf SaisieUTM_Load
        AddHandler FormClosing, AddressOf SaisieUTM_FormClosing
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
        SuspendLayout()
        '
        'L_Hem
        '
        L_Hem = New Label() With {
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(1, 1),
            .Margin = New Padding(0),
            .Name = "L_Hem",
            .Size = New Size(50, 26),
            .TabIndex = 10,
            .Text = "Hem. : ",
            .TextAlign = ContentAlignment.MiddleLeft}
        '
        'LabelHem
        '
        LabelHem = New Label() With {
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(50, 1),
            .Margin = New Padding(0),
            .Name = "LabelHem",
            .Size = New Size(30, 26),
            .TabIndex = 12,
            .TextAlign = ContentAlignment.MiddleCenter}
        '
        'Hem
        '
        Hem = New ComboBox() With {
            .BackColor = Color.White,
            .DropDownWidth = 30,
            .FormattingEnabled = True,
            .Location = New Point(50, -1),
            .Name = "Hem",
            .Size = New Size(30, 0),
            .TabIndex = 0,
            .Visible = False}
        Hem.Items.AddRange(New Object() {"N", "S"})
        '
        'L_X
        '
        L_X = New Label() With {
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(79, 1),
            .Margin = New Padding(0),
            .Name = "L_X",
            .Size = New Size(26, 26),
            .TabIndex = 5,
            .Text = "X : ",
            .TextAlign = ContentAlignment.MiddleCenter}
        '
        'CoordX
        '
        CoordX = New TextBox() With {
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(104, 1),
            .Margin = New Padding(0),
            .MaxLength = 9,
            .Name = "CoordX",
            .Size = New Size(105, 26),
            .TabIndex = 2}
        '
        'L_Zone
        '
        L_Zone = New Label() With {
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(1, 26),
            .Margin = New Padding(0),
            .Name = "L_Zone",
            .Size = New Size(50, 26),
            .TabIndex = 9,
            .Text = "Zone : ",
            .TextAlign = ContentAlignment.MiddleCenter}
        '
        'LabelZone
        '
        LabelZone = New Label() With {
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(50, 26),
            .Margin = New Padding(0),
            .Name = "LabelZone",
            .Size = New Size(30, 26),
            .TabIndex = 11,
            .TextAlign = ContentAlignment.MiddleCenter}
        '
        'Zone
        '
        Zone = New ComboBox() With {
            .BackColor = Color.White,
            .DropDownWidth = 30,
            .FlatStyle = FlatStyle.System,
            .FormattingEnabled = True,
            .Location = New Point(50, 24),
            .Name = "Zone",
            .Size = New Size(30, 20),
            .TabIndex = 1,
            .Visible = False}
        Zone.Items.AddRange(New Object() {"20", "21", "22", "30", "31", "32", "38", "40"})
        '
        'L_Y
        '
        L_Y = New Label() With {
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(79, 26),
            .Margin = New Padding(0),
            .Name = "L_Y",
            .Size = New Size(26, 26),
            .TabIndex = 5,
            .Text = "Y : ",
            .TextAlign = ContentAlignment.MiddleCenter}
        '
        'CoordY
        '
        CoordY = New TextBox() With {
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(104, 26),
            .Margin = New Padding(0),
            .MaxLength = 10,
            .Name = "CoordY",
            .Size = New Size(105, 26),
            .TabIndex = 3}
        '
        'Support
        '
        Support = New Panel() With {
            .BackColor = Color.FromArgb(247, 247, 247),
            .Location = New Point(0, 0),
            .Name = "Support",
            .Size = New Size(210, 58),
            .TabIndex = 62}
        Support.Controls.Add(L_Hem)
        Support.Controls.Add(LabelHem)
        Support.Controls.Add(Hem)
        Support.Controls.Add(L_X)
        Support.Controls.Add(CoordX)
        Support.Controls.Add(L_Zone)
        Support.Controls.Add(LabelZone)
        Support.Controls.Add(Zone)
        Support.Controls.Add(L_Y)
        Support.Controls.Add(CoordY)
        '
        'OK
        '
        OK = New Button() With {
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right,
            .DialogResult = DialogResult.OK,
            .Location = New Point(134, 64),
            .Name = "OK",
            .Size = New Size(70, 30),
            .TabIndex = 5,
            .TabStop = False,
            .Text = "Valider",
            .UseVisualStyleBackColor = True}
        '
        'ESC
        '
        ESC = New Button() With {
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right,
            .DialogResult = DialogResult.Cancel,
            .Location = New Point(58, 64),
            .Name = "ESC",
            .Size = New Size(70, 30),
            .TabIndex = 4,
            .TabStop = False,
            .Text = "Annuler",
            .UseVisualStyleBackColor = True}
        '
        'SaisieUTM
        '
        AcceptButton = OK
        AutoScaleMode = AutoScaleMode.None
        CancelButton = ESC
        ClientSize = New Size(210, 100)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(OK)
        Controls.Add(ESC)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "SaisieUTM"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterParent
        TopMost = True
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private L_Hem As Label
    Private LabelHem As Label
    Private Hem As ComboBox
    Private CoordX As TextBox
    Private L_X As Label
    Private L_Zone As Label
    Private LabelZone As Label
    Private Zone As ComboBox
    Private CoordY As TextBox
    Private L_Y As Label
    Private Support As Panel
    Private ESC As Button
    Private OK As Button
End Class
