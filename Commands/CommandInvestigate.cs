using SDG.Unturned;
using System;
using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned.Chat;

namespace Rocket.Unturned.Commands
{
    public class CommandInvestigate : IRocketCommand
    {
        public bool AllowFromConsole
        {
            get { return true; }
        }

        public string Name
        {
            get { return "investigate"; }
        }

        public string Help
        {
            get { return "Shows you the SteamID64 of a player";}
        }

        public string Syntax
        {
            get { return "<player>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.investigate" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length!=1)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                return;
            }

            SteamPlayer otherPlayer = PlayerTool.getSteamPlayer(command[0]);
            if (otherPlayer != null && (caller == null || otherPlayer.SteamPlayerID.CSteamID.ToString() != caller.ToString()))
            {
                UnturnedChat.Say(caller, U.Translate("command_investigate_private", otherPlayer.SteamPlayerID.CharacterName, otherPlayer.SteamPlayerID.CSteamID.ToString()));
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_failed_find_player"));
            }
        }
    }
}