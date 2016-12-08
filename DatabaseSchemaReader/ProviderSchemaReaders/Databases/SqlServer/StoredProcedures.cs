using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    internal class StoredProcedures : SqlExecuter<DatabaseStoredProcedure>
    {
        private readonly string _name;

        public StoredProcedures(string owner, string name, string[] additionalStoredProcedurePropertyNames) : base(additionalStoredProcedurePropertyNames)
        {
            _name = name;
            Owner = owner;
            Sql = @"SELECT
  SPECIFIC_SCHEMA,
  SPECIFIC_NAME,
  ROUTINE_DEFINITION
  {0}
FROM INFORMATION_SCHEMA.ROUTINES {ai}
WHERE 
    (SPECIFIC_SCHEMA = @Owner OR (@Owner IS NULL))
    AND (SPECIFIC_NAME = @Name OR (@Name IS NULL))
    AND (ROUTINE_TYPE = 'PROCEDURE')
    AND ObjectProperty (Object_Id ({ai}.ROUTINE_NAME), 'IsMSShipped') = 0 and
        (
            select 
                major_id 
            from 
                sys.extended_properties 
            where 
                major_id = object_id({ai}.ROUTINE_NAME) and 
                minor_id = 0 and 
                class = 1 and 
                name = N'microsoft_database_tools_support'
        ) is null
ORDER BY SPECIFIC_SCHEMA, SPECIFIC_NAME".Replace("{ai}", ADDITIONAL_INFO);

        }

        public IList<DatabaseStoredProcedure> Execute(DbConnection connection)
        {
            ExecuteDbReader(connection);
            return Result;
        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "Owner", Owner);
            AddDbParameter(command, "Name", _name);
        }

        protected override void Mapper(IDataRecord record)
        {
            var owner = record.GetString("SPECIFIC_SCHEMA");
            var name = record.GetString("SPECIFIC_NAME");
            var sql = record.GetString("ROUTINE_DEFINITION");
            var sproc = new DatabaseStoredProcedure
            {
                SchemaOwner = owner,
                Name = name,
                Sql = sql
            };

            sproc.AddAdditionalProperties(record, _additionalPropertyNames);

            Result.Add(sproc);
        }
    }
}
