/*
 * File: ProductsController.cs
 * Description: This file defines the ProductsController for managing product-related API requests
 *              in the UrbanMart application. It handles operations such as retrieving, creating, 
 *              updating, and deleting products.
 * Author: Imesh Vitharana
 * Date: 15/09/24
 * 
 * The ProductsController interacts with the ProductsService to perform CRUD operations on the 
 * product catalog and provides HTTP endpoints for external clients to access the product data.
 */
using urbanmart.Models;
using urbanmart.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productService;

        public ProductsController(ProductsService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        // Retrieves all products.
        [HttpGet]
        public ActionResult<List<Product>> Get() => _productService.Get();

        // GET: api/Products/{id}
        /// Retrieves a product by its unique ID.
        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public ActionResult<Product> Get(string id)
        {
            var product = _productService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        // POST: api/Products
        [HttpPost]
        public ActionResult<Product> Create(Product product)
        {
            _productService.Create(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        // PUT: api/Products/{id}
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Product productIn)
        {
            var product = _productService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            _productService.Update(id, productIn);
            return NoContent();
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var product = _productService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            _productService.Delete(id);
            return NoContent();
        }
    }
}
