using Ant0nRocket.Lib.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Ant0nRocket.Lib.Data
{
    /// <summary>
    /// Simple ADO.Net connection wrapper.
    /// This lib was designed to work with desktop apps, no async functions
    /// here, because in UI development it goes to hell.
    /// So DON'T use the class with web apps or apps with multiple connection strings!
    /// This class is only for apps where DI, EF and all of this huge stuff
    /// are overkill.
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
        uses _connectionString, do some job and then dispose the connection!
        Dont' try to mix reading and writing operations here!!!

        If you need to something mixed call WithDbConnection function and do what you want inside
        of a callback. Connection will be closed automatically!

        */

        private T GetDbConnection()
        {
            var dbConnection = new T();
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
                using var dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                callback?.Invoke(dataReader);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        }

        /// <summary>
        /// Executes multiple non-query SQL commands (INSERT, UPDATE, DELETE, etc.).
        /// Pay attension! All commands will be executed inside a single transaction! Or please, explain me,
        /// why you pack them all in one list? 😄
        /// But how to execute one command, you ask? Easy, provide a list with single element.
        /// If success - <see cref="Result{T}"/> of type int will be returned with int equals rows affected.
        /// </summary>
        public Result<int> ExecBatchNonQuery(List<(string commandText, object paramsObject)> batchPackage)
        {
            using var dbConnection = GetDbConnection();

            var rowsAffected = 0;

            try
            {
                dbConnection.Open();

                using var transaction = dbConnection.BeginTransaction();
                using var dbCommand = dbConnection.CreateCommand();
                dbCommand.Transaction = transaction;

                foreach (var (commandText, paramsObject) in batchPackage)
                {
                    dbCommand.Parameters.Clear(); // clean-up from previous use
                    dbCommand.CommandText = commandText;

                    if (paramsObject != null)
                        ApplyParameters(dbCommand, paramsObject);

                    rowsAffected += dbCommand.ExecuteNonQuery();
                }

                transaction.Commit();
                return Result<int>.Success(rowsAffected);
            }
            catch (Exception ex)
            {
                
                return Result<int>.Failure(ex);
            }
        }

        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = [];
        private static readonly object _propertyCacheLocker = new();

        private static void ApplyParameters(IDbCommand command, object paramsObject)
        {
            var paramsObjectType = paramsObject.GetType();
            if (!_propertyCache.TryGetValue(paramsObjectType, out var paramsObjectProperties))
            {
                lock (_propertyCacheLocker) // I don't want to use ConcurrentDictionary here, to heavy
                {
                    paramsObjectProperties = paramsObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    _propertyCache[paramsObjectType] = paramsObjectProperties;
                }
            }

            foreach (var property in paramsObjectProperties)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property.Name}";
                parameter.Value = property.GetValue(paramsObject) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }
    }
}
