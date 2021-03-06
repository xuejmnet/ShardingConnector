using System;
using System.Collections.Generic;
using System.Data.Common;
using ShardingConnector.CommandParser.Command;
using ShardingConnector.CommandParser.Command.DML;
using ShardingConnector.ParserBinder.Command.DML;
using ShardingConnector.ParserBinder.MetaData.Schema;
using ShardingConnector.ShardingAdoNet;

namespace ShardingConnector.ParserBinder
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 08 April 2021 21:51:28
* @Email: 326308290@qq.com
*/
    public static class SqlCommandContextFactory
    {
        public static ParserBinder.Command.ISqlCommandContext<ISqlCommand> NewInstance(SchemaMetaData schemaMetaData, string sql, ParameterContext parameterContext, ISqlCommand sqlCommand) {
            if(sqlCommand is DMLCommand dmlCommand)
            {
                return GetDMLCommandContext(schemaMetaData, sql, parameterContext, dmlCommand);
            }
           
            //if (sqlCommand is DMLStatement) {
            //    return getDMLStatementContext(schemaMetaData, sql, parameters, (DMLStatement) sqlStatement);
            //}
            //if (sqlStatement instanceof DDLStatement) {
            //    return getDDLStatementContext((DDLStatement) sqlStatement);
            //}
            //if (sqlStatement instanceof DCLStatement) {
            //    return getDCLStatementContext((DCLStatement) sqlStatement);
            //}
            //if (sqlStatement instanceof DALStatement) {
            //    return getDALStatementContext((DALStatement) sqlStatement);
            //}
            return new ParserBinder.Command.GenericSqlCommandContext<ISqlCommand>(sqlCommand);
        }

        private static ParserBinder.Command.ISqlCommandContext<ISqlCommand> GetDMLCommandContext(SchemaMetaData schemaMetaData, string sql, ParameterContext parameterContext, DMLCommand sqlCommand)
        {
            if (sqlCommand is SelectCommand selectCommand)
            {
                return new SelectCommandContext(schemaMetaData, sql, parameterContext, selectCommand);
            }
            //if (sqlStatement instanceof SelectStatement) {
            //    return new SelectStatementContext(schemaMetaData, sql, parameters, selectCommand);
            //}
            if (sqlCommand is UpdateCommand updateCommand)
            {
                return new UpdateCommandContext(updateCommand);
            }
            if (sqlCommand is DeleteCommand deleteCommand)
            {
                return new DeleteCommandContext(deleteCommand);
            }
            if (sqlCommand is InsertCommand insertCommand) {
                return new InsertCommandContext(schemaMetaData, parameterContext, insertCommand);
            }
            throw new NotSupportedException($"Unsupported SQL statement `{sqlCommand.GetType().Name}`");
        }
    }
}