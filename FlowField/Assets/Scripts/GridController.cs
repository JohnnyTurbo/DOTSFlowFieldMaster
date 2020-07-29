using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Debug = UnityEngine.Debug;

namespace TMG.FlowField
{
    public class GridController : MonoBehaviour
    {
        public Vector2Int gridSize;
        public float nodeRadius = 1;
        public bool displayGrid;
		public FlowFieldGrid sceneGrid;
		

        Node goalNode;

		private void GenerateNewGrid()
		{
			Debug.Log($"Generating grid of size: {gridSize.ToString()}");
			sceneGrid = new FlowFieldGrid(nodeRadius, gridSize);
			sceneGrid.CreateGrid();
		}

		
		void Update()
        {
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Ctrl + Click");
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node curNode = sceneGrid.GetNodeFromWorldPos(worldMousePos);
				curNode.MakeImpassible();
			}

			else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Alt + Click");
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node curNode = sceneGrid.GetNodeFromWorldPos(worldMousePos);
				curNode.IncreaseCost(10);
			}

			else if (Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Click");
				//Set goal & recalc cost
				if(sceneGrid == null || sceneGrid.gridSize != gridSize)
				{
					GenerateNewGrid();
				}

				if (goalNode != null) 
				{
					goalNode.isDestination = false;
					goalNode.cost = 1; 
				}

				Stopwatch st = new Stopwatch();
				st.Start();

				sceneGrid.CreateCostField();

				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node curNode = sceneGrid.GetNodeFromWorldPos(worldMousePos);
				goalNode = curNode;
				goalNode.cost = 0;
				goalNode.isDestination = true;

				sceneGrid.CreateIntegrationField(goalNode);
				sceneGrid.CreateFlowField();

				st.Stop();
				Debug.Log($"FFTime: {st.ElapsedMilliseconds}");
			}
		}

		void OnDrawGizmos()
		{
			if (!displayGrid) { return; }
			Gizmos.color = Color.green;
			for (int x = 0; x < gridSize.x; x++)
			{
				for(int y = 0; y < gridSize.y; y++)
				{
					Vector3 center = new Vector3(nodeRadius * 2 * x + nodeRadius, 0, nodeRadius * 2 * y + nodeRadius);
					Vector3 size = Vector3.one * nodeRadius * 2;
					Gizmos.DrawWireCube(center, size);
				}
			}
		}
	}
}