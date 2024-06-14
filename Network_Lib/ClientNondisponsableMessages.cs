using System;
using System.Collections.Generic;



namespace Net
{
    public class ClientNondisponsableMessage : NondisponsableMessageBase
    {
        private Dictionary<MessageType, Queue<byte[]>> LastMessageSendToServer = new Dictionary<MessageType, Queue<byte[]>>();
        private Dictionary<MessageType, float> resendPackageCounterToServer = new Dictionary<MessageType, float>();

        public ClientNondisponsableMessage(NetworkEntity networkEntity) : base(networkEntity) { }

        protected override void HandleConfirmationMessage(NetConfirmMessage netConfirm, int id)
        {
            if (LastMessageSendToServer.ContainsKey(netConfirm.GetData()) && LastMessageSendToServer[netConfirm.GetData()].Count > 0)
            {
                byte[] message = LastMessageSendToServer[netConfirm.GetData()].Peek();

                if (MessagesHistory.ContainsKey(message))
                {
                    LastMessageSendToServer[netConfirm.GetData()].Dequeue();
                }
                else
                {
                    MessagesHistory.Add(LastMessageSendToServer[netConfirm.GetData()].Dequeue(), secondsToDeleteMessageHistory);
                }
            }
        }

        public override void AddSentMessages(byte[] data, int clientId = -1)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);

                if (!LastMessageSendToServer.ContainsKey(messageType))
                {
                    LastMessageSendToServer.Add(messageType, new Queue<byte[]>());
                }

                LastMessageSendToServer[messageType].Enqueue(data);
            }
        }
        public override void ResendPackages()
        {
            if (resendPackageCounterToServer.Count > 0)
            {
                foreach (MessageType messageType in resendPackageCounterToServer.Keys)
                {
                    resendPackageCounterToServer[messageType] += pingPong.deltaTime;

                    if (resendPackageCounterToServer[messageType] >= ((ClientPingPong)pingPong).GetLatencyFormServer() * 5)
                    {
                        if (LastMessageSendToServer[messageType].Count > 0)
                        {
                            Console.WriteLine("Resending package to server");
                            networkEntity.SendMessage(LastMessageSendToServer[messageType].Peek());
                            resendPackageCounterToServer[messageType] = 0;
                        }
                    }
                }
            }

            CleanupMessageHistory();
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
            networkEntity.SendMessage(netConfirmMessage.Serialize());
        }
    }
}