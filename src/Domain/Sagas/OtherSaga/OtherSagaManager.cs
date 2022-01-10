using System;
using System.Linq.Expressions;
using Akkatecture.Commands;
using Akkatecture.Sagas.AggregateSaga;

namespace Domain.Sagas.OtherSaga
{
    public class OtherSagaManager : AggregateSagaManager<OtherSaga, OtherSagaId, OtherSagaLocator>
    {
        public OtherSagaManager(Expression<Func<OtherSaga>> sagaFactory) : base(sagaFactory)
        {
            Receive<Command<OtherSaga, OtherSagaId>>(Handle);
        }
        
        protected virtual bool Handle(Command<OtherSaga, OtherSagaId> command)
        {
            var sagaId = SagaLocator.LocateSaga(command);
            var saga = FindOrSpawn(sagaId);
            saga.Tell(command,Sender);
            return true;
        }
    }
}