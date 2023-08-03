using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Warren.Domain;
using Warren.SlotMachine.Account;

namespace Warren.SlotMachine.SlotMachine
{
    public class SlotMachine : ISlotMachine
    {
        private readonly Settings _settings;

        private readonly IAccountService _accountService;

        private readonly ISlotMachineEngine _slotMachineEngine;

        private double MinStake
        {
            get
            {
                var accountBalance = _accountService.GetBalance();
                return accountBalance > _settings.MinStake ? _settings.MinStake : accountBalance;
            }
        }

        private double MaxStake
        {
            get
            {
                var accountBalance = _accountService.GetBalance();
                return accountBalance > _settings.MaxStake ? _settings.MaxStake : accountBalance;
            }
        }

        public SlotMachine(IOptions<Settings> settings, IAccountService accountService, ISlotMachineEngine slotMachineEngine)
        {
            _settings = settings.Value;
            _accountService = accountService;
            _slotMachineEngine = slotMachineEngine;
        }

        public IList<SpinResult> Spin(double stakeAmount)
        {
            if (stakeAmount < MinStake || stakeAmount > MaxStake)
                throw new Exception($"Please enter a stake amount between {MinStake} & {MaxStake}");

            var results = new List<SpinResult>();
            try
            {
                //Withdraw from account
                _accountService.Withdraw(stakeAmount);

                //Spin machine
                while (results.Count < _settings.SlotSettings.RollsPerSpin)
                {
                    var roll = RollAndEvaluateResult(stakeAmount);
                    results.Add(roll);
                }
            }catch (Exception ex)
            {
                _accountService.Deposit(stakeAmount);
            }

            //Deposit to account
            if (results.Any(a => a.WinLose))
                _accountService.Deposit(results.Where(w => w.WinLose).Sum(s => s.WinAmount));

            return results;
        }

        private SpinResult RollAndEvaluateResult(double stakeAmount)
        {
            var roll = _slotMachineEngine.Roll();
            _slotMachineEngine.EvaluateSpin(roll, stakeAmount);

            return roll;
        }
    }
}
