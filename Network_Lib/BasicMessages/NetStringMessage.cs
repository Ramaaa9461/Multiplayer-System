using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetStringMessage), MessageType.String)]
    class NetStringMessage : BaseReflectionMessage<string>
    {
        string data;

        public NetStringMessage(MessagePriority messagePriority, string data, List<RouteInfo> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.String;
            this.data = data;
        }

        public NetStringMessage(byte[] data) : base(MessagePriority.Default, new List<RouteInfo>())
        {
            currentMessageType = MessageType.String;
            this.data = Deserialize(data);
        }

        public override string Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                MessageChecker.DeserializeString(message, ref messageHeaderSize);
            }
            return data;
        }

        public string GetData()
        {
            return data;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(MessageChecker.SerializeString(data.ToCharArray()));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}
