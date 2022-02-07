﻿using System;
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
        
        private Camera _mainCam;
        [FormerlySerializedAs("_canMove")]
        [SyncVar]
        public bool canMove = true;
        [FormerlySerializedAs("_destination")]
        [SyncVar(hook = nameof(OnDestinationChanged))]
        public Vector3 destination = Vector3.zero;
        [FormerlySerializedAs("_currentPosition")]
        [SyncVar(hook = nameof(OnCurrentPositionChanged))]
        public Vector3 currentPosition = Vector3.zero;

        private Rigidbody _rigidbody;
        
        private bool _moving = false;
        private bool _rotating = false;

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
                _moving = true;
                _rotating = true;
            }

        }
        
        public void OnCurrentPositionChanged(Vector3 oldValue,Vector3 newValue)
        {

            _moving = false;
            _rotating = false;
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
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
           if(destination != Vector3.zero && canMove)
               _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, destination, 2 * Time.deltaTime));

           if (destination != Vector3.zero && _rotating)
           {
               
               var localTarget = transform.InverseTransformPoint(destination);
     
               var angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
 
               var eulerAngleVelocity  = new Vector3 (0, angle, 0);
               var deltaRotation  = Quaternion.Euler(eulerAngleVelocity * 5 * Time.deltaTime );
               
               _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);

               if (angle >= -0.5 && angle <= 0.5)
                   _rotating = false;
           }
 
        }

        private void LateUpdate()
        {
            if (_rigidbody.position == destination && _moving)
            {
                _moving = false;
                _rigidbody.velocity = Vector3.zero;
                if(isLocalPlayer)
                    CmdNotifyArrived(transform.position);
            }
        }

        void Update()
        {
            
            if (!isLocalPlayer)
                return;

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