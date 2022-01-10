using Akkatecture.Commands;

namespace Domain.Sagas.OtherSaga.Commands
{
    public class StartOtherSaga : Command< OtherSaga,OtherSagaId> 
    {
        public StartOtherSaga(OtherSagaId sagaId) : base(sagaId)
        {
        }

        public StartOtherSaga(OtherSagaId sagaId, CommandId sourceId) : base(sagaId, sourceId)
        {
        }
    }
}