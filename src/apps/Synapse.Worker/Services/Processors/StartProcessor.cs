﻿/*
* Copyright © 2022-Present The Synapse Authors
* <p>
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* <p>
* http://www.apache.org/licenses/LICENSE-2.0
* <p>
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

namespace Synapse.Worker.Services.Processors
{

    /// <summary>
    /// Represents an <see cref="IWorkflowActivityProcessor"/> used to process <see cref="StartDefinition"/>s
    /// </summary>
    public class StartProcessor
        : WorkflowActivityProcessor
    {

        /// <inheritdoc/>
        public StartProcessor(ILoggerFactory loggerFactory, IWorkflowRuntimeContext context, IWorkflowActivityProcessorFactory activityProcessorFactory,
            IOptions<ApplicationOptions> options, V1WorkflowActivity activity, StartDefinition start)
            : base(loggerFactory, context, activityProcessorFactory, options, activity)
        {
            this.Start = start;
        }

        /// <summary>
        /// Gets the <see cref="StartDefinition"/> to process
        /// </summary>
        public StartDefinition? Start { get; }

        /// <inheritdoc/>
        protected override Task InitializeAsync(CancellationToken cancellationToken)
        {
            //TODO
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await this.OnNextAsync(new V1WorkflowActivityCompletedIntegrationEvent(this.Activity.Id, this.Activity.Input), cancellationToken);
            await this.OnCompletedAsync(cancellationToken);
        }

    }

}
