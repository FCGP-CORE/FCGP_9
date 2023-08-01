using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary>Donne accès à un ou plusieurs chronomètres. Sert surtout pour le débogage pour mesurer le temps d'éxécution d'une action </summary>
    internal class Chronometre
    {
        private readonly int Capacity;
        private readonly Stopwatch T;
        private readonly TimeSpan[] Durees;
        private readonly SortedSet<int> Isfree;
        internal readonly StatutChrono[] Statut;
        private int IndexMax;
        /// <summary> lecture de la durée du chrono depuis sa mise en route </summary>
        /// <param name="index">N° du chrono</param>
        internal TimeSpan Encours(int index)
        {
            return T.Elapsed - Durees[index];
        }
        /// <summary> lecture de la durée du chrono depuis sa mise en route jusqu'à l'arrêt. Rend le chrono de nouveau disponible pour un autre usage</summary>
        /// <param name="index">N° du chrono</param>
        internal TimeSpan Duree(int index)
        {
            TimeSpan DureeRet = Durees[index].Duration();
            if (Statut[index] == StatutChrono.Stoped)
            {
                Statut[index] = StatutChrono.Read;
                if (index == IndexMax) // cas du chrono au dessus de la pile
                {
                    IndexMax -= 1;
                }
                else // cas du chrono en milieu de pile on garde le numéro pour le réallouer
                {
                    Isfree.Add(index);
                }
            }
            else if (Statut[index] != StatutChrono.Read)
            {
                DureeRet = new TimeSpan(); // on renvoie null si le chrono n'est pas stoppé
            }

            return DureeRet;
        }
        /// <summary> dimensionne des chronomètres de précision </summary>
        /// <param name="Capacite"> nombre de chronomètres désirés </param>
        internal Chronometre(int Capacite)
        {
            T = new Stopwatch();
            Capacity = Capacite - 1;
            Durees = new TimeSpan[Capacity + 1];
            Statut = new StatutChrono[Capacity + 1];
            Isfree = new SortedSet<int>();
            IndexMax = -1;
            T.Start();
        }
        /// <summary> démarre le premier chronomètre disponible </summary>
        /// <returns> le numéro du chrono ou index </returns>
        internal int Demarre()
        {
            int DemarreRet;
            if (Isfree.Count > 0) // on regarde si il y a des index < à IndexMax pour les réallouer. On commence par le plus petit
            {
                DemarreRet = Isfree.First();
                Isfree.Remove(DemarreRet);
            }
            else
            {
                if (IndexMax + 1 > Capacity)
                    return -1; // on signale que le max est atteint et on sort sans rien faire
                IndexMax += 1;
                DemarreRet = IndexMax;
            }
            Statut[DemarreRet] = StatutChrono.Run;
            Durees[DemarreRet] = T.Elapsed;
            return DemarreRet;
        }
        /// <summary> arrête le chrono. la durée de celui-ci est disponible dans la proprité en lecture seule : Duree </summary>
        /// <param name="Index"></param>
        internal void Arrete(int Index)
        {
            if (Statut[Index] == StatutChrono.Run) // on ne peut arreter un chrono que si il est démarré
            {
                Durees[Index] = T.Elapsed - Durees[Index];
                Statut[Index] = StatutChrono.Stoped;
            }
        }
    }
}