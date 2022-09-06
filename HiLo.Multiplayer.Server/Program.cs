using System;
using System.Net;

namespace HiLo.Multiplayer.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameServer = new GameServer(IPAddress.Any, 9898);

            Console.Write("Server starting...");
            gameServer.Start();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    gameServer.Restart();
                    Console.WriteLine("Done!");
                    continue;
                }

                // Multicast admin message to all sessions
                line = "(admin) " + line;
                gameServer.Multicast(line);
            }

            // Stop the server
            Console.Write("Server stopping...");
            gameServer.Stop();
            Console.WriteLine("Done!");
        }
    }
}
