using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.ObjetRectangle;
using static FCGP.Settings;

namespace FCGP
{
    /// <summary> permet la modification par sélection et suppression de tuiles, dans un fichier KMZ </summary>
    internal partial class AffichageKMZ
    {
        // constantes pour la lecture des différents groupes composants un fichier doc.kml ou d'une tuile
        // todo : enlever 1  à NbLignesTuile
        private const int NbLignesRegion = 8;
        private const int NbLignesFinDoc = 2;
        private const int NbBytesMaxJpg = 1024 * 500;
        private double DD_PixelX, DD_PixelY;
        private string NomFichierKMZ;
        private FileStream FluxLectureKMZ;
        // variable partagées avec le form calepinageKMZ
        private DescriptionImageFichier[] TuilesKMZ;
        private DescriptionImageFichier DescriptionFichierKMZ;
        private bool[] TuilesKMZIsSelected;
        private bool FlagInitCalepinage;
        private int NbTuileKMZSuppressed, NbTuilesKMZ, NbColsKMZ, NbRangsKMZ, NbLignesTuile;
        private Size TailleAffichage;
        private ZipArchive ArchiveLectureKMZ;
        private string Titre;
        private void AffichageKMZ_Load(object sender, EventArgs e)
        {
            Titre = TitreInformation;
            TitreInformation = "Modification Fichier KMZ";
            ChercheFichierKMZ.Title = "Sélection d'un fichier KMZ";
            ChercheFichierKMZ.Filter = "kmz files (*.kmz)|*.kmz";
            ChercheFichierKMZ.InitialDirectory = PartagerSettings.CHEMIN_TUILE;
            TailleAffichage = new Size(800, 800);
            Initialiser();
        }
        private void AffichageKMZ_FormClosed(object sender, FormClosedEventArgs e)
        {
            LibererFluxLecture();
            TitreInformation = Titre;
        }
        private void ChoixFichierKMZ_Click(object sender, EventArgs e)
        {
            if (ChercheFichierKMZ.ShowDialog(this) == DialogResult.OK)
            {
                NomFichier.Text = "";
                NbTuileKMZSuppressed = 0;
                NbColsKMZ = 0;
                NbRangsKMZ = 0;
                FlagInitCalepinage = true;
                if (ChargerKMZ(ChercheFichierKMZ.FileName))
                {
                    NomFichierKMZ = ChercheFichierKMZ.FileName;
                    NomFichier.Text = Path.GetFileNameWithoutExtension(NomFichierKMZ);
                    Selectionner.Enabled = true;
                    NbTuilesFichier.Text = $"{NbTuilesKMZ} ({NbColsKMZ}*{NbRangsKMZ})";
                    TailleFichier.Text = $"{DescriptionFichierKMZ.TaillePixels.Width}*{DescriptionFichierKMZ.TaillePixels.Height}";
                    TailleTuile.Text = $"{TuilesKMZ[0].TaillePixels.Width}*{TuilesKMZ[0].TaillePixels.Height}";
                    NbTuilesSelect.Text = "0";
                }
                else
                {
                    Initialiser();
                }
            }
        }
        /// <summary> supprime les tuiles sélectionnées du fichier KMZ </summary>
        private void ModifierTuiles_Click(object sender, EventArgs e)
        {
            var DebutTraitement = DateTime.Now;
            Cursor = Cursors.WaitCursor;
            string NomNouveauFichierKMZ = $@"{Path.GetDirectoryName(NomFichierKMZ)}\{Path.GetFileNameWithoutExtension(NomFichierKMZ)}_Modif.kmz";
            if (File.Exists(NomNouveauFichierKMZ))
                File.Delete(NomNouveauFichierKMZ);
            ModifierArchiveKMZ(NomNouveauFichierKMZ);
            Cursor = Cursors.Default;
            var Duree = DateTime.Now - DebutTraitement;
            MessageInformation = $"Le fichier {NomFichier.Text}_Modif.kmz a été créé.{CrLf}Temps de traitement : {TimeSpanToStr(Duree, Duree.TotalSeconds > 59)}";
            AfficherInformation();
            Initialiser();
        }
        private void Selectionner_Click(object sender, EventArgs e)
        {
            using (var F = new CalepinageKMZ())
            {
                F.ShowDialog(this);
                // armement de la non recréation des contrôlesrectangles, l'arrangement des tuiles ne changera que si on change de fichier KMZ
                FlagInitCalepinage = false;
                NbTuilesSelect.Text = NbTuileKMZSuppressed.ToString();
            }

            Modifier.Enabled = NbTuileKMZSuppressed > 0;
        }
        /// <summary> remplit le tableau des tuiles qui composent le fichier kmz créé avec FCGP 8.6 ou + </summary>
        /// <param name="FichierKMZ"> chemin du fichier kmz </param>
        public bool ChargerKMZ(string FichierKMZ)
        {
            var Ret = default(bool);
            LibererFluxLecture();
            // ouverture du fichier kmz existant et récupération de celui-ci sous forme d'archive
            FluxLectureKMZ = new FileStream(FichierKMZ, FileMode.Open);
            ArchiveLectureKMZ = new ZipArchive(FluxLectureKMZ, ZipArchiveMode.Read);
            // récupération de l'entrée du fichier doc.kml sous forme de flux en lecture
            using (var DocKMLFlux = new StreamReader(ArchiveLectureKMZ.GetEntry("doc.kml").Open()))
            {
                var KMZ = XElement.Load(DocKMLFlux);
                XNamespace xmlnsimport = KMZ.Attribute(XName.Get("xmlns")).Value;
                var Document = KMZ.Element(xmlnsimport.GetName("Document"));
                if (Document is not null && xmlnsimport.NamespaceName.EndsWith("FCGP"))
                {
                    // l'élement Document a été trouvé, créé avec FCGP 8.6 et pas encore modifié
                    // recuperation des données concernant le fichier kmz
                    string NomKMZ = Document.Element(xmlnsimport.GetName("name")).Value;
                    var RegionKMZ = Document.Element(xmlnsimport.GetName("Region"));
                    var LatLonBoxKMZ = RegionKMZ.Element(xmlnsimport.GetName("LatLonAltBox"));

                    // récupération de la description des tuiles composant le fichier
                    var ElementTuilesKMZ = Document.Elements(xmlnsimport.GetName("GroundOverlay"));
                    if (ElementTuilesKMZ.Any()) // il y a au moins une tuile
                    {
                        // tableaux de stockage
                        NbTuilesKMZ = ElementTuilesKMZ.Count();
                        TuilesKMZ = new DescriptionImageFichier[NbTuilesKMZ];
                        TrouverDescriptionTuilesKMZ(0, ElementTuilesKMZ.ElementAtOrDefault(0), xmlnsimport);
                        // récupération des informations cartographiques concernant le fichier kmz
                        TrouverDescriptionFichierKMZ(NomKMZ, LatLonBoxKMZ, xmlnsimport);
                        for (int Cpt = 1, loopTo = NbTuilesKMZ - 1; Cpt <= loopTo; Cpt++)
                            // récupération de la description de chaque tuile
                            TrouverDescriptionTuilesKMZ(Cpt, ElementTuilesKMZ.ElementAtOrDefault(Cpt), xmlnsimport);
                        NbColsKMZ += 1;
                        NbRangsKMZ += 1;
                        if (NbColsKMZ * NbRangsKMZ != NbTuilesKMZ)
                        {
                            MessageInformation = "Le fichier KMZ est corrompu." + CrLf + "Les indices des tuiles n'est pas en relation le nb de tuiles du fichier";
                            AfficherInformation();
                        }
                        Ret = true;
                    }
                    else
                    {
                        MessageInformation = "Le fichier KMZ est corrompu. Il n'y a pas de tuiles";
                        AfficherInformation();
                    }
                }
                else
                {
                    if (xmlnsimport.NamespaceName.EndsWith("FCGP_Modif"))
                    {
                        MessageInformation = "Le fichier KMZ a déjà été modifié";
                    }
                    else
                    {
                        MessageInformation = "Le fichier KMZ n'a pas été créé avec " + CrLf + "FCGP Capturer N°8.600 ou supérieur ou" + CrLf + "FCGP Regrouper N°3.600 ou supérieur";
                    }
                    AfficherInformation();
                }
            }
            return Ret;
        }
        /// <summary> supprime les tuiles sélectionnées d'un fichier KMZ créer avec FCGP </summary>
        /// <param name="FichierKMZ"> chemin du fichier kmz </param>
        private void ModifierArchiveKMZ(string FichierKMZ)
        {
            // récupération du fichier doc.kml existant
            var BB = new RectangleD(180.0d, -90.0d, -180.0d, 90.0d);
            var DocKmlEntete = new StringBuilder();
            var DocKmlTuiles = new StringBuilder();
            var DocKmlFin = new StringBuilder();
            var DocKmlRegion = new StringBuilder();
            using (var FluxEcritureKMZ = new FileStream(FichierKMZ, FileMode.Create))
            {
                using (var ArchiveEcritureKMZ = new ZipArchive(FluxEcritureKMZ, ZipArchiveMode.Create))
                {
                    // ouverture du fichier doc.kml du fichier KMZ existant
                    using (var DocKmlLecture = new StreamReader(ArchiveLectureKMZ.GetEntry("doc.kml").Open()))
                    {
                        // récupère le début de l'entête du fichier doc
                        LireDocKML(DocKmlLecture, DocKmlEntete, 1);
                        // on modifie le Xmlns du fichier doc.kml pour indiquer que le fichier kmz est modifié
                        LireDocKML(DocKmlLecture, DocKmlEntete, "FCGP", "FCGP_Modif");
                        // récupère la fin de l'entête du fichier doc
                        LireDocKML(DocKmlLecture, DocKmlEntete, 3);
                        // ignore la région du fichier
                        LireDocKML(DocKmlLecture, NbLignesRegion);
                        // lecture des tuiles existantes
                        for (int NumTuile = 0, loopTo = NbTuilesKMZ - 1; NumTuile <= loopTo; NumTuile++)
                        {
                            if (!TuilesKMZIsSelected[NumTuile])
                            {
                                // ignore la tuile qui n'est pas marquée pour être supprimée mais avance le pointeur de lecture en conséquence
                                LireDocKML(DocKmlLecture, NbLignesTuile);
                            }
                            else
                            {
                                // recupère les informations de la tuile marquée
                                LireDocKML(DocKmlLecture, DocKmlTuiles, NbLignesTuile);
                                // calcul de la plus grande surface contenant l'ensemble des tuiles. 
                                var RegionKMZ = TuilesKMZ[NumTuile].Region;
                                BB = new RectangleD(Math.Min(RegionKMZ.Pt0.X, BB.Pt0.X), Math.Max(RegionKMZ.Pt0.Y, BB.Pt0.Y), Math.Max(RegionKMZ.Pt2.X, BB.Pt2.X), Math.Min(RegionKMZ.Pt2.Y, BB.Pt2.Y));  // O
                                                                                                                                                                                                          // N
                                                                                                                                                                                                          // E
                                                                                                                                                                                                          // S
                                                                                                                                                                                                          // copie l'image de la tuile dans le nouveau fichier
                                string Nomtuile = "files/" + TuilesKMZ[NumTuile].Nom;
                                using (var TuileLecture = new BinaryReader(ArchiveLectureKMZ.GetEntry(Nomtuile).Open()))
                                {
                                    using (var TuileEcriture = new BinaryWriter(ArchiveEcritureKMZ.CreateEntry(Nomtuile).Open()))
                                    {
                                        TuileEcriture.Write(TuileLecture.ReadBytes(NbBytesMaxJpg));
                                    }
                                }
                            }
                        }
                        // recupère la fin du fichier dockml
                        LireDocKML(DocKmlLecture, DocKmlFin, NbLignesFinDoc);
                    }
                    // recupère la region du fichier
                    EcrireRegionDocKML(DocKmlRegion, BB);
                    using (var DocKmlEcriture = new StreamWriter(ArchiveEcritureKMZ.CreateEntry("doc.kml").Open()))
                    {
                        DocKmlEcriture.Write(DocKmlEntete.ToString());
                        DocKmlEcriture.Write(DocKmlRegion.ToString());
                        DocKmlEcriture.Write(DocKmlTuiles.ToString());
                        DocKmlEcriture.Write(DocKmlFin.ToString());
                    }
                }
                FluxEcritureKMZ.Close();
            }
        }
        /// <summary> calcule la taille en pixels du rectangle englobant toutes les tuiles du fichier </summary>
        /// <param name="ArchiveKMZ"> archive du fichier KMZ </param>
        private void TrouverDescriptionFichierKMZ(string NomKMZ, XElement LatLonBoxKMZ, XNamespace xmlnsimport)
        {
            // récupère les constantes du fichier sur les X et Y en prenant la 1ère tuile comme référence, c'est toujours la tuile qui a la plus grande taille en pixels
            var Tuile0 = TuilesKMZ[0];
            DescriptionFichierKMZ = new DescriptionImageFichier()
            {
                Nom = NomKMZ,
                Region = RenvoyerBoundingBox(LatLonBoxKMZ, xmlnsimport)
            };

            using (var E = new Bitmap(ArchiveLectureKMZ.GetEntry("files/" + Tuile0.Nom).Open()))
            {
                Tuile0.TaillePixels = E.Size;
                DD_PixelX = Tuile0.Region.LargeurCoordonnees / E.Width;
                DD_PixelY = Tuile0.Region.HauteurCoordonnees / E.Height;
            }
            // calcule la taille en pixels représentée par le fichier KMZ
            DescriptionFichierKMZ.TaillePixels = new Size((int)Math.Round(DescriptionFichierKMZ.Region.LargeurCoordonnees / DD_PixelX),
                                                          (int)Math.Round(DescriptionFichierKMZ.Region.HauteurCoordonnees / DD_PixelY));
        }
        /// <summary> déserialise une tuile de fichier kmz </summary>
        /// <param name="T"> tuile sérialisée </param>
        /// <param name="xmlnsimport"> espace de nom du fichier </param>
        /// <returns> la tuile sous forme de tuile fichier </returns>
        private void TrouverDescriptionTuilesKMZ(int NumTuile, XElement T, XNamespace xmlnsimport)
        {
            string Nom = T.Element(xmlnsimport.GetName("name")).Value;
            var BB = RenvoyerBoundingBox(T.Element(xmlnsimport.GetName("LatLonBox")), xmlnsimport);
            TuilesKMZ[NumTuile] = new DescriptionImageFichier() { Nom = Nom, Region = BB };
            // le nb de ligne peut être 13 ou 14 suivant que le fichier a été créé en Debug ou Release
            NbLignesTuile = T.ToString().Split(CrLf).Length;
            // récupération des informations cartographiques concernant la tuiles du fichier sans passer par l'image
            // calcule la taille en pixels représentée par la tuile
            var Taille = NumTuile == 0 ? Size.Empty : new Size((int)Math.Round(TuilesKMZ[NumTuile].Region.Largeur / DD_PixelX),
                                                               (int)Math.Round(-TuilesKMZ[NumTuile].Region.Hauteur / DD_PixelY));
            TuilesKMZ[NumTuile].TaillePixels = Taille;
            // calcule les indices de la tuile
            var Indice = new Point(int.Parse(TuilesKMZ[NumTuile].Nom.Substring(1, 3)), int.Parse(TuilesKMZ[NumTuile].Nom.Substring(4, 3)));
            // cherche le nb de colonnes et de rangées dans le fichier
            if (Indice.X > NbColsKMZ)
                NbColsKMZ = Indice.X;
            if (Indice.Y > NbRangsKMZ)
                NbRangsKMZ = Indice.Y;
        }
        /// <summary> initialise les différents champs d'information de l'utilisateur </summary>
        private void Initialiser()
        {
            Modifier.Enabled = false;
            Selectionner.Enabled = false;
            NbTuilesFichier.Text = "0";
            TailleFichier.Text = "";
            TailleTuile.Text = "";
            NomFichier.Text = "";
            NbTuilesSelect.Text = "0";
            NomFichierKMZ = null;
            NomFichier.Text = null;
        }
        /// <summary> Libère les ressources associées au fichier KMZ existant </summary>
        private void LibererFluxLecture()
        {
            if (ArchiveLectureKMZ is not null)
            {
                ArchiveLectureKMZ.Dispose();
                ArchiveLectureKMZ = null;
            }
            if (FluxLectureKMZ is not null)
            {
                FluxLectureKMZ.Close();
                FluxLectureKMZ.Dispose();
                FluxLectureKMZ = null;
            }
        }
        /// <summary> sérialize une boudingbox dans une stringbuilder </summary>
        /// <param name="SB"> stringbuilder de stockage </param>
        /// <param name="BB"> boudingbox à sérialiser </param>
        private static void EcrireRegionDocKML(StringBuilder SB, RectangleD BB)
        {
            SB.AppendLine("    <Region> ");
            SB.AppendLine("      <LatLonAltBox>");
            SB.AppendLine($"        <north>{DblToStr(BB.Pt0.Y, "0.000000000")}</north>");    // BB.N
            SB.AppendLine($"        <south>{DblToStr(BB.Pt2.Y, "0.000000000")}</south>");    // BB.S
            SB.AppendLine($"        <east>{DblToStr(BB.Pt2.X, "0.000000000")}</east>");      // BB.E
            SB.AppendLine($"        <west>{DblToStr(BB.Pt0.X, "0.000000000")}</west>");      // BB.O
            SB.AppendLine("      </LatLonAltBox>");
            SB.AppendLine("    </Region>");
        }
        /// <summary> Lit des lignes dans un flux texte et les écrit dans un stringbuilder </summary>
        /// <param name="Flux"> flux associé au fichier doc.kml de l'archive </param>
        /// <param name="SB"> stringbuilder pour le stockage </param>
        /// <param name="NbLignes"> nb de lignes à lire </param>
        private static void LireDocKML(StreamReader Flux, StringBuilder SB, int NbLignes)
        {
            for (int Line = 1, loopTo = NbLignes; Line <= loopTo; Line++)
                SB.AppendLine(Flux.ReadLine());
        }
        /// <summary> lit des lignes dans un flux texte sans les stocker, donc on les ignore </summary>
        /// <param name="Flux"> flux associé au fichier doc.kml de l'archive </param>
        /// <param name="NbLignes"> nb de lignes à ignorer </param>
        private static void LireDocKML(StreamReader Flux, int NbLignes)
        {
            for (int Line = 1, loopTo = NbLignes; Line <= loopTo; Line++)
                Flux.ReadLine();
        }
        /// <summary> lit une ligne du flux à la position actuelle. remplace une chaine de caractère par une autre
        /// et l'ajoute à la string builder </summary>
        /// <param name="Flux"> flux associé au fichier doc.kml de l'archive </param>
        /// <param name="SB"> stringbuilder pour le stockage </param>
        /// <param name="Acien"> Chaine à remplacer </param>
        /// <param name="Nouveau"> chaine de remplacement </param>
        private static void LireDocKML(StreamReader Flux, StringBuilder SB, string Ancien, string Nouveau)
        {
            SB.AppendLine(Flux.ReadLine().Replace(Ancien, Nouveau));
        }
        /// <summary> déserialise une boundingbox sérialisée sous forme de 4 valeurs distinctes </summary>
        /// <param name="BB"> BoundingBox sérialisée </param>
        /// <param name="xmlnsimport"> espace de nom du fichier </param>
        /// <returns>les valeurs numériques sous forme de tuplevaleur </returns>
        private static RectangleD RenvoyerBoundingBox(XElement BB, XNamespace xmlnsimport)
        {
            double N = StrToDbl(BB.Element(xmlnsimport.GetName("north")).Value);
            double S = StrToDbl(BB.Element(xmlnsimport.GetName("south")).Value);
            double O = StrToDbl(BB.Element(xmlnsimport.GetName("west")).Value);
            double E = StrToDbl(BB.Element(xmlnsimport.GetName("east")).Value);
            return new RectangleD(O, N, E, S);
        }

        #region Form CalepinageKMZ
        private partial class CalepinageKMZ
        {
            private int NumForme;
            private double CoefR;
            private int Xorigine;
            private int Yorigine;
            private const int Affiche_Bords = 27;
            // communication entre formulaire parent et enfant
            private AffichageKMZ F;
            private Keys Modifier;
            private Point PosMouse;
            /// <summary> initialise le formulaire d'affichage du plan de l'échelle </summary>
            private void CalepinageKMZ_Load(object sender, EventArgs e)
            {
                // trouve le formulaire appelant pour le centrage et pour l'appel de la boite de dialogue Image
                F = (AffichageKMZ)Owner;
#pragma warning disable CS1690 // L'accès à un membre sur le champ d'une classe de marshaling par référence peut entraîner une exception de runtime
                ClientSize = new Size(F.TailleAffichage.Width, F.TailleAffichage.Height + Info.Height);
#pragma warning restore CS1690 // L'accès à un membre sur le champ d'une classe de marshaling par référence peut entraîner une exception de runtime
                // centre le formulaire sur le formulaire parent
                {
                    Location = new Point(Owner.Location.X + (Owner.Width - Width) / 2, Owner.Location.Y + (Owner.Height - Height) / 2);
                }

                Info1.Text = $"Nb Tuiles : {F.NbTuilesKMZ}";
                Info2.Text = $"Nb Tuiles à supprimer : {F.NbTuileKMZSuppressed}";
                // on ajoute les tuiles du fichier kmz en tant que rectangles simples
                FaireRectanglesTuiles();
            }
            /// <summary> renvoie les tuiles sélectionnées (nb et liste) pour pouvoir les supprimer </summary>
            private void CalepinageKMZ_FormClosed(object sender, FormClosedEventArgs e)
            {
                F.NbTuileKMZSuppressed = NbRectanglesNotSelected;
                // ainsi que la valeur de IsSelected pour chaque tuile
                F.TuilesKMZIsSelected = RectanglesIsSelected;
            }
            /// <summary>  Affichage sur le formulaire l'ensemble des rectangles qui composent le fichier kmz </summary>
            private void CalepinageKMZ_Paint(object sender, PaintEventArgs e)
            {
                AfficherRectangles(e.Graphics);
            }
            /// <summary> prend en compte le fait que l'utilisateur appui sur un des boutons de la souris pour débuter une action 
            /// qui dépend du type de rectangle et de leur utilisation </summary>
            private void CalepinageKMZ_MouseDown(object sender, MouseEventArgs e)
            {
                // on enregistre la position de départ pour replacer le curseur en revenant de la boite de dialogue Image
                PosMouse = Cursor.Position;
                Modifier = ModifierKeys;
                // cherche si on est au début création de rectangle de travail ou au début click sur un rectangle simple 
                AppuyerBoutonSouris(e.Location, Modifier);
            }
            /// <summary> gestion du déplacement de la souris sur les rectangles </summary>
            private void CalepinageKMZ_MouseMove(object sender, MouseEventArgs e)
            {
                // trouver le rectangle qui est survolé
                DeplacerSouris(e.Location);
                switch (NumRectangleSurvol)
                {
                    case -1: // pas de rectangle survolé
                        {
                            // on efface l'ancien rectangle survolé puisqu'il ne l'est plus
                            if (NumForme != -1)
                                Invalidate();
                            break;
                        }
                    case 0: // rectangle de travail survolé
                        {
                            // on redessine toujours le rectangle de travail sur un évènement MouseMove (changement de dimensions)
                            Invalidate();
                            break;
                        }
                    case var @case when @case > 0: // tuile survolée
                        {
                            if (NumForme != NumRectangleSurvol)
                            {
                                // on affiche à l'écran le nouveau rectangle simple survolé
                                Invalidate();
                            }

                            break;
                        }
                }
                // on garde en mémoire le rectangle survolé
                NumForme = NumRectangleSurvol;
            }
            /// <summary> prend en compte le fait que l'utilisateur relache le bouton de la souris pour finir l'action en cours </summary>
            private void CalepinageKMZ_MouseUp(object sender, MouseEventArgs e)
            {
                RelacherBoutonSouris();
                if (ClickRectangle)
                {
                    // maj de l'info sur le formulaire
                    Info2.Text = $"Nb Tuiles à supprimer : {NbRectanglesNotSelected}";
                    Invalidate();
                    // replace le curseur à l'endroit de départ si la boite de dialogue Image a été appelée
                    if (Modifier == Keys.Alt && NumForme > 0)
                    {
                        Cursor.Position = PosMouse;
                    }
                }
            }
            /// <summary> gère les touches clavier envoyées au formulaire. Les touches escape ou entrée permettent de sortir </summary>
            private void CalepinageRegroupement_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                if (e.KeyCode == Keys.Enter)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            /// <summary> affiche l'image de la tuile. Action à passer en propriété aux rectangles simples </summary>
            /// <param name="Numtuile"> index de la tuile dans le fichier KMZ </param>
            private DialogResult AfficherImageTuile(int NumTuile)
            {
                string NomTuile = "files/" + F.TuilesKMZ[NumTuile - 1].Nom;
                using (var ImageTuile = Image.FromStream(F.ArchiveLectureKMZ.GetEntry(NomTuile).Open()))
                {
                    int LargeurAffichage = ImageTuile.Width + 2;
                    int HauteurAffichage = ImageTuile.Height + 2;
                    if (DimensionsEcranSupport.Width < LargeurAffichage)
                        LargeurAffichage = DimensionsEcranSupport.Width;
                    if (DimensionsEcranSupport.Height < HauteurAffichage)
                        HauteurAffichage = DimensionsEcranSupport.Height;
                    // F est le formulaire parent
                    return AfficherImage(F, ImageTuile, new Size(LargeurAffichage, HauteurAffichage));
                }
            }
            /// <summary> détermine les rectangles qui composent la représentation des tuiles du fichier KMZ </summary>
            private void FaireRectanglesTuiles()
            {
                var DimensionsPlanTuiles = new Size(ClientSize.Width - Affiche_Bords * 2, ClientSize.Height - Affiche_Bords * 2 - Info.Height);
                // récupération de la surface de l'ensemble des tuiles du ficheir KMZ et calcul du coefficient de mise à l'échelle dessin/regroupement
                var DimensionsTuiles = F.DescriptionFichierKMZ.TaillePixels;
                double R_X = DimensionsPlanTuiles.Width / (double)DimensionsTuiles.Width;
                double R_Y = DimensionsPlanTuiles.Height / (double)DimensionsTuiles.Height;
                // on garde les proportions des tuiles
                if (R_X > R_Y)
                {
                    CoefR = R_Y;
                }
                else
                {
                    CoefR = R_X;
                }
                // calcul des X et Y d'origine en pixels de la zone de dessin en prenant les dimensions pixels des rectangles et pas de la carte
                // pour éviter les erreurs d'arrondi
                int XPixels = (int)Math.Round(F.TuilesKMZ[0].LargeurPixels * CoefR) * (F.NbColsKMZ - 1) +
                              (int)Math.Round((F.DescriptionFichierKMZ.LargeurPixels - (F.NbColsKMZ - 1) * F.TuilesKMZ[0].LargeurPixels) * CoefR);
                int YPixels = (int)Math.Round(F.TuilesKMZ[0].HauteurPixels * CoefR) * (F.NbRangsKMZ - 1) +
                              (int)Math.Round((F.DescriptionFichierKMZ.HauteurPixels - (F.NbRangsKMZ - 1) * F.TuilesKMZ[0].HauteurPixels) * CoefR);
                Xorigine = (DimensionsPlanTuiles.Width - XPixels) / 2 + Affiche_Bords;
                Yorigine = (DimensionsPlanTuiles.Height - YPixels) / 2 + Affiche_Bords;
                // si c'est la 1ère fois que l'on ouvre le fichier KMZ on lance le calcul et la création des tuiles sous forme de rectangle simples
                if (F.FlagInitCalepinage)
                {
                    // définit l'action à réaliser pour un click sur un rectangle simple
                    ActionRectangleSimple = AfficherImageTuile;
                    // initialisation de la classe rectangle_base avec la possibilité d'avoir un rectangle de travail
                    // Le +3, -6 correspond à la largeur minimum du rectangle de séléction
                    var SurfaceDessin = new Rectangle(new Point(ClientRectangle.X + 3, ClientRectangle.Y + 3),
                                                      new Size(ClientRectangle.Width - 6, ClientRectangle.Height - Info.Height - 6));
                    InitialiserRectangles(SurfaceDessin, Mode.Selection, Rectangle.Empty, true);
                    int IndexTuileKMZ = 0;
                    int Y = Yorigine;
                    for (int Row = 0, loopTo = F.NbRangsKMZ - 1; Row <= loopTo; Row++)
                    {
                        int X = Xorigine;
                        int H = 0;
                        int L = 0;
                        for (int Col = 0, loopTo1 = F.NbColsKMZ - 1; Col <= loopTo1; Col++)
                        {
                            string Name = F.TuilesKMZ[IndexTuileKMZ].Nom;
                            // calcul des dimensions de la tuile en pixels de la zone de dessin
                            var DimensionsTuileKMZ = F.TuilesKMZ[IndexTuileKMZ].TaillePixels;
                            // la largeur est à calculer pour la 1ere et la dernière colonne
                            if (Col == 0 || Col == F.NbColsKMZ - 1)
                                L = (int)Math.Round(DimensionsTuileKMZ.Width * CoefR);
                            // la hauteur est à calculer qu'une seule fois à la 1ere colonne
                            if (Col == 0)
                                H = (int)Math.Round(DimensionsTuileKMZ.Height * CoefR);
                            // ajout dans la collection du rectangle représentant la tuile. Au départ ils sont tous sélectionnés
                            AjouterRectangle(new Point(X, Y), new Point(X + L, Y + H), Name, true);
                            IndexTuileKMZ += 1;
                            X += L;
                        }
                        Y += H;
                    }
                }
            }
        }
        private partial class CalepinageKMZ : Form
        {
            internal CalepinageKMZ()
            {
                InitialiseComposants();
                InitialiserEvenements();
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && components is not null)
                    {
                        components.Dispose();
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }

            // Requise par le Concepteur Windows Form
            private IContainer components;
            private void InitialiserEvenements()
            {
                Load += CalepinageKMZ_Load;
                FormClosed += CalepinageKMZ_FormClosed;
                Paint += CalepinageKMZ_Paint;
                MouseDown += CalepinageKMZ_MouseDown;
                MouseMove += CalepinageKMZ_MouseMove;
                MouseUp += CalepinageKMZ_MouseUp;
                KeyDown += CalepinageRegroupement_KeyDown;
            }
            private void InitialiseComposants()
            {
                components = new Container();
                Info1 = new Label();
                Info2 = new Label();
                Info = new Label();
                SuspendLayout();
                // 
                // Info1
                // 
                Info1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                Info1.BorderStyle = BorderStyle.FixedSingle;
                Info1.FlatStyle = FlatStyle.Flat;
                Info1.Location = new Point(-1, 701);
                Info1.Name = "Info1";
                Info1.Size = new Size(127, 23);
                Info1.TabIndex = 0;
                Info1.Text = "Nb Tuiles : 1000";
                Info1.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // Info2
                // 
                Info2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                Info2.BorderStyle = BorderStyle.FixedSingle;
                Info2.FlatStyle = FlatStyle.Flat;
                Info2.Location = new Point(125, 701);
                Info2.Name = "Info2";
                Info2.Size = new Size(201, 23);
                Info2.TabIndex = 1;
                Info2.Text = "Nb Tuiles à supprimer : 1000";
                Info2.TextAlign = ContentAlignment.MiddleCenter;
                // 
                // Info
                // 
                Info.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
                Info.BorderStyle = BorderStyle.FixedSingle;
                Info.FlatStyle = FlatStyle.Flat;
                Info.Font = new Font("Segoe UI", 14.0f, FontStyle.Bold, GraphicsUnit.Pixel);
                Info.Location = new Point(325, 701);
                Info.Name = "Info";
                Info.Size = new Size(376, 23);
                Info.TabIndex = 2;
                Info.Text = "Esc ou Enter pour Fermer";
                Info.TextAlign = ContentAlignment.MiddleRight;
                // 
                // CalepinageKMZ
                // 
                AutoScaleMode = AutoScaleMode.None;
                AutoSize = false;
                AutoSizeMode = AutoSizeMode.GrowOnly;
                ClientSize = new Size(700, 723);
                ControlBox = false;
                Controls.Add(Info1);
                Controls.Add(Info2);
                Controls.Add(Info);
                Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "CalepinageKMZ";
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.Manual;
                DoubleBuffered = true;
                ResumeLayout(false);
            }

            private Label Info;
            private Label Info1;
            private Label Info2;
        }
        #endregion
    }
}