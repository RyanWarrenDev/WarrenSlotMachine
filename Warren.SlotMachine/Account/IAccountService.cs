using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warren.SlotMachine.Account
{
    public interface IAccountService
    {
        double Deposit(double amount);

        double Withdraw(double amount);

        double GetBalance();
    }
}
