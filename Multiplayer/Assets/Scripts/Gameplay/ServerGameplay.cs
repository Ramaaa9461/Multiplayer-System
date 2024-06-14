//using Net;
//using System.Net;
//using TMPro;
//using UnityEngine;

//enum States { Init, Lobby, Game, Finish };

//public class ServerGameplay : MonoBehaviour
//{
//    int minutesInLobby = 15; // 2 minutos
//    float minutesGameplay = 30; //3 minutos
//    float timeUntilCloseServer = 5;

//    int minPlayerToInitCounter = 2;

//    GameManager gm;
//    NetworkManager nm;
//    States currentState;

//    bool counterInit = false;

//    bool initLobby = true;
//    bool initGameplay = true;

//    float counter = 0;

//    bool clientLobbyTimer = false;
//    bool clientGameplayTimer = false;

//    int serverClientsCount = 0;

//    NetworkEntity server;

//    private void Start()
//    {
//        gm = GameManager.Instance;
//        nm = NetworkManager.Instance;

//        nm.onInitEntity += ()=> nm.networkEntity.OnReceivedMessage += OnRecievedData;

//        nm.onInitEntity += () => nm.networkEntity.OnNewPlayer += () => serverClientsCount++;


//     gm.OnInitLobbyTimer += SetLobbyTimer;
//        gm.OnInitGameplayTimer += SetGameplayTimer;

//        gm.OnChangeLobbyPlayers += CheckForAddNewPlayer;
//    }

//    void CheckForAddNewPlayer(int clientID)
//    {
//        if (nm.isServer && currentState == States.Lobby && counterInit)
//        {
//            Debug.Log("Mando el tiempo a nuevos clientes en el lobby");
//            NetUpdateNewPlayersTimer timer = new NetUpdateNewPlayersTimer(MessagePriority.NonDisposable, counter);
//            server.Broadcast(timer.Serialize(), server.clients[clientID].ipEndPoint);
//        }
//    }

//    void OnRecievedData(byte[] data, IPEndPoint ip)
//    {
//        if (MessageChecker.CheckMessageType(data) == MessageType.UpdateLobbyTimerForNewPlayers)
//        {
//            Debug.Log("Llego mensaje de update timer");
//            NetUpdateNewPlayersTimer timer = new NetUpdateNewPlayersTimer(data);

//            counter = timer.GetData();
//            clientLobbyTimer = true;
//        }
//    }

//    private void Update()
//    {
//        if (nm.isServer)
//        {
//            UpdateServer();
//        }
//        else
//        {
//            UpdateClient();
//        }
//    }

//    void UpdateServer()
//    {
//        if (nm != null && nm.isServer)
//        {
//                server = nm.GetNetworkServer();

//            switch (currentState)
//            {
//                case States.Init:

//                    currentState = States.Lobby;

//                    break;
//                case States.Lobby:

//                    if (server.pla.Count >= minPlayerToInitCounter)
//                    {
//                        counterInit = true;

//                        if (initLobby)
//                        {
//                            NetUpdateTimer netUpdateLobbyTimer = new NetUpdateTimer(MessagePriority.NonDisposable, true);
//                            netUpdateLobbyTimer.CurrentMessageType = MessageType.UpdateLobbyTimer;
//                            server.Broadcast(netUpdateLobbyTimer.Serialize());
//                            initLobby = false;
//                        }

//                        counter += Time.deltaTime;
//                        gm.timer.text = counter.ToString("F2") + "s";

//                        if (counter >= minutesInLobby)
//                        {
//                            counter = 0;
//                            gm.timer.text = "";
//                            server.matchOnGoing = true;
//                            currentState = States.Game;
//                        }
//                    }
//                    else
//                    {
//                        if (counterInit)
//                        {
//                            NetUpdateTimer netUpdateLobbyTimer = new NetUpdateTimer(MessagePriority.NonDisposable, false);
//                            netUpdateLobbyTimer.CurrentMessageType = MessageType.UpdateLobbyTimer;
//                            server.Broadcast(netUpdateLobbyTimer.Serialize());

//                            counterInit = false;
//                            initLobby = true;

//                            counter = 0;
//                            gm.timer.text = "";

//                            currentState = States.Init;

//                        }
//                    }

//                    break;
//                case States.Game:

//                    if (initGameplay)
//                    {
//                        NetUpdateTimer netUpdateGameplayTimer = new NetUpdateTimer(MessagePriority.NonDisposable, true);
//                        netUpdateGameplayTimer.CurrentMessageType = MessageType.UpdateGameplayTimer;
//                        server.Broadcast(netUpdateGameplayTimer.Serialize());

//                        initGameplay = false;
//                    }

//                    counter += Time.deltaTime;
//                    gm.timer.text = counter.ToString("F2") + "s";

//                    if (counter >= minutesGameplay || gm.playerList.Count <= 1)
//                    {
//                        SendMatchWinner();

//                        counter = 0;
//                        gm.timer.text = "";
//                        currentState = States.Finish;
//                    }
//                    break;

//                case States.Finish:

//                    timeUntilCloseServer -= Time.deltaTime;

//                    if (timeUntilCloseServer <= 0)
//                    {
//                        server.CloseConnection();
//                    }

//                    break;


//                default:
//                    break;
//            }
//        }
//    }

//    void UpdateClient()
//    {
//        if (clientLobbyTimer)
//        {
//            counter += Time.deltaTime;
//            gm.timer.text = counter.ToString("F2") + "s";
//        }

//        if (clientGameplayTimer && !NetworkScreen.Instance.gameObject.activeInHierarchy)
//        {
//            clientLobbyTimer = false;

//            counter += Time.deltaTime;
//            gm.timer.text = counter.ToString("F2") + "s";
//        }
//    }

//    void SetGameplayTimer()
//    {
//        clientGameplayTimer = true;
//        counter = 0;
//    }

//    void SetLobbyTimer(bool init)
//    {
//        counter = 0;
//        gm.timer.text = "";
//        clientLobbyTimer = init;
//    }

//    void SendMatchWinner()
//    {
//        PlayerController playerWithMaxHealth = null;
//        int maxHealth = int.MinValue;

//        foreach (int index in gm.playerList.Keys)
//        {
//            if (gm.playerList[index].TryGetComponent(out PlayerController pc))
//            {
//                if (pc.health > maxHealth)
//                {
//                    maxHealth = pc.health;
//                    playerWithMaxHealth = pc;
//                }
//            }
//        }

//        NetIDMessage netIDMessage = new NetIDMessage(MessagePriority.Default, playerWithMaxHealth.clientID);
//        netIDMessage.CurrentMessageType = MessageType.Winner;
//        nm.GetNetworkServer().Broadcast(netIDMessage.Serialize());
//    }
//}
