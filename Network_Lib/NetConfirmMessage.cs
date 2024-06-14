using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetConfirmMessage : BaseMessage<MessageType>
    {
        MessageType messageTypeToConfirm = MessageType.Default;

        public NetConfirmMessage(MessagePriority messagePriority, MessageType messageTypeToConfirm) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Confirm;
            this.messageTypeToConfirm = messageTypeToConfirm;
        }

        public NetConfirmMessage(byte[] message) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Confirm;
            this.messageTypeToConfirm = Deserialize(message);
        }

        public MessageType GetData()
        {
            return messageTypeToConfirm;
        }

        public override MessageType Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                messageTypeToConfirm = (MessageType)BitConverter.ToInt32(message, messageHeaderSize);
            }

            return messageTypeToConfirm;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes((int)messageTypeToConfirm));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}