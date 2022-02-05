using Mirror;

namespace controllers
{
    public class StatsManager: NetworkBehaviour
    {
        [SyncVar]
        public bool isDead;
        
        //0 = player, 1 = monster, 2 = npc, 3 = boss
        public byte role { get; set; }
        public bool attackable { get; set; }
        public byte raceId { get; set; }
        public byte gender { get; set; }
        public byte level { get; set; }
        public double xp { get; set; }
        public byte classId { get; set; }
        public int maxHp { get; set; }
        [SyncVar] 
        public int hp;
        public int maxMp { get; set; }
        public int mp { get; set; }
        public int maxCp { get; set; }
        public int cp { get; set; }
        public int pAtk { get; set; }
        public int mAtk { get; set; }
        public int critRate { get; set; }
        public int mCritRate { get; set; }
        public int critPower { get; set; }
        public int mCritPower { get; set; }
        public int speed { get; set; }
        public int castSpeed { get; set; }
        public int atkSpeed { get; set; }
        public int pDef { get; set; }
        public int mDef { get; set; }
        public int accuracy { get; set; }
        public int evasion { get; set; }
        

    }
}