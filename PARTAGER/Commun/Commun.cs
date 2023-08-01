using static FCGP.AfficheInformation;
using static FCGP.ConvertirCoordonnees;
using static FCGP.DonneesSiteWeb;

namespace FCGP
{
    /// <summary> contient toutes les variables, fonctions, procédures communes à au moins 2 des 3 applications Capturer, Convertir et Regrouper </summary>
    internal static class Commun
    {
        #region Variables  et procedures communes
        internal readonly static string CrLf = Environment.NewLine;
        internal readonly static char Cr = '\r';
        internal readonly static char Lf = '\n';
        internal readonly static char Tab = '\t';
        internal readonly static char NullChar = '\0';
        internal readonly static char Back = '\b';
        internal const string Contact = "fcgp@laposte.net";
        internal static bool FlagAborted;
        private static string _CheminParDefaut;

        /// <summary> id du thread de l'interface utilisateur </summary>
        internal static int ID_Thread_IU { get; set; }
        internal static string CheminParDefaut
        {
            get
            {
                _CheminParDefaut = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FCGP";
                if (!Directory.Exists(_CheminParDefaut))
                    Directory.CreateDirectory(_CheminParDefaut);
                return _CheminParDefaut;
            }
        }
        /// <summary>pour stocker le séparateur de nombre décimal de l'ordinateur</summary>
        internal static char SeparateurDecimalOriginal { get; private set; }
        /// <summary>indique que l'ordinateur à 2Go ou moins de mémoire</summary>
        internal static bool FlagSmallMemory { get; private set; }
        /// <summary>pour stocker le nom du process en cours FCGP ou FCGP.vhost</summary>
        internal static string NomProcess { get; private set; }
        /// <summary>correspond au chemin des documents de l'utilisateur allonger par un repertoire à créer</summary>
        internal static string CheminEnregistrementProvisoire { get; private set; }
        /// <summary>Version de FCGP en texte version longue</summary>
        internal static string NumFCGP { get; private set; }
        /// <summary> détermine le code page pour l'encodages des fichiers textes en . net et net core </summary>
        internal static Encoding Encoding_FCGP { get; private set; }
        /// <summary>numero de version du programme FCGP</summary>
        internal static string NumVersionFCGP
        {
            set
            {
                _NumVersionFCGP = value;
            }
        }
        /// <summary>pour stocker le type du programme  Capturer, Convertir ou Regrouper </summary>
        internal static string TypeFCGP
        {
            get
            {
                return _TypeFCGP;
            }
            set
            {
                _TypeFCGP = value;
            }
        }
        /// <summary> label qui sert à l'affichage des informations d'un thread de travail à un thread WindowsForm </summary>
        internal static Label LabelInformation
        {
            set
            {
                _LabelInformation = value;
            }
        }
        /// <summary> Formulaire principal de l'application </summary>
        internal static Form FormApplication { get; private set; }
        /// <summary>pour stocker la date de la version de fcgp en cours</summary>
        internal static string DateVersionFCGP { get; set; }
        /// <summary> indique si le système est connecté à internet </summary>
        internal static bool IsConnected { get; set; }
        /// <summary> donne la plus grande resolution d'écran du système </summary>
        internal static Size MaxBoundsEcran { get; private set; }
        /// <summary> dimension en pixel de l'écran servant de support au formulaire principal de l'application </summary>
        internal static Rectangle DimensionsEcranSupport { get; set; }
        /// <summary> position et surface de la visue(PictureBox qui sert à afficher la carte) sur le bureau windows </summary>
        internal static Rectangle VisueRectangle { get; set; }
        /// <summary> renvoie le GUID de l'encoder Jpeg .net </summary>
        internal static ImageCodecInfo EncodeurJpeg { get; private set; }
        /// <summary>taille maxi pour le tampon d'une carte</summary>
        internal const int TailleMaxTampon = 536870912;  // 500 Mo ou 0.5 Go
        /// <summary>titre de la fenêtre information si erreur FCGP</summary>
        internal const string TitreErreur = "Erreur gérée dans FCGP_";
        /// <summary>nb d'octets par pixel pour le format du bitmap</summary>
        internal const int NbOctetsPixel = 3;
        /// <summary>nb d'octets par pixel pour le format du bitmap</summary>
        internal const PixelFormat FormatPixel = PixelFormat.Format24bppRgb;
        /// <summary> Nb de milleseconde à attendre que tout réagisse </summary>
        internal const int DureeMinuterie = 250;
        /// <summary> Nb d'octets de métadonnées dans l'entête des fichiers BMP </summary>
        internal const int OffsetDataBMP = 54;
        /// <summary> pouce en millimètre </summary>
        internal const double InchToMm = 25.4d;
        /// <summary> centième de pouce en millimètre </summary>
        internal const double CinchToMm = InchToMm / 100d;
        /// <summary> maximum de la largeur ou de la hauteur d'une carte pour une sauvegarde en JPEG ou PNG avec les encodeurs GDI+ </summary>
        internal const int MaxPixelsJpeg = 255 * NbPixelsTuile;
        /// <summary> caractères intedits pour les noms de fichier sous windows </summary>
        internal readonly static char[] CarsInterdits = new char[] { '"', '/', '\\', '*', '?', '<', '>', '|', ':' };
        /// <summary> Vérifie que la chaine passée en paramètre ne contient aucun des caractères interdits pour les noms de fichiers </summary>
        /// <param name="Chaine"> nom du fichier avec ou sans extensions </param>
        internal static string VerifierNomFichier(string Chaine)
        {
            var SN = new StringBuilder(Chaine.Length);
            foreach (char Caractere in Chaine)
            {
                if (Array.IndexOf(CarsInterdits, Caractere) == -1)
                    SN.Append(Caractere);
            }
            return SN.ToString();
        }
        /// <summary> Largeur de l'image au format BMP en octects. Est forcément un multiple de 4 </summary>
        /// <param name="LargeurPixels"></param>
        internal static int StrideImage(int LargeurPixels)
        {
            int StrideImageRet = (LargeurPixels * NbOctetsPixel + NbOctetsPixel) / 4 * 4;
            return StrideImageRet;
        }
        /// <summary> Convertit un nombre double en texte avec le point comme séparateur décimal</summary>
        /// <param name="NombreDouble">nombre (type double) à convertir en texte</param>
        /// <param name="Format">format attendu en sortie (nb de digit, séparateur de millier et décimale)</param>
        internal static string DblToStr(double NombreDouble, string Format)
        {
            string DblToStrRet = NombreDouble.ToString(Format, Nfi_FCGP);
            return DblToStrRet;
        }
        /// <summary> convertit une chaine représentant un nombre décimal en double sans lever d'execption à cause du séparateur décimal du système </summary>
        /// <param name="ChaineNombreDouble"></param>
        internal static double StrToDbl(string ChaineNombreDouble)
        {
            if (SeparateurDecimalOriginal == '.' && ChaineNombreDouble.IndexOf(',') > -1)
            {
                ChaineNombreDouble = ChaineNombreDouble.Replace(',', '.').Replace(" ", null);
            }
            else if (SeparateurDecimalOriginal == ',' && ChaineNombreDouble.IndexOf('.') > -1)
            {
                ChaineNombreDouble = ChaineNombreDouble.Replace('.', ',').Replace(" ", null);
            }
            return double.Parse(ChaineNombreDouble);
        }
        internal static float StrToSng(string ChaineNombreSingle)
        {
            if (SeparateurDecimalOriginal == '.' && ChaineNombreSingle.IndexOf(',') > -1)
            {
                ChaineNombreSingle = ChaineNombreSingle.Replace(',', '.').Replace(" ", null);
            }
            else if (SeparateurDecimalOriginal == ',' && ChaineNombreSingle.IndexOf('.') > -1)
            {
                ChaineNombreSingle = ChaineNombreSingle.Replace('.', ',').Replace(" ", null);
            }
            return float.Parse(ChaineNombreSingle);
        }
        /// <summary> Trouve une chaine qui est comprise entre 2 caractères spécialisés de préférence </summary>
        /// <param name="Chaine"></param>
        /// <param name="PosCar1">Indice de début de recherche du premier caractère spécialisé</param>
        /// <param name="CAR1">caractère spécialisé N°1</param>
        /// <param name="CAR2">caractère spécialisé N°2</param>
        /// <returns>chaine comprise entre les 2 caractères spécialisés sans espace
        /// Poscar1 pointe sur le caractère suivant CAR2 afin de pouvoir enchainer les recherches </returns>
        /// <remarks>il n'y a pas de test pour savoir si car1 ou car2 appartiennent bien à la chaine</remarks>
        internal static string TrouverChaineCompriseEntre(string Chaine, ref int PosCar1, char CAR1, char CAR2 = default)
        {
            int PosCar2;
            string Trouve;
            PosCar1 = Chaine.IndexOf(CAR1, PosCar1);
            if (CAR2 == default(char))
            {
                PosCar2 = Chaine.Length;
            }
            else
            {
                PosCar2 = Chaine.IndexOf(CAR2, PosCar1);
            }
            Trouve = Chaine.Substring(PosCar1 + 1, PosCar2 - PosCar1 - 1).Trim();
            PosCar1 = PosCar2;
            return Trouve;
        }
        /// <summary>convertit une chaine en imageformat. Par défaut en jpeg</summary>
        /// <param name="FormatTexte">texte à convertir</param>
        internal static ImageFormat ConvertStringToImageFormat(string FormatTexte)
        {
            switch (FormatTexte ?? "")
            {
                case "bmp":
                case "Bmp":
                case "BMP":
                    {
                        return ImageFormat.Bmp;
                    }
                case "png":
                case "Png":
                case "PNG":
                    {
                        return ImageFormat.Png;
                    }
                case "Jpeg":
                case "jpg":
                    {
                        return ImageFormat.Jpeg;
                    }
                case "raw":
                    {
                        return ImageFormat.MemoryBmp;
                    }

                default:
                    {
                        return ImageFormat.Jpeg;
                    }
            }
        }
        /// <summary>convertit un integer (généralement un index d'une liste déroulante) en imageformat. par défaut en jpeg</summary>
        /// <param name="IndexFormat ">index à convertir</param>
        internal static ImageFormat ConvertIntToImageFormat(int IndexFormat)
        {
            switch (IndexFormat)
            {
                case 0:
                    {
                        return ImageFormat.Bmp;
                    }
                case 1:
                    {
                        return ImageFormat.Png;
                    }

                default:
                    {
                        return ImageFormat.Jpeg;
                    }
            }
        }
        /// <summary> convertit un point exprimé en DD WGS84 en coordonnées UTM WGS84 puis en texte </summary>
        internal static string ConvertPointDDtoUTM(PointF PointDD)
        {
            var UTM = ConvertWGS84ToProjection(new PointD(PointDD.X, PointDD.Y), Enumerations.Datums.UTM_WGS84);
            var ConvertPointDDtoUTMRet = ConvertPointXYtoChaine(UTM);
            return ConvertPointDDtoUTMRet;
        }
        /// <summary> convertit un point exprimé en DD WGS84 en coordonnées UTM WGS84 puis en texte </summary>
        internal static string ConvertPointDDtoUTM(PointD PointDD)
        {
            var UTM = ConvertWGS84ToProjection(PointDD, Enumerations.Datums.UTM_WGS84);
            var ConvertPointDDtoUTMRet = ConvertPointXYtoChaine(UTM);
            return ConvertPointDDtoUTMRet;
        }
        /// <summary> convertit un pointD qui représente une coordonnées DD en chaine de caractère formatée DMS </summary>
        /// <param name="PointCoordonneesDD"> coordonnées DD </param>
        /// <param name="Separateur">séparateur entre la chaine de la longitude et  la chaine de la latitude</param>
        /// <remarks>ne fait aucune vérification sur la validité du nombre passer en paramètre</remarks>
        internal static string ConvertPointDDtoDMS(PointD PointCoordonneesDD, bool InclureSens = true, int Prec = 1, string Separateur = ",")
        {
            string PrecText = Prec > 0 ? $"00.{new string('0', Prec)}" : "00";
            string LatitudeDMS = ConvertDDtoDMS(PointCoordonneesDD.Y, Enumerations.TypesCoordsDD.Latitude, PrecText, InclureSens).DMSTxt;
            string LongitudeDMS = ConvertDDtoDMS(PointCoordonneesDD.X, Enumerations.TypesCoordsDD.Longitude, PrecText, InclureSens).DMSTxt;
            return $"{LongitudeDMS}{Separateur} {LatitudeDMS}";
        }
        /// <summary> convertit un double qui représente une Lat ou long DD en un Caractère E,O ou N,S un entier qui contient les degrés un entier qui contient les minutes
        /// et un double qui contient les secondes</summary>
        /// <param name="Coordonnees">Coordonnées en DD</param>
        /// <param name="LatLon">Latitude (=0) ou Longitude (=1) </param>
        /// <param name="PrecText">chaine de formatage des décimales associées aux secondes. Ex pas de décimale "", 1 décimale ".0", 2 décimales ".00"</param>
        /// <param name="InclureSens"> Si oui indique le sens dans le DMSTxt avec une lettre sinon ce sera le signe </param>
        internal static (char Sens, int Deg, int Min, double Sec, string DMSTxt) ConvertDDtoDMS(double Coordonnees, Enumerations.TypesCoordsDD LatLon, string PrecText, bool InclureSens)
        {
            (char Sens, int Deg, int Min, double Sec, string DMSTxt) ConvertDDtoDMSRet;
            string Prefix;
            if (InclureSens)
            {
                ConvertDDtoDMSRet.Sens = SensDD[(int)LatLon][Coordonnees < 0d ? 1 : 0];
                Prefix = $"{ConvertDDtoDMSRet.Sens}:";
            }
            else
            {
                ConvertDDtoDMSRet.Sens = Coordonnees < 0d ? '-' : '+';
                Prefix = ConvertDDtoDMSRet.Sens.ToString();
            }
            Coordonnees = Math.Abs(Coordonnees);
            ConvertDDtoDMSRet.Deg = (int)Math.Round(Math.Truncate(Coordonnees));
            double ResteMin = (Coordonnees - ConvertDDtoDMSRet.Deg) * NbMin;
            ConvertDDtoDMSRet.Min = (int)Math.Round(Math.Truncate(ResteMin));
            double ResteSec = ResteMin - ConvertDDtoDMSRet.Min;
            ConvertDDtoDMSRet.Sec = ResteSec * NbSec;
            ConvertDDtoDMSRet.DMSTxt = ConvertDDtoDMSRet.Sec.ToString(PrecText);
            if ((ConvertDDtoDMSRet.DMSTxt ?? "") == (60.ToString(PrecText) ?? ""))
            {
                ConvertDDtoDMSRet.Sec = 0d;
                ConvertDDtoDMSRet.Min += 1;
            }
            ConvertDDtoDMSRet.DMSTxt = ConvertDDtoDMSRet.Min.ToString("00");
            if (ConvertDDtoDMSRet.DMSTxt == "60")
            {
                ConvertDDtoDMSRet.Min = 0;
                ConvertDDtoDMSRet.Deg += 1;
            }
            ConvertDDtoDMSRet.DMSTxt = $"{Prefix}{ConvertDDtoDMSRet.Deg.ToString(FormatCoord[(int)LatLon])}°{ConvertDDtoDMSRet.Min:00}'{DblToStr(ConvertDDtoDMSRet.Sec, PrecText)}\"";
            return ConvertDDtoDMSRet;
        }
        /// <summary>convertit des degrés en DD avec les secondes exprimées par un double </summary>
        /// <param name="Sens">"E" ou "O" pour les longitudes et "N" ou "S" pour les latitudes </param>
        /// <remarks>ne fait aucune vérification sur la validité des nombres passés en paramètre</remarks>
        internal static double ConvertDMStoDD(char Sens, short Degre, short Minute, double Secondes)
        {
            double ConvertDMStoDDRet;
            switch (Sens)
            {
                case 'N':
                case 'E':
                    {
                        ConvertDMStoDDRet = Degre + Minute * InvNbMin + Secondes * Arc;
                        break;
                    }
                case 'S':
                case 'O':
                    {
                        ConvertDMStoDDRet = -(Degre + Minute * InvNbMin + Secondes * Arc);
                        break;
                    }

                default:
                    {
                        ConvertDMStoDDRet = 0d;
                        break;
                    }
            }

            return ConvertDMStoDDRet;
        }
        /// <summary> Convertit un pointD qui représente une coordonnée X/Y en chaine de caractère pour affichage sur l'écran ou mise dans un fichier </summary>
        /// <param name="PointCoordonneesXY"> point single </param>
        /// <param name="FormatXY"> Chaine représentant le nb de décimales des coordonnées X ou Y </param>
        internal static string ConvertPointXYtoChaine(PointProjection PointCoordonneesXY, string FormatXY = "N0")
        {
            string ConvertPointXYtoChaineRet = null;
            if (PointCoordonneesXY.Zone != 0)
            {
                ConvertPointXYtoChaineRet = $"{PointCoordonneesXY.Zone:00} {PointCoordonneesXY.Hemisphere} ";
            }
            ConvertPointXYtoChaineRet += $"X: {DblToStr(PointCoordonneesXY.Coordonnees.X, FormatXY)}, Y: {DblToStr(PointCoordonneesXY.Coordonnees.Y, FormatXY)}";
            return ConvertPointXYtoChaineRet;
        }
        /// <summary> Convertit un point double qui représente une coordonnée long/lat en chaine de caractère pour affichage sur l'écran ou mise dans un fichier </summary>
        /// <param name="PointCoordonneesDD"> point double </param>
        internal static string ConvertPointDDtoChaine(PointD PointCoordonneesDD, string FormatXY = "N8")
        {
            var ConvertPointDDtoChaineRet = $"Lon : {DblToStr(PointCoordonneesDD.X, FormatXY)}, Lat : {DblToStr(PointCoordonneesDD.Y, FormatXY)}";
            return ConvertPointDDtoChaineRet;
        }
        /// <summary> renvoie nombre aléatoire compris entre 2 bornes </summary>
        /// <param name="BorneInf"> Borne infèrieure qui peut être incluse </param>
        /// <param name="BorneSup"> Borne supèrieure qui peut être incluse </param>
        /// <returns></returns>
        internal static int IntAleatoire(int BorneInf, int BorneSup)
        {
            return Rand.Next(BorneInf, BorneSup);
        }
        /// <summary> crée un GUID de 32 caractères à partir de 16 octets aléatoires de la forme d8672581-daf9-5f61-28b7-de75c6000256 </summary>
        internal static string CreateGUID()
        {
            byte[] Buffer = new byte[16];
            Rand.NextBytes(Buffer);
            Guid G = new(Buffer);
            return G.ToString();
        }
        /// <summary> met le paramètre Qualité pour la comprssion des images avec l'encodeurJpeg</summary>
        /// <param name="Qualite">qualité souhaitée de 0 : mauvais à 100 : très bon</param>
        internal static EncoderParameters QualiteJPEG(int Qualite)
        {
            // for the Quality parameter category.
            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var myEncoderParameter = new EncoderParameter(myEncoder, Qualite);
            var QualiteJPEGRet = new EncoderParameters();
            QualiteJPEGRet.Param[0] = myEncoderParameter;
            return QualiteJPEGRet;
        }
        /// <summary> renvoie le projet, module ou class et nom de méthode en fonction d'une clé </summary>
        /// <param name="CleStr"> clé de la méthode </param>
        internal static string RenvoyerNomMethodeErreur(string CleStr)
        {
            ListeMethodes.TryGetValue(CleStr, out string NomMethode);
            return (NomMethode ?? CleStr);
        }
        /// <summary> attend le nb de ms indiqué sans bloquer le thread appelant.</summary>
        /// <param name="NbMillisecondes">nb de ms à attendre avant de rendre la main</param>
        internal static void Attendre(int NbMillisecondes)
        {
            FlagMinuterie = true;
            Minuterie.Change(NbMillisecondes, Timeout.Infinite);
            while (FlagMinuterie)
                Application.DoEvents();
        }
        /// <summary> initialisation de base pour l'ensemble des applications </summary>
        /// <param name="FormApplication"> formulaire de démarage de l'application</param>
        /// <param name="NumeroVersion"> Numéro de la version de l'application sous la forme : ""</param>
        /// <param name="DateVersion"> date de l'application sous la forme : ""</param>
        internal static bool InitialiserBaseApplication(Form FormApplication, string NumeroVersion, string DateVersion)
        {
            // definit le formulaire principal de l'application
            Commun.FormApplication = FormApplication;
            _TypeFCGP = Commun.FormApplication.Name.ToUpper();
            _NumVersionFCGP = NumeroVersion;
            DateVersionFCGP = DateVersion;
            ID_Thread_IU = Environment.CurrentManagedThreadId;
            FlagAborted = true;
            // on indique l'écran support de l'application pour centrer les messages d'information et d'erreur dés le chargement
            VisueRectangle = Commun.FormApplication.Bounds;
            // remplit une liste qui permet d'indiquer correctement la méthode où a eu lieu une erreur gérée par l'application
            if (!RemplirListeMethode())
            {
                return false;
            }

            SeparateurDecimalOriginal = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            Nfi_FCGP = new CultureInfo(CultureInfo.CurrentCulture.Name, false).NumberFormat;
            Nfi_FCGP.NumberDecimalSeparator = ".";
            Nfi_FCGP.NumberGroupSeparator = " ";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding_FCGP = Encoding.GetEncoding(1252);
            return true;
        }
        /// <summary> initialise le module commun à tous les FCGP </summary>
        /// <param name="TailleReserve">Taille en octet de la réserve mémoire de base</param>
        /// <param name="FlagVersion">si true vérifie si une nouvelle version est disponible</param>
        internal static bool InitialiserCommun()
        {
            bool InitialiserCommunRet = false;
#if BETA
            TypeVersion = " Beta";
#elif DEBUG
            TypeVersion = " Debug";
#else
            TypeVersion = " Release";
#endif
            try
            {
                FlagSmallMemory = NativeMethods.GetTotalMemory <= TailleMaxTampon * 4m;
                InitialiserEcran();
                // initialise la version de FCGP en cours
                InitialiseVersion();
                // créer le chemin provisoire
                CheminEnregistrementProvisoire = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $@"\FCGP_{_TypeFCGP}_FichiersProvisoiresCS";
                Directory.CreateDirectory(CheminEnregistrementProvisoire);
                Minuterie = new System.Threading.Timer(new TimerCallback(Minuterie_Tick), null, Timeout.Infinite, Timeout.Infinite);
                GUIDJpeg();
                LabelInformationMethodeMessage = new LabelInformationCallDelegate(AfficherVisueInformation);
                Rand = new Random();
                IsConnected = true;
                ID_Thread_IU = Environment.CurrentManagedThreadId;
                InitialiserCommunRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "Z0A3");
            }

            return InitialiserCommunRet;
        }
#if !BETA
        internal static void VerifierVersionApplication()
        {
            string Uri = $"{CheminFichierMAJ}information{_TypeFCGP.ToLower()}.txt";
            string Fichier = CheminEnregistrementProvisoire + @"\FichierVersion.txt";
            TitreInformation = "Information FCGP_" + _TypeFCGP;
            MessageInformation = "";
            int Ret = TelechargerFichier(Uri, Fichier);
            if (Ret == 0)
            {
                RechercherInformationFCGP(Fichier);
            }
            else if (Ret == 1)
            {
                MessageInformation = $"Une nouvelle version de FCGP_{_TypeFCGP + CrLf}sera bientôt disponible en téléchargement.";
            }
            else
            {
                MessageInformation = $"Le serveur du site internet FCGP n'est pas accessible.{CrLf}Ré-essayer plus tard";
            }
            if (!string.IsNullOrEmpty(MessageInformation))
                AfficherInformation();
        }
#endif
        /// <summary>libère les ressources associées au module commun</summary>
        internal static void CloturerCommun()
        {
            // on efface le répertoire provisoire créer au lancement de FCGP
            if (Directory.Exists(CheminEnregistrementProvisoire))
                Directory.Delete(CheminEnregistrementProvisoire, true);
        }
        /// <summary> determine si l'ordinateur hôte est connecté à internet </summary>
        internal async static Task TesterConnectionInternetAsync()
        {
            try
            {
                using (var F = new System.Net.Sockets.TcpClient())
                {
                    await F.ConnectAsync("www.google.com", 80);
                }
                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
            }
        }
        /// <summary> méthode appelée dans le code pour afficher un text dans le label d'information de l'application</summary>
        /// <param name="Message"> message à afficher </param>
        internal static void AfficherVisueInformation(string Message)
        {
            LabelInformationMessage(Message);
        }
        /// <summary> calcule la distance en mètre entre 2 points exprimés en DD et le cap pour les relier </summary>
        /// <param name="PtDeb"> point de départ </param>
        /// <param name="PtFin"> point d'arrivée </param>
        /// <returns> Longueur en mètre et cap en degré </returns>
        internal static (double Longueur, double Cap) CalculerDistanceCap(PointD PtDeb, PointD PtFin)
        {
            double DeltaLon = Deg_Rad * (PtFin.Lon - PtDeb.Lon);
            double DeltaLat = Deg_Rad * (PtFin.Lat - PtDeb.Lat);
            double CosLon = Math.Cos(Deg_Rad * (PtFin.Lat + PtDeb.Lat) / 2d) * DeltaLon;
            double Longueur = Math.Sqrt(RayonWGS84 * RayonWGS84 * (DeltaLat * DeltaLat + CosLon * CosLon));
            double Cap = Rad_Deg * Math.Atan2(CosLon, DeltaLat);
            return (Longueur, Cap);
        }
        /// <summary> calcule la largeur et la hauteur en mètre d'une carte dont les coordonnées  
        /// sont exprimées en DD ou en mètres. La carte peut être interpolée </summary>
        /// <param name="Pt0"> point HautGauche de la carte </param>
        /// <param name="Pt2"> point BasDroit de la carte </param>
        /// <param name="FlagDD"> indique que les coordonnées sont exprimées en DD </param>
        internal static (double Largeur, double Hauteur) CalculerDimensionsCarte(PointD Pt0, PointD Pt2, bool FlagDD)
        {
            double Hauteur, largeur;
            if (FlagDD)
            {
                var Pt1 = new PointD(Pt2.X, Pt0.Y);
                var Pt3 = new PointD(Pt0.X, Pt2.Y);
                // la hauteur est la même du coté ouest ou est de la carte.
                Hauteur = CalculerDistanceCap(Pt0, Pt3).Longueur;
                // Ce n'est pas le cas pour la largeur qui est plus petite au nord qu'au sud. La largeur tend vers le 0 plus on va au nord
                double LargeurN = CalculerDistanceCap(Pt0, Pt1).Longueur;
                double LargeurS = CalculerDistanceCap(Pt3, Pt2).Longueur;
                largeur = (LargeurN + LargeurS) / 2d;
            }
            else
            {
                Hauteur = Pt0.Y - Pt2.Y;
                largeur = Pt2.X - Pt0.X;
            }
            return (largeur, Hauteur);
        }
        /// <summary> transforme un timespan en chaine de la forme Heure sur 2 chiffres, Minutes sur 2 chiffres, secondes sur 2 chiffres et millièmes de secondes </summary>
        /// <param name="TS"> timespan à transformer </param>
        /// <param name="FlagMs"> indique que l'on veut aussi les millièmes de secondes</param>
        internal static string TimeSpanToStr(TimeSpan TS, bool FlagMs)
        {
            if (FlagMs)
            {
                return TS.ToString(@"hh\:mm\:ss\.fff");
            }
            else
            {
                return TS.ToString(@"hh\:mm\:ss");
            }
        }
        #endregion
        #region variables et procédures internes
        /// <summary> liste des méthodes qui ont une gestion d'erreur </summary>
        private static Dictionary<string, string> ListeMethodes;
        /// <summary> remplit la liste des méthodes qui ont une gestion d'erreur </summary>
        private static bool RemplirListeMethode()
        {
            bool Ret = false;
            try
            {
                ListeMethodes = new Dictionary<string, string>(200);
                if (File.Exists("ListeMethodes.xml"))
                {
                    var MethodesXML = XElement.Load("ListeMethodes.xml");
                    foreach (XElement MethodeXML in MethodesXML.Elements(XName.Get("Methode")))
                    {
                        string CleStr = MethodeXML.Attribute(XName.Get("cle")).Value;
                        if (!string.IsNullOrEmpty(CleStr))
                        {
                            string Program = MethodeXML.Attribute(XName.Get("program")).Value;
                            string Module = MethodeXML.Attribute(XName.Get("module")).Value;
                            string Nom = MethodeXML.Attribute(XName.Get("nom")).Value;
                            ListeMethodes.Add(CleStr, $"{Program} : {Module} : {Nom}");
                        }
                    }
                    Ret = true;
                }
                else
                {
#if DEBUG
                    // obligatoire pour la version Debug afin d'avoir une bonne idée des erreurs gérées
                    MessageInformation = $"La partie Debug de FCGP_{Commun.FormApplication.Name} n'a pas pu être initialisée" + CrLf +
                                          "Veuillez recharger le fichier ListeMethodes.xml dans le répertoire" + CrLf +
                                          "de l'application.";
                    TitreInformation = $"Information FCGP_{_TypeFCGP}_{_NumVersionFCGP} du {DateVersionFCGP}";
                    AfficherInformation();
                    return false;
#else
                    // retourne toujours vrai pour la version Release car cela dépend du package si il est distribué avec ou sans ListeMethodes
                    Ret = true;
#endif
                }

            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "W7X7");
            }
            return Ret;
        }
        private readonly static char[][] SensDD = new char[][] { new char[] { 'N', 'S' }, new char[] { 'E', 'O' } };
        private readonly static string[] FormatCoord = new string[] { "00", "000" };
        private const double NbMin = 60d;
        private const double NbSec = 60d;
        private const double NbArc = NbMin * NbSec;
        private const double InvNbMin = 1d / NbMin;
        private const double Arc = 1d / NbArc;
        private static Label _LabelInformation;
        private static Random Rand;
        /// <summary>chemin de téléchargement des fichiers des mises à jour de version et d'information</summary>
        private const string CheminFichierMAJ = "http://fcgp.e-monsite.com/medias/files/";
        /// <summary> pour écrire des points à la place des virgules sur les ordinateurs qui n'ont pas le point comme séparateur décimal par défaut </summary>
        private static NumberFormatInfo Nfi_FCGP { get; set; }
        /// <summary>flag servant a indiquer que le temps de la minuterie est écoulé</summary>
        private static bool FlagMinuterie;
        /// <summary> minuterie pour les tâches qui sont éxécutées sur un thread qui ne doit pas être bloqué </summary>
        private static System.Threading.Timer Minuterie;
        /// <summary> Indique le numéro de version </summary>
        private static string _NumVersionFCGP;
        /// <summary> FCGP est composé de 3 programmes, indique lequel c'est </summary>
        private static string _TypeFCGP;
        /// <summary> indique si il s'agit d'une Version Release ou d'une Bêta </summary>
        private static string TypeVersion;
        /// <summary> Touve le codec JPEG dans la liste des codecs de GDI+ </summary>
        private static void GUIDJpeg()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            var format = ImageFormat.Jpeg;
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    EncodeurJpeg = codec;
                }
            }
        }
        /// <summary> procédure de rappel pour indiquer que le temps d'attente de la minuterie est écoulé </summary>
        private static void Minuterie_Tick(object Obj)
        {
            FlagMinuterie = false;
        }
        /// <summary> télécharge un fichier qui contient le numéro de la dernière version de FCGP et des informations afin de prévenir les utilisateurs </summary>
        private static void RechercherInformationFCGP(string FichierMAJ)
        {
            string[] LignesFichierMAJ = File.ReadAllLines(FichierMAJ, Encoding_FCGP);
            string DateFCGP = LignesFichierMAJ[1][18..];
            string AnneeFCGP = DateFCGP[6..];
            if (LignesFichierMAJ[0].StartsWith("NumVersionFCGP"))
            {
                try
                {
                    if ((DateFCGP ?? "") != (DateVersionFCGP ?? ""))
                    {
                        string NumVersion = LignesFichierMAJ[0][17..];
                        MessageInformation = $"FCGP_{_TypeFCGP} Version {AnneeFCGP}-{NumVersion} en date du {DateFCGP}" + CrLf +
                                              "est disponible en téléchargement.";
                        // affichage du message d'information
                        var Information = LignesFichierMAJ[2].AsSpan(18);
                        if (Information != "Aucune")
                        {
                            MessageInformation += string.Concat(CrLf, Information);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "S6H7");
                }
            }
            else
            {
                // si le fichier ne correspond pas à ce qu'on attend c'est qu'il est encours de changement
                MessageInformation = $"Une nouvelle version de FCGP_{_TypeFCGP + CrLf}sera bientôt disponible en téléchargement.";
            }
        }
        /// <summary> initialise l'écran qui sera support du formulaire principal et des autres formulaires (navigateur de capture, information ou message d'erreur, aide </summary>
        private static void InitialiserEcran()
        {
            // si c'est la première ouverture ou si le numéro d'écran est > au nb d'écran 
            // la configuration du système a changé généralement pour les portables passage de 2 à 1 écran, on choisi display1
            MaxBoundsEcran = Size.Empty;
            for (int Cpt = 0, loopTo = Screen.AllScreens.Length - 1; Cpt <= loopTo; Cpt++)
            {
                var R = new Rectangle(Point.Empty, Screen.AllScreens[Cpt].Bounds.Size);
                if (R.Contains(new Rectangle(Point.Empty, MaxBoundsEcran)))
                    MaxBoundsEcran = Screen.AllScreens[Cpt].Bounds.Size;
                if (Settings.PartagerSettings.NUM_ECRAN == -1 || Settings.PartagerSettings.NUM_ECRAN > Screen.AllScreens.Length - 1)
                {
                    if (Screen.AllScreens[Cpt].Primary)
                    {
                        Settings.PartagerSettings.NUM_ECRAN = Cpt;
                    }
                }
            }
            // initialise la variable pour le positionnement de la boite d'information
            DimensionsEcranSupport = Screen.AllScreens[Settings.PartagerSettings.NUM_ECRAN].Bounds;
            VisueRectangle = DimensionsEcranSupport;
        }
        /// <summary> renvoie la chaine d'identification du programme </summary>
        private static void InitialiseVersion()
        {
            string TexteVersionFCGP = "FCGP " + _TypeFCGP + " - Version : " + DateVersionFCGP[^4..] + "-";
            NumFCGP = TexteVersionFCGP + _NumVersionFCGP + TypeVersion;
        }
        /// <summary> permet de télécharger un fichier distant en libre accès (sans identifiants) sur le PC en mode synchrone </summary>
        /// <param name="URI"> Adresse du fichier à téléchager </param>
        /// <param name="Fichier"> Chemin du fichier sur le PC</param>
        /// <returns> 0 : Ok, 1 : FileNotFound, 2 : Erreur sur le Serveur de l'Uri, 3 : Serveur de l'Uri inaccessible ou fichier déjà existant </returns>
        private static int TelechargerFichier(string URI, string Fichier)
        {
            using (var Serv = new HttpClient())
            {
                try
                {
                    var requete = new HttpRequestMessage(HttpMethod.Get, URI);
                    var reponse = Serv.Send(requete);
                    if (reponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var Source = reponse.Content.ReadAsStream())
                        {
                            using (var DestinationStream = File.Create(Fichier))
                            {
                                Source.CopyTo(DestinationStream);
                            }
                        }
                        return 0; // Ok
                    }
                    else if (reponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return 1; // FileNotFound
                    }
                    else
                    {
                        return 2;
                    } // Erreur sur le Serveur de l'Uri
                }
                catch
                {
                    return 3;
                } // Serveur de l'Uri inaccessible
            }
        }
        /// <summary> méthode effective qui affiche le texte dans le label d'information </summary>
        /// <param name="Message"> message à afficher </param>
        private static void LabelInformationMessage(string Message)
        {
            if (_LabelInformation.InvokeRequired)
            {
                _LabelInformation.Invoke(LabelInformationMethodeMessage, new object[] { Message });
            }
            else
            {
                _LabelInformation.Text = Message;
            }
        }
        /// <summary> définition de délégué pour l'affichage de text dans le label d'information </summary>
        /// <param name="Message"> paramètre du délégué </param>
        private delegate void LabelInformationCallDelegate(string Message);
        /// <summary> délégué pour l'affichage de text dans le label d'information </summary>
        private static LabelInformationCallDelegate LabelInformationMethodeMessage;
        #endregion
    }
}