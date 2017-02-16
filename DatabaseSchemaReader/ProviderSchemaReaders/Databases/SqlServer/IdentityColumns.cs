using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class IdentityColumns : SqlExecuter<DatabaseColumn>
    {
        private readonly string _tableName;

        public IdentityColumns(string owner, string tableName, string[] additionalIdentityColumnPropertyNames, int commandTimeout) : base(additionalIdentityColumnPropertyNames, commandTimeout)
        {
            _tableName = tableName;
            Owner = owner;
            Sql = @"SELECT 
SchemaOwner = s.name, 
TableName = o.name, 
ColumnName = {ai}.name,
IdentitySeed = seed_value,
IdentityIncrement = increment_value
{0}
FROM sys.identity_columns {ai}
INNER JOIN sys.all_objects o ON {ai}.object_id = o.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
WHERE 
(o.name = @TableName OR @TableName IS NULL) AND 
(s.name = @schemaOwner OR @schemaOwner IS NULL) AND 
o.type= 'U' 
ORDER BY o.name, {ai}.name".Replace("{ai}", ADDITIONAL_INFO);
        }

        public IList<DatabaseColumn> Execute(DbConnection connection)
        {
            ExecuteDbReader(connection);
            return Result;
        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "schemaOwner", Owner);
            AddDbParameter(command, "TableName", _tableName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var schema = record.GetString("SchemaOwner");
            var tableName = record.GetString("TableName");
            var columnName = record.GetString("ColumnName");
            var seed = record.GetNullableLong("IdentitySeed").GetValueOrDefault();
            var increment = record.GetNullableLong("IdentityIncrement").GetValueOrDefault();
            var column = new DatabaseColumn
            {
                SchemaOwner = schema,
                TableName = tableName,
                Name = columnName,
                IsAutoNumber = true,
                IdentityDefinition = new DatabaseColumnIdentity { IdentityIncrement = increment, IdentitySeed = seed },
            };

            column.AddAdditionalProperties(record, _additionalPropertyNames);

            Result.Add(column);
        }
    }
}
