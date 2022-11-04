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

using Synapse.Application.Commands.Schedules;
using Synapse.Application.Commands.Workflows;
using Synapse.Infrastructure.Plugins;

namespace Synapse.Application.Services
{

    /// <summary>
    /// Represents a <see cref="BackgroundService"/> used to schedule all CRON-based <see cref="V1Workflow"/>s at startup
    /// </summary>
    public class WorkflowScheduler
        : BackgroundService
    {

        /// <summary>
        /// Initializes a new <see cref="WorkflowScheduler"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="pluginManager">The service used to manage <see cref="IPlugin"/>s</param>
        public WorkflowScheduler(IServiceProvider serviceProvider, ILogger<WorkflowScheduler> logger, IPluginManager pluginManager)
        {
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.PluginManager = pluginManager;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to manage <see cref="IPlugin"/>s
        /// </summary>
        protected IPluginManager PluginManager { get; }

        /// <summary>
        /// Gets <see cref="WorkflowScheduler"/>'s <see cref="System.Threading.CancellationTokenSource"/>
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            await this.PluginManager.WaitForStartupAsync(stoppingToken);
            using var scope = this.ServiceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var schedules = scope.ServiceProvider.GetRequiredService<IRepository<Integration.Models.V1Schedule>>();
            foreach(var schedule in schedules.AsQueryable()
                .Where(s => s.Status == V1ScheduleStatus.Active)
                .ToList())
            {

            }
        }

        public async Task ScheduleJobAsync(V1Schedule schedule)
        {

        }

    }

    /// <summary>
    /// Represents a scheduled job
    /// </summary>
    public class ScheduledJob
    {

        /// <summary>
        /// Gets the <see cref="ScheduledJob"/>'s id. Equals to the id of the <see cref="V1Schedule"/> it is bound to
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the <see cref="System.Threading.Timer"/> used to clock the next job occurence
        /// </summary>
        protected Timer Timer { get; private set; } = null!;

        /// <inheritdoc/>
        public virtual async Task ScheduleAsync(CancellationToken cancellationToken = default)
        {
            this.Timer = new(this.OnNextOccurenceAsync, null, delay, Timeout.InfiniteTimeSpan);
        }

        protected virtual async Task OnNextOccurenceAsync(CancellationToken cancellationToken = default)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var schedule = await mediator.ExecuteAndUnwrapAsync(new V1TriggerScheduleCommand(this.Id), cancellationToken);
        }

    }

}