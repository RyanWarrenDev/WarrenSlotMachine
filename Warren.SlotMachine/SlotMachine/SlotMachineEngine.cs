using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;

namespace Warren.SlotMachine.SlotMachine
{
    public class SlotMachineEngine : ISlotMachineEngine
    {
        private readonly SlotSettings _slotSettings;

        private readonly Random _random = new Random();

        public SlotMachineEngine(IOptions<SlotSettings> slotSettings)
        {
            _slotSettings = slotSettings.Value;
        }

        public SpinResult Roll()
        {
            var spinResult = new SpinResult();

            while (spinResult.Symbols.Count() < _slotSettings.SymbolsPerRoll)
            {
                var randomSymbol = GetRandomSymbol();
                spinResult.Symbols.Add(randomSymbol);
            }

            return spinResult;
        }

        private Symbol GetRandomSymbol()
        {
            var spinValue = _random.NextDouble();
            var cumulative = 0.0;

            Symbol winner = null;
            foreach (var symbol in _slotSettings.Symbols.OrderBy(o => o.Probability))
            {
                cumulative += symbol.Probability;
                if (spinValue < cumulative)
                {
                    winner = symbol;
                    break;
                }
            }

            return winner!;
        }

        public void EvaluateSpin(SpinResult spinResult, double stakeAmount)
        {
            var distinctSymols = spinResult.Symbols.GroupBy(g => g.Icon).Select(s => s.First());
            foreach (var symbol in distinctSymols)
            {
                var winningSymbols = spinResult.Symbols.Where(c => c.Icon == symbol.Icon || c.IsWildcard);
                if (winningSymbols.Count() < _slotSettings.MatchToWin)
                    continue;

                spinResult.WinLose = true;
                spinResult.WinAmount = winningSymbols.Sum(s => s.Coefficient) * stakeAmount;
            }
        }
    }
}
