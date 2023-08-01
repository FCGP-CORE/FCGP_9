using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    internal partial class Carte
    {
        #region Interpole Carte Avec Grille
        /// <summary> interpolation bilineaire avec correction de l'erreur sur les Y (carte qui ont déjà une grille) </summary>
        /// <remarks>la largeur et la hauteur en pixels de la carte interpolée sont les mêmes que celles de la carte source
        /// En coordonnées on prend le rectangle DD max de la carte source </remarks>
        private ErreurInterpolation InterpolerCarteAvecGrille(Carte CarteInterpole)
        {
            ErreurInterpolation InterpolerCarteAvecGrilleRet = default;
            InterpolerCarteAvecGrilleRet = ErreurInterpolation.NotOk;
            // 1 pixel de la couleur de carte par défaut
            PixelInterpol = new byte[] { PartagerSettings.COULEUR_CARTE.B, PartagerSettings.COULEUR_CARTE.G, PartagerSettings.COULEUR_CARTE.R };
            NbTotalIterations = HauteurPixel;
            NbIterations = 0;
            AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
            var z_ = InitialiserICAG(CarteInterpole); // structure spécifique pour éviter la transmission de tout un tas de paramètres
            if (z_ is null)
                return InterpolerCarteAvecGrilleRet; // si La structure est vide c'est qu'il y a un problème
                                                     // la hauteur du tampon source est au plus = à la hauteur de la carte -1 à cause de l'interpolation qui demande 2 lignes de pixels pour être réalisée
            int HauteurTamponSource = HauteurReserve >= CarteInterpole.HauteurPixel ? CarteInterpole.HauteurPixel - 1 : HauteurReserve - 1;
            var HauteurEcrite = default(int);
            int TailleTamponInterpole;
            int HauteurTamponInterpole;
            var DebutSource = default(int);
            int TailleTamponSource = LargeurOctets * (HauteurTamponSource + 1); // la taille du tampon est fixe . On charge 1 ligne en plus pour permettre l'interpolation
            try
            {
                if (CarteInterpole.TamponType == TypeSupportCarte.Fichier) // Il y aura normalement plusieurs tranches de travail
                {
                    using (var FichierBIN = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                    {
                        CarteInterpole.CheminFluxLecture = CheminEnregistrementProvisoire + @"\CarteInterpole.raw";
                        using (var FichierInterpole = new FileStream(CarteInterpole.CheminFluxLecture, FileMode.Create, FileAccess.Write))
                        {
                            do
                            {
                                HauteurTamponInterpole = TrouverHauteurInterpole(z_, HauteurTamponSource, FichierBIN) - HauteurEcrite;
                                TailleTamponInterpole = LargeurOctets * HauteurTamponInterpole;
                                FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTamponSource); // on charge une partie de la carte sans grille dans le tampon
                                                                                                    // on interpole une tranche de la carte

#if DEBUG && !PARALLEL
                                for (int CptY = 0, loopTo = HauteurTamponInterpole - 1; CptY <= loopTo; CptY++) //Boucle sans éxecution en parallele
                                    InterpolerLigneCarteAvecGrille(HauteurEcrite, CptY, DebutSource, z_);
#else

                                Parallel.For(0, HauteurTamponInterpole, (Cpty) => InterpolerLigneCarteAvecGrille(HauteurEcrite, Cpty, DebutSource, z_));

#endif
                                HauteurEcrite += HauteurTamponInterpole;
                                DebutSource = z_.DebutSource;
                                // on enregistre la partie de la carte interpolée
                                FichierInterpole.Write(CarteInterpole.Tampon.Bits, CarteInterpole.Tampon.IndexBits, TailleTamponInterpole);
                            }
                            while (HauteurEcrite < HauteurPixel);
                        }
                    }
                }
                else
                {
                    // interpolation bilinéaire de la carte : pour chaque ligne de pixels sur la hauteur de l'image interpolée,
                    // il faut trouver l'interpolation de la couleur des pixels composant la ligne

#if DEBUG && !PARALLEL
                    for (int CptY = 0, loopTo = HauteurPixel - 1; CptY <= loopTo; CptY++) //Boucle sans éxecution en parallele
                        InterpolerLigneCarteAvecGrille(HauteurEcrite, CptY, DebutSource, z_);
#else
                    Parallel.For(0, HauteurPixel, CptY => InterpolerLigneCarteAvecGrille(HauteurEcrite, CptY, DebutSource, z_));

#endif
                }
                InterpolerCarteAvecGrilleRet = ErreurInterpolation.Ok;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "S4E2");
            }

            return InterpolerCarteAvecGrilleRet;
        }
        /// <summary> calcule le N° de ligne interpolée et la latitude source minimale pour cette ligne interpolée</summary>
        /// <param name="z_"> structure de passage d'informatio n</param>
        /// <param name="HauteurTamponSource"> hauteur du tampon de la carte source en nb de lignes de pixel </param>
        /// <param name="FichierBin"> fichier qui contient les données binaire de la carte source </param>
        /// <returns> HauteurInterpole </returns>
        private int TrouverHauteurInterpole(ICAG z_, int HauteurTamponSource, FileStream FichierBin)
        {
            int HauteurSource = HauteurTamponSource + z_.DebutSource - 1; // correspond à la ligne de la latitude maxi de la tranche précédente
            FichierBin.Position = z_.DebutSource * (long)LargeurOctets; // on positionne le pointeur de fichier pour être prêt à la prochaine lecture
            z_.TrouverIndexLatMaxInterpole(HauteurSource); // une fois qu'on a trouvé l'index supérieur de la ligne interpolée
            double ValeurLat0 = z_.CoordY[z_.IndexLatMax, z_.IndexLigneInterpole - 1];
            double ValeurLat1 = z_.CoordY[z_.IndexLatMax, z_.IndexLigneInterpole];
            if (z_.DeltaLatMax > 0d)
            {
                ValeurLat0 += (z_.CoordY[z_.IndexLatMax + 1, z_.IndexLigneInterpole - 1] - ValeurLat0) * z_.DeltaLatMax;
                ValeurLat1 += (z_.CoordY[z_.IndexLatMax + 1, z_.IndexLigneInterpole] - ValeurLat1) * z_.DeltaLatMax;
            }
            // on peut calculer la ligne interpolée par interpolation des 2 latitudes qui encadrent hauteursource
            double LigneInterpoleFinTranche = (int)Math.Truncate(z_.IndexLigneInterpole * z_.PasY - z_.PasY * (ValeurLat1 - HauteurSource) / (ValeurLat1 - ValeurLat0));
            // mais on ne doit pas dépasser la hauteur de l'image
            if (LigneInterpoleFinTranche >= HauteurPixel)
                LigneInterpoleFinTranche = HauteurPixel - 1;
            double DeltaLigneInterpole = LigneInterpoleFinTranche / z_.PasY;
            DeltaLigneInterpole -= (int)Math.Truncate(DeltaLigneInterpole);
            // maintenant que l'on a la ligne interpolée on peut calculer la latitude mini correspondante pour le début de la prochaine tranche
            ValeurLat0 = z_.CoordY[z_.IndexLatMin, z_.IndexLigneInterpole - 1];
            ValeurLat1 = z_.CoordY[z_.IndexLatMin, z_.IndexLigneInterpole];
            if (z_.DeltaLatMin > 0d)
            {
                ValeurLat0 += (z_.CoordY[z_.IndexLatMin + 1, z_.IndexLigneInterpole - 1] - ValeurLat0) * z_.DeltaLatMin;
                ValeurLat1 += (z_.CoordY[z_.IndexLatMin + 1, z_.IndexLigneInterpole] - ValeurLat1) * z_.DeltaLatMin;
            }
            z_.DebutSource = (int)Math.Ceiling(ValeurLat0 + (ValeurLat1 - ValeurLat0) * DeltaLigneInterpole);
            return (int)Math.Round(LigneInterpoleFinTranche) + 1;
        }
        /// <summary> Calcul normaux pour l'interpolation des cartes avec grille. Facile à comprendre mais très gourmand en calculs donc en temps
        /// à cause de la fonction ConvertWGS84ToProjection qui est précise mais qui utilise des puissances de 2 ou 3 sur les latitudes et longitudes</summary>
        /// <param name="CarteInterpole">carte qui reçoit le résultat de l'interpolation</param>
        /// <param name="CptY">N° de ligne de travail de la carte interpolée</param>
        /// <param name="z_">structure contenant les paramètres de calculs</param>
        private ErreurInterpolation InterpolerLigneCarteAvecGrille_ConvertCoordonnees(Carte CarteInterpole, int DecalageYInterpole, int CptY, int DecalageYSource, ICAG z_)
        {
            int IndexPixelInterpol = CptY * CarteInterpole.LargeurOctets;
            var PointInterpole = CarteInterpole.Coins[0].LatLon;
            PointInterpole.Lat -= (DecalageYInterpole + CptY) * z_.InterDeltaLat;
            double EchelleMPixelX = Echelle_M_PixelX;
            double EchelleMPixelY = Echelle_M_PixelY;
            for (int Cptx = 0, loopTo = LargeurPixel - 1; Cptx <= loopTo; Cptx++)
            {
                // calcul des coordonnées métriques de la grille  
                var PointSource = ConvertWGS84ToProjection(PointInterpole, SystemCartographique.Projection.Datum);
                double XSource = (PointSource.X - Coins[0].X_Grid) / EchelleMPixelX; // transforme en pixel de la carte source
                double YSource = (Coins[0].Y_Grid - PointSource.Y) / EchelleMPixelY; // transforme en pixel de la carte source
                                                                                     // Trouver le pixel interpolé (interpolation linéaire sur 4 pixels : X et X+1, Y et Y+1) de la carte originale
                RemplirPixelsTampon(DecalageYSource, XSource, YSource, CarteInterpole.Tampon.Bits, IndexPixelInterpol);
                IndexPixelInterpol += 3;
                PointInterpole.Lon += z_.InterDeltaLong;
                NbIterations += 1;
                if (NbIterations % 100 == 0)
                    AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
            }
            return ErreurInterpolation.Ok;
        }
        /// <summary> procédure qui remplace la procedure InterpolerLigneCarteAvecGrille_ConvertCoordonnees
        /// plus difficile à comprendre car il s'agit d'une interpolation linéaire sur les X et sur les Y que l'on appelle bilinéaire avec correction d'erreur car l'interpolation ne considère
        /// que des droites alors qu'il s'agit de courbe. QualiteX permet d'ajuster le niveau de correction d'erreur (segment de droite) en fonction de l'échelle de la carte </summary>
        /// <param name="CptY">n° de ligne de travail de la carte interpolée</param>
        /// <param name="HauteurEcrite">decalage en nb de ligne du tampon par rapport à CptY. Sert quand il y a plusieurs tranches</param>
        /// <param name="HauteurSource">nb de ligne du debut du tampon source par rapport au début du fichier source. Sert quand il y a plusieurs tranches</param>
        /// <param name="z_">structures qui contient toutes les variables de calcul communes pour les boucles en parallele</param>
        private void InterpolerLigneCarteAvecGrille(int HauteurEcrite, int CptY, int HauteurSource, ICAG z_)
        {
            // préparation de la boucle sur les X de la carte interpolée. Les dimensions en pixels de la carteInterpolée sont les mêmes que celles de la carte Originale
            int IndexPixelInterpol = CptY * LargeurOctets + z_.IndexBits;
            double DeltaY_Carte = (CptY + HauteurEcrite) / (double)z_.PasY;
            int IndiceY_Grille = (int)Math.Floor(DeltaY_Carte);
            DeltaY_Carte -= IndiceY_Grille;
            int FinX = 0;
            int DebutX = 0;
            // pour chaque pas sur l'axe des X ou Segment de droite de l'interpolation
            for (int IndiceX_Grille = 0, loopTo = z_.NbPasX - 1; IndiceX_Grille <= loopTo; IndiceX_Grille++)
            {
                FinX += z_.PasX;
                if (FinX > LargeurPixel)
                {
                    FinX = LargeurPixel;
                }
                // on peut charger les valeurs exprimées en pixels de début et de fin de segment et le poids d'un pixel du segment exprimé en % 
                double X0 = z_.CoordX[IndiceX_Grille, IndiceY_Grille];
                double DeltaX0 = (z_.CoordX[IndiceX_Grille + 1, IndiceY_Grille] - X0) / z_.PasX;
                double X1 = z_.CoordX[IndiceX_Grille, IndiceY_Grille + 1];
                double DeltaX1 = (z_.CoordX[IndiceX_Grille + 1, IndiceY_Grille + 1] - X1) / z_.PasX;
                double Y0 = z_.CoordY[IndiceX_Grille, IndiceY_Grille];
                double DeltaY0 = (z_.CoordY[IndiceX_Grille + 1, IndiceY_Grille] - Y0) / z_.PasX;
                double Y1 = z_.CoordY[IndiceX_Grille, IndiceY_Grille + 1];
                double DeltaY1 = (z_.CoordY[IndiceX_Grille + 1, IndiceY_Grille + 1] - Y1) / z_.PasX;
                // on peut calculer les coordonnées théoriques du point exprimée en pixel 
                for (int CptX = DebutX, loopTo1 = FinX - 1; CptX <= loopTo1; CptX++)
                {
                    double XReel = X0 + (X1 - X0) * DeltaY_Carte;
                    double YReel = Y0 + (Y1 - Y0) * DeltaY_Carte;
                    // et trouver le pixel interpolé correspondant en faisant une interpolation linéaire sur les 4 pixels englobant le point théorique (X et X+1, Y et Y+1) de la carte source
                    RemplirPixelsTampon(HauteurSource, XReel, YReel, z_.Bits, IndexPixelInterpol);
                    IndexPixelInterpol += 3;
                    X0 += DeltaX0;
                    X1 += DeltaX1;
                    Y0 += DeltaY0;
                    Y1 += DeltaY1;
                }
                DebutX = FinX;
            }
            Interlocked.Increment(ref NbIterations);
            if (NbIterations % 100 == 0)
                AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
        }
        /// <summary>calcul et écriture d'un pixel interpolé à partir de 4 pixels (3 octects par pixel) issus des coordonnées entières de la carte source</summary>
        /// <param name="DecalageY">Nb de ligne de la carte source déja lue</param>
        /// <param name="XReel"> coordonnées X du pixel interpolé </param>
        /// <param name="YReel"> coordonnées Y du pixel interpolé</param>
        /// <param name="TamponInterpole">tampon qui sert de récéptacle pour le pixel interpolé</param>
        /// <param name="IndexTamponInterpole">index dans le tampon où écrire le pixel interpolé</param>
        /// <remarks> Cette procédure peut être appelée en asynchrone (TPL) les variables hors procédure ne doivent pa être modifié pendant le traitement
        /// autrement dit elles doivent être considérée en lecture seule sous peine de résultats inattendus </remarks>
        private void RemplirPixelsTampon(int DecalageY, double XReel, double YReel, byte[] TamponInterpole, int IndexTamponInterpole)
        {
            int DebutLigne;
            int FinLigne;
            int DebutPixel;
            int FinPixel;
            // calcul des coordonnées entières (carte source) et des poids respectifs entre ligne et ligne+1
            int X = (int)Math.Floor(XReel);
            int Y = (int)Math.Floor(YReel);
            // si le point est complétement en dehors de la carte en latitude ou longitude on remplit le tampon de la carte interpolée par 1 pixel de la couleur de carte par defaut
            if (Y > HauteurPixel - 2 || Y < -1 || X > LargeurPixel - 1 || X < -1)
            {
                PixelInterpol.CopyTo(TamponInterpole, IndexTamponInterpole);
            }
            else
            {
                // on prépare un tampon de 4 pixels de la couleur de carte par defaut qui représente les pixels XY, X1Y,XY1,X1Y1 sous forme d'array et pas de tableau
                byte[][][] PixelsSrc = new byte[][][] {
                    new byte[][] { PixelInterpol.ToArray(), PixelInterpol.ToArray() },
                    new byte[][] { PixelInterpol.ToArray(), PixelInterpol.ToArray() } };
                double PoidsX = XReel - X;
                double PoidsY = YReel - Y;
                // il y a 2 cas particuliers concernant les pixels de l'axe des Y
                if (Y == HauteurPixel - 1)
                {
                    DebutLigne = 0; // il n'y a pas de 2ème ligne pour réaliser l'interpolation
                    FinLigne = 0; // on limite à la dernière ligne de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
                }
                else if (Y == -1) // on double la ligne 0 autrement dit il n'y a pas d'interpolation pour cette ligne
                {
                    Y = 0;  // il n'y a pas de 1ère ligne pour réaliser l'interpolation
                    DebutLigne = 1; // on limite à la 1ère ligne de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
                    FinLigne = 1;
                }
                else // cas normal 2 lignes à lire
                {
                    DebutLigne = 0; // l'interpolation sera faite avec 2 lignes de la carte
                    FinLigne = 1;
                }
                // il y 2 cas particuliers concernant les pixels de l'axe des X
                if (X == LargeurPixel - 1)
                {
                    DebutPixel = 0; // il n'y a pas de pixel suivant pour réaliser l'interpolation
                    FinPixel = 0; // on limite au dernier pixel de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
                }
                else if (X == -1)
                {
                    X = 0; // il n'y a pas de 1er pixel pour réaliser l'interpolation 
                    DebutPixel = 1; // on limite au 1er pixel de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
                    FinPixel = 1;
                }
                else // cas normal il y a 2 pixels à lire
                {
                    DebutPixel = 0; // l'interpolation sera faite avec 2 pixels de la carte
                    FinPixel = 1;
                }
                // on calcule l'index à partir duquel les octets de la carte source seront lus
                int IndexCarteSource = X * NbOctetsPixel + (Y - DecalageY) * LargeurOctets + Tampon.IndexBits;
                int Index;
                for (int CptLigne = DebutLigne, loopTo = FinLigne; CptLigne <= loopTo; CptLigne++)
                {
                    Index = 0;
                    // Lecture des pixels de la carte source (de de 1 à 4) et copie dans le tableau représentant les 4 pixels à interpolés
                    for (int CptPixel = DebutPixel, loopTo1 = FinPixel; CptPixel <= loopTo1; CptPixel++)
                    {
                        // si la carte vient d'une image enregistrée en mémoire il faut copier à partir de la mémoire non managée d'où l'utilisation de Marshal
                        if (Tampon.IsBitmap)
                        {
                            Marshal.Copy(Tampon.BitPtr + IndexCarteSource + Index, PixelsSrc[CptLigne][CptPixel], 0, NbOctetsPixel);
                        }
                        else if (IndexCarteSource + Index + NbOctetsPixel < Tampon.Bits.Length)
                        {
                            Array.Copy(Tampon.Bits, IndexCarteSource + Index, PixelsSrc[CptLigne][CptPixel], 0, NbOctetsPixel);
                        }
                        Index += NbOctetsPixel;
                    }
                    IndexCarteSource += LargeurOctets;
                }
                // 3 couleurs par pixel. 1 octet par couleur. ordre des couleurs en mémoire  : 0 = Bleu, 1 = Vert, 2 = Rouge
                for (int Couleur = 0; Couleur <= 2; Couleur++)
                {
                    // interpolation de une des 3 couleurs sur l'axe des X
                    double CouleurPixelL0 = PixelsSrc[0][0][Couleur] + (PixelsSrc[0][1][Couleur] - (double)PixelsSrc[0][0][Couleur]) * PoidsX;
                    double CouleurPixelL1 = PixelsSrc[1][0][Couleur] + (PixelsSrc[1][1][Couleur] - (double)PixelsSrc[1][0][Couleur]) * PoidsX;
                    // interpolation de une des 3 couleurs sur l'axe des Y
                    TamponInterpole[IndexTamponInterpole + Couleur] = (byte)Math.Round(CouleurPixelL0 + (CouleurPixelL1 - CouleurPixelL0) * PoidsY);
                }
            }
        }
        /// <summary> initialise la structure qui contient tous les élements de calcul pour l'interpolation des cartes qui ont déjà une grille </summary>
        private ICAG InitialiserICAG(Carte CarteInterpole)
        {
            var z_ = new ICAG();
            try
            {
                z_.IndexBits = CarteInterpole.Tampon.IndexBits;
                z_.Bits = CarteInterpole.Tampon.Bits;
                // Trouver les pas X(Longitude) et Y(Latitude) exprimés en DD/pixel de la carte interpolée. Ce sont des valeurs constantes.
                // dans FCGP elles sont propres à chaque carte interpolée. d'autres logiciels les imposent au niveau d'un pays ou du monde 
                // ce qui induit des déformations beaucoup plus imortantes de l'image de la carte source
                z_.InterDeltaLong = (CarteInterpole.Coins[2].Longitude - CarteInterpole.Coins[0].Longitude) / LargeurPixel;
                z_.InterDeltaLat = (CarteInterpole.Coins[0].Latitude - CarteInterpole.Coins[2].Latitude) / HauteurPixel;
                // Calcul du nb de pas sur l'axe des X et des Y en fonction de la distance de qualité.
                // La distance de qualité à pour but de couper la droite orignale créer par une interpolation en segments.
                // Plus la distance de qualité est petite et plus la qualité de l'interpolation sera bonne
                z_.NbPasX = (int)Math.Ceiling((Coins[2].X_Grid - Coins[0].X_Grid) / SystemCartographique.Niveau.QualiteInterpolGrille);
                z_.NbPasY = (int)Math.Ceiling((Coins[0].Y_Grid - Coins[2].Y_Grid) / SystemCartographique.Niveau.QualiteInterpolGrille);
                // Calcul de la valeur du pas sur l'axe des X et des Y exprimés en pixels
                z_.PasX = (int)Math.Ceiling(LargeurPixel / (double)z_.NbPasX);
                z_.PasY = (int)Math.Ceiling(HauteurPixel / (double)z_.NbPasY);
                // Calcul de la grille des coordonnées sur X et Y
                z_.CoordX = new double[z_.NbPasX + 1, z_.NbPasY + 1];
                z_.CoordY = new double[z_.NbPasX + 1, z_.NbPasY + 1];
                if (CalculerGrilleCoordonnees(CarteInterpole, z_) == ErreurInterpolation.NotOk)
                    z_ = null;
            }
            catch (Exception Ex)
            {
                z_ = null;
                AfficherErreur(Ex, "X6X6");
            }
            return z_;
        }
        /// <summary> Calcule un ensemble de points exprimées en pixels de la carte source qui représente les points de la grille DD de la carte interpolée. 
        /// Ces points peuvent être situés en dehors de la carte source. Pour rappel une grille à ses lignes parrallèles aux axes </summary>
        private ErreurInterpolation CalculerGrilleCoordonnees(Carte CarteInterpole, ICAG z_)
        {
            var MaxLatSource = default(double);
            var MinLatSource = default(double);
            // pour avoir le Pt0 Grille de la carte source en PointProjection. Cela induit une inprécision de l'ordre de 0.5 mètre
            // mais cela permet si la carte interpolée à le même Pt0 que celui de la carte source d'avoir un delta à 0
            var CaptureSource = ConvertWGS84ToProjection(Coins[0].LatLon, SystemCartographique.Projection.Datum);
            // Valeur exprimée en DD des pas sur l'axe des X et des Y
            double PasLongitude = z_.PasX * z_.InterDeltaLong;
            double PasLatitude = z_.PasY * z_.InterDeltaLat;
            // Valeur exprimée en Métres/Pixel afin d'être sur que le pas soit un multiple de 0.1
            double EchY = Math.Round(Echelle_M_PixelY, 1);
            double EchX = Math.Round(Echelle_M_PixelX, 1);
            // représente 1 point de la grille de la carte interpolée exprimé en DD
            var PointLatLon = default(PointD);
            // pour chaque point de la grille DD de la carte interpolée, on convertit le point DD en point mètres puis en point Pixel
            for (int CptPasX = 0, loopTo = z_.NbPasX; CptPasX <= loopTo; CptPasX++)
            {
                if (CptPasX > 0) // calcule Longitude du point de la grille
                {
                    PointLatLon.Lon += PasLongitude;
                }
                else
                {
                    PointLatLon.Lon = CarteInterpole.Coins[0].Longitude;
                }
                for (int CptPasY = 0, loopTo1 = z_.NbPasY; CptPasY <= loopTo1; CptPasY++)
                {
                    if (CptPasY > 0)
                    {
                        PointLatLon.Lat -= PasLatitude;
                    }
                    else
                    {
                        PointLatLon.Lat = CarteInterpole.Coins[0].Latitude;
                    }
                    // calcul du point LatLon de la carte interpolée transformé en coordonnées métriques de la carte source
                    var PointGrille = ConvertWGS84ToProjection(PointLatLon, SystemCartographique.Projection.Datum);
                    // calcul du point LatLon de la carte interpolée transformé en pixels de la carte source
                    z_.CoordY[CptPasX, CptPasY] = (CaptureSource.Y - PointGrille.Y) / EchY;
                    z_.CoordX[CptPasX, CptPasY] = (PointGrille.X - CaptureSource.X) / EchX;
                    // on cherche pour la première ligne de la carte interpolée
                    if (CptPasY == 0)
                    {
                        // le pas et le pixel de la carte source où se situe le Y Source le plus grand
                        if (z_.CoordY[CptPasX, 0] > MaxLatSource)
                        {
                            MaxLatSource = z_.CoordY[CptPasX, 0];
                            z_.IndexLatMax = CptPasX;
                        }
                        // et le pas et le pixel de la carte source où se situe le Y Source le plus petit
                        if (z_.CoordY[CptPasX, 0] < MinLatSource)
                        {
                            MinLatSource = z_.CoordY[CptPasX, 0];
                            z_.IndexLatMin = CptPasX;
                        }
                    }
                }
            }
            // si les indices LatMax et LatMin sont situés après la largeur de la carte, il faut les corriger pour
            // que la largeur en pixel de la carte interpolée reste la même que la largeur en pixel de la carte source
            // en fait largeurPixel -1 à cause de l'interpolation qui demande 2 pixels pour pouvoir être réalisée
            if (z_.IndexLatMax * z_.PasX > LargeurPixel)
            {
                z_.IndexLatMax -= 1; // on calcule le delta avec l'indice précédent
                z_.DeltaLatMax = (LargeurPixel - 1) / (double)z_.PasX - z_.IndexLatMax;
            }
            if (z_.IndexLatMin * z_.PasX > LargeurPixel)
            {
                z_.IndexLatMin -= 1; // on calcule le delta avec l'indice précédent
                z_.DeltaLatMin = (LargeurPixel - 1) / (double)z_.PasX - z_.IndexLatMin;
            }
            return ErreurInterpolation.Ok;
        }
        #endregion
    }
}