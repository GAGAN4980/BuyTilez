using System.Collections.ObjectModel;

namespace BuyTilez.Utilities
{
    public static class Constants
    {
        public const string ImagePath = @"\images\product\";

        public const string ShoppingCartSession = "ShoppingCartSession";

        public const string OrderSessionId = "OrderSession";

        public const string AdminRole = "Admin";

        public const string CustomerRole = "Customer";

        public const string AdminEmail = "julianzapatag7@gmail.com";

        public const string CategoryName = "Category";

        public const string ApplicationTypeName = "ApplicationType";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusProcessing = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCanceled = "Canceled";
        public const string StatusReturned = "Returned";

        public static readonly IEnumerable<string> StatusList = new ReadOnlyCollection<string>(
            new List<string>
            {
                StatusPending,
                StatusApproved,
                StatusProcessing,
                StatusShipped,
                StatusCanceled,
                StatusReturned
            }
        );
    }
}
