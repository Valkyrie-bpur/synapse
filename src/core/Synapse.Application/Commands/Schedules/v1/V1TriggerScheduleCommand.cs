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

using ServerlessWorkflow.Sdk;
using Synapse.Application.Commands.WorkflowInstances;

namespace Synapse.Application.Commands.Schedules
{

    /// <summary>
    /// Represents the <see cref="ICommand"/> used to trigger a <see cref="V1Schedule"/> occurence
    /// </summary>
    [DataTransferObjectType(typeof(Integration.Commands.Schedules.V1TriggerScheduleCommand))]
    public class V1TriggerScheduleCommand
        : Command<Integration.Models.V1Schedule>
    {

        /// <summary>
        /// Initializes a new <see cref="V1TriggerScheduleCommand"/>
        /// </summary>
        protected V1TriggerScheduleCommand() { }

        /// <summary>
        /// Initializes a new <see cref="V1TriggerScheduleCommand"/>
        /// </summary>
        /// <param name="scheduleId">The id of the <see cref="V1Schedule"/> to trigger</param>
        public V1TriggerScheduleCommand(string scheduleId)
        {
            this.ScheduleId = scheduleId;
        }

        /// <summary>
        /// Gets the id of the <see cref="V1Schedule"/> to trigger
        /// </summary>
        public virtual string ScheduleId { get; protected set; } = null!;

    }

    /// <summary>
    /// Represents the service used to handle <see cref="V1TriggerScheduleCommand"/>s
    /// </summary>
    public class V1TriggerScheduleCommandHandler
        : CommandHandlerBase,
        ICommandHandler<V1TriggerScheduleCommand, Integration.Models.V1Schedule>
    {

        /// <summary>
        /// Initializes a new <see cref="V1TriggerScheduleCommandHandler"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="mediator">The service used to mediate calls</param>
        /// <param name="mapper">The service used to map objects</param>
        /// <param name="schedules">The <see cref="IRepository"/> used to manage <see cref="V1Schedule"/>s</param>
        /// <param name="backgroundJobManager">The service used to manage background jobs</param>
        public V1TriggerScheduleCommandHandler(ILoggerFactory loggerFactory, IMediator mediator, IMapper mapper, IRepository<V1Schedule> schedules, IBackgroundJobManager backgroundJobManager)
            : base(loggerFactory, mediator, mapper)
        {
            this.Schedules = schedules;
            this.BackgroundJobManager = backgroundJobManager;
        }

        /// <summary>
        /// Gets the <see cref="IRepository"/> used to manage <see cref="V1Schedule"/>s
        /// </summary>
        protected IRepository<V1Schedule> Schedules { get; }

        /// <summary>
        /// Gets the service used to manage background jobs
        /// </summary>
        protected IBackgroundJobManager BackgroundJobManager { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult<Integration.Models.V1Schedule>> HandleAsync(V1TriggerScheduleCommand command, CancellationToken cancellationToken = default)
        {
            var schedule = await this.Schedules.FindAsync(command.ScheduleId, cancellationToken);
            if (schedule == null) throw DomainException.NullReference(typeof(V1Schedule), command.ScheduleId);

            // Check if we need to instantiate or suspend a workflow
            if (schedule.ActionType.Equals("instantiate")) {
                var workflowInstance = await this.Mediator.ExecuteAndUnwrapAsync(new V1CreateWorkflowInstanceCommand(schedule.WorkflowId, V1WorkflowInstanceActivationType.Schedule, null, null, true, null));
                schedule.Occur(workflowInstance.Id);
                schedule = await this.Schedules.UpdateAsync(schedule, cancellationToken);
                await this.Schedules.SaveChangesAsync(cancellationToken);
                if (schedule.Definition.Type == ScheduleDefinitionType.Cron && schedule.NextOccurenceAt.HasValue) await this.BackgroundJobManager.ScheduleJobAsync(schedule, cancellationToken);
            }else if(schedule.ActionType.Equals("suspend")) {
                await this.Mediator.ExecuteAndUnwrapAsync(new V1SuspendWorkflowInstanceCommand(schedule.WorkflowId));
                // Also delete the schedule since we anyways suspended the workflow
                await this.Mediator.ExecuteAndUnwrapAsync(new V1MakeScheduleObsoleteCommand(schedule.Id));
            }

            return this.Ok(this.Mapper.Map<Integration.Models.V1Schedule>(schedule));
        }

    }

}