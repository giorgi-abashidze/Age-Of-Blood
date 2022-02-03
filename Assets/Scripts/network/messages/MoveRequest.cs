using Mirror;
using UnityEngine;

namespace network.messages
{
    public struct MoveRequest: NetworkMessage
    {
        public uint IssuerNetId;
        public Vector3 TargetPoint;
        public Vector3 CurrentPoint;
        
    }
}