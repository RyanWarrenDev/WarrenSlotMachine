using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;
using Warren.SlotMachine.Account;
using Warren.SlotMachine.SlotMachine;

namespace Warren.SlotMachine.UnitTesting
{
    public class SlotMachineTests
    {
        private Mock<IAccountService> _accountService = new Mock<IAccountService>();

        private Mock<ISlotMachineEngine> _slotMachineEngine = new Mock<ISlotMachineEngine>();

        private Settings settings;
        
        private SlotMachine.SlotMachine sut;

        public SlotMachineTests()
        {
            IOptions<Settings> options = Options.Create(new Settings()
            {
                MaxDeposit = 100,
                MinDeposit = 100,
                MaxStake = 30,
                MinStake = 10,
                SlotSettings = new SlotSettings
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
                }
            });

            settings = options.Value;

            sut = new SlotMachine.SlotMachine(options, _accountService.Object, _slotMachineEngine.Object);
        }

        [Theory]
        [InlineData(100, AccountOrSettings.Settings)]
        [InlineData(20, AccountOrSettings.Account)]
        public void MaxStakeTest(double accountBalance, AccountOrSettings source)
        {
            _accountService.Setup(s => s.GetBalance()).Returns(accountBalance);

            //If account balance is greater than settings
            if (source == AccountOrSettings.Settings)
                Assert.Equal(sut.MaxStake, settings.MaxStake);
            else
                Assert.Equal(sut.MaxStake, _accountService.Object.GetBalance());
        }

        [Theory]
        [InlineData(100, 25, true)]
        public void SpinTest(double accountBalance, double stakeAmount, bool winningRoll)
        {
            _accountService.Setup(s => s.GetBalance()).Returns(accountBalance);

            if (winningRoll)
                _slotMachineEngine.Setup(s => s.Roll()).Returns(new SpinResult { 
                    Symbols = new[] { settings.SlotSettings.Symbols[1], settings.SlotSettings.Symbols[1], settings.SlotSettings.Symbols[1] },
                    WinAmount = (settings.SlotSettings.Symbols[1].Coefficient * 3) * stakeAmount,
                    WinLose = true
                });
            else
                _slotMachineEngine.Setup(s => s.Roll()).Returns(new SpinResult
                {
                    Symbols = new[] { settings.SlotSettings.Symbols[1], settings.SlotSettings.Symbols[2], settings.SlotSettings.Symbols[3] },
                    WinAmount = 0,
                    WinLose = false
                });

            var result = sut.Spin(stakeAmount);

            _accountService.Verify(v => v.Withdraw(It.Is<double>(a => a.Equals(stakeAmount))), Times.Once);
            _slotMachineEngine.Verify(v => v.Roll(), Times.Exactly(settings.SlotSettings.RollsPerSpin));
            _slotMachineEngine.Verify(v => v.EvaluateSpin(It.IsAny<SpinResult>(), It.Is<double>(a => a.Equals(stakeAmount))), 
                Times.Exactly(settings.SlotSettings.RollsPerSpin));

            if (winningRoll)
                _accountService.Verify(v => v.Deposit(It.Is<double>(a => a.Equals(result.Sum(s => s.WinAmount)))), Times.Once);

            Assert.Equal(settings.SlotSettings.RollsPerSpin, result.Count);
        }

        [Fact]
        public void SpinOverMaxStake()
        {
            _accountService.Setup(s => s.GetBalance()).Returns(10);

            Action spin = () => sut.Spin(35);
            var exception = Assert.Throws<Exception>(spin);
            Assert.Equal($"Please enter a stake amount below {sut.MaxStake}", exception.Message);
        }

        [Fact]
        public void SpinWithException()
        {
            var stakeAmount = 25;

            _accountService.Setup(s => s.GetBalance()).Returns(100);
            _slotMachineEngine.Setup(s => s.Roll()).Throws(new Exception());

            sut.Spin(stakeAmount);
            _accountService.Verify(v => v.Deposit(It.Is<double>(a => a.Equals(stakeAmount))), Times.Once);
        }

        public enum AccountOrSettings
        {
            Account, 
            Settings
        }
    }
}
