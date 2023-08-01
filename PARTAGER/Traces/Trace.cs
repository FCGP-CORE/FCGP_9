using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ConvertirCoordonnees;
using static FCGP.CurseurSouris;
using static FCGP.DonneesSiteWeb;
using static FCGP.Enumerations;
using static FCGP.Settings;
using static FCGP.SystemeCartographique;

namespace FCGP.TRKs
{
    /// <summary> Gère une trace. La trace est vide si nbsegments = 0. </summary>
    internal partial class TRK
    {
        #region Commune à toutes les instances
        #region Partie visible et commune ou shared concernant les traces
        /// <summary> signature de la fonction pour afficher les graphiques d'une trace </summary>
        /// <param name="Trace"> Trace dont on doit afficher un graphique </param>
        /// <param name="Is_Vitesse"> S'agit'il de la vitesse ou de l'altitude </param>
        /// <returns> l'index du point séléctionner en dernier et Ok si il y a eu un double click </returns>
        internal delegate (int IndexPoint, DialogResult DialogResult) AfficherGraphiqueTrace(TRK Trace, bool Is_Vitesse);

        /// <summary> determine la distance maximale de la souris à un point ou un segment pour que celui-ci soit sélectionnable </summary>
        internal static int Sensibilite
        {
            get
            {
                return _Sensibilite;
            }
            set
            {
                _Sensibilite = value;
                InitialiserLargeurPinceauxTrace();
            }
        }
        /// <summary> couleur du point lorsqu'il va être ajouté ou supprimé. Il n'apparait pas quand il n'est pas survolé </summary>
        internal static Color CouleurPointAjout
        {
            get
            {
                return PenPointAjout.Color;
            }
            set
            {
                PenPointAjout.Color = value;
            }
        }
        /// <summary> couleur du segment lorsqu'il est ajouté </summary>
        internal static Color CouleurSegmentAjout
        {
            get
            {
                return PenSegmentAjout.Color;
            }
            set
            {
                PenSegmentAjout.Color = value;
            }
        }
        /// <summary> couleur du point lorsqu'il est survolé. Il n'apparait pas quand il n'est pas survolé </summary>
        internal static Color CouleurPointSurvol
        {
            get
            {
                return PenPointSurvol.Color;
            }
            set
            {
                PenPointSurvol.Color = value;
            }
        }
        /// <summary> couleur du segment lorsqu'il est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        internal static Color CouleurSegmentSurvol
        {
            get
            {
                return PenSegmentSurvol.Color;
            }
            set
            {
                PenSegmentSurvol.Color = value;
            }
        }
        /// <summary> couleur des points lorsque le segment est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        internal static Color CouleurSegmentPointSurvol
        {
            get
            {
                return PenSegmentPointSurvol.Color;
            }
            set
            {
                PenSegmentPointSurvol.Color = value;
            }
        }
        /// <summary> couleur du segment lorsqu'il est supprimé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        internal static Color CouleurPointSuppression
        {
            get
            {
                return PenPointSuppression.Color;
            }
            set
            {
                PenPointSuppression.Color = value;
            }
        }
        /// <summary> couleur du segment lorsqu'il est supprimé </summary>
        internal static Color CouleurSegmentSuppression
        {
            get
            {
                return PenSegmentSuppression.Color;
            }
            set
            {
                PenSegmentSuppression.Color = value;
            }
        }
        /// <summary> site cartographique pour les coordonnées virtuelles des traces </summary>
        internal static SitesCartographiques SiteCarto { get; private set; }
        /// <summary> Domtom du site cartographique pour les coordonnées virtuelles des traces </summary>
        internal static DomsToms DomTom { get; private set; }

        internal static int IndiceSite { get; private set; }
        /// <summary> Echelle du site cartographique pour les coordonnées virtuelles des traces </summary>
        internal static Echelles EchelleTrks { get; private set; }
        /// <summary> les différentes sorte de format de fichier pour lire une trace </summary>
        internal enum TypesFichier : int
        {
            Aucun = 0,
            TRK = 1,
            GPX = 2
        }
        /// <summary> trace encours d'édition </summary>
        internal static TRK TraceEncours
        {
            get
            {
                if (IndexTraceEncours == -1)
                {
                    return null;
                }
                else
                {
                    return ListeTraces[IndexTraceEncours];
                }
            }
        }
        /// <summary> indique si il y a une trace qui est encours </summary>
        internal static bool IsTraceEncours
        {
            get
            {
                return IndexTraceEncours > -1;
            }
        }
        /// <summary> flag indiquant qu'il y a une trace encours et que son mode édition est autorisé </summary>
        internal static bool IsTraceEncoursModeEdition
        {
            get
            {
                return IndexTraceEncours > -1 && ListeTraces[IndexTraceEncours]._ModeEdition;
            }
        }
        /// <summary> flag indiquant qu'il y a une trace encours et qu'elle est encours d'édition </summary>
        internal static bool IsTraceEncoursEditing
        {
            get
            {
                return IndexTraceEncours > -1 && ListeTraces[IndexTraceEncours].Edition > Editions.Aucune;
            }
        }
        /// <summary> soit crée une trace vierge soit ouvre une trace existante de la collection ou un importe un fichier GPX. Par inteface utilisateur uniquement </summary>
        /// <param name="Altitude"> La trace va contenir l'information de l'altitude pour chaque point. Dans le cas FCGP_Capture dépend si l'option altiude est cochée </param>
        /// <param name="Horaire"> La trace va contenir l'information de l'horaire pour chaque point. Dans le cas FCGP_Capture toujours false </param>
        internal static bool OuvrirTraceEncours(bool Altitude = false, bool Horaire = false)
        {
            bool OuvrirTraceEncoursRet = false;
            // on ouvre le formulaire OuvrirTrace pour receuillir les intentions de l'utilisateur en transmettant la collection de traces et celle en cours
            using (var F = new FormOuvrirTrace() { CollectionTraces = ListeTraces, IndexTrace = IndexTraceEncours })
            {
                F.ShowDialog(FormApplication);
                if (F.DialogResult == DialogResult.OK)
                {
                    // si il y a une trace en cours on la sauve avant de changer
                    FermerTrace(IndexTraceEncours);
                    // on récupère les informations concernant la nouvelle trace
                    ResumeTrace R = (ResumeTrace)F.Tag;
                    if (R.Index == -1)
                    {
                        // Si c'est une nouvelle trace il faut la créer
                        var TraceNew = new TRK(100, R.Nom, Altitude, Horaire) { _IsVisible = true, CouleurSegment = R.CouleurSegment };
                        ListeTraces.Add(TraceNew);
                        IndexTraceEncours = ListeTraces.Count - 1;
                    }
                    else
                    {
                        // si la trace est dans la collection
                        IndexTraceEncours = R.Index;
                        ListeTraces[IndexTraceEncours].CouleurSegment = R.CouleurSegment;
                        // on appele ChangerSiteEchelle à travers la propriété IsVisible pour mettre les coordonnées virtuelles de la trace à jour
                        ListeTraces[IndexTraceEncours].IsVisible = true;
                    }
                    OuvrirTraceEncoursRet = true;
                }
            }

            return OuvrirTraceEncoursRet;
        }
        /// <summary> ferme une trace soit en la supprimant soit en la conservant et sauvegardant dans un fichier sur dique. 
        /// Le formulaire permet soit la sauvegarde sous soit l'export GPX avant la fermeture de la trace. Par programmation ou par inteface utilisateur </summary>
        /// <param name="FlagDialog"> si true ouvre le formulaire fermeture de trace, sinon ferme directement la trace encours </param>
        internal static bool FermerTraceEncours(bool FlagDialog)
        {
            bool FermerTraceEncoursRet = true;
            if (FlagDialog)
            {
                using (var F = new FormFermerTrace() { CollectionTraces = ListeTraces, IndexTrace = IndexTraceEncours })
                {
                    F.ShowDialog(FormApplication);
                    if (F.DialogResult == DialogResult.OK)
                    {
                        // il faut juste fermer la trace en enregistrant ls modifs 
                        FermerTrace(IndexTraceEncours);
                    }
                    else if (F.DialogResult == DialogResult.Abort)
                    {
                        // il faut supprimer la trace de la collection et son fichier sur le disque
                        SupprimerTrace(IndexTraceEncours);
                        IndexTraceEncours = -1;
                    }
                    else
                    {
                        FermerTraceEncoursRet = false;
                    }
                }
            }
            else
            {
                FermerTrace(IndexTraceEncours);
            }

            return FermerTraceEncoursRet;
        }
        /// <summary> ouvre le formulaire PropriétésTrace. Le changement du nom de la trace est traité à la fermeture du formulaire </summary>
        /// <returns> Numpoint si >-1 indique le N° du point de la trace qu'il faut afficher au centre de l'écran
        /// Return un flag indiquant si la couleur de la trace a changée et qu'il faut la repeindre </returns>
        internal static bool ChangerProprietesTraceEncours(ref int NumPoint)
        {
            using (var F = new FormProprietesTrace() { Trace = ListeTraces[IndexTraceEncours], IndexTrace = IndexTraceEncours })
            {
                F.ShowDialog(FormApplication);
                NumPoint = F.IndexPoint;
                return F.Repeindre;
            }
        }
        /// <summary> enregistre la trace en cours sous forme de fichier binaire </summary>
        internal static bool EnregistrerTraceEncours()
        {
            try
            {
                string NomFichier = Path.Combine(TracesSettings.CHEMIN_TRACE, ListeTraces[IndexTraceEncours].Nom + ".trk");
                // on enregistre la trace
                ListeTraces[IndexTraceEncours].EnregistrerTRK(NomFichier);
                return true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "L6O1");
                return false;
            }
        }
        /// <summary> change la trace encours. Intervient quand on change de site ou de Domtom. Par programmation, pas d'inteface utilisateur </summary>
        internal static void ChargerTraceCaptureSettings(Echelles Echelle)
        {
            IndexTraceEncours = -1;
            SiteCarto = CapturerSettings.SITE;
            DomTom = CapturerSettings.DOMTOM;
            IndiceSite = CapturerSettings.INDICE_SITE;
            EchelleTrks = Echelle;
            // on recherche dans la collection de traces si le nom existe
            if (!string.IsNullOrEmpty(CapturerSettings.NOM_TRACE))
            {
                for (int Cpt = 0, loopTo = ListeTraces.Count - 1; Cpt <= loopTo; Cpt++)
                {
                    if ((ListeTraces[Cpt].Nom ?? "") == (CapturerSettings.NOM_TRACE ?? ""))
                    {
                        // si on trouve le nom de la trace dans la collection on la charge comme trace encours en indiquant l'index dans la collection
                        IndexTraceEncours = Cpt;
                        // et en la rendant visible en calculant les coordonnées virtuelles propre au site et à l'échelle
                        ListeTraces[IndexTraceEncours].IsVisible = true;
                        return;
                    }
                }
                // si on sort normalement de la boucle c'est que la trace n'a pas été trouvée
                CapturerSettings.NOM_TRACE = "";
            }
        }
        /// <summary> importe une trace d'après un fichier GPX ou TRK. Dans le cas d'un fichier GPX seule la 1ère trace est importée. Les autres éléments sont ignorés </summary>
        /// <param name="FichierTrace"> Chemin complet du fichier GPX </param>
        /// <param name="TypeFichier"> sert à aiguiller le constructeur sur la bonne méthode pour déserialiser les données </param>
        internal static bool OuvrirTrace(string FichierTrace, TypesFichier TypeFichier)
        {
            bool OuvrirTraceRet = false;
            var Titre = TitreInformation;
            TitreInformation = $"Information Trace";
            try
            {
                var IsOk = false;
                var Trace = new TRK(FichierTrace, TypeFichier, ref IsOk);
                if (!IsOk)
                {
                    MessageInformation = $"Le fichier {Path.GetFileName(FichierTrace)} est corrompu.";
                    AfficherInformation();
                }
                else if (Trace.ListePoints.Count < 2) // pour avoir une trace il faut au moins un segment c'est à dire 2 points
                {
                    MessageInformation = $"Le fichier {Path.GetFileName(FichierTrace)} ne contient aucune trace valide.";
                    AfficherInformation();
                }
                else
                {
                    ListeTraces.Add(Trace);
                    OuvrirTraceRet = true;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T9Y9");
            }
            TitreInformation = Titre;
            return OuvrirTraceRet;
        }
        /// <summary> enrgistre une trace sous la forme d'un fichier TRK qui est un fichier binaire spécifique à FCGP ou GPX </summary>
        /// <param name="IndexTrace"> trace à enregistrer </param>
        /// <param name="FichierTrace"> Chemin complet du fichier pour sauvegarder la trace </param>
        internal static void EnregistrerTrace(int IndexTrace, string FichierTrace, TypesFichier TypeFichier)
        {
            if (IndexTrace > -1 && IndexTrace < ListeTraces.Count)
            {
                switch (TypeFichier)
                {
                    case TypesFichier.TRK:
                        {
                            // on cherche d'abord le nom indiquer par le chemin
                            string NewNomtrace = Path.GetFileNameWithoutExtension(FichierTrace);
                            // si il y a une différence il faut indiqué que l'enregistrement de la trace est obligatoire
                            if ((NewNomtrace ?? "") != (ListeTraces[IndexTrace].Nom ?? ""))
                            {
                                // on sauvegarde le nom et le flag de modification avant qu'on les change pour l'enregistrement
                                bool FlagModif = ListeTraces[IndexTrace].FlagIsModifie;
                                string Nomtrace = ListeTraces[IndexTrace].Nom;
                                ListeTraces[IndexTrace].FlagIsModifie = true;
                                ListeTraces[IndexTrace].Nom = NewNomtrace;
                                ListeTraces[IndexTrace].EnregistrerTRK(FichierTrace);
                                // on remet la trace en cours à son état initial
                                ListeTraces[IndexTrace].FlagIsModifie = FlagModif;
                                ListeTraces[IndexTrace].Nom = Nomtrace;
                                // si la trace est enregistrée dans le répertoire de la collection on l'ajoute à la collection pour qu'elle apparaissent dans la liste de choix de l'ouverture
                                if ((Path.GetDirectoryName(FichierTrace) ?? "") == (TracesSettings.CHEMIN_TRACE ?? ""))
                                {
                                    var IsOk = default(bool);
                                    var Trace = new TRK(FichierTrace, TypeFichier, ref IsOk);
                                    ListeTraces.Add(Trace);
                                }
                            }
                            else if ((Path.GetDirectoryName(FichierTrace) ?? "") == (TracesSettings.CHEMIN_TRACE ?? ""))
                            {
                                // cas de l'enregistrement normal. Si la trace a été modifié elle sera enregistrée
                                ListeTraces[IndexTrace].EnregistrerTRK(FichierTrace);
                            }
                            else
                            {
                                // cas de l'enregistrement dans un autre répertoire et il faut rendre l'enregistrement obligatoire. On sauvegarde le flag de modification
                                bool FlagModif = ListeTraces[IndexTrace].FlagIsModifie;
                                ListeTraces[IndexTrace].FlagIsModifie = true;
                                ListeTraces[IndexTrace].EnregistrerTRK(FichierTrace);
                                // on remet la trace en cours à son état initial
                                ListeTraces[IndexTrace].FlagIsModifie = FlagModif;
                            }

                            break;
                        }
                    case TypesFichier.GPX:
                        {
                            // il n'y a pas de gestion liée à une collection c'est beaucoup plus simple
                            ListeTraces[IndexTrace].EnregistrerGPX(FichierTrace);
                            break;
                        }
                }
            }
        }
        /// <summary> supprime la trace de la collection (fichier trk inclus) </summary>
        /// <param name="IndexTrace"> index de la trace à supprimer </param>
        internal static void SupprimerTrace(int IndexTrace)
        {
            if (IndexTrace > -1 && IndexTrace < ListeTraces.Count)
            {
                string FichierTrace = Path.Combine(TracesSettings.CHEMIN_TRACE, ListeTraces[IndexTrace].Nom + ".trk");
                // on supprime le fichier si il existe
                if (File.Exists(FichierTrace))
                    File.Delete(FichierTrace);
                // on supprime la trace de la collection 
                ListeTraces[IndexTrace] = null; // on met la trace à null pour le GC
                ListeTraces.RemoveAt(IndexTrace); // et on la supprime de la collection
            }
        }
        /// <summary> change le nom de la trace avec le nouveau nom </summary>
        /// <param name="IndexTrace"> index de la trace dont on doit changer le nom </param>
        /// <param name="NouveauNom"> nouveau nom de la trace </param>
        internal static void RenommerTrace(int IndexTrace, string NouveauNom)
        {
            if (IndexTrace > -1 && IndexTrace < ListeTraces.Count)
            {
                string FichierTrace = Path.Combine(TracesSettings.CHEMIN_TRACE, ListeTraces[IndexTrace].Nom + ".trk");
                if (File.Exists(FichierTrace))
                    File.Delete(FichierTrace); // on supprime le fichier trk existant
                ListeTraces[IndexTrace].Nom = NouveauNom; // on change le nom 
                ListeTraces[IndexTrace].FlagIsModifie = true; // et on marque qu'elle est modifiée pour être sur quelle soit sauvegardée
            }
        }
        /// <summary> renvoie true si le nom passé en paramètre existe dans la collection </summary>
        /// <param name="NomTrace"> nom de la trace à chercher </param>
        internal static bool IsExisteNomTrace(string NomTrace)
        {
            foreach (TRK Trk in ListeTraces)
            {
                if ((Trk.Nom ?? "") == (NomTrace ?? ""))
                    return true;
            }
            return false;
        }
        /// <summary> renvoi un nom unique basé sur le nom passé en paramètre. Le nom renvoyé n'existe pas dans la collection </summary>
        /// <param name="NomTrace"> nom de base </param>
        public static string RenvoyerNomTraceUnique(string NomTrace)
        {
            int Cpt = 1;
            string Nom = NomTrace;
            while (IsExisteNomTrace(Nom))
            {
                Nom = NomTrace + Cpt.ToString("_00");
                Cpt += 1;
            }
            return Nom;
        }
        /// <summary> Change le site ou l'échelle des coordonnées virtuelles </summary>
        /// <param name="NouveauSiteCarto"> Nouveau Site carto </param>
        /// <param name="NouvelleEchelle"> Nouvelle Echelle </param>
        /// <param name="FlagTraceEncours"> flag qui indique que seule la trace encours est concernée (FCGP_Capture) sinon l'ensemble des traces (FCGP_Visue) </param>
        internal static void ChangerSiteEchelleTraces(SitesCartographiques NouveauSiteCarto, DomsToms NouveauDomTom, Echelles NouvelleEchelle, bool FlagTraceEncours)
        {
            SiteCarto = NouveauSiteCarto;
            DomTom = NouveauDomTom;
            IndiceSite = SiteDomTomToIndex(NouveauSiteCarto, NouveauDomTom);
            EchelleTrks = NouvelleEchelle;
            if (FlagTraceEncours && IndexTraceEncours > -1)
            {
                ListeTraces[IndexTraceEncours].ChangerSiteEchelle();
            }
            else if (!FlagTraceEncours)
            {
                foreach (TRK Trace in ListeTraces)
                {
                    if (Trace._IsVisible)
                        Trace.ChangerSiteEchelle();
                }
            }
        }
        /// <summary> dessine la trace encours ou l'ensemble des traces de la collection qui sont visibles à l'affichage </summary>
        /// <param name="Graph"> surface de dessin </param>
        /// <param name="BoundingBoxVirtuelleVisue"> coordonnées virtuelles de la surface de dessin. </param>
        /// <param name="FlagTraceEncours"> flag qui indique que seule la trace encours est concernée (FCGP_Capture) sinon l'ensemble des traces (FCGP_Visue) </param>
        internal static void DessinerTraces(Graphics Graph, Rectangle BoundingBoxVirtuelleVisue, bool FlagTraceEncours)
        {
            // on ne dessine que la trace encours
            if (FlagTraceEncours && IndexTraceEncours > -1)
            {
                ListeTraces[IndexTraceEncours].Dessine(Graph);
            }
            else
            {
                // on dessine toutes les traces si elles traversent la surface de dessin
                for (int Index = 0, loopTo = ListeTraces.Count - 1; Index <= loopTo; Index++)
                {
                    if (ListeTraces[Index]._IsVisible && ListeTraces[Index].BoundingBoxVirtuel.IntersectsWith(BoundingBoxVirtuelleVisue))
                    {
                        ListeTraces[Index].Dessine(Graph);
                    }
                }
            }
        }
        /// <summary> Initialise les pinceaux pour le dessin des traces et initialise la collection des traces </summary>
        /// <param name="AppelAffichegraphique"> méthode pour l'affichage d'un graphique associé à une trace </param>
        internal static void InitialiserTraces(AfficherGraphiqueTrace AppelAffichegraphique = null)
        {
            // 1ère initialisation à l'ouverture de l'application
            if (ListeTraces is null)
            {
                AfficheGraphique = AppelAffichegraphique;
                IndexTraceEncours = -1;
                _Sensibilite = TracesSettings.SENSIB;
                PenPointSurvol = new Pen(TracesSettings.PT_SV, (float)(_Sensibilite * 4));
                PenSegmentSurvol = new Pen(TracesSettings.SEG_SV, (float)(_Sensibilite * 2)) { LineJoin = LineJoin.Round };
                PenSegmentPointSurvol = new Pen(TracesSettings.SEG_PT_SV, (float)(_Sensibilite * 3));
                PenPointSuppression = new Pen(TracesSettings.PT_SP, (float)(_Sensibilite * 4));
                PenSegmentSuppression = new Pen(TracesSettings.SEG_SP, (float)(_Sensibilite * 4)) { LineJoin = LineJoin.Round };
                PenPointAjout = new Pen(TracesSettings.PT_AJ, (float)(_Sensibilite * 4));
                PenSegmentAjout = new Pen(TracesSettings.SEG_AJ, (float)_Sensibilite) { LineJoin = LineJoin.Round };
                PenTriangle = new Pen(Color.Black, _Sensibilite * 2) { EndCap = LineCap.ArrowAnchor };
                c_CouleurSegment = TracesSettings.SEG_TRK;
            }
            // initialisation de la collection des traces avec les fichiers trace existants dans le répertoire de la collection
            FermerTrace(IndexTraceEncours);
            ListeTraces = new List<TRK>(25);
            foreach (string FichierTRK in Directory.GetFiles(TracesSettings.CHEMIN_TRACE, "*.trk", SearchOption.TopDirectoryOnly))
                OuvrirTrace(FichierTRK, TypesFichier.TRK);
        }
        /// <summary> Sauvegarde automatique de la trace encours pour ouverture automatique lors de la prochaine utilisation </summary>
        internal static void CloturerTraces()
        {
            // on sauvegarde automatiquement la trace en cours que si elle a au moins un segment
            if (ListeTraces is not null)
                CapturerSettings.NOM_TRACE = FermerTrace(IndexTraceEncours);
            // utile pour enregistrer les changements associé au settings des traces
            TracesSettings?.Ecrire();
            if (PenPointSurvol is not null)
            {
                PenPointSurvol.Dispose();
                PenSegmentSurvol.Dispose();
                PenSegmentPointSurvol.Dispose();
                PenPointSuppression.Dispose();
                PenSegmentSuppression.Dispose();
                PenPointAjout.Dispose();
                PenTriangle.Dispose();
            }
        }
        #endregion
        #region export kml
        /// <summary> renvoie une chaine de caratère formater en XML au format Style Map KML  1 ou 2 fois </summary>
        /// <param name="NbTraces"> nb de fois que l'on doit répeter styleMap</param>
        /// <param name="CouleurTrace1"> Couleur de la trace pour la 1ère fois</param>
        /// <param name="EpaisseurTrace1"> Epaisseur de la trace pour la 1ère fois</param>
        /// <param name="Couleurtrace2"> Couleur de la trace pour la 2ème fois</param>
        /// <param name="EpaisseurTrace2"> Epaisseur de la trace pour la 2ème fois</param>
        internal static StringBuilder AjouterStyleKML(int NbTraces, Color CouleurTrace1, int EpaisseurTrace1, Color Couleurtrace2, int EpaisseurTrace2)
        {
            var S = new StringBuilder(500);
            try
            {
                string CouleurStr = CouleurToStrKML(CouleurTrace1);
                S.Append(StyleLineKML("0", CouleurStr, EpaisseurTrace1.ToString("0")));
                S.Append(StyleLineKML("1", CouleurStr, EpaisseurTrace1.ToString("0")));
                S.Append(StyleMapKML("", "0", "1"));
                if (NbTraces == 2)
                {
                    CouleurStr = CouleurToStrKML(Couleurtrace2);
                    if (CouleurStr is null)
                        return new StringBuilder();
                    S.Append(StyleLineKML("2", CouleurStr, EpaisseurTrace2.ToString("0")));
                    S.Append(StyleLineKML("3", CouleurStr, EpaisseurTrace2.ToString("0")));
                    S.Append(StyleMapKML("4", "2", "3"));
                }
                return S;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "D5W9");
                return new StringBuilder();
            }
        }
        /// <summary> renvoie une chaine de caractères formater en XML au format path KML </summary>
        /// <param name="NbTraces"> nb de fois que l'on incopore les points de la trace dans le fichiers kml</param>
        /// <param name="Trace"> trace à incorporer </param>
        internal static StringBuilder AjouterTraceKML(int NbTraces, TRK Trace)
        {
            var S = new StringBuilder(25000); // , NbCarateres As Integer = 0
            try
            {
                S.Append(LineStringKMLDebut(""));
                var SF = LineStringKMLFin(Trace);
                S.Append(SF);
                if (NbTraces == 2)
                {
                    S.Append(LineStringKMLDebut("4"));
                    S.Append(SF);
                }
                return S;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "K5J8");
                return new StringBuilder();
            }
        }
        /// <summary> renvoie une chaine de caractères formater en XML au format path KML mais uniquement la partie avant les points </summary>
        /// <param name="StyleMap"> styleMap du path KML </param>
        private static StringBuilder LineStringKMLDebut(string StyleMap)
        {
            var S = new StringBuilder(200);
            try
            {
                S.AppendLine("<Placemark>");
                S.AppendLine("  <name>Path</name>");
                S.AppendLine("  <open>1</open>");
                S.AppendLine("  <styleUrl>#lineStyle" + StyleMap + "</styleUrl>");
                S.AppendLine("  <LineString>");
                S.AppendLine("    <tessellate>1</tessellate>");
                S.AppendLine("    <coordinates>");
                return S;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "G0Y5");
                return new StringBuilder();
            }
        }
        /// <summary> renvoie une chaine de caractères formater en XML au format path KML mais uniquement la partie points  jusqu'à la fin </summary>
        /// <param name="Trace"> trace à incorporer </param>
        private static StringBuilder LineStringKMLFin(TRK Trace)
        {
            var S = new StringBuilder(25000);
            var D = DatumSiteWeb((int)Trace.Site);
            try
            {
                for (int Cpt = 0, loopTo = Trace.ListePoints.Count - 1; Cpt <= loopTo; Cpt++)
                {
                    short Altitude = Trace.IsAltitude ? Trace.ListeAttributs[Cpt].Altitude : (short)0;
                    var PointDD = PointPixelToPointGrille(Trace.ListePoints[Cpt], Trace.Site, Trace.Echelle);
                    PointDD = PointGrilleToPointDD(PointDD, D);
                    S.AppendLine(DblToStr(PointDD.X, "N8") + "," + DblToStr(PointDD.Y, "N8") + "," + Altitude);
                }
                S.AppendLine("    </coordinates>");
                S.AppendLine("  </LineString>");
                S.AppendLine("</Placemark>");
                return S;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "B0Q6");
                return new StringBuilder();
            }
        }
        /// <summary> renvoie une chaine de caractères formater en XML au format StyleLine KML </summary>
        /// <param name="Num"> Numéro du style </param>
        /// <param name="Couleur"> Couleur du style </param>
        /// <param name="Epaisseur"> Epaisseur du style </param>
        private static StringBuilder StyleLineKML(string Num, string Couleur, string Epaisseur)
        {
            var S = new StringBuilder(500);
            try
            {
                S.AppendLine("<Style id=\"lineStyle" + Num + "\">");
                S.AppendLine("  <LineStyle>");
                S.AppendLine("    <color>" + Couleur + "</color>");
                S.AppendLine("    <width>" + Epaisseur + "</width>");
                S.AppendLine("  </LineStyle>");
                S.AppendLine("</Style>");
                return S;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "X2S5");
                return new StringBuilder();
            }
        }
        /// <summary> renvoie une chaine de caractères formater en XML au format StyleMap KML </summary>
        /// <param name="Num"> numéro du style </param>
        /// <param name="StyleLine1"> styline normal </param>
        /// <param name="StyleLine2"> styline Highlight </param>
        private static StringBuilder StyleMapKML(string Num, string StyleLine1, string StyleLine2)
        {
            var S = new StringBuilder(500);
            try
            {
                S.AppendLine("<StyleMap id=\"lineStyle" + Num + "\">");
                S.AppendLine("  <Pair>");
                S.AppendLine("    <key>normal</key>");
                S.AppendLine("    <styleUrl>#lineStyle" + StyleLine1 + "</styleUrl>");
                S.AppendLine("  </Pair>");
                S.AppendLine("  <Pair>");
                S.AppendLine("    <key>highlight</key>");
                S.AppendLine("    <styleUrl>#lineStyle" + StyleLine2 + "</styleUrl>");
                S.AppendLine("  </Pair>");
                S.AppendLine("</StyleMap>");
                return S;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "X9N3");
                return new StringBuilder();
            }
        }
        /// <summary> renvoie une chaine de caractère HEX représentant la couleur en ABGR </summary>
        /// <param name="Couleur"> Couleur à renvoyer </param>
        private static string CouleurToStrKML(Color Couleur)
        {
            try
            {
                var C = Color.FromArgb(Couleur.A, Couleur.B, Couleur.G, Couleur.R);
                return C.ToArgb().ToString("X");
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "I5F4");
                return null;
            }
        }
        #endregion
        #region Champs, propriétés, Enum, méthodes, fonctions partagés et privés
        /// <summary> délégué à invoquer pour l'affichage des graphiques d'une trace</summary>
        private static AfficherGraphiqueTrace AfficheGraphique;
        /// <summary> collection des traces recensées dans le réperoire des traces yc la trace encours </summary>
        private static List<TRK> ListeTraces;
        /// <summary> index dans la collection de la trace encours. -1 si pas de trace encours </summary>
        private static int IndexTraceEncours;
        /// <summary> les différentes sorte d'édition d'une trace </summary>
        private enum Editions : int
        {
            Aucune = 0,
            AjouterDeplacer = 1,
            Supprimer = 2,
            Couper = 3
        }
        /// <summary> actions possible lors de l'édition d'une trace </summary>
        private enum Actions : int
        {
            Aucune = 0,
            AjouterPointFin = 1,
            AjouterPointSegment = 2,
            DeplacerPoint = 3,
            SupprimerPoint = 4,
            SuprimerSegment = 5,
            CouperPoint = 6,
            CouperSegment = 7
        }
        /// <summary> sensibilité de la souris pour le survol et la sélection d'un point ou d'un segment exprimé en pixel </summary>
        private static int _Sensibilite;
        /// <summary> pinceau du point lorsqu'il va être ajouté ou supprimé. Il n'apparait pas quand il n'est pas survolé </summary>
        private static Pen PenPointAjout;
        /// <summary> pinceau du segment lorsqu'il est va être ajouté </summary>
        private static Pen PenSegmentAjout;
        /// <summary> pinceau du point lorsqu'il est survolé. </summary>
        private static Pen PenPointSurvol;
        /// <summary> pinceau du segment lorsqu'il est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        private static Pen PenSegmentSurvol;
        /// <summary> pinceau des points lorsque le segment est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        private static Pen PenSegmentPointSurvol;
        /// <summary> pinceau du segment lorsqu'il va être supprimé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        private static Pen PenPointSuppression;
        /// <summary> pinceau du segment lorsqu'il va être supprimé </summary>
        private static Pen PenSegmentSuppression;
        /// <summary> pinceau des triangles (flêches) indiquant le sens de la trace </summary>
        private static Pen PenTriangle;
        /// <summary> couleur par defaut de la trace </summary>
        private static Color c_CouleurSegment;
        /// <summary> change la largeur des pinceaux par defaut et des pinceuax de dessins de chaque trace dans la collection </summary>
        private static void InitialiserLargeurPinceauxTrace()
        {
            foreach (TRK T in ListeTraces)
                T.PenSegment.Width = _Sensibilite;
            PenPointSurvol.Width = _Sensibilite * 4;
            PenSegmentSurvol.Width = _Sensibilite * 2;
            PenSegmentPointSurvol.Width = _Sensibilite * 3;
            PenPointSuppression.Width = _Sensibilite * 4;
            PenSegmentSuppression.Width = _Sensibilite * 4;
            PenPointAjout.Width = _Sensibilite * 4;
            PenSegmentAjout.Width = _Sensibilite;
            PenTriangle.Width = _Sensibilite * 2;
        }
        /// <summary> doit être appeler uniquement par les functions OuvrirTraceEncours ou FermerTraceEncours </summary>
        /// <param name="IndexTrace"></param>
        private static string FermerTrace(int IndexTrace)
        {
            string Ret = "";
            if (IndexTrace > -1 && IndexTrace < ListeTraces.Count)
            {
                if (ListeTraces[IndexTrace].NbSegments > 0)
                {
                    Ret = ListeTraces[IndexTrace].Nom;
                    string NomFichier = Path.Combine(TracesSettings.CHEMIN_TRACE, Ret + ".trk");
                    // on enregistre la trace
                    ListeTraces[IndexTrace].EnregistrerTRK(NomFichier);
                }
                else
                {
                    SupprimerTrace(IndexTrace);
                }
                IndexTraceEncours = -1;
            }
            return Ret;
        }
        #endregion
        #endregion
        #region Instance
        #region champs privés de l'instance
        /// <summary> touches pour les différentes Edition de la trace </summary>
        private const Keys ToucheAjouteDeplace = Keys.A;
        private const Keys ToucheSupprime = Keys.S;
        private const Keys ToucheCoupe = Keys.C;
        /// <summary> flag indiquant que la trace a été modiée depuis sa lecture et qu'il faut la sauvegarder si on demande son enregistrement </summary>
        private bool FlagIsModifie;
        /// <summary> site des coordonnées virtuelles et grille </summary>
        private SitesCartographiques Site;
        private Echelles Echelle { get; set; }
        /// <summary> renvoie true si la trace à au moins un attribut </summary>
        private bool IsAttribut;
        // ''' <summary> stockage des attributs des points de la trace </summary>
        private List<Attribut> ListeAttributs;
        /// <summary> stockage des coordonnées virtuelles des points de la trace </summary>
        private List<Point> ListePoints;
        /// <summary> stockage des coordonnées grille (indépendantes de l'échelle) des points de la trace </summary>
        private List<PointD> ListeCoordonnees;
        /// <summary> index du point survolé avec le curseur de souris en mode édition </summary>
        private int PointSurvol;
        /// <summary> index du segment de trace survolé avec le curseur de souris en mode édition </summary>
        private int SegmentSurvol;
        /// <summary> indique quel sorte d'édition est en cours </summary>
        private Editions Edition;
        /// <summary> dernière position connue du curseur de souris en coordonnées virtuelles </summary>
        private Point PosMouse;
        /// <summary> attribut associé au point de la souris lors du click </summary>
        private Attribut Attribut;
        /// <summary> Est ce que la trace est affichée sur la carte </summary>
        private bool _IsVisible;
        /// <summary> pinceau des segments "normaux" de la trace </summary>
        private Pen PenSegment;
        /// <summary> flag indiquant que la trace est en mode édition donc peut être éditée </summary>
        private bool _ModeEdition;
        /// <summary> flag indiquant si une action est encours. Dans ce cas il faut attendre qu'elle soit finie pour en définir une autre </summary>
        private bool IsActionEncours;
        /// <summary> indique quelle action est en cours </summary>
        private Actions Action;
        #endregion
        #region constructeurs et méthodes associées
        /// <summary> constructeur pour l'import d'une trace existante sous forme de fichier GPX ou TRK </summary>
        /// <param name="Fichier"> chemin complet du fichier contenant la trace à importer </param> 
        /// <param name="TypeFichier"> Type de fichier à ouvrir </param>
        /// <param name="IsOk"> indique si le chargement de la trace n'a provoqué une erreur </param>
        private TRK(string Fichier, TypesFichier TypeFichier, ref bool IsOk)
        {
            switch (TypeFichier)
            {
                case TypesFichier.TRK:
                    {
                        IsOk = ChargerTRK(Fichier);
                        break;
                    }
                case TypesFichier.GPX:
                    {
                        IsOk = ChargerGPX(Fichier);
                        break;
                    }
            }
        }
        /// <summary> import d'une trace existante sous forme de fichier GPX </summary>
        /// <param name="FichierGPX"> fichier contenant la trace à importer </param>
        /// <returns> true si il n'y a pas d'erreur </returns>
        private bool ChargerGPX(string FichierGPX)
        {
            bool ChargerGPXRet;
            var Titre = TitreInformation;
            TitreInformation = $"Information GPX";
            try
            {
                var GPX = XElement.Load(FichierGPX);
                XNamespace xmlnsimport = GPX.Attribute(XName.Get("xmlns")).Value;
                Site = SiteCarto; // on met à jour les différentes propriétés de la trace
                Echelle = Echelles._000;
                _IsVisible = true;
                BoundingBoxDD = new RectangleD(360d, -180d, -360d, 180d);
                IntialiserTrace(50, c_CouleurSegment); // on dimensionne petit au départ
                // nom par défaut si il n'y a pas de trace mais que des points
                Nom = RenvoyerNomTraceUnique(Path.GetFileNameWithoutExtension(FichierGPX));
                var Traces = GPX.Elements(xmlnsimport.GetName("trk"));
                if (Traces.Any()) // il y a au moins une trace
                {
                    foreach (var Trace in Traces) // si une trace est déclarée on l'importe mais on ne prend que la 1ère
                    {
                        foreach (XElement ElementTrace in Trace.Elements()) // on cherche les éléments présents
                        {
                            if (ElementTrace.Name.LocalName == "name")
                            {
                                Nom = VerifierNomFichier(Trace.Element(xmlnsimport.GetName("name")).Value);
                                Nom = RenvoyerNomTraceUnique(Nom);
                            }
                            else if (ElementTrace.Name.LocalName == "trkseg") // on regarde si il y doit y avoir une collection de point
                            {
                                // si oui on récupère la collection des points de trace
                                var TracePoints = Trace.Element(xmlnsimport.GetName("trkseg")).Elements(xmlnsimport.GetName("trkpt"));
                                if (TracePoints is not null)
                                    ImporterPointsGPX(TracePoints, xmlnsimport, 2);
                            }
                        }
                        if (Traces.Count() > 1)
                        {
                            MessageInformation = "Attention il y a plusieurs traces dans le fichier GPX" + CrLf + "Seule la première est importée.";
                            AfficherInformation();
                            break;
                        }
                    }
                }
                else
                {
                    var RTEs = GPX.Elements(xmlnsimport.GetName("rte"));
                    if (RTEs.Any()) // il y a au moins une trace
                    {
                        XNamespace xmlnsextensions = GPX.Attribute(XNamespace.Xmlns.GetName("gpxx")).Value;
                        foreach (var RTE in RTEs) // si une trace est déclarée on l'importe mais on ne prend que la 1ère
                        {
                            foreach (XElement ElementRTE in RTE.Elements()) // on cherche les éléments présents
                            {
                                if (ElementRTE.Name.LocalName == "name")
                                {
                                    Nom = VerifierNomFichier(RTE.Element(xmlnsimport.GetName("name")).Value);
                                    Nom = RenvoyerNomTraceUnique(Nom);
                                }
                                else if (ElementRTE.Name.LocalName == "rtept") // on regarde si il y a un point de départ ou d'arrivée
                                {
                                    // si oui on récupère la collection des points de trace
                                    var PointDD = new PointD(StrToDbl(ElementRTE.Attribute(XName.Get("lon")).Value), StrToDbl(ElementRTE.Attribute(XName.Get("lat")).Value));
                                    ListeCoordonnees.Add(PointDDToPointGrille(PointDD, DatumSiteWeb((int)Site)));
                                    ListePoints.Add(Point.Empty);
                                    if (ListeCoordonnees.Count == 1)
                                        BoundingBoxDD = new RectangleD(PointDD.X, PointDD.Y, PointDD.X, PointDD.Y);
                                    // on récupère la collection des points
                                    var EXTENSION = ElementRTE.Element(xmlnsimport.GetName("extensions"));
                                    if (EXTENSION is not null)
                                    {
                                        var RPTs = EXTENSION.Elements(xmlnsextensions.GetName("RoutePointExtension")).Elements(xmlnsextensions.GetName("rpt"));
                                        if (RPTs is not null)
                                            ImporterPointsGPX(RPTs, xmlnsextensions, 0);
                                    }
                                }
                            }
                            if (RTEs.Count() > 1)
                            {
                                MessageInformation = "Attention il y a plusieurs routes dans le fichier GPX" + CrLf + "Seule la première est importée.";
                                AfficherInformation();
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageInformation = "Le fichier GPX ne contient ni trace ni route." + CrLf + "Voulez vous essayer d'importer des points en tant que trace?.";
                        if (AfficherConfirmation() == DialogResult.OK)
                        {
                            // sinon on récupère la collection des points 
                            var WPTs = GPX.Elements(xmlnsimport.GetName("wpt"));
                            if (WPTs is not null)
                                ImporterPointsGPX(WPTs, xmlnsimport, 2);
                        }
                    }
                }
                ChargerGPXRet = true; // OK dans le sens il n'y a pas d'erreur à la lecture du fichier GPX
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "R7D1");
                ChargerGPXRet = false;
            }
            TitreInformation = Titre;
            return ChargerGPXRet;
        }
        /// <summary> importe les points GPX à partir d'une collection </summary>
        /// <param name="CollectionPoints"> collection de points GPX </param>
        /// <param name="xmlnsimport"> le xnamespace import du document xml</param>
        /// <param name="TypeAttribut"> indique si'il faut prendre en compte les attributs Altiude et Time s'ils sont présents </param>
        private void ImporterPointsGPX(IEnumerable<XElement> CollectionPoints, XNamespace xmlnsimport, int TypeAttribut)
        {
            try
            {
                int NbPoints = CollectionPoints.Count();
                if (NbPoints > 1) // mais pour que la trace soit importée il faut au moin un segment c'est à dire 2 points
                {
                    double MinLon = BoundingBoxDD.Pt0.X;
                    double MaxLon = BoundingBoxDD.Pt2.X;
                    double MinLat = BoundingBoxDD.Pt2.Y;
                    double MaxLat = BoundingBoxDD.Pt0.Y;
                    if (TypeAttribut > 0)
                    {
                        foreach (XElement E in CollectionPoints.First().Elements()) // on cherche les attributs associés à un point
                        {
                            if (TypeAttribut > 0 && E.Name.LocalName == "ele") // il peut y avoir l'altitude
                            {
                                IsAltitude = true;
                            }
                            else if (TypeAttribut > 1 && E.Name.LocalName == "time") // et la date et heure de création
                            {
                                IsHoraire = true;
                            }
                        }
                    }
                    var Datum = DatumSiteWeb((int)Site);
                    IntialiserTrace(NbPoints, c_CouleurSegment); // on étend la capacité des différentes liste de points pour éviter un redim des différents tableaux
                    // variables servant à calculer le boudingboxDD de la trace
                    foreach (XElement Pt in CollectionPoints)
                    {
                        var PointDD = new PointD(StrToDbl(Pt.Attribute(XName.Get("lon")).Value), StrToDbl(Pt.Attribute(XName.Get("lat")).Value));
                        ListeCoordonnees.Add(PointDDToPointGrille(PointDD, Datum));
                        if (PointDD.X < MinLon)
                            MinLon = PointDD.X;
                        if (PointDD.X > MaxLon)
                            MaxLon = PointDD.X;
                        if (PointDD.Y < MinLat)
                            MinLat = PointDD.Y;
                        if (PointDD.Y > MaxLat)
                            MaxLat = PointDD.Y;
                        ListePoints.Add(Point.Empty);
                        if (IsAttribut)
                        {

                            var AttributEnCours = new Attribut();
                            if (IsAltitude)
                                AttributEnCours.Altitude = (short)Math.Round(StrToDbl(Pt.Element(xmlnsimport.GetName("ele")).Value));
                            if (IsHoraire)
                                AttributEnCours.Horaire = Convert.ToDateTime(Pt.Element(xmlnsimport.GetName("time")).Value);
                            ListeAttributs.Add(AttributEnCours);
                        }
                    }
                    BoundingBoxDD = new RectangleD(MinLon, MaxLat, MaxLon, MinLat);
                    FlagIsModifie = true; // afin de permettre l'enregistrement sous forme binaire
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "F1I7");
            }
        }
        /// <summary> enregistre la trace sous forme de fichier GPX </summary>
        /// <param name="NomFichierGPX"> Chemin complet du fichier GPX </param>
        private void EnregistrerGPX(string NomFichierGPX)
        {
            XNamespace xmlnsexport = "http://www.topografix.com/GPX/1/0";
            // élémént XML pour contenir les points de la trace
            var TRKSEG = new XElement(xmlnsexport + "trkseg");
            var Datum = DatumSiteWeb((int)Site);
            // on sérialisee chaque point en parcourant la liste du début à la fin
            for (int PointEncours = 0, loopTo = ListePoints.Count - 1; PointEncours <= loopTo; PointEncours++)
            {
                // élément XML pour contenir un point de la trace
                var PointDD = PointGrilleToPointDD(ListeCoordonnees[PointEncours], Datum);
                var TRKPT = new XElement(xmlnsexport + "trkpt", new XAttribute("lat", (object)PointDD.Y), new XAttribute("lon", (object)PointDD.X));
                if (IsAttribut)
                {
                    if (IsAltitude == true)
                        TRKPT.Add(new XElement(xmlnsexport + "ele", ListeAttributs[PointEncours].Altitude));
                    // on transforme le temps ordinateur en temps universel
                    if (IsHoraire == true)
                        TRKPT.Add(new XElement(xmlnsexport + "time", ListeAttributs[PointEncours].Horaire.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'")));
                }
                TRKSEG.Add(TRKPT);
            }
            // définit un xml GPX en écriture
            var xws = new XmlWriterSettings()
            {
                CloseOutput = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false,
                Indent = true
            };
            // détermine les attributs du GPX 
            var GPX_VERSION = new XAttribute("version", "1.0");
            var GPX_CREATOR = new XAttribute("creator", "FCGP_Capturer - fcgp@laposte.net");
            var GPX_XSI = new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance");
            var GPX_XMLNS = new XAttribute("xmlns", "http://www.topografix.com/GPX/1/0");
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            var GPX_SCHEMA = new XAttribute(xsi + "schemalocation", "http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd");
            // détermine l'élément XML pour contenir la trace
            var TRK = new XElement(xmlnsexport + "trk", new XElement(xmlnsexport + "name", Nom), TRKSEG);
            // détermine l'élément XML du fichier GPX
            var GPX = new XElement(xmlnsexport + "gpx", GPX_VERSION, GPX_CREATOR, GPX_XMLNS, GPX_XSI, GPX_SCHEMA, TRK);
            using (var xw = XmlWriter.Create(NomFichierGPX, xws))
            {
                GPX.Save(xw);
            }
        }
        /// <summary> charge la nouvelle trace à partir d'une trace enregistrée sous forme de fichier TRK </summary>
        /// <param name="FichierTRK"> fichier contenant la trace à importer </param>
        private bool ChargerTRK(string FichierTRK)
        {
            bool ChargerTRKRet;
            try
            {
                using (var FileBinaire = new BinaryReader(File.Open(FichierTRK, FileMode.Open, FileAccess.Read)))
                {
                    byte Flags = FileBinaire.ReadByte();
                    Nom = FileBinaire.ReadString();
                    var CouleurSegment = Color.FromArgb(FileBinaire.ReadInt32());
                    Site = (SitesCartographiques)FileBinaire.ReadByte();
                    BoundingBoxDD = new RectangleD(FileBinaire.ReadDouble(), FileBinaire.ReadDouble(), FileBinaire.ReadDouble(), FileBinaire.ReadDouble());
                    Echelle = Echelles._000;
                    int NbPoints = FileBinaire.ReadInt32();
                    IsAltitude = (Flags & 1) == 1;
                    IsHoraire = (Flags & 2) == 2;
                    _IsVisible = (Flags & 4) == 4;
                    IntialiserTrace(NbPoints + 50, CouleurSegment);
                    // on indique le nom du fichier pour pouvoir faire le remplacement lors du prochain enregistrment
                    for (int PointEncours = 0, loopTo = NbPoints - 1; PointEncours <= loopTo; PointEncours++)
                    {
                        ListeCoordonnees.Add(new PointD(FileBinaire.ReadDouble(), FileBinaire.ReadDouble()));
                        ListePoints.Add(Point.Empty); // on remplit la liste avec rien
                        if (IsAttribut)
                        {
                            var AttributEncours = new Attribut();
                            if (IsAltitude)
                                AttributEncours.Altitude = FileBinaire.ReadInt16();
                            if (IsHoraire)
                                AttributEncours.Horaire = DateTime.FromBinary(FileBinaire.ReadInt64());
                            ListeAttributs.Add(AttributEncours);
                        }
                    }
                }
                // met à jour les coordonnées virtuelles
                ChargerTRKRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "D1Z1");
                ChargerTRKRet = false;
            }

            return ChargerTRKRet;
        }
        /// <summary> enregistre la trace sous forme de fichier TRK </summary>
        /// <param name="NomFichierTRK"> nom du fichier </param>
        private void EnregistrerTRK(string NomFichierTRK)
        {
            if (FlagIsModifie)
            {
                try
                {
                    // la trace est définie et finie donc on peut calculer son emprise
                    CalculerBoundingBoxVirtuel();
                    BoundingBoxDD = RegionPixelsToRegionDD(BoundingBoxVirtuel, Site, Echelle);
                    using (var FileBinaire = new BinaryWriter(File.Open(NomFichierTRK, FileMode.Create, FileAccess.Write)))
                    {
                        FileBinaire.Write((byte)((IsAltitude ? 1 : 0) + (IsHoraire ? 2 : 0) + (_IsVisible ? 4 : 0)));
                        FileBinaire.Write(Nom);
                        FileBinaire.Write(PenSegment.Color.ToArgb());
                        FileBinaire.Write((byte)Site);
                        FileBinaire.Write(BoundingBoxDD.Pt0.X);
                        FileBinaire.Write(BoundingBoxDD.Pt0.Y);
                        FileBinaire.Write(BoundingBoxDD.Pt2.X);
                        FileBinaire.Write(BoundingBoxDD.Pt2.Y);
                        FileBinaire.Write(ListePoints.Count);
                        for (int PointEncours = 0, loopTo = ListePoints.Count - 1; PointEncours <= loopTo; PointEncours++)
                        {
                            FileBinaire.Write(ListeCoordonnees[PointEncours].X);
                            FileBinaire.Write(ListeCoordonnees[PointEncours].Y);
                            if (IsAltitude)
                                FileBinaire.Write(ListeAttributs[PointEncours].Altitude);
                            if (IsHoraire)
                                FileBinaire.Write(ListeAttributs[PointEncours].Horaire.ToBinary());
                        }
                    }
                }
                catch (Exception Ex)
                {
                    AfficherErreur(Ex, "Y2O4");
                }
            }
        }
        /// <summary> constructeur pour une nouvelle route à remplir avec des données virtuelles existantes. Sert uniquement lors de la coupe d'une trace </summary>
        /// <param name="PVirtuels"> liste des noeuds à ajouter </param>
        /// <param name="Altitude"> la nouvelle trace contiendra des altitudes </param>
        /// <param name="Horaire"> la nouvelle trace conteirndra des horaires </param>
        /// <param name="Attributs"> si unb attribut est déclaré il s'agit de la liste des attributs à ajouter </param>
        /// <param name="Nom"> nom de la nouvelle trace </param>
        private TRK(Point[] PVirtuels, PointD[] PWeb, string Nom, bool Altitude, bool Horaire, Attribut[] Attributs) : this(PVirtuels.Length, Nom, Altitude, Horaire)
        {
            ListePoints.AddRange(PVirtuels);
            ListeCoordonnees.AddRange(PWeb);
            if (IsAttribut)
                ListeAttributs.AddRange(Attributs);
            _IsVisible = true;
            PenSegment.Color = Color.Black;
            FlagIsModifie = true;
            // et l'enregistrer pour qu'elle apparaisse dans la collection
            EnregistrerTRK(Path.Combine(TracesSettings.CHEMIN_TRACE, this.Nom + ".trk"));
        }
        /// <summary> constructeur pour une nouvelle trace </summary>
        /// <param name="Nbpoints"> nb de noeuds maximum à prévoir </param>
        /// <param name="Altitude"> la nouvelle trace contiendra des altitudes </param>
        /// <param name="Horaire"> la nouvelle trace conteirndra des horaires </param>
        /// <param name="Nom"> nom de la nouvelle trace </param>
        private TRK(int NbPoints, string Nom, bool Altitude, bool Horaire)
        {
            Site = SiteCarto;
            Echelle = EchelleTrks;
            this.Nom = Nom;
            IsAltitude = Altitude;
            IsHoraire = Horaire;
            IntialiserTrace(NbPoints, c_CouleurSegment);
        }
        /// <summary> initialise les stockages de données de la trace </summary>
        /// <param name="Capacite"> Nb de points prévus au départ sans avoir de redimensionnement des tableaux </param>
        /// <param name="CouleurSegment"> Couleur de la trace </param>
        private void IntialiserTrace(int Capacite, Color CouleurSegment)
        {
            PointSurvol = -1;
            PenSegment = new Pen(CouleurSegment, _Sensibilite) { LineJoin = LineJoin.Round };
            IsAttribut = IsHoraire || IsAltitude;
            if (ListePoints is null)
            {
                ListePoints = new List<Point>(Capacite);
                ListeCoordonnees = new List<PointD>(Capacite);
                ListeAttributs = new List<Attribut>(Capacite);
            }
            else
            {
                ListePoints.Capacity += Capacite;
                ListeCoordonnees.Capacity += Capacite;
                if (IsAttribut)
                    ListeAttributs.Capacity += Capacite;
            }
        }
        /// <summary> seul construceur public qui permet de récupérer une trace existante en dehors de la collection </summary>
        /// <param name="FichierTRK"> fichier binaire qui correspond à la sérialisation de la trace </param>
        internal TRK(string FichierTRK, SitesCartographiques Site, DomsToms Domtom, Echelles Echelle)
        {
            SiteCarto = Site;
            DomTom = Domtom;
            IndiceSite = SiteDomTomToIndex(Site, Domtom);
            EchelleTrks = Echelle;
            // si le fichier a bien été charger on calcule les coordonnées virtuelles de latrace et son emprise
            if (ChargerTRK(FichierTRK))
                ChangerSiteEchelle();
        }
        #endregion
        #region Fonction ou méthode se rapportant au point
        /// <summary> renvoie les points par rapport au noeud survolé qui permettent le dessin d'un carré de 4*4 sensibilité avec un stylo de 4 sensibilité  d'épaisseur </summary>
        private static Point[] LimitesPoint(Point Pointencours)
        {
            return new Point[] { Point.Add(Pointencours, new Size(-_Sensibilite * 2, 0)), Point.Add(Pointencours, new Size(_Sensibilite * 2, 0)) };
        }
        /// <summary> renvoie les points par rapport au segment survolé qui permettent le dessin d'un carré de 3*3 sensibilité avec un stylo de 3 sensibilité d'épaisseur </summary>
        private static Point[] LimitesPointSegmentSurvol(Point Pointencours)
        {
            int Width = (int)Math.Round(_Sensibilite * 1.5d);
            return new Point[] { Point.Add(Pointencours, new Size(-Width, 0)), Point.Add(Pointencours, new Size(Width, 0)) };
        }
        /// <summary> cherche si un point de la trace est survolé par le curseur de souris </summary>
        private bool ChercherMouseSurvolPoint()
        {
            PointSurvol = -1;
            // on définit le rectangle de sensibilité définit par la position de la souris
            var MouseRectangle = new Rectangle(PosMouse.X - _Sensibilite, PosMouse.Y - _Sensibilite, _Sensibilite * 2 + 1, _Sensibilite * 2 + 1);
            // et pour chaque point on regarde si il est dans le rectangle de sensibilité en commençant par le dernier
            for (int PointEncours = ListePoints.Count - 1; PointEncours >= 0; PointEncours -= 1)
            {
                if (MouseRectangle.Contains(ListePoints[PointEncours]))
                {
                    PointSurvol = PointEncours;
                    break; // le premier trouver est gagnant
                }
            }
            return PointSurvol > -1;
        }
        /// <summary> cherche si un point de la trace est survolé par le curseur de souris. Même que précédament mais en version multi threads </summary>
#pragma warning disable IDE0051 // Supprimer les membres privés non utilisés
        private bool ChercherMouseSurvolPointParallel()
#pragma warning restore IDE0051 // Supprimer les membres privés non utilisés
        {
            PointSurvol = -1;
            var MouseRectangle = new Rectangle(PosMouse.X - _Sensibilite, PosMouse.Y - _Sensibilite, _Sensibilite * 2 + 1, _Sensibilite * 2 + 1);
            Parallel.For(0, ListePoints.Count, (PointEncours, State) =>
            {
                int Cpt = ListePoints.Count - PointEncours - 1;
                if (MouseRectangle.Contains(ListePoints[Cpt]))
                {
                    PointSurvol = Cpt;
                    State.Stop();
                }
            });
            return PointSurvol > -1;
        }
        #endregion
        #region Fonction ou méthode se rapportant au segment
        /// <summary> cette fonction permet de savoir si le segment est sélectionable lors d'un déplacement de la souris </summary>
        /// <param name="SegmentEncours"> segment à tester </param>
        /// <returns> true si le segment est sélectionable </returns>
        /// <remarks> pour l'instant ne renvoie pas le point d'intersection avec le segment </remarks>
        private bool SegmentEstSelectionable(int SegmentEncours)
        {
            var PtDeb = ListePoints[SegmentEncours - 1];
            var PtFin = ListePoints[SegmentEncours];
            // calcul de la Longueur du segment au carré et de la longueur
            int Bx_Ax = PtFin.X - PtDeb.X;
            int By_Ay = PtFin.Y - PtDeb.Y;
            int L_2 = Bx_Ax * Bx_Ax + By_Ay * By_Ay;
            // Dim L As Double = Math.Sqrt(L_2)
            int Cx_Ax = PosMouse.X - PtDeb.X;
            int Cy_Ay = PosMouse.Y - PtDeb.Y;
            int r_numer = Cx_Ax * Bx_Ax + Cy_Ay * By_Ay;
            // Le point C ne sera pas sur le segment si r_numer < 0 ou > L_2 donc le segment ne peut pas être sélectionné
            if (r_numer < 0 || r_numer > L_2)
                return false;
            // le point d'intersection est sur le segment AB on calcule la distance de C à AB
            double D = Math.Abs((-Cy_Ay * Bx_Ax + Cx_Ax * By_Ay) / Math.Sqrt(L_2));
            // si la distance est supérieure à la sensibilité le segment ne peut pas être sélectionné
            if (D > _Sensibilite)
                return false;
            // on calcule les coordonnées du point d'intersection de la projection orthogonnale de C sur le segment AB.
            //var Coef_R = (double)r_numer / L_2;
            //var PtIntersect = new Point();
            //PtIntersect.X = PtDeb.X + (int)Math.Round(Coef_R * Bx_Ax);
            //PtIntersect.Y = PtDeb.Y + (int)Math.Round(Coef_R * By_Ay);
            return true;
        }
        /// <summary> renvoie un rectangle qui contient le segment </summary>
        /// <param name="SegmentEncours"> index du segment </param>
        /// <remarks> aucune vérification sur le N° de segment </remarks>
        private Rectangle BoundingBoxSegment(int SegmentEncours)
        {
            Rectangle BoundingBoxSegmentRet = default;
            var PtDeb = ListePoints[SegmentEncours - 1];
            var PtFin = ListePoints[SegmentEncours];
            BoundingBoxSegmentRet.X = Math.Min(PtDeb.X, PtFin.X);
            BoundingBoxSegmentRet.Y = Math.Min(PtDeb.Y, PtFin.Y);
            int D = Math.Abs(PtDeb.X - PtFin.X);
            BoundingBoxSegmentRet.Width = D < _Sensibilite * 2 ? 4 : D;
            D = Math.Abs(PtDeb.Y - PtFin.Y);
            BoundingBoxSegmentRet.Height = D < _Sensibilite * 2 ? 4 : D;
            return BoundingBoxSegmentRet;
        }
        /// <summary> cherche si un segment est survolé par la souris </summary>
        private bool ChercherMouseSurvolSegment()
        {
            SegmentSurvol = 0;
            for (int SegmentEncours = 1, loopTo = ListePoints.Count - 1; SegmentEncours <= loopTo; SegmentEncours++)
            {
                // on cherche le boudingBox représenté par le segment
                var SegmentRectangle = BoundingBoxSegment(SegmentEncours);
                // si la souris est dans le boudingbox du rectangle on cherche si elle est à proximté du segment
                if (SegmentRectangle.Contains(PosMouse))
                {
                    if (SegmentEstSelectionable(SegmentEncours))
                    {
                        SegmentSurvol = SegmentEncours;
                        break; // c'est le 1er qui gagne
                    }
                }
            }
            return SegmentSurvol > 0;
        }
        /// <summary> cherche si un segment est survolé par la souris mais en version multi-threads </summary>
#pragma warning disable IDE0051 // Supprimer les membres privés non utilisés
        private bool ChercherMouseSurvolSegmentParallel()
#pragma warning restore IDE0051 // Supprimer les membres privés non utilisés
        {
            SegmentSurvol = 0;
            Parallel.For(1, ListePoints.Count, (SegmentEncours, State) =>
            {
                int Cpt = ListePoints.Count - SegmentEncours;
                // on cherche le boudingBox représenté par le segment
                var SegmentRectangle = BoundingBoxSegment(Cpt);
                // si la souris est dans le boudingbox du rectangle on cherche si elle est à proximté du segment
                if (SegmentEstSelectionable(Cpt))
                {
                    SegmentSurvol = Cpt;
                    State.Stop();
                }
            });
            return SegmentSurvol > 0;
        }
        #endregion
        #region Fonctions Stat.
        /// <summary> renvoi 6 champs correspondants aux informations de 2 points exprimés en coordonnées DD </summary>
        /// <param name="PtDeb"> 1er point ou point de début </param>
        /// <param name="PtFin"> 2ème point ou point de fin </param>
        /// <param name="NumSeg"> Numéro du segment (point de fin) dans la collection </param>
        /// <returns> voir CalculerStatSegmentsTrace </returns>
        private (int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse)
                CalculerSegment(PointD PtDeb, PointD PtFin, int NumSeg, ref bool Isok)
        {
            (int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse) CalculerSegmentRet = default;
            try
            {
                var (Longueur, Cap) = CalculerDistanceCap(PtDeb, PtFin);
                CalculerSegmentRet.Longueur = (int)Math.Round(Longueur);
                CalculerSegmentRet.Cap = Cap;
                if (CalculerSegmentRet.Cap < 0d)
                    CalculerSegmentRet.Cap += 360d;
                CalculerSegmentRet.Duree = 0;
                CalculerSegmentRet.Vitesse = 0d;
                CalculerSegmentRet.Denivele = 0;
                CalculerSegmentRet.Pente = 0d;
                if (IsHoraire)
                {
                    CalculerSegmentRet.Duree = (int)Math.Round((ListeAttributs[NumSeg].Horaire - ListeAttributs[NumSeg - 1].Horaire).TotalSeconds);
                    if (CalculerSegmentRet.Duree > 0)
                        CalculerSegmentRet.Vitesse = CalculerSegmentRet.Longueur / (double)CalculerSegmentRet.Duree;
                }
                if (IsAltitude)
                {
                    CalculerSegmentRet.Denivele = ListeAttributs[NumSeg].Altitude - ListeAttributs[NumSeg - 1].Altitude;
                    if (CalculerSegmentRet.Longueur > 0)
                        CalculerSegmentRet.Pente = CalculerSegmentRet.Denivele / (double)CalculerSegmentRet.Longueur;
                }
                Isok = true;
            }
            catch
            {
                Isok = false;
            }

            return CalculerSegmentRet;
        }
        /// <summary> renvoie les informations correspondant aux segments demandés </summary>
        /// <param name="NumSegmentDeb"> Numéro du 1er segment dans la collection </param>
        /// <param name="NumSegmentFin"> Numéro du dernier segment dans la collection </param>
        /// <returns> 
        /// Vitesse en m/s 
        /// Pente en % de la longueur 
        /// Cap en degré toujours positif de 0 à 359.99
        /// Durée en seconde
        /// Denivele en metres
        /// Longeur en mètres
        /// </returns>
        internal List<(int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse)>
                 CalculerStatSegmentsTrace(int NumSegmentDeb, int NumSegmentFin)
        {
            var Ret = new List<(int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse)>(NumSegmentFin - NumSegmentDeb + 1);
            var Datum = DatumSiteWeb((int)Site);
            try
            {
                var PtDeb = PointGrilleToPointDD(ListeCoordonnees[NumSegmentDeb - 1], Datum);
                PointD PtFin;
                var IsOk = default(bool);
                for (int NumSeg = NumSegmentDeb, loopTo = NumSegmentFin; NumSeg <= loopTo; NumSeg++)
                {
                    PtFin = PointGrilleToPointDD(ListeCoordonnees[NumSeg], Datum);
                    var Segment = this.CalculerSegment(PtDeb, PtFin, NumSeg, ref IsOk);
                    if (IsOk)
                    {
                        Ret.Add(Segment);
                    }
                    else
                    {
                        Ret = null;
                        break;
                    }
                    PtDeb = PtFin;
                }
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "F7M8");
                Ret = null;
            }
            return Ret;
        }
        /// <summary> renvoie les informations correspondant au segment demandé </summary>
        /// <param name="NumSegment"> Numéro du segment dans la collection </param>
        /// <returns> 
        /// Vitesse en m/s 
        /// Pente en % de la longueur 
        /// Cap en degré toujours positif de 0 à 359.99
        /// Durée en seconde
        /// Denivele en metres
        /// Longeur en mètres
        /// </returns>
        internal (int Longueur, int Denivele, int Duree, double Cap, double Pente, double Vitesse)
                 StatsSegment(int NumSegment)
        {
            var Datum = DatumSiteWeb((int)Site);
            var PtDeb = PointGrilleToPointDD(ListeCoordonnees[NumSegment - 1], Datum);
            var PtFin = PointGrilleToPointDD(ListeCoordonnees[NumSegment], Datum);
            var IsOk = default(bool);
            var Stat = this.CalculerSegment(PtDeb, PtFin, NumSegment, ref IsOk);
            if (IsOk)
            {
                return Stat;
            }
            else
            {
                return default;
            }
        }
        /// <summary> calcule l'emprise de la trace sous forme d'un rectangle exprimé en coordonnées Virtuelles </summary>
        private void CalculerBoundingBoxVirtuel()
        {
            if (ListePoints.Count == 0)
            {
                BoundingBoxVirtuel = Rectangle.Empty;
            }
            else
            {
                int MinLon = ListePoints[0].X;
                int MaxLon = ListePoints[0].X;
                int MinLat = ListePoints[0].Y;
                int MaxLat = ListePoints[0].Y;
                for (int Cpt = 1, loopTo = ListePoints.Count - 1; Cpt <= loopTo; Cpt++)
                {
                    if (ListePoints[Cpt].X < MinLon)
                        MinLon = ListePoints[Cpt].X;
                    if (ListePoints[Cpt].X > MaxLon)
                        MaxLon = ListePoints[Cpt].X;
                    if (ListePoints[Cpt].Y < MinLat)
                        MinLat = ListePoints[Cpt].Y;
                    if (ListePoints[Cpt].Y > MaxLat)
                        MaxLat = ListePoints[Cpt].Y;
                }
                BoundingBoxVirtuel = new Rectangle(new Point(MinLon, MinLat), new Size(MaxLon - MinLon, MaxLat - MinLat));
            }
        }
        #endregion
        #region Propriétés se rapportant à la trace
        /// <summary> nd de points composant la trace </summary>
        internal int NbPoints
        {
            get
            {
                return ListePoints.Count;
            }
        }
        /// <summary> Nb de segments de la trace. 1 segment est définit par 2 points </summary>
        internal int NbSegments
        {
            get
            {
                if (ListePoints.Count > 1)
                {
                    return ListePoints.Count - 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary> rend la trace visible ou invisible sur l'affichage. Pour cela elle adapte les coordonnées grille en coordonnées virtuelles </summary>
        internal bool IsVisible
        {
            set
            {
                _IsVisible = value;
                if (_IsVisible)
                {
                    // si la trace devient visible il faut être sur que les coord correspondent au système encours
                    ChangerSiteEchelle();
                }
                else
                {
                    // c'est une précaution mais normalement on ne peut pas rendre invisible la trace encours
                    _ModeEdition = false;
                }
            }
        }
        /// <summary> indique si les points de la trace contienne une altitude </summary>
        internal bool IsAltitude { get; private set; }
        /// <summary> indique si les points de la trace contienne une heure de création </summary>
        internal bool IsHoraire { get; private set; }
        /// <summary> boundingbox en latlon wgs84 </summary>
        internal RectangleD BoundingBoxDD { get; private set; }
        /// <summary> pour une nouvelle trace ou une trace qui vient d'être chargée c'est rectangle.Empty </summary>
        internal Rectangle BoundingBoxVirtuel { get; private set; }
        /// <summary> Nom donné à la trace </summary>
        internal string Nom { get; set; }
        /// <summary> couleur de la trace </summary>
        internal Color CouleurSegment
        {
            get
            {
                return PenSegment.Color;
            }
            set
            {
                if (PenSegment.Color != value)
                {
                    PenSegment.Color = value;
                    FlagIsModifie = true;
                }
            }
        }
        /// <summary> flag indiquant si on peut modifier la trace en ajoutant, déplaçant et en supprimant des points à l'aide de la souris </summary>
        internal bool ModeEdition
        {
            set
            {
                _ModeEdition = value;
            }
        }
        #endregion
        #region propriété se rapportant au Points
        /// <summary> renvoie les coordonnées virtuelles d'un point de la trace </summary>
        /// <param name="NumPoint"> indice du noeud </param>
        internal Point CoordonneesVirtuelles(int NumPoint)
        {
            if (NumPoint >= 0 && NumPoint < ListePoints.Count)
            {
                return ListePoints[NumPoint];
            }
            else
            {
                throw new Exception("Indice Hors Limites");
            }
        }
        /// <summary> renvoie les coordonnées systèmecartographique d'un point de la trace </summary>
        /// <param name="NumPoint"> indice du noeud </param>
        internal PointD CoordonneesGrille(short NumPoint)
        {
            if (NumPoint >= 0 && NumPoint < ListePoints.Count)
            {
                return PointPixelToPointGrille(ListePoints[NumPoint], Site, Echelle);
            }
            else
            {
                throw new Exception("Indice Hors Limites");
            }
        }
        /// <summary> renvoie les coordonnées longitude / latitude d'un point de la trace </summary>
        /// <param name="NumPoint"> indice du noeud </param>
        internal PointD CoordonneesDD(int NumPoint)
        {
            if (NumPoint >= 0 && NumPoint < ListePoints.Count)
            {
                var PointGrille = PointPixelToPointGrille(ListePoints[NumPoint], Site, Echelle);
                return PointGrilleToPointDD(PointGrille, DatumSiteWeb((int)Site));
            }
            else
            {
                throw new Exception("Indice Hors Limites");
            }
        }
        /// <summary> renvoie l'altitude d'un point de la trace </summary>
        /// <param name="NumPoint"> indice du noeud </param>
        internal short Altitude(int NumPoint)
        {
            if (NumPoint >= 0 && NumPoint < ListePoints.Count)
            {
                if (IsAltitude)
                {
                    return ListeAttributs[NumPoint].Altitude;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new Exception("Indice Hors Limites");
            }
        }
        /// <summary> renvoie la date d'un point de la trace </summary>
        /// <param name="NumPoint"> indice du noeud </param>
        internal DateTime Horaire(int NumPoint)
        {
            if (NumPoint >= 0 && NumPoint < ListePoints.Count)
            {
                if (IsHoraire)
                {
                    return ListeAttributs[NumPoint].Horaire;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            else
            {
                throw new Exception("Indice Hors Limites");
            }
        }
        internal Point[] ListePointsPixel
        {
            get
            {
                if (ListePoints is not null)
                {
                    return ListePoints.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion
        #region Interaction avec l'utilisateur
        /// <summary> Transforme les coordonnées virtuelles des points existants pour en coordonnées virtuelles pour le nouveau site et ou la nouvelle echelle </summary>
        private void ChangerSiteEchelle()
        {
            // on fait le changement que si le site est différent mais on change l'échelle avec
            if (Site != SiteCarto)
            {
                var DatumAncienSite = DatumSiteWeb((int)Site);
                var DatumNouveauSite = DatumSiteWeb((int)SiteCarto);
                // puis à répéter autant de fois qu'il y a de noeuds restants
                for (int PointEncours = 0, loopTo = ListeCoordonnees.Count - 1; PointEncours <= loopTo; PointEncours++)
                {
                    // on converti les coordonnéesGrille avec le site et l'echelle en cours en coordonnées lon/lat wgs84
                    var PointWebNew = ConvertProjectionToWGS84(DatumAncienSite, new PointProjection(ListeCoordonnees[PointEncours]));
                    // on converti les coordonnées lon/lat wgs84 en coordonnées grille du nouveau site 
                    PointWebNew = ConvertWGS84ToProjection(PointWebNew, DatumNouveauSite).Coordonnees;
                    // on converti les coordonnées grille du nouveau site en coordonnées virtuelle pour le nouveau site et la nouvelle echelle
                    ListeCoordonnees[PointEncours] = PointWebNew;
                    ListePoints[PointEncours] = PointGrilleToPointPixel(PointWebNew, SiteCarto, EchelleTrks);
                }
                Site = SiteCarto;   // on met à jour le site
                Echelle = EchelleTrks; // et l'échelle
                CalculerBoundingBoxVirtuel();
            }
            else if (Echelle != EchelleTrks) // le site est identique seule l'echelle change
            {
                for (int PointEncours = 0, loopTo1 = ListeCoordonnees.Count - 1; PointEncours <= loopTo1; PointEncours++)
                    // on converti les coordonnées grille  en coordonnées virtuelle pour la nouvelle echelle
                    ListePoints[PointEncours] = PointGrilleToPointPixel(ListeCoordonnees[PointEncours], Site, EchelleTrks);
                Echelle = EchelleTrks;
                CalculerBoundingBoxVirtuel();
            }
        }
        /// <summary> dessine la trace à la demande de l'application si visible. Le test de visibilité doit être fait par la procédure appelante </summary>
        /// <param name="Graph"> surface graphique représentée par la surface d'affichage </param>
        private void Dessine(Graphics Graph)
        {
            // si il y a au moins un segment contenu par la trace on le(s) représente(nt) sur la surface d'affichage
            if (ListePoints.Count - 1 > 0)
            {
                var T = ListePoints.ToArray();
                Graph.DrawLines(PenSegment, T);
                DessinerSensLigne(Graph);
            }
            else if (ListePoints.Count > 0)
            {
                // on représente le premier point
                Graph.DrawLines(PenPointAjout, LimitesPoint(ListePoints[0]));
            }
            // si la trace est modifiable on passe au dessin du survol et des actions
            if (_ModeEdition)
                DessinerTraceModification(Graph);
        }
        /// <summary> dessine la trace à sa demande </summary>
        /// <param name="Graph"> surface graphique représentée par la surface d'affichage </param>
        private void DessinerTraceModification(Graphics Graph)
        {
            if (PointSurvol > -1)
            {
                DessinerTracePointSurvol(Graph);
            }
            else if (SegmentSurvol > 0)
            {
                DessinerTraceSegmentSurvol(Graph);
                if (Edition == Editions.AjouterDeplacer)
                {
                    Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse));
                }
            }
            else if (Edition == Editions.AjouterDeplacer && ListePoints.Count > 0)
            {
                // dessine le segment qui peut être ajouté en fin de trace
                Graph.DrawLine(PenSegmentAjout, ListePoints[^1], PosMouse);
            }
        }
        /// <summary> dessinne le point qui est survolé si pas d'action ou le ou les segements ajoutés et ou supprimés si action </summary>
        /// <param name="Graph"> surface de dessin de l'affichage </param>
        private void DessinerTracePointSurvol(Graphics Graph)
        {
            switch (Action)
            {
                case Actions.DeplacerPoint:
                    {
                        if (IsActionEncours)
                        {
                            if (PointSurvol > 0)
                            {
                                // dessin du segment existant donc supprimé avant le point à déplacer  si celui-ci existe
                                Graph.DrawLine(PenSegmentSuppression, ListePoints[PointSurvol - 1], ListePoints[PointSurvol]);
                                // dessin du segment déplacé donc ajouté situé avant le point à déplacer  si celui-ci existe
                                Graph.DrawLine(PenSegmentAjout, ListePoints[PointSurvol - 1], PosMouse);
                            }
                            if (PointSurvol < ListePoints.Count - 1)
                            {
                                // dessin du segment existant donc supprimé après le point à déplacer  si celui-ci existe
                                Graph.DrawLine(PenSegmentSuppression, ListePoints[PointSurvol], ListePoints[PointSurvol + 1]);
                                // dessin du segment situé après le point à déplacer si celui-ci existe
                                Graph.DrawLine(PenSegmentAjout, PosMouse, ListePoints[PointSurvol + 1]);
                            }
                            // dessin du point existant donc supprimé
                            Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints[PointSurvol]));
                            // dessin du point déplacer
                            Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse));
                        }
                        else
                        {
                            Graph.DrawLines(PenPointSurvol, LimitesPoint(ListePoints[PointSurvol]));
                        }

                        break;
                    }
                case Actions.SupprimerPoint:
                    {
                        if (!IsActionEncours)
                        {
                            // dessin d'un segment à supprimer si il exite. Cas particulier du segment de début de trace, il faut un segment après le point survolé
                            Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints[PointSurvol]));
                            if (PointSurvol >= 0 && PointSurvol < ListePoints.Count - 1)  // point de fin de trace exclu
                            {
                                Graph.DrawLine(PenSegmentSuppression, ListePoints[PointSurvol], ListePoints[PointSurvol + 1]);
                            }
                            // dessin d'un 2ème segment à supprimer si il exite. Cas particulier du segment de fin de trace, il faut un segment avant le point survolé
                            if (PointSurvol > 0 && PointSurvol <= ListePoints.Count - 1) // point de début de trace exclu
                            {
                                Graph.DrawLine(PenSegmentSuppression, ListePoints[PointSurvol - 1], ListePoints[PointSurvol]);
                            }
                            // dessin du segment à ajouter si il existe. Il faut un segment de part et d'autre du point survolé
                            if (PointSurvol > 0 && PointSurvol < ListePoints.Count - 1)
                            {
                                Graph.DrawLine(PenSegmentAjout, ListePoints[PointSurvol - 1], ListePoints[PointSurvol + 1]);
                            }
                        }

                        break;
                    }
                case Actions.CouperPoint:
                    {
                        if (!IsActionEncours)
                        {
                            int NbPointsCouper = ListePoints.Count - PointSurvol;
                            if (NbPointsCouper > 1)
                            {
                                var T = new Point[NbPointsCouper];
                                ListePoints.CopyTo(PointSurvol, T, 0, NbPointsCouper);
                                Graph.DrawLines(PenSegmentSuppression, T);
                            }
                            // dessin du  point de coupe
                            Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse));
                        }

                        break;
                    }

                default:
                    {
                        // si un point est survolé, il y a une action à réaliser
                        Graph.DrawLines(PenPointSurvol, LimitesPoint(ListePoints[PointSurvol]));
                        break;
                    }
            }
        }
        /// <summary> dessinne le segment qui est survolé si pas d'action ou le ou les segements ajoutés et ou supprimés si action </summary>
        /// <param name="Graph"> surface de dessin de l'affichage </param>
        private void DessinerTraceSegmentSurvol(Graphics Graph)
        {
            switch (Action)
            {
                case Actions.SuprimerSegment:
                    {
                        if (!IsActionEncours)
                        {
                            // dessin du segment et du point à supprimer
                            Graph.DrawLine(PenSegmentSuppression, ListePoints[SegmentSurvol - 1], ListePoints[SegmentSurvol]);
                            if (SegmentSurvol == 1)
                            {
                                // si c'est le premier segment de la trace le point à supprimer est le point de début du segment
                                Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints[SegmentSurvol - 1]));
                            }
                            else
                            {
                                // sinon c'est le point de fin du segment
                                Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints[SegmentSurvol]));
                            }
                            if (SegmentSurvol > 1 && SegmentSurvol < ListePoints.Count - 1)
                            {
                                // eventuellement dessin du nouveau segment après suppression du segment survolé (point de fin du segment)
                                Graph.DrawLine(PenSegmentSuppression, ListePoints[SegmentSurvol], ListePoints[SegmentSurvol + 1]);
                                Graph.DrawLine(PenSegmentAjout, ListePoints[SegmentSurvol - 1], ListePoints[SegmentSurvol + 1]);
                            }
                        }

                        break;
                    }
                case Actions.CouperSegment:
                    {
                        if (!IsActionEncours)
                        {
                            int NbPointsCouper = ListePoints.Count - SegmentSurvol;
                            var T = new Point[NbPointsCouper + 1];
                            T[0] = PosMouse;
                            ListePoints.CopyTo(SegmentSurvol, T, 1, NbPointsCouper);
                            // dessin des segments à couper
                            Graph.DrawLines(PenSegmentSuppression, T);
                            // dessin du  point de coupe
                            Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse));
                        }

                        break;
                    }

                default:
                    {
                        // on dessine le segment survolé
                        Graph.DrawLine(PenSegmentSurvol, ListePoints[SegmentSurvol - 1], ListePoints[SegmentSurvol]);
                        Graph.DrawLines(PenSegmentPointSurvol, LimitesPointSegmentSurvol(ListePoints[SegmentSurvol - 1]));
                        Graph.DrawLines(PenSegmentPointSurvol, LimitesPointSegmentSurvol(ListePoints[SegmentSurvol]));
                        break;
                    }
            }
        }
        /// <summary> dessinne les flêches qui indique le sens des segments </summary>
        /// <param name="G"> surface de dessin de l'affichage </param>
        private void DessinerSensLigne(Graphics G)
        {
            var PointsFleche = new PointF[] { default, default };
            for (int Cpt = 1, loopTo = ListePoints.Count - 1; Cpt <= loopTo; Cpt++)
            {
                if (MilieuSegment(Cpt, PointsFleche))
                    G.DrawLines(PenTriangle, PointsFleche);
            }
        }
        /// <summary> renvoie true si le segment à une longueur = ou supérieur à 10 * sensibilité et renvoie le point milieu du segment</summary>
        /// <param name="NumSegment"> numéro du segment de 1 à listepoints.count-1</param>
        /// <param name="PointsFleche"> Point qui contient le milieu du segment</param>
        private bool MilieuSegment(int NumSegment, PointF[] PointsFleche)
        {
            int DeltaX = ListePoints[NumSegment].X - ListePoints[NumSegment - 1].X;
            int DeltaY = ListePoints[NumSegment].Y - ListePoints[NumSegment - 1].Y;
            // si la longueur du segment est trop petite on ne dessine pas le flèche
            double L = Math.Sqrt(Math.Pow(DeltaX, 2d) + Math.Pow(DeltaY, 2d));
            if (L < 10 * _Sensibilite)
                return false;
            // calcul des coordonnées du point milieu au segment
            var M = new PointD((ListePoints[NumSegment].X + ListePoints[NumSegment - 1].X) / 2d, (ListePoints[NumSegment].Y + ListePoints[NumSegment - 1].Y) / 2d);
            // et calcul des points de début et de fin de la flêche par trigo
            double Sin = DeltaY / L;
            double Cos = DeltaX / L;
            PointsFleche[0] = PointD.Offset(M, -_Sensibilite * Cos, -_Sensibilite * Sin).ToPointF;
            PointsFleche[1] = PointD.Offset(M, _Sensibilite * Cos, _Sensibilite * Sin).ToPointF;
            return true;
        }
        /// <summary> indique si la trace est concernée par le déplacement de la souris. On donne priorité au point </summary>
        internal bool SourisBouge(Point PointMouse)
        {
            PosMouse = PointMouse;
            // si une action est encours il n'y a plus besoin de trouver un point ou un segment survolé
            if (!IsActionEncours) // c'est le point survolé qui est prioritaire
            {
                if (!ChercherMouseSurvolPoint())
                {
                    // si pas de point survolé est ce qu'il y a un segment survolé
                    ChercherMouseSurvolSegment();
                }
                // si la trace est mode mode édition on cherche l'action que l'on peut réaliser
                switch (Edition)
                {
                    case Editions.AjouterDeplacer:
                        {
                            if (PointSurvol > -1)  // action à réaliser sur l'événementSourisUP
                            {
                                Action = Actions.DeplacerPoint;
                                CurseurEncours = Curseurs.TraceDeplacePt;
                            }
                            else if (SegmentSurvol > 0)  // action à réaliser sur l'événementSourisDOWN
                            {
                                Action = Actions.AjouterPointSegment;
                                CurseurEncours = Curseurs.TraceInsertPt;
                            }
                            else  // action à réaliser sur l'événementSourisDOWN
                            {
                                Action = Actions.AjouterPointFin;
                                CurseurEncours = Curseurs.TraceInsertPt;
                            }

                            break;
                        }
                    case Editions.Supprimer:
                        {
                            CurseurEncours = Curseurs.TraceSupprime;
                            // on se contente de définir l'action à réaliser sur l'événementSourisUP
                            if (PointSurvol > -1)
                            {
                                Action = Actions.SupprimerPoint;
                                CurseurEncours = Curseurs.TraceSupprimePtSeg;
                            }
                            else if (SegmentSurvol > 0)
                            {
                                Action = Actions.SuprimerSegment;
                                CurseurEncours = Curseurs.TraceSupprimePtSeg;
                            }

                            break;
                        }
                    case Editions.Couper:
                        {
                            CurseurEncours = Curseurs.TraceCoupe;
                            // on se contente de définir l'action à réaliser sur l'événementSourisUP
                            if (PointSurvol > -1)
                            {
                                Action = Actions.CouperPoint;
                                CurseurEncours = Curseurs.TraceCoupePtSeg;
                            }
                            else if (SegmentSurvol > 0)
                            {
                                Action = Actions.CouperSegment;
                                CurseurEncours = Curseurs.TraceCoupePtSeg;
                            }

                            break;
                        }
                }
            }
            return true; // pour dessiner le point ou le segment survolé
        }
        /// <summary> Lance l'action qui peut être réalisée sur un relachement du bouton gauche de la souris </summary>
        internal bool SourisUp(Point PointMouse, Attribut Attrib)
        {
            // c'est la fin du déplacement qui compte pour cette édition
            PosMouse = PointMouse;
            Attribut = Attrib;
            // si une action est encours il faut aller voir si il y en a une qui prend à son compte l'évenement
            if (IsActionEncours)
            {
                RealiserActionsSourisUP();
                FlagIsModifie = true; // on indique que la trace a été modifié
                IsActionEncours = false; // on indique qu'il n'y a plus d'action encours
                Action = Actions.Aucune;
                SegmentSurvol = 0;
                PointSurvol = -1;
                return true; // pour dessiner l'action si besoin
            }
            return false;
        }
        /// <summary>réalise l'action sur la trace quand le bouton gauche de la souris est relevé : Evenement MouseUP </summary>
        private void RealiserActionsSourisUP()
        {
            switch (Action)
            {
                case Actions.DeplacerPoint:
                    {
                        ListePoints[PointSurvol] = PosMouse;
                        ListeCoordonnees[PointSurvol] = PointPixelToPointGrille(PosMouse, Site, Echelle);
                        if (IsAttribut)
                        {
                            Attribut.Horaire = ListeAttributs[PointSurvol].Horaire;
                            ListeAttributs[PointSurvol] = Attribut; // on met à jour l'altitude du point
                        }
                        CurseurEncours = Curseurs.TraceEdite;
                        break;
                    }
                case Actions.SupprimerPoint:
                    {
                        ListePoints.RemoveAt(PointSurvol);
                        ListeCoordonnees.RemoveAt(PointSurvol);
                        if (IsAttribut)
                            ListeAttributs.RemoveAt(PointSurvol);
                        CurseurEncours = Curseurs.TraceSupprime;
                        break;
                    }
                case Actions.SuprimerSegment:
                    {
                        ListePoints.RemoveAt(SegmentSurvol);
                        ListeCoordonnees.RemoveAt(SegmentSurvol);
                        if (IsAttribut)
                            ListeAttributs.RemoveAt(SegmentSurvol);
                        CurseurEncours = Curseurs.TraceSupprime;
                        break;
                    }
                case Actions.CouperPoint:
                    {
                        CouperTraceAuPoint();
                        CurseurEncours = Curseurs.TraceCoupe;
                        FlagIsModifie = true;
                        break;
                    }
                case Actions.CouperSegment:
                    {
                        CouperTraceAuSegment();
                        CurseurEncours = Curseurs.TraceCoupe;
                        FlagIsModifie = true;
                        break;
                    }
            }
        }
        /// <summary> coupe la trace principale au niveau du point et enregistre la partie coupée (du point de coupe à la fin) en tant que trace TRK et l'ajoute dans la collection </summary>
        private void CouperTraceAuPoint()
        {
            int NbPointsNouvelleTrace = ListePoints.Count - PointSurvol - 1;
            if (NbPointsNouvelleTrace > 0) // Il faut au moins un segment pour créer une nouvelle trace
            {
                var PVirtuels = new Point[NbPointsNouvelleTrace + 1];
                ListePoints.CopyTo(PointSurvol, PVirtuels, 0, NbPointsNouvelleTrace + 1);
                ListePoints.RemoveRange(PointSurvol + 1, NbPointsNouvelleTrace);
                var PWeb = new PointD[NbPointsNouvelleTrace + 1];
                ListeCoordonnees.CopyTo(PointSurvol, PWeb, 0, NbPointsNouvelleTrace + 1);
                ListeCoordonnees.RemoveRange(PointSurvol + 1, NbPointsNouvelleTrace);
                Attribut[] PAttribut = null;
                if (IsAttribut)
                {
                    PAttribut = new Attribut[NbPointsNouvelleTrace + 1];
                    ListeAttributs.CopyTo(PointSurvol, PAttribut, 0, NbPointsNouvelleTrace + 1);
                    ListeAttributs.RemoveRange(PointSurvol + 1, NbPointsNouvelleTrace);
                }
                ListeTraces.Add(new TRK(PVirtuels, PWeb, RenvoyerNomTraceUnique(Nom), IsAltitude, IsHoraire, PAttribut));
            }
        }
        /// <summary> coupe la trace principale sur le segment et enregistre la partie coupée (du point de coupe à la fin) en tant que trace TRK et l'ajoute dans la collection </summary>
        private void CouperTraceAuSegment()
        {
            int NbPointsNouvelleTrace = ListePoints.Count - SegmentSurvol;
            var PVirtuels = new Point[NbPointsNouvelleTrace + 1];
            PVirtuels[0] = PosMouse; // met à jour le point de coupe en coordonnées virtuelles pour la nouvelle trace
            ListePoints.CopyTo(SegmentSurvol, PVirtuels, 1, NbPointsNouvelleTrace);
            ListePoints.RemoveRange(SegmentSurvol, NbPointsNouvelleTrace);
            ListePoints.Add(PVirtuels[0]); // insert le point de coupe pour la trace existante
            var PWeb = new PointD[NbPointsNouvelleTrace + 1];
            PWeb[0] = PointPixelToPointGrille(PosMouse, Site, Echelle); // met à jour le point de coupe en coordonnées Web pour la nouvelle trace
            ListeCoordonnees.CopyTo(SegmentSurvol, PWeb, 1, NbPointsNouvelleTrace);
            ListeCoordonnees.RemoveRange(SegmentSurvol, NbPointsNouvelleTrace);
            ListeCoordonnees.Add(PWeb[0]); // insert le point de coupe pour la trace existante en coordonnées Web pour la nouvelle trace
            Attribut[] PAttribut = null;
            if (IsAttribut)
            {
                PAttribut = new Attribut[NbPointsNouvelleTrace + 1];
                if (IsAltitude)
                {
                    if (Attribut.Altitude == -9999) // si l'altitude FCGP n'est pas correcte lors de l'ajout on la recalcule
                    {
                        PAttribut[0].Altitude = (short)Math.Round((ListeAttributs[SegmentSurvol - 1].Altitude + ListeAttributs[SegmentSurvol].Altitude) / 2d);
                    }
                    else
                    {
                        PAttribut[0].Altitude = Attribut.Altitude;
                    }
                }
                if (IsHoraire) // on recalcule toujours l'horaire car FCGP ne le donne jamais
                {
                    int NbSecondes = (int)Math.Round((ListeAttributs[SegmentSurvol].Horaire - ListeAttributs[SegmentSurvol - 1].Horaire).TotalSeconds / 2d);
                    PAttribut[0].Horaire = ListeAttributs[SegmentSurvol - 1].Horaire + new TimeSpan(0, 0, NbSecondes);
                }
                ListeAttributs.CopyTo(SegmentSurvol, PAttribut, 1, NbPointsNouvelleTrace);
                ListeAttributs.RemoveRange(SegmentSurvol, NbPointsNouvelleTrace);
                ListeAttributs.Add(PAttribut[0]);
            }
            ListeTraces.Add(new TRK(PVirtuels, PWeb, RenvoyerNomTraceUnique(Nom), IsAltitude, IsHoraire, PAttribut));
        }
        /// <summary> Lance l'action qui peut être réalisée sur un appui du bouton gauche de la souris </summary>
        /// <param name="PointMouse"> position de la souris lors de l'appui sur le bouton </param>
        /// <param name="Attrib"> attribut associé au point </param>
        internal bool SourisDown(Point PointMouse, Attribut Attrib)
        {
            PosMouse = PointMouse;
            Attribut = Attrib;
            RealiserActionSourisDown();
            return true;  // si il y a une action encours on demande le dessin
        }
        /// <summary>réalise l'action sur la trace quand le bouton gauche de la souris est appuyé : Evenement MouseDOWN </summary>
        private void RealiserActionSourisDown()
        {
            IsActionEncours = true; // une action aura forcément lieu
            FlagIsModifie = true; // on indique que la trace est modifié par rapport au chargement
            if (Action == Actions.AjouterPointSegment)
            {
                ListePoints.Insert(SegmentSurvol, PosMouse);
                ListeCoordonnees.Insert(SegmentSurvol, PointPixelToPointGrille(PosMouse, Site, Echelle));
                // fait en sorte que les attributs soient corrects
                if (IsAttribut)
                {
                    if (IsAltitude && Attribut.Altitude == -9999) // si l'altitude renvoyée par FCGP n'est pas bonne on fait la moyenne des 2 points englobant le pt inséré
                    {
                        Attribut.Altitude = (short)Math.Round((ListeAttributs[SegmentSurvol - 1].Altitude + ListeAttributs[SegmentSurvol].Altitude) / 2d);
                    }
                    if (IsHoraire) // on reclacule l'horaire car FCGP ne renvoie jamais de date
                    {
                        int NbSecondes = (int)Math.Round((ListeAttributs[SegmentSurvol].Horaire - ListeAttributs[SegmentSurvol - 1].Horaire).TotalSeconds / 2d);
                        Attribut.Horaire = ListeAttributs[SegmentSurvol - 1].Horaire + new TimeSpan(0, 0, NbSecondes);
                    }
                    ListeAttributs.Insert(SegmentSurvol, Attribut);
                }
                // on transforme l'action initiale en Deplacement du pt afin de pouvoir le déplacer tant qu'on ne relève pas le bouton gauche de la souris
                PointSurvol = SegmentSurvol;
                Action = Actions.DeplacerPoint;
                CurseurEncours = Curseurs.TraceDeplacePt;
            }
            else if (Action == Actions.AjouterPointFin)
            {
                if (IsAttribut)
                {
                    if (IsAltitude && Attribut.Altitude == -9999)
                        Attribut.Altitude = ListeAttributs[^1].Altitude;
                    if (IsHoraire)
                        Attribut.Horaire = ListeAttributs[^1].Horaire;
                    ListeAttributs.Add(Attribut);
                }
                ListePoints.Add(PosMouse);
                ListeCoordonnees.Add(PointPixelToPointGrille(PosMouse, Site, Echelle));
                // on transforme l'action initiale en Deplacement du pt afin de pouvoir le déplacer tant qu'on ne relève pas le bouton gauche de la souris
                PointSurvol = ListePoints.Count - 1;
                Action = Actions.DeplacerPoint;
                CurseurEncours = Curseurs.TraceDeplacePt;
            }
        }
        /// <summary> trouve le mode édition à réaliser quand on appui sur une des 3 touches qui permettent de choisir un mode d'édition </summary>
        internal bool ToucheDown(Keys Touche)
        {
            if (Edition == Editions.Aucune)
            {
                switch (Touche)
                {
                    case ToucheAjouteDeplace:
                        {
                            Edition = Editions.AjouterDeplacer;
                            if (PointSurvol > -1)  // action à réaliser sur l'événementSourisUP
                            {
                                Action = Actions.DeplacerPoint;
                                CurseurEncours = Curseurs.TraceDeplacePt;
                            }
                            else if (SegmentSurvol > 0)  // action à réaliser sur l'événementSourisDOWN
                            {
                                Action = Actions.AjouterPointSegment;
                                CurseurEncours = Curseurs.TraceInsertPt;
                            }
                            else  // action à réaliser sur l'événementSourisDOWN
                            {
                                Action = Actions.AjouterPointFin;
                                CurseurEncours = Curseurs.TraceInsertPt;
                            }

                            break;
                        }
                    case ToucheSupprime:
                        {
                            Edition = Editions.Supprimer;
                            CurseurEncours = Curseurs.TraceSupprime;
                            // on se contente de définir l'action à réaliser sur l'événementSourisUP
                            if (PointSurvol > -1)
                            {
                                Action = Actions.SupprimerPoint;
                                CurseurEncours = Curseurs.TraceSupprimePtSeg;
                            }
                            else if (SegmentSurvol > 0)
                            {
                                Action = Actions.SuprimerSegment;
                                CurseurEncours = Curseurs.TraceSupprimePtSeg;
                            }

                            break;
                        }
                    case ToucheCoupe:
                        {
                            Edition = Editions.Couper;
                            CurseurEncours = Curseurs.TraceCoupe;
                            // on se contente de définir l'action à réaliser sur l'événementSourisUP
                            if (PointSurvol > -1)
                            {
                                Action = Actions.CouperPoint;
                                CurseurEncours = Curseurs.TraceCoupePtSeg;
                            }
                            else if (SegmentSurvol > 0)
                            {
                                Action = Actions.CouperSegment;
                                CurseurEncours = Curseurs.TraceCoupePtSeg;
                            }

                            break;
                        }
                }
                // on renvoie true si une édition possible
                return Edition > Editions.Aucune;
            }
            return false;
        }
        /// <summary> Si touche correspond à une des 3 touches qui permettent de choisir un mode d'édition on enlève le mode d'édition </summary>
        internal bool ToucheUp(Keys Touche)
        {
            if (Touche == ToucheAjouteDeplace || Touche == ToucheSupprime || Touche == ToucheCoupe)
            {
                // on termine l'action en cours en l'annulant au cas ou le bouton gauche de la souris soit encore enfoncé
                IsActionEncours = false;
                Action = Actions.Aucune;
                // on quitte le mode d'édition encours
                Edition = Editions.Aucune;
                CurseurEncours = Curseurs.TraceDefaut;
                return true;
            }
            return false;
        }
        #endregion
        #endregion
    }

    // ''' <summary> stockage des attributs d'un point de la trace </summary>
    internal struct Attribut
    {
        internal short Altitude;
        internal DateTime Horaire;
        internal Attribut(short alti = 0, DateTime Heure = default)
        {
            Altitude = alti;
            Horaire = Heure;
        }
    }
    internal class ResumeTrace
    {
        internal readonly int Index;
        internal string Nom;
        internal bool IsVisible;
        internal Color CouleurSegment;
        internal ResumeTrace(int IndexTrace, string NomTrace, bool Visible, Color Couleur)
        {
            Index = IndexTrace;
            Nom = NomTrace;
            IsVisible = Visible;
            CouleurSegment = Couleur;
        }
    }
}