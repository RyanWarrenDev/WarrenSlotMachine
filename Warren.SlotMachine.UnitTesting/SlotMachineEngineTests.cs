using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;
using Warren.SlotMachine.SlotMachine;

namespace Warren.SlotMachine.UnitTesting
{
    public class SlotMachineEngineTests : SlotMachineEngine
    {
        private SlotSettings slotSettings;

        static IOptions<SlotSettings> options = Options.Create(new SlotSettings()
        {

            MatchToWin = 3,
            RollsPerSpin = 3,
            SymbolsPerRoll = 3,
            Symbols = new[] {
                    new Symbol("A", 0.45, 0.4),
                    new Symbol("B", 0.35, 0.6),
                    new Symbol("P", 0.15, 0.8),
                    new Symbol("*", 0.05, 0, true)
                }
        });

        public SlotMachineEngineTests(): base(options)
        {
            slotSettings = options.Value;
        }

        [Fact]
        public void RollTest()
        {
            var result = Roll();

            Assert.Equal(slotSettings.SymbolsPerRoll, result.Symbols.Count());
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(3000)]
        public void ProbabilityTest(int numberOfTests)
        {
            var results = new List<Symbol>();
            while (results.Count < numberOfTests)
            {
                results.Add(GetRandomSymbol());
            }

            foreach (var symbol in slotSettings.Symbols)
            {
                var occurrences = results.Where(w => w.Icon == symbol.Icon).ToList();
                var percentageOccurrences = (double)occurrences.Count / (double)results.Count;

                //Still random number so assert within +/- 2%
                Assert.InRange(percentageOccurrences, symbol.Probability - 0.02, symbol.Probability + 0.02);
            }
        }

        [Theory]
        [InlineData(0, 0, 0, true)]
        [InlineData(1, 1, 1, true)]
        [InlineData(2, 2, 2, true)]
        [InlineData(3, 3, 3, true)]
        [InlineData(0, 0, 3, true)]
        [InlineData(1, 1, 3, true)]
        [InlineData(2, 2, 3, true)]
        [InlineData(0, 3, 3, true)]
        [InlineData(1, 3, 3, true)]
        [InlineData(2, 3, 3, true)]
        [InlineData(0, 0, 1, false)]
        [InlineData(1, 1, 0, false)]
        [InlineData(2, 2, 0, false)]
        public void EvalutateSpinToKnownHands(int symbol1, int symbol2, int symbol3, bool winLose)
        {
            var stakeAmount = 10;
            var winningRoll = new SpinResult()
            {
                Symbols = new[] {
                    slotSettings.Symbols[symbol1],
                    slotSettings.Symbols[symbol2],
                    slotSettings.Symbols[symbol3]
                }
            };

            EvaluateSpin(winningRoll, stakeAmount);

            Assert.Equal(winLose, winningRoll.WinLose);

            if (winLose)
                Assert.Equal(winningRoll.WinAmount, winningRoll.Symbols.Sum(s => s.Coefficient) * stakeAmount);
        }
    }
}
