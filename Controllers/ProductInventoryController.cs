using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using urbanmart.Models;
using urbanmart.Services;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInventoryController : ControllerBase
    {
        private readonly ProductInventoryService _productInventoryService;

        public ProductInventoryController(ProductInventoryService productInventoryService)
        {
            _productInventoryService = productInventoryService;
        }

        // GET: api/ProductInventory
        [HttpGet]
        public ActionResult<List<ProductInventory>> Get() => _productInventoryService.Get();

       // GET: api/ProductInventory/{id}
[HttpGet("{id:length(24)}", Name = "GetProductInventory")]
public ActionResult<ProductInventory> Get(string id)
{
    var product = _productInventoryService.Get(id);
    if (product == null)
    {
        return NotFound();
    }
    return product;
}


        // POST: api/ProductInventory
        [HttpPost]
        public ActionResult<ProductInventory> Create(ProductInventory product)
        {
            _productInventoryService.Create(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        // PUT: api/ProductInventory/{id}
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, ProductInventory productIn)
        {
            var product = _productInventoryService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            _productInventoryService.Update(id, productIn);
            return NoContent();
        }

        // PUT: api/ProductInventory/{id}/inventory
        [HttpPut("{id:length(24)}/inventory")]
        public IActionResult UpdateInventory(string id, [FromBody] int quantity)
        {
            var product = _productInventoryService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            _productInventoryService.UpdateInventory(id, quantity);
            return NoContent();
        }

        // GET: api/ProductInventory/low-stock/{reorderLevel}
        [HttpGet("low-stock/{reorderLevel}")]
        public ActionResult<List<ProductInventory>> GetLowStock(int reorderLevel)
        {
            var lowStockItems = _productInventoryService.GetLowStockItems(reorderLevel);
            if (lowStockItems.Count == 0)
            {
                return NoContent();
            }
            return lowStockItems;
        }

        // DELETE: api/ProductInventory/{id}
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var product = _productInventoryService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
            _productInventoryService.Delete(id);
            return NoContent();
        }
    }
}
