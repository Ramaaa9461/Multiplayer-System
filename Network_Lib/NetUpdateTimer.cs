using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetUpdateTimer : BaseMessage<bool>
    {
        bool initTimer;

        public NetUpdateTimer(MessagePriority messagePriority, bool initTimer) : base(messagePriority)
        {
            currentMessageType = MessageType.UpdateLobbyTimer;
            this.initTimer = initTimer;
        }

        public NetUpdateTimer(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.UpdateLobbyTimer;
            this.initTimer = Deserialize(data);
        }

        public bool GetData()
        {
            return initTimer;
        }

        public override bool Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                return BitConverter.ToBoolean(message, messageHeaderSize);
            }

            return false;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(initTimer));
            outData.AddRange(BitConverter.GetBytes(initTimer));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}