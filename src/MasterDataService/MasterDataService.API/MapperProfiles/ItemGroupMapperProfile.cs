using MasterDataService.Shared.Dtos.ItemGroups;

namespace MasterDataService.API.MapperProfiles;

public class ItemGroupMapperProfile : Profile
{
    public ItemGroupMapperProfile()
    {
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        CreateMap<ItemGroupModel, ItemGroupDetailsDto>();
    }
}