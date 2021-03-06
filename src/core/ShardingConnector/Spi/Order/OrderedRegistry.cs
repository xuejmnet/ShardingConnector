using System;
using System.Collections.Generic;

namespace ShardingConnector.Spi.Order
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/5 13:01:20
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class OrderedRegistry
    {
        public static ICollection<T> GetRegisteredOrderedAware<T>() where T: IOrderAware
        {
            IDictionary<int, T> result = new SortedDictionary<int, T>();
            var newServiceInstances = NewInstanceServiceLoader.NewServiceInstances<T>();
            foreach (var serviceInstance in newServiceInstances)
            {
                result.Add(serviceInstance.GetOrder(), serviceInstance);
            }

            return result.Values;
        }
        public static ICollection<IOrderAware> GetRegisteredOrderedAware(Type type)
        {
            IDictionary<int, IOrderAware> result = new SortedDictionary<int, IOrderAware>();
            var newServiceInstances = NewInstanceServiceLoader.NewServiceInstances(type);
            foreach (var serviceInstance in newServiceInstances)
            {
                var serviceOrderWareInstance = (IOrderAware)serviceInstance;
                result.Add(serviceOrderWareInstance.GetOrder(), serviceOrderWareInstance);
            }

            return result.Values;
        }
    }
}
