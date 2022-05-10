﻿using System;
using System.Collections.Generic;
using System.Text;

using ShardingConnector.CommandParser.Command;

namespace ShardingConnector.ParserEngine.Hook
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/20 10:04:11
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class ParsingHookManager
    {
        private static readonly ParsingHookManager Instance;
        private readonly ICollection<IParsingHook> _parsingHooks = NewInstanceServiceLoader.NewServiceInstances<IParsingHook>();

        static ParsingHookManager()
        {
            NewInstanceServiceLoader.Register<IParsingHook>();
            Instance = new ParsingHookManager();
        }

        private ParsingHookManager()
        {
            
        }
        public static ParsingHookManager GetInstance()
        {
            return Instance;
        }
        public void Start(string sql)
        {
            foreach (var parsingHook in _parsingHooks)
            {
                parsingHook.Start(sql);
            }
        }

        public void FinishSuccess(ISqlCommand sqlCommand)
        {
            foreach (var parsingHook in _parsingHooks)
            {
                parsingHook.FinishSuccess(sqlCommand);
            }
        }

        public void FinishFailure(Exception exception)
        {
            foreach (var parsingHook in _parsingHooks)
            {
                parsingHook.FinishFailure(exception);
            }
        }
    }
}
