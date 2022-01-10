using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Domain.Model.Account;
using Domain.Model.Account.ValueObjects;
using Domain.Repositories.Revenue.Commands;
using Domain.Sagas.OtherSaga.Commands;

namespace Domain.Sagas.OtherSaga
{
    public class OtherSaga : AggregateSaga<OtherSaga, OtherSagaId, OtherSagaState>
    {
        public IActorRef RevenueRepository { get; }
        public OtherSaga(IActorRef revenueRepository)
        {
            RevenueRepository = revenueRepository;
            Command<StartOtherSaga>(Execute);
        }

        public bool Execute(StartOtherSaga startOtherSaga)
        {
            var addRevenueCommand = new AddRevenueCommand(new Money(0.35m));
            RevenueRepository.Tell(addRevenueCommand);
            Sender.Tell(new SuccessExecutionResult());
            return true;
        }
    }
}