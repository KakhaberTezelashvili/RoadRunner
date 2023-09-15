using MasterDataService.Shared.Dtos.Customers;

namespace MasterDataService.API.MapperProfiles;

public class CustomerMapperProfile : Profile
{
    public CustomerMapperProfile()
    {
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        CreateMap<CustomerModel, CustomerDetailsDto>();
    }
}