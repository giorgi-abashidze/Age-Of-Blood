using System;

namespace enums
{
    [Serializable]
    public enum SkillType: byte{
        Active,
        Passive,
        Buff,
        DeBuff
    }
}