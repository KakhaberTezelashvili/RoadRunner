using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace TDOC.EntityFramework.KeyGeneration
{
    /// <summary>
    /// Factory for creating key id generator class instances.
    /// </summary>
    public static class KeyIdGeneratorFactory
    {
        /// <summary>
        /// Creates and returns a new instance of the <see cref="BaseKeyIdGenerator"/> class for
        /// generating primary keys for non production tables.
        /// </summary>
        /// <param name="keyColumnName">Name of the database column that the generator will generate keys for.</param>
        /// <returns>A new instance of the <see cref="BaseKeyIdGenerator"/> class.</returns>
        public static KeyIdGenerator CreateBaseKeyIdGenerator(string keyColumnName) => new BaseKeyIdGenerator(keyColumnName);

        /// <summary>
        /// Creates and returns a new instance of the <see cref="ProcessKeyIdGenerator"/> class for
        /// generating primary keys for the machine process table.
        /// </summary>
        /// <param name="keyColumnName">Name of the database column that the generator will generate keys for.</param>
        /// <returns>A new instance of the <see cref="ProcessKeyIdGenerator"/> class.</returns>
        public static KeyIdGenerator CreateProcessKeyIdGenerator(string keyColumnName) => new ProcessKeyIdGenerator(keyColumnName);

        /// <summary>
        /// Creates and returns a new instance of the <see cref="UnitKeyIdGenerator"/> class for
        /// generating primary keys for the unit table.
        /// </summary>
        /// <param name="keyColumnName">Name of the database column that the generator will generate keys for.</param>
        /// <returns>A new instance of the <see cref="UnitKeyIdGenerator"/> class.</returns>
        public static KeyIdGenerator CreateUnitKeyIdGenerator(string keyColumnName) => new UnitKeyIdGenerator(keyColumnName);
    }

    /// <summary>
    /// Base class for key id <see cref="ValueGenerator"/> classes.
    /// </summary>
    public abstract class KeyIdGenerator : ValueGenerator<int>
    {
        /// <summary>
        /// Name of the stored procedure that is used for generating keys.
        /// </summary>
        private readonly string _storedProcedureName;
        /// <summary>
        /// Name of the database key column that the stored procedure is generating keys for.
        /// </summary>
        private readonly string _keyColumnName;

        /// <summary>
        /// Creates a new instance of the <see cref="KeyIdGenerator"/> class.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure that will be used for key generation.</param>
        /// <param name="keyColumnName">Name of the database column that the key will be created for.</param>
        public KeyIdGenerator(string storedProcedureName, string keyColumnName)
        {
            _storedProcedureName = storedProcedureName;
            _keyColumnName = keyColumnName;
        }

        /// <summary>
        /// Denotes if the key generated are temporary or permanent. Is always
        /// <c>false</c> to indicate that the generated keys are permanent.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;

        /// <summary>
        /// Returns the next key id for the specified database column using
        /// the specified stored procedure.
        /// </summary>
        /// <param name="entry">Change tracking for the entity.</param>
        /// <returns>The next key id.</returns>
        public override int Next(EntityEntry entry) => GetNextKeyId(entry.Context);

        /// <summary>
        /// Returns the next key id for the specified database column using
        /// the specified stored procedure.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <returns>The next key id.</returns>
        public int GetNextKeyId(Microsoft.EntityFrameworkCore.DbContext context) => GetKeyId(context);

        /// <summary>
        /// Returns the next key id for the specified database column using
        /// the specified stored procedure.
        /// </summary>
        /// <param name="context">Entity Framework database context.</param>
        /// <returns>The next key id.</returns>
        private int GetKeyId(Microsoft.EntityFrameworkCore.DbContext context)
        {
            var keyIdIdent = new SqlParameter()
            {
                ParameterName = "KEYIDIDENT",
                SqlDbType = System.Data.SqlDbType.VarChar,
                Direction = System.Data.ParameterDirection.Input,
                Value = _keyColumnName
            };

            var nextKeyId = new SqlParameter()
            {
                ParameterName = "NEXTKEYID",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };

            context.Database.ExecuteSqlRaw(
                $"{_storedProcedureName} @KEYIDIDENT, @NEXTKEYID out",
                keyIdIdent,
                nextKeyId
            );

            return System.Convert.ToInt32(nextKeyId.Value);
        }
    }

    /// <summary>
    /// Primary key generator for base (non production) tables.
    /// </summary>
    public class BaseKeyIdGenerator : KeyIdGenerator
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BaseKeyIdGenerator" /> class.
        /// </summary>
        /// <param name="keyColumnName">Name of the database column that keys will be generated for.</param>
        public BaseKeyIdGenerator(string keyColumnName) : base("SP_NEXTBASEKEYID", keyColumnName)
        {
        }
    }

    /// <summary>
    /// Primary key generator for machine processes (TPROCESS).
    /// </summary>
    public class ProcessKeyIdGenerator : KeyIdGenerator
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ProcessKeyIdGenerator" /> class.
        /// </summary>
        /// <param name="keyColumnName">Name of the database column that keys will be generated for.</param>
        public ProcessKeyIdGenerator(string keyColumnName) : base("SP_NEXTPROCKEYID", keyColumnName)
        {
        }
    }

    /// <summary>
    /// Primary key id generator for units (TUNIT).
    /// </summary>
    public class UnitKeyIdGenerator : KeyIdGenerator
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UnitKeyIdGenerator" /> class.
        /// </summary>
        /// <param name="keyColumnName">Name of the database column that keys will be generated for.</param>
        public UnitKeyIdGenerator(string keyColumnName) : base("SP_NEXTUNITKEYID", keyColumnName)
        {
        }
    }
}