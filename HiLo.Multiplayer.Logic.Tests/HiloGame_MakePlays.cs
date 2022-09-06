using HiLo.Multiplayer.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HiLo.Multiplayer.Logic.Tests
{
    public class HiloGame_MakePlays
    {
        [Fact]
        public void NewHiloGame_2PlayersMakePlay_ValidHighMove()
        {
            var gameId = Guid.NewGuid();

            var player1 = Guid.NewGuid();
            var player2 = Guid.NewGuid();
            var players = new List<Guid> { player1, player2 };

            var gameInstance = new Mock<HiloGame>(players);          

            gameInstance.Setup(x => x.gameState)
                .Returns(new GameState() 
                { 
                        GameEnded = false, 
                        GameId = gameId, 
                        PlayerIds = players, 
                        SecretNumber = 199 
                });

            var playerGameState = new List<PlayerGameState>()
            {
                new PlayerGameState()
                {
                    GameId = gameId,
                    CurrentPlay = 200,
                    PlayerId = player1,
                },
                new PlayerGameState()
                {
                    GameId = gameId,
                    CurrentPlay = 200,
                    PlayerId = player2,
                },
            };

            var result = gameInstance.Object.MakePlay(playerGameState);

            Assert.Equal(HiloState.High, result.Item1.ElementAtOrDefault(0).HighLow);
            Assert.Equal(HiloState.High, result.Item1.ElementAtOrDefault(1).HighLow);
            Assert.False(result.gameEnded);
        }

        [Fact]
        public void NewHiloGame_2PlayersMakePlay_ValidLowMove()
        {
            var gameId = Guid.NewGuid();

            var player1 = Guid.NewGuid();
            var player2 = Guid.NewGuid();
            var players = new List<Guid> { player1, player2 };

            var gameInstance = new Mock<HiloGame>(players);

            gameInstance.Setup(x => x.gameState)
                .Returns(new GameState()
                {
                    GameEnded = false,
                    GameId = gameId,
                    PlayerIds = players,
                    SecretNumber = 199
                });

            var playerGameState = new List<PlayerGameState>()
            {
                new PlayerGameState()
                {
                    GameId = gameId,
                    CurrentPlay = 198,
                    PlayerId = player1,
                },
                new PlayerGameState()
                {
                    GameId = gameId,
                    CurrentPlay = 198,
                    PlayerId = player2,
                },
            };

            var result = gameInstance.Object.MakePlay(playerGameState);

            Assert.Equal(HiloState.Low, result.Item1.ElementAtOrDefault(0).HighLow);
            Assert.Equal(HiloState.Low, result.Item1.ElementAtOrDefault(1).HighLow);
            Assert.False(result.gameEnded);

        }

        [Fact]
        public void NewHiloGame_2PlayersMakePlay_Valid1Winner()
        {
            var gameId = Guid.NewGuid();

            var player1 = Guid.NewGuid();
            var player2 = Guid.NewGuid();
            var players = new List<Guid> { player1, player2 };

            var gameInstance = new Mock<HiloGame>(players);

            gameInstance.Setup(x => x.gameState)
                .Returns(new GameState()
                {
                    GameEnded = false,
                    GameId = gameId,
                    PlayerIds = players,
                    SecretNumber = 199
                });

            var playerGameState = new List<PlayerGameState>()
            {
                new PlayerGameState()
                {
                    GameId = gameId,
                    CurrentPlay = 199,
                    PlayerId = player1,
                },
                new PlayerGameState()
                {
                    GameId = gameId,
                    CurrentPlay = 198,
                    PlayerId = player2,
                },
            };

            var result = gameInstance.Object.MakePlay(playerGameState);

            Assert.Equal(HiloState.Correct, result.Item1.ElementAtOrDefault(0).HighLow);
            Assert.Equal(HiloState.Low, result.Item1.ElementAtOrDefault(1).HighLow);
            Assert.True(result.gameEnded);
        }

        // TODO: ilegal moves 
    }
}
