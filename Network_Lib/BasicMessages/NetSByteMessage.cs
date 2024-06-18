using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetSByteMessage : BaseMessage<sbyte>
    {
        sbyte data;

        public NetSByteMessage(MessagePriority messagePriority, sbyte data) : base(messagePriority)
        {
            currentMessageType = MessageType.Sbyte;
            this.data = data;
        }

        public NetSByteMessage(byte[] data) : base(MessagePriority.Default)
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
