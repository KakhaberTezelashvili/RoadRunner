using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.API.MapperProfiles;

public class ItemMapperProfile : Profile
{
    public ItemMapperProfile()
    {
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();     
        CreateMap<ItemModel, ItemDetailsDto>()
            .ForMember(dest => dest.LocalName, opt => opt.MapFrom(src => src.HospitalDescription))
            .ForMember(dest => dest.ManufacturerNo, opt => opt.MapFrom(src => src.ManufactorNo))
            .ForMember(dest => dest.ItemGroup, opt => opt.MapFrom(src => src.ItGrp))
            .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Manu))
            .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supp));
    }
}