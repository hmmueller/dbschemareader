﻿using System.Collections.Generic;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.Databases.Firebird;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    class FirebirdAdapter : ReaderAdapter
    {
        //reference: http://www.alberton.info/firebird_sql_meta_info.html
        //http://firebirdsql.org/file/documentation/reference_manuals/fblangref25-en/html/fblangref25-appx04-systables.html


        public FirebirdAdapter(SchemaParameters schemaParameters, int commandTimeout) : base(schemaParameters, commandTimeout)
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

        public override IList<DatabaseConstraint> CheckConstraints(string tableName)
        {
            return new CheckConstraints(Owner, tableName, AdditionalCheckConstraintProperties, CommandTimeout).Execute(DbConnection);
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

        public override IList<DatabaseStoredProcedure> StoredProcedures(string name)
        {
            return new StoredProcedures(Owner, AdditionalStoredProcedureProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseFunction> Functions(string name)
        {
            return new Functions(name, AdditionalFunctionProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseArgument> ProcedureArguments(string name)
        {
            return new ProcedureArguments(Owner, AdditionalProcedureArgumentProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseSequence> Sequences(string name)
        {
            return new Sequences(AdditionalSequenceProperties, CommandTimeout).Execute(DbConnection);
        }

        public override IList<DatabaseUser> Users()
        {
            return new Users(AdditionalUserProperties, CommandTimeout).Execute(DbConnection);
        }
    }
}