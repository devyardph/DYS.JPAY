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
        Task<PageDto<Product>> GetProductsAsync(SearchDto search);
        Task<Product> SubmitProductAsync(ProductDto product);
    }
    public class ProductService : BaseService,IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetProductsAsync() => await _productRepository.GetAllAsync();
        public async Task<Product> GetProductByIdAsync(Guid id) => await _productRepository.GetAsync(query => query.Id == id);
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
    }

}
