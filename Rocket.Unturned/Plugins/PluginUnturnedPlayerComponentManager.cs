using Rocket.API.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.Event;
using Rocket.API.Event.Player;
using Rocket.API.Player;
using Rocket.API.Providers.Plugins;
using Rocket.Core;
using UnityEngine;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour, IListener
    {
        private Assembly assembly;
        private List<Type> unturnedPlayerComponents = new List<Type>();
        private bool _enabled = false;
        private void OnDisable()
        {
            try
            {
                _enabled = false;
                unturnedPlayerComponents = unturnedPlayerComponents.Where(p => p.Assembly != assembly).ToList();
                List<Type> playerComponents = assembly.GetTypesFromParentClass(typeof(UnturnedPlayerComponent));
                foreach (Type playerComponent in playerComponents)
                {
                    Provider.clients.ForEach(p => p.player.gameObject.TryRemoveComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                R.Logger.Error(ex);
            }
        }

        private void OnEnable()
        {
            try
            {
                _enabled = true;
                IRocketPlugin plugin = GetComponent<IRocketPlugin>();
                assembly = plugin.GetType().Assembly;
                EventManager.Instance.RegisterEventsInternal(this, null);
                unturnedPlayerComponents.AddRange(assembly.GetTypesFromParentClass(typeof(UnturnedPlayerComponent)));

                foreach (Type playerComponent in unturnedPlayerComponents)
                {
                    R.Logger.Info("Adding UnturnedPlayerComponent: "+playerComponent.Name);
                    //Provider.Players.ForEach(p => p.Player.gameObject.TryAddComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                R.Logger.Error(ex);
            }
        }

        [API.Event.EventHandler]
        public void OnPreConnect(PrePlayerConnectedEvent @event)
        {
            addPlayerComponents(@event.Player);
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