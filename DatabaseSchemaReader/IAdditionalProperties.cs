namespace DatabaseSchemaReader
{
    public interface IAdditionalProperties
    {
        string[] AdditionalProcedureArgumentPropertyNames
        {
            get;
        }
        string[] AdditionalColumnPropertyNames
        {
            get;
        }
        string[] AdditionalComputedColumnPropertyNames
        {
            get;
        }
        string[] AdditionalIdentityColumnPropertyNames
        {
            get;
        }
        string[] AdditionalViewColumnPropertyNames
        {
            get;
        }
        string[] AdditionalCheckConstraintPropertyNames
        {
            get;
        }
        string[] AdditionalDefaultConstraintPropertyNames
        {
            get;
        }
        string[] AdditionalForeignKeyPropertyNames
        {
            get;
        }
        string[] AdditionalPrimaryKeyPropertyNames
        {
            get;
        }
        string[] AdditionalUniqueKeyPropertyNames
        {
            get;
        }
        string[] AdditionalFunctionPropertyNames
        {
            get;
        }
        string[] AdditionalIndexColumnPropertyNames
        {
            get;
        }
        string[] AdditionalIndexPropertyNames
        {
            get;
        }
        string[] AdditionalPackagePropertyNames
        {
            get;
        }
        string[] AdditionalSequencePropertyNames
        {
            get;
        }
        string[] AdditionalStatisticsPropertyNames
        {
            get;
        }
        string[] AdditionalStoredProcedurePropertyNames
        {
            get;
        }
        string[] AdditionalColumnDescriptionPropertyNames
        {
            get;
        }
        string[] AdditionalTableDescriptionPropertyNames
        {
            get;
        }
        string[] AdditionalTablePropertyNames
        {
            get;
        }
        string[] AdditionalTopLevelPropertyNames
        {
            get;
        }        
        string[] AdditionalTriggerPropertyNames
        {
            get;
        }
        string[] AdditionalUserPropertyNames
        {
            get;
        }
        string[] AdditionalViewPropertyNames
        {
            get;
        }
        string[] AdditionalProcedureSourcePropertyNames
        {
            get;
        }
        string[] AdditionalViewSourcePropertyNames
        {
            get;
        }
    }
}