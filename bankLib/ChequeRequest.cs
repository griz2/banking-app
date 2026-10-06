namespace bankLib
{
    public class ChequeRequest
    {
        public int RequestId { get; set; }

        public int AccNo { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime RequestDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
