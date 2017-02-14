namespace DatabaseSchemaReader
{
    /// <summary>
    /// Definition of additional database-specific properties to be loaded in <see cref="DatabaseSchemaReader"/>
    /// </summary>
    public class AdditionalProperties : IAdditionalProperties
    {
        /// <summary>
        /// Additional database-specific properties to be loaded for stored procedure arguments
        /// </summary>
        public string[] AdditionalProcedureArgumentPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for columns
        /// </summary>
        public string[] AdditionalColumnPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for computed columns
        /// </summary>
        public string[] AdditionalComputedColumnPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for identity columns
        /// </summary>
        public string[] AdditionalIdentityColumnPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for columns in views
        /// </summary>
        public string[] AdditionalViewColumnPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for check constraints
        /// </summary>
        public string[] AdditionalCheckConstraintPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for default constraints
        /// </summary>
        public string[] AdditionalDefaultConstraintPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for foreign key constraints
        /// </summary>
        public string[] AdditionalForeignKeyPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for primary key constraints
        /// </summary>
        public string[] AdditionalPrimaryKeyPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for unique key constraints
        /// </summary>
        public string[] AdditionalUniqueKeyPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for stored functions
        /// </summary>
        public string[] AdditionalFunctionPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for index columns
        /// </summary>
        public string[] AdditionalIndexColumnPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for indexes
        /// </summary>
        public string[] AdditionalIndexPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for pacakges (Oracle specific)
        /// </summary>
        public string[] AdditionalPackagePropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for sequences
        /// </summary>
        public string[] AdditionalSequencePropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for statistics (SQL server specific)
        /// </summary>
        public string[] AdditionalStatisticsPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for stored procedures
        /// </summary>
        public string[] AdditionalStoredProcedurePropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for column desriptions // TODO: DOES NOT WORK
        /// </summary>
        public string[] AdditionalColumnDescriptionPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for table descriptions // TODO: DOES NOT WORK
        /// </summary>
        public string[] AdditionalTableDescriptionPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for tables
        /// </summary>
        public string[] AdditionalTablePropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for top level properties of the database
        /// </summary>
        public string[] AdditionalTopLevelPropertyNames {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for triggers
        /// </summary>
        public string[] AdditionalTriggerPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for users
        /// </summary>
        public string[] AdditionalUserPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for views
        /// </summary>
        public string[] AdditionalViewPropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for procedure and function sources // TODO: DOES NOT WORK
        /// </summary>
        public string[] AdditionalProcedureSourcePropertyNames
        {
            get; set;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for view sources // TODO: DOES NOT WORK
        /// </summary>
        public string[] AdditionalViewSourcePropertyNames
        {
            get; set;
        }
    }
}