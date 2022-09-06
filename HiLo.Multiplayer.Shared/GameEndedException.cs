using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiLo.Multiplayer.Shared
{
    public class GameEndedException : Exception
    {
        public GameEndedException(string message) : base(message)
        {
        }
    }
}
