using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetCharMessage : BaseMessage<char>
    {
        char data;

        public NetCharMessage(MessagePriority messagePriority, char data) : base(messagePriority)
        {
            currentMessageType = MessageType.Char;
            this.data = data;
        }

        public NetCharMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Char;
            this.data = Deserialize(data);
        }

        public override char Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToChar(message, messageHeaderSize);
            }
            return data;
        }

        public char GetData()
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
