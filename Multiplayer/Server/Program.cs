using System;
using System.Collections.Generic;
using System.Net;


namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class NetworkServer : NetworkEntity
    {
        public struct Client
        {
            public float timeStamp; // Time stamp of when the client connected
            public int id; // Unique identifier of the client
            public IPEndPoint ipEndPoint; // IP endpoint of the client
            public string clientName; // Name of the client

            /// <summary>
            /// Initializes a new instance of the Client struct with the specified IP endpoint, ID, time stamp, and client name.
            /// </summary>
            /// <param name="ipEndPoint">The IP endpoint of the client.</param>
            /// <param name="id">The unique identifier of the client.</param>
            /// <param name="timeStamp">The time stamp of when the client connected.</param>
            /// <param name="clientName">The name of the client.</param>
            public Client(IPEndPoint ipEndPoint, int id, float timeStamp, string clientName)
            {
                this.timeStamp = timeStamp;
                this.id = id;
                this.ipEndPoint = ipEndPoint;
                this.clientName = clientName;
            }
        }

        public readonly Dictionary<int, Client> clients = new();

        public readonly Dictionary<IPEndPoint, int> ipToId = new();

        DateTime appStartTime;

        private int maxPlayersPerServer = 4;
        public bool matchOnGoing = false;

        IGameActions gameActions;

        /// <summary>
        /// Starts the server on the specified port.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public NetworkServer(IGameActions gameActions, int port, DateTime appStartTime) : base()
        {
            this.gameActions = gameActions;
            this.port = port;

            connection = new UdpConnection(port, this);
            checkActivity = new PingPong(this);

            onInitPingPong?.Invoke();

            this.appStartTime = appStartTime;
        }

        /// <summary>
        /// Adds a new client to the server.
        /// </summary>
        /// <param name="ip">The IP endpoint of the client.</param>
        /// <param name="newClientID">The ID of the new client.</param>
        /// <param name="clientName">The name of the new client.</param>
        public override void AddClient(IPEndPoint ip, int newClientID, string clientName)
        {
            if (!ipToId.ContainsKey(ip) && !clients.ContainsKey(newClientID))
            {
                Console.WriteLine("Adding Client: " + ip.Address);

                ipToId[ip] = newClientID;
                clients.Add(newClientID, new Client(ip, newClientID, (float)(DateTime.UtcNow - appStartTime).TotalSeconds, clientName));
                checkActivity.AddClientForList(newClientID);
                OnNewPlayer?.Invoke(newClientID);

                List<(int, string)> playersInServer = new();

                foreach (int id in clients.Keys)
                {
                    playersInServer.Add((clients[id].id, clients[id].clientName));
                }

                ServerToClientHandShake serverToClient = new(MessagePriority.NonDisposable, playersInServer);
                Broadcast(serverToClient.Serialize());
            }
            else
            {
                Console.WriteLine("It's a repeated Client");
            }
        }

        /// <summary>
        /// Removes a client from the server.
        /// </summary>
        /// <param name="idToRemove">The ID of the client to remove.</param>
        public override void RemoveClient(int idToRemove)
        {
            OnRemovePlayer?.Invoke(idToRemove);

            if (clients.ContainsKey(idToRemove))
            {
                Console.WriteLine("Removing client: " + idToRemove);
                checkActivity.RemoveClientForList(idToRemove);
                ipToId.Remove(clients[idToRemove].ipEndPoint);
                clients.Remove(idToRemove);
            }
        }

        /// <summary>
        /// Handles incoming data received over the network.
        /// </summary>
        /// <param name="data">The data received.</param>
        /// <param name="ip">The IP address of the sender.</param>
        public override void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            // Invoke the event to notify listeners about the received message
            OnReceivedMessage?.Invoke(data, ip);

            switch (MessageChecker.CheckMessageType(data))
            {
                case MessageType.Ping:

                    if (ipToId.ContainsKey(ip))
                    {
                        checkActivity.ReciveClientToServerPingMessage(ipToId[ip]);
                        checkActivity.CalculateLatencyFromClients(ipToId[ip]);
                    }

                    break;

                case MessageType.ClientToServerHandShake:

                    ReceiveClientToServerHandShake(data, ip);

                    break;

                case MessageType.Console:

                    UpdateChatText(data, ip);

                    break;

                case MessageType.Position:

                    NetVector3 netVector3 = new(data);

                    if (ipToId.ContainsKey(ip))
                    {
                        if (sortableMessages.CheckMessageOrderRecievedFromClients(ipToId[ip], MessageChecker.CheckMessageType(data), netVector3.MessageOrder))
                        {
                            UpdatePlayerPosition(data);
                        }
                    }

                    break;

                case MessageType.BulletInstatiate:

                    NetVector3 netBullet = new(data);
                    OnInstantiateBullet?.Invoke(netBullet.GetData().id, netBullet.GetData().position);

                    BroadcastPlayerPosition(netBullet.GetData().id, data);

                    break;

                case MessageType.Disconnection:

                    NetIDMessage netDisconnection = new(data);
                    int playerID = netDisconnection.GetData();

                    Broadcast(data);
                    RemoveClient(playerID);

                    break;

                case MessageType.Error:

                    NetErrorMessage netErrorMessage = new(data);
                    gameActions.SwitchToMenuScreen();
                    gameActions.ShowErrorPanel(netErrorMessage.GetData());
                    connection.Close();

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Broadcasts data to a specific endpoint.
        /// </summary>
        /// <param name="data">The data to broadcast.</param>
        /// <param name="ip">The IP endpoint to broadcast to.</param>
        public void Broadcast(byte[] data, IPEndPoint ip)
        {
            if (ipToId.ContainsKey(ip))
            {
                nonDisposablesMessages?.AddSentMessagesFromServer(data, ipToId[ip]);
            }
            connection.Send(data, ip);
        }

        /// <summary>
        /// Broadcasts data to all connected clients.
        /// </summary>
        /// <param name="data">The data to broadcast.</param>
        public void Broadcast(byte[] data)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    nonDisposablesMessages?.AddSentMessagesFromServer(data, iterator.Current.Value.id);
                    connection.Send(data, iterator.Current.Value.ipEndPoint);
                }
            }
        }

        /// <summary>
        /// Receives a handshake message from a client to the server.
        /// </summary>
        /// <param name="data">The data received.</param>
        /// <param name="ip">The IP endpoint of the client.</param>
        void ReceiveClientToServerHandShake(byte[] data, IPEndPoint ip)
        {
            ClientToServerNetHandShake handShake = new(data);

            if (!MatchOnGoing(ip) && CheckValidUserName(handShake.GetData().Item3, ip) && !ServerIsFull(ip))
            {
                AddClient(ip, clientID, handShake.GetData().Item3);
                clientID++;
            }
        }

        /// <summary>
        /// Checks if a match is ongoing.
        /// </summary>
        /// <param name="ip">The IP endpoint of the client.</param>
        /// <returns>True if a match is ongoing, false otherwise.</returns>
        bool MatchOnGoing(IPEndPoint ip)
        {
            if (matchOnGoing)
            {
                NetErrorMessage netServerIsFull = new("Match has already started");
                Broadcast(netServerIsFull.Serialize(), ip);
            }

            return matchOnGoing;
        }

        /// <summary>
        /// Checks if the server is full.
        /// </summary>
        /// <param name="ip">The IP endpoint of the client.</param>
        /// <returns>True if the server is full, false otherwise.</returns>
        bool ServerIsFull(IPEndPoint ip)
        {
            // Check if the number of connected clients exceeds the maximum allowed
            bool serverIsFull = clients.Count >= maxPlayersPerServer;

            if (serverIsFull)
            {
                NetErrorMessage netServerIsFull = new("Server is full");
                Broadcast(netServerIsFull.Serialize(), ip);
            }

            return serverIsFull;
        }

        /// <summary>
        /// Checks if a given username is valid.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <param name="ip">The IP endpoint of the client.</param>
        /// <returns>True if the username is valid, false otherwise.</returns>
        bool CheckValidUserName(string userName, IPEndPoint ip)
        {
            foreach (int clientID in clients.Keys)
            {
                if (userName == clients[clientID].clientName)
                {
                    NetErrorMessage netInvalidUserName = new("Invalid User Name");
                    Broadcast(netInvalidUserName.Serialize(), ip);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates the chat text with received message data.
        /// </summary>
        /// <param name="data">The message data received.</param>
        /// <param name="ip">The IP endpoint of the client.</param>
        protected override void UpdateChatText(byte[] data, IPEndPoint ip)
        {
            string messageText = "";

            NetMessage netMessage = new(data);
            messageText += new string(netMessage.GetData());

            Broadcast(data);

            gameActions.WriteChat(messageText + System.Environment.NewLine);
        }

        /// <summary>
        /// Handles the cleanup when the application is about to quit.
        /// </summary>
        public override void OnApplicationQuit()
        {
            // Notify all clients about the server's disconnection and close the server
            NetErrorMessage netErrorMessage = new("Lost Connection To Server");
            Broadcast(netErrorMessage.Serialize());
            CloseServer();
        }

        /// <summary>
        /// Closes the server and removes all connected clients.
        /// </summary>
        public void CloseServer()
        {
            // Notify all clients about their disconnection and remove them
            List<int> clientIdsToRemove = new(clients.Keys);

            foreach (int clientId in clientIdsToRemove)
            {
                NetIDMessage netDisconnection = new(MessagePriority.Default, clientId);
                Broadcast(netDisconnection.Serialize());
                RemoveClient(clientId);
            }

            gameActions.SwitchToMenuScreen();
        }

        /// <summary>
        /// Updates the position of a player based on received data.
        /// </summary>
        /// <param name="data">The data containing the player's position.</param>
        protected override void UpdatePlayerPosition(byte[] data)
        {
            NetVector3 netPosition = new(data);
            int clientId = netPosition.GetData().id;

            gameActions.UpdatePlayerPosition(netPosition.GetData());

            // Broadcast the player's position to all clients except the sender
            BroadcastPlayerPosition(clientId, data);
        }

        /// <summary>
        /// Broadcasts a player's position to all clients except the sender.
        /// </summary>
        /// <param name="senderClientId">The ID of the player sending the position.</param>
        /// <param name="data">The data containing the player's position.</param>
        private void BroadcastPlayerPosition(int senderClientId, byte[] data)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    int receiverClientId = iterator.Current.Key;

                    // Avoids sending the position back to the sender
                    if (receiverClientId != senderClientId)
                    {
                        // Check if the sender and receiver have the same IP endpoint
                        if (clients[receiverClientId].ipEndPoint.Equals(clients[senderClientId].ipEndPoint)) continue;
                        Broadcast(data, clients[receiverClientId].ipEndPoint);
                    }
                }
            }
        }
    }
}
