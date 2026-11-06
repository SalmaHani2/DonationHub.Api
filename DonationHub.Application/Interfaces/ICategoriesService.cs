using DonationHub.Application.DTOs.Categories;

namespace DonationHub.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync( CancellationToken cancellationToken);

    Task<CategoryResponse> GetByIdAsync(Guid id,CancellationToken cancellationToken);

    Task<CategoryResponse> CreateAsync( CreateCategoryRequest request,CancellationToken cancellationToken);

    Task<CategoryResponse> UpdateAsync(Guid id,UpdateCategoryRequest request,CancellationToken cancellationToken);

    Task DeleteAsync(Guid id,CancellationToken cancellationToken);
}