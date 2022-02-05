﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using helpers;
using Mirror;
using models;
using network.messages;
using Telepathy;
using UnityEngine;
using System.IO;
using enums;
using UnityEditor.Experimental.TerrainAPI;
using Random = System.Random;

namespace controllers
{
    public class AbilityManager: NetworkBehaviour
    {
        private SortedDictionary<KeyCode,int> _fPanel = new SortedDictionary<KeyCode,int>();
        private SortedDictionary<KeyCode,int> _numPanel = new SortedDictionary<KeyCode,int>();
        
        private SortedDictionary<int,Skill> _allAbilities = new SortedDictionary<int,Skill>();
        
        private SortedDictionary<int,Skill> _allAbilitiesOnClient = new SortedDictionary<int,Skill>();
        private SortedDictionary<int,Skill> _classAbilities = new SortedDictionary<int,Skill>();

        //int skillId, float remainingTime
        private Dictionary<int,float> _buffList = new Dictionary<int,float>();
        private SortedDictionary<int,float> _deBuffList = new SortedDictionary<int,float>();
        
        private List<Skill> _cooldownList = new List<Skill>();
        
        
        //Can do magic skill
        private bool _canDoMSkill = true;
        
        //Can do warrior skill
        private bool _canDoWSkill = true;

        private bool _canBeHealed = true;

        private StatsManager _statsManager;
        private GameObject _target;
        
        [Command]
        void CmdRequestAbilityUse(GameObject player,GameObject target, AbilityUseRequest request)
        {
            if (!_allAbilities.ContainsKey(request.SkillId))
                return;
            
            var ability = _allAbilities[request.SkillId];

            if (request.IssuerClassId != ability.ClassId || ability.RequiredLevel > request.IssuerLevel)
                return;

            var targetIdentity = target.GetComponent<NetworkIdentity>();
            
            var isSelfTarget = request.IssuerNetId == targetIdentity.netId;

            if (ability.TargetType == SkillTargetType.Self)
                isSelfTarget = true;
            
            var playerStats = player.GetComponent<StatsManager>();
            var playerAbilityManager = player.GetComponent<AbilityManager>();

            if (ability.TypeSecondary == SkillSecondaryType.Magic && !ability.isBasicPAttack && !playerAbilityManager._canDoMSkill)
                return;
            if (ability.TypeSecondary == SkillSecondaryType.Physical && !ability.isBasicPAttack && !playerAbilityManager._canDoWSkill)
                return;
            
            //self target
            if (isSelfTarget)
            {
                
                var playerMovementManager = player.GetComponent<MovementManager>();
                
                if (ability.Type == SkillType.Buff)
                {
                    if (playerAbilityManager._buffList.Count >= Constants.MaxBuffListSize)
                    {
                        playerAbilityManager._buffList.Remove(playerAbilityManager._buffList.First().Key);
                    }
                    if (playerAbilityManager._buffList.ContainsKey(ability.Id))
                    {
                        playerAbilityManager._buffList[ability.Id] = ability.Time;
                    }
                    else
                    {
                        playerAbilityManager._buffList.Add(ability.Id,ability.Time);
                    }
                    
                    TargetNotifySelfAbilityEffect(1,ability.Id);
                    
                    for (var i = 0; i < ability.AffectTarget.Count; i++)
                    {
                        var at = ability.AffectTarget[i];
                   
                        switch (at)
                        {
                            case SkillAffectTarget.Accuracy:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.accuracy += ((ability.Power / 100) * playerStats.accuracy);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.accuracy -= ((ability.Power / 100) * playerStats.accuracy);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Hp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.maxHp += ((ability.Power / 100) * playerStats.maxHp);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.maxHp -= ((ability.Power / 100) * playerStats.maxHp);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Mp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.maxMp += ((ability.Power / 100) * playerStats.maxMp);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.maxMp -= ((ability.Power / 100) * playerStats.maxMp);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Evasion:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.evasion += ((ability.Power / 100) * playerStats.evasion);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.evasion -= ((ability.Power / 100) * playerStats.evasion);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Speed:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.speed += ((ability.Power / 100) * playerStats.speed);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.speed -= ((ability.Power / 100) * playerStats.speed);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.AtkSpeed:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.atkSpeed += ((ability.Power / 100) * playerStats.atkSpeed);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.atkSpeed -= ((ability.Power / 100) * playerStats.atkSpeed);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.CastSpeed:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.castSpeed += ((ability.Power / 100) * playerStats.castSpeed);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.castSpeed -= ((ability.Power / 100) * playerStats.castSpeed);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.CritPower:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.critPower += ((ability.Power / 100) * playerStats.critPower);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.critPower -= ((ability.Power / 100) * playerStats.critPower);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.CritRate:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.critRate += ((ability.Power / 100) * playerStats.critRate);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.critRate -= ((ability.Power / 100) * playerStats.critRate);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MAtk:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.mAtk += ((ability.Power / 100) * playerStats.mAtk);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.mAtk -= ((ability.Power / 100) * playerStats.mAtk);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.PAtk:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.pAtk += ((ability.Power / 100) * playerStats.pAtk);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.pAtk -= ((ability.Power / 100) * playerStats.pAtk);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MDef:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.mDef += ((ability.Power / 100) * playerStats.mDef);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.mDef -= ((ability.Power / 100) * playerStats.mDef);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.PDef:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.pDef += ((ability.Power / 100) * playerStats.pDef);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.pDef -= ((ability.Power / 100) * playerStats.pDef);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MCritRate:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.mCritRate += ((ability.Power / 100) * playerStats.mCritRate);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.mCritRate -= ((ability.Power / 100) * playerStats.mCritRate);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MCritPower:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        playerStats.mCritPower += ((ability.Power / 100) * playerStats.mCritPower);
                                        break;
                                    case SkillAffectType.Decrease:
                                        playerStats.mCritPower -= ((ability.Power / 100) * playerStats.mCritPower);
                                        break;
                                }
                                break;
                        }
                    }
                }
                else if(ability.Type == SkillType.Active)
                {
                    for (var i = 0; i < ability.AffectTarget.Count; i++)
                    {
                        var at = ability.AffectTarget[i];
                        
                        var affectValue = 0;
                        
                        switch (at)
                        {
                            
                            case SkillAffectTarget.Hp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        if (playerAbilityManager._canBeHealed)
                                        {
                                            affectValue = ((ability.Power / 100) * playerStats.hp);
                                            if (playerStats.hp + affectValue > playerStats.maxHp)
                                                playerStats.hp = playerStats.maxHp;
                                            else
                                                playerStats.hp += affectValue;
                                            if(playerStats.role == 0)
                                                TargetNotifySelfAbilityEffect(2,ability.Id);
                                        }
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Mp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        affectValue = ((ability.Power / 100) * playerStats.mp);
                                        if (playerStats.mp + affectValue > playerStats.maxMp)
                                            playerStats.mp = playerStats.maxMp;
                                        else
                                            playerStats.mp += affectValue;
                                        if(playerStats.role == 0)
                                            TargetNotifySelfAbilityEffect(3,ability.Id);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.AbilityMove:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Heal:
                                        playerMovementManager._canMove = true;
                                        if(playerStats.role == 0)
                                            TargetNotifySelfAbilityEffect(6,ability.Id);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            //end of self target
            else
            {
                
                var targetStats = target.GetComponent<StatsManager>();
                if (!targetStats.attackable || targetStats.isDead)
                    return;
                
                var targetAbilityManager = target.GetComponent<AbilityManager>();
                var targetMovementManager = target.GetComponent<MovementManager>();
                
                if (ability.Type == SkillType.Buff)
                {
                    if (targetAbilityManager._buffList.Count >= Constants.MaxBuffListSize)
                    {
                        targetAbilityManager._buffList.Remove(targetAbilityManager._buffList.First().Key);
                    }
                    
                    if (targetAbilityManager._buffList.ContainsKey(ability.Id))
                    {
                        targetAbilityManager._buffList[ability.Id] = ability.Time;
                    }
                    else
                    {
                        targetAbilityManager._buffList.Add(ability.Id,ability.Time);
                    }
                    
                    if(targetStats.role == 0)
                        TargetNotifyAbilityEffect(targetIdentity.connectionToClient,1,ability.Id);
                    
                    for (var i = 0; i < ability.AffectTarget.Count; i++)
                    {
                        var at = ability.AffectTarget[i];
                   
                        switch (at)
                        {
                            case SkillAffectTarget.Accuracy:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.accuracy += ((ability.Power / 100) * targetStats.accuracy);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.accuracy -= ((ability.Power / 100) * targetStats.accuracy);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Hp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.maxHp += ((ability.Power / 100) * targetStats.maxHp);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.maxHp -= ((ability.Power / 100) * targetStats.maxHp);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Mp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.maxMp += ((ability.Power / 100) * targetStats.maxMp);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.maxMp -= ((ability.Power / 100) * targetStats.maxMp);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Evasion:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.evasion += ((ability.Power / 100) * targetStats.evasion);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.evasion -= ((ability.Power / 100) * targetStats.evasion);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Speed:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.speed += ((ability.Power / 100) * targetStats.speed);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.speed -= ((ability.Power / 100) * targetStats.speed);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.AtkSpeed:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.atkSpeed += ((ability.Power / 100) * targetStats.atkSpeed);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.atkSpeed -= ((ability.Power / 100) * targetStats.atkSpeed);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.CastSpeed:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.castSpeed += ((ability.Power / 100) * targetStats.castSpeed);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.castSpeed -= ((ability.Power / 100) * targetStats.castSpeed);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.CritPower:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.critPower += ((ability.Power / 100) * targetStats.critPower);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.critPower -= ((ability.Power / 100) * targetStats.critPower);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.CritRate:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.critRate += ((ability.Power / 100) * targetStats.critRate);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.critRate -= ((ability.Power / 100) * targetStats.critRate);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MAtk:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.mAtk += ((ability.Power / 100) * targetStats.mAtk);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.mAtk -= ((ability.Power / 100) * targetStats.mAtk);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.PAtk:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.pAtk += ((ability.Power / 100) * targetStats.pAtk);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.pAtk -= ((ability.Power / 100) * targetStats.pAtk);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MDef:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.mDef += ((ability.Power / 100) * targetStats.mDef);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.mDef -= ((ability.Power / 100) * targetStats.mDef);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.PDef:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.pDef += ((ability.Power / 100) * targetStats.pDef);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.pDef -= ((ability.Power / 100) * targetStats.pDef);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MCritRate:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.mCritRate += ((ability.Power / 100) * targetStats.mCritRate);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.mCritRate -= ((ability.Power / 100) * targetStats.mCritRate);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.MCritPower:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        targetStats.mCritPower += ((ability.Power / 100) * targetStats.mCritPower);
                                        break;
                                    case SkillAffectType.Decrease:
                                        targetStats.mCritPower -= ((ability.Power / 100) * targetStats.mCritPower);
                                        break;
                                }
                                break;
                        }
                    }
                    
                }
                else if (ability.Type == SkillType.DeBuff)
                {
                    if (targetStats.role != 0 || targetStats.role != 1)
                    {
                        if(playerStats.role == 0)
                            TargetNotifySelfAbilityEffect(7,ability.Id);
                    }
                    else
                    {

                        var chance = 100;
                        var levelDiff = targetStats.level - playerStats.level;
                        if (levelDiff > 10)
                        {
                            chance -= 70;
                           
                        }
                        

                        if (ability.TypeSecondary == SkillSecondaryType.Magic)
                        {
                            chance -= (targetStats.mDef - playerStats.mAtk) * 10;
                        }
                        else if (ability.TypeSecondary == SkillSecondaryType.Physical)
                        {
                            chance -= (targetStats.pDef - playerStats.pAtk) * 10;
                        }

                        if (chance < 100)
                        {
                            var randomStart = levelDiff < -10 ? 10 : 0;
                            int rand = new Random().Next(randomStart, chance);
                            int rand2 = new Random().Next(0, 10);

                            if (rand / 2 < rand2)
                            {
                                if(playerStats.role == 0)
                                    TargetNotifySelfAbilityEffect(7,ability.Id);
                                TargetPutInCooldown(ability.Id);
                                return;
                            }
                        }

                        targetAbilityManager._deBuffList.Add(ability.Id,ability.Time);
                        
                        TargetNotifySelfAbilityEffect(6,ability.Id);
                        
                        for (var i = 0; i < ability.AffectTarget.Count; i++)
                        {
                            var at = ability.AffectTarget[i];
                       
                            switch (at)
                            {
                                case SkillAffectTarget.Accuracy:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.accuracy += ((ability.Power / 100) * targetStats.accuracy);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.accuracy -= ((ability.Power / 100) * targetStats.accuracy);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.Hp:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.maxHp += ((ability.Power / 100) * targetStats.maxHp);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.maxHp -= ((ability.Power / 100) * targetStats.maxHp);
                                            break;
                                        case SkillAffectType.Block:
                                            targetAbilityManager._canBeHealed = false;
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.Mp:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.maxMp += ((ability.Power / 100) * targetStats.maxMp);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.maxMp -= ((ability.Power / 100) * targetStats.maxMp);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.Evasion:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.evasion += ((ability.Power / 100) * targetStats.evasion);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.evasion -= ((ability.Power / 100) * targetStats.evasion);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.Speed:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.speed += ((ability.Power / 100) * targetStats.speed);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.speed -= ((ability.Power / 100) * targetStats.speed);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.AtkSpeed:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.atkSpeed += ((ability.Power / 100) * targetStats.atkSpeed);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.atkSpeed -= ((ability.Power / 100) * targetStats.atkSpeed);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.CastSpeed:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.castSpeed += ((ability.Power / 100) * targetStats.castSpeed);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.castSpeed -= ((ability.Power / 100) * targetStats.castSpeed);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.CritPower:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.critPower += ((ability.Power / 100) * targetStats.critPower);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.critPower -= ((ability.Power / 100) * targetStats.critPower);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.CritRate:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.critRate += ((ability.Power / 100) * targetStats.critRate);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.critRate -= ((ability.Power / 100) * targetStats.critRate);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.MAtk:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.mAtk += ((ability.Power / 100) * targetStats.mAtk);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.mAtk -= ((ability.Power / 100) * targetStats.mAtk);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.PAtk:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.pAtk += ((ability.Power / 100) * targetStats.pAtk);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.pAtk -= ((ability.Power / 100) * targetStats.pAtk);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.MDef:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.mDef += ((ability.Power / 100) * targetStats.mDef);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.mDef -= ((ability.Power / 100) * targetStats.mDef);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.PDef:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.pDef += ((ability.Power / 100) * targetStats.pDef);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.pDef -= ((ability.Power / 100) * targetStats.pDef);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.MCritRate:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.mCritRate += ((ability.Power / 100) * targetStats.mCritRate);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.mCritRate -= ((ability.Power / 100) * targetStats.mCritRate);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.MCritPower:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Increase:
                                            targetStats.mCritPower += ((ability.Power / 100) * targetStats.mCritPower);
                                            break;
                                        case SkillAffectType.Decrease:
                                            targetStats.mCritPower -= ((ability.Power / 100) * targetStats.mCritPower);
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.AbilityMove:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Block:
                                            targetMovementManager._canMove = false;
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.AbilitySkillMagic:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Block:
                                            targetAbilityManager._canDoMSkill = false;
                                            break;
                                    }
                                    break;
                                case SkillAffectTarget.AbilitySkillPhysical:
                                    switch (ability.AffectType[i])
                                    {
                                        case SkillAffectType.Block:
                                            targetAbilityManager._canDoWSkill = false;
                                            break;
                                    }
                                    break;
                            }
                        } 
                    }
   
                }
                else if (ability.Type == SkillType.Active)
                {
                    
                    var chance = 0;
                    var critChance = 0;
                    var damage = 1;
                    var isCrit = false;

                    for (var i = 0; i < ability.AffectTarget.Count; i++)
                    {
                        var affectValue = 0;
                        var at = ability.AffectTarget[i];
                        switch (at)
                        {
                            case SkillAffectTarget.Hp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        if (targetAbilityManager._canBeHealed)
                                        {
                                            affectValue = ((ability.Power / 100) * targetStats.hp);
                                            if (targetStats.hp + affectValue > targetStats.maxHp)
                                                targetStats.hp = targetStats.maxHp;
                                            else
                                                targetStats.hp += affectValue;
                                            if (targetStats.role == 0)
                                                TargetNotifyAbilityEffect(targetIdentity.connectionToClient, 2,
                                                    ability.Id);
                                        }
                                        break;
                                    case SkillAffectType.Heal:
                                        targetAbilityManager._canBeHealed = true;
                                        if(targetStats.role == 0)
                                            TargetNotifyAbilityEffect(targetIdentity.connectionToClient,1,ability.Id);
                                        break;
                                    case SkillAffectType.Decrease:
                                        if (ability.TypeSecondary == SkillSecondaryType.Physical)
                                        {
                                            chance += playerStats.accuracy - (targetStats.evasion / 2);
                                            if (chance < 100)
                                            {
                                                int rand = new Random().Next(chance, 100);
                                                int rand2 = new Random().Next(0, 50);

                                                if (rand / 2 < rand2)
                                                {
                                                    if(playerStats.role == 0)
                                                        TargetNotifySelfAbilityEffect(8,ability.Id);
                                                    TargetPutInCooldown(ability.Id);
                                                    return;
                                                }
                                            }
                                            
                                            damage = playerStats.pAtk + ability.Power - targetStats.pDef;

                                            if (ability.canDoCrit)
                                            {
                                                critChance = playerStats.critRate / 4;
                                                int rand = new Random().Next(critChance, 100);
                                                int rand2 = new Random().Next(0, 50);

                                                if (rand / 2 > rand2)
                                                {
                                                    isCrit = true;
                                                    damage += playerStats.critPower * 2;
                                                }
                                            }
                                            
                                            if (targetStats.hp - damage <= 0)
                                            {
                                                targetStats.hp = 0;
                                                targetStats.isDead = true;
                                            }
                                            else
                                            {
                                                targetStats.hp -= damage;
                                            }

                                            if(targetStats.role == 0)
                                                TargetNotifyReceivedDamage(targetIdentity.connectionToClient, damage);
                                            TargetNotifyDamage(damage,isCrit);

                                        }
                                        else
                                        {
                                            //magic skills cant be missed
                                            damage = playerStats.mAtk + ability.Power - targetStats.mDef;
                                            
                                            if (ability.canDoCrit)
                                            {
                                                critChance = playerStats.mCritRate / 4;
                                                int rand = new Random().Next(critChance, 100);
                                                int rand2 = new Random().Next(30, 50);

                                                if (rand / 2 > rand2)
                                                {
                                                    isCrit = true;
                                                    damage += playerStats.mCritPower * 2;
                                                }
                                            }
                                            
                                            if (targetStats.hp - damage <= 0)
                                            {
                                                targetStats.hp = 0;
                                                targetStats.isDead = true;
                                            }
                                            else
                                            {
                                                targetStats.hp -= damage;
                                            }
                                            
                                            if(targetStats.role == 0)
                                                TargetNotifyReceivedDamage(targetIdentity.connectionToClient, damage);
                                            TargetNotifyDamage(damage,isCrit);
                                        }
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Mp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Increase:
                                        affectValue = ((ability.Power / 100) * targetStats.mp);
                                        if (targetStats.mp + affectValue > targetStats.maxMp)
                                        {
                                            targetStats.mp = targetStats.maxMp;
                                        }
                                        else
                                        {
                                            targetStats.mp += affectValue;
                                        }

                                        if(targetStats.role == 0)
                                            TargetNotifyAbilityEffect(targetIdentity.connectionToClient,2,ability.Id);
                                        break;
                                    case SkillAffectType.Decrease:
                                        affectValue = ((ability.Power / 100) * targetStats.mp);
                                        if (targetStats.mp - affectValue < 0)
                                            targetStats.mp = 0;
                                        else
                                            targetStats.mp -= affectValue;
                                        
                                        if(targetStats.role == 0)
                                            TargetNotifyReceivedDamage(targetIdentity.connectionToClient, damage);
                                        TargetNotifyDamage(damage,false);
                                        break;
                                }
                                break;
                            case SkillAffectTarget.Cp:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Decrease:
                                          if (ability.TypeSecondary == SkillSecondaryType.Physical)
                                          {
                                            chance += playerStats.accuracy - (targetStats.evasion / 2);
                                            if (chance < 100)
                                            {
                                                int rand = new Random().Next(chance, 100);
                                                int rand2 = new Random().Next(0, 50);

                                                if (rand / 2 < rand2)
                                                {
                                                    if(playerStats.role == 0)
                                                        TargetNotifySelfAbilityEffect(8,ability.Id);
                                                    TargetPutInCooldown(ability.Id);
                                                    return;
                                                }
                                            }
                                            
                                            damage = playerStats.pAtk + ability.Power - targetStats.pDef;

                                            if (ability.canDoCrit)
                                            {
                                                critChance = playerStats.critRate / 4;
                                                int rand = new Random().Next(critChance, 100);
                                                int rand2 = new Random().Next(0, 50);

                                                if (rand / 2 > rand2)
                                                {
                                                    isCrit = true;
                                                    damage += playerStats.critPower * 2;
                                                }
                                            }
                                            
                                            if (targetStats.cp - damage < 0)
                                            {
                                                if ((targetStats.hp - (damage - targetStats.cp)) <= 0)
                                                {
                                                    targetStats.hp = 0;
                                                    targetStats.isDead = true;
                                                }
                                                else
                                                {
                                                    targetStats.hp -= (damage - targetStats.cp);
                                                }
                                                
                                                targetStats.cp = 0;
                                            }
                                            else
                                            {
                                                targetStats.cp -= damage;
                                            }
                                            if(targetStats.role == 0)
                                                TargetNotifyReceivedDamage(targetIdentity.connectionToClient, damage);
                                            TargetNotifyDamage(damage,isCrit);

                                          }
                                          else
                                          {
                                            //magic skills cant be missed
                                            damage = playerStats.mAtk + ability.Power - targetStats.mDef;
                                            
                                            if (ability.canDoCrit)
                                            {
                                                critChance = playerStats.mCritRate / 4;
                                                int rand = new Random().Next(critChance, 100);
                                                int rand2 = new Random().Next(30, 50);

                                                if (rand / 2 > rand2)
                                                {
                                                    isCrit = true;
                                                    damage += playerStats.mCritPower * 2;
                                                }
                                            }
           
                                            if (targetStats.cp - damage < 0)
                                            {
                                                if ((targetStats.hp - (damage - targetStats.cp)) <= 0)
                                                {
                                                    targetStats.hp = 0;
                                                    targetStats.isDead = true;
                                                }
                                                else
                                                {
                                                    targetStats.hp -= (damage - targetStats.cp);
                                                }
                                                
                                                targetStats.cp = 0;
                                            }
                                            else
                                            {
                                                targetStats.cp -= damage;
                                            }
                                            if(targetStats.role == 0)
                                                TargetNotifyReceivedDamage(targetIdentity.connectionToClient, damage);
                                            TargetNotifyDamage(damage,isCrit);
                                          } 
                                          
                                          break;
                                    case SkillAffectType.Vamp:
                                         //magic skills cant be missed
                                            damage = playerStats.mAtk + ability.Power - targetStats.mDef;
                                            
                                            if (ability.canDoCrit)
                                            {
                                                critChance = playerStats.mCritRate / 4;
                                                int rand = new Random().Next(critChance, 100);
                                                int rand2 = new Random().Next(30, 50);

                                                if (rand / 2 > rand2)
                                                {
                                                    isCrit = true;
                                                    damage += playerStats.mCritPower * 2;
                                                }
                                            }
           
                                            if (targetStats.cp - damage < 0)
                                            {
                                                if ((targetStats.hp - (damage - targetStats.cp)) <= 0)
                                                {
                                                    targetStats.hp = 0;
                                                    targetStats.isDead = true;
                                                }
                                                else
                                                {
                                                    targetStats.hp -= (damage - targetStats.cp);
                                                }
                                                
                                                targetStats.cp = 0;
                                            }
                                            else
                                            {
                                                targetStats.cp -= damage;
                                            }

                                            var vampValue = damage / 3;

                                             if (playerStats.hp + vampValue > playerStats.maxHp)
                                             {
                                                 playerStats.hp = playerStats.maxHp;
                                             }
                                             else
                                             {
                                                 playerStats.hp += vampValue;
                                             }

                                            if(targetStats.role == 0)
                                                TargetNotifyReceivedDamage(targetIdentity.connectionToClient, damage);
                                            TargetNotifyDamage(damage,isCrit);
                                        break;
                                }
                                
                                break;
                            case SkillAffectTarget.AbilityMove:
                                switch (ability.AffectType[i])
                                {
                                    case SkillAffectType.Heal:
                                        targetMovementManager._canMove = true;
                                        if(targetStats.role == 0)
                                            TargetNotifyAbilityEffect(targetIdentity.connectionToClient,1,ability.Id);
                                        break;
                                }
                                break;
                        }
                    }

                }

            }

            
            TargetPutInCooldown(ability.Id);
            
        }

        [TargetRpc]
        void TargetPutInCooldown(int skillId)
        {
            var skill = _classAbilities[skillId];
            skill.StartReload();
            _cooldownList.Add(skill);
        }
        
        [TargetRpc]
        void TargetNotifyAbilityEffect(NetworkConnection target, int msgId,ushort abilityId)
        {
            Debug.Log((NotificationMessages.AbilityNotifications[msgId],_allAbilitiesOnClient[abilityId].Name),this);
        }
        
        [TargetRpc]
        void TargetNotifySelfAbilityEffect(int msgId,ushort abilityId)
        {
            Debug.Log((NotificationMessages.AbilityNotifications[msgId],_allAbilitiesOnClient[abilityId].Name),this);
        }

        [TargetRpc]
        void TargetNotifyReceivedDamage(NetworkConnection target,int damage)
        {
            Debug.Log(damage+" damage received",this);
           
        }
        
        [TargetRpc]
        void TargetNotifyDamage(int damage,bool iscrit)
        {
            if(iscrit)
                Debug.Log("Critical Damage!",this);
            Debug.Log("You hit for "+damage+" damage",this);
        }

        //Load panel config from locally saved file
        [Client]
        private void LoadSkillPanelConfig()
        {
            var fSkillsDataExists = File.Exists(Application.persistentDataPath + "/FPanelData.json");

            if (fSkillsDataExists)
            {
                var data = File.ReadAllText(Application.persistentDataPath + "/FPanelData.json");
                _fPanel = JsonUtility.FromJson<SortedDictionary<KeyCode,int>>(data);
            }

            var numSkillsDataExists = File.Exists(Application.persistentDataPath + "/NumPanelData.json");

            if (numSkillsDataExists)
            {
                var data = File.ReadAllText(Application.persistentDataPath + "/NumPanelData.json");
                _numPanel = JsonUtility.FromJson<SortedDictionary<KeyCode,int>>(data);
            }
            
        }
        
        //Write panel config to json file
        [Client]
        private void WriteSkillPanelConfig()
        {
            
            File.WriteAllText(Application.persistentDataPath + "/FPanelData.json", JsonUtility.ToJson(_fPanel));
            
            File.WriteAllText(Application.persistentDataPath + "/NumPanelData.json", JsonUtility.ToJson(_numPanel));
            
        }

        

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            LoadSkillPanelConfig();
        }

        private void Start()
        {
            _statsManager = gameObject.GetComponent<StatsManager>();
        }

        private void Update()
        {
            
            for (var i = 0; i < _cooldownList.Count;i++)
            {
                var element = _cooldownList.ElementAt(i);
                element.ReloadValue -= Time.deltaTime;

                if (element.ReloadValue <= 0f)
                {
                    element.Reloading = false;
                    _cooldownList.Remove(element);
                }
            }
            
            foreach(var pair in _buffList.ToArray()) {
                _buffList[pair.Key] -= Time.deltaTime;
                if (_buffList[pair.Key] <= 0)
                    _buffList.Remove(pair.Key);
            }
            
            foreach(var pair in _deBuffList.ToArray()) {
                _deBuffList[pair.Key] -= Time.deltaTime;
                if (_deBuffList[pair.Key] <= 0)
                    _deBuffList.Remove(pair.Key);
            }
            
        }

        [Client]
        public void UseAbilityFromFPanel(KeyCode key)
        {

            
            if (_fPanel.ContainsKey(key))
            {
                var skill = _classAbilities[_fPanel[key]];

                if (skill != null)
                { 
                    if(skill.TargetType == SkillTargetType.Target && _target == null)
                        return;
                    
                    if (!skill.Reloading && skill.Type != SkillType.Passive)
                    {
                        CmdRequestAbilityUse(gameObject,_target,new AbilityUseRequest
                        {
                            IssuerNetId = netId,
                            SkillId = skill.Id,
                            IssuerLevel =  _statsManager.level,
                            IssuerClassId = _statsManager.classId
                        });
                       
                       
                    }
                }

            }
        }
        
        [Client]
        public void UseAbilityFromNumPanel(KeyCode key)
        {
            
            if (_numPanel.ContainsKey(key))
            {
                var skill = _classAbilities[_fPanel[key]];

                if (skill != null)
                {
                    if (skill.TargetType == SkillTargetType.Target && _target == null)
                        return;
                    
                    if (!skill.Reloading && skill.Type != SkillType.Passive)
                    {
                        CmdRequestAbilityUse(gameObject,_target,new AbilityUseRequest
                        {
                            IssuerNetId = netId,
                            SkillId = skill.Id,
                            IssuerLevel =  _statsManager.level,
                            IssuerClassId = _statsManager.classId
                        });

                    }
                }
            }
        }
        
        


    }
}