using System;
using System.Threading;
using Magnum.Extensions;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Services.Subscriptions.Server;
using MassTransit.Transports.Stomp.Configuration;
using MassTransitTest;

namespace Publisher
{
    class Program
    {
        private static IServiceBus _bus;

        static void Main(string[] args)
        {
            StartSubscriptionService();

            using (_bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseStomp();
                sbc.ReceiveFrom(string.Format("{0}/queue/matt2", Constants.HostUri));
                sbc.UseSubscriptionService("{0}/queue/matt_subscriptions".FormatWith(Constants.HostUri));
                sbc.UseControlBus();
                sbc.Subscribe(s => s.Handler<Response>(HandleMessage));
            }))
            {
                var id = 0;
                while (true)
                {
                    _bus.Publish(new Request { CorrelationId = id, Text = "Hiiiii" });                    
                    Console.Out.WriteLine("Published request " + id);
                    id++;
                    Thread.Sleep(5000);
                }               
            }
        }

        private static void StartSubscriptionService()
        {
            Console.Out.WriteLine("starting the publisher");

            var subscriptionSagaRepository = new InMemorySagaRepository<SubscriptionSaga>();
            var clientSagaRepository = new InMemorySagaRepository<SubscriptionClientSaga>();

            var serviceBus =
                ServiceBusFactory.New(sbc =>
                {
                    sbc.UseStomp();
                    
                    sbc.ReceiveFrom("{0}/queue/matt_subscriptions".FormatWith(Constants.HostUri));
                    sbc.SetConcurrentConsumerLimit(1);
                });

            var subscriptionService = new SubscriptionService(serviceBus, subscriptionSagaRepository,
                                                              clientSagaRepository);
            subscriptionService.Start();
        }

        private static void HandleMessage(Response msg)
        {
            Console.Out.WriteLine(string.Format("got {1} response for msg {0}", msg.CorrelationId,
                                                msg.Successful ? "Successful" : "Failed"));
        }
    }
}
