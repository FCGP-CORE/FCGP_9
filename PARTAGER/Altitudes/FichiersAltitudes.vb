''' <summary> gestions des Altitudes : Télechargement des fichiers DEM, Vérification des identifiants User, altitude d'un point </summary>
Friend Module Altitudes
#Region "Variables internes"
    Private Const URS As String = "https://urs.earthdata.nasa.gov"
    Private Const URI_Debut As String = "https://e4ftl01.cr.usgs.gov/MEASURES/SRTMGL"
    Private Const URI_Fin As String = ".003/2000.02.11/"
    Private Const ExtFich As String = ".hgt"
    Private Const TitleInformation As String = "Information Altidudes"
    Private Const TailleDem1 As Long = 3601 * 2 * 3601 '25934402
    Private Const TailleDem3 As Long = 1201 * 2 * 1201 '2884802
    ''' <summary>nb d'actions de téléchargement de fichiers d'altitudes possible en même temps</summary>
    Private Const Nb_Taches As Integer = 5
    ''' <summary> uri pour tester les identifiants de l'utilisateur. On est sur que le fichier existe </summary>
    Private Const URI_User As String = "https://e4ftl01.cr.usgs.gov/MEASURES/SRTMGL3.003/2000.02.11/N46E004.SRTMGL3.hgt.zip"
    ''' <summary> identifiants de l'utilisateur à tester </summary>
    Private ID_User, MP_User As String
    ''' <summary>adresse pour le téléchargement des fichiers d'altitudes</summary>
    Private ReadOnly Property UriFichiersAltitudes As String
        Get
            Return URI_Debut & AltitudesSettings.TYPE_DEM.ToString & URI_Fin
        End Get
    End Property
    ''' <summary> extensions des fichiers d'altitude sur le serveur car cela dépend du type du fichier </summary>
    Private ExtFichZip As String
    ''' <summary>emplacement des fichiers d'altitudes</summary>
    Private CheminFichiersAltitudes As String
    '''' <summary>tableau de tâches de téléchargement de fichiers d'altitudes possible en même temps</summary>
    Private ReadOnly ListeTaches As New Dictionary(Of Integer, Task)(Nb_Taches)
    ''' <summary> fichiers d'altitudes en cours de téléchargement </summary>
    Private ReadOnly ListeFichiers As New Dictionary(Of Integer, String)(Nb_Taches)
    ''' <summary> clef reliant tache et nom de fichiers d'altitudes </summary>
    Private Clef As Integer
    ''' <summary>liste des fichiers d'altitude présents dans le répertoire encours. Ils sont soient présents en local soit en statut téléchargement encours </summary>
    Private ListeFichiersAltitudes As SortedDictionary(Of String, StatutFichierAltitude)
    ''' <summary> fournisseur de requête pour les fichiers d'altitude </summary>
    Private HttpClientAltitude As HttpClient
    ''' <summary> le serveur de la NASA est sécurisé, il faut envoyer les identifiants de connexions </summary>
    Private MessageHandler As HttpClientHandler
    ''' <summary> flag indiquant que la connexion avec le serveur est établie </summary>
    Friend ReadOnly Property IsConnexion As Boolean
    ''' <summary> flag indiquant que l'initialisation des fichiers DEM est faite </summary>
    Friend ReadOnly Property IsOkFichiersAltitudes As Boolean
    ''' <summary> statut d'un fichier dont on a demandé le téléchargement </summary>
    Private Enum StatutFichierAltitude
        PresentDem3 = 1 ' soit à l'initialisation de la liste soit au retour du téléchargement si cela c'est bien passé en fonction de la taille
        PresentDem1 = 2 ' soit à l'initialisation de la liste soit au retour du téléchargement si cela c'est bien passé en fonction de la taille
        FichierInexistant = 3 'soit à l'initialisation de la liste soit au retour du serveur qui indique quele fichier n'existe pas
        EncoursChargementServeur = 4 ' le téléchargement est considéré en cours tant que la tache de téléchargement n'est pas finie
    End Enum
#End Region
#Region "Procédures externes"
    ''' <summary> mise à jour de la connection avec le serveur</summary>
    Friend Sub InitialiserConnexionAltitudes()
        _IsConnexion = False
        'on supprime l'ancienne connexion si il y en a une active
        If HttpClientAltitude IsNot Nothing Then
            HttpClientAltitude.Dispose()
            MessageHandler.Dispose()
        End If
        'pour avoir la connexion avec le serveur il faut que les 2 conditions
        If AltitudesSettings.IS_ALTITUDE AndAlso AltitudesSettings.IS_VALIDE Then
            'on met à jour les identifiants de connexion
            Dim NTC As New NetworkCredential(AltitudesSettings.ID, AltitudesSettings.MP)
            Dim myCredentials As ICredentials = New CredentialCache From {{New Uri(URS), "Basic", NTC}}
            MessageHandler = New HttpClientHandler() With {
                .Credentials = myCredentials,
                .PreAuthenticate = False,
                .AllowAutoRedirect = True}
            'on cré la nouvelle connexion
            HttpClientAltitude = New HttpClient(MessageHandler)
            _IsConnexion = True
        End If
    End Sub
    ''' <summary> initialise les différentes variables nécessaires au renvoi d'une altitude et regarde si il existe déjà des fichiers d'altitudes </summary>
    ''' <param name="FlagIdentifiants"> Indique que l'on veut aussi initialiser la connexion avec le serveur </param>
    Friend Sub InitialiserListeFichiersAltitudes()
        _IsOkFichiersAltitudes = False
        If ListeFichiersAltitudes Is Nothing Then
            ListeFichiersAltitudes = New SortedDictionary(Of String, StatutFichierAltitude)
        Else
            'nettoyage de la liste des fichiers
            ListeFichiersAltitudes.Clear()
        End If
        ExtFichZip = ".SRTMGL" & AltitudesSettings.TYPE_DEM.ToString & ".hgt.zip"
        CheminFichiersAltitudes = AltitudesSettings.CHEMIN_DEM
        For Each fichier As String In Directory.GetFiles(CheminFichiersAltitudes, "*.hgt", SearchOption.TopDirectoryOnly)
            'il ne peut y avoir que 3 tailles différentes pour les fichiers HGT : 0 = inexistant, 2884802 = DEM 3, 25934402 = DEM 1
            Dim Fi As New FileInfo(fichier)
            If Fi.Length = TailleDem1 Then
                ListeFichiersAltitudes.Add(Path.GetFileNameWithoutExtension(fichier), StatutFichierAltitude.PresentDem1)
            ElseIf Fi.Length = TailleDem3 Then
                ListeFichiersAltitudes.Add(Path.GetFileNameWithoutExtension(fichier), StatutFichierAltitude.PresentDem3)
            Else
                ListeFichiersAltitudes.Add(Path.GetFileNameWithoutExtension(fichier), StatutFichierAltitude.FichierInexistant)
            End If
        Next
        _IsOkFichiersAltitudes = True
    End Sub
    ''' <summary> Arrête le téléchargement encours des fichiers d'altitudes et libère les ressources </summary>
    Friend Sub CloturerAltitudes()
        'nettoyage des taches de téléchargement encore encours
        If ListeTaches.Count > 0 Then
            For Each KVP As KeyValuePair(Of Integer, Task) In ListeTaches
                Try
                    KVP.Value.Dispose()
                    ListeTaches.Remove(KVP.Key)
                    ListeFichiers.Remove(KVP.Key)
                Catch Ex As Exception
                    AfficherErreur(Ex, "4CFA")
                End Try
            Next
        End If
        'enregistrements des différentes variables associées aux altitudes
        If IsSettingsOk Then AltitudesSettings.Ecrire()
        'fermeture de la connexion avec le serveur
        If HttpClientAltitude IsNot Nothing Then
            HttpClientAltitude.Dispose()
            MessageHandler.Dispose()
        End If
    End Sub
    ''' <summary> Affiche des messages d'information liées au modules des Altitudes </summary>
    Private Sub AfficherInformationAltitudes()
        Dim Titre = TitreInformation
        TitreInformation = TitleInformation
        AfficherInformation()
        TitreInformation = Titre
    End Sub
    ''' <summary> cette fonction renvoie une altitude si le fichier correspondant à la longitude et à la latitude est présent dans le répertoire adéquat sinon
    ''' elle demande le téléchargement de celui-ci mais il faut 3 conditions </summary>
    ''' <param name="PT"> point représentant la longitude (X) et la latitude (Y)</param>
    ''' <returns>l'altitude du point en mètres</returns>
    ''' <remarks> -9999 si le fichier n'existe pas en local ou l'altitude dans le cas contraire </remarks>
    Friend Function TrouverAltitudeCoordonnees(PT As PointD) As Short
        Dim Altitude As Short = -9999
        If Not AltitudesSettings.IS_ALTITUDE OrElse Not IsConnexion OrElse Not IsConnected Then Return Altitude
        Dim Latitude As Double = Math.Floor(PT.Lat)
        Dim Longitude As Double = Math.Floor(PT.Lon)
        Dim FichierEncours As String = $"{If(Latitude < 0, "S"c, "N"c)}{Math.Abs(Latitude):00}" &
                                       $"{If(Longitude < 0, "W"c, "E"c)}{Math.Abs(Longitude):000}"
        'si le fichier est déja dans la liste c'est que soit il est présent en local,  soit le chargement est en cours, soit le est fichierinexistant ou en erreur
        If ListeFichiersAltitudes.ContainsKey(FichierEncours) Then
            If ListeFichiersAltitudes(FichierEncours) < StatutFichierAltitude.FichierInexistant Then
                Dim NbArc As Double, NbOctetsLigneFichierHGT As Integer
                NbArc = If(ListeFichiersAltitudes(FichierEncours) = StatutFichierAltitude.PresentDem1, 3600, 1200)
                NbOctetsLigneFichierHGT = CInt(NbArc * 2 + 2)
                'on ouvre le fichier des altitudes en lecture
                Using AltitudesStream As New FileStream(CheminFichiersAltitudes & "\" & FichierEncours & ".hgt", FileMode.Open, FileAccess.Read, FileShare.Read)
                    'on peut renvoyer une altitude (calcul)
                    'on calcule la position du premier octect du point connu entourant l'altitude du point cherché
                    Dim Y As Double = PT.Lat - Latitude
                    Y = (1 - Y) * NbArc
                    Dim X As Double = PT.Lon - Longitude
                    X *= NbArc
                    Dim PositionY As Integer = CInt(Math.Floor(Y))
                    Dim PositionX As Integer = CInt(Math.Floor(X))
                    'pas d'interpolation juste par rapport au plus proche
                    Dim PositionOctect = PositionY * NbOctetsLigneFichierHGT + PositionX * 2
                    AltitudesStream.Position = PositionOctect
                    Dim AltitudesCalculees0 As Double
                    'on lit l'atitude à la position du pointeur de fichier
                    Dim AltitudesPosition0 As Short = (CShort(AltitudesStream.ReadByte()) << 8) + CShort(AltitudesStream.ReadByte())
                    If (X - PositionX) < 0.0001 Then
                        'dans ce cas il n'y a pas d'interpolation sur les longitudes : exactement egale à un multiple d'arc
                        AltitudesCalculees0 = AltitudesPosition0
                    Else
                        Dim AltitudesPosition1 As Short = (CShort(AltitudesStream.ReadByte()) << 8) + CShort(AltitudesStream.ReadByte())
                        'interpolation de l'altitude sur la longitude du point cherché
                        AltitudesCalculees0 = AltitudesPosition0 + (AltitudesPosition1 - AltitudesPosition0) * (X - PositionX)
                    End If
                    If (Y - PositionY) < 0.0001 Then
                        'dans ce cas il n'y a pas d'interpolation sur les latitudes : exactement egale à un multiple d'arc
                        Altitude = CShort(AltitudesCalculees0)
                    Else
                        Dim AltitudesCalculees1 As Double
                        'on calcule la position du 3ème point connu entourant l'altitude du point cherché
                        AltitudesStream.Position = PositionOctect + NbOctetsLigneFichierHGT
                        'on lit l'altitude du 3ème point et du 4ème point car ils se suivent
                        AltitudesPosition0 = (CShort(AltitudesStream.ReadByte()) << 8) + CShort(AltitudesStream.ReadByte())
                        If (X - PositionX) < 0.01 Then
                            'dans ce cas il n'y a pas d'interpolation sur les longitudes 
                            AltitudesCalculees1 = AltitudesPosition0
                        Else
                            Dim AltitudesPosition1 As Short = (CShort(AltitudesStream.ReadByte()) << 8) + CShort(AltitudesStream.ReadByte())
                            'interpolation de l'altitude sur la longitude du point cherché
                            AltitudesCalculees1 = AltitudesPosition0 + (AltitudesPosition1 - AltitudesPosition0) * (X - PositionX)
                        End If
                        'interpolation de l'altitude sur la latitude du point cherché
                        Altitude = CShort(AltitudesCalculees1 + (AltitudesCalculees0 - AltitudesCalculees1) * (Y - PositionY))
                    End If
                End Using
            Else
                'le fichier est présent mais n'a pas le bon statut soit il est inexistant sur le serveur soit en cours de téléchargement
                'donc on renvoie 0
            End If
        ElseIf IsConnected Then 'le fichier n'est pas présent en local car jamais demandé ou en erreur, on demande le téléchargement du fichier en arrière plan
            Try
                If ListeTaches.Count < Nb_Taches Then 'on prend la 1ere dispo
                    Dim FichierZipEncours As String = FichierEncours & ExtFichZip
                    'on ajoute le fichier à télécharger dans la liste avec le statut chargementencours. c'est TelechargementFichierHGTAsync qui fixe le statut final
                    ListeFichiersAltitudes.Add(FichierEncours, StatutFichierAltitude.EncoursChargementServeur)
                    ListeFichiers.Add(Clef, FichierEncours)
                    ListeTaches.Add(Clef, TelechargerFichierHGTAsync(UriFichiersAltitudes & FichierZipEncours, CheminEnregistrementProvisoire & "\" & FichierZipEncours, Clef))
                    Clef += 1
                    MessageInformation = "Demande de téléchargement du fichier " & FichierEncours & ExtFich
                End If
            Catch ex As Exception
                AfficherErreur(ex, "6989")
                MessageInformation = "Demande de téléchargement du fichier " & FichierEncours & " annulée suite erreur sur le serveur"
            End Try
            If AltitudesSettings.IS_MSG Then AfficherInformationAltitudes()
        End If
        Return Altitude
    End Function
    ''' <summary> affiche un formulaire d'attente pendant la vérification des identifiants sur le site de la NASA </summary>
    ''' <param name="ID"> identifiant </param>
    ''' <param name="MP"> mot de passe </param>
    Friend Function VerifierIDAltitudes(ID As String, MP As String) As DialogResult
        ID_User = ID
        MP_User = MP
        Dim Titre = TitreInformation
        TitreInformation = TitleInformation
        MessageInformation = "Vérification des identifiants en cours ...."
        VerifierIDAltitudes = LancerTache(AddressOf VerifierID_User)
        TitreInformation = Titre
    End Function
    ''' <summary> télécharge de manière asynchrone un fichier hgt sur le site de données de la NASA </summary>
    ''' <param name="URI"> Non complet du fichier HGT sur le serveur </param>
    ''' <param name="FichierZipEncours"> Nom complet du fichier dans lequel sera enregistré le fichier hgt en local </param>
    ''' <param name="Cpt"> indice du téléchargement dans la liste des téléchargement en cours </param>
    Private Async Function TelechargerFichierHGTAsync(URI As String, FichierZipEncours As String, Cpt As Integer) As Task
        Try
            Dim response As HttpResponseMessage = Await HttpClientAltitude.GetAsync(URI)
            'à ce niveau on gère si le fichier existant ou non existant et on ignore les erreurs
            If response.StatusCode = HttpStatusCode.OK Then
                'il n'y a pas eu d'erreur de liaision avec le serveur. Les identifiants sont corrects et le fichier est existant on peut poursuivre le traitement
                'en copiant le fichier reçu du serveur de la NASA en local
                Using Source As Stream = Await response.Content.ReadAsStreamAsync
                    Using DestinationStream As FileStream = File.Create(FichierZipEncours)
                        Await Source.CopyToAsync(DestinationStream)
                    End Using
                End Using
                Await Task.Run(Sub() TelechargerFichierAltitudesZipComplet(FichierZipEncours, Cpt))
            ElseIf response.StatusCode = HttpStatusCode.NotFound Then
                'on indique dans la liste que le fichier est inexsistant pour éviter une nouvelle demande de téléchargement
                ListeFichiersAltitudes(ListeFichiers(Cpt)) = StatutFichierAltitude.FichierInexistant
                Dim FichierEncours As String = ListeFichiers(Cpt) + ExtFich
                MessageInformation = $"Le fichier d'altitude {FichierEncours} est inexistant"
                'on crée un fichier vide (0 octet) pour que les prochaines sessions sachent qu'il n'existe pas
                Using FS As New FileStream($"{CheminFichiersAltitudes}\{FichierEncours}", FileMode.Create)
                End Using
            Else
                'à ce niveau on gère les autres exceptions
                MessageInformation = response.ReasonPhrase.ToString
                ListeFichiersAltitudes.Remove(ListeFichiers(Cpt))
            End If
        Catch ex As Exception
            'à ce niveau on gère les exeptions qui ne sont pas gérées directement dans la classe HTTP
            AfficherErreur(ex, "H4M7")
            ListeFichiersAltitudes.Remove(ListeFichiers(Cpt))
        End Try
        ListeFichiers.Remove(Cpt)
        ListeTaches.Remove(Cpt)
        If AltitudesSettings.IS_MSG Then AfficherInformationAltitudes()
    End Function
    ''' <summary> Contient la logique du dézippage une fois que le fichier zippé des altitudes a été téléchargé sur le micro </summary>
    Private Sub TelechargerFichierAltitudesZipComplet(FichierZipEncours As String, Cpt As Integer)
        Dim FichierEncours As String = ListeFichiers(Cpt) & ExtFich
        MessageInformation = $"Le fichier d'altitude {FichierEncours}"
        FichierEncours = $"{CheminFichiersAltitudes}\{FichierEncours}"
        'on decompresse le fichier qui vient d'être téléchargé mais en vérifiant qu'il n'existe pas sur le disque pour éviter une erreur
        If File.Exists(FichierEncours) Then
            File.Delete(FichierEncours)
        End If
        ZipFile.ExtractToDirectory(FichierZipEncours, CheminFichiersAltitudes)
        Dim FichierInfo As New FileInfo(FichierEncours)
        'les fichiers font tous la même taille
        If FichierInfo.Length = TailleDem1 Then
            'on indique que le fichier est disponible en enlevant le statut chargement encours 
            ListeFichiersAltitudes(ListeFichiers(Cpt)) = StatutFichierAltitude.PresentDem1
            MessageInformation &= " a été téléchargé"
        ElseIf FichierInfo.Length = TailleDem3 Then
            'on indique que le fichier est disponible en enlevant le statut chargement encours 
            ListeFichiersAltitudes(ListeFichiers(Cpt)) = StatutFichierAltitude.PresentDem3
            MessageInformation &= " a été téléchargé"
        Else
            'comme le fichier n'est pas bon on le supprime physiquement et dans la liste pour pouvoir le re-télécharger
            FichierInfo.Delete()
            ListeFichiersAltitudes.Remove(ListeFichiers(Cpt))
            MessageInformation &= " n'a pas été téléchargé correctement"
        End If
    End Sub
    ''' <summary> Vérifie que les identifiants de l'utilisateur sont connus du site de téléchargement des fichiers HGT </summary>
    Private Function VerifierID_User() As DialogResult
        Dim Ret As DialogResult
        'on met à jour les identifiants de connexion
        Dim myCredentials As ICredentials = New CredentialCache From {{New Uri(URS), "Basic", New NetworkCredential(ID_User, MP_User)}}
        Using MessageHandler = New HttpClientHandler() With {
            .Credentials = myCredentials,
            .PreAuthenticate = False,
            .AllowAutoRedirect = True}
            'on crée la nouvelle connexion
            Using HttpClientAltitude As New HttpClient(MessageHandler)
                Try
                    'Execute la requête
                    Dim response As HttpResponseMessage = HttpClientAltitude.GetAsync(URI_User).Result
                    'à ce niveau on regarde simplement si la requête est Ok. Cela indique que les identifiants sont corrects car
                    'le fichier existe sur le serveur.
                    If response.StatusCode = HttpStatusCode.OK Then
                        Ret = DialogResult.OK
                    Else
                        Ret = DialogResult.Cancel
                    End If
                Catch ex As Exception
                    'Mais le serveur peut être tombé!!!
                    MessageInformation = "Le serveur EarthData est indisponible." & CrLf & "Veuillez ré-essayer plus tard."
                    AfficherInformation()
                    'il s'agit d'une erreur non gérée par la classe HTTP
                    Ret = DialogResult.Cancel
                End Try
            End Using
        End Using
        Return Ret
    End Function
#End Region
End Module