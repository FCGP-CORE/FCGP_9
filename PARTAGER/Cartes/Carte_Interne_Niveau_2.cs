using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Coordonnees.ProjectionMercatorUtm;
using static FCGP.Enumerations;

namespace FCGP
{
    internal partial class Carte
    {
        #region Structures internes
        /// <summary> permet le dessin de la grille ou l'ajout des étiquettes de la grille. </summary>
        /// <remarks> Chaque point est exprimé en pixel par rapport au coin N°0 </remarks>
        private class Grille
        {
            /// <summary>Tableau qui stockent l'ensemble des points qui composent la grille</summary>
            internal Point[,] Points;
            /// <summary>Indice de début de boucle sur les colonnes de la carte (axe des X)</summary>
            internal int MinX;
            /// <summary>Indice de fin de boucle sur les colonnes de la carte (axe des X)</summary>
            internal int MaxX;
            /// <summary>Indice de fin de boucle sur les lignes de la carte (axe des Y)</summary>
            internal int MinY;
            /// <summary>Indice de début de boucle sur les lignes de la carte (axe des Y)</summary>
            internal int MaxY;
            /// <summary>N°de pixel en deça duquel le dessin ne sera pas effectif sur l'axe des X</summary>
            internal int ClipZoneDeb;
            /// <summary>N°de pixel au dela duquel le dessin ne sera pas effectif sur l'axe des X</summary>
            internal int ClipZoneFin;
        }
        /// <summary>Variables spécifiques pour l'interpolation des cartes avec grille : rendu obligatoire pour les calculs multi threads</summary>
        private class ICAG
        {
            /// <summary>pas de longitude en DD/Pixels de la carte interpolée</summary>
            internal double InterDeltaLong;
            /// <summary>pas de latitude en DD/Pixels de la carte interpolée</summary>
            internal double InterDeltaLat;
            /// <summary>Coordonnée X en pixel source pour chaque pas en longitude de la carte interpolée</summary>
            internal double[,] CoordX;
            /// <summary>Coordonnée Y en pixel source pour chaque pas en latitude de la carte interpolée</summary>
            internal double[,] CoordY;
            /// <summary>nb de colonnes des coordonnées sur l'axe des X</summary>
            internal int NbPasX;
            /// <summary>pas de la grille des coordonnées en pixels sur l'axe des X</summary>
            internal int PasX;
            /// <summary>nb de colonnes des coordonnées sur l'axe des Y</summary>
            internal int NbPasY;
            /// <summary>pas de la grille des coordonnées en pixels sur l'axe des Y</summary>
            internal int PasY;
            /// <summary>Fraction entre les valeurs de colonnes Deltamax et DeltaMax+1  dans CoordY qui contient les latitudes min de la carte (Fin de carte)</summary>
            internal double DeltaLatMax;
            /// <summary>index de la colonne dans CoordY qui contient les latitudes maximum de la carte</summary>
            internal int IndexLatMax;
            /// <summary>Fraction entre les valeurs de colonnes Deltamin et DeltaMin+1  dans CoordY qui contient les latitudes min de la carte (Fin de carte)</summary>
            internal double DeltaLatMin;
            /// <summary>index de la colonne dans CoordY qui contient les latitudes minimum de la carte</summary>
            internal int IndexLatMin;
            /// <summary>latitude minimale de début de tranche</summary>
            internal int DebutSource;
            /// <summary>index sup de la ligne interpolée de fin de tranche dans la table CoordY</summary>
            internal int IndexLigneInterpole;
            /// <summary> index du 1er octet dans le tableau </summary>
            internal int IndexBits;
            /// <summary> tableau d'octects de la carte interpole </summary>
            internal byte[] Bits;
            /// <summary> trouve l'index de la latitude maximale </summary>
            internal void TrouverIndexLatMaxInterpole(int HauteurSource)
            {
                for (int Cpt = IndexLigneInterpole, loopTo = CoordY.GetUpperBound(1); Cpt <= loopTo; Cpt++)
                {
                    if (CoordY[IndexLatMax, Cpt] >= HauteurSource)
                    {
                        IndexLigneInterpole = Cpt;
                        return;
                    }
                }
            }
        }
        /// <summary>Enumération liés au retour de la fonction interpole une carte</summary>
        private enum ErreurInterpolation
        {
            /// <summary>l'interpolation de la carte c'est bien déroulée</summary>
            Ok = 0,
            /// <summary>l'interpolation de la carte ne c'est pas bien déroulée</summary>
            NotOk = 1,
            /// <summary>la carte n'appartient pas à un système qui supporte l'interpolation</summary>
            PasConcerne = 2
        }
        #endregion
        #region Constructeurs internes
        /// <summary> constructeur commun </summary>
        private Carte()
        {
            Coins = new GeoRef[4]; // on dimensionne pour mettre à 0 les différente variables pour les 4 coins de la carte
            for (int Cpt = 0; Cpt <= 3; Cpt++)
                // on dimensionne pour remettre à 0 les coordonnées UTM des différentes zones, maximum 3
                Coins[Cpt] = new GeoRef(3);
            // si la carte fait partie d'une collection. Le numéro est géré directement par le programme appelant
            NumId = -1;
            // toute les autres variables friend sont à initialiser par le programme
        }
        /// <summary> Ce constructeur sert pour renvoyer une carte existante </summary>
        /// <param name="Fichiergeoref">chemin complet du fichier georef</param>
        /// <remarks>on regarde uniquement la validité du fichier Georef</remarks>
        private Carte(string Fichiergeoref) : this()
        {
            // Lire l'ensemble du fichier georef
            string[] LignesFichierGeoref = File.ReadAllLines(Fichiergeoref, Encoding_FCGP);
            CheminCarte = Path.GetDirectoryName(Fichiergeoref);
            IsOk = false;
            if (VerifierGeoref(LignesFichierGeoref, ref SystemCartographique) == VerificationRenvoiCarte.OK)
            {
                // initialiser les variables de base de la carte
                string[] S = CheminFluxLecture.Split('.');
                CheminFluxLecture = (string.IsNullOrEmpty(CheminCarte) ? "" : CheminCarte + @"\") + CheminFluxLecture;
                CheminFichiersTuile = CheminCarte; // initialisé par défaut
                Nom = NomSansSuffixe(S[0]);
                NomAjout = SuffixeNom(SystemCartographique);
                Format = ConvertStringToImageFormat(S[1]);
                IsOk = LireFichierGeoref(LignesFichierGeoref);
            }
        }
        #endregion
        #region Variables et constantes internes
        /// <summary> stocke les coordonnées des points de la grille pour le dessin. 1 grille par zone UTM </summary>
        private Grille[] Grilles;
        /// <summary> représente dans le système virtuel la position en pixel du coin(0) de la carte. </summary>
        private Size PointOrigne;
        /// <summary>Nb de zone UTM de la carte. De 1 à 3 pour GF, de 1 à 2 pour DT et SM</summary>
        private int NbZonesUTM;
        /// <summary>dans le cas d'une carte existante indique si la grille est dessinée, dans le cas d'une capture indique si la grille a été dessinée sur la carte ''' </summary>
        private bool GrilleIsAffiche;
        /// <summary>Largeur en mètres  la carte. A partir des coordonnées capturées pour les cartes avec Grille ou DD pour les cartes Mercator ou interpolée </summary>
        private double LargeurMetres;
        /// <summary>Hauteur en mètres de la carte. A partir des coordonnées capturées pour les cartes avec Grille ou DD pour les cartes Mercator ou interpolée </summary>
        private double HauteurMetres;
        /// <summary> Couleur d'un pixel interpoler en dehors de la carte </summary>
        private byte[] PixelInterpol;
        /// <summary>Numéro de la carte. Cela permet d'avoir une cle unique dans une collection de carte (Echelle)</summary>
        private int NumId;
        /// <summary>indique le support de traitement des cartes</summary>
        private TypeSupportCarte TamponType;
        /// <summary>indique pour le support fichier si on autorise tous les formats d'enregistrement de la carte ou uniquement BMP (pas besoin de mémoire)</summary>
        private bool ToutFormatCarte;
        /// <summary>Hauteur en pixel de la reserve de la carte, la largeur étant toujours le nb octets de la largeur de la carte</summary>
        private int HauteurReserve;
        /// <summary>Tampon de stockage de l'image de la carte</summary>
        private EditableBitmap Tampon;
        /// <summary> Nb de DPI pour obtenir l'échelle d'impression sur les X</summary>
        private float DPIImpressionX;
        /// <summary> Nb de DPI pour obtenir l'échelle d'impression sur les Y</summary>
        private float DPIImpressionY;
        /// <summary> indique si la carte a été détruite</summary>
        private bool disposed;
        private const string ExtensionCheminTuiles = @"\tuiles";
        private const string ExtensionCheminFichiers = @"\files";
        private const string Suffixe_Interpol = "_WGS84";
        private const string Suffixe_Grille = "_LV03";
        private const string Suffixe_DD = "_Mercator";
        #endregion
        #region procédure internes 02
        #region Tampon mémoire <--> fichier raw ou BMP
        /// <summary>réalise la copie des données image du fichier source dans le fichier destination en inversant les lignes de pixel</summary>
        /// <param name="NomFichierDst">chemin complet du fichier destination résultant de la conversion</param>
        /// <remarks> cette procédure ne sert que pour le support fichier </remarks>
        private bool FichierSrcToDst(string NomFichierDst, bool FlagFormatBMP)
        {
            bool FichierSrcToDstRet = false;
            try
            {
                // calcul de la hauteur des 2 tampons pour réaliser le transfert
                int HauteurTravail = HauteurReserve / 2d > HauteurPixel ? HauteurPixel : HauteurReserve / 2;
                // on divise le tampon en 2 pour faire le transfert
                int TailleTravail = LargeurOctets * HauteurTravail;
                using (var FichierDst = new FileStream(NomFichierDst, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    if (FlagFormatBMP)
                        EcrireEnteteFichierBMP(FichierDst);
                    EcrireDonneesFichierSrcToFichierDst(FichierDst, TailleTravail, HauteurTravail);
                }
                FichierSrcToDstRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "U3Q7");
            }

            return FichierSrcToDstRet;
        }
        /// <summary> écrit les données d'un fichier raw (binaire) dans un fichier BMP (inversion des lignes de pixels)</summary>
        /// <param name="FichierDst">fichier qui va recevoir les données</param>
        /// <param name="TailleTampon">nombre d'octets réservés pour le transfert de fichier à fichier</param>
        private void EcrireDonneesFichierSrcToFichierDst(FileStream FichierDst, int TailleTampon, int HauteurTampon)
        {
            try
            {
                using (var FichierSrc = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                {
                    // on commence par la fin du fichier
                    long PointeurFichierSrc = FichierSrc.Seek(0L, SeekOrigin.End);
                    byte[] TamponTransfert = Tampon.Bits;
                    int TamponSrcIndex = Tampon.IndexBits;
                    int TamponDstIndex = Tampon.IndexBits + TailleTampon;
                    int PointeurTamponSrc, PointeurTamponDst, HauteurALire = default;
                    do
                    {
                        if (HauteurALire + HauteurTampon < HauteurPixel)
                        {
                            HauteurALire += HauteurTampon;
                        }
                        else
                        {
                            HauteurTampon = HauteurPixel - HauteurALire;
                            TailleTampon = LargeurOctets * HauteurTampon;
                            HauteurALire = HauteurPixel;
                        }
                        // on retir la taille du tampon à la base de la dernière lecture 
                        PointeurFichierSrc -= TailleTampon;
                        FichierSrc.Seek(PointeurFichierSrc, SeekOrigin.Begin);
                        // on lit le nb d'octets dans la partie du tampon réservé à Src
                        FichierSrc.Read(TamponTransfert, TamponSrcIndex, TailleTampon);
                        PointeurTamponSrc = TamponSrcIndex + TailleTampon;
                        PointeurTamponDst = TamponDstIndex;
                        for (int Cpt = 0, loopTo = HauteurTampon - 1; Cpt <= loopTo; Cpt++) // pour chaque ligne
                        {
                            PointeurTamponSrc -= LargeurOctets;
                            Buffer.BlockCopy(TamponTransfert, PointeurTamponSrc, TamponTransfert, PointeurTamponDst, LargeurOctets);
                            PointeurTamponDst += LargeurOctets;
                        }
                        // on écrit la part dont les ligne ont été inversées
                        FichierDst.Write(TamponTransfert, TamponDstIndex, TailleTampon);
                    }
                    while (HauteurALire != HauteurPixel);
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "W3K9");
            }
        }
        /// <summary> écrit dans le fichier BMP passé en paramètre l'entête du fichier en fonction des différents éléments de la carte</summary>
        private void EcrireEnteteFichierBMP(FileStream FichierBMP)
        {
            uint TailleImage = (uint)((uint)LargeurOctets * HauteurPixel);
            var Reserved = new byte[8];
            byte[] Marque = Encoding.UTF8.GetBytes("BM");
            FichierBMP.Write(Marque, 0, Marque.Length); // marque des fichiers BMP : 2 octets
            FichierBMP.Write(BitConverter.GetBytes(TailleImage + (long)OffsetDataBMP), 0, 4); // taille du fichier : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(0), 0, 4); // réservé 2*2 octets : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(OffsetDataBMP), 0, 4); // offset image : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(40), 0, 4); // taille structure : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(LargeurPixel), 0, 4); // largeur image : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(HauteurPixel), 0, 4); // hauteur image : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(1), 0, 2); // nb de plan : 2 octets
            FichierBMP.Write(BitConverter.GetBytes(24), 0, 2); // nb de bits par pixels : 2 octets
            FichierBMP.Write(BitConverter.GetBytes(0), 0, 4); // compression de l'image : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(TailleImage), 0, 4); // taille de l'image : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(DPIImpression(DPIImpressionX)), 0, 4); // DpiX de l'image : 4 octets
            FichierBMP.Write(BitConverter.GetBytes(DPIImpression(DPIImpressionY)), 0, 4); // DpiY de l'image : 4 octets
            FichierBMP.Write(Reserved, 0, Reserved.Length); // réservé : 8 octets
            FichierBMP.Flush();
        }
        #endregion
        #region fichier RAW <--> Fichier Image (Bmp, Png, Jpeg)
        /// <summary>réalise la conversion d'un fichier image au format BMP, PNG ou JPG en un fichier au format RAW </summary>
        /// <param name="NomFichierBIN">chemin complet du fichier Raw résultant de la conversion</param>
        private bool FichierFormatsToRaw(string NomFichierBIN)
        {
            bool FichierFormatsToRawRet = false;
            try
            {
                // décompresse l'image de la carte en mémoire
                using (var m_bitmap = new Bitmap(CheminFluxLecture))
                {
                    // on verrouille les données pour y avoir acces
                    var BD = m_bitmap.LockBits(new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height), ImageLockMode.ReadOnly, m_bitmap.PixelFormat);
                    using (var F = new FileStream(NomFichierBIN, FileMode.Create, FileAccess.Write))
                    {
                        for (int Cpt = 0, loopTo = BD.Stride * BD.Height - 1; Cpt <= loopTo; Cpt++)
                            F.WriteByte(Marshal.ReadByte(BD.Scan0, Cpt));
                    }
                    m_bitmap.UnlockBits(BD);
                }
                FichierFormatsToRawRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "E0E4");
            }

            return FichierFormatsToRawRet;
        }
        /// <summary>enregistre un fichier binaire en tant qu'image et avec un format spécifique. Utilise en mémoire la taille de l'image soit jusqu'à 1.5 Go</summary>
        /// <param name="NomFichierDst"> chemin d'enregistrement de la carte</param>
        /// <param name="FlagJpeg">indique si true que le format Jpeg est obligatoire sinon on utilise le format de la carte</param>
        private bool FichierRawToFormats(string NomFichierDst, bool FlagJpeg)
        {
            bool FichierRawToFormatsRet = false;
            try
            {
                using (var B = new EditableBitmap(LargeurPixel, HauteurPixel, null, "Tampon Convertion Image"))
                {
                    B.Bitmap.SetResolution(DPIImpressionX, DPIImpressionY);
                    using (var FichierBIN = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                    {
                        FichierBIN.Read(B.Bits, 0, B.Bits.Length);
                    }
                    if (Format == ImageFormat.Jpeg || FlagJpeg)
                    {
                        B.Bitmap.Save(NomFichierDst, EncodeurJpeg, QualiteJPEG(Settings.PartagerSettings.QUALITE_JPEG));
                    }
                    else
                    {
                        B.Bitmap.Save(NomFichierDst, Format);
                    }
                }
                FichierRawToFormatsRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "V2T6");
            }

            return FichierRawToFormatsRet;
        }
        #endregion
        #region Décompose coordonnées référencement d'un fichier Georef
        /// <summary>Décompose les coordonnées capturées sous forme de chaine  et les transforme en coordonnées numériques et en DD WGS84</summary>
        /// <param name="Site"> site carto des coordonnées </param>
        /// <param name="Coin">structure Georef qui contient les coordonnées capturées à décomposer</param>
        private static bool DecomposerCoordonneesCapturees(SystemeCartographique Site, GeoRef Coin)
        {
            bool DecomposerCoordonneesCaptureesRet = false;
            try
            {
                {
                    ref var withBlock = ref Coin;
                    var D = Site.Projection.Datum;
                    int Z = Site.ZoneUtmReferencement;
                    char H = Site.HemisphereUtmReferencement;
                    if (Site.Projection.UniteCoordonnees == UnitesCoordonnees.DMS)
                    {
                        // si les coordonnées sont de format longitude latitude
                        if (DecomposerCoordonneesDMS(ref Coin))
                        {
                            if (D != Datums.WGS84) // si les coordonnées DD ne sont pas en WGS84 on fait la transformation
                            {
                                var Ret = ProjectionCartesienne.ConvertDatumToDatum(D, Coin.LatLon, Datums.WGS84, true);
                                if (Ret.IsEmpty)
                                {
                                    var Ex = new Exception("DecomposerCoordonneesDMS && ConvertDatumToDatum" + CrLf + "retourne une coordonnée vide.");
                                    AfficherErreur(Ex, "N3L5");
                                    return DecomposerCoordonneesCaptureesRet;
                                }
                                Coin.LatLon = Ret;
                            }
                            DecomposerCoordonneesCaptureesRet = true;
                        }
                    }
                    // si les coordonnées sont en mètres
                    else if (DecomposerCoordonneesMetres(ref Coin))
                    {
                        if (D == Datums.Grille_Suisse)
                        {
                            ProjectionSuisse.LV95ToLV03(ref Coin.Grille);
                        }
                        var Ret = ConvertProjectionToWGS84(D, new PointProjection(Coin.Grille, Z, H));
                        if (Ret.IsEmpty)
                        {
                            var Ex = new Exception("DecomposerCoordonneesMetres && ConvertProjectionToWGS84" + CrLf + "retourne une coordonnée vide.");
                            AfficherErreur(Ex, "ACE9");
                            return DecomposerCoordonneesCaptureesRet;
                        }
                        Coin.LatLon = Ret;
                        DecomposerCoordonneesCaptureesRet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "X6J8");
            }
            return DecomposerCoordonneesCaptureesRet;
        }
        /// <summary>Décompose une chaine de caractères copiée à partir du site GéoPortail qui contient une coordonnée en degrès, minutes, secondes
        /// et renvoie cette coordonnée sous forme de double en degrés. Il y a une distinction entre longitude en début de chaine ou 
        /// latitude en fin de chaine. Format chaine attendu : E:04°51'10.4"N:45°31'02.8" (pas d'espace)
        /// ou : E: 4° 51' 10.4''   N: 45° 31' 02.8'' (avec espaces)</summary>
        /// <param name="Coin"> coin sur lequel on travail</param>
        private static bool DecomposerCoordonneesDMS(ref GeoRef Coin)
        {
            bool DecomposerCoordonneesDMSRet = false;
            try
            {
                string CoordonneesDMS = Coin.CoordonneesCaptureEcran;
                // si ça ne commence pas par une lettre de Longitude ou si la longueur est <= à 25 il y a un problème
                if (!((CoordonneesDMS.StartsWith("E:") || CoordonneesDMS.StartsWith("O:")) && CoordonneesDMS.Length >= 25))
                    return DecomposerCoordonneesDMSRet;
                double Negatif = 1.0d;
                int Pos2 = CoordonneesDMS.IndexOf(':');
                int Pos1 = CoordonneesDMS.IndexOf('°');
                int Pos3 = CoordonneesDMS.IndexOf('\'');
                // D'abord la longitude c'est au début de la chaine des coordonnées
                double Deg = double.Parse(CoordonneesDMS.Substring(Pos2 + 1, Pos1 - Pos2 - 1));          // les degrés sont enregistrés sous forme d'entier. pas de risque d'erreur
                double Minute = double.Parse(CoordonneesDMS.Substring(Pos1 + 1, Pos3 - Pos1 - 1));       // les minutes sont enregistrées sous forme d'entier. pas de risque d'erreur
                Pos1 = CoordonneesDMS.IndexOf('"');
                double Seconde = StrToDbl(CoordonneesDMS.Substring(Pos3 + 1, Pos1 - Pos3 - 1));  // les secondes sont enregistrés sous forme de float xx.x
                                                                                                 // A l'ouest il s'agit d'une longitude négative
                if (CoordonneesDMS.StartsWith("O"))
                    Negatif = -1.0d;
                Coin.Longitude = (Deg + Minute / 60d + Seconde / 3600d) * Negatif;
                // Puis la latitude c'est à la fin de la chaine des coordonnées
                Negatif = 1.0d;
                Pos2 = CoordonneesDMS.IndexOf(':', Pos2 + 1);
                Pos1 = CoordonneesDMS.IndexOf('°', Pos2);
                Pos3 = CoordonneesDMS.IndexOf('\'', Pos2);
                // Si il s'agit de la latitude c'est à la fin de la chaine des coordonnées
                Deg = double.Parse(CoordonneesDMS.Substring(Pos2 + 1, Pos1 - Pos2 - 1));                // les degrés sont enregistrés sous forme d'entier. pas de risque d'erreur
                Minute = double.Parse(CoordonneesDMS.Substring(Pos1 + 1, Pos3 - Pos1 - 1));             // les minutes sont enregistrées sous forme d'entier. pas de risque d'erreur
                Pos1 = CoordonneesDMS.IndexOf('"', Pos2);
                Seconde = StrToDbl(CoordonneesDMS.Substring(Pos3 + 1, Pos1 - Pos3 - 1));        // les secondes sont enregistrés sous forme de float xx.x
                                                                                                // Au sud il s'agit d'une latitude négative
                if (CoordonneesDMS[Pos2 - 1] == 'S')
                    Negatif = -1.0d;
                Coin.Latitude = (Deg + Minute / 60d + Seconde / 3600d) * Negatif;
                DecomposerCoordonneesDMSRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "H0H4");
            }
            return DecomposerCoordonneesDMSRet;
        }
        /// <summary> Décompose une chaine de caractères copiée à partir du site GéoPortail ou de suisse mobile qui contient des coordonnées en mètres
        /// et renvoie ces coordonnées sous forme de double. Format chaine attendu : Coordonnées(m):657954,188821 sans espace
        /// ou E:796746.25N:2060585.36 sans espace, en fait un groupe de caractères suivi d'un nombre le 1er groupe représente les X et le 2ème les Y 
        /// sans séparateur suivi d'un groupe de caractères suivi d'un nombre sans séparateur</summary>
        /// <param name="Coin">coin sur lequel on travaille</param>
        private static bool DecomposerCoordonneesMetres(ref GeoRef Coin)
        {
            bool DecomposerCoordonneesMetresRet = false;
            string CoordonneesGrille;
            try
            {
                const string SeparteurCoord = ",";
                if (Coin.CoordonneesCaptureEcran.IndexOf("N:") != -1)
                {
                    // remplace le N: des captures de coordonnées des sites GF, DT en UTM WGS84 ou Lambert 93 des versions 6 et moins
                    CoordonneesGrille = Coin.CoordonneesCaptureEcran.Replace("N:", SeparteurCoord).Trim();
                }
                else if (Coin.CoordonneesCaptureEcran.IndexOf("Y:") != -1)
                {
                    // remplace le Y: des captures de coordonnées des sites GF, DT en UTM WGS84 ou grilles web des versions 8.5 et plus
                    CoordonneesGrille = Coin.CoordonneesCaptureEcran.Replace(" Y:", "").Trim();
                }
                else
                {
                    CoordonneesGrille = Coin.CoordonneesCaptureEcran.Trim();
                }

                int Pos1 = 0;
                string Nombre = TrouverChaineCompriseEntre(CoordonneesGrille, ref Pos1, ':', SeparteurCoord[0]);
                Coin.X_Grid = StrToDbl(Nombre);
                Nombre = TrouverChaineCompriseEntre(CoordonneesGrille, ref Pos1, SeparteurCoord[0]);
                Coin.Y_Grid = StrToDbl(Nombre);
                DecomposerCoordonneesMetresRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "L2C6");
            }

            return DecomposerCoordonneesMetresRet;
        }
        #endregion
        #region coordonnées référencement --> UTM et DPI
        /// <summary>Cette procédure calcule le nb de zones UTM de la carte.
        /// Elle calcule les X_UTM et Y_UTM pour l'ensemble des zones UTM.
        /// Zone 30 ,31 et 32 pour la France métropolitaine et la Corse. Cela permet de faire le dessin d'une grille UTM si le fond de carte n'en a pas une</summary>
        /// <remarks>Prend en charge le fait que la carte peut être à cheval sur plus d'1 zone UTM</remarks>
        private bool TrouverNbZonesUtm()
        {
            bool TrouverNbZonesUtmRet = false;
            try
            {
                // on détermine le nb de zones en faisant la différence des zones + un
                NbZonesUTM = (int)(Math.Floor(Coins[2].Longitude / 6d) - Math.Floor(Coins[0].Longitude / 6d) + 1d);
                // on calcule la 1ère zone utm commune à toute les coins, le premier appel à la fonction ConvertDatumToUtm détermine le numéro mini de zone UTM
                for (int Cpt = 0; Cpt <= 3; Cpt++)
                {
                    if (Cpt > 0)
                    {
                        Coins[Cpt].NumZone_UTM = Coins[0].NumZone_UTM;
                    }
                    var UTM = ConvertLLToUtm(Coins[Cpt].LatLon, Datums.UTM_WGS84, Coins[0].NumZone_UTM, true);
                    if (UTM.Coordonnees.IsEmpty)
                    {
                        Exception Ex = new("Erreur dans la procédure ConvertLLToUtm");
                        AfficherErreur(Ex, "A44C");
                        return TrouverNbZonesUtmRet;
                    }
                    else
                    {
                        Coins[Cpt].NumZone_UTM = UTM.Zone;
                        Coins[Cpt].Hemisphere_UTM = UTM.Hemisphere;
                        Coins[Cpt].UTMs[0] = UTM.Coordonnees;
                    }
                }
                // on calcule les coordonnées UTM des autres zones afin de permettre le dessin de la grille UTM pour l'ensemble des zones
                for (int Boucle = 1, loopTo = NbZonesUTM - 1; Boucle <= loopTo; Boucle++)
                {
                    for (int Cpt = 0; Cpt <= 3; Cpt++)
                    {
                        var UTM = ConvertLLToUtm(Coins[Cpt].LatLon, Datums.UTM_WGS84, Coins[Cpt].NumZone_UTM + Boucle, true);
                        if (UTM.Coordonnees.IsEmpty)
                        {
                            Exception Ex = new("Erreur dans la procédure ConvertLLToUtm");
                            AfficherErreur(Ex, "CA48");
                            return TrouverNbZonesUtmRet;
                        }
                        else
                        {
                            Coins[Cpt].UTMs[Boucle] = UTM.Coordonnees;
                        }
                    }
                }
                TrouverNbZonesUtmRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "X7Q8");
            }
            return TrouverNbZonesUtmRet;
        }
        /// <summary>calcul le nb de metres par pixel. Assume des pixels carrés pour la carte </summary>
        private bool TrouverDPIImpression()
        {
            bool TrouverDPIImpressionRet = false;
            try
            {
                (double Largeur, double Hauteur) DimensionsCarte;
                string EchelleImpression = SystemCartographique.Niveau.ImpressionClef;
                if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.DD || SystemCartographique.IsInterpol)
                {
                    // on estime les dimensions de la carte par rapport aux latitudes/Longitudes des coins car elle est calée dessus
                    DimensionsCarte = CalculerDimensionsCarte(Coins[0].LatLon, Coins[2].LatLon, true);
                }
                else // c'est le sytème suisse non interpolé calé sur la grille du système carto
                {
                    DimensionsCarte = CalculerDimensionsCarte(Coins[0].Grille, Coins[2].Grille, false);
                }
                LargeurMetres = DimensionsCarte.Largeur;
                HauteurMetres = DimensionsCarte.Hauteur;
                DPIImpressionX = (float)(1d / Echelle_M_PixelX * StrToDbl(EchelleImpression) * InchToMm);
                DPIImpressionY = (float)(1d / Echelle_M_PixelY * StrToDbl(EchelleImpression) * InchToMm);
                TrouverDPIImpressionRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "EDA8");
            }

            return TrouverDPIImpressionRet;
        }
        /// <summary>transforme un nb de dpi en résolution par mètres</summary>
        /// <param name="DPI">DPI à transformer</param>
        private static int DPIImpression(float DPI)
        {
            return (int)Math.Floor((double)(DPI * 10f) / CinchToMm);
        }
        #endregion
        #region fonctions liées au nom de carte
        /// <summary> Renvoie un suffixe en fonction du site et de l'interpolation de la carte </summary>
        private static string SuffixeNom(SystemeCartographique SystemCartographique)
        {
            string SuffixeNomRet;
            if (SystemCartographique.IsInterpol)
            {
                SuffixeNomRet = Suffixe_Interpol;
            }
            else if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
            {
                SuffixeNomRet = Suffixe_Grille;
            }
            else
            {
                SuffixeNomRet = Suffixe_DD;
            }

            return SuffixeNomRet;
        }
        /// <summary> renvoie le nom de la carte sans suffixe. sert pour les fichiers georef qui ont un nom de carte avec suffixe depuis la version 7.000 </summary>
        private static string NomSansSuffixe(string Nom)
        {
            string NomSansSuffixeRet;
            int Pos = Nom.LastIndexOf('_');
            string Suffixe = null;
            if (Pos > -1)
                Suffixe = Nom[Pos..];

            if ((Suffixe ?? "") == Suffixe_Grille || (Suffixe ?? "") == Suffixe_DD || (Suffixe ?? "") == Suffixe_Interpol)
            {
                NomSansSuffixeRet = Nom[..Pos];
            }
            else
            {
                NomSansSuffixeRet = Nom;
            }

            return NomSansSuffixeRet;
        }
        #endregion
        /// <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                PinceauTrace?.Dispose();
                Tampon?.Dispose();
                Reserve?.Dispose();
                disposed = true;
            }
        }
        #endregion
        #region Functions internes liées à l'interpolation d'une carte
        /// <summary>  interpole la carte avec la méthode bilineaire et correction d'erreur </summary>
        /// <remarks>La carte interpolée aura les mêmes dimensions en pixel que la carte d'origine</remarks>
        private ErreurInterpolation InterpolerCarte(Carte CarteInterpole)
        {
            ErreurInterpolation InterpolerCarteRet = ErreurInterpolation.NotOk;
            try
            {
                MessInfo = $"{NomComplet} :{Separateur}Interpolation de la carte :{Separateur}";
                // on initialise la carte interpolée avec les principales variables de la carte source
                if (!InitialiserCarteInterpole(CarteInterpole))
                    return InterpolerCarteRet;

                int HauteurTravail = CarteInterpole.TamponType == TypeSupportCarte.Fichier ? HauteurReserve : HauteurPixel;
                // on associe un tampon à la carte
                CarteInterpole.Tampon = new EditableBitmap(LargeurPixel, HauteurTravail, CarteInterpole.Reserve, "Tampon Interpole");
                // pour éviter de bloquer les boucles parallel, il ne faut pas faire appel à l'objet bitmap du tampon de la carte interpolée
                if (CarteInterpole.SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile)
                {
                    if (InterpolerCarteAvecGrille(CarteInterpole) != ErreurInterpolation.Ok)
                        return InterpolerCarteRet;
                }
                else if (InterpolerCarteSansGrille(CarteInterpole) != ErreurInterpolation.Ok)
                    return InterpolerCarteRet;
                if (CarteInterpole.TamponType != TypeSupportCarte.Fichier)
                {
                    // Pour l'impression de l'image de la carte à la bonne échelle on affecte les DPI à l'image
                    CarteInterpole.Image.SetResolution(CarteInterpole.DPIImpressionX, CarteInterpole.DPIImpressionY);
                }
                InterpolerCarteRet = ErreurInterpolation.Ok;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "C2L2");
            }
            return InterpolerCarteRet;
        }
        /// <summary> copie la carte source et recherche les coordonnées longitude, latitude pour préparer l'interpolation de la carte </summary>
        private bool InitialiserCarteInterpole(Carte CarteInterpole)
        {
            // remplit tous les champs directs
            CarteInterpole.Separateur = Separateur;
            CarteInterpole.HauteurPixel = HauteurPixel;
            CarteInterpole.HauteurReserve = HauteurReserve;
            CarteInterpole.LargeurPixel = LargeurPixel;
            CarteInterpole.Format = Format;
            CarteInterpole.Nom = Nom;
            CarteInterpole.CheminCarte = CheminCarte;
            CarteInterpole.CheminFichiersTuile = CheminFichiersTuile;
            CarteInterpole.GrilleIsAffiche = GrilleIsAffiche;
            CarteInterpole.DemandeAfficherGrille = DemandeAfficherGrille;
            CarteInterpole.DemandeAjouterCoordonneesGrille = DemandeAjouterCoordonneesGrille;
            CarteInterpole.DemandeFichiersGeoref = DemandeFichiersGeoref;
            CarteInterpole.DemandeFichiersTuiles = DemandeFichiersTuiles;
            CarteInterpole.DemandeAjoutNiveau = DemandeAjoutNiveau;
            CarteInterpole.FacteurJNX = FacteurJNX;
            CarteInterpole.FacteurORUX = FacteurORUX;
            CarteInterpole.PointOrigne = Size.Empty;
            CarteInterpole.NbZonesUTM = NbZonesUTM;
            CarteInterpole.DimensionsTuile = DimensionsTuile;
            CarteInterpole.TamponType = TamponType;
            CarteInterpole.ToutFormatCarte = ToutFormatCarte;
            CarteInterpole.NbTracesKML = NbTracesKML;
            CarteInterpole.FichiersTRK = FichiersTRK;
            CarteInterpole.RegionVirtuelle = RegionVirtuelle;
            CarteInterpole.LargeurOctets = LargeurOctets;
            CarteInterpole.TailleReserve = TailleReserve;
            var R = default(RectangleD);
            if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
            {
                // trouve les coordonnees des 4 coins de la carte interpolée en LL. On prend en compte le rectangle maximum contenu dans les LL de la cartesource
                // on garde les mêmes dimensions pour les pixels ce qui induit de fait un changement d'échelle et du blanc qui apparait sur les bords du à la distorsion
                // de l'image
                TrouverCoordonnesCarteAvecGrille(CarteInterpole, ref R);
            }
            else
            {
                // trouve les coordonnées des 4 coins de la carte interpolée en LL. Il s'agit d'une simple copie d'informations, Coins étant une structure
                // et les LL et pixels sont les mêmes
                CarteInterpole.Coins = Coins;
                // on prend un système cartographique hors site de capture car les fonctions de conversion de coordonnées ne s'appliquent pas
                R = RegionReelle;
            }
            // pour une carte interpolée le systeme cartographique est forcément Autre (les fonctions de calcul de coordonnées sont basées sur des pas à unités/pixel constants) 
            // et le point d'origine = empty 
            CarteInterpole.SystemCartographique = new SystemeCartographique(R, RegionVirtuelle.Size, SystemCartographique.Clef, SystemCartographique.Niveau.Clef, SystemCartographique.Projection.Libelle, SystemCartographique.ZoneUtmReferencement, SystemCartographique.HemisphereUtmReferencement);
            CarteInterpole.TrouverDPIImpression();
            CarteInterpole.NomAjout = SuffixeNom(CarteInterpole.SystemCartographique);
            CarteInterpole.IsOk = CarteInterpole.ReserverMemoireTampon("Reserve Carte Interpole");
            return CarteInterpole.IsOk;
        }
        /// <summary> trouve les coordonnées qui vont servir à l'interpolation. C'est les LL qui sont prise en compte. Il s'agit du plus grand rectangle en LL contenu dans la carte </summary>
        private void TrouverCoordonnesCarteAvecGrille(Carte CarteInterpole, ref RectangleD R)
        {
            try
            {
                var InterLon = new double[4];
                var InterLat = new double[4];
                // Trouver les coordonnées en DMS de la carte interpolée. On choisit le plus grand rectangle  de DD contenu dans la carte source
                InterLon[0] = Math.Min(Coins[0].Longitude, Coins[3].Longitude);
                InterLon[3] = InterLon[0];
                InterLon[2] = Math.Max(Coins[2].Longitude, Coins[1].Longitude);
                InterLon[1] = InterLon[2];
                InterLat[0] = Math.Max(Coins[0].Latitude, Coins[1].Latitude);
                InterLat[1] = InterLat[0];
                InterLat[2] = Math.Min(Coins[2].Latitude, Coins[3].Latitude);
                InterLat[3] = InterLat[2];
                R = new RectangleD(InterLon[0], InterLat[0], InterLon[2], InterLat[2]);
                for (int CptPt = 0; CptPt <= 3; CptPt++)
                {
                    // on recalcul les coordonnées MN03 à partir des coordonnees DMS
                    var PointGrille = ConvertWGS84ToProjection(new PointD(InterLon[CptPt], InterLat[CptPt]), SystemCartographique.Projection.Datum);
                    // calcul des captures d'écran correspondantes
                    CarteInterpole.Coins[CptPt].CoordonneesCaptureEcran = ConvertPointXYtoChaine(PointGrille, "N2");
                    // remplissage des diverses données nécessaires à l'écriture du fichier Georef
                    CarteInterpole.Coins[CptPt].Grille = PointGrille.Coordonnees;
                    CarteInterpole.Coins[CptPt].LatLon = new PointD(InterLon[CptPt], InterLat[CptPt]);
                    CarteInterpole.Coins[CptPt].Pixels = Coins[CptPt].Pixels;
                }
                // calcul des coordonnées en UTM WGS84
                CarteInterpole.TrouverNbZonesUtm();
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A5L0");
            }
        }
        #endregion
    }
}