using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TMG.ECSFlowField
{
	public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField, CostHeatMap };

	public class GridDebug : MonoBehaviour
	{
		public static GridDebug instance;

		[SerializeField] private FlowFieldDisplayType _curDisplayType;
		[SerializeField] private bool _displayGrid;
		
		private FlowFieldControllerData _flowFieldControllerData;
		public FlowFieldControllerData FlowFieldControllerData
		{
			get => _flowFieldControllerData;
			set => _flowFieldControllerData = value;
		}
		
		private List<CellData> _gridCellData;
		
		private Vector2Int _gridSize;
		private float _cellRadius;

		private void Awake()
		{
			instance = this;
			_gridCellData = new List<CellData>();
		}

		private void OnDrawGizmos()
		{
			if (_displayGrid)
			{
				_gridSize = new Vector2Int { x = _flowFieldControllerData.gridSize.x, y = _flowFieldControllerData.gridSize.y };
				_cellRadius = _flowFieldControllerData.cellRadius;

				DrawGrid(_gridSize, (_gridCellData == null || _gridCellData.Count == 0) ? Color.yellow : Color.green, _cellRadius);
			}

			if (_gridCellData == null || _gridCellData.Count == 0) { return; }

			GUIStyle style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};

			switch (_curDisplayType)
			{
				case FlowFieldDisplayType.CostField:

					foreach (CellData curCell in _gridCellData)
					{
						Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
					}
					break;

				case FlowFieldDisplayType.IntegrationField:

					foreach (CellData curCell in _gridCellData)
					{
						Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
					}
					break;
				
				case FlowFieldDisplayType.CostHeatMap:
					foreach (CellData curCell in _gridCellData)
					{
						float costHeat = curCell.cost / 255f;
						Gizmos.color = new Color(costHeat, costHeat, costHeat);
						Vector3 center = new Vector3(_cellRadius * 2 * curCell.gridIndex.x + _cellRadius, 0, _cellRadius * 2 * curCell.gridIndex.y + _cellRadius);
						Vector3 size = Vector3.one * _cellRadius * 2;
						Gizmos.DrawCube(center, size);
					}
					break;
				
				case FlowFieldDisplayType.AllIcons:
					foreach (CellData curCell in _gridCellData)
					{
						Handles.Label(curCell.worldPos, curCell.bestDirection.ToString(), style);
					}
					break;
				
				case FlowFieldDisplayType.DestinationIcon:
					break;
				
				case FlowFieldDisplayType.None:
					break;
				
				default:
					Debug.LogWarning("Warning: Invalid Grid Debug Display Type", gameObject);
					break;
			}
		}

		private static void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
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

		public void ClearList() => _gridCellData.Clear();

		public void AddToList(CellData cellToAdd) => _gridCellData.Add(cellToAdd);
	}
}