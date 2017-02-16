using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.Converters.KeyMaps;
using DatabaseSchemaReader.ProviderSchemaReaders.Converters.RowConverters;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    internal class Columns : SqlExecuter<DatabaseColumn>
    {
        private readonly string _tableName;
        private readonly ColumnRowConverter _converter;

        public Columns(string owner, string tableName, string[] additionalPropertyNames, int commandTimeout) : base(additionalPropertyNames, commandTimeout)
        {
            _tableName = tableName;
            Owner = owner;
            Sql = @"select c.TABLE_SCHEMA, 
c.TABLE_NAME, 
c.COLUMN_NAME, 
c.ORDINAL_POSITION, 
c.COLUMN_DEFAULT, 
c.IS_NULLABLE, 
c.DATA_TYPE, 
c.CHARACTER_MAXIMUM_LENGTH, 
c.NUMERIC_PRECISION, 
c.NUMERIC_SCALE, 
c.DATETIME_PRECISION 
{0}
from INFORMATION_SCHEMA.COLUMNS c
JOIN INFORMATION_SCHEMA.TABLES t 
 ON c.TABLE_SCHEMA = t.TABLE_SCHEMA AND 
    c.TABLE_NAME = t.TABLE_NAME
{1}
where 
    (c.TABLE_SCHEMA = @Owner or (@Owner is null)) and 
    (c.TABLE_NAME = @TableName or (@TableName is null)) AND
    TABLE_TYPE = 'BASE TABLE'
 order by 
    c.TABLE_SCHEMA, c.TABLE_NAME, ORDINAL_POSITION";

            AdditionalPropertiesJoin = @"LEFT OUTER JOIN sys.tables syst 
              ON c.TABLE_SCHEMA = schema_name(syst.schema_id) AND 
                 c.TABLE_NAME = syst.name
           LEFT OUTER JOIN sys.columns {ai} ON 
                 syst.object_id = {ai}.object_id AND
                 c.COLUMN_NAME = {ai}.Name".Replace("{ai}", ADDITIONAL_INFO);

            var keyMap = new ColumnsKeyMap();
            _converter = new ColumnRowConverter(keyMap);
        }

        public IList<DatabaseColumn> Execute(DbConnection connection)
        {
            ExecuteDbReader(connection);
            return Result;
        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "Owner", Owner);
            AddDbParameter(command, "TableName", _tableName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var col = _converter.Convert(record, _additionalPropertyNames);
            Result.Add(col);
        }
    }
}
