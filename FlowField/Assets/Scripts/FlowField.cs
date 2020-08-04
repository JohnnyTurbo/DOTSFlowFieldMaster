using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TMG.FlowField
{
	public class FlowField
	{
		public Cell[,] grid { get; private set; }
		public Vector2Int gridSize { get; private set; }
		private float cellRadius;
		private float cellDiameter;
		private Sprite[] ffIcons;
		private GameObject iconContainer;

		public FlowField(float _cellRadius, Vector2Int _gridSize)
		{
			cellRadius = _cellRadius;
			cellDiameter = _cellRadius * 2;
			gridSize = _gridSize;
			ffIcons = Resources.LoadAll<Sprite>("Sprites/FFicons");
			iconContainer = new GameObject();
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
					grid[x, y] = new Cell(true, worldPos, new Vector2Int(x,y));
				}
			}
		}

		public void ResetGrid()
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

		public void CreateIntegrationField(Cell destinationCell)
		{
			foreach(Cell curCell in grid)
			{
				curCell.bestCost = ushort.MaxValue;
			}

			destinationCell.bestCost = 0;

			Queue<Cell> cellsToCheck = new Queue<Cell>();

			cellsToCheck.Enqueue(destinationCell);

			while(cellsToCheck.Count > 0)
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

		public void CreateFlowField(bool displayField, bool displayGoal)
		{
			ClearCellDisplay();
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

				if (displayField || (!displayField && displayGoal && curCell.isDestination))
				{
					DisplayCell(curCell);
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

		private void DisplayCell(Cell cell)
		{
			GameObject iconGO = new GameObject();
			SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
			iconGO.transform.parent = iconContainer.transform;
			iconGO.transform.position = cell.worldPos;

			if (cell.isDestination)
			{
				iconSR.sprite = ffIcons[3];
				Quaternion newRot = Quaternion.Euler(90, 0, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (!cell.isWalkable)
			{
				iconSR.sprite = ffIcons[2];
				Quaternion newRot = Quaternion.Euler(90, 0, 0);
				iconGO.transform.rotation = newRot;
			}
			else if(cell.bestDirection == GridDirection.North)
			{
				iconSR.sprite = ffIcons[0];
				Quaternion newRot = Quaternion.Euler(90, 0, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.South)
			{
				iconSR.sprite = ffIcons[0];
				Quaternion newRot = Quaternion.Euler(90, 180, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.East)
			{
				iconSR.sprite = ffIcons[0];
				Quaternion newRot = Quaternion.Euler(90, 90, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.West)
			{
				iconSR.sprite = ffIcons[0];
				Quaternion newRot = Quaternion.Euler(90, 270, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.NorthEast)
			{
				iconSR.sprite = ffIcons[1];
				Quaternion newRot = Quaternion.Euler(90, 0, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.NorthWest)
			{
				iconSR.sprite = ffIcons[1];
				Quaternion newRot = Quaternion.Euler(90, 270, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.SouthEast)
			{
				iconSR.sprite = ffIcons[1];
				Quaternion newRot = Quaternion.Euler(90, 90, 0);
				iconGO.transform.rotation = newRot;
			}
			else if (cell.bestDirection == GridDirection.SouthWest)
			{
				iconSR.sprite = ffIcons[1];
				Quaternion newRot = Quaternion.Euler(90, 180, 0);
				iconGO.transform.rotation = newRot;
			}
			else
			{
				iconSR.sprite = ffIcons[0];
			}
		}

		public void ClearCellDisplay()
		{
			foreach(Transform t in iconContainer.transform)
			{
				GameObject.Destroy(t.gameObject);
			}
		}
	}
}