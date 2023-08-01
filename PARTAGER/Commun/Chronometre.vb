'''<summary>Donne accès à un ou plusieurs chronomètres. Sert surtout pour le débogage pour mesurer le temps d'éxécution d'une action </summary>
Friend Class Chronometre
    Private ReadOnly Capacity As Integer
    Private ReadOnly T As Stopwatch
    Private ReadOnly Durees() As TimeSpan
    Private ReadOnly Isfree As SortedSet(Of Integer)
    Friend ReadOnly Statut() As StatutChrono
    Private IndexMax As Integer
    ''' <summary> lecture de la durée du chrono depuis sa mise en route </summary>
    ''' <param name="index">N° du chrono</param>
    Friend ReadOnly Property Encours(index As Integer) As TimeSpan
        Get
            Return T.Elapsed - Durees(index)
        End Get
    End Property
    ''' <summary> lecture de la durée du chrono depuis sa mise en route jusqu'à l'arrêt. Rend le chrono de nouveau disponible pour un autre usage</summary>
    ''' <param name="index">N° du chrono</param>
    Friend ReadOnly Property Duree(index As Integer) As TimeSpan
        Get
            Duree = Durees(index).Duration
            If Statut(index) = StatutChrono.Stoped Then
                Statut(index) = StatutChrono.Read
                If index = IndexMax Then 'cas du chrono au dessus de la pile
                    IndexMax -= 1
                Else 'cas du chrono en milieu de pile on garde le numéro pour le réallouer
                    Isfree.Add(index)
                End If
            ElseIf Statut(index) <> StatutChrono.Read Then
                Duree = New TimeSpan 'on renvoie null si le chrono n'est pas stoppé
            End If
        End Get
    End Property
    ''' <summary> dimensionne des chronomètres de précision </summary>
    ''' <param name="Capacite"> nombre de chronomètres désirés </param>
    Friend Sub New(Capacite As Integer)
        T = New Stopwatch
        Capacity = Capacite - 1
        ReDim Durees(Capacity)
        ReDim Statut(Capacity)
        Isfree = New SortedSet(Of Integer)
        IndexMax = -1
        T.Start()
    End Sub
    ''' <summary> démarre le premier chronomètre disponible </summary>
    ''' <returns> le numéro du chrono ou index </returns>
    Friend Function Demarre() As Integer
        If Isfree.Count > 0 Then 'on regarde si il y a des index < à IndexMax pour les réallouer. On commence par le plus petit
            Demarre = Isfree.First
            Isfree.Remove(Demarre)
        Else
            If IndexMax + 1 > Capacity Then Return -1 'on signale que le max est atteint et on sort sans rien faire
            IndexMax += 1
            Demarre = IndexMax
        End If
        Statut(Demarre) = StatutChrono.Run
        Durees(Demarre) = T.Elapsed
    End Function
    ''' <summary> arrête le chrono. la durée de celui-ci est disponible dans la proprité en lecture seule : Duree </summary>
    ''' <param name="Index"></param>
    Friend Sub Arrete(Index As Integer)
        If Statut(Index) = StatutChrono.Run Then 'on ne peut arreter un chrono que si il est démarré
            Durees(Index) = T.Elapsed - Durees(Index)
            Statut(Index) = StatutChrono.Stoped
        End If
    End Sub
End Class