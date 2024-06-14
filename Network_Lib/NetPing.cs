using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetPing
    {
        MessageType messageType = MessageType.Ping;

        public MessageType GetMessageType()
        {
            return messageType;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes((int)MessagePriority.Default));

            return outData.ToArray();
        }
    }
}