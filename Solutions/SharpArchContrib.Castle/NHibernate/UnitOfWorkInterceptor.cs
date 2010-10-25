namespace SharpArchContrib.Castle.NHibernate
{
    using System;

    using SharpArch.Data.NHibernate;

    using SharpArchContrib.Core.Logging;
    using SharpArchContrib.Data.NHibernate;

    public class UnitOfWorkInterceptor : TransactionInterceptor
    {
        public UnitOfWorkInterceptor(ITransactionManager transactionManager, IExceptionLogger exceptionLogger)
            : base(transactionManager, exceptionLogger)
        {
        }

        protected override object CloseUnitOfWork(
            TransactionAttributeSettings transactionAttributeSettings, object transactionState, Exception err)
        {
            transactionState = base.CloseUnitOfWork(transactionAttributeSettings, transactionState, err);
            if (this.transactionManager.TransactionDepth == 0)
            {
                var sessionStorage = NHibernateSession.Storage as IUnitOfWorkSessionStorage;
                if (sessionStorage != null)
                {
                    sessionStorage.EndUnitOfWork(
                        ((UnitOfWorkAttributeSettings)transactionAttributeSettings).CloseSessions);
                }
            }

            return transactionState;
        }

        protected override Type GetAttributeType()
        {
            return typeof(UnitOfWorkAttribute);
        }
    }
}