using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Sagas;

namespace Domain.Sagas.OtherSaga
{
    public class OtherSagaLocator: ISagaLocator<OtherSagaId>
    {
        public OtherSagaId LocateSaga(IDomainEvent domainEvent)
        {
            throw new System.NotImplementedException();
            // switch (domainEvent.GetAggregateEvent())
            // {
            //     case MoneySentEvent evt:
            //         return new MoneyTransferSagaId($"{LocatorIdPrefix}-{evt.Transaction.Id}");
            //
            //     case MoneyReceivedEvent evt:
            //         return new MoneyTransferSagaId($"{LocatorIdPrefix}-{evt.Transaction.Id}");
            //
            //     default:
            //         throw new ArgumentException(nameof(domainEvent));
            // } 
        }
        
        public OtherSagaId LocateSaga(Command<OtherSaga, OtherSagaId> command)
        {
            return new OtherSagaId(command.AggregateId.ToString());
        }
    }
}