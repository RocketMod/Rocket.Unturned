//using Rocket.API;
//using Rocket.API.Extensions;
//using Rocket.Unturned.Events;
//using Rocket.Unturned.Player;
//using SDG.Unturned;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Rocket.API.Player;
//using Rocket.API.Providers.Plugins;
//using Rocket.Core;
//using UnityEngine;

//namespace Rocket.Unturned.Plugins
//{
//    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour
//    {
//        private Assembly assembly;
//        private List<Type> unturnedPlayerComponents = new List<Type>();
        
//        private void OnDisable()
//        {
//            try
//            {
//                U.Instance.OnPlayerConnected -= addPlayerComponents;
//                unturnedPlayerComponents = unturnedPlayerComponents.Where(p => p.Assembly != assembly).ToList();
//                List<Type> playerComponents = assembly.GetTypesFromParentClass(typeof(UnturnedPlayerComponent));
//                foreach (Type playerComponent in playerComponents)
//                {
//                    Provider.clients.ForEach(p => p.player.gameObject.TryRemoveComponent(playerComponent.GetType()));
//                }
//            }
//            catch (Exception ex)
//            {
//                R.Logger.Error(ex);
//            }
//        }

//        private void OnEnable()
//        {
//            try
//            {  
//                IRocketPlugin plugin = GetComponent<IRocketPlugin>();
//                assembly = plugin.GetType().Assembly;

//                U.Instance.OnBeforePlayerConnected += addPlayerComponents;
//                unturnedPlayerComponents.AddRange(assembly.GetTypesFromParentClass(typeof(UnturnedPlayerComponent)));

//                foreach (Type playerComponent in unturnedPlayerComponents)
//                {
//                    Logger.Info("Adding UnturnedPlayerComponent: "+playerComponent.Name);
//                    //Provider.Players.ForEach(p => p.Player.gameObject.TryAddComponent(playerComponent.GetType()));
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.Error(ex);
//            }
//        }

//        private void addPlayerComponents(IRocketPlayer p)
//        {
//            foreach (Type component in unturnedPlayerComponents)
//            {
//                ((UnturnedPlayer)p).Player.gameObject.AddComponent(component);
//            }
//        }
//    }
//}