using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetFloatMessage), MessageType.Byte)]
    class NetByteMessage : BaseReflectionMessage<byte>
    {
        byte data;

        public NetByteMessage(MessagePriority messagePriority, byte data, List<int> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Byte;
            this.data = data;
        }

        public NetByteMessage(byte[] data) : base(MessagePriority.Default, null)
        {
            currentMessageType = MessageType.Byte;
            this.data = Deserialize(data);
        }

        public override byte Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = message[messageHeaderSize];
            }
            return data;
        }

        public byte GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.Add(data);

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
