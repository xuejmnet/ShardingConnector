using System;
using System.Collections.Generic;
using System.Text;
using ShardingConnector.Base;
using ShardingConnector.CommandParser.Segment.DML.Order.Item;
using ShardingConnector.CommandParser.Segment.Generic.Table;
using ShardingConnector.Executor;
using ShardingConnector.ParserBinder.Command.DML;
using ShardingConnector.ParserBinder.MetaData.Column;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.ParserBinder.MetaData.Table;
using ShardingConnector.ParserBinder.Segment.Select.OrderBy;

namespace ShardingConnector.ShardingMerge.DQL.OrderBy
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/5/6 8:23:16
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class OrderByValue : IComparable, IComparable<OrderByValue>
    {
        private readonly IStreamDataReader _streamDataReader;

        private readonly ICollection<OrderByItem> orderByItems;

        private readonly List<bool> orderValuesCaseSensitive;

        private List<IComparable> orderValues;

        public OrderByValue(IStreamDataReader streamDataReader, ICollection<OrderByItem> orderByItems, SelectCommandContext selectCommandContext, SchemaMetaData schemaMetaData)
        {
            this._streamDataReader = streamDataReader;
            this.orderByItems = orderByItems;
            this.orderValuesCaseSensitive = GetOrderValuesCaseSensitive(selectCommandContext, schemaMetaData);
        }

        private List<bool> GetOrderValuesCaseSensitive(SelectCommandContext selectCommandContext, SchemaMetaData schemaMetaData)
        {
            List<bool> result = new List<bool>(orderByItems.Count);
            foreach (var orderByItem in orderByItems)
            {
                result.Add(GetOrderValuesCaseSensitiveFromTables(selectCommandContext, schemaMetaData, orderByItem));
            }

            return result;
        }

        private bool GetOrderValuesCaseSensitiveFromTables(SelectCommandContext selectCommandContext, SchemaMetaData schemaMetaData, OrderByItem eachOrderByItem)
        {
            foreach (var simpleTableSegment in selectCommandContext.GetAllTables())
            {
                var tableName = simpleTableSegment.GetTableName().GetIdentifier().GetValue();
                var tableMetaData = schemaMetaData.Get(tableName);
                IDictionary<String, ColumnMetaData> columns = tableMetaData.GetColumns();
                var orderByItemSegment = eachOrderByItem.GetSegment();
                if (orderByItemSegment is ColumnOrderByItemSegment columnOrderByItemSegment)
                {
                    String columnName = columnOrderByItemSegment.GetColumn().GetIdentifier().GetValue();
                    if (columns.ContainsKey(columnName))
                    {
                        return columns[columnName].CaseSensitive;
                    }
                }
                else if (orderByItemSegment is IndexOrderByItemSegment indexOrderByItemSegment)
                {
                    int columnIndex = indexOrderByItemSegment.GetColumnIndex();
                    String columnName = _streamDataReader.GetColumnName(columnIndex);
                    if (columns.ContainsKey(columnName))
                    {
                        return columns[columnName].CaseSensitive;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool MoveNext()
        {
            var result = _streamDataReader.Read();
            orderValues = result ? GetOrderValues() : new List<IComparable>(0);
            return result;
        }

        private List<IComparable> GetOrderValues()
        {
            var result = new List<IComparable>(orderByItems.Count);
            foreach (var orderByItem in orderByItems)
            {
                var value = _streamDataReader.GetValue(orderByItem.GetIndex());
                ShardingAssert.If(value == null || !(value is IComparable), "Order by value must implements Comparable");
                result.Add((IComparable) value);
            }

            return result;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((OrderByValue) obj);
        }

        public int CompareTo(OrderByValue other)
        {
            int i = 0;
            foreach (var orderByItem in orderByItems)
            {
                int result = CompareUtil.CompareTo(orderValues[i], other.orderValues[i], orderByItem.GetSegment().GetOrderDirection(),
                    orderByItem.GetSegment().GetNullOrderDirection(), orderValuesCaseSensitive[i]);
                if (0 != result)
                {
                    return result;
                }

                i++;
            }

            return 0;
        }

        public IStreamDataReader GetStreamDataReader()
        {
            return _streamDataReader;
        }
    }
}