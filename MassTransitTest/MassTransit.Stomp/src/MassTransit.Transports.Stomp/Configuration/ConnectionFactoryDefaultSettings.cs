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

namespace MassTransit.Transports.Stomp.Configuration
{
    using System;
    using Ultralight.Client;
    using Ultralight.Client.Transport;

    /// <summary>
    /// Default <see cref="StompClientFactory"/> configuration settings
    /// </summary>
    public class ConnectionFactoryDefaultSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactoryDefaultSettings"/> class.
        /// </summary>
        public ConnectionFactoryDefaultSettings()
        {
            BuidMethod = adress => new StompClient(new WebTransportTransport(adress.ToString()));
        }

        /// <summary>
        /// Gets the buid method.
        /// </summary>
        public Func<Uri, StompClient> BuidMethod { get; set; }
    }
}