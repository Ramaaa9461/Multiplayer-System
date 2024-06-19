using System;
using System.Net;

namespace Net
{
    public abstract class NetworkEntity : IReceiveData
    {
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

        public NetworkEntity()
        {
            NetObjFactory.SetNetworkEntity(this);
        }

        public abstract void AddClient(IPEndPoint ip, int newClientID, string clientName);

        public abstract void RemoveClient(int idToRemove);

        public abstract void CloseConnection();

        public abstract void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);

        public abstract void SendMessage(byte[] data, int id);

        public abstract void SendMessage(byte[] data);

        public virtual void Update()
        {
            if (connection != null)
            {
                connection.FlushReceiveData();
                checkActivity?.UpdateCheckActivity();
            }
        }

        protected abstract void UpdateChatText(byte[] data);

        protected abstract void UpdatePlayerPosition(byte[] data);

        public abstract void OnApplicationQuit();

    }
}