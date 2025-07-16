using System;
using System.Collections.Generic;
using System.Data;
using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.Logging;

namespace Ant0nRocket.Lib.Data
{
    /// <summary>
    /// Basic implementation of <see cref="ISqlDatabaseAdapter"/>.
    /// </summary>
    public class SqlDatabaseAdapter<T> : ISqlDatabaseAdapter, IDisposable where T : IDbConnection
    {
        private T? _connection;

        /// <inheritdoc />
        public string? ConnectionString { get; private set; }

        /// <inheritdoc />
        public bool Connect(string connectionString)
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
                throw new ApplicationException("you have fucked up with database connections!");

            ConnectionString = connectionString;
            _connection = Activator.CreateInstance<T>();

            Logger.LogTrace(connectionString);
            _connection.ConnectionString = connectionString;

            try
            {
                _connection.Open();
                Logger.LogInformation($"Connection '{typeof(T).Name}' opened");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

        /// <inheritdoc />
        public void Close()
        {
            if (_connection == null) return;
            _connection.Close();
            Logger.LogInformation($"Connection '{typeof(T).Name}' closed");
        }

        /// <inheritdoc />
        public int ExecBatchNonQuerySql(IEnumerable<SqlParamMapper> sqlParamMappers)
        {
            using var transaction = _connection?.BeginTransaction() ??
                throw new NoNullAllowedException(nameof(_connection));

            var rowsAffected = 0;

            foreach (var sqlParamMapper in sqlParamMappers)
            {
                using var command = sqlParamMapper.CreateDbCommand(_connection);
                command.Transaction = transaction;
                try
                {
                    rowsAffected += command.ExecuteNonQuery();

#if DEBUG 
                    Logger.LogDebug($"OK - \n{sqlParamMapper.AsJson(pretty: true)}");
#endif
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    transaction.Rollback();
                    return 0;
                }
            }

            try
            {
                transaction.Commit();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return 0;
            }
        }

        /// <inheritdoc />
        public int ExecNonQuerySql(SqlParamMapper sqlParamMapper) =>
            ExecBatchNonQuerySql(new List<SqlParamMapper> { sqlParamMapper });

        /// <inheritdoc />
        public void ExecQuerySql(SqlParamMapper sqlParamMapper, Action<IDataReader> onNextRowRead)
        {
            using var command = sqlParamMapper.CreateDbCommand(_connection!);

            try
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                    onNextRowRead(reader);

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /// <inheritdoc />
        public void Dispose() => Close();
    }
}
