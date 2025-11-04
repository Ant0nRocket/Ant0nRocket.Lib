using System;
using System.Data;

using Ant0nRocket.Lib.Patterns;

namespace Ant0nRocket.Lib.Data
{
    /// <summary>
    /// Simple ADO.Net connection wrapper.
    /// </summary>
    public class SqlDatabaseAdapter<T> where T : IDbConnection, new()
    {
        // Will try to use it only if no connection string provided to constructor.
        // But if no connection string provided and no getters registered - constructor
        // will throw an InvalidOperationException
        private static Func<string>? _FuncGetDefaultConnectionString;

        private readonly string _connectionString;

        /// <summary>
        /// If you don't need to warry about getting a connection string from
        /// application settings - register getter function here and just create
        /// <see cref="SqlDatabaseAdapter{T}"/> instance with no connection
        /// strings provided.
        /// </summary>
        /// <param name="funcGetDefaultConnectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RegisterDefaultConnectionStringGetter(Func<string> funcGetDefaultConnectionString)
        {
            _FuncGetDefaultConnectionString = funcGetDefaultConnectionString ??
                throw new ArgumentNullException(nameof(funcGetDefaultConnectionString));
        }

        /// <summary>
        /// Creates an instance of <see cref="SqlDatabaseAdapter{T}"/>.
        /// </summary>
        /// <param name="connectionString">Connection string for specified database</param>
        public SqlDatabaseAdapter(string? connectionString = default)
        {
            _connectionString = connectionString ??
                _FuncGetDefaultConnectionString?.Invoke() ??
                throw new InvalidOperationException("there is no way known for getting a connection string");

        }

        /*
        
        IMPORTANT NOTES !!!

        Every function that executes SQL will create it's own IDbConnection of type T,
        use _connectionString, do some job and then dispose the connection!
        Dont' try to mix reading and writing operations here!!!

        If you need to something mixed call WithDbConnection function and do what you want inside
        of a callback. Connection will be closed automatically!

        */

        private T GetDbConnection()
        {
            var dbConnection = Activator.CreateInstance<T>();
            dbConnection.ConnectionString = _connectionString;
            return dbConnection;
        }

        /// <summary>
        /// Provides a IDbConnection for your own purpose, do whatever you want.
        /// </summary>
        public Result WithDbConnection(Action<T> callback)
        {
            using var dbConnection = GetDbConnection();

            try
            {
                dbConnection.Open();
                callback?.Invoke(dbConnection);
                return Result.Success(); // connection will be closed automatically
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        }

        /// <summary>
        /// For GET operations only!
        /// Executes the <paramref name="sqlCommand"/> on DataReader and
        /// invoke <paramref name="callback"/> when the reader is ready.
        /// </summary>
        public Result WithDataReader(string sqlCommand, Action<IDataReader> callback)
        {
            using var dbConnection = GetDbConnection();
            using var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sqlCommand;

            try
            {
                dbConnection.Open();
                using var dataReader = dbCommand.ExecuteReader();
                callback?.Invoke(dataReader);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        }
    }

    /*
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
     */
}
