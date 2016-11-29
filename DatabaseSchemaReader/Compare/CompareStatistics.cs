using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.Compare
{
    class CompareStatistics
    {
        private readonly IList<CompareResult> _results;
        private readonly ComparisonWriter _writer;

        public CompareStatistics(IList<CompareResult> results, ComparisonWriter writer)
        {
            _results = results;
            _writer = writer;
        }

        public void Execute(DatabaseTable databaseTable, DatabaseTable compareTable)
        {
            var firstStatistics = databaseTable.Statistics;
            var secondStatistics = compareTable.Statistics;
            foreach (var statistics in firstStatistics)
            {
                var statisticsName = statistics.Name;
                var match = secondStatistics.FirstOrDefault(c => c.Name == statisticsName);
                if (match == null)
                {
                    CreateResult(ResultType.Delete, databaseTable, statisticsName,
                        _writer.DropStatistics(databaseTable, statistics));
                    continue;
                }
                if (!ColumnsEqual(statistics, match))
                {
                    CreateResult(ResultType.Add, databaseTable, statisticsName,
                       _writer.DropStatistics(databaseTable, statistics) + Environment.NewLine +
                       _writer.AddStatistics(databaseTable, match));
                }
            }

            foreach (var statistics in secondStatistics)
            {
                var statisticsName = statistics.Name;
                var firstConstraint = firstStatistics.FirstOrDefault(c => c.Name == statisticsName);
                if (firstConstraint == null)
                {
                    CreateResult(ResultType.Add, databaseTable, statisticsName,
                        _writer.AddStatistics(databaseTable, statistics));
                }
            }
        }

        private static bool ColumnsEqual(DatabaseStatistics first, DatabaseStatistics second)
        {
            if (first.Columns == null && second.Columns == null) return true; //same, both null
            if (first.Columns == null || second.Columns == null) return false; //one is null, they are different
            //the two sequences have the same names
            var columnNames1 = first.Columns.OrderBy(c => c.Ordinal).Select(c => c.Name);
            var columnNames2 = second.Columns.OrderBy(c => c.Ordinal).Select(c => c.Name);

            return columnNames1.SequenceEqual(columnNames2);
        }


        private void CreateResult(ResultType resultType, DatabaseTable table, string name, string script)
        {
            var result = new CompareResult
                {
                    SchemaObjectType = SchemaObjectType.Statistics,
                    ResultType = resultType,
                    TableName = table.Name,
                    SchemaOwner = table.SchemaOwner,
                    Name = name,
                    Script = script
                };
            _results.Add(result);
        }
    }
}
