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

using Synapse.Integration.Events.WorkflowActivities;

namespace Synapse.Worker.Services.Processors
{

    /// <summary>
    /// Represents the <see cref="IWorkflowActivityProcessor"/> used to process <see cref="InjectStateDefinition"/>s
    /// </summary>
    public class InjectStateProcessor
        : StateProcessor<InjectStateDefinition>
    {

        /// <inheritdoc/>
        public InjectStateProcessor(ILoggerFactory loggerFactory, IWorkflowRuntimeContext context, IWorkflowActivityProcessorFactory activityProcessorFactory, 
            IOptions<ApplicationOptions> options, V1WorkflowActivity activity, InjectStateDefinition state)
            : base(loggerFactory, context, activityProcessorFactory, options, activity, state)
        {

        }

        /// <inheritdoc/>
        protected override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var output = this.Activity.Input!.ToObject()!;
            var toInject = this.State.Data!.ToObject()!;
            toInject = await this.Context.EvaluateObjectAsync(toInject, output, cancellationToken);
            output = output.Merge(toInject);
            await this.OnNextAsync(new V1WorkflowActivityCompletedIntegrationEvent(this.Activity.Id, output), cancellationToken);
            await this.OnCompletedAsync(cancellationToken);
        }

    }

}
