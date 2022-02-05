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
        
        public bool isServer;

        public override void Start()
        {
            base.Start();

            
            playerPrefab = Resources.LoadAll<GameObject>(SpawnableDir)[0];
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