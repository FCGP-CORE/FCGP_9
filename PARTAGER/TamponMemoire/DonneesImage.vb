''' <summary>  sert pour le pasage de paramètres des fonctions ChargerImageFichier et ChargerImageEditableBitmap ''' </summary> 
Friend Class DonneesImage
    ''' <summary>initialise l'instance: structure de données qui sert de paramètre lors de la copie d'une image sur une autre </summary>
    ''' <param name="TamponDonneesSource"></param>
    ''' <param name="LargeurSource">largeur en pixel du tampon source</param>
    ''' <param name="HauteurSource">hauteur en pixel du tampon source</param>
    ''' <param name="LargeurTampon">largeur en pixel du tampon receveur</param>
    Friend Sub New(TamponDonneesSource As Object, LargeurSource As Integer, HauteurSource As Integer, LargeurTampon As Integer)
        If TypeOf TamponDonneesSource Is Byte() Then
            ChargeImage = TypeImage.Memoire
            TamponImageSource = CType(TamponDonneesSource, Byte())
            CheminImageSource = Nothing
        ElseIf TypeOf TamponDonneesSource Is String Then
            ChargeImage = TypeImage.Fichier
            CheminImageSource = CType(TamponDonneesSource, String)
            TamponImageSource = Nothing
        Else
            ChargeImage = TypeImage.Aucun
            Exit Sub
        End If
        'par defaut il n'y a pas de décalage
        DecalXImageSource = 0
        'par defaut il n'y a pas de décalage
        DecalYImageSource = 0
        'hauteur de l'image en pixel
        HauteurImageSource = HauteurSource
        'largeur de l'image en pixel
        LargeurImageSource = LargeurSource
        'par defaut la hauteur à lire est la l'hauteur de l'image
        HauteurALire = HauteurSource
        'par defaut la largeur à lire est la largeur de l'image
        LargeurALire = LargeurSource
        'doit être un multipe de 4, appelé stride en anglais : nb d'octets pour passer à la ligne suivante de l'image
        NbOctetsLigneImageSource = StrideImage(LargeurSource)
        'doit être un multipe de 4, appelé stride en anglais, par défaut le même que la source mais ce n'est pas une obligation : nb d'octets pour passer à la ligne suivante de l'image
        NbOctectsLigneTampon = StrideImage(LargeurTampon)
    End Sub
    ''' <summary>tampon contenant l'image de la carte à lire</summary>
    Friend TamponImageSource As Byte()
    ''' <summary>index dans le tampon général où commence le tampon contenant l'image de la carte à lire</summary>
    Friend IndexTamponImageSource As Integer
    ''' <summary>flux contenant l'image de la carte à lire</summary>
    Friend CheminImageSource As String
    ''' <summary>largeur en pixel de l'image</summary>
    Friend LargeurImageSource As Integer
    ''' <summary>hauteur en pixel de l'image</summary>
    Friend HauteurImageSource As Integer
    ''' <summary>nb d'octets à lire pour passer à la ligne suivante dans le flux doit être un multiple de 4</summary>
    Friend NbOctetsLigneImageSource As Integer
    ''' <summary>nb de pixel à lire sur la ligne</summary>
    Friend LargeurALire As Integer
    ''' <summary>nb de ligne (pixel) à lire de l'image</summary>
    Friend HauteurALire As Integer
    ''' <summary>décalage en pixel en début de ligne</summary>
    Friend DecalXImageSource As Integer
    ''' <summary>décalage en ligne (pixel) en partant du haut de l'image</summary>
    Friend DecalYImageSource As Integer
    ''' <summary>sert pour l'écriture des données dans un SharedPinnedByteArray ou d'un BMPWriter
    ''' différencier un donnéesImage Carte d'un DonneesImage Tampon</summary>
    Friend ChargeImage As TypeImage
    ''' <summary>nb d'octets à lire pour passer à la ligne suivante dans le tampon receveur doit être un multiple de 4
    ''' sert principalement quand le SBA n'est qu'un réservoir d'octects (fichier)</summary>
    Friend NbOctectsLigneTampon As Integer
End Class
