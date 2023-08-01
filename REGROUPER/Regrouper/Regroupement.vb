Imports FCGP.Regrouper
Imports FCGP.NiveauDetailCartographique
Imports FCGP.SystemeCartographique
''' <summary>permet d'indiquer à l'appelant comment c'est passer l'action demandée sur l'échelle</summary>
Friend Enum VerifierRegroupement As Integer
    ''' <summary>La création de l'échelle est ok</summary>
    OK = 0
    ''' <summary>Le fichier géoref n'est pas valide</summary>
    FichierGeoref_Non_Valide = 1
    ''' <summary>L'échelle est déja existante</summary>
    Regroupement_Déja_Existant = 2
    ''' <summary>L'échelle est inexistante</summary>
    Regroupement_Inexistant = 3
    ''' <summary>Les cartes du répertoire coorespondantes à l'échelle sont déja importées</summary>
    Repertoire_Déja_Importé = 4
    ''' <summary> aucune carte valide n'a été trouvée pour ce regroupement dans ce répertoire </summary>
    Aucune_Carte_Valide = 5
End Enum
''' <summary> regroupe toutes les cartes situées dans un même répertoire et ayant le même système cartographique ou compatible 
''' Site+Echelle et datum peut être différent. Ex : GF 015 avec WebMercator ou WGS84 ou Lambert93 </summary>
Friend NotInheritable Class Regroupement
#Region "Elements Regroupement"
    ''' <summary> numéro unique représantant le site carto et le domtom. Sert pour la sélection relle associée à un SiteCarto+DomTom </summary>
    Friend ReadOnly Property NumSite As Integer
    ''' <summary>Cle du système cartographique </summary>
    ''' <remarks> donné par la carte qui initialise le regroupement </remarks>
    Friend ReadOnly Property ClefEchelle() As String
        Get
            Return _SystemeType.ClefEchelle
        End Get
    End Property
    ''' <summary> datum associé au regroupement. Si coordonnées de géoréférencement = DD alors WGS84 sinon C'est un datum Grille </summary>
    ''' <remarks> donné par la carte qui initialise le regroupement </remarks>
    Friend ReadOnly Property Datum As Datums
        Get
            Return _SystemeType.Projection.Datum
        End Get
    End Property
    ''' <summary>données définissant le système de coordonnées du regroupement </summary>
    ''' <remarks> donné par la carte qui initialise le regroupement </remarks>
    Friend ReadOnly Property SystemeType() As SystemeCartographique
    ''' <summary> Cle du regroupement. Chaine unique identifiant Site+DomTom+Echelle, Doit permettre le tri croissant ou décroissant.
    ''' Fourni à la demande de création par l'appelant </summary>
    Friend ReadOnly Property Clef As String
    ''' <summary> Cartes associées au regroupement </summary>
    ''' <remarks> par défaut les cartes sont issues du même répertoire que la carte qui sert à la création du regroupement </remarks>
    Friend ReadOnly Property Cartes() As List(Of Carte)
    '''<summary> nb de cartes du regroupement </summary>
    Friend ReadOnly Property NbCartes As Integer
        Get
            Return _Cartes.Count
        End Get
    End Property
    '''<summary> Région virtuelle couverte par l'ensemble des cartes du regroupement. C'est la plus petite zone rectangulaire qui contient l'ensemble des cartes </summary>
    Friend ReadOnly Property RegionVirtuelle() As Rectangle
    ''' <summary> Coef du zoom de l'affichage du regroupement </summary>   
    Friend Property CoefZoomAffichageRetour() As Double
    ''' <summary> coordonnées en monde virtuel qui sera le centre de l'affichage lors de l'affichage du regroupement </summary>
    Friend Property Affiche_PositionRetour() As Point
    ''' <summary> taille en unité virtuelle de la zone de sélection du regroupement en cours </summary>
    Friend ReadOnly Property SelectionGeoref() As Rectangle
        Get
            'On appele la transformation pour obtenir la sélection de référencement associé au site carto </remarks>
            SelectionReferencementToSelectionGeoref()
            Return _SelectionGeoref
        End Get
    End Property
    ''' <summary> position du pointeur de souris en unité virtuelles lors de la sélection de la carte: Retour AffichePlanEchelle ou AjouterCartes</summary>
    Friend ReadOnly Property PositionGeoref As Point
    ''' <summary>Numéro de carte sélectionnée -1 ou 1 à N cartes : Retour AffichePlanEchelle ou AjouterCartes</summary>
    Friend ReadOnly Property NumCarteSelection As Integer
    ''' <summary> Permet l'ajout de cartes dans la collection de cartes de l'échelle transmise en pramètre </summary>
    ''' <param name="FichierGeoref">fichier georef permettant l'initalisation de l'échelle</param>
    ''' <param name="NbCartesAjout"> Nb de carte ajoutées à l'échelle </param>
    Friend Function AjouterCartes(FichierGeoref As String, ByRef NbCartesAjout As Integer) As VerifierRegroupement
        'trouve le répertoire du fichier
        Dim Chemin As String = Path.GetDirectoryName(FichierGeoref)
        'on regarde si le répertoire a déja été importé 
        Dim IndexRepertoireImporte As Integer = RepertoiresImportes.BinarySearch(Chemin)
        'si oui on ressort car le répertoire a déja été importé
        If IndexRepertoireImporte >= 0 Then
            NbCartesAjout = 0
            Return VerifierRegroupement.Repertoire_Déja_Importé
        Else
            'si non on insert le répertoire dans la collection en respectant le tri
            IndexRepertoireImporte = IndexRepertoireImporte Xor -1
            RepertoiresImportes.Insert(IndexRepertoireImporte, Chemin)
        End If
        'commence la boucle d'ajout des cartes, on trouve l'ensemble des fichiers georef du répertoire
        Dim RetCarte As VerificationRenvoiCarte
        NbCartesAjout = _Cartes.Count
        Dim ListeFichierGeoref As String() = Directory.GetFiles(Path.GetDirectoryName(FichierGeoref), "*.GEOREF", SearchOption.TopDirectoryOnly)
        For Each Fichier As String In ListeFichierGeoref
            'si la carte est valide et correspond à l'échelle actuelle
            Dim CarteActuelle As Carte = Carte.RenvoyerCarte(Fichier, RetCarte, TypeSupportCarte.Aucun, False, SystemeType, ChoixFichiersTuiles.Aucun, _Cartes.Count)
            's'agit'il du bon système et de la bonne échelle  
            If RetCarte = VerificationRenvoiCarte.OK AndAlso CarteActuelle.IsOk AndAlso CarteActuelle.Format Is ImageFormat.Bmp Then
                'si c'est la carte qui sert de point de départ on détermine le numéro et son centre pour l'affichage au retour
                If FichierGeoref = Fichier Then
                    'on indique le numéro dans la collection
                    _NumCarteSelection = _Cartes.Count
                    ' et le point central de la carte
                    _PositionGeoref.X = CarteActuelle.RegionVirtuelle.X + CarteActuelle.RegionVirtuelle.Width \ 2
                    _PositionGeoref.Y = CarteActuelle.RegionVirtuelle.Y + CarteActuelle.RegionVirtuelle.Height \ 2
                End If
                _Cartes.Add(CarteActuelle)
                'on met à jour l'emprise virtuelle représentée par l'ensemble des cartes
                If _RegionVirtuelle = Rectangle.Empty Then
                    _RegionVirtuelle = CarteActuelle.RegionVirtuelle
                Else
                    _RegionVirtuelle = Rectangle.Union(_RegionVirtuelle, CarteActuelle.RegionVirtuelle)
                End If
            End If
        Next
        NbCartesAjout = _Cartes.Count - NbCartesAjout
        If NbCartesAjout = 0 Then
            Return VerifierRegroupement.Aucune_Carte_Valide
        Else
            Return VerifierRegroupement.OK
        End If
    End Function
    ''' <summary> affiche le plan de calepinage du regroupement sur une fenêtre qui aura les dimensions indiquées et à bords des bords de la fenêtres </summary>
    ''' <param name="Dimension"> dimensions de la fenêtre d'affichage du plan </param>
    ''' <param name="Bords"> le plan sera à minima à bords des bors de la fenêtre (padding) </param>
    ''' <param name="Affichage">rectangle qui représente les dimensions et la position de l'affichage en monde virtuel
    ''' Si rectangle.empty, la représentation de l'affichage de la carte ne sera pas affichée sur le plan</param>
    Friend Sub AfficherCalepinage(Affichage As Rectangle, Dimension As Integer, Bords As Integer)
        Using F As New CalepinageRegroupement With {.RegroupementActuel = Me}
            SelectionReferencementToSelectionGeoref()
            'mise en place des variables à passer au formulaire MappingAffiche
            RegionAffichage = Affichage
            'dimensionnement de la fenêtre plan Echelle   
            If Dimension < 700 Then Dimension = 700
            TaillePlanRegroupement = New Size(Dimension, Dimension)
            If Bords < 10 OrElse Bords > 150 Then Bords = 35
            Affiche_Bords = Bords
            F.ShowDialog(FormApplication)
            SelectionGeorefToSelectionReferencement()
        End Using
    End Sub
    ''' <summary> chaque site peut avoir une sélection qui est commune à tous les regroupement associé au site </summary>
    Friend Sub SelectionReferencementToSelectionGeoref()
        'Si la sélection grille n'a pas changée, la sélection virtuelle non plus
        If OldSelectionReferencement = SelectionsReferencement(_NumSite) Then Exit Sub
        OldSelectionReferencement = SelectionsReferencement(_NumSite)
        If OldSelectionReferencement.IsEmpty Then
            _SelectionGeoref = Rectangle.Empty
        Else
            Dim PT0_Virtuel As Point = _SystemeType.ConvertirCoordonneesReellesToVirtuelles(OldSelectionReferencement.Pt0)
            Dim PT2_Virtuel As Point = _SystemeType.ConvertirCoordonneesReellesToVirtuelles(OldSelectionReferencement.Pt2)
            _SelectionGeoref = New Rectangle(PT0_Virtuel.X, PT0_Virtuel.Y, PT2_Virtuel.X - PT0_Virtuel.X, PT2_Virtuel.Y - PT0_Virtuel.Y)
        End If
    End Sub
    ''' <summary> transforme rectangle exprimée en coordonnées virtuelle du systeme cartographique en un rectangle exprimée 
    ''' en coordonnées de référencement du systeme cartographique</summary>
    ''' <param name="SystemeCarto">sytème cartographique de l'échelle à charger ou à afficher</param>
    ''' <param name="Sel">si non null permet de modifier la selection réelles</param>
    Friend Sub SelectionGeorefToSelectionReferencement(Optional Sel As Rectangle = Nothing)
        Dim Selection As Rectangle = If(Sel = Nothing, _SelectionGeoref, Sel)
        If Selection = Rectangle.Empty Then
            SelectionsReferencement(_NumSite) = RectangleD.Empty
        Else
            Dim PT0_Virtuel As Point = Selection.Location, PT2_Virtuel As Point = Point.Add(PT0_Virtuel, Selection.Size)
            Dim PT0_Reel As PointD = _SystemeType.ConvertirCoordonneesVirtuellesToReelles(PT0_Virtuel)
            Dim PT2_Reel As PointD = _SystemeType.ConvertirCoordonneesVirtuellesToReelles(PT2_Virtuel)
            SelectionsReferencement(_NumSite) = New RectangleD(PT0_Reel, PT2_Reel)
        End If
    End Sub
    ''' <summary> initialisation du regroupement </summary>
    ''' <param name="Sys"> Système cartographique associé au regroupement (Site, domtom, Echelle) </param>
    Private Sub New(Sys As SystemeCartographique, Cle As String)
        _Clef = Cle
        _SystemeType = Sys
        RepertoiresImportes = New List(Of String)(15)
        _Cartes = New List(Of Carte)(50)
        _NumSite = SiteDomTomToIndex(_SystemeType.SiteCarto, _SystemeType.DomTom)
    End Sub
    ''' <summary> Contient tous les répertoires dans lequel sont stockés les cartes de la collection pour éviter de les ré-importer </summary>
    Private RepertoiresImportes As List(Of String)
    ''' <summary> taille en unité virtuelle de la zone de sélection du regroupement en cours: Retour AffichePlanEchelle</summary>
    Private _SelectionGeoref As Rectangle
    Private OldSelectionReferencement As RectangleD
#End Region
#Region "Elements Regroupements"
    ''' <summary> initialisation des regroupements </summary>
    Shared Sub New()
        _Regroupements = New SortedList(Of String, Regroupement)
        'il y a 12 sites carto gérer par fcgp. 5 sites uniques + 7 domstoms associés au site DomTom et chaucun peut avoir ses regroupements et sa sélection 
        ReDim SelectionsReferencement(11)
    End Sub
    ''' <summary> taille en unité de reférencement de la zone de sélection du site carto </summary>
    Friend Shared Function SelectionReferencementSiteCarto(NumSite As Integer) As RectangleD
        Return SelectionsReferencement(NumSite)
    End Function
    ''' <summary> efface la sélection associée au site carto </summary>
    Friend Shared Sub EffacerSelectionReferencementSiteCarto(NumSite As Integer)
        SelectionsReferencement(NumSite) = RectangleD.Empty
    End Sub
    ''' <summary> liste des regroupements existants </summary>
    Friend Shared ReadOnly Property Regroupements As SortedList(Of String, Regroupement)
    ''' <summary> renvoie le nb de regroupements appartenants au même site dans la collection </summary>
    Friend Shared Function NbRegroupementsSiteCarto(NumSite As Integer) As Integer
        Dim NbEchelles As Integer = 0
        For Each R As KeyValuePair(Of String, Regroupement) In Regroupements
            If R.Value.NumSite = NumSite Then NbEchelles += 1
        Next
        Return NbEchelles
    End Function
    ''' <summary> Verifie si le fichier Georef est valide. Si Ok verifie que le regroupement correspondant n'existe pas. Si il n'existe pas 
    ''' on le crée et on charge les cartes du répertoire sinon on envoie un message </summary>
    ''' <param name="FichierGeorefInit"> Chemin de la carte d'initialisation. Le système doit être valide et non interpolée</param>
    ''' <param name="ClefRegroupement"> Clef du regroupement </param>
    ''' <param name="NbCartesAjout"> Nb de carte ajoutées à l'échelle </param>
    Friend Shared Function CreerRegroupement(FichierGeorefInit As String, ClefRegroupement As String, ByRef NbCartesAjout As Integer) As VerifierRegroupement
        'si l'échelle existe on renvoie un message d'erreur. L'utilisateur pourra toujour utiliser la fonction ajout d'un repertoire pour ajouter les cartes
        If _Regroupements.ContainsKey(ClefRegroupement) Then
            Return VerifierRegroupement.Regroupement_Déja_Existant
        End If
        Dim SystemeType As SystemeCartographique = Carte.RenvoyerSystemeCartographique(FichierGeorefInit)
        'dans le cas du regroupement, le système type est ramené à la projection principale du site carto associé, ce qui rend compatible les différentes projections de cartes compatibles
        Dim DatumPrincipal As Datums = ProjectionCartographique.DatumPrincipal(SystemeType.SiteCarto)
        If SystemeType.Projection.Datum <> DatumPrincipal Then
            SystemeType = New SystemeCartographique(SystemeType.IndiceSiteCarto, SystemeType.Niveau.Echelle, DatumPrincipal)
        End If
        'Création de la nouvelle échelle 
        Dim RegroupementActuel As New Regroupement(SystemeType, ClefRegroupement)
        'ajoute les cartes du répertoire
        CreerRegroupement = RegroupementActuel.AjouterCartes(FichierGeorefInit, NbCartesAjout)
        'si l'ajout des cartes c'est bien passé on ajoute le regroupement dans la liste
        If CreerRegroupement = VerifierRegroupement.OK Then
            _Regroupements.Add(RegroupementActuel._Clef, RegroupementActuel)
        End If
    End Function
    ''' <summary> Supprime l'échelle passée en paramètre et libère les ressources associées </summary>
    Friend Shared Sub SupprimerRegroupement(RegroupementActuel As Regroupement)
        RegroupementActuel._Cartes.Clear()
        RegroupementActuel._Cartes = Nothing
        RegroupementActuel.RepertoiresImportes.Clear()
        RegroupementActuel.RepertoiresImportes = Nothing
        _Regroupements.Remove(RegroupementActuel._Clef)
    End Sub
    ''' <summary> associé à la propriété SelectionReferencement. Sert de tampon pour l'ensemble SiteCarto-DomTom possible </summary>
    Private Shared ReadOnly SelectionsReferencement() As RectangleD
    ''' <summary> taille en pixel du formulaire AffichePlanRegroupement </summary>
    Private Shared TaillePlanRegroupement As Size
    ''' <summary>associé à la propriété AfficheBord</summary>
    Private Shared Affiche_Bords As Integer
    ''' <summary>associé à la propriété Affiche_PositionVise</summary>
    Private Shared RegionAffichage As Rectangle
#End Region
#Region "Formulaire d'affichage du plan de calpinage d'un regroupement"
    Private Class CalepinageRegroupement
        Private NumForme As Integer, CoefR As Double, Xorigine As Integer, Yorigine As Integer
        Private _RegroupementActuel As Regroupement, SelectionVirtuelle, SelectionPixel As Rectangle
        Private SiteCarto As SitesCartographiques, Datum As Datums, Echelle As Echelles, IndiceEchelle As Integer
        Friend WriteOnly Property RegroupementActuel As Regroupement
            Set(value As Regroupement)
                _RegroupementActuel = value
            End Set
        End Property
        ''' <summary> initialise le formulaire d'affichage du plan de l'échelle </summary>
        Private Sub CalepinageRegroupement_Load(sender As Object, e As EventArgs)
            Size = TaillePlanRegroupement
            'centre le formulaire sur le formulaire parent
            With Owner
                Location = New Point(.Location.X + (.Width - Width) \ 2, .Location.Y + (.Height - Height) \ 2)
            End With
            'récupération de l'echelle actuelle
            SelectionVirtuelle = _RegroupementActuel._SelectionGeoref
            'on ajoute les cartes
            FaireRectanglesCartes()
            Dim S = _RegroupementActuel.SystemeType
            SiteCarto = S.SiteCarto
            Datum = DatumSiteWeb(SiteCarto)
            Echelle = S.Niveau.Echelle
            IndiceEchelle = EchelleToSiteIndiceEchelle(SiteCarto, Echelle)
        End Sub
        Private Sub CalepinageRegroupement_FormClosed(sender As Object, e As FormClosedEventArgs)
            Dim S = SurfaceTravail
            If S <> SelectionPixel Then
                If S = Rectangle.Empty Then
                    _RegroupementActuel._SelectionGeoref = Rectangle.Empty
                Else
                    'il faut recalculer les coordonnées virtuelles car elles ont été modifiées à partir du plan de calepinage du regroupement
                    _RegroupementActuel._SelectionGeoref = New Rectangle(CInt((S.X - Xorigine) / CoefR),
                                                                     CInt((S.Y - Yorigine) / CoefR),
                                                                     CInt(S.Width / CoefR),
                                                                     CInt(S.Height / CoefR))
                End If
            End If
        End Sub
        ''' <summary>  Affichage sur le formulaire de l'ensemble des rectangles qui compose l'échelle et éventuellement de la visue  </summary>
        Private Sub CalepinageRegroupement_Paint(sender As Object, e As PaintEventArgs)
            AfficherRectangles(e.Graphics)
            'si le dessin de la position de la visue a été demandé, on l'affiche
            If Not RegionAffichage.IsEmpty Then
                'ajout d'un rectangle qui représente l'affichage de l'écran
                e.Graphics.DrawRectangle(New Pen(Color.Blue, 2), New Rectangle(Xorigine + CInt(RegionAffichage.X * CoefR),
                                                                               Yorigine + CInt(RegionAffichage.Y * CoefR),
                                                                               CInt(RegionAffichage.Width * CoefR),
                                                                               CInt(RegionAffichage.Height * CoefR)))
            End If
        End Sub
        ''' <summary> détermine si une carte est survolée et prépare la création ou la modification du rectangle de travail </summary>
        Private Sub CalepinageRegroupement_MouseDown(sender As Object, e As MouseEventArgs)
            AppuyerBoutonSouris(e.Location, ModifierKeys)
        End Sub
        ''' <summary>  affiche les différentes informations cartographiques concernant la carte survolée et redessine le plan du regroupement si besoin </summary>
        Private Sub CalepinageRegroupement_MouseMove(sender As Object, e As MouseEventArgs)
            'trouver le rectangle qui est survolé
            DeplacerSouris(e.Location)
            Select Case NumRectangleSurvol
                Case -1
                    'pas de rectangle survolé
                    RemplirCoordonneesCarte(-1)
                    'on efface de l'écran l'ancien rectangle survolé (il ne l'est plus)
                    If NumForme <> -1 Then Invalidate()
                Case 0
                    'rectangle de travail survolé
                    RemplirCoordonneesCarte(0)
                    Invalidate()'on redessine toujours le rectangle de travail sur un évènement MouseMove
                Case > 0
                    'carte survolée
                    If NumForme <> NumRectangleSurvol Then
                        RemplirCoordonneesCarte(NumRectangleSurvol)
                        'on affiche à l'écran le nouveau rectangle survolé
                        Invalidate()
                    End If
            End Select
            'on garde en mémoire le rectangle survolé
            NumForme = NumRectangleSurvol
            RemplirCoordonneesCurseur(e.Location)
        End Sub
        ''' <summary> En mode Surface un click sur le plan de calepinage ferme le formulaire et détermine
        ''' les nouvelles coordonnées virtuelles de l'affichage si demandé par l'utilisateur </summary>
        Private Sub CalepinageRegroupement_MouseUp(sender As Object, e As MouseEventArgs)
            RelacherBoutonSouris()
            'il y a eu un click sur le plan de calepinage.
            If ClickRectangle Then
                'La fermeture est demandée sur une carte existante cela indique que l'on veut changer la position de l'affichage
                _RegroupementActuel._PositionGeoref = New Point(CInt((e.X - Xorigine) / CoefR), CInt((e.Y - Yorigine) / CoefR))
                'on indique la carte survolée.
                _RegroupementActuel._NumCarteSelection = NumRectangleSurvol
                'on ferme le formulaire de calepinage
                Close()
            Else
                If NumRectangleSurvol = -1 Then
                    _RegroupementActuel._NumCarteSelection = NumRectangleSurvol
                    'click en dehors des cartes
                    Close()
                Else
                    'c'est la fin de création ou de modification du rectangle de travail
                    Invalidate()
                End If
            End If
        End Sub
        ''' <summary> gère les touches clavier envoyées au formulaire. La touche escape permet de sortir sans carte sélectionnée </summary>
        Private Sub CalepinageRegroupement_KeyDown(sender As Object, e As KeyEventArgs)
            Select Case e.KeyCode
                Case Keys.Escape
                    DialogResult = DialogResult.Cancel
                    _RegroupementActuel._NumCarteSelection = -1
                    Close()
                Case Keys.Enter
                    DialogResult = DialogResult.OK
                    _RegroupementActuel._NumCarteSelection = -1
                    Close()
                Case Keys.Delete, Keys.Back
                    CreerRectangleTravail(Rectangle.Empty)
                    Invalidate()
            End Select
        End Sub
        ''' <summary> détermine l'échelle et les rectangles qui composent la représentation de l'échelle sur le formulaire</summary>
        Private Sub FaireRectanglesCartes()
            Dim DimensionsPlanRegroupement As New Size(ClientSize.Width - Affiche_Bords * 2, ClientSize.Height - Affiche_Bords * 2 - CoordonneesCurseur.Height)
            'récupération de la surface de l'ensemble des cartes du regroupement et calcul du coefficient de mise à l'échelle dessin/regroupement
            Dim DimensionsRegroupement As Rectangle = _RegroupementActuel.RegionVirtuelle
            'si le positionnement de l'affichage sur le plan est demandé, il peut être ne dehors de l'emprise du regroupement
            If Not RegionAffichage.IsEmpty Then
                DimensionsRegroupement = Rectangle.Union(DimensionsRegroupement, RegionAffichage)
            End If
            ' si il y a une sélection à afficher sur le plan, elle peut être ne dehors de l'emprise du regroupement
            If SelectionVirtuelle <> Rectangle.Empty Then
                DimensionsRegroupement = Rectangle.Union(DimensionsRegroupement, SelectionVirtuelle)
            End If
            Dim R_X As Double = DimensionsPlanRegroupement.Width / DimensionsRegroupement.Width
            Dim R_Y As Double = DimensionsPlanRegroupement.Height / DimensionsRegroupement.Height
            If R_X > R_Y Then
                CoefR = R_Y
            Else
                CoefR = R_X
            End If
            'calcul des X et Y d'origine en pixels de la zone de dessin
            Xorigine = (DimensionsPlanRegroupement.Width - CInt(Math.Round(DimensionsRegroupement.Width * CoefR, 0))) \ 2 + CInt(Math.Round(-DimensionsRegroupement.X * CoefR, 0)) + Affiche_Bords
            Yorigine = (DimensionsPlanRegroupement.Height - CInt(Math.Round(DimensionsRegroupement.Height * CoefR, 0))) \ 2 + CInt(Math.Round(-DimensionsRegroupement.Y * CoefR, 0)) + Affiche_Bords

            'calcul de la selection
            If SelectionVirtuelle <> Rectangle.Empty Then
                SelectionPixel = New Rectangle(Xorigine + CInt(Math.Round(SelectionVirtuelle.X * CoefR, 0)),
                                               Yorigine + CInt(Math.Round(SelectionVirtuelle.Y * CoefR, 0)),
                                               CInt(Math.Round(SelectionVirtuelle.Width * CoefR, 0)),
                                               CInt(Math.Round(SelectionVirtuelle.Height * CoefR, 0)))
            End If
            'initialisation du module Objetrectangle avec le rectangle de travail en mode surface et la possibilité d'en avoir un
            'Le -3 correspond à la largeur minimum du rectangle de séléction, -24 correspond à la hauteur de la barre d'info plus 2 pixels de bordure
            Dim SurfaceDessin As New Rectangle(New Point(ClientRectangle.X + 3, ClientRectangle.Y + 3),
                                                   New Size(ClientRectangle.Width - 6, ClientRectangle.Height - 24 - 6))
            InitialiserRectangles(SurfaceDessin, Mode.Surface, SelectionPixel, True)
            For Each CarteActuelle In _RegroupementActuel.Cartes
                'calcul des dimensions de la carte en pixels de la zone de dessin
                Dim DimensionsCarte As Rectangle = CarteActuelle.RegionVirtuelle
                Dim X As Integer = Xorigine + CInt(Math.Round(DimensionsCarte.X * CoefR, 0))
                Dim Y As Integer = Yorigine + CInt(Math.Round(DimensionsCarte.Y * CoefR, 0))
                Dim L As Integer = CInt(Math.Round(DimensionsCarte.Width * CoefR, 0))
                Dim H As Integer = CInt(Math.Round(DimensionsCarte.Height * CoefR, 0))
                Dim Name As String = CarteActuelle.Nom
                'ajout dans la collection du rectangle représentant la carte
                AjouterRectangle(New Point(X, Y), New Point(X + L, Y + H), Name)
            Next
        End Sub
        ''' <summary> remplit la barre d'information concernant les coordonnées du pointeur de souris </summary>
        Private Sub RemplirCoordonneesCurseur(PtCurseur As Point)
            'affiche les coordonnées du curseur de souris en unité du monde réel
            Dim Pt = New Point(CInt((PtCurseur.X - Xorigine) / CoefR), CInt((PtCurseur.Y - Yorigine) / CoefR))
            CoordonneesCurseur.Text = CoordonneesReelles(Pt)
        End Sub
        ''' <summary> remplit la barre d'information concernant la carte survolée </summary>
        Private Sub RemplirCoordonneesCarte(NumCarte As Integer)
            If NumCarte > 0 Then
                Dim Nom = _RegroupementActuel._Cartes(NumCarte - 1).Nom
                Dim R = _RegroupementActuel._Cartes(NumCarte - 1).RegionVirtuelle

                Dim Pt0 As Point = R.Location
                Dim Pt2 As Point = Point.Add(Pt0, R.Size)
                CoordonneesCartes.Text = $"{Nom} : Pt0 {CoordonneesReelles(Pt0)}, Pt2 {CoordonneesReelles(Pt2)}"
            Else
                CoordonneesCartes.Text = "Pas de carte sélectionnée"
            End If
        End Sub

        Private Function CoordonneesReelles(PtPixels As Point) As String
            Dim LabelCoordonnees As String = ""
            Select Case RegrouperSettings.INDICE_TYPE_COORDONNEES
                Case 0
                    LabelCoordonnees = CoordMouseGrilleText(CoordMouseGrille(PtPixels, SiteCarto, Echelle))
                Case 1
                    LabelCoordonnees = CoordMousseDDText(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum)
                Case 2
                    LabelCoordonnees = CoordMouseDMSText(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum)
                Case 3
                    LabelCoordonnees = CoordMouseUtmWGS84(CoordMouseGrille(PtPixels, SiteCarto, Echelle), Datum)
                Case 4
                    LabelCoordonnees = CoordMousePointG(PtPixels, IndiceEchelle).ToString()
            End Select
            Return LabelCoordonnees
        End Function
    End Class
    Partial Private Class CalepinageRegroupement
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
            AddHandler Load, AddressOf CalepinageRegroupement_Load
            AddHandler FormClosed, AddressOf CalepinageRegroupement_FormClosed
            AddHandler Paint, AddressOf CalepinageRegroupement_Paint
            AddHandler MouseDown, AddressOf CalepinageRegroupement_MouseDown
            AddHandler MouseMove, AddressOf CalepinageRegroupement_MouseMove
            AddHandler MouseUp, AddressOf CalepinageRegroupement_MouseUp
            AddHandler KeyDown, AddressOf CalepinageRegroupement_KeyDown
        End Sub
        Private Sub InitialiseComposants()
            components = New Container()
            CoordonneesCartes = New Label()
            CoordonneesCurseur = New Label()
            SuspendLayout()
            '
            'CoordonneesCurseur
            '
            CoordonneesCurseur.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            CoordonneesCurseur.BorderStyle = BorderStyle.FixedSingle
            CoordonneesCurseur.FlatStyle = FlatStyle.Flat
            CoordonneesCurseur.Location = New Point(-1, 701)
            CoordonneesCurseur.Name = "Info1"
            CoordonneesCurseur.Size = New Size(257, 23)
            CoordonneesCurseur.TabIndex = 0
            'CoordonneesCurseur.Text = "Nb Tuiles : 1000"
            CoordonneesCurseur.TextAlign = ContentAlignment.MiddleCenter
            '
            'CoordonneesCartes
            '
            CoordonneesCartes.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            CoordonneesCartes.BorderStyle = BorderStyle.FixedSingle
            CoordonneesCartes.FlatStyle = FlatStyle.Flat
            CoordonneesCartes.Location = New Point(255, 701)
            CoordonneesCartes.Name = "Info2"
            CoordonneesCartes.Size = New Size(446, 23)
            CoordonneesCartes.TabIndex = 1
            'CoordonneesCartes.Text = "Nb Tuiles à supprimer : 1000"
            CoordonneesCartes.TextAlign = ContentAlignment.MiddleCenter


            'Form1
            '
            AutoScaleMode = AutoScaleMode.None
            AutoSize = False
            AutoSizeMode = AutoSizeMode.GrowOnly
            ClientSize = New Size(700, 723)
            ControlBox = False
            Controls.Add(CoordonneesCurseur)
            Controls.Add(CoordonneesCartes)
            FormBorderStyle = FormBorderStyle.FixedToolWindow
            MaximizeBox = False
            MinimizeBox = False
            Name = "Form1"
            SizeGripStyle = SizeGripStyle.Hide
            StartPosition = FormStartPosition.Manual
            DoubleBuffered = True
            ResumeLayout(False)
        End Sub

        Private CoordonneesCartes As Label
        Private CoordonneesCurseur As Label
    End Class
#End Region
End Class