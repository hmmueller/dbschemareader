using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer;
using DatabaseSchemaReader.ProviderSchemaReaders.ResultModels;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    class SqlServerAdapter : ReaderAdapter
    {
        private bool? _isAzureSqlDatabase;
        private const int SqlServerEngineAzure = 5;

        public SqlServerAdapter(SchemaParameters schemaParameters, int commandTimeout) : base(schemaParameters, commandTimeout)
        {
        }

        /// <summary>
        /// returns the SqlServer version (10 is SqlServer 2008).
        /// </summary>
        /// <param name="connection">The connection (must be OPEN).</param>
        /// <returns>9 is SqlServer 2005, 10 is SqlServer 2008, 11 is SqlServer 2012, 12 is SqlServer 2014</returns>
        public int SqlServerVersion(DbConnection connection)
        {
            //an open connection contains a server version
            //SqlServer 2014 = 12.00.2000
            //SqlAzure (as of 201407 it's SqlServer 2012) = 11.0.9216.62
            //SqlServer 2012 SP2 = 11.0.5058.0
            //SqlServer 2008 R2 SP2 = 10.50.4000.0
            //2005 = 9.00.5000.00 , 2000 = 8.00.2039
            int serverVersion;
            var version = connection.ServerVersion;
            if (string.IsNullOrEmpty(version) || !int.TryParse(version.Substring(0, 2), out serverVersion))
            {
                serverVersion = 9; //SqlServer 2005
            }
            return serverVersion;
        }

        public bool IsAzureSqlDatabase
        {
            get
            {
                if (!_isAzureSqlDatabase.HasValue)
                {
                    var conn = DbConnection;
                    var serverVersion = SqlServerVersion(conn);

                    if (serverVersion < 11) //before SqlServer 2012, there was no cloud edition
                    {
                        _isAzureSqlDatabase = false;
                    }
                    else
                    {
                        using (var command = conn.CreateCommand())
                        {
                            //Database Engine edition of the instance of SQL Server installed on the server.
                            //1 = Personal or Desktop Engine (Not available for SQL Server 2005.)
                            //2 = Standard (This is returned for Standard and Workgroup.)
                            //3 = Enterprise (This is returned for Enterprise, Enterprise Evaluation, and Developer.)
                            //4 = Express (This is returned for Express, Express Edition with Advanced Services, and Windows Embedded SQL.)
                            //5 = SQL Database
                            //NB: in MONO this returns a SqlVariant, so the CAST is required
                            command.CommandText = "SELECT CAST(SERVERPROPERTY('EngineEdition') AS int)";
                            _isAzureSqlDatabase = (int)command.ExecuteScalar() == SqlServerEngineAzure;
                        }
                    }
                }
                return _isAzureSqlDatabase.Value;
            }
        }

        public override IList<DataType> DataTypes()
        {
            return new DataTypes().Execute();
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

        public override IList<ProcedureSource> ViewSources(string viewName)
        {
            return new ViewSources(Owner, viewName, AdditionalViewSourceProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> ViewColumns(string viewName)
        {
            return new ViewColumns(Owner, viewName, AdditionalViewColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> IdentityColumns(string tableName)
        {
            return new IdentityColumns(Owner, tableName, AdditionalIdentityColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseConstraint> CheckConstraints(string tableName)
        {
            return new CheckConstraints(Owner, tableName, AdditionalCheckConstraintProperties, CommandTimeout)
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

        public override IList<DatabaseConstraint> DefaultConstraints(string tableName)
        {
            return new DefaultConstraints(Owner, tableName, AdditionalDefaultConstraintProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseColumn> ComputedColumns(string tableName)
        {
            return new ComputedColumns(Owner, tableName, AdditionalComputedColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseIndex> Indexes(string tableName) 
            {
            return new Indexes(Owner, tableName, AdditionalIndexProperties, AdditionalIndexColumnProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseStatistics> Statistics(string tableName) 
            {
            return new Statistics(Owner, tableName, AdditionalStatisticsProperties, CommandTimeout)
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

        public override IList<DatabaseSequence> Sequences(string name)
        {
            return new Sequences(Owner, AdditionalSequenceProperties, CommandTimeout).Execute(DbConnection);
        }

        public override IList<DatabaseStoredProcedure> StoredProcedures(string name)
        {
            return new StoredProcedures(Owner, name, AdditionalStoredProcedureProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseFunction> Functions(string name)
        {
            return new Functions(Owner, name, AdditionalFunctionProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseArgument> ProcedureArguments(string name)
        {
            return new ProcedureArguments(Owner, name, AdditionalProcedureArgumentProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<ProcedureSource> ProcedureSources(string name)
        {
            return new ProcedureSources(Owner, null, AdditionalProcedureSourceProperties, CommandTimeout)
                .Execute(DbConnection);
        }

        public override IList<DatabaseUser> Users()
        {
            return new Users(AdditionalUserProperties, CommandTimeout).Execute(DbConnection);
        }

        public override SerializableAdditionalProperties TopLevelProperties() {
            return new TopLevelProperties(AdditionalTopLevelProperties, CommandTimeout).Execute(DbConnection);
        }

        public override void PostProcessing(DatabaseTable databaseTable)
        {
            if (databaseTable == null) return;
            //look at default values to see if uses a sequence
            LookForAutoGeneratedId(databaseTable);
        }

        private static void LookForAutoGeneratedId(DatabaseTable databaseTable)
        {
            var pk = databaseTable.PrimaryKeyColumn;
            if (pk == null) return;
            if (databaseTable.HasAutoNumberColumn) return;
            if (string.IsNullOrEmpty(pk.DefaultValue)) return;
            if (pk.DefaultValue.IndexOf("NEXT VALUE FOR ", StringComparison.OrdinalIgnoreCase) != -1)
                pk.IsAutoNumber = true;
        }
    }
}