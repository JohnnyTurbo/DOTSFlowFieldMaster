using UnityEngine;

public class Node
{
	public bool walkable;
	public Vector3 worldPos;
	
	public Node(bool _walkable, Vector3 _worldPos)
	{
		walkable = _walkable;
		worldPos = _worldPos;
	}
}
