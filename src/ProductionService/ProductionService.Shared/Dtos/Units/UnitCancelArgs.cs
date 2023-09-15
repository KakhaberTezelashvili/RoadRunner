namespace ProductionService.Shared.Dtos.Units
{
    /// <summary>
    /// Unit cancel arguments.
    /// </summary>
    public record UnitCancelArgs
    {
        /// <summary>
        /// Unit key identfiers. 
        /// </summary>
        public List<int> UnitKeyIds { get; init; }
    }
}