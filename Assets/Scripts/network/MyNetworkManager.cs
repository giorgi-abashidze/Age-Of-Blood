using System;
using System.Collections.Generic;
using System.Linq;
using controllers;
using helpers;
using kcp2k;
using Mirror;
using models;
using network.messages;
using Telepathy;
using UnityEngine;
using Log = kcp2k.Log;

namespace network
{
    public class MyNetworkManager: NetworkManager
    {
        private const string SpawnableDir = "Prefabs/";
        private readonly List<GameObject> _spawnList = new List<GameObject>();
        public readonly SortedDictionary<ushort,Skill> AllAbilities = new SortedDictionary<ushort,Skill>();
        public readonly SortedDictionary<byte,Race> Races = new SortedDictionary<byte,Race>();
        public readonly SortedDictionary<byte,ClassPath> Classes = new SortedDictionary<byte,ClassPath>();
        public readonly SortedDictionary<byte,ClassType> ClassTypes = new SortedDictionary<byte,ClassType>();
        public readonly List<BaseStatsModel> BaseStats = new List<BaseStatsModel>();
      
        public bool isServer;
        
        public override void Start()
        {
            base.Start();

            
            playerPrefab = Resources.LoadAll<GameObject>(SpawnableDir)[0];
            maxConnections = 500;
            
            JsonHelper.LoadSkillsFromJson().ForEach(c =>
            {
                AllAbilities.Add(c.Id,c);
            });
                
            Debug.Log(">>>>>>>>> "+AllAbilities.Count+" Skills loaded <<<<<<<<<<",this);
                
            JsonHelper.LoadRacesFromJson().ForEach(c =>
            {
                Races.Add(c.Id,c);
            });
                
            JsonHelper.LoadClassesFromJson().ForEach(c =>
            {
                Classes.Add(c.Id,c);
            });
                
            Debug.Log(">>>>>>>>> "+Classes.Count+" Classes loaded <<<<<<<<<<",this);
                
            JsonHelper.LoadClassTypesFromJson().ForEach(c =>
            {
                ClassTypes.Add(c.Id,c);
            });

            BaseStats.Clear();
            BaseStats.AddRange(JsonHelper.LoadBaseStatsFromJson());
           
            
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
                Name = "test",
                ClassTypeId = 1,
                ClassId = 1
            };
            NetworkClient.Send(createCharMessage);
            
        }

        public override void OnStartServer()
        {
            
            base.OnStartServer();

            
            NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        }
        
        
        
        [ServerCallback]
        private void OnCreateCharacter(NetworkConnection conn,CreateCharacterMessage msg)
        {
          
            var player = Instantiate(playerPrefab);
            var statsManager = player.GetComponent<StatsManager>();
            var baseStats = BaseStats.FirstOrDefault(s => s.classType == msg.ClassTypeId);
            if (baseStats == null)
                return;
            
            statsManager.SetBaseStats(baseStats);
            
            statsManager.classId = msg.ClassId;
            statsManager.classType = msg.ClassTypeId;

            statsManager.hp = statsManager.maxHp;
            statsManager.mp = statsManager.maxMp;
            statsManager.cp = statsManager.maxCp;
            
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        
    }
}