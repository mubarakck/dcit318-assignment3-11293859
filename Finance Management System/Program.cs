namespace Finance_Management_System
{
    


        // 1a. Define Transaction record
        public record Transaction(
            int Id, 
            DateTime Date, 
            decimal Amount, 
            string Category
            );

        // 1b. Define ITransactionProcessor interface
        public interface ITransactionProcessor
        {
            void Process(Transaction transaction);
        }

        // 1c. Implement concrete processor classes
        public class BankTransferProcessor : ITransactionProcessor
        {
            public void Process(Transaction transaction)
            {
                Console.WriteLine($"Processing Bank Transfer: ${transaction.Amount} for {transaction.Category}");
            }
        }

        public class MobileMoneyProcessor : ITransactionProcessor
        {
            public void Process(Transaction transaction)
            {
                Console.WriteLine($"Processing Mobile Money: ${transaction.Amount} for {transaction.Category}");
            }
        }

        public class CryptoWalletProcessor : ITransactionProcessor
        {
            public void Process(Transaction transaction)
            {
                Console.WriteLine($"Processing Crypto Transaction: ${transaction.Amount} for {transaction.Category}");
            }
        }

        // 1d. Define base Account class
        public class Account
        {
            public string AccountNumber { get; }
            public decimal Balance { get; protected set; }

            public Account(string accountNumber, decimal initialBalance)
            {
                AccountNumber = accountNumber;
                Balance = initialBalance;
            }

            public virtual void ApplyTransaction(Transaction transaction)
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Applied transaction: -${transaction.Amount}. New balance: ${Balance}");
            }
        }

        // 1e. Define sealed SavingsAccount class
        public sealed class SavingsAccount : Account
        {
            public SavingsAccount(string accountNumber, decimal initialBalance)
                : base(accountNumber, initialBalance)
            {
            }

            public override void ApplyTransaction(Transaction transaction)
            {
                if (transaction.Amount > Balance)
                {
                    Console.WriteLine("Insufficient funds");
                }
                else
                {
                    base.ApplyTransaction(transaction);
                }
            }
        }

        // 1f. FinanceApp class to integrate everything
        public class FinanceApp
        {
            private List<Transaction> _transactions = new List<Transaction>();

            public void Run()
            {
                // i. Instantiate SavingsAccount
                var savingsAccount = new SavingsAccount("MUBARAK001", 1000m);
                Console.WriteLine($"Initial balance: ${savingsAccount.Balance}");

                // ii. Create sample transactions
                var transaction1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
                var transaction2 = new Transaction(2, DateTime.Now, 75m, "Utilities");
                var transaction3 = new Transaction(3, DateTime.Now, 200m, "Entertainment");

                // iii. Process transactions with different processors
                var mobileProcessor = new MobileMoneyProcessor();
                var bankProcessor = new BankTransferProcessor();
                var cryptoProcessor = new CryptoWalletProcessor();

                mobileProcessor.Process(transaction1);
                bankProcessor.Process(transaction2);
                cryptoProcessor.Process(transaction3);

                // iv. Apply transactions to account
                Console.WriteLine("\nApplying transactions to account:");
                savingsAccount.ApplyTransaction(transaction1);
                savingsAccount.ApplyTransaction(transaction2);
                savingsAccount.ApplyTransaction(transaction3);

                // v. Add transactions to list
                _transactions.Add(transaction1);
                _transactions.Add(transaction2);
                _transactions.Add(transaction3);

                Console.WriteLine("\nTransaction History:");
                foreach (var transaction in _transactions)
                {
                    Console.WriteLine($"ID: {transaction.Id}, Amount: ${transaction.Amount}, Category: {transaction.Category}, Date: {transaction.Date}");
                }
            }
        }

        // Main program
        class Program
        {
            static void Main(string[] args)
            {
                var financeApp = new FinanceApp();
                financeApp.Run();
            }
        }
    }
