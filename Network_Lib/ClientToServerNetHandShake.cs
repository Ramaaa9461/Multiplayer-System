using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class ClientToServerNetHandShake : BaseMessage<(long, int, string)>
    {
        (long ip, int port, string name) data;

        public ClientToServerNetHandShake(MessagePriority messagePriority, (long, int, string) data) : base(messagePriority)
        {
            currentMessageType = MessageType.ClientToServerHandShake;
            this.data = data;
        }

        public ClientToServerNetHandShake(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.ClientToServerHandShake;
            this.data = Deserialize(data);
        }

        public override (long, int, string) Deserialize(byte[] message)
        {
            (long, int, string) outData = (0, 0, "");

            if (MessageChecker.DeserializeCheckSum(message))
            {
                DeserializeHeader(message);

                outData.Item1 = BitConverter.ToInt64(message, messageHeaderSize);
                messageHeaderSize += sizeof(long);
                outData.Item2 = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                outData.Item3 = MessageChecker.DeserializeString(message, ref messageHeaderSize);
            }

            return outData;
        }

        public (long, int, string) GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.Item1));
            outData.AddRange(BitConverter.GetBytes(data.Item2));

            outData.AddRange(MessageChecker.SerializeString(data.name.ToCharArray()));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}