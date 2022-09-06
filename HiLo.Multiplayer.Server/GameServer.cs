using NetCoreServer;
using System;
using System.Net;
using System.Net.Sockets;

namespace HiLo.Multiplayer.Server
{
    class GameServer : TcpServer
    {
        public GameServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new GameSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"TCP server caught an error with code {error}");
        }
    }
}
