using System;
using System.Collections.Generic;

namespace Net
{
    public class NetAssignServerMessage : BaseMessage<int>
    {
        int port;

        public NetAssignServerMessage(MessagePriority messagePriority, int port) : base(messagePriority)
        {
            currentMessageType = MessageType.AssignServer;
            this.port = port;
        }

        public NetAssignServerMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.AssignServer;
            this.port = Deserialize(data);
        }

        public override int Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                port = BitConverter.ToInt32(message, messageHeaderSize);
            }
            return port;
        }

        public int GetData()
        {
            return port;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(port));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
