Partial Friend Class ServeurCarto

    ''' <summary> Décrit un layer associé à un serveur carto </summary>
    Private Class Layer
#Region "Champs publiques"
        ''' <summary> nb d'échelles ou de couches -1 (indice base zéro) </summary>
        Friend NbEchelles As Integer
        ''' <summary> tableau des libellés d'échelle de la couche </summary>
        Friend Echelles() As String
        ''' <summary> tableau des numéros de couche </summary>
        Friend Couches() As Byte
        ''' <summary> indice de la couche dans le tableau des couches du serveur </summary>
        Friend Indice As Integer
        ''' <summary>nom de la couche dans le fichier xml des serveurs </summary>
        Friend Nom As String
        ''' <summary> identifiant de la couche </summary>
        Friend Identifier As String
        ''' <summary> style de la couche renvoyé par le serveur </summary>
        Friend Style As String
        ''' <summary> date des informations de la couche renvoyées par le serveur </summary>
        Friend Time As String
        ''' <summary> format graphique de la couche exprimé en type mime</summary>
        Friend Format As String
        ''' <summary> valeur de transparence de la couche de 0: invisible à 1: opaque </summary>
        Friend CoefAlpha As Single
        ''' <summary> Début de l'url commun à toutes les tuiles de ce layer </summary>
        Friend UriDeb As String
        ''' <summary> partie de l'url correspondant au format des tuiles de ce layer </summary>
        Friend UriFormat As String
        ''' <summary> format graphique de la couche exprimé en image.format du .net </summary>
        Friend ReadOnly Property FormatNet As ImageFormat
            Get
                Return If(Format.ToLower = "png", ImageFormat.Png, ImageFormat.Jpeg)
            End Get
        End Property
#End Region
        ''' <summary> calule la partie numéro de la tuile </summary>
        ''' <param name="Couche"> numéro de la couche serveur de la tuile </param>
        ''' <param name="NumCol"> numéro de colonne de la tuile </param>
        ''' <param name="NumRow"> numéro de rangée de la tuile</param>
        ''' <returns> une chaine ayant le format attendu en fonction des caractéristiques du serveur </returns>
        Friend Function CalculerUriTilePosition(Couche As Byte, NumCol As Integer, NumRow As Integer) As String
            If ServeurType = TypesServeur.KVP Then
                CalculerUriTilePosition = $"&TileMatrix={Couche}&TileCol={NumCol}&TileRow={NumRow}"
            Else
                If SensColRow Then
                    CalculerUriTilePosition = $"/{Couche}/{NumCol}/{NumRow}{UriFormat}"
                Else
                    CalculerUriTilePosition = $"/{Couche}/{NumRow}/{NumCol}{UriFormat}"
                End If
            End If
        End Function
        ''' <summary> initialise la chaine de début de l'adresse d'une tuile </summary>
        ''' <param name="Url"> Url du serveur </param>
        Friend Sub Initialiser(Url As String)
            'valeur commune à toutes les tuiles de la couche d'affichage concernant le format des tuiles
            UriFormat = FormatFichierTile()
            'valeur commune à toutes les tuiles de la couche d'affichage concernant le début de l'adresse des tuiles
            If ServeurType = TypesServeur.KVP Then
                UriDeb = $"{Url}?layer={Identifier}&style={Style}&tilematrixset={TileMatrix}&Service=WMTS&Request=GetTile&Version=1.0.0{UriFormat}"
            ElseIf ServeurType = TypesServeur.REST Then
                UriDeb = $"{Url}/1.0.0/{Identifier}/{Style}/{Time}/{EPSG}"
            Else 'TypesServeur.OSM
                UriDeb = Url
            End If
        End Sub
        ''' <summary> calcule la partie format de la tuile </summary>
        ''' <param name="Format"> "jpeg" ou "png" </param>
        ''' <returns>  une chaine ayant le format attendu en fonction des caractéristiques du serveur </returns>
        Private Function FormatFichierTile() As String
            If ServeurType = TypesServeur.KVP Then
                FormatFichierTile = "&Format=image/" & Format
            Else 'TypeRest ou TypeOSM
                FormatFichierTile = "." & Format
            End If
        End Function
        ''' <summary> initialise les variables communes à tous les layers du serveurs </summary>
        ''' <param name="TypeServeur"> le Type de serveur des layers </param>
        ''' <param name="TileMatrixSet"> la matrice pour les tuiles des layers </param>
        ''' <param name="EPSG_Rendu"> le numéro de la projection associé à la matrice des tuiles </param>
        ''' <param name="XY"> le sens des colonnes / Rangées pour le calcul de la position </param>
        Friend Shared Sub InitialiserLayers(TypeServeur As TypesServeur, TileMatrixSet As String, EPSG_Rendu As String, XY As Boolean)
            ServeurType = TypeServeur
            TileMatrix = TileMatrixSet
            EPSG = EPSG_Rendu
            SensColRow = XY
        End Sub
        Private Shared ServeurType As TypesServeur
        ''' <summary> nom del'ensemble des matrices de tuile par niveau de zoom associé à la couche </summary>
        Private Shared TileMatrix As String
        ''' <summary> projection cartographique du serveur </summary>
        Private Shared EPSG As String
        ''' <summary> si true les X puis les Y dans la demande, sinon on inverse </summary>
        Private Shared SensColRow As Boolean
    End Class
    Friend Class DemandeTuiles
        ''' <summary> Définition des méthodes qui écoutent l'évenement d'une demande de tuile </summary>
        ''' <param name="ID"> ID de la demande. Sera transmis à la méthode qui écoute l'évènement </param>
        Friend Delegate Sub DemandeTuilesHandler(ID As Integer, IsErreur As Boolean)
        ''' <summary> définition de l'évènement de la tuile </summary>
        Friend Custom Event Finished As DemandeTuilesHandler
            'Ajout d'une méthode qui écoute l'évenement
            AddHandler(value As DemandeTuilesHandler)
                OnFinished = value
            End AddHandler
            'Suppression d'une méthode qui écoute l'évenement
            RemoveHandler(value As DemandeTuilesHandler)
                OnFinished = Nothing
            End RemoveHandler
            'génération de l'évènement. il faut qu'il y ait une méthode d'écoute pour lancer l'évènement
            'On avertit si le nb d'erreur correspond au nb de demande sur le serveur
            RaiseEvent()
                Dim ErreursTotales = _ErreursServeur + _ErreursFCGP
                OnFinished(_ID, ErreursTotales > 0 AndAlso ErreursTotales = (_TotalTuiles - _Caches))
            End RaiseEvent
        End Event
        ''' <summary> nombre de tuiles concernées par la demande </summary> 
        Friend ReadOnly Property TotalTuiles As Integer
        ''' <summary> stats sur le nombre de tuiles en erreur, inexistantes, téléchargées, en cache, plus concerné par l'affichage et autres </summary>
        Friend ReadOnly Property ErreursServeur As Integer
        Friend ReadOnly Property ErreursFCGP As Integer
        Friend ReadOnly Property Inexistants As Integer
        Friend ReadOnly Property Serveurs As Integer
        Friend ReadOnly Property Caches As Integer
        Friend ReadOnly Property PlusConcerne As Integer
        Friend ReadOnly Property Autres As Integer
        ''' <summary> constructeur de la demande </summary>
        ''' <param name="NbTuiles"> nb de tuiles concernées par la demande </param>
        ''' <param name="NumCouche"> numéro de couche associé au layer d'affichage </param>
        ''' <param name="ServeurCarto"> serveur carto associée à la demande </param>
        Friend Sub New(NbTuiles As Integer, NumCouche As Byte, ServeurCarto As ServeurCarto)
            _ID = CptDemandesAffichage
            CptDemandesAffichage += 1
            _TotalTuiles = NbTuiles
            Serveur = ServeurCarto
            Serveur._NbTuilesDemandeAffichage += _TotalTuiles
            IsPentes = NumCouche = 255
            Couche = NumCouche
        End Sub
        ''' <summary> Création des tuiles correspondant à la surface de la demande. Ne peut pas être appelée
        ''' dans le constructeur à cause de l'asynchronisme. Doit être appelé après la création
        ''' car la suppression de la demande peut avoir lieu avant le retour de la création 
        ''' L'appelant peut s'abonner à l'évenement OnFinshed pour être prévenu de la réalisation de la demande </summary>
        ''' <param name="Surface"> Surface exprimée en tuile </param>
        Friend Async Function CreerTuilesAffichageAsync(Surface As Rectangle) As Task
            For Col As Integer = Surface.Left To Surface.Right - 1
                For Row As Integer = Surface.Top To Surface.Bottom - 1
                    Dim Info = New TuileAffichage(Col, Row, Me)
                Next
            Next
            'pour que le compilateur accepte async car l'asynchronisme est au niveau du constructeur de la tuile
            Await Task.Delay(0)
        End Function
        ''' <summary> résume les principales propriétés de la demande </summary>
        Public Overrides Function ToString() As String
            Return $"DemandeTuile Id : {_ID}, Type : {If(IsPentes, "Pentes", "Fond")}, Nb Total Tuiles : {_TotalTuiles} --> C:{_Caches} S:{_Serveurs} I:{_Inexistants} P:{_PlusConcerne} E:{_ErreursServeur + _ErreursFCGP}"
        End Function
        ''' <summary> Identifiant unique de la demande </summary>
        Friend ReadOnly Property ID As Integer
        ''' <summary> les tuiles de la demande concerne la couche des pentes</summary>
        Private ReadOnly IsPentes As Boolean
        ''' <summary> Numéro du serveur de la couche de fond des tuiles de la demande </summary>
        Private ReadOnly Couche As Byte
        ''' <summary> Numéro du serveur de la couche de fond des tuiles de la demande </summary>
        Private ReadOnly Serveur As ServeurCarto
        ''' <summary> stocke les différents statut de sortie d'une tuile quand elle est finie </summary>
        ''' <param name="Statut"> statut de fin de la tuile </param>
        Private Sub EcrireStatut(Statut As StatutTuile)
            Select Case Statut
                Case StatutTuile.ErreurServeur
                    _ErreursServeur += 1
                Case StatutTuile.ErreurFCGP
                    _ErreursFCGP += 1
                Case StatutTuile.Cache
                    _Caches += 1
                Case StatutTuile.Serveur
                    _Serveurs += 1
                Case StatutTuile.Inexistant
                    _Inexistants += 1
                Case StatutTuile.PlusConcerne
                    _PlusConcerne += 1
                Case Else
                    _Autres += 1
            End Select
            Interlocked.Decrement(Serveur._NbTuilesDemandeAffichage)
            Interlocked.Increment(NbTuilesFinies)
            If NbTuilesFinies = _TotalTuiles Then RaiseEvent Finished()
        End Sub
        ''' <summary> Pointeur sur la ou les méthodes qui écoutent l'évènement de la demande </summary>
        Private OnFinished As DemandeTuilesHandler
        ''' <summary> stocke le nb de tuiles finies de la demande </summary>
        Private NbTuilesFinies As Integer
        ''' <summary> compteur des création de demande d'affichage </summary>
        Private Shared CptDemandesAffichage As Integer

        ''' <summary> sert pour le transport des informations nécessaires à la gestion d'une tuile servant à l'affichage
        ''' et à la demande qui lui est associée à travers les threads. Concerne le fond de plan et les pentes </summary>
        Friend Class TuileAffichage
            ''' <summary> index de la colonne de la tuile </summary>
            Friend ReadOnly Col As Integer
            ''' <summary> index de la rangéee de la tuile </summary>
            Friend ReadOnly Row As Integer
            ''' <summary> valeur du zoom de la tuile sous forme de texte </summary>
            Friend ReadOnly CoucheTxt As String
            ''' <summary> comment a été rempli le tampon de la tuile </summary>
            Friend StatutTuile As StatutTuile
            ''' <summary> tampon de la tuile représentant les octets d'un fichier image (jpeg) </summary>
            Friend BytesTuile As Byte()
            ''' <summary> constructeur de l'instance pour les couches de l'affichage et de la couche des pentes </summary>
            ''' <param name="Col"> index de la colonne de la tuile </param>
            ''' <param name="Row"> index de la rangéee de la tuile </param>
            ''' <param name="Demande"> demande associée à la tuile </param>
            Friend Sub New(Col As Integer, Row As Integer, Demande As DemandeTuiles)
                Me.Col = Col
                Me.Row = Row
                Index = New Point(Col, Row)
                Me.Demande = Demande
                Couche = Demande.Couche
                If Demande.IsPentes Then
                    CoucheTxt = Couche.ToString("Pentes")
                    Uri = Demande.Serveur.UriTuilePentes(Col, Row)
                    TelechargerTuilePentesAsync()
                Else
                    CoucheTxt = Couche.ToString("00")
                    Uri = Demande.Serveur.UriTuileAffichage(0, Couche, Col, Row)
                    'on cherche vraiment à télécharger car on essaie 5 fois
                    TelechargerTuileAffichageAsync()
                End If
            End Sub
            ''' <summary> télécharge et enregistre une tuile d'affichage dans le cache si elle n'est pas présente dans le cache 
            ''' on redemande jusqu'à 5 fois en cas de non réponse du serveur </summary>
            Private Async Sub TelechargerTuileAffichageAsync()
                Dim CptErreur As Integer
                StatutTuile = StatutTuile.PlusConcerne
                Do
                    'si la tuile n'est plus dans le périmètre de l'affichage on sort
                    If Not Demande.Serveur.Affichage.SurfaceFond.Contains(Index) OrElse Couche <> Demande.Serveur.Affichage.CoucheFond Then Exit Do
                    Await Demande.Serveur._Cache.LireOctetsTuileAsync(Me)
                    If StatutTuile <> StatutTuile.DemandeTeleChargement Then Exit Do
                    If Not IsConnected Then Exit Do
                    'si la tuile n'est plus dans le périmètre de l'affichage on sort
                    If Not Demande.Serveur.Affichage.SurfaceFond.Contains(Index) OrElse Couche <> Demande.Serveur.Affichage.CoucheFond Then Exit Do
                    'on demande le téléchargement si le nb de requêtes encours n'est pas trop important pour éviter de saturer le serveur 
                    Await TelechargerBytesTuileAsync()
                    If StatutTuile <> StatutTuile.ErreurServeur Then Exit Do 'on a fini et bien initialisé 
                    CptErreur += 1
                Loop While CptErreur < 5 'on limite à 5 demandes sur le serveur
                If CptErreur = 5 Then
                    StatutTuile = StatutTuile.ErreurServeur
                End If
                BytesTuile = Nothing
                Demande.EcrireStatut(StatutTuile)
            End Sub
            ''' <summary> télécharge et enregistre une tuile de la couche des pentes si elle n'est pas présente dans le cache </summary>
            Private Async Sub TelechargerTuilePentesAsync()
                If Not Await Demande.Serveur._Cache.ContenirLaTuileAsync(Me) Then
                    Await TelechargerBytesTuileAsync()
                Else
                    StatutTuile = StatutTuile.Cache
                End If
                BytesTuile = Nothing
                Demande.EcrireStatut(StatutTuile)
            End Sub
            ''' <summary> demande un tuile (image avec un format graphique compressé (jpeg, png) sous forme d'un tableau octets (blob).
            ''' elle est téléchargée à partir du serveur carto de manière asynchrone </summary>
            Private Async Function TelechargerBytesTuileAsync() As Task
                If Demande.Serveur._NbRequetes(0) >= MaxRequetesTuiles Then
                    StatutTuile = StatutTuile.PlusConcerne
                End If
                Interlocked.Increment(Demande.Serveur._NbRequetes(0))
                Try
                    'lance l'action de lecture de la tuile sur le serveur cartographique et ressort
                    Using ReponseHttp As HttpResponseMessage = Await Demande.Serveur._RequeteHttp.GetAsync(Uri)
                        If ReponseHttp.StatusCode = HttpStatusCode.OK Then
                            BytesTuile = Await ReponseHttp.Content.ReadAsByteArrayAsync()
                            StatutTuile = StatutTuile.Serveur
                            'enregistre la tuile dans le cache
                            Await Demande.Serveur._Cache.EcrireOctetsTuileAsync(Me)
                        ElseIf ReponseHttp.StatusCode = HttpStatusCode.BadRequest OrElse ReponseHttp.StatusCode = HttpStatusCode.NotFound Then
                            BytesTuile = New Byte() {0}
                            StatutTuile = StatutTuile.Inexistant
                        Else
                            'le code erreur renvoyé par le serveur carto n'est pas géré
                            StatutTuile = StatutTuile.ErreurServeur
                        End If
                    End Using
                Catch
                    'erreur géré par HttpClient sur timeOut (10secondes).
                    'Pour certains sites l'affichage du message d'erreur est problématique car le serveur est trop long à répondre
                    'on se contente du message au niveau de la suppression de la demande de tuile si il n'y a que des erreurs
                    'Ex As Exception
                    'AfficherErreur(Ex, "D3Y0")
                    StatutTuile = StatutTuile.ErreurFCGP
                End Try
                Interlocked.Decrement(Demande.Serveur._NbRequetes(0))
            End Function
            ''' <summary> demande associée à la tuile </summary>
            Private ReadOnly Demande As DemandeTuiles
            ''' <summary> Uri de la tuile </summary>
            Private ReadOnly Uri As String
            ''' <summary> indices de la tuile </summary>
            Private ReadOnly Index As Point
            ''' <summary> valeur du zoom de la tuile </summary>
            Private ReadOnly Couche As Byte
            ''' <summary> renvoie une chaine descriptive de tuileInfo </summary>
            Public Overrides Function ToString() As String
                Return $"ID : {Demande.ID}, Col : {Col}, Row : {Row}, Couche : {CoucheTxt}, Statut : {StatutTuile}"
            End Function
        End Class
    End Class
End Class