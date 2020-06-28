using UnityEngine;

public class Node
{
	public bool walkable;
	public Vector3 worldPos;
	public byte cost;
	public ushort bestCost;
	
	public Node(bool _walkable, Vector3 _worldPos)
	{
		walkable = _walkable;
		worldPos = _worldPos;
		cost = 1;
		bestCost = ushort.MaxValue;
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
