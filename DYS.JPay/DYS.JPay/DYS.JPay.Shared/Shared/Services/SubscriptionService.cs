using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using Plugin.InAppBilling;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface ISubscriptionService: IBaseService
    {
        Task<InAppBillingPurchase> PurchaseSubscriptionAsync(string subscriptionId);
        Task<bool> IsSubscriptionActiveAsync();
        Task<IEnumerable<InAppBillingPurchase>> GetAllSubscriptionsAsync();
        Task<IEnumerable<InAppBillingProduct>> GetAllProductPlansAsync();
        Task<List<Subscription>> GetSubscriptionPlansAsync();
        Task<Subscription> GetSubscriptionPlanByIdAsync(Guid? id);
        Task<InAppBillingPurchase> GetActiveSubscriptionAsync();
    }
    public class SubscriptionService: BaseService, ISubscriptionService
    {
        private readonly IRepository<Subscription> _subscriptionRepository;
        public SubscriptionService(IRepository<Subscription> subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        private const string SubscriptionId = "monthly_subscription_id";
        // Replace with your product ID from App Store / Play Console

        /// <summary>
        /// Initiates a subscription purchase.
        /// </summary>
        public async Task<InAppBillingPurchase> PurchaseSubscriptionAsync(string subscriptionId)
        {
            try
            {
                var billing = CrossInAppBilling.Current;

                // Connect to billing service
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return null;

                // Purchase subscription
                var purchase = await billing.PurchaseAsync(subscriptionId, ItemType.Subscription);

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
        public async Task<InAppBillingPurchase> GetActiveSubscriptionAsync()
        {
            try
            {
                
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return new InAppBillingPurchase();

                var purchases = await billing.GetPurchasesAsync(ItemType.Subscription);

                await billing.DisconnectAsync();
                var purchase = purchases?.OrderByDescending(x => x.TransactionDateUtc)?
                    .FirstOrDefault(query => query.State == PurchaseState.Purchased);

                return purchase ?? new InAppBillingPurchase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Check failed: {ex.Message}");
                return new InAppBillingPurchase();
            }
        }
        public async Task<IEnumerable<InAppBillingPurchase>> GetAllSubscriptionsAsync()
        {
            try
            {
                var billing = CrossInAppBilling.Current;

                // Connect to the billing service
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return null;

                // Retrieve all active subscriptions
                var purchases = await billing.GetPurchasesAsync(ItemType.Subscription);
                return purchases;
            }
            catch (InAppBillingPurchaseException ex)
            {
                // Handle billing errors (network, invalid state, etc.)
                Console.WriteLine($"Billing error: {ex.Message}");
                return null;
            }
            finally
            {
                // Disconnect when done
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
        public async Task<IEnumerable<InAppBillingProduct>> GetAllProductPlansAsync()
        {
            try
            {
                
                var billing = CrossInAppBilling.Current;

                // Connect to the billing service
                var connected = await billing.ConnectAsync();
                if (!connected) return null;

                var productIds = new[] { "jpay_monthly_basic", "jpay_monthly_growth" };
                var products = await billing.GetProductInfoAsync(ItemType.Subscription, productIds);
                return products;
            }
            catch (InAppBillingPurchaseException ex)
            {
                // Handle billing errors (network, invalid state, etc.)
                Console.WriteLine($"Billing error: {ex?.Message}");
            }
            finally
            {
                // Disconnect when done
                await CrossInAppBilling.Current.DisconnectAsync();
            }
            return null;

        }
        public async Task<List<Subscription>> GetSubscriptionPlansAsync()
        {
            return await _subscriptionRepository.GetAllAsync();
        }
        public async Task<Subscription> GetSubscriptionPlanByIdAsync(Guid? id)
        {
            return await _subscriptionRepository.GetAsync(query => query.Id == id);
        }
    }
}
