using DatabaseSchemaReader.ProviderSchemaReaders.Adapters;

namespace DatabaseSchemaReader
{
    public class AdditionalProperties : IAdditionalProperties
    {
        public string[] AdditionalProcedureArgumentPropertyNames
        {
            get; set;
        }
        public string[] AdditionalColumnPropertyNames
        {
            get; set;
        }
        public string[] AdditionalComputedColumnPropertyNames
        {
            get; set;
        }
        public string[] AdditionalIdentityColumnPropertyNames
        {
            get; set;
        }
        public string[] AdditionalViewColumnPropertyNames
        {
            get; set;
        }
        public string[] AdditionalCheckConstraintPropertyNames
        {
            get; set;
        }
        public string[] AdditionalDefaultConstraintPropertyNames
        {
            get; set;
        }
        public string[] AdditionalForeignKeyPropertyNames
        {
            get; set;
        }
        public string[] AdditionalPrimaryKeyPropertyNames
        {
            get; set;
        }
        public string[] AdditionalUniqueKeyPropertyNames
        {
            get; set;
        }
        public string[] AdditionalFunctionPropertyNames
        {
            get; set;
        }
        public string[] AdditionalIndexColumnPropertyNames
        {
            get; set;
        }
        public string[] AdditionalIndexPropertyNames
        {
            get; set;
        }
        public string[] AdditionalPackagePropertyNames
        {
            get; set;
        }
        public string[] AdditionalSequencePropertyNames
        {
            get; set;
        }
        public string[] AdditionalStatisticsPropertyNames
        {
            get; set;
        }
        public string[] AdditionalStoredProcedurePropertyNames
        {
            get; set;
        }
        public string[] AdditionalColumnDescriptionPropertyNames
        {
            get; set;
        }
        public string[] AdditionalTableDescriptionPropertyNames
        {
            get; set;
        }
        public string[] AdditionalTablePropertyNames
        {
            get; set;
        }
        public string[] AdditionalTopLevelPropertyNames {
            get; set;
        }
        public string[] AdditionalTriggerPropertyNames
        {
            get; set;
        }
        public string[] AdditionalUserPropertyNames
        {
            get; set;
        }
        public string[] AdditionalViewPropertyNames
        {
            get; set;
        }
        public string[] AdditionalProcedureSourcePropertyNames
        {
            get; set;
        }
        public string[] AdditionalViewSourcePropertyNames
        {
            get; set;
        }
    }
}