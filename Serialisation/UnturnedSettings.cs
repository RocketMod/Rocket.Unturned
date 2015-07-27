using System.Xml.Serialization;

namespace Rocket.Unturned.Serialisation
{
    public sealed class AutomaticSave
    {
        [XmlAttribute]
        public bool Enabled = true;

        [XmlAttribute]
        public int Interval = 1800;
    }

    public class UnturnedSettings
    {
        [XmlElement("AutomaticSave")]
        public AutomaticSave AutomaticSave = new AutomaticSave();
    }
}