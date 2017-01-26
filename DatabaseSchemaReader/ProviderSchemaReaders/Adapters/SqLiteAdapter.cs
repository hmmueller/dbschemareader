using System.Collections.Generic;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.Databases.SQLite;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    class SqLiteAdapter : ReaderAdapter
    {
        public SqLiteAdapter(SchemaParameters schemaParameters) : base(schemaParameters)
        {
        }
        public override IList<DataType> DataTypes()
        {
            return new DataTypeList().Execute();
        }

        public override IList<DatabaseTable> Tables(string tableName)
        {
            return new Tables(tableName, AdditionalTableProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> Columns(string tableName)
        {
            return new Columns(tableName, AdditionalColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }


        public override IList<DatabaseIndex> Indexes(string tableName)
        {
            return new Indexes(tableName, AdditionalIndexProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseConstraint> ForeignKeys(string tableName)
        {
            return new Constraints(tableName, AdditionalForeignKeyProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseTrigger> Triggers(string tableName)
        {
            return new Triggers(tableName, AdditionalTriggerProperties, CommandTimeout)
                .Execute(DbConnection);
        }


        public override IList<DatabaseView> Views(string viewName)
        {
            return new Views(viewName, AdditionalViewProperties, CommandTimeout)
               .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> ViewColumns(string viewName)
        {
            return new ViewColumns(viewName, AdditionalColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }
    }
}