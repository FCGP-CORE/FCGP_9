Partial Friend Class Carte
#Region "Interpole carte sans grille"
    ''' <summary> interpolation linéaire sur les latitudes pour les cartes WebMercator </summary>
    Private Function InterpolerCarteSansGrille(CarteInterpole As Carte) As ErreurInterpolation
        InterpolerCarteSansGrille = ErreurInterpolation.NotOk
        NbTotalIterations = _RegionVirtuelle.Height
        NbIterations = 0
        AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
        Try
            'initialisation des variables globales externes à boucle de calcul
            'calcul des latitudes de départ et de fin. On repart des coordonnées virtuelles de la carte source pour plus de précision. Elles sont communes aux cartes source et interpolée
            Dim z_LatitudeOrigine As Double = SystemCartographique.ConvertirCoordonneesVirtuellesToReelles(_RegionVirtuelle.Location).Y
            Dim z_LatitudeFin As Double = SystemCartographique.ConvertirCoordonneesVirtuellesToReelles(Point.Add(_RegionVirtuelle.Location, _RegionVirtuelle.Size)).Y
            'pas des DD par pixel interpolé qui est forcément négatif
            Dim z_PasLatitudeInterpole As Double = (z_LatitudeFin - z_LatitudeOrigine) / _RegionVirtuelle.Height
            'variables de parcours de la boucle ou des boucles
            Dim HauteurSourceLue As Integer, HauteurInterpoleEcrite As Integer, NbLignesInterpole As Integer
            If TamponType = TypeSupportCarte.Fichier Then
                Using FichierSource As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                    CarteInterpole.CheminFluxLecture = CheminEnregistrementProvisoire & "\CarteInterpole.raw"
                    Using FichierInterpole As New FileStream(CarteInterpole.CheminFluxLecture, FileMode.Create, FileAccess.Write, FileShare.None)
                        Do
                            'calcul des variables de parcours de la boucle
                            NbLignesInterpole = CalculerNbLignesInterpole(z_LatitudeOrigine, z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue)
                            Dim NbLignesSource As Integer = CalculerNbLignesSource(z_LatitudeOrigine, z_PasLatitudeInterpole,
                                                                                   HauteurInterpoleEcrite + NbLignesInterpole - 1, HauteurSourceLue)
                            'remplir le tampon source avec les données de la carte source à partir de la position du pointeur de lecture du fichier source qui correspond à HauteurSourceLue
                            FichierSource.Read(Tampon.Bits, Tampon.IndexBits, NbLignesSource * LargeurOctets)
                            'on interpole la partie de la carte correspondant à la boucle encours de HauteurInterpoleEcrite à HauteurInterpoleEcrite + NbLignesInterpole - 1
#If DEBUG And Not PARALLEL Then
                            For LigneInterpole As Integer = 0 To NbLignesInterpole - 1 'Boucle sans éxecution en parallele
                                InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine, 
                                                               z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue)
                            Next
#Else
                            Parallel.For(0, NbLignesInterpole, Sub(LigneInterpole As Integer) 'Boucle avec éxecution en parallele
                                                                   InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine,
                                                                   z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue)
                                                               End Sub)
#End If
                            'on enregistre la partie de la carte interpolée
                            FichierInterpole.Write(CarteInterpole.Tampon.Bits, CarteInterpole.Tampon.IndexBits, NbLignesInterpole * LargeurOctets)
                            'on met à jour la variable correspondante
                            HauteurInterpoleEcrite += NbLignesInterpole
                            'vérification nb de lignes effectivement lues qui peut être faussé par la précision des nb doubles avec positionnement du pointeur de lecture du fichier source
                            HauteurSourceLue = VerifierHauteurSource(z_LatitudeOrigine, z_PasLatitudeInterpole, HauteurInterpoleEcrite,
                                                                            HauteurSourceLue + NbLignesSource - 1, FichierSource)
                        Loop While HauteurInterpoleEcrite < _RegionVirtuelle.Height
                    End Using
                End Using
            Else
                NbLignesInterpole = _RegionVirtuelle.Height
#If DEBUG And Not PARALLEL Then
                For LigneInterpole As Integer = 0 To NbLignesInterpole - 1 'Boucle sans éxecution en parallele
                    InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine, 
                                                   z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue)
                Next
#Else
                Parallel.For(0, NbLignesInterpole, Sub(LigneInterpole As Integer) 'Boucle avec éxecution en parallele
                                                       InterpolerLigneCarteSansGrille(CarteInterpole.Tampon, LigneInterpole, z_LatitudeOrigine,
                                                       z_PasLatitudeInterpole, HauteurInterpoleEcrite, HauteurSourceLue)
                                                   End Sub)
#End If
            End If
            InterpolerCarteSansGrille = ErreurInterpolation.Ok
        Catch Ex As Exception
            AfficherErreur(Ex, "P8A8")
        End Try
    End Function
    ''' <summary> calcul le nb maxi de lignes à interpoler en fonction de la hauteur du tampon des cartes par pas de 2.5% de la hauteur du tampon </summary>
    ''' <param name="z_LatitudeOrigine"> Latitude de départ de la carte </param>
    ''' <param name="z_PasLatitudeInterpole"> le pas de latitude par pixels de la carte interpolée </param>
    ''' <param name="HauteurInterpoleEcrite"> la hauteur déjà écrite de la carte interpolée </param>
    ''' <param name="HauteurSourceLue"> la hauteur lue de la carte source correspondant à la carte déjà interpolée </param>
    Private Function CalculerNbLignesInterpole(z_LatitudeOrigine As Double, z_PasLatitudeInterpole As Double, HauteurInterpoleEcrite As Integer, HauteurSourceLue As Integer) As Integer
        If HauteurReserve > _HauteurPixel Then
            'si le tampon est supèrieur à la carte source il n'y a qu'une seule tranche.
            'L'égalité provoque une erreur sur le Nb de lignes de la carte source à lire quand la hauteur de carte = la hauteur d'une tuile fichier
            'Exemple 4*256 tuile serveur et 1024 tuile KMZ ==> CalculerNbLignesInterpole = 1024, NbLignesSource = 1025
            CalculerNbLignesInterpole = _RegionVirtuelle.Height
        Else
            'initialisation des variables avant la boucle de calcul
            Dim DeltaTampon As Integer = CInt(HauteurReserve * 0.025R)
            Dim LatitudeDebutTranche As Double = z_LatitudeOrigine + HauteurInterpoleEcrite * z_PasLatitudeInterpole
            Dim NbLignesInterpole As Integer = HauteurReserve + DeltaTampon 'on ajoute 2.5% en initialisation avant la boucle
            Dim HauteurSource As Integer
            Do
                NbLignesInterpole -= DeltaTampon 'à chaque tour on diminue le nb de lignes interpolées de 2.5% de la hauteur du tampon
                Dim LatitudeFinTranche As Double = LatitudeDebutTranche + NbLignesInterpole * z_PasLatitudeInterpole
                HauteurSource = SystemCartographique.ConvertirCoordonneesReellesToVirtuelles(New PointD(0, LatitudeFinTranche)).Y - _RegionVirtuelle.Y - HauteurSourceLue
            Loop Until HauteurSource < HauteurReserve
            If HauteurInterpoleEcrite + NbLignesInterpole < _RegionVirtuelle.Height Then
                CalculerNbLignesInterpole = NbLignesInterpole
            Else
                CalculerNbLignesInterpole = _RegionVirtuelle.Height - HauteurInterpoleEcrite
            End If
        End If
    End Function
    ''' <summary> calcul le nb de lignes de la carte source nécessaires pour l'interpolation du nb de lignes à interpoler 
    ''' part du principe que la latitude de HauteurSourceLue est supérieur à la latitude de LatitudeDébutTranche de la carte interpolée </summary>
    ''' <param name="z_LatitudeOrigine"> Latitude de départ de la carte </param>
    ''' <param name="z_PasLatitudeInterpole"> le pas de ltitude par pixels de la carte interpolée </param>
    ''' <param name="HauteurInterpole"> la hauteur déjà écrite de la carte interpolée + nombre de ligne à interpoler de la tranche suivante -1 </param>
    ''' <param name="HauteurSourceLue"> la hauteur lue de la carte source correspondant à la carte déjà interpolée </param>
    Private Function CalculerNbLignesSource(z_LatitudeOrigine As Double, z_PasLatitudeInterpole As Double, HauteurInterpole As Integer, HauteurSourceLue As Integer) As Integer
        Dim LatitudeFinTranche As Double = z_LatitudeOrigine + HauteurInterpole * z_PasLatitudeInterpole
        CalculerNbLignesSource = CInt(Math.Floor(LatitudeWebMercatorToPixel(LatitudeFinTranche, SystemCartographique.SiteCarto,
                                                 SystemCartographique.Niveau.Echelle))) + 2 - _RegionVirtuelle.Y - HauteurSourceLue
    End Function
    ''' <summary> Verifie que la latitude de HauteurSource est > à la latitude de HauteurInterpoleEcrite de la carte interpolée 
    ''' Dans tous les cas corrige le pointeur de lecture du fichier source soit de 1 ligne en arrière soit de 2 lignes </summary>
    ''' <param name="z_LatitudeOrigine">latitude de départ pour l'interpolation. Commun carte source, carte interpolée</param>
    ''' <param name="z_PasLatitudeInterpole">pas ou au coef moyen DD par pixels interpolé</param>
    ''' <param name="HauteurSource"> la hauteur lue de la carte source correspondant à la carte déjà interpolée plus le nb de ligne à lire de la tranche en cours - 1 </param>
    ''' <param name="HauteurInterpoleEcrite">nb de ligne qui ont déjà été écrite dans le fichier interpolé</param>
    ''' <param name="FichierSource"> fichier source </param>
    Private Function VerifierHauteurSource(z_LatitudeOrigine As Double, z_PasLatitudeInterpole As Double, HauteurInterpoleEcrite As Integer,
                                           HauteurSource As Integer, FichierSource As FileStream) As Integer
        Dim LatitudeDebutTrancheInterpole As Double = z_LatitudeOrigine + (HauteurInterpoleEcrite) * z_PasLatitudeInterpole
        Dim LatitudeDebutTrancheSource As Double = SystemCartographique.ConvertirCoordonneesVirtuellesToReelles(New Point(0, HauteurSource + _RegionVirtuelle.Y)).Y
        If LatitudeDebutTrancheSource < LatitudeDebutTrancheInterpole Then
            VerifierHauteurSource = HauteurSource - 1
            'pour tenir compte que l'on recule d'une ligne en plus on se positionne au début de l'avant dernière ligne
            FichierSource.Position -= LargeurOctets * 2
        Else
            VerifierHauteurSource = HauteurSource
            'pour tenir compte de la dernière ligne car elle est obligatoire pour l'interpolation de la tranche suivante on se positionne au début de la dernière ligne
            FichierSource.Position -= LargeurOctets
        End If
    End Function
    ''' <summary> interpole une ligne de la carte la carte inteoplée à partir de 2 lignes de la carte source </summary>
    ''' <param name="TamponInterpole"> Tampon de la Carte à interpoler où sont enregistrés les résultats de l'interpolation </param>
    ''' <param name="CptLigneInterpole"> numéro de ligne à interpoler (de 0 à hauteurpixel-1 ou 0 à nblignesinterpole-1 si il y a plus d'une tranche </param>
    ''' <param name="z_LatitudeInterpole">latitude du coin(0) en DD</param>
    ''' <param name="z_PasLatitudeInterpole">pas de la latitude pour 1 pixel de la carte interpolée (en DD)</param>
    ''' <param name="HauteurInterpoleEcrite"> combien de lignes de la carte interpolée ont déja été traitées et écrites </param>
    ''' <param name="HauteurSourceLue"> combien de lignes de la carte source ont déja été lue</param>
    Private Sub InterpolerLigneCarteSansGrille(TamponInterpole As EditableBitmap, CptLigneInterpole As Integer, z_LatitudeInterpole As Double,
                                               z_PasLatitudeInterpole As Double, HauteurInterpoleEcrite As Integer, HauteurSourceLue As Integer)
        'adresse du 1er pixel dans le tampon de la carte interpolée
        Dim IndexInterpole As Integer = CptLigneInterpole * _LargeurOctets + TamponInterpole.IndexBits
        'il faut trouver la ligne du pixel de la latitude source en coordonnées du monde virtuel correspondant à la latitude interpolée en cours. Pour cela on calcule la latitude interpolée
        Dim LigneInterpole As Integer = HauteurInterpoleEcrite + CptLigneInterpole
        Dim EcartLatitudeInterpole As Double = LigneInterpole * z_PasLatitudeInterpole
        'on trouve le pixel avec la fonction qui va bien mais on le ramène au début du tampon qui contient tout ou partie de la carte source
        Dim PoidsL2 As Double, YSource As Integer
        If LigneInterpole = 0 Then
            YSource = 0
            PoidsL2 = 0 'pour la 1ere ligne de la carte interpolée le poids du pixel de la ligne YSource+1 est null
        Else
            PoidsL2 = LatitudeWebMercatorToPixel(z_LatitudeInterpole + EcartLatitudeInterpole, SystemCartographique.SiteCarto, SystemCartographique.Niveau.Echelle) - _RegionVirtuelle.Y
            YSource = CInt(Math.Floor(PoidsL2))
            If LigneInterpole = _RegionVirtuelle.Height - 1 Then
                PoidsL2 = 0 ' pour la derniere ligne de la carte interpolée le poids du pixel de la ligne YSource+1 est null
            Else
                PoidsL2 -= YSource 'poids du pixel ligne YSource+1
            End If
        End If
        Dim IndexSourceL1 = (YSource - HauteurSourceLue) * _LargeurOctets + Tampon.IndexBits
        Dim IndexSourceL2 = IndexSourceL1 + _LargeurOctets
        Dim L1, L2 As Double
        For CptX As Integer = 0 To _LargeurOctets - 1
            With Tampon 'de la carte source
                'interpolation de la couleur. à ce niveau on ne regarde que les bytes(8 bits par couleur)
                If .IsBitmap Then 'si la carte vient d'une image enregistrée il faut les trouver à partir de la mémoire non managée d'où l'utilisation de Marshal et des pointeurs
                    If PoidsL2 = 0D Then
                        TamponInterpole.Bits(IndexInterpole) = Marshal.ReadByte(.BitPtr, IndexSourceL1)
                    Else
                        L1 = Marshal.ReadByte(.BitPtr, IndexSourceL1)
                        L2 = Marshal.ReadByte(.BitPtr, IndexSourceL2)
                        TamponInterpole.Bits(IndexInterpole) = CByte(Math.Round(L1 + (L2 - L1) * PoidsL2))
                    End If
                Else
                    If PoidsL2 = 0D Then
                        TamponInterpole.Bits(IndexInterpole) = .Bits(IndexSourceL1)
                    Else
                        L1 = .Bits(IndexSourceL1)
                        L2 = .Bits(IndexSourceL2)
                        TamponInterpole.Bits(IndexInterpole) = CByte(Math.Round(L1 + (L2 - L1) * PoidsL2))
                    End If
                End If
            End With
            IndexSourceL1 += 1
            IndexSourceL2 += 1
            IndexInterpole += 1
        Next
        Interlocked.Increment(NbIterations)
        If NbIterations Mod 100 = 0 Then AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
    End Sub
#End Region
End Class