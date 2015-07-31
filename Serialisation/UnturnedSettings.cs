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

        public void LoadDefaults()
        {
            AutomaticSave = new AutomaticSave();
        }
    }
}