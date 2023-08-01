using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary>contient toutes les procédures et fonctions nécessaires pour écrire les fichiers de géoréferencement
    /// d'une carte ou lire un fichier georef mais uniquement private</summary>
    internal partial class Carte
    {
        #region procédures générales
        /// <summary> Procède à la création de la carte interpolée et réalise les demandes associées. </summary>
        private bool GenererInterpolation()
        {
            bool GenererInterpolationRet = false;
            try
            {
                // création de la carte interpolée
                using (var CarteInterpole = new Carte())
                {
                    // il faut faire l'interpolation de la carte avant de générer les fichiers tuiles
                    if (!(InterpolerCarte(CarteInterpole) == ErreurInterpolation.Ok))
                        return GenererInterpolationRet;
                    if ((DemandeFichiersGeoref & ChoixFichiersGeoref.Interpols) > ChoixFichiersGeoref.Aucun)
                    {
                        if (!CarteInterpole.GenererFichiersGeoreferencement())
                            return GenererInterpolationRet;
                    }
                    if (DemandeFichiersTuiles > ChoixFichiersTuiles.Aucun)
                    {
                        if (!CarteInterpole.GenererFichiersTuiles())
                            return GenererInterpolationRet;
                    }
                    if (GrilleIsAffiche & DemandeAjouterCoordonneesGrille.HasFlag(ChoixCoordonneesGrille.Ref_Interpol))
                    {
                        if (!CarteInterpole.AjouterReferences())
                            return GenererInterpolationRet;
                    }
                }
                GenererInterpolationRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "L8L7");
            }
            return GenererInterpolationRet;
        }
        /// <summary> Ecrit les fichiers de géoréférencement associés à la carte. Cette procédure est appelée soit par la carte originale soit par la carte interpolée</summary>
        private bool GenererFichiersGeoreferencement()
        {
            bool GenererFichiersGeoreferencementRet = false;
            AfficherVisueInformation($"{NomComplet} :{Separateur}Création des fichiers de géoréférencement");

            try
            {
                // on en enregistre systématiquement la carte par type de carte car il y a forcément un fichier de géo-référencment si on est là
                if ((DemandeFichiersGeoref & ChoixFichiersGeoref.Cartes) > ChoixFichiersGeoref.Aucun & !SystemCartographique.IsInterpol || (DemandeFichiersGeoref & ChoixFichiersGeoref.Interpols) > ChoixFichiersGeoref.Aucun & SystemCartographique.IsInterpol)
                {
                    string NomFichierCarte = CheminCarte + @"\" + NomComplet;
                    if (TamponType == TypeSupportCarte.Fichier)
                    {
                        if (File.Exists(NomFichierCarte))
                            File.Delete(NomFichierCarte);
                        if (Image.Width > MaxPixelsJpeg || Image.Height > MaxPixelsJpeg || Format == ImageFormat.Bmp)
                        {
                            // l'enregistrement se fera en BMP et on change éventuellement le format de la carte si il n'est pas supporté
                            if (Format != ImageFormat.Bmp)
                                Format = ImageFormat.Bmp;
                            if (!FichierSrcToDst(NomFichierCarte + "." + Format.ToString(), true))
                                // l'enregistrement se fera en BMP
                                return GenererFichiersGeoreferencementRet;
                        }
                        else
                        {
                            if (!FichierRawToFormats(NomFichierCarte + "." + Format.ToString(), false))
                                // sinon en JPEG ou PNG suivant le choix effectif de l'utilisateur
                                return GenererFichiersGeoreferencementRet;
                        }
                    }
                    else
                    {
                        if (Image.Width > MaxPixelsJpeg || Image.Height > MaxPixelsJpeg || Format == ImageFormat.Bmp)
                        {
                            // 'l'enregistrement se fera en BMP et on change éventuellement le format de la carte si il n'est pas supporté
                            if (Format != ImageFormat.Bmp)
                                Format = ImageFormat.Bmp;
                            Image.Save(NomFichierCarte + "." + Format.ToString(), ImageFormat.Bmp);
                        }
                        else if (Format == ImageFormat.Jpeg)
                            // ou en JPEG avec la qualité désirée
                            Image.Save(NomFichierCarte + "." + Format.ToString(), EncodeurJpeg, QualiteJPEG(PartagerSettings.QUALITE_JPEG));
                        else
                            //ou en PNG sans perte
                            Image.Save(NomFichierCarte + "." + Format.ToString(), ImageFormat.Png);
                    }
                }
                if ((DemandeFichiersGeoref & ChoixFichiersGeoref.Georefs) > ChoixFichiersGeoref.Aucun)
                {
                    if ((DemandeFichiersGeoref & ChoixFichiersGeoref.Georef_Carte) > ChoixFichiersGeoref.Aucun && !SystemCartographique.IsInterpol || (DemandeFichiersGeoref & ChoixFichiersGeoref.Georef_Interpol) > ChoixFichiersGeoref.Aucun && SystemCartographique.IsInterpol)
                    {
                        if (!EcrireFichierGeoref())
                            return GenererFichiersGeoreferencementRet;
                    }
                }
                // on sauve les fichiers de géoréferencement
                if ((DemandeFichiersGeoref & ChoixFichiersGeoref.MapInfos) > ChoixFichiersGeoref.Aucun)
                {
                    if ((SystemCartographique.SiteCarto == SitesCartographiques.Géofoncier || SystemCartographique.SiteCarto == SitesCartographiques.DomTom) & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireMapInfoMercator())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireMapInfoLV03())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (!EcrireMapInfoWGS84())
                        return GenererFichiersGeoreferencementRet;
                }
                if ((DemandeFichiersGeoref & ChoixFichiersGeoref.OziExploreurs) > ChoixFichiersGeoref.Aucun)
                {
                    // si retail cela indique que c'est une Demarage.Captures Suisse interpolée en wgs84
                    if ((SystemCartographique.SiteCarto == SitesCartographiques.Géofoncier || SystemCartographique.SiteCarto == SitesCartographiques.DomTom) & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireOziExploreurMercator())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireOziExploreurLV03())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (!EcrireOziExploreurWGS84())
                        return GenererFichiersGeoreferencementRet;
                }
                if ((DemandeFichiersGeoref & ChoixFichiersGeoref.CompeLands) > ChoixFichiersGeoref.Aucun)
                {
                    if ((SystemCartographique.SiteCarto == SitesCartographiques.Géofoncier || SystemCartographique.SiteCarto == SitesCartographiques.DomTom) & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireCompeLandMercator())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireCompeLandLV03())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (!EcrireCompeLandWGS84())
                        return GenererFichiersGeoreferencementRet;
                }
                if ((DemandeFichiersGeoref & ChoixFichiersGeoref.QGISs) > ChoixFichiersGeoref.Aucun)
                {
                    if ((SystemCartographique.SiteCarto == SitesCartographiques.Géofoncier || SystemCartographique.SiteCarto == SitesCartographiques.DomTom) & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireQgisMercator())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile & !SystemCartographique.IsInterpol)
                    {
                        if (!EcrireQgisLV03())
                            return GenererFichiersGeoreferencementRet;
                    }
                    else if (!EcrireQgisWGS84())
                        return GenererFichiersGeoreferencementRet;
                }
                GenererFichiersGeoreferencementRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "D9Z0");
            }
            return GenererFichiersGeoreferencementRet;
        }
        /// <summary> fait le ou les fichiers tuiles à partir de la carte transmise et du nb de tuiles demandées </summary>
        private bool GenererFichiersTuiles()
        {
            bool GenererFichiersTuilesRet = false;
            // créer un repertoire tampon fichier KML et un sous repertoire tampon des tuiles

            try
            {
                var DirInfo = Directory.CreateDirectory(CheminEnregistrementProvisoire + ExtensionCheminImagesTuiles);
                MessInfo = $"{NomComplet} :{Separateur}Génération des tuiles :{Separateur}";
                if (!EcrireTuiles())
                    return GenererFichiersTuilesRet;
                if (Tuiles is not null) // si il y a au moins une tuile on peut générer les fichiers tuiles si ils sont demandés
                {
                    if (DemandeFichiersTuiles.HasFlag(ChoixFichiersTuiles.KMZ))
                    {
                        MessInfo = $"{NomComplet} :{Separateur}Génération du fichier KMZ :{Separateur}";
                        if (!EcrireFichierKMZ())
                            return GenererFichiersTuilesRet;
                    }
                    if (DemandeFichiersTuiles.HasFlag(ChoixFichiersTuiles.JNX))
                    {
                        if (DemandeAjoutNiveau)
                        {
                            MessInfo = $"{NomComplet} :{Separateur}Ajout d'un niveau au fichier JNX :{Separateur}";
                            if (!StructuresJNX.AjouterNiveauCarteJNX(this))
                                return GenererFichiersTuilesRet;
                        }
                        else
                        {
                            MessInfo = $"{NomComplet} :{Separateur}Génération du fichier JNX :{Separateur}";
                            if (!StructuresJNX.CreerCarteJNX(this))
                                return GenererFichiersTuilesRet;
                        }
                    }
                    if (DemandeFichiersTuiles.HasFlag(ChoixFichiersTuiles.ORUX))
                    {
                        if (DemandeAjoutNiveau)
                        {
                            MessInfo = $"{NomComplet} :{Separateur}Ajout d'un niveau au fichier ORUX :{Separateur}";
                            if (!FichierCarteOrux.AjouterNiveauCarteOrux(this))
                                return GenererFichiersTuilesRet;
                        }
                        else
                        {
                            MessInfo = $"{NomComplet} :{Separateur}Génération du fichier ORUX :{Separateur}";
                            if (!FichierCarteOrux.CreerCarteOrux(this))
                                return GenererFichiersTuilesRet;
                        }
                    }
                }
                GenererFichiersTuilesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "G3U1");
            }
            finally
            {
                // efface le repertoire tampon fichier KML et le sous repertoire tampon des tuiles avec tous les fichiers
                if (Directory.Exists(CheminEnregistrementProvisoire + ExtensionCheminTuiles))
                    Directory.Delete(CheminEnregistrementProvisoire + ExtensionCheminTuiles, true);
            }
            return GenererFichiersTuilesRet;
        }
        /// <summary> fait un fichier kmz à partir des tuiles transmises </summary>
        private bool EcrireFichierKMZ()
        {
            bool EcrireFichierKMZRet = false;
            const string NomKML = "doc.kml";
            var STuiles = new StringBuilder();
            var SCarte = new StringBuilder();
            double N = -100.0d;
            double E = -200.0d;
            double S = 100.0d;
            double O = 200.0d;
            AfficherVisueInformation($"{MessInfo}Ecriture fichier KMZ");
            try
            {
                // ecrire la partie des tuiles dans le tampon texte
                foreach (DescriptionImageFichier T in Tuiles)
                {
                    STuiles.AppendLine("    <GroundOverlay>");
                    STuiles.AppendLine($"      <name>{T.Nom}</name>");

#if DEBUG //'Permet de vérifier que l'interpolation des cartes est correcte avec basecamp ou google earth
                    STuiles.AppendLine("      <color>80ffffff</color>");
#endif
                    STuiles.AppendLine("      <drawOrder>50</drawOrder>");
                    STuiles.AppendLine("      <Icon>");
                    STuiles.AppendLine($"        <href>files/{T.Nom}</href>");
                    STuiles.AppendLine("      </Icon>");
                    STuiles.AppendLine("      <LatLonBox>");
                    STuiles.AppendLine($"        <north>{DblToStr(T.Nord, "0.000000000")}</north>");
                    STuiles.AppendLine($"        <south>{DblToStr(T.Sud, "0.000000000")}</south>");
                    STuiles.AppendLine($"        <east>{DblToStr(T.Est, "0.000000000")}</east>");
                    STuiles.AppendLine($"        <west>{DblToStr(T.Ouest, "0.000000000")}</west>");
                    STuiles.AppendLine("      </LatLonBox>");
                    STuiles.AppendLine("    </GroundOverlay>");
                    // calcule la plus grande surface contenant l'ensemble des tuiles. normalement les coordonnées LL en DD du fichier georef
                    N = Math.Max(T.Nord, N);
                    E = Math.Max(T.Est, E);
                    S = Math.Min(T.Sud, S);
                    O = Math.Min(T.Ouest, O);
                }
                // Ecrire entête du fichier
                SCarte.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                SCarte.AppendLine("<kml xmlns=\"http://earth.google.com/kml/2.2/FCGP\">");
                SCarte.AppendLine("  <Document>");
                SCarte.AppendLine($"    <name>{Nom}.{Format}</name>");
                SCarte.AppendLine("    <open>1</open>");
                SCarte.AppendLine("    <Region> ");
                SCarte.AppendLine("      <LatLonAltBox>");
                SCarte.AppendLine($"        <north>{DblToStr(N, "0.000000000")}</north>");
                SCarte.AppendLine($"        <south>{DblToStr(S, "0.000000000")}</south>");
                SCarte.AppendLine($"        <east>{DblToStr(E, "0.000000000")}</east>");
                SCarte.AppendLine($"        <west>{DblToStr(O, "0.000000000")}</west>");
                SCarte.AppendLine("      </LatLonAltBox>");
                SCarte.AppendLine("    </Region>");
                // ajouter la description des tuiles
                SCarte.Append(STuiles);
                // si il y a eu une demande pour incorporer une ou des traces dans le fichier kmz en tant qu'objet kml
                if (NbTracesKML > 0)
                {
                    var Strace = new StringBuilder(25000 * NbTracesKML);
                    if (!DefinirTracesKML(ref Strace))
                        return EcrireFichierKMZRet;
                    // si il y a eu une ou des traces ok on les ajoute au fichier KMZ
                    if (Strace.Length > 0)
                    {
                        SCarte.Append(Strace);
                        // on rajoute un suffixe au nom pour indiquer que le fichier KMZ contient une ou des traces
                        Nom += "_KML";
                    }
                }
                // ecrire la fin du fichier kmz
                SCarte.AppendLine("  </Document>");
                SCarte.AppendLine("</kml>");
                AfficherVisueInformation($"{MessInfo}Compression des fichiers");
                // enregistrer le fichier doc.kml sur le disque dur
                File.WriteAllText(CheminEnregistrementProvisoire + ExtensionCheminTuiles + @"\" + NomKML, SCarte.ToString(), Encoding_FCGP);
                // zip les differents fichiers pour en faire un fichier kmz
                string NomArchive = CheminFichiersTuile + @"\" + NomComplet + ".kmz";
                if (File.Exists(NomArchive))
                    File.Delete(NomArchive);
                ZipFile.CreateFromDirectory(CheminEnregistrementProvisoire + ExtensionCheminTuiles, NomArchive);
                EcrireFichierKMZRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "D6W2");
            }
            return EcrireFichierKMZRet;
        }
        #endregion
        #region Traces
        /// <summary> procédure à appeler pour ajouter les points de la trace en cours dans le fichier kmz en tant que trace KML(objet de dessin) </summary>
        /// <param name="S"> Stringbuilder vide mais dimmensionée </param>
        private bool DefinirTracesKML(ref StringBuilder S)
        {
            bool FlagUnPath = false;
            bool FlagTraceOk = false;
            try
            {
                FlagUnPath = false;
                // on ajoute le style des traces KMZ au sens Google
                S.Append(TRK.AjouterStyleKML(NbTracesKML, Color.Red, 5, Color.FromArgb(252, Color.White), 8));
                foreach (string NomTrace in FichiersTRK)
                {
                    var Trace = new TRK(NomTrace, SystemCartographique.SiteCarto, SystemCartographique.DomTom, SystemCartographique.Niveau.Echelle);
                    // si la trace est tout ou en partie sur la carte on peut la dessiner
                    if (RegionVirtuelle.IntersectsWith(Trace.BoundingBoxVirtuel))
                    {
                        S.Append(TRK.AjouterTraceKML(NbTracesKML, Trace));
                        FlagTraceOk = true;
                    }
                    else
                    {
                        MessageInformation = $"La trace {Trace.Nom} n'est pas dans l'emprise de la carte {Nom}";
                        string Titre = TitreInformation;
                        TitreInformation = "Information Trace";
                        AfficherInformation();
                        TitreInformation = Titre;
                    }
                }
                if (!FlagTraceOk)
                    S.Clear();
                FlagUnPath = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A0T4");
            }
            return FlagUnPath;
        }
        /// <summary> procédure à appeler pour dessiner les traces sur la carte ou les ajouter au kmz. Uniquement dans carte interpole </summary>
        private bool AfficherTraces()
        {
            bool AfficherTracesRet = false;
            AfficherVisueInformation($"{NomComplet} :{Separateur}Dessin des traces sur la carte");

            try
            {
                var CptTrace = default(int);
                if (FichiersTRK is not null)
                {
                    // on s'assure qu'il y a bien un pinceau pour peindre la trace
                    PinceauTrace ??= new Pen(Settings.ConvertirSettings.COULEUR_TRACE, Settings.ConvertirSettings.EPAISSEUR_TRACE) { LineJoin = LineJoin.Round };

                    foreach (string NomTrace in FichiersTRK)
                    {
                        var Trace = new TRK(NomTrace, SystemCartographique.SiteCarto, SystemCartographique.DomTom, SystemCartographique.Niveau.Echelle);
                        if (RegionVirtuelle.IntersectsWith(Trace.BoundingBoxVirtuel)) // si la trace est tout ou en partie sur la carte on peut la dessiner
                        {
                            if (TamponType == TypeSupportCarte.Fichier)
                            {
                                using (var FichierBIN = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                                {
                                    int TailleTampon = LargeurOctets * HauteurReserve;
                                    int HauteurALire = 0;
                                    CheminFluxLecture = CheminEnregistrementProvisoire + @"\CarteAvecTrace.raw";
                                    using (var FichierAvecGrille = new FileStream(CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.Read, TailleTampon))
                                    {
                                        do
                                        {
                                            // on charge une partie de la carte sans grille dans le tampon
                                            FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTampon);
                                            // on dessinne la grille
                                            for (int Cpt = 0, loopTo = NbZonesUTM - 1; Cpt <= loopTo; Cpt++)
                                            {
                                                if (!this.DessinerTrace(HauteurALire, Trace.ListePointsPixel))
                                                    return AfficherTracesRet; // il peut y avoir une erreur
                                            }
                                            // on enregistrer la carte avec la grille
                                            FichierAvecGrille.Write(Tampon.Bits, Tampon.IndexBits, TailleTampon);
                                            HauteurALire += HauteurReserve;
                                            if (HauteurALire < HauteurPixel & HauteurALire + HauteurReserve > HauteurPixel)
                                            {
                                                TailleTampon = LargeurOctets * (HauteurPixel - HauteurALire);
                                            }
                                        }
                                        while (HauteurALire < HauteurPixel);
                                    }
                                }
                            }
                            else if (!DessinerTrace(0, Trace.ListePointsPixel)) // cas du suppport de carte en mémoire. Il n'y a qu'une seule tranche donc pas de décalage pour le dessin et on travaille sur la carte entière
                            {
                                return AfficherTracesRet;// il peut y avoir une erreur
                            }
                            CptTrace += 1;
                        }
                        else
                        {
                            MessageInformation = $"La trace {Trace.Nom} n'est pas dans l'emprise de la carte {Nom}";
                            string Titre = TitreInformation;
                            TitreInformation = "Information Trace";
                            AfficherInformation();
                            TitreInformation = Titre;
                        }
                    }
                }
                // on indique que la carte contient 1 ou plusieurs traces
                if (CptTrace > 0)
                    Nom += "_Trace";
                AfficherTracesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "F0I1");
            }

            return AfficherTracesRet;
        }
        /// <summary> dessine une trace sur un graphics qui représente tout ou partie de la carte </summary>
        /// <param name="DecalY"> si il y a plusieurs tranche, représente le décalage à prendre en compte  sur l'axe des y en pixels </param>
        /// <param name="PointsTrace"> points de la trace à dessiner sur la carte </param>
        private bool DessinerTrace(int DecalY, Point[] PointsTrace)
        {
            bool DessinerTraceRet = false;
            try
            {
                // On indique que l'on travaille avec la carte en cours, si l'image provient d'une carte existante on est obligé de recréer un bitmap car le bitmap existant est en lecture seul
                using (var Carte_TraceGraphic = Graphics.FromImage(Tampon.IsBitmap ? new Bitmap(LargeurPixel, HauteurReserve, LargeurOctets, FormatPixel, ImageBitPtr) : Image))
                {
                    Carte_TraceGraphic.PageUnit = GraphicsUnit.Pixel;
                    Carte_TraceGraphic.SmoothingMode = SmoothingMode.AntiAlias;
                    Carte_TraceGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    Carte_TraceGraphic.TranslateTransform(-PointOrigne.Width, -PointOrigne.Height - DecalY);
                    Carte_TraceGraphic.DrawLines(PinceauTrace, PointsTrace);
                }
                DessinerTraceRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "F8J4");
            }

            return DessinerTraceRet;
        }
        #endregion
        #region Création tuiles
        /// <summary> génère un tableau décrivant l'ensemble des tuiles et ecrit les images correspondantes dans le repertoire idoine </summary>
        private bool EcrireTuiles()
        {
            bool EcrireTuilesRet = false;
            try
            {
                // pour le calcul du geo-referencement des tuiles des fichiers tuiles JNX et KMZ. Orux n'en a pas besoin seulement la carte
                double EchelleX = (Coins[2].Longitude - Coins[0].Longitude) / LargeurPixel;
                double EchelleY = (Coins[0].Latitude - Coins[2].Latitude) / HauteurPixel;
                // Cherche le nb de tuiles en largeur et hauteur
                NbTuilesX = (int)Math.Ceiling(LargeurPixel / (double)DimensionsTuile);
                NbTuilesY = (int)Math.Ceiling(HauteurPixel / (double)DimensionsTuile);
                // il faut au moins une tuile pour faire un fichier tuile et dans le cas de IsExact = True et  IsRompu = False, il peut ne pas y en avoir
                NbIterations = 0;
                NbTotalIterations = NbTuilesX * NbTuilesY;
                AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
                Tuiles = new DescriptionImageFichier[NbTotalIterations];
                if (TamponType == TypeSupportCarte.Fichier)
                {
                    using (var FichierRaw = new FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read))
                    {
                        if (HauteurReserve < DimensionsTuile)
                        {
                            throw new Exception("La hauteur du tampon de la carte est " + CrLf + "inférieure à la hauteur de la tuile du fichier tuile");
                        }
                        int NbHauteurTuiles, TailleTamponTuiles;
                        NbHauteurTuiles = HauteurReserve / DimensionsTuile;  // Nb tuiles maximum en vertical par tranche
                        TailleTamponTuiles = LargeurOctets * NbHauteurTuiles * DimensionsTuile;
                        int DecalTuileY = 0;
                        do
                        {
                            // on charge une partie de la carte sans grille dans le tampon
                            FichierRaw.Read(Tampon.Bits, Tampon.IndexBits, TailleTamponTuiles);
                            // on génère les tuiles de la tranche
                            if (!EcrireTuilesTranche(DecalTuileY, NbHauteurTuiles, EchelleX, EchelleY))
                            {
                                return EcrireTuilesRet;
                            }
                            DecalTuileY += NbHauteurTuiles;
                            // la tranche  à venir est elle entière : NbHauteurTuiles, partielle : 1 à NbHauteurTuiles-1 ou vide : 0
                            if (DecalTuileY + NbHauteurTuiles >= NbTuilesY) // il faut recalculer la hauteur en nombre de tuile
                            {
                                NbHauteurTuiles = NbTuilesY - DecalTuileY;
                            }
                        }
                        while (DecalTuileY < NbTuilesY);
                    }
                }
                else if (!EcrireTuilesTranche(0, NbTuilesY, EchelleX, EchelleY)) // il n'y a qu'une seule tranche quand le support de carte est la mémoire
                    return EcrireTuilesRet;
                EcrireTuilesRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "D5X8");
            }
            return EcrireTuilesRet;
        }
        /// <summary> génère les tuiles de la carte transmise en fonction des dimensions fournies de l'image</summary>
        /// <remarks>si les dimensions de la carte sont plus grandes que le nb de tuiles, il y aura des petites tuiles en plus pour compléter</remarks>
        private bool EcrireTuilesTranche(int DebutTuileY, int NBY, double EchelleX, double EchelleY)
        {
            bool EcrireTuilesTrancheRet = false;
            try
            {
                int CptTuiles = DebutTuileY * NbTuilesX;
                int FinY = -1;
                int HTuile = DimensionsTuile;
                for (int CptTuilesY = DebutTuileY, loopTo = DebutTuileY + NBY - 1; CptTuilesY <= loopTo; CptTuilesY++)
                {
                    int DebutY = FinY + 1; // debut des tuiles en pixel
                    if (CptTuilesY == 0)
                    {
                        // si c'est la première tuile sur les Y C00X00Y ou ligne 0, le nord égal à la latitude du coins 0
                        Tuiles[CptTuiles] = new DescriptionImageFichier() { Nord = Coins[0].Latitude };
                    }
                    else
                    {
                        // sinon les données sont les mêmes hormis nord  = sud de la tuile précedente le sud et la hauteur qui dépendent si c'est la dernière tuile de la colonne
                        Tuiles[CptTuiles] = new DescriptionImageFichier() { Nord = Tuiles[CptTuiles - NbTuilesX].Sud };
                    }
                    if (CptTuilesY == NbTuilesY - 1)
                    {
                        HTuile = HauteurPixel - CptTuilesY * HTuile; // la dernière ligne est différente si les rompus sont acceptés
                        Tuiles[CptTuiles].Sud = Coins[2].Latitude;
                    }
                    else
                    {
                        FinY += HTuile;
                        Tuiles[CptTuiles].Sud = Tuiles[CptTuiles].Nord - HTuile * EchelleY;
                    }
                    Tuiles[CptTuiles].HauteurPixels = HTuile;
                    // boucle sur la largeur de la carte
                    int FinX = -1;
                    int LTuile = DimensionsTuile;
                    for (int CptTuilesX = 0, loopTo1 = NbTuilesX - 1; CptTuilesX <= loopTo1; CptTuilesX++)
                    {
                        int DebutX = FinX + 1;
                        if (CptTuilesX == 0)
                        {
                            // si c'est la première tuile sur les X  C00000Y ou colonne 0, l'ouest égal à la longitude du coins 0
                            Tuiles[CptTuiles].Ouest = Coins[0].Longitude;    // la tuile est déjà créée dans la boucle Y
                        }
                        else
                        {
                            // sinon l'ouest égal l'est de la colonne précédente et les latitudes et la hauteur aussi
                            // il faut créer la tuile
                            Tuiles[CptTuiles] = new DescriptionImageFichier()
                            {
                                Ouest = Tuiles[CptTuiles - 1].Est,
                                Nord = Tuiles[CptTuiles - 1].Nord,
                                Sud = Tuiles[CptTuiles - 1].Sud,
                                HauteurPixels = Tuiles[CptTuiles - 1].HauteurPixels
                            };
                        }
                        if (CptTuilesX == NbTuilesX - 1)
                        {
                            LTuile = LargeurPixel - CptTuilesX * LTuile; // la dernière colonne est différente si les rompus sont acceptés
                            Tuiles[CptTuiles].Est = Coins[2].Longitude;
                        }
                        else
                        {
                            FinX += LTuile;
                            Tuiles[CptTuiles].Est = Tuiles[CptTuiles].Ouest + LTuile * EchelleX;
                        }
                        Tuiles[CptTuiles].LargeurPixels = LTuile;
                        // calcule le nom de la tuile
                        Tuiles[CptTuiles].Nom = "C" + CptTuilesX.ToString("000") + CptTuilesY.ToString("000") + ".jpg";
                        // déclaration de la tuile en tant qu'image jpeg
                        using (var b = new Bitmap(LTuile, HTuile, Tampon.Stride, FormatPixel, Tampon.BitPtr + DebutX * NbOctetsPixel + DebutY * Tampon.Stride))
                        {
                            b.Save(CheminEnregistrementProvisoire + ExtensionCheminImagesTuiles + @"\" + Tuiles[CptTuiles].Nom, ImageFormat.Jpeg);
                        }
                        CptTuiles += 1;
                        Interlocked.Increment(ref NbIterations);
                        if (NbIterations % 100 == 0)
                            AfficherVisueInformation($"{MessInfo + NbIterations} / {NbTotalIterations}");
                    }
                }
                EcrireTuilesTrancheRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "P9G3");
            }

            return EcrireTuilesTrancheRet;
        }
        #endregion
        #region Création Carte existante à partir fichier Georef
        /// <summary>Lit un fichier georef et remplit toutes les variables de la carte hormis le système cartographique
        /// ne prend pas en compte la première version du format (geoportail)</summary>
        /// <param name="LignesFichierGeoref">lignes du fichier Georef à lire</param>
        private bool LireFichierGeoref(string[] LignesFichierGeoref)
        {
            bool Ret = false;
            try
            {
                int Cpt;
                Cpt = LignesFichierGeoref[2].IndexOf(',') + 1;
                GrilleIsAffiche = true;
                if (TrouverChaineCompriseEntre(LignesFichierGeoref[2], ref Cpt, ':', ',') == "Non")
                    GrilleIsAffiche = false;
                for (int Boucle = 0; Boucle <= 3; Boucle++)
                {
                    Cpt = 0;
                    Coins[Boucle].CoordonneesCaptureEcran = TrouverChaineCompriseEntre(LignesFichierGeoref[3 * Boucle + 3], ref Cpt, ':');
                    // remplir toutes les variables associées aux coordonnées Grille et LL de la structure Georef de 4 coins
                    if (!DecomposerCoordonneesCapturees(SystemCartographique, Coins[Boucle]))
                        return false;
                    // lire les coordonnées en pixel de chaque coins
                    Cpt = 0;
                    Coins[Boucle].X_Pixel = int.Parse(TrouverChaineCompriseEntre(LignesFichierGeoref[3 * Boucle + 5], ref Cpt, ':', ','));
                    Coins[Boucle].Y_Pixel = int.Parse(TrouverChaineCompriseEntre(LignesFichierGeoref[3 * Boucle + 5], ref Cpt, ':'));
                }
                // remplir toutes les variables associées aux coordonnées UTM de la structure Georef de 4 coins
                TrouverNbZonesUtm();
                LargeurPixel = Coins[2].X_Pixel;
                HauteurPixel = Coins[2].Y_Pixel;
                LargeurOctets = StrideImage(LargeurPixel);
                HauteurReserve = HauteurPixel;
                // pour les cartes issues d'une capture d'écran on peut faire un monde virtuel (échelle)
                // pour les autres l'origine du monde virtuel est le pt(0) car on ne peut pas faire d'échelle (positionnement relatif des cartes les unes par rapport au autres)
                if (SystemCartographique.IsCapture)
                {
                    if (SystemCartographique.CoordonneesGeoreferencement == CoordonneesGeoreferencements.Grille)
                    {
                        PointOrigne = new Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins[0].Grille));
                    }
                    else
                    {
                        PointOrigne = new Size(SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(Coins[0].LatLon));
                    }
                }
                RegionVirtuelle = new Rectangle(new Point(PointOrigne), new Size(LargeurPixel, HauteurPixel));
                // on calcule divers paramètres afin de connaître l'échelle de la carte et les DPI correspondants
                TrouverDPIImpression();
                Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A3A1");
            }
            return Ret;
        }
        /// <summary> Ouvre un fichier de type GEOREF et vérifie un certain nombre de paramètres avant de le déclarer valide. 
        /// notatement en vérifiant que le système carto du fichier est compatible avec le système passé en paramètre </summary>
        /// <param name="LignesFichierGeoref">Les lignes de texte du fichier de type GEOREF de la carte</param>
        /// <param name="SystemeCarto">Données concernant le système géographique à verifier dan le fichier georef, normalement celui d'une échelle</param>
        /// <returns>VerifSystemeGeographique.Ok si le fichier GEOREF est valide ou VerifSystemeGeographique.xx .</returns>
        private VerificationRenvoiCarte VerifierGeoref(string[] LignesFichierGeoref, ref SystemeCartographique SystemeCarto)
        {
            // Si ce n'est pas le bon fichier (2 formats GEOREF cohabite)
            if (LignesFichierGeoref[0][1] != 'C')
                return VerificationRenvoiCarte.Georef_Non_Supporte;
            var Ret = VerificationRenvoiCarte.OK;
            // verification communes aux 2 manières (création echelle et ajout carte)
            var Cpt = default(int);
            // Trouve le nom du fichier de l'image associée à la carte sur la 1ère ligne du fichier georef
            CheminFluxLecture = TrouverChaineCompriseEntre(LignesFichierGeoref[0], ref Cpt, ':', ',');
            // Est ce que fichier graphique existe dans le même répertoire que le fichier GEOREF
            if (!File.Exists((string.IsNullOrEmpty(CheminCarte) ? "" : CheminCarte + @"\") + CheminFluxLecture))
                return VerificationRenvoiCarte.Carte_Inexistante;
            string SiteCarto = null;
            string Projection = null;
            string Echelle = null;
            int Z = 0;
            char H = default;
            bool Interpol = false;
            TrouverElementsSystemeCartographique(LignesFichierGeoref, ref SiteCarto, ref Echelle, ref Projection, ref Z, ref H, ref Interpol);
            if (Interpol)
            {
                Ret = VerificationRenvoiCarte.Carte_Retaillee;
            }
            else
            {
                SystemeCarto = new SystemeCartographique(SiteCarto, Echelle, Projection, Z, H);
                if (!SystemeCarto.IsOk)
                    Ret = VerificationRenvoiCarte.Erreur_Creation_Systeme;
            }
            return Ret;
        }
        /// <summary> renvoie les 6 élements qui permettent de créer un système cartograhique à partir d'un fichier georef </summary>
        /// <param name="LignesFichierGeoref">textez du fichier georef</param>
        /// <param name="SiteCarto">chaine qui contiendra le libellé du site carto</param>
        /// <param name="ClefEchelle">chaine qui contiendra la cle de l'echelle de la carte</param>
        /// <param name="LibelleProjection">chaine qui contiendra le libellé de la projection</param>
        /// <param name="Hemis">char qui contiendra l'hemisphère de la projection si celle ci est UTM</param>
        /// <param name="Zone">long qui contiendra le numéro de zone de la projection si celle ci est UTM</param>
        /// <param name="Interpol">boolean qui indiquera si la carte est interpolée</param>
        private static void TrouverElementsSystemeCartographique(string[] LignesFichierGeoref, ref string SiteCarto, ref string ClefEchelle, ref string LibelleProjection, ref int Zone, ref char Hemis, ref bool Interpol)
        {
            int Cpt = LignesFichierGeoref[0].IndexOf('.') + 1;
            // Trouve le système de capture de la carte
            SiteCarto = TrouverChaineCompriseEntre(LignesFichierGeoref[0], ref Cpt, ':');
            // Trouve l'échelle de capture de la carte sur la deuxième ligne du fichier georef
            Cpt = 0;
            ClefEchelle = TrouverChaineCompriseEntre(LignesFichierGeoref[1], ref Cpt, ':', ',');
            // Trouve le type de coordonnées capturées de la carte sur la deuxième ligne du fichier georef
            Cpt = LignesFichierGeoref[1].IndexOf(", C");
            LibelleProjection = TrouverChaineCompriseEntre(LignesFichierGeoref[1], ref Cpt, ':');
            Zone = 0;
            Hemis = default;
            Interpol = true;
            int argPosCar1 = 0;
            if (TrouverChaineCompriseEntre(LignesFichierGeoref[2], ref argPosCar1, ':', ',') == "Non")
                Interpol = false;
            if (LibelleProjection[0] == 'U') // si il s'agit d'une projection UTM il faut trouver la zone et l'hemisphère
            {
                string[] CH = LibelleProjection.Split(' ');
                if (CH.Length > 2)
                {
                    LibelleProjection = CH[0] + " " + CH[1];
                    Hemis = CH[2][0];
                    Zone = int.Parse(CH[3]);
                }
            }
        }
        /// <summary> vérifie la faisabilité d'associer un tampon à une carte et charge l'image si besoin</summary>
        /// <remarks>ReserveSource ou interpole doit être initialisé avant d'appeler cette procédure lorqu'il s'agit d'un support fichier</remarks>
        private VerificationRenvoiCarte AssocierTampon()
        {
            ReserverMemoireTampon("Reserve Carte Base");
            if (TamponType != TypeSupportCarte.Fichier)
            {
                Tampon = new EditableBitmap(CheminFluxLecture);
                if (!Tampon.IsOk)
                    return VerificationRenvoiCarte.Erreur_Creation_Tampon;
            }
            else
            {
                // la hauteur max du tampon de travail dépend de la taille effective de la réserve
                HauteurReserve = TailleReserve / LargeurOctets;
                // avec un support de carte fichier on a besoin d'un tampon spécifique 
                Tampon = new EditableBitmap(LargeurPixel, HauteurReserve, Reserve, "Tampon Base"); // on dimensionne le nouveau tampon
                if (!Tampon.IsOk)
                    return VerificationRenvoiCarte.Erreur_Creation_Tampon;
                // si le support est Fichier, l'image de la carte doit être transformée en .bin et le fichier indiquer dans CheminFluxLecture
                // transformation de l'image de la carte en fichier bin
                string Dst = CheminEnregistrementProvisoire + @"\" + "Carte.raw";
                if (Format == ImageFormat.Bmp) // pas d'utilisation superflue de mémoire
                {
                    if (!FichierSrcToDst(Dst, false))
                        return VerificationRenvoiCarte.Erreur_Chargement_Image;
                }
                else if (!FichierFormatsToRaw(Dst))
                    return VerificationRenvoiCarte.Erreur_Chargement_Image;
                CheminFluxLecture = Dst;
            }
            return VerificationRenvoiCarte.OK;
        }
        /// <summary> reserve des octets en mémoire qui serviront pour le tampon (image de la carte) ou pour l'écriture dans un fichier
        private bool ReserverMemoireTampon(string NomReserve)
        {
            // Si egal à 0 cela indique une demande de Capturer ou de Convertir
            if (TailleReserve == 0)
            {
                // pour le support de tampon type fichier le plus grand besoin se situe au niveau du découpage des tuiles du fichier tuile soit une hauteur mini de 256,512 ou 1024
                DimensionsTuile = DemandeFichiersTuiles > ChoixFichiersTuiles.Aucun ? Settings.CartesSettings.DIMENSIONS_TUILE : DonneesSiteWeb.NbPixelsTuile;
                // ce qui limite pour le support fichier à 682 tuiles sur l'axe des tuiles sinon on dépasse TailleMaxTampon
                HauteurReserve = TamponType == TypeSupportCarte.Fichier ? DimensionsTuile : HauteurPixel;
                // on détermine d'abord si il y a un interpolation avec une carte suisse car il faut réserver plus de mémoire pour le support fichier
                bool InterpolerCarteSuisse = SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile && DemandeFichiersTuiles > ChoixFichiersTuiles.ORUX;
                // pour une interpolation de carte suisse il faut que la réserve puisse contenir le maximum de la hauteur de l'image
                TailleReserve = LargeurOctets * (InterpolerCarteSuisse ? Math.Max(HauteurReserve, HauteurPixel) : HauteurReserve);
                // mais on limite à tailleMaxTampon d'où la limite sur le nb de colonnes à 410 tuiles serveur car la hauteur n'à pas d'influence
                // en effet plus il y a de largeur et plus le nb de ligne source est important pour pouvoir réaliser l'interpolation
                if (TailleReserve > TailleMaxTampon)
                    TailleReserve = TailleMaxTampon;
            }
            // on reserve la mémoire.
            Reserve = new SharedPinnedByteArray(TailleReserve, 1, null, NomReserve);
            if (Reserve.IsOk)
                Reserve.ClearColor(Settings.PartagerSettings.COULEUR_CARTE);
            return Reserve.IsOk;
        }
        #endregion
    }
}