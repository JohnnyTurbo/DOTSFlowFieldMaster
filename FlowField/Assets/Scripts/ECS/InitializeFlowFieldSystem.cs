using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();
			EntityArchetype cellArchetype = EntityManager.CreateArchetype(
																			typeof(CellData),
																			typeof(CellSharedData),
																			typeof(GenerateCostFieldTag)
																			);
			
			Entities.ForEach((Entity entity, int entityInQueryIndex, in NewFlowFieldTag newFlowFieldTag, in FlowFieldData flowFieldData) =>
			{
				// TODO: Figure out why this isn't working with Burst
				commandBuffer.RemoveComponent<NewFlowFieldTag>(entity);
				
				DynamicBuffer<GridCellBufferElement> buffer = commandBuffer.AddBuffer<GridCellBufferElement>(entity);
				DynamicBuffer<CellData> cellBuffer = buffer.Reinterpret<CellData>();

				int2 gridSize = flowFieldData.gridSize;
				float cellRadius = flowFieldData.cellRadius;
				float cellDiameter = cellRadius * 2;

				for (int x = 0; x < gridSize.x; x++)
				{
					for (int y = 0; y < gridSize.y; y++)
					{
						//UnityEngine.Debug.Log($"{x}, {y}");
						float3 worldPos = new float3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
						CellData newCellData = new CellData
						{
							worldPos = worldPos,
							gridIndex = new int2(x, y),
							cost = 1,
							bestCost = ushort.MaxValue,
							bestDirection = int2.zero
						};

						Entity newCell = commandBuffer.CreateEntity(cellArchetype);
						commandBuffer.SetComponent(newCell, newCellData);
						cellBuffer.Add(newCellData);
					}
				}
				//commandBuffer.AddComponent<GenerateCostFieldTag>(entity);

			}).Run();
			
			EntityQuery newCells = GetEntityQuery(typeof(CellSharedData));
			CellSharedData defaultCellSharedData = new CellSharedData
			{
				cellRadius = 0.5f,
				cellDiameter = 1f,
				halfExtents = new float3(0.5f, 0.5f, 0.5f)
			};
			EntityManager.SetSharedComponentData(newCells, defaultCellSharedData);
		}
	}
}