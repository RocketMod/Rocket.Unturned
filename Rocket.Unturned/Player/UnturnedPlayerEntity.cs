using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.UnityEngine.Extensions;
using SDG.Unturned;
using Steamworks;
using Vector3 = UnityEngine.Vector3;

namespace Rocket.Unturned.Player {
    public class UnturnedPlayerEntity : IPlayerEntity<UnturnedPlayer>, ILivingEntity
    {
        public UnturnedPlayerEntity(UnturnedPlayer unturnedPlayer)
        {
            Player = unturnedPlayer;
        }

        public System.Numerics.Vector3 Position => Player.NativePlayer?.transform?.position.ToSystemVector() ?? throw new PlayerNotOnlineException();

        public async Task<bool> TeleportAsync(System.Numerics.Vector3 position, float rotation)
        {
            return Teleport(position, rotation);
        }

        public string EntityTypeName => "Player";


        public EPlayerStance Stance => Player.NativePlayer.stance.stance;

        public float Rotation => Player.NativePlayer.transform.rotation.eulerAngles.y;

        public byte Stamina => Player.NativePlayer.life.stamina;

        public byte Infection
        {
            get => Player.NativePlayer.life.virus;
            set
            {
                Player.NativePlayer.life.askDisinfect(100);
                Player.NativePlayer.life.askInfect(value);
            }
        }

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.nodes.FirstOrDefault(n
                => n.type == ENodeType.LOCATION
                    && ((LocationNode)n).name.ToLower().Contains(nodeName));
            if (node != null)
            {
                Vector3 c = node.point + new Vector3(0f, 0.5f, 0f);
                Player.NativePlayer.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }

            return false;
        }


        public uint Experience
        {
            get => Player.NativePlayer.skills.experience;
            set
            {
                Player.NativePlayer.skills.channel.send("tellExperience", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    value);
                Player.NativePlayer.skills.channel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    value);
            }
        }

        public int Reputation
        {
            get => Player.NativePlayer.skills.reputation;
            set => Player.NativePlayer.skills.askRep(value);
        }

        public Task KillAsync() => throw new NotImplementedException();

        public Task KillAsync(IEntity killer) => throw new NotImplementedException();

        public Task KillAsync(IUser killer) => throw new NotImplementedException();

        public double MaxHealth { get; } = 100;

        public double Health
        {
            get { return Player.NativePlayer.life.health; }
            set
            {
                if (value > 255)
                {
                    value = 255;
                }

                byte healthToSet = (byte) value;
                byte amountToHeal = (byte)(Player.NativePlayer.life.health - healthToSet);

                Player.NativePlayer.life.askHeal(amountToHeal, false, false);
            }
        }


        public byte Hunger
        {
            get => Player.NativePlayer.life.food;
            set
            {
                Player.NativePlayer.life.askEat(100);
                Player.NativePlayer.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get => Player.NativePlayer.life.water;
            set
            {
                Player.NativePlayer.life.askDrink(100);
                Player.NativePlayer.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get => Player.NativePlayer.life.isBroken;
            set
            {
                Player.NativePlayer.life.tellBroken(Provider.server, value);
                Player.NativePlayer.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public bool Bleeding
        {
            get => Player.NativePlayer.life.isBleeding;
            set
            {
                Player.NativePlayer.life.tellBleeding(Provider.server, value);
                Player.NativePlayer.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public bool Dead => Player.NativePlayer.life.isDead;

        public void Heal(byte amount)
        {
            Heal(amount, null, null);
        }

        public void Heal(byte amount, bool? bleeding, bool? broken)
        {
            Player.NativePlayer.life.askHeal(amount, bleeding ?? Player.NativePlayer.life.isBleeding, broken ?? Player.NativePlayer.life.isBroken);
        }

        public void Suicide()
        {
            Player.NativePlayer.life.askSuicide(Player.CSteamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            Player.NativePlayer.life.askDamage(amount, direction, cause, limb, damageDealer, out EPlayerKill playerKill);
            return playerKill;
        }

        public EPlayerKill Damage(byte amount, System.Numerics.Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            return Damage(amount, direction.ToUnityVector(), cause, limb, damageDealer);
        }

        public void Teleport(UnturnedPlayer target)
        {
            var targetPos = target.Entity.Position;
            var rotation = target.Entity.Rotation;

            Teleport(targetPos, MeasurementTool.angleToByte(rotation));
        }

        public bool Teleport(System.Numerics.Vector3 position, float rotation)
        {
            if (EnableVanish)
            {
                Player.NativePlayer.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                Player.NativePlayer.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.X, position.Y + 1337, position.Z), MeasurementTool.angleToByte(rotation));
                Player.NativePlayer.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
                Player.NativePlayer.channel.send("askTeleport", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, position.ToUnityVector(), MeasurementTool.angleToByte(Rotation));
            }
            return true;
        }

        public bool EnableVanish { get; set; } = false;

        public bool Teleport(System.Numerics.Vector3 position)
        {
            return Teleport(position, Rotation);
        }

        public UnturnedPlayer Player { get; }
    }
}