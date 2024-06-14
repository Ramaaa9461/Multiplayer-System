using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetErrorMessage : BaseMessage<string>
    {
        string error;

        public NetErrorMessage(string error) : base(MessagePriority.Default) //Simepre van a ser default, lo dejo implicito aca desde el principio
        {
            currentMessageType = MessageType.Error;
            this.error = error;
        }

        public NetErrorMessage(byte[] message) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Error;
            error = Deserialize(message);
        }

        public override string Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                error = MessageChecker.DeserializeString(message, messageHeaderSize);
            }

            return error;
        }

        public string GetData()
        {
            return error;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(MessageChecker.SerializeString(error.ToCharArray()));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}