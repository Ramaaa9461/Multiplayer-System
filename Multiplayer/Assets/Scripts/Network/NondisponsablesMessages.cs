using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NondisponsablesMessages // Reworked to use DynamicByteQueueMatrix
{
    private NetworkEntity networkEntity;

    private Dictionary<MessageType, Queue<byte[]>> LastMessageSendToServer;
    private DynamicByteQueueMatrix LastMessageBroadcastToClients;

    private Dictionary<byte[], float> MessagesHistory = new();
    private int secondsToDeleteMessageHistory = 15;

    private PingPong pingPong;

    private DynamicFloatQueueMatrix resendPackageCounterToClients;
    private Dictionary<MessageType, float> resendPackageCounterToServer;

    public NondisponsablesMessages(NetworkEntity networkEntity)
    {
        this.networkEntity = networkEntity;

        pingPong = networkEntity.checkActivity;
        Debug.Log(pingPong + " - " + networkEntity.checkActivity);

        networkEntity.OnReceivedMessage += OnRecievedData;

        networkEntity.OnNewPlayer += AddNewClient;
        networkEntity.OnRemovePlayer += RemoveClient;

        LastMessageSendToServer = new Dictionary<MessageType, Queue<byte[]>>();
        LastMessageBroadcastToClients = new DynamicByteQueueMatrix();

        resendPackageCounterToClients = new DynamicFloatQueueMatrix();
        resendPackageCounterToServer = new Dictionary<MessageType, float>();
    }

    void OnRecievedData(byte[] data, IPEndPoint ip)
    {
        MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);
        MessageType messageType = MessageChecker.CheckMessageType(data);

        if ((messagePriority & MessagePriority.NonDisposable) != 0)
        {
            NetConfirmMessage netConfirmMessage = new NetConfirmMessage(MessagePriority.Default, messageType);

            if (networkEntity.isServer)
            {
                //networkEntity.GetNetworkServer().Broadcast(netConfirmMessage.Serialize(), ip);
            }
            else
            {
               // networkEntity.GetNetworkClient().SendToServer(netConfirmMessage.Serialize());
            }
        }

        if (messageType == MessageType.Confirm)
        {
            NetConfirmMessage netConfirm = new(data);

            if (networkEntity.isServer)
            {
                NetworkServer server = networkEntity.GetNetworkServer();

                if (server.ipToId.ContainsKey(ip))
                {
                    int clientId = server.ipToId[ip];
                    Queue<byte[]> clientMessages = LastMessageBroadcastToClients.Get(clientId, netConfirm.GetData());

                    if (clientMessages.Count > 0)
                    {
                        byte[] message = clientMessages.Peek();
                        Debug.Log(MessageChecker.CheckMessageType(message));

                        if (MessagesHistory.ContainsKey(message))
                        {
                            clientMessages.Dequeue();
                        }
                        else
                        {
                            MessagesHistory.Add(clientMessages.Dequeue(), secondsToDeleteMessageHistory);
                        }
                    }
                }
            }
            else
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
        }
    }

    public void AddSentMessagesFromServer(byte[] data, int clientId) // Cada vez que haces un broadcast
    {
        if (networkEntity.isServer)
        {
            MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);

            if ((messagePriority & MessagePriority.NonDisposable) != 0)
            {
                MessageType messageType = MessageChecker.CheckMessageType(data);
                Queue<byte[]> clientMessages = LastMessageBroadcastToClients.Get(clientId, messageType);

                clientMessages.Enqueue(data);
                LastMessageBroadcastToClients.Set(clientId, messageType, clientMessages);
                resendPackageCounterToClients.Set(clientId, messageType, new Queue<float>());
            }
        }
    }

    public void AddSentMessagesFromClients(byte[] data)
    {
        if (!networkEntity.isServer)
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
                resendPackageCounterToServer[messageType] = 0;
            }
        }
    }

    void AddNewClient(int clientID)
    {
        if (networkEntity.isServer)
        {
            LastMessageBroadcastToClients.Set(clientID, MessageType.Default, new Queue<byte[]>());
            resendPackageCounterToClients.Set(clientID, MessageType.Default, new Queue<float>());
        }
    }

    void RemoveClient(int clientID)
    {
        if (networkEntity.isServer)
        {
            LastMessageBroadcastToClients.ClearRow(clientID);
            resendPackageCounterToClients.ClearRow(clientID);
        }
    }

    public void ResendPackages()
    {
        if (networkEntity.isServer)
        {
            NetworkServer server = networkEntity.GetNetworkServer();

            for (int id = 0; id < resendPackageCounterToClients.Rows; id++)
            {
                for (int messageTypeIndex = 0; messageTypeIndex < (int)MessageType.Winner; messageTypeIndex++)
                {
                    MessageType messageType = (MessageType)messageTypeIndex;
                    Queue<float> clientCounters = resendPackageCounterToClients.Get(id, messageType);

                    if (clientCounters.Count > 0)
                    {
                        float counter = clientCounters.Dequeue();
                        counter += pingPong.deltaTime;

                        if (counter >= ((ServerPingPong)pingPong).GetLatencyFormClient(id) * 5)
                        {
                            Queue<byte[]> clientMessages = LastMessageBroadcastToClients.Get(id, messageType);

                            if (clientMessages.Count > 0)
                            {
                                Debug.Log("Se envio el packete de nuevo hacia el cliente " + id);
                                server.Broadcast(clientMessages.Peek(), server.clients[id].ipEndPoint);
                                counter = 0;
                            }
                        }

                        clientCounters.Enqueue(counter);
                        resendPackageCounterToClients.Set(id, messageType, clientCounters);
                    }
                }
            }
        }
        else
        {
            List<MessageType> keys = new List<MessageType>(resendPackageCounterToServer.Keys);

            if (keys.Count > 0)
            {
                foreach (MessageType messageType in keys)
                {
                    if (LastMessageSendToServer[messageType].Count > 0)
                    {
                        resendPackageCounterToServer[messageType] += pingPong.deltaTime;

                        if (resendPackageCounterToServer[messageType] >= ((ClientPingPong)pingPong).GetLatencyFormServer() * 5)
                        {
                            Debug.Log("Se envio el packete de nuevo hacia el server");
                            networkEntity.GetNetworkClient().SendToServer(LastMessageSendToServer[messageType].Peek());
                            resendPackageCounterToServer[messageType] = 0;
                        }
                    }
                }
            }
        }

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
}