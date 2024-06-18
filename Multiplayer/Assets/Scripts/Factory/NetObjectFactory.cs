using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;

public interface IService
{
    int GetID();
}

public static class NetObjectFactory //service provider pattern c#
{

    public static void NetInstance(GameObject gameObjectToIntanciate, Vector3 position, Quaternion rotation, Vector3 scale, GameObject parentGameObject)
    {
        int prefabID = -1; //TODO: Ver de donde sacarlo
        int parentId = -1; //TODO: Ver de donde sacarlo
        
        if (gameObjectToIntanciate.TryGetComponent(out IService gameObjectService))
        {
            prefabID = gameObjectService.GetID();
        }
        else
        {
            return;
        }
        
        if (parentGameObject != null && parentGameObject.TryGetComponent(out IService parentService))
        {
            parentId = parentService.GetID();
        }


        InstanceRequestPayload instanceRequestPayload = new(prefabID, ToVec3(position), ToVec3(rotation.eulerAngles), ToVec3(scale), parentId);

        InstanceRequestMenssage instanceRequest = new(MessagePriority.NonDisposable, instanceRequestPayload);
        NetworkManager.Instance.networkEntity.SendMessage(instanceRequest.Serialize());
    }

    static Vec3 ToVec3(Vector3 vector3)
    {
        return new Vec3(vector3.x, vector3.y, vector3.z);
    }
}