namespace Net
{
    public class ServerSortableMessage : SortableMessagesBase
    {
        public ServerSortableMessage(NetworkEntity networkEntity) : base(networkEntity)
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

                int clientId = id;
                if (clientToRowMapping.ContainsKey(clientId))
                {
                    int row = clientToRowMapping[clientId];
                    OrderLastMessageReciveFromClients.Set(row, messageTypeIndex, !OrderLastMessageReciveFromClients.Get(row, messageTypeIndex));
                }
            }
        }
    }
}