using UnityEngine;
using TMG.FlowField;

namespace Tests
{ 
	public class NodeBuilder
	{
		private bool isWalkable = false;
		private Vector3 worldPos;
		private Vector2Int nodeIndex;

		public NodeBuilder IsWalkable()
		{
			isWalkable = true;
			return this;
		}

		public NodeBuilder AtZero()
		{
			worldPos = Vector3.zero;
			nodeIndex = Vector2Int.zero;
			return this;
		}

		public NodeBuilder AtPosition(Vector3 position)
		{
			worldPos = position;
			return this;
		}

		public NodeBuilder AtIndex(Vector2Int index)
		{
			nodeIndex = index;
			return this;
		}

		public Node Build()
		{
			return new Node(isWalkable, worldPos, nodeIndex);
		}

		public static implicit operator Node(NodeBuilder builder)
		{
			return builder.Build();
		}
	}
}
