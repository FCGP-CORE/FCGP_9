using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;
using static FCGP.NativeMethods;

namespace FCGP
{
    /// <summary> definit un tableau d'octets avec une adresse mémoire fixe ce qui empêche le garbage collecteur de collecter ou de bouger l'emplacement du tableau </summary>
    /// <remarks>il y a obligation d'appeler dispose pour libérer la mémoire </remarks>
    internal class SharedPinnedByteArray : IDisposable
    {
        /// <summary> tableau de byte servant de tampon mémoire </summary>
        internal byte[] Bits { get; private set; }
        /// <summary> nombre d'octets constituant le tampon mémoire </summary>
        internal int NbBytes
        {
            get
            {
                return NbOctetsLigne * NbLignes;
            }
        }
        /// <summary> index de base dans le tableau constituant le tampon mémoire </summary>
        internal int IndexByte { get; private set; }
        /// <summary> index de fin dans le tableau constituant le tampon mémoire </summary>
        internal int IndexBytesDisponible { get; set; }
        /// <summary> addresse du tampon mémoire </summary>
        internal IntPtr BitPtr { get; private set; }
        /// <summary> nb de ligne indiqué à la création du tampon </summary>
        internal int NbLignes { get; private set; }
        /// <summary> nb d'octets par ligne indiqué à la création du tampon </summary>
        internal int NbOctetsLigne { get; private set; }
        /// <summary> indique si il reste des octets disponible par rapport à la réserve du départ </summary>
        internal bool IsFull
        {
            get
            {
                return IndexBytesDisponible >= IndexByte + NbOctetsLigne * NbLignes;
            }
        }
        /// <summary> indique le nombres d'octets disponible par rapport à la réserve du départ </summary>
        internal int ResteDisponible
        {
            get
            {
                return IndexByte + NbOctetsLigne * NbLignes - IndexBytesDisponible;
            }
        }
        /// <summary> flag indiquant que le création du sharedPinned c'est bien passé</summary>
        internal bool IsOk { get; private set; }
        /// <summary> Nom du tampon. Sert à distinguer les sous tampons d'un tampon géneral</summary>
        internal string Nom { get; private set; }
        /// <summary> Crée un tableau d'octects ayant pour taille soit  NbOctetsParLigne-1 si NbdeLignes=1 ou
        /// NbOctetsParLigne*NbdeLignes-1 si NbDeLignes>1. constructeur pour un tableau bloqué original </summary>
        /// <param name="NbDeLignes"></param>
        /// <param name="NbOctetsParLigne"> nombre d'octects par ligne. Si ligne = 1 cela équivaut à réserver une quantité de mémoire déconnectée de la taille réelle d'une image </param>
        /// <param name="BitsShared"> tampon mémoire que l'on veut utilisé sinon il sera créé </param>
        internal SharedPinnedByteArray(int NbOctetsParLigne, int NbDeLignes = 1, SharedPinnedByteArray BitsShared = null, string NomTampon = "Carte")
        {
            try
            {
                Nom = NomTampon;
                NbLignes = NbDeLignes;
                NbOctetsLigne = NbOctetsParLigne;
                if (BitsShared is null) // i faut créer un nouveau tampon
                {
                    Bits = new byte[NbBytes];
                    // if not pinned the GC can move around the array
                    handle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
                    BitPtr = Marshal.UnsafeAddrOfPinnedArrayElement(Bits, 0);
                    IsOk = true;
                }
                else if (BitsShared.ResteDisponible >= NbBytes) // on se sert du tampon existant si il reste assez de place
                {
                    BaseTampon = BitsShared;
                    Bits = BitsShared.Bits;
                    IndexByte = BitsShared.IndexBytesDisponible;
                    BitPtr = BitsShared.BitPtr + BitsShared.IndexBytesDisponible - BitsShared.IndexByte;
                    BitsShared.IndexBytesDisponible += NbBytes;
                    IndexBytesDisponible = IndexByte;
                    IsOk = true;
                }
                else
                {
                    throw new Exception("Le tampon support n'a pas la capacité suffisante");
                }
            }
            catch (Exception Ex)
            {
                Bits = null;
                handle = default;
                BitPtr = IntPtr.Zero;
                AfficherErreur(Ex, "R4F4");
            }
        }
        /// <summary> remplit le tampon avec la valeur indiquée </summary>
        /// <param name="Valeur"> valeur pour remplir le tampon </param>
        internal void ClearValeur(byte Valeur)
        {
            InitialiserMemoire(BitPtr, (int)Valeur, NbOctetsLigne * NbLignes);
        }
        /// <summary> remplit une partie du tampon avec la valeur indiquée </summary>
        /// <param name="Valeur"> valeur pour remplir le tampon </param>
        internal void ClearValeur(byte Valeur, int IndexDecalage, int NbOctets)
        {
            InitialiserMemoire(BitPtr + IndexDecalage, (int)Valeur, NbOctets);
        }
        /// <summary> remplit le tampon avec la couleur indiquée </summary>
        /// <param name="Couleur"> couleur pour remplir le tampon</param>
        internal void ClearColor(Color Couleur)
        {
            if (Couleur.B == Couleur.G && Couleur.B == Couleur.R)
            {
                InitialiserMemoire(BitPtr, (int)Couleur.B, NbOctetsLigne * NbLignes);
            }
            else
            {
                // remplir les 3 1ers octets (1er pixel) de la 1ère ligne avec la couleur
                Bits[IndexByte + 0] = Couleur.B;
                Bits[IndexByte + 1] = Couleur.G;
                Bits[IndexByte + 2] = Couleur.R;
                // remplir le reste des pixels de la 1ère ligne avec la couleur désirée
                int index = 3;
                while (index <= NbOctetsLigne / 2)
                {
                    CopierMemoire(BitPtr + index, BitPtr, index);
                    index *= 2;
                }
                CopierMemoire(BitPtr + index, BitPtr, NbOctetsLigne - index);
                // remplir les autres lignes avec la 1ère ligne
                Parallel.For(1, NbLignes, Cpt => CopierMemoire(BitPtr + Cpt * NbOctetsLigne, BitPtr, NbOctetsLigne));
            }
        }
        /// <summary> remplit une partie du tampon avec la couleur indiquée </summary>
        /// <param name="Couleur"> couleur pour remplir le tampon</param>
        internal void ClearColor(Color Couleur, int IndexDecalage, int NbOctets, int Hauteur)
        {
            var PointBase = BitPtr + IndexDecalage;
            if (Couleur.B == Couleur.G && Couleur.B == Couleur.R)
            {
                // remplir les lignes avec la valeur
                Parallel.For(0, Hauteur, Cpt => InitialiserMemoire(PointBase + Cpt * NbOctetsLigne, (int)Couleur.B, NbOctets));
            }
            else
            {
                // remplir les 3 1ers octets (1er pixel) de la 1ère ligne avec la couleur
                Bits[IndexByte + IndexDecalage + 0] = Couleur.B;
                Bits[IndexByte + IndexDecalage + 1] = Couleur.G;
                Bits[IndexByte + IndexDecalage + 2] = Couleur.R;
                // remplir le reste des pixels de la 1ère ligne avec la couleur désirée
                int index = 3;
                while (index <= NbOctets / 2)
                {
                    CopierMemoire(PointBase + index, PointBase, index);
                    index *= 2;
                }
                CopierMemoire(PointBase + index, PointBase, NbOctets - index);
                // remplir les autres lignes avec la 1ère ligne
                Parallel.For(1, Hauteur, Cpt => CopierMemoire(PointBase + Cpt * NbOctetsLigne, PointBase, NbOctets));
            }
        }
        /// <summary> si le SPBA est vu comme recepteur, ce champ permet l'échange de données avec l'émmetteur </summary>
        internal DonneesImage EcritureDonnees;
        /// <summary> référence au tampon utilisé pour ce SPBA </summary>
        private readonly SharedPinnedByteArray BaseTampon;
        /// <summary> permet que le GC ne modifie pas l'adresse du tampon en mémoire </summary>
        private GCHandle handle;
        /// <summary> initialise la structure de donnnées (paramètres) qui permettent l'écriture de données d'un tampon dans le fichier </summary>
        /// <param name="DonneesTamponSource">source de données qui contient les données à lire fichier</param>
        /// <param name="LargeurTamponReceveur">largeur du tampon receveur en pixel</param>
        /// <param name="HauteurSource">hauteur du tampon source en pixel</param>
        /// <param name="LargeurSource">largeur du tampon source en pixel</param>
        internal void InitialiserEcritureDonnees(object DonneesTamponSource, int LargeurSource, int HauteurSource, int LargeurTamponReceveur)
        {
            EcritureDonnees = new DonneesImage(DonneesTamponSource, LargeurSource, HauteurSource, LargeurTamponReceveur);
            if (EcritureDonnees.ChargeImage == TypeImage.Aucun)
                EcritureDonnees = null;
        }
        /// <summary> écrit les données d'un tampon mémoire ou fichier BMP dans un le sharedpinnedByteArray actuel</summary>
        internal bool EcrireDonnees(int DecalTamponX, int DecalTamponY)
        {
            if (EcritureDonnees.ChargeImage == TypeImage.Fichier)
            {
                if (EcritureDonnees.CheminImageSource.EndsWith("Bmp"))
                {
                    return LireOctetsFichierBMP(DecalTamponX, DecalTamponY);
                }
                else
                {
                    return LireOctetsFichierBin(DecalTamponX, DecalTamponY);
                }
            }
            else
            {
                return LireOctetsTampon(DecalTamponX, DecalTamponY);
            }
        }
        /// <summary> flag indiquant que le création  du sharedPinned c'est bien passé</summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        /// <summary> si true indique que l'objet a été déposé en attente d'un garbage collector </summary>
        private bool destroyed;
        /// <summary> restitue les ressources du sharedPinnedByteArray </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (destroyed)
                return;
            if (BaseTampon is null)
            {
                if (disposing)
                {
                    Bits = null;
                }
                handle.Free();
                GC.Collect();
            }
            else
            {
                BaseTampon.IndexBytesDisponible -= NbBytes;
            }
            destroyed = true;
        }
        /// <summary> restitue les ressources du sharedPinnedByteArray mais par l'intermédiaire du garbage collector</summary>
        ~SharedPinnedByteArray()
        {
            Dispose(false);
        }
        /// <summary> copie tout ou partie d'une image en mémoire dans un tampon mémoire receveur. </summary>
        /// <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon</param>
        /// <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon</param>
        /// <remarks>aucunes vérifications de fait dans cette fonction</remarks>
        private bool LireOctetsTampon(int DecalTamponX = 0, int DecalTamponY = 0)
        {
            bool LireOctetsTamponRet = false;
            try
            {
                {
                    ref var withBlock = ref EcritureDonnees;
                    // on calcule l'index à partir duquel seront copiés dans le tampon, les octets lus à partir du fichier. 
                    // Attention inversion des N° des lignes de pixel dans le fichier BMP (du bas en haut) en mémoire de haut en bas
                    int IndexTamponReceveur = IndexByte + DecalTamponX * NbOctetsPixel + DecalTamponY * withBlock.NbOctectsLigneTampon;
                    // on calcule le nb d'octets à copier à partir du tampon source par ligne
                    int NbOctectsCarteALire = withBlock.LargeurALire * NbOctetsPixel;
                    // index de départ du tampon source
                    int IndexTamponsource = withBlock.IndexTamponImageSource + withBlock.DecalXImageSource * NbOctetsPixel + withBlock.DecalYImageSource * withBlock.NbOctetsLigneImageSource;
                    for (int BoucleLigne = withBlock.DecalYImageSource, loopTo = withBlock.DecalYImageSource + withBlock.HauteurALire - 1; BoucleLigne <= loopTo; BoucleLigne++)
                    {
                        Buffer.BlockCopy(withBlock.TamponImageSource, IndexTamponsource, Bits, IndexTamponReceveur, NbOctectsCarteALire);
                        IndexTamponReceveur += withBlock.NbOctectsLigneTampon;
                        IndexTamponsource += withBlock.NbOctetsLigneImageSource;
                    }
                }
                LireOctetsTamponRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "Y4U4");
            }

            return LireOctetsTamponRet;
        }
        /// <summary> Copie tout ou partie d'un fichier image BMP dans un tampon mémoire receveur </summary>
        /// <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon</param>
        /// <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon</param>
        /// <remarks>les vérifications effectuées return false si erreur</remarks>
        private bool LireOctetsFichierBMP(int DecalTamponX = 0, int DecalTamponY = 0)
        {
            bool LireOctetsFichierBMPRet = false;
            try
            {
                {
                    ref var withBlock = ref EcritureDonnees;
                    using (var CarteStream = new FileStream(withBlock.CheminImageSource, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // on calcule l'index à partir duquel seront copiés dans le tampon, les octets lus à partir du fichier. 
                        // Attention inversion des N° des lignes de pixel dans le fichier BMP (du bas en haut) en mémoire de haut en bas
                        int IndexTamponReceveur = IndexByte + (withBlock.HauteurALire + DecalTamponY - 1) * withBlock.NbOctectsLigneTampon + DecalTamponX * NbOctetsPixel;
                        // on calcule le nb d'octets à copier du fichier dans le tampon par ligne
                        int NbOctectsCarteALire = withBlock.LargeurALire * NbOctetsPixel;
                        // Offset de décalage des bits de données. Toujours 54 pour les images BMP
                        int IndexCarteSource = OffsetDataBMP + withBlock.DecalXImageSource * NbOctetsPixel + (withBlock.HauteurImageSource - withBlock.DecalYImageSource - withBlock.HauteurALire) * withBlock.NbOctetsLigneImageSource;
                        for (int BoucleLigne = withBlock.DecalYImageSource, loopTo = withBlock.DecalYImageSource + withBlock.HauteurALire - 1; BoucleLigne <= loopTo; BoucleLigne++)
                        {
                            CarteStream.Position = IndexCarteSource;
                            CarteStream.Read(Bits, IndexTamponReceveur, NbOctectsCarteALire);
                            IndexTamponReceveur -= withBlock.NbOctectsLigneTampon;
                            IndexCarteSource += withBlock.NbOctetsLigneImageSource;
                        }
                    }
                }
                LireOctetsFichierBMPRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "S5M6");
            }

            return LireOctetsFichierBMPRet;
        }
        /// <summary> Copie tout ou partie d'un fichier bin dans un tampon mémoire receveur </summary>
        /// <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon</param>
        /// <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon</param>
        /// <remarks>les vérifications effectuées return false si erreur</remarks>
#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        private static bool LireOctetsFichierBin(int DecalTamponX = 0, int DecalTamponY = 0)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            bool LireOctetsFichierBinRet = false;
            return LireOctetsFichierBinRet;
        }
    }
}