using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TMG.ECSFlowField
{
	public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };

	public class GridDebug : MonoBehaviour
	{
		public static GridDebug instance;

		public FlowFieldDisplayType curDisplayType;

		public bool displayGrid;
		public List<CellData> gridCellData;
		public FlowFieldControllerData flowFieldControllerData;

		Vector2Int gridSize;
		float cellRadius;

		private void Awake()
		{
			instance = this;
			gridCellData = new List<CellData>();
		}

		private void OnDrawGizmos()
		{
			if (displayGrid)
			{
				gridSize = new Vector2Int { x = flowFieldControllerData.gridSize.x, y = flowFieldControllerData.gridSize.y };
				cellRadius = flowFieldControllerData.cellRadius;

				if (gridCellData.IsNullOrEmpty())
				{
					DrawGrid(gridSize, Color.yellow, cellRadius);
				}
				else
				{
					DrawGrid(gridSize, Color.green, cellRadius);
				}
			}

			if (gridCellData.IsNullOrEmpty()) { return; }

			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;

			switch (curDisplayType)
			{
				case FlowFieldDisplayType.CostField:

					foreach (CellData curCell in gridCellData)
					{
						Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
					}
					break;

				case FlowFieldDisplayType.IntegrationField:

					foreach (CellData curCell in gridCellData)
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