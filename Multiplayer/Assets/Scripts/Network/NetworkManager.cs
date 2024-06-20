using Net;
using System;
using System.Net;
using UnityEngine;


public class NetworkManager : MonoBehaviourSingleton<NetworkManager>  
{

    public NetworkEntity networkEntity;

    public Action onInitEntity;
    public Action<int, GameObject> onInstanceCreated;

    public int ClientID
    {
        get { return networkEntity.clientID; }
    }

    public bool isServer
    {
        get { return !(networkEntity is NetworkClient); }
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

    public void StartServer(int port)
    {
   //     networkEntity = new NetworkServer(this, port, appStartTime);
   //     onInitEntity?.Invoke();

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
            networkEntity.Update();
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

    public void UpdatePlayerPosition((int index, Vector3 newPosition) data )
    {
        GameManager.Instance.UpdatePlayerPosition(data);
    }
}