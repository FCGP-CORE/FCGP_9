Partial Class FichierTuilesKMZ
    Inherits Form

    Friend Sub New()
        InitializeComponent()
        InitialiserEvenements()
    End Sub
    Private Sub InitialiserEvenements()
        'contrôle simples
        AddHandler CorLargeur.ValueChanged, AddressOf CorDimension_ValueChanged
        AddHandler CorLargeur.MouseDoubleClick, AddressOf CorDimension_MouseDoubleClick
        AddHandler CorHauteur.ValueChanged, AddressOf CorDimension_ValueChanged
        AddHandler CorHauteur.MouseDoubleClick, AddressOf CorDimension_MouseDoubleClick
        AddHandler CorPT0X.ValueChanged, AddressOf CorLocation_ValueChanged
        AddHandler CorPT0X.MouseDoubleClick, AddressOf CorLocation_MouseDoubleClick
        AddHandler CorPT0Y.ValueChanged, AddressOf CorLocation_ValueChanged
        AddHandler CorPT0Y.MouseDoubleClick, AddressOf CorLocation_MouseDoubleClick
        AddHandler N512.ValueChanged, AddressOf N512_ValueChanged
        AddHandler Lancer.Click, AddressOf Lancer_Click
        AddHandler ChoixTailleTampon.SelectedIndexChanged, AddressOf ChoixTailleTampon_SelectedIndexChanged
        'formulaire
        AddHandler Load, AddressOf FichierTuilesKMZ_Load
        AddHandler FormClosed, AddressOf FichierTuilesKMZ_FormClosed
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

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    Private Sub InitializeComponent()
#Region "Constructeurs"
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(FichierTuilesKMZ))
        ToolTip1 = New ToolTip(components)
        CorLargeur = New NumericUpDown()
        CorHauteur = New NumericUpDown()
        CorPT0X = New NumericUpDown()
        CorPT0Y = New NumericUpDown()
        PT2Reel = New Label()
        PT0Reel = New Label()
        N512 = New NumericUpDown()
        NbTuilesKMZ = New Label()
        Lancer = New Button()
        Quitter = New Button()
        NomCarte = New TextBox()
        NbRangs = New Label()
        NbCols = New Label()
        LPT0X = New Label()
        LPT0Y = New Label()
        LLARGEUR = New Label()
        LHAUTEUR = New Label()
        L011 = New Label()
        L018 = New Label()
        L014 = New Label()
        LargeurMaxi = New Label()
        HauteurMaxi = New Label()
        L015 = New Label()
        L029 = New Label()
        L028 = New Label()
        L027 = New Label()
        L024 = New Label()
        PT0X = New Label()
        L012 = New Label()
        PT0Y = New Label()
        L013 = New Label()
        L017 = New Label()
        L016 = New Label()
        L030 = New Label()
        L025 = New Label()
        L026 = New Label()
        ChoixTailleTampon = New ComboBox()
        Information = New Label()
        Support = New Panel
        CType(CorLargeur, ISupportInitialize).BeginInit()
        CType(CorHauteur, ISupportInitialize).BeginInit()
        CType(CorPT0X, ISupportInitialize).BeginInit()
        CType(CorPT0Y, ISupportInitialize).BeginInit()
        Support.SuspendLayout()
        SuspendLayout()
#End Region
#Region "Dimensions Fichier"
#Region "Entêtes"
        '
        'L011
        '
        L011.BackColor = Color.Transparent
        L011.BorderStyle = BorderStyle.FixedSingle
        L011.Location = New Point(6, 6) '-183
        L011.Name = "L011"
        L011.Size = New Size(480, 23)
        L011.TabIndex = 13
        L011.Text = "Taille et position du fichier KMZ"
        L011.TextAlign = ContentAlignment.MiddleCenter
#End Region
#Region "Pt0X"
        '
        'L012
        '
        L012.BackColor = Color.Transparent
        L012.BorderStyle = BorderStyle.FixedSingle
        L012.Location = New Point(6, 28)
        L012.Name = "L012"
        L012.Size = New Size(241, 24)
        L012.TabIndex = 6
        L012.Text = "PT0 X  en pixel du fichier"
        L012.TextAlign = ContentAlignment.MiddleCenter
        '
        'LPT0X
        '
        LPT0X.BackColor = Color.Transparent
        LPT0X.BorderStyle = BorderStyle.FixedSingle
        LPT0X.Location = New Point(246, 28)
        LPT0X.Name = "LPT0X"
        LPT0X.Size = New Size(121, 24)
        LPT0X.TabIndex = 6
        LPT0X.TextAlign = ContentAlignment.TopCenter
        'PT0X
        '
        PT0X.BackColor = Color.Transparent
        PT0X.BorderStyle = BorderStyle.FixedSingle
        PT0X.Location = New Point(365, 28)
        PT0X.Margin = New Padding(0)
        PT0X.Name = "PT0X"
        PT0X.Size = New Size(121, 24)
        PT0X.TabIndex = 17
        '
        'CorPT0X
        '
        CorPT0X.BackColor = Color.White
        CorPT0X.BorderStyle = BorderStyle.None
        CorPT0X.ForeColor = Color.YellowGreen
        CorPT0X.Location = New Point(366, 29)
        CorPT0X.Name = "CorPT0X"
        CorPT0X.Size = New Size(119, 23)
        CorPT0X.TabIndex = 7
        CorPT0X.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorPT0X, resources.GetString("CorPT0X.ToolTip"))
#End Region
#Region "Pt0Y"
        '
        'L013
        '
        L013.BackColor = Color.Transparent
        L013.BorderStyle = BorderStyle.FixedSingle
        L013.Location = New Point(6, 51)
        L013.Name = "L013"
        L013.Size = New Size(241, 24)
        L013.TabIndex = 6
        L013.Text = "PT0 Y en pixel du fichier"
        L013.TextAlign = ContentAlignment.MiddleCenter
        '
        'LPT0Y
        '
        LPT0Y.BackColor = Color.Transparent
        LPT0Y.BorderStyle = BorderStyle.FixedSingle
        LPT0Y.Location = New Point(246, 51)
        LPT0Y.Name = "LPT0Y"
        LPT0Y.Size = New Size(121, 24)
        LPT0Y.TabIndex = 8
        LPT0Y.TextAlign = ContentAlignment.TopCenter
        '
        'PT0Y
        '
        PT0Y.BackColor = Color.Transparent
        PT0Y.BorderStyle = BorderStyle.FixedSingle
        PT0Y.Location = New Point(365, 51)
        PT0Y.Margin = New Padding(0)
        PT0Y.Name = "PT0Y"
        PT0Y.Size = New Size(121, 24)
        PT0Y.TabIndex = 18
        '
        'CorPT0Y
        '
        CorPT0Y.BackColor = Color.White
        CorPT0Y.BorderStyle = BorderStyle.None
        CorPT0Y.ForeColor = Color.YellowGreen
        CorPT0Y.Location = New Point(366, 52)
        CorPT0Y.Name = "CorPT0Y"
        CorPT0Y.Size = New Size(119, 23)
        CorPT0Y.TabIndex = 7
        CorPT0Y.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorPT0Y, resources.GetString("CorPT0Y.ToolTip"))
#End Region
#Region "Largeur"
        '
        'L014
        '
        L014.BackColor = Color.Transparent
        L014.BorderStyle = BorderStyle.FixedSingle
        L014.Location = New Point(6, 74)
        L014.Name = "L014"
        L014.Size = New Size(241, 24)
        L014.TabIndex = 6
        L014.Text = "Largeur en pixel du fichier"
        L014.TextAlign = ContentAlignment.MiddleCenter
        '
        'LLARGEUR
        '
        LLARGEUR.BackColor = Color.Transparent
        LLARGEUR.BorderStyle = BorderStyle.FixedSingle
        LLARGEUR.Location = New Point(246, 74)
        LLARGEUR.Name = "LLARGEUR"
        LLARGEUR.Size = New Size(121, 24)
        LLARGEUR.TabIndex = 9
        LLARGEUR.TextAlign = ContentAlignment.TopCenter
        '
        'LargeurMaxi
        '
        LargeurMaxi.BackColor = Color.Transparent
        LargeurMaxi.BorderStyle = BorderStyle.FixedSingle
        LargeurMaxi.Location = New Point(365, 74)
        LargeurMaxi.Margin = New Padding(0)
        LargeurMaxi.Name = "LargeurMaxi"
        LargeurMaxi.Size = New Size(121, 24)
        LargeurMaxi.TabIndex = 15

        '
        'CorLargeur
        '
        CorLargeur.BorderStyle = BorderStyle.None
        CorLargeur.ForeColor = Color.YellowGreen
        CorLargeur.Location = New Point(366, 75)
        CorLargeur.Name = "CorLargeur"
        CorLargeur.Size = New Size(119, 23)
        CorLargeur.TabIndex = 7
        CorLargeur.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorLargeur, resources.GetString("CorLargeur.ToolTip"))
#End Region
#Region "Hauteur"
        '
        'L015
        '
        L015.BackColor = Color.Transparent
        L015.BorderStyle = BorderStyle.FixedSingle
        L015.Location = New Point(6, 97)
        L015.Name = "L015"
        L015.Size = New Size(241, 24)
        L015.TabIndex = 6
        L015.Text = "Hauteur en pixel du fichier"
        L015.TextAlign = ContentAlignment.MiddleCenter
        '
        'LHAUTEUR
        '
        LHAUTEUR.BorderStyle = BorderStyle.FixedSingle
        LHAUTEUR.Location = New Point(246, 97)
        LHAUTEUR.Name = "LHAUTEUR"
        LHAUTEUR.Size = New Size(121, 24)
        LHAUTEUR.TabIndex = 10
        LHAUTEUR.TextAlign = ContentAlignment.TopCenter
        '
        'HauteurMaxi
        '
        HauteurMaxi.BackColor = Color.Transparent
        HauteurMaxi.BorderStyle = BorderStyle.FixedSingle
        HauteurMaxi.Location = New Point(365, 97)
        HauteurMaxi.Margin = New Padding(0)
        HauteurMaxi.Name = "HauteurMaxi"
        HauteurMaxi.Size = New Size(121, 24)
        HauteurMaxi.TabIndex = 16
        '
        'CorHauteur
        '
        CorHauteur.BorderStyle = BorderStyle.None
        CorHauteur.ForeColor = Color.YellowGreen
        CorHauteur.Location = New Point(366, 98)
        CorHauteur.Name = "CorHauteur"
        CorHauteur.Size = New Size(119, 23)
        CorHauteur.TabIndex = 7
        CorHauteur.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(CorHauteur, resources.GetString("CorHauteur.ToolTip"))
#End Region
#Region "Dimensions Réelles corrigées"
        '
        'L016
        '
        L016.BackColor = Color.Transparent
        L016.BorderStyle = BorderStyle.FixedSingle
        L016.Location = New Point(6, 120)
        L016.Name = "L016"
        L016.Size = New Size(120, 23)
        L016.TabIndex = 34
        L016.Text = "PT0 Réel corrigé"
        L016.TextAlign = ContentAlignment.MiddleCenter
        '
        'PT0Reel
        '
        PT0Reel.BackColor = Color.Transparent
        PT0Reel.BorderStyle = BorderStyle.FixedSingle
        PT0Reel.Location = New Point(125, 120)
        PT0Reel.Name = "PT0Reel"
        PT0Reel.Size = New Size(361, 23)
        PT0Reel.TabIndex = 32
        PT0Reel.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(PT0Reel, "Coin Nord Ouest de la sélection résultant des corrections" & CrLf &
                                     "apportées en coordonnées réelles de géoréférencement.")
        '
        'L017
        '
        L017.BackColor = Color.Transparent
        L017.BorderStyle = BorderStyle.FixedSingle
        L017.Location = New Point(6, 142)
        L017.Name = "L017"
        L017.Size = New Size(120, 23)
        L017.TabIndex = 33
        L017.Text = "PT2 Réel corrigé"
        L017.TextAlign = ContentAlignment.MiddleCenter
        '
        'PT2Reel
        '
        PT2Reel.BackColor = Color.Transparent
        PT2Reel.BorderStyle = BorderStyle.FixedSingle
        PT2Reel.Location = New Point(125, 142)
        PT2Reel.Name = "PT2Reel"
        PT2Reel.Size = New Size(361, 23)
        PT2Reel.TabIndex = 8
        PT2Reel.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(PT2Reel, "Coin Sud Est de la sélection résultant des corrections" & CrLf &
                                     "apportées en coordonnées réelles de géoréférencement.")
#End Region
#End Region
#Region "Création Fichier"
#Region "entête"
        '
        'L018
        '
        L018.BackColor = Color.Transparent
        L018.BorderStyle = BorderStyle.FixedSingle
        L018.Location = New Point(6, 170)
        L018.Name = "L018"
        L018.Size = New Size(480, 23)
        L018.TabIndex = 14
        L018.Text = "Création du fichier KMZ"
        L018.TextAlign = ContentAlignment.MiddleCenter
#End Region
#Region "Ligne Nom"
        '
        'L024
        '
        L024.BackColor = Color.Transparent
        L024.BorderStyle = BorderStyle.FixedSingle
        L024.Location = New Point(6, 192)
        L024.Name = "L024"
        L024.Size = New Size(360, 21)
        L024.TabIndex = 30
        L024.Text = "Nom"
        L024.TextAlign = ContentAlignment.MiddleCenter
        '
        'L025
        '
        L025.BackColor = Color.Transparent
        L025.BorderStyle = BorderStyle.FixedSingle
        L025.Location = New Point(365, 192)
        L025.Name = "L025"
        L025.Size = New Size(121, 21)
        L025.TabIndex = 35
        L025.Text = "Dimensions Tuiles"
        L025.TextAlign = ContentAlignment.MiddleCenter
        '
        'NomCarte
        '
        NomCarte.BackColor = SystemColors.Window
        NomCarte.Location = New Point(6, 212)
        NomCarte.Name = "NomCarte"
        NomCarte.Size = New Size(360, 24)
        NomCarte.TabIndex = 39
        ToolTip1.SetToolTip(NomCarte, "Nom des fichiers tuiles")
        '
        'L026
        '
        L026.BorderStyle = BorderStyle.FixedSingle
        L026.Location = New Point(365, 212)
        L026.Name = "L026"
        L026.Size = New Size(121, 24)
        L026.TabIndex = 38
        '
        'N512
        '
        N512.BorderStyle = BorderStyle.None
        N512.BackColor = Color.White
        N512.Location = New Point(366, 213)
        N512.Increment = 512D
        N512.Maximum = 1024D
        N512.Minimum = 512D
        N512.Value = 512D
        N512.ReadOnly = True
        N512.Name = "N256"
        N512.Size = New Size(119, 23)
        N512.TabIndex = 7
        N512.TextAlign = HorizontalAlignment.Center
        ToolTip1.SetToolTip(N512, resources.GetString("N512.ToolTip"))
#End Region
#Region "Ligne Taille"
        '
        'L027
        '
        L027.BackColor = Color.Transparent
        L027.BorderStyle = BorderStyle.FixedSingle
        L027.Location = New Point(6, 235)
        L027.Name = "L027"
        L027.Size = New Size(120, 21)
        L027.TabIndex = 6
        L027.Text = "Nb Tuiles"
        L027.TextAlign = ContentAlignment.MiddleCenter
        '
        'L028
        '
        L028.BackColor = Color.Transparent
        L028.BorderStyle = BorderStyle.FixedSingle
        L028.Location = New Point(125, 235)
        L028.Name = "L028"
        L028.Size = New Size(121, 21)
        L028.TabIndex = 10
        L028.Text = "Nb Colonnes"
        L028.TextAlign = ContentAlignment.MiddleCenter
        '
        'L029
        '
        L029.BackColor = Color.Transparent
        L029.BorderStyle = BorderStyle.FixedSingle
        L029.Location = New Point(245, 235)
        L029.Name = "L029"
        L029.Size = New Size(121, 21)
        L029.TabIndex = 12
        L029.Text = "Nb Rangées"
        L029.TextAlign = ContentAlignment.MiddleCenter

        '
        'L030
        '
        L030.BackColor = Color.Transparent
        L030.BorderStyle = BorderStyle.FixedSingle
        L030.Location = New Point(365, 235)
        L030.Name = "L030"
        L030.Size = New Size(121, 21)
        L030.TabIndex = 7
        L030.Text = "Tampon Support"
        L030.TextAlign = ContentAlignment.MiddleCenter

        '
        'Poids
        '
        NbTuilesKMZ.BackColor = Color.YellowGreen
        NbTuilesKMZ.BorderStyle = BorderStyle.FixedSingle
        NbTuilesKMZ.ForeColor = Color.Black
        NbTuilesKMZ.Location = New Point(6, 255)
        NbTuilesKMZ.Name = "Poids"
        NbTuilesKMZ.Size = New Size(120, 23)
        NbTuilesKMZ.TabIndex = 6
        NbTuilesKMZ.Text = "0"
        NbTuilesKMZ.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbTuilesKMZ, "Vert      --> OK pour le poids" & CrLf &
                                         "Rouge  --> le poids total est trop important")
        '
        'NbColonnes
        '
        NbCols.BackColor = Color.Transparent
        NbCols.BorderStyle = BorderStyle.FixedSingle
        NbCols.Location = New Point(125, 255)
        NbCols.Name = "NbColonnes"
        NbCols.Size = New Size(121, 23)
        NbCols.TabIndex = 11
        NbCols.Text = "0"
        NbCols.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbCols, "Vert     --> OK pour le poids et le nb de tuiles." & CrLf &
                                    "Rouge --> le poids total est trop important")
        '
        'NbRangees
        '
        NbRangs.BackColor = Color.Transparent
        NbRangs.BorderStyle = BorderStyle.FixedSingle
        NbRangs.Location = New Point(245, 255)
        NbRangs.Name = "NbRangees"
        NbRangs.Size = New Size(121, 23)
        NbRangs.TabIndex = 8
        NbRangs.Text = "0"
        NbRangs.TextAlign = ContentAlignment.MiddleCenter
        ToolTip1.SetToolTip(NbRangs, "Vert     --> OK le nb de tuiles maxi sur l'axe des X n'est pas dépassé." & CrLf &
                                     "Rouge --> le nb de tuiles maxi sur l'axe des X est dépassé.")

        '
        'ChoixTailleTampon
        '
        ChoixTailleTampon.FlatStyle = FlatStyle.System
        ChoixTailleTampon.ItemHeight = 19
        ChoixTailleTampon.Location = New Point(365, 251)
        ChoixTailleTampon.Margin = New Padding(0)
        ChoixTailleTampon.MaxDropDownItems = 3
        ChoixTailleTampon.Name = "ChoixTailleTampon"
        ChoixTailleTampon.Size = New Size(121, 26)
        ChoixTailleTampon.TabIndex = 9
        ToolTip1.SetToolTip(ChoixTailleTampon, "Influe sur le nb de tuiles maximum sur l'axe des X. " & CrLf &
                                               "Influe également sur la rapidité du traitement.")

#End Region
#End Region
#Region "Controles formulaire"
        '
        'Information
        '
        Information.BackColor = Color.Transparent
        Information.BorderStyle = BorderStyle.FixedSingle
        Information.Location = New Point(6, 277)
        Information.Name = "Information"
        Information.Size = New Size(480, 63)
        Information.TabIndex = 40
        '
        'Support
        '
        Support.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        Support.BackColor = Color.FromArgb(247, 247, 247)
        Support.Controls.Add(L030)
        Support.Controls.Add(ChoixTailleTampon)
        Support.Controls.Add(NbRangs)
        Support.Controls.Add(NbCols)
        Support.Controls.Add(L027)
        Support.Controls.Add(L029)
        Support.Controls.Add(Information)
        Support.Controls.Add(NbTuilesKMZ)
        Support.Controls.Add(L028)
        Support.Controls.Add(NomCarte)
        Support.Controls.Add(N512)
        Support.Controls.Add(L026)
        Support.Controls.Add(L025)
        Support.Controls.Add(L016)
        Support.Controls.Add(L017)
        Support.Controls.Add(PT0Reel)
        Support.Controls.Add(PT2Reel)
        Support.Controls.Add(L012)
        Support.Controls.Add(LPT0X)
        Support.Controls.Add(CorPT0X)
        Support.Controls.Add(PT0X)
        Support.Controls.Add(L013)
        Support.Controls.Add(LPT0Y)
        Support.Controls.Add(CorPT0Y)
        Support.Controls.Add(PT0Y)
        Support.Controls.Add(L014)
        Support.Controls.Add(LLARGEUR)
        Support.Controls.Add(CorLargeur)
        Support.Controls.Add(LargeurMaxi)
        Support.Controls.Add(L015)
        Support.Controls.Add(LHAUTEUR)
        Support.Controls.Add(CorHauteur)
        Support.Controls.Add(HauteurMaxi)
        Support.Controls.Add(L024)
        Support.Controls.Add(L018)
        Support.Controls.Add(L011)
        Support.Location = New Point(0, 0)
        Support.Name = "Support"
        Support.Size = New Size(492, 346)
        Support.TabIndex = 0
        '
        'Lancer
        '
        Lancer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Lancer.Location = New Point(359, 352)
        Lancer.Name = "Lancer"
        Lancer.Size = New Size(124, 33)
        Lancer.TabIndex = 25
        Lancer.Text = "Lancer la création"
        ToolTip1.SetToolTip(Lancer, "Lance la création des fichiers tuiles demandés avec la configuration choisie.")
        Lancer.UseVisualStyleBackColor = True
        '
        'Quitter
        '
        Quitter.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Quitter.DialogResult = DialogResult.Cancel
        Quitter.Location = New Point(229, 352)
        Quitter.Name = "Quitter"
        Quitter.Size = New Size(124, 33)
        Quitter.TabIndex = 26
        Quitter.Text = "Quitter"
        ToolTip1.SetToolTip(Quitter, "Ferme le formulaire sans la génération des fichiers tuiles")
        Quitter.UseVisualStyleBackColor = True
        '
        'FichierTuilesKMZ
        '
        AutoScaleMode = AutoScaleMode.None
        CancelButton = Quitter
        ClientSize = New Size(492, 391)
        ControlBox = False
        Controls.Add(Support)
        Controls.Add(Quitter)
        Controls.Add(Lancer)
        Font = New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "FichierTuilesKMZ"
        StartPosition = FormStartPosition.CenterParent
        Text = "Création des Fichiers KMZ"
        CType(CorLargeur, ISupportInitialize).EndInit()
        CType(CorHauteur, ISupportInitialize).EndInit()
        CType(CorPT0X, ISupportInitialize).EndInit()
        CType(CorPT0Y, ISupportInitialize).EndInit()
        Support.ResumeLayout()
        ResumeLayout(False)
        PerformLayout()
#End Region
    End Sub
#Region "Déclaration"
    Private ToolTip1 As ToolTip
    Private L011 As Label
    Private L018 As Label
    Private L014 As Label
    Private CorLargeur As NumericUpDown
    Private LargeurMaxi As Label
    Private HauteurMaxi As Label
    Private L015 As Label
    Private CorHauteur As NumericUpDown
    Private L027 As Label
    Private NbTuilesKMZ As Label
    Private Lancer As Button
    Private Quitter As Button
    Private L024 As Label
    Private PT0X As Label
    Private L012 As Label
    Private CorPT0X As NumericUpDown
    Private PT0Y As Label
    Private L013 As Label
    Private CorPT0Y As NumericUpDown
    Private PT2Reel As Label
    Private PT0Reel As Label
    Private L017 As Label
    Private L016 As Label
    Private L025 As Label
    Private N512 As NumericUpDown
    Private L026 As Label
    Private NomCarte As TextBox
    Private Information As Label
    Private NbRangs As Label
    Private L029 As Label
    Private L028 As Label
    Private NbCols As Label
    Private LLARGEUR As Label
    Private LHAUTEUR As Label
    Private LPT0X As Label
    Private LPT0Y As Label
    Private L030 As Label
    Private ChoixTailleTampon As ComboBox
    Private Support As Panel
#End Region
End Class
