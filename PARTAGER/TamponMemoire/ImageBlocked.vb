''' <summary>permet de créer un bitmap à partir d'un tableau de byte. cela permet d'accéder aux données bitmap à partir du tableau et vice versa. 
''' Les 2 sont manipulables en même temps. Pour le bitmap seul le format RVB sur 24 bits (8 bits ou 1 octet par couleur) </summary>
Friend Class EditableBitmap
    Implements IDisposable
    ''' <summary>tampon à adresse fixe en mémoire associé au bitmap.</summary>
    Private byteArray As SharedPinnedByteArray
    ''' <summary>flag indiquant si bitmap a été libéré</summary>
    Private m_disposed As Boolean
    ''' <summary> flag indiquant si l'EditableBitmap est correctement créé : Le tableau de byte est OK et le bitMap associé aussi </summary>
    Friend ReadOnly Property IsOk As Boolean
    ''' <summary>flag indiquant si bitmap a été libéré</summary>
    Friend ReadOnly Property Disposed() As Boolean
        Get
            Return m_disposed
        End Get
    End Property
    ''' <summary>nb d'octect pour lire une ligne du bitmap, forcément un multiple de 4</summary>
    Friend ReadOnly Property Stride() As Integer
    ''' <summary>indique le bitmap est associé à un shared bitArray ou à un BitmapData</summary>
    Friend ReadOnly Property IsBitmap() As Boolean
    ''' <summary>bitmap associé au tampon</summary>
    Private m_bitmap As Bitmap
    ''' <summary>bitmap associé au tampon</summary>
    Friend ReadOnly Property Bitmap() As Bitmap
        Get
            Return m_bitmap
        End Get
    End Property
    Private ReadOnly BmpData As BitmapData
    ''' <summary>tableau d'octets associé au tampon</summary>
    Friend ReadOnly Property Bits() As Byte()
        Get
            If _IsBitmap Then
                Return Nothing
            Else
                Return byteArray.Bits
            End If
        End Get
    End Property
    ''' <summary>Nombre d'octets du tableau d'octets associé au tampon</summary>
    Friend ReadOnly Property NbBits() As Integer
        Get
            If _IsBitmap Then
                Return Nothing
            Else
                Return byteArray.NbBytes
            End If
        End Get
    End Property
    ''' <summary>Index de départ dans le tableau d'octets associé au tampon</summary>
    Friend ReadOnly Property IndexBits() As Integer
        Get
            If _IsBitmap Then
                Return 0
            Else
                Return byteArray.IndexByte
            End If
        End Get
    End Property
    ''' <summary>pointeur sur le tampon</summary>
    Friend ReadOnly Property BitPtr() As IntPtr
        Get
            If _IsBitmap Then
                Return BmpData.Scan0
            Else
                Return byteArray.BitPtr
            End If
        End Get
    End Property
    ''' <summary>pointeur sur le tampon</summary>
    Friend ReadOnly Property DonneesChargementImage As DonneesImage
        Get
            Return byteArray.EcritureDonnees
        End Get
    End Property
    ''' <summary>pointeur sur le pixel en cours de traitement</summary>
    Friend Property IndexPixel As Integer
    ''' <summary>pointeur sur la ligne en cours de traitement</summary>
    Friend Property IndexLigne As Integer
    ''' <summary> initialise la couleur de l'image avec la couleur passée en paramètre </summary>
    Friend Sub ClearColor(Couleur As Color)
        byteArray.ClearColor(Couleur)
    End Sub
    ''' <summary> Creation d'un nouveau editable bitmap vierge avec les largeur, hauteur</summary>
    ''' <param name="width">largeur en pixel</param>
    ''' <param name="height">hauteur en pixel</param>
    ''' <remarks>sert pour la creation dynamique ou static d'un editablebitmap</remarks>
    Friend Sub New(width As Integer, height As Integer, BitsShared As SharedPinnedByteArray, Nom As String)
        'on transforme les pixels en nb d'octects à écrire dans le tampon  
        _Stride = StrideImage(width)
        byteArray = New SharedPinnedByteArray(_Stride, height, BitsShared, Nom)
        If byteArray.IsOk Then
            Try 'partie posant problème au niveau new d'un editablebitmap
                'le tampon a pu être créé, mais il faut aussi associé un bitmap au tampon
                m_bitmap = New Bitmap(width, height, _Stride, FormatPixel, byteArray.BitPtr)
                _IsBitmap = False
                _IsOk = True
            Catch Ex As Exception
                m_bitmap = Nothing
                byteArray.Dispose()
                AfficherErreur(Ex, "R4H7")
            End Try
        End If
    End Sub
    ''' <summary> créer un editablebitmap à partir d'une image existante</summary>
    ''' <param name="CheminImage"></param>
    Friend Sub New(CheminImage As String)
        Try 'partie posant problème au niveau new d'un editablebitmap
            'décompresse l'image de la carte en mémoire
            If Not File.Exists(CheminImage) Then Throw New Exception("Le fichier image n'existe pas")
            m_bitmap = New Bitmap(CheminImage)
            'on verrouille les données pour y avoir acces
            Dim rect As New Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height)
            BmpData = m_bitmap.LockBits(rect, ImageLockMode.ReadWrite, m_bitmap.PixelFormat)
            _Stride = BmpData.Stride
            _IsBitmap = True
            _IsOk = True
        Catch Ex As Exception
            m_bitmap = Nothing
            MessageInformation = "Erreur dans Sub New EditableBitmap"
            AfficherErreur(Ex, "E4P7")
        End Try
    End Sub
#Region "Complément à Editablebitmap original"
    ''' <summary>charge une image dans le bitmap. Les dimensions du bitmap et de l'image doivent être identiques</summary>
    ''' <param name="CheminImage">image à charger</param>
    Friend Sub ChargerImage(CheminImage As String)
        _IsOk = False
        Try
            'décompresse l'image de la carte en mémoire
            Using B As New Bitmap(CheminImage)
                'on verrouille les données pour y avoir acces
                Dim rect As New Rectangle(0, 0, B.Width, B.Height)
                Dim bmpData As BitmapData = B.LockBits(rect, ImageLockMode.ReadOnly, B.PixelFormat)
                ' on copie les données dans notre tampon
                Marshal.Copy(bmpData.Scan0, Bits, 0, Bits.Length)
                ' on libère les ressources non managées.
                B.UnlockBits(bmpData)
            End Using
            _IsOk = True
        Catch Ex As Exception
            AfficherErreur(Ex, "Y9U2")
        End Try
    End Sub
    ''' <summary>initialise les différents paramètres qui permettront l'écriture des données graphiques dans le tableau de byte</summary>
    ''' <param name="TamponDonneesSource"></param>
    ''' <param name="LargeurSource">Largeur en pixel de l'image source</param>
    ''' <param name="HauteurSource">Hauteur en pixel de l'iamge source</param>
    ''' <param name="LargeurTampon">Largeur en pixel du tampon receveur</param>
    ''' <returns></returns>
    Friend Function InitialiserChargementImage(TamponDonneesSource As Object, LargeurSource As Integer, HauteurSource As Integer, LargeurTampon As Integer) As ErreurEditableBitmap
        byteArray.InitialiserEcritureDonnees(TamponDonneesSource, LargeurSource, HauteurSource, LargeurTampon)
        If byteArray.EcritureDonnees Is Nothing Then
            Return ErreurEditableBitmap.Tampon_Donnees_Incompatible
        Else
            Return ErreurEditableBitmap.Ok
        End If
    End Function
    ''' <summary>
    ''' Charge une image physique dans le tampon mémoire sans blocage de la ressource image.Permet à travers 
    ''' la strucuture CarteImage d'échanger les diverses infos nécessaires au bon fonctionnement
    ''' </summary>
    ''' <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon receveur</param>
    ''' <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon receveur</param>
    ''' <remarks>les vérifications effectuées return false si erreur</remarks>
    Friend Function ChargerImage(Optional DecalTamponX As Integer = 0, Optional DecalTamponY As Integer = 0) As ErreurEditableBitmap
        If DonneesChargementImage Is Nothing Then
            Return ErreurEditableBitmap.Donnees_Image_Non_Renseignees
        End If
        With DonneesChargementImage
            If .ChargeImage = TypeImage.Aucun Then
                Return ErreurEditableBitmap.ChargeCarte_Non_Renseigne
            End If
            If .ChargeImage = TypeImage.Fichier And .CheminImageSource = Nothing Then
                Return ErreurEditableBitmap.CheminCarte_Non_Renseigne
            End If
            If .ChargeImage = TypeImage.Memoire And .TamponImageSource Is Nothing Then
                Return ErreurEditableBitmap.TamponSource_Non_Renseigne
            End If
            If DecalTamponX < 0 OrElse DecalTamponX > Bitmap.Width - 1 Then
                Return ErreurEditableBitmap.DecalTamponX_incompatible_tampon
            End If
            If DecalTamponX + .LargeurALire > Bitmap.Width Then
                Return ErreurEditableBitmap.DecalTamponX_LargeurALire_incompatible_tampon
            End If
            If DecalTamponY < 0 Or DecalTamponY > Bitmap.Height - 1 Then
                Return ErreurEditableBitmap.DecalTamponY_incompatible_tampon
            End If
            If DecalTamponY + .HauteurALire > Bitmap.Height Then
                Return ErreurEditableBitmap.DecalTamponY_HauteurALire_incompatible_tampon
            End If
            If .DecalXImageSource + .LargeurALire > .LargeurImageSource Then
                Return ErreurEditableBitmap.DecalCarteX_LargeurALire_incompatible_Image
            End If
            If .DecalYImageSource + .HauteurALire > .HauteurImageSource Then
                Return ErreurEditableBitmap.DecalCarteY_HauteurALire_incompatible_Image
            End If
        End With
        'on copie les données sous forme d'octets
        If byteArray.EcrireDonnees(DecalTamponX, DecalTamponY) Then
            Return ErreurEditableBitmap.Ok
        Else
            Return ErreurEditableBitmap.Erreur_Fonction
        End If
    End Function
#End Region
    ''' <summary> permet de libérer les ressources associées à l'EditableBitmap </summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        'nécessaire pour éviter que le GC ne redemande Finalize()
        GC.SuppressFinalize(Me)
    End Sub
    ''' <summary>permet de libérer les ressources associées à l'EditableBitmap</summary>
    Private Sub Dispose(disposing As Boolean)
        'si l'objet est déja déposé onsort
        If m_disposed Then Exit Sub
        'sinon on libère le bitmap si demandé 'sinon on libère le bitmap si demandé
        If _IsBitmap Then
            m_bitmap.UnlockBits(BmpData)
        Else
            'si le dimensionnement du tampon est dynamique on récupère la mémoire
            byteArray.Dispose()
            byteArray = Nothing
        End If
        m_disposed = True
        If disposing Then
            If m_bitmap IsNot Nothing Then m_bitmap.Dispose() 'If Not IsNothing(m_bitmap) Then m_bitmap.Dispose()
            m_bitmap = Nothing
        End If
    End Sub
    ''' <summary>appeler par le GC si l'objet n'a plus d'attache</summary>
    Protected Overrides Sub Finalize()
        Try
            Dispose(False)
        Finally
            MyBase.Finalize()
        End Try
    End Sub
End Class