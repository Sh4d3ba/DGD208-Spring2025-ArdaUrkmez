
namespace GameProg
{
    public class Cat : Pet
    {
        public Cat(string name) : base(name, PetType.Cat)
        {
            Console.WriteLine($"Meow! {Name} the Cat has joined your family!");
        }
    }
}