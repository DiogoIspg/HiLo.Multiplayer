using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiLo.Multiplayer.Server
{
    public class Lobby
    {
        public Guid GameId { get; set; }

        public Guid CreatorPlayerId { get; set; }

        public List<Guid> ListPlayers { get; set; }
    }
}
