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
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using TestFramework;

    public class message_publishing_between_two_busses
        : given_a_stomp_bus_with_a_subscriptionservice
    {
        private Future<A> _received;

        [When]
        public void A_message_is_published_one_the_local_bus()
        {
            _received = new Future<A>();
            RemoteBus.SubscribeHandler<A>(message => _received.Complete(message));

            Thread.Sleep(3.Seconds());

            LocalBus.Publish(new A
                                 {
                                     StringA = "ValueA",
                                 });
        }

        [Then]
        public void Should_be_received_by_the_remote_queue()
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