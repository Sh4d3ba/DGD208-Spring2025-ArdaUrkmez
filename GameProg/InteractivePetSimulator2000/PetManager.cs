
// Manages adopted pets, their stats, and handles periodic updates.

using System.Collections.Generic;
using System.Linq; 
using System.Threading; 

namespace GameProg
{
    public class PetManager
    {
        private readonly List<IPet> adoptedPets;
        private Timer? petUpdateTimer; // Timer for periodic stat decrease
        
        // Configurable stat decrease parameters
        private readonly int statDecreaseAmount = 1; // Amount to decrease each stat by per tick
        
        private readonly TimeSpan statDecreaseInterval = TimeSpan.FromSeconds(5); 

        // Event to notify Game when a pet under care has died
        public event EventHandler<PetDiedEventArgs> PetDiedInCare;

        public PetManager()
        {
            adoptedPets = new List<IPet>();
        }

        public void AdoptNewPet(IPet pet)
        {
            if (pet == null)
            {
                Console.WriteLine("Cannot adopt a null pet.");
                return;
            }
            adoptedPets.Add(pet);
            pet.Died += HandlePetDeathInternal; 
            pet.StatsChanged += HandlePetStatsChanged;

            Console.WriteLine($"{pet.Name} the {pet.Type} is now under your care.");

            if (petUpdateTimer == null && adoptedPets.Any(p => p.IsAlive))
            {
                StartPetUpdates();
            }
        }
        
        private void HandlePetStatsChanged(object? sender, PetStatsChangedEventArgs e)
        {
        }

        private void HandlePetDeathInternal(object? sender, PetDiedEventArgs e)
        {
            PetDiedInCare?.Invoke(this, e); 
            
            if (!adoptedPets.Any(p => p.IsAlive))
            {
                StopPetUpdates();
            }
        }
        
        public void FormallyRemoveDeceasedPet(string petName, PetType petType)
        {
            IPet? petToRemove = adoptedPets.FirstOrDefault(p => p.Name == petName && p.Type == petType && !p.IsAlive);
            if (petToRemove != null)
            {
                adoptedPets.Remove(petToRemove);
            }
        }

        public void StartPetUpdates()
        {
            if (petUpdateTimer == null && adoptedPets.Any(p => p.IsAlive))
            {
                petUpdateTimer = new Timer(UpdateAllPetStatsTick, null, statDecreaseInterval, statDecreaseInterval);
                
                Console.WriteLine($"\nPet care routine initiated. Remember to look after your pets, as their Hunger, Fun, and Sleep will decrease by {statDecreaseAmount} point every {statDecreaseInterval.TotalSeconds} seconds.");
            }
        }

        public void StopPetUpdates()
        {
            petUpdateTimer?.Dispose(); 
            petUpdateTimer = null;
            Console.WriteLine("Pet care routine paused/stopped.");
        }

        private void UpdateAllPetStatsTick(object? state)
        {
            List<IPet> petsToUpdateThisTick = adoptedPets.Where(p => p.IsAlive).ToList(); 
            
            if (!petsToUpdateThisTick.Any())
            {
                StopPetUpdates();
                return;
            }
            
            foreach (var pet in petsToUpdateThisTick)
            {
                if (pet.IsAlive)
                {
                    pet.PassTime(statDecreaseAmount);
                }
            }
        }

        public List<IPet> GetAdoptedPets()
        {
            return adoptedPets.ToList();
        }

        public List<IPet> GetLivingPets()
        {
            return adoptedPets.Where(p => p.IsAlive).ToList();
        }

        public IPet? GetPetByName(string name)
        {
            return adoptedPets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.IsAlive);
        }
    }
}
