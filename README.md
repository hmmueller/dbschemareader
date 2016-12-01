# DatabaseSchemaReader

A simple, cross-database facade over .Net 2.0 DbProviderFactories to read database metadata.

Any ADO provider can be read  (SqlServer, SqlServer CE 4, MySQL, SQLite, System.Data.OracleClient, ODP, Devart, PostgreSql, DB2...) into a single standard model. For .net Core, we support SqlServer, SqlServer CE 4, SQLite, PostgreSql, MySQL and Oracle (even if the database clients  are not yet available in .net Core, we are ready for them).

https://github.com/martinjw/dbschemareader or https://dbschemareader.codeplex.com/

https://dbschemareader.codeplex.com/documentation

Nuget: Install-Package DatabaseSchemaReader [![Nuget](https://img.shields.io/nuget/v/DatabaseSchemaReader.svg) ](https://www.nuget.org/packages/DatabaseSchemaReader/)

[![Appveyor Build Status](https://ci.appveyor.com/api/projects/status/github/martinjw/dbschemareader?svg=true)](https://ci.appveyor.com/project/martinjw/dbschemareader)

* Database schema read from most ADO providers
* Simple .net code generation:
  * Generate POCO classes for tables, and NHibernate or EF Code First mapping files
  * Generate simple ADO classes to use stored procedures
* Simple sql generation:
  * Generate table DDL (and translate to another SQL syntax, eg SqlServer to Oracle or SQLite)
  * Generate CRUD stored procedures (for SqlServer, Oracle, MySQL, DB2)
* Copy a database schema and data from any provider (SqlServer, Oracle etc) to a new SQLite database (and, with limitations, to SqlServer CE 4)
* Compare two schemas to generate a migration script
* Simple cross-database migrations generator

## Use

* Full .net framework (v3.5, v4.0, v4.5)
```C#
//To use it simply specify the connection string and ADO provider (eg System.Data,SqlClient or System.Data.OracleClient)
const string providername = "System.Data.SqlClient";
const string connectionString = @"Data Source=.\SQLEXPRESS;Integrated Security=true;Initial Catalog=Northwind";

//Create the database reader object.
var dbReader = new DatabaseReader(connectionString, providername);
//For Oracle, you should always specify the Owner (Schema).
//dbReader.Owner = "HR";

//Then load the schema (this will take a little time on moderate to large database structures)
var schema = dbReader.ReadAll();

//There are no datatables, and the structure is identical for all providers.
foreach (var table in schema.Tables)
{
  //do something with your model
}
```
* .net Core (netStandard1.5)
```C#
//In .net Core, create the connection with the connection string
using (var connection = new SqlConnection("Data Source=.\SQLEXPRESS;Integrated Security=true;Initial Catalog=Northwind"))
{
    var dr = new DatabaseSchemaReader.DatabaseReader(connection);
    //Then load the schema (this will take a little time on moderate to large database structures)
    var schema = dbReader.ReadAll();

    //The structure is identical for all providers (and the full framework).
    foreach (var table in schema.Tables)
    {
      //do something with your model
    }
}
```
## UIs

There are two simple UIs.

* DatabaseSchemaViewer. It reads all the schema and displays it in a treeview. It also includes options for
 - code generation, table DDL and stored procedure generation.
 - comparing the schema to another database.

* CopyToSQLite. It reads all the schema and creates a new SQLite database file with the same tables and data. If Sql Server CE 4.0 is detected, it can do the same for that database. These databases do not have the full range of data types as other databases, so creating tables may fail (e.g. SqlServer CE 4 does not have VARCHAR(MAX)). In addition, copying data may violate foreign key constraints (especially for identity primary keys) and will fail.

(end of text from @martinjw's dbschemareader)

## Extensions by @hmmueller

My plan is to add more server-specific support for a project where we need extended database schema comparison of Sql Server databases.
The philosophy I will follow is the following:
* For "structural elements", I will introduce new classes in DatabaseSchema. Like "DatabasePackage" (PACKAGE is an Oracle-specific thing), I'll add some concepts which we need, the first being Sql Server STATISTICS. Of course, the question is whether a general tool like dbschemareader should be "polluted" with vendor-specific extensions like that? On the other hand, what can I do? - I need them (in the comparison)!
* For "simple properties", I will add a generic, dictionary-like approach so that each class below DatabaseSchema can hold simple information retrieved from suitable meta information in the database. The access to this information is string-key-based, and the keys are exactly the column names in the database's meta-information. This will lead to SqlExceptions in ReadAll() if a wrong key is passed. The feature is "opt-in", i.e., the corresponding joins in the meta-model sql statements are only added, and the properties retrieved, if they are specified.

### API to retrieve additional properties for columns

The following excerpt from a unit test shows how to get the "is_sparse" and "collation_name" additional properties from Sql Server database columns:

```C#
var isSparseProperty = "is_sparse";
var collationProperty = "collation_name";
DatabaseReader dbReader = TestHelper.GetNorthwindReader(new AdditionalProperties
{
    AdditionalColumnPropertyNames = new[] { isSparseProperty, collationProperty }
});

DatabaseSchema schema = dbReader.ReadAll();

var table = schema.FindTableByName(tableName);
Assert.IsTrue(table.Columns.All(c => c.GetAdditionalProperty(isSparseProperty) != null));
Assert.IsTrue(table.Columns
              .Where(c => c.DbDataType.ToUpperInvariant().Contains("CHAR"))
              .All(c => c.GetAdditionalProperty(collationProperty) != null));
```

### API to retrieve additional database information

As a representative of the database, the DatabaseSchema object also can contain additional properties. Here is an example from a unit test that shows how to retrieve the collation for a Sql Server database:
```C#
var collationProperty = "collation_name";
DatabaseReader dbReader = TestHelper.GetNorthwindReader(new AdditionalProperties
{
    AdditionalTopLevelPropertyNames = new[] { collationProperty }
});

DatabaseSchema schema = dbReader.ReadAll();

Assert.IsNotNull(schema.TopLevelProperties.Get(collationProperty));
```

### A few design considerations for additional properties

The design is more or less straightforward - passing through the required names where they are needed:

a) The SqlExecuter gets them to create the Sql statement in its new FormatSql method:
```C#
    private string FormatSql(string[] additionalProperties) {
        return string.Format(Sql, // must contain {0} and {1}
            additionalProperties == null ? "" : string.Join("", additionalProperties.Select(c => ", " + ADDITIONAL_INFO + "." + c).ToArray()),
            additionalProperties == null ? "" : AdditionalPropertiesJoin);
    }
```
For this, it is assumed that Sql setup in the constructor contains {0} and {1} where the column list and possible additional joins should be added. Here is the example from the SqlServer.Columns class:

```C#
            Sql = @"select c.TABLE_SCHEMA,
c.TABLE_NAME,
c.COLUMN_NAME,
c.ORDINAL_POSITION,
c.COLUMN_DEFAULT,
c.IS_NULLABLE,
c.DATA_TYPE,
c.CHARACTER_MAXIMUM_LENGTH,
c.NUMERIC_PRECISION,
c.NUMERIC_SCALE,
c.DATETIME_PRECISION
{0}
from INFORMATION_SCHEMA.COLUMNS c
JOIN INFORMATION_SCHEMA.TABLES t
 ON c.TABLE_SCHEMA = t.TABLE_SCHEMA AND
    c.TABLE_NAME = t.TABLE_NAME
{1}
where
    (c.TABLE_SCHEMA = @Owner or (@Owner is null)) and
    (c.TABLE_NAME = @TableName or (@TableName is null)) AND
    TABLE_TYPE = 'BASE TABLE'
 order by
    c.TABLE_SCHEMA, c.TABLE_NAME, ORDINAL_POSITION";
```
The joins are specified in an additional property AdditionalPropertiesJoin. By convention, the keys passed in are prefixed with "ADDITIONAL_INFO." in the Sql's {0} part, hence, the table where the properties reside must get the alias ADDITIONAL_INFO in the JOIN (which seems to create a problem if the additional properties reside in different meta tables; however, an "aliased join" like (a ...JOIN... b) AS ADDITIONAL_INFO can solve this):
```C#
            AdditionalPropertiesJoin = string.Format(@"LEFT OUTER JOIN sys.tables syst
              ON c.TABLE_SCHEMA = schema_name(syst.schema_id) AND
                 c.TABLE_NAME = syst.name
           LEFT OUTER JOIN sys.columns {0} ON
                 syst.object_id = {0}.object_id AND
                 c.COLUMN_NAME = {0}.Name", ADDITIONAL_INFO);
```
There is one problem with this design: It is not possible to retrieve an additional property that has the same name as a column already present in the base Sql (e.g., "DATA_TYPE" in the example above). But why would one want to that? 
Possible scenarios might be a comparer that works "generally" on all properties; or the possibility to specify * for "all additional properties". A way to make that work would be to give all predefined columns an alias that will most probably not be present in any metadata, e.g. 
```C#
            Sql = @"select c.TABLE_SCHEMA AS _DBSR_TABLE_SCHEMA,
c.TABLE_NAME AS _DBSR_TABLE_NAME,
c.COLUMN_NAME AS _DBSR_COLUMN_NAME,
c.ORDINAL_POSITION AS _DBSR_ORDINAL_POSITION,
...
```

b) The names are also used in the Converter as follows (this is from the ColumnRowConverter, the only one where support for additional properties is right now added):
```C#
if (additionalProperties != null)
{
    foreach (var s in additionalProperties) {
        int ix = row.GetOrdinal(s);
        column.AddAdditionalProperty(s, row.IsDBNull(ix) ? null : row.GetValue(ix));
    }
}
```

