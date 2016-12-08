using System.Diagnostics;
using System.Linq;
using DatabaseSchemaReader;
using DatabaseSchemaReader.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseSchemaReaderTest.IntegrationTests
{
    /// <summary>
    /// Summary description for SqlServerNorthwind
    /// </summary>
    [TestClass]
    public class SqlServerNorthwind
    {

        [TestMethod, TestCategory("SqlServer.Odbc")]
        public void ReadNorthwindUsingOdbc()
        {
            //you'll get much more information from System.Data.SqlClient
            const string providername = "System.Data.Odbc";
            const string connectionString = @"Driver={SQL Server};Server=.\SQLEXPRESS;Database=Northwind;Trusted_Connection=Yes;";
            ProviderChecker.Check(providername, connectionString);

            var dbReader = new DatabaseReader(connectionString, providername) { Owner = "dbo" };
            //this is slow because it pulls in sp_ stored procedures and system views.
            dbReader.Exclusions.StoredProcedureFilter = new PrefixFilter("sp_", "fn_", "dm_", "xp_");
            var schema = dbReader.ReadAll();

            Assert.IsTrue(schema.Tables.Count > 0);
        }


        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindProducts()
        {
            var dbReader = TestHelper.GetNorthwindReader();
            var table = dbReader.Table("Products");
            Debug.WriteLine("Table " + table.Name);

            foreach (var column in table.Columns)
            {
                //because we loaded only a single table
                //relations aren't available here (to datatypes/foreign key tables)
                Debug.Write("\tColumn " + column.Name + "\t" + column.DbDataType);
                if (column.Length > 0)
                    Debug.Write("(" + column.Length + ")");
                if (column.IsPrimaryKey)
                    Debug.Write("\tPrimary key");
                if (column.IsForeignKey)
                    Debug.Write("\tForeign key to " + column.ForeignKeyTableName);
                Debug.WriteLine("");
            }
            //Table Products
            //	Column ProductID	int	Primary key
            //	Column ProductName	nvarchar(40)
            //	Column SupplierID	int	Foreign key to Suppliers
            //	Column CategoryID	int	Foreign key to Categories
            //	Column QuantityPerUnit	nvarchar(20)
            //	Column UnitPrice	money
            //	Column UnitsInStock	smallint
            //	Column UnitsOnOrder	smallint
            //	Column ReorderLevel	smallint
            //	Column Discontinued	bit
        }

        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindAllTables()
        {
            var dbReader = TestHelper.GetNorthwindReader();
            var tables = dbReader.AllTables();
            foreach (var table in tables)
            {
                Debug.WriteLine("Table " + table.Name);

                foreach (var column in table.Columns)
                {
                    //because we loaded only tables
                    //relations to datatypes aren't available here
                    //but foreign key tables are linked up
                    Debug.Write("\tColumn " + column.Name + "\t" + column.DbDataType);
                    if (column.Length > 0)
                        Debug.Write("(" + column.Length + ")");
                    if (column.IsPrimaryKey)
                        Debug.Write("\tPrimary key");
                    if (column.IsForeignKey)
                        Debug.Write("\tForeign key to " + column.ForeignKeyTable.Name);
                    Debug.WriteLine("");
                }
            }
            //Table Products
            //	Column ProductID	int	Primary key
            //	Column ProductName	nvarchar(40)
            //	Column SupplierID	int	Foreign key to Suppliers
            //	Column CategoryID	int	Foreign key to Categories
            //	Column QuantityPerUnit	nvarchar(20)
            //	Column UnitPrice	money
            //	Column UnitsInStock	smallint
            //	Column UnitsOnOrder	smallint
            //	Column ReorderLevel	smallint
            //	Column Discontinued	bit
        }

        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwind()
        {
            var dbReader = TestHelper.GetNorthwindReader();
            var schema = dbReader.ReadAll();

            foreach (var table in schema.Tables)
            {
                Debug.WriteLine("Table " + table.Name);

                foreach (var column in table.Columns)
                {
                    Debug.Write("\tColumn " + column.Name + "\t" + column.DataType.TypeName);
                    if (column.DataType.IsString)
                        Debug.Write("(" + column.Length + ")");
                    if (column.IsPrimaryKey)
                        Debug.Write("\tPrimary key");
                    if (column.IsForeignKey)
                        Debug.Write("\tForeign key to " + column.ForeignKeyTable.Name);
                    Debug.WriteLine("");
                }
                //Table Products
                //	Column ProductID	int	Primary key
                //	Column ProductName	nvarchar(40)
                //	Column SupplierID	int	Foreign key to Suppliers
                //	Column CategoryID	int	Foreign key to Categories
                //	Column QuantityPerUnit	nvarchar(20)
                //	Column UnitPrice	money
                //	Column UnitsInStock	smallint
                //	Column UnitsOnOrder	smallint
                //	Column ReorderLevel	smallint
                //	Column Discontinued	bit
            }
        }

        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindViews()
        {
            var dbReader = TestHelper.GetNorthwindReader();
            var schema = dbReader.ReadAll();
            foreach (var view in schema.Views)
            {
                var sql = view.Sql;
                Assert.IsNotNull(sql, "ProcedureSource should also fill in the view source");
            }
        }


        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindProductsWithCodeGen()
        {
            var dbReader = TestHelper.GetNorthwindReader();
            dbReader.DataTypes(); //load the datatypes
            var table = dbReader.Table("Products");
            Debug.WriteLine("Table " + table.Name);

            foreach (var column in table.Columns)
            {
                //Cs properties (the column name could be made .Net friendly too)
                Debug.WriteLine("\tpublic " + column.DataType.NetCodeName(column) + " " + column.Name + " { get; set; }");
            }
            //	public int ProductID { get; set; }
            //	public string ProductName { get; set; }
            //	public int SupplierID { get; set; }
            //	public int CategoryID { get; set; }
            //	public string QuantityPerUnit { get; set; }
            //	public decimal UnitPrice { get; set; }
            //	public short UnitsInStock { get; set; }
            //	public short UnitsOnOrder { get; set; }
            //	public short ReorderLevel { get; set; }
            //	public bool Discontinued { get; set; }

            //get the sql
            var sqlWriter =
                new SqlWriter(table, DatabaseSchemaReader.DataSchema.SqlType.SqlServer);
            var sql = sqlWriter.SelectPageSql(); //paging sql
            sql = SqlWriter.SimpleFormat(sql); //remove line breaks

            Debug.WriteLine(sql);
            //SELECT [ProductID], [ProductName], ...etc... 
            //FROM 
            //(SELECT ROW_NUMBER() OVER( ORDER BY [ProductID]) AS 
            //rowNumber, [ProductID], [ProductName],  ...etc..
            //FROM [Products]) AS countedTable 
            //WHERE rowNumber >= (@pageSize * (@currentPage - 1)) 
            //AND rowNumber <= (@pageSize * @currentPage)
        }


        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindWithFilters()
        {
            //arrange
            const string category = "Categories";
            const string alphaList = "Alphabetical list of products";
            const string custorderhist = "CustOrderHist";
            var dbReader = TestHelper.GetNorthwindReader();
            dbReader.Exclusions.TableFilter.FilterExclusions.Add(category);
            dbReader.Exclusions.ViewFilter.FilterExclusions.Add(alphaList);
            dbReader.Exclusions.StoredProcedureFilter.FilterExclusions.Add(custorderhist);

            //act
            var schema = dbReader.ReadAll();

            //assert
            var table = schema.FindTableByName(category);
            Assert.IsNull(table);
            var view = schema.Views.FirstOrDefault(v => v.Name == alphaList);
            Assert.IsNull(view);
            var sproc = schema.StoredProcedures.FirstOrDefault(sp => sp.Name == custorderhist);
            Assert.IsNull(sproc);
        }

        [TestMethod, TestCategory("SqlServer")]
        public void DublicatedArgumentsDemo()
        {
            var dbReader = TestHelper.GetNorthwindReader();
            var procedures = dbReader.AllStoredProcedures();

            var proc = procedures.First(x => x.Name == "CustOrderHist");
            var argsNumber = proc.Arguments.Count();

            dbReader.AllStoredProcedures();
            Assert.AreEqual(argsNumber,
                            proc.Arguments.Count(),
                            "Number of args changed");
        }

        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindWithAdditionalColumnProperties()
        {
            //arrange
            const string tableName = "Categories";
            var isSparseProperty = "is_sparse";
            var collationProperty = "collation_name";
            DatabaseReader dbReader = TestHelper.GetNorthwindReader(new AdditionalProperties
            {
                AdditionalColumnPropertyNames = new[] { isSparseProperty, collationProperty }
            });

            //act
            var schema = dbReader.ReadAll();

            //assert
            var table = schema.FindTableByName(tableName);
            Assert.IsTrue(table.Columns.All(c => c.GetAdditionalProperty(isSparseProperty) != null));
            Assert.IsTrue(table.Columns.Where(c => c.DbDataType.ToUpperInvariant().Contains("CHAR")).All(c => c.GetAdditionalProperty(collationProperty) != null));
        }

        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindWithAdditionalDatabaseProperties()
        {
            //arrange
            var collationProperty = "collation_name";
            DatabaseReader dbReader = TestHelper.GetNorthwindReader(new AdditionalProperties
            {
                AdditionalTopLevelPropertyNames = new[] { collationProperty }
            });

            //act
            var schema = dbReader.ReadAll();

            //assert
            Assert.IsNotNull(schema.TopLevelProperties.Get(collationProperty));
        }

        [TestMethod, TestCategory("SqlServer")]
        public void ReadNorthwindWithManyAdditionalProperties()
        {
            //arrange
            DatabaseReader dbReader = TestHelper.GetNorthwindReader(new AdditionalProperties
            {
                AdditionalTopLevelPropertyNames = new[] {
                        // retrieved from sys.databases
                        "collation_name", "compatibility_level",
                        "is_ansi_null_default_on", "is_ansi_nulls_on", "is_ansi_padding_on", "is_ansi_warnings_on", "is_arithabort_on",
                        "is_auto_close_on", "is_auto_create_stats_on", "is_auto_shrink_on", "is_auto_update_stats_async_on", "is_auto_update_stats_on",
                        "is_broker_enabled", "is_cdc_enabled", "is_cleanly_shutdown", "is_concat_null_yields_null_on", "is_cursor_close_on_commit_on",
                        "is_date_correlation_on", "is_db_chaining_on", "is_distributor", "is_encrypted", "is_fulltext_enabled", "is_honor_broker_priority_on",
                        "is_in_standby", "is_local_cursor_default", "is_master_key_encrypted_by_server", "is_merge_published", "is_numeric_roundabort_on",
                        "is_parameterization_forced", "is_published", "is_quoted_identifier_on", "is_read_committed_snapshot_on", "is_read_only",
                        "is_recursive_triggers_on", "is_subscribed", "is_supplemental_logging_enabled", "is_sync_with_backup", "is_trustworthy_on",
                        "log_reuse_wait", "log_reuse_wait_desc",
                        "page_verify_option", "page_verify_option_desc",
                        "snapshot_isolation_state", "snapshot_isolation_state_desc", "state", "state_desc",
                        "user_access", "user_access_desc",
                        // not retrieved from sys.databases
                        // "create_date", "database_id", "name", "owner_sid", "service_broker_guid", "source_database_id",
                        // "recovery_model", "recovery_model_desc" - weil in Test- und Entw.systemen abweichend von Produktivsystemen
                    },
                AdditionalTablePropertyNames = new[] {
                        // retrieved from sys.tables
                        "has_replication_filter", "has_unchecked_assembly_data",
                        "is_merge_published", "is_ms_shipped", "is_published", "is_replicated", "is_schema_published", "is_sync_tran_subscribed", "is_tracked_by_cdc",
                        "large_value_types_out_of_row", "lock_escalation", "lock_escalation_desc", "lock_on_bulk_load",
                        "text_in_row_limit", "type", "type_desc",
                        "uses_ansi_nulls",
                        // not retrieved from sys.tables
                        // "create_date", "filestream_data_space_id", "lob_data_space_id", "max_column_id_used",
                        // "modify_date", "name", "object_id", "parent_object_id", "principal_id", "schema_id",
                    },
                AdditionalViewPropertyNames = new[] {
                        // retrieved from sys.views
                        "has_opaque_metadata", "has_replication_filter", "has_unchecked_assembly_data",
                        "is_date_correlation_view", "is_ms_shipped", "is_published", "is_replicated", "is_schema_published", "is_tracked_by_cdc",
                        "type", "type_desc",
                        "with_check_option",
                        // not retrieved from sys.views
                        // "create_date", "modify_date", "name", "object_id", "parent_object_id", "principal_id", "schema_id",
                    },
                AdditionalColumnPropertyNames = new[] {
                        // retrieved from sys.columns
                        "collation_name",
                        "is_ansi_padded", "is_column_set", "is_computed", "is_dts_replicated", "is_filestream",
                        "is_identity", "is_merge_published", "is_non_sql_subscribed", "is_nullable", "is_replicated",
                        "is_rowguidcol", "is_sparse", "is_xml_document",
                        "system_type_id",
                        "user_type_id",
                        // not retrieved from sys.columns
                        // "column_id", "default_object_id", "name", "object_id", "rule_object_id", "xml_collection_id",
                    },
                AdditionalCheckConstraintPropertyNames = new[] { "create_date",
                    // retrieved from sys.check_constraints
                    "definition",
                    "is_disabled", "is_ms_shipped", "is_not_for_replication", "is_not_trusted",
                    "is_published", "is_schema_published", "is_system_named",
                    "modify_date",
                    "name",
                    "type", "type_desc",
                    "uses_database_collation",

                    // not retrieved from sys.check_constraints
                    // "object_id", "parent_column_id", "parent_object_id", "principal_id", "schema_id",
                },
                AdditionalColumnDescriptionPropertyNames = new[] { "xxx1" },
                AdditionalComputedColumnPropertyNames = new[] { "collation_name",
                    // retrieved from sys.computed_columns
                    "definition",
                    "is_ansi_padded", "is_column_set", "is_computed", "is_dts_replicated", "is_filestream", "is_identity", "is_merge_published",
                    "is_non_sql_subscribed", "is_nullable", "is_persisted", "is_replicated", "is_rowguidcol", "is_sparse", "is_xml_document",
                    "max_length",
                    "name",
                    "precision",
                    "scale",
                    "uses_database_collation",

                    // not retrieved from sys.computed_columns
                    // "column_id", "default_object_id", "object_id", "rule_object_id", "system_type_id", "user_type_id", "xml_collection_id",
                },
                AdditionalDefaultConstraintPropertyNames = new string[] {
                    // no additional properties for default constraints supported
                },
                AdditionalForeignKeyPropertyNames = new[] {
                    // retrieved from sys.foreign_keys
                    "delete_referential_action", "delete_referential_action_desc",
                    "is_disabled", "is_ms_shipped", "is_not_for_replication", "is_not_trusted", "is_published", "is_schema_published", "is_system_named",
                    "modify_date",
                    "name",
                    "type", "type_desc",
                    "update_referential_action", "update_referential_action_desc",

                    // not retrieved from sys.foreign_keys
                    // "create_date", "key_index_id", "object_id", "parent_object_id", "principal_id", "referenced_object_id", "schema_id",
                },
                AdditionalFunctionPropertyNames = new[] { "CHARACTER_MAXIMUM_LENGTH", "CHARACTER_OCTET_LENGTH", "CHARACTER_SET_CATALOG", "CHARACTER_SET_NAME", "CHARACTER_SET_SCHEMA",
                    // retrieved from INFORMATION_SCHEMA.ROUTINES
                    "COLLATION_CATALOG", "COLLATION_NAME", "COLLATION_SCHEMA",
                    "DATA_TYPE", "DATETIME_PRECISION", "DTD_IDENTIFIER",
                    "EXTERNAL_LANGUAGE", "EXTERNAL_NAME",
                    "INTERVAL_PRECISION", "INTERVAL_TYPE", "IS_DETERMINISTIC", "IS_IMPLICITLY_INVOCABLE", "IS_NULL_CALL", "IS_USER_DEFINED_CAST",
                    "MAXIMUM_CARDINALITY", "MAX_DYNAMIC_RESULT_SETS", "MODULE_CATALOG", "MODULE_NAME", "MODULE_SCHEMA",
                    "NUMERIC_PRECISION", "NUMERIC_PRECISION_RADIX", "NUMERIC_SCALE",
                    "PARAMETER_STYLE",
                    "ROUTINE_BODY", "ROUTINE_CATALOG", "ROUTINE_NAME", "ROUTINE_SCHEMA", "ROUTINE_TYPE",
                    "SCHEMA_LEVEL_ROUTINE", "SCOPE_CATALOG", "SCOPE_NAME", "SCOPE_SCHEMA", "SPECIFIC_CATALOG", "SQL_DATA_ACCESS", "SQL_PATH",
                    "TYPE_UDT_CATALOG", "TYPE_UDT_NAME", "TYPE_UDT_SCHEMA",
                    "UDT_CATALOG", "UDT_NAME", "UDT_SCHEMA",

                    // not retrieved from INFORMATION_SCHEMA.ROUTINES
                    // "CREATED", "LAST_ALTERED",
                },
                AdditionalIdentityColumnPropertyNames = new[] {
                    // retrieved from sys.identity_columns
                    "collation_name",
                    "increment_value", "is_ansi_padded", "is_column_set", "is_computed", "is_dts_replicated",
                    "is_filestream", "is_identity", "is_merge_published", "is_non_sql_subscribed", "is_not_for_replication",
                    "is_nullable", "is_replicated", "is_rowguidcol", "is_sparse", "is_xml_document",
                    "last_value",
                    "max_length",
                    "name",
                    "precision",
                    "scale", "seed_value",

                    // not retrieved from sys.identity_columns
                    // "column_id", "default_object_id", "object_id", "rule_object_id",
                    // "system_type_id", "user_type_id", "xml_collection_id"
                },
                AdditionalIndexColumnPropertyNames = new string[] {
                    // TODO: Does not work :_( - needed for is_included_column!!!!

                    // retrieved from sys.index_columns
                    //"is_descending_key", "is_included_column",                    
                    //"key_ordinal",

                    // not retrieved from sys.index_columns
                    // "column_id", "data_space_id", "index_column_id", "index_id", "object_id",
                },
                AdditionalIndexPropertyNames = new[] {
                    // retrieved from sys.indexes
                    "allow_page_locks", "allow_row_locks",
                    "fill_factor", "filter_definition",
                    "has_filter",
                    "ignore_dup_key", "is_disabled", "is_hypothetical",
                    "is_padded", "is_primary_key", "is_unique", "is_unique_constraint",
                    "name",
                    "partition_ordinal",
                    "type", "type_desc",

                    // not retrieved from sys.indexes
                    // "column_id", "data_space_id", "index_column_id", "index_id", "object_id",
                },
                AdditionalPackagePropertyNames = new[] { "xxx7" },
                AdditionalPrimaryKeyPropertyNames = new string[] {
                    // no additional properties for PKs supported
                },
                AdditionalProcedureArgumentPropertyNames = new[] { "xxx8" },
                AdditionalProcedureSourcePropertyNames = new[] { "xxx9" },
                AdditionalSequencePropertyNames = new[] { "xxxa" },
                AdditionalStatisticsPropertyNames = new[] { "auto_created",
                    // retrieved from sys.stats
                    "filter_definition",
                    "has_filter",
                    "name",
                    "no_recompute",
                    "user_created",

                    // not retrieved from sys.stats
                    // "object_id", "stats_id",
                },
                AdditionalStoredProcedurePropertyNames = new[] { "CHARACTER_MAXIMUM_LENGTH", "CHARACTER_OCTET_LENGTH", "CHARACTER_SET_CATALOG", "CHARACTER_SET_NAME", "CHARACTER_SET_SCHEMA",
                    // retrieved from INFORMATION_SCHEMA.ROUTINES
                    "COLLATION_CATALOG", "COLLATION_NAME", "COLLATION_SCHEMA",
                    "DATA_TYPE", "DATETIME_PRECISION", "DTD_IDENTIFIER",
                    "EXTERNAL_LANGUAGE", "EXTERNAL_NAME",
                    "INTERVAL_PRECISION", "INTERVAL_TYPE", "IS_DETERMINISTIC", "IS_IMPLICITLY_INVOCABLE", "IS_NULL_CALL", "IS_USER_DEFINED_CAST",
                    "MAXIMUM_CARDINALITY", "MAX_DYNAMIC_RESULT_SETS", "MODULE_CATALOG", "MODULE_NAME", "MODULE_SCHEMA",
                    "NUMERIC_PRECISION", "NUMERIC_PRECISION_RADIX", "NUMERIC_SCALE",
                    "PARAMETER_STYLE",
                    "ROUTINE_BODY", "ROUTINE_CATALOG", "ROUTINE_NAME", "ROUTINE_SCHEMA", "ROUTINE_TYPE",
                    "SCHEMA_LEVEL_ROUTINE", "SCOPE_CATALOG", "SCOPE_NAME", "SCOPE_SCHEMA", "SPECIFIC_CATALOG", "SQL_DATA_ACCESS", "SQL_PATH",
                    "TYPE_UDT_CATALOG", "TYPE_UDT_NAME", "TYPE_UDT_SCHEMA",
                    "UDT_CATALOG", "UDT_NAME", "UDT_SCHEMA",

                    // not retrieved from INFORMATION_SCHEMA.ROUTINES
                    // "CREATED", "LAST_ALTERED",
                },
                AdditionalTableDescriptionPropertyNames = new[] { "xxxd" },
                AdditionalTriggerPropertyNames = new[] { "name",
                    // retrieved from sys.triggers
                    "parent_class", "parent_class_desc",
                    "type", "type_desc",
                    "is_ms_shipped", "is_disabled", "is_not_for_replication", "is_instead_of_trigger"

                    // not retrieved from sys.triggers
                    // "object_id", "parent_id", "create_date", "modify_date",
                },
                AdditionalUniqueKeyPropertyNames = new string[] {
                    // no additional properties for UKs supported
                },
                AdditionalUserPropertyNames = new[] {
                    // retrieved from sysusers
                    "environ",
                    "hasdbaccess",
                    "isaliased", "isapprole", "islogin", "isntgroup", "isntname", "isntuser", "issqlrole", "issqluser",
                    "name",
                    "password",
                    "roles",
                    "status",

                    // not retrieved from sysusers
                    // "altuid", "createdate", "gid", "sid", "uid", "updatedate",
                },
                AdditionalViewColumnPropertyNames = new[] { "xxxg" },
                AdditionalViewSourcePropertyNames = new[] { "xxxh" },
            });

            //act
            dbReader.ReadAll();

            //assert
            // We survived all the SQL statements!
        }

    }
}
