// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using Filters;
    using PipeConfigurators;


    public class ConsumePipe :
        IConsumePipe
    {
        readonly MessageTypeConsumeFilter _filter;
        readonly IPipe<ConsumeContext> _pipe;

        public ConsumePipe()
            : this(new PipeConfigurator<ConsumeContext>())
        {
        }

        public ConsumePipe(IBuildPipeConfigurator<ConsumeContext> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _filter = new MessageTypeConsumeFilter();

            configurator.Filter(_filter);

            _pipe = configurator.Build();
        }

        Task IPipe<ConsumeContext>.Send(ConsumeContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
        {
            return _filter.ConnectConsumeMessageObserver(observer);
        }

        bool IPipe<ConsumeContext>.Visit(IPipeVisitor visitor)
        {
            return _pipe.Visit(visitor);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _filter.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _filter.ConnectRequestPipe(requestId, pipe);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _filter.ConnectConsumeObserver(observer);
        }
    }
}