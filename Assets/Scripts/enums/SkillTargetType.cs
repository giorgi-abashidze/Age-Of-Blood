using System;

namespace enums
{
    [Serializable]
    public  enum SkillTargetType: byte{
        Self,
        Target,
        Party,
        //massive
        NoTarget
    }
}