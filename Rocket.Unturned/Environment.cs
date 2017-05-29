using System.IO;

namespace Rocket.Unturned
{
    public static class Environment
    {
        static Environment()
        {
            RocketDirectory = string.Format("Servers/{0}/Rocket/", U.Instance.InstanceName);
            if (!Directory.Exists(RocketDirectory)) Directory.CreateDirectory(RocketDirectory);
            Directory.SetCurrentDirectory(RocketDirectory);
        }

        public static readonly string RocketDirectory;

        public static readonly string SettingsFile = "Rocket.Unturned.config.xml";
        public static readonly string TranslationFile = "Rocket.Unturned.{0}.translation.xml";
    }
}