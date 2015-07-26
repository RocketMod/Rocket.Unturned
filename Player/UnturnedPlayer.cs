using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.Unturned.Events;
using Rocket.API;

namespace Rocket.Unturned.Player
{
    public class PlayerIsConsoleException : Exception { }

    public sealed class UnturnedPlayer : IRocketPlayer
    {

        public string Id
        {
            get
            {
                return CSteamID.ToString();
            }
        }

        public string DisplayName
        {
            get
            {
                return CharacterName;
            }
        }

        private SDG.Unturned.Player player;
        public SDG.Unturned.Player Player
        {
            get { return player; }
        }

        public CSteamID CSteamID {
            get { return player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID;  }
        }

        public Exception PlayerIsConsoleException;

        private UnturnedPlayer(SteamPlayer player)
        {
            this.player = player.player;
        }

        private UnturnedPlayer(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                throw new PlayerIsConsoleException();
            }
            else
            {
                player = PlayerTool.getPlayer(cSteamID);
            }
        }

        public float Ping
        {
            get
            {
                return player.SteamChannel.SteamPlayer.ping;
            }
        }

        public bool Equals(UnturnedPlayer p)
        {
            if ((object)p == null)
            {
                return false;
            }

            return (this.CSteamID.ToString() == p.CSteamID.ToString());
        }

        public T GetComponent<T>() {
            return (T)(object)Player.GetComponent(typeof(T));
        }

        private UnturnedPlayer(SDG.Unturned.Player p)
        {
            player = p;
        }

        public static UnturnedPlayer FromName(string name)
        {
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
            return new UnturnedPlayer(p);
        }

        public static UnturnedPlayer FromCSteamID(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                return null;
            }
            else
            {
                return new UnturnedPlayer(cSteamID);
            }
        }

        public static UnturnedPlayer FromPlayer(SDG.Unturned.Player player)
        {
            return new UnturnedPlayer(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID);
        }

        public static UnturnedPlayer FromSteamPlayer(SteamPlayer player)
        {
            return new UnturnedPlayer(player.SteamPlayerID.CSteamID);
        }

        public UnturnedPlayerFeatures Features
        {
            get { return player.gameObject.transform.GetComponent<UnturnedPlayerFeatures>(); }
        }

        public UnturnedPlayerEvents Events
        {
            get { return player.gameObject.transform.GetComponent<UnturnedPlayerEvents>(); }
        }

        public override string ToString()
        {
            return CSteamID.ToString();
        }
        
        public void TriggerEffect(ushort effectID){
            SDG.Unturned.EffectManager.Instance.SteamChannel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UDP_BUFFER, new object[] { effectID, player.transform.position });
        }

        public PlayerInventory Inventory
        {
            get { return player.Inventory; }
        }

        public bool GiveItem(ushort itemId, byte amount)
        {
            return ItemTool.tryForceGiveItem(player,itemId,amount);
        }

        public bool GiveItem(Item item)
        {
            return player.Inventory.tryAddItem(item, false);
        }

        public bool GiveVehicle(ushort vehicleId)
        {
            return VehicleTool.giveVehicle(player, vehicleId);
        }

        //public void GiveZombie(byte amount)
        //{
        //    ZombieTool.giveZombie(player,amount);
        //}

        public CSteamID SteamGroupID
        {
            get
            {
                return player.SteamChannel.SteamPlayer.SteamPlayerID.SteamGroupID;
            }
        }

        public void Kick(string reason)
        {
            Steam.kick(this.CSteamID, reason);
        }

        public void Ban(string reason, uint duration)
        {
            Steam.ban(this.CSteamID, reason, duration);
        }

        public bool IsAdmin
        {
            get { 
                return player.SteamChannel.SteamPlayer.IsAdmin; 
            }
        }
		
		public void Admin(bool admin){
			Admin (admin, null);
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
                SteamAdminlist.unadmin(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID);
            }
        }

        public void Teleport(UnturnedPlayer target)
        {
            Vector3 d1 = target.player.transform.position;
            Vector3 vector31 = target.player.transform.rotation.eulerAngles;
            player.sendTeleport(d1, MeasurementTool.angleToByte(vector31.y));
        }

        public void Teleport(Vector3 position ,float rotation)
        {
            player.sendTeleport(position, MeasurementTool.angleToByte(rotation));
        }

        public Vector3 Position
        {
            get
            {
                return player.transform.position;
            }
        }

        public EPlayerStance Stance
        {
            get
            {
                return player.Stance.Stance;
            }
        }

        public float Rotation { 
            get { 
                return player.transform.rotation.eulerAngles.y; 
            } 
        }

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.Nodes.Where(n => n.NodeType == ENodeType.LOCATION && ((LocationNode)n).Name.ToLower().Contains(nodeName)).FirstOrDefault();
            if (node != null)
            {
                Vector3 c = node.Position + new Vector3(0f, 0.5f, 0f);
                player.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }
            return false;
        }

        public byte Stamina
        {
            get
            {
                return player.PlayerLife.Stamina;
            }
        }

        public string CharacterName
        {
            get
            {
                return player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName;
            }
        }

        public string SteamName
        {
            get
            {
                return player.SteamChannel.SteamPlayer.SteamPlayerID.SteamName;
            }
        }

        public byte Infection
        {
            get { 
                return player.PlayerLife.Infection; 
            }
            set{
                player.PlayerLife.askDisinfect(100);
                player.PlayerLife.askInfect(value);
            }
        }

        public uint Experience
        {
            get
            {
                return player.Skills.Experience;
            }
            set
            {
                player.Skills.Experience = value;
                player.SteamChannel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_TCP_BUFFER, new object[] { value });
            }
        }

        public byte Health
        {
            get {
                return player.PlayerLife.health; 
            }
        }

        public byte Hunger
        {
            get{
                return player.PlayerLife.Hunger;
            }
            set
            {
                player.PlayerLife.askEat(100);
                player.PlayerLife.askStarve(value);
            }
        }

        public byte Thirst
        {
            get { 
                return player.PlayerLife.Thirst; 
            }
            set
            {
                player.PlayerLife.askDrink(100);
                player.PlayerLife.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get { 
                return player.PlayerLife.Broken; 
            }
            set
            {
                player.PlayerLife.SteamChannel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_TCP_BUFFER, new object[] { value });
            }
        }
        public bool Bleeding
        {
            get{
                return player.PlayerLife.Bleeding; 
            }
            set
            {
                player.PlayerLife.Bleeding = value;
                player.PlayerLife.SteamChannel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_TCP_BUFFER, new object[] { value });
            }
        }

        public bool Dead
        {
            get { 
                return player.PlayerLife.Dead; 
            }
        }

        public bool Freezing
        {
            get { 
                return player.PlayerLife.Freezing; 
            }
        }

		public void Heal(byte amount)
		{
			Heal (amount, null, null);
		}

        public void Heal(byte amount, bool? bleeding , bool? broken)
        {
            player.PlayerLife.askHeal(amount, bleeding != null ? bleeding.Value : player.PlayerLife.Bleeding, broken != null ? broken.Value : player.PlayerLife.Broken);
        }

        public void Suicide()
        {
            player.PlayerLife.askSuicide(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            EPlayerKill playerKill;
            player.PlayerLife.askDamage(amount, direction, cause, limb, damageDealer, out playerKill);
            return playerKill;
        }

        public bool IsPro
        {
            get
            {
                return player.SteamChannel.SteamPlayer.IsPro;
            }

        }
    }
}