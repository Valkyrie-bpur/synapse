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
using Synapse.Integration.Events.Schedules;

namespace Synapse.Domain.Events.Schedules
{
    /// <summary>
    /// Represents the <see cref="IDomainEvent"/> fired whenever a <see cref="V1Schedule"/> has occured
    /// </summary>
    [DataTransferObjectType(typeof(V1ScheduleOccuredIntegrationEvent))]
    public class V1ScheduleOccuredDomainEvent
        : DomainEvent<Models.V1Schedule, string>
    {

        /// <summary>
        /// Initializes a new <see cref="V1ScheduleOccuredDomainEvent"/>
        /// </summary>
        protected V1ScheduleOccuredDomainEvent() { }

        /// <summary>
        /// Initializes a new <see cref="V1ScheduleOccuredDomainEvent"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="V1Schedule"/> that has occured</param>
        /// <param name="nextOccurenceAt">The date and time at which the <see cref="V1Schedule"/> will next occur</param>
        public V1ScheduleOccuredDomainEvent(string id, DateTimeOffset? nextOccurenceAt) 
            : base(id) 
        { 
            this.NextOccurenceAt = nextOccurenceAt;
        }

        /// <summary>
        /// Gets the <see cref="V1Schedule"/>'s next occurence
        /// </summary>
        public virtual DateTimeOffset? NextOccurenceAt { get; protected set; }

    }

}
