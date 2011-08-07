﻿using System;
using System.Collections.Generic;
using DatabaseSchemaReader.Data;
using DatabaseSchemaReader.DataSchema;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
#endif

namespace DatabaseSchemaReaderTest.SqlGen.InsertWriterTests
{
    /// <summary>
    /// Tests converting data to INSERT strings
    /// </summary>
    [TestClass]
    public class ConverterTest
    {

        [TestMethod]
        public void TestNull()
        {
            //arrange
            string s = null;
            const SqlType sqlType = SqlType.Db2;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(string), s);

            //assert
            Assert.AreEqual("NULL", result);
        }

        [TestMethod]
        public void TestDbNull()
        {
            //arrange
            var dbNull = DBNull.Value;
            const SqlType sqlType = SqlType.Db2;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(string), dbNull);

            //assert
            Assert.AreEqual("NULL", result);
        }

        [TestMethod]
        public void TestString()
        {
            //arrange
            const string s = "Hello";
            const SqlType sqlType = SqlType.Db2;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(string), s);

            //assert
            Assert.AreEqual("'Hello'", result);
        }

        [TestMethod]
        public void TestStringWithSingleQuotes()
        {
            //arrange
            const string s = "Hello 'Boys'";
            const SqlType sqlType = SqlType.Db2;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(string), s);

            //assert
            Assert.AreEqual("'Hello ''Boys'''", result);
        }

        [TestMethod]
        public void TestStringUnicodeSqlServer()
        {
            //arrange
            const string s = "Hello";
            const SqlType sqlType = SqlType.SqlServer;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(string), s);

            //assert
            Assert.AreEqual("N'Hello'", result);
        }


        [TestMethod]
        public void TestStringInteger()
        {
            //arrange
            const int i = 10;
            const SqlType sqlType = SqlType.SqlServer;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(int), i);

            //assert
            Assert.AreEqual("10", result);
        }

        [TestMethod]
        public void TestStringDecimal()
        {
            //arrange
            const decimal i = 10.5M;
            const SqlType sqlType = SqlType.SqlServer;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(decimal), i);

            //assert
            Assert.AreEqual("10.5", result);
        }

        [TestMethod]
        public void TestDate()
        {
            //arrange
            var dt = new DateTime(2001, 3, 30, 10, 45, 30, 839);
            const SqlType sqlType = SqlType.Db2;
            var dateTypes = new Dictionary<string, string>();
            dateTypes.Add("StartDate", "TIMESTAMP");

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("StartDate", typeof(DateTime), dt);

            //assert
            Assert.AreEqual("'2001-03-30 10:45:30.839'", result);
        }


        [TestMethod]
        public void TestStringTimeSpan()
        {
            //arrange
            var ts = new TimeSpan(1, 2, 3);
            const SqlType sqlType = SqlType.SqlServer;
            var dateTypes = new Dictionary<string, string>();

            var converter = new Converter(sqlType, dateTypes);

            //act
            var result = converter.Convert("Name", typeof(TimeSpan), ts);

            //assert
            Assert.AreEqual("'01:02:03'", result);
        }
    }
}