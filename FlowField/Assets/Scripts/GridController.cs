using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG.FlowField
{
    public class GridController : MonoBehaviour
    {
        public Vector2Int gridSize;
        public float nodeRadius = 1;

        Node goalNode;
        bool displayFF;
		FlowFieldGrid sceneGrid;

		void Start()
        {
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
				if (goalNode != null) { goalNode.cost = 1; }

				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				Node curNode = sceneGrid.GetNodeFromWorldPos(worldMousePos);
				curNode.cost = 0;
				goalNode = curNode;

				sceneGrid.CreateIntegrationField(goalNode);
				sceneGrid.CreateFlowField();
				displayFF = true;
			}
		}

		void OnDrawGizmos()
		{
			if (Application.isPlaying && sceneGrid.grid != null)
			{
				foreach (Node n in sceneGrid.grid)
				{
					float greenLevel = (float)(256f - n.cost) / 256f;
					Color walkableColor = new Color(0, greenLevel, 0);
					Gizmos.color = n.walkable ? walkableColor : Color.red;
					if (n.cost == 0) { Gizmos.color = Color.yellow; }
					Gizmos.DrawCube(n.worldPos, Vector3.one * ((nodeRadius * 2) - 0.1f));

					if (displayFF)
					{
						Gizmos.color = Color.white;
						Vector3 endLinePos = new Vector3(n.worldPos.x + n.bestDirection.x, 3, n.worldPos.z + n.bestDirection.y);
						Gizmos.DrawLine(n.worldPos, endLinePos);
					}
				}
			}
		}
	}
}