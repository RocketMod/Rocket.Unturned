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
        private bool _enabled;
        private void OnDisable()
        {
            try
            {
                _enabled = false;
                unturnedPlayerComponents = unturnedPlayerComponents.Where(p => p.Assembly != assembly).ToList();
                List<Type> playerComponents = GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent));
                foreach (Type playerComponent in playerComponents)
                {
                    Provider.clients.ForEach(p => p.player.gameObject.TryRemoveComponent(playerComponent.GetType()));
                }

                EventManager.Instance.UnregisterEventsInternal(this, null);
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
                unturnedPlayerComponents.AddRange(GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent)));

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

        private List<Type> GetTypesFromParentClass(Assembly assembly, Type parentClass)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.IsSubclassOf(parentClass))
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
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