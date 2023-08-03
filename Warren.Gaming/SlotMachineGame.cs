using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;

namespace Warren.Consle
{
    public class SlotMachineGame
    {
        private readonly Settings _settings;
        private readonly SlotSettings _slotSettings;

        public SlotMachineGame(IOptions<Settings> settings, IOptions<SlotSettings> slotSettings)
        {
            _settings= settings.Value;
            _slotSettings= slotSettings.Value;
        }

        public void Execute()
        {

        }
    }
}
