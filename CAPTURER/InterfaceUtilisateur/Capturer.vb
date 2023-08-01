''' <summary> formulaire principal de l'application FCGP_Capturer </summary>
Friend NotInheritable Class Capturer
#Region "Champs et propriétés"
    ''' <summary> libellés des sites et Domstoms pour le remplissage de la liste déroulante de sélection des sites </summary>
    Private ReadOnly LibellesSitesDomTom() As String = New String() {"Géofoncier", "Suisse Mobile", "Réunion", "Martinique", "Guadeloupe", "St Martin",
                                                                     "Guyane", "Mayotte", "St Barthélémy", "CycleOSM", "OpenTopo Map", "Ign Espagnol"}
    ''' <summary> Liste des différentes unités pour les coordonnées du pointeur de souris. Evite une variable static interne méthode </summary>
    Private ReadOnly CoordType As String() = New String() {"DD WGS84", "DMS WGS84", "UTM WGS84", "TUILE Couche N°00"}
    ''' <summary> conserve le nb de crans de la molette de la souris. Evite une variable static interne méthode </summary>
    Private MemoireDeltaSouris As Integer
    ''' <summary> Liste des différentes actions possibles sur l'affichage </summary>
    Private Enum Actions As Integer
        Aucune = 0          ' rien ou c'est la trace qui a la main
        DefinirPoints       ' Début de la définition des 2 points de capture avec la touche Alt + le click et déplacement de la souris
        FinirDefinirPoints  ' fin de la définition des 2 points de capture avec la touche Alt + le click et déplacement de la souris
        DeplacerCarte       ' la carte est en cours de déplacement
    End Enum
    Private NomBaseCacheTuile As String
    ''' <summary> action encours sur l'affichage </summary>
    Private Action As Actions
    ''' <summary> Pas en pixels utilisé par les touches de déplacement pixels avec les modifiers </summary>
    Private ReadOnly PasTouchesPixels() As Integer = New Integer() {1, 5, 25, 0, 125}
    ''' <summary> Pas en % de la hauteur ou de la largeur de la surface d'affichage, utilisé par les touches de déplacement écran avec les modifiers </summary>
    Private ReadOnly PasTouchesAffichage() As Double = New Double() {1.0, 0.25, 0.5, 0, 0.75}
    ''' <summary> flag indiquant que le programme est en mode initialisation. Certains évenement ne seront pas traité ou modifié </summary>
    Private FlagInit As Boolean = True
    ''' <summary> flag indiquant qu'il s'agit d'un évenement déclencher par programmation et pas par IU </summary>
    Private FlagEchelle As Boolean
    ''' <summary> Taille maximale des tampons d'affichage. sert à dimensionner la reserve d'octets nécessaires en mémoire </summary>
    Private TailleMaxTamponAffichage As Size
    ''' <summary> tampon réservé à l'affichage </summary>
    Private ReserveOctetsAffichage As SharedPinnedByteArray
    ''' <summary> permet de connaitre la position du pointeur de souris afin de calculer les coordonnées pixels du pointeur et de calculer un delta de position du pointeur </summary>
    Private PosMouse As Point
    ''' <summary>altitude à la position du pointeur de souris </summary>
    Private AltitudePosMouse As Short
    ''' <summary> flag indiquant si les coordonnées à traiter sont associées au pointeur de souris ou au centre de l'affichage </summary>
    Private FlagPosMouse As Boolean
    ''' <summary> flag indiquant si une carte est encours de création </summary>
    Private FlagCapture As Boolean
    ''' <summary> flag indiquant qu'il y a eu une action AllerA de réalisée </summary>
    Private FlagRevenir As Boolean
    ''' <summary> Dans le cas d'une action AllerA le site de destination concerné. </summary>
    Private SiteAllerA As SitesCartographiques
    ''' <summary> Dans le cas d'un changement de site avec le menu contextuel de la visue la position du centre du site sélectionné. </summary>
    Private PositionSiteAllerA As PointG
    ''' <summary> Coordonnees de sauvegarde lors d'une action AllerA </summary>
    Private CoordonneesRevenir As PointD
    ''' <summary> label sur lequel est positionné le curseur de la souris </summary>
    Private LabelEncours As Label
    ''' <summary> nom du Pointlabel sur lequel est le curseur de la souris </summary>
    Private NomPointEncours As String
    ''' <summary> différentes variables pour le dessin des points et de la surface de capture </summary>
    Private PinceauPoint, PinceauEchelle_1, PinceauEchelle_2, PinceauLimites As Pen, BrosseOk, BrosseNotOk, BrosseEtiquette As SolidBrush, EtiquettePoint, DemiEpaisseur As Size
    ''' <summary> Indique la surface maximale de décalage dans la visue </summary>
    Private SurfaceDecalage As Rectangle
    ''' <summary> surface de capture définit à partir des 2 points de capture </summary>
    Private SurfaceTraitement As SurfaceTuiles
    ''' <summary> flag qui indique si la surface de traitement est ok pour une création de carte, une suppression de tuiles ou une vue 3D (à venir) </summary>
    Private SurfaceTraitementIsOk As Boolean
    ''' <summary> indique qu'un menu associé à un label est ouvert </summary>
    Private FlagContextMenuShow As Boolean
    ''' <summary> combox qui est ouverte lors d'un click sur un label de choix possibles (Sites, Echelle ou TypeCoordonnées) </summary>
    Private ComboxDropDown As ComboBox
    ''' <summary> Limites en coordonnées virtuelles pour lesquelles le site est définit </summary>
    Friend LimitesVirtuellesSite As Rectangle
    ''' <summary> location ou Pt0 de l'affichage en Coordonnées bureau. définit comme size pour éviter les transtypages </summary>
    Private AffichageOffsetEcran As Size
    ''' <summary> Centre de l'affichage de l'affichage en Coordonnées bureau </summary>
    Private CentreAffichage As Point
#End Region
#Region "Evènements de Capturer, des 3 listes déroulantes et des procédures associées"
    ''' <summary> initialisation de l'application </summary>
    Private Sub Capturer_Load(sender As Object, e As EventArgs)
        If Not InitialiserBaseApplication(Me, "Core 9.200 VB", "01/05/2023") Then
            Close()
            Exit Sub
        End If
        TitreInformation = "Information FCGP_Capturer"
        'indique aux threads hors UI la procédure à appeler pour afficher un message d'information ou une erreur
        FormHorsUI = New Action(AddressOf AfficherInfomationsErreurs)

        If Not InitialiserSettings("9.000") Then
            Close()
            Exit Sub
        End If
        If Not InitialiserCommun() Then
            Close()
            Exit Sub
        End If
        If Not InitialiserCapturer() Then
            Close()
        End If

        'sélectionne l'index dans la liste déroulante des Sites et DomsToms avec le dernier site visité
        ListeChoixSites.SelectedIndex = CapturerSettings.INDICE_SITE
        If FlagAborted Then
            'l'initialisation de l'affichage s'est mal passé on ferme l'application
            Dim Ex As New Exception("Erreur inconnue lors de l'initialisation du site de capture" & CrLf &
                                    "Fermeture de l'application")
            AfficherErreur(Ex, "Y1H3")
            Close()
        Else
            FlagInit = False
        End If
    End Sub
    ''' <summary> verifie si on peut sortir correctement du programme </summary>
    Private Sub Capturer_FormClosing(sender As Object, e As FormClosingEventArgs)
        Dim NbDemandes = If(Serveur IsNot Nothing, Serveur.NbDemandesAffichage, 0)
        If NbDemandes > 0 Then 'cas de l'affichage en cours de mise à jour
            MessageInformation = "Le formulaire sera fermé quand toutes " & CrLf & "les demandes de tuiles seront terminées." & CrLf &
                                 $"{NbDemandes} demande{If(NbDemandes > 1, "s", "")} de tuiles {If(NbDemandes > 1, "sont", "est")} en attente."
            AfficherInformation()
            e.Cancel = True
            Exit Sub
        End If
        If FlagCapture Then 'cas de la carte en cours de création
            MessageInformation = "Vous ne pouvez pas quitter l'application" & CrLf &
                                 "tant qu'une carte est en cours de création."
            AfficherInformation()
            e.Cancel = True
        End If
    End Sub
    ''' <summary> sauvegarde de l'environement de la visue et libération des ressources </summary>
    Private Sub Capturer_FormClosed(sender As Object, e As FormClosedEventArgs)
        If FlagAborted Then
            MessageInformation = "Problème dans l'initialisation de l'application FCGP_Capturer" & CrLf &
                                 "Veuillez le signaler à " & Contact & CrLf &
                                 "L'application va être fermée"
        ElseIf Affichage IsNot Nothing Then
            'on sauvegarde toutes les paramètres concernant le site actuel
            If IsSettingsOk Then
                TRK.CloturerTraces()
                CapturerSettings.LOCATION_CENTRE = Affichage.CentreAffichage
                CapturerSettings.Ecrire()
                CapturerSettings.EcrirePosition()
            End If
        End If
        CompacterCache()
        CloturerAltitudes()
        CloturerSettings()
        CloturerCommun()
        CloturerCapturer()
    End Sub
    ''' <summary> marque les touches de direction pour pouvoir gèrer le déplacement de la carte </summary>
    Private Sub Capturer_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        Select Case e.KeyCode
            Case Keys.Left To Keys.Down, Keys.PageUp To Keys.Home
                e.IsInputKey = True
        End Select
    End Sub
    ''' <summary> gestion des actions déclenchées par les touches du clavier </summary>
    Private Sub Capturer_KeyDown(sender As Object, e As KeyEventArgs)
        'on ne prend pas en compte l'événement si une action est déjà lancée
        If Action = Actions.Aucune Then
            If TouchesMenuTrace(e) Then Exit Sub
            If TouchesMenuVisue(e) Then Exit Sub
            If TouchesMenuApp(e) Then Exit Sub
            If TouchesMenuPoints(e) Then Exit Sub
            If TouchesDeplacementCarte(e) Then Exit Sub
            If TouchesDeplacementCurseur(e) Then Exit Sub
            'touches du formulaire
            Select Case e.KeyCode
                Case Keys.Escape
                    'ferme le menu qui est ouvert ou la liste de choix ouverte. La fermeture effective du menu est faite par
                    'l'événement ContextMenu_Closed provoqué par DroppedDown
                    If ComboxDropDown IsNot Nothing Then ComboxDropDown.DroppedDown = False
                Case Keys.F11
                    If e.Modifiers = Keys.Alt Then
                        AppliquerLimitesSite()
                    ElseIf e.Modifiers = Keys.None Then
                        If WindowState = FormWindowState.Maximized Then
                            WindowState = FormWindowState.Normal
                        Else
                            WindowState = FormWindowState.Maximized
                        End If
                    End If
            End Select
            'on ne fait pas suivre les touches inutilsées
            e.SuppressKeyPress = True
        End If
    End Sub
    ''' <summary> l'action DeplacerCarte n'est pas concernée par l'événement KeyUp </summary>
    Private Sub Capturer_KeyUp(sender As Object, e As KeyEventArgs)
        If Action = Actions.DefinirPoints Then
            'le bouton gauche de la souris a été relevé en 1er, on ne peut pas finir l'action
            FinirActionDefinirPoints()
        ElseIf Action = Actions.FinirDefinirPoints Then
            'la touche menu a été relevé en premier on peut fini l'action
            Action = Actions.Aucune
            TrouverCurseurCarte()
        ElseIf Action <> Actions.DeplacerCarte AndAlso TRK.IsTraceEncoursModeEdition Then
            'on transmet à la trace l'évenement, il faut réaliser l'ActionEncours
            If TRK.TraceEncours.ToucheUp(e.KeyCode) Then
                'la trace a pris l'événement à son compte
                SurfaceAffichageTuile.Invalidate()
            End If
        End If
    End Sub
    ''' <summary> gestion des déplacements du formulaire et redimensionnent du formulaire </summary>
    Private Sub Capturer_LocationChanged(sender As Object, e As EventArgs)
        If Not FlagInit Then
            VisueRectangle = SurfaceAffichageTuile.RectangleToScreen(SurfaceAffichageTuile.ClientRectangle)
            AffichageOffsetEcran = CType(VisueRectangle.Location, Size)
        End If
    End Sub
    ''' <summary> met à jour la visue quand le site change à partir de la liste déroulante ou à l'ouverture du programme </summary>
    Private Sub ListeChoixSites_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelSites.Text = ListeChoixSites.SelectedItem.ToString()
        'si ce n'est pas l'ouverture du programme
        If Not FlagInit Then
            'il faut sauver la position centre visue et des autres données du site actuel avant d'effectuer le changement
            CapturerSettings.LOCATION_CENTRE = Affichage.CentreAffichage
            'on change de site donc on ferme la trace encours
            If TRK.IsTraceEncours Then
                'd'abord on sauvegarde son nom
                CapturerSettings.NOM_TRACE = TRK.TraceEncours.Nom
                TRK.FermerTraceEncours(False)
            Else
                CapturerSettings.NOM_TRACE = ""
            End If
            CapturerSettings.EcrirePosition()
            CapturerSettings.INDICE_SITE = ListeChoixSites.SelectedIndex
            'et supprime le serveur web associé à l'ancien site de capture
            Serveur.Dispose()
        Else
            'on crée le support d'affichage
            Affichage = New AffichageCarte(ReserveOctetsAffichage, TailleMaxTamponAffichage, PartagerSettings.COULEUR_VISUE)
            'si l'initialisation de l'affichage ne s'est pas bien passée on sort
            If Not Affichage.IsOk Then Exit Sub
            'pour le site on garde les valeurs liées à l'initialistion du settings c'est à dire le dernier visité
        End If
        'on lit les paramètres enregistrés du nouveau site 
        CapturerSettings.LirePosition()
        'Si le changement de site est du à la liste déroulante du menu contextuel de la visue il faut modifier la position et l'échelle
        If SiteAllerA > SitesCartographiques.Aucun Then
            CapturerSettings.INDICE_ECHELLE = PositionSiteAllerA.IndiceEchelle
            CapturerSettings.LOCATION_CENTRE = PositionSiteAllerA.Location
            SiteAllerA = SitesCartographiques.Aucun
        End If
        GererMenuRevenir(False)
        'on créé le serveur associé au nouveau site de capture
        Serveur = New ServeurCarto(Affichage)
        If Not Serveur.IsOk Then Exit Sub
        'on initialise l'affichage (dimensions et IU)
        InitialiserAffichage()
        'fin de l'initialisation de FCGP_Capturer, tout c'est bien passé
        FlagAborted = False
    End Sub
    ''' <summary> met à jour la visue quand l'echelle change à partir de la liste déroulante ou de la souris </summary>
    Private Sub ListeChoixEchelles_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim Delta As Integer, PositionCoordonnees As Point
        If Not FlagEchelle Then
            'changement par la liste déroulante ou souris wheel. On récupère le point à afficher au centre de l'affichage
            PositionCoordonnees = If(FlagPosMouse, PosMouse, CentreAffichage)
            'et l'écart entre l'indice actuel et demandé. Toujours 1 ou -1 pour la souris wheel
            Delta = ListeChoixEchelles.SelectedIndex - CapturerSettings.INDICE_ECHELLE
            'on indique la nouvelle échelle
            CapturerSettings.INDICE_ECHELLE = ListeChoixEchelles.SelectedIndex
        End If
        'on maj les variables qui dépendent de l'échelle
        Affichage.CoucheFond = Serveur.CouchesLayer(0)(CapturerSettings.INDICE_ECHELLE)
        Affichage.Echelle = NiveauDetailCartographique.SiteIndiceEchelleToEchelle(CapturerSettings.SITE, CapturerSettings.INDICE_ECHELLE)
        Affichage.CoucheFondIspentes = Serveur.IsIndiceEchellesPentes(CapturerSettings.INDICE_ECHELLE)
        LimitesVirtuellesSite = RegionGrilleToRegionPixels(Serveur.Limites, Serveur.SiteCarto, Affichage.Echelle)
        SysCartoEncours = New SystemeCartographique(CapturerSettings.INDICE_SITE, Affichage.Echelle, Serveur.Datum)
        ListeChoixTypeCoordonnees.Items(4) = "TUILE Couche N°" & Affichage.CoucheFond.ToString("00")
        If Not FlagEchelle Then
            'on recalcule le monde virtuel de l'éventuelle trace en cours
            TRK.ChangerSiteEchelleTraces(CapturerSettings.SITE, CapturerSettings.DOMTOM, Affichage.Echelle, True)
            'copie l'ancien tampon en position et facteur pour faire patienter pendant la demande de remplissage du tampon 
            GererChangementCouche(Delta, PositionCoordonnees)
        End If
        'on recalcule la surface de traitement pour savoir si elle encore ok
        GererIUMenuPts()
        LabelEchelles.Text = ListeChoixEchelles.SelectedItem.ToString()
        FlagEchelle = False
    End Sub
    ''' <summary> change l'affichage des coordonnées du pointeur de souris </summary>
    Private Sub ListeChoixTypeCoordonnees_SelectedIndexChanged(sender As Object, e As EventArgs)
        CapturerSettings.INDICE_TYPE_COORDONNEES = ListeChoixTypeCoordonnees.SelectedIndex
        AfficherCoordonnees()
    End Sub
    ''' <summary> Initialise l'affichahe lors d'un changement de site de capture </summary>
    Private Sub InitialiserAffichage()
        'on met à jour la liste des types de coordonnées qui dépend du datum du site
        MettreAjourTypeCoordonnees()
        ListeChoixTypeCoordonnees.SelectedIndex = CapturerSettings.INDICE_TYPE_COORDONNEES
        'on met à jour l'affichage sur l'IU'on met à jour la liste déroulante des échelles qui dépend du site
        MettreAJourListeEchelles()
        'MAJ des variables qui dépendent de l'échelle du site
        FlagEchelle = True
        ListeChoixEchelles.SelectedIndex = CapturerSettings.INDICE_ECHELLE
        FlagEchelle = False
        'configuration du menu contextuel de la visue
        GererMenuVisue()
        'eventuellement chargement de la trace associée au site de capture
        TRK.ChargerTraceCaptureSettings(Affichage.Echelle)
        GererMenuTrace()
        'MAJ des variables de l'affichage qui dépendent de la taille et du point central à afficher
        If FlagInit Then
            'si c'est l'ouverture du programme, on initialise l'affichage à partir de l'évenement resize à 80% de la taille de l'écran
            DimensionnerAffichage()
        Else
            'cas du changement de site carto où il faut demander explicitement la réinitialisation de l'affichage
            FlagInit = True
            GererChangementTaille()
            FlagInit = False
        End If
    End Sub
    ''' <summary> gère le menus contextuel de l'affichage (click droit sur la visue) en fonction du site </summary>
    Private Sub GererMenuVisue()
        'flag indiquant que le site peut renvoyer directement sur un site OSM
        Dim FlagChangeSite As Boolean = IsNationalToEurope(CapturerSettings.SITE)
        MenuVisueSeparateur_2.Visible = FlagChangeSite
        'on interdit l'évenement de se produire
        ChoisitSitesOSM.Visible = False
        ChoisitSitesOSM.SelectedIndex = CapturerSettings.INDICE_OSM
        ChoisitSitesOSM.Visible = FlagChangeSite

        AfficheDessinEchelle.Text = ChangerTextBascule(False)

        'flag indiquant que le site peut afficher une couche des pentes
        Dim FlagPentes = Serveur.IsPentes
        AfficheCouchePentes.Text = ChangerTextBascule(True)
        ChoisitCoefAlphaPentes.SelectedIndex = CapturerSettings.INDICE_COEF_ALPHA_PENTES
        AfficheCouchePentes.Visible = FlagPentes
        ChoisitCoefAlphaPentes.Visible = FlagPentes
        Dim CoefAlphaPentes = CoefsAlphasPentes(ChoisitCoefAlphaPentes.SelectedIndex)
        ImageAttributsPentes.SetColorMatrix(New ColorMatrix With {.Matrix33 = CoefAlphaPentes}, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
        'flag indiquant que le site à plusieurs couches possible d'affichage (2 ou 3)
        Dim FlagFondPlan As Boolean = Serveur.IsFondsPlan
        If FlagFondPlan Then
            ChoisitFondsPlan.Items.Clear()
            ChoisitFondsPlan.Items.AddRange(Serveur.TypesFondsPlan())
            ChoisitFondsPlan.SelectedIndex = CapturerSettings.INDICE_FOND_PLAN
        End If
        MenuVisueSeparateur_1.Visible = FlagFondPlan
        ChoisitFondsPlan.Visible = FlagFondPlan
    End Sub
    ''' <summary> mise à jour des items de la liste déroulante des échelles </summary>
    Private Sub MettreAJourListeEchelles()
        ListeChoixEchelles.Items.Clear()
        ListeChoixEchelles.Items.AddRange(Serveur.EchellesLayer(0))
    End Sub
    ''' <summary> mise à jour de la liste déroulante des types de coordonnées du pointeur de souris </summary>
    Private Sub MettreAjourTypeCoordonnees()
        ListeChoixTypeCoordonnees.Items.Clear()
        ListeChoixTypeCoordonnees.Items.Add(DatumsLibelles(Serveur.Datum))
        ListeChoixTypeCoordonnees.Items.AddRange(CoordType)
    End Sub
    ''' <summary> Verifie toutes les 15 secondes que la connection internet n'est pas tombée </summary>
    Private Async Sub ConnectedInternet_Tick(sender As Object, e As EventArgs) 'Handles ConnectedInternet.Tick
        Await TesterConnectionInternetAsync()
        LabelConnexionInternet.BackColor = If(IsConnected, Color.YellowGreen, Color.Tomato)
        LabelConnexionInternet.Refresh()
    End Sub
    ''' <summary> initialise les différentes variables privées </summary>
    Private Function InitialiserCapturer() As Boolean
        Text = NumFCGP 'mise à jour du titre du formulaire
        'initialisation des différents curseurs utilisés par FCGP
        InitialiserCurseurs(SurfaceAffichageTuile, Cursor)
        'initialisation des fichiers altitudes et de la connexion si besoin
        If AltitudesSettings.IS_ALTITUDE Then
            InitialiserListeFichiersAltitudes()
            InitialiserConnexionAltitudes()
        End If
        PinceauEchelle_1 = New Pen(Color.Black, 4)
        PinceauEchelle_2 = New Pen(Color.Black, 2)
        PinceauPoint = New Pen(Color.Red, 2)
        PinceauLimites = New Pen(Color.FromArgb(96, Color.DarkCyan), 10)
        BrosseOk = New SolidBrush(Color.FromArgb(96, Color.YellowGreen))
        BrosseNotOk = New SolidBrush(Color.FromArgb(64, Color.Tomato))
        BrosseEtiquette = New SolidBrush(Color.FromArgb(128, Color.White))
        FontEtiquette = New Font("Segoe UI", 14.0!, FontStyle.Bold, GraphicsUnit.Pixel)
        EtiquettePoint = SurfaceAffichageTuile.CreateGraphics.MeasureString("Pt0", FontEtiquette).ToSize
        DemiEpaisseur = New Size(1, 0)
        SurfaceDecalage = New Rectangle(Point.Empty, New Size(NbPixelsTuile, NbPixelsTuile))
        'dimensionnement du tampon d'affichage en fonction de la résolution maximale des écrans et du nb de tuiles serveur pour le recouvrir
        Dim SizeTuile As New Size(PixelToNumTile(MaxBoundsEcran.Width + NbPixelsTuile, False) + 1,
                                  PixelToNumTile(MaxBoundsEcran.Height + NbPixelsTuile, False) + 1)
        TailleMaxTamponAffichage = New Size(StrideImage(SizeTuile.Width * NbPixelsTuile),
                                            SizeTuile.Height * NbPixelsTuile)
        'la mémoire liée à l'affichage est en dehors de la réserve commune. 2 tampons Pour la couche par défaut
        ReserveOctetsAffichage = New SharedPinnedByteArray(TailleMaxTamponAffichage.Width, TailleMaxTamponAffichage.Height * 2, Nothing, "AffichageVisue")
        If Not ReserveOctetsAffichage.IsOk Then
            Return False
        End If
        TRK.InitialiserTraces(New TRK.AfficherGraphiqueTrace(AddressOf AfficherGraphiqueTrace))
        'remplisage de la liste des sites
        ListeChoixSites.Items.AddRange(LibellesSitesDomTom)
        'remplisage de la liste des sites OSM Européan du menu contextuel de la visue
        For Cpt As Integer = 0 To SitesOSMEurope.Length - 1
            ChoisitSitesOSM.Items.Add(LibellesSitesDomTom(SystemeCartographique.SiteDomTomToIndex(SitesOSMEurope(Cpt), DomsToms.aucun)))
        Next
        SiteAllerA = SitesCartographiques.Aucun
        ImageAttributsPentes = New ImageAttributes
        'test de la connection internet et lance sa vérification toutes le 10 secondes
        ConnectedInternet_Tick(Nothing, Nothing)
        ConnexionInternet.Enabled = True
        ConnexionInternet.Start()
        CurseurEncours = Curseurs.CarteDefaut
#If Not BETA Then
        VerifierVersionApplication()
#End If
        Return True
    End Function
    ''' <summary> libère les ressources du formulaire </summary>
    Private Sub CloturerCapturer()
        If BrosseEtiquette IsNot Nothing Then
            BrosseEtiquette.Dispose()
            BrosseNotOk.Dispose()
            BrosseOk.Dispose()
            PinceauPoint.Dispose()
            PinceauLimites.Dispose()
            PinceauEchelle_1.Dispose()
            PinceauEchelle_2.Dispose()
            FontEtiquette.Dispose()
        End If
        Serveur?.Dispose()
        Affichage?.Dispose()
        ReserveOctetsAffichage?.Dispose()
    End Sub
    ''' <summary> Gère le compactage du cache en fonction des réponses de l'utilisateur </summary>
    Private Sub CompacterCache()
        If Not String.IsNullOrEmpty(NomBaseCacheTuile) Then
            MessageInformation = $"Le compactage du cache tuiles {NomBaseCacheTuile} a été demandé.{CrLf}" &
                                 $"Cette opération peut prendre beaucoup de temps{CrLf}Voulez continuer?"
            If AfficherConfirmation() = DialogResult.Cancel Then
                NomBaseCacheTuile = Nothing
            Else
                Cursor = Cursors.WaitCursor
                LabelInformation.Text = $"Compactage du cache tuiles {NomBaseCacheTuile}"
                LabelInformation.BackColor = Color.SeaShell
                LabelInformation.Refresh()
                Dim Titre = TitreInformation
                MessageInformation = $"Le compactage du cache tuiles{CrLf}{NomBaseCacheTuile}{CrLf} est encours. Veuillez patienter"
                TitreInformation = "Compactage d'un cache tuiles"
                LancerTache(AddressOf CompacterCacheTuiles)
                TitreInformation = Titre
            End If
        End If
    End Sub
    ''' <summary> Compacte le cache tuiles sélectionné </summary>
    Private Function CompacterCacheTuiles() As DialogResult
        Dim Connection As New SQLiteConnection($"Data Source={NomBaseCacheTuile};")
        Dim CommandConnection As SQLiteCommand
        Connection.Open()
        CommandConnection = Connection.CreateCommand()
        CommandConnection.CommandText = "VACUUM 'main';"
        CommandConnection.ExecuteNonQuery()
        Connection.Close()
        CommandConnection.Dispose()
        Connection.Dispose()
        Return DialogResult.Yes
    End Function
    ''' <summary> événement KeyDown du formulaire principal mais spécifique au deplacement du curseur de souris sur la visue </summary>
    ''' <param name="Touche"> touche de déplacement </param>
    ''' <summary> gére le point Decalage lors d'un déplacement de la souris sur la surface d'affichage </summary>
    Private Function TouchesDeplacementCurseur(e As KeyEventArgs) As Boolean
        TouchesDeplacementCurseur = True
        Select Case e.KeyCode
            Case Keys.NumPad0, Keys.D0
                If e.Control Then
                    AllerAuPointTraitementSurface(0)
                ElseIf e.Modifiers = Keys.None Then
                    Cursor.Position = New Point(VisueRectangle.X, VisueRectangle.Y)
                Else
                    TouchesDeplacementCurseur = False
                End If
            Case Keys.NumPad1, Keys.D1
                If e.Control Then
                    AllerAuPointTraitementSurface(1)
                ElseIf e.Modifiers = Keys.None Then
                    Cursor.Position = New Point(VisueRectangle.X + VisueRectangle.Width - 1, VisueRectangle.Y)
                Else
                    TouchesDeplacementCurseur = False
                End If
            Case Keys.NumPad2, Keys.D2
                If e.Control Then
                    AllerAuPointTraitementSurface(2)
                ElseIf e.Modifiers = Keys.None Then
                    Cursor.Position = New Point(VisueRectangle.X + VisueRectangle.Width - 1, VisueRectangle.Y + VisueRectangle.Height - 1)
                Else
                    TouchesDeplacementCurseur = False
                End If
            Case Keys.NumPad3, Keys.D3
                If e.Control Then
                    AllerAuPointTraitementSurface(3)
                ElseIf e.Modifiers = Keys.None Then
                    Cursor.Position = New Point(VisueRectangle.X, VisueRectangle.Y - 1 + VisueRectangle.Height)
                Else
                    TouchesDeplacementCurseur = False
                End If
            Case Keys.NumPad4, Keys.D4
                If e.Modifiers = Keys.None Then
                    DeplacerCurseurCentreVisue()
                Else
                    TouchesDeplacementCurseur = False
                End If
            Case Else
                TouchesDeplacementCurseur = False
        End Select
        e.SuppressKeyPress = TouchesDeplacementCurseur
    End Function
    ''' <summary> procédure appeler par le formulaire Proprité des traces pour afficher un Graphique </summary>
    ''' <param name="Trace"> trace concernée par le graphique </param>
    ''' <param name="Is_Vitesse"> s'agit'il du graphique Vitesse / par longueur de trace </param>
    Private Function AfficherGraphiqueTrace(Trace As TRK, Is_Vitesse As Boolean) As (IndexPoint As Integer, DialogResult As DialogResult)
        Using F As New AfficheGraphiqueTrace With {.Trace = Trace, .Is_Vitesse = Is_Vitesse}
            F.ShowDialog(Me)
            Return (F.IndexPoint, F.DialogResult)
        End Using
    End Function
    ''' <summary> procédure appeler par le module Information pour éviter les appels inter-threads </summary>
    Friend Sub AfficherInfomationsErreurs()
        AfficherInformation()
    End Sub
#End Region
#Region "Demande de tuiles et remplissage du tampon"
    ''' <summary> demande la creation d'une demande de tuile </summary>
    ''' <param name="TotalTuiles"> nb de tuiles concernées par la demande </param>
    Private Async Sub CreerDemandeTuilesAsync(Surface As Rectangle, Couche As Byte)
        If Serveur.NbDemandesAffichage = 0 Then
            'pour l'affichage automatique des tuiles à leur arrivée sur la surface d'affichage
            RafraichisseurVisue.Start()
            ListeChoixSites.Enabled = False
            GererMenuLancerCreation()
            LabelNbDemandesTuiles.BackColor = Color.Tomato
            ConfigureApp.Enabled = False
        End If
        Dim Demande = Serveur.CreerDemandeAffichage(Surface.Width * Surface.Height, Couche)
        'on s'abonne à l'évenement de fin de réalisation de la demande
        AddHandler Demande.Finished, AddressOf SupprimerDemandeTuiles
        Await Demande.CreerTuilesAffichageAsync(Surface)
        LabelNbDemandesTuiles.Text = Serveur.NbDemandesAffichage.ToString
        LabelNbDemandesTuiles.Refresh()
    End Sub
    ''' <summary> demande la suppression d'une demande de tuile </summary>
    ''' <param name="ID_Demande"> identifiant de la demande </param>
    Private Sub SupprimerDemandeTuiles(ID_Demande As Integer, IsErreur As Boolean)
        If IsErreur Then
            MessageInformation = $"Impossible de télécharger de nouvelles tuiles{ CrLf & Serveur.DemandesAffichage(ID_Demande).ToString & CrLf }" &
                                 "Le serveur ne répond pas ou la connexion internet est tombée"
            AfficherInformation()
        End If
        'on supprime la demande
        Serveur.SupprimerDemandeAffichage(ID_Demande)
        LabelNbDemandesTuiles.Text = Serveur.NbDemandesAffichage.ToString
        LabelNbDemandesTuiles.Refresh()
        If Serveur.NbDemandesAffichage = 0 Then
            'on arrête la mise à jour automatique de l'affichage
            RafraichisseurVisue.Stop()
            ListeChoixSites.Enabled = True
            GererMenuLancerCreation()
            LabelNbDemandesTuiles.BackColor = Color.YellowGreen
            ConfigureApp.Enabled = True
            FlagRemplirTampon = True
            'on lance la maj de l'affichage pour être sur que les dernières tuiles téléchargées sont prises en compte
            SurfaceAffichageTuile.Invalidate()
        End If
    End Sub
    ''' <summary> lance la copie sur le tampon de toutes les tuiles qui composent la surface d'affichage </summary>
    Private Sub RemplirTampon()
        FlagRemplirTampon = True
        'demander le téléchargement des tuiles de fond de plan
        CreerDemandeTuilesAsync(Affichage.SurfaceFond, Affichage.CoucheFond)
        If IsAffichagePentes Then
            'demander le téléchargement des tuiles des pentes
            CreerDemandeTuilesAsync(Affichage.SurfacePentes, 255)
        End If
    End Sub
#End Region
#Region "Afficher les coordonnees et mettre à jour le dessin de la carte"
    ''' <summary> sert quand on a besoin d'un automatisme pour repeindre la surface d'affichage lors d'une DemandeTuile </summary>
    Private Sub RafraichisseurVisue_Tick(sender As Object, e As EventArgs)
        SurfaceAffichageTuile.Invalidate()
    End Sub
    ''' <summary> affiche les coordonnées du curseur avec le type spécifié par l'unité sélectionnée </summary>
    Private Sub AfficherCoordonnees()
        Select Case ListeChoixTypeCoordonnees.SelectedIndex
            Case 3
                LabelCoordonnees.Text = CoordMouseUtmWGS84
            Case 2
                LabelCoordonnees.Text = CoordMouseDMSText
            Case 1
                LabelCoordonnees.Text = CoordMouseDDText
            Case 0
                LabelCoordonnees.Text = CoordMouseGrilleText
            Case 4
                LabelCoordonnees.Text = CoordMousePointG.ToString
        End Select
        If AltitudesSettings.IS_ALTITUDE Then
            LabelAltitude.Text = If(AltitudePosMouse = -9999, "0 m", AltitudePosMouse.ToString & " m")
        Else
            LabelAltitude.Text = "---- m"
        End If
    End Sub
    ''' <summary> demande la mise à jour de la surface d'affichage </summary>
    Private Sub RepeindreSurfaceAffichage()
        AfficherCoordonnees()
        'si il y a une capture on affiche les informations la concernant
        If Not FlagCapture Then
            LabelInformation.Text = SurfaceTraitement.ToString
            LabelInformation.BackColor = Color.Transparent
        Else
            LabelInformation.BackColor = Color.SeaShell
        End If
        SurfaceAffichageTuile.Invalidate()
    End Sub
    ''' <summary> redessine la surface d'affichage </summary>
    Private Async Sub SurfaceAffichageTuile_Paint(sender As Object, e As PaintEventArgs)
        If Not FlagAborted Then
            Using Image = Await Affichage.ImageAffichage(FlagRemplirTampon)
                'on ne translate pas avec e.Graphics.TranslateTransform(-Affichage.LocationAffichage.X, -Affichage.LocationAffichage.Y)
                'car il y a des transformations qui induisent des artefacts à l'affichage, dommage ce serait beaucoup plus simple
                e.Graphics.DrawImage(Image, New Rectangle(New Point(-Affichage.DecalageTamponX, -Affichage.DecalageTamponY), Image.Size))
                FlagRemplirTampon = False
                If Not SurfaceTraitement.IsEmpty Then DessinerSurfaceTraitement(e.Graphics)
                If Not CapturerSettings.POINT1.IsEmpty Then DessinerPoint(e.Graphics, CapturerSettings.POINT1, "Pt1")
                If Not CapturerSettings.POINT2.IsEmpty Then DessinerPoint(e.Graphics, CapturerSettings.POINT2, "Pt2")
                Dim SauveEtats = e.Graphics.BeginContainer()
                e.Graphics.TranslateTransform(-Affichage.LocationAffichage.X, -Affichage.LocationAffichage.Y)
                e.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                TRK.DessinerTraces(e.Graphics, Rectangle.Empty, True)
                If FlagLimitesSite Then e.Graphics.DrawRectangle(PinceauLimites, LimitesVirtuellesSite)
                e.Graphics.EndContainer(SauveEtats)
                'on est revenu par rapport à l'affichage
                If CapturerSettings.IS_AFFICHE_ECHELLE Then DessinerEchelle(e.Graphics)
            End Using
        End If
    End Sub
    ''' <summary> dessinne une échelle sur la visue en bas à droite </summary>
    ''' <param name="G"> surface de dessin où l'on dessinne </param>
    Private Sub DessinerEchelle(G As Graphics)
        If IsDessinEchelle Then
            With EchelleDessin
                'Dessin du texte de l'échelle
                G.FillRectangle(BrosseEtiquette, .RectangleEtiquette)
                G.DrawString(.TexteEchelle, FontEtiquette, Brushes.Black, .Pt4)
                G.DrawLine(PinceauEchelle_1, .Pt1, .Pt2)
                'dessin des traits de l'échelle
                'pour prendre en compte l'épaisseur du trait
                Dim Pt1 = Point.Add(.Pt1, DemiEpaisseur)
                Dim Pt2 = Point.Subtract(.Pt2, DemiEpaisseur)
                G.DrawLine(PinceauEchelle_2, Pt1, New Point(Pt1.X, Pt1.Y - 20))
                G.DrawLine(PinceauEchelle_2, .Pt3, New Point(.Pt3.X, .Pt3.Y - 10))
                G.DrawLine(PinceauEchelle_2, Pt2, New Point(Pt2.X, Pt2.Y - 20))
            End With
        End If
    End Sub
    ''' <summary> dessine une croix et ajoute une étiquette avec le nom du point </summary>
    ''' <param name="G"> surface de dessin où l'on dessinne </param>
    ''' <param name="PointGrille"> point à dessinner en coordonnées grille </param>
    ''' <param name="NomPoint"> Nom du point à mettre sur l'étiquette</param>
    Private Sub DessinerPoint(G As Graphics, PointGrille As PointD, NomPoint As String)
        Dim PointPixelSite As Point = PointGrilleToPointPixel(PointGrille, CapturerSettings.SITE, Affichage.Echelle)
        Dim Centre As Point = PixelsSiteToPixelsEcran(PointPixelSite)
        G.DrawLine(PinceauPoint, Centre.X - 15, Centre.Y, Centre.X + 15, Centre.Y)
        G.DrawLine(PinceauPoint, Centre.X, Centre.Y - 15, Centre.X, Centre.Y + 15)
        'on dessine un rectangle contenant la taille de l'étiquette
        Dim Pt4 As New Point(Centre.X + 4, Centre.Y - EtiquettePoint.Height - 4)
        G.FillRectangle(BrosseEtiquette, New Rectangle(Pt4, EtiquettePoint))
        'on ecrit l'étiquette par dessus
        G.DrawString(NomPoint, FontEtiquette, Brushes.Black, Pt4)
    End Sub
    ''' <summary> dessine un rectangle translucide pour indiquer la surface de capture à l'écran </summary>
    ''' <param name="G"> surface de dessin où l'on dessinne </param>
    Private Sub DessinerSurfaceTraitement(G As Graphics)
        Dim PointPixelSite As Point = SurfaceTraitement.PtPixels_NO
        Dim LocEcran As Point = PixelsSiteToPixelsEcran(PointPixelSite)
        Dim SizeEcran As Size = SurfaceTraitement.RectanglePixels.Size
        G.FillRectangle(If(SurfaceTraitementIsOk, BrosseOk, BrosseNotOk), New Rectangle(LocEcran, SizeEcran))
    End Sub
#End Region
#Region "Gestion de la souris, du clavier sur la Visue et des évenements de la visue"
    ''' <summary> transforme un point donné en pixels de la surface d'affichage en pixels de la projection grille et pour l'echelle encours </summary>
    Private Shared Function PixelsEcranToPixelsSite(PixelsEcran As Point) As Point
        Return New Point(Affichage.LocationAffichage.X + PixelsEcran.X, Affichage.LocationAffichage.Y + PixelsEcran.Y)
    End Function
    ''' <summary> transforme un point donné en pixels de la projection grille et pour l'echelle encours, en pixels de la surface d'affichage </summary>
    Private Shared Function PixelsSiteToPixelsEcran(PixelsSite As Point) As Point
        Return New Point(PixelsSite.X - Affichage.LocationAffichage.X, PixelsSite.Y - Affichage.LocationAffichage.Y)
    End Function
    ''' <summary> Coordonnées du pointeur de souris exprimées en coordonnées du monde virtuel (pixels) </summary>
    Private ReadOnly Property CoordMousePixels As Point
        Get
            Return PixelsEcranToPixelsSite(PosMouse)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en index tuile et offset. </summary>
    Private ReadOnly Property CoordMousePointG As PointG
        Get
            Return New PointG(CapturerSettings.INDICE_ECHELLE, CoordMousePixels)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection grille </summary>
    Private ReadOnly Property CoordMouseGrille As PointD
        Get
            Return PointPixelToPointGrille(CoordMousePixels, CapturerSettings.SITE, Affichage.Echelle)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en mètre de la projection Grille sous forme de chaine caractères formatée </summary>
    Private ReadOnly Property CoordMouseGrilleText As String
        Get
            Return ConvertPointXYtoChaine(New PointProjection(CoordMouseGrille))
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux </summary>
    Private ReadOnly Property CoordMouseDD As PointD
        Get
            Return PointGrilleToPointDD(CoordMouseGrille, Serveur.Datum)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimées en Degrés décimaux sous forme de chaine de caractères formatée </summary>
    Private ReadOnly Property CoordMouseDDText As String
        Get
            Return ConvertPointDDtoChaine(CoordMouseDD)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimée en degrés Miniute et secondes sous forme de chaine de caractères formatée </summary>
    Private ReadOnly Property CoordMouseDMSText As String
        Get
            Return ConvertPointDDtoDMS(CoordMouseDD)
        End Get
    End Property
    ''' <summary> Coordonnées du pointeur de souris exprimée en mètre de la projection UTM WGS84 sous forme de chaine de caractères formatée </summary>
    Private ReadOnly Property CoordMouseUtmWGS84 As String
        Get
            Return ConvertPointDDtoUTM(CoordMouseDD)
        End Get
    End Property
    ''' <summary> gère le décalage du tampon d'affichage en fonction des décalages X et Y exprimés en pixels </summary>
    Private Sub GererDecalageAffichage()
        'il y a besoin d'un décalage si la valeur du décalage tampon n'est plus dans le nombre des pixels d'une tuile
        If Not SurfaceDecalage.Contains(Affichage.DecalageTampon) Then
            Affichage.DecalerTampon()
            RemplirTampon()
        End If
    End Sub
    ''' <summary> gère le changement du zoom de l'affichage </summary>
    ''' <param name="DeltaIndiceCouche"> Ecart entre les indices de l'échelle actuelle et de l'échelle future </param>
    ''' <param name="PositionCoordonnees">Position du curseur de souris exprimé en pixel écran qui restera au même endroit sur le nouvel affichage </param>
    Private Sub GererChangementCouche(DeltaIndiceCouche As Integer, PositionCoordonnees As Point)
        Affichage.ChangerCouche(PositionCoordonnees, DeltaIndiceCouche)
        'on affiche la couche précédente avec le zoom de l'actuelle. sert quand le ser
        SurfaceAffichageTuile.Invalidate()
        RemplirTampon()
    End Sub
    ''' <summary> gère le changement de la taille de l'affichage </summary>
    Private Sub GererChangementTaille()
        If Not FlagInit Then
            'changement de taille du à la touche F11 ou équivalent, on prend comme coordonnées celles du centre de l'affichage
            CapturerSettings.LOCATION_CENTRE = Affichage.CentreAffichage
        End If
        'sinon on affiche les dernières coordonnées du site
        Affichage.ChangerTaille(VisueRectangle.Size, CapturerSettings.CENTRE_AFFICHAGE_SITE, FlagInit)
        RemplirTampon()
    End Sub
    Private Shared Function RegionAffichageDecalage(Decalage As Size) As Rectangle
        Dim Pt0Decalage = Point.Add(Affichage.LocationAffichage, Decalage)
        Return New Rectangle(Pt0Decalage, VisueRectangle.Size)
    End Function
    ''' <summary> événement KeyDown du formulaire principal mais spécifique aux touches de déplacement de la Visue </summary>
    ''' <param name="e"> information sur la touche de déplacement et de son modifieur éventuel </param>
    Private Function TouchesDeplacementCarte(e As KeyEventArgs) As Boolean
        TouchesDeplacementCarte = False
        Dim PasX, PasY As Integer
        'si le nb maximum de tuiles en cours de téléchargement est atteint on ne fait pas le décalage
        If Serveur.NbTuilesDemandeAffichage < ServeurCarto.MaxRequetesTuiles Then
            Select Case e.KeyCode
                Case Keys.Left To Keys.Down 'on déplace l'affichage par pixels
                    Dim Pas As Integer = PasTouchesPixels(e.Modifiers \ Keys.Shift)
                    Select Case e.KeyCode
                        Case Keys.Left
                            PasX = Pas
                        Case Keys.Right
                            PasX = -Pas
                        Case Keys.Up
                            PasY = Pas
                        Case Else 'Keys.Down
                            PasY = -Pas
                    End Select
                    TouchesDeplacementCarte = True
                Case Keys.PageUp To Keys.Home 'on déplace l'affichage en fonction des dimensions de la visue
                    Dim Pas As Double = PasTouchesAffichage(e.Modifiers \ Keys.Shift)
                    Select Case e.KeyCode
                        Case Keys.Home
                            PasX = CInt(Pas * SurfaceAffichageTuile.ClientSize.Width)
                        Case Keys.End
                            PasX = -CInt(Pas * SurfaceAffichageTuile.ClientSize.Width)
                        Case Keys.PageUp
                            PasY = CInt(Pas * SurfaceAffichageTuile.ClientSize.Height)
                        Case Else 'Keys.PageDown
                            PasY = -CInt(Pas * SurfaceAffichageTuile.ClientSize.Height)
                    End Select
                    TouchesDeplacementCarte = True
            End Select
            If TouchesDeplacementCarte Then
                'limitation du déplacement aux limites du site web
                If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.IntersectsWith(RegionAffichageDecalage(New Size(PasX, PasY))) Then
                    MessageInformation = "La surface de l'affichage sera en dehors des limites du site de capture."
                    AfficherInformation()
                Else
                    AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD)
                    Affichage.DecalageTamponX += PasX
                    Affichage.DecalageTamponY += PasY
                    GererDecalageAffichage()
                    RepeindreSurfaceAffichage()
                End If
            End If
        End If
        e.SuppressKeyPress = TouchesDeplacementCarte
    End Function
    Private Sub SurfaceAffichageTuile_MouseMove(sender As Object, e As MouseEventArgs)
        'si l'évenement mouse leave d'un label liste ou menu n'a pas été généré on le force
        If LabelEncours IsNot Nothing Then LabelListesMenus_MouseLeave(LabelEncours, Nothing)
        'on verifie d'abord que le curseur de souris est toujours sur l'affichage
        If VisueRectangle.Contains(Point.Add(e.Location, AffichageOffsetEcran)) Then
            If Action = Actions.DefinirPoints Then
                PosMouse = e.Location
                If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.Contains(CoordMousePixels) Then
                    MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture."
                    AfficherInformation()
                    Action = Actions.Aucune
                Else
                    NomPointEncours = "Pt2"
                    DefinirPoint()
                End If
            ElseIf Action = Actions.DeplacerCarte Then
                Dim PasX = PosMouse.X - e.Location.X
                Dim PasY = PosMouse.Y - e.Location.Y
                'limitation du déplacement aux limites du site web
                If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.IntersectsWith(RegionAffichageDecalage(New Size(PasX, PasY))) Then
                    MessageInformation = "La surface de l'affichage sera en dehors des limites du site de capture."
                    AfficherInformation()
                    Action = Actions.Aucune
                    TrouverCurseurCarte()
                Else
                    Affichage.DecalageTamponX += PasX
                    Affichage.DecalageTamponY += PasY
                    PosMouse = e.Location
                    GererDecalageAffichage()
                End If
            ElseIf TRK.IsTraceEncoursModeEdition Then 'Si la trace peut être éditée ou est éditée
                PosMouse = e.Location
                If TRK.TraceEncours.SourisBouge(CoordMousePixels) Then SurfaceAffichageTuile.Invalidate()
            Else
                PosMouse = e.Location
            End If
            AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD)
            RepeindreSurfaceAffichage()
        End If
    End Sub
    ''' <summary> gère le flag qui indique que le bouton gauche de la souris est enfoncé (outil déplacement de la carte avec la souris) </summary>
    Private Sub SurfaceAffichageTuile_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then 'on ne prend en compte que le bouton gauche de la souris
            PosMouse = e.Location
            AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD)
            If ModifierKeys = Keys.Alt Then 'il s'agit de la définition des PT1 et PT2 avec la souris
                If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.Contains(CoordMousePixels) Then
                    MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture."
                    AfficherInformation()
                Else
                    CurseurEncours = Curseurs.FRefDeplacer
                    Action = Actions.DefinirPoints
                    NomPointEncours = "Pt1"
                    DefinirPoint()
                End If
            ElseIf TRK.IsTraceEncoursEditing Then
                'la trace encours est entrain d'être éditée Action DéplacerPoint, CouperPoint ou Segment, SupprimerPoint ou Segment
                'on avertit la trace qu'il faut commencer l'ActionEncours
                Dim Attr As New Attribut With {.Altitude = AltitudePosMouse}
                If TRK.TraceEncours.SourisDown(CoordMousePixels, Attr) Then SurfaceAffichageTuile.Invalidate()
            Else 'action Deplacercarte
                If Serveur.NbTuilesDemandeAffichage < ServeurCarto.MaxRequetesTuiles Then
                    Action = Actions.DeplacerCarte
                    TrouverCurseurCarte()
                End If
            End If
        End If
    End Sub
    ''' <summary> gère le flag qui indique que le bouton gauche de la souris est relevé (outil déplacement avec la souris) </summary>
    Private Sub SurfaceAffichageTuile_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then 'on ne prend en compte que le bouton gauche de la souris
            PosMouse = e.Location
            If Action = Actions.DefinirPoints Then
                'le bouton gauche a été relevée en 1er on ne peut pas finir l'action directement
                FinirActionDefinirPoints()
            ElseIf Action = Actions.FinirDefinirPoints Then
                'la touche menu a été relevé en 1er on peut finir l'action
                Action = Actions.Aucune
                TrouverCurseurCarte()
            ElseIf TRK.IsTraceEncoursEditing Then
                'la trace encours est entrain d'être éditée Action DéplacerPoint, CouperPoint ou Segment, SupprimerPoint ou Segment
                'on avertit la trace qu'il faut finir l'ActionEncours
                Dim Attr As New Attribut With {.Altitude = AltitudePosMouse}
                If TRK.TraceEncours.SourisUp(CoordMousePixels, Attr) Then SurfaceAffichageTuile.Invalidate()
                CurseurEncours = Curseurs.TraceDefaut
            Else 'action Deplacercarte
                Action = Actions.Aucune
                TrouverCurseurCarte()
            End If
        End If
    End Sub
    ''' <summary> logique de gestion de la molette de la souris : changement de couche </summary>
    Private Sub SurfaceAffichageTuile_MouseWheel(sender As Object, e As MouseEventArgs)
        Dim DeltaIndiceEchelle As Integer
        If e.Delta > 0 Then
            If MemoireDeltaSouris >= 0 Then
                MemoireDeltaSouris += 1
            Else
                MemoireDeltaSouris = 1
            End If
            If MemoireDeltaSouris = 3 Then
                If CapturerSettings.INDICE_ECHELLE > 0 Then
                    PosMouse = e.Location
                    MemoireDeltaSouris = 0
                    DeltaIndiceEchelle = -1
                End If
            End If
        Else
            If MemoireDeltaSouris <= 0 Then
                MemoireDeltaSouris -= 1
            Else
                MemoireDeltaSouris = -1
            End If
            If MemoireDeltaSouris = -3 Then
                If CapturerSettings.INDICE_ECHELLE < Serveur.NbEchellesLayer(0) Then
                    PosMouse = e.Location
                    MemoireDeltaSouris = 0
                    DeltaIndiceEchelle = +1
                End If
            End If
        End If
        If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.Contains(CoordMousePixels) Then
            MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture."
            AfficherInformation()
        Else
            FlagPosMouse = True
            ListeChoixEchelles.SelectedIndex += DeltaIndiceEchelle
            FlagPosMouse = False
        End If
    End Sub
    '''' <summary> met à jour les coordonnées du pointeur de souris en cas de click </summary>
    Private Sub SurfaceAffichageTuile_MouseClick(sender As Object, e As MouseEventArgs) 'Handles SurfaceAffichageTuile.MouseClick
        PosMouse = e.Location
        AltitudePosMouse = TrouverAltitudeCoordonnees(CoordMouseDD)
    End Sub
    ''' <summary> gère le redimensionnement de la surface d'affichage </summary>
    Private Sub SurfaceAffichageTuile_Resize(sender As Object, e As EventArgs)
        'Size.Empty est le cas de la réduction de FCGP_Capturer à une icone sur la barre des tâches, on sort car il n'y a rien à afficher
        If SurfaceAffichageTuile.ClientRectangle.Size <> Size.Empty Then
            'sert à déterminer si le curseur de la souris est sur la visue
            VisueRectangle = SurfaceAffichageTuile.RectangleToScreen(SurfaceAffichageTuile.ClientRectangle)
            CentreAffichage = New Point(VisueRectangle.Width \ 2, VisueRectangle.Height \ 2)
            AffichageOffsetEcran = CType(VisueRectangle.Location, Size)
            GererChangementTaille()
        End If
    End Sub
#End Region
#Region "Actions spécifiques Menu_Application et touches associées"
    ''' <summary> ouvre le formulaire de configuration à partir du menu du programme </summary>
    Private Sub ConfigureApp_Click(sender As Object, e As EventArgs)
        OuvrirConfiguration()
    End Sub
    ''' <summary> lance le test du serveur avec une requête GetCapabilities </summary>
    Private Sub TesteServeur_Click(sender As Object, e As EventArgs)
        Serveur.TesterServeurAsync()
    End Sub
    ''' <summary> compacte le cache tuile selectionné </summary>
    Private Sub CompacteCacheTuiles_Click(sender As Object, e As EventArgs)
        MessageInformation = $"Vous avez demandé à compacter le cache du site {ListeChoixSites.SelectedItem}{CrLf}" &
                             $"Cette action peut prendre beaucoup de temps en sortie de FCGP Capturer{CrLf}Voulez continuer?"
        If AfficherConfirmation() = DialogResult.OK Then
            NomBaseCacheTuile = CapturerSettings.CHEMIN_CACHE & "\CacheTuiles" & SitesCartoClefs(CapturerSettings.SITE) & ".db"
        End If
    End Sub
    ''' <summary> ouvre le formulaire de configuration des captures de cartes à partir du menu du programme </summary>
    Private Sub LanceCreationCarte_Click(sender As Object, e As EventArgs)
        OuvrirPreferencesAsync()
    End Sub
    ''' <summary> Supprime les tuiles correspondantes à la surface de téléchargement (1 seul niveau de détails) </summary>
    Private Sub SupprimeTuiles_Click(sender As Object, e As EventArgs)
        SupprimerTuiles()
    End Sub
    ''' <summary> ouvre le formulaire de saisie de coordonnées en fonction du type de celle-ci </summary>
    Private Sub Va_A_App_Click(sender As Object, e As EventArgs)
        CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(PointEncours, CapturerSettings.SITE, Affichage.Echelle)
        OuvrirSaisieCoordonnees()
    End Sub
    Private Sub Va_A_Site_Click(sender As Object, e As EventArgs)
        AllerAuCentreSite()
    End Sub
    ''' <summary> ouvre la boite d'information A Propos à partir du menu du programme </summary>
    Private Sub APropos_Click(sender As Object, e As EventArgs)
        OuvrirAPropos()
    End Sub
    ''' <summary> ouvre le formulaire d'aide à partir du menu du programme </summary>
    Private Sub Aide_Click(sender As Object, e As EventArgs)
        AfficherAide(Me)
    End Sub
    ''' <summary> ouvre le formulaire d'aide à partir du menu du programme </summary>
    Private Sub ImprimerCarte_Click(sender As Object, e As EventArgs)
        OuvrirImprimerCarte()
    End Sub
    Private Sub LimitesSite_Click(sender As Object, e As EventArgs)
        AppliquerLimitesSite()
    End Sub
    ''' <summary> quite le programme à partir du menu du programme </summary>
    Private Sub FermeApp_Click(sender As Object, e As EventArgs)
        Close()
    End Sub
    ''' <summary> événement KeyDown du formulaire principal mais spécifique au Menu Application </summary>
    Private Function TouchesMenuApp(e As KeyEventArgs) As Boolean
        TouchesMenuApp = True
        Select Case e.KeyCode
            Case Keys.F1
                If e.Alt Then
                    OuvrirImprimerCarte()
                ElseIf e.Modifiers = Keys.None Then
                    AfficherAide(Me)
                Else
                    TouchesMenuApp = False
                End If
            Case Keys.F2 'on ouvre  Préférences si il y a une capture encours ou si les conditions sont réunies si il n'y a pas de capture encours
                If e.Alt Then
                    If SurfaceTraitementIsOk Then SupprimerTuiles()
                ElseIf e.Modifiers = Keys.None Then
                    If (SurfaceTraitementIsOk AndAlso Serveur.NbDemandesAffichage = 0 AndAlso IsConnected) OrElse FlagCapture Then OuvrirPreferencesAsync()
                Else
                    TouchesMenuApp = False
                End If
            Case Keys.F3
                If e.Alt Then
                    Serveur.TesterServeurAsync()
                ElseIf e.Modifiers = Keys.None Then
                    If Serveur.NbDemandesAffichage = 0 AndAlso Not FlagCapture Then OuvrirConfiguration()
                Else
                    TouchesMenuApp = False
                End If
            Case Keys.F4
                If e.Alt Then
                    Close()
                ElseIf e.Modifiers = Keys.None Then
                    OuvrirAPropos()
                Else
                    TouchesMenuApp = False
                End If
            Case Keys.F5
                If e.Modifiers = Keys.None Then
                    If FlagRevenir Then
                        RevenirDe()
                    End If
                Else
                    TouchesMenuApp = False
                End If
            Case Keys.F6
                If e.Alt Then
                    AllerAuCentreSite()
                Else
                    TouchesMenuApp = False
                End If
            Case Else
                TouchesMenuApp = False
        End Select
        e.SuppressKeyPress = TouchesMenuApp
    End Function
#End Region
#Region "Actions spécifiques MenuPoints et touches associées"
    ''' <summary> définit le PointEncours avec les coordonnées du centre de la visue </summary>
    Private Sub DefinitCentrePts_Click(sender As Object, e As EventArgs)
        DefinirPointCentre()
    End Sub
    ''' <summary> efface le pointencours </summary>
    Private Sub EffacePts_Click(sender As Object, e As EventArgs)
        EffacerPoint()
    End Sub
    ''' <summary> Place l </summary>
    Private Sub Va_A_Pts_Click(sender As Object, e As EventArgs)
        AllerAuPointCapture()
    End Sub
    ''' <summary> événement KeyDown du formulaire principal mais spécifique au Menu Points </summary>
    Private Function TouchesMenuPoints(e As KeyEventArgs) As Boolean
        TouchesMenuPoints = True 'par defaut on dit que la touche est traitée
        Select Case e.KeyCode
            Case Keys.F6
                If Not CapturerSettings.POINT1.IsEmpty AndAlso e.Shift Then
                    NomPointEncours = "Pt1"
                    AllerAuPointCapture()
                ElseIf Not CapturerSettings.POINT2.IsEmpty AndAlso e.Control Then
                    NomPointEncours = "Pt2"
                    AllerAuPointCapture()
                ElseIf e.Modifiers = Keys.None Then
                    OuvrirSaisieCoordonnees()
                Else
                    TouchesMenuPoints = False
                End If
            Case Keys.F7
                If e.Shift OrElse e.Control Then
                    If Not CapturerSettings.POINT1.IsEmpty And Not CapturerSettings.POINT2.IsEmpty Then
                        OuvrirSurfaceTraitement(SurfaceTraitement)
                    End If
                ElseIf e.Modifiers = Keys.None Then
                    OuvrirSurfaceTraitement(New SurfaceTuiles)
                Else
                    TouchesMenuPoints = False
                End If
            Case Keys.F8
                If e.Shift Then
                    NomPointEncours = "Pt1"
                    DefinirPointCentre()
                ElseIf e.Control Then
                    NomPointEncours = "Pt2"
                    DefinirPointCentre()
                Else
                    TouchesMenuPoints = False
                End If
            Case Keys.F9
                If e.Shift Then
                    NomPointEncours = "Pt1"
                    EffacerPoint()
                ElseIf e.Control Then
                    NomPointEncours = "Pt2"
                    EffacerPoint()
                Else
                    TouchesMenuPoints = False
                End If
            Case Else
                TouchesMenuPoints = False
        End Select
        e.SuppressKeyPress = TouchesMenuPoints
    End Function
#End Region
#Region "Actions spécifiques Menu_Trace et touches associées"
    ''' <summary> invers le mode d'édition de la trace encours </summary>
    Private Sub EditeTrace_Click(sender As Object, e As EventArgs)
        InverserEditionTrace()
    End Sub
    ''' <summary> ouvre le formulaire de fermeture de la trace encours </summary>
    Private Sub FermeTrace_Click(sender As Object, e As EventArgs)
        FermerTraceAffichage()
    End Sub
    ''' <summary> ouvre le formulaire des propriétés de la trace encours </summary>
    Private Sub ProprietesTrace_Click(sender As Object, e As EventArgs)
        ProprietesTraceAffichage()
    End Sub
    ''' <summary> positionne début de la trace encours au centre de l'affichage </summary>
    Private Sub Va_A_DebutTrace_Click(sender As Object, e As EventArgs)
        AllerAuPointTrace(True)
    End Sub
    ''' <summary> positionne la fin de la trace encours au centre de l'affichage </summary>
    Private Sub Va_A_FinTrace_Click(sender As Object, e As EventArgs)
        AllerAuPointTrace(False)
    End Sub
    ''' <summary> ouvre le formulaire de d'ouverture de la trace encours </summary>
    Private Sub OuvreTrace_Click(sender As Object, e As EventArgs)
        OuvrirTraceAffichage()
    End Sub
    ''' <summary> événement KeyDown du formulaire principale mais spécifique au Menu Trace </summary>
    ''' <param name="ToucheMenuTrace"> Code de la touche à transmettre à la trace </param>
    ''' <returns> True si l'évenement est pris en compte </returns>
    Private Function TouchesMenuTrace(e As KeyEventArgs) As Boolean
        TouchesMenuTrace = True
        If TRK.IsTraceEncours AndAlso e.Alt Then
            Select Case e.KeyCode
                Case Keys.E 'edition trace on/off
                    InverserEditionTrace()
                Case Keys.P
                    ProprietesTraceAffichage()
                Case Keys.F
                    FermerTraceAffichage()
                Case Keys.D
                    AllerAuPointTrace(True)
                Case Keys.A
                    AllerAuPointTrace(False)
                Case Else
                    TouchesMenuTrace = False
            End Select
        ElseIf e.KeyCode = Keys.F10 Then
            OuvrirTraceAffichage()
        ElseIf TRK.IsTraceEncoursModeEdition AndAlso TRK.TraceEncours.ToucheDown(e.KeyCode) Then
            'si la trace peut être éditée, on transmet l'événement pour qu'elle puisse traiter les touches A,S,C
            SurfaceAffichageTuile.Invalidate()
        Else
            TouchesMenuTrace = False
        End If
        e.SuppressKeyPress = TouchesMenuTrace
    End Function
#End Region
#Region "Actions spécifiques Context Menu_Visue et touches associées"
    ''' <summary> Définit le point qui est cliqué à la position de la souris </summary>
    Private Sub DefinitPts_Click(sender As Object, e As EventArgs)
        NomPointEncours = CType(sender, ToolStripMenuItem).Tag.ToString()
        DefinirPoint()
    End Sub
    ''' <summary> permet ou non d'afficher une échelle dans le coin bas droit de l'écran pour certains niveaux </summary>
    Private Sub AfficheDessinEchelle_Click(sender As Object, e As EventArgs)
        AfficherDessinEchelle()
    End Sub
    ''' <summary> permet ou non d'afficher la couche des pentes pour certains sites et certains niveaux </summary>
    Private Sub AfficheCouchePentes_Click(sender As Object, e As EventArgs)
        AfficherPentes()
    End Sub
    ''' <summary> permet de choisir le coefficient de transparence de la couche des Pentes </summary>
    Private Sub ChoisitCoefAlphaPentes_SelectedIndexChanged(sender As Object, e As EventArgs) 'Async
        If CapturerSettings.INDICE_COEF_ALPHA_PENTES <> ChoisitCoefAlphaPentes.SelectedIndex Then
            'on force la fermeture du tooltip
            ChoisitCoefAlphaPentes.Visible = False
            ContextMenuVisue.Hide()
            ChoisitCoefAlphaPentes.Visible = True
            CapturerSettings.INDICE_COEF_ALPHA_PENTES = ChoisitCoefAlphaPentes.SelectedIndex
            Dim CoefAlphaPentes = CoefsAlphasPentes(ChoisitCoefAlphaPentes.SelectedIndex)
            ImageAttributsPentes.SetColorMatrix(New ColorMatrix With {.Matrix33 = CoefAlphaPentes}, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
            RemplirTampon()
        End If
    End Sub
    ''' <summary> Change le site actuel pour le site à portée européenne indiqué mais avec la couche la plus approchante et la même position </summary>
    Private Sub ChoisitSitesOSM_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ChoisitSitesOSM.Visible = True Then
            'on force la fermeture du tooltip
            ChoisitSitesOSM.Visible = False
            ContextMenuVisue.Hide()
            ChoisitSitesOSM.Visible = True
            SiteAllerA = SitesOSMEurope(ChoisitSitesOSM.SelectedIndex)
            If DatumSiteWeb(CapturerSettings.SITE) <> Datums.Web_Mercator Then
                'les sites Europeen sont forcément des sites WebMercator donc si le site de départ est SM il faut passer par une transformation de coordonnées
                Dim IndiceEchelle As Integer = CapturerSettings.INDICE_ECHELLE
                Dim Echelle As Echelles = NiveauDetailCartographique.SiteIndiceEchelleToEchelle(SiteAllerA, IndiceEchelle)
                Dim Point_WebMercator As Point = PointDDToPointPixels(CoordMouseDD, SiteAllerA, Echelle)
                PositionSiteAllerA = New PointG(IndiceEchelle, Point_WebMercator)
            Else
                PositionSiteAllerA = CoordMousePointG
            End If
            CapturerSettings.INDICE_OSM = ChoisitSitesOSM.SelectedIndex
            'on demande le changement du site actuel par le site OSM
            ListeChoixSites.SelectedIndex = SystemeCartographique.SiteDomTomToIndex(SiteAllerA, DomsToms.aucun)
        End If
    End Sub
    ''' <summary> Change le fond de plan d'affichage du site actuel </summary>
    Private Sub ChoisitFondPlan_SelectedIndexChanged(sender As Object, e As EventArgs) 'Async
        If CapturerSettings.INDICE_FOND_PLAN <> ChoisitFondsPlan.SelectedIndex Then
            ContextMenuVisue.Hide()
            CapturerSettings.INDICE_FOND_PLAN = ChoisitFondsPlan.SelectedIndex
            Serveur.IndiceFondPlan = ChoisitFondsPlan.SelectedIndex
            RemplirTampon()
        End If
    End Sub
    ''' <summary> événement KeyDown du formulaire principal mais spécifique au context Menu Visue </summary>
    Private Function TouchesMenuVisue(e As KeyEventArgs) As Boolean
        TouchesMenuVisue = True
        Select Case e.KeyCode
            Case Keys.F12
                If ModifierKeys = Keys.Alt Then
                    AfficherPentes()
                ElseIf e.Modifiers = Keys.None Then
                    AfficherDessinEchelle()
                Else
                    TouchesMenuVisue = False
                End If
            Case Else
                TouchesMenuVisue = False
        End Select
        e.SuppressKeyPress = TouchesMenuVisue
    End Function
#End Region
#Region "Actions Communes à plusieurs menus ou liste de choix ou touches communes à plusieurs Menus"
    'la gestion des évènements dans windowsforms est sujette à caution. Le système ne réagit pas très vite.
    'Cela concerne surtout les 1er contrôles ajoutés dans la collection de contrôles du formulaire.
    'De ce fait on est obligé de vérifier que les évenements prévus ont bien été émis et réalisés ce qui complexifie un peu le code
    ''' <summary> ouvre le formulaire de saisie d'une surface de capture </summary>
    Private Sub SaisieSurfacesTraitements_Click(sender As Object, e As EventArgs)
        Dim NomMenu As String = CType(sender, ToolStripMenuItem).Tag.ToString()
        Dim Surface = If(NomMenu = "App", New SurfaceTuiles(), SurfaceTraitement)
        OuvrirSurfaceTraitement(Surface)
    End Sub
    ''' <summary> lance l'action de revenir au dernier point de sauvegarde </summary>
    Private Sub RevientDe_Click(sender As Object, e As EventArgs)
        RevenirDe()
    End Sub
    ''' <summary> vérifie que l'apparence des Labels avec liste ou menu soit non survolé </summary>
    ''' <remarks> A l'entrée Labelencours doit être null </remarks>
    Private Sub Information_MouseMove(sender As Object, e As EventArgs)
        'si l'évenement mouse leave d'un label liste ou menu n'a pas été généré on le force
        If LabelEncours IsNot Nothing Then
            LabelListesMenus_MouseLeave(LabelEncours, Nothing)
        End If
    End Sub
    ''' <summary> vérifie que l'apparence de sender soit survolé et non survolée pour les autres </summary>
    ''' <remarks> à l'entrée labelencours doit être sender </remarks>
    Private Sub LabelListesMenus_MouseMove(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        'l'évenement mouse leave de l'ancien labelencours n'a pas été généré et ou l'évenement enter du label encours n'a pas été généré, on le force
        If L IsNot LabelEncours Then
            LabelListesMenus_MouseEnter(L, Nothing)
        End If
    End Sub
    ''' <summary> passe l'apparence de sender à survolée et non survolée pour les autres </summary>
    ''' <remarks> A l'entrée Labelencours doit être null </remarks>
    Private Sub LabelListesMenus_MouseEnter(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        'l'évenement mouse leave de l'ancien labelencours n'a pas été généré on le force
        If LabelEncours IsNot Nothing Then
            LabelListesMenus_MouseLeave(LabelEncours, Nothing)
        End If
        'mettre à jour l'apparence du label comme étant survolé
        L.BackColor = Color.FromArgb(255, 229, 241, 251)
        L.BorderStyle = BorderStyle.FixedSingle
        L.Refresh()
        'on indique le labelencours
        LabelEncours = L
    End Sub
    ''' <summary> passe l'apparence de sender à non survolée </summary>
    Private Sub LabelListesMenus_MouseLeave(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        'mettre à jour l'apparence du label comme étant non survolé
        L.BackColor = Color.Transparent
        L.BorderStyle = BorderStyle.None
        L.Refresh()
        'on indique qu'il n'y a plus de labelencours
        LabelEncours = Nothing
        If L Is LabelMenuPt1 OrElse L Is LabelMenuPt2 Then
            GererIUMenuPts()
        ElseIf L Is LabelTrace Then
            GererIUMenuTrace()
        End If
    End Sub
    ''' <summary> ouvre le menu associé à un LabelMenu </summary>
    Private Sub LabelMenus_Click(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        If Not FlagContextMenuShow Then
            If L Is LabelMenuPt1 OrElse L Is LabelMenuPt2 Then
                NomPointEncours = L.Tag.ToString()
                GererMenuPts()
            End If
            Dim R2 As Rectangle = L.RectangleToScreen(L.ClientRectangle)
            FlagContextMenuShow = True
            L.ContextMenuStrip.Show()
            Dim s As Size = L.ContextMenuStrip.Size
            If L Is LabelMenuApp OrElse L Is LabelMenuPt1 Then ' à gauche du label
                L.ContextMenuStrip.SetBounds(R2.Right - s.Width, R2.Top - s.Height, s.Width, s.Height)
            Else 'à droite du label
                L.ContextMenuStrip.SetBounds(R2.Left, R2.Top - s.Height, s.Width, s.Height)
            End If
        Else
            L.ContextMenuStrip.Close()
            FlagContextMenuShow = False
        End If
    End Sub
    ''' <summary> ouvre la liste de choix associée à un LabelListe </summary>
    Private Sub LabelListes_Click(sender As Object, e As EventArgs)
        Dim L As Label = CType(sender, Label)
        If L Is LabelSites AndAlso Serveur.NbDemandesAffichage > 0 Then Exit Sub
        If L Is LabelEchelles Then
            'si le centre de l'affichage est en dehors des limites du site on sort
            If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.Contains(Affichage.CentreAffichage) Then
                MessageInformation = "Le centre de l'affichage est en dehors des limites du site de capture."
                AfficherInformation()
                Exit Sub
            End If
        End If
        'trouve la combobox associée au label
        ComboxDropDown = CType(L.Tag, ComboBox)
        'ouvre la liste de choix de la combobox associée au label
        ComboxDropDown.DroppedDown = True
        'rend visible la combobox associée au label
        ComboxDropDown.Show()
    End Sub
    ''' <summary> ferme la liste déroulante des choix de type sites, echelles ou coordonnées </summary>
    Private Sub ListesChoix_DropDownClosed(sender As Object, e As EventArgs)
        'rendre la combobox support invisible
        ComboxDropDown.Hide()
        'supprimer la relation avec la combobox support
        ComboxDropDown = Nothing
    End Sub
    ''' <summary> autorise ou non l'ouverture du menu trace</summary>
    Private Sub ContextMenus_Opening(sender As Object, e As CancelEventArgs)
        If Not FlagContextMenuShow Then e.Cancel = True
    End Sub
    ''' <summary> gère la fermeture des menus Points, trace et application </summary>
    Private Sub ContextMenus_Closed(sender As Object, e As ToolStripDropDownClosedEventArgs)
        Dim C As ContextMenuStrip = CType(sender, ContextMenuStrip)
        Dim T As Label = CType(C.Tag, Label)
        'si un click en dehors du menu ou la touche escape ou une action du menu on indique que le menu est fermé
        If e.CloseReason = ToolStripDropDownCloseReason.Keyboard OrElse e.CloseReason = ToolStripDropDownCloseReason.ItemClicked OrElse
           T.BackColor <> Color.FromArgb(255, 229, 241, 251) Then
            FlagContextMenuShow = False
        End If
    End Sub
#End Region
#Region "Actions communes à des clicks sur menu et des touches de raccourci"
    ''' <summary> dimensionne la taille de la visue à 80% environ de l'écran </summary>
    Private Sub DimensionnerAffichage()
        'si c'est le premier passage on dimensionne la visue et l'affichage à partir du déclemenchement de l'évenement resize
        Dim R As Rectangle = Rectangle.Inflate(DimensionsEcranSupport, CInt(DimensionsEcranSupport.Width * -0.1), CInt(DimensionsEcranSupport.Height * -0.1))
        Location = R.Location
        ClientSize = R.Size
    End Sub
    ''' <summary> inverse le flag qui permet l'affichage ou non d'une echelle dans le coin bas droit de la visue </summary>
    Private Sub AfficherDessinEchelle()
        CapturerSettings.IS_AFFICHE_ECHELLE = Not CapturerSettings.IS_AFFICHE_ECHELLE
        AfficheDessinEchelle.Text = ChangerTextBascule(False)
        SurfaceAffichageTuile.Invalidate()
    End Sub
    ''' <summary> inverse le flag qui permet l'affichage ou non de la couche des pentes par dessus la couche de fond de plan </summary>
    Private Sub AfficherPentes()
        CapturerSettings.IS_AFFICHE_PENTES = Not CapturerSettings.IS_AFFICHE_PENTES
        AfficheCouchePentes.Text = ChangerTextBascule(True)
        Affichage.CalculerTamponPentes()
        RemplirTampon()
    End Sub
    ''' <summary> Calcul le texte du context menueVisue. il s'agit d'une bascule On / OFf </summary>
    ''' <param name="FlagPentes"> Oui si le menu concerne les pentes, Non si le menu concerne le dessin de l'échelle </param>
    Private Shared Function ChangerTextBascule(FlagPentes As Boolean) As String
        Dim Ret As String
        If FlagPentes Then
            Ret = $"Afficher Pentes  : {If(CapturerSettings.IS_AFFICHE_PENTES, "Off", "On")}"
        Else
            Ret = $"Afficher Echelle  : {If(CapturerSettings.IS_AFFICHE_ECHELLE, "Off", "On")}"
        End If
        Return Ret
    End Function
    ''' <summary> supprime toutes les tuiles qui font partie de la surface de téléchargement. Les tuiles Pentes ne sont pas concernées </summary>
    Private Sub SupprimerTuiles()
        MessageInformation = $"Effacer les tuiles sélectionnées ?" & CrLf
        If AfficherConfirmation() = DialogResult.OK Then
            'suppression des tuiles définies par la surface de capture
            Serveur.Cache.EffacerSurfaceTuile(SurfaceTraitement)
            CapturerSettings.POINT1 = PointD.Empty
            CapturerSettings.POINT2 = PointD.Empty
            GererIUMenuPts()
            'on télécharge les tuiles éffacées et qui doivent être affichées
            RemplirTampon()
        End If
    End Sub
    ''' <summary> ouvre le formulaire de saisie de coordonnées adequat en fonction du type d'unité </summary>
    Private Sub OuvrirSaisieCoordonnees()
        Dim Coordonnees As New PointD
        If OuvrirFormulaireSaisieCoordonnees(Me, Point.Empty, ListeChoixTypeCoordonnees.SelectedIndex, Coordonnees) = DialogResult.OK Then
            GererMenuRevenir(True)
            CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(Coordonnees, CapturerSettings.SITE, Affichage.Echelle)
            AllerA()
        End If
    End Sub
    ''' <summary> ouvre la boite d'information A Propos </summary>
    Private Sub OuvrirAPropos()
        Dim S As New StringBuilder
        S.AppendLine(NumFCGP)
        S.AppendLine("en date du " & DateVersionFCGP)
        S.AppendLine("Faire des Cartes à partir de GéoPortail (Capture)")
        S.AppendLine("Contact : " & Contact)
        S.AppendLine("Dimensions Ecran : " & DimensionsEcranSupport.Width & "*" & DimensionsEcranSupport.Height)
        S.AppendLine("Nom Ecran : " & Screen.AllScreens(PartagerSettings.NUM_ECRAN).DeviceName)
        S.AppendLine("Site de capture : " & SitesCartoLibelles(CapturerSettings.SITE))
        S.AppendLine("Memoire dispo : " & (GetTotalMemory / 1024 ^ 3).ToString("0.00") & " Go, Nb Processeurs" & Environment.ProcessorCount)
        S.AppendLine("Support capture : " & SupportsCarteNom(CartesSettings.SUPPORT_CARTE))
        MessageInformation = S.ToString
        AfficherInformationEtLien(Me, "Visitez le site FCGP", "https://fcgp.e-monsite.com")
    End Sub
    ''' <summary> ouvre le formulaire de configuration </summary>
    Private Sub OuvrirConfiguration()
        Dim NumEcran As Integer = PartagerSettings.NUM_ECRAN
        Dim CouleurFond As Color = PartagerSettings.COULEUR_VISUE
        Dim SupportCarte As TypeSupportCarte = CartesSettings.SUPPORT_CARTE
        Dim RepAncien As String = TracesSettings.CHEMIN_TRACE
        'on demande à l'utilisateur le site sur lequel il va travailler et sur quel écran
        Using R As Form = New Configuration
            R.ShowDialog(Me)
        End Using
        If NumEcran <> PartagerSettings.NUM_ECRAN Then
            Dim FlagMaximize = WindowState = FormWindowState.Maximized
            If FlagMaximize Then WindowState = FormWindowState.Normal
            DimensionsEcranSupport = Screen.AllScreens(PartagerSettings.NUM_ECRAN).Bounds
            DimensionnerAffichage()
            If FlagMaximize Then WindowState = FormWindowState.Maximized
        End If
        If TracesSettings.CHEMIN_TRACE <> RepAncien Then
            TRK.InitialiserTraces()
            GererMenuTrace()
        End If
        If SupportCarte <> CartesSettings.SUPPORT_CARTE Then
            'InitialiserTelechargement()
            GererIUMenuPts()
        End If
        If CouleurFond <> PartagerSettings.COULEUR_VISUE Then Affichage.CouleurFond = PartagerSettings.COULEUR_VISUE
    End Sub
    ''' <summary> ouvre le formulaire de configuration des captures de cartes </summary>
    Private Async Sub OuvrirPreferencesAsync()
        If Not FlagCapture Then
            Using R As New Preferences With {.SurfaceTraitement = SurfaceTraitement}
                R.Tag = False
                If TRK.IsTraceEncours Then
                    'pour mettre à jour l'emprise de la trace et le fichier qui peut servir pour le dessin de la carte
                    TRK.EnregistrerTraceEncours()
                    R.Tag = SurfaceTraitement.RectanglePixels.IntersectsWith(TRK.TraceEncours.BoundingBoxVirtuel)
                End If
                If R.ShowDialog(Me) = DialogResult.OK Then
                    FlagCapture = True
                    LanceCreationCarte.Text = "Annuler Création ..."
                    ConfigureApp.Enabled = False
                    LabelInformation.BackColor = Color.SeaShell
                    Dim NomCarte As String = R.Tag.ToString()
                    Await RealiserCarteAsync(NomCarte, SurfaceTraitement, LabelInformation)
                    'on remet tout en ordre pour la prochaine capture
                    FlagCapture = False
                    LanceCreationCarte.Text = "Lancer Création ..."
                    ConfigureApp.Enabled = True
                End If
            End Using
        Else
            MessageInformation = "Voulez vous annuler le téléchargement des tuiles?"
            If AfficherConfirmation() = DialogResult.OK Then
                FlagAnnulerTelechargement = True
            End If
        End If
    End Sub
    ''' <summary> ouvre le formulaire de configuration des captures de cartes </summary>
    ''' <param name="SurfaceTraitement"> surfacetuiles représentée par 2 points exprimés en mètre de la grille du site d</param>
    Private Sub OuvrirSurfaceTraitement(SurfaceTraitement As SurfaceTuiles)
        Using R As Form = New SaisieSurfaceTraitement
            R.Tag = (SurfaceTraitement, ListeChoixTypeCoordonnees.SelectedIndex)
            If R.ShowDialog(Me) = DialogResult.OK Then
                Dim Retour As SurfaceTuiles = CType(R.Tag, SurfaceTuiles)
                CapturerSettings.POINT1 = Retour.PtGrille_NO
                CapturerSettings.POINT2 = Retour.PtGrille_SE
                GererIUMenuPts()
            End If
        End Using
    End Sub
    ''' <summary> Lance l'ouverture du formulaire qui permet l'impression d'une carte </summary>
    Private Shared Sub OuvrirImprimerCarte()
        If IsPrinting Then
            MessageInformation = "Veuillez attendre la fin" & CrLf &
                                 "de l'impression en cours"
            AfficherInformation()
        Else
            LancerImpressionCarteAsync()
        End If
    End Sub
    ''' <summary> ouvre la trace qui sera éditée sur l'affichage donc la trace encours </summary>
    Private Sub OuvrirTraceAffichage()
        If TRK.OuvrirTraceEncours(AltitudesSettings.IS_ALTITUDE, False) Then
            AllerAuPointTrace(True)
            'on indique le nom de la trace en cours
            TRK.TraceEncours.ModeEdition = True
            GererMenuTrace()
        End If
    End Sub
    ''' <summary> ferme la trace qui est éditée sur l'affichage donc la trace encours </summary>
    Private Sub FermerTraceAffichage()
        If TRK.IsTraceEncours Then
            If TRK.FermerTraceEncours(True) Then
                GererMenuTrace()
            End If
        End If
    End Sub
    ''' <summary> ouvre le formulaire de propriété des traces </summary>
    Private Sub ProprietesTraceAffichage()
        Dim Index As Integer = -1
        Dim Repeindre As Boolean = TRK.ChangerProprietesTraceEncours(Index)
        If Index > -1 Then
            'un point précis de la trace a été selectionné
            CapturerSettings.LOCATION_CENTRE = TRK.TraceEncours.CoordonneesVirtuelles(Index)
            AllerA()
        ElseIf Repeindre Then
            SurfaceAffichageTuile.Invalidate() 'changement de couleur
        End If
        LabelTrace.Text = TRK.TraceEncours.Nom 'changement de nom
    End Sub
    ''' <summary> positionne le centre de l'affichage au coordonnées définit par CapturerSettings.LOCATION_CENTRE </summary>
    Private Sub AllerA()
        FlagInit = True
        GererChangementTaille()
        'déplace le cuseur au centre de l'affichage
        DeplacerCurseurCentreVisue()
        FlagInit = False
    End Sub
    ''' <summary> positionne le centre de l'affichage en fonction d'un des points de capture </summary>
    Private Sub AllerAuPointCapture()
        GererMenuRevenir(True)
        CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(PointEncours, CapturerSettings.SITE, Affichage.Echelle)
        AllerA()
    End Sub
    ''' <summary> positionne le centre de l'affichage en fonction du centre du site de capture </summary>
    Private Sub AllerAuCentreSite()
        GererMenuRevenir(True)
        Dim EchelleCentre = NiveauDetailCartographique.SiteIndiceEchelleToEchelle(Serveur.SiteCarto, Serveur.Centre.IndiceEchelle)
        Dim SiteCentreGrille = PointPixelToPointGrille(Serveur.Centre.Location, Serveur.SiteCarto, EchelleCentre)
        CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(SiteCentreGrille, Serveur.SiteCarto, Affichage.Echelle)
        AllerA()
    End Sub
    ''' <summary> positionne le centre de l'affichage sur le point de début ou de fin de la trace </summary>
    ''' <param name="Debut"> Point de début si true, point de fin si false </param>
    Private Sub AllerAuPointTrace(Debut As Boolean)
        'si la trace a au moins un point on peut le positionner au centre de l'affichage
        If TRK.TraceEncours.NbPoints > 0 Then
            Dim Cpt As Integer = If(Debut, 0, TRK.TraceEncours.NbPoints - 1)
            GererMenuRevenir(True)
            CapturerSettings.LOCATION_CENTRE = TRK.TraceEncours.CoordonneesVirtuelles(Cpt)
            AllerA()
        End If
    End Sub
    ''' <summary> positionne le centre de l'affichage en fonction d'un des points de la surface de traitement </summary>
    ''' <param name="Pt">N° du point de 0 à 3</param>
    Private Sub AllerAuPointTraitementSurface(Pt As Integer)
        If Not SurfaceTraitement.IsEmpty Then
            GererMenuRevenir(True)
            CapturerSettings.LOCATION_CENTRE = SurfaceTraitement.PtPixels(Pt)
            AllerA()
        End If
    End Sub
    ''' <summary> positionne le centre de l'affichage tel qu'il était avant une action AllerA </summary>
    Private Sub RevenirDe()
        GererMenuRevenir(False)
        CapturerSettings.LOCATION_CENTRE = PointGrilleToPointPixel(CoordonneesRevenir, CapturerSettings.SITE, Affichage.Echelle)
        AllerA()
    End Sub
    ''' <summary> bascule permettant de mettre le mode Edition de la trace On/Off </summary>
    Private Sub InverserEditionTrace()
        If TRK.IsTraceEncoursModeEdition Then
            TRK.TraceEncours.ModeEdition = False
            CurseurEncours = Curseurs.CarteDefaut
        Else
            TRK.TraceEncours.ModeEdition = True
            CurseurEncours = Curseurs.TraceDefaut
        End If
        'on indique qu'on active ou désactive le mode trace sur l'interface
        GererIUMenuTrace()
    End Sub
    ''' <summary> autorise ou interdit les menus Revenir </summary>
    Private Sub GererMenuRevenir(FlagAutoriser As Boolean)
        If FlagAutoriser Then
            CoordonneesRevenir = PointPixelToPointGrille(Affichage.CentreAffichage, CapturerSettings.SITE, Affichage.Echelle)
        End If
        RevientDe_MenuTrace.Enabled = FlagAutoriser
        RevientDe_Pts.Enabled = FlagAutoriser
        RevientDe_App.Enabled = FlagAutoriser
        FlagRevenir = FlagAutoriser
    End Sub
    ''' <summary> gère l'apparence des MenuPts </summary>
    Private Sub GererIUMenuPts()
        LabelMenuPt1.BorderStyle = BorderStyle.None
        LabelMenuPt2.BorderStyle = BorderStyle.None
        If CapturerSettings.POINT1.IsEmpty Then
            LabelMenuPt1.BackColor = Color.Transparent
        Else
            LabelMenuPt1.BackColor = Color.Orange
        End If
        If CapturerSettings.POINT2.IsEmpty Then
            LabelMenuPt2.BackColor = Color.Transparent
        Else
            LabelMenuPt2.BackColor = Color.Orange
        End If
        CalculerSurfaceTraitement()
        If Not CapturerSettings.POINT1.IsEmpty AndAlso Not CapturerSettings.POINT2.IsEmpty Then
            If Not SurfaceTraitementIsOk Then
                LabelMenuPt1.BackColor = Color.Tomato
                LabelMenuPt2.BackColor = Color.Tomato
            Else
                LabelMenuPt1.BackColor = Color.YellowGreen
                LabelMenuPt2.BackColor = Color.YellowGreen
            End If
        End If
        GererMenuLancerCreation()
        LabelMenuPt1.Refresh()
        LabelMenuPt2.Refresh()
        SurfaceAffichageTuile.Invalidate()
    End Sub
    ''' <summary> gére le menu associé à un point </summary>
    Private Sub GererMenuPts()
        SaisieSurfaceTraitementPts.Enabled = Not SurfaceTraitement.IsEmpty
        Dim Modifier = If(NomPointEncours = "Pt1", Keys.Shift, Keys.Control)
        Dim IsEmpty = If(NomPointEncours = "Pt1", CapturerSettings.POINT1.IsEmpty, CapturerSettings.POINT2.IsEmpty)
        SaisieSurfaceTraitementPts.ShortcutKeys = Modifier Or Keys.F7
        DefinitCentrePts.Text = $"Définir {NomPointEncours} au centre"
        DefinitCentrePts.ShortcutKeys = Modifier Or Keys.F8
        EffacePts.Enabled = Not IsEmpty
        EffacePts.Text = $"Effacer {NomPointEncours}"
        EffacePts.ShortcutKeys = Modifier Or Keys.F9
        Va_A_Pts.Enabled = Not IsEmpty
        Va_A_Pts.Text = $"Aller à {NomPointEncours}"
        Va_A_Pts.ShortcutKeys = Modifier Or Keys.F6
        ContextMenuPts.Tag = If(NomPointEncours = "Pt1", LabelMenuPt1, LabelMenuPt2)
    End Sub
    ''' <summary> Gère l'apparence du Menu Trace </summary>
    Private Sub GererIUMenuTrace()
        If TRK.IsTraceEncoursModeEdition Then
            CurseurEncours = Curseurs.TraceDefaut
            LabelTrace.BackColor = Color.Orange
            EditeTrace.Text = "&Edition Trace Off"
        ElseIf TRK.IsTraceEncours Then
            CurseurEncours = Curseurs.CarteDefaut
            LabelTrace.BackColor = Color.YellowGreen
            EditeTrace.Text = "&Edition Trace On"
        Else
            CurseurEncours = Curseurs.CarteDefaut
            LabelTrace.BackColor = Color.Transparent
            EditeTrace.Text = "&Edition Trace On"
        End If
    End Sub
    ''' <summary> gère le menu trace </summary>
    Private Sub GererMenuTrace()
        Dim FlagTrace = TRK.IsTraceEncours
        LabelTrace.Text = If(FlagTrace, TRK.TraceEncours.Nom, "Pas de trace")
        FermeTrace.Enabled = FlagTrace
        Va_A_DebutTrace.Enabled = FlagTrace
        Va_A_FinTrace.Enabled = FlagTrace
        ProprietesTrace.Enabled = FlagTrace
        EditeTrace.Enabled = FlagTrace
        GererIUMenuTrace()
        SurfaceAffichageTuile.Invalidate()
    End Sub
    ''' <summary> gère l'accessibilité au menu lancer </summary>
    Private Sub GererMenuLancerCreation()
        If FlagCapture Then
            'on autorise toujours l'annulation d'une carte
            LanceCreationCarte.Enabled = True
        Else
            LanceCreationCarte.Enabled = SurfaceTraitementIsOk AndAlso Serveur.NbDemandesAffichage = 0 AndAlso IsConnected
        End If
        SupprimeTuiles.Enabled = SurfaceTraitementIsOk
    End Sub
    ''' <summary> Finit réellement l'action définirPoint au relevé soit de la touche Menu soit du bouton gauche de la souris mais indique que 
    ''' l'action n'est pas totalement terminer tant que l'autre n'est pas relevé afin d'interdir les événements Down (clavier et souris) </summary>
    Private Sub FinirActionDefinirPoints()
        'action qui ne sert qu'à attendre le relèvement de la touche Alt ou du bouton gauche de la souris
        Action = Actions.FinirDefinirPoints
        TrouverCurseurCarte()
    End Sub
    ''' <summary> trouve le curseur par défaut de la carte soit celui de la carte soit celui de la trace si il y en a une en mode édition </summary>
    Private Sub TrouverCurseurCarte()
        'determine le curseur en fonction de la trace
        If TRK.IsTraceEncoursModeEdition Then
            If Action = Actions.DeplacerCarte Then
                CurseurEncours = Curseurs.TraceDeplacement
            Else
                CurseurEncours = Curseurs.TraceDefaut
            End If
        Else
            If Action = Actions.DeplacerCarte Then
                CurseurEncours = Curseurs.CarteDeplacement
            Else
                CurseurEncours = Curseurs.CarteDefaut
            End If
        End If
    End Sub
    ''' <summary> renvoie le pointencours </summary>
    Private Property PointEncours As PointD
        Get
            If NomPointEncours = "Pt1" Then Return CapturerSettings.POINT1
            If NomPointEncours = "Pt2" Then Return CapturerSettings.POINT2
            Return Nothing
        End Get
        Set(value As PointD)
            If NomPointEncours = "Pt1" Then
                CapturerSettings.POINT1 = value
            ElseIf NomPointEncours = "Pt2" Then
                CapturerSettings.POINT2 = value
            End If
        End Set
    End Property
    ''' <summary> Définit le pointencours avec les coordonnées de la souris </summary>
    Private Sub DefinirPoint()
        If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.Contains(CoordMousePixels) Then
            TitreInformation = "Action Hors Limites"
            MessageInformation = "Le pointeur de souris est en dehors des limites du site de capture."
            AfficherInformation()
        Else
            PointEncours = CoordMouseGrille
            GererIUMenuPts()
        End If
    End Sub
    ''' <summary> Définit le pointencours avec les coordonnées du centre de l'affichage </summary>
    Private Sub DefinirPointCentre()
        If FlagLimitesSite AndAlso Not LimitesVirtuellesSite.Contains(Affichage.CentreAffichage) Then
            MessageInformation = "Le centre de l'affichage est en dehors des limites du site de capture."
            AfficherInformation()
        Else
            DeplacerCurseurCentreVisue()
            'on attend que le curseur de la souris soit bien positioné par le systeme
            Attendre(150)
            DefinirPoint()
        End If
    End Sub
    ''' <summary> Efface le point </summary>
    Private Sub EffacerPoint()
        PointEncours = New PointD
        GererIUMenuPts()
    End Sub
    ''' <summary> demande au système de placer le curseur de souris au centre de l'affichage </summary>
    Private Sub DeplacerCurseurCentreVisue()
        Cursor.Position = New Point(VisueRectangle.X + CentreAffichage.X, VisueRectangle.Y + CentreAffichage.Y)
    End Sub
    ''' <summary> indique si la surfacecapture est bien définie pour la capture </summary>
    Private Sub CalculerSurfaceTraitement()
        SurfaceTraitement = New SurfaceTuiles(CapturerSettings.POINT1, CapturerSettings.POINT2, CapturerSettings.SITE, Affichage.Echelle)
        SurfaceTraitementIsOk = Not SurfaceTraitement.IsEmpty AndAlso SurfaceTraitement.NbTuiles <= NB_Max_TuilesServeurCarto AndAlso
                                SurfaceTraitement.NbColonnes <= NB_Max_Tuiles_X(CartesSettings.SUPPORT_CARTE, 1)
        If FlagLimitesSite AndAlso Not SurfaceTraitement.IsEmpty Then
            'on doit vérifier que les 2 points de la surface de traitement sont dans la limite du site
            Dim Pt1 = PointGrilleToPointPixel(CapturerSettings.POINT1, CapturerSettings.SITE, Affichage.Echelle)
            Dim Pt2 = PointGrilleToPointPixel(CapturerSettings.POINT2, CapturerSettings.SITE, Affichage.Echelle)
            If Not (LimitesVirtuellesSite.Contains(Pt1) AndAlso LimitesVirtuellesSite.Contains(Pt2)) Then
                MessageInformation = "Au moins un des points de la surface de traitement" & CrLf &
                                     "est en dehors des limites du site de capture."
                AfficherInformation()
                SurfaceTraitementIsOk = False
            End If
        End If
    End Sub
    ''' <summary> autorise ou non le dépassement des limites du site de caapture </summary>
    Private Sub AppliquerLimitesSite()
        FlagLimitesSite = Not FlagLimitesSite
        LimitesSite.Text = $"Limites site : {If(FlagLimitesSite, "Off", "On")}"
        CalculerSurfaceTraitement()
        SurfaceAffichageTuile.Invalidate()
    End Sub
#End Region
End Class
