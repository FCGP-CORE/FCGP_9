using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> gestions des Altitudes : Télechargement des fichiers DEM, Vérification des identifiants User, altitude d'un point </summary>
    internal static class Altitudes
    {
        #region Variables internes
        private const string URS = "https://urs.earthdata.nasa.gov";
        private const string URI_Debut = "https://e4ftl01.cr.usgs.gov/MEASURES/SRTMGL";
        private const string URI_Fin = ".003/2000.02.11/";
        private const string ExtFich = ".hgt";
        private const string TitleInformation = "Information Altidudes";
        private const long TailleDem1 = 3601 * 2 * 3601;//25934402
        private const long TailleDem3 = 1201 * 2 * 1201;//2884802
        /// <summary>nb d'actions de téléchargement de fichiers d'altitudes possible en même temps</summary>
        private const int Nb_Taches = 5;
        /// <summary> uri pour tester les identifiants de l'utilisateur. On est sur que le fichier existe </summary>
        private const string URI_User = "https://e4ftl01.cr.usgs.gov/MEASURES/SRTMGL3.003/2000.02.11/N46E004.SRTMGL3.hgt.zip";
        /// <summary> identifiants de l'utilisateur à tester </summary>
        private static string ID_User, MP_User;
        /// <summary>adresse pour le téléchargement des fichiers d'altitudes</summary>
        private static string UriFichiersAltitudes
        {
            get
            {
                return URI_Debut + AltitudesSettings.TYPE_DEM.ToString() + URI_Fin;
            }
        }
        /// <summary> extensions des fichiers d'altitude sur le serveur car cela dépend du type du fichier </summary>
        private static string ExtFichZip;
        /// <summary>emplacement des fichiers d'altitudes</summary>
        private static string CheminFichiersAltitudes;
        // ''' <summary>tableau de tâches de téléchargement de fichiers d'altitudes possible en même temps</summary>
        private readonly static Dictionary<int, Task> ListeTaches = new(Nb_Taches);
        /// <summary> fichiers d'altitudes en cours de téléchargement </summary>
        private readonly static Dictionary<int, string> ListeFichiers = new(Nb_Taches);
        /// <summary> clef reliant tache et nom de fichiers d'altitudes </summary>
        private static int Clef;
        /// <summary>liste des fichiers d'altitude présents dans le répertoire encours. Ils sont soient présents en local soit en statut téléchargement encours </summary>
        private static SortedDictionary<string, StatutFichierAltitude> ListeFichiersAltitudes;
        /// <summary> fournisseur de requête pour les fichiers d'altitude </summary>
        private static HttpClient HttpClientAltitude;
        /// <summary> le serveur de la NASA est sécurisé, il faut envoyer les identifiants de connexions </summary>
        private static HttpClientHandler MessageHandler;
        /// <summary> flag indiquant que la connexion avec le serveur est établie </summary>
        internal static bool IsConnexion { get; private set; }
        /// <summary> flag indiquant que l'initialisation des fichiers DEM est faite </summary>
        internal static bool IsOkFichiersAltitudes { get; private set; }
        /// <summary> statut d'un fichier dont on a demandé le téléchargement </summary>
        private enum StatutFichierAltitude
        {
            PresentDem3 = 1, // soit à l'initialisation de la liste soit au retour du téléchargement si cela c'est bien passé en fonction de la taille
            PresentDem1 = 2, // soit à l'initialisation de la liste soit au retour du téléchargement si cela c'est bien passé en fonction de la taille
            FichierInexistant = 3, // soit à l'initialisation de la liste soit au retour du serveur qui indique quele fichier n'existe pas
            EncoursChargementServeur = 4 // le téléchargement est considéré en cours tant que la tache de téléchargement n'est pas finie
        }
        #endregion
        #region Procédures externes
        /// <summary> mise à jour de la connection avec le serveur</summary>
        internal static void InitialiserConnexionAltitudes()
        {
            IsConnexion = false;
            // on supprime l'ancienne connexion si il y en a une active
            if (HttpClientAltitude is not null)
            {
                HttpClientAltitude.Dispose();
                MessageHandler.Dispose();
            }
            // pour avoir la connexion avec le serveur il faut que les 2 conditions
            if (AltitudesSettings.IS_ALTITUDE && AltitudesSettings.IS_VALIDE)
            {
                // on met à jour les identifiants de connexion
                var NTC = new NetworkCredential(AltitudesSettings.ID, AltitudesSettings.MP);
                ICredentials myCredentials = new CredentialCache() { { new Uri(URS), "Basic", NTC } };
                MessageHandler = new HttpClientHandler()
                {
                    Credentials = myCredentials,
                    PreAuthenticate = false,
                    AllowAutoRedirect = true
                };
                // on cré la nouvelle connexion
                HttpClientAltitude = new HttpClient(MessageHandler);
                IsConnexion = true;
            }
        }
        /// <summary> initialise les différentes variables nécessaires au renvoi d'une altitude et regarde si il existe déjà des fichiers d'altitudes </summary>
        /// <param name="FlagIdentifiants"> Indique que l'on veut aussi initialiser la connexion avec le serveur </param>
        internal static void InitialiserListeFichiersAltitudes()
        {
            IsOkFichiersAltitudes = false;
            if (ListeFichiersAltitudes is null)
            {
                ListeFichiersAltitudes = new SortedDictionary<string, StatutFichierAltitude>();
            }
            else
            {
                // nettoyage de la liste des fichiers
                ListeFichiersAltitudes.Clear();
            }
            ExtFichZip = ".SRTMGL" + AltitudesSettings.TYPE_DEM.ToString() + ".hgt.zip";
            CheminFichiersAltitudes = AltitudesSettings.CHEMIN_DEM;
            foreach (string fichier in Directory.GetFiles(CheminFichiersAltitudes, "*.hgt", SearchOption.TopDirectoryOnly))
            {
                // il ne peut y avoir que 3 tailles différentes pour les fichiers HGT : 0 = inexistant, 2884802 = DEM 3, 25934402 = DEM 1
                var Fi = new FileInfo(fichier);
                if (Fi.Length == TailleDem1)
                {
                    ListeFichiersAltitudes.Add(Path.GetFileNameWithoutExtension(fichier), StatutFichierAltitude.PresentDem1);
                }
                else if (Fi.Length == TailleDem3)
                {
                    ListeFichiersAltitudes.Add(Path.GetFileNameWithoutExtension(fichier), StatutFichierAltitude.PresentDem3);
                }
                else
                {
                    ListeFichiersAltitudes.Add(Path.GetFileNameWithoutExtension(fichier), StatutFichierAltitude.FichierInexistant);
                }
            }
            IsOkFichiersAltitudes = true;
        }
        /// <summary> Arrête le téléchargement encours des fichiers d'altitudes et libère les ressources </summary>
        internal static void CloturerAltitudes()
        {
            // nettoyage des taches de téléchargement encore encours
            if (ListeTaches.Count > 0)
            {
                foreach (KeyValuePair<int, Task> KVP in ListeTaches)
                {
                    try
                    {
                        KVP.Value.Dispose();
                        ListeTaches.Remove(KVP.Key);
                        ListeFichiers.Remove(KVP.Key);
                    }
                    catch (Exception Ex)
                    {
                        AfficherErreur(Ex, "4CFA");
                    }
                }
            }
            // enregistrements des différentes variables associées aux altitudes
            if (IsSettingsOk)
                AltitudesSettings.Ecrire();
            // fermeture de la connexion avec le serveur
            if (HttpClientAltitude is not null)
            {
                HttpClientAltitude.Dispose();
                MessageHandler.Dispose();
            }
        }
        /// <summary> Affiche des messages d'information liées au modules des Altitudes </summary>
        private static void AfficherInformationAltitudes()
        {
            string Titre = TitreInformation;
            TitreInformation = TitleInformation;
            AfficherInformation();
            TitreInformation = Titre;
        }
        /// <summary> cette fonction renvoie une altitude si le fichier correspondant à la longitude et à la latitude est présent dans le répertoire adéquat sinon
        /// elle demande le téléchargement de celui-ci mais il faut 3 conditions </summary>
        /// <param name="PT"> point représentant la longitude (X) et la latitude (Y)</param>
        /// <returns>l'altitude du point en mètres</returns>
        /// <remarks> -9999 si le fichier n'existe pas en local ou l'altitude dans le cas contraire </remarks>
        internal static short TrouverAltitudeCoordonnees(PointD PT)
        {
            short Altitude = -9999;
            if (!AltitudesSettings.IS_ALTITUDE || !IsConnexion || !IsConnected)
                return Altitude;
            double Latitude = Math.Floor(PT.Lat);
            double Longitude = Math.Floor(PT.Lon);
            string FichierEncours = $"{(Latitude < 0d ? 'S' : 'N')}{Math.Abs(Latitude):00}" + $"{(Longitude < 0d ? 'W' : 'E')}{Math.Abs(Longitude):000}";
            // si le fichier est déja dans la liste c'est que soit il est présent en local,  soit le chargement est en cours, soit le est fichierinexistant ou en erreur
            if (ListeFichiersAltitudes.ContainsKey(FichierEncours))
            {
                if (ListeFichiersAltitudes[FichierEncours] < StatutFichierAltitude.FichierInexistant)
                {
                    double NbArc;
                    int NbOctetsLigneFichierHGT;
                    NbArc = ListeFichiersAltitudes[FichierEncours] == StatutFichierAltitude.PresentDem1 ? 3600 : 1200;
                    NbOctetsLigneFichierHGT = (int)Math.Round(NbArc * 2d + 2d);
                    // on ouvre le fichier des altitudes en lecture
                    using (var AltitudesStream = new FileStream(CheminFichiersAltitudes + @"\" + FichierEncours + ".hgt", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // on peut renvoyer une altitude (calcul)
                        // on calcule la position du premier octect du point connu entourant l'altitude du point cherché
                        double Y = PT.Lat - Latitude;
                        Y = (1d - Y) * NbArc;
                        double X = PT.Lon - Longitude;
                        X *= NbArc;
                        int PositionY = (int)Math.Floor(Y);
                        int PositionX = (int)Math.Floor(X);
                        // pas d'interpolation juste par rapport au plus proche
                        int PositionOctect = PositionY * NbOctetsLigneFichierHGT + PositionX * 2;
                        AltitudesStream.Position = PositionOctect;
                        double AltitudesCalculees0;
                        // on lit l'atitude à la position du pointeur de fichier
                        short AltitudesPosition0 = (short)((AltitudesStream.ReadByte() << 8) + AltitudesStream.ReadByte());
                        if (X - PositionX < 0.0001d)
                        {
                            // dans ce cas il n'y a pas d'interpolation sur les longitudes : exactement egale à un multiple d'arc
                            AltitudesCalculees0 = AltitudesPosition0;
                        }
                        else
                        {
                            short AltitudesPosition1 = (short)((AltitudesStream.ReadByte() << 8) + AltitudesStream.ReadByte());
                            // interpolation de l'altitude sur la longitude du point cherché
                            AltitudesCalculees0 = AltitudesPosition0 + (AltitudesPosition1 - AltitudesPosition0) * (X - PositionX);
                        }
                        if (Y - PositionY < 0.0001d)
                        {
                            // dans ce cas il n'y a pas d'interpolation sur les latitudes : exactement egale à un multiple d'arc
                            Altitude = (short)Math.Round(AltitudesCalculees0);
                        }
                        else
                        {
                            double AltitudesCalculees1;
                            // on calcule la position du 3ème point connu entourant l'altitude du point cherché
                            AltitudesStream.Position = PositionOctect + NbOctetsLigneFichierHGT;
                            // on lit l'altitude du 3ème point et du 4ème point car ils se suivent
                            AltitudesPosition0 = (short)((AltitudesStream.ReadByte() << 8) + AltitudesStream.ReadByte());
                            if (X - PositionX < 0.01d)
                            {
                                // dans ce cas il n'y a pas d'interpolation sur les longitudes 
                                AltitudesCalculees1 = AltitudesPosition0;
                            }
                            else
                            {
                                short AltitudesPosition1 = (short)((AltitudesStream.ReadByte() << 8) + AltitudesStream.ReadByte());
                                // interpolation de l'altitude sur la longitude du point cherché
                                AltitudesCalculees1 = AltitudesPosition0 + (AltitudesPosition1 - AltitudesPosition0) * (X - PositionX);
                            }
                            // interpolation de l'altitude sur la latitude du point cherché
                            Altitude = (short)Math.Round(AltitudesCalculees1 + (AltitudesCalculees0 - AltitudesCalculees1) * (Y - PositionY));
                        }
                    }
                }
                else
                {
                    // le fichier est présent mais n'a pas le bon statut soit il est inexistant sur le serveur soit en cours de téléchargement
                    // donc on renvoie 0
                }
            }
            else if (IsConnected) // le fichier n'est pas présent en local car jamais demandé ou en erreur, on demande le téléchargement du fichier en arrière plan
            {
                try
                {
                    if (ListeTaches.Count < Nb_Taches) // on prend la 1ere dispo
                    {
                        string FichierZipEncours = FichierEncours + ExtFichZip;
                        // on ajoute le fichier à télécharger dans la liste avec le statut chargementencours. c'est TelechargementFichierHGTAsync qui fixe le statut final
                        ListeFichiersAltitudes.Add(FichierEncours, StatutFichierAltitude.EncoursChargementServeur);
                        ListeFichiers.Add(Clef, FichierEncours);
                        ListeTaches.Add(Clef, TelechargerFichierHGTAsync(UriFichiersAltitudes + FichierZipEncours, CheminEnregistrementProvisoire + @"\" + FichierZipEncours, Clef));
                        Clef += 1;
                        MessageInformation = "Demande de téléchargement du fichier " + FichierEncours + ExtFich;
                    }
                }
                catch (Exception ex)
                {
                    AfficherErreur(ex, "6989");
                    MessageInformation = "Demande de téléchargement du fichier " + FichierEncours + " annulée suite erreur sur le serveur";
                }
                if (AltitudesSettings.IS_MSG)
                    AfficherInformationAltitudes();
            }
            return Altitude;
        }
        /// <summary> affiche un formulaire d'attente pendant la vérification des identifiants sur le site de la NASA </summary>
        /// <param name="ID"> identifiant </param>
        /// <param name="MP"> mot de passe </param>
        internal static DialogResult VerifierIDAltitudes(string ID, string MP)
        {
            ID_User = ID;
            MP_User = MP;
            string Titre = TitreInformation;
            TitreInformation = TitleInformation;
            MessageInformation = "Vérification des identifiants en cours ....";
            var Ret = LancerTache(VerifierID_User);
            TitreInformation = Titre;
            return Ret;
        }
        /// <summary> télécharge de manière asynchrone un fichier hgt sur le site de données de la NASA </summary>
        /// <param name="URI"> Non complet du fichier HGT sur le serveur </param>
        /// <param name="FichierZipEncours"> Nom complet du fichier dans lequel sera enregistré le fichier hgt en local </param>
        /// <param name="Cpt"> indice du téléchargement dans la liste des téléchargement en cours </param>
        private async static Task TelechargerFichierHGTAsync(string URI, string FichierZipEncours, int Cpt)
        {
            try
            {
                var response = await HttpClientAltitude.GetAsync(URI);
                // à ce niveau on gère si le fichier existant ou non existant et on ignore les erreurs
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // il n'y a pas eu d'erreur de liaision avec le serveur. Les identifiants sont corrects et le fichier est existant on peut poursuivre le traitement
                    // en copiant le fichier reçu du serveur de la NASA en local
                    using (var Source = await response.Content.ReadAsStreamAsync())
                    {
                        using (var DestinationStream = File.Create(FichierZipEncours))
                        {
                            await Source.CopyToAsync(DestinationStream);
                        }
                    }
                    await Task.Run(() => TelechargerFichierAltitudesZipComplet(FichierZipEncours, Cpt));
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // on indique dans la liste que le fichier est inexsistant pour éviter une nouvelle demande de téléchargement
                    ListeFichiersAltitudes[ListeFichiers[Cpt]] = StatutFichierAltitude.FichierInexistant;
                    string FichierEncours = ListeFichiers[Cpt] + ExtFich;
                    MessageInformation = $"Le fichier d'altitude {FichierEncours} est inexistant";
                    // on crée un fichier vide (0 octet) pour que les prochaines sessions sachent qu'il n'existe pas
                    using (FileStream FS = new($@"{CheminFichiersAltitudes}\{FichierEncours}", FileMode.Create))
                    {
                    }
                }
                else
                {
                    // à ce niveau on gère les autres exceptions
                    MessageInformation = response.ReasonPhrase.ToString();
                    ListeFichiersAltitudes.Remove(ListeFichiers[Cpt]);
                }
            }
            catch (Exception ex)
            {
                // à ce niveau on gère les exeptions qui ne sont pas gérées directement dans la classe HTTP
                AfficherErreur(ex, "H4M7");
                ListeFichiersAltitudes.Remove(ListeFichiers[Cpt]);
            }
            ListeFichiers.Remove(Cpt);
            ListeTaches.Remove(Cpt);
            if (AltitudesSettings.IS_MSG)
                AfficherInformation();
        }
        /// <summary> Contient la logique du dézippage une fois que le fichier zippé des altitudes a été téléchargé sur le micro </summary>
        private static void TelechargerFichierAltitudesZipComplet(string FichierZipEncours, int Cpt)
        {
            string FichierEncours = ListeFichiers[Cpt] + ExtFich;
            MessageInformation = $"Le fichier d'altitude {FichierEncours}";
            FichierEncours = $@"{CheminFichiersAltitudes}\{FichierEncours}";
            // on decompresse le fichier qui vient d'être téléchargé mais en vérifiant qu'il n'existe pas sur le disque pour éviter une erreur
            if (File.Exists(FichierEncours))
            {
                File.Delete(FichierEncours);
            }
            ZipFile.ExtractToDirectory(FichierZipEncours, CheminFichiersAltitudes);
            var FichierInfo = new FileInfo(FichierEncours);
            // les fichiers font tous la même taille
            if (FichierInfo.Length == TailleDem1)
            {
                // on indique que le fichier est disponible en enlevant le statut chargement encours 
                ListeFichiersAltitudes[ListeFichiers[Cpt]] = StatutFichierAltitude.PresentDem1;
                MessageInformation += " a été téléchargé";
            }
            else if (FichierInfo.Length == TailleDem3)
            {
                // on indique que le fichier est disponible en enlevant le statut chargement encours 
                ListeFichiersAltitudes[ListeFichiers[Cpt]] = StatutFichierAltitude.PresentDem3;
                MessageInformation += " a été téléchargé";
            }
            else
            {
                // comme le fichier n'est pas bon on le supprime physiquement et dans la liste pour pouvoir le re-télécharger
                FichierInfo.Delete();
                ListeFichiersAltitudes.Remove(ListeFichiers[Cpt]);
                MessageInformation += " n'a pas été téléchargé correctement";
            }
        }
        /// <summary> Vérifie que les identifiants de l'utilisateur sont connus du site de téléchargement des fichiers HGT </summary>
        private static DialogResult VerifierID_User()
        {
            DialogResult Ret;
            // on met à jour les identifiants de connexion
            ICredentials myCredentials = new CredentialCache() { { new Uri(URS), "Basic", new NetworkCredential(ID_User, MP_User) } };
            using (var MessageHandler = new HttpClientHandler(){
                                                                    Credentials = myCredentials,
                                                                    PreAuthenticate = false,
                                                                    AllowAutoRedirect = true
                                                                })
            {
                // on crée la nouvelle connexion
                using (var HttpClientAltitude = new HttpClient(MessageHandler))
                {
                    try
                    {
                        // Execute la requête
                        var response = HttpClientAltitude.GetAsync(URI_User).Result;
                        // à ce niveau on regarde simplement si la requête est Ok. Cela indique que les identifiants sont corrects car
                        // le fichier existe sur le serveur.
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Ret = DialogResult.OK;
                        }
                        else
                        {
                            Ret = DialogResult.Cancel;
                        }
                    }
                    catch
                    {
                        // Mais le serveur peut être tombé!!!
                        MessageInformation = "Le serveur EarthData est indisponible." + CrLf + "Veuillez ré-essayer plus tard.";
                        AfficherInformation();
                        Ret = DialogResult.Cancel;
                    }
                }
            }
            return Ret;
        }
        #endregion
    }
}