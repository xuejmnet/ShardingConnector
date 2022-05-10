﻿using System.Collections.Generic;

using ShardingConnector.CommandParser.Command;
using ShardingConnector.ParserBinder.Command;
using ShardingConnector.ParserBinder.Command.DML;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.RewriteEngine.Parameter.Builder;
using ShardingConnector.RewriteEngine.Parameter.Builder.Impl;
using ShardingConnector.RewriteEngine.Sql.Token.Generator;
using ShardingConnector.RewriteEngine.Sql.Token.Generator.Builder;
using ShardingConnector.RewriteEngine.Sql.Token.SimpleObject;

namespace ShardingConnector.RewriteEngine.Context
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 16:00:25
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class SqlRewriteContext
    {
        private readonly SchemaMetaData _schemaMetaData;
    
        private readonly ISqlCommandContext<ISqlCommand> _sqlCommandContext;
    
        private readonly string _sql;
    
        private readonly List<object> _parameters;

        private readonly IParameterBuilder _parameterBuilder;
    
        private readonly List<SqlToken> _sqlTokens = new List<SqlToken>();

        private readonly SqlTokenGenerators _sqlTokenGenerators = new SqlTokenGenerators();

        public SqlRewriteContext(SchemaMetaData schemaMetaData, ISqlCommandContext<ISqlCommand> sqlCommandContext, string sql, List<object> parameters)
        {
            this._schemaMetaData = schemaMetaData;
            this._sqlCommandContext = sqlCommandContext;
            this._sql = sql;
            this._parameters = parameters;
            AddSqlTokenGenerators(new DefaultTokenGeneratorBuilder().GetSqlTokenGenerators());
            if (sqlCommandContext is InsertCommandContext insertCommandContext)
            {
                _parameterBuilder = new GroupedParameterBuilder(insertCommandContext.GetGroupedParameters());
            }
            else
            {
                _parameterBuilder=new StandardParameterBuilder(parameters);
            }
        }


        /**
         * Add SQL token generators.
         * 
         * @param sqlTokenGenerators SQL token generators
         */
        public void AddSqlTokenGenerators(ICollection<ISqlTokenGenerator> sqlTokenGenerators)
        {
            this._sqlTokenGenerators.AddAll(sqlTokenGenerators);
        }

        /**
         * Generate SQL tokens.
         */
        public void GenerateSqlTokens()
        {
            _sqlTokens.AddRange(_sqlTokenGenerators.GenerateSqlTokens(_sqlCommandContext, _parameters, _schemaMetaData));
        }

        public List<SqlToken> GetSqlTokens()
        {
            return _sqlTokens;
        }

        public string GetSql()
        {
            return _sql;
        }

        public IParameterBuilder GetParameterBuilder()
        {
            return _parameterBuilder;
        }

        public SchemaMetaData GetSchemaMetaData()
        {
            return _schemaMetaData;
        }

        public List<object> GetParameters()
        {
            return _parameters;
        }

        public ISqlCommandContext<ISqlCommand> GetSqlCommandContext()
        {
            return _sqlCommandContext;

        }


    }
}
