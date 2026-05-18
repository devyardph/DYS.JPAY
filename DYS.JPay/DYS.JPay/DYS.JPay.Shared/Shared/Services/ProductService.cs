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
    public interface IProductService: IBaseService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<(Product product, List<Variant> variants)> GetProductWithVariantsByIdAsync(Guid id);
        Task<PageDto<Product>> GetProductsAsync(SearchDto search);
        Task<Product> SubmitProductAsync(ProductDto product);
        Task<List<Product>> SubmitProductsAsync(List<ProductDto> products);
        Task<Product> SubmitProductWithVariantsAsync(ProductDto product, List<VariantDto> variants);
    }
    public class ProductService : BaseService,IProductService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Variant> _variantRepository;
        private readonly IRepository<Logger> _loggerRepository;
        private readonly SessionService _sessionService;
        public ProductService(
            IRepository<Category> categoryRepository,
            IRepository<Product> productRepository, 
            IRepository<Variant> variantRepository,
            IRepository<Logger> loggerRepository,
            SessionService sessionService)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _loggerRepository = loggerRepository;
            _sessionService = sessionService;

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
            var categories = products.Select(query => query.Type);
            foreach (var category in categories)
            {
                var c = await _categoryRepository.GetAsync(query =>
                            query.Name!.ToLower() == category!.ToLower());
                if (c == null)
                {
                    await _categoryRepository.InsertAsync(new Category() { Name = category });
                }
            }
            foreach (var p in products)
            {
                var c = await _categoryRepository.GetAsync(query => query.Name.ToLower() == p.Type.ToLower());
                if (c != null) p.CategoryId = c.Id;
            }
            var items = products.Adapt<List<Product>>();
            //ADD OR UPDATE
            foreach (var item in items)
            {
                var product = await _productRepository.GetAsync(query => 
                  (query.Name!.ToLower() == item.Name!.ToLower()) ||
                  (query.Id == item.Id) ||
                  (query.Code!.ToLower()! == item.Code!.ToLower()) 
                );

                if (product == null)
                    await _productRepository.InsertAsync(item);
                else await _productRepository.UpdateAsync(product);
            }
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
    }

}
