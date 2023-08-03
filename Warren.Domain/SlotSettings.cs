using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warren.Domain
{
    public class SlotSettings
    {
        public int SymbolsPerRoll { get; set; }

        public int RollsPerSpin { get; set; }

        public int MatchToWin { get; set; }

        public IList<Symbol> Symbols { get; set; }
    }
}
