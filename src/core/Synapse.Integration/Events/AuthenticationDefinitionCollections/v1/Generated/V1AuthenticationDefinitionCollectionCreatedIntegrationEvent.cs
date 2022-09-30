﻿
/*
 * Copyright © 2022-Present The Synapse Authors
 * <p>
 * Licensed under the Apache License, Version 2.0(the "License");
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
 */

/* -----------------------------------------------------------------------
 * This file has been automatically generated by a tool
 * -----------------------------------------------------------------------
 */

namespace Synapse.Integration.Events.AuthenticationDefinitionCollections
{

	/// <summary>
	/// Represents the IDomainEvent fired whenever a new V1EventDefinitionCollection has been created
	/// </summary>
	[DataContract]
	public partial class V1AuthenticationDefinitionCollectionCreatedIntegrationEvent
		: V1IntegrationEvent
	{

		/// <summary>
		/// Gets the id of the aggregate that has produced the event
		/// </summary>
		[DataMember(Name = "AggregateId", Order = 1)]
		[Description("Gets the id of the aggregate that has produced the event")]
		public virtual string AggregateId { get; set; }

		/// <summary>
		/// Gets the date and time at which the event has been produced
		/// </summary>
		[DataMember(Name = "CreatedAt", Order = 2)]
		[Description("Gets the date and time at which the event has been produced")]
		public virtual DateTime CreatedAt { get; set; }

		/// <summary>
		/// The V1EventDefinitionCollection's name
		/// </summary>
		[DataMember(Name = "Name", Order = 3)]
		[Description("The V1EventDefinitionCollection's name")]
		public virtual string Name { get; set; }

		/// <summary>
		/// The V1EventDefinitionCollection's version
		/// </summary>
		[DataMember(Name = "Version", Order = 4)]
		[Description("The V1EventDefinitionCollection's version")]
		public virtual string Version { get; set; }

		/// <summary>
		/// The V1EventDefinitionCollection's description
		/// </summary>
		[DataMember(Name = "Description", Order = 5)]
		[Description("The V1EventDefinitionCollection's description")]
		public virtual string Description { get; set; }

		/// <summary>
		/// An IReadOnlyCollection`1 containing the EventDefinitions the V1EventDefinitionCollection is made out of
		/// </summary>
		[DataMember(Name = "Events", Order = 6)]
		[Description("An IReadOnlyCollection`1 containing the EventDefinitions the V1EventDefinitionCollection is made out of")]
		public virtual ICollection<EventDefinition> Events { get; set; }

    }

}
