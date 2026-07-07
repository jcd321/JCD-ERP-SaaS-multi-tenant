using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Brands;
using Jcd.Erp.Domain.Catalog.Categories;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfMeasureRepository _unitRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(
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

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null || !category.IsActive)
            return Result.Failure<Guid>("Product.CategoryNotFound");

        var unit = await _unitRepository.GetByIdAsync(request.UnitId, cancellationToken);
        if (unit is null || !unit.IsActive)
            return Result.Failure<Guid>("Product.UnitNotFound");

        if (request.BrandId.HasValue)
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value, cancellationToken);
            if (brand is null || !brand.IsActive)
                return Result.Failure<Guid>("Product.BrandNotFound");
        }

        if (await _productRepository.GetBySkuAsync(request.Sku, cancellationToken) is not null)
            return Result.Failure<Guid>("Product.SkuAlreadyExists");

        var productResult = Product.Create(
            tenantId,
            request.Sku,
            request.Name,
            request.CategoryId,
            request.UnitId,
            request.Description,
            request.BrandId);

        if (productResult.IsFailure)
            return Result.Failure<Guid>(productResult.Error);

        var product = productResult.Value;
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}
