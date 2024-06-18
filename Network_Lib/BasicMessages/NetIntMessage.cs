using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetIntMessage : BaseMessage<int>
    {
        int data;

        public NetIntMessage(MessagePriority messagePriority, int data) : base(messagePriority)
        {
            currentMessageType = MessageType.Int;
            this.data = data;
        }

        public NetIntMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Int;
            this.data = Deserialize(data);
        }

        public override int Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToInt32(message, messageHeaderSize);
            }
            return data;
        }

        public int GetData()
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
