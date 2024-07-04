using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetUIntMessage), MessageType.Uint)]
    class NetUIntMessage : BaseReflectionMessage<uint>
    {
        uint data;

        public NetUIntMessage(MessagePriority messagePriority, uint data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Uint;
            this.data = data;
        }

        public NetUIntMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.Uint;
            this.data = Deserialize(data);
        }

        public override uint Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToUInt32(message, messageHeaderSize);
            }
            return data;
        }

        public uint GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
