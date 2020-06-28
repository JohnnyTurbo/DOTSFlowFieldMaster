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
		Vector2Int[] cardinalDirections = { new Vector2Int(0, 1), new Vector2Int(1,0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
		Vector2Int[] eightDirections = { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1) };

		Node goalNode;
		float nodeDiameter;
		bool displayFF;

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
				Node curNode = GetNodeFromWorldPos(worldMousePos);
				curNode.MakeImpassible();
			}

			else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Alt + Click");
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node curNode = GetNodeFromWorldPos(worldMousePos);
				curNode.IncreaseCost(10);
			}

			else if (Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Click");
				//Set goal & recalc cost
				if(goalNode != null) { goalNode.cost = 1; }
				
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node curNode = GetNodeFromWorldPos(worldMousePos);
				curNode.cost = 0;
				goalNode = curNode;

				CreateIntegrationField();
				CreateFlowField();
			}
		}

		private void CreateFlowField()
		{
			foreach(Node curNode in grid)
			{
				List<Node> curNeighbors = GetNeighborNodes(curNode.nodeIndex, eightDirections);

				int bestCost = curNode.bestCost;
				
				foreach(Node curNeighbor in curNeighbors)
				{
					if(curNeighbor.bestCost < bestCost)
					{
						bestCost = curNeighbor.bestCost;
						curNode.bestDirection = curNeighbor.nodeIndex - curNode.nodeIndex;
					}
				}
			}
			displayFF = true;
		}

		private void CreateIntegrationField()
		{
			foreach(Node n in grid)
			{
				n.bestCost = ushort.MaxValue;
			}

			goalNode.bestCost = 0;

			//List<Node> openList = new List<Node>();
			Queue<Node> nodes = new Queue<Node>();

			nodes.Enqueue(goalNode);

			while(nodes.Count > 0)
			{
				Node curNode = nodes.Dequeue();
				List<Node> curNeighbors = GetNeighborNodes(curNode.nodeIndex, cardinalDirections);
				foreach(Node curNeighbor in curNeighbors)
				{
					if(curNeighbor.cost == byte.MaxValue) { continue; }
					if(curNeighbor.cost + curNode.cost < curNeighbor.bestCost)
					{
						curNeighbor.bestCost = (ushort) (curNeighbor.cost + curNode.cost);
						nodes.Enqueue(curNeighbor);
					}
				}
			}
		}

		private List<Node> GetNeighborNodes(Vector2Int nodeIndex, Vector2Int[] directions)
		{
			List<Node> neighborNodes = new List<Node>();

			foreach(Vector2Int curDirection in directions)
			{
				Node newNeighbor = GetNodeAtRelativePos(nodeIndex, curDirection);
				if(newNeighbor != null)
				{
					neighborNodes.Add(newNeighbor);
				}
			}

			return neighborNodes;
		}

		private Node GetNodeAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
		{
			Vector2Int finalPos = orignPos + relativePos;

			if(finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
			{
				return null;
			}

			else { return grid[finalPos.x, finalPos.y]; }
		}

		private void CreateGrid()
		{
			//Debug.Log("Creating Grid");
			grid = new Node[gridSize.x, gridSize.y];

			for(int x = 0; x < gridSize.x; x++)
			{
				for(int y = 0; y < gridSize.y; y++)
				{
					Vector3 worldPos = new Vector3(nodeDiameter * x + nodeRadius, 0, nodeDiameter * y + nodeRadius);
					grid[x, y] = new Node(true, worldPos, new Vector2Int(x,y));
				}
			}
		}

		void OnDrawGizmos()
		{		
			if(grid != null)
			{				
				foreach (Node n in grid)
				{
					float greenLevel = (float)(256f - n.cost) / 256f;
					Color walkableColor = new Color(0, greenLevel, 0);
					Gizmos.color = n.walkable ? walkableColor : Color.red;
					if(n.cost == 0) { Gizmos.color = Color.yellow; }
					Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 0.1f));

					if (displayFF)
					{
						Gizmos.color = Color.white;
						Vector3 endLinePos = new Vector3(n.worldPos.x + n.bestDirection.x, 3, n.worldPos.z + n.bestDirection.y);
						Gizmos.DrawLine(n.worldPos, endLinePos);
					}
				}
			}
		}

		Node GetNodeFromWorldPos(Vector3 worldPos)
		{
			float percentX = worldPos.x / (gridSize.x * nodeDiameter);
			float percentY = worldPos.z / (gridSize.y * nodeDiameter);
			
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);
			
			int x = Mathf.FloorToInt((gridSize.x) * percentX);
			int y = Mathf.FloorToInt((gridSize.y) * percentY);
			return grid[x, y];
		}
	}
}