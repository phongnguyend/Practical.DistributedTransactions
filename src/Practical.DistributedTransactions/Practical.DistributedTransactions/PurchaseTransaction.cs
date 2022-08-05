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

        public void NextCommitingStep(PurchaseTransaction transaction)
        {
            switch (transaction.State)
            {
                case ORDER_CREATED:
                    ChargeMoney(transaction);
                    break;
                case PAYMENT_CHARGED_SUCCEDED:
                    BookShipping(transaction);
                    break;
                case SHIPPING_BOOKED_SUCCEDED:
                    transaction.Completed = true;
                    break;
            }
        }

        public void NextRevertingStep(PurchaseTransaction transaction)
        {
            switch (transaction.State)
            {
                case ORDER_CANCELED:
                    CancelShipping(transaction);
                    break;
                case SHIPPING_CANCELED_SUCCEDED:
                    ReturnMoney(transaction);
                    break;
                case PAYMENT_RETURNED_SUCCEDED:
                    transaction.Canceled = true;
                    break;
            }
        }

        private void ChargeMoney(PurchaseTransaction transaction)
        {
            transaction.State = PAYMENT_CHARGING;

            // TODO: call payment service
            var paymentRs = true;

            if (paymentRs)
            {
                transaction.State = PAYMENT_CHARGED_SUCCEDED;
                NextCommitingStep(transaction);
            }
            else
            {
                transaction.State = PAYMENT_CHARGED_FAILED;
                // stop or trigger reverting here
            }
        }

        private void ReturnMoney(PurchaseTransaction transaction)
        {
            transaction.State = PAYMENT_RETURNING;

            // TODO: call payment service
            var paymentRs = true;

            if (paymentRs)
            {
                transaction.State = PAYMENT_RETURNED_SUCCEDED;
                NextRevertingStep(transaction);
            }
            else
            {
                transaction.State = PAYMENT_RETURNED_FAILED;
                // stop or continue reverting here
            }
        }

        private void BookShipping(PurchaseTransaction transaction)
        {
            transaction.State = SHIPPING_BOOKING;

            // TODO: call booking service
            var shippingRs = true;

            if(shippingRs)
            {
                transaction.State = SHIPPING_BOOKED_SUCCEDED;
                NextCommitingStep(transaction);
            }
            else
            {
                transaction.State = SHIPPING_BOOKED_FAILED;
                // stop or trigger reverting here
            }
        }

        private void CancelShipping(PurchaseTransaction transaction)
        {
            transaction.State = SHIPPING_CANCELING;

            // TODO: call booking service
            var shippingRs = true;

            if (shippingRs)
            {
                transaction.State = SHIPPING_CANCELED_SUCCEDED;
                NextRevertingStep(transaction);
            }
            else
            {
                transaction.State = SHIPPING_CANCELED_FAILED;
                // stop or continue reverting here
            }
        }
    }
}
