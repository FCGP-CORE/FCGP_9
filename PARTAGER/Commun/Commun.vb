''' <summary> contient toutes les variables, fonctions, procédures communes à au moins 2 des 3 applications Capturer, Convertir et Regrouper </summary>
Friend Module Commun
#Region "Variables  et procedures communes"
    Friend ReadOnly CrLf As String = Environment.NewLine
    Friend ReadOnly Cr As Char = Convert.ToChar(13)
    Friend ReadOnly Lf As Char = Convert.ToChar(10)
    Friend ReadOnly Tab As Char = Convert.ToChar(9)
    Friend ReadOnly NullChar As Char = Convert.ToChar(0)
    Friend ReadOnly Back As Char = Convert.ToChar(8)
    Friend Const Contact As String = "fcgp@laposte.net"
    Friend FlagAborted As Boolean
    Private _CheminParDefaut As String

    ''' <summary> id du thread de l'interface utilisateur </summary>
    Friend Property ID_Thread_IU As Integer
    Friend ReadOnly Property CheminParDefaut As String
        Get
            _CheminParDefaut = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\FCGP"
            If Not Directory.Exists(_CheminParDefaut) Then Directory.CreateDirectory(_CheminParDefaut)
            Return _CheminParDefaut
        End Get
    End Property
    ''' <summary>pour stocker le séparateur de nombre décimal de l'ordinateur</summary>
    Friend ReadOnly Property SeparateurDecimalOriginal As Char
    ''' <summary>indique que l'ordinateur à 2Go ou moins de mémoire</summary>
    Friend ReadOnly Property FlagSmallMemory As Boolean
    ''' <summary>pour stocker le nom du process en cours FCGP ou FCGP.vhost</summary>
    Friend ReadOnly Property NomProcess As String
    ''' <summary>correspond au chemin des documents de l'utilisateur allonger par un repertoire à créer</summary>
    Friend ReadOnly Property CheminEnregistrementProvisoire As String
    ''' <summary>Version de FCGP en texte version longue</summary>
    Friend ReadOnly Property NumFCGP As String
    ''' <summary> détermine le code page pour l'encodages des fichiers textes en . net et net core </summary>
    Friend ReadOnly Property Encoding_FCGP As Encoding
    ''' <summary>numero de version du programme FCGP</summary>
    Friend WriteOnly Property NumVersionFCGP As String
        Set(value As String)
            _NumVersionFCGP = value
        End Set
    End Property
    ''' <summary>pour stocker le type du programme  Capturer, Convertir ou Regrouper </summary>
    Friend Property TypeFCGP As String
        Get
            Return _TypeFCGP
        End Get
        Set(value As String)
            _TypeFCGP = value
        End Set
    End Property
    ''' <summary> label qui sert à l'affichage des informations d'un thread de travail à un thread WindowsForm </summary>
    Friend WriteOnly Property LabelInformation As Label
        Set(value As Label)
            _LabelInformation = value
        End Set
    End Property
    ''' <summary> Formulaire principal de l'application </summary>
    Friend ReadOnly Property FormApplication As Form
    ''' <summary>pour stocker la date de la version de fcgp en cours</summary>
    Friend Property DateVersionFCGP As String
    ''' <summary> indique si le système est connecté à internet </summary>
    Friend Property IsConnected As Boolean
    ''' <summary> donne la plus grande resolution d'écran du système </summary>
    Friend ReadOnly Property MaxBoundsEcran As Size
    ''' <summary> dimension en pixel de l'écran servant de support au formulaire principal de l'application </summary>
    Friend Property DimensionsEcranSupport As Rectangle
    ''' <summary> position et surface de la visue(PictureBox qui sert à afficher la carte) sur le bureau windows </summary>
    Friend Property VisueRectangle As Rectangle
    ''' <summary> renvoie le GUID de l'encoder Jpeg .net </summary>
    Friend ReadOnly Property EncodeurJpeg As ImageCodecInfo
    ''' <summary>taille maxi pour le tampon d'une carte</summary>
    Friend Const TailleMaxTampon As Integer = CInt(2 ^ 29)  '500 Mo ou 0.5 Go
    ''' <summary>titre de la fenêtre information si erreur FCGP</summary>
    Friend Const TitreErreur As String = "Erreur gérée dans FCGP_"
    ''' <summary>nb d'octets par pixel pour le format du bitmap</summary>
    Friend Const NbOctetsPixel As Integer = 3
    ''' <summary>nb d'octets par pixel pour le format du bitmap</summary>
    Friend Const FormatPixel As PixelFormat = PixelFormat.Format24bppRgb
    ''' <summary> Nb de milleseconde à attendre que tout réagisse </summary>
    Friend Const DureeMinuterie As Integer = 250
    ''' <summary> Nb d'octets de métadonnées dans l'entête des fichiers BMP </summary>
    Friend Const OffsetDataBMP As Integer = 54
    ''' <summary> pouce en millimètre </summary>
    Friend Const InchToMm As Double = 25.4
    ''' <summary> centième de pouce en millimètre </summary>
    Friend Const CinchToMm As Double = InchToMm / 100
    ''' <summary> maximum de la largeur ou de la hauteur d'une carte pour une sauvegarde en JPEG ou PNG avec les encodeurs GDI+ </summary>
    Friend Const MaxPixelsJpeg As Integer = 255 * NbPixelsTuile
    ''' <summary> caractères intedits pour les noms de fichier sous windows </summary>
    Friend ReadOnly CarsInterdits As Char() = New Char() {""""c, "/"c, "\"c, "*"c, "?"c, "<"c, ">"c, "|"c, ":"c}
    ''' <summary> Vérifie que la chaine passée en paramètre ne contient aucun des caractères interdits pour les noms de fichiers </summary>
    ''' <param name="Chaine"> nom du fichier avec ou sans extensions </param>
    Friend Function VerifierNomFichier(Chaine As String) As String
        Dim SN As New StringBuilder(Chaine.Length)
        For Each Caractere As Char In Chaine
            If Array.IndexOf(CarsInterdits, Caractere) = -1 Then SN.Append(Caractere)
        Next
        Return SN.ToString()
    End Function
    ''' <summary> Largeur de l'image au format BMP en octects. Est forcément un multiple de 4 </summary>
    ''' <param name="LargeurPixels"></param>
    Friend Function StrideImage(LargeurPixels As Integer) As Integer
        StrideImage = ((LargeurPixels * NbOctetsPixel + NbOctetsPixel) \ 4) * 4
    End Function
    ''' <summary> Convertit un nombre double en texte avec le point comme séparateur décimal</summary>
    ''' <param name="NombreDouble">nombre (type double) à convertir en texte</param>
    ''' <param name="Format">format attendu en sortie (nb de digit, séparateur de millier et décimale)</param>
    Friend Function DblToStr(NombreDouble As Double, Format As String) As String
        DblToStr = NombreDouble.ToString(Format, _Nfi_FCGP)
    End Function
    ''' <summary> convertit une chaine représentant un nombre décimal en double sans lever d'execption à cause du séparateur décimal du système </summary>
    ''' <param name="ChaineNombreDouble"></param>
    Friend Function StrToDbl(ChaineNombreDouble As String) As Double
        If _SeparateurDecimalOriginal = "."c AndAlso ChaineNombreDouble.IndexOf(","c) > -1 Then
            ChaineNombreDouble = ChaineNombreDouble.Replace(","c, "."c).Replace(" ", Nothing)
        ElseIf _SeparateurDecimalOriginal = ","c AndAlso ChaineNombreDouble.IndexOf("."c) > -1 Then
            ChaineNombreDouble = ChaineNombreDouble.Replace("."c, ","c).Replace(" ", Nothing)
        End If
        Return CDbl(ChaineNombreDouble)
    End Function
    Friend Function StrToSng(ChaineNombreSingle As String) As Single
        If _SeparateurDecimalOriginal = "."c AndAlso ChaineNombreSingle.IndexOf(","c) > -1 Then
            ChaineNombreSingle = ChaineNombreSingle.Replace(","c, "."c).Replace(" ", Nothing)
        ElseIf _SeparateurDecimalOriginal = ","c AndAlso ChaineNombreSingle.IndexOf("."c) > -1 Then
            ChaineNombreSingle = ChaineNombreSingle.Replace("."c, ","c).Replace(" ", Nothing)
        End If
        Return CSng(ChaineNombreSingle)
    End Function
    ''' <summary> Trouve une chaine qui est comprise entre 2 caractères spécialisés de préférence </summary>
    ''' <param name="Chaine"></param>
    ''' <param name="PosCar1">Indice de début de recherche du premier caractère spécialisé</param>
    ''' <param name="CAR1">caractère spécialisé N°1</param>
    ''' <param name="CAR2">caractère spécialisé N°2</param>
    ''' <returns>chaine comprise entre les 2 caractères spécialisés sans espace
    ''' Poscar1 pointe sur le caractère suivant CAR2 afin de pouvoir enchainer les recherches </returns>
    ''' <remarks>il n'y a pas de test pour savoir si car1 ou car2 appartiennent bien à la chaine</remarks>
    Friend Function TrouverChaineCompriseEntre(Chaine As String, ByRef PosCar1 As Integer, CAR1 As Char, Optional CAR2 As Char = Nothing) As String
        Dim PosCar2 As Integer, Trouve As String
        PosCar1 = Chaine.IndexOf(CAR1, PosCar1)
        If CAR2 = Nothing Then
            PosCar2 = Chaine.Length
        Else
            PosCar2 = Chaine.IndexOf(CAR2, PosCar1)
        End If
        Trouve = Chaine.Substring(PosCar1 + 1, PosCar2 - PosCar1 - 1).Trim
        PosCar1 = PosCar2
        Return Trouve
    End Function
    ''' <summary>convertit une chaine en imageformat. Par défaut en jpeg</summary>
    ''' <param name="FormatTexte">texte à convertir</param>
    Friend Function ConvertStringToImageFormat(FormatTexte As String) As ImageFormat
        Select Case FormatTexte
            Case "bmp", "Bmp", "BMP"
                Return ImageFormat.Bmp
            Case "png", "Png", "PNG"
                Return ImageFormat.Png
            Case "Jpeg", "jpg"
                Return ImageFormat.Jpeg
            Case "raw"
                Return ImageFormat.MemoryBmp
            Case Else
                Return ImageFormat.Jpeg
        End Select
    End Function
    ''' <summary>convertit un integer (généralement un index d'une liste déroulante) en imageformat. par défaut en jpeg</summary>
    ''' <param name="IndexFormat ">index à convertir</param>
    Friend Function ConvertIntToImageFormat(IndexFormat As Integer) As ImageFormat
        Select Case IndexFormat
            Case 0
                Return ImageFormat.Bmp
            Case 1
                Return ImageFormat.Png
            Case Else
                Return ImageFormat.Jpeg
        End Select
    End Function
    ''' <summary> convertit un point exprimé en DD WGS84 en coordonnées UTM WGS84 puis en texte </summary>
    Friend Function ConvertPointDDtoUTM(PointDD As PointF) As String
        Dim UTM As PointProjection = ConvertWGS84ToProjection(New PointD(PointDD.X, PointDD.Y), Datums.UTM_WGS84)
        ConvertPointDDtoUTM = ConvertPointXYtoChaine(UTM)
    End Function
    ''' <summary> convertit un point exprimé en DD WGS84 en coordonnées UTM WGS84 puis en texte </summary>
    Friend Function ConvertPointDDtoUTM(PointDD As PointD) As String
        Dim UTM As PointProjection = ConvertWGS84ToProjection(PointDD, Datums.UTM_WGS84)
        ConvertPointDDtoUTM = ConvertPointXYtoChaine(UTM)
    End Function
    ''' <summary> convertit un pointD qui représente une coordonnées DD en chaine de caractère formatée DMS </summary>
    ''' <param name="PointCoordonneesDD"> coordonnées DD </param>
    ''' <param name="Separateur">séparateur entre la chaine de la longitude et  la chaine de la latitude</param>
    ''' <remarks>ne fait aucune vérification sur la validité du nombre passer en paramètre</remarks>
    Friend Function ConvertPointDDtoDMS(PointCoordonneesDD As PointD, Optional InclureSens As Boolean = True, Optional Prec As Integer = 1, Optional Separateur As String = ",") As String
        Dim PrecText As String = If(Prec > 0, $"00.{New String("0"c, Prec)}", "00")
        Dim LatitudeDMS = ConvertDDtoDMS(PointCoordonneesDD.Y, TypesCoordsDD.Latitude, PrecText, InclureSens).DMSTxt
        Dim LongitudeDMS = ConvertDDtoDMS(PointCoordonneesDD.X, TypesCoordsDD.Longitude, PrecText, InclureSens).DMSTxt
        Return $"{LongitudeDMS}{Separateur} {LatitudeDMS}"
    End Function
    ''' <summary> convertit un double qui représente une Lat ou long DD en un Caractère E,O ou N,S un entier qui contient les degrés un entier qui contient les minutes
    ''' et un double qui contient les secondes</summary>
    ''' <param name="Coordonnees">Coordonnées en DD</param>
    ''' <param name="LatLon">Latitude (=0) ou Longitude (=1) </param>
    ''' <param name="PrecText">chaine de formatage des décimales associées aux secondes. Ex pas de décimale "", 1 décimale ".0", 2 décimales ".00"</param>
    ''' <param name="InclureSens"> Si oui indique le sens dans le DMSTxt avec une lettre sinon ce sera le signe </param>
    Friend Function ConvertDDtoDMS(Coordonnees As Double, LatLon As TypesCoordsDD, PrecText As String, InclureSens As Boolean) _
        As (Sens As Char, Deg As Integer, Min As Integer, Sec As Double, DMSTxt As String)
        Dim Prefix As String
        With ConvertDDtoDMS
            If InclureSens Then
                .Sens = SensDD(LatLon)(If(Coordonnees < 0, 1, 0))
                Prefix = $"{ .Sens}:"
            Else
                .Sens = If(Coordonnees < 0, "-"c, "+"c)
                Prefix = .Sens.ToString()
            End If
            Coordonnees = Math.Abs(Coordonnees)
            .Deg = CInt(Math.Truncate(Coordonnees))
            Dim ResteMin As Double = (Coordonnees - .Deg) * NbMin
            .Min = CInt(Math.Truncate(ResteMin))
            Dim ResteSec As Double = ResteMin - .Min
            .Sec = ResteSec * NbSec
            .DMSTxt = .Sec.ToString(PrecText)
            If .DMSTxt = 60.ToString(PrecText) Then
                .Sec = 0
                .Min += 1
            End If
            .DMSTxt = .Min.ToString("00")
            If .DMSTxt = "60" Then
                .Min = 0
                .Deg += 1
            End If
            .DMSTxt = $"{Prefix}{ .Deg.ToString(FormatCoord(LatLon))}°{ .Min:00}'{DblToStr(.Sec, PrecText)}"""
        End With
    End Function
    ''' <summary>convertit des degrés en DD avec les secondes exprimées par un double </summary>
    ''' <param name="Sens">"E" ou "O" pour les longitudes et "N" ou "S" pour les latitudes </param>
    ''' <remarks>ne fait aucune vérification sur la validité des nombres passés en paramètre</remarks>
    Friend Function ConvertDMStoDD(Sens As Char, Degre As Short, Minute As Short, Secondes As Double) As Double
        Select Case Sens
            Case "N"c, "E"c
                ConvertDMStoDD = Degre + Minute * InvNbMin + Secondes * Arc
            Case "S"c, "O"c
                ConvertDMStoDD = -(Degre + Minute * InvNbMin + Secondes * Arc)
            Case Else
                ConvertDMStoDD = 0
        End Select
    End Function
    ''' <summary> Convertit un pointD qui représente une coordonnée X/Y en chaine de caractère pour affichage sur l'écran ou mise dans un fichier </summary>
    ''' <param name="PointCoordonneesXY"> point single </param>
    ''' <param name="FormatXY"> Chaine représentant le nb de décimales des coordonnées X ou Y </param>
    Friend Function ConvertPointXYtoChaine(PointCoordonneesXY As PointProjection, Optional FormatXY As String = "N0") As String
        ConvertPointXYtoChaine = Nothing
        If PointCoordonneesXY.Zone <> 0 Then
            ConvertPointXYtoChaine = $"{PointCoordonneesXY.Zone:00} {PointCoordonneesXY.Hemisphere} "
        End If
        ConvertPointXYtoChaine &= $"X: {DblToStr(PointCoordonneesXY.Coordonnees.X, FormatXY)}, Y: {DblToStr(PointCoordonneesXY.Coordonnees.Y, FormatXY)}"
    End Function
    ''' <summary> Convertit un point double qui représente une coordonnée long/lat en chaine de caractère pour affichage sur l'écran ou mise dans un fichier </summary>
    ''' <param name="PointCoordonneesDD"> point double </param>
    Friend Function ConvertPointDDtoChaine(PointCoordonneesDD As PointD, Optional FormatXY As String = "N8") As String
        ConvertPointDDtoChaine = $"Lon : {DblToStr(PointCoordonneesDD.X, FormatXY)}, Lat : {DblToStr(PointCoordonneesDD.Y, FormatXY)}"
    End Function
    ''' <summary> renvoie nombre aléatoire compris entre 2 bornes </summary>
    ''' <param name="BorneInf"> Borne infèrieure qui peut être incluse </param>
    ''' <param name="BorneSup"> Borne supèrieure qui peut être incluse </param>
    ''' <returns></returns>
    Friend Function IntAleatoire(BorneInf As Integer, BorneSup As Integer) As Integer
        Return Rand.Next(BorneInf, BorneSup)
    End Function
    ''' <summary> crée un GUID de 32 caractères à partir de 16 octets aléatoires de la forme d8672581-daf9-5f61-28b7-de75c6000256 </summary>
    Friend Function CreateGUID() As String
        Dim Buffer(15) As Byte
        Rand.NextBytes(Buffer)
        Dim G As New Guid(Buffer)
        Return G.ToString
    End Function
    ''' <summary> met le paramètre Qualité pour la comprssion des images avec l'encodeurJpeg</summary>
    ''' <param name="Qualite">qualité souhaitée de 0 : mauvais à 100 : très bon</param>
    Friend Function QualiteJPEG(Qualite As Integer) As EncoderParameters
        ' for the Quality parameter category.
        Dim myEncoder As Imaging.Encoder = Imaging.Encoder.Quality
        Dim myEncoderParameter As New EncoderParameter(myEncoder, Qualite)
        QualiteJPEG = New EncoderParameters()
        QualiteJPEG.Param(0) = myEncoderParameter
    End Function
    ''' <summary> renvoie le projet, module ou class et nom de méthode en fonction d'une clé </summary>
    ''' <param name="CleStr"> clé de la méthode </param>
    Friend Function RenvoyerNomMethodeErreur(CleStr As String) As String
        Dim NomMethode As String = Nothing
        ListeMethodes.TryGetValue(CleStr, NomMethode)
        Return If(NomMethode, CleStr)
    End Function
    ''' <summary> attend le nb de ms indiqué sans bloquer le thread appelant.</summary>
    ''' <param name="NbMillisecondes">nb de ms à attendre avant de rendre la main</param>
    Friend Sub Attendre(NbMillisecondes As Integer)
        FlagMinuterie = True
        Minuterie.Change(NbMillisecondes, Timeout.Infinite)
        While FlagMinuterie
            Application.DoEvents()
        End While
    End Sub
    ''' <summary> initialisation de base pour l'ensemble des applications </summary>
    ''' <param name="FormApplication"> formulaire de démarage de l'application</param>
    ''' <param name="NumeroVersion"> Numéro de la version de l'application sous la forme : ""</param>
    ''' <param name="DateVersion"> date de l'application sous la forme : ""</param>
    Friend Function InitialiserBaseApplication(FormApplication As Form, NumeroVersion As String, DateVersion As String) As Boolean
        'definit le formulaire principal de l'application
        _FormApplication = FormApplication
        _TypeFCGP = _FormApplication.Name.ToUpper
        _NumVersionFCGP = NumeroVersion
        _DateVersionFCGP = DateVersion
        ID_Thread_IU = Environment.CurrentManagedThreadId
        FlagAborted = True
        'on indique l'écran support de l'application pour centrer les messages d'information et d'erreur dés le chargement
        VisueRectangle = _FormApplication.Bounds
        'remplit une liste qui permet d'indiquer correctement la méthode où a eu lieu une erreur gérée par l'application
        If Not RemplirListeMethode() Then Return False
        _SeparateurDecimalOriginal = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator(0)
        _Nfi_FCGP = New CultureInfo(CultureInfo.CurrentCulture.Name, False).NumberFormat
        _Nfi_FCGP.NumberDecimalSeparator = "."
        _Nfi_FCGP.NumberGroupSeparator = " "
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        _Encoding_FCGP = Encoding.GetEncoding(1252)
        Return True
    End Function
    ''' <summary> initialise le module commun à tous les FCGP </summary>
    ''' <param name="TailleReserve">Taille en octet de la réserve mémoire de base</param>
    ''' <param name="FlagVersion">si true vérifie si une nouvelle version est disponible</param>
    Friend Function InitialiserCommun() As Boolean
        InitialiserCommun = False
#If BETA Then
        TypeVersion = " Beta"
#ElseIf DEBUG Then
        TypeVersion = " Debug"
#Else
        TypeVersion = " Release"
#End If
        Try
            _FlagSmallMemory = GetTotalMemory() <= CULng(TailleMaxTampon) * 4
            InitialiserEcran()
            'initialise la version de FCGP en cours
            InitialiseVersion()
            'créer le chemin provisoire
            _CheminEnregistrementProvisoire = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"\FCGP_{_TypeFCGP}_FichiersProvisoiresVB"
            Directory.CreateDirectory(_CheminEnregistrementProvisoire)
            Minuterie = New Threading.Timer(New TimerCallback(AddressOf Minuterie_Tick), Nothing, Timeout.Infinite, Timeout.Infinite)
            GUIDJpeg()
            LabelInformationMethodeMessage = New LabelInformationCallDelegate(AddressOf AfficherVisueInformation)
            Rand = New Random
            IsConnected = True
            ID_Thread_IU = Environment.CurrentManagedThreadId
            InitialiserCommun = True
        Catch Ex As Exception
            AfficherErreur(Ex, "Z0A3")
        End Try
    End Function
#If Not BETA Then
    Friend Sub VerifierVersionApplication()
        Dim Uri As String = $"{CheminFichierMAJ}information{_TypeFCGP.ToLower}.txt"
        Dim Fichier As String = _CheminEnregistrementProvisoire & "\FichierVersion.txt"
        TitreInformation = "Information FCGP_" & _TypeFCGP
        MessageInformation = ""
        Dim Ret = TelechargerFichier(Uri, Fichier)
        If Ret = 0 Then
            RechercherInformationFCGP(Fichier)
        ElseIf Ret = 1 Then
            MessageInformation = $"Une nouvelle version de FCGP_{_TypeFCGP & CrLf}sera bientôt disponible en téléchargement."
        Else
            MessageInformation = $"Le serveur du site internet FCGP n'est pas accessible.{CrLf}Ré-essayer plus tard"
        End If
        If MessageInformation <> "" Then AfficherInformation()
    End Sub
#End If
    ''' <summary>libère les ressources associées au module commun</summary>
    Friend Sub CloturerCommun()
        'on efface le répertoire provisoire créer au lancement de FCGP
        If Directory.Exists(_CheminEnregistrementProvisoire) Then Directory.Delete(_CheminEnregistrementProvisoire, True)
    End Sub
    ''' <summary> determine si l'ordinateur hôte est connecté à internet </summary>
    Friend Async Function TesterConnectionInternetAsync() As Task
        Try
            Using F = New Sockets.TcpClient()
                Await F.ConnectAsync("www.google.com", 80)
            End Using
            IsConnected = True
        Catch
            IsConnected = False
        End Try
    End Function
    ''' <summary> méthode appelée dans le code pour afficher un text dans le label d'information de l'application</summary>
    ''' <param name="Message"> message à afficher </param>
    Friend Sub AfficherVisueInformation(Message As String)
        LabelInformationMessage(Message)
    End Sub
    ''' <summary> calcule la distance en mètre entre 2 points exprimés en DD et le cap pour les relier </summary>
    ''' <param name="PtDeb"> point de départ </param>
    ''' <param name="PtFin"> point d'arrivée </param>
    ''' <returns> Longueur en mètre et cap en degré </returns>
    Friend Function CalculerDistanceCap(PtDeb As PointD, PtFin As PointD) As (Longueur As Double, Cap As Double)
        Dim DeltaLon As Double = Deg_Rad * (PtFin.Lon - PtDeb.Lon)
        Dim DeltaLat As Double = Deg_Rad * (PtFin.Lat - PtDeb.Lat)
        Dim CosLon As Double = Math.Cos(Deg_Rad * (PtFin.Lat + PtDeb.Lat) / 2) * DeltaLon
        Dim Longueur = Math.Sqrt(RayonWGS84 * RayonWGS84 * (DeltaLat * DeltaLat + CosLon * CosLon))
        Dim Cap = Rad_Deg * Math.Atan2(CosLon, DeltaLat)
        Return (Longueur, Cap)
    End Function
    ''' <summary> calcule la largeur et la hauteur en mètre d'une carte dont les coordonnées  
    ''' sont exprimées en DD ou en mètres. La carte peut être interpolée </summary>
    ''' <param name="Pt0"> point HautGauche de la carte </param>
    ''' <param name="Pt2"> point BasDroit de la carte </param>
    ''' <param name="FlagDD"> indique que les coordonnées sont exprimées en DD </param>
    Friend Function CalculerDimensionsCarte(Pt0 As PointD, Pt2 As PointD, FlagDD As Boolean) As (Largeur As Double, Hauteur As Double)
        Dim Hauteur, largeur As Double
        If FlagDD Then
            Dim Pt1 = New PointD(Pt2.X, Pt0.Y)
            Dim Pt3 = New PointD(Pt0.X, Pt2.Y)
            'la hauteur est la même du coté ouest ou est de la carte.
            Hauteur = CalculerDistanceCap(Pt0, Pt3).Longueur
            'Ce n'est pas le cas pour la largeur qui est plus petite au nord qu'au sud. La largeur tend vers le 0 plus on va au nord
            Dim LargeurN = CalculerDistanceCap(Pt0, Pt1).Longueur
            Dim LargeurS = CalculerDistanceCap(Pt3, Pt2).Longueur
            largeur = (LargeurN + LargeurS) / 2
        Else
            Hauteur = Pt0.Y - Pt2.Y
            largeur = Pt2.X - Pt0.X
        End If
        Return (largeur, Hauteur)
    End Function
    ''' <summary> transforme un timespan en chaine de la forme Heure sur 2 chiffres, Minutes sur 2 chiffres, secondes sur 2 chiffres et millièmes de secondes </summary>
    ''' <param name="TS"> timespan à transformer </param>
    ''' <param name="FlagMs"> indique que l'on veut aussi les millièmes de secondes</param>
    Friend Function TimeSpanToStr(TS As TimeSpan, FlagMs As Boolean) As String
        If FlagMs Then
            Return TS.ToString("hh\:mm\:ss\.fff")
        Else
            Return TS.ToString("hh\:mm\:ss")
        End If
    End Function
#End Region
#Region "variables et procédures internes"
    ''' <summary> liste des méthodes qui ont une gestion d'erreur </summary>
    Private ListeMethodes As Dictionary(Of String, String)
    ''' <summary> remplit la liste des méthodes qui ont une gestion d'erreur </summary>
    Private Function RemplirListeMethode() As Boolean
        Dim Ret As Boolean = False
        Try
            ListeMethodes = New Dictionary(Of String, String)(200)
            If File.Exists("ListeMethodes.xml") Then
                Dim MethodesXML As XElement = XElement.Load("ListeMethodes.xml")
                For Each MethodeXML As XElement In MethodesXML.Elements(XName.Get("Methode"))
                    Dim CleStr As String = MethodeXML.Attribute(XName.Get("cle")).Value
                    If Not String.IsNullOrEmpty(CleStr) Then
                        Dim [Program] As String = MethodeXML.Attribute(XName.Get("program")).Value
                        Dim [Module] As String = MethodeXML.Attribute(XName.Get("module")).Value
                        Dim Nom As String = MethodeXML.Attribute(XName.Get("nom")).Value
                        ListeMethodes.Add(CleStr, $"{[Program]} : {[Module]} : {Nom}")
                    End If
                Next
                Ret = True
            Else
#If DEBUG Then
                'obligatoire pour la version Debug afin d'avoir une bonne idée des erreurs gérées
                MessageInformation = $"La partie Debug de FCGP_{_FormApplication.Name} n'a pas pu être initialisée" & CrLf &
                                     "Veuillez recharger le fichier ListeMethodes.xml dans le répertoire" & CrLf &
                                     "de l'application."
                TitreInformation = $"Information FCGP_{_TypeFCGP}_{_NumVersionFCGP} du {_DateVersionFCGP}"
                AfficherInformation()
#Else
                'retourne toujours vrai pour la version Release car cela dépend du package si il est distribué avec ou sans ListeMethodes
                Ret = True
#End If
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "W7X7")
        End Try
        Return Ret
    End Function
    Private ReadOnly SensDD()() As Char = {New Char() {"N"c, "S"c}, New Char() {"E"c, "O"c}}
    Private ReadOnly FormatCoord() As String = New String() {"00", "000"}
    Private Const NbMin As Double = 60
    Private Const NbSec As Double = 60
    Private Const NbArc As Double = NbMin * NbSec
    Private Const InvNbMin As Double = 1 / NbMin
    Private Const Arc As Double = 1 / NbArc
    Private _LabelInformation As Label
    Private Rand As Random
    ''' <summary>chemin de téléchargement des fichiers des mises à jour de version et d'information</summary>
    Private Const CheminFichierMAJ As String = "http://fcgp.e-monsite.com/medias/files/"
    ''' <summary> pour écrire des points à la place des virgules sur les ordinateurs qui n'ont pas le point comme séparateur décimal par défaut </summary>
    Private ReadOnly Property Nfi_FCGP As NumberFormatInfo
    '''<summary>flag servant a indiquer que le temps de la minuterie est écoulé</summary>
    Private FlagMinuterie As Boolean
    '''<summary> minuterie pour les tâches qui sont éxécutées sur un thread qui ne doit pas être bloqué </summary>
    Private Minuterie As Threading.Timer
    ''' <summary> Indique le numéro de version </summary>
    Private _NumVersionFCGP As String
    ''' <summary> FCGP est composé de 3 programmes, indique lequel c'est </summary>
    Private _TypeFCGP As String
    ''' <summary> indique si il s'agit d'une Version Release ou d'une Bêta </summary>
    Private TypeVersion As String
    ''' <summary> Touve le codec JPEG dans la liste des codecs de GDI+ </summary>
    Private Sub GUIDJpeg()
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        Dim format As ImageFormat = ImageFormat.Jpeg
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                _EncodeurJpeg = codec
            End If
        Next codec
    End Sub
    ''' <summary> procédure de rappel pour indiquer que le temps d'attente de la minuterie est écoulé </summary>
    Private Sub Minuterie_Tick(Obj As Object)
        FlagMinuterie = False
    End Sub
    ''' <summary> télécharge un fichier qui contient le numéro de la dernière version de FCGP et des informations afin de prévenir les utilisateurs </summary>
    Private Sub RechercherInformationFCGP(FichierMAJ As String)
        Dim LignesFichierMAJ() As String = File.ReadAllLines(FichierMAJ, Encoding_FCGP)
        Dim DateFCGP = LignesFichierMAJ(1).Substring(18)
        Dim AnneeFCGP = DateFCGP.Substring(6)
        If LignesFichierMAJ(0).StartsWith("NumVersionFCGP") Then
            Try
                If DateFCGP <> _DateVersionFCGP Then
                    Dim NumVersion = LignesFichierMAJ(0).Substring(17)
                    MessageInformation = $"FCGP_{_TypeFCGP} Version {AnneeFCGP}-{NumVersion} en date du {DateFCGP}" & CrLf &
                                          "est disponible en téléchargement."
                    'affichage du message d'information
                    Dim Information = LignesFichierMAJ(2).AsSpan(18)
                    If Information <> "Aucune" Then
                        MessageInformation &= String.Concat(CrLf, Information)
                    End If
                End If
            Catch Ex As Exception
                AfficherErreur(Ex, "S6H7")
            End Try
        Else
            'si le fichier ne correspond pas à ce qu'on attend c'est qu'il est encours de changement
            MessageInformation = $"Une nouvelle version de FCGP_{_TypeFCGP & CrLf}sera bientôt disponible en téléchargement."
        End If
    End Sub
    ''' <summary> initialise l'écran qui sera support du formulaire principal et des autres formulaires (navigateur de capture, information ou message d'erreur, aide </summary>
    Private Sub InitialiserEcran()
        'si c'est la première ouverture ou si le numéro d'écran est > au nb d'écran 
        'la configuration du système a changé généralement pour les portables passage de 2 à 1 écran, on choisi display1
        _MaxBoundsEcran = Size.Empty
        For Cpt As Integer = 0 To Screen.AllScreens.Length - 1
            Dim R As New Rectangle(Point.Empty, Screen.AllScreens(Cpt).Bounds.Size)
            If R.Contains(New Rectangle(Point.Empty, _MaxBoundsEcran)) Then _MaxBoundsEcran = Screen.AllScreens(Cpt).Bounds.Size
            If PartagerSettings.NUM_ECRAN = -1 OrElse PartagerSettings.NUM_ECRAN > Screen.AllScreens.Length - 1 Then
                If Screen.AllScreens(Cpt).Primary Then
                    PartagerSettings.NUM_ECRAN = Cpt
                End If
            End If
        Next
        'initialise la variable pour le positionnement de la boite d'information
        _DimensionsEcranSupport = Screen.AllScreens(PartagerSettings.NUM_ECRAN).Bounds
        VisueRectangle = _DimensionsEcranSupport
    End Sub
    ''' <summary> renvoie la chaine d'identification du programme </summary>
    Private Sub InitialiseVersion()
        Dim TexteVersionFCGP As String = "FCGP " & _TypeFCGP & " - Version : " & DateVersionFCGP.Substring(DateVersionFCGP.Length - 4) & "-"
        _NumFCGP = TexteVersionFCGP & _NumVersionFCGP & TypeVersion
    End Sub
    ''' <summary> permet de télécharger un fichier distant en libre accès (sans identifiants) sur le PC en mode synchrone </summary>
    ''' <param name="URI"> Adresse du fichier à téléchager </param>
    ''' <param name="Fichier"> Chemin du fichier sur le PC</param>
    ''' <returns> 0 : Ok, 1 : FileNotFound, 2 : Erreur sur le Serveur de l'Uri, 3 : Serveur de l'Uri inaccessible ou fichier déjà existant </returns>
    Private Function TelechargerFichier(URI As String, Fichier As String) As Integer
        Using Serv As New HttpClient
            Try
                Dim requete As New HttpRequestMessage(HttpMethod.Get, URI)
                Dim reponse As HttpResponseMessage = Serv.Send(requete)
                If reponse.StatusCode = HttpStatusCode.OK Then
                    Using Source As Stream = reponse.Content.ReadAsStream()
                        Using DestinationStream As FileStream = File.Create(Fichier)
                            Source.CopyTo(DestinationStream)
                        End Using
                    End Using
                    Return 0 'Ok
                ElseIf reponse.StatusCode = HttpStatusCode.NotFound Then
                    Return 1 'FileNotFound
                Else
                    Return 2 'Erreur sur le Serveur de l'Uri
                End If
            Catch
                Return 3 'Serveur de l'Uri inaccessible
            End Try
        End Using
    End Function
    ''' <summary> méthode effective qui affiche le texte dans le label d'information </summary>
    ''' <param name="Message"> message à afficher </param>
    Private Sub LabelInformationMessage(Message As String)
        If _LabelInformation.InvokeRequired Then
            _LabelInformation.Invoke(LabelInformationMethodeMessage, New Object() {Message})
        Else
            _LabelInformation.Text = Message
        End If
    End Sub
    ''' <summary> définition de délégué pour l'affichage de text dans le label d'information </summary>
    ''' <param name="Message"> paramètre du délégué </param>
    Private Delegate Sub LabelInformationCallDelegate(Message As String)
    ''' <summary> délégué pour l'affichage de text dans le label d'information </summary>
    Private LabelInformationMethodeMessage As LabelInformationCallDelegate
#End Region
End Module