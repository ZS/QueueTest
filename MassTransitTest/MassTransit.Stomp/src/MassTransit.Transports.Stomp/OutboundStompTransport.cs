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

namespace MassTransit.Transports.Stomp
{
    using System.IO;
    using System.Text;

    public class OutboundStompTransport
        : IOutboundTransport
    {
        private readonly ConnectionHandler<StompConnection> _connectionHandler;
        private readonly IEndpointAddress _address;
 
        public OutboundStompTransport(IEndpointAddress address, ConnectionHandler<StompConnection> connectionHandler)
        {
            _connectionHandler = connectionHandler;
            _address = address;
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Send(ISendContext context)
        {
            _connectionHandler
                .Use(connection =>
                         {
                             using (var body = new MemoryStream())
                             {
                                 context.SerializeTo(body);

                                 var msg = Encoding.UTF8.GetString(body.ToArray());
                                 connection.Send(Address.Uri.PathAndQuery, msg);
                             }
                         });
        }

        public void Dispose()
        {            
        }
    }
}