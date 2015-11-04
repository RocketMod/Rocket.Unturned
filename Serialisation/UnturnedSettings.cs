using Rocket.Core.Assets;
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
        [XmlElement("AutomaticSave")]
        public AutomaticSave AutomaticSave;

        [XmlElement("CharacterNameValidation")]
        public bool CharacterNameValidation = false;

        [XmlElement("CharacterNameValidationRule")]
        public string CharacterNameValidationRule = @"([^\x00-\x7F]|[\w_\ \.\+\-])+";


        public void LoadDefaults()
        {
            AutomaticSave = new AutomaticSave();
            CharacterNameValidation = true;
            CharacterNameValidationRule = @"([^\x00-\x7F]|[\w_\ \.\+\-])+";
        }
    }
}