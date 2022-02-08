using System;
using UnityEngine.Serialization;

namespace models
{
    [Serializable]
    public class BaseStatsModel
    {
        public byte classType;
        public int maxHp;
        public int maxMp;
        public int maxCp;
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
    }
}