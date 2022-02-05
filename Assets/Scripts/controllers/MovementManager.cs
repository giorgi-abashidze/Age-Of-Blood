using System;
using helpers;
using Mirror;
using network.messages;
using UnityEngine;
using UnityEngine.AI;

namespace controllers
{
    public class MovementManager: NetworkBehaviour
    {
        
        private NavMeshAgent _agent;
        private Camera _mainCam;
        
        [SyncVar]
        public bool _canMove = true;

        [Command]
        void CmdRequestMove(MoveRequest request)
        {
            var dist = Vector3.Distance(request.CurrentPoint, request.TargetPoint);

            if (dist > Constants.MoveRange)
                return;
            
            OnMoveRequest(request);
        }

        
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _mainCam = Camera.main;
            NetworkClient.RegisterHandler<MoveRequest>(OnMoveRequest);
        }
        
        [ClientRpc]
        private void OnMoveRequest(MoveRequest msg)
        {
            if(netId == msg.IssuerNetId)
                MoveToPoint(msg.TargetPoint);
            else
            {
                if (NetworkClient.spawned.ContainsKey(msg.IssuerNetId))
                {
                    var identity = NetworkClient.spawned[msg.IssuerNetId];
                    var manager = identity.gameObject.GetComponent<MovementManager>();
                    manager.MoveToPoint(msg.TargetPoint);
                }
            }
        }
        
        public void StopAgent()
        {
            _agent.velocity /=  2;
            _agent.ResetPath();
        }

        [Client]
        private void MoveToPoint(Vector3 point)
        {
            if (!_canMove)
                return;
            
            print("moving to point: "+point);
            _agent.velocity /=  2;
            _agent.ResetPath();
            _agent.destination = point;
        }

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        
        void Update()
        {
            if (!_agent.pathPending && _agent.remainingDistance <= 0.3)
            {
                _agent.velocity = Vector3.zero;
                _agent.ResetPath();
            
            }
            
            if (!isLocalPlayer)
                return;
            
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out var hit, Constants.MoveRange)) {
                    if(hit.transform.CompareTag("Terrain") && _canMove){
                        CmdRequestMove(new MoveRequest
                        {
                            IssuerNetId = netId,
                            CurrentPoint = transform.position,
                            TargetPoint = hit.point
                        });
                    } 
                }
            }
            
    
        }
    }
}