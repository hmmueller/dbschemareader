using System;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseSchemaReaderTest
{
    
    /// <summary>
    /// 
    ///</summary>
    [TestClass]
    public class DatabaseReaderTest
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NoConnectionStringTest()
        {
            new DatabaseReader(null, SqlType.SqlServer, commandTimeout: 5);

            Assert.Fail("Should not have succeeded");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NoProviderTest()
        {
            new DatabaseReader("Dummy", null, 0);

            Assert.Fail("Should not have succeeded");
        }

        [TestMethod]
        public void SqlTypeTest()
        {
            var dr =  new DatabaseReader("Dummy", SqlType.SqlServer, commandTimeout: 5);
            Assert.AreEqual("System.Data.SqlClient", dr.DatabaseSchema.Provider);

            //the other types will fail if they aren't installed
        }
    }
}
