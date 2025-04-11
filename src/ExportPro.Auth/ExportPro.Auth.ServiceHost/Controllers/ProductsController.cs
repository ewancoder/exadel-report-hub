using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Auth.ServiceHost.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200.00m, Category = "Electronics", InStock = true },
            new Product { Id = 2, Name = "Smartphone", Price = 800.00m, Category = "Electronics", InStock = true },
            new Product { Id = 3, Name = "Headphones", Price = 150.00m, Category = "Audio", InStock = true },
            new Product { Id = 4, Name = "Monitor", Price = 300.00m, Category = "Electronics", InStock = false },
            new Product { Id = 5, Name = "Keyboard", Price = 80.00m, Category = "Accessories", InStock = true }
        };

    // GET: api/Products
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(_products);
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public bool InStock { get; set; }
}