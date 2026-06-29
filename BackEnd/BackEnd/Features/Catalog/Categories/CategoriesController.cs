using BackEnd.Domain.Entities;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Catalog.Categories;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _svc;
    public CategoriesController(CategoryService svc) => _svc = svc;

    [HttpGet]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> List([FromQuery] bool? hienThi)
    {
        var data = await _svc.ListAsync(hienThi);
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Create(CreateCategoryRequest req)
    {
        var dm = await _svc.CreateAsync(req);
        return CreatedAtAction(nameof(List), new { id = dm.MaDanhMuc }, new { dm.MaDanhMuc });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Update(int id, UpdateCategoryRequest req)
    {
        try
        {
            var updated = await _svc.UpdateAsync(id, req);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _svc.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("reorder")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> UpdateOrder([FromBody] List<CategoryOrderRequest> requests)
    {
        await _svc.UpdateOrderAsync(requests);
        return NoContent();
    }

    [HttpGet("products")]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> GetAllProducts()
    {
        var data = await _svc.GetAllProductsAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}/products")]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> GetCategoryProducts(int id)
    {
        var data = await _svc.GetCategoryProductsAsync(id);
        return Ok(data);
    }

    [HttpPost("{id:int}/assign-products")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> AssignProducts(int id, [FromBody] AssignProductsRequest req)
    {
        var success = await _svc.AssignProductsAsync(id, req.ProductIds);
        if (!success) return NotFound(new { message = "Không tìm thấy danh mục." });
        return NoContent();
    }
}
