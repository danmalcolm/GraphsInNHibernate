using System.Data;
using NHibernate.Driver;

namespace Network.NHibernate
{
    public class InterceptingSqlClientDriver : SqlClientDriver
    {
        public override System.Data.IDbCommand GenerateCommand(System.Data.CommandType type, global::NHibernate.SqlCommand.SqlString sqlString, global::NHibernate.SqlTypes.SqlType[] parameterTypes)
        {
            var dbCommand = base.GenerateCommand(type, sqlString, parameterTypes);
            return dbCommand;
        }

        protected override void OnBeforePrepare(IDbCommand command)
        {
            base.OnBeforePrepare(command);
        }

        public override IDbCommand CreateCommand()
        {
            return new InterceptingCommand(base.CreateCommand());
        }

        public override void ExpandQueryParameters(IDbCommand cmd, global::NHibernate.SqlCommand.SqlString sqlString)
        {
            base.ExpandQueryParameters(cmd, sqlString);
        }

        public override void AdjustCommand(IDbCommand command)
        {
            base.AdjustCommand(command);
        }

    }
}