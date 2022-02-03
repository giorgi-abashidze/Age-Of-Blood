using System;
using Cinemachine;
using Mirror;
using UnityEngine;

namespace controllers
{
    public class CameraController: NetworkBehaviour{

        private const float MouseSensitivity = 80f;

        private CinemachineFreeLook _cam;

        private const float MaxHeight = 12.0f;
        private const float MinHeight = 3.0f;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            CinemachineCore.GetInputAxis = GetAxisCustom;
            _cam = GameObject.FindGameObjectWithTag("freelookcam").GetComponent<CinemachineFreeLook>();
            var transform1 = transform;
            _cam.LookAt = transform1;
            _cam.Follow = transform1;
        }
        

        [Client]
        private float GetAxisCustom(string axisName)
        {

            var scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll < 0)
            {

                if (_cam.m_Orbits[0].m_Height > MinHeight) {
                    _cam.m_Orbits[1].m_Height -= Math.Abs(scroll) ;
                    _cam.m_Orbits[0].m_Height -= Math.Abs(scroll) * 1.5f;
                    _cam.m_Orbits[1].m_Radius -= Math.Abs(scroll) * 1.5f;
                    _cam.m_Orbits[0].m_Radius -= Math.Abs(scroll);
                    _cam.m_Orbits[2].m_Radius -= Math.Abs(scroll) * 1.5f;
                }


            }
            else if (scroll > 0)
            {
                if (!(_cam.m_Orbits[1].m_Height < MaxHeight))
                    return axisName switch
                    {
                        "Mouse X" => Input.GetMouseButton(1) ? Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime : 0,
                        "Mouse Y" => Input.GetMouseButton(1) ? Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime : 0,
                        _ => Input.GetAxis(axisName)
                    };
                _cam.m_Orbits[1].m_Height += scroll ;
                _cam.m_Orbits[0].m_Height += scroll * 1.5f;
                _cam.m_Orbits[1].m_Radius += scroll * 1.5f;
                _cam.m_Orbits[0].m_Radius += scroll;
                _cam.m_Orbits[2].m_Radius += scroll * 1.5f;

            }

            return axisName switch
            {
                "Mouse X" => Input.GetMouseButton(1) ? Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime : 0,
                "Mouse Y" => Input.GetMouseButton(1) ? Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime : 0,
                _ => Input.GetAxis(axisName)
            };
        }
    }
}
