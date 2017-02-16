﻿using System;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Adapters
{
    static class ReaderAdapterFactory
    {
        public static ReaderAdapter Create(SchemaParameters schemaParameters, int commandTimeout)
        {
            ReaderAdapter schemaReader = null;
            switch (schemaParameters.SqlType)
            {
                case SqlType.SqlServer:
                    return new SqlServerAdapter(schemaParameters, commandTimeout);
                case SqlType.Oracle:
                    schemaReader = new OracleAdapter(schemaParameters, commandTimeout);
                    break;
                case SqlType.SQLite:
                    schemaReader = new SqLiteAdapter(schemaParameters, commandTimeout);
                    break;               
                case SqlType.PostgreSql:
                    schemaReader = new PostgreSqlAdapter(schemaParameters, commandTimeout);
                    break;
                case SqlType.MySql:
                    schemaReader = new MySqlAdapter(schemaParameters, commandTimeout);
                    break;
                case SqlType.SqlServerCe:
                    schemaReader = new SqlServerCeAdapter(schemaParameters, commandTimeout);
                    break;

                default:
                    //var providerName = schemaParameters.ProviderName;
                    //all the other types
                    //if (providerName.Equals("Ingres.Client", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new IngresSchemaReader(connectionString, providerName);
                    //}
                    //else if (providerName.Equals("iAnyWhere.Data.SQLAnyWhere", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new SybaseAsaSchemaReader(connectionString, providerName);
                    //}
                    //else if (providerName.Equals("Sybase.Data.AseClient", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new SybaseAseSchemaReader(connectionString, providerName);
                    //}
                    //else if (providerName.Equals("iAnyWhere.Data.UltraLite", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new SybaseUltraLiteSchemaReader(connectionString, providerName);
                    //}
                    //else if (providerName.Equals("System.Data.OleDb", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new OleDbSchemaReader(connectionString, providerName);
                    //}
                    //else if (providerName.Equals("System.Data.VistaDB", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new VistaDbSchemaReader(connectionString, providerName);
                    //}
                    //else if (providerName.Equals("IBM.Data.DB2.iSeries", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    schemaReader = new Db2ISeriesSchemaReader(connectionString, providerName);
                    //}
                    if (string.Equals(schemaParameters.ProviderName, "FirebirdSql.Data.FirebirdClient", StringComparison.OrdinalIgnoreCase))
                    {
                        schemaReader = new FirebirdAdapter(schemaParameters, commandTimeout);
                    }

                    break;
            }
            if (schemaReader == null)
            {
#if !COREFX
                schemaReader = new DbProviderReaderAdapter(schemaParameters, commandTimeout);
#else
                schemaReader = new ReaderAdapter(schemaParameters);
#endif
            }
            return schemaReader;
        }
    }
}
