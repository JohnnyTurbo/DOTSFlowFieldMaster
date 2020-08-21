using System;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TMG.ECSFlowField
{
	public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField, CostHeatMap };

	public class GridDebug : MonoBehaviour
	{
		public static GridDebug instance;

		public FlowFieldDisplayType curDisplayType;

		public bool displayGrid;
		public List<CellData> gridCellData;
		public FlowFieldControllerData flowFieldControllerData;

		private Vector2Int _gridSize;
		private float _cellRadius;

		private void Awake()
		{
			instance = this;
			gridCellData = new List<CellData>();
		}

		private void OnDrawGizmos()
		{
			if (displayGrid)
			{
				_gridSize = new Vector2Int { x = flowFieldControllerData.gridSize.x, y = flowFieldControllerData.gridSize.y };
				_cellRadius = flowFieldControllerData.cellRadius;

				DrawGrid(_gridSize, gridCellData.IsNullOrEmpty() ? Color.yellow : Color.green, _cellRadius);
			}

			if (gridCellData.IsNullOrEmpty()) { return; }

			GUIStyle style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};

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
				
				case FlowFieldDisplayType.CostHeatMap:
					foreach (CellData curCell in gridCellData)
					{
						float costHeat = curCell.cost / 255f;
						Gizmos.color = new Color(costHeat, costHeat, costHeat);
						Vector3 center = new Vector3(_cellRadius * 2 * curCell.gridIndex.x + _cellRadius, 0, _cellRadius * 2 * curCell.gridIndex.y + _cellRadius);
						Vector3 size = Vector3.one * _cellRadius * 2;
						Gizmos.DrawCube(center, size);
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