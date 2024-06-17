using System;

namespace Match_Maker
{
    class Program
    {
        static void Main(string[] args)
        {
            int initalPort = 60000;
            DateTime dateTime = DateTime.UtcNow;
            MatchMaker matchMaker = new MatchMaker(initalPort, dateTime);

            Console.WriteLine($"Match Maker created succesfully in port {initalPort} ({dateTime})");

            while (true)
            {
                matchMaker.Update();
            }
        }
    }
}
