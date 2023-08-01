'''<summary>Minuterie qui permet d'attendre un certain temps exprimé en milli secondes de manière synchrone ou asynchrone</summary>
Friend Class Minuterie
    Implements IDisposable
    ''' <summary> Flag indiquant que l'attente demandée est arrivée à terme </summary>
    Friend Shared ReadOnly Property FlagFinAttente As Boolean
    ''' <summary> attend de manière asynchrone (rend la main à l'appelant) un certain délai avant de positionner à true le flag de fin d'attente </summary>
    ''' <param name="Delai"> nombre de millisecondes à attendre </param>
    Friend Shared Async Sub DeclencherAttenteAsync(Delai As Integer)
        Using M As New Minuterie
            _FlagFinAttente = False
            Await M.AttendreAsync(Delai)
            _FlagFinAttente = True
        End Using
    End Sub
    ''' <summary> envoi de manière asynchrone (rend la main à l'appelant) une touche au programme appelant </summary>
    ''' <param name="Delai"> nombre de millisecondes à attendre avant d'envoyer la touche au programme </param>
    ''' <param name="Touche"> touche à envoyer au programme </param>
    Friend Shared Async Sub DeclencherToucheAsync(Delai As Integer, Touche As String)
        Using M As New Minuterie
            Await M.AttendreAsync(Delai)
            SendKeys.Send(Touche)
        End Using
    End Sub
    ''' <summary>message à afficher dans la fenêtre information</summary>
    Private ReadOnly Chrono As Chronometre
    '''<summary>evenement pour indiqué que le temps d'attente de la minuterie est écoulé</summary>
    Private ReadOnly FinAttente As AutoResetEvent
    '''<summary> minuterie pour les taches qui sont éxécutée sur un thread qui ne doit pas être bloqué</summary>
    Private ReadOnly Minuterie As Threading.Timer
    ''' <summary> si true indique que l'objet a été déposé en attente d'un garbage collector </summary>
    Private disposedValue As Boolean
    ''' <summary> restitue les ressources managées par l'instance en cours</summary>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Minuterie.Dispose()
                FinAttente.Dispose()
            End If
        End If
        disposedValue = True
    End Sub
    ''' <summary> procédure de rappel pour déclencher l'évenement fin d'attente </summary>
    Private Sub Minuterie_Tick(Obj As Object)
        FinAttente.Set()
    End Sub
    ''' <summary> implémention de l'interface IDispose </summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
    ''' <summary> constructeur de l'instance </summary>
    Friend Sub New()
        Minuterie = New Threading.Timer(New TimerCallback(AddressOf Minuterie_Tick), Nothing, Timeout.Infinite, Timeout.Infinite)
        FinAttente = New AutoResetEvent(False)
        Chrono = New Chronometre(1)
    End Sub
    ''' <summary>Lancer l'attente de la minuterie en mode asynchrone, le thread appelant n'est pas bloqué</summary>
    ''' <param name="NbMillisecondes">nb de ms à attendre avant de rendre la main</param>
    Friend Async Function AttendreAsync(NbMillisecondes As Integer) As Task(Of TimeSpan)
        Dim TacheAttendre As TimeSpan = Await Task.Run(Function() Attendre(NbMillisecondes))
        Return TacheAttendre
    End Function
    ''' <summary> attend le nb de ms indiqué de manière synchrone</summary>
    ''' <param name="NbMillisecondes">nb de ms à attendre avant de rendre la main</param>
    Friend Function Attendre(NbMillisecondes As Integer) As TimeSpan
        Dim IndexChrono = Chrono.Demarre()
        Minuterie.Change(NbMillisecondes, Timeout.Infinite)
        FinAttente.WaitOne()
        Chrono.Arrete(IndexChrono)
        Return Chrono.Duree(IndexChrono)
    End Function
End Class