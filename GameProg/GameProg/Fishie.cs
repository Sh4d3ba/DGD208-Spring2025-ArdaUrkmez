
namespace GameProg
{
    public class Fish : Pet
    {
        public Fish(string name) : base(name, PetType.Fish)
        {
            Console.WriteLine($"Blub! {Name} the Fish swims into view!");
        }
    }
}