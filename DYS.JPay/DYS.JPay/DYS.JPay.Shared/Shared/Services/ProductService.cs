using DYS.JPay.Common.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<PageDto<Product>> GetProductsAsync(SearchDto search);
    }
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<List<Product>> GetProductsAsync() => _productRepository.GetAllAsync();
        public Task<PageDto<Product>> GetProductsAsync(SearchDto search)  =>
             _productRepository.GetPagedAsync(search.CurrentPage, 
                 search.PageSize, 
                 search.Keyword, 
                 search.Columns);
    }

}
