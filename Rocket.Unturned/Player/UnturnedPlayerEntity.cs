using System;
using System.Linq;
using System.Numerics;
using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.UnityEngine.Extensions;
using SDG.Unturned;
using Steamworks;
using Vector3 = UnityEngine.Vector3;

namespace Rocket.Unturned.Player {
    public class UnturnedPlayerEntity : IPlayerEntity//, ILivingEntity
    {
        public UnturnedPlayer UnturnedPlayer { get; }

        public UnturnedPlayerEntity(UnturnedPlayer unturnedPlayer)
        {
            this.UnturnedPlayer = unturnedPlayer;
        }

        public System.Numerics.Vector3 Position => UnturnedPlayer.NativePlayer?.transform?.position.ToSystemVector() ?? throw new PlayerNotOnlineException();

        public string EntityTypeName => "Player";


        public EPlayerStance Stance => UnturnedPlayer.NativePlayer.stance.stance;

        public float Rotation => UnturnedPlayer.NativePlayer.transform.rotation.eulerAngles.y;

        public byte Stamina => UnturnedPlayer.NativePlayer.life.stamina;

        public byte Infection
        {
            get => UnturnedPlayer.NativePlayer.life.virus;
            set
            {
                UnturnedPlayer.NativePlayer.life.askDisinfect(100);
                UnturnedPlayer.NativePlayer.life.askInfect(value);
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
                UnturnedPlayer.NativePlayer.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }

            return false;
        }


        public uint Experience
        {
            get => UnturnedPlayer.NativePlayer.skills.experience;
            set
            {
                UnturnedPlayer.NativePlayer.skills.channel.send("tellExperience", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    value);
                UnturnedPlayer.NativePlayer.skills.channel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                    value);
            }
        }

        public int Reputation
        {
            get => UnturnedPlayer.NativePlayer.skills.reputation;
            set => UnturnedPlayer.NativePlayer.skills.askRep(value);
        }

        public double Health
        {
            get { return UnturnedPlayer.NativePlayer.life.health; }
            set => throw new NotImplementedException();
        }


        public byte Hunger
        {
            get => UnturnedPlayer.NativePlayer.life.food;
            set
            {
                UnturnedPlayer.NativePlayer.life.askEat(100);
                UnturnedPlayer.NativePlayer.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get => UnturnedPlayer.NativePlayer.life.water;
            set
            {
                UnturnedPlayer.NativePlayer.life.askDrink(100);
                UnturnedPlayer.NativePlayer.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get => UnturnedPlayer.NativePlayer.life.isBroken;
            set
            {
                UnturnedPlayer.NativePlayer.life.tellBroken(Provider.server, value);
                UnturnedPlayer.NativePlayer.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public bool Bleeding
        {
            get => UnturnedPlayer.NativePlayer.life.isBleeding;
            set
            {
                UnturnedPlayer.NativePlayer.life.tellBleeding(Provider.server, value);
                UnturnedPlayer.NativePlayer.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public bool Dead => UnturnedPlayer.NativePlayer.life.isDead;

        public void Heal(byte amount)
        {
            Heal(amount, null, null);
        }

        public void Heal(byte amount, bool? bleeding, bool? broken)
        {
            UnturnedPlayer.NativePlayer.life.askHeal(amount, bleeding ?? UnturnedPlayer.NativePlayer.life.isBleeding, broken ?? UnturnedPlayer.NativePlayer.life.isBroken);
        }

        public void Suicide()
        {
            UnturnedPlayer.NativePlayer.life.askSuicide(UnturnedPlayer.CSteamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            UnturnedPlayer.NativePlayer.life.askDamage(amount, direction, cause, limb, damageDealer, out EPlayerKill playerKill);
            return playerKill;
        }

        public EPlayerKill Damage(byte amount, System.Numerics.Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            return Damage(amount, direction.ToUnityVector(), cause, limb, damageDealer);
        }

        public void Teleport(UnturnedPlayer target)
        {
            var targetPos = target.Entity.Position;
            var rotation = ((UnturnedPlayerEntity)target.Entity).Rotation;

            Teleport(targetPos, MeasurementTool.angleToByte(rotation));
        }

        public bool Teleport(System.Numerics.Vector3 position, float rotation)
        {
            if (EnableVanish)
            {
                UnturnedPlayer.NativePlayer.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                UnturnedPlayer.NativePlayer.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.X, position.Y + 1337, position.Z), MeasurementTool.angleToByte(rotation));
                UnturnedPlayer.NativePlayer.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
                UnturnedPlayer.NativePlayer.channel.send("askTeleport", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, position.ToUnityVector(), MeasurementTool.angleToByte(Rotation));
            }
            return true;
        }

        public bool EnableVanish { get; set; } = false;

        public bool Teleport(System.Numerics.Vector3 position)
        {
            return Teleport(position, Rotation);
        }

        public IPlayer Player => UnturnedPlayer;
    }
}