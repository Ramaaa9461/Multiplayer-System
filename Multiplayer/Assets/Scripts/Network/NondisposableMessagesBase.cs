using System.Collections.Generic;
using System.Net;

public abstract class NondisponsableMessageBase
{
    protected NetworkEntity networkEntity;
    protected Dictionary<byte[], float> MessagesHistory = new();
    protected int secondsToDeleteMessageHistory = 15;
    protected PingPong pingPong;

    public NondisponsableMessageBase(NetworkEntity networkEntity)
    {
        this.networkEntity = networkEntity;
        networkEntity.onInitPingPong += () => pingPong = networkEntity.checkActivity;
    }

    public void OnReceivedData(byte[] data, int id)
    {
        MessagePriority messagePriority = MessageChecker.CheckMessagePriority(data);
        MessageType messageType = MessageChecker.CheckMessageType(data);

        if ((messagePriority & MessagePriority.NonDisposable) != 0)
        {
            SendConfirmationMessage(messageType, id);
        }

        if (messageType == MessageType.Confirm)
        {
            NetConfirmMessage netConfirm = new(data);

            HandleConfirmationMessage(netConfirm, id);
        }
    }

    protected abstract void HandleConfirmationMessage(NetConfirmMessage netConfirm, int id);
    public abstract void AddSentMessages(byte[] data, int clientId = -1);
    public abstract void SendConfirmationMessage(MessageType messageType, int id = -1);
    public abstract void ResendPackages();
}