using System;
using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public static class UnturnedCommandExtensions
    {
        [Obsolete("Use GetParameter<T> instead")]
        public static UnturnedPlayer GetUnturnedPlayerParameter(this string[] array, int index)
        {
            return (array.Length <= index) ? null : UnturnedPlayer.FromName(array[index]);
        }
        /*
        public static RocketPlayer GetRocketPlayerParameter(this string[] array, int index)
        {
            if(array.Length > index)
            {
                ulong id = 0;
                if (ulong.TryParse(array[index], out id) && id > 76561197960265728)
                {
                    return new RocketPlayer(id.ToString());
                }
            }
            return null;
        }
        */

        [Obsolete("Use GetParameter<T> instead")]
        public static ulong? GetCSteamIDParameter(this string[] array, int index)
        {
            if (array.Length > index)
            {
                ulong id = 0;
                if (ulong.TryParse(array[index], out id) && id > 76561197960265728)
                {
                    return id;
                }
            }
            return null;
        }

        [Obsolete("Use GetParameter<T> instead")]
        public static Color? GetColorParameter(this string[] array, int index)
        {
            if(array.Length <= index) return null;
            Color output = ColorUtils.GetColorFromName(array[index], Color.clear);
            return (output == Color.clear) ? null : (Color?)output;
        }
    }
}
