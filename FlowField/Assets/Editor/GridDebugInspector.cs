using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMG.FlowField;

[CustomEditor(typeof(GridDebug))]
public class GridDebugInspector : Editor
{
	GridDebug gridDebug;

	private void OnEnable()
	{
		gridDebug = (GridDebug)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if(GUILayout.Button("Draw Flow Field"))
		{
			gridDebug.DrawFlowField();
		}
	}
}
