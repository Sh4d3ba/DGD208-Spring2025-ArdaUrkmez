
using System.Threading.Tasks; 

namespace GameProg 
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Game petSimulator = new Game();
            await petSimulator.RunAsync(); 
        }
    }
}