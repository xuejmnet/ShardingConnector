using System;
using System.Collections.Generic;
using System.Linq;
using ShardingConnector.Parser.Sql.Segment.DML.Assignment;
using ShardingConnector.Parser.Sql.Segment.DML.Column;
using ShardingConnector.Parser.Sql.Segment.DML.Expr;
using ShardingConnector.Parser.Sql.Segment.Generic.Table;

namespace ShardingConnector.Parser.Sql.Command.DML
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: Monday, 12 April 2021 22:38:40
    * @Email: 326308290@qq.com
    */
    public sealed class InsertCommand : DMLCommand
    {

        public SimpleTableSegment Table;

        public InsertColumnsSegment InsertColumns;

        public SetAssignmentSegment SetAssignment;

        public OnDuplicateKeyColumnsSegment OnDuplicateKeyColumns;

        public readonly ICollection<InsertValuesSegment> Values = new LinkedList<InsertValuesSegment>();

        public List<ColumnSegment> GtColumns()
        {
            return null == InsertColumns ? new List<ColumnSegment>(0) : InsertColumns.GetColumns();
        }



        public bool UseDefaultColumns()
        {
            return !GtColumns().Any() && null == SetAssignment;
        }

        /**
         * Get column names.
         *
         * @return column names
         */
        public List<string> GetColumnNames()
        {
            return null == SetAssignment ? GetColumnNamesForInsertColumns() : GetColumnNamesForSetAssignment();
        }

        private List<string> GetColumnNamesForInsertColumns()
        {
            List<string> result = new List<string>(GtColumns().Count);
            foreach (var column in GtColumns())
            {
                result.Add(column.GetIdentifier().GetValue().ToLower());
            }
            return result;
        }

        private List<string> GetColumnNamesForSetAssignment()
        {
            List<string> result = new List<string>(SetAssignment.GetAssignments().Count);
            foreach (var assignment in SetAssignment.GetAssignments())
            {
                result.Add(assignment.GetColumn().GetIdentifier().GetValue().ToLower());
            }
            return result;
        }

        /**
         * Get value list count.
         *
         * @return value list count
         */
        public int GetValueListCount()
        {
            return null == SetAssignment ? Values.Count : 1;
        }

        /**
         * Get value count for per value list.
         * 
         * @return value count
         */
        public int GetValueCountForPerGroup()
        {
            if (Values.Any())
            {
                return Values.First().GetValues().Count;
            }
            if (null != SetAssignment)
            {
                return SetAssignment.GetAssignments().Count;
            }
            return 0;
        }

        /**
         * Get all value expressions.
         * 
         * @return all value expressions
         */
        public List<List<IExpressionSegment>> GetAllValueExpressions()
        {
            return null == SetAssignment ? GetAllValueExpressionsFromValues() : new List<List<IExpressionSegment>>(){ GetAllValueExpressionsFromSetAssignment() };
        }

        private List<List<IExpressionSegment>> GetAllValueExpressionsFromValues()
        {
            List<List<IExpressionSegment>> result = new List<List<IExpressionSegment>>(Values.Count);
            foreach (var insertValues in Values)
            {
                result.Add(insertValues.GetValues());
            }
            return result;
        }

        private List<IExpressionSegment> GetAllValueExpressionsFromSetAssignment()
        {
            List<IExpressionSegment> result = new List<IExpressionSegment>(SetAssignment.GetAssignments().Count);
            foreach (var assignment in SetAssignment.GetAssignments())
            {
                result.Add(assignment.GetValue());

            }
            return result;
        }
    }
}