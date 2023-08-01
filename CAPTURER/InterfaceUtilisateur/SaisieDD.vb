''' <summary> permet la saisie d'un PointD au format DD </summary>
Friend Class SaisieDD
    Private LimiteSite As RectangleD
    Private ReadOnly SaisieLatLon As TypeSaisie = TypeSaisie.DecimalNegatif
    Private StockClipCurseur As Rectangle
    Private Titre As String
    ''' <summary> initialisation de la location et des différents champs de saisie </summary>
    Private Sub SaisieDD_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        'Attend un pointD en DD
        Dim Pt As PointD = CType(Tag, PointD)
        If Not Pt.IsEmpty Then
            CoordLon.Text = DblToStr(Pt.X, "N8")
            CoordLon.Select(CoordLon.Text.Length, 0)
            CoordLat.Text = DblToStr(Pt.Y, "N8")
            CoordLat.Select(CoordLat.Text.Length, 0)
        End If
        LimiteSite = RegionGrilleToRegionDD(Serveur.Limites, Serveur.SiteCarto)
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> filtrage des touches admises </summary>
    Private Sub CoordLatLon_KeyDown(sender As Object, e As KeyEventArgs)
        e.SuppressKeyPress = SuppressionTouche(SaisieLatLon, e.KeyCode)
    End Sub
    ''' <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
    Private Sub CoordLatLon_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        e.Handled = SuppressionCaractere(SaisieLatLon, C, e.KeyChar)
    End Sub
    Private Sub CoordLatLon_Leave(sender As Object, e As EventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        C.Text = FormaterDecimal(C.Text)
    End Sub
    ''' <summary> affiche un message d'erreur </summary>
    Private Shared Sub Erreur(Champ As String, Min As Double, Max As Double)
        MessageInformation = $"Le champ {Champ} doit être compris" & CrLf & $"entre {Min:0.00} et {Max:0.00}"
        TitreInformation = "Erreur de saisie"
        AfficherInformation()
    End Sub
    ''' <summary> convertit une chaine de caractères en double. Si null en double incompatible seconde </summary>
    Private Shared Function TextToDbl(Text As String) As Double
        If String.IsNullOrEmpty(Text) OrElse Text = "." OrElse Text = "-." OrElse Text = "-" Then
            Return 500.0
        Else
            Return StrToDbl(Text)
        End If
    End Function
    ''' <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
    Private Sub SaisieDD_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            Dim VDX As Double = TextToDbl(CoordLon.Text)
            If VDX > 180 OrElse VDX < -180 Then
                Erreur("Longitude", -180, 180)
                CoordLon.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim VDY As Double = TextToDbl(CoordLat.Text)
            If VDY > 90 OrElse VDY < -90 Then
                Erreur("Latitude", -90, 90)
                CoordLat.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim Result As New PointD(VDX, VDY)
            'Logic point saisi en dehors des limites
            If FlagLimitesSite AndAlso Not LimiteSite.CoordonneesContains(Result) Then
                e.Cancel = True
                MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointDDtoChaine(LimiteSite.Pt0, "N5")}{CrLf}et {CrLf}{ConvertPointDDtoChaine(LimiteSite.Pt2, "N5")}"
                TitreInformation = "Point Hors limites"
                AfficherInformation()
                Exit Sub
            End If
            'renvoie un point double en DD
            Tag = Result
        End If
        Cursor.Clip = StockClipCurseur
        TitreInformation = Titre
    End Sub
End Class