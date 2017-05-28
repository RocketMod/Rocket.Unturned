using System;
using Rocket.Core;
using Rocket.Core.Utils.Web;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public class UnturnedPlayerMovement : UnturnedPlayerComponent
    {
        public bool VanishMode = false;
        DateTime lastUpdate = DateTime.Now;
        Vector3 lastVector = new Vector3(0,-1,0);

        DateTime? requested;
        string webClientResult;

        private void OnEnable()
        {
            if (U.Instance.Settings.Instance.RocketModObservatory.CommunityBans)
            {
                using (RocketWebClient webClient = new RocketWebClient())
                {
                    try
                    {
                        webClient.DownloadStringCompleted += (sender, e) =>
                        {
                            if (e.Error == null)
                            {
                                if (e.Result.Contains(","))
                                {
                                    string[] result = e.Result.Split(',');
                                    long age;
                                    if (result[0] == "true")
                                    {
                                        R.Logger.Info("[RocketMod Observatory] Kicking Player " + Player.DisplayName + "because he is banned:" + result[1]);
                                        webClientResult = result[1];
                                        requested = DateTime.Now;
                                        Player.Kick("you are banned from observatory: " + result[1]);
                                    }
                                    else if (U.Instance.Settings.Instance.RocketModObservatory.KickLimitedAccounts && result.Length >= 2 && result[1] == "true")
                                    {
                                        R.Logger.Info("[RocketMod Observatory] Kicking Player " + Player.DisplayName + " because his account is limited");
                                        Player.Kick("your Steam account is limited");
                                    }
                                    else if (U.Instance.Settings.Instance.RocketModObservatory.KickTooYoungAccounts && result.Length == 3 && long.TryParse(result[2].ToString(), out age))
                                    {
                                        long epochTicks = new DateTime(1970, 1, 1).Ticks;
                                        long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
                                        long d = (unixTime - age);
                                        if (d < U.Instance.Settings.Instance.RocketModObservatory.MinimumAge)
                                        {
                                            R.Logger.Info("[RocketMod Observatory] Kicking Player " + Player.DisplayName + " because his account is younger then " + U.Instance.Settings.Instance.RocketModObservatory.MinimumAge + " seconds (" + d + " seconds)");
                                            Player.Kick("your Steam account is not old enough");
                                        }
                                    }
                                }
                            }
                        };
                        webClient.DownloadStringAsync(new Uri(string.Format("http://banlist.observatory.rocketmod.net/?steamid={0}", Player.CSteamID)));
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (requested.HasValue && (DateTime.Now - requested.Value).TotalSeconds >= 2){
                Provider.kick(Player.CSteamID, webClientResult);
                requested = null;
            }

            PlayerMovement movement = (PlayerMovement)Player.GetComponent<PlayerMovement>();

            if (!VanishMode)
            {
                if (U.Instance.Settings.Instance.LogSuspiciousPlayerMovement && lastUpdate.AddSeconds(1) < DateTime.Now)
                {
                    lastUpdate = DateTime.Now;

                    Vector3 positon = movement.real;

                    if (lastVector.y != -1)
                    {
                        float x = Math.Abs(lastVector.x - positon.x);
                        float y = positon.y - lastVector.y;
                        float z = Math.Abs(lastVector.z - positon.z);
                        if (y > 15)
                        {
                            RaycastHit raycastHit = new RaycastHit();
                            Physics.Raycast(positon, Vector3.down, out raycastHit);
                            Vector3 floor = raycastHit.point;
                            float distance = Math.Abs(floor.y - positon.y);
                            R.Logger.Info(Player.DisplayName + " moved x:" + positon.x + " y:" + positon.y + "(+" + y + ") z:" + positon.z + " in the last second (" + distance + ")");
                        }
                    }
                    lastVector = movement.real;
                }
            }
        }
    }
}
