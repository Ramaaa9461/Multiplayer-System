using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    [Serializable]
    public class NetMessage : BaseMessage<char[]>
    {
        char[] data;

        public NetMessage(MessagePriority priority, char[] data) : base(priority)
        {
            currentMessageType = MessageType.Console;
            this.data = data;
        }

        public NetMessage(byte[] data) : base(MessagePriority.Default) //Se actualiza en el Deserialize esto
        {
            currentMessageType = MessageType.Console;
            this.data = Deserialize(data);
        }

        public char[] GetData()
        {
            return data;
        }

        public override char[] Deserialize(byte[] message)
        {
            string text = "";

            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                text = MessageChecker.DeserializeString(message, messageHeaderSize);
            }

            return text.ToCharArray();
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);


            outData.AddRange(MessageChecker.SerializeString(data));


            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}