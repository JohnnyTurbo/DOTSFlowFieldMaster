using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldGridSystem : SystemBase
	{
		private EntityCommandBufferSystem _ecbSystem;
		private static EntityArchetype _cellArchetype;

		protected override void OnCreate()
		{
			_ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
			_cellArchetype = EntityManager.CreateArchetype(typeof(CellData));
		}

		protected override void OnUpdate()
		{
			var commandBuffer = _ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, int entityInQueryIndex, in NewFlowFieldData newFlowFieldData, in FlowFieldData flowFieldData) =>
			{
				

				DynamicBuffer<EntityBufferElement> buffer = newFlowFieldData.isExistingFlowField
					? GetBuffer<EntityBufferElement>(entity)
					: commandBuffer.AddBuffer<EntityBufferElement>(entity);
				DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();
				
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
						float3 worldPos = new float3(newCellDiameter * x + newCellRadius, 0, newCellDiameter * y + newCellRadius);
						byte newCost = CostFieldHelper.instance.EvaluateCost(worldPos, newCellRadius);
						CellData newCellData = new CellData
						{
							worldPos = worldPos,
							gridIndex = new int2(x, y),
							cost = newCost,
							bestCost = ushort.MaxValue,
							bestDirection = int2.zero,
							cellBlobData = csd
						};
						
						if (newFlowFieldData.isExistingFlowField)
						{
							int flatIndex = FlowFieldHelper.ToFlatIndex(new int2(x, y), gridSize.y);
							Entity existingCell = entityBuffer[flatIndex];
							commandBuffer.SetComponent(existingCell, newCellData);
							commandBuffer.AddComponent<AddToDebugTag>(existingCell);
						}
						else
						{
							Entity newCell = commandBuffer.CreateEntity(_cellArchetype);
							commandBuffer.SetComponent(newCell, newCellData);
							commandBuffer.AddComponent<AddToDebugTag>(newCell);
							entityBuffer.Add(newCell);
						}
					}
				}
				
				commandBuffer.RemoveComponent<NewFlowFieldData>(entity);
				commandBuffer.AddComponent<CalculateFlowFieldTag>(entity);

				int2 destinationIndex = FlowFieldHelper.GetCellIndexFromWorldPos(flowFieldData.clickedPos, gridSize, newCellDiameter);
				DestinationCellData newDestinationCellData = new DestinationCellData{ destinationIndex = destinationIndex};
				if (!newFlowFieldData.isExistingFlowField)
				{
					commandBuffer.AddComponent<DestinationCellData>(entity);
				}
				commandBuffer.SetComponent(entity, newDestinationCellData);
			}).Run();
		}
	}
}