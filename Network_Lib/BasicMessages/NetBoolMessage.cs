using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetBoolMessage : BaseMessage<bool>
    {
        bool data;

        public NetBoolMessage(MessagePriority messagePriority, bool data) : base(messagePriority)
        {
            currentMessageType = MessageType.Bool;
            this.data = data;
        }

        public NetBoolMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Bool;
            this.data = Deserialize(data);
        }

        public override bool Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToBoolean(message, messageHeaderSize);
            }
            return data;
        }

        public bool GetData()
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
