using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{


    [NetMessageClass(typeof(NetFloatMessage), MessageType.Float)]
    class NetFloatMessage : BaseReflectionMessage<float>
    {
        float data;

        public NetFloatMessage(MessagePriority messagePriority, float data, List<int> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Float;
            this.data = data;
        }

        public NetFloatMessage(byte[] data) : base(MessagePriority.Default, null)
        {
            currentMessageType = MessageType.Float;
            this.data = Deserialize(data);
        }

        public override float Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                int messageRouteLength = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                for (int i = 0; i < messageRouteLength; i++)
                {
                    messageRoute.Add(BitConverter.ToInt32(message, messageHeaderSize));
                    messageHeaderSize += sizeof(int);
                }

                data = BitConverter.ToSingle(message, messageHeaderSize);
            }
            return data;
        }

        public float GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(messageRoute.Count));

            foreach (int id in messageRoute)
            {
                outData.AddRange(BitConverter.GetBytes(id));
            }


            outData.AddRange(BitConverter.GetBytes(data));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
