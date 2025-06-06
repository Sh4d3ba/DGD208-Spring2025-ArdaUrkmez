
// event arguments for when a pet's stats change

using System.Collections.Generic;

namespace GameProg
{
    public class PetStatsChangedEventArgs : EventArgs
    {
        public string PetName { get; }
        public PetType PetType { get; }
        public IReadOnlyDictionary<PetStat, int> CurrentStats { get; }

        public PetStatsChangedEventArgs(string petName, PetType petType, IReadOnlyDictionary<PetStat, int> currentStats)
        {
            PetName = petName;
            PetType = petType;
            CurrentStats = currentStats;
        }
    }
}