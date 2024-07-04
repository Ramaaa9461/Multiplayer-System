using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetSByteMessage), MessageType.Sbyte)]
    class NetSByteMessage : BaseReflectionMessage<sbyte>
    {
        sbyte data;

        public NetSByteMessage(MessagePriority messagePriority, sbyte data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Sbyte;
            this.data = data;
        }

        public NetSByteMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Sbyte;
            this.data = Deserialize(data);
        }

        public override sbyte Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = (sbyte)message[messageHeaderSize];
            }
            return data;
        }

        public sbyte GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.Add((byte)data);

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
