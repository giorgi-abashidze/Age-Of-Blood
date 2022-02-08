using Mirror;

namespace network.messages
{
    public struct CreateCharacterMessage: NetworkMessage
    {
        public string Name;
        public byte ClassId;
        public byte ClassTypeId;
    }
}