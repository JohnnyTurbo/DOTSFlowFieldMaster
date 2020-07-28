using UnityEngine;

namespace TMG.FlowField
{
	public class Node
	{
		public bool walkable { get; private set; }
		public bool isDestination;
		public Vector3 worldPos;
		public Vector2Int nodeIndex;
		public GridDirection bestDirection;
		public byte cost;
		public ushort bestCost;

		public Node(bool _walkable, Vector3 _worldPos, Vector2Int _nodeIndex)
		{
			walkable = _walkable;
			isDestination = false;
			worldPos = _worldPos;
			nodeIndex = _nodeIndex;
			cost = 1;
			bestCost = ushort.MaxValue;
			bestDirection = GridDirection.None;
		}

		public void IncreaseCost(int amnt)
		{
			if (cost == byte.MaxValue) { return; }
			if (amnt + cost >= 255) { MakeImpassible(); }
			else { cost += (byte)amnt; };
		}

		public void MakeImpassible()
		{
			cost = byte.MaxValue;
			walkable = false;
		}
	}
}