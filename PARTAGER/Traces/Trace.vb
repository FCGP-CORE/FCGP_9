Namespace TRKs
    ''' <summary> Gère une trace. La trace est vide si nbsegments = 0. </summary>
    Friend Class TRK
#Region "Commune à toutes les instances"
#Region "Partie visible et commune ou shared concernant les traces"
        ''' <summary> signature de la fonction pour afficher les graphiques d'une trace </summary>
        ''' <param name="Trace"> Trace dont on doit afficher un graphique </param>
        ''' <param name="Is_Vitesse"> S'agit'il de la vitesse ou de l'altitude </param>
        ''' <returns> l'index du point séléctionner en dernier et Ok si il y a eu un double click </returns>
        Friend Delegate Function AfficherGraphiqueTrace(Trace As TRK, Is_Vitesse As Boolean) As (IndexPoint As Integer, DialogResult As DialogResult)

        ''' <summary> determine la distance maximale de la souris à un point ou un segment pour que celui-ci soit sélectionnable </summary>
        Friend Shared Property Sensibilite As Integer
            Get
                Return _Sensibilite
            End Get
            Set(value As Integer)
                _Sensibilite = value
                InitialiserLargeurPinceauxTrace()
            End Set
        End Property
        ''' <summary> couleur du point lorsqu'il va être ajouté ou supprimé. Il n'apparait pas quand il n'est pas survolé </summary>
        Friend Shared Property CouleurPointAjout As Color
            Get
                Return PenPointAjout.Color
            End Get
            Set(value As Color)
                PenPointAjout.Color = value
            End Set
        End Property
        ''' <summary> couleur du segment lorsqu'il est ajouté </summary>
        Friend Shared Property CouleurSegmentAjout As Color
            Get
                Return PenSegmentAjout.Color
            End Get
            Set(value As Color)
                PenSegmentAjout.Color = value
            End Set
        End Property
        ''' <summary> couleur du point lorsqu'il est survolé. Il n'apparait pas quand il n'est pas survolé </summary>
        Friend Shared Property CouleurPointSurvol As Color
            Get
                Return PenPointSurvol.Color
            End Get
            Set(value As Color)
                PenPointSurvol.Color = value
            End Set
        End Property
        ''' <summary> couleur du segment lorsqu'il est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        Friend Shared Property CouleurSegmentSurvol As Color
            Get
                Return PenSegmentSurvol.Color
            End Get
            Set(value As Color)
                PenSegmentSurvol.Color = value
            End Set
        End Property
        ''' <summary> couleur des points lorsque le segment est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        Friend Shared Property CouleurSegmentPointSurvol As Color
            Get
                Return PenSegmentPointSurvol.Color
            End Get
            Set(value As Color)
                PenSegmentPointSurvol.Color = value
            End Set
        End Property
        ''' <summary> couleur du segment lorsqu'il est supprimé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        Friend Shared Property CouleurPointSuppression As Color
            Get
                Return PenPointSuppression.Color
            End Get
            Set(value As Color)
                PenPointSuppression.Color = value
            End Set
        End Property
        ''' <summary> couleur du segment lorsqu'il est supprimé </summary>
        Friend Shared Property CouleurSegmentSuppression As Color
            Get
                Return PenSegmentSuppression.Color
            End Get
            Set(value As Color)
                PenSegmentSuppression.Color = value
            End Set
        End Property
        ''' <summary> site cartographique pour les coordonnées virtuelles des traces </summary>
        Friend Shared ReadOnly Property SiteCarto As SitesCartographiques
        ''' <summary> Domtom du site cartographique pour les coordonnées virtuelles des traces </summary>
        Friend Shared ReadOnly Property DomTom As DomsToms

        Friend Shared ReadOnly Property IndiceSite As Integer
        ''' <summary> Echelle du site cartographique pour les coordonnées virtuelles des traces </summary>
        Friend Shared ReadOnly Property EchelleTrks As Echelles
        ''' <summary> les différentes sorte de format de fichier pour lire une trace </summary>
        Friend Enum TypesFichier As Integer
            Aucun = 0
            TRK = 1
            GPX = 2
        End Enum
        ''' <summary> trace encours d'édition </summary>
        Friend Shared ReadOnly Property TraceEncours As TRK
            Get
                If IndexTraceEncours = -1 Then
                    Return Nothing
                Else
                    Return ListeTraces(IndexTraceEncours)
                End If
            End Get
        End Property
        ''' <summary> indique si il y a une trace qui est encours </summary>
        Friend Shared ReadOnly Property IsTraceEncours() As Boolean
            Get
                Return IndexTraceEncours > -1
            End Get
        End Property
        ''' <summary> flag indiquant qu'il y a une trace encours et que son mode édition est autorisé </summary>
        Friend Shared ReadOnly Property IsTraceEncoursModeEdition() As Boolean
            Get
                Return IndexTraceEncours > -1 AndAlso ListeTraces(IndexTraceEncours)._ModeEdition
            End Get
        End Property
        ''' <summary> flag indiquant qu'il y a une trace encours et qu'elle est encours d'édition </summary>
        Friend Shared ReadOnly Property IsTraceEncoursEditing() As Boolean
            Get
                Return IndexTraceEncours > -1 AndAlso ListeTraces(IndexTraceEncours).Edition > Editions.Aucune
            End Get
        End Property
        ''' <summary> soit crée une trace vierge soit ouvre une trace existante de la collection ou un importe un fichier GPX. Par inteface utilisateur uniquement </summary>
        ''' <param name="Altitude"> La trace va contenir l'information de l'altitude pour chaque point. Dans le cas FCGP_Capture dépend si l'option altiude est cochée </param>
        ''' <param name="Horaire"> La trace va contenir l'information de l'horaire pour chaque point. Dans le cas FCGP_Capture toujours false </param>
        Friend Shared Function OuvrirTraceEncours(Optional Altitude As Boolean = False, Optional Horaire As Boolean = False) As Boolean
            OuvrirTraceEncours = False
            'on ouvre le formulaire OuvrirTrace pour receuillir les intentions de l'utilisateur en transmettant la collection de traces et celle en cours
            Using F As New FormOuvrirTrace() With {.CollectionTraces = ListeTraces, .IndexTrace = IndexTraceEncours}
                F.ShowDialog(FormApplication)
                If F.DialogResult = DialogResult.OK Then
                    'si il y a une trace en cours on la sauve avant de changer
                    FermerTrace(IndexTraceEncours)
                    'on récupère les informations concernant la nouvelle trace
                    Dim R As ResumeTrace = CType(F.Tag, ResumeTrace)
                    If R.Index = -1 Then
                        'Si c'est une nouvelle trace il faut la créer
                        Dim TraceNew As New TRK(100, R.Nom, Altitude, Horaire) With {._IsVisible = True, .CouleurSegment = R.CouleurSegment}
                        ListeTraces.Add(TraceNew)
                        IndexTraceEncours = ListeTraces.Count - 1
                    Else
                        'si la trace est dans la collection
                        IndexTraceEncours = R.Index
                        ListeTraces(IndexTraceEncours).CouleurSegment = R.CouleurSegment
                        'on appele ChangerSiteEchelle à travers la propriété IsVisible pour mettre les coordonnées virtuelles de la trace à jour
                        ListeTraces(IndexTraceEncours).IsVisible = True
                    End If
                    OuvrirTraceEncours = True
                End If
            End Using
        End Function
        ''' <summary> ferme une trace soit en la supprimant soit en la conservant et sauvegardant dans un fichier sur dique. 
        ''' Le formulaire permet soit la sauvegarde sous soit l'export GPX avant la fermeture de la trace. Par programmation ou par inteface utilisateur </summary>
        ''' <param name="FlagDialog"> si true ouvre le formulaire fermeture de trace, sinon ferme directement la trace encours </param>
        Friend Shared Function FermerTraceEncours(FlagDialog As Boolean) As Boolean
            FermerTraceEncours = True
            If FlagDialog Then
                Using F As New FormFermerTrace With {.CollectionTraces = ListeTraces, .IndexTrace = IndexTraceEncours}
                    F.ShowDialog(FormApplication)
                    If F.DialogResult = DialogResult.OK Then
                        'il faut juste fermer la trace en enregistrant ls modifs 
                        FermerTrace(IndexTraceEncours)
                    ElseIf F.DialogResult = DialogResult.Abort Then
                        'il faut supprimer la trace de la collection et son fichier sur le disque
                        SupprimerTrace(IndexTraceEncours)
                        IndexTraceEncours = -1
                    Else
                        FermerTraceEncours = False
                    End If
                End Using
            Else
                FermerTrace(IndexTraceEncours)
            End If
        End Function
        ''' <summary> ouvre le formulaire PropriétésTrace. Le changement du nom de la trace est traité à la fermeture du formulaire </summary>
        ''' <returns> Numpoint si >-1 indique le N° du point de la trace qu'il faut afficher au centre de l'écran
        ''' Return un flag indiquant si la couleur de la trace a changée et qu'il faut la repeindre </returns>
        Friend Shared Function ChangerProprietesTraceEncours(ByRef NumPoint As Integer) As Boolean
            Using F As New FormProprietesTrace With {.Trace = ListeTraces(IndexTraceEncours), .IndexTrace = IndexTraceEncours}
                F.ShowDialog(FormApplication)
                NumPoint = F.IndexPoint
                Return F.Repeindre
            End Using
        End Function
        ''' <summary> enregistre la trace en cours sous forme de fichier binaire </summary>
        Friend Shared Function EnregistrerTraceEncours() As Boolean
            Try
                Dim NomFichier As String = Path.Combine(TracesSettings.CHEMIN_TRACE, ListeTraces(IndexTraceEncours)._Nom & ".trk")
                'on enregistre la trace
                ListeTraces(IndexTraceEncours).EnregistrerTRK(NomFichier)
                Return True
            Catch Ex As Exception
                AfficherErreur(Ex, "L6O1")
                Return False
            End Try
        End Function
        ''' <summary> change la trace encours. Intervient quand on change de site ou de Domtom. Par programmation, pas d'inteface utilisateur </summary>
        Friend Shared Sub ChargerTraceCaptureSettings(Echelle As Echelles)
            IndexTraceEncours = -1
            _SiteCarto = CapturerSettings.SITE
            _DomTom = CapturerSettings.DOMTOM
            _IndiceSite = CapturerSettings.INDICE_SITE
            _EchelleTrks = Echelle
            'on recherche dans la collection de traces si le nom existe
            If CapturerSettings.NOM_TRACE <> "" Then
                For Cpt As Integer = 0 To ListeTraces.Count - 1
                    If ListeTraces(Cpt)._Nom = CapturerSettings.NOM_TRACE Then
                        'si on trouve le nom de la trace dans la collection on la charge comme trace encours en indiquant l'index dans la collection
                        IndexTraceEncours = Cpt
                        'et en la rendant visible en calculant les coordonnées virtuelles propre au site et à l'échelle
                        ListeTraces(IndexTraceEncours).IsVisible = True
                        Exit Sub
                    End If
                Next
                'si on sort normalement de la boucle c'est que la trace n'a pas été trouvée
                CapturerSettings.NOM_TRACE = ""
            End If
        End Sub
        ''' <summary> importe une trace d'après un fichier GPX ou TRK. Dans le cas d'un fichier GPX seule la 1ère trace est importée. Les autres éléments sont ignorés </summary>
        ''' <param name="FichierTrace"> Chemin complet du fichier GPX </param>
        ''' <param name="TypeFichier"> sert à aiguiller le constructeur sur la bonne méthode pour déserialiser les données </param>
        Friend Shared Function OuvrirTrace(FichierTrace As String, TypeFichier As TypesFichier) As Boolean
            OuvrirTrace = False
            Dim Titre = TitreInformation
            TitreInformation = $"Information Trace"
            Try
                Dim IsOk As Boolean
                Dim Trace As New TRK(FichierTrace, TypeFichier, IsOk)
                If Not IsOk Then
                    MessageInformation = $"Le fichier {Path.GetFileName(FichierTrace)} est corrompu."
                    AfficherInformation()
                ElseIf Trace.ListePoints.Count < 2 Then ' pour avoir une trace il faut au moins un segment c'est à dire 2 points
                    MessageInformation = $"Le fichier {Path.GetFileName(FichierTrace)} ne contient aucune trace valide."
                    AfficherInformation()
                Else
                    ListeTraces.Add(Trace)
                    OuvrirTrace = True
                End If
            Catch Ex As Exception
                AfficherErreur(Ex, "T9Y9")
            End Try
            TitreInformation = Titre
        End Function
        ''' <summary> enrgistre une trace sous la forme d'un fichier TRK qui est un fichier binaire spécifique à FCGP ou GPX </summary>
        ''' <param name="IndexTrace"> trace à enregistrer </param>
        ''' <param name="FichierTrace"> Chemin complet du fichier pour sauvegarder la trace </param>
        Friend Shared Sub EnregistrerTrace(IndexTrace As Integer, FichierTrace As String, TypeFichier As TypesFichier)
            If IndexTrace > -1 AndAlso IndexTrace < ListeTraces.Count Then
                Select Case TypeFichier
                    Case TypesFichier.TRK
                        'on cherche d'abord le nom indiquer par le chemin
                        Dim NewNomtrace As String = Path.GetFileNameWithoutExtension(FichierTrace)
                        'si il y a une différence il faut indiqué que l'enregistrement de la trace est obligatoire
                        If NewNomtrace <> ListeTraces(IndexTrace)._Nom Then
                            'on sauvegarde le nom et le flag de modification avant qu'on les change pour l'enregistrement
                            Dim FlagModif As Boolean = ListeTraces(IndexTrace).FlagIsModifie, Nomtrace As String = ListeTraces(IndexTrace)._Nom
                            ListeTraces(IndexTrace).FlagIsModifie = True
                            ListeTraces(IndexTrace)._Nom = NewNomtrace
                            ListeTraces(IndexTrace).EnregistrerTRK(FichierTrace)
                            'on remet la trace en cours à son état initial
                            ListeTraces(IndexTrace).FlagIsModifie = FlagModif
                            ListeTraces(IndexTrace)._Nom = Nomtrace
                            'si la trace est enregistrée dans le répertoire de la collection on l'ajoute à la collection pour qu'elle apparaissent dans la liste de choix de l'ouverture
                            If Path.GetDirectoryName(FichierTrace) = TracesSettings.CHEMIN_TRACE Then
                                Dim IsOk As Boolean
                                Dim Trace As New TRK(FichierTrace, TypeFichier, IsOk)
                                ListeTraces.Add(Trace)
                            End If
                        Else
                            If Path.GetDirectoryName(FichierTrace) = TracesSettings.CHEMIN_TRACE Then
                                'cas de l'enregistrement normal. Si la trace a été modifié elle sera enregistrée
                                ListeTraces(IndexTrace).EnregistrerTRK(FichierTrace)
                            Else
                                'cas de l'enregistrement dans un autre répertoire et il faut rendre l'enregistrement obligatoire. On sauvegarde le flag de modification
                                Dim FlagModif As Boolean = ListeTraces(IndexTrace).FlagIsModifie
                                ListeTraces(IndexTrace).FlagIsModifie = True
                                ListeTraces(IndexTrace).EnregistrerTRK(FichierTrace)
                                'on remet la trace en cours à son état initial
                                ListeTraces(IndexTrace).FlagIsModifie = FlagModif
                            End If
                        End If
                    Case TypesFichier.GPX
                        'il n'y a pas de gestion liée à une collection c'est beaucoup plus simple
                        ListeTraces(IndexTrace).EnregistrerGPX(FichierTrace)
                End Select
            End If
        End Sub
        ''' <summary> supprime la trace de la collection (fichier trk inclus) </summary>
        ''' <param name="IndexTrace"> index de la trace à supprimer </param>
        Friend Shared Sub SupprimerTrace(IndexTrace As Integer)
            If IndexTrace > -1 AndAlso IndexTrace < ListeTraces.Count Then
                Dim FichierTrace As String = Path.Combine(TracesSettings.CHEMIN_TRACE, ListeTraces(IndexTrace)._Nom & ".trk")
                'on supprime le fichier si il existe
                If File.Exists(FichierTrace) Then File.Delete(FichierTrace)
                'on supprime la trace de la collection 
                ListeTraces(IndexTrace) = Nothing 'on met la trace à null pour le GC
                ListeTraces.RemoveAt(IndexTrace) 'et on la supprime de la collection
            End If
        End Sub
        ''' <summary> change le nom de la trace avec le nouveau nom </summary>
        ''' <param name="IndexTrace"> index de la trace dont on doit changer le nom </param>
        ''' <param name="NouveauNom"> nouveau nom de la trace </param>
        Friend Shared Sub RenommerTrace(IndexTrace As Integer, NouveauNom As String)
            If IndexTrace > -1 AndAlso IndexTrace < ListeTraces.Count Then
                Dim FichierTrace As String = Path.Combine(TracesSettings.CHEMIN_TRACE, ListeTraces(IndexTrace)._Nom & ".trk")
                If File.Exists(FichierTrace) Then File.Delete(FichierTrace) 'on supprime le fichier trk existant
                ListeTraces(IndexTrace)._Nom = NouveauNom 'on change le nom 
                ListeTraces(IndexTrace).FlagIsModifie = True ' et on marque qu'elle est modifiée pour être sur quelle soit sauvegardée
            End If
        End Sub
        ''' <summary> renvoie true si le nom passé en paramètre existe dans la collection </summary>
        ''' <param name="NomTrace"> nom de la trace à chercher </param>
        Friend Shared Function IsExisteNomTrace(NomTrace As String) As Boolean
            For Each Trk As TRK In ListeTraces
                If Trk.Nom = NomTrace Then Return True
            Next
            Return False
        End Function
        ''' <summary> renvoi un nom unique basé sur le nom passé en paramètre. Le nom renvoyé n'existe pas dans la collection </summary>
        ''' <param name="NomTrace"> nom de base </param>
        Shared Function RenvoyerNomTraceUnique(NomTrace As String) As String
            Dim Cpt As Integer = 1, Nom As String = NomTrace
            Do While IsExisteNomTrace(Nom)
                Nom = NomTrace & Cpt.ToString("_00")
                Cpt += 1
            Loop
            Return Nom
        End Function
        ''' <summary> Change le site ou l'échelle des coordonnées virtuelles </summary>
        ''' <param name="NouveauSiteCarto"> Nouveau Site carto </param>
        ''' <param name="NouvelleEchelle"> Nouvelle Echelle </param>
        ''' <param name="FlagTraceEncours"> flag qui indique que seule la trace encours est concernée (FCGP_Capture) sinon l'ensemble des traces (FCGP_Visue) </param>
        Friend Shared Sub ChangerSiteEchelleTraces(NouveauSiteCarto As SitesCartographiques, NouveauDomTom As DomsToms, NouvelleEchelle As Echelles, FlagTraceEncours As Boolean)
            _SiteCarto = NouveauSiteCarto
            _DomTom = NouveauDomTom
            _IndiceSite = SystemeCartographique.SiteDomTomToIndex(NouveauSiteCarto, NouveauDomTom)
            _EchelleTrks = NouvelleEchelle
            If FlagTraceEncours AndAlso IndexTraceEncours > -1 Then
                ListeTraces(IndexTraceEncours).ChangerSiteEchelle()
            ElseIf Not FlagTraceEncours Then
                For Each Trace As TRK In ListeTraces
                    If Trace._IsVisible Then Trace.ChangerSiteEchelle()
                Next
            End If
        End Sub
        ''' <summary> dessine la trace encours ou l'ensemble des traces de la collection qui sont visibles à l'affichage </summary>
        ''' <param name="Graph"> surface de dessin </param>
        ''' <param name="BoundingBoxVirtuelleVisue"> coordonnées virtuelles de la surface de dessin. </param>
        ''' <param name="FlagTraceEncours"> flag qui indique que seule la trace encours est concernée (FCGP_Capture) sinon l'ensemble des traces (FCGP_Visue) </param>
        Friend Shared Sub DessinerTraces(Graph As Graphics, BoundingBoxVirtuelleVisue As Rectangle, FlagTraceEncours As Boolean)
            'on ne dessine que la trace encours
            If FlagTraceEncours AndAlso IndexTraceEncours > -1 Then
                ListeTraces(IndexTraceEncours).Dessine(Graph)
            Else
                'on dessine toutes les traces si elles traversent la surface de dessin
                For Index As Integer = 0 To ListeTraces.Count - 1
                    If (ListeTraces(Index)._IsVisible AndAlso ListeTraces(Index)._BoundingBoxVirtuel.IntersectsWith(BoundingBoxVirtuelleVisue)) Then
                        ListeTraces(Index).Dessine(Graph)
                    End If
                Next
            End If
        End Sub
        ''' <summary> Initialise les pinceaux pour le dessin des traces et initialise la collection des traces </summary>
        ''' <param name="AppelAffichegraphique"> méthode pour l'affichage d'un graphique associé à une trace </param>
        Friend Shared Sub InitialiserTraces(Optional AppelAffichegraphique As AfficherGraphiqueTrace = Nothing)
            '1ère initialisation à l'ouverture de l'application
            If ListeTraces Is Nothing Then
                AfficheGraphique = AppelAffichegraphique
                IndexTraceEncours = -1
                _Sensibilite = TracesSettings.SENSIB
                PenPointSurvol = New Pen(TracesSettings.PT_SV, _Sensibilite * 4)
                PenSegmentSurvol = New Pen(TracesSettings.SEG_SV, _Sensibilite * 2) With {.LineJoin = Drawing2D.LineJoin.Round}
                PenSegmentPointSurvol = New Pen(TracesSettings.SEG_PT_SV, _Sensibilite * 3)
                PenPointSuppression = New Pen(TracesSettings.PT_SP, _Sensibilite * 4)
                PenSegmentSuppression = New Pen(TracesSettings.SEG_SP, _Sensibilite * 4) With {.LineJoin = Drawing2D.LineJoin.Round}
                PenPointAjout = New Pen(TracesSettings.PT_AJ, _Sensibilite * 4)
                PenSegmentAjout = New Pen(TracesSettings.SEG_AJ, _Sensibilite) With {.LineJoin = Drawing2D.LineJoin.Round}
                PenTriangle = New Pen(Color.Black, _Sensibilite * 2) With {.EndCap = Drawing2D.LineCap.ArrowAnchor}
                c_CouleurSegment = TracesSettings.SEG_TRK
            End If
            'initialisation de la collection des traces avec les fichiers trace existants dans le répertoire de la collection
            FermerTrace(IndexTraceEncours)
            ListeTraces = New List(Of TRK)(25)
            For Each FichierTRK As String In Directory.GetFiles(TracesSettings.CHEMIN_TRACE, "*.trk", SearchOption.TopDirectoryOnly)
                OuvrirTrace(FichierTRK, TypesFichier.TRK)
            Next
        End Sub
        ''' <summary> Sauvegarde automatique de la trace encours pour ouverture automatique lors de la prochaine utilisation </summary>
        Friend Shared Sub CloturerTraces()
            'on sauvegarde automatiquement la trace en cours que si elle a au moins un segment
            If ListeTraces IsNot Nothing Then CapturerSettings.NOM_TRACE = FermerTrace(IndexTraceEncours)
            'utile pour enregistrer les changements associé au settings des traces
            If TracesSettings IsNot Nothing Then TracesSettings.Ecrire()
            If PenPointSurvol IsNot Nothing Then
                PenPointSurvol.Dispose()
                PenSegmentSurvol.Dispose()
                PenSegmentPointSurvol.Dispose()
                PenPointSuppression.Dispose()
                PenSegmentSuppression.Dispose()
                PenPointAjout.Dispose()
                PenTriangle.Dispose()
            End If
        End Sub
#End Region
#Region "export kml"
        ''' <summary> renvoie une chaine de caratère formater en XML au format Style Map KML  1 ou 2 fois </summary>
        ''' <param name="NbTraces"> nb de fois que l'on doit répeter styleMap</param>
        ''' <param name="CouleurTrace1"> Couleur de la trace pour la 1ère fois</param>
        ''' <param name="EpaisseurTrace1"> Epaisseur de la trace pour la 1ère fois</param>
        ''' <param name="Couleurtrace2"> Couleur de la trace pour la 2ème fois</param>
        ''' <param name="EpaisseurTrace2"> Epaisseur de la trace pour la 2ème fois</param>
        Friend Shared Function AjouterStyleKML(NbTraces As Integer, CouleurTrace1 As Color, EpaisseurTrace1 As Integer,
                                               Couleurtrace2 As Color, EpaisseurTrace2 As Integer) As StringBuilder
            Dim S As New StringBuilder(500)
            Try
                Dim CouleurStr As String = CouleurToStrKML(CouleurTrace1)
                S.Append(StyleLineKML("0", CouleurStr, EpaisseurTrace1.ToString("0")))
                S.Append(StyleLineKML("1", CouleurStr, EpaisseurTrace1.ToString("0")))
                S.Append(StyleMapKML("", "0", "1"))
                If NbTraces = 2 Then
                    CouleurStr = CouleurToStrKML(Couleurtrace2)
                    If CouleurStr Is Nothing Then Return New StringBuilder()
                    S.Append(StyleLineKML("2", CouleurStr, EpaisseurTrace2.ToString("0")))
                    S.Append(StyleLineKML("3", CouleurStr, EpaisseurTrace2.ToString("0")))
                    S.Append(StyleMapKML("4", "2", "3"))
                End If
                Return S
            Catch Ex As Exception
                AfficherErreur(Ex, "D5W9")
                Return New StringBuilder()
            End Try
        End Function
        ''' <summary> renvoie une chaine de caractères formater en XML au format path KML </summary>
        ''' <param name="NbTraces"> nb de fois que l'on incopore les points de la trace dans le fichiers kml</param>
        ''' <param name="Trace"> trace à incorporer </param>
        Friend Shared Function AjouterTraceKML(NbTraces As Integer, Trace As TRK) As StringBuilder
            Dim S As New StringBuilder(25000) ', NbCarateres As Integer = 0
            Try
                S.Append(LineStringKMLDebut(""))
                Dim SF As StringBuilder = LineStringKMLFin(Trace)
                S.Append(SF)
                If NbTraces = 2 Then
                    S.Append(LineStringKMLDebut("4"))
                    S.Append(SF)
                End If
                Return S
            Catch Ex As Exception
                AfficherErreur(Ex, "K5J8")
                Return New StringBuilder()
            End Try
        End Function
        ''' <summary> renvoie une chaine de caractères formater en XML au format path KML mais uniquement la partie avant les points </summary>
        ''' <param name="StyleMap"> styleMap du path KML </param>
        Private Shared Function LineStringKMLDebut(StyleMap As String) As StringBuilder
            Dim S As New StringBuilder(200)
            Try
                S.AppendLine("<Placemark>")
                S.AppendLine("  <name>Path</name>")
                S.AppendLine("  <open>1</open>")
                S.AppendLine("  <styleUrl>#lineStyle" & StyleMap & "</styleUrl>")
                S.AppendLine("  <LineString>")
                S.AppendLine("    <tessellate>1</tessellate>")
                S.AppendLine("    <coordinates>")
                Return S
            Catch Ex As Exception
                AfficherErreur(Ex, "G0Y5")
                Return New StringBuilder()
            End Try
        End Function
        ''' <summary> renvoie une chaine de caractères formater en XML au format path KML mais uniquement la partie points  jusqu'à la fin </summary>
        ''' <param name="Trace"> trace à incorporer </param>
        Private Shared Function LineStringKMLFin(Trace As TRK) As StringBuilder
            Dim S As New StringBuilder(25000), D As Datums = DatumSiteWeb(Trace.Site)
            Try
                For Cpt As Integer = 0 To Trace.ListePoints.Count - 1
                    Dim Altitude As Short = If(Trace.IsAltitude, Trace.ListeAttributs(Cpt).Altitude, 0S)
                    Dim PointDD As PointD = PointPixelToPointGrille(Trace.ListePoints(Cpt), Trace.Site, Trace.Echelle)
                    PointDD = PointGrilleToPointDD(PointDD, D)
                    S.AppendLine(DblToStr(PointDD.X, "N8") & "," & DblToStr(PointDD.Y, "N8") & "," & Altitude)
                Next
                S.AppendLine("    </coordinates>")
                S.AppendLine("  </LineString>")
                S.AppendLine("</Placemark>")
                Return S
            Catch Ex As Exception
                AfficherErreur(Ex, "B0Q6")
                Return New StringBuilder()
            End Try
        End Function
        ''' <summary> renvoie une chaine de caractères formater en XML au format StyleLine KML </summary>
        ''' <param name="Num"> Numéro du style </param>
        ''' <param name="Couleur"> Couleur du style </param>
        ''' <param name="Epaisseur"> Epaisseur du style </param>
        Private Shared Function StyleLineKML(Num As String, Couleur As String, Epaisseur As String) As StringBuilder
            Dim S As New StringBuilder(500)
            Try
                S.AppendLine("<Style id=""lineStyle" & Num & """>")
                S.AppendLine("  <LineStyle>")
                S.AppendLine("    <color>" & Couleur & "</color>")
                S.AppendLine("    <width>" & Epaisseur & "</width>")
                S.AppendLine("  </LineStyle>")
                S.AppendLine("</Style>")
                Return S
            Catch Ex As Exception
                AfficherErreur(Ex, "X2S5")
                Return New StringBuilder()
            End Try
        End Function
        ''' <summary> renvoie une chaine de caractères formater en XML au format StyleMap KML </summary>
        ''' <param name="Num"> numéro du style </param>
        ''' <param name="StyleLine1"> styline normal </param>
        ''' <param name="StyleLine2"> styline Highlight </param>
        Private Shared Function StyleMapKML(Num As String, StyleLine1 As String, StyleLine2 As String) As StringBuilder
            Dim S As New StringBuilder(500)
            Try
                S.AppendLine("<StyleMap id=""lineStyle" & Num & """>")
                S.AppendLine("  <Pair>")
                S.AppendLine("    <key>normal</key>")
                S.AppendLine("    <styleUrl>#lineStyle" & StyleLine1 & "</styleUrl>")
                S.AppendLine("  </Pair>")
                S.AppendLine("  <Pair>")
                S.AppendLine("    <key>highlight</key>")
                S.AppendLine("    <styleUrl>#lineStyle" & StyleLine2 & "</styleUrl>")
                S.AppendLine("  </Pair>")
                S.AppendLine("</StyleMap>")
                Return S
            Catch Ex As Exception
                AfficherErreur(Ex, "X9N3")
                Return New StringBuilder()
            End Try
        End Function
        ''' <summary> renvoie une chaine de caractère HEX représentant la couleur en ABGR </summary>
        ''' <param name="Couleur"> Couleur à renvoyer </param>
        Private Shared Function CouleurToStrKML(Couleur As Color) As String
            Try
                Dim C As Color = Color.FromArgb(Couleur.A, Couleur.B, Couleur.G, Couleur.R)
                Return C.ToArgb.ToString("X")
            Catch Ex As Exception
                AfficherErreur(Ex, "I5F4")
                Return Nothing
            End Try
        End Function
#End Region
#Region "Champs, propriétés, Enum, méthodes, fonctions partagés et privés"
        ''' <summary> délégué à invoquer pour l'affichage des graphiques d'une trace</summary>
        Private Shared AfficheGraphique As AfficherGraphiqueTrace
        ''' <summary> collection des traces recensées dans le réperoire des traces yc la trace encours </summary>
        Private Shared ListeTraces As List(Of TRK)
        ''' <summary> index dans la collection de la trace encours. -1 si pas de trace encours </summary>
        Private Shared IndexTraceEncours As Integer
        ''' <summary> les différentes sorte d'édition d'une trace </summary>
        Private Enum Editions As Integer
            Aucune = 0
            AjouterDeplacer = 1
            Supprimer = 2
            Couper = 3
        End Enum
        ''' <summary> actions possible lors de l'édition d'une trace </summary>
        Private Enum Actions As Integer
            Aucune = 0
            AjouterPointFin = 1
            AjouterPointSegment = 2
            DeplacerPoint = 3
            SupprimerPoint = 4
            SuprimerSegment = 5
            CouperPoint = 6
            CouperSegment = 7
        End Enum
        ''' <summary> sensibilité de la souris pour le survol et la sélection d'un point ou d'un segment exprimé en pixel </summary>
        Private Shared _Sensibilite As Integer
        ''' <summary> pinceau du point lorsqu'il va être ajouté ou supprimé. Il n'apparait pas quand il n'est pas survolé </summary>
        Private Shared PenPointAjout As Pen
        ''' <summary> pinceau du segment lorsqu'il est va être ajouté </summary>
        Private Shared PenSegmentAjout As Pen
        ''' <summary> pinceau du point lorsqu'il est survolé. </summary>
        Private Shared PenPointSurvol As Pen
        ''' <summary> pinceau du segment lorsqu'il est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        Private Shared PenSegmentSurvol As Pen
        ''' <summary> pinceau des points lorsque le segment est survolé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        Private Shared PenSegmentPointSurvol As Pen
        ''' <summary> pinceau du segment lorsqu'il va être supprimé. Doit être différent de la couleur d'ajout et de la couleur de trace </summary>
        Private Shared PenPointSuppression As Pen
        ''' <summary> pinceau du segment lorsqu'il va être supprimé </summary>
        Private Shared PenSegmentSuppression As Pen
        ''' <summary> pinceau des triangles (flêches) indiquant le sens de la trace </summary>
        Private Shared PenTriangle As Pen
        ''' <summary> couleur par defaut de la trace </summary>
        Private Shared c_CouleurSegment As Color
        ''' <summary> change la largeur des pinceaux par defaut et des pinceuax de dessins de chaque trace dans la collection </summary>
        Private Shared Sub InitialiserLargeurPinceauxTrace()
            For Each T As TRK In ListeTraces
                T.PenSegment.Width = _Sensibilite
            Next
            PenPointSurvol.Width = _Sensibilite * 4
            PenSegmentSurvol.Width = _Sensibilite * 2
            PenSegmentPointSurvol.Width = _Sensibilite * 3
            PenPointSuppression.Width = _Sensibilite * 4
            PenSegmentSuppression.Width = _Sensibilite * 4
            PenPointAjout.Width = _Sensibilite * 4
            PenSegmentAjout.Width = _Sensibilite
            PenTriangle.Width = _Sensibilite * 2
        End Sub
        ''' <summary> doit être appeler uniquement par les functions OuvrirTraceEncours ou FermerTraceEncours </summary>
        ''' <param name="IndexTrace"></param>
        Private Shared Function FermerTrace(IndexTrace As Integer) As String
            Dim Ret As String = ""
            If IndexTrace > -1 AndAlso IndexTrace < ListeTraces.Count Then
                If ListeTraces(IndexTrace).NbSegments > 0 Then
                    Ret = ListeTraces(IndexTrace)._Nom
                    Dim NomFichier As String = Path.Combine(TracesSettings.CHEMIN_TRACE, Ret & ".trk")
                    'on enregistre la trace
                    ListeTraces(IndexTrace).EnregistrerTRK(NomFichier)
                Else
                    SupprimerTrace(IndexTrace)
                End If
                IndexTraceEncours = -1
            End If
            Return Ret
        End Function
#End Region
#End Region
#Region "Instance"
#Region "champs privés de l'instance"
        ''' <summary> touches pour les différentes Edition de la trace </summary>
        Private Const ToucheAjouteDeplace As Keys = Keys.A
        Private Const ToucheSupprime As Keys = Keys.S
        Private Const ToucheCoupe As Keys = Keys.C
        ''' <summary> flag indiquant que la trace a été modiée depuis sa lecture et qu'il faut la sauvegarder si on demande son enregistrement </summary>
        Private FlagIsModifie As Boolean
        ''' <summary> site des coordonnées virtuelles et grille </summary>
        Private Site As SitesCartographiques
        Private ReadOnly Property Echelle As Echelles
        ''' <summary> renvoie true si la trace à au moins un attribut </summary>
        Private IsAttribut As Boolean
        '''' <summary> stockage des attributs des points de la trace </summary>
        Private ListeAttributs As List(Of Attribut)
        ''' <summary> stockage des coordonnées virtuelles des points de la trace </summary>
        Private ListePoints As List(Of Point)
        ''' <summary> stockage des coordonnées grille (indépendantes de l'échelle) des points de la trace </summary>
        Private ListeCoordonnees As List(Of PointD)
        ''' <summary> index du point survolé avec le curseur de souris en mode édition </summary>
        Private PointSurvol As Integer
        ''' <summary> index du segment de trace survolé avec le curseur de souris en mode édition </summary>
        Private SegmentSurvol As Integer
        ''' <summary> indique quel sorte d'édition est en cours </summary>
        Private Edition As Editions
        ''' <summary> dernière position connue du curseur de souris en coordonnées virtuelles </summary>
        Private PosMouse As Point
        ''' <summary> attribut associé au point de la souris lors du click </summary>
        Private Attribut As Attribut
        ''' <summary> Est ce que la trace est affichée sur la carte </summary>
        Private _IsVisible As Boolean
        ''' <summary> pinceau des segments "normaux" de la trace </summary>
        Private PenSegment As Pen
        ''' <summary> flag indiquant que la trace est en mode édition donc peut être éditée </summary>
        Private _ModeEdition As Boolean
        ''' <summary> flag indiquant si une action est encours. Dans ce cas il faut attendre qu'elle soit finie pour en définir une autre </summary>
        Private IsActionEncours As Boolean
        ''' <summary> indique quelle action est en cours </summary>
        Private Action As Actions
#End Region
#Region "constructeurs et méthodes associées"
        ''' <summary> constructeur pour l'import d'une trace existante sous forme de fichier GPX ou TRK </summary>
        ''' <param name="Fichier"> chemin complet du fichier contenant la trace à importer </param> 
        ''' <param name="TypeFichier"> Type de fichier à ouvrir </param>
        ''' <param name="IsOk"> indique si le chargement de la trace n'a provoqué une erreur </param>
        Private Sub New(Fichier As String, TypeFichier As TypesFichier, ByRef IsOk As Boolean)
            Select Case TypeFichier
                Case TypesFichier.TRK
                    IsOk = ChargerTRK(Fichier)
                Case TypesFichier.GPX
                    IsOk = ChargerGPX(Fichier)
            End Select
        End Sub
        ''' <summary> import d'une trace existante sous forme de fichier GPX </summary>
        ''' <param name="FichierGPX"> fichier contenant la trace à importer </param>
        ''' <returns> true si il n'y a pas d'erreur </returns>
        Private Function ChargerGPX(FichierGPX As String) As Boolean
            Dim Titre = TitreInformation
            TitreInformation = "Information GPX"
            Try
                Dim GPX As XElement = XElement.Load(FichierGPX)
                Dim xmlnsimport As XNamespace = GPX.Attribute(XName.Get("xmlns")).Value
                Site = _SiteCarto 'on met à jour les différentes propriétés de la trace
                _Echelle = Echelles._000
                _IsVisible = True
                _BoundingBoxDD = New RectangleD(360, -180, -360, 180)
                IntialiserTrace(50, c_CouleurSegment) 'on dimensionne petit au départ
                'nom par défaut si il n'y a pas de trace mais que des points
                _Nom = RenvoyerNomTraceUnique(Path.GetFileNameWithoutExtension(FichierGPX))
                Dim Traces As IEnumerable(Of XElement) = GPX.Elements(xmlnsimport.GetName("trk"))
                If Traces.Any() Then 'il y a au moins une trace
                    For Each Trace In Traces 'si une trace est déclarée on l'importe mais on ne prend que la 1ère
                        For Each ElementTrace As XElement In Trace.Elements 'on cherche les éléments présents
                            If ElementTrace.Name.LocalName = "name" Then
                                _Nom = VerifierNomFichier(Trace.Element(xmlnsimport.GetName("name")).Value)
                                _Nom = RenvoyerNomTraceUnique(_Nom)
                            ElseIf ElementTrace.Name.LocalName = "trkseg" Then 'on regarde si il y doit y avoir une collection de point
                                'si oui on récupère la collection des points de trace
                                Dim TracePoints As IEnumerable(Of XElement) = Trace.Element(xmlnsimport.GetName("trkseg")).Elements(xmlnsimport.GetName("trkpt"))
                                If TracePoints IsNot Nothing Then ImporterPointsGPX(TracePoints, xmlnsimport, 2)
                            End If
                        Next
                        If Traces.Count > 1 Then
                            MessageInformation = "Attention il y a plusieurs traces dans le fichier GPX" & CrLf &
                                                 "Seule la première est importée."
                            AfficherInformation()
                            Exit For
                        End If
                    Next
                Else
                    Dim RTEs As IEnumerable(Of XElement) = GPX.Elements(xmlnsimport.GetName("rte"))
                    If RTEs.Any() Then 'il y a au moins une trace
                        Dim xmlnsextensions As XNamespace = GPX.Attribute(XNamespace.Xmlns.GetName("gpxx")).Value
                        For Each RTE In RTEs 'si une trace est déclarée on l'importe mais on ne prend que la 1ère
                            For Each ElementRTE As XElement In RTE.Elements 'on cherche les éléments présents
                                If ElementRTE.Name.LocalName = "name" Then
                                    _Nom = VerifierNomFichier(RTE.Element(xmlnsimport.GetName("name")).Value)
                                    _Nom = RenvoyerNomTraceUnique(_Nom)
                                ElseIf ElementRTE.Name.LocalName = "rtept" Then 'on regarde si il y a un point de départ ou d'arrivée
                                    'si oui on récupère la collection des points de trace
                                    Dim PointDD As New PointD(StrToDbl(ElementRTE.Attribute(XName.Get("lon")).Value), StrToDbl(ElementRTE.Attribute(XName.Get("lat")).Value))
                                    ListeCoordonnees.Add(PointDDToPointGrille(PointDD, DatumSiteWeb(Site)))
                                    ListePoints.Add(Point.Empty)
                                    If ListeCoordonnees.Count = 1 Then _BoundingBoxDD = New RectangleD(PointDD.X, PointDD.Y, PointDD.X, PointDD.Y)
                                    'on récupère la collection des points
                                    Dim EXTENSION As XElement = ElementRTE.Element(xmlnsimport.GetName("extensions"))
                                    If EXTENSION IsNot Nothing Then
                                        Dim RPTs As IEnumerable(Of XElement) = EXTENSION.Elements(xmlnsextensions.GetName("RoutePointExtension")).Elements(xmlnsextensions.GetName("rpt"))
                                        If RPTs IsNot Nothing Then ImporterPointsGPX(RPTs, xmlnsextensions, 0)
                                    End If
                                End If
                            Next
                            If RTEs.Count > 1 Then
                                MessageInformation = "Attention il y a plusieurs routes dans le fichier GPX" & CrLf &
                                                     "Seule la première est importée."
                                AfficherInformation()
                                Exit For
                            End If
                        Next
                    Else
                        MessageInformation = "Le fichier GPX ne contient ni trace ni route." & CrLf &
                                             "Voulez vous essayer d'importer des points en tant que trace?."
                        If AfficherConfirmation() = DialogResult.OK Then
                            'sinon on récupère la collection des points 
                            Dim WPTs As IEnumerable(Of XElement) = GPX.Elements(xmlnsimport.GetName("wpt"))
                            If WPTs IsNot Nothing Then ImporterPointsGPX(WPTs, xmlnsimport, 2)
                        End If
                    End If
                End If
                ChargerGPX = True 'OK dans le sens il n'y a pas d'erreur à la lecture du fichier GPX
            Catch Ex As Exception
                AfficherErreur(Ex, "R7D1")
                ChargerGPX = False
            End Try
            TitreInformation = Titre
        End Function
        ''' <summary> importe les points GPX à partir d'une collection </summary>
        ''' <param name="CollectionPoints"> collection de points GPX </param>
        ''' <param name="xmlnsimport"> le xnamespace import du document xml</param>
        ''' <param name="TypeAttribut"> indique si'il faut prendre en compte les attributs Altiude et Time s'ils sont présents </param>
        Private Sub ImporterPointsGPX(CollectionPoints As IEnumerable(Of XElement), xmlnsimport As XNamespace, TypeAttribut As Integer)
            Try
                Dim NbPoints As Integer = CollectionPoints.Count
                If NbPoints > 1 Then 'mais pour que la trace soit importée il faut au moin un segment c'est à dire 2 points
                    Dim MinLon As Double = _BoundingBoxDD.Pt0.X, MaxLon As Double = _BoundingBoxDD.Pt2.X
                    Dim MinLat As Double = _BoundingBoxDD.Pt2.Y, MaxLat As Double = _BoundingBoxDD.Pt0.Y
                    If TypeAttribut > 0 Then
                        For Each E As XElement In CollectionPoints.First.Elements 'on cherche les attributs associés à un point
                            If TypeAttribut > 0 AndAlso E.Name.LocalName = "ele" Then 'il peut y avoir l'altitude
                                _IsAltitude = True
                            ElseIf TypeAttribut > 1 AndAlso E.Name.LocalName = "time" Then 'et la date et heure de création
                                _IsHoraire = True
                            End If
                        Next
                    End If
                    Dim Datum As Datums = DatumSiteWeb(Site)
                    IntialiserTrace(NbPoints, c_CouleurSegment) 'on étend la capacité des différentes liste de points pour éviter un redim des différents tableaux
                    'variables servant à calculer le boudingboxDD de la trace
                    For Each Pt As XElement In CollectionPoints
                        Dim PointDD As New PointD(StrToDbl(Pt.Attribute(XName.Get("lon")).Value), StrToDbl(Pt.Attribute(XName.Get("lat")).Value))
                        ListeCoordonnees.Add(PointDDToPointGrille(PointDD, Datum))
                        If PointDD.X < MinLon Then MinLon = PointDD.X
                        If PointDD.X > MaxLon Then MaxLon = PointDD.X
                        If PointDD.Y < MinLat Then MinLat = PointDD.Y
                        If PointDD.Y > MaxLat Then MaxLat = PointDD.Y
                        ListePoints.Add(Point.Empty)
                        If IsAttribut Then
                            Dim Attribut As Attribut
                            If _IsAltitude Then Attribut.Altitude = CShort(StrToDbl(Pt.Element(xmlnsimport.GetName("ele")).Value))
                            If _IsHoraire Then Attribut.Horaire = CDate(Pt.Element(xmlnsimport.GetName("time")).Value)
                            ListeAttributs.Add(Attribut)
                        End If
                    Next
                    _BoundingBoxDD = New RectangleD(MinLon, MaxLat, MaxLon, MinLat)
                    FlagIsModifie = True 'afin de permettre l'enregistrement sous forme binaire
                End If
            Catch Ex As Exception
                AfficherErreur(Ex, "F1I7")
            End Try
        End Sub
        ''' <summary> enregistre la trace sous forme de fichier GPX </summary>
        ''' <param name="NomFichierGPX"> Chemin complet du fichier GPX </param>
        Private Sub EnregistrerGPX(NomFichierGPX As String)
            Dim xmlnsexport As XNamespace = "http://www.topografix.com/GPX/1/0"
            'élémént XML pour contenir les points de la trace
            Dim TRKSEG As New XElement(xmlnsexport + "trkseg")
            Dim Datum As Datums = DatumSiteWeb(Site)
            'on sérialisee chaque point en parcourant la liste du début à la fin
            For PointEncours As Integer = 0 To ListePoints.Count - 1
                'élément XML pour contenir un point de la trace
                Dim PointDD As PointD = PointGrilleToPointDD(ListeCoordonnees(PointEncours), Datum)
                Dim TRKPT As New XElement(xmlnsexport + "trkpt", New XAttribute("lat", PointDD.Y), New XAttribute("lon", PointDD.X))
                If IsAttribut Then
                    If _IsAltitude = True Then TRKPT.Add(New XElement(xmlnsexport + "ele", ListeAttributs(PointEncours).Altitude))
                    'on transforme le temps ordinateur en temps universel
                    If _IsHoraire = True Then TRKPT.Add(New XElement(xmlnsexport + "time",
                                                                     ListeAttributs(PointEncours).Horaire.ToUniversalTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'")))
                End If
                TRKSEG.Add(TRKPT)
            Next
            'définit un xml GPX en écriture
            Dim xws As New XmlWriterSettings() With {.CloseOutput = True,
                                                     .Encoding = Encoding.UTF8,
                                                     .OmitXmlDeclaration = False,
                                                     .Indent = True}
            'détermine les attributs du GPX 
            Dim GPX_VERSION As New XAttribute("version", "1.0")
            Dim GPX_CREATOR As New XAttribute("creator", "FCGP_Capturer - fcgp@laposte.net")
            Dim GPX_XSI As New XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
            Dim GPX_XMLNS As New XAttribute("xmlns", "http://www.topografix.com/GPX/1/0")
            Dim xsi As XNamespace = "http://www.w3.org/2001/XMLSchema-instance"
            Dim GPX_SCHEMA As New XAttribute(xsi + "schemalocation", "http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd")
            'détermine l'élément XML pour contenir la trace
            Dim TRK As New XElement(xmlnsexport + "trk", New XElement(xmlnsexport + "name", _Nom), TRKSEG)
            'détermine l'élément XML du fichier GPX
            Dim GPX As New XElement(xmlnsexport + "gpx", GPX_VERSION, GPX_CREATOR, GPX_XMLNS, GPX_XSI, GPX_SCHEMA, TRK)
            Using xw As XmlWriter = XmlWriter.Create(NomFichierGPX, xws)
                GPX.Save(xw)
            End Using
        End Sub
        ''' <summary> charge la nouvelle trace à partir d'une trace enregistrée sous forme de fichier TRK </summary>
        ''' <param name="FichierTRK"> fichier contenant la trace à importer </param>
        Private Function ChargerTRK(FichierTRK As String) As Boolean
            Try
                Using FileBinaire As New BinaryReader(File.Open(FichierTRK, FileMode.Open, FileAccess.Read))
                    Dim Flags As Byte = FileBinaire.ReadByte
                    _Nom = FileBinaire.ReadString
                    Dim CouleurSegment As Color = Color.FromArgb(FileBinaire.ReadInt32)
                    Site = CType(FileBinaire.ReadByte, SitesCartographiques)
                    _BoundingBoxDD = New RectangleD(FileBinaire.ReadDouble, FileBinaire.ReadDouble, FileBinaire.ReadDouble, FileBinaire.ReadDouble)
                    _Echelle = Echelles._000
                    Dim NbPoints As Integer = FileBinaire.ReadInt32
                    _IsAltitude = (Flags And 1) = 1
                    _IsHoraire = (Flags And 2) = 2
                    _IsVisible = (Flags And 4) = 4
                    IntialiserTrace(NbPoints + 50, CouleurSegment)
                    'on indique le nom du fichier pour pouvoir faire le remplacement lors du prochain enregistrment
                    For PointEncours As Integer = 0 To NbPoints - 1
                        ListeCoordonnees.Add(New PointD(FileBinaire.ReadDouble, FileBinaire.ReadDouble))
                        ListePoints.Add(Point.Empty) 'on remplit la liste avec rien
                        If IsAttribut Then
                            Dim AttributEncours As Attribut
                            If _IsAltitude Then AttributEncours.Altitude = FileBinaire.ReadInt16
                            If _IsHoraire Then AttributEncours.Horaire = DateTime.FromBinary(FileBinaire.ReadInt64)
                            ListeAttributs.Add(AttributEncours)
                        End If
                    Next
                End Using
                'met à jour les coordonnées virtuelles
                ChargerTRK = True
            Catch Ex As Exception
                AfficherErreur(Ex, "D1Z1")
                ChargerTRK = False
            End Try
        End Function
        ''' <summary> enregistre la trace sous forme de fichier TRK </summary>
        ''' <param name="NomFichierTRK"> nom du fichier </param>
        Private Sub EnregistrerTRK(NomFichierTRK As String)
            If FlagIsModifie Then
                Try
                    'la trace est définie et finie donc on peut calculer son emprise
                    CalculerBoundingBoxVirtuel()
                    _BoundingBoxDD = RegionPixelsToRegionDD(_BoundingBoxVirtuel, Site, _Echelle)
                    Using FileBinaire As New BinaryWriter(File.Open(NomFichierTRK, FileMode.Create, FileAccess.Write))
                        FileBinaire.Write(CByte(If(_IsAltitude, 1, 0) + If(_IsHoraire, 2, 0) + If(_IsVisible, 4, 0)))
                        FileBinaire.Write(_Nom)
                        FileBinaire.Write(PenSegment.Color.ToArgb)
                        FileBinaire.Write(CByte(Site))
                        FileBinaire.Write(_BoundingBoxDD.Pt0.X)
                        FileBinaire.Write(_BoundingBoxDD.Pt0.Y)
                        FileBinaire.Write(_BoundingBoxDD.Pt2.X)
                        FileBinaire.Write(_BoundingBoxDD.Pt2.Y)
                        FileBinaire.Write(ListePoints.Count)
                        For PointEncours As Integer = 0 To ListePoints.Count - 1
                            FileBinaire.Write(ListeCoordonnees(PointEncours).X)
                            FileBinaire.Write(ListeCoordonnees(PointEncours).Y)
                            If _IsAltitude Then FileBinaire.Write(ListeAttributs(PointEncours).Altitude)
                            If _IsHoraire Then FileBinaire.Write(ListeAttributs(PointEncours).Horaire.ToBinary)
                        Next
                    End Using
                Catch Ex As Exception
                    AfficherErreur(Ex, "Y2O4")
                End Try
            End If
        End Sub
        ''' <summary> constructeur pour une nouvelle route à remplir avec des données virtuelles existantes. Sert uniquement lors de la coupe d'une trace </summary>
        ''' <param name="PVirtuels"> liste des noeuds à ajouter </param>
        ''' <param name="Altitude"> la nouvelle trace contiendra des altitudes </param>
        ''' <param name="Horaire"> la nouvelle trace conteirndra des horaires </param>
        ''' <param name="Attributs"> si unb attribut est déclaré il s'agit de la liste des attributs à ajouter </param>
        ''' <param name="Nom"> nom de la nouvelle trace </param>
        Private Sub New(PVirtuels() As Point, PWeb() As PointD, Nom As String, Altitude As Boolean, Horaire As Boolean, Attributs() As Attribut)
            Me.New(PVirtuels.Length, Nom, Altitude, Horaire)
            ListePoints.AddRange(PVirtuels)
            ListeCoordonnees.AddRange(PWeb)
            If IsAttribut Then ListeAttributs.AddRange(Attributs)
            _IsVisible = True
            PenSegment.Color = Color.Black
            FlagIsModifie = True
            'et l'enregistrer pour qu'elle apparaisse dans la collection
            EnregistrerTRK(Path.Combine(TracesSettings.CHEMIN_TRACE, _Nom & ".trk"))
        End Sub
        ''' <summary> constructeur pour une nouvelle trace </summary>
        ''' <param name="Nbpoints"> nb de noeuds maximum à prévoir </param>
        ''' <param name="Altitude"> la nouvelle trace contiendra des altitudes </param>
        ''' <param name="Horaire"> la nouvelle trace conteirndra des horaires </param>
        ''' <param name="Nom"> nom de la nouvelle trace </param>
        Private Sub New(NbPoints As Integer, Nom As String, Altitude As Boolean, Horaire As Boolean)
            Site = _SiteCarto
            _Echelle = _EchelleTrks
            _Nom = Nom
            _IsAltitude = Altitude
            _IsHoraire = Horaire
            IntialiserTrace(NbPoints, c_CouleurSegment)
        End Sub
        ''' <summary> initialise les stockages de données de la trace </summary>
        ''' <param name="Capacite"> Nb de points prévus au départ sans avoir de redimensionnement des tableaux </param>
        ''' <param name="CouleurSegment"> Couleur de la trace </param>
        Private Sub IntialiserTrace(Capacite As Integer, CouleurSegment As Color)
            PointSurvol = -1
            PenSegment = New Pen(CouleurSegment, _Sensibilite) With {.LineJoin = Drawing2D.LineJoin.Round}
            IsAttribut = _IsHoraire OrElse _IsAltitude
            If ListePoints Is Nothing Then
                ListePoints = New List(Of Point)(Capacite)
                ListeCoordonnees = New List(Of PointD)(Capacite)
                ListeAttributs = New List(Of Attribut)(Capacite)
            Else
                ListePoints.Capacity += Capacite
                ListeCoordonnees.Capacity += Capacite
                If IsAttribut Then ListeAttributs.Capacity += Capacite
            End If
        End Sub
        ''' <summary> seul construceur public qui permet de récupérer une trace existante en dehors de la collection </summary>
        ''' <param name="FichierTRK"> fichier binaire qui correspond à la sérialisation de la trace </param>
        Friend Sub New(FichierTRK As String, Site As SitesCartographiques, Domtom As DomsToms, Echelle As Echelles)
            _SiteCarto = Site
            _DomTom = Domtom
            _IndiceSite = SystemeCartographique.SiteDomTomToIndex(Site, Domtom)
            _EchelleTrks = Echelle
            'si le fichier a bien été charger on calcule les coordonnées virtuelles de latrace et son emprise
            If ChargerTRK(FichierTRK) Then ChangerSiteEchelle()
        End Sub
#End Region
#Region "Fonction ou méthode se rapportant au point"
        ''' <summary> renvoie les points par rapport au noeud survolé qui permettent le dessin d'un carré de 4*4 sensibilité avec un stylo de 4 sensibilité  d'épaisseur </summary>
        Private Shared Function LimitesPoint(Pointencours As Point) As Point()
            Return New Point() {Point.Add(Pointencours, New Size(-_Sensibilite * 2, 0)), Point.Add(Pointencours, New Size(_Sensibilite * 2, 0))}
        End Function
        ''' <summary> renvoie les points par rapport au segment survolé qui permettent le dessin d'un carré de 3*3 sensibilité avec un stylo de 3 sensibilité d'épaisseur </summary>
        Private Shared Function LimitesPointSegmentSurvol(Pointencours As Point) As Point()
            Dim Width As Integer = CInt(_Sensibilite * 1.5)
            Return New Point() {Point.Add(Pointencours, New Size(-Width, 0)), Point.Add(Pointencours, New Size(Width, 0))}
        End Function
        ''' <summary> cherche si un point de la trace est survolé par le curseur de souris </summary>
        Private Function ChercherMouseSurvolPoint() As Boolean
            PointSurvol = -1
            'on définit le rectangle de sensibilité définit par la position de la souris
            Dim MouseRectangle As New Rectangle(PosMouse.X - _Sensibilite, PosMouse.Y - _Sensibilite, _Sensibilite * 2 + 1, _Sensibilite * 2 + 1)
            'et pour chaque point on regarde si il est dans le rectangle de sensibilité en commençant par le dernier
            For PointEncours = ListePoints.Count - 1 To 0 Step -1
                If MouseRectangle.Contains(ListePoints(PointEncours)) Then
                    PointSurvol = PointEncours
                    Exit For 'le premier trouver est gagnant
                End If
            Next
            Return PointSurvol > -1
        End Function
#Disable Warning IDE0051 ' Supprimer les membres privés non utilisés
        ''' <summary> cherche si un point de la trace est survolé par le curseur de souris. Même que précédament mais en version multi threads </summary>
        Private Function ChercherMouseSurvolPointParallel() As Boolean
#Enable Warning IDE0051 ' Supprimer les membres privés non utilisés
            PointSurvol = -1
            Dim MouseRectangle As New Rectangle(PosMouse.X - _Sensibilite, PosMouse.Y - _Sensibilite, _Sensibilite * 2 + 1, _Sensibilite * 2 + 1)
            Parallel.For(0, ListePoints.Count,
                     Sub(PointEncours As Integer, State As ParallelLoopState)
                         Dim Cpt As Integer = ListePoints.Count - PointEncours - 1
                         If MouseRectangle.Contains(ListePoints(Cpt)) Then
                             PointSurvol = Cpt
                             State.Stop()
                         End If
                     End Sub)
            Return PointSurvol > -1
        End Function
#End Region
#Region "Fonction ou méthode se rapportant au segment"
        ''' <summary> cette fonction permet de savoir si le segment est sélectionable lors d'un déplacement de la souris </summary>
        ''' <param name="SegmentEncours"> segment à tester </param>
        ''' <returns> true si le segment est sélectionable </returns>
        ''' <remarks> pour l'instant ne renvoie pas le point d'intersection avec le segment </remarks>
        Private Function SegmentEstSelectionable(SegmentEncours As Integer) As Boolean
            Dim PtDeb As Point = ListePoints(SegmentEncours - 1), PtFin As Point = ListePoints(SegmentEncours)
            'calcul de la Longueur du segment au carré et de la longueur
            Dim Bx_Ax As Integer = PtFin.X - PtDeb.X
            Dim By_Ay As Integer = PtFin.Y - PtDeb.Y
            Dim L_2 As Integer = Bx_Ax * Bx_Ax + By_Ay * By_Ay
            'Dim L As Double = Math.Sqrt(L_2)
            Dim Cx_Ax As Integer = PosMouse.X - PtDeb.X
            Dim Cy_Ay As Integer = PosMouse.Y - PtDeb.Y
            Dim r_numer As Integer = Cx_Ax * Bx_Ax + Cy_Ay * By_Ay
            'Le point C ne sera pas sur le segment si r_numer < 0 ou > L_2 donc le segment ne peut pas être sélectionné
            If r_numer < 0 OrElse r_numer > L_2 Then Return False
            'le point d'intersection est sur le segment AB on calcule la distance de C à AB
            Dim D As Double = Math.Abs((-Cy_Ay * Bx_Ax + Cx_Ax * By_Ay) / Math.Sqrt(L_2))
            'si la distance est supérieure à la sensibilité le segment ne peut pas être sélectionné
            If D > _Sensibilite Then Return False
            'on calcule les coordonnées du point d'intersection de la projection orthogonnale de C sur le segment AB.
            'Dim Coef_R As Double = r_numer / L_2
            'PtIntersect.X = PtDeb.X + CInt(Coef_R * Bx_Ax)
            'PtIntersect.Y = PtDeb.Y + CInt(Coef_R * By_Ay)
            Return True
        End Function
        ''' <summary> renvoie un rectangle qui contient le segment </summary>
        ''' <param name="SegmentEncours"> index du segment </param>
        ''' <remarks> aucune vérification sur le N° de segment </remarks>
        Private Function BoundingBoxSegment(SegmentEncours As Integer) As Rectangle
            Dim PtDeb As Point = ListePoints(SegmentEncours - 1), PtFin As Point = ListePoints(SegmentEncours)
            BoundingBoxSegment.X = Math.Min(PtDeb.X, PtFin.X)
            BoundingBoxSegment.Y = Math.Min(PtDeb.Y, PtFin.Y)
            Dim D As Integer = Math.Abs(PtDeb.X - PtFin.X)
            BoundingBoxSegment.Width = If(D < _Sensibilite * 2, 4, D)
            D = Math.Abs(PtDeb.Y - PtFin.Y)
            BoundingBoxSegment.Height = If(D < _Sensibilite * 2, 4, D)
        End Function
        ''' <summary> cherche si un segment est survolé par la souris </summary>
        Private Function ChercherMouseSurvolSegment() As Boolean
            SegmentSurvol = 0
            For SegmentEncours = 1 To ListePoints.Count - 1
                'on cherche le boudingBox représenté par le segment
                Dim SegmentRectangle As Rectangle = BoundingBoxSegment(SegmentEncours)
                'si la souris est dans le boudingbox du rectangle on cherche si elle est à proximté du segment
                If SegmentRectangle.Contains(PosMouse) Then
                    If SegmentEstSelectionable(SegmentEncours) Then
                        SegmentSurvol = SegmentEncours
                        Exit For 'c'est le 1er qui gagne
                    End If
                End If
            Next
            Return SegmentSurvol > 0
        End Function
#Disable Warning IDE0051 ' Supprimer les membres privés non utilisés
        ''' <summary> cherche si un segment est survolé par la souris mais en version multi-threads </summary>
        Private Function ChercherMouseSurvolSegmentParallel() As Boolean
#Enable Warning IDE0051 ' Supprimer les membres privés non utilisés
            SegmentSurvol = 0
            Parallel.For(1, ListePoints.Count,
                     Sub(SegmentEncours As Integer, State As ParallelLoopState)
                         Dim Cpt As Integer = ListePoints.Count - SegmentEncours
                         'on cherche le boudingBox représenté par le segment
                         Dim SegmentRectangle As Rectangle = BoundingBoxSegment(Cpt)
                         'si la souris est dans le boudingbox du rectangle on cherche si elle est à proximté du segment
                         If SegmentEstSelectionable(Cpt) Then
                             SegmentSurvol = Cpt
                             State.Stop()
                         End If
                     End Sub)
            Return SegmentSurvol > 0
        End Function
#End Region
#Region "Fonctions Stat."
        ''' <summary> renvoi 6 champs correspondants aux informations de 2 points exprimés en coordonnées DD </summary>
        ''' <param name="PtDeb"> 1er point ou point de début </param>
        ''' <param name="PtFin"> 2ème point ou point de fin </param>
        ''' <param name="NumSeg"> Numéro du segment (point de fin) dans la collection </param>
        ''' <returns> voir CalculerStatSegmentsTrace </returns>
        Private Function CalculerSegment(PtDeb As PointD, PtFin As PointD, NumSeg As Integer, ByRef Isok As Boolean) _
                         As (Longueur As Integer, Denivele As Integer, Duree As Integer, Cap As Double, Pente As Double, Vitesse As Double)
            Try
                Dim DC = CalculerDistanceCap(PtDeb, PtFin)
                CalculerSegment.Longueur = CInt(DC.Longueur)
                CalculerSegment.Cap = DC.Cap
                If CalculerSegment.Cap < 0 Then CalculerSegment.Cap += 360
                CalculerSegment.Duree = 0
                CalculerSegment.Vitesse = 0
                CalculerSegment.Denivele = 0
                CalculerSegment.Pente = 0
                If _IsHoraire Then
                    CalculerSegment.Duree = CInt((ListeAttributs(NumSeg).Horaire - ListeAttributs(NumSeg - 1).Horaire).TotalSeconds)
                    If CalculerSegment.Duree > 0 Then CalculerSegment.Vitesse = CalculerSegment.Longueur / CalculerSegment.Duree
                End If
                If _IsAltitude Then
                    CalculerSegment.Denivele = ListeAttributs(NumSeg).Altitude - ListeAttributs(NumSeg - 1).Altitude
                    If CalculerSegment.Longueur > 0 Then CalculerSegment.Pente = CalculerSegment.Denivele / CalculerSegment.Longueur
                End If
                Isok = True
            Catch Ex As Exception
                AfficherErreur(Ex, "F7M8")
                Isok = False
            End Try
        End Function
        ''' <summary> renvoie les informations correspondant aux segments demandés </summary>
        ''' <param name="NumSegmentDeb"> Numéro du 1er segment dans la collection </param>
        ''' <param name="NumSegmentFin"> Numéro du dernier segment dans la collection </param>
        ''' <returns> 
        ''' Vitesse en m/s 
        ''' Pente en % de la longueur 
        ''' Cap en degré toujours positif de 0 à 359.99
        ''' Durée en seconde
        ''' Denivele en metres
        ''' Longeur en mètres
        ''' </returns>
        Friend Function CalculerStatSegmentsTrace(NumSegmentDeb As Integer, NumSegmentFin As Integer) _
                As List(Of (Longueur As Integer, Denivele As Integer, Duree As Integer, Cap As Double, Pente As Double, Vitesse As Double))
            Dim Ret = New List(Of (Longueur As Integer, Denivele As Integer, Duree As Integer, Cap As Double, Pente As Double, Vitesse As Double))(NumSegmentFin - NumSegmentDeb + 1)
            Dim Datum As Datums = DatumSiteWeb(Site)
            Try
                Dim PtDeb As PointD = PointGrilleToPointDD(ListeCoordonnees(NumSegmentDeb - 1), Datum)
                Dim PtFin As PointD
                Dim IsOk As Boolean
                For NumSeg As Integer = NumSegmentDeb To NumSegmentFin
                    PtFin = PointGrilleToPointDD(ListeCoordonnees(NumSeg), Datum)
                    Dim Segment = CalculerSegment(PtDeb, PtFin, NumSeg, IsOk)
                    If IsOk Then
                        Ret.Add(Segment)
                    Else
                        Ret = Nothing
                        Exit For
                    End If
                    PtDeb = PtFin
                Next
            Catch Ex As Exception
                AfficherErreur(Ex, "7D49")
                Ret = Nothing
            End Try
            Return Ret
        End Function
        ''' <summary> renvoie les informations correspondant au segment demandé </summary>
        ''' <param name="NumSegment"> Numéro du segment dans la collection </param>
        ''' <returns> 
        ''' Vitesse en m/s 
        ''' Pente en % de la longueur 
        ''' Cap en degré toujours positif de 0 à 359.99
        ''' Durée en seconde
        ''' Denivele en metres
        ''' Longeur en mètres
        ''' </returns>
        Friend ReadOnly Property StatsSegment(NumSegment As Integer) _
                        As (Longueur As Integer, Denivele As Integer, Duree As Integer, Cap As Double, Pente As Double, Vitesse As Double)
            Get
                Dim Datum As Datums = DatumSiteWeb(Site)
                Dim PtDeb As PointD = PointGrilleToPointDD(ListeCoordonnees(NumSegment - 1), Datum)
                Dim PtFin As PointD = PointGrilleToPointDD(ListeCoordonnees(NumSegment), Datum)
                Dim IsOk As Boolean
                Dim Stat = CalculerSegment(PtDeb, PtFin, NumSegment, IsOk)
                If IsOk Then
                    Return Stat
                Else
                    Return Nothing
                End If
            End Get
        End Property
        ''' <summary> calcule l'emprise de la trace sous forme d'un rectangle exprimé en coordonnées Virtuelles </summary>
        Private Sub CalculerBoundingBoxVirtuel()
            If ListePoints.Count = 0 Then
                _BoundingBoxVirtuel = Rectangle.Empty
            Else
                Dim MinLon As Integer = ListePoints(0).X, MaxLon As Integer = ListePoints(0).X
                Dim MinLat As Integer = ListePoints(0).Y, MaxLat As Integer = ListePoints(0).Y
                For Cpt As Integer = 1 To ListePoints.Count - 1
                    If ListePoints(Cpt).X < MinLon Then MinLon = ListePoints(Cpt).X
                    If ListePoints(Cpt).X > MaxLon Then MaxLon = ListePoints(Cpt).X
                    If ListePoints(Cpt).Y < MinLat Then MinLat = ListePoints(Cpt).Y
                    If ListePoints(Cpt).Y > MaxLat Then MaxLat = ListePoints(Cpt).Y
                Next
                _BoundingBoxVirtuel = New Rectangle(New Point(MinLon, MinLat), New Size(MaxLon - MinLon, MaxLat - MinLat))
            End If
        End Sub
#End Region
#Region "Propriétés se rapportant à la trace"
        ''' <summary> nd de points composant la trace </summary>
        Friend ReadOnly Property NbPoints As Integer
            Get
                Return ListePoints.Count
            End Get
        End Property
        ''' <summary> Nb de segments de la trace. 1 segment est définit par 2 points </summary>
        Friend ReadOnly Property NbSegments() As Integer
            Get
                If ListePoints.Count > 1 Then
                    Return ListePoints.Count - 1
                Else
                    Return 0
                End If
            End Get
        End Property
        ''' <summary> rend la trace visible ou invisible sur l'affichage. Pour cela elle adapte les coordonnées grille en coordonnées virtuelles </summary>
        Friend WriteOnly Property IsVisible As Boolean
            Set(value As Boolean)
                _IsVisible = value
                If _IsVisible Then
                    'si la trace devient visible il faut être sur que les coord correspondent au système encours
                    ChangerSiteEchelle()
                Else
                    'c'est une précaution mais normalement on ne peut pas rendre invisible la trace encours
                    _ModeEdition = False
                End If
            End Set
        End Property
        ''' <summary> indique si les points de la trace contienne une altitude </summary>
        Friend ReadOnly Property IsAltitude() As Boolean
        ''' <summary> indique si les points de la trace contienne une heure de création </summary>
        Friend ReadOnly Property IsHoraire() As Boolean
        ''' <summary> boundingbox en latlon wgs84 </summary>
        Friend ReadOnly Property BoundingBoxDD() As RectangleD
        '''<summary> pour une nouvelle trace ou une trace qui vient d'être chargée c'est rectangle.Empty </summary>
        Friend ReadOnly Property BoundingBoxVirtuel() As Rectangle
        ''' <summary> Nom donné à la trace </summary>
        Friend Property Nom As String
        ''' <summary> couleur de la trace </summary>
        Friend Property CouleurSegment As Color
            Get
                Return PenSegment.Color
            End Get
            Set(value As Color)
                If PenSegment.Color <> value Then
                    PenSegment.Color = value
                    FlagIsModifie = True
                End If
            End Set
        End Property
        ''' <summary> flag indiquant si on peut modifier la trace en ajoutant, déplaçant et en supprimant des points à l'aide de la souris </summary>
        Friend WriteOnly Property ModeEdition As Boolean
            Set(value As Boolean)
                _ModeEdition = value
            End Set
        End Property
#End Region
#Region "propriété se rapportant au Points"
        ''' <summary> renvoie les coordonnées virtuelles d'un point de la trace </summary>
        ''' <param name="NumPoint"> indice du noeud </param>
        Friend ReadOnly Property CoordonneesVirtuelles(NumPoint As Integer) As Point
            Get
                If NumPoint >= 0 AndAlso NumPoint < ListePoints.Count Then
                    Return ListePoints(NumPoint)
                Else
                    Throw New Exception("Indice Hors Limites")
                End If
            End Get
        End Property
        ''' <summary> renvoie les coordonnées systèmecartographique d'un point de la trace </summary>
        ''' <param name="NumPoint"> indice du noeud </param>
        Friend ReadOnly Property CoordonneesGrille(NumPoint As Short) As PointD
            Get
                If NumPoint >= 0 AndAlso NumPoint < ListePoints.Count Then
                    Return PointPixelToPointGrille(ListePoints(NumPoint), Site, Echelle)
                Else
                    Throw New Exception("Indice Hors Limites")
                End If
            End Get
        End Property
        ''' <summary> renvoie les coordonnées longitude / latitude d'un point de la trace </summary>
        ''' <param name="NumPoint"> indice du noeud </param>
        Friend ReadOnly Property CoordonneesDD(NumPoint As Integer) As PointD
            Get
                If NumPoint >= 0 AndAlso NumPoint < ListePoints.Count Then
                    Dim PointGrille As PointD = PointPixelToPointGrille(ListePoints(NumPoint), Site, Echelle)
                    Return PointGrilleToPointDD(PointGrille, DatumSiteWeb(Site))
                Else
                    Throw New Exception("Indice Hors Limites")
                End If
            End Get
        End Property
        ''' <summary> renvoie l'altitude d'un point de la trace </summary>
        ''' <param name="NumPoint"> indice du noeud </param>
        Friend ReadOnly Property Altitude(NumPoint As Integer) As Short
            Get
                If NumPoint >= 0 AndAlso NumPoint < ListePoints.Count Then
                    If _IsAltitude Then
                        Return ListeAttributs(NumPoint).Altitude
                    Else
                        Return 0
                    End If
                Else
                    Throw New Exception("Indice Hors Limites")
                End If
            End Get
        End Property
        ''' <summary> renvoie la date d'un point de la trace </summary>
        ''' <param name="NumPoint"> indice du noeud </param>
        Friend ReadOnly Property Horaire(NumPoint As Integer) As Date
            Get
                If NumPoint >= 0 AndAlso NumPoint < ListePoints.Count Then
                    If _IsHoraire Then
                        Return ListeAttributs(NumPoint).Horaire
                    Else
                        Return Date.MinValue
                    End If
                Else
                    Throw New Exception("Indice Hors Limites")
                End If
            End Get
        End Property
        Friend ReadOnly Property ListePointsPixel() As Point()
            Get
                If ListePoints IsNot Nothing Then
                    Return ListePoints.ToArray
                Else
                    Return Nothing
                End If
            End Get
        End Property
#End Region
#Region "Interaction avec l'utilisateur"
        ''' <summary> Transforme les coordonnées virtuelles des points existants pour en coordonnées virtuelles pour le nouveau site et ou la nouvelle echelle </summary>
        Private Sub ChangerSiteEchelle()
            'on fait le changement que si le site est différent mais on change l'échelle avec
            If Site <> _SiteCarto Then
                Dim DatumAncienSite = DatumSiteWeb(Site)
                Dim DatumNouveauSite = DatumSiteWeb(_SiteCarto)
                'puis à répéter autant de fois qu'il y a de noeuds restants
                For PointEncours As Integer = 0 To ListeCoordonnees.Count - 1
                    'on converti les coordonnéesGrille avec le site et l'echelle en cours en coordonnées lon/lat wgs84
                    Dim PointWebNew As PointD = ConvertProjectionToWGS84(DatumAncienSite, New PointProjection(ListeCoordonnees(PointEncours)))
                    'on converti les coordonnées lon/lat wgs84 en coordonnées grille du nouveau site 
                    PointWebNew = ConvertWGS84ToProjection(PointWebNew, DatumNouveauSite).Coordonnees
                    'on converti les coordonnées grille du nouveau site en coordonnées virtuelle pour le nouveau site et la nouvelle echelle
                    ListeCoordonnees(PointEncours) = PointWebNew
                    ListePoints(PointEncours) = PointGrilleToPointPixel(PointWebNew, _SiteCarto, _EchelleTrks)
                Next
                Site = _SiteCarto   'on met à jour le site
                _Echelle = _EchelleTrks 'et l'échelle
                CalculerBoundingBoxVirtuel()
            ElseIf _Echelle <> _EchelleTrks Then 'le site est identique seule l'echelle change
                For PointEncours As Integer = 0 To ListeCoordonnees.Count - 1
                    'on converti les coordonnées grille  en coordonnées virtuelle pour la nouvelle echelle
                    ListePoints(PointEncours) = PointGrilleToPointPixel(ListeCoordonnees(PointEncours), Site, _EchelleTrks)
                Next
                _Echelle = _EchelleTrks
                CalculerBoundingBoxVirtuel()
            End If
        End Sub
        ''' <summary> dessine la trace à la demande de l'application si visible. Le test de visibilité doit être fait par la procédure appelante </summary>
        ''' <param name="Graph"> surface graphique représentée par la surface d'affichage </param>
        Private Sub Dessine(Graph As Graphics)
            'si il y a au moins un segment contenu par la trace on le(s) représente(nt) sur la surface d'affichage
            If ListePoints.Count - 1 > 0 Then
                Dim T As Point() = ListePoints.ToArray
                Graph.DrawLines(PenSegment, T)
                DessinerSensLigne(Graph)
            ElseIf ListePoints.Count > 0 Then
                'on représente le premier point
                Graph.DrawLines(PenPointAjout, LimitesPoint(ListePoints(0)))
            End If
            'si la trace est modifiable on passe au dessin du survol et des actions
            If _ModeEdition Then DessinerTraceModification(Graph)
        End Sub
        ''' <summary> dessine la trace à sa demande </summary>
        ''' <param name="Graph"> surface graphique représentée par la surface d'affichage </param>
        Private Sub DessinerTraceModification(Graph As Graphics)
            If PointSurvol > -1 Then
                DessinerTracePointSurvol(Graph)
            ElseIf SegmentSurvol > 0 Then
                DessinerTraceSegmentSurvol(Graph)
                If Edition = Editions.AjouterDeplacer Then
                    Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse))
                End If
            ElseIf Edition = Editions.AjouterDeplacer AndAlso ListePoints.Count > 0 Then
                'dessine le segment qui peut être ajouté en fin de trace
                Graph.DrawLine(PenSegmentAjout, ListePoints(ListePoints.Count - 1), PosMouse)
            End If
        End Sub
        ''' <summary> dessinne le point qui est survolé si pas d'action ou le ou les segements ajoutés et ou supprimés si action </summary>
        ''' <param name="Graph"> surface de dessin de l'affichage </param>
        Private Sub DessinerTracePointSurvol(Graph As Graphics)
            Select Case Action
                Case Actions.DeplacerPoint
                    If IsActionEncours Then
                        If PointSurvol > 0 Then
                            'dessin du segment existant donc supprimé avant le point à déplacer  si celui-ci existe
                            Graph.DrawLine(PenSegmentSuppression, ListePoints(PointSurvol - 1), ListePoints(PointSurvol))
                            'dessin du segment déplacé donc ajouté situé avant le point à déplacer  si celui-ci existe
                            Graph.DrawLine(PenSegmentAjout, ListePoints(PointSurvol - 1), PosMouse)
                        End If
                        If PointSurvol < ListePoints.Count - 1 Then
                            'dessin du segment existant donc supprimé après le point à déplacer  si celui-ci existe
                            Graph.DrawLine(PenSegmentSuppression, ListePoints(PointSurvol), ListePoints(PointSurvol + 1))
                            'dessin du segment situé après le point à déplacer si celui-ci existe
                            Graph.DrawLine(PenSegmentAjout, PosMouse, ListePoints(PointSurvol + 1))
                        End If
                        'dessin du point existant donc supprimé
                        Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints(PointSurvol)))
                        'dessin du point déplacer
                        Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse))
                    Else
                        Graph.DrawLines(PenPointSurvol, LimitesPoint(ListePoints(PointSurvol)))
                    End If
                Case Actions.SupprimerPoint
                    If Not IsActionEncours Then
                        'dessin d'un segment à supprimer si il exite. Cas particulier du segment de début de trace, il faut un segment après le point survolé
                        Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints(PointSurvol)))
                        If PointSurvol >= 0 AndAlso PointSurvol < ListePoints.Count - 1 Then  'point de fin de trace exclu
                            Graph.DrawLine(PenSegmentSuppression, ListePoints(PointSurvol), ListePoints(PointSurvol + 1))
                        End If
                        'dessin d'un 2ème segment à supprimer si il exite. Cas particulier du segment de fin de trace, il faut un segment avant le point survolé
                        If PointSurvol > 0 AndAlso PointSurvol <= ListePoints.Count - 1 Then 'point de début de trace exclu
                            Graph.DrawLine(PenSegmentSuppression, ListePoints(PointSurvol - 1), ListePoints(PointSurvol))
                        End If
                        'dessin du segment à ajouter si il existe. Il faut un segment de part et d'autre du point survolé
                        If PointSurvol > 0 AndAlso PointSurvol < ListePoints.Count - 1 Then
                            Graph.DrawLine(PenSegmentAjout, ListePoints(PointSurvol - 1), ListePoints(PointSurvol + 1))
                        End If
                    End If
                Case Actions.CouperPoint
                    If Not IsActionEncours Then
                        Dim NbPointsCouper = ListePoints.Count - PointSurvol
                        If NbPointsCouper > 1 Then
                            Dim T(NbPointsCouper - 1) As Point
                            ListePoints.CopyTo(PointSurvol, T, 0, NbPointsCouper)
                            Graph.DrawLines(PenSegmentSuppression, T)
                        End If
                        'dessin du  point de coupe
                        Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse))
                    End If
                Case Else
                    'si un point est survolé, il y a une action à réaliser
                    Graph.DrawLines(PenPointSurvol, LimitesPoint(ListePoints(PointSurvol)))
            End Select
        End Sub
        ''' <summary> dessinne le segment qui est survolé si pas d'action ou le ou les segements ajoutés et ou supprimés si action </summary>
        ''' <param name="Graph"> surface de dessin de l'affichage </param>
        Private Sub DessinerTraceSegmentSurvol(Graph As Graphics)
            Select Case Action
                Case Actions.SuprimerSegment
                    If Not IsActionEncours Then
                        'dessin du segment et du point à supprimer
                        Graph.DrawLine(PenSegmentSuppression, ListePoints(SegmentSurvol - 1), ListePoints(SegmentSurvol))
                        If SegmentSurvol = 1 Then
                            'si c'est le premier segment de la trace le point à supprimer est le point de début du segment
                            Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints(SegmentSurvol - 1)))
                        Else
                            'sinon c'est le point de fin du segment
                            Graph.DrawLines(PenPointSuppression, LimitesPoint(ListePoints(SegmentSurvol)))
                        End If
                        If SegmentSurvol > 1 AndAlso SegmentSurvol < ListePoints.Count - 1 Then
                            'eventuellement dessin du nouveau segment après suppression du segment survolé (point de fin du segment)
                            Graph.DrawLine(PenSegmentSuppression, ListePoints(SegmentSurvol), ListePoints(SegmentSurvol + 1))
                            Graph.DrawLine(PenSegmentAjout, ListePoints(SegmentSurvol - 1), ListePoints(SegmentSurvol + 1))
                        End If
                    End If
                Case Actions.CouperSegment
                    If Not IsActionEncours Then
                        Dim NbPointsCouper = ListePoints.Count - SegmentSurvol
                        Dim T(NbPointsCouper) As Point
                        T(0) = PosMouse
                        ListePoints.CopyTo(SegmentSurvol, T, 1, NbPointsCouper)
                        'dessin des segments à couper
                        Graph.DrawLines(PenSegmentSuppression, T)
                        'dessin du  point de coupe
                        Graph.DrawLines(PenPointAjout, LimitesPoint(PosMouse))
                    End If
                Case Else
                    'on dessine le segment survolé
                    Graph.DrawLine(PenSegmentSurvol, ListePoints(SegmentSurvol - 1), ListePoints(SegmentSurvol))
                    Graph.DrawLines(PenSegmentPointSurvol, LimitesPointSegmentSurvol(ListePoints(SegmentSurvol - 1)))
                    Graph.DrawLines(PenSegmentPointSurvol, LimitesPointSegmentSurvol(ListePoints(SegmentSurvol)))
            End Select
        End Sub
        ''' <summary> dessinne les flêches qui indique le sens des segments </summary>
        ''' <param name="G"> surface de dessin de l'affichage </param>
        Private Sub DessinerSensLigne(G As Graphics)
            Dim PointsFleche As PointF() = New PointF() {Nothing, Nothing}
            For Cpt As Integer = 1 To ListePoints.Count - 1
                If MilieuSegment(Cpt, PointsFleche) Then G.DrawLines(PenTriangle, PointsFleche)
            Next
        End Sub
        ''' <summary> renvoie true si le segment à une longueur = ou supérieur à 10 * sensibilité et renvoie le point milieu du segment</summary>
        ''' <param name="NumSegment"> numéro du segment de 1 à listepoints.count-1</param>
        ''' <param name="PointsFleche"> Point qui contient le milieu du segment</param>
        Private Function MilieuSegment(NumSegment As Integer, PointsFleche() As PointF) As Boolean
            Dim DeltaX As Integer = ListePoints(NumSegment).X - ListePoints(NumSegment - 1).X
            Dim DeltaY As Integer = ListePoints(NumSegment).Y - ListePoints(NumSegment - 1).Y
            'si la longueur du segment est trop petite on ne dessine pas le flèche
            Dim L As Double = Math.Sqrt(DeltaX ^ 2 + DeltaY ^ 2)
            If L < 10 * _Sensibilite Then Return False
            'calcul des coordonnées du point milieu au segment
            Dim M As New PointD((ListePoints(NumSegment).X + ListePoints(NumSegment - 1).X) / 2, (ListePoints(NumSegment).Y + ListePoints(NumSegment - 1).Y) / 2)
            'et calcul des points de début et de fin de la flêche par trigo
            Dim Sin As Double = DeltaY / L
            Dim Cos As Double = DeltaX / L
            PointsFleche(0) = PointD.Offset(M, -_Sensibilite * Cos, -_Sensibilite * Sin).ToPointF
            PointsFleche(1) = PointD.Offset(M, _Sensibilite * Cos, _Sensibilite * Sin).ToPointF
            Return True
        End Function
        ''' <summary> indique si la trace est concernée par le déplacement de la souris. On donne priorité au point </summary>
        Friend Function SourisBouge(PointMouse As Point) As Boolean
            PosMouse = PointMouse
            'si une action est encours il n'y a plus besoin de trouver un point ou un segment survolé
            If Not IsActionEncours Then 'c'est le point survolé qui est prioritaire
                If Not ChercherMouseSurvolPoint() Then
                    'si pas de point survolé est ce qu'il y a un segment survolé
                    ChercherMouseSurvolSegment()
                End If
                'si la trace est mode mode édition on cherche l'action que l'on peut réaliser
                Select Case Edition
                    Case Editions.AjouterDeplacer
                        If PointSurvol > -1 Then  'action à réaliser sur l'événementSourisUP
                            Action = Actions.DeplacerPoint
                            CurseurEncours = Curseurs.TraceDeplacePt
                        ElseIf SegmentSurvol > 0 Then  'action à réaliser sur l'événementSourisDOWN
                            Action = Actions.AjouterPointSegment
                            CurseurEncours = Curseurs.TraceInsertPt
                        Else  'action à réaliser sur l'événementSourisDOWN
                            Action = Actions.AjouterPointFin
                            CurseurEncours = Curseurs.TraceInsertPt
                        End If
                    Case Editions.Supprimer
                        CurseurEncours = Curseurs.TraceSupprime
                        'on se contente de définir l'action à réaliser sur l'événementSourisUP
                        If PointSurvol > -1 Then
                            Action = Actions.SupprimerPoint
                            CurseurEncours = Curseurs.TraceSupprimePtSeg
                        ElseIf SegmentSurvol > 0 Then
                            Action = Actions.SuprimerSegment
                            CurseurEncours = Curseurs.TraceSupprimePtSeg
                        End If
                    Case Editions.Couper
                        CurseurEncours = Curseurs.TraceCoupe
                        'on se contente de définir l'action à réaliser sur l'événementSourisUP
                        If PointSurvol > -1 Then
                            Action = Actions.CouperPoint
                            CurseurEncours = Curseurs.TraceCoupePtSeg
                        ElseIf SegmentSurvol > 0 Then
                            Action = Actions.CouperSegment
                            CurseurEncours = Curseurs.TraceCoupePtSeg
                        End If
                End Select
            End If
            Return True 'pour dessiner le point ou le segment survolé
        End Function
        ''' <summary> Lance l'action qui peut être réalisée sur un relachement du bouton gauche de la souris </summary>
        Friend Function SourisUp(PointMouse As Point, Attrib As Attribut) As Boolean
            'c'est la fin du déplacement qui compte pour cette édition
            PosMouse = PointMouse
            Attribut = Attrib
            'si une action est encours il faut aller voir si il y en a une qui prend à son compte l'évenement
            If IsActionEncours Then
                RealiserActionsSourisUP()
                FlagIsModifie = True 'on indique que la trace a été modifié
                IsActionEncours = False 'on indique qu'il n'y a plus d'action encours
                Action = Actions.Aucune
                SegmentSurvol = 0
                PointSurvol = -1
                Return True 'pour dessiner l'action si besoin
            End If
            Return False
        End Function
        ''' <summary>réalise l'action sur la trace quand le bouton gauche de la souris est relevé : Evenement MouseUP </summary>
        Private Sub RealiserActionsSourisUP()
            Select Case Action
                Case Actions.DeplacerPoint
                    ListePoints(PointSurvol) = PosMouse
                    ListeCoordonnees(PointSurvol) = PointPixelToPointGrille(PosMouse, Site, Echelle)
                    If IsAttribut Then
                        Attribut.Horaire = ListeAttributs(PointSurvol).Horaire
                        ListeAttributs(PointSurvol) = Attribut 'on met à jour l'altitude du point
                    End If
                    CurseurEncours = Curseurs.TraceEdite
                Case Actions.SupprimerPoint
                    ListePoints.RemoveAt(PointSurvol)
                    ListeCoordonnees.RemoveAt(PointSurvol)
                    If IsAttribut Then ListeAttributs.RemoveAt(PointSurvol)
                    CurseurEncours = Curseurs.TraceSupprime
                Case Actions.SuprimerSegment
                    ListePoints.RemoveAt(SegmentSurvol)
                    ListeCoordonnees.RemoveAt(SegmentSurvol)
                    If IsAttribut Then ListeAttributs.RemoveAt(SegmentSurvol)
                    CurseurEncours = Curseurs.TraceSupprime
                Case Actions.CouperPoint
                    CouperTraceAuPoint()
                    CurseurEncours = Curseurs.TraceCoupe
                    FlagIsModifie = True
                Case Actions.CouperSegment
                    CouperTraceAuSegment()
                    CurseurEncours = Curseurs.TraceCoupe
                    FlagIsModifie = True
            End Select
        End Sub
        ''' <summary> coupe la trace principale au niveau du point et enregistre la partie coupée (du point de coupe à la fin) en tant que trace TRK et l'ajoute dans la collection </summary>
        Private Sub CouperTraceAuPoint()
            Dim NbPointsNouvelleTrace = ListePoints.Count - PointSurvol - 1
            If NbPointsNouvelleTrace > 0 Then 'Il faut au moins un segment pour créer une nouvelle trace
                Dim PVirtuels(NbPointsNouvelleTrace) As Point
                ListePoints.CopyTo(PointSurvol, PVirtuels, 0, NbPointsNouvelleTrace + 1)
                ListePoints.RemoveRange(PointSurvol + 1, NbPointsNouvelleTrace)
                Dim PWeb(NbPointsNouvelleTrace) As PointD
                ListeCoordonnees.CopyTo(PointSurvol, PWeb, 0, NbPointsNouvelleTrace + 1)
                ListeCoordonnees.RemoveRange(PointSurvol + 1, NbPointsNouvelleTrace)
                Dim PAttribut() As Attribut = Nothing
                If IsAttribut Then
                    ReDim PAttribut(NbPointsNouvelleTrace)
                    ListeAttributs.CopyTo(PointSurvol, PAttribut, 0, NbPointsNouvelleTrace + 1)
                    ListeAttributs.RemoveRange(PointSurvol + 1, NbPointsNouvelleTrace)
                End If
                ListeTraces.Add(New TRK(PVirtuels, PWeb, RenvoyerNomTraceUnique(_Nom), _IsAltitude, _IsHoraire, PAttribut))
            End If
        End Sub
        ''' <summary> coupe la trace principale sur le segment et enregistre la partie coupée (du point de coupe à la fin) en tant que trace TRK et l'ajoute dans la collection </summary>
        Private Sub CouperTraceAuSegment()
            Dim NbPointsNouvelleTrace = ListePoints.Count - SegmentSurvol
            Dim PVirtuels(NbPointsNouvelleTrace) As Point
            PVirtuels(0) = PosMouse 'met à jour le point de coupe en coordonnées virtuelles pour la nouvelle trace
            ListePoints.CopyTo(SegmentSurvol, PVirtuels, 1, NbPointsNouvelleTrace)
            ListePoints.RemoveRange(SegmentSurvol, NbPointsNouvelleTrace)
            ListePoints.Add(PVirtuels(0)) 'insert le point de coupe pour la trace existante
            Dim PWeb(NbPointsNouvelleTrace) As PointD
            PWeb(0) = PointPixelToPointGrille(PosMouse, Site, Echelle) 'met à jour le point de coupe en coordonnées Web pour la nouvelle trace
            ListeCoordonnees.CopyTo(SegmentSurvol, PWeb, 1, NbPointsNouvelleTrace)
            ListeCoordonnees.RemoveRange(SegmentSurvol, NbPointsNouvelleTrace)
            ListeCoordonnees.Add(PWeb(0)) 'insert le point de coupe pour la trace existante en coordonnées Web pour la nouvelle trace
            Dim PAttribut() As Attribut = Nothing
            If IsAttribut Then
                ReDim PAttribut(NbPointsNouvelleTrace)
                If IsAltitude Then
                    If Attribut.Altitude = -9999 Then 'si l'altitude FCGP n'est pas correcte lors de l'ajout on la recalcule
                        PAttribut(0).Altitude = CShort((ListeAttributs(SegmentSurvol - 1).Altitude + ListeAttributs(SegmentSurvol).Altitude) / 2)
                    Else
                        PAttribut(0).Altitude = Attribut.Altitude
                    End If
                End If
                If IsHoraire Then 'on recalcule toujours l'horaire car FCGP ne le donne jamais
                    Dim NbSecondes As Integer = CInt((ListeAttributs(SegmentSurvol).Horaire - ListeAttributs(SegmentSurvol - 1).Horaire).TotalSeconds / 2)
                    PAttribut(0).Horaire = ListeAttributs(SegmentSurvol - 1).Horaire + New TimeSpan(0, 0, NbSecondes)
                End If
                ListeAttributs.CopyTo(SegmentSurvol, PAttribut, 1, NbPointsNouvelleTrace)
                ListeAttributs.RemoveRange(SegmentSurvol, NbPointsNouvelleTrace)
                ListeAttributs.Add(PAttribut(0))
            End If
            ListeTraces.Add(New TRK(PVirtuels, PWeb, RenvoyerNomTraceUnique(_Nom), _IsAltitude, _IsHoraire, PAttribut))
        End Sub
        ''' <summary> Lance l'action qui peut être réalisée sur un appui du bouton gauche de la souris </summary>
        ''' <param name="PointMouse"> position de la souris lors de l'appui sur le bouton </param>
        ''' <param name="Attrib"> attribut associé au point </param>
        Friend Function SourisDown(PointMouse As Point, Attrib As Attribut) As Boolean
            PosMouse = PointMouse
            Attribut = Attrib
            RealiserActionSourisDown()
            Return True  'si il y a une action encours on demande le dessin
        End Function
        ''' <summary>réalise l'action sur la trace quand le bouton gauche de la souris est appuyé : Evenement MouseDOWN </summary>
        Private Sub RealiserActionSourisDown()
            IsActionEncours = True ' une action aura forcément lieu
            FlagIsModifie = True 'on indique que la trace est modifié par rapport au chargement
            If Action = Actions.AjouterPointSegment Then
                ListePoints.Insert(SegmentSurvol, PosMouse)
                ListeCoordonnees.Insert(SegmentSurvol, PointPixelToPointGrille(PosMouse, Site, Echelle))
                'fait en sorte que les attributs soient corrects
                If IsAttribut Then
                    If IsAltitude AndAlso Attribut.Altitude = -9999 Then 'si l'altitude renvoyée par FCGP n'est pas bonne on fait la moyenne des 2 points englobant le pt inséré
                        Attribut.Altitude = CShort((ListeAttributs(SegmentSurvol - 1).Altitude + ListeAttributs(SegmentSurvol).Altitude) / 2)
                    End If
                    If IsHoraire Then 'on reclacule l'horaire car FCGP ne renvoie jamais de date
                        Dim NbSecondes As Integer = CInt((ListeAttributs(SegmentSurvol).Horaire - ListeAttributs(SegmentSurvol - 1).Horaire).TotalSeconds / 2)
                        Attribut.Horaire = ListeAttributs(SegmentSurvol - 1).Horaire + New TimeSpan(0, 0, NbSecondes)
                    End If
                    ListeAttributs.Insert(SegmentSurvol, Attribut)
                End If
                'on transforme l'action initiale en Deplacement du pt afin de pouvoir le déplacer tant qu'on ne relève pas le bouton gauche de la souris
                PointSurvol = SegmentSurvol
                Action = Actions.DeplacerPoint
                CurseurEncours = Curseurs.TraceDeplacePt
            ElseIf Action = Actions.AjouterPointFin Then
                If IsAttribut Then
                    If IsAltitude AndAlso Attribut.Altitude = -9999 Then Attribut.Altitude = ListeAttributs(ListeAttributs.Count - 1).Altitude
                    If IsHoraire Then Attribut.Horaire = ListeAttributs(ListeAttributs.Count - 1).Horaire
                    ListeAttributs.Add(Attribut)
                End If
                ListePoints.Add(PosMouse)
                ListeCoordonnees.Add(PointPixelToPointGrille(PosMouse, Site, Echelle))
                'on transforme l'action initiale en Deplacement du pt afin de pouvoir le déplacer tant qu'on ne relève pas le bouton gauche de la souris
                PointSurvol = ListePoints.Count - 1
                Action = Actions.DeplacerPoint
                CurseurEncours = Curseurs.TraceDeplacePt
            End If
        End Sub
        ''' <summary> trouve le mode édition à réaliser quand on appui sur une des 3 touches qui permettent de choisir un mode d'édition </summary>
        Friend Function ToucheDown(Touche As Keys) As Boolean
            If Edition = Editions.Aucune Then
                Select Case Touche
                    Case ToucheAjouteDeplace
                        Edition = Editions.AjouterDeplacer
                        If PointSurvol > -1 Then  'action à réaliser sur l'événementSourisUP
                            Action = Actions.DeplacerPoint
                            CurseurEncours = Curseurs.TraceDeplacePt
                        ElseIf SegmentSurvol > 0 Then  'action à réaliser sur l'événementSourisDOWN
                            Action = Actions.AjouterPointSegment
                            CurseurEncours = Curseurs.TraceInsertPt
                        Else  'action à réaliser sur l'événementSourisDOWN
                            Action = Actions.AjouterPointFin
                            CurseurEncours = Curseurs.TraceInsertPt
                        End If
                    Case ToucheSupprime
                        Edition = Editions.Supprimer
                        CurseurEncours = Curseurs.TraceSupprime
                        'on se contente de définir l'action à réaliser sur l'événementSourisUP
                        If PointSurvol > -1 Then
                            Action = Actions.SupprimerPoint
                            CurseurEncours = Curseurs.TraceSupprimePtSeg
                        ElseIf SegmentSurvol > 0 Then
                            Action = Actions.SuprimerSegment
                            CurseurEncours = Curseurs.TraceSupprimePtSeg
                        End If
                    Case ToucheCoupe
                        Edition = Editions.Couper
                        CurseurEncours = Curseurs.TraceCoupe
                        'on se contente de définir l'action à réaliser sur l'événementSourisUP
                        If PointSurvol > -1 Then
                            Action = Actions.CouperPoint
                            CurseurEncours = Curseurs.TraceCoupePtSeg
                        ElseIf SegmentSurvol > 0 Then
                            Action = Actions.CouperSegment
                            CurseurEncours = Curseurs.TraceCoupePtSeg
                        End If
                End Select
                'on renvoie true si une édition possible
                Return Edition > Editions.Aucune
            End If
            Return False
        End Function
        ''' <summary> Si touche correspond à une des 3 touches qui permettent de choisir un mode d'édition on enlève le mode d'édition </summary>
        Friend Function ToucheUp(Touche As Keys) As Boolean
            If Touche = ToucheAjouteDeplace OrElse Touche = ToucheSupprime OrElse Touche = ToucheCoupe Then
                'on termine l'action en cours en l'annulant au cas ou le bouton gauche de la souris soit encore enfoncé
                IsActionEncours = False
                Action = Actions.Aucune
                'on quitte le mode d'édition encours
                Edition = Editions.Aucune
                CurseurEncours = Curseurs.TraceDefaut
                Return True
            End If
            Return False
        End Function
#End Region
#End Region
    End Class

    '''' <summary> stockage des attributs d'un point de la trace </summary>
    Friend Structure Attribut
        Friend Altitude As Short
        Friend Horaire As Date
        Friend Sub New(Optional alti As Short = 0, Optional Heure As Date = Nothing)
            Altitude = alti
            Horaire = Heure
        End Sub
    End Structure
    Friend Class ResumeTrace
        Friend ReadOnly Index As Integer
        Friend Nom As String
        Friend IsVisible As Boolean
        Friend CouleurSegment As Color
        Friend Sub New(IndexTrace As Integer, NomTrace As String, Visible As Boolean, Couleur As Color)
            Index = IndexTrace
            Nom = NomTrace
            IsVisible = Visible
            CouleurSegment = Couleur
        End Sub
    End Class
End Namespace