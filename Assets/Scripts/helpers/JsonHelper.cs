using System.Collections.Generic;
using models;
using UnityEngine;

namespace helpers
{
    public class JsonHelper
    {
        
        public static List<Skill> LoadSkillsFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/skills");
            var wrapper = JsonUtility.FromJson<SkillsWrapper>(jsonTextFile.text);
            return wrapper.Skills ?? new List<Skill>();
        }
        
        public static List<ClassPath> LoadClassesFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/classes");
            var wrapper = JsonUtility.FromJson<ClassesWrapper>(jsonTextFile.text);
            return wrapper.Classes ?? new List<ClassPath>();
        }
        
        public static List<Race> LoadRacesFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/races");
            var wrapper = JsonUtility.FromJson<RacesWrapper>(jsonTextFile.text);
            return wrapper.Races ?? new List<Race>();
        }
        
        public static List<ClassType> LoadClassTypesFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/classTypes");
            var wrapper = JsonUtility.FromJson<ClassTypesWrapper>(jsonTextFile.text);
            return wrapper.ClassTypes ?? new List<ClassType>();
        }
        
        public static List<BaseStatsModel> LoadBaseStatsFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/baseStats");
            var wrapper = JsonUtility.FromJson<BaseStatsWrapper>(jsonTextFile.text);
            return wrapper.BaseStats ?? new List<BaseStatsModel>();
        }
    }
}