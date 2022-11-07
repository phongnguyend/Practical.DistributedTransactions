namespace Practical.DistributedTransactions
{
    public class DistributedTransaction
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public bool Completed { get; set; }

        public bool Canceled { get; set; }
    }

    public interface IDistributedTransactionHandler<T> where T : DistributedTransaction
    {
        void Handle(T transaction);

        void Compensate(T transaction);
    }
}
