using Unity.Entities;
using Unity.Mathematics;

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

			Entities.ForEach((Entity entity, in NewFlowFieldData newFlowFieldData, in FlowFieldData flowFieldData) =>
			{
				commandBuffer.RemoveComponent<NewFlowFieldData>(entity);

				DynamicBuffer<EntityBufferElement> buffer = newFlowFieldData.isExistingFlowField
					? GetBuffer<EntityBufferElement>(entity)
					: commandBuffer.AddBuffer<EntityBufferElement>(entity);
				DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();
				
				float cellRadius = flowFieldData.cellRadius;
				float cellDiameter = cellRadius * 2;

				int2 gridSize = flowFieldData.gridSize;

				for (int x = 0; x < gridSize.x; x++)
				{
					for (int y = 0; y < gridSize.y; y++)
					{
						float3 cellWorldPos = new float3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
						byte cellCost = CostFieldHelper.instance.EvaluateCost(cellWorldPos, cellRadius);
						CellData newCellData = new CellData
						{
							worldPos = cellWorldPos,
							gridIndex = new int2(x, y),
							cost = cellCost,
							bestCost = ushort.MaxValue,
							bestDirection = int2.zero
						};

						Entity curCell;
						if (newFlowFieldData.isExistingFlowField)
						{
							int flatIndex = FlowFieldHelper.ToFlatIndex(new int2(x, y), gridSize.y);
							curCell = entityBuffer[flatIndex];
						}
						else
						{
							curCell = commandBuffer.CreateEntity(_cellArchetype);
							entityBuffer.Add(curCell);
						}
						commandBuffer.SetComponent(curCell, newCellData);
					}
				}

				int2 destinationIndex = FlowFieldHelper.GetCellIndexFromWorldPos(flowFieldData.clickedPos, gridSize, cellDiameter);
				DestinationCellData newDestinationCellData = new DestinationCellData{ destinationIndex = destinationIndex};
				if (!newFlowFieldData.isExistingFlowField)
				{
					commandBuffer.AddComponent<DestinationCellData>(entity);
				}
				commandBuffer.SetComponent(entity, newDestinationCellData);
				commandBuffer.AddComponent<CalculateFlowFieldTag>(entity);
			}).Run();
		}
	}
}