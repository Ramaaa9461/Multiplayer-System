using System;
using System.Collections.Generic;

namespace Net
{
    [System.Serializable]
    public struct RouteInfo
    {
      [NetVariable(0)] public int route;
      [NetVariable(1)] public int collectionIndex;

        public RouteInfo(int route, int collectionIndex = -1)
        {
            this.route = route;
            this.collectionIndex = collectionIndex;
        }

        public void SetRoute(int route)
        {
           this.route= route;
        }

        public void SetCollectionIndex(int collectionIndex)
        {
            this.collectionIndex = collectionIndex;
        }

        public int GetRoute()
        {
            return route;
        }

        public int GetCollectionIndex()
        {
            return collectionIndex;
        }

        public override string ToString()
        {
            return $"Route: {route} - CollectionIndex: {collectionIndex}";
        }
    }


    public abstract class BaseReflectionMessage<T> : BaseMessage<T>
    {
        protected List<RouteInfo> messageRoute = new List<RouteInfo>();

        public BaseReflectionMessage(MessagePriority messagePriority, List<RouteInfo> messageRoute) : base(messagePriority)
        {
            this.messageRoute = messageRoute;
        }

        public List<RouteInfo> GetMessageRoute()
        {
            return messageRoute;
        }

        public override void SerializeHeader(ref List<byte> outData)
        {
            base.SerializeHeader(ref outData);
            SerializeMessageRoute(ref outData);
        }

        public override void DeserializeHeader(byte[] message)
        {
            base.DeserializeHeader(message);
            DeserializeMessageRoute(message);
        }

        protected void SerializeMessageRoute(ref List<byte> outData)
        {
            outData.AddRange(BitConverter.GetBytes(messageRoute.Count));

            foreach (RouteInfo info in messageRoute)
            {
                outData.AddRange(BitConverter.GetBytes(info.route));
                outData.AddRange(BitConverter.GetBytes(info.collectionIndex));
            }
        }

        protected void DeserializeMessageRoute(byte[] message)
        {
            int messageRouteLength = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            for (int i = 0; i < messageRouteLength; i++)
            {
                int route = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);
                int collectionIndex = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                messageRoute.Add(new RouteInfo(route, collectionIndex));
            }
        }
    }
}
