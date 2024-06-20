using Net;
using UnityEngine;

public class PlayerController : MonoBehaviour, INetObj
{
    [SerializeField, NetVariable(1)] TowerTurns towerTurns;   //Esta clase seria el punto de entrada para reflection de los players
    [SerializeField, NetVariable(2)] TankMovement movement; //Deberia contener todos los scripts que envien informacion por ej TowerTurns o movement si queremos enviar sus datos

    [SerializeField] Transform cameraPivot;

    [NetVariable(0)] public int health = 3;

    public bool currentPlayer = false;
    public int clientID = -1;

    NetObj netObj = new(-1,-1);

    NetworkManager nm;

    static int positionMessageOrder = 1;
    static int bulletsMessageOrder = 1;

    private void Start()
    {
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

