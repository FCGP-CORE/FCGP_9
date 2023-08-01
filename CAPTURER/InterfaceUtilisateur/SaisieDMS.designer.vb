Partial Class SaisieDMS
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler DegX.KeyDown, AddressOf Coord_KeyDown
        AddHandler DegX.KeyPress, AddressOf Coord_KeyPress
        AddHandler MinX.KeyDown, AddressOf Coord_KeyDown
        AddHandler MinX.KeyPress, AddressOf Coord_KeyPress
        AddHandler SecX.KeyDown, AddressOf Coord_KeyDown
        AddHandler SecX.KeyPress, AddressOf Coord_KeyPress
        AddHandler SecX.Leave, AddressOf Coord_Leave
        AddHandler DegY.KeyDown, AddressOf Coord_KeyDown
        AddHandler DegY.KeyPress, AddressOf Coord_KeyPress
        AddHandler MinY.KeyDown, AddressOf Coord_KeyDown
        AddHandler MinY.KeyPress, AddressOf Coord_KeyPress
        AddHandler SecY.KeyDown, AddressOf Coord_KeyDown
        AddHandler SecY.KeyPress, AddressOf Coord_KeyPress
        AddHandler SecY.Leave, AddressOf Coord_Leave
        'Longitude
        AddHandler LabelLongitude.Click, AddressOf LabelLongitude_Click
        AddHandler Longitude.SelectedIndexChanged, AddressOf Longitude_SelectedIndexChanged
        'Latitude
        AddHandler LabelLatitude.Click, AddressOf LabelLatitude_Click
        AddHandler Latitude.SelectedIndexChanged, AddressOf Latitude_SelectedIndexChanged
        'formulaire
        AddHandler Me.Load, AddressOf SaisieDMS_Load
        AddHandler Me.FormClosing, AddressOf SaisieDMS_FormClosing
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
        LabelLongitude = New Label()
        DegY = New TextBox()
        L_Latitude = New Label()
        ESC = New Button()
        OK = New Button()
        L_Deg = New Label()
        L_Sec = New Label()
        L_Min = New Label()
        Longitude = New ComboBox()
        Latitude = New ComboBox()
        L_Sens = New Label()
        SecY = New TextBox()
        MinY = New TextBox()
        SecX = New TextBox()
        MinX = New TextBox()
        DegX = New TextBox()
        LabelLatitude = New Label()
        L_Longitude = New Label()
        Support = New Panel()
        SuspendLayout()
        '
        'L_Sens
        '
        L_Sens.BackColor = Color.Transparent
        L_Sens.BorderStyle = BorderStyle.FixedSingle
        L_Sens.Location = New Point(84, 1)
        L_Sens.Margin = New Padding(0)
        L_Sens.Name = "L_Sens"
        L_Sens.Size = New Size(40, 21)
        L_Sens.TabIndex = 23
        L_Sens.Text = "Sens"
        L_Sens.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Deg
        '
        L_Deg.BackColor = Color.Transparent
        L_Deg.BorderStyle = BorderStyle.FixedSingle
        L_Deg.Location = New Point(123, 1)
        L_Deg.Margin = New Padding(0)
        L_Deg.Name = "L_Deg"
        L_Deg.Size = New Size(36, 21)
        L_Deg.TabIndex = 20
        L_Deg.Text = "Deg"
        L_Deg.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Min
        '
        L_Min.BackColor = Color.Transparent
        L_Min.BorderStyle = BorderStyle.FixedSingle
        L_Min.Location = New Point(158, 1)
        L_Min.Margin = New Padding(0)
        L_Min.Name = "L_Min"
        L_Min.Size = New Size(36, 21)
        L_Min.TabIndex = 20
        L_Min.Text = "Min"
        L_Min.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Sec
        '
        L_Sec.BackColor = Color.Transparent
        L_Sec.BorderStyle = BorderStyle.FixedSingle
        L_Sec.Location = New Point(193, 1)
        L_Sec.Margin = New Padding(0)
        L_Sec.Name = "L_Sec"
        L_Sec.Size = New Size(51, 21)
        L_Sec.TabIndex = 20
        L_Sec.Text = "Sec"
        L_Sec.TextAlign = ContentAlignment.MiddleCenter
        '
        'L_Longitude
        '
        L_Longitude.BackColor = Color.Transparent
        L_Longitude.BorderStyle = BorderStyle.FixedSingle
        L_Longitude.Location = New Point(1, 21)
        L_Longitude.Margin = New Padding(0)
        L_Longitude.Name = "L_Longitude"
        L_Longitude.Size = New Size(84, 26)
        L_Longitude.TabIndex = 25
        L_Longitude.Text = "Longitude : "
        L_Longitude.TextAlign = ContentAlignment.MiddleLeft
        '
        'LabelLongitude
        '
        LabelLongitude.BackColor = Color.White
        LabelLongitude.BorderStyle = BorderStyle.FixedSingle
        LabelLongitude.Location = New Point(84, 21)
        LabelLongitude.Margin = New Padding(0)
        LabelLongitude.Name = "LabelLongitude"
        LabelLongitude.Size = New Size(40, 26)
        LabelLongitude.TabIndex = 20
        LabelLongitude.TextAlign = ContentAlignment.MiddleCenter
        '
        'Longitude
        '
        Longitude.BackColor = Color.White
        Longitude.FormattingEnabled = True
        Longitude.Items.AddRange(New Object() {"E", "O"})
        Longitude.Location = New Point(84, 19)
        Longitude.Name = "Longitude"
        Longitude.Size = New Size(40, 20)
        Longitude.TabIndex = 0
        Longitude.Visible = False
        '
        'DegX
        '
        DegX.BackColor = Color.White
        DegX.BorderStyle = BorderStyle.FixedSingle
        DegX.Location = New Point(123, 21)
        DegX.Margin = New Padding(0)
        DegX.MaxLength = 3
        DegX.Name = "DegX"
        DegX.Size = New Size(36, 26)
        DegX.TabIndex = 1
        '
        'MinX
        '
        MinX.BackColor = Color.White
        MinX.BorderStyle = BorderStyle.FixedSingle
        MinX.Location = New Point(158, 21)
        MinX.Margin = New Padding(0)
        MinX.MaxLength = 2
        MinX.Name = "MinX"
        MinX.Size = New Size(36, 26)
        MinX.TabIndex = 2
        '
        'SecX
        '
        SecX.BackColor = Color.White
        SecX.BorderStyle = BorderStyle.FixedSingle
        SecX.Location = New Point(193, 21)
        SecX.Margin = New Padding(0)
        SecX.MaxLength = 5
        SecX.Name = "SecX"
        SecX.Size = New Size(51, 26)
        SecX.TabIndex = 3
        '
        'L_Latitude
        '
        L_Latitude.BackColor = Color.Transparent
        L_Latitude.BorderStyle = BorderStyle.FixedSingle
        L_Latitude.Location = New Point(1, 46)
        L_Latitude.Margin = New Padding(0)
        L_Latitude.Name = "L_Latitude"
        L_Latitude.Size = New Size(84, 26)
        L_Latitude.TabIndex = 20
        L_Latitude.Text = "Latitude : "
        L_Latitude.TextAlign = ContentAlignment.MiddleLeft
        '
        'LabelLatitude
        '
        LabelLatitude.BackColor = Color.White
        LabelLatitude.BorderStyle = BorderStyle.FixedSingle
        LabelLatitude.Location = New Point(84, 46)
        LabelLatitude.Margin = New Padding(0)
        LabelLatitude.Name = "LabelLatitude"
        LabelLatitude.Size = New Size(40, 26)
        LabelLatitude.TabIndex = 24
        LabelLatitude.TextAlign = ContentAlignment.MiddleCenter
        '
        'Latitude
        '
        Latitude.BackColor = Color.White
        Latitude.FormattingEnabled = True
        Latitude.Items.AddRange(New Object() {"N", "S"})
        Latitude.Location = New Point(84, 44)
        Latitude.Margin = New Padding(0)
        Latitude.Name = "Latitude"
        Latitude.Size = New Size(40, 20)
        Latitude.TabIndex = 4
        Latitude.Visible = False
        '
        'DegY
        '
        DegY.BackColor = Color.White
        DegY.BorderStyle = BorderStyle.FixedSingle
        DegY.Location = New Point(123, 46)
        DegY.Margin = New Padding(0)
        DegY.MaxLength = 2
        DegY.Name = "DegY"
        DegY.Size = New Size(36, 26)
        DegY.TabIndex = 5
        '
        'MinY
        '
        MinY.BackColor = Color.White
        MinY.BorderStyle = BorderStyle.FixedSingle
        MinY.Location = New Point(158, 46)
        MinY.Margin = New Padding(0)
        MinY.MaxLength = 2
        MinY.Name = "MinY"
        MinY.Size = New Size(36, 26)
        MinY.TabIndex = 6
        '
        'SecY
        '
        SecY.BackColor = Color.White
        SecY.BorderStyle = BorderStyle.FixedSingle
        SecY.Location = New Point(193, 46)
        SecY.Margin = New Padding(0)
        SecY.MaxLength = 5
        SecY.Name = "SecY"
        SecY.Size = New Size(51, 26)
        SecY.TabIndex = 7
        '
        'Support
        '
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(245, 78)
        Support.TabIndex = 62
        Support.Controls.Add(L_Longitude)
        Support.Controls.Add(LabelLatitude)
        Support.Controls.Add(DegY)
        Support.Controls.Add(DegX)
        Support.Controls.Add(L_Latitude)
        Support.Controls.Add(LabelLongitude)
        Support.Controls.Add(L_Sens)
        Support.Controls.Add(SecY)
        Support.Controls.Add(MinY)
        Support.Controls.Add(SecX)
        Support.Controls.Add(MinX)
        Support.Controls.Add(L_Min)
        Support.Controls.Add(L_Sec)
        Support.Controls.Add(L_Deg)
        Support.Controls.Add(Latitude)
        Support.Controls.Add(Longitude)
        '
        'ESC
        '
        ESC.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ESC.DialogResult = DialogResult.Cancel
        ESC.Location = New Point(93, 84)
        ESC.Name = "ESC"
        ESC.Size = New Size(70, 30)
        ESC.TabIndex = 8
        ESC.TabStop = False
        ESC.Text = "Annuler"
        ESC.UseVisualStyleBackColor = True
        '
        'OK
        '
        OK.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        OK.DialogResult = DialogResult.OK
        OK.Location = New Point(169, 84)
        OK.Name = "OK"
        OK.Size = New Size(70, 30)
        OK.TabIndex = 9
        OK.TabStop = False
        OK.Text = "Valider"
        OK.UseVisualStyleBackColor = True
        '
        'SaisieDMS
        '
        AcceptButton = OK
        AccessibleRole = AccessibleRole.Cursor
        AutoScaleMode = AutoScaleMode.None
        CancelButton = ESC
        ClientSize = New Size(245, 120)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(OK)
        Controls.Add(ESC)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "SaisieDMS"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterParent
        TopMost = True
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private LabelLongitude As Label
    Private DegY As TextBox
    Private L_Latitude As Label
    Private ESC As Button
    Private OK As Button
    Private L_Deg As Label
    Private L_Sec As Label
    Private L_Min As Label
    Private Longitude As ComboBox
    Private Latitude As ComboBox
    Private L_Sens As Label
    Private SecY As TextBox
    Private MinY As TextBox
    Private SecX As TextBox
    Private MinX As TextBox
    Private DegX As TextBox
    Private LabelLatitude As Label
    Private L_Longitude As Label
    Private Support As Panel
End Class
