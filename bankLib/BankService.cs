namespace bankLib
{
    public class BankService
    {
        public Accounts? CustomerLogin(string username, string password)
        {
            using (BankDbContext db = new BankDbContext())
            {
                Accounts? account = db.Accounts.FirstOrDefault(a => a.Username == username);

                if (account != null && password == account.Password)
                {
                    return account;
                }

                return null;
            }
        }

        public bool AdminLogin(string username, string password)
        {
            using (BankDbContext db = new BankDbContext())
            {
                Admin? admin = db.Admins.FirstOrDefault(a => a.Username == username);

                if (admin != null && password == admin.Password)
                {
                    return true;
                }

                return false;
            }
        }

        private Accounts FindAccount(BankDbContext db, int accNo)
        {
            Accounts? account = db.Accounts.FirstOrDefault(a => a.AccNo == accNo);

            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            return account;
        }

        public Accounts GetAccountDetails(int accNo)
        {
            using (BankDbContext db = new BankDbContext())
            {
                return FindAccount(db, accNo);
            }
        }

        private void AddTransaction(BankDbContext db, Accounts account, string type,
            double amount, int? relatedAccNo)
        {
            BankTransaction transaction = new BankTransaction()
            {
                AccNo = account.AccNo,
                TransactionType = type,
                Amount = amount,
                BalanceAfter = account.AccBalance,
                RelatedAccNo = relatedAccNo,
                TransactionDate = DateTime.UtcNow
            };

            db.BankTransactions.Add(transaction);
        }

        public double Withdraw(int accNo, double amount)
        {
            using (BankDbContext db = new BankDbContext())
            {
                Accounts account = FindAccount(db, accNo);
                account.Withdraw(amount);
                AddTransaction(db, account, "Withdraw", amount, null);
                db.SaveChanges();

                return account.AccBalance;
            }
        }

        public double Deposit(int accNo, double amount)
        {
            using (BankDbContext db = new BankDbContext())
            {
                Accounts account = FindAccount(db, accNo);
                account.Deposit(amount);
                AddTransaction(db, account, "Deposit", amount, null);
                db.SaveChanges();

                return account.AccBalance;
            }
        }

        public double Transfer(int accNo, int receiverAccNo, double amount)
        {
            if (accNo == receiverAccNo)
            {
                throw new Exception("You cannot transfer to the same account.");
            }

            using (BankDbContext db = new BankDbContext())
            {
                Accounts sender = FindAccount(db, accNo);
                Accounts receiver = FindAccount(db, receiverAccNo);
                sender.Withdraw(amount);
                receiver.Deposit(amount);
                AddTransaction(db, sender, "Transfer Out", amount, receiverAccNo);
                AddTransaction(db, receiver, "Transfer In", amount, accNo);
                db.SaveChanges();

                return sender.AccBalance;
            }
        }

        public List<BankTransaction> GetLastFiveTransactions(int accNo)
        {
            using (BankDbContext db = new BankDbContext())
            {
                FindAccount(db, accNo);
                var transactions = from t in db.BankTransactions
                                   where t.AccNo == accNo
                                   orderby t.TransactionDate descending, t.TransactionId descending
                                   select t;
                List<BankTransaction> lastFive = new List<BankTransaction>();

                foreach (BankTransaction transaction in transactions)
                {
                    lastFive.Add(transaction);

                    if (lastFive.Count == 5)
                    {
                        break;
                    }
                }

                return lastFive;
            }
        }

        public void RequestChequeBook(int accNo)
        {
            using (BankDbContext db = new BankDbContext())
            {
                FindAccount(db, accNo);
                ChequeRequest request = new ChequeRequest()
                {
                    AccNo = accNo,
                    RequestDate = DateTime.UtcNow
                };

                db.ChequeRequests.Add(request);
                db.SaveChanges();
            }
        }

        public void ChangePassword(int accNo, string newPassword)
        {
            CheckPassword(newPassword);

            using (BankDbContext db = new BankDbContext())
            {
                Accounts account = FindAccount(db, accNo);
                account.Password = newPassword;
                db.SaveChanges();
            }
        }

        private void CheckPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length > 200)
            {
                throw new Exception("Password must contain 1 to 200 characters.");
            }
        }

        private void CheckDetails(string accName, string accType)
        {
            if (string.IsNullOrWhiteSpace(accName) || accName.Length > 100)
            {
                throw new Exception("Account name must contain 1 to 100 characters.");
            }

            if (accType != "Savings" && accType != "Checking")
            {
                throw new Exception("Account type must be Savings or Checking.");
            }
        }

        public void CreateAccount(int accNo, string accName, string accType,
            string username, string password, double openingBalance)
        {
            CheckDetails(accName, accType);
            CheckPassword(password);

            if (accNo <= 0)
            {
                throw new Exception("Account number must be positive.");
            }

            if (string.IsNullOrWhiteSpace(username) || username.Length > 50)
            {
                throw new Exception("Username must contain 1 to 50 characters.");
            }

            username = username.Trim();

            if (double.IsNaN(openingBalance) || double.IsInfinity(openingBalance) || openingBalance < 0)
            {
                throw new Exception("Please enter a valid opening balance of zero or more.");
            }

            using (BankDbContext db = new BankDbContext())
            {
                var existingAccount = db.Accounts.FirstOrDefault(a => a.AccNo == accNo || a.Username == username);

                if (existingAccount != null)
                {
                    throw new Exception("Account number or username already exists.");
                }

                Accounts account = new Accounts()
                {
                    AccNo = accNo,
                    AccName = accName.Trim(),
                    AccType = accType,
                    Username = username,
                    Password = password
                };

                db.Accounts.Add(account);

                if (openingBalance > 0)
                {
                    account.Deposit(openingBalance);
                    AddTransaction(db, account, "Opening Deposit", openingBalance, null);
                }

                db.SaveChanges();
            }
        }

        public void DeleteAccount(int accNo)
        {
            using (BankDbContext db = new BankDbContext())
            {
                Accounts account = FindAccount(db, accNo);
                var requests = from r in db.ChequeRequests
                               where r.AccNo == accNo
                               select r;

                foreach (ChequeRequest request in requests)
                {
                    db.ChequeRequests.Remove(request);
                }

                var transactions = from t in db.BankTransactions
                                   where t.AccNo == accNo
                                   select t;

                foreach (BankTransaction transaction in transactions)
                {
                    db.BankTransactions.Remove(transaction);
                }

                var relatedTransactions = from t in db.BankTransactions
                                          where t.RelatedAccNo == accNo && t.AccNo != accNo
                                          select t;

                foreach (BankTransaction transaction in relatedTransactions)
                {
                    transaction.RelatedAccNo = null;
                }

                db.Accounts.Remove(account);
                db.SaveChanges();
            }
        }

        public void EditAccountDetails(int accNo, string accName, string accType)
        {
            CheckDetails(accName, accType);

            using (BankDbContext db = new BankDbContext())
            {
                Accounts account = FindAccount(db, accNo);
                account.AccName = accName.Trim();
                account.AccType = accType;
                db.SaveChanges();
            }
        }

        public List<Accounts> GetAccounts()
        {
            using (BankDbContext db = new BankDbContext())
            {
                var accounts = from a in db.Accounts
                               orderby a.AccNo
                               select a;
                List<Accounts> accountList = new List<Accounts>();

                foreach (Accounts account in accounts)
                {
                    accountList.Add(account);
                }

                return accountList;
            }
        }

        public void ResetCustomerPassword(int accNo, string newPassword)
        {
            ChangePassword(accNo, newPassword);
        }

        public List<ChequeRequest> GetPendingRequests()
        {
            using (BankDbContext db = new BankDbContext())
            {
                var requests = from r in db.ChequeRequests
                               where r.Status == "Pending"
                               orderby r.RequestId
                               select r;
                List<ChequeRequest> requestList = new List<ChequeRequest>();

                foreach (ChequeRequest request in requests)
                {
                    requestList.Add(request);
                }

                return requestList;
            }
        }

        public void ApproveChequeRequest(int requestId)
        {
            using (BankDbContext db = new BankDbContext())
            {
                ChequeRequest? request = db.ChequeRequests
                    .FirstOrDefault(r => r.RequestId == requestId && r.Status == "Pending");

                if (request == null)
                {
                    throw new Exception("Pending request not found.");
                }

                FindAccount(db, request.AccNo);
                request.Status = "Approved";
                request.ApprovalDate = DateTime.UtcNow;
                db.SaveChanges();
            }
        }
    }
}
