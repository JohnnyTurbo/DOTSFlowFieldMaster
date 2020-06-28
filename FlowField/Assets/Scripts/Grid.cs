using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG.FlowField
{
	public class Grid : MonoBehaviour
	{
		public Vector2Int gridSize;
		public float nodeRadius = 1;

		Node[,] grid;
		float nodeDiameter;

		private void Start()
		{
			nodeDiameter = nodeRadius * 2;
			CreateGrid();
		}

		private void CreateGrid()
		{
			Debug.Log("Creating Cube");
			grid = new Node[gridSize.x, gridSize.y];

			for(int x = 0; x < gridSize.x; x++)
			{
				for(int y = 0; y < gridSize.y; y++)
				{
					Vector3 worldPos = new Vector3(nodeDiameter * x, 0, nodeDiameter * y);
					grid[x, y] = new Node(true, worldPos);
				}
			}
		}

		void OnDrawGizmos()
		{
			
			if(grid != null)
			{
				
				foreach (Node n in grid)
				{
					Gizmos.color = n.walkable ? Color.green : Color.red;
					Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 0.1f));
				}
			}
		}
	}
}