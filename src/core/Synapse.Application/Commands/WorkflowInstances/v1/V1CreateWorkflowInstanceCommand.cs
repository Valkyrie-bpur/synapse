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

using Neuroglia.Data.Expressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Synapse.Application.Queries.Workflows;
using ServerlessWorkflow.Sdk.Models;
using Synapse.Application.Commands.Schedules;

namespace Synapse.Application.Commands.WorkflowInstances
{

    /// <summary>
    /// Represents the <see cref="ICommand"/> used to create a new <see cref="V1Workflow"/>s
    /// </summary>
    [DataTransferObjectType(typeof(Integration.Commands.WorkflowInstances.V1CreateWorkflowInstanceCommand))]
    public class V1CreateWorkflowInstanceCommand
        : Command<Integration.Models.V1WorkflowInstance>
    {

        /// <summary>
        /// Initializes a new <see cref="V1CreateWorkflowInstanceCommand"/>
        /// </summary>
        protected V1CreateWorkflowInstanceCommand()
        {
            this.WorkflowId = null!;
        }

        /// <summary>
        /// Initializes a new <see cref="V1CreateWorkflowInstanceCommand"/>
        /// </summary>
        /// <param name="workflowId">The id of the <see cref="V1Workflow"/> to instanciate</param>
        /// <param name="activationType">The <see cref="V1Workflow"/>'s activation type</param>
        /// <param name="inputData">The input data of the <see cref="V1WorkflowInstance"/> to create</param>
        /// <param name="correlationContext">The <see cref="V1CorrelationContext"/> of the <see cref="V1WorkflowInstance"/> to create</param>
        /// <param name="autoStart">A boolean indicating whether or not to start the <see cref="V1WorkflowInstance"/> once it has been created</param>
        /// <param name="parentId">The id of the parent <see cref="V1WorkflowInstance"/> of the <see cref="V1WorkflowInstance"/> to create</param>
        public V1CreateWorkflowInstanceCommand(string workflowId, V1WorkflowInstanceActivationType activationType, object? inputData, V1CorrelationContext? correlationContext, bool autoStart, string? parentId)
        {
            this.WorkflowId = workflowId;
            this.ActivationType = activationType;
            this.InputData = inputData;
            this.CorrelationContext = correlationContext;
            this.AutoStart = autoStart;
            this.ParentId = parentId;
        }

        /// <summary>
        /// Gets the id of the <see cref="V1Workflow"/> to instanciate
        /// </summary>
        public virtual string WorkflowId { get; protected set; }

        /// <summary>
        /// Gets the <see cref="V1Workflow"/>'s activation type
        /// </summary>
        public virtual V1WorkflowInstanceActivationType ActivationType { get; protected set; }

        /// <summary>
        /// Gets the input data of the <see cref="V1WorkflowInstance"/> to create
        /// </summary>
        public virtual object? InputData { get; protected set; }

        /// <summary>
        /// Gets <see cref="V1CorrelationContext"/> of the <see cref="V1WorkflowInstance"/> to create
        /// </summary>
        public virtual V1CorrelationContext? CorrelationContext { get; protected set; }

        /// <summary>
        /// Gets a boolean indicating whether or not to automatically start the <see cref="V1WorkflowInstance"/> once it has been created
        /// </summary>
        public virtual bool AutoStart { get; protected set; }

        /// <summary>
        /// Gets the id of the parent <see cref="V1WorkflowInstance"/> of the <see cref="V1WorkflowInstance"/> to create
        /// </summary>
        public virtual string? ParentId { get; protected set; }

    }

    /// <summary>
    /// Represents the service used to handle <see cref="V1CreateWorkflowInstanceCommand"/>s
    /// </summary>
    public class V1CreateWorkflowInstanceCommandHandler
        : CommandHandlerBase,
        ICommandHandler<V1CreateWorkflowInstanceCommand, Integration.Models.V1WorkflowInstance>
    {

        /// <inheritdoc/>
        public V1CreateWorkflowInstanceCommandHandler(ILoggerFactory loggerFactory, IMediator mediator, IMapper mapper, IHttpClientFactory httpClientFactory,
            IRepository<V1Workflow> workflows, IRepository<V1WorkflowInstance> workflowInstances, IExpressionEvaluatorProvider expressionEvaluatorProvider)
            : base(loggerFactory, mediator, mapper)
        {
            this.HttpClientFactory = httpClientFactory;
            this.Workflows = workflows;
            this.WorkflowInstances = workflowInstances;
            this.ExpressionEvaluatorProvider = expressionEvaluatorProvider;
        }

        /// <summary>
        /// Gets the service used to create <see cref="HttpClient"/>s
        /// </summary>
        protected IHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// Gets the <see cref="IRepository"/> used to manage <see cref="V1Workflow"/>s
        /// </summary>
        protected IRepository<V1Workflow> Workflows { get; }

        /// <summary>
        /// Gets the <see cref="IRepository"/> used to manage <see cref="V1WorkflowInstance"/>s
        /// </summary>
        protected IRepository<V1WorkflowInstance> WorkflowInstances { get;}

        /// <summary>
        /// Gets the service used to provide <see cref="IExpressionEvaluator"/>s
        /// </summary>
        protected IExpressionEvaluatorProvider ExpressionEvaluatorProvider { get; }

        /// <inheritdoc/>
        public virtual async Task<IOperationResult<Integration.Models.V1WorkflowInstance>> HandleAsync(V1CreateWorkflowInstanceCommand command, CancellationToken cancellationToken = default)
        {
            var workflowIdComponents = command.WorkflowId.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var id = workflowIdComponents[0];
            var version = string.Empty;
            if (workflowIdComponents.Length > 1)
                version = workflowIdComponents[1];
            var workflowId = (await this.Mediator.ExecuteAndUnwrapAsync(new V1GetWorkflowByIdQuery(id, version), cancellationToken)).Id;
            var workflow = await this.Workflows.FindAsync(workflowId, cancellationToken);
            if(workflow == null)
                throw DomainException.NullReference(typeof(V1Workflow), workflowId);
            var parent = null as V1WorkflowInstance;
            if (!string.IsNullOrWhiteSpace(command.ParentId))
            {
                parent = await this.WorkflowInstances.FindAsync(command.ParentId, cancellationToken);
                if(parent == null)
                    throw DomainException.NullReference(typeof(V1WorkflowInstance), command.ParentId);
            }
            string? key = null;
            var dataInputSchema = workflow.Definition.DataInputSchema?.Schema;
            if (dataInputSchema == null
                && workflow.Definition.DataInputSchemaUri != null)
            {
                using var httpClient = this.HttpClientFactory.CreateClient();
                var json = await httpClient.GetStringAsync(workflow.Definition.DataInputSchemaUri, cancellationToken);
                dataInputSchema = JSchema.Parse(json);
            }
            if(dataInputSchema != null)
            {
                var input = command.InputData;
                JObject? jobj;
                if (input == null)
                    jobj = new JObject();
                else
                    jobj = JObject.FromObject(input);
                if (!jobj.IsValid(dataInputSchema, out IList<string> errors))
                    throw new DomainArgumentException($"Invalid workflow input data:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}", nameof(command.InputData));
            }
            if (!string.IsNullOrWhiteSpace(workflow.Definition.Key)
                && command.InputData != null)
            {
                try
                {
                    key = this.ExpressionEvaluatorProvider.GetEvaluator(workflow.Definition.ExpressionLanguage)!.Evaluate(workflow.Definition.Key, command.InputData)?.ToString();
                }
                catch { }
            }
            if (string.IsNullOrWhiteSpace(key))
                key = Guid.NewGuid().ToBase64();
            while (await this.WorkflowInstances.ContainsAsync(V1WorkflowInstance.BuildUniqueIdentifier(key, workflow), cancellationToken))
            {
                key = Guid.NewGuid().ToBase64();
            }
            var workflowInstance = await this.WorkflowInstances.AddAsync(new(key.ToLowerInvariant(), workflow, command.ActivationType, command.InputData, command.CorrelationContext, parent), cancellationToken);
            await this.WorkflowInstances.SaveChangesAsync(cancellationToken);
            workflow.Instanciate();
            await this.Workflows.UpdateAsync(workflow, cancellationToken);
            await this.Workflows.SaveChangesAsync(cancellationToken);
            if (command.AutoStart)
                await this.Mediator.ExecuteAndUnwrapAsync(new V1StartWorkflowInstanceCommand(workflowInstance.Id), cancellationToken);

            //// If a workflowExecTimeout is specified, then create a new schedule that will run once to suspend the workflow
            if (workflow.Definition.Timeouts != null && workflow.Definition.Timeouts.WorkflowExecutionTimeout != null) {
                // We have a timeout defined. We therefore need to create a schedule with the timeout and schedule it so that the workflow gets suspended
                ScheduleDefinition definition = new() {
                    Interval = workflow.Definition.Timeouts.WorkflowExecutionTimeout.Duration
                };                
                //System.Xml.XmlConvert.ToTimeSpan("PT8H")
                await this.Mediator.ExecuteAndUnwrapAsync(new V1CreateScheduleCommand(V1ScheduleActivationType.Explicit, definition, workflowInstance.Id, "suspend"));
            }

            return this.Ok(this.Mapper.Map<Integration.Models.V1WorkflowInstance>(workflowInstance));
        }

    }

}
