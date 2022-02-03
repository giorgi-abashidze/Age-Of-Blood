using System;
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

namespace controllers
{
    public class AbilityManager: NetworkBehaviour
    {
        private SortedDictionary<KeyCode,Skill> _fPanel = new SortedDictionary<KeyCode,Skill>();
        private SortedDictionary<KeyCode,Skill> _numPanel = new SortedDictionary<KeyCode,Skill>();
        
        private List<Skill> _cooldownList = new List<Skill>();
        
        private SortedDictionary<int,Skill> _classAbilities = new SortedDictionary<int,Skill>();
        
        private PlayerController _playerController;
        
        public static AbilityManager Instance;
        
        //M = magic, W = Warrior
        private bool _canDoMSkill = true;
        private bool _canDoWSkill = true;
        

        //Load panel config from locally saved file
        private void LoadSkillPanelConfig()
        {
            var fSkillsDataExists = File.Exists(Application.persistentDataPath + "/FPanelData.json");

            if (fSkillsDataExists)
            {
                var data = File.ReadAllText(Application.persistentDataPath + "/FPanelData.json");
                _fPanel = JsonUtility.FromJson<SortedDictionary<KeyCode,Skill>>(data);
            }

            var numSkillsDataExists = File.Exists(Application.persistentDataPath + "/NumPanelData.json");

            if (numSkillsDataExists)
            {
                var data = File.ReadAllText(Application.persistentDataPath + "/NumPanelData.json");
                _numPanel = JsonUtility.FromJson<SortedDictionary<KeyCode,Skill>>(data);
            }
            
        }
        
        //Write panel config to json file
        private void WriteSkillPanelConfig()
        {
            
            File.WriteAllText(Application.persistentDataPath + "/FPanelData.json", JsonUtility.ToJson(_fPanel));
            
            File.WriteAllText(Application.persistentDataPath + "/NumPanelData.json", JsonUtility.ToJson(_numPanel));
            
        }

        

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _playerController = gameObject.GetComponent<PlayerController>();
            LoadSkillPanelConfig();
        }
        
        private void Awake() {
            
            if (Instance != null) {
                Destroy(gameObject);
            }else{
                Instance = this;
            }
        }
        
        private void Update()
        {
            if(_cooldownList.Count == 0 || !isLocalPlayer)
                return;


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
        }

        [Client]
        public void UseAbilityFromFPanel(KeyCode key)
        {
            if (_playerController.GetTargetNetId() == 0)
                return;
            
            if (_fPanel.ContainsKey(key))
            {
                var skill = _fPanel[key];

                if (skill != null)
                {
                    if (!skill.Reloading)
                    {
                        connectionToServer.Send(new AbilityUseRequest
                        {
                            IssuerNetId = netId,
                            TargetNetId = _playerController.GetTargetNetId(),
                            SkillId = skill.Id,
                            IssuerLevel =  _playerController.GetLevel(),
                            IssuerClassId = _playerController.GetClassId()
                        });
                       
                        skill.StartReload();
                    }
                }

            }
        }
        
        [Client]
        public void UseAbilityFromNumPanel(KeyCode key)
        {
            if (_playerController.GetTargetNetId() == 0)
                return;
            
            if (_numPanel.ContainsKey(key))
            {
                var skill = _fPanel[key];

                if (skill != null)
                {
                    if (!skill.Reloading)
                    {
                        connectionToServer.Send(new AbilityUseRequest
                        {
                            IssuerNetId = netId,
                            TargetNetId = _playerController.GetTargetNetId(),
                            SkillId = skill.Id,
                            IssuerLevel =  _playerController.GetLevel(),
                            IssuerClassId = _playerController.GetClassId()
                        });

                        skill.StartReload();
                    }
                }
            }
        }

        [Client]
        public void PutInCooldown(Skill skill)
        {
            _cooldownList.Add(skill);
        }
        
    }
}