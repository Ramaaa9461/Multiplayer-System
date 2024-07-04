﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    [NetMessageClass(typeof(NetDoubleMessage), MessageType.Double)]
    class NetDoubleMessage : BaseReflectionMessage<double>
    {
        double data;

        public NetDoubleMessage(MessagePriority messagePriority, double data, List<int> messageRoute) : base(messagePriority, messageRoute)
        {
            currentMessageType = MessageType.Double;
            this.data = data;
        }

        public NetDoubleMessage(byte[] data) : base(MessagePriority.Default, new List<int>())
        {
            currentMessageType = MessageType.Double;
            this.data = Deserialize(data);
        }

        public override double Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                data = BitConverter.ToDouble(message, messageHeaderSize);
            }
            return data;
        }

        public double GetData()
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
