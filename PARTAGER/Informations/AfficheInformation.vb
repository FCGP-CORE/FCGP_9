''' <summary> affiche une information sous différentes formes </summary>
Module AfficheInformation
#Region "Procédures et autres partagées"
    ''' <summary> message à afficher dans la fenêtre information. Commun à toutes les boites </summary>
    Friend Property MessageInformation As String
    ''' <summary>titre de la fenêtre information. Commun à toutes les boites </summary>
    Friend Property TitreInformation As String
    ''' <summary> action qui permet le rappel d'AfficherInformation à partir du thread UI </summary>
    Friend Property FormHorsUI As Action
    Friend Sub AfficherInformation()
        If Environment.CurrentManagedThreadId <> ID_Thread_IU Then
            FormApplication.Invoke(_FormHorsUI)
        Else
            Using F As New DialogueInformation With {.Boutons = TypeBoutons.Info_OK}
                F.ShowDialog()
            End Using
        End If
    End Sub
    ''' <summary> sert a indiquer qu'il y a eu une erreur gérée dans l'application appelante </summary>
    Friend Sub AfficherErreur(Ex As Exception, CleProcedureErreur As String)
        'on sauvegarde les messages d'information
        Dim Message = _MessageInformation
        Dim Titre = _TitreInformation
        Dim NomProcedureErreur As String = RenvoyerNomMethodeErreur(CleProcedureErreur)
        _MessageInformation = NomProcedureErreur & CrLf & Ex.Message & CrLf & "Source : " & Ex.Source
        'on indique l'erreur
        _TitreInformation = TitreErreur & TypeFCGP
        AfficherInformation()
        'on restore les messages d'information
        _MessageInformation = Message
        _TitreInformation = Titre
    End Sub
    ''' <summary> Affiche dans une boite de dialogue le message passé en paramètre et permet de cliquer sur un hyperlien </summary>
    ''' <param name="NomLien"> texte à afficher pour le lien </param>
    ''' <param name="UriLien"> addresse du lien </param>
    Friend Sub AfficherInformationEtLien(Parent As Form, NomLien As String, UriLien As String)

        Using F As New DialogueInformation With {.Boutons = TypeBoutons.Info_OK_Mail,
                                                 .NomLien = NomLien,
                                                 .UriLien = UriLien}
            F.ShowDialog(Parent)
        End Using
    End Sub
    ''' <summary> Affiche dans une boite de dialogue le message passé en paramètre et demande une confirmation </summary>
    Friend Function AfficherConfirmation() As DialogResult
        Using F As New DialogueInformation With {.Boutons = TypeBoutons.Info_Oui_Non}
            Return F.ShowDialog()
        End Using
    End Function
    ''' <summary> Affiche dans une boite de dialogue un message passé en paramètre et attend qu'une tache soit finie pour se fermer tout seul </summary>
    ''' <param name="Parent"> formulaire appeleant </param>
    ''' <returns> le dialogresult renvoyé par le tâche </returns>
    Friend Function LancerTache(Tache As Func(Of DialogResult)) As DialogResult
        Using F As New DialogueInformation With {.Boutons = TypeBoutons.Tache, .Tache = Tache}
            Return F.ShowDialog()
        End Using
    End Function
    ''' <summary> affiche le fichier d'aide de l'application appelante </summary>
    ''' <param name="Parent"> Le formulaire appelant </param>
    Friend Sub AfficherAide(Parent As Form)
        Using F As New DialogueAide
            F.ShowDialog(Parent)
        End Using
    End Sub
    ''' <summary> affiche une image avec le centre de l'image au centre de la zone d'affichage.
    ''' L'image peut être déplacée sur l'affichage avec un appui d'un bouton de la souris et déplacement de celle-ci </summary>
    ''' <param name="Parent"> Le formulaire appelant </param>
    ''' <param name="Image"> Image à afficher </param>
    ''' <param name="DimensionsAffichage"> Largeur et hauteur de la zone d'affichage </param>
    Friend Function AfficherImage(Parent As Form, Image As Image, DimensionsAffichage As Size) As DialogResult
        Using F As New DialogueImage With {.ImageCarte = Image, .DimensionsAffichage = DimensionsAffichage}
            Return F.ShowDialog(Parent)
        End Using
    End Function
    Private Enum TypeBoutons
        ''' <summary> Texte d'information + Bouton OK </summary>
        Info_OK = 0
        ''' <summary> Texte d'information + Bouton OK + Bouton servant de lien sur une addresse web </summary>
        Info_OK_Mail = 1
        ''' <summary> Texte d'information + Bouton Oui + Bouton Non </summary>
        Info_Oui_Non = 2
        ''' <summary> Texte d'information sans bouton pour les tâches </summary>
        Tache = 3
    End Enum
#End Region
#Region "DialogueInformation"
    ''' <summary> classe pouvant être remplacer par le taskdialog de windowsforms quand elle gera les hyperliens. Normalement Net 7.0 </summary>
    Private Class DialogueInformation
        Inherits Form
        ''' <summary>largeur minimale de la fenêtre en fonction du ou des boutons demandés </summary>
        Private ReadOnly LargeurMinBoutons() As Integer = New Integer() {90, 195, 170, 0}
        Private StockClipCurseur As Rectangle, _Tache As Func(Of DialogResult)
        Friend Property Boutons As TypeBoutons
        Friend Property UriLien As String
        Friend WriteOnly Property Tache As Func(Of DialogResult)
            Set(value As Func(Of DialogResult))
                _Tache = value
            End Set
        End Property
        Friend Property NomLien As String
        Private Const LargeurTexteInit As Integer = 262 'Panel.Width -6 -6
        Private Const HauteurTexteInit As Integer = 38 'Panel.Height -6 -6
        Private Sub BoiteInformation_Load(sender As Object, e As EventArgs)
            SuspendLayout()
            StockClipCurseur = Cursor.Clip
            Text = TitreInformation
            If Boutons < TypeBoutons.Tache Then
                'il y a toujours un texte d'information
                AfficherInformationBoutons()
                'si il y a des boutons on les affiche
                AfficherBoutons()
            Else
                AfficherInformationTache()
            End If
            'on met à jour les dimensions des différents contrôles
            ResumeLayout(True)
            'on centre par rapport au formulaire de l'application sans passer le formulaire en tant que parent. 
            'Cela permet d'éviter les appels inter threads.  
            Dim V = VisueRectangle
            Dim Pc = New Point(V.X + V.Width \ 2, V.Y + V.Height \ 2)
            Location = New Point(Pc.X - ClientSize.Width \ 2, Pc.Y - ClientSize.Height \ 2)
            'met le curseur sur le centre du bouton OK
            Cursor.Position = New Point(Location.X + B_OK.Location.X + B_OK.Width \ 2 + 7, Location.Y + B_OK.Location.Y + B_OK.Height \ 2 + 31)
            'et limite les déplacements de la souris au formulaire
            Cursor.Clip = New Rectangle(Location, Size)
        End Sub
        ''' <summary> utilise cet évenement pour tester les identifants et fermer automatiquement le formulaire </summary>
        Private Sub DialogueTache_Shown(sender As Object, e As EventArgs)
            'on force le dessin du formulaire car il arrive qu'il n'est pas le temps de le faire avant de fermer
            Refresh()
            'si c'est une tâche on ferme la boite de dialogue au retour de la tache
            If Boutons = TypeBoutons.Tache Then
                DialogResult = _Tache()
                Close()
            End If
            Focus()
        End Sub
        Private Sub BoiteInformation_FormClosed(sender As Object, e As FormClosedEventArgs)
            Cursor.Clip = StockClipCurseur
        End Sub
        Private Sub AfficherInformationBoutons()
            Dim TailleClient As Size = ClientSize
            Dim LargeurTexte As Integer = Information.ClientSize.Width
            Dim Hauteurtexte As Integer = Information.ClientSize.Height
            Dim LargeurTitre As Integer
            Using G As Graphics = Information.CreateGraphics
                LargeurTitre = G.MeasureString(Text, Font).ToSize.Width
            End Using
            Dim LargeurMinFormulaire = Math.Max(LargeurMinBoutons(_Boutons), LargeurTitre)
            Dim LargeurFormulaire = Math.Max(LargeurTexte, LargeurMinFormulaire)
            Dim DeltaLargeur = LargeurFormulaire - LargeurTexteInit
            Dim DeltaHauteur = Hauteurtexte - HauteurTexteInit
            TailleClient = Size.Add(TailleClient, New Size(DeltaLargeur, DeltaHauteur))
            ClientSize = TailleClient
        End Sub
        Private Sub AfficherInformationTache()
            Dim LargeurTexte As Integer = Information.ClientSize.Width
            Dim Hauteurtexte As Integer = Information.ClientSize.Height
            'on supprime la hauteur réservée pour les boutons
            Controls.Remove(LienSite)
            Controls.Remove(B_ESC)
            Controls.Remove(B_OK)
            Information.Location = New Point(10, 10)
            Information.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            Panel1.Dock = DockStyle.Fill
            ClientSize = New Size(LargeurTexte + 20, Hauteurtexte + 20)
        End Sub
        ''' <summary> Affiche le ou les boutons concernés et vérifie que la largeur du formulaire permet de les voir </summary>
        Private Sub AfficherBoutons()
            Select Case Boutons
                Case TypeBoutons.Info_OK
                    CancelButton = B_OK
                    B_ESC.Visible = False
                    LienSite.Visible = False
                Case TypeBoutons.Info_Oui_Non
                    B_OK.Text = "Oui"
                    LienSite.Visible = False
                Case TypeBoutons.Info_OK_Mail
                    CancelButton = B_OK
                    B_ESC.Visible = False
                    LienSite.Text = NomLien
            End Select
        End Sub
        ''' <summary> ouvre le navigateur par défaut avec le lien du site de FCGP</summary>
        Private Sub Info_2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
            LienSite.LinkVisited = True
            Dim psi = New ProcessStartInfo With
                {
                    .FileName = UriLien,
                    .UseShellExecute = True
                }
            Process.Start(psi)
            Close()
        End Sub
    End Class
    Partial Private Class DialogueInformation
        Inherits Form
        Friend Sub New()
            InitializeComponent()
            InitialiserEvenements()
        End Sub
        Private Sub InitialiserEvenements()
            AddHandler LienSite.LinkClicked, AddressOf Info_2_LinkClicked
            AddHandler Load, AddressOf BoiteInformation_Load
            AddHandler Shown, AddressOf DialogueTache_Shown
            AddHandler FormClosed, AddressOf BoiteInformation_FormClosed
        End Sub
        Private Sub InitializeComponent()
            B_OK = New Button()
            B_ESC = New Button()
            LienSite = New LinkLabel()
            Information = New Label()
            Panel1 = New Panel()
            Panel1.SuspendLayout()
            SuspendLayout()
            '
            'B_OK
            '
            B_OK.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            B_OK.DialogResult = DialogResult.OK
            B_OK.Location = New Point(193, 56)
            B_OK.Name = "B_OK"
            B_OK.Size = New Size(75, 30)
            B_OK.TabIndex = 1
            B_OK.Text = "Fermer"
            B_OK.UseVisualStyleBackColor = True
            '
            'B_ESC
            '
            B_ESC.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            B_ESC.DialogResult = DialogResult.Cancel
            B_ESC.Location = New Point(112, 56)
            B_ESC.Name = "B_ESC"
            B_ESC.Size = New Size(75, 30)
            B_ESC.TabIndex = 2
            B_ESC.Text = "Non"
            B_ESC.UseVisualStyleBackColor = True
            '
            'LienSite
            '
            LienSite.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            LienSite.Location = New Point(6, 56)
            LienSite.Name = "LienSite"
            LienSite.Size = New Size(100, 30)
            LienSite.TabIndex = 3
            LienSite.TabStop = True
            LienSite.Text = ""
            LienSite.TextAlign = ContentAlignment.MiddleLeft
            '
            'Information
            '
            Information.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            Information.Location = New Point(6, 6)
            Information.Padding = New Padding(0)
            Information.Margin = New Padding(0)
            Information.Name = "Information"
            Information.AutoSize = True
            Information.Text = MessageInformation
            Information.TabIndex = 4
            '
            'Panel1
            '
            Panel1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            Panel1.BackColor = Color.FromArgb(247, 247, 247)
            Panel1.Controls.Add(Information)
            Panel1.Location = New Point(0, 0)
            Panel1.Name = "Panel1"
            Panel1.Size = New Size(274, 50)
            Panel1.TabIndex = 5
            '
            'BoiteInformation
            '
            AutoScaleMode = AutoScaleMode.None
            BackColor = SystemColors.Control
            ClientSize = New Size(274, 92)
            ControlBox = False
            Controls.Add(Panel1)
            Controls.Add(LienSite)
            Controls.Add(B_ESC)
            Controls.Add(B_OK)
            Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
            FormBorderStyle = FormBorderStyle.FixedToolWindow
            MaximizeBox = False
            MinimizeBox = False
            Name = "BoiteInformation"
            ShowIcon = False
            ShowInTaskbar = False
            SizeGripStyle = SizeGripStyle.Hide
            StartPosition = FormStartPosition.Manual
            TopMost = True
            AcceptButton = B_OK
            CancelButton = B_ESC
            TopMost = True
            Panel1.ResumeLayout(False)
            ResumeLayout(False)
        End Sub

        Private B_OK As Button
        Private B_ESC As Button
        Private LienSite As LinkLabel
        Private Information As Label
        Private Panel1 As Panel
    End Class
#End Region
#Region "DialogueAide"
    ''' <summary> affiche un formulaire d'aide concernant l'utilisation de l'application appelante </summary>
    Private Class DialogueAide
        Inherits Form
        Private StockClipCurseur As Rectangle
        ''' <summary> charge le fichier d'aide </summary>
        Private Sub FormAide_Load(sender As Object, e As EventArgs)
            StockClipCurseur = Cursor.Clip
            Dim DonneesAide As Byte() = CType(ResourceManager.GetObject("Aide_FCGP_" & TypeFCGP, Culture), Byte())
            Using MS As New MemoryStream(DonneesAide)
                Aide.LoadFile(MS, RichTextBoxStreamType.RichText)
            End Using
            Aide.RightMargin = ClientSize.Width - 37
            Cursor.Clip = New Rectangle(Location, Size)
        End Sub
        Private Sub FormAide_FormClosed(sender As Object, e As FormClosedEventArgs)
            Cursor.Clip = StockClipCurseur
        End Sub
        ''' <summary> lance un navigateur avec l'adresse du site sélectionnée </summary>
        Private Sub Aide_LinkClicked(sender As Object, e As LinkClickedEventArgs)
            Dim psi = New ProcessStartInfo With
            {
                .FileName = e.LinkText,
                .UseShellExecute = True
            }
            Process.Start(psi)
        End Sub
    End Class
    Partial Private Class DialogueAide
        Inherits Form
        Friend Sub New()
            InitializeComponent()
            InitialiserEvenements()
        End Sub
        Private Sub InitialiserEvenements()
            AddHandler Aide.LinkClicked, AddressOf Aide_LinkClicked
            AddHandler Load, AddressOf FormAide_Load
            AddHandler FormClosed, AddressOf FormAide_FormClosed
        End Sub

        'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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
        Private Sub InitializeComponent()
            components = New Container()
            Fond = New Panel()
            Aide = New RichTextBox()
            Fermer = New Button()
            Infos = New ToolTip(components)
            SuspendLayout()
            '
            'Aide
            '
            Aide.Dock = DockStyle.Fill
            Aide.BackColor = Color.Linen
            Aide.BorderStyle = BorderStyle.None
            Aide.Name = "Aide"
            Aide.ReadOnly = True
            Aide.ScrollBars = RichTextBoxScrollBars.Vertical
            Aide.TabIndex = 0
            Aide.TabStop = False
            Aide.Text = ""
            '
            'Fond
            '
            Fond.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            Fond.BackColor = Color.Linen
            Fond.Location = New Point(0, 0)
            Fond.Name = "Fond"
            Fond.Size = New Size(798, 557)
            Fond.Padding = New Padding(10, 10, 0, 0)
            Fond.TabIndex = 4
            Fond.Controls.Add(Aide)
            '
            'Fermer
            '
            Fermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            Fermer.BackColor = SystemColors.Control
            Fermer.DialogResult = DialogResult.Cancel
            Fermer.Location = New Point(717, 563)
            Fermer.Name = "Fermer"
            Fermer.TextAlign = ContentAlignment.MiddleCenter
            Fermer.Size = New Size(75, 30)
            Fermer.TabIndex = 3
            Fermer.Text = "Fermer"
            Infos.SetToolTip(Fermer, "Ferme le formulaire d'aide")
            Fermer.UseVisualStyleBackColor = True
            '
            'FormAide
            '
            AcceptButton = Fermer
            AutoScaleMode = AutoScaleMode.None
            BackColor = SystemColors.Control
            CancelButton = Fermer
            ClientSize = New Size(798, 599)
            ControlBox = False
            Controls.Add(Fond)
            Controls.Add(Fermer)
            Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
            FormBorderStyle = FormBorderStyle.FixedToolWindow
            MaximizeBox = False
            Name = "FormAide"
            ShowIcon = False
            SizeGripStyle = SizeGripStyle.Hide
            StartPosition = FormStartPosition.CenterParent
            TopMost = True
            Text = Nothing
            ResumeLayout(False)
        End Sub

        Private Fermer As Button
        Private Infos As ToolTip
        Private Fond As Panel
        Private Aide As RichTextBox
    End Class
#End Region
#Region "DialogueImage"
    ''' <summary> affiche un formulaire qui affiche une image et permet de la déplacer sur la surface de celui-ci </summary>
    Private Class DialogueImage
        Inherits Form
        ''' <summary> coordonnées en pixels du coin haut gauche de l'image par rapport à la zone d'affichage </summary>
        Private Pt0 As Point
        ''' <summary> Largeur et hauteur (carré) de la zone d'affichage de l'image </summary>
        Friend Property DimensionsAffichage As Size
        ''' <summary> image à afficher </summary>
        Friend Property ImageCarte As Image
        ''' <summary> positionnement de l'image sur le formulaire d'affichage. Permet le scrolling de l'image
        ''' juste en faisant varier Pt0 </summary>
        Private ReadOnly Property RegionImage As Rectangle
            Get
                Return New Rectangle(Pt0, ImageCarte.Size)
            End Get
        End Property
        ''' <summary> bornes à ne pas dépasser pour Pt0 </summary>
        Private BorneMiniX, BorneMaxiX, BorneMiniY, BorneMaxiY As Integer
        ''' <summary> indique qu'un bouton de souris est appuié </summary>
        Private FlagMouseDown As Boolean
        ''' <summary> position du curseur de souris lors du dernier événement de souris </summary>
        Private PtMouse As Point
        ''' <summary> limitation sur le déplacement du curseur de souris du formulaire appelant </summary>
        Private StockClipCurseur As Rectangle
        Private FlagDeplacement As Boolean
        ''' <summary> initialise les différentes varaibles et la psotion du formulaire </summary>
        Private Sub DialogueImage_Load(sender As Object, e As EventArgs)
            StockClipCurseur = Cursor.Clip
            FlagMouseDown = False
            BorneMiniX = -ImageCarte.Width + 10
            BorneMaxiX = _DimensionsAffichage.Width - 10
            BorneMiniY = -ImageCarte.Height + 10
            BorneMaxiY = _DimensionsAffichage.Height - 10
            Pt0 = New Point((_DimensionsAffichage.Width - ImageCarte.Width) \ 2, (_DimensionsAffichage.Height - ImageCarte.Height) \ 2)
            ClientSize = If(_DimensionsAffichage.Width = 0, New Size(700, 700 + Info.Height), New Size(_DimensionsAffichage.Width, _DimensionsAffichage.Height + Info.Height))
            'centre le formulaire sur le formulaire parent
            If Owner Is Nothing Then
                Dim V = VisueRectangle
                Dim Pc = New Point(V.X + V.Width \ 2, V.Y + V.Height \ 2)
                Location = New Point(Pc.X - ClientSize.Width \ 2, Pc.Y - ClientSize.Height \ 2)
            Else
                With Owner
                    Location = New Point(.Location.X + (.Width - Width) \ 2, .Location.Y + (.Height - Height) \ 2)
                End With
            End If
            Cursor.Position = New Point(Location.X + _DimensionsAffichage.Width \ 2, Location.Y + _DimensionsAffichage.Height \ 2)
            Dim RegionAffichage = New Rectangle(Point.Empty, Affichage.ClientSize)
            FlagDeplacement = Not RegionAffichage.Contains(RegionImage)
            'et limite les déplacements de la souris au formulaire
            Cursor.Clip = New Rectangle(Location, Size)
        End Sub
        ''' <summary> replace les limitations de déplacement du curseur de souris du formulaire appeleant </summary>
        Private Sub DialogueImage_FormClosed(sender As Object, e As FormClosedEventArgs)
            Cursor.Clip = StockClipCurseur
        End Sub
        ''' <summary> Ferme le formulaire sur Escape ou Enter en différencier le dialogresult du formulaire </summary>
        Private Sub DialogueImage_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                DialogResult = DialogResult.Cancel
                Close()
            End If
            If e.KeyCode = Keys.Enter Then
                DialogResult = DialogResult.OK
                Close()
            End If
        End Sub
        ''' <summary> enregistre le fait d'appuyer sur un bouton et l'emplacement de la souris </summary>
        Private Sub Affichage_MouseDown(sender As Object, e As MouseEventArgs)
            FlagMouseDown = True
            PtMouse = e.Location
        End Sub
        ''' <summary> calcule le delta de déplacement de la souris et modifie le PT0 de l'image en conséquence </summary>
        Private Sub Affichage_MouseMove(sender As Object, e As MouseEventArgs)
            If FlagMouseDown AndAlso FlagDeplacement Then
                Dim X = Pt0.X - PtMouse.X + e.X
                Dim Y = Pt0.Y - PtMouse.Y + e.Y
                If X < BorneMiniX Then
                    Pt0.X = BorneMiniX
                ElseIf X > BorneMaxiX Then
                    Pt0.X = BorneMaxiX
                Else
                    Pt0.X = X
                End If
                If Y < BorneMiniY Then
                    Pt0.Y = BorneMiniY
                ElseIf Y > BorneMaxiY Then
                    Pt0.Y = BorneMaxiY
                Else
                    Pt0.Y = Y
                End If
                Affichage.Invalidate()
                PtMouse = e.Location
            End If
        End Sub
        ''' <summary> enregistre le fait de relacher le bouton de la souris </summary>
        Private Sub Affichage_MouseUp(sender As Object, e As MouseEventArgs)
            FlagMouseDown = False
        End Sub
        ''' <summary> dessinne l'image à l'endroit spécifié sur la zone d'affichage </summary>
        Private Sub Affichage_Paint(sender As Object, e As PaintEventArgs)
            e.Graphics.DrawImage(ImageCarte, RegionImage)
        End Sub
    End Class
    Partial Private Class DialogueImage
        Inherits Form

        Sub New()
            InitializeComponent()
            InitialiserEvenements()
        End Sub

        Private Sub InitialiserEvenements()
            AddHandler Load, AddressOf DialogueImage_Load
            AddHandler FormClosed, AddressOf DialogueImage_FormClosed
            AddHandler KeyDown, AddressOf DialogueImage_KeyDown
            AddHandler Affichage.Paint, AddressOf Affichage_Paint
            AddHandler Affichage.MouseDown, AddressOf Affichage_MouseDown
            AddHandler Affichage.MouseMove, AddressOf Affichage_MouseMove
            AddHandler Affichage.MouseUp, AddressOf Affichage_MouseUp
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
#Disable Warning IDE0044 ' Ajouter un modificateur readonly
        Private components As IContainer
#Enable Warning IDE0044 ' Ajouter un modificateur readonly

        'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
        'Ne la modifiez pas à l'aide de l'éditeur de code.
        Private Sub InitializeComponent()
            Affichage = New PictureBox()
            Info = New Label()
            CType(Affichage, ISupportInitialize).BeginInit()
            SuspendLayout()
            '
            'Affichage
            '
            Affichage.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            Affichage.BorderStyle = BorderStyle.None
            Affichage.Location = New Point(0, 0)
            Affichage.Margin = New Padding(0)
            Affichage.Name = "Affichage"
            Affichage.Size = New Size(700, 700)
            Affichage.TabIndex = 0
            Affichage.TabStop = False
            '
            'Info
            '
            Info.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            Info.BorderStyle = BorderStyle.FixedSingle
            Info.FlatStyle = FlatStyle.Flat
            Info.Location = New Point(-1, 701)
            Info.Name = "Info"
            Info.Size = New Size(702, 23)
            Info.TabIndex = 1
            Info.Font = New Font("Segoe UI", 14.0!, FontStyle.Bold, GraphicsUnit.Pixel)
            Info.Text = "Click pour déplacer Image, Esc ou Enter pour fermer"
            Info.TextAlign = ContentAlignment.MiddleCenter
            '
            'AfficheImage
            '
            AutoScaleMode = AutoScaleMode.None
            ClientSize = New Size(700, 723)
            ControlBox = False
            Controls.Add(Info)
            Controls.Add(Affichage)
            Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
            FormBorderStyle = FormBorderStyle.FixedToolWindow
            MaximizeBox = False
            MinimizeBox = False
            Name = "AfficheImage"
            ShowIcon = False
            ShowInTaskbar = False
            CType(Affichage, ISupportInitialize).EndInit()
            ResumeLayout(False)
        End Sub

        Private Affichage As PictureBox
        Private Info As Label
    End Class
#End Region
End Module