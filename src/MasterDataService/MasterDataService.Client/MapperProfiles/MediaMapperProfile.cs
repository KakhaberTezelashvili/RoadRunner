using AutoMapper;
using MasterDataService.Shared.Dtos.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Client.MapperProfiles;

public class MediaMapperProfile : Profile
{
    public MediaMapperProfile()
    {
        // This will map the following properties to each other: property_name->PropertyName
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();

        CreateMap<MediaEntryDto, MediaEntryData>();
        CreateMap<MediaItemDto, MediaItemType>();
    }
}