Imports FCGP.ServeurCarto
''' <summary> création d'une carte et des fichiers tuiles associés à partir du téléchargement de tuiles sur un serveur cartographique </summary>
Friend Module TelechargementCarte
#Region "Variables"
    Friend WriteOnly Property FlagAnnulerTelechargement As Boolean
        Set(value As Boolean)
            _FlagAnnuler = value
        End Set
    End Property
    ''' <summary> Nb maximum de tuiles du serveur carto pour une carte enfonction du type de support de la carte </summary>
    Friend ReadOnly Property NB_Max_TuilesServeurCarto As Integer
        Get
            Dim Tuiles = If(CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier, TailleMaxTampon * 3, TailleTamponCarteSource(CartesSettings.SUPPORT_CARTE))
            Return Tuiles \ PoidsTuile
        End Get
    End Property
    ''' <summary> Nb maximum de tuiles pour une rangée d'un fichier tuiles hors carte suisse interpolée </summary>
    ''' <param name="SupportCarte"> Typesupport de la carte </param>
    ''' <param name="Rapport"> rapport entre les dimensions de la tuile du fichier tuiles et celles du serveur : 1,2 ou 4 </param>
    Friend ReadOnly Property NB_Max_Tuiles_X(SupportCarte As TypeSupportCarte, Rapport As Integer) As Integer
        Get
            Return TailleTamponCarteSource(SupportCarte) \ PoidsTuile * Rapport
        End Get
    End Property
    ''' <summary> flag indiquant qu'il y a un empêchement à la continuation du téléchargement des tuiles </summary>
    Private ReadOnly Property FlagArret As Boolean
        Get
            Return _FlagAnnuler OrElse Not IsConnected
        End Get
    End Property
    ''' <summary> flag indiquant que rien n'empêche la continuation du téléchargement des tuiles </summary>
    Private ReadOnly Property FlagContinue As Boolean
        Get
            Return Not _FlagAnnuler AndAlso IsConnected
        End Get
    End Property
    ''' <summary> nb d'octects réserver de base pour les cartes autres que les cartes suisse interpolées </summary>
    Private ReadOnly TailleTamponCarteSource() As Integer = New Integer() {TailleMaxTampon, TailleMaxTampon \ 4, TailleMaxTampon \ 2}
    ''' <summary> indique si la demande d'annulation du téléchargement des tuiles a été demandée et par riccochet l'abandon de la création de la carte </summary>
    Private _FlagAnnuler As Boolean
    ''' <summary> serveur à partir duquel les tuiles seront téléchargées </summary>
    Private ServeurCarto As ServeurCarto
    ''' <summary> fichier servant de support d'écriture à la capture de la carte si le support de carte est fichier </summary>
    Private FichierBin As FileStream
    ''' <summary> flux mémoire associé au fichier binaire pour éviter des engorgements de la mémoire</summary>
    Private TamponStream As MemoryStream
    ''' <summary> carte support du téléchargement des tuiles </summary>
    Private CarteSource As Carte
    ''' <summary> pointeur sur le tampon de copie des tuiles téléchargées </summary>
    Private ImageBitsPtr As IntPtr
    ''' <summary> en cas de support de carte type fichier il faut un tampon pour la copie des tuiles téléchargées </summary>
    Private TamponSupportFichier As SharedPinnedByteArray
    ''' <summary> Surface à télécharger exprimée en tuiles de la projection du serveur associé au site carto </summary>
    Private SurfaceCapture As SurfaceTuiles
    ''' <summary> indique le nb de tuiles qui ont été téléchargées </summary>
    Private NbTuilesTelechargees As Integer
    ''' <summary> indique le nb de tuiles qui ont été téléchargées pour une rangée </summary>
    Private NbTuilesRangeeTelechargees As Integer
    ''' <summary> indique un petit message avant le nb de tuiles telechargées </summary>
    Private MessageInfo As String
    ''' <summary> sert à mesurer le temps de téléchargement des tuiles et de traitement de la carte </summary>
    Private ChronoCarte As Chronometre

#End Region
#Region "Initialisation globale"
    ''' <summary> création d'une carte ayant les coordonnées de celles demandées </summary>
    ''' <param name="NomCarte"> nom de la carte sans chemin et sans extension </param>
    ''' <param name="SurfaceTuilesDemande"> surface tuiles demandée pour la nouvelle carte </param>
    ''' <param name="Information"> label du programme appelant où l'on va écrire les informations générées pendant le téléchargement et la création de la carte </param>
    ''' <param name="Site"> si on veut un site différent de celui de CapturerSettings </param>
    ''' <param name="DomTom"> si on veut un DomTom différent de celui de CapturerSettings </param>
    ''' <param name="CheminCache"> si on veut un Cache différent de celui de CapturerSettings </param>
    Friend Async Function RealiserCarteAsync(NomCarte As String, SurfaceTuilesDemande As SurfaceTuiles, Information As Label,
                                             Optional Site As SitesCartographiques = SitesCartographiques.Aucun,
                                             Optional DomTom As DomsToms = DomsToms.aucun,
                                             Optional CheminCache As String = Nothing) As Task
        Try
            LabelInformation = Information
            SurfaceCapture = SurfaceTuilesDemande
            If Not InitialiserCarteSource(NomCarte) Then Exit Try
            AfficherVisueInformation($"{MessageInfo}{NbTuilesTelechargees} / {SurfaceCapture.NbTuiles}")
            ServeurCarto = New ServeurCarto(Nothing, Site, DomTom, CheminCache)
            _FlagAnnuler = False
            NbTuilesTelechargees = 0
            ChronoCarte = New Chronometre(2)
            'lance l'action de téléchargement des tuiles et rend la main
            Dim Telechargement = ChronoCarte.Demarre()
            'rend la main à l'interface utilisateur pendant le téléchargement de la carte
            Await Task.Run(Sub() TelechargerCarte())
            ChronoCarte.Arrete(Telechargement)
            If CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier Then LibererFichierBin()

            If FlagContinue Then
                Dim Traitement = ChronoCarte.Demarre()
                'lance l'action de traitement de la carte et rend la main
                Dim Coordonnees = CoordonneesCarte()
                If Await Task.Run(Function() CarteSource.FinaliserCarte(Coordonnees, "Préparation de la carte suite au téléchargement des tuiles.")) Then
                    ChronoCarte.Arrete(Traitement)
                    'si il n'y a pas d'erreur on rajoute le temps de capture et de traitement aux infos concernant les coordonnées.
                    MessageInformation = CoinsCarte() & $"Temps de téléchargement : {TimeSpanToStr(ChronoCarte.Duree(Telechargement), False)}{CrLf}" &
                                                        $"Temps de traitement : {TimeSpanToStr(ChronoCarte.Duree(Traitement), False)}"
                    If FormApplication.WindowState = FormWindowState.Minimized Then FormApplication.WindowState = FormWindowState.Normal
                    AfficherInformation()
                End If
            Else
                Dim Titre = TitreInformation
                TitreInformation = "Module Téléchargement des tuiles"
                If _FlagAnnuler Then
                    MessageInformation = "Téléchargement des tuiles annulé par l'utilisateur"
                Else
                    MessageInformation = "Téléchargement arrêté par connexion Internet interrompue"
                End If
                AfficherInformation()
                TitreInformation = Titre
            End If
        Catch ex As Exception
            AfficherErreur(ex, "A2AF")
        End Try
        CarteSource.Dispose()
        CarteSource = Nothing

        ServeurCarto.Dispose()
        ServeurCarto = Nothing
    End Function
    ''' <summary> télécharge la carte par rangée de tuiles. Chaque tuile gère sa composition et son écriture dans le tampon </summary>
    Private Sub TelechargerCarte()
        Dim T As TuileCarte, NbColRangee, NbCol As Integer
        For Row As Integer = SurfaceCapture.NumRangDeb To SurfaceCapture.NumRangFin
            NbTuilesRangeeTelechargees = 0
            NbCol = SurfaceCapture.NbColonnes
            NbColRangee = NbCol
            For Col As Integer = SurfaceCapture.NumColDeb To SurfaceCapture.NumColFin
                If FlagContinue Then
                    'on lance la composition de la tuile ainsi que son écriture dans le tampon de l'image
                    'et redonne la main à la boucle. N'attend pas que la tuile soit finie (await dans le constructeur)
                    T = New TuileCarte(Col, Row)
                    NbColRangee -= 1
                Else
                    'toutes les tuiles non demandées sont considérées comme finies
                    NbTuilesRangeeTelechargees += NbColRangee
                    Exit For
                End If
            Next
            'on attend que toutes les tuiles de la rangée soient considérées comme finies
            Do While NbTuilesRangeeTelechargees < NbCol
            Loop
            If FlagArret Then Exit For
            If CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier Then
                TamponStream.CopyTo(FichierBin)
                TamponStream.Position = 0
                TamponSupportFichier.ClearColor(PartagerSettings.COULEUR_CARTE)
            End If
        Next
    End Sub
#End Region
#Region "Initialisation Carte"
    ''' <summary> création de la carte support du téléchargement des tuiles </summary>
    ''' <param name="NomCarte"> nom de la carte sans chemin et sans extension </param>
    ''' <param name="SysCarto"> système cartographique de la carte </param>
    Private Function InitialiserCarteSource(NomCarte As String) As Boolean
        'création de la nouvelle carte
        CarteSource = New Carte(SurfaceCapture.NbColonnes, SurfaceCapture.NbRangees, SysCartoEncours, NomCarte)
        'initialisation des différentes variables globales à la carte
        With CarteSource
            If .IsOk Then
                .DemandeFichiersGeoref = ConvertirIndexToFichiersGeoref(CartesSettings.GEOREF)
                If CartesSettings.SUPPORT_CARTE = TypeSupportCarte.Fichier Then
                    AttribuerFichierBin()
                    ImageBitsPtr = TamponSupportFichier.BitPtr
                Else
                    ImageBitsPtr = .ImageBitPtr
                End If
                MessageInfo = $"{ .NomComplet} :{ .Separateur}Téléchargement données :{ .Separateur}"
                'infos concernant les cartes
                'si il y a une trace encours et que som emprise soit sur la carte on renseigne les informations concernant son dessin
                If TRK.IsTraceEncours AndAlso SurfaceCapture.RectanglePixels.IntersectsWith(TRK.TraceEncours.BoundingBoxVirtuel) AndAlso CartesSettings.IS_TRACE Then
                    .PinceauTrace = New Pen(TracesSettings.COULEUR_TRACE, TracesSettings.EPAISSEUR_TRACE) With {.LineJoin = LineJoin.Round}
                    .DemandeAfficherTrace = True
                    .NbTracesKML = 0
                    .FichiersTRK = New List(Of String)(1) From {Path.Combine(TracesSettings.CHEMIN_TRACE, TRK.TraceEncours.Nom & ".trk")}
                End If
            End If
            Return .IsOk
        End With
    End Function
    ''' <summary> détermination des 4 coins de la carte sous forme de chaine. Historique par rapport aux versions avec captures d'écran </summary>
    Private Function CoordonneesCarte() As String()
        Dim Coordonnees(3) As String
        For Cpt As Integer = 0 To 3
            Coordonnees(Cpt) = ConvertPointXYtoChaine(New PointProjection(SurfaceCapture.RectangleGrille.Pt(Cpt)), "N2")
        Next
        Return Coordonnees
    End Function
    ''' <summary> initialise la variable FichierGeoref en fonction d'index (liste déroulante du formulaire Preférences </summary>
    ''' <param name="Index"></param>
    Private Function ConvertirIndexToFichiersGeoref(Index As Integer) As ChoixFichiersGeoref
        If Index = 0 Then
            Return ChoixFichiersGeoref.Aucun
        Else
            'si la demande concerne un georeferencement autre que celui de FCGP on le rajoute
            Return CType(2 ^ (Index - 1), ChoixFichiersGeoref) Or ChoixFichiersGeoref.Georef_Carte
        End If
    End Function
    ''' <summary> convertit les 4 coins de la carte en texte avec les unités de coordonnées en cours </summary>
    Private Function CoinsCarte() As String
        Dim SelectIndex As Integer = CapturerSettings.INDICE_TYPE_COORDONNEES
        Dim Coincarte As New StringBuilder
        Coincarte.AppendLine($"La carte {CarteSource.Nom}.{CarteSource.Format} a été sauvegardée{CrLf & CrLf}Coordonnées des tuiles téléchargées :")
        Select Case SelectIndex
            Case 0
                For Cpt = 0 To 3
                    Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointXYtoChaine(New PointProjection(SurfaceCapture.RectangleGrille.Pt(Cpt)))}")
                Next
            Case 1
                For Cpt = 0 To 3
                    Dim CoordDD = PointGrilleToPointDD(SurfaceCapture.RectangleGrille.Pt(Cpt), ServeurCarto.Datum)
                    Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointDDtoChaine(CoordDD)}")
                Next
            Case 2
                For Cpt = 0 To 3
                    Dim CoordDD = PointGrilleToPointDD(SurfaceCapture.RectangleGrille.Pt(Cpt), ServeurCarto.Datum)
                    Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointDDtoDMS(CoordDD)}")
                Next
            Case 3
                For Cpt = 0 To 3
                    Dim CoordDD = PointGrilleToPointDD(SurfaceCapture.RectangleGrille.Pt(Cpt), ServeurCarto.Datum)
                    Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointDDtoUTM(CoordDD)}")
                Next
            Case 4
                For Cpt = 0 To 3
                    Dim PointTuile As New PointG(Affichage.Echelle, SurfaceCapture.PtPixels(Cpt))
                    Coincarte.AppendLine($"Pt{Cpt} : {PointTuile.ToString}")
                Next
        End Select
        Coincarte.AppendLine()
        Return Coincarte.ToString
    End Function
    ''' <summary> met en place les ressources associées au fichier support de la capture</summary>
    Private Sub AttribuerFichierBin()
        'pour le téléchargement on travaille avec 1 rangée de tuile serveur pui son l'enregistre dans un fichier raw
        TamponSupportFichier = New SharedPinnedByteArray(SurfaceCapture.NbColonnes * NbOctetsDecalXTuile, NbPixelsTuile,
                                                         CarteSource.Reserve, "Téléchargement Carte")
        TamponStream = New MemoryStream(TamponSupportFichier.Bits, TamponSupportFichier.IndexBytesDisponible, TamponSupportFichier.NbBytes)
        FichierBin = New FileStream(CarteSource.CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None, PoidsTuile)
    End Sub
    ''' <summary> libère les ressources associées au fichier support de la capture</summary>
    Private Sub LibererFichierBin()
        If TamponStream IsNot Nothing Then
            TamponStream.Close()
            TamponStream = Nothing
        End If
        If TamponSupportFichier IsNot Nothing Then
            TamponSupportFichier.Dispose()
            TamponSupportFichier = Nothing
        End If
        If FichierBin IsNot Nothing Then
            FichierBin.Close()
            FichierBin = Nothing
        End If
    End Sub
#End Region

    ''' <summary> assume une tuile de serveur qui compose une carte. Elle peut être composée de 1 à 3 couches de données (layers) supperposées. 
    ''' Autonomme pour le téléchargement et le dessin sur la carte. Le constructeur assure l'asynchronisme </summary>
    Private Class TuileCarte
        'position de la tuile par rapport à la carte. 1er octet de la tuile (0, 0) sur la 1ère ligne de la carte (Col (X), Row (Y))
        Private ReadOnly PointeurDestinationCarte As IntPtr
        Private Layers() As Layer
        Private ReadOnly Col As Integer
        Private ReadOnly Row As Integer
        ''' <summary> on a besoin que de l'index colonne et de l'index rangée pour définir la tuile. Le reste des paramètres est commun </summary>
        ''' <param name="Col"> index da la colonne</param>
        ''' <param name="Row"> index de la rangée </param>
        Friend Sub New(Col As Integer, Row As Integer)
            Me.Col = Col
            Me.Row = Row
            'calcul du pointeur de la tuile par rapport à la carte
            Dim DecalX As Integer = (Col - SurfaceCapture.NumColDeb) * NbOctetsDecalXTuile
            'dans le cas d'un support de carte fichier on télécharge 1 rangée uniquement
            Dim DecalY As Integer = If(CartesSettings.SUPPORT_CARTE <> TypeSupportCarte.Fichier, (Row - SurfaceCapture.NumRangDeb) * CarteSource.LargeurOctets * NbPixelsTuile, 0)
            PointeurDestinationCarte = ImageBitsPtr + DecalX + DecalY
            If FlagContinue Then
                ComposerTuileAsync()
            Else
                Interlocked.Increment(NbTuilesTelechargees)
                Interlocked.Increment(NbTuilesRangeeTelechargees)
                AfficherVisueInformation($"{MessageInfo}{NbTuilesTelechargees} / {SurfaceCapture.NbTuiles}")
            End If
        End Sub
        ''' <summary> télécharge et dessine sur la carte jusqu'à 3 couches </summary>
        Private Async Sub ComposerTuileAsync()
            ReDim Layers(ServeurCarto.NbLayers - 1)
            For Cpt As Integer = 0 To ServeurCarto.NbLayers - 1
                If FlagContinue Then
                    Dim IndiceLayer As Integer = Cpt 'obligatoire car on lance une tache
                    Dim Uri As String = ServeurCarto.UriTuileAffichage(IndiceLayer, SurfaceCapture.Couche, Col, Row)
                    Layers(IndiceLayer) = New Layer() With {.Uri = Uri, .IndiceLayer = IndiceLayer, .Statut = StatutTuile.DemandeTeleChargement}
                    Await TelechargerLayerAsync(IndiceLayer)
                Else
                    Exit For
                End If
            Next
            'et quand toutes les couches de la tuile sont téléchargées on les dessine sur la carte
            If FlagContinue Then ComposerImageTuile()
            'on indique que la tuile à finie sa tâche
            Interlocked.Increment(NbTuilesTelechargees)
            Interlocked.Increment(NbTuilesRangeeTelechargees)
            AfficherVisueInformation($"{MessageInfo}{NbTuilesTelechargees} / {SurfaceCapture.NbTuiles}")
        End Sub
        ''' <summary> télécharge la couche. Essaie jusqu'à 10 fois avant d'abandonner </summary>
        ''' <param name="IndiceLayer"></param>
        ''' <returns> pas de retour direct mais le byte() de la couche est remplit en cas de succès </returns>
        Private Async Function TelechargerLayerAsync(IndiceLayer As Integer) As Task
            Dim Cpt As Integer
            Dim Couche As Layer = Layers(IndiceLayer)
            Do
                If FlagArret Then Exit Do
                'on demande le téléchargement si le nb de requêtes ne depasse pas la limite autorisée
                If ServeurCarto.NbRequetes(IndiceLayer) < 3 Then
                    Await TelechargerDataLayerAsync(IndiceLayer)
                    If Couche.Statut = StatutTuile.ErreurServeur Then
                        'connexion internet tombée ou autre
                        Cpt += 1
                    Else
                        Exit Do 'on a fini le téléchargement de la couche 
                    End If
                End If
                If FlagContinue Then Await Task.Delay(IntAleatoire(50, 350))
            Loop While Cpt < 10
        End Function
        ''' <summary> télécharge la couche à partir d'un serveur graphique et d'une URI </summary>
        ''' <param name="IndiceLayer"></param>
        ''' <returns> Pas de retour direct mais modification du statut de la couche. 3 possibilités </returns>
        Private Async Function TelechargerDataLayerAsync(IndiceLayer As Integer) As Task
            Dim Couche As Layer = Layers(IndiceLayer)
            Interlocked.Increment(ServeurCarto.NbRequetes(IndiceLayer))
            Try
                Using Reponse As HttpResponseMessage = Await ServeurCarto.RequeteHttp.GetAsync(Couche.Uri)
                    If Reponse.StatusCode = HttpStatusCode.OK Then
                        Couche.Bits = Await Reponse.Content.ReadAsByteArrayAsync()
                        Couche.Statut = StatutTuile.Serveur
                    ElseIf Reponse.StatusCode = HttpStatusCode.BadRequest OrElse Reponse.StatusCode = HttpStatusCode.NotFound Then
                        Couche.Statut = StatutTuile.Inexistant
                    Else
                        Couche.Statut = StatutTuile.ErreurServeur 'erreur gérée en interne de la couche HttpClient
                    End If
                End Using
            Catch
                'erreur gérée en externe de la couche HttpClient. Généralement annulation de la tache liées à Await sur timeout
                'problématique d'avoir un message. On gère uniquement le statut de la tuile
                'Ex As Exception
                'AfficherErreur(Ex, "Y0V7")
                Couche.Statut = StatutTuile.ErreurServeur
            End Try
            Interlocked.Decrement(ServeurCarto.NbRequetes(IndiceLayer))
        End Function
        ''' <summary> ajoute les couches de 1 à 3 sur la carte. La 1ere est forcément le fond de plan, les 2 autres ont forcément un fond transparent </summary>
        Private Sub ComposerImageTuile()
            'delimitation de la zone de dessin sur la carte
            Using Tuile As New Bitmap(NbPixelsTuile, NbPixelsTuile, CarteSource.LargeurOctets, FormatPixel, PointeurDestinationCarte)
                Using G As Graphics = Graphics.FromImage(Tuile)
                    For Each Layer In Layers
                        If FlagArret Then Exit For
                        If Layer.Statut = StatutTuile.Serveur Then 'il faut que le layer soit téléchargé pour le dessiner
                            Using LayerImage As New Bitmap(New MemoryStream(Layer.Bits))
                                'dessinne la couche sur la carte en fonction du coeficient de transparence et du format de la couche (PNG ou Jpeg)
                                AjouterImageLayer(G, LayerImage, ServeurCarto.CoefAlphaLayer(Layer.IndiceLayer))
                            End Using
                        End If
                    Next
                End Using
            End Using
        End Sub
        ''' <summary> ajoute la couche sur l'image de la tuile avec ou sans transparence</summary>
        ''' <param name="G"> graphics où l'on doit dessinner la couche </param>
        ''' <param name="LayerImage"> couche à dessinner </param>
        ''' <param name="CoefAlpha"> coed de transparence de la couche </param>
        Private Shared Sub AjouterImageLayer(G As Graphics, LayerImage As Bitmap, CoefAlpha As Single)
            'si le coef de transparence est 1, complètement opaque, on peut aller vite 
            If CoefAlpha = 1 Then 'la couche est opaque hors le fond qui peut être transparent (format PNG uniquement)
                G.DrawImage(LayerImage, Point.Empty)
            Else 'la couche n'est pas opaque
                Dim imageAttributes As New ImageAttributes
                imageAttributes.SetColorMatrix(New ColorMatrix With {.Matrix33 = CoefAlpha}, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                'Le fond est transparent à 100% (image PNG obligatoire), le coef alpha de l'image peut varié de 100% (Opaque) à 0% (Invisible)
                G.DrawImage(LayerImage, New Rectangle(Point.Empty, New Size(NbPixelsTuile, NbPixelsTuile)), 0, 0, NbPixelsTuile, NbPixelsTuile, GraphicsUnit.Pixel, imageAttributes)
            End If
        End Sub
        ''' <summary> donne une structure qui permet de receuillir le téléchargement d'une tuile sur le serveur </summary>
        Private Class Layer
            Friend Bits As Byte()
            Friend IndiceLayer As Integer
            Friend Statut As StatutTuile
            Friend Uri As String
        End Class
    End Class
End Module