using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;

namespace Warren.SlotMachine.SlotMachine
{
    public interface ISlotMachine
    {
        double MaxStake { get; }

        public IList<SpinResult> Spin(double stakeAmount);
    }
}
