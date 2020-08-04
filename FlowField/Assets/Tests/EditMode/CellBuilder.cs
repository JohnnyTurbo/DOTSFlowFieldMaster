using UnityEngine;
using TMG.FlowField;

namespace Tests
{ 
	public class CellBuilder
	{
		private bool isWalkable = false;
		private Vector3 worldPos;
		private Vector2Int cellIndex;

		public CellBuilder IsWalkable()
		{
			isWalkable = true;
			return this;
		}

		public CellBuilder AtZero()
		{
			worldPos = Vector3.zero;
			cellIndex = Vector2Int.zero;
			return this;
		}

		public CellBuilder AtPosition(Vector3 position)
		{
			worldPos = position;
			return this;
		}

		public CellBuilder AtIndex(Vector2Int index)
		{
			cellIndex = index;
			return this;
		}

		public Cell Build()
		{
			return new Cell(isWalkable, worldPos, cellIndex);
		}

		public static implicit operator Cell(CellBuilder builder)
		{
			return builder.Build();
		}
	}
}
