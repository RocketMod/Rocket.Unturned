using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour
    {
        private Assembly assembly = Assembly.GetExecutingAssembly();
        private List<Type> unturnedPlayerComponents = new List<Type>();
        
        private void OnDisable()
        {
            try
            {
                U.Events.OnPlayerConnected -= addPlayerComponents;
                unturnedPlayerComponents = unturnedPlayerComponents.Where(p => p.Assembly != assembly).ToList();
                List<Type> playerComponents = RocketHelper.GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent));
                foreach (Type playerComponent in playerComponents)
                {
                    Steam.Players.ForEach(p => p.Player.gameObject.TryRemoveComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void OnEnable()
        {
            try
            {
                U.Events.OnPlayerConnected += addPlayerComponents;
                System.Console.ForegroundColor = ConsoleColor.Cyan;

                unturnedPlayerComponents.AddRange(RocketHelper.GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent)));

                foreach (Type playerComponent in unturnedPlayerComponents)
                {
                    Steam.Players.ForEach(p => p.Player.gameObject.TryAddComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void addPlayerComponents(IRocketPlayer p)
        {
            foreach (Type component in unturnedPlayerComponents)
            {
                ((UnturnedPlayer)p).Player.gameObject.AddComponent(component);
            }
        }
    }
}