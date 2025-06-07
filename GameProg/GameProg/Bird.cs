
namespace GameProg
{
    public class Bird : Pet
    {
        public Bird(string name) : base(name, PetType.Bird)
        {
            Console.WriteLine($"Chirp, chirp! {Name} the Bird flies in!");
        }
    }
}