﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class CheckConstraints : SqlExecuter<DatabaseConstraint>
    {
        private readonly string _tableName;

        public CheckConstraints(string owner, string tableName, string[] additionalCheckConstraintPropertyNames, int? commandTimeout) : base(additionalCheckConstraintPropertyNames, commandTimeout)
        {
            _tableName = tableName;
            Owner = owner;
            //information_schema.check_constraints doesn't have table, so we join to table constraints
            Sql = @"SELECT 
cons.constraint_name, 
cons.constraint_schema,
cons.table_name, 
cons2.check_clause AS Expression
{0}
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS cons
INNER JOIN INFORMATION_SCHEMA.CHECK_CONSTRAINTS AS cons2
 ON cons2.constraint_catalog = cons.constraint_catalog AND
  cons2.constraint_schema = cons.constraint_schema AND
  cons2.constraint_name = cons.constraint_name
{1}
WHERE 
    (cons.table_name = @tableName OR @tableName IS NULL) AND 
    (cons.constraint_catalog = @schemaOwner OR @schemaOwner IS NULL) AND 
     cons.constraint_type = 'CHECK'
ORDER BY cons.table_name, cons.constraint_name";

            // TODO OK?????
            AdditionalPropertiesJoin = @"LEFT OUTER JOIN sys.check_constraints {ai} ON
  cons.table_name = object_name({ai}.object_id) AND
  cons2.constraint_schema = schema_name({ai}.schema_id) AND
  cons2.constraint_name = {ai}.name".Replace("{ai}", ADDITIONAL_INFO);
        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "schemaOwner", Owner);
            AddDbParameter(command, "tableName", _tableName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var schema = record.GetString("constraint_schema");
            var tableName = record.GetString("table_name");
            var name = record.GetString("constraint_name");
            var expression = record.GetString("Expression");
            var constraint = new DatabaseConstraint
            {
                ConstraintType = ConstraintType.Check,
                Expression = expression,
                SchemaOwner = schema,
                TableName = tableName,
                Name = name,
            };

            constraint.AddAdditionalProperties(record, _additionalPropertyNames);

            Result.Add(constraint);
        }

        public IList<DatabaseConstraint> Execute(DbConnection dbConnection)
        {
            ExecuteDbReader(dbConnection);
            return Result;
        }
    }
}
