
// base class for all pets

using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace GameProg
{
    public abstract class Pet : IPet
    {
        public string Name { get; protected set; }
        public PetType Type { get; protected set; }
        
        protected Dictionary<PetStat, int> currentStats;
        public IReadOnlyDictionary<PetStat, int> Stats => currentStats; 

        public bool IsAlive { get; protected set; } = true;

        // events as required by petContract
        public event EventHandler<PetStatsChangedEventArgs> StatsChanged;
        public event EventHandler<PetDiedEventArgs> Died;

        protected Pet(string name, PetType type)
        {
            Name = name;
            Type = type;
            currentStats = new Dictionary<PetStat, int>();
            InitializeStats(); 
        }

        public virtual void InitializeStats(int initialValue = 50)
        {
            foreach (PetStat statName in Enum.GetValues(typeof(PetStat)))
            {
                currentStats[statName] = initialValue;
            }
            OnStatsChanged(); // notify bout stats
        }

        public virtual void DecreaseStat(PetStat stat, int amount)
        {
            if (!IsAlive) return;

            currentStats[stat] = Math.Max(0, currentStats[stat] - amount);
            OnStatsChanged();
            CheckLiveliness(); // check after stat decrease
        }

        public virtual void IncreaseStat(PetStat stat, int amount)
        {
            if (!IsAlive) return;

            currentStats[stat] = Math.Min(100, currentStats[stat] + amount);
            Console.WriteLine($"{Name}'s {stat} increased to {currentStats[stat]} using an item.");
            OnStatsChanged();
        }

        public virtual async Task UseItemAsync(Item item)
        {
            if (!IsAlive)
            {
                Console.WriteLine($"{Name} can't use items, it's no longer with us.");
                return;
            }
            Console.WriteLine($"{Name} is using {item.Name}...");
            // convert float to int milliseconds
            await Task.Delay((int)(item.Duration * 1000)); 
            Console.WriteLine($"{Name} finished using {item.Name}.");
            IncreaseStat(item.AffectedStat, item.EffectAmount);
        }
        
        public virtual void PassTime(int statDecreaseAmount)
        {
            if (!IsAlive) return;

            // decrease all stats by an amount
            foreach (PetStat statKey in currentStats.Keys.ToList()) 
            {
                 DecreaseStat(statKey, statDecreaseAmount);
                 if (!IsAlive) break; 
            }
        }

        public virtual void CheckLiveliness()
        {
            if (!IsAlive) return; 
            
            if (currentStats.Any(kvp => kvp.Value <= 0))
            {
                IsAlive = false;
                OnDied(); 
            }
        }

        public virtual string GetStatus()
        {
            if (!IsAlive) return $"{Name} the {Type} (Deceased)";
            
            var statStrings = currentStats.Select(kvp => $"{kvp.Key}: {kvp.Value}");
            return $"{Name} ({Type}) - Stats: [{string.Join(", ", statStrings)}]";
        }

        // methods to raise events
        protected virtual void OnStatsChanged()
        {
            if (IsAlive) 
            {
                 StatsChanged?.Invoke(this, new PetStatsChangedEventArgs(Name, Type, new Dictionary<PetStat, int>(currentStats)));
            }
        }

        protected virtual void OnDied()
        {
            Console.WriteLine($"!!! Oh no! {Name} the {Type} has passed away due to neglect. !!!");
            Died?.Invoke(this, new PetDiedEventArgs(Name, Type));
        }
    }
}
