''' <summary> ObjetRectangle
'''Cette classe gère un ensemble de rectangles qui représente des objets de forme similaire comme des cartes ou des tuiles en ayant pour but
'''l'affichage d'un plan de mapping et l'interaction avec celui-ci.
'''Il s'agit principalement d'une liste de rectangles simples et d'un rectangle de travail dont l'index réservé dans la liste est 0
'''
'''Il y a 2 modes de fonctionnement. 
'''Le premier est utilisé pour la représentation d'un regroupement de cartes ou de tuiles et le rectangle de travail est utilisé
'''pour déterminé une surface en relation avec les dimensions du regroupement des rectangles simples. Il s'agit du mode_Surface
'''Le deuxième est utilsé pour la représentation d'un regroupement de cartes ou de tuiles et le rectangle de travail est utilisé
'''pour sélectionner un ou des rectangles simples. Il s'agit du mode_Selection
'''
'''1er mode ou mode_Surface  
'''Le rectangle simple est obligatoirement créé par programmation. Il n'a qu'un seul état et 2 représentations : Survolé ou non survolé.
'''Il n 'a qu'une seule zone d'interaction qui est sa surface.
'''Le click(modificateur de clavier non pris en compte) sur la zone d'interaction produit une action qui renvoie simplement le numéro d'index du rectangle simple dans la liste.
'''Le rectangle de travail peut être créé par programmation ou avec la souris sur le surface d'affichage. Dans les 2 cas il est modifiable avec la souris.
'''Il n 'à qu'un seul état et 2 représentations : Créé et non survolé ou survolé, encours de création ou encours de modification.
'''Il a 8 zones d'interaction. Ces zones permettent de détecter le survol du rectangle de travail et la modification de celui-ci lorsque la zone est accrochée
'''par la souris. La création se fait obligatoirement par un click qui marque une des 4 zones d'interaction en coins et un déplacement vers le coin opposé du
'''rectangle de travail à créer. 
'''Les zones d'interaction sont les 4 coins et les 4 cotés du rectangle.
'''
'''2ème mode ou mode_Selection :
'''Le rectangle simple est obligatoirment créé par programmation. Il a 2 états : Sélectionné ou pas sélectionné et 2 représentations par états : Survolé ou
'''non survolé. Le click renvoie une action fonction du modificateur de clavier. 
'''Pas de modificateur : inverse l 'état sélectionné, 
'''Shift: passe à l'état sélectionné,
'''Ctrl: passe à l'état non sélectionné,
'''Alt: lance une action qui est à la charge de l'appelant (délégate).
'''Le rectangle de travail est obligatoirement créé avec la souris sur la surface d'affichage. Il est  modifiable uniquement pendant la création et il n'est pas sauvegardé une fois créé.
'''La fin de la création du rectangle de travail sélectionne l'ensemble des rectangles simples dans son emprise en fonction du modificateur de clavier.
'''Pas de modificateur : inverse l 'état sélectionné, 
'''Shift: passe à l'état sélectionné,
'''Ctrl: passe à l'état non sélectionné. 
'''Alt : au choix </summary> 

Friend Module ObjetRectangle
#Region "Variables et fonctions partagées"
    ''' <summary> indique que la forme est sélectionnée pour faire une action (modification dimensions ou rien) </summary>
    Private ActionTravail As TypeActionLibelle
    ''' <summary> sert pour le calcul de DX et DY du mode action (différence entre 2 positions du pointeur de souris) </summary>
    Private Pt_Depart As Point
    ''' <summary> constante valeur par défaut du pinceau de base( hors survol et action): vert et 1 pixels de large </summary>
    Private ReadOnly s_p_Base As New Pen(Color.LimeGreen, 1)
    ''' <summary> constante valeur par défaut du pinceau de base spécial(survol ou action): rouge et 1 pixels de large</summary>
    Private ReadOnly s_p_Special As New Pen(Color.Red, 2)
    ''' <summary>valeur par defaut de la demi largeur d'une zone de sensibilité : 1 pixels par défault</summary>
    Private Const s_d_Sensibilite As Integer = 1
    ''' <summary> index réservé pour le rectangle de travail </summary>
    Private Const Travail As Integer = 0
    ''' <summary> constante valeur par défaut de la brosse semi transparente pour le remplissage du rectangle simple si il est sélectionné </summary>
    Private ReadOnly s_b_SemiTransBrush As New SolidBrush(Color.FromArgb(64, 128, 128, 128))
    ''' <summary>constante valeur par défaut de la police d'affichage des infos</summary>
    Private ReadOnly s_d_Police As New Font("Segoe UI", 12.0!, FontStyle.Regular, GraphicsUnit.Pixel)
    ''' <summary> collection des rectangles pour cette session. L'indice 0 est réservé pour le rectangle de travail </summary>
    Private ReadOnly Rectangles As New List(Of RectangleSimple)
    ''' <summary> Concerne le rectangle de travail. 1 quand le 1er coin est connu
    ''' 2 quand le coin opposé est connu. Le rectangle de travail est affichable. </summary>
    Private FlagCreationTravail As Integer
    ''' <summary> modifieur de clavier lors de l'évenement Down de la souris (debut click) </summary>
    Private ModifierAction As Keys
    ''' <summary> flag indiquant si le rectangle de travail est autorisé </summary>
    Private FlagRectangleTravail As Boolean
    ''' <summary> Action à lancer lors d'un click sur un rectangle simple avec le modificateur Alt et le mode Sélection </summary>
    Private _ActionRectangleSimple As Func(Of Integer, DialogResult)
    ''' <summary> Surface du rectangle de travail exprimée en pixel. </summary>
    Friend ReadOnly Property SurfaceTravail As Rectangle
        Get
            Return Rectangles(Travail).Surface
        End Get
    End Property
    ''' <summary> Numéro du rectangle qui a une zone action en cours de survol par la souris </summary>
    Friend ReadOnly Property NumRectangleSurvol As Integer
    ''' <summary> surface dans laquelle le dessin des Rectangles est autorisée. Généralement correspond au rectangle du Graphics
    ''' d'affichage mais pour certaines Rectangle peut être ramenée à l'univers virtuel et pas la surface d'affichage </summary>
    Friend ReadOnly Property SurfaceAutorise As Rectangle
    ''' <summary> indique le mode d'utilisation du rectangle de travail </summary>
    Friend ReadOnly Property ModeTravail As Mode
    ''' <summary> renvoi le nb de rectangles simples </summary>
    Friend ReadOnly Property NbRectangles As Integer
        Get
            Return Rectangles.Count - 1
        End Get
    End Property
    ''' <summary> renvoi le nb de rectangles simples dont la propriété IsSelected est false </summary>
    Friend ReadOnly Property NbRectanglesNotSelected As Integer
        Get
            Return Rectangles.Count - 1 - NbRectanglesSelected
        End Get
    End Property
    ''' <summary> renvoi le nb de rectangles simples dont la propriété IsSelected est true </summary>
    Friend ReadOnly Property NbRectanglesSelected As Integer
    ''' <summary> renvoi un tableau de boolean correspondant à la propriété IsSelected des rectangles simples. IndexBase 0 </summary>
    Friend ReadOnly Property RectanglesIsSelected As Boolean()
        Get
            'on dimensionne le tableau en fonction de nb de rectangles simples
            Dim Ret(Rectangles.Count - 2) As Boolean
            For IndexRectangle As Integer = 1 To Rectangles.Count - 1
                'on livre un tableau à index base 0
                Ret(IndexRectangle - 1) = Rectangles(IndexRectangle).IsSelected
            Next
            Return Ret
        End Get
    End Property
    ''' <summary> renvoi un tableau qui contient l'index des rectangles qui ont la propriété IsSelected à true. IndexBase 0 </summary>
    Friend ReadOnly Property RectanglesSelected As Integer()
        Get
            Dim T(NbRectanglesSelected - 1) As Integer
            Dim Cpt, CptRectangle As Integer
            For CptRectangle = 1 To Rectangles.Count - 1
                If Rectangles(CptRectangle).IsSelected Then
                    T(Cpt) = CptRectangle - 1
                    Cpt += 1
                End If
            Next
            Return T
        End Get
    End Property

    ''' <summary> permet d'indiquer à l'appelant qu'il y a eu un click sur un rectangle simple </summary>
    Friend ReadOnly Property ClickRectangle As Boolean
    ''' <summary> définit l'action qui doit être réalisée lors d'un click sur un rectangle simple (addresse d'une fonction à 1 paramètre integer) </summary>
    Friend WriteOnly Property ActionRectangleSimple As Func(Of Integer, DialogResult)
        Set(value As Func(Of Integer, DialogResult))
            _ActionRectangleSimple = value
        End Set
    End Property

    ''' <summary> permet de connaitre à partir d'une seule instruction (manip collective) le rectangle qui est détecté </summary>
    ''' <param name="Pt"> Position du curseur de souris</param>
    ''' <param name="ModifiersKey"> Etat des touches shift, ctrl et alt lors de l'appui sur le bouton de souris </param>
    ''' <remarks> Cette action est appelée dans l'évenement Mouse_Down du controle support du plan de calepinage </remarks>
    Friend Sub AppuyerBoutonSouris(Pt As Point, ModifiersKey As Keys)
        Pt_Depart = Pt
        ModifierAction = ModifiersKey
        _ClickRectangle = False
        'les rectangles simples ne sont pas concernés par l'évènement down si le rectangle de travail est créé
        If _NumRectangleSurvol > Travail AndAlso FlagCreationTravail = 2 Then
            Exit Sub
        End If
        'si le rectangle de travail n'est pas créé on vérifie avant de le créer qu'il ne s'agit pas d'un click sur un rectangle simple 
        'mais d'un appui avec déplacement d'où un armement qui sera enlevé si l'évènement Click suit directement au lieu d'un évenement Move
        If _NumRectangleSurvol <> Travail AndAlso FlagCreationTravail = 0 AndAlso FlagRectangleTravail Then
            FlagCreationTravail = 1
            ActionTravail = TypeActionLibelle.Modification
            Exit Sub
        End If
        'si le rectangle travail est créé on indique que l'on rentre dans une action de changement de taille du rectangle
        If _NumRectangleSurvol = Travail AndAlso FlagCreationTravail = 2 Then
            Rectangles(Travail).RealiserActionSouris(Pt)
            ActionTravail = TypeActionLibelle.Modification
        End If
    End Sub
    ''' <summary> renvoie le numéro de l'instance qui a terminée l'action(manip collective)</summary>
    ''' <remarks> Cette action est appelée dans l'évenement Mouse_Up du controle support du plan de calepinage </remarks>
    Friend Sub RelacherBoutonSouris()
        'si le flag concernant la création du rectangle de travail est mis on l'enlève car il indique qu'il n'y a pas eu de déplacement de la souris
        If FlagCreationTravail = 1 Then
            FlagCreationTravail = 0
            ActionTravail = TypeActionLibelle.Aucune
        End If
        If _NumRectangleSurvol >= Travail Then
            Rectangles(_NumRectangleSurvol).FinirActionSouris()
        End If
    End Sub
    ''' <summary> permet à partir d'une seule instruction(manip collective) de definir l'action à réaliser l'ensemble des formes de la collection. </summary>
    ''' <param name="Pt"> position du curseur de souris </param>
    ''' <remarks> Cette action est appelée dans l'évenement Mouse_Move du controle support du plan de calepinage </remarks>
    Friend Sub DeplacerSouris(Pt As Point)
        'si il n'y a pas d'action en cours, il faut savoir si un des rectangles est ou est suceptible d'être survolé.
        If ActionTravail = TypeActionLibelle.Aucune Then
            'Cas du rectangle de travail créé
            If _NumRectangleSurvol = Travail AndAlso Rectangles(Travail).ZonesAction(Rectangles(Travail).ZoneActionEnCours).Contains(Pt) Then
                'le rectangle de travail est déjà survolé avec la même zone d'action
                'et comme il est prioritaire il n'y a plus rien à faire
                Exit Sub
            Else
                'on cherche en premier si le rectangle de travail peut être survolé car il est prioritaire mais il faut qu'il soit autorisé
                If FlagRectangleTravail AndAlso Rectangles(Travail).TrouverZoneActionSurvol(Pt) Then
                    'si oui on rénitialise le rectangle survolé sauf si c'est déjà le rectangle de travail
                    If _NumRectangleSurvol > Travail Then
                        Rectangles(_NumRectangleSurvol).ZoneActionEnCours = ZonesInterAction.Aucune
                        Rectangles(_NumRectangleSurvol).IsOver = False
                    End If
                    'on indique que le rectangle de travail est survolé
                    _NumRectangleSurvol = Travail
                    Exit Sub
                Else
                    'sinon est ce qu'un rectangle simple est déja survolé avec la même zone d'action. Il n'y a qu'une seule zone d'action
                    If _NumRectangleSurvol > Travail AndAlso Rectangles(_NumRectangleSurvol).ZonesAction(Rectangles(_NumRectangleSurvol).ZoneActionEnCours).Contains(Pt) Then
                        'dans ce cas il 'y a plus rien à faire
                        Exit Sub
                    Else
                        'sinon il faut trouver parmi l'ensemble des rectangles simples si il y en a un qui est survolé par le curseur de souris
                        For Cpt = 1 To Rectangles.Count - 1
                            'il ne peut y avoir qu'un seul rectangle qui prend la main donc on sort au premier trouvé.
                            'Cela peut bloquer un rectangle plus petit ayant un index supérieur. Il ne sera jamais survolé
                            If Rectangles(Cpt).TrouverZoneActionSurvol(Pt) Then
                                'si oui on rénitialise le rectangle survolé
                                If _NumRectangleSurvol >= Travail Then
                                    Rectangles(_NumRectangleSurvol).ZoneActionEnCours = ZonesInterAction.Aucune
                                    Rectangles(_NumRectangleSurvol).IsOver = False
                                End If
                                'on indique que l'un des rectangle simple est survolé
                                _NumRectangleSurvol = Cpt
                                Exit Sub
                            End If
                        Next
                        'il n'y a pas de rectangle survolé donc on l'indique et on RAZ éventuellement l'ancien
                        If _NumRectangleSurvol >= Travail Then
                            Rectangles(_NumRectangleSurvol).ZoneActionEnCours = ZonesInterAction.Aucune
                            Rectangles(_NumRectangleSurvol).IsOver = False
                        End If
                        _NumRectangleSurvol = -1
                    End If
                End If
            End If
        Else
            'TypeAction = TypeActionPossible.Modification car c'est la seule possible avec le rectangle de travail
            'Le click est suivi d'un mouvement de la souris donc on considère que l'on crée le rectangle de travail
            If FlagCreationTravail = 1 Then
                Dim DX = Pt.X - Pt_Depart.X
                Dim DY = Pt.Y - Pt_Depart.Y
                'on attend que le déplacement soit bien marqué pour choisir la zone d'action de départ
                If DX = 0 OrElse DY = 0 Then Exit Sub
                Dim Zone As ZonesInterAction
                'determine le sens de déplacement de la souris afin de connaitre le coin de départ du rectangle de travail
                If DX >= 0 Then
                    If DY >= 0 Then
                        Zone = ZonesInterAction.Coin_BasDroit
                    Else
                        Zone = ZonesInterAction.Coin_HautDroit
                    End If
                Else
                    If DY <= 0 Then
                        Zone = ZonesInterAction.Coin_HautGauche
                    Else
                        Zone = ZonesInterAction.Coin_BasGauche
                    End If
                End If
                'le rectangle de travail va être créé, on marque le 1er coin
                Rectangles(Travail).DebuterActionSouris(Pt_Depart, Zone)
                'éventuellement on reinitialise le rectangle simple qui était survolé
                If _NumRectangleSurvol > Travail Then
                    Rectangles(_NumRectangleSurvol).ZoneActionEnCours = ZonesInterAction.Aucune
                    Rectangles(_NumRectangleSurvol).IsOver = False
                End If
                'on indique que le rectangle de travail est le rectangle survolé et qu'il est créé au sens affichable
                _NumRectangleSurvol = Travail
                FlagCreationTravail = 2
            Else
                ' le rectangle de travail est créé, il faut réaliser l'action demandée, c'est à dire le redimensionnement de celui-ci
                If FlagRectangleTravail Then Rectangles(Travail).RealiserActionSouris(Pt)
            End If
        End If
    End Sub
    ''' <summary> permet à partir d'une seule instruction(manip collective) de dessiner l'ensemble des rectangles de la collection. </summary>
    ''' <param name="Graph">zone de dessin</param>
    ''' <remarks> Cette action est appelée dans l'évenement Paint du controle support du plan de calepinage </remarks> 
    Friend Sub AfficherRectangles(Graph As Graphics)
        'affiche tous les rectangles simples qui ne sont pas survolés
        For Cpt As Integer = 1 To Rectangles.Count - 1
            If Not Rectangles(Cpt).IsOver Then
                Rectangles(Cpt).Dessiner(Graph)
            End If
        Next
        'affiche tous les rectangles simples qui sont survolés après afin qu'ils soient visibles
        For Cpt As Integer = 1 To Rectangles.Count - 1
            If Rectangles(Cpt).IsOver Then
                Rectangles(Cpt).Dessiner(Graph)
            End If
        Next
        'si le rectangle de travail existe on l'affiche par dessus tous les autres
        If FlagCreationTravail > 1 Then
            Rectangles(Travail).Dessiner(Graph)
        End If
        If _NumRectangleSurvol <> -1 Then
            CurseurEncours = Rectangles(_NumRectangleSurvol).TypeCurseurEncours
        Else
            CurseurEncours = Curseurs.Defaut
        End If
    End Sub
    ''' <summary> Permet d'instancier un rectangle simple avec les 2 principaux paramètres obligatoires (Point de début ou hautgauche  
    ''' et point de fin ou point basdroit. Ces deux points permettent de définir le rectangle </summary>
    ''' <param name="PtHG">Obligatoire : Point Haut Gauche du rectangle</param>
    ''' <param name="PtBD">Obligatoire : Point Bas Droit du rectangle</param>
    ''' <param name="Info"> Optionnel : si vide rien ne sera affiché lors du survol du rectangle simple </param>
    ''' <param name="IsSelect"> Optionnel : indique si le rectangle est considéré comme selectionné </param>
    Friend Sub AjouterRectangle(PtHG As Point, PtBD As Point, Optional Info As String = "", Optional IsSelect As Boolean = False)
        Rectangles.Add(New RectangleSimple(PtHG, PtBD, Info, Rectangles.Count, IsSelect))
        If IsSelect Then _NbRectanglesSelected += 1
    End Sub
    ''' <summary> vide la collection et remet à zéro les variables partagées par l'ensembles des rectangles </summary>
    ''' <param name="SurfaceDessin"> indique la surface sur laquelle les rectangles doivent être contenus. Cela permet de limiter
    ''' l'extension du rectangle de sélection, c'est 3 pixels des bords </param>
    ''' <param name="SurfaceTravail" > indique la surface du rectangle de travail </param>
    ''' <param name="Utilite"> indique le mode d'utilisation souhaité pour le rectangle de travail </param>
    ''' <param name="FlagTravail"> indique que l'on veut ou non un rectangle de travail </param>
    Friend Sub InitialiserRectangles(SurfaceDessin As Rectangle, Utilite As Mode, SurfaceTravail As Rectangle, Optional FlagTravail As Boolean = True)
        Rectangles.Clear()
        _NumRectangleSurvol = -1
        _SurfaceAutorise = SurfaceDessin
        _ModeTravail = Utilite
        FlagRectangleTravail = FlagTravail
        _NbRectanglesSelected = 0
        _ClickRectangle = False
        ActionTravail = TypeActionLibelle.Aucune
        'on crée le rectangle de travail à l'indice 0
        CreerRectangleTravail(SurfaceTravail)
    End Sub
    ''' <summary> initialisation du rectangle de travail. Indice 0 dans la collection sans passer par l'initialisation des rectangles simples </summary> 
    ''' <param name="Selection" > indique la surface du rectangle de sélection </param>
    Friend Sub CreerRectangleTravail(Selection As Rectangle)
        Dim RectangleSelection As New ObjetRectangle_Travail(Selection.Location, New Point(Selection.Right, Selection.Bottom), "", 0)
        'on ajoute ou remplace le rectangle selection
        If Rectangles.Count > 0 Then
            Rectangles(Travail) = RectangleSelection
        Else
            Rectangles.Add(RectangleSelection)
        End If
        FlagCreationTravail = If(Selection = Rectangle.Empty, 0, 2)
    End Sub
    ''' <summary> calcule si le rectangle doit être sélectionné ou pas et le nb de rectangles simples sélectionnés </summary>
    ''' <param name="R"> Rectangle simple qui doit être modifié </param>
    ''' <param name="Alt"> ce que l'on veut comme sélection si le modificateur est Alt </param>
    Private Sub ModifierSelection(R As RectangleSimple, Optional Alt As Boolean = True)
        Dim AncienSelected = R.IsSelected
        Dim Selected As Boolean
        Select Case ModifierAction
            Case Keys.Shift
                Selected = True
            Case Keys.Control
                Selected = False
            Case Keys.Alt
                Selected = Alt
            Case Else 'keys.none 
                Selected = Not AncienSelected
        End Select
        If AncienSelected <> Selected Then
            _NbRectanglesSelected += If(AncienSelected, -1, +1)
        End If
        R.IsSelected = Selected
    End Sub
#End Region
#Region "Classe Rectangle Simple"
    Private Class RectangleSimple
#Region "Variables de l'instance"
        ''' <summary>Flag indiquant si le rectangle est survolé par le curseur de souris. Sert pour le rendu de l'affichage</summary>
        Friend IsOver As Boolean
        ''' <summary>renvoie true si le controle rectangle est selectionné </summary>
        Friend IsSelected As Boolean
        ''' <summary> tableau des zones de sensibilité : surface. Doit être redimensionné dans la phase d'initialisation du rectangle et mis à jour par la procédure DetermineZones</summary>
        Friend ZonesAction() As Rectangle
        ''' <summary>indique la zone action du rectangle qui est survolée par le curseur. index des 2 tableaux précédents</summary>
        Friend ZoneActionEnCours As ZonesInterAction
        ''' <summary>sommet Haut- Gauche du rectangle</summary>
        Protected PointHautGauche As Point
        ''' <summary>sommet Bas-Droit du rectangle</summary>
        Protected PointBasDroit As Point
        ''' <summary>variable associée à la propriété PinceauBase du rectangle</summary>
        Protected PinceauBase As Pen
        ''' <summary>variable associée à la propriété PinceauSpecial du rectangle action et survol</summary>
        Protected PinceauSpecial As Pen
        ''' <summary> tableau des zones de sensibilité : curseurs. Doit être redimensionné dans la phase d'initialisation du rectangle et mis à jour par la procédure DetermineZones</summary>
        Protected ZonesActionCurseurs() As Curseurs
        ''' <summary> nb de zones de sensibilité sur une longueur ou largeur du contrôle rectangle </summary>
        Protected NbZonesSensibleLH As Integer
        ''' <summary> renvoie la 1/2 largeur de la zone de sensibilité (1 pixels par défault). 1 de chaque coté de la ligne </summary> 
        Protected Sensibilite As Integer
        ''' <summary>variable de la police d'affichage des infos</summary>
        Private Police As Font
        ''' <summary> Flag indiquant si les infos associées au rectangle doivent être affichées quand le rectangle est survolé</summary>
        Private AfficherInfo As Boolean
        ''' <summary>Informations à afficher lors du survol du rectangle</summary>
        Private Information As String
#End Region
#Region "Procédures et propriétés Instance"
        ''' <summary> renvoie la surface du rectangle  </summary> 
        Friend Overridable ReadOnly Property Surface() As Rectangle
            Get
                Return New Rectangle(PointHautGauche, New Size(Largeur, Hauteur))
            End Get
        End Property
        ''' <summary>renvoie le curseur de la zone d'action en cours</summary>
        Friend Overridable ReadOnly Property TypeCurseurEncours As Curseurs
            Get
                Return ZonesActionCurseurs(ZoneActionEnCours)
            End Get
        End Property
        ''' <summary>réalise l'action de l'instance.  Si on arrive dans cette procédure, il y a forcément une zone de sensibilité de l'instance qui est selectionnée</summary>
        ''' <param name="Pt">position du pointeur de souris</param>
        ''' <remarks> est appelé suite à l'évenement Mousemove </remarks>
        Friend Overridable Sub RealiserActionSouris(Pt As Point)
            'il n'y a pas d'action pour le rectangle simple
        End Sub
        ''' <summary> Permet le passage au mode 'Actif' de l'instance. L'action est déterminée par le type du rectangle et éventuellement par la zone d'action survolée </summary>
        ''' <param name="Pt">point correspondant au pointeur de souris au départ de l'action</param>
        ''' <remarks> est appelé suite à l'évenement MouseDOWN </remarks>
        Friend Overridable Sub DebuterActionSouris(Pt As Point, Zone As ZonesInterAction)
            'il n'y a pas d'action pour le rectangle de base
        End Sub
        ''' <summary> finalise l'action de l'instance </summary>
        ''' <remarks> est appelé suite à l'évenement MouseUP </remarks>
        Friend Overridable Sub FinirActionSouris()
            'on renvoie toujours vrai pour un click sur rectangle simple
            _ClickRectangle = True
            If _ModeTravail = Mode.Selection Then
                'en mode sélection le modificateur alt indique qu'il faut lancer l'action
                If ModifierAction = Keys.Alt Then
                    If _ActionRectangleSimple(_NumRectangleSurvol) = DialogResult.OK Then
                        'si enter on déselectionne le rectangle simple
                        ModifierSelection(Me, False)
                    Else
                        'sinon on reconduit la sélection du rectangle simple
                        ModifierSelection(Me, Rectangles(_NumRectangleSurvol).IsSelected)
                    End If
                Else
                    'c'est le modifieur de clavier qui force ou non la sélection du rectangle simple
                    ModifierSelection(Me)
                End If
            End If
        End Sub
        ''' <summary> constructeur de rectangle simple </summary>
        Friend Sub New(PtHG As Point, PtBD As Point, Info As String, Num As Integer, IsSelect As Boolean)
            'il n'y a qu'une seule zone de survol possible pour le rectangle de base
            NbZonesSensibleLH = 1
            ReDim ZonesAction(1)
            ReDim ZonesActionCurseurs(1)
            Initialiser(PtHG, PtBD, Info, Num)
            'couleur de la forme
            PinceauBase = s_p_Base
            PinceauSpecial = s_p_Special
            IsSelected = IsSelect
        End Sub
        ''' <summary> dessine le rectangle sur la surface d'affichage passée en paramètre </summary>
        ''' <param name="Graph">surface d'affichage</param>
        ''' <remarks>est appelé indirectement par l'évenement paint du controle support de l'affichage </remarks>
        Friend Sub Dessiner(Graph As Graphics)
            Dim Pinceau As Pen = PinceauBase, R As Rectangle = Surface
            If IsOver Then Pinceau = PinceauSpecial
            'on dessine la forme avec le pinceau qui va bien
            Graph.DrawRectangle(Pinceau, R)
            'si il est sélectionné on le prend en compte
            If IsSelected Then Graph.FillRectangle(s_b_SemiTransBrush, R)
            'si l'affichage de l'info associée est demandé et si le rectangle est survolé
            If AfficherInfo = True AndAlso IsOver AndAlso _NumRectangleSurvol > Travail Then
                'trouve la surface de la chaine d'information
                Dim S As SizeF = Graph.MeasureString(Information, Police)
                Dim X As Single, Y As Single
                'on affiche l'info centré par raport au rectangle
                X = Surface.X + (R.Width - S.Width) / 2
                Y = R.Y + (R.Height - S.Height) / 2
                Graph.DrawString(Information, Police, Brushes.Black, X, Y)
            End If
        End Sub
        ''' <summary>permet la détection de la zone d'action du rectangle</summary>
        ''' <param name="Pt">position du curseur de souris</param>
        Friend Function TrouverZoneActionSurvol(Pt As Point) As Boolean
            ZoneActionEnCours = ZonesInterAction.Aucune
            IsOver = False
            'on cherche la zone d'action qui contient le curseur de souris
            For Cpt As Integer = 1 To ZonesAction.Length - 1
                If ZonesAction(Cpt).Contains(Pt) Then
                    ZoneActionEnCours = CType(Cpt, ZonesInterAction)
                    IsOver = True
                    Exit For
                End If
            Next
            Return IsOver
        End Function
        ''' <summary> initalise une partie des variables associées au rectangle simple </summary>
        ''' <param name="PtHG">Point haut gauche exprimé en pixel de la zone client d'affichage</param>
        ''' <param name="PtBD">Point bas droit exprimé en pixel de la zone client d'affichage</param>
        ''' <param name="Info"> l'information à affiché lors du survol du rectangle par le pointeur de souris</param>
        ''' <param name="Num"> numéro du rectangle </param>
        Protected Sub Initialiser(PtHG As Point, PtBD As Point, Info As String, Num As Integer)
            ZoneActionEnCours = ZonesInterAction.Aucune
            Police = s_d_Police
            Sensibilite = s_d_Sensibilite
            If PtHG = Point.Empty AndAlso PtBD = Point.Empty Then
                'on considère que le rectangle n'est pas créé et qu'il faudra l'initialiser après coup par programation.
                'Pas implémenté pour le rectangle de base
                If Type = "Rectangle_Travail" Then Exit Sub
                Throw New Exception("Les points du rectangle simple doivent être définis avant de demander sa création")
            Else
                'le point de début ne peut pas être en dehors du graphics. 
                If PtHG.X < 0 Then PtHG.X = 0
                If PtHG.Y < 0 Then PtHG.Y = 0
                'la longueur et la hauteur ne peuvent pas être inférieures à distance mini
                If PtBD.X - PtHG.X < DistanceMini Then
                    PtBD.X = PtHG.X + DistanceMini
                End If
                If PtBD.Y - PtHG.Y < DistanceMini Then
                    PtBD.Y = PtHG.Y + DistanceMini
                End If
            End If
            PointHautGauche = PtHG
            PointBasDroit = PtBD
            'ajout des infos de la forme dans la collection Infos
            If Info = "" Then
                'par défaut le type et le numID
                Information = Type & "-" & Num.ToString("00")
                AfficherInfo = False
            Else
                Information = Info
                AfficherInfo = True
            End If
            'aucune zone action initiale
            ZonesAction(ZonesInterAction.Aucune) = Rectangle.Empty
            ZonesActionCurseurs(ZonesInterAction.Aucune) = Curseurs.Defaut
            'on determine les différentes zones d'action et les curseurs de souris qui vont avec
            CalculerZonesAction()
        End Sub
        ''' <summary>met à jour différentes zones d'action du rectangle </summary>
        ''' <remarks>doit être appelé à la création du rectangle si il est créé et à la fin d'une action qui change la taille ou la postion du rectangle</remarks>
        Protected Overridable Sub CalculerZonesAction()
            'il n'y a qu'une zone d'action pour le rectangle de base
            ZonesAction(ZonesInterAction.Zone_Unique) = Surface
            ZonesActionCurseurs(ZonesInterAction.Zone_Unique) = Curseurs.Defaut
        End Sub
        ''' <summary> représente la longueur ou la hauteur minimale du rectangle afin qu'il puisse contenir le nombre de zones de sensibiité prévues </summary>
        Protected ReadOnly Property DistanceMini As Integer
            Get
                Return (NbZonesSensibleLH) * Sensibilite
            End Get
        End Property
        ''' <summary> renvoie la largeur du rectangle </summary> 
        Protected ReadOnly Property Largeur() As Integer
            Get
                Return PointBasDroit.X - PointHautGauche.X
            End Get
        End Property
        ''' <summary> renvoie la hauteur du rectangle </summary> 
        Protected ReadOnly Property Hauteur() As Integer
            Get
                Return PointBasDroit.Y - PointHautGauche.Y
            End Get
        End Property
        ''' <summary> renvoie le type controle_Reference </summary> 
        Protected Overridable ReadOnly Property Type() As String
            Get
                Return "Rectangle_Simple"
            End Get
        End Property
        ''' <summary> uniquement pour l'héritage de la classe </summary> 
        Protected Sub New()
        End Sub
#End Region
    End Class
#End Region
#Region "Types Imbriqués"
    ''' <summary>definit les différents types possibles d'action pour les formes </summary>
    Private Enum TypeActionLibelle As Integer
        ''' <summary> quand il n'y a pas d'action en cours </summary>
        Aucune = 0
        ''' <summary> pour indiquer qu'il s'agit d'une modification du rectangle de travail </summary>
        Modification = 1
    End Enum
    ''' <summary> definit les zones de sensibilité possibles </summary>
    Private Enum ZonesInterAction As Integer
        ''' <summary> quand il n'y a pas de controle rectangle survolé ou en cours d'action move </summary>
        Aucune = 0
        ''' <summary> Zone unique pour les rectangles simples </summary>
        Zone_Unique = 1
#Disable Warning CA1069 ' Les valeurs enum ne doivent pas être dupliquées
        ''' <summary> modification du rectangle de travail avec le coin haut gauche </summary>
        Coin_HautGauche = 1 'pour les 8 zones qui recouvre l'ensemble du rectangle de sélection lors de la création ou de l'agrandissement
#Enable Warning CA1069 ' Les valeurs enum ne doivent pas être dupliquées
        ''' <summary> modification du rectangle de travail avec le coin bas droit </summary>
        Coin_BasDroit = 2
        ''' <summary> modification du rectangle de travail avec le coin haut droit </summary>
        Coin_HautDroit = 3
        ''' <summary> modification du rectangle de travail avec le coin bas gauche </summary>
        Coin_BasGauche = 4
        ''' <summary> modification du rectangle de travail avec le bord du haut </summary>
        Bord_Haut = 5
        ''' <summary> modification du rectangle de travail avec le bord du bas </summary>
        Bord_Bas = 6
        ''' <summary> modification du rectangle de travail avec le bord gaughe </summary>
        Bord_Gauche = 7
        ''' <summary> modification du rectangle de travail avec le bord droit </summary>
        Bord_Droit = 8
    End Enum
    ''' <summary> definit les differents modes possibles pour utiliser le rectangle de travail </summary>
    Friend Enum Mode
        Aucun = 0
        ''' <summary> Le rectangle de travail est utilisé pour derterminer une surface. Il n'interagit pas avec les rectangles simples </summary>
        Surface = 1
        ''' <summary> Le rectangle de travail est utilisé pour sélectionner les rectangles simples </summary>
        Selection = 2
    End Enum
#End Region
#Region "Class Rectangle_Travail"
    Private Class ObjetRectangle_Travail
        Inherits RectangleSimple
        ''' <summary>Permet de calculer 8 zones action du rectangle : 4 coins et 4 lignes </summary>
        Protected Overrides Sub CalculerZonesAction()
            Dim Sensib_2_Plus As Integer = Sensibilite * 2 + 1
            Dim Sensib_Plus As Integer = Sensibilite + 1
            Dim LargeurLigne As Integer = Largeur - Sensib_2_Plus, HauteurLigne As Integer = Hauteur - Sensib_2_Plus

            'calcul des 2 autres coins du rectanglereference pour faciliter les différents calculs des zones de sensibilité
            Dim _PointHautDroit As New Point(PointHautGauche.X + Largeur, PointHautGauche.Y)
            Dim _PointBasGauche As New Point(PointHautGauche.X, PointHautGauche.Y + Hauteur)
            'Coin Haut Gauche
            ZonesAction(ZonesInterAction.Coin_HautGauche) = New Rectangle(PointHautGauche.X - Sensibilite, PointHautGauche.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus)
            ZonesActionCurseurs(ZonesInterAction.Coin_HautGauche) = Curseurs.FRefAgrandir_HG_BD
            'Coin Bas Gauche
            ZonesAction(ZonesInterAction.Coin_BasGauche) = New Rectangle(_PointBasGauche.X - Sensibilite, _PointBasGauche.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus)
            ZonesActionCurseurs(ZonesInterAction.Coin_BasGauche) = Curseurs.FRefAgrandir_HD_BG
            'Coin Haut Droit
            ZonesAction(ZonesInterAction.Coin_HautDroit) = New Rectangle(_PointHautDroit.X - Sensibilite, _PointHautDroit.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus)
            ZonesActionCurseurs(ZonesInterAction.Coin_HautDroit) = Curseurs.FRefAgrandir_HD_BG
            'Coin Bas Droit
            ZonesAction(ZonesInterAction.Coin_BasDroit) = New Rectangle(PointBasDroit.X - Sensibilite, PointBasDroit.Y - Sensibilite, Sensib_2_Plus, Sensib_2_Plus)
            ZonesActionCurseurs(ZonesInterAction.Coin_BasDroit) = Curseurs.FRefAgrandir_HG_BD
            'Lignes Horizontales
            ZonesAction(ZonesInterAction.Bord_Haut) = New Rectangle(PointHautGauche.X + Sensib_Plus, PointHautGauche.Y - Sensibilite, LargeurLigne, Sensib_2_Plus)
            ZonesActionCurseurs(ZonesInterAction.Bord_Haut) = Curseurs.FRefDeplacerHorizontal

            ZonesAction(ZonesInterAction.Bord_Bas) = New Rectangle(_PointBasGauche.X + Sensib_Plus, _PointBasGauche.Y - Sensibilite, LargeurLigne, Sensib_2_Plus)
            ZonesActionCurseurs(ZonesInterAction.Bord_Bas) = Curseurs.FRefDeplacerHorizontal
            'Lignes Verticales
            ZonesAction(ZonesInterAction.Bord_Gauche) = New Rectangle(PointHautGauche.X - Sensibilite, PointHautGauche.Y + Sensib_Plus, Sensib_2_Plus, HauteurLigne)
            ZonesActionCurseurs(ZonesInterAction.Bord_Gauche) = Curseurs.FRefDeplacerVertical

            ZonesAction(ZonesInterAction.Bord_Droit) = New Rectangle(_PointHautDroit.X - Sensibilite, _PointHautDroit.Y + Sensib_Plus, Sensib_2_Plus, HauteurLigne)
            ZonesActionCurseurs(ZonesInterAction.Bord_Droit) = Curseurs.FRefDeplacerVertical
        End Sub
        ''' <summary> initialise les coins du rectangle de sélection lors de sa création sur le plan d'affichage </summary>
        ''' <param name="Pt"> position de la souris </param>
        Friend Overrides Sub DebuterActionSouris(Pt As Point, Zone As ZonesInterAction)
            PointHautGauche = Pt
            PointBasDroit = Pt
            ZoneActionEnCours = Zone
            CurseurEncours = TypeCurseurEncours
            IsOver = True
        End Sub
        ''' <summary> réalise l'action de l'instance modification de taille. </summary>
        ''' <param name="Pt">position du pointeur de souris</param>
        Friend Overrides Sub RealiserActionSouris(Pt As Point)
            CurseurEncours = TypeCurseurEncours
            'si le pointeur de souris n'est plus dans la surface autorisée on arrête l'action en cours
            If Not _SurfaceAutorise.Contains(Pt) Then Exit Sub
            'calcul de Pt0 ou Pt2 en fonction du déplacement de la souris
            Select Case ZoneActionEnCours
                Case ZonesInterAction.Coin_HautGauche
                    PointHautGauche = Pt
                Case ZonesInterAction.Coin_BasDroit
                    PointBasDroit = Pt
                Case ZonesInterAction.Coin_HautDroit
                    PointHautGauche.Y = Pt.Y
                    PointBasDroit.X = Pt.X
                Case ZonesInterAction.Coin_BasGauche
                    PointHautGauche.X = Pt.X
                    PointBasDroit.Y = Pt.Y
                Case ZonesInterAction.Bord_Haut
                    PointHautGauche.Y = Pt.Y
                Case ZonesInterAction.Bord_Bas
                    PointBasDroit.Y = Pt.Y
                Case ZonesInterAction.Bord_Gauche
                    PointHautGauche.X = Pt.X
                Case ZonesInterAction.Bord_Droit
                    PointBasDroit.X = Pt.X
            End Select
            CalculerZonesAction()
            If _ModeTravail = Mode.Selection Then
                'on trouve les rectangles simples survolés par la surface du rectangle de travail afin de 
                'pouvoir les visualiser comme des rectangles simples survolés
                Dim St = Rectangles(Travail).Surface
                For IndexRectangle As Integer = 1 To Rectangles.Count - 1
                    Dim R = Rectangles(IndexRectangle)
                    R.IsOver = R.Surface.IntersectsWith(St)
                Next
            End If
        End Sub
        ''' <summary> Permet de revenir au mode 'Inactif' de l'instance. C'est l'équivalent du click </summary>
        Friend Overrides Sub FinirActionSouris()
            IsOver = False 'on dessine le rectangle en mode inactif
            ZoneActionEnCours = ZonesInterAction.Aucune
            ActionTravail = TypeActionLibelle.Aucune

            CurseurEncours = TypeCurseurEncours
            If _ModeTravail = Mode.Surface Then
                'cas où le rectangle de travail est créé
                If Largeur < DistanceMini Then
                    PointBasDroit.X = PointHautGauche.X + DistanceMini
                End If
                If Hauteur < DistanceMini Then
                    PointBasDroit.Y = PointHautGauche.Y + DistanceMini
                End If
            Else
                Dim R = Surface
                'la surface de travail n'a pas pour vocation à rester affiché sur le plan
                CreerRectangleTravail(Rectangle.Empty)
                'il n'y a pas de modification spécifique associée au modifieur alt dans le mode Sélection
                If ModifierAction = Keys.Alt Then ModifierAction = Keys.None
                'sélectionner les rectangles simples en fonction du modifier
                For IndexRectangle As Integer = 1 To Rectangles.Count - 1
                    Dim Rsimple = Rectangles(IndexRectangle)
                    If R.IntersectsWith(Rsimple.Surface) Then ModifierSelection(Rsimple)
                    'le marquage du survol doit être effacé sauf pour le dernier rectangle simple qui a été marqué
                    Rsimple.IsOver = IndexRectangle = _NumRectangleSurvol
                Next
                'le rectangle de travail agit comme un click sur les rectangles simples survolés 
                _ClickRectangle = True
            End If
        End Sub
        Friend Sub New(PtHG As Point, PtBD As Point, Info As String, Num As Integer)
            NbZonesSensibleLH = 3
            ReDim ZonesAction(8)
            ReDim ZonesActionCurseurs(8)
            Initialiser(PtHG, PtBD, Info, Num)
            'couleur du rectangle de sélection en affichage normal
            PinceauBase = New Pen(Color.Black, 2) 's_p_Base
            'couleur du rectangle de sélection en mode création ou redimensionnent
            If _ModeTravail = Mode.Surface Then
                PinceauSpecial = New Pen(Color.Magenta, 1)
            Else
                PinceauSpecial = New Pen(Color.Orange, 2)
            End If
        End Sub
        ''' <summary>variable associée à la propriété type de la forme</summary>
        Protected Overrides ReadOnly Property Type() As String
            Get
                Return "Rectangle_Travail"
            End Get
        End Property
    End Class
#End Region
End Module