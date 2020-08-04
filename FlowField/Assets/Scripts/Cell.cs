using UnityEngine;

namespace TMG.FlowField
{
	public class Cell
	{
		public bool isWalkable { get; private set; }
		public bool isDestination { get; private set; }
		public Vector3 worldPos;
		public Vector2Int gridIndex;
		public GridDirection bestDirection;
		public byte cost;
		public ushort bestCost;

		public Cell(bool _walkable, Vector3 _worldPos, Vector2Int _gridIndex)
		{
			isWalkable = _walkable;
			isDestination = false;
			worldPos = _worldPos;
			gridIndex = _gridIndex;
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
			isWalkable = false;
		}

		public void SetAsDestination()
		{
			cost = 0;
			bestCost = 0;
			isWalkable = true;
			isDestination = true;
			bestDirection = GridDirection.None;
		}

		public void ResetCell()
		{
			cost = 1;
			bestCost = ushort.MaxValue;
			isWalkable = true;
			isDestination = false;
		}
	}
}