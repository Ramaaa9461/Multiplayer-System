using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    public static class NetObjFactory
    {
        static NetworkEntity entity; //Se usa para mandar los mensajes 
        static Dictionary<int, INetObj> NetObjectsInstances = new Dictionary<int, INetObj>();

        public static void SetNetworkEntity(NetworkEntity networkEntity)
        {
            entity = networkEntity;
        }

        public static void AddINetObject(int key, INetObj netObj)
        {
            NetObjectsInstances[key] = netObj;
        }

        public static INetObj GetINetObject(int key)
        {
            if (NetObjectsInstances.ContainsKey(key))
            {
                return NetObjectsInstances[key];
            }

            return null;
        }

        public static void NetInstance(int gameObjectToIntanciateID, float positionX, float positionY, float positionZ,
                                                                      float rotationX, float rotationY, float rotationZ, float rotationW,
                                                                      float scaleX, float scaleY, float scaleZ,
                                                                      int parentGameObjectID)
        {
            InstanceRequestPayload instanceRequestPayload = new InstanceRequestPayload(gameObjectToIntanciateID,
                                                                       positionX, positionY, positionZ,
                                                                       rotationX, rotationY, rotationZ, rotationW,
                                                                       scaleX, scaleY, scaleZ,
                                                                       parentGameObjectID);

            InstanceRequestMenssage instanceRequest = new InstanceRequestMenssage(MessagePriority.NonDisposable, instanceRequestPayload);
            entity.SendMessage(instanceRequest.Serialize());
        }
    }
}
