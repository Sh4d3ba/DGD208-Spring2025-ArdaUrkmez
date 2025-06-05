// Program.cs
// Entry point for the Interactive Pet Simulator.

using System.Threading.Tasks; 

namespace GameProg 
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Game petSimulator = new Game();
            await petSimulator.RunAsync(); // run game loop
        }
    }
}