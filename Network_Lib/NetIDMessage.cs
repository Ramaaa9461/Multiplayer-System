using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetIDMessage : BaseMessage<int>
    {
        int clientID;

        public NetIDMessage(MessagePriority messagePriority, int clientID) : base(messagePriority)
        {
            currentMessageType = MessageType.Disconnection;
            this.clientID = clientID;
        }

        public NetIDMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Disconnection;
            this.clientID = Deserialize(data);
        }

        public override int Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                clientID = BitConverter.ToInt32(message, messageHeaderSize);
            }
            return clientID;
        }

        public int GetData()
        {
            return clientID;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(clientID));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}