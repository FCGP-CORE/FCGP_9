using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;

namespace FCGP
{
    internal partial class ServeurCarto
    {
        /// <summary> types possibles pour un serveur WMTS </summary>
        private enum TypesServeur : int
        {
            KVP = 0,
            REST = 1,
            OSM = 2
        }
        internal enum StatutTuile : int
        {
            /// <summary> valeur à la création d'une tuile visue </summary>
            Inconnu = -1,
            /// <summary> réserver en interne. Indique que la tuile n'est pas dans le cache et qu'il faut la télécharger </summary>
            DemandeTeleChargement = 0,
            /// <summary> indique qu'il y a eu une erreur de la part du serveur et gérée par HttpClient </summary>
            ErreurServeur = 1,
            /// <summary> indique que le tampon de la tuile a été rempli à partir du cache </summary>
            Cache = 2,
            /// <summary> indique que le tampon de la tuile a été rempli à partir du serveur </summary>
            Serveur = 3,
            /// <summary> indique que la tuile n'existe pas sur le serveur ou qu'elle ne contient pas assez d'informations pour être dessinner </summary>
            Inexistant = 4,
            /// <summary> indique que la tuile n'est plus concernée par le tampon encours </summary>
            PlusConcerne = 5,
            /// <summary> indique qu'il y a eu une erreur interne à FCGP </summary>
            ErreurFCGP = 6
        }
        /// <summary> Gestion de la base de données des tuiles d'un serveur carto </summary>
        internal class CacheTuiles : IDisposable
        {
            #region Champs et Proprietes
            /// <summary> renvoie vrai si tout est ok lors de l'initialisation du cache </summary>
            internal bool IsOk { get; private set; }
            /// <summary> permet la connection avec le cache </summary>
            private SQLiteConnection _Connection;
            /// <summary> permet la connection avec le cache </summary>
            private SQLiteConnection Connection
            {
                get
                {
                    if (_Connection is null)
                    {
                        _Connection = new SQLiteConnection($"Data Source={NomBaseCacheTuile};");
                        CommandConnection = _Connection.CreateCommand();
                    }
                    return _Connection;
                }
            }
            /// <summary> pour passer de commandes SQL à la base de données </summary>
            private SQLiteCommand CommandConnection;
            /// <summary> nom de la base de données SQLite. C'est toujours le même</summary>
            private string NomBaseCacheTuile;
            /// <summary> serveur carto associé au cache tuiles </summary>
            private readonly ServeurCarto Serveur;
            /// <summary> clé du site carto </summary>
            private string Cle { get; set; }
            /// <summary> indique si la carte a été détruite</summary>
            private bool disposed;
            #endregion
            #region functions et requêtes asynchrone
            /// <summary> lit à partir du cache le tableau de bytes correspondant au champ IMAGE d'une tuile d'affichage de manière asynchrone </summary>
            /// <param name="Col"> Index de colonne </param>
            /// <param name="Row"> Index de rangée </param>
            /// <param name="Couche"> N° de couche de la tuile </param>
            internal async Task<byte[]> LireOctetsTuileAsync(int Col, int Row, string Couche)
            {
                CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};";
                return (byte[])await CommandConnection.ExecuteScalarAsync();
            }
            /// <summary> lit à partir du cache le tableau de bytes correspondant au champ IMAGE d'une tuile de pentes de manière asynchrone </summary>
            /// <remarks> le paramètre Info sert pour aussi pour le retour </remarks>
            /// <param name="Col"> Index de colonne </param>
            /// <param name="Row"> Index de rangée </param>
            internal async Task<byte[]> LireOctetsTuileAsync(int Col, int Row)
            {
                CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_Pentes] Where [COL]={Col} And [ROW]={Row};";
                return (byte[])await CommandConnection.ExecuteScalarAsync();
            }
            /// <summary> lit à partir du cache le tableau de bytes correspondant au champ JPEG de manière asynchrone </summary>
            /// <param name="Info"> Tuile affichage contenant les infos </param>
            /// <remarks> le paramètre Info sert pour le retour </remarks>
            internal async Task LireOctetsTuileAsync(DemandeTuiles.TuileAffichage Info)
            {
                CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_{Info.CoucheTxt}] Where [COL]={Info.Col} And [ROW]={Info.Row};";
                Info.BytesTuile = (byte[])await CommandConnection.ExecuteScalarAsync();
                if (Info.BytesTuile is null) // indique que la tuile n'est pas présente dans le cache et qu'il faut éventuellement la télécharger pour l'obtenir
                {
                    Info.StatutTuile = StatutTuile.DemandeTeleChargement;
                }
                else if (Info.BytesTuile.Length > 1) // indique que la tuile est présente dans le cache et que le tampon est rempli
                {
                    Info.StatutTuile = StatutTuile.Cache;
                }
                else // indique que la tuile est dans le cache mais que la tuile n'existe pas sur le serveur
                {
                    Info.StatutTuile = StatutTuile.Inexistant;
                }
            }
            /// <summary> écrit une tuile dans le cache de manière asynchrone. </summary>
            /// <remarks> on test pour savoir si la tuile existe afin d'éviter une erreur potentielle car elle a pu être enregistrée 
            /// à partir d'une autre demande de tuile que celle qui appelle la méthode </remarks>
            internal async Task EcrireOctetsTuileAsync(DemandeTuiles.TuileAffichage Info)
            {
                if (!await ContenirLaTuileAsync(Info))
                {
                    // insertion d'un seul enregistrement
                    using (var EcritureBase = Connection.BeginTransaction())
                    {
                        CommandConnection.CommandText = $"Insert Into [Tuiles_{Info.CoucheTxt}] ([COL], [ROW], [IMAGE]) Values ({Info.Col}, {Info.Row}, @Image);";
                        CommandConnection.Parameters.Add(new SQLiteParameter("@Image", DbType.Binary) { Value = Info.BytesTuile });
                        await CommandConnection.ExecuteNonQueryAsync();
                        EcritureBase.Commit();
                    }
                }
                else if (Info.BytesTuile.Length > 1)
                {
                    Info.StatutTuile = StatutTuile.Cache;
                }
                else
                {
                    Info.StatutTuile = StatutTuile.Inexistant;
                }
            }
            /// <summary> renvoie de manière asynchrone true si la tuile existe </summary>
            internal async Task<bool> ContenirLaTuileAsync(DemandeTuiles.TuileAffichage Info)
            {
                CommandConnection.CommandText = $"Select [Rowid] From [Tuiles_{Info.CoucheTxt}] Where [Col]={Info.Col} And [Row]={Info.Row};";
                return Convert.ToBoolean(await CommandConnection.ExecuteScalarAsync());
            }
            #endregion
            #region requêtes synchrones
            /// <summary> Lit de façon synchrone une tuile de la couche pentes dans le cache </summary>
            /// <param name="Col"></param>
            /// <param name="Row"></param>
            internal byte[] LireOctetsTuile(int Col, int Row, string Couche)
            {
                CommandConnection.CommandText = $"Select [IMAGE] From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};";
                return (byte[])CommandConnection.ExecuteScalar();
            }
            /// <summary> écrit une tuile dans le cache de manière synchrone </summary>
            internal void EcrireOctetsTuile(int Col, int Row, string Couche, byte[] BytesTuile)
            {
                // insertion d'un seul enregistrement
                using (var Transaction = Connection.BeginTransaction())
                {
                    CommandConnection.CommandText = $"Insert Into [Tuiles_{Couche}] ([COL], [ROW], [IMAGE]) Values ({Col}, {Row}, @Image);";
                    CommandConnection.Parameters.Add(new SQLiteParameter("@Image", DbType.Binary) { Value = BytesTuile });
                    CommandConnection.ExecuteNonQuery();
                    Transaction.Commit();
                }
            }
            /// <summary> Efface dans le cache la tuile passée en paramètre. Pas de controle sur l'existence de celle-ci </summary>
            internal void EffacerTuile(int Col, int Row, string Couche)
            {
                using (var EcritureBase = Connection.BeginTransaction())
                {
                    CommandConnection.CommandText = $"Delete From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};";
                    CommandConnection.ExecuteNonQuery();
                    EcritureBase.Commit();
                }
            }
            /// <summary> renvoie de manière synchrone true si la tuile existe </summary>
            internal bool ContenirLaTuile(int Col, int Row, string Couche)
            {
                CommandConnection.CommandText = $"Select [Rowid] From [Tuiles_{Couche}] Where [COL]={Col} And [ROW]={Row};";
                return Convert.ToBoolean(CommandConnection.ExecuteScalar());
            }
            /// <summary> renvoie le nb de tuiles contenues dans le cahce et qui appartiennent à une region </summary>
            /// <param name="ColMin"> index Colonne mini </param>
            /// <param name="ColMax"> index Colonne maxi </param>
            /// <param name="RowMin"> index Rangéée mini </param>
            /// <param name="RowMax"> index Rangéée maxi </param>
            /// <param name="Couche"> valeur du zoom des tuiles </param>
            internal int TrouverNbTuilesRegion(int ColMin, int ColMax, int RowMin, int RowMax, int Couche)
            {
                CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche:00}] Where [COL] Between {ColMin} And {ColMax} And [ROW] Between {RowMin} And {RowMax};";
                return Convert.ToInt32(CommandConnection.ExecuteScalar());
            }
            /// <summary> renvoie sous forme de liste de tuple les tuiles contenues dans le cahce et qui appartiennent à une region </summary>
            /// <param name="ColMin"> index Colonne mini </param>
            /// <param name="ColMax"> index Colonne maxi </param>
            /// <param name="RowMin"> index Rangéée mini </param>
            /// <param name="RowMax"> index Rangéée maxi </param>
            /// <param name="Couche"> valeur du zoom des tuiles </param>
            internal List<(int Col, int Row, byte[] OctetsTuile)> RenvoyerTuilesRegion(int ColMin, int ColMax, int RowMin, int RowMax, string Couche)
            {
                CommandConnection.CommandText = $"Select [COL], [ROW], [IMAGE] From [Tuiles_{Couche}] Where [COL] Between {ColMin} " + $"And {ColMax} And [ROW] Between {RowMin} And {RowMax};";
                var RenvoyerTuilesRegionRet = new List<(int Col, int Row, byte[] OctetsTuile)>();
                using (var Reponse = CommandConnection.ExecuteReader())
                {
                    while (Reponse.Read())
                        RenvoyerTuilesRegionRet.Add((Convert.ToInt32(Reponse[0]), Convert.ToInt32(Reponse[1]), (byte[])Reponse[2]));
                }

                return RenvoyerTuilesRegionRet;
            }
            /// <summary> renvoie le nb de tuiles contenues dans le cache concernant une rangée compris entre 2 bornes délimitant les colonnes </summary>
            /// <param name="ColMin"> index Colonne mini </param>
            /// <param name="ColMax"> index Colonne maxi </param>
            /// <param name="Row"> index de la Rangée concernée </param>
            /// <param name="Couche"> valeur du zoom des tuiles </param>
            internal int TrouverNbTuilesRow(int ColMin, int ColMax, int Row, string Couche)
            {
                CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche}] Where [Col] Between {ColMin} And {ColMax} And [Row] = {Row};";
                return Convert.ToInt32(CommandConnection.ExecuteScalar());
            }
            /// <summary> renvoie le nb de tuiles contenues dans le cache concernant une colonne compris entre 2 bornes délimant les rangées </summary>
            /// <param name="Col"> index de la Colonne concernée </param>
            /// <param name="RowMin"> index Rangéée mini </param>
            /// <param name="RowMax"> index Rangéée maxi </param>
            /// <param name="Couche"> valeur du zoom des tuiles </param>
            /// <returns></returns>
            internal int TrouverNbTuilesCol(int Col, int RowMin, int RowMax, string Couche)
            {
                CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche}] Where [Col] = {Col} And [Row] Between {RowMin} And {RowMax};";
                return Convert.ToInt32(CommandConnection.ExecuteScalar());
            }
            /// <summary> renvoie le nb de tuiles contenues dans le cache concernant un zoom donné </summary>
            /// <param name="Couche"> valeur du zoom des tuiles </param>
            internal int TrouverNbEnregTable(string Couche)
            {
                CommandConnection.CommandText = $"Select Count(*) From [Tuiles_{Couche}];";
                return Convert.ToInt32(CommandConnection.ExecuteScalar());
            }
            /// <summary>  renvoie sous forme de tuple les stats concernant les tuiles contenues dans le cache pour un zoom donné </summary>
            /// <param name="Couche"> valeur du zoom des tuiles </param>
            internal (int NbTuiles, int MinCol, int MaxCol, int MinRow, int MaxRow) TrouverStatsTable(string Couche)
            {
                CommandConnection.CommandText = $"Select Count(*), Min([Col]), Max([Col]), Min([Row]), Max([Row]) From [Tuiles_{Couche}];";
                using (var Reponse = CommandConnection.ExecuteReader())
                {
                    Reponse.Read();
                    return (Convert.ToInt32(Reponse[0]), Convert.ToInt32(Reponse[1]), Convert.ToInt32(Reponse[2]),
                            Convert.ToInt32(Reponse[3]), Convert.ToInt32(Reponse[4]));
                }
            }
            /// <summary> efface les tuiles du cache contenues dans la région </summary>
            /// <param name="Region"> region contenant les tuile à effacer </param>
            internal void EffacerSurfaceTuile(SurfaceTuiles Region)
            {
                using (var EcritureBase = Connection.BeginTransaction())
                {
                    CommandConnection.CommandText = $"Delete From [Tuiles_{Region.Couche:00}] Where [Col] Between {Region.NumColDeb} " + $"And {Region.NumColFin} And [Row] Between {Region.NumRangDeb} And {Region.NumRangFin};";
                    CommandConnection.ExecuteNonQueryAsync();
                    EcritureBase.Commit();
                }
            }
            #endregion
            #region Création
            /// <summary> initialise un nouveau cache soit en le créant soit en l'ouvrant si il existe </summary>
            /// <param name="ServeurCarto"> serveur carto associé au cache </param>
            internal CacheTuiles(ServeurCarto ServeurCarto)
            {
                Serveur = ServeurCarto;
                Cle = Serveur.IndiceFondPlan == 0 ? Serveur.Cle : $"{Serveur.Cle}_{Serveur.IndiceFondPlan}";
                NomBaseCacheTuile = Serveur.CheminCache + "CacheTuiles" + Cle + ".db";
                IsOk = Initialiser();
                if (IsOk)
                {
                    Connection.Open();
                }
            }
            /// <summary> Change le répertoire du cache tuile en créant un nouveau cache si besoin </summary>
            /// <param name="NouveauRepertoire"> Repertoire où doit se trouver le cache </param>
            internal bool ChangerRepertoireCaches(string NouveauRepertoire)
            {
                bool Ret = false;
                try
                {
                    string AnciennBase = NomBaseCacheTuile;
                    CommandConnection.Dispose();
                    // Fermer la connection
                    _Connection.Close();
                    _Connection.Dispose();
                    // on force la réinitialisation
                    _Connection = null;
                    Serveur.CheminCache = NouveauRepertoire;
                    NomBaseCacheTuile = NouveauRepertoire + @"\CacheTuiles" + Cle + ".db";
                    Ret = Initialiser();
                    if (!Ret)
                    {
                        NomBaseCacheTuile = AnciennBase;
                        MessageInformation = "Le nouveau cache tuiles n'a pas pu être initialisé" + CrLf +
                                             "L'ancien cache tuiles reste actif.";
                        AfficherInformation();
                    }
                    Connection.Open();
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "T6Q5");
                }
                return Ret;
            }
            /// <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                    return;
                if (disposing)
                {
                    if (_Connection is not null)
                    {
                        CommandConnection.Dispose();
                        _Connection.Close();
                        _Connection.Dispose();
                    }
                }
                disposed = true;
            }
            /// <summary> crée la base si elle n'existe pas et gère les erreurs lors de la création </summary>
            private bool Initialiser()
            {
                bool Ret = true;
                if (!File.Exists(NomBaseCacheTuile))
                {
                    byte[] CouchesPossiblesServeur = null;
                    //Si le site est DomTom, il faut prévoir les tables des couches pour celui qui en a le plus c'est à dire la Guyane
                    if (Cle == "DT")
                    {
                        CouchesPossiblesServeur = NiveauDetailCartographique.CouchesSite(Serveur.SiteCarto, DomsToms.guyane);
                    }
                    else
                    {
                        CouchesPossiblesServeur = Serveur.CouchesLayer(0);
                    }
                    Connection.Open();
                    CommandConnection.CommandText = "PRAGMA main.page_size = 8192; VACUUM; PRAGMA journal_mode = 'WAL';PRAGMA journal_size_limit = 65536";
                    CommandConnection.ExecuteNonQuery();
                    foreach (byte Couche in CouchesPossiblesServeur)
                    {
                        Ret = CreerTable(Couche.ToString("00"));
                        if (!Ret)
                        {
                            // pas la peine d'aller plus loin la base est corrompue
                            break;
                        }
                    }
                    // on créé la table des pentes sauf si elle existe déjà
                    if (Ret)
                        Ret = CreerTable("Pentes");
                    Connection.Close();
                }
                else
                {
                    // on vérifie que le cache est bien à la dernière version
                    Connection.Open();
                    Ret = VerifierVersionCache();
                    Connection.Close();
                }
                CommandConnection.Dispose();
                _Connection.Dispose();
                _Connection = null;
                // on supprime le fichier support de la base
                if (!Ret)
                    File.Delete(NomBaseCacheTuile);
                return Ret;
            }
            /// <summary> crée une table pour un niveau de Zoom donné, sauf pour les pentes ou il y a une couche pour 3 echelles d'affichage </summary>
            /// <param name="Couche"> Niveau de zoom </param>
            private bool CreerTable(string Couche)
            {
                bool Ret = false;
                try
                {
                    CommandConnection.CommandText = $"CREATE TABLE IF NOT EXISTS [Tuiles_{Couche}] ([COL] Integer Not NULL, [ROW] Integer Not NULL, [IMAGE] BLOB Not NULL);" +
                                                    $"CREATE UNIQUE INDEX IF NOT EXISTS [ColRow_{Couche}] On [Tuiles_{Couche}] ([COL] ASC, [ROW] ASC );" +
                                                    $"CREATE UNIQUE INDEX IF NOT EXISTS [RowCol_{Couche}] On [Tuiles_{Couche}] ([ROW] ASC, [COL] ASC );";
                    CommandConnection.ExecuteNonQuery();
                    Ret = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "C6M7");
                }
                return Ret;
            }

            /// <summary> supprime du cache la table correspondnant à la couche </summary>
            /// <param name="Couche"> Couche à supprimer </param>
            private bool SupprimerTable(string Couche)
            {
                bool Ret = false;
                try
                {
                    CommandConnection.CommandText = $"DROP TABLE IF EXISTS [Tuiles_{Couche}]";
                    CommandConnection.ExecuteNonQuery();
                    Ret = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "T5E6");
                }
                return Ret;
            }
            private bool VerifierVersionCache()
            {
                // la table 'Tuiles_Pentes' n'existe pas dans les versions antérieures de FCGP
                CommandConnection.CommandText = "SELECT CASE WHEN [name] = 'Tuiles_Pentes' THEN -1 ELSE 0 END AS [OK] FROM [sqlite_master] WHERE type='table' AND name ='Tuiles_Pentes';";
                // on a rien à faire si elle existe, c'est la bonne version du cache ou le site n'affiche
                if (Convert.ToBoolean(CommandConnection.ExecuteScalar()))
                    return true;
                var ListeTables = new List<string>(12);
                bool Ret;
                CommandConnection.CommandText = "SELECT [name] FROM [sqlite_master] WHERE type='table';";
                using (var Reponse = CommandConnection.ExecuteReader())
                {
                    while (Reponse.Read())
                        ListeTables.Add(Convert.ToString(Reponse["name"]));
                }
                Ret = RenommerChampsBlob(ListeTables);
                // on crée la table des pentes dans les anciens caches
                if (Ret)
                    Ret = CreerTable("Pentes");
                // Si le cache est ancien on crée la table Tuiles_25 et on supprime la table Tuiles_23
                if (Cle == "SM")
                {
                    Ret = CreerTable("25");
                    if (!Ret)
                        return Ret;
                    Ret = SupprimerTable("23");
                }
                return Ret;
            }
            private bool RenommerChampsBlob(List<string> ListeTables)
            {
                bool Ret;
                try
                {
                    foreach (var Table in ListeTables)
                    {
                        CommandConnection.CommandText = $"ALTER TABLE [{Table}] RENAME COLUMN [JPEG] TO [IMAGE];";
                        CommandConnection.ExecuteNonQuery();
                    }
                    Ret = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "N0J9");
                    Ret = false;
                }
                return Ret;
            }
            /// <summary> permet la libération des ressources http </summary>
            public void Dispose()
            {
                Dispose(true);
                // nécessaire pour éviter que le GC ne redemande Finalize()
                GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}