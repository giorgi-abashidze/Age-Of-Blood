using Mirror;
using models;

namespace controllers
{
    public class StatsManager: NetworkBehaviour
    {
        [SyncVar]
        public bool isDead;
        
        //0 = player, 1 = monster, 2 = npc, 3 = boss
        public byte role;
        public bool attackable;
        public byte raceId;
        public byte gender;
        public byte level;
        public double xp;
        public byte classId;
        public byte classType;
        public int maxHp;
        public int hp;
        public int maxMp;
        public int mp;
        public int maxCp;
        public int cp;
        public int pAtk;
        public int mAtk;
        public int critRate;
        public int mCritRate;
        public int critPower;
        public int mCritPower;
        public int speed;
        public int castSpeed;
        public int atkSpeed;
        public int pDef;
        public int mDef;
        public int accuracy;
        public int evasion;

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