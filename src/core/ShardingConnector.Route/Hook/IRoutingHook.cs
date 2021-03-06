using System;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.Route.Context;

namespace ShardingConnector.Route.Hook
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/13 15:45:40
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IRoutingHook
    {
        void Start(string sql);

        void FinishSuccess(RouteContext routeContext, SchemaMetaData schemaMetaData);

        
        void FinishFailure(Exception cause);
    }
}
