using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetVector3 : BaseMessage<(int, Vec3)>
    {
        private (int id, Vec3 position) data;

        public NetVector3(MessagePriority messagePriority, (int, Vec3) data) : base(messagePriority)
        {
            currentMessageType = MessageType.Position;
            this.data = data;
        }

        public NetVector3(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Position;
            this.data = Deserialize(data);
        }

        public (int id, Vec3 position) GetData()
        {
            return data;
        }

        public override (int, Vec3) Deserialize(byte[] message)
        {
            (int id, Vec3 position) outData = (-1, Vec3.Zero);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                DeserializeHeader(message);

                outData.id = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                outData.position.x = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);
                outData.position.y = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);
                outData.position.z = BitConverter.ToSingle(message, messageHeaderSize);
            }

            return outData;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.id));

            outData.AddRange(BitConverter.GetBytes(data.position.x));
            outData.AddRange(BitConverter.GetBytes(data.position.y));
            outData.AddRange(BitConverter.GetBytes(data.position.z));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}