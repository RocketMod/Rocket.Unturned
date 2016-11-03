using Rocket.API;
using System.Xml.Serialization;
using System;

namespace Rocket.Unturned
{
    public sealed class AutomaticSave
    {
        [XmlAttribute]
        public bool Enabled = true;

        [XmlAttribute]
        public int Interval = 1800;
    }

    public sealed class RocketModObservatorySettings
    {
        [XmlAttribute]
        public bool CommunityBans = true;

        [XmlAttribute]
        public bool KickLimitedAccounts = false;

        [XmlAttribute]
        public bool KickTooYoungAccounts = false;

        [XmlAttribute]
        public long MinimumAge = 604800;
    }

    public class UnturnedSettings : IDefaultable
    {
        [XmlElement("RocketModObservatory")]
        public RocketModObservatorySettings RocketModObservatory = new RocketModObservatorySettings();

        [XmlElement("AutomaticSave")]
        public AutomaticSave AutomaticSave = new AutomaticSave();

        [XmlElement("CharacterNameValidation")]
        public bool CharacterNameValidation = false;

        [XmlElement("CharacterNameValidationRule")]
        public string CharacterNameValidationRule = @"([\x00-\xAA]|[\w_\ \.\+\-])+";

        public bool LogSuspiciousPlayerMovement = true;

        public bool Debug = false;

        public void LoadDefaults()
        {
            AutomaticSave = new AutomaticSave();
            RocketModObservatory = new RocketModObservatorySettings();
            Debug = false;
            CharacterNameValidation = true;
            CharacterNameValidationRule = @"([\x00-\xAA]|[\w_\ \.\+\-])+";
            LogSuspiciousPlayerMovement = true;
        }
    }
}