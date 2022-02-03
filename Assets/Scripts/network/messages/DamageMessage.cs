using Mirror;

namespace network.messages
{
    public struct DamageMessage: NetworkMessage
    {
        public uint IssuerNetId { get; set; }
        public uint TargetNetId { get; set; }
        public int Amount { get; set; }
    }
}