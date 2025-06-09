
namespace GameProg
{
    public class Rabbit : Pet
    {
        public Rabbit(string name) : base(name, PetType.Rabbit)
        {
            Console.WriteLine($"Squeak! {Name} the Rabbit hops into your life!");
        }
    }
}