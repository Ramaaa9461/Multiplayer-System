using System.Net;

public class ServerSortableMessage : SortableMessagesBase
{
    public ServerSortableMessage(NetworkEntity networkEntity) : base(networkEntity)
    {
        networkEntity.OnReceivedMessage += OnRecievedData;
        networkEntity.OnNewPlayer += AddNewClient;
        networkEntity.OnRemovePlayer += RemoveClient;
    }

    protected override void OnRecievedData(byte[] data, IPEndPoint ip)
    {
        MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

        if ((messagePriority & MessagePriority.Sorteable) != 0)
        {
            MessageType messageType = MessageChecker.CheckMessageType(data);
            int messageTypeIndex = (int)messageType;

            NetworkServer server = networkEntity.GetNetworkServer();

            if (server.ipToId.ContainsKey(ip))
            {
                int clientId = server.ipToId[ip];
                if (clientToRowMapping.ContainsKey(clientId))
                {
                    int row = clientToRowMapping[clientId];
                    OrderLastMessageReciveFromClients.Set(row, messageTypeIndex, !OrderLastMessageReciveFromClients.Get(row, messageTypeIndex));
                }
            }
        }
    }
}
