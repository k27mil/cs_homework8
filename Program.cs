using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_homework8
{
    public enum AccountType
    {
        Current,
        Savings
    }

    public class BankAccount
    {
        private static int _nextAccountNumber = 1;
        private readonly int _accountNumber;
        private decimal _balance;
        private AccountType _accountType;

        public BankAccount()
        {
            _accountNumber = GenerateAccountNumber();
        }

        public BankAccount(decimal balance)
        {
            _accountNumber = GenerateAccountNumber();
            _balance = balance;
        }

        public BankAccount(AccountType accountType)
        {
            _accountNumber = GenerateAccountNumber();
            _accountType = accountType;
        }

        public BankAccount(decimal balance, AccountType accountType)
        {
            _accountNumber = GenerateAccountNumber();
            _balance = balance;
            _accountType = accountType;
        }

        private static int GenerateAccountNumber()
        {
            return _nextAccountNumber++;
        }

        public bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            _balance += amount;
            return true;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0 || amount > _balance)
            {
                return false;
            }

            _balance -= amount;
            return true;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Счет {_accountNumber}  Тип: {_accountType}  Баланс: {_balance}");
        }
    }

    internal class Program
    {
        private static void Main()
        {
            BankAccount acc1 = new BankAccount();
            BankAccount acc2 = new BankAccount(5000m);
            BankAccount acc3 = new BankAccount(AccountType.Savings);
            BankAccount acc4 = new BankAccount(12000m, AccountType.Current);

            acc1.PrintInfo();
            acc2.PrintInfo();
            acc3.PrintInfo();
            acc4.PrintInfo();
        }
    }
}
