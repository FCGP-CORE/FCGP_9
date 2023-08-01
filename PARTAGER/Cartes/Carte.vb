Friend Class Carte
    Implements IDisposable
#Region "constructeurs, fonctions et méthodes accessibles"
    ''' <summary> Création de la carte pour les dimensions indiquées et avec soit un tampon dynamique pour contenir les octets de l'image, soit un tampon pour fichier
    ''' le reste des champs accessibles reste à initialiser par ailleurs. Ce constructeur est utilisé exclusivement lors de la capture d'une carte. </summary>
    ''' <param name="NbX"> nb de tuiles serveur en horizontal </param>
    ''' <param name="NbY"> nb de tuiles serveur en vertical </param>
    ''' <param name="SystemeCarto"> système cartographique de la carte </param>
    ''' <param name="Nomcarte"> Nom de la carte </param>
    ''' <remarks> appel de la procédure FinaliserCarte obligatoire après l'initialisation des différentes variables publiques </remarks>
    Friend Sub New(NbX As Integer, NbY As Integer, SystemeCarto As SystemeCartographique, NomCarte As String)
        'appel du constructeur qui initialise la structure georef des 4 coins de la carte
        Me.New()
        Try
            Nom = NomCarte
            _LargeurPixel = NbX * NbPixelsTuile
            _HauteurPixel = NbY * NbPixelsTuile
            _LargeurOctets = StrideImage(_LargeurPixel)
            SystemCartographique = SystemeCarto
            _NomAjout = SuffixeNom(SystemCartographique)
            TamponType = CartesSettings.SUPPORT_CARTE
            ToutFormatCarte = CartesSettings.IS_TOUT_FORMAT
            Separateur = " "
            Format = ConvertIntToImageFormat(CartesSettings.FORMAT)
            CheminCarte = CapturerSettings.CHEMIN_CARTE
            DemandeAfficherGrille = CartesSettings.IS_GRILLE
            DemandeAjouterCoordonneesGrille = If(CartesSettings.IS_REFERENCES, ChoixCoordonneesGrille.Ref_Carte, ChoixCoordonneesGrille.Aucune)
            'infos concernant  les fichiers tuiles
            DemandeFichiersTuiles = CType(CartesSettings.FICHIER_TUILE, ChoixFichiersTuiles)
            CheminFichiersTuile = PartagerSettings.CHEMIN_TUILE
            FacteurJNX = CartesSettings.FACTEUR_JNX
            FacteurORUX = CartesSettings.FACTEUR_ORUX

            If Not ReserverMemoireTampon("Reserve Carte Base") Then Exit Try
            If TamponType <> TypeSupportCarte.Fichier Then
                'on associe directement le tampon car c'est le même pour la capture et le traitement
                Tampon = New EditableBitmap(_LargeurPixel, _HauteurPixel, _Reserve, "Tampon Base")
            Else
                'la hauteur max du tampon de travail dépend de la taille effective de la réserve
                HauteurReserve = TailleReserve \ _LargeurOctets
                CheminFluxLecture = CheminEnregistrementProvisoire & "\CarteSansGrille.raw"
            End If
            _IsOk = True

        Catch Ex As Exception
            AfficherErreur(Ex, "T9B7")
        End Try
    End Sub
    ''' <summary> Création de la carte pour les dimensions indiquées et avec un support d'image fichier
    ''' le reste des champs accessibles reste à initialiser par ailleurs. Ce constructeur est utilisé 
    ''' exclusivement lors  de l'aglomération de plusieurs cartes à la demande d'un regroupement </summary>
    ''' <param name="LargeurCarte">largeur en pixel de la carte</param>
    ''' <param name="HauteurCarte">hauteur en pixel de la carte</param>
    ''' <param name="SystemeCarto">systeme cartographique de la carte</param>
    ''' <param name="FichierBin">chemin du fichier qui contient les données binaires de la carte</param>
    ''' <remarks>appel de la procédure FinaliserCarte obligatoire après l'initialisation des différentes variables publiques</remarks>
    Friend Sub New(LargeurCarte As Integer, HauteurCarte As Integer, SystemeCarto As SystemeCartographique, FichierBin As String, TailleReserveMax As Integer)
        'appel du constructeur qui initialise la strucutre georef des 4 coins de la carte
        Me.New()
        Try
            _TailleReserve = TailleReserveMax
            If Not ReserverMemoireTampon("Reserve Carte Base") Then Exit Try
            _IsOk = _Reserve.IsOk
            _LargeurPixel = LargeurCarte
            _HauteurPixel = HauteurCarte
            _LargeurOctets = StrideImage(_LargeurPixel)
            HauteurReserve = _HauteurPixel
            SystemCartographique = SystemeCarto
            _NomAjout = SuffixeNom(SystemCartographique)
            TamponType = TypeSupportCarte.Fichier 'obligatoirement support fichier car normalement on va travailler sur de grandes quantités de données
            ToutFormatCarte = False 'forcément BMP mais normalement on est pas censé enregistrer la carte car on fait uniquement les fichiers tuiles
            CheminFluxLecture = FichierBin
        Catch Ex As Exception
            AfficherErreur(Ex, "C4C5")
        End Try
    End Sub
    ''' <summary> permet de libérer les ressources associées à l'EditableBitmap </summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        'nécessaire pour éviter que le GC ne redemande Finalize()
        GC.SuppressFinalize(Me)
    End Sub
    ''' <summary>permet de remplir les coins de la carte d'après les coordonnées sous forme de texte. Doit être appelée obligatoirement après un New Friend</summary>
    ''' <param name="Coordonnees">coordonnées sous forme de texte</param>
    ''' <param name="TexteInformation">texte à afficher dans la boite d'information pendant le travail de la fonction</param>
    Friend Function FinaliserCarte(Coordonnees() As String, TexteInformation As String) As Boolean
        FinaliserCarte = False
        Try
            AfficherVisueInformation(TexteInformation)
            'transforme les coordonnées capturées en coordonnées réelles WGS84 DD et UTM zone 30 à 32 GF, 20, 22 ou 40 DT
            For Cpt As Integer = 0 To 3
                Coins(Cpt).CoordonneesCaptureEcran = Coordonnees(Cpt)
                'on calcule les pixels en X et Y correspondant au coin en cours de traitement
                Select Case Cpt
                    Case 0
                        _Coins(Cpt).X_Pixel = 0
                        _Coins(Cpt).Y_Pixel = 0
                    Case 1
                        _Coins(Cpt).X_Pixel = _LargeurPixel
                        _Coins(Cpt).Y_Pixel = 0
                    Case 2
                        _Coins(Cpt).X_Pixel = _LargeurPixel
                        _Coins(Cpt).Y_Pixel = _HauteurPixel
                    Case 3
                        _Coins(Cpt).X_Pixel = 0
                        _Coins(Cpt).Y_Pixel = _HauteurPixel
                End Select
                If Not DecomposerCoordonneesCapturees(SystemCartographique, Coins(Cpt)) Then Exit Try 'il peut y avoir une erreur
            Next
            'Combien de zones UTM différentes pour la carte : de 1 à 3 au maximum (Echelle 1/500000) (pour le dessin de la grille)
            If Not TrouverNbZonesUtm() Then Exit Try
            'pour le calcul des dpi impression de l'image
            If Not TrouverDPIImpression() Then Exit Try
            If TamponType = TypeSupportCarte.Fichier Then 'on différencie le support de carte car quand il y a capture 
                'avec un support de carte fichier on a besoin d'un tampon spécifique mais si la réserve de base est trop importante on en prend qu'une partie
                HauteurReserve = _TailleReserve \ _LargeurOctets
                'on associe un tampon à la carte
                Tampon = New EditableBitmap(_LargeurPixel, HauteurReserve, _Reserve, "Tampon Base")
                If Not Tampon.IsOk Then Exit Try
            Else
                'on indique les DPI au niveau de l'image pour qu'ils soient enregistrés et avoir une impression qui corresponde à l'échelle prévue
                Image.SetResolution(DPIImpressionX, DPIImpressionY)
            End If
            If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille Then
                PointOrigne = New Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins(0).Grille))
            Else
                PointOrigne = New Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins(0).LatLon))
            End If
            _RegionVirtuelle = New Rectangle(New Point(PointOrigne), New Size(_LargeurPixel, _HauteurPixel))
            'on calcul les points de la grille et on la dessine si elle est demandée
            GrilleIsAffiche = SystemCartographique.GrilleExiste
            'et enregistrer les différents fichiers
            If Not RealiserDemandes() Then Exit Try 'il y a eu une erreur
            FinaliserCarte = True
        Catch Ex As Exception
            AfficherErreur(Ex, "U1F0")
        End Try
    End Function
    ''' <summary> permet la création des différents fichiers associés à une carte </summary>
    Friend Function RealiserDemandes() As Boolean
        RealiserDemandes = False
        Try
            If DemandeAfficherTrace AndAlso FichiersTRK IsNot Nothing Then
                If Not AfficherTraces() Then Exit Try 'il peut y avoir une erreur  
            End If
            If DemandeAfficherGrille AndAlso Not GrilleIsAffiche Then 'c'est le cas de Géofoncier
                If Not AfficherGrille() Then Exit Try 'il peut y avoir une erreur  
            End If
            GrilleIsAffiche = SystemCartographique.AfficherGrilleIsOK And GrilleIsAffiche
            If DemandeFichiersGeoref > ChoixFichiersGeoref.Aucun Then
                If Not GenererFichiersGeoreferencement() Then Exit Try
            End If
            'génerer l'interpolation de la carte et les différents fichiers si demandé
            If DemandeFichiersTuiles > ChoixFichiersTuiles.ORUX OrElse
               DemandeFichiersGeoref > ChoixFichiersGeoref.Cartes OrElse
               DemandeAjouterCoordonneesGrille > ChoixCoordonneesGrille.Ref_Carte Then
                'il faut à minima demander l'interpolation de la carte
                If Not GenererInterpolation() Then Exit Try
            ElseIf DemandeFichiersTuiles > ChoixFichiersTuiles.Aucun Then
                If Not GenererFichiersTuiles() Then Exit Try
            End If
            'faire les références de la carte source si demandé et la grille aussi. Pas de références sans grille
            If DemandeAjouterCoordonneesGrille.HasFlag(ChoixCoordonneesGrille.Ref_Carte) AndAlso GrilleIsAffiche Then
                If Not AjouterReferences() Then Exit Try
            End If
            RealiserDemandes = True
        Catch Ex As Exception
            AfficherErreur(Ex, "A3G6")
        End Try
    End Function
#End Region
#Region "Champs accessibles"
    ''' <summary> lors de la génération des fichiers tuiles, de l'interpolation de la carte, indique combien il y a d'iterations totales à accomplir </summary>
    Friend NbTotalIterations As Integer
    ''' <summary> lors de la génération des fichiers tuiles, de l'interpolation de la carte, indique combien il y a d'iterations déjà accompli </summary>
    Friend NbIterations As Integer
    ''' <summary> pour les messages d'information. correspond au début de la chaine d'info </summary>
    Friend MessInfo As String
    ''' <summary> pour les messages d'information. permet soit de continuer sur la même ligne avec " " ou à la ligne suivante avec CrLf </summary>
    Friend Separateur As String
    ''' <summary> structure qui enregistre les données du système cartographique de la carte </summary>
    Friend SystemCartographique As SystemeCartographique
    ''' <summary>chemin d'enregistrement de la carte si demandé</summary>
    Friend CheminCarte As String
    ''' <summary>chemin d'enregistrement du fichier tuile si demandé</summary>
    Friend CheminFichiersTuile As String
    ''' <summary>nom de la carte sans extension</summary>
    Friend Nom As String
    ''' <summary> format d'enregistrement de l'image de la carte par exemple BMP </summary>
    Friend Format As ImageFormat
    ''' <summary>on demande à ce que la ou les traces soit affichées sur la carte</summary>
    Friend DemandeAfficherTrace As Boolean
    ''' <summary>on demande à ce que la grille soit affichée sur la carte</summary>
    Friend DemandeAfficherGrille As Boolean
    ''' <summary>indique si l'on doit créer un ou des fichiers tuiles</summary>
    Friend DemandeFichiersTuiles As ChoixFichiersTuiles
    ''' <summary>indique si l'on doit ajouter un niveau à un fichier JNX ou ORUX si True ou créer un fichier JNX ou ORUX si false </summary>
    Friend DemandeAjoutNiveau As Boolean
    ''' <summary>indique si un fichier images JPEG contenant les références de la grille doit être inclus dans les fichiers générés</summary>
    Friend DemandeAjouterCoordonneesGrille As ChoixCoordonneesGrille
    ''' <summary>indique les fichiers georef associés à la carte demandés par l'utilisateur</summary>
    Friend DemandeFichiersGeoref As ChoixFichiersGeoref
    ''' <summary>Largeur et hauteur en pixels des tuiles composant un fichier tuile</summary>
    Friend DimensionsTuile As Integer
    ''' <summary>facteur multiplicateur pour l'échelle maximum de visualisation d'un niveau par rapport à l'original</summary>
    Friend FacteurJNX As Double
    ''' <summary>facteur de correction pour l'échelle maximum de visualisation d'un niveau par rapport à l'original</summary>
    Friend FacteurORUX As Integer
    ''' <summary>chemin associé à l'image de travail encours en cas de support de carte typetampon fichier ou en remplissage d'un tampon</summary>
    Friend CheminFluxLecture As String
    ''' <summary> stocke les tuiles pour les fichiers tuiles. </summary>
    Friend Tuiles() As DescriptionImageFichier
    ''' <summary> listes des traces à dessiner sur la carte </summary>
    Friend FichiersTRK As List(Of String)
    ''' <summary> pinceau qui va servir à dessiner la trace sur les cartes sur la carte </summary>
    Friend PinceauTrace As Pen
    ''' <summary> combien de fois doit'on inserer les points d'une trace dans le fichier KMZ : 0 à 2 </summary>
    Friend NbTracesKML As Integer
#End Region
#Region "Propriétés accessibles"
    ''' <summary>chemin relatif où seront enregistrées les tuiles de l'image de la carte</summary>
    Friend Shared ReadOnly Property ExtensionCheminImagesTuiles As String
        Get
            Return ExtensionCheminTuiles & ExtensionCheminFichiers
        End Get
    End Property
    ''' <summary> nb d'octects associés aux tampons de travail pour les supports de carte type Fichier et SmallMemory </summary>
    Friend ReadOnly Property TailleReserve As Integer
    ''' <summary>tampon reserve (mémoire) pour les supports de carte type Fichier et SmallMemory des cartes interpolées</summary>
    Friend ReadOnly Property Reserve As SharedPinnedByteArray
    ''' <summary>Nb de mètres par pixel sur l'axe des X </summary>
    Friend ReadOnly Property Echelle_M_PixelX() As Double
        Get
            Return LargeurMetres / LargeurPixel     'En mètre par pixel
        End Get
    End Property
    ''' <summary>Nb de mètres par pixel sur l'axe des Y </summary>
    Friend ReadOnly Property Echelle_M_PixelY() As Double 'peut être déclarée Private mais pour avoir la concordance avec Echelle_M_PixelX() 
        Get
            Return HauteurMetres / HauteurPixel     'En mètre par pixel
        End Get
    End Property
    ''' <summary> Nb de tuiles sur l'axe des X pour un fichier tuile</summary>
    Friend ReadOnly Property NbTuilesX As Integer
    ''' <summary> Nb de tuiles sur l'axe des Y pour un fichier tuile</summary>
    Friend ReadOnly Property NbTuilesY As Integer
    ''' <summary>  </summary>
    Friend ReadOnly Property NomAjout As String
    ''' <summary>  </summary>
    Friend ReadOnly Property Coins As GeoRef()
    ''' <summary>  </summary>
    Friend ReadOnly Property LargeurPixel As Integer
    ''' <summary>  </summary>
    Friend ReadOnly Property HauteurPixel As Integer
    ''' <summary> nom complet de la carte. Le suffixe indique si il s'agit d'une carte interpolée ou pas </summary>
    Friend ReadOnly Property NomComplet() As String
        Get
            Return Nom & _NomAjout
        End Get
    End Property
    ''' <summary> flag indiquant si la création de la carte est ok. Ce flag dépend de la manière dont est créée la carte</summary>
    Friend ReadOnly Property IsOk As Boolean
    ''' <summary>Image de la carte, attention il s'agit de l'image associé au tampon de la carte. En cas de support de carte Fichier, le tampon n'est pas une image</summary>
    Friend ReadOnly Property Image() As Bitmap
        Get
            Return Tampon.Bitmap
        End Get
    End Property
    ''' <summary>pointeur sur le tableau d'octects qui correspond à l'Image de la carte</summary>
    Friend ReadOnly Property ImageBitPtr() As IntPtr
        Get
            Return Tampon.BitPtr
        End Get
    End Property
    ''' <summary>tableau d'octects qui correspond à l'Image de la carte</summary>
    Friend ReadOnly Property ImageBits() As Byte()
        Get
            Return Tampon.Bits
        End Get
    End Property
    Friend Sub ColorerTampon(Couleur As Color)
        Tampon.ClearColor(Couleur)
    End Sub
    ''' <summary>Nb d'octet à lire pour passer à la ligne suivante (3 octets par pixel + complément de 0 à 3 octets pour arriver à un nb d'octets qui soit un multiple de 4</summary>
    Friend ReadOnly Property LargeurOctets As Integer
    ''' <summary> X,Y, Largeur, hauteur de la carte en monde réel exprimé en unité du système géographique (DD ou M)</summary>
    Friend ReadOnly Property RegionReelle() As RectangleD
        Get
            If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD Then
                Return New RectangleD(_Coins(0).LatLon, _Coins(2).LatLon)
            Else
                Return New RectangleD(_Coins(0).Grille, _Coins(2).Grille)
            End If
        End Get
    End Property
    ''' <summary>region commune entre RegionVirtuelle et la région passée en paramètre</summary>
    Friend ReadOnly Property CroiserAvecRegionVirtuelle(Region As Rectangle) As Rectangle
        Get
            Return Rectangle.Intersect(_RegionVirtuelle, Region)
        End Get
    End Property
    ''' <summary>X,Y par rapport à l'origine système et Largeur, hauteur de la carte exprimé en Pixel du monde virtuel</summary>
    Friend ReadOnly Property RegionVirtuelle() As Rectangle
    ''' <summary>Sud de la carte exprimé en pixel du monde virtuel, équivaut à Y + Hauteur ou  Bottom de la position Georef</summary>
    Friend ReadOnly Property Sud() As Integer
        Get
            Return PointOrigne.Height + _HauteurPixel
        End Get
    End Property
    ''' <summary> Nord de la carte exprimé en pixel du monde virtuel, équivaut à Y de la position Georef</summary>
    Friend ReadOnly Property Nord() As Integer
        Get
            Return PointOrigne.Height
        End Get
    End Property
    ''' <summary>Ouest de la carte exprimé en pixel du monde virtuel, équivaut à X de la position Georef</summary>
    Friend ReadOnly Property Ouest() As Integer
        Get
            Return PointOrigne.Width
        End Get
    End Property
    ''' <summary>Est de la carte exprimé en pixel du monde virtuel, équivaut à X + Largeur ou Right de la position Georef</summary>
    Friend ReadOnly Property Est() As Integer
        Get
            Return PointOrigne.Width + _LargeurPixel
        End Get
    End Property
#End Region
#Region "fonctions partagées accessibles"
    ''' <summary>renvoie une carte si le système carto du fichier georef est correct et correspond au systeme carto de référence. sert lors de l'ajout d'une carte dans une echelle</summary>
    ''' <param name="FichierGeoref">Fichier Géoref à vérifier</param>
    ''' <param name="SupportTampon">type de support tampon souhaité</param>
    ''' <param name="NumCarte">Numéro de la carte à indiquer dans le champ correspondant uniquement si le programme appelant gére ce numéro pour une collection</param>
    ''' <param name="Syst_Carto">systeme géographique au quel doit correspondre celui de la carte</param>
    ''' <param name="Ret">Code d'erreur</param>
    ''' <returns>CarteAjout crée et renseignée  ou nothing si le systeme Cartographique du fichier Georef est différent de celui passé en paramètre</returns>
    ''' <remarks> L'image est associée si le support de la carte est différent de Aucun </remarks>
    Friend Shared Function RenvoyerCarte(FichierGeoref As String, ByRef Ret As VerificationRenvoiCarte,
                                         Optional SupportTampon As TypeSupportCarte = TypeSupportCarte.Aucun,
                                         Optional ToutFormat As Boolean = True,
                                         Optional Syst_Carto As SystemeCartographique = Nothing,
                                         Optional FichiersTuiles As ChoixFichiersTuiles = ChoixFichiersTuiles.Aucun,
                                         Optional NumCarte As Integer = -1) As Carte
        Dim Car_Aj As New Carte(FichierGeoref)
        Ret = VerificationRenvoiCarte.OK
        If Not Car_Aj.IsOk Then
            Ret = VerificationRenvoiCarte.Erreur_Creation_Systeme
            Car_Aj.Dispose()
            Return Nothing
        End If
        If Syst_Carto IsNot Nothing AndAlso (Car_Aj.SystemCartographique.ClefEchelle <> Syst_Carto.ClefEchelle OrElse Car_Aj.SystemCartographique.IsInterpol) Then
            Ret = VerificationRenvoiCarte.Systeme_Different
            Car_Aj.Dispose()
            Return Nothing
        End If
        Car_Aj.TamponType = SupportTampon
        Car_Aj.ToutFormatCarte = ToutFormat
        Car_Aj.DemandeFichiersTuiles = FichiersTuiles
        Car_Aj.NumId = NumCarte
        If SupportTampon <> TypeSupportCarte.Aucun Then
            Ret = Car_Aj.AssocierTampon
            If Ret <> VerificationRenvoiCarte.OK Then
                Car_Aj.Dispose()
                Return Nothing
            End If
        End If
        Return Car_Aj
    End Function
    ''' <summary> Sert à renvoyer un système géographique correspondant au fichier Georef si celui ci est valide </summary>
    ''' <param name="FichierGeoref">Fichier Géoref à vérifier</param>
    ''' <returns>Ok si tout c'est bien passé sinon description de l'erreur</returns>
    ''' <remarks>SystemeEchelle est indefinit en cas d'erreur</remarks>
    Friend Shared Function RenvoyerSystemeCartographique(FichierGeoref As String) As SystemeCartographique
        Dim SiteCarto As String = Nothing, Projection As String = Nothing, ClefEchelle As String = Nothing, Z As Integer = 0, H As Char = Nothing, Interpol As Boolean = False
        'Lire l'ensemble du fichier georef
        Dim LignesFichierGeoref() As String = File.ReadAllLines(FichierGeoref, Encoding_FCGP)
        TrouverElementsSystemeCartographique(LignesFichierGeoref, SiteCarto, ClefEchelle, Projection, Z, H, Interpol)
        If Interpol Then Return Nothing
        RenvoyerSystemeCartographique = New SystemeCartographique(SiteCarto, ClefEchelle, Projection, Z, H)
    End Function
#End Region
End Class