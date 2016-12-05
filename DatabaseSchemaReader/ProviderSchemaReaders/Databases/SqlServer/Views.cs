using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    internal class Views : SqlExecuter<DatabaseView>
    {
        private readonly string _viewName;

        public Views(string owner, string viewName, string[] additionalViewPropertyNames) : base(additionalViewPropertyNames)
        {
            _viewName = viewName;
            Owner = owner;
            Sql = @"select v.TABLE_SCHEMA, v.TABLE_NAME
{0}
from INFORMATION_SCHEMA.VIEWS v
{1}
where 
    (v.TABLE_SCHEMA = @Owner or (@Owner is null)) and 
    (v.TABLE_NAME = @TABLE_NAME or (@TABLE_NAME is null))
 order by 
    v.TABLE_SCHEMA, v.TABLE_NAME";

            AdditionalPropertiesJoin = string.Format(@"LEFT OUTER JOIN sys.views {0}  
              ON v.TABLE_SCHEMA = schema_name({0}.schema_id) AND 
                 v.TABLE_NAME = {0}.name", ADDITIONAL_INFO);
        }

        public IList<DatabaseView> Execute(DbConnection connection)
        {
            ExecuteDbReader(connection);
            return Result;
        }



        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "Owner", Owner);
            AddDbParameter(command, "TABLE_NAME", _viewName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var schema = record["TABLE_SCHEMA"].ToString();
            var name = record["TABLE_NAME"].ToString();
            var table = new DatabaseView
                        {
                            Name = name,
                            SchemaOwner = schema
                        };

            table.AddAdditionalProperties(record, _additionalPropertyNames);

            Result.Add(table);
        }
    }
}
