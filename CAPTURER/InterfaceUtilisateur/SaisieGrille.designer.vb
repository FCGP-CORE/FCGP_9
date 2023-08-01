Partial Class SaisieGrille
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
        'formulaire
        AddHandler Load, AddressOf SaisieGrille_Load
        AddHandler FormClosing, AddressOf SaisieGrille_FormClosing
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
        CoordX = New TextBox()
        L_CoordX = New Label()
        CoordY = New TextBox()
        L_CoordY = New Label()
        Support = New Panel()
        ESC = New Button()
        OK = New Button()
        SuspendLayout()
        '
        'L_CoordX
        '
        L_CoordX.BorderStyle = BorderStyle.FixedSingle
        L_CoordX.Location = New Point(1, 1)
        L_CoordX.Margin = New Padding(0)
        L_CoordX.Name = "L_CoordX"
        L_CoordX.Size = New Size(30, 26)
        L_CoordX.TabIndex = 5
        L_CoordX.Text = "X : "
        L_CoordX.TextAlign = ContentAlignment.MiddleCenter
        '
        'CoordX
        '
        CoordX.BackColor = Color.White
        CoordX.BorderStyle = BorderStyle.FixedSingle
        CoordX.Location = New Point(30, 1)
        CoordX.Margin = New Padding(0)
        CoordX.MaxLength = 9
        CoordX.Name = "CoordX"
        CoordX.Size = New Size(127, 26)
        CoordX.TabIndex = 0
        '
        'L_CoordY
        '
        L_CoordY.BorderStyle = BorderStyle.FixedSingle
        L_CoordY.Location = New Point(1, 26)
        L_CoordY.Margin = New Padding(0)
        L_CoordY.Name = "L_CoordY"
        L_CoordY.Size = New Size(30, 26)
        L_CoordY.TabIndex = 5
        L_CoordY.Text = "Y : "
        L_CoordY.TextAlign = ContentAlignment.MiddleCenter
        '
        'CoordY
        '
        CoordY.BackColor = Color.White
        CoordY.BorderStyle = BorderStyle.FixedSingle
        CoordY.Location = New Point(30, 26)
        CoordY.Margin = New Padding(0)
        CoordY.MaxLength = 10
        CoordY.Name = "CoordY"
        CoordY.Size = New Size(127, 26)
        CoordY.TabIndex = 1
        '
        'Support
        '
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(158, 58)
        Support.TabIndex = 62
        Support.Controls.Add(L_CoordX)
        Support.Controls.Add(CoordX)
        Support.Controls.Add(L_CoordY)
        Support.Controls.Add(CoordY)
        '
        'OK
        '
        OK.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        OK.DialogResult = DialogResult.OK
        OK.Location = New Point(82, 64)
        OK.Name = "OK"
        OK.Size = New Size(70, 30)
        OK.TabIndex = 3
        OK.TabStop = False
        OK.Text = "Valider"
        OK.UseVisualStyleBackColor = True
        '
        'ESC
        '
        ESC.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ESC.DialogResult = DialogResult.Cancel
        ESC.Location = New Point(6, 64)
        ESC.Name = "ESC"
        ESC.Size = New Size(70, 30)
        ESC.TabIndex = 2
        ESC.TabStop = False
        ESC.Text = "Annuler"
        ESC.UseVisualStyleBackColor = True
        '
        'SaisieGrille
        '
        AcceptButton = OK
        AccessibleRole = AccessibleRole.Cursor
        AutoScaleMode = AutoScaleMode.None
        CancelButton = ESC
        ClientSize = New Size(158, 100)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(OK)
        Controls.Add(ESC)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "SaisieGrille"
        ShowIcon = False
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.CenterParent
        TopMost = True
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private CoordX As TextBox
    Private L_CoordX As Label
    Private CoordY As TextBox
    Private L_CoordY As Label
    Private Support As Panel
    Private ESC As Button
    Private OK As Button
End Class
