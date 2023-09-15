using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TDOC.EntityFramework.KeyGeneration
{
    public class SequenceNumberGenerator
    {
        private const string seqTypeParameter = "SEQTYPE";
        private const string seqRefTypeParameter = "SEQREFTYPE";
        private const string seqRefKeyIdParameter = "SEQREFKEYID";
        private const string nextCounterParameter = "NEXTCOUNTER";

        /// <summary>
        /// Returns the next sequence number for the specified sequence type and key id of the entity type.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <param name="type">Sequence type.</param>
        /// <param name="refType">The entiry the sequence counter belongs to.
        /// Possible values: None, Factory, Supplier, Customer (None indicates it is a global/system value).</param>
        /// <param name="refKeyId">The entity key id.</param>
        /// <returns>The next sequence number.</returns>
        public static async Task<int> GetSequenceNumberAsync(TDocEFDbContext context, SequenceType type, TableTypes refType, int refKeyId)
        {
            string storedProcedureName = "SP_NEXTSEQUENCECOUNT";
            var seqType = new SqlParameter()
            {
                ParameterName = seqTypeParameter,
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Value = (int)type
            };

            var seqRefType = new SqlParameter()
            {
                ParameterName = seqRefTypeParameter,
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Value = (int)refType
            };

            var seqRefKeyId = new SqlParameter()
            {
                ParameterName = seqRefKeyIdParameter,
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input,
                Value = refKeyId
            };

            var nextCounter = new SqlParameter()
            {
                ParameterName = nextCounterParameter,
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };

            await context.Database.ExecuteSqlRawAsync(
                $"{storedProcedureName} @{seqTypeParameter}, @{seqRefTypeParameter}, @{seqRefKeyIdParameter}, @{nextCounterParameter} out",
                seqType,
                seqRefType,
                seqRefKeyId,
                nextCounter);

            return System.Convert.ToInt32(nextCounter.Value);
        }
    }
}