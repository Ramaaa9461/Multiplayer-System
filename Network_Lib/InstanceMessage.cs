using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    public struct InstancePayload
    {
        public int instanceId;
        public int ownerId;

        public int objectId;

        public float positionX;
        public float positionY;
        public float positionZ;

        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float rotationW;

        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public int parentInstanceID;


        public InstancePayload(int instanceId, int ownerId, int objectId, float positionX, float positionY, float positionZ,
                                                            float rotationX, float rotationY, float rotationZ, float rotationW,
                                                            float scaleX, float scaleY, float scaleZ, int parentInstanceID)
        {
            this.instanceId = instanceId;
            this.ownerId = ownerId;

            this.objectId = objectId;

            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;

            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.rotationW = rotationW;

            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;

            this.parentInstanceID = parentInstanceID;
        }
    }

    public class InstanceMessage : BaseMessage<InstancePayload>
    {
        private InstancePayload data;

        public InstanceMessage(MessagePriority messagePriority, InstancePayload data) : base(messagePriority)
        {
            currentMessageType = MessageType.Instance;
            this.data = data;
        }

        public InstanceMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Instance;
            this.data = Deserialize(data);
        }

        public InstancePayload GetData()
        {
            return data;
        }

        public override InstancePayload Deserialize(byte[] message)
        {
            InstancePayload outData = new InstancePayload();

            if (MessageChecker.DeserializeCheckSum(message))
            {
                DeserializeHeader(message);

                outData.instanceId = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);
                outData.ownerId = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);
                outData.objectId = BitConverter.ToInt32(message, messageHeaderSize);
                messageHeaderSize += sizeof(int);

                outData.positionX = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);
                outData.positionY = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);
                outData.positionZ = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);

                outData.rotationX = BitConverter.ToSingle(message, messageHeaderSize); ;
                messageHeaderSize += sizeof(float);
                outData.rotationY = BitConverter.ToSingle(message, messageHeaderSize); ;
                messageHeaderSize += sizeof(float);
                outData.rotationZ = BitConverter.ToSingle(message, messageHeaderSize); ;
                messageHeaderSize += sizeof(float);
                outData.rotationW = BitConverter.ToSingle(message, messageHeaderSize); ;
                messageHeaderSize += sizeof(float);

                outData.scaleX = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);
                outData.scaleY = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);
                outData.scaleZ = BitConverter.ToSingle(message, messageHeaderSize);
                messageHeaderSize += sizeof(float);

                outData.parentInstanceID = BitConverter.ToInt32(message, messageHeaderSize);
            }

            return outData;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.instanceId));
            outData.AddRange(BitConverter.GetBytes(data.ownerId));

            outData.AddRange(BitConverter.GetBytes(data.objectId));

            SerializeVec3(ref outData, data.positionX, data.positionY, data.positionZ);
            SerializeVec3(ref outData, data.rotationX, data.rotationY, data.rotationZ);
            outData.AddRange(BitConverter.GetBytes(data.rotationW));
            SerializeVec3(ref outData, data.scaleX, data.scaleY, data.scaleZ);

            outData.AddRange(BitConverter.GetBytes(data.parentInstanceID));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }

        void SerializeVec3(ref List<byte> outData, float x, float y, float z)
        {
            outData.AddRange(BitConverter.GetBytes(x));
            outData.AddRange(BitConverter.GetBytes(y));
            outData.AddRange(BitConverter.GetBytes(z));
        }

       
    }
}
