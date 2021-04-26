﻿using System.Data.Common;
using ShardingConnector.NewConnector.DataSource;

namespace ShardingConnector.AdoNet.AdoNet.Core.DataSource
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/19 13:17:30
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class GenericDataSource:IDataSource
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly string _connectionString;

        public GenericDataSource(DbProviderFactory dbProviderFactory,string connectionString)
        {
            _dbProviderFactory = dbProviderFactory;
            _connectionString = connectionString;
        }

        public DbConnection GetDbConnection()
        {
            var dbConnection = _dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;
            return dbConnection;
        }
    }
}