using HiLo.Multiplayer.Shared;
using System;
using System.Collections.Generic;

namespace HiLo.Multiplayer.Shared
{
    public class GameState
    {
        public Guid GameId { get; set; }
        public List<Guid> PlayerIds { get; set; }
        public int SecretNumber { get; set; }

        public bool GameEnded { get; set; }
    }
}
