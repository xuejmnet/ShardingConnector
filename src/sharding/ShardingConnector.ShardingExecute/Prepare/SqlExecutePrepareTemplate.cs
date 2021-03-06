using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ShardingConnector.Executor.Constant;
using ShardingConnector.Executor.Context;
using ShardingConnector.Executor.Engine;
using ShardingConnector.Extensions;
using ShardingConnector.ShardingExecute.Execute;

namespace ShardingConnector.ShardingExecute.Prepare
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/04/16 09:19:32
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */

    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlExecutePrepareTemplate
    {
        private readonly int _maxConnectionsSizePerQuery;

        //
        // List<DbConnection> GetConnections(ConnectionModeEnum connectionMode, string dataSourceName, int connectionSize);
        //
        // CommandExecuteUnit CreateCommandExecuteUnit(DbConnection connection, ExecutionUnit executionUnit, ConnectionModeEnum connectionMode);

        public SqlExecutePrepareTemplate(int maxConnectionsSizePerQuery)
        {
            _maxConnectionsSizePerQuery = maxConnectionsSizePerQuery;
        }

        /**
     * Get execute unit groups.
     *
     * @param executionUnits execution units
     * @param callback SQL execute prepare callback
     * @return statement execute unit groups
     * @throws SQLException SQL exception
     */
        public ICollection<InputGroup<CommandExecuteUnit>> GetExecuteUnitGroups(
            ICollection<ExecutionUnit> executionUnits, ISqlExecutePrepareCallback callback)
        {
            return GetSynchronizedExecuteUnitGroups(executionUnits, callback);
        }

        private ICollection<InputGroup<CommandExecuteUnit>> GetSynchronizedExecuteUnitGroups(
            ICollection<ExecutionUnit> executionUnits, ISqlExecutePrepareCallback callback)
        {
            IDictionary<string, List<SqlUnit>> sqlUnitGroups = GetSqlUnitGroups(executionUnits);
            ICollection<InputGroup<CommandExecuteUnit>> result = new LinkedList<InputGroup<CommandExecuteUnit>>();
            foreach (var sqlUnitGroup in sqlUnitGroups)
            {
                result.AddAll(GetSqlExecuteGroups(sqlUnitGroup.Key, sqlUnitGroup.Value, callback));
            }

            return result;
        }

        private IDictionary<string, List<SqlUnit>> GetSqlUnitGroups(ICollection<ExecutionUnit> executionUnits)
        {
            IDictionary<string, List<SqlUnit>> result = new Dictionary<string, List<SqlUnit>>(executionUnits.Count);
            foreach (var executionUnit in executionUnits)
            {
                if (!result.ContainsKey(executionUnit.GetDataSourceName()))
                {
                    result.Add(executionUnit.GetDataSourceName(), new List<SqlUnit>());
                }

                result[executionUnit.GetDataSourceName()].Add(executionUnit.GetSqlUnit());
            }

            return result;
        }

        /// <summary>
        /// 优化主要包括的是支持顺序查询
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <param name="sqlUnits"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private List<InputGroup<CommandExecuteUnit>> GetSqlExecuteGroups(string dataSourceName, List<SqlUnit> sqlUnits,
            ISqlExecutePrepareCallback callback)
        {
            ICollection<InputGroup<CommandExecuteUnit>> result = new LinkedList<InputGroup<CommandExecuteUnit>>();
            int desiredPartitionSize =
                Math.Max(
                    0 == sqlUnits.Count % _maxConnectionsSizePerQuery
                        ? sqlUnits.Count / _maxConnectionsSizePerQuery
                        : sqlUnits.Count / _maxConnectionsSizePerQuery + 1, 1);
            List<List<SqlUnit>> sqlUnitPartitions = sqlUnits
                .Select((o, i) => new {Obj = o, index = i % desiredPartitionSize}).GroupBy(o => o.index)
                .Select(o => o.Select(g => g.Obj).ToList()).ToList();
            ConnectionModeEnum connectionMode = _maxConnectionsSizePerQuery < sqlUnits.Count
                ? ConnectionModeEnum.CONNECTION_STRICTLY
                : ConnectionModeEnum.MEMORY_STRICTLY;
            List<DbConnection> connections =
                callback.GetConnections(connectionMode, dataSourceName, sqlUnitPartitions.Count);
            int count = 0;
            foreach (var item in sqlUnitPartitions)
            {
                result.Add(GetSqlExecuteGroup(connectionMode, connections[count++], dataSourceName, item, callback));
            }

            return result.ToList();
        }

        private InputGroup<CommandExecuteUnit> GetSqlExecuteGroup(ConnectionModeEnum connectionMode,
            DbConnection connection,
            string dataSourceName, List<SqlUnit> sqlUnitGroup, ISqlExecutePrepareCallback callback)
        {
            List<CommandExecuteUnit> result = new List<CommandExecuteUnit>();
            foreach (var sqlUnit in sqlUnitGroup)
            {
                result.Add(callback.CreateCommandExecuteUnit(connection, new ExecutionUnit(dataSourceName, sqlUnit),
                    connectionMode));
            }

            return new InputGroup<CommandExecuteUnit>(result);
        }
    }
}