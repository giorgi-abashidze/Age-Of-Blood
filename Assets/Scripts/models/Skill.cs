using System;
using System.Collections.Generic;
using controllers;
using enums;

namespace models
{
    [Serializable]
    public class Skill{

        public ushort Id {get;set;}
        public bool IsBasicPAttack { get; set; }
        public string Name {get;set;}
        public byte RequiredLevel { get; set; }
        public byte ClassId { get; set; }
        
        public byte ClassTypeId { get; set; }
        public byte Level {get;set;}
        
        //time in seconds (if its a buff)
        public float Time {get;set;}
        
        public SkillType Type {get;set;}

        public SkillSecondaryType TypeSecondary {get;set;}

        public List<SkillAffectTarget> AffectTarget {get;set;} 

        public List<SkillAffectType> AffectType {get;set;} 
    
        public SkillTargetType TargetType {get;set;}

        public int Power {get;set;}
        
        public bool CanDoCrit { get; set; }
        
        //used as affect radius if skill is massive and dont need target
        //else as minimum range
        public int Range {get;set;}

        public int MaxReloadTime {get;set;}
        
        public float ReloadValue { get; set; }
        public bool Reloading { get; set; }
        
        public String Description { get; set; }

        public void StartReload()
        {
            if(this.Reloading)
                return;
            
            this.Reloading = true;
            ReloadValue = (float)MaxReloadTime;
        }

    }
}