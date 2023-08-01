Friend Module SupportVisue
    ''' <summary> brosse semi transparente pour le dessin de la zone de sélection </summary>
    Private ReadOnly Brosse_Selection As New SolidBrush(Color.FromArgb(48, 64, 64, 64))
    ''' <summary>indique si l'etat du déplacement du tampon d'affichage est en cours</summary>
    Private EtatDecalage As EtatsTampon
    ''' <summary>tache effectuant le déplacement du tampon d'affichage </summary>
    Private TacheDecalage As Task
    ''' <summary> tampon servant pour l'initialisation du tampon avec une couleur </summary>
    Private CouleurVisue As Color
    ''' <summary> surface de l'écran sur lequel s'éxécute FCGP exprimée en pixels </summary>
    Private SurfaceEcran As Size
    ''' <summary> surface de la partie horizontale à remplir sur à la demande de décalage exprimée en monde virtuel </summary>
    Private GV_Horizontal As Rectangle
    ''' <summary> surface de la partie verticale à remplir sur à la demande de décalage exprimée en monde virtuel </summary>
    Private GV_Vertical As Rectangle
    ''' <summary>definit la zone au delà de laquelle il faut demander le remplissage du tampon futur, depend du coefzoom</summary>
    Private ReadOnly Property GeorefRemplissage As Rectangle
        Get
            Return Rectangle.Inflate(GeorefEncours, CInt(-SurfaceEcran.Width * CoefGeorefRemplissage), CInt(-SurfaceEcran.Height * CoefGeorefRemplissage))
        End Get
    End Property
    ''' <summary>definit la région du tampon en cours exprimée en unité du monde virtuel</summary>
    Private ReadOnly Property GeorefEncours As Rectangle
        Get
            Return Tampons(Encours).Georef
        End Get
    End Property
    ''' <summary>definit le pt0 de la region du tampon encours exprimée en unité du monde virtuel</summary>
    Private ReadOnly Property Pt0Encours As Point
        Get
            Return Tampons(Encours).Pt0
        End Get
    End Property
    ''' <summary> surface de la region représentée par les tampons, encours et futur, exprimée en unité du monde virtuel </summary>
    Private ReadOnly Property SurfaceGeorefEncours As Size
        Get
            Return Tampons(Encours).Surface
        End Get
    End Property
    ''' <summary> nb d'octects d'une ligne du tampon </summary>
    Private ReadOnly Property StrideEncours As Integer
        Get
            Return Tampons(Encours).Stride
        End Get
    End Property
    ''' <summary> nb de lignes de pixels  des tampons encours et futur </summary>
    Private ReadOnly Property NbLignesEncours As Integer
        Get
            Return Tampons(Encours).NbLignes
        End Get
    End Property
    ''' <summary>definit le pt0 de la region du tampon encours exprimée en unité du monde virtuel</summary>
    Private ReadOnly Property ImageEncours As Bitmap
        Get
            Return Tampons(Encours).Image
        End Get
    End Property
    ''' <summary> tableau qui contient les 2 tampons d'affichage </summary>
    Private Tampons() As TamponVisue
    ''' <summary> indice du tamponsupportvisue qui est à l'affichage</summary>
    Private Encours As Integer
    ''' <summary> indice du TamponSupportVisue qui sera à l'affichage après permutation avec le tampon encours</summary>
    Private Futur As Integer
    ''' <summary>Etat du tampon d'affichage</summary>
    Private Enum EtatsTampon As Integer
        ''' <summary>Après MAJ de l'origine du tampon, la visue est à l'interieur de la zone de demande de remplissage et de décalage donc pas d'action</summary>
        Aucun = 0
        ''' <summary>demande de calcul du GeorefDécalage et de remplissage des tampons auxillaires</summary>
        DemandeDecalage = 1
        ''' <summary>la demande de décalage (copie des 3 tampon) a été traitée</summary>
        DemandeDecalageFinie = 2
    End Enum
    ''' <summary>coefZoom de la visue qui sert de coef multiplicateur au niveau de l'affichage</summary>
    Private CoefZoomAffichage As Double
    ''' <summary>coef multiplicateur au niveau de la surfarce de demande de remplissage</summary>
    Private CoefGeorefRemplissage As Double
    ''' <summary>coef multiplicateur au niveau des tampons horizontal et vertical</summary>
    Private CoefDimensions As Double
    ''' <summary> surface de la visue exprimée en pixel </summary>
    Private SurfaceAffichage As Size
    ''' <summary> surface de la visue exprimée en pixel zoom, le zoom etant le rapport  entre affichage et visueZoom</summary>
    Private VisueZoom As Size
    ''' <summary>Largeur du tampon vertical, depend du coefzoom</summary>
    Private NbPixelsX As Integer
    ''' <summary>Hauteur du tampon horizontal du coefzoom</summary>
    Private NbPixelsY As Integer
    ''' <summary> cette procédure calcule pour les 2 tampons auxiliaires la surface, en coordonnées du monde virtuel de l'échelle encours,
    '''  qu'il faut remplir en fonction du ou des  déplacements demandés</summary>
    ''' <param name="GeorefVisue">surface de l'affichage en coordonnées du monde virtuel</param>
    ''' <remarks> en sortie soit GeoRef futur est égal à Geroef encours et cela indique qu'il n'y a pas de déplacement demandé
    ''' soit le GeorRef futur n'est pas égal à GeroefVirtuel et cela indique la surface ou les surfaces qu'il faudra remplir avant de réaliser 
    ''' la permutation des tampons</remarks>
    Private Function CalculerDeplacement(GeorefVisue As Rectangle) As Integer
        Dim NbDeplacements As Integer
        'cherche le ou les décalages
        GV_Vertical = Rectangle.Empty
        GV_Horizontal = Rectangle.Empty
        Tampons(Futur).Pt0 = Pt0Encours
        '1 si deplacement Est, -1 si déplacement  Ouest et 0 si pas de déplacement
        Dim DecalX As Integer = If(GeorefVisue.Right > GeorefRemplissage.Right, 1, If(GeorefVisue.Left < GeorefRemplissage.Left, -1, 0))
        '1 si deplacement Sud, -1 si déplacement  Nord et 0 si pas de déplacement
        Dim DecalY As Integer = If(GeorefVisue.Bottom > GeorefRemplissage.Bottom, 1, If(GeorefVisue.Top < GeorefRemplissage.Top, -1, 0))
        If DecalX <> 0 OrElse DecalY <> 0 Then  'il y a au moins un déplacement de décalage
            'mise à jour du Pt0 du tampon futur
            Tampons(Futur).Pt0 = New Point(Pt0Encours.X + NbPixelsX * DecalX, Pt0Encours.Y + NbPixelsY * DecalY)
            'mise à jour du tampon horizontal si il y a un déplacement dans ce sens
            If DecalX <> 0 Then
                NbDeplacements += 1
                GV_Horizontal.X = If(DecalX > 0, GeorefEncours.Right, GeorefEncours.Left - NbPixelsX)
                GV_Horizontal.Y = GeorefEncours.Top + If(DecalY > 0, NbPixelsY, 0)
                GV_Horizontal.Size = New Size(NbPixelsX, GeorefEncours.Height - If(DecalY <> 0, NbPixelsY, 0))
            End If
            'mise à jour du tampon vertical si il y a un déplacement dans ce sens
            If DecalY <> 0 Then
                NbDeplacements += 1
                GV_Vertical.X = GeorefEncours.Left + DecalX * NbPixelsX
                GV_Vertical.Y = If(DecalY > 0, GeorefEncours.Bottom, GeorefEncours.Top - NbPixelsY)
                GV_Vertical.Size = New Size(GeorefEncours.Width, NbPixelsY)
            End If
        End If
        Return NbDeplacements
    End Function
    ''' <summary> remplit les 3 parties du tampon futur</summary>
    Private Sub DecalerTampon(NbDeplacements As Integer)
        'dimensionne le tableau des taches à lancer de manière asynchrone
        Dim T(NbDeplacements) As Task
        Dim NbTaches As Integer
        If GV_Horizontal <> Rectangle.Empty Then
            T(NbTaches) = Task.Factory.StartNew(Sub() Tampons(Futur).RemplirTampon(GV_Horizontal, CouleurVisue))
            NbTaches += 1
        End If
        If GV_Vertical <> Rectangle.Empty Then
            T(NbTaches) = Task.Factory.StartNew(Sub() Tampons(Futur).RemplirTampon(GV_Vertical, CouleurVisue))
            NbTaches += 1
        End If
        T(NbTaches) = Task.Factory.StartNew(Sub() CopierTampon())
        Task.WaitAll(T)
    End Sub
    ''' <summary> demande le decalage si besoin du tampon futur </summary>
    ''' <param name="Visue"></param>
    Private Sub DemanderDecalage(Visue As Rectangle)
        EtatDecalage = EtatsTampon.DemandeDecalage
        Dim NbDeplacements As Integer = CalculerDeplacement(Visue)
        If NbDeplacements > 0 Then
            DecalerTampon(NbDeplacements)
            PermutterTampon()
        End If
        EtatDecalage = EtatsTampon.Aucun
    End Sub
    ''' <summary> permute entre les 2 tampons si besoin. cette action finie la demande de décalage</summary>
    Private Sub PermutterTampon()
        Dim Tempo As Integer = Futur
        Futur = Encours
        Encours = Tempo
        EtatDecalage = EtatsTampon.Aucun
    End Sub
    ''' <summary> calcule certaines variables de la visue qui dépendent du CoefZoom de la visue ou de la surface de la visue</summary>
    ''' <param name="CoefZoom">coeficient zoom de la visue de 2.0 à 0.5</param>
    ''' <param name="VisueAffichage"> surface de la visue exprimée en pixels</param>
    Private Sub CalculerVariablesSupportVisue(CoefZoom As Double, VisueAffichage As Size)
        If CoefZoomAffichage <> CoefZoom Then
            CoefZoomAffichage = CoefZoom
            CoefGeorefRemplissage = 2 - If(CoefZoom > 1, 0, 1 - CoefZoom)
            CoefDimensions = 1 + If(CoefZoom >= 1, 0, (1 - CoefZoom) * 2)
            NbPixelsX = CInt(SurfaceEcran.Width * CoefDimensions)
            NbPixelsY = CInt(SurfaceEcran.Height * CoefDimensions)
            VisueZoom = New Size(CInt(VisueAffichage.Width / CoefZoom), CInt(VisueAffichage.Height / CoefZoom))
        End If
        If SurfaceAffichage <> VisueAffichage Then
            SurfaceAffichage = VisueAffichage
            VisueZoom = New Size(CInt(VisueAffichage.Width / CoefZoom), CInt(VisueAffichage.Height / CoefZoom))
        End If
    End Sub
    ''' <summary> copie du tampon encours vers le tampon futur de la partie non modifiée suite au décalage</summary>
    Private Sub CopierTampon()
        Dim DecalX As Integer = (Tampons(Futur).Pt0.X - Pt0Encours.X) * NbOctetsPixel
        Dim DecalY As Integer = (Tampons(Futur).Pt0.Y - Pt0Encours.Y) * StrideEncours
        Dim NbOctectsTransfert As Integer = StrideEncours - Math.Abs(DecalX)
        Dim IndexSource As IntPtr = Tampons(Encours).BitPtr + If(DecalY > 0, DecalY, 0) + If(DecalX > 0, DecalX, 0)
        Dim IndexDestination As IntPtr = Tampons(Futur).BitPtr + If(DecalY < 0, -DecalY, 0) + If(DecalX < 0, -DecalX, 0)
        Parallel.For(0, NbLignesEncours - Math.Abs(Tampons(Futur).Pt0.Y - Pt0Encours.Y),
                         Sub(Cpt As Integer)
                             Dim IndexEncours As IntPtr = IndexSource + Cpt * StrideEncours
                             Dim IndexFutur As IntPtr = IndexDestination + Cpt * StrideEncours
                             CopierMemoire(IndexFutur, IndexEncours, NbOctectsTransfert)
                         End Sub)
    End Sub
    ''' <summary> dimensionne les tampons en fonction de la surface d'ecran sur lequel FCGP tourne. Remplace le constructeur d'une classe </summary>
    ''' <param name="SurfEcran"> surface de l'écran</param>
    ''' <param name="CouleurFond">couleur de la visue quand il n'y a pas de carte</param>
    Friend Sub InitialiserSupportVisue(SurfEcran As Size, CouleurVisue As Color)
        'il n'y a pas de vérification sur la taille de réservebase ce qui peut poser des problèmes sur les écrans de dimensions > 1920*1200
        ReDim Tampons(1)
        SurfaceEcran = SurfEcran
        Dim Georef As New Size(SurfaceEcran.Width * 6, SurfaceEcran.Height * 6)
        Tampons(0) = New TamponVisue(Georef, "Visue_1") With {.Pt0 = Point.Empty}
        Tampons(1) = New TamponVisue(Georef, "Visue_2") With {.Pt0 = Point.Empty}
        ChangerCouleurVisue(CouleurVisue)
        CalculerVariablesSupportVisue(1, SurfEcran)
    End Sub
    ''' <summary>libère l'espace memoire des tampons si besoin</summary>
    Friend Sub CloturerVisue()
        Tampons(0).Dispose()
        Tampons(1).Dispose()
    End Sub
    ''' <summary> affichage la carte sur la visue en fonction des 3 paramètres. appel par la méthode paint de la visue si il y a un déplacement demandé</summary>
    ''' <param name="CoefZoom">coeficient zoom de la visue de 2.0 à 0.5</param>
    ''' <param name="VisueAffichage"> surface de la visue exprimée en pixels</param>
    ''' <param name="VisuePtOrigine">Pt0 de la visue exprimée en corrdonnéées virtuel</param>
    ''' <param name="e"></param>
    Friend Sub AfficherTamponVisue(CoefZoom As Double, VisueAffichage As Size, VisuePtOrigine As Point, Selection As Rectangle, e As Graphics)
        CalculerVariablesSupportVisue(CoefZoom, VisueAffichage)
        'calcul du georef de la visue
        Dim GeorefVisue As New Rectangle(VisuePtOrigine, VisueZoom)
        If Not GeorefEncours.Contains(GeorefVisue) Then
            Try
                If EtatDecalage = EtatsTampon.Aucun Then
                    DemanderDecalage(GeorefVisue)
                Else 'If EtatDecalage = EtatsTampon.DemandeDecalage Then
                    TacheDecalage.Wait()
                End If
            Catch Ex As Exception
                AfficherErreur(Ex, "L1P4")
            End Try
        End If
        AfficherTamponvisue(VisueAffichage, VisuePtOrigine, Selection, e)
        'on ne peut lancer qu'une tache
        If Not GeorefRemplissage.Contains(GeorefVisue) And EtatDecalage = EtatsTampon.Aucun Then
            'on calcule le ou les déplacements et on demande le ou les remplissage de tampon
            TacheDecalage = Task.Factory.StartNew(Sub() DemanderDecalage(GeorefVisue), TaskCreationOptions.PreferFairness)
        End If
    End Sub
    ''' <summary> affichage la carte sur la visue en fonction des 2 paramètres.  appel par la méthode paint de la visue si il n'y a pas de déplacement demandé </summary>
    ''' <param name="VisueAffichage"> surface de la visue exprimée en pixels</param>
    ''' <param name="VisuePtOrigine">Pt0 de la visue exprimée en corrdonnéées virtuel</param>
    ''' <param name="e"></param>
    Friend Sub AfficherTamponvisue(VisueAffichage As Size, VisuePtOrigine As Point, Selection As Rectangle, e As Graphics)
        'calcul du décalage en pixel de l'image du tampon par rapport à la surface d'affichage
        Dim DecalageTamponX As Integer = VisuePtOrigine.X - Pt0Encours.X
        Dim DecalageTamponY As Integer = VisuePtOrigine.Y - Pt0Encours.Y
        'La dimension de la visue reste toujours identique, on joue sur la largeur et la hauteur de l'image à afficher pour générer l'effet zoom
        e.DrawImage(ImageEncours, New Rectangle(0, 0, VisueAffichage.Width, VisueAffichage.Height),
                New Rectangle(DecalageTamponX, DecalageTamponY, VisueZoom.Width, VisueZoom.Height), GraphicsUnit.Pixel)
        If Not Selection.IsEmpty Then AfficherSelection(VisuePtOrigine, Selection, e)
    End Sub
    ''' <summary> affiche un rectangle représentant la séléction en cours </summary>
    ''' <param name="VisuePtOrigine"> Pt0 de l'affichage </param>
    ''' <param name="Selection"> sélection en coordonnées virtuelles </param>
    ''' <param name="e"> Graphics sur lequel dessiner </param>
    Friend Sub AfficherSelection(VisuePtOrigine As Point, Selection As Rectangle, e As Graphics)
        Dim Pt0 As Point = VisuePtOrigine
        Dim DecalageTamponX As Integer = Selection.X - Pt0.X
        Dim DecalageTamponY As Integer = Selection.Y - Pt0.Y
        Selection.Location = New Point(CInt(DecalageTamponX * CoefZoomAffichage), CInt(DecalageTamponY * CoefZoomAffichage))
        Dim S = Selection.Size
        Selection.Size = New Size(CInt(S.Width * CoefZoomAffichage), CInt(S.Height * CoefZoomAffichage))
        e.FillRectangle(Brosse_Selection, Selection)
    End Sub
    ''' <summary> Charge une échelle et remplit le tampon encours </summary>
    ''' <param name="VisuePtOrigine">point exprimé en coordonnées virtuelles qui sera au centre de la visue</param>
    ''' <param name="ListeCartes">liste des cartes composants l'échelle</param>
    ''' <param name="CoefZoom">CoefZoom demandé pour l'affichage</param>
    ''' <param name="VisueAffichage">dimensions en pixel de la visue</param>
    Friend Sub ChargerRegroupement(VisuePtOrigine As Point, ListeCartes As List(Of Carte), CoefZoom As Double, VisueAffichage As Size, CoulVisue As Color)
        'initialisation des différentes variables de supportvisue
        CalculerVariablesSupportVisue(CoefZoom, VisueAffichage)
        Encours = 0
        Futur = 1
        Tampons(Encours).ListeCartes = ListeCartes
        Tampons(Futur).ListeCartes = ListeCartes
        'détermination du pt(0) du tampon et donc du Georef en sachant que PTInit représente le centre du tampon
        Tampons(Encours).Pt0 = New Point(VisuePtOrigine.X - SurfaceGeorefEncours.Width \ 2, VisuePtOrigine.Y - SurfaceGeorefEncours.Height \ 2)
        ChangerCouleurVisue(CoulVisue)
        EtatDecalage = EtatsTampon.Aucun
    End Sub
    ''' <summary> initialise la ligne de couleur avec la couleur de fond demandée et met à jour le tampon </summary>
    ''' <param name="CouleurFond">couleur pour l'initialisation </param>
    Friend Sub ChangerCouleurVisue(CouleurFond As Color)
        CouleurVisue = CouleurFond
        Tampons(Encours).RemplirTampon(GeorefEncours, CouleurVisue)
    End Sub
End Module