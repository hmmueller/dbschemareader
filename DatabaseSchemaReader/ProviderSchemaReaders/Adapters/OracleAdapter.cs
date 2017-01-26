using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.Databases.Oracle;
using System.Collections.Generic;
using DatabaseSchemaReader.ProviderSchemaReaders.ResultModels;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    class OracleAdapter : ReaderAdapter
    {
        public OracleAdapter(SchemaParameters schemaParameters) : base(schemaParameters)
        {
        }

        public override IList<DataType> DataTypes()
        {
            return new DataTypeList().Execute();
        }

        public override IList<DatabaseTable> Tables(string tableName)
        {
            return new Tables(Owner, tableName, AdditionalTableProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> Columns(string tableName)
        {
            return new Columns(Owner, tableName, AdditionalColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseConstraint> PrimaryKeys(string tableName)
        {
            return new Constraints(Owner, tableName, ConstraintType.PrimaryKey, AdditionalPrimaryKeyProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseConstraint> UniqueKeys(string tableName)
        {
            return new Constraints(Owner, tableName, ConstraintType.UniqueKey, AdditionalUniqueKeyProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseConstraint> ForeignKeys(string tableName)
        {
            return new Constraints(Owner, tableName, ConstraintType.ForeignKey, AdditionalForeignKeyProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> ComputedColumns(string tableName)
        {
            return new ComputedColumns(Owner, tableName, AdditionalComputedColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> IdentityColumns(string tableName)
        {
            return new IdentityColumns(Owner, tableName, AdditionalIdentityColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseIndex> Indexes(string tableName)
        {
            return new Indexes(Owner, tableName, AdditionalIndexProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseTrigger> Triggers(string tableName)
        {
            return new Triggers(Owner, tableName, AdditionalTriggerProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseTable> ColumnDescriptions(string tableName)
        {
            return new ColumnDescriptions(Owner, tableName, AdditionalColumnDescriptionProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseTable> TableDescriptions(string tableName)
        {
            return new TableDescriptions(Owner, tableName, AdditionalTableDescriptionProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseConstraint> CheckConstraints(string tableName)
        {
            return new CheckConstraints(Owner, tableName, AdditionalCheckConstraintProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseView> Views(string viewName)
        {
            return new Views(Owner, viewName, AdditionalViewProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> ViewColumns(string viewName)
        {
            return new ViewColumns(Owner, viewName, AdditionalViewColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabasePackage> Packages(string name)
        {
            return new Packages(Owner, name, AdditionalPackageProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseStoredProcedure> StoredProcedures(string name)
        {
            return new StoredProcedures(Owner, name, AdditionalStoredProcedureProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseFunction> Functions(string name)
        {
            return new Functions(Owner, AdditionalFunctionProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseArgument> ProcedureArguments(string name)
        {
            return new ProcedureArguments(Owner, name, AdditionalProcedureArgumentProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<ProcedureSource> ProcedureSources(string name)
        {
            return new ProcedureSources(Owner, name, AdditionalProcedureSourceProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseSequence> Sequences(string name)
        {
            return new Sequences(Owner, AdditionalSequenceProperties, CommandTimeout).Execute(DbConnection);
        }

        public override IList<DatabaseUser> Users()
        {
            return new Users(AdditionalUserProperties, CommandTimeout).Execute(DbConnection);
        }

        public override void PostProcessing(DatabaseTable databaseTable)
        {
            if (databaseTable == null) return;
            //look at Oracle triggers which suggest the primary key id is autogenerated (in SQLServer terms, Identity)
            LookForAutoGeneratedId(databaseTable);
        }

        private static void LookForAutoGeneratedId(DatabaseTable databaseTable)
        {
            var pk = databaseTable.PrimaryKeyColumn;
            if (pk == null) return;
            if (Databases.Oracle.Conversion.LooksLikeAutoNumberDefaults(pk.DefaultValue))
            {
                //Oracle 12c default values from sequence
                pk.IsAutoNumber = true;
                return;
            }
            var match = OracleSequenceTrigger.FindTrigger(databaseTable);
            if (match != null) pk.IsAutoNumber = true;
        }
    }
}