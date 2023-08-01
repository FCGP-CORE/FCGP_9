Imports FCGP.NiveauDetailCartographique
Imports FCGP.SystemeCartographique

''' <summary> permet la modification des niveaux d'affichage dans 1 fichier ORUX </summary>
Friend Class AffichageJNX
    'variables container des champs de données du formulaires
    Private ReadOnly NiveauJNXModifie(4), NiveauJNX(4), ValeursDefaut(4) As Double, ClefNiveauJNX(4) As String, NiveauORUX(4), IndicesEchelle(4) As Integer
    Private NivAffichage As Integer, NivAffichageModifie As Integer, FichierJNX As String, NbNiveauxDetail As Integer
    Private F As InfoJNX, IndiceSiteJNX As Integer, Titre As String
    ''' <summary> initialisation de la boite de dialogue d'ouverture des ficheirs JNX </summary>
    Private Sub AffichageJNX_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        TitreInformation = "Information modification fichier Jnx"
        ChercheFichierJNX.Filter = "JNX Files|*.jnx"
        ChercheFichierJNX.Title = "Choix du fichier JNX"
        'valeur par défault du répertoire lors de la 1ère recherche de fichier
        ChercheFichierJNX.InitialDirectory = PartagerSettings.CHEMIN_TUILE
    End Sub
    Private Sub AffichageJNX_FormClosed(sender As Object, e As FormClosedEventArgs)
        TitreInformation = Titre
    End Sub
    ''' <summary> Choix du fichier à modifié </summary>
    Private Sub ChoixFichierJNX_Click(sender As Object, e As EventArgs)
        If ChercheFichierJNX.ShowDialog(Me) = DialogResult.OK Then
            FichierJNX = ChercheFichierJNX.FileName
            F = VerifierCarteJNX(FichierJNX)
            IndiceSiteJNX = SiteDomTomToIndex(F.SiteCapture, F.DomTom)

            If F.NbNiveauxDetail = -1 Then
                MessageInformation = "Le fichier JNX est corrompu ou n'a pas été créé" & CrLf &
                                     "avec une version FCGP Capture N°5 ou supérieure ou FCGP Visue"
                AfficherInformation()
            Else
                'on garde le répertoire choisi pour la prochaine recherche
                ChercheFichierJNX.InitialDirectory = Path.GetDirectoryName(FichierJNX)
                'on efface le fichier choisi pour obliger une nouvelle sélection
                ChercheFichierJNX.FileName = Nothing
                InitialiserPanel()
            End If
        End If
    End Sub
    ''' <summary> applique les modifications demandées aux indices d'affichage du fichier JNX </summary>
    Private Sub Appliquer_Click(sender As Object, e As EventArgs)
        If Not VerifierValidation() Then Exit Sub
        Dim DebutTraitement As Date = Now
        Cursor = Cursors.WaitCursor
        Appliquer.Enabled = False
        ChoixFichierJNX.Enabled = False
        Quitter.Enabled = False
        For Cpt As Integer = 0 To NbNiveauxDetail - 1
            If NiveauJNXModifie(Cpt) <> NiveauJNX(Cpt) Then 'si il y a une modification on change dans le fichier et dans la base
                ChangerNiveauAffichageCarteJNX(FichierJNX, NbNiveauxDetail - Cpt - 1, NiveauJNXModifie(Cpt))
                FacteursSettings.Ecrire(IndiceSiteJNX, IndicesEchelle(Cpt), NiveauJNXModifie(Cpt), NiveauORUX(Cpt))
            End If
        Next
        If NivAffichageModifie <> NivAffichage Then
            ChangerOrdreAffichageCarteJNX(FichierJNX, NivAffichageModifie)
        End If
        Cursor = Cursors.Default
        MessageInformation = $"Le fichier {Path.GetFileNameWithoutExtension(FichierJNX)} a été modifié.{CrLf}Temps de traitement : {TimeSpanToStr(Now - DebutTraitement, True)}"
        AfficherInformation()

        RAZPanel()
    End Sub
    ''' <summary> remise à zéro des informations concernant les différents niveaux de détails </summary>
    Private Sub RAZPanel()
        'initialise les champs à zéro
        For Cpt As Integer = 0 To 4
            Dim Pp As Panel = CType(Support.Controls("Echelle" & Cpt), Panel)
            Pp.Controls("Clef" & Cpt).Text = ""
            Pp.Controls("ZoomJNX" & Cpt).Text = ""
            Dim SaisieValeurModif As NumericUpDown = CType(Pp.Controls("CorZoomJNX" & Cpt), NumericUpDown)
            SaisieValeurModif.Minimum = 0
            SaisieValeurModif.Value = 0
            SaisieValeurModif.ForeColor = SystemColors.ControlText
        Next
        Dim P As Panel = CType(Support.Controls("OrdreAffichage"), Panel)
        P.Controls("NiveauAffichage").Text = ""
        CType(P.Controls("CorNiveauAffichage"), NumericUpDown).Value = 0
        Appliquer.Enabled = False
        ChoixFichierJNX.Enabled = True
        Quitter.Enabled = True
    End Sub
    ''' <summary> initialise les information concernant les différents niveaux de détails d'un fichier JNX </summary>
    Private Sub InitialiserPanel()
        NbNiveauxDetail = F.NbNiveauxDetail
        Dim FacteursJNXDefauts = FacteursJNXSite(F.SiteCapture, F.DomTom)
        'remplissage des champs avec les valeurs liées au fichier
        For Cpt As Integer = NbNiveauxDetail - 1 To 0 Step -1
            Dim IndiceNiveau As Integer = NbNiveauxDetail - Cpt - 1
            Dim Pp As Panel = CType(Support.Controls("Echelle" & IndiceNiveau), Panel)
            Pp.Enabled = True
            ClefNiveauJNX(IndiceNiveau) = F.NiveauDetail(Cpt)
            IndicesEchelle(IndiceNiveau) = F.IndiceEchelleCapture(Cpt)
            Pp.Controls("Clef" & IndiceNiveau).Text = ClefNiveauJNX(IndiceNiveau)
            NiveauJNX(IndiceNiveau) = F.IndiceAffichage(Cpt)
            Dim AfficheValeurActuelle As Control = Pp.Controls("ZoomJNX" & IndiceNiveau)
            AfficheValeurActuelle.Text = DblToStr(NiveauJNX(IndiceNiveau), PrecisionJNX)
            ValeursDefaut(IndiceNiveau) = FacteursJNXDefauts(F.IndiceEchelleCapture(Cpt))
            If NiveauJNX(IndiceNiveau) = ValeursDefaut(IndiceNiveau) Then
                AfficheValeurActuelle.ForeColor = Color.YellowGreen
            Else
                AfficheValeurActuelle.ForeColor = Color.Tomato
            End If
            Dim SaisieValeurModif As NumericUpDown = CType(Pp.Controls("CorZoomJNX" & IndiceNiveau), NumericUpDown)
            SaisieValeurModif.Minimum = CDec(ValeursDefaut(IndiceNiveau) - 3)
            SaisieValeurModif.Maximum = CDec(ValeursDefaut(IndiceNiveau) + 3)
            Dim ValeurActuelle = FacteursSettings.Lire(IndiceSiteJNX, F.IndiceEchelleCapture(Cpt)).FacteurJNX
            NiveauORUX(IndiceNiveau) = FacteursSettings.Lire(IndiceSiteJNX, F.IndiceEchelleCapture(Cpt)).FacteurORUX
            If ValeurActuelle > ValeursDefaut(IndiceNiveau) + 3 Then ValeurActuelle = ValeursDefaut(IndiceNiveau) + 3
            If ValeurActuelle < ValeursDefaut(IndiceNiveau) - 3 Then ValeurActuelle = ValeursDefaut(IndiceNiveau) - 3
            SaisieValeurModif.Value = SaisieValeurModif.Minimum 'pour forcer l'évenement changed du control
            SaisieValeurModif.Value = CDec(ValeurActuelle)
        Next
        Dim P As Panel = CType(Support.Controls("OrdreAffichage"), Panel)
        P.Enabled = True
        NivAffichage = F.OrdreAffichage
        NivAffichageModifie = NivAffichage
        P.Controls("NiveauAffichage").Text = NivAffichage.ToString
        CType(P.Controls("CorNiveauAffichage"), NumericUpDown).Value = CDec(F.OrdreAffichage)
        Appliquer.Enabled = Is_Appliquer()
    End Sub
    Private Sub CorZoomJNX_ValueChanged(sender As Object, e As EventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown), IndiceNiveau As Integer = CInt(C.Tag)

        NiveauJNXModifie(IndiceNiveau) = C.Value

        If NiveauJNXModifie(IndiceNiveau) = ValeursDefaut(IndiceNiveau) Then
            C.ForeColor = Color.YellowGreen
        Else
            C.ForeColor = Color.Tomato
        End If
        Appliquer.Enabled = Is_Appliquer()
    End Sub
    ''' <summary> indique si il y a au moins une différence entre les valeurs actuelles et les valeurs modifiées</summary>
    Private Function Is_Appliquer() As Boolean
        For Cpt As Integer = 0 To 4
            If Support.Controls("Echelle" & Cpt).Enabled Then
                If NiveauJNXModifie(Cpt) <> NiveauJNX(Cpt) Then Return True
            End If
        Next
        If NivAffichageModifie <> NivAffichage Then Return True
        Return False
    End Function
    ''' <summary> prend en compte la modification d'un niveau d'affichage </summary>
    Private Sub CorNiveauAffichage_ValueChanged(sender As Object, e As EventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown)
        NivAffichageModifie = CInt(C.Value)
        Appliquer.Enabled = Is_Appliquer()
    End Sub
    ''' <summary> Verifie que les niveaux d'affichages des différents niveau de détail soient cohérents entre eux </summary>
    Private Function VerifierValidation() As Boolean
        Dim ValeurInfJNX As Double = 0
        For Cpt As Integer = 0 To NbNiveauxDetail - 1
            If NiveauJNXModifie(Cpt) > ValeurInfJNX Then
                ValeurInfJNX = NiveauJNXModifie(Cpt)
            Else
                MessageInformation = $"Le zoom JNX du niveau de détail {ClefNiveauJNX(Cpt)} n'est pas supérieur {CrLf} au zoom JNX du niveau de détail précédent"
                AfficherInformation()
                Return False
            End If
        Next
        Return True
    End Function
End Class