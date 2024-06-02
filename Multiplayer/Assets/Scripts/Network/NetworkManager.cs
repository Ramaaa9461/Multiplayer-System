using System;
using System.Net;

public interface IGameActions
{
    void SwitchToMenuScreen();
    void WriteChat(string text);
    void ShowErrorPanel(string errorText);
    void UpdatePlayerPosition((int index, Vec3 newPosition) data);
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager> , IGameActions 
{

    public NetworkEntity networkEntity;

    public Action onInitEntity;


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
        networkEntity = new NetworkServer(this, port, appStartTime);
        onInitEntity?.Invoke();

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



    public void SwitchToMenuScreen()
    {
        NetworkScreen.Instance.SwitchToMenuScreen();
    }

    public void WriteChat(string text)
    {
        ChatScreen.Instance.messages.text += text;
    }

    public void ShowErrorPanel(string errorText)
    {
        NetworkScreen.Instance.ShowErrorPanel(errorText);
    }

    public void UpdatePlayerPosition((int index, Vec3 newPosition) data )
    {
        GameManager.Instance.UpdatePlayerPosition(data);
    }
}