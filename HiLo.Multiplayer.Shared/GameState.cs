using System;
using System.Collections.Generic;

namespace TCP2.Shared
{
    public class GameState
    {
        public Guid GameId { get; set; }
        public List<Guid> PlayerIds { get; set; }
        public int SecretNumber { get; set; }
    }
}
