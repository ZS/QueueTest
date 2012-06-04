// Copyright 2011 Ernst Naezer, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace MassTransit.Transports.Stomp.Tests
{
    using System;
    using BusConfigurators;
    using Configuration;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using TestFramework;

    [Scenario]
    public class message_publishing 
        : given_a_stomp_server
    {
        private Future<A> _received;

        protected Uri LocalUri { get; set; }
        protected IServiceBus LocalBus { get; set; }

        public message_publishing()
        {
            LocalUri = new Uri("stomp://localhost:8181/test_queue");
            LocalBus = SetupServiceBus(LocalUri);
        }

        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            _received = new Future<A>();
            
            configurator.Subscribe(s => s.Handler<A>(message => _received.Complete(message)));
            configurator.UseControlBus();
            configurator.UseStomp();
        }

        [When]
        public void A_message_is_published()
        {
            LocalBus.Publish(new A
                                 {
                                     StringA = "ValueA",
                                 });
        }

        [Then]
        public void Should_be_received_by_the_queue()
        {
            _received.WaitUntilCompleted(3.Seconds()).ShouldBeTrue();
            _received.Value.StringA.ShouldEqual("ValueA");
        }

        private class A 
        {
            public string StringA { get; set; }
        }
    }
}