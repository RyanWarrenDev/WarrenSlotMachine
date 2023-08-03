using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;

namespace Warren.SlotMachine.Validation
{
    public class SlotMachineSettingsValidator
    {
        public static bool ValidateSettings(Settings settings)
        {
            if (settings == null || settings.SlotSettings == null)
                return false;

            if (settings.MinDeposit == default)
                return false;

            if (settings.MaxDeposit == default)
                return false;

            if (settings.MinStake == default)
                return false;

            if (settings.MaxStake == default)
                return false;

            return ValidateSlotSettings(settings.SlotSettings);
        }

        public static bool ValidateSlotSettings(SlotSettings slotSettings)
        {
            if (slotSettings == null)
                return false;

            if (slotSettings.SymbolsPerRoll == default)
                return false;

            if (slotSettings.RollsPerSpin == default)
                return false;
            if (slotSettings.Symbols == null || slotSettings.Symbols.Count == 0)
                return false;

            //Find duplicates
            if (slotSettings.Symbols.GroupBy(g => g.Icon).Where(c => c.Count() > 1).Count() > 1)
                return false;

            //Sum of all probabilities must total 1
            var probabilitySum = slotSettings.Symbols.Sum(s => s.Probability);
            if (probabilitySum != 1)
                return false;

            return true;
        }
    }
}
