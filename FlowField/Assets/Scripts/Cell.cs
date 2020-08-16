using UnityEngine;

namespace TMG.FlowField
{
	public class Cell
	{
		public Vector3 worldPos;
		public Vector2Int gridIndex;

		public byte cost;

		public ushort bestCost;
		
		public GridDirection bestDirection;

		public Cell(Vector3 _worldPos, Vector2Int _gridIndex)
		{
			worldPos = _worldPos;
			gridIndex = _gridIndex;

			cost = 1;

			bestCost = ushort.MaxValue;

			bestDirection = GridDirection.None;
		}

		public Cell(bool _walkable, Vector3 _worldPos, Vector2Int _gridIndex)
		{
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
		}

		public void SetAsDestination()
		{
			cost = 0;
			bestCost = 0;
			
			bestDirection = GridDirection.None;
		}

		public void ResetCell()
		{
			cost = 1;
			bestCost = ushort.MaxValue;

			bestDirection = GridDirection.None;
		}
	}
}