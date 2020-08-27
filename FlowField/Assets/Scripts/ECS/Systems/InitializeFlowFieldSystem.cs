using Unity.Entities;
using UnityEngine;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldSystem : SystemBase
	{
		private Entity _flowField;
		private EntityQuery _flowFieldControllerQuery;
		private Entity _flowFieldControllerEntity;

		protected override void OnCreate()
		{
			_flowFieldControllerQuery = GetEntityQuery(typeof(FlowFieldControllerData));
			
		}

		protected override void OnUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
				
				_flowFieldControllerEntity = _flowFieldControllerQuery.GetSingletonEntity();

				FlowFieldControllerData flowFieldControllerData = EntityManager.GetComponentData<FlowFieldControllerData>(_flowFieldControllerEntity);
				GridDebug.instance.flowFieldControllerData = flowFieldControllerData;
				FlowFieldData flowFieldData = new FlowFieldData
				{
					gridSize = flowFieldControllerData.gridSize,
					cellRadius = flowFieldControllerData.cellRadius,
					clickedPos = worldMousePos
				};
				
				NewFlowFieldData newFlowFieldData = new NewFlowFieldData {isExistingFlowField = true};
				if (_flowField.Equals(Entity.Null))
				{
					Debug.Log("nullEntity");
					_flowField = EntityManager.CreateEntity();
					EntityManager.AddComponent<FlowFieldData>(_flowField);
					newFlowFieldData.isExistingFlowField = false;
				}
				EntityManager.AddComponent<NewFlowFieldData>(_flowField);
				EntityManager.SetComponentData(_flowField, flowFieldData);
				EntityManager.SetComponentData(_flowField, newFlowFieldData);
			}
		}
	}
}
