''' <summary> permet la saisie d'un pointD au format tuile </summary>
Friend Class SaisieTuile
    Private LimiteSite, StockClipCurseur As Rectangle
    Private Titre As String
    Private ReadOnly SaisieXY As TypeSaisie = TypeSaisie.EntierPositif
    ''' <summary> initialisation de la location et des différents champs de saisie </summary>
    Private Sub SaisieTuile_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        'Attend un point dd en PointGrille
        Dim PointCoordonnees As PointD = CType(Tag, PointD)
        If Not PointCoordonnees.IsEmpty Then
            Dim PointPixels As Point = PointGrilleToPointPixel(PointCoordonnees, Serveur.SiteCarto, Affichage.Echelle)
            Dim PG As New PointG(CapturerSettings.INDICE_ECHELLE, PointPixels)
            Col.Text = PG.IndexTuile.X.ToString("#0")
            Col.Select(Col.Text.Length, 0)
            DecalX.Text = PG.Offset.Width.ToString("#0")
            DecalX.Select(DecalX.Text.Length, 0)
            Row.Text = PG.IndexTuile.Y.ToString("#0")
            Row.Select(Row.Text.Length, 0)
            DecalY.Text = PG.Offset.Height.ToString("#0")
            DecalY.Select(DecalY.Text.Length, 0)
        End If
        LimiteSite = RegionGrilleToRegionPixels(Serveur.Limites, Serveur.SiteCarto, Affichage.Echelle)
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> filtrage des touches admises </summary>
    Private Sub Coord_KeyDown(sender As Object, e As KeyEventArgs)
        e.SuppressKeyPress = SuppressionTouche(SaisieXY, e.KeyCode)
    End Sub
    ''' <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
    Private Sub Coord_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        e.Handled = SuppressionCaractere(SaisieXY, C, e.KeyChar)
    End Sub
    ''' <summary> affiche un message d'erreur </summary>
    Private Shared Sub Erreur(Champ As String)
        MessageInformation = $"Le champ {Champ} doit être égal ou supérieur à 0"
        TitreInformation = "Erreur de saisie"
        AfficherInformation()
    End Sub
    ''' <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
    Private Sub SaisieTuile_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            'renvoie un point double en DD
            If String.IsNullOrEmpty(Col.Text) Then
                e.Cancel = True
                Erreur("Index Tuile X")
                Col.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(Row.Text) Then
                e.Cancel = True
                Erreur("Index Tuile Y")
                Row.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(DecalX.Text) Then
                e.Cancel = True
                Erreur("Decalage Pixels X")
                DecalX.Focus()
                Exit Sub
            End If
            If String.IsNullOrEmpty(DecalY.Text) Then
                e.Cancel = True
                Erreur("Decalage Pixels Y")
                DecalY.Focus()
                Exit Sub
            End If
            Dim Result As New PointG(0, CInt(Col.Text), CInt(Row.Text), CInt(DecalX.Text), CInt(DecalY.Text))
            'Logic point saisi en dehors des limites
            If FlagLimitesSite AndAlso Not LimiteSite.Contains(Result.Location) Then
                e.Cancel = True
                MessageInformation = $"Le point doit être compris entre :{CrLf}{New PointG(CapturerSettings.INDICE_ECHELLE, LimiteSite.Location).ToString}{CrLf}et {CrLf}" &
                                       New PointG(CapturerSettings.INDICE_ECHELLE, Point.Add(LimiteSite.Location, New Size(LimiteSite.Width, LimiteSite.Height))).ToString
                TitreInformation = "Point Hors limites"
                AfficherInformation()
                Exit Sub
            End If
            'renvoie un point double en mètres
            Tag = PointPixelToPointGrille(Result.Location, Serveur.SiteCarto, Affichage.Echelle)
        End If
        Cursor.Clip = StockClipCurseur
        TitreInformation = Titre
    End Sub
End Class