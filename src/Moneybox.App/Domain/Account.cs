using System;
using Moneybox.App.Domain.Services;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public const decimal FundsLowAmount = 500m;

        public const decimal PayInWarningAmount = 500m;

        public Account(Guid id, User user, decimal balance, decimal withdrawn, decimal paidIn)
        {
            this.Id = id;
            this.User = user;
            this.Balance = balance;
            this.Withdrawn = withdrawn;
            this.PaidIn = paidIn;

        }

        public Guid Id { get; private set; }

        public User User { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public void Withdraw(decimal amount, INotificationService notificationService)
        {
            var accountBalance = Balance - amount;
            if (accountBalance < FundsLowAmount)
            {
                notificationService.NotifyFundsLow(User.Email);
            }
            Balance = accountBalance;
            Withdrawn = Withdrawn - amount;
        }

        public void CheckSufficientBalance(decimal amount)
        {
            var accountBalance = Balance - amount;
            if (accountBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make a withdraw");
            }
        }

        public void PayIn(decimal amount, INotificationService notificationService)
        {
            var paidIn = Balance + amount;

            if (Account.PayInLimit - paidIn < PayInWarningAmount)
            {
                notificationService.NotifyApproachingPayInLimit(User.Email);
            }

            Balance = Balance + amount;
            PaidIn = PaidIn + amount;
        }

        public void CheckIfLimitReached(decimal amount)
        {
            var paidIn = Balance + amount;
            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
        }
    }
}
