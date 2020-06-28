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

		private void Update()
		{
			if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Ctrl + Click");
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node unwalkNode = GetNodeFromWorldPos(worldMousePos);
				unwalkNode.MakeImpassible();
			}

			else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Alt + Click");

			}

			else if (Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Click");
				//Set goal & recalc cost
			}
		}

		private void CreateGrid()
		{
			Debug.Log("Creating Cube");
			grid = new Node[gridSize.x, gridSize.y];

			for(int x = 0; x < gridSize.x; x++)
			{
				for(int y = 0; y < gridSize.y; y++)
				{
					Vector3 worldPos = new Vector3(nodeDiameter * x + nodeRadius, 0, nodeDiameter * y + nodeRadius);
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

		Node GetNodeFromWorldPos(Vector3 worldPos)
		{
			float percentX = worldPos.x / (gridSize.x * nodeDiameter);
			float percentY = worldPos.z / (gridSize.y * nodeDiameter);
			Debug.Log("X: " + percentX + " Y: " + percentY);
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);
			
			int x = Mathf.FloorToInt((gridSize.x) * percentX);
			int y = Mathf.FloorToInt((gridSize.y) * percentY);
			return grid[x, y];
		}
	}
}