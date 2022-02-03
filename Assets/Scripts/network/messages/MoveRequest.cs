using Mirror;
using UnityEngine;

namespace network.messages
{
    public struct MoveRequest: NetworkMessage
    {
        public uint IssuerNetId { get; set; }
        public Vector3 TargetPoint { get; set; }
        public Vector3 CurrentPoint { get; set; }
        
    }
}