Imports FCGP.ServeurCarto
''' <summary> classe servant de support à l'affichage de la carte </summary>
Friend Class AffichageCarte
    Implements IDisposable
    ''' <summary> couleur de base de l'affichage avant le dessin des tuiles </summary>
    Friend Property CouleurFond As Color
    ''' <summary> surface des fonds de plan exprimée en tuiles dela couche des fonds de plan </summary>
    Friend ReadOnly Property SurfaceFond As Rectangle
        Get
            Return New Rectangle(Pt0.IndexTuile, Taille)
        End Get
    End Property
    ''' <summary> surface des pentes exprimée en tuiles de la couche des pentes </summary>
    Friend ReadOnly Property SurfacePentes As Rectangle
        Get
            Return New Rectangle(Pt0Pentes.IndexTuile, TaillePentes)
        End Get
    End Property
    ''' <summary> Décalage en pixels entre Pt0 de l'affichage et le Pt0 du tampon de fond. Calculer à chaque décalage effectif du tampon puis mis à jour
    ''' avec le déplacement de la carte par la souris ou les touches. Le décalage doit toujours être compris dans la surface d'une tuile de fond </summary>
    Friend ReadOnly Property DecalageTampon As Point
        Get
            Return CType(_DecalageTampon, Point)
        End Get
    End Property
    ''' <summary> Décalage en pixels entre Pt0.X de l'affichage et le Pt0.X du tampon de fond. Calculer à chaque décalage effectif du tamon puis mis
    ''' à jour avec le déplacement de la carte par la souris ou les touches. Le décalage X doit toujours être entre 0 et la largeur d'une tuile -1 </summary>
    Friend Property DecalageTamponX As Integer
        Get
            Return _DecalageTampon.Width
        End Get
        Set(value As Integer)
            _DecalageTampon.Width = value
        End Set
    End Property
    ''' <summary> Décalage en pixels entre PT0.Y de l'affichage et le Pt0.Y du tampon de fond. Calculer à chaque décalage effectif du tamon puis mis 
    ''' à jour avec le déplacement de la carte par la souris ou les touches. Le décalage Y doit toujours être entre 0 et la hauteur d'une tuile -1 </summary>
    Friend Property DecalageTamponY As Integer
        Get
            Return _DecalageTampon.Height
        End Get
        Set(value As Integer)
            _DecalageTampon.Height = value
        End Set
    End Property
    ''' <summary> coordonnées en monde virtuel du centre de l'affichage </summary>
    Friend ReadOnly Property CentreAffichage As Point
        Get
            Return Point.Add(LocationAffichage, QuartVisue)
        End Get
    End Property
    ''' <summary> coordonnées en monde virtuel (pixels) du coin haut gauche de la visue. </summary>
    Friend ReadOnly Property LocationAffichage As Point
        Get
            Return Point.Add(Pt0.Pt0Tuile, _DecalageTampon)
        End Get
    End Property
    ''' <summary> indique si l'initialisation de l'affichage s'est bien déroulée </summary>
    Friend ReadOnly Property IsOk As Boolean
    ''' <summary> initialise l'affichage </summary>
    ''' <param name="ReserveTampon"> Espace mémoire pour stocker des données sous forme binaire </param>
    ''' <param name="TailleTampon"> dimensions maximum accordé au stockage de l'image de fond </param>
    ''' <param name="CouleurFond"> couleur initiale de l'image de fond </param>
    Friend Sub New(ReserveTampon As SharedPinnedByteArray, TailleTampon As Size, CouleurFond As Color)
        Encours = 0
        Futur = 1
        _CouleurFond = CouleurFond
        'dimensionne 2 tampons d'affichage pour le fond
        ReDim SurfaceAffichage(1)
        'réserve l'espace mémoire pour les 2 tampons associé à la visue
        SurfaceAffichage(Encours) = New SharedPinnedByteArray(TailleTampon.Width, TailleTampon.Height, ReserveTampon, "Affichage_1")
        If Not SurfaceAffichage(Encours).IsOk Then Exit Sub
        SurfaceAffichage(Futur) = New SharedPinnedByteArray(TailleTampon.Width, TailleTampon.Height, ReserveTampon, "Affichage_2")
        If Not SurfaceAffichage(Futur).IsOk Then Exit Sub
        _IsOk = True
    End Sub
    ''' <summary> initialise les dimensions réelles de l'affichage en fonction de la surface de la visue </summary>
    ''' <param name="TailleVisue"> taille en pixels de la surface d'affichage </param>
    ''' <param name="Coordonnees"> coordonnées exprimées en pixels du monde virtuel du centre de l'affichage </param>
    ''' <param name="FlagInit"> indique que l'application n'a pas encore finie d'être initialisée </param>
    Friend Sub ChangerTaille(TailleVisue As Size, Coordonnees As PointG, FlagInit As Boolean)
        Dim IndexTuile_Encours As Point = Pt0.IndexTuile
        Dim Stride_Encours As Integer = StrideImageFond
        'Recherche de la taille des tampons de l'affichage
        Taille = New Size(CInt(Math.Ceiling((TailleVisue.Width + NbPixelsTuile) / NbPixelsTuile)),
                          CInt(Math.Ceiling((TailleVisue.Height + NbPixelsTuile) / NbPixelsTuile)))
        'initialisation de variables utiles durant l'utilisation de l'affichage
        NbLignesFond = (Taille.Height) * NbPixelsTuile
        NbPixelsLigneFond = (Taille.Width) * NbPixelsTuile
        QuartVisue = New Size(TailleVisue.Width \ 2, TailleVisue.Height \ 2)
        StrideImageFond = StrideImage(NbPixelsLigneFond)
        'calcule les coordonnées du Pt0 de la visue
        Pt0 = PointG.Substract(Coordonnees, QuartVisue)
        'au depart l'offset du tampon et le décalage sont égaux. offset est immuable alors que le décalage varie avec le déplacement
        _DecalageTampon = Pt0.Offset
        If IsAffichagePentes Then CalculerTamponPentes()
        EchelleDessin = New DessinEchelle(IsDessinEchelle)
        If Not FlagInit Then
            'on RAZ le tampon futur afin de voir les nouvelles tuiles arrivées
            SurfaceAffichage(Futur).ClearColor(_CouleurFond)
            'on copie la partie du tampon en cours qui se trouve encore sur le tampon futur afin d'éviter les scintillements
            CopierTampon(IndexTuile_Encours.X - Pt0.IndexTuile.X, IndexTuile_Encours.Y - Pt0.IndexTuile.Y, Stride_Encours, StrideImageFond)
            Dim Tempo As Integer = Futur
            Futur = Encours
            Encours = Tempo
        Else
            'on RAZ les tampons
            SurfaceAffichage(Encours).ClearColor(_CouleurFond)
            SurfaceAffichage(Futur).ClearColor(_CouleurFond)
        End If
    End Sub
    ''' <summary> calcul des coordoonnées pixels avec la nouvelle échelle, positionne l'image actuelle en fonction de la nouvelle échelle </summary>
    ''' <param name="LocationCurseur"> Position du curseur de souris sur la surface de la Visue en pixels écran qui devient le centre du nouvel affichage </param>
    ''' <param name="DeltaIndiceEchelles"> Ecart entre les indices de l'échelle actuelle et de l'échelle future </param>
    Friend Sub ChangerCouche(LocationCurseur As Point, DeltaIndiceEchelles As Integer)
        'calcul du facteur de modification de l'image actuelle. Si deltaEchelle < 0 alors agrandissement l'image actuelle par fact sinon réduction par 1/fact
        Dim FactZoom As Double = Serveur.FacteurEchelles(Pt0.IndiceEchelle, DeltaIndiceEchelles)
        'calcul des coordonnées de la souris ou du centre de la visue avec le nouveau zoom fonction du deltazoom
        Dim CoordDebut As Point = Point.Add(LocationAffichage, New Size(LocationCurseur))
        Dim CoordFin = New PointG(Pt0.IndiceEchelle + DeltaIndiceEchelles, New Point(CInt(CoordDebut.X * FactZoom), CInt(CoordDebut.Y * FactZoom)))
        Dim Pt0Fin As PointG = PointG.Substract(CoordFin, LocationCurseur)
        'rectangle servant pour l'affichage de l'image actuelle agrandie ou réduite à l'endroit de la nouvelle image pour donner l'impression de continuté
        Dim RectSource As Rectangle, RectDest As Rectangle
        'calcul des rectangles permettant l'agrandissement ou la réduction lors du dessin de l'image dans le tampon futur
        If DeltaIndiceEchelles < 0 Then
            'dans ce cas il faut inverser le debut et la fin ainsi que le facteur de zoom pour le calcul du rectangle zoomé
            FactZoom = 1 / FactZoom
            'le rectangle Source est une partie de l'image source
            RectSource = TrouverRectangleZoom(Pt0Fin.IndexTuile, Pt0.IndexTuile, FactZoom)
            'qui sera dessiné agrandi à la taille du tampon de fond
            RectDest = New Rectangle(Point.Empty, New Size(NbPixelsLigneFond, NbLignesFond))
        Else
            'le rectangle source est la totalité de l'image source
            RectSource = New Rectangle(Point.Empty, New Size(NbPixelsLigneFond, NbLignesFond))
            'qui sera dessiné réduit à une partie de la taille du tampon de fond
            RectDest = TrouverRectangleZoom(Pt0.IndexTuile, Pt0Fin.IndexTuile, FactZoom)
        End If
        'on RAZ le tampon futur afin de voir les nouvelles tuiles arrivées
        SurfaceAffichage(Futur).ClearColor(CouleurFond)
        'on dessine sur le tampon futur la partie de l'image agrandie ou réduite qui se trouve à l'endroit de la nouvelle image
        Using Bmp As New Bitmap(NbPixelsLigneFond, NbLignesFond, StrideImageFond, FormatPixel, SurfaceAffichage(Futur).BitPtr)
            Using G As Graphics = Graphics.FromImage(Bmp)
                G.DrawImage(ImageFond, RectDest, RectSource, GraphicsUnit.Pixel)
            End Using
        End Using
        'échange des surfaces d'affichage
        Dim Tempo As Integer = Futur
        Futur = Encours
        Encours = Tempo
        'change les coordonnées du Pt0 avec les coordonnées du nouveau zoom 
        Pt0 = Pt0Fin
        _DecalageTampon = Pt0.Offset
        If IsAffichagePentes Then CalculerTamponPentes()
        EchelleDessin = New DessinEchelle(IsDessinEchelle)
    End Sub
    ''' <summary> calcule la surface en tuile Pentes qu'il faut télécharger pour correspondre à la surface visue actuelle </summary>
    Friend Sub CalculerTamponPentes()
        Dim IndiceEchellePentes = Serveur.IndiceEchellePentes
        Dim IndiceEchelleAffichage = Pt0.IndiceEchelle
        Dim DeltaIndiceEchelle = IndiceEchellePentes - IndiceEchelleAffichage
        Dim FactZoom As Double = Serveur.FacteurEchelles(IndiceEchelleAffichage, DeltaIndiceEchelle)
        Dim InvFactZoom As Double = 1 / FactZoom
        'l'emploi du Round est obligatoire pour le site SM qui n'a pas que des coefs de zoom de 2 pour chaque niveaux
        DimensionsTuilePentes = CInt(Math.Round(NbPixelsTuile * InvFactZoom, 0))
        Dim LocationPentes As New Point(CInt(Pt0.Location.X * FactZoom), CInt(Pt0.Location.Y * FactZoom))
        Pt0Pentes = New PointG(IndiceEchellePentes, LocationPentes)
        TaillePentes = New Size(CInt(Math.Ceiling((QuartVisue.Width * 2 + Pt0Pentes.Offset.Width * InvFactZoom) / DimensionsTuilePentes)),
                                CInt(Math.Ceiling((QuartVisue.Height * 2 + Pt0Pentes.Offset.Height * InvFactZoom) / DimensionsTuilePentes)))
        DecalageTamponPentes = New Point(If(DeltaIndiceEchelle = 0, 0, Pt0.Offset.Width - CInt(Math.Round(Pt0Pentes.Offset.Width * InvFactZoom, 0))),
                                         If(DeltaIndiceEchelle = 0, 0, Pt0.Offset.Height - CInt(Math.Round(Pt0Pentes.Offset.Height * InvFactZoom, 0))))
    End Sub
    ''' <summary> décale le tampon en mettant à jour indexDeb et Decale X ou Y et en copiant une partie du tampon encours sur le tampon futur </summary>
    Friend Sub DecalerTampon()
        'on calcule le nb entier de colonne et de rangée à décaler, pour la copie du tampon encours sur le tampon futur
        Dim DecalageCol As Integer = CInt(Math.Floor(_DecalageTampon.Width / NbPixelsTuile))
        Dim DecalageRow As Integer = CInt(Math.Floor(_DecalageTampon.Height / NbPixelsTuile))
        'on RAZ le tampon futur afin de voir les nouvelles tuiles arrivées
        SurfaceAffichage(Futur).ClearColor(CouleurFond)
        'on copie la partie du tampon en cours qui se trouve encore sur le tampon futur afin d'éviter les scintillements
        CopierTampon(DecalageCol, DecalageRow)
        'on procède au décalage du point Pt0 du tampon et de l'affichage
        Dim DecalageReelVisue = Size.Subtract(_DecalageTampon, Pt0.Offset)
        Pt0.Add(DecalageReelVisue)
        'on ajuste le décalage de la visue en fonction
        _DecalageTampon = Pt0.Offset
        'on échange les surfaces d'affichage
        Dim Tempo As Integer = Futur
        Futur = Encours
        Encours = Tempo
        If IsAffichagePentes Then CalculerTamponPentes()
    End Sub
    ''' <summary> Renvoie une image qui représente le fond de plan avec éventuellement la couche des pentes </summary>
    ''' <param name="FlagRemplirTampon"> indique qu'une demande de remplissage du tampon a été demandée </param>
    Friend Async Function ImageAffichage(FlagRemplirTampon As Boolean) As Task(Of Bitmap)
        If Serveur.NbDemandesAffichage > 0 OrElse FlagRemplirTampon Then
            Return Await CreerImageAffichageAsync()
        Else
            Return ImageFond
        End If
    End Function
    ''' <summary> permet la libération des ressources http et memoire </summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        'nécessaire pour éviter que le GC ne redemande Finalize()
        GC.SuppressFinalize(Me)
    End Sub
#Region "private"
    Private Async Function CreerImageAffichageAsync() As Task(Of Bitmap)
        Dim DecalPixelsX, DecalPixelsY As Integer, Surface As Rectangle, ReserveBitPtr As IntPtr, Image As Bitmap
        Dim IndexTamponPtr As IntPtr, CoucheTxt As String
        Surface = SurfaceFond
        ReserveBitPtr = SurfaceAffichage(Encours).BitPtr
        CoucheTxt = CoucheFond.ToString("00")
        'decalage d'une tuile vers le bas
        Dim DecalY = NbPixelsTuile * StrideImageFond
        Dim DecalTuile As Integer = 0
        Dim IndexTuile As Integer = 0
        SurfaceAffichage(Futur).ClearColor(_CouleurFond)
        'remplir le tampon selectionné avec les tuiles correspondantes à partir du cache
        For Row As Integer = Surface.Top To Surface.Bottom - 1
            For Col As Integer = Surface.Left To Surface.Right - 1
                Dim BytesTuile = Await Serveur.Cache.LireOctetsTuileAsync(Col, Row, CoucheTxt)
                If BytesTuile IsNot Nothing Then
                    IndexTamponPtr = ReserveBitPtr + DecalPixelsX + DecalPixelsY
                    Using TuileImage = New Bitmap(New MemoryStream(BytesTuile))
                        Dim BD As BitmapData = TuileImage.LockBits(New Rectangle(0, 0, NbPixelsTuile, NbPixelsTuile), ImageLockMode.ReadOnly, FormatPixel)
                        Dim IndexTuilePtr As IntPtr = BD.Scan0
                        For Cpt As Integer = 0 To NbPixelsTuile - 1
                            CopierMemoire(IndexTamponPtr, IndexTuilePtr, NbOctetsDecalXTuile)
                            IndexTamponPtr += StrideImageFond
                            IndexTuilePtr += NbOctetsDecalXTuile
                        Next
                        TuileImage.UnlockBits(BD)
                    End Using
                End If
                DecalPixelsX += NbOctetsDecalXTuile
                IndexTuile += 1
            Next
            DecalPixelsX = 0
            DecalPixelsY += DecalY
            DecalTuile += Surface.Width
            IndexTuile = DecalTuile
        Next
        Image = New Bitmap(NbPixelsLigneFond, NbLignesFond, StrideImageFond, FormatPixel, ReserveBitPtr)
        If IsAffichagePentes Then Await AjouterPentes(Image)
        Return Image
    End Function
    ''' <summary> Ajoute la couche des Pentes à l'image de l'affichage</summary>
    Private Async Function AjouterPentes(Image As Bitmap) As Task
        Dim DecalageTuileDestination = DecalageTamponPentes
        Dim TailleTuileDestination = New Size(DimensionsTuilePentes, DimensionsTuilePentes)
        Dim IndexTuile As Integer = 0
        Dim TuileImageDestination As Rectangle
        Dim SurfacePentes = New Rectangle(Pt0Pentes.IndexTuile, TaillePentes)
        Using G = Graphics.FromImage(Image)
            For Row As Integer = SurfacePentes.Top To SurfacePentes.Bottom - 1
                For Col As Integer = SurfacePentes.Left To SurfacePentes.Right - 1
                    Dim BytesTuile = Await Serveur.Cache.LireOctetsTuileAsync(Col, Row)
                    If BytesTuile IsNot Nothing Then
                        Using TuileImage = New Bitmap(New MemoryStream(BytesTuile))
                            TuileImageDestination = New Rectangle(DecalageTuileDestination, TailleTuileDestination)
                            G.DrawImage(TuileImage, TuileImageDestination, 0, 0, NbPixelsTuile, NbPixelsTuile, GraphicsUnit.Pixel, ImageAttributsPentes)
                        End Using
                    End If
                    IndexTuile += 1
                    DecalageTuileDestination.X += DimensionsTuilePentes
                Next
                DecalageTuileDestination.X = DecalageTamponPentes.X
                DecalageTuileDestination.Y += DimensionsTuilePentes
            Next
        End Using
    End Function
    ''' <summary> rectangle qui permet d'afficher une couche serveur avec une échelle différente de celle de la couche
    ''' tout en conservant la même coordonnées indiquer pour la couche (centre affichage ou click souris </summary>
    ''' <param name="IndexTuileDeb"> Indextuile du Pt0 de la couche de début </param>
    ''' <param name="IndexTuileFin"> Indextuile du Pt0 de la couche de fin </param>
    ''' <param name="FactZoom"> facteur de Zoom à appliquer à la couche début </param>
    Private Function TrouverRectangleZoom(IndexTuileDeb As Point, IndexTuileFin As Point, FactZoom As Double) As Rectangle
        Dim Rect As New Rectangle(New Point(CInt((IndexTuileDeb.X * FactZoom - IndexTuileFin.X) * NbPixelsTuile),
                                            CInt((IndexTuileDeb.Y * FactZoom - IndexTuileFin.Y) * NbPixelsTuile)),
                                  New Size(CInt(NbPixelsLigneFond * FactZoom), CInt(NbLignesFond * FactZoom)))
        Return Rect
    End Function
    ''' <summary> copie en partie le tampon encours sur le tampon futur. Cas du changement de taille de la fenêtre </summary>
    ''' <param name="DecalageCol"> nb de colonnes de tuiles à décaler </param>
    ''' <param name="DecalageRow"> nb de rangées de tuiles à décaler </param>
    ''' <param name="StrideEncours">Nb de pixels par ligne du tampon encours </param>
    ''' <param name="StrideFutur"> Nb de pixels par ligne du tampon futur </param>
    Private Sub CopierTampon(DecalageCol As Integer, DecalageRow As Integer, StrideEncours As Integer, StrideFutur As Integer)
        Dim NbOctectsTransfert As Integer, IndexEncours, IndexFutur As IntPtr
        If DecalageCol < 0 OrElse DecalageRow < 0 Then 'La taille de la fenêtre a diminuée, le décalage se fait par rapport au tampon encours
            NbOctectsTransfert = StrideFutur
            IndexEncours = SurfaceAffichage(Encours).BitPtr - DecalageCol * NbOctetsDecalXTuile - DecalageRow * NbPixelsTuile * StrideEncours
            IndexFutur = SurfaceAffichage(Futur).BitPtr
        Else 'la taille de la fenêtre a augmentée, le décalage se fait par rapport au tampon futur
            NbOctectsTransfert = StrideEncours
            IndexEncours = SurfaceAffichage(Encours).BitPtr
            IndexFutur = SurfaceAffichage(Futur).BitPtr + DecalageCol * NbOctetsDecalXTuile + DecalageRow * NbPixelsTuile * StrideFutur
        End If
        Parallel.For(0, NbLignesFond - Math.Abs(DecalageRow * NbPixelsTuile),
                     Sub(Cpt As Integer)
                         Dim IndexSource As IntPtr = IndexEncours + Cpt * StrideEncours
                         Dim IndexDestination As IntPtr = IndexFutur + Cpt * StrideFutur
                         CopierMemoire(IndexDestination, IndexSource, NbOctectsTransfert)
                     End Sub)
    End Sub
    ''' <summary> copie en partie le tampon encours sur le tampon futur. Cas du décalage de la carte dans la même fenêtre </summary>
    ''' <param name="DecalageCol"> nb de colonnes de tuiles à décaler </param>
    ''' <param name="DecalageRow"> nb de rangées de tuiles à décaler </param>
    Private Sub CopierTampon(DecalageCol As Integer, DecalageRow As Integer)
        Dim DecalOctetsX As Integer = DecalageCol * NbOctetsDecalXTuile
        Dim DecalOctetsY As Integer = DecalageRow * NbPixelsTuile * StrideImageFond
        Dim NbOctectsTransfert As Integer = StrideImageFond - Math.Abs(DecalOctetsX)
        Dim IndexEncours As IntPtr = SurfaceAffichage(Encours).BitPtr + If(DecalOctetsX > 0, DecalOctetsX, 0) + If(DecalOctetsY > 0, DecalOctetsY, 0)
        Dim IndexFutur As IntPtr = SurfaceAffichage(Futur).BitPtr + If(DecalOctetsX > 0, 0, -DecalOctetsX) + If(DecalOctetsY > 0, 0, -DecalOctetsY)
        Parallel.For(0, NbLignesFond - Math.Abs(DecalageRow * NbPixelsTuile),
                     Sub(Cpt As Integer)
                         Dim IndexSource As IntPtr = IndexEncours + Cpt * StrideImageFond
                         Dim IndexDestination As IntPtr = IndexFutur + Cpt * StrideImageFond
                         CopierMemoire(IndexDestination, IndexSource, NbOctectsTransfert)
                     End Sub)
    End Sub
    ''' <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If disposed Then Exit Sub
        If disposing Then
            SurfaceAffichage(Encours)?.Dispose()
            SurfaceAffichage(Futur)?.Dispose()
        End If
        disposed = True
    End Sub
    ''' <summary> c'est l'image des fonds de plan de l'affichage (tampon encours) </summary>
    Private ReadOnly Property ImageFond() As Bitmap
        Get
            Return New Bitmap(NbPixelsLigneFond, NbLignesFond, StrideImageFond, FormatPixel, SurfaceAffichage(Encours).BitPtr)
        End Get
    End Property
    ''' <summary> dimensions d'une tuile pentes vue de la couche Fond. il y a un coef de zoom </summary>
    Private DimensionsTuilePentes As Integer
    ''' <summary> décalage du tampon des pentes par rapport au tampon du fond exprimé en pixels. C'est toujours 0 ou une dimension de tuilePentes </summary>
    Private DecalageTamponPentes As Point
    ''' <summary> nb d'octets par ligne du tampon de fond </summary>
    Private StrideImageFond As Integer
    ''' <summary> tampons pour l'affichage </summary>
    Private ReadOnly SurfaceAffichage() As SharedPinnedByteArray
    ''' <summary> indique si la carte a été détruite</summary>
    Private disposed As Boolean
    ''' <summary> nb de lignes composant l'affichage du fond </summary>
    Private NbLignesFond As Integer
    ''' <summary> nb de pixels par ligne composant l'affichage du fond </summary>
    Private NbPixelsLigneFond As Integer
    ''' <summary> information initiale (changerTaille ou decalerTampon) du point haut gauche de l'affichage du fond </summary>
    Private Pt0 As PointG
    ''' <summary> information initale (changerTaille ou decalerTampon) du point haut gauche de l'affichage des pentes </summary>
    Private Pt0Pentes As PointG
    ''' <summary> décalage de l'affichage par rapport au tampon </summary>
    Private _DecalageTampon As Size
    ''' <summary> Nb de colonnes et de rangées de l'affichage exprimée en nb de tuiles </summary>
    Private Taille As Size
    ''' <summary> Nb de colonnes et de rangées des pentes exprimée en nb de tuiles </summary>
    Private TaillePentes As Size
    ''' <summary> index du tampon d'affichage encours d'utilisation </summary>
    Private Encours As Integer
    ''' <summary> index du tampon d'affichage qui sera utiliser après une opération de décalage ou d'agrandissement ou de réduction </summary>
    Private Futur As Integer
    ''' <summary> quart de la surface de la visue </summary>
    Friend QuartVisue As Size
    ''' <summary> Couche sur le serveur correspondant à l'échelle en cours du tampon de fond </summary>
    Friend CoucheFond As Byte
    ''' <summary> Echelle des fonds de plan. </summary>
    Friend Echelle As Echelles
    ''' <summary> indique si la couche encours du tampon de fond peut afficher les pentes </summary>
    Friend CoucheFondIspentes As Boolean
#End Region
End Class