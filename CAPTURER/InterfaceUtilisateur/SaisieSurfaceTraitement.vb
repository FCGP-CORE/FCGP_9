''' <summary> Permet la saisie d'une surface de capture à partir de 2 points de délimitation  ou 
''' directement à partir des index et du nombre de rangées et de colonnes </summary>
Friend Class SaisieSurfaceTraitement
    Private Const DC As String = "Double Click", N As String = "nb de "
    Private Index As Label, IndexUT As Integer, SiteCarto As SitesCartographiques, Echelle As Echelles, IndiceEchelle As Integer, Datum As Datums
    Private PG1, PG2 As PointD, PP1, PP2 As Point, HauteurMetres, LargeurMetres, SurfaceKm As Double
    Private ValeurIndex() As Integer, NumIndex As NI, Direction As Dir
    Private SurfaceTuileLimite As SurfaceTuiles
    Private Diviseur As Integer, ToucheModif As Keys, StockClipCurseur As Rectangle
    Private Titre As String
    ''' <summary> pour indiquer en clair le type des index et des nombres de colonne ou de rangée</summary>
    Private Enum NI As Integer
        DebCol = 0 : NbCol : DebRow : NbRow
    End Enum
    ''' <summary> pour indiquer en clair les directions </summary>
    Private Enum Dir As Integer
        Aucun = 0 : Est : Ouest : Nord : Sud
    End Enum
    ''' <summary> Positionne le formulaire au centre de l'écran et remplit les informations </summary>
    Private Sub SurfaceCaptureTuile_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        StockClipCurseur = Cursor.Clip
        Dim R As (SurfaceCapture As SurfaceTuiles, SelectedIndex As Integer) = CType(Tag, (SurfaceTuiles, Integer))
        Datum = Serveur.Datum
        IndexUT = R.SelectedIndex
        NbTuilesMax.Text = NB_Max_TuilesServeurCarto.ToString("N0")
        NbTuilesMaxX.Text = NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, 1).ToString("N0")
        SiteCarto = CapturerSettings.SITE
        Echelle = Affichage.Echelle
        IndiceEchelle = CapturerSettings.INDICE_ECHELLE
        ReDim ValeurIndex(3)
        RemplirInformations(R.SurfaceCapture)
        SurfaceTuileLimite = New SurfaceTuiles(RegionGrilleToRegionPixels(Serveur.Limites, SiteCarto, Echelle), SiteCarto, Echelle)
        'positionnement du curseur de souris au milieu du bouton ok
        With Valider
            Cursor.Position = New Point(Location.X + .Bounds.X + .Bounds.Width \ 2, Location.Y + .Location.Y + .Size.Height \ 2)
        End With
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> ferme le formulaire et met à jour les points de capture de la visue si demandé </summary>
    Private Sub SurfaceCaptureTuile_FormClosed(sender As Object, e As FormClosedEventArgs)
        If DialogResult = DialogResult.OK Then
            Tag = New SurfaceTuiles(PG1, PG2, SiteCarto, Echelle)
        End If
        Cursor.Clip = StockClipCurseur
        TitreInformation = Titre
    End Sub
    ''' <summary> Remplit les informations des champs tuile et des points </summary>
    Private Sub RemplirInformations(Surface As SurfaceTuiles)
        'mettre à jour les champs dépendants de la surface tuile si elle n'est pas vide
        RemplirChampsTuile(Surface)
        'mettre à jour les champs concernants les 2 points de capture si ils ne sont pas vide et en fonction du type de coordonnées
        Select Case IndexUT
            Case 0 'GRILLE
                MettreAJourPointGrille(PG1, Point1)
                MettreAJourPointGrille(PG2, Point2)
            Case 1 'DD
                MettreAJourPointDD(PG1, Point1)
                MettreAJourPointDD(PG2, Point2)
            Case 2 'DMS
                MettreAJourPointDMS(PG1, Point1)
                MettreAJourPointDMS(PG2, Point2)
            Case 3 'UTM
                MettreAjourPointUTM(PG1, Point1)
                MettreAjourPointUTM(PG2, Point2)
            Case 4 'Tuile
                MettreAjourPointTuile(PG1, Point1)
                MettreAjourPointTuile(PG2, Point2)
        End Select
    End Sub
    ''' <summary> Remplit les informations des points avec des coordoonées en Grille   </summary>
    Private Shared Sub MettreAJourPointGrille(PG As PointD, Lab As Label)
        If PG.IsEmpty Then
            Lab.Text = ""
        Else
            Lab.Text = ConvertPointXYtoChaine(New PointProjection(PG))
        End If
    End Sub
    ''' <summary> Remplit les informations des points avec des coordoonées en DD   </summary>
    Private Sub MettreAJourPointDD(PG As PointD, Lab As Label)
        If PG.IsEmpty Then
            Lab.Text = ""
        Else
            PG = PointGrilleToPointDD(PG, Datum)
            Lab.Text = ConvertPointDDtoChaine(PG)
        End If
    End Sub
    ''' <summary> Remplit les informations des points avec des coordoonées en DMS   </summary>
    Private Sub MettreAJourPointDMS(PG As PointD, Lab As Label)
        If PG.IsEmpty Then
            Lab.Text = ""
        Else
            PG = PointGrilleToPointDD(PG, Datum)
            Lab.Text = ConvertPointDDtoDMS(PG)
        End If
    End Sub
    ''' <summary> Remplit les informations des points avec des coordoonées en UTM </summary>
    Private Sub MettreAjourPointUTM(PG As PointD, Lab As Label)
        If PG.IsEmpty Then
            Lab.Text = ""
        Else
            PG = PointGrilleToPointDD(PG, Datum)
            Lab.Text = ConvertPointDDtoUTM(PG)
        End If
    End Sub
    ''' <summary> Remplit les informations des points avec des coordoonées en tuile_Niveau </summary>
    Private Sub MettreAjourPointTuile(PG As PointD, Lab As Label)
        If PG.IsEmpty Then
            Lab.Text = ""
        Else
            Dim PointPixel As Point = PointGrilleToPointPixel(PG, SiteCarto, Echelle)
            Lab.Text = New PointG(IndiceEchelle, PointPixel).ToString
        End If
    End Sub
    ''' <summary> calcule de manière aprroximative les dimensions de la surface de téléchargement à partir des coordonnées </summary>
    Private Sub CalculerDimensionsCapture(S As SurfaceTuiles)
        Dim DimensionsSurface As (Largeur As Double, Hauteur As Double)
        Dim Pt0, Pt2 As PointD
        If Datum = Datums.Web_Mercator Then ' il s'agit de mètre qui n'en sont plus sorti de l'équateur
            Pt0 = PointGrilleToPointDD(S.RectangleGrille.Pt(0))
            Pt2 = PointGrilleToPointDD(S.RectangleGrille.Pt(2))
            DimensionsSurface = CalculerDimensionsCarte(Pt0, Pt2, True)
        Else
            Pt0 = S.RectangleGrille.Pt(0)
            Pt2 = S.RectangleGrille.Pt(2)
            DimensionsSurface = CalculerDimensionsCarte(Pt0, Pt2, False)
        End If
        LargeurMetres = DimensionsSurface.Largeur
        HauteurMetres = DimensionsSurface.Hauteur
        SurfaceKm = (LargeurMetres * HauteurMetres) / 1000000
    End Sub
    ''' <summary> Remplit les informations des champs tuile </summary>
    Private Sub RemplirChampsTuile(S As SurfaceTuiles)
        ValeurIndex(NI.DebCol) = S.NumColDeb
        DebCol.Text = S.NumColDeb.ToString("N0")
        ValeurIndex(NI.NbCol) = S.NbColonnes
        NbCol.Text = S.NbColonnes.ToString("N0")
        ValeurIndex(NI.DebRow) = S.NumRangDeb
        DebRow.Text = S.NumRangDeb.ToString("N0")
        ValeurIndex(NI.NbRow) = S.NbRangees
        NbRow.Text = S.NbRangees.ToString("N0")
        'on remplit largeur, hauteur, surface et tous les champs associé aux tuiles
        If Not S.IsEmpty Then
            PG1 = S.PtGrille_NO
            PG2 = S.PtGrille_SE
            PP1 = S.PtPixels_NO
            PP2 = S.PtPixels_SE
            CalculerDimensionsCapture(S)
            Largeur.Text = LargeurMetres.ToString("N0")
            Hauteur.Text = HauteurMetres.ToString("N0")
            Surface.Text = SurfaceKm.ToString("N0")
            NbTuilesTelecharger.Text = S.NbTuiles.ToString("N0")
            If S.NbColonnes > NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, 1) Then
                NbTuilesMaxX.BackColor = Color.Tomato
            Else
                NbTuilesMaxX.BackColor = Color.YellowGreen
            End If
            Dim TextTooltip As New StringBuilder("Nombre de tuiles pour la surface de téléchargement encours.", 250)
            If S.NbTuiles > NB_Max_TuilesServeurCarto Then
                NbTuilesMax.BackColor = Color.Tomato
                ToolTexteNbTuile(S, TextTooltip)
            Else
                NbTuilesMax.BackColor = Color.YellowGreen
            End If
            ToolTip1.SetToolTip(NbTuilesTelecharger, TextTooltip.ToString)
        Else
            Largeur.Text = ""
            Hauteur.Text = ""
            Surface.Text = ""
            NbTuilesTelecharger.Text = ""
        End If
    End Sub
    ''' <summary> Action du bouton Fermer </summary>
    Private Sub Fermer_Click(sender As Object, e As EventArgs)
        'si l'on est entrain de saisir un index on valide les donnees avant de fermer
        FermerSaisieIndex()
        Close()
    End Sub
    ''' <summary> ferme la zone de saisie des index et des nombres si elle est ouverte avec validation </summary>
    Private Sub FermerSaisieIndex()
        If SaisieIndex.Visible Then
            SaisieIndex_KeyDown(Nothing, New KeyEventArgs(Keys.Enter))
        End If
    End Sub
    ''' <summary> Configure le formulaire pour qu'il ne puisse pas se fermer sur Enter ou ESC et remplit le champ de saisie </summary>
    Private Sub IndexNombreTuile_Click(sender As Object, e As EventArgs)
        'si l'on est deja entrain de saisir un index on valide les donnees
        FermerSaisieIndex()
        AcceptButton = Nothing
        CancelButton = Nothing
        Index = CType(sender, Label)
        NumIndex = CType(Index.Tag, NI)
        SaisieIndex.Text = ValeurIndexToStr(ValeurIndex(NumIndex))
        SaisieIndex.Select(SaisieIndex.Text.Length, 0)
        SaisieIndex.Location = New Point(Index.Location.X + 3, Index.Location.Y + 1)
        SaisieIndex.Visible = True
        SaisieIndex.Focus()
        ToolTip1.SetToolTip(SaisieIndex, TextToolTip())
    End Sub
    ''' <summary> Permet d'ajuster les index ou les nombres en fonction du modificateur de touche sur le doubleClick </summary>
    Private Sub SaisieIndex_DoubleClick(sender As Object, e As EventArgs)
        If Not String.IsNullOrEmpty(SaisieIndex.Text) Then
            Dim Valeur As Integer = CInt(SaisieIndex.Text)
            If NumIndex = NI.NbCol OrElse NumIndex = NI.NbRow Then ' sur les nombres de col ou row  on peut diviser
                If ToucheModif <> Keys.None Then CalculerModificationNombre()
                If Diviseur > 0 Then
                    Valeur = CInt(Math.Ceiling(Valeur / Diviseur))
                    Diviseur = 0
                Else
                    'on réinitialise à la valeur de départ
                    Valeur = ValeurIndex(NumIndex)
                End If
            Else
                If ToucheModif <> Keys.None Then CalculerModificationIndex()
                'sur les index de col ou de row on peut influer sur la direction
                Select Case Direction
                    Case Dir.Est
                        Valeur += ValeurIndex(NI.NbCol)
                    Case Dir.Ouest
                        Valeur -= ValeurIndex(NI.NbCol)
                    Case Dir.Sud
                        Valeur += ValeurIndex(NI.NbRow)
                    Case Dir.Nord
                        Valeur -= ValeurIndex(NI.NbRow)
                    Case Dir.Aucun
                        'on réinitialise à la valeur de départ
                        Valeur = ValeurIndex(NumIndex)
                End Select
                Direction = Dir.Aucun
            End If
            SaisieIndex.Text = Valeur.ToString("0")
            SaisieIndex.Select(SaisieIndex.Text.Length, 0)
            AcceptButton = Nothing
            CancelButton = Nothing
        End If
    End Sub
    ''' <summary> autorise la touche Tab à être traitée par saisieindex </summary>
    Private Sub SaisieIndex_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        If e.KeyCode = Keys.Tab Then e.IsInputKey = True
    End Sub
    ''' <summary> n'autorise que les touches clavier représentant un chiffre ou les touches d'édition </summary>
    Private Sub SaisieIndex_KeyUp(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.C, Keys.D, Keys.E, Keys.N, Keys.O, Keys.Q, Keys.S, Keys.T
                ToucheModif = Keys.None
        End Select
    End Sub
    ''' <summary> n'autorise que les touches clavier représentant un chiffre ou les touches d'édition </summary>
    Private Sub SaisieIndex_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.NumPad0 To Keys.NumPad9, Keys.D0 To Keys.D9, Keys.Back, Keys.Delete, Keys.Left, Keys.Right
            Case Keys.C, Keys.D, Keys.E, Keys.N, Keys.O, Keys.Q, Keys.S, Keys.T
                ToucheModif = e.KeyCode
                e.SuppressKeyPress = True
                Exit Sub
            Case Keys.Tab
                'DebCol doit être le N°0 dans la liste des controles du panel et les autres doivent suivre l'ordre de NI
                If e.Modifiers = Keys.Shift Then
                    IndexNombreTuile_Click(Support.Controls(If(NumIndex > NI.DebCol, NumIndex - 1, NI.NbRow)), New EventArgs())
                Else
                    IndexNombreTuile_Click(Support.Controls(If(NumIndex < NI.NbRow, NumIndex + 1, NI.DebCol)), New EventArgs())
                End If
                e.SuppressKeyPress = True
            Case Keys.Enter
                If String.IsNullOrEmpty(SaisieIndex.Text) Then
                    ValeurIndex(NumIndex) = 0
                    Index.Text = ""
                Else
                    Dim Valeur As Integer = CInt(SaisieIndex.Text)
                    Select Case NumIndex
                        Case NI.DebCol
                            If (Valeur < SurfaceTuileLimite.NumColDeb OrElse Valeur > SurfaceTuileLimite.NumColFin) AndAlso FlagLimitesSite Then
                                MessageInformation = $"L'index de colonnes doit être compris entre :{CrLf}{SurfaceTuileLimite.NumColDeb} et {SurfaceTuileLimite.NumColFin}"
                                TitreInformation = "Point Hors limites"
                                AfficherInformation()
                                Exit Sub
                            End If
                        Case NI.NbCol
                            Dim ValeurX = ValeurIndex(NI.DebCol)
                            If (ValeurX + Valeur > SurfaceTuileLimite.NumColFin) AndAlso FlagLimitesSite Then
                                MessageInformation = $"Le nb de colonnes doit être compris entre :{CrLf} 0 et {SurfaceTuileLimite.NumColFin - ValeurX }{CrLf}" &
                                                     "par rapport à l'index des colonnes"
                                TitreInformation = "Erreur de saisie"
                                AfficherInformation()
                                Exit Sub
                            End If
                        Case NI.DebRow
                            If (Valeur < SurfaceTuileLimite.NumRangDeb OrElse Valeur > SurfaceTuileLimite.NumRangFin) AndAlso FlagLimitesSite Then
                                MessageInformation = $"L'index de rangées doit être compris entre : {CrLf}{SurfaceTuileLimite.NumRangDeb} et {SurfaceTuileLimite.NumRangFin}"
                                TitreInformation = "Point Hors limites"
                                AfficherInformation()
                                Exit Sub
                            End If
                        Case NI.NbRow
                            Dim ValeurY = ValeurIndex(NI.DebRow)
                            If (ValeurY + Valeur > SurfaceTuileLimite.NumRangFin) AndAlso FlagLimitesSite Then
                                MessageInformation = $"Le nb de rangées doit être compris entre :{CrLf} 0 et {SurfaceTuileLimite.NumRangFin - ValeurY}{CrLf}" &
                                                     "par rapport à l'index des rangées"
                                TitreInformation = "Erreur de saisie"
                                AfficherInformation()
                                Exit Sub
                            End If
                    End Select
                    ValeurIndex(NumIndex) = Valeur
                    Index.Text = Valeur.ToString("N0")
                End If
                SortirSaisieIndex()
                CalculerPointsPixels()
                e.SuppressKeyPress = True
            Case Keys.Escape
                SortirSaisieIndex()
                e.SuppressKeyPress = True
            Case Else
                e.SuppressKeyPress = True
        End Select
    End Sub
    ''' <summary> Configure le formulaire pour une fermeture par ESC ou ENTER </summary>
    Private Sub SortirSaisieIndex()
        'on marque saisieIndex comme fermer
        SaisieIndex.Visible = False
        'on permet au formumaire principal de réagir avec les touches Enter et Escape
        AcceptButton = Valider
        CancelButton = Annuler
    End Sub
    ''' <summary> calcul les modification à apporter aux index ou aux nombres </summary>
    Private Sub CalculerModificationIndex()
        Direction = Dir.Aucun
        Select Case ToucheModif
            Case Keys.E
                If NumIndex = NI.DebCol Then Direction = Dir.Est
            Case Keys.N
                If NumIndex = NI.DebRow Then Direction = Dir.Nord
            Case Keys.O
                If NumIndex = NI.DebCol Then Direction = Dir.Ouest
            Case Keys.S
                If NumIndex = NI.DebRow Then Direction = Dir.Sud
        End Select
    End Sub
    ''' <summary> trouve le divisiseur des nombres en fonction de la touche appuyée </summary>
    Private Sub CalculerModificationNombre()
        Diviseur = 0
        Select Case ToucheModif
            Case Keys.C
                Diviseur = 5
            Case Keys.D
                Diviseur = 2
            Case Keys.N
                Diviseur = 9
            Case Keys.Q
                Diviseur = 4
            Case Keys.S
                Diviseur = 7
            Case Keys.T
                Diviseur = 3
        End Select
    End Sub
    ''' <summary> ouvre le formulaire adequat pour la saisie d'un point de coordonnée </summary>
    Private Sub Point_Click(sender As Object, e As EventArgs)
        Cursor.Clip = StockClipCurseur
        'si l'on est deja entrain de saisir un index on valide les donnees
        If SaisieIndex.Visible = True Then SaisieIndex_KeyDown(Nothing, New KeyEventArgs(Keys.Enter))
        Dim NomPoint As String = CType(sender, Label).Name
        Dim PositionSaisiePoints As Point
        Dim Coordonnees As PointD
        If NomPoint = "Point1" Then
            PositionSaisiePoints = PointToScreen(Point1.Location)
            Coordonnees = PG1
        Else
            PositionSaisiePoints = PointToScreen(Point2.Location)
            Coordonnees = PG2
        End If
        PositionSaisiePoints.Y += 2
        PositionSaisiePoints.X += 2
        If OuvrirFormulaireSaisieCoordonnees(Me, PositionSaisiePoints, IndexUT, Coordonnees) = DialogResult.OK Then
            If NomPoint = "Point1" Then
                PG1 = Coordonnees
            Else
                PG2 = Coordonnees
            End If
            RemplirInformations(New SurfaceTuiles(PG1, PG2, SiteCarto, Echelle))
        End If
        Cursor.Clip = New Rectangle(Location, Size)
    End Sub
    ''' <summary> Recalcul les points de capture en fonction des nouveaux index de tuiles </summary>
    Private Sub CalculerPointsPixels()
        If ValeurIndex(NI.DebCol) > 0 AndAlso ValeurIndex(NI.DebRow) > 0 AndAlso ValeurIndex(NI.NbCol) > 0 AndAlso ValeurIndex(NI.NbRow) > 0 Then
            Dim Pt0Tuile As New Point(ValeurIndex(NI.DebCol), ValeurIndex(NI.DebRow))
            Dim Pt2Tuile As New Point(ValeurIndex(NI.DebCol) + ValeurIndex(NI.NbCol) - 1, ValeurIndex(NI.DebRow) + ValeurIndex(NI.NbRow) - 1)
            PP1 = TuileToPointPixel(Pt0Tuile, False)
            PP2 = TuileToPointPixel(Pt2Tuile, True)
            RemplirInformations(New SurfaceTuiles(PP1, PP2, SiteCarto, Echelle))
        End If
    End Sub
    ''' <summary> renvoie un texte d'aide pour les index en fonction du type d'index</summary>
    ''' <param name="TypeIndex"> type d'index </param>
    Private Shared Function ToolTexteIndex(TypeIndex As String) As String
        If TypeIndex = "colonnes" Then
            Return $"{DC} + O --> Déplace l'index vers l'Ouest du {N}{TypeIndex}." & CrLf &
                   $"{DC} + E --> Déplace l'index vers l'Est du {N}{TypeIndex}." & CrLf &
                   $"{DC} --> Initialise l'index de {TypeIndex} à sa valeur de départ."
        Else
            Return $"{DC} + N --> Déplace l'index vers le Nord du {N}{TypeIndex}." & CrLf &
                   $"{DC} + S --> Déplace l'index vers le Sud du {N}{TypeIndex}." & CrLf &
                   $"{DC} --> Initialise l'index de {TypeIndex} à sa valeur de départ."
        End If
    End Function
    ''' <summary> renvoie un texte d'aide pour les nombres en fonction du type d'index</summary>
    ''' <param name="TypeIndex"> type d'index </param>
    Private Shared Function ToolTexteNombre(TypeIndex As String) As String
        Return $"{DC} + D --> Divise le {N}{TypeIndex} par 2." & CrLf &
               $"{DC} + T --> Divise le {N}{TypeIndex} par 3." & CrLf &
               $"{DC} + Q --> Divise le {N}{TypeIndex} par 4." & CrLf &
               $"{DC} + C --> Divise le {N}{TypeIndex} par 5." & CrLf &
               $"{DC} + S --> Divise le {N}{TypeIndex} par 7." & CrLf &
               $"{DC} + N --> Divise le {N}{TypeIndex} par 9." & CrLf &
               $"{DC} --> Initialise le {N}{TypeIndex} à sa valeur de départ."
    End Function
    ''' <summary> renvoie un texte d'aide en fonction de l'index ou du nombre passé en paramètre </summary>
    Private Function TextToolTip() As String
        Select Case NumIndex
            Case NI.DebCol
                Return ToolTexteIndex("colonnes")
            Case NI.NbCol
                Return ToolTexteNombre("colonnes")
            Case NI.DebRow
                Return ToolTexteIndex("rangées")
            Case NI.NbRow
                Return ToolTexteNombre("rangées")
            Case Else
                Return ""
        End Select
    End Function
    '''<summary> met à jour la description de la décomposition du nb de tuiles si celui-ci dépasse le nb de tuiles Max </summary>
    Private Shared Sub ToolTexteNbTuile(S As SurfaceTuiles, TextTooltip As StringBuilder)
        'mettre à jour le nb de rectangles pour découper la surface principale afin que le nb de tuiles maxi ne soit dépassé
        Dim Exa = Facteurs.NbMiniRectangle(S.NbColonnes, S.NbRangees, NB_Max_TuilesServeurCarto, True, True)
        Dim NExa = Facteurs.NbMiniRectangle(S.NbColonnes, S.NbRangees, NB_Max_TuilesServeurCarto, False, False)
        TextTooltip.AppendLine($"{CrLf}Décomposition possible de la surface")
        TextTooltip.Append($"Exact : Nb = {Exa.NbMini & CrLf}X : {MulText(S.NbColonnes, Exa.MultipleX, Exa.Xn)}")
        TextTooltip.Append($", Y : {MulText(S.NbRangees, Exa.MultipleY, Exa.Yn)}, Nb Tuiles : {Exa.Xn * Exa.Yn}")
        If Exa.NbMini <> NExa.NbMini Then
            TextTooltip.Append($"{CrLf}{If(NExa.IsExact, "Autre possibilité", "Pas Exact")} : Nb = {NExa.NbMini & CrLf}X : {MulText(S.NbColonnes, NExa.MultipleX, NExa.Xn)}")
            TextTooltip.Append($", Y : {MulText(S.NbRangees, NExa.MultipleY, NExa.Yn)}, Nb Tuiles : {NExa.Xn * NExa.Yn}")
        End If
    End Sub
    '''<summary> renvoie un texte décrivant une division en texte en fonction du divisieur </summary>
    Private Shared Function MulText(Nb As Integer, Mul As Integer, Result As Integer) As String
        MulText = If(Mul = 1, $"{Nb}", $"{Nb}/{Mul}={Result}")
    End Function
    '''<summary> transforme un nombre entier en texte </summary>
    Private Shared Function ValeurIndexToStr(ValeurIndex As Integer) As String
        Dim Ret As String = ""
        If ValeurIndex > 0 Then
            Ret = ValeurIndex.ToString()
        End If
        Return Ret
    End Function

    ''' <summary> contient la décomposition d'un nombre en facteurs </summary>
    Private Class Facteurs
        ''' <summary> calcul le nb minimal de rectangles nécessaires pour couvrir nbtotal sans dépasser nbmax </summary>
        ''' <param name="X"> Largeur de la surface </param>
        ''' <param name="Y"> Hauteur de la surface </param>
        ''' <param name="NbMax"> Nombre maximum autorisé pour la surface représenté par X*Y </param>
        ''' <param name="FlagExact"> autorise t'on une surface un peu plus grande que la surface initiale. True : Non. Dans ce cas on ne prend pas en compte FlagPremier </param>
        ''' <param name="FlagPremier"> Autorise t'on les nb premiers comme retour. Cela suppose qu'une seule dimension soit coupée </param>
        Friend Shared Function NbMiniRectangle(X As Integer, Y As Integer, NbMax As Integer, Optional FlagExact As Boolean = True, Optional FlagPremier As Boolean = False) _
                                As (NbMini As Integer, IsExact As Boolean, Xn As Integer, MultipleX As Integer, Yn As Integer, MultipleY As Integer)
            Dim FactDimension As Facteurs, Reste, Result, NbMini As Integer
            Dim Surface As Integer = X * Y
            'si le nbtotal est < nbmax on peut télécharger en une seule fois
            If Surface <= NbMax Then Return (1, True, X, 1, Y, 1)
            NbMini = CInt(Math.Ceiling(Surface / NbMax)) 'nb minimum de rectangles pour découper la surface initiale et respecter la contrainte de NbMax
            'on veut absolument que le nb de rectangle mini renvoie le même nb de tuile que la surface initiale au détriment du nb de rectangle qui peut devenir très grand.
            'il y a forcément un diviseur qui fonctionne c'est 1
            If FlagExact Then
                Do
                    Result = Math.DivRem(Surface, NbMini, Reste)
                    If Reste = 0 Then 'on a trouver le nb mini qui coupe la surface en rectangle entier
                        FactDimension = New Facteurs(NbMini)
                        Dim MultipleX As Integer = 1, MultipleY As Integer = 1
                        For Cpt As Integer = FactDimension.Multiples.Length - 1 To 0 Step -1 ' il y a forcément 1 ou 2 dimensions qui sont divisibles par le multiple encours
                            Dim ResteX, ResteY As Integer, Multiple As Integer = FactDimension.Multiples(Cpt)
                            Dim ResultX As Integer = Math.DivRem(X, Multiple, ResteX)
                            Dim ResultY As Integer = Math.DivRem(Y, Multiple, ResteY)
                            If ResteX = 0 And ResteY > 0 Then 'x est divisible par le multiple de nbmini
                                X = ResultX
                                MultipleX *= Multiple
                            ElseIf ResteX > 0 And ResteY = 0 Then 'Y est divisible par le multiple de nbmini
                                Y = ResultY
                                MultipleY *= Multiple
                            Else 'X et Y sont divisibles par le multiple de nbmini et on prend la dimension la plus grande
                                If X > Y Then
                                    X = ResultX
                                    MultipleX *= Multiple
                                Else
                                    Y = ResultY
                                    MultipleY *= Multiple
                                End If
                            End If
                        Next
                        Return (NbMini, True, X, MultipleX, Y, MultipleY)
                    Else
                        NbMini += 1
                    End If
                Loop While True
            Else
                'on privilégie le nb de rectangle minimum au détriment du nb de tuiles à télécharger qui pourra être plus grand
                'si on veut pouvoir diviser les 2 cotés il ne faut pas que nbmini soit premier
                If Not FlagPremier Then
                    If NbMini = 2 Then
                        NbMini = 4 '1er nombre non premier
                    ElseIf IsNbPremier(NbMini) Then
                        NbMini += 1 ' les autres sont impair il suffit de rajouter 1 pour les rendre pair donc non premier
                    End If
                End If
                FactDimension = New Facteurs(NbMini)
                'on regarde si l'opération renvoie le même nb de tuiles que celui de la surface initiale
                Result = Math.DivRem(Surface, NbMini, Reste) 'normalement c'es
                'si NbMini est premier (1 seul multiple total) on ne peut diviser la surface que d'un seul coté
                If FactDimension.IsPremier Then
                    'calcul des dimensions divisées par le multiple
                    Dim X0 As Integer = CInt(Math.Ceiling(X / NbMini)), Y0 As Integer = CInt(Math.Ceiling(Y / NbMini))
                    'on prend celui qui rajoute le moins de tuile par rapport à la surface initiale
                    If X0 * Y < Y0 * X Then
                        Return (NbMini, Reste = 0, X0, NbMini, Y, 1)
                    Else
                        Return (NbMini, Reste = 0, X, 1, Y0, NbMini)
                    End If
                Else
                    'si il y a 2 multiples ou plus il faut trouver une composition qui donne 1 diviseur pour chaque dimension
                    'et faire en sorte de prendre ceux qui ajoutent le moins de tuiles
                    Dim Multiple0 As Integer = 1, Multiple1 As Integer = 1
                    For Cpt As Integer = FactDimension.Multiples.Length - 1 To 0 Step -1
                        If Multiple0 > Multiple1 Then
                            Multiple1 *= FactDimension.Multiples(Cpt)
                        Else
                            Multiple0 *= FactDimension.Multiples(Cpt)
                        End If
                    Next
                    Dim X0 As Integer = CInt(Math.Ceiling(X / Multiple0)),
                        X1 As Integer = CInt(Math.Ceiling(X / Multiple1)),
                        Y0 As Integer = CInt(Math.Ceiling(Y / Multiple0)),
                        Y1 As Integer = CInt(Math.Ceiling(Y / Multiple1))
                    If X0 * Y1 < X1 * Y0 Then
                        Return (NbMini, X0 * Multiple0 * Y1 * Multiple1 = Surface, X0, Multiple0, Y1, Multiple1)
                    Else
                        Return (NbMini, X1 * Multiple1 * Y0 * Multiple0 = Surface, X1, Multiple1, Y0, Multiple0)
                    End If
                End If
            End If
        End Function
        ''' <summary> les 33 premiers nombres Premier. sert à accélérer les calculs et recherches associés aux nbs premiers </summary>
        Private Shared ReadOnly NombresPremiersConnus As Integer() = {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,
                                                                      67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137}
        Private ReadOnly ListeMultiples As List(Of Integer)
        '''<summary> tableau des mulitples triés par ordre croissant issus de la décomposition du nb </summary>
        Private ReadOnly Property Multiples As Integer()
            Get
                Return ListeMultiples.ToArray
            End Get
        End Property
        ''' <summary> indique si le nb est un nombre premier </summary>
        Private ReadOnly Property IsPremier As Boolean
            Get
                Return ListeMultiples.Count = 1
            End Get
        End Property
        ''' <summary> nombre initial </summary>
        Private ReadOnly Nombre As Integer
        ''' <summary> interdit l'initialisation de la structure en externe </summary>
        Private Sub New()
        End Sub
        ''' <summary> initialisation de la structure en interne</summary>
        Private Sub New(NbInitial As Integer)
            If NbInitial > 1 And NbInitial < 18770 Then
                Nombre = NbInitial
                ListeMultiples = New List(Of Integer)(15)
                DecomposerNombre()
            Else
                Throw New Exception("Le nombre à tester doit être compris entre 2 et 18769 inclus")
            End If
        End Sub
        ''' <summary> décompose un nombre en multiple de nb premiers </summary>
        Private Sub DecomposerNombre()
            Dim NbInit = Nombre, IndNbPrem As Integer, Max As Integer = CInt(Math.Floor(Math.Sqrt(Nombre))) 'borne à laquelle s'arrête les nb premiers connus
            Do
                Dim Reste As Integer, Result As Integer = Math.DivRem(NbInit, NombresPremiersConnus(IndNbPrem), Reste)
                If Reste = 0 Then 'NbInit est divisible par le nombre premier connu encours
                    ListeMultiples.Add(NombresPremiersConnus(IndNbPrem)) 'on l'ajoute à la liste des multiples
                    If Result = 1 Then
                        Exit Sub 'on a finit de décomposer NbInit en nombre premier connu on sort
                    Else
                        NbInit = Result 'on permute pour avoir la nouvelle valeur sur NbInit
                    End If
                Else 'NbInit n'est pas divisible par le nombre premier connu encours
                    IndNbPrem += 1 'donc on passe au nb premier connu suivant
                End If
            Loop While NombresPremiersConnus(IndNbPrem) <= Max
            'si ListeMultiples.count = 1 alors NbInit est un nombre premier non connu 
            'sinon c'est un multiple d'un ou plusieurs nombre premier connu et NbInit est Nb Premier connu ou pas
            ListeMultiples.Add(NbInit)
        End Sub
        ''' <summary> indique si un nombre est un nombre premier </summary>
        ''' <param name="Nombre"> nombre à tester </param> 
        Private Shared Function IsNbPremier(Nombre As Integer) As Boolean
            IsNbPremier = False
            If Nombre > 1 And Nombre < 18770 Then
                If NombresPremiersConnus.Contains(Nombre) OrElse IsNbPremierNonConnu(Nombre) Then IsNbPremier = True
            Else
                Throw New Exception("Le nombre à tester doit être compris entre 2 et 18769 inclus")
            End If
        End Function
        ''' <summary> renvoie true si NbInit est un nombre premier non connu </summary>
        ''' <param name="NbInit"> nombre à tester</param>
        Private Shared Function IsNbPremierNonConnu(NbInit As Integer) As Boolean
            IsNbPremierNonConnu = False
            Dim IndicePremier As Integer, Max As Integer = CInt(Math.Floor(Math.Sqrt(NbInit)))  'borne à laquelle s'arrête les nbs premiers connus pour le nb à tester
            Do
                If NombresPremiersConnus(IndicePremier) <= Max Then 'on commence par diviser NBInit par et au max les 33 Nombres Premier connus et stockés
                    Dim Reste As Integer, Result As Integer = Math.DivRem(NbInit, NombresPremiersConnus(IndicePremier), Reste)
                    If Reste = 0 Then 'Nb à tester est divisible par le nombre premier connu encours
                        Exit Do 'on a finit car le nb à tester est un multiple d'un nb premier connu
                    Else 'Nb à tester n'est pas divisible par le nombre premier connu encours
                        IndicePremier += 1 'on passe au nb premier connu suivant
                    End If
                Else ' on a finit car on a passé tous les nb premier inférieur à la racine du nb initial
                    IsNbPremierNonConnu = True ' NbInit est multiple d'un nombre premier connu
                    Exit Do
                End If
            Loop While IndicePremier <> -1 'on fait en sorte que la boucle soit infinie
        End Function
    End Class
End Class