using UnityEngine;

public class Node
{
	public bool walkable;
	public Vector3 worldPos;
	public Vector2Int nodeIndex;
	public Vector2Int bestDirection;
	public byte cost;
	public ushort bestCost;
	
	public Node(bool _walkable, Vector3 _worldPos, Vector2Int _nodeIndex)
	{
		walkable = _walkable;
		worldPos = _worldPos;
		nodeIndex = _nodeIndex;
		cost = 1;
		bestCost = ushort.MaxValue;
		bestDirection = Vector2Int.zero;
	}

	public void IncreaseCost(int amnt)
	{
		if(cost == byte.MaxValue) { return; }
		if (amnt + cost >= 255) { MakeImpassible(); }
		else { cost += (byte) amnt; } ;
	}

	public void MakeImpassible()
	{
		cost = byte.MaxValue;
		walkable = false;
	}
}
