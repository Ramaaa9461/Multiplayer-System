using System;
using System.Collections.Generic;

namespace Net
{
    public class ServerNondisponsableMessage : NondisponsableMessageBase
    {
        private Dictionary<int, Dictionary<MessageType, Queue<byte[]>>> LastMessageBroadcastToClients = new Dictionary<int, Dictionary<MessageType, Queue<byte[]>>>();
        private Dictionary<int, Dictionary<MessageType, float>> resendPackageCounterToClients = new Dictionary<int, Dictionary<MessageType, float>>();

        public ServerNondisponsableMessage(NetworkEntity networkEntity) : base(networkEntity)
        {
            networkEntity.OnNewPlayer += AddNewClient;
            networkEntity.OnRemovePlayer += RemoveClient;
        }

        protected override void HandleConfirmationMessage(NetConfirmMessage netConfirm, int id)
        {
            if (LastMessageBroadcastToClients.ContainsKey(id))
            {
                var clientMessages = LastMessageBroadcastToClients[id];

                if (clientMessages.ContainsKey(netConfirm.GetData()) && clientMessages[netConfirm.GetData()].Count > 0)
                {
                    byte[] message = clientMessages[netConfirm.GetData()].Peek();

                    if (MessagesHistory.ContainsKey(message))
                    {
                        clientMessages[netConfirm.GetData()].Dequeue();
                    }
                    else
                    {
                        MessagesHistory.Add(clientMessages[netConfirm.GetData()].Dequeue(), secondsToDeleteMessageHistory);
                    }
                }
            }
        }

        public override void AddSentMessages(byte[] data, int clientId)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                if (!LastMessageBroadcastToClients.ContainsKey(clientId))
                {
                    LastMessageBroadcastToClients.Add(clientId, new Dictionary<MessageType, Queue<byte[]>>());
                }

                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!LastMessageBroadcastToClients[clientId].ContainsKey(messageType))
                {
                    LastMessageBroadcastToClients[clientId].Add(messageType, new Queue<byte[]>());
                }

                LastMessageBroadcastToClients[clientId][messageType].Enqueue(data);
            }
        }

        public override void ResendPackages()
        {
            if (resendPackageCounterToClients.Count > 0)
            {
                foreach (int id in resendPackageCounterToClients.Keys)
                {
                    foreach (MessageType messageType in resendPackageCounterToClients[id].Keys)
                    {
                        resendPackageCounterToClients[id][messageType] += pingPong.deltaTime;

                        if (resendPackageCounterToClients[id][messageType] >= ((ServerPingPong)pingPong).GetLatencyFormClient(id) * 5)
                        {
                            if (LastMessageBroadcastToClients[id][messageType].Count > 0)
                            {
                                Console.WriteLine("Resending package to client " + id);
                                networkEntity.SendMessage(LastMessageBroadcastToClients[id][messageType].Peek(), id);
                                resendPackageCounterToClients[id][messageType] = 0;
                            }
                        }
                    }
                }
            }

            CleanupMessageHistory();
        }
        private void AddNewClient(int clientID)
        {
            LastMessageBroadcastToClients.Add(clientID, new Dictionary<MessageType, Queue<byte[]>>());
            resendPackageCounterToClients.Add(clientID, new Dictionary<MessageType, float>());
        }

        private void RemoveClient(int clientID)
        {
            LastMessageBroadcastToClients.Remove(clientID);
            resendPackageCounterToClients.Remove(clientID);
        }

        private void CleanupMessageHistory()
        {
            if (MessagesHistory.Count > 0 && pingPong != null)
            {
                List<byte[]> keysToRemove = new List<byte[]>(MessagesHistory.Count);

                foreach (byte[] messageKey in MessagesHistory.Keys)
                {
                    keysToRemove.Add(messageKey);
                }

                foreach (byte[] messageKey in keysToRemove)
                {
                    MessagesHistory[messageKey] -= pingPong.deltaTime;

                    if (MessagesHistory[messageKey] <= 0)
                    {
                        MessagesHistory.Remove(messageKey);
                    }
                }
            }
        }

        public override void SendConfirmationMessage(MessageType messageType, int id = -1)
        {
            NetConfirmMessage netConfirmMessage = new NetConfirmMessage(MessagePriority.Default, messageType);
            networkEntity.SendMessage(netConfirmMessage.Serialize(), id);
        }
    }
}