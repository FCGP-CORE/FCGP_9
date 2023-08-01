using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary>permet de créer un bitmap à partir d'un tableau de byte. cela permet d'accéder aux données bitmap à partir du tableau et vice versa. 
    /// Les 2 sont manipulables en même temps. Pour le bitmap seul le format RVB sur 24 bits (8 bits ou 1 octet par couleur) </summary>
    internal class EditableBitmap : IDisposable
    {
        /// <summary>tampon à adresse fixe en mémoire associé au bitmap.</summary>
        private SharedPinnedByteArray byteArray;
        /// <summary>flag indiquant si bitmap a été libéré</summary>
        private bool m_disposed;
        /// <summary> flag indiquant si l'EditableBitmap est correctement créé : Le tableau de byte est OK et le bitMap associé aussi </summary>
        internal bool IsOk { get; private set; }
        /// <summary>flag indiquant si bitmap a été libéré</summary>
        internal bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }
        /// <summary>nb d'octect pour lire une ligne du bitmap, forcément un multiple de 4</summary>
        internal int Stride { get; private set; }
        /// <summary>indique le bitmap est associé à un shared bitArray ou à un BitmapData</summary>
        internal bool IsBitmap { get; private set; }
        /// <summary>bitmap associé au tampon</summary>
        private Bitmap m_bitmap;
        /// <summary>bitmap associé au tampon</summary>
        internal Bitmap Bitmap
        {
            get
            {
                return m_bitmap;
            }
        }
        private readonly BitmapData BmpData;
        /// <summary>tableau d'octets associé au tampon</summary>
        internal byte[] Bits
        {
            get
            {
                if (IsBitmap)
                {
                    return null;
                }
                else
                {
                    return byteArray.Bits;
                }
            }
        }
        /// <summary>Nombre d'octets du tableau d'octets associé au tampon</summary>
        internal int NbBits
        {
            get
            {
                if (IsBitmap)
                {
                    return 0;
                }
                else
                {
                    return byteArray.NbBytes;
                }
            }
        }
        /// <summary>Index de départ dans le tableau d'octets associé au tampon</summary>
        internal int IndexBits
        {
            get
            {
                if (IsBitmap)
                {
                    return 0;
                }
                else
                {
                    return byteArray.IndexByte;
                }
            }
        }
        /// <summary>pointeur sur le tampon</summary>
        internal IntPtr BitPtr
        {
            get
            {
                if (IsBitmap)
                {
                    return BmpData.Scan0;
                }
                else
                {
                    return byteArray.BitPtr;
                }
            }
        }
        /// <summary>pointeur sur le tampon</summary>
        internal DonneesImage DonneesChargementImage
        {
            get
            {
                return byteArray.EcritureDonnees;
            }
        }
        /// <summary>pointeur sur le pixel en cours de traitement</summary>
        internal int IndexPixel { get; set; }
        /// <summary>pointeur sur la ligne en cours de traitement</summary>
        internal int IndexLigne { get; set; }
        /// <summary> initialise la couleur de l'image avec la couleur passée en paramètre </summary>
        internal void ClearColor(Color Couleur)
        {
            byteArray.ClearColor(Couleur);
        }
        /// <summary> Creation d'un nouveau editable bitmap vierge avec les largeur, hauteur</summary>
        /// <param name="width">largeur en pixel</param>
        /// <param name="height">hauteur en pixel</param>
        /// <remarks>sert pour la creation dynamique ou static d'un editablebitmap</remarks>
        internal EditableBitmap(int width, int height, SharedPinnedByteArray BitsShared, string Nom)
        {
            // on transforme les pixels en nb d'octects à écrire dans le tampon  
            Stride = StrideImage(width);
            byteArray = new SharedPinnedByteArray(Stride, height, BitsShared, Nom);
            if (byteArray.IsOk)
            {
                try // partie posant problème au niveau new d'un editablebitmap
                {
                    // le tampon a pu être créé, mais il faut aussi associé un bitmap au tampon
                    m_bitmap = new Bitmap(width, height, Stride, FormatPixel, byteArray.BitPtr);
                    IsBitmap = false;
                    IsOk = true;
                }
                catch (Exception Ex)
                {
                    m_bitmap = null;
                    byteArray.Dispose();
                    AfficherErreur(Ex, "R4H7");
                }
            }
        }
        /// <summary> créer un editablebitmap à partir d'une image existante</summary>
        /// <param name="CheminImage"></param>
        internal EditableBitmap(string CheminImage)
        {
            try // partie posant problème au niveau new d'un editablebitmap
            {
                // décompresse l'image de la carte en mémoire
                if (!File.Exists(CheminImage))
                    throw new Exception("Le fichier image n'existe pas");
                m_bitmap = new Bitmap(CheminImage);
                // on verrouille les données pour y avoir acces
                var rect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);
                BmpData = m_bitmap.LockBits(rect, ImageLockMode.ReadWrite, m_bitmap.PixelFormat);
                Stride = BmpData.Stride;
                IsBitmap = true;
                IsOk = true;
            }
            catch (Exception Ex)
            {
                m_bitmap = null;
                MessageInformation = "Erreur dans Sub New EditableBitmap";
                AfficherErreur(Ex, "E4P7");
            }
        }
        #region Complément à Editablebitmap original
        /// <summary>charge une image dans le bitmap. Les dimensions du bitmap et de l'image doivent être identiques</summary>
        /// <param name="CheminImage">image à charger</param>
        internal void ChargerImage(string CheminImage)
        {
            IsOk = false;
            try
            {
                // décompresse l'image de la carte en mémoire
                using (var B = new Bitmap(CheminImage))
                {
                    // on verrouille les données pour y avoir acces
                    var rect = new Rectangle(0, 0, B.Width, B.Height);
                    var bmpData = B.LockBits(rect, ImageLockMode.ReadOnly, B.PixelFormat);
                    // on copie les données dans notre tampon
                    Marshal.Copy(bmpData.Scan0, Bits, 0, Bits.Length);
                    // on libère les ressources non managées.
                    B.UnlockBits(bmpData);
                }
                IsOk = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "Y9U2");
            }
        }
        /// <summary>initialise les différents paramètres qui permettront l'écriture des données graphiques dans le tableau de byte</summary>
        /// <param name="TamponDonneesSource"></param>
        /// <param name="LargeurSource">Largeur en pixel de l'image source</param>
        /// <param name="HauteurSource">Hauteur en pixel de l'iamge source</param>
        /// <param name="LargeurTampon">Largeur en pixel du tampon receveur</param>
        /// <returns></returns>
        internal ErreurEditableBitmap InitialiserChargementImage(object TamponDonneesSource, int LargeurSource, int HauteurSource, int LargeurTampon)
        {
            byteArray.InitialiserEcritureDonnees(TamponDonneesSource, LargeurSource, HauteurSource, LargeurTampon);
            if (byteArray.EcritureDonnees is null) // If IsNothing(byteArray.EcritureDonnees) Then
            {
                return ErreurEditableBitmap.Tampon_Donnees_Incompatible;
            }
            else
            {
                return ErreurEditableBitmap.Ok;
            }
        }
        /// <summary>
        /// Charge une image physique dans le tampon mémoire sans blocage de la ressource image.Permet à travers 
        /// la strucuture CarteImage d'échanger les diverses infos nécessaires au bon fonctionnement
        /// </summary>
        /// <param name="DecalTamponX">decalage X en pixel de l'image dans le tampon receveur</param>
        /// <param name="DecalTamponY">decalage Y en pixel de l'image dans le tampon receveur</param>
        /// <remarks>les vérifications effectuées return false si erreur</remarks>
        internal ErreurEditableBitmap ChargerImage(int DecalTamponX = 0, int DecalTamponY = 0)
        {
            if (DonneesChargementImage is null) // If IsNothing(DonneesChargementImage) Then
            {
                return ErreurEditableBitmap.Donnees_Image_Non_Renseignees;
            }
            {
                var withBlock = DonneesChargementImage;
                if (withBlock.ChargeImage == TypeImage.Aucun)
                {
                    return ErreurEditableBitmap.ChargeCarte_Non_Renseigne;
                }
                if (withBlock.ChargeImage == TypeImage.Fichier & string.IsNullOrEmpty(withBlock.CheminImageSource))
                {
                    return ErreurEditableBitmap.CheminCarte_Non_Renseigne;
                }
                if (withBlock.ChargeImage == TypeImage.Memoire & withBlock.TamponImageSource is null) // If .ChargeImage = TypeImage.Memoire And IsNothing(.TamponImageSource) Then
                {
                    return ErreurEditableBitmap.TamponSource_Non_Renseigne;
                }
                if (DecalTamponX < 0 || DecalTamponX > Bitmap.Width - 1)
                {
                    return ErreurEditableBitmap.DecalTamponX_incompatible_tampon;
                }
                if (DecalTamponX + withBlock.LargeurALire > Bitmap.Width)
                {
                    return ErreurEditableBitmap.DecalTamponX_LargeurALire_incompatible_tampon;
                }
                if (DecalTamponY < 0 | DecalTamponY > Bitmap.Height - 1)
                {
                    return ErreurEditableBitmap.DecalTamponY_incompatible_tampon;
                }
                if (DecalTamponY + withBlock.HauteurALire > Bitmap.Height)
                {
                    return ErreurEditableBitmap.DecalTamponY_HauteurALire_incompatible_tampon;
                }
                if (withBlock.DecalXImageSource + withBlock.LargeurALire > withBlock.LargeurImageSource)
                {
                    return ErreurEditableBitmap.DecalCarteX_LargeurALire_incompatible_Image;
                }
                if (withBlock.DecalYImageSource + withBlock.HauteurALire > withBlock.HauteurImageSource)
                {
                    return ErreurEditableBitmap.DecalCarteY_HauteurALire_incompatible_Image;
                }
            }
            // on copie les données sous forme d'octets
            if (byteArray.EcrireDonnees(DecalTamponX, DecalTamponY))
            {
                return ErreurEditableBitmap.Ok;
            }
            else
            {
                return ErreurEditableBitmap.Erreur_Fonction;
            }
        }
        #endregion
        /// <summary> permet de libérer les ressources associées à l'EditableBitmap </summary>
        public void Dispose()
        {
            Dispose(true);
            // nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(this);
        }
        /// <summary>permet de libérer les ressources associées à l'EditableBitmap</summary>
        private void Dispose(bool disposing)
        {
            // si l'objet est déja déposé onsort
            if (m_disposed)
                return;
            // sinon on libère le bitmap si demandé 'sinon on libère le bitmap si demandé
            if (IsBitmap)
            {
                m_bitmap.UnlockBits(BmpData);
            }
            else
            {
                // si le dimensionnement du tampon est dynamique on récupère la mémoire
                byteArray.Dispose();
                byteArray = null;
            }
            m_disposed = true;
            if (disposing)
            {
                m_bitmap?.Dispose(); // If Not IsNothing(m_bitmap) Then m_bitmap.Dispose()
                m_bitmap = null;
            }
        }
        /// <summary>appeler par le GC si l'objet n'a plus d'attache</summary>
        ~EditableBitmap()
        {
            try
            {
                Dispose(false);
            }
            finally
            {
            }
        }
    }
}