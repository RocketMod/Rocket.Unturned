using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.API.Chat;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Node = SDG.Unturned.Node;

namespace Rocket.Unturned.Player
{
    public sealed class UnturnedPlayer : IPlayer
    {
        public SDG.Unturned.Player Player { get; }
        public SteamPlayer SteamPlayer => Player.channel.owner;

        public string Id => CSteamID.ToString();

        public string DisplayName => CharacterName;

        public bool IsAdmin => Player.channel.owner.isAdmin;

        public CSteamID CSteamID => Player.channel.owner.playerID.steamID;

        private readonly IDependencyContainer container;

        public UnturnedPlayer(IDependencyContainer container, SteamPlayer player)
        {
            this.container = container;
            Player = player.player;
        }

        public UnturnedPlayer(IDependencyContainer container, CSteamID cSteamID) : this(container, PlayerTool.getSteamPlayer(cSteamID))
        {
            
        }

        public float Ping => Player.channel.owner.ping;

        public bool Equals(UnturnedPlayer p)
        {
            if (p == null) return false;

            return CSteamID.ToString() == p.CSteamID.ToString();
        }

        public T GetComponent<T>() => (T) (object) Player.GetComponent(typeof(T));

        public int CompareTo(IIdentifiable other) => string.Compare(Id, other.Id, StringComparison.Ordinal);

        public bool Equals(IIdentifiable other)
        {
            if (other == null)
                return false;

            return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(string other) => string.Compare(Id, other, StringComparison.Ordinal);

        public bool Equals(string other) => Id.Equals(other, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Id;

        public void TriggerEffect(ushort effectID)
        {
            EffectManager.instance.channel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER,
                effectID, Player.transform.position);
        }

        public string RemoteIp
        {
            get
            {
                SteamGameServerNetworking.GetP2PSessionState(CSteamID, out P2PSessionState_t state);
                return Parser.getIPFromUInt32(state.m_nRemoteIP);
            }
        }

        public void MaxSkills()
        {
            PlayerSkills skills = Player.skills;

            foreach (Skill skill in skills.skills.SelectMany(s => s)) skill.level = skill.max;

            skills.askSkills(Player.channel.owner.playerID.steamID);
        }

        public string SteamGroupName()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short) SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupName(id);
        }

        public int SteamGroupMembersCount()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short) SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupMembersCount(id);
        }

        public PlayerInventory Inventory => Player.inventory;

        public bool GiveItem(ushort itemId, byte amount) => ItemTool.tryForceGiveItem(Player, itemId, amount);

        public bool GiveItem(Item item) => Player.inventory.tryAddItem(item, false);

        public bool GiveVehicle(ushort vehicleId) => VehicleTool.giveVehicle(Player, vehicleId);

        public CSteamID SteamGroupID => Player.channel.owner.playerID.group;

        public void Kick(string reason)
        {
            Provider.kick(CSteamID, reason);
        }

        public void Ban(string reason, uint duration)
        {
            Provider.ban(CSteamID, reason, duration);
        }

        public void Admin(bool admin)
        {
            Admin(admin, null);
        }

        public void Admin(bool admin, UnturnedPlayer issuer)
        {
            if (admin)
                SteamAdminlist.admin(CSteamID, issuer?.CSteamID ?? new CSteamID(0));
            else
                SteamAdminlist.unadmin(Player.channel.owner.playerID.steamID);
        }

        public void Teleport(UnturnedPlayer target)
        {
            Vector3 d1 = target.Player.transform.position;
            Vector3 vector31 = target.Player.transform.rotation.eulerAngles;
            Teleport(d1, MeasurementTool.angleToByte(vector31.y));
        }

        public void Teleport(Vector3 position, float rotation)
        {
            /*
            if (VanishMode)
            {
                player.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                player.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.y, position.y + 1337, position.z), MeasurementTool.angleToByte(rotation));
                player.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
            */
            Player.channel.send("askTeleport", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, position,
                MeasurementTool.angleToByte(rotation));
            /*}*/
        }

        public Vector3 Position => Player.transform.position;

        public EPlayerStance Stance => Player.stance.stance;

        public float Rotation => Player.transform.rotation.eulerAngles.y;

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.nodes.FirstOrDefault(n
                => n.type == ENodeType.LOCATION
                    && ((LocationNode) n).name.ToLower().Contains(nodeName));
            if (node != null)
            {
                Vector3 c = node.point + new Vector3(0f, 0.5f, 0f);
                Player.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }

            return false;
        }

        public byte Stamina => Player.life.stamina;

        public string CharacterName => Player.channel.owner.playerID.characterName;

        public string SteamName => Player.channel.owner.playerID.playerName;

        public byte Infection
        {
            get => Player.life.virus;
            set
            {
                Player.life.askDisinfect(100);
                Player.life.askInfect(value);
            }
        }

        public uint Experience
        {
            get => Player.skills.experience;
            set
            {
                Player.skills.channel.send("tellExperience", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    value);
                Player.skills.channel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    value);
            }
        }

        public int Reputation
        {
            get => Player.skills.reputation;
            set => Player.skills.askRep(value);
        }

        public byte Health => Player.life.health;

        public byte Hunger
        {
            get => Player.life.food;
            set
            {
                Player.life.askEat(100);
                Player.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get => Player.life.water;
            set
            {
                Player.life.askDrink(100);
                Player.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get => Player.life.isBroken;
            set
            {
                Player.life.tellBroken(Provider.server, value);
                Player.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public bool Bleeding
        {
            get => Player.life.isBleeding;
            set
            {
                Player.life.tellBleeding(Provider.server, value);
                Player.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public bool Dead => Player.life.isDead;

        public void Heal(byte amount)
        {
            Heal(amount, null, null);
        }

        public void Heal(byte amount, bool? bleeding, bool? broken)
        {
            Player.life.askHeal(amount, bleeding ?? Player.life.isBleeding, broken ?? Player.life.isBroken);
        }

        public void Suicide()
        {
            Player.life.askSuicide(Player.channel.owner.playerID.steamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            Player.life.askDamage(amount, direction, cause, limb, damageDealer, out EPlayerKill playerKill);
            return playerKill;
        }

        public bool IsPro => Player.channel.owner.isPro;

        public InteractableVehicle CurrentVehicle => Player.movement.getVehicle();

        public bool IsInVehicle => CurrentVehicle != null;

        public int CompareTo(object obj) => Id.CompareTo(obj);

        public void SendMessage(string message)
        {
            IChatManager chat = container.Get<IChatManager>();
            chat.SendMessage(this, message);
        }

        public string Name => Player.channel.owner.playerID.playerName;
        public Type PlayerType => typeof(UnturnedPlayer);
    }
}