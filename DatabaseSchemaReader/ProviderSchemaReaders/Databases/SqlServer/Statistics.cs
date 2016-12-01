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

        public Statistics(string owner, string tableName, string[] additionalStatisticsProperties) : base(additionalStatisticsProperties)
        {
            _tableName = tableName;
            Owner = owner;
            Sql = @"SELECT
    SchemaName = SCHEMA_NAME(t.schema_id),
    TableName = t.name,
    StatisticsName = st.name,
    ColumnName = col.name
FROM
    sys.stats st
INNER JOIN
    sys.stats_columns stc ON st.object_id = stc.object_id and st.stats_id = stc.stats_id
INNER JOIN 
    sys.columns col ON stc.object_id = col.object_id and stc.column_id = col.column_id 
INNER JOIN 
    sys.tables t ON st.object_id = t.object_id 
WHERE 
    (t.name = @TableName OR @TableName IS NULL) AND 
    (SCHEMA_NAME(t.schema_id) = @schemaOwner OR @schemaOwner IS NULL) AND 
        t.is_ms_shipped = 0 
ORDER BY 
        t.name, st.name, col.name
";

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
            if (string.IsNullOrEmpty(colName))
                return;

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
