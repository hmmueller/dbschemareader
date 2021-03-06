﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SQLite
{
    internal class Views : SqlExecuter<DatabaseView>
    {
        private readonly string _viewName;

        public Views(string viewName, string[] additionalViewPropertyNames, int commandTimeout) : base(additionalViewPropertyNames, commandTimeout)
        {
            _viewName = viewName;
            Sql = @"SELECT name, sql FROM sqlite_master
WHERE type='view' AND
    (name = @NAME or (@NAME is null))
ORDER BY name";
        }

        public IList<DatabaseView> Execute(DbConnection connection)
        {
            ExecuteDbReader(connection);
            return Result;
        }

        protected override void AddParameters(DbCommand command)
        {

            AddDbParameter(command, "NAME", _viewName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var name = record["name"].ToString();
            var table = new DatabaseView
                        {
                            Name = name,
                            SchemaOwner = null,
							Sql = record.GetString("sql"),
                        };

            Result.Add(table);
        }
    }
}
