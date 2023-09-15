using AutoMapper;
using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.API.MapperProfiles
{
    public class PositionMapperProfile : Profile
    {
        public PositionMapperProfile()
        {
            // This will map the following properties to each other: property_name->PropertyName
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            ClearPrefixes();

            CreateMap<PosLocationModel, PositionLocationsDetailsDto>()
            .ForMember(dest => dest.LocationKeyId, opt => opt.MapFrom(src => src.Loca.KeyId))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Loca.Name))
            .ForMember(dest => dest.Process, opt => opt.MapFrom(src => src.Loca.Process))
            .ForMember(dest => dest.UIAvailability, opt => opt.MapFrom(src => src.UIAvailability != UILocaAvailability.Default ? src.UIAvailability : src.Loca.UIAvailability))
            .ForMember(dest => dest.Default, opt => opt.MapFrom(src => src.Default))
            .ForMember(dest => dest.ShowMES, opt => opt.MapFrom(src => false));
        }
    }
}