namespace Practical.DistributedTransactions
{
    public class PurchaseTransaction : DistributedTransaction
    {

    }

    public class PurchaseTransactionHandler : IDistributedTransactionHandler<PurchaseTransaction>
    {
        public const string ORDER_CREATED = "ORDER_CREATED";
        public const string ORDER_CANCELED = "ORDER_CANCELED";
        public const string PAYMENT_CHARGING = "PAYMENT_CHARGING";
        public const string PAYMENT_CHARGED_SUCCEDED = "PAYMENT_CHARGED_SUCCEDED";
        public const string PAYMENT_CHARGED_FAILED = "PAYMENT_CHARGED_FAILED";
        public const string PAYMENT_RETURNING = "PAYMENT_RETURNING";
        public const string PAYMENT_RETURNED_SUCCEDED = "PAYMENT_RETURNED_SUCCEDED";
        public const string PAYMENT_RETURNED_FAILED = "PAYMENT_RETURNED_FAILED";
        public const string SHIPPING_BOOKING = "SHIPPING_BOOKING";
        public const string SHIPPING_BOOKED_SUCCEDED = "SHIPPING_BOOKED_SUCCEDED";
        public const string SHIPPING_BOOKED_FAILED = "SHIPPING_BOOKED_FAILED";
        public const string SHIPPING_CANCELING = "SHIPPING_CANCELING";
        public const string SHIPPING_CANCELED_SUCCEDED = "SHIPPING_CANCELED_SUCCEDED";
        public const string SHIPPING_CANCELED_FAILED = "SHIPPING_CANCELED_FAILED";

        public void Handle(PurchaseTransaction transaction)
        {
            switch (transaction.State)
            {
                case ORDER_CREATED:
                    HandleMoney(transaction);
                    break;
                case PAYMENT_CHARGED_SUCCEDED:
                    HandleShipping(transaction);
                    break;
                case SHIPPING_BOOKED_SUCCEDED:
                    transaction.Completed = true;
                    break;
            }
        }

        public void Compensate(PurchaseTransaction transaction)
        {
            switch (transaction.State)
            {
                case ORDER_CANCELED:
                    CompensateShipping(transaction);
                    break;
                case SHIPPING_CANCELED_SUCCEDED:
                    CompensateMoney(transaction);
                    break;
                case PAYMENT_RETURNED_SUCCEDED:
                    transaction.Canceled = true;
                    break;
            }
        }

        private void HandleMoney(PurchaseTransaction transaction)
        {
            transaction.State = PAYMENT_CHARGING;

            // TODO: call payment service
            var paymentRs = true;

            if (paymentRs)
            {
                transaction.State = PAYMENT_CHARGED_SUCCEDED;
                Handle(transaction);
            }
            else
            {
                transaction.State = PAYMENT_CHARGED_FAILED;
                // stop or trigger reverting here
            }
        }

        private void CompensateMoney(PurchaseTransaction transaction)
        {
            transaction.State = PAYMENT_RETURNING;

            // TODO: call payment service
            var paymentRs = true;

            if (paymentRs)
            {
                transaction.State = PAYMENT_RETURNED_SUCCEDED;
                Compensate(transaction);
            }
            else
            {
                transaction.State = PAYMENT_RETURNED_FAILED;
                // stop or continue reverting here
            }
        }

        private void HandleShipping(PurchaseTransaction transaction)
        {
            transaction.State = SHIPPING_BOOKING;

            // TODO: call booking service
            var shippingRs = true;

            if(shippingRs)
            {
                transaction.State = SHIPPING_BOOKED_SUCCEDED;
                Handle(transaction);
            }
            else
            {
                transaction.State = SHIPPING_BOOKED_FAILED;
                // stop or trigger reverting here
            }
        }

        private void CompensateShipping(PurchaseTransaction transaction)
        {
            transaction.State = SHIPPING_CANCELING;

            // TODO: call booking service
            var shippingRs = true;

            if (shippingRs)
            {
                transaction.State = SHIPPING_CANCELED_SUCCEDED;
                Compensate(transaction);
            }
            else
            {
                transaction.State = SHIPPING_CANCELED_FAILED;
                // stop or continue reverting here
            }
        }
    }
}
