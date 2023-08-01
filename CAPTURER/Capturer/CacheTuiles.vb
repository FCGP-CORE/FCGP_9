Partial Friend Class ServeurCarto
    ''' <summary> types possibles pour un serveur WMTS </summary>
    Private Enum TypesServeur As Integer
        KVP = 0
        REST = 1
        OSM = 2
    End Enum
    Friend Enum StatutTuile As Integer
        ''' <summary> valeur à la création d'une tuile visue </summary>
        Inconnu = -1
        ''' <summary> réserver en interne. Indique que la tuile n'est pas dans le cache et qu'il faut la télécharger </summary>
        DemandeTeleChargement = 0
        ''' <summary> indique qu'il y a eu une erreur de la part du serveur et gérée par HttpClient </summary>
        ErreurServeur = 1
        ''' <summary> indique que le tampon de la tuile a été rempli à partir du cache </summary>
        Cache = 2
        ''' <summary> indique que le tampon de la tuile a été rempli à partir du serveur </summary>
        Serveur = 3
        ''' <summary> indique que la tuile n'existe pas sur le serveur ou qu'elle ne contient pas assez d'informations pour être dessinner </summary>
        Inexistant = 4
        ''' <summary> indique que la tuile n'est plus concernée par le tampon encours </summary>
        PlusConcerne = 5
        ''' <summary> indique qu'il y a eu une erreur interne à FCGP </summary>
        ErreurFCGP = 6
    End Enum
    ''' <summary> Gestion de la base de données des tuiles d'un serveur carto </summary>
    Friend Class CacheTuiles
        Implements IDisposable
#Region "Champs et Proprietes"
        ''' <summary> renvoie vrai si tout est ok lors de l'initialisation du cache </summary>
        Friend ReadOnly Property IsOk As Boolean
        ''' <summary> permet la connection avec le cache </summary>
        Private _Connection As SQLiteConnection
        ''' <summary> permet la connection avec le cache </summary>
        Private ReadOnly Property Connection() As SQLiteConnection
            Get
                If _Connection Is Nothing Then
                    _Connection = New SQLiteConnection($"Data Source={NomBaseCacheTuile};")
                    CommandConnection = _Connection.CreateCommand()
                End If
                Return _Connection
            End Get
        End Property
        ''' <summary> pour passer de commandes SQL à la base de données </summary>
        Private CommandConnection As SQLiteCommand
        ''' <summary> nom de la base de données SQLite. C'est toujours le même</summary>
        Private NomBaseCacheTuile As String
        ''' <summary> serveur carto associé au cache tuiles </summary>
        Private ReadOnly Serveur As ServeurCarto
        ''' <summary> clé du site carto </summary>
        Private ReadOnly Property Cle As String
        ''' <summary> indique si la carte a été détruite</summary>
        Private disposed As Boolean
#End Region
#Region "functions et requêtes asynchrone"
        ''' <summary> lit à partir du cache le tableau de bytes correspondant au champ IMAGE d'une tuile d'affichage de manière asynchrone </summary>
        ''' <param name="Col"> Index de colonne </param>
        ''' <param name="Row"> Index de rangée </param>
        ''' <param name="Couche"> N° de couche de la tuile </param> 
        Friend Async Function LireOctetsTuileAsync(Col As Integer, Row As Integer, Couche As String) As Task(Of Byte())
            CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};"
            Return CType(Await CommandConnection.ExecuteScalarAsync(), Byte())
        End Function
        ''' <summary> lit à partir du cache le tableau de bytes correspondant au champ IMAGE d'une tuile de pentes de manière asynchrone </summary>
        ''' <remarks> le paramètre Info sert pour aussi pour le retour </remarks>
        ''' <param name="Col"> Index de colonne </param>
        ''' <param name="Row"> Index de rangée </param>
        Friend Async Function LireOctetsTuileAsync(Col As Integer, Row As Integer) As Task(Of Byte())
            CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_Pentes] Where [COL]={Col} And [ROW]={Row};"
            Return CType(Await CommandConnection.ExecuteScalarAsync(), Byte())
        End Function
        ''' <summary> lit à partir du cache le tableau de bytes correspondant au champ JPEG de manière asynchrone </summary>
        ''' <param name="Info"> Tuile affichage contenant les infos </param>
        ''' <remarks> le paramètre Info sert pour le retour </remarks>
        Friend Async Function LireOctetsTuileAsync(Info As DemandeTuiles.TuileAffichage) As Task
            CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_{Info.CoucheTxt}] Where [COL]={Info.Col} And [ROW]={Info.Row};"
            Info.BytesTuile = CType(Await CommandConnection.ExecuteScalarAsync(), Byte())
            If Info.BytesTuile Is Nothing Then 'indique que la tuile n'est pas présente dans le cache et qu'il faut éventuellement la télécharger pour l'obtenir
                Info.StatutTuile = StatutTuile.DemandeTeleChargement
            ElseIf Info.BytesTuile.Length > 1 Then 'indique que la tuile est présente dans le cache et que le tampon est rempli
                Info.StatutTuile = StatutTuile.Cache
            Else 'indique que la tuile est dans le cache mais que la tuile n'existe pas sur le serveur
                Info.StatutTuile = StatutTuile.Inexistant
            End If
        End Function
        ''' <summary> écrit une tuile dans le cache de manière asynchrone. </summary>
        ''' <remarks> on test pour savoir si la tuile existe afin d'éviter une erreur potentielle car elle a pu être enregistrée 
        ''' à partir d'une autre demande de tuile que celle qui appelle la méthode </remarks>
        Friend Async Function EcrireOctetsTuileAsync(Info As DemandeTuiles.TuileAffichage) As Task
            If Not Await ContenirLaTuileAsync(Info) Then
                'insertion d'un seul enregistrement
                Using EcritureBase = Connection.BeginTransaction()
                    CommandConnection.CommandText = $"Insert Into [Tuiles_{Info.CoucheTxt}] ([COL], [ROW], [IMAGE]) Values ({Info.Col}, {Info.Row}, @Image);"
                    CommandConnection.Parameters.Add(New SQLiteParameter("@Image", DbType.Binary) With {.Value = Info.BytesTuile})
                    Await CommandConnection.ExecuteNonQueryAsync()
                    EcritureBase.Commit()
                End Using
            Else
                If Info.BytesTuile.Length > 1 Then
                    Info.StatutTuile = StatutTuile.Cache
                Else
                    Info.StatutTuile = StatutTuile.Inexistant
                End If
            End If
        End Function
        ''' <summary> renvoie de manière asynchrone true si la tuile existe </summary>
        Friend Async Function ContenirLaTuileAsync(Info As DemandeTuiles.TuileAffichage) As Task(Of Boolean)
            CommandConnection.CommandText = $"Select [Rowid] From [Tuiles_{Info.CoucheTxt}] Where [Col]={Info.Col} And [Row]={Info.Row};"
            Return CBool(Await CommandConnection.ExecuteScalarAsync())
        End Function
#End Region
#Region "requêtes synchrones"
        ''' <summary> Lit de façon synchrone une tuile de la couche pentes dans le cache </summary>
        ''' <param name="Col"></param>
        ''' <param name="Row"></param>
        Friend Function LireOctetsTuile(Col As Integer, Row As Integer, Couche As String) As Byte()
            CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};"
            Return CType(CommandConnection.ExecuteScalar(), Byte())
        End Function
        ''' <summary> écrit une tuile dans le cache de manière synchrone </summary>
        Friend Sub EcrireOctetsTuile(Col As Integer, Row As Integer, Couche As String, BytesTuile As Byte())
            'insertion d'un seul enregistrement
            Using Transaction = Connection.BeginTransaction()
                CommandConnection.CommandText = $"Insert Into [Tuiles_{Couche}] ([COL], [ROW], [IMAGE]) Values ({Col}, {Row}, @Image);"
                CommandConnection.Parameters.Add(New SQLiteParameter("@Image", DbType.Binary) With {.Value = BytesTuile})
                CommandConnection.ExecuteNonQuery()
                Transaction.Commit()
            End Using
        End Sub
        ''' <summary> Efface dans le cache la tuile passée en paramètre. Pas de controle sur l'existence de celle-ci </summary>
        Friend Sub EffacerTuile(Col As Integer, Row As Integer, Couche As String)
            Using EcritureBase = Connection.BeginTransaction()
                CommandConnection.CommandText = $"Delete From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};"
                CommandConnection.ExecuteNonQuery()
                EcritureBase.Commit()
            End Using
        End Sub
        ''' <summary> renvoie de manière synchrone true si la tuile existe </summary>
        Friend Function ContenirLaTuile(Col As Integer, Row As Integer, Couche As String) As Boolean
            CommandConnection.CommandText = $"Select [Rowid] From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};"
            Return CBool(CommandConnection.ExecuteScalar())
        End Function
        ''' <summary> renvoie le nb de tuiles contenues dans le cahce et qui appartiennent à une region </summary>
        ''' <param name="ColMin"> index Colonne mini </param>
        ''' <param name="ColMax"> index Colonne maxi </param>
        ''' <param name="RowMin"> index Rangéée mini </param>
        ''' <param name="RowMax"> index Rangéée maxi </param>
        ''' <param name="Couche"> valeur du zoom des tuiles </param>
        Friend Function TrouverNbTuilesRegion(ColMin As Integer, ColMax As Integer, RowMin As Integer, RowMax As Integer, Couche As Integer) As Integer
            CommandConnection.CommandText =
            $"Select Count(*) From [Tuiles_{Couche:00}] Where [COL] Between {ColMin} And {ColMax} And [ROW] Between {RowMin} And {RowMax};"
            Return CInt(CommandConnection.ExecuteScalar())
        End Function
        ''' <summary> renvoie sous forme de liste de tuple les tuiles contenues dans le cahce et qui appartiennent à une region </summary>
        ''' <param name="ColMin"> index Colonne mini </param>
        ''' <param name="ColMax"> index Colonne maxi </param>
        ''' <param name="RowMin"> index Rangéée mini </param>
        ''' <param name="RowMax"> index Rangéée maxi </param>
        ''' <param name="Couche"> valeur du zoom des tuiles </param>
        Friend Function RenvoyerTuilesRegion(ColMin As Integer, ColMax As Integer, RowMin As Integer, RowMax As Integer, Couche As String) _
                                             As List(Of (Col As Integer, Row As Integer, OctetsTuile As Byte()))
            CommandConnection.CommandText = $"Select [COL], [ROW], [IMAGE] From [Tuiles_{Couche}] Where [COL] Between {ColMin} " &
                                            $"And {ColMax} And [ROW] Between {RowMin} And {RowMax};"
            RenvoyerTuilesRegion = New List(Of (Col As Integer, Row As Integer, OctetsTuile As Byte()))
            Using Reponse = CommandConnection.ExecuteReader
                Do While Reponse.Read
                    RenvoyerTuilesRegion.Add((CInt(Reponse(0)), CInt(Reponse(1)), CType(Reponse(2), Byte())))
                Loop
            End Using
        End Function
        ''' <summary> renvoie le nb de tuiles contenues dans le cache concernant une rangée compris entre 2 bornes délimitant les colonnes </summary>
        ''' <param name="ColMin"> index Colonne mini </param>
        ''' <param name="ColMax"> index Colonne maxi </param>
        ''' <param name="Row"> index de la Rangée concernée </param>
        ''' <param name="Couche"> valeur du zoom des tuiles </param>
        Friend Function TrouverNbTuilesRow(ColMin As Integer, ColMax As Integer, Row As Integer, Couche As String) As Integer
            CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche}] Where [Col] Between {ColMin} And {ColMax} And [Row] = {Row};"
            Return CInt(CommandConnection.ExecuteScalar())
        End Function
        ''' <summary> renvoie le nb de tuiles contenues dans le cache concernant une colonne compris entre 2 bornes délimant les rangées </summary>
        ''' <param name="Col"> index de la Colonne concernée </param>
        ''' <param name="RowMin"> index Rangéée mini </param>
        ''' <param name="RowMax"> index Rangéée maxi </param>
        ''' <param name="Couche"> valeur du zoom des tuiles </param>
        ''' <returns></returns>
        Friend Function TrouverNbTuilesCol(Col As Integer, RowMin As Integer, RowMax As Integer, Couche As String) As Integer
            CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche}] Where [Col] = {Col} And [Row] Between {RowMin} And {RowMax};"
            Return CInt(CommandConnection.ExecuteScalar())
        End Function
        ''' <summary> renvoie le nb de tuiles contenues dans le cache concernant un zoom donné </summary>
        ''' <param name="Couche"> valeur du zoom des tuiles </param>
        Friend Function TrouverNbEnregTable(Couche As String) As Integer
            CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche}];"
            Return CInt(CommandConnection.ExecuteScalar())
        End Function
        ''' <summary>  renvoie sous forme de tuple les stats concernant les tuiles contenues dans le cache pour un zoom donné </summary>
        ''' <param name="Couche"> valeur du zoom des tuiles </param>
        Friend Function TrouverStatsTable(Couche As String) As (NbTuiles As Integer, MinCol As Integer, MaxCol As Integer, MinRow As Integer, MaxRow As Integer)
            CommandConnection.CommandText = $"Select Count(*), Min([Col]), Max([Col]), Min([Row]), Max([Row]) From [Tuiles_{Couche}];"
            Using Reponse = CommandConnection.ExecuteReader
                Reponse.Read()
                Return (CInt(Reponse(0)), CInt(Reponse(1)), CInt(Reponse(2)),
                        CInt(Reponse(3)), CInt(Reponse(4)))
            End Using
        End Function
        ''' <summary> efface les tuiles du cache contenues dans la région </summary>
        ''' <param name="Region"> region contenant les tuile à effacer </param>
        Friend Sub EffacerSurfaceTuile(Region As SurfaceTuiles)
            Using EcritureBase = Connection.BeginTransaction()
                CommandConnection.CommandText = $"Delete From [Tuiles_{Region.Couche:00}] Where [Col] Between {Region.NumColDeb} " &
                                                $"And {Region.NumColFin} And [Row] Between {Region.NumRangDeb} And {Region.NumRangFin};"
                CommandConnection.ExecuteNonQueryAsync()
                EcritureBase.Commit()
            End Using
        End Sub
#End Region
#Region "Création"
        ''' <summary> initialise un nouveau cache soit en le créant soit en l'ouvrant si il existe </summary>
        ''' <param name="ServeurCarto"> serveur carto associé au cache </param>
        Friend Sub New(ServeurCarto As ServeurCarto)
            Serveur = ServeurCarto
            _Cle = If(Serveur.IndiceFondPlan = 0, Serveur.Cle, $"{Serveur.Cle}_{Serveur.IndiceFondPlan}")
            NomBaseCacheTuile = Serveur.CheminCache & "CacheTuiles" & Cle & ".db"
            IsOk = Initialiser()
            If IsOk Then
                Connection.Open()
            End If
        End Sub
        ''' <summary> Change le répertoire du cache tuile en créant un nouveau cache si besoin </summary>
        ''' <param name="NouveauRepertoire"> Repertoire où doit se trouver le cache </param>
        Friend Function ChangerRepertoireCaches(NouveauRepertoire As String) As Boolean
            Dim Ret As Boolean = False
            Try
                Dim AnciennBase As String = NomBaseCacheTuile
                CommandConnection.Dispose()
                'Fermer la connection
                _Connection.Close()
                _Connection.Dispose()
                ' on force la réinitialisation
                _Connection = Nothing
                Serveur.CheminCache = NouveauRepertoire
                NomBaseCacheTuile = NouveauRepertoire & "\CacheTuiles" & _Cle & ".db"
                Ret = Initialiser()
                If Not Ret Then
                    NomBaseCacheTuile = AnciennBase
                    MessageInformation = "Le nouveau cache tuiles n'a pas pu être initialisé" & CrLf &
                                         "L'ancien cache tuiles reste actif."
                    AfficherInformation()
                End If
                Connection.Open()
            Catch Ex As Exception
                AfficherErreur(Ex, "T6Q5")
            End Try
            Return Ret
        End Function
        ''' <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
        Protected Overridable Sub Dispose(disposing As Boolean)
            If disposed Then Exit Sub
            If disposing Then
                If _Connection IsNot Nothing Then
                    CommandConnection.Dispose()
                    _Connection.Close()
                    _Connection.Dispose()
                End If
            End If
            disposed = True
        End Sub
        ''' <summary> crée la base si elle n'existe pas et gère les erreurs lors de la création </summary>
        Private Function Initialiser() As Boolean
            Dim Ret As Boolean = True
            If Not File.Exists(NomBaseCacheTuile) Then
                Dim CouchesPossiblesServeur() As Byte
                'Si le site est DomTom, il faut prévoir les tables des couches pour celui qui en a le plus c'est à dire la Guyane
                If _Cle = "DT" Then
                    CouchesPossiblesServeur = NiveauDetailCartographique.CouchesSite(Serveur.SiteCarto, DomsToms.guyane)
                Else
                    CouchesPossiblesServeur = Serveur.CouchesLayer(0)
                End If
                Connection.Open()
                CommandConnection.CommandText = "PRAGMA main.page_size = 8192; VACUUM; PRAGMA journal_mode = 'WAL';PRAGMA journal_size_limit = 65536"
                CommandConnection.ExecuteNonQuery()
                For Each Couche As Byte In CouchesPossiblesServeur
                    Ret = CreerTable(Couche.ToString("00"))
                    If Not Ret Then
                        'pas la peine d'aller plus loin la base est corrompue
                        Exit For
                    End If
                Next
                'on créé la table des pentes sauf si elle existe déjà
                If Ret Then Ret = CreerTable("Pentes")
                Connection.Close()
            Else
                'on vérifie que le cache est bien à la dernière version
                Connection.Open()
                Ret = VerifierVersionCache()
                Connection.Close()
            End If
            CommandConnection.Dispose()
            _Connection.Dispose()
            _Connection = Nothing
            'on supprime le fichier support de la base
            If Not Ret Then File.Delete(NomBaseCacheTuile)
            Return Ret
        End Function
        ''' <summary> crée une table pour un niveau de Zoom donné, sauf pour les pentes ou il y a une couche pour 3 echelles d'affichage </summary>
        ''' <param name="Couche"> Niveau de zoom </param>
        Private Function CreerTable(Couche As String) As Boolean
            Dim Ret As Boolean = False
            Try
                CommandConnection.CommandText = $"CREATE TABLE IF NOT EXISTS [Tuiles_{Couche}] ([COL] Integer Not NULL, [ROW] Integer Not NULL, [IMAGE] BLOB Not NULL);" &
                                                $"CREATE UNIQUE INDEX IF NOT EXISTS [ColRow_{Couche}] On [Tuiles_{Couche}] ([COL] ASC, [ROW] ASC );" &
                                                $"CREATE UNIQUE INDEX IF NOT EXISTS [RowCol_{Couche}] On [Tuiles_{Couche}] ([ROW] ASC, [COL] ASC );"
                CommandConnection.ExecuteNonQuery()
                Ret = True
            Catch Ex As Exception
                AfficherErreur(Ex, "C6M7")
            End Try
            Return Ret
        End Function

        ''' <summary> supprime du cache la table correspondnant à la couche </summary>
        ''' <param name="Couche"> Couche à supprimer </param>
        Private Function SupprimerTable(Couche As String) As Boolean
            Dim Ret As Boolean = False
            Try
                CommandConnection.CommandText = $"DROP TABLE IF EXISTS [Tuiles_{Couche}]"
                CommandConnection.ExecuteNonQuery()
                Ret = True
            Catch Ex As Exception
                AfficherErreur(Ex, "T5E6")
            End Try
            Return Ret
        End Function
        Private Function VerifierVersionCache() As Boolean
            'la table 'Tuiles_Pentes' n'existe pas dans les versions antérieures de FCGP
            CommandConnection.CommandText = "SELECT CASE WHEN [name] = 'Tuiles_Pentes' THEN -1 ELSE 0 END AS [OK] FROM [sqlite_master] WHERE type='table' AND name ='Tuiles_Pentes';"
            'on a rien à faire si elle existe, c'est la bonne version du cache ou le site n'affiche
            If CBool(CommandConnection.ExecuteScalar()) Then Return True
            Dim ListeTables As New List(Of String)(12), Ret As Boolean
            CommandConnection.CommandText = "SELECT [name] FROM [sqlite_master] WHERE type='table';"
            Using Reponse = CommandConnection.ExecuteReader
                Do While Reponse.Read()
                    ListeTables.Add(CStr(Reponse.Item("name")))
                Loop
            End Using
            Ret = RenommerChampsBlob(ListeTables)
            'on crée la table des pentes dans les anciens caches
            If Ret Then Ret = CreerTable("Pentes")
            'Si le cache est ancien on crée la table Tuiles_25 et on supprime la table Tuiles_23
            If Cle = "SM" Then
                Ret = CreerTable("25")
                If Not Ret Then Return Ret
                Ret = SupprimerTable("23")
            End If
            Return Ret
        End Function
        Private Function RenommerChampsBlob(ListeTables As List(Of String)) As Boolean
            Dim Ret As Boolean
            Try
                For Each Table In ListeTables
                    CommandConnection.CommandText = $"ALTER TABLE [{Table}] RENAME COLUMN [JPEG] TO [IMAGE];"
                    CommandConnection.ExecuteNonQuery()
                Next
                Ret = True
            Catch Ex As Exception
                AfficherErreur(Ex, "N0J9")
                Ret = False
            End Try
            Return Ret
        End Function
        ''' <summary> permet la libération des ressources http </summary>
        Friend Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            'nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Class