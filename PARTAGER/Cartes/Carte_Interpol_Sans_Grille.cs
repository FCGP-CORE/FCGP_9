using System.Text;

using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;

namespace FCGP
{
    internal partial class Carte
    {
        #region Interpole carte sans grille
        /// <summary> interpolation linéaire sur les latitudes pour les cartes WebMercator </summary>
        private ErreurInterpolation InterpolerCarteSansGrille(Carte CarteInterpole)
        {
            ErreurInterpolation InterpolerCarteSansGrilleRet = ErreurInterpolation.NotOk;
            NbTotalIterations = RegionVirtuelle.Height;
            NbIterations = 0;
            AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
            try
            {
                // initialisation des variables globales externes à boucle de calcul
                // calcul des latitudes de départ et de fin. On repart des coordonnées virtuelles de la carte source pour plus de précision. Elles sont communes aux cartes source et interpolée
                double z_LatitudeOrigine = SystemCartographique.ConvertirCoordonneesVirtuellesToReelles(RegionVirtuelle.Location).Y;
                double z_LatitudeFin = SystemCartographique.ConvertirCoordonneesVirtuellesToReelles(Point.Add(RegionVirtuelle.Location, RegionVirtuelle.Size)).Y;
                // pas des DD par pixel interpolé qui est forcément négatif
                double z_PasLatitudeInterpole = (z_LatitudeFin - z_LatitudeOrigine) / RegionVirtuelle.Height;
                // variables de parcours de la boucle ou des boucles
                var HauteurSourceLue = default(int);
                var HauteurInterpoleEcrite = default(int);
                int NbLignesInterpole;
                if (TamponType == TypeSupportCarte.Fichier)
                {
                    using (var FichierSource = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                    {
                        CarteInterpole.CheminFluxLecture = CheminEnregistrementProvisoire + @"\CarteInterpole.raw";
                        using (var FichierInterpole = new FileStream(CarteInterpole.CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            do
                            {
                                // calcul des variables de parcours de la boucle
                                NbLignesInterpole = CalculerNbLignesInterpole(z_LatitudeOrigine, z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue);
                                int NbLignesSource = CalculerNbLignesSource(z_LatitudeOrigine, z_PasLatitudeInterpole, HauteurInterpoleEcrite + NbLignesInterpole - 1, HauteurSourceLue);
                                // remplir le tampon source avec les données de la carte source à partir de la position du pointeur de lecture du fichier source qui correspond à HauteurSourceLue
                                FichierSource.Read(Tampon.Bits, Tampon.IndexBits, NbLignesSource * LargeurOctets);
                                // on interpole la partie de la carte correspondant à la boucle encours de HauteurInterpoleEcrite à HauteurInterpoleEcrite + NbLignesInterpole - 1
#if DEBUG && !PARALLEL
                                for (int LigneInterpole = 0, loopTo = NbLignesInterpole - 1; LigneInterpole <= loopTo; LigneInterpole++) // Boucle sans éxecution en parallele
                                    InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine, 
                                                                   z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue);
#else
                                Parallel.For(0, NbLignesInterpole, (LigneInterpole) =>
                                                                        InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine,
                                                                                                       z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue));
#endif
                                // on enregistre la partie de la carte interpolée
                                FichierInterpole.Write(CarteInterpole.Tampon.Bits, CarteInterpole.Tampon.IndexBits, NbLignesInterpole * LargeurOctets);
                                // on met à jour la variable correspondante
                                HauteurInterpoleEcrite += NbLignesInterpole;
                                // vérification nb de lignes effectivement lues qui peut être faussé par la précision des nb doubles avec positionnement du pointeur de lecture du fichier source
                                HauteurSourceLue = VerifierHauteurSource(z_LatitudeOrigine, z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue + NbLignesSource - 1, FichierSource);
                            }
                            while (HauteurInterpoleEcrite < RegionVirtuelle.Height);
                        }
                    }
                }
                else
                {
                    NbLignesInterpole = RegionVirtuelle.Height;
#if DEBUG && !PARALLEL
                    for (int LigneInterpole = 0, loopTo1 = NbLignesInterpole - 1; LigneInterpole <= loopTo1; LigneInterpole++) // Boucle sans éxecution en parallele
                        InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine, 
                                                       z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue);
#else
                    Parallel.For(0, NbLignesInterpole, (LigneInterpole) =>
                                                            InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine, // Boucle avec éxecution en parallele
                                                                                           z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue));
#endif
                }
                InterpolerCarteSansGrilleRet = ErreurInterpolation.Ok;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "P8A8");
            }

            return InterpolerCarteSansGrilleRet;
        }
        /// <summary> calcul le nb maxi de lignes à interpoler en fonction de la hauteur du tampon des cartes par pas de 2.5% de la hauteur du tampon </summary>
        /// <param name="z_LatitudeOrigine"> Latitude de départ de la carte </param>
        /// <param name="z_PasLatitudeInterpole"> le pas de latitude par pixels de la carte interpolée </param>
        /// <param name="HauteurInterpoleEcrite"> la hauteur déjà écrite de la carte interpolée </param>
        /// <param name="HauteurSourceLue"> la hauteur lue de la carte source correspondant à la carte déjà interpolée </param>
        private int CalculerNbLignesInterpole(double z_LatitudeOrigine, double z_PasLatitudeInterpole, int HauteurInterpoleEcrite, int HauteurSourceLue)
        {
            int CalculerNbLignesInterpoleRet;
            if (HauteurReserve > HauteurPixel)
            {
                // 'si le tampon est supèrieur à la carte source il n'y a qu'une seule tranche.
                // L'égalité provoque une erreur sur le Nb de lignes de la carte source à lire quand la hauteur de carte = la hauteur d'une tuile fichier
                // Exemple 4*256 tuile serveur et 1024 tuile KMZ ==> CalculerNbLignesInterpole = 1024, NbLignesSource = 1025
                CalculerNbLignesInterpoleRet = RegionVirtuelle.Height;
            }
            else
            {
                // initialisation des variables avant la boucle de calcul
                int DeltaTampon = (int)Math.Round(HauteurReserve * 0.025d);
                double LatitudeDebutTranche = z_LatitudeOrigine + HauteurInterpoleEcrite * z_PasLatitudeInterpole;
                int NbLignesInterpole = HauteurReserve + DeltaTampon; // on ajoute 2.5% en initialisation avant la boucle
                int HauteurSource;
                do
                {
                    NbLignesInterpole -= DeltaTampon; // à chaque tour on diminue le nb de lignes interpolées de 2.5% de la hauteur du tampon
                    double LatitudeFinTranche = LatitudeDebutTranche + NbLignesInterpole * z_PasLatitudeInterpole;
                    HauteurSource = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(new PointD(0d, LatitudeFinTranche)).Y - RegionVirtuelle.Y - HauteurSourceLue;
                }
                while (HauteurSource >= HauteurReserve);
                if (HauteurInterpoleEcrite + NbLignesInterpole < RegionVirtuelle.Height)
                {
                    CalculerNbLignesInterpoleRet = NbLignesInterpole;
                }
                else
                {
                    CalculerNbLignesInterpoleRet = RegionVirtuelle.Height - HauteurInterpoleEcrite;
                }
            }

            return CalculerNbLignesInterpoleRet;
        }
        /// <summary> calcul le nb de lignes de la carte source nécessaires pour l'interpolation du nb de lignes à interpoler 
        /// part du principe que la latitude de HauteurSourceLue est supérieur à la latitude de LatitudeDébutTranche de la carte interpolée </summary>
        /// <param name="z_LatitudeOrigine"> Latitude de départ de la carte </param>
        /// <param name="z_PasLatitudeInterpole"> le pas de ltitude par pixels de la carte interpolée </param>
        /// <param name="HauteurInterpole"> la hauteur déjà écrite de la carte interpolée + nombre de ligne à interpoler de la tranche suivante -1 </param>
        /// <param name="HauteurSourceLue"> la hauteur lue de la carte source correspondant à la carte déjà interpolée </param>
        private int CalculerNbLignesSource(double z_LatitudeOrigine, double z_PasLatitudeInterpole, int HauteurInterpole, int HauteurSourceLue)
        {
            double LatitudeFinTranche = z_LatitudeOrigine + HauteurInterpole * z_PasLatitudeInterpole;
            int CalculerNbLignesSourceRet = (int)Math.Floor(LatitudeWebMercatorToPixel(LatitudeFinTranche, SystemCartographique.SiteCarto, SystemCartographique.Niveau.Echelle)) + 2 - RegionVirtuelle.Y - HauteurSourceLue;
            return CalculerNbLignesSourceRet;
        }
        /// <summary> Verifie que la latitude de HauteurSource est > à la latitude de HauteurInterpoleEcrite de la carte interpolée 
        /// Dans tous les cas corrige le pointeur de lecture du fichier source soit de 1 ligne en arrière soit de 2 lignes </summary>
        /// <param name="z_LatitudeOrigine">latitude de départ pour l'interpolation. Commun carte source, carte interpolée</param>
        /// <param name="z_PasLatitudeInterpole">pas ou au coef moyen DD par pixels interpolé</param>
        /// <param name="HauteurSource"> la hauteur lue de la carte source correspondant à la carte déjà interpolée plus le nb de ligne à lire de la tranche en cours - 1 </param>
        /// <param name="HauteurInterpoleEcrite">nb de ligne qui ont déjà été écrite dans le fichier interpolé</param>
        /// <param name="FichierSource"> fichier source </param>
        private int VerifierHauteurSource(double z_LatitudeOrigine, double z_PasLatitudeInterpole, int HauteurInterpoleEcrite, int HauteurSource, FileStream FichierSource)
        {
            int VerifierHauteurSourceRet;
            double LatitudeDebutTrancheInterpole = z_LatitudeOrigine + HauteurInterpoleEcrite * z_PasLatitudeInterpole;
            double LatitudeDebutTrancheSource = SystemCartographique.ConvertirCoordonneesVirtuellesToReelles(new Point(0, HauteurSource + RegionVirtuelle.Y)).Y;
            if (LatitudeDebutTrancheSource < LatitudeDebutTrancheInterpole)
            {
                VerifierHauteurSourceRet = HauteurSource - 1;
                // pour tenir compte que l'on recule d'une ligne en plus on se positionne au début de l'avant dernière ligne
                FichierSource.Position -= LargeurOctets * 2;
            }
            else
            {
                VerifierHauteurSourceRet = HauteurSource;
                // pour tenir compte de la dernière ligne car elle est obligatoire pour l'interpolation de la tranche suivante on se positionne au début de la dernière ligne
                FichierSource.Position -= LargeurOctets;
            }

            return VerifierHauteurSourceRet;
        }
        /// <summary> interpole une ligne de la carte la carte inteoplée à partir de 2 lignes de la carte source </summary>
        /// <param name="TamponInterpole"> Tampon de la Carte à interpoler où sont enregistrés les résultats de l'interpolation </param>
        /// <param name="CptLigneInterpole"> numéro de ligne à interpoler (de 0 à hauteurpixel-1 ou 0 à nblignesinterpole-1 si il y a plus d'une tranche </param>
        /// <param name="z_LatitudeInterpole">latitude du coin(0) en DD</param>
        /// <param name="z_PasLatitudeInterpole">pas de la latitude pour 1 pixel de la carte interpolée (en DD)</param>
        /// <param name="HauteurInterpoleEcrite"> combien de lignes de la carte interpolée ont déja été traitées et écrites </param>
        /// <param name="HauteurSourceLue"> combien de lignes de la carte source ont déja été lue</param>
        private void InterpolerLigneCarteSansGrille(EditableBitmap TamponInterpole, int CptLigneInterpole, double z_LatitudeInterpole, double z_PasLatitudeInterpole, int HauteurInterpoleEcrite, int HauteurSourceLue)
        {
            // adresse du 1er pixel dans le tampon de la carte interpolée
            int IndexInterpole = CptLigneInterpole * LargeurOctets + TamponInterpole.IndexBits;
            // il faut trouver la ligne du pixel de la latitude source en coordonnées du monde virtuel correspondant à la latitude interpolée en cours. Pour cela on calcule la latitude interpolée
            int LigneInterpole = HauteurInterpoleEcrite + CptLigneInterpole;
            double EcartLatitudeInterpole = LigneInterpole * z_PasLatitudeInterpole;
            // on trouve le pixel avec la fonction qui va bien mais on le ramène au début du tampon qui contient tout ou partie de la carte source
            double PoidsL2;
            int YSource;
            if (LigneInterpole == 0)
            {
                YSource = 0;
                PoidsL2 = 0d; // pour la 1ere ligne de la carte interpolée le poids du pixel de la ligne YSource+1 est null
            }
            else
            {
                PoidsL2 = LatitudeWebMercatorToPixel(z_LatitudeInterpole + EcartLatitudeInterpole, SystemCartographique.SiteCarto, SystemCartographique.Niveau.Echelle) - RegionVirtuelle.Y;
                YSource = (int)Math.Floor(PoidsL2);
                if (LigneInterpole == RegionVirtuelle.Height - 1)
                {
                    PoidsL2 = 0d; // pour la derniere ligne de la carte interpolée le poids du pixel de la ligne YSource+1 est null
                }
                else
                {
                    PoidsL2 -= YSource;
                } // poids du pixel ligne YSource+1
            }
            int IndexSourceL1 = (YSource - HauteurSourceLue) * LargeurOctets + Tampon.IndexBits;
            int IndexSourceL2 = IndexSourceL1 + LargeurOctets;
            double L1, L2;
            for (int CptX = 0, loopTo = LargeurOctets - 1; CptX <= loopTo; CptX++)
            {
                {
                    // de la carte source
                    // interpolation de la couleur. à ce niveau on ne regarde que les bytes(8 bits par couleur)
                    if (Tampon.IsBitmap) // si la carte vient d'une image enregistrée il faut les trouver à partir de la mémoire non managée d'où l'utilisation de Marshal et des pointeurs
                    {
                        if (PoidsL2 == 0d)
                        {
                            TamponInterpole.Bits[IndexInterpole] = Marshal.ReadByte(Tampon.BitPtr, IndexSourceL1);
                        }
                        else
                        {
                            L1 = Marshal.ReadByte(Tampon.BitPtr, IndexSourceL1);
                            L2 = Marshal.ReadByte(Tampon.BitPtr, IndexSourceL2);
                            TamponInterpole.Bits[IndexInterpole] = (byte)Math.Round(L1 + (L2 - L1) * PoidsL2);
                        }
                    }
                    else if (PoidsL2 == 0d)
                    {
                        TamponInterpole.Bits[IndexInterpole] = Tampon.Bits[IndexSourceL1];
                    }
                    else
                    {
                        L1 = Tampon.Bits[IndexSourceL1];
                        L2 = Tampon.Bits[IndexSourceL2];
                        TamponInterpole.Bits[IndexInterpole] = (byte)Math.Round(L1 + (L2 - L1) * PoidsL2);
                    }
                }
                IndexSourceL1 += 1;
                IndexSourceL2 += 1;
                IndexInterpole += 1;
            }
            Interlocked.Increment(ref NbIterations);
            if (NbIterations % 100 == 0)
                AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
        }
        #endregion
    }
}