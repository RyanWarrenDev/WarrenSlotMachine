using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warren.Domain
{
    public class SpinResult
    {
        public IList<Symbol> Symbols { get; set; }

        public bool WinLose { get; set; }

        public double WinAmount { get; set; }

        public SpinResult()
        {
            Symbols = new List<Symbol>();
            WinLose = false;
            WinAmount = 0;
        }
    }
}
