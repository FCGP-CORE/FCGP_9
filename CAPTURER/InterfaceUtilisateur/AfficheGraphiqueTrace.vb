Imports ScottPlot
Imports ScottPlot.Plottable

Friend Class AfficheGraphiqueTrace
    Private CrossHair As Crosshair
    Private Plt As Plot
    Private PltPoint As ScatterPlot
    Private PltPointLight As MarkerPlot
    Private PltTooltip As Tooltip
    Private PltPoly As Polygon
    Private EpaisseurLinePltPoly As Double
    Private FlagRatio, FlagIndex As Boolean
    Private Coordinates As (x As Double, y As Double)
    Private Point As (x As Double, y As Double, Index As Integer)
    Private Dists(), Vits(), Alts(), Durees() As Double
    Private DeltaRecherche, UnitParSegments As Double
    Private _Trace As TRK, _Is_Vitesse As Boolean
    Private Segments As List(Of (Longueur As Integer, Denivele As Integer, Duree As Integer, Cap As Double, Pente As Double, Vitesse As Double))

    Private ReadOnly CustomFormatter As Func(Of Double, String) =
        Function(Position)
            Return If(_Is_Vitesse, Position.ToString("0.00"), Position.ToString("0"))
        End Function

    Private StockClipCurseur As Rectangle
    Friend WriteOnly Property Trace As TRK
        Set(value As TRK)
            _Trace = value
        End Set
    End Property
    ''' <summary> true : graphique Vitesse / Distance 
    ''' false : graphique Altitude / Distance </summary>
    Friend WriteOnly Property Is_Vitesse As Boolean
        Set(value As Boolean)
            _Is_Vitesse = value
        End Set
    End Property
    Friend ReadOnly Property IndexPoint As Integer
        Get
            Return Point.Index
        End Get
    End Property

    Private Sub AfficheGraphiqueTrace_Load(sender As Object, e As EventArgs)
        StockClipCurseur = Windows.Forms.Cursor.Clip
        'centre le formulaire sur le formulaire parent
        ClientSize = New Size(CInt(VisueRectangle.Width * 0.8), CInt(VisueRectangle.Height * 0.8))
        With Owner
            Location = New Point(.Location.X + (.Width - Width) \ 2, .Location.Y + (.Height - Height) \ 2)
        End With
        Text = $"Trace {_Trace.Nom}"

        InitialaliserSeries()

        Plt = Plot1.Plot
        Plt.Style(figureBackground:=Color.FromArgb(247, 247, 247), dataBackground:=Color.FromArgb(240, 240, 240))
        Plt.Grid(enable:=False)
        Plt.Title(If(_Is_Vitesse, "Vitesse en fonction de la distance", "Altitude en fonction de la distance"))
        Plt.XLabel("Distance en Km")
        Plt.YLabel(If(_Is_Vitesse, "Vitesse en Km/h", "Altitude en Mètres"))

        PltPoint = Plt.AddScatterPoints(Dists, If(_Is_Vitesse, Vits, Alts))
        PltPoint.Color = Color.FromArgb(247, 247, 247)
        PltPoint.MarkerColor = Plt.GetNextColor(1)
        PltPoint.MarkerLineWidth = 0
        PltPoint.MarkerSize = 0
        ' Add a red circle we can move around later as a highlighted point indicator
        PltPointLight = Plt.AddPoint(0, 0)
        PltPointLight.Color = Plt.GetNextColor(1)
        PltPointLight.MarkerSize = 10
        PltPointLight.MarkerShape = MarkerShape.openCircle
        PltPointLight.IsVisible = False

        CrossHair = Plt.AddCrosshair(0, 0)
        CrossHair.HorizontalLine.PositionFormatter = CustomFormatter

        Dim Xsr = Tools.Pad(Dists, cloneEdges:=True)
        Dim Ysr = Tools.Pad(If(_Is_Vitesse, Vits, Alts))

        EpaisseurLinePltPoly = 1
        PltPoly = Plt.AddPolygon(Xsr, Ysr, Color.FromArgb(40, 225, 175, 125), lineWidth:=EpaisseurLinePltPoly)

        PltTooltip = Plt.AddTooltip("", 0, 0)
        PltTooltip.IsVisible = False
        PltTooltip.ArrowSize = 35

        Plot1.Refresh()

        Point.Index = -1
        'et limite les déplacements de la souris au formulaire
        Windows.Forms.Cursor.Clip = New Rectangle(Location, Size)
    End Sub

    Private Sub AfficheGraphiqueTrace_FormClosed(sender As Object, e As FormClosedEventArgs)
        Windows.Forms.Cursor.Clip = StockClipCurseur
    End Sub
    Private Sub AfficheGraphiqueTrace_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Escape Then
            Point.Index = -1
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub
    ''' <summary> fait ressortir les données associées aux points et aux segments d'une trace </summary>
    Private Sub InitialaliserSeries()
        Dim DureeTot, LongueurTot, Cpt As Integer
        LongueurTot = 0
        Dim NbSegments = _Trace.NbSegments
        Segments = _Trace.CalculerStatSegmentsTrace(1, NbSegments)
        ReDim Dists(NbSegments)
        ReDim Vits(NbSegments)
        ReDim Alts(NbSegments)
        ReDim Durees(NbSegments)
        Dists(0) = 0
        Vits(0) = 0
        Durees(0) = 0
        Alts(0) = _Trace.Altitude(0)
        For Each Segment In Segments
            Cpt += 1
            LongueurTot += Segment.Longueur
            DureeTot += Segment.Duree
            Dists(Cpt) = LongueurTot / 1000
            Vits(Cpt) = Segment.Vitesse * 3.6
            Alts(Cpt) = _Trace.Altitude(Cpt)
            Durees(Cpt) = DureeTot
        Next
        UnitParSegments = Dists(Cpt) / NbSegments
    End Sub
    Private Sub DessinnerToolTip()
        TrouverFlagIndex(DeltaRecherche)
        If FlagIndex Then
            PltPointLight.X = Point.x
            PltPointLight.Y = Point.y
            PltPointLight.IsVisible = True
            PltTooltip.X = Point.x
            PltTooltip.Y = Point.y
            Dim Index = Point.Index
            Dim PointStr As String = $"Distance: {Dists(Index):0.00 km}, Altitude: {Alts(Index):0} m"
            Dim PointStrHoraire As String = If(_Trace.IsHoraire, $", Temps: {New TimeSpan(0, 0, CInt(Durees(Index))):c}", "")
            PltTooltip.Label = PointStr & PointStrHoraire
            If Index > 0 Then
                Dim NumSeg As Integer = Index - 1
                Dim SegmentStr As String = $"Longueur: {Segments(NumSeg).Longueur} m, Denivelé: {Segments(NumSeg).Denivele} m, " & CrLf &
                                           $"Pente: {Segments(NumSeg).Pente:P}, Cap: {Segments(NumSeg).Cap:N2}°"
                Dim SegmentStrHoraire As String = If(_Trace.IsHoraire, CrLf & $"Durée: {Segments(NumSeg).Duree} s, Vitesse : {Vits(Index):0.00 km/h}", "")
                PltTooltip.Label &= CrLf & SegmentStr & SegmentStrHoraire
            End If
            PltTooltip.IsVisible = True
        Else
            CrossHair.X = Coordinates.x
            CrossHair.Y = Coordinates.y
            PltPointLight.IsVisible = False
            PltTooltip.IsVisible = False
        End If
    End Sub

    Private Sub TrouverFlagIndex(Optional Espsilon As Double = 0.25D)
        Coordinates = Plot1.GetMouseCoordinates()
        FlagIndex = False
        If FlagRatio Then
            Point = PltPoint.GetPointNearest(Coordinates.x, Coordinates.y, XyRatio)
            FlagIndex = Math.Abs(Point.x - Coordinates.x) < Espsilon AndAlso Math.Abs(Point.y - Coordinates.y) < Espsilon
        End If
        If Not FlagIndex Then Point.Index = -1
    End Sub

    Private ReadOnly Property XyRatio As Double
        Get
            Return Plt.XAxis.Dims.PxPerUnit / Plt.YAxis.Dims.PxPerUnit
        End Get
    End Property
    Private Sub Plot1_DoubleClicked(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            PltPointLight.IsVisible = False
            PltTooltip.IsVisible = False
            If Point.Index > -1 Then
                DialogResult = DialogResult.OK
                Close()
            End If
        End If
    End Sub
    Private Sub Plot1_AxesChanged(sender As Object, e As EventArgs)
        Dim RatioDistance = UnitParSegments * Plt.XAxis.Dims.PxPerUnit
        DeltaRecherche = 0.1 * RatioDistance
        FlagRatio = RatioDistance > 3.2
        If FlagRatio Then
            CrossHair.IsVisible = False
            PltPoint.LineWidth = 1
            PltPoint.MarkerSize = 8
            PltPoly.LineWidth = 0
        Else
            CrossHair.IsVisible = True
            PltPoint.MarkerLineWidth = 0
            PltPoint.MarkerSize = 0
            PltPoly.LineWidth = EpaisseurLinePltPoly
        End If
        DessinnerToolTip()
        Plot1.Refresh(lowQuality:=False, skipIfCurrentlyRendering:=True)
    End Sub
    Private Sub Plot1_MouseEnter(sender As Object, e As EventArgs) 'Handles Plot1.MouseEnter
        CrossHair.IsVisible = Not FlagRatio
    End Sub
    Private Sub Plot1_MouseMove(sender As Object, e As MouseEventArgs) ' Handles Plot1.MouseMove
        DessinnerToolTip()
        Plot1.Refresh(lowQuality:=False, skipIfCurrentlyRendering:=True)
    End Sub
    Private Sub Plot1_MouseLeave(sender As Object, e As EventArgs) 'Handles Plot1.MouseLeave
        CrossHair.IsVisible = False
        Plot1.Refresh()
    End Sub

#Region "ScottPlot WinForms"
    '''<summary> ScottPlot Interactive Plotting Library for .NET : https://scottplot.net/
    '''Code traduit de C# à VB et modifié du controle WindowsForms de la librairie ScottPlot  
    '''https://github.com/ScottPlot/ScottPlot/tree/main/src/ScottPlot4/ScottPlot.WinForms  
    '''pour correspondre aux besoins spécifiques de FCGP Capturer. 
    '''Les modifications portent principalement sur le LeftdoubleClick et sur le RightClick menu par défaut</summary>
    Private Class ControlScottPlot
        Inherits UserControl
        ''' <summary> This is the plot displayed by the user control.
        ''' After modifying it you may need to call Refresh() to request the plot be redrawn on the screen. </summary>
        Friend ReadOnly Property Plot As Plot
            Get
                Return Backend.Plot
            End Get
        End Property

        ''' <summary> This object can be used to modify advanced behaior and customization of this user control. </summary>
        Friend ReadOnly Configuration As Control.Configuration

        ''' <summary> This event is invoked any time the axis limits are modified. </summary>
        Friend Event AxesChanged As EventHandler
        ''' <summary> This event is invoked any time the users doubleclick with the mouse on the plot. </summary>
        Friend Event DoubleClicked As MouseEventHandler
        ''' <summary> This event is invoked any time the plot is right-clicked.
        ''' By default it contains DefaultRightClickEvent(), but you can remove this and add your own method. </summary>
        Friend Event RightClicked As EventHandler

        ''' <summary> This event is invoked after the mouse moves while dragging a draggable plottable.
        ''' The object passed is the plottable being dragged. </summary>
        Friend Event PlottableDragged As EventHandler

        ''' <summary> This event is invoked right after a draggable plottable was dropped.
        ''' The object passed is the plottable that was just dropped. </summary>
        Friend Event PlottableDropped As EventHandler
        Private ReadOnly Backend As Control.ControlBackEnd
        Private ReadOnly Cursors As Dictionary(Of Cursor, Windows.Forms.Cursor)
        Private ReadOnly IsDesignerMode As Boolean = Equals(Process.GetCurrentProcess().ProcessName, "devenv")

        Friend Sub New()
            Backend = New Control.ControlBackEnd(1, 1, "FormsPlot")
            Backend.Resize(Width, Height, useDelayedRendering:=False)
            AddHandler Backend.BitmapChanged, New EventHandler(AddressOf OnBitmapChanged)
            AddHandler Backend.BitmapUpdated, New EventHandler(AddressOf OnBitmapUpdated)
            AddHandler Backend.CursorChanged, New EventHandler(AddressOf OnCursorChanged)
            AddHandler Backend.RightClicked, New EventHandler(AddressOf OnRightClicked)
            AddHandler Backend.AxesChanged, New EventHandler(AddressOf OnAxesChanged)
            AddHandler Backend.PlottableDragged, New EventHandler(AddressOf OnPlottableDragged)
            AddHandler Backend.PlottableDropped, New EventHandler(AddressOf OnPlottableDropped)
            Configuration = Backend.Configuration

            If IsDesignerMode Then
                Try
                    Configuration.WarnIfRenderNotCalledManually = False
                    Plot.Title($"ScottPlot {Plot.Version}")
                    Plot.Render()
                Catch e As Exception
                    InitializeComponent()
                    pictureBox1.Visible = False
                    rtbErrorMessage.Visible = True
                    rtbErrorMessage.Dock = DockStyle.Fill
                    rtbErrorMessage.Text = "ERROR: ScottPlot failed to render in design mode." & CrLf &
                        "This may be due to incompatible Common versions or a 32-bit/64-bit mismatch." & CrLf &
                        "Although rendering failed at design time, it may still function normally at runtime." & CrLf &
                        $"Exception details:{e}"
                    Return
                End Try
            End If

            Cursors = New Dictionary(Of Cursor, Windows.Forms.Cursor)() From {
                {ScottPlot.Cursor.Arrow, Windows.Forms.Cursors.Arrow},
                {ScottPlot.Cursor.WE, Windows.Forms.Cursors.SizeWE},
                {ScottPlot.Cursor.NS, Windows.Forms.Cursors.SizeNS},
                {ScottPlot.Cursor.All, Windows.Forms.Cursors.SizeAll},
                {ScottPlot.Cursor.Crosshair, Windows.Forms.Cursors.Cross},
                {ScottPlot.Cursor.Hand, Windows.Forms.Cursors.Hand},
                {ScottPlot.Cursor.Question, Windows.Forms.Cursors.Help}
            }
            InitializeComponent()
            rtbErrorMessage.Visible = False
            pictureBox1.BackColor = Color.Transparent
            Plot.Style(figureBackground:=Color.Transparent)
            AddHandler pictureBox1.MouseWheel, AddressOf PictureBox1_MouseWheel
            AddHandler RightClicked, AddressOf DefaultRightClickEvent
            Backend.StartProcessingEvents()
        End Sub

        ''' <summary> Return the mouse position on the plot (in coordinate space) for the latest X and Y coordinates </summary>
        Friend Function GetMouseCoordinates(Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As (x As Double, y As Double)
            Return Backend.GetMouseCoordinates(xAxisIndex, yAxisIndex)
        End Function

        ''' <summary> Reset this control by replacing the current plot with an existing plot </summary>
        Friend Sub Reset(ByVal newPlot As Plot)
            Backend.Reset(Width, Height, newPlot)
        End Sub

        ''' <summary> Re-render the plot and update the image displayed by this control. </summary>
        ''' <param name="lowQuality">disable anti-aliasing to produce faster (but lower quality) plots </param>
        ''' <param name="skipIfCurrentlyRendering"></param>
        Friend Shadows Sub Refresh(Optional lowQuality As Boolean = False, Optional skipIfCurrentlyRendering As Boolean = False)
            Application.DoEvents()
            Backend.WasManuallyRendered = True
            Backend.Render(lowQuality, skipIfCurrentlyRendering)
        End Sub

        Private Sub FormsPlot_Load(sender As Object, e As EventArgs)
            OnSizeChanged(Nothing, Nothing)
        End Sub

        Private Sub OnBitmapUpdated(sender As Object, e As EventArgs)
            Application.DoEvents()
            pictureBox1.Invalidate()
        End Sub

        Private Sub OnBitmapChanged(sender As Object, e As EventArgs)
            pictureBox1.Image = Backend.GetLatestBitmap()
        End Sub

        Private Shadows Sub OnCursorChanged(sender As Object, e As EventArgs)
            Cursor = Cursors(Backend.Cursor)
        End Sub

        Private Shadows Sub OnSizeChanged(sender As Object, e As EventArgs)
            Backend.Resize(Width, Height, useDelayedRendering:=True)
        End Sub

        Private Sub OnAxesChanged(sender As Object, e As EventArgs)
            RaiseEvent AxesChanged(Me, e)
        End Sub

        Private Sub OnRightClicked(sender As Object, e As EventArgs)
            RaiseEvent RightClicked(Me, e)
        End Sub

        Private Sub OnPlottableDragged(sender As Object, e As EventArgs)
            RaiseEvent PlottableDragged(sender, e)
        End Sub

        Private Sub OnPlottableDropped(sender As Object, e As EventArgs)
            RaiseEvent PlottableDropped(sender, e)
        End Sub

        Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs)
            Backend.MouseDown(GetInputState(e))
            OnMouseDown(e)
        End Sub

        Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs)
            Backend.MouseUp(GetInputState(e))
            MyBase.OnMouseUp(e)
        End Sub

        Private Sub OnDoubleClicked(sender As Object, e As EventArgs)
            RaiseEvent DoubleClicked(Me, CType(e, MouseEventArgs))
        End Sub

        Private Sub PictureBox1_MouseWheel(sender As Object, e As MouseEventArgs)
            Backend.MouseWheel(GetInputState(e))
            OnMouseWheel(e)
        End Sub

        Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs)
            Backend.MouseMove(GetInputState(e))
            MyBase.OnMouseMove(e)
        End Sub

        Private Sub PictureBox1_MouseEnter(sender As Object, e As EventArgs)
            MyBase.OnMouseEnter(e)
        End Sub

        Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs)
            MyBase.OnMouseLeave(e)
        End Sub

        Private Shared Function GetInputState(e As MouseEventArgs) As Control.InputState
            Return New Control.InputState With {
             .X = e.X,
             .Y = e.Y,
             .LeftWasJustPressed = e.Button = MouseButtons.Left,
             .RightWasJustPressed = e.Button = MouseButtons.Right,
             .MiddleWasJustPressed = e.Button = MouseButtons.Middle,
             .ShiftDown = ModifierKeys.HasFlag(Keys.Shift),
             .CtrlDown = ModifierKeys.HasFlag(Keys.Control),
             .AltDown = ModifierKeys.HasFlag(Keys.Alt),
             .WheelScrolledUp = e.Delta > 0,
             .WheelScrolledDown = e.Delta < 0
         }
        End Function

        Public Sub DefaultRightClickEvent(sender As Object, e As EventArgs)
            DefaultRightClickMenu.Show(Windows.Forms.Cursor.Position)
        End Sub

        Private Sub RightClickMenu_Copy_Click(sender As Object, e As EventArgs)
            Clipboard.SetImage(Plot.Render())
        End Sub

        Private Sub RightClickMenu_Help_Click(sender As Object, e As EventArgs)
            Call New ControlScottPlotHelp().Show(Me)
        End Sub

        Private Sub RightClickMenu_AutoAxis_Click(sender As Object, e As EventArgs)
            Plot.AxisAuto()
            Refresh()
        End Sub

        Private Sub RightClickMenu_SaveImage_Click(sender As Object, e As EventArgs)
            Dim sfd = New SaveFileDialog With {
                .FileName = "ScottPlot.png",
                .Filter = "PNG Files (*.png)|*.png;*.png" & "|JPG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg" & "|BMP Files (*.bmp)|*.bmp;*.bmp" & "|All files (*.*)|*.*"
            }
            If sfd.ShowDialog() = DialogResult.OK Then Plot.SaveFig(sfd.FileName)
        End Sub
#Region "FormHelp"
#Region "Hors concepteur"
        Private Class ControlScottPlotHelp
            Inherits Form

            Friend Sub New()
                InitializeComponent()
                Dim resources As New ComponentResourceManager(GetType(AfficheGraphiqueTrace))
                lblVersion.Text = $"ScottPlot {Plot.Version}"
                lblMessage.Text = resources.GetString("lblMessage.Text")
            End Sub
        End Class
#End Region
#Region "concepteur"
        Partial Class ControlScottPlotHelp
            ''' <summary> Required designer variable. </summary>
            Private ReadOnly components As IContainer = Nothing

            ''' <summary> Clean up any resources being used. </summary>
            ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            Protected Overrides Sub Dispose(ByVal disposing As Boolean)
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If

                MyBase.Dispose(disposing)
            End Sub
            ''' <summary> Required method for Designer support - do not modify
            ''' the contents of this method with the code editor. </summary>
            Private Sub InitializeComponent()
                lblTitle = New Label()
                lblVersion = New Label()
                lblMessage = New Label()
                SuspendLayout()
                '
                'lblTitle
                '
                lblTitle.AutoSize = True
                lblTitle.Font = New Font("Segoe UI", 18.0!, FontStyle.Regular, GraphicsUnit.Point)
                lblTitle.Location = New Point(14, 10)
                lblTitle.Margin = New Padding(4, 0, 4, 0)
                lblTitle.Name = "lblTitle"
                lblTitle.Size = New Size(265, 32)
                lblTitle.TabIndex = 0
                lblTitle.Text = "Contrôles avec la souris"
                '
                'lblVersion
                '
                lblVersion.AutoSize = True
                lblVersion.Font = New Font("Segoe UI", 9.75!, FontStyle.Regular, GraphicsUnit.Point)
                lblVersion.ForeColor = SystemColors.ControlDark
                lblVersion.Location = New Point(16, 47)
                lblVersion.Margin = New Padding(4, 0, 4, 0)
                lblVersion.Name = "lblVersion"
                lblVersion.Size = New Size(0, 17)
                lblVersion.TabIndex = 1
                '
                'lblMessage
                '
                lblMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
                lblMessage.AutoSize = True
                lblMessage.Font = New Font("Segoe UI", 11.25!, FontStyle.Regular, GraphicsUnit.Point)
                lblMessage.Location = New Point(14, 83)
                lblMessage.Margin = New Padding(4, 0, 4, 0)
                lblMessage.Name = "lblMessage"
                lblMessage.Size = New Size(293, 340)
                lblMessage.TabIndex = 2
                '
                'FormHelp
                '
                AutoScaleDimensions = New SizeF(7.0!, 15.0!)
                AutoScaleMode = AutoScaleMode.Font
                ClientSize = New Size(364, 500)
                Controls.Add(lblMessage)
                Controls.Add(lblVersion)
                Controls.Add(lblTitle)
                FormBorderStyle = FormBorderStyle.FixedSingle
                Margin = New Padding(4, 3, 4, 3)
                Name = "FormHelp"
                Text = "Help"
                ResumeLayout(False)
                PerformLayout()
            End Sub
#End Region
            Private lblTitle As Windows.Forms.Label
            Private lblVersion As Windows.Forms.Label
            Private lblMessage As Windows.Forms.Label
        End Class
#End Region
    End Class
    Partial Private Class ControlScottPlot
        ''' <summary>  Required designer variable. </summary>
        Private components As IContainer = Nothing

        ''' <summary> Clean up any resources being used. </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "Component Designer generated code"
        ''' <summary> Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor. </summary>
        Private Sub InitializeComponent()
            components = New Container()
            pictureBox1 = New PictureBox()
            DefaultRightClickMenu = New ContextMenuStrip(components)
            copyMenuItem = New ToolStripMenuItem()
            saveImageMenuItem = New ToolStripMenuItem()
            toolStripSeparator1 = New ToolStripSeparator()
            autoAxisMenuItem = New ToolStripMenuItem()
            toolStripSeparator2 = New ToolStripSeparator()
            helpMenuItem = New ToolStripMenuItem()
            rtbErrorMessage = New RichTextBox()
            CType(pictureBox1, ISupportInitialize).BeginInit()
            DefaultRightClickMenu.SuspendLayout()
            SuspendLayout()
            ' 
            ' pictureBox1
            ' 
            pictureBox1.BackColor = Color.Navy
            pictureBox1.Dock = DockStyle.Fill
            pictureBox1.Location = New Point(0, 0)
            pictureBox1.Name = "pictureBox1"
            pictureBox1.Size = New Size(400, 300)
            pictureBox1.TabIndex = 0
            pictureBox1.TabStop = False
            AddHandler pictureBox1.DoubleClick, New EventHandler(AddressOf OnDoubleClicked)
            AddHandler pictureBox1.MouseDown, New MouseEventHandler(AddressOf PictureBox1_MouseDown)
            AddHandler pictureBox1.MouseEnter, New EventHandler(AddressOf PictureBox1_MouseEnter)
            AddHandler pictureBox1.MouseLeave, New EventHandler(AddressOf PictureBox1_MouseLeave)
            AddHandler pictureBox1.MouseMove, New MouseEventHandler(AddressOf PictureBox1_MouseMove)
            AddHandler pictureBox1.MouseUp, New MouseEventHandler(AddressOf PictureBox1_MouseUp)
            ' 
            ' DefaultRightClickMenu
            ' 
            DefaultRightClickMenu.ImageScalingSize = New Size(20, 20)
            DefaultRightClickMenu.Items.AddRange(New ToolStripItem() {
               copyMenuItem,
               saveImageMenuItem,
               toolStripSeparator1,
               autoAxisMenuItem,
               toolStripSeparator2,
               helpMenuItem})
            DefaultRightClickMenu.Name = "contextMenuStrip1"
            DefaultRightClickMenu.Size = New Size(191, 132)
            DefaultRightClickMenu.ShowImageMargin = False
            ' 
            ' copyMenuItem
            ' 
            copyMenuItem.Name = "copyMenuItem"
            copyMenuItem.Size = New Size(190, 22)
            copyMenuItem.Text = "Copie Image"
            AddHandler copyMenuItem.Click, New EventHandler(AddressOf RightClickMenu_Copy_Click)
            ' 
            ' saveImageMenuItem
            ' 
            saveImageMenuItem.Name = "saveImageMenuItem"
            saveImageMenuItem.Size = New Size(190, 22)
            saveImageMenuItem.Text = "Sauve Image ..."
            AddHandler saveImageMenuItem.Click, New EventHandler(AddressOf RightClickMenu_SaveImage_Click)
            ' 
            ' toolStripSeparator1
            ' 
            toolStripSeparator1.Name = "toolStripSeparator1"
            toolStripSeparator1.Size = New Size(187, 6)
            ' 
            ' autoAxisMenuItem
            ' 
            autoAxisMenuItem.Name = "autoAxisMenuItem"
            autoAxisMenuItem.Size = New Size(190, 22)
            autoAxisMenuItem.Text = "Recentre le tracé"
            AddHandler autoAxisMenuItem.Click, New EventHandler(AddressOf RightClickMenu_AutoAxis_Click)
            ' 
            ' toolStripSeparator2
            ' 
            toolStripSeparator2.Name = "toolStripSeparator2"
            toolStripSeparator2.Size = New Size(187, 6)
            ' 
            ' helpMenuItem
            ' 
            helpMenuItem.Name = "helpMenuItem"
            helpMenuItem.Size = New Size(190, 22)
            helpMenuItem.Text = "Aide ..."
            AddHandler helpMenuItem.Click, New EventHandler(AddressOf RightClickMenu_Help_Click)
            ' 
            ' rtbErrorMessage
            ' 
            rtbErrorMessage.BackColor = Color.Maroon
            rtbErrorMessage.Font = New Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0)
            rtbErrorMessage.ForeColor = Color.White
            rtbErrorMessage.Location = New Point(21, 24)
            rtbErrorMessage.Name = "rtbErrorMessage"
            rtbErrorMessage.Size = New Size(186, 84)
            rtbErrorMessage.TabIndex = 1
            rtbErrorMessage.Text = "error message"
            rtbErrorMessage.Visible = False
            ' 
            ' FormsPlot
            ' 
            AutoScaleDimensions = New SizeF(6.0F, 13.0F)
            AutoScaleMode = AutoScaleMode.Font
            Controls.Add(rtbErrorMessage)
            Controls.Add(pictureBox1)
            Name = "FormsPlot"
            Size = New Size(400, 300)
            AddHandler Load, New EventHandler(AddressOf FormsPlot_Load)
            AddHandler SizeChanged, New EventHandler(AddressOf OnSizeChanged)
            CType(pictureBox1, ISupportInitialize).EndInit()
            DefaultRightClickMenu.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
#End Region

        Private pictureBox1 As PictureBox
        Private DefaultRightClickMenu As ContextMenuStrip
        Private autoAxisMenuItem As ToolStripMenuItem
        Private saveImageMenuItem As ToolStripMenuItem
        Private helpMenuItem As ToolStripMenuItem
        Private copyMenuItem As ToolStripMenuItem
        Private toolStripSeparator1 As ToolStripSeparator
        Private toolStripSeparator2 As ToolStripSeparator
        Private rtbErrorMessage As RichTextBox
    End Class
#End Region
End Class