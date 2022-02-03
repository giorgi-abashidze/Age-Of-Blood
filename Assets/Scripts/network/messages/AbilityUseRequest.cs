using Mirror;

namespace network.messages
{
    public struct AbilityUseRequest: NetworkMessage
    {
        public uint IssuerNetId { get; set; }
        public uint TargetNetId { get; set; }
        public ushort SkillId { get; set; }
        public byte IssuerClassId { get; set; }
        public byte IssuerLevel { get; set; }
        
    }
}