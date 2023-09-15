using AutoMapper;
using MasterDataService.Client.MapperProfiles.Resolvers.Item;
using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Client.MapperProfiles;

public class ItemMapperProfile : Profile
{
    public ItemMapperProfile()
    {
        // This will map the following properties to each other: property_name->PropertyName
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();

        CreateMap<ItemDetailsDto, ItemUpdateArgs>()
            .ForMember(dest => dest.SupplierKeyId, opt => opt.MapFrom(new SupplierKeyIdResolver()))
            .ForMember(dest => dest.ItemGroupKeyId, opt => opt.MapFrom(new ItemGroupKeyIdResolver()))
            .ForMember(dest => dest.ManufacturerKeyId, opt => opt.MapFrom(new ManufacturerKeyIdResolver()));
    }
}