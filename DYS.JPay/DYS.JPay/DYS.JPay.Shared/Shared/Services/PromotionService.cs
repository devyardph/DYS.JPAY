using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Settings;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IPromotionService : IBaseService
    {
        Task<List<Product>> GetProductsAsync();
        Task<PageDto<Promotion>> GetPromotionsAsync(SearchDto search);
        Task<Promotion> GetPromotionAsync(Expression<Func<Promotion, bool>> predicate = null);
        Task<List<PromotionItem>> GetPromotionItemsByPromotionIdAsync(Guid? promotionId);
        Task<OutputDto<PromotionDto>> SubmitPromotionAsync(PromotionDto promotion);
        Task<OutputDto<List<PromotionItemDto>>> SubmitPromotionItemsAsync(List<PromotionItemDto> promotions);
        Task<int> DeletePromotionAsync(Guid? id);
    }
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Variant> _variantRepository;
        private readonly IRepository<Logger> _loggerRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<PromotionItem> _promotionItemRepository;
        private readonly SessionService _sessionService;
        public PromotionService(
            IRepository<Product> productRepository, 
            IRepository<Variant> variantRepository,
            IRepository<Logger> loggerRepository,
            IRepository<Promotion> promotionRepository,
            IRepository<PromotionItem> promotionItemRepository,
            SessionService sessionService)
        {
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _loggerRepository = loggerRepository;
            _sessionService = sessionService;
            _promotionRepository = promotionRepository;
            _promotionItemRepository = promotionItemRepository;

            _productRepository.EntityChanged += (s, e) =>
            {
                var logger = new Logger
                {
                    Type = GlobalSettings.INFO,
                    Message = $"{e.Action} entity of type {typeof(Product).Name}: {e.Entity.Name}",
                    DateCreated = DateTime.UtcNow,
                    ExecutedBy = _sessionService.CurrentUser?.Name ?? string.Empty
                };
                _loggerRepository.InsertAsync(logger);
            };

            _variantRepository.EntityChanged += (s, e) =>
            {
                var logger = new Logger
                {
                    Type = GlobalSettings.INFO,
                    Message = $"{e.Action} entity of type {typeof(Variant).Name}:{JsonExtensions.Convert(e.Entity)}",
                    DateCreated = DateTime.UtcNow,
                    ExecutedBy = _sessionService.CurrentUser?.Name ?? string.Empty
                };
                _loggerRepository.InsertAsync(logger);
            };
        }

        public async Task<List<Product>> GetProductsAsync() => await _productRepository.GetAllAsync();
        public async Task<PageDto<Promotion>> GetPromotionsAsync(SearchDto search) =>
          await _promotionRepository.GetPagedAsync(search.CurrentPage,
              search.PageSize,
              search.Keyword,
              search.Columns,
              search.SortColumn,
              sortDescending: true,
              showAll: false);
        public async Task<Promotion> GetPromotionAsync(Expression<Func<Promotion, bool>> predicate = null) =>
          await _promotionRepository.GetAsync(predicate);
        public async Task<List<PromotionItem>> GetPromotionItemsByPromotionIdAsync(Guid? promotionId) =>
         await _promotionItemRepository.GetAllAsync(query => query.PromotionId == promotionId);
        public async Task<OutputDto<PromotionDto>> SubmitPromotionAsync(PromotionDto promotion)
        {

            var output = new OutputDto<PromotionDto>();
            try
            {
                // Ensure dates are valid
                if (promotion.StartDate == null || promotion.EndDate == null)
                {
                    output.Success = false;
                    output.Message = "Promotion must have both start and end dates.";
                    return output;
                }

                if (promotion.StartDate > promotion.EndDate)
                {
                    output.Success = false;
                    output.Message = "Start date cannot be after end date.";
                    return output;
                }

                // Check overlap
                var overlaps = await _promotionRepository.GetAllAsync(p => p.Id != promotion.Id && 
                        p.StartDate <= promotion.EndDate &&
                        p.EndDate >= promotion.StartDate);

                if (overlaps.Any())
                {
                    output.Success = false;
                    output.Message = "Promotion dates overlap with an existing promotion.";
                    return output;
                }
            
                var item = promotion.Adapt<Promotion>();
                if (promotion.Id == Guid.Empty ||
                    promotion.Id == null)
                {
                    item.Id = Guid.NewGuid();
                    await _promotionRepository.InsertAsync(item);
                    output.Entity = promotion;
                    output.Success = true;
                    output.Message = "Added promotion.";
                }
                else
                {
                    await _promotionRepository.UpdateAsync(item);
                    output.Entity = promotion;
                    output.Success = true;
                    output.Message = "Updated promotion.";
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                throw;
            }
            return output;
        }
        public async Task<OutputDto<List<PromotionItemDto>>> SubmitPromotionItemsAsync(List<PromotionItemDto> promotions)
        {

            var output = new OutputDto<List<PromotionItemDto>>();
            try
            {
                var items = promotions.Adapt<List<PromotionItem>>();
                foreach (var item in items)
                {
                    var promo = await _promotionItemRepository.GetAsync(query => query.Id == item.Id || 
                    (query.PromotionId == item.PromotionId && 
                     query.ProductId == item.ProductId &&
                     query.VariationId == item.VariationId));
                    if (promo == null)
                    {
                        await _promotionItemRepository.InsertAsync(item);
                    }
                    else await _promotionItemRepository.UpdateAsync(item);
                }
                output.Success = true;
                output.Entity = promotions;
            }
            catch (Exception ex)
            {
                output.Success = true;
                output.Message = ex.Message;
            }
            return output;
        }
        public async Task<int> DeletePromotionAsync(Guid? id)
        {
            var promotion = await _promotionRepository.GetAsync(query => query.Id == id);
            if (promotion != null)
            {
                var promotionItems = await _promotionItemRepository.GetAllAsync(query => query.PromotionId == id);
                foreach (var promotionItem in promotionItems)
                    await _promotionItemRepository.DeleteAsync(promotionItem);
                return await _promotionRepository.DeleteAsync(promotion);
            }
            return 0;
        }
    }

}
