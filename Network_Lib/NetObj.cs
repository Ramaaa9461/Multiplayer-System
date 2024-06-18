
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

        public void SetValues(int id, int ownerId)
        {
            this.id = id;
            this.ownerId = ownerId;
        }


        public int OwnerId
        {
            get { return ownerId; }
        }
        public int ID
        {
            get { return id; }
        }
    }
}