using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    class NetLongMessage : BaseMessage<long>
    {
        long data;

        public NetLongMessage(MessagePriority messagePriority, long data) : base(messagePriority)
        {
            currentMessageType = MessageType.Long;
            this.data = data;
        }

        public NetLongMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Long;
            this.data = Deserialize(data);
        }

        public override long Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToInt64(message, messageHeaderSize);
            }
            return data;
        }

        public long GetData()
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
