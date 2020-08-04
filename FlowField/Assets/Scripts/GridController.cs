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
		public FlowField curFlowField;
		public GridDebug gridDebug;

        private Cell destinationCell;

		private void GenerateNewGrid()
		{
			Debug.Log($"Generating grid of size: {gridSize.ToString()}");
			curFlowField = new FlowField(cellRadius, gridSize);
			curFlowField.CreateGrid();
			gridDebug.SetFlowField(curFlowField);
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
				curFlowField.CreateFlowField();

				st.Stop();
				Debug.Log($"FFTime: {st.ElapsedMilliseconds}");
			}
		}
	}
}