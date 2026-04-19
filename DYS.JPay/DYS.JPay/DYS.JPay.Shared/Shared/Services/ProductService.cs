using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
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
        Task<(Product, List<Variant>)> GetProductWithVariantsByIdAsync(Guid id);
        Task<PageDto<Product>> GetProductsAsync(SearchDto search);
        Task<Product> SubmitProductAsync(ProductDto product);
        Task<Product> SubmitProductWithVariantsAsync(ProductDto product, List<VariantDto> variants);
    }
    public class ProductService : BaseService,IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Variant> _variantRepository;
        public ProductService(IRepository<Product> productRepository, IRepository<Variant> variantRepository)
        {
            _productRepository = productRepository;
            _variantRepository = variantRepository;
        }

        public async Task<List<Product>> GetProductsAsync() => await _productRepository.GetAllAsync();
        public async Task<Product> GetProductByIdAsync(Guid id) => await _productRepository.GetAsync(query => query.Id == id);
        public async Task<(Product, List<Variant>)> GetProductWithVariantsByIdAsync(Guid id) {
            var product = await _productRepository.GetAsync(query => query.Id == id);
            var variants = await _variantRepository.GetAllAsync(query => query.ProductId == id);
            return (product, variants);
        }
        public async Task<PageDto<Product>> GetProductsAsync(SearchDto search)  =>
             await _productRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns);

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
                await _productRepository.UpdateAsync(item);
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
