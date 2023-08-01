''' <summary> permet la saisie d'un PointD au format Grille </summary>
Friend Class SaisieGrille
    Private LimiteSite As RectangleD
    Private ReadOnly SaisieXY As TypeSaisie = TypeSaisie.EntierNegatif
    Private StockClipCurseur As Rectangle
    Private Titre As String
    ''' <summary> initialisation de la location et des différents champs de saisie </summary>
    Private Sub SaisieGrille_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        'Attend un point dd en DD
        Dim P As PointD = CType(Tag, PointD)
        If Not P.IsEmpty Then
            CoordX.Text = P.X.ToString("#0")
            CoordX.Select(CoordX.Text.Length, 0)
            CoordY.Text = P.Y.ToString("#0")
            CoordY.Select(CoordY.Text.Length, 0)
        End If
        LimiteSite = Serveur.Limites
        Cursor.Clip = New Rectangle(Location, Size)
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
    Private Sub SaisieGrille_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            If String.IsNullOrEmpty(CoordX.Text) Then
                e.Cancel = True
                Erreur("X", -20000000, 20000000)
                CoordX.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(CoordY.Text) Then
                e.Cancel = True
                Erreur("Y", -10000000, 10000000)
                CoordY.Focus()
                Exit Sub
            End If
            Dim Result As New PointD(CInt(CoordX.Text), CInt(CoordY.Text))
            'Logic point saisi en dehors des limites
            If FlagLimitesSite AndAlso Not LimiteSite.CoordonneesContains(Result) Then
                e.Cancel = True
                MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointXYtoChaine(New PointProjection(LimiteSite.Pt0))}{CrLf}et {CrLf}" &
                                     ConvertPointXYtoChaine(New PointProjection(LimiteSite.Pt2))
                TitreInformation = "Point Hors limites"
                AfficherInformation()
                Exit Sub
            End If
            'renvoie un point double en mètres
            Tag = Result
        End If
        Cursor.Clip = StockClipCurseur
        TitreInformation = Titre
    End Sub
End Class