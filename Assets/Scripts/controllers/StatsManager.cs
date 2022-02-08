using Mirror;
using models;

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
        public byte classType { get; set; }
        public int maxHp { get; set; }
        public int hp { get; set; }
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

        public void SetBaseStats(BaseStatsModel stats)
        {
            maxHp = stats.maxHp;
            maxMp = stats.maxMp;
            maxCp = stats.maxCp;
            pAtk = stats.pAtk;
            mAtk = stats.mAtk;
            critRate = stats.critRate;
            mCritRate = stats.mCritRate;
            critPower = stats.critPower;
            mCritPower = stats.mCritPower;
            speed = stats.speed;
            castSpeed = stats.castSpeed;
            atkSpeed = stats.atkSpeed;
            pDef = stats.pDef;
            mDef = stats.mDef;
            accuracy = stats.accuracy;
            evasion = stats.evasion;
        }
        

    }
}