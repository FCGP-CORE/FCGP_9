''' <summary> permet la saisie d'un PointD au format DMS </summary>
Friend Class SaisieDMS
    Private LimiteSite As RectangleD
    Private ReadOnly SaisieDM As TypeSaisie = TypeSaisie.EntierPositif, SaisieS As TypeSaisie = TypeSaisie.DecimalPositif
    Private StockClipCurseur As Rectangle
    Private Titre As String
    ''' <summary> initialisation de la location et des différents champs de saisie </summary>
    Private Sub SaisieDMS_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        'Attend un pointD en DD
        Dim P As PointD = CType(Tag, PointD)
        If P.IsEmpty Then
            Longitude.SelectedIndex = 0 '"E"
            Latitude.SelectedIndex = 0 '"N"
        Else
            Dim DMS As (Sens As Char, Deg As Integer, Min As Integer, Sec As Double, DMSTxt As String)
            DMS = ConvertDDtoDMS(P.X, TypesCoordsDD.Longitude, ".0", True)
            DegX.Text = DMS.Deg.ToString
            DegX.Select(DegX.Text.Length, 0)
            MinX.Text = DMS.Min.ToString
            MinX.Select(MinX.Text.Length, 0)
            SecX.Text = DblToStr(DMS.Sec, "N1")
            SecX.Select(SecX.Text.Length, 0)
            Longitude.SelectedIndex = If(DMS.Sens = "E"c, 0, 1)
            DMS = ConvertDDtoDMS(P.Y, TypesCoordsDD.Latitude, ".0", True)
            DegY.Text = DMS.Deg.ToString
            DegY.Select(DegY.Text.Length, 0)
            MinY.Text = DMS.Min.ToString
            MinY.Select(MinY.Text.Length, 0)
            SecY.Text = DblToStr(DMS.Sec, "N1")
            SecY.Select(SecY.Text.Length, 0)
            Latitude.SelectedIndex = If(DMS.Sens = "N"c, 0, 1)
        End If
        LimiteSite = RegionGrilleToRegionDD(Serveur.Limites, Serveur.SiteCarto)
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> filtrage des touches admises </summary>
    Private Sub Coord_KeyDown(sender As Object, e As KeyEventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        If C.Name(0) = "S"c Then
            e.SuppressKeyPress = SuppressionTouche(SaisieS, e.KeyCode)
        Else
            e.SuppressKeyPress = SuppressionTouche(SaisieDM, e.KeyCode)
        End If
    End Sub
    ''' <summary> filtrage du caractère . pour les champs concernant les secondes </summary>
    Private Sub Coord_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        If C.Name(0) = "S"c Then
            e.Handled = SuppressionCaractere(SaisieS, C, e.KeyChar)
        Else
            e.Handled = SuppressionCaractere(SaisieDM, C, e.KeyChar)
        End If
    End Sub
    Private Sub Coord_Leave(sender As Object, e As EventArgs)
        Dim C As TextBox = CType(sender, TextBox)
        C.Text = FormaterDecimal(C.Text)
    End Sub
    ''' <summary> convertit une chaine de caractères en integer. Si null en integer incompatible Lon/Lat </summary>
    Private Shared Function TextToShort(Text As String) As Short
        If String.IsNullOrEmpty(Text) Then
            Return 500
        Else
            Return CShort(Text)
        End If
    End Function
    ''' <summary> convertit une chaine de caractères en double. Si null en double incompatible seconde </summary>
    Private Shared Function TextToDbl(Text As String) As Double
        If String.IsNullOrEmpty(Text) OrElse Text = "." Then
            Return 500.0
        Else
            Return StrToDbl(Text)
        End If
    End Function
    ''' <summary> affiche un message d'erreur </summary>
    Private Shared Sub Erreur(Champ As String, Min As Double, Max As Double)
        MessageInformation = $"Le champ {Champ} doit être compris" & CrLf & $"entre {Min} et {Max}"
        TitreInformation = "Erreur de saisie"
        AfficherInformation()
    End Sub
    ''' <summary> validation des saisies avec message d'erreur en cas d'erreur de saisie </summary>
    Private Sub SaisieDMS_FormClosing(sender As Object, e As FormClosingEventArgs)
        If DialogResult = DialogResult.OK Then
            Dim VDX As Short = TextToShort(DegX.Text)
            If VDX > 180 Then
                Erreur("Longitude Degré", 0, 180)
                DegX.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim VMX As Short = TextToShort(MinX.Text)
            If VMX > 60 Then
                Erreur("Minute", 0, 60)
                MinX.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim VSX As Double = TextToDbl(SecX.Text)
            If VSX > 60.0 Then
                Erreur("Seconde", 0, 60)
                SecX.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim VDY As Short = TextToShort(DegY.Text)
            If VDY > 90 Then
                Erreur("Latitude Degré", 0, 90)
                DegY.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim VMY As Short = TextToShort(MinY.Text)
            If VMY > 60 Then
                Erreur("Minute", 0, 60)
                MinY.Focus()
                e.Cancel = True
                Exit Sub
            End If
            Dim VSY As Double = TextToDbl(SecY.Text)
            If VSY > 60.0 Then
                Erreur("Seconde", 0, 60)
                SecY.Focus()
                e.Cancel = True
                Exit Sub
            End If
            'renvoie un point double en DD
            Dim Result = New PointD(ConvertDMStoDD(Longitude.Text(0), VDX, VMX, VSX),
                                    ConvertDMStoDD(Latitude.Text(0), VDY, VMY, VSY))
            'Logic point saisi en dehors des limites
            If FlagLimitesSite AndAlso Not LimiteSite.CoordonneesContains(Result) Then
                e.Cancel = True
                MessageInformation = $"Le point doit être compris entre :{CrLf}{ConvertPointDDtoDMS(LimiteSite.Pt0)}{CrLf}et {CrLf}{ConvertPointDDtoDMS(LimiteSite.Pt2)}"
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

    Private Sub Latitude_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelLatitude.Text = Latitude.SelectedItem.ToString
    End Sub

    Private Sub LabelLatitude_Click(sender As Object, e As EventArgs)
        Latitude.DroppedDown = True
    End Sub

    Private Sub Longitude_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelLongitude.Text = Longitude.SelectedItem.ToString
    End Sub

    Private Sub LabelLongitude_Click(sender As Object, e As EventArgs)
        Longitude.DroppedDown = True
    End Sub
End Class