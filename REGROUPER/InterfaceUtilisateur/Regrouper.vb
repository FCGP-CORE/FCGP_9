Imports FCGP.Regroupement

''' <summary> formulaire principal de l'application FCGP_Regrouper </summary>
Friend NotInheritable Class Regrouper
#Region "variables Privées"
    Private Enum ModesFCGP As Integer
        ''' <summary>il n'y a aucune carte ou échelle affichée</summary>
        AucunMode = 0
        ''' <summary>Mode Dépalcer carte</summary>
        DeplacerCarte = 1
        ''' <summary>Mode Créer Trace</summary>
        CreerTrace = 2
        ''' <summary>Mode Modifier Trace</summary>
        ModifierTrace = 3
        ''' <summary>mode qui permet l'export d'une séléction de carte sous forme de fichier KMZ</summary>
        ExporterSelectionKMZ = 4
        ''' <summary>mode qui permet l'impression d'une sélection de carte</summary>
        ImprimerSelection = 5
    End Enum
    ''' <summary>les différents outils ou mode disponible dans FCGP_Visue</summary>
    ''' <remarks> pour l'instant seul l'outil Aucun et DeplacerCarte sont pris en compte </remarks>
    Private Enum Outils As Integer
        ''' <summary>il n'y a aucune carte ou échelle affichée</summary>
        AucunOutil = 0
        ''' <summary>Mode Dépalcer carte</summary>
        DeplacerCarte = 1
        ''' <summary>Mode Créer Trace</summary>
        CreerTrace = 2
        ''' <summary>Mode Modifier Trace</summary>
        ModifierTrace = 3
        ''' <summary>mode qui permet l'export d'une séléction de carte sous forme de fichier KMZ</summary>
        ExporterSelectionKMZ = 4
        ''' <summary>mode qui permet l'impression d'une sélection de carte</summary>
        ImprimerSelection = 5
        ''' <summary>en mode déplacer carte, l'outil de déplacement (clic de souris maintenu)</summary>
        DeplacementCarteHV = 6
        ''' <summary>mode qui permet l'impression d'une sélection de carte</summary>
        DeplacementPointTrace = 7
        ''' <summary>mode qui permet l'impression d'une sélection de carte</summary>
        SuppressionPointTrace = 8
        ''' <summary>mode qui permet l'impression d'une sélection de carte</summary>
        InsertionPointTrace = 9
        ''' <summary>en mode déplacer carte, l'outil de zoom (mollette de la souris sans modificateur)</summary>
        Zoom = 10
        ''' <summary>mode KMZ</summary>
        KMZ = 11
        ''' <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
        Attente = 12
        ''' <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
        DeplacementCarteH = 13
        ''' <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
        DeplacementCarteV = 14
        ''' <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
        DeplacementCarteVHS = 15
        ''' <summary>en mode déplacer carte, l'outil d'attente pour les opérations longues de chargement des échelles ou des cartes</summary>
        DeplacementCarteVSH = 16

    End Enum
    Private WriteOnly Property OutilEncours As Outils
        Set(value As Outils)
            _OutilEncours = value
            AffichageCarte.Cursor = CurseurOutilEncours
            AffichageCarte.Refresh()
            LOutilEnCours.Text = _OutilEncours.ToString
            LOutilEnCours.Refresh()
        End Set
    End Property
    Private ReadOnly Property DimTuile As Integer
        Get
            If ListeChoixDimensions.SelectedIndex = 0 Then
                'Dimensions des tuiles pour les fichiers JNX ou ORUX
                Return RegrouperSettings.DIM_TUILE_JNXORUX
            Else
                'Dimensions des tuiles pour les fichiers KMZ
                Return RegrouperSettings.DIM_TUILE_KMZ
            End If
        End Get
    End Property
    ''' <summary>Nb d'unité en monde virtuel pour un déplacement Horizontal ou vertical soit à la molette de souris soit aux touches de déplacement</summary>
    Private ReadOnly Property DeltaDeplacement As Integer
        Get
            Return CInt(DeltaDeplacementPixel / ZoomAffichage)
        End Get
    End Property
    Private ReadOnly Property DeltaDeplacementVertical As Integer
        Get
            Return CInt(TailleAffichage.Height / ZoomAffichage / 2)
        End Get
    End Property
    Private ReadOnly Property DeltaDeplacementHorizontal As Integer
        Get
            Return CInt(TailleAffichage.Width / ZoomAffichage / 2)
        End Get
    End Property
    ''' <summary>les évenements concernant l'affichage des cartes sont autorisés, c'est à dire qu'il y a une échelle affichée</summary>
    Private ReadOnly Property EvenementCarteAutorise As Boolean
        Get
            Return ClefRegroupementEncours IsNot Nothing
        End Get
    End Property
    ''' <summary>le changement d'affichage et la suppression des échelles est possible</summary>
    Private Shared ReadOnly Property GestionRegroupementAutorise As Boolean
        Get
            Return Regroupements.Count > 0
        End Get
    End Property
    ''' <summary> renvoie le curseur associé à l'outil encours </summary>
    Private ReadOnly Property CurseurOutilEncours As Cursor
        Get
            Return CurseursOutils(_OutilEncours)
        End Get
    End Property
    ''' <summary>region en unité du monde virtuel représentée par la surface d'affichage de la carte. 
    ''' Cette région est dépendante du coef Zoom et de la surface en pixel de la surface d'affichage</summary>
    ''' <remarks>est modifié par une action de zoom appliquée à la surface d'affichage ou par un changement de la surface d'affichage</remarks>
    Private ReadOnly Property RegionAffichage As Rectangle
        Get
            Return New Rectangle(Pt0Affichage, New Size(CInt(TailleAffichage.Width / ZoomAffichage), CInt(TailleAffichage.Height / ZoomAffichage)))
        End Get
    End Property
    ''' <summary>point en unité du monde virtuel représentée par le centre de la surface d'affichage de la carte. 
    ''' Ce point est directement dépendant du coef Zoom et de la surface en pixel de la surface d'affichage</summary>
    ''' <remarks>est modifié par une action de zoom appliquée à la surface d'affichage</remarks>
    Private ReadOnly Property CentreAffichage As Point
        Get
            Return PixelsEcranToPixelsSite(New Point(TailleAffichage.Width \ 2, TailleAffichage.Height \ 2))
        End Get
    End Property
    Private Function PixelsEcranToPixelsSite(PixelsEcran As Point) As Point
        Return New Point(Pt0Affichage.X + CInt(PixelsEcran.X / ZoomAffichage), Pt0Affichage.Y + CInt(PixelsEcran.Y / ZoomAffichage))
    End Function
    ''' <summary> transforme un point donné en pixels de la projection grille et pour l'echelle encours, en pixels de la surface d'affichage </summary>
    Private Function PixelsSiteToPixelsEcran(PixelsSite As Point) As Point
        Return New Point(CInt(PixelsSite.X * ZoomAffichage) - Pt0Affichage.X, CInt(PixelsSite.Y * ZoomAffichage) - Pt0Affichage.Y)
    End Function

    ''' <summary>dernière position enregistrée lors d'un évenement, du pointeur de souris sur la surface d'affichage de la carte exprimée en monde virtuel</summary>
    '''<remarks>Ce point est directement dépendant du coef Zoom et de la position en pixel du pointeur de souris </remarks>
    Private ReadOnly Property CoordMousePixels As Point
        Get
            Return PixelsEcranToPixelsSite(PosMouse)
        End Get
    End Property

    ''' <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection grille </summary>
    Friend Shared ReadOnly Property CoordMouseGrille(PtPixels As Point, SiteCarto As SitesCartographiques, Echelle As Echelles) As PointD
        Get
            Return PointPixelToPointGrille(PtPixels, SiteCarto, Echelle)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection Grille sous forme de chaine caractères formatée </summary>
    Friend Shared ReadOnly Property CoordMouseGrilleText(PtGrille As PointD) As String
        Get
            Return ConvertPointXYtoChaine(New PointProjection(PtGrille))
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux </summary>
    Friend Shared ReadOnly Property CoordMousseDD(PtGrille As PointD, Datum As Datums) As PointD
        Get
            Return PointGrilleToPointDD(PtGrille, Datum)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux sous forme de chaine de caractères formatée </summary>
    Friend Shared ReadOnly Property CoordMousseDDText(PtGrille As PointD, Datum As Datums) As String
        Get
            Return ConvertPointDDtoChaine(CoordMousseDD(PtGrille, Datum))
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimée en degrés Miniute et secondes sous forme de chaine de caractères formatée </summary>
    Friend Shared ReadOnly Property CoordMouseDMSText(PtGrille As PointD, Datum As Datums) As String
        Get
            Return ConvertPointDDtoDMS(CoordMousseDD(PtGrille, Datum))
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimée en mètre de la projection UTM WGS84 sous forme de chaine de caractères formatée </summary>
    Friend Shared ReadOnly Property CoordMouseUtmWGS84(PtGrille As PointD, Datum As Datums) As String
        Get
            Return ConvertPointDDtoUTM(CoordMousseDD(PtGrille, Datum))
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en index tuile et offset. </summary>
    Friend Shared ReadOnly Property CoordMousePointG(PtPixels As Point, IndiceEchelle As Integer) As PointG
        Get
            Return New PointG(IndiceEchelle, PtPixels)
        End Get
    End Property
    ''' <summary> 4 variables utiles pour le calcul des différents types de coordonnées concernant le regroupement encours </summary>
    Friend Shared ReadOnly Property Datum As Datums
    Friend Shared ReadOnly Property SiteCarto As SitesCartographiques
    Friend Shared ReadOnly Property Echelle As Echelles
    Friend Shared ReadOnly Property IndiceEchelle As Integer
    ''' <summary> altitude au coordonnées du pointeur de souris </summary>
    Private ReadOnly Property CoordMouseAltitude() As String
        Get
            If AltitudesSettings.IS_ALTITUDE Then
                Dim AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMousseDD(CoordMouseGrille(CoordMousePixels, _SiteCarto, _Echelle), _Datum))
                Return If(AltitudePosMouse = -9999, "0 m", AltitudePosMouse.ToString & " m")
            Else
                Return "--- m"
            End If
        End Get
    End Property
    ''' <summary> regroupement actuellement encours d'affichage ou regroupement de travail</summary> 
    Private ReadOnly Property RegroupementEncours As Regroupement
        Get
            Return If(ClefRegroupementEncours Is Nothing, Nothing, Regroupements(ClefRegroupementEncours))
        End Get
    End Property
    ''' <summary> système Cartographique associé au regroupement en cours</summary> 
    Private ReadOnly Property SystemeEncours As SystemeCartographique
        Get
            Return RegroupementEncours?.SystemeType
        End Get
    End Property
    ''' <summary> indique si il y a une sélection pour le regroupement en cours</summary> 
    Private ReadOnly Property FlagSelectionEncours As Boolean
        Get
            Return SystemeEncours IsNot Nothing AndAlso Not SelectionReferencementSiteCarto(RegroupementEncours.NumSite).IsEmpty
        End Get
    End Property

    ''' <summary> Liste des différentes unités pour les coordonnées du pointeur de souris. Evite une variable static interne méthode </summary>
    Private ReadOnly CoordType As String() = New String() {"DD WGS84", "DMS WGS84", "UTM WGS84"}
    ''' <summary>Quel est le mode en cours</summary>
    ''' <remarks> Pour l'instant seul le mode déplacer carte est pris en charge </remarks>
    Private ModeEncours As ModesFCGP = ModesFCGP.AucunMode
    ''' <summary>Quel est l'outil ou le mode en cours</summary>
    Private _OutilEncours As Outils
    ''' <summary>collection de curseurs correspondant aux différents outils disponible dans FCGP_Visue</summary>
    Private ReadOnly CurseursOutils([Enum].GetNames(GetType(Outils)).GetLength(0) - 1) As Cursor
    ''' <summary>Nb de pixel pour un déplacement Horizontal ou vertical soit à la molette de souris soit aux touches de déplacement</summary>
    Private DeltaDeplacementPixel As Integer
    ''' <summary>couleur d'affichage par defaut du fond de l'affichage carte</summary>
    Private CouleurAffichage As Color
    ''' <summary>flag indiquant que le bouton gauche de la souris est appuyé. Cela sert dans l'outil de déplacement de la carte au niveau de l'évènement move de la souris</summary>
    Private FlagSourisDown As Boolean
    ''' <summary>flag indiquant qu'il ne faut pas prendre en compte l'évenement encours. Principalement pour la gestion des menus : par exemple combo ChoixEchelle</summary>
    Private FlagIgnoreEvenement As Boolean
    ''' <summary>clef de l'échelle en cours</summary>
    Private ClefRegroupementEncours As String
    ''' <summary>Coef du Zoom. inférieur à 1 on voit plus de carte, supérieur à 1 on voit moins de carte</summary>
    Private ZoomAffichage As Double = 1
    ''' <summary>taille en pixel de la zone client de la picturebox qui supporte l'affichage de la carte. Mis à jour à la fin de l'évenement Resize</summary> 
    Private TailleAffichage As Size
    ''' <summary>Point de reférence de la surface d'affichage de la carte par rapport au monde virtuel de l'échelle</summary>
    ''' <remarks>peut être modifié par une action de déplacement ou par une action de zoom ou par une action de resize appliquée à la surface d'affichage de la carte</remarks>
    Private Pt0Affichage As Point
    ''' <summary>dernière position enregistrée lors d'un évenement, du pointeur de souris sur la surface d'affichage de la carte exprimée en pixel</summary>
    ''' <remarks>permet de garder en mémoire la position du pointeur de souris afin de calculer un delta en pixel avec la nouvelle position du pointeur</remarks>
    Private PosMouse As Point
#End Region
#Region "Evènement du formulaire"
    ''' <summary> Actions lors de l'ouverture du formulaire </summary>
    Private Sub Regrouper_Load(sender As Object, e As EventArgs)
        'initialisation du type de programme
        InitialiserBaseApplication(Me, "Core 4.000 VB", "01/03/2023")
        TitreInformation = "Information FCGP_Regrouper"
        'indique aux threads hors UI la procédure à appeler pour afficher un message d'information ou une erreur
        FormHorsUI = New Action(AddressOf AfficherInfomationsErreurs)
        If Not InitialiserSettings("9.000") Then
            Close()
            Exit Sub
        End If
        Try
            InitialiserCommun()
            InitialiserCurseursVisue()
            InitialiserAffichageCarte(True)
            InitialiserRegionAffichageCarte(True)
            If AltitudesSettings.IS_ALTITUDE Then
                InitialiserListeFichiersAltitudes()
                InitialiserConnexionAltitudes()
            End If
            Text = NumFCGP
            ListeChoixDimensions.SelectedIndex = RegrouperSettings.TYPE_DIMENSIONS
            GererMenus()
            MAJ_ZoneInfoRegroupement()
            MAJ_ZoneInfoCoordonnees()
        Catch Ex As Exception
            AfficherErreur(Ex, "O8K0")
        End Try
    End Sub
    ''' <summary> permet de traiter la touche alt comme modifiers et non comme sélecteur du menu </summary>
    Private Sub Regrouper_KeyUp(sender As Object, e As KeyEventArgs)
        If e.KeyData = Keys.Alt Then
            AffichageCarte.Select()
            e.SuppressKeyPress = True
        End If
    End Sub
    ''' <summary>Actions lors de la fermeture du formulaire</summary>
    Private Sub Regrouper_FormClosed(sender As Object, e As FormClosedEventArgs)
        'on sauvegarde les informations de travail
        RegrouperSettings.Ecrire()
        'arrête éventuellement le téléchargement  de fichiers d'altitude en cours et libère les resource
        CloturerCommun()
        CloturerAltitudes()
        'on attend que le thread des téléchargements libère effectivement les ressources
        Thread.Sleep(500)
    End Sub
    ''' <summary> met à jour la surface d'affichage de l'application </summary>
    Private Sub Regrouper_Shown(sender As Object, e As EventArgs)
#If Not BETA Then
        VerifierVersionApplication()
#End If
    End Sub
    ''' <summary> gère les touches de fonction de l'application </summary>
    Private Sub Regrouper_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.F11
                If WindowState = FormWindowState.Maximized Then
                    WindowState = FormWindowState.Normal
                Else
                    WindowState = FormWindowState.Maximized
                End If
            Case Keys.F1
                AfficherAide(Me)
            Case Keys.F3
                OuvrirConfiguration()
            Case Keys.F4
                If e.Modifiers = Keys.Alt Then
                    Close()
                Else
                    OuvrirAPropos()
                End If
            Case Keys.NumPad0 To Keys.NumPad3, Keys.D0 To Keys.D3
                If RegroupementEncours.SelectionGeoref.IsEmpty Then Exit Sub
                If e.Control Then
                    AfficherCoinSelection(e.KeyCode)
                End If
            Case Keys.NumPad4, Keys.D4
                'on positionne le curseur au centre de l'affichage soit le coin choisi
                Cursor.Position = AffichageCarte.PointToScreen(New Point(TailleAffichage.Width \ 2, TailleAffichage.Height \ 2))
                'affichage à l'écran des coordonnées du pointeur de souris  
                MAJ_ZoneInfoCoordonnees()
            Case Keys.X, Keys.Y, Keys.L, Keys.H
                If RegroupementEncours.SelectionGeoref.IsEmpty Then Exit Sub
                If e.Modifiers = Keys.Control OrElse e.Modifiers = Keys.Shift OrElse e.Modifiers = Keys.Alt Then
                    ModifierSelection(e.KeyCode)
                End If
        End Select
    End Sub
    ''' <summary> Stocke la surface d'affichage de l'application. Ne sert à rien pour l'instant. Préparation pour les traces </summary>
    Private Sub Regrouper_LocationChanged(sender As Object, e As EventArgs)
        VisueRectangle = AffichageCarte.RectangleToScreen(AffichageCarte.ClientRectangle)
    End Sub
#End Region
#Region "évenement VisualisationCarte"
    ''' <summary> redessine la carte avec la position indiquée indiquée en coordonnées virtuelles, le coef zoom et la surface d'affichage exprimée en pixel </summary> 
    Private Sub AffichageCarte_Paint(sender As Object, e As PaintEventArgs)
        Static Pt0 As Point = Point.Empty
        Dim Selection = If(RegroupementEncours Is Nothing, Rectangle.Empty, RegroupementEncours.SelectionGeoref)
        If EvenementCarteAutorise = True And Pt0 <> Pt0Affichage Then
            'affichage avec un déplacement de la carte
            Pt0 = Pt0Affichage
            AfficherTamponVisue(ZoomAffichage, TailleAffichage, Pt0Affichage, Selection, e.Graphics)
        Else
            'affichage sans déplacement de la carte
            AfficherTamponvisue(TailleAffichage, Pt0Affichage, Selection, e.Graphics)
        End If
    End Sub
    ''' <summary> logique en cas de redimensionnement de la surface d'affichage. Le centre de la carte reste au centre </summary> 
    Private Sub AffichageCarte_Resize(sender As Object, e As EventArgs)
        If EvenementCarteAutorise = True Then
            'calcul de PT0Visualisation pour que le centre de la carte avant le changement de carte reste le centre de la carte après le changement de taille
            Pt0Affichage.X = CentreAffichage.X - CInt(AffichageCarte.ClientSize.Width / 2 / ZoomAffichage)
            Pt0Affichage.Y = CentreAffichage.Y - CInt(AffichageCarte.ClientSize.Height / 2 / ZoomAffichage)
            AffichageCarte.Invalidate()
        End If
        TailleAffichage = AffichageCarte.ClientSize
        VisueRectangle = AffichageCarte.RectangleToScreen(AffichageCarte.ClientRectangle)
    End Sub
    ''' <summary> pas d'utilisation pour l'instant si ce n'est de rendre actif les évenements sur la surface d'affichage. Servira dans l'outil trace </summary> 
    Private Sub AffichageCarte_MouseClick(sender As Object, e As MouseEventArgs)
        AffichageCarte.Select()
    End Sub
    ''' <summary> évenement qui permet d'activer l'outil à utiliser en fonction du modeEncours </summary> 
    Private Sub AffichageCarte_MouseDown(sender As Object, e As MouseEventArgs)
        If EvenementCarteAutorise = True Then
            'on garde en mémoire le fait que la souris est appuyée. Cela permet de déplacer la carte
            FlagSourisDown = True
            Select Case ModeEncours
                Case ModesFCGP.DeplacerCarte
                    OutilEncours = Outils.DeplacementCarteHV
                Case ModesFCGP.CreerTrace
                    OutilEncours = Outils.CreerTrace
                Case ModesFCGP.ModifierTrace
                    OutilEncours = Outils.ModifierTrace
                Case ModesFCGP.ExporterSelectionKMZ
                    OutilEncours = Outils.ExporterSelectionKMZ
                Case ModesFCGP.ImprimerSelection
                    OutilEncours = Outils.ImprimerSelection
            End Select
        End If
    End Sub
    ''' <summary> gere le deplacement de la carte avec le bouton gauche de la souris appuyé. Gére l'affichage des coordonnées de la carte </summary>
    Private Sub AffichageCarte_MouseMove(sender As Object, e As MouseEventArgs)
        'si il y a une carte ou une échelle à l'affichage on peut la dépalcer
        If EvenementCarteAutorise = True Then
            'si le flag souris appuyé est mis c'est que l'on bouge la carte
            If FlagSourisDown = True Then
                'il faut que le curseur soit positionné sur la surface de visualisation de la carte
                If e.X >= 0 And e.X <= TailleAffichage.Width And e.Y >= 0 And e.Y <= TailleAffichage.Height Then
                    Select Case ModeEncours
                        Case ModesFCGP.DeplacerCarte
                            'on calcul les nouvelles coordonnées du Pt(0) ou point d'origine de la visue en prenant en compte le zoom 
                            Pt0Affichage.X += CInt((PosMouse.X - e.X) / ZoomAffichage)
                            Pt0Affichage.Y += CInt((PosMouse.Y - e.Y) / ZoomAffichage)
                            AffichageCarte.Invalidate()
                    End Select
                Else
                    'sinon on considère que la souris n'est plus appuyée et on ne bouge plus la carte
                    FlagSourisDown = False
                    OutilEncours = CType(ModeEncours, Outils)
                End If
            Else
                OutilEncours = CType(ModeEncours, Outils)
            End If
            PosMouse = e.Location
            'affichage à l'écran des coordonnées du pointeur de souris  
            MAJ_ZoneInfoCoordonnees()
        End If
    End Sub
    ''' <summary>  gere la fin du déplacement de la carte avec bouton gauche de la souris appuyé </summary> 
    Private Sub AffichageCarte_MouseUp(sender As Object, e As MouseEventArgs)
        OutilEncours = CType(ModeEncours, Outils)
        FlagSourisDown = False
    End Sub
    ''' <summary> logique de gestion de la molette de la souris : Deplacement vertical, horizontal ou diagonal de la carte et zoom plus ou moins </summary>
    Private Sub AffichageCarte_MouseWheel(sender As Object, e As MouseEventArgs)
        Static Dim MemoireDeltaSouris As Integer
        'si il y a une carte ou une échelle à l'affichage on peut la déplacer ou la zoomer
        If EvenementCarteAutorise = True Then
            'calcul de la différence entre la largeur de la picturebox et de la surface d'affichage
            Dim DeltaX As Integer = (AffichageCarte.Width - TailleAffichage.Width) \ 2
            Dim DeltaY As Integer = (AffichageCarte.Height - TailleAffichage.Height) \ 2
            'si la position de la souris est à l'intérieur de la surface d'affichage, l'évenement concerne la visue, sinon il s'agit d'un autre controle
            If e.X >= AffichageCarte.Left + DeltaX And e.X <= AffichageCarte.Right - DeltaX And e.Y <= AffichageCarte.Bottom - DeltaY And e.Y >= AffichageCarte.Top + DeltaY Then
                Select Case ModifierKeys
                    Case Keys.Shift, Keys.Shift Or Keys.Alt
                        MemoireDeltaSouris = 0
                        If ModifierKeys = (Keys.Shift Or Keys.Alt) Then
                            OutilEncours = Outils.DeplacementCarteVHS
                            Pt0Affichage.Y += If(e.Delta > 0, DeltaDeplacement, -DeltaDeplacement)
                            Pt0Affichage.X += If(e.Delta > 0, DeltaDeplacement, -DeltaDeplacement)
                        Else
                            OutilEncours = Outils.DeplacementCarteV
                            Pt0Affichage.Y += If(e.Delta > 0, DeltaDeplacement, -DeltaDeplacement)
                        End If
                        AffichageCarte.Invalidate()
                    Case Keys.Control, Keys.Control Or Keys.Alt
                        MemoireDeltaSouris = 0
                        If ModifierKeys = (Keys.Control Or Keys.Alt) Then
                            OutilEncours = Outils.DeplacementCarteVSH
                            Pt0Affichage.Y += If(e.Delta > 0, -DeltaDeplacement, +DeltaDeplacement)
                            Pt0Affichage.X += If(e.Delta > 0, DeltaDeplacement, -DeltaDeplacement)
                        Else
                            OutilEncours = Outils.DeplacementCarteH
                            Pt0Affichage.X += If(e.Delta > 0, DeltaDeplacement, -DeltaDeplacement)
                        End If
                        AffichageCarte.Invalidate()
                    Case Keys.Alt
                        OutilEncours = Outils.Zoom
                        'cette section ne déplace pas la carte mais joue sur le coef zoom
                        If e.Delta > 0 Then
                            If MemoireDeltaSouris >= 0 Then
                                MemoireDeltaSouris += 1
                            Else
                                MemoireDeltaSouris = 1
                            End If
                            If MemoireDeltaSouris = 5 Then
                                PosMouse = e.Location
                                Zoom(DonneProchaineValeurZoom(False), True, True)
                                MemoireDeltaSouris = 0
                            End If
                        Else
                            If MemoireDeltaSouris <= 0 Then
                                MemoireDeltaSouris -= 1
                            Else
                                MemoireDeltaSouris = -1
                            End If
                            If MemoireDeltaSouris = -5 Then
                                PosMouse = e.Location
                                Zoom(DonneProchaineValeurZoom(True), True, True)
                                MemoireDeltaSouris = 0
                            End If
                        End If
                    Case Else
                        MemoireDeltaSouris = 0
                        OutilEncours = CType(ModeEncours, Outils)
                End Select
                MAJ_ZoneInfoCoordonnees()
            End If
        End If
    End Sub
    ''' <summary> gère le déplacement de la carte avec les touches clavier de direction </summary>
    Private Sub AffichageCarte_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        'si il y a une carte ou une échelle à l'affichage on peut la dépalcer
        If EvenementCarteAutorise Then
            Dim DX As Integer, DY As Integer
            Select Case e.KeyCode
                Case Keys.Left
                    If ModifierKeys = Keys.Alt Then
                        DY = -DeltaDeplacement
                    End If
                    DX = -DeltaDeplacement
                Case Keys.Right
                    If ModifierKeys = Keys.Alt Then
                        DY = DeltaDeplacement
                    End If
                    DX = DeltaDeplacement
                Case Keys.Up
                    If ModifierKeys = Keys.Alt Then
                        DX = DeltaDeplacement
                    End If
                    DY = -DeltaDeplacement
                Case Keys.Down
                    If ModifierKeys = Keys.Alt Then
                        DX = -DeltaDeplacement
                    End If
                    DY = DeltaDeplacement
                Case Keys.Home
                    If ModifierKeys = Keys.Alt Then
                        DY = -DeltaDeplacementVertical
                    End If
                    DX = -DeltaDeplacementHorizontal
                Case Keys.End
                    If ModifierKeys = Keys.Alt Then
                        DY = DeltaDeplacementVertical
                    End If
                    DX = DeltaDeplacementHorizontal
                Case Keys.PageUp
                    If ModifierKeys = Keys.Alt Then
                        DX = DeltaDeplacementHorizontal
                    End If
                    DY = -DeltaDeplacementVertical
                Case Keys.PageDown
                    If ModifierKeys = Keys.Alt Then
                        DX = -DeltaDeplacementHorizontal
                    End If
                    DY = DeltaDeplacementVertical
            End Select
            Pt0Affichage.X -= DX
            Pt0Affichage.Y -= DY
            AffichageCarte.Invalidate()
            MAJ_ZoneInfoCoordonnees()
        End If
    End Sub
#End Region
#Region "Gestion des menus et barre de statut"
    ''' <summary>  gère la disponibilité des différents menus et boutons </summary> 
    Private Sub GererMenus()
        AffichageCarte.Select()
        EtatMenuGestionRegroupements()
        EtatMenuFichiersTuiles()
        EtatBarreIconeMode()
        EtatBarreIconeZoom()
        EtatBarreIconeListeRegroupements()
        EtatBarreIconeRegroupementEtMenus()
        EtatMenucontextuel()
        MAJ_ZoneInfoRegroupement()
    End Sub
    Private Sub EtatMenucontextuel()
        'si il y a une carte d'affichée on peut mettre le menu contextuel correspondant
        If EvenementCarteAutorise Then
            AffichageCarte.ContextMenuStrip = MenuCxtVisualisationCarte
            'pour la liste des regroupements il faut au moins 2 echelles pour la débloquer
            ContextMenuListeRegroupement.Enabled = Regroupements.Count > 1
        Else
            AffichageCarte.ContextMenuStrip = Nothing
        End If
    End Sub
    Private Sub EtatBarreIconeZoom()
        'pour que la barre soit active il faut une échelle à l'affichage
        BarreIconeZoom.Enabled = EvenementCarteAutorise
    End Sub
    Private Sub EtatBarreIconeMode()
        BarreIconeMode.Enabled = EvenementCarteAutorise OrElse GestionRegroupementAutorise
    End Sub
    Private Sub EtatBarreIconeRegroupementEtMenus()
        IconeAfficherPlanRegroupement.Enabled = EvenementCarteAutorise
        IconeAjouterCartesRegroupement.Enabled = EvenementCarteAutorise
    End Sub
    Private Sub EtatMenuGestionRegroupements()
        MenuSupprimerRegroupement.Enabled = GestionRegroupementAutorise
        MenuAjouterRegroupement.Enabled = EvenementCarteAutorise
    End Sub
    Private Sub EtatMenuFichiersTuiles()
        MenuCreerFichiersJNX_ORUX.Enabled = EvenementCarteAutorise AndAlso FlagSelectionEncours
        MenuCreerFichiersKMZ.Enabled = EvenementCarteAutorise AndAlso FlagSelectionEncours
    End Sub
    Private Sub EtatBarreIconeListeRegroupements()
        BarreIconeListeRegroupement.Enabled = GestionRegroupementAutorise AndAlso ModeEncours = ModesFCGP.DeplacerCarte
        IconeListeRegroupement.Enabled = GestionRegroupementAutorise AndAlso ModeEncours = ModesFCGP.DeplacerCarte
    End Sub
    ''' <summary> Mettre à jour la zone de texte réservée à l'échelle </summary> 
    Private Sub MAJ_ZoneInfoRegroupement()
        If EvenementCarteAutorise Then
            Regroupement.Text = $"{ RegroupementEncours.ClefEchelle }, Nb cartes: { RegroupementEncours.Cartes.Count}"
            ZoneSelection.Text = DimensionsSelection()
        Else
            Regroupement.Text = "Aucun Regroupement"
            ZoneSelection.Text = $"Aucune Sélection [{DimTuile}]"
        End If
        Regroupement.Refresh()
        ZoneSelection.Refresh()
    End Sub
    Private Function DimensionsSelection() As String
        If FlagSelectionEncours Then
            Dim R = RegroupementEncours.SelectionGeoref
            Dim Txd = R.Width / DimTuile
            Dim Tx = CInt(Math.Ceiling(Txd))
            Dim Tyd = R.Height / DimTuile
            Dim Ty = CInt(Math.Ceiling(Tyd))
            Return ($"{Tx} * {Ty} ({Txd:N2} * {Tyd:N2}) [{DimTuile}]")
        End If
        Return $"Aucune Sélection [{DimTuile}]"
    End Function
    ''' <summary> mettre à jour les zones de texte réservée au coordonnées </summary> 
    Private Sub MAJ_ZoneInfoCoordonnees()
        If EvenementCarteAutorise Then
            Select Case RegrouperSettings.INDICE_TYPE_COORDONNEES
                Case 3
                    LabelCoordonnees.Text = CoordMouseUtmWGS84(CoordMouseGrille(CoordMousePixels, _SiteCarto, _Echelle), _Datum)
                Case 2
                    LabelCoordonnees.Text = CoordMouseDMSText(CoordMouseGrille(CoordMousePixels, _SiteCarto, _Echelle), _Datum)
                Case 1
                    LabelCoordonnees.Text = CoordMousseDDText(CoordMouseGrille(CoordMousePixels, _SiteCarto, _Echelle), _Datum)
                Case 0
                    LabelCoordonnees.Text = CoordMouseGrilleText(CoordMouseGrille(CoordMousePixels, _SiteCarto, _Echelle))
                Case 4
                    LabelCoordonnees.Text = CoordMousePointG(CoordMousePixels, _IndiceEchelle).ToString
            End Select
            If AltitudesSettings.IS_ALTITUDE Then
                Altitude.Text = CoordMouseAltitude
            Else
                Altitude.Text = "---- m"
            End If
        Else
            LabelCoordonnees.Text = ""
            Altitude.Text = ""
        End If
        LabelCoordonnees.Refresh()
        Altitude.Refresh()
    End Sub
    ''' <summary> supprime la sélection associé au site encours </summary>
    Private Sub SupprimerSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs)
        EffacerSelectionReferencementSiteCarto(RegroupementEncours.NumSite)
        EtatMenuFichiersTuiles()
    End Sub
    ''' <summary> procedure appelée par programme </summary>
    ''' <param name="Mode"></param> 
    Private Sub SelectionnerMode(Mode As ModesFCGP)
        Dim ts As ToolStripButton
        'on déselectionne l'ensemble des boutons
        For Each ts In BarreIconeMode.Items
            ts.Checked = False
        Next
        CType(BarreIconeMode.Items(0), ToolStripButton).Checked = False
        If Mode <> ModesFCGP.AucunMode Then
            'on selectionne le bouton qui vient d'être cliqué
            ts = CType(BarreIconeMode.Items(Mode - 1), ToolStripButton)
            ts.Checked = True
        End If
        'on indique le mode en cours et l'outil correspondant
        ModeEncours = Mode
        OutilEncours = CType(Mode, Outils)
        'on indique à l'utilisateur
        LModeEncours.Text = Mode.ToString
        LModeEncours.Refresh()
        GererMenus()
    End Sub
    Private Sub IconeDeplacerCarte_Click(sender As Object, e As EventArgs)
        Dim ts As ToolStripButton = CType(sender, ToolStripButton)
        If ts.Checked = False Then
            'on indique le mode en cours et l'outil correspondant
            ModeEncours = ModesFCGP.DeplacerCarte
            OutilEncours = CType(ModeEncours, Outils)
            ts.Checked = True
            GererMenus()
        End If
    End Sub
    ''' <summary>Fermeture du formulaire par le menu</summary>
    Private Sub MenuQuitter_Click(sender As Object, e As EventArgs) 'Handles MenuQuitter.Click
        Close()
    End Sub
#End Region
#Region "Gestion du zoom"
    ''' <summary> quand on click sur le menu Zoom- ou le bouton Zoom-, permet de diminuer la valeur du Zoom d'un cran prédéfini à l'avance
    ''' n'efface pas la valeur du zoom sélectionnée par valeur </summary>
    Private Sub ZoomMoins_Click(sender As Object, e As EventArgs)
        Zoom(DonneProchaineValeurZoom(False))
    End Sub
    ''' <summary> quand on click sur le menu Zoom+ ou le bouton Zoom+  permet d'augmenter la valeur du Zoom d'un cran prédéfini à l'avance
    ''' n'efface pas la valeur du zoom sélectionnée par valeur  </summary>
    Private Sub ZoomPlus_Click(sender As Object, e As EventArgs)
        Zoom(DonneProchaineValeurZoom(True))
    End Sub
    ''' <summary> permet d'afficher la liste deroulante des valeurs du zoom si on click sur l'image du bouton </summary>
    Private Sub IconeZoom_Click(sender As Object, e As EventArgs)
        IconeZoom.ShowDropDown()
    End Sub
    ''' <summary>  quand on click sur un des menus représentant une valeur définie permet de sélectionner une valeur pour le zoom </summary>
    Private Sub ZoomValeur_Click(sender As Object, e As EventArgs)
        Zoom(CDbl(CType(sender, ToolStripMenuItem).Name.Substring(5)) / 1000)
    End Sub
    ''' <summary>  contient la logique de gestion des menus associés au Zoom et le calcul des coordonnées de la fenêtre d'affichage de la carte
    ''' soit par rapport au centre de la carte (liste déroulant icone)  soit par rapport au pointeur (menu ctxt) </summary>
    ''' <param name="ValeurZoom"> valeur du zoom de l'affichage de 0.5 à 2 </param>
    ''' <param name="FlagCoordonneesPointeur"> Est ce que l'on prend en compte la position du curseur lors du dessin de l'affichage </param>
    ''' <param name="FlagRedessineImage"> Est ce que l'on redessine l'affichage </param>
    Private Sub Zoom(ValeurZoom As Double, Optional FlagRedessineImage As Boolean = True, Optional FlagCoordonneesPointeur As Boolean = False)
        If ValeurZoom <> ZoomAffichage Then
            Dim tsmi As ToolStripMenuItem
            'on dé-cheke l'ancienne valeur 
            tsmi = CType(cmsZoomMenu.Items($"Zoom_{(ZoomAffichage * 1000):0000}"), ToolStripMenuItem)
            tsmi.Checked = False
            'on checke la nouvelle valeur
            tsmi = CType(cmsZoomMenu.Items($"Zoom_{(ValeurZoom * 1000):0000}"), ToolStripMenuItem)
            tsmi.Checked = True
            'calcul du point d'origine 
            If FlagCoordonneesPointeur Then
                'par rapport à la position de la souris
                Pt0Affichage.X = CoordMousePixels.X - CInt(PosMouse.X / ValeurZoom)
                Pt0Affichage.Y = CoordMousePixels.Y - CInt(PosMouse.Y / ValeurZoom)
            Else
                'par rapport au centre de la carte
                Pt0Affichage.X = CentreAffichage.X - CInt(TailleAffichage.Width / 2 / ValeurZoom)
                Pt0Affichage.Y = CentreAffichage.Y - CInt(TailleAffichage.Height / 2 / ValeurZoom)
            End If
            'on mémorise le nouveau coef du zoom
            ZoomAffichage = ValeurZoom
            'on redessine la carte
            If FlagRedessineImage Then AffichageCarte.Invalidate()
        End If
        'on met à jour l'info utilisateur
        If EvenementCarteAutorise Then
            LZoom.Text = $"{(ZoomAffichage * 100):000}%"
        Else
            LZoom.Text = ""
        End If
        LZoom.Refresh()
    End Sub
    ''' <summary> renvoie la prochaine valeur du coef zoom en fonction du sens demandé  </summary>
    ''' <param name="FlagPlus"></param>
    Private Function DonneProchaineValeurZoom(Optional FlagPlus As Boolean = False) As Double
        Dim valeur As Double
        If FlagPlus Then
            Select Case ZoomAffichage
                Case 0.5 : valeur = 0.66
                Case 0.66 : valeur = 0.75
                Case 0.75 : valeur = 0.875
                Case 0.875 : valeur = 1.0
                Case 1.0 : valeur = 1.125
                Case 1.125 : valeur = 1.25
                Case 1.25 : valeur = 1.5
                Case 1.5 : valeur = 1.75
                Case 1.75 : valeur = 2
                Case Else
                    valeur = 2.0
            End Select
        Else
            Select Case ZoomAffichage
                Case 0.66 : valeur = 0.5
                Case 0.75 : valeur = 0.66
                Case 0.875 : valeur = 0.75
                Case 1.0 : valeur = 0.875
                Case 1.125 : valeur = 1.0
                Case 1.25 : valeur = 1.125
                Case 1.5 : valeur = 1.25
                Case 1.75 : valeur = 1.5
                Case 2.0 : valeur = 1.75
                Case Else
                    valeur = 0.5
            End Select
        End If
        Return valeur
    End Function
#End Region
#Region "Gestion des Echelles"
    ''' <summary> Création d'un regroupement à partir d'une carte et affichage du regroupement avec le centre de la carte </summary> 
    Private Sub OuvirRegroupement_Click(sender As Object, e As EventArgs)
        If OuvrirFichier.ShowDialog(Me) = DialogResult.OK Then
            Dim SystemeType As SystemeCartographique = Carte.RenvoyerSystemeCartographique(OuvrirFichier.FileName)
            'pour que le systeme soit pris en compte dans une échelle il faut qu'il soit issue d'une capture de site sans interpolation
            If SystemeType.IsOk And SystemeType.IsCapture Then
                Dim ClefSysNew As String = CalculerClefRegroupement(SystemeType.ClefEchelle)
                Dim NbCartes As Integer
                Dim Ret = CreerRegroupement(OuvrirFichier.FileName, ClefSysNew, NbCartes) 'flagcalpinage = true
                If Ret = VerifierRegroupement.OK Then
                    Dim R As Regroupement = RegroupementEncours
                    'on met l'outil Attente car l'opération peut être longue
                    OutilEncours = Outils.Attente
                    'on  sauvegarde la position d'affichage et le coefzoom du regroupement en cours si il y en a un d'affiché
                    If ClefRegroupementEncours IsNot Nothing Then
                        'sauvegarde le coefZoom d'affichage de l'échelle en cours
                        R.CoefZoomAffichageRetour = ZoomAffichage
                        'sauvegarde la position d'affichage de l'échelle en cours
                        R.Affiche_PositionRetour = CentreAffichage
                    End If
                    ClefRegroupementEncours = ClefSysNew
                    R = Regroupements(ClefSysNew)
                    MessageInformation = "Le regroupement " & R.ClefEchelle & " a été créé : " & NbCartes & " cartes ajoutées"
                    'réinitialisation du coeficient du zoom
                    Zoom(1.0, False)
                    'trouver le centre de la carte qui sera aussi le centre de l'affichage et du tampon, c'est le point du centre de la carte qui
                    'a servie à la création de l'échelle + la moitié de la surface d'affichage. Quand le coefzoom =1, monde virtuel = monde physique en pixel
                    Pt0Affichage.X = R.PositionGeoref.X - TailleAffichage.Width \ 2
                    Pt0Affichage.Y = R.PositionGeoref.Y - TailleAffichage.Height \ 2
                    'mettre à jour le tampon avec l'image de la carte
                    ChargerRegroupement(R.PositionGeoref, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage)
                    'demander le mode correspondant
                    SelectionnerMode(ModesFCGP.DeplacerCarte)
                    'il y a au moins une carte à l'affichage donc il faut remplir les différentes combox associées aux échelles
                    RemplirChoixRegroupements()
                    'demander la mise à jour de l'affichage
                    AffichageCarte.Invalidate()
                Else
                    MessageInformation = "Le regroupement n'a pas été créé : " & Ret.ToString
                End If
            Else
                MessageInformation = "Le regroupement n'a pas été créé : " & CrLf &
                                     "Le système Cartographique de la carte" & CrLf &
                                     "n'est pas valide ou il s'agit d'une carte interpolée."
            End If
            AfficherInformation()
            OutilEncours = CType(ModeEncours, Outils)
        End If
    End Sub
    ''' <summary> permet d'ajouter les cartes d'un autre répertoire dans l'échelle courante </summary> 
    Private Sub AjouterCartesRegroupement_Click(sender As Object, e As EventArgs)
        If OuvrirFichier.ShowDialog() = DialogResult.OK Then
            Dim NbCartes As Integer
            Dim R As Regroupement = RegroupementEncours
            Dim FichierGeorefInit As String = OuvrirFichier.FileName
            'on autorise le traitement
            Dim Ret As VerifierRegroupement = R.AjouterCartes(FichierGeorefInit, NbCartes)
            If Ret = VerifierRegroupement.OK Then
                OutilEncours = Outils.Attente
                MessageInformation = NbCartes & " cartes ont ajoutées au regroupement : " & R.ClefEchelle
                'on demande l'initialisation du tampon à partir de la carte qui a été selectionnée
                'le centre de la carte sera aussi le centre de la visue et du tampon. 
                'coefzoom =1, monde virtuel = monde physique en pixel. On le prend en compte pour les cas ou il soit différent
                Pt0Affichage.X = R.PositionGeoref.X - CInt(TailleAffichage.Width / 2 / ZoomAffichage)
                Pt0Affichage.Y = R.PositionGeoref.Y - CInt(TailleAffichage.Height / 2 / ZoomAffichage)
                ChargerRegroupement(R.PositionGeoref, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage)
                OutilEncours = CType(ModeEncours, Outils)
                MAJ_ZoneInfoRegroupement()
                AffichageCarte.Invalidate()
            Else
                MessageInformation = "Aucune carte n'a été ajoutée au regroupement : " & Ret.ToString
            End If
            AfficherInformation()
            OutilEncours = CType(ModeEncours, Outils)
        End If
    End Sub
    ''' <summary> permet d'afficher le plan de l'échelle courante </summary> 
    Private Sub AfficherPlanRegroupement_Click(sender As Object, e As EventArgs)
        Dim R As Regroupement = RegroupementEncours
        'on limite la hauteur du plan de clapinage à 80% de la hauteur du formulaire
        R.AfficherCalepinage(RegionAffichage, CInt(Height * 0.8), 35)
        If R.NumCarteSelection > 0 Then 'si il y a une carte de sélectionnée
            OutilEncours = Outils.Attente
            'on calcule les nouvelles coordonnées du pt(0) de l'écran
            Pt0Affichage.X = R.PositionGeoref.X - CInt(TailleAffichage.Width / 2 / ZoomAffichage)
            Pt0Affichage.Y = R.PositionGeoref.Y - CInt(TailleAffichage.Height / 2 / ZoomAffichage)
            'on demande l'initialisation du tampon à partir du point sélectionné qui sera au centre du tampon
            ChargerRegroupement(R.PositionGeoref, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage)
            OutilEncours = CType(ModeEncours, Outils)
            AffichageCarte.Invalidate()
        End If
        MAJ_ZoneInfoRegroupement()
        GererMenus()
    End Sub
    ''' <summary> gère la logique du changement de regroupement à l'affichage en remettant le coef zoom et la position de la visue 
    ''' quand le regroupement a été enlevée de l'affichage </summary> 
    Private Sub IconeListeEchelle_SelectedIndexChanged(sender As Object, e As EventArgs)
        'si le flag est mis, cela indique qu'il ne s'agit pas d'une MAJ par un click de l'utilisateur, donc on ne prend pas en compte l'évènement
        If Not FlagIgnoreEvenement Then
            Dim R As Regroupement = RegroupementEncours
            'si le choix est différent de l'échelle encours on affiche le choix
            Dim ClefSysNew As String = CalculerClefRegroupement(IconeListeRegroupement.SelectedItem.ToString)
            If ClefSysNew <> ClefRegroupementEncours Then
                OutilEncours = Outils.Attente
                'si il y a une échelle d'affichée on sauvegarde les données d'affichage associée pour pouvoir la réafficher aux mêmes conditions
                If EvenementCarteAutorise Then
                    'sauvegarde le coefZoom d'affichage de l'échelle en cours
                    R.CoefZoomAffichageRetour = ZoomAffichage
                    'sauvegarde la position d'affichage de l'échelle en cours
                    R.Affiche_PositionRetour = CentreAffichage
                End If
                ClefRegroupementEncours = ClefSysNew
                R = Regroupements(ClefSysNew)
                'et on met à jour la 2ème combox
                FlagIgnoreEvenement = True
                ContextMenuListeRegroupement.SelectedItem = R.ClefEchelle
                FlagIgnoreEvenement = False
                'on affiche aux mêmes conditions
                Zoom(R.CoefZoomAffichageRetour, False)
                'mettre  jour les coordonnées de la fenêtre d'affichage
                Pt0Affichage.X = R.Affiche_PositionRetour.X - CInt(TailleAffichage.Width / 2 / ZoomAffichage)
                Pt0Affichage.Y = R.Affiche_PositionRetour.Y - CInt(TailleAffichage.Height / 2 / ZoomAffichage)
                'on met à jour le tampon avec l'image de la carte
                ChargerRegroupement(R.Affiche_PositionRetour, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage)
                'on met à jour les infos de l'échelle qui sont affichées
                GererMenus()
                OutilEncours = CType(ModeEncours, Outils)
                AffichageCarte.Invalidate()
            End If
        End If
        MAJ_TypeCoordonnees()
    End Sub
    ''' <summary> logique associée à la liste déroulante des regroupements du menu contextuel </summary> 
    Private Sub ContextMenuListeRegroupement_SelectedIndexChanged(sender As Object, e As EventArgs)
        'si le flag est mis, cela indique qu'il ne s'agit pas d'une MAJ par un click de l'utilisateur, donc on ne prend pas en compte l'évènement
        If Not FlagIgnoreEvenement Then
            'Si on arrive ici c'est qu'il y a au moins 2 echelles et qu'il y en a une qui est en cours d'affichage
            'on ferme le menu
            MenuCxtVisualisationCarte.Hide()
            Dim ClefSysNew As String = CalculerClefRegroupement(ContextMenuListeRegroupement.SelectedItem.ToString)
            If ClefSysNew <> ClefRegroupementEncours Then
                Dim R As Regroupement = RegroupementEncours
                'on met le curseur d'attente
                AffichageCarte.Cursor = CurseursOutils(Outils.Attente)
                'sauvegarde le coefZoom d'affichage de l'échelle en cours
                R.CoefZoomAffichageRetour = ZoomAffichage
                'sauvegarde la position d'affichage de l'échelle en cours
                R.Affiche_PositionRetour = CentreAffichage
                'on récupère les coordonnées virtuelles du point cliqué 
                Dim CoordonneesClick = CoordMousePixels
                'on les transforme en coordonnées réelles
                Dim PtR As PointD = R.SystemeType.ConvertirCoordonneesVirtuellesToReelles(CoordonneesClick)
                Dim Datum_Old As Datums = R.Datum
                'on met la valeur de zoom à 1
                Zoom(1, False)
                'puis en coordonnées virtuelles de la nouvelle echelle
                ClefRegroupementEncours = ClefSysNew
                R = Regroupements(ClefSysNew)
                If Datum_Old <> R.Datum Then
                    ConvertirCoordonnees(Datum_Old, R.Datum, PtR)
                End If
                'on récupère les coordonnées virtuelles du nouveau site
                CoordonneesClick = R.SystemeType.ConvertirCoordonneesReellesToVirtuelles(PtR)
                'on le recentre par rapport à l'affichage
                Pt0Affichage.X = CoordonneesClick.X - CInt(TailleAffichage.Width / 2 / ZoomAffichage)
                Pt0Affichage.Y = CoordonneesClick.Y - CInt(TailleAffichage.Height / 2 / ZoomAffichage)
                'et on met à jour la 2ème combox
                FlagIgnoreEvenement = True
                IconeListeRegroupement.SelectedItem = R.ClefEchelle
                FlagIgnoreEvenement = False
                'on met à jour le tampon avec l'image de la carte
                ChargerRegroupement(CoordonneesClick, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage)
                GererMenus()
                AffichageCarte.Invalidate()
                AffichageCarte.Cursor = CurseurOutilEncours
            End If
        End If
    End Sub
    ''' <summary> logique associée à la liste déroulante des regroupements du menu principal/ supprimer un regroupement </summary>
    Private Sub SuppressionListeRegroupement_SelectedIndexChanged(sender As Object, e As EventArgs)
        If Not FlagIgnoreEvenement Then
            'pour fermer la liste et le menu
            MenuSupprimerRegroupement.HideDropDown()
            MenuGestionRegroupements.HideDropDown()
            Dim T As ToolStripComboBox = CType(sender, ToolStripComboBox)
            Dim CleRegroupementSupprime As String = CStr(T.SelectedItem)
            Dim ClefSysSupprimee As String = CalculerClefRegroupement(CleRegroupementSupprime)

            MessageInformation = "Etes vous sur de vouloir supprimer" & CrLf &
                                 "le regroupement : " & CleRegroupementSupprime
            If AfficherConfirmation() = DialogResult.OK Then
                'on met le curseur d'attente
                AffichageCarte.Cursor = CurseursOutils(Outils.Attente)
                Dim NumSiteRegroupementSupprime As Integer = Regroupements(ClefSysSupprimee).NumSite
                'on supprime l'échelle de la collection
                SupprimerRegroupement(Regroupements(ClefSysSupprimee))
                'si il n'y a plus d'échelle pour le site on supprime la séléction associée au site
                If NbRegroupementsSiteCarto(NumSiteRegroupementSupprime) = 0 Then EffacerSelectionReferencementSiteCarto(NumSiteRegroupementSupprime)
                'si il n'y a pas d'échelle à afficher, il faut reinitialiser les variables et l'affichage
                If ClefSysSupprimee = ClefRegroupementEncours Or Regroupements.Count = 0 Then
                    ClefRegroupementEncours = Nothing
                    Zoom(1.0, False)
                    Pt0Affichage = Point.Empty
                    ChargerRegroupement(CentreAffichage, Nothing, ZoomAffichage, TailleAffichage, CouleurAffichage)
                    SelectionnerMode(ModesFCGP.AucunMode)
                    AffichageCarte.Invalidate()
                    MAJ_ZoneInfoCoordonnees()
                End If
                'on met à jour les listes déroulantes associées aux échelles
                RemplirChoixRegroupements()
            End If
        End If
    End Sub
    ''' <summary> Remplit les 3 listes déroulantes (Menu, menu contextuel et icone) avec la liste des regroupements en cours et évite le déclenchement</summary>
    Private Sub RemplirChoixRegroupements()
        IconeListeRegroupement.Items.Clear()
        ContextMenuListeRegroupement.Items.Clear()
        SuppressionListeRegroupement.Items.Clear()
        'pour chaque echelle dans la collection
        For Each T As KeyValuePair(Of String, Regroupement) In Regroupements
            IconeListeRegroupement.Items.Add(T.Value.ClefEchelle)
            ContextMenuListeRegroupement.Items.Add(T.Value.ClefEchelle)
            SuppressionListeRegroupement.Items.Add(T.Value.ClefEchelle)
        Next
        'on met à jour les 3 combobox associées mais on fait en sorte de ne pas prendre en compte l'évenement
        If RegroupementEncours IsNot Nothing Then
            'on interdit le traitements des évenements sur les listes de regroupements
            FlagIgnoreEvenement = True
            SuppressionListeRegroupement.SelectedItem = RegroupementEncours.ClefEchelle
            IconeListeRegroupement.SelectedItem = RegroupementEncours.ClefEchelle
            ContextMenuListeRegroupement.SelectedItem = RegroupementEncours.ClefEchelle
            'on autorise le traitements des évenements sur les listes de regroupements
            FlagIgnoreEvenement = False
        End If
        GererMenus()
    End Sub
#End Region
#Region "Gestion des autres actions"
    ''' <summary> lance l'ouverture du formulaire liés aux informations générales de l'application </summary>
    Private Sub MenuAPropos_Click(sender As Object, e As EventArgs)
        OuvrirAPropos()
    End Sub
    ''' <summary> lance l'ouverture du formulaire associé à l'aide de l'application </summary>
    Private Sub MenuAideRegrouper_Click(sender As Object, e As EventArgs)
        AfficherAide(Me)
    End Sub
    ''' <summary> lance l'ouverture du formulaire associé à la configuration de l'application </summary>
    Private Sub MenuConfigurationRegrouper_Click(sender As Object, e As EventArgs)
        OuvrirConfiguration()
    End Sub
    ''' <summary> lance l'ouverture du formulaire associé à la création des fichiers ORUX et ou JNX </summary>
    Private Sub MenuCreerFichiersJNX_ORUX_Click(sender As Object, e As EventArgs)
        'sauvegarde le coefZoom d'affichage de l'échelle en cours
        RegroupementEncours.CoefZoomAffichageRetour = ZoomAffichage
        'sauvegarde la position d'affichage de l'échelle en cours
        RegroupementEncours.Affiche_PositionRetour = CentreAffichage
        Using R As Form = New FichiersTuilesJnxOrux With {.RegroupementEncours = RegroupementEncours}
            R.ShowDialog(Me)
        End Using
        AffichageCarte.Invalidate()
        ListeChoixDimensions.SelectedIndex = 0
        MAJ_ZoneInfoRegroupement()
    End Sub
    ''' <summary> lance l'ouverture du formulaire associé à la création des fichiers KMZ </summary>
    Private Sub MenuCreerFichiersKMZ_Click(sender As Object, e As EventArgs)
        'sauvegarde le coefZoom d'affichage de l'échelle en cours
        RegroupementEncours.CoefZoomAffichageRetour = ZoomAffichage
        'sauvegarde la position d'affichage de l'échelle en cours
        RegroupementEncours.Affiche_PositionRetour = CentreAffichage
        Using R As Form = New FichierTuilesKMZ With {.RegroupementEncours = RegroupementEncours}
            R.ShowDialog(Me)
        End Using
        AffichageCarte.Invalidate()
        ListeChoixDimensions.SelectedIndex = 1
        MAJ_ZoneInfoRegroupement()
    End Sub
    ''' <summary> lance l'ouverture du formulaire associé à la modification des fichiers KMZ </summary>
    Private Sub MenuModifierFichierKMZ_Click(sender As Object, e As EventArgs)
        Using R As Form = New AffichageKMZ
            R.ShowDialog(Me)
        End Using
    End Sub
    ''' <summary> ouvre le formulaire liés aux indices d'affichage des fichiers JNX </summary>
    Private Sub MenuModifierFichiersJNX_Click(sender As Object, e As EventArgs) 'Handles FichiersJNXToolStripMenuItem.Click
        Using R As Form = New AffichageJNX
            R.ShowDialog(Me)
        End Using
    End Sub
    ''' <summary> ouvre le formulaire liés aux indices d'affichage des fichiers ORUX </summary>
    Private Sub MenuModifierFichiersORUX_Click(sender As Object, e As EventArgs) 'Handles FichiersORUXToolStripMenuItem.Click
        Using R As Form = New AffichageORUX
            R.ShowDialog(Me)
        End Using
    End Sub
#End Region
#Region "Procédures et fonction générales"
    Friend Sub AfficherInfomationsErreurs()
        AfficherInformation()
    End Sub
    ''' <summary> initialise les différents curseurs dont on a besoin que ceux ci soit déja présents dans le formulaire ou pas </summary>
    Private Sub InitialiserCurseursVisue()
        InitialiserCurseurs(AffichageCarte, Cursor)
        CurseursOutils(Outils.AucunOutil) = Curseur(Curseurs.Arrow)
        CurseursOutils(Outils.DeplacerCarte) = Curseur(Curseurs.CarteDefaut)
        CurseursOutils(Outils.CreerTrace) = Curseur(Curseurs.TraceDefaut)
        CurseursOutils(Outils.ModifierTrace) = Curseur(Curseurs.TraceDeplacePt)
        CurseursOutils(Outils.ExporterSelectionKMZ) = Curseur(Curseurs.Arrow)
        CurseursOutils(Outils.ImprimerSelection) = Curseur(Curseurs.Arrow)
        CurseursOutils(Outils.DeplacementCarteHV) = Curseur(Curseurs.CarteDeplacement)
        CurseursOutils(Outils.DeplacementPointTrace) = Curseur(Curseurs.TraceDeplacePt)
        CurseursOutils(Outils.InsertionPointTrace) = Curseur(Curseurs.TraceInsertPt)
        CurseursOutils(Outils.SuppressionPointTrace) = Curseur(Curseurs.TraceSupprimePtSeg)
        CurseursOutils(Outils.Zoom) = Curseur(Curseurs.Zoom)
        CurseursOutils(Outils.KMZ) = Curseur(Curseurs.Arrow)
        CurseursOutils(Outils.Attente) = Curseur(Curseurs.Attendre)
        CurseursOutils(Outils.DeplacementCarteH) = Curseur(Curseurs.CarteDeplacementVertical)
        CurseursOutils(Outils.DeplacementCarteV) = Curseur(Curseurs.CarteDeplacementHorizontal)
        CurseursOutils(Outils.DeplacementCarteVHS) = Curseur(Curseurs.CarteDeplacementVHS)
        CurseursOutils(Outils.DeplacementCarteVSH) = Curseur(Curseurs.CarteDeplacementVSH)
    End Sub
    ''' <summary> met à jour les principales variables globales du formulaire</summary>
    ''' <param name="FlagInitSupportVisue"> flag indiquant si il faut initialiser les variables du support visue </param>
    Private Sub InitialiserAffichageCarte(FlagInitSupportVisue As Boolean)
        DeltaDeplacementPixel = RegrouperSettings.PAS_DEPLACEMENT
        CouleurAffichage = PartagerSettings.COULEUR_VISUE
        If FlagInitSupportVisue Then
            InitialiserSupportVisue(MaxBoundsEcran, CouleurAffichage)
        Else
            ChangerCouleurVisue(CouleurAffichage)
        End If
        AffichageCarte.Invalidate()
    End Sub
    ''' <summary>  gère le redimensionnement de l'écran et la taille de l'affichage </summary>
    ''' <param name="FlagMaximize"> indique si le formulaire doit être maximisé </param>
    Private Sub InitialiserRegionAffichageCarte(FlagMaximize As Boolean)
        WindowState = FormWindowState.Normal
        'dimensions du formulaire environ 80% des dimensions de l'écran
        Bounds = Rectangle.Inflate(DimensionsEcranSupport, CInt(DimensionsEcranSupport.Width * -0.1), CInt(DimensionsEcranSupport.Height * -0.1))
        'a l'ouverture, l'application est maximisée sur l'écran de travail définit par le settings
        If FlagMaximize Then WindowState = FormWindowState.Maximized
    End Sub
    ''' <summary> renvoie une chaine unique basée sur la clefEchelle d'un système cartographique. Permet un tri par ordre croissant ou décroissant  </summary>
    ''' <param name="ClefEchelle"></param>
    Private Shared Function CalculerClefRegroupement(ClefEchelle As String) As String
        Dim s() As String = ClefEchelle.Split("-"c)
        If s.Length = 3 Then
            s(0) &= "-" & s(1) & If(s(2).Length > 3, "-", "-0") & s(2)
        Else
            s(0) &= If(s(1).Length > 3, "-", "-0") & s(1)
        End If
        Return s(0)
    End Function
    ''' <summary> sert pour passer des coordonnées réelles d'une échelle en projection aux coordonnées réelles d'une échelle en LL et vice versa </summary>
    ''' <param name="ProjOld">projection en cours du point Ptr</param>
    ''' <param name="ProjNew">projection future du point Ptr</param>
    ''' <param name="Ptr">point en coordonnées réelles à transformer d'une projection à une autre</param>
    Private Shared Sub ConvertirCoordonnees(ProjOld As Datums, ProjNew As Datums, ByRef Ptr As PointD)
        If ProjOld = Datums.WGS84 Then
            Ptr = ConvertWGS84ToProjection(Ptr, ProjNew).Coordonnees
        Else
            Ptr = ConvertProjectionToWGS84(ProjOld, New PointProjection(Ptr))
        End If
    End Sub
    ''' <summary> ouvre le formulaire associé à la configuration de l'application </summary>
    Private Sub OuvrirConfiguration()
        Dim NumEcran As Integer = PartagerSettings.NUM_ECRAN
        'on demande à l'utilisateur le site sur lequel il va travailler et sur quel écran
        Using R As Form = New Configuration
            R.ShowDialog(Me)
            If R.DialogResult = DialogResult.OK Then
                InitialiserAffichageCarte(False)
                'si on change d'écran 
                If NumEcran <> PartagerSettings.NUM_ECRAN Then
                    DimensionsEcranSupport = Screen.AllScreens(PartagerSettings.NUM_ECRAN).Bounds
                    InitialiserRegionAffichageCarte(WindowState = FormWindowState.Maximized)
                End If
            End If
        End Using
    End Sub
    ''' <summary> ouvre le formulaire lié aux informations générales de l'application </summary>
    Private Sub OuvrirAPropos()
        MessageInformation = NumFCGP & CrLf & "en date du " & DateVersionFCGP & CrLf &
            "Faire des Cartes à partir de GéoPortail (Regrouper)" & CrLf &
            "Contact : " & Contact & CrLf &
            "Dimensions Ecran : " & DimensionsEcranSupport.Width & "*" & DimensionsEcranSupport.Height & CrLf &
            "Memoire dispo : " & (GetTotalMemory / 1024 ^ 3).ToString("0.00") & " Go, Nb Processeurs : " & Environment.ProcessorCount
        AfficherInformationEtLien(Me, "Visitez le site FCGP", "https://fcgp.e-monsite.com")
    End Sub
    ''' <summary> affiche un des coins de la sélection au centre de l'affichage </summary>
    ''' <param name="Coin"> numéro du coin de la sélection que l'on veut voir afficher au centre de l'affichage </param>
    Private Sub AfficherCoinSelection(Coin As Keys)
        Dim Curseur = Cursor
        Cursor = Cursors.WaitCursor
        Dim R = RegroupementEncours
        Dim Selection = R.SelectionGeoref

        'on récupère les coordonnées virtuelles du nouveau site
        Dim CoordonneesCoin As Point
        Select Case Coin
            Case Keys.NumPad0, Keys.D0
                CoordonneesCoin = New Point(Selection.Left, Selection.Top)
            Case Keys.NumPad1, Keys.D1
                CoordonneesCoin = New Point(Selection.Right, Selection.Top)
            Case Keys.NumPad2, Keys.D2
                CoordonneesCoin = New Point(Selection.Right, Selection.Bottom)
            Case Keys.NumPad3, Keys.D3
                CoordonneesCoin = New Point(Selection.Left, Selection.Bottom)
        End Select
        'on le recentre par rapport à l'affichage
        Pt0Affichage.X = CoordonneesCoin.X - CInt(TailleAffichage.Width / 2 / ZoomAffichage)
        Pt0Affichage.Y = CoordonneesCoin.Y - CInt(TailleAffichage.Height / 2 / ZoomAffichage)
        'on met à jour le tampon avec l'image de la carte
        ChargerRegroupement(CoordonneesCoin, R.Cartes, ZoomAffichage, TailleAffichage, CouleurAffichage)
        AffichageCarte.Invalidate()
        'on positionne le curseur au centre de l'affichage soit le coin choisi
        Cursor.Position = AffichageCarte.PointToScreen(New Point(TailleAffichage.Width \ 2, TailleAffichage.Height \ 2))
        MAJ_ZoneInfoCoordonnees()
        Cursor = Curseur
    End Sub
    Private Sub ModifierSelection(Touche As Keys)
        Dim Selection = RegroupementEncours.SelectionGeoref
        Dim Value As Integer
        Select Case Touche
            Case Keys.X
                Value = Selection.X
                Select Case ModifierKeys
                    Case Keys.Shift 'décale en ajoutant le nb de pixels d'une tuile du serveur
                        Selection.X = Value + NbPixelsTuile
                    Case Keys.Control 'décale en enlèvant le nb de pixels d'une tuile du serveur
                        Selection.X = Value - NbPixelsTuile
                    Case Keys.Alt 'arrondi au pixel de début de la tuile serveur la plus proche. Permet de démarrer sur une valeur connue
                        Selection.X = CInt(Math.Round(Value / NbPixelsTuile)) * NbPixelsTuile
                End Select
            Case Keys.Y
                Value = Selection.Y
                Select Case ModifierKeys
                    Case Keys.Shift 'décale en ajoutant le nb de pixels d'une tuile du serveur
                        Selection.Y = Value + NbPixelsTuile
                    Case Keys.Control 'décale en enlèvant le nb de pixels d'une tuile du serveur
                        Selection.Y = Value - NbPixelsTuile
                    Case Keys.Alt 'arrondi au pixel de début de la tuile serveur la plus proche. Permet de démarrer sur une valeur connue
                        Selection.Y = CInt(Math.Round(Value / NbPixelsTuile)) * NbPixelsTuile
                End Select
            Case Keys.L
                Value = Selection.Width
                Select Case ModifierKeys
                    Case Keys.Shift ' arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                        Selection.Width = (Value \ DimTuile + 1) * DimTuile
                    Case Keys.Control ' arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                        Selection.Width = (Value \ DimTuile - 1) * DimTuile
                    Case Keys.Alt 'arrondi le nb de pixels aux dimensions du nb de tuiles du fichier tuile le plus proche. Permet de démarrer sur une valeur connue
                        Selection.Width = CInt(Math.Round(Value / DimTuile)) * DimTuile
                End Select
            Case Keys.H
                Value = Selection.Height
                Select Case ModifierKeys
                    Case Keys.Shift ' arrondi inférieur à la dimension de la tuile du fichier tuile et ajoute une tuile
                        Selection.Height = (Value \ DimTuile + 1) * DimTuile
                    Case Keys.Control ' arrondi inférieur à la dimension de la tuile du fichier tuile et enleve une tuile
                        Selection.Height = (Value \ DimTuile - 1) * DimTuile
                    Case Keys.Alt 'arrondi le nb de pixels aux dimensions du nb de tuiles du fichier tuile le plus proche. Permet de démarrer sur une valeur connue
                        Selection.Height = CInt(Math.Round(Value / DimTuile)) * DimTuile
                End Select
        End Select
        RegroupementEncours.SelectionGeorefToSelectionReferencement(Selection)
        AffichageCarte.Invalidate()
        MAJ_ZoneInfoRegroupement()
    End Sub
#End Region
#Region "Listes de Choix"
    ''' <summary> ouvre la liste de choix associée à un LabelListe </summary>
    Private Sub LabelListes_Click(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        Dim C = CType(L.Tag, ComboBox)
        If RegroupementEncours IsNot Nothing Then C.DroppedDown = True
    End Sub
    ''' <summary> change les couleurs d'un Label avec menu à l'entrée de la souris sur la surface d'un label avec menu </summary>
    Private Sub LabelListesMenus_MouseEnter(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        L.BackColor = Color.FromArgb(255, 229, 241, 251)
        L.Refresh()
    End Sub
    ''' <summary> change les couleurs d'un Label avec menu à la sortie de la souris sur sa surface </summary>
    Private Sub LabelListesMenus_MouseLeave(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        L.BackColor = Color.Transparent
        LabelCoordonnees.Refresh()
    End Sub
    ''' <summary> change l'affichage des coordonnées du pointeur de souris </summary>
    Private Sub ListeChoixTypeCoordonnees_SelectedIndexChanged(sender As Object, e As EventArgs)
        RegrouperSettings.INDICE_TYPE_COORDONNEES = ListeChoixTypeCoordonnees.SelectedIndex
        MAJ_ZoneInfoCoordonnees()
        AffichageCarte.Select()
    End Sub
    ''' <summary> change l'affichage des coordonnées du pointeur de souris </summary>
    Private Sub ListeChoixDimensions_SelectedIndexChanged(sender As Object, e As EventArgs)
        RegrouperSettings.TYPE_DIMENSIONS = ListeChoixDimensions.SelectedIndex
        LabelDimensions.Text = ListeChoixDimensions.SelectedItem.ToString
        MAJ_ZoneInfoRegroupement()
        AffichageCarte.Select()
    End Sub
    ''' <summary> mise à jour de la liste déroulante des types de coordonnées du pointeur de souris </summary>
    Private Sub MAJ_TypeCoordonnees()
        Dim SystemeType = RegroupementEncours.SystemeType
        _SiteCarto = SystemeType.SiteCarto
        _Datum = DatumSiteWeb(_SiteCarto)
        _Echelle = SystemeType.Niveau.Echelle
        _IndiceEchelle = NiveauDetailCartographique.EchelleToSiteIndiceEchelle(_SiteCarto, _Echelle)
        ListeChoixTypeCoordonnees.Items.Clear()
        ListeChoixTypeCoordonnees.Items.Add(DatumsLibelles(_Datum))
        ListeChoixTypeCoordonnees.Items.AddRange(CoordType)
        Dim Couche As Integer = EchelleToCouche(_Echelle)
        ListeChoixTypeCoordonnees.Items.Add("TUILE Couche N°" & Couche)
        ListeChoixTypeCoordonnees.SelectedIndex = RegrouperSettings.INDICE_TYPE_COORDONNEES
        PosMouse = PixelsSiteToPixelsEcran(CentreAffichage)
    End Sub
#End Region
End Class