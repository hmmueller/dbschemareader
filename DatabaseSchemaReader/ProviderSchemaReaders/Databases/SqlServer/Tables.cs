using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    internal class Tables : SqlExecuter<DatabaseTable>
    {
        private readonly string _tableName;

        public Tables(string owner, string tableName, string[] additionalTablePropertyNames, int? commandTimeout) : base(additionalTablePropertyNames, commandTimeout)
        {
            _tableName = tableName;
            Owner = owner;
            Sql = @"select t.TABLE_SCHEMA, t.TABLE_NAME 
{0}
from INFORMATION_SCHEMA.TABLES t
{1}
where 
    (t.TABLE_SCHEMA = @Owner or (@Owner is null)) and 
    (t.TABLE_NAME = @TABLE_NAME or (@TABLE_NAME is null)) and 
    t.TABLE_TYPE = 'BASE TABLE'
 order by 
    t.TABLE_SCHEMA, t.TABLE_NAME";

            AdditionalPropertiesJoin = @"LEFT OUTER JOIN sys.tables {ai}
              ON t.TABLE_SCHEMA = schema_name({ai}.schema_id) AND 
                 t.TABLE_NAME = {ai}.name".Replace("{ai}", ADDITIONAL_INFO);
        }

        public IList<DatabaseTable> Execute(DbConnection connection)
        {
            ExecuteDbReader(connection);
            return Result;
        }



        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "Owner", Owner);
            AddDbParameter(command, "TABLE_NAME", _tableName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var schema = record["TABLE_SCHEMA"].ToString();
            var name = record["TABLE_NAME"].ToString();
            var table = new DatabaseTable
                        {
                            Name = name,
                            SchemaOwner = schema
                        };

            table.AddAdditionalProperties(record, _additionalPropertyNames);

            Result.Add(table);
        }
    }
}
