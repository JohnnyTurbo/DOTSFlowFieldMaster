using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;
		static EntityArchetype cellArchetype;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
			cellArchetype = EntityManager.CreateArchetype(
															typeof(CellData),
															typeof(GenerateCostFieldTag)
															);
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, int entityInQueryIndex, in NewFlowFieldTag newFlowFieldTag, in FlowFieldData flowFieldData) =>
			{
				commandBuffer.RemoveComponent<NewFlowFieldTag>(entity);

				DynamicBuffer<GridCellBufferElement> buffer = commandBuffer.AddBuffer<GridCellBufferElement>(entity);
				DynamicBuffer<CellData> cellBuffer = buffer.Reinterpret<CellData>();

				CollisionFilter sharedCollisionFilter = new CollisionFilter()
				{
					BelongsTo = ~0u,
					CollidesWith = (1u << 0) | (1u << 1),
					GroupIndex = 0
				};
				
				float newCellRadius = flowFieldData.cellRadius;
				float newCellDiameter = newCellRadius * 2;

				CellSharedData newCellSharedData = new CellSharedData
				{
					cellRadius = newCellRadius,
					cellDiameter = newCellRadius * 2,
					halfExtents = new float3(newCellRadius, newCellRadius, newCellRadius),
					costFieldFilter = sharedCollisionFilter
				};
				
				BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);

				ref CellSharedData cbd = ref blobBuilder.ConstructRoot<CellSharedData>();
				cbd =  newCellSharedData;
				BlobAssetReference<CellSharedData> csd = blobBuilder.CreateBlobAssetReference<CellSharedData>(Allocator.Persistent);
				blobBuilder.Dispose();

				int2 gridSize = flowFieldData.gridSize;

				for (int x = 0; x < gridSize.x; x++)
				{
					for (int y = 0; y < gridSize.y; y++)
					{
						//UnityEngine.Debug.Log($"{x}, {y}");
						float3 worldPos = new float3(newCellDiameter * x + newCellRadius, 0, newCellDiameter * y + newCellRadius);
						CellData newCellData = new CellData
						{
							worldPos = worldPos,
							gridIndex = new int2(x, y),
							cost = 1,
							bestCost = ushort.MaxValue,
							bestDirection = int2.zero,
							cellBlobData = csd
						};

						Entity newCell = commandBuffer.CreateEntity(cellArchetype);
						commandBuffer.SetComponent(newCell, newCellData);
						cellBuffer.Add(newCellData);
					}
				}
				commandBuffer.AddComponent<GenerateCostFieldTag>(entity);
			}).Run();
		}
	}
}