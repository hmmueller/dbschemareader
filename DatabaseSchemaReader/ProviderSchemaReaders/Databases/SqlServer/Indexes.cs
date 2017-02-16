using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class Indexes : SqlExecuter<DatabaseIndex>
    {
        private readonly string _tableName;
        private readonly string[] _additionalIndexColumnProperties;

        public Indexes(string owner, string tableName, string[] additionalIndexPropertyNames, string[] additionalIndexColumnProperties, int commandTimeout) 
            : base(
                  additionalIndexPropertyNames == null ? additionalIndexColumnProperties
                      : additionalIndexColumnProperties == null ? additionalIndexPropertyNames
                          : additionalIndexPropertyNames.Concat(additionalIndexColumnProperties).ToArray(), commandTimeout)
        {
            _additionalIndexColumnProperties = additionalIndexColumnProperties;

            _tableName = tableName;
            Owner = owner;
            Sql = @" SELECT 
     SchemaName = SCHEMA_NAME(t.schema_id),
     TableName = t.name,
     IndexName = {ai}.name,
     ColumnName = col.name,
     INDEX_TYPE = {ai}.type_desc,
     IsPrimary = is_primary_key,
     IsUnique = is_unique_constraint,
     IsIncluded = is_included_column
	 {0}
FROM 
     (select ind.*, ic.index_column_id, column_id, key_ordinal, partition_ordinal, is_descending_key, is_included_column from sys.indexes ind 
INNER JOIN 
     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id) as {ai}
INNER JOIN 
     sys.columns col ON {ai}.object_id = col.object_id and {ai}.column_id = col.column_id 
INNER JOIN 
     sys.tables t ON {ai}.object_id = t.object_id
WHERE 
    (t.name = @TableName OR @TableName IS NULL) AND 
    (SCHEMA_NAME(t.schema_id) = @schemaOwner OR @schemaOwner IS NULL) AND 
	 t.is_ms_shipped = 0 
ORDER BY 
     t.name, {ai}.name, col.name".Replace("{ai}", ADDITIONAL_INFO);

        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "schemaOwner", Owner);
            AddDbParameter(command, "TableName", _tableName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var schema = record.GetString("SchemaName");
            var tableName = record.GetString("TableName");
            var name = record.GetString("IndexName");
            var index = Result.FirstOrDefault(f => f.Name == name && f.SchemaOwner == schema && f.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            if (index == null)
            {
                index = new DatabaseIndex
                {
                    SchemaOwner = schema,
                    TableName = tableName,
                    Name = name,
                    IndexType = record.GetString("INDEX_TYPE"),
                    IsUnique = record.GetBoolean("IsUnique"),
                };
                if (record.GetBoolean("IsPrimary"))
                {
                    index.IndexType = "PRIMARY";
                }

                index.AddAdditionalProperties(record, _additionalPropertyNames);

                Result.Add(index);
            }
            var colName = record.GetString("ColumnName");
            var isIncluded = record.GetBoolean("IsIncluded");
            if (string.IsNullOrEmpty(colName)) return;

            var col = new DatabaseColumn
            {
                Name = colName,
                IsIncludeColumnInIndex = isIncluded
            };

            col.AddAdditionalProperties(record, _additionalIndexColumnProperties);

            index.Columns.Add(col);

        }

        public IList<DatabaseIndex> Execute(DbConnection dbConnection)
        {
            ExecuteDbReader(dbConnection);
            return Result;
        }
    }
}
