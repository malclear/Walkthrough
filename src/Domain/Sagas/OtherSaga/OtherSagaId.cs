using Akkatecture.Sagas;

namespace Domain.Sagas.OtherSaga
{
    public class OtherSagaId : SagaId<OtherSagaId>
    {
        public OtherSagaId(string value) : base(value)
        {
        }
    }
}