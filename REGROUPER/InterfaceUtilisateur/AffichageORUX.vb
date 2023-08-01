Imports FCGP.NiveauDetailCartographique

''' <summary> permet la modification des niveaux d'affichage dans 1 fichier ORUX </summary>
Friend Class AffichageORUX
    Private ReadOnly NiveauORUXModifie(4), NiveauORUX(4), ValeursDefaut(4), IndicesEchelle(4) As Integer, ClefNiveauORUX(4) As String, NiveauJNX(4) As Double
    Dim FichierORUX As String, NbNiveauxDetail As Integer, F As InfoORUX, IndiceSiteORUX As Integer, Titre As String

    Private Sub AffichageORUX_Load(sender As Object, e As EventArgs)
        Titre = TitreInformation
        TitreInformation = "Information modification fichier Orux"
        ChercheFichierORUX.Filter = "ORUX Files|*.otrk2.xml"
        ChercheFichierORUX.Title = "Choix du fichier ORUX"
        ChercheFichierORUX.InitialDirectory = PartagerSettings.CHEMIN_TUILE
    End Sub
    Private Sub AffichageORUX_FormClosed(sender As Object, e As FormClosedEventArgs)
        TitreInformation = Titre
    End Sub
    Private Sub ChoixFichierORUX_Click(sender As Object, e As EventArgs)
        If ChercheFichierORUX.ShowDialog(Me) = DialogResult.OK Then
            FichierORUX = ChercheFichierORUX.FileName
            F = VerifierCarteORUX(FichierORUX)
            If F.NbNiveauxDetail = -1 Then
                MessageInformation = "Le fichier ORUX est corrompu ou n'a pas été créé" & CrLf &
                                     "avec une version FCGP Capture N°5 ou supérieure ou FCGP Visue"
                AfficherInformation()
            Else
                ChercheFichierORUX.InitialDirectory = Path.GetDirectoryName(FichierORUX)
                ChercheFichierORUX.FileName = Nothing
                IndiceSiteORUX = SystemeCartographique.SiteDomTomToIndex(F.SiteCapture, F.DomTom)
                InitialiserPanel()
            End If
        End If
    End Sub
    Private Sub Appliquer_Click(sender As Object, e As EventArgs)
        If Not VerifierValidation() Then Exit Sub
        Dim DebutTraitement As Date = Now
        Appliquer.Enabled = False
        ChoixFichierORUX.Enabled = False
        Quitter.Enabled = False
        Cursor = Cursors.WaitCursor
        If SensMaxMin() Then
            For Cpt As Integer = NbNiveauxDetail - 1 To 0 Step -1
                If NiveauORUXModifie(Cpt) <> NiveauORUX(Cpt) Then
                    ChangerNiveauAffichageCarteOrux(FichierORUX, NbNiveauxDetail - Cpt - 1, NiveauORUXModifie(Cpt))
                    FacteursSettings.Ecrire(IndiceSiteORUX, IndicesEchelle(Cpt), NiveauJNX(Cpt), NiveauORUXModifie(Cpt))
                End If
            Next
        Else
            For Cpt As Integer = 0 To NbNiveauxDetail - 1
                If NiveauORUXModifie(Cpt) <> NiveauORUX(Cpt) Then
                    ChangerNiveauAffichageCarteOrux(FichierORUX, NbNiveauxDetail - Cpt - 1, NiveauORUXModifie(Cpt))
                    FacteursSettings.Ecrire(IndiceSiteORUX, IndicesEchelle(Cpt), NiveauJNX(Cpt), NiveauORUXModifie(Cpt))
                End If
            Next
        End If
        TitreInformation = "Information Regrouper"
        MessageInformation = $"Le fichier {Path.GetFileNameWithoutExtension(FichierORUX)} a été modifié.{CrLf}Temps de traitement : {TimeSpanToStr(Now - DebutTraitement, True)}"
        AfficherInformation()
        Cursor = Cursors.Default
        RAZPanel()
    End Sub
    Private Function SensMaxMin() As Boolean
        For Cpt As Integer = NbNiveauxDetail - 1 To 0 Step -1
            If NiveauORUX.Contains(NiveauORUXModifie(Cpt)) Then
                For CptInf As Integer = Cpt - 1 To 0 Step -1
                    If NiveauORUX(CptInf) = NiveauORUXModifie(Cpt) Then Return False
                Next
            End If
        Next
        Return True
    End Function
    Private Sub RAZPanel()
        'initialise les champs à zéro
        For Cpt As Integer = 0 To 4
            Dim Pp As Panel = CType(Support.Controls("Echelle" & Cpt), Panel)
            Pp.Controls("Clef" & Cpt).Text = ""
            Pp.Controls("ZoomORUX" & Cpt).Text = ""
            Dim SaisieValeurModif As NumericUpDown = CType(Pp.Controls("CorZoomORUX" & Cpt), NumericUpDown)
            SaisieValeurModif.Minimum = 0
            SaisieValeurModif.Value = 0
            SaisieValeurModif.ForeColor = SystemColors.ControlText
        Next
        Appliquer.Enabled = False
        ChoixFichierORUX.Enabled = True
        Quitter.Enabled = True
    End Sub
    Private Sub InitialiserPanel()
        NbNiveauxDetail = F.NbNiveauxDetail
        Dim FacteursORUX = FacteursORUXSite(F.SiteCapture, F.DomTom)
        'remplissage des champs avec les valers liées au fichier
        For Cpt As Integer = NbNiveauxDetail - 1 To 0 Step -1
            Dim IndiceNiveau As Integer = NbNiveauxDetail - Cpt - 1
            Dim Pp As Panel = CType(Support.Controls("Echelle" & IndiceNiveau), Panel)
            Pp.Enabled = True
            ClefNiveauORUX(IndiceNiveau) = F.NiveauDetail(Cpt)
            Pp.Controls("Clef" & IndiceNiveau).Text = ClefNiveauORUX(IndiceNiveau)

            ValeursDefaut(IndiceNiveau) = FacteursORUX(F.IndiceEchelleCapture(Cpt))
            IndicesEchelle(IndiceNiveau) = F.IndiceEchelleCapture(Cpt)
            NiveauORUX(IndiceNiveau) = F.IndiceAffichage(Cpt)
            Dim AfficheValeurActuelle As Control = Pp.Controls("ZoomORUX" & IndiceNiveau)
            AfficheValeurActuelle.Text = NiveauORUX(IndiceNiveau).ToString("0")
            If NiveauORUX(IndiceNiveau) = ValeursDefaut(IndiceNiveau) Then
                AfficheValeurActuelle.ForeColor = Color.YellowGreen
            Else
                AfficheValeurActuelle.ForeColor = Color.Tomato
            End If
            Dim SaisieValeurModif As NumericUpDown = CType(Pp.Controls("CorZoomORUX" & IndiceNiveau), NumericUpDown)
            SaisieValeurModif.Minimum = ValeursDefaut(IndiceNiveau) - 3
            SaisieValeurModif.Maximum = ValeursDefaut(IndiceNiveau) + 3
            NiveauJNX(IndiceNiveau) = FacteursSettings.Lire(IndiceSiteORUX, F.IndiceEchelleCapture(Cpt)).FacteurJNX
            Dim ValeurActuelle = FacteursSettings.Lire(IndiceSiteORUX, F.IndiceEchelleCapture(Cpt)).FacteurORUX
            If ValeurActuelle > ValeursDefaut(IndiceNiveau) + 3 Then ValeurActuelle = ValeursDefaut(IndiceNiveau) + 3
            If ValeurActuelle < ValeursDefaut(IndiceNiveau) - 3 Then ValeurActuelle = ValeursDefaut(IndiceNiveau) - 3
            SaisieValeurModif.Value = SaisieValeurModif.Minimum 'pour forcer l'évenement changed du control
            SaisieValeurModif.Value = ValeurActuelle
        Next
        Appliquer.Enabled = Is_Appliquer()
    End Sub
    Private Sub CorZoomORUX_ValueChanged(sender As Object, e As EventArgs)
        Dim C As NumericUpDown = CType(sender, NumericUpDown), IndiceNiveau As Integer = CInt(C.Tag)
        NiveauORUXModifie(IndiceNiveau) = CInt(C.Value)
        If NiveauORUXModifie(IndiceNiveau) = ValeursDefaut(IndiceNiveau) Then
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
                If NiveauORUXModifie(Cpt) <> NiveauORUX(Cpt) Then Return True
            End If
        Next
        Return False
    End Function

    Private Function VerifierValidation() As Boolean
        Dim ValeurSupORUX As Double = 50
        For Cpt As Integer = 0 To NbNiveauxDetail - 1
            If NiveauORUXModifie(Cpt) < ValeurSupORUX Then
                ValeurSupORUX = NiveauORUXModifie(Cpt)
            Else
                MessageInformation = "Le zoom corrigé du niveau de détail " & ClefNiveauORUX(Cpt) & " n'est pas inférieur" & CrLf & "à celui du niveau de détail précédent"
                TitreInformation = "Information Fichier ORUX"
                AfficherInformation()
                Return False
            End If
        Next
        Return True
    End Function
End Class