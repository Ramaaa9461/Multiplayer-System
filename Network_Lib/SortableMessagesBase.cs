using System.Collections.Generic;


namespace Net
{

    public abstract class SortableMessagesBase
    {
        protected NetworkEntity networkEntity;

        protected DynamicBitMatrix OrderLastMessageReciveFromServer;
        protected DynamicBitMatrix OrderLastMessageReciveFromClients;

        protected Dictionary<int, int> clientToRowMapping;
        protected int messageTypeCount;

        public SortableMessagesBase(NetworkEntity networkEntity)
        {
            this.networkEntity = networkEntity;

            messageTypeCount = System.Enum.GetValues(typeof(MessageType)).Length;

            OrderLastMessageReciveFromClients = new DynamicBitMatrix(messageTypeCount);
            OrderLastMessageReciveFromServer = new DynamicBitMatrix(messageTypeCount);

            clientToRowMapping = new Dictionary<int, int>();
        }

        public abstract void OnRecievedData(byte[] data, int id);

        public bool CheckMessageOrderRecievedFromClients(int clientID, MessageType messageType, int messageOrder)
        {
            if (clientToRowMapping.ContainsKey(clientID))
            {
                int row = clientToRowMapping[clientID];
                int messageTypeIndex = (int)messageType;

                return OrderLastMessageReciveFromClients.Get(row, messageTypeIndex) == (messageOrder % 2 == 1);
            }
            return false;
        }

        public bool CheckMessageOrderRecievedFromServer(int clientID, MessageType messageType, int messageOrder)
        {
            if (clientToRowMapping.ContainsKey(clientID))
            {
                int row = clientToRowMapping[clientID];
                int messageTypeIndex = (int)messageType;

                return OrderLastMessageReciveFromServer.Get(row, messageTypeIndex) == (messageOrder % 2 == 1);
            }
            return false;
        }

        protected void AddNewClient(int clientID)
        {
            if (!clientToRowMapping.ContainsKey(clientID))
            {
                int newRow = OrderLastMessageReciveFromClients.Rows;
                clientToRowMapping[clientID] = newRow;
                OrderLastMessageReciveFromClients.Set(newRow, 0, false); // Ensure a new row is added
                OrderLastMessageReciveFromServer.Set(newRow, 0, false); // Ensure a new row is added
            }
        }

        protected void RemoveClient(int clientID)
        {
            if (clientToRowMapping.ContainsKey(clientID))
            {
                int row = clientToRowMapping[clientID];
                clientToRowMapping.Remove(clientID);
                OrderLastMessageReciveFromClients.ClearRow(row);
                OrderLastMessageReciveFromServer.ClearRow(row);
            }
        }

    }
}