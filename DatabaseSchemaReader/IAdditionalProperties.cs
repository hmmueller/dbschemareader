namespace DatabaseSchemaReader
{
    /// <summary>
    /// Definition of additional database-specific properties to be loaded in <see cref="DatabaseSchemaReader"/>
    /// </summary>
    public interface IAdditionalProperties
    {
        /// <summary>
        /// Additional database-specific properties to be loaded for stored procedure arguments
        /// </summary>
        string[] AdditionalProcedureArgumentPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for columns
        /// </summary>
        string[] AdditionalColumnPropertyNames
        {
            get;
        }
        
        /// <summary>
        /// Additional database-specific properties to be loaded for computed columns
        /// </summary>
        string[] AdditionalComputedColumnPropertyNames
        {
            get;
        }
        
        /// <summary>
        /// Additional database-specific properties to be loaded for identity columns
        /// </summary>
        string[] AdditionalIdentityColumnPropertyNames
        {
            get;
        }
        
        /// <summary>
        /// Additional database-specific properties to be loaded for columns in views
        /// </summary>
        string[] AdditionalViewColumnPropertyNames
        {
            get;
        }
        
        /// <summary>
        /// Additional database-specific properties to be loaded for check constraints
        /// </summary>
        string[] AdditionalCheckConstraintPropertyNames
        {
            get;
        }
        
        /// <summary>
        /// Additional database-specific properties to be loaded for default constraints
        /// </summary>
        string[] AdditionalDefaultConstraintPropertyNames
        {
            get;
        }
        
        /// <summary>
        /// Additional database-specific properties to be loaded for foreign key constraints
        /// </summary>
        string[] AdditionalForeignKeyPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for primary key constraints
        /// </summary>
        string[] AdditionalPrimaryKeyPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for unique key constraints
        /// </summary>
        string[] AdditionalUniqueKeyPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for stored functions
        /// </summary>
        string[] AdditionalFunctionPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for index columns
        /// </summary>
        string[] AdditionalIndexColumnPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for indexes
        /// </summary>
        string[] AdditionalIndexPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for pacakges (Oracle specific)
        /// </summary>
        string[] AdditionalPackagePropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for sequences
        /// </summary>
        string[] AdditionalSequencePropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for statistics (SQL server specific)
        /// </summary>
        string[] AdditionalStatisticsPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for stored procedures
        /// </summary>
        string[] AdditionalStoredProcedurePropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for column desriptions // TODO: DOES NOT WORK
        /// </summary>
        string[] AdditionalColumnDescriptionPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for table descriptions // TODO: DOES NOT WORK
        /// </summary>
        string[] AdditionalTableDescriptionPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for tables
        /// </summary>
        string[] AdditionalTablePropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for top level properties of the database
        /// </summary>
        string[] AdditionalTopLevelPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for triggers
        /// </summary>
        string[] AdditionalTriggerPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for users
        /// </summary>
        string[] AdditionalUserPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for views
        /// </summary>
        string[] AdditionalViewPropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for procedure and function sources // TODO: DOES NOT WORK
        /// </summary>
        string[] AdditionalProcedureSourcePropertyNames
        {
            get;
        }

        /// <summary>
        /// Additional database-specific properties to be loaded for view sources // TODO: DOES NOT WORK
        /// </summary>
        string[] AdditionalViewSourcePropertyNames
        {
            get;
        }
    }
}