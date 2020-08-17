using Unity.Entities;
using UnityEngine;

namespace TMG.ECSFlowField
{
	public class GenerateFlowFieldSystem : SystemBase
	{
		Entity flowField;
		EntityQuery flowFieldControllerQuery;

		protected override void OnCreate()
		{
			flowFieldControllerQuery = GetEntityQuery(typeof(FlowFieldControllerData));
		}

		protected override void OnUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Entity flowFieldControllerEntity = flowFieldControllerQuery.GetSingletonEntity();
				FlowFieldControllerData flowFieldControllerData = EntityManager.GetComponentData<FlowFieldControllerData>(flowFieldControllerEntity);
				GridDebug.instance.flowFieldControllerData = flowFieldControllerData;
				FlowFieldData flowFieldData = new FlowFieldData
				{
					gridSize = flowFieldControllerData.gridSize,
					cellRadius = flowFieldControllerData.cellRadius
				};
				flowField = EntityManager.CreateEntity();
				EntityManager.AddComponent<FlowFieldData>(flowField);
				EntityManager.AddComponent<NewFlowFieldTag>(flowField);
				EntityManager.SetComponentData(flowField, flowFieldData);
			}
		}
	}
}
