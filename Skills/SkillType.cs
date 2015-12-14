using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Unturned.Skills
{
    public class SkillType
    {
        public static OffenseSpecialityType OFFENSE  = new OffenseSpecialityType();
        public static DefenseSpecialityType DEFENSE  = new DefenseSpecialityType();
        public static SupportSpecialityType SUPPORT  = new SupportSpecialityType();

        internal byte SpecialityIndex;
        internal byte SkillIndex;

        internal SkillType( byte specialityIndex, byte skillIndex )
        {
            SpecialityIndex = specialityIndex;
            SkillIndex = skillIndex;
        }
    };

    public class OffenseSpecialityType
    {
        public SkillType OVERKILL       = new SkillType(0, 0);
        public SkillType SHARPSHOOTER   = new SkillType(0, 1);
        public SkillType DEXTERITY      = new SkillType(0, 2);
        public SkillType CARDIO         = new SkillType(0, 3);
        public SkillType EXERCISE       = new SkillType(0, 4);
        public SkillType DIVING         = new SkillType(0, 5);
        public SkillType PARKOUR        = new SkillType(0, 6);
    };

    public class DefenseSpecialityType
    {
        public SkillType SNEAKYBEAKY    = new SkillType(1, 0);
        public SkillType VITALITY       = new SkillType(1, 1);
        public SkillType IMMUNITY       = new SkillType(1, 2);
        public SkillType TOUGHNESS      = new SkillType(1, 3);
        public SkillType STRENGTH       = new SkillType(1, 4);
        public SkillType WARMBLOODED    = new SkillType(1, 5);
        public SkillType SURVIVAL       = new SkillType(1, 6);
    };

    public class SupportSpecialityType
    {
        public SkillType HEALING        = new SkillType(2, 0);
        public SkillType CRAFTING       = new SkillType(2, 1);
        public SkillType OUTDOORS       = new SkillType(2, 2);
        public SkillType COOKING        = new SkillType(2, 3);
        public SkillType FISHING        = new SkillType(2, 4);
        public SkillType AGRICULTURE    = new SkillType(2, 5);
        public SkillType MECHANIC       = new SkillType(2, 6);
        public SkillType ENGINEER       = new SkillType(2, 7);
    };
}
