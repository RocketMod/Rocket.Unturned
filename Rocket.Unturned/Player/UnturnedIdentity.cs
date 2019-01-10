using Rocket.API.User;

namespace Rocket.Unturned.Player {
    public class UnturnedIdentity : IIdentity
    {
        public UnturnedIdentity(ulong id)
        {
            Id = id.ToString();
        }

        public string Id { get; }
        public string IdentityType => "Unturned";
    }
}