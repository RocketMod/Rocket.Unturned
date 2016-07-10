using Rocket.API;
using System.Xml.Serialization;
using System;

namespace Rocket.Unturned.Serialisation
{
    public sealed class AutomaticSave
    {
        [XmlAttribute]
        public bool Enabled = true;

        [XmlAttribute]
        public int Interval = 1800;
    }

    public class UnturnedSettings : IDefaultable
    {
        [XmlElement("CommunityBans")]
        public bool CommunityBans = true;

        [XmlElement("AutomaticSave")]
        public AutomaticSave AutomaticSave = new AutomaticSave();

        [XmlElement("CharacterNameValidation")]
        public bool CharacterNameValidation = false;

        [XmlElement("CharacterNameValidationRule")]
        public string CharacterNameValidationRule = @"([\x00-\xAA]|[\w_\ \.\+\-])+";

        public bool LogSuspiciousPlayerMovement = true;

        public void LoadDefaults()
        {
            AutomaticSave = new AutomaticSave();
            CommunityBans = true;
            CharacterNameValidation = true;
            CharacterNameValidationRule = @"([\x00-\xAA]|[\w_\ \.\+\-])+";
            LogSuspiciousPlayerMovement = true;
        }
    }
}