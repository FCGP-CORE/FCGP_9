using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    internal partial class Carte : IDisposable
    {
        #region constructeurs, fonctions et méthodes accessibles
        /// <summary> Création de la carte pour les dimensions indiquées et avec soit un tampon dynamique pour contenir les octets de l'image, soit un tampon pour fichier
        /// le reste des champs accessibles reste à initialiser par ailleurs. Ce constructeur est utilisé exclusivement lors de la capture d'une carte. </summary>
        /// <param name="NbX"> nb de tuiles serveur en horizontal </param>
        /// <param name="NbY"> nb de tuiles serveur en vertical </param>
        /// <param name="SystemeCarto"> système cartographique de la carte </param>
        /// <param name="NomCarte"> Nom de la carte </param>
        /// <remarks> appel de la procédure FinaliserCarte obligatoire après l'initialisation des différentes variables publiques </remarks>
        internal Carte(int NbX, int NbY, SystemeCartographique SystemeCarto, string NomCarte) : this()
        {
            try
            {
                Nom = NomCarte;
                LargeurPixel = NbX * NbPixelsTuile;
                HauteurPixel = NbY * NbPixelsTuile;
                LargeurOctets = StrideImage(LargeurPixel);
                SystemCartographique = SystemeCarto;
                NomAjout = SuffixeNom(SystemCartographique);
                TamponType = CartesSettings.SUPPORT_CARTE;
                ToutFormatCarte = CartesSettings.IS_TOUT_FORMAT;
                Separateur = " ";
                Format = ConvertIntToImageFormat(CartesSettings.FORMAT);
                CheminCarte = CapturerSettings.CHEMIN_CARTE;
                DemandeAfficherGrille = CartesSettings.IS_GRILLE;
                DemandeAjouterCoordonneesGrille = CartesSettings.IS_REFERENCES ? ChoixCoordonneesGrille.Ref_Carte : ChoixCoordonneesGrille.Aucune;
                // infos concernant  les fichiers tuiles
                DemandeFichiersTuiles = (ChoixFichiersTuiles)CartesSettings.FICHIER_TUILE;
                CheminFichiersTuile = PartagerSettings.CHEMIN_TUILE;
                FacteurJNX = CartesSettings.FACTEUR_JNX;
                FacteurORUX = CartesSettings.FACTEUR_ORUX;

                if (!ReserverMemoireTampon("Reserve Carte Base"))
                    return;
                if (TamponType != TypeSupportCarte.Fichier)
                {
                    // on associe directement le tampon car c'est le même pour la capture et le traitement
                    Tampon = new EditableBitmap(LargeurPixel, HauteurPixel, Reserve, "Tampon Base");
                }
                else
                {
                    // la hauteur max du tampon de travail dépend de la taille effective de la réserve
                    HauteurReserve = TailleReserve / LargeurOctets;
                    CheminFluxLecture = CheminEnregistrementProvisoire + @"\CarteSansGrille.raw";
                }
                IsOk = true;
            }

            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T9B7");
            }
        }
        /// <summary>Création de la carte pour les dimensions indiquées et avec un support d'image fichier
        /// le reste des champs accessibles reste à initialiser par ailleurs. Ce constructeur est utilisé 
        /// exclusivement lors  de l'aglomération de plusieurs cartes à la demande d'un regroupement </summary>
        /// <param name="LargeurCarte">largeur en pixel de la carte</param>
        /// <param name="HauteurCarte">hauteur en pixel de la carte</param>
        /// <param name="SystemeCarto">systeme cartographique de la carte</param>
        /// <param name="FichierBin">chemin du fichier qui contient les données binaires de la carte</param>
        /// <remarks>appel de la procédure FinaliserCarte obligatoire après l'initialisation des différentes variables publiques</remarks>
        internal Carte(int LargeurCarte, int HauteurCarte, SystemeCartographique SystemeCarto, string FichierBin, int TailleReserveMax) : this()
        {
            try
            {
                TailleReserve = TailleReserveMax;
                if (!ReserverMemoireTampon("Reserve Carte Base"))
                    return;
                IsOk = Reserve.IsOk;
                LargeurPixel = LargeurCarte;
                HauteurPixel = HauteurCarte;
                LargeurOctets = StrideImage(LargeurPixel);
                HauteurReserve = HauteurPixel;
                SystemCartographique = SystemeCarto;
                NomAjout = SuffixeNom(SystemCartographique);
                TamponType = TypeSupportCarte.Fichier; // obligatoirement support fichier car normalement on va travailler sur de grandes quantités de données
                ToutFormatCarte = false; // forcément BMP mais normalement on est pas censé enregistrer la carte car on fait uniquement les fichiers tuiles
                CheminFluxLecture = FichierBin;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "C4C5");
            }
        }
        /// <summary> permet de libérer les ressources associées à l'EditableBitmap </summary>
        public void Dispose()
        {
            Dispose(true);
            // nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(this);
        }

        /// <summary>permet de remplir les coins de la carte d'après les coordonnées sous forme de texte. Doit être appelée obligatoirement après un New Friend</summary>
        /// <param name="Coordonnees">coordonnées sous forme de texte</param>
        /// <param name="TexteInformation">texte à afficher dans la boite d'information pendant le travail de la fonction</param>
        internal bool FinaliserCarte(string[] Coordonnees, string TexteInformation)
        {
            bool FinaliserCarteRet = false;
            try
            {
                AfficherVisueInformation(TexteInformation);
                // transforme les coordonnées capturées en coordonnées réelles WGS84 DD et UTM zone 30 à 32 GF, 20, 22 ou 40 DT
                for (int Cpt = 0; Cpt <= 3; Cpt++)
                {
                    Coins[Cpt].CoordonneesCaptureEcran = Coordonnees[Cpt];
                    // on calcule les pixels en X et Y correspondant au coin en cours de traitement
                    switch (Cpt)
                    {
                        case 0:
                            {
                                Coins[Cpt].X_Pixel = 0;
                                Coins[Cpt].Y_Pixel = 0;
                                break;
                            }
                        case 1:
                            {
                                Coins[Cpt].X_Pixel = LargeurPixel;
                                Coins[Cpt].Y_Pixel = 0;
                                break;
                            }
                        case 2:
                            {
                                Coins[Cpt].X_Pixel = LargeurPixel;
                                Coins[Cpt].Y_Pixel = HauteurPixel;
                                break;
                            }
                        case 3:
                            {
                                Coins[Cpt].X_Pixel = 0;
                                Coins[Cpt].Y_Pixel = HauteurPixel;
                                break;
                            }
                    }
                    if (!DecomposerCoordonneesCapturees(SystemCartographique, Coins[Cpt]))
                        return FinaliserCarteRet;// il peut y avoir une erreur
                }

                // Combien de zones UTM différentes pour la carte : de 1 à 3 au maximum (Echelle 1/500000) (pour le dessin de la grille)
                if (!TrouverNbZonesUtm())
                    return FinaliserCarteRet;
                // pour le calcul des dpi impression de l'image
                if (!TrouverDPIImpression())
                    return FinaliserCarteRet;
                if (TamponType == TypeSupportCarte.Fichier) // on différencie le support de carte car quand il y a capture 
                {
                    // avec un support de carte fichier on a besoin d'un tampon spécifique mais si la réserve de base est trop importante on en prend qu'une partie
                    HauteurReserve = TailleReserve / LargeurOctets;
                    Tampon = new EditableBitmap(LargeurPixel, HauteurReserve, Reserve, "Tampon Base");  // on dimensionne le nouveau tampon
                    if (!Tampon.IsOk)
                        return FinaliserCarteRet;
                }
                else
                {
                    // on indique les DPI au niveau de l'image pour qu'ils soient enregistrés et avoir une impression qui corresponde à l'échelle prévue
                    Image.SetResolution(DPIImpressionX, DPIImpressionY);
                }
                if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
                {
                    PointOrigne = new Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins[0].Grille));
                }
                else
                {
                    PointOrigne = new Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins[0].LatLon));
                }
                RegionVirtuelle = new Rectangle(new Point(PointOrigne), new Size(LargeurPixel, HauteurPixel));
                // on calcul les points de la grille et on la dessine si elle est demandée
                GrilleIsAffiche = SystemCartographique.GrilleExiste;
                // et enregistrer les différents fichiers
                if (!RealiserDemandes())
                    return FinaliserCarteRet; // il y a eu une erreur
                FinaliserCarteRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "U1F0");
            }

            return FinaliserCarteRet;
        }
        /// <summary> permet la création des différents fichiers associés à une carte </summary>
        internal bool RealiserDemandes()
        {
            bool RealiserDemandesRet = false;
            try
            {
                if (DemandeAfficherTrace && FichiersTRK is not null)
                {
                    if (!AfficherTraces())
                        return RealiserDemandesRet; // il peut y avoir une erreur  
                }
                if (DemandeAfficherGrille && !GrilleIsAffiche) // c'est le cas de Géofoncier
                {
                    if (!AfficherGrille())
                        return RealiserDemandesRet; // il peut y avoir une erreur  
                }
                GrilleIsAffiche = SystemCartographique.AfficherGrilleIsOK & GrilleIsAffiche;
                if (DemandeFichiersGeoref > ChoixFichiersGeoref.Aucun)
                {
                    if (!GenererFichiersGeoreferencement())
                        return RealiserDemandesRet;
                }
                // génerer l'interpolation de la carte et les différents fichiers si demandé
                if (DemandeFichiersTuiles > ChoixFichiersTuiles.ORUX ||
                    DemandeFichiersGeoref > ChoixFichiersGeoref.Cartes ||
                    DemandeAjouterCoordonneesGrille > ChoixCoordonneesGrille.Ref_Carte)
                {
                    // il faut à minima demander l'interpolation de la carte
                    if (!GenererInterpolation())
                        return RealiserDemandesRet;
                }
                else if (DemandeFichiersTuiles > ChoixFichiersTuiles.Aucun)
                {
                    if (!GenererFichiersTuiles())
                        return RealiserDemandesRet;
                }
                // faire les références de la carte source si demandé et la grille aussi. Pas de références sans grille
                if (DemandeAjouterCoordonneesGrille.HasFlag((Enum)ChoixCoordonneesGrille.Ref_Carte) && GrilleIsAffiche)
                {
                    if (!AjouterReferences())
                        return RealiserDemandesRet;
                }
                RealiserDemandesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A3G6");
            }

            return RealiserDemandesRet;
        }
        #endregion
        #region Champs accessibles
        /// <summary> lors de la génération des fichiers tuiles, de l'interpolation de la carte, indique combien il y a d'iterations totales à accomplir </summary>
        internal int NbTotalIterations;
        /// <summary> lors de la génération des fichiers tuiles, de l'interpolation de la carte, indique combien il y a d'iterations déjà accompli </summary>
        internal int NbIterations;
        /// <summary> pour les messages d'information. correspond au début de la chaine d'info </summary>
        internal string MessInfo;
        /// <summary> pour les messages d'information. permet soit de continuer sur la même ligne avec " " ou à la ligne suivante avec CrLf </summary>
        internal string Separateur;
        /// <summary> structure qui enregistre les données du système cartographique de la carte </summary>
        internal SystemeCartographique SystemCartographique;
        /// <summary>chemin d'enregistrement de la carte si demandé</summary>
        internal string CheminCarte;
        /// <summary>chemin d'enregistrement du fichier tuile si demandé</summary>
        internal string CheminFichiersTuile;
        /// <summary>nom de la carte sans extension</summary>
        internal string Nom;
        /// <summary> format d'enregistrement de l'image de la carte par exemple BMP </summary>
        internal ImageFormat Format;
        /// <summary>on demande à ce que la ou les traces soit affichées sur la carte</summary>
        internal bool DemandeAfficherTrace;
        /// <summary>on demande à ce que la grille soit affichée sur la carte</summary>
        internal bool DemandeAfficherGrille;
        /// <summary>indique si l'on doit créer un ou des fichiers tuiles</summary>
        internal ChoixFichiersTuiles DemandeFichiersTuiles;
        /// <summary>indique si l'on doit ajouter un niveau à un fichier JNX ou ORUX si True ou créer un fichier JNX ou ORUX si false </summary>
        internal bool DemandeAjoutNiveau;
        /// <summary>indique si un fichier images JPEG contenant les références de la grille doit être inclus dans les fichiers générés</summary>
        internal ChoixCoordonneesGrille DemandeAjouterCoordonneesGrille;
        /// <summary>indique les fichiers georef associés à la carte demandés par l'utilisateur</summary>
        internal ChoixFichiersGeoref DemandeFichiersGeoref;
        /// <summary>Largeur et hauteur en pixels des tuiles composant un fichier tuile</summary>
        internal int DimensionsTuile;
        /// <summary>facteur multiplicateur pour l'échelle maximum de visualisation d'un niveau par rapport à l'original</summary>
        internal double FacteurJNX;
        /// <summary>facteur de correction pour l'échelle maximum de visualisation d'un niveau par rapport à l'original</summary>
        internal int FacteurORUX;
        /// <summary>chemin associé à l'image de travail encours en cas de support de carte typetampon fichier ou en remplissage d'un tampon</summary>
        internal string CheminFluxLecture;
        /// <summary> stocke les tuiles pour les fichiers tuiles. </summary>
        internal DescriptionImageFichier[] Tuiles;
        /// <summary> listes des traces à dessiner sur la carte </summary>
        internal List<string> FichiersTRK;
        /// <summary> pinceau qui va servir à dessiner la trace sur les cartes sur la carte </summary>
        internal Pen PinceauTrace;
        /// <summary> combien de fois doit'on inserer les points d'une trace dans le fichier KMZ : 0 à 2 </summary>
        internal int NbTracesKML;
        #endregion
        #region Propriétés accessibles
        /// <summary>chemin relatif où seront enregistrées les tuiles de l'image de la carte</summary>
        internal static string ExtensionCheminImagesTuiles
        {
            get
            {
                return ExtensionCheminTuiles + ExtensionCheminFichiers;
            }
        }
        /// <summary> nb d'octects associés aux tampons de travail pour les supports de carte type Fichier et SmallMemory </summary>
        internal int TailleReserve { get; private set; }
        /// <summary>tampon reserve (mémoire) pour les supports de carte type Fichier et SmallMemory des cartes interpolées</summary>
        internal SharedPinnedByteArray Reserve { get; private set; }
        /// <summary>Nb de mètres par pixel sur l'axe des X </summary>
        internal double Echelle_M_PixelX
        {
            get
            {
                return LargeurMetres / LargeurPixel;     // En mètre par pixel
            }
        }
        /// <summary>Nb de mètres par pixel sur l'axe des Y </summary>
        internal double Echelle_M_PixelY // peut être déclarée Private mais pour avoir la concordance avec Echelle_M_PixelX() 
        {
            get
            {
                return HauteurMetres / HauteurPixel;     // En mètre par pixel
            }
        }
        /// <summary> Nb de tuiles sur l'axe des X pour un fichier tuile</summary>
        internal int NbTuilesX { get; private set; }
        /// <summary> Nb de tuiles sur l'axe des Y pour un fichier tuile</summary>
        internal int NbTuilesY { get; private set; }
        /// <summary>  </summary>
        internal string NomAjout { get; private set; }
        /// <summary>  </summary>
        internal GeoRef[] Coins { get; private set; }
        /// <summary>  </summary>
        internal int LargeurPixel { get; private set; }
        /// <summary>  </summary>
        internal int HauteurPixel { get; private set; }
        /// <summary> nom complet de la carte. Le suffixe indique si il s'agit d'une carte interpolée ou pas </summary>
        internal string NomComplet
        {
            get
            {
                return Nom + NomAjout;
            }
        }
        /// <summary> flag indiquant si la création de la carte est ok. Ce flag dépend de la manière dont est créée la carte</summary>
        internal bool IsOk { get; private set; }
        /// <summary>Image de la carte, attention il s'agit de l'image associé au tampon de la carte. En cas de support de carte Fichier, le tampon n'est pas une image</summary>
        internal Bitmap Image
        {
            get
            {
                return Tampon.Bitmap;
            }
        }
        /// <summary>pointeur sur le tableau d'octects qui correspond à l'Image de la carte</summary>
        internal IntPtr ImageBitPtr
        {
            get
            {
                return Tampon.BitPtr;
            }
        }
        /// <summary>tableau d'octects qui correspond à l'Image de la carte</summary>
        internal byte[] ImageBits
        {
            get
            {
                return Tampon.Bits;
            }
        }
        internal void ColorerTampon(Color Couleur)
        {
            Tampon.ClearColor(Couleur);
        }
        /// <summary>Nb d'octet à lire pour passer à la ligne suivante (3 octets par pixel + complément de 0 à 3 octets pour arriver à un nb d'octets qui soit un multiple de 4</summary>
        internal int LargeurOctets { get; private set; }
        /// <summary> X,Y, Largeur, hauteur de la carte en monde réel exprimé en unité du système géographique (DD ou M)</summary>
        internal RectangleD RegionReelle
        {
            get
            {
                if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.DD)
                {
                    return new RectangleD(Coins[0].LatLon, Coins[2].LatLon);
                }
                else
                {
                    return new RectangleD(Coins[0].Grille, Coins[2].Grille);
                }
            }
        }
        /// <summary>region commune entre RegionVirtuelle et la région passée en paramètre</summary>
        internal Rectangle CroiserAvecRegionVirtuelle(Rectangle Region)
        {
            return Rectangle.Intersect(RegionVirtuelle, Region);
        }
        /// <summary>X,Y par rapport à l'origine système et Largeur, hauteur de la carte exprimé en Pixel du monde virtuel</summary>
        internal Rectangle RegionVirtuelle { get; private set; }
        /// <summary>Sud de la carte exprimé en pixel du monde virtuel, équivaut à Y + Hauteur ou  Bottom de la position Georef</summary>
        internal int Sud
        {
            get
            {
                return PointOrigne.Height + HauteurPixel;
            }
        }
        /// <summary> Nord de la carte exprimé en pixel du monde virtuel, équivaut à Y de la position Georef</summary>
        internal int Nord
        {
            get
            {
                return PointOrigne.Height;
            }
        }
        /// <summary>Ouest de la carte exprimé en pixel du monde virtuel, équivaut à X de la position Georef</summary>
        internal int Ouest
        {
            get
            {
                return PointOrigne.Width;
            }
        }
        /// <summary>Est de la carte exprimé en pixel du monde virtuel, équivaut à X + Largeur ou Right de la position Georef</summary>
        internal int Est
        {
            get
            {
                return PointOrigne.Width + LargeurPixel;
            }
        }
        #endregion
        #region fonctions partagées accessibles
        /// <summary>renvoie une carte si le système carto du fichier georef est correct et correspond au systeme carto de référence. sert lors de l'ajout d'une carte dans une echelle</summary>
        /// <param name="FichierGeoref">Fichier Géoref à vérifier</param>
        /// <param name="SupportTampon">type de support tampon souhaité</param>
        /// <param name="NumCarte">Numéro de la carte à indiquer dans le champ correspondant uniquement si le programme appelant gére ce numéro pour une collection</param>
        /// <param name="Syst_Carto">systeme géographique au quel doit correspondre celui de la carte</param>
        /// <param name="Ret">Code d'erreur</param>
        /// <returns>CarteAjout crée et renseignée  ou nothing si le systeme Cartographique du fichier Georef est différent de celui passé en paramètre</returns>
        /// <remarks> L'image est associée si le support de la carte est différent de Aucun </remarks>
        internal static Carte RenvoyerCarte(string FichierGeoref, ref VerificationRenvoiCarte Ret, TypeSupportCarte SupportTampon = TypeSupportCarte.Aucun, bool ToutFormat = true, SystemeCartographique Syst_Carto = null, ChoixFichiersTuiles FichiersTuiles = ChoixFichiersTuiles.Aucun, int NumCarte = -1)
        {
            var Car_Aj = new Carte(FichierGeoref);
            Ret = VerificationRenvoiCarte.OK;
            if (!Car_Aj.IsOk)
            {
                Ret = VerificationRenvoiCarte.Erreur_Creation_Systeme;
                Car_Aj.Dispose();
                return null;
            }
            if (Syst_Carto is not null && ((Car_Aj.SystemCartographique.ClefEchelle ?? "") != (Syst_Carto.ClefEchelle ?? "") || Car_Aj.SystemCartographique.IsInterpol))
            {
                Ret = VerificationRenvoiCarte.Systeme_Different;
                Car_Aj.Dispose();
                return null;
            }
            Car_Aj.TamponType = SupportTampon;
            Car_Aj.ToutFormatCarte = ToutFormat;
            Car_Aj.DemandeFichiersTuiles = FichiersTuiles;
            Car_Aj.NumId = NumCarte;
            if (SupportTampon != TypeSupportCarte.Aucun)
            {
                Ret = Car_Aj.AssocierTampon();
                if (Ret != VerificationRenvoiCarte.OK)
                {
                    Car_Aj.Dispose();
                    return null;
                }
            }
            return Car_Aj;
        }
        /// <summary> Sert à renvoyer un système géographique correspondant au fichier Georef si celui ci est valide </summary>
        /// <param name="FichierGeoref">Fichier Géoref à vérifier</param>
        /// <returns>Ok si tout c'est bien passé sinon description de l'erreur</returns>
        /// <remarks>SystemeEchelle est indefinit en cas d'erreur</remarks>
        internal static SystemeCartographique RenvoyerSystemeCartographique(string FichierGeoref)
        {
            string SiteCarto = null;
            string Projection = null;
            string ClefEchelle = null;
            int Z = 0;
            char H = default;
            bool Interpol = false;
            // Lire l'ensemble du fichier georef
            var LignesFichierGeoref = File.ReadAllLines(FichierGeoref, Encoding_FCGP);
            TrouverElementsSystemeCartographique(LignesFichierGeoref, ref SiteCarto, ref ClefEchelle, ref Projection, ref Z, ref H, ref Interpol);
            if (Interpol)
                return null;

            return new(SiteCarto, ClefEchelle, Projection, Z, H);
        }
        #endregion
    }
}