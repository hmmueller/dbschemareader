namespace DatabaseSchemaReader.Compare
{
    /// <summary>
    /// 
    /// </summary>
    public enum SchemaObjectType
    {
        /// <summary>
        /// table
        /// </summary>
        Table,
        /// <summary>
        /// view
        /// </summary>
        View,
        /// <summary>
        /// column
        /// </summary>
        Column,
        /// <summary>
        /// constraint
        /// </summary>
        Constraint,
        /// <summary>
        /// index
        /// </summary>
        Index,
        /// <summary>
        /// trigger
        /// </summary>
        Trigger,
        /// <summary>
        /// stored procedure
        /// </summary>
        StoredProcedure,
        /// <summary>
        /// function
        /// </summary>
        Function,
        /// <summary>
        /// sequence
        /// </summary>
        Sequence,
        /// <summary>
        /// package - Oracls concept
        /// </summary>
        Package,

        /// <summary>
        /// statistics - SQL Server concept
        /// </summary>
        Statistics,
    }
}