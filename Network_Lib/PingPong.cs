using System;

namespace Net
{
    public abstract class PingPong
    {
        protected NetworkEntity networkEntity;
        public float deltaTime = 0;
        protected int timeUntilDisconnection = 5;
        protected float sendMessageCounter = 0;
        protected float secondsPerCheck = 1.0f;
        protected DateTime currentDateTime = DateTime.UtcNow;
        protected DateTime lastUpdateTime = DateTime.UtcNow;

        public PingPong(NetworkEntity networkEntity)
        {
            this.networkEntity = networkEntity;
        }

        public void UpdateCheckActivity()
        {
            DateTime currentTime = DateTime.UtcNow;
            deltaTime = (float)(currentTime - lastUpdateTime).TotalSeconds;
            lastUpdateTime = currentTime;

            sendMessageCounter += deltaTime;

            if (sendMessageCounter > secondsPerCheck) // Envio cada 1 segundo el mensaje
            {
                SendPingMessage();
                sendMessageCounter = 0;
            }

            CheckActivityCounter(deltaTime);
            CheckTimeUntilDisconection();
        }

        protected abstract void CheckActivityCounter(float deltaTime);
        protected abstract void CheckTimeUntilDisconection();
        protected abstract void SendPingMessage();
    }
}