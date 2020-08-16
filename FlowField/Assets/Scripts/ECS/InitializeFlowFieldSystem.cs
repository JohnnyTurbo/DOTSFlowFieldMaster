using Unity.Physics;
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
				commandBuffer.RemoveComponent<NewFlowFieldTag>(entity);

				DynamicBuffer<GridCellBufferElement> buffer = commandBuffer.AddBuffer<GridCellBufferElement>(entity);
				DynamicBuffer<CellData> cellBuffer = buffer.Reinterpret<CellData>();

				float newCellRadius = flowFieldData.cellRadius;
				CollisionFilter newCostFieldFilter = new CollisionFilter();
				newCostFieldFilter.CollidesWith = 1u << 1 | 1u << 2;

				CellSharedData newCellSharedData = new CellSharedData
				{
					cellRadius = newCellRadius,
					cellDiameter = newCellRadius * 2,
					halfExtents = new float3(newCellRadius, newCellRadius, newCellRadius),
					costFieldFilter = newCostFieldFilter
				};

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
						// TODO Figure out a way to make this work with Burst
						commandBuffer.SetSharedComponent(newCell, newCellSharedData);
						cellBuffer.Add(newCellData);
					}
				}
			}).WithoutBurst().Run();
		}
	}
}