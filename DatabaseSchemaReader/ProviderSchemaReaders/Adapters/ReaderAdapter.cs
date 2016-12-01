using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.ResultModels;
using System.Collections.Generic;
using System.Data.Common;
using DatabaseSchemaReader.ProviderSchemaReaders.ConnectionContext;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    class ReaderAdapter
    {
        public readonly SchemaParameters Parameters;
        private ConnectionAdapter _connectionAdapter;

        public ReaderAdapter(SchemaParameters schemaParameters)
        {
            Parameters = schemaParameters;
        }

        /// <summary>
        /// Call this in a using block to share a connection over several calls.
        /// </summary>
        public virtual IConnectionAdapter CreateConnection()
        {
            if (_connectionAdapter != null)
            {
                //already have a connection, a wrapper to access it but won't be disposed
                return new NestedConnection(_connectionAdapter);
            }
            //top level connection
            _connectionAdapter = new ConnectionAdapter(Parameters);
            return _connectionAdapter;
        }

        protected DbConnection DbConnection
        {
            get
            {
                if (_connectionAdapter == null)
                {
                    _connectionAdapter = new ConnectionAdapter(Parameters);
                }
                return _connectionAdapter.DbConnection;
            }
        }

        public virtual string Owner
        {
            get
            {
                return Parameters.Owner;
            }
            set
            {
                Parameters.Owner = value;
            }
        }

        public IAdditionalProperties AdditionalParameters
        {
            get; set;
        }

        internal string[] AdditionalProcedureArgumentProperties => AdditionalParameters?.AdditionalProcedureArgumentPropertyNames;
        internal string[] AdditionalColumnProperties => AdditionalParameters?.AdditionalColumnPropertyNames;
        internal string[] AdditionalComputedColumnProperties => AdditionalParameters?.AdditionalComputedColumnPropertyNames;
        internal string[] AdditionalIdentityColumnProperties => AdditionalParameters?.AdditionalIdentityColumnPropertyNames;
        internal string[] AdditionalViewColumnProperties => AdditionalParameters?.AdditionalViewColumnPropertyNames;
        internal string[] AdditionalCheckConstraintProperties => AdditionalParameters?.AdditionalCheckConstraintPropertyNames;
        internal string[] AdditionalDefaultConstraintProperties => AdditionalParameters?.AdditionalDefaultConstraintPropertyNames;
        internal string[] AdditionalForeignKeyProperties => AdditionalParameters?.AdditionalForeignKeyPropertyNames;
        internal string[] AdditionalPrimaryKeyProperties => AdditionalParameters?.AdditionalPrimaryKeyPropertyNames;
        internal string[] AdditionalUniqueKeyProperties => AdditionalParameters?.AdditionalUniqueKeyPropertyNames;
        internal string[] AdditionalFunctionProperties => AdditionalParameters?.AdditionalFunctionPropertyNames;
        internal string[] AdditionalIndexColumnProperties => AdditionalParameters?.AdditionalIndexColumnPropertyNames;
        internal string[] AdditionalIndexProperties => AdditionalParameters?.AdditionalIndexPropertyNames;
        internal string[] AdditionalPackageProperties => AdditionalParameters?.AdditionalPackagePropertyNames;
        internal string[] AdditionalSequenceProperties => AdditionalParameters?.AdditionalSequencePropertyNames;
        internal string[] AdditionalStatisticsProperties => AdditionalParameters?.AdditionalStatisticsPropertyNames;
        internal string[] AdditionalStoredProcedureProperties => AdditionalParameters?.AdditionalStoredProcedurePropertyNames;
        internal string[] AdditionalColumnDescriptionProperties => AdditionalParameters?.AdditionalColumnDescriptionPropertyNames;
        internal string[] AdditionalTableDescriptionProperties => AdditionalParameters?.AdditionalTableDescriptionPropertyNames;
        internal string[] AdditionalTableProperties => AdditionalParameters?.AdditionalTablePropertyNames;
        internal string[] AdditionalTopLevelProperties => AdditionalParameters?.AdditionalTopLevelPropertyNames;
        internal string[] AdditionalTriggerProperties => AdditionalParameters?.AdditionalTriggerPropertyNames;
        internal string[] AdditionalUserProperties => AdditionalParameters?.AdditionalUserPropertyNames;
        internal string[] AdditionalViewProperties => AdditionalParameters?.AdditionalViewPropertyNames;
        internal string[] AdditionalProcedureSourceProperties => AdditionalParameters?.AdditionalProcedureSourcePropertyNames;
        internal string[] AdditionalViewSourceProperties => AdditionalParameters?.AdditionalViewSourcePropertyNames;

        public virtual IList<DataType> DataTypes()
        {
            return new List<DataType>();
        }

        public virtual IList<DatabaseTable> Tables(string tableName)
        {
            return new List<DatabaseTable>();
        }

        public virtual IList<DatabaseColumn> Columns(string tableName)
        {
            return new List<DatabaseColumn>();
        }

        public virtual IList<DatabaseView> Views(string viewName)
        {
            return new List<DatabaseView>();
        }

        public virtual IList<ProcedureSource> ViewSources(string viewName)
        {
            return new List<ProcedureSource>();
        }

        public virtual IList<DatabaseColumn> ViewColumns(string viewName)
        {
            return new List<DatabaseColumn>();
        }

        public virtual IList<DatabaseColumn> IdentityColumns(string tableName)
        {
            return new List<DatabaseColumn>();
        }

        public virtual IList<DatabaseColumn> ComputedColumns(string tableName)
        {
            return new List<DatabaseColumn>();
        }

        public virtual IList<DatabaseConstraint> CheckConstraints(string tableName)
        {
            return new List<DatabaseConstraint>();
        }

        public virtual IList<DatabaseConstraint> DefaultConstraints(string tableName)
        {
            return new List<DatabaseConstraint>();
        }

        public virtual IList<DatabaseConstraint> PrimaryKeys(string tableName)
        {
            return new List<DatabaseConstraint>();
        }

        public virtual IList<DatabaseConstraint> UniqueKeys(string tableName)
        {
            return new List<DatabaseConstraint>();
        }

        public virtual IList<DatabaseConstraint> ForeignKeys(string tableName)
        {
            return new List<DatabaseConstraint>();
        }

        public virtual IList<DatabaseTable> TableDescriptions(string tableName)
        {
            return new List<DatabaseTable>();
        }

        public virtual IList<DatabaseTable> ColumnDescriptions(string tableName)
        {
            return new List<DatabaseTable>();
        }

        public virtual IList<DatabaseIndex> Indexes(string tableName)
        {
            return new List<DatabaseIndex>();
        }

        public virtual IList<DatabaseStatistics> Statistics(string tableName)
        {
            return new List<DatabaseStatistics>();
        }

        public virtual IList<DatabaseIndex> IndexColumns(string tableName)
        {
            return new List<DatabaseIndex>();
        }

        public virtual IList<DatabaseTrigger> Triggers(string tableName)
        {
            return new List<DatabaseTrigger>();
        }

        public virtual IList<DatabaseSequence> Sequences(string name)
        {
            return new List<DatabaseSequence>();
        }

        public virtual IList<DatabaseStoredProcedure> StoredProcedures(string name)
        {
            return new List<DatabaseStoredProcedure>();
        }

        public virtual IList<DatabaseFunction> Functions(string name)
        {
            return new List<DatabaseFunction>();
        }

        public virtual IList<DatabaseArgument> ProcedureArguments(string name)
        {
            return new List<DatabaseArgument>();
        }

        public virtual IList<DatabasePackage> Packages(string name)
        {
            return new List<DatabasePackage>();
        }

        public virtual IList<ProcedureSource> ProcedureSources(string name)
        {
            return new List<ProcedureSource>();
        }

        public virtual IList<DatabaseUser> Users()
        {
            return new List<DatabaseUser>();
        }

        public virtual SerializableAdditionalProperties TopLevelProperties()
        {
            return new SerializableAdditionalProperties();
        }

        public virtual void PostProcessing(DatabaseTable databaseTable)
        {
            //nothing
        }
    }
}