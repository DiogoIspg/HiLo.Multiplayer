using HiLo.Multiplayer.Shared;
using System.Collections.Generic;

namespace HiLo.Multiplayer.Logic
{
    public interface IHiloGame
    {
        GameState gameState { get; }

        (List<PlayerGameState>, bool gameEnded) MakePlay(List<PlayerGameState> allPlayersMoves);
    }
}