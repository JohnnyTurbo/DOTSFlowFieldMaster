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

	public void IncreaseCost()
	{
		if(cost == byte.MaxValue) { return; }
		cost++;
		if(cost == byte.MaxValue) { walkable = false; }
	}

	public void MakeImpassible()
	{
		cost = byte.MaxValue;
		walkable = false;
	}
}
