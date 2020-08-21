using Unity.Entities;
using UnityEngine;

namespace TMG.ECSFlowField
{
	public class GenerateFlowFieldSystem : SystemBase
	{
		private Entity _flowField;
		private EntityQuery _flowFieldControllerQuery;
		private Entity _flowFieldControllerEntity;

		protected override void OnCreate()
		{
			_flowFieldControllerQuery = GetEntityQuery(typeof(FlowFieldControllerData));
			
		}

		protected override void OnStartRunning()
		{
			
		}

		protected override void OnUpdate()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				_flowFieldControllerEntity = _flowFieldControllerQuery.GetSingletonEntity();
				_flowField = EntityManager.CreateEntity();
				EntityManager.AddComponent<GeneratePerlinNoiseTag>(_flowField);
				EntityManager.AddComponent<FlowFieldData>(_flowField);
				FlowFieldControllerData flowFieldControllerData = EntityManager.GetComponentData<FlowFieldControllerData>(_flowFieldControllerEntity);
				GridDebug.instance.flowFieldControllerData = flowFieldControllerData;
				FlowFieldData flowFieldData = new FlowFieldData
				{
					gridSize = flowFieldControllerData.gridSize,
					cellRadius = flowFieldControllerData.cellRadius,
					noiseScale = flowFieldControllerData.noiseScale
				};
				
				EntityManager.SetComponentData(_flowField, flowFieldData);
				
				
			}
			if (Input.GetMouseButtonDown(0))
			{
				
				EntityManager.AddComponent<NewFlowFieldTag>(_flowField);
			}
		}
	}
}
