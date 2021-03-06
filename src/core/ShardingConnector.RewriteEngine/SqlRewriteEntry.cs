using System.Collections.Generic;
using System.Data.Common;
using ShardingConnector.CommandParser.Command;
using ShardingConnector.Common.Config.Properties;
using ShardingConnector.Common.Rule;
using ShardingConnector.ParserBinder.Command;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.RewriteEngine.Context;
using ShardingConnector.RewriteEngine.Sql.Token.Generator.Aware;
using ShardingConnector.Route.Context;
using ShardingConnector.ShardingAdoNet;

namespace ShardingConnector.RewriteEngine
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 15:02:38
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class SqlRewriteEntry
    {
        private readonly SchemaMetaData _schemaMetaData;
    
        private readonly ConfigurationProperties _properties;
    
        private readonly IDictionary<IBaseRule, ISqlRewriteContextDecorator> _decorators = new Dictionary<IBaseRule, ISqlRewriteContextDecorator>();

        public SqlRewriteEntry(SchemaMetaData schemaMetaData, ConfigurationProperties properties)
        {
            _schemaMetaData = schemaMetaData;
            _properties = properties;
        }

        /**
         * Register route decorator.
         *
         * @param rule rule
         * @param decorator SQL rewrite context decorator
         */
        public void RegisterDecorator(IBaseRule rule, ISqlRewriteContextDecorator decorator)
        {
            _decorators.Add(rule, decorator);
        }

        /**
         * Create SQL rewrite context.
         * 
         * @param sql SQL
         * @param parameters parameters
         * @param sqlStatementContext SQL statement context
         * @param routeContext route context
         * @return SQL rewrite context
         */
        public SqlRewriteContext CreateSqlRewriteContext(string sql, ParameterContext parameterContext,  ISqlCommandContext<ISqlCommand> sqlCommandContext, RouteContext routeContext)
        {
            var result = new SqlRewriteContext(_schemaMetaData, sqlCommandContext, sql, parameterContext);
            Decorate(_decorators, result, routeContext);
            result.GenerateSqlTokens();
            return result;
        }
        
        private void Decorate(IDictionary<IBaseRule, ISqlRewriteContextDecorator> decorators, SqlRewriteContext sqlRewriteContext, RouteContext routeContext)
        {
            foreach (var decoratorEntry in decorators)
            {
                var rule = decoratorEntry.Key;
                var decorator = decoratorEntry.Value;
                if (decorator is IRouteContextAware routeContextAware) {
                    routeContextAware.SetRouteContext(routeContext);
                }
                decorator.Decorate(rule, _properties, sqlRewriteContext);
            }
        }
}
}
