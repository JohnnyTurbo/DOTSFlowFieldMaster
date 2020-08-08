using UnityEngine;
using UnityEditor;

namespace TMG.FlowField
{
	public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField};

    public class GridDebug : MonoBehaviour
    {
		public GridController gridController;
		public bool displayGrid;

		public FlowFieldDisplayType curDisplayType;

		private Vector2Int gridSize;
		private float cellRadius;
		private FlowField curFlowField;

		private Sprite[] ffIcons;

		private void Start()
		{
			ffIcons = Resources.LoadAll<Sprite>("Sprites/FFicons");
		}

		public void SetFlowField(FlowField newFlowField)
		{
			curFlowField = newFlowField;
			cellRadius = newFlowField.cellRadius;
			gridSize = newFlowField.gridSize;
		}

		public void DrawFlowField()
		{
			ClearCellDisplay();

			switch (curDisplayType)
			{
				case FlowFieldDisplayType.AllIcons:
					DisplayAllCells();
					break;

				case FlowFieldDisplayType.DestinationIcon:
					DisplayDestinationCell();
					break;

				default:
					break;
			}
		}

		private void DisplayAllCells()
		{
			if (curFlowField == null) { return; }
			foreach(Cell curCell in curFlowField.grid)
			{
				DisplayCell(curCell);
			}
		}

		private void DisplayDestinationCell()
		{
			if (curFlowField == null) { return; }
			DisplayCell(curFlowField.destinationCell);
		}

		private void DisplayCell(Cell cell)
		{
			GameObject iconGO = new GameObject();
			SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
			iconGO.transform.parent = transform;
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
			else if (cell.bestDirection == GridDirection.North)
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
			foreach (Transform t in transform)
			{
				GameObject.Destroy(t.gameObject);
			}
		}
		
		private void OnDrawGizmos()
		{
			if (displayGrid)
			{
				if (curFlowField == null)
				{
					DrawGrid(gridController.gridSize, Color.yellow, gridController.cellRadius);
				}
				else
				{
					DrawGrid(gridSize, Color.green, cellRadius);
				}
			}

			if (curFlowField == null) { return; }

			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;

			switch (curDisplayType)
			{
				case FlowFieldDisplayType.CostField:

					foreach (Cell curCell in curFlowField.grid)
					{
						Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
					}
					break;

				case FlowFieldDisplayType.IntegrationField:

					foreach (Cell curCell in curFlowField.grid)
					{
						Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
					}
					break;

				default:
					break;
			}
		}

		private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
		{
			Gizmos.color = drawColor;
			for (int x = 0; x < drawGridSize.x; x++)
			{
				for (int y = 0; y < drawGridSize.y; y++)
				{
					Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
					Vector3 size = Vector3.one * drawCellRadius * 2;
					Gizmos.DrawWireCube(center, size);
				}
			}
		}
	}
}