// using System;
// using System.Collections.Generic;
// using System.Data.Common;
// using ShardingConnector.AdoNet.AdoNet.Core.Context;
// using ShardingConnector.Api.Database.DatabaseType;
// using ShardingConnector.Common.Rule;
// using ShardingConnector.Exceptions;
// using ShardingConnector.NewConnector.DataSource;
// using ShardingConnector.Spi.DataBase.DataBaseType;
//
// namespace ShardingConnector.AdoNet.AdoNet.Abstraction
// {
//     /*
//     * @Author: xjm
//     * @Description:
//     * @Date: 2021/04/14 10:39:39
//     * @Ver: 1.0
//     * @Email: 326308290@qq.com
//     */
//
//     /// <summary>
//     /// 
//     /// </summary>
//     public abstract class AbstractDbProviderFactory : DbProviderFactory
//     {
//
//
//         public AbstractDbProviderFactory(IDictionary<string, IDataSource> dataSourceMap)
//         {
//             this.DataSourceMap = dataSourceMap;
//             _databaseType = CreateDatabaseType();
//         }
//
//         public AbstractDbProviderFactory(IDataSource dataSource):this(new Dictionary<string, IDataSource>() { { "unique", dataSource } })
//         {
//         }
//
//         private IDatabaseType CreateDatabaseType()
//         {
//             IDatabaseType result = null;
//             foreach (var dataSource in DataSourceMap)
//             {
//                 IDatabaseType databaseType = CreateDatabaseType(dataSource.Value);
//                 var flag = result == null || result == _databaseType;
//                 if (!flag)
//                 {
//                     throw new ShardingException($"Database type inconsistent with '{result}' and '{databaseType}'");
//                 }
//
//                 result = databaseType;
//             }
//
//             return result;
//         }
//
//         private IDatabaseType CreateDatabaseType(IDataSource dataSource)
//         {
//             if (dataSource is AbstractDbProviderFactory abstractDataSourceAdapter)
//             {
//                 return abstractDataSourceAdapter._databaseType;
//             }
//
//             using (var connection = dataSource.CreateConnection())
//             {
//                 return DatabaseTypes.GetDataBaseTypeByDbConnection(connection);
//             }
//         }
//
//         public IDatabaseType GetDatabaseType()
//         {
//             return _databaseType;
//         }
//         protected abstract IRuntimeContext<IBaseRule> GetRuntimeContext();
//     }
// }