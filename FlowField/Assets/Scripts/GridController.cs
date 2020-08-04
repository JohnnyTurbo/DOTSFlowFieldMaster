using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TMG.FlowField
{
    public class GridController : MonoBehaviour
    {
        public Vector2Int gridSize;
        public float cellRadius = 1;
        public bool displayGrid;
		public bool displayFF;
		public bool displayGoal;
		public bool displayCF;
		public bool displayIF;
		public FlowField curFlowField;
		

        Cell destinationCell;

		private void GenerateNewGrid()
		{
			Debug.Log($"Generating grid of size: {gridSize.ToString()}");
			curFlowField = new FlowField(cellRadius, gridSize);
			curFlowField.CreateGrid();
		}

		
		void Update()
        {
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Ctrl + Click");
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Cell curCell = curFlowField.GetCellFromWorldPos(worldMousePos);
				curCell.MakeImpassible();
			}

			else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Alt + Click");
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Cell curCell = curFlowField.GetCellFromWorldPos(worldMousePos);
				curCell.IncreaseCost(10);
			}

			else if (Input.GetMouseButtonDown(0))
			{
				if(curFlowField == null || curFlowField.gridSize != gridSize)
				{
					GenerateNewGrid();
				}
				else
				{
					curFlowField.ResetGrid();
				}

				Stopwatch st = new Stopwatch();
				st.Start();

				curFlowField.CreateCostField();

				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Cell curCell = curFlowField.GetCellFromWorldPos(worldMousePos);
				destinationCell = curCell;
				destinationCell.SetAsDestination();

				curFlowField.CreateIntegrationField(destinationCell);
				curFlowField.CreateFlowField(displayFF, displayGoal);

				st.Stop();
				Debug.Log($"FFTime: {st.ElapsedMilliseconds}");
			}
		}

		void OnDrawGizmos()
		{
			if (displayGrid)
			{
				Gizmos.color = Color.green;
				for (int x = 0; x < gridSize.x; x++)
				{
					for (int y = 0; y < gridSize.y; y++)
					{
						Vector3 center = new Vector3(cellRadius * 2 * x + cellRadius, 0, cellRadius * 2 * y + cellRadius);
						Vector3 size = Vector3.one * cellRadius * 2;
						Gizmos.DrawWireCube(center, size);
					}
				}
			}

			if (curFlowField != null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.MiddleCenter;

				if (displayCF)
				{
					foreach (Cell curCell in curFlowField.grid)
					{
						Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
					}
				}

				else if (displayIF)
				{
					foreach (Cell curCell in curFlowField.grid)
					{
						Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
					}
				}
			}
		}
	}
}