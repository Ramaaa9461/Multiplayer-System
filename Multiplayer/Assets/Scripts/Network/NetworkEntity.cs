using System;
using System.Net;

public abstract class NetworkEntity : IReceiveData
{

    public bool isServer
    {
        get { return this is NetworkServer; }
        private set { }
    }
    public NetworkClient GetNetworkClient()
    {
        if (isServer)
        {
            return null;
        }

        return (NetworkClient)this;
    }
    public NetworkServer GetNetworkServer()
    {
        if (!isServer)
        {
            return null;
        }

        return (NetworkServer)this;
    }

    public int port
    {
        get; protected set;
    }

    public Action onInitPingPong;
    public Action<int, Vec3> OnInstantiateBullet;
    public Action<int> OnNewPlayer;
    public Action<int> OnRemovePlayer;

    protected UdpConnection connection;
    public Action<byte[], IPEndPoint> OnReceivedMessage;

    public string userName = "Server";
    public int clientID = 0;

    public PingPong checkActivity;

    protected GameManager gm;
    protected SortableMessages sortableMessages;
    protected NondisponsablesMessages nonDisposablesMessages;

    public NetworkEntity()
    {
        gm = GameManager.Instance;

        onInitPingPong += ()=> sortableMessages = new(this);
        onInitPingPong += () => nonDisposablesMessages = new(this);
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
    
    public abstract void OnApplicationQuit();


}
