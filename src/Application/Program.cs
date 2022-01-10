﻿// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates.ExecutionResults;
using Domain.Model.Account;
using Domain.Model.Account.Commands;
using Domain.Model.Account.Entities;
using Domain.Model.Account.ValueObjects;
using Domain.Repositories.Revenue;
using Domain.Repositories.Revenue.Queries;
using Domain.Repositories.Revenue.ReadModels;
using Domain.Sagas.MoneyTransfer;
using Domain.Sagas.OtherSaga;
using Domain.Sagas.OtherSaga.Commands;
using Domain.Subscribers.Revenue;

namespace Application
{
    public class Program
    {
        public static IActorRef AccountManager { get; set; }
        public static IActorRef RevenueRepository { get; set; }

        public static IActorRef OtherSagaManager { get; set; }
        
        public static void CreateActorSystem()
        {
            //Create actor system
            var system = ActorSystem.Create("bank-system");

            //Create aggregate manager for accounts
            var aggregateManager = system.ActorOf(Props.Create(() => new AccountManager()), "account-manager");

            //Create revenue repository
            var revenueRepository = system.ActorOf(Props.Create(() => new RevenueRepository()), "revenue-repository");
            
           //Create subscriber for revenue repository
            system.ActorOf(Props.Create(() => new RevenueSubscriber(revenueRepository)), "revenue-subscriber");

            //Create saga manager for money transfer
            system.ActorOf(Props.Create(() =>
                new MoneyTransferSagaManager(() => new MoneyTransferSaga(aggregateManager))), "moneytransfer-saga");

             //Create saga manager for OtherSaga
             var otherSagaManager = system.ActorOf(Props.Create(() => new OtherSagaManager(() => new OtherSaga(revenueRepository))), "othersaga-manager");
 
             AccountManager = aggregateManager;
            RevenueRepository = revenueRepository;
            OtherSagaManager = otherSagaManager;
        }

        public static async Task Main(string[] args)
        {

            //initialize actor system
            CreateActorSystem();

            //create send receiver identifiers
            var senderId = AccountId.New;
            var receiverId = AccountId.New;

            //create mock opening balances
            var senderOpeningBalance = new Money(509.23m);
            var receiverOpeningBalance = new Money(30.45m);

            //create commands for opening the sender and receiver accounts
            var openSenderAccountCommand = new OpenNewAccountCommand(senderId, senderOpeningBalance);
            var openReceiverAccountCommand = new OpenNewAccountCommand(receiverId, receiverOpeningBalance);

            //send the command to be handled by the account aggregate
            AccountManager.Tell(openReceiverAccountCommand);
            AccountManager.Tell(openSenderAccountCommand);

            //create command to initiate money transfer
            var amountToSend = new Money(125.23m);
            var transaction = new Transaction(senderId, receiverId, amountToSend);
            var transferMoneyCommand = new TransferMoneyCommand(senderId, transaction);

            //send the command to initiate the money transfer
            AccountManager.Tell(transferMoneyCommand);

            //fake 'wait' to let the saga process the chain of events
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            Console.WriteLine("Walkthrough operations complete.\n\n");
            Console.WriteLine("Press Enter to get the revenue:");

            Console.ReadLine();

            //get the revenue stored in the repository
            var revenue = RevenueRepository.Ask<RevenueReadModel>(new GetRevenueQuery(), TimeSpan.FromMilliseconds(500)).Result;

            //print the results
            Console.WriteLine($"The Revenue is: {revenue.Revenue.Value}.");
            Console.WriteLine($"From: {revenue.Transactions} transaction(s).");
            var startMessage = new OtherSagaId("asdf");
            var responseFromOtherSaga = await OtherSagaManager.Ask<IExecutionResult>(new StartOtherSaga(startMessage),TimeSpan.FromMilliseconds(500));

            await Task.Delay(TimeSpan.FromSeconds(1));
            revenue = RevenueRepository.Ask<RevenueReadModel>(new GetRevenueQuery(), TimeSpan.FromMilliseconds(500)).Result;
            Console.WriteLine($"The revenue has now changed to {revenue.Revenue.Value} from {revenue.Transactions} transactions.");
            
            Console.ReadLine();
        }
    }
}
