﻿using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.Databases.PostgreSql;
using DatabaseSchemaReader.SqlGen.PostgreSql;
using System;
using System.Collections.Generic;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    class PostgreSqlAdapter : ReaderAdapter
    {
        public PostgreSqlAdapter(SchemaParameters schemaParameters) : base(schemaParameters)
        {
        }

        public override IList<DataType> DataTypes()
        {
            return new DataTypeList().Execute();
        }

        public override IList<DatabaseTable> Tables(string tableName)
        {
            return new Tables(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseColumn> Columns(string tableName)
        {
            return new Columns(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseConstraint> PrimaryKeys(string tableName)
        {
            return new Constraints(Parameters.Owner, tableName, ConstraintType.PrimaryKey)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseConstraint> UniqueKeys(string tableName)
        {
            return new Constraints(Parameters.Owner, tableName, ConstraintType.UniqueKey)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseConstraint> ForeignKeys(string tableName)
        {
            return new Constraints(Parameters.Owner, tableName, ConstraintType.ForeignKey)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseIndex> Indexes(string tableName)
        {
            return new Indexes(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseTrigger> Triggers(string tableName)
        {
            return new Triggers(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseConstraint> CheckConstraints(string tableName)
        {
            return new CheckConstraints(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseTable> ColumnDescriptions(string tableName)
        {
            return new ColumnDescriptions(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseTable> TableDescriptions(string tableName)
        {
            return new TableDescriptions(Parameters.Owner, tableName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseView> Views(string viewName)
        {
            return new Views(Parameters.Owner, viewName)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseFunction> Functions(string name)
        {
            return new Functions(Parameters.Owner)
                .Execute(Parameters.DbConnection);
        }

        public override IList<DatabaseArgument> ProcedureArguments(string name)
        {
            return new ProcedureArguments(Parameters.Owner, name)
                .Execute(Parameters.DbConnection);
        }
        public override IList<DatabaseUser> Users()
        {
            return new Users().Execute(Parameters.DbConnection);
        }

        public override void PostProcessing(DatabaseTable databaseTable)
        {
            if (databaseTable == null) return;
            //the devart providers GetSchema are a little weird so we fix them up here
            var typeWriter = new DataTypeWriter();

            foreach (var databaseColumn in databaseTable.Columns)
            {
                var santizedType = typeWriter.WriteDataType(databaseColumn);
                //all the different native types are reworked
                if ((santizedType.StartsWith("VARCHAR", StringComparison.OrdinalIgnoreCase)
                    || santizedType.StartsWith("CHAR", StringComparison.OrdinalIgnoreCase)))
                {
                    if (databaseColumn.Length == -1 && databaseColumn.Precision > 0)
                    {
                        databaseColumn.Length = databaseColumn.Precision;
                        databaseColumn.Precision = -1;
                    }
                }
                if ((santizedType.StartsWith("NUMERIC", StringComparison.OrdinalIgnoreCase)
                     || santizedType.StartsWith("DECIMAL", StringComparison.OrdinalIgnoreCase)
                     || santizedType.StartsWith("INTEGER", StringComparison.OrdinalIgnoreCase)))
                {
                    if (databaseColumn.Length > 0 && databaseColumn.Precision == -1)
                    {
                        databaseColumn.Precision = databaseColumn.Length;
                        databaseColumn.Length = -1;
                    }
                }
                //if it's a varchar or char, and the length is -1 but the precision is positive, swap them
                //and vice versa for numerics.

                var defaultValue = databaseColumn.DefaultValue;
                if (!string.IsNullOrEmpty(defaultValue) && defaultValue.StartsWith("nextval('", StringComparison.OrdinalIgnoreCase))
                {
                    databaseColumn.IsAutoNumber = true;
                    databaseColumn.IsPrimaryKey = true;
                }
                //if defaultValue looks like the nextval from a sequence, it's a pk
                //change the type to serial (or bigserial), ensure it's the primary key
            }
        }
    }
}