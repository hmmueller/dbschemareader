using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class DefaultConstraints : SqlExecuter<DatabaseConstraint>
    {
        private readonly string _tableName;

        public DefaultConstraints(string owner, string tableName, string[] additionalDefaultConstraintPropertyNames) : base(additionalDefaultConstraintPropertyNames)
        {
            _tableName = tableName;
            Owner = owner;
            Sql = @"SELECT 
    s.name AS SCHEMA_NAME, 
    o.name AS TABLE_NAME,
    c.name AS COLUMN_NAME,
    {ai}.name AS CONSTRAINT_NAME,
    {ai}.[definition] AS EXPRESSION
    {0}
FROM sys.[default_constraints] {ai}
INNER JOIN sys.objects o
    ON o.object_id = {ai}.parent_object_id
INNER JOIN sys.columns c
    ON c.default_object_id = {ai}.object_id
INNER JOIN  sys.schemas s
    ON s.schema_id = o.schema_id
WHERE 
    (o.name = @tableName OR @tableName IS NULL) AND 
    (s.name = @schemaOwner OR @schemaOwner IS NULL) AND 
o.type= 'U' 
ORDER BY s.name, o.name, c.name, {ai}.name".Replace("{ai}", ADDITIONAL_INFO);

        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "schemaOwner", Owner);
            AddDbParameter(command, "tableName", _tableName);
        }

        private DatabaseConstraint FindConstraint(string name, string constraintTableName, string schemaName)
        {
            return Result.Find(f => f.Name == name && f.TableName == constraintTableName && f.SchemaOwner == schemaName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var schema = record.GetString("SCHEMA_NAME");
            var tableName = record.GetString("TABLE_NAME");
            var name = record.GetString("CONSTRAINT_NAME");

            DatabaseConstraint constraint = FindConstraint(name, tableName, schema);
            if (constraint == null)
            {
                constraint = new DatabaseConstraint
                {
                    ConstraintType = ConstraintType.Default,
                    SchemaOwner = schema,
                    TableName = tableName,
                    Name = name,
                    Expression = record.GetString("EXPRESSION"),
                };
                Result.Add(constraint);
            }

            constraint.AddAdditionalProperties(record, _additionalPropertyNames);

            var columnName = record.GetString("COLUMN_NAME");
            constraint.Columns.Add(columnName);
        }

        public IList<DatabaseConstraint> Execute(DbConnection dbConnection)
        {
            ExecuteDbReader(dbConnection);
            return Result;
        }
    }
}
