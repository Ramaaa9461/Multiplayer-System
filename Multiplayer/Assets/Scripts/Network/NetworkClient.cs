using System;
using System.Collections.Generic;
using System.Net;
using Net;
using UnityEngine;

public class NetworkClient : NetworkEntity
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public struct Player
    {
        public int id;
        public string name;

        /// <summary>
        /// Initializes a new instance of the Player struct with the specified ID and name.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="name">The name of the player.</param>
        public Player(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    /// <summary>
    /// The IP address of the network manager.
    /// </summary>
    public IPAddress ipAddress
    {
        get; private set;
    }

    private readonly Dictionary<int, Player> players = new();

    protected GameManager gm;

    ClientPingPong pingPong;
    ClientSortableMessage sortableMessage;
    ClientNondisponsableMessage nondisposablesMessages;

    /// <summary>
    /// Starts the client with the specified IP address, port, and name.
    /// </summary>
    /// <param name="ip">The IP address of the server.</param>
    /// <param name="port">The port of the server.</param>
    /// <param name="name">The name of the client.</param>
    public NetworkClient(IPAddress ip, int port, string name) : base()
    {
        this.port = port;
        this.ipAddress = ip;
        this.userName = name;

        gm = GameManager.Instance;

        connection = new UdpConnection(ip, port, this);

        onInitPingPong += () => nondisposablesMessages = new(this);
        onInitPingPong += () => sortableMessage = new(this);

        ClientToServerNetHandShake handShakeMesage = new(MessagePriority.NonDisposable, (UdpConnection.IPToLong(ip), port, name));
        SendToServer(handShakeMesage.Serialize());
    }

    /// <summary>
    /// Adds a new client to the server.
    /// </summary>
    /// <param name="ip">The IP endpoint of the client.</param>
    /// <param name="newClientID">The ID of the new client.</param>
    /// <param name="clientName">The name of the new client.</param>
    public override void AddClient(IPEndPoint ip, int newClientID, string clientName)
    {
        Console.WriteLine("Adding Client: " + ip.Address);

        OnNewPlayer?.Invoke(newClientID);
    }

    /// <summary>
    /// Removes a client from the server.
    /// </summary>
    /// <param name="idToRemove">The ID of the client to remove.</param>
    public override void RemoveClient(int idToRemove)
    {
        OnRemovePlayer?.Invoke(idToRemove);

        Debug.Log("Removing client: " + idToRemove);
        players.Remove(idToRemove);

        if (clientID == idToRemove)
        {
            gm.RemoveAllPlayers();
            checkActivity = null;
            NetworkScreen.Instance.SwitchToMenuScreen();
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

        OnReceivedMessagePriority(data);

        switch (MessageChecker.CheckMessageType(data))
        {
            case MessageType.Ping:

                pingPong.ReciveServerToClientPingMessage();
                pingPong.CalculateLatencyFromServer();

                break;

            case MessageType.ServerToClientHandShake:

                ServerToClientHandShake netGetClientID = new(data);
                List<(int clientId, string userName)> playerList = netGetClientID.GetData();

                if (checkActivity == null)
                {
                    pingPong = new ClientPingPong(this);
                    checkActivity = pingPong;
                    onInitPingPong?.Invoke();
                }

                for (int i = 0; i < playerList.Count; i++) // First verify which client am I
                {
                    if (playerList[i].userName == userName)
                    {
                        if (NetworkScreen.Instance.isInMenu)
                        {
                            NetworkScreen.Instance.SwitchToChatScreen();
                        }

                        clientID = playerList[i].clientId;
                    }
                }

                players.Clear();

                for (int i = 0; i < playerList.Count; i++)
                {
                    Console.WriteLine(playerList[i].clientId + " - " + playerList[i].userName);
                    Player playerToAdd = new(playerList[i].clientId, playerList[i].userName);
                    players.Add(playerList[i].clientId, playerToAdd);
                    OnNewPlayer?.Invoke(playerToAdd.id);
                }

                break;

            case MessageType.Console:

                UpdateChatText(data, ip);

                break;

            case MessageType.Position:

                NetVector3 netVector3 = new(data);

                if (sortableMessage.CheckMessageOrderRecievedFromServer(netVector3.GetData().id, MessageType.Position, netVector3.MessageOrder))
                {
                    UpdatePlayerPosition(data);
                }

                break;

            case MessageType.BulletInstatiate:

                NetVector3 netBullet = new(data);
                OnInstantiateBullet?.Invoke(netBullet.GetData().id, netBullet.GetData().position);

                break;

            case MessageType.Disconnection:

                NetIDMessage netDisconnection = new(data);
                int playerID = netDisconnection.GetData();

                Console.WriteLine("Remove player " + playerID);
                RemoveClient(playerID);

                break;

            case MessageType.UpdateLobbyTimer:

                NetUpdateTimer netUpdate = new(data);
                gm.OnInitLobbyTimer?.Invoke(netUpdate.GetData());

                break;

            case MessageType.UpdateGameplayTimer:

                gm.OnInitGameplayTimer?.Invoke();

                break;

            case MessageType.Error:

                NetErrorMessage netErrorMessage = new(data);
                NetworkScreen.Instance.SwitchToMenuScreen();
                NetworkScreen.Instance.ShowErrorPanel(netErrorMessage.GetData());
                connection.Close();

                break;

            case MessageType.Winner:

                NetIDMessage netIDMessage = new(data);
                string winText = $"Congratulations! \n {players[netIDMessage.GetData()].name} won the game!";
                NetworkScreen.Instance.SwitchToMenuScreen();
                NetworkScreen.Instance.ShowWinPanel(winText);
                gm.EndMatch();

                break;

            default:
                break;
        }

    }

    void OnReceivedMessagePriority(byte[] data)
    {

        if (MessageChecker.IsSorteableMessage(data))
        {
            sortableMessage?.OnRecievedData(data, -1);
        }
        if (MessageChecker.IsNondisponsableMessage(data))
        {
            nondisposablesMessages?.OnReceivedData(data, -1);
        }
    }

    /// <summary>
    /// Sends data to the server.
    /// </summary>
    /// <param name="data">The data to send.</param>
    public void SendToServer(byte[] data)
    {
        nondisposablesMessages?.AddSentMessages(data);
        connection.Send(data);
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

        ChatScreen.Instance.messages.text += messageText + System.Environment.NewLine;
    }

    /// <summary>
    /// Handles the cleanup when the application is about to quit.
    /// </summary>
    public override void OnApplicationQuit()
    {
        // Notify the server about the client's disconnection
        NetIDMessage netDisconnection = new(MessagePriority.Default, clientID);
        SendToServer(netDisconnection.Serialize());
    }

    /// <summary>
    /// Disconnects the player from the network.
    /// </summary>
    public override void CloseConnection()
    {
        connection.Close();
        NetworkScreen.Instance.SwitchToMenuScreen();
    }

    /// <summary>
    /// Updates the position of a player based on received data.
    /// </summary>
    /// <param name="data">The data containing the player's position.</param>
    protected override void UpdatePlayerPosition(byte[] data)
    {
        NetVector3 netPosition = new(data);
        int clientId = netPosition.GetData().id;

        Vec3 position = netPosition.GetData().position;
        gm.UpdatePlayerPosition((clientId, new UnityEngine.Vector3(position.x, position.y, position.z)));
    }

    public override void Update()
    {
        base.Update();

        if (connection != null)
        {
            nondisposablesMessages?.ResendPackages();
        }
    }

    public override void SendMessage(byte[] data)
    {
        SendToServer(data);
    }

    public override void SendMessage(byte[] data, int id = -1) //Es una sobrecarga que solo usa el SERVER
    {
        SendToServer(data);
    }
}
