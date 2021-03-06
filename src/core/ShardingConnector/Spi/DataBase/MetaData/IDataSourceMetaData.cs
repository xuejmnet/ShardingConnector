using System;
using System.Collections.Generic;
using System.Text;

namespace ShardingConnector.Spi.DataBase.MetaData
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/13 14:52:27
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IDataSourceMetaData
    {
        string GetHostName();

        int GetPort();

        string GetCatalog();

        string GetSchema();
    }
}
