using System;

namespace NetworkServer
{
    class Program
    {

        static void Main(string[] args)
        {
            int port = 51000;
            DateTime dateTime = DateTime.UtcNow;
            Server server = new Server(port, dateTime);

            while (true)
            {
                server.Update();
            }
        }
    }
}
