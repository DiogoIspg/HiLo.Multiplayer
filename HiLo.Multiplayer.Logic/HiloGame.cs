using HiLo.Multiplayer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HiLo.Multiplayer.Logic
{
    public class HiloGame : IHiloGame
    {
        public virtual GameState gameState { get; private set; }

        private const int MAX_PLAYABLE_NUMBER = 200;

        public HiloGame(List<Guid> playerIds)
        {
            gameState = new GameState()
            {
                GameId = Guid.NewGuid(),
                PlayerIds = playerIds,
                SecretNumber = new Random().Next(1, MAX_PLAYABLE_NUMBER),
                GameEnded = false,
            };
        }

        public (List<PlayerGameState>, bool gameEnded) MakePlay(List<PlayerGameState> allPlayersMoves)
        {
            if (this.gameState.GameEnded)
            {
                throw new GameEndedException($"Game id {this.gameState.GameId} already ended, but players tried to make a move!");
            }

            foreach (var move in allPlayersMoves)
            {
                // TODO: remove state mutation
                move.HighLow = MakePlay(move.CurrentPlay);
            }

            this.gameState.GameEnded = allPlayersMoves.Any(move => move.HighLow == HiloState.Correct);

            return (allPlayersMoves, this.gameState.GameEnded);
        }

        private HiloState MakePlay(int numberPlayed)
        {
            if (numberPlayed == this.gameState.SecretNumber)
            {
                return HiloState.Correct;
            }

            if (numberPlayed > this.gameState.SecretNumber)
            {
                return HiloState.High;
            }

            return HiloState.Low;
        }
    }
}
