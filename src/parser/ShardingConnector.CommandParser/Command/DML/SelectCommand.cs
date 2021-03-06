using System.Collections.Generic;
using ShardingConnector.CommandParser.Segment;
using ShardingConnector.CommandParser.Segment.DML;
using ShardingConnector.CommandParser.Segment.DML.Item;
using ShardingConnector.CommandParser.Segment.DML.Order;
using ShardingConnector.CommandParser.Segment.DML.Pagination.limit;
using ShardingConnector.CommandParser.Segment.Generic.Table;
using ShardingConnector.CommandParser.Segment.Predicate;

namespace ShardingConnector.CommandParser.Command.DML
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/10 9:39:02
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class SelectCommand : DMLCommand
    {
        public ProjectionsSegment Projections { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public readonly ICollection<TableReferenceSegment> TableReferences = new LinkedList<TableReferenceSegment>();
        public WhereSegment Where { get; set; }
        public GroupBySegment GroupBy { get; set; }
        public OrderBySegment OrderBy { get; set; }
        public LimitSegment Limit { get; set; }
        public SelectCommand ParentCommand { get; set; }
        public LockSegment Lock { get; set; }


        /// <summary>
        /// Get simple table segments.
        /// </summary>
        /// <returns></returns>
        public ICollection<SimpleTableSegment> GetSimpleTableSegments()
        {
            ICollection<SimpleTableSegment> result = new LinkedList<SimpleTableSegment>();
            foreach (var tableReference in TableReferences)
            {
                foreach (var simpleTableSegment in tableReference.GetTables())
                {
                    result.Add(simpleTableSegment);
                }
            }

            return result;
        }
    }
}