using System.Collections.Generic;
using System.Data.Common;
using ShardingConnector.ShardingAdoNet;

namespace ShardingConnector.RewriteEngine.Parameter.Builder
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 16:02:08
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IParameterBuilder
    {
        ParameterContext GetParameterContext();
    }
}
