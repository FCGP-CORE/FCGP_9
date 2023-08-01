Imports System.Drawing.Printing
Imports System.Drawing.Text
''' <summary> pour imprimer 1 carte géoreférencée. pas d'impression simultanée </summary>
Module ImprimerCarte
#Region "Document Page Impression"
    ''' <summary> 8 valeurs en Mm converties en pixels pour éviter des calculs redondants lors du du dessin des pages </summary>
    Private PixelsRecouvreX, PixelsRecouvreY, PixelsLargeurImprimable, PixelsHauteurImprimable As Integer
    Private PixelsCentrageXDeb, PixelsCentrageYDeb, PixelsCentrageXFin, PixelsCentrageYFin As Integer
    ''' <summary> image de la carte à imprimer </summary>
    Private ImageCarte As Image
    ''' <summary> valeurs réeeles du recouvrement entre 2 pages sur les 2 axes (en Mm) </summary>
    Private RecouvreX, RecouvreY As Single
    ''' <summary> echelle d'impression réelle de la carte  </summary>
    Private FPoint As Font
    ''' <summary> différence entre la hauteur de ligne de la font et la hauteur de la font </summary>
    Private DeltaYFontPage As Single
    ''' <summary> texte invariant de l'impression des coordonnées </summary>
    Private TexteInvariant As String
    ''' <summary> valeur de la marge pour l'impression des coordonnées de la page si demandé (en mm) </summary>
    Private Const MargeCoordonnees As Integer = 3
#End Region
#Region "ValeurImpression : Commune module et IU"
#Region "Variables issues du Fichier Georef"
    ''' <summary> Chemin du fichier georef et du ou des fichiers images associés (carte et ou références coordonnées) </summary>
    Private CheminFichierGeoref As String
    ''' <summary> Nom du fichier image à imprimer </summary>
    Private NomCarte As String
    ''' <summary> dimension en pixels de la carte </summary>
    Private PixelGeoref As Size
    ''' <summary> dimensions réelles de la carte exprimées en millimètres. </summary>
    Private MmGeoref As SizeF
    ''' <summary> coordonnées virtuelles du Pt0 de la carte (pixels). </summary>
    Private Pt0Pixels As Point
    ''' <summary> site carto de la carte </summary>
    Private SiteCartoCarte As SitesCartographiques
    ''' <summary> site carto de la carte </summary>
    Private EchelleCarte As Echelles
#End Region
#Region "Document Impression"
    ''' <summary> imprimante pour le document </summary>
    Private NomImprimante As String
    ''' <summary> taille du papier sélectionné pour le document sans tenir compte de l'orientation (portrait ou paysage) </summary>
    Private Papier As PaperSize
    ''' <summary> résolution sélectionnée pour le document </summary>
    Private Resolution As PrinterResolution
    ''' <summary> valeur de la marge des bords Top, Bottom, Left et Right qui ne sera pas imprimée (en mm) hors valeur minimale due au matériel </summary>
    Private Bords As Integer
    ''' <summary> valeur du recouvrement entre 2 feuilles (en mm) </summary>
    Private ReCouvre As Integer
    ''' <summary> indique si les coordonnées du PT0 des pages sont immprimées </summary>
    Private IsCoordonnees As Boolean
    ''' <summary> orientation des pages : Paysage si true sinon portrait </summary>
    Private IsPaysage As Boolean
    ''' <summary> indique si la carte est imprimée en couleur ou en niveau de gris </summary>
    Private IsColor As Boolean
    ''' <summary> indique si l'imprimante peut imprimer en recto verso </summary>
    Private IsRectoVerso As Boolean
    ''' <summary> largeur et hauteur imprimable d'une page prenant en compte l'orientation (portrait ou paysage) </summary>
    Private LargeurPageImprimable, HauteurPageImprimable As Single
    ''' <summary> dimensions de la surface de la carte 1 fois imprimée (en mm) </summary>
    Private DimensionsCarteImprimable As SizeF
    ''' <summary> concerne les pages à imprimer  </summary>
    Private NbPagesX, NbPagesY As Integer
    ''' <summary> echelle d'impression réelle de la carte  </summary>
    Private EchelleImpression As String
    ''' <summary> valeur des décalages pour centrer ou pas l'image de la carte sur l'ensemble des pages à imprimer (en mm) </summary>
    Private CentrageImprXDeb, CentrageImprXFin, CentrageImprYDeb, CentrageImprYFin As Single
    ''' <summary> Nb de pages à imprimer et Nb de pages déjà imprimées </summary>
    Private NbPagesTotal, NbPagesImprimees As Integer
    ''' <summary> liste des pages à imprimer </summary>
    Private ListePages() As Integer
#End Region
#End Region
    ''' <summary> indique si une impression est en cours </summary>
    Friend ReadOnly Property IsPrinting As Boolean
    ''' <summary> méthode à appeler pour imprimer une carte. 1 seule impression encours </summary>
    Friend Async Sub LancerImpressionCarteAsync()
        Using R As New ChoixImpressionCarte
            If R.ShowDialog(FormApplication) = DialogResult.OK Then
                _IsPrinting = True
                Await Task.Run(Sub() Imprimer())
                MessageInformation = $"La carte {NomCarte} a été imprimée"
                TitreInformation = "Information"
                AfficherInformation()
                _IsPrinting = False
            End If
        End Using
    End Sub
    ''' <summary> arrondi une valeur théorique pour que celle ci soit un multiple d'une valeur entière (physique) </summary>
    ''' <param name="Valeur"> valeur théoriquee </param>
    ''' <param name="Multiple"> multiple </param>
    ''' <returns> valeur entière (physique) </returns>
    Private Function ArrondirValeur(ByRef Valeur As Single, Multiple As Single) As Integer
        Dim Resultats As Integer = CInt(Math.Round(Valeur * Multiple, 0))
        Valeur = Resultats / Multiple
        Return Resultats
    End Function
    ''' <summary> Imprime la carte avec les choix de l'utilisateur </summary>
    Private Sub Imprimer()
        NbPagesImprimees = 0
        'on transforme les Mm théoriques de l'impression en pixels d'éviter des calculs redondants lors de l'impression
        'mais les pixels sont des entiers. On réajuste les valeurs théoriques des dimensions en Mm pour correspondre à un nombre entier de pixels
        'Coef X d'échelle Mm imprimé en Pixel image carte
        Dim MmToPixelX = PixelGeoref.Width / DimensionsCarteImprimable.Width
        RecouvreX = ReCouvre
        PixelsRecouvreX = ArrondirValeur(RecouvreX, MmToPixelX)
        PixelsLargeurImprimable = ArrondirValeur(LargeurPageImprimable, MmToPixelX)
        PixelsCentrageXDeb = ArrondirValeur(CentrageImprXDeb, MmToPixelX)
        'on calcule le nb de pixels nécessaire au centrage du fin de page
        PixelsCentrageXFin = NbPagesX * PixelsLargeurImprimable + PixelsRecouvreX - PixelGeoref.Width - PixelsCentrageXDeb
        'on affecte les erreurs dues aux arrondis au décalage de fin
        CentrageImprXFin = PixelsCentrageXFin / MmToPixelX
        'Normalement ce sont des pixels carrés (x=Y) sauf pour les cartes interpolées suisse
        'Coef Y d'échelle Mm imprimé en Pixel image carte
        Dim MmToPixelY = PixelGeoref.Height / DimensionsCarteImprimable.Height
        RecouvreY = ReCouvre
        PixelsRecouvreY = ArrondirValeur(RecouvreY, MmToPixelY)
        PixelsHauteurImprimable = ArrondirValeur(HauteurPageImprimable, MmToPixelY)
        PixelsCentrageYDeb = ArrondirValeur(CentrageImprYDeb, MmToPixelY)
        'on calcule le nb de pixels nécessaire au centrage de fin de page
        PixelsCentrageYFin = NbPagesY * PixelsHauteurImprimable + PixelsRecouvreY - PixelGeoref.Height - PixelsCentrageYDeb
        'on affecte les erreurs dues aux arrondis au décalage de fin
        CentrageImprYFin = PixelsCentrageYFin / MmToPixelY
        'configuration du document. Toutes les pages ont la même configuration
        Using Carte = New PrintDocument()
            AddHandler Carte.PrintPage, AddressOf Carte_PrintPage
            Carte.DocumentName = NomCarte
            Carte.PrinterSettings.PrinterName = NomImprimante
            Carte.PrinterSettings.Copies = 1
            'on imprime pas en rectoverso
            If IsRectoVerso Then Carte.PrinterSettings.Duplex = Duplex.Simplex
            Carte.DefaultPageSettings.Landscape = IsPaysage
            Carte.DefaultPageSettings.PaperSize = Papier
            Carte.DefaultPageSettings.Color = IsColor
            Carte.DefaultPageSettings.PrinterResolution = Resolution
            'on indique une sélection automatique de la source de papier par l'imprimante
            Carte.DefaultPageSettings.PaperSource = New PaperSource() With {.SourceName = "Sélection automatique du magasin", .RawKind = 7}
            'on ouvre l'image ici pour éviter de le faire lors de chaque impression de page
            ImageCarte = Image.FromFile(CheminFichierGeoref & "\" & NomCarte)
            Carte.Print()
            ImageCarte.Dispose()
        End Using
    End Sub
    ''' <summary> imprime une page sur l'imprimante sélectionnée à la demande du document. Chaque page ne dépend que du nb de pages déjà imprimées
    ''' on pourrait donc facilement n'imprimer qu'une plage de pages au lieu de la totalité </summary>
    Private Sub Carte_PrintPage(sender As Object, e As PrintPageEventArgs)
        'retrouve la colonne et le rang de la page à imprimer
        Dim NumPage = ListePages(NbPagesImprimees)
        Dim XImpr As Integer
        Dim YImpr As Integer = Math.DivRem(NumPage, NbPagesX, XImpr)
        Using g As Graphics = e.Graphics
            g.PageUnit = GraphicsUnit.Millimeter 'on travaille en millimètre
            g.PageScale = 1 'sans mise à l'échelle particulière
            'valeur en MM du décalage de l'image de la carte par rapport au coin haut gauche de la page en incluant les bords non imprimables.
            'Les pages sur les bords ont un delta qui sert à centrer l'impression de la carte
            Dim MmX As Single = If(XImpr = 0, CentrageImprXDeb, 0) + Bords
            Dim MmY As Single = If(YImpr = 0, CentrageImprYDeb, 0) + Bords + If(IsCoordonnees, MargeCoordonnees, 0)
            'valeur en MM de la largeur et hauteur de la partie de l'image de la carte correspondant à la page actuelle.
            'Les pages sur le bord ont un delta qui sert à centrer l'impression de la carte
            Dim MmL As Single = LargeurPageImprimable + RecouvreX - If(XImpr = 0, CentrageImprXDeb, 0) - If(XImpr = NbPagesX - 1, CentrageImprXFin, 0)
            Dim MmH As Single = HauteurPageImprimable + RecouvreY - If(YImpr = 0, CentrageImprYDeb, 0) - If(YImpr = NbPagesY - 1, CentrageImprYFin, 0)
            'valeur en pixels du décalage de la partie de l'image de la carte
            Dim PixelsX = If(XImpr = 0, 0, PixelsLargeurImprimable * XImpr - PixelsCentrageXDeb)
            Dim PixelsY = If(YImpr = 0, 0, PixelsHauteurImprimable * YImpr - PixelsCentrageYDeb)
            'valeur en pixels de la largeur et de la hauteur de la partie de l'image de la carte correspondant à la page actuelle.
            Dim PixelsL = PixelsLargeurImprimable + PixelsRecouvreX - If(XImpr = 0, PixelsCentrageXDeb, 0) - If(XImpr = NbPagesX - 1, PixelsCentrageXFin, 0)
            Dim PixelsH = PixelsHauteurImprimable + PixelsRecouvreY - If(YImpr = 0, PixelsCentrageYDeb, 0) - If(YImpr = NbPagesY - 1, PixelsCentrageYFin, 0)
            'envoi de la partie de la carte correspondant à la page actuelle à l'imprimante                                   
            g.DrawImage(ImageCarte,
                        New RectangleF(New PointF(MmX, MmY), New SizeF(MmL, MmH)),              'le rectangle destination est exprimé en Mm sur la page
                        New Rectangle(New Point(PixelsX, PixelsY), New Size(PixelsL, PixelsH)), 'le rectangle source est exprimé en pixels. Il s'agit de tout ou partie 
                        GraphicsUnit.Pixel)                                                     'de l'image de la carte afin de respecter l'échelle ou le nb de pages

            If IsCoordonnees Then
                g.TextRenderingHint = TextRenderingHint.AntiAlias
                If NbPagesImprimees = 0 Then
                    FPoint = New Font("Segoe UI", 72.0F / 25.4F * MargeCoordonnees)
                    DeltaYFontPage = FPoint.GetHeight(g) - MargeCoordonnees
                    TexteInvariant = $", Echelle {EchelleImpression}, {NomCarte}"
                End If
                Dim T = $"Page N°{NumPage + 1}, {CoordonneesPage(PixelsX, PixelsY)}{TexteInvariant}"
                g.DrawString(T, FPoint, Brushes.Black, Bords, Bords - DeltaYFontPage)
            End If
        End Using
        'on indique que la page a été imprimée
        NbPagesImprimees += 1
        'et on continue tant que toutes les pages n'ont pas été imprimées
        e.HasMorePages = NbPagesImprimees < NbPagesTotal
    End Sub
    ''' <summary> transforme les coordonnées virtuelles du point Pt0 de la page en coordonnées Grille Suisse ou UTM </summary>
    ''' <param name="PixelsX"> Coordonnée X du point PT0 </param>
    ''' <param name="PixelsY"> Coordonnée Y du point PT0 </param>
    Private Function CoordonneesPage(PixelsX As Integer, PixelsY As Integer) As String
        Dim Pt0Page = New Point(Pt0Pixels.X + PixelsX, Pt0Pixels.Y + PixelsY)
        Dim Pt0 As PointD, Ret As String
        If SiteCartoCarte = SitesCartographiques.SuisseMobile Then
            Pt0 = PointPixelToPointGrille(Pt0Page, SiteCartoCarte, EchelleCarte)
            Ret = ConvertPointXYtoChaine(New PointProjection(Pt0))
        Else
            Pt0 = PointPixelsToPointDD(Pt0Page, SiteCartoCarte, EchelleCarte)
            Ret = ConvertPointDDtoUTM(Pt0)
        End If
        Return Ret
    End Function

    ''' <summary> Formulaire concernant les choix de la carte à imprimer </summary>
    Private Class ChoixImpressionCarte
        Inherits Form
        ''' <summary> Portrait indique que l'on prend pour les dimensions de la page imprimables largeur * Hauteur des dimensions de la page
        ''' Paysage indique que l'on prend pour les dimensions de la page imprimables Hauteur * largeur des dimensions de la page </summary>
        Private Enum OrientationPapier As Integer
            Portrait = 0 : Paysage
        End Enum
        ''' <summary> Vertical indique que l'on prend pour les dimensions imprimables nbCols * nbRangs
        ''' Horizontal indique que l'on prend pour les dimensions imprimables nbRangs * nbCols </summary>
        Private Enum OrientionCarte As Integer
            Vertical = 0 : Horizontal
        End Enum
#Region "Plan Impression"
        ''' <summary> pinceau  pour le dessin de la carte </summary>
        Private ReadOnly Pen_Carte As New Pen(Color.Blue, 1)
        ''' <summary> brosse semi transparente pour le dessin de la carte </summary>
        Private ReadOnly Brosse_Carte As New SolidBrush(Color.FromArgb(64, 0, 0, 64))
        ''' <summary> pour la sélection des pages à imprimer </summary>
        Private Modifier As Keys
        ''' <summary> Retour de la visualisation d'une page à imprimer </summary>
        Private PosMouse As Point
        ''' <summary> numéro de la page à visualiser </summary>
        Private NumPage As Integer
        ''' <summary> dimensions des pages à imprimer sur la surface d'affichage du plan </summary>
        Private RegionPagesPlan As Rectangle
        ''' <summary> rapport Mm et pixel de la region d'impression du plan </summary>
        Private CoefR As Single
        ''' <summary> valeur en pixel de la marge des bords </summary>
        Private Marge As Integer
        ''' <summary> valeur en pixel de la marge des coordonnées </summary>
        Private Coord As Integer
        ''' <summary> dimensions de la surface d'affichage du plan </summary>
        Private TailleImpressionPlan As Size
        ''' <summary> dimensions maximales pour le rectangle de travail (sélection) </summary>
        Private SurfaceRectangleTravail As Rectangle
        ''' <summary> taille de la carte en pixels du plan </summary>
        Private TailleCartePlan As Size
        ''' <summary> btmap de la carte pour pouvoir afficher une page en particulier </summary>
        Private BitMapCarte As Bitmap
        ''' <summary> valeur associé à l'impression des coordonnées sur la page. 0 si pas demandé ou MargeCoordonnées (en mm) </summary>
        Private BordCoordonnees As Integer
        ''' <summary> indique si la carte est centrée sur la surface imprimable </summary>
        Private IsCentrage As Boolean
        ''' <summary> dimensions réelles de la page de papier choisit (en mm) </summary>
        Private DimensionsPapier As SizeF
        ''' <summary> dimensions théoriques d'une page de papier une fois enlever les bords et le recouvrement 
        ''' sans présager de l'orientation et de la marge des coordonnées (en mm) </summary>
        Private DimensionsPageImprimable As SizeF
        ''' <summary> dimensions théoriques de la surface imprimable représentée par l'ensemble des pages à imprimer et de leur orientation (en mm) </summary>
        Private DimensionsPagesImprimable As SizeF
        ''' <summary> valeur de toutes les marges du sens vertical soit 2 bords et un recouvrement (en mm) </summary>
        Private BordsXPageImprimable As Integer
        ''' <summary> valeur de toutes les marges du sens horizontal soit 2 bords, un recouvrement et une coordonnées (en mm) </summary>
        Private BordsYPageImprimable As Integer
        ''' <summary> rapport larg / haut de la carte sur les dimensions en Mm </summary>
        Private RapCarteMm As Single
        ''' <summary> 1/echelle d'impression du fichier georef de la carte par exemple 25000 pour 1/25000 </summary>
        Private EchelleImpressionGeoref As Single
        ''' <summary> valeur de non dessin entre la surface d'affichage du plan et celle du controle support (en pixels) </summary>
        Const AffichageBords As Integer = 15
#End Region
#Region "Imprimante"
        ''' <summary> liste des imprimantes acceptées par l'application: objets </summary>
        Private NomsImprimantes As List(Of String)
        ''' <summary> marge minimum du aux contraintes matérielles d'une imprimante donnée (en mm) </summary>
        Private BordsMini As Integer
        ''' <summary> liste des papiers acceptés pour une imprimante donnée : noms pour liste de choix </summary>
        Private PapiersNoms As List(Of String)
        ''' <summary> liste des papiers acceptés pour une imprimante donnée : objets </summary>
        Private Papiers As List(Of PaperSize)
        ''' <summary> liste des resolutions d'impression acceptées par une imprimante donnée : noms pour liste de choix </summary>
        Private ResolutionsNoms As List(Of String)
        ''' <summary> Liste des résolutions d'impression acceptées par une imprimante donnée : objets </summary>
        Private Resolutions As List(Of PrinterResolution)
        ''' <summary> flag indiquant si une imprimante donné supporte le mode paysage </summary>
        Private IsSupportPaysage As Boolean
        ''' <summary> flag indiquant si une imprimante donné supporte le mode couleur </summary>
        Private IsSupportColor As Boolean
#End Region
        Private Sub ChoixImpressionCarte_Load(sender As Object, e As EventArgs)
            'donne l'imprimante par défaut du système
            Dim Imprimante = New PrinterSettings()
            'et son nom
            Dim NomImprimante = Imprimante.PrinterName
            Dim IndexImprimante As Integer = -1
            TitreInformation = "Information FCGP_Partager"
            OuvrirFichierGeoref.Filter = "Georef Files|*.georef"
            OuvrirFichierGeoref.Title = "Choix du fichier Georef"
            OuvrirFichierGeoref.InitialDirectory = CapturerSettings.CHEMIN_CARTE
            'remplit la liste de choix des imprimantes acceptées du système 
            NomsImprimantes = New List(Of String)
            For Each Nom As String In PrinterSettings.InstalledPrinters
                'on ajoute les imprimantes physiques et virtuelles XPS ou PDF mais pas les traceurs
                If Not (Nom.StartsWith("One") OrElse Nom.StartsWith("Fax") OrElse Imprimante.IsPlotter) AndAlso Imprimante.IsValid Then
                    If NomImprimante = Nom Then IndexImprimante = NomsImprimantes.Count
                    NomsImprimantes.Add(Nom)
                End If
            Next
            ListeChoixImprimante.Items.AddRange(NomsImprimantes.ToArray)
            ListeChoixImprimante.SelectedIndex = IndexImprimante

            'valeurs constantes liées au contrôle d'affichage du plan de mapping des pages à imprimer
            TailleImpressionPlan = New Size(PlanImpression.ClientSize.Width - AffichageBords * 2,   'taille en pixels
                                            PlanImpression.ClientSize.Height - AffichageBords * 2)
            SurfaceRectangleTravail = New Rectangle(New Point(3, 3), New Size(PlanImpression.ClientSize.Width - 6, PlanImpression.ClientRectangle.Height - 6))
            'détermine la procédure à appeler lors d'un click sur une page du plan avec le modificateur Alt
            ActionRectangleSimple = AddressOf AfficherImagePage
            IsCentrage = ChoixCentrage.Checked
            IsCoordonnees = ChoixCoordonnees.Checked
        End Sub
        Private Sub ChoixImpressionCarte_FormClosing(sender As Object, e As FormClosingEventArgs)
            If DialogResult = DialogResult.OK Then
                TitreInformation = "Information Impression Carte"
                If NbRectanglesSelected = 0 Then
                    MessageInformation = $"Il n'y a plus de page à imprimer"
                    e.Cancel = True
                    Exit Sub
                End If
                MessageInformation = $"Imprimer la carte avec ces choix ?"
                If AfficherConfirmation() = DialogResult.Cancel Then
                    e.Cancel = True
                End If
            End If
        End Sub
        Private Sub ChoixImpressionCarte_FormClosed(sender As Object, e As FormClosedEventArgs)
            If BitMapCarte IsNot Nothing Then
                BitMapCarte.Dispose()
                BitMapCarte = Nothing
            End If
            If DialogResult = DialogResult.OK Then
                'on indique les bonnes valeurs de centrage en fonction du choix final de l'utilisateur
                If Not IsCentrage Then
                    CentrageImprXFin += CentrageImprXDeb
                    CentrageImprXDeb = 0
                    CentrageImprYFin += CentrageImprYDeb
                    CentrageImprYDeb = 0
                End If
                'on enlève la marge inhérente du matériel qui sera ajoutée automatiquement par celui-ci
                Bords -= BordsMini
                'on indique les pages à imprimer
                NbPagesTotal = NbRectanglesSelected
                ListePages = RectanglesSelected
                'on indique les coordonnées de la carte
                TrouverPt0Pixels()
                'et tous les renseignements de l'imprimante non liés directement au calcul de l'impression
                NomImprimante = ListeChoixImprimante.Text
                Resolution = Resolutions(ListeChoixResolution.SelectedIndex)
                IsColor = ListeChoixCouleur.SelectedIndex = 0
            End If
        End Sub
        ''' <summary> gestion du choix du fichier Georef </summary>
        Private Sub LabelChoixFichierGeoref_Click(sender As Object, e As EventArgs)
            'Affiche la boite de dialogue de sélection des fichiers georef
            If OuvrirFichierGeoref.ShowDialog() = DialogResult.OK Then
                Dim Ret = LireFichierGeoref()
                If Ret <> VerificationRenvoiCarte.OK Then
                    MessageInformation = "Problème avec le fichier Georef : " & OuvrirFichierGeoref.FileName & CrLf & Ret.ToString
                    AfficherInformation()
                Else
                    If BitMapCarte IsNot Nothing Then
                        BitMapCarte.Dispose()
                        BitMapCarte = Nothing
                    End If
                    OuvrirFichierGeoref.InitialDirectory = CheminFichierGeoref
                    BitMapCarte = CType(Image.FromFile(CheminFichierGeoref & "\" & NomCarte), Bitmap)
                    LabelChoixFichierGeoref.Text = NomCarte
                    btnYES.Enabled = True
                    btnYES.Focus()
                    CalculerImpression()
                End If
            End If
        End Sub
        ''' <summary> Met à jour le label de l'échelle en fonction de la valeur choisie 
        ''' avec éventuellement le lancement des calculs liés à l'impression </summary>
        Private Sub ValeurEchelle_ValueChanged(sender As Object, e As EventArgs)
            If ChoixEchelle.Checked AndAlso btnYES.Enabled Then CalculerImpression()
        End Sub
        Private Sub ValeurNbPages_ValueChanged(sender As Object, e As EventArgs)
            If ChoixNbPages.Checked AndAlso btnYES.Enabled Then CalculerImpression()
        End Sub
        ''' <summary> Lancement des calculs liés à l'impression </summary>
        Private Sub ValeurMarges_ValueChanged(sender As Object, e As EventArgs)
            If btnYES.Enabled Then CalculerImpression()
        End Sub
        ''' <summary> Demande un nouvel affichage du plan de mapping </summary>
        Private Sub ChoixCentrage_CheckedChanged(sender As Object, e As EventArgs)
            IsCentrage = ChoixCentrage.Checked
            If btnYES.Enabled Then PlanImpression.Invalidate()
        End Sub
        ''' <summary> Lancement des calculs liés à l'impression </summary>
        Private Sub ChoixCoordonnees_CheckedChanged(sender As Object, e As EventArgs)
            IsCoordonnees = ChoixCoordonnees.Checked
            If btnYES.Enabled Then CalculerImpression()
        End Sub
        ''' <summary> ouvre les listes déroulantes </summary>
        Private Sub Labels_Click(sender As Object, e As EventArgs)
            Dim L As Label = CType(sender, Label), C As ComboBox = CType(L.Tag, ComboBox)
            If L.Name = "LabelChoixCouleur" AndAlso Not IsSupportColor Then Exit Sub
            C.DroppedDown = True
        End Sub
        ''' <summary> Met à jour le texte du label associé à la liste </summary>
        Private Sub Listes_SelectedIndexChanged(sender As Object, e As EventArgs)
            Dim C As ComboBox = CType(sender, ComboBox), L As Label = CType(C.Tag, Label)
            L.Text = C.SelectedItem.ToString()
            If C.Name = "ListeChoixImprimante" Then ChoisirImprimante()
            If C.Name = "ListeChoixPapier" AndAlso btnYES.Enabled Then CalculerImpression()
        End Sub
        ''' <summary> lance le dessin du plan impression. Normalement on pourrait appelr directement la procédure sous-jacente
        ''' mais ne donne pas de bon résultats sur la 1ère miseà jour d'une numericupdown </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub PlanImpression_Paint(sender As Object, e As PaintEventArgs)
            If Not btnYES.Enabled Then Exit Sub
            Dim G = e.Graphics
            'afficher les rectangles représeantant les pages à imprimer
            AfficherRectangles(G)
            'afficher la carte à imprimer
            Dim R = RegionCartePlan()
            G.FillRectangle(Brosse_Carte, R)
            G.DrawRectangle(Pen_Carte, R)
        End Sub
        ''' <summary> prend en compte le fait que l'utilisateur appui sur un des boutons de la souris pour débuter une action 
        ''' qui dépend du type de rectangle et de leur utilisation </summary>
        Private Sub PlanImpression_MouseDown(sender As Object, e As MouseEventArgs)
            If Not btnYES.Enabled Then Exit Sub
            'on enregistre la position de départ pour replacer le curseur en revenant de la boite de dialogue Image
            PosMouse = Cursor.Position
            Modifier = ModifierKeys
            'cherche si on est au début création de rectangle de travail ou au début click sur un rectangle simple 
            AppuyerBoutonSouris(e.Location, Modifier)
        End Sub
        ''' <summary> gestion du déplacement de la souris sur les rectangles </summary>
        Private Sub PlanImpression_MouseMove(sender As Object, e As MouseEventArgs)
            If Not btnYES.Enabled Then Exit Sub
            'trouver le rectangle qui est survolé
            DeplacerSouris(e.Location)
            Select Case NumRectangleSurvol
                Case -1 'pas de rectangle survolé
                    'on efface l'ancien rectangle survolé puisqu'il ne l'est plus
                    If NumPage <> -1 Then PlanImpression.Invalidate()
                Case 0 'rectangle de travail survolé
                    'on redessine toujours le rectangle de travail sur un évènement MouseMove (changement de dimensions)
                    PlanImpression.Invalidate()
                Case > 0 'tuile survolée
                    If NumPage <> NumRectangleSurvol Then
                        'on affiche à l'écran le nouveau rectangle simple survolé
                        PlanImpression.Invalidate()
                    End If
            End Select
            'on garde en mémoire le rectangle survolé
            NumPage = NumRectangleSurvol
        End Sub
        ''' <summary> prend en compte le fait que l'utilisateur relache le bouton de la souris pour finir l'action en cours </summary>
        Private Sub PlanImpression_MouseUp(sender As Object, e As MouseEventArgs)
            If Not btnYES.Enabled Then Exit Sub
            RelacherBoutonSouris()
            If ClickRectangle Then
                PlanImpression.Invalidate()
                'replace le curseur à l'endroit de départ si la boite de dialogue Image a été appelée
                If Modifier = Keys.Alt AndAlso NumPage > 0 Then
                    Cursor.Position = PosMouse
                End If
            End If
        End Sub
        ''' <summary> calcule l'image de la page à imprimer et l'affiche </summary>
        Private Function AfficherImagePage(NumPage As Integer) As DialogResult
            'retrouve la colonne et le rang de la page à imprimer
            Dim XImpr As Integer
            Dim YImpr As Integer = Math.DivRem(NumPage - 1, NbPagesX, XImpr)
            'facteur d'échelle pixels virtuel en Mm à imprimer sur l'axe des X
            Dim MmToPixelX = PixelGeoref.Width / DimensionsCarteImprimable.Width
            RecouvreX = ReCouvre
            PixelsRecouvreX = CInt(Math.Round(RecouvreX * MmToPixelX, 0))
            PixelsLargeurImprimable = CInt(Math.Round(LargeurPageImprimable * MmToPixelX, 0))
            PixelsCentrageXDeb = If(ChoixCentrage.Checked, CInt(Math.Round(CentrageImprXDeb * MmToPixelX, 0)), 0)
            'on calcule le nb de pixels nécessaire au centrage du fin de page
            PixelsCentrageXFin = NbPagesX * PixelsLargeurImprimable + PixelsRecouvreX - PixelGeoref.Width - PixelsCentrageXDeb

            'puis les valeurs pixels sur l'axe des y en mettant à jour les valeurs Mm théorique.  
            'Normalement ce sont des pixels carrés (x=Y) sauf pour les cartes interpolées suisse
            'facteur d'échelle pixels virtuel en Mm à imprimer sur l'axe des Y
            Dim MmToPixelY = PixelGeoref.Height / DimensionsCarteImprimable.Height
            RecouvreY = ReCouvre
            PixelsRecouvreY = CInt(Math.Round(RecouvreY * MmToPixelY, 0))
            PixelsHauteurImprimable = CInt(Math.Round(HauteurPageImprimable * MmToPixelY, 0))
            PixelsCentrageYDeb = If(ChoixCentrage.Checked, CInt(Math.Round(CentrageImprYDeb * MmToPixelY, 0)), 0)
            'on calcule le nb de pixels nécessaire au centrage de fin de page
            PixelsCentrageYFin = NbPagesY * PixelsHauteurImprimable + PixelsRecouvreY - PixelGeoref.Height - PixelsCentrageYDeb

            'valeur en pixels du décalage de la partie de l'image de la carte de la page demandée
            Dim PixelsX = If(XImpr = 0, 0, PixelsLargeurImprimable * XImpr - PixelsCentrageXDeb)
            Dim PixelsY = If(YImpr = 0, 0, PixelsHauteurImprimable * YImpr - PixelsCentrageYDeb)
            'valeur en pixels de la largeur et de la hauteur de la partie de l'image de la carte correspondant à la page demandée.
            Dim PixelsL = PixelsLargeurImprimable + PixelsRecouvreX - If(XImpr = 0, PixelsCentrageXDeb, 0) - If(XImpr = NbPagesX - 1, PixelsCentrageXFin, 0)
            Dim PixelsH = PixelsHauteurImprimable + PixelsRecouvreY - If(YImpr = 0, PixelsCentrageYDeb, 0) - If(YImpr = NbPagesY - 1, PixelsCentrageYFin, 0)
            'Découpage de l'image de la carte en fonction des dimensions calculées et passage à la boite de dialogue
            Using S = BitMapCarte.Clone(New Rectangle(New Point(PixelsX, PixelsY), New Size(PixelsL, PixelsH)), BitMapCarte.PixelFormat)
                Return AfficherImage(Me, S, New Size(800, 800))
            End Using
        End Function
        ''' <summary> trouve les variables associées à une imprssion en fonction des choix de l'utilisateur </summary>
        Private Sub CalculerImpression()
            InitialiserImpression()
            'calcul des différentes variables
            If ChoixEchelle.Checked Then
                EchelleToNbPages(ValeurEchelle.Value)
            Else
                NbPagesToEchelle(CInt(ValeurNbCol.Value), CInt(ValeurNbRang.Value))
            End If
            FinaliserImpression()
            'affichage des résultats sous forme de texte
            LabelResultats.Text = $"Nb Pages : { NbPagesX * NbPagesY } ({ NbPagesX}*{ NbPagesY}) en {If(IsPaysage, "Paysage", "Portrait")}, Echelle : { EchelleImpression}"
            'et de plan
            CalculerPlanImpresion()
            PlanImpression.Invalidate()
        End Sub
        ''' <summary> Variables associées à la page de papier </summary>
        Private Sub InitialiserImpression()
            'trouver les dimensions du papier
            Dim PosCar As Integer = 2
            Dim NomPapier As String = ListeChoixPapier.Text
            Dim Largeur As String = TrouverChaineCompriseEntre(NomPapier, PosCar, "("c, "*"c)
            Dim Hauteur As String = TrouverChaineCompriseEntre(NomPapier, PosCar, "*"c, " "c)
            DimensionsPapier = New SizeF(StrToSng(Largeur), StrToSng(Hauteur))
            Bords = CInt(ValeurBordures.Value)
            ReCouvre = CInt(ValeurRecouvrement.Value)
            IsPaysage = False
            BordCoordonnees = If(IsCoordonnees, MargeCoordonnees, 0)
            BordsXPageImprimable = Bords * 2 + ReCouvre
            BordsYPageImprimable = Bords * 2 + ReCouvre + BordCoordonnees
            'dimensions d'une page imprimable sans tenir compte de la marge pour les coordonnées car on ne connait pas encore l'orentiation du papier.
            DimensionsPageImprimable = New SizeF(DimensionsPapier.Width - BordsXPageImprimable, DimensionsPapier.Height - BordsXPageImprimable)
        End Sub
        ''' <summary> variables associées à la surface d'impression. Ne traite que des dimensions théoriques en Mm </summary>
        Private Sub FinaliserImpression()
            'échelle d'impression de la carte
            EchelleImpression = $"1/{ MmGeoref.Height / DimensionsCarteImprimable.Height:N0}"
            'trouver les dimensions de la surface imprimable. La dernière page en X ou Y a un recouvrement qui ne compte pas
            DimensionsPagesImprimable = New SizeF(LargeurPageImprimable * NbPagesX + ReCouvre, HauteurPageImprimable * NbPagesY + ReCouvre)
            'les valeurs de centrage de la carte si demandé
            CentrageImprXDeb = (DimensionsPagesImprimable.Width - DimensionsCarteImprimable.Width) / 2
            CentrageImprXFin = CentrageImprXDeb
            CentrageImprYDeb = (DimensionsPagesImprimable.Height - DimensionsCarteImprimable.Height) / 2
            CentrageImprYFin = CentrageImprYDeb
            'Valeurs de l'imprimante liées à l'impression
            Papier = Papiers(ListeChoixPapier.SelectedIndex)
        End Sub
        ''' <summary> Affiche les résultats des calculs liés à l'impression sous forme de plan </summary>
        Private Sub CalculerPlanImpresion()
            Dim LargeurPages As Single = DimensionsPagesImprimable.Width + BordsXPageImprimable
            Dim HauteurPages As Single = DimensionsPagesImprimable.Height + BordsYPageImprimable
            'calcul du coef d'échelle Mm Imprimé en Pixel écran
            CoefR = TailleImpressionPlan.Width / LargeurPages
            Dim R_Y As Single = TailleImpressionPlan.Height / HauteurPages
            'on prend le plus petit pour que cela tienne dans la surface de dessin allouée sans déformation
            If CoefR > R_Y Then CoefR = R_Y
            'transformation des dimensions Mm imprimé en pixel écran
            Marge = CInt(Math.Round(Bords * CoefR))
            Coord = CInt(Math.Round(BordCoordonnees * CoefR))
            Dim Recouv As Integer = CInt(Math.Round(ReCouvre * CoefR))
            Dim LargP As Integer = CInt(Math.Round((LargeurPageImprimable + BordsXPageImprimable) * CoefR))
            Dim DeltaX As Integer = LargP - (Recouv + Marge)
            Dim HautP As Integer = CInt(Math.Round((HauteurPageImprimable + BordsYPageImprimable) * CoefR))
            Dim DeltaY As Integer = HautP - (Recouv + Marge + Coord)
            'calcule les dimensions de l'ensemble des pages à imprimer en pixels
            Dim TaillePagesPlan = New Size(DeltaX * NbPagesX + Recouv + Marge, DeltaY * NbPagesY + Coord + Recouv + Marge)
            'calcul de l'origine des pages à imprimer en pixels en centrant dans la zone de dessin
            Dim XPagesPlan As Integer = (TailleImpressionPlan.Width - TaillePagesPlan.Width) \ 2 + AffichageBords
            Dim YPagesPlan As Integer = (TailleImpressionPlan.Height - TaillePagesPlan.Height) \ 2 + AffichageBords
            RegionPagesPlan = New Rectangle(New Point(XPagesPlan, YPagesPlan), TaillePagesPlan)
            'dimensions de la carte à imprimer en rapport avec la taille des pages en pixels pour éviter les erreurs d'arrondi sur la représentation de la carte
            Dim PX As Single = DimensionsCarteImprimable.Width / LargeurPages
            Dim PY As Single = DimensionsCarteImprimable.Height / HauteurPages
            TailleCartePlan = New Size(CInt(Math.Round(TaillePagesPlan.Width * PX)), CInt(Math.Round(TaillePagesPlan.Height * PY)))
            'initialisation et remplissage de la collection des pages à imprimer
            InitialiserRectangles(SurfaceRectangleTravail, Mode.Selection, Rectangle.Empty, True)
            Dim Cpt As Integer = 1
            Dim DY As Integer = YPagesPlan
            For PageY As Integer = 0 To NbPagesY - 1
                Dim DX = XPagesPlan
                For PageX As Integer = 0 To NbPagesX - 1
                    Dim Name As String = $"Page {Cpt}"
                    AjouterRectangle(New Point(DX, DY), New Point(DX + LargP, DY + HautP), Name, True)
                    DX += DeltaX
                    Cpt += 1
                Next
                DY += DeltaY
            Next
        End Sub
        ''' <summary> renvoi le rectangle occupé par la carte sur le plan de mapping en fonction du choix de centrage </summary>
        Private Function RegionCartePlan() As Rectangle
            Dim R As Rectangle
            If IsCentrage Then
                Dim DeltaX = (RegionPagesPlan.Width - TailleCartePlan.Width) \ 2
                If DeltaX < Marge Then DeltaX = Marge
                Dim DeltaY = Coord + (RegionPagesPlan.Height - Coord - TailleCartePlan.Height) \ 2
                If DeltaY < Marge Then DeltaY = Marge
                R = New Rectangle(New Point(RegionPagesPlan.X + DeltaX, RegionPagesPlan.Y + DeltaY), TailleCartePlan)
            Else
                Dim Pt = Point.Add(RegionPagesPlan.Location, New Size(Marge, Marge + Coord))
                R = New Rectangle(Pt, TailleCartePlan)
            End If
            Return R
        End Function
        ''' <summary> calcule le nb de pages minimum en X, Y pour imprimer la carte avec l'échelle demandée </summary>
        ''' <param name="EchelleImpr"> échelle demandée </param>
        Private Sub EchelleToNbPages(EchelleImpr As Single)
            'dimensions de la carte en Mm une fois imprimée pour respecter l'échelle demandée
            DimensionsCarteImprimable = New SizeF(MmGeoref.Width / EchelleImpr, MmGeoref.Height / EchelleImpr)
            'La surface imprimable doit être => à la surface imprimée de la carte. On teste l'orientation portrait et paysage et on prend celle qui a le moins de pages
            NbPagesX = CInt(Math.Ceiling((DimensionsCarteImprimable.Width - ReCouvre) / DimensionsPageImprimable.Width))
            NbPagesY = CInt(Math.Ceiling((DimensionsCarteImprimable.Height - ReCouvre) / (DimensionsPageImprimable.Height - BordCoordonnees)))
            NbPagesTotal = NbPagesX * NbPagesY
            'si l'imprimante accepte le mode paysage il y a une 2ème possibilité
            If IsSupportPaysage Then
                Dim NbPagesXH = CInt(Math.Ceiling((DimensionsCarteImprimable.Width - ReCouvre) / DimensionsPageImprimable.Height))
                Dim NbPagesYH = CInt(Math.Ceiling((DimensionsCarteImprimable.Height - ReCouvre) / (DimensionsPageImprimable.Width - BordCoordonnees)))
                If NbPagesTotal > NbPagesXH * NbPagesYH Then
                    NbPagesX = NbPagesXH
                    NbPagesY = NbPagesYH
                    IsPaysage = True
                End If
            End If
            'on prend en compte l'orientation du papier pour trouver les dimensions réelles des pages imprimables
            LargeurPageImprimable = If(IsPaysage, DimensionsPageImprimable.Height, DimensionsPageImprimable.Width)
            HauteurPageImprimable = If(IsPaysage, DimensionsPageImprimable.Width, DimensionsPageImprimable.Height) - BordCoordonnees
        End Sub
        ''' <summary> calcule la plus grande échelle d'impression de la carte avec le nb de pages demandé. </summary>
        ''' <param name="NbCol"> nb de colonnes ou axe des X </param>
        ''' <param name="NbRang"> nb de rangées ou axe des Y </param>
        Private Sub NbPagesToEchelle(NbCol As Integer, NbRang As Integer)
            Dim SurfaceCalcul, SurfaceCarte As Double
            Dim OrientationsCalcul, OrientationsCarte As (SensPapier As OrientationPapier, SensCarte As OrientionCarte)
            'il y a 2 possibilités par orientation de la carte. il n'y a qu'une seule orientation de carte si nbCol=nbRang
            For Possible As Integer = 0 To If(NbCol = NbRang, 1, 3)
                OrientationsCalcul = (CType(Possible Mod 2, OrientationPapier), CType(Possible \ 2, OrientionCarte))
                'si l'imprimante accepte le mode paysage il y a une 2ème possibilité pour une orientation de carte
                If OrientationsCalcul.SensPapier = OrientationPapier.Portrait OrElse IsSupportPaysage Then
                    SurfaceCalcul = CalculerDimensionsCarteImpression(OrientationsCalcul, NbCol, NbRang)
                    If SurfaceCalcul > SurfaceCarte Then
                        OrientationsCarte = OrientationsCalcul
                        SurfaceCarte = SurfaceCalcul
                    End If
                End If
            Next
            'on prend en compte l'orientation du papier et de la carte qui rend l'échelle la plus grande
            CalculerDimensionsCarteImpression(OrientationsCarte, NbCol, NbRang)
            'On Vérifie que le nb de pages demandé pour l'impression correspond au besoin de l'impression, cela supprime les pages vides 
            Dim DeltaX As Integer = CInt(Math.Floor((LargeurPageImprimable * NbPagesX + ReCouvre - DimensionsCarteImprimable.Width) / LargeurPageImprimable))
            NbPagesX -= DeltaX
            Dim DeltaY = CInt(Math.Floor((HauteurPageImprimable * NbPagesY + ReCouvre - DimensionsCarteImprimable.Height) / HauteurPageImprimable))
            NbPagesY -= DeltaY
        End Sub
        ''' <summary> détermine les variables et calcule les dimensions de la carte imprimée </summary>
        ''' <param name="OrientationCarte">SensPapier = Portrait ou Paysage et SensCarte = Horizontal ou Vertical </param>
        ''' <param name="NbCol"> Nb de Colonnes </param>
        ''' <param name="NbRang"> Nb de Rangées</param>
        Private Function CalculerDimensionsCarteImpression(OrientationCarte As (SensPapier As OrientationPapier, SensCarte As OrientionCarte),
                                                           NbCol As Integer, NbRang As Integer) As Single
            IsPaysage = OrientationCarte.SensPapier = OrientationPapier.Paysage
            NbPagesX = If(OrientationCarte.SensCarte = OrientionCarte.Horizontal, NbRang, NbCol)
            NbPagesY = If(OrientationCarte.SensCarte = OrientionCarte.Horizontal, NbCol, NbRang)
            LargeurPageImprimable = If(IsPaysage, DimensionsPageImprimable.Height, DimensionsPageImprimable.Width)
            HauteurPageImprimable = If(IsPaysage, DimensionsPageImprimable.Width, DimensionsPageImprimable.Height) - BordCoordonnees
            DimensionsPagesImprimable = New SizeF(LargeurPageImprimable * NbPagesX + ReCouvre, HauteurPageImprimable * NbPagesY + ReCouvre)
            Dim RapportI = DimensionsPagesImprimable.Width / RapCarteMm
            If RapportI > DimensionsPagesImprimable.Height Then
                'on s'assure ainsi que DimensionsCarteImpression.Hauteur=DimensionsImprimable.Hauteur sinon il peut avoir des erreurs d'arrondi
                DimensionsCarteImprimable = New SizeF(DimensionsPagesImprimable.Height * RapCarteMm, DimensionsPagesImprimable.Height)
            Else
                'on s'assure ainsi que DimensionsCarteImpression.Largeur=DimensionsImprimable.Largeur  sinon il peut avoir des erreurs d'arrondi
                DimensionsCarteImprimable = New SizeF(DimensionsPagesImprimable.Width, RapportI)
            End If
            Return DimensionsCarteImprimable.Width * DimensionsCarteImprimable.Height
        End Function
        ''' <summary> Trouve les valeurs par défaut associées à l'imprimante chosie par l'utilisateur </summary>
        Private Sub ChoisirImprimante()
            Dim Imprimante As New PrinterSettings()
            Dim Nom = ListeChoixImprimante.Text
            Imprimante.PrinterName = Nom
            'on cherche les bords matériels minimum de l'imprimante
            BordsMini = CInt(Math.Round(Imprimante.DefaultPageSettings.HardMarginX * CinchToMm, 0))
            BordsMini = Math.Max(BordsMini, CInt(Math.Round(Imprimante.DefaultPageSettings.HardMarginY * CinchToMm, 0)))
            'et on l'indique dans le formulaire
            ValeurBordures.Minimum = BordsMini
            Dim PapierImprimante = Imprimante.DefaultPageSettings.PaperSize
            Dim FlagAdd As Boolean, KindNom As String = ""
            Dim IndexPapier = -1
            Papiers = New List(Of PaperSize)
            PapiersNoms = New List(Of String)
            ListeChoixPapier.Items.Clear()
            For Each Pap As PaperSize In Imprimante.PaperSizes
                FlagAdd = True
                Select Case Pap.Kind
                    Case PaperKind.A4
                        KindNom = "A4 (210*297 mm)"
                    Case PaperKind.A3
                        KindNom = "A3 (297*420 mm)"
                    Case PaperKind.Letter
                        KindNom = "Lettre US (215.9*279.4 mm)"
                    Case PaperKind.Ledger
                        KindNom = "Ledger US (431.8*279.4 mm)"
                    Case PaperKind.Legal
                        KindNom = "Legal US (215.9*355.6 mm)"
                    Case PaperKind.Tabloid
                        KindNom = "Tabloïd (279.4*431.8 mm)"
                    Case Else
                        FlagAdd = False
                End Select
                If FlagAdd Then
                    If Pap.Kind = PapierImprimante.Kind Then IndexPapier = Papiers.Count
                    Papiers.Add(Pap)
                    PapiersNoms.Add(KindNom)
                End If
            Next
            ListeChoixPapier.Items.AddRange(PapiersNoms.ToArray)
            ListeChoixPapier.SelectedIndex = IndexPapier

            Dim ResolutionImprimante = Imprimante.DefaultPageSettings.PrinterResolution
            Dim IndexResolution = -1
            Resolutions = New List(Of PrinterResolution)
            ResolutionsNoms = New List(Of String)
            ListeChoixResolution.Items.Clear()
            For Each Reso As PrinterResolution In Imprimante.PrinterResolutions
                FlagAdd = True
                Select Case Reso.Kind
                    Case PrinterResolutionKind.Draft
                        KindNom = "Qualité Brouillon"
                    Case PrinterResolutionKind.Low
                        KindNom = "Qualité Basse (Texte)"
                    Case PrinterResolutionKind.Medium
                        KindNom = "Qualité Moyenne (Texte & Photos)"
                    Case PrinterResolutionKind.High
                        KindNom = "Qualité Haute (Photos)"
                    Case PrinterResolutionKind.Custom
                        KindNom = $"Qualité Personnalisée ({Reso.X}*{Reso.Y})"
                    Case Else
                        FlagAdd = False
                End Select
                If FlagAdd Then
                    If Reso.Kind = ResolutionImprimante.Kind Then IndexResolution = Resolutions.Count
                    Resolutions.Add(Reso)
                    ResolutionsNoms.Add(KindNom)
                End If
            Next
            If IndexResolution = -1 Then
                IndexResolution = Resolutions.Count
                Resolutions.Add(ResolutionImprimante)
                ResolutionsNoms.Add($"Qualité Personnalisée ({ResolutionImprimante.X}*{ResolutionImprimante.Y})")
            End If
            ListeChoixResolution.Items.AddRange(ResolutionsNoms.ToArray)
            ListeChoixResolution.SelectedIndex = IndexResolution

            If Imprimante.SupportsColor Then
                IsSupportColor = True
                ListeChoixCouleur.SelectedIndex = If(Imprimante.DefaultPageSettings.Color, 0, 1)
                ListeChoixCouleur.Enabled = True
            Else
                IsSupportColor = False
                ListeChoixCouleur.SelectedIndex = 1
                ListeChoixCouleur.Enabled = False
            End If
            IsSupportPaysage = Imprimante.LandscapeAngle <> 0
            IsRectoVerso = Imprimante.CanDuplex
        End Sub
        ''' <summary> calcul le Pt0 de la carte à imprimer en pixels </summary>
        Private Sub TrouverPt0Pixels()
            Dim LignesFichierGeoref() As String = File.ReadAllLines(OuvrirFichierGeoref.FileName, Encoding_FCGP)
            Dim PosCar As Integer = LignesFichierGeoref(0).IndexOf("."c) + 1
            'Trouve le système de capture de la carte
            Dim SiteLibelle = TrouverChaineCompriseEntre(LignesFichierGeoref(0), PosCar, ":"c)
            'Trouve l'échelle de capture de la carte sur la deuxième ligne du fichier georef
            PosCar = 0
            Dim ClefEchelle = TrouverChaineCompriseEntre(LignesFichierGeoref(1), PosCar, ":"c, ","c)
            'Trouve le type de coordonnées capturées de la carte sur la deuxième ligne du fichier georef
            PosCar = LignesFichierGeoref(1).IndexOf(", C")
            Dim LibelleProjection = TrouverChaineCompriseEntre(LignesFichierGeoref(1), PosCar, ":"c)

            PosCar = LignesFichierGeoref(3).IndexOf(":"c) + 1
            Dim X As String = TrouverChaineCompriseEntre(LignesFichierGeoref(3), PosCar, ":"c, ","c)
            Dim Bid = LignesFichierGeoref(3).IndexOf(":"c, PosCar)
            Dim Y As String
            If Bid = -1 Then
                Y = TrouverChaineCompriseEntre(LignesFichierGeoref(3), PosCar, ","c)
            Else
                Y = TrouverChaineCompriseEntre(LignesFichierGeoref(3), PosCar + 1, ":"c)
            End If
            Dim Pt0Grille As New PointD(StrToDbl(X), StrToDbl(Y))

            Dim Z As Integer = 0
            Dim H As Char
            Dim SystemeCartographiqueCarte = New SystemeCartographique(SiteLibelle, ClefEchelle, LibelleProjection, Z, H)
            If SystemeCartographiqueCarte.SiteCarto = SitesCartographiques.SuisseMobile Then
                LV95ToLV03(Pt0Grille)
            End If

            SiteCartoCarte = SystemeCartographiqueCarte.SiteCarto
            EchelleCarte = SystemeCartographiqueCarte.Niveau.Echelle
            Pt0Pixels = PointGrilleToPointPixel(Pt0Grille, SiteCartoCarte, EchelleCarte)
        End Sub
        ''' <summary> Trouve toutes les variables de la carte associées au fichier Georef </summary>
        ''' <param name="NomFichierGeoref"> Chemin complet du fichier Georef </param>
        Private Function LireFichierGeoref() As VerificationRenvoiCarte
            CheminFichierGeoref = Path.GetDirectoryName(OuvrirFichierGeoref.FileName)
            OuvrirFichierGeoref.InitialDirectory = CheminFichierGeoref
            Dim LignesFichierGeoref() As String = File.ReadAllLines(OuvrirFichierGeoref.FileName, Encoding_FCGP)
            Dim PosCar As Integer
            'Trouve le nom du fichier de l'image associée à la carte sur la 1ère ligne du fichier georef
            Dim NomFichierCapture = TrouverChaineCompriseEntre(LignesFichierGeoref(0), PosCar, ":"c, ","c)
            Dim Di As New DirectoryInfo(CheminFichierGeoref)
            'on cherche si la carte a un fichier associé avec les références des coordonnées
            NomCarte = $"{Path.GetFileNameWithoutExtension(NomFichierCapture)}_R"
            Dim Fichiers = Di.GetFiles(NomCarte & ".*")
            If Fichiers.Length = 0 Then
                'puis le fichier image associé au fichier georef
                Fichiers = Di.GetFiles(NomFichierCapture)
                If Fichiers.Length = 0 Then Return VerificationRenvoiCarte.Carte_Inexistante
                NomCarte = NomFichierCapture
            Else
                NomCarte = Fichiers(0).Name
            End If
            'recherche de l'échelle d'impression
            PosCar = LignesFichierGeoref(1).IndexOf(","c)
            Dim EchImpr = TrouverChaineCompriseEntre(LignesFichierGeoref(1), PosCar, ":"c, ","c)
            'rendu nécessaire pour les fichiers georef antérieur à 8.600
            If String.IsNullOrEmpty(EchImpr) Then EchImpr = "1000"
            EchelleImpressionGeoref = StrToSng(EchImpr) * 1000
            ValeurEchelle.Value = CDec(EchelleImpressionGeoref)
            PosCar = LignesFichierGeoref(2).IndexOf("DPI")
            Dim InchtoMm = CSng(Commun.InchToMm)
            Dim DPI_X = StrToSng(TrouverChaineCompriseEntre(LignesFichierGeoref(2), PosCar, ":"c, ","c)) / InchtoMm
            Dim DPI_Y = StrToSng(TrouverChaineCompriseEntre(LignesFichierGeoref(2), PosCar, ":"c)) / InchtoMm
            PosCar = 10
            PixelGeoref.Width = CInt(TrouverChaineCompriseEntre(LignesFichierGeoref(11), PosCar, ":"c, ","c))
            PixelGeoref.Height = CInt(TrouverChaineCompriseEntre(LignesFichierGeoref(11), PosCar, ":"c))
            MmGeoref = New SizeF(PixelGeoref.Width * EchelleImpressionGeoref / DPI_X, PixelGeoref.Height * EchelleImpressionGeoref / DPI_Y)
            RapCarteMm = MmGeoref.Width / MmGeoref.Height
            Return VerificationRenvoiCarte.OK
        End Function
    End Class
    Partial Class ChoixImpressionCarte
        Inherits Form
        Friend Sub New()
            ' Cet appel est requis par le concepteur.
            InitializeComponent()
            InitialiserEvenements()
        End Sub
        ''' <summary> Initialise l'ensemble des évenement liés au formulaire car aucune variable avec le modificateur WithEvents 
        ''' rendu obligatoire pour ne pas avoir à refaire les liaisons si l'on modifie le formulaire avec le concepteur </summary>
        Private Sub InitialiserEvenements()
            AddHandler ValeurBordures.ValueChanged, AddressOf ValeurMarges_ValueChanged
            AddHandler ValeurRecouvrement.ValueChanged, AddressOf ValeurMarges_ValueChanged

            AddHandler ChoixCentrage.CheckedChanged, AddressOf ChoixCentrage_CheckedChanged

            AddHandler ChoixCoordonnees.CheckedChanged, AddressOf ChoixCoordonnees_CheckedChanged

            AddHandler ChoixNbPages.CheckedChanged, AddressOf ValeurNbPages_ValueChanged
            AddHandler ValeurNbCol.ValueChanged, AddressOf ValeurNbPages_ValueChanged
            AddHandler ValeurNbRang.ValueChanged, AddressOf ValeurNbPages_ValueChanged

            AddHandler ChoixEchelle.CheckedChanged, AddressOf ValeurEchelle_ValueChanged
            AddHandler ValeurEchelle.ValueChanged, AddressOf ValeurEchelle_ValueChanged

            AddHandler LabelChoixFichierGeoref.Click, AddressOf LabelChoixFichierGeoref_Click
            AddHandler PlanImpression.Paint, AddressOf PlanImpression_Paint
            AddHandler PlanImpression.MouseDown, AddressOf PlanImpression_MouseDown
            AddHandler PlanImpression.MouseMove, AddressOf PlanImpression_MouseMove
            AddHandler PlanImpression.MouseUp, AddressOf PlanImpression_MouseUp
#Region "Listes et label associés"
            AddHandler LabelChoixImprimante.Click, AddressOf Labels_Click
            AddHandler LabelChoixPapier.Click, AddressOf Labels_Click
            AddHandler LabelChoixResolution.Click, AddressOf Labels_Click
            AddHandler LabelChoixCouleur.Click, AddressOf Labels_Click

            AddHandler ListeChoixImprimante.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
            AddHandler ListeChoixPapier.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
            AddHandler ListeChoixResolution.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
            AddHandler ListeChoixCouleur.SelectedIndexChanged, AddressOf Listes_SelectedIndexChanged
#End Region
#Region "Formulaire"
            AddHandler Load, AddressOf ChoixImpressionCarte_Load
            AddHandler FormClosing, AddressOf ChoixImpressionCarte_FormClosing
            AddHandler FormClosed, AddressOf ChoixImpressionCarte_FormClosed
#End Region
        End Sub
        'Form remplace la méthode Dispose pour nettoyer la liste des composants.
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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

        'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
        'Ne la modifiez pas à l'aide de l'éditeur de code.
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            components = New Container()
            PanelChoix = New Panel()
            ChoixCentrage = New CheckBox()
            LabelCentrage = New Label()
            LabelImprimante = New Label()
            LabelChoixImprimante = New Label()
            ListeChoixImprimante = New ComboBox()
            LabelChoixPapier = New Label()
            ListeChoixPapier = New ComboBox()
            LabelChoixResolution = New Label()
            ListeChoixResolution = New ComboBox()
            LabelChoixCouleur = New Label()
            ListeChoixCouleur = New ComboBox()
            LabelFichierGeoref = New Label()
            LabelChoixFichierGeoref = New Label()
            LabelImpression = New Label()
            LabelBordures = New Label()
            ChoixNbPages = New RadioButton()
            LabelNbPages = New Label()
            LabelRecouvrement = New Label()
            LabelNbCol = New Label()
            LabelNbRang = New Label()
            ChoixEchelle = New RadioButton()
            LabelEchelle = New Label()
            LabelCoordonnees = New Label()
            ChoixCoordonnees = New CheckBox()
            ValeurBordures = New NumericUpDown()
            ValeurRecouvrement = New NumericUpDown()
            ValeurNbCol = New NumericUpDown()
            ValeurNbRang = New NumericUpDown()
            ValeurEchelle = New NumericUpDown()
            LabelPlanImpression = New Label()
            PlanImpression = New Label()
            LabelResultats = New Label()
            OuvrirFichierGeoref = New OpenFileDialog()
            ToolTip1 = New ToolTip(components)
            btnNO = New Button()
            btnYES = New Button()
            PanelChoix.SuspendLayout()
            CType(ValeurBordures, ISupportInitialize).BeginInit()
            CType(ValeurRecouvrement, ISupportInitialize).BeginInit()
            CType(ValeurNbCol, ISupportInitialize).BeginInit()
            CType(ValeurNbRang, ISupportInitialize).BeginInit()
            CType(ValeurEchelle, ISupportInitialize).BeginInit()
            SuspendLayout()
            '
            'LabelImprimante
            '
            LabelImprimante.BorderStyle = BorderStyle.FixedSingle
            LabelImprimante.Location = New Point(6, 6)
            LabelImprimante.Name = "LabelImprimante"
            LabelImprimante.Size = New Size(353, 27)
            LabelImprimante.TabIndex = 14
            LabelImprimante.Text = "Choix concernant l'imprimante"
            LabelImprimante.TextAlign = ContentAlignment.MiddleCenter
            '
            'LabelChoixImprimante
            '
            LabelChoixImprimante.BackColor = Color.White
            LabelChoixImprimante.BorderStyle = BorderStyle.FixedSingle
            LabelChoixImprimante.Location = New Point(6, 32)
            LabelChoixImprimante.Name = "LabelChoixImprimante"
            LabelChoixImprimante.Size = New Size(353, 23)
            LabelChoixImprimante.TabIndex = 3
            LabelChoixImprimante.Tag = ListeChoixImprimante
            ToolTip1.SetToolTip(LabelChoixImprimante, "Cliquez pour choisir une imprimante dans la liste")
            '
            'ListeChoixImprimante
            '
            ListeChoixImprimante.BackColor = Color.White
            ListeChoixImprimante.FormattingEnabled = True
            ListeChoixImprimante.Location = New Point(6, 27)
            ListeChoixImprimante.Name = "ListeChoixImprimante"
            ListeChoixImprimante.Size = New Size(353, 27)
            ListeChoixImprimante.TabIndex = 2
            ListeChoixImprimante.Tag = LabelChoixImprimante
            ListeChoixImprimante.Visible = False
            '
            'LabelChoixPapier
            '
            LabelChoixPapier.BackColor = Color.White
            LabelChoixPapier.BorderStyle = BorderStyle.FixedSingle
            LabelChoixPapier.Location = New Point(6, 54)
            LabelChoixPapier.Name = "LabelChoixPapier"
            LabelChoixPapier.Size = New Size(353, 23)
            LabelChoixPapier.TabIndex = 4
            LabelChoixPapier.Tag = ListeChoixPapier
            ToolTip1.SetToolTip(LabelChoixPapier, "Cliquez choisir le format de papier" & CrLf &
                                              "parmi ceux acceptés par l'imprimante.")
            '
            'ListeChoixPapier
            '
            ListeChoixPapier.FormattingEnabled = True
            ListeChoixPapier.Location = New Point(6, 49)
            ListeChoixPapier.Name = "ListeChoixPapier"
            ListeChoixPapier.Size = New Size(353, 27)
            ListeChoixPapier.TabIndex = 3
            ListeChoixPapier.Tag = LabelChoixPapier
            ListeChoixPapier.Visible = False
            '
            'LabelChoixResolution
            '
            LabelChoixResolution.BackColor = Color.White
            LabelChoixResolution.BorderStyle = BorderStyle.FixedSingle
            LabelChoixResolution.Location = New Point(6, 76)
            LabelChoixResolution.Name = "LabelChoixResolution"
            LabelChoixResolution.Size = New Size(243, 23)
            LabelChoixResolution.TabIndex = 5
            LabelChoixResolution.Tag = ListeChoixResolution
            ToolTip1.SetToolTip(LabelChoixResolution, "Cliquez pour choisir la qualité d'impression")
            '
            'ListeChoixResolution
            '
            ListeChoixResolution.FormattingEnabled = True
            ListeChoixResolution.Location = New Point(6, 71)
            ListeChoixResolution.Name = "ListeChoixResolution"
            ListeChoixResolution.Size = New Size(243, 27)
            ListeChoixResolution.TabIndex = 4
            ListeChoixResolution.Tag = LabelChoixResolution
            ListeChoixResolution.Visible = False
            '
            'LabelChoixCouleur
            '
            LabelChoixCouleur.BackColor = Color.White
            LabelChoixCouleur.BorderStyle = BorderStyle.FixedSingle
            LabelChoixCouleur.Location = New Point(248, 76)
            LabelChoixCouleur.Name = "LabelChoixCouleur"
            LabelChoixCouleur.Size = New Size(111, 23)
            LabelChoixCouleur.TabIndex = 6
            LabelChoixCouleur.Tag = ListeChoixCouleur
            ToolTip1.SetToolTip(LabelChoixCouleur, "Cliquez pour choisir entre " & CrLf &
                                               "couleur et niveau de gris")
            '
            'ListeChoixCouleur
            '
            ListeChoixCouleur.FormattingEnabled = True
            ListeChoixCouleur.Items.AddRange(New Object() {"Couleur", "Niveau de gris"})
            ListeChoixCouleur.Location = New Point(248, 71)
            ListeChoixCouleur.Name = "ListeChoixCouleur"
            ListeChoixCouleur.Size = New Size(111, 27)
            ListeChoixCouleur.TabIndex = 5
            ListeChoixCouleur.Tag = LabelChoixCouleur
            ListeChoixCouleur.Visible = False
            '
            'LabelFichierGeoref
            '
            LabelFichierGeoref.BorderStyle = BorderStyle.FixedSingle
            LabelFichierGeoref.Location = New Point(6, 105)
            LabelFichierGeoref.Name = "LabelFichierGeoref"
            LabelFichierGeoref.Size = New Size(353, 27)
            LabelFichierGeoref.TabIndex = 19
            LabelFichierGeoref.Text = "Fichier Georef de la carte"
            LabelFichierGeoref.TextAlign = ContentAlignment.MiddleCenter
            '
            'LabelChoixFichierGeoref
            '
            LabelChoixFichierGeoref.BackColor = Color.White
            LabelChoixFichierGeoref.BorderStyle = BorderStyle.FixedSingle
            LabelChoixFichierGeoref.Location = New Point(6, 131)
            LabelChoixFichierGeoref.Name = "LabelChoixFichierGeoref"
            LabelChoixFichierGeoref.Size = New Size(353, 23)
            LabelChoixFichierGeoref.TabIndex = 7
            LabelChoixFichierGeoref.TextAlign = ContentAlignment.MiddleLeft
            ToolTip1.SetToolTip(LabelChoixFichierGeoref, "Cliquez pour sélectionner une carte (fichier Georef associé)")
            '
            'LabelImpression
            '
            LabelImpression.BorderStyle = BorderStyle.FixedSingle
            LabelImpression.Location = New Point(6, 160)
            LabelImpression.Name = "LabelImpression"
            LabelImpression.Size = New Size(353, 27)
            LabelImpression.TabIndex = 20
            LabelImpression.Text = "Choix concernant l'impression"
            LabelImpression.TextAlign = ContentAlignment.MiddleCenter
            '
            'LabelCentrage
            '
            LabelCentrage.BorderStyle = BorderStyle.FixedSingle
            LabelCentrage.Location = New Point(6, 186)
            LabelCentrage.Name = "LabelCentrage"
            LabelCentrage.Size = New Size(117, 26)
            LabelCentrage.TabIndex = 34
            '
            'ChoixCentrage
            '
            ChoixCentrage.CheckAlign = ContentAlignment.MiddleRight
            ChoixCentrage.Checked = True
            ChoixCentrage.CheckState = CheckState.Checked
            ChoixCentrage.Location = New Point(9, 187)
            ChoixCentrage.Name = "ChoixCentrage"
            ChoixCentrage.Size = New Size(105, 24)
            ChoixCentrage.TabIndex = 35
            ChoixCentrage.Text = "Centrage"
            ChoixCentrage.TextAlign = ContentAlignment.TopLeft
            ToolTip1.SetToolTip(ChoixCentrage, "Centre la carte sur la surface imprimable")
            ChoixCentrage.UseVisualStyleBackColor = True
            '
            'LabelBordures
            '
            LabelBordures.BorderStyle = BorderStyle.FixedSingle
            LabelBordures.Location = New Point(122, 186)
            LabelBordures.Name = "LabelBordures"
            LabelBordures.Size = New Size(80, 26)
            LabelBordures.TabIndex = 7
            LabelBordures.Text = "Bordures"
            '
            'ValeurBordures
            '
            ValeurBordures.BorderStyle = BorderStyle.FixedSingle
            ValeurBordures.Location = New Point(201, 186)
            ValeurBordures.Maximum = 10D
            ValeurBordures.Name = "ValeurBordures"
            ValeurBordures.Size = New Size(40, 26)
            ValeurBordures.TabIndex = 8
            ToolTip1.SetToolTip(ValeurBordures, "Définit en Mm la largeur des bords" & CrLf & "de la page qui n'est pas imprimée")
            ValeurBordures.Value = 5D
            '
            'LabelRecouvrement
            '
            LabelRecouvrement.BorderStyle = BorderStyle.FixedSingle
            LabelRecouvrement.Location = New Point(240, 186)
            LabelRecouvrement.Name = "LabelRecouvrement"
            LabelRecouvrement.Size = New Size(80, 26)
            LabelRecouvrement.TabIndex = 9
            LabelRecouvrement.Text = "Recouvre"
            '
            'ValeurRecouvrement
            '
            ValeurRecouvrement.BorderStyle = BorderStyle.FixedSingle
            ValeurRecouvrement.Location = New Point(319, 186)
            ValeurRecouvrement.Maximum = 10D
            ValeurRecouvrement.Name = "ValeurRecouvrement"
            ValeurRecouvrement.Size = New Size(40, 26)
            ValeurRecouvrement.TabIndex = 9
            ToolTip1.SetToolTip(ValeurRecouvrement, "Définit en Mm le recouvrement ou " & CrLf &
                                                "chevauchement entre 2 pages")
            '
            'LabelNbPages
            '
            LabelNbPages.BorderStyle = BorderStyle.FixedSingle
            LabelNbPages.Location = New Point(6, 211)
            LabelNbPages.Name = "LabelNbPages"
            LabelNbPages.Size = New Size(117, 26)
            LabelNbPages.TabIndex = 21
            '
            'ChoixNbPages
            '
            ChoixNbPages.CheckAlign = ContentAlignment.MiddleRight
            ChoixNbPages.Checked = True
            ChoixNbPages.Location = New Point(9, 212)
            ChoixNbPages.Name = "ChoixNbPages"
            ChoixNbPages.Size = New Size(104, 24)
            ChoixNbPages.TabIndex = 10
            ChoixNbPages.TabStop = True
            ChoixNbPages.Text = "Nb de Pages"
            ToolTip1.SetToolTip(ChoixNbPages, "Le nombre de pages demandé définit" & CrLf &
                                          "l'échelle maximale d'impression")
            ChoixNbPages.UseVisualStyleBackColor = True
            '
            'LabelNbCol
            '
            LabelNbCol.BorderStyle = BorderStyle.FixedSingle
            LabelNbCol.Location = New Point(122, 211)
            LabelNbCol.Name = "LabelNbCol"
            LabelNbCol.Size = New Size(80, 26)
            LabelNbCol.TabIndex = 28
            LabelNbCol.Text = "Nb Cols"
            '
            'ValeurNbCol
            '
            ValeurNbCol.Location = New Point(201, 211)
            ValeurNbCol.Maximum = 25D
            ValeurNbCol.Minimum = 1D
            ValeurNbCol.Name = "ValeurNbCol"
            ValeurNbCol.Size = New Size(40, 26)
            ValeurNbCol.TabIndex = 11
            ToolTip1.SetToolTip(ValeurNbCol, "Nombre de colonnes." & CrLf &
                                         "Peut être inversée avec le nombre de rangées" & CrLf &
                                         "ou diminué si il y a des pages qui ne sont imprimées")
            ValeurNbCol.Value = 1D
            '
            'LabelNbRang
            '
            LabelNbRang.BorderStyle = BorderStyle.FixedSingle
            LabelNbRang.Location = New Point(240, 211)
            LabelNbRang.Name = "LabelNbRang"
            LabelNbRang.Size = New Size(80, 26)
            LabelNbRang.TabIndex = 30
            LabelNbRang.Text = "Nb Rangs"
            '
            'ValeurNbRang
            '
            ValeurNbRang.Location = New Point(319, 211)
            ValeurNbRang.Maximum = 25D
            ValeurNbRang.Minimum = 1D
            ValeurNbRang.Name = "ValeurNbRang"
            ValeurNbRang.Size = New Size(40, 26)
            ValeurNbRang.TabIndex = 12
            ToolTip1.SetToolTip(ValeurNbRang, "Nombre de rangées." & CrLf &
                                          "Peut être inversé avec le nombre de colonnes " & CrLf &
                                          "ou diminué si il y a des pages qui ne sont imprimées")
            ValeurNbRang.Value = 1D
            '
            'LabelEchelle
            '
            LabelEchelle.BorderStyle = BorderStyle.FixedSingle
            LabelEchelle.Location = New Point(6, 236)
            LabelEchelle.Name = "LabelEchelle"
            LabelEchelle.Size = New Size(117, 26)
            LabelEchelle.TabIndex = 31
            '
            'ChoixEchelle
            '
            ChoixEchelle.CheckAlign = ContentAlignment.MiddleRight
            ChoixEchelle.Location = New Point(9, 237)
            ChoixEchelle.Name = "ChoixEchelle"
            ChoixEchelle.Size = New Size(104, 24)
            ChoixEchelle.TabIndex = 13
            ChoixEchelle.TabStop = True
            ChoixEchelle.Text = "Echelle"
            ToolTip1.SetToolTip(ChoixEchelle, "L'échelle demandée définit" & CrLf &
                                          "le nombre minimal de pages")
            ChoixEchelle.UseVisualStyleBackColor = True
            '
            'ValeurEchelle
            '
            ValeurEchelle.Increment = 500D
            ValeurEchelle.Location = New Point(122, 236)
            ValeurEchelle.Maximum = 1_000_000D
            ValeurEchelle.Minimum = 5_000D
            ValeurEchelle.Name = "ValeurEchelle"
            ValeurEchelle.Size = New Size(80, 26)
            ValeurEchelle.TabIndex = 14
            ValeurEchelle.TextAlign = CType(HorizontalAlignment.Right, Windows.Forms.HorizontalAlignment)
            ValeurEchelle.ThousandsSeparator = True
            ToolTip1.SetToolTip(ValeurEchelle, "Valeur demandée pour l'échelle" & CrLf &
                                           "Toujours respectée")
            ValeurEchelle.Value = 5_000D
            '
            'TexteEchelle
            '
            LabelCoordonnees.BorderStyle = BorderStyle.FixedSingle
            LabelCoordonnees.Location = New Point(201, 236)
            LabelCoordonnees.Margin = New Padding(0, 0, 3, 0)
            LabelCoordonnees.Name = "TexteEchelle"
            LabelCoordonnees.Size = New Size(158, 26)
            LabelCoordonnees.TabIndex = 26
            LabelCoordonnees.Text = "1/5 000"
            LabelCoordonnees.TextAlign = ContentAlignment.MiddleCenter
            '
            'ChoixCoordonnees
            '
            ChoixCoordonnees.CheckAlign = ContentAlignment.MiddleRight
            'ChoixCoordonnees.Checked = False
            'ChoixCoordonnees.CheckState = CheckState.Checked
            ChoixCoordonnees.Location = New Point(205, 237)
            ChoixCoordonnees.Name = "ChoixCoordonnees"
            ChoixCoordonnees.Size = New Size(147, 24)
            ChoixCoordonnees.TabIndex = 35
            ChoixCoordonnees.Text = "Coordonnées Page"
            ChoixCoordonnees.TextAlign = ContentAlignment.TopLeft
            ToolTip1.SetToolTip(ChoixCoordonnees, "Imprime les coordonnées du Pt0 de la page" & CrLf &
                                                   "en UTM pour les sites WebMercator" & CrLf &
                                                   "ou Grille Suisse pour le site Suisse Mobile")
            ChoixCoordonnees.UseVisualStyleBackColor = True
            '
            'LabelPlanImpression
            '
            LabelPlanImpression.BorderStyle = BorderStyle.FixedSingle
            LabelPlanImpression.Location = New Point(6, 268)
            LabelPlanImpression.Name = "LabelPlanImpression"
            LabelPlanImpression.Size = New Size(353, 27)
            LabelPlanImpression.TabIndex = 32
            LabelPlanImpression.Text = "Plan des pages à imprimer en fonction des choix"
            LabelPlanImpression.TextAlign = ContentAlignment.MiddleCenter
            '
            'PlanImpression
            '
            PlanImpression.BorderStyle = BorderStyle.FixedSingle
            PlanImpression.Location = New Point(6, 294)
            PlanImpression.Name = "PlanImpression"
            PlanImpression.Size = New Size(353, 353)
            PlanImpression.TabIndex = 33
            '
            'LabelResultats
            '
            LabelResultats.BorderStyle = BorderStyle.FixedSingle
            LabelResultats.Location = New Point(6, 646)
            LabelResultats.Name = "LabelResultats"
            LabelResultats.Size = New Size(353, 23)
            LabelResultats.TabIndex = 32
            LabelResultats.TextAlign = ContentAlignment.MiddleCenter
            '
            'PanelChoix
            '
            PanelChoix.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            PanelChoix.BackColor = Color.FromArgb(247, 247, 247)
            PanelChoix.Controls.Add(ChoixCentrage)
            PanelChoix.Controls.Add(LabelCentrage)
            PanelChoix.Controls.Add(LabelImprimante)
            PanelChoix.Controls.Add(LabelChoixImprimante)
            PanelChoix.Controls.Add(ListeChoixImprimante)
            PanelChoix.Controls.Add(LabelChoixPapier)
            PanelChoix.Controls.Add(ListeChoixPapier)
            PanelChoix.Controls.Add(LabelChoixResolution)
            PanelChoix.Controls.Add(ListeChoixResolution)
            PanelChoix.Controls.Add(LabelChoixCouleur)
            PanelChoix.Controls.Add(ListeChoixCouleur)
            PanelChoix.Controls.Add(LabelFichierGeoref)
            PanelChoix.Controls.Add(LabelChoixFichierGeoref)
            PanelChoix.Controls.Add(LabelImpression)
            PanelChoix.Controls.Add(LabelBordures)
            PanelChoix.Controls.Add(ChoixNbPages)
            PanelChoix.Controls.Add(LabelNbPages)
            PanelChoix.Controls.Add(LabelRecouvrement)
            PanelChoix.Controls.Add(LabelNbCol)
            PanelChoix.Controls.Add(LabelNbRang)
            PanelChoix.Controls.Add(ChoixEchelle)
            PanelChoix.Controls.Add(LabelEchelle)
            PanelChoix.Controls.Add(ChoixCoordonnees)
            PanelChoix.Controls.Add(LabelCoordonnees)
            PanelChoix.Controls.Add(ValeurBordures)
            PanelChoix.Controls.Add(ValeurRecouvrement)
            PanelChoix.Controls.Add(ValeurNbCol)
            PanelChoix.Controls.Add(ValeurNbRang)
            PanelChoix.Controls.Add(ValeurEchelle)
            PanelChoix.Controls.Add(LabelPlanImpression)
            PanelChoix.Controls.Add(PlanImpression)
            PanelChoix.Controls.Add(LabelResultats)
            PanelChoix.Location = New Point(0, 0)
            PanelChoix.Name = "PanelChoix"
            PanelChoix.Size = New Size(365, 676)
            PanelChoix.TabIndex = 26
            '
            'btnNO
            '
            btnNO.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnNO.DialogResult = DialogResult.Cancel
            btnNO.Location = New Point(203, 682)
            btnNO.Name = "btnNO"
            btnNO.Size = New Size(75, 30)
            btnNO.TabIndex = 2
            btnNO.Text = "Quitter"
            ToolTip1.SetToolTip(btnNO, "Ferme le formulaire sans " & CrLf &
                                   "imprimer la carte")
            btnNO.UseVisualStyleBackColor = True
            '
            'btnYES
            '
            btnYES.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnYES.DialogResult = DialogResult.OK
            btnYES.Enabled = False
            btnYES.Location = New Point(284, 682)
            btnYES.Name = "btnYES"
            btnYES.Size = New Size(75, 30)
            btnYES.TabIndex = 1
            btnYES.Text = "Imprimer"
            ToolTip1.SetToolTip(btnYES, "Ferme le formulaire " & CrLf &
                                    "et lance l'impression")
            btnYES.UseVisualStyleBackColor = True
            '
            'ImprimeCarte
            '
            AcceptButton = btnYES
            AutoScaleMode = AutoScaleMode.None
            CancelButton = btnNO
            ClientSize = New Size(365, 718)
            ControlBox = False
            Controls.Add(PanelChoix)
            Controls.Add(btnNO)
            Controls.Add(btnYES)
            Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
            FormBorderStyle = FormBorderStyle.FixedToolWindow
            MaximizeBox = False
            MinimizeBox = False
            Name = "ImprimeCarte"
            ShowIcon = False
            ShowInTaskbar = False
            SizeGripStyle = SizeGripStyle.Hide
            StartPosition = FormStartPosition.CenterScreen
            'TopMost = True
            PanelChoix.ResumeLayout(False)
            CType(ValeurBordures, ISupportInitialize).EndInit()
            CType(ValeurRecouvrement, ISupportInitialize).EndInit()
            CType(ValeurNbCol, ISupportInitialize).EndInit()
            CType(ValeurNbRang, ISupportInitialize).EndInit()
            CType(ValeurEchelle, ISupportInitialize).EndInit()
            PanelChoix.PerformLayout()
            ResumeLayout(False)
        End Sub
#Region "Declration variables"
        Private LabelChoixFichierGeoref As Label
        Private OuvrirFichierGeoref As OpenFileDialog
        Private ListeChoixImprimante As ComboBox
        Private ListeChoixPapier As ComboBox
        Private ListeChoixResolution As ComboBox
        Private ListeChoixCouleur As ComboBox
        Private ValeurBordures As NumericUpDown
        Private LabelBordures As Label
        Private LabelRecouvrement As Label
        Private ValeurRecouvrement As NumericUpDown
        Private ToolTip1 As ToolTip
        Private btnNO As Button
        Private btnYES As Button
        Private LabelImprimante As Label
        Private LabelChoixImprimante As Label
        Private LabelChoixPapier As Label
        Private LabelChoixResolution As Label
        Private LabelChoixCouleur As Label
        Private LabelFichierGeoref As Label
        Private LabelImpression As Label
        Private LabelNbPages As Label
        Private ChoixNbPages As RadioButton
        Private ChoixEchelle As RadioButton
        Private ValeurEchelle As NumericUpDown
        Private LabelCoordonnees As Label
        Private ChoixCoordonnees As CheckBox
        Private LabelNbCol As Label
        Private ValeurNbCol As NumericUpDown
        Private LabelNbRang As Label
        Private ValeurNbRang As NumericUpDown
        Private LabelEchelle As Label
        Private LabelPlanImpression As Label
        Private LabelResultats As Label
        Private PlanImpression As Label
        Private PanelChoix As Panel
        Private ChoixCentrage As CheckBox
        Private LabelCentrage As Label
#End Region
    End Class
End Module