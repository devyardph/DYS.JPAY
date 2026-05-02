using Plugin.InAppBilling;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ISubscriptionService: IBaseService
    {
        Task<InAppBillingPurchase> PurchaseSubscriptionAsync();
        Task<bool> IsSubscriptionActiveAsync();
    }
    public class SubscriptionService: BaseService, ISubscriptionService
    {
        private const string SubscriptionId = "monthly_subscription_id";
        // Replace with your product ID from App Store / Play Console

        /// <summary>
        /// Initiates a subscription purchase.
        /// </summary>
        public async Task<InAppBillingPurchase> PurchaseSubscriptionAsync()
        {
            try
            {
                var billing = CrossInAppBilling.Current;

                // Connect to billing service
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return null;

                // Purchase subscription
                var purchase = await billing.PurchaseAsync(SubscriptionId, ItemType.Subscription);

                // Disconnect
                await billing.DisconnectAsync();

                return purchase;
            }
            catch (Exception ex)
            {
                // Handle errors (network, user cancel, etc.)
                Console.WriteLine($"Purchase failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if the subscription is still active.
        /// </summary>
        public async Task<bool> IsSubscriptionActiveAsync()
        {
            try
            {
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return false;

                var purchases = await billing.GetPurchasesAsync(ItemType.Subscription);

                await billing.DisconnectAsync();

                // Check if subscription exists and is valid
                foreach (var p in purchases)
                {
                    if (p.ProductId == SubscriptionId && p.State == PurchaseState.Purchased)
                    {
                        // Optional: validate with backend server here
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Check failed: {ex.Message}");
                return false;
            }
        }
    }
}
