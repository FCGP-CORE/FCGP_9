Partial Class AfficheGraphiqueTrace
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'plot
        AddHandler Plot1.DoubleClicked, AddressOf Plot1_DoubleClicked
        AddHandler Plot1.AxesChanged, AddressOf Plot1_AxesChanged
        AddHandler Plot1.MouseMove, AddressOf Plot1_MouseMove
        AddHandler Plot1.MouseEnter, AddressOf Plot1_MouseEnter
        AddHandler Plot1.MouseLeave, AddressOf Plot1_MouseLeave
        'formulaire
        AddHandler Load, AddressOf AfficheGraphiqueTrace_Load
        AddHandler FormClosed, AddressOf AfficheGraphiqueTrace_FormClosed
        AddHandler KeyDown, AddressOf AfficheGraphiqueTrace_KeyDown
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
    Private Sub InitializeComponent()
        components = New Container()
        Plot1 = New ControlScottPlot()
        SuspendLayout()
        '
        'Plot1
        '
        Plot1.Location = New Point(0, 0)
        Plot1.Dock = DockStyle.Fill
        Plot1.Margin = New Padding(0, 0, 0, 0)
        Plot1.Name = "Plot1"
        Plot1.Size = New Size(685, 373)
        Plot1.TabIndex = 0
        '
        'formulaire
        '
        MaximizeBox = False
        MinimizeBox = False
        ControlBox = False
        AutoScaleMode = AutoScaleMode.None
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        KeyPreview = True
        Controls.Add(Plot1)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        Name = "AfficheGraphique"
        SizeGripStyle = SizeGripStyle.Hide
        StartPosition = FormStartPosition.Manual
        ResumeLayout(False)
    End Sub

    Private Plot1 As ControlScottPlot
End Class
