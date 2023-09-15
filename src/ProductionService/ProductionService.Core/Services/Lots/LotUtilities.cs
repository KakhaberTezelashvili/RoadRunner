namespace ProductionService.Core.Services.Lots;

/// <summary>
/// Keeps additional information needed for displaying on lot information dialog.
/// </summary>
public struct HeaderInfo
{
    public string Info1;
    public string Info2;
    public string Info3;
    public string Info4;
}

/// <summary>
/// Provides different utilities for lots.
/// </summary>
public static class LotUtilities
{
    /// <summary>
    /// Returns lot information helper class fot the specified T-DOC table.
    /// </summary>
    /// <param name="table">T-DOC table.</param>
    /// <returns>A <see cref="CustomLotInfoHelper"/> instance, if it exists for the specified T-DOC table;
    /// <c>null</c> otherwise.</returns>
    public static CustomLotInfoHelper LotInfoHelperByTable(TDOCTable table)
    {
        CustomLotInfoHelper lotInfoHelper = null;
        switch (table)
        {
            case TDOCTable.Unit:
                lotInfoHelper = new UnitLotInfoHelper();
                break;

                // TODO
                //case TDOCTable.Product:
                //    lotInfoHelper = new ProductLotInfoHelper();
                //    break;

                // TODO
                //case TDOCTable.Composite:
                //    lotInfoHelper = new CompositeLotInfoHelper();
                //    break;

                // TODO
                //case TDOCTable.IndicatorType:
                //    lotInfoHelper = new IndicTypeLotInfoHelper();
                //    break;
        }

        return lotInfoHelper;
    }
}