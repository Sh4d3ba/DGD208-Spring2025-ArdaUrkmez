
// Event arguments for when a pet dies.

namespace GameProg
{
    public class PetDiedEventArgs : EventArgs
    {
        public string PetName { get; }
        public PetType PetType { get; }

        public PetDiedEventArgs(string petName, PetType petType)
        {
            PetName = petName;
            PetType = petType;
        }
    }
}