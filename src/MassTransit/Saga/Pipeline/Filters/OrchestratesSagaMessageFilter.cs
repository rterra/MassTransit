﻿// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Util;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TSaga">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class OrchestratesSagaMessageFilter<TSaga, TMessage> :
        ISagaMessageFilter<TSaga, TMessage>
        where TSaga : class, ISaga, Orchestrates<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
        public Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            var messageConsumer = context.Saga as Orchestrates<TMessage>;
            if (messageConsumer == null)
            {
                string message = string.Format("Saga type {0} does not orchestrate message type {1}",
                    TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<TMessage>.ShortName);

                throw new ConsumerMessageException(message);
            }

            return messageConsumer.Consume(context);
        }

        public bool Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}