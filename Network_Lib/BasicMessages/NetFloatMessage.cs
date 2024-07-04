using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetFloatMessage), MessageType.Float)]
    class NetFloatMessage : BaseReflectionMessage<float>
    {
        float data;

        public NetFloatMessage(MessagePriority messagePriority, float data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Float;
            this.data = data;
        }

        public NetFloatMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Float;
            this.data = Deserialize(data);
        }

        public override float Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToSingle(message, messageHeaderSize);
            }
            return data;
        }

        public float GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            SerializeMessageRoute(ref outData);

            outData.AddRange(BitConverter.GetBytes(data));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
