using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using ShardingConnector.CommandParser.Command;
using ShardingConnector.ShardingAdoNet;
using ShardingConnector.ShardingCommon.Core.Rule;

namespace ShardingConnector.ShardingRoute.Engine.Validator
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/28 11:23:13
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IShardingCommandValidator
    {
        void Validate(ShardingRule shardingRule, ISqlCommand sqlCommand, ParameterContext parameterContext);
    }
    public interface IShardingCommandValidator<in T>: IShardingCommandValidator where T:ISqlCommand
    {
        void Validate(ShardingRule shardingRule, T sqlCommand, ParameterContext parameterContext);
    }
}
