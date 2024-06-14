

namespace Net
{
    public class ClientSortableMessage : SortableMessagesBase
    {
        public ClientSortableMessage(NetworkEntity networkEntity) : base(networkEntity)
        {
            networkEntity.OnNewPlayer += AddNewClient;
            networkEntity.OnRemovePlayer += RemoveClient;
        }

        public override void OnRecievedData(byte[] data, int id)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.Sorteable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);
                int messageTypeIndex = (int)messageType;

                if (messageType == MessageType.Position)
                {
                    int clientId = new NetVector3(data).GetData().id;
                    if (clientToRowMapping.ContainsKey(clientId))
                    {
                        int row = clientToRowMapping[clientId];
                        OrderLastMessageReciveFromServer.Set(row, messageTypeIndex, !OrderLastMessageReciveFromServer.Get(row, messageTypeIndex));
                    }
                }
            }
        }
    }
}