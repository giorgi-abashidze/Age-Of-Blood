using System;
using System.Collections.Generic;
using controllers;
using enums;

namespace models
{
    [Serializable]
    public class Skill
    {

        public ushort Id;
        public bool IsBasicPAttack;
        public string Name;
        public byte RequiredLevel;
        public short ClassId;

        public short ClassTypeId;
        public byte Level;
        
        //time in seconds (if its a buff)
        public float Time;

        public ConsumeType ConsumeType;

        public int ConsumeValue;
        public SkillType Type;

        public SkillSecondaryType TypeSecondary;

        public List<SkillAffectTarget> AffectTarget;

        public List<SkillAffectType> AffectType;

        public SkillTargetType TargetType;

        public int Power;

        public bool CanDoCrit;
        
        //used as affect radius if skill is massive and dont need target
        //else as minimum range
        public int Range;

        public int MaxReloadTime;

        public float ReloadValue;
        public bool Reloading;

        public string Description;

        public void StartReload()
        {
            if(this.Reloading)
                return;
            
            this.Reloading = true;
            ReloadValue = (float)MaxReloadTime;
        }

    }
}