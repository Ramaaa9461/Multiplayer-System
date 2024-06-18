using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;

public class NetObjectFactory : MonoBehaviour
{


//    public void CreateObject(int intanceID, Vector3 position, Vector3 rotation, Vector3 scale, Transform parent)
//    {
//        NetObjectMessage netObjectFactory = new(MessagePriority.NonDisposable, 
//            new NetObjPackage(intanceID, ToVec3(position), ToVec3(rotation), ToVec3(scale), parent.GetInstanceID()));
//        NetworkManager.Instance.networkEntity.SendMessage(netObjectFactory.Serialize());
//    }

    Vec3 ToVec3(Vector3 vector3)
    {
        return new Vec3(vector3.x, vector3.y, vector3.z);
    }
}