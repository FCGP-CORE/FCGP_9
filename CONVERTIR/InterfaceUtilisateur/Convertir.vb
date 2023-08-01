''' <summary> formulaire principal de l'application FCGP_Convertir </summary>
Friend NotInheritable Class Convertir
#Region "Variables globales"
    ''' <summary> remplace le controle FolderBrowser classique du windowsForm </summary>
    Private ChercheChemin As FolderBrowserDialog
    ''' <summary> nom de la carte en cours de traitement </summary>
    Private NomCarte As String
    ''' <summary> format d'enregistrement de la carte en cours de traitement </summary>
    Private FormatCarte As ImageFormat
    ''' <summary> support de la carte en cours de traitement </summary>
    Private SupportCarte As TypeSupportCarte
    ''' <summary> autorise le format d'enregistrment de la carte en cours de traitement </summary>
    Private ToutFormat As Boolean
    ''' <summary> indique si l'utilisateur à demandé l'annulation lors d'un traitement à plusieurs carte (mode batch) </summary>
    Private FlagAnnuler As Boolean
    ''' <summary> indique que l'application est en cours d'initialisation </summary>
    Private FlagInit As Boolean
    ''' <summary> indique si les 3 listes de choix sont libres ou gérées par le nb de path (choix imposés)</summary>
    Private FlagNbPahsChanged As Boolean
    ''' <summary> indique qu'il faudra enregistrer les choix de l'utilisateur en sortant de l'application </summary>
    Private FlagEnregisterSettings As Boolean
    ''' <summary> variables de sauvegarde associées au 3 listes de choix du formulaire </summary>
    Private SauvegardeIndexFichierTuile, SauvegardeIndexGeoref, SauvegardeIndexChoixReferences As Integer
    ''' <summary> sauvegarde temporaire </summary>
    Private ValeurDimTuilesLarg As Decimal
    ''' <summary> sauvegarde temporaire de la liste de choix ouverte </summary>
    Private ComboxDropDown As ComboBox
    ''' <summary> liste pour stocker les traces sélectionnées </summary>
    Private FichiersTRK As List(Of String)
    ''' <summary> Nb de tuiles serveur sur l'axe des X </summary>
    Private FlagNbTuilesX As Boolean
    ''' <summary> traduction index text en valeur et vice versa pour les listes de choix </summary>
    Private ReadOnly FichiersTuilesNoms As String() = {"Aucun", "ORUX", "JNX", "KMZ", "JNX KMZ", "JNX ORUX", "KMZ ORUX", "Tous"}
    Private ReadOnly FichiersTuilesValeurs As ChoixFichiersTuiles() = {ChoixFichiersTuiles.Aucun, ChoixFichiersTuiles.ORUX,
                                                                       ChoixFichiersTuiles.JNX, ChoixFichiersTuiles.KMZ,
                                                                       ChoixFichiersTuiles.JNX_KMZ, ChoixFichiersTuiles.JNX_ORUX,
                                                                       ChoixFichiersTuiles.KMZ_ORUX, ChoixFichiersTuiles.Tous}
    Private ReadOnly FichiersGeorefNoms As String() = {"Aucun", "Cartes",
                                                       "Compeland Carte", "Compeland Interpol",
                                                       "Compelands", "Georef Carte",
                                                       "Georef Interpol", "Georefs",
                                                       "Interpols", "MapInfo Carte",
                                                       "MapInfo Interpol", "MapInfos",
                                                       "OziExploreur Carte", "OziExploreur Interpol",
                                                       "OziExploreurs", "QGIS Carte",
                                                       "QGIS Interpol", "QGISs",
                                                       "Tous"}
    Private ReadOnly FichiersGeorefValeurs As ChoixFichiersGeoref() = {ChoixFichiersGeoref.Aucun, ChoixFichiersGeoref.Cartes,
                                                                       ChoixFichiersGeoref.CompeLand_Carte, ChoixFichiersGeoref.CompeLand_Interpol,
                                                                       ChoixFichiersGeoref.CompeLands, ChoixFichiersGeoref.Georef_Carte,
                                                                       ChoixFichiersGeoref.Georef_Interpol, ChoixFichiersGeoref.Georefs,
                                                                       ChoixFichiersGeoref.Interpols, ChoixFichiersGeoref.MapInfo_Carte,
                                                                       ChoixFichiersGeoref.MapInfo_Interpol, ChoixFichiersGeoref.MapInfos,
                                                                       ChoixFichiersGeoref.OziExploreur_Carte, ChoixFichiersGeoref.OziExploreur_Interpol,
                                                                       ChoixFichiersGeoref.OziExploreurs, ChoixFichiersGeoref.QGIS_Carte,
                                                                       ChoixFichiersGeoref.QGIS_Interpol, ChoixFichiersGeoref.QGISs,
                                                                       ChoixFichiersGeoref.Tous}
    Private ReadOnly GrillesReferenceValeurs As ChoixCoordonneesGrille() = {ChoixCoordonneesGrille.Aucune, ChoixCoordonneesGrille.Ref_Carte,
                                                                            ChoixCoordonneesGrille.Ref_Interpol, ChoixCoordonneesGrille.Toutes}
    Private ReadOnly NbTuilesMaxCarteSuisseInterpol() As Integer = New Integer() {410, 410, 205}
#End Region
#Region "Evenements du formulaire et des controles"
    ''' <summary> initialisation complète de tous les controles à partir du settings </summary>
    Private Sub Convertir_Load(sender As Object, e As EventArgs)
        If Not InitialiserBaseApplication(Me, "Core 4.000 VB", "01/03/2023") Then
            Close()
            Exit Sub
        End If
        TitreInformation = "Information FCGP_Convertir"
        'indique aux threads hors UI la procédure à appeler pour afficher un message d'information ou une erreur
        FormHorsUI = New Action(AddressOf AfficherInfomationsErreurs)
        'si la base des settings n'existe pas elle est crée avec des valeurs par défaut
        If Not InitialiserSettings("9.000") Then
            Close()
            Exit Sub
        End If
        If InitialiserCommun() Then
            FlagInit = True
            MessageInformation = ""
            LabelInformation = Information
            'met à jour les listes déroulantes des choix qui permettent de produire une sortie de fichier. Celles-ci sont associées à une variable de sauvegarde
            GeoRefs.Items.AddRange(FichiersGeorefNoms)
            References.Items.AddRange(CoordonneesGrille)
            FichiersTuiles.Items.AddRange(FichiersTuilesNoms)
            'sélectionne les derniers choix effectués
            GeoRefs.SelectedIndex = ConvertirSettings.GEOREF
            References.SelectedIndex = ConvertirSettings.REFERENCE
            FichiersTuiles.SelectedIndex = ConvertirSettings.FICHIER_TUILE
            FormatCartes.SelectedIndex = ConvertirSettings.FORMAT
            IsGrille.Checked = ConvertirSettings.IS_GRILLE
            IsTrace.Checked = ConvertirSettings.IS_TRACE
            DimTuilesLarg.Value = ConvertirSettings.DIMENSIONS_TUILE
            Couleur.BackColor = ConvertirSettings.COULEUR_TRACE
            Transparence.Value = ValeurToPourcent(Couleur.BackColor.A)
            Epaisseur.Value = ConvertirSettings.EPAISSEUR_TRACE
            NbPaths.Value = ConvertirSettings.NB_PATHS
            LabelRepertoireCartes.Text = ConvertirSettings.CHEMIN_CARTE
            LabelRepertoireTuiles.Text = ConvertirSettings.CHEMIN_TUILE
            'initialisation des autres contrôlesdépendant d'un autre settings
            OuvrirFichiersTrace.Title = "Choisir une ou plusieurs traces"
            OuvrirFichiersTrace.InitialDirectory = TracesSettings.CHEMIN_TRACE
            OuvrirFichiersCarte.Title = "Choisir une ou plusieurs cartes pour le traitement"
            OuvrirFichiersCarte.InitialDirectory = CapturerSettings.CHEMIN_CARTE
            FichiersTRK = New List(Of String)(5)
            Text = NumFCGP
            FlagInit = False
            InitialiserConvertir()
            With VisueRectangle
                Location = New Point(.X + .Width \ 2 - Size.Width \ 2, .Y + .Height \ 2 - Size.Height \ 2)
            End With
            ChercheChemin = New FolderBrowserDialog()
#If Not BETA Then
            VerifierVersionApplication()
#End If
        Else
            Close()
        End If
    End Sub
    ''' <summary> vérifie que le formulaire de l'application peut être fermé suite à une demande de l'utilisateur </summary>
    Private Sub Convertir_FormClosing(sender As Object, e As FormClosingEventArgs)
        If Traitement.Text.StartsWith("Annuler") Then
            MessageInformation = "Vous devez attendre la fin du traitement."
            AfficherInformation()
            e.Cancel = True
        End If
    End Sub
    ''' <summary> ferme le formulaire de l'application suite à une demande de l'utilisateur </summary>
    Private Sub Convertir_FormClosed(sender As Object, e As FormClosedEventArgs)
        'on libère les ressources et efface le répertoire provisoire créé au lancement de FCGP
        CloturerCommun()
        If FlagEnregisterSettings Then
            ConvertirSettings.GEOREF = SauvegardeIndexGeoref
            ConvertirSettings.REFERENCE = SauvegardeIndexChoixReferences
            ConvertirSettings.FICHIER_TUILE = SauvegardeIndexFichierTuile
            ConvertirSettings.Ecrire()
        End If
    End Sub
    ''' <summary> on récupère toutes le frappe de clavier à cet endroit pour traiter l'ouverture du formulaire d'aide </summary>
    Private Sub Convertir_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Escape
                If ComboxDropDown IsNot Nothing Then
                    ComboxDropDown.DroppedDown = False
                    ComboxDropDown = Nothing
                End If
            Case Keys.F1
                AfficherAide(Me)
        End Select
    End Sub
    ''' <summary> connaitre l'écran sur lequel le formulaire est affiché</summary>
    Private Sub Convertir_LocationChanged(sender As Object, e As EventArgs)
        VisueRectangle = RectangleToScreen(ClientRectangle)
    End Sub
    ''' <summary> met ajour la couleur de fond du label en fonction d'un choix de couleur et du % de transparence </summary>
    Private Sub Couleur_Click(sender As Object, e As EventArgs)
        ChoixCouleurTrace.Color = Couleur.BackColor
        If ChoixCouleurTrace.ShowDialog() = DialogResult.OK Then
            Couleur.BackColor = Color.FromArgb(PourcentToValeur(CInt(Transparence.Value)), ChoixCouleurTrace.Color)
        End If
    End Sub
    ''' <summary> action à réaliser lors la modification du check du controle </summary>
    Private Sub IsTrace_CheckedChanged(sender As Object, e As EventArgs)
        DefinirEtatControles()
    End Sub
    ''' <summary> met à jour la couleur du label couleur en fonction du pourcentage de transparence désiré par l'utilisateur </summary>
    Private Sub Transparence_ValueChanged(sender As Object, e As EventArgs)
        Couleur.BackColor = Color.FromArgb(PourcentToValeur(CInt(Transparence.Value)), Couleur.BackColor)
    End Sub
    ''' <summary> permet de changer le répertoire d'enregistrement des cartes issues du traitement </summary>
    Private Sub LabelRepertoireCartes_Click(sender As Object, e As EventArgs)
        Try
            ChercheChemin.Description = "Chemin d'enregistrement des fichiers de géo-référencement"
            ChercheChemin.InitialDirectory = LabelRepertoireCartes.Text
            'Affiche la boite de dialogue de sélection de répertoires
            If ChercheChemin.ShowDialog(Me) = DialogResult.OK Then
                'et met à jour le champ
                LabelRepertoireCartes.Text = ChercheChemin.SelectedPath
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "G4M6")
        End Try
    End Sub
    ''' <summary> permet de changer le répertoire d'enregistrement des fichiers tuiles issus du traitement </summary>
    Private Sub LabelRepertoireFichiersTuiles_Click(sender As Object, e As EventArgs)
        Try
            ChercheChemin.Description = "Chemin d'enregistrement des fichiers tuiles"
            ChercheChemin.InitialDirectory = LabelRepertoireTuiles.Text
            'Affiche la boite de dialogue de sélection de répertoires
            If ChercheChemin.ShowDialog(Me) = DialogResult.OK Then
                'et met à jour le champ
                LabelRepertoireTuiles.Text = ChercheChemin.SelectedPath
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "R7C3")
        End Try
    End Sub
    ''' <summary> permet de sélectionner une ou plusieurs traces de type FCGP (.trk) </summary>
    Private Sub LabelTraces_Click(sender As Object, e As EventArgs)
        Try
            'Affiche la boite de dialogue de sélection des traces
            If OuvrirFichiersTrace.ShowDialog() = DialogResult.OK Then
                OuvrirFichiersTrace.InitialDirectory = Path.GetDirectoryName(OuvrirFichiersTrace.FileName)
                'et met à jour la liste déroulante des traces choisies
                Traces.Items.Clear()
                FichiersTRK.Clear()
                For Each T As String In OuvrirFichiersTrace.FileNames
                    Dim Ext As String = Path.GetExtension(T)
                    If Ext = ".trk" Then
                        Traces.Items.Add(Path.GetFileNameWithoutExtension(T))
                        FichiersTRK.Add(T)
                    End If
                Next
                If Traces.Items.Count > 0 Then
                    LabelTraces.Text = CStr(Traces.Items(0))
                    OuvrirFichiersTrace.InitialDirectory = Path.GetDirectoryName(FichiersTRK(0)) '
                End If
                DefinirEtatControles()
            End If
        Catch Ex As Exception
            AfficherErreur(Ex, "T4S5")
        End Try
    End Sub
    ''' <summary> le nb de paths détermine l'état des groupe Cartes et FichiersTuiles </summary>
    Private Sub NbPaths_ValueChanged(sender As Object, e As EventArgs)
        DefinirEtatControles()
    End Sub
    ''' <summary> ouvre la liste des choix associée au label </summary>
    Private Sub LabelListesChoix_Click(sender As Object, e As EventArgs)
        Dim L = CType(sender, Label)
        ComboxDropDown = CType(L.Tag, ComboBox)
        ComboxDropDown.DroppedDown = True
    End Sub
    ''' <summary> ferme la liste déroulante des choix de type de coordonnées </summary>
    Private Sub ListesChoix_DropDownClosed(sender As Object, e As EventArgs)
        ComboxDropDown = Nothing
    End Sub
    ''' <summary> met à jour le label associé à la liste des choix </summary>
    Private Sub FormatCartes_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelFormatCartes.Text = FormatCartes.SelectedItem.ToString()
    End Sub
    ''' <summary> met à jour le label associé à la liste des choix </summary>
    Private Sub References_SelectedIndexChanged(sender As Object, e As EventArgs)
        If Not FlagNbPahsChanged Then
            'si ce n'est pas la phase d'initialisation du formulaire
            SauvegardeIndexChoixReferences = References.SelectedIndex
        End If
        LabelReferences.Text = CoordonneesGrille(References.SelectedIndex)
    End Sub
    ''' <summary> met à jour le label associé à la liste des choix </summary>
    Private Sub GeoRefs_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelGeoRefs.Text = GeoRefs.SelectedItem.ToString()
        If Not FlagNbPahsChanged Then
            SauvegardeIndexGeoref = GeoRefs.SelectedIndex
            DefinirEtatControles()
        End If
    End Sub
    ''' <summary> action à réaliser lors de la modification de la sélection d'un index du controle et met à jour le label associé à la liste des choix </summary>
    Private Sub FichiersTuiles_SelectedIndexChanged(sender As Object, e As EventArgs)
        LabelFichiersTuiles.Text = FichiersTuiles.SelectedItem.ToString()
        If Not FlagNbPahsChanged Then
            SauvegardeIndexFichierTuile = FichiersTuiles.SelectedIndex
            DefinirEtatControles()
        End If
    End Sub
    ''' <summary> enlève la possibilité du choix 768</summary>
    Private Sub DimTuilesLarg_ValueChanged(sender As Object, e As EventArgs)
        If DimTuilesLarg.Value = 768 Then
            If ValeurDimTuilesLarg > 768 Then
                DimTuilesLarg.Value = 512
            Else
                DimTuilesLarg.Value = 1024
            End If
        End If
        ValeurDimTuilesLarg = DimTuilesLarg.Value
    End Sub
    ''' <summary> annule la changement d'index </summary>
    Private Sub Traces_SelectedIndexChanged(sender As Object, e As EventArgs)
        Traces.SelectedIndex = -1
    End Sub
    ''' <summary> action lié au bouton Quitter : Demande de fermeture du formulaire général </summary>
    Private Sub Quitter_Click(sender As Object, e As EventArgs)
        Close()
    End Sub
    ''' <summary> Lance le traitement des cartes si la sélection d'une ou plusieurs cartes est OK </summary>
    Private Sub Traitement_Click(sender As Object, e As EventArgs)
        If Not Traitement.Text.StartsWith("Annuler") Then
            If OuvrirFichiersCarte.ShowDialog() = DialogResult.OK Then
                FlagEnregisterSettings = True
                OuvrirFichiersCarte.InitialDirectory = Path.GetDirectoryName(OuvrirFichiersCarte.FileName)
                'met le bouton de traitement soit en mode Annuler si plusieurs cartes soit 
                'le rend inactif si une seule carte car on ne peut pas arrêter un traitment encours
                If Me.OuvrirFichiersCarte.FileNames.Length > 1 Then
                    Traitement.Text = "Annuler le traitement des autres cartes"
                    Informations.SetToolTip(Traitement, "Annulez les traitements restants" & CrLf &
                                                        "après le traitement en cours.")
                Else
                    Traitement.Enabled = False
                End If
                GroupeConfigurations.Enabled = False
                Quitter.Enabled = False
                'renseigne le setting avec les différents paramètres choisis par l'utilisateur
                ConvertirSettings.GEOREF = GeoRefs.SelectedIndex
                ConvertirSettings.FORMAT = FormatCartes.SelectedIndex
                ConvertirSettings.IS_GRILLE = IsGrille.Checked
                ConvertirSettings.REFERENCE = References.SelectedIndex
                ConvertirSettings.FICHIER_TUILE = FichiersTuiles.SelectedIndex
                ConvertirSettings.DIMENSIONS_TUILE = CInt(DimTuilesLarg.Value)
                ConvertirSettings.CHEMIN_CARTE = LabelRepertoireCartes.Text
                ConvertirSettings.CHEMIN_TUILE = LabelRepertoireTuiles.Text
                ConvertirSettings.IS_TRACE = IsTrace.Checked
                ConvertirSettings.COULEUR_TRACE = Couleur.BackColor
                ConvertirSettings.EPAISSEUR_TRACE = CInt(Epaisseur.Value)
                ConvertirSettings.NB_PATHS = CInt(NbPaths.Value)
                TraitementAsync(OuvrirFichiersCarte.FileNames)
            End If
        Else
            If Not FlagAnnuler Then FlagAnnuler = True
        End If
    End Sub
#End Region
#Region "Traitement des cartes"
    ''' <summary> Traitement des cartes séléctionées. On lance le travail et rend la main immédiatement
    ''' de manière à pouvoir annuler le traitement si il y a plusieurs cartes à traiter </summary>
    ''' <param name="FichiersGeorefInit"></param>
    Private Async Sub TraitementAsync(FichiersGeorefInit As String())
        Dim SystemeType As SystemeCartographique = Nothing, Cpt As Integer
        'pour chaque fichier georef sélectionné
        For Cpt = 0 To FichiersGeorefInit.Length - 1
            If Not FlagAnnuler Then
                TrouverParametresCarte(FichiersGeorefInit(Cpt))
                If FlagNbTuilesX Then
                    MessageInformation = $"La carte {NomCarte} ne supporte pas d'être interpolée.{CrLf}Elle ne sera pas prise en compte."
                    AfficherInformation()
                Else
                    SystemeType = Carte.RenvoyerSystemeCartographique(FichiersGeorefInit(Cpt))
                    If SystemeType?.IsOk Then
                        'on cherche les différents paramètre permettant le dimensionnement du tampon
                        Information.Text = "Chargement de la carte : " & CrLf & NomCarte
                        Await Task.Run(Function() TraitementCarte(FichiersGeorefInit(Cpt)))
                    Else
                        If SystemeType Is Nothing Then
                            MessageInformation = $"La carte {NomCarte} est une carte interpolée.{CrLf}Elle ne sera pas prise en compte."
                        Else
                            MessageInformation = $"Le système cartographique de la carte {NomCarte} n'est pas reconnu.{CrLf}Elle ne sera pas prise en compte."
                        End If
                        AfficherInformation()
                    End If
                End If
            End If
        Next
        InitialiserConvertir()
    End Sub
    ''' <summary> traitement pour une carte </summary>
    ''' <param name="FichierGeoref"></param>
    ''' <returns></returns>
    Private Function TraitementCarte(FichierGeoref As String) As VerificationRenvoiCarte
        Dim Ret As VerificationRenvoiCarte
        Using CarteActuelle As Carte = Carte.RenvoyerCarte(FichierGeoref, Ret, SupportCarte, ToutFormat,, FichiersTuilesValeurs(ConvertirSettings.FICHIER_TUILE))
            If Ret = VerificationRenvoiCarte.OK Then
                With CarteActuelle
                    .Separateur = CrLf '" "
                    .Format = FormatCarte
                    .CheminCarte = ConvertirSettings.CHEMIN_CARTE
                    .CheminFichiersTuile = ConvertirSettings.CHEMIN_TUILE
                    .FacteurJNX = FacteursSettings.Lire(.SystemCartographique.IndiceSiteCarto, .SystemCartographique.IndiceEchelle).FacteurJNX
                    .FacteurORUX = FacteursSettings.Lire(.SystemCartographique.IndiceSiteCarto, .SystemCartographique.IndiceEchelle).FacteurORUX
                    .DemandeAfficherGrille = ConvertirSettings.IS_GRILLE
                    .DemandeAjouterCoordonneesGrille = GrillesReferenceValeurs(ConvertirSettings.REFERENCE)
                    .DemandeFichiersGeoref = FichiersGeorefValeurs(ConvertirSettings.GEOREF)
                    .PinceauTrace = New Pen(ConvertirSettings.COULEUR_TRACE, ConvertirSettings.EPAISSEUR_TRACE) With {.LineJoin = LineJoin.Round}
                    .FichiersTRK = FichiersTRK
                    .DemandeAfficherTrace = ConvertirSettings.IS_TRACE AndAlso ConvertirSettings.NB_PATHS = 0
                    .NbTracesKML = If(ConvertirSettings.IS_TRACE, ConvertirSettings.NB_PATHS, 0)
                    .RealiserDemandes()
                End With
            Else
                MessageInformation = $"Le fichier de la carte{NomCarte} n'est pas conforme"
                AfficherInformation()
            End If
        End Using
        Return Ret
    End Function
    ''' <summary> calcule le nom de base, le format et la taille du tampon support pour la carte au début du traitement </summary>
    ''' <param name="FichierGeoref"></param>
    Private Sub TrouverParametresCarte(FichierGeoref As String)
        Dim LignesGeoref As String() = File.ReadAllLines(FichierGeoref, Encoding_FCGP)
        Dim Pos As Integer = LignesGeoref(0).IndexOf("."c) + 1
        'Trouve le système de capture de la carte
        Dim Site = TrouverChaineCompriseEntre(LignesGeoref(0), Pos, ":"c)
        Dim CheminCarte As String = TrouverChaineCompriseEntre(LignesGeoref(0), 0, ":"c, ","c).Trim(" "c)
        Dim S() As String = CheminCarte.Split("."c)
        NomCarte = S(0)
        FormatCarte = ConvertIntToImageFormat(ConvertirSettings.FORMAT)

        Pos = 0
        Dim LargPix As Integer = CInt(TrouverChaineCompriseEntre(LignesGeoref(11), Pos, ":"c, ","c))
        Dim HautPix As Integer = CInt(TrouverChaineCompriseEntre(LignesGeoref(11), Pos, ":"c))
        Dim PoidsCarte As Integer = StrideImage(LargPix) * HautPix
        If PoidsCarte > TailleMaxTampon Then
            SupportCarte = TypeSupportCarte.Fichier
        Else
            If FlagSmallMemory Then
                SupportCarte = TypeSupportCarte.MemoryStatic
            Else
                SupportCarte = TypeSupportCarte.MemoryDynamic
            End If
        End If
        ToutFormat = True
        If SupportCarte = TypeSupportCarte.Fichier AndAlso FormatCarte Is ImageFormat.Bmp Then ToutFormat = False
        Dim NbTuilesX = CInt(Math.Ceiling(LargPix / NbPixelsTuile))
        FlagNbTuilesX = Site = "SuisseMobile" AndAlso NbTuilesX > NbTuilesMaxCarteSuisseInterpol(CartesSettings.SUPPORT_CARTE)
    End Sub
#End Region
#Region "Autres procédures ou fonctions"
    ''' <summary> initialisation à chaque fin de traitement de l'ensemble des cartes </summary>
    Private Sub InitialiserConvertir()
        Traitement.Text = "Selection Fichier(s) Georef et Traitement"
        Informations.SetToolTip(Traitement, "Sélectionnez un ou plusieurs fichiers Georef à convertir." & CrLf &
                                            "La validation de la sélection des fichiers lancera le traitement.")
        Quitter.Enabled = True
        GroupeConfigurations.Enabled = True
        DefinirEtatControles()
        Information.Text = ""
        FlagAnnuler = False
    End Sub
    ''' <summary> définit l'etat et éventuellement la valeur des controles du formulaire
    ''' en fonction des choix faits par l'utilisateur </summary>
    Private Sub DefinirEtatControles()
        If Not FlagInit Then
            'on interdit l'évenement ChoixFichiersTuile.SelectedIndexChanged
            FlagNbPahsChanged = True
            If NbPaths.Value > 0 AndAlso IsTrace.Checked Then
                'kmz uniquement
                FichiersTuiles.SelectedIndex = 3
                GeoRefs.SelectedIndex = 0
                References.SelectedIndex = 0
            Else
                'choix utilisateur
                FichiersTuiles.SelectedIndex = SauvegardeIndexFichierTuile
                GeoRefs.SelectedIndex = SauvegardeIndexGeoref
                References.SelectedIndex = SauvegardeIndexChoixReferences
            End If
            FlagNbPahsChanged = False
            'd'abord les controles liés à l'inclusion des trace
            OuvrirFichiersCarte.Multiselect = NbPaths.Value = 0
            Traces.Enabled = IsTrace.Checked AndAlso Traces.Items.Count > 1
            EtiquetteTraces.Enabled = IsTrace.Checked AndAlso Traces.Items.Count > 0
            LabelTraces.Enabled = IsTrace.Checked
            NbPaths.Enabled = IsTrace.Checked
            EtiquetteNbPaths.Enabled = NbPaths.Enabled
            LabelGeoRefs.Enabled = Not IsTrace.Checked OrElse NbPaths.Value = 0
            EtiquetteGeoRefs.Enabled = LabelGeoRefs.Enabled
            'si il s'agit juste de rajoutée la trace dans un fichier kmz on interdit tous les controles
            'sauf le choix de traces, la dimension des tuiles, le nombre de path et si on prend en compte les trace        
            GroupeCartes.Enabled = GeoRefs.SelectedIndex > 0
            GroupeFichiersTuiles.Enabled = FichiersTuiles.SelectedIndex > 0
            GroupeConfigurationTraces.Enabled = IsTrace.Checked AndAlso NbPaths.Value = 0
            LabelFichiersTuiles.Enabled = NbPaths.Value = 0 OrElse Not IsTrace.Checked
            EtiquetteFichiersTuiles.Enabled = LabelFichiersTuiles.Enabled

            Traitement.Enabled = False
            If Not IsTrace.Checked Then
                Traitement.Enabled = (FichiersTuiles.SelectedIndex > 0 OrElse GeoRefs.SelectedIndex > 0)
            Else
                Traitement.Enabled = Traces.Items.Count > 0 AndAlso (FichiersTuiles.SelectedIndex > 0 OrElse GeoRefs.SelectedIndex > 0)
            End If
        End If
    End Sub
    ''' <summary> tranforme un % transparence de 0 à 100 à une valeur du canal alpha de couleur de 255 à 0 </summary>
    ''' <param name="Trans"> % transparence </param>
    Private Shared Function PourcentToValeur(Trans As Integer) As Integer
        Return 255 - (255 * Trans) \ 100
    End Function
    ''' <summary> transforme une valeur du canal alpha de couleur de 255 à 0 en un % transparence de 0 à 100 </summary>
    ''' <param name="Valeur"> valeur du canal alpha </param>
    ''' <returns></returns>
    Private Shared Function ValeurToPourcent(Valeur As Byte) As Integer
        Return 100 - ((Valeur * 100) \ 255)
    End Function
    ''' <summary> procédure appelée par AffichageInformation du module Information lors d'appel inter-threads </summary>
    Friend Sub AfficherInfomationsErreurs()
        AfficherInformation()
    End Sub
#End Region
End Class