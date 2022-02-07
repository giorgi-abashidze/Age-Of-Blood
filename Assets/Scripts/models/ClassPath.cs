using System;

namespace models
{
    [Serializable]
    public class ClassPath
    {
        public byte Id;
        public byte ParentId;
        public byte RaceId;
        public byte RequiredLevel;
        public byte ClassTypeId;
        public string Name;

    }
}