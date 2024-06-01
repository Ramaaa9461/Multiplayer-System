using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>
{

    public NetworkEntity networkEntity;

    public Action onInitEntity;
    public Action onInitPingPong;

    public int ClientID
    {
        get { return networkEntity.clientID; }
    }
    public bool isServer
    {
        get { return networkEntity is NetworkServer; }
        private set { }
    }
    
    DateTime appStartTime;

    private void Start()
    {
        appStartTime = DateTime.UtcNow;
    }

    public NetworkClient GetNetworkClient()
    {
        if (isServer)
        {
            return null;
        }

        return (NetworkClient)networkEntity;
    }

    public NetworkServer GetNetworkServer()
    {
        if (!isServer)
        {
            return null;
        }

        return (NetworkServer)networkEntity;
    }

    public void StartServer(int port)
    {
        networkEntity = new NetworkServer(port, appStartTime);
        onInitEntity?.Invoke();
        onInitPingPong?.Invoke();
    }

    public void StartClient(IPAddress ip, int port, string name)
    {
        networkEntity = new NetworkClient(ip, port, name);
        onInitEntity?.Invoke();
    }

    private void Update()
    {
        if (networkEntity != null)
        {
            if (isServer)
            {
                NetworkServer server = GetNetworkServer();

                if (server != null)
                {
                    server.Update();
                }
            }
            else
            {
                NetworkClient client = GetNetworkClient();

                if (client != null)
                {
                    client.Update();
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        networkEntity.OnApplicationQuit();
    }
}