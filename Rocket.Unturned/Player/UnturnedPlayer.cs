using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.Unturned.Events;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Skills;
using Rocket.API.Serialisation;
using Rocket.Core.Player;

namespace Rocket.Unturned.Player
{
    public class PlayerIsConsoleException : Exception { }

    public sealed class UnturnedPlayer : RocketPlayerBase
    {
        public SDG.Unturned.Player Player { get; }

        public Exception PlayerIsConsoleException;

        private UnturnedPlayer(SteamPlayer player) : base(player.playerID.steamID.ToString(), player.playerID.characterName, player.isAdmin)
        {
            Player = player.player;
        }

        public Color Color
        {
            get
            {
                if (Features.Color.HasValue)
                {
                    return Features.Color.Value;
                }
                if (IsAdmin)
                {
                    return Palette.ADMIN;
                }

                RocketPermissionsGroup group = R.Permissions.GetGroups(this).FirstOrDefault(g => g.Properties[BuiltinProperties.COLOR] != null && g.Properties[BuiltinProperties.COLOR] != "white");
                string color = "";
                if (group != null) color = group.Properties[BuiltinProperties.COLOR];
                return UnturnedChat.GetColorFromName(color, Palette.COLOR_W);
            }
            set
            {
                Features.Color = value;
            }
        }

        public float Ping => Player.channel.owner.ping;

        public bool Equals(UnturnedPlayer p)
        {
            if ((object)p == null)
            {
                return false;
            }

            return (this.CSteamID.ToString() == p.CSteamID.ToString());
        }

        public CSteamID CSteamID => Player.channel.owner.playerID.steamID;

        public T GetComponent<T>()
        {
            return (T)(object)Player.GetComponent(typeof(T));
        }

        public static UnturnedPlayer FromName(string name)
        {
            if (String.IsNullOrEmpty(name)) return null;
            SDG.Unturned.Player p = null;
            ulong id = 0;
            if (ulong.TryParse(name, out id) && id > 76561197960265728)
            {
                p = PlayerTool.getPlayer(new CSteamID(id));
            }
            else
            {
                p = PlayerTool.getPlayer(name);
            }
            if (p == null) return null;

            throw new NotImplementedException(); // note: do NOT return new instance, safe the player somewhere and return them
        }

        public static UnturnedPlayer FromCSteamID(CSteamID cSteamID)
        {
            throw new NotImplementedException(); // note: do NOT return new instance, safe the player somewhere and return them
        }

        public static UnturnedPlayer FromPlayer(SDG.Unturned.Player player)
        {
            throw new NotImplementedException(); // note: do NOT return new instance, safe the player somewhere and return them
        }

        public static UnturnedPlayer FromSteamPlayer(SteamPlayer player)
        {
            throw new NotImplementedException(); // note: do NOT return new instance, safe the player somewhere and return them
        }

        public UnturnedPlayerFeatures Features => Player.gameObject.transform.GetComponent<UnturnedPlayerFeatures>();

        public UnturnedPlayerEvents Events => Player.gameObject.transform.GetComponent<UnturnedPlayerEvents>();

        public override string ToString()
        {
            return CSteamID.ToString();
        }

        public void TriggerEffect(ushort effectId)
        {
            EffectManager.instance.channel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, effectId, Player.transform.position);
        }

        public PlayerInventory Inventory => Player.inventory;

        public bool GiveItem(ushort itemId, byte amount)
        {
            return ItemTool.tryForceGiveItem(Player, itemId, amount);
        }

        public bool GiveItem(Item item)
        {
            return Player.inventory.tryAddItem(item, false);
        }

        public bool GiveVehicle(ushort vehicleId)
        {
            return VehicleTool.giveVehicle(Player, vehicleId);
        }

        public CSteamID SteamGroupID => Player.channel.owner.playerID.@group;

        public override void Kick(string reason)
        {
            Provider.kick(this.CSteamID, reason);
        }

        public override void Ban(string reason, uint duration)
        {
            Provider.ban(this.CSteamID, reason, duration);
        }

        public void Admin(bool admin)
        {
            Admin(admin, null);
        }

        public void Admin(bool admin, UnturnedPlayer issuer)
        {
            if (admin)
            {
                if (issuer == null)
                {
                    SteamAdminlist.admin(this.CSteamID, new CSteamID(0));
                }
                else
                {
                    SteamAdminlist.admin(this.CSteamID, issuer.CSteamID);
                }
            }
            else
            {
                SteamAdminlist.unadmin(Player.channel.owner.playerID.steamID);
            }
        }

        public void Teleport(UnturnedPlayer target)
        {
            Vector3 d1 = target.Player.transform.position;
            Vector3 vector31 = target.Player.transform.rotation.eulerAngles;
            Teleport(d1, MeasurementTool.angleToByte(vector31.y));
        }

        public void Teleport(Vector3 position, float rotation)
        {
            if (VanishMode)
            {
                Player.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                Player.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.y, position.y + 1337, position.z), MeasurementTool.angleToByte(rotation));
                Player.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
                Player.channel.send("askTeleport", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
        }

        public bool VanishMode
        {
            get
            {
                UnturnedPlayerFeatures features = Player.GetComponent<UnturnedPlayerFeatures>();
                return features.VanishMode;
            }
            set
            {
                UnturnedPlayerFeatures features = Player.GetComponent<UnturnedPlayerFeatures>();
                features.VanishMode = value;
            }
        }

        public bool GodMode
        {
            get
            {
                UnturnedPlayerFeatures features = Player.GetComponent<UnturnedPlayerFeatures>();
                return features.GodMode;
            }
            set
            {
                UnturnedPlayerFeatures features = Player.GetComponent<UnturnedPlayerFeatures>();
                features.GodMode = value;
            }
        }

        public Vector3 Position => Player.transform.position;

        public EPlayerStance Stance => Player.stance.stance;

        public float Rotation => Player.transform.rotation.eulerAngles.y;

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.nodes.FirstOrDefault(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(nodeName));
            if (node != null)
            {
                Vector3 c = node.point + new Vector3(0f, 0.5f, 0f);
                Player.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }
            return false;
        }

        public byte Stamina => Player.life.stamina;

        public string SteamName => Player.channel.owner.playerID.playerName;

        public byte Infection
        {
            get
            {
                return Player.life.virus;
            }
            set
            {
                Player.life.askDisinfect(100);
                Player.life.askInfect(value);
            }
        }

        public uint Experience
        {
            get
            {
                return Player.skills.experience;
            }
            set
            {
                Player.skills.askAward(value);
            }
        }

        public int Reputation
        {
            get
            {
                return Player.skills.reputation;
            }
            set
            {
                Player.skills.askRep(value);
            }
        }

        public byte Health => Player.life.health;

        public byte Hunger
        {
            get
            {
                return Player.life.food;
            }
            set
            {
                Player.life.askEat(100);
                Player.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get
            {
                return Player.life.water;
            }
            set
            {
                Player.life.askDrink(100);
                Player.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get
            {
                return Player.life.isBroken;
            }
            set
            {
                Player.life.tellBroken(Provider.server, value);
                Player.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }
        public bool Bleeding
        {
            get
            {
                return Player.life.isBleeding;
            }
            set
            {
                Player.life.tellBleeding(Provider.server, value);
                Player.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
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
            EPlayerKill playerKill;
            Player.life.askDamage(amount, direction, cause, limb, damageDealer, out playerKill);
            return playerKill;
        }

        public bool IsPro => Player.channel.owner.isPro;

        public InteractableVehicle CurrentVehicle => Player.movement.getVehicle();

        public bool IsInVehicle => CurrentVehicle != null;

        public void SetSkillLevel(UnturnedSkill skill, byte level)
        {
            GetSkill(skill).level = level;
            Player.skills.askSkills(CSteamID);
        }

        public byte GetSkillLevel(UnturnedSkill skill)
        {
            return GetSkill(skill).level;
        }

        public Skill GetSkill(UnturnedSkill skill)
        {
            var skills = Player.skills;
            return skills.skills[skill.Speciality][skill.Skill];
        }        
    }
}
