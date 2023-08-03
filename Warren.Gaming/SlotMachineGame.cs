using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;
using Warren.SlotMachine.SlotMachine;

namespace Warren.Consle
{
    public class SlotMachineGame
    {
        private readonly ISlotMachine _slotMachine;

        public SlotMachineGame(ISlotMachine slotMachine)
        {
            this._slotMachine = slotMachine;   
        }

        public void Execute()
        {

        }
    }
}
