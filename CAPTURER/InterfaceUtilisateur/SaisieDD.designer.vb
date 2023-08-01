Partial Class SaisieDD
    Inherits Form
    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôles simples
        AddHandler CoordLon.KeyDown, AddressOf CoordLatLon_KeyDown
        AddHandler CoordLon.KeyPress, AddressOf CoordLatLon_KeyPress
        AddHandler CoordLon.Leave, AddressOf CoordLatLon_Leave
        AddHandler CoordLat.KeyDown, AddressOf CoordLatLon_KeyDown
        AddHandler CoordLat.KeyPress, AddressOf CoordLatLon_KeyPress
        AddHandler CoordLat.Leave, AddressOf CoordLatLon_Leave
        'formulaire
        AddHandler Me.Load, AddressOf SaisieDD_Load
        AddHandler Me.FormClosing, AddressOf SaisieDD_FormClosing
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
        CoordLon = New TextBox()
        L_Longitude = New Label()
        CoordLat = New TextBox()
        L_Latitude = New Label()
        ESC = New Button()
        OK = New Button()
        Support = New Panel()
        SuspendLayout()
        '
        'L_Longitude
        '
        L_Longitude.BorderStyle = BorderStyle.FixedSingle
        L_Longitude.Location = New Point(1, 1)
        L_Longitude.Margin = New Padding(0)
        L_Longitude.Name = "L_Longitude"
        L_Longitude.Size = New Size(85, 26)
        L_Longitude.TabIndex = 5
        L_Longitude.Text = "Longitude : "
        L_Longitude.TextAlign = ContentAlignment.MiddleLeft
        '
        'CoordX
        '
        CoordLon.BackColor = Color.White
        CoordLon.BorderStyle = BorderStyle.FixedSingle
        CoordLon.Location = New Point(85, 1)
        CoordLon.Margin = New Padding(0)
        CoordLon.MaxLength = 10
        CoordLon.Name = "CoordX"
        CoordLon.Size = New Size(108, 26)
        CoordLon.TabIndex = 0
        '
        'L_Latitude
        '
        L_Latitude.BorderStyle = BorderStyle.FixedSingle
        L_Latitude.Location = New Point(1, 26)
        L_Latitude.Margin = New Padding(0)
        L_Latitude.Name = "L_Latitude"
        L_Latitude.Size = New Size(85, 26)
        L_Latitude.TabIndex = 5
        L_Latitude.Text = "Latitude : "
        L_Latitude.TextAlign = ContentAlignment.MiddleLeft

        '
        'CoordY
        '
        CoordLat.BackColor = Color.White
        CoordLat.BorderStyle = BorderStyle.FixedSingle
        CoordLat.Location = New Point(85, 26)
        CoordLat.Margin = New Padding(0)
        CoordLat.MaxLength = 9
        CoordLat.Name = "CoordY"
        CoordLat.Size = New Size(108, 26)
        CoordLat.TabIndex = 1
        '
        'Support
        '
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(194, 58)
        Support.TabIndex = 62
        Support.Controls.Add(CoordLat)
        Support.Controls.Add(L_Latitude)
        Support.Controls.Add(CoordLon)
        Support.Controls.Add(L_Longitude)
        '
        'OK
        '
        OK.DialogResult = DialogResult.OK
        OK.Location = New Point(118, 64)
        OK.Name = "OK"
        OK.Size = New Size(70, 30)
        OK.TabIndex = 2
        OK.TabStop = False
        OK.Text = "Valider"
        OK.UseVisualStyleBackColor = True
        '
        'ESC
        '
        ESC.DialogResult = DialogResult.Cancel
        ESC.Location = New Point(42, 64)
        ESC.Name = "ESC"
        ESC.Size = New Size(70, 30)
        ESC.TabIndex = 3
        ESC.TabStop = False
        ESC.Text = "Annuler"
        ESC.UseVisualStyleBackColor = True
        '
        'SaisieDD
        '
        AcceptButton = OK
        AccessibleRole = AccessibleRole.Cursor
        AutoScaleMode = AutoScaleMode.None
        CancelButton = ESC
        ClientSize = New Size(194, 100)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(OK)
        Controls.Add(ESC)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "SaisieDD"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterParent
        TopMost = True
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private CoordLon As TextBox
    Private L_Longitude As Label
    Private CoordLat As TextBox
    Private L_Latitude As Label
    Private ESC As Button
    Private OK As Button
    Private Support As Panel
End Class
