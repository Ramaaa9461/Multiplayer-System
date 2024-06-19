using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    public struct InstanceRequestPayload
    {
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

        public InstanceRequestPayload(int instanceId, float positionX, float positionY, float positionZ,
                                                       float rotationX, float rotationY, float rotationZ, float rotationW,
                                                       float scaleX, float scaleY, float scaleZ, int parentInstanceID)
        {
            this.objectId = instanceId;

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

    public class InstanceRequestMenssage : BaseMessage<InstanceRequestPayload>
    {
        private InstanceRequestPayload data;

        public InstanceRequestMenssage(MessagePriority messagePriority, InstanceRequestPayload data) : base(messagePriority)
        {
            currentMessageType = MessageType.InstanceRequest;
            this.data = data;
        }

        public InstanceRequestMenssage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.InstanceRequest;
            this.data = Deserialize(data);
        }

        public InstanceRequestPayload GetData()
        {
            return data;
        }

        public override InstanceRequestPayload Deserialize(byte[] message)
        {
            InstanceRequestPayload outData = new InstanceRequestPayload();

            if (MessageChecker.DeserializeCheckSum(message))
            {
                DeserializeHeader(message);

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
