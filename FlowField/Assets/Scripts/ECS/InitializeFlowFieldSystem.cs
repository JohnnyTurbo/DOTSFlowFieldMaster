using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;
		EntityArchetype cellArchetype;
		CollisionFilter sharedCollisionFilter;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnStartRunning()
		{
			cellArchetype = EntityManager.CreateArchetype(
															typeof(CellData),
															typeof(CellSharedData),
															typeof(GenerateCostFieldTag)
															);
			sharedCollisionFilter = new CollisionFilter()
			{
				BelongsTo = ~0u,
				CollidesWith = (1u << 0) | (1u << 1),
				GroupIndex = 0
			};
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();
			
			
			Entities.ForEach((Entity entity, int entityInQueryIndex, in NewFlowFieldTag newFlowFieldTag, in FlowFieldData flowFieldData) =>
			{
				commandBuffer.RemoveComponent<NewFlowFieldTag>(entity);

				DynamicBuffer<GridCellBufferElement> buffer = commandBuffer.AddBuffer<GridCellBufferElement>(entity);
				DynamicBuffer<CellData> cellBuffer = buffer.Reinterpret<CellData>();

				float newCellRadius = flowFieldData.cellRadius;

				CellSharedData newCellSharedData = new CellSharedData
				{
					cellRadius = newCellRadius,
					cellDiameter = newCellRadius * 2,
					halfExtents = new float3(newCellRadius, newCellRadius, newCellRadius),
					costFieldFilter = sharedCollisionFilter
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