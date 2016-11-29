using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class Statistics : SqlExecuter<DatabaseStatistics>
        {
        private readonly string _tableName;

        public Statistics(string owner, string tableName)
        {
            _tableName = tableName;
            Owner = owner;
////            Sql = @" SELECT 
////     SchemaName = SCHEMA_NAME(t.schema_id),
////     TableName = t.name,
////     IndexName = ind.name,
////     ColumnName = col.name,
////     INDEX_TYPE = ind.type_desc,
////     IsPrimary = is_primary_key,
////     IsUnique = is_unique_constraint
////FROM 
////     sys.indexes ind 
////INNER JOIN 
////     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
////INNER JOIN 
////     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
////INNER JOIN 
////     sys.tables t ON ind.object_id = t.object_id 
////WHERE 
////    (t.name = @TableName OR @TableName IS NULL) AND 
////    (SCHEMA_NAME(t.schema_id) = @schemaOwner OR @schemaOwner IS NULL) AND 
////	 t.is_ms_shipped = 0 
////ORDER BY 
////     t.name, ind.name, col.name";

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
            var name = record.GetString("StatisticsName");
            var statistics = Result.FirstOrDefault(f => f.Name == name && f.SchemaOwner == schema && f.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            if (statistics == null)
            {
                statistics = new DatabaseStatistics
                {
                    SchemaOwner = schema,
                    TableName = tableName,
                    Name = name,
                };
                Result.Add(statistics);
            }
            var colName = record.GetString("ColumnName");
            if (string.IsNullOrEmpty(colName)) return;

            var col = new DatabaseColumn
            {
                Name = colName,
            };
            statistics.Columns.Add(col);

        }

        public IList<DatabaseStatistics> Execute(DbConnection dbConnection)
        {
            ExecuteDbReader(dbConnection);
            return Result;
        }
    }
}
