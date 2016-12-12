using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseSchemaReaderTest.ProviderSchemaReaders
{
    [TestClass]
    public class SqlServerSymbolParserTests {
        private const string SQL = @"create /*as 1*/ procedure [dbo]/*as 2*/.-- as 3
[def]/* as 4*/

@x varchar(10) = 'as 5'

as
begin print 'def'+@x end
";

        [TestMethod, TestCategory("SqlServer")]
        public void CheckSymbols() {
            var symbols = new List<string>();
            string sql = SQL;
            for (var nextSym = SymbolParser.NextSym(sql); nextSym != null; nextSym = SymbolParser.NextSym(sql)) {
                symbols.Add(nextSym);
                sql = sql.Substring(nextSym.Length);
                Console.WriteLine(">" + nextSym + "<");
            }

            Assert.AreEqual(36, symbols.Count);
        }

        [TestMethod, TestCategory("SqlServer")]
        public void CheckSkipToParameter()
        {
            var rest = SymbolParser.SkipSymbolsUntil(SQL, new Regex("^@"));
            Assert.IsTrue(rest.StartsWith("@x varchar"));
        }

        [TestMethod, TestCategory("SqlServer")]
        public void CheckSkipToKeyword()
        {
            var rest = SymbolParser.SkipSymbolsUntil(SQL, new Regex("^AS$", RegexOptions.IgnoreCase), new Regex("^RETURNS$", RegexOptions.IgnoreCase));
            Assert.IsTrue(rest.StartsWith("as\r\nbegin"));
        }

    }
}
