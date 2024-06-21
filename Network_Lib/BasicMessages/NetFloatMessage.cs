using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetFloatMessage), MessageType.Float)]
    class NetFloatMessage : BaseMessage<float>
    {
        float data;
        List<int> messageRoute = new List<int>();

        public NetFloatMessage(MessagePriority messagePriority, float data, List<int> messageRoute) : base(messagePriority)
        {
            currentMessageType = MessageType.Float;
            this.messageRoute = messageRoute;
            this.data = data;
        }

        public NetFloatMessage(byte[] data) : base(MessagePriority.Default)
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

        public List<int> GetMessageRoute()
        {
            return messageRoute;
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
