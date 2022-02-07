using System.Collections.Generic;
using System.IO;
using models;
using UnityEngine;

namespace helpers
{
    public class JsonHelper
    {
        
        public static List<Skill> LoadSkillsFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/skills");

            return JsonUtility.FromJson<SkillsWrapper>(jsonTextFile.text).Skills;
        }
        
        public static List<ClassPath> LoadClassesFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/classes");

            return JsonUtility.FromJson<List<ClassPath>>(jsonTextFile.text);
        }
        
        public static List<Race> LoadRacesFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/races");

            return JsonUtility.FromJson<List<Race>>(jsonTextFile.text);
        }
        
        public static List<ClassType> LoadClassTypesFromJson()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Json/classTypes");

            return JsonUtility.FromJson<List<ClassType>>(jsonTextFile.text);
        }
    }
}