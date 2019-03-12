using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.Core.Player;
using Rocket.API.User;

namespace Rocket.Unturned.Player
{
    public class UnturnedPlayer : BasePlayer<UnturnedUser, UnturnedPlayerEntity, UnturnedPlayer>
    {
        public override int GetHashCode()
        {
            return User.Id.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (!(o is UnturnedPlayer uPlayer))
                return false;

            return uPlayer.GetHashCode() == GetHashCode();
        }


        public SDG.Unturned.Player NativePlayer => PlayerTool.getPlayer(CSteamID);

        public SteamPlayer SteamPlayer => NativePlayer?.channel?.owner;

        public string DisplayName => CharacterName;

        public bool IsAdmin => NativePlayer.channel.owner.isAdmin;

        public CSteamID CSteamID { get; }

        private readonly UnturnedPlayerManager manager;
        
        private readonly IDependencyContainer container;

        public UnturnedPlayer(IDependencyContainer container, SteamPlayer player, UnturnedPlayerManager manager) : this(container, player.playerID.steamID, manager)
        {

        }

        public UnturnedPlayer(IDependencyContainer container, CSteamID cSteamID, UnturnedPlayerManager manager) : base(container, manager)
        {
            this.container = container;
            this.manager = manager;
            CSteamID = cSteamID;
        }

        public float Ping => NativePlayer.channel.owner.ping;

        public override UnturnedUser User
        {
            get
            {
                return (UnturnedUser)container.Resolve<IUserManager>().GetUserAsync(CSteamID.ToString()).Result;
            }
        }

        public bool Equals(UnturnedPlayer p)
        {
            if (p == null) return false;

            return CSteamID.ToString() == p.CSteamID.ToString();
        }

        public T GetComponent<T>() => (T)(object)NativePlayer.GetComponent(typeof(T));

        public void TriggerEffect(ushort effectID)
        {
            EffectManager.instance.channel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER,
                effectID, NativePlayer.transform.position);
        }

        public string RemoteAddress
        {
            get
            {
                SteamGameServerNetworking.GetP2PSessionState(CSteamID, out P2PSessionState_t state);
                return Parser.getIPFromUInt32(state.m_nRemoteIP);
            }
        }

        public void MaximizeSkills()
        {
            PlayerSkills skills = NativePlayer.skills;

            foreach (Skill skill in skills.skills.SelectMany(s => s)) skill.level = skill.max;

            skills.askSkills(NativePlayer.channel.owner.playerID.steamID);
        }

        public string SteamGroupName
        {
            get
            {
                {
                    FriendsGroupID_t id;
                    id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
                    return SteamFriends.GetFriendsGroupName(id);
                }
            }
        }

        public int SteamGroupMembersCount
        {
            get
            {
                FriendsGroupID_t id;
                id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
                return SteamFriends.GetFriendsGroupMembersCount(id);
            }
        }

        public PlayerInventory Inventory => NativePlayer.inventory;

        public bool GiveItem(ushort itemId, byte amount) => ItemTool.tryForceGiveItem(NativePlayer, itemId, amount);

        public bool GiveItem(Item item) => NativePlayer.inventory.tryAddItem(item, false);

        public bool GiveVehicle(ushort vehicleId) => VehicleTool.giveVehicle(NativePlayer, vehicleId);

        public CSteamID SteamGroupID => NativePlayer.channel.owner.playerID.group;

        public void SetAdmin(bool isAdmin)
        {
            SetAdmin(isAdmin, null);
        }

        public void SetAdmin(bool isAdmin, UnturnedPlayer issuer)
        {
            if (isAdmin)
                SteamAdminlist.admin(CSteamID, issuer?.CSteamID ?? new CSteamID(0));
            else
                SteamAdminlist.unadmin(NativePlayer.channel.owner.playerID.steamID);
        }

        public string CharacterName => NativePlayer.channel.owner.playerID.characterName;

        public string SteamName => NativePlayer.channel.owner.playerID.playerName;

        public bool IsPro => NativePlayer.channel.owner.isPro;

        public InteractableVehicle CurrentVehicle => NativePlayer.movement.getVehicle();

        public bool IsInVehicle => CurrentVehicle != null;

        public override UnturnedPlayerEntity Entity => new UnturnedPlayerEntity(this);

        public override bool IsOnline => Provider.clients.Any(c => c.playerID.steamID == CSteamID);

        public override DateTime SessionConnectTime => DateTime.Now;

        public override DateTime? SessionDisconnectTime => null;

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format != null && SteamPlayer != null)
            {
                string[] subFormats = format.Split(':');

                format = subFormats[0];
                string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

                if (format.Equals("nick", StringComparison.OrdinalIgnoreCase)
                    || format.Equals("nickname", StringComparison.OrdinalIgnoreCase))
                {
                    return SteamPlayer.playerID.nickName.ToString(formatProvider);
                }

                if (format.Equals("playername", StringComparison.OrdinalIgnoreCase))
                {
                    return SteamPlayer.playerID.playerName.ToString(formatProvider);
                }

                if (format.Equals("charachtername", StringComparison.OrdinalIgnoreCase))
                {
                    return SteamPlayer.playerID.characterName.ToString(formatProvider);
                }

                if (format.Equals("group", StringComparison.OrdinalIgnoreCase)
                    || format.Equals("groupname", StringComparison.OrdinalIgnoreCase))
                {
                    var gid = SteamPlayer.playerID.@group;
                    if (gid == CSteamID.Nil)
                        return "no group";

                    GroupInfo group = GroupManager.getGroupInfo(gid);
                    return group.name.ToString(formatProvider);
                }


                if (format.Equals("groupid", StringComparison.OrdinalIgnoreCase))
                {
                    var gid = SteamPlayer.playerID.@group;
                    if (gid == CSteamID.Nil)
                        return "no group";

                    return gid.m_SteamID.ToString(subFormat, formatProvider);
                }
            }

            return base.ToString(format, formatProvider);
        }
    }
}
