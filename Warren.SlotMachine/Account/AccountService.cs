using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warren.SlotMachine.Account
{
    public class AccountService : IAccountService
    {
        private double AccountBalance { get; set; }

        public AccountService()
        {
            AccountBalance = 0;
        }

        public double Deposit(double depositAmount)
        {
            AccountBalance += depositAmount;
            return AccountBalance;
        }

        public double Withdraw(double withdrawlAmount)
        {
            AccountBalance -= withdrawlAmount;
            return AccountBalance;
        }

        public double GetBalance() => AccountBalance;

    }
}
