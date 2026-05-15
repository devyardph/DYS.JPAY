using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Repositories;
using DYS.JPay.Shared.Shared.Settings;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IPromotionService : IBaseService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<(Product product, List<Variant> variants)> GetProductWithVariantsByIdAsync(Guid id);
        Task<PageDto<Product>> GetProductsAsync(SearchDto search);
        Task<Product> SubmitProductAsync(ProductDto product);
        Task<List<Product>> SubmitProductsAsync(List<ProductDto> products);
        Task<Product> SubmitProductWithVariantsAsync(ProductDto product, List<VariantDto> variants);


        Task<PageDto<Promotion>> GetPromotionsAsync(SearchDto search);
        Task<List<PromotionItem>> GetPromotionItemsByPromotionIdAsync(Guid? promotionId);
        Task<OutputDto<PromotionDto>> SubmitPromotionAsync(PromotionDto promotion);
        Task<OutputDto<List<PromotionItemDto>>> SubmitPromotionItemsAsync(List<PromotionItemDto> promotions);
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
        public async Task<Product> GetProductByIdAsync(Guid id) => await _productRepository.GetAsync(query => query.Id == id);
        public async Task<(Product product, List<Variant> variants)> GetProductWithVariantsByIdAsync(Guid id) {
            var product = await _productRepository.GetAsync(query => query.Id == id);
            var variants = await _variantRepository.GetAllAsync(query => query.ProductId == id);
            return (product, variants);
        }
        public async Task<PageDto<Product>> GetProductsAsync(SearchDto search)  =>
             await _productRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns, 
                 showAll: false);
        public async Task<Product> SubmitProductAsync(ProductDto product)
        {
            try
            {
                var item = product.Adapt<Product>();
                if (product.Id == Guid.Empty ||
                    product.Id == null)
                {
                    item.Id = Guid.NewGuid();
                    await _productRepository.InsertAsync(item);

                }
                else
                {
                    await _productRepository.UpdateAsync(item);
                }
                return item;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                throw;
            }
         
        }
        public async Task<List<Product>> SubmitProductsAsync(List<ProductDto> products)
        {
            var items = products.Adapt<List<Product>>();
            await _productRepository.InsertAsync(items);
            return items;
        }
        public async Task<Product> SubmitProductWithVariantsAsync(ProductDto product, List<VariantDto> variants)
        {
            var item = product.Adapt<Product>();
            if (product.Id == Guid.Empty ||
                product.Id == null)
            {
                item.Id = Guid.NewGuid();
                await _productRepository.InsertAsync(item);
                //ADD NEW VARIANTS
                var newVariants = new List<Variant>();
                foreach (var variant in variants)
                {
                    var newVariant = variant.Adapt<Variant>();
                    newVariant.ProductId = item.Id;
                    newVariants.Add(newVariant);
                }
                await _variantRepository.InsertAsync(newVariants);
            }
            else
            {
                var existingVariants = await _variantRepository.GetAllAsync(query => query.ProductId == item.Id);
                foreach (var variant in variants)
                {
                    var existingVariant = existingVariants.FirstOrDefault(query => query.Id == variant.Id);
                    if(existingVariant != null)
                    {
                        // Update existing variant
                        existingVariant = variant.Adapt(existingVariant);
                        await _variantRepository.UpdateAsync(existingVariant);
                    }
                    else
                    {
                        // Insert new variant
                        var newVariant = variant.Adapt<Variant>();
                        newVariant.ProductId = item.Id;
                        await _variantRepository.InsertAsync(newVariant);
                    }   
                }
            }
            return item;
        }



        public async Task<PageDto<Promotion>> GetPromotionsAsync(SearchDto search) =>
          await _promotionRepository.GetPagedAsync(search.CurrentPage,
              search.PageSize,
              search.Keyword,
              search.Columns,
              showAll: false);

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
                    (query.PromotionId == item.PromotionId && query.ProductId == item.ProductId));
                    if (promo == null)
                    {
                        await _promotionItemRepository.InsertAsync(item);
                    }
                    else await _promotionItemRepository.UpdateAsync(promo);
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
    }

}
