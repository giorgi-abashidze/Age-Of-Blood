using Mirror;
using network.messages;
using UnityEngine;
using UnityEngine.AI;

namespace controllers
{
    public class PlayerController : NetworkBehaviour
    {
        
        private GameObject _target;
        private uint _targetNetId = 0;
        private RaycastHit _hit;
        private byte _classId;
        private byte _level = 1;
       
        public byte GetLevel()
        {
            return _level;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            NetworkClient.RegisterHandler<DamageMessage>(OnNotifyDamage);
        }

        public GameObject GetTarget()
        {
            return _target;
        }
        
        public byte GetClassId()
        {
            return _classId;
        }
        
        public uint GetTargetNetId()
        {
            return _targetNetId;
        }
        
        [ClientCallback]
        private void OnNotifyDamage(DamageMessage message)
        {
            if (!isLocalPlayer)
                return;
            
            if (message.TargetNetId == netId)
            {
              
                ApplyDamage(message.Amount);
                ShowReceivedDamagePopup(message.Amount);
           
            }
            else if (message.IssuerNetId == netId)
            {
                
                ShowGivenDamagePopup(message.Amount);
            }
        }

        //todo:nothing inside yet 
        private void ApplyDamage(int amount)
        {
            
        }
        
        //todo:nothing inside yet 
        private void ShowReceivedDamagePopup(int amount)
        {
            
        }
        
        //todo:nothing inside yet 
        private void ShowGivenDamagePopup(int amount)
        {
            
        }

        
    }
}
