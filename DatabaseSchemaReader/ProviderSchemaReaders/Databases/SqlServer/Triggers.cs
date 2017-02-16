using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class Triggers : SqlExecuter<DatabaseTrigger>
    {
        private readonly string _tableName;
        public Triggers(string owner, string tableName, string[] additionalTriggerPropertyNames, int commandTimeout) : base(additionalTriggerPropertyNames, commandTimeout)
        {
            _tableName = tableName;
            Owner = owner;

            Sql = @"SELECT
  {ai}.name AS TRIGGER_NAME,
  SCHEMA_NAME(parent.schema_id) AS TRIGGER_SCHEMA,
  SCHEMA_NAME(parent.schema_id) AS TABLE_SCHEMA,
  parent.name AS TABLE_NAME,
  OBJECTPROPERTY({ai}.object_id, 'ExecIsUpdateTrigger') AS IS_UPDATE,
  OBJECTPROPERTY({ai}.object_id, 'ExecIsDeleteTrigger') AS IS_DELETE,
  OBJECTPROPERTY({ai}.object_id, 'ExecIsInsertTrigger') AS IS_INSERT,
  OBJECTPROPERTY({ai}.object_id, 'ExecIsAfterTrigger') AS IS_AFTER,
  {ai}.is_instead_of_trigger AS IS_INSTEADOF,
  {ai}.is_disabled AS IS_DISABLED,
  OBJECT_DEFINITION({ai}.object_id) AS TRIGGER_BODY
  {0}
FROM sys.triggers AS {ai}
INNER JOIN sys.tables AS parent
  ON {ai}.parent_id = parent.object_id
  {1}
WHERE (SCHEMA_NAME(parent.schema_id) = @Owner or (@Owner is null)) 
  AND (parent.name = @TABLE_NAME or (@TABLE_NAME is null)) 
".Replace("{ai}", ADDITIONAL_INFO)
;
            AdditionalPropertiesJoin = "";

        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "Owner", Owner);
            AddDbParameter(command, "TABLE_NAME", _tableName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var trigger = new DatabaseTrigger
            {
                Name = record.GetString("TRIGGER_NAME"),
                SchemaOwner = record.GetString("TABLE_SCHEMA"),
                TableName = record.GetString("TABLE_NAME"),
                TriggerBody = record.GetString("TRIGGER_BODY"),
            };
            var triggerEvents = new List<string>();
            if (record.GetBoolean("IS_UPDATE"))
            {
                triggerEvents.Add("UPDATE");
            }
            if (record.GetBoolean("IS_DELETE"))
            {
                triggerEvents.Add("DELETE");
            }
            if (record.GetBoolean("IS_INSERT"))
            {
                triggerEvents.Add("INSERT");
            }
            trigger.TriggerEvent = string.Join(",", triggerEvents.ToArray());
            if (record.GetBoolean("IS_AFTER"))
            {
                trigger.TriggerType = "AFTER";
            }
            if (record.GetBoolean("IS_INSTEADOF"))
            {
                trigger.TriggerType = "INSTEAD OF";
            }
            Result.Add(trigger);
        }

        public IList<DatabaseTrigger> Execute(DbConnection dbConnection)
        {
            ExecuteDbReader(dbConnection);
            return Result;
        }
    }
}
