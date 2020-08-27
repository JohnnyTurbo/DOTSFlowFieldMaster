using Unity.Entities;
using UnityEngine;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldSystem : SystemBase
	{
		private Entity _flowFieldEntity;
		private EntityQuery _flowFieldControllerQuery;
		private Entity _flowFieldControllerEntity;
		private Camera _mainCamera;
		protected override void OnCreate()
		{
			_flowFieldControllerQuery = GetEntityQuery(typeof(FlowFieldControllerData));
		}

		protected override void OnStartRunning()
		{
			_mainCamera = Camera.main;
		}

		protected override void OnUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
				Vector3 worldMousePos = _mainCamera.ScreenToWorldPoint(mousePos);
				
				_flowFieldControllerEntity = _flowFieldControllerQuery.GetSingletonEntity();

				FlowFieldControllerData flowFieldControllerData = EntityManager.GetComponentData<FlowFieldControllerData>(_flowFieldControllerEntity);
				GridDebug.instance.FlowFieldControllerData = flowFieldControllerData;
				
				FlowFieldData flowFieldData = new FlowFieldData
				{
					gridSize = flowFieldControllerData.gridSize,
					cellRadius = flowFieldControllerData.cellRadius,
					clickedPos = worldMousePos
				};
				
				NewFlowFieldData newFlowFieldData = new NewFlowFieldData {isExistingFlowField = true};
				
				if (_flowFieldEntity.Equals(Entity.Null))
				{
					_flowFieldEntity = EntityManager.CreateEntity();
					EntityManager.AddComponent<FlowFieldData>(_flowFieldEntity);
					newFlowFieldData.isExistingFlowField = false;
				}
				
				EntityManager.AddComponent<NewFlowFieldData>(_flowFieldEntity);
				EntityManager.SetComponentData(_flowFieldEntity, flowFieldData);
				EntityManager.SetComponentData(_flowFieldEntity, newFlowFieldData);
			}
		}
	}
}
