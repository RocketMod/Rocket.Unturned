using Rocket.API.User;
using Rocket.Core.Permissions;
using Rocket.Core.ServiceProxies;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Permissions
{
    [ServicePriority(Priority = ServicePriority.Lowest)]
    public class UnturnedAdminPermissionProvider : FullPermitPermissionProvider
    {
        public override bool SupportsTarget(IIdentity target)
        {
            if (target is UnturnedUser user)
                return user.Player.IsAdmin;

            if (target is UnturnedPlayer player)
                return player.IsAdmin;

            return false;
        }
    }
}