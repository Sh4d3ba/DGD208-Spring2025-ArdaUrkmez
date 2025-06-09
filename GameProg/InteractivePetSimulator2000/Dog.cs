
namespace GameProg
{
    public class Dog : Pet
    {
        public Dog(string name) : base(name, PetType.Dog)
        {
            Console.WriteLine($"Woof! {Name} the Dog has been adopted!");
        }
        
        public override async Task UseItemAsync(Item item)
        {
            Console.WriteLine($"{Name} shakes its tail, ready for the {item.Name}!");
            await base.UseItemAsync(item);
        }
    }
}