using TMG.FlowField;
using UnityEngine;

namespace Tests
{
	public class FlowFieldBuilder
	{
		private float nodeRadius;
		private Vector2Int gridSize;

		public FlowFieldBuilder WithNodeRadius(float radius)
		{
			nodeRadius = radius;
			return this;
		}

		public FlowFieldBuilder WithSize(int x, int y)
		{
			gridSize = new Vector2Int(x, y);
			return this;
		}

		public FlowFieldGrid Build()
		{
			return new FlowFieldGrid(nodeRadius, gridSize);
		}

		public static implicit operator FlowFieldGrid(FlowFieldBuilder builder)
		{
			return builder.Build();
		}
	}
}
