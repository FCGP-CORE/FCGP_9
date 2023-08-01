''' <summary> tampon support de l'affichage de la visue. Les données afichées sont stockées ici </summary>
Friend Class TamponVisue
    Implements IDisposable
    ''' <summary> le tableau d'octets traité en tant que bitMap (image)</summary>
    Friend ReadOnly Property Image As Bitmap
    ''' <summary> Nb d'octets du tampon</summary>
    Friend ReadOnly Property NbBits As Integer
        Get
            Return Tampon.NbBytes
        End Get
    End Property
    ''' <summary> tableau d'octets du tampon</summary>
    Friend ReadOnly Property Bits As Byte()
        Get
            Return Tampon.Bits
        End Get
    End Property
    ''' <summary> index du 1er octet du tampon dans le tableau</summary>
    Friend ReadOnly Property IndexBits As Integer
        Get
            Return Tampon.IndexByte
        End Get
    End Property
    ''' <summary> pointeur sur le 1er octet du tampon dans le tableau</summary>
    Friend ReadOnly Property BitPtr As IntPtr
        Get
            Return Tampon.BitPtr
        End Get
    End Property
    ''' <summary> Region représentée par le tampon exprimée en unité du monde virtuel</summary>
    Friend ReadOnly Property Georef As Rectangle
    Friend Property Pt0 As Point
        Get
            Return _Pt0
        End Get
        Set(value As Point)
            _Pt0 = value
            _Georef = New Rectangle(_Pt0, _Surface)
        End Set
    End Property
    Friend ReadOnly Property Stride As Integer
    Friend ReadOnly Property NbLignes As Integer
    Friend ReadOnly Property Surface As Size
    ''' <summary> Pt0 de la region représentée par le tampon exprimée en unité du monde virtuel</summary>
    Private _Pt0 As Point
    ''' <summary> tampon servant pour l'initialisation du tampon avec une couleur </summary>
    Private ReadOnly Tampon As SharedPinnedByteArray
    ''' <summary> représente le décalge en octets par rapport au début du tampon de RaR </summary>
    Private ReadOnly Property IndexDepart(Rar As Rectangle) As Integer
        Get
            Return (Rar.Y - _Pt0.Y) * _Stride + (Rar.X - _Pt0.X) * NbOctetsPixel
        End Get
    End Property
    Private Cartes As List(Of Carte)
    Private CartePrincipale As CarteIntersectTampon
    ''' <summary> liste des cartes servant pour le remplissage des tampons </summary>
    Friend WriteOnly Property ListeCartes As List(Of Carte)
        Set(value As List(Of Carte))
            Cartes = value
        End Set
    End Property
    ''' <summary> réserve en mémoire le nb d'octects nécéssaires pour le stockage d'une image ayant la surface indiquée </summary>
    ''' <param name="SurfGeoref"> surface de l'image exprimée en pixels</param>
    ''' <param name="ReserveBase"> faut'il utiliser un tampon d'octets déja existant en memoire </param>
    ''' <param name="LC">Liste des cartes servant au remplissage du tampon</param>
    Friend Sub New(SurfGeoref As Size, Reserve As SharedPinnedByteArray, LC As List(Of Carte), NomTampon As String)
        _Surface = SurfGeoref
        _Stride = StrideImage(_Surface.Width)
        _NbLignes = _Surface.Height
        Cartes = LC
        CartePrincipale = New CarteIntersectTampon
        Tampon = New SharedPinnedByteArray(_Stride, _NbLignes, Reserve, NomTampon)
        _Image = New Bitmap(_Surface.Width, _Surface.Height, _Stride, FormatPixel, Tampon.BitPtr)
    End Sub
    Friend Sub New(SurfGeoref As Size, NomTampon As String)
        _Surface = SurfGeoref
        _Stride = StrideImage(_Surface.Width)
        _NbLignes = _Surface.Height
        CartePrincipale = New CarteIntersectTampon
        Tampon = New SharedPinnedByteArray(_Stride, _NbLignes, Nothing, NomTampon)
        _Image = New Bitmap(_Surface.Width, _Surface.Height, _Stride, FormatPixel, Tampon.BitPtr)
    End Sub
    ''' <summary> remplit la region du tampon avec des cartes en fonction de leur Georef commun </summary>
    ''' <param name="RaR">Region du tampon à remplir</param>
    ''' <param name="Couleur"> couleur de fond du tampon </param>
    Friend Sub RemplirTampon(RaR As Rectangle, Couleur As Color)
        'avant de remplir il faut mettre la couleur par défaut au cas où il n'y aurait pas la totalité du remplissage par les cartes
        If RaR = _Georef Then
            Tampon.ClearColor(Couleur)
        Else
            Tampon.ClearColor(Couleur, IndexDepart(RaR), StrideImage(RaR.Width), RaR.Height)
        End If
        If Cartes IsNot Nothing Then 'on peut remplir le tampon avec des cartes
            Dim Cs As List(Of CarteIntersectTampon) = RenvoyerCartesIntersectWithTampon(RaR)
            Dim ListeTache(Cs.Count - 1) As Task
            For Cpt As Integer = 0 To Cs.Count - 1
                Dim C As CarteIntersectTampon = Cs(Cpt)
                ListeTache(Cpt) = Task.Run(Sub() LireCarteDansTampon(RaR, C, 4))
            Next
            Task.WaitAll(ListeTache)
        End If
    End Sub
#Region "Procedures pour remplir un tampon"
    ''' <summary> Transfert de tout ou partie d'une image au format BMP dans un tampon en fonction du georef commun</summary>
    ''' <param name="CIT">carte à transférer d'un fichier vers le tampon avec son Georef</param>
    ''' <param name="RaR">Georef du tampon</param>
    ''' <param name="NbSousCartes"> si > 1, lance la lecture de la carte avec des boucles en parallele </param>
    Private Sub LireCarteDansTampon(RaR As Rectangle, CIT As CarteIntersectTampon, Optional NbSousCartes As Integer = 4)
        Dim StrideCarte As Integer = CIT.LargeurCarte
        'où démarre la lecture dans la carte. Dans la carte les lignes sont stockées dans le sens inverse. On commence par la dernière ligne
        Dim IndexOctetsCarteTotal As Long = (CIT.PositionGeoref.Bottom - CIT.Intersect.Bottom) * StrideCarte + (CIT.Intersect.Left - CIT.PositionGeoref.Left) * NbOctetsPixel + OffsetDataBMP
        'nb d'octets à lire dans l'image et à écrire dans le tampon = largeur de l'intersection carte tampon * nb octets du format graphique (3)
        Dim NbOctectsAlire As Integer = CIT.Intersect.Width * NbOctetsPixel
        'où démarre l'écriture dans le tampon
        Dim IndexOctetsTamponTotal As Integer = IndexDepart(RaR) + (RaR.Height + CIT.Intersect.Bottom - RaR.Bottom - 1) * Stride + (CIT.Intersect.Left - RaR.Left) * NbOctetsPixel + IndexBits
        Dim HauteurTotale As Integer = CIT.Intersect.Height
        If NbSousCartes = 1 Then
            Dim HauteurPartielle As Integer = CIT.Intersect.Height
            LireSousCarteDansSousTampon(CIT.Chemin, StrideCarte, IndexOctetsCarteTotal, NbOctectsAlire, IndexOctetsTamponTotal, HauteurTotale, HauteurPartielle, 0)
        Else
            Dim HauteurPartielle As Integer = (CIT.Intersect.Height - 1) \ NbSousCartes + 1
            Parallel.For(0, NbSousCartes, Sub(IndexSousCarte As Integer)
                                              LireSousCarteDansSousTampon(CIT.Chemin, StrideCarte, IndexOctetsCarteTotal, NbOctectsAlire,
                                                                          IndexOctetsTamponTotal, HauteurTotale, HauteurPartielle, IndexSousCarte)
                                          End Sub)
        End If
    End Sub
    ''' <summary> transfert d'une sous partie de la carte à partir d'un fichier de type .bmp  vers le tampon</summary>
    ''' <param name="CheminCarte">chemin de la carte</param>
    ''' <param name="StrideCarte">nb d'octets à lire pour passer à la ligne suivante de la carte</param>
    ''' <param name="IndexOctetsCarteTotal">ou commence dans la carte la lecture de la première ligne à transférer</param>
    ''' <param name="NbOctectsAlire">combien d'octets doit'on lire sur la ligne en cours</param>
    ''' <param name="IndexOctetsTamponTotal">ou commence dans le tampon l'écriture de la première ligne à transférer</param>
    ''' <param name="HauteurTotale">Hauteur totale à lire et écrire</param>
    ''' <param name="HauteurPartielle">Hauteur partielle à lire et écrire</param>
    ''' <param name="IndexSousCarte">index de la sous-carte en cours</param>
    Private Sub LireSousCarteDansSousTampon(CheminCarte As String, StrideCarte As Integer, IndexOctetsCarteTotal As Long, NbOctectsAlire As Integer, IndexOctetsTamponTotal As Integer,
                                            HauteurTotale As Integer, HauteurPartielle As Integer, IndexSousCarte As Integer)
        'nb de lignes à lire et à écrire
        Dim HauteurLue As Integer = If(IndexSousCarte = 0, 0, HauteurPartielle * (IndexSousCarte))
        Dim HauteurSousCarte As Integer = If(HauteurLue + HauteurPartielle > HauteurTotale, HauteurTotale - HauteurLue, HauteurPartielle)
        'où démarre la lecture dans la sous-carte
        Dim IndexOctetsSousCarte As Long = IndexOctetsCarteTotal + If(IndexSousCarte = 0, 0, IndexSousCarte * HauteurPartielle * StrideCarte)
        'où démarre l'écriture dans le sous-tampon
        Dim IndexOctetsSousTampon As Integer = IndexOctetsTamponTotal - If(IndexSousCarte = 0, 0, IndexSousCarte * HauteurPartielle * Stride)
        Using FluxBMP As New IO.FileStream(CheminCarte, FileMode.Open, FileAccess.Read, FileShare.Read)
            For BoucleLigne As Integer = 0 To HauteurSousCarte - 1
                FluxBMP.Position = IndexOctetsSousCarte
                FluxBMP.Read(Bits, IndexOctetsSousTampon, NbOctectsAlire)
                IndexOctetsSousCarte += StrideCarte
                IndexOctetsSousTampon -= Stride
            Next
        End Using
    End Sub
    ''' <summary> sert à l'initialisation de la recherche récursive et aux tests initiaux </summary>
    ''' <returns>la liste des parties de carte associées au tampon qui recouvrent le tampon sans chevauchement</returns>
    Private Function RenvoyerCartesIntersectWithTampon(Rar As Rectangle) As List(Of CarteIntersectTampon)
        'pour contenir le résultat des cartes qui ont une intersection avec le tampon ainsi que la surface correspondante
        Dim ResultatListeSurfaceCarte As New List(Of CarteIntersectTampon)
        'est ce que la carte de l'historique, celle qui a été la première au dernier remplissage, contient la totalité de la surface du tampon
        If Rar = Rectangle.Intersect(CartePrincipale.PositionGeoref, Rar) Then
            'si oui il n'y a plus rien à faire, si ce n'est l'indiquer dans le résultat
            ResultatListeSurfaceCarte.Add(New CarteIntersectTampon With {.Intersect = Rar, .IntersectTot = True,
                                              .Chemin = CartePrincipale.Chemin, .PositionGeoref = CartePrincipale.PositionGeoref, .LargeurCarte = CartePrincipale.LargeurCarte})
        Else
            'sinon il faut décomposer le résultat. on initialise la liste des cartes servant de paramètre
            Dim ListeCarteIntersectTampon As List(Of CarteIntersectTampon) = RequeterCartesIntersectWithTampon(Rar)
            'si il y a au moins une carte qui intersect avec le tampon, la plus grande surface est toujours la première de la liste
            If ResultatListeSurfaceCarte.Count > 0 Then
                CartePrincipale = ListeCarteIntersectTampon(0)
            Else
                CartePrincipale = New CarteIntersectTampon
            End If
            'on lance la recherche récursive, on récupère le résultat et on le trie par ordre croissant du nom de la carte
            ResultatListeSurfaceCarte = TrouverCartesIntersectWithRectangles(Rar, ListeCarteIntersectTampon).OrderBy(Function(C) C.Chemin).ToList
            'on fusionne les enregistrements des cartes qui apparaissent plusieurs fois afin de n'avoir qu'un seul acces au fichier .bmp de la carte
            Dim NomCarte As String = ""
            For Cpt As Integer = ResultatListeSurfaceCarte.Count - 1 To 0 Step -1
                'si le nom de la carte est égal à celui de l'enregistrement précédent c'est que la carte est présente plusieurs fois
                If ResultatListeSurfaceCarte(Cpt).Chemin = NomCarte Then
                    'on les fusionne en faisant une union de leur surface
                    ResultatListeSurfaceCarte(Cpt).Intersect =
                            Rectangle.Union(ResultatListeSurfaceCarte(Cpt).Intersect, ResultatListeSurfaceCarte(Cpt + 1).Intersect)
                    'et en supprimant l'enregistrement précédent
                    ResultatListeSurfaceCarte.RemoveAt(Cpt + 1)
                Else
                    'on stocke le nom de la carte pour le prochain tour
                    NomCarte = ResultatListeSurfaceCarte(Cpt).Chemin
                End If
            Next
        End If
        Return ResultatListeSurfaceCarte
    End Function
    ''' <summary> Trouve l'ensemble des cartes qui ont une surface en intersection avec la surface virtuelle représentée par le tampon et les renvoie sous forme de liste 
    ''' il s'agit d'une simple requête</summary>
    Private Function RequeterCartesIntersectWithTampon(Region As Rectangle) As List(Of CarteIntersectTampon)
        Dim F As IEnumerable(Of CarteIntersectTampon) = From C As Carte In Cartes
                                                        Where Not (C.RegionVirtuelle.Right < Region.Left OrElse C.RegionVirtuelle.Left > Region.Right OrElse
                                                                   C.RegionVirtuelle.Top > Region.Bottom OrElse C.RegionVirtuelle.Bottom < Region.Top)
                                                        Let Intersect As Rectangle = C.CroiserAvecRegionVirtuelle(Region),
                                                                Surface As Long = Intersect.Width * Intersect.Height,
                                                                IntersectTot As Boolean = Surface = (Region.Width * Region.Height)
                                                        Order By IntersectTot Descending, Surface Descending
                                                        Select New CarteIntersectTampon With {.Intersect = Intersect, .Chemin = C.CheminFluxLecture,
                                                                .IntersectTot = IntersectTot, .PositionGeoref = C.RegionVirtuelle, .LargeurCarte = C.LargeurOctets}
        Return F.ToList()
    End Function
    ''' <summary> Trouve l'ensemble des cartes qui ont une surface en intersection avec la surface virtuelle représentée par le rectangle et les renvoie sous forme de liste 
    ''' il s'agit d'une simple requête</summary>
    ''' <param name="LCIT"> Liste de cartes ayant été selectionnées auparavant </param>
    ''' <param name="RaR"> Rectangle de sélection </param>
    Private Shared Function RequeterCartesIntersectWithRectangle(RaR As Rectangle, LCIT As List(Of CarteIntersectTampon)) As List(Of CarteIntersectTampon)
        'trouve les cartes qui intersectent la surface du rectangle
        Dim F As IEnumerable(Of CarteIntersectTampon) =
                                From L As CarteIntersectTampon In LCIT
                                Where Not (L.Intersect.Right < RaR.Left OrElse L.Intersect.Left > RaR.Right OrElse
                                    L.Intersect.Top > RaR.Bottom OrElse L.Intersect.Bottom < RaR.Top)
                                Let Intersect As Rectangle = L.IntersectWithTampon(RaR),
                                    Surface As Long = Intersect.Width * Intersect.Height,
                                    IntersectTot As Boolean = Surface = (RaR.Width * RaR.Height)
                                Order By IntersectTot Descending, Surface Descending
                                Select New CarteIntersectTampon With {.Intersect = Intersect, .IntersectTot = IntersectTot,
                                    .Chemin = L.Chemin, .PositionGeoref = L.PositionGeoref, .LargeurCarte = L.LargeurCarte}
        Return F.ToList()
    End Function
    ''' <summary>procédure récursive. renvoie les cartes qui ont une intersection avec le tampon mais par rapport à une liste initale
    ''' la surface d'intersection est ajustée pour qu'elles ne se chevauche pas en éliminant les doublons et en priorisant les cartes ayant les plus grandes surfaces d'intersection </summary>
    ''' <param name="RaR">surface à remplir</param>
    ''' <param name="LCIT">liste des cartes qui ont une intersection avec le tampon</param>
    Private Function TrouverCartesIntersectWithRectangles(RaR As Rectangle, LCIT As List(Of CarteIntersectTampon)) As List(Of CarteIntersectTampon)
        'pour le retour de la fonction
        Dim ResultatListesurfaceCarte As New List(Of CarteIntersectTampon)
        'si il y a au moins une carte qui intersecte avec le tampon on met à jour le résulat
        If (LCIT.Count > 0) Then
            'il s'agit de la première carte
            ResultatListesurfaceCarte.Add(LCIT(0))
            'Si la carte ne remplit pas toute la surface du tampon il faut continuer le processus de décompositon 
            'de la surface restante en rectangle
            If Not LCIT(0).IntersectTot Then
                'on prend la surface initiale du tampon
                Dim DejaRemplit As Rectangle = LCIT(0).Intersect
                'on a plus besoin de la carte que l'on a pris en résultat
                LCIT.RemoveAt(0)
                'et on trouve de 1 à 4 rectangles qui composent la surface restant à remplir
                Dim ListeRectangle As New List(Of Rectangle)
                'Si les rectangles sont vides c'est qu'il n'y a pas d'intersection
                If RaR.Bottom <> DejaRemplit.Bottom Then _
                    ListeRectangle.Add(New Rectangle(RaR.Left, DejaRemplit.Bottom, RaR.Width, RaR.Bottom - DejaRemplit.Bottom))
                If RaR.Right <> DejaRemplit.Right Then _
                    ListeRectangle.Add(New Rectangle(DejaRemplit.Right, DejaRemplit.Top, RaR.Right - DejaRemplit.Right, DejaRemplit.Height))
                If RaR.Left <> DejaRemplit.Left Then _
                    ListeRectangle.Add(New Rectangle(RaR.Left, DejaRemplit.Top, DejaRemplit.Left - RaR.Left, DejaRemplit.Height))
                If RaR.Top <> DejaRemplit.Top Then _
                    ListeRectangle.Add(New Rectangle(RaR.Left, RaR.Top, RaR.Width, DejaRemplit.Top - RaR.Top))
                'pour chaque rectangle trouvé on repète le processus jusqu'à ce qu'il n'y ai plus de carte dans la liste ou que l'on
                'trouve une carte qui fasse toute la surface à remplir
                For Each R As Rectangle In ListeRectangle
                    'on continue la recherche récursive et on récupère le résultat
                    ResultatListesurfaceCarte.AddRange(TrouverCartesIntersectWithRectangles(R, RequeterCartesIntersectWithRectangle(R, LCIT)))
                Next
            End If
        End If
        Return ResultatListesurfaceCarte
    End Function
#End Region
#Region "IDisposable Support"
    Private disposedValue As Boolean ' Pour détecter les appels redondants
    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ''' <summary> libère si besoin la mémoire réservé par le tampon</summary>
                _Image.Dispose()
                Tampon.Dispose()
            End If
        End If
        disposedValue = True
    End Sub
    ' Ce code est ajouté par Visual Basic pour implémenter correctement le modèle supprimable.
    Friend Sub Dispose() Implements IDisposable.Dispose
        ' Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(disposing As Boolean) ci-dessus.
        Dispose(True)
    End Sub
#End Region
    ''' <summary>  classe qui permet de faire la relation entre une carte et un tampon  : intersection des 2 Georeférencement en monde virtuel </summary>
    Private Class CarteIntersectTampon
        ''' <summary>pour stocker la surface qui représente l'intersection d'une carte et du tampon virtuel</summary>
        Friend Intersect As Rectangle
        ''' <summary>true si la surface d'intersection est égale à la surface du tampon</summary>
        Friend IntersectTot As Boolean
        ''' <summary>Géoréférencement du tampon dans le système virtuel associé à l'échelle en cours</summary>
        Friend PositionGeoref As Rectangle
        ''' <summary>Nom de la carte yc le chemin complet, permet l'acces au fichier</summary>
        Friend Chemin As String
        ''' <summary>nb d'octets à lire pour passer à la ligne suivante : nb bits * nb bits du format graphique --> multiple de 4</summary>
        Friend LargeurCarte As Integer
        ''' <summary>  trouve la surface qui intersecte la carte et le tampon passé en paramètre </summary>
        ''' <param name="Region">Rectangle en monde virtuel désignant la région à tester</param>
        ''' <returns>la surface qui intersect la surface de la carte avec la surface du tampon passé en paramètre. Peut être vide</returns>
        Friend ReadOnly Property IntersectWithTampon(Region As Rectangle) As Rectangle
            Get
                Return Rectangle.Intersect(Intersect, Region)
            End Get
        End Property
    End Class
End Class