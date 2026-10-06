namespace bankLib
{
    public class Accounts
    {
        #region Properties

        public int AccNo { get; set; }

        public string AccName { get; set; } = "";

        public string AccType { get; set; } = "";

        public double AccBalance { get; set; }

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        #endregion

        #region Methods

        public double Withdraw(double amount)
        {
            CheckAmount(amount);

            if (amount > AccBalance)
            {
                throw new Exception("Insufficient balance.");
            }

            AccBalance = Math.Round(AccBalance - amount, 2);

            return AccBalance;
        }

        public double Deposit(double amount)
        {
            CheckAmount(amount);

            if (double.IsInfinity(AccBalance + amount))
            {
                throw new Exception("The account balance would be too large.");
            }

            AccBalance = Math.Round(AccBalance + amount, 2);

            return AccBalance;
        }

        private void CheckAmount(double amount)
        {
            if (double.IsNaN(amount) || double.IsInfinity(amount) || amount <= 0)
            {
                throw new Exception("Please enter a valid positive amount.");
            }

            if (Math.Round(amount, 2) != amount)
            {
                throw new Exception("Amounts can have no more than 2 decimal places.");
            }
        }

        #endregion
    }
}
