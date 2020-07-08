using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG.FlowField
{
	public class FlowFieldGrid
	{
		public Node[,] grid { get; private set; }
		private float nodeRadius;
		private float nodeDiameter;
		private Vector2Int gridSize;

		public FlowFieldGrid(float _nodeRadius, Vector2Int _gridSize)
		{
			nodeRadius = _nodeRadius;
			nodeDiameter = _nodeRadius * 2;
			gridSize = _gridSize;
		}

		public void CreateGrid()
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

		public void CreateIntegrationField(Node goalNode)
		{
			foreach(Node n in grid)
			{
				n.bestCost = ushort.MaxValue;
			}

			goalNode.bestCost = 0;

			Queue<Node> nodes = new Queue<Node>();

			nodes.Enqueue(goalNode);

			while(nodes.Count > 0)
			{
				Node curNode = nodes.Dequeue();
				List<Node> curNeighbors = GetNeighborNodes(curNode.nodeIndex, GridDirection.CardinalDirections);
				foreach(Node curNeighbor in curNeighbors)
				{
					if(curNeighbor.cost == byte.MaxValue) { continue; }
					if(curNeighbor.cost + curNode.bestCost < curNeighbor.bestCost)
					{
						curNeighbor.bestCost = (ushort) (curNeighbor.cost + curNode.bestCost);
						nodes.Enqueue(curNeighbor);
					}
				}
			}
		}

		public void CreateFlowField()
		{
			foreach(Node curNode in grid)
			{
				curNode.bestDirection = GridDirection.None;
				List<Node> curNeighbors = GetNeighborNodes(curNode.nodeIndex, GridDirection.AllDirections);

				int bestCost = curNode.bestCost;
				
				foreach(Node curNeighbor in curNeighbors)
				{
					if(curNeighbor.bestCost < bestCost)
					{
						bestCost = curNeighbor.bestCost;
						curNode.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.nodeIndex - curNode.nodeIndex);
					}
				}
			}
		}

		public Node GetNodeFromWorldPos(Vector3 worldPos)
		{
			float percentX = worldPos.x / (gridSize.x * nodeDiameter);
			float percentY = worldPos.z / (gridSize.y * nodeDiameter);
			
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);
			
			int x = Mathf.FloorToInt((gridSize.x) * percentX);
			int y = Mathf.FloorToInt((gridSize.y) * percentY);
			return grid[x, y];
		}

		public List<Node> GetNeighborNodes(Vector2Int nodeIndex, List<GridDirection> directions)
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
	}
}