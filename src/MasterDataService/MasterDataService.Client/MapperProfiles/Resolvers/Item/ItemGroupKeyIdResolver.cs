using AutoMapper;
using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Client.MapperProfiles.Resolvers.Item;

public class ItemGroupKeyIdResolver : IValueResolver<ItemDetailsDto, ItemUpdateArgs, int?>
{
    public int? Resolve(ItemDetailsDto source, ItemUpdateArgs destination, int? destMember, ResolutionContext context) 
        => source.ItemGroup?.KeyId;
}