using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer
{
    class Users : SqlExecuter<DatabaseUser>
    {
        public Users(string[] additionalUserPropertyNames, int commandTimeout) : base(additionalUserPropertyNames, commandTimeout)
        {
            Sql = @"select name {0} from sysusers {ai}".Replace("{ai}", ADDITIONAL_INFO);
        }

        protected override void AddParameters(DbCommand command)
        {
        }

        protected override void Mapper(IDataRecord record)
        {
            var name = record.GetString("name");
            var user = new DatabaseUser
            {
                Name = name,
            };

            user.AddAdditionalProperties(record, _additionalPropertyNames);

            Result.Add(user);
        }

        public IList<DatabaseUser> Execute(DbConnection dbConnection)
        {
            ExecuteDbReader(dbConnection);
            return Result;
        }
    }
}