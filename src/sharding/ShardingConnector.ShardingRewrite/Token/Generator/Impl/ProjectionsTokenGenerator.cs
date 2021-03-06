using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ShardingConnector.Base;
using ShardingConnector.CommandParser.Command;
using ShardingConnector.Extensions;
using ShardingConnector.ParserBinder.Command;
using ShardingConnector.ParserBinder.Command.DML;
using ShardingConnector.ParserBinder.Segment.Select.Projection;
using ShardingConnector.ParserBinder.Segment.Select.Projection.Impl;
using ShardingConnector.RewriteEngine.Sql.Token.Generator;
using ShardingConnector.RewriteEngine.Sql.Token.SimpleObject;
using ShardingConnector.ShardingRewrite.Token.SimpleObject;

namespace ShardingConnector.ShardingRewrite.Token.Generator.Impl
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/28 8:17:52
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
   public sealed class ProjectionsTokenGenerator: IOptionalSqlTokenGenerator<SelectCommandContext>, IIgnoreForSingleRoute
    {
        public SqlToken GenerateSqlToken(SelectCommandContext sqlCommandContext)
        {
            ICollection<string> derivedProjectionTexts = GetDerivedProjectionTexts(sqlCommandContext);
            return new ProjectionsToken(sqlCommandContext.GetProjectionsContext().GetStopIndex() + 1 + " ".Length, derivedProjectionTexts);
        }

        public SqlToken GenerateSqlToken(ISqlCommandContext<ISqlCommand>  sqlCommandContext)
        {
            return GenerateSqlToken((SelectCommandContext)sqlCommandContext);

        }

        public bool IsGenerateSqlToken(ISqlCommandContext<ISqlCommand> sqlCommandContext)
        {
            return sqlCommandContext is SelectCommandContext selectCommandContext && GetDerivedProjectionTexts(selectCommandContext).Any();

        }
        private ICollection<string> GetDerivedProjectionTexts(SelectCommandContext selectCommandContext)
        {
            ICollection<string> result = new LinkedList<string>();
            foreach (var projection in selectCommandContext.GetProjectionsContext().GetProjections())
            {
                if (projection is AggregationProjection aggregationProjection && aggregationProjection.GetDerivedAggregationProjections().Any()) {
                    result.AddAll(aggregationProjection.GetDerivedAggregationProjections().Select(GetDerivedProjectionText).ToList());
                } else if (projection is DerivedProjection derivedProjection) {
                    result.Add(GetDerivedProjectionText(derivedProjection));
                }
            }
            return result;
        }

        private string GetDerivedProjectionText( IProjection projection)
        {
            ShardingAssert.ShouldBeNotNull(projection.GetAlias(),"alias is required");
            if (projection is AggregationDistinctProjection aggregationDistinctProjection) {
                return aggregationDistinctProjection.GetDistinctInnerExpression() + " AS " + projection.GetAlias() + " ";
            }
            return projection.GetExpression() + " AS " + projection.GetAlias() + " ";
        }
}
}
