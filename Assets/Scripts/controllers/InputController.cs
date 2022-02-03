using Mirror;
using UnityEngine;

namespace controllers
{
    public class InputController : NetworkBehaviour
    {
        
        void OnGUI()
        {
            if (!isLocalPlayer)
                return;
            
            Event e = Event.current;
            if (e.isKey && e.type == EventType.KeyDown)
            {

                if(e.keyCode >= KeyCode.F1 && e.keyCode <= KeyCode.F12){
                    AbilityManager.Instance.UseAbilityFromFPanel(e.keyCode);
                }
                else if(e.keyCode >= KeyCode.Alpha0 && e.keyCode <= KeyCode.Alpha9){
                    AbilityManager.Instance.UseAbilityFromNumPanel(e.keyCode);
                }
            
            }
        
        }
    }
}