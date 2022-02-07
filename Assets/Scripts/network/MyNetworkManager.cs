using System;
using System.Collections.Generic;
using System.Linq;
using helpers;
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
        public static readonly SortedDictionary<ushort,Skill> AllAbilities = new SortedDictionary<ushort,Skill>();
        public static readonly SortedDictionary<byte,Race> Races = new SortedDictionary<byte,Race>();
        public static readonly SortedDictionary<byte,ClassPath> Classes = new SortedDictionary<byte,ClassPath>();
        public static readonly SortedDictionary<byte,ClassType> ClassTypes = new SortedDictionary<byte,ClassType>();
        
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

            
            NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        }
        
        
        
        [ServerCallback]
        private void OnCreateCharacter(NetworkConnection conn,CreateCharacterMessage msg)
        {
          
            var player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        
    }
}