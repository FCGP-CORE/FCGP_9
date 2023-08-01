''' <summary> Saisie d'un PointD en format UTM </summary>
Friend Class SaisieUTM
    Private ListeZones() As Integer, LimiteSite As RectangleD
    Private ReadOnly SaisieXY As TypeSaisie = TypeSaisie.EntierPositif
    Private StockClipCurseur As Rectangle
    Private Titre As String
    ''' <summary> initialisation de la location et des différents champs de saisie </summary>
    Private Sub SaisieUTM_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        'Attend un pointD en DD
        Dim P As PointD = CType(Tag, PointD)
        ListeZones = New Integer() {20, 21, 22, 30, 31, 32, 38, 40}
        If Not P.IsEmpty Then
            'on tranforme en UTM
            Dim UTM = ConvertWGS84ToProjection(P, Datums.UTM_WGS84)
            If UTM.Hemisphere = "N"c Then
                Hem.SelectedIndex = 0
            Else
                Hem.SelectedIndex = 1
            End If
            Zone.SelectedIndex = Array.IndexOf(ListeZones, UTM.Zone)
            CoordX.Text = UTM.X.ToString("#0")
            CoordX.Select(CoordX.Text.Length, 0)
            CoordY.Text = UTM.Y.ToString("#0")
            CoordY.Select(CoordY.Text.Length, 0)
        Else
            Hem.SelectedIndex = 0
            Zone.SelectedIndex = 4
        End If
        LimiteSite = RegionGrilleToRegionDD(Serveur.Limites, Serveur.SiteCarto)
        Cursor.Clip = New Rectangle(Location, New Size(Size.Width, Size.Height + 105))
    End Sub
    ''' <summary> filtrage des touches admises </summary>
    Private Sub CoordXY_KeyDown(sender As Object, e As KeyEventArgs)
        e.SuppressKeyPress = SuppressionTouche(SaisieXY, e.KeyCode)
    End Sub
    ''' <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
    Private Sub CoordXY_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        e.Handled = SuppressionCaractere(SaisieXY, C, e.KeyChar)
    End Sub
    ''' <summary> affiche un message d'erreur </summary>
    Private Shared Sub Erreur(Champ As String, Min As Integer, Max As Integer)
        MessageInformation = $"Le champ {Champ} doit être compris" & CrLf & $"entre {Min} et {Max}"
        TitreInformation = "Erreur de saisie"
        AfficherInformation()
    End Sub
    ''' <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
    Private Sub SaisieUTM_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            If String.IsNullOrEmpty(CoordX.Text) Then
                e.Cancel = True
                Erreur("X", 0, 1000000)
                CoordX.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(CoordY.Text) Then
                e.Cancel = True
                Erreur("Y", 0, 10000000)
                CoordY.Focus()
                Exit Sub
            End If
            Dim UTM = New PointProjection(New PointD(CInt(CoordX.Text), CInt(CoordY.Text)), ListeZones(Zone.SelectedIndex), CChar(Hem.SelectedItem))
            Dim Result = ConvertProjectionToWGS84(Datums.UTM_WGS84, UTM)
            'Logic point saisi en dehors des limites
            If FlagLimitesSite AndAlso Not LimiteSite.CoordonneesContains(Result) Then
                e.Cancel = True
                MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointDDtoUTM(LimiteSite.Pt0)}{CrLf}et {CrLf}{ConvertPointDDtoUTM(LimiteSite.Pt2)}"
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

    Private Sub HEM_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelHem.Text = Hem.SelectedItem.ToString
    End Sub

    Private Sub LabelHem_Click(sender As Object, e As EventArgs)
        Hem.DroppedDown = True
    End Sub

    Private Sub ZONE_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelZone.Text = Zone.SelectedItem.ToString
    End Sub

    Private Sub LabelZONE_Click(sender As Object, e As EventArgs)
        Zone.DroppedDown = True
    End Sub
End Class