using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.Coordonnees.ProjectionMercatorUtm;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    internal partial class Carte
    {
        #region Procédures associées aux grilles d'une carte
        /// <summary>Permet de dessiner la grille sur l'image de la carte pour 1 des 3 zones max.</summary>
        private bool AfficherGrille()
        {
            bool AfficherGrilleRet = false;
            // si l'échelle est trop petite pour afficher une grille kilométrique on sort sans générer d'erreur 
            if (!SystemCartographique.AfficherGrilleIsOK)
                return true;
            // si la grille est déja dessinnée on passe son chemin sans renvoyer d'erreur
            if (SystemCartographique.GrilleExiste == true)
                return true;
            AfficherVisueInformation($"{NomComplet} :{Separateur}Affichage de la grille sur la carte");
            if (!CalculerGrille())
                return AfficherGrilleRet; // c'est une erreur
            try
            {
                int HauteurTravail = HauteurReserve > HauteurPixel ? HauteurPixel : HauteurReserve;
                if (TamponType == TypeSupportCarte.Fichier)
                {
                    using (var FichierBIN = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                    {
                        int TailleTravail = LargeurOctets * HauteurTravail;
                        var HauteurALire = default(int);
                        CheminFluxLecture = CheminEnregistrementProvisoire + @"\CarteAvecGrille.raw";
                        using (var FichierAvecGrille = new FileStream(CheminFluxLecture, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, TailleTravail))
                        {
                            do
                            {
                                // on charge une partie de la carte sans grille dans le tampon
                                FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTravail);
                                // on dessinne la grille
                                for (int Cpt = 0, loopTo = NbZonesUTM - 1; Cpt <= loopTo; Cpt++)
                                {
                                    if (!AfficherGrilleZones(Cpt, HauteurTravail, HauteurALire))
                                    {
                                        return AfficherGrilleRet;
                                    } // il peut y avoir une erreur
                                }
                                // on enregistre la carte avec la grille
                                FichierAvecGrille.Write(Tampon.Bits, Tampon.IndexBits, TailleTravail);
                                HauteurALire += HauteurReserve;
                                if (HauteurALire < HauteurPixel & HauteurALire + HauteurReserve > HauteurPixel)
                                {
                                    TailleTravail = LargeurOctets * (HauteurPixel - HauteurALire);
                                }
                            }
                            while (HauteurALire < HauteurPixel);
                        }
                    }
                }
                else // cas du suppport de carte en mémoire. Il n'y a qu'une seule tranche donc pas de décalage pour le dessin et on travaille sur la carte entière
                {
                    for (int Cpt = 0, loopTo1 = NbZonesUTM - 1; Cpt <= loopTo1; Cpt++)
                    {
                        if (!AfficherGrilleZones(Cpt, HauteurTravail, 0))
                        {
                            return AfficherGrilleRet;
                        } // il peut y avoir une erreur
                    }
                }
                GrilleIsAffiche = true;
                AfficherGrilleRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "O9A5");
            }
            return AfficherGrilleRet;
        }
        /// <summary>Permet de dessiner la grille sur l'image de la carte pour 1 des 3 zones max.</summary>
        /// <param name="Zone_UTM">1 à 3 si la carte est à cheval sur 3 zones UTM</param>
        /// <remarks>normalement cette procédure n'est pas appelé directement</remarks>
        private bool AfficherGrilleZones(int Zone_UTM, int HauteurTravail, int DecalY = 0)
        {
            bool AfficherGrilleZonesRet = false;
            try
            {
                // Pour dessiner avec 2 largeurs différentes
                Pen Pb_1;
                Pen Pb_2;
                var Decalage = new Size(0, -DecalY);
                // pour l'échelle d'impression "025" et l'échelle de capture "007" on élargi la taille
                if (SystemCartographique.SystemDoubleGrille)
                {
                    Pb_1 = new Pen(Color.Blue, 2f);
                    Pb_2 = new Pen(Color.Blue, 4f);
                }
                else    // environ 190 dpi pour toutes les autres échelles
                {
                    Pb_1 = new Pen(Color.Blue, 1f);
                    Pb_2 = new Pen(Color.Blue, 2f);
                }
                // On indique que l'on travaille avec la carte en cours, si l'image provient d'une carte existante on est obligé de recréer un bitmap car le bitmap existant est en lecture seul
                using (var Carte_GrilleGraphic = Graphics.FromImage(Tampon.IsBitmap ? new Bitmap(LargeurPixel, HauteurTravail, LargeurOctets, FormatPixel, ImageBitPtr) : Image))
                {
                    ref var WB = ref Grilles[Zone_UTM];
                    Carte_GrilleGraphic.PageUnit = GraphicsUnit.Pixel;
                    Carte_GrilleGraphic.SmoothingMode = SmoothingMode.AntiAlias;
                    Carte_GrilleGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    // Trace les méridiens multiples de 6° qui délimitent les zones UTM pour les zones N°2 et N°3
                    if (Zone_UTM > 0)
                    {
                        Carte_GrilleGraphic.DrawLine(Pb_2, WB.ClipZoneDeb, 0, WB.ClipZoneDeb, HauteurTravail);
                    }
                    // définit la surface sur laquelle on peut dessiner (pixel par rapport à  ceux de l'image
                    var ClipRect = new Rectangle(WB.ClipZoneDeb, 0, WB.ClipZoneFin - WB.ClipZoneDeb, HauteurTravail);
                    Carte_GrilleGraphic.SetClip(ClipRect);
                    // Pour chaque ligne sur l'axe des X on la trace sur la carte les segments de droite représentants les lignes verticales de la grilles
                    for (int BoucleX = 0, loopTo = WB.Points.GetUpperBound(0); BoucleX <= loopTo; BoucleX++)
                    {
                        for (int BoucleY = 0, loopTo1 = WB.Points.GetUpperBound(1); BoucleY < loopTo1; BoucleY++)
                            // on desinne les droites entre 2 points de la grille
                            Carte_GrilleGraphic.DrawLine(Pb_1, Point.Add(WB.Points[BoucleX, BoucleY], Decalage), Point.Add(WB.Points[BoucleX, BoucleY + 1], Decalage));
                    }
                    // Pour chaque ligne sur l'axe des Y on la trace sur la carte les segments de droite représentants les lignes horizontales de la grilles
                    for (int BoucleY = 0, loopTo2 = WB.Points.GetUpperBound(1); BoucleY <= loopTo2; BoucleY++)
                    {
                        for (int BoucleX = 0, loopTo3 = WB.Points.GetUpperBound(0); BoucleX < loopTo3; BoucleX++)
                            // on desinne les droites entre 2 points de la grille
                            Carte_GrilleGraphic.DrawLine(Pb_1, Point.Add(WB.Points[BoucleX, BoucleY], Decalage), Point.Add(WB.Points[BoucleX + 1, BoucleY], Decalage));
                    }
                }
                AfficherGrilleZonesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "N2X2");
            }

            return AfficherGrilleZonesRet;
        }
        /// <summary>Permet d'ajouter les étiquettes de la grille sur l'image de la carte. Cette procédure gère les grilles à cheval sur plusieurs zones </summary>
        private bool AjouterReferences()
        {
            bool AjouterReferencesRet = false;
            if (!SystemCartographique.AfficherGrilleIsOK)
            {
                GrilleIsAffiche = false;
                return true; // on sort sans erreur mais le fichier des références grille ne sera pas généré car l'échelle est trop petite.
            }
            // mettre à jour la fenêtre d'information
            AfficherVisueInformation($"{NomComplet} :{Separateur}Ajout des références de la grille");
            if (Grilles is null) // on ne recalcule pas si cela a déjà été fait pour le dessin
            {
                if (!CalculerGrille())
                    return AjouterReferencesRet; // erreur dans le calcul de la grille
            }
            try
            {
                int HauteurTravail = HauteurReserve > HauteurPixel ? HauteurPixel : HauteurReserve;
                string NomFichierCarte = $@"{CheminCarte}\{NomComplet + "_R"}";
                if (TamponType == TypeSupportCarte.Fichier)
                {
                    using (var FichierBIN = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                    {
                        int TailleTravail = LargeurOctets * HauteurTravail;
                        var HauteurALire = default(int);
                        // On enregistre la carte avec les references directement au bon endroit mais en format raw
                        CheminFluxLecture = CheminEnregistrementProvisoire + @"\CarteAvecGrilleReference.raw";
                        using (var FichierAvecReference = new FileStream(CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None, TailleTravail))
                        {
                            do
                            {
                                // on charge une partie de la carte sans grille dans le tampon
                                FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTravail);
                                // on dessinne la grille
                                for (int Cpt = 0, loopTo = NbZonesUTM - 1; Cpt <= loopTo; Cpt++)
                                {
                                    if (!AjouterReferencesZones(Cpt, HauteurALire, HauteurTravail))
                                        return AjouterReferencesRet; // il peut y avoir une erreur
                                }
                                // on enregistrer la carte avec la grille
                                FichierAvecReference.Write(Tampon.Bits, Tampon.IndexBits, TailleTravail);
                                HauteurALire += HauteurTravail;
                                if (HauteurALire < HauteurPixel & HauteurALire + HauteurTravail > HauteurPixel)
                                {
                                    TailleTravail = LargeurOctets * (HauteurPixel - HauteurALire);
                                }
                            }
                            while (HauteurALire < HauteurPixel);
                        }
                    }
                    if (ToutFormatCarte && Image.Width <= MaxPixelsJpeg && Image.Height <= MaxPixelsJpeg)
                    {
                        // on sauvegarde toujours en JPEG
                        if (!FichierRawToFormats(NomFichierCarte + ".Jpeg", true))
                            return AjouterReferencesRet;
                    }
                    else
                    {
                        // on sauvegarde toujours en BMP
                        if (!FichierSrcToDst(NomFichierCarte + ".Bmp", true))
                            return AjouterReferencesRet;
                    }
                }
                else // cas du suppport de carte en mémoire. Il n'y a qu'une seule tranche donc pas de décalage pour le dessin et on travaille sur la carte entière
                {
                    for (int Cpt = 0, loopTo1 = NbZonesUTM - 1; Cpt <= loopTo1; Cpt++)
                    {
                        if (!AjouterReferencesZones(Cpt, 0, HauteurTravail))
                            return AjouterReferencesRet;
                    }
                    //on sauvegarde toujours en JPEG sauf dépassement des capacités de l'encodeur Jpeg de .net
                    if (Image.Width > MaxPixelsJpeg || Image.Height > MaxPixelsJpeg)
                    {
                        Image.Save(NomFichierCarte + ".Bmp", ImageFormat.Bmp);
                    }
                    else
                    {
                        Image.Save(NomFichierCarte + ".Jpeg", EncodeurJpeg, QualiteJPEG(PartagerSettings.QUALITE_JPEG));
                    }
                }
                AjouterReferencesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "B2Y3");
            }
            return AjouterReferencesRet;
        }
        /// <summary> calcul et dessine les références des grilles (Etiquettes des lignes et nom de la grille) </summary>
        /// <param name="Zone">Numéro de la zone UTM de 0 à 2</param>
        private bool AjouterReferencesZones(int Zone, int DecalTrancheY, int HauteurTravail)
        {
            bool AjouterReferencesZonesRet = false;
            try
            {
                var Pb_1 = new Pen(Color.Blue, 2f);
                var Pb_2 = new Pen(Color.Blue, 4f);
                // Pour recevoir la police des étiquettes. permet d'adapter la taille du texte en fonction de l'échelle d'impression et de l'échelle de capture
                var F = new Font("Microsoft Sans Serif", 30f * (DPIImpressionX / SystemCartographique.MaxDPIImpression), FontStyle.Bold, GraphicsUnit.Pixel);
                // si l'image provient d'une carte existante on est obligé de recréer un bitmap car le bitmap existant est en lecture seule
                using (var SourceGraphic = Graphics.FromImage(Tampon.IsBitmap ? new Bitmap(LargeurPixel, HauteurTravail, LargeurOctets, FormatPixel, ImageBitPtr) : Image))
                {
                    {
                        ref var WB = ref Grilles[Zone];
                        #region Ecriture des étiquettes sur l'axe des X
                        // définit la surface sur laquelle on peut dessiner
                        var ClipRect = new Rectangle(WB.ClipZoneDeb, 0, WB.ClipZoneFin - WB.ClipZoneDeb, HauteurTravail);
                        SourceGraphic.SetClip(ClipRect);
                        // pour chaque ligne de la grille sur l'axe des X (vertical)
                        for (int Boucle = WB.MinX, loopTo = WB.MaxX; Boucle <= loopTo; Boucle++)
                        {
                            // On trouve le texte de l'étiquette en km et une décimale pour l'échelle 1/4000
                            string Etiquette = (Boucle * SystemCartographique.Niveau.PasGrilleNumeric / 1000d).ToString();
                            // on regarde la dimension de l'étiquette avec la police choisie (taille constante quelque soit l'échelle d'impression)
                            var S = SourceGraphic.MeasureString(Etiquette, F);
                            // le Y correspond à la hauteur de la carte (attention les Y du monde virtuel sont inversés par rapport aux y d'une image - la hauteur de l'étiquette
                            float YEtiquette = HauteurPixel;
                            // Le X en pixel correspondant pour le haut de la carte (sud)
                            int X1 = WB.Points[Boucle - WB.MinX, 1].X;
                            int Y1 = WB.Points[Boucle - WB.MinX, 1].Y;
                            int X2 = WB.Points[Boucle - WB.MinX, 0].X;
                            int Y2 = WB.Points[Boucle - WB.MinX, 0].Y;
                            float XEtiquette = X1 + (X2 - X1) * (Y1 - YEtiquette) / (Y1 - Y2);
                            YEtiquette = YEtiquette - DecalTrancheY - S.Height;
                            // on dessine un rectangle contenant la taille de l'étiquette
                            SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette - S.Width / 2f, YEtiquette, S.Width, S.Height);
                            // on ecrit l'étiquette par dessus
                            SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette - S.Width / 2f, YEtiquette);
                            // Y correspondant du nord de la carte toujours  0
                            YEtiquette = 0f;
                            // Le X en pixel correspondant pour le haut de la carte (nord)
                            X1 = WB.Points[Boucle - WB.MinX, WB.MaxY - WB.MinY - 1].X;
                            Y1 = WB.Points[Boucle - WB.MinX, WB.MaxY - WB.MinY - 1].Y;
                            X2 = WB.Points[Boucle - WB.MinX, WB.MaxY - WB.MinY].X;
                            Y2 = WB.Points[Boucle - WB.MinX, WB.MaxY - WB.MinY].Y;
                            XEtiquette = X1 + (X2 - X1) * (Y1 - YEtiquette) / (Y1 - Y2);
                            YEtiquette = -DecalTrancheY;
                            // on dessine un rectangle contenant la taille de l'étiquette
                            SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette - S.Width / 2f, YEtiquette, S.Width, S.Height);
                            // on ecrit l'étiquette par dessus
                            SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette - S.Width / 2f, YEtiquette);
                        }
                        #endregion
                        #region Ecriture des étiquettes sur l'axe des Y
                        // pour chaque ligne de la grille sur l'axe des Y (horizontal) si la zone est concernée
                        if (Zone == 0 | Zone == NbZonesUTM - 1)
                        {
                            for (int Boucle = WB.MinY, loopTo1 = WB.MaxY; Boucle <= loopTo1; Boucle++)
                            {
                                // On trouve l'étiquette en km
                                string Etiquette = (Boucle * SystemCartographique.Niveau.PasGrilleNumeric / 1000d).ToString();
                                // on regarde la dimension de l'étiquette avec la police choisie (taille constante quelque soit l'échelle d'impression)
                                var S = SourceGraphic.MeasureString(Etiquette, F);
                                // si il s'agit de la première zone (0) on met les étiquette coté ouest
                                if (Zone == 0)
                                {
                                    // le X correspondant toujours 0 pour l'ouest
                                    float XEtiquette = 0f;
                                    // Le Y en pixel correspondant pour le coté gauche de la carte (ouest)
                                    // pour l'interpolation, les lignes verticales n'étant pas forcément verticale on est obligé de tester si le X du 2ème point est bien 
                                    // sur la carte
                                    int DecaleY = WB.Points[1, Boucle - WB.MinY].X < 0 ? 1 : 0;
                                    int X1 = WB.Points[0 + DecaleY, Boucle - WB.MinY].X;
                                    int Y1 = WB.Points[0 + DecaleY, Boucle - WB.MinY].Y;
                                    int X2 = WB.Points[1 + DecaleY, Boucle - WB.MinY].X;
                                    int Y2 = WB.Points[1 + DecaleY, Boucle - WB.MinY].Y;
                                    float YEtiquette = Y1 + (Y2 - Y1) * (X1 - XEtiquette) / (X1 - X2) - DecalTrancheY;
                                    // on dessine un rectangle contenant la taille de l'étiquette
                                    SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette, YEtiquette - S.Height / 2f, S.Width, S.Height);
                                    // on ecrit l'étiquette par dessus
                                    SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette, YEtiquette - S.Height / 2f);
                                }
                                // si il s'agit de la dernière zone on met les étiquettes coté Est
                                if (Zone == NbZonesUTM - 1 | SystemCartographique.GrilleExiste == true)
                                {
                                    // le X correspond toujours la largeur de la carte
                                    float XEtiquette = LargeurPixel;
                                    // Le Y en pixel correspondant pour le coté droit de la carte (est)
                                    // pour l'interpolation, les lignes verticales n'étant pas forcément verticale on est obligé de tester si le X du 1er point est bien
                                    // sur la carte
                                    int DecaleY = WB.Points[WB.MaxX - WB.MinX - 1, Boucle - WB.MinY].X > LargeurPixel ? 1 : 0;
                                    int X1 = WB.Points[WB.MaxX - WB.MinX - 1 - DecaleY, Boucle - WB.MinY].X;
                                    int Y1 = WB.Points[WB.MaxX - WB.MinX - 1 - DecaleY, Boucle - WB.MinY].Y;
                                    int X2 = WB.Points[WB.MaxX - WB.MinX - DecaleY, Boucle - WB.MinY].X;
                                    int Y2 = WB.Points[WB.MaxX - WB.MinX - DecaleY, Boucle - WB.MinY].Y;
                                    float YEtiquette = Y1 + (Y2 - Y1) * (X1 - XEtiquette) / (X1 - X2) - DecalTrancheY;
                                    // on dessine un rectangle contenant la taille de l'étiquette
                                    SourceGraphic.FillRectangle(Brushes.PaleGoldenrod, XEtiquette - S.Width, YEtiquette - S.Height / 2f, S.Width, S.Height);
                                    // on ecrit l'étiquette par dessus
                                    SourceGraphic.DrawString(Etiquette, F, Brushes.CornflowerBlue, XEtiquette - S.Width, YEtiquette - S.Height / 2f);
                                }
                            }
                        }
                        #endregion
                        #region Ecriture du nom de grille
                        // on écrit la référence de la grille dans le coin haut gauche de la carte
                        string EtiquetteGrille = SystemCartographique.GrilleNom;
                        float YEtiquetteGrille = 0f + DecalTrancheY;
                        // si il s'agit d'un grille UTM on rajoute la zone
                        if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.DD)
                        {
                            EtiquetteGrille += $" {Coins[0].Hemisphere_UTM}{Coins[0].NumZone_UTM + Zone:N0}";
                        }
                        // on mesure la taille du texte
                        var S_Grille = SourceGraphic.MeasureString(EtiquetteGrille + ".", F);
                        // on dessine un rectangle
                        SourceGraphic.FillRectangle(Brushes.Coral, WB.ClipZoneDeb, YEtiquetteGrille, S_Grille.Width, S_Grille.Height);
                        // on ecrit par dessus
                        SourceGraphic.DrawString(EtiquetteGrille, F, Brushes.Black, WB.ClipZoneDeb, YEtiquetteGrille);
                        #endregion
                    }
                }
                AjouterReferencesZonesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "H9Z5");
            }

            return AjouterReferencesZonesRet;
        }
        /// <summary>Procédure qui définit la surface sur laquelle peut être dessinée la grille en fonction du nb de zones et du numéro de zone
        /// elle appelle le calcul de la grille et des références</summary>
        /// <remarks>normalement cette procédure n'est pas appelé directement</remarks>
        private bool CalculerGrille()
        {
            bool CalculerGrilleRet = false;
            try
            {
                // on dimensionne le receptacle des calculs de grille (1 par zone utm)
                Grilles = new Grille[NbZonesUTM];
                // Calcul du méridien qui sépare la zone N°1 de la zone N°2
                int MeridienFinZoneUTM = (Coins[0].NumZone_UTM - 30) * 6;
                int ZoneFin = 0;
                double LongitudeDebZone = Coins[0].Longitude;
                double LongitudeFinZone = 0d;
                // Il peut y avoir jusquà 3 zones en France et 2 en Guyanne et 1 en suisse
                for (int Cpt = 0, loopTo = NbZonesUTM - 1; Cpt <= loopTo; Cpt++)
                {
                    // Le début de la zone est égal à la fin de la zone suivante
                    Grilles[Cpt] = new Grille() { ClipZoneDeb = ZoneFin };
                    // Le début de la zone est égal à la fin de la zone suivante
                    // si le méridien de la première zone est supèrieur à la longitude de fin de la carte il s'agit de la dernière zone et le clip de fin est
                    // égal à la fin de la carte. C'est aussi le cas pour les cartes Suisse qui ne comporte qu'une seule zone pour la grille suisse
                    if (MeridienFinZoneUTM > Coins[1].Longitude) // Or SystemCartographique.GrilleExiste = True
                    {
                        Grilles[Cpt].ClipZoneFin = LargeurPixel;
                        LongitudeFinZone = Coins[1].Longitude;
                    }
                    else
                    {
                        // sinon on calcule la fin de la zone exprimée en pixel. Les longitudes étant de la forme ax+b on peut se contenter de faire une règle de 3
                        Grilles[Cpt].ClipZoneFin = (int)Math.Round((MeridienFinZoneUTM - Coins[0].Longitude) * LargeurPixel / (Coins[1].Longitude - Coins[0].Longitude));
                        LongitudeFinZone = MeridienFinZoneUTM;
                    }
                    var MeridienMilieuZone = MeridienFinZoneUTM - 3;
                    var IsOverMiddleZoneUtm = LongitudeDebZone < MeridienMilieuZone && LongitudeFinZone > MeridienMilieuZone;
                    // Calcul des lignes X et Y de la grille de la zone UTM N°1 et plus. il peut y avoir des erreurs
                    if (!CalculerGrilleZones(Cpt, MeridienMilieuZone, IsOverMiddleZoneUtm))
                        return CalculerGrilleRet; // il peut y avoir une erreur
                    // on oublie pas de passer à la zone suivante
                    MeridienFinZoneUTM += 6;
                    ZoneFin = Grilles[Cpt].ClipZoneFin;
                    LongitudeDebZone = LongitudeFinZone;
                }
                CalculerGrilleRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T5Y0");
            }
            return CalculerGrilleRet;
        }
        /// <summary>Calcule les lignes qui composent la grille UTM qui sera dessinée sur la carte géo-référencée
        /// réservé pour les carte sans grille donc issues des données IGN</summary>
        /// <param name="Zone_UTM">Numéro de la zone à calculer (de 1 à 3)</param>''' 
        /// <param name="MeridienMilieuZone"> Méridien du milieu de la zone utm (-3, 3, 9 pour les 3 zones utm de la france) </param>
        /// <param name="IsOverMiddleZoneUtm"> La zone de capture est à cheval sur le méridien de milieu de zone </param>
        private bool CalculerGrilleZones(int Zone_UTM, int MeridienMilieuZone, bool IsOverMiddleZoneUtm)
        {
            bool CalculerGrilleZonesRet = false;
            int PasGrille = SystemCartographique.Niveau.PasGrilleNumeric;
            try
            {
                {
                    ref var WB = ref Grilles[Zone_UTM];
                    if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
                    {
                        #region SystemCartographique.GrilleExiste = True
                        // Il s'agit de la grille Suisse
                        // On trouve le minimum et le maximum sur chaque axe en multiple du pas de la grille
                        WB.MinX = (int)Math.Truncate(Math.Min(Coins[0].X_Grid, Coins[3].X_Grid) / PasGrille);
                        WB.MaxX = (int)Math.Truncate(Math.Max(Coins[1].X_Grid, Coins[2].X_Grid) / PasGrille + 1d);
                        WB.MinY = (int)Math.Truncate(Math.Min(Coins[3].Y_Grid, Coins[2].Y_Grid) / PasGrille);
                        WB.MaxY = (int)Math.Truncate(Math.Max(Coins[0].Y_Grid, Coins[1].Y_Grid) / PasGrille + 1d);
                        WB.Points = new Point[WB.MaxX - WB.MinX + 1, WB.MaxY - WB.MinY + 1];
                        // On calcule les coordonnées Géodésique WGS84 de chaque point de la grille

                        for (int BoucleX = WB.MinX, loopTo = WB.MaxX; BoucleX <= loopTo; BoucleX++)
                        {
                            for (int BoucleY = WB.MinY, loopTo1 = WB.MaxY; BoucleY <= loopTo1; BoucleY++)
                            {
                                var Grille = new PointProjection(new PointD(BoucleX * PasGrille, BoucleY * PasGrille));
                                if (SystemCartographique.IsInterpol)
                                {
                                    // si la carte est retaillée il faut passer par les DD à partir des coordonnées de la grille
                                    var LatLon = ConvertProjectionToWGS84(SystemCartographique.Projection.Datum, Grille);
                                    if (LatLon.IsEmpty)
                                        return CalculerGrilleZonesRet;
                                    // calculer les pixels avec les fonctions du systemecartographique propre à chaque carte
                                    WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(LatLon);
                                }
                                else
                                {
                                    // calculer les pixels correspondants aux LL à partir des fonctions coordonnées to pixel de la carte
                                    WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Grille.Coordonnees);
                                    // pour les cartes non interpolées il faut passer du système virtuel à l'image
                                    WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] = WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] - PointOrigne;
                                }
                            }

                        }
                    }
                    #endregion
                    else
                    {
                        #region SystemCartographique.GrilleExiste = False
                        // On trouve le minimum et le maximum sur chaque axe en multiple du pas de la grille UTM
                        WB.MinX = (int)Math.Truncate(Math.Min(Coins[0].UTMs[Zone_UTM].X, Coins[3].UTMs[Zone_UTM].X) / PasGrille);
                        WB.MaxX = (int)Math.Truncate(Math.Max(Coins[1].UTMs[Zone_UTM].X, Coins[2].UTMs[Zone_UTM].X) / PasGrille + 1d);
                        WB.MaxY = (int)Math.Truncate(Math.Max(Coins[0].UTMs[Zone_UTM].Y, Coins[1].UTMs[Zone_UTM].Y) / PasGrille + 1d);
                        if (IsOverMiddleZoneUtm)
                        {
                            //surface de capture à cheval sur le méridien de milieu de zone UTM. Le milieu de la zone donne le Y le plus bas 
                            var PointUtmZone2 = ConvertLLToUtm(new PointD(MeridienMilieuZone, Coins[2].Latitude),
                                                               Datums.WGS84, Coins[2].NumZone_UTM + Zone_UTM);
                            WB.MinY = (int)Math.Truncate(PointUtmZone2.Y / PasGrille);
                        }
                        else
                        {
                            //surface de capture sur une moitié de zone UTM
                            WB.MinY = (int)Math.Truncate(Math.Min(Coins[3].UTMs[Zone_UTM].Y, Coins[2].UTMs[Zone_UTM].Y) / PasGrille);
                        }
                        WB.Points = new Point[WB.MaxX - WB.MinX + 1, WB.MaxY - WB.MinY + 1];
                        // On calcule les coordonnées Géodésique WGS84 de chaque point de la grille car le géoréférencement est en LL sur GF et DT

                        for (int BoucleX = WB.MinX, loopTo2 = WB.MaxX; BoucleX <= loopTo2; BoucleX++)
                        {
                            for (int BoucleY = WB.MinY, loopTo3 = WB.MaxY; BoucleY <= loopTo3; BoucleY++)
                            {
                                // calcul de la longitude, latitude à partir des coordonnées UTM
                                var Grille = new PointProjection(new PointD(BoucleX * PasGrille, BoucleY * PasGrille), Coins[0].NumZone_UTM + Zone_UTM, Coins[0].Hemisphere_UTM);
                                var LatLon = ConvertUtmToLL(Datums.UTM_WGS84, Grille);
                                if (LatLon.IsEmpty)
                                    return CalculerGrilleZonesRet;
                                // conversion des DD en Pixel.
                                WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(LatLon);
                                // pour les cartes non interpolées il faut passer du système virtuel à l'image, pour les cartes interpolée on est déja ramené à l'image
                                WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] = WB.Points[BoucleX - WB.MinX, BoucleY - WB.MinY] - PointOrigne;
                            }
                        }
                        #endregion
                    }
                }
                CalculerGrilleZonesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "E0W8");
            }
            return CalculerGrilleZonesRet;
        }
        #endregion
    }
}