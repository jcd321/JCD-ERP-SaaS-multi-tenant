using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfMeasureRepository _unitRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IUnitOfMeasureRepository unitRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitRepository = unitRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure("Product.NotFound");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null || !category.IsActive)
            return Result.Failure("Product.CategoryNotFound");

        var unit = await _unitRepository.GetByIdAsync(request.UnitId, cancellationToken);
        if (unit is null || !unit.IsActive)
            return Result.Failure("Product.UnitNotFound");

        if (request.BrandId.HasValue)
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value, cancellationToken);
            if (brand is null || !brand.IsActive)
                return Result.Failure("Product.BrandNotFound");
        }

        var existing = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
            return Result.Failure("Product.SkuAlreadyExists");

        var updateResult = product.Update(
            request.Sku,
            request.Name,
            request.CategoryId,
            request.UnitId,
            request.Description,
            request.BrandId,
            request.IsActive);

        if (updateResult.IsFailure)
            return updateResult;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
