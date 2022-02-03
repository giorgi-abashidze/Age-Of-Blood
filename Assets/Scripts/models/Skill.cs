using System;
using System.Collections.Generic;
using controllers;
using enums;

namespace models
{
    [Serializable]
    public class Skill{

        public ushort Id {get;set;}
        public string Name {get;set;}
        public byte RequiredLevel { get; set; }
        public byte ClassId { get; set; }
        public byte Level {get;set;}

        public SkillType Type {get;set;}

        public SkillSecondaryType TypeSecondary {get;set;}

        public List<SkillAffectTarget> AffectTarget {get;set;} 

        public List<SkillAffectType> AffectType {get;set;} 
    
        public SkillTargetType TargetType {get;set;}

        public int Power {get;set;}

        public int Range {get;set;}

        public int MaxReloadTime {get;set;}
        
        public float ReloadValue { get; set; }
        public bool Reloading { get; set; }

        public void StartReload()
        {
            if(this.Reloading)
                return;
            
            this.Reloading = true;
            ReloadValue = (float)MaxReloadTime;
            AbilityManager.Instance.PutInCooldown(this);
        }

    }
}