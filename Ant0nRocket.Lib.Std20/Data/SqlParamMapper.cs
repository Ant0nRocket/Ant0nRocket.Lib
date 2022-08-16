using System.Collections.Generic;
using System.Data;

namespace Ant0nRocket.Lib.Std20.Data
{
    public class SqlParamMapper
    {
        public Dictionary<string, object> KeyValues { get; } = new();

        public string SqlCommandText { get; private set; }

        public SqlParamMapper(string sqlCommandText)
        {
            SqlCommandText = sqlCommandText;
        }

        public SqlParamMapper AddKeyValue(string key, object value, bool overrideExistingValues = false)
        {
            if (!KeyValues.ContainsKey(key))
            {
                KeyValues.Add(key, value);
            }
            else
            {
                if (overrideExistingValues)
                    KeyValues[key] = value;
            }

            return this;
        }

        public IDbCommand CreateDbCommand(IDbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = SqlCommandText;

            foreach (var kvp in KeyValues)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = kvp.Key;
                parameter.Value = kvp.Value;
                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}
