using TMG.FlowField;
using UnityEngine;

namespace Tests
{
	public class FlowFieldBuilder
	{
		private float cellRadius;
		private Vector2Int gridSize;

		public FlowFieldBuilder WithCellRadius(float radius)
		{
			cellRadius = radius;
			return this;
		}

		public FlowFieldBuilder WithSize(int x, int y)
		{
			gridSize = new Vector2Int(x, y);
			return this;
		}

		public FlowField Build()
		{
			return new FlowField(cellRadius, gridSize);
		}

		public static implicit operator FlowField(FlowFieldBuilder builder)
		{
			return builder.Build();
		}
	}
}
