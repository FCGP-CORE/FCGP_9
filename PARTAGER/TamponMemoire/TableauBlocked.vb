''' <summary> definit un tableau d'octets avec une adresse mémoire fixe ce qui empêche le garbage collecteur de collecter ou de bouger l'emplacement du tableau </summary>
''' <remarks>il y a obligation d'appeler dispose pour libérer la mémoire </remarks>
Friend Class SharedPinnedByteArray
    Implements IDisposable
    ''' <summary> tableau de byte servant de tampon mémoire </summary>
    Friend ReadOnly Property Bits As Byte()
    ''' <summary> nombre d'octets constituant le tampon mémoire </summary>
    Friend ReadOnly Property NbBytes As Integer
        Get
            Return _NbOctetsLigne * _NbLignes
        End Get
    End Property
    ''' <summary> index de base dans le tableau constituant le tampon mémoire </summary>
    Friend ReadOnly Property IndexByte As Integer
    ''' <summary> index de fin dans le tableau constituant le tampon mémoire </summary>
    Friend Property IndexBytesDisponible As Integer
    ''' <summary> addresse du tampon mémoire </summary>
    Friend ReadOnly Property BitPtr() As IntPtr
    ''' <summary> nb de ligne indiqué à la création du tampon </summary>
    Friend ReadOnly Property NbLignes As Integer
    ''' <summary> nb d'octets par ligne indiqué à la création du tampon </summary>
    Friend ReadOnly Property NbOctetsLigne As Integer
    ''' <summary> indique si il reste des octets disponible par rapport à la réserve du départ </summary>
    Friend ReadOnly Property IsFull As Boolean
        Get
            Return _IndexBytesDisponible >= _IndexByte + _NbOctetsLigne * _NbLignes
        End Get
    End Property
    ''' <summary> indique le nombres d'octets disponible par rapport à la réserve du départ </summary>
    Friend ReadOnly Property ResteDisponible() As Integer
        Get
            Return _IndexByte + _NbOctetsLigne * _NbLignes - _IndexBytesDisponible
        End Get
    End Property
    ''' <summary> flag indiquant que le création du sharedPinned c'est bien passé</summary>
    Friend ReadOnly Property IsOk As Boolean
    ''' <summary> Nom du tampon. Sert à distinguer les sous tampons d'un tampon géneral</summary>
    Friend ReadOnly Property Nom As String
    ''' <summary> Crée un tableau d'octects ayant pour taille soit  NbOctetsParLigne-1 si NbdeLignes=1 ou
    ''' NbOctetsParLigne*NbdeLignes-1 si NbDeLignes>1. constructeur pour un tableau bloqué original </summary>
    ''' <param name="NbDeLignes"></param>
    ''' <param name="NbOctetsParLigne"> nombre d'octects par ligne. Si ligne = 1 cela équivaut à réserver une quantité de mémoire déconnectée de la taille réelle d'une image </param>
    ''' <param name="BitsShared"> tampon mémoire que l'on veut utilisé sinon il sera créé </param>
    Friend Sub New(NbOctetsParLigne As Integer, Optional NbDeLignes As Integer = 1, Optional BitsShared As SharedPinnedByteArray = Nothing, Optional NomTampon As String = "Carte")
        Try
            _Nom = NomTampon
            _NbLignes = NbDeLignes
            _NbOctetsLigne = NbOctetsParLigne
            If BitsShared Is Nothing Then
                'il faut créer un nouveau tampon
                _Bits = New Byte(NbBytes - 1) {}
                ' if not pinned the GC can move around the array
                handle = GCHandle.Alloc(Bits, GCHandleType.Pinned)
                _BitPtr = Marshal.UnsafeAddrOfPinnedArrayElement(Bits, 0)
                _IsOk = True
            Else
                'on se sert du tampon existant si il reste assez de place
                If BitsShared.ResteDisponible >= NbBytes Then
                    BaseTampon = BitsShared
                    _Bits = BitsShared.Bits
                    _IndexByte = BitsShared.IndexBytesDisponible
                    _BitPtr = BitsShared.BitPtr + BitsShared.IndexBytesDisponible - BitsShared.IndexByte
                    BitsShared.IndexBytesDisponible += NbBytes
                    _IndexBytesDisponible = _IndexByte
                    _IsOk = True
                Else
                    Throw New Exception("Le tampon support n'a pas la capacité suffisante")
                End If
            End If
        Catch Ex As Exception
            _Bits = Nothing
            handle = Nothing
            _BitPtr = IntPtr.Zero
            AfficherErreur(Ex, "R4F4")
        End Try
    End Sub
    ''' <summary> remplit le tampon avec la valeur indiquée </summary>
    ''' <param name="Valeur"> valeur pour remplir le tampon </param>
    Friend Sub ClearValeur(Valeur As Byte)
        InitialiserMemoire(_BitPtr, Valeur, _NbOctetsLigne * _NbLignes)
    End Sub
    ''' <summary> remplit une partie du tampon avec la valeur indiquée </summary>
    ''' <param name="Valeur"> valeur pour remplir le tampon </param>
    Friend Sub ClearValeur(Valeur As Byte, IndexDecalage As Integer, NbOctets As Integer)
        InitialiserMemoire(_BitPtr + IndexDecalage, Valeur, NbOctets)
    End Sub
    ''' <summary> remplit le tampon avec la couleur indiquée </summary>
    ''' <param name="Couleur"> couleur pour remplir le tampon</param>
    Friend Sub ClearColor(Couleur As Color)
        If (Couleur.B = Couleur.G) AndAlso (Couleur.B = Couleur.R) Then
            InitialiserMemoire(_BitPtr, Couleur.B, _NbOctetsLigne * _NbLignes)
        Else
            'remplir les 3 1ers octets (1er pixel) de la 1ère ligne avec la couleur
            _Bits(_IndexByte + 0) = Couleur.B
            _Bits(_IndexByte + 1) = Couleur.G
            _Bits(_IndexByte + 2) = Couleur.R
            'remplir le reste des pixels de la 1ère ligne avec la couleur désirée
            Dim index As Integer = 3
            Do While index <= _NbOctetsLigne \ 2
                CopierMemoire(_BitPtr + index, _BitPtr, index)
                index *= 2
            Loop
            CopierMemoire(_BitPtr + index, _BitPtr, _NbOctetsLigne - index)
            'remplir les autres lignes avec la 1ère ligne
            Parallel.For(1, _NbLignes,
                         Sub(Cpt)
                             CopierMemoire(_BitPtr + Cpt * _NbOctetsLigne, _BitPtr, _NbOctetsLigne)
                         End Sub)
        End If
    End Sub
    ''' <summary> remplit une partie du tampon avec la couleur indiquée </summary>
    ''' <param name="Couleur"> couleur pour remplir le tampon</param>
    Friend Sub ClearColor(Couleur As Color, IndexDecalage As Integer, NbOctets As Integer, Hauteur As Integer)
        Dim PointBase As IntPtr = _BitPtr + IndexDecalage
        If (Couleur.B = Couleur.G) AndAlso (Couleur.B = Couleur.R) Then
            'remplir les lignes avec la valeur
            Parallel.For(0, Hauteur,
                         Sub(Cpt)
                             InitialiserMemoire(PointBase + Cpt * _NbOctetsLigne, Couleur.B, NbOctets)
                         End Sub)
        Else
            'remplir les 3 1ers octets (1er pixel) de la 1ère ligne avec la couleur
            _Bits(_IndexByte + IndexDecalage + 0) = Couleur.B
            _Bits(_IndexByte + IndexDecalage + 1) = Couleur.G
            _Bits(_IndexByte + IndexDecalage + 2) = Couleur.R
            'remplir le reste des pixels de la 1ère ligne avec la couleur désirée
            Dim index As Integer = 3
            Do While index <= NbOctets \ 2
                CopierMemoire(PointBase + index, PointBase, index)
                index *= 2
            Loop
            CopierMemoire(PointBase + index, PointBase, NbOctets - index)
            'remplir les autres lignes avec la 1ère ligne
            Parallel.For(1, Hauteur,
                         Sub(Cpt)
                             CopierMemoire(PointBase + Cpt * _NbOctetsLigne, PointBase, NbOctets)
                         End Sub)
        End If
    End Sub
    ''' <summary> si le SPBA est vu comme recepteur, ce champ permet l'échange de données avec l'émmetteur </summary>
    Friend EcritureDonnees As DonneesImage
    ''' <summary> référence au tampon utilisé pour ce SPBA </summary>
    Private ReadOnly BaseTampon As SharedPinnedByteArray
    ''' <summary> permet que le GC ne modifie pas l'adresse du tampon en mémoire </summary>
    Private handle As GCHandle
    ''' <summary> initialise la structure de donnnées (paramètres) qui permettent l'écriture de données d'un tampon dans le fichier </summary>
    ''' <param name="DonneesTamponSource">source de données qui contient les données à lire fichier</param>
    ''' <param name="LargeurTamponReceveur">largeur du tampon receveur en pixel</param>
    ''' <param name="HauteurSource">hauteur du tampon source en pixel</param>
    ''' <param name="LargeurSource">largeur du tampon source en pixel</param>
    Friend Sub InitialiserEcritureDonnees(DonneesTamponSource As Object, LargeurSource As Integer, HauteurSource As Integer, LargeurTamponReceveur As Integer)
        EcritureDonnees = New DonneesImage(DonneesTamponSource, LargeurSource, HauteurSource, LargeurTamponReceveur)
        If EcritureDonnees.ChargeImage = TypeImage.Aucun Then EcritureDonnees = Nothing
    End Sub
    ''' <summary> écrit les données d'un tampon mémoire ou fichier BMP dans un le sharedpinnedByteArray actuel</summary>
    Friend Function EcrireDonnees(DecalTamponX As Integer, DecalTamponY As Integer) As Boolean
        If EcritureDonnees.ChargeImage = TypeImage.Fichier Then
            If EcritureDonnees.CheminImageSource.EndsWith("Bmp") Then
                Return LireOctetsFichierBMP(DecalTamponX, DecalTamponY)
            Else
                Return LireOctetsFichierBin(DecalTamponX, DecalTamponY)
            End If
        Else
            Return LireOctetsTampon(DecalTamponX, DecalTamponY)
        End If
    End Function
    ''' <summary> flag indiquant que le création  du sharedPinned c'est bien passé</summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        ' Dispose of unmanaged resources.
        Dispose(True)
        ' Suppress finalization.
        GC.SuppressFinalize(Me)
    End Sub
    ''' <summary> si true indique que l'objet a été déposé en attente d'un garbage collector </summary>
    Private destroyed As Boolean
    ''' <summary> restitue les ressources du sharedPinnedByteArray </summary>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If destroyed Then Exit Sub
        If BaseTampon Is Nothing Then
            If disposing Then
                _Bits = Nothing
            End If
            handle.Free()
            GC.Collect()
        Else
            BaseTampon.IndexBytesDisponible -= NbBytes
        End If
        destroyed = True
    End Sub
    ''' <summary> restitue les ressources du sharedPinnedByteArray mais par l'intermédiaire du garbage collector</summary>
    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub
    ''' <summary> copie tout ou partie d'une image en mémoire dans un tampon mémoire receveur. </summary>
    ''' <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon</param>
    ''' <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon</param>
    ''' <remarks>aucunes vérifications de fait dans cette fonction</remarks>
    Private Function LireOctetsTampon(Optional DecalTamponX As Integer = 0, Optional DecalTamponY As Integer = 0) As Boolean
        LireOctetsTampon = False
        Try
            With EcritureDonnees
                'on calcule l'index à partir duquel seront copiés dans le tampon, les octets lus à partir du fichier. 
                'Attention inversion des N° des lignes de pixel dans le fichier BMP (du bas en haut) en mémoire de haut en bas
                Dim IndexTamponReceveur As Integer = IndexByte + DecalTamponX * NbOctetsPixel + DecalTamponY * .NbOctectsLigneTampon
                'on calcule le nb d'octets à copier à partir du tampon source par ligne
                Dim NbOctectsCarteALire As Integer = .LargeurALire * NbOctetsPixel
                'index de départ du tampon source
                Dim IndexTamponsource = .IndexTamponImageSource + .DecalXImageSource * NbOctetsPixel + .DecalYImageSource * .NbOctetsLigneImageSource
                For BoucleLigne As Integer = .DecalYImageSource To .DecalYImageSource + .HauteurALire - 1
                    Buffer.BlockCopy(.TamponImageSource, IndexTamponsource, Bits, IndexTamponReceveur, NbOctectsCarteALire)
                    IndexTamponReceveur += .NbOctectsLigneTampon
                    IndexTamponsource += .NbOctetsLigneImageSource
                Next
            End With
            LireOctetsTampon = True
        Catch Ex As Exception
            AfficherErreur(Ex, "Y4U4")
        End Try
    End Function
    ''' <summary> Copie tout ou partie d'un fichier image BMP dans un tampon mémoire receveur </summary>
    ''' <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon</param>
    ''' <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon</param>
    ''' <remarks>les vérifications effectuées return false si erreur</remarks>
    Private Function LireOctetsFichierBMP(Optional DecalTamponX As Integer = 0, Optional DecalTamponY As Integer = 0) As Boolean
        LireOctetsFichierBMP = False
        Try
            With EcritureDonnees
                Using CarteStream As New FileStream(.CheminImageSource, FileMode.Open, FileAccess.Read, FileShare.Read)
                    'on calcule l'index à partir duquel seront copiés dans le tampon, les octets lus à partir du fichier. 
                    'Attention inversion des N° des lignes de pixel dans le fichier BMP (du bas en haut) en mémoire de haut en bas
                    Dim IndexTamponReceveur As Integer = IndexByte + (.HauteurALire + DecalTamponY - 1) * .NbOctectsLigneTampon + DecalTamponX * NbOctetsPixel
                    'on calcule le nb d'octets à copier du fichier dans le tampon par ligne
                    Dim NbOctectsCarteALire As Integer = .LargeurALire * NbOctetsPixel
                    'Offset de décalage des bits de données. Toujours 54 pour les images BMP
                    Dim IndexCarteSource As Integer = OffsetDataBMP + .DecalXImageSource * NbOctetsPixel +
                                                (.HauteurImageSource - .DecalYImageSource - .HauteurALire) * .NbOctetsLigneImageSource
                    For BoucleLigne As Integer = .DecalYImageSource To .DecalYImageSource + .HauteurALire - 1
                        CarteStream.Position = IndexCarteSource
                        CarteStream.Read(Bits, IndexTamponReceveur, NbOctectsCarteALire)
                        IndexTamponReceveur -= .NbOctectsLigneTampon
                        IndexCarteSource += .NbOctetsLigneImageSource
                    Next
                End Using
            End With
            LireOctetsFichierBMP = True
        Catch Ex As Exception
            AfficherErreur(Ex, "S5M6")
        End Try
    End Function
#Disable Warning IDE0060 ' Supprimer le paramètre inutilisé
    ''' <summary> Copie tout ou partie d'un fichier bin dans un tampon mémoire receveur </summary>
    ''' <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon</param>
    ''' <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon</param>
    ''' <remarks>les vérifications effectuées return false si erreur</remarks>
    Private Shared Function LireOctetsFichierBin(Optional DecalTamponX As Integer = 0, Optional DecalTamponY As Integer = 0) As Boolean
#Enable Warning IDE0060 ' Supprimer le paramètre inutilisé
        LireOctetsFichierBin = False
    End Function
End Class