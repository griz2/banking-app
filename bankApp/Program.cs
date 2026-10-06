using bankLib;

BankMenu menu = new BankMenu();
menu.Start();

class BankMenu
{
    private BankService bank = new BankService();
    private bool continueSession = true;

    public void Start()
    {
        while (continueSession)
        {
            Console.WriteLine("!~~~~~~~~~~~~ Welcome to the Banking System! ~~~~~~~~~~~~!");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Admin");
            Console.WriteLine("3. Exit");

            try
            {
                int userChoice = ReadNumber("Please select an option (1-3): ");

                switch (userChoice)
                {
                    case 1:
                        string customerUsername = ReadInput("Please enter username: ").Trim();
                        string customerPassword = ReadInput("Please enter Password: ");
                        
                        Accounts? account = bank.CustomerLogin(customerUsername, customerPassword);

                        if (account != null)
                        {
                            Console.WriteLine("Welcome, " + account.AccName + "!");
                            CustomerMenu(account.AccNo);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Credential");
                        }

                        break;

                    case 2:
                        string adminUsername = ReadInput("Please enter username: ").Trim();
                        string adminPassword = ReadInput("Please enter Password: ");

                        if (bank.AdminLogin(adminUsername, adminPassword))
                        {
                            AdminMenu();
                        }
                        else
                        {
                            Console.WriteLine("Invalid Credential");
                        }

                        break;

                    case 3:
                        continueSession = false;

                        Console.WriteLine("Thank you for banking with us. Goodbye!");

                        break;

                    default:
                        Console.WriteLine("Invalid choice");

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
        }
    }

    private void CustomerMenu(int accNo)
    {
        bool customerSession = true;

        while (customerSession && continueSession)
        {
            Console.WriteLine();
            Console.WriteLine("!~~~~~~~~~~~~ Customer Menu ~~~~~~~~~~~~!");
            Console.WriteLine("1. Check Account Details");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Transfer");
            Console.WriteLine("5. Last 5 transactions");
            Console.WriteLine("6. Request Cheque Book");
            Console.WriteLine("7. Change Password");
            Console.WriteLine("8. Exit");

            try
            {
                int userChoice = ReadNumber("Please select an option (1-8): ");

                switch (userChoice)
                {
                    case 1:
                        Accounts account = bank.GetAccountDetails(accNo);

                        Console.WriteLine("Account Number: " + account.AccNo);
                        Console.WriteLine("Account Name: " + account.AccName);
                        Console.WriteLine("Account Type: " + account.AccType);
                        Console.WriteLine("Username: " + account.Username);
                        Console.WriteLine("Balance: " + account.AccBalance.ToString("F2"));

                        break;

                    case 2:
                        double withdrawAmount = ReadAmount("Enter amount to withdraw: ");
                        double withdrawBalance = bank.Withdraw(accNo, withdrawAmount);

                        Console.WriteLine("Withdrawal successful. Balance: " + withdrawBalance.ToString("F2"));

                        break;

                    case 3:
                        double depositAmount = ReadAmount("Enter amount to deposit: ");
                        double depositBalance = bank.Deposit(accNo, depositAmount);

                        Console.WriteLine("Deposit successful. Balance: " + depositBalance.ToString("F2"));

                        break;

                    case 4:
                        int receiverAccNo = ReadNumber("Enter the receiver's account number: ");
                        double transferAmount = ReadAmount("Enter amount to transfer: ");
                        double transferBalance = bank.Transfer(accNo, receiverAccNo, transferAmount);

                        Console.WriteLine("Transfer successful. Balance: " + transferBalance.ToString("F2"));

                        break;

                    case 5:
                        List<BankTransaction> transactions = bank.GetLastFiveTransactions(accNo);

                        if (transactions.Count == 0)
                        {
                            Console.WriteLine("No transactions found.");
                        }

                        foreach (BankTransaction transaction in transactions)
                        {
                            Console.WriteLine("Transaction: " + transaction.TransactionId);
                            Console.WriteLine("Date (UTC): " + transaction.TransactionDate);
                            Console.WriteLine("Type: " + transaction.TransactionType);
                            Console.WriteLine("Amount: " + transaction.Amount.ToString("F2"));
                            Console.WriteLine("Balance: " + transaction.BalanceAfter.ToString("F2"));

                            if (transaction.RelatedAccNo != null)
                            {
                                Console.WriteLine("Related Account: " + transaction.RelatedAccNo);
                            }

                            Console.WriteLine();
                        }

                        break;

                    case 6:
                        bank.RequestChequeBook(accNo);
                        Console.WriteLine("Cheque book requested. Waiting for admin approval.");

                        break;

                    case 7:
                        string newPassword = ReadInput("Enter new password: ");

                        bank.ChangePassword(accNo, newPassword);

                        Console.WriteLine("Password changed successfully.");

                        break;

                    case 8:
                        customerSession = false;

                        Console.WriteLine("Customer logged out.");

                        break;

                    default:
                        Console.WriteLine("Invalid choice");

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private void AdminMenu()
    {
        bool adminSession = true;

        while (adminSession && continueSession)
        {
            Console.WriteLine();
            Console.WriteLine("!~~~~~~~~~~~~ Admin Menu ~~~~~~~~~~~~!");
            Console.WriteLine("1. Create New Account");
            Console.WriteLine("2. Delete Account");
            Console.WriteLine("3. Edit Account Details");
            Console.WriteLine("4. Display Summary");
            Console.WriteLine("5. Reset Customer Password");
            Console.WriteLine("6. Approve Cheque book request");
            Console.WriteLine("7. Exit");

            try
            {
                int userChoice = ReadNumber("Please select an option (1-7): ");

                switch (userChoice)
                {
                    case 1:
                        int newAccNo = ReadNumber("Enter account number: ");
                        string newAccName = ReadInput("Enter account name: ").Trim();
                        string newAccType = ReadAccountType();
                        string username = ReadInput("Enter customer username: ").Trim();
                        string password = ReadInput("Enter customer password: ");
                        double openingBalance = ReadAmount("Enter opening balance: ");

                        bank.CreateAccount(newAccNo, newAccName, newAccType, username, password, openingBalance);

                        Console.WriteLine("Account created successfully.");

                        break;

                    case 2:
                        int deleteAccNo = ReadNumber("Enter account number to delete: ");

                        bank.DeleteAccount(deleteAccNo);

                        Console.WriteLine("Account deleted successfully.");

                        break;

                    case 3:
                        int editAccNo = ReadNumber("Enter account number to edit: ");
                        string editAccName = ReadInput("Enter new account name: ").Trim();
                        string editAccType = ReadAccountType();

                        bank.EditAccountDetails(editAccNo, editAccName, editAccType);

                        Console.WriteLine("Account details updated successfully.");

                        break;

                    case 4:
                        DisplaySummary();

                        break;

                    case 5:
                        int resetAccNo = ReadNumber("Enter customer account number: ");
                        string resetPassword = ReadInput("Enter new customer password: ");

                        bank.ResetCustomerPassword(resetAccNo, resetPassword);

                        Console.WriteLine("Customer password reset successfully.");

                        break;

                    case 6:
                        List<ChequeRequest> requests = bank.GetPendingRequests();

                        if (requests.Count == 0)
                        {
                            Console.WriteLine("No pending cheque book requests.");

                            break;
                        }

                        foreach (ChequeRequest request in requests)
                        {
                            Console.WriteLine("Request: " + request.RequestId + " | Account: " +
                                request.AccNo + " | Date (UTC): " + request.RequestDate);
                        }

                        int requestId = ReadNumber("Enter request number to approve: ");
                        
                        bank.ApproveChequeRequest(requestId);

                        Console.WriteLine("Cheque book request approved successfully.");

                        break;

                    case 7:
                        adminSession = false;

                        Console.WriteLine("Admin logged out.");

                        break;

                    default:
                        Console.WriteLine("Invalid choice");

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private void DisplaySummary()
    {
        List<Accounts> accounts = bank.GetAccounts();
        double totalBalance = 0;

        foreach (Accounts account in accounts)
        {
            Console.WriteLine("Account: " + account.AccNo + " | Name: " + account.AccName +
                " | Type: " + account.AccType + " | Balance: " + account.AccBalance.ToString("F2"));
            totalBalance += account.AccBalance;
        }

        Console.WriteLine("Total Accounts: " + accounts.Count);
        Console.WriteLine("Total Balance: " + totalBalance.ToString("F2"));
        Console.WriteLine("Pending Cheque Book Requests: " + bank.GetPendingRequests().Count);
    }

    private string ReadInput(string message)
    {
        Console.WriteLine(message);
        string? input = Console.ReadLine();

        if (input == null)
        {
            continueSession = false;
            throw new Exception("Input closed. Goodbye!");
        }

        return input;
    }

    private int ReadNumber(string message)
    {
        string input = ReadInput(message);

        try
        {
            return Convert.ToInt32(input);
        }
        catch (Exception)
        {
            throw new Exception("Invalid choice. Please enter a whole number.");
        }
    }

    private double ReadAmount(string message)
    {
        string input = ReadInput(message);

        try
        {
            return Convert.ToDouble(input);
        }
        catch (Exception)
        {
            throw new Exception("Invalid amount. Please enter a number.");
        }
    }

    private string ReadAccountType()
    {
        int typeChoice = ReadNumber("Choose account type: 1. Savings  2. Checking");

        switch (typeChoice)
        {
            case 1:
                return "Savings";

            case 2:
                return "Checking";

            default:
                throw new Exception("Invalid choice. Choose 1 for Savings or 2 for Checking.");
        }
    }
}
