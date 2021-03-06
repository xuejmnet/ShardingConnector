using ShardingConnector.Common.Config.Properties;
using ShardingConnector.Common.Rule;
using ShardingConnector.Spi.Order;

namespace ShardingConnector.RewriteEngine.Context
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 15:59:18
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface ISqlRewriteContextDecorator: IOrderAware
    {
        void Decorate(IBaseRule rule, ConfigurationProperties properties, SqlRewriteContext sqlRewriteContext);
    }
    public interface ISqlRewriteContextDecorator<in T>: ISqlRewriteContextDecorator where T: IBaseRule
    {
        void Decorate(T rule, ConfigurationProperties properties, SqlRewriteContext sqlRewriteContext);
    }
}
