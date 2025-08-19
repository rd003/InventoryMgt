using System.Text.Json;
using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Data.models.DTOs;
using InventoryMgt.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;

[ApiController]
[Route("/api/suppliers")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepo;

    public SupplierController(ISupplierRepository supplierRepo)
    {
        _supplierRepo = supplierRepo;
    }

    [HttpPost]
    public async Task<IActionResult> AddSupplier(SupplierDto supplier)
    {
        var createdSupplier = await _supplierRepo.AddSupplierAsync(supplier);
        return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.Id }, createdSupplier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierDto supplier)
    {
        if (id != supplier.Id)
            throw new BadRequestException("id in uri and body does not match");

        var existingSupplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        if (existingSupplier == null)
            throw new NotFoundException($"Supplier with id : {id} does not found");

        await _supplierRepo.UpdateSupplierAsync(supplier);

        // Get updated supplier to return
        var updatedSupplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        return Ok(updatedSupplier);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var existingSupplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        if (existingSupplier == null)
            throw new NotFoundException($"Supplier with id : {id} does not found");

        await _supplierRepo.DeleteSupplierAsnc(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplier(int id)
    {
        var supplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        if (supplier == null)
            throw new NotFoundException($"Supplier with id : {id} does not found");

        return Ok(supplier);
    }

    [HttpGet()]
    public async Task<IActionResult> GetSuppliers(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null)
    {
        var supplierResult = await _supplierRepo.GetSuppliersAsnc(page, limit, searchTerm, sortColumn, sortDirection);
        var suppliers = supplierResult.Suppliers;

        var paginationHeader = new
        {
            supplierResult.TotalRecords,
            supplierResult.TotalPages,
            supplierResult.Page,
            supplierResult.Limit,
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
        return Ok(suppliers);
    }
}