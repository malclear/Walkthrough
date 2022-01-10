using Akkatecture.Aggregates;
using Akkatecture.Sagas;

namespace Domain.Sagas.OtherSaga
{
    public class OtherSagaState : SagaState<OtherSaga, OtherSagaId, IMessageApplier<OtherSaga, OtherSagaId>>
        // IApply(some event)
    {
    }
}