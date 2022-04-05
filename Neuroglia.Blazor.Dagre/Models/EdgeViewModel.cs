﻿using System.Collections.ObjectModel;

namespace Neuroglia.Blazor.Dagre.Models
{
    public class EdgeViewModel
        : GraphElement, IEdgeViewModel
    {
        public virtual Guid SourceId { get; set; }
        public virtual Guid TargetId { get; set; }
        public virtual ICollection<IPosition> Points { get; set; }
        public virtual string Shape { get; set; }
        public virtual string? StartMarkerId { get; set; }
        public virtual string? EndMarkerId { get; set; }

        public EdgeViewModel(Guid sourceId, Guid targetId, string? cssClass = null, string? shape = null, ICollection<IPosition>? points = null, string? label = null, Type? componentType = null)
            : base(label, cssClass, componentType)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
            this.Points = points ?? new Collection<IPosition>();
            this.Shape = shape ?? EdgeShape.BSpline;
            this.EndMarkerId = Consts.EdgeEndArrowId;
        }
    }
}
