namespace FCGP
{
    /// <summary>Minuterie qui permet d'attendre un certain temps exprimé en milli secondes de manière synchrone ou asynchrone</summary>
    internal class Minuterie : IDisposable
    {
        /// <summary> Flag indiquant que l'attente demandée est arrivée à terme </summary>
        internal static bool FlagFinAttente { get; private set; }
        /// <summary> attend de manière asynchrone (rend la main à l'appelant) un certain délai avant de positionner à true le flag de fin d'attente </summary>
        /// <param name="Delai"> nombre de millisecondes à attendre </param>
        internal static async void DeclencherAttenteAsync(int Delai)
        {
            using (var M = new Minuterie())
            {
                FlagFinAttente = false;
                await M.AttendreAsync(Delai);
                FlagFinAttente = true;
            }
        }
        /// <summary> envoi de manière asynchrone (rend la main à l'appelant) une touche au programme appelant </summary>
        /// <param name="Delai"> nombre de millisecondes à attendre avant d'envoyer la touche au programme </param>
        /// <param name="Touche"> touche à envoyer au programme </param>
        internal static async void DeclencherToucheAsync(int Delai, string Touche)
        {
            using (var M = new Minuterie())
            {
                await M.AttendreAsync(Delai);
                SendKeys.Send(Touche);
            }
        }
        /// <summary>message à afficher dans la fenêtre information</summary>
        private readonly Chronometre Chrono;
        /// <summary>evenement pour indiqué que le temps d'attente de la minuterie est écoulé</summary>
        private readonly AutoResetEvent FinAttente;
        /// <summary> minuterie pour les taches qui sont éxécutée sur un thread qui ne doit pas être bloqué</summary>
        private readonly System.Threading.Timer MinuterieField;
        /// <summary> si true indique que l'objet a été déposé en attente d'un garbage collector </summary>
        private bool disposedValue;
        /// <summary> restitue les ressources managées par l'instance en cours</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MinuterieField.Dispose();
                    FinAttente.Dispose();
                }
            }
            disposedValue = true;
        }
        /// <summary> procédure de rappel pour déclencher l'évenement fin d'attente </summary>
        private void Minuterie_Tick(object Obj)
        {
            FinAttente.Set();
        }
        /// <summary> implémention de l'interface IDispose </summary>
        internal void Dispose()
        {
            Dispose(true);
        }

        void IDisposable.Dispose() => Dispose();
        /// <summary> constructeur de l'instance </summary>
        internal Minuterie()
        {
            MinuterieField = new System.Threading.Timer(new TimerCallback(Minuterie_Tick), null, Timeout.Infinite, Timeout.Infinite);
            FinAttente = new AutoResetEvent(false);
            Chrono = new Chronometre(1);
        }
        /// <summary>Lancer l'attente de la minuterie en mode asynchrone, le thread appelant n'est pas bloqué</summary>
        /// <param name="NbMillisecondes">nb de ms à attendre avant de rendre la main</param>
        internal async Task<TimeSpan> AttendreAsync(int NbMillisecondes)
        {
            var TacheAttendre = await Task.Run(() => Attendre(NbMillisecondes));
            return TacheAttendre;
        }
        /// <summary> attend le nb de ms indiqué de manière synchrone</summary>
        /// <param name="NbMillisecondes">nb de ms à attendre avant de rendre la main</param>
        internal TimeSpan Attendre(int NbMillisecondes)
        {
            int IndexChrono = Chrono.Demarre();
            MinuterieField.Change(NbMillisecondes, Timeout.Infinite);
            FinAttente.WaitOne();
            Chrono.Arrete(IndexChrono);
            return Chrono.Duree(IndexChrono);
        }
    }
}