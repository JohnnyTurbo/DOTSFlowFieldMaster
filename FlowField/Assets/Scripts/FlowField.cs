using System.Collections.Generic;
using UnityEngine;

namespace TMG.FlowField
{
	public class FlowField
	{
		public Cell[,] grid { get; private set; }
		public Vector2Int gridSize { get; private set; }
		public float cellRadius { get; private set; }

		public Cell destinationCell;
		
		private float cellDiameter;

		public FlowField(float _cellRadius, Vector2Int _gridSize)
		{
			cellRadius = _cellRadius;
			cellDiameter = _cellRadius * 2;
			gridSize = _gridSize;
		}

		public void CreateGrid()
		{
			//Debug.Log("Creating Grid");
			grid = new Cell[gridSize.x, gridSize.y];

			for(int x = 0; x < gridSize.x; x++)
			{
				for(int y = 0; y < gridSize.y; y++)
				{
					Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
					grid[x, y] = new Cell(worldPos, new Vector2Int(x,y));
				}
			}
		}

		public void Reset()
		{
			foreach(Cell curCell in grid)
			{
				curCell.ResetCell();
			}
		}

		public void CreateCostField()
		{
			Vector3 cellHalfExtents = Vector3.one * cellRadius;
			int terrainMask = LayerMask.GetMask("Impassible", "Terrain");
			foreach(Cell curCell in grid)
			{
				Collider[] obstacles = Physics.OverlapBox(curCell.worldPos, cellHalfExtents, Quaternion.identity, terrainMask);
				bool hasIncreasedCost = false;
				foreach(Collider col in obstacles)
				{
					if(col.gameObject.layer == 8)
					{
						curCell.MakeImpassible();
						continue;
					}
					else if(!hasIncreasedCost && col.gameObject.layer == 9)
					{
						curCell.IncreaseCost(3);
						hasIncreasedCost = true;
					}
				}
			}
		}

		public void CreateIntegrationField(Cell _destinationCell)
		{
			destinationCell = _destinationCell;
			
			destinationCell.bestCost = 0;

			Queue<Cell> cellsToCheck = new Queue<Cell>();

			cellsToCheck.Enqueue(destinationCell);

			Debug.Log(cellsToCheck.Count);
			while (cellsToCheck.Count > 0)
			{
				Cell curCell = cellsToCheck.Dequeue();
				List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
				foreach(Cell curNeighbor in curNeighbors)
				{
					if(curNeighbor.cost == byte.MaxValue) { continue; }
					if(curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
					{
						curNeighbor.bestCost = (ushort) (curNeighbor.cost + curCell.bestCost);
						cellsToCheck.Enqueue(curNeighbor);
					}
				}
			}
		}

		public void CreateFlowField()
		{
			foreach (Cell curCell in grid)
			{
				curCell.bestDirection = GridDirection.None;

				List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

				int bestCost = curCell.bestCost;

				foreach (Cell curNeighbor in curNeighbors)
				{
					if (curNeighbor.bestCost < bestCost)
					{
						bestCost = curNeighbor.bestCost;
						curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
					}
				}
			}
		}

		public Cell GetCellFromWorldPos(Vector3 worldPos)
		{
			float percentX = worldPos.x / (gridSize.x * cellDiameter);
			float percentY = worldPos.z / (gridSize.y * cellDiameter);
			
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
			int y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
			return grid[x, y];
		}

		public List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
		{
			List<Cell> neighborCells = new List<Cell>();

			foreach(Vector2Int curDirection in directions)
			{
				Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
				if(newNeighbor != null)
				{
					neighborCells.Add(newNeighbor);
				}
			}

			return neighborCells;
		}

		private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
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