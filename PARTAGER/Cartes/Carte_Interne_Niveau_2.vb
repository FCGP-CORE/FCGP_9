Partial Friend Class Carte
#Region "Structures internes"
    ''' <summary> permet le dessin de la grille ou l'ajout des étiquettes de la grille. </summary>
    ''' <remarks> Chaque point est exprimé en pixel par rapport au coin N°0 </remarks>
    Private Class Grille
        ''' <summary>Tableau qui stockent l'ensemble des points qui composent la grille</summary>
        Friend Points As Point(,)
        ''' <summary>Indice de début de boucle sur les colonnes de la carte (axe des X)</summary>
        Friend MinX As Integer
        ''' <summary>Indice de fin de boucle sur les colonnes de la carte (axe des X)</summary>
        Friend MaxX As Integer
        ''' <summary>Indice de fin de boucle sur les lignes de la carte (axe des Y)</summary>
        Friend MinY As Integer
        ''' <summary>Indice de début de boucle sur les lignes de la carte (axe des Y)</summary>
        Friend MaxY As Integer
        ''' <summary>N°de pixel en deça duquel le dessin ne sera pas effectif sur l'axe des X</summary>
        Friend ClipZoneDeb As Integer
        ''' <summary>N°de pixel au dela duquel le dessin ne sera pas effectif sur l'axe des X</summary>
        Friend ClipZoneFin As Integer
    End Class
    ''' <summary>Variables spécifiques pour l'interpolation des cartes avec grille : rendu obligatoire pour les calculs multi threads</summary>
    Private Class ICAG
        ''' <summary>pas de longitude en DD/Pixels de la carte interpolée</summary>
        Friend InterDeltaLong As Double
        ''' <summary>pas de latitude en DD/Pixels de la carte interpolée</summary>
        Friend InterDeltaLat As Double
        ''' <summary>Coordonnée X en pixel source pour chaque pas en longitude de la carte interpolée</summary>
        Friend CoordX(,) As Double
        ''' <summary>Coordonnée Y en pixel source pour chaque pas en latitude de la carte interpolée</summary>
        Friend CoordY(,) As Double
        ''' <summary>nb de colonnes des coordonnées sur l'axe des X</summary>
        Friend NbPasX As Integer
        ''' <summary>pas de la grille des coordonnées en pixels sur l'axe des X</summary>
        Friend PasX As Integer
        ''' <summary>nb de colonnes des coordonnées sur l'axe des Y</summary>
        Friend NbPasY As Integer
        ''' <summary>pas de la grille des coordonnées en pixels sur l'axe des Y</summary>
        Friend PasY As Integer
        ''' <summary>Fraction entre les valeurs de colonnes Deltamax et DeltaMax+1  dans CoordY qui contient les latitudes min de la carte (Fin de carte)</summary>
        Friend DeltaLatMax As Double
        ''' <summary>index de la colonne dans CoordY qui contient les latitudes maximum de la carte</summary>
        Friend IndexLatMax As Integer
        ''' <summary>Fraction entre les valeurs de colonnes Deltamin et DeltaMin+1  dans CoordY qui contient les latitudes min de la carte (Fin de carte)</summary>
        Friend DeltaLatMin As Double
        ''' <summary>index de la colonne dans CoordY qui contient les latitudes minimum de la carte</summary>
        Friend IndexLatMin As Integer
        ''' <summary>latitude minimale de début de tranche</summary>
        Friend DebutSource As Integer
        ''' <summary>index sup de la ligne interpolée de fin de tranche dans la table CoordY</summary>
        Friend IndexLigneInterpole As Integer
        ''' <summary> index du 1er octet dans le tableau </summary>
        Friend IndexBits As Integer
        ''' <summary> tableau d'octects de la carte interpole </summary>
        Friend Bits As Byte()
        ''' <summary> trouve l'index de la latitude maximale </summary>
        Friend Sub TrouverIndexLatMaxInterpole(HauteurSource As Integer)
            For Cpt As Integer = IndexLigneInterpole To CoordY.GetUpperBound(1)
                If CoordY(IndexLatMax, Cpt) >= HauteurSource Then
                    IndexLigneInterpole = Cpt
                    Exit Sub
                End If
            Next
        End Sub
    End Class
    ''' <summary>Enumération liés au retour de la fonction interpole une carte</summary>
    Private Enum ErreurInterpolation
        ''' <summary>l'interpolation de la carte c'est bien déroulée</summary>
        Ok = 0
        ''' <summary>l'interpolation de la carte ne c'est pas bien déroulée</summary>
        NotOk = 1
        ''' <summary>la carte n'appartient pas à un système qui supporte l'interpolation</summary>
        PasConcerne = 2
    End Enum
#End Region
#Region "Constructeurs internes"
    ''' <summary> constructeur commun </summary>
    Private Sub New()
        ReDim _Coins(3) 'on dimensionne pour mettre à 0 les différente variables pour les 4 coins de la carte
        For Cpt = 0 To 3
            'on dimensionne pour remettre à 0 les coordonnées UTM des différentes zones, maximum 3
            _Coins(Cpt) = New GeoRef(3)
        Next
        'si la carte fait partie d'une collection. Le numéro est géré directement par le programme appelant
        NumId = -1
        'toute les autres variables friend sont à initialiser par le programme
    End Sub
    ''' <summary> Ce constructeur sert pour renvoyer une carte existante </summary>
    ''' <param name="Fichiergeoref">chemin complet du fichier georef</param>
    ''' <remarks>on regarde uniquement la validité du fichier Georef</remarks>
    Private Sub New(Fichiergeoref As String)
        Me.New()
        'Lire l'ensemble du fichier georef
        Dim LignesFichierGeoref() As String = File.ReadAllLines(Fichiergeoref, Encoding_FCGP)
        CheminCarte = Path.GetDirectoryName(Fichiergeoref)
        _IsOk = False
        If VerifierGeoref(LignesFichierGeoref, SystemCartographique) = VerificationRenvoiCarte.OK Then
            'initialiser les variables de base de la carte
            Dim S() As String = CheminFluxLecture.Split("."c)
            CheminFluxLecture = If(String.IsNullOrEmpty(CheminCarte), "", CheminCarte & "\") & CheminFluxLecture
            CheminFichiersTuile = CheminCarte 'initialisé par défaut
            Nom = NomSansSuffixe(S(0))
            _NomAjout = SuffixeNom(SystemCartographique)
            Format = ConvertStringToImageFormat(S(1))
            _IsOk = LireFichierGeoref(LignesFichierGeoref)
        End If
    End Sub
#End Region
#Region "Variables et constantes internes"
    ''' <summary> stocke les coordonnées des points de la grille pour le dessin. 1 grille par zone UTM </summary>
    Private Grilles As Grille()
    ''' <summary> représente dans le système virtuel la position en pixel du coin(0) de la carte. </summary>
    Private PointOrigne As Size
    ''' <summary>Nb de zone UTM de la carte. De 1 à 3 pour GF, de 1 à 2 pour DT et SM</summary>
    Private NbZonesUTM As Integer
    ''' <summary>dans le cas d'une carte existante indique si la grille est dessinée, dans le cas d'une capture indique si la grille a été dessinée sur la carte ''' </summary>
    Private GrilleIsAffiche As Boolean
    ''' <summary>Largeur en mètres  la carte. A partir des coordonnées capturées pour les cartes avec Grille ou DD pour les cartes Mercator ou interpolée </summary>
    Private LargeurMetres As Double
    ''' <summary>Hauteur en mètres de la carte. A partir des coordonnées capturées pour les cartes avec Grille ou DD pour les cartes Mercator ou interpolée </summary>
    Private HauteurMetres As Double
    ''' <summary> Couleur d'un pixel interpoler en dehors de la carte </summary>
    Private PixelInterpol() As Byte
    ''' <summary>Numéro de la carte. Cela permet d'avoir une cle unique dans une collection de carte (Echelle)</summary>
    Private NumId As Integer
    ''' <summary>indique le support de traitement des cartes</summary>
    Private TamponType As TypeSupportCarte
    ''' <summary>indique pour le support fichier si on autorise tous les formats d'enregistrement de la carte ou uniquement BMP (pas besoin de mémoire)</summary>
    Private ToutFormatCarte As Boolean
    ''' <summary>Hauteur en pixel de la reserve de la carte, la largeur étant toujours le nb octets de la largeur de la carte</summary>
    Private HauteurReserve As Integer
    ''' <summary>Tampon de stockage de l'image de la carte</summary>
    Private Tampon As EditableBitmap
    ''' <summary> Nb de DPI pour obtenir l'échelle d'impression sur les X</summary>
    Private DPIImpressionX As Single
    ''' <summary> Nb de DPI pour obtenir l'échelle d'impression sur les Y</summary>
    Private DPIImpressionY As Single
    ''' <summary> indique si la carte a été détruite</summary>
    Private disposed As Boolean
    Private Const ExtensionCheminTuiles As String = "\tuiles"
    Private Const ExtensionCheminFichiers As String = "\files"
    Private Const Suffixe_Interpol As String = "_WGS84"
    Private Const Suffixe_Grille As String = "_LV03"
    Private Const Suffixe_DD As String = "_Mercator"
#End Region
#Region "procédure internes 02"
#Region "Tampon mémoire <--> fichier raw ou BMP"
    ''' <summary>réalise la copie des données image du fichier source dans le fichier destination en inversant les lignes de pixel</summary>
    ''' <param name="NomFichierDst">chemin complet du fichier destination résultant de la conversion</param>
    ''' <remarks> cette procédure ne sert que pour le support fichier </remarks>
    Private Function FichierSrcToDst(NomFichierDst As String, FlagFormatBMP As Boolean) As Boolean
        FichierSrcToDst = False
        Try
            'calcul de la hauteur des 2 tampons pour réaliser le transfert
            Dim HauteurTravail = If(HauteurReserve / 2 > HauteurPixel, HauteurPixel, HauteurReserve \ 2)
            'on divise le tampon en 2 pour faire le transfert
            Dim TailleTravail As Integer = _LargeurOctets * HauteurTravail
            Using FichierDst As New FileStream(NomFichierDst, FileMode.Create, FileAccess.Write, FileShare.None)
                If FlagFormatBMP Then EcrireEnteteFichierBMP(FichierDst)
                EcrireDonneesFichierSrcToFichierDst(FichierDst, TailleTravail, HauteurTravail)
            End Using
            FichierSrcToDst = True
        Catch Ex As Exception
            AfficherErreur(Ex, "U3Q7")
        End Try
    End Function
    ''' <summary> écrit les données d'un fichier raw (binaire) dans un fichier BMP (inversion des lignes de pixels)</summary>
    ''' <param name="FichierDst">fichier qui va recevoir les données</param>
    ''' <param name="TailleTampon">nombre d'octets réservés pour le transfert de fichier à fichier</param>
    Private Sub EcrireDonneesFichierSrcToFichierDst(FichierDst As FileStream, TailleTampon As Integer, HauteurTampon As Integer)
        Try
            Using FichierSrc As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                'on commence par la fin du fichier
                Dim PointeurFichierSrc As Long = FichierSrc.Seek(0, SeekOrigin.End)
                Dim TamponTransfert = Tampon.Bits
                Dim TamponSrcIndex = Tampon.IndexBits
                Dim TamponDstIndex = Tampon.IndexBits + TailleTampon
                Dim PointeurTamponSrc, PointeurTamponDst, HauteurALire As Integer
                Do
                    If HauteurALire + HauteurTampon < HauteurPixel Then
                        HauteurALire += HauteurTampon
                    Else
                        HauteurTampon = HauteurPixel - HauteurALire
                        TailleTampon = LargeurOctets * HauteurTampon
                        HauteurALire = HauteurPixel
                    End If
                    'on retir la taille du tampon à la base de la dernière lecture 
                    PointeurFichierSrc -= TailleTampon
                    FichierSrc.Seek(PointeurFichierSrc, SeekOrigin.Begin)
                    'on lit le nb d'octets dans la partie du tampon réservé à Src
                    FichierSrc.Read(TamponTransfert, TamponSrcIndex, TailleTampon)
                    PointeurTamponSrc = TamponSrcIndex + TailleTampon
                    PointeurTamponDst = TamponDstIndex
                    For Cpt = 0 To HauteurTampon - 1 'pour chaque ligne
                        PointeurTamponSrc -= LargeurOctets
                        Buffer.BlockCopy(TamponTransfert, PointeurTamponSrc, TamponTransfert, PointeurTamponDst, LargeurOctets)
                        PointeurTamponDst += LargeurOctets
                    Next
                    'on écrit la part dont les ligne ont été inversées
                    FichierDst.Write(TamponTransfert, TamponDstIndex, TailleTampon)
                Loop While HauteurALire <> HauteurPixel
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "W3K9")
        End Try
    End Sub
    ''' <summary> écrit dans le fichier BMP passé en paramètre l'entête du fichier en fonction des différents éléments de la carte</summary>
    Private Sub EcrireEnteteFichierBMP(FichierBMP As FileStream)
        Dim TailleImage As UInteger = CUInt(CUInt(LargeurOctets) * HauteurPixel)
        Dim Reserved(7) As Byte
        Dim Marque() As Byte = Encoding.UTF8.GetBytes("BM")
        FichierBMP.Write(Marque, 0, Marque.Length) 'marque des fichiers BMP : 2 octets
        FichierBMP.Write(BitConverter.GetBytes(TailleImage + OffsetDataBMP), 0, 4) 'taille du fichier : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(0), 0, 4) 'réservé 2*2 octets : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(OffsetDataBMP), 0, 4) 'offset image : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(40), 0, 4) 'taille structure : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(LargeurPixel), 0, 4) 'largeur image : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(HauteurPixel), 0, 4) 'hauteur image : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(1), 0, 2) 'nb de plan : 2 octets
        FichierBMP.Write(BitConverter.GetBytes(24), 0, 2) 'nb de bits par pixels : 2 octets
        FichierBMP.Write(BitConverter.GetBytes(0), 0, 4) 'compression de l'image : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(TailleImage), 0, 4) 'taille de l'image : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(DPIImpression(DPIImpressionX)), 0, 4) 'DpiX de l'image : 4 octets
        FichierBMP.Write(BitConverter.GetBytes(DPIImpression(DPIImpressionY)), 0, 4) 'DpiY de l'image : 4 octets
        FichierBMP.Write(Reserved, 0, Reserved.Length) 'réservé : 8 octets
        FichierBMP.Flush()
    End Sub
#End Region
#Region "fichier RAW <--> Fichier Image (Bmp, Png, Jpeg)"
    ''' <summary>réalise la conversion d'un fichier image au format BMP, PNG ou JPG en un fichier au format RAW </summary>
    ''' <param name="NomFichierBIN">chemin complet du fichier Raw résultant de la conversion</param>
    Private Function FichierFormatsToRaw(NomFichierBIN As String) As Boolean
        FichierFormatsToRaw = False
        Try
            'décompresse l'image de la carte en mémoire
            Using m_bitmap As New Bitmap(CheminFluxLecture)
                'on verrouille les données pour y avoir acces
                Dim BD As BitmapData = m_bitmap.LockBits(New Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height), ImageLockMode.ReadOnly, m_bitmap.PixelFormat)
                Using F As New FileStream(NomFichierBIN, FileMode.Create, FileAccess.Write)
                    For Cpt As Integer = 0 To BD.Stride * BD.Height - 1
                        F.WriteByte(Marshal.ReadByte(BD.Scan0, Cpt))
                    Next
                End Using
                m_bitmap.UnlockBits(BD)
            End Using
            FichierFormatsToRaw = True
        Catch Ex As Exception
            AfficherErreur(Ex, "E0E4")
        End Try
    End Function
    ''' <summary>enregistre un fichier binaire en tant qu'image et avec un format spécifique. Utilise en mémoire la taille de l'image soit jusqu'à 1.5 Go</summary>
    ''' <param name="NomFichierDst"> chemin d'enregistrement de la carte</param>
    ''' <param name="FlagJpeg">indique si true que le format Jpeg est obligatoire sinon on utilise le format de la carte</param>
    Private Function FichierRawToFormats(NomFichierDst As String, FlagJpeg As Boolean) As Boolean
        FichierRawToFormats = False
        Try
            Using B As New EditableBitmap(LargeurPixel, HauteurPixel, Nothing, "Tampon Convertion Image")
                B.Bitmap.SetResolution(DPIImpressionX, DPIImpressionY)
                Using FichierBIN As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                    FichierBIN.Read(B.Bits, 0, B.Bits.Length)
                End Using
                If Format Is ImageFormat.Jpeg OrElse FlagJpeg Then
                    B.Bitmap.Save(NomFichierDst, EncodeurJpeg, QualiteJPEG(PartagerSettings.QUALITE_JPEG))
                Else
                    B.Bitmap.Save(NomFichierDst, Format)
                End If
            End Using
            FichierRawToFormats = True
        Catch Ex As Exception
            AfficherErreur(Ex, "V2T6")
        End Try
    End Function
#End Region
#Region "Décompose coordonnées référencement d'un fichier Georef"
    ''' <summary>Décompose les coordonnées capturées sous forme de chaine  et les transforme en coordonnées numériques et en DD WGS84</summary>
    ''' <param name="Site"> site carto des coordonnées </param>
    ''' <param name="Coin">structure Georef qui contient les coordonnées capturées à décomposer</param>
    Private Shared Function DecomposerCoordonneesCapturees(Site As SystemeCartographique, Coin As GeoRef) As Boolean
        DecomposerCoordonneesCapturees = False
        Try
            With Coin
                Dim D As Datums = Site.Projection.Datum, Z As Integer = Site.ZoneUtmReferencement, H As Char = Site.HemisphereUtmReferencement
                If Site.Projection.UniteCoordonnees = UnitesCoordonnees.DMS Then
                    'si les coordonnées sont de format longitude latitude
                    If DecomposerCoordonneesDMS(Coin) Then
                        If D <> Datums.WGS84 Then 'si les coordonnées DD ne sont pas en WGS84 on fait la transformation
                            Dim Ret As PointD = ConvertDatumToDatum(D, Coin.LatLon, Datums.WGS84, True)
                            If Ret.IsEmpty Then
                                Dim Ex As New Exception("DecomposerCoordonneesDMS && ConvertDatumToDatum" & CrLf & "retourne une coordonnée vide.")
                                AfficherErreur(Ex, "N3L5")
                                Exit Try
                            End If
                            Coin.LatLon = Ret
                        End If
                        DecomposerCoordonneesCapturees = True
                    End If
                Else
                    'si les coordonnées sont en mètres
                    If DecomposerCoordonneesMetres(Coin) Then
                        If D = Datums.Grille_Suisse Then
                            LV95ToLV03(Coin.Grille)
                        End If
                        Dim Ret = ConvertProjectionToWGS84(D, New PointProjection(Coin.Grille, Z, H))
                        If Ret.IsEmpty Then
                            Dim Ex As New Exception("DecomposerCoordonneesMetres && ConvertProjectionToWGS84" & CrLf & "retourne une coordonnée vide.")
                            AfficherErreur(Ex, "ACE9")
                            Exit Try
                        End If
                        Coin.LatLon = Ret
                        DecomposerCoordonneesCapturees = True
                    End If
                End If
            End With
        Catch Ex As Exception
            AfficherErreur(Ex, "X6J8")
        End Try
    End Function
    ''' <summary>Décompose une chaine de caractères copiée à partir du site GéoPortail qui contient une coordonnée en degrès, minutes, secondes
    ''' et renvoie cette coordonnée sous forme de double en degrés. Il y a une distinction entre longitude en début de chaine ou 
    ''' latitude en fin de chaine. Format chaine attendu : E:04°51'10.4"N:45°31'02.8" (pas d'espace)
    ''' ou : E: 4° 51' 10.4''   N: 45° 31' 02.8'' (avec espaces)</summary>
    ''' <param name="Coin"> coin sur lequel on travail</param>
    Private Shared Function DecomposerCoordonneesDMS(ByRef Coin As GeoRef) As Boolean
        DecomposerCoordonneesDMS = False
        Try
            With Coin
                Dim CoordonneesDMS As String = .CoordonneesCaptureEcran
                'si ça ne commence pas par une lettre de Longitude ou si la longueur est <= à 25 il y a un problème
                If Not ((CoordonneesDMS.StartsWith("E:") OrElse CoordonneesDMS.StartsWith("O:")) AndAlso CoordonneesDMS.Length >= 25) Then Exit Try
                Dim Negatif As Double = 1.0
                Dim Pos2 As Integer = CoordonneesDMS.IndexOf(":"c)
                Dim Pos1 As Integer = CoordonneesDMS.IndexOf("°"c)
                Dim Pos3 As Integer = CoordonneesDMS.IndexOf("'"c)
                'D'abord la longitude c'est au début de la chaine des coordonnées
                Dim Deg As Double = CDbl(CoordonneesDMS.Substring(Pos2 + 1, Pos1 - Pos2 - 1))          'les degrés sont enregistrés sous forme d'entier. pas de risque d'erreur
                Dim Minute As Double = CDbl(CoordonneesDMS.Substring(Pos1 + 1, Pos3 - Pos1 - 1))       'les minutes sont enregistrées sous forme d'entier. pas de risque d'erreur
                Pos1 = CoordonneesDMS.IndexOf(""""c)
                Dim Seconde As Double = StrToDbl(CoordonneesDMS.Substring(Pos3 + 1, Pos1 - Pos3 - 1))  'les secondes sont enregistrés sous forme de float xx.x
                'A l'ouest il s'agit d'une longitude négative
                If CoordonneesDMS.StartsWith("O") Then Negatif = -1.0
                .Longitude = (Deg + Minute / 60 + Seconde / 3600) * Negatif
                'Puis la latitude c'est à la fin de la chaine des coordonnées
                Negatif = 1.0
                Pos2 = CoordonneesDMS.IndexOf(":"c, Pos2 + 1)
                Pos1 = CoordonneesDMS.IndexOf("°"c, Pos2)
                Pos3 = CoordonneesDMS.IndexOf("'"c, Pos2)
                'Si il s'agit de la latitude c'est à la fin de la chaine des coordonnées
                Deg = CDbl(CoordonneesDMS.Substring(Pos2 + 1, Pos1 - Pos2 - 1))                'les degrés sont enregistrés sous forme d'entier. pas de risque d'erreur
                Minute = CDbl(CoordonneesDMS.Substring(Pos1 + 1, Pos3 - Pos1 - 1))             'les minutes sont enregistrées sous forme d'entier. pas de risque d'erreur
                Pos1 = CoordonneesDMS.IndexOf(""""c, Pos2)
                Seconde = StrToDbl(CoordonneesDMS.Substring(Pos3 + 1, Pos1 - Pos3 - 1))        'les secondes sont enregistrés sous forme de float xx.x
                'Au sud il s'agit d'une latitude négative
                If CoordonneesDMS.Chars(Pos2 - 1) = "S"c Then Negatif = -1.0
                .Latitude = (Deg + Minute / 60 + Seconde / 3600) * Negatif
            End With
            DecomposerCoordonneesDMS = True
        Catch Ex As Exception
            AfficherErreur(Ex, "H0H4")
        End Try
    End Function
    ''' <summary> Décompose une chaine de caractères copiée à partir du site GéoPortail ou de suisse mobile qui contient des coordonnées en mètres
    ''' et renvoie ces coordonnées sous forme de double. Format chaine attendu : Coordonnées(m):657954,188821 sans espace
    ''' ou E:796746.25N:2060585.36 sans espace, en fait un groupe de caractères suivi d'un nombre le 1er groupe représente les X et le 2ème les Y 
    ''' sans séparateur suivi d'un groupe de caractères suivi d'un nombre sans séparateur</summary>
    ''' <param name="Coin">coin sur lequel on travaille</param>
    Private Shared Function DecomposerCoordonneesMetres(ByRef Coin As GeoRef) As Boolean
        DecomposerCoordonneesMetres = False
        Dim CoordonneesGrille As String
        Try
            Const SeparteurCoord As String = ","
            With Coin
                If .CoordonneesCaptureEcran.IndexOf("N:") <> -1 Then
                    'remplace le N: des captures de coordonnées des sites GF, DT en UTM WGS84 ou Lambert 93 des versions 6 et moins
                    CoordonneesGrille = .CoordonneesCaptureEcran.Replace("N:", SeparteurCoord).Trim
                ElseIf .CoordonneesCaptureEcran.IndexOf("Y:") <> -1 Then
                    'remplace le Y: des captures de coordonnées des sites GF, DT en UTM WGS84 ou grilles web des versions 8.5 et plus
                    CoordonneesGrille = .CoordonneesCaptureEcran.Replace(" Y:", "").Trim
                Else
                    CoordonneesGrille = .CoordonneesCaptureEcran.Trim
                End If

                Dim Pos1 As Integer
                Dim Nombre = TrouverChaineCompriseEntre(CoordonneesGrille, Pos1, ":"c, SeparteurCoord(0))
                .X_Grid = StrToDbl(Nombre)
                Nombre = TrouverChaineCompriseEntre(CoordonneesGrille, Pos1, SeparteurCoord(0))
                .Y_Grid = StrToDbl(Nombre)
            End With
            DecomposerCoordonneesMetres = True
        Catch Ex As Exception
            AfficherErreur(Ex, "L2C6")
        End Try
    End Function
#End Region
#Region "coordonnées référencement --> UTM et DPI"
    ''' <summary>Cette procédure calcule le nb de zones UTM de la carte.
    ''' Elle calcule les X_UTM et Y_UTM pour l'ensemble des zones UTM.
    ''' Zone 30 ,31 et 32 pour la France métropolitaine et la Corse. Cela permet de faire le dessin d'une grille UTM si le fond de carte n'en a pas une</summary>
    ''' <remarks>Prend en charge le fait que la carte peut être à cheval sur plus d'1 zone UTM</remarks>
    Private Function TrouverNbZonesUtm() As Boolean
        TrouverNbZonesUtm = False
        Try
            'on détermine le nb de zones en faisant la différence des zones + un
            NbZonesUTM = CInt(Math.Floor(Coins(2).Longitude / 6) - Math.Floor(Coins(0).Longitude / 6) + 1)
            'on calcule la 1ère zone utm commune à toute les coins, le premier appel à la fonction ConvertDatumToUtm détermine le numéro mini de zone UTM
            For Cpt = 0 To 3
                If Cpt > 0 Then
                    Coins(Cpt).NumZone_UTM = Coins(0).NumZone_UTM
                End If
                Dim UTM = ConvertLLToUtm(Coins(Cpt).LatLon, Datums.UTM_WGS84, Coins(0).NumZone_UTM, True)
                If UTM.Coordonnees.IsEmpty Then
                    Dim Ex As New Exception("Erreur dans la procédure ConvertLLToUtm")
                    AfficherErreur(Ex, "A44C")
                    Exit Try
                Else
                    Coins(Cpt).NumZone_UTM = UTM.Zone
                    Coins(Cpt).Hemisphere_UTM = UTM.Hemisphere
                    Coins(Cpt).UTMs(0) = UTM.Coordonnees
                End If
            Next
            'on calcule les coordonnées UTM des autres zones afin de permettre le dessin de la grille UTM pour l'ensemble des zones
            For Boucle As Integer = 1 To NbZonesUTM - 1
                For Cpt = 0 To 3
                    Dim UTM = ConvertLLToUtm(Coins(Cpt).LatLon, Datums.UTM_WGS84, Coins(Cpt).NumZone_UTM + Boucle, True)
                    If UTM.Coordonnees.IsEmpty Then
                        Dim Ex As New Exception("Erreur dans la procédure ConvertLLToUtm")
                        AfficherErreur(Ex, "CA48")
                        Exit Try
                    Else
                        Coins(Cpt).UTMs(Boucle) = UTM.Coordonnees
                    End If
                Next
            Next
            TrouverNbZonesUtm = True
        Catch Ex As Exception
            AfficherErreur(Ex, "X7Q8")
        End Try
    End Function
    ''' <summary>calcul le nb de metres par pixel. Assume des pixels carrés pour la carte </summary>
    Private Function TrouverDPIImpression() As Boolean
        TrouverDPIImpression = False
        Try
            Dim DimensionsCarte As (Largeur As Double, Hauteur As Double)
            Dim EchelleImpression As String = SystemCartographique.Niveau.ImpressionClef
            If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.DD OrElse SystemCartographique.IsInterpol Then
                'on estime les dimensions de la carte par rapport aux latitudes/Longitudes des coins car elle est calée dessus
                DimensionsCarte = CalculerDimensionsCarte(Coins(0).LatLon, Coins(2).LatLon, True)
            Else 'c'est le sytème suisse non interpolé calé sur la grille du système carto
                DimensionsCarte = CalculerDimensionsCarte(Coins(0).Grille, Coins(2).Grille, False)
            End If
            LargeurMetres = DimensionsCarte.Largeur
            HauteurMetres = DimensionsCarte.Hauteur
            DPIImpressionX = CSng(1 / Echelle_M_PixelX * StrToDbl(EchelleImpression) * InchToMm)
            DPIImpressionY = CSng(1 / Echelle_M_PixelY * StrToDbl(EchelleImpression) * InchToMm)
            TrouverDPIImpression = True
        Catch Ex As Exception
            AfficherErreur(Ex, "EDA8")
        End Try
    End Function
    ''' <summary>transforme un nb de dpi en résolution par mètres</summary>
    ''' <param name="DPI">DPI à transformer</param>
    Private Shared Function DPIImpression(DPI As Single) As Integer
        Return CInt(Math.Floor(DPI * 10 / CinchToMm))
    End Function
#End Region
#Region "fonctions liées au nom de carte"
    ''' <summary> Renvoie un suffixe en fonction du site et de l'interpolation de la carte </summary>
    Private Shared Function SuffixeNom(SystemCartographique As SystemeCartographique) As String
        If SystemCartographique.IsInterpol Then
            SuffixeNom = Suffixe_Interpol
        ElseIf SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille Then
            SuffixeNom = Suffixe_Grille
        Else
            SuffixeNom = Suffixe_DD
        End If
    End Function
    ''' <summary> renvoie le nom de la carte sans suffixe. sert pour les fichiers georef qui ont un nom de carte avec suffixe depuis la version 7.000 </summary>
    Private Shared Function NomSansSuffixe(Nom As String) As String
        Dim Pos As Integer = Nom.LastIndexOf("_"c)
        Dim Suffixe As String = Nothing
        If Pos > -1 Then Suffixe = Nom.Substring(Pos)

        If (Suffixe = Suffixe_Grille OrElse Suffixe = Suffixe_DD OrElse Suffixe = Suffixe_Interpol) Then
            NomSansSuffixe = Nom.Substring(0, Pos)
        Else
            NomSansSuffixe = Nom
        End If
    End Function
#End Region
    ''' <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If disposed Then Exit Sub
        If disposing Then
            PinceauTrace?.Dispose()
            Tampon?.Dispose()
            _Reserve?.Dispose()
            disposed = True
        End If
    End Sub
#End Region
#Region "Functions internes liées à l'interpolation d'une carte"
    ''' <summary>  interpole la carte avec la méthode bilineaire et correction d'erreur </summary>
    ''' <remarks>La carte interpolée aura les mêmes dimensions en pixel que la carte d'origine</remarks>
    Private Function InterpolerCarte(CarteInterpole As Carte) As ErreurInterpolation
        InterpolerCarte = ErreurInterpolation.NotOk
        Try
            MessInfo = $"{NomComplet} :{Separateur}Interpolation de la carte :{Separateur}"
            'on initialise la carte interpolée avec les principales variables de la carte source
            If Not InitialiserCarteInterpole(CarteInterpole) Then Exit Try

            Dim HauteurTravail = If(CarteInterpole.TamponType = TypeSupportCarte.Fichier, HauteurReserve, _HauteurPixel)
            'on associe un tampon à la carte
            CarteInterpole.Tampon = New EditableBitmap(_LargeurPixel, HauteurTravail, CarteInterpole._Reserve, "Tampon Interpole")
            'pour éviter de bloquer les boucles parallel, il ne faut pas faire appel à l'objet bitmap du tampon de la carte interpolée
            If CarteInterpole.SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile Then
                If InterpolerCarteAvecGrille(CarteInterpole) <> ErreurInterpolation.Ok Then Exit Try
            Else
                If InterpolerCarteSansGrille(CarteInterpole) <> ErreurInterpolation.Ok Then Exit Try
            End If
            If CarteInterpole.TamponType <> TypeSupportCarte.Fichier Then
                'Pour l'impression de l'image de la carte à la bonne échelle on affecte les DPI à l'image
                CarteInterpole.Image.SetResolution(CarteInterpole.DPIImpressionX, CarteInterpole.DPIImpressionY)
            End If
            InterpolerCarte = ErreurInterpolation.Ok
        Catch Ex As Exception
            AfficherErreur(Ex, "C2L2")
        End Try
    End Function
    ''' <summary> copie la carte source et recherche les coordonnées longitude, latitude pour préparer l'interpolation de la carte </summary>
    Private Function InitialiserCarteInterpole(CarteInterpole As Carte) As Boolean
        'remplit tous les champs directs
        With CarteInterpole
            .Separateur = Separateur
            ._HauteurPixel = _HauteurPixel
            .HauteurReserve = HauteurReserve
            ._LargeurPixel = _LargeurPixel
            .Format = Format
            .Nom = Nom
            .CheminCarte = CheminCarte
            .CheminFichiersTuile = CheminFichiersTuile
            .GrilleIsAffiche = GrilleIsAffiche
            .DemandeAfficherGrille = DemandeAfficherGrille
            .DemandeAjouterCoordonneesGrille = DemandeAjouterCoordonneesGrille
            .DemandeFichiersGeoref = DemandeFichiersGeoref
            .DemandeFichiersTuiles = DemandeFichiersTuiles
            .DemandeAjoutNiveau = DemandeAjoutNiveau
            .FacteurJNX = FacteurJNX
            .FacteurORUX = FacteurORUX
            .PointOrigne = Size.Empty
            .NbZonesUTM = NbZonesUTM
            .DimensionsTuile = DimensionsTuile
            .TamponType = TamponType
            .ToutFormatCarte = ToutFormatCarte
            .NbTracesKML = NbTracesKML
            .FichiersTRK = FichiersTRK
            ._RegionVirtuelle = _RegionVirtuelle
            ._LargeurOctets = _LargeurOctets
            ._TailleReserve = _TailleReserve
            Dim R As RectangleD
            If SystemCartographique.CoordonneesGeoreferencement = CoordonneesGeoreferencements.Grille Then
                'trouve les coordonnees des 4 coins de la carte interpolée en LL. On prend en compte le rectangle maximum contenu dans les LL de la cartesource
                'on garde les mêmes dimensions pour les pixels ce qui induit de fait un changement d'échelle et du blanc qui apparait sur les bords du à la distorsion
                'de l'image
                TrouverCoordonnesCarteAvecGrille(CarteInterpole, R)
            Else
                'trouve les coordonnées des 4 coins de la carte interpolée en LL. Il s'agit d'une simple copie d'informations, Coins étant une structure
                'et les LL et pixels sont les mêmes
                ._Coins = _Coins
                'on prend un système cartographique hors site de capture car les fonctions de conversion de coordonnées ne s'appliquent pas
                R = RegionReelle
            End If
            'pour une carte interpolée le systeme cartographique est forcément Autre (les fonctions de calcul de coordonnées sont basées sur des pas à unités/pixel constants) 
            'et le point d'origine = empty 
            .SystemCartographique = New SystemeCartographique(R, _RegionVirtuelle.Size, SystemCartographique.Clef, SystemCartographique.Niveau.Clef, SystemCartographique.Projection.Libelle,
                                                              SystemCartographique.ZoneUtmReferencement, SystemCartographique.HemisphereUtmReferencement)
            .TrouverDPIImpression()
            ._NomAjout = SuffixeNom(.SystemCartographique)
            ._IsOk = CarteInterpole.ReserverMemoireTampon("Reserve Carte Interpole")
            Return ._IsOk
        End With
    End Function
    ''' <summary> trouve les coordonnées qui vont servir à l'interpolation. C'est les LL qui sont prise en compte. Il s'agit du plus grand rectangle en LL contenu dans la carte </summary>
    Private Sub TrouverCoordonnesCarteAvecGrille(CarteInterpole As Carte, ByRef R As RectangleD)
        Try
            Dim InterLon(3) As Double, InterLat(3) As Double
            'Trouver les coordonnées en DMS de la carte interpolée. On choisit le plus grand rectangle  de DD contenu dans la carte source
            InterLon(0) = Math.Min(Coins(0).Longitude, Coins(3).Longitude)
            InterLon(3) = InterLon(0)
            InterLon(2) = Math.Max(Coins(2).Longitude, Coins(1).Longitude)
            InterLon(1) = InterLon(2)
            InterLat(0) = Math.Max(Coins(0).Latitude, Coins(1).Latitude)
            InterLat(1) = InterLat(0)
            InterLat(2) = Math.Min(Coins(2).Latitude, Coins(3).Latitude)
            InterLat(3) = InterLat(2)
            R = New RectangleD(InterLon(0), InterLat(0), InterLon(2), InterLat(2))
            With CarteInterpole
                For CptPt = 0 To 3
                    'on recalcul les coordonnées MN03 à partir des coordonnees DMS
                    Dim PointGrille = ConvertWGS84ToProjection(New PointD(InterLon(CptPt), InterLat(CptPt)), SystemCartographique.Projection.Datum)
                    'calcul des captures d'écran correspondantes
                    .Coins(CptPt).CoordonneesCaptureEcran = ConvertPointXYtoChaine(PointGrille, "N2")
                    'remplissage des diverses données nécessaires à l'écriture du fichier Georef
                    .Coins(CptPt).Grille = PointGrille.Coordonnees
                    .Coins(CptPt).LatLon = New PointD(InterLon(CptPt), InterLat(CptPt))
                    .Coins(CptPt).Pixels = Coins(CptPt).Pixels
                Next
                'calcul des coordonnées en UTM WGS84
                .TrouverNbZonesUtm()
            End With
        Catch Ex As Exception
            AfficherErreur(Ex, "A5L0")
        End Try
    End Sub
#End Region
End Class