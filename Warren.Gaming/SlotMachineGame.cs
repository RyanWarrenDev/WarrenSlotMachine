using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Warren.Domain;
using Warren.SlotMachine.Account;
using Warren.SlotMachine.SlotMachine;

namespace Warren.Consle
{
    public class SlotMachineGame
    {
        private readonly IAccountService _accountService;

        private readonly ISlotMachine _slotMachine;

        private readonly Settings _settings;

        public SlotMachineGame(IAccountService accountService, ISlotMachine slotMachine, IOptions<Settings> settings)
        {
            _accountService = accountService;
            _slotMachine = slotMachine;
            _settings = settings.Value;
        }

        public void Execute()
        {
            MakeDeposit();

            while (_accountService.GetBalance() > 0)
            {
                var selectedOption = WriteOptionsToConsole();

                if (selectedOption == 1)
                    PlaySlots();

                if (selectedOption == 2)
                    MakeDeposit();
            }
        }

        private void MakeDeposit()
        {
            var depositAmount = 0.0;
            while (depositAmount < _settings.MinDeposit || depositAmount> _settings.MaxDeposit)
            {
                Console.WriteLine($"Please enter a deposit amount between £{_settings.MinDeposit} & £{_settings.MaxDeposit}");
                if (!double.TryParse(Console.ReadLine(), out depositAmount))
                {
                    Console.WriteLine("Sorry, looks like something went wrong. Please try again");
                }
            }

            var accountBalance = _accountService.Deposit(depositAmount);

            Console.WriteLine($"Successfully deposited {depositAmount}, account balance now £{accountBalance}");
        }

        private int WriteOptionsToConsole()
        {
            var option = 0;
            while (!new[] { 1, 2 }.Contains(option))
            {
                Console.WriteLine("Please choose an option.");
                Console.WriteLine("\n1. Spin");
                Console.WriteLine("\n2. Make Deposit");
                Console.WriteLine($"\n\n Account Balance: £{_accountService.GetBalance()}");

                if (!int.TryParse(Console.ReadLine(), out option))
                {
                    Console.WriteLine("Sorry, looks like something went wrong. Please try again");
                }
            }

            return option;
        }

        private void PlaySlots()
        {
            var stakeAmount = GetStakeAmount();

            var result = _slotMachine.Spin(stakeAmount);

            foreach (var roll in result)
            {
                var rollIcons = roll.Symbols.Select(s => s.Icon);
                Console.WriteLine($"{string.Join("", rollIcons)} {(roll.WinLose ? "<- WINNER!" : string.Empty)}");
            }

            if (result.All(a => !a.WinLose))
            {
                Console.WriteLine("Sorry! Better luck next time!");
                return;
            }

            var winAmount = result.Sum(s => s.WinAmount);
            Console.WriteLine($"Congratulations! You just won £{winAmount.ToString("0.##")}");
        }

        private double GetStakeAmount()
        {
            var stakeAmount = 0.0;
            while (stakeAmount < _settings.MinStake || stakeAmount > _slotMachine.MaxStake)
            {
                Console.WriteLine($"Please enter a stake equal to or less than £{_slotMachine.MaxStake}");
                if (!double.TryParse(Console.ReadLine(), out stakeAmount))
                {
                    Console.WriteLine("Sorry, looks like something went wrong. Please try again");
                }
            }

            return stakeAmount;
        }
    }
}
