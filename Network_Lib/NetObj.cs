
namespace Net
{
    public class NetObj
    {
        int id;
        int ownerId;

        public NetObj(int netObjId, int ownerId)
        {
            id = netObjId;
            this.ownerId = ownerId;
        }
    }
}