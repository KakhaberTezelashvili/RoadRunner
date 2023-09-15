using MasterDataService.Shared.Dtos.Products;

namespace MasterDataService.API.MapperProfiles;

public class ProductMapperProfile : Profile
{
    public ProductMapperProfile()
    {
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        CreateMap<ProductModel, ProductDetailsDto>();
    }
}