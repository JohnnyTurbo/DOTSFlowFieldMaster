using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

namespace TMG.ECSFlowField
{
	public class InitializeFlowFieldSystem : SystemBase
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

			Entities.ForEach((Entity entity, int entityInQueryIndex, in NewFlowFieldTag newFlowFieldTag, in FlowFieldData flowFieldData) =>
			{
				commandBuffer.RemoveComponent<NewFlowFieldTag>(entity);

				//DynamicBuffer<GridCellBufferElement> buffer = commandBuffer.AddBuffer<GridCellBufferElement>(entity);
				//DynamicBuffer<CellData> cellBuffer = buffer.Reinterpret<CellData>();
				
				DynamicBuffer<EntityBufferElement> buffer = commandBuffer.AddBuffer<EntityBufferElement>(entity);
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
						//UnityEngine.Debug.Log($"{x}, {y}");
						float3 worldPos = new float3(newCellDiameter * x + newCellRadius, 0, newCellDiameter * y + newCellRadius);
						byte newCost = ECSCostFieldHelper.instance.EvaluateCost(worldPos, newCellRadius);
						CellData newCellData = new CellData
						{
							worldPos = worldPos,
							gridIndex = new int2(x, y),
							cost = newCost,
							bestCost = ushort.MaxValue,
							bestDirection = int2.zero,
							cellBlobData = csd
						};

						Entity newCell = commandBuffer.CreateEntity(_cellArchetype);
						commandBuffer.SetComponent(newCell, newCellData);
						commandBuffer.AddComponent<AddToDebugTag>(newCell);
						entityBuffer.Add(newCell);
					}
				}
				commandBuffer.AddComponent<GenerateIntegrationFieldTag>(entity);

				float3 mousePos = flowFieldData.clickedPos;
				
				float percentX = mousePos.x / (gridSize.x * newCellDiameter);
				float percentY = mousePos.z / (gridSize.y * newCellDiameter);
			
				percentX = Mathf.Clamp01(percentX);
				percentY = Mathf.Clamp01(percentY);

				int xDest = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
				int yDest = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
				
				int2 destinationIndex = new int2(xDest,yDest);
				DestinationCellData newDestinationCellData = new DestinationCellData{ destinationIndex = destinationIndex};
				commandBuffer.AddComponent<DestinationCellData>(entity);
				commandBuffer.SetComponent(entity, newDestinationCellData);
				
			}).Run();
		}
	}
}