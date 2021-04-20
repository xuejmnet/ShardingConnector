using System;
using System.Collections.Generic;
using ShardingConnector.CommandParser.Command;
using ShardingConnector.CommandParser.Command.DML;
using ShardingConnector.ParserBinder.Command.DML;
using ShardingConnector.ParserBinder.MetaData.Schema;

namespace ShardingConnector.ParserBinder
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 08 April 2021 21:51:28
* @Email: 326308290@qq.com
*/
    public class SqlCommandContextFactory
    {
        private SqlCommandContextFactory(){}
        public static ParserBinder.Command.ISqlCommandContext<ISqlCommand> NewInstance(SchemaMetaData schemaMetaData, string sql, List<object> parameters, ISqlCommand sqlCommand) {
            if(sqlCommand is DMLCommand dmlCommand)
            {
                return GetDMLCommandContext(schemaMetaData, sql, parameters, dmlCommand);
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

        private static ParserBinder.Command.ISqlCommandContext<ISqlCommand> GetDMLCommandContext(SchemaMetaData schemaMetaData, string sql, List<object> parameters, DMLCommand sqlCommand)
        {
            if (sqlCommand is SelectCommand selectCommand)
            {
                return new SelectCommandContext(schemaMetaData, sql, parameters, selectCommand);
            }
            //if (sqlStatement instanceof SelectStatement) {
            //    return new SelectStatementContext(schemaMetaData, sql, parameters, selectCommand);
            //}
            //if (sqlStatement instanceof UpdateStatement) {
            //    return new UpdateStatementContext((UpdateStatement)sqlStatement);
            //}
            //if (sqlStatement instanceof DeleteStatement) {
            //    return new DeleteStatementContext((DeleteStatement)sqlStatement);
            //}
            //if (sqlStatement instanceof InsertStatement) {
            //    return new InsertStatementContext(schemaMetaData, parameters, (InsertStatement)sqlStatement);
            //}
            throw new NotSupportedException($"Unsupported SQL statement `{sqlCommand.GetType().Name}`");
        }
    }
}