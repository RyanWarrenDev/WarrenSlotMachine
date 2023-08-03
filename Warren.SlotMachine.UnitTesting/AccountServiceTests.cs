using Microsoft.Extensions.Options;
using Warren.Domain;
using Warren.SlotMachine.Account;

namespace Warren.SlotMachine.UnitTesting
{
    public class AccountServiceTests
    {
        [Theory]
        [InlineData(1000)]
        public void MakeDeposit(double depositAmount)
        {
            var sut = new AccountService();

            sut.Deposit(depositAmount);

            Assert.Equal(depositAmount, sut.GetBalance());
        }

        [Theory]
        [InlineData(1000, 450)]
        public void MakeWithdrawl(double depositAmount, double withdrawlAmount)
        {
            var sut = new AccountService();

            sut.Deposit(depositAmount);

            sut.Withdraw(withdrawlAmount);

            Assert.Equal(depositAmount - withdrawlAmount, sut.GetBalance());
        }
    }
}