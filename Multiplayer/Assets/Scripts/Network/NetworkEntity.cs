using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class NetworkEntity : IReceiveData
{
    /// <summary>
    /// The port of the network manager.
    /// </summary>
    public int port
    {
        get; protected set;
    }

    protected UdpConnection connection;
    public Action<byte[], IPEndPoint> OnReceivedMessage;

    public string userName = "Server";
    public int clientID = -1;

    public PingPong checkActivity;

    protected GameManager gm;
    protected SortableMessages sortableMessages;
    protected NondisponsablesMessages nonDisposablesMessages;

    public NetworkEntity()
    {
        gm = GameManager.Instance;
        sortableMessages = new();
        nonDisposablesMessages = new();
    }

    public abstract void AddClient(IPEndPoint ip, int newClientID, string clientName);
    public abstract void RemoveClient(int idToRemove);

    public abstract void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);

    /// <summary>
    /// Updates the network manager.
    /// </summary>
    public virtual void Update()
    {
        // Flush the data in the main thread
        if (connection != null)
        {
            connection.FlushReceiveData();
            checkActivity?.UpdateCheckActivity();
            nonDisposablesMessages?.ResendPackages();
        }
    }

    protected abstract void UpdateChatText(byte[] data, IPEndPoint ip);
    protected abstract void UpdatePlayerPosition(byte[] data);

}
