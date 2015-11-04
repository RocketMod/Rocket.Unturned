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
        public bool CharacterNameValidation;

        [XmlElement("CharacterNameValidationRule")]
        public string CharacterNameValidationRule;


        public void LoadDefaults()
        {
            AutomaticSave = new AutomaticSave();
            CharacterNameValidation = false;
            CharacterNameValidationRule = @"([^\x00-\x7F]|[\w_\ \.\+\-])+";
        }
    }
}