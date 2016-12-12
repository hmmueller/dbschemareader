using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.SqlServer {
    public class SymbolParser
    {
        public static string SkipSymbolsUntil(string sql, params Regex[] stops) {
            for (;;)
            {
                string next = NextSym(sql);
                if (next == null || stops.Any(stop => stop.IsMatch(next)))
                {
                    break;
                }
                sql = sql.Substring(next.Length);
            }
            return sql;
        }

        private readonly static Regex[] _symbolRegexes = {
            new Regex(@"^\s+"), // Whitespace
            new Regex(@"^[\p{L}\p{N}@_$]+"), // Some name or number
            new Regex(@"^\[[^\]]*\]"), // [] escaped name
            new Regex(@"^""[^""]*"""), // " escaped name
            new Regex(@"^'([^']|'')*'"), // string
            new Regex(@"^--.*\r\n"), // -- comment
            new Regex(@"^/\*(.|[\r\n])*?\*/"), // /* comment
            new Regex(@"^[!%&/()=?{}+*~#:.,;<>|^\\-]"), // single symbol
        };

        public static string NextSym(string sql)
        {
            return _symbolRegexes.Select(r => r.Match(sql)).Where(m => m.Success).Select(m => m.Value).FirstOrDefault();
        }
    }
}