using Microsoft.Data.SqlClient;
using System.Data;

namespace TDOC.Common.Server.IncrementalDataAccess
{
    /// <summary>
    /// Provides stream operations for reading from a textual or binary database field.
    /// </summary>
    public class BinaryDbStream : Stream
    {
        private bool _disposed = false;
        // Database access
        private readonly SqlConnection _connection;
        private SqlCommand _command;
        private SqlDataReader _reader;
        // Command parameters
        private readonly string _commandParameterKeyColumnValue = "@VALUE";
        // Data source information
        private readonly string _tableName;
        private readonly string _dataColumnName;
        private readonly string _keyColumnName;
        private readonly object _keyColumnValue;
        // Stream information
        private readonly long _length;
        private long _position;
        private int _readTimeout;
        // Pre-allocate a one byte buffer to avoid allocations during calls to ReadByte()
        private readonly byte[] _oneByteBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDbStream" /> class.
        /// </summary>
        /// <param name="connection">Connection to the Sql database.</param>
        /// <param name="tableName">Name of the database table to read from.</param>
        /// <param name="dataColumnName">Name of the table column to read from.</param>
        /// <param name="keyColumnName">Name of the column used to identify the row to read from.</param>
        /// <param name="keyColumnValue">Value used to identify the row to read from (see <paramref name="keyColumnName" />).</param>
        public BinaryDbStream(SqlConnection connection, string tableName, string dataColumnName, string keyColumnName, object keyColumnValue)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _reader = null; // Created on demand
            _tableName = tableName;
            _dataColumnName = dataColumnName;
            _keyColumnName = keyColumnName;
            _keyColumnValue = keyColumnValue;

            _length = GetLength();
            _position = 0;
            _readTimeout = 30 * 1000; // 30 seconds

            _oneByteBuffer = new byte[1];

            PrepareCommand();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _command.Dispose();
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        #region Private methods

        /// <summary>
        /// Instantiates and configures the internal <see cref="SqlCommand"/> for reading from the internal database connection.
        /// </summary>
        private void PrepareCommand()
        {
            string sql = $@"
                SELECT
                    {_dataColumnName}
                FROM
                    {_tableName}
                WHERE
                    {_keyColumnName} = {_commandParameterKeyColumnValue}
            ";

            // Create command
            _command = new SqlCommand(sql, _connection)
            {
                CommandTimeout = _readTimeout / 1000
            };
            // Add parameter for key value
            _command.Parameters.AddWithValue(_commandParameterKeyColumnValue, _keyColumnValue);
        }

        private long GetLength()
        {
            string sql = $@"
                SELECT
                    DATALENGTH({_dataColumnName})
                FROM
                    {_tableName}
                WHERE
                    {_keyColumnName} = @{_keyColumnName}
            ";

            using var cmd = new SqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue($"@{_keyColumnName}", _keyColumnValue);

            _connection.Open();
            try
            {
                return (long)cmd.ExecuteScalar();
            }
            finally
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Reads the specified amount of bytes from the data source. This does not advance the position.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer will contain the bytes read;
        /// the result of the method will indicate the actual number of bytes that were read.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        private long DoRead(byte[] buffer, int count)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            if (_reader == null)
            {
                _reader = _command.ExecuteReader(CommandBehavior.SequentialAccess);
                _reader.Read();
            }

            return _reader.GetBytes(0, _position, buffer, 0, count);
        }

        #endregion Private methods

        #region Stream overrides

        /// <summary>
        /// Gets a value indicating whether the stream supports reading. Always returns <c>true</c>.
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// Gets a value indicating whether the stream supports seeking. Always returns <c>true</c>.
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// Gets a value that determines whether the stream can time out. Always returns <c>true</c>.
        /// </summary>
        public override bool CanTimeout => true;

        /// <summary>
        /// Gets a value indicating whether the stream supports writing. Always returns <c>false</c>.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => _length;

        /// <summary>
        /// Gets or sets the position within the stream.
        /// </summary>
        public override long Position
        {
            get => _position;
            set => _position = (value >= 0) ? value : 0;
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before timing out.
        /// </summary>
        public override int ReadTimeout
        {
            get => _readTimeout;
            set
            {
                _readTimeout = value;
                _command.CommandTimeout = _readTimeout / 1000;
            }
        }

        /// <summary>
        /// Not supported. Will always throw an <see cref="InvalidOperationException"/> exception.
        /// </summary>
        /// <exception cref="InvalidOperationException">Always thrown.</exception>
        public override int WriteTimeout
        {
            get => throw new InvalidOperationException("This class does not support write operations.");
            set => throw new InvalidOperationException("This class does not support write operations.");
        }

        public override void Close()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
            }
        }

        public override int EndRead(IAsyncResult asyncResult) => throw new NotImplementedException();

        public override void EndWrite(IAsyncResult asyncResult) => throw new NotImplementedException();

        public override void Flush() => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(offset)} cannot be negative.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");
            }

            long numberBytesRead = DoRead(buffer, count);

            _position += numberBytesRead;

            return (int)numberBytesRead;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(offset)} cannot be negative.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");
            }

            // If cancellation was requested, bail early
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            int bytesRead = Read(buffer, offset, count);

            return Task.FromResult(bytesRead);
        }

        public override int ReadByte()
        {
            if (_position >= _length)
            {
                return -1;
            }

            long numberBytesRead = DoRead(_oneByteBuffer, 1);

            _position += numberBytesRead;

            return _oneByteBuffer[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        if (offset < 0)
                        {
                            // Should this throw?
                            _position = 0;
                        }
                        else
                        {
                            _position = offset;
                        }
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        _position += offset;

                        if (_position < 0)
                        {
                            _position = 0;
                        }
                        break;
                    }
                case SeekOrigin.End:
                    {
                        _position = _length - 1 + offset;
                        break;
                    }
                default:
                    throw new ArgumentException($"Invalid {nameof(SeekOrigin)}.");
            }

            return _position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        #endregion Stream overrides
    }
}