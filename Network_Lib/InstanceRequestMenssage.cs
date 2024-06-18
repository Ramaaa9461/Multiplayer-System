using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    public struct InstanceRequestPayload
    {
        public int objectId;
        public Vec3 position;
        public Vec3 rotation;
        public Vec3 scale;
        public int parentInstanceID;

        public InstanceRequestPayload(int instanceId, Vec3 position, Vec3 rotation, Vec3 scale, int parentInstanceID)
        {
            this.objectId = instanceId;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
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

                outData.position = DeserializeVec3(message, ref messageHeaderSize);
                outData.rotation = DeserializeVec3(message, ref messageHeaderSize);
                outData.scale = DeserializeVec3(message, ref messageHeaderSize);

                outData.parentInstanceID = BitConverter.ToInt32(message, messageHeaderSize);
            }

            return outData;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(data.objectId));
            SerializeVec3(ref outData, data.position);
            SerializeVec3(ref outData, data.rotation);
            SerializeVec3(ref outData, data.scale);
            outData.AddRange(BitConverter.GetBytes(data.parentInstanceID));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }

        void SerializeVec3(ref List<byte> outData, Vec3 vec3)
        {
            outData.AddRange(BitConverter.GetBytes(vec3.x));
            outData.AddRange(BitConverter.GetBytes(vec3.y));
            outData.AddRange(BitConverter.GetBytes(vec3.z));
        }

        Vec3 DeserializeVec3(byte[] message, ref int messageHeaderSize)
        {
            Vec3 outVec3 = Vec3.Zero;

            outVec3.x = BitConverter.ToSingle(message, messageHeaderSize);
            messageHeaderSize += sizeof(float);
            outVec3.y = BitConverter.ToSingle(message, messageHeaderSize);
            messageHeaderSize += sizeof(float);
            outVec3.z = BitConverter.ToSingle(message, messageHeaderSize);
            messageHeaderSize += sizeof(float);

            return outVec3;
        }
    }
}
