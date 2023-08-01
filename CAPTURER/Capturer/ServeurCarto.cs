using static FCGP.AfficheInformation;
using static FCGP.CapturerGlobal;
using static FCGP.Commun;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.NiveauDetailCartographique;
using static FCGP.Proprietes.Resources;
using static FCGP.Settings;
using static FCGP.SystemeCartographique;

namespace FCGP
{

    /// <summary> Décrit les caractéristiques d'un serveur Cartographique </summary>
    internal partial class ServeurCarto : IDisposable
    {
        #region Visible
        /// <summary> nombre d'octects à lire pour une ligne de la tuile de fond </summary>
        internal const int NbOctetsDecalXTuile = NbPixelsTuile * NbOctetsPixel;
        /// <summary> nombre d'octects d'une tuile de fond </summary>
        internal const int PoidsTuile = NbOctetsDecalXTuile * NbPixelsTuile;
        /// <summary> Nb maximum de tuiles demandées en même temps par le http du serveur carto </summary>
        internal const int MaxRequetesTuiles = 80;
        /// <summary> cache associé au serveur et au layer de fond de plan </summary>
        internal CacheTuiles Cache { get; private set; }
        /// <summary> nb de demande d'affichage encours </summary>
        internal int NbDemandesAffichage
        {
            get
            {
                return ListeDemandesAffichage is null ? 0 : ListeDemandesAffichage.Count;
            }
        }
        /// <summary> nb de tuile concernées par les demande d'affichage encours </summary>
        internal int NbTuilesDemandeAffichage
        {
            get
            {
                return _NbTuilesDemandeAffichage;
            }
        }
        /// <summary> retourne la demande d'affichage ayant l'Id demandé </summary>
        internal DemandeTuiles DemandesAffichage(int Id)
        {
            return ListeDemandesAffichage[Id];
        }
        /// <summary> nb de layers associé au serveur carto pour le téléchargement </summary>
        internal int NbLayers { get; private set; }
        /// <summary> nb requêtes encours associées à la ressource http </summary>
        internal int[] NbRequetes { get; private set; }
        /// <summary> ressources http </summary>
        internal HttpClient RequeteHttp { get; private set; }
        /// <summary> Sitecarto associé au serveur Carto. permet d'obtenir les constantes associées </summary>
        internal SitesCartographiques SiteCarto { get; private set; }
        /// <summary> pointG décrivant le centre de la projection du site de capture </summary>
        internal PointG Centre { get; private set; }
        /// <summary> RectangleD décrivant les limites de la projection du site de capture en DD ou GrilleSuisse</summary>
        internal RectangleD Limites { get; private set; }
        /// <summary> Nb d'échelles ou de couches pour le layer </summary>
        /// <param name="IndexLayer"> Layer demandé </param>
        internal int NbEchellesLayer(int IndexLayer)
        {
            return Layers[IndexLayer].NbEchelles;
        }
        /// <summary> Echelles du layer </summary>
        /// <param name="IndexLayer"> Layer demandé </param>
        internal string[] EchellesLayer(int IndexLayer)
        {
            return Layers[IndexLayer].Echelles;
        }
        /// <summary> Couches du tileMatrixSet du layer </summary>
        /// <param name="IndexLayer"> Layer demandé </param>
        internal byte[] CouchesLayer(int IndexLayer)
        {
            return Layers[IndexLayer].Couches;
        }
        /// <summary> Coefficient Alpha (transparence) du layer </summary>
        /// <param name="IndexLayer"> Layer demandé </param>
        internal float CoefAlphaLayer(int IndexLayer)
        {
            return Layers[IndexLayer].CoefAlpha;
        }
        /// <summary> Nom du serveur </summary>
        internal string Nom { get; private set; }
        /// <summary> clef du site carto associé au serveur carto </summary>
        internal string Cle { get; private set; }
        /// <summary> datum du site carto associé au serveur carto </summary>
        internal Datums Datum { get; private set; }
        /// <summary> indique si l'initialisation du serveur carto s'est bien déroulée </summary>
        internal bool IsOk { get; private set; }
        /// <summary> indique si le site associé au serveur carto peut afficher les pentes </summary>
        internal bool IsPentes { get; private set; }
        /// <summary> indique si l'echelle associée au site du serveur carto peut afficher les pentes </summary>
        internal bool IsIndiceEchellesPentes(int IndiceEchelle)
        {
            return Array.IndexOf(DonneesLayersPentes[(int)SiteCarto].IndicesEchelle, IndiceEchelle) > -1;
        }
        /// <summary> N° de la couche sur le serveur pour le téléchargement des pentes. 1 couche unique pour 4 niveaux d'affichage </summary>
        internal int IndiceEchellePentes { get; private set; }
        /// <summary> Change le Fond de Plan affiché sur la Visue pour les sites ayant plusieurs fond de Plan d'affichage </summary>
        internal int IndiceFondPlan
        {
            get
            {
                return _IndiceFondPlan;
            }
            set
            {
                if (value != _IndiceFondPlan && value >= 0 && value <= NbLayersAffichages)
                {
                    _IndiceFondPlan = value;
                    ChangerFondPlanAffichage();
                }
            }
        }
        /// <summary> renvoie un tableau décrivant les types de fond de plan d'affichage (Carte, Plan ou Photo) du serveur carto</summary>
        internal string[] TypesFondsPlan
        {
            get
            {
                var Types = new string[NbLayersAffichages + 1];
                for (int Cpt = 0, loopTo = NbLayersAffichages; Cpt <= loopTo; Cpt++)
                    Types[Cpt] = DonneesLayersAffichages[(int)SiteCarto][Cpt].Type;

                return Types;
            }
        }
        /// <summary> indique si le site associé au serveurWMTS peut afficher plusieurs fond de plan </summary>
        internal bool IsFondsPlan
        {
            get
            {
                return NbLayersAffichages > 0;
            }
        }
        /// <summary> constructeur et vérifie que le serveur existe </summary>
        /// <param name="Affichage"> Si le serveur est pour un affichage sinon null </param>
        /// <param name="Site"> Site carto associé au serveur si différent de celui de capturerSettings </param>
        /// <param name="Domtom"> DomTom associé au serveur si site différent de celui de CapturerSettings </param>
        /// <param name="CheminCache">chemin du cache associé au serveur si différent de celui de capturerSetings </param>
        internal ServeurCarto(AffichageCarte Affichage = null,
                              SitesCartographiques Site = SitesCartographiques.Aucun,
                              DomsToms DomTom = DomsToms.aucun, string Chemin = null)
        {
            string Titre = TitreInformation;
            TitreInformation = "Information Serveur Carto";
            #region définition des infos liées au serveur carto
            SiteCarto = Site == SitesCartographiques.Aucun ? CapturerSettings.SITE : Site;
            _DomTom = Site == SitesCartographiques.Aucun ? CapturerSettings.DOMTOM : DomTom;
            Url = DonneesSitesWebs[(int)SiteCarto].Url;
            Datum = DatumSiteWeb((int)SiteCarto);
            int IndiceSite = Site == SitesCartographiques.Aucun ? CapturerSettings.INDICE_SITE : SiteDomTomToIndex(Site, DomTom);
            Centre = CentreSiteWeb(IndiceSite);
            Limites = LimiteSiteWeb(IndiceSite);
            Nom = SitesCartoLibelles[(int)SiteCarto];
            Cle = SiteCartoClef(SiteCarto);
            IsPentes = !string.IsNullOrEmpty(DonneesLayersPentes[(int)SiteCarto].Identifer);
            IndiceEchellePentes = -1;
            TypeTelechargement = Affichage is null;
            InitialiserHttp();
            #endregion
            #region définition des infos liées aux layers
            CheminCache = Chemin is null ? CapturerSettings.CHEMIN_CACHE + @"\" : string.IsNullOrEmpty(Chemin) ? Chemin : Chemin + @"\";
            _IndiceFondPlan = Site == SitesCartographiques.Aucun ? CapturerSettings.INDICE_FOND_PLAN : 0;
            NbLayersAffichages = DonneesLayersAffichages[(int)SiteCarto].Length - 1;
            TypeServeur = DonneesSitesWebs[(int)SiteCarto].TypeWMTS;
            EPSG_Rendu = DonneesSitesWebs[(int)SiteCarto].EPSG;
            Layer.InitialiserLayers(TypeServeur, DonneesSitesWebs[(int)SiteCarto].TileMatrixSet, EPSG_Rendu, DonneesSitesWebs[(int)SiteCarto].XY);
            int NbLayersMax = TypeTelechargement ? 3 : 1;
            ListeLayers = new SortedList<int, Layer>(NbLayersMax);
            if (TypeTelechargement)
            {
                FichierLayersServeurs = CheminCache + @"\LayersServeurs.xml";
                // on vérifie que le fichier CouchesServeur existe
                if (!File.Exists(FichierLayersServeurs))
                {
                    using (var f = File.CreateText(FichierLayersServeurs))
                    {
                        f.Write(LayersServeurs);
                    }
                }
                // on ajoute les Layers du fichier LayersServeur associés au serveur du site carto en testant qu'ils existent
                IsOk = AjouterLayers(true);
            }
            else // TypeAffichage
            {
                // on ajoute le layer par défaut pour la visualisation
                IsOk = AjouterLayersAffichage();
            }
            if (IsOk)
            {
                NbLayers = ListeLayers.Count;
                Layers = ListeLayers.Values.ToArray();
                ListeLayers.Clear();
                ListeLayers = null;
                NbRequetes = new int[NbLayers + 1];
                if (!TypeTelechargement)
                {
                    IsOk = InitialiserAffichage(Affichage);
                    if (IsOk)
                    {
                        #region définition des infos liées au layer Pentes
                        // pour les pentes on prend la plus petite échelle afin de réduire au maximum le nb de tuiles à télécharger
                        if (IsPentes)
                        {
                            IndiceEchellePentes = DonneesLayersPentes[(int)SiteCarto].IndicesEchelle.Length - 1;
                            CouchePentes = Layers[0].Couches[IndiceEchellePentes];
                            LayerPentes = new Layer()
                            {
                                Indice = -1,
                                Nom = "Pentes",
                                Identifier = DonneesLayersPentes[(int)SiteCarto].Identifer,
                                Style = DonneesLayersPentes[(int)SiteCarto].Style,
                                Format = DonneesLayersPentes[(int)SiteCarto].Format,
                                Time = DonneesLayersPentes[(int)SiteCarto].Time,
                                CoefAlpha = CoefsAlphasPentes[Site == SitesCartographiques.Aucun ? CapturerSettings.INDICE_COEF_ALPHA_PENTES : 1]
                            };
                            LayerPentes.Initialiser(Url);
                        }
                        #endregion
                    }
                }
            }
            #endregion
            TitreInformation = Titre;
        }
        /// <summary> uri de la tuile que l'on veut demander au serveur. </summary>
        /// <param name="IndexLayer"> Index de la couche dans le tableau des couches </param>
        /// <param name="Numcol"> Numéro de colonne de la tuile </param>
        /// <param name="Numrow"> Numéro de rangée de la tuile </param>
        /// <param name="Couche"> Couche de la tuile </param>
        internal string UriTuileAffichage(int IndexLayer, byte Couche, int Numcol, int Numrow)
        {
            var Layer = Layers[IndexLayer];
            string UriTuile;
            if (TypeServeur == TypesServeur.OSM)
            {
                CptOSM += 1;
                UriTuile = PorteSiteOSM[CptOSM % 3] + Layer.UriDeb;
            }
            else
            {
                UriTuile = Layer.UriDeb;
            }
            UriTuile += Layer.CalculerUriTilePosition(Couche, Numcol, Numrow);
            return UriTuile;
        }
        /// <summary> uri de la tuile des pentes que l'on veut demander au serveur. </summary>
        /// <param name="Numcol"> Numéro de colonne de la tuile </param>
        /// <param name="Numrow"> Numéro de rangée de la tuile </param>
        internal string UriTuilePentes(int Numcol, int Numrow)
        {
            string UriTuile = LayerPentes.UriDeb + LayerPentes.CalculerUriTilePosition(CouchePentes, Numcol, Numrow);
            return UriTuile;
        }
        /// <summary> renvoie une demande de tuile </summary>
        /// <param name="NbTuiles"> Nb de tuiles de la demande </param>
        /// <param name="Couche"> Couche du layer de fond de plan </param>
        internal DemandeTuiles CreerDemandeAffichage(int NbTuiles, byte Couche)
        {
            var Demande = new DemandeTuiles(NbTuiles, Couche, this);
            // on ajoute la demande dans une liste cela permet de savoir si les taches sont terminées juste avec un test sur le nb d'éléments de la liste
            ListeDemandesAffichage.Add(Demande.ID, Demande);
            return Demande;
        }
        /// <summary> Suppression d'une demande de tuiles. On stocke son id. On suppose qu'elle est finie </summary>
        internal void SupprimerDemandeAffichage(int ID)
        {
            ListeDemandesAffichage.Remove(ID);
        }
        /// <summary> Teste l'url du serveur avec une demande des capacités </summary>
        internal async void TesterServeurAsync()
        {
            string Titre = TitreInformation;
            TitreInformation = "Information Serveur Carto";
            if (SiteCarto > SitesCartographiques.DomTom)
            {
                MessageInformation = $"Il n'y a qu'une seule couche de données pour ce site.";
                AfficherInformation();
            }
            else
            {
                MessageInformation = $"Votre demande a été prise en compte.{CrLf}Une information vous avertira de la fin du test.";
                AfficherInformation();
                string NomCapacities = $@"{CapturerSettings.CHEMIN_CACHE}\Capacites_{Cle}.xml";
                if (await RequeteWebCapactiesAsync(0, NomCapacities))
                {
                    MessageInformation = $"Le fichier {NomCapacities} a été créé en réponse au test du serveur{CrLf}du site cartographique {Nom}";
                    AfficherInformation();
                }
                else
                {
                    MessageInformation = $"Le serveur du site cartographique {Nom} n'est pas accessible";
                    AfficherInformation();
                }
            }
            TitreInformation = Titre;
        }
        /// <summary> donne le facteur d'agrandissement ou de réduction entre 2 valeurs d'échelle associées aux couches du TileMatrixSet du serveur </summary>
        /// <param name="IndiceEchelle"> valeur actuelle de l'indiceEchelle </param>
        /// <param name="DeltaIndicesEchelle"> différence d'indice d'échelle </param>
        internal double FacteurEchelles(int IndiceEchelle, int DeltaIndicesEchelle)
        {
            double FacteurEchellesRet;
            if (DeltaIndicesEchelle == 0)
            {
                FacteurEchellesRet = 1d;
            }
            else if (Datum == Datums.Web_Mercator)
            {
                FacteurEchellesRet = Math.Pow(2d, -DeltaIndicesEchelle);
            }
            else // Datums.Grille_Suisse_LV03. On est obligé de passer par la données Mètre par pixel
            {
                var EchelleDepart = SiteIndiceEchelleToEchelle(SiteCarto, IndiceEchelle);
                var EchelleArrivee = SiteIndiceEchelleToEchelle(SiteCarto, IndiceEchelle + DeltaIndicesEchelle);
                FacteurEchellesRet = MetrePixels(EchelleDepart) / MetrePixels(EchelleArrivee);
            }

            return FacteurEchellesRet;
        }
        /// <summary> permet la libération des ressources http </summary>
        public void Dispose()
        {
            Dispose(true);
            // nécessaire pour éviter que le GC ne redemande Finalize()
            GC.SuppressFinalize(this);
        }

        #endregion
        #region Privée
        #region Champs
        /// <summary> Liste des demandes de tuiles qui ne sont pas finies </summary>
        private SortedList<int, DemandeTuiles> ListeDemandesAffichage;
        /// <summary> Nb de tuiles correspondant à l'ensemble des demandes de tuiles pas encore finies </summary>
        private int _NbTuilesDemandeAffichage;
        /// <summary> Appelant de la création du serveur si ce n'est pas un serveur pour le téléchargement </summary>
        private AffichageCarte Affichage;
        /// <summary> chemin ou se situe le ou les caches tuiles </summary>
        private string CheminCache;
        /// <summary> indique si le serveur a été détruit </summary>
        private bool disposed;
        /// <summary> permet de change la porte d'accès des serveurs OSM</summary>
        private int CptOSM;
        /// <summary> indice de la couche d'affichage encours </summary>
        private int _IndiceFondPlan;
        /// <summary> Layers d'affichage acceptés pour le serveur. Stockage provisioire </summary>
        private readonly SortedList<int, Layer> ListeLayers;
        /// <summary> Layers d'affichage acceptés pour le serveur </summary>
        private readonly Layer[] Layers;
        /// <summary> Layer des pentes accepté pour le serveur </summary>
        private readonly Layer LayerPentes;
        /// <summary> Domtom associé au serveur de capture </summary>
        private readonly DomsToms _DomTom;
        /// <summary> Addresse du serveur </summary>
        private readonly string Url;
        /// <summary> indique qu'il s'agit d'un serveur dédié au téléchargement de carte </summary>
        private readonly bool TypeTelechargement;
        /// <summary> couches de données possibles pour les serveurs carto </summary>
        private readonly string FichierLayersServeurs;
        /// <summary> Numuméro de la couche du layer Pentes </summary>
        private readonly byte CouchePentes;
        /// <summary> Nb possible de layers pour l'affichage de 1 à 3 </summary>
        private readonly int NbLayersAffichages;
        /// <summary> Type du serveur </summary>
        private readonly TypesServeur TypeServeur;
        /// <summary> Type du serveur </summary>
        private readonly string EPSG_Rendu;
        #endregion
        #region Données des sites web
        /// <summary> Cle d'accès aux données IGN (GéoPortail) mais il est possible de la remplacer par sa propre clé
        /// que l'on demande à IGN Professionnel pour avoir des données particulières et des services </summary>
        private const string ClefIGN = "an7nvfzojv5wa96dsga5nk8w";
        /// <summary> valeur defaut pour les UsersAgents </summary>
        private const string UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv: 42.0) Gecko/20100101 Firefox/42.0";
        /// <summary> debut d'une définition de porte (accès) pour un site OSM </summary>
        private const string DebutPorte = "https://";
        /// <summary> 3 portes d'accès diffèrentes pour les serveur de type OSM </summary>
        private static readonly string[] PorteSiteOSM = new string[] { DebutPorte + 'a', DebutPorte + 'b', DebutPorte + 'c' };
        /// <summary> datas qui définit l'accés aux données correspondant aux sites web  </summary>
        private static readonly (string TileMatrixSet, TypesServeur TypeWMTS, string EPSG, bool XY, string Url, string Referer, string UserAgent)[] DonneesSitesWebs =
            new (string, TypesServeur, string, bool, string, string, string)[] {
                ("PM", TypesServeur.KVP, "", true, $"https://wxs.ign.fr/{ClefIGN}/geoportail/wmts", "https://www.geoportail.gouv.fr/carte", UserAgent),
                ("", TypesServeur.REST, "21781", false, "https://wmts100.geo.admin.ch", "https://map.geo.admin.ch", UserAgent),
                ("PM", TypesServeur.KVP, "", true, $"https://wxs.ign.fr/{ClefIGN}/geoportail/wmts", "https://www.geoportail.gouv.fr/carte", UserAgent),
                ("", TypesServeur.OSM, "", true, ".tile-cyclosm.openstreetmap.fr/cyclosm", "https://www.cyclosm.org", UserAgent),
                ("", TypesServeur.OSM, "", true, ".tile.opentopomap.org", "https://opentopomap.org", UserAgent),
                ("GoogleMapsCompatible", TypesServeur.KVP, "", true, "http://www.ign.es/wmts/mapa-raster", "https://www.ign.es", UserAgent),
                ("", TypesServeur.REST, "2056", true, "https://wmts100.geo.admin.ch", "https://map.geo.admin.ch", UserAgent)
            };
        /// <summary> Data pour le ou les layers de l'affichage d'un site </summary>
        private static readonly (string Identifer, string Format, string Style, string Time, string Type)[][] DonneesLayersAffichages =
            new (string, string, string, string, string)[][] {
                new (string, string, string, string, string)[] {
                    ("GEOGRAPHICALGRIDSYSTEMS.MAPS", "jpeg", "normal", "", "Cartes"),
                    ("GEOGRAPHICALGRIDSYSTEMS.PLANIGNV2", "png", "normal", "", "Plans"),
                    ("ORTHOIMAGERY.ORTHOPHOTOS", "jpeg", "normal", "", "Photos") },
                new (string, string, string, string, string)[] {
                    ("ch.swisstopo.pixelkarte-farbe", "jpeg", "default", "current", "Cartes"),
                    ("ch.swisstopo.swissimage", "jpeg", "default", "current", "Photos") },
                new (string, string, string, string, string)[] {
                    ("GEOGRAPHICALGRIDSYSTEMS.MAPS", "jpeg", "normal", "", "Cartes"),
                    ("GEOGRAPHICALGRIDSYSTEMS.PLANIGNV2", "png", "normal", "", "Plans"),
                    ("ORTHOIMAGERY.ORTHOPHOTOS", "jpeg", "normal", "", "Photos") },
                new (string, string, string, string, string)[] {
                    ("", "png", "", "", "Cartes") },
                new (string, string, string, string, string)[] {
                    ("", "png", "", "", "Cartes") },
                new (string, string, string, string, string)[] {
                    ("MTN", "jpeg", "default", "", "Cartes") },
                new (string, string, string, string, string)[] {
                    ("ch.swisstopo.pixelkarte-farbe", "jpeg", "default", "current", "Cartes") }
            };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // S5
        /// <summary> Data pour la couche des pentes </summary>
        private static readonly (string Identifer, string Format, string Style, string Time, int[] IndicesEchelle)[] DonneesLayersPentes =
            new (string, string, string, string, int[])[] {
                ("GEOGRAPHICALGRIDSYSTEMS.SLOPES.MOUNTAIN", "png", "normal", "", new int[] { 0, 1, 2, 3 }),
                ("ch.swisstopo.hangneigung-ueber_30", "png", "default", "current", new int[] { 0, 1, 2, 3 }),
                ("", "", "", "", Array.Empty<int>()),
                ("", "", "", "", Array.Empty<int>()),
                ("", "", "", "", Array.Empty<int>()),
                ("", "", "", "", Array.Empty<int>()),
                ("ch.swisstopo.hangneigung-ueber_30", "png", "default", "current", new int[] { 0, 1, 2, 3 })
            };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  // S5
        #endregion
        /// <summary> permet de changer le layer des fonds de plan associé à l'affichage 
        /// Attention après cette action il faut recréer un cache sinon les tuiles déjà enregistrées
        /// dans le cache ne seront pas affectées </summary>
        private void ChangerFondPlanAffichage()
        {
            // d'abord on change le layer d'affichage
            Layers[0].Identifier = DonneesLayersAffichages[(int)SiteCarto][_IndiceFondPlan].Identifer;
            Layers[0].Format = DonneesLayersAffichages[(int)SiteCarto][_IndiceFondPlan].Format;
            Layers[0].Initialiser(Url);

            Cache.Dispose();
            Cache = null;
            Cache = new CacheTuiles(this);
        }
        /// <summary> quand il n'y a pas de layer à ajouter on ajoute le layer carte </summary>
        private bool AjouterLayersAffichage()
        {
            var L = new Layer()
            {
                Indice = 0,
                Nom = "Defaut",
                Identifier = DonneesLayersAffichages[(int)SiteCarto][_IndiceFondPlan].Identifer,
                Style = DonneesLayersAffichages[(int)SiteCarto][_IndiceFondPlan].Style,
                Format = DonneesLayersAffichages[(int)SiteCarto][_IndiceFondPlan].Format,
                Time = DonneesLayersAffichages[(int)SiteCarto][_IndiceFondPlan].Time,
                CoefAlpha = 1f
            };
            InitialiserLayer(L, false);
            return ListeLayers.Count > 0;
        }
        /// <summary> remplit le tableau des serveurs connus à partir de ceux contenus dans un fichier XML </summary>
        /// <param name="TestLayer"> Doit'on tester les couches. Si oui on teste l'existence de la couche au centre de la projection du site et à l'echelle définit </param>
        private bool AjouterLayers(bool TestLayer)
        {
            bool Ret = false;
            try
            {
                var Cpt = default(int);
                var ServeursXML = XElement.Load(FichierLayersServeurs);
                foreach (XElement ServeurXML in ServeursXML.Elements(XName.Get("Serveur")))
                {
                    string Fcgp = ServeurXML.Attribute(XName.Get("fcgp")).Value;
                    if ((Fcgp ?? "") == (Cle ?? ""))
                    {
                        // on verifie les différents couches
                        var LayersXML = ServeurXML.Element(XName.Get("Layers"));
                        foreach (XElement LayerXML in LayersXML.Elements(XName.Get("Layer")))
                        {
                            int IndiceLayer = int.Parse(LayerXML.Attribute(XName.Get("indice")).Value);
                            if (IndiceLayer > -1 & IndiceLayer < 3)
                            {
                                var Layer = new Layer()
                                {
                                    Indice = IndiceLayer,
                                    CoefAlpha = (float)StrToDbl(LayerXML.Attribute(XName.Get("visibilite")).Value),
                                    Nom = LayerXML.Element(XName.Get("Nom")).Value,
                                    Identifier = LayerXML.Attribute(XName.Get("identifiant")).Value,
                                    Format = LayerXML.Attribute(XName.Get("format")).Value,
                                    Style = LayerXML.Attribute(XName.Get("style")).Value,
                                    Time = LayerXML.Attribute(XName.Get("time")).Value
                                };
                                InitialiserLayer(Layer, TestLayer);
                                Cpt += 1;
                            }
                        }
                    }
                }
                if (ListeLayers.Count == 0)
                {
                    AjouterLayersAffichage();
                    if (SiteCarto <= SitesCartographiques.DomTom && Cpt > 0)
                    {
                        MessageInformation = "Aucun des Layers du fichier LayersServeurs.xml" + CrLf + $"n'a été validé par le serveur{Nom}" + CrLf + "Le Layer d'affichage sera utilisé pour le téléchargement";
                        AfficherInformation();
                    }
                }
                Ret = ListeLayers.Count > 0;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "L3N5");
            }
            return Ret;
        }
        /// <summary> initialise la couche et trouve les zooms associés </summary>
        /// <param name="Layer"> Layer à tester </param>
        /// <param name="FlagTestLayer"> indique si l'on doit tester si le layer existe sur le serveur </param>
        private void InitialiserLayer(Layer Layer, bool FlagTestLayer)
        {
            Layer.Initialiser(Url);
            string UriTuile = Layer.UriDeb + Layer.CalculerUriTilePosition(CouchesSite(SiteCarto, _DomTom)[Centre.IndiceEchelle], Centre.IndexTuile.X, Centre.IndexTuile.Y);
            // on provoque une erreur si le serveur ne réagit pas pas notre requête avant le délai indiqué
            if (!FlagTestLayer || TesterLayer(UriTuile, Layer.Identifier))
            {
                Layer.NbEchelles = NbEchellesSite(SiteCarto, _DomTom) - 1;
                Layer.Couches = CouchesSite(SiteCarto, _DomTom);
                Layer.Echelles = EchellesSite(SiteCarto, _DomTom);
                ListeLayers.Add(Layer.Indice, Layer);
            }
        }
        /// <summary> initialise le cache et les demandes de tuiles pour l'affichage </summary>
        /// <param name="Affichage"> Affichage qui demandera les demande de tuiles </param>
        private bool InitialiserAffichage(AffichageCarte Affichage)
        {
            Cache = new CacheTuiles(this);
            if (!Cache.IsOk)
                return false;
            this.Affichage = Affichage;
            ListeDemandesAffichage = new SortedList<int, DemandeTuiles>(50);
            return true;
        }
        /// <summary> initialise le support des requêtes </summary>
        private void InitialiserHttp()
        {
            RequeteHttp = new HttpClient() { Timeout = new TimeSpan(0, 0, 10) };
            RequeteHttp.DefaultRequestHeaders.Clear();
            RequeteHttp.DefaultRequestHeaders.Add("User-Agent", DonneesSitesWebs[(int)SiteCarto].UserAgent);
            RequeteHttp.DefaultRequestHeaders.Add("Referer", DonneesSitesWebs[(int)SiteCarto].Referer);
        }
        /// <summary> teste si la couche existe sur le serveur et trouve les zooms associés </summary>
        private bool TesterLayer(string UriTuile, string Identifier)
        {
            MessageInformation = null;
            try
            {
                int Cpt = 0;
                do
                {
                    using (var Reponse = RequeteHttp.Send(new HttpRequestMessage(HttpMethod.Get, UriTuile)))
                    {
                        if (Reponse.StatusCode == HttpStatusCode.OK)
                        {
                            // pas de message tout est normal
                            return true;
                        }
                        else if (Reponse.StatusCode == HttpStatusCode.Forbidden || Reponse.StatusCode == HttpStatusCode.BadRequest)
                        {
                            MessageInformation = $"Le Layer {Identifier} tel que défini dans{CrLf}le fichier LayersServeurs.xml n'existe pas sur le serveur {Nom}";
                            Cpt = 3;
                        }
                        else
                        {
                            Cpt += 1;
                        }
                    }
                }
                while (Cpt < 2);
                if (string.IsNullOrEmpty(MessageInformation))
                {
                    MessageInformation = $"Le Layer {Identifier} n'existe pas sur le serveur {Nom}" + CrLf + "ou problème avec le serveur";
                }
            }
            catch (Exception ex)
            {
                MessageInformation = $"Problème avec le serveur : {ex.Message}";
            }
            AfficherInformation();
            return false;
        }
        /// <summary> demande les capacités du serveur </summary>
        /// <param name="VersionWMTS"> dernier chiffre de la version WMTS. peut être à ou 1 </param>
        /// <returns> une chaine ayant le format attendu en fonction des caractéristiques du serveur </returns>
        private string ServiceCapabilities(int VersionWMTS)
        {
            if (TypeServeur == TypesServeur.KVP)
            {
                return "?Service=WMTS&Request=GetCapabilities";
            }
            else
            {
                return $"/EPSG/{EPSG_Rendu}/1.0.{VersionWMTS}/WMTSCapabilities.xml";
            }
        }
        /// <summary> demande les capacités du serveur </summary>
        /// <param name="VersionWMTS"> peut être 0 ou 1 mais 0 fonctionne très bien </param>
        /// <returns> un Xml qui peut être utilisé pour déterminer les capacités du serveur ou nothing si la demande retourne une erreur </returns>
        private async Task<bool> RequeteWebCapactiesAsync(int VersionWMTS, string NomCapacities)
        {
            try
            {
                using (var Reponse = await RequeteHttp.GetAsync(Url + ServiceCapabilities(VersionWMTS)))
                {
                    if (Reponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var S = await Reponse.Content.ReadAsStreamAsync())
                        {
                            await Task.Run(
                                () =>
                                {
                                    using (var X = XmlReader.Create(S))
                                    {
                                        var CapacitiesXML = XElement.Load(X);
                                        CapacitiesXML.Save(NomCapacities);
                                    }
                                });
                        }
                        return true;
                    }
                    else
                    {
                        MessageInformation = "Erreur lors de la requête WebCapacties" + CrLf + "Réessayez plus tard";
                        string Titre = TitreInformation;
                        TitreInformation = "Information Serveur Carto";
                        AfficherInformation();
                        TitreInformation = Titre;
                        return false;
                    }
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "V5B8");
                return false;
            }
        }
        /// <summary> permet la depose d'une carte en liberant les ressources managées et non managées </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                RequeteHttp?.Dispose();
                Cache?.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}