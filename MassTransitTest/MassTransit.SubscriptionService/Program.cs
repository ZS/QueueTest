using System;
using Magnum.Extensions;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Services.Subscriptions.Server;
using MassTransit.Transports.Stomp.Configuration;
using MassTransitTest;


namespace Subscriber
{
    class Program
    {
        private static IServiceBus _bus;

        static void Main(string[] args)
        {
            StartSubscriber();
        }

        private static void StartSubscriber()
        {
            StartSubscriptionService();

            Console.Out.WriteLine("starting the subscriber");


            using (_bus =
                ServiceBusFactory.New(sbc =>
                {
                    sbc.UseStomp();
                    sbc.ReceiveFrom("{0}/queue/matt1".FormatWith(Constants.HostUri));
                    sbc.UseSubscriptionService("{0}/queue/matt_subscriptions".FormatWith(Constants.HostUri));
                    sbc.UseControlBus();       
                    sbc.Subscribe(s => s.Handler<Request>(HandleMessage));
                }))
            {                
                Console.WriteLine("ready... type 'exit' to stop");
                while (Console.ReadLine() != "exit")
                {
                    Console.WriteLine("type 'exit' to stop");
                }
            }            
        }

        private static void StartSubscriptionService()
        {
            Console.Out.WriteLine("starting the subscription service");

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



        private static void HandleMessage(Request msg)
        {
            _bus.MessageContext<Request>().Respond(new Response
            {
                CorrelationId = msg.CorrelationId,
                Successful = msg.CorrelationId % 6 == 0
            });
            Console.Out.WriteLine(String.Format("got request {0}! text: {1}", msg.CorrelationId,
                                                msg.Text));
        }
    }
}
