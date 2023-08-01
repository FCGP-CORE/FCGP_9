''' <summary> permet la modification par sélection et suppression de tuiles, dans un fichier KMZ </summary>
Friend Class AffichageKMZ
    'constantes pour la lecture des différents groupes composants un fichier doc.kml ou d'une tuile
    'todo : enlever 1  à NbLignesTuile
    Private Const NbLignesRegion As Integer = 8
    Private Const NbLignesFinDoc As Integer = 2, NbBytesMaxJpg As Integer = 1024 * 500
    Private DD_PixelX, DD_PixelY As Double, NomFichierKMZ As String, FluxLectureKMZ As FileStream
    'variable partagées avec le form calepinageKMZ
    Private TuilesKMZ(), DescriptionFichierKMZ As DescriptionImageFichier
    Private TuilesKMZIsSelected(), FlagInitCalepinage As Boolean
    Private NbTuileKMZSuppressed, NbTuilesKMZ, NbColsKMZ, NbRangsKMZ, NbLignesTuile As Integer
    Private TailleAffichage As Size, ArchiveLectureKMZ As ZipArchive
    Private Titre As String
    Private Sub AffichageKMZ_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        TitreInformation = "Modification Fichier KMZ"
        ChercheFichierKMZ.Title = "Sélection d'un fichier KMZ"
        ChercheFichierKMZ.Filter = "kmz files (*.kmz)|*.kmz"
        ChercheFichierKMZ.InitialDirectory = PartagerSettings.CHEMIN_TUILE
        TailleAffichage = New Size(800, 800)
        Initialiser()
    End Sub
    Private Sub AffichageKMZ_FormClosed(sender As Object, e As FormClosedEventArgs)
        LibererFluxLecture()
        TitreInformation = Titre
    End Sub
    Private Sub ChoixFichierKMZ_Click(sender As Object, e As EventArgs)
        If ChercheFichierKMZ.ShowDialog(Me) = DialogResult.OK Then
            NomFichier.Text = ""
            NbTuileKMZSuppressed = 0
            NbColsKMZ = 0
            NbRangsKMZ = 0
            FlagInitCalepinage = True
            If ChargerKMZ(ChercheFichierKMZ.FileName) Then
                NomFichierKMZ = ChercheFichierKMZ.FileName
                NomFichier.Text = Path.GetFileNameWithoutExtension(NomFichierKMZ)
                Selectionner.Enabled = True
                NbTuilesFichier.Text = $"{NbTuilesKMZ} ({NbColsKMZ}*{NbRangsKMZ})"
                TailleFichier.Text = $"{DescriptionFichierKMZ.TaillePixels.Width}*{DescriptionFichierKMZ.TaillePixels.Height}"
                TailleTuile.Text = $"{TuilesKMZ(0).TaillePixels.Width}*{TuilesKMZ(0).TaillePixels.Height}"
                NbTuilesSelect.Text = "0"
            Else
                Initialiser()
            End If
        End If
    End Sub
    ''' <summary> supprime les tuiles sélectionnées du fichier KMZ </summary>
    Private Sub ModifierTuiles_Click(sender As Object, e As EventArgs)
        Dim DebutTraitement As Date = Now
        Cursor = Cursors.WaitCursor
        Dim NomNouveauFichierKMZ = $"{Path.GetDirectoryName(NomFichierKMZ)}\{Path.GetFileNameWithoutExtension(NomFichierKMZ)}_Modif.kmz"
        If File.Exists(NomNouveauFichierKMZ) Then File.Delete(NomNouveauFichierKMZ)
        ModifierArchiveKMZ(NomNouveauFichierKMZ)
        Cursor = Cursors.Default
        Dim Duree = Now - DebutTraitement
        MessageInformation = $"Le fichier {NomFichier.Text}_Modif.kmz a été créé.{CrLf}Temps de traitement : {TimeSpanToStr(Duree, Duree.TotalSeconds > 59)}"
        AfficherInformation()
        Initialiser()
    End Sub
    Private Sub Selectionner_Click(sender As Object, e As EventArgs)
        Using F As New CalepinageKMZ
            F.ShowDialog(Me)
            'armement de la non recréation des contrôlesrectangles, l'arrangement des tuiles ne changera que si on change de fichier KMZ
            FlagInitCalepinage = False
            NbTuilesSelect.Text = NbTuileKMZSuppressed.ToString
        End Using

        Modifier.Enabled = NbTuileKMZSuppressed > 0
    End Sub
    ''' <summary> remplit le tableau des tuiles qui composent le fichier kmz créé avec FCGP 8.6 ou + </summary>
    ''' <param name="FichierKMZ"> chemin du fichier kmz </param>
    Function ChargerKMZ(FichierKMZ As String) As Boolean
        Dim Ret As Boolean
        LibererFluxLecture()
        'ouverture du fichier kmz existant et récupération de celui-ci sous forme d'archive
        FluxLectureKMZ = New FileStream(FichierKMZ, FileMode.Open)
        ArchiveLectureKMZ = New ZipArchive(FluxLectureKMZ, ZipArchiveMode.Read)
        'récupération de l'entrée du fichier doc.kml sous forme de flux en lecture
        Using DocKMLFlux As New StreamReader(ArchiveLectureKMZ.GetEntry("doc.kml").Open())
            Dim KMZ As XElement = XElement.Load(DocKMLFlux)
            Dim xmlnsimport As XNamespace = KMZ.Attribute(XName.Get("xmlns")).Value
            Dim Document As XElement = KMZ.Element(xmlnsimport.GetName("Document"))
            If Document IsNot Nothing AndAlso xmlnsimport.NamespaceName.EndsWith("FCGP") Then
                'l'élement Document a été trouvé, créé avec FCGP 8.6 et pas encore modifié
                'recuperation des données concernant le fichier kmz
                Dim NomKMZ = Document.Element(xmlnsimport.GetName("name")).Value
                Dim RegionKMZ = Document.Element(xmlnsimport.GetName("Region"))
                Dim LatLonBoxKMZ = RegionKMZ.Element(xmlnsimport.GetName("LatLonAltBox"))

                'récupération de la description des tuiles composant le fichier
                Dim ElementTuilesKMZ = Document.Elements(xmlnsimport.GetName("GroundOverlay"))
                If ElementTuilesKMZ.Any() Then 'il y a au moins une tuile
                    'tableaux de stockage
                    NbTuilesKMZ = ElementTuilesKMZ.Count
                    ReDim TuilesKMZ(NbTuilesKMZ - 1)
                    TrouverDescriptionTuilesKMZ(0, ElementTuilesKMZ(0), xmlnsimport)
                    'récupération des informations cartographiques concernant le fichier kmz
                    TrouverDescriptionFichierKMZ(NomKMZ, LatLonBoxKMZ, xmlnsimport)
                    For Cpt As Integer = 1 To NbTuilesKMZ - 1
                        'récupération de la description de chaque tuile
                        TrouverDescriptionTuilesKMZ(Cpt, ElementTuilesKMZ(Cpt), xmlnsimport)
                    Next
                    NbColsKMZ += 1
                    NbRangsKMZ += 1
                    If NbColsKMZ * NbRangsKMZ <> NbTuilesKMZ Then
                        MessageInformation = "Le fichier KMZ est corrompu." & CrLf &
                                             "Les indices des tuiles n'est pas en relation le nb de tuiles du fichier"
                        AfficherInformation()
                    End If
                    Ret = True
                Else
                    MessageInformation = "Le fichier KMZ est corrompu. Il n'y a pas de tuiles"
                    AfficherInformation()
                End If
            Else
                If xmlnsimport.NamespaceName.EndsWith("FCGP_Modif") Then
                    MessageInformation = "Le fichier KMZ a déjà été modifié"
                Else
                    MessageInformation = "Le fichier KMZ n'a pas été créé avec " & CrLf &
                                         "FCGP Capturer N°8.600 ou supérieur ou" & CrLf &
                                         "FCGP Regrouper N°3.600 ou supérieur"
                End If
                AfficherInformation()
            End If
        End Using
        Return Ret
    End Function
    ''' <summary> supprime les tuiles sélectionnées d'un fichier KMZ créer avec FCGP </summary>
    ''' <param name="FichierKMZ"> chemin du fichier kmz </param>
    Private Sub ModifierArchiveKMZ(FichierKMZ As String)
        'récupération du fichier doc.kml existant
        Dim BB As New RectangleD(180.0R, -90.0R, -180.0R, 90.0R)
        Dim DocKmlEntete As New StringBuilder, DocKmlTuiles As New StringBuilder, DocKmlFin As New StringBuilder, DocKmlRegion As New StringBuilder
        Using FluxEcritureKMZ As New FileStream(FichierKMZ, FileMode.Create)
            Using ArchiveEcritureKMZ As New ZipArchive(FluxEcritureKMZ, ZipArchiveMode.Create)
                'ouverture du fichier doc.kml du fichier KMZ existant
                Using DocKmlLecture As New StreamReader(ArchiveLectureKMZ.GetEntry("doc.kml").Open())
                    'récupère le début de l'entête du fichier doc
                    LireDocKML(DocKmlLecture, DocKmlEntete, 1)
                    'on modifie le Xmlns du fichier doc.kml pour indiquer que le fichier kmz est modifié
                    LireDocKML(DocKmlLecture, DocKmlEntete, "FCGP", "FCGP_Modif")
                    'récupère la fin de l'entête du fichier doc
                    LireDocKML(DocKmlLecture, DocKmlEntete, 3)
                    'ignore la région du fichier
                    LireDocKML(DocKmlLecture, NbLignesRegion)
                    'lecture des tuiles existantes
                    For NumTuile As Integer = 0 To NbTuilesKMZ - 1
                        If Not TuilesKMZIsSelected(NumTuile) Then
                            'ignore la tuile qui n'est pas marquée pour être supprimée mais avance le pointeur de lecture en conséquence
                            LireDocKML(DocKmlLecture, NbLignesTuile)
                        Else
                            'recupère les informations de la tuile marquée
                            LireDocKML(DocKmlLecture, DocKmlTuiles, NbLignesTuile)
                            'calcul de la plus grande surface contenant l'ensemble des tuiles. 
                            Dim RegionKMZ = TuilesKMZ(NumTuile).Region
                            BB = New RectangleD(Math.Min(RegionKMZ.Pt0.X, BB.Pt0.X),  'O
                                                Math.Max(RegionKMZ.Pt0.Y, BB.Pt0.Y),  'N
                                                Math.Max(RegionKMZ.Pt2.X, BB.Pt2.X),  'E
                                                Math.Min(RegionKMZ.Pt2.Y, BB.Pt2.Y))  'S
                            'copie l'image de la tuile dans le nouveau fichier
                            Dim Nomtuile = "files/" & TuilesKMZ(NumTuile).Nom
                            Using TuileLecture = New BinaryReader(ArchiveLectureKMZ.GetEntry(Nomtuile).Open)
                                Using TuileEcriture = New BinaryWriter(ArchiveEcritureKMZ.CreateEntry(Nomtuile).Open)
                                    TuileEcriture.Write(TuileLecture.ReadBytes(NbBytesMaxJpg))
                                End Using
                            End Using
                        End If
                    Next
                    'recupère la fin du fichier dockml
                    LireDocKML(DocKmlLecture, DocKmlFin, NbLignesFinDoc)
                End Using
                'recupère la region du fichier
                EcrireRegionDocKML(DocKmlRegion, BB)
                Using DocKmlEcriture As New StreamWriter(ArchiveEcritureKMZ.CreateEntry("doc.kml").Open())
                    DocKmlEcriture.Write(DocKmlEntete.ToString)
                    DocKmlEcriture.Write(DocKmlRegion.ToString)
                    DocKmlEcriture.Write(DocKmlTuiles.ToString)
                    DocKmlEcriture.Write(DocKmlFin.ToString)
                End Using
            End Using
            FluxEcritureKMZ.Close()
        End Using
    End Sub
    ''' <summary> calcule la taille en pixels du rectangle englobant toutes les tuiles du fichier </summary>
    ''' <param name="ArchiveKMZ"> archive du fichier KMZ </param>
    Private Sub TrouverDescriptionFichierKMZ(NomKMZ As String, LatLonBoxKMZ As XElement, xmlnsimport As XNamespace)
        'récupère les constantes du fichier sur les X et Y en prenant la 1ère tuile comme référence, c'est toujours la tuile qui a la plus grande taille en pixels
        Dim Tuile0 = TuilesKMZ(0)
        DescriptionFichierKMZ = New DescriptionImageFichier With {.Nom = NomKMZ,
                                                                  .Region = RenvoyerBoundingBox(LatLonBoxKMZ, xmlnsimport)}

        Using E As New Bitmap(ArchiveLectureKMZ.GetEntry("files/" & Tuile0.Nom).Open)
            Tuile0.TaillePixels = E.Size
            DD_PixelX = Tuile0.Region.LargeurCoordonnees / E.Width
            DD_PixelY = Tuile0.Region.HauteurCoordonnees / E.Height
        End Using
        'calcule la taille en pixels représentée par le fichier KMZ
        DescriptionFichierKMZ.TaillePixels = New Size(CInt(Math.Round(DescriptionFichierKMZ.Region.LargeurCoordonnees / DD_PixelX)),
                                                      CInt(Math.Round(DescriptionFichierKMZ.Region.HauteurCoordonnees / DD_PixelY)))
    End Sub
    ''' <summary> déserialise une tuile de fichier kmz </summary>
    ''' <param name="T"> tuile sérialisée </param>
    ''' <param name="xmlnsimport"> espace de nom du fichier </param>
    ''' <returns> la tuile sous forme de tuile fichier </returns>
    Private Sub TrouverDescriptionTuilesKMZ(NumTuile As Integer, T As XElement, xmlnsimport As XNamespace)
        Dim Nom As String = T.Element(xmlnsimport.GetName("name")).Value
        Dim BB = RenvoyerBoundingBox(T.Element(xmlnsimport.GetName("LatLonBox")), xmlnsimport)
        TuilesKMZ(NumTuile) = New DescriptionImageFichier() With {.Nom = Nom, .Region = BB}
        'le nb de ligne peut être 13 ou 14 suivant que le fichier a été créé en Debug ou Release
        NbLignesTuile = T.ToString.Split(CrLf).Length
        'récupération des informations cartographiques concernant la tuiles du fichier sans passer par l'image
        'calcule la taille en pixels représentée par la tuile
        Dim Taille = If(NumTuile = 0, Size.Empty, New Size(CInt(Math.Round(TuilesKMZ(NumTuile).Region.Largeur / DD_PixelX)),
                                                           CInt(Math.Round(-TuilesKMZ(NumTuile).Region.Hauteur / DD_PixelY))))
        TuilesKMZ(NumTuile).TaillePixels = Taille
        'calcule les indices de la tuile
        Dim Indice = New Point(CInt(TuilesKMZ(NumTuile).Nom.Substring(1, 3)), CInt(TuilesKMZ(NumTuile).Nom.Substring(4, 3)))
        'cherche le nb de colonnes et de rangées dans le fichier
        If Indice.X > NbColsKMZ Then NbColsKMZ = Indice.X
        If Indice.Y > NbRangsKMZ Then NbRangsKMZ = Indice.Y
    End Sub
    ''' <summary> initialise les différents champs d'information de l'utilisateur </summary>
    Private Sub Initialiser()
        Modifier.Enabled = False
        Selectionner.Enabled = False
        NbTuilesFichier.Text = "0"
        TailleFichier.Text = ""
        TailleTuile.Text = ""
        NomFichier.Text = ""
        NbTuilesSelect.Text = "0"
        NomFichierKMZ = Nothing
        NomFichier.Text = Nothing
    End Sub
    ''' <summary> Libère les ressources associées au fichier KMZ existant </summary>
    Private Sub LibererFluxLecture()
        If ArchiveLectureKMZ IsNot Nothing Then
            ArchiveLectureKMZ.Dispose()
            ArchiveLectureKMZ = Nothing
        End If
        If FluxLectureKMZ IsNot Nothing Then
            FluxLectureKMZ.Close()
            FluxLectureKMZ.Dispose()
            FluxLectureKMZ = Nothing
        End If
    End Sub
    ''' <summary> sérialize une boudingbox dans une stringbuilder </summary>
    ''' <param name="SB"> stringbuilder de stockage </param>
    ''' <param name="BB"> boudingbox à sérialiser </param>
    Private Shared Sub EcrireRegionDocKML(SB As StringBuilder, BB As RectangleD)
        SB.AppendLine("    <Region> ")
        SB.AppendLine("      <LatLonAltBox>")
        SB.AppendLine($"        <north>{DblToStr(BB.Pt0.Y, "0.000000000")}</north>")    'BB.N
        SB.AppendLine($"        <south>{DblToStr(BB.Pt2.Y, "0.000000000")}</south>")    'BB.S
        SB.AppendLine($"        <east>{DblToStr(BB.Pt2.X, "0.000000000")}</east>")      'BB.E
        SB.AppendLine($"        <west>{DblToStr(BB.Pt0.X, "0.000000000")}</west>")      'BB.O
        SB.AppendLine("      </LatLonAltBox>")
        SB.AppendLine("    </Region>")
    End Sub
    ''' <summary> Lit des lignes dans un flux texte et les écrit dans un stringbuilder </summary>
    ''' <param name="Flux"> flux associé au fichier doc.kml de l'archive </param>
    ''' <param name="SB"> stringbuilder pour le stockage </param>
    ''' <param name="NbLignes"> nb de lignes à lire </param>
    Private Shared Sub LireDocKML(Flux As StreamReader, SB As StringBuilder, NbLignes As Integer)
        For Line = 1 To NbLignes
            SB.AppendLine(Flux.ReadLine)
        Next
    End Sub
    ''' <summary> lit des lignes dans un flux texte sans les stocker, donc on les ignore </summary>
    ''' <param name="Flux"> flux associé au fichier doc.kml de l'archive </param>
    ''' <param name="NbLignes"> nb de lignes à ignorer </param>
    Private Shared Sub LireDocKML(Flux As StreamReader, NbLignes As Integer)
        For Line = 1 To NbLignes
            Flux.ReadLine()
        Next
    End Sub
    ''' <summary> lit une ligne du flux à la position actuelle. remplace une chaine de caractère par une autre
    ''' et l'ajoute à la string builder </summary>
    ''' <param name="Flux"> flux associé au fichier doc.kml de l'archive </param>
    ''' <param name="SB"> stringbuilder pour le stockage </param>
    ''' <param name="Acien"> Chaine à remplacer </param>
    ''' <param name="Nouveau"> chaine de remplacement </param>
    Private Shared Sub LireDocKML(Flux As StreamReader, SB As StringBuilder, Ancien As String, Nouveau As String)
        SB.AppendLine(Flux.ReadLine().Replace(Ancien, Nouveau))
    End Sub
    ''' <summary> déserialise une boundingbox sérialisée sous forme de 4 valeurs distinctes </summary>
    ''' <param name="BB"> BoundingBox sérialisée </param>
    ''' <param name="xmlnsimport"> espace de nom du fichier </param>
    ''' <returns>les valeurs numériques sous forme de tuplevaleur </returns>
    Private Shared Function RenvoyerBoundingBox(BB As XElement, xmlnsimport As XNamespace) As RectangleD
        Dim N As Double = StrToDbl(BB.Element(xmlnsimport.GetName("north")).Value)
        Dim S As Double = StrToDbl(BB.Element(xmlnsimport.GetName("south")).Value)
        Dim O As Double = StrToDbl(BB.Element(xmlnsimport.GetName("west")).Value)
        Dim E As Double = StrToDbl(BB.Element(xmlnsimport.GetName("east")).Value)
        Return New RectangleD(O, N, E, S)
    End Function

#Region "Form CalepinageKMZ"
    Private Class CalepinageKMZ
        Private NumForme As Integer, CoefR As Double, Xorigine As Integer, Yorigine As Integer
        Private Const Affiche_Bords As Integer = 27
        'communication entre formulaire parent et enfant
        Private F As AffichageKMZ, Modifier As Keys, PosMouse As Point
        ''' <summary> initialise le formulaire d'affichage du plan de l'échelle </summary>
        Private Sub CalepinageKMZ_Load(sender As Object, e As EventArgs)
            'trouve le formulaire appelant pour le centrage et pour l'appel de la boite de dialogue Image
            F = CType(Owner, AffichageKMZ)
            ClientSize = New Size(F.TailleAffichage.Width, F.TailleAffichage.Height + Info.Height)
            'centre le formulaire sur le formulaire parent
            With Owner
                Location = New Point(.Location.X + (.Width - Width) \ 2, .Location.Y + (.Height - Height) \ 2)
            End With

            Info1.Text = $"Nb Tuiles : {F.NbTuilesKMZ}"
            Info2.Text = $"Nb Tuiles à supprimer : {F.NbTuileKMZSuppressed}"
            'on ajoute les tuiles du fichier kmz en tant que rectangles simples
            FaireRectanglesTuiles()
        End Sub
        ''' <summary> renvoie les tuiles sélectionnées (nb et liste) pour pouvoir les supprimer </summary>
        Private Sub CalepinageKMZ_FormClosed(sender As Object, e As FormClosedEventArgs)
            F.NbTuileKMZSuppressed = NbRectanglesNotSelected
            'ainsi que la valeur de IsSelected pour chaque tuile
            F.TuilesKMZIsSelected = RectanglesIsSelected
        End Sub
        ''' <summary>  Affichage sur le formulaire l'ensemble des rectangles qui composent le fichier kmz </summary>
        Private Sub CalepinageKMZ_Paint(sender As Object, e As PaintEventArgs)
            AfficherRectangles(e.Graphics)
        End Sub
        ''' <summary> prend en compte le fait que l'utilisateur appui sur un des boutons de la souris pour débuter une action 
        ''' qui dépend du type de rectangle et de leur utilisation </summary>
        Private Sub CalepinageKMZ_MouseDown(sender As Object, e As MouseEventArgs)
            'on enregistre la position de départ pour replacer le curseur en revenant de la boite de dialogue Image
            PosMouse = Cursor.Position
            Modifier = ModifierKeys
            'cherche si on est au début création de rectangle de travail ou au début click sur un rectangle simple 
            AppuyerBoutonSouris(e.Location, Modifier)
        End Sub
        ''' <summary> gestion du déplacement de la souris sur les rectangles </summary>
        Private Sub CalepinageKMZ_MouseMove(sender As Object, e As MouseEventArgs)
            'trouver le rectangle qui est survolé
            DeplacerSouris(e.Location)
            Select Case NumRectangleSurvol
                Case -1 'pas de rectangle survolé
                    'on efface l'ancien rectangle survolé puisqu'il ne l'est plus
                    If NumForme <> -1 Then Invalidate()
                Case 0 'rectangle de travail survolé
                    'on redessine toujours le rectangle de travail sur un évènement MouseMove (changement de dimensions)
                    Invalidate()
                Case > 0 'tuile survolée
                    If NumForme <> NumRectangleSurvol Then
                        'on affiche à l'écran le nouveau rectangle simple survolé
                        Invalidate()
                    End If
            End Select
            'on garde en mémoire le rectangle survolé
            NumForme = NumRectangleSurvol
        End Sub
        ''' <summary> prend en compte le fait que l'utilisateur relache le bouton de la souris pour finir l'action en cours </summary>
        Private Sub CalepinageKMZ_MouseUp(sender As Object, e As MouseEventArgs)
            RelacherBoutonSouris()
            If ClickRectangle Then
                'maj de l'info sur le formulaire
                Info2.Text = $"Nb Tuiles à supprimer : {NbRectanglesNotSelected}"
                Invalidate()
                'replace le curseur à l'endroit de départ si la boite de dialogue Image a été appelée
                If Modifier = Keys.Alt AndAlso NumForme > 0 Then
                    Cursor.Position = PosMouse
                End If
            End If
        End Sub
        ''' <summary> gère les touches clavier envoyées au formulaire. Les touches escape ou entrée permettent de sortir </summary>
        Private Sub CalepinageRegroupement_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                DialogResult = DialogResult.Cancel
                Close()
            End If
            If e.KeyCode = Keys.Enter Then
                DialogResult = DialogResult.OK
                Close()
            End If
        End Sub
        ''' <summary> affiche l'image de la tuile. Action à passer en propriété aux rectangles simples </summary>
        ''' <param name="Numtuile"> index de la tuile dans le fichier KMZ </param>
        Private Function AfficherImageTuile(NumTuile As Integer) As DialogResult
            Dim NomTuile As String = "files/" & F.TuilesKMZ(NumTuile - 1).Nom
            Using ImageTuile As Image = Image.FromStream(F.ArchiveLectureKMZ.GetEntry(NomTuile).Open())
                Dim LargeurAffichage = ImageTuile.Width + 2
                Dim HauteurAffichage = ImageTuile.Height + 2
                If DimensionsEcranSupport.Width < LargeurAffichage Then LargeurAffichage = DimensionsEcranSupport.Width
                If DimensionsEcranSupport.Height < HauteurAffichage Then HauteurAffichage = DimensionsEcranSupport.Height
                'F est le formulaire parent
                Return AfficherImage(F, ImageTuile, New Size(LargeurAffichage, HauteurAffichage))
            End Using
        End Function
        ''' <summary> détermine les rectangles qui composent la représentation des tuiles du fichier KMZ </summary>
        Private Sub FaireRectanglesTuiles()
            Dim DimensionsPlanTuiles As New Size(ClientSize.Width - Affiche_Bords * 2, ClientSize.Height - Affiche_Bords * 2 - Info.Height)
            'récupération de la surface de l'ensemble des tuiles du ficheir KMZ et calcul du coefficient de mise à l'échelle dessin/regroupement
            Dim DimensionsTuiles = F.DescriptionFichierKMZ.TaillePixels
            Dim R_X = DimensionsPlanTuiles.Width / DimensionsTuiles.Width
            Dim R_Y = DimensionsPlanTuiles.Height / DimensionsTuiles.Height
            'on garde les proportions des tuiles
            If R_X > R_Y Then
                CoefR = R_Y
            Else
                CoefR = R_X
            End If
            'calcul des X et Y d'origine en pixels de la zone de dessin en prenant les dimensions pixels des rectangles et pas de la carte
            'pour éviter les erreurs d'arrondi
            Dim XPixels = CInt(F.TuilesKMZ(0).LargeurPixels * CoefR) * (F.NbColsKMZ - 1) +
                          CInt((F.DescriptionFichierKMZ.LargeurPixels - (F.NbColsKMZ - 1) * F.TuilesKMZ(0).LargeurPixels) * CoefR)
            Dim YPixels = CInt(F.TuilesKMZ(0).HauteurPixels * CoefR) * (F.NbRangsKMZ - 1) +
                          CInt((F.DescriptionFichierKMZ.HauteurPixels - (F.NbRangsKMZ - 1) * F.TuilesKMZ(0).HauteurPixels) * CoefR)
            Xorigine = (DimensionsPlanTuiles.Width - XPixels) \ 2 + Affiche_Bords
            Yorigine = (DimensionsPlanTuiles.Height - YPixels) \ 2 + Affiche_Bords
            'si c'est la 1ère fois que l'on ouvre le fichier KMZ on lance le calcul et la création des tuiles sous forme de rectangle simples
            If F.FlagInitCalepinage Then
                'définit l'action à réaliser pour un click sur un rectangle simple
                ActionRectangleSimple = AddressOf AfficherImageTuile
                'initialisation de la classe rectangle_base avec la possibilité d'avoir un rectangle de travail
                'Le +3, -6 correspond à la largeur minimum du rectangle de séléction
                Dim SurfaceDessin As New Rectangle(New Point(ClientRectangle.X + 3, ClientRectangle.Y + 3),
                                                   New Size(ClientRectangle.Width - 6, ClientRectangle.Height - Info.Height - 6))
                InitialiserRectangles(SurfaceDessin, Mode.Selection, Rectangle.Empty, True)
                Dim IndexTuileKMZ As Integer = 0
                Dim Y = Yorigine
                For Row As Integer = 0 To F.NbRangsKMZ - 1
                    Dim X As Integer = Xorigine
                    Dim H As Integer = 0
                    Dim L As Integer = 0
                    For Col As Integer = 0 To F.NbColsKMZ - 1
                        Dim Name As String = F.TuilesKMZ(IndexTuileKMZ).Nom
                        'calcul des dimensions de la tuile en pixels de la zone de dessin
                        Dim DimensionsTuileKMZ As Size = F.TuilesKMZ(IndexTuileKMZ).TaillePixels
                        'la largeur est à calculer pour la 1ere et la dernière colonne
                        If Col = 0 OrElse Col = F.NbColsKMZ - 1 Then L = CInt(DimensionsTuileKMZ.Width * CoefR)
                        'la hauteur est à calculer qu'une seule fois à la 1ere colonne
                        If Col = 0 Then H = CInt(DimensionsTuileKMZ.Height * CoefR)
                        'ajout dans la collection du rectangle représentant la tuile. Au départ ils sont tous sélectionnés
                        AjouterRectangle(New Point(X, Y), New Point(X + L, Y + H), Name, True)
                        IndexTuileKMZ += 1
                        X += L
                    Next
                    Y += H
                Next
            End If
        End Sub
    End Class
    Partial Private Class CalepinageKMZ
        Inherits Form
        Friend Sub New()
            InitialiseComposants()
            InitialiserEvenements()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Requise par le Concepteur Windows Form
        Private components As IContainer
        Private Sub InitialiserEvenements()
            AddHandler Load, AddressOf CalepinageKMZ_Load
            AddHandler FormClosed, AddressOf CalepinageKMZ_FormClosed
            AddHandler Paint, AddressOf CalepinageKMZ_Paint
            AddHandler MouseDown, AddressOf CalepinageKMZ_MouseDown
            AddHandler MouseMove, AddressOf CalepinageKMZ_MouseMove
            AddHandler MouseUp, AddressOf CalepinageKMZ_MouseUp
            AddHandler KeyDown, AddressOf CalepinageRegroupement_KeyDown
        End Sub
        Private Sub InitialiseComposants()
            components = New Container()
            Info1 = New Label()
            Info2 = New Label()
            Info = New Label()
            SuspendLayout()
            '
            'Info1
            '
            Info1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            Info1.BorderStyle = BorderStyle.FixedSingle
            Info1.FlatStyle = FlatStyle.Flat
            Info1.Location = New Point(-1, 701)
            Info1.Name = "Info1"
            Info1.Size = New Size(127, 23)
            Info1.TabIndex = 0
            Info1.Text = "Nb Tuiles : 1000"
            Info1.TextAlign = ContentAlignment.MiddleCenter
            '
            'Info2
            '
            Info2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            Info2.BorderStyle = BorderStyle.FixedSingle
            Info2.FlatStyle = FlatStyle.Flat
            Info2.Location = New Point(125, 701)
            Info2.Name = "Info2"
            Info2.Size = New Size(201, 23)
            Info2.TabIndex = 1
            Info2.Text = "Nb Tuiles à supprimer : 1000"
            Info2.TextAlign = ContentAlignment.MiddleCenter
            '
            'Info
            '
            Info.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right Or AnchorStyles.Left
            Info.BorderStyle = BorderStyle.FixedSingle
            Info.FlatStyle = FlatStyle.Flat
            Info.Font = New Font("Segoe UI", 14.0!, FontStyle.Bold, GraphicsUnit.Pixel)
            Info.Location = New Point(325, 701)
            Info.Name = "Info"
            Info.Size = New Size(376, 23)
            Info.TabIndex = 2
            Info.Text = "Esc ou Enter pour Fermer"
            Info.TextAlign = ContentAlignment.MiddleRight
            '
            'CalepinageKMZ
            '
            AutoScaleMode = AutoScaleMode.None
            AutoSize = False
            AutoSizeMode = AutoSizeMode.GrowOnly
            ClientSize = New Size(700, 723)
            ControlBox = False
            Controls.Add(Info1)
            Controls.Add(Info2)
            Controls.Add(Info)
            Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
            FormBorderStyle = FormBorderStyle.FixedToolWindow
            MaximizeBox = False
            MinimizeBox = False
            Name = "CalepinageKMZ"
            SizeGripStyle = SizeGripStyle.Hide
            StartPosition = FormStartPosition.Manual
            DoubleBuffered = True
            ResumeLayout(False)
        End Sub

        Private Info As Label
        Private Info1 As Label
        Private Info2 As Label
    End Class
#End Region
End Class