using System;
using System.Collections.Generic;

namespace Net
{
    public class ServerPingPong : PingPong
    {
        private Dictionary<int, float> lastMessageReceivedFromClients = new Dictionary<int, float>();
        private Dictionary<int, float> latencyFromClients = new Dictionary<int, float>();

        public ServerPingPong(NetworkEntity networkEntity) : base(networkEntity) { }

        public void AddClientForList(int idToAdd)
        {
            lastMessageReceivedFromClients.Add(idToAdd, 0.0f);
        }

        public void RemoveClientForList(int idToRemove)
        {
            lastMessageReceivedFromClients.Remove(idToRemove);
        }

        public void ReciveClientToServerPingMessage(int playerID)
        {
            lastMessageReceivedFromClients[playerID] = 0;
        }

        protected override void CheckActivityCounter(float deltaTime)
        {
            var keys = new List<int>(lastMessageReceivedFromClients.Keys);

            foreach (var key in keys)
            {
                lastMessageReceivedFromClients[key] += deltaTime;
            }
        }

        protected override void CheckTimeUntilDisconection()
        {
            foreach (int clientID in lastMessageReceivedFromClients.Keys)
            {
                if (lastMessageReceivedFromClients[clientID] > timeUntilDisconnection)
                {
                    networkEntity.RemoveClient(clientID);

                    NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
                    networkEntity.SendMessage(netDisconnection.Serialize());
                }
            }
        }

        protected override void SendPingMessage()
        {
            NetPing netPing = new NetPing();
            networkEntity.SendMessage(netPing.Serialize());
            currentDateTime = DateTime.UtcNow;
        }

        public void CalculateLatencyFromClients(int clientID)
        {
            TimeSpan newDateTime = DateTime.UtcNow - currentDateTime;
            latencyFromClients[clientID] = (float)(newDateTime.TotalMilliseconds / 1000);

            // UnityEngine.Debug.Log("Calculate Latency from client " + clientID + " - " + latencyFromClients[clientID] / 1000);
        }

        public float GetLatencyFormClient(int clientId)
        {
            if (latencyFromClients.ContainsKey(clientId))
            {
                // UnityEngine.Debug.Log("Latency from " + clientId + " = " + latencyFromClients[clientId]);
                return latencyFromClients[clientId];
            }

            return -1;
        }
    }
}