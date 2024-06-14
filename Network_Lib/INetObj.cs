namespace Net
{
    public interface INetObj
    {
        int GetID();

        int GetOwnerID();

        NetObj GetNetObj();
    }
}