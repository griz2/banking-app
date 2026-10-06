namespace bankLib
{
    public class BankTransaction
    {
        public int TransactionId { get; set; }

        public int AccNo { get; set; }

        public string TransactionType { get; set; } = "";

        public double Amount { get; set; }

        public double BalanceAfter { get; set; }

        public int? RelatedAccNo { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
