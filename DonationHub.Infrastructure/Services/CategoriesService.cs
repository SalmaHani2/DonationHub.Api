
using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Categories.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                NameAr = category.NameAr!,
                NameEn = category.NameEn!,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category == null)
            throw new KeyNotFoundException("Category.NotFound");

        return MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var currentTime = DateTime.UtcNow;

        var category = Category.Create(
            request.NameAr.Trim(),
            request.NameEn.Trim(),
            request.DescriptionAr?.Trim(),
            request.DescriptionEn?.Trim(),
            currentTime);

        _context.Categories.Add(category);

        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponse(category);
    }

    public async Task<CategoryResponse> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category == null)
            throw new KeyNotFoundException("Category.NotFound");

        category.Update(
            request.NameAr.Trim(),
            request.NameEn.Trim(),
            request.DescriptionAr?.Trim(),
            request.DescriptionEn?.Trim(),
            DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponse(category);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(category => category.Campaigns)
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category == null)
            throw new KeyNotFoundException("Category.NotFound");

        category.EnsureCanBeDeleted();

        category.Deleted(DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr!,
            NameEn = category.NameEn!,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        };
    }
}
