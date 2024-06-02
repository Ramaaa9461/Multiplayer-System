using System;
using System.Collections.Generic;
using UnityEngine;

public class PingPong
{
    NetworkEntity networkEntity;

    public float deltaTime = 0;

    int timeUntilDisconnection = 5;

    private Dictionary<int, float> lastMessageReceivedFromClients = new Dictionary<int, float>(); //Lo usa el Server
    float lastMessageReceivedFromServer = 0; //Lo usan los clientes

    float sendMessageCounter = 0;
    float secondsPerCheck = 1.0f;


    private Dictionary<int, float> latencyFromClients = new Dictionary<int, float>(); //Lo usa el Server
    float latencyFromServer = 0;


    DateTime currentDateTime;
    DateTime lastUpdateTime = DateTime.UtcNow;

    public PingPong(NetworkEntity networkEntity) 
    {
        this.networkEntity = networkEntity;
    }

    public void AddClientForList(int idToAdd)
    {
        lastMessageReceivedFromClients.Add(idToAdd, 0.0f);
    }

    public void RemoveClientForList(int idToRemove)
    {
        lastMessageReceivedFromClients.Remove(idToRemove);
    }

    public void ReciveServerToClientPingMessage()
    {
        lastMessageReceivedFromServer = 0;
    }

    public void ReciveClientToServerPingMessage(int playerID)
    {
        lastMessageReceivedFromClients[playerID] = 0;
    }

    public void UpdateCheckActivity()
    {
        DateTime currentTime = DateTime.UtcNow;
        deltaTime = (float)(currentTime - lastUpdateTime).TotalSeconds;
        lastUpdateTime = currentTime;

        sendMessageCounter += deltaTime;

        if (sendMessageCounter > secondsPerCheck) //Envio cada 1 segundo el mensaje
        {
            SendPingMessage();
            sendMessageCounter = 0;
        }

            CheckActivityCounter(deltaTime);
            CheckTimeUntilDisconection();
    }

    void CheckActivityCounter(float deltaTime)
    {
        if (networkEntity.isServer)
        {
            var keys = new List<int>(lastMessageReceivedFromClients.Keys);

            foreach (var key in keys)
            {
                lastMessageReceivedFromClients[key] += deltaTime;
            }
        }
        else
        {
            lastMessageReceivedFromServer += deltaTime;
        }
    }

    void CheckTimeUntilDisconection()
    {
        if (networkEntity.isServer)
        {
            foreach (int clientID in lastMessageReceivedFromClients.Keys)
            {
                if (lastMessageReceivedFromClients[clientID] > timeUntilDisconnection)
                {
                    networkEntity.RemoveClient(clientID);

                    NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
                    networkEntity.GetNetworkServer().Broadcast(netDisconnection.Serialize());
                }
            }
        }
        else
        {
            if (lastMessageReceivedFromServer > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, NetworkManager.Instance.ClientID);
                networkEntity.GetNetworkClient().SendToServer(netDisconnection.Serialize());
                networkEntity.GetNetworkClient().DisconectPlayer();
            }
        }
    }

    void SendPingMessage()
    {
        NetPing netPing = new NetPing();

        if (networkEntity.isServer)
        {
            networkEntity.GetNetworkServer().Broadcast(netPing.Serialize());
        }
        else
        {
            networkEntity.GetNetworkClient().SendToServer(netPing.Serialize());
        }

        currentDateTime = DateTime.UtcNow;
    }

    public void CalculateLatencyFromServer()
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        latencyFromServer = (float)newDateTime.Milliseconds;
      //Debug.Log("Latency from Server " + latencyFromServer / 1000);
    }

    public void CalculateLatencyFromClients(int clientID)
    {
        TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
        latencyFromClients[clientID] = (float)newDateTime.TotalMilliseconds;
      //Debug.Log("Latency from client " + clientID + " - " + latencyFromClients[clientID] /1000);
    }

    public float GetLatencyFormClient(int clientId)
    {
        if (latencyFromClients.ContainsKey(clientId))
        {
            return latencyFromClients[clientId];
        }

        return -1;
    }
    public float GetLatencyFormServer()
    {
        return latencyFromServer;
    }
}
