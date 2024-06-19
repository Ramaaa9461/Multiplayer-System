using Net;
using UnityEngine;

public class PlayerController : MonoBehaviour, INetObj
{
    [SerializeField, NetVariable(1)] TowerTurns towerTurns;
    [SerializeField, NetVariable(2)] TankMovement movement;

    [SerializeField] Transform cameraPivot;

    [NetVariable(0)] public int health = 3;

    public bool currentPlayer = false;
    public int clientID = -1;

    NetObj netObj;

    //Esta clase seria el punto de entrada para reflection de los players
    //Deberia contener todos los scripts que envien informacion por ej TowerTurns o movement si queremos enviar sus datos

    GameManager gm;
    NetworkManager nm;

    static int positionMessageOrder = 1;
    static int bulletsMessageOrder = 1;

    private void Start()
    {
        gm = GameManager.Instance;
        nm = NetworkManager.Instance;

        if (currentPlayer)
        {
            Camera.main.gameObject.GetComponent<CameraOrbit>().SetFollowObject(cameraPivot);
        }
    }

    public void OnReciveDamage() //Solo lo maneja el server esta funcion
    {
        health--;

        if (health <= 0)
        {
            //TODO: El server tiene que hecharlo de la partida
            NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
            nm.networkEntity.SendMessage(netDisconnection.Serialize());
            nm.networkEntity.RemoveClient(clientID);
        }
    }

    public int GetID()
    {
        return netObj.ID;
    }

    public int GetOwnerID()
    {
        return netObj.OwnerId;
    }

    public NetObj GetNetObj()
    {
        return netObj;
    }
}

