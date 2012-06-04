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

    /// <summary>
    ///   Holds the configuration settings for <see cref="StompClientFactory" />
    /// </summary>
    public class ConnectionFactoryConfiguratorImpl
        : ConnectionFactoryConfigurator
    {
        private readonly ConnectionFactorySettings _settings;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConnectionFactoryConfiguratorImpl" /> class.
        /// </summary>
        /// <param name="defaultSettings"> The default configuration. </param>
        public ConnectionFactoryConfiguratorImpl(ConnectionFactoryDefaultSettings defaultSettings)
        {
            _settings = new ConnectionFactorySettings(defaultSettings);
        }

        /// <summary>
        ///   Builds the new <see cref="StompClient" /> to connect to the given address
        /// </summary>
        /// <param name="buildMethod"> </param>
        public void UseBuildMethod(Func<Uri, StompClient> buildMethod)
        {
            _settings.BuidMethod = buildMethod;
        }

        public StompConnectionFactory CreateStompClientFactory()
        {
            return new StompConnectionFactory(_settings.BuidMethod);
        }
    }
}