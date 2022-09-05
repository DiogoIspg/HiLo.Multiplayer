using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP2.Server
{
    class GameSession : TcpSession
    {
        public GameSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"TCP Client with Id {Id} connected!");

            // Send message
            var message = "Welcome to Hi-Lo Multiplayer. \n Press 'C' to Create a new game \nPress 'J' to join an existing game. \nPress '!' to disconnect the client!";
            SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"TCP Client with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Disconnect();

        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }

}
