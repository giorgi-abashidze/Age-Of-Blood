using System;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using Mirror;
using models;
using network.messages;
using Telepathy;
using UnityEngine;

namespace network
{
    public class MyNetworkManager: NetworkManager
    {
        private const string SpawnableDir = "Prefabs/";
        private readonly List<GameObject> _spawnList = new List<GameObject>();
        
        private SortedDictionary<int,Skill> _allAbilities = new SortedDictionary<int,Skill>();
        public bool isServer;

        public override void Start()
        {
            base.Start();
            
            _spawnList.AddRange(Resources.LoadAll<GameObject>(SpawnableDir));
                        
            playerPrefab = _spawnList[0];
            maxConnections = 500;
            
            if (!isServer)
            {
               
                StartClient();

            }
            else
            {
                
                Debug.Log(">>>>>>>>> Starting Server... <<<<<<<<<<",this);
                StartServer();
                Debug.Log(">>>>>>>>> Server Started at: "+networkAddress+" :"+GetComponent<KcpTransport>().Port+" <<<<<<<<<<",this);
            }

        }
        
        
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            Debug.Log(">>>>>>>>> New client connected from: "+ conn.address+" <<<<<<<<<<",this);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            
            var createCharMessage = new CreateCharacterMessage
            {
                Name = "test"
            };
            NetworkClient.Send(createCharMessage);
            
        }

        public override void OnStartServer()
        {
            
            base.OnStartServer();
            Debug.Log(">>>>>>>>> Loading classes... <<<<<<<<<<",this);
            LoadClasses();
            Debug.Log(">>>>>>>>> Loading skills... <<<<<<<<<<",this);
            LoadSkills();
            Debug.Log(">>>>>>>>> "+_allAbilities.Count+" skills Loaded <<<<<<<<<<",this);
            
            NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
            NetworkServer.RegisterHandler<AbilityUseRequest>(UseAbilityRequest);
            NetworkServer.RegisterHandler<MoveRequest>(OnMoveRequest);
        }
        
        //todo:nothing inside yet 
        [Server]
        private void LoadClasses()
        {
            List<Skill> abilities = new List<Skill>();
            
            for (var i = 0; i < abilities.Count; i++)
            {
                var item = abilities.ElementAt(i);
                _allAbilities.Add(item.Id,item);
            }
        }
        
        //todo:nothing inside yet 
        [Server]
        private void LoadSkills()
        {
            List<Skill> abilities = new List<Skill>();
            
            for (var i = 0; i < abilities.Count; i++)
            {
                var item = abilities.ElementAt(i);
                _allAbilities.Add(item.Id,item);
            }
        }
        
        //todo:nothing inside yet 
        [ServerCallback]
        private void UseAbilityRequest(NetworkConnection conn, AbilityUseRequest message)
        {
            if (!_allAbilities.ContainsKey(message.SkillId))
                return;
            
            var ability = _allAbilities[message.SkillId];
            
            if (message.IssuerClassId != ability.ClassId || ability.RequiredLevel > message.IssuerLevel)
                return;
            
            var isSelfTarget = message.IssuerNetId == message.TargetNetId;
            
            var msg = new DamageMessage
            {
                IssuerNetId = message.IssuerNetId,
                TargetNetId = message.TargetNetId,
                Amount = CalculateSkillDamage(ability)
            };

            if (isSelfTarget)
            {
                conn.Send(msg);
            }
            else
            {
                conn.Send(msg);
                NetworkServer.spawned[message.TargetNetId].connectionToClient.Send(msg);
            }
            
        }

        [ServerCallback]
        private void OnMoveRequest(NetworkConnection conn,MoveRequest message)
        {
            float dist = Vector3.Distance(message.CurrentPoint, message.TargetPoint);
            Debug.Log(">>>>>>>>> move requested from "+message.CurrentPoint+" to "+message.TargetPoint+" <<<<<<<<<<",this);
            Debug.Log(">>>>>>>>> netId: "+message.IssuerNetId+" <<<<<<<<<<",this);
            if (dist > 150)
                return;
            
            //send to issuer
            //conn.Send(message);
            
            NetworkServer.SendToReady(message);
        }

        [ServerCallback]
        private void OnCreateCharacter(NetworkConnection conn,CreateCharacterMessage msg)
        {
          
            var player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        //todo:nothing inside yet 
        [Server]
        private int CalculateSkillDamage(Skill skill)
        {
            return 0;
        }
        
    }
}