using UnityEngine;
using UnityEditor;
using TMG.FlowField;
//using TMG.ECSFlowField;

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
