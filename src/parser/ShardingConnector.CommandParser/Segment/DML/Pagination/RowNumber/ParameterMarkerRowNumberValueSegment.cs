using System;

namespace ShardingConnector.CommandParser.Segment.DML.Pagination.RowNumber
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/12 12:25:22
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class ParameterMarkerRowNumberValueSegment:RowNumberValueSegment,IParameterMarkerPaginationValueSegment
    {
        private readonly int _parameterIndex;
        private readonly string _parameterName;

        public ParameterMarkerRowNumberValueSegment(int startIndex, int stopIndex,int parameterIndex, string parameterName, bool boundOpened) : base(startIndex, stopIndex, boundOpened)
        {
            this._parameterIndex = parameterIndex;
            _parameterName = NormalizeParameterName(parameterName);
        }
        internal static string NormalizeParameterName(string name)
        {
            return name.StartsWith("@", StringComparison.Ordinal) || name.StartsWith("?", StringComparison.Ordinal) ? name.Substring(1) : name;
        }

        public int GetParameterIndex()
        {
            return _parameterIndex;
        }

        public string GetParameterName()
        {
            return _parameterName;
        }
    }
}
