using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.SystemeCartographique;

namespace FCGP
{
    /// <summary> permet la gestion d'une carte au format ORUX. On suppose que la base de données est toujours en concordance avec le fichier XML </summary>
    internal static class FichierCarteOrux
    {
        #region Public
        /// <summary> Lit les tuiles d'un niveau du fichier ORUX et les écrit dans un répertoire </summary>
        /// <param name="FichierCarteOrux"> chemin du fichier otrk2.xml de description de la carte </param>
        /// <param name="Echelle"> Echelle dont on veut récupérer les tuiles </param>
        /// <param name="CheminTuiles"> répertoire où l'on veut enregister les tuiles </param>
        /// <remarks> cette fonction est donnée à titre d'exemple. Elle ne sert pas dans FCGP </remarks>
        internal static bool LireNiveauCarteOrux(string FichierCarteOrux, Echelles Echelle, string CheminTuiles)
        {
            // vérifier que le fichier est correct
            var InfosOrux = VerifierCarteORUX(FichierCarteOrux);
            if (InfosOrux.NbNiveauxDetail == -1)
                return false;
            try
            {
                CheminORUX = Path.GetDirectoryName(FichierCarteOrux) + @"\";
                using (var BDSqlite = new BDTuilesOruxMaps(CheminORUX, false)) // on ouvre la base sqlite associée à la carte
                {
                    if (BDSqlite.Isok)
                    {
                        for (int Cpt = 0, loopTo = InfosOrux.NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
                        {
                            var EchelleCapture = SiteIndiceEchelleToEchelle(InfosOrux.SiteCapture, InfosOrux.IndiceEchelleCapture[Cpt]);
                            if (EchelleCapture == Echelle)
                            {
                                return BDSqlite.LireNiveauFichier(InfosOrux.IndiceAffichage[Cpt], CheminTuiles);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "224E");
            }
            return false;
        }
        /// <summary>  change le niveau d'affichage d'un niveau</summary>
        /// <param name="CheminCarteOrux"> chemin du fichier otrk2.xml de description de la carte</param>
        /// <param name="IndexNiveau">index du niveau de détail concerné par la modification</param>
        /// <param name="IndiceAffichageModifie">valeur à ajouter à l'indice d'affichage. Si positif le niveau apparait à une échelle plus petite</param>
        internal static bool ChangerNiveauAffichageCarteOrux(string CheminCarteOrux, int IndexNiveau, int IndiceAffichageModifie)
        {
            bool ChangerNiveauAffichageCarteOruxRet = false;
            // vérifier que le fichier est correct
            var InfosOrux = VerifierCarteORUX(CheminCarteOrux);
            if (InfosOrux.NbNiveauxDetail == -1)
                return ChangerNiveauAffichageCarteOruxRet;
            // trouver le repertoire lié à la carte
            CheminORUX = Path.GetDirectoryName(CheminCarteOrux) + @"\";
            // le fichier comporte 2 extensions d'où l'utilisation de la méthode GetFileNameWithoutExtension 2 fois
            NomORUX = InfosOrux.NomCarte;
            try
            {
                using (var BDSqlite = new BDTuilesOruxMaps(CheminORUX, false)) // on ouvre la base sqlite associée à la carte
                {
                    if (BDSqlite.Isok)
                    {
                        string[] LignesORUX = File.ReadAllLines(CheminCarteOrux, Encoding_FCGP);
                        string[] EnteteORUX = RenvoyerEnteteORUX_XML(LignesORUX);
                        var NiveauORUX = new string[InfosOrux.NbNiveauxDetail][];
                        for (int Cpt = 0, loopTo = InfosOrux.NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
                        {
                            NiveauORUX[Cpt] = RenvoyerNiveauORUX_XML(LignesORUX, Cpt);
                            if (Cpt == IndexNiveau)
                            {
                                ChangerNiveauAffichageORUX_XML(NiveauORUX[Cpt], InfosOrux.IndiceAffichage[Cpt], IndiceAffichageModifie); // change dans le texte xml
                                BDSqlite.ChangerNiveauAffichage(InfosOrux.IndiceAffichage[Cpt], IndiceAffichageModifie); // change dans la base de données
                            }
                        }
                        // créer le fichier XML
                        var S = new StringBuilder(1500);
                        // écriture de l'entête
                        for (int CptLigne = 0; CptLigne <= NbLignesEnteteOrux - 1; CptLigne++)
                            S.AppendLine(EnteteORUX[CptLigne]);
                        // écriture du ou des niveaux
                        for (int CptNiveau = 0, loopTo1 = InfosOrux.NbNiveauxDetail - 1; CptNiveau <= loopTo1; CptNiveau++)
                        {
                            for (int CptLigne = 0; CptLigne <= NbLignesNiveauOrux - 1; CptLigne++)
                                S.AppendLine(NiveauORUX[CptNiveau][CptLigne]);
                        }
                        // ajouter la fin de la carte
                        EcrireBaliseFinEnteteORUX_XML(S);
                        // ecrire le fichier XML au bon endroit
                        File.WriteAllText(CheminCarteOrux, S.ToString(), Encoding_FCGP);
                        ChangerNiveauAffichageCarteOruxRet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "F9Z1");
            }

            return ChangerNiveauAffichageCarteOruxRet;
        }
        /// <summary>  verifie l'integrité du fichier otrk2.xml telque FCGP Version 5 doit les écrire</summary>
        /// <param name="CheminCarteOrux">chemin complet du fichier OTRK</param>
        /// <remarks> on ne verifie que le fichier TRK2 et pas la base de données</remarks>
        internal static InfoORUX VerifierCarteORUX(string CheminCarteOrux)
        {
            InfoORUX VerifierCarteORUXRet = new()
            {
                NbNiveauxDetail = -1,
                SiteCapture = SitesCartographiques.Aucun
            };

            try
            {
                string[] LignesORUX = File.ReadAllLines(CheminCarteOrux, Encoding_FCGP);
                int NbNiveaux = TrouverNbNiveauORUX_XML(LignesORUX);
                if (NbNiveaux == 0)
                    return VerifierCarteORUXRet;
                InfoORUX InOr = new()
                {
                    NbNiveauxDetail = NbNiveaux,
                    IndiceEchelleCapture = new int[NbNiveaux],
                    IndiceAffichage = new int[NbNiveaux],
                    NiveauDetail = new string[NbNiveaux]
                };

                string[] S = RenvoyerEnteteORUX_XML(LignesORUX);
                if (S is null)
                    return VerifierCarteORUXRet;
                string ClefSite = null;
                DecomposerEnteteFichierORUX_XML(S[3], ref InOr.SiteCapture, ref InOr.DomTom, ref InOr.NomCarte, ref ClefSite);

                if (InOr.SiteCapture == SitesCartographiques.Aucun)
                    return VerifierCarteORUXRet;
                for (int Cpt = 0, loopTo = NbNiveaux - 1; Cpt <= loopTo; Cpt++)
                {
                    S = RenvoyerNiveauORUX_XML(LignesORUX, Cpt);
                    if (S is null)
                        return VerifierCarteORUXRet;
                    string ClefEchelle = null;
                    DecomposerNiveauFichierOrux(S[2], InOr.SiteCapture, ref ClefEchelle, ref InOr.IndiceEchelleCapture[Cpt]);
                    if (InOr.IndiceEchelleCapture[Cpt] == -1)
                        return VerifierCarteORUXRet;
                    InOr.IndiceAffichage[Cpt] = TrouverNiveauAffichageORUX_XML(S);
                    InOr.NiveauDetail[Cpt] = ClefSite + "-" + ClefEchelle;
                }
                VerifierCarteORUXRet = InOr;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "Z5H0");
            }
            return VerifierCarteORUXRet;
        }
        /// <summary> strucutre de description du fichier </summary>
        internal struct InfoORUX
        {
            internal SitesCartographiques SiteCapture;
            internal DomsToms DomTom;
            internal int NbNiveauxDetail;
            internal int[] IndiceEchelleCapture;
            /// <summary> valeur par pas de 1 indiquant à quelle échelle d'affichage le niveau de détail apparait </summary>
            internal int[] IndiceAffichage;
            internal string[] NiveauDetail;
            internal string NomCarte;
        }
        /// <summary>créer une carte oruxmaps avec un niveau de détail à partir d'une carte</summary>
        /// <param name="CarteActuelle">carte servant de support aux tuiles</param>
        /// <remarks>n'est accessible que pour la fonction carte.realiserdemandes</remarks>
        internal static bool CreerCarteOrux(Carte CarteActuelle)
        {
            bool CreerCarteOruxRet = false;
            try
            {
                CarteActuelle.NbIterations = 0;
                CarteActuelle.NbTotalIterations = CarteActuelle.Tuiles.Length;
                AfficherVisueInformation($"{CarteActuelle.MessInfo + CarteActuelle.NbIterations} / {CarteActuelle.NbTotalIterations}");
                // créer le repertoire lié à la carte
                CheminORUX = CarteActuelle.CheminFichiersTuile + @"\" + CarteActuelle.Nom + CarteActuelle.NomAjout + @"\";
                NomORUX = CarteActuelle.Nom;  // nom de la carte
                Directory.CreateDirectory(CheminORUX);
                // Créer la base SQLITE oruximages
                using (var BDSqlite = new BDTuilesOruxMaps(CheminORUX, true))
                {
                    if (BDSqlite.Isok)
                    {
                        // ecrire les tuiles du premier niveau dans la base orux
                        BDSqlite.EcrireNiveauFichier(CheminEnregistrementProvisoire + Carte.ExtensionCheminImagesTuiles + @"\", CarteActuelle);
                        // créer le fichier XML
                        var S = new StringBuilder(1500);
                        // ajouter l'entête de la carte
                        EcrireEnteteORUX_XML(CarteActuelle.SystemCartographique, S);
                        // ajouter le fichier de description du premier niveau
                        EcrireNiveauORUX_XML(CarteActuelle, S);
                        // ajouter la fin de la carte
                        EcrireBaliseFinEnteteORUX_XML(S);
                        // ecrire le fichier XML au bon endroit et avec le bon nom
                        NomORUX += CarteActuelle.NomAjout;
                        File.WriteAllText(CheminORUX + NomORUX + ExtensionORUX, S.ToString(), Encoding_FCGP);
                        CreerCarteOruxRet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T1P0");
            }

            return CreerCarteOruxRet;
        }
        /// <summary> ajoute un niveau de détail à la carte oruxmap à partir d'une carte avec une échelle de capture differente</summary>
        /// <param name="CarteActuelle">carte qui contient le niveau à ajouter</param>
        /// <returns>true si tout c'est bien passé</returns>
        /// <remarks>aucune vérif n'est faite quant aux dimensions du nouveau niveau par rapport à celles de la carte orux. n'est accessible que pour la fonction carte.realiserdemandes</remarks>
        internal static bool AjouterNiveauCarteOrux(Carte CarteActuelle)
        {
            bool AjouterNiveauCarteOruxRet = false;
            CarteActuelle.NbIterations = 0;
            CarteActuelle.NbTotalIterations = CarteActuelle.Tuiles.Length;
            AfficherVisueInformation($"{CarteActuelle.MessInfo + CarteActuelle.NbIterations} / {CarteActuelle.NbTotalIterations}");
            string CheminCarteOrux = CarteActuelle.CheminFichiersTuile + @"\" + CarteActuelle.Nom + CarteActuelle.NomAjout + @"\" + CarteActuelle.Nom + CarteActuelle.NomAjout + ".otrk2.xml";
            var InfORUX = VerifierCarteORUX(CheminCarteOrux); // chemin complet du fichier otrk2.xml
            if (InfORUX.NbNiveauxDetail == -1)
                return AjouterNiveauCarteOruxRet;
            if (InfORUX.SiteCapture != CarteActuelle.SystemCartographique.SiteCarto)
                return AjouterNiveauCarteOruxRet;
            for (int Cpt = 0, loopTo = InfORUX.NbNiveauxDetail - 1; Cpt <= loopTo; Cpt++)
            {
                int IndiceEchelleSite = EchelleToSiteIndiceEchelle(CarteActuelle.SystemCartographique.SiteCarto, CarteActuelle.SystemCartographique.Niveau.Echelle);
                if (InfORUX.IndiceEchelleCapture[Cpt] == IndiceEchelleSite)
                    return false; // on ne rajoute pas le niveau de détail si il existe déjà
            }

            CheminORUX = Path.GetDirectoryName(CheminCarteOrux) + @"\";  // trouver le repertoire lié à la carte
            try
            {
                using (var BDSqlite = new BDTuilesOruxMaps(CheminORUX, false))
                {
                    if (BDSqlite.Isok) // si l'ouverture est ok
                    {
                        string[] LignesORUX = File.ReadAllLines(CheminCarteOrux, Encoding_FCGP);
                        // créer le nouveau fichier XML à partir de l'ancien
                        var S = new StringBuilder(1500);
                        // écriture du fichier sans la fin de la carte
                        for (int CptLigne = 0, loopTo1 = LignesORUX.Length - NbLignesFinOrux - 1; CptLigne <= loopTo1; CptLigne++)
                            S.AppendLine(LignesORUX[CptLigne]);
                        // créer le répertoire qui va recevoir les tuiles du nouveau niveau de détail
                        string CheminTuiles = CheminEnregistrementProvisoire + Carte.ExtensionCheminImagesTuiles + @"\";
                        // ecrire les tuiles du niveau dans la base orux
                        BDSqlite.EcrireNiveauFichier(CheminTuiles, CarteActuelle);
                        // ajouter le fichier de description du premier niveau soit à partir de la carte passée en paramètre soit de son interpolation si elle a été demandée
                        NomORUX = InfORUX.NomCarte;
                        EcrireNiveauORUX_XML(CarteActuelle, S);
                        // ajouter la fin de la carte
                        EcrireBaliseFinEnteteORUX_XML(S);
                        // ecrire le fichier XML au bon endroit
                        File.WriteAllText(CheminCarteOrux, S.ToString(), Encoding_FCGP);
                        AjouterNiveauCarteOruxRet = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A6C7");
            }

            return AjouterNiveauCarteOruxRet;
        }
        #endregion
        #region Private
        private static string NomORUX;
        private static string CheminORUX;
        internal const string ExtensionORUX = ".otrk2.xml";
        private const int NbLignesNiveauOrux = 18;
        private const int NbLignesEnteteOrux = 4;
        private const int NbLignesFinOrux = 2;
        /// <summary>  trouve le nb de niveau de la carte en fonction du nb de lignes du fichier otrk2.xml</summary>
        private static int TrouverNbNiveauORUX_XML(string[] LignesOrux)
        {
            int TrouverNbNiveauORUX_XMLRet;
            if ((LignesOrux.Length - NbLignesEnteteOrux - NbLignesFinOrux) % NbLignesNiveauOrux != 0)
            {
                TrouverNbNiveauORUX_XMLRet = 0;
            }
            else
            {
                TrouverNbNiveauORUX_XMLRet = (LignesOrux.Length - NbLignesEnteteOrux - NbLignesFinOrux) / NbLignesNiveauOrux;
            }

            return TrouverNbNiveauORUX_XMLRet;
        }
        /// <summary> trouve l'échelle et la clé correspondante d'un niveau d'une carte ORUX </summary>
        /// <param name="Niveau"> Texte correspondant du niveau </param>
        /// <param name="SiteCapture"> site correspondant à la carte </param>
        /// <param name="ClefEchelle"> Cle de l'échelle </param>
        /// <param name="Echelle"> indice de l'échelle </param>
        private static void DecomposerNiveauFichierOrux(string Niveau, SitesCartographiques SiteCapture, ref string ClefEchelle, ref int Echelle)
        {
            int Pos = PosSeparateur(ref Niveau);
            ClefEchelle = Niveau[..Pos];
            Echelle = EchelleClefToSiteIndiceEchelle(SiteCapture, ClefEchelle);
        }
        /// <summary> renvoie la chaine comprise entre les balises </summary>
        /// <param name="Texte">chaine à nettoyer </param>
        /// <returns> la chaine netoyée et la position du séparateur '-' </returns>
        private static int PosSeparateur(ref string Texte)
        {
            int Pos = Texte.IndexOf("<![CDATA[");
            int Pos1 = Texte.IndexOf("]]></MapName>");
            Texte = Texte.Substring(Pos + 9, Pos1 - Pos - 9);
            return Texte.IndexOf('-');
        }
        /// <summary>  renvoie le niveau d'affichage d'un niveau</summary>
        private static int TrouverNiveauAffichageORUX_XML(string[] NiveauOruxXML)
        {
            int Pos = NiveauOruxXML[1].IndexOf("layerLevel=\"");
            int Pos1 = NiveauOruxXML[1].IndexOf('"', Pos + 13);
            return int.Parse(NiveauOruxXML[1].Substring(Pos + 12, Pos1 - Pos - 12));
        }
        /// <summary> renvoie le site de capture ayant servi pour la création de la carte ORUX </summary>
        private static void DecomposerEnteteFichierORUX_XML(string Entete, ref SitesCartographiques SiteCarto, ref DomsToms DomTom, ref string NomCarte, ref string ClefSite)
        {
            int Pos = PosSeparateur(ref Entete);
            NomCarte = Entete[(Pos + 1)..];
            ClefSite = Entete[..Pos];
            SiteCarto = TrouverSiteCartographiqueCle(ClefSite);
            DomTom = DomsToms.aucun;
            if (SiteCarto == SitesCartographiques.Aucun)
            {
                DomTom = TrouverDomTomCle(ClefSite);
                if (DomTom > DomsToms.aucun)
                {
                    SiteCarto = SitesCartographiques.DomTom;
                }
                else
                {
                    throw new Exception("Fichier Orux corrompu");
                }
            }
        }
        /// <summary> renvoie les lignes de texte d'un niveau d'un fichier OTRK</summary>
        private static string[] RenvoyerNiveauORUX_XML(string[] LignesOrux, int Niveau)
        {
            var S = new string[18];
            int Depart = Niveau * NbLignesNiveauOrux + NbLignesEnteteOrux;
            for (int Cpt = 0; Cpt <= NbLignesNiveauOrux - 1; Cpt++)
                S[Cpt] = LignesOrux[Cpt + Depart];
            if (!S[0].StartsWith("<OruxTracker"))
                return null;
            if (!S[NbLignesNiveauOrux - 1].EndsWith("</OruxTracker>"))
                return null;
            return S;
        }
        /// <summary> renvoie les lignes de texte de  l'entête d'un fichier OTRK</summary>
        private static string[] RenvoyerEnteteORUX_XML(string[] LignesOrux)
        {
            var S = new string[NbLignesEnteteOrux];
            for (int Cpt = 0; Cpt <= NbLignesEnteteOrux - 1; Cpt++)
                S[Cpt] = LignesOrux[Cpt];
            if (!S[NbLignesEnteteOrux - 1].EndsWith("</MapName>"))
                return null;
            return S;
        }
        /// <summary> change le niveau d'affichage d'un niveau  d'un fichier OTRK</summary>
        /// <param name="NiveauOruxXML">ligne de texte du niveau à modifier</param>
        /// <param name="Ancien">valeur actuelle du niveau. 3 endroits à trouver par niveau</param>
        /// <param name="Nouveau">valeur future</param>
        private static void ChangerNiveauAffichageORUX_XML(string[] NiveauOruxXML, int Ancien, int Nouveau)
        {
            NiveauOruxXML[1] = NiveauOruxXML[1].Replace(Ancien.ToString(), Nouveau.ToString());
            NiveauOruxXML[2] = NiveauOruxXML[2].Replace(NomORUX + " " + Ancien.ToString(), NomORUX + " " + Nouveau.ToString());
            NiveauOruxXML[7] = NiveauOruxXML[7].Replace(NomORUX + " " + Ancien.ToString(), NomORUX + " " + Nouveau.ToString());
        }
        /// <summary> ecrit les lignes de texte de la fin  d'un fichier OTRK</summary>
        private static void EcrireBaliseFinEnteteORUX_XML(StringBuilder S)
        {
            // fin de la carte
            S.AppendLine("</MapCalibration>");
            S.AppendLine("</OruxTracker>");
        }
        /// <summary> écrit les lignes de texte de l'entête d'un fichier OTRK</summary>
        private static void EcrireEnteteORUX_XML(SystemeCartographique SysCarto, StringBuilder S)
        {
            // entête de la carte
            S.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            S.AppendLine("<OruxTracker xmlns:orux=\"http://oruxtracker.com/app/res/calibration\" versionCode=\"3.0\">");
            S.AppendLine("<MapCalibration layers=\"true\" layerLevel=\"0\">");
            string Clef = SysCarto.SiteCarto == SitesCartographiques.DomTom ? DomTomClef(SysCarto.DomTom) : SiteCartoClef(SysCarto.SiteCarto);
            S.AppendLine("<MapName><![CDATA[" + Clef + "-" + NomORUX + "]]></MapName>");
        }
        /// <summary> écrit les lignes de texte d'un niveau d'un fichier OTRK</summary>
        private static void EcrireNiveauORUX_XML(Carte CarteActuelle, StringBuilder S)
        {
            const string Format = "0.000000000000";
            string Projection, Datum;
            if (CarteActuelle.SystemCartographique.IsInterpol)
            {
                // initalisation Datum et projection ORUX pour les cartes interpolées
                Projection = "Latitude/Longitude,";
                Datum = "WGS 1984:Global Definition";
            }
            // si la carte concerne un site de capture dont la projection n'est pas Latitude/Longitude on change le datum et la projection attendue par ORUX
            else if (CarteActuelle.SystemCartographique.SiteCarto == SitesCartographiques.SuisseMobile)
            {
                Projection = "(SUI) Swiss Grid,";
                Datum = "CH-1903:Swiss";
            }
            else
            {
                Projection = "Mercator,0.0";
                Datum = "WGS 1984:Global Definition";
            }
            // entête niveau
            S.AppendLine("<OruxTracker versionCode= \"2.1\">");
            S.AppendLine("<MapCalibration layers=\"False\" layerLevel=\"" + CarteActuelle.FacteurORUX + "\">");
            // definition globale du niveau
            S.AppendLine("<MapName><![CDATA[" + CarteActuelle.SystemCartographique.Niveau.Clef + "-" + NomORUX + " " + CarteActuelle.FacteurORUX + "]]></MapName>");
            S.AppendLine("<MapChunks xMax=\"" + CarteActuelle.NbTuilesX + "\" yMax=\"" + CarteActuelle.NbTuilesY + "\"");
            S.AppendLine(Tab + "datum=\"" + Datum + "@WGS 1984: Global Definition\"");
            S.AppendLine(Tab + "projection=\"" + Projection + "\"");
            S.AppendLine(Tab + "img_height=\"" + CarteActuelle.DimensionsTuile + "\" img_width=\"" + CarteActuelle.DimensionsTuile + "\"");
            S.AppendLine(Tab + "file_name=\"" + NomORUX + " " + CarteActuelle.FacteurORUX + "\" />");
            S.AppendLine("<MapDimensions height=\"" + CarteActuelle.HauteurPixel + "\" width=\"" + CarteActuelle.LargeurPixel + "\" />");
            S.AppendLine("<MapBounds minLat=\"" + DblToStr(CarteActuelle.Coins[2].Latitude, Format) + "\" maxLat=\"" + DblToStr(CarteActuelle.Coins[0].Latitude, Format) + "\" minLon=\"" + DblToStr(CarteActuelle.Coins[0].Longitude, Format) + "\" maxLon=\"" + DblToStr(CarteActuelle.Coins[2].Longitude, Format) + "\" />");
            // definition des points de référencement du niveau
            S.AppendLine("<CalibrationPoints>");
            S.AppendLine("<CalibrationPoint corner=\"TL\" lon=\"" + DblToStr(CarteActuelle.Coins[0].Longitude, Format) + "\" lat=\"" + DblToStr(CarteActuelle.Coins[0].Latitude, Format) + "\" />");
            S.AppendLine("<CalibrationPoint corner=\"BR\" lon=\"" + DblToStr(CarteActuelle.Coins[2].Longitude, Format) + "\" lat=\"" + DblToStr(CarteActuelle.Coins[2].Latitude, Format) + "\" />");
            S.AppendLine("<CalibrationPoint corner=\"TR\" lon=\"" + DblToStr(CarteActuelle.Coins[1].Longitude, Format) + "\" lat=\"" + DblToStr(CarteActuelle.Coins[1].Latitude, Format) + "\" />");
            S.AppendLine("<CalibrationPoint corner=\"BL\" lon=\"" + DblToStr(CarteActuelle.Coins[3].Longitude, Format) + "\" lat=\"" + DblToStr(CarteActuelle.Coins[3].Latitude, Format) + "\" />");
            S.AppendLine("</CalibrationPoints>");
            // fin niveau
            S.AppendLine("</MapCalibration>");
            S.AppendLine("</OruxTracker>");
        }
        /// <summary> permet la gestion de la base de données sqlite associée à une carte au format ORUX </summary>
        private class BDTuilesOruxMaps : IDisposable
        {
            /// <summary> connexion avec la base SQLite</summary>
            internal SQLiteConnection SQLconnect;
            /// <summary> true si l'initialisation c'est bien passée</summary>
            internal bool Isok { get; private set; }
            /// <summary> command a envoyer à la base de données SQLite</summary>
            private SQLiteCommand SQLcommand;
            /// <summary> nom de la base de données SQLite. C'est toujours le même</summary>
            private const string NomBase = "OruxMapsImages.db";
            /// <summary> Crée ou ouvre une base SQLite pour les fichiers tuiles issus des cartes FCGP au format attendu par  ORUXMAPS</summary>
            /// <param name="CheminBase">chemin complet de la base de donnée Sqlite yc le slash de fin</param>
            /// <param name="FlagCreation">Creer la structure de la base si true sinon juste creer la connexion</param>
            internal BDTuilesOruxMaps(string CheminBase, bool FlagCreation)
            {
                try
                {
                    string CheminComplet = CheminBase + NomBase;
                    SQLconnect = new SQLiteConnection("Data Source=" + CheminComplet + ";");
                    SQLcommand = SQLconnect.CreateCommand();
                    if (FlagCreation)
                    {
                        if (File.Exists(CheminComplet))
                            File.Delete(CheminComplet);
                        // creation de la base
                        SQLconnect.Open();
                        if (!CreerTableTuiles())
                            return;
                        if (!CreerTableAndroid())
                            return;
                        // création effectif du fichier sur le disque
                        SQLconnect.Close();
                    }
                    Isok = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "A1X5");
                }
            }
            /// <summary> libère les ressources</summary>
            public void Dispose()
            {
                SQLcommand?.Dispose();
                SQLconnect?.Dispose();
            }
            /// <summary> écrit une tuile dans la base de données mais dans une boucle d'appel</summary>
            internal void InsererTuile(int X, int Y, int Z, string CheminTuile)
            {
                SQLcommand.Parameters.AddWithValue("@X", X);
                SQLcommand.Parameters.AddWithValue("@Y", Y);
                SQLcommand.Parameters.AddWithValue("@Z", Z);
                SQLcommand.Parameters.Add(new SQLiteParameter("@image", DbType.Binary) { Value = File.ReadAllBytes(CheminTuile) });
                SQLcommand.ExecuteNonQuery();
            }
            /// <summary> renvoie l'image d'une tuile</summary>
            internal Image LireTuileToImage(int X, int Y, int Z)
            {
                // décompresse (jpeg) le tableau de byte sous forme d'une image système
                var LireTuileToImageRet = Image.FromStream(new MemoryStream(LireTuile(X, Y, Z)));
                return LireTuileToImageRet;
            }
            /// <summary> enregistre l'image d'une tuile sous forme de fichier jpeg</summary>
            internal void LireTuileToFichier(int X, int Y, int Z, string FilePath)
            {
                try
                {
                    // enregistre le tableau de bytes sous forme de fichier sans présager du format effectif de l'image lors de l'enregistrement
                    File.WriteAllBytes(FilePath, LireTuile(X, Y, Z));
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "H6Y2");
                }
            }
            /// <summary> renvoie un tableau de byte correspondant au champ image de la Tuile X, Y, Z</summary>
            internal byte[] LireTuile(int X, int Y, int Z)
            {
                byte[] LireTuileRet;
                SQLconnect.Open();
                // lecture d'une seule tuile
                SQLcommand.CommandText = "Select [image] FROM [tiles] WHERE [x]=@X And [y]=@Y And [z]=@Z;";
                SQLcommand.Parameters.AddWithValue("@X", X);
                SQLcommand.Parameters.AddWithValue("@Y", Y);
                SQLcommand.Parameters.AddWithValue("@Z", Z);
                LireTuileRet = (byte[])SQLcommand.ExecuteScalar();
                SQLconnect.Close();
                return LireTuileRet;
            }
            /// <summary>ecrit chaque tuile d'un niveau de détail sous forme d'un fichier jpeg</summary>
            /// <param name="Z">Niveau d'affichage</param>
            /// <param name="Chemin">chemin où seront enregistrés les tuiles</param>
            internal bool LireNiveauFichier(int Z, string Chemin)
            {
                bool LireNiveauFichierRet = false;
                SQLconnect.Open();
                try
                {
                    // lecture de toutes les tuiles du niveau
                    SQLcommand.CommandText = "Select * FROM [tiles] WHERE [z]=@Z;";
                    SQLcommand.Prepare();
                    SQLcommand.Parameters.AddWithValue("@Z", Z);
                    using (var rdr = SQLcommand.ExecuteReader())
                    {
                        while (rdr.Read())
                            // pour chaque tuile on enregistre l'image sous forme de fichier
                            File.WriteAllBytes(Chemin + "C" + Convert.ToInt32(rdr["x"]).ToString("000") + Convert.ToInt32(rdr["y"]).ToString("000") + ".jpg", (byte[])rdr["image"]);
                    }
                    LireNiveauFichierRet = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "T0B0");
                }
                SQLconnect.Close();
                return LireNiveauFichierRet;
            }
            /// <summary>ecrit dans la base de données tous les fichiers jpeg d'un répertoire</summary>
            /// <param name="CarteActuelle">carte correspondante au niveau ORUX</param>
            /// <param name="Chemin">chemin où sont enregistrés les tuiles</param>
            internal bool EcrireNiveauFichier(string Chemin, Carte CarteActuelle)
            {
                bool EcrireNiveauFichierRet = false;
                // trouve toutes les tuiles du niveau
                FileInfo[] ImageTuiles = new DirectoryInfo(Chemin).GetFiles("*.jpg");
                int Z = CarteActuelle.FacteurORUX;
                SQLconnect.Open();
                SQLcommand.CommandText = "INSERT INTO [tiles] ([x], [y], [z], [Image]) VALUES (@X, @Y, @Z, @Image);";
                try
                {
                    foreach (FileInfo ImageTuile in ImageTuiles)
                    {
                        // trouve les X et Y en fonction du nom de l'image
                        int X = int.Parse(ImageTuile.Name.Substring(1, 3));
                        int Y = int.Parse(ImageTuile.Name.Substring(4, 3));
                        // insere la tuile dans la table des tuiles
                        InsererTuile(X, Y, Z, ImageTuile.FullName);
                        CarteActuelle.NbIterations += 1;
                        if (CarteActuelle.NbIterations % 100 == 0)
                            AfficherVisueInformation($"{CarteActuelle.MessInfo + CarteActuelle.NbIterations} / {CarteActuelle.NbTotalIterations}");
                    }
                    EcrireNiveauFichierRet = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "N0P5");
                }
                SQLconnect.Close();
                return EcrireNiveauFichierRet;
            }
            /// <summary>change l'indice d'affichage d'un niveau de détail</summary>
            /// <param name="ZA">valeur actuelle du niveau qu'il faut changer</param>
            /// <param name="ZN">nouvelle valeur du niveau qu'il faut changer</param>
            internal bool ChangerNiveauAffichage(int ZA, int ZN)
            {
                bool ChangerNiveauAffichageRet = false;
                SQLconnect.Open();
                try
                {
                    SQLcommand.CommandText = "UPDATE [tiles]  Set  [z] = @ZN  WHERE [z]=@ZA;";
                    SQLcommand.Parameters.AddWithValue("@ZA", ZA);
                    SQLcommand.Parameters.AddWithValue("@ZN", ZN);
                    SQLcommand.ExecuteNonQuery();
                    ChangerNiveauAffichageRet = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "O7Y5");
                }
                SQLconnect.Close();
                return ChangerNiveauAffichageRet;
            }
            /// <summary> 1ère partie de la structure de la base. true si la création de la table c'est bien passée</summary>
            private bool CreerTableTuiles()
            {
                bool CreerTableTuilesRet = false;
                try
                {
                    SQLcommand = SQLconnect.CreateCommand();
                    SQLcommand.CommandText = @"CREATE TABLE [tiles] ([x] int NOT NULL,  
                                                                     [y] int NOT NULL, 
                                                                     [z] int NOT NULL, 
                                                                     [image] image NULL,  
                                                          CONSTRAINT [sqlite_autoindex_tiles_1] PRIMARY KEY ([x],[y],[z]));
                                                        CREATE INDEX [IND_tiles] ON [tiles] ([x] ASC,[y] ASC,[z] ASC);";
                    SQLcommand.ExecuteNonQuery();
                    CreerTableTuilesRet = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "S2V7");
                }

                return CreerTableTuilesRet;
            }
            /// <summary> 2ème partie de la structure de la base. true si la création de la table c'est bien passée</summary>
            private bool CreerTableAndroid()
            {
                bool CreerTableAndroidRet = false;
                try
                {
                    // création de la table
                    SQLcommand.CommandText = "CREATE TABLE [android_metadata] ( [locale] text NULL );";
                    SQLcommand.ExecuteNonQuery();
                    // insertion du seul enregistrement
                    SQLcommand.CommandText = "INSERT INTO [android_metadata] ([locale]) VALUES ('fr_FR');";
                    SQLcommand.ExecuteNonQuery();
                    CreerTableAndroidRet = true;
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "O5X2");
                }

                return CreerTableAndroidRet;
            }
        }
        #endregion
    }
}