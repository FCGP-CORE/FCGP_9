Imports FCGP.NiveauDetailCartographique
Imports FCGP.SystemeCartographique
''' <summary> permet la gestion d'une carte au format ORUX. On suppose que la base de données est toujours en concordance avec le fichier XML </summary>
Friend Module FichierCarteOrux
#Region "Public"
    ''' <summary> Lit les tuiles d'un niveau du fichier ORUX et les écrit dans un répertoire </summary>
    ''' <param name="FichierCarteOrux"> chemin du fichier otrk2.xml de description de la carte </param>
    ''' <param name="Echelle"> Echelle dont on veut récupérer les tuiles </param>
    ''' <param name="CheminTuiles"> répertoire où l'on veut enregister les tuiles </param>
    ''' <remarks> cette fonction est donnée à titre d'exemple. Elle ne sert pas dans FCGP </remarks> 
    Friend Function LireNiveauCarteOrux(FichierCarteOrux As String, Echelle As Echelles, CheminTuiles As String) As Boolean
        'vérifier que le fichier est correct
        Dim InfosOrux As InfoORUX = VerifierCarteORUX(FichierCarteOrux)
        If InfosOrux.NbNiveauxDetail = -1 Then Return False
        Try
            CheminORUX = Path.GetDirectoryName(FichierCarteOrux) & "\"
            Using BDSqlite As New BDTuilesOruxMaps(CheminORUX, False) 'on ouvre la base sqlite associée à la carte
                If BDSqlite.Isok Then
                    For Cpt = 0 To InfosOrux.NbNiveauxDetail - 1
                        Dim EchelleCapture = SiteIndiceEchelleToEchelle(InfosOrux.SiteCapture, InfosOrux.IndiceEchelleCapture(Cpt))
                        If EchelleCapture = Echelle Then
                            Return BDSqlite.LireNiveauFichier(InfosOrux.IndiceAffichage(Cpt), CheminTuiles)
                        End If
                    Next
                End If
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "224E")
        End Try
        Return False
    End Function
    ''' <summary>  change le niveau d'affichage d'un niveau</summary>
    ''' <param name="CheminCarteOrux"> chemin du fichier otrk2.xml de description de la carte</param>
    ''' <param name="IndexNiveau">index du niveau de détail concerné par la modification</param>
    ''' <param name="IndiceAffichageModifie">valeur à ajouter à l'indice d'affichage. Si positif le niveau apparait à une échelle plus petite</param>
    Friend Function ChangerNiveauAffichageCarteOrux(CheminCarteOrux As String, IndexNiveau As Integer, IndiceAffichageModifie As Integer) As Boolean
        ChangerNiveauAffichageCarteOrux = False
        'vérifier que le fichier est correct
        Dim InfosOrux As InfoORUX = VerifierCarteORUX(CheminCarteOrux)
        If InfosOrux.NbNiveauxDetail = -1 Then Return False
        'trouver le repertoire lié à la carte
        CheminORUX = Path.GetDirectoryName(CheminCarteOrux) & "\"
        'le fichier comporte 2 extensions d'où l'utilisation de la méthode GetFileNameWithoutExtension 2 fois
        NomORUX = InfosOrux.NomCarte
        Try
            Using BDSqlite As New BDTuilesOruxMaps(CheminORUX, False) 'on ouvre la base sqlite associée à la carte
                If BDSqlite.Isok Then
                    Dim LignesORUX() As String = File.ReadAllLines(CheminCarteOrux, Encoding_FCGP)
                    Dim EnteteORUX() As String = RenvoyerEnteteORUX_XML(LignesORUX)
                    Dim NiveauORUX(InfosOrux.NbNiveauxDetail - 1)() As String
                    For Cpt As Integer = 0 To InfosOrux.NbNiveauxDetail - 1
                        NiveauORUX(Cpt) = RenvoyerNiveauORUX_XML(LignesORUX, Cpt)
                        If Cpt = IndexNiveau Then
                            ChangerNiveauAffichageORUX_XML(NiveauORUX(Cpt), InfosOrux.IndiceAffichage(Cpt), IndiceAffichageModifie) 'change dans le texte xml
                            BDSqlite.ChangerNiveauAffichage(InfosOrux.IndiceAffichage(Cpt), IndiceAffichageModifie) 'change dans la base de données
                        End If
                    Next
                    'créer le fichier XML
                    Dim S As New StringBuilder(1500)
                    'écriture de l'entête
                    For CptLigne As Integer = 0 To NbLignesEnteteOrux - 1
                        S.AppendLine(EnteteORUX(CptLigne))
                    Next
                    'écriture du ou des niveaux
                    For CptNiveau As Integer = 0 To InfosOrux.NbNiveauxDetail - 1
                        For CptLigne As Integer = 0 To NbLignesNiveauOrux - 1
                            S.AppendLine(NiveauORUX(CptNiveau)(CptLigne))
                        Next
                    Next
                    'ajouter la fin de la carte
                    EcrireBaliseFinEnteteORUX_XML(S)
                    'ecrire le fichier XML au bon endroit
                    File.WriteAllText(CheminCarteOrux, S.ToString(), Encoding_FCGP)
                    ChangerNiveauAffichageCarteOrux = True
                End If
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "F9Z1")
        End Try
    End Function
    ''' <summary>  verifie l'integrité du fichier otrk2.xml telque FCGP Version 5 doit les écrire</summary>
    ''' <param name="CheminCarteOrux">chemin complet du fichier OTRK</param>
    ''' <remarks> on ne verifie que le fichier TRK2 et pas la base de données</remarks>
    Friend Function VerifierCarteORUX(CheminCarteOrux As String) As InfoORUX
        VerifierCarteORUX = New InfoORUX With {.NbNiveauxDetail = -1, .SiteCapture = SitesCartographiques.Aucun}
        Try
            Dim LignesORUX() As String = File.ReadAllLines(CheminCarteOrux, Encoding_FCGP)
            Dim NbNiveaux = TrouverNbNiveauORUX_XML(LignesORUX)
            If NbNiveaux = 0 Then Exit Try
            Dim InOr = New InfoORUX With {.NbNiveauxDetail = NbNiveaux,
                                          .IndiceEchelleCapture = New Integer(NbNiveaux - 1) {},
                                          .IndiceAffichage = New Integer(NbNiveaux - 1) {},
                                          .NiveauDetail = New String(NbNiveaux - 1) {}}

            Dim S As String() = RenvoyerEnteteORUX_XML(LignesORUX)
            If S Is Nothing Then Exit Try
            Dim ClefSite As String = Nothing
            DecomposerEnteteFichierORUX_XML(S(3), InOr.SiteCapture, InOr.DomTom, InOr.NomCarte, ClefSite)

            If InOr.SiteCapture = SitesCartographiques.Aucun Then Exit Try
            For Cpt As Integer = 0 To NbNiveaux - 1
                S = RenvoyerNiveauORUX_XML(LignesORUX, Cpt)
                If S Is Nothing Then Exit Try
                Dim ClefEchelle As String = Nothing
                DecomposerNiveauFichierOrux(S(2), InOr.SiteCapture, ClefEchelle, InOr.IndiceEchelleCapture(Cpt))
                If InOr.IndiceEchelleCapture(Cpt) = -1 Then Exit Try
                InOr.IndiceAffichage(Cpt) = TrouverNiveauAffichageORUX_XML(S)
                InOr.NiveauDetail(Cpt) = ClefSite & "-" & ClefEchelle
            Next
            VerifierCarteORUX = InOr
        Catch Ex As Exception
            AfficherErreur(Ex, "Z5H0")
        End Try
    End Function
    ''' <summary> strucutre de description du fichier </summary>
    Friend Structure InfoORUX
        Friend SiteCapture As SitesCartographiques
        Friend DomTom As DomsToms
        Friend NbNiveauxDetail As Integer
        Friend IndiceEchelleCapture As Integer()
        ''' <summary> valeur par pas de 1 indiquant à quelle échelle d'affichage le niveau de détail apparait </summary>
        Friend IndiceAffichage As Integer()
        Friend NiveauDetail As String()
        Friend NomCarte As String
    End Structure
    ''' <summary>créer une carte oruxmaps avec un niveau de détail à partir d'une carte</summary>
    ''' <param name="CarteActuelle">carte servant de support aux tuiles</param>
    ''' <remarks>n'est accessible que pour la fonction carte.realiserdemandes</remarks>
    Friend Function CreerCarteOrux(CarteActuelle As Carte) As Boolean
        CreerCarteOrux = False
        Try
            With CarteActuelle
                .NbIterations = 0
                .NbTotalIterations = .Tuiles.Length
                AfficherVisueInformation($"{ .MessInfo & .NbIterations} / { .NbTotalIterations}")
                'créer le repertoire lié à la carte
                CheminORUX = .CheminFichiersTuile & "\" & .Nom & .NomAjout & "\"
                NomORUX = .Nom  'nom de la carte
                Directory.CreateDirectory(CheminORUX)
                'Créer la base SQLITE oruximages
                Using BDSqlite As New BDTuilesOruxMaps(CheminORUX, True)
                    If BDSqlite.Isok Then
                        'ecrire les tuiles du premier niveau dans la base orux
                        BDSqlite.EcrireNiveauFichier(CheminEnregistrementProvisoire & Carte.ExtensionCheminImagesTuiles & "\", CarteActuelle)
                        'créer le fichier XML
                        Dim S As New StringBuilder(1500)
                        'ajouter l'entête de la carte
                        EcrireEnteteORUX_XML(.SystemCartographique, S)
                        'ajouter le fichier de description du premier niveau
                        EcrireNiveauORUX_XML(CarteActuelle, S)
                        'ajouter la fin de la carte
                        EcrireBaliseFinEnteteORUX_XML(S)
                        'ecrire le fichier XML au bon endroit et avec le bon nom
                        NomORUX &= .NomAjout
                        File.WriteAllText(CheminORUX & NomORUX & ExtensionORUX, S.ToString(), Encoding_FCGP)
                        CreerCarteOrux = True
                    End If
                End Using
            End With
        Catch Ex As Exception
            AfficherErreur(Ex, "T1P0")
        End Try
    End Function
    ''' <summary> ajoute un niveau de détail à la carte oruxmap à partir d'une carte avec une échelle de capture differente</summary>
    ''' <param name="CarteActuelle">carte qui contient le niveau à ajouter</param>
    ''' <returns>true si tout c'est bien passé</returns>
    ''' <remarks>aucune vérif n'est faite quant aux dimensions du nouveau niveau par rapport à celles de la carte orux. n'est accessible que pour la fonction carte.realiserdemandes</remarks>
    Friend Function AjouterNiveauCarteOrux(CarteActuelle As Carte) As Boolean
        With CarteActuelle
            .NbIterations = 0
            .NbTotalIterations = .Tuiles.Length
            AfficherVisueInformation($"{ .MessInfo & .NbIterations} / { .NbTotalIterations}")
        End With
        Dim CheminCarteOrux As String = CarteActuelle.CheminFichiersTuile & "\" & CarteActuelle.Nom & CarteActuelle.NomAjout & "\" & CarteActuelle.Nom & CarteActuelle.NomAjout & ".otrk2.xml"
        Dim InfORUX As InfoORUX = VerifierCarteORUX(CheminCarteOrux) 'chemin complet du fichier otrk2.xml
        If InfORUX.NbNiveauxDetail = -1 Then Return False
        If InfORUX.SiteCapture <> CarteActuelle.SystemCartographique.SiteCarto Then Return False
        For Cpt As Integer = 0 To InfORUX.NbNiveauxDetail - 1
            Dim IndiceEchelleSite As Integer = EchelleToSiteIndiceEchelle(CarteActuelle.SystemCartographique.SiteCarto,
                                                                                                     CarteActuelle.SystemCartographique.Niveau.Echelle)
            If InfORUX.IndiceEchelleCapture(Cpt) = IndiceEchelleSite Then Return False 'on ne rajoute pas le niveau de détail si il existe déjà
        Next
        AjouterNiveauCarteOrux = False
        CheminORUX = Path.GetDirectoryName(CheminCarteOrux) & "\"  'trouver le repertoire lié à la carte
        Try
            Using BDSqlite As New BDTuilesOruxMaps(CheminORUX, False)
                If BDSqlite.Isok Then 'si l'ouverture est ok
                    Dim LignesORUX() As String = File.ReadAllLines(CheminCarteOrux, Encoding_FCGP)
                    'créer le nouveau fichier XML à partir de l'ancien
                    Dim S As New StringBuilder(1500)
                    'écriture du fichier sans la fin de la carte
                    For CptLigne As Integer = 0 To LignesORUX.Length - NbLignesFinOrux - 1
                        S.AppendLine(LignesORUX(CptLigne))
                    Next
                    'créer le répertoire qui va recevoir les tuiles du nouveau niveau de détail
                    Dim CheminTuiles As String = CheminEnregistrementProvisoire & Carte.ExtensionCheminImagesTuiles & "\"
                    'ecrire les tuiles du niveau dans la base orux
                    BDSqlite.EcrireNiveauFichier(CheminTuiles, CarteActuelle)
                    'ajouter le fichier de description du premier niveau soit à partir de la carte passée en paramètre soit de son interpolation si elle a été demandée
                    NomORUX = InfORUX.NomCarte
                    EcrireNiveauORUX_XML(CarteActuelle, S)
                    'ajouter la fin de la carte
                    EcrireBaliseFinEnteteORUX_XML(S)
                    'ecrire le fichier XML au bon endroit
                    File.WriteAllText(CheminCarteOrux, S.ToString(), Encoding_FCGP)
                    AjouterNiveauCarteOrux = True
                End If
            End Using
        Catch Ex As Exception
            AfficherErreur(Ex, "A6C7")
        End Try
    End Function
#End Region
#Region "Private"
    Private NomORUX As String
    Private CheminORUX As String
    Friend Const ExtensionORUX As String = ".otrk2.xml"
    Private Const NbLignesNiveauOrux = 18
    Private Const NbLignesEnteteOrux = 4
    Private Const NbLignesFinOrux = 2
    ''' <summary>  trouve le nb de niveau de la carte en fonction du nb de lignes du fichier otrk2.xml</summary>
    Private Function TrouverNbNiveauORUX_XML(LignesOrux As String()) As Integer
        If (LignesOrux.Length - NbLignesEnteteOrux - NbLignesFinOrux) Mod NbLignesNiveauOrux <> 0 Then
            TrouverNbNiveauORUX_XML = 0
        Else
            TrouverNbNiveauORUX_XML = (LignesOrux.Length - NbLignesEnteteOrux - NbLignesFinOrux) \ NbLignesNiveauOrux
        End If
    End Function
    ''' <summary> trouve l'échelle et la clé correspondante d'un niveau d'une carte ORUX </summary>
    ''' <param name="Niveau"> Texte correspondant du niveau </param>
    ''' <param name="SiteCapture"> site correspondant à la carte </param>
    ''' <param name="ClefEchelle"> Cle de l'échelle </param>
    ''' <param name="Echelle"> indice de l'échelle </param>
    Private Sub DecomposerNiveauFichierOrux(Niveau As String, SiteCapture As SitesCartographiques, ByRef ClefEchelle As String, ByRef Echelle As Integer)
        Dim Pos As Integer = PosSeparateur(Niveau)
        ClefEchelle = Niveau.Substring(0, Pos)
        Echelle = EchelleClefToSiteIndiceEchelle(SiteCapture, ClefEchelle)
    End Sub
    ''' <summary> renvoie la chaine comprise entre les balises </summary>
    ''' <param name="Texte">chaine à nettoyer </param>
    ''' <returns> la chaine netoyée et la position du séparateur '-' </returns>
    Private Function PosSeparateur(ByRef Texte As String) As Integer
        Dim Pos As Integer = Texte.IndexOf("<![CDATA[")
        Dim Pos1 As Integer = Texte.IndexOf("]]></MapName>")
        Texte = Texte.Substring(Pos + 9, Pos1 - Pos - 9)
        Return Texte.IndexOf("-"c)
    End Function
    ''' <summary>  renvoie le niveau d'affichage d'un niveau</summary>
    Private Function TrouverNiveauAffichageORUX_XML(NiveauOruxXML As String()) As Integer
        Dim Pos As Integer = NiveauOruxXML(1).IndexOf("layerLevel=""")
        Dim Pos1 As Integer = NiveauOruxXML(1).IndexOf(""""c, Pos + 13)
        Return CInt(NiveauOruxXML(1).Substring(Pos + 12, Pos1 - Pos - 12))
    End Function
    ''' <summary> renvoie le site de capture ayant servi pour la création de la carte ORUX </summary>
    Private Sub DecomposerEnteteFichierORUX_XML(Entete As String, ByRef SiteCarto As SitesCartographiques,
                                                ByRef DomTom As DomsToms, ByRef NomCarte As String, ByRef ClefSite As String)
        Dim Pos As Integer = PosSeparateur(Entete)
        NomCarte = Entete.Substring(Pos + 1)
        ClefSite = Entete.Substring(0, Pos)
        SiteCarto = TrouverSiteCartographiqueCle(ClefSite)
        DomTom = DomsToms.aucun
        If SiteCarto = SitesCartographiques.Aucun Then
            DomTom = TrouverDomTomCle(ClefSite)
            If DomTom > DomsToms.aucun Then
                SiteCarto = SitesCartographiques.DomTom
            Else
                Throw New Exception("Fichier Orux corrompu")
            End If
        End If
    End Sub
    ''' <summary> renvoie les lignes de texte d'un niveau d'un fichier OTRK</summary>
    Private Function RenvoyerNiveauORUX_XML(LignesOrux As String(), Niveau As Integer) As String()
        Dim S(NbLignesNiveauOrux - 1) As String
        Dim Depart As Integer = (Niveau) * NbLignesNiveauOrux + NbLignesEnteteOrux
        For Cpt As Integer = 0 To NbLignesNiveauOrux - 1
            S(Cpt) = LignesOrux(Cpt + Depart)
        Next
        If Not S(0).StartsWith("<OruxTracker") Then Return Nothing
        If Not S(NbLignesNiveauOrux - 1).EndsWith("</OruxTracker>") Then Return Nothing
        Return S
    End Function
    ''' <summary> renvoie les lignes de texte de  l'entête d'un fichier OTRK</summary>
    Private Function RenvoyerEnteteORUX_XML(LignesOrux As String()) As String()
        Dim S(NbLignesEnteteOrux - 1) As String
        For Cpt As Integer = 0 To NbLignesEnteteOrux - 1
            S(Cpt) = LignesOrux(Cpt)
        Next
        If Not S(NbLignesEnteteOrux - 1).EndsWith("</MapName>") Then Return Nothing
        Return S
    End Function
    ''' <summary> change le niveau d'affichage d'un niveau  d'un fichier OTRK</summary>
    ''' <param name="NiveauOruxXML">ligne de texte du niveau à modifier</param>
    ''' <param name="Ancien">valeur actuelle du niveau. 3 endroits à trouver par niveau</param>
    ''' <param name="Nouveau">valeur future</param>
    Private Sub ChangerNiveauAffichageORUX_XML(NiveauOruxXML As String(), Ancien As Integer, Nouveau As Integer)
        NiveauOruxXML(1) = NiveauOruxXML(1).Replace(Ancien.ToString, Nouveau.ToString)
        NiveauOruxXML(2) = NiveauOruxXML(2).Replace(NomORUX & " " & Ancien.ToString, NomORUX & " " & Nouveau.ToString)
        NiveauOruxXML(7) = NiveauOruxXML(7).Replace(NomORUX & " " & Ancien.ToString, NomORUX & " " & Nouveau.ToString)
    End Sub
    ''' <summary> ecrit les lignes de texte de la fin  d'un fichier OTRK</summary>
    Private Sub EcrireBaliseFinEnteteORUX_XML(S As StringBuilder)
        'fin de la carte
        S.AppendLine("</MapCalibration>")
        S.AppendLine("</OruxTracker>")
    End Sub
    ''' <summary> écrit les lignes de texte de l'entête d'un fichier OTRK</summary>
    Private Sub EcrireEnteteORUX_XML(SysCarto As SystemeCartographique, S As StringBuilder)
        'entête de la carte
        S.AppendLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
        S.AppendLine("<OruxTracker xmlns:orux=""http://oruxtracker.com/app/res/calibration"" versionCode=""3.0"">")
        S.AppendLine("<MapCalibration layers=""true"" layerLevel=""0"">")
        Dim Clef As String = If(SysCarto.SiteCarto = SitesCartographiques.DomTom, DomTomClef(SysCarto.DomTom),
                                                                                  SiteCartoClef(SysCarto.SiteCarto))
        S.AppendLine("<MapName><![CDATA[" & Clef & "-" & NomORUX & "]]></MapName>")
    End Sub
    ''' <summary> écrit les lignes de texte d'un niveau d'un fichier OTRK</summary>
    Private Sub EcrireNiveauORUX_XML(CarteActuelle As Carte, S As StringBuilder)
        Const Format As String = "0.000000000000"
        With CarteActuelle
            Dim Projection, Datum As String
            If .SystemCartographique.IsInterpol Then
                'initalisation Datum et projection ORUX pour les cartes interpolées
                Projection = "Latitude/Longitude,"
                Datum = "WGS 1984:Global Definition"
            Else
                'si la carte concerne un site de capture dont la projection n'est pas Latitude/Longitude on change le datum et la projection attendue par ORUX
                If .SystemCartographique.SiteCarto = SitesCartographiques.SuisseMobile Then
                    Projection = "(SUI) Swiss Grid,"
                    Datum = "CH-1903:Swiss"
                Else
                    Projection = "Mercator,0.0"
                    Datum = "WGS 1984:Global Definition"
                End If
            End If
            'entête niveau
            S.AppendLine("<OruxTracker versionCode= ""2.1"">")
            S.AppendLine("<MapCalibration layers=""False"" layerLevel=""" & CarteActuelle.FacteurORUX & """>")
            'definition globale du niveau
            S.AppendLine("<MapName><![CDATA[" & .SystemCartographique.Niveau.Clef & "-" & NomORUX & " " & CarteActuelle.FacteurORUX & "]]></MapName>")
            S.AppendLine("<MapChunks xMax=""" & .NbTuilesX & """ yMax=""" & .NbTuilesY & """")
            S.AppendLine(Tab & "datum=""" & Datum & "@WGS 1984: Global Definition""")
            S.AppendLine(Tab & "projection=""" & Projection & """")
            S.AppendLine(Tab & "img_height=""" & .DimensionsTuile & """ img_width=""" & .DimensionsTuile & """")
            S.AppendLine(Tab & "file_name=""" & NomORUX & " " & CarteActuelle.FacteurORUX & """ />")
            S.AppendLine("<MapDimensions height=""" & .HauteurPixel & """ width=""" & .LargeurPixel & """ />")
            S.AppendLine("<MapBounds minLat=""" & DblToStr(.Coins(2).Latitude, Format) & """ maxLat=""" & DblToStr(.Coins(0).Latitude, Format) &
                         """ minLon=""" & DblToStr(.Coins(0).Longitude, Format) & """ maxLon=""" & DblToStr(.Coins(2).Longitude, Format) & """ />")
            'definition des points de référencement du niveau
            S.AppendLine("<CalibrationPoints>")
            S.AppendLine("<CalibrationPoint corner=""TL"" lon=""" & DblToStr(.Coins(0).Longitude, Format) & """ lat=""" & DblToStr(.Coins(0).Latitude, Format) & """ />")
            S.AppendLine("<CalibrationPoint corner=""BR"" lon=""" & DblToStr(.Coins(2).Longitude, Format) & """ lat=""" & DblToStr(.Coins(2).Latitude, Format) & """ />")
            S.AppendLine("<CalibrationPoint corner=""TR"" lon=""" & DblToStr(.Coins(1).Longitude, Format) & """ lat=""" & DblToStr(.Coins(1).Latitude, Format) & """ />")
            S.AppendLine("<CalibrationPoint corner=""BL"" lon=""" & DblToStr(.Coins(3).Longitude, Format) & """ lat=""" & DblToStr(.Coins(3).Latitude, Format) & """ />")
            S.AppendLine("</CalibrationPoints>")
            'fin niveau
            S.AppendLine("</MapCalibration>")
            S.AppendLine("</OruxTracker>")
        End With
    End Sub
    ''' <summary> permet la gestion de la base de données sqlite associée à une carte au format ORUX </summary>
    Private Class BDTuilesOruxMaps
        Implements IDisposable
        ''' <summary> connexion avec la base SQLite</summary>
        Friend SQLconnect As SQLiteConnection
        ''' <summary> true si l'initialisation c'est bien passée</summary>
        Friend ReadOnly Property Isok As Boolean
        ''' <summary> command a envoyer à la base de données SQLite</summary>
        Private SQLcommand As SQLiteCommand
        ''' <summary> nom de la base de données SQLite. C'est toujours le même</summary>
        Private Const NomBase As String = "OruxMapsImages.db"
        ''' <summary> Crée ou ouvre une base SQLite pour les fichiers tuiles issus des cartes FCGP au format attendu par  ORUXMAPS</summary>
        ''' <param name="CheminBase">chemin complet de la base de donnée Sqlite yc le slash de fin</param>
        ''' <param name="FlagCreation">Creer la structure de la base si true sinon juste creer la connexion</param>
        Friend Sub New(CheminBase As String, FlagCreation As Boolean)
            Try
                Dim CheminComplet As String = CheminBase & NomBase
                SQLconnect = New SQLiteConnection("Data Source=" & CheminComplet & ";")
                SQLcommand = SQLconnect.CreateCommand
                If FlagCreation Then
                    If File.Exists(CheminComplet) Then File.Delete(CheminComplet)
                    'creation de la base
                    SQLconnect.Open()
                    If Not CreerTableTuiles() Then Exit Try
                    If Not CreerTableAndroid() Then Exit Try
                    'création effectif du fichier sur le disque
                    SQLconnect.Close()
                End If
                Isok = True
            Catch Ex As Exception
                AfficherErreur(Ex, "A1X5")
            End Try
        End Sub
        ''' <summary> libère les ressources</summary>
        Friend Sub Dispose() Implements IDisposable.Dispose
            SQLcommand?.Dispose()
            SQLconnect?.Dispose()
        End Sub
        ''' <summary> écrit une tuile dans la base de données mais dans une boucle d'appel</summary>
        Friend Sub InsererTuile(X As Integer, Y As Integer, Z As Integer, CheminTuile As String)
            SQLcommand.Parameters.AddWithValue("@X", X)
            SQLcommand.Parameters.AddWithValue("@Y", Y)
            SQLcommand.Parameters.AddWithValue("@Z", Z)
            SQLcommand.Parameters.Add(New SQLiteParameter("@image", DbType.Binary) With {.Value = File.ReadAllBytes(CheminTuile)})
            SQLcommand.ExecuteNonQuery()
        End Sub
        ''' <summary> renvoie l'image d'une tuile</summary>
        Friend Function LireTuileToImage(X As Integer, Y As Integer, Z As Integer) As Image
            'décompresse (jpeg) le tableau de byte sous forme d'une image système
            LireTuileToImage = Image.FromStream(New MemoryStream(LireTuile(X, Y, Z)))
        End Function
        ''' <summary> enregistre l'image d'une tuile sous forme de fichier jpeg</summary>
        Friend Sub LireTuileToFichier(X As Integer, Y As Integer, Z As Integer, FilePath As String)
            Try
                'enregistre le tableau de bytes sous forme de fichier sans présager du format effectif de l'image lors de l'enregistrement
                File.WriteAllBytes(FilePath, LireTuile(X, Y, Z))
            Catch Ex As Exception
                AfficherErreur(Ex, "H6Y2")
            End Try
        End Sub
        ''' <summary> renvoie un tableau de byte correspondant au champ image de la Tuile X, Y, Z</summary>
        Friend Function LireTuile(X As Integer, Y As Integer, Z As Integer) As Byte()
            SQLconnect.Open()
            'lecture d'une seule tuile
            SQLcommand.CommandText = "Select [image] FROM [tiles] WHERE [x]=@X And [y]=@Y And [z]=@Z;"
            SQLcommand.Parameters.AddWithValue("@X", X)
            SQLcommand.Parameters.AddWithValue("@Y", Y)
            SQLcommand.Parameters.AddWithValue("@Z", Z)
            LireTuile = DirectCast(SQLcommand.ExecuteScalar(), Byte())
            SQLconnect.Close()
        End Function
        ''' <summary>ecrit chaque tuile d'un niveau de détail sous forme d'un fichier jpeg</summary>
        ''' <param name="Z">Niveau d'affichage</param>
        ''' <param name="Chemin">chemin où seront enregistrés les tuiles</param>
        Friend Function LireNiveauFichier(Z As Integer, Chemin As String) As Boolean
            LireNiveauFichier = False
            SQLconnect.Open()
            Try
                'lecture de toutes les tuiles du niveau
                SQLcommand.CommandText = "Select * FROM [tiles] WHERE [z]=@Z;"
                SQLcommand.Prepare()
                SQLcommand.Parameters.AddWithValue("@Z", Z)
                Using rdr As SQLiteDataReader = SQLcommand.ExecuteReader()
                    While (rdr.Read())
                        'pour chaque tuile on enregistre l'image sous forme de fichier
                        File.WriteAllBytes(Chemin & "C" & CInt(rdr.Item("x")).ToString("000") &
                                                          CInt(rdr.Item("y")).ToString("000") & ".jpg",
                                                          DirectCast(rdr.Item("image"), Byte()))
                    End While
                End Using
                LireNiveauFichier = True
            Catch Ex As Exception
                AfficherErreur(Ex, "T0B0")
            End Try
            SQLconnect.Close()
        End Function
        ''' <summary>ecrit dans la base de données tous les fichiers jpeg d'un répertoire</summary>
        ''' <param name="CarteActuelle">carte correspondante au niveau ORUX</param>
        ''' <param name="Chemin">chemin où sont enregistrés les tuiles</param>
        Friend Function EcrireNiveauFichier(Chemin As String, CarteActuelle As Carte) As Boolean
            EcrireNiveauFichier = False
            'trouve toutes les tuiles du niveau
            Dim ImageTuiles() As FileInfo = New DirectoryInfo(Chemin).GetFiles("*.jpg")
            Dim Z As Integer = CarteActuelle.FacteurORUX
            SQLconnect.Open()
            SQLcommand.CommandText = "INSERT INTO [tiles] ([x], [y], [z], [Image]) VALUES (@X, @Y, @Z, @Image);"
            Try
                With CarteActuelle
                    For Each ImageTuile As FileInfo In ImageTuiles
                        'trouve les X et Y en fonction du nom de l'image
                        Dim X As Integer = CInt(ImageTuile.Name.Substring(1, 3))
                        Dim Y As Integer = CInt(ImageTuile.Name.Substring(4, 3))
                        'insere la tuile dans la table des tuiles
                        InsererTuile(X, Y, Z, ImageTuile.FullName)
                        .NbIterations += 1
                        If .NbIterations Mod 100 = 0 Then AfficherVisueInformation($"{ .MessInfo & .NbIterations} / { .NbTotalIterations}")
                    Next
                End With
                EcrireNiveauFichier = True
            Catch Ex As Exception
                AfficherErreur(Ex, "N0P5")
            End Try
            SQLconnect.Close()
        End Function
        ''' <summary>change l'indice d'affichage d'un niveau de détail</summary>
        ''' <param name="ZA">valeur actuelle du niveau qu'il faut changer</param>
        ''' <param name="ZN">nouvelle valeur du niveau qu'il faut changer</param>
        Friend Function ChangerNiveauAffichage(ZA As Integer, ZN As Integer) As Boolean
            ChangerNiveauAffichage = False
            SQLconnect.Open()
            Try
                SQLcommand.CommandText = "UPDATE [tiles]  Set  [z] = @ZN  WHERE [z]=@ZA;"
                SQLcommand.Parameters.AddWithValue("@ZA", ZA)
                SQLcommand.Parameters.AddWithValue("@ZN", ZN)
                SQLcommand.ExecuteNonQuery()
                ChangerNiveauAffichage = True
            Catch Ex As Exception
                AfficherErreur(Ex, "O7Y5")
            End Try
            SQLconnect.Close()
        End Function
        ''' <summary> 1ère partie de la structure de la base. true si la création de la table c'est bien passée</summary>
        Private Function CreerTableTuiles() As Boolean
            CreerTableTuiles = False
            Try
                SQLcommand = SQLconnect.CreateCommand
                SQLcommand.CommandText = "CREATE TABLE [tiles] ([x] int NOT NULL,  
                                                                [y] int NOT NULL, 
                                                                [z] int NOT NULL, 
                                                                [image] image NULL,  
                                                     CONSTRAINT [sqlite_autoindex_tiles_1] PRIMARY KEY ([x],[y],[z]));
                                                   CREATE INDEX [IND_tiles] ON [tiles] ([x] ASC,[y] ASC,[z] ASC);"
                SQLcommand.ExecuteNonQuery()
                CreerTableTuiles = True
            Catch Ex As Exception
                AfficherErreur(Ex, "S2V7")
            End Try
        End Function
        ''' <summary> 2ème partie de la structure de la base. true si la création de la table c'est bien passée</summary>
        Private Function CreerTableAndroid() As Boolean
            CreerTableAndroid = False
            Try
                'création de la table
                SQLcommand.CommandText = "CREATE TABLE [android_metadata] ( [locale] text NULL );"
                SQLcommand.ExecuteNonQuery()
                'insertion du seul enregistrement
                SQLcommand.CommandText = "INSERT INTO [android_metadata] ([locale]) VALUES ('fr_FR');"
                SQLcommand.ExecuteNonQuery()
                CreerTableAndroid = True
            Catch Ex As Exception
                AfficherErreur(Ex, "O5X2")
            End Try
        End Function
    End Class
#End Region
End Module