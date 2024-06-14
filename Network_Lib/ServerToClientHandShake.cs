using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class ServerToClientHandShake : BaseMessage<List<(int clientID, string clientName)>>
    {

        private List<(int clientID, string clientName)> data;

        public ServerToClientHandShake(MessagePriority messagePriority, List<(int clientID, string clientName)> data) : base(messagePriority)
        {
            currentMessageType = MessageType.ServerToClientHandShake;
            this.data = data;

        }

        public ServerToClientHandShake(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.ServerToClientHandShake;
            this.data = Deserialize(data);
        }

        public List<(int clientID, string clientName)> GetData()
        {
            return data;
        }

        public override List<(int clientID, string clientName)> Deserialize(byte[] message)
        {
            List<(int clientID, string clientName)> outData = new List<(int, string)>();

            if (MessageChecker.DeserializeCheckSum(message))
            {
                DeserializeHeader(message);

                int listCount = BitConverter.ToInt32(message, messageHeaderSize);

                int offSet = messageHeaderSize + sizeof(int);
                for (int i = 0; i < listCount; i++)
                {
                    int clientID = BitConverter.ToInt32(message, offSet);
                    offSet += sizeof(int);
                    int clientNameLength = BitConverter.ToInt32(message, offSet);
                    string name = MessageChecker.DeserializeString(message, offSet);
                    offSet += sizeof(char) * clientNameLength + sizeof(int);

                    outData.Add((clientID, name));
                }
            }
            return outData;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.Count));

            foreach ((int clientID, string clientName) clientInfo in data)
            {
                outData.AddRange(BitConverter.GetBytes(clientInfo.clientID)); // ID del client
                outData.AddRange(MessageChecker.SerializeString(clientInfo.clientName.ToCharArray())); //Nombre
            }

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}