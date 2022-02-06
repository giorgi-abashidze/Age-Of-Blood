using System;
using System.Collections;
using helpers;
using Mirror;
using network.messages;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace controllers
{
    public class MovementManager: NetworkBehaviour
    {
        
        private NavMeshAgent _agent;
        private Camera _mainCam;
        [FormerlySerializedAs("_canMove")]
        public bool canMove = true;
        [FormerlySerializedAs("_destination")]
        [SyncVar(hook = nameof(OnDestinationChanged))]
        public Vector3 destination = Vector3.zero;
        [FormerlySerializedAs("_currentPosition")]
        [SyncVar(hook = nameof(OnCurrentPositionChanged))]
        public Vector3 currentPosition = Vector3.zero;

        private bool _moving = false;

        [Command]
        void CmdRequestMove(MoveRequest request)
        {
            var dist = Vector3.Distance(request.CurrentPoint, request.TargetPoint);

            if (dist > Constants.MoveRange)
                return;
            
            destination = request.TargetPoint;
        }
        
        [Command]
        void CmdNotifyArrived(Vector3 point)
        {
            destination = Vector3.zero;
            currentPosition = point;
        }

        public void OnDestinationChanged(Vector3 oldValue,Vector3 newValue)
        {
            if (canMove && newValue != Vector3.zero)
            {
                if(_agent.isStopped)
                    _agent.isStopped = false;
                _agent.ResetPath();
                _agent.destination = newValue;

                _moving = true;
            }

        }
        
        public void OnCurrentPositionChanged(Vector3 oldValue,Vector3 newValue)
        {
            _agent.velocity = Vector3.zero;
            _agent.ResetPath();
            _agent.isStopped = true;
            _moving = false;
            var dist = Vector3.Distance(transform.position, newValue);
            if (dist > 2)
            {
                transform.position = newValue;
                
            }

        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _mainCam = Camera.main;
        }

        
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            
        }
        
        void Update()
        {
            
            if (!isLocalPlayer)
                return;

            
            if (_agent.remainingDistance <= _agent.stoppingDistance && _moving)
            {
                _moving = false;
                _agent.velocity = Vector3.zero;
                CmdNotifyArrived(transform.position);
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out var hit, Constants.MoveRange)) {
                    if(hit.transform.CompareTag("Terrain") && canMove){
                       
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