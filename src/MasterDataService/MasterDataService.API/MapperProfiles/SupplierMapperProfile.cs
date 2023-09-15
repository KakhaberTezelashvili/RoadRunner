using MasterDataService.Shared.Dtos.Suppliers;

namespace MasterDataService.API.MapperProfiles;

public class SupplierMapperProfile : Profile
{
    public SupplierMapperProfile()
    {
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        CreateMap<SupplierModel, SupplierDetailsDto>();
    }
}