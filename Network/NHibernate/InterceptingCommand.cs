using System;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Xml;

namespace Network.NHibernate
{
    public class InterceptingCommand : IDbCommand
    {
        private readonly SqlCommand inner;

        public InterceptingCommand(IDbCommand command)
        {
            inner = (SqlCommand)command;
        }


        public void Dispose()
        {
            inner.Dispose();
        }

        public void Prepare()
        {
            inner.Prepare();
        }

        public void Cancel()
        {
            inner.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return inner.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            LogCommand();
            return inner.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            LogCommand();
            return inner.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            LogCommand();
            return inner.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            LogCommand();
            return inner.ExecuteScalar();
        }

        public IDbConnection Connection
        {
            get { return inner.Connection; }
            set { inner.Connection = (SqlConnection) value; }
        }

        public IDbTransaction Transaction
        {
            get { return inner.Transaction; }
            set { inner.Transaction = (SqlTransaction) value; }
        }

        public string CommandText
        {
            get { return inner.CommandText; }
            set { inner.CommandText = value; }
        }
        
        public int CommandTimeout
        {
            get { return inner.CommandTimeout; }
            set { inner.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return inner.CommandType; }
            set { inner.CommandType = value; }
        }

        public IDataParameterCollection Parameters
        {
            get { return inner.Parameters; }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get { return inner.UpdatedRowSource; }
            set { inner.UpdatedRowSource = value; }
        }

        private void LogCommand()
        {
            Console.WriteLine(inner.CommandText);
        }
    }
}