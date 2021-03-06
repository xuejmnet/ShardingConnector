using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShardingConnector.Common.Rule;
using ShardingConnector.Exceptions;
using ShardingConnector.Extensions;
using ShardingConnector.Logger;
using ShardingConnector.NewConnector.DataSource;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.ParserBinder.MetaData.Table;
using ShardingConnector.ShardingCommon.Core.Rule;
using ShardingConnector.Spi.DataBase.DataBaseType;

namespace ShardingConnector.ShardingCommon.Core.MetaData
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/19 13:33:41
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class ShardingMetaDataLoader
    {
        private static readonly ILogger<ShardingMetaDataLoader> _logger =
            InternalLoggerFactory.CreateLogger<ShardingMetaDataLoader>();
        private static readonly int CORES = Environment.ProcessorCount;

        private static readonly int FUTURE_GET_TIME_OUT_SEC = 5;

        private readonly IDictionary<string, IDataSource> dataSourceMap;

        private readonly ShardingRule shardingRule;

        private readonly int maxConnectionsLimitSize;

        private readonly bool isCheckingMetaData;

        public ShardingMetaDataLoader(IDictionary<string, IDataSource> dataSourceMap, ShardingRule shardingRule, int maxConnectionsLimitSize, bool isCheckingMetaData)
        {
            this.dataSourceMap = dataSourceMap;
            this.shardingRule = shardingRule;
            this.maxConnectionsLimitSize = maxConnectionsLimitSize;
            this.isCheckingMetaData = isCheckingMetaData;
        }
        /**
         * Load table meta data.
         *
         * @param logicTableName logic table name
         * @param databaseType database type
         * @return table meta data
         * @throws SQLException SQL exception
         */
        public TableMetaData Load(string logicTableName, IDatabaseType databaseType)
        {
            TableRule tableRule = shardingRule.GetTableRule(logicTableName);
            if (!isCheckingMetaData)
            {
                DataNode dataNode = tableRule.ActualDataNodes.First();
                var rawMasterDataSourceName = shardingRule.ShardingDataSourceNames.GetRawMasterDataSourceName(dataNode.GetDataSourceName());
                return TableMetaDataLoader.Load(dataSourceMap[rawMasterDataSourceName], dataNode.GetTableName(), databaseType.GetName());
            }
            var dataNodeGroups = tableRule.GetDataNodeGroups();
            ConcurrentDictionary<string, TableMetaData> actualTableMetaDataMap = new ConcurrentDictionary<string, TableMetaData>();

            var tasks = dataNodeGroups.SelectMany(o => o.Value.Select(dataNode => Task.Run(() =>
                {
                    var tableMetaData = Load(dataNode, databaseType);
                    actualTableMetaDataMap.TryAdd(dataNode.GetTableName(), tableMetaData);
                }))).ToArray();
             Task.WaitAll(tasks);


            CheckUniformed(logicTableName, actualTableMetaDataMap);
            return actualTableMetaDataMap.Select(o => o.Value).FirstOrDefault();
        }

        private TableMetaData Load(DataNode dataNode, IDatabaseType databaseType)
        {
            try
            {
                return TableMetaDataLoader.Load(dataSourceMap[dataNode.GetDataSourceName()], dataNode.GetTableName(), databaseType.GetName());
            }
            catch (Exception e)
            {
                throw new ShardingException($"SqlException for DataNode={dataNode} and databaseType={databaseType.GetName()}", e);
            }
        }

        /**
         * Load schema Meta data.
         *
         * @param databaseType database type
         * @return schema Meta data
         * @throws SQLException SQL exception
         */
        public SchemaMetaData Load(IDatabaseType databaseType)
        {
            SchemaMetaData result = LoadShardingSchemaMetaData(databaseType);
            result.Merge( LoadDefaultSchemaMetaData(databaseType));
            return result;
        }

        private SchemaMetaData LoadShardingSchemaMetaData(IDatabaseType databaseType)
        {
            _logger.LogInformation($"Loading {shardingRule.TableRules.Count} logic tables' meta data.");
            IDictionary<string, TableMetaData> tableMetaDataMap = new Dictionary<string, TableMetaData>(shardingRule.TableRules.Count);
            foreach (var tableRule in shardingRule.TableRules)
            {
                tableMetaDataMap.Add(tableRule.LogicTable,  Load(tableRule.LogicTable, databaseType));
            }
            return new SchemaMetaData(tableMetaDataMap);
        }

        private SchemaMetaData LoadDefaultSchemaMetaData(IDatabaseType databaseType)
        {
            var actualDefaultDataSourceName = shardingRule.FindActualDefaultDataSourceName();
            if (actualDefaultDataSourceName != null)
            {
                return  SchemaMetaDataLoader.Load(dataSourceMap[actualDefaultDataSourceName], maxConnectionsLimitSize, databaseType.GetName());
            }
            return new SchemaMetaData(new Dictionary<string, TableMetaData>(0));
        }


        private void CheckUniformed(string logicTableName, IDictionary<string, TableMetaData> actualTableMetaDataMap)
        {
            var decorator = new ShardingTableMetaDataDecorator();
            var sample = decorator.Decorate(actualTableMetaDataMap.Select(o => o.Value).FirstOrDefault(), logicTableName, shardingRule);
            var violations = actualTableMetaDataMap
                .Where(o => !sample.Equals(decorator.Decorate(o.Value, logicTableName, shardingRule)))
                .Select(o => new TableMetaDataViolation(o.Key, o.Value)).ToList();

            throwExceptionIfNecessary(violations, logicTableName);
        }

        private void throwExceptionIfNecessary(ICollection<TableMetaDataViolation> violations, string logicTableName)
        {
            if (violations.Any())
            {
                StringBuilder errorMessage = new StringBuilder(
                    $"Cannot get uniformed table structure for logic table `{logicTableName}`, it has different meta data of actual tables are as follows:").Append(Environment.NewLine);
                foreach (var violation in violations)
                {
                    errorMessage.Append("actual table: ").Append(violation.ActualTableName).Append(", meta data: ").Append(violation.TableMetaData).Append(Environment.NewLine);
                }
                throw new ShardingException(errorMessage.ToString());
            }
        }

        private class TableMetaDataViolation
        {
            public TableMetaDataViolation(string actualTableName, TableMetaData tableMetaData)
            {
                this.ActualTableName = actualTableName;
                this.TableMetaData = tableMetaData;
            }

            public String ActualTableName { get; }

            public TableMetaData TableMetaData { get; }
        }
    }
}
