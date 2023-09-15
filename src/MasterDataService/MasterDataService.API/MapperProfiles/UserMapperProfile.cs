using MasterDataService.Shared.Dtos.Users;

namespace MasterDataService.API.MapperProfiles;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        CreateMap<UserModel, UserDetailsDto>();
    }
}