''' <summary> regroupement des données concernant une image représentant une région cartographique
''' Tuile de fichier tuile, carte ou page à imprimer </summary>
Friend Class DescriptionImageFichier
    ''' <summary> nom de l'image qui peut être un chemin pour un fichier ou juste un nom pour un élement en mémoire </summary>
    Friend Nom As String
    ''' <summary> coordonnées réelles sous différentes formes </summary>
    Friend Property Nord As Double
        Get
            Return Region.Nord
        End Get
        Set(value As Double)
            Region.Nord = value
        End Set
    End Property
    Friend Property Ouest As Double
        Get
            Return Region.Ouest
        End Get
        Set(value As Double)
            Region.Ouest = value
        End Set
    End Property
    Friend Property Est As Double
        Get
            Return Region.Est
        End Get
        Set(value As Double)
            Region.Est = value
        End Set
    End Property
    Friend Property Sud As Double
        Get
            Return Region.Sud
        End Get
        Set(value As Double)
            Region.Sud = value
        End Set
    End Property
    Friend Region As RectangleD
    Friend Property LargeurPixels As Integer
        Get
            Return TaillePixels.Width
        End Get
        Set(value As Integer)
            TaillePixels.Width = value
        End Set
    End Property

    Friend Property HauteurPixels As Integer
        Get
            Return TaillePixels.Height
        End Get
        Set(value As Integer)
            TaillePixels.Height = value
        End Set
    End Property
    ''' <summary> taille de la tuile exprimée en pixels </summary>
    Friend TaillePixels As Size
End Class
