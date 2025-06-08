

using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace GameProg
{
    public class Game
    {
        private bool _isRunning;
        private readonly PetManager _petManager;
        
        // main menu items
        private class MainMenuItem
        {
            public int Id { get; set; }
            public string Description { get; set; } = "";
            public Func<Task> Action { get; set; } = async () => await Task.CompletedTask;
        }
        private List<MainMenuItem> _mainMenuItems;
        
        private const string CREATOR_NAME = "Arda Urkmez"; 
        private const string STUDENT_NUMBER = "205040095"; 

        public Game()
        {
            _petManager = new PetManager();
            _petManager.PetDiedInCare += OnPetDiedInCare; // Subscribe to event
            _mainMenuItems = new List<MainMenuItem>();
        }
        
        private void Initialize()
        {
            Console.WriteLine("Welcome, this is the Interactive Pet Simulator 2000!");
            Console.WriteLine("Initializing game settings...");

            _mainMenuItems = new List<MainMenuItem>()
            {
                new MainMenuItem { Id = 1, Description = "Adopt a new Pet", Action = AdoptNewPetAsync },
                new MainMenuItem { Id = 2, Description = "View your Pets' Status", Action = ViewPetsStatusAsync },
                new MainMenuItem { Id = 3, Description = "Use an Item on a Pet", Action = UseItemOnPetAsync },
                new MainMenuItem { Id = 4, Description = "Display Creator Info", Action = DisplayCreatorInfoAsync },
                new MainMenuItem { Id = 5, Description = "Exit Game", Action = ExitGameAsync }
            };
            
        }

        public async Task RunAsync()
        {
            Initialize();
            _isRunning = true;

            while (_isRunning)
            {
                Console.Clear();
                Console.WriteLine("\n-Interactive Pet Simulator 2000-");
                
                var mainMenu = new Menu<MainMenuItem>(
                    "Main Menu",
                    _mainMenuItems,
                    menuItem => menuItem.Description // display each MainMenuItem
                );

                MainMenuItem? selectedAction = mainMenu.ShowAndGetSelection(); // return default(MainMenuItem) if "Go Back" is chosen

                if (selectedAction != null && selectedAction.Id != 0) 
                {
                    await selectedAction.Action.Invoke(); // execute selected 
                }
                else if (selectedAction == null && _mainMenuItems.Any()) 
                {
                   
                }


                if (_isRunning) 
                {
                    if(selectedAction != null && selectedAction.Id != 5) // don't pause if exiting
                    {
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                    }
                }
            }
            
            _petManager.StopPetUpdates(); 
            Console.WriteLine("\nThank you for playing! All pet care routines have been stopped. Goodbye!");
        }

        private async Task AdoptNewPetAsync()
        {
            Console.Clear();
            Console.WriteLine("-Adopt a New Pet-");

            var availablePetTypes = Enum.GetValues(typeof(PetType)).Cast<PetType>().ToList();
            var petTypeMenu = new Menu<PetType>("Choose Pet Type", availablePetTypes, pt => pt.ToString());
            PetType selectedPetType = petTypeMenu.ShowAndGetSelection();

            if (selectedPetType == default(PetType) && !Enum.IsDefined(typeof(PetType), selectedPetType) ) 
            {
                bool isActualDefaultChoice = true; 
                if (availablePetTypes.Count > 0 && selectedPetType.Equals(availablePetTypes[0])) {
                 
                }
                
                if (EqualityComparer<PetType>.Default.Equals(selectedPetType, default(PetType)) && !availablePetTypes.Contains(default(PetType)))
                { 
                   Console.WriteLine("Adoption cancelled.");
                   return;
                }


            }
             

            Console.Write($"Enter a name for your new {selectedPetType}: ");
            string? petName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(petName))
            {
                Console.WriteLine("Pet name cannot be empty. Adoption cancelled.");
                return;
            }

            IPet? newPet = null;
            switch (selectedPetType)
            {
                case PetType.Dog: newPet = new Dog(petName); break;
                case PetType.Cat: newPet = new Cat(petName); break;
                case PetType.Rabbit: newPet = new Rabbit(petName); break;
                case PetType.Bird: newPet = new Bird(petName); break;
                case PetType.Fish: newPet = new Fish(petName); break;
                default:
                    Console.WriteLine("Invalid pet type selected. This shouldn't happen with the menu.");
                    return;
            }
            
            _petManager.AdoptNewPet(newPet);
            await Task.CompletedTask;
        }

        private async Task ViewPetsStatusAsync()
        {
            Console.Clear();
            Console.WriteLine(" -Your Pets' Status- ");
            var pets = _petManager.GetAdoptedPets(); // gets all, including deceased

            if (!pets.Any())
            {
                Console.WriteLine("You haven't adopted any pets yet!");
                return;
            }

            foreach (var pet in pets)
            {
                Console.WriteLine(pet.GetStatus());
            }
            await Task.CompletedTask;
        }

        private async Task UseItemOnPetAsync()
        {
            Console.Clear();
            Console.WriteLine(" -Use an Item- ");

            var livingPets = _petManager.GetLivingPets();
            if (!livingPets.Any())
            {
                Console.WriteLine("You have no living pets to use items on.");
                return;
            }

            var petMenu = new Menu<IPet>("Select a Pet", livingPets, p => $"{p.Name} ({p.Type})");
            IPet? selectedPet = petMenu.ShowAndGetSelection();

            if (selectedPet == null) // chose "Go Back"
            {
                Console.WriteLine("Item usage cancelled.");
                return;
            }

            // LINQ to filter items with the selected pet type
            var usableItems = ItemDatabase.AllItems
                .Where(item => item.CompatibleWith == null || !item.CompatibleWith.Any() || item.CompatibleWith.Contains(selectedPet.Type))
                .ToList();

            if (!usableItems.Any())
            {
                Console.WriteLine($"No items available for {selectedPet.Name} the {selectedPet.Type}.");
                return;
            }
            
            var itemMenu = new Menu<Item>(
                $"Select an Item for {selectedPet.Name}", 
                usableItems, 
                item => $"{item.Name} (Affects: {item.AffectedStat}, Amount: +{item.EffectAmount}, Duration: {item.Duration}s)"
            );
            Item? selectedItem = itemMenu.ShowAndGetSelection();

            if (selectedItem == null) // chose "Go Back"
            {
                Console.WriteLine("Item selection cancelled.");
                return;
            }

            // perform item usage
            await selectedPet.UseItemAsync(selectedItem);
        }

        private Task DisplayCreatorInfoAsync()
        {
            Console.Clear();
            Console.WriteLine(" -Project Creator Information- ");
            Console.WriteLine($"Name: {CREATOR_NAME}");
            Console.WriteLine($"Student Number: {STUDENT_NUMBER}");
            return Task.CompletedTask;
        }

        private async Task ExitGameAsync()
        {
            Console.WriteLine("Preparing to exit game...");
            _isRunning = false;
            await Task.CompletedTask;
        }

        // event handler for PetDiedInCare from PetManager
        private void OnPetDiedInCare(object? sender, PetDiedEventArgs e)
        {
            Console.WriteLine($"\n[GAME ALERT] Sad news from the Pet Care Center: {e.PetName} the {e.PetType} has passed away.");
            Console.WriteLine("They will be missed. You can view their status in the pets list.");
         
        }
    }
}
