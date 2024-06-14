using System;


namespace Net
{
    public class ClientPingPong : PingPong
    {
        private float lastMessageReceivedFromServer = 0;
        private float latencyFromServer = 0;

        public ClientPingPong(NetworkEntity networkEntity) : base(networkEntity) { }

        public void ReciveServerToClientPingMessage()
        {
            lastMessageReceivedFromServer = 0;
        }

        protected override void CheckActivityCounter(float deltaTime)
        {
            lastMessageReceivedFromServer += deltaTime;
        }

        protected override void CheckTimeUntilDisconection()
        {
            if (lastMessageReceivedFromServer > timeUntilDisconnection)
            {
                NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, networkEntity.clientID);
                networkEntity.SendMessage(netDisconnection.Serialize());
                networkEntity.CloseConnection();
            }
        }

        protected override void SendPingMessage()
        {
            NetPing netPing = new NetPing();
            networkEntity.SendMessage(netPing.Serialize());
            currentDateTime = DateTime.UtcNow;
        }

        public void CalculateLatencyFromServer()
        {
            TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
            latencyFromServer = (float)(newDateTime.TotalMilliseconds / 1000);

            //  UnityEngine.Debug.Log("Calculate Latency from Server " + latencyFromServer / 1000);
        }

        public float GetLatencyFormServer()
        {
            // UnityEngine.Debug.Log("Latency from Server: " + latencyFromServer);
            return latencyFromServer;
        }
    }
}