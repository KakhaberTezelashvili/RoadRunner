using Microsoft.Data.SqlClient;
using TDOC.Common.Data.Enumerations.IncrementalRead;

namespace TDOC.Common.Server.IncrementalDataAccess
{
    /// <summary>
    /// Provides functionality for incremental reading of textual and binary data from a Sql Server database.
    /// </summary>
    public class IncrementalDataReader
    {
        /// <summary>
        /// Retrieves a sql query designed for incremental data reading.
        /// </summary>
        /// <param name="tableName">Name of the table that data should be read from.</param>
        /// <param name="dataColumnName">Name of the column that the data should be read from.</param>
        /// <param name="keyColumnName">Name of the column used to identify the row to read from.</param>
        /// <returns>A sql query for incremental reading.</returns>
        private static string GetSqlQuery(string tableName, string dataColumnName, string keyColumnName)
        {
            return $@"
                SELECT
                    {dataColumnName}
                FROM
                    {tableName}
                WHERE
                    {keyColumnName} = @{keyColumnName}
            ";
        }

        /// <summary>
        /// Performs incremental retrieval of the specified data source, repeatedly returning parts to the caller through
        /// the specified callback.
        /// </summary>
        /// <param name="sqlConnection">Database connection.</param>
        /// <param name="tableName">Name of the database table to read from.</param>
        /// <param name="dataColumnName">Name of the table column to read from.</param>
        /// <param name="keyColumnName">Name of the column used to identify the row to read from.</param>
        /// <param name="keyColumnValue">Value used to identify the row to read from.</param>
        /// <param name="bytesReadCallback">Delegate that is called each time data has been read.</param>
        /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
        /// <param name="bufferSize">The size (in bytes) of the data to return after each read.</param>
        /// <param name="startPosition">The position (in bytes) of where the first read should begin. The position of the first byte is always 0.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result specifies the result of the entire operation.</returns>
        private static async Task<IncrementalReadResult> DoIncrementalReadsAsync(SqlConnection sqlConnection, string tableName, string dataColumnName,
            string keyColumnName, object keyColumnValue,
            IncrementalReadCallback bytesReadCallback, CancellationToken? cancellationToken,
            int bufferSize = 1024, int startPosition = 0)
        {
            using var cmd = new SqlCommand(GetSqlQuery(tableName, dataColumnName, keyColumnName), sqlConnection);
            cmd.Parameters.AddWithValue($"@{keyColumnName}", keyColumnValue);

            long position = startPosition;

            await sqlConnection.OpenAsync();
            SqlDataReader reader = (cancellationToken != null) ?
                await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess, cancellationToken.Value) :
                await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess)
            ;
            try
            {
                if (!await reader.ReadAsync())
                {
                    return IncrementalReadResult.NotFound;
                }
                if (await reader.IsDBNullAsync(0))
                {
                    return IncrementalReadResult.NotFound;
                }

                byte[] partialData = new byte[bufferSize];
                bool firstIncrementalRead = true;
                long numberBytesRead;
                do
                {
                    if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                    {
                        return IncrementalReadResult.Cancelled;
                    }
                    numberBytesRead = reader.GetBytes(0, position, partialData, 0, bufferSize);
                    if (numberBytesRead > 0)
                    {
                        position += numberBytesRead;
                        await bytesReadCallback(partialData, (int)numberBytesRead, firstIncrementalRead);
                        firstIncrementalRead = false;
                    }
                } while (numberBytesRead == bufferSize);

                return IncrementalReadResult.Success;
            }
            finally
            {
                await reader.CloseAsync();
                await sqlConnection.CloseAsync();
            }
        }

        /// <summary>
        /// Performs incremental retrieval of the specified data source, repeatedly returning parts to the caller through
        /// the specified callback.
        /// </summary>
        /// <param name="sqlConnection">Database connection.</param>
        /// <param name="tableName">Name of the database table to read from.</param>
        /// <param name="dataColumnName">Name of the table column to read from.</param>
        /// <param name="keyColumnName">Name of the column used to identify the row to read from.</param>
        /// <param name="keyColumnValue">Value used to identify the row to read from.</param>
        /// <param name="bytesReadCallback">Delegate that is called each time data has been read.</param>
        /// <param name="cancellationToken">Cancellation token allowing the caller to cancel the task.</param>
        /// <param name="bufferSize">The size (in bytes) of the data to return after each read.</param>
        /// <param name="startPosition">The position (in bytes) of where the first read should begin. The position of the first byte is always 0.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result specifies the result of the entire operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sqlConnection"/>, <paramref name="keyColumnValue"/>, or <paramref name="bytesReadCallback"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="tableName"/>, <paramref name="dataColumnName"/>, or <paramref name="keyColumnName"/> is either <c>null</c> or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="bufferSize"/> or <paramref name="startPosition"/> is zero or less.</exception>
        public static async Task<IncrementalReadResult> IncrementalReadAsync(SqlConnection sqlConnection, string tableName, string dataColumnName,
            string keyColumnName, object keyColumnValue,
            IncrementalReadCallback bytesReadCallback, CancellationToken? cancellationToken,
            int bufferSize = 1024, int startPosition = 0)
        {
            if (sqlConnection == null)
            {
                throw new ArgumentNullException(nameof(sqlConnection));
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException($"{nameof(tableName)} is null or empty.");
            }
            if (string.IsNullOrEmpty(dataColumnName))
            {
                throw new ArgumentException($"{nameof(dataColumnName)} is null or empty.");
            }
            if (string.IsNullOrEmpty(keyColumnName))
            {
                throw new ArgumentException($"{nameof(keyColumnName)} is null or empty.");
            }
            if (keyColumnValue == null)
            {
                throw new ArgumentNullException(nameof(keyColumnValue));
            }
            if (bytesReadCallback == null)
            {
                throw new ArgumentNullException(nameof(bytesReadCallback));
            }
            if (bufferSize < 1)
            {
                throw new ArgumentException($"{nameof(bufferSize)} must be greater than 0.");
            }
            if (startPosition < 0)
            {
                throw new ArgumentException($"{nameof(startPosition)} must be greater than or equal to 0.");
            }

            return await DoIncrementalReadsAsync(sqlConnection, tableName, dataColumnName,
                keyColumnName, keyColumnValue,
                bytesReadCallback, cancellationToken,
                bufferSize, startPosition
            );
        }

        /// <summary>
        /// Reads the specified amount of bytes from the specified data source.
        /// </summary>
        /// <param name="sqlConnection">Database connection.</param>
        /// <param name="tableName">Name of the database table to read from.</param>
        /// <param name="dataColumnName">Name of the table column to read from.</param>
        /// <param name="keyColumnName">Name of the column used to identify the row to read from.</param>
        /// <param name="keyColumnValue">Value used to identify the row to read from.</param>
        /// <param name="startPosition">The position (in bytes) of where the first read should begin. The position of the first byte is always 1.</param>
        /// <param name="length">The amount of bytes to read.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result are the bytes read, if the data source was found; <c>null</c>, otherwise.</returns>
        private static async Task<byte[]> DoReadAsync(SqlConnection sqlConnection, string tableName, string dataColumnName,
            string keyColumnName, object keyColumnValue, int startPosition, int length)
        {
            using var cmd = new SqlCommand(GetSqlQuery(tableName, dataColumnName, keyColumnName), sqlConnection);

            cmd.Parameters.AddWithValue($"@{keyColumnName}", keyColumnValue);

            await sqlConnection.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess);
            try
            {
                if (!await reader.ReadAsync())
                {
                    // Not found
                    return null;
                }
                if (await reader.IsDBNullAsync(0))
                {
                    // Field is NULL
                    return null;
                }

                byte[] buffer = new byte[length];

                long numberBytesRead = reader.GetBytes(0, startPosition, buffer, 0, length);

                byte[] result = new byte[numberBytesRead];

                Buffer.BlockCopy(buffer, 0, result, 0, (int)numberBytesRead);

                return result;
            }
            finally
            {
                await reader.CloseAsync();
                await sqlConnection.CloseAsync();
            }
        }

        /// <summary>
        /// Reads the specified amount of bytes from the specified data source.
        /// </summary>
        /// <param name="sqlConnection">Database connection.</param>
        /// <param name="tableName">Name of the database table to read from.</param>
        /// <param name="dataColumnName">Name of the table column to read from.</param>
        /// <param name="keyColumnName">Name of the column used to identify the row to read from.</param>
        /// <param name="keyColumnValue">Value used to identify the row to read from.</param>
        /// <param name="startPosition">The position (in bytes) of where the first read should begin. The position of the first byte is always 1.</param>
        /// <param name="length">The amount of bytes to read.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result are the bytes read, if the data source was found; <c>null</c>, otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sqlConnection"/> or <paramref name="keyColumnValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="tableName"/>, <paramref name="dataColumnName"/>, or <paramref name="keyColumnName"/> is either <c>null</c> or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="startPosition"/> or <paramref name="length"/> is zero or less.</exception>
        public static async Task<byte[]> ReadAsync(SqlConnection sqlConnection, string tableName, string dataColumnName,
            string keyColumnName, object keyColumnValue, int startPosition, int length)
        {
            if (sqlConnection == null)
            {
                throw new ArgumentNullException(nameof(sqlConnection));
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException($"{nameof(tableName)} is null or empty.");
            }
            if (string.IsNullOrEmpty(dataColumnName))
            {
                throw new ArgumentException($"{nameof(dataColumnName)} is null or empty.");
            }
            if (string.IsNullOrEmpty(keyColumnName))
            {
                throw new ArgumentException($"{nameof(keyColumnName)} is null or empty.");
            }
            if (keyColumnValue == null)
            {
                throw new ArgumentNullException(nameof(keyColumnValue));
            }
            if (startPosition < 0)
            {
                throw new ArgumentException($"{nameof(startPosition)} must be greater than or equal to 0.");
            }
            if (length < 1)
            {
                throw new ArgumentException($"{nameof(length)} must be greater than 0.");
            }

            return await DoReadAsync(sqlConnection, tableName, dataColumnName,
                keyColumnName, keyColumnValue, startPosition, length
            );
        }
    }
}