''' <summary> variables et fonctions globales aux classes et modules de FCGP Capturer </summary>
Module CapturerGlobal
#Region "Variables globales"
    ''' <summary> font servant pour l'écriture des étiquettes des points ou de l'échelle sur la visue </summary>
    Friend FontEtiquette As Font
    ''' <summary> gestion de l'affichage des tuiles </summary>
    Friend Affichage As AffichageCarte
    ''' <summary> gestion de l'obtention des tuiles </summary>
    Friend Serveur As ServeurCarto
    ''' <summary> conteneur pour les variables qui permettent le dessin d'une échelle sur l'affichage </summary>
    Friend EchelleDessin As DessinEchelle
    ''' <summary> flag indiquant si le site et niveau de détail encours permettent l'affichage du dessin de l'échelle </summary>
    Friend IsDessinEchelle As Boolean
    ''' <summary> définition du système cartographique encours et qui servira pour le téléchargement des cartes </summary>
    Friend SysCartoEncours As SystemeCartographique
    ''' <summary> liste des différents coefs alpha possibles de la couche pentes </summary>
    Friend ReadOnly CoefsAlphasPentes As Single() = New Single() {0.15, 0.33, 0.5, 0.66, 0.8, 1}
    ''' <summary> coef alpha de la couche des pentes sous forme de matrice GDI pour le dessin de la couche </summary>
    Friend ImageAttributsPentes As ImageAttributes
    ''' <summary> flag indiquant qu'un remplissage de tampon a été demandé </summary>
    Friend FlagRemplirTampon As Boolean
    ''' <summary> flag limitant les déplacements et la difinition des points aux limites du site de capture </summary>
    Friend FlagLimitesSite As Boolean = True
    ''' <summary> Flag indiquant que la couche des pentes est à afficher sur la couche de fond de plan </summary>
    Friend ReadOnly Property IsAffichagePentes As Boolean
        Get
            Return Affichage.CoucheFondIspentes AndAlso CapturerSettings.IS_AFFICHE_PENTES
        End Get
    End Property
#End Region
    ''' <summary> ouvre un formulaire de saisie de coordonnées en fonction du type de coordonnées qui est sélectionné </summary>
    ''' <param name="IndexUT"> type de coordonnées sélectionné </param>
    ''' <param name="PointCoordonnees"> coordonnées saisies </param>
    ''' <param name="Parent"> Formulaire à partir duquel l'action est lancée </param>
    ''' <param name="Position"> position en pixel de la fenêtre parente. Si différent de Point.empty la position du formulaire sera manuel, sinon centrée sur le parent </param>
    ''' <returns> Ok ou Abort </returns>
    Friend Function OuvrirFormulaireSaisieCoordonnees(Parent As Form, Position As Point, IndexUT As Integer, ByRef PointCoordonnees As PointD) As DialogResult
        Dim R As Form = Nothing, FlagDD As Boolean
        Select Case IndexUT
            Case 0
                R = New SaisieGrille
            Case 1
                FlagDD = True
                R = New SaisieDD
            Case 2
                FlagDD = True
                R = New SaisieDMS
            Case 3
                FlagDD = True
                R = New SaisieUTM
            Case 4
                R = New SaisieTuile
        End Select
        'si l'appelant le désire, il peut positionner le formulaire de saisie des coordonnées là où il le souhaite
        If Position <> Point.Empty Then
            R.StartPosition = FormStartPosition.Manual
            R.Location = Position
        End If
        If FlagDD AndAlso Not PointCoordonnees.IsEmpty Then
            PointCoordonnees = PointGrilleToPointDD(PointCoordonnees, SysCartoEncours.Projection.Datum)
        End If
        R.Tag = PointCoordonnees
        Dim Ret = R.ShowDialog(Parent)
        If Ret = DialogResult.OK Then
            PointCoordonnees = CType(R.Tag, PointD)
            If FlagDD Then PointCoordonnees = PointDDToPointGrille(PointCoordonnees, SysCartoEncours.Projection.Datum)
        End If
        R.Dispose()
        Return Ret
    End Function
    ''' <summary> conteneur pour le dessin des échelles qui est composé par 4 segments de droite(3 Verticaux et 1 horizontal)
    ''' et une étiquette indiquant la distance représentée par le segment horizontal. dessinné en bas à droite de l'affichage 
    ''' attention pour les sites WebMercator l'échelle n'est pas valable pour l'axe des Y </summary>
    Friend Structure DessinEchelle
        Friend TexteEchelle As String
        Friend Pt1, Pt2, Pt3, Pt4 As Point
        Friend RectangleEtiquette As Rectangle
        Friend Sub New(ByRef IsDessinEchelle As Boolean)
            If Not Affichage.CentreAffichage.IsEmpty Then
                Dim NiveauDetail = SysCartoEncours.Niveau
                'on vérifie que le niveau de détail permet l'affichage d'une échelle (équivalent à Grille)
                If NiveauDetail.PasGrilleNumeric = 0 Then
                    IsDessinEchelle = False
                    Exit Sub
                End If
                'initialisation des différentes variables
                Dim Echelle = NiveauDetail.Echelle
                Dim LargeurVisue = VisueRectangle.Size.Width
                Dim HauteurVisue = VisueRectangle.Size.Height
                Dim LongueurEchelle As Integer
                If Serveur.Datum = Datums.Web_Mercator Then
                    'la grille WebMercator ne peut pas servir pour des calculs de distance, il faut passer par des conversions en grille UTM ou Lambert.
                    'UTM couvre le monde entier alors que Lambert est plus précis mais local à la France Métropolitaine
                    Dim Site = SysCartoEncours.SiteCarto
                    Dim PDD As PointD = PointPixelsToPointDD(Affichage.CentreAffichage, Site, Echelle)
                    Dim PP As PointProjection = ConvertWGS84ToProjection(PDD, Datums.UTM_WGS84)
                    PP.X += NiveauDetail.PasGrilleNumeric
                    PDD = ConvertProjectionToWGS84(Datums.UTM_WGS84, PP)
                    Dim CentreDecale As Point = PointDDToPointPixels(PDD, Site, Echelle)
                    LongueurEchelle = CentreDecale.X - Affichage.CentreAffichage.X
                Else
                    'SuisseMobile est le seul site géré par FCGP qui a une grille qui ne soit pas WebMercator
                    LongueurEchelle = CInt(NiveauDetail.PasGrilleNumeric * PixelsMetre(Echelle))
                End If
                TexteEchelle = NiveauDetail.PasGrilleTexte
                Using G As Graphics = FormApplication.CreateGraphics
                    Dim S = G.MeasureString(TexteEchelle, FontEtiquette).ToSize
                    Pt2 = New Point(LargeurVisue - 10, HauteurVisue - 10)
                    Pt1 = New Point(Pt2.X - LongueurEchelle, HauteurVisue - 10)
                    Pt3 = New Point(Pt2.X - LongueurEchelle \ 2, HauteurVisue - 10)
                    Pt4 = New Point((Pt1.X + (LongueurEchelle - S.Width) \ 2), Pt1.Y - S.Height - 15)
                    RectangleEtiquette = New Rectangle(Pt4, S)
                End Using
                IsDessinEchelle = True
            End If
        End Sub
    End Structure
    ''' <summary> assume que certaines touche d'édition sont admises </summary>
    Friend Function SuppressionTouche(TypeSaisie As TypeSaisie, Touche As Keys) As Boolean
        Dim Ret As Boolean = True
        Select Case Touche
            'Touches d'édition. Commun à tous les types de saisie
            Case Keys.Back, Keys.Delete, Keys.Left, Keys.Right, Keys.Tab
                Ret = False
            'Touches des digits. Commun à tous les types de saisie
            Case Keys.NumPad0 To Keys.NumPad9, Keys.D0 To Keys.D9
                Ret = False
            'Touches moins et décimal
            Case Keys.Decimal, Keys.OemPeriod
                If TypeSaisie > TypeSaisie.EntierNegatif Then
                    Ret = False
                End If
            Case Keys.Subtract
                If TypeSaisie = TypeSaisie.EntierNegatif OrElse TypeSaisie = TypeSaisie.DecimalNegatif Then
                    Ret = False
                End If
        End Select
        Return Ret
    End Function
    Friend Function SuppressionCaractere(Saisie As TypeSaisie, C As TextBox, Caractere As Char) As Boolean
        Dim Ret As Boolean = False
        'on interdit la saisie du 1er caractère si celui existant est le signe -, il faut l'effacer en premier
        If C.SelectionStart = 0 And C.Text.StartsWith("-") Then
            Ret = True
        Else
            Select Case Caractere
                Case ";"c 'on interdit le point virgule mais c'est la même touche que le . décimal
                    Ret = True
                Case "."c
                    If Saisie > TypeSaisie.EntierNegatif Then
                        'on interdit d'avoir plus d'un caractère . dans le texte
                        If C.Text.Contains("."c) Then
                            Ret = True
                        End If
                    Else
                        'on interdit d'avoir le caractère . dans le texte
                        Ret = True
                    End If
                Case "-"c 'on interdit d'avoir plus d'un caractère - dans le texte
                    If Saisie = TypeSaisie.EntierNegatif OrElse Saisie = TypeSaisie.DecimalNegatif Then
                        'on interdit plus d'un caractère - dans le texte
                        If C.Text.Contains("-"c) Then
                            Ret = True
                            'on interdit que le caractère - si il n'est pas le premier
                        ElseIf C.SelectionStart <> 0 Then
                            Ret = True
                        End If
                    Else
                        'on interdit d'avoir le caractère - dans le texte
                        Ret = True
                    End If
                Case "0"c To "9"c
                    Ret = False
                Case Back
                    Ret = False
                Case Else
                    Ret = True
            End Select
        End If
        Return Ret
    End Function
    Friend Function FormaterDecimal(Texte As String) As String
        Dim Ret As String = Texte
        If Texte.StartsWith(".") OrElse Texte.StartsWith("-.") Then
            Ret = Texte.Replace(".", "0.")
        End If
        Return Ret
    End Function
    Friend Enum TypeSaisie
        EntierPositif = 0 : EntierNegatif : DecimalPositif : DecimalNegatif
    End Enum
End Module
