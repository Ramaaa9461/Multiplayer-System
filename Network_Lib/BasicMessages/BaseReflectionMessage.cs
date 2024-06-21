using System.Collections.Generic;

namespace Net
{
    public abstract class BaseReflectionMessage<T> : BaseMessage<T>
    {
        protected List<int> messageRoute = new List<int>();

        public BaseReflectionMessage(MessagePriority messagePriority, List<int> messageRoute) : base(messagePriority)
        {
            this.messageRoute = messageRoute;
        }

        public List<int> GetMessageRoute()
        {
            return messageRoute;
        }
    }
}
