using System;
using Mirror;
using UnityEngine;

namespace controllers
{
    public class InputController : NetworkBehaviour
    {
        private AbilityManager _abilityManager;

        private void Start()
        {
            _abilityManager = gameObject.GetComponent<AbilityManager>();
        }

        void OnGUI()
        {
            if (!isLocalPlayer)
                return;
            
            Event e = Event.current;
            if (e.isKey && e.type == EventType.KeyDown)
            {

                if(e.keyCode >= KeyCode.F1 && e.keyCode <= KeyCode.F12){
                    _abilityManager.UseAbilityFromFPanel(e.keyCode);
                }
                else if(e.keyCode >= KeyCode.Alpha0 && e.keyCode <= KeyCode.Alpha9){
                    _abilityManager.UseAbilityFromNumPanel(e.keyCode);
                }
            
            }
        
        }
    }
}