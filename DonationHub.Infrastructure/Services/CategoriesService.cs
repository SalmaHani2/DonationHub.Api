using DonationHub.Application.DTOs.Categories;
using DonationHub.Application.Interfaces;
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

    public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            })
            .ToListAsync(cancellationToken);

        return categories;
    }

    public async Task<CategoryResponse> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException("Category.NotFound");
        }

        return new CategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        };
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        };
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id,UpdateCategoryRequest request,CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException("Category.NotFound");
        }

        category.NameAr = request.NameAr;
        category.NameEn = request.NameEn;
        category.DescriptionAr = request.DescriptionAr;
        category.DescriptionEn = request.DescriptionEn;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        };
    }

    public async Task DeleteAsync(Guid id,CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(category => category.Campaigns)
            .FirstOrDefaultAsync(
                category => category.Id == id,
                cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException("Category.NotFound");
        }

        if (category.Campaigns.Any())
        {
            throw new InvalidOperationException(
                "Category.HasCampaigns");
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);
    }
}