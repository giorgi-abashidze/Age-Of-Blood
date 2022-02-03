using Mirror;

namespace network.messages
{
    public struct CreateCharacterMessage: NetworkMessage
    {
        public string Name;
    }
}