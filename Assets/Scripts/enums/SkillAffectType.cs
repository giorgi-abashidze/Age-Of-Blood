using System;

namespace enums
{
    [Serializable]
    public enum SkillAffectType: byte{
        Increase,
        Decrease,
        Heal,
        Block,
        Vamp
    }
}