using Mirror;
using models;

namespace helpers
{
    public static class CustomReadWriteFunctions
    {
        
        
        public static void WriteSkillType(this NetworkWriter writer, Skill value)
        {
            writer.WriteUShort(value.Id);
            writer.WriteByte(value.Level);
            writer.WriteByte(value.ClassId);

        }
        
        public static Skill ReadSkillType(this NetworkReader reader)
        {
            ushort id = reader.ReadUShort();
            byte level = reader.ReadByte();
            byte classId = reader.ReadByte();
            
            var skill = new Skill
            {
                Id = id,
                Level = level,
                ClassId = classId
            };

            return skill;
        }
    }
}