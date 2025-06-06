
// manage adopted pets, stats, and handle periodic updates.

using System.Collections.Generic;
using System.Linq; 
using System.Threading; 

namespace GameProg
{
    public class PetManager
    {
        private readonly List<IPet> adoptedPets;
        private Timer? petUpdateTimer; // periodic stat decrease
        
        // stat decrease variables
        private readonly int statDecreaseAmount = 1; // decrease amount
        private readonly TimeSpan statDecreaseInterval = TimeSpan.FromSeconds(5); // how often stats decrease

        // dvent to notify Game when a pet has died(RIP)
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
            pet.Died += HandlePetDeathInternal; // sub to pets own Died event

            Console.WriteLine($"{pet.Name} the {pet.Type} is now under your care.");

            // start the timer only if there are living pets and it's not already running
            if (petUpdateTimer == null && adoptedPets.Any(p => p.IsAlive))
            {
                StartPetUpdates();
            }
        }

        private void HandlePetStatsChanged(object? sender, PetStatsChangedEventArgs e)

        private void HandlePetDeathInternal(object? sender, PetDiedEventArgs e)
        {
            PetDiedInCare?.Invoke(this, e); 
            
            // if no living pets stop the timer.
            if (!adoptedPets.Any(p => p.IsAlive))
            {
                StopPetUpdates();
            }
        }
        
        public void FormallyRemoveDeceasedPet(string petName, PetType petType)
        {
            // find deceased pet
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
                Console.WriteLine($"Pet care routine initiated: stats will decrease by {statDecreaseAmount} every {statDecreaseInterval.TotalSeconds} seconds.");
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
                StopPetUpdates(); // timer stops if no one is left
                return;
            }
            
            Console.WriteLine($"\n Time Passes for Your Pets ({DateTime.Now:T}) ");
            foreach (var pet in petsToUpdateThisTick)
            {
                if (pet.IsAlive)
                {
                    pet.PassTime(statDecreaseAmount); // pet own stat decrease and death check
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
            // find living pet
            return adoptedPets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.IsAlive);
        }
    }
}
