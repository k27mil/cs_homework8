using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public class BankTransaction
    {
        public readonly DateTime Date;
        public readonly decimal Amount;

        public BankTransaction(decimal amount)
        {
            Date = DateTime.Now;
            Amount = amount;
        }
    }

    public class BankAccount : IDisposable
    {
        private static int _nextAccountNumber = 1;
        private readonly int _accountNumber;
        private decimal _balance;
        private AccountType _accountType;
        private readonly Queue _transactions = new Queue();
        private bool _disposed;

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
            _transactions.Enqueue(new BankTransaction(amount));
            return true;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0 || amount > _balance)
            {
                return false;
            }

            _balance -= amount;
            _transactions.Enqueue(new BankTransaction(-amount));
            return true;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Счет №{_accountNumber}  Тип: {_accountType}  Баланс: {_balance}");
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            string fileName = $"transactions_{_accountNumber}.txt";
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (BankTransaction transaction in _transactions)
                {
                    writer.WriteLine($"{transaction.Date:yyyy-MM-dd HH:mm:ss} / {transaction.Amount}");
                }
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    internal class Program
    {
        private static void Main()
        {
            using (BankAccount account = new BankAccount(10000m, AccountType.Current))
            {
                account.Deposit(3500m);
                account.Withdraw(2000m);
                account.Deposit(1500m);
                account.PrintInfo();
            }

            Console.WriteLine("Проводки записаны в файл transactions_1.txt.");
        }
    }
}
