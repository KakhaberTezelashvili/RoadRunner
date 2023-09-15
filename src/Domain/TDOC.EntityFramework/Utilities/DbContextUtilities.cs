using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TDOC.EntityFramework.Utilities
{
    /// <summary>
    /// DB context utility.
    /// </summary>
    public static class DbContextUtilities
    {
        // TASK: TRO 18237 - Handling of database concurrency issues
        /// <summary>
        /// The purpose of using this method is to avoid exceptions when concurrency adding new records into TDESKTOP table.
        /// </summary>
        public static async Task TryToSaveDBChangesAsync(TDocEFDbContext context)
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdEx)
              when (dbUpdEx.InnerException != null && dbUpdEx.InnerException is SqlException sqlEx &&
                (sqlEx.Number == 2601 || sqlEx.Number == 2627)) //2601: Duplicated key row error; 2627: Unique constraint error;
            {
                // This exception can occur if DB record was never added before and two concurrency threads are trying to add it.
                // We eat the exception.
                return;
            }
        }
    }
}