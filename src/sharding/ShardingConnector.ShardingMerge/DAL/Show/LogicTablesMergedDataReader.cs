using System;
using System.Collections.Generic;
using System.Linq;

using ShardingConnector.CommandParser.Command;
using ShardingConnector.Executor;
using ShardingConnector.Extensions;
using ShardingConnector.Merge.Reader.Memory;
using ShardingConnector.ParserBinder.Command;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.ShardingCommon.Core.Rule;

namespace ShardingConnector.ShardingMerge.DAL.Show
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/5/5 10:48:06
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public class LogicTablesMergedDataReader:MemoryMergedDataReader<ShardingRule>
    {
        public LogicTablesMergedDataReader(ShardingRule rule, SchemaMetaData schemaMetaData, ISqlCommandContext<ISqlCommand> sqlCommandContext, List<IStreamDataReader> streamDataReaders) : base(rule, schemaMetaData, sqlCommandContext, streamDataReaders)
        {
        }

        protected override List<MemoryQueryResultRow> Init(ShardingRule rule, SchemaMetaData schemaMetaData, ISqlCommandContext<ISqlCommand> sqlCommandContext,
            List<IStreamDataReader> streamDataReaders)
        {
            ICollection<MemoryQueryResultRow> result = new LinkedList<MemoryQueryResultRow>();
            var tableNames = new HashSet<string>();
            foreach (var streamDataReader in streamDataReaders)
            {
                while (streamDataReader.Read()) {
                    MemoryQueryResultRow memoryResultSetRow = new MemoryQueryResultRow(streamDataReader);
                    var actualTableName = memoryResultSetRow.GetCell(0).ToString();
                    var tableRule = rule.FindTableRuleByActualTable(actualTableName);
                    if (tableRule==null) {
                        if (rule.TableRules.IsEmpty() || schemaMetaData.ContainsTable(actualTableName) && tableNames.Add(actualTableName)) {
                            result.Add(memoryResultSetRow);
                        }
                    } else if (tableNames.Add(tableRule.LogicTable)) {
                        memoryResultSetRow.SetCell(1, tableRule.LogicTable);
                        SetCellValue(memoryResultSetRow, tableRule.LogicTable, actualTableName);
                        result.Add(memoryResultSetRow);
                    }
                }
            }
            return result.ToList();
        }
    
        protected virtual void SetCellValue( MemoryQueryResultRow memoryResultSetRow,  string logicTableName,  string actualTableName) {
        }
    }
}
