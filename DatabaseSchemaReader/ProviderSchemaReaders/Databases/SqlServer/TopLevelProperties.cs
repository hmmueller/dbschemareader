using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class TopLevelProperties : SqlExecuter
    {
        private SerializableAdditionalProperties _result;

        public TopLevelProperties(string[] additionalTopLevelProperties) : base(additionalTopLevelProperties)
        {
            Sql = @"SELECT 0 AS Dummy {0} FROM sys.databases ADDITIONAL_INFO WHERE name = db_name()";
        }

        protected override void AddParameters(DbCommand command)
        {
        }

        protected override void Mapper(IDataRecord row)
        {
            _result = new SerializableAdditionalProperties();
            if (_additionalProperties != null)
            {
                foreach (var p in _additionalProperties)
                {
                    int ix = row.GetOrdinal(p);
                    _result.Add(p, row.IsDBNull(ix) ? null : row.GetValue(ix));
                }
            }
        }

        public SerializableAdditionalProperties Execute(DbConnection dbConnection)
        {
            if (_additionalProperties != null)
                ExecuteDbReader(dbConnection);
            return _result;
        }
    }
}