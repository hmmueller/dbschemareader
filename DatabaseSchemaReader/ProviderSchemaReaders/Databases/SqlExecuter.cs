using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases
{

    internal abstract class SqlExecuter<T> : SqlExecuter where T : new()
    {
        protected SqlExecuter(string[] additionalPropertyNames, int? commandTimeout) : base(additionalPropertyNames, commandTimeout) {
        }

        protected List<T> Result { get; } = new List<T>();
    }

    abstract class SqlExecuter
    {
        public const string ADDITIONAL_INFO = "ADDITIONAL_INFO";
        protected readonly string[] _additionalPropertyNames;
        protected readonly int? _commandTimeout;

        protected SqlExecuter(string[] additionalPropertyNames, int? commandTimeout) {
            _additionalPropertyNames = additionalPropertyNames;
            _commandTimeout = commandTimeout;
        }

        public string Sql { get; set; }

        public string Owner { get; set; }

        protected string AdditionalPropertiesJoin
        {
            private get; set;
        }

        protected void ExecuteDbReader(DbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var sql = FormatSql(_additionalPropertyNames);
            Trace.WriteLine($"Sql: {sql}");
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (_commandTimeout.HasValue) 
                {
                    cmd.CommandTimeout = _commandTimeout.Value;
                }
                AddParameters(cmd);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Mapper(dr);
                    }
                }
            }
        }

        private string FormatSql(string[] additionalProperties) {
            return string.Format(Sql, // must contain {0} and {1}
                additionalProperties == null ? "" : string.Join("", additionalProperties.Select(c => ", " + ADDITIONAL_INFO + "." + c).ToArray()),
                additionalProperties == null ? "" : AdditionalPropertiesJoin);
        }

        protected static DbParameter AddDbParameter(DbCommand command, string parameterName, object value, DbType? dbType = null)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            if (dbType.HasValue) parameter.DbType = dbType.Value; //SqlServerCe needs this
            command.Parameters.Add(parameter);
            return parameter;
        }

        protected abstract void AddParameters(DbCommand command);

        protected abstract void Mapper(IDataRecord record);
    }
}
