Partial Friend Class Carte
#Region "Interpole Carte Avec Grille"
    ''' <summary> interpolation bilineaire avec correction de l'erreur sur les Y (carte qui ont déjà une grille) </summary>
    ''' <remarks>la largeur et la hauteur en pixels de la carte interpolée sont les mêmes que celles de la carte source
    ''' En coordonnées on prend le rectangle DD max de la carte source </remarks>
    Private Function InterpolerCarteAvecGrille(CarteInterpole As Carte) As ErreurInterpolation
        InterpolerCarteAvecGrille = ErreurInterpolation.NotOk
        '1 pixel de la couleur de carte par défaut
        PixelInterpol = New Byte() {PartagerSettings.COULEUR_CARTE.B, PartagerSettings.COULEUR_CARTE.G, PartagerSettings.COULEUR_CARTE.R}
        NbTotalIterations = _HauteurPixel
        NbIterations = 0
        AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
        Dim z_ As ICAG = InitialiserICAG(CarteInterpole) 'structure spécifique pour éviter la transmission de tout un tas de paramètres
        If z_ Is Nothing Then Return InterpolerCarteAvecGrille 'si La structure est vide c'est qu'il y a un problème
        'la hauteur du tampon source est au plus = à la hauteur de la carte -1 à cause de l'interpolation qui demande 2 lignes de pixels pour être réalisée
        Dim HauteurTamponSource As Integer = If(HauteurReserve >= CarteInterpole._HauteurPixel, CarteInterpole._HauteurPixel - 1, HauteurReserve - 1)
        Dim HauteurEcrite As Integer, TailleTamponInterpole As Integer, HauteurTamponInterpole As Integer, DebutSource As Integer
        Dim TailleTamponSource As Integer = _LargeurOctets * (HauteurTamponSource + 1) 'la taille du tampon est fixe . On charge 1 ligne en plus pour permettre l'interpolation
        Try
            If CarteInterpole.TamponType = TypeSupportCarte.Fichier Then 'Il y aura normalement plusieurs tranches de travail
                Using FichierBIN As New FileStream(CheminFluxLecture, FileMode.Open, FileAccess.Read)
                    CarteInterpole.CheminFluxLecture = CheminEnregistrementProvisoire & "\CarteInterpole.raw"
                    Using FichierInterpole As New FileStream(CarteInterpole.CheminFluxLecture, FileMode.Create, FileAccess.Write)
                        Do
                            HauteurTamponInterpole = TrouverHauteurInterpole(z_, HauteurTamponSource, FichierBIN) - HauteurEcrite
                            TailleTamponInterpole = _LargeurOctets * HauteurTamponInterpole
                            FichierBIN.Read(Tampon.Bits, Tampon.IndexBits, TailleTamponSource) 'on charge une partie de la carte sans grille dans le tampon
                            'on interpole une tranche de la carte
#If DEBUG And Not PARALLEL Then
                            For Cpty As Integer = 0 To HauteurTamponInterpole - 1 'Boucle sans éxecution en parallele
                                InterpolerLigneCarteAvecGrille(HauteurEcrite, Cpty, DebutSource, z_)
                            Next 'sans la TPL
#Else
                            Parallel.For(0, HauteurTamponInterpole, Sub(Cpty As Integer) InterpolerLigneCarteAvecGrille(HauteurEcrite, Cpty, DebutSource, z_))
#End If
                            HauteurEcrite += HauteurTamponInterpole
                            DebutSource = z_.DebutSource
                            'on enregistre la partie de la carte interpolée
                            FichierInterpole.Write(CarteInterpole.Tampon.Bits, CarteInterpole.Tampon.IndexBits, TailleTamponInterpole)
                        Loop While HauteurEcrite < _HauteurPixel
                    End Using
                End Using
            Else
                'interpolation bilinéaire de la carte : pour chaque ligne de pixels sur la hauteur de l'image interpolée,
                'il faut trouver l'interpolation de la couleur des pixels composant la ligne
#If DEBUG And Not PARALLEL Then
                For Cpty As Integer = 0 To HauteurPixel - 1 'Boucle sans éxecution en parallele
                    InterpolerLigneCarteAvecGrille(HauteurEcrite, Cpty, DebutSource, z_)
                Next
#Else
                Parallel.For(0, _HauteurPixel, Sub(CptY) InterpolerLigneCarteAvecGrille(HauteurEcrite, CptY, DebutSource, z_))
#End If
            End If
            InterpolerCarteAvecGrille = ErreurInterpolation.Ok
        Catch Ex As Exception
            AfficherErreur(Ex, "S4E2")
        End Try
    End Function
    ''' <summary> calcule le N° de ligne interpolée et la latitude source minimale pour cette ligne interpolée</summary>
    ''' <param name="z_"> structure de passage d'informatio n</param>
    ''' <param name="HauteurTamponSource"> hauteur du tampon de la carte source en nb de lignes de pixel </param>
    ''' <param name="FichierBin"> fichier qui contient les données binaire de la carte source </param>
    ''' <returns> HauteurInterpole </returns>
    Private Function TrouverHauteurInterpole(z_ As ICAG, HauteurTamponSource As Integer, FichierBin As FileStream) As Integer
        Dim HauteurSource As Integer = HauteurTamponSource + z_.DebutSource - 1 'correspond à la ligne de la latitude maxi de la tranche précédente
        FichierBin.Position = CLng(z_.DebutSource) * _LargeurOctets 'on positionne le pointeur de fichier pour être prêt à la prochaine lecture
        z_.TrouverIndexLatMaxInterpole(HauteurSource) 'une fois qu'on a trouvé l'index supérieur de la ligne interpolée
        Dim ValeurLat0 As Double = z_.CoordY(z_.IndexLatMax, z_.IndexLigneInterpole - 1)
        Dim ValeurLat1 As Double = z_.CoordY(z_.IndexLatMax, z_.IndexLigneInterpole)
        If z_.DeltaLatMax > 0 Then
            ValeurLat0 += (z_.CoordY(z_.IndexLatMax + 1, z_.IndexLigneInterpole - 1) - ValeurLat0) * z_.DeltaLatMax
            ValeurLat1 += (z_.CoordY(z_.IndexLatMax + 1, z_.IndexLigneInterpole) - ValeurLat1) * z_.DeltaLatMax
        End If
        'on peut calculer la ligne interpolée par interpolation des 2 latitudes qui encadrent hauteursource
        Dim LigneInterpoleFinTranche As Double = CInt(Math.Truncate(z_.IndexLigneInterpole * z_.PasY - z_.PasY * (ValeurLat1 - HauteurSource) / (ValeurLat1 - ValeurLat0)))
        'mais on ne doit pas dépasser la hauteur de l'image
        If LigneInterpoleFinTranche >= _HauteurPixel Then LigneInterpoleFinTranche = _HauteurPixel - 1
        Dim DeltaLigneInterpole As Double = LigneInterpoleFinTranche / z_.PasY
        DeltaLigneInterpole -= CInt(Math.Truncate(DeltaLigneInterpole))
        'maintenant que l'on a la ligne interpolée on peut calculer la latitude mini correspondante pour le début de la prochaine tranche
        ValeurLat0 = z_.CoordY(z_.IndexLatMin, z_.IndexLigneInterpole - 1)
        ValeurLat1 = z_.CoordY(z_.IndexLatMin, z_.IndexLigneInterpole)
        If z_.DeltaLatMin > 0 Then
            ValeurLat0 += (z_.CoordY(z_.IndexLatMin + 1, z_.IndexLigneInterpole - 1) - ValeurLat0) * z_.DeltaLatMin
            ValeurLat1 += (z_.CoordY(z_.IndexLatMin + 1, z_.IndexLigneInterpole) - ValeurLat1) * z_.DeltaLatMin
        End If
        z_.DebutSource = CInt(Math.Ceiling((ValeurLat0) + (ValeurLat1 - ValeurLat0) * DeltaLigneInterpole))
        Return CInt(LigneInterpoleFinTranche) + 1
    End Function
    ''' <summary> Calcul normaux pour l'interpolation des cartes avec grille. Facile à comprendre mais très gourmand en calculs donc en temps
    ''' à cause de la fonction ConvertWGS84ToProjection qui est précise mais qui utilise des puissances de 2 ou 3 sur les latitudes et longitudes</summary>
    ''' <param name="CarteInterpole">carte qui reçoit le résultat de l'interpolation</param>
    ''' <param name="CptY">N° de ligne de travail de la carte interpolée</param>
    ''' <param name="z_">structure contenant les paramètres de calculs</param>
    Private Function InterpolerLigneCarteAvecGrille_ConvertCoordonnees(CarteInterpole As Carte, DecalageYInterpole As Integer, CptY As Integer, DecalageYSource As Integer, z_ As ICAG) As ErreurInterpolation
        Dim IndexPixelInterpol As Integer = CptY * CarteInterpole._LargeurOctets
        Dim PointInterpole = CarteInterpole._Coins(0).LatLon
        PointInterpole.Lat -= (DecalageYInterpole + CptY) * z_.InterDeltaLat
        Dim EchelleMPixelX As Double = Echelle_M_PixelX
        Dim EchelleMPixelY As Double = Echelle_M_PixelY
        For Cptx As Integer = 0 To _LargeurPixel - 1
            'calcul des coordonnées métriques de la grille  
            Dim PointSource = ConvertWGS84ToProjection(PointInterpole, SystemCartographique.Projection.Datum)
            Dim XSource As Double = (PointSource.X - _Coins(0).X_Grid) / EchelleMPixelX 'transforme en pixel de la carte source
            Dim YSource As Double = (_Coins(0).Y_Grid - PointSource.Y) / EchelleMPixelY 'transforme en pixel de la carte source
            'Trouver le pixel interpolé (interpolation linéaire sur 4 pixels : X et X+1, Y et Y+1) de la carte originale
            RemplirPixelsTampon(DecalageYSource, XSource, YSource, CarteInterpole.Tampon.Bits, IndexPixelInterpol)
            IndexPixelInterpol += 3
            PointInterpole.Lon += z_.InterDeltaLong
            NbIterations += 1
            If NbIterations Mod 100 = 0 Then AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
        Next
        Return ErreurInterpolation.Ok
    End Function
    ''' <summary> procédure qui remplace la procedure InterpolerLigneCarteAvecGrille_ConvertCoordonnees
    ''' plus difficile à comprendre car il s'agit d'une interpolation linéaire sur les X et sur les Y que l'on appelle bilinéaire avec correction d'erreur car l'interpolation ne considère
    ''' que des droites alors qu'il s'agit de courbe. QualiteX permet d'ajuster le niveau de correction d'erreur (segment de droite) en fonction de l'échelle de la carte </summary>
    ''' <param name="CptY">n° de ligne de travail de la carte interpolée</param>
    ''' <param name="HauteurEcrite">decalage en nb de ligne du tampon par rapport à CptY. Sert quand il y a plusieurs tranches</param>
    ''' <param name="HauteurSource">nb de ligne du debut du tampon source par rapport au début du fichier source. Sert quand il y a plusieurs tranches</param>
    ''' <param name="z_">structures qui contient toutes les variables de calcul communes pour les boucles en parallele</param>
    Private Sub InterpolerLigneCarteAvecGrille(HauteurEcrite As Integer, CptY As Integer, HauteurSource As Integer, z_ As ICAG)
        'préparation de la boucle sur les X de la carte interpolée. Les dimensions en pixels de la carteInterpolée sont les mêmes que celles de la carte Originale
        Dim IndexPixelInterpol As Integer = CptY * _LargeurOctets + z_.IndexBits
        Dim DeltaY_Carte As Double = (CptY + HauteurEcrite) / z_.PasY
        Dim IndiceY_Grille As Integer = CInt(Math.Floor(DeltaY_Carte))
        DeltaY_Carte -= IndiceY_Grille
        Dim FinX As Integer, DebutX As Integer = 0
        'pour chaque pas sur l'axe des X ou Segment de droite de l'interpolation
        For IndiceX_Grille As Integer = 0 To z_.NbPasX - 1
            FinX += z_.PasX
            If FinX > _LargeurPixel Then
                FinX = _LargeurPixel
            End If
            'on peut charger les valeurs exprimées en pixels de début et de fin de segment et le poids d'un pixel du segment exprimé en % 
            Dim X0 As Double = z_.CoordX(IndiceX_Grille, IndiceY_Grille), DeltaX0 As Double = (z_.CoordX(IndiceX_Grille + 1, IndiceY_Grille) - X0) / z_.PasX
            Dim X1 As Double = z_.CoordX(IndiceX_Grille, IndiceY_Grille + 1), DeltaX1 As Double = (z_.CoordX(IndiceX_Grille + 1, IndiceY_Grille + 1) - X1) / z_.PasX
            Dim Y0 As Double = z_.CoordY(IndiceX_Grille, IndiceY_Grille), DeltaY0 As Double = (z_.CoordY(IndiceX_Grille + 1, IndiceY_Grille) - Y0) / z_.PasX
            Dim Y1 As Double = z_.CoordY(IndiceX_Grille, IndiceY_Grille + 1), DeltaY1 As Double = (z_.CoordY(IndiceX_Grille + 1, IndiceY_Grille + 1) - Y1) / z_.PasX
            'on peut calculer les coordonnées théoriques du point exprimée en pixel 
            For CptX As Integer = DebutX To FinX - 1
                Dim XReel As Double = X0 + (X1 - X0) * DeltaY_Carte
                Dim YReel As Double = Y0 + (Y1 - Y0) * DeltaY_Carte
                'et trouver le pixel interpolé correspondant en faisant une interpolation linéaire sur les 4 pixels englobant le point théorique (X et X+1, Y et Y+1) de la carte source
                RemplirPixelsTampon(HauteurSource, XReel, YReel, z_.Bits, IndexPixelInterpol)
                IndexPixelInterpol += 3
                X0 += DeltaX0
                X1 += DeltaX1
                Y0 += DeltaY0
                Y1 += DeltaY1
            Next
            DebutX = FinX
        Next
        Interlocked.Increment(NbIterations)
        If NbIterations Mod 100 = 0 Then AfficherVisueInformation($"{MessInfo & NbIterations} / {NbTotalIterations}")
    End Sub
    ''' <summary>calcul et écriture d'un pixel interpolé à partir de 4 pixels (3 octects par pixel) issus des coordonnées entières de la carte source</summary>
    ''' <param name="DecalageY">Nb de ligne de la carte source déja lue</param>
    ''' <param name="XReel"> coordonnées X du pixel interpolé </param>
    ''' <param name="YReel"> coordonnées Y du pixel interpolé</param>
    ''' <param name="TamponInterpole">tampon qui sert de récéptacle pour le pixel interpolé</param>
    ''' <param name="IndexTamponInterpole">index dans le tampon où écrire le pixel interpolé</param>
    ''' <remarks> Cette procédure peut être appelée en asynchrone (TPL) les variables hors procédure ne doivent pa être modifié pendant le traitement
    ''' autrement dit elles doivent être considérée en lecture seule sous peine de résultats inattendus </remarks>
    Private Sub RemplirPixelsTampon(DecalageY As Integer, XReel As Double, YReel As Double, TamponInterpole As Byte(), IndexTamponInterpole As Integer)
        Dim DebutLigne As Integer, FinLigne As Integer, DebutPixel As Integer, FinPixel As Integer
        'calcul des coordonnées entières (carte source) et des poids respectifs entre ligne et ligne+1
        Dim X As Integer = CInt(Math.Floor(XReel)), Y As Integer = CInt(Math.Floor(YReel))
        'si le point est complétement en dehors de la carte en latitude ou longitude on remplit le tampon de la carte interpolée par 1 pixel de la couleur de carte par defaut
        If Y > _HauteurPixel - 2 OrElse Y < -1 OrElse X > _LargeurPixel - 1 OrElse X < -1 Then
            PixelInterpol.CopyTo(TamponInterpole, IndexTamponInterpole)
        Else
            'on prépare un tampon de 4 pixels de la couleur de carte par defaut qui représente les pixels XY, X1Y,XY1,X1Y1 sous forme d'array et pas de tableau
            Dim PixelsSrc()()() As Byte = New Byte()()() {New Byte()() {PixelInterpol.ToArray, PixelInterpol.ToArray},
                                                          New Byte()() {PixelInterpol.ToArray, PixelInterpol.ToArray}}
            Dim PoidsX As Double = XReel - X, PoidsY As Double = YReel - Y
            'il y a 2 cas particuliers concernant les pixels de l'axe des Y
            If Y = _HauteurPixel - 1 Then
                DebutLigne = 0 'il n'y a pas de 2ème ligne pour réaliser l'interpolation
                FinLigne = 0 'on limite à la dernière ligne de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
            ElseIf Y = -1 Then 'on double la ligne 0 autrement dit il n'y a pas d'interpolation pour cette ligne
                Y = 0  'il n'y a pas de 1ère ligne pour réaliser l'interpolation
                DebutLigne = 1 'on limite à la 1ère ligne de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
                FinLigne = 1
            Else 'cas normal 2 lignes à lire
                DebutLigne = 0 'l'interpolation sera faite avec 2 lignes de la carte
                FinLigne = 1
            End If
            'il y 2 cas particuliers concernant les pixels de l'axe des X
            If X = _LargeurPixel - 1 Then
                DebutPixel = 0 'il n'y a pas de pixel suivant pour réaliser l'interpolation
                FinPixel = 0 'on limite au dernier pixel de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
            ElseIf X = -1 Then
                X = 0 'il n'y a pas de 1er pixel pour réaliser l'interpolation 
                DebutPixel = 1 'on limite au 1er pixel de la carte ce qui implique que l'interpolation sera faite avec la couleur de fond de carte
                FinPixel = 1
            Else 'cas normal il y a 2 pixels à lire
                DebutPixel = 0 'l'interpolation sera faite avec 2 pixels de la carte
                FinPixel = 1
            End If
            'on calcule l'index à partir duquel les octets de la carte source seront lus
            Dim IndexCarteSource As Integer = X * NbOctetsPixel + (Y - DecalageY) * _LargeurOctets + Tampon.IndexBits
            Dim Index As Integer
            For CptLigne As Integer = DebutLigne To FinLigne
                Index = 0
                'Lecture des pixels de la carte source (de de 1 à 4) et copie dans le tableau représentant les 4 pixels à interpolés
                For CptPixel = DebutPixel To FinPixel
                    'si la carte vient d'une image enregistrée en mémoire il faut copier à partir de la mémoire non managée d'où l'utilisation de Marshal
                    If Tampon.IsBitmap Then
                        Marshal.Copy(Tampon.BitPtr + IndexCarteSource + Index, PixelsSrc(CptLigne)(CptPixel), 0, NbOctetsPixel)
                    Else
                        If IndexCarteSource + Index + NbOctetsPixel < Tampon.Bits.Length Then
                            Array.Copy(Tampon.Bits, IndexCarteSource + Index, PixelsSrc(CptLigne)(CptPixel), 0, NbOctetsPixel)
                        End If
                    End If
                    Index += NbOctetsPixel
                Next
                IndexCarteSource += LargeurOctets
            Next
            '3 couleurs par pixel. 1 octet par couleur. ordre des couleurs en mémoire  : 0 = Bleu, 1 = Vert, 2 = Rouge
            For Couleur As Integer = 0 To 2
                'interpolation de une des 3 couleurs sur l'axe des X
                Dim CouleurPixelL0 As Double = PixelsSrc(0)(0)(Couleur) + (CDbl(PixelsSrc(0)(1)(Couleur)) - PixelsSrc(0)(0)(Couleur)) * PoidsX
                Dim CouleurPixelL1 As Double = PixelsSrc(1)(0)(Couleur) + (CDbl(PixelsSrc(1)(1)(Couleur)) - PixelsSrc(1)(0)(Couleur)) * PoidsX
                'interpolation de une des 3 couleurs sur l'axe des Y
                TamponInterpole(IndexTamponInterpole + Couleur) = CByte(Math.Round(CouleurPixelL0 + (CouleurPixelL1 - CouleurPixelL0) * PoidsY))
            Next
        End If
    End Sub
    ''' <summary> initialise la structure qui contient tous les élements de calcul pour l'interpolation des cartes qui ont déjà une grille </summary>
    Private Function InitialiserICAG(CarteInterpole As Carte) As ICAG
        Dim z_ As New ICAG
        Try
            With CarteInterpole
                z_.IndexBits = .Tampon.IndexBits
                z_.Bits = .Tampon.Bits
                'Trouver les pas X(Longitude) et Y(Latitude) exprimés en DD/pixel de la carte interpolée. Ce sont des valeurs constantes.
                'dans FCGP elles sont propres à chaque carte interpolée. d'autres logiciels les imposent au niveau d'un pays ou du monde 
                'ce qui induit des déformations beaucoup plus imortantes de l'image de la carte source
                z_.InterDeltaLong = (.Coins(2).Longitude - .Coins(0).Longitude) / LargeurPixel
                z_.InterDeltaLat = (.Coins(0).Latitude - .Coins(2).Latitude) / HauteurPixel
                'Calcul du nb de pas sur l'axe des X et des Y en fonction de la distance de qualité.
                'La distance de qualité à pour but de couper la droite orignale créer par une interpolation en segments.
                'Plus la distance de qualité est petite et plus la qualité de l'interpolation sera bonne
                z_.NbPasX = CInt(Math.Ceiling((Coins(2).X_Grid - Coins(0).X_Grid) / SystemCartographique.Niveau.QualiteInterpolGrille()))
                z_.NbPasY = CInt(Math.Ceiling((Coins(0).Y_Grid - Coins(2).Y_Grid) / SystemCartographique.Niveau.QualiteInterpolGrille()))
                'Calcul de la valeur du pas sur l'axe des X et des Y exprimés en pixels
                z_.PasX = CInt(Math.Ceiling(LargeurPixel / z_.NbPasX))
                z_.PasY = CInt(Math.Ceiling(HauteurPixel / z_.NbPasY))
                'Calcul de la grille des coordonnées sur X et Y
                ReDim z_.CoordX(z_.NbPasX, z_.NbPasY)
                ReDim z_.CoordY(z_.NbPasX, z_.NbPasY)
                If CalculerGrilleCoordonnees(CarteInterpole, z_) = ErreurInterpolation.NotOk Then z_ = Nothing
            End With
        Catch Ex As Exception
            z_ = Nothing
            AfficherErreur(Ex, "X6X6")
        End Try
        Return z_
    End Function
    ''' <summary> Calcule un ensemble de points exprimées en pixels de la carte source qui représente les points de la grille DD de la carte interpolée. 
    ''' Ces points peuvent être situés en dehors de la carte source. Pour rappel une grille à ses lignes parrallèles aux axes </summary>
    Private Function CalculerGrilleCoordonnees(CarteInterpole As Carte, z_ As ICAG) As ErreurInterpolation
        Dim MaxLatSource As Double, MinLatSource As Double
        'pour avoir le Pt0 Grille de la carte source en PointProjection. Cela induit une inprécision de l'ordre de 0.5 mètre
        'mais cela permet si la carte interpolée à le même Pt0 que celui de la carte source d'avoir un delta à 0
        Dim CaptureSource = ConvertWGS84ToProjection(Coins(0).LatLon, SystemCartographique.Projection.Datum)
        'Valeur exprimée en DD des pas sur l'axe des X et des Y
        Dim PasLongitude As Double = z_.PasX * z_.InterDeltaLong, PasLatitude As Double = z_.PasY * z_.InterDeltaLat
        'Valeur exprimée en Métres/Pixel afin d'être sur que le pas soit un multiple de 0.1
        Dim EchY As Double = Math.Round(Echelle_M_PixelY, 1), EchX As Double = Math.Round(Echelle_M_PixelX, 1)
        'représente 1 point de la grille de la carte interpolée exprimé en DD
        Dim PointLatLon As PointD
        'pour chaque point de la grille DD de la carte interpolée, on convertit le point DD en point mètres puis en point Pixel
        For CptPasX As Integer = 0 To z_.NbPasX
            If CptPasX > 0 Then 'calcule Longitude du point de la grille
                PointLatLon.Lon += PasLongitude
            Else
                PointLatLon.Lon = CarteInterpole.Coins(0).Longitude
            End If
            For CptPasY = 0 To z_.NbPasY
                If CptPasY > 0 Then
                    PointLatLon.Lat -= PasLatitude
                Else
                    PointLatLon.Lat = CarteInterpole.Coins(0).Latitude
                End If
                'calcul du point LatLon de la carte interpolée transformé en coordonnées métriques de la carte source
                Dim PointGrille = ConvertWGS84ToProjection(PointLatLon, SystemCartographique.Projection.Datum)
                'calcul du point LatLon de la carte interpolée transformé en pixels de la carte source
                z_.CoordY(CptPasX, CptPasY) = (CaptureSource.Y - PointGrille.Y) / EchY
                z_.CoordX(CptPasX, CptPasY) = (PointGrille.X - CaptureSource.X) / EchX
                'on cherche pour la première ligne de la carte interpolée
                If CptPasY = 0 Then
                    'le pas et le pixel de la carte source où se situe le Y Source le plus grand
                    If z_.CoordY(CptPasX, 0) > MaxLatSource Then
                        MaxLatSource = z_.CoordY(CptPasX, 0)
                        z_.IndexLatMax = CptPasX
                    End If
                    'et le pas et le pixel de la carte source où se situe le Y Source le plus petit
                    If z_.CoordY(CptPasX, 0) < MinLatSource Then
                        MinLatSource = z_.CoordY(CptPasX, 0)
                        z_.IndexLatMin = CptPasX
                    End If
                End If
            Next
        Next
        'si les indices LatMax et LatMin sont situés après la largeur de la carte, il faut les corriger pour
        'que la largeur en pixel de la carte interpolée reste la même que la largeur en pixel de la carte source
        'en fait largeurPixel -1 à cause de l'interpolation qui demande 2 pixels pour pouvoir être réalisée
        If z_.IndexLatMax * z_.PasX > LargeurPixel Then
            z_.IndexLatMax -= 1 'on calcule le delta avec l'indice précédent
            z_.DeltaLatMax = (LargeurPixel - 1) / z_.PasX - z_.IndexLatMax
        End If
        If z_.IndexLatMin * z_.PasX > LargeurPixel Then
            z_.IndexLatMin -= 1 'on calcule le delta avec l'indice précédent
            z_.DeltaLatMin = (LargeurPixel - 1) / z_.PasX - z_.IndexLatMin
        End If
        Return ErreurInterpolation.Ok
    End Function
#End Region
End Class