using static FCGP.Commun;
using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary>  sert pour le pasage de paramètres des fonctions ChargerImageFichier et ChargerImageEditableBitmap ''' </summary>
    internal class DonneesImage
    {
        /// <summary>initialise l'instance: structure de données qui sert de paramètre lors de la copie d'une image sur une autre </summary>
        /// <param name="TamponDonneesSource"></param>
        /// <param name="LargeurSource">largeur en pixel du tampon source</param>
        /// <param name="HauteurSource">hauteur en pixel du tampon source</param>
        /// <param name="LargeurTampon">largeur en pixel du tampon receveur</param>
        internal DonneesImage(object TamponDonneesSource, int LargeurSource, int HauteurSource, int LargeurTampon)
        {
            if (TamponDonneesSource is byte[] Bits)
            {
                ChargeImage = TypeImage.Memoire;
                TamponImageSource = Bits;
                CheminImageSource = null;
            }
            else if (TamponDonneesSource is string)
            {
                ChargeImage = TypeImage.Fichier;
                CheminImageSource = Convert.ToString(TamponDonneesSource);
                TamponImageSource = null;
            }
            else
            {
                ChargeImage = TypeImage.Aucun;
                return;
            }
            // par defaut il n'y a pas de décalage
            DecalXImageSource = 0;
            // par defaut il n'y a pas de décalage
            DecalYImageSource = 0;
            // hauteur de l'image en pixel
            HauteurImageSource = HauteurSource;
            // largeur de l'image en pixel
            LargeurImageSource = LargeurSource;
            // par defaut la hauteur à lire est la l'hauteur de l'image
            HauteurALire = HauteurSource;
            // par defaut la largeur à lire est la largeur de l'image
            LargeurALire = LargeurSource;
            // doit être un multipe de 4, appelé stride en anglais : nb d'octets pour passer à la ligne suivante de l'image
            NbOctetsLigneImageSource = StrideImage(LargeurSource);
            // doit être un multipe de 4, appelé stride en anglais, par défaut le même que la source mais ce n'est pas une obligation : nb d'octets pour passer à la ligne suivante de l'image
            NbOctectsLigneTampon = StrideImage(LargeurTampon);
        }
        /// <summary>tampon contenant l'image de la carte à lire</summary>
        internal byte[] TamponImageSource;
        /// <summary>index dans le tampon général où commence le tampon contenant l'image de la carte à lire</summary>
        internal int IndexTamponImageSource;
        /// <summary>flux contenant l'image de la carte à lire</summary>
        internal string CheminImageSource;
        /// <summary>largeur en pixel de l'image</summary>
        internal int LargeurImageSource;
        /// <summary>hauteur en pixel de l'image</summary>
        internal int HauteurImageSource;
        /// <summary>nb d'octets à lire pour passer à la ligne suivante dans le flux doit être un multiple de 4</summary>
        internal int NbOctetsLigneImageSource;
        /// <summary>nb de pixel à lire sur la ligne</summary>
        internal int LargeurALire;
        /// <summary>nb de ligne (pixel) à lire de l'image</summary>
        internal int HauteurALire;
        /// <summary>décalage en pixel en début de ligne</summary>
        internal int DecalXImageSource;
        /// <summary>décalage en ligne (pixel) en partant du haut de l'image</summary>
        internal int DecalYImageSource;
        /// <summary>sert pour l'écriture des données dans un SharedPinnedByteArray ou d'un BMPWriter
        /// différencier un donnéesImage Carte d'un DonneesImage Tampon</summary>
        internal TypeImage ChargeImage;
        /// <summary>nb d'octets à lire pour passer à la ligne suivante dans le tampon receveur doit être un multiple de 4
        /// sert principalement quand le SBA n'est qu'un réservoir d'octects (fichier)</summary>
        internal int NbOctectsLigneTampon;
    }
}