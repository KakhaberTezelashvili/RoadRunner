using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Models.Lots
{
    /// <summary>
    /// Class helper for <see cref="LotInformationDto" /> DTO.
    /// </summary>
    public class LotInformationExtension
    {
        private readonly LotInformationDto _lotInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="LotInformationExtension" /> class.
        /// </summary>
        /// <param name="lotInformation">Lot information.</param>
        public LotInformationExtension(LotInformationDto lotInformation)
        {
            _lotInformation = lotInformation;
        }

        /// <summary>
        /// Retrieves a lot information of the specified item at the specified position.
        /// </summary>
        /// <param name="itemKeyId">Item key id.</param>
        /// <param name="position">Item position.</param>
        /// <returns>An <see cref="ItemLotInformationDto" /> instance, if the item was found at the specified position or at any position when "position" = -1;
        /// <c>null</c> otherwise.</returns>
        private ItemLotInformationDto GetItemByKeyId(int itemKeyId, int position)
        {
            foreach (ItemLotInformationDto item in _lotInformation.Items)
            {
                if ((item.KeyId == itemKeyId) && ((item.Position == position) || (position == TDocConstants.NotAssigned)))
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves a lot information of the specified item.
        /// </summary>
        /// <param name="itemKeyId">Item key id.</param>
        /// <returns>An <see cref="ItemLotInformationDto" /> instance, if the item was found;
        /// <c>null</c> otherwise.</returns>
        public ItemLotInformationDto GetItemByKeyId(int itemKeyId) => GetItemByKeyId(itemKeyId, TDocConstants.NotAssigned);

        /// <summary>
        /// Checks if the LotInformation contains the specified item at the specified position.
        /// </summary>
        /// <param name="itemKeyId">Item key id.</param>
        /// <param name="position">Item position.</param>
        /// <returns><c>true</c> if the LotInformation contains the specified item at the specified position;
        /// <c>false</c> otherwise.</returns>
        public bool ItemExists(int itemKeyId, int position) => GetItemByKeyId(itemKeyId, position) != null;

        /// <summary>
        /// Checks if the LotInformation contains the specified item.
        /// </summary>
        /// <param name="itemKeyId">Item key id.</param>
        /// <returns><c>true</c> if the LotInformation contains the specified item;
        /// <c>false</c> otherwise.</returns>
        public bool ItemExists(int itemKeyId) => ItemExists(itemKeyId, TDocConstants.NotAssigned);

        /// <summary>
        /// Retrieves count of items present in lot information.
        /// </summary>
        /// <returns>Count of items.</returns>
        public int ItemCount() => _lotInformation.Items.Count;
    }
}