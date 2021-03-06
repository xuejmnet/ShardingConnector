using System;
using System.Collections.Generic;
using System.Text;
using ShardingConnector.Executor;
using ShardingConnector.Extensions;
using ShardingConnector.Merge.Reader.Stream;

namespace ShardingConnector.ShardingMerge.DQL.Iterator
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/5/6 8:07:35
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class IteratorStreamMergedDataReader:StreamMergedDataReader
    {
        private readonly IEnumerator<IStreamDataReader> _streamDataReaderEnumerator;

        public IteratorStreamMergedDataReader( List<IStreamDataReader> streamDataReaders)
        {
            _streamDataReaderEnumerator = streamDataReaders.GetEnumerator();
            SetCurrentStreamDataReader(this._streamDataReaderEnumerator.Next());
        }
        public override bool Read()
        {
            if (GetCurrentStreamDataReader().Read())
            {
                return true;
            }
            if (!_streamDataReaderEnumerator.MoveNext())
            {
                return false;
            }
            SetCurrentStreamDataReader(_streamDataReaderEnumerator.Current);
            var hasNext = GetCurrentStreamDataReader().Read();
            if (hasNext)
            {
                return true;
            }
            while (!hasNext && _streamDataReaderEnumerator.MoveNext())
            {
                SetCurrentStreamDataReader(_streamDataReaderEnumerator.Current);
                hasNext = GetCurrentStreamDataReader().Read();
            }
            return hasNext;
        }
    }
}
