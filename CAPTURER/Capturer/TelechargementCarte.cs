using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.ServeurCarto;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> création d'une carte et des fichiers tuiles associés à partir du téléchargement de tuiles sur un serveur cartographique </summary>
    internal static class TelechargementCarte
    {
        #region Variables
        internal static bool FlagAnnulerTelechargement
        {
            set
            {
                _FlagAnnuler = value;
            }
        }
        /// <summary> Nb maximum de tuiles du serveur carto pour une carte enfonction du type de support de la carte </summary>
        internal static int NB_Max_TuilesServeurCarto
        {
            get
            {
                int Tuiles = CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier ? TailleMaxTampon * 3 : TailleTamponCarteSource[(int)CartesSettings.SUPPORT_CARTE];
                return Tuiles / PoidsTuile;
            }
        }
        /// <summary> Nb maximum de tuiles pour une rangée d'un fichier tuiles hors carte suisse interpolée </summary>
        /// <param name="SupportCarte"> Typesupport de la carte </param>
        /// <param name="Rapport"> rapport entre les dimensions de la tuile du fichier tuiles et celles du serveur : 1,2 ou 4 </param>
        internal static int NB_Max_Tuiles_X(TypeSupportCarte SupportCarte, int Rapport)
        {
            return TailleTamponCarteSource[(int)SupportCarte] / (PoidsTuile * Rapport);
        }
        /// <summary> flag indiquant qu'il y a un empêchement à la continuation du téléchargement des tuiles </summary>
        private static bool FlagArret
        {
            get
            {
                return _FlagAnnuler || !IsConnected;
            }
        }
        /// <summary> flag indiquant que rien n'empêche la continuation du téléchargement des tuiles </summary>
        private static bool FlagContinue
        {
            get
            {
                return !_FlagAnnuler && IsConnected;
            }
        }
        /// <summary> nb d'octects réserver de base pour les cartes autres que les cartes suisse interpolées </summary>
        private readonly static int[] TailleTamponCarteSource = new int[] { TailleMaxTampon, TailleMaxTampon / 4, TailleMaxTampon / 2 };
        /// <summary> indique si la demande d'annulation du téléchargement des tuiles a été demandée et par riccochet l'abandon de la création de la carte </summary>
        private static bool _FlagAnnuler;
        /// <summary> serveur à partir duquel les tuiles seront téléchargées </summary>
        private static ServeurCarto ServeurCarto;
        /// <summary> fichier servant de support d'écriture à la capture de la carte si le support de carte est fichier </summary>
        private static FileStream FichierBin;
        /// <summary> flux mémoire associé au fichier binaire pour éviter des engorgements de la mémoire</summary>
        private static MemoryStream TamponStream;
        /// <summary> carte support du téléchargement des tuiles </summary>
        private static Carte CarteSource;
        /// <summary> pointeur sur le tampon de copie des tuiles téléchargées </summary>
        private static IntPtr ImageBitsPtr;
        /// <summary> en cas de support de carte type fichier il faut un tampon pour la copie des tuiles téléchargées </summary>
        private static SharedPinnedByteArray TamponSupportFichier;
        /// <summary> Surface à télécharger exprimée en tuiles de la projection du serveur associé au site carto </summary>
        private static SurfaceTuiles SurfaceCapture;
        /// <summary> indique le nb de tuiles qui ont été téléchargées </summary>
        private static int NbTuilesTelechargees;
        /// <summary> indique le nb de tuiles qui ont été téléchargées pour une rangée </summary>
        private static int NbTuilesRangeeTelechargees;
        /// <summary> indique un petit message avant le nb de tuiles telechargées </summary>
        private static string MessageInfo;
        /// <summary> sert à mesurer le temps de téléchargement des tuiles et de traitement de la carte </summary>
        private static Chronometre ChronoCarte;

        #endregion
        #region Initialisation globale
        /// <summary> création d'une carte ayant les coordonnées de celles demandées </summary>
        /// <param name="NomCarte"> nom de la carte sans chemin et sans extension </param>
        /// <param name="SurfaceTuilesDemande"> surface tuiles demandée pour la nouvelle carte </param>
        /// <param name="Information"> label du programme appelant où l'on va écrire les informations générées pendant le téléchargement et la création de la carte </param>
        /// <param name="Site"> si on veut un site différent de celui de CapturerSettings </param>
        /// <param name="DomTom"> si on veut un DomTom différent de celui de CapturerSettings </param>
        /// <param name="CheminCache"> si on veut un Cache différent de celui de CapturerSettings </param>
        internal async static Task RealiserCarteAsync(string NomCarte, SurfaceTuiles SurfaceTuilesDemande, Label Information,
                                                      SitesCartographiques Site = SitesCartographiques.Aucun,
                                                      DomsToms DomTom = DomsToms.aucun,
                                                      string CheminCache = null)
        {
            try
            {
                LabelInformation = Information;
                SurfaceCapture = SurfaceTuilesDemande;
                if (!InitialiserCarteSource(NomCarte))
                    return;
                AfficherVisueInformation($"{MessageInfo}{NbTuilesTelechargees} / {SurfaceCapture.NbTuiles}");
                ServeurCarto = new ServeurCarto(null, Site, DomTom, CheminCache);
                _FlagAnnuler = false;
                NbTuilesTelechargees = 0;
                ChronoCarte = new Chronometre(2);
                // lance l'action de téléchargement des tuiles et rend la main
                int Telechargement = ChronoCarte.Demarre();
                // rend la main à l'interface utilisateur pendant le téléchargement de la carte
                await Task.Run(() => TelechargerCarte());
                ChronoCarte.Arrete(Telechargement);
                if (CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier)
                    LibererFichierBin();

                if (FlagContinue)
                {
                    int Traitement = ChronoCarte.Demarre();
                    // lance l'action de traitement de la carte et rend la main
                    string[] Coordonnees = CoordonneesCarte();
                    if (await Task.Run(() => CarteSource.FinaliserCarte(Coordonnees, "Préparation de la carte suite au téléchargement des tuiles.")))
                    {
                        ChronoCarte.Arrete(Traitement);
                        // si il n'y a pas d'erreur on rajoute le temps de capture et de traitement aux infos concernant les coordonnées.
                        MessageInformation = CoinsCarte() + $"Temps de téléchargement : {TimeSpanToStr(ChronoCarte.Duree(Telechargement), false)}{CrLf}" +
                                                            $"Temps de traitement : {TimeSpanToStr(ChronoCarte.Duree(Traitement), false)}";
                        if (FormApplication.WindowState == FormWindowState.Minimized)
                            FormApplication.WindowState = FormWindowState.Normal;
                        AfficherInformation();
                    }
                }
                else
                {
                    string Titre = TitreInformation;
                    TitreInformation = "Module Téléchargement des tuiles";
                    if (_FlagAnnuler)
                    {
                        MessageInformation = "Téléchargement des tuiles annulé par l'utilisateur";
                    }
                    else
                    {
                        MessageInformation = "Téléchargement arrêté par connexion Internet interrompue";
                    }
                    AfficherInformation();
                    TitreInformation = Titre;
                }
            }
            catch (Exception ex)
            {
                AfficherErreur(ex, "A2AF");
            }
            CarteSource.Dispose();
            CarteSource = null;

            ServeurCarto.Dispose();
            ServeurCarto = null;
        }
        /// <summary> télécharge la carte par rangée de tuiles. Chaque tuile gère sa composition et son écriture dans le tampon </summary>
        private static void TelechargerCarte()
        {
            int NbColRangee, NbCol;
            for (int Row = SurfaceCapture.NumRangDeb, loopTo = SurfaceCapture.NumRangFin; Row <= loopTo; Row++)
            {
                NbTuilesRangeeTelechargees = 0;
                NbCol = SurfaceCapture.NbColonnes;
                NbColRangee = NbCol;
                for (int Col = SurfaceCapture.NumColDeb, loopTo1 = SurfaceCapture.NumColFin; Col <= loopTo1; Col++)
                {
                    if (FlagContinue)
                    {
                        // on lance la composition de la tuile ainsi que son écriture dans le tampon de l'image
                        // et redonne la main à la boucle. N'attend pas que la tuile soit finie (await dans le constructeur)
                        _ = new TuileCarte(Col, Row);
                        NbColRangee -= 1;
                    }
                    else
                    {
                        // toutes les tuiles non demandées sont considérées comme finies
                        NbTuilesRangeeTelechargees += NbColRangee;
                        break;
                    }
                }
                // on attend que toutes les tuiles de la rangée soient considérées comme finies
                while (NbTuilesRangeeTelechargees < NbCol)
                {
                }
                if (FlagArret)
                    break;
                if (CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier)
                {
                    TamponStream.CopyTo(FichierBin);
                    TamponStream.Position = 0L;
                    TamponSupportFichier.ClearColor(PartagerSettings.COULEUR_CARTE);
                }
            }
        }
        #endregion
        #region Initialisation Carte
        /// <summary> création de la carte support du téléchargement des tuiles </summary>
        /// <param name="NomCarte"> nom de la carte sans chemin et sans extension </param>
        /// <param name="SysCarto"> système cartographique de la carte </param>
        private static bool InitialiserCarteSource(string NomCarte)
        {
            // création de la nouvelle carte
            CarteSource = new Carte(SurfaceCapture.NbColonnes, SurfaceCapture.NbRangees, SysCartoEncours, NomCarte);
            // initialisation des différentes variables globales à la carte
            {
                if (CarteSource.IsOk)
                {
                    CarteSource.DemandeFichiersGeoref = ConvertirIndexToFichiersGeoref(CartesSettings.GEOREF);
                    if (CartesSettings.SUPPORT_CARTE == TypeSupportCarte.Fichier)
                    {
                        AttribuerFichierBin();
                        ImageBitsPtr = TamponSupportFichier.BitPtr;
                    }
                    else
                    {
                        ImageBitsPtr = CarteSource.ImageBitPtr;
                    }
                    MessageInfo = $"{CarteSource.NomComplet} :{CarteSource.Separateur}Téléchargement données :{CarteSource.Separateur}";
                    // infos concernant les cartes
                    // si il y a une trace encours et que som emprise soit sur la carte on renseigne les informations concernant son dessin
                    if (TRK.IsTraceEncours && SurfaceCapture.RectanglePixels.IntersectsWith(TRK.TraceEncours.BoundingBoxVirtuel) && CartesSettings.IS_TRACE)
                    {
                        CarteSource.PinceauTrace = new Pen(TracesSettings.COULEUR_TRACE, TracesSettings.EPAISSEUR_TRACE) { LineJoin = LineJoin.Round };
                        CarteSource.DemandeAfficherTrace = true;
                        CarteSource.NbTracesKML = 0;
                        CarteSource.FichiersTRK = new List<string>(1) { Path.Combine(TracesSettings.CHEMIN_TRACE, TRK.TraceEncours.Nom + ".trk") };
                    }
                }
                return CarteSource.IsOk;
            }
        }
        /// <summary> détermination des 4 coins de la carte sous forme de chaine. Historique par rapport aux versions avec captures d'écran </summary>
        private static string[] CoordonneesCarte()
        {
            var Coordonnees = new string[4];
            for (int Cpt = 0; Cpt <= 3; Cpt++)
                Coordonnees[Cpt] = ConvertPointXYtoChaine(new PointProjection(SurfaceCapture.RectangleGrille.Pt(Cpt)), "N2");
            return Coordonnees;
        }
        /// <summary> initialise la variable FichierGeoref en fonction d'index (liste déroulante du formulaire Preférences </summary>
        /// <param name="Index"></param>
        private static ChoixFichiersGeoref ConvertirIndexToFichiersGeoref(int Index)
        {
            if (Index == 0)
            {
                return ChoixFichiersGeoref.Aucun;
            }
            else
            {
                // si la demande concerne un georeferencement autre que celui de FCGP on le rajoute
                return (ChoixFichiersGeoref)Math.Pow(2d, Index - 1) | ChoixFichiersGeoref.Georef_Carte;
            }
        }
        /// <summary> convertit les 4 coins de la carte en texte avec les unités de coordonnées en cours </summary>
        private static string CoinsCarte()
        {
            int SelectIndex = CapturerSettings.INDICE_TYPE_COORDONNEES;
            var Coincarte = new StringBuilder();
            Coincarte.AppendLine($"La carte {CarteSource.Nom}.{CarteSource.Format} a été sauvegardée{CrLf + CrLf}Coordonnées des tuiles téléchargées :");
            switch (SelectIndex)
            {
                case 0:
                    {
                        for (int Cpt = 0; Cpt <= 3; Cpt++)
                            Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointXYtoChaine(new PointProjection(SurfaceCapture.RectangleGrille.Pt(Cpt)))}");
                        break;
                    }
                case 1:
                    {
                        for (int Cpt = 0; Cpt <= 3; Cpt++)
                        {
                            var CoordDD = PointGrilleToPointDD(SurfaceCapture.RectangleGrille.Pt(Cpt), ServeurCarto.Datum);
                            Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointDDtoChaine(CoordDD)}");
                        }

                        break;
                    }
                case 2:
                    {
                        for (int Cpt = 0; Cpt <= 3; Cpt++)
                        {
                            var CoordDD = PointGrilleToPointDD(SurfaceCapture.RectangleGrille.Pt(Cpt), ServeurCarto.Datum);
                            Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointDDtoDMS(CoordDD)}");
                        }

                        break;
                    }
                case 3:
                    {
                        for (int Cpt = 0; Cpt <= 3; Cpt++)
                        {
                            var CoordDD = PointGrilleToPointDD(SurfaceCapture.RectangleGrille.Pt(Cpt), ServeurCarto.Datum);
                            Coincarte.AppendLine($"Pt{Cpt} : {ConvertPointDDtoUTM(CoordDD)}");
                        }

                        break;
                    }
                case 4:
                    {
                        for (int Cpt = 0; Cpt <= 3; Cpt++)
                        {
                            var PointTuile = new PointG((int)Affichage.Echelle, SurfaceCapture.PtPixels(Cpt));
                            Coincarte.AppendLine($"Pt{Cpt} : {PointTuile.ToString()}");
                        }

                        break;
                    }
            }
            Coincarte.AppendLine();
            return Coincarte.ToString();
        }
        /// <summary> met en place les ressources associées au fichier support de la capture</summary>
        private static void AttribuerFichierBin()
        {
            // pour le téléchargement on travaille avec 1 rangée de tuile serveur pui son l'enregistre dans un fichier raw
            TamponSupportFichier = new SharedPinnedByteArray(SurfaceCapture.NbColonnes * NbOctetsDecalXTuile, NbPixelsTuile, CarteSource.Reserve, "Téléchargement Carte");
            TamponStream = new MemoryStream(TamponSupportFichier.Bits, TamponSupportFichier.IndexBytesDisponible, TamponSupportFichier.NbBytes);
            FichierBin = new FileStream(CarteSource.CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None, PoidsTuile);
        }
        /// <summary> libère les ressources associées au fichier support de la capture</summary>
        private static void LibererFichierBin()
        {
            if (TamponStream is not null)
            {
                TamponStream.Close();
                TamponStream = null;
            }
            if (TamponSupportFichier is not null)
            {
                TamponSupportFichier.Dispose();
                TamponSupportFichier = null;
            }
            if (FichierBin is not null)
            {
                FichierBin.Close();
                FichierBin = null;
            }
        }
        #endregion

        /// <summary> assume une tuile de serveur qui compose une carte. Elle peut être composée de 1 à 3 couches de données (layers) supperposées. 
        /// Autonomme pour le téléchargement et le dessin sur la carte. Le constructeur assure l'asynchronisme </summary>
        private class TuileCarte
        {
            // position de la tuile par rapport à la carte. 1er octet de la tuile (0, 0) sur la 1ère ligne de la carte (Col (X), Row (Y))
            private readonly IntPtr PointeurDestinationCarte;
            private Layer[] Layers;
            private readonly int Col;
            private readonly int Row;
            /// <summary> on a besoin que de l'index colonne et de l'index rangée pour définir la tuile. Le reste des paramètres est commun </summary>
            /// <param name="Col"> index da la colonne</param>
            /// <param name="Row"> index de la rangée </param>
            internal TuileCarte(int Col, int Row)
            {
                this.Col = Col;
                this.Row = Row;
                // calcul du pointeur de la tuile par rapport à la carte
                int DecalX = (Col - SurfaceCapture.NumColDeb) * NbOctetsDecalXTuile;
                // dans le cas d'un support de carte fichier on télécharge 1 rangée uniquement
                int DecalY = CartesSettings.SUPPORT_CARTE != TypeSupportCarte.Fichier ? (Row - SurfaceCapture.NumRangDeb) * CarteSource.LargeurOctets * NbPixelsTuile : 0;
                PointeurDestinationCarte = ImageBitsPtr + DecalX + DecalY;
                if (FlagContinue)
                {
                    ComposerTuileAsync();
                }
                else
                {
                    Interlocked.Increment(ref NbTuilesTelechargees);
                    Interlocked.Increment(ref NbTuilesRangeeTelechargees);
                    AfficherVisueInformation($"{MessageInfo}{NbTuilesTelechargees} / {SurfaceCapture.NbTuiles}");
                }
            }
            /// <summary> télécharge et dessine sur la carte jusqu'à 3 couches </summary>
            private async void ComposerTuileAsync()
            {
                Layers = new Layer[ServeurCarto.NbLayers];
                for (int Cpt = 0, loopTo = ServeurCarto.NbLayers - 1; Cpt <= loopTo; Cpt++)
                {
                    if (FlagContinue)
                    {
                        int IndiceLayer = Cpt; // obligatoire car on lance une tache
                        string Uri = ServeurCarto.UriTuileAffichage(IndiceLayer, SurfaceCapture.Couche, Col, Row);
                        Layers[IndiceLayer] = new Layer() { Uri = Uri, IndiceLayer = IndiceLayer, Statut = StatutTuile.DemandeTeleChargement };
                        await TelechargerLayerAsync(IndiceLayer);
                    }
                    else
                    {
                        break;
                    }
                }
                // et quand toutes les couches de la tuile sont téléchargées on les dessine sur la carte
                if (FlagContinue)
                    ComposerImageTuile();
                // on indique que la tuile à finie sa tâche
                Interlocked.Increment(ref NbTuilesTelechargees);
                Interlocked.Increment(ref NbTuilesRangeeTelechargees);
                AfficherVisueInformation($"{MessageInfo}{NbTuilesTelechargees} / {SurfaceCapture.NbTuiles}");
            }
            /// <summary> télécharge la couche. Essaie jusqu'à 10 fois avant d'abandonner </summary>
            /// <param name="IndiceLayer"></param>
            /// <returns> pas de retour direct mais le byte() de la couche est remplit en cas de succès </returns>
            private async Task TelechargerLayerAsync(int IndiceLayer)
            {
                var Cpt = default(int);
                var Couche = Layers[IndiceLayer];
                do
                {
                    if (FlagArret)
                        break;
                    // on demande le téléchargement si le nb de requêtes ne depasse pas la limite autorisée
                    if (ServeurCarto.NbRequetes[IndiceLayer] < 3)
                    {
                        await TelechargerDataLayerAsync(IndiceLayer);
                        if (Couche.Statut == StatutTuile.ErreurServeur)
                        {
                            // connexion internet tombée ou autre
                            Cpt += 1;
                        }
                        else
                        {
                            break;
                        } // on a fini le téléchargement de la couche 
                    }
                    if (FlagContinue)
                        await Task.Delay(IntAleatoire(50, 350));
                }
                while (Cpt < 10);
            }
            /// <summary> télécharge la couche à partir d'un serveur graphique et d'une URI </summary>
            /// <param name="IndiceLayer"></param>
            /// <returns> Pas de retour direct mais modification du statut de la couche. 3 possibilités </returns>
            private async Task TelechargerDataLayerAsync(int IndiceLayer)
            {
                var Couche = Layers[IndiceLayer];
                var tmp = ServeurCarto.NbRequetes;
                int arglocation = tmp[IndiceLayer];
                Interlocked.Increment(ref arglocation);
                try
                {
                    using (var Reponse = await ServeurCarto.RequeteHttp.GetAsync(Couche.Uri))
                    {
                        if (Reponse.StatusCode == HttpStatusCode.OK)
                        {
                            Couche.Bits = await Reponse.Content.ReadAsByteArrayAsync();
                            Couche.Statut = StatutTuile.Serveur;
                        }
                        else if (Reponse.StatusCode == HttpStatusCode.BadRequest || Reponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            Couche.Statut = StatutTuile.Inexistant;
                        }
                        else
                        {
                            // erreur gérée en interne de la couche HttpClient
                            Couche.Statut = StatutTuile.ErreurServeur;
                        }
                    }
                }
                catch
                {
                    // erreur gérée en externe de la couche HttpClient. Généralement annulation de la tache liées à Await sur timeout
                    // problématique d'avoir un message. On gère uniquement le statut de la tuile
                    // Ex As Exception;
                    // AfficherErreur(Ex, "Y0V7");
                    Couche.Statut = StatutTuile.ErreurServeur;
                }
                var tmp1 = ServeurCarto.NbRequetes;
                int arglocation1 = tmp1[IndiceLayer];
                Interlocked.Decrement(ref arglocation1);
            }
            /// <summary> ajoute les couches de 1 à 3 sur la carte. La 1ere est forcément le fond de plan, les 2 autres ont forcément un fond transparent </summary>
            private void ComposerImageTuile()
            {
                // delimitation de la zone de dessin sur la carte
                using (var Tuile = new Bitmap(NbPixelsTuile, NbPixelsTuile, CarteSource.LargeurOctets, FormatPixel, PointeurDestinationCarte))
                {
                    using (var G = Graphics.FromImage(Tuile))
                    {
                        foreach (var Layer in Layers)
                        {
                            if (FlagArret)
                                break;
                            if (Layer.Statut == StatutTuile.Serveur) // il faut que le layer soit téléchargé pour le dessiner
                            {
                                using (var LayerImage = new Bitmap(new MemoryStream(Layer.Bits)))
                                {
                                    // dessinne la couche sur la carte en fonction du coeficient de transparence et du format de la couche (PNG ou Jpeg)
                                    AjouterImageLayer(G, LayerImage, ServeurCarto.CoefAlphaLayer(Layer.IndiceLayer));
                                }
                            }
                        }
                    }
                }
            }
            /// <summary> ajoute la couche sur l'image de la tuile avec ou sans transparence</summary>
            /// <param name="G"> graphics où l'on doit dessinner la couche </param>
            /// <param name="LayerImage"> couche à dessinner </param>
            /// <param name="CoefAlpha"> coed de transparence de la couche </param>
            private static void AjouterImageLayer(Graphics G, Bitmap LayerImage, float CoefAlpha)
            {
                // si le coef de transparence est 1, complètement opaque, on peut aller vite 
                if (CoefAlpha == 1f) // la couche est opaque hors le fond qui peut être transparent (format PNG uniquement)
                {
                    G.DrawImage(LayerImage, Point.Empty);
                }
                else // la couche n'est pas opaque
                {
                    var imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(new ColorMatrix() { Matrix33 = CoefAlpha }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    // Le fond est transparent à 100% (image PNG obligatoire), le coef alpha de l'image peut varié de 100% (Opaque) à 0% (Invisible)
                    G.DrawImage(LayerImage, new Rectangle(Point.Empty, new Size(NbPixelsTuile, NbPixelsTuile)), 0, 0, NbPixelsTuile, NbPixelsTuile, GraphicsUnit.Pixel, imageAttributes);
                }
            }
            /// <summary> donne une structure qui permet de receuillir le téléchargement d'une tuile sur le serveur </summary>
            private class Layer
            {
                internal byte[] Bits;
                internal int IndiceLayer;
                internal StatutTuile Statut;
                internal string Uri;
            }
        }
    }
}