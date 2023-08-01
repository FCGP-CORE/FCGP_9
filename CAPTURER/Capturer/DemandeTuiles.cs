namespace FCGP
{
    internal partial class ServeurCarto
    {

        /// <summary> Décrit un layer associé à un serveur carto </summary>
        private class Layer
        {
            #region Champs publiques
            /// <summary> nb d'échelles ou de couches -1 (indice base zéro) </summary>
            internal int NbEchelles;
            /// <summary> tableau des libellés d'échelle de la couche </summary>
            internal string[] Echelles;
            /// <summary> tableau des numéros de couche </summary>
            internal byte[] Couches;
            /// <summary> indice de la couche dans le tableau des couches du serveur </summary>
            internal int Indice;
            /// <summary>nom de la couche dans le fichier xml des serveurs </summary>
            internal string Nom;
            /// <summary> identifiant de la couche </summary>
            internal string Identifier;
            /// <summary> style de la couche renvoyé par le serveur </summary>
            internal string Style;
            /// <summary> date des informations de la couche renvoyées par le serveur </summary>
            internal string Time;
            /// <summary> format graphique de la couche exprimé en type mime</summary>
            internal string Format;
            /// <summary> valeur de transparence de la couche de 0: invisible à 1: opaque </summary>
            internal float CoefAlpha;
            /// <summary> Début de l'url commun à toutes les tuiles de ce layer </summary>
            internal string UriDeb;
            /// <summary> partie de l'url correspondant au format des tuiles de ce layer </summary>
            internal string UriFormat;
            /// <summary> format graphique de la couche exprimé en image.format du .net </summary>
            internal ImageFormat FormatNet
            {
                get
                {
                    return Format.ToLower() == "png" ? ImageFormat.Png : ImageFormat.Jpeg;
                }
            }
            #endregion
            /// <summary> calule la partie numéro de la tuile </summary>
            /// <param name="Couche"> numéro de la couche serveur de la tuile </param>
            /// <param name="NumCol"> numéro de colonne de la tuile </param>
            /// <param name="NumRow"> numéro de rangée de la tuile</param>
            /// <returns> une chaine ayant le format attendu en fonction des caractéristiques du serveur </returns>
            internal string CalculerUriTilePosition(byte Couche, int NumCol, int NumRow)
            {
                string CalculerUriTilePositionRet;
                if (ServeurType == TypesServeur.KVP)
                {
                    CalculerUriTilePositionRet = $"&TileMatrix={Couche}&TileCol={NumCol}&TileRow={NumRow}";
                }
                else if (SensColRow)
                {
                    CalculerUriTilePositionRet = $"/{Couche}/{NumCol}/{NumRow}{UriFormat}";
                }
                else
                {
                    CalculerUriTilePositionRet = $"/{Couche}/{NumRow}/{NumCol}{UriFormat}";
                }

                return CalculerUriTilePositionRet;
            }
            /// <summary> initialise la chaine de début de l'adresse d'une tuile </summary>
            /// <param name="Url"> Url du serveur </param>
            internal void Initialiser(string Url)
            {
                // valeur commune à toutes les tuiles de la couche d'affichage concernant le format des tuiles
                UriFormat = FormatFichierTile();
                // valeur commune à toutes les tuiles de la couche d'affichage concernant le début de l'adresse des tuiles
                if (ServeurType == TypesServeur.KVP)
                {
                    UriDeb = $"{Url}?layer={Identifier}&style={Style}&tilematrixset={TileMatrix}&Service=WMTS&Request=GetTile&Version=1.0.0{UriFormat}";
                }
                else if (ServeurType == TypesServeur.REST)
                {
                    UriDeb = $"{Url}/1.0.0/{Identifier}/{Style}/{Time}/{EPSG}";
                }
                else // TypesServeur.OSM
                {
                    UriDeb = Url;
                }
            }
            /// <summary> calcule la partie format de la tuile </summary>
            /// <param name="Format"> "jpeg" ou "png" </param>
            /// <returns>  une chaine ayant le format attendu en fonction des caractéristiques du serveur </returns>
            private string FormatFichierTile()
            {
                string FormatFichierTileRet;
                if (ServeurType == TypesServeur.KVP)
                {
                    FormatFichierTileRet = "&Format=image/" + Format;
                }
                else // TypeRest ou TypeOSM
                {
                    FormatFichierTileRet = "." + Format;
                }

                return FormatFichierTileRet;
            }
            /// <summary> initialise les variables communes à tous les layers du serveurs </summary>
            /// <param name="TypeServeur"> le Type de serveur des layers </param>
            /// <param name="TileMatrixSet"> la matrice pour les tuiles des layers </param>
            /// <param name="EPSG_Rendu"> le numéro de la projection associé à la matrice des tuiles </param>
            /// <param name="XY"> le sens des colonnes / Rangées pour le calcul de la position </param>
            internal static void InitialiserLayers(TypesServeur TypeServeur, string TileMatrixSet, string EPSG_Rendu, bool XY)
            {
                ServeurType = TypeServeur;
                TileMatrix = TileMatrixSet;
                EPSG = EPSG_Rendu;
                SensColRow = XY;
            }
            private static TypesServeur ServeurType;
            /// <summary> nom del'ensemble des matrices de tuile par niveau de zoom associé à la couche </summary>
            private static string TileMatrix;
            /// <summary> projection cartographique du serveur </summary>
            private static string EPSG;
            /// <summary> si true les X puis les Y dans la demande, sinon on inverse </summary>
            private static bool SensColRow;
        }
        internal class DemandeTuiles
        {
            /// <summary> Définition des méthodes qui écoutent l'évenement d'une demande de tuile </summary>
            /// <param name="ID"> ID de la demande. Sera transmis à la méthode qui écoute l'évènement </param>
            internal delegate void DemandeTuilesHandler(int ID, bool IsErreur);
            /// <summary> définition de l'évènement de la tuile </summary>
            internal event DemandeTuilesHandler Finished
            {
                // Ajout d'une méthode qui écoute l'évenement
                add
                {
                    OnFinished = value;
                }
                // Suppression d'une méthode qui écoute l'évenement
                remove
                {
                    OnFinished = null;
                }
            }
            // génération de l'évènement OnFinished. il faut qu'il y ait une méthode d'écoute pour lancer l'évènement
            // On avertit si le nb d'erreur correspond au nb de demande sur le serveur
            private void RaiseEvent_Finished()
            {
                int ErreursTotales = ErreursServeur + ErreursFCGP;
                OnFinished(ID, ErreursTotales > 0 && ErreursTotales == TotalTuiles - Caches);
            }
            /// <summary> nombre de tuiles concernées par la demande </summary>
            internal int TotalTuiles { get; private set; }
            /// <summary> stats sur le nombre de tuiles en erreur, inexistantes, téléchargées, en cache, plus concerné par l'affichage et autres </summary>
            internal int ErreursServeur { get; private set; }
            internal int ErreursFCGP { get; private set; }
            internal int Inexistants { get; private set; }
            internal int Serveurs { get; private set; }
            internal int Caches { get; private set; }
            internal int PlusConcerne { get; private set; }
            internal int Autres { get; private set; }
            /// <summary> constructeur de la demande </summary>
            /// <param name="NbTuiles"> nb de tuiles concernées par la demande </param>
            /// <param name="NumCouche"> numéro de couche associé au layer d'affichage </param>
            /// <param name="ServeurCarto"> serveur carto associée à la demande </param>
            internal DemandeTuiles(int NbTuiles, byte NumCouche, ServeurCarto ServeurCarto)
            {
                ID = CptDemandesAffichage;
                CptDemandesAffichage += 1;
                TotalTuiles = NbTuiles;
                Serveur = ServeurCarto;
                Serveur._NbTuilesDemandeAffichage += TotalTuiles;
                IsPentes = NumCouche == 255;
                Couche = NumCouche;
            }
            /// <summary> Création des tuiles correspondant à la surface de la demande. Ne peut pas être appelée
            /// dans le constructeur à cause de l'asynchronisme. Doit être appelé après la création
            /// car la suppression de la demande peut avoir lieu avant le retour de la création 
            /// L'appelant peut s'abonner à l'évenement OnFinshed pour être prévenu de la réalisation de la demande </summary>
            /// <param name="Surface"> Surface exprimée en tuile </param>
            internal async Task CreerTuilesAffichageAsync(Rectangle Surface)
            {
                for (int Col = Surface.Left, loopTo = Surface.Right - 1; Col <= loopTo; Col++)
                {
                    for (int Row = Surface.Top, loopTo1 = Surface.Bottom - 1; Row <= loopTo1; Row++)
                        _ = new TuileAffichage(Col, Row, this);
                }
                // pour que le compilateur accepte async car l'asynchronisme est au niveau du constructeur de la tuile
                await Task.Delay(0);
            }
            /// <summary> résume les principales propriétés de la demande </summary>
            public override string ToString()
            {
                return $"DemandeTuile Id : {ID}, Type : {(IsPentes ? "Pentes" : "Fond")}, Nb Total Tuiles : {TotalTuiles} --> C:{Caches} S:{Serveurs} I:{Inexistants} P:{PlusConcerne} E:{ErreursServeur + ErreursFCGP}";
            }
            /// <summary> Identifiant unique de la demande </summary>
            internal int ID { get; private set; }
            /// <summary> les tuiles de la demande concerne la couche des pentes</summary>
            private readonly bool IsPentes;
            /// <summary> Numéro du serveur de la couche de fond des tuiles de la demande </summary>
            private readonly byte Couche;
            /// <summary> Numéro du serveur de la couche de fond des tuiles de la demande </summary>
            private readonly ServeurCarto Serveur;
            /// <summary> stocke les différents statut de sortie d'une tuile quand elle est finie </summary>
            /// <param name="Statut"> statut de fin de la tuile </param>
            private void EcrireStatut(StatutTuile Statut)
            {
                switch (Statut)
                {
                    case StatutTuile.ErreurServeur:
                        {
                            ErreursServeur += 1;
                            break;
                        }
                    case StatutTuile.ErreurFCGP:
                        {
                            ErreursFCGP += 1;
                            break;
                        }
                    case StatutTuile.Cache:
                        {
                            Caches += 1;
                            break;
                        }
                    case StatutTuile.Serveur:
                        {
                            Serveurs += 1;
                            break;
                        }
                    case StatutTuile.Inexistant:
                        {
                            Inexistants += 1;
                            break;
                        }
                    case StatutTuile.PlusConcerne:
                        {
                            PlusConcerne += 1;
                            break;
                        }

                    default:
                        {
                            Autres += 1;
                            break;
                        }
                }
                Interlocked.Decrement(ref Serveur._NbTuilesDemandeAffichage);
                Interlocked.Increment(ref NbTuilesFinies);
                if (NbTuilesFinies == TotalTuiles)
                    RaiseEvent_Finished();
            }
            /// <summary> Pointeur sur la ou les méthodes qui écoutent l'évènement de la demande </summary>
            private DemandeTuilesHandler OnFinished;
            /// <summary> stocke le nb de tuiles finies de la demande </summary>
            private int NbTuilesFinies;
            /// <summary> compteur des création de demande d'affichage </summary>
            private static int CptDemandesAffichage;

            /// <summary> sert pour le transport des informations nécessaires à la gestion d'une tuile servant à l'affichage
            /// et à la demande qui lui est associée à travers les threads. Concerne le fond de plan et les pentes </summary>
            internal class TuileAffichage
            {
                /// <summary> index de la colonne de la tuile </summary>
                internal readonly int Col;
                /// <summary> index de la rangéee de la tuile </summary>
                internal readonly int Row;
                /// <summary> valeur du zoom de la tuile sous forme de texte </summary>
                internal readonly string CoucheTxt;
                /// <summary> comment a été rempli le tampon de la tuile </summary>
                internal StatutTuile StatutTuile;
                /// <summary> tampon de la tuile représentant les octets d'un fichier image (jpeg) </summary>
                internal byte[] BytesTuile;
                /// <summary> constructeur de l'instance pour les couches de l'affichage et de la couche des pentes </summary>
                /// <param name="Col"> index de la colonne de la tuile </param>
                /// <param name="Row"> index de la rangéee de la tuile </param>
                /// <param name="Demande"> demande associée à la tuile </param>
                internal TuileAffichage(int Col, int Row, DemandeTuiles Demande)
                {
                    this.Col = Col;
                    this.Row = Row;
                    Index = new Point(Col, Row);
                    this.Demande = Demande;
                    Couche = Demande.Couche;
                    if (Demande.IsPentes)
                    {
                        CoucheTxt = Couche.ToString("Pentes");
                        Uri = Demande.Serveur.UriTuilePentes(Col, Row);
                        TelechargerTuilePentesAsync();
                    }
                    else
                    {
                        CoucheTxt = Couche.ToString("00");
                        Uri = Demande.Serveur.UriTuileAffichage(0, Couche, Col, Row);
                        // on cherche vraiment à télécharger car on essaie 5 fois
                        TelechargerTuileAffichageAsync();
                    }
                }
                /// <summary> télécharge et enregistre une tuile d'affichage dans le cache si elle n'est pas présente dans le cache 
                /// on redemande jusqu'à 5 fois en cas de non réponse du serveur </summary>
                private async void TelechargerTuileAffichageAsync()
                {
                    var CptErreur = default(int);
                    StatutTuile = StatutTuile.PlusConcerne;
                    do
                    {
                        // si la tuile n'est plus dans le périmètre de l'affichage on sort
                        if (!Demande.Serveur.Affichage.SurfaceFond.Contains(Index) || Couche != Demande.Serveur.Affichage.CoucheFond)
                            break;
                        await Demande.Serveur.Cache.LireOctetsTuileAsync(this);
                        if (StatutTuile != StatutTuile.DemandeTeleChargement)
                            break;
                        if (!Commun.IsConnected)
                            break;
                        // si la tuile n'est plus dans le périmètre de l'affichage on sort
                        if (!Demande.Serveur.Affichage.SurfaceFond.Contains(Index) || Couche != Demande.Serveur.Affichage.CoucheFond)
                            break;
                        // on demande le téléchargement si le nb de requêtes encours n'est pas trop important pour éviter de saturer le serveur 
                        await TelechargerBytesTuileAsync();
                        if (StatutTuile != StatutTuile.ErreurServeur)
                            break; // on a fini et bien initialisé 
                        CptErreur += 1;
                    }
                    while (CptErreur < 5); // on limite à 5 demandes sur le serveur
                    if (CptErreur == 5)
                    {
                        StatutTuile = StatutTuile.ErreurServeur;
                    }
                    BytesTuile = null;
                    Demande.EcrireStatut(StatutTuile);
                }
                /// <summary> télécharge et enregistre une tuile de la couche des pentes si elle n'est pas présente dans le cache </summary>
                private async void TelechargerTuilePentesAsync()
                {
                    if (!await Demande.Serveur.Cache.ContenirLaTuileAsync(this))
                    {
                        await TelechargerBytesTuileAsync();
                    }
                    else
                    {
                        StatutTuile = StatutTuile.Cache;
                    }
                    BytesTuile = null;
                    Demande.EcrireStatut(StatutTuile);
                }
                /// <summary> demande un tuile (image avec un format graphique compressé (jpeg, png) sous forme d'un tableau octets (blob).
                /// elle est téléchargée à partir du serveur carto de manière asynchrone </summary>
                private async Task TelechargerBytesTuileAsync()
                {
                    if (Demande.Serveur.NbRequetes[0] >= MaxRequetesTuiles)
                    {
                        StatutTuile = StatutTuile.PlusConcerne;
                    }
                    Interlocked.Increment(ref Demande.Serveur.NbRequetes[0]);
                    try
                    {
                        // lance l'action de lecture de la tuile sur le serveur cartographique et ressort
                        using (var ReponseHttp = await Demande.Serveur.RequeteHttp.GetAsync(Uri))
                        {
                            if (ReponseHttp.StatusCode == HttpStatusCode.OK)
                            {
                                BytesTuile = await ReponseHttp.Content.ReadAsByteArrayAsync();
                                StatutTuile = StatutTuile.Serveur;
                                // enregistre la tuile dans le cache
                                await Demande.Serveur.Cache.EcrireOctetsTuileAsync(this);
                            }
                            else if (ReponseHttp.StatusCode == HttpStatusCode.BadRequest || ReponseHttp.StatusCode == HttpStatusCode.NotFound)
                            {
                                BytesTuile = new byte[] { 0 };
                                StatutTuile = StatutTuile.Inexistant;
                            }
                            else
                            {
                                // le code erreur renvoyé par le serveur carto n'est pas géré
                                StatutTuile = StatutTuile.ErreurServeur;
                            }
                        }
                    }
                    catch
                    {
                        // erreur géré par HttpClient sur timeOut (10secondes).
                        // Pour certains sites l'affichage du message d'erreur est problématique car le serveur est trop long à répondre
                        // on se contente du message au niveau de la suppression de la demande de tuile si il n'y a que des erreurs
                        // Ex As Exception
                        // AfficherErreur(Ex, "D3Y0")
                        StatutTuile = StatutTuile.ErreurFCGP;
                    }
                    Interlocked.Decrement(ref Demande.Serveur.NbRequetes[0]);
                }
                /// <summary> demande associée à la tuile </summary>
                private readonly DemandeTuiles Demande;
                /// <summary> Uri de la tuile </summary>
                private readonly string Uri;
                /// <summary> indices de la tuile </summary>
                private readonly Point Index;
                /// <summary> valeur du zoom de la tuile </summary>
                private readonly byte Couche;
                /// <summary> renvoie une chaine descriptive de tuileInfo </summary>
                public override string ToString()
                {
                    return $"ID : {Demande.ID}, Col : {Col}, Row : {Row}, Couche : {CoucheTxt}, Statut : {StatutTuile}";
                }
            }
        }
    }
}