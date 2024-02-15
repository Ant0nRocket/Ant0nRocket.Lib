using System;
using System.Collections.Generic;
using System.Data;

namespace Ant0nRocket.Lib.Data
{
    /// <summary>
    /// Basic SQL-Database adapter.
    /// </summary>
    public interface ISqlDatabaseAdapter
    {
        /// <summary>
        /// Connect to database with <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Proper connection string</param>
        bool Connect(string connectionString);

        /// <summary>
        /// Actual connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Execute single SQL command. If you need to execute multiple
        /// SQL commands inside same trascaction use <see cref="ExecBatchNonQuerySql(IEnumerable{SqlParamMapper})"/>.
        /// </summary>
        int ExecNonQuerySql(SqlParamMapper sqlParamMapper);

        /// <summary>
        /// Execute multiple SQL commands inside same trascaction.
        /// If you need to execute single command - consider use of
        /// <see cref="ExecNonQuerySql(SqlParamMapper)"/>.
        /// </summary>
        int ExecBatchNonQuerySql(IEnumerable<SqlParamMapper> sqlParamMappers);

        /// <summary>
        /// Execute SQL with some SELECT syntax.<br />
        /// Parameters are in <paramref name="sqlParamMapper"/>.<br />
        /// Action with each new line <paramref name="onNextRowRead"/>.
        /// </summary>
        void ExecQuerySql(SqlParamMapper sqlParamMapper, Action<IDataReader> onNextRowRead);

        /// <summary>
        /// Close connection to database.
        /// </summary>
        void Close();
    }
}
