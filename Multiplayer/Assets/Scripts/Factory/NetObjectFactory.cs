//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Net;



//public static class NetObjectFactory
//{
//    static Dictionary<int, INetObj> NetObjectsInstances = new();

//    public static void AddINetObject(int key, INetObj netObj)
//    {
//        NetObjectsInstances[key] = (netObj);
//    }

//    public static INetObj GetINetObject(int key)
//    {
//        if (NetObjectsInstances.ContainsKey(key))
//        {
//            return NetObjectsInstances[key];
//        }
//        return null;
//    }

//    public static void NetInstance(GameObject gameObjectToIntanciate, Vector3 position, Quaternion rotation, Vector3 scale, GameObject parentGameObject)
//    {
//        IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();
//        int prefabID = prefabService.GetIdByPrefab(gameObjectToIntanciate);

//        int parentId = -1;

//        if (parentGameObject != null && parentGameObject.TryGetComponent(out INetObj netObj))
//        {
//            parentId = netObj.GetID();
//        }

//        InstanceRequestPayload instanceRequestPayload = new(prefabID, positionX, positionY, positionZ,
//                                                                      rotationX, rotationY, rotationZ, rotationW,
//                                                                      scaleX,    scaleY,    scaleZ, parentId);

//        InstanceRequestMenssage instanceRequest = new(MessagePriority.NonDisposable, instanceRequestPayload);
//        NetworkManager.Instance.networkEntity.SendMessage(instanceRequest.Serialize());
//    }

//    static Vec3 ToVec3(Vector3 vector3)
//    {
//        return new Vec3(vector3.x, vector3.y, vector3.z);
//    }
//}