using HiLo.Multiplayer.Logic;
using HiLo.Multiplayer.Shared;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HiLo.Multiplayer.Server
{
    public class GameSession : TcpSession
    {
        public static List<HiloGame> currentGames = new();
        public static List<Lobby> currentlyOpenLobbies = new();
        public static Dictionary<Guid, int> playerMoves = new();

        public GameSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"TCP Client with Id {Id} connected!");

            // Send message
            var message = "Welcome to Hi-Lo Multiplayer. \n\nPress 'C' to Create a new game \nPress 'J' to join an existing game. \nPress '!' to disconnect the client!";
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

            ParseClientMessage(Id, message);
        }

        private void ParseClientMessage(Guid id, string message)
        {
            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
            {
                Disconnect();
                return;
            }
            
            if(message == "J")
            {
                var lobby = currentlyOpenLobbies.FirstOrDefault();
                if (lobby == null)
                {
                    SendAsync("No lobbies available, please create a new game.");
                    return;
                }

                lobby.ListPlayers.Add(id);
                SendAsync("Lobby joined! Waiting for game to start...");
                return;
            }

            if(message == "S")
            {
                var lobby = currentlyOpenLobbies.FirstOrDefault(x => x.CreatorPlayerId == id);
                if (lobby == null)
                {
                    SendAsync("You're not in a lobby.");
                    return;
                }

                // TODO: why 2 players ?
                //if (lobby.ListPlayers.Count < 2)
                //{
                //    SendAsync("You need at least 2 players to start a game.");
                //}

                currentGames.Add(new HiloGame(lobby.ListPlayers));
                currentlyOpenLobbies = currentlyOpenLobbies.Where(x => x.GameId != lobby.GameId).ToList();

                foreach (var player in lobby.ListPlayers)
                {
                    Server.FindSession(player).SendAsync("Game Started, type a number: \n");
                }

                return;
            }

            if (message == "C")
            {
                if(currentGames.Any(x => x.gameState.PlayerIds.Any(y => y == id)))
                {
                    // TODO: Message type ?
                    SendAsync("You can't create a room when already inside a game.");
                    return;
                }

                var newLobby = Guid.NewGuid();
                currentlyOpenLobbies.Add(new Lobby() { CreatorPlayerId = id, GameId = newLobby, ListPlayers = new List<Guid>() { id } });
                SendAsync("Lobby Created, waiting for other players to join...");
                return;
            }

            if(int.TryParse(message, out int num))
            {
                var game = FindPlayerGame(id);
                if(game == null)
                {
                    SendAsync("You're currently not in-game.");
                }

                var (playerGameState, gameEnded) = StoreOrMove(game, id, num);

                if(playerGameState == null)
                {
                    SendAsync("Move registered, waiting for other players...");
                    return;
                }
              
                foreach (var item in playerGameState)
                {
                    if (gameEnded)
                    {
                        if (item.HighLow == HiloState.Correct)
                        {
                            Server.FindSession(item.PlayerId).SendAsync("Correct number! \n End of game.");
                            Server.FindSession(item.PlayerId).Disconnect();
                        }
                        else
                        {
                            Server.FindSession(item.PlayerId).SendAsync("Another player won :( \n End of game.");
                            Server.FindSession(item.PlayerId).Disconnect();
                        }
                    }
                    else
                    {
                        var highLow = item.HighLow == HiloState.High ? "Hi" : "Lo";
                        Server.FindSession(item.PlayerId).SendAsync($"Your tentative was {highLow} try again.");
                    }
                }
            }
        }

        private (List<PlayerGameState>, bool gameEnded) StoreOrMove(HiloGame game, Guid id, int num)
        {
            if(playerMoves.ContainsKey(id))
            {
                playerMoves[id] = num;
                SendAsync("Number Changed! Waiting for other Players");
                return (null,false);
            }

            playerMoves.Add(id,num);

            var listOfPlayers = game.gameState.PlayerIds;
            var playerMovesId = playerMoves.Select(x => x.Key);

            if (listOfPlayers.All(x => playerMovesId.Contains(x)))
            {
                var allPlayerMoves = playerMoves
                        .Where(x => listOfPlayers.Contains(x.Key))
                        .Select(x => new PlayerGameState()
                        {
                            CurrentPlay = x.Value,
                            GameId = game.gameState.GameId,
                            PlayerId = x.Key,
                        }).ToList();

                foreach (var item in listOfPlayers)
                {
                    playerMoves.Remove(item);
                }

                return game.MakePlay(allPlayerMoves);
            }

            SendAsync("Move registered, waiting for other players...");
            return (null, false);
        }

        private HiloGame FindPlayerGame(Guid id)
        {
            return currentGames.FirstOrDefault(x => x.gameState.PlayerIds.Any(y => y == id));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }

}
