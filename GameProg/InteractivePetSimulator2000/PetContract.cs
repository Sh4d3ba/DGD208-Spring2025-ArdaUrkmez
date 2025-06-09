
// interface defining the contract for pets.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameProg
{
    public interface IPet
    {
        string Name { get; }
        PetType Type { get; }
        IReadOnlyDictionary<PetStat, int> Stats { get; }
        bool IsAlive { get; }

        // event for when stats change or general updates
        event EventHandler<PetStatsChangedEventArgs> StatsChanged;
        // event only for when a pet dies
        event EventHandler<PetDiedEventArgs> Died;

        void InitializeStats(int initialValue = 50);
        void DecreaseStat(PetStat stat, int amount);
        void IncreaseStat(PetStat stat, int amount);
        Task UseItemAsync(Item item); // method to use an item
        void PassTime(int statDecreaseAmount); // called by PetManager to decrease stats over time
        void CheckLiveliness();
        string GetStatus();
    }
}