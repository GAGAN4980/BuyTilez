using Braintree;

namespace BuyTilez.Utilities.BrainTree
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();

    }
}
