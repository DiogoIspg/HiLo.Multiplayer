using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiLo.Multiplayer.Shared
{
    public class PlayerGameState
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public List<Guid> PlayerIds { get; set; }
        public int CurrentPlay { get; set; }
        public HiloState HighLow { get; set; }
    }
}
