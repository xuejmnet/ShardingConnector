﻿using System;
using System.Collections.Generic;
using System.Text;
using ShardingConnector.Kernels.Route.Rule;

namespace ShardingConnector.Kernels.Route
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/3/30 13:00:50
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class DataNodeRouter
    {
        private readonly IDictionary<IBaseRule, IRouteDecorator<IBaseRule>> _decorators =
            new Dictionary<IBaseRule, IRouteDecorator<IBaseRule>>();
        public void RegisterDecorator(IBaseRule rule,IRouteDecorator<IBaseRule> decorator)
        {
            _decorators.Add(rule,decorator);
        }

        public RouteContext Route(string sql,List<object> parameters)
        {

        }

        private RouteContext ExecuteRoute(string sql, List<object> parameters)
        {

        }

        private RouteContext CreateRouteContext(string sql, List<object> parameters)
        {

        }
    }
}