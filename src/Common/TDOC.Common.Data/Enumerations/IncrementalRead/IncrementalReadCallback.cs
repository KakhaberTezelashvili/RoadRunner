namespace TDOC.Common.Data.Enumerations.IncrementalRead
{
    /// <summary>
    /// References a method to be called during an incremental read operation.
    /// </summary>
    /// <param name="buffer">The data read during the current increment.</param>
    /// <param name="count">The number of bytes delivered in this increment; this number may be smaller
    /// than the size of the buffer.</param>
    /// <param name="isFirstIncrement">Indicates if the current increment is the first.</param>
    /// <returns></returns>
    public delegate Task IncrementalReadCallback(byte[] buffer, int count, bool isFirstIncrement);
}