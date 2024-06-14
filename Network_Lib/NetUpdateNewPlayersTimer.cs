using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetUpdateNewPlayersTimer : BaseMessage<float>
    {
        float timer = -1;

        public NetUpdateNewPlayersTimer(MessagePriority messagePriority, float timer) : base(messagePriority)
        {
            currentMessageType = MessageType.UpdateLobbyTimerForNewPlayers;
            this.timer = timer;
        }

        public NetUpdateNewPlayersTimer(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.UpdateLobbyTimerForNewPlayers;
            timer = Deserialize(data);
        }

        public float GetData()
        {
            return timer;
        }

        public override float Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                timer = BitConverter.ToSingle(message, messageHeaderSize);
            }

            return timer;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(timer));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}